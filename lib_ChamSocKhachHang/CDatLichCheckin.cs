using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_ChamSocKhachHang
{
    public class CDatLichCheckin
    {
        private SsoftvnContext db;
        public CDatLichCheckin(SsoftvnContext _db)
        {
            db = _db;
        }

        public bool InsertDatLich(DatLichIn objIn)
        {
            try
            {
                CSKH_DatLich dl = new CSKH_DatLich();
                dl.Id = Guid.NewGuid();
                dl.IDDonVi = objIn.IdChiNhanh;
                dl.TenDoiTuong = objIn.TenKhachHang;
                dl.SoDienThoai = objIn.SoDienThoai;
                dl.DiaChi = objIn.DiaChi;
                dl.BienSo = objIn.BienSo;
                dl.LoaiXe = objIn.LoaiXe;
                dl.NgaySinh = objIn.NgaySinh;
                dl.ThoiGian = objIn.ThoiGian;
                dl.TrangThai = 1;
                dl.LoaiDatLich = objIn.LoaiDatLich;
                List<CSKH_DatLich_HangHoa> lstHangHoa = new List<CSKH_DatLich_HangHoa>();
                List<CSKH_DatLich_NhanVien> lstNhanVien = new List<CSKH_DatLich_NhanVien>();
                if (objIn.ListIdHangHoa.Count > 0)
                {
                    foreach (var item in objIn.ListIdHangHoa)
                    {
                        CSKH_DatLich_HangHoa hanghoa = new CSKH_DatLich_HangHoa();
                        hanghoa.Id = Guid.NewGuid();
                        hanghoa.IDDatLich = dl.Id;
                        hanghoa.IDHangHoa = item;
                        lstHangHoa.Add(hanghoa);
                    }
                }
                if (objIn.ListIdNhanVien.Count > 0)
                {
                    foreach (var item in objIn.ListIdNhanVien)
                    {
                        CSKH_DatLich_NhanVien nhanvien = new CSKH_DatLich_NhanVien();
                        nhanvien.Id = Guid.NewGuid();
                        nhanvien.IDDatLich = dl.Id;
                        nhanvien.IDNhanVien = item;
                        lstNhanVien.Add(nhanvien);
                    }
                }
                db.CSKH_DatLich.Add(dl);
                if (lstHangHoa.Count > 0)
                {
                    db.CSKH_DatLich_HangHoa.AddRange(lstHangHoa);
                }    
                if(lstNhanVien.Count > 0)
                {
                    db.CSKH_DatLich_NhanVien.AddRange(lstNhanVien);
                }
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public bool InsertDatLich(Guid IdKhachHang, Guid IdDonVi, Guid IdXe, DateTime ThoiGian, List<Guid> ListHangHoa, List<Guid> ListNhanVien, int LoaiDatLich, int TrangThai)
        {
            try
            {
                CSKH_DatLich dl = new CSKH_DatLich();
                dl.Id = Guid.NewGuid();
                dl.IDDonVi = IdDonVi;
                dl.IDDoiTuong = IdKhachHang;
                dl.IDXe = IdXe;
                dl.TrangThai = TrangThai;
                dl.LoaiDatLich = LoaiDatLich;
                dl.ThoiGian = ThoiGian;
                List<CSKH_DatLich_HangHoa> lstHangHoa = new List<CSKH_DatLich_HangHoa>();
                List<CSKH_DatLich_NhanVien> lstNhanVien = new List<CSKH_DatLich_NhanVien>();
                if (ListHangHoa.Count > 0)
                {
                    foreach (var item in ListHangHoa)
                    {
                        CSKH_DatLich_HangHoa hanghoa = new CSKH_DatLich_HangHoa();
                        hanghoa.Id = Guid.NewGuid();
                        hanghoa.IDDatLich = dl.Id;
                        hanghoa.IDHangHoa = item;
                        lstHangHoa.Add(hanghoa);
                    }
                }
                if (ListNhanVien.Count > 0)
                {
                    foreach (var item in ListNhanVien)
                    {
                        CSKH_DatLich_NhanVien nhanvien = new CSKH_DatLich_NhanVien();
                        nhanvien.Id = Guid.NewGuid();
                        nhanvien.IDDatLich = dl.Id;
                        nhanvien.IDNhanVien = item;
                        lstNhanVien.Add(nhanvien);
                    }
                }
                db.CSKH_DatLich.Add(dl);
                if (lstHangHoa.Count > 0)
                {
                    db.CSKH_DatLich_HangHoa.AddRange(lstHangHoa);
                }
                if (lstNhanVien.Count > 0)
                {
                    db.CSKH_DatLich_NhanVien.AddRange(lstNhanVien);
                }
                db.SaveChanges();
                return true;
            }
            catch 
            {
                return false;
            }

        }

        public bool UpdateDatLich(Guid Id, Guid IdKhachHang, Guid IdDonVi, Guid IdXe, DateTime ThoiGian, List<Guid> ListHangHoaAdd, List<Guid> ListHangHoaRemove,
            List<Guid> ListNhanVienAdd, List<Guid> ListNhanVienRemove)
        {
            try
            {
                CSKH_DatLich dl = new CSKH_DatLich();
                dl = db.CSKH_DatLich.Where(p => p.Id == Id).FirstOrDefault();
                if (dl != null)
                {
                    dl.IDDonVi = IdDonVi;
                    dl.IDDoiTuong = IdKhachHang;
                    dl.IDXe = IdXe;
                    dl.ThoiGian = ThoiGian;
                    List<CSKH_DatLich_HangHoa> lstHangHoa = new List<CSKH_DatLich_HangHoa>();
                    List<CSKH_DatLich_NhanVien> lstNhanVien = new List<CSKH_DatLich_NhanVien>();
                    if (ListHangHoaAdd.Count > 0)
                    {
                        foreach (var item in ListHangHoaAdd)
                        {
                            CSKH_DatLich_HangHoa hanghoa = new CSKH_DatLich_HangHoa();
                            hanghoa.Id = Guid.NewGuid();
                            hanghoa.IDDatLich = dl.Id;
                            hanghoa.IDHangHoa = item;
                            lstHangHoa.Add(hanghoa);
                        }
                    }
                    if (ListNhanVienAdd.Count > 0)
                    {
                        foreach (var item in ListNhanVienAdd)
                        {
                            CSKH_DatLich_NhanVien nhanvien = new CSKH_DatLich_NhanVien();
                            nhanvien.Id = Guid.NewGuid();
                            nhanvien.IDDatLich = dl.Id;
                            nhanvien.IDNhanVien = item;
                            lstNhanVien.Add(nhanvien);
                        }
                    }
                    List<CSKH_DatLich_HangHoa> lstHangHoaRemove = new List<CSKH_DatLich_HangHoa>();
                    List<CSKH_DatLich_NhanVien> lstNhanVienRemove = new List<CSKH_DatLich_NhanVien>();
                    if (ListHangHoaRemove.Count > 0)
                    {
                        foreach (var item in ListHangHoaRemove)
                        {
                            CSKH_DatLich_HangHoa hanghoa = db.CSKH_DatLich_HangHoa.Where(p => p.IDDatLich == dl.Id && p.IDHangHoa == item).FirstOrDefault();
                            if(hanghoa != null)
                                lstHangHoaRemove.Add(hanghoa);
                        }
                    }
                    if (ListNhanVienRemove.Count > 0)
                    {
                        foreach (var item in ListNhanVienRemove)
                        {
                            CSKH_DatLich_NhanVien nhanvien = db.CSKH_DatLich_NhanVien.Where(p => p.IDDatLich == dl.Id && p.IDNhanVien == item).FirstOrDefault();
                            if(nhanvien != null)
                                lstNhanVienRemove.Add(nhanvien);
                        }
                    }

                    if (lstHangHoa.Count > 0)
                    {
                        db.CSKH_DatLich_HangHoa.AddRange(lstHangHoa);
                    }
                    if (lstNhanVien.Count > 0)
                    {
                        db.CSKH_DatLich_NhanVien.AddRange(lstNhanVien);
                    }
                    if (lstHangHoaRemove.Count > 0)
                    {
                        db.CSKH_DatLich_HangHoa.RemoveRange(lstHangHoaRemove);
                    }
                    if (lstNhanVienRemove.Count > 0)
                    {
                        db.CSKH_DatLich_NhanVien.RemoveRange(lstNhanVienRemove);
                    }
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }

        }

        public List<GetListDatLichResult> GetListDatLich(ParamGetListDatLich param)
        {
            List<GetListDatLichResult> result = new List<GetListDatLichResult>();
            try
            {
                string idChiNhanh = "";
                if (param.IdChiNhanhs.Count > 0)
                {
                    idChiNhanh = string.Join(",", param.IdChiNhanhs);
                }

                string strTrangThai = "";
                if (param.TrangThais.Count > 0)
                {
                    strTrangThai = string.Join(",", param.TrangThais);
                }
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                sql.Add(new SqlParameter("ThoiGianFrom", param.ThoiGianFrom == null ? (object)DBNull.Value : param.ThoiGianFrom.Value));
                sql.Add(new SqlParameter("ThoiGianTo", param.ThoiGianTo == null ? (object)DBNull.Value : param.ThoiGianTo.Value));
                sql.Add(new SqlParameter("TrangThais", strTrangThai));
                sql.Add(new SqlParameter("TextSearch", param.TextSearch));
                sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
                sql.Add(new SqlParameter("PageSize", param.PageSize));

                string sqlquery = "GetListDatLich @IdChiNhanhs, @ThoiGianFrom, @ThoiGianTo, " +
                    "@TrangThais, @TextSearch, @CurrentPage, @PageSize";
                result = db.Database.SqlQuery<GetListDatLichResult>(sqlquery, sql.ToArray()).ToList();
            }
            catch
            {

            }
            return result;
        }

        public bool UpdateTrangThaiDatLich(int trangthai, Guid id, Guid iddonvi, Guid idnhanvien)
        {
            try
            {
                CSKH_DatLich datlich = db.CSKH_DatLich.Where(p => p.Id == id).FirstOrDefault();
                int trangthaiold = datlich.TrangThai;
                if(datlich != null)
                {
                    datlich.TrangThai = trangthai;
                }
                HT_NhatKySuDung nhatKy = new HT_NhatKySuDung();
                nhatKy.ID = Guid.NewGuid();
                nhatKy.ID_NhanVien = idnhanvien;
                nhatKy.ID_DonVi = iddonvi;
                nhatKy.LoaiNhatKy = 2;
                nhatKy.ChucNang = "Đặt lịch - Checkin";
                nhatKy.ThoiGian = DateTime.Now;
                nhatKy.NoiDung = "Cập nhật trạng thái đặt lịch - checkin";
                nhatKy.NoiDungChiTiet = "Cập nhật trang thái từ " + GetTextTrangThai(trangthaiold) + " sang " + GetTextTrangThai(trangthai);
                db.HT_NhatKySuDung.Add(nhatKy);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateDoiTuongDatLich(Guid id, Guid iddoituong)
        {
            try
            {
                CSKH_DatLich datlich = db.CSKH_DatLich.Where(p => p.Id == id).FirstOrDefault();
                if (datlich != null)
                {
                    datlich.IDDoiTuong = iddoituong;
                }
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateXeDatLich(Guid id, Guid idxe)
        {
            try
            {
                CSKH_DatLich datlich = db.CSKH_DatLich.Where(p => p.Id == id).FirstOrDefault();
                if (datlich != null)
                {
                    datlich.IDXe = idxe;
                }
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetTextTrangThai(int trangthai)
        {
            if(trangthai == 0)
            {
                return "Hủy";
            }    
            else if(trangthai == 1)
            {
                return "Chưa xử lý";
            }    
            else
            {
                return "Đã liên hệ";
            }
        }

        public List<HangHoaDatLichChiTiet> GetHangHoaDatLichChiTiet(Guid id)
        {
            List<HangHoaDatLichChiTiet> result = new List<HangHoaDatLichChiTiet>();
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Id", id));
                result = db.Database.SqlQuery<HangHoaDatLichChiTiet>("GetHangHoaDatLichChiTiet @Id", sql.ToArray()).ToList();
            }
            catch
            { }
            return result;
        }

        public List<NhanVienDatLichChiTiet> GetNhanVienDatLichChiTiet(Guid id)
        {
            List<NhanVienDatLichChiTiet> result = new List<NhanVienDatLichChiTiet>();
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Id", id));
                result = db.Database.SqlQuery<NhanVienDatLichChiTiet>("GetNhanVienDatLichChiTiet @Id", sql.ToArray()).ToList();
            }
            catch
            { }
            return result;
        }
    }
}
