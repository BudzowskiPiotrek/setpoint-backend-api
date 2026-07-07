using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._0.Common;
using SetPoint.BLL._12.FeedEventManagement;
using SetPoint.BLL._12.FeedEventManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL.Tests.Management
{
    public class FeedEventBllTests
    {
        private static SetPointDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<SetPointDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SetPointDbContext(options);
        }

        public FeedEventBllTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        private readonly IMapper _mapper;

        #region SyncFeedEvent
        [Fact]
        public async Task SyncFeedEvent_WithValidData_PersistsFeedEvent()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var feedEventBll = new FeedEventBll(context, _mapper);
            var newFeedEvent = new FeedEventDto
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EventType = FeedEventType.START_SESSION,

            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await feedEventBll.SyncFeedEvent(newFeedEvent);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().BeTrue();
            var persisted = await context.FeedEvents.FirstOrDefaultAsync(fe => fe.Id == newFeedEvent.Id);
            persisted.Should().NotBeNull();
        }

        [Fact]
        public async Task SyncFeedEvent_WithExistingFeedEvent_UpdatesFeedEvent()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var existingFeedEvent = new FeedEventDto
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                EventType = FeedEventType.START_SESSION,
            };
            context.FeedEvents.Add(_mapper.Map<FeedEvent>(existingFeedEvent));
            await context.SaveChangesAsync();
            var feedEventBll = new FeedEventBll(context, _mapper);
            var updatedFeedEvent = new FeedEventDto
            {
                Id = existingFeedEvent.Id,
                CreatedAt = existingFeedEvent.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                EventType = FeedEventType.GENERAL,
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await feedEventBll.SyncFeedEvent(updatedFeedEvent);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().BeTrue();
            var persisted = await context.FeedEvents.FirstOrDefaultAsync(fe => fe.Id == updatedFeedEvent.Id);
            persisted.Should().NotBeNull();
            persisted!.UpdatedAt.Should().Be(updatedFeedEvent.UpdatedAt);
            persisted!.EventType.Should().Be(updatedFeedEvent.EventType);
        }

        [Fact]
        public async Task SyncFeedEvent_WithOlderUpdatedAt_DoesNotUpdateFeedEvent()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var existingFeedEvent = new FeedEventDto
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                EventType = FeedEventType.START_SESSION,
            };
            context.FeedEvents.Add(_mapper.Map<FeedEvent>(existingFeedEvent));
            await context.SaveChangesAsync();
            var feedEventBll = new FeedEventBll(context, _mapper);
            var olderFeedEvent = new FeedEventDto
            {
                Id = existingFeedEvent.Id,
                CreatedAt = existingFeedEvent.CreatedAt,
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                EventType = FeedEventType.GENERAL,
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await feedEventBll.SyncFeedEvent(olderFeedEvent);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().BeTrue();
            var persisted = await context.FeedEvents.FirstOrDefaultAsync(fe => fe.Id == olderFeedEvent.Id);
            persisted.Should().NotBeNull();
            persisted!.UpdatedAt.Should().Be(existingFeedEvent.UpdatedAt);
            persisted!.EventType.Should().Be(existingFeedEvent.EventType);
        }
        [Fact]
        public async Task SyncFeedEvent_WithoutDeletedAt_ResetsDeletedAt()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var existingFeedEvent = new FeedEventDto
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = DateTime.UtcNow.AddDays(-1),
                EventType = FeedEventType.START_SESSION,
            };
            context.FeedEvents.Add(_mapper.Map<FeedEvent>(existingFeedEvent));
            await context.SaveChangesAsync();
            var feedEventBll = new FeedEventBll(context, _mapper);
            var updatedFeedEvent = new FeedEventDto
            {
                Id = existingFeedEvent.Id,
                CreatedAt = existingFeedEvent.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                EventType = FeedEventType.GENERAL,
            };
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await feedEventBll.SyncFeedEvent(updatedFeedEvent);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().BeTrue();
            var persisted = await context.FeedEvents.FirstOrDefaultAsync(fe => fe.Id == updatedFeedEvent.Id);
            persisted.Should().NotBeNull();
            persisted!.DeletedAt.Should().BeNull();
        }
        #endregion
    }
}
