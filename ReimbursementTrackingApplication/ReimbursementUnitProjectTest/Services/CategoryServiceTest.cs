using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
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
    class CategoryServiceTest
    {
        DbContextOptions options;
        ContextApp context;

        Mock<ILogger<CategoryRepositories>> categoryLogger;
        Mock<IMapper> mapper;

        CategoryRepositories repository;
        ExpenseCategoryService service;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
               .UseInMemoryDatabase("database" + Guid.NewGuid())
                .Options;
            context = new ContextApp(options);

            categoryLogger = new Mock<ILogger<CategoryRepositories>>();

            mapper = new Mock<IMapper>();

            repository = new CategoryRepositories(context, categoryLogger.Object);
            service = new ExpenseCategoryService(repository, mapper.Object);
            
        }

        [Test]
        public async Task TestAddExpenseCategory()
        {
            CreateCategoryDTO categoryDTO = new CreateCategoryDTO()
            {
                Name = "Test",
                Description = "Test Description"
            };

            ExpenseCategory category = new ExpenseCategory()
            {
                Id = 1,
                Name = "Test",
                Description = "Test Description"
            };
            mapper.Setup(m=>m.Map<ExpenseCategory>(It.IsAny<CreateCategoryDTO>())).Returns(category);
            var result = await service.AddCategoryAsync(categoryDTO);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public async Task TestAddExpenseCategoryException()
        {
            CreateCategoryDTO categoryDTO = new CreateCategoryDTO()
            {
                Name = "Test",
                Description = null
            };

            ExpenseCategory category = new ExpenseCategory()
            {
                Id = 1,
                Name = "Test",
                Description = null
            };
            mapper.Setup(m => m.Map<ExpenseCategory>(It.IsAny<CreateCategoryDTO>())).Returns(category);
            Assert.ThrowsAsync<Exception>(async()=>await service.AddCategoryAsync(categoryDTO));
            //Assert.IsNotNull(result.Data);
        }

        [Test]
        public async Task TestDeleteCategory()
        {
            await TestAddExpenseCategory();

            var result = await service.DeleteCategoryAsync(1);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task TestExceptionDeleteCategory()
        {
            Assert.ThrowsAsync<Exception>(async () => await service.DeleteCategoryAsync(1));
        }

        [Test]
        public async Task TestUpdateCategory()
        {
            await TestAddExpenseCategory();
            CreateCategoryDTO updateCategory = new CreateCategoryDTO()
            {
                Name = "UpdatedTest",
                Description = "Test Description"
            };
            var result = await service.UpdateCategoryAsync(1,updateCategory);
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task TestExceptionUpdateCategory()
        {
            //await TestAddExpenseCategory();
            CreateCategoryDTO updateCategory = new CreateCategoryDTO()
            {
                Name = "UpdatedTest",
                Description = "Test Description"
            };
            Assert.ThrowsAsync<Exception>(async () => await service.UpdateCategoryAsync(1,updateCategory));
        }

        [Test]
        public async Task TestGetAllCategories()
        {
            await TestAddExpenseCategory();

            List<ExpenseCategoryDTO> categoryDTOs = new List<ExpenseCategoryDTO>()
            {
                new ExpenseCategoryDTO()
                {
                    Id = 1,
                    Name="Meal",
                    Description="Test Description"
                }
            };

            mapper.Setup(m => m.Map<List<ExpenseCategoryDTO>>(It.IsAny<List<ExpenseCategory>>())).Returns(categoryDTOs);

            var result = await service.GetAllCategoriesAsync(1,100);
            Assert.IsNotNull(result);

        }


        [Test]
        public async Task TestExceptionGetAllCategories()
        {
            //await TestAddExpenseCategory();

            List<ExpenseCategoryDTO> categoryDTOs = new List<ExpenseCategoryDTO>()
            {
                new ExpenseCategoryDTO()
                {
                    Id = 1,
                    Name="Meal",
                    Description="Test Description"
                }
            };

            mapper.Setup(m => m.Map<List<ExpenseCategoryDTO>>(It.IsAny<List<ExpenseCategory>>())).Returns(categoryDTOs);

            Assert.ThrowsAsync<Exception>(async()=> await service.GetAllCategoriesAsync(1, 100));
            //Assert.IsNotNull(result);

        }

        [Test]
        public async Task TestGetCategoryByID()
        {
            await TestAddExpenseCategory();
            ExpenseCategoryDTO categoryDTO = new ExpenseCategoryDTO()
            {
                Id = 1,
                Name = "Meal",
                Description = "Test Description"
            };

            mapper.Setup(m => m.Map<ExpenseCategoryDTO>(It.IsAny<ExpenseCategory>())).Returns(categoryDTO);

            var result = await service.GetCategoryByIdAsync(1);
            Assert.IsNotNull(result.Data);

        }

        [Test]
        public async Task TestExceptionCategoryById()
        {
            ExpenseCategoryDTO categoryDTO = new ExpenseCategoryDTO()
            {
                Id = 1,
                Name = "Meal",
                Description = "Test Description"
            };

            mapper.Setup(m => m.Map<ExpenseCategoryDTO>(It.IsAny<ExpenseCategory>())).Returns(categoryDTO);

            Assert.ThrowsAsync<Exception>(async()=> await service.GetCategoryByIdAsync(1));
        
        }
    }
}
