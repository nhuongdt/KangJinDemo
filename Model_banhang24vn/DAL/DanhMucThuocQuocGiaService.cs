using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
    public class DanhMucThuocQuocGiaService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<DanhMucThuocQuocGia> _danhmucthuoc;
        public DanhMucThuocQuocGiaService()
        {
            _danhmucthuoc = unitOfWork.GetRepository<DanhMucThuocQuocGia>();
        }

        public IQueryable<DanhMucThuocQuocGia> GetAll()
        {
            return _danhmucthuoc.All();
        }
    }
}
