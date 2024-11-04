using AutoMapper;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace ReimbursementTrackingApplication.Services
{
    public class UserService : IUserServices
    {
        private readonly IRepository<int, User> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITokenService _tokenService;

        public UserService(IRepository<int, User> repository, IMapper mapper, ILogger<UserService> logger, ITokenService tokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<bool> ChangePassword(ChangePasswordDTO changePassword)
        {
            var user = await _repository.Get(changePassword.UserId);
            HMACSHA256 hmac = new HMACSHA256(user.HashKey);
            byte[] currentPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changePassword.currentPassword));


            if (!currentPasswordHash.SequenceEqual(user.Password))
            {
                throw new Exception("Invalid current password");
            }


            if (changePassword.newPassword != changePassword.confirmPassword)
            {
                throw new Exception("New password and confirm password do not match");
            }

            HMACSHA256 newHmac = new HMACSHA256();
            byte[] newPasswordHash = newHmac.ComputeHash(Encoding.UTF8.GetBytes(changePassword.newPassword));


            user.Password = newPasswordHash;
            user.HashKey = newHmac.Key;

            var updatedUser = await _repository.Update(changePassword.UserId, user);

            return updatedUser != null;



        }

        public async Task<UserDTO> GetUserProfile(int id)
        {
            var user = await _repository.Get(id);
            // var userDTO = new _mapper.Map<UserDTO>(user);
            var userDTO = mappings(user);
            return userDTO;
        }

        public UserDTO mappings(User user)
        {
            // return _mapper.Map<UserDTO>(user);
            return new UserDTO()
            {
                UserName = user.UserName,
                Email = user.Email,
                Department = user.Department
            };
        }

        public async Task<LoginResponseDTO> Login(LoginDTO login)
        {
            try
            {

                var users = await _repository.GetAll();

                var user = users.FirstOrDefault(u =>
         string.Equals(u.Email, login.Email, StringComparison.OrdinalIgnoreCase));



                if (user == null)
                {
                    throw new NotFoundException("User not found");
                }
                HMACSHA256 hmac = new HMACSHA256(user.HashKey);
                byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));
                for (int i = 0; i < passwordHash.Length; i++)
                {

                    if (passwordHash[i] != user.Password[i])
                    {
                        throw new Exception("Invalid username or password");
                    }
                }
                return new LoginResponseDTO()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = await _tokenService.GenerateToken(new UserTokenDTO()
                    {
                        Username = user.UserName,
                        Department = user.Department.ToString()
                    })

                };
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException($"Failed to login {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to login {ex.Message}");
            }
        }

        public async Task<LoginResponseDTO> Register(UserCreateDTO registerUser)
        {
            HMACSHA256 hmac = new HMACSHA256();
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUser.Password));
            User user = new User()
            {
                UserName = registerUser.UserName,
                Password = passwordHash,
                HashKey = hmac.Key,
                Department = registerUser.Department,
                Email = registerUser.Email,
            };
            try
            {
                var addedUser = await _repository.Add(user);
                LoginResponseDTO response = new LoginResponseDTO()
                {
                    UserName = user.UserName,
                    Email = user.Email
                };
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not register user");
                throw new Exception("Could not register user");
            }
        }

        public async Task<IEnumerable<UserDTO>> SearchUser(string searchTerm)
        {
            var users = await _repository.GetAll();
            var filteredUsers = users.Where(u =>
         u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
         u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
         u.Department.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase));


            var sortedUsers = filteredUsers.OrderBy(u => u.UserName);


            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(sortedUsers);
            return userDTOs;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = await _repository.GetAll();
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
            return userDTOs;
        }
    }
}
