using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._01.LogsManagement;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL.Tests.Management
{
    public class LogsBllTests
    {
        private static SetPointDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<SetPointDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SetPointDbContext(options);
        }

        #region CreateLogAsync
        [Fact]
        public async Task CreateLogAsync_WithValidData_ReturnsTrueAndPersistedLog()
        {
            //---------------------------------------------------------------------------------------------------------------- Arrange
            await using var context = CreateInMemoryContext();
            var userId = Guid.NewGuid();
            var type = "TestLog";
            var bll = new LogsBll(context);
            //---------------------------------------------------------------------------------------------------------------- Act
            var result = await bll.CreateLogAsync(userId, type);
            //---------------------------------------------------------------------------------------------------------------- Assert
            result.Should().BeTrue();

            var persistedLog = await context.Logs.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId && u.Type == type);
            using (new AssertionScope())
            {
                persistedLog.Should().NotBeNull();
                persistedLog!.UserId.Should().Be(userId);
                persistedLog.Type.Should().Be(type);
            }
        }
        #endregion
    }
}
