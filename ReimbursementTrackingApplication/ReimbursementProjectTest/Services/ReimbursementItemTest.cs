using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Interfaces;
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
    internal class ReimbursementItemTest
    {

        DbContextOptions options;
        ContextApp context;
        CategoryRepositories categoryRepository;
        ReimbursementItemRepositories reimbursementItemRepositories;
        Mock<IMapper> mapper;
        Mock<ILogger<CategoryRepositories>> categoryLogger;
        Mock<ILogger<ReimbursementItemRepositories>> reimbursementItemLogger;
        string uploadFolder;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                .UseInMemoryDatabase("database" + Guid.NewGuid())
                 .Options;
            context = new ContextApp(options);
            categoryLogger = new Mock<ILogger<CategoryRepositories>>();
            reimbursementItemLogger= new Mock<ILogger<ReimbursementItemRepositories>>();
            mapper = new Mock<IMapper>();
            uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "File");
            categoryRepository = new CategoryRepositories(context, categoryLogger.Object);
            reimbursementItemRepositories= new ReimbursementItemRepositories(context, reimbursementItemLogger.Object);


        }
        [Test]
        public async Task AddItemTest()
        {
            
            var formFile = new Mock<IFormFile>();
            var fileName = "testfile.jpg";
            var fileContent = "This is a test file content";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(fileContent);
            writer.Flush();
            stream.Position = 0; // Reset stream position

            formFile.Setup(f => f.Length).Returns(stream.Length);
            formFile.Setup(f => f.FileName).Returns(fileName);
            formFile.Setup(f => f.OpenReadStream()).Returns(stream);
            formFile.Setup(f => f.ContentType).Returns("image/jpeg");

            ReimbursementItemDTO item = new ReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = formFile.Object
            };
            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };

            ReimbursementItem itemObject = new ReimbursementItem
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };


            // Ensure this setup matches the actual object you're using in AddItemAsync
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                  .Returns(itemDTO);

            IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
            var itemAdded = await service.AddItemAsync(item);

            Assert.NotNull(itemAdded.Data);
            Assert.IsTrue(itemAdded.IsSuccess);

        }
        [Test]
        public async Task AddItemTestException()
        {

            var formFile = new Mock<IFormFile>();
            var fileName = "testfile.jpg";
            var fileContent = "This is a test file content";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(fileContent);
            writer.Flush();
            stream.Position = 0; // Reset stream position

            formFile.Setup(f => f.Length).Returns(stream.Length);
            formFile.Setup(f => f.FileName).Returns(fileName);
            formFile.Setup(f => f.OpenReadStream()).Returns(stream);
            formFile.Setup(f => f.ContentType).Returns("image/jpeg");

            ReimbursementItemDTO item = new ReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = null
            };

            IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
           Assert.ThrowsAsync<Exception>(async()=> await service.AddItemAsync(item));

        }

        [Test]
        public async Task DeleteItemAsyncTest()
        {
            var formFile = new Mock<IFormFile>();
            var fileName = "testfile.jpg";
            var fileContent = "This is a test file content";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(fileContent);
            writer.Flush();
            stream.Position = 0; // Reset stream position

            formFile.Setup(f => f.Length).Returns(stream.Length);
            formFile.Setup(f => f.FileName).Returns(fileName);
            formFile.Setup(f => f.OpenReadStream()).Returns(stream);
            formFile.Setup(f => f.ContentType).Returns("image/jpeg");

            ReimbursementItemDTO item = new ReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = formFile.Object
            };

            IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
            var itemAdded = await service.AddItemAsync(item);

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };

            ReimbursementItem itemObject = new ReimbursementItem
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                //receiptFile = "image.jpg"
            };

            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                 .Returns(itemDTO);
            var result = service.DeleteItemAsync(1);
            Assert.NotNull(result.Result.Data);
            Assert.IsTrue(result.Result.IsSuccess);
        }
        [Test]
        public async Task DeleteItemAsyncTestException()
        {
            

            IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
          ;

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                //receiptFile = "image.jpg"
            };

            ReimbursementItem itemObject = new ReimbursementItem
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                //receiptFile = "image.jpg"
            };

            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(itemObject)).Returns(itemDTO);
           Assert.ThrowsAsync<Exception>(async()=> await service.DeleteItemAsync(1));
            
        }


    }
}
