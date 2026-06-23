using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SetPoint.API._2.Controllers.Common;
using SetPoint.API.Common;
using SetPoint.BLL._02.UsersInvitationManagement;
using SetPoint.BLL._02.UsersInvitationManagement.Dto;

namespace SetPoint.API.Controllers.InvitationsControler
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : BaseController
    {
        #region Fields
        private readonly IUsersInvitationBll _invitationBll;
        #endregion


        #region Constructors
        public InvitationsController(IUsersInvitationBll invitationBll, ILogger<InvitationsController> logger) : base(logger)
        {
            _invitationBll = invitationBll;
        }
        #endregion


        #region Methods
        [EnableRateLimiting("SincronizacionLenta")]
        [HttpPost("accept")]
        [AllowAnonymous]
        public async Task<IActionResult> Accept([FromBody] AcceptInvitationDto dto)
        {
            try
            {
                var result = await _invitationBll.AcceptInvitationAsync(dto.Token, dto.FullName, dto.Password);

                if (result == null)
                    return NotFound(new ApiResponse
                    {
                        WithError = true,
                        Message = "Invalid or expired invitation",
                        StatusCode = 404
                    });

                return SuccessResponse(result, "Account created successfully.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on invitation accept");
                return Conflict(new ApiResponse
                {
                    WithError = true,
                    Message = ex.Message,
                    StatusCode = 409
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "Error processing invitation");
            }
        }
        #endregion
    }
}
