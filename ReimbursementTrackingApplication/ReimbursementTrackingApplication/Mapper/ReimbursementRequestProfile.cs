using AutoMapper;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Mapper
{
    public class ReimbursementRequestProfile:Profile
    {
        public ReimbursementRequestProfile()
        {
            CreateMap<ReimbursementRequest, ResponseReimbursementRequestDTO>();
            CreateMap<ResponseReimbursementRequestDTO, ReimbursementRequest>();
        }
    }
}
