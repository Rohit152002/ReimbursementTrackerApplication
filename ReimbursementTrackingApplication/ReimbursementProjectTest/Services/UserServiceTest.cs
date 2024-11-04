
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;
using ReimbursementTrackingApplication.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementProjectTest.Services
{
    internal class UserServiceTest
    {
        UserService service;
        DbContextOptions options;
        Mock<ILogger<UserService>> logger;
        Mock<IMapper> mapper;
        Mock<IRepository<int, User>> repository;
        Mock<IConfiguration> mockConfiguration;
        Mock<TokenService> mockTokenService;
        ContextApp context;
        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);

            logger = new Mock<ILogger<UserService>>();
            repository = new Mock<IRepository<int, User>>();
            mockConfiguration = new Mock<IConfiguration>();
            mockTokenService = new Mock<TokenService>(mockConfiguration.Object);
            mockTokenService.Setup(t => t.GenerateToken(It.IsAny<UserTokenDTO>())).ReturnsAsync("TestToken");
            mapper = new Mock<IMapper>();
        }

        [Test]
        [TestCase("TestUser2", "TestPassword2", "laishramrohit15@gmail.com", "TestHashKey",  Departments.Admin)]
        public async Task RegisterTest(string username, string password, string email, string hashKey, Departments department)
        {
            var user = new UserCreateDTO
            {
                UserName = username,
                Password = password,
                Email = email,
                Department = department
            };
            var userService = new UserService(repository.Object, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);
            Assert.IsTrue(addedUser.UserName == user.UserName);
        }
        [Test]
        [TestCase("TestUser2", "null", "TestHashKey", "laishramrohit15@gmail.com", Departments.Admin)]
        public async Task RegisterTestException(string username, string password, string email, string hashKey, Departments department)
        {
            var user = new UserCreateDTO
            {
                UserName = username,
                Password = password,
                Email = null,
                Department = department
            };
            repository.Setup(r => r.Add(It.IsAny<User>())).ThrowsAsync(new Exception("Repository failure"));
            var userService = new UserService(repository.Object, mapper.Object, logger.Object, mockTokenService.Object);



            Assert.ThrowsAsync<Exception>(async () =>
             {
                 await userService.Register(user);
             });
        }

        [Test]
        [TestCase("TestUser2", "TestPassword", "laishramrohit15@gmail.com", "TestHashKey",  Departments.Admin)]
        public async Task TestAuthenticate(string username, string password, string email, string hashKey, Departments department)
        {
            var user = new UserCreateDTO
            {
                UserName = username,
                Password = password,
                Email = email,
                Department = department
            };

            var users = new List<User>
    {
        new User { Id = 1, UserName = "Alice", Email = "alice@example.com", Department = Departments.IT },
        new User { Id = 2, UserName = "Bob", Email = "bob@example.com", Department = Departments.HR },
    };


            repository.Setup(r => r.GetAll()).ReturnsAsync(users);

            repository.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync((User user) =>
            {
                user.Id = 3;
                users.Add(user);
                return user;
            });
            var userService = new UserService(repository.Object, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);

            var loggedInUser = await userService.Login(new LoginDTO
            {
                Email = user.Email,
                Password = user.Password
            });
            Assert.IsNotNull(addedUser);
            Assert.AreEqual(username, addedUser.UserName);
            Assert.AreEqual(email, addedUser.Email);

            Assert.IsNotNull(loggedInUser);
            Assert.AreEqual(username, loggedInUser.UserName);
            Assert.AreEqual(email, loggedInUser.Email);
        }

        [Test]
    }
}
