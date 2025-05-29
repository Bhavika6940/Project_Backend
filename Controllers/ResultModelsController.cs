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
    public class ResultModelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ResultModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ResultModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetResults()
        {
            var results = await _context.ResultModels.ToListAsync();

            var resultDtos = results.Select(r => new ResultDto
            {
                ResultId = r.ResultId,
                AssessmentId = r.AssessmentId,
                UserId = r.UserId,
                Score = r.Score,
                AttemptDate = r.AttemptDate
            });

            return Ok(resultDtos);
        }

        // GET: api/ResultModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDto>> GetResult(Guid id)
        {
            var r = await _context.ResultModels.FindAsync(id);

            if (r == null)
                return NotFound();

            var dto = new ResultDto
            {
                ResultId = r.ResultId,
                AssessmentId = r.AssessmentId,
                UserId = r.UserId,
                Score = r.Score,
                AttemptDate = r.AttemptDate
            };

            return Ok(dto);
        }

        // POST: api/ResultModels
        [HttpPost]
        public async Task<ActionResult<ResultDto>> CreateResult([FromBody] CreateResultDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate AssessmentId exists
            var assessmentExists = await _context.AssessmentModels.AnyAsync(a => a.AssessmentId == dto.AssessmentId);
            if (!assessmentExists)
                return BadRequest(new { message = "Invalid AssessmentId" });

            // Validate UserId exists
            var userExists = await _context.UserModels.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                return BadRequest(new { message = "Invalid UserId" });

            var result = new ResultModel
            {
                ResultId = Guid.NewGuid(),
                AssessmentId = dto.AssessmentId,
                UserId = dto.UserId,
                Score = dto.Score,
                AttemptDate = dto.AttemptDate
            };

            _context.ResultModels.Add(result);
            await _context.SaveChangesAsync();

            var resultDto = new ResultDto
            {
                ResultId = result.ResultId,
                AssessmentId = result.AssessmentId,
                UserId = result.UserId,
                Score = result.Score,
                AttemptDate = result.AttemptDate
            };

            return CreatedAtAction(nameof(GetResult), new { id = result.ResultId }, resultDto);
        }

        // GET: api/ResultModel/ByAssessment/{assessmentId}/ByUser/{userId}
        [HttpGet("ByAssessment/{assessmentId}/ByUser/{userId}")]
        public async Task<ActionResult<ResultModel>> GetResultByAssessmentAndUser(Guid assessmentId, Guid userId)
        {
            var result = await _context.ResultModels
                .FirstOrDefaultAsync(r => r.AssessmentId == assessmentId && r.UserId == userId);

            if (result == null)
            {
                return NotFound(new { message = "Result not found for the specified assessment and user." });
            }

            return Ok(result);
        }


        // PUT: api/ResultModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(Guid id, [FromBody] CreateResultDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _context.ResultModels.FindAsync(id);
            if (result == null)
                return NotFound();

            result.AssessmentId = dto.AssessmentId;
            result.UserId = dto.UserId;
            result.Score = dto.Score;
            result.AttemptDate = dto.AttemptDate;

            _context.Entry(result).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ResultModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(Guid id)
        {
            var result = await _context.ResultModels.FindAsync(id);
            if (result == null)
                return NotFound();

            _context.ResultModels.Remove(result);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}




















