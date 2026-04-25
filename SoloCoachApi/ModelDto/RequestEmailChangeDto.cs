using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class RequestEmailChangeDto
    {
        [EmailAddress]
        public string NewEmail { get; set; } = null!;
    }
}
