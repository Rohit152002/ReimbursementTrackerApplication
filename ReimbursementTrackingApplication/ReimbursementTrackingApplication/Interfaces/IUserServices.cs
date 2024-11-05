using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IUserServices
    {
        public Task<LoginResponseDTO> Register(UserCreateDTO user);
        public Task<LoginResponseDTO> Login(LoginDTO login);

        public Task<UserDTO> GetUserProfile(int id);

        public Task<PaginatedResultDTO<UserDTO>> GetAllUsers(int pageno , int pageSize );
        public Task<PaginatedResultDTO<UserDTO>> SearchUser(string searchTerm, int pageno, int pageSize);
        public Task<bool> ChangePassword(ChangePasswordDTO changePassword);

    }
}
