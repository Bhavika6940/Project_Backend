using System.ComponentModel.DataAnnotations;
namespace Project.DTOs
{
    public class CreateAssessmentDto
    {
        [Required]
        public Guid CourseId { get; set; }

        public string Title { get; set; }

        [Required]
        public string Questions { get; set; } // As JSON string

        [Required]
        public int MaxScore { get; set; }
    }

    public class AssessmentDto
    {
        public Guid AssessmentId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Questions { get; set; }
        public int MaxScore { get; set; }
    }
}
