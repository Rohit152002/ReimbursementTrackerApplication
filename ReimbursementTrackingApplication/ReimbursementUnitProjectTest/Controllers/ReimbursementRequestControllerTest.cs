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
    internal class ReimbursementRequestControllerTest
    {
        private ReimbursementRequestController _controller;
        private Mock<IReimbursementRequestService> _mockService;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IReimbursementRequestService>();
            _controller = new ReimbursementRequestController(_mockService.Object);
        }

        [Test]
        public async Task AddRequestAsync_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var createRequestDTO = new CreateReimbursementRequestDTO
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 100.0,
                Comments = "Test reimbursement",
                Items = new List<RequestITemDTO>
                {
                    new RequestITemDTO { Amount = 50.0, CategoryId = 1, Description = "Test item" }
                }
            };

            var responseDTO = new SuccessResponseDTO<ResponseReimbursementRequestDTO>
            {
                Data = new ResponseReimbursementRequestDTO { Id = 1, UserId = 1, TotalAmount = 100.0 }
            };

            _mockService.Setup(s => s.SubmitRequestAsync(createRequestDTO))
                        .ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.AddRequestAsync(createRequestDTO);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(responseDTO, okResult.Value);
        }

        [Test]
        public async Task AddRequestAsync_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var createRequestDTO = new CreateReimbursementRequestDTO
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 100.0,
                Items = new List<RequestITemDTO> { }
            };

            _mockService.Setup(s => s.SubmitRequestAsync(createRequestDTO))
                        .ThrowsAsync(new Exception("Error occurred"));

            // Act
            var result = await _controller.AddRequestAsync(createRequestDTO);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual("Error occurred", (badRequestResult.Value as ErrorResponseDTO).ErrorMessage);
        }

        [Test]
        public async Task GetRequestsById_ValidId_ReturnsOkResult()
        {
            // Arrange
            int requestId = 1;
            var responseDTO = new SuccessResponseDTO<ResponseReimbursementRequestDTO>
            {
                Data = new ResponseReimbursementRequestDTO { Id = 1, UserId = 1, TotalAmount = 100.0 }
            };

            _mockService.Setup(s => s.GetRequestByIdAsync(requestId))
                        .ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.GetRequestsById(requestId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responseDTO, okResult.Value);
        }

        [Test]
        public async Task GetRequestsById_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            int requestId = 1;
            _mockService.Setup(s => s.GetRequestByIdAsync(requestId))
                        .ThrowsAsync(new Exception("Request not found"));

            // Act
            var result = await _controller.GetRequestsById(requestId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual("Request not found", (badRequestResult.Value as ErrorResponseDTO).ErrorMessage);
        }

        [Test]
        public async Task GetAllRequest_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var paginatedResultDTO = new PaginatedResultDTO<ResponseReimbursementRequestDTO>
            {
                Data = new List<ResponseReimbursementRequestDTO>(){
                    new ResponseReimbursementRequestDTO() { Id = 1, TotalAmount = 100.0 } 
                }
            };

            _mockService.Setup(s => s.GetAllRequest(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(paginatedResultDTO);

            // Act
            var result = await _controller.GetAllRequest(1, 10);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(paginatedResultDTO, okResult.Value);
        }

        [Test]
        public async Task DeleteRequestsById_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            int requestId = 1;
            var responseDTO = new SuccessResponseDTO<int> { Data = requestId };

            _mockService.Setup(s => s.DeleteRequestAsync(requestId))
                        .ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.DeleteRequestsById(requestId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responseDTO, okResult.Value);
        }

        [Test]
        public async Task DeleteRequestsById_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            int requestId = 1;
            _mockService.Setup(s => s.DeleteRequestAsync(requestId))
                        .ThrowsAsync(new Exception("Error deleting request"));

            // Act
            var result = await _controller.DeleteRequestsById(requestId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual("Error deleting request", (badRequestResult.Value as ErrorResponseDTO).ErrorMessage);
        }
    }
}
