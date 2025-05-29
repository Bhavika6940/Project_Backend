using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;
[Table("UserModel")]
public partial class UserModel
{
    [Key]
    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<CourseModel> CourseModels { get; set; } = new List<CourseModel>();

    public virtual ICollection<ResultModel> ResultModels { get; set; } = new List<ResultModel>();
}
