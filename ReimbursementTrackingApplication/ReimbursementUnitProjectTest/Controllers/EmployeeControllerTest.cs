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
    internal class EmployeeControllerTest
    {
        private EmployeeController _controller;
        private Mock<IEmployeeService> _mockEmployeeService;

        [SetUp]
        public void Setup()
        {
            _mockEmployeeService = new Mock<IEmployeeService>();
            _controller = new EmployeeController(_mockEmployeeService.Object);
        }

        [Test]
        public async Task AssignEmployeeManager_ShouldReturnOk_WhenEmployeeIsAdded()
        {
            // Arrange
            var employeeDTO = new EmployeeDTO { EmployeeId = 1, ManagerId = 2 };
            var successResponse = new SuccessResponseDTO<int> { Data = 1 };
            _mockEmployeeService.Setup(service => service.AddEmployeeAsync(employeeDTO))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.AssignEmployeeManager(employeeDTO);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(1, ((SuccessResponseDTO<int>)okResult.Value).Data);
        }

        [Test]
        public async Task AssignEmployeeManager_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var employeeDTO = new EmployeeDTO { EmployeeId = 1, ManagerId = 2 };
            _mockEmployeeService.Setup(service => service.AddEmployeeAsync(employeeDTO))
                .ThrowsAsync(new System.Exception("Error occurred"));

            // Act
            var result = await _controller.AssignEmployeeManager(employeeDTO);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual("Error occurred", ((ErrorResponseDTO)badRequestResult.Value).ErrorMessage);
        }

        [Test]
        public async Task DeleteAssignEmployee_ShouldReturnOk_WhenEmployeeIsDeleted()
        {
            // Arrange
            var successResponse = new SuccessResponseDTO<int> { Data = 1 };
            _mockEmployeeService.Setup(service => service.DeleteEmployeeAsync(1))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.DeleteAssignEmployee(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(1, ((SuccessResponseDTO<int>)okResult.Value).Data);
        }

        [Test]
        public async Task GetEmployeeById_ShouldReturnOk_WhenEmployeeExists()
        {
            // Arrange
            var responseEmployeeDTO = new SuccessResponseDTO<ResponseEmployeeDTO>
            {
                Data = new ResponseEmployeeDTO { Id = 1, EmployeeId = 1, ManagerId = 2 }
            };
            _mockEmployeeService.Setup(service => service.GetEmployeeByIdAsync(1))
                .ReturnsAsync(responseEmployeeDTO);

            // Act
            var result = await _controller.GetEmployeeById(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(1, ((SuccessResponseDTO<ResponseEmployeeDTO>)okResult.Value).Data.Id);
        }

        [Test]
        public async Task GetEmployeeById_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            _mockEmployeeService.Setup(service => service.GetEmployeeByIdAsync(1))
                .ThrowsAsync(new System.Exception("Error occurred"));

            // Act
            var result = await _controller.GetEmployeeById(1);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual("Error occurred", ((ErrorResponseDTO)badRequestResult.Value).ErrorMessage);
        }
    }
}
