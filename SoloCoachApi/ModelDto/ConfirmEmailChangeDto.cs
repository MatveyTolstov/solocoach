using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class ConfirmEmailChangeDto
    {
        [MaxLength(6)]
        public string Code { get; set; } = null!;
    }
}
