﻿
namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ResponseApprovalStageDTO
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public ResponseReimbursementRequestDTO Request { get; set; }
        public int ReviewId { get; set; }
        public UserDTO Review { get; set; }

        public string Comments { get; set; } = string.Empty;
    }
}