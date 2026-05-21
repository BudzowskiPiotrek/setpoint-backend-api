
using Microsoft.Extensions.Configuration;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._01.LogsManagement
{
    public class LogsBll : ILogsBll
    {
        #region Fields

        public readonly IConfiguration _config;

        public readonly string _connectionString;

        #endregion


        #region Constructors

        public LogsBll(IConfiguration config)
        {
            _config = config;

            _connectionString = _config.GetConnectionString("PostgreConnection")
                ?? throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");
        }


        #endregion


        #region Methods

        public async Task<bool> CreateLogAsync(Guid userId, string type)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = type,
                    CreatedAt = DateTime.UtcNow
                };
                context.Logs.Add(log);
                return (await context.SaveChangesAsync()) > 0;
            }
        }

        #endregion
    }
}
