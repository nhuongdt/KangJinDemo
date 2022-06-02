using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.Interfaces
{
   public interface IUnitOfWork
    {
       IRepository<T> GetRepository<T>() where T : class;
       void Save();
        Task<int> SaveAsync();
    }
}
