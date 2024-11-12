using Microsoft.AspNetCore.Mvc;
using Moq;
using ReimbursementTrackingApplication.Controllers;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementUnitProjectTest.Controllers
{
    internal class CategoryControllerTest
    {
        private Mock<IExpenseCategoryService> _mockExpenseCategoryService;
        private CategoryController _categoryController;

        [SetUp]
        public void Setup()
        {
            // Initialize the mock service
            _mockExpenseCategoryService = new Mock<IExpenseCategoryService>();

            // Inject the mock service into the controller
            _categoryController = new CategoryController(_mockExpenseCategoryService.Object);
        }

        [Test]
        public async Task GetCategoryById_ValidId_ReturnsCategory()
        {
            // Arrange
            var categoryId = 1;
            var categoryDTO = new ExpenseCategoryDTO { Id = categoryId, Name = "Travel" };
            _mockExpenseCategoryService.Setup(service => service.GetCategoryByIdAsync(categoryId))
                                .ReturnsAsync(new SuccessResponseDTO<ExpenseCategoryDTO> { Data = categoryDTO });

            // Act
            var result = await _categoryController.GetCategoryByid(categoryId);
            var okResult = result.Result as OkObjectResult;
            var returnedCategory = okResult?.Value as SuccessResponseDTO<ExpenseCategoryDTO>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(categoryId, returnedCategory.Data.Id);
        }

        [Test]
        public async Task GetCategoryById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var categoryId = 999;
            _mockExpenseCategoryService.Setup(service => service.GetCategoryByIdAsync(categoryId))
                                .Throws(new KeyNotFoundException("Category not found"));

            // Act
            var result = await _categoryController.GetCategoryByid(categoryId);
            var notFoundResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Category not found", ((ErrorResponseDTO)notFoundResult.Value).ErrorMessage);
        }

        [Test]
        public async Task GetAllCategories_ReturnsPaginatedCategories()
        {
            // Arrange
            var categories = new PaginatedResultDTO<ExpenseCategoryDTO>
            {
                Data = new List<ExpenseCategoryDTO> { new ExpenseCategoryDTO { Id = 1, Name = "Travel" } },
                TotalCount = 1
            };
            _mockExpenseCategoryService.Setup(service => service.GetAllCategoriesAsync(1, 10))
                                .ReturnsAsync(categories);

            // Act
            var result = await _categoryController.GetAllCategories(1, 10);
            var okResult = result.Result as OkObjectResult;
            var returnedCategories = okResult?.Value as PaginatedResultDTO<ExpenseCategoryDTO>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, returnedCategories.TotalCount);
        }

        [Test]
        public async Task GetAllCategories_ThrowsException_ReturnsNotFound()
        {
            // Arrange
            _mockExpenseCategoryService.Setup(service => service.GetAllCategoriesAsync(1, 10))
                                .Throws(new Exception("Collection is Empty"));

            // Act
            var result = await _categoryController.GetAllCategories(1, 10);
            var notFoundResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Collection is Empty", ((ErrorResponseDTO)notFoundResult.Value).ErrorMessage);
        }

        [Test]
        public async Task AddCategory_ValidCategory_ReturnsCreatedCategory()
        {
            // Arrange
            var categoryDTO = new CreateCategoryDTO { Name = "Travel", Description = "Travel related expenses" };
            var createdCategoryResponse = new SuccessResponseDTO<int> { Data = 1 };
            _mockExpenseCategoryService.Setup(service => service.AddCategoryAsync(categoryDTO))
                                .ReturnsAsync(createdCategoryResponse);

            // Act
            var result = await _categoryController.AddCategories(categoryDTO);
            var okResult = result.Result as OkObjectResult;
            var returnedResult = okResult?.Value as SuccessResponseDTO<int>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, returnedResult.Data);
        }

        [Test]
        public async Task AddCategory_ThrowsException_ReturnsError()
        {
            // Arrange
            var categoryDTO = new CreateCategoryDTO { Name = "Travel", Description = "Travel related expenses" };
            _mockExpenseCategoryService.Setup(service => service.AddCategoryAsync(categoryDTO))
                                .Throws(new Exception("Failed to add category"));

            // Act
            var result = await _categoryController.AddCategories(categoryDTO);
            var errorResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(200, errorResult.StatusCode);
            Assert.AreEqual("Failed to add category", ((ErrorResponseDTO)errorResult.Value).ErrorMessage);
        }

        [Test]
        public async Task UpdateCategory_ValidCategory_ReturnsUpdatedCategory()
        {
            // Arrange
            var categoryId = 1;
            var categoryDTO = new CreateCategoryDTO { Name = "Travel Updated", Description = "Updated description" };
            var updatedCategoryResponse = new SuccessResponseDTO<int> { Data = 1 };

            _mockExpenseCategoryService.Setup(service => service.UpdateCategoryAsync(categoryId, categoryDTO))
                                .ReturnsAsync(updatedCategoryResponse);

            // Act
            var result = await _categoryController.UpdateCategory(categoryId, categoryDTO);
            var okResult = result.Result as OkObjectResult;
            var returnedResult = okResult?.Value as SuccessResponseDTO<int>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, returnedResult.Data);
        }

        [Test]
        public async Task UpdateCategory_ThrowsException_ReturnsError()
        {
            // Arrange
            var categoryId = 1;
            var categoryDTO = new CreateCategoryDTO { Name = "Travel Updated", Description = "Updated description" };
            _mockExpenseCategoryService.Setup(service => service.UpdateCategoryAsync(categoryId, categoryDTO))
                                .Throws(new Exception("Failed to update category"));

            // Act
            var result = await _categoryController.UpdateCategory(categoryId, categoryDTO);
            var errorResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(200, errorResult.StatusCode);
            Assert.AreEqual("Failed to update category", ((ErrorResponseDTO)errorResult.Value).ErrorMessage);
        }

        [Test]
        public async Task DeleteCategory_ValidId_ReturnsDeletedCategory()
        {
            // Arrange
            var categoryId = 1;
            var deleteResponse = new SuccessResponseDTO<int> { Data = 1 };

            _mockExpenseCategoryService.Setup(service => service.DeleteCategoryAsync(categoryId))
                                .ReturnsAsync(deleteResponse);

            // Act
            var result = await _categoryController.Delete(categoryId);
            var okResult = result.Result as OkObjectResult;
            var returnedResult = okResult?.Value as SuccessResponseDTO<int>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, returnedResult.Data);
        }

        [Test]
        public async Task DeleteCategory_ThrowsException_ReturnsError()
        {
            // Arrange
            var categoryId = 1;
            _mockExpenseCategoryService.Setup(service => service.DeleteCategoryAsync(categoryId))
                                .Throws(new Exception("Failed to delete category"));

            // Act
            var result = await _categoryController.Delete(categoryId);
            var errorResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(200, errorResult.StatusCode);
            Assert.AreEqual("Failed to delete category", ((ErrorResponseDTO)errorResult.Value).ErrorMessage);
        }
    }
}
