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
    internal class PolicyRepositoryTest
    {
        DbContextOptions options;
        ContextApp context;
        PolicyRepository repository;
        Mock<ILogger<PolicyRepository>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger = new Mock<ILogger<PolicyRepository>>();
            repository = new PolicyRepository(context, logger.Object);
        }

        [Test]
        public async Task AddPolicy()
        {

            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount=10000
            };
            var addedPolicy = await repository.Add(policy);
            Assert.IsTrue(addedPolicy.PolicyName == policy.PolicyName);

        }

        [Test]
        public async Task AddPolicyException()
        {

            Policy policy = new Policy()
            {
                PolicyName = null,
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };

            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(policy));
        }

        [Test]
        public async Task DeletePolicy()
        {

            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            var addedPolicy = await repository.Add(policy);
            var deletePolicy = await repository.Delete(addedPolicy.Id);
            Assert.AreEqual(addedPolicy.MaxAmount, deletePolicy.MaxAmount);
        }

        [Test]
        public async Task DeletePolicyExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }
        [Test]
        public async Task GetAllPolicy()
        {
            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            await repository.Add(policy);
            var policies = await repository.GetAll();
            Assert.NotNull(policies);
        }
        [Test]
        public async Task GetAllPolicyException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdatePolicy()
        {
            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            await repository.Add(policy);

            Policy updatePolicy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 40000
            };
            var update = await repository.Update(1, updatePolicy);
            Assert.AreEqual(update.MaxAmount, updatePolicy.MaxAmount);
        }

        [Test]
        public async Task UpdateUserExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new Policy { PolicyName = "hello " }));

        }

        [Test]
        public async Task GetUser()
        {
            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            await repository.Add(policy);
            var getUser = await repository.Get(1);
            Assert.IsNotNull(getUser);
        }

        [Test]
        public async Task GetUserExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Get(1));
        }
    }
}
