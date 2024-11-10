using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Controllers;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;
using ReimbursementTrackingApplication.Services;

namespace ReimbursementUnitProjectTest.Controllers
{
    internal class UserControllersTest
    {
        DbContextOptions options;
        ContextApp context;
        IRepository<int, User> repository;
        IUserServices userServices;
        UserController userController;
        Mock<ILogger<UserRepository>> repoLogger;
        Mock<ILogger<UserService>> userServiceLogger;
        Mock<IMapper> mapper;
        Mock<TokenService> mockTokenService;
        Mock<IConfiguration> mockConfiguration;




        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            repoLogger = new Mock<ILogger<UserRepository>>();
            userServiceLogger = new Mock<ILogger<UserService>>();
            mapper = new Mock<IMapper>();
            mockConfiguration = new Mock<IConfiguration>();
            mockTokenService = new Mock<TokenService>(mockConfiguration.Object);
            repository = new UserRepository(context, repoLogger.Object);
            userServices = new UserService(repository, mapper.Object, userServiceLogger.Object, mockTokenService.Object);
            userController = new UserController(userServices);
        }
        [Test]
        [TestCase("TestUser2", "TestPassword2", "laishramrohit15@gmail.com", "TestHashKey", Departments.Admin)]
        public async Task RegisterUserTest(string username, string password, string email, string hashKey, Departments department)
        {
            var user = new UserCreateDTO
            {
                UserName = username,
                Password = password,
                Email = email,
                Department = department
            };

            var result = await userController.Register(user);
            Assert.IsNotNull(result);
            var resultObject = result.Result as OkObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(200, resultObject.StatusCode);

        }
        [Test]
        [TestCase("TestUser2", "testPassword", "laishramrohit15@gmail.com", "TestHashKey", Departments.Admin)]
        public async Task RegisterUserTestException(string username, string password, string email, string hashKey, Departments department)
        {
            
            var user = new UserCreateDTO
            {
                UserName = username,
                Password = password,
                Email = email,
                Department = department
            };

            
            userController.ModelState.AddModelError("Email", "Invalid email format");

     
            Assert.ThrowsAsync<Exception>(async() => await userController.Register(user));

        }

    }
}
