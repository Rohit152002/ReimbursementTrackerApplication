using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Repositories;
using ReimbursementTrackingApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementUnitProjectTest.Services
{
    internal class PolicyServiceTest
    {
        DbContextOptions options;
        ContextApp context;

        Mock<ILogger<PolicyRepository>> policyLogger;
        Mock<IMapper> mapper;

        PolicyRepository repository;
        PolicyService service;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
               .UseInMemoryDatabase("database" + Guid.NewGuid())
                .Options;
            context = new ContextApp(options);

            policyLogger = new Mock<ILogger<PolicyRepository>>();

            mapper = new Mock<IMapper>();

            repository = new PolicyRepository(context, policyLogger.Object);
            service = new PolicyService(repository, mapper.Object);

        }

        [Test]
        public async Task TestAddPolicyCategory()
        {
            CreatePolicyDTO policyDTO = new CreatePolicyDTO()
            {
                PolicyName = "Test",
                PolicyDescription = "Test Description",
                MaxAmount=1000,
                
            };

            Policy policy = new Policy()
            {
                PolicyName = "Test",
                PolicyDescription = "Test Description",
                MaxAmount = 1000,
            };
            mapper.Setup(m => m.Map<Policy>(It.IsAny<CreatePolicyDTO>())).Returns(policy);
            var result = await service.AddPolicyAsync(policyDTO);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public async Task TestPolicyException()
        {
            CreatePolicyDTO policyDTO = new CreatePolicyDTO()
            {
                PolicyName = "Test",
                PolicyDescription = null,
                MaxAmount = 1000,

            };

            Policy policy = new Policy()
            {
                PolicyName = "Test",
                PolicyDescription =null,
                MaxAmount = 1000,
            };
            mapper.Setup(m => m.Map<Policy>(It.IsAny<CreatePolicyDTO>())).Returns(policy);
            Assert.ThrowsAsync<Exception>(async () => await service.AddPolicyAsync(policyDTO));
            //Assert.IsNotNull(result.Data);
        }
        [Test]
        public async Task TestDeletePolicy()
        {
            await TestAddPolicyCategory();

            var result = await service.DeletePolicyAsync(1);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task TestExceptionDeletePolicy()
        {
            Assert.ThrowsAsync<Exception>(async () => await service.DeletePolicyAsync(1));
        }
        [Test]
        public async Task TestUpdatePolicy()
        {
            await TestAddPolicyCategory();
            CreatePolicyDTO policyDTO = new CreatePolicyDTO()
            {
                PolicyName = "Test",
                PolicyDescription = "Test Description",
                MaxAmount = 1000,

            };

            var result = await service.UpdatePolicyAsync(1, policyDTO);
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task TestExceptionUpdatePolicy()
        {
            //await TestAddExpenseCategory();
            CreatePolicyDTO policyDTO = new CreatePolicyDTO()
            {
                PolicyName = "Test",
                PolicyDescription = "Test Description",
                MaxAmount = 1000,

            };
            Assert.ThrowsAsync<Exception>(async () => await service.UpdatePolicyAsync(1, policyDTO));
        }

        [Test]
        public async Task TestGetAllPolicies()
        {
            await TestAddPolicyCategory();

            List<ResponsePolicyDTO> categoryDTOs = new List<ResponsePolicyDTO>()
            {
                new ResponsePolicyDTO()
                {
                    Id = 1,
                    PolicyName = "Test",
                PolicyDescription = "Test Description",
                MaxAmount = 1000,
                }
            };

            mapper.Setup(m => m.Map<List<ResponsePolicyDTO>>(It.IsAny<List<Policy>>())).Returns(categoryDTOs);

            var result = await service.GetAllPolicesAsync(1, 100);
            Assert.IsNotNull(result);

        }


        [Test]
        public async Task TestExceptionGetAllPolicies()
        {
            //await TestAddExpenseCategory();

            List<ResponsePolicyDTO> categoryDTOs = new List<ResponsePolicyDTO>()
            {
                new ResponsePolicyDTO()
                {
                    Id = 1,
                    PolicyName = "Test",
                PolicyDescription = "Test Description",
                MaxAmount = 1000,
                }
            };


            mapper.Setup(m => m.Map<List<ResponsePolicyDTO>>(It.IsAny<List<Policy>>())).Returns(categoryDTOs);

            Assert.ThrowsAsync<Exception>(async () => await service.GetAllPolicesAsync(1, 100));
            //Assert.IsNotNull(result);

        }

        [Test]
        public async Task TestGePolictByID()
        {
            await TestAddPolicyCategory();
            ResponsePolicyDTO policyDTO = new ResponsePolicyDTO()
            {
                Id = 1,
                PolicyName = "Test",
                PolicyDescription = "Test Description",
                MaxAmount = 1000,
            };

            mapper.Setup(m => m.Map<ResponsePolicyDTO>(It.IsAny<Policy>())).Returns(policyDTO);

            var result = await service.GetPolicyByIdAsync(1);
            Assert.IsNotNull(result.Data);

        }

        [Test]
        public async Task TestExceptionCategoryById()
        {
            ResponsePolicyDTO policyDTO = new ResponsePolicyDTO()
            {
                Id = 1,
                PolicyName = "Test",
                PolicyDescription = "Test Description",
                MaxAmount = 1000,
            };

            mapper.Setup(m => m.Map<ResponsePolicyDTO>(It.IsAny<Policy>())).Returns(policyDTO);

            Assert.ThrowsAsync<Exception>(async () => await service.GetPolicyByIdAsync(1));

        }

    }
}
