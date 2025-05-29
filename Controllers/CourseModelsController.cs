using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Models;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseModelsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IOptions<AzureStorageConfig> _storageConfig;

        public CourseModelsController(AppDbContext context, IOptions<AzureStorageConfig> storageConfig)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _storageConfig = storageConfig ?? throw new ArgumentNullException(nameof(storageConfig));
            
            if (string.IsNullOrEmpty(_storageConfig.Value.ConnectionString))
                throw new InvalidOperationException("Azure Storage ConnectionString is not configured");
            
            if (string.IsNullOrEmpty(_storageConfig.Value.ContainerName))
                throw new InvalidOperationException("Azure Storage ContainerName is not configured");
        }

        // GET: api/CourseModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _context.CourseModels.ToListAsync();

            var courseDtos = courses.Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                UserId = c.UserId,
                MediaUrl = c.MediaUrl
            });

            return Ok(courseDtos);
        }

        // GET: api/CourseModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
        {
            var c = await _context.CourseModels.FindAsync(id);

            if (c == null)
                return NotFound();

            var dto = new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                UserId = c.UserId,
                MediaUrl = c.MediaUrl
            };

            return Ok(dto);
        }

        // POST: api/CourseModels
        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate if the user exists
            var userExists = await _context.UserModels.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                return BadRequest("Invalid UserId: User does not exist.");

            var course = new CourseModel
            {
                CourseId = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                UserId = dto.UserId,
                MediaUrl = dto.MediaUrl
            };

            _context.CourseModels.Add(course);
            await _context.SaveChangesAsync();

            // 🔁 Push updated list to blob
            await UploadCoursesToBlob();

            var createdDto = new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                UserId = course.UserId,
                MediaUrl = course.MediaUrl
            };

            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, createdDto);
        }

        private async Task UploadCoursesToBlob()
        {
            var courses = await _context.CourseModels.ToListAsync();

            var courseDtos = courses.Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                UserId = c.UserId,
                MediaUrl = c.MediaUrl
            }).ToList();

            var jsonData = JsonSerializer.Serialize(courseDtos);
            var blobServiceClient = new BlobServiceClient(_storageConfig.Value.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_storageConfig.Value.ContainerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobName = $"courses-{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var blobClient = containerClient.GetBlobClient(blobName);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        // PUT: api/CourseModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CreateCourseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _context.CourseModels.FindAsync(id);
            if (course == null)
                return NotFound();

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.UserId = dto.UserId;
            course.MediaUrl = dto.MediaUrl;

            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CourseModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.CourseModels.FindAsync(id);
            if (course == null)
                return NotFound();

            _context.CourseModels.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
















//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Project.Data;
//using Project.DTOs;
//using Project.Models;

//namespace Project.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CourseModelsController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public CourseModelsController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/CourseModels
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
//        {
//            var courses = await _context.CourseModels.ToListAsync();

//            var courseDtos = courses.Select(c => new CourseDto
//            {
//                CourseId = c.CourseId,
//                Title = c.Title,
//                Description = c.Description,
//                UserId = c.UserId,
//                MediaUrl = c.MediaUrl

//            });

//            return Ok(courseDtos);
//        }

//        // GET: api/CourseModels/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
//        {
//            var c = await _context.CourseModels.FindAsync(id);

//            if (c == null)
//                return NotFound();

//            var dto = new CourseDto
//            {
//                CourseId = c.CourseId,
//                Title = c.Title,
//                Description = c.Description,
//                UserId = c.UserId,
//                MediaUrl = c.MediaUrl
//            };

//            return Ok(dto);
//        }

//        // POST: api/CourseModels
//        [HttpPost]
//        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var course = new CourseModel
//            {
//                CourseId = Guid.NewGuid(),
//                Title = dto.Title,
//                Description = dto.Description,
//                UserId = dto.UserId,
//                MediaUrl = dto.MediaUrl
//            };

//            _context.CourseModels.Add(course);
//            await _context.SaveChangesAsync();

//            var createdDto = new CourseDto
//            {
//                CourseId = course.CourseId,
//                Title = course.Title,
//                Description = course.Description,
//                UserId = course.UserId,
//                MediaUrl = course.MediaUrl
//            };

//            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, createdDto);
//        }

//        // PUT: api/CourseModels/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CreateCourseDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var course = await _context.CourseModels.FindAsync(id);
//            if (course == null)
//                return NotFound();

//            course.Title = dto.Title;
//            course.Description = dto.Description;
//            course.UserId = dto.UserId;
//            course.MediaUrl = dto.MediaUrl;

//            _context.Entry(course).State = EntityState.Modified;
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        // DELETE: api/CourseModels/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteCourse(Guid id)
//        {
//            var course = await _context.CourseModels.FindAsync(id);
//            if (course == null)
//                return NotFound();

//            _context.CourseModels.Remove(course);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
