using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SetPoint.BLL._0.Infrastructure;
using SetPoint.BLL._02.UsersInvitationManagement;
using SetPoint.BLL._02.UsersInvitationManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL.Tests.Management;

public class UsersInvitationBllTests
{
    private static IConfiguration MockConfigWithUrls()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["EmailSettings:DownloadUrl"]).Returns("https://test.com/download");
        configMock.Setup(c => c["EmailSettings:ActivationUrl"]).Returns("https://test.com/activate");
        return configMock.Object;
    }

    private static SetPointDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<SetPointDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SetPointDbContext(options);
    }

    private readonly Mock<IEmailService> _emailMock = new();
    private readonly Mock<ILogger<UsersInvitationBll>> _loggerMock = new();

    [Fact]
    public async Task CreateAndSendInvitationAsync_WhenEmailSendsSuccessfully_ReturnsTrueAndPersistsInvitation()
    {
        //---------------------------------------------------------------------------------------------------------------- Arrange
        await using var context = CreateInMemoryContext();
        _emailMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(true);
        var sut = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, context, _loggerMock.Object);
        var dto = new UsersInvitationDto
        {
            Id = Guid.NewGuid(),
            Email = "friend@test.com",
            SenderUserId = Guid.NewGuid()
        };
        //---------------------------------------------------------------------------------------------------------------- Act
        var result = await sut.CreateAndSendInvitationAsync(dto);
        //---------------------------------------------------------------------------------------------------------------- Assert
        result.Should().BeTrue();

        var persisted = await context.UsersInvitations.FindAsync(dto.Id);
        persisted.Should().NotBeNull();
        persisted!.Sended.Should().BeTrue();
        persisted.Status.Should().Be(InvitationStatus.Pending);
        persisted.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task CreateAndSendInvitationAsync_WhenEmailFails_ReturnsFalseButStillPersistsInvitation()
    {
        //---------------------------------------------------------------------------------------------------------------- Arrange
        await using var context = CreateInMemoryContext();
        _emailMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
          .ReturnsAsync(false);
        var sut = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, context, _loggerMock.Object);
        var dto = new UsersInvitationDto
        {
            Id = Guid.NewGuid(),
            Email = "friend@test.com",
            SenderUserId = Guid.NewGuid()
        };
        //---------------------------------------------------------------------------------------------------------------- Act
        var result = await sut.CreateAndSendInvitationAsync(dto);
        //---------------------------------------------------------------------------------------------------------------- Assert
        result.Should().BeFalse();

        var persisted = await context.UsersInvitations.FindAsync(dto.Id);
        persisted.Should().NotBeNull();
        persisted!.Sended.Should().BeFalse();
        persisted.Status.Should().Be(InvitationStatus.Pending);
        persisted.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task AcceptInvitationAsync_WithValidPendingToken_SetsStatusToAccepted()
    {
        //---------------------------------------------------------------------------------------------------------------- Arrange
        await using var context = CreateInMemoryContext();
        var invitation = new UsersInvitations
        {
            Id = Guid.NewGuid(),
            Email = "friend@test.com",
            Token = Guid.NewGuid(),
            Status = InvitationStatus.Pending,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow,
            Sended = true
        };
        context.UsersInvitations.Add(invitation);
        await context.SaveChangesAsync();
        var sut = new UsersInvitationBll(MockConfigWithUrls(), _emailMock.Object, context, _loggerMock.Object);
        //---------------------------------------------------------------------------------------------------------------- Act
        var result = await sut.AcceptInvitationAsync(invitation.Token, Guid.NewGuid());
        //---------------------------------------------------------------------------------------------------------------- Assert
        result.Should().BeTrue();
        var updated = await context.UsersInvitations.FindAsync(invitation.Id);
        updated!.Status.Should().Be(InvitationStatus.Accepted);
    }

    [Fact(Skip = "TODO: yet to be developed")]
    public async Task AcceptInvitationAsync_WithExpiredInvitation_ReturnsFalse()
    {
        //---------------------------------------------------------------------------------------------------------------- Arrange
        //---------------------------------------------------------------------------------------------------------------- Act
        //---------------------------------------------------------------------------------------------------------------- Assert
        Assert.Fail("TODO: yet to be developed");
    }

    [Fact(Skip = "TODO: yet to be developed")]
    public async Task AcceptInvitationAsync_WithInvalidToken_ReturnsFalse()
    {
        //---------------------------------------------------------------------------------------------------------------- Arrange
        //---------------------------------------------------------------------------------------------------------------- Act
        //---------------------------------------------------------------------------------------------------------------- Assert
        Assert.Fail("TODO: yet to be developed");
    }
}
