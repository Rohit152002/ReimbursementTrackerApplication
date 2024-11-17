using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
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
    internal class ApprovalServiceTest
    {

        DbContextOptions options;
        ContextApp context;

        Mock<ILogger<ApprovalStageRepository>> approvalLogger;
        Mock<ILogger<UserRepository>> userLogger;
        Mock<ILogger<ReimbursementRequestRepository>> requestLogger;
        Mock<ILogger<ReimbursementItemRepositories>> itemLogger;
        Mock<ILogger<CategoryRepositories>> categoryLogger;
        Mock<ILogger<PolicyRepository>> policyLogger;
        Mock<ILogger<PaymentRepository>> paymentLogger;
        Mock<ILogger<EmployeeRepository>> employeeLogger;

        ApprovalStageRepository repository;
       UserRepository userRepository;
        ReimbursementRequestRepository requestRepository;
        CategoryRepositories categoryRepositories;
        ReimbursementItemRepositories itemRepositories;
        EmployeeRepository employeeRepository;
        PolicyRepository policyRepository;
        PaymentRepository paymentRepository;
        Mock<IMapper> mapper;
        Mock<IMailSender> _mockMailSender;
        ApprovalService service;
        ReimbursementItemService itemService;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                  .UseInMemoryDatabase("database" + Guid.NewGuid())
                   .Options;
            context = new ContextApp(options);
            approvalLogger = new Mock<ILogger<ApprovalStageRepository>> ();
            userLogger= new Mock<ILogger<UserRepository>> ();
            requestLogger= new Mock<ILogger<ReimbursementRequestRepository>> ();
            itemLogger= new Mock<ILogger<ReimbursementItemRepositories>>();
            categoryLogger = new Mock<ILogger<CategoryRepositories>>();
            policyLogger = new Mock<ILogger<PolicyRepository>> ();
            paymentLogger= new Mock<ILogger<PaymentRepository>>();
            employeeLogger= new Mock<ILogger<EmployeeRepository>>();

            repository = new ApprovalStageRepository(context, approvalLogger.Object);
            userRepository= new UserRepository(context, userLogger.Object);
            requestRepository= new ReimbursementRequestRepository(context, requestLogger.Object);
            itemRepositories= new ReimbursementItemRepositories(context, itemLogger.Object);
            categoryRepositories= new CategoryRepositories(context, categoryLogger.Object);
            policyRepository = new PolicyRepository(context, policyLogger.Object);
            paymentRepository = new PaymentRepository(context, paymentLogger.Object);
            employeeRepository = new EmployeeRepository(context, employeeLogger.Object);

            _mockMailSender = new Mock<IMailSender>();
            mapper = new Mock<IMapper>();
            itemService = new ReimbursementItemService(itemRepositories, categoryRepositories, "Files", mapper.Object);
            service = new ApprovalService(repository,userRepository,requestRepository,employeeRepository,policyRepository,paymentRepository,itemRepositories,categoryRepositories,mapper.Object,_mockMailSender.Object);

        }

        [Test]
        public async Task TestApproveRequestAsync()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };

            User hr = new User
            {
                UserName = "HR",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "admin@gmail.com",
            };

            var adddedUser2= await userRepository.Add(hr);
            var addedUser = await userRepository.Add(user);

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

            ApprovalStageDTO stageDTO = new ApprovalStageDTO()
            {
                ReviewId=1,
                RequestId=1,
                Comments="All fine"
            };

            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 1,
               
                Comments = "All fine",
              
            };
            mapper.Setup(m=>m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);

            var result = await service.ApproveRequestAsync(stageDTO);
            Assert.NotNull(result);
        }

        [Test]
        public async Task TestApprovebyManagerRequestAsync()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };

            User manager = new User
            {
                UserName = "Manager",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "admin@gmail.com",
            };

            var adddedUser2 = await userRepository.Add(manager);
            var addedUser = await userRepository.Add(user);

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

            ApprovalStageDTO stageDTO = new ApprovalStageDTO()
            {
                ReviewId = 1,
                RequestId = 1,
                Comments = "All fine"
            };

            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 1,

                Comments = "All fine",

            };
            mapper.Setup(m => m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);
            Employee employee = new Employee()
            {
                EmployeeId = 1,
                ManagerId = 2,

            };
            var addedEmployee = await employeeRepository.Add(employee);
            Assert.IsTrue(addedEmployee.ManagerId == employee.ManagerId);

            var result = await service.ApproveRequestAsync(stageDTO);
            Assert.NotNull(result);
        }

        [Test]
        public async Task TestApproveFinanceTestAsync()
        {
            await TestApproveRequestAsync();
            User finance = new User
            {
                UserName = "Finance",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Finance,
                Email = "admin@gmail.com",
            };

            var adddedUser2 = await userRepository.Add(finance);
            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 3,

                Comments = "All fine",

            };

            ApprovalStageDTO stageDTO = new ApprovalStageDTO()
            {
                RequestId = 1,
                ReviewId = 3,
                Comments = "Passed"
            };

            mapper.Setup(m => m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);

            var result = await service.ApproveRequestAsync(stageDTO);
            Assert.NotNull(result);
        }



        [Test]
        public async Task TestApproveFinanceTestAsyncException()
        {
            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 3,

                Comments = "All fine",

            };
            ApprovalStageDTO stageDTO = new ApprovalStageDTO()
            {
                RequestId = 1,
                ReviewId = 3,
                Comments = "Passed"
            };

            mapper.Setup(m => m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);

            Assert.ThrowsAsync<Exception>(async()=> await service.ApproveRequestAsync(stageDTO));
            
        }

        [Test]
        public async Task GetApprovalByIdAsyncTest()
        {
            await TestApproveRequestAsync();
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
            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };


            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                 .Returns(itemDTO);

            List<ResponseReimbursementItemDTO> itemDTOs = new List<ResponseReimbursementItemDTO>() { itemDTO };
            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            mapper.Setup(m => m.Map<List<ResponseReimbursementItemDTO>>(It.IsAny<List<ReimbursementItem>>()))
               .Returns(itemDTOs);

            await AddCategory();
            await AddItem();

            PaginatedResultDTO<ResponseReimbursementItemDTO> results = await itemService.GetItemsByRequestIdAsync(1, 1, 10);
            Assert.NotNull(results.Data);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };

            User hr = new User
            {
                UserName = "HR",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "admin@gmail.com",
            };

            //_mapper.Map<UserDTO>(reviewUser);
            UserDTO reviewUserDTO = new UserDTO()
            {
                Department = Departments.HR,
                UserName = "HR",
                Email = "admin@gmail.com"
            };

            //_mapper.Map<UserDTO>(requestUser);
            UserDTO requestUserDTO = new UserDTO()
            {
                Department = Departments.IT,
                UserName = "Rohit",
                Email = "laishramrohit15@gmail.com"
            };

            //mapper.Setup(m => m.Map<UserDTO>(user)).Returns(requestUserDTO);
            //mapper.Setup(m => m.Map<UserDTO>(hr)).Returns(reviewUserDTO);
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(reviewUserDTO);
            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            await policyRepository.Add(policy);
            var result = await service.GetApprovalByIdAsync(1);
            Assert.NotNull(result.Data);

        }

        public async Task AddCategory()
        {
            ExpenseCategory category = new ExpenseCategory()
            {
                Name = "Meals",
                Description = "this is for meals"
            };
            var addedCategory = await categoryRepositories.Add(category);
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
            var itemAdded = await itemService.AddItemAsync(item);
            return itemAdded;
        }

        [Test]
        public async Task GetApprovalByIdAsyncTestException()
        {
            Assert.ThrowsAsync<Exception>(async()=> await service.GetApprovalByIdAsync(1));
        }
        [Test]
        public async Task TestRejectRequestAsync()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };

            User hr = new User
            {
                UserName = "HR",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "admin@gmail.com",
            };

            var adddedUser2 = await userRepository.Add(hr);
            var addedUser = await userRepository.Add(user);

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

            ApprovalStageDTO stageDTO = new ApprovalStageDTO()
            {
                ReviewId = 1,
                RequestId = 1,
                Comments = "All fine"
            };

            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 1,

                Comments = "All fine",

            };
            mapper.Setup(m => m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);

            var result = await service.RejectRequestAsync(stageDTO);
            Assert.NotNull(result);
        }

        [Test]
        public async Task TestRejectManagerRequestAsync()
        {
            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };

            User manager = new User
            {
                UserName = "Manager",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "admin@gmail.com",
            };

            var adddedUser2 = await userRepository.Add(manager);
            var addedUser = await userRepository.Add(user);

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

            ApprovalStageDTO stageDTO = new ApprovalStageDTO()
            {
                ReviewId = 1,
                RequestId = 1,
                Comments = "All fine"
            };

            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 1,

                Comments = "All fine",

            };
            mapper.Setup(m => m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);

            var result = await service.RejectRequestAsync(stageDTO);
            Assert.NotNull(result);
        }
        [Test]
        public async Task TestRejectAsyncException()
        {
            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 3,

                Comments = "All fine",

            };
            ApprovalStageDTO stageDTO = new ApprovalStageDTO()
            {
                RequestId = 1,
                ReviewId = 3,
                Comments = "Passed"
            };

            mapper.Setup(m => m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);

            Assert.ThrowsAsync<Exception>(async () => await service.RejectRequestAsync(stageDTO));

        }


        [Test]
        public async Task TestRejectFinanceTestAsync()
        {
            await TestApproveRequestAsync();
            User finance = new User
            {
                UserName = "Finance",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Finance,
                Email = "admin@gmail.com",
            };

            var adddedUser2 = await userRepository.Add(finance);
            ApprovalStage approval = new ApprovalStage()
            {
                RequestId = 1,
                ReviewId = 3,

                Comments = "All fine",

            };

            ApprovalStageDTO stageDTO = new ApprovalStageDTO()
            {
                RequestId = 1,
                ReviewId = 3,
                Comments = "Passed"
            };

            mapper.Setup(m => m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);

            var result = await service.RejectRequestAsync(stageDTO);
            Assert.NotNull(result);
        }

        [Test]
        public async Task GetApprovalByRequestIdAsyncTest()
        {
            await TestApproveRequestAsync();
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
            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };

            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                 .Returns(itemDTO);
            await AddItem();

            PaginatedResultDTO<ResponseReimbursementItemDTO> results = await itemService.GetItemsByRequestIdAsync(1, 1, 10);
            Assert.NotNull(results.Data);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };

            User hr = new User
            {
                UserName = "HR",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "admin@gmail.com",
            };

            //_mapper.Map<UserDTO>(reviewUser);
            UserDTO reviewUserDTO = new UserDTO()
            {
                Department = Departments.HR,
                UserName = "HR",
                Email = "admin@gmail.com"
            };

            //_mapper.Map<UserDTO>(requestUser);
            UserDTO requestUserDTO = new UserDTO()
            {
                Department = Departments.IT,
                UserName = "Rohit",
                Email = "laishramrohit15@gmail.com"
            };

            //mapper.Setup(m => m.Map<UserDTO>(user)).Returns(requestUserDTO);
            //mapper.Setup(m => m.Map<UserDTO>(hr)).Returns(reviewUserDTO);
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(reviewUserDTO);
            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            await policyRepository.Add(policy);
            List<ResponseReimbursementItemDTO> itemDTOs = new List<ResponseReimbursementItemDTO>() { itemDTO };
            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            mapper.Setup(m => m.Map<List<ResponseReimbursementItemDTO>>(It.IsAny<List<ReimbursementItem>>()))
               .Returns(itemDTOs);
            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);


            var result = await service.GetApprovalsByRequestIdAsync(1,1,100);
            Assert.NotNull(result.Data);

        }

        [Test]
        public async Task TestExceptionGetApprovalsByRequestId()
        {
            Assert.ThrowsAsync<Exception>(async () => await service.GetApprovalsByRequestIdAsync(1, 1, 100));
        }

        [Test]
        public async Task TestGetApprovalsByReviewerIdAsync()
        {
            await GetApprovalByRequestIdAsyncTest();
            var result = await service.GetApprovalsByReviewerIdAsync(1, 1, 100);
            Assert.NotNull(result.Data);
        }

        [Test]
        public async Task TEstGetApprovalsByReviewerIdAsyncException()
        {
            Assert.ThrowsAsync<Exception>(async () => await service.GetApprovalsByReviewerIdAsync(1, 1, 100));
        }

        [Test]
        public async Task TestGetFinancePendingApprovalsAsync()
        {
            await GetApprovalByRequestIdAsyncTest();
            var result = await service.GetFinancePendingApprovalsAsync( 1, 100);
            Assert.NotNull(result.Data);
        }

        [Test]
        public async Task TestExceptionGetFinancePendingApprovalsAsync()
        {
            Assert.ThrowsAsync<Exception>(async () => await service.GetFinancePendingApprovalsAsync( 1, 100));
        }
        [Test]
        public async Task TestGetHrPendingApprovalsAsync()
        {
            await TestApprovebyManagerRequestAsync();
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

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"
            };

            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                 .Returns(itemDTO);
            await AddCategory();
            await AddItem();

            PaginatedResultDTO<ResponseReimbursementItemDTO> results = await itemService.GetItemsByRequestIdAsync(1, 1, 10);
            Assert.NotNull(results.Data);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.IT,
                Email = "laishramrohit15@gmail.com",
            };

            User hr = new User
            {
                UserName = "HR",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "admin@gmail.com",
            };

            //_mapper.Map<UserDTO>(reviewUser);
            UserDTO reviewUserDTO = new UserDTO()
            {
                Department = Departments.HR,
                UserName = "HR",
                Email = "admin@gmail.com"
            };

            //_mapper.Map<UserDTO>(requestUser);
            UserDTO requestUserDTO = new UserDTO()
            {
                Department = Departments.IT,
                UserName = "Rohit",
                Email = "laishramrohit15@gmail.com"
            };

            //mapper.Setup(m => m.Map<UserDTO>(user)).Returns(requestUserDTO);
            //mapper.Setup(m => m.Map<UserDTO>(hr)).Returns(reviewUserDTO);
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(reviewUserDTO);
            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            await policyRepository.Add(policy);
            var result = await service.GetHrPendingApprovalsAsync(1, 100);
            Assert.NotNull(result.Data);
        }
        [Test]
        public async Task TestExceptionGetHrPendingApprovalsAsync()
        {
            Assert.ThrowsAsync<Exception>(async () => await service.GetHrPendingApprovalsAsync( 1, 100));
        }




    }
}
