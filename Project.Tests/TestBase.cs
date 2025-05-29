using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Project.Data;
using Project.Models;
using System;
using System.Collections.Generic;

namespace Project.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected AppDbContext _context;

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            // This runs once before all tests in the derived class
        }

        [SetUp]
        public virtual void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            SeedTestData();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        protected virtual void SeedTestData()
        {
            // Add sample user
            var user = new UserModel
            {
                UserId = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                Role = "User",
                PasswordHash = "hashedPassword123"
            };
            _context.UserModels.Add(user);

            // Add sample course
            var course = new CourseModel
            {
                CourseId = Guid.NewGuid(),
                Title = "Test Course",
                Description = "Test Description",
                UserId = user.UserId,
                MediaUrl = "https://example.com/test-course-media.mp4"
            };
            _context.CourseModels.Add(course);

            // Add sample assessment
            var assessment = new AssessmentModel
            {
                AssessmentId = Guid.NewGuid(),
                Title = "Test Assessment",
                CourseId = course.CourseId,
                MaxScore = 100,
                Questions = "[\"What is dependency injection?\", \"Explain SOLID principles.\"]"
            };
            _context.AssessmentModels.Add(assessment);

            // Add sample result
            var result = new ResultModel
            {
                ResultId = Guid.NewGuid(),
                AssessmentId = assessment.AssessmentId,
                UserId = user.UserId,
                Score = 85,
                AttemptDate = DateTime.UtcNow
            };
            _context.ResultModels.Add(result);

            _context.SaveChanges();
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            // This runs once after all tests in the derived class
        }
    }
} 