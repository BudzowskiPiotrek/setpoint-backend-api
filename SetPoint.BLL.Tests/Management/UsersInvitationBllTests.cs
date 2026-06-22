using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SetPoint.BLL._0.Infrastructure;
using SetPoint.BLL._02.UserRelationManagement;
using SetPoint.BLL._02.UsersInvitationManagement;
using SetPoint.BLL._02.UsersInvitationManagement.Dto;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._02.UsersManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL.Tests.Management
{

    public class UsersInvitationBllTests
    {
        private static SetPointDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<SetPointDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SetPointDbContext(options);
        }

        private static IConfiguration MockConfigWithUrls()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["EmailSettings:DownloadUrl"]).Returns("https://test.com/download");
            configMock.Setup(c => c["EmailSettings:ActivationUrl"]).Returns("https://test.com/activate");
            return configMock.Object;
        }

        private readonly Mock<IEmailService> _emailMock = new();
        private readonly Mock<ILogger<UsersInvitationBll>> _loggerMock = new();
        private readonly Mock<IUserBll> _userBllMock = new();
        private readonly Mock<IUserRelationBll> _userRelationBllMock = new();

        [Fact(Skip = "TODO: yet to be developed")]
        public async Task Base_For_Copy()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            //---------------------------------------------------------------------------------------------------------------- Act
            //---------------------------------------------------------------------------------------------------------------- Assert
            Assert.Fail("TODO: yet to be developed");
        }

        #region CreateAndSendInvitationAsync
        [Fact]
        public async Task CreateAndSendInvitationAsync_WhenEmailSendsSuccessfully_ReturnsTrueAndPersistsInvitation()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            _emailMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .ReturnsAsync(true);
            var bll = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, _userBllMock.Object, _userRelationBllMock.Object, context, _loggerMock.Object);
            var dto = new UsersInvitationDto
            {
                Id = Guid.NewGuid(),
                Email = "friend@test.com",
                SenderUserId = Guid.NewGuid()
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await bll.CreateAndSendInvitationAsync(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().BeTrue();

            var persisted = await context.UsersInvitations.FindAsync(dto.Id);
            persisted.Should().NotBeNull();
            using (new AssertionScope())
            {
                persisted!.Sended.Should().BeTrue();
                persisted.Status.Should().Be(InvitationStatus.Pending);
                persisted.Email.Should().Be(dto.Email);
            }
        }

        [Fact]
        public async Task CreateAndSendInvitationAsync_WhenEmailFails_ReturnsFalseButStillPersistsInvitation()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            _emailMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(false);
            var bll = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, _userBllMock.Object, _userRelationBllMock.Object, context, _loggerMock.Object);
            var dto = new UsersInvitationDto
            {
                Id = Guid.NewGuid(),
                Email = "friend@test.com",
                SenderUserId = Guid.NewGuid()
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await bll.CreateAndSendInvitationAsync(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().BeFalse();

            var persisted = await context.UsersInvitations.FindAsync(dto.Id);
            persisted.Should().NotBeNull();
            using (new AssertionScope())
            {
                persisted!.Sended.Should().BeFalse();
                persisted.Status.Should().Be(InvitationStatus.Pending);
                persisted.Email.Should().Be(dto.Email);
            }
        }

        [Fact]
        public async Task CreateAndSendInvitationAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var existingUser = new Users
            {
                Id = Guid.NewGuid(),
                Email = "existing@test.com",
                FullName = "Test User",
                PasswordHash = "irrelevant-hash",
                CreatedAt = DateTime.UtcNow
            };
            var dto = new UsersInvitationDto
            {
                Id = Guid.NewGuid(),
                Email = "existing@test.com",
                SenderUserId = Guid.NewGuid()
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();
            var bll = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, _userBllMock.Object, _userRelationBllMock.Object, context, _loggerMock.Object);
            //---------------------------------------------------------------------------------------------------------------- Act
            Func<Task> act = async () => await bll.CreateAndSendInvitationAsync(dto);
            //---------------------------------------------------------------------------------------------------------------- Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
        #endregion

        #region AcceptInvitationAsync
        [Fact]
        public async Task AcceptInvitationAsync_WithValidPendingToken_CreatesUserAndFriendshipAndReturnsLoginResponseDto()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var senderUser = new Users
            {
                Id = Guid.NewGuid(),
                Email = "sender@test.com",
                FullName = "Sender User",
                PasswordHash = "irrelevant-hash",
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(senderUser);
            var invitation = new UsersInvitations
            {
                Id = Guid.NewGuid(),
                Email = "friend@test.com",
                Token = Guid.NewGuid(),
                SenderUserId = senderUser.Id,
                Status = InvitationStatus.Pending,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow,
                Sended = true
            };
            context.UsersInvitations.Add(invitation);
            await context.SaveChangesAsync();
            var newUserId = Guid.NewGuid();
            var fakeLoginResponse = new LoginResponseDto
            {
                Token = "fake-jwt-token",
                User = new UserReadDto
                {
                    Id = newUserId,
                    Email = invitation.Email,
                    FullName = "Receiver User"
                }
            };
            _userBllMock.Setup(u => u.CreateUserAsync(It.Is<UserDto>(d => d.Email == invitation.Email && d.FullName == "Receiver User" && d.Password == "Password123!")))
                        .ReturnsAsync(fakeLoginResponse);
            _userRelationBllMock.Setup(r => r.CreateFriendshipAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                                .ReturnsAsync(true);
            var bll = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, _userBllMock.Object, _userRelationBllMock.Object, context, _loggerMock.Object);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await bll.AcceptInvitationAsync(invitation.Token, "Receiver User", "Password123!");
            //---------------------------------------------------------------------------------------------------------------- Assert
            using (new AssertionScope())
            {
                result.Should().BeSameAs(fakeLoginResponse);
                _userBllMock.Verify(u => u.CreateUserAsync(It.Is<UserDto>(d => d.Email == invitation.Email && d.FullName == "Receiver User")), Times.Once());
                _userRelationBllMock.Verify(r => r.CreateFriendshipAsync(senderUser.Id, newUserId), Times.Once());
                var updatedInvitation = await context.UsersInvitations.FindAsync(invitation.Id);
                updatedInvitation!.Status.Should().Be(InvitationStatus.Accepted);
            }
        }

        [Fact]
        public async Task AcceptInvitationAsync_WithExpiredInvitation_ReturnsNull()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var senderUser = new Users
            {
                Id = Guid.NewGuid(),
                Email = "sender@test.com",
                FullName = "Sender User",
                PasswordHash = "irrelevant-hash",
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(senderUser);
            var invitation = new UsersInvitations
            {
                Id = Guid.NewGuid(),
                Email = "friend@test.com",
                Token = Guid.NewGuid(),
                SenderUserId = senderUser.Id,
                Status = InvitationStatus.Pending,
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Sended = true
            };
            context.UsersInvitations.Add(invitation);
            await context.SaveChangesAsync();
            var bll = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, _userBllMock.Object, _userRelationBllMock.Object, context, _loggerMock.Object);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await bll.AcceptInvitationAsync(invitation.Token, "John Cena", "Password123!");
            //---------------------------------------------------------------------------------------------------------------- Assert
            using (new AssertionScope())
            {
                result.Should().BeNull();
                _userBllMock.Verify(u => u.CreateUserAsync(It.IsAny<UserDto>()), Times.Never());
                _userRelationBllMock.Verify(r => r.CreateFriendshipAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            }
        }

        [Fact]
        public async Task AcceptInvitationAsync_WithInvalidToken_ReturnsNull()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var token = Guid.NewGuid();
            var fullName = "fake-fullname";
            var password = "password";
            var bll = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, _userBllMock.Object, _userRelationBllMock.Object, context, _loggerMock.Object);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await bll.AcceptInvitationAsync(token, fullName, password);
            //---------------------------------------------------------------------------------------------------------------- Assert
            using (new AssertionScope())
            {
                result.Should().BeNull();
                _userBllMock.Verify(u => u.CreateUserAsync(It.IsAny<UserDto>()), Times.Never());
                _userRelationBllMock.Verify(r => r.CreateFriendshipAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            }
        }
        #endregion
    }
}