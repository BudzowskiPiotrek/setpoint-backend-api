using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SetPoint.API.Common;
using SetPoint.API.Controllers.InvitationsControler;
using SetPoint.BLL._02.UsersInvitationManagement;
using SetPoint.BLL._02.UsersInvitationManagement.Dto;
using SetPoint.BLL._02.UsersManagement.Dto;
namespace SetPoint.Api.Tests.Controllers
{
    public class InvitationsControllerTests
    {
        private readonly Mock<IUsersInvitationBll> _userInvitationBll = new();
        private readonly Mock<ILogger<InvitationsController>> _logger = new();
        private readonly InvitationsController _controller;

        public InvitationsControllerTests()
        {
            _controller = new(_userInvitationBll.Object, _logger.Object);
        }

        #region Accept 
        [Fact]
        public async Task InvitationsController_Accept_WhenBllReturnsDto_Returns200WithToken()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var dto = new AcceptInvitationDto
            {
                Token = Guid.NewGuid(),
                FullName = "fake-fullname",
                Password = "fake-password"
            };
            var respondeDto = new LoginResponseDto
            {
                Token = "fake-token",
                User = new UserReadDto
                {
                    FullName = "Test",
                    Email = "user@test.com",

                }
            };
            _userInvitationBll.Setup(x => x.AcceptInvitationAsync(It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<String>()))
                              .ReturnsAsync(respondeDto);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Accept(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            var apiResponse = objectResult!.Value as ApiResponse;
            objectResult!.StatusCode.Should().Be(200);
            apiResponse!.Result.Should().Be(respondeDto);
            _userInvitationBll.Verify(u => u.AcceptInvitationAsync(dto.Token, dto.FullName, dto.Password), Times.Once);
        }

        [Fact]
        public async Task InvitationsController_Accept_WhenBllReturnsNull_Returns404()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var dto = new AcceptInvitationDto
            {
                Token = Guid.NewGuid(),
                FullName = "fake-fullname",
                Password = "fake-password"
            };
            _userInvitationBll.Setup(x => x.AcceptInvitationAsync(It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<String>()))
                              .ReturnsAsync((LoginResponseDto?)null);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Accept(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task InvitationsController_Accept_WhenBllThrowsInvalidOperationException_Returns409()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var dto = new AcceptInvitationDto
            {
                Token = Guid.NewGuid(),
                FullName = "fake-fullname",
                Password = "fake-password"
            };
            _userInvitationBll.Setup(x => x.AcceptInvitationAsync(It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<String>()))
                              .ThrowsAsync(new InvalidOperationException("fake-error"));
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Accept(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task InvitationsController_Accept_WhenBllThrowsUnexpectedException_Returns500()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            var dto = new AcceptInvitationDto
            {
                Token = Guid.NewGuid(),
                FullName = "fake-fullname",
                Password = "fake-password"
            };
            _userInvitationBll.Setup(x => x.AcceptInvitationAsync(It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<String>()))
                              .ThrowsAsync(new Exception("fake-error"));
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await _controller.Accept(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }
        #endregion
    }
}
