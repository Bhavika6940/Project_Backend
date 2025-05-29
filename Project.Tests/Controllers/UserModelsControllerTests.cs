using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Project.Controllers;
using Project.Models;
using Project.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Tests.Controllers
{
    [TestFixture]
    public class UserModelsControllerTests : TestBase
    {
        private UserModelsController _controller;

        [SetUp]
        public void SetupController()
        {
            _controller = new UserModelsController(_context);
        }

        [Test]
        public async Task GetUsers_ReturnsAllUsers()
        {
            // Act
            var result = await _controller.GetUsers();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var users = okResult.Value as IEnumerable<UserDto>;
            Assert.That(users.Count(), Is.EqualTo(1));
            Assert.That(users.First().Name, Is.EqualTo("Test User"));
        }

        [Test]
        public async Task GetUser_WithValidId_ReturnsUser()
        {
            // Arrange
            var existingUser = _context.UserModels.First();

            // Act
            var result = await _controller.GetUser(existingUser.UserId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var user = okResult.Value as UserDto;
            Assert.That(user.Name, Is.EqualTo("Test User"));
        }

        [Test]
        public async Task CreateUser_WithValidData_ReturnsUser()
        {
            // Arrange
            var newUser = new CreateUserDto
            {
                Name = "New User",
                Email = "newuser@example.com",
                Password = "Password123!",
                Role = "Student"
            };

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            var user = createdResult.Value as UserDto;
            Assert.That(user.Name, Is.EqualTo("New User"));
            Assert.That(user.Email, Is.EqualTo("newuser@example.com"));
            Assert.That(user.Role, Is.EqualTo("Student"));
        }

        [Test]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "hashedPassword123"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            dynamic response = okResult.Value;
            Assert.That(response.token, Is.Not.Null);
            Assert.That(response.role, Is.EqualTo("User"));
        }
    }
} 