using System;
using Model;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AsposeCellsDocument;

namespace libGara
{
    public class ClassXe
    {
        private SsoftvnContext _db;
        public ClassXe(SsoftvnContext dbcontext)
        {
            _db = dbcontext;
        }

        public Gara_DanhMucXe GetGara_DanhMucXeById(Guid guid)
        {
            return _db.Gara_DanhMucXe.Where(p => p.ID == guid).FirstOrDefault();
        }
        public List<Xe> GetListXe(ParamSearch param)
        {
            var idChiNhanh = string.Join(",", param.LstIDChiNhanh);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("ID_HangXe", param.ID_HangXe));
            sql.Add(new SqlParameter("ID_LoaiXe", param.ID_LoaiXe));
            sql.Add(new SqlParameter("NamSanXuat", param.NamSanXuat));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            List<Xe> xx = _db.Database.SqlQuery<Xe>(" @IDChiNhanhs, @ID_HangXe, @ID_LoaiXe, @NamSanXuat, @TextSearch, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            return xx;
        }
        public List<Xe> JqAuto_SearchXe(string txt = null, string status = null, string idCustomer = null, int? laHangHoa = null, int? nguoisohuu = null)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("TextSearch", txt ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("StatusTN", status ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("IDCustomer", idCustomer ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("LaHangHoa", laHangHoa ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("NguoiSoHuu", nguoisohuu ?? (object)DBNull.Value));
            var data = _db.Database.SqlQuery<Xe>("exec JqAuto_SearchXe @TextSearch, @StatusTN, @IDCustomer, @LaHangHoa, @NguoiSoHuu", sql.ToArray()).ToList();
            return data;
        }

        public List<Xe> GetInforCar_ByID(Guid id)
        {
            var data = (from xe in _db.Gara_DanhMucXe
                        join mau in _db.Gara_MauXe on xe.ID_MauXe equals mau.ID into carAndmodel
                        from car_model in carAndmodel.DefaultIfEmpty()
                        join hang in _db.Gara_HangXe on car_model.ID_HangXe equals hang.ID into carAndManufact
                        from car_manufact in carAndManufact.DefaultIfEmpty()
                        join loaixe in _db.Gara_LoaiXe on car_model.ID_LoaiXe equals loaixe.ID into carType
                        from car_type in carType.DefaultIfEmpty()
                        join cx in _db.DM_DoiTuong on xe.ID_KhachHang equals cx.ID into cx_Xe
                        from chuXe in cx_Xe.DefaultIfEmpty()
                        join hh in _db.DM_HangHoa on xe.ID equals hh.ID_Xe into HH_Xe
                        from hhxe in HH_Xe.DefaultIfEmpty()
                        where xe.ID == id
                        select new Xe
                        {
                            ID = xe.ID,
                            ID_MauXe = xe.ID_MauXe,
                            ID_KhachHang = xe.ID_KhachHang,
                            ID_HangXe = car_model != null ? car_model.ID_HangXe : Guid.Empty,
                            ID_LoaiXe = car_model != null ? car_model.ID_LoaiXe : Guid.Empty,
                            BienSo = xe.BienSo,
                            SoKhung = xe.SoKhung,
                            SoMay = xe.SoMay,
                            HopSo = xe.HopSo,
                            DungTich = xe.DungTich,
                            MauSon = xe.MauSon,
                            NamSanXuat = xe.NamSanXuat,
                            TenMauXe = car_model != null ? car_model.TenMauXe : "",
                            TenHangXe = car_manufact != null ? car_manufact.TenHangXe : "",
                            TenLoaiXe = car_type != null ? car_type.TenLoaiXe : "",
                            TenDoiTuong = chuXe != null ? chuXe.TenDoiTuong : "",
                            DienThoai = chuXe != null ? chuXe.DienThoai : "",
                            MaDoiTuong = chuXe != null ? chuXe.MaDoiTuong : "",
                            ID_HangHoa = hhxe != null ? hhxe.ID : Guid.Empty,
                        }).ToList();
            return data;
        }

        public List<GetListGaraDanhMucXe_v1> GetListGaraDanhMucXe_v1(ParamGetListGaraDanhMucXe_v1 param)
        {
            string strTrangThai = "";
            if (param.TrangThais.Count > 0)
            {
                strTrangThai = string.Join(",", param.TrangThais);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IdHangXe", param.IdHangXe == "" ? (object)DBNull.Value : new Guid(param.IdHangXe)));
            sql.Add(new SqlParameter("IdLoaiXe", param.IdLoaiXe == "" ? (object)DBNull.Value : new Guid(param.IdLoaiXe)));
            sql.Add(new SqlParameter("IdMauXe", param.IdMauXe == "" ? (object)DBNull.Value : new Guid(param.IdMauXe)));
            sql.Add(new SqlParameter("TrangThais", strTrangThai));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));

            string sqlquery = "GetListGaraDanhMucXe_v1 @IdHangXe, @IdLoaiXe, @IdMauXe, " +
                "@TrangThais, @TextSearch, @CurrentPage, @PageSize";
            List<GetListGaraDanhMucXe_v1> xx = _db.Database.SqlQuery<GetListGaraDanhMucXe_v1>(sqlquery, sql.ToArray()).ToList();
            return xx;
        }

        public void Insert(Gara_DanhMucXe obj)
        {
            _db.Gara_DanhMucXe.Add(obj);
            _db.SaveChanges();
        }

        public void Update(Gara_DanhMucXe obj)
        {
            Gara_DanhMucXe xe = _db.Gara_DanhMucXe.Find(obj.ID);
            xe.BienSo = obj.BienSo;
            xe.ID_MauXe = obj.ID_MauXe;
            xe.ID_KhachHang = obj.ID_KhachHang;
            xe.SoKhung = obj.SoKhung;
            xe.SoMay = obj.SoMay;
            xe.NamSanXuat = obj.NamSanXuat;
            xe.MauSon = obj.MauSon;
            xe.DungTich = obj.DungTich;
            xe.HopSo = obj.HopSo;
            xe.GhiChu = obj.GhiChu;
            xe.NgaySua = DateTime.Now;
            xe.NguoiSua = obj.NguoiSua;
            xe.TrangThai = obj.TrangThai;
            xe.NguoiSoHuu = obj.NguoiSoHuu;
            _db.SaveChanges();
        }

        public void DeleteDanhMucXe(Guid id, int trangthai = 0)
        {
            var garaXe = _db.Gara_DanhMucXe.Find(id);
            garaXe.TrangThai = trangthai;
            _db.SaveChanges();
        }

        public bool ImportDanhSachXe(List<DanhSachXeImport> danhSachXeImports, string TaiKhoan)
        {
            using (DbContextTransaction transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    DateTime now = DateTime.Now;
                    List<Gara_LoaiXe> lstLoaiXe = new List<Gara_LoaiXe>();
                    lstLoaiXe = danhSachXeImports.GroupBy(p => p.TenLoaiXe).Select(p => p.Key).GroupJoin(_db.Gara_LoaiXe.Where(p => p.TrangThai == 1), ilx => ilx, dblx => dblx.TenLoaiXe, (ilx, dblx) => new { ilx, dblx })
                        .SelectMany(p => p.dblx.DefaultIfEmpty(), (p, dblx) => new Gara_LoaiXe
                        {
                            ID = dblx == null ? Guid.NewGuid() : dblx.ID,
                            MaLoaiXe = dblx == null ? CommonStatic.ConvertToUnSign(p.ilx) : dblx.MaLoaiXe,
                            TenLoaiXe = dblx == null ? p.ilx : dblx.TenLoaiXe,
                            TrangThai = 1,
                            NguoiTao = dblx == null ? TaiKhoan : dblx.NguoiTao,
                            NgayTao = dblx == null ? now : dblx.NgayTao
                        }).ToList();
                    List<Gara_HangXe> lstHangXe = new List<Gara_HangXe>();
                    lstHangXe = danhSachXeImports.GroupBy(p => p.TenHangXe).Select(p => p.Key).GroupJoin(_db.Gara_HangXe.Where(p => p.TrangThai == 1), ilx => ilx, dblx => dblx.TenHangXe, (ilx, dblx) => new { ilx, dblx })
                        .SelectMany(p => p.dblx.DefaultIfEmpty(), (p, dblx) => new Gara_HangXe
                        {
                            ID = dblx == null ? Guid.NewGuid() : dblx.ID,
                            MaHangXe = dblx == null ? CommonStatic.ConvertToUnSign(p.ilx) : dblx.MaHangXe,
                            TenHangXe = dblx == null ? p.ilx : dblx.TenHangXe,
                            TrangThai = 1,
                            NguoiTao = dblx == null ? TaiKhoan : dblx.NguoiTao,
                            NgayTao = dblx == null ? now : dblx.NgayTao
                        }).ToList();

                    List<Gara_MauXe> lstMauXe = new List<Gara_MauXe>();
                    lstMauXe = danhSachXeImports.GroupBy(p => new { p.TenMauXe, p.TenHangXe, p.TenLoaiXe }).Select(p => new
                    {
                        TenMauXe = p.Key.TenMauXe,
                        TenHangXe = p.Key.TenHangXe,
                        TenLoaiXe = p.Key.TenLoaiXe
                    }).GroupJoin(lstHangXe, imx => imx.TenHangXe, ihx => ihx.TenHangXe, (imx, ihx) => new { imx, ihx })
                    .SelectMany(p => p.ihx.DefaultIfEmpty(), (p, ihx) => new
                    {
                        TenMauXe = p.imx.TenMauXe,
                        ID_HangXe = ihx.ID,
                        TenLoaiXe = p.imx.TenLoaiXe
                    }).GroupJoin(lstLoaiXe, imhx => imhx.TenLoaiXe, ilx => ilx.TenLoaiXe, (imhx, ilx) => new { imhx, ilx })
                    .SelectMany(p => p.ilx.DefaultIfEmpty(), (p, ilx) => new
                    {
                        TenMauXe = p.imhx.TenMauXe,
                        ID_HangXe = p.imhx.ID_HangXe,
                        ID_LoaiXe = ilx.ID
                    }).GroupJoin(_db.Gara_MauXe.Where(p => p.TrangThai == 1), imx => new { imx.TenMauXe, imx.ID_HangXe, imx.ID_LoaiXe },
                    dbmx => new { dbmx.TenMauXe, dbmx.ID_HangXe, dbmx.ID_LoaiXe }, (imx, dbmx) => new { imx, dbmx })
                    .SelectMany(p => p.dbmx.DefaultIfEmpty(), (p, dbmx) => new Gara_MauXe
                    {
                        ID = dbmx == null ? Guid.NewGuid() : dbmx.ID,
                        TenMauXe = dbmx == null ? p.imx.TenMauXe : dbmx.TenMauXe,
                        ID_HangXe = p.imx.ID_HangXe,
                        ID_LoaiXe = p.imx.ID_LoaiXe,
                        NguoiTao = TaiKhoan,
                        NgayTao = dbmx == null ? now : dbmx.NgayTao
                    }).ToList();


                    List<Gara_DanhMucXe> lstDanhMucXe = new List<Gara_DanhMucXe>();
                    lstDanhMucXe = danhSachXeImports.GroupJoin(
                    lstMauXe.GroupJoin(lstHangXe, mx => mx.ID_HangXe, hx => hx.ID, (mx, hx) => new { mx, hx })
                        .SelectMany(p => p.hx.DefaultIfEmpty(), (p, hx) => new
                        {
                            ID = p.mx.ID,
                            TenMauXe = p.mx.TenMauXe,
                            TenHangXe = hx.TenHangXe,
                            ID_LoaiXe = p.mx.ID_LoaiXe
                        })
                        .GroupJoin(lstLoaiXe, mx => mx.ID_LoaiXe, lx => lx.ID, (mx, lx) => new { mx, lx })
                        .SelectMany(p => p.lx.DefaultIfEmpty(), (p, lx) => new
                        {
                            ID = p.mx.ID,
                            TenMauXe = p.mx.TenMauXe,
                            TenHangXe = p.mx.TenHangXe,
                            TenLoaiXe = lx.TenLoaiXe
                        }), i => new { i.TenMauXe, i.TenHangXe, i.TenLoaiXe }, mx => new { mx.TenMauXe, mx.TenHangXe, mx.TenLoaiXe }, (i, mx) => new { i, mx })
                        .SelectMany(p => p.mx.DefaultIfEmpty(), (p, mx) => new
                        {
                            BienSo = p.i.BienSo,
                            IDMauXe = mx.ID,
                            NamSanXuat = p.i.NamSanXuat,
                            SoMay = p.i.SoMay,
                            SoKhung = p.i.SoKhung,
                            MauSon = p.i.MauSon,
                            DungTich = p.i.DungTich,
                            HopSo = p.i.HopSo,
                            GhiChu = p.i.GhiChu,
                            MaKhachHang = p.i.MaKhachHang
                        }).GroupJoin(_db.DM_DoiTuong, gx => gx.MaKhachHang, dt => dt.MaDoiTuong, (gx, dt) => new { gx, dt })
                        .SelectMany(p => p.dt.DefaultIfEmpty(), (p, dt) => new Gara_DanhMucXe
                        {
                            ID = Guid.NewGuid(),
                            BienSo = p.gx.BienSo,
                            ID_KhachHang = dt?.ID,
                            ID_MauXe = p.gx.IDMauXe,
                            NamSanXuat = p.gx.NamSanXuat,
                            SoMay = p.gx.SoMay,
                            SoKhung = p.gx.SoKhung,
                            MauSon = p.gx.MauSon,
                            DungTich = p.gx.DungTich,
                            HopSo = p.gx.HopSo,
                            GhiChu = p.gx.GhiChu,
                            NguoiTao = TaiKhoan,
                            NgayTao = now
                        }).ToList();
                    //Insert to db
                    //insert loại xe
                    _db.Gara_LoaiXe.AddRange(lstLoaiXe.Where(p => p.NgayTao == now));
                    //insert hãng xe
                    _db.Gara_HangXe.AddRange(lstHangXe.Where(p => p.NgayTao == now));
                    //insert mẫu xe
                    _db.Gara_MauXe.AddRange(lstMauXe.Where(p => p.NgayTao == now));
                    //insert danh mục xe
                    _db.Gara_DanhMucXe.AddRange(lstDanhMucXe);
                    _db.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public List<GetListNhatKyBaoDuongTheoXe> GetListNhatKyBaoDuongTheoXe(Guid IdXe, int CurrentPage, int PageSize)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IdXe", IdXe));
            sql.Add(new SqlParameter("CurrentPage", CurrentPage));
            sql.Add(new SqlParameter("PageSize", PageSize));

            string sqlquery = "GetListNhatKyBaoDuongTheoXe @IdXe, @CurrentPage, @PageSize";
            List<GetListNhatKyBaoDuongTheoXe> xx = _db.Database.SqlQuery<GetListNhatKyBaoDuongTheoXe>(sqlquery, sql.ToArray()).ToList();
            return xx;
        }

        //public List<GetListLichBaoDuongTheoXe> GetListLichBaoDuongTheoXe(Guid IdXe, string TrangThai, int CurrentPage, int PageSize)
        //{
        //    List<SqlParameter> sql = new List<SqlParameter>();
        //    sql.Add(new SqlParameter("IdXe", IdXe));
        //    sql.Add(new SqlParameter("TrangThais", TrangThai));
        //    sql.Add(new SqlParameter("CurrentPage", CurrentPage));
        //    sql.Add(new SqlParameter("PageSize", PageSize));

        //    string sqlquery = "GetListLichBaoDuongTheoXe @IdXe, @TrangThais, @CurrentPage, @PageSize";
        //    List<GetListLichBaoDuongTheoXe> xx = _db.Database.SqlQuery<GetListLichBaoDuongTheoXe>(sqlquery, sql.ToArray()).ToList();
        //    return xx;
        //}
    }
}
