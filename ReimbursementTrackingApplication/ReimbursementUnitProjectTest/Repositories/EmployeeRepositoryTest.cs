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

namespace ReimbursementUnitProjectTest.Repositories
{
    internal class EmployeeRepositoryTest
    {
        DbContextOptions options;
        ContextApp context;
        EmployeeRepository repository;
        Mock<ILogger<EmployeeRepository>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger = new Mock<ILogger<EmployeeRepository>>();
            repository = new EmployeeRepository(context, logger.Object);
        }

        [Test]
        public async Task AddEmployee()
        {

            Employee employee = new Employee()
            {
                EmployeeId = 1,
                ManagerId = 2,
              
            };
            var addedEmployee = await repository.Add(employee);
            Assert.IsTrue(addedEmployee.ManagerId == employee.ManagerId);

        }

        [Test]
        public async Task AddEmployeeException()
        {

            Employee employee = new Employee()
            {
                EmployeeId = 1,
            };
            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(employee));
        }

        [Test]
        public async Task DeleteEmployee()
        {
            Employee employee = new Employee()
            {
                EmployeeId = 1,
                ManagerId = 2,

            };
            var addedEmployee = await repository.Add(employee);
            var deleteEmployee = await repository.Delete(addedEmployee.Id);
            Assert.AreEqual(addedEmployee.EmployeeId, deleteEmployee.EmployeeId);
        }

        [Test]
        public async Task DeleteEmployeeExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }


        [Test]
        public async Task GetAllEmployeee()
        {
            Employee employee = new Employee()
            {
                EmployeeId = 1,
                ManagerId = 2,

            };
            await repository.Add(employee);
            var employees = await repository.GetAll();
            Assert.NotNull(employees);
        }
        [Test]
        public async Task GetAllEmployeeException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdateEmployee()
        {
            Employee employee = new Employee()
            {
                Id = 1,
                EmployeeId = 1,
                ManagerId = 2,

            };
           var addedEmployee= await repository.Add(employee);

            Employee updateEmployee = new Employee()
            {
                Id=1,
                EmployeeId = 1,
                ManagerId = 3,

            };
            var update = await repository.Update(addedEmployee.Id, updateEmployee);
            Assert.AreEqual(update.ManagerId, updateEmployee.ManagerId);
        }

        [Test]
        public async Task UpdateEmployeeExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new Employee { ManagerId = 2}));

        }

        [Test]
        public async Task GetEmployeeTest()
        {
            Employee employee = new Employee()
            {
                Id = 1,
                EmployeeId = 1,
                ManagerId = 2,

            };
            await repository.Add(employee);
            var getEmployee = await repository.Get(1);
            Assert.IsNotNull(getEmployee);
        }

        [Test]
        public async Task GetaEmployeeExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Get(1));
        }
    }
}
