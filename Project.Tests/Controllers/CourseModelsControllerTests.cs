using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NUnit.Framework;
using Project.Controllers;
using Project.Models;
using Project.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Project.Tests.Controllers
{
    [TestFixture]
    public class CourseModelsControllerTests : TestBase
    {
        private CourseModelsController _controller;
        [SetUp]
        public void SetupController()
        {
            var storageConfig = Options.Create(new AzureStorageConfig
            {
                ConnectionString = "UseDevelopmentStorage=true",  // Use Azure Storage Emulator for tests
                ContainerName = "test-container"
            });

            _controller = new CourseModelsController(_context, storageConfig);
        }

        [Test]
        public async Task GetCourses_ReturnsAllCourses()
        {
            // Act
            var result = await _controller.GetCourses();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var courses = okResult.Value as IEnumerable<CourseDto>;
            Assert.That(courses.Count(), Is.EqualTo(1));
            Assert.That(courses.First().Title, Is.EqualTo("Test Course"));
        }

        [Test]
        public async Task GetCourse_WithValidId_ReturnsCourse()
        {
            // Arrange
            var existingCourse = _context.CourseModels.First();

            // Act
            var result = await _controller.GetCourse(existingCourse.CourseId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var course = okResult.Value as CourseDto;
            Assert.That(course.Title, Is.EqualTo("Test Course"));
        }

        [Test]
        public async Task GetCourse_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetCourse(Guid.NewGuid());

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }

    

        [Test]
        public async Task CreateCourse_WithInvalidUserId_ReturnsBadRequest()
        {
            // Arrange
            var invalidUserId = Guid.NewGuid();
            var newCourse = new CreateCourseDto
            {
                Title = "New Course",
                Description = "New Description",
                UserId = invalidUserId,
                MediaUrl = "https://example.com/media.mp4"
            };

            // Act
            var result = await _controller.CreateCourse(newCourse);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value.ToString(), Is.EqualTo("Invalid UserId: User does not exist."));
        }

        [Test]
        public async Task CreateCourse_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var newCourse = new CreateCourseDto
            {
                Title = "Test Title",  // Providing required Title
                Description = "Test Description",
                UserId = _context.UserModels.First().UserId,
                MediaUrl = "https://example.com/test.mp4"  // Providing required MediaUrl
            };

            // Invalidate ModelState manually to simulate invalid data
            _controller.ModelState.AddModelError("Title", "Title is invalid");

            // Act
            var result = await _controller.CreateCourse(newCourse);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateCourse_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new CreateCourseDto
            {
                Title = "Updated Course",
                Description = "Updated Description",
                UserId = _context.UserModels.First().UserId,
                MediaUrl = "https://example.com/updated-media.mp4"
            };

            // Act
            var result = await _controller.UpdateCourse(Guid.NewGuid(), updateDto);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteCourse_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteCourse(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
} 