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
    internal class ExpensesCategoryRepositoryTest
    {
        DbContextOptions options;
        ContextApp context;
        CategoryRepositories repository;
        Mock<ILogger<CategoryRepositories>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger = new Mock<ILogger<CategoryRepositories>>();
            repository = new CategoryRepositories(context, logger.Object);
        }

        [Test]
        public async Task AddCategory()
        {

            ExpenseCategory category = new ExpenseCategory()
            { 
                Name = "Meals",
                Description="this is for meals" 
            };
            var addedUser = await repository.Add(category);
            Assert.IsTrue(addedUser.Name == category.Name);

        }

        [Test]
        public async Task AddCategoryException()
        {

            ExpenseCategory category = new ExpenseCategory()
            {
                Name = null,
                Description = "this is for meals"
            }; 

            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(category));
        }

        [Test]
        public async Task DeleteCategory()
        {
            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "Meals",
                Description = "this is for meals"
            }; ;
            var addedCategory = await repository.Add(category);
            var deleteCategory = await repository.Delete(addedCategory.Id);
            Assert.AreEqual(category.Name, deleteCategory.Name);
        }

        [Test]
        public async Task DeleteCategoryExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }
        [Test]
        public async Task GetAllCategory()
        {
            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "meals",
                Description = "this is for meals"
            }; ;
            await repository.Add(category);
            var categories = await repository.GetAll();
            Assert.NotNull(categories);
        }
        [Test]
        public async Task GetAllCategoryException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdateCategory()
        {
            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "meals",
                Description = "this is for meals"
            }; 
            await repository.Add(category);

            ExpenseCategory updatedCategory = new ExpenseCategory()
            {
                Name = "food",
                Description = "this is for meals"
            };
            var update = await repository.Update(1, updatedCategory);
            Assert.AreEqual(update.Name, updatedCategory.Name);
        }

        [Test]
        public async Task UpdateUserExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new ExpenseCategory { Description = "hello " }));

        }

        [Test]
        public async Task GetUser()
        {
            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "meals",
                Description = "this is for meals"
            };
            await repository.Add(category);
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
