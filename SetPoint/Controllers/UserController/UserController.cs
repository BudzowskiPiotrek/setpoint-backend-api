using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SetPoint.API._2.Controllers._0.Common;
using SetPoint.API.Common;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._02.UsersManagement.Dto;
using System.Text.RegularExpressions;

namespace SetPoint.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        #region Fields

        private readonly IUserBll _userBll;

        private static readonly string _emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        private static readonly string _passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";

        #endregion


        #region Constructors
        public UserController(IUserBll userBll)
        {
            _userBll = userBll;
        }

        #endregion


        #region Methods
        ////[Authorize]
        //[EnableRateLimiting("SincronizacionLenta")]
        //[HttpPost("create")]
        //public async Task<IActionResult> Create([FromBody] UserDto userDto)
        //{

        //    if (!ModelState.IsValid)
        //        return BadRequest(new ApiResponse
        //        {
        //            WithError = true,
        //            Message = "Invalid data",
        //            StatusCode = 400
        //        });

        //    try
        //    {
        //        ValidateUserDto(userDto);

        //        var success = await _userBll.CreateUser(userDto);

        //        return SuccessResponse(success, "User created successfully.", 201);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ErrorResponse(ex, "An error occurred while creating the user");
        //    }
        //}


        //[HttpPut("update")]
        //public async Task<IActionResult> Update([FromBody] UserDto userDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(new ApiResponse
        //        {
        //            WithError = true,
        //            Message = "Invalid data",
        //            StatusCode = 400
        //        });

        //    try
        //    {
        //        ValidateUserDto(userDto);

        //        var result = await _userBll.UpdateUser(userDto);

        //        return SuccessResponse(result, "User updated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ErrorResponse(ex, "Error updating user");
        //    }
        //}

        //[HttpDelete("delete")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    try
        //    {
        //        var success = await _userBll.DeleteUser(id);

        //        if (!success)
        //            return NotFound(new ApiResponse
        //            {
        //                WithError = true,
        //                Message = "User not found",
        //                StatusCode = 404
        //            });

        //        return SuccessResponse(success, "User deleted successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ErrorResponse(ex, "Error deleting user");
        //    }
        //}

        //[HttpGet("getbyid")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    try
        //    {
        //        var user = await _userBll.GetUserById(id);

        //        if (user == null)
        //            return NotFound(new ApiResponse
        //            {
        //                WithError = true,
        //                Message = "User not found",
        //                StatusCode = 404
        //            });

        //        return SuccessResponse(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ErrorResponse(ex, "Error retrieving user");
        //    }
        //}

        //[HttpGet("getbyemail")]
        //public async Task<IActionResult> GetByEmail(string email)
        //{
        //    try
        //    {
        //        var user = await _userBll.GetUserByEmail(email);

        //        if (user == null)
        //            return NotFound(new ApiResponse
        //            {
        //                WithError = true,
        //                Message = "User not found",
        //                StatusCode = 404
        //            });

        //        return SuccessResponse(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ErrorResponse(ex, "Error retrieving user");
        //    }
        //}

        //[HttpGet("getall")]
        //public async Task<IActionResult> GetAll()
        //{
        //    try
        //    {
        //        var users = await _userBll.GetAllUsers();

        //        return SuccessResponse(users);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ErrorResponse(ex, "Error retrieving users");
        //    }
        //}

        [EnableRateLimiting("SincronizacionLenta")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse
                {
                    WithError = true,
                    Message = "Email and password required",
                    StatusCode = 400
                });

            try
            {
                var user = await _userBll.Login(loginRequest);

                if (user == null)
                    return Unauthorized(new ApiResponse
                    {
                        WithError = true,
                        Message = "Invalid credentials",
                        StatusCode = 401
                    });

                return SuccessResponse(user, "Login successful.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "Login error");
            }
        }

        //[Authorize]
        //[EnableRateLimiting("SincronizacionLenta")]
        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout(Guid id)
        //{
        //    try
        //    {
        //        var success = await _userBll.Logout(id);

        //        if (!success)
        //            return BadRequest(new ApiResponse
        //            {
        //                WithError = true,
        //                Message = "Logout failed",
        //                StatusCode = 400
        //            });

        //        return SuccessResponse(success, "Logout successful.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ErrorResponse(ex, "Logout error");
        //    }
        //}

        #endregion


        #region Private Mthods
        private void ValidateUserDto(UserDto userDto)
        {
            if (userDto == null)
                throw new Exception("User data cannot be null.");

            if (string.IsNullOrWhiteSpace(userDto.Email))
                throw new Exception("Email data cannot be null.");

            if (string.IsNullOrWhiteSpace(userDto.Password))
                throw new Exception("Password data cannot be null.");

            if (!Regex.IsMatch(userDto.Email, _emailPattern))
                throw new Exception("The email format is invalid.");

            if (!Regex.IsMatch(userDto.Password, _passwordPattern))
                throw new Exception("Password must be at least 8 characters long and contain at least one letter and one number.");
        }

        #endregion
    }
}
