using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementProjectTest.Repositories
{
    internal class ReimbursementItemRepositoryTest
    {

        DbContextOptions options;
        ContextApp context;
        ReimbursementItemRepositories repository;
        Mock<ILogger<ReimbursementItemRepositories>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger = new Mock<ILogger<ReimbursementItemRepositories>>();
            repository = new ReimbursementItemRepositories(context, logger.Object);
        }

        [Test]
        public async Task AddItem()
        {

            ReimbursementItem item = new ReimbursementItem()
            {
                RequestId = 1,
                Amount = 150,
                CategoryId = 1,
                Description = "Office supplies purchased for the project.",
                receiptFile = "hello.jpg",
                //DateIncurred = DateTime.Now,
            };

            var requestAdded = await repository.Add(item);
            Assert.IsTrue(requestAdded.RequestId == requestAdded.RequestId);

        }

        [Test]
        public async Task AddItemException()
        {

            ReimbursementItem item = new ReimbursementItem()
            {
                Id = 1,
                RequestId = 1,
                Amount = 150.75,
                Description = null,
                DateIncurred = DateTime.UtcNow,
                CategoryId = 1,
                receiptFile = "hello.jpg"
            };
            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(item));
        }

        [Test]
        public async Task DeleteItem()
        {
            ReimbursementItem item = new ReimbursementItem()
            {
                Id = 1,
                RequestId = 1,
                Amount = 150.75,
                Description = "description",
                DateIncurred = DateTime.UtcNow,
                CategoryId = 1,
                receiptFile = "hello.jpg"
            };

            var requestAdded = await repository.Add(item);
            var deleteUser = await repository.Delete(requestAdded.Id);
            Assert.AreEqual(requestAdded.Id, deleteUser.Id);
        }

        [Test]
        public async Task DeleteItemExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }
        [Test]
        public async Task GetAllItem()
        {
            ReimbursementItem item = new ReimbursementItem()
            {
                RequestId = 1,
                Amount = 150.75,
                Description = "description",
                DateIncurred = DateTime.UtcNow,
                CategoryId = 1,
                receiptFile = "hello.jpg"
            };

            var requestAdded = await repository.Add(item);
            var requests = await repository.GetAll();
            Assert.NotNull(requests);
        }
        [Test]
        public async Task GetAllItemException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdateRequest()
        {
            ReimbursementItem item = new ReimbursementItem()
            {
                //Id = 1,
                RequestId = 1,
                Amount = 150.75,
                Description = "description",
                CategoryId = 1,
                receiptFile = "hello.jpg"
            };

            await repository.Add(item);

            ReimbursementItem updateItem = new ReimbursementItem()
            {
                //Id = 1,
                RequestId = 1,
                Amount = 120.75,
                Description = "hello",
                CategoryId = 1,
                receiptFile = "hello.jpg"
            };
            var update = await repository.Update(1, updateItem);
            Assert.AreEqual(update.Amount, updateItem.Amount);
        }

        [Test]
        public async Task UpdateRequestExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new ReimbursementItem { Id = 2 }));

        }

        [Test]
        public async Task GetRequest()
        {
            ReimbursementItem item = new ReimbursementItem()
            {
                Id = 1,
                RequestId = 1,
                Amount = 150.75,
                Description = "description",
                DateIncurred = DateTime.UtcNow,
                CategoryId = 1,
                receiptFile = "hello.jpg"
            };

            var requestAdded = await repository.Add(item);
            var getRequest = await repository.Get(1);
            Assert.IsNotNull(getRequest);
        }

        [Test]
        public async Task GetRequestExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Get(1));
        }
    }
}
