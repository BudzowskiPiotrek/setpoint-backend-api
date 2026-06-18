
using Microsoft.Extensions.Configuration;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._01.LogsManagement
{
    public class LogsBll : ILogsBll
    {
        #region Fields
        public readonly IConfiguration _config;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public LogsBll(IConfiguration config, SetPointDbContext context)
        {
            _config = config;
            _context = context;
        }
        #endregion


        #region Methods
        public async Task<bool> CreateLogAsync(Guid userId, string type)
        {
            var log = new Logs
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };
            _context.Logs.Add(log);
            return (await _context.SaveChangesAsync()) > 0;
        }
        #endregion
    }
}
