
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;
using ReimbursementTrackingApplication.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementUnitProjectTest.Services
{
    internal class UserServiceTest
    {

        DbContextOptions options;
        Mock<ILogger<UserService>> logger;
        Mock<ILogger<UserRepository>> loggerRepository;
        Mock<IMapper> mapper;
        IRepository<int, User> repository;
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
            loggerRepository = new Mock<ILogger<UserRepository>>();
            repository = new UserRepository(context, loggerRepository.Object);
            mockConfiguration = new Mock<IConfiguration>();
            mockTokenService = new Mock<TokenService>(mockConfiguration.Object);
            mockTokenService.Setup(t => t.GenerateToken(It.IsAny<UserTokenDTO>())).ReturnsAsync("TestToken");
            mapper = new Mock<IMapper>();
        }

        [Test]
        [TestCase("TestUser2", "TestPassword2", "laishramrohit15@gmail.com", "TestHashKey", Departments.Admin)]
        public async Task RegisterTest(string username, string password, string email, string hashKey, Departments department)
        {
            var user = new UserCreateDTO
            {
                UserName = username,
                Password = password,
                Email = email,
                Department = department
            };
            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);
            Assert.IsTrue(addedUser.UserName == user.UserName);
        }
        
        
        [Test]
        [TestCase("TestUser2", "null", "TestHashKey", "laishramrohit15@gmail.com", Departments.Admin)]
        public async Task RegisterTestException(string username, string password, string email, string hashKey, Departments department)
        {
            var user = new UserCreateDTO
            {
                UserName = null,
                Password = password,
                Email = email,
                Department = department
            };

            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);



            Assert.ThrowsAsync<Exception>(async () =>
             {
                 await userService.Register(user);
             });
        }

        
        
        [Test]
        [TestCase("TestUser2", "TestPassword", "laishramrohit15@gmail.com", "TestHashKey", Departments.Admin)]
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



            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
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
        public async Task TestAuthenticateException()
        {
            var user = new UserCreateDTO
            {
                UserName = "TestUser2",
                Password = "TestPassword",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.Admin
            };
            var users = new List<User>
                        {
                            new User { Id = 1, UserName = "Alice", Email = "alice@example.com", Department = Departments.IT },
                            new User { Id = 2, UserName = "Bob", Email = "bob@example.com", Department = Departments.HR },
                        };


            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);



            Assert.ThrowsAsync<NotFoundException>(async () => await userService.Login(new LoginDTO { Email = "abc@example.com", Password = user.Password }));



        }
        
        
        [Test]
        public async Task TestExceptionInvalidUsernameAndPassword()
        {
            var user = new UserCreateDTO
            {
                UserName = "TestUser2",
                Password = "TestPassword",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.Admin
            };
            var users = new List<User>
                        {
                            new User { Id = 1, UserName = "Alice", Email = "alice@example.com", Department = Departments.IT },
                            new User { Id = 2, UserName = "Bob", Email = "bob@example.com", Department = Departments.HR },
                        };



            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);

            Assert.ThrowsAsync<Exception>(async () => await userService.Login(new LoginDTO { Email = user.Email, Password = "IncorrectPassword" }));
        }

        [Test]
        [TestCase(3, "test", "newpassword", "newpassword")]
        public async Task ChangePassword(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = new UserCreateDTO
            {
                UserName = "Rohit Laishrma",
                Password = currentPassword,
                Email = "laishrma@gmail.com",
                Department = Departments.CustomerSupport
            };
            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);

            var changePasswordDTO = new ChangePasswordDTO
            {
                UserId = 1,
                currentPassword = currentPassword,
                newPassword = newPassword, 
                confirmPassword = confirmPassword 
            };


            // Act
            var result = await userService.ChangePassword(changePasswordDTO);

            // Assert
            Assert.IsTrue(result);
        }
        
        
        [Test]
        [TestCase(3, "test", "newpassword", "newpassword")]
        public async Task TestIncorrectNotSamePassword(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = new UserCreateDTO
            {
                UserName = "Rohit Laishrma",
                Password = currentPassword,
                Email = "laishrma@gmail.com",
                Department = Departments.CustomerSupport
            };
            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);

            var changePasswordDTO = new ChangePasswordDTO
            {
                UserId = 1,
                currentPassword = currentPassword, // Correct current password
                newPassword = newPassword, // New password
                confirmPassword = "anotherPassword" // Matching confirm password
            };


            // Act
            Assert.ThrowsAsync<Exception>(async () => await userService.ChangePassword(changePasswordDTO));
        }

        [Test]
        [TestCase(3, "test", "newpassword", "newpassword")]
        public async Task TestInvalidCurrentPassword(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = new UserCreateDTO
            {
                UserName = "Rohit Laishrma",
                Password = currentPassword,
                Email = "laishrma@gmail.com",
                Department = Departments.CustomerSupport
            };
            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);

            var changePasswordDTO = new ChangePasswordDTO
            {
                UserId = 1,
                currentPassword = "flsdjkfP", // Correct current password
                newPassword = newPassword, // New password
                confirmPassword = "anotherPassword" // Matching confirm password
            };


            // Act
            Assert.ThrowsAsync<Exception>(async () => await userService.ChangePassword(changePasswordDTO));
        }

        [Test]
        public async Task TestGetProfileById()
        {

            // Arrange
            var userCreateDTO = new UserCreateDTO
            {
                UserName = "Rohit Laishram",
                Password = "TestPassword",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };

            var user = new User
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT,
            };

            UserDTO userDTO = new UserDTO
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };


            IUserServices userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);

            // Act
            var addedUser = await userService.Register(userCreateDTO);

            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);
            var result = await userService.GetUserProfile(1);

            // Assert
            Assert.IsNotNull(result); // Check that the result is not null
            Assert.AreEqual(userDTO.UserName, result.UserName); // Verify the mapped values
            Assert.AreEqual(userDTO.Email, result.Email);
            Assert.AreEqual(userDTO.Department, result.Department);

        }
        [Test]
        public async Task TestGetAllUsers()
        {
            var user = new UserCreateDTO
            {
                UserName = "Rohit Laishram",
                Password = "TestPassword",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            var userList = new List<User>
        {
            new User { UserName = "Rohit Laishram", Email = "laishramrohit15@gmail.com", Department = Departments.IT }
            
        };

            var userDTOList = new List<UserDTO>
        {
            new UserDTO { UserName = "Rohit Laishram", Email = "laishramrohit15@gmail.com", Department = Departments.IT }
          
        };
            mapper.Setup(m => m.Map<IList<UserDTO>>(It.IsAny<IList<User>>()))
                   .Returns(userDTOList);
            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userService.Register(user);
            PaginatedResultDTO<UserDTO> users = await userService.GetAllUsers(1,10);
            Assert.NotNull(users.Data);
        }

        [Test]
        public async Task TestSearchUser()
        {
            var user = new UserCreateDTO
            {
                UserName = "Rohit Laishram",
                Password = "TestPassword",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            UserDTO userDTO = new UserDTO
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            mapper.Setup(m => m.Map<UserDTO>(user)).Returns(userDTO);
            var addedUser = await userService.Register(user);
            PaginatedResultDTO<UserDTO> users = await userService.SearchUser("Rohit",1,10);
            Assert.NotNull(users);
        }







    }

}
