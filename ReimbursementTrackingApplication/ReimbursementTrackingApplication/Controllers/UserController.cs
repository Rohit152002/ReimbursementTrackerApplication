using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userService;
        public UserController(IUserServices userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDTO>> Register(UserCreateDTO createDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.Register(createDTO);
                return Ok(user);
            }
            else
            {
                throw new Exception("one or more validation errors");
            }
        }
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.Login(login);
                return Ok(user);
            }
            else
            {
                throw new Exception("one or more validation errors");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserProfileAsync(int id)
        {
            try
            {

                var user = await _userService.GetUserProfile(id);
                return Ok(user);
            }catch(Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }

        [HttpGet("users")]
        public async Task<ActionResult<PaginatedResultDTO<UserDTO>>> GetAllUsersAsync(int pagenumber, int pageSize)
        {
            try
            {

            var users = await _userService.GetAllUsers(pagenumber, pageSize);
            return Ok(users);
            }
            catch(Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage=ex.Message,ErrorNumber=404});
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResultDTO<UserDTO>>> SearchUsers(string searchItem,int pagenumber, int pageSize)
        {
            try
            {
            var users = await _userService.SearchUser(searchItem,pagenumber, pageSize);
            return Ok(users);

            }catch(Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        [HttpPost("change")]
        public async Task<ActionResult<SuccessResponseDTO<bool>>> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            try
            {

            var result = await _userService.ChangePassword(changePasswordDTO);
            return Ok(new SuccessResponseDTO<bool>()
            { IsSuccess= true,
            Message="Password updated",
            Data=result});
            }catch(Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

    }
}
