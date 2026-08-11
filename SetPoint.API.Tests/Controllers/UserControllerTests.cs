
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SetPoint.API.Common;
using SetPoint.API.Controllers;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._02.UsersManagement.Dto;

namespace SetPoint.Api.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserBll> _userBll = new();
        private readonly Mock<ILogger<UserController>> _logger = new();
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _controller = new(_userBll.Object, _logger.Object);
        }

        #region Login
        [Fact]
        public async Task UserController_Login_WhenBllReturnsUser_Returns200WithToken()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var dto = new LoginRequestDto
            {
                Email = "user@test.com",
                Password = "fake-password"
            };
            var responseDto = new LoginResponseDto
            {
                Token = "fake-token",
                User = new UserReadDto
                {
                    FullName = "Test",
                    Email = "user@test.com",
                }
            };
            _userBll.Setup(x => x.Login(It.IsAny<LoginRequestDto>()))
                    .ReturnsAsync(responseDto);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Login(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            var apiResponse = objectResult!.Value as ApiResponse;
            objectResult!.StatusCode.Should().Be(200);
            apiResponse!.Result.Should().Be(responseDto);
            _userBll.Verify(u => u.Login(dto), Times.Once);
        }

        [Fact]
        public async Task UserController_Login_WhenBllReturnsNull_Returns401()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var dto = new LoginRequestDto
            {
                Email = "user@test.com",
                Password = "wrong-password"
            };
            _userBll.Setup(x => x.Login(It.IsAny<LoginRequestDto>()))
                    .ReturnsAsync((LoginResponseDto?)null);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Login(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task UserController_Login_WhenModelStateInvalid_Returns400()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var dto = new LoginRequestDto
            {
                Email = "",
                Password = ""
            };
            _controller.ModelState.AddModelError("Email", "Required");
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Login(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(400);
            _userBll.Verify(u => u.Login(It.IsAny<LoginRequestDto>()), Times.Never);
        }

        [Fact]
        public async Task UserController_Login_WhenBllThrowsException_Returns500()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var dto = new LoginRequestDto
            {
                Email = "user@test.com",
                Password = "fake-password"
            };
            _userBll.Setup(x => x.Login(It.IsAny<LoginRequestDto>()))
                    .ThrowsAsync(new Exception("fake-error"));
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Login(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }
        #endregion
    }
}
