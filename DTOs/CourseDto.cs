using System.ComponentModel.DataAnnotations;
namespace Project.DTOs
{
    public class CreateCourseDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public string MediaUrl { get; set; }
    }

    public class CourseDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public string MediaUrl { get; set; }
    }
}
