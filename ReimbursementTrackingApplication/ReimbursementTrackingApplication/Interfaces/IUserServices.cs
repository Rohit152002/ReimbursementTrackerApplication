using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IUserServices
    {
        public Task<LoginResponseDTO> Register(UserCreateDTO user);
        public Task<LoginResponseDTO> Login(LoginDTO login);

        public Task<UserDTO> GetUserProfile(int id);

        public Task<IEnumerable<UserDTO>> GetAllUsers();
        public Task<IEnumerable<UserDTO>> SearchUser(string searchTerm);
        Task<bool> ChangePassword(ChangePasswordDTO changePassword);

    }
}
