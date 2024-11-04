using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementProjectTest.Repositories
{
     class UserRepositoryTest
    {
        DbContextOptions options;
        ContextApp context;
        UserRepository repository;
        Mock<ILogger<UserRepository>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger= new Mock<ILogger<UserRepository>>();
            repository = new UserRepository(context, logger.Object);
        }

        [Test]
        public async Task AddUser()
        {
          
                User user = new User
                {
                    UserName = "Rohit",
                    Password = Encoding.UTF8.GetBytes("TestPassword"),
                    HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                    Department = Departments.IT,
                    Email = "laishramrohit15@gmail.com",
                };
                var addedUser = await repository.Add(user);
                Assert.IsTrue(addedUser.UserName == user.UserName);
         
        }

        [Test]
        public async Task AddUserException()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = null,
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            
            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(user));
        }

        [Test]
        public async Task DeleteUser()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await repository.Add(user);
            var deleteUser = await repository.Delete(addedUser.Id);
            Assert.AreEqual(user.UserName, deleteUser.UserName);
        }

        [Test]
        public async Task DeleteUserExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }
        [Test]
        public async Task GetAllUsers()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
             await repository.Add(user);
            var users= await repository.GetAll();
            Assert.NotNull(users);
        }
        [Test]
        public async Task GetAllUsersException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdateUser()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            await repository.Add(user);

            User updatedUser = new User()
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Sales,
                Email = "laishramrohit15@gmail.com",
            };
            var update = await repository.Update(1, updatedUser);
            Assert.AreEqual(update.Department, Departments.Sales);
        }

        [Test]
        public async Task UpdateUserExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new User { Email = "rohitlaishram@gmail.com" }));

        }

        [Test] 
        public async Task GetUser()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            await repository.Add(user);
            var getUser = await repository.Get(1);
            Assert.IsNotNull(getUser);  
        }

        [Test]
        public async Task GetUserExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Get(1));
        }
    }
}
