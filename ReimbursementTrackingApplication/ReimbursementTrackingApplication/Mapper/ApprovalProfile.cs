using AutoMapper;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Mapper
{
    public class ApprovalProfile:Profile
    {
        public ApprovalProfile()
        {
            CreateMap<ApprovalStage,ApprovalStageDTO>();
            CreateMap<ApprovalStageDTO,ApprovalStage>();
        }
    }
}
