using AutoMapper;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Mapper
{
    public class PolicyProfile:Profile
    {
        public PolicyProfile()
        {
            CreateMap<CreatePolicyDTO, Policy>();
            CreateMap<Policy, CreatePolicyDTO>();
            CreateMap<Policy, ResponsePolicyDTO>();
            CreateMap<ResponsePolicyDTO, Policy>();
            
        }
    }
}
