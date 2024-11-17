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
    internal class ApprovalControllerTest
    {
        private Mock<IApprovalService> _approvalServiceMock;
        private ApprovalController _approvalController;

        [SetUp]
        public void SetUp()
        {
            // Initialize the mock service and controller
            _approvalServiceMock = new Mock<IApprovalService>();
            _approvalController = new ApprovalController(_approvalServiceMock.Object);
        }

        [Test]
        public async Task GetApprovalById_ReturnsOkResult_WhenApprovalExists()
        {
            // Arrange
            var approvalId = 1;
            var responseApproval = new SuccessResponseDTO<ResponseApprovalStageDTO>
            {
                Data = new ResponseApprovalStageDTO { Id = approvalId }
            };

            _approvalServiceMock.Setup(service => service.GetApprovalByIdAsync(approvalId))
                                .ReturnsAsync(responseApproval);

            // Act
            var result = await _approvalController.GetApprovalById(approvalId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responseApproval, okResult.Value);
        }

        [Test]
        public async Task GetApprovalById_ReturnsBadRequest_WhenApprovalNotFound()
        {
            // Arrange
            var approvalId = 1;
            _approvalServiceMock.Setup(service => service.GetApprovalByIdAsync(approvalId))
                                .ThrowsAsync(new Exception("Approval not found"));

            // Act
            var result = await _approvalController.GetApprovalById(approvalId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber);
        }

        [Test]
        public async Task GetApprovalByRequestId_ReturnsOkResult_WithApprovals()
        {
            // Arrange
            var requestId = 1;
            var pageNumber = 1;
            var pageSize = 10;
            var approvals = new PaginatedResultDTO<ResponseApprovalStageDTO>();
            _approvalServiceMock.Setup(service => service.GetApprovalsByRequestIdAsync(requestId, pageNumber, pageSize))
                                .ReturnsAsync(approvals);

            // Act
            var result = await _approvalController.GetApprovalByRequestId(requestId, pageNumber, pageSize);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(approvals, okResult.Value);
        }

        [Test]
        public async Task GetApprovalByRequestId_ReturnsBadObjectResult_WithApprovals()
        {
            // Arrange
            var requestId = 1;
            var pageNumber = 1;
            var pageSize = 10;
            var approvals = new PaginatedResultDTO<ResponseApprovalStageDTO>();
            _approvalServiceMock.Setup(service => service.GetApprovalsByRequestIdAsync(requestId, pageNumber, pageSize))
                                .Throws(new Exception("error"));

            // Act
            var result = await _approvalController.GetApprovalByRequestId(requestId, pageNumber, pageSize);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber); ;
        }

        [Test]
        public async Task ApproveRequest_ReturnsOkResult_WhenApprovalSucceeds()
        {
            // Arrange
            var approval = new ApprovalStageDTO { RequestId = 1, ReviewId = 1 };
            var successResponse = new SuccessResponseDTO<int> { Data = 1 };
            _approvalServiceMock.Setup(service => service.ApproveRequestAsync(approval))
                                .ReturnsAsync(successResponse);

            // Act
            var result = await _approvalController.ApproveRequest(approval);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(successResponse, okResult.Value);
        }
        
        [Test]
        public async Task ApproveRequest_ReturnsBad_WhenApprovalSucceeds()
        {
            // Arrange
            var approval = new ApprovalStageDTO { RequestId = 1, ReviewId = 1 };
            var successResponse = new SuccessResponseDTO<int> { Data = 1 };
            _approvalServiceMock.Setup(service => service.ApproveRequestAsync(approval))
                                .Throws(new Exception("Exception Error"));

            // Act
            var result = await _approvalController.ApproveRequest(approval);

            // Assert
            var badRequestResult = result.Result as NotFoundObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber); ;
        }

        [Test]
        public async Task RejectRequest_ReturnsOkResult_WhenRejectionSucceeds()
        {
            // Arrange
            var approval = new ApprovalStageDTO { RequestId = 1, ReviewId = 1 };
            var successResponse = new SuccessResponseDTO<int> { Data = 1 };
            _approvalServiceMock.Setup(service => service.RejectRequestAsync(approval))
                                .ReturnsAsync(successResponse);

            // Act
            var result = await _approvalController.RejectRequest(approval);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(successResponse, okResult.Value);
        }
        
        [Test]
        public async Task RejectRequest_ReturnsBadObjectResult_WhenRejectionSucceeds()
        {
            // Arrange
            var approval = new ApprovalStageDTO { RequestId = 1, ReviewId = 1 };
            var successResponse = new SuccessResponseDTO<int> { Data = 1 };
            _approvalServiceMock.Setup(service => service.RejectRequestAsync(approval))
                                .Throws(new Exception("Cannnot reject the request"));

            // Act
            var result = await _approvalController.RejectRequest(approval);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber); ;
        }

        [Test]
        public async Task GetHrPendingApproval_ReturnsOkResult_WithPendingApprovals()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var approvals = new PaginatedResultDTO<ResponseApprovalStageDTO>();
            _approvalServiceMock.Setup(service => service.GetHrPendingApprovalsAsync(pageNumber, pageSize))
                                .ReturnsAsync(approvals);

            // Act
            var result = await _approvalController.GetHrPendingApproval(pageNumber, pageSize);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(approvals, okResult.Value);
        }

        [Test]
        public async Task GetHrPendingApproval_ReturnsBadObjectResult_WithPendingApprovals()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var approvals = new PaginatedResultDTO<ResponseApprovalStageDTO>();
            _approvalServiceMock.Setup(service => service.GetHrPendingApprovalsAsync(pageNumber, pageSize))
                                .Throws(new Exception("Something Error "));

            // Act
            var result = await _approvalController.GetHrPendingApproval(pageNumber, pageSize);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber); 
        }

        [Test]
        public async Task GetFinacePendingApproval_ReturnsOkResult_WithPendingApprovals()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var approvals = new PaginatedResultDTO<ResponseApprovalStageDTO>();
            _approvalServiceMock.Setup(service => service.GetFinancePendingApprovalsAsync(pageNumber, pageSize))
                                .ReturnsAsync(approvals);

            // Act
            var result = await _approvalController.GetFinacePendingApproval(pageNumber, pageSize);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(approvals, okResult.Value);
        }
        [Test]
        public async Task GetFinacePendingApproval_ReturnsBadObjectResult_WithPendingApprovals()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var approvals = new PaginatedResultDTO<ResponseApprovalStageDTO>();
            _approvalServiceMock.Setup(service => service.GetFinancePendingApprovalsAsync(pageNumber, pageSize))
                                .Throws(new Exception("error message"));

            // Act
            var result = await _approvalController.GetFinacePendingApproval(pageNumber, pageSize);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber);
        }
        [Test]
        public async Task GetApprovalByReviewID_ReturnsOKResult()
        {
            var pageNumber = 1;
            var pageSize = 10;
            var approvals = new PaginatedResultDTO<ResponseApprovalStageDTO>();
            _approvalServiceMock.Setup(service => service.GetApprovalsByReviewerIdAsync(1,pageNumber, pageSize))
                                .ReturnsAsync(approvals);

            // Act
            var result = await _approvalController.GetApprovalByReviewId(1,pageNumber, pageSize);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(approvals, okResult.Value);
        }

        [Test]
        public async Task GetApprovalByReviewID_ReturnsBadObjectResult()
        {
            var pageNumber = 1;
            var pageSize = 10;
            var approvals = new PaginatedResultDTO<ResponseApprovalStageDTO>();
            _approvalServiceMock.Setup(service => service.GetApprovalsByReviewerIdAsync(1, pageNumber, pageSize))
                                .Throws(new Exception("Something Error happen"));

            // Act
            var result = await _approvalController.GetApprovalByReviewId(1, pageNumber, pageSize);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber); ;
        }
    }
}

