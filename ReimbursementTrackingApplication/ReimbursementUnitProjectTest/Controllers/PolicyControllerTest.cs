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
    internal class PolicyControllerTest
    {
        private Mock<IPolicyService> _mockPolicyService;
        private PolicyController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockPolicyService = new Mock<IPolicyService>();
            _controller = new PolicyController(_mockPolicyService.Object);
        }

        [Test]
        public async Task GetPolicyByid_ShouldReturnOkResult_WhenPolicyExists()
        {
            // Arrange
            int policyId = 1;
            var responsePolicyDTO = new SuccessResponseDTO<ResponsePolicyDTO>
            {
                Data = new ResponsePolicyDTO { Id = policyId, PolicyName = "Policy 1", MaxAmount = 5000 }
            };
            _mockPolicyService.Setup(service => service.GetPolicyByIdAsync(policyId))
                .ReturnsAsync(responsePolicyDTO);

            // Act
            var result = await _controller.GetPolicyByid(policyId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responsePolicyDTO, okResult.Value);
        }

        [Test]
        public async Task GetPolicyByid_ShouldReturnNotFoundResult_WhenExceptionIsThrown()
        {
            // Arrange
            int policyId = 1;
            _mockPolicyService.Setup(service => service.GetPolicyByIdAsync(policyId))
                .ThrowsAsync(new Exception("Policy not found"));

            // Act
            var result = await _controller.GetPolicyByid(policyId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.AreEqual("Policy not found", ((ErrorResponseDTO)notFoundResult.Value).ErrorMessage);
        }

        [Test]
        public async Task GetAllPolicies_ShouldReturnOkResult_WhenPoliciesExist()
        {
            // Arrange
            var responsePolicies = new PaginatedResultDTO<ResponsePolicyDTO>
            {
                Data = new List<ResponsePolicyDTO>() { new ResponsePolicyDTO { Id = 1, PolicyName = "Policy 1", MaxAmount = 5000 } },
                TotalCount = 1
            };
            _mockPolicyService.Setup(service => service.GetAllPolicesAsync(1, 10))
                .ReturnsAsync(responsePolicies);

            // Act
            var result = await _controller.GetAllPolicies(1, 10);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responsePolicies, okResult.Value);
        } 
        
        [Test]
        public async Task GetAllPolicies_ShouldReturnBadObjectReturn_WhenPoliciesExist()
        {
            // Arrange
            var responsePolicies = new PaginatedResultDTO<ResponsePolicyDTO>
            {
                Data = new List<ResponsePolicyDTO>() { new ResponsePolicyDTO { Id = 1, PolicyName = "Policy 1", MaxAmount = 5000 } },
                TotalCount = 1
            };
            _mockPolicyService.Setup(service => service.GetAllPolicesAsync(1, 10))
                .Throws(new Exception("error"));

            // Act
            var result = await _controller.GetAllPolicies(1, 10);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber);
        }

        [Test]
        public async Task AddPolicies_ShouldReturnOkResult_WhenPolicyIsAdded()
        {
            // Arrange
            var policyDTO = new CreatePolicyDTO { PolicyName = "New Policy", MaxAmount = 1000, PolicyDescription = "Description" };
            var successResponse = new SuccessResponseDTO<int> { Data = 1 };
            _mockPolicyService.Setup(service => service.AddPolicyAsync(policyDTO))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.AddPolicies(policyDTO);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(successResponse, okResult.Value);
        }
        [Test]
        public async Task AddPolicies_ShouldReturnBadObjectResult_WhenPolicyIsAdded()
        {
            // Arrange
            var policyDTO = new CreatePolicyDTO { PolicyName = "New Policy", MaxAmount = 1000, PolicyDescription = null };
            var successResponse = new SuccessResponseDTO<int> { Data = 1 };
            _mockPolicyService.Setup(service => service.AddPolicyAsync(policyDTO))
                .Throws(new Exception("error"));

            // Act
            var result = await _controller.AddPolicies(policyDTO);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

            // Assert
            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult.Value as ErrorResponseDTO;
            Assert.AreEqual(404, errorResponse.ErrorNumber);
        }

        [Test]
        public async Task UpdatePolicy_ShouldReturnOkResult_WhenPolicyIsUpdated()
        {
            // Arrange
            int policyId = 1;
            var policyDTO = new CreatePolicyDTO { PolicyName = "Updated Policy", MaxAmount = 2000, PolicyDescription = "Updated Description" };
            var successResponse = new SuccessResponseDTO<int> { Data = policyId };
            _mockPolicyService.Setup(service => service.UpdatePolicyAsync(policyId, policyDTO))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.UpdatePolicy(policyId, policyDTO);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(successResponse, okResult.Value);
        }

        [Test]
        public async Task DeletePolicy_ShouldReturnOkResult_WhenPolicyIsDeleted()
        {
            // Arrange
            int policyId = 1;
            var successResponse = new SuccessResponseDTO<int> { Data = policyId };
            _mockPolicyService.Setup(service => service.DeletePolicyAsync(policyId))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.Delete(policyId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(successResponse, okResult.Value);
        }
    }
}
