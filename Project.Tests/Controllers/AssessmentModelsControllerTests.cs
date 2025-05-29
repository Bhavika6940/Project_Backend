using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Project.Controllers;
using Project.Models;
using Project.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Project.Tests.Controllers
{
    [TestFixture]
    public class AssessmentModelsControllerTests : TestBase
    {
        private AssessmentModelsController _controller;

        [SetUp]
        public void SetupController()
        {
            _controller = new AssessmentModelsController(_context);
        }

        [Test]
        public async Task GetAssessments_ReturnsAllAssessments()
        {
            // Act
            var result = await _controller.GetAssessments();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var assessments = okResult.Value as IEnumerable<AssessmentDto>;
            Assert.That(assessments.Count(), Is.EqualTo(1));
            Assert.That(assessments.First().Title, Is.EqualTo("Test Assessment"));
        }

        [Test]
        public async Task GetAssessment_WithValidId_ReturnsAssessment()
        {
            // Arrange
            var existingAssessment = _context.AssessmentModels.First();

            // Act
            var result = await _controller.GetAssessment(existingAssessment.AssessmentId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var assessment = okResult.Value as AssessmentDto;
            Assert.That(assessment.Title, Is.EqualTo("Test Assessment"));
        }

        [Test]
        public async Task GetAssessment_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetAssessment(Guid.NewGuid());

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateAssessment_WithValidData_ReturnsAssessment()
        {
            // Arrange
            var newAssessment = new CreateAssessmentDto
            {
                Title = "New Assessment",
                CourseId = _context.CourseModels.First().CourseId,
                MaxScore = 100,
                Questions = "[\"What is unit testing?\", \"Explain test-driven development.\"]"
            };

            // Act
            var result = await _controller.CreateAssessment(newAssessment);

            // Assert
            Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            var assessment = createdResult.Value as AssessmentDto;
            Assert.That(assessment.Title, Is.EqualTo("New Assessment"));
        }

        [Test]
        public async Task CreateAssessment_WithInvalidCourseId_ReturnsBadRequest()
        {
            // Arrange
            var newAssessment = new CreateAssessmentDto
            {
                Title = "New Assessment",
                CourseId = Guid.NewGuid(), // Invalid CourseId
                MaxScore = 100,
                Questions = "[\"What is unit testing?\"]"
            };

            // Act
            var result = await _controller.CreateAssessment(newAssessment);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value.ToString(), Is.EqualTo("Invalid CourseId: Course does not exist."));
        }

        [Test]
        public async Task CreateAssessment_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var newAssessment = new CreateAssessmentDto
            {
                Title = "",
                CourseId = _context.CourseModels.First().CourseId,
                MaxScore = -1,
                Questions = ""
            };

            // Invalidate ModelState manually
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.CreateAssessment(newAssessment);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetAssessmentByCourse_WithInvalidCourseId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetAssessmentByCourseId(Guid.NewGuid());

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }
    }
} 