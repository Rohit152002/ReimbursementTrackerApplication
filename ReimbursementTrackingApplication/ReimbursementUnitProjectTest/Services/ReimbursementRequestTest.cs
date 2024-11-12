using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Common;
using Moq;
using NUnit.Framework.Interfaces;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
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
    class ReimbursementRequestTest
    {
        DbContextOptions options;
        ContextApp context;

        ReimbursementRequestRepository requestRepository;
        ReimbursementItemRepositories itemRepository;
        PolicyRepository policyRepository;
        CategoryRepositories categoryRepository;
        UserRepository userRepository;
      EmployeeRepository employeeRepository;
        
        Mock<ILogger<ReimbursementRequestRepository>> loggerRequest;
        Mock<ILogger<EmployeeRepository>> employeeLogger;
        Mock<ILogger<CategoryRepositories>> loggerCategory;
        Mock<ILogger<PolicyRepository>> loggerPolicy;
        Mock<ILogger<UserRepository>> loggerUser;
        Mock<ILogger<UserService>> loggerServiceUser;
        Mock<ILogger<ReimbursementItemRepositories>> loggerItemRepository;

        Mock<IMapper> mapper;
        Mock<IConfiguration> mockConfiguration;
        Mock<TokenService> mockTokenService;
        
        UserService userService;
        EmployeeService employeeService;
        ReimbursementItemService itemService;
        ReimbursementRequestService requestService;

        string uploadFolder;
        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                .UseInMemoryDatabase("database" + Guid.NewGuid())
                 .Options;

            context = new ContextApp(options);

            mapper = new Mock<IMapper>();
            loggerRequest = new Mock<ILogger<ReimbursementRequestRepository>>();
            loggerPolicy = new Mock<ILogger<PolicyRepository>>();
            loggerUser = new Mock<ILogger<UserRepository>>();
            loggerServiceUser = new Mock<ILogger<UserService>>();
            loggerCategory = new Mock<ILogger<CategoryRepositories>>();
            loggerItemRepository = new Mock<ILogger<ReimbursementItemRepositories>>();
            employeeLogger = new Mock<ILogger<EmployeeRepository>>();

            mockConfiguration = new Mock<IConfiguration>();
            mockTokenService = new Mock<TokenService>(mockConfiguration.Object);
            mockTokenService.Setup(t => t.GenerateToken(It.IsAny<UserTokenDTO>())).ReturnsAsync("TestToken");

            uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "File");

            categoryRepository = new CategoryRepositories(context, loggerCategory.Object);
            userRepository = new UserRepository(context,loggerUser.Object);
            requestRepository = new ReimbursementRequestRepository(context, loggerRequest.Object);
            policyRepository= new PolicyRepository(context, loggerPolicy.Object);
            itemRepository = new ReimbursementItemRepositories(context, loggerItemRepository.Object);
            employeeRepository = new EmployeeRepository(context, employeeLogger.Object);


            userService = new UserService(userRepository, mapper.Object, loggerServiceUser.Object, mockTokenService.Object);
            itemService = new ReimbursementItemService(itemRepository, categoryRepository, uploadFolder, mapper.Object);
            employeeService = new EmployeeService(employeeRepository, mapper.Object,userRepository);
            requestService = new ReimbursementRequestService(requestRepository, policyRepository, mapper.Object,uploadFolder,itemRepository,employeeRepository,categoryRepository,userRepository);
            
        }

        private async Task CreateUser()
        {
            var userCreateDTO = new UserCreateDTO
            {
                UserName = "Rohit Laishram",
                Password = "TestPassword",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            var addedUser = await userService.Register(userCreateDTO);

        }

        //public async Task<SuccessResponseDTO<ResponseReimbursementRequestDTO>> SubmitRequestAsync(CreateReimbursementRequestDTO requestDto)

        private async Task CreatePolicy()
        {
            Policy policy = new Policy()
            {
                PolicyName = "Travel Policy",
                PolicyDescription = "it all includes meals and policy ",
                MaxAmount = 10000
            };
            await policyRepository.Add(policy);
        }

        private async Task<CreateReimbursementRequestDTO> CreateRequest()
        {
            List<RequestITemDTO> items = new List<RequestITemDTO>();
            items.Add(await AddItem());
            return new CreateReimbursementRequestDTO()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 9000,
                Comments = "Request for Businees ",
                Items=items
            };

        }
        public async Task<RequestITemDTO> AddItem()
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

            RequestITemDTO item = new RequestITemDTO
            {
              
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = formFile.Object
            };

            //IReimbursementItemService service = new ReimbursementItemService(reimbursementItemRepositories, categoryRepository, uploadFolder, mapper.Object);
            return item;
        }
        [Test]
        public async Task SubmitRequestTest()
        {
           await CreateUser();
            await CreatePolicy();
            CreateReimbursementRequestDTO request = await CreateRequest();

            ReimbursementItem item = new ReimbursementItem
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "string"
            };

            // Create an IEnumerable<ReimbursementItem> with a single element
            IList<ReimbursementItem> items = new List<ReimbursementItem> { item };

            ReimbursementRequest requestTest =new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 9000,
                Comments = "Request for Businees ",

            };
            UserDTO userDTO = new UserDTO
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

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
                Items= responseItems
            };

            //1. _mapper.Map<ReimbursementRequest>(requestDto);
            mapper.Setup(m => m.Map<ReimbursementRequest>(It.IsAny<ResponseReimbursementRequestDTO>())).Returns(requestTest);

            //2. _mapper.Map<ResponseReimbursementItemDTO>(item);
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                  .Returns(requestDTO);

            ////3.  _mapper.Map<List<ReimbursementItem>>(items);
            mapper.Setup(m => m.Map<List<ReimbursementItem>>(It.IsAny<IList<ResponseReimbursementItemDTO>>()))
                .Returns(new List<ReimbursementItem> { item });

            ////4. _mapper.Map<ResponseReimbursementRequestDTO>(requestAdded);
            mapper.Setup(m => m.Map<ResponseReimbursementRequestDTO>(It.IsAny<ReimbursementRequest>())).Returns(reimRequestDTO);


            //action 
            await AddCategory();

            var result = await requestService.SubmitRequestAsync(request);
            Assert.IsNotNull(result.Data);



        }
        [Test]
        public async Task SubmitRequestTestException()
        {
            await CreateUser();
            await CreatePolicy();
            CreateReimbursementRequestDTO request = await CreateRequest();
            request.Comments = null;
            ReimbursementItem item = new ReimbursementItem
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "string"
            };

            // Create an IEnumerable<ReimbursementItem> with a single element
            IList<ReimbursementItem> items = new List<ReimbursementItem> { item };

            ReimbursementRequest requestTest = new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 9000,
                Comments = "Request for Businees ",

            };
            UserDTO userDTO = new UserDTO
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                Id=1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            List<ResponseReimbursementItemDTO> responseItems = new List<ResponseReimbursementItemDTO> { requestDTO };

            ResponseReimbursementRequestDTO reimRequestDTO = new ResponseReimbursementRequestDTO
            {
                Id = 1,
                UserId = 1,
                User = userDTO,
                PolicyId = 1,
                PolicyName = "Travel Policy",
                TotalAmount = 9000,
                Comments = "Request for Businees ",
                Items = responseItems
            };

            //1. _mapper.Map<ReimbursementRequest>(requestDto);
            mapper.Setup(m => m.Map<ReimbursementRequest>(It.IsAny<ResponseReimbursementRequestDTO>())).Returns(requestTest);

            //2. _mapper.Map<ResponseReimbursementItemDTO>(item);
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                  .Returns(requestDTO);

            ////3.  _mapper.Map<List<ReimbursementItem>>(items);
            mapper.Setup(m => m.Map<List<ReimbursementItem>>(It.IsAny<IList<ResponseReimbursementItemDTO>>()))
                .Returns(new List<ReimbursementItem> { item });

            ////4. _mapper.Map<ResponseReimbursementRequestDTO>(requestAdded);
            mapper.Setup(m => m.Map<ResponseReimbursementRequestDTO>(It.IsAny<ReimbursementRequest>())).Returns(reimRequestDTO);


            //action 

            Assert.ThrowsAsync<Exception>(async () => await requestService.SubmitRequestAsync(request));
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

        [Test]
        public async Task GetRequestByRequestId()
        {
            await SubmitRequestTest();
            await AddCategory();

            UserDTO userDTO = new UserDTO
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {Id = 1,

                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            List<ResponseReimbursementItemDTO> responseItems = new List<ResponseReimbursementItemDTO> { requestDTO };
            ResponseReimbursementRequestDTO reimRequestDTO = new ResponseReimbursementRequestDTO
            {
                Id=1,
                UserId = 1,
                User = userDTO,
                PolicyId = 1,
                PolicyName = "Travel Policy",
                TotalAmount = 9000,
                Comments = "Request for Businees ",
                Items = responseItems
            };

            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            mapper.Setup(m => m.Map<List<ResponseReimbursementItemDTO>>(It.IsAny<List<ReimbursementItem>>())).Returns(responseItems);
            mapper.Setup(m => m.Map<ResponseReimbursementRequestDTO>(It.IsAny<ReimbursementRequest>())).Returns(reimRequestDTO);
            var request = await requestService.GetRequestByIdAsync(1);
            Assert.NotNull(request.Data);
        }

        [Test]
        public async Task GetRequestByRequestIdTestException()
        {
           

            UserDTO userDTO = new UserDTO
            {
                UserName = "Rohit Laishram",
                Email = "laishramrohit15@gmail.com",
                Department = Departments.IT
            };
            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

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
            mapper.Setup(m => m.Map<ResponseReimbursementRequestDTO>(It.IsAny<ReimbursementRequest>())).Returns(reimRequestDTO);
           Assert.ThrowsAsync<NotFoundException>(async()=> await requestService.GetRequestByIdAsync(1));
            
        }

        [Test]
        public async Task GetRequestsByUserIdAsyncTest()
        {
            await SubmitRequestTest();
            await AddCategory();

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };


            var requestDTOs = new List<ResponseReimbursementRequestDTO>
        {
            new ResponseReimbursementRequestDTO { Id=1, UserId = 1, PolicyId = 1, TotalAmount = 100, Comments = "Test request", Items = new List<ResponseReimbursementItemDTO>() {itemDTO } },
            new ResponseReimbursementRequestDTO { Id=2, UserId = 1, PolicyId = 2, TotalAmount = 200, Comments = "Test request 2", Items = new List<ResponseReimbursementItemDTO>() { itemDTO } }
        };

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
            mapper.Setup(m => m.Map<List<ResponseReimbursementRequestDTO>>(It.IsAny<List<ReimbursementRequest>>))
            .Returns(requestDTOs);

            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                .Returns(requestDTO);

            //_mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
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
            var request = await requestService.GetRequestsByUserIdAsync(1,1,100);
            Assert.NotNull(request.Data);
        }
     

        [Test]
        public async Task GetRequestsByUserIdAsyncTestException()
        {
            //await SubmitRequestTest();
            await AddCategory();

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };


            var requestDTOs = new List<ResponseReimbursementRequestDTO>
        {
            new ResponseReimbursementRequestDTO { Id=1, UserId = 1, PolicyId = 1, TotalAmount = 100, Comments = "Test request", Items = new List<ResponseReimbursementItemDTO>() {itemDTO } },
            new ResponseReimbursementRequestDTO { Id=2, UserId = 1, PolicyId = 2, TotalAmount = 200, Comments = "Test request 2", Items = new List<ResponseReimbursementItemDTO>() { itemDTO } }
        };

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
            mapper.Setup(m => m.Map<List<ResponseReimbursementRequestDTO>>(It.IsAny<List<ReimbursementRequest>>))
            .Returns(requestDTOs);

            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                .Returns(requestDTO);

            //_mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {Id = 1,
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);
            Assert.ThrowsAsync<Exception>(async()=> await requestService.GetRequestsByUserIdAsync(1, 1, 100));
            
        }


        [Test]
        public async Task GetRequestsByStatusAsyncTest()
        {
            await SubmitRequestTest();
            await AddCategory();

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };


            var requestDTOs = new List<ResponseReimbursementRequestDTO>
        {
            new ResponseReimbursementRequestDTO { Id=1, UserId = 1, PolicyId = 1, TotalAmount = 100, Comments = "Test request", Items = new List<ResponseReimbursementItemDTO>() {itemDTO } },
            new ResponseReimbursementRequestDTO { Id=2, UserId = 1, PolicyId = 2, TotalAmount = 200, Comments = "Test request 2", Items = new List<ResponseReimbursementItemDTO>() { itemDTO } }
        };

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
            mapper.Setup(m => m.Map<List<ResponseReimbursementRequestDTO>>(It.IsAny<List<ReimbursementRequest>>))
            .Returns(requestDTOs);

            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                .Returns(requestDTO);

            //_mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {Id = 1,
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);
            var request = await requestService.GetRequestsByStatusAsync(RequestStatus.Pending, 1, 100);
            Assert.NotNull(request.Data);
        }

        [Test]
        public async Task GetRequestsByStatusAsyncTestException()
        {
            //await SubmitRequestTest();
            await AddCategory();

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {Id = 1,
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };


            var requestDTOs = new List<ResponseReimbursementRequestDTO>
        {
            new ResponseReimbursementRequestDTO { Id=1, UserId = 1, PolicyId = 1, TotalAmount = 100, Comments = "Test request", Items = new List<ResponseReimbursementItemDTO>() {itemDTO } },
            new ResponseReimbursementRequestDTO { Id=2, UserId = 1, PolicyId = 2, TotalAmount = 200, Comments = "Test request 2", Items = new List<ResponseReimbursementItemDTO>() { itemDTO } }
        };

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
            mapper.Setup(m => m.Map<List<ResponseReimbursementRequestDTO>>(It.IsAny<List<ReimbursementRequest>>))
            .Returns(requestDTOs);

            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                .Returns(requestDTO);

            //_mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);
            
            Assert.ThrowsAsync<Exception>(async () => await requestService.GetRequestsByStatusAsync(RequestStatus.Pending, 1, 100));
        
        }

        [Test]
        public async Task GetRequestsByPolicyAsyncTest()
        {
            await SubmitRequestTest();
            await AddCategory();

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };


            var requestDTOs = new List<ResponseReimbursementRequestDTO>
        {
            new ResponseReimbursementRequestDTO { Id=1, UserId = 1, PolicyId = 1, TotalAmount = 100, Comments = "Test request", Items = new List<ResponseReimbursementItemDTO>() {itemDTO } },
            new ResponseReimbursementRequestDTO { Id=2, UserId = 1, PolicyId = 2, TotalAmount = 200, Comments = "Test request 2", Items = new List<ResponseReimbursementItemDTO>() { itemDTO } }
        };

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
            mapper.Setup(m => m.Map<List<ResponseReimbursementRequestDTO>>(It.IsAny<List<ReimbursementRequest>>))
            .Returns(requestDTOs);

            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                .Returns(requestDTO);

            //_mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);
            var request = await requestService.GetRequestsByPolicyAsync(1, 1, 100);
            Assert.NotNull(request.Data);
        }

        [Test]
        public async Task GetRequestsByPolicyAsyncTestException()
        {
            //await SubmitRequestTest();
            await AddCategory();

            ResponseReimbursementItemDTO itemDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };


            var requestDTOs = new List<ResponseReimbursementRequestDTO>
        {
            new ResponseReimbursementRequestDTO { Id=1, UserId = 1, PolicyId = 1, TotalAmount = 100, Comments = "Test request", Items = new List<ResponseReimbursementItemDTO>() {itemDTO } },
            new ResponseReimbursementRequestDTO { Id=2, UserId = 1, PolicyId = 2, TotalAmount = 200, Comments = "Test request 2", Items = new List<ResponseReimbursementItemDTO>() { itemDTO } }
        };

            ResponseReimbursementItemDTO requestDTO = new ResponseReimbursementItemDTO
            {
                RequestId = 1,
                Amount = 9000,
                CategoryId = 1,
                Description = "Travelling",
                receiptFile = "image.jpg"

            };
            //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
            mapper.Setup(m => m.Map<List<ResponseReimbursementRequestDTO>>(It.IsAny<List<ReimbursementRequest>>))
            .Returns(requestDTOs);

            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            mapper.Setup(m => m.Map<ResponseReimbursementItemDTO>(It.IsAny<ReimbursementItem>()))
                .Returns(requestDTO);

            //_mapper.Map<List<ResponseReimbursementItemDTO>>(pagedItems);
            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);

            Assert.ThrowsAsync<Exception>(async () => await requestService.GetRequestsByPolicyAsync(1, 1, 100));

        }
        [Test]
        public async Task TestDeleteRequestAsync()
        {
            await SubmitRequestTest();
            await AddCategory();
            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {
                                 Id=2,
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<List<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);
            //var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            //var responseRequest = _mapper.Map<ResponseReimbursementRequestDTO>(requestAdded);
            UserDTO user = new UserDTO { 
                UserName="Rohit",
                Department=Departments.IT,
                Email="laishramrohit15@gmail.com"

            };

            //var userDTO = _mapper.Map<UserDTO>(user);
            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>()))
                  .Returns(user);


            var result = await requestService.DeleteRequestAsync(1);
            Assert.IsNotNull(result);

        }

        [Test]
        public async Task TestDeleteRequestAsyncException()
        {
            //await SubmitRequestTest();
            await AddCategory();
            var itemDTOList = new List<ResponseReimbursementItemDTO>
                        {
                             new ResponseReimbursementItemDTO
                            {
                                 Id=2,
                                RequestId = 1,
                                Amount = 9000,
                                CategoryId = 1,
                                Description = "Travelling",
                                receiptFile = "image.jpg"
                            }

                        };
            mapper.Setup(m => m.Map<IList<ResponseReimbursementItemDTO>>(It.IsAny<IList<ReimbursementItem>>()))
                   .Returns(itemDTOList);


            Assert.ThrowsAsync<Exception>(async()=> await requestService.DeleteRequestAsync(1));


        }

        [Test]
        public async Task GetRequestsByManagerIdAsyncTest()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 1,
                ManagerId = 3
            };

            Employee employee = new Employee()
            {
                EmployeeId = 1,
                ManagerId = 3,
            };
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit",
                Department = Departments.Marketing,
                Email = "laishramrohit15@gmail.com"
            };

            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            var result = await employeeService.GetEmployeesByManagerIdAsync(1, 1, 100);
            Assert.IsNotNull(result.Data);

            await GetRequestsByUserIdAsyncTest();

            var requestresult = await requestService.GetRequestsByManagerIdAsync(3, 1, 100);
            Assert.IsNotNull(requestresult.Data);
        }
        [Test]
        public async Task GetAllRequestAsyncTest()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 1,
                ManagerId = 3
            };

            Employee employee = new Employee()
            {
                EmployeeId = 1,
                ManagerId = 3,
            };
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit",
                Department = Departments.Marketing,
                Email = "laishramrohit15@gmail.com"
            };

            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            //var result = await employeeService.GetEmployeesByManagerIdAsync(1, 1, 100);
            //Assert.IsNotNull(result.Data);

            await GetRequestsByUserIdAsyncTest();

            var requestresult = await requestService.GetAllRequest( 1, 100);
            Assert.IsNotNull(requestresult.Data);
        }

        [Test]
        public async Task GetAllRequestAsyncTestException()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 1,
                ManagerId = 3
            };

            Employee employee = new Employee()
            {
                EmployeeId = 1,
                ManagerId = 3,
            };
            UserDTO userDTO = new UserDTO()
            {
                UserName = "Rohit",
                Department = Departments.Marketing,
                Email = "laishramrohit15@gmail.com"
            };

            mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            //var result = await employeeService.GetEmployeesByManagerIdAsync(1, 1, 100);
            //Assert.IsNotNull(result.Data);

            //await GetRequestsByUserIdAsyncTest();

            Assert.ThrowsAsync<Exception>(async()=> await requestService.GetAllRequest(1, 100));
       
        }

        [Test]
        public async Task GetRequestsByManagerIdAsyncTestException()
        {
            EmployeeDTO employeeDTO = new EmployeeDTO()
            {
                EmployeeId = 2,
                ManagerId = 1
            };

            Employee employee = new Employee()
            {
                EmployeeId = 2,
                ManagerId = 1,
            };

            

            mapper.Setup(m => m.Map<Employee>(It.IsAny<EmployeeDTO>())).Returns(employee);

            var add = await employeeService.AddEmployeeAsync(employeeDTO);

            User user = new User
            {
                UserName = "Rohit",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.HR,
                Email = "laishramrohit15@gmail.com",
            };
            User user2 = new User
            {
                UserName = "Rahul",
                Password = Encoding.UTF8.GetBytes("TestPassword"),
                HashKey = Encoding.UTF8.GetBytes("TestHashKey"),
                Department = Departments.Marketing,
                Email = "rahul@gmail.com",
            };
            var addedUser = await userRepository.Add(user);
            var addedUser2 = await userRepository.Add(user2);
            Assert.IsTrue(addedUser.UserName == user.UserName);

            var result = await employeeService.GetEmployeesByManagerIdAsync(1, 1, 100);
            Assert.IsNotNull(result.Data);

            //await GetRequestsByUserIdAsyncTest();

            Assert.ThrowsAsync<NotFoundException>(async()=> await requestService.GetRequestsByManagerIdAsync(1,1, 100));
            
        }
    }
}





