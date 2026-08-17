using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SetPoint.API.Common;
using SetPoint.API.Controllers.SyncController;
using SetPoint.BLL._0.Sync;
using SetPoint.BLL._0.Sync.Dto;
using System.Security.Claims;

namespace SetPoint.Api.Tests.Controllers
{
    public class SyncControllerTests
    {
        private readonly Mock<ISyncService> _syncBll = new();
        private readonly Mock<ILogger<SyncController>> _logger = new();
        private readonly SyncController _controller;

        public SyncControllerTests()
        {
            _controller = new(_syncBll.Object, _logger.Object);
        }


        #region Push
        [Fact]
        public async Task SyncController_Push_WhenModelStateInvalid_Returns400()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var payload = new SyncPayloadDto();
            _controller.ModelState.AddModelError("Token", "Required");
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Push(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(400);
            _syncBll.Verify(s => s.ProcessPush(It.IsAny<SyncPayloadDto>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SyncController_Push_WhenUserIdClaimMissing_Returns401()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var payload = new SyncPayloadDto
            {
                Token = "fake-token"
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Push(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(401);
            _syncBll.Verify(s => s.ProcessPush(It.IsAny<SyncPayloadDto>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SyncController_Push_WhenUserIdClaimInvalidGuid_Returns401()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var payload = new SyncPayloadDto
            {
                Token = "fake-token"
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "not-a-guid")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Push(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(401);
            _syncBll.Verify(s => s.ProcessPush(It.IsAny<SyncPayloadDto>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SyncController_Push_WhenBllReturnsResult_Returns200()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var userId = Guid.NewGuid();
            var payload = new SyncPayloadDto
            {
                Token = "fake-token"
            };
            var responseDto = new SyncErrorDetail
            {
                ItemId = new List<string>(),
                Success = new List<bool>()
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            _syncBll.Setup(s => s.ProcessPush(It.IsAny<SyncPayloadDto>(), It.IsAny<Guid>()))
                    .ReturnsAsync(responseDto);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Push(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            var apiResponse = objectResult!.Value as ApiResponse;
            objectResult!.StatusCode.Should().Be(200);
            apiResponse!.Result.Should().Be(responseDto);
            _syncBll.Verify(s => s.ProcessPush(payload, userId), Times.Once);
        }

        [Fact]
        public async Task SyncController_Push_WhenBllThrowsException_Returns500()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var userId = Guid.NewGuid();
            var payload = new SyncPayloadDto
            {
                Token = "fake-token"
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            _syncBll.Setup(s => s.ProcessPush(It.IsAny<SyncPayloadDto>(), It.IsAny<Guid>()))
                    .ThrowsAsync(new Exception("fake-error"));
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Push(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }
        #endregion

        #region Pull
        [Fact]
        public async Task SyncController_Pull_WhenModelStateInvalid_Returns400()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var payload = new PullRequestDto();
            _controller.ModelState.AddModelError("USER_ID", "Required");
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Pull(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(400);
            _syncBll.Verify(s => s.ProcessPull(It.IsAny<PullRequestDto>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SyncController_Pull_WhenUserIdClaimMissing_Returns401()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var payload = new PullRequestDto()
            {
                UserId = Guid.NewGuid(),
                LastSync = DateTime.UtcNow.AddDays(-1)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Pull(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(401);
            _syncBll.Verify(s => s.ProcessPull(It.IsAny<PullRequestDto>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SyncController_Pull_WhenUserIdClaimInvalidGuid_Returns401()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var payload = new PullRequestDto()
            {
                UserId = Guid.NewGuid(),
                LastSync = DateTime.UtcNow.AddDays(-1)
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "not-a-guid")
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Pull(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(401);
            _syncBll.Verify(s => s.ProcessPull(It.IsAny<PullRequestDto>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SyncController_Pull_WhenBllReturnsResult_Returns200()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var userId = Guid.NewGuid();
            var payload = new PullRequestDto()
            {
                UserId = Guid.NewGuid(),
                LastSync = DateTime.UtcNow.AddDays(-1)
            };
            var responseDto = new SyncPayloadDto();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            _syncBll.Setup(s => s.ProcessPull(It.IsAny<PullRequestDto>(), It.IsAny<Guid>()))
                    .ReturnsAsync(responseDto);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Pull(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            var apiResponse = objectResult!.Value as ApiResponse;
            objectResult!.StatusCode.Should().Be(200);
            apiResponse!.Result.Should().Be(responseDto);
            _syncBll.Verify(s => s.ProcessPull(It.IsAny<PullRequestDto>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task SyncController_Pull_WhenBllThrowsException_Returns500()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var userId = Guid.NewGuid();
            var payload = new PullRequestDto()
            {
                UserId = Guid.NewGuid(),
                LastSync = DateTime.UtcNow.AddDays(-1)
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
            _syncBll.Setup(s => s.ProcessPull(It.IsAny<PullRequestDto>(), It.IsAny<Guid>()))
                    .ThrowsAsync(new Exception("fake-error"));
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Pull(payload);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var resultObject = result as ObjectResult;
            var apiResponse = resultObject!.Value as ApiResponse;
            resultObject!.StatusCode.Should().Be(500);
            apiResponse!.WithError.Should().BeTrue();
            _syncBll.Verify(s => s.ProcessPull(It.IsAny<PullRequestDto>(), It.IsAny<Guid>()), Times.Once);
        }
        #endregion
    }
}
