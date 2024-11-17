using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using MimeKit;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userService;
        private readonly IMailSender _mailSender;


        public UserController(IUserServices userService, IMailSender mailSender)
        {
            _userService = userService;
            _mailSender = mailSender;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDTO>> Register(UserCreateDTO createDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.Register(createDTO);

var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            color: #333333;
        }}
        .email-container {{
            background-color: #ffffff;
            width: 100%;
            max-width: 600px;
            margin: 20px auto;
            border: 1px solid #dddddd;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .email-header {{
            background-color: #28a745;
            padding: 20px;
            color: white;
            text-align: center;
        }}
        .email-header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .email-body {{
            padding: 20px;
            line-height: 1.6;
        }}
        .email-body p {{
            font-size: 16px;
            margin-bottom: 10px;
        }}
        .email-footer {{
            padding: 10px;
            background-color: #f4f4f4;
            text-align: center;
            font-size: 14px;
            color: #888888;
        }}
        .email-footer a {{
            color: #28a745;
            text-decoration: none;
        }}
        .button {{
            background-color: #007bff;
            color: white;
            padding: 10px 20px;
            border-radius: 5px;
            text-decoration: none;
            display: inline-block;
            margin-top: 20px;
        }}
        .button:hover {{
            background-color: #0056b3;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>Welcome to Reimbursement Tracker</h1>
        </div>
        <div class='email-body'>
            <p>Dear {createDTO.UserName},</p>
            <p>We are pleased to inform you that your registration to the <strong>Reimbursement Tracker</strong> application has been successfully completed.</p>
            <p>With this application, you can now easily manage your reimbursement requests, track approvals, and get real-time updates on your claims.</p>
            <p>To get started, log in to your account using the link below:</p>
            <a href='#' class='button'>Log In to Your Account</a>
            <p>If you have any questions or need further assistance, feel free to contact our support team at any time.</p>
        </div>
        <div class='email-footer'>
            <p>Thank you for choosing Reimbursement Tracker.</p>
            <p><a href='#'>Visit our website</a></p>
        </div>
    </div>
</body>
</html>
";
                var message = new Message(new string[] { createDTO.Email }, "Registrations Successfull",htmlContent);
                _mailSender.SendEmail(message);

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
            try{

            if (ModelState.IsValid)
            {
                var user = await _userService.Login(login);
                return Ok(user);
            }
            else
            {
                throw new Exception("one or more validation errors");
            }
            }catch(Exception ex)
            {
                return Unauthorized( new ErrorResponseDTO()
                {
                    ErrorMessage = ex.Message,
                    ErrorNumber=StatusCodes.Status401Unauthorized
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUserProfileAsync(int id)
        {
            try
            {
                Console.WriteLine("hello"+User.FindFirst("Id")?.Value);

                var user = await _userService.GetUserProfile(id);
                return Ok(user);
            }catch(Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUserProfileAsyncByToken()
        {
            try
            {
                var id = User.FindFirst("Id")?.Value;
                var user = await _userService.GetUserProfile(Convert.ToInt32(id));
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
