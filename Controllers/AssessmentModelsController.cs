using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Models;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentModelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssessmentModelsController(AppDbContext context)
        {
            _context = context;
        }

        

        // GET: api/AssessmentModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> GetAssessments()
        {
            var assessments = await _context.AssessmentModels.ToListAsync();

            var assessmentDtos = assessments.Select(a => new AssessmentDto
            {
                AssessmentId = a.AssessmentId,
                CourseId = a.CourseId,
                Title = a.Title,
                Questions = a.Questions,
                MaxScore = a.MaxScore
            });

            return Ok(assessmentDtos);
        }

        // GET: api/AssessmentModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentDto>> GetAssessment(Guid id)
        {
            var a = await _context.AssessmentModels.FindAsync(id);

            if (a == null)
                return NotFound();

            var dto = new AssessmentDto
            {
                AssessmentId = a.AssessmentId,
                CourseId = a.CourseId,
                Title = a.Title,
                Questions = a.Questions,
                MaxScore = a.MaxScore
            };

            return Ok(dto);
        }
        // GET: api/AssessmentModels/ByCourse/{courseId}
        [HttpGet("ByCourse/{courseId}")]
        public async Task<ActionResult<AssessmentModel>> GetAssessmentByCourseId(Guid courseId)
        {
            var assessment = await _context.AssessmentModels
                                           .FirstOrDefaultAsync(a => a.CourseId == courseId);

            if (assessment == null)
            {
                return NotFound();
            }

            return assessment;
        }

        // POST: api/AssessmentModels
        [HttpPost]
        public async Task<ActionResult<AssessmentDto>> CreateAssessment([FromBody] CreateAssessmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate if the course exists
            var courseExists = await _context.CourseModels.AnyAsync(c => c.CourseId == dto.CourseId);
            if (!courseExists)
                return BadRequest("Invalid CourseId: Course does not exist.");

            var assessment = new AssessmentModel
            {
                AssessmentId = Guid.NewGuid(),
                CourseId = dto.CourseId,
                Title = dto.Title,
                Questions = dto.Questions,
                MaxScore = dto.MaxScore
            };

            _context.AssessmentModels.Add(assessment);
            await _context.SaveChangesAsync();

            var createdDto = new AssessmentDto
            {
                AssessmentId = assessment.AssessmentId,
                CourseId = assessment.CourseId,
                Title = assessment.Title,
                Questions = assessment.Questions,
                MaxScore = assessment.MaxScore
            };

            return CreatedAtAction(nameof(GetAssessment), new { id = assessment.AssessmentId }, createdDto);
        }

        // PUT: api/AssessmentModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssessment(Guid id, [FromBody] CreateAssessmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate if the course exists
            var courseExists = await _context.CourseModels.AnyAsync(c => c.CourseId == dto.CourseId);
            if (!courseExists)
                return BadRequest("Invalid CourseId: Course does not exist.");

            var assessment = await _context.AssessmentModels.FindAsync(id);
            if (assessment == null)
                return NotFound();

            assessment.CourseId = dto.CourseId;
            assessment.Title = dto.Title;
            assessment.Questions = dto.Questions;
            assessment.MaxScore = dto.MaxScore;

            _context.Entry(assessment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/AssessmentModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessment(Guid id)
        {
            var assessment = await _context.AssessmentModels.FindAsync(id);
            if (assessment == null)
                return NotFound();

            _context.AssessmentModels.Remove(assessment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}













