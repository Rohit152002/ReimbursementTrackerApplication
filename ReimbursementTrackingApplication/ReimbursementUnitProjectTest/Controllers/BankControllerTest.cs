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
    internal class BankControllerTest
    {
        private BankController _controller;
        private Mock<IBankService> _mockBankService;

        [SetUp]
        public void SetUp()
        {
            _mockBankService = new Mock<IBankService>();
            _controller = new BankController(_mockBankService.Object);
        }

        [Test]
        public async Task AddBankAccount_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var bankDTO = new BankDTO { UserId = 1, AccNo = "1234567890", BranchName = "Main Branch", IFSCCode = "IFSC001", BranchAddress = "123 Street" };
            var responseDTO = new SuccessResponseDTO<ResponseBankDTO> { Data = new ResponseBankDTO { Id = 1, UserId = 1, AccNo = "1234567890" } };
            _mockBankService.Setup(x => x.AddBankAccountAsync(bankDTO)).ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.AddBankAccount(bankDTO);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(responseDTO, okResult.Value);
        }

        [Test]
        public async Task AddBankAccount_ExceptionThrown_ReturnsNotFoundResult()
        {
            // Arrange
            var bankDTO = new BankDTO { UserId = 1, AccNo = "1234567890", BranchName = "Main Branch" };
            _mockBankService.Setup(x => x.AddBankAccountAsync(bankDTO)).ThrowsAsync(new Exception("Error occurred"));

            // Act
            var result = await _controller.AddBankAccount(bankDTO);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }

        [Test]
        public async Task UpdateBankAccount_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var bankDTO = new BankDTO { UserId = 1, AccNo = "1234567890", BranchName = "Main Branch" };
            var responseDTO = new SuccessResponseDTO<ResponseBankDTO> { Data = new ResponseBankDTO { Id = 1, UserId = 1, AccNo = "1234567890" } };
            _mockBankService.Setup(x => x.UpdateBankAccountAsync(id, bankDTO)).ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.UpdateBankAccount(id, bankDTO);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responseDTO, okResult.Value);
        }

        [Test]
        public async Task DeleteBankAccount_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var responseDTO = new SuccessResponseDTO<int> { Data=1  };


            _mockBankService.Setup(x => x.DeleteBankAccountAsync(id)).ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.DeleteBankAccount(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responseDTO, okResult.Value);
        }

        [Test]
        public async Task GetBankAccountById_ValidId_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var responseDTO = new SuccessResponseDTO<ResponseBankDTO> { Data = new ResponseBankDTO { Id = 1, UserId = 1, AccNo = "1234567890" } };
            _mockBankService.Setup(x => x.GetBankAccountByIdAsync(id)).ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.GetBankAccountById(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responseDTO, okResult.Value);
        }

        [Test]
        public async Task GetBankAccountByUserId_ValidId_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var responseDTO = new SuccessResponseDTO<ResponseBankDTO> { Data = new ResponseBankDTO { Id = 1, UserId = 1, AccNo = "1234567890" } };
            _mockBankService.Setup(x => x.GetBankAccountByUserIdAsync(id)).ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.GetBankAccountByUserId(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responseDTO, okResult.Value);
        }

        [Test]
        public async Task GetAllBankAccounts_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            int pageNumber = 1, pageSize = 10;
            var responseDTO = new PaginatedResultDTO<ResponseBankDTO>
            {
                Data = new List<ResponseBankDTO> { new ResponseBankDTO { Id = 1, UserId = 1, AccNo = "1234567890" } }
            };
            _mockBankService.Setup(x => x.GetAllBankAccountsAsync(pageNumber, pageSize)).ReturnsAsync(responseDTO);

            // Act
            var result = await _controller.GetAllBankAccounts(pageNumber, pageSize);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(responseDTO, okResult.Value);
        }
    }
}
