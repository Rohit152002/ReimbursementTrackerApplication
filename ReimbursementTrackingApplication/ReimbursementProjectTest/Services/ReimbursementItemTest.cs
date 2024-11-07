using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        IReimbursementItemService service;
        string uploadFolder;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                .UseInMemoryDatabase("database" + Guid.NewGuid())
                 .Options;
            context = new ContextApp(options);
            categoryLogger = new Mock<ILogger<CategoryRepositories>>();
            reimbursementItemLogger = new Mock<ILogger<ReimbursementItemRepositories>>();
            mapper = new Mock<IMapper>();
            uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "File");
            categoryRepository = new CategoryRepositories(context, categoryLogger.Object);
            reimbursementItemRepositories = new ReimbursementItemRepositories(context, reimbursementItemLogger.Object);
            service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);


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
                Id = 1,
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

            //IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
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

            //IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
            Assert.ThrowsAsync<Exception>(async () => await service.AddItemAsync(item));

        }

        [Test]
        public async Task DeleteItemAsyncTest()
        {
            var itemAdded = await AddItem();
            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "Meals",
                Description = "this is for meals"
            };
            var addedUser = await categoryRepository.Add(category);

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
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
            var result = await service.DeleteItemAsync(1);
            Assert.NotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
        }
        [Test]
        public async Task DeleteItemAsyncTestException()
        {


            //IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);


            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
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

            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>())).Returns(itemDTO);
            Assert.ThrowsAsync<Exception>(async () => await service.DeleteItemAsync(1));

        }


        //public async Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> GetItemByIdAsync(int itemId)
        [Test]
        public async Task GetItemByAsyncTest()
        {
            //var formFile = new Mock<IFormFile>();
            //var fileName = "testfile.jpg";
            //var fileContent = "This is a test file content";
            //var stream = new MemoryStream();
            //var writer = new StreamWriter(stream);
            //writer.Write(fileContent);
            //writer.Flush();
            //stream.Position = 0; // Reset stream position

            //formFile.Setup(f => f.Length).Returns(stream.Length);
            //formFile.Setup(f => f.FileName).Returns(fileName);
            //formFile.Setup(f => f.OpenReadStream()).Returns(stream);
            //formFile.Setup(f => f.ContentType).Returns("image/jpeg");

            //ReimbursementItemDTO item = new ReimbursementItemDTO
            //{
            //    RequestId = 1,
            //    Amount = 9000,
            //    CategoryId = 1,
            //    Description = "Travelling",
            //    receiptFile = formFile.Object
            //};

            ////IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
            //var itemAdded = await service.AddItemAsync(item);
            var itemAdded = await AddItem();
            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "Meals",
                Description = "this is for meals"
            };
            var addedUser = await categoryRepository.Add(category);
            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };


            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>())).Returns(itemDTO);

            var result = await service.GetItemByIdAsync(1);
            Assert.NotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task GetItemByIDExceptionTest()
        {

            Assert.ThrowsAsync<Exception>(async () => await service.GetItemByIdAsync(1));
        }


        //public async Task<PaginatedResultDTO<ResponseReimbursementItemDTO>> GetItemsByRequestIdAsync(int requestId, int pageNumber, int pageSize)

        [Test]
        public async Task GetItemsByRequestIdTest()
        {


            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {
                                     Id=1,
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);

            await AddCategory();
            await AddItem();

            PaginatedResultDTO<ResponseReimbursementItemDTO> results =  await service.GetItemsByRequestIdAsync(1, 1, 10);
            Assert.NotNull(results.Data);
        }

        [Test]
        public async Task GetAllItemsTest()
        {


            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {
                                 Id=1,
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);

            await AddCategory();
            await AddItem();

            PaginatedResultDTO<ResponseReimbursementItemDTO> results = await service.GetAllItems(1, 10);
            Assert.NotNull(results.Data);
        }
        [Test]
        public async Task GetAllItemsTestException()
        {


            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {
                                 Id=1,
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);

            //await AddCategory();
            //await AddItem();
            Assert.ThrowsAsync<Exception>(async()=>await service.GetAllItems(1, 10));
         
        }

        [Test]
        public async Task ExceptionTestGetRequestById()
        {
            Assert.ThrowsAsync<Exception>(async () => await service.GetItemsByRequestIdAsync(1, 1, 10));
        }
        //public async Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> UpdateItemAsync(int itemId, ReimbursementItemDTO itemDto)

        [Test]
        public async Task UpdateItemTest()
        {
            await AddItem();
            await AddCategory();

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

            ReimbursementItemDTO UpdatedItem = new ReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 8000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = formFile.Object
            };
            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>())).Returns(itemDTO);
             var result = await service.UpdateItemAsync(1, UpdatedItem);
            Assert.NotNull(result.Data);
        }
        [Test]
        public async Task UpdateItemTestException()
        {
            await AddItem();
            await AddCategory();

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

            ReimbursementItemDTO UpdatedItem = new ReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 8000,
                CategoryId = 1,
                Description = null,
                receiptFile = formFile.Object
            };
            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description =null,
                receiptFile = "image.jpg"
            };
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>())).Returns(itemDTO);
           
            Assert.ThrowsAsync<Exception>(async () => await service.UpdateItemAsync(1, UpdatedItem));
        }

        public async Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> AddItem()
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

            //IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
            var itemAdded = await service.AddItemAsync(item);
            return itemAdded;
        }

        public async Task AddCategory()
        {
            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "Meals",
                Description = "this is for meals"
            };
            var addedCategory = await categoryRepository.Add(category);
        }


    }
}
