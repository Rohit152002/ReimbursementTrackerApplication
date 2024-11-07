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
    internal class ApprovalServiceTest
    {

        DbContextOptions options;
        ContextApp context;

        Mock<ILogger<ApprovalStageRepository>> approvalLogger;
        Mock<ILogger<UserRepository>> userLogger;
        Mock<ILogger<ReimbursementRequestRepository>> requestLogger;

        ApprovalStageRepository _repository;
       UserRepository _userRepository;
        ReimbursementRequestRepository _requestRepository;
        Mock<IMapper> _mapper;

        ApprovalService service;

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

            _repository = new ApprovalStageRepository(context, approvalLogger.Object);
            _userRepository= new UserRepository(context, userLogger.Object);
            _requestRepository= new ReimbursementRequestRepository(context, requestLogger.Object); 
            _mapper = new Mock<IMapper>();
            service = new ApprovalService(_repository,_userRepository,_requestRepository,_mapper.Object);

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

            var adddedUser2= await _userRepository.Add(hr);
            var addedUser = await _userRepository.Add(user);

            ReimbursementRequest request = new ReimbursementRequest()
            {
                UserId = 1,
                PolicyId = 1,
                TotalAmount = 150.75,
                Comments = "Office supplies purchased for the project.",
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };

            var requestAdded = await _requestRepository.Add(request);

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
            _mapper.Setup(m=>m.Map<ApprovalStage>(It.IsAny<ApprovalStageDTO>())).Returns(approval);

            var result = await service.ApproveRequestAsync(stageDTO);
            Assert.NotNull(result);
        }


    }
}
