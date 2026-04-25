using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoloCoachApi.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise",
                columns: table => new
                {
                    id_exercise = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    complexity = table.Column<string>(type: "text", nullable: true),
                    picture_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise", x => x.id_exercise);
                });

            migrationBuilder.CreateTable(
                name: "goals",
                columns: table => new
                {
                    id_goal = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_goal = table.Column<string>(type: "text", nullable: false),
                    target_weight = table.Column<float>(type: "real", nullable: false),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goals", x => x.id_goal);
                });

            migrationBuilder.CreateTable(
                name: "groups_muscles",
                columns: table => new
                {
                    id_groups_muscle = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups_muscles", x => x.id_groups_muscle);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_role = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_role);
                });

            migrationBuilder.CreateTable(
                name: "workouts",
                columns: table => new
                {
                    id_workout = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    complexity = table.Column<string>(type: "text", nullable: false),
                    type_workout = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workouts", x => x.id_workout);
                });

            migrationBuilder.CreateTable(
                name: "metrics_users",
                columns: table => new
                {
                    id_metrics_user = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    height = table.Column<float>(type: "real", nullable: false),
                    weight = table.Column<float>(type: "real", nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: false),
                    experience_level = table.Column<string>(type: "text", nullable: true),
                    activity_level = table.Column<string>(type: "text", nullable: true),
                    GoalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metrics_users", x => x.id_metrics_user);
                    table.ForeignKey(
                        name: "FK_metrics_users_goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "goals",
                        principalColumn: "id_goal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exercise_groups_muscles",
                columns: table => new
                {
                    id_exercise_groups_muscle = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    exercise_id = table.Column<int>(type: "integer", nullable: false),
                    groups_muscles_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_groups_muscles", x => x.id_exercise_groups_muscle);
                    table.ForeignKey(
                        name: "FK_exercise_groups_muscles_exercise_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercise",
                        principalColumn: "id_exercise",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_groups_muscles_groups_muscles_groups_muscles_id",
                        column: x => x.groups_muscles_id,
                        principalTable: "groups_muscles",
                        principalColumn: "id_groups_muscle",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_exercises",
                columns: table => new
                {
                    id_training_exercise = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workout_id = table.Column<int>(type: "integer", nullable: false),
                    exercise_id = table.Column<int>(type: "integer", nullable: false),
                    execution_order = table.Column<int>(type: "integer", nullable: false),
                    repetitions = table.Column<int>(type: "integer", nullable: false),
                    sets = table.Column<int>(type: "integer", nullable: false),
                    rest_time = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_exercises", x => x.id_training_exercise);
                    table.ForeignKey(
                        name: "FK_training_exercises_exercise_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercise",
                        principalColumn: "id_exercise",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_training_exercises_workouts_workout_id",
                        column: x => x.workout_id,
                        principalTable: "workouts",
                        principalColumn: "id_workout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    metrics_user_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id_user);
                    table.ForeignKey(
                        name: "FK_users_metrics_users_metrics_user_id",
                        column: x => x.metrics_user_id,
                        principalTable: "metrics_users",
                        principalColumn: "id_metrics_user");
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id_role",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_workouts",
                columns: table => new
                {
                    id_plan_workouts = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    workout_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_workouts", x => x.id_plan_workouts);
                    table.ForeignKey(
                        name: "FK_plan_workouts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plan_workouts_workouts_workout_id",
                        column: x => x.workout_id,
                        principalTable: "workouts",
                        principalColumn: "id_workout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_calendars",
                columns: table => new
                {
                    id_workout_calendar = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    workout_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_calendars", x => x.id_workout_calendar);
                    table.ForeignKey(
                        name: "FK_workout_calendars_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workout_calendars_workouts_workout_id",
                        column: x => x.workout_id,
                        principalTable: "workouts",
                        principalColumn: "id_workout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_users",
                columns: table => new
                {
                    id_workout_users = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    workout_id = table.Column<int>(type: "integer", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_users", x => x.id_workout_users);
                    table.ForeignKey(
                        name: "FK_workout_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workout_users_workouts_workout_id",
                        column: x => x.workout_id,
                        principalTable: "workouts",
                        principalColumn: "id_workout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_user_logs",
                columns: table => new
                {
                    id_workout_user_log = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workout_user_id = table.Column<int>(type: "integer", nullable: false),
                    workout_id = table.Column<int>(type: "integer", nullable: false),
                    repetitions = table.Column<int>(type: "integer", nullable: false),
                    sets = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_user_logs", x => x.id_workout_user_log);
                    table.ForeignKey(
                        name: "FK_workout_user_logs_workout_users_workout_user_id",
                        column: x => x.workout_user_id,
                        principalTable: "workout_users",
                        principalColumn: "id_workout_users",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workout_user_logs_workouts_workout_id",
                        column: x => x.workout_id,
                        principalTable: "workouts",
                        principalColumn: "id_workout",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_groups_muscles_exercise_id",
                table: "exercise_groups_muscles",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_groups_muscles_groups_muscles_id",
                table: "exercise_groups_muscles",
                column: "groups_muscles_id");

            migrationBuilder.CreateIndex(
                name: "IX_metrics_users_GoalId",
                table: "metrics_users",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_plan_workouts_user_id",
                table: "plan_workouts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_plan_workouts_workout_id",
                table: "plan_workouts",
                column: "workout_id");

            migrationBuilder.CreateIndex(
                name: "IX_training_exercises_exercise_id",
                table: "training_exercises",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_training_exercises_workout_id",
                table: "training_exercises",
                column: "workout_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_metrics_user_id",
                table: "users",
                column: "metrics_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_calendars_user_id",
                table: "workout_calendars",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_calendars_workout_id",
                table: "workout_calendars",
                column: "workout_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_user_logs_workout_id",
                table: "workout_user_logs",
                column: "workout_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_user_logs_workout_user_id",
                table: "workout_user_logs",
                column: "workout_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_users_user_id",
                table: "workout_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_users_workout_id",
                table: "workout_users",
                column: "workout_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_groups_muscles");

            migrationBuilder.DropTable(
                name: "plan_workouts");

            migrationBuilder.DropTable(
                name: "training_exercises");

            migrationBuilder.DropTable(
                name: "workout_calendars");

            migrationBuilder.DropTable(
                name: "workout_user_logs");

            migrationBuilder.DropTable(
                name: "groups_muscles");

            migrationBuilder.DropTable(
                name: "exercise");

            migrationBuilder.DropTable(
                name: "workout_users");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "workouts");

            migrationBuilder.DropTable(
                name: "metrics_users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "goals");
        }
    }
}
