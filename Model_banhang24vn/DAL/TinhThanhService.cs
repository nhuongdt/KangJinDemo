using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
     public  class TinhThanhService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());      
        IRepository<DM_TinhThanh> _TinhThanh;
        IRepository<TinhThanh_QuanHuyen> _TinhThanh_QuanHuyen;
        public TinhThanhService()
        {
            _TinhThanh = unitOfWork.GetRepository<DM_TinhThanh>();
            _TinhThanh_QuanHuyen = unitOfWork.GetRepository<TinhThanh_QuanHuyen>();
        }
        public IQueryable<DM_TinhThanh> Query { get { return _TinhThanh.All(); } }

        public IQueryable<TinhThanh_QuanHuyen> GetTinhThanhQuanHuyen()
        {
            return _TinhThanh_QuanHuyen.All();
        }
    }
}
