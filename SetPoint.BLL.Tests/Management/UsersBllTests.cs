using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SetPoint.BLL._0.Common;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._02.UsersManagement.Dto;
using SetPoint.BLL._1.Security;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL.Tests.Management
{
    public class UsersBllTests
    {
        private static SetPointDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<SetPointDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SetPointDbContext(options);
        }

        public UsersBllTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        private readonly IMapper _mapper;
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IPasswordService> _passwordServiceMock = new();

        [Fact]
        public async Task CreateUserAsync_WithValidData_ReturnsLoginResponseDtoAndPersistsUser()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            _tokenServiceMock.Setup(ts => ts.CreateToken(It.IsAny<UserReadDto>())).Returns("mocked-token");
            _passwordServiceMock.Setup(ps => ps.HashPassword(It.IsAny<string>())).Returns("hashed-password");
            var userBll = new UserBll(context, _authServiceMock.Object, _mapper, _tokenServiceMock.Object, _passwordServiceMock.Object);
            var newUser = new UserDto
            {
                Email = "new@test.com",
                FullName = "New User",
                Password = "irrelevant-hash",
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await userBll.CreateUserAsync(newUser);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().NotBeNull();
            result!.User.Should().NotBeNull();
            result!.Token.Should().Be("mocked-token");

            var persisted = await context.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
            persisted.Should().NotBeNull();
            persisted.PasswordHash.Should().Be("hashed-password");
        }

        [Fact]
        public async Task CreateUserAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var existingUser = new Users
            {
                Id = Guid.NewGuid(),
                Email = "duplicate@test.com",
                FullName = "Existing User",
                PasswordHash = "hashed-password",
                CreatedAt = DateTime.UtcNow,
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();
            var userBll = new UserBll(context, _authServiceMock.Object, _mapper, _tokenServiceMock.Object, _passwordServiceMock.Object);
            var newUser = new UserDto
            {
                Email = "duplicate@test.com",
                FullName = "New User",
                Password = "irrelevant-hash",
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            Func<Task> act = async () => await userBll.CreateUserAsync(newUser);
            //---------------------------------------------------------------------------------------------------------------- Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
