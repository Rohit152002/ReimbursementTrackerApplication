using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;
using ReimbursementTrackingApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementUnitProjectTest.Services
{
    class EmployeeServiceTest
    {
        DbContextOptions options;
        ContextApp context;

        EmployeeRepository employeeRepository;
        UserRepository userRepository;
        Mock<ILogger<EmployeeRepository>> employeeLogger;
        Mock<ILogger<UserRepository>> userLogger;
        Mock<ILogger<UserService>> userServiceLogger;

        Mock<IMapper> mapper;

        Mock<IConfiguration> mockConfiguration;
        Mock<TokenService> mockTokenService;
        IEmployeeService employeeService;
        UserService userServices;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                .UseInMemoryDatabase("database" + Guid.NewGuid())
                 .Options;
            context = new ContextApp(options);
            employeeLogger = new Mock<ILogger<EmployeeRepository>>();
            userLogger = new Mock<ILogger<UserRepository>>();
            userServiceLogger = new Mock<ILogger<UserService>>();
            mapper = new Mock<IMapper>();
            mockConfiguration = new Mock<IConfiguration>();
            mockTokenService = new Mock<TokenService>(mockConfiguration.Object);
            mockTokenService.Setup(t => t.GenerateToken(It.IsAny<UserTokenDTO>())).ReturnsAsync("TestToken");
            employeeRepository = new EmployeeRepository(context,employeeLogger.Object);
            userRepository= new UserRepository(context,userLogger.Object);
            userServices= new UserService(userRepository,mapper.Object,userServiceLogger.Object,mockTokenService.Object);

            employeeService = new EmployeeService(employeeRepository,mapper.Object,userServices);
        }

        [Test]
        public async Task AddEmployeeTest()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 5,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 5,
                ManagerId = 1,
            };

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);
            
            var result = await employeeService.AddEmployeeAsync(employeeDTO);
            Assert.AreEqual(result.Data, employee.EmployeeId);
        }

        [Test]
        public async Task AddEmployeeTestException()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 5,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 5,
             
            };

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            Assert.ThrowsAsync<Exception>(async()=> await employeeService.AddEmployeeAsync(employeeDTO));
        }
        [Test]
        public async Task DeleteEmployeeTest()
        {
            await AddEmployeeTest();
            var result = await employeeService.DeleteEmployeeAsync(1);
            Assert.AreEqual(result.Data, 5);
        }

        [Test]
        public async Task DeleteEmployeeTestException()
        {
            Assert.ThrowsAsync<Exception>(async () => await employeeService.DeleteEmployeeAsync(1));
        }

        [Test]
        public async Task UpdateEmployeeTest()
        {
            await AddEmployeeTest();

            Employee employee = new Employee()
            {
                //EmployeeId = 5,
                ManagerId = 1,
            };

            EmployeeDTO updateEmployee = new EmployeeDTO()
            { 
                ManagerId=2,
            };



            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            var result = await employeeService.UpdateEmployeeAsync(1, updateEmployee);

            Assert.NotNull(result.Data);

        }
        [Test]
        public async Task UpdateEmployeeTestException()
        {
            //await AddEmployeeTest();

            Employee employee = new Employee()
            {
                //EmployeeId = 5,
                ManagerId = 1,
            };

            EmployeeDTO updateEmployee = new EmployeeDTO()
            {
                ManagerId = 2,
            };



            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

           

            Assert.ThrowsAsync<Exception>(async()=> await employeeService.UpdateEmployeeAsync(1, updateEmployee));

        }

        //public async Task<PaginatedResultDTO<ResponseEmployeeDTO>> GetAllEmployeesAsync(int pageNumber, int pageSize)

        [Test]
        public async Task TestGetAllEmployees()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 2,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 2,
                ManagerId = 1,
            };

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            var result = await employeeService.GetAllEmployeesAsync(1, 100);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public async Task TestGetAllEmployeesById()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 2,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 2,
                ManagerId = 1,
            };

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            var result = await employeeService.GetEmployeeByIdAsync(1);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public async Task TestGetAllEmployeesByManagerId()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 2,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 2,
                ManagerId = 1,
            };

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            var result = await employeeService.GetEmployeesByManagerIdAsync(2,1,100);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public async Task TestExceptionGetAllEmployeesById()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 2,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 2,
                ManagerId = 1,
            };

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            //var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

           Assert.ThrowsAsync<Exception>(async()=> await employeeService.GetEmployeeByIdAsync(1));
            
        }

        [Test]
        public async Task TestExceptionGetAllEmployeesByManagerId()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 2,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 2,
                ManagerId = 1,
            };

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            //var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            Assert.ThrowsAsync<Exception>(async () => await employeeService.GetEmployeeByIdAsync(1));

        }

        [Test]
        public async Task TestGetAllEmployeesException()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 2,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 2,
                ManagerId = 1,
            };

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            //var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            Assert.ThrowsAsync<Exception>(async() => await employeeService.GetAllEmployeesAsync(1, 100));
           
        }
    }
}
