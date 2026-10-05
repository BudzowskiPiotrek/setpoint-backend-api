using HabityFit.DAL._1.Entity;
using HabityFit.DAL._2.Context;

namespace HabityFit.BLL._01.LogsManagement
{
    public class LogsBll : ILogsBll
    {
        #region Fields
        private readonly HabityFitDbContext _context;
        #endregion


        #region Constructors
        public LogsBll(HabityFitDbContext context)
        {
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
