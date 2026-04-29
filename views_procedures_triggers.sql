-- ============================================================
-- Представления (Views)
-- ============================================================

-- 1. Полный профиль пользователя с метриками и целью
CREATE OR REPLACE VIEW public.v_user_profile AS
SELECT
    u.id_user,
    u.name,
    u.login,
    u.email,
    r.role_name,
    m.height,
    m.weight,
    m.age,
    m.gender,
    m.experience_level,
    m.activity_level,
    g.type_goal,
    g.target_weight,
    g.date_start,
    g.date_end,
    g.status AS goal_status
FROM public.users u
JOIN public.roles r ON r.id_role = u.role_id
LEFT JOIN public.metrics_users m ON m.id_metrics_user = u.metrics_user_id
LEFT JOIN public.goals g ON g.id_goal = m."GoalId";


-- 2. Детали тренировки — упражнения, группы мышц, порядок выполнения
CREATE OR REPLACE VIEW public.v_workout_details AS
SELECT
    w.id_workout,
    w.name        AS workout_name,
    w.complexity  AS workout_complexity,
    w.type_workout,
    w.duration    AS workout_duration,
    te.execution_order,
    e.id_exercise,
    e.name        AS exercise_name,
    e.complexity  AS exercise_complexity,
    te.sets,
    te.repetitions,
    te.rest_time,
    string_agg(gm.name, ', ' ORDER BY gm.name) AS muscle_groups
FROM public.workouts w
JOIN public.training_exercises te ON te.workout_id = w.id_workout
JOIN public.exercise e ON e.id_exercise = te.exercise_id
LEFT JOIN public.exercise_groups_muscles egm ON egm.exercise_id = e.id_exercise
LEFT JOIN public.groups_muscles gm ON gm.id_groups_muscle = egm.groups_muscles_id
GROUP BY w.id_workout, w.name, w.complexity, w.type_workout, w.duration,
         te.execution_order, e.id_exercise, e.name, e.complexity,
         te.sets, te.repetitions, te.rest_time;


-- 3. Статистика тренировок по пользователям
CREATE OR REPLACE VIEW public.v_user_workout_stats AS
SELECT
    u.id_user,
    u.name,
    u.login,
    COUNT(wu.id_workout_users)                          AS total_sessions,
    SUM(wu.duration)                                    AS total_minutes,
    ROUND(AVG(wu.duration)::numeric, 1)                 AS avg_duration,
    MAX(wu.date)                                        AS last_workout_date,
    COUNT(DISTINCT wu.workout_id)                       AS unique_workouts
FROM public.users u
LEFT JOIN public.workout_users wu ON wu.user_id = u.id_user
GROUP BY u.id_user, u.name, u.login;


-- ============================================================
-- Функции (Functions)
-- ============================================================

-- 1. Количество завершённых тренировок пользователя
CREATE OR REPLACE FUNCTION public.get_user_workout_count(p_user_id integer)
RETURNS integer
LANGUAGE sql
STABLE
AS $$
    SELECT COUNT(*)::integer
    FROM public.workout_users
    WHERE user_id = p_user_id;
$$;


-- 2. Общий объём нагрузки сессии (сумма sets * repetitions * weight по всем упражнениям)
CREATE OR REPLACE FUNCTION public.get_session_volume(p_workout_user_id integer)
RETURNS numeric
LANGUAGE sql
STABLE
AS $$
    SELECT COALESCE(SUM(sets * repetitions * weight), 0)
    FROM public.workout_user_logs
    WHERE workout_user_id = p_workout_user_id;
$$;


-- 3. Вычислить ИМТ (индекс массы тела) по росту (см) и весу (кг)
CREATE OR REPLACE FUNCTION public.calculate_bmi(p_height_cm real, p_weight_kg real)
RETURNS numeric
LANGUAGE sql
IMMUTABLE
AS $$
    SELECT ROUND((p_weight_kg / POWER(p_height_cm / 100.0, 2))::numeric, 2);
$$;


-- ============================================================
-- Хранимые процедуры (Stored Procedures)
-- ============================================================

