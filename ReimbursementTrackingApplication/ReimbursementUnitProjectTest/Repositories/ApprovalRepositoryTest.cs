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

namespace ReimbursementUnitProjectTest.Repositories
{
    internal class ApprovalRepositoryTest
    {
        DbContextOptions options;
        ContextApp context;
        ApprovalStageRepository repository;
        Mock<ILogger<ApprovalStageRepository>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger = new Mock<ILogger<ApprovalStageRepository>>();
            repository = new ApprovalStageRepository(context, logger.Object);
        }

        [Test]
        public async Task AddApprovals()
        {
        
        ApprovalStage approval = new ApprovalStage()
            {
                RequestId =1,
                ReviewId = 2,
                Stage=Stage.Hr,
                Comments="Processing ",
                Status=Status.Approved,
            };
            var addedUser = await repository.Add(approval);
            Assert.IsTrue(addedUser.RequestId == approval.RequestId);

        }

        [Test]
        public async Task AddApprovalException()
        {

            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 2,
                Stage = Stage.Hr,
                Comments =null,
                Status = Status.Approved,
            };

            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(approval));
        }

        [Test]
        public async Task DeleteApproval()
        {
            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 2,
                Stage = Stage.Hr,
                Comments = "Processing ",
                Status = Status.Approved,
            };
            var addedApproval = await repository.Add(approval);
            var deleteApproval = await repository.Delete(addedApproval.RequestId);
            Assert.AreEqual(approval.RequestId, deleteApproval.RequestId);
        }

        [Test]
        public async Task DeleteApprovalExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }
        [Test]
        public async Task GetAllApproval()
        {
            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 2,
                Stage = Stage.Hr,
                Comments = "Processing ",
                Status = Status.Approved,
            };
            await repository.Add(approval);
            var categories = await repository.GetAll();
            Assert.NotNull(categories);
        }
        [Test]
        public async Task GetAllApprovalException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdateApproval()
        {
            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 2,
                Stage = Stage.Hr,
                Comments = "Processing ",
                Status = Status.Pending,
            };
            await repository.Add(approval);

            ApprovalStage updatedApproval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 2,
                Stage = Stage.Hr,
                Comments = "Processing ",
                Status = Status.Approved,
            };
            var update = await repository.Update(1, updatedApproval);
            Assert.AreEqual(update.Status, Status.Approved);
        }

        [Test]
        public async Task UpdateApprovalExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new ApprovalStage { Comments = "hello " }));

        }

        [Test]
        public async Task GetApprovalTest()
        {
            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 2,
                Stage = Stage.Hr,
                Comments = "Processing ",
                Status = Status.Pending,
            };
            await repository.Add(approval);
            var getApproval = await repository.Get(1);
            Assert.IsNotNull(getApproval);
        }

        [Test]
        public async Task GetaApprovalExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Get(1));
        }
    }
}
