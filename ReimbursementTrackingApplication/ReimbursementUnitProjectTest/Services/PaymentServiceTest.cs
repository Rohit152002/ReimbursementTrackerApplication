using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Common;
using Moq;
using NUnit.Framework.Interfaces;
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
    internal class PaymentServiceTest
    {
        DbContextOptions options;
        ContextApp context;
        Mock<IMapper> mapper;
        IRepository<int,ReimbursementRequest> requestRepository;
        IRepository<int, Policy> policyRepository;
        IRepository<int, User> userRepository;
        IRepository<int, ReimbursementItem> itemRepository;
        IRepository<int, Payment> paymentRepository;
        IRepository<int, ExpenseCategory> categoryRepository;

        Mock<ILogger<ReimbursementItemRepositories>> itemLogger;
        Mock<ILogger<PolicyRepository>> policyLogger;
        Mock<ILogger<UserRepository>> userLogger;
        Mock<ILogger<ReimbursementRequestRepository>> requestLogger;
        Mock<ILogger<PaymentRepository>> paymentLogger;
        Mock<ILogger<CategoryRepositories>> categoryLogger;

        PaymentService service;

        

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                  .UseInMemoryDatabase("database" + Guid.NewGuid())
                   .Options;
            context = new ContextApp(options);
            mapper = new Mock<IMapper>();
            itemLogger= new Mock<ILogger<ReimbursementItemRepositories>>();
            policyLogger= new Mock<ILogger<PolicyRepository>>();
            userLogger = new Mock<ILogger<UserRepository>>();
            requestLogger = new Mock<ILogger<ReimbursementRequestRepository>>();
            paymentLogger = new Mock<ILogger<PaymentRepository>>();
            categoryLogger = new Mock<ILogger<CategoryRepositories>>();

            itemRepository = new ReimbursementItemRepositories(context, itemLogger.Object);
            policyRepository= new PolicyRepository(context, policyLogger.Object);
            userRepository = new UserRepository(context, userLogger.Object);
            requestRepository= new ReimbursementRequestRepository(context, requestLogger.Object);
            paymentRepository = new PaymentRepository(context, paymentLogger.Object);
            categoryRepository = new CategoryRepositories(context, categoryLogger.Object);
           
            service= new PaymentService(paymentRepository,requestRepository,mapper.Object,policyRepository,userRepository,itemRepository,categoryRepository);
        }

        public async Task AddEverything()
        {
            await AddPayment();
            await AddPolicy();
            await AddRequest();
            await AddItem();
            await AddCategory();
            await AddUser();

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            UserDTO userDTO = new UserDTO
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            List<ResponseReimbursementItemDTO> responseItems = new List<ResponseReimbursementItemDTO> { requestDTO };

            ResponseReimbursementRequestDTO reimRequestDTO = new ResponseReimbursementRequestDTO
            {
                UserId = 1,
                User = userDTO,
                PolicyId = 1,
                PolicyName = "Travel Policy",
                TotalAmount = 9000,
                Comments = "Request for Businees ",
                Items = responseItems
            };
            //ResponseReimbursementRequestDTO requestDTO = _mapper.Map<ResponseReimbursementRequestDTO>(request);
            mapper.Setup(m => m.Map<ResponseReimbursementRequestDTO>(It.IsAny<ReimbursementRequest>())).Returns(reimRequestDTO);

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };

            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            List<ResponseReimbursementItemDTO> itemDTOs = new List<ResponseReimbursementItemDTO>() { itemDTO };

            mapper.Setup(m => m.Map<List<ResponseReimbursementItemDTO>>(It.IsAny<List<ReimbursementItem>>()))
               .Returns(itemDTOs);
            //var userDTO = _mapper.Map<UserDTO>(user);
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);
        }

        public async Task AddPayment()
        {
            Payment payment = new Payment()
            {
                RequestId = 1,
                AmountPaid = 1000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Paid
            };
            var addedPayment = await paymentRepository.Add(payment);
        }

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

            var requestAdded = await itemRepository.Add(item);
            Assert.IsTrue(requestAdded.RequestId == requestAdded.RequestId);

        }
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

            var requestAdded = await requestRepository.Add(request);
            Assert.IsTrue(requestAdded.UserId == requestAdded.UserId);
        }

        public async Task AddPolicy()
        {

            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            var addedPolicy = await policyRepository.Add(policy);
            Assert.IsTrue(addedPolicy.PolicyName == policy.PolicyName);

        }
        public async Task AddCategory()
        {

            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "Meals",
                Description = "this is for meals"
            };
            var addedUser = await categoryRepository.Add(category);
            Assert.IsTrue(addedUser.Name == category.Name);

        }
        public async Task AddUser()
        {

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            Assert.IsTrue(addedUser.UserName == user.UserName);

        }

        [Test]
        public async Task TestDeletePayment()
        {
            await AddPayment();

            var result = await service.DeletePaymentAsync(1);
            Assert.AreEqual(result.Data, 1);
        }

        [Test]
        public async Task TestDeletePaymentException()
        {
         

            Assert.ThrowsAsync<Exception>(async()=> await service.DeletePaymentAsync(1));
           
        }


        [Test]
        public async Task TestGetAllPayments()
        {
            await AddEverything();

            var result = await service.GetAllPaymentsAsync(1, 100);
            Assert.NotNull(result.Data);
        }
        [Test]
        public async Task TestExceptionGetAllPayments()
        {
           

            Assert.ThrowsAsync<Exception>(async()=> await service.GetAllPaymentsAsync(1, 100));
            //Assert.NotNull(result.Data);
        }


        [Test]
        public async Task TestGetPaymentRequestId()
        {
            await AddEverything();

            var result = await service.GetPaymentByRequestIdAsync(1);
            Assert.NotNull(result.Data);
        }

        [Test]
        public async Task TestExceptionGetPaymentRequestId()
        {
            //await AddEverything();

            Assert.ThrowsAsync<Exception>(async () => await service.GetPaymentByRequestIdAsync(1));
        }
        [Test]
        public async Task TestGetPaymentsByUserIdAsync()
        {
            await AddEverything();
            var result = await service.GetPaymentsByUserIdAsync(1, 1, 100);
            Assert.NotNull(result.Data);
        }


        [Test]
        public async Task TestExceptionGetPaymentsByUserIdAsync()
        {
            //await AddEverything();
            Assert.ThrowsAsync<Exception>(async()=> await service.GetPaymentsByUserIdAsync(1, 1, 100));
            
        }

        [Test]
        public async Task TestProcessPayment()
        {
            await AddEverything();
            PaymentDTO payment = new PaymentDTO()
            {
                Id = 1,
                PaymentMethod=PaymentMethod.DirectDeposit,
            };
            var result = await service.ProcessPaymentAsync(payment);
        }

        [Test]
        public async Task TestExceptionProcessPayment()
        {
            //await AddEverything();
            PaymentDTO payment = new PaymentDTO()
            {
                Id = 1,
                PaymentMethod = PaymentMethod.DirectDeposit,
            };
            Assert.ThrowsAsync<Exception>(async()=> await service.ProcessPaymentAsync(payment));
        }
    }
}
