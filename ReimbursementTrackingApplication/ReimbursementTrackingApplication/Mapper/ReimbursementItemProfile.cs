using AutoMapper;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Mapper
{
    public class ReimbursementItemProfile:Profile
    {
        public ReimbursementItemProfile()
        {
            CreateMap<ResponseReimbursementItemDTO,ReimbursementItem>();
            CreateMap<ReimbursementItem, ResponseReimbursementItemDTO>(); 
        }
    }
}
