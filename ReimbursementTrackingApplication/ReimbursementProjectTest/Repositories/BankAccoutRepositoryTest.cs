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
    internal class BankAccoutRepositoryTest
    {
        DbContextOptions options;
        ContextApp context;
        BankAccountRepository repository;
        Mock<ILogger<BankAccountRepository>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger = new Mock<ILogger<BankAccountRepository>>();
            repository = new BankAccountRepository(context, logger.Object);
        }

        [Test]
        public async Task AddBank()
        {
        BankAccount account = new BankAccount()
            {
            UserId=1,
                AccNo = 2293874242,
                BranchName = "Branch Name",
            IFSCCode = "23LK4LKJF ",
            BranchAddress ="Address",
            };
            var addedBank = await repository.Add(account);
            Assert.IsTrue(addedBank.BranchName == account.BranchName);

        }

        [Test]
        public async Task AddAccountException()
        {

            BankAccount account = new BankAccount()
            {
                UserId = 1,
                AccNo = 2293874242,
                BranchName = null,
                IFSCCode = "23LK4LKJF ",
                BranchAddress = "Address",
            };
            
            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(account));
        }

        [Test]
        public async Task DeleteAccount()
        {
            BankAccount account = new BankAccount()
            {
                UserId = 1,
                AccNo = 2293874242,
                BranchName = "branchname",
                IFSCCode = "23LK4LKJF ",
                BranchAddress = "Address",
            };

            var addedAccount = await repository.Add(account);
            var deleteAccount = await repository.Delete(addedAccount.Id);
            Assert.AreEqual(addedAccount.BranchAddress, deleteAccount.BranchAddress);
        }

        [Test]
        public async Task DeleteAccountExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }
        [Test]
        public async Task GetAllAccount()
        {
            BankAccount account = new BankAccount()
            {
                UserId = 1,
                AccNo = 2293874242,
                BranchName = "branchname",
                IFSCCode = "23LK4LKJF ",
                BranchAddress = "Address",
            };
            await repository.Add(account);
            var accounts = await repository.GetAll();
            Assert.NotNull(accounts);
        }
        [Test]
        public async Task GetAllAccountException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdateAccount()
        {
            BankAccount account = new BankAccount()
            {
                UserId = 1,
                AccNo = 2293874242,
                BranchName = "branchname",
                IFSCCode = "23LK4LKJF ",
                BranchAddress = "Address",
            };
            await repository.Add(account);

            BankAccount updateAccount = new BankAccount()
            {
                UserId = 1,
                AccNo = 2293874242,
                BranchName = "updatedbranchname",
                IFSCCode = "23LK4LKJF ",
                BranchAddress = "Address",
            };
            var update = await repository.Update(1, updateAccount);
            Assert.AreEqual(update.BranchName, updateAccount.BranchName);
        }

        [Test]
        public async Task UpdateAccountExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new BankAccount { BranchAddress = "updatedAddress " }));

        }

        [Test]
        public async Task GetAccountTest()
        {
            BankAccount account = new BankAccount()
            {
                UserId = 1,
                AccNo = 2293874242,
                BranchName = "updatedbranchname",
                IFSCCode = "23LK4LKJF ",
                BranchAddress = "Address",
            };
            await repository.Add(account);
            var getApproval = await repository.Get(1);
            Assert.IsNotNull(getApproval);
        }

        [Test]
        public async Task GetAccountExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Get(1));
        }
    }
}
