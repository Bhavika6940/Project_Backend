using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Project.Controllers;
using Project.Models;
using Project.DTOs;
using Project.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Project.Tests.Controllers
{
    public class ResultModelsControllerTests : TestBase
    {
        private ResultModelsController _controller;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // This runs once before all tests
        }

        [SetUp]
        public void Setup()
        {
            _controller = new ResultModelsController(_context);
        }

        [Test]
        public async Task GetResults_ReturnsAllResults()
        {
            // Act
            var result = await _controller.GetResults();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var results = okResult.Value as IEnumerable<ResultDto>;
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().Score, Is.EqualTo(85));
        }

        [Test]
        public async Task GetResult_WithValidId_ReturnsResult()
        {
            // Arrange
            var existingResult = await _context.ResultModels.FirstAsync();

            // Act
            var result = await _controller.GetResult(existingResult.ResultId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var resultDto = okResult.Value as ResultDto;
            Assert.That(resultDto.Score, Is.EqualTo(85));
        }

        [Test]
        public async Task GetResult_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetResult(Guid.NewGuid());

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateResult_WithValidData_ReturnsResult()
        {
            // Arrange
            var assessment = await _context.AssessmentModels.FirstAsync();
            var user = await _context.UserModels.FirstAsync();
            
            var newResult = new CreateResultDto
            {
                AssessmentId = assessment.AssessmentId,
                UserId = user.UserId,
                Score = 90,
                AttemptDate = DateTime.UtcNow
            };

            // Act
            var result = await _controller.CreateResult(newResult);

            // Assert
            Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            var resultDto = createdResult.Value as ResultDto;
            Assert.That(resultDto.Score, Is.EqualTo(90));
        }

        [Test]
        public async Task CreateResult_WithInvalidAssessmentId_ReturnsBadRequest()
        {
            // Arrange
            var invalidAssessmentId = Guid.NewGuid();
            var user = _context.UserModels.First();
            var newResult = new CreateResultDto
            {
                AssessmentId = invalidAssessmentId,
                UserId = user.UserId,
                Score = 90,
                AttemptDate = DateTime.UtcNow
            };

            // Act
            var result = await _controller.CreateResult(newResult);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value.ToString(), Contains.Substring("Invalid AssessmentId"));
        }

        [Test]
        public async Task CreateResult_WithInvalidUserId_ReturnsBadRequest()
        {
            // Arrange
            var invalidUserId = Guid.NewGuid();
            var assessment = _context.AssessmentModels.First();
            var newResult = new CreateResultDto
            {
                AssessmentId = assessment.AssessmentId,
                UserId = invalidUserId,
                Score = 90,
                AttemptDate = DateTime.UtcNow
            };

            // Act
            var result = await _controller.CreateResult(newResult);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value.ToString(), Contains.Substring("Invalid UserId"));
        }

        [Test]
        public async Task GetResultByAssessmentAndUser_WithInvalidIds_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetResultByAssessmentAndUser(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateResult_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var assessment = await _context.AssessmentModels.FirstAsync();
            var user = await _context.UserModels.FirstAsync();
            var updateDto = new CreateResultDto
            {
                AssessmentId = assessment.AssessmentId,
                UserId = user.UserId,
                Score = 95,
                AttemptDate = DateTime.UtcNow
            };

            // Act
            var result = await _controller.UpdateResult(Guid.NewGuid(), updateDto);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteResult_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteResult(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // This runs once after all tests
        }
    }
} 