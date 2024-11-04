using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface ITokenService
    {
        public  Task<string> GenerateToken(UserTokenDTO user);
    }
}
