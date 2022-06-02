using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.Interfaces
{
    public interface IDbContextFactory
    {
        DbContext GetDbContext();
    }
    public class DbContextFactory<T> : IDbContextFactory where T : DbContext, new()
    {
        private readonly DbContext _context;
        public DbContextFactory()
        {
            _context = new T();
        }
        public DbContext GetDbContext()
        {
            return _context;
        }
    }
}
