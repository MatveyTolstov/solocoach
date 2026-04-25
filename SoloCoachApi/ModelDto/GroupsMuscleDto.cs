using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class GroupsMuscleDto
    {
        public int IdGroupsMuscle { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
    }
}