-- 1. Завершить сессию тренировки: записать workout_users и обновить статус в calendar
CREATE OR REPLACE PROCEDURE public.complete_workout_session(
    p_user_id    integer,
    p_workout_id integer,
    p_duration   integer
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_session_id integer;
BEGIN
    INSERT INTO public.workout_users (user_id, workout_id, duration, date, status)
    VALUES (p_user_id, p_workout_id, p_duration, NOW(), 'Завершена')
    RETURNING id_workout_users INTO v_session_id;

    UPDATE public.workout_calendars
    SET status = 'Выполнена'
    WHERE user_id    = p_user_id
      AND workout_id = p_workout_id
      AND status     = 'Запланирована'
      AND date::date = CURRENT_DATE;

    INSERT INTO public.application_logs (action, entity_type, entity_id, user_id, details, created_at, status)
    VALUES ('COMPLETE_WORKOUT_SESSION', 'WorkoutSession', v_session_id, p_user_id,
            json_build_object('workoutId', p_workout_id, 'duration', p_duration)::text,
            NOW(), 'SUCCESS');
END;
$$;


-- 2. Назначить тренировку пользователю в календарь (без дублей на ту же дату)
CREATE OR REPLACE PROCEDURE public.schedule_workout(
    p_user_id    integer,
    p_workout_id integer,
    p_date       timestamp with time zone
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM public.workout_calendars
        WHERE user_id = p_user_id AND workout_id = p_workout_id
          AND date::date = p_date::date
    ) THEN
        RAISE EXCEPTION 'Тренировка уже запланирована на эту дату';
    END IF;

    INSERT INTO public.workout_calendars (user_id, workout_id, status, date)
    VALUES (p_user_id, p_workout_id, 'Запланирована', p_date);
END;
$$;


-- 3. Обновить статус просроченных целей на «Завершена»
CREATE OR REPLACE PROCEDURE public.expire_overdue_goals()
LANGUAGE plpgsql
AS $$
DECLARE
    v_count integer;
BEGIN
    UPDATE public.goals
    SET status = 'Завершена'
    WHERE date_end < NOW()
      AND status NOT IN ('Завершена', 'Отменена');

    GET DIAGNOSTICS v_count = ROW_COUNT;

    IF v_count > 0 THEN
        INSERT INTO public.application_logs (action, entity_type, entity_id, user_id, details, created_at, status)
        VALUES ('EXPIRE_GOALS', 'Goal', NULL, NULL,
                json_build_object('expiredCount', v_count)::text,
                NOW(), 'SUCCESS');
    END IF;
END;
$$;


-- ============================================================
-- Триггеры (Triggers)
-- ============================================================

-- 1. Логировать изменения имени / email пользователя
CREATE OR REPLACE FUNCTION public.trg_fn_log_user_update()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF OLD.name <> NEW.name OR OLD.email <> NEW.email THEN
        INSERT INTO public.application_logs
            (action, entity_type, entity_id, user_id, details, created_at, status)
        VALUES (
            'UPDATE_USER',
            'User',
            NEW.id_user,
            NEW.id_user,
            json_build_object(
                'oldName',  OLD.name,  'newName',  NEW.name,
                'oldEmail', OLD.email, 'newEmail', NEW.email
            )::text,
            NOW(),
            'SUCCESS'
        );
    END IF;
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_log_user_update ON public.users;
CREATE TRIGGER trg_log_user_update
AFTER UPDATE ON public.users
FOR EACH ROW EXECUTE FUNCTION public.trg_fn_log_user_update();


-- 2. Запретить удаление тренировки, если есть завершённые сессии пользователей
CREATE OR REPLACE FUNCTION public.trg_fn_protect_workout_delete()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM public.workout_users
        WHERE workout_id = OLD.id_workout AND status = 'Завершена'
    ) THEN
        RAISE EXCEPTION 'Нельзя удалить тренировку: есть завершённые сессии (workout_id=%)', OLD.id_workout;
    END IF;
    RETURN OLD;
END;
$$;

DROP TRIGGER IF EXISTS trg_protect_workout_delete ON public.workouts;
CREATE TRIGGER trg_protect_workout_delete
BEFORE DELETE ON public.workouts
FOR EACH ROW EXECUTE FUNCTION public.trg_fn_protect_workout_delete();


-- 3. Автоматически проставить дату начала цели при вставке, если не указана
CREATE OR REPLACE FUNCTION public.trg_fn_goal_default_dates()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF NEW.date_start IS NULL THEN
        NEW.date_start := NOW();
    END IF;
    IF NEW.status IS NULL OR NEW.status = '' THEN
        NEW.status := 'Активна';
    END IF;
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_goal_default_dates ON public.goals;
CREATE TRIGGER trg_goal_default_dates
BEFORE INSERT ON public.goals
FOR EACH ROW EXECUTE FUNCTION public.trg_fn_goal_default_dates();
