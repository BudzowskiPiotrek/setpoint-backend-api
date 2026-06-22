using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._0.Common;
using SetPoint.BLL._02.UserRelationManagement;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL.Tests.Management
{
    public class UsersRelationsBllTests
    {
        private static SetPointDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<SetPointDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SetPointDbContext(options);
        }

        public UsersRelationsBllTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        private readonly IMapper _mapper;

        #region CreateFriendshipAsync
        [Fact]
        public async Task CreateFriendshipAsync_WithTwoValidUsers__ReturnsTrueAndPersistsRelation()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var senderUser = new Users
            {
                Id = Guid.NewGuid(),
                Email = "sender@example.com",
                FullName = "Sender User",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,

            };
            var receiverUser = new Users
            {
                Id = Guid.NewGuid(),
                Email = "receiver@example.com",
                FullName = "Receiver User",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
            };
            context.Users.Add(senderUser);
            context.Users.Add(receiverUser);
            await context.SaveChangesAsync();

            var relationBll = new UserRelationBll(context, _mapper);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await relationBll.CreateFriendshipAsync(senderUser.Id, receiverUser.Id);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().BeTrue();
            var persisted = await context.UsersRelations.FirstOrDefaultAsync(r => r.UserId == senderUser.Id && r.FriendId == receiverUser.Id);
            persisted.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateFriendshipAsync_WhenRelationshipExists_ThrowsInvalidOperationException()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            // InMemory does not validate FKs — for this test, we only care that the relationship exists between these two GUIDs.
            var relation = new UsersRelations
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UserId = Guid.NewGuid(),
                FriendId = Guid.NewGuid(),
                Status = RelationStatus.Accepted
            };
            context.UsersRelations.Add(relation);
            await context.SaveChangesAsync();

            var relationBll = new UserRelationBll(context, _mapper);
            //---------------------------------------------------------------------------------------------------------------- Act
            Func<Task> act = async () => await relationBll.CreateFriendshipAsync(relation.UserId, relation.FriendId);
            //---------------------------------------------------------------------------------------------------------------- Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
        #endregion
    }
}
