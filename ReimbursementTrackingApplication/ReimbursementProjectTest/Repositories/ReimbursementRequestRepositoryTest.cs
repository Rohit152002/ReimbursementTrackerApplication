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
    internal class ReimbursementRequestRepositoryTest
    {
        DbContextOptions options;
        ContextApp context;
        ReimbursementRequestRepository repository;
        Mock<ILogger<ReimbursementRequestRepository>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger = new Mock<ILogger<ReimbursementRequestRepository>>();
            repository = new ReimbursementRequestRepository(context, logger.Object);
        }

        [Test]
        public async Task AddRequest()
        {

            ReimbursementRequest request = new ReimbursementRequest()
            {
                UserId = 1, 
                PolicyId = 1,  
                TotalAmount = 150.75, 
                Comments = "Office supplies purchased for the project.",
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };

            var requestAdded = await repository.Add(request);
            Assert.IsTrue(requestAdded.UserId == requestAdded.UserId);

        }

        [Test]
        public async Task AddRequestException()
        {
            ReimbursementRequest request = new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 150.75,
                Comments = null,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };

            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(request));
        }

        [Test]
        public async Task DeleteRequest()
        {
            ReimbursementRequest request = new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 150.75,
                Comments = "Office supplies purchased for the project.",
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };

            var requestAdded = await repository.Add(request);
            var deleteUser = await repository.Delete(requestAdded.Id);
            Assert.AreEqual(requestAdded.UserId, deleteUser.UserId);
        }

        [Test]
        public async Task DeleteRequestExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }
        [Test]
        public async Task GetAllRequest()
        {
            ReimbursementRequest request = new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 150.75,
                Comments = "Office supplies purchased for the project.",
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };

            var requestAdded = await repository.Add(request);
            var requests = await repository.GetAll();
            Assert.NotNull(requests);
        }
        [Test]
        public async Task GetAllRequestException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdateRequest()
        {
            ReimbursementRequest request = new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 150.75,
                Comments = "Office supplies purchased for the project.",
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };

             await repository.Add(request);

            ReimbursementRequest updateRequest = new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 150.75,
                Comments = "Office supplies purchased for the project.",
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Passed
            };
            var update = await repository.Update(1, updateRequest);
            Assert.AreEqual(update.Status, RequestStatus.Passed);
        }

        [Test]
        public async Task UpdateRequestExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new ReimbursementRequest { UserId = 2 }));

        }

        [Test]
        public async Task GetRequest()
        {
            ReimbursementRequest request = new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 150.75,
                Comments = "Office supplies purchased for the project.",
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };

            var requestAdded = await repository.Add(request);
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
