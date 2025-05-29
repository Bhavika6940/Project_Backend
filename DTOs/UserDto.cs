using System.ComponentModel.DataAnnotations;
namespace Project.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } // "Student" or "Instructor"

        [Required]
        public string Password { get; set; }
    }

    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
