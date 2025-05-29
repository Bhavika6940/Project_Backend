using System.ComponentModel.DataAnnotations;
namespace Project.DTOs
{
    public class CreateResultDto
    {
        [Required]
        public Guid AssessmentId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int Score { get; set; }

        public DateTime AttemptDate { get; set; } = DateTime.UtcNow;
    }

    public class ResultDto
    {
        public Guid ResultId { get; set; }
        public Guid AssessmentId { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; }
    }

}
