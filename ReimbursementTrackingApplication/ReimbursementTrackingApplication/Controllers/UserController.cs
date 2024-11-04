using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;

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
    }
}
