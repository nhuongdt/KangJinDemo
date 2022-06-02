
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbContext _dbContext;
        private bool _disposed;
        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(this._dbContext);
        }
        public UnitOfWork(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
        }
        public void Save()
        {
            if (_dbContext.GetValidationErrors().Any())
            {
                throw (new Exception(_dbContext.GetValidationErrors().ToList()[0].ValidationErrors.ToList()[0].ErrorMessage));
            }
            _dbContext.SaveChanges();
        }
        public Task<int> SaveAsync()
        {
            if (_dbContext.GetValidationErrors().Any())
            {
                throw (new Exception(_dbContext.GetValidationErrors().ToList()[0].ValidationErrors.ToList()[0].ErrorMessage));
            }
           return _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }

            _disposed = true;

        }
    }
}
