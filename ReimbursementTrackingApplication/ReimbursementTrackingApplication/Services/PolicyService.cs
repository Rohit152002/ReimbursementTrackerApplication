using AutoMapper;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IRepository<int, Policy> _repository;
        private readonly IMapper _mapper;
        public PolicyService(IRepository<int,Policy> repository,IMapper mapper) {
        _repository = repository;
            _mapper = mapper;
        }
        public async Task<SuccessResponseDTO<int>> AddPolicyAsync(CreatePolicyDTO policyDTO)
        {
            try
            {
                var policy = _mapper.Map<Policy>(policyDTO);
                var policyData = await _repository.Add(policy);
                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Add Policy Successfull",
                    Data = policyData.Id
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<int>> DeletePolicyAsync(int policyId)
        {
            try
            {

                var policyData = await _repository.Get(policyId);
                policyData.IsDeleted = true;

                var updateData = await _repository.Update(policyId, policyData);
                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Delete Policy Successfull",
                    Data = updateData.Id
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponsePolicyDTO>> GetAllPolicesAsync(int pageNumber, int pageSize)
        {
            try
            {

                var policies = (await _repository.GetAll()).Where(c => c.IsDeleted == false);
                var total = policies.Count();

                var pagedItems = policies
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();

                var categoryDTOs = _mapper.Map<List<ResponsePolicyDTO>>(pagedItems);
                return new PaginatedResultDTO<ResponsePolicyDTO>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = categoryDTOs

                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<ResponsePolicyDTO>> GetPolicyByIdAsync(int policyId)
        {
            try
            {

                var policyData = await _repository.Get(policyId);
                var responsePolicyDTO = _mapper.Map<ResponsePolicyDTO>(policyData);

                return new SuccessResponseDTO<ResponsePolicyDTO>
                {
                    IsSuccess = true,
                    Message = "Delete Category Successfull",
                    Data = responsePolicyDTO
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<int>> UpdatePolicyAsync(int policyId, CreatePolicyDTO policyDTO)
        {
            try
            {
                var policy = _mapper.Map<Policy>(policyDTO);

                var policyData = await _repository.Update(policyId, policy);
                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Update Policy Successfull",
                    Data = policyData.Id
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
