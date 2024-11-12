using AutoMapper;
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
    class BankServiceTest
    {
        DbContextOptions options;
        ContextApp context;

        BankAccountRepository bankAccountRepository;
        UserRepository userRepository;

        Mock<IMapper> mapper;

        Mock<ILogger<BankAccountRepository>> bankLogger;
        Mock<ILogger<UserRepository>> userLogger;

        BankService bankService;
        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                 .UseInMemoryDatabase("database" + Guid.NewGuid())
                  .Options;
            context = new ContextApp(options);

            bankLogger= new Mock<ILogger<BankAccountRepository>>();
            userLogger= new Mock<ILogger<UserRepository>>(); 
            mapper = new Mock<IMapper>();

            bankAccountRepository = new BankAccountRepository(context, bankLogger.Object);
            userRepository= new UserRepository(context, userLogger.Object);

            bankService = new BankService(userRepository, bankAccountRepository, mapper.Object);

        }

        [Test]
        public async Task AddBankTest()
        {
            BankDTO bankDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName="Bank of Baroda",
                IFSCCode="KSD213KJN",
                UserId=1
            };

            BankAccount bankAccount = new BankAccount()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1,         
            };

            mapper.Setup(m => m.Map<BankAccount>(It.IsAny<BankDTO>())).Returns(bankAccount);
            User user = new User()
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            var result= await bankService.AddBankAccountAsync(bankDTO); 
            Assert.NotNull(result.Data);
        }

        [Test]
        public async Task DeleteBankTest()
        {
            BankDTO bankDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1
            };

            BankAccount bankAccount = new BankAccount()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1,
            };
            mapper.Setup(m => m.Map<BankAccount>(It.IsAny<BankDTO>())).Returns(bankAccount);
            User user = new User()
            {
                UserName = "tests",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            var added = await bankService.AddBankAccountAsync(bankDTO);
            var result = await bankService.DeleteBankAccountAsync(added.Data.Id);
            Assert.AreEqual(added.Data.Id, result.Data);
        }

        [Test]
        public async Task GetAllBank()
        {
            await AddBankTest();
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            //var banksDTO = _mapper.Map<List<ResponseBankDTO>>(banks);
            List<ResponseBankDTO> responsesBank = new List<ResponseBankDTO>()
                { new ResponseBankDTO()
                        {
                           Id = 1,
                           BranchAddress="TEST ADDRESS",
                           AccNo="12312423",
                           BranchName="Test branch",
                           IFSCCode="FSD23FDS",
                           User=userDTO,
                           UserId=1,
                        }
            };

            mapper.Setup(m => m.Map<List<ResponseBankDTO>>(It.IsAny<List<BankAccount>>())).Returns(responsesBank);

            var result = await bankService.GetAllBankAccountsAsync(1, 100);
            Assert.NotNull(result.Data);



        }

        [Test]
        public async Task GetBankAccountById()
        {
            BankDTO bankDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1
            };

            BankAccount bankAccount = new BankAccount()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1,
            };

            mapper.Setup(m => m.Map<BankAccount>(It.IsAny<BankDTO>())).Returns(bankAccount);
            User user = new User()
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            var added = await bankService.AddBankAccountAsync(bankDTO);
            var result = await bankService.GetBankAccountByIdAsync(added.Data.Id);
            Assert.NotNull(result.Data);
        }

        [Test]
        public async Task TestGetBankAccountByUserId()
        {
            await AddBankTest();
            var result = await bankService.GetBankAccountByUserIdAsync(1);
            Assert.NotNull(result.Data);
        }

        [Test]
        public async Task TestUpdateBankAccount()
        {
            BankDTO bankDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1
            };

            BankAccount bankAccount = new BankAccount()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1,
            };

            mapper.Setup(m => m.Map<BankAccount>(It.IsAny<BankDTO>())).Returns(bankAccount);
            User user = new User()
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            var added = await bankService.AddBankAccountAsync(bankDTO);
            BankDTO updatedDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KFJD87AJKDS",
                UserId = 1
            };

            var updateresult = await bankService.UpdateBankAccountAsync(added.Data.Id, updatedDTO);
            Assert.NotNull(updateresult);

        }

        [Test]
        public async Task AddBankTestException()
        {
            BankDTO bankDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1
            };

            BankAccount bankAccount = new BankAccount()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = null,
                UserId = 1,
            };

            mapper.Setup(m => m.Map<BankAccount>(It.IsAny<BankDTO>())).Returns(bankAccount);
            User user = new User()
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            Assert.ThrowsAsync<Exception>(async()=>  await bankService.AddBankAccountAsync(bankDTO));
            
        }

        [Test]
        public async Task DeleteBankTestException()
        {
            BankDTO bankDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1
            };

            BankAccount bankAccount = new BankAccount()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1,
            };
            mapper.Setup(m => m.Map<BankAccount>(It.IsAny<BankDTO>())).Returns(bankAccount);
            User user = new User()
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            //var added = await bankService.AddBankAccountAsync(bankDTO);
            //var result = await bankService.DeleteBankAccountAsync(added.Data.Id);
            //Assert.AreEqual(added.Data.Id, result.Data);
            Assert.ThrowsAsync<Exception>(async () => await bankService.DeleteBankAccountAsync(1));
        }

        [Test]
        public async Task GetAllBankException()
        {
            //await AddBankTest();
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            //var banksDTO = _mapper.Map<List<ResponseBankDTO>>(banks);
            List<ResponseBankDTO> responsesBank = new List<ResponseBankDTO>()
                { new ResponseBankDTO()
                        {
                           Id = 1,
                           BranchAddress="TEST ADDRESS",
                           AccNo="12312423",
                           BranchName="Test branch",
                           IFSCCode="FSD23FDS",
                           User=userDTO,
                           UserId=1,
                        }
            };

            mapper.Setup(m => m.Map<List<ResponseBankDTO>>(It.IsAny<List<BankAccount>>())).Returns(responsesBank);

            Assert.ThrowsAsync<Exception>(async () => await bankService.GetAllBankAccountsAsync(1,100));



        }

        [Test]
        public async Task GetBankAccountByIdTest()
        {
            BankDTO bankDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1
            };

            BankAccount bankAccount = new BankAccount()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1,
            };

            mapper.Setup(m => m.Map<BankAccount>(It.IsAny<BankDTO>())).Returns(bankAccount);
            User user = new User()
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            //var added = await bankService.AddBankAccountAsync(bankDTO);
            //var result = await bankService.GetBankAccountByIdAsync(added.Data.Id);
            //Assert.NotNull(result.Data);/
            Assert.ThrowsAsync<Exception>(async () => await bankService.GetBankAccountByIdAsync(1));
        }

        [Test]
        public async Task TestGetBankAccountByUserIdException()
        {
            //await AddBankTest();
            //var result = await bankService.GetBankAccountByUserIdAsync(1);
            Assert.ThrowsAsync<Exception>(async () => await bankService.GetBankAccountByUserIdAsync(1));
            //Assert.NotNull(result.Data);
        }

        [Test]
        public async Task TestExceptionUpdateBankAccount()
        {
            BankDTO bankDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1
            };

            BankAccount bankAccount = new BankAccount()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KSD213KJN",
                UserId = 1,
            };

            mapper.Setup(m => m.Map<BankAccount>(It.IsAny<BankDTO>())).Returns(bankAccount);
            User user = new User()
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            //var added = await bankService.AddBankAccountAsync(bankDTO);
            BankDTO updatedDTO = new BankDTO()
            {
                AccNo = "917283192873",
                BranchAddress = "Test Address",
                BranchName = "Bank of Baroda",
                IFSCCode = "KFJD87AJKDS",
                UserId = 1
            };

            //var updateresult = await bankService.UpdateBankAccountAsync(added.Data.Id, updatedDTO);
            Assert.ThrowsAsync<Exception>(async () => await bankService.UpdateBankAccountAsync(1,updatedDTO));
            //Assert.NotNull(updateresult);

        }





    }
}
