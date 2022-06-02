using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ssoft.Common.Cache
{
    public interface ICacheHelper
    {
        object Get(string key);
        void Set(string key, object data, int cacheTime = 180);
        bool IsSet(string key);
        void Invalidate(string key);
    }
}
