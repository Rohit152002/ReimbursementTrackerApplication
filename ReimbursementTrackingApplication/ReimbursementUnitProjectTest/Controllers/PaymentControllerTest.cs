using Microsoft.AspNetCore.Mvc;
using Moq;
using ReimbursementTrackingApplication.Controllers;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementUnitProjectTest.Controllers
{
    internal class PaymentControllerTest
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private PaymentController _paymentController;

        [SetUp]
        public void SetUp()
        {
            // Initialize the mock service and controller
            _paymentServiceMock = new Mock<IPaymentService>();
            _paymentController = new PaymentController(_paymentServiceMock.Object);
        }

        [Test]
        public async Task GetPaymentByRequestId_ReturnsOkResult_WhenPaymentExists()
        {
            // Arrange
            var requestId = 1;
            var payment = new SuccessResponseDTO<ResponsePayment> { Data = new ResponsePayment { RequestId = requestId } };
            _paymentServiceMock.Setup(service => service.GetPaymentByRequestIdAsync(requestId))
                               .ReturnsAsync(payment);

            // Act
            var result = await _paymentController.GetPaymentByRequestId(requestId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(payment, okResult.Value);
        }

        [Test]
        public async Task GetPaymentByRequestId_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var requestId = 1;
            _paymentServiceMock.Setup(service => service.GetPaymentByRequestIdAsync(requestId))
                               .ThrowsAsync(new System.Exception("Payment not found"));

            // Act
            var result = await _paymentController.GetPaymentByRequestId(requestId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            var errorResponse = notFoundResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber);
        }

        [Test]
        public async Task GetAllPayments_ReturnsOkResult_WithPayments()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var payments = new PaginatedResultDTO<ResponsePayment>();
            _paymentServiceMock.Setup(service => service.GetAllPaymentsAsync(pageNumber, pageSize))
                               .ReturnsAsync(payments);

            // Act
            var result = await _paymentController.GetAllPayments(pageNumber, pageSize);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(payments, okResult.Value);
        }

        [Test]
        public async Task ProcessPayment_ReturnsOkResult_WhenPaymentProcessedSuccessfully()
        {
            // Arrange
            var paymentDTO = new PaymentDTO { Id=1, PaymentMethod = PaymentMethod.DirectDeposit };
            var successResponse = new SuccessResponseDTO<ResponsePayment> { Data = new ResponsePayment { RequestId = 1 } };
            _paymentServiceMock.Setup(service => service.ProcessPaymentAsync(paymentDTO))
                               .ReturnsAsync(successResponse);

            // Act
            var result = await _paymentController.ProcessPayment(paymentDTO);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(successResponse, okResult.Value);
        }

        [Test]
        public async Task DeletePayment_ReturnsOkResult_WhenPaymentDeletedSuccessfully()
        {
            // Arrange
            var paymentId = 1;
            var successResponse = new SuccessResponseDTO<int> { Data = paymentId };
            _paymentServiceMock.Setup(service => service.DeletePaymentAsync(paymentId))
                               .ReturnsAsync(successResponse);

            // Act
            var result = await _paymentController.DeletePayment(paymentId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(successResponse, okResult.Value);
        }

        [Test]
        public async Task DeletePayment_ReturnsNotFound_WhenPaymentNotFound()
        {
            // Arrange
            var paymentId = 1;
            _paymentServiceMock.Setup(service => service.DeletePaymentAsync(paymentId))
                               .ThrowsAsync(new System.Exception("Payment not found"));

            // Act
            var result = await _paymentController.DeletePayment(paymentId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            var errorResponse = notFoundResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber);
        }
    }
}
