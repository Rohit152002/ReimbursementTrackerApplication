using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
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
        Mock<IMailSender> _mockMailSender;

        //private Mock<IUserServices> _userServiceMock;





        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            repoLogger = new Mock<ILogger<UserRepository>>();
            userServiceLogger = new Mock<ILogger<UserService>>();
            //_userServiceMock = new Mock<IUserServices>();
            _mockMailSender = new Mock<IMailSender>();
            mapper = new Mock<IMapper>();
            mockConfiguration = new Mock<IConfiguration>();
            mockTokenService = new Mock<TokenService>(mockConfiguration.Object);
            repository = new UserRepository(context, repoLogger.Object);
            userServices = new UserService(repository, mapper.Object, userServiceLogger.Object, mockTokenService.Object);
            userController = new UserController(userServices,_mockMailSender.Object);
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

        [Test]
        public async Task LoginTest()
        {
            await RegisterUserTest("TestUser2", "testPassword", "laishramrohit15@gmail.com", "TestHashKey", Departments.Admin);

            var user = new LoginDTO
            {
                Email = "laishramrohit15@gmail.com",
                Password = "testPassword",
              
            };

            var result = await userController.Login(user);
            Assert.IsNotNull(result);
            var resultObject = result.Result as OkObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(200, resultObject.StatusCode);
        }

        [Test]
        [TestCase( "testPassword", "laishramrohit15@gmail.com")]
        public async Task TestExceptionLoginResponse(string password,string email)
        {
            var user = new LoginDTO
            {
               
                Password = password,
                Email = email
                
            };


            userController.ModelState.AddModelError("Email", "Invalid email format");


            Assert.ThrowsAsync<Exception>(async () => await userController.Login(user));
        }

        [Test]
        public async Task GetUserProfileAsync_WhenValidId_ReturnsUserProfile()
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



            // Act
            var addedUser = await userServices.Register(userCreateDTO);

            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);
            var result = await userServices.GetUserProfile(1);

            // Assert
            Assert.IsNotNull(result); // Check that the result is not null
            Assert.AreEqual(userDTO.UserName, result.UserName); // Verify the mapped values
            Assert.AreEqual(userDTO.Email, result.Email);
            Assert.AreEqual(userDTO.Department, result.Department);
            // Act
            var result1 = await userController.GetUserProfileAsync(1);

            Assert.IsNotNull(result1);
            var resultObject = result1.Result as OkObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(200, resultObject.StatusCode);

    
        }
        [Test]
        public async Task ExceptionGetUserProfileAsync_WhenValidId_ReturnsUserProfile()
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


         

            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);
            
            // Act
            var result1 = await userController.GetUserProfileAsync(1);

            Assert.IsNotNull(result1);
            var resultObject = result1.Result as NotFoundObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(404, resultObject.StatusCode);

        }
        [Test]
        public async Task TestGetAllUser()
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
            //var userService = new UserService(repository, mapper.Object, logger.Object, mockTokenService.Object);
            var addedUser = await userServices.Register(user);
            PaginatedResultDTO<UserDTO> users = await userServices.GetAllUsers(1, 10);
            Assert.NotNull(users.Data);

            var result1 = await userController.GetAllUsersAsync(1,10);

            Assert.IsNotNull(result1);
            var resultObject = result1.Result as OkObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(200, resultObject.StatusCode);
        }

        [Test]
        public async Task TestExceptionGetAllUser()
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
            

            var result1 = await userController.GetAllUsersAsync(1, 10);

            Assert.IsNotNull(result1);
            var resultObject = result1.Result as NotFoundObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(404, resultObject.StatusCode);
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
          
            mapper.Setup(m => m.Map<UserDTO>(user)).Returns(userDTO);
            var addedUser = await userServices.Register(user);
            PaginatedResultDTO<UserDTO> users = await userServices.SearchUser("Rohit", 1, 10);
            Assert.NotNull(users);

            var result1 = await userController.SearchUsers("Rohit",1, 10);

            Assert.IsNotNull(result1);
            var resultObject = result1.Result as OkObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(200, resultObject.StatusCode);


        }

        [Test]
        public async Task TestExceptionSearchUser()
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

            mapper.Setup(m => m.Map<UserDTO>(user)).Returns(userDTO);
            //var addedUser = await userServices.Register(user);
            //PaginatedResultDTO<UserDTO> users = await userServices.SearchUser("Rohit", 1, 10);
            //Assert.NotNull(users);

            var result1 = await userController.SearchUsers("Rohit", 1, 10);

            Assert.IsNotNull(result1);
            var resultObject = result1.Result as NotFoundObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(404, resultObject.StatusCode);

        }

        [Test]
        [TestCase(3, "test", "newpassword", "newpassword")]
        public async Task TestChangePassword(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = new UserCreateDTO
            {
                UserName = "Rohit Laishram",
                Password = currentPassword,
                Email = "laishram@gmail.com",
                Department = Departments.CustomerSupport
            };
          
            var addedUser = await userServices.Register(user);

            var changePasswordDTO = new ChangePasswordDTO
            {
                UserId = addedUser.Id,
                currentPassword = currentPassword,
                newPassword = newPassword,
                confirmPassword = confirmPassword
            };


            // Act
        
            // Assert
            
            var result1 = await userController.ChangePassword(changePasswordDTO);

            Assert.IsNotNull(result1);
            var resultObject = result1.Result as OkObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(200, resultObject.StatusCode);
        }

        [Test]
        [TestCase(3, "test", "newpassword", "newpassword")]
        public async Task TestExceptionChangePassword(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = new UserCreateDTO
            {
                UserName = "Rohit Laishram",
                Password = currentPassword,
                Email = "laishram@gmail.com",
                Department = Departments.CustomerSupport
            };

            //var addedUser = await userServices.Register(user);

            var changePasswordDTO = new ChangePasswordDTO
            {
                UserId = 1,
                currentPassword = currentPassword,
                newPassword = newPassword,
                confirmPassword = confirmPassword
            };


            // Act

            // Assert

            var result1 = await userController.ChangePassword(changePasswordDTO);

            Assert.IsNotNull(result1);
            var resultObject = result1.Result as NotFoundObjectResult;
            // Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(404, resultObject.StatusCode);

        }


    }
}
