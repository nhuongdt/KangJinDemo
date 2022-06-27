using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace libNS_NhanVien
{
    public class ClassNS_NhanVien
    {
        private readonly SsoftvnContext db;
        public ClassNS_NhanVien(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        /// <summary>
        /// get list chiet khau mac dinh of all nhan vien by ID_DonViQuiDoi
        /// </summary>
        /// <param name="idQuiDoi"></param>
        /// <returns></returns>
        public List<SP_ChietKhauNV> SP_GetListChietKhauNhanVien_By_IDQuiDoi(string idQuiDoi, Guid idChiNhanh)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    List<SqlParameter> lstPr = new List<SqlParameter>();
                    lstPr.Add(new SqlParameter("ID_DonViQuiDoi", idQuiDoi));
                    lstPr.Add(new SqlParameter("ID_DonVi", idChiNhanh));
                    var data = db.Database.SqlQuery<SP_ChietKhauNV>("exec SP_GetListChietKhauNhanVien_By_IDQuiDoi @ID_DonViQuiDoi, @ID_DonVi", lstPr.ToArray()).ToList();
                    if (data != null)
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    CookieStore.WriteLog("SP_GetListChietKhauNhanVien_By_IDQuiDoi " + e.InnerException + e.Message);
                    return null;
                }
            }

        }

        public List<GetListNhanVienDatLichCheckinResult> GetListNhanVienDatLich(Guid id)
        {
            List<GetListNhanVienDatLichCheckinResult> result = new List<GetListNhanVienDatLichCheckinResult>();
            try
            {
                List<SqlParameter> lst = new List<SqlParameter>();
                lst.Add(new SqlParameter("IdDonVi", id));
                result = db.Database.SqlQuery<GetListNhanVienDatLichCheckinResult>("exec GetListNhanVienDatLichCheckin @IdDonVi", lst.ToArray()).ToList();
            }
            catch
            {

            }
            return result;
        }

        public NS_NhanVien Select_NhanVien(Guid? id)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                return db.NS_NhanVien.Find(id);
            }
        }
        public IQueryable<NS_NhanVien> Gets(Expression<Func<NS_NhanVien, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.NS_NhanVien.OrderByDescending(x => x.MaNhanVien);
                else
                    return db.NS_NhanVien.Where(query);
            }
        }
        // trinhpv getListNhanVien_DonVi
        public List<NS_NhanVien_DonVi> getListNhanVien_DonVi(string ID_ChiNhanh, string nameNV)
        {
            List<NS_NhanVien_DonVi> lst = new List<NS_NhanVien_DonVi>();
            try
            {
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tb1 = db.Database.SqlQuery<NS_NhanVien_DonVi>("exec getListNhanVien_allDonVi @ID_ChiNhanh", prm.ToArray()).ToList();
                var tb = tb1.AsEnumerable().Select(t => new NS_NhanVien_DonVi
                {
                    ID = t.ID,
                    MaNhanVien = t.MaNhanVien,
                    TenNhanVien = t.TenNhanVien,
                    ID_PhongBan = t.ID_PhongBan,
                    TenNhanVien_GC = CommonStatic.ConvertToUnSign(t.TenNhanVien).ToLower(),
                    TenNhanVien_CV = CommonStatic.GetCharsStart(t.TenNhanVien).ToLower()
                });
                if (nameNV != null & nameNV != "null" & nameNV != "")
                {
                    nameNV = CommonStatic.ConvertToUnSign(nameNV).ToLower();
                    tb = tb.Where(x => x.TenNhanVien_CV.Contains(@nameNV) || x.TenNhanVien_GC.Contains(@nameNV) || x.MaNhanVien.Contains(@nameNV));
                }

                lst = tb.OrderBy(x => x.TenNhanVien).ToList();
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("NS_NhanVien_DonVi " + e.InnerException + e.Message);
            }
            return lst;
        }
        public List<NS_ReportNhanVien> getListNhanViens(string maNhanVen, int trangthai)
        {
            string str = CookieStore.GetCookieAes("SubDomain");
            string MaNVSearch = CommonStatic.ConvertToUnSign(maNhanVen).ToLower();
            var tb_fm = from nv in db.NS_NhanVien
                        orderby nv.MaNhanVien descending
                        select new NS_ReportNhanVien
                        {
                            ID = nv.ID,
                            TenNhanVien = nv.TenNhanVien,
                            MaNhanVien = nv.MaNhanVien,
                            DienThoaiDiDong = nv.DienThoaiDiDong,
                            GioiTinh = nv.GioiTinh,
                            SoBHXH = nv.SoBHXH,
                            SoCMND = nv.SoCMND,
                            GhiChu = nv.GhiChu,
                            NgaySinh = nv.NgaySinh,
                            ThuongTru = nv.ThuongTru,
                            NguyenQuan = nv.NguyenQuan,
                            Email = nv.Email,
                            DaNghiViec = nv.DaNghiViec,
                            Image = nv.NS_NhanVien_Anh.Any() ? nv.NS_NhanVien_Anh.FirstOrDefault().URLAnh : string.Empty
                        };
            var tb = tb_fm.AsEnumerable().Select(t => new NS_ReportNhanVien
            {
                ID = t.ID,
                TenNhanVien = t.TenNhanVien,
                TenNhanVien_KhongDau = CommonStatic.ConvertToUnSign(t.TenNhanVien).ToLower(),
                TenNhanVien_ChuCaiDau = CommonStatic.GetCharsStart(t.TenNhanVien).ToLower(),
                MaNhanVien = t.MaNhanVien,
                DienThoaiDiDong = t.DienThoaiDiDong,
                GioiTinh = t.GioiTinh,
                SoBHXH = t.SoBHXH,
                SoCMND = t.SoCMND,
                GhiChu = t.GhiChu,
                NgaySinh = t.NgaySinh,
                ThuongTru = t.ThuongTru,
                NguyenQuan = t.NguyenQuan,
                Email = t.Email,
                DaNghiViec = t.DaNghiViec,
                Image = t.Image
            });
            if (maNhanVen != "" & maNhanVen != null & maNhanVen != "null")
            {
                tb = tb.Where(x => x.MaNhanVien.ToLower().Contains(@MaNVSearch) || x.TenNhanVien_KhongDau.Contains(@MaNVSearch) || x.TenNhanVien_ChuCaiDau.Contains(@MaNVSearch));
            }
            if (trangthai == 1)
            {
                tb = tb.Where(x => x.DaNghiViec == true);
            }
            if (trangthai == 2)
            {
                tb = tb.Where(x => x.DaNghiViec == false);
            }
            List<NS_ReportNhanVien> lst = new List<NS_ReportNhanVien>();
            try
            {
                lst = tb.ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("List<NS_ReportNhanVien> getListNhanViens(string maNhanVen, int trangthai): " + ex.InnerException + ex.Message, str);
                lst = null;
            }
            return lst;
            //foreach (var item in tb)
            //{
            //    NS_NhanVienDTO DM = new NS_NhanVienDTO();
            //    DM.ID = item.ID;
            //    DM.TenNhanVien = item.TenNhanVien;
            //    DM.MaNhanVien = item.MaNhanVien;
            //    DM.DienThoaiDiDong = item.DienThoaiDiDong;
            //    DM.GioiTinh = item.GioiTinh;
            //    DM.SoBHXH = item.SoBHXH;
            //    DM.SoCMND = item.SoCMND;
            //    DM.GhiChu = item.GhiChu;
            //    DM.NgaySinh = item.NgaySinh;
            //    DM.ThuongTru = item.ThuongTru;
            //    DM.NguyenQuan = item.NguyenQuan;
            //    DM.Email = item.Email;
            //    DM.DaNghiViec = item.DaNghiViec;
            //    lst.Add(DM);
            //}
            //if (lst != null)
            //{
            //    return lst;
            //}
            //else
            //{
            //    return null;
            //}
        }

        public IQueryable<NS_NhanVien> getListNhanViens_news(Guid? phongbanId, string maNhanVen, int trangthai)
        {
            var listPhongban = new List<Guid>();
            if (phongbanId != null)
            {
                var phongban = db.NS_PhongBan.FirstOrDefault(o => o.ID == phongbanId);
                if (phongban != null && phongban.ID_PhongBanCha != null)
                {
                    listPhongban = db.NS_PhongBan.Where(o => o.ID_PhongBanCha == phongban.ID || o.ID == phongban.ID).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                }
                else if (phongban != null)
                {
                    listPhongban = db.NS_PhongBan.Where(o => o.ID_PhongBanCha == phongban.ID || o.ID == phongban.ID).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                    var listPhongbannew = db.NS_PhongBan.Where(o => listPhongban.Contains(o.ID_PhongBanCha ?? new Guid())).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                    listPhongban.AddRange(listPhongbannew);
                    listPhongban = listPhongban.Distinct().ToList();
                }
            }
            string MaNVSearch = CommonStatic.ConvertToUnSign(maNhanVen).ToLower();
            var tb_fm = db.NS_NhanVien.Where(o => (o.TrangThai == null || o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa) &&
                        (listPhongban.Contains(o.ID_NSPhongBan ?? new Guid()) || phongbanId == null)).AsEnumerable();


            if (maNhanVen != "" & maNhanVen != null & maNhanVen != "null")
            {
                maNhanVen = maNhanVen.ToLower();
                tb_fm = tb_fm.Where(x => x.MaNhanVien.ToLower().Contains(MaNVSearch)
                || x.TenNhanVien.ToLower().Contains(maNhanVen)
                || (x.TenNhanVienKhongDau != null && x.TenNhanVienKhongDau.ToLower().Contains(MaNVSearch))
                || (x.TenNhanVienChuCaiDau != null && x.TenNhanVienChuCaiDau.ToLower().Contains(MaNVSearch)));
            }
            if (trangthai == 1)
            {
                tb_fm = tb_fm.Where(x => x.DaNghiViec == true);
            }
            if (trangthai == 2)
            {
                tb_fm = tb_fm.Where(x => x.DaNghiViec == false);
            }
            return tb_fm.OrderByDescending(o => o.NgayTao).AsQueryable();
        }

        public List<Guid?> GetlistIDPhongBanChild(List<Guid?> lstid_nsphongban, Guid idphongban)
        {
            List<NS_PhongBan> lstPhongBanByIDParent = db.NS_PhongBan.Where(p => p.ID_PhongBanCha == idphongban).ToList();
            if (lstPhongBanByIDParent.Count() == 0)
            {
                return lstid_nsphongban;
            }
            else
            {
                foreach (var item in lstPhongBanByIDParent)
                {
                    lstid_nsphongban = GetlistIDPhongBanChild(lstid_nsphongban, item.ID);

                    lstid_nsphongban.Add(item.ID);
                }
                return lstid_nsphongban;
            }
        }
        public string GetPhongBanHienThoi(Guid nhanvienId)
        {
            var phongban = db.NS_QuaTrinhCongTac.FirstOrDefault(o => o.ID_NhanVien == nhanvienId && o.LaDonViHienThoi);
            if (phongban == null)
            {
                return string.Empty;
            }
            return phongban.NS_PhongBan.TenPhongBan;

        }
        public List<DM_DonViNV> getlistQTCongTac(Guid ID_NhanVien)
        {
            var tb = from qtct in db.NS_QuaTrinhCongTac
                     join dv in db.DM_DonVi on qtct.ID_DonVi equals dv.ID

                     where qtct.ID_NhanVien == ID_NhanVien
                     select new
                     {
                         ID = dv.ID,
                         MaDonVi = dv.MaDonVi,
                         TenDonVi = dv.TenDonVi,
                         DiaChi = dv.DiaChi,
                         SoDienThoai = dv.SoDienThoai
                     };
            List<DM_DonViNV> lst = new List<DM_DonViNV>();
            foreach (var item in tb)
            {
                DM_DonViNV DM = new DM_DonViNV();
                DM.ID = item.ID;
                DM.MaDonVi = item.MaDonVi;
                DM.TenDonVi = item.TenDonVi;
                DM.DiaChi = item.DiaChi;
                DM.SoDienThoai = item.SoDienThoai;
                lst.Add(DM);
            }
            if (lst != null)
            {
                return lst;
            }

            else
            {
                return null;
            }
        }

        public List<HD_NhanVien> getlistHD_NhanVien(Guid ID_NhanVien)
        {
            List<HD_NhanVien> lst = new List<HD_NhanVien>();
            List<SqlParameter> prm = new List<SqlParameter>();
            prm.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
            lst = db.Database.SqlQuery<HD_NhanVien>("exec getList_HoaDonbyNhanVien @ID_NhanVien", prm.ToArray()).ToList();
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }

        }

        public IQueryable<Quy_HoaDon> getListSQ_NhanVien(Expression<Func<Quy_HoaDon, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.Quy_HoaDon;
                else
                    return db.Quy_HoaDon.Where(query);
            }
        }
        public JsonViewModel<string> UpdatettChinhTri(NS_NhanVien_new model, HT_NguoiDung User)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Nhân viên không tồn tại hoặc đã bị xóa";
            }
            else
            {
                data.NgayVaoDangChinhThuc = model.NgayVaoDangChinhThuc;
                data.NgayVaoDoan = model.NgayVaoDoan;
                data.NoiVaoDoan = model.NoiVaoDoan;
                data.NoiSinhHoatDang = model.NoiSinhHoatDang;
                data.GhiChuThongTinChinhTri = model.GhiChuThongTinChinhTri;
                data.NgayXuatNgu = model.NgayXuatNgu;
                data.NgayNhapNgu = model.NgayNhapNgu;
                data.NgayVaoDang = model.NgayVaoDang;
                data.NgayRoiDang = model.NgayRoiDang;
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                {
                    ID = Guid.NewGuid(),
                    ID_NhanVien = User.ID_NhanVien,
                    ID_DonVi = User.ID_DonVi ?? new Guid(),
                    ChucNang = "Nhân viên",
                    ThoiGian = DateTime.Now,
                    NoiDung = "Cập nhật thông tin chính trị nhân viên: " + model.TenNhanVien,
                    NoiDungChiTiet = "Cập nhật thông tin chính trị nhân viên: <a style= \"cursor: pointer\" onclick = \"loadNhanVienbyMaKM('" + model.MaNhanVien + "')\" >" + model.MaNhanVien + "</a> <br>",
                    LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update
                };
                db.HT_NhatKySuDung.Add(hT_NhatKySuDung);
                db.SaveChanges();
                result.ErrorCode = true;
            }

            return result;

        }

        public void InsertNhanVienGiaDinh(NS_NhanVien_GiaDinh model, HT_NhatKySuDung history)
        {
            db.NS_NhanVien_GiaDinh.Add(model);
            var nguoidung = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
            if (nguoidung != null)
            {
                history.NoiDung = "Thêm mới thông tin gia đình nhân viên có Mã: " + nguoidung.MaNhanVien + " - Tên: " + nguoidung.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin gia đình nhân viên được thêm mới:</br>" +
                                        "Họ tên: " + model.HoTen +
                                         "<br /> Quan hệ: " + model.QuanHe +
                                        "<br /> Nơi ở: " + model.NoiO;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);
            }
            db.SaveChanges();

        }
        public JsonViewModel<string> UpdateNhanVienGiaDinh(NS_NhanVien_GiaDinh model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_NhanVien_GiaDinh.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (data.NS_NhanVien != null)
                {
                    history.NoiDung = "Cập nhật thông thông tin gia đình nhân viên có Mã: " + data.NS_NhanVien.MaNhanVien + " - Tên: " + data.NS_NhanVien.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin gia đình nhân viên được Cập nhật lại:</br>" +
                                            "Họ tên: " + data.HoTen + " => " + model.HoTen +
                                             "<br /> Quan hệ : " + data.QuanHe + " => " + model.QuanHe +
                                            "<br /> Nơi ở: " + data.NoiO + " => " + model.NoiO;
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                    db.HT_NhatKySuDung.Add(history);
                }

                data.DiaChi = model.DiaChi;
                data.HoTen = model.HoTen;
                data.NgaySinh = model.NgaySinh;
                data.NoiO = model.NoiO;
                data.QuanHe = model.QuanHe;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }


        public void InsertNvTTDaoTao(NS_NhanVien_DaoTao model, HT_NhatKySuDung history)
        {
            db.NS_NhanVien_DaoTao.Add(model);
            var nguoidung = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
            if (nguoidung != null)
            {
                history.NoiDung = "Thêm mới quy trình đào tạo  nhân viên có Mã: " + nguoidung.MaNhanVien + " - Tên: " + nguoidung.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin quy trình đào tạo nhân viên được thêm mới:</br>" +
                                        "Nơi học: " + model.NoiHoc +
                                         "<br />Ngành học: " + model.NganhHoc +
                                        "<br /> Hệ đào tạo: " + model.HeDaoTao +
                                        "<br /> Bằng cấp: " + model.BangCap +
                                        "<br /> Từ ngày : " + model.TuNgay.ToString("dd/MM/yyyy") + (model.DenNgay != null ? " - Đến ngày:" + model.DenNgay.Value.ToString("dd/MM/yyyy") : string.Empty);
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);
            }
            db.SaveChanges();

        }
        public JsonViewModel<string> UpdateNvTTDaoTao(NS_NhanVien_DaoTao model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_NhanVien_DaoTao.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (data.NS_NhanVien != null)
                {
                    history.NoiDung = "Cập nhật quy trình đào tạo  nhân viên có Mã: " + data.NS_NhanVien.MaNhanVien + " - Tên: " + data.NS_NhanVien.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin quy trình đào tạo nhân viên được cập nhật lại:</br>" +
                                            "Nơi học: " + data.NoiHoc + " => " + model.NoiHoc +
                                             "<br />Ngành học: " + data.NganhHoc + " => " + model.NganhHoc +
                                            "<br /> Hệ đào tạo: " + data.HeDaoTao + " => " + model.HeDaoTao +
                                            "<br /> Bằng cấp: " + data.BangCap + " => " + model.BangCap +
                                            "<br /> Từ ngày : " + model.TuNgay.ToString("dd/MM/yyyy") + (model.DenNgay != null ? " - Đến ngày:" + model.DenNgay.Value.ToString("dd/MM/yyyy") : string.Empty);
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                    db.HT_NhatKySuDung.Add(history);
                }
                data.BangCap = model.BangCap;
                data.DenNgay = model.DenNgay;
                data.HeDaoTao = model.HeDaoTao;
                data.NganhHoc = model.NganhHoc;
                data.NoiHoc = model.NoiHoc;
                data.TuNgay = model.TuNgay;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }


        public void InsertNvQTCongTac(NS_NhanVien_CongTac model, HT_NhatKySuDung history)
        {
            db.NS_NhanVien_CongTac.Add(model);
            var nguoidung = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
            if (nguoidung != null)
            {
                history.NoiDung = "Thêm mới quá trình công tác nhân viên có Mã: " + nguoidung.MaNhanVien + " - Tên: " + nguoidung.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin quá trình công tác nhân viên được thêm mới:</br>" +
                                        "Cơ quan: " + model.CoQuan +
                                         "<br />Vị trí: " + model.ViTri +
                                        "<br /> Địa chỉ: " + model.DiaChi +
                                        "<br /> Từ ngày : " + model.TuNgay.ToString("dd/MM/yyyy") + (model.DenNgay != null ? " - Đến ngày:" + model.DenNgay.Value.ToString("dd/MM/yyyy") : string.Empty);
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);
            }
            db.SaveChanges();

        }
        public JsonViewModel<string> UpdateNvQTCongTac(NS_NhanVien_CongTac model, HT_NhatKySuDung history)
        {

            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_NhanVien_CongTac.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (data.NS_NhanVien != null)
                {
                    history.NoiDung = "Cập nhật quá trình công tác nhân viên có Mã: " + data.NS_NhanVien.MaNhanVien + " - Tên: " + data.NS_NhanVien.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin quá trình công tác nhân viên được cập nhật lại:</br>" +
                                            "Cơ quan: " + data.CoQuan + " => " + model.CoQuan +
                                             "<br />Vị trí: " + data.ViTri + " => " + model.ViTri +
                                            "<br /> Địa chỉ: " + data.DiaChi + " => " + model.DiaChi +
                                            "<br /> Từ ngày : " + model.TuNgay.ToString("dd/MM/yyyy") + (model.DenNgay != null ? " - Đến ngày:" + model.DenNgay.Value.ToString("dd/MM/yyyy") : string.Empty);
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                    db.HT_NhatKySuDung.Add(history);
                }
                data.CoQuan = model.CoQuan;
                data.DenNgay = model.DenNgay;
                data.ViTri = model.ViTri;
                data.DiaChi = model.DiaChi;
                data.TuNgay = model.TuNgay;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }


        public void InsertNvTTSucKhoe(NS_NhanVien_SucKhoe model, HT_NhatKySuDung history)
        {
            db.NS_NhanVien_SucKhoe.Add(model);
            var nguoidung = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
            if (nguoidung != null)
            {
                history.NoiDung = "Thêm mới thông tin sức khỏe nhân viên có Mã: " + nguoidung.MaNhanVien + " - Tên: " + nguoidung.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin sức khỏe nhân viên được thêm mới:</br>" +
                                        "Chiều cao: " + model.ChieuCao +
                                         "<br />Cân nặng: " + model.CanNang +
                                        "<br /> Tình hính sức khỏe: " + model.TinhHinhSucKhoe +
                                        "<br /> Ngày khám : " + model.NgayKham.ToString("dd/MM/yyyy");
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);
            }
            db.SaveChanges();

        }
        public JsonViewModel<string> UpdateNvTTSuwcKhoe(NS_NhanVien_SucKhoe model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_NhanVien_SucKhoe.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (data.NS_NhanVien != null)
                {
                    history.NoiDung = "Cập nhật tình trạng sức khỏe nhân viên có Mã: " + data.NS_NhanVien.MaNhanVien + " - Tên: " + data.NS_NhanVien.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin quá trình công tác nhân viên được cập nhật lại:</br>" +
                                            "Chiều cao: " + data.ChieuCao + " => " + model.ChieuCao +
                                             "<br />Cân nặng: " + data.CanNang + " => " + model.CanNang +
                                            "<br />Tính hình sức khỏe: " + data.TinhHinhSucKhoe + " => " + model.TinhHinhSucKhoe +
                                            "<br /> Ngày khám : " + data.NgayKham.ToString("dd/MM/yyyy") + " => " + model.NgayKham.ToString("dd/MM/yyyy");
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                    db.HT_NhatKySuDung.Add(history);
                }
                data.NgayKham = model.NgayKham;
                data.TinhHinhSucKhoe = model.TinhHinhSucKhoe;
                data.ChieuCao = model.ChieuCao;
                data.CanNang = model.CanNang;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }


        public JsonViewModel<string> InsertNvHopDong(NS_HopDong model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };

            if (db.NS_HopDong.Any(o => o.SoHopDong.ToUpper().Equals(model.SoHopDong.ToUpper()) && o.LoaiHopDong == model.LoaiHopDong && o.ID_NhanVien == model.ID_NhanVien))
            {
                result.Data = "Số hợp đồng nhân viên này đã tồn tại";

            }
            else
            {
                db.NS_HopDong.Add(model);
                var nguoidung = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
                if (nguoidung != null)
                {
                    var thoihan = commonEnumHellper.ListThoiHan.FirstOrDefault(o => o.Key == model.DonViThoiHan);
                    var loaihopdong = commonEnumHellper.ListLoaiHopDong.FirstOrDefault(o => o.Key == model.LoaiHopDong);
                    history.NoiDung = "Thêm mới thông tin hợp đồng nhân viên có Mã: " + nguoidung.MaNhanVien + " - Tên: " + nguoidung.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin hợp đồng nhân viên được thêm mới:</br>" +
                                            "Số hợp đồng: " + model.SoHopDong +
                                             "<br />Loại hợp đồng: " + loaihopdong.Value +
                                            "<br /> Thời hạn: " + model.ThoiHan + "(" + thoihan.Value + ")" +
                                            "<br /> Ngày ký : " + model.NgayKy.ToString("dd/MM/yyyy");
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                    db.HT_NhatKySuDung.Add(history);
                    result.ErrorCode = true;
                }
                db.SaveChanges();
            }

            return result;
        }
        public JsonViewModel<string> UpdateNvHOpDong(NS_HopDong model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_HopDong.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi hợp đồng không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (data.NS_NhanVien != null)
                {
                    var thoihanold = commonEnumHellper.ListThoiHan.FirstOrDefault(o => o.Key == data.DonViThoiHan);
                    var loaihopdongold = commonEnumHellper.ListLoaiHopDong.FirstOrDefault(o => o.Key == data.LoaiHopDong);
                    var thoihan = commonEnumHellper.ListThoiHan.FirstOrDefault(o => o.Key == model.DonViThoiHan);
                    var loaihopdong = commonEnumHellper.ListLoaiHopDong.FirstOrDefault(o => o.Key == model.LoaiHopDong);
                    history.NoiDung = "Cập nhật hợp đồng nhân viên có Mã: " + data.NS_NhanVien.MaNhanVien + " - Tên: " + data.NS_NhanVien.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin hợp đồng nhân viên được cập nhật lại:</br>" +
                                            "Số hợp đồng: " + data.SoHopDong + " => " + model.SoHopDong +
                                             "<br />Loại hợp đồng: " + loaihopdongold.Value + " => " + loaihopdong.Value +
                                            "<br />Thời hạn: " + data.ThoiHan + "(" + thoihanold.Value + ")" + " => " + model.ThoiHan + "(" + thoihan.Value + ")" +
                                            "<br /> Ngày ký : " + data.NgayKy.ToString("dd/MM/yyyy") + " => " + model.NgayKy.ToString("dd/MM/yyyy");
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                    db.HT_NhatKySuDung.Add(history);
                }
                data.DonViThoiHan = model.DonViThoiHan;
                data.GhiChu = model.GhiChu;
                data.LoaiHopDong = model.LoaiHopDong;
                data.NgayKy = model.NgayKy;
                data.SoHopDong = model.SoHopDong;
                data.ThoiHan = model.ThoiHan;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }


        public void InsertNvKhenThuong(NS_KhenThuong model, HT_NhatKySuDung history)
        {
            db.NS_KhenThuong.Add(model);
            var nguoidung = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
            if (nguoidung != null)
            {
                history.NoiDung = "Thêm mới thông tin khen thưởng nhân viên có Mã: " + nguoidung.MaNhanVien + " - Tên: " + nguoidung.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin khen thưởng nhân viên được thêm mới:</br>" +
                                        "Hình thức: " + model.HinhThuc +
                                         "<br />Số quyết định: " + model.SoQuyetDinh +
                                        "<br /> Nội dung: " + model.NoiDung +
                                        "<br /> Ngày ban hành : " + model.NgayBanHang.ToString("dd/MM/yyyy");
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);
            }
            db.SaveChanges();

        }
        public JsonViewModel<string> UpdateNvKhenThuong(NS_KhenThuong model, HT_NhatKySuDung history)
        {

            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_KhenThuong.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi khen thưởng không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (data.NS_NhanVien != null)
                {
                    history.NoiDung = "Cập nhật thông tin khen thưởng nhân viên có Mã: " + data.NS_NhanVien.MaNhanVien + " - Tên: " + data.NS_NhanVien.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin khen thưởng nhân viên được cập nhật lại:</br>" +
                                          "Hình thức: " + data.HinhThuc + " => " + model.HinhThuc +
                                         "<br />Số quyết định: " + data.SoQuyetDinh + " => " + model.SoQuyetDinh +
                                        "<br /> Nội dung: " + data.NoiDung + " => " + model.NoiDung +
                                        "<br /> Ngày ban hành : " + data.NgayBanHang.ToString("dd/MM/yyyy") + " => " + model.NgayBanHang.ToString("dd/MM/yyyy");
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                    db.HT_NhatKySuDung.Add(history);
                }
                data.GhiChu = model.GhiChu;
                data.HinhThuc = model.HinhThuc;
                data.NgayBanHang = model.NgayBanHang;
                data.NoiDung = model.NoiDung;
                data.SoQuyetDinh = model.SoQuyetDinh;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }

        public void InsertNvMienGiamThue(NS_MienGiamThue model, HT_NhatKySuDung history)
        {

            db.NS_MienGiamThue.Add(model);
            var nguoidung = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
            if (nguoidung != null)
            {
                history.NoiDung = "Thêm mới thông tin miễn giảm thuế nhân viên có Mã: " + nguoidung.MaNhanVien + " - Tên: " + nguoidung.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin miễn giảm thuế nhân viên được thêm mới:</br>" +
                                        "Khoản miễn giảm: " + model.KhoanMienGiam +
                                         "<br />Số tiền: " + CommonStatic.FormatVND(model.SoTien) +
                                        "<br /> Ngày áp dụng : " + model.NgayApDung.ToString("dd/MM/yyyy") + (model.NgayKetThuc != null ? "Ngày kết thúc: " + model.NgayKetThuc.Value.ToString("dd/MM/yyyy") : string.Empty);
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);
            }
            db.SaveChanges();

        }
        public JsonViewModel<string> UpdateNvMienGiamThue(NS_MienGiamThue model, HT_NhatKySuDung history)
        {

            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_MienGiamThue.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi miễn giảm thuế không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (data.NS_NhanVien != null)
                {
                    history.NoiDung = "Cập nhật thông tin miễn giảm thuế nhân viên có Mã: " + data.NS_NhanVien.MaNhanVien + " - Tên: " + data.NS_NhanVien.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin miễn giảm thuế nhân viên được cập nhật lại:</br>" +
                                          "Khoản miễn giảm: " + data.KhoanMienGiam + " => " + model.KhoanMienGiam +
                                         "<br />Số tiền: " + CommonStatic.FormatVND(data.SoTien) + " => " + CommonStatic.FormatVND(model.SoTien) +
                                        "<br /> Ngày áp dụng  : " + model.NgayApDung.ToString("dd/MM/yyyy") + (model.NgayKetThuc != null ? "Ngày kết thúc: " + model.NgayKetThuc.Value.ToString("dd/MM/yyyy") : string.Empty);
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                    db.HT_NhatKySuDung.Add(history);
                }
                data.GhiChu = model.GhiChu;
                data.KhoanMienGiam = model.KhoanMienGiam;
                data.NgayApDung = model.NgayApDung;
                data.NgayKetThuc = model.NgayKetThuc;
                data.SoTien = model.SoTien;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;
        }

        public bool CheckExist_ThietLapLuong(NS_Luong_PhuCap model)
        {
            var sql = string.Concat("select dbo.CheckExist_ThietLapLuong ('", model.ID_DonVi, "','", model.ID_NhanVien, "', '", model.ID, "', '",
                model.ID_LoaiLuong, "',", model.LoaiLuong, ", '", model.NgayApDung.ToString("yyyy-MM-dd"), "', '",
                model.NgayKetThuc != null ? model.NgayKetThuc.Value.ToString("yyyy-MM-dd") : string.Empty, "')");
            return db.Database.SqlQuery<bool>(sql).First();
        }

        public JsonViewModel<string> InsertNvLuongPhuCap(NS_Luong_PhuCap model, List<NS_ThietLapLuongChiTiet> lst)
        {
            var result = new JsonViewModel<string>() { ErrorCode = true };
            var ctluong = string.Empty;

            model.ID = Guid.NewGuid();
            model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
            db.NS_Luong_PhuCap.Add(model);

            if (lst != null && lst.Count() > 0)
            {
                List<NS_ThietLapLuongChiTiet> lstAdd = new List<NS_ThietLapLuongChiTiet>();
                foreach (var item in lst)
                {
                    NS_ThietLapLuongChiTiet ct = new NS_ThietLapLuongChiTiet
                    {
                        ID = Guid.NewGuid(),
                        ID_LuongPhuCap = model.ID,
                        ID_CaLamViec = item.ID_CaLamViec,
                        LuongNgayThuong = item.LuongNgayThuong,
                        NgayThuong_LaPhanTramLuong = item.NgayThuong_LaPhanTramLuong,
                        Thu7_GiaTri = item.Thu7_GiaTri,
                        Thu7_LaPhanTramLuong = item.Thu7_LaPhanTramLuong,
                        ThCN_GiaTri = item.ThCN_GiaTri,
                        CN_LaPhanTramLuong = item.CN_LaPhanTramLuong,
                        NgayNghi_GiaTri = item.NgayNghi_GiaTri,
                        NgayNghi_LaPhanTramLuong = item.NgayNghi_LaPhanTramLuong,
                        NgayLe_GiaTri = item.NgayLe_GiaTri,
                        NgayLe_LaPhanTramLuong = item.NgayLe_LaPhanTramLuong,
                        LaOT = item.LaOT,
                    };
                    lstAdd.Add(ct);
                }
                db.NS_ThietLapLuongChiTiet.AddRange(lstAdd);
            }
            db.SaveChanges();

            var _NhanSuService = new NhanSuService(db);
            if (model.LoaiLuong < 5)
            {
                _NhanSuService.UpdateCong_WhenChangeThietLapLuong(model.ID, model.ID_NhanVien, model.ID_DonVi ?? Guid.Empty);
            }
            _NhanSuService.UpdateStatusBangLuong_whenChangeCong(model.ID_DonVi ?? Guid.Empty, DateTime.Now);

            result.ErrorCode = false;
            result.Data = db.Database.SqlQuery<string>(" SELECT dbo.Diary_LuongPhuCap('" + model.ID + "')").First();
            return result;
        }

        public void InsertThietLapLuongChiTiet(List<NS_ThietLapLuongChiTiet> lst)
        {
            db.NS_ThietLapLuongChiTiet.AddRange(lst);
        }

        public JsonViewModel<string> UpdateNvLuongPhuCap(NS_Luong_PhuCap model, List<NS_ThietLapLuongChiTiet> lst)
        {

            var result = new JsonViewModel<string>() { ErrorCode = true };
            var data = db.NS_Luong_PhuCap.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi lương phụ cấp không tồn tại hoặc đã bị xóa";
            }
            else
            {
                var inforOld = db.Database.SqlQuery<string>(" SELECT dbo.Diary_LuongPhuCap('" + model.ID + "')").First();

                data.NoiDung = model.NoiDung;
                data.HeSo = model.HeSo;
                data.NgayApDung = model.NgayApDung;
                data.NgayKetThuc = model.NgayKetThuc;
                data.SoTien = model.SoTien;
                data.Bac = model.Bac;
                data.LoaiLuong = model.LoaiLuong;
                data.ID_LoaiLuong = model.ID_LoaiLuong;

                // delete & add again
                var lstCT = db.NS_ThietLapLuongChiTiet.Where(x => x.ID_LuongPhuCap == model.ID);
                db.NS_ThietLapLuongChiTiet.RemoveRange(lstCT);

                if (lst != null && lst.Count() > 0)
                {
                    List<NS_ThietLapLuongChiTiet> lstAdd = new List<NS_ThietLapLuongChiTiet>();
                    foreach (var item in lst)
                    {
                        NS_ThietLapLuongChiTiet ct = new NS_ThietLapLuongChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_LuongPhuCap = model.ID,
                            ID_CaLamViec = item.ID_CaLamViec,
                            LuongNgayThuong = item.LuongNgayThuong,
                            NgayThuong_LaPhanTramLuong = item.NgayThuong_LaPhanTramLuong,
                            Thu7_GiaTri = item.Thu7_GiaTri,
                            Thu7_LaPhanTramLuong = item.Thu7_LaPhanTramLuong,
                            ThCN_GiaTri = item.ThCN_GiaTri,
                            CN_LaPhanTramLuong = item.CN_LaPhanTramLuong,
                            NgayNghi_GiaTri = item.NgayNghi_GiaTri,
                            NgayNghi_LaPhanTramLuong = item.NgayNghi_LaPhanTramLuong,
                            NgayLe_GiaTri = item.NgayLe_GiaTri,
                            NgayLe_LaPhanTramLuong = item.NgayLe_LaPhanTramLuong,
                            LaOT = item.LaOT,
                        };
                        lstAdd.Add(ct);
                    }
                    db.NS_ThietLapLuongChiTiet.AddRange(lstAdd);
                }
                db.SaveChanges();

                var _NhanSuService = new NhanSuService(db);
                if (model.LoaiLuong < 5)
                {
                    _NhanSuService.UpdateCong_WhenChangeThietLapLuong(model.ID, model.ID_NhanVien, model.ID_DonVi ?? Guid.Empty);
                }
                _NhanSuService.UpdateStatusBangLuong_whenChangeCong(model.ID_DonVi ?? Guid.Empty, DateTime.Now);

                var inforNew = db.Database.SqlQuery<string>(" SELECT dbo.Diary_LuongPhuCap('" + model.ID + "')").First();
                result.Data = string.Concat(" <br/ > <b> Thông tin mới: </b>", inforNew, " <br /> <b> Thông tin cũ: </b > ", inforOld);
                result.ErrorCode = false;
            }
            return result;
        }

        public JsonViewModel<string> InsertNvLoaiLuong(NS_LoaiLuong model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };

            if (db.NS_LoaiLuong.Any(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa && o.TenLoaiLuong.ToUpper().Equals(model.TenLoaiLuong.ToUpper())))
            {
                result.Data = "Tên loại lương bị trùng";
            }
            else
            {

                history.NoiDung = "Thêm mới thông tin loại lương có tên: " + model.TenLoaiLuong;
                history.NoiDungChiTiet = "Thông tin loại lương được thêm mới:</br>" +
                                        "Loại lương: " + model.TenLoaiLuong +
                                         "Ghi chú: " + model.GhiChu;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);

                db.NS_LoaiLuong.Add(model);
                db.SaveChanges();
                result.ErrorCode = true;
            }

            return result;
        }
        public JsonViewModel<string> UpdateNvLoaiLuong(NS_LoaiLuong model, HT_NhatKySuDung history)
        {

            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_LoaiLuong.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi loại lương không tồn tại hoặc đã bị xóa";
            }
            else if (db.NS_LoaiLuong.Any(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa && o.TenLoaiLuong.Equals(model.TenLoaiLuong)
                                                && o.ID != model.ID))
            {
                result.Data = "Tên loại lương bị trùng";
            }
            else
            {
                history.NoiDung = "Cập nhật thông tin loại lương: " + data.TenLoaiLuong;
                history.NoiDungChiTiet = "Thông tin loại lương được cập nhật:</br>" +
                                        "Loại lương: " + data.TenLoaiLuong + " => " + model.TenLoaiLuong +
                                         "Ghi chú: " + model.GhiChu;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                db.HT_NhatKySuDung.Add(history);
                data.TenLoaiLuong = model.TenLoaiLuong;
                data.GhiChu = model.GhiChu;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }

        public void InsertNvBaoHiem(NS_BaoHiem model, HT_NhatKySuDung history)
        {

            db.NS_BaoHiem.Add(model);
            var nguoidung = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
            if (nguoidung != null)
            {
                var loaibaohiem = commonEnumHellper.ListLoaiBaoHiem.FirstOrDefault(o => o.Key == model.LoaiBaoHiem);
                history.NoiDung = "Thêm mới thông tin bảo hiểm nhân viên có Mã: " + nguoidung.MaNhanVien + " - Tên: " + nguoidung.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin bảo hiểm nhân viên được thêm mới:</br>" +
                                          "Sổ bảo hiểm: " + model.SoBaoHiem +
                                        "Loại lương: " + loaibaohiem.Value +
                                         "Nơi bảo hiểm: " + model.NoiBaoHiem +
                                        "<br /> Ngày cấp : " + model.NgayCap.ToString("dd/MM/yyyy") + "Ngày hết hạn: " + model.NgayHetHan.Value.ToString("dd/MM/yyyy");
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);
            }
            db.SaveChanges();

        }
        public JsonViewModel<string> UpdateNvBaoHiem(NS_BaoHiem model, HT_NhatKySuDung history)
        {

            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_BaoHiem.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi sổ bảo hiểm không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (data.NS_NhanVien != null)
                {
                    var loaibaohiemold = commonEnumHellper.ListLoaiBaoHiem.FirstOrDefault(o => o.Key == data.LoaiBaoHiem);
                    var loaibaohiem = commonEnumHellper.ListLoaiBaoHiem.FirstOrDefault(o => o.Key == model.LoaiBaoHiem);
                    history.NoiDung = "Cập nhật thông tin bảo hiểm nhân viên có Mã: " + data.NS_NhanVien.MaNhanVien + " - Tên: " + data.NS_NhanVien.TenNhanVien;
                    history.NoiDungChiTiet = "Thông tin bảo hiểm nhân viên được cập nhật lại:</br>" +
                                             "Sổ bảo hiểm: " + data.SoBaoHiem + " => " + model.SoBaoHiem +
                                            "Loại lương: " + loaibaohiemold.Value + " => " + loaibaohiem.Value +
                                             "Nơi bảo hiểm: " + data.NoiBaoHiem + " => " + model.NoiBaoHiem +
                                            "<br /> Ngày cấp : " + model.NgayCap.ToString("dd/MM/yyyy") + "Ngày hết hạn: " + model.NgayHetHan.Value.ToString("dd/MM/yyyy");
                    history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                    db.HT_NhatKySuDung.Add(history);
                }
                data.GhiChu = model.GhiChu;
                data.LoaiBaoHiem = model.LoaiBaoHiem;
                data.NgayCap = model.NgayCap;
                data.NgayHetHan = model.NgayHetHan;
                data.NoiBaoHiem = model.NoiBaoHiem;
                data.SoBaoHiem = model.SoBaoHiem;
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }

        public void DeleteNvTTSucKhoe(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_NhanVien_SucKhoe.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa thông tin khám sức khỏe ngày:" + model.NgayKham.ToString("dd/MM/yyyy")
                                + " của nhân viên có Mã: " + model.NS_NhanVien.MaNhanVien
                                + " - Tên: " + model.NS_NhanVien.TenNhanVien;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvQTCongTac(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_NhanVien_CongTac.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa quá trình công tác từ ngày: " + model.TuNgay.ToString("dd/MM/yyyy")
                                + (model.DenNgay != null ? "- Đến ngày: " + model.DenNgay.Value.ToString("dd/MM/yyyy") : string.Empty)
                               + " của nhân viên có Mã: " + model.NS_NhanVien.MaNhanVien
                               + " - Tên: " + model.NS_NhanVien.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin vừa bị xóa <br/>"
                                        + "Cơ quan: " + model.CoQuan
                                        + "<br/> Vị trí" + model.ViTri
                                        + "<br/> Từ ngày: " + model.TuNgay.ToString("dd/MM/yyyy")
                                        + (model.DenNgay != null ? "<br/> Đến ngày: " + model.TuNgay.ToString("dd/MM/yyyy") : string.Empty);

                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvQTDaoTao(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_NhanVien_DaoTao.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                db.SaveChanges();
            }

        }

        public void DeleteNvGiaDinh(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_NhanVien_GiaDinh.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa thông tin gia đình với tên: " + model.HoTen
                              + " của nhân viên có Mã: " + model.NS_NhanVien.MaNhanVien
                              + " - Tên: " + model.NS_NhanVien.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin vừa bị xóa <br/>"
                                        + "Họ tên: " + model.HoTen
                                        + "<br/> Quan hệ" + model.QuanHe
                                        + "<br/> Địa chỉ: " + model.DiaChi;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvHopDong(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_HopDong.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa thông tin hợp đồng số: " + model.SoHopDong
                            + " của nhân viên có Mã: " + model.NS_NhanVien.MaNhanVien
                            + " - Tên: " + model.NS_NhanVien.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin vừa bị xóa <br/>"
                                        + "Số hợp đồng: " + model.SoHopDong
                                        + "<br/> Ngày ký" + model.NgayKy.ToString("dd/MM/yyyy");
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvBaoHiem(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_BaoHiem.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                var nhanvien = db.NS_NhanVien.FirstOrDefault(o => o.ID == model.ID_NhanVien);
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa thông tin bảo hiểm có sổ BH : " + model.SoBaoHiem
                          + " của nhân viên có Mã: " + nhanvien.MaNhanVien
                          + " - Tên: " + nhanvien.TenNhanVien;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvKhenThuong(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_KhenThuong.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa thông tin khen thưởng có sổ QD : " + model.SoQuyetDinh
                         + " của nhân viên có Mã: " + model.NS_NhanVien.MaNhanVien
                         + " - Tên: " + model.NS_NhanVien.TenNhanVien;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvMienGiamThue(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_MienGiamThue.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {

                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa mục miễn giảm thuế : " + model.KhoanMienGiam
                        + " của nhân viên có Mã: " + model.NS_NhanVien.MaNhanVien
                        + " - Tên: " + model.NS_NhanVien.TenNhanVien;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvLuongPhuCap(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_Luong_PhuCap.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {

                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa lương phụ cấp ngày áp dụng : " + model.NgayApDung.ToString("dd/MM/yyyy")
                    + " với nhân viên có Mã: " + model.NS_NhanVien.MaNhanVien
                    + " - Tên: " + model.NS_NhanVien.TenNhanVien;
                history.NoiDungChiTiet = "Thông tin bị xóa"
                                        + "<br/> Loại lương: " + (commonEnumHellper.ListLoaiLuongPhuCap.Any(o => o.Key == model.LoaiLuong) ? commonEnumHellper.ListLoaiLuongPhuCap.FirstOrDefault(o => o.Key == model.LoaiLuong).Value : string.Empty)
                                        + "<br/> Số tiền: " + CommonStatic.FormatVND(model.SoTien)
                                        + "<br/> Ngày áp dụng " + model.NgayApDung.ToString("dd/MM/yyyy")
                                        + (model.NgayKetThuc != null ? " - Ngày kết thúc: " + model.NgayKetThuc.Value.ToString("dd/MM/yyyy") : string.Empty);
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvLoaiLuong(Guid id, HT_NhatKySuDung history)
        {

            var model = db.NS_LoaiLuong.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                history.NoiDung = "Xóa loại lương : " + model.TenLoaiLuong;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
            }

        }

        public void DeleteNvPhongBan(Guid id, HT_NhatKySuDung history)
        {


            var model = db.NS_PhongBan.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                history.NoiDung = "Xóa phòng ban : " + model.TenPhongBan;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
                db.HT_NhatKySuDung.Add(history);
                db.Database.ExecuteSqlCommand("exec CapNhatPhongBanMacDinhKhiXoa @PhongBanId", new SqlParameter("@PhongBanId", id));

            }

        }

        public JsonViewModel<string> InsertNsPhongban(NS_PhongBan model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };

            if (db.NS_PhongBan.Any(o => o.TenPhongBan.ToUpper().Equals(model.TenPhongBan.ToUpper())
                && o.ID_PhongBanCha == model.ID_PhongBanCha
                && o.ID_DonVi == model.ID_DonVi
                && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa))
            {
                result.Data = "Tên phòng ban đã tồn tại";
            }
            else
            {
                history.NoiDung = "Thêm mới phòng ban : " + model.TenPhongBan;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                db.HT_NhatKySuDung.Add(history);
                var mapb = db.NS_PhongBan.OrderByDescending(o => o.MaPhongBan).Take(1).FirstOrDefault();
                if (mapb != null)
                {
                    var maint = (int.Parse("1" + mapb.MaPhongBan.Substring(2, (mapb.MaPhongBan.Length - 2))) + 1).ToString();
                    var ma = maint.ToString().Substring(1, (maint.Length - 1));
                    model.MaPhongBan = "PB" + ma;
                }
                else
                {
                    model.MaPhongBan = "PB0001";

                }
                db.NS_PhongBan.Add(model);
                db.SaveChanges();
                result.ErrorCode = true;
            }

            return result;
        }

        public JsonViewModel<string> UpdateNsPhongban(NS_PhongBan model, HT_NhatKySuDung history)
        {

            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = db.NS_PhongBan.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bản ghi không tồn tại hoặc đã bị xóa";
            }
            else if (db.NS_PhongBan.Any(o => o.TenPhongBan.ToUpper().Equals(model.TenPhongBan.ToUpper())
               && o.ID_PhongBanCha == model.ID_PhongBanCha
               && o.ID_DonVi == model.ID_DonVi
               && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa
               && o.ID != model.ID))
            {
                result.Data = "Tên phòng ban đã tồn tại";
            }
            else
            {
                if (model.ID_PhongBanCha != null)
                {
                    // gọp vào thằng con - mà thằng đi gộp lại có con >3 cấp
                    if (db.NS_PhongBan.Any(o => o.ID_PhongBanCha == data.ID) && db.NS_PhongBan.Any(o => o.ID == model.ID_PhongBanCha && o.ID_PhongBanCha != null))
                    {
                        result.Data = "Không thể gộp các phòng ban vượt quá 3 cấp";
                        return result;
                    }
                    var listPbcon = db.NS_PhongBan.Where(o => o.ID_PhongBanCha == data.ID).Select(o => o.ID).ToList();
                    if (db.NS_PhongBan.Any(o => o.ID == model.ID_PhongBanCha && o.ID_PhongBanCha == null)
                           && listPbcon.Count > 0
                           && db.NS_PhongBan.Any(o => listPbcon.Contains(o.ID_PhongBanCha ?? new Guid())))
                    {
                        result.Data = "Không thể gộp các phòng ban vượt quá 3 cấp";
                        return result;
                    }
                }
                data.TenPhongBan = model.TenPhongBan;
                data.ID_PhongBanCha = model.ID_PhongBanCha;
                history.NoiDung = "Cập nhật phòng ban : " + data.TenPhongBan;
                history.NoiDungChiTiet = "Thông tin cập nhật : "
                                        + "<br/> Tên phòng ban: " + data.TenPhongBan + " => " + model.TenPhongBan;
                history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                db.HT_NhatKySuDung.Add(history);
                db.SaveChanges();
                result.ErrorCode = true;

            }
            return result;

        }




        public PrintNhanVien GetPrintNhanVien(Guid? nhanvienId, Guid? donviId)
        {

            var data = db.NS_NhanVien.Where(o => o.ID == nhanvienId);
            var model = new PrintNhanVien();
            if (data.Any())
            {
                var chinhanh = db.DM_DonVi.FirstOrDefault(o => o.ID == donviId);

                model.CongTy = new PrintCongTy();
                model.CongTy.TenCongTy = db.HT_CongTy.FirstOrDefault().TenCongTy;
                if (chinhanh != null)
                {
                    model.CongTy.TenChiNhanh = chinhanh.TenDonVi;
                    model.CongTy.SDTChiNhanh = chinhanh.SoDienThoai;
                    model.CongTy.DiaChiChiNhanh = chinhanh.DiaChi;
                }
                model.NhanVien = data.AsEnumerable().Select(o => new
                {
                    o.TenNhanVien,
                    o.NgaySinh,
                    GioiTinh = o.GioiTinh ? "Nam" : "Nữ",
                    o.NoiSinh,
                    TinhTrangHonNhan = commonEnumHellper.ListFamily.Any(c => c.Key.Equals(o.TinhTrangHonNhan)) ? commonEnumHellper.ListFamily.FirstOrDefault(c => c.Key.Equals(o.TinhTrangHonNhan)).Value : string.Empty,
                    o.SoCMND,
                    o.NgayCap,
                    o.NoiCap,
                    o.DienThoaiNhaRieng,
                    o.DienThoaiDiDong,
                    o.Email,
                    DanToc = o.DanTocTonGiao,
                    TonGiao = o.TonGiao,
                    HK_DiaChi = o.NguyenQuan,
                    o.GhiChu,
                    Image = o.NS_NhanVien_Anh.Any() ? o.NS_NhanVien_Anh.FirstOrDefault().URLAnh : "/Content/images/photo.png",
                    HK_QH = o.DM_QuanHuyenHKTT != null ? o.DM_QuanHuyenHKTT.TenQuanHuyen : string.Empty,
                    HK_TT = o.DM_TinhThanhHKTT != null ? o.DM_TinhThanhHKTT.TenTinhThanh : string.Empty,
                    HK_XP = o.DM_XaPhuongHKTT != null ? o.DM_XaPhuongHKTT.TenXaPhuong : string.Empty,
                    NgayVaolam = o.NgayVaoLamViec,
                    PhongBan = o.NS_PhongBan != null ? o.NS_PhongBan.TenPhongBan : string.Empty,
                    QuocTich = o.DM_QuocGia != null ? o.DM_QuocGia.TenQuocGia : string.Empty,
                    TT_DiaChi = o.ThuongTru,
                    TT_QH = o.DM_QuanHuyenTT != null ? o.DM_QuanHuyenTT.TenQuanHuyen : string.Empty,
                    TT_TT = o.DM_TinhThanhTT != null ? o.DM_TinhThanhTT.TenTinhThanh : string.Empty,
                    TT_XP = o.DM_XaPhuongTT != null ? o.DM_XaPhuongTT.TenXaPhuong : string.Empty,
                    DaNghiViec = o.DaNghiViec ? "Đã nghỉ việc" : "Đang làm việc",
                    o.NgayVaoDoan,
                    o.NoiVaoDoan,
                    o.NgayVaoDang,
                    o.NgayVaoDangChinhThuc,
                    o.NgayRoiDang,
                    o.NoiSinhHoatDang,
                    o.GhiChuThongTinChinhTri,
                    o.NgayNhapNgu,
                    o.NgayXuatNgu

                }).First();
                model.BaoHiem = db.NS_BaoHiem.Where(o => o.ID_NhanVien == nhanvienId &&
                                                         o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                                         {
                                                             o.ID,
                                                             TextLoaiBaoHiem = commonEnumHellper.ListLoaiBaoHiem.Any(c => c.Key.Equals(o.LoaiBaoHiem)) ? commonEnumHellper.ListLoaiBaoHiem.FirstOrDefault(c => c.Key.Equals(o.LoaiBaoHiem)).Value : string.Empty,

                                                             o.NoiBaoHiem,
                                                             o.GhiChu,
                                                             o.SoBaoHiem,
                                                             o.NgayCap,
                                                             o.NgayHetHan

                                                         }).AsEnumerable();
                model.HopDong = db.NS_HopDong.Where(o => o.ID_NhanVien == nhanvienId &&
                                                    o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                                    {
                                                        o.ID,
                                                        TextLoaiHopDong = commonEnumHellper.ListLoaiHopDong.Any(c => c.Key.Equals(o.LoaiHopDong)) ? commonEnumHellper.ListLoaiHopDong.FirstOrDefault(c => c.Key.Equals(o.LoaiHopDong)).Value : string.Empty,

                                                        o.NgayKy,
                                                        o.SoHopDong,
                                                        ThoiHanText = ConvertThoiHan(o.LoaiHopDong, o.ThoiHan, o.DonViThoiHan),
                                                        o.GhiChu

                                                    }).AsEnumerable();
                model.SucKhoe = db.NS_NhanVien_SucKhoe.Where(o => o.ID_NhanVien == nhanvienId &&
                                                    o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                                    {
                                                        o.ID,
                                                        o.CanNang,
                                                        o.ChieuCao,
                                                        o.NgayKham,
                                                        o.TinhHinhSucKhoe,

                                                    }).AsEnumerable();
                model.CongTac = db.NS_NhanVien_CongTac.Where(o => o.ID_NhanVien == nhanvienId &&
                                                    o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                                    {
                                                        o.ID,
                                                        o.CoQuan,
                                                        o.TuNgay,
                                                        o.DenNgay,
                                                        o.DiaChi,
                                                        o.ViTri,

                                                    }).AsEnumerable();
                model.DaoTao = db.NS_NhanVien_DaoTao.Where(o => o.ID_NhanVien == nhanvienId &&
                                                  o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                                  {
                                                      o.ID,
                                                      o.NoiHoc,
                                                      o.TuNgay,
                                                      o.DenNgay,
                                                      o.NganhHoc,
                                                      o.HeDaoTao,
                                                      o.BangCap

                                                  }).AsEnumerable();
                model.Thue = db.NS_MienGiamThue.Where(o => o.ID_NhanVien == nhanvienId &&
                                                 o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                                 {
                                                     o.ID,
                                                     o.GhiChu,
                                                     o.KhoanMienGiam,
                                                     o.NgayApDung,
                                                     o.NgayKetThuc,
                                                     o.SoTien,

                                                 }).AsEnumerable();
                model.KhenThuong = db.NS_KhenThuong.Where(o => o.ID_NhanVien == nhanvienId &&
                                                o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                                {
                                                    o.ID,
                                                    o.GhiChu,
                                                    o.HinhThuc,
                                                    o.NgayBanHang,
                                                    o.NoiDung,
                                                    o.SoQuyetDinh,

                                                }).AsEnumerable();
                model.Luong = db.NS_Luong_PhuCap.Where(o => o.ID_NhanVien == nhanvienId &&
                                                o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                                {
                                                    o.ID,
                                                    o.SoTien,
                                                    o.HeSo,
                                                    o.Bac,
                                                    o.NoiDung,
                                                    o.NgayApDung,
                                                    o.NgayKetThuc,
                                                    TenLoaiLuong = commonEnumHellper.ListLoaiLuongPhuCap.Any(c => c.Key == o.LoaiLuong) ? commonEnumHellper.ListLoaiLuongPhuCap.FirstOrDefault(c => c.Key == o.LoaiLuong).Value : string.Empty,

                                                }).AsEnumerable();
                model.GiaDinh = db.NS_NhanVien_GiaDinh.Where(o => o.ID_NhanVien == nhanvienId &&
                                               o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                                               {
                                                   o.ID,
                                                   o.HoTen,
                                                   NgaySinh = convertDate(o.NgaySinh.ToString()),
                                                   o.NoiO,
                                                   o.QuanHe,
                                                   o.DiaChi
                                               }).AsEnumerable();
            }
            return model;
        }
        public string ConvertThoiHan(int loai, int thoihan, int donvi)
        {
            if (loai == (int)commonEnumHellper.TypeLoaiHopDong.khongxacdinh)
                return string.Empty;
            if (donvi == (int)commonEnumHellper.TypeThoiHan.thang)
            {
                return thoihan + " tháng";
            }
            else if (donvi == (int)commonEnumHellper.TypeThoiHan.nam)
            {
                return thoihan + " năm";
            }
            return string.Empty;
        }
        private string convertDate(string input)
        {
            if (input.Length == 4)
            {
                return input.ToString();
            }
            else if (input.Length == 6)
            {
                return input.Substring(4, 2) + "/" + input.Substring(0, 4);
            }
            else if (input.Length == 8)
            {
                return input.Substring(6, 2) + "/" + input.Substring(4, 2) + "/" + input.Substring(0, 4);
            }
            return string.Empty;
        }
        public class DM_DonViNV
        {
            public Guid ID { get; set; }
            public string MaDonVi { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChi { get; set; }
            public string SoDienThoai { get; set; }
        }
        public class HD_NhanVien
        {
            public Guid ID { get; set; }
            public Guid ID_NhanVien { get; set; }
            public string MaHoaDon { get; set; }
            public string TenKhachHang { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenLoaiHoaDon { get; set; }
            public double TongTienHang { get; set; }
        }
        public List<NS_NhanVienDTO> GetDTO(Guid idDonVi)
        {

            List<NS_NhanVienDTO> lst = new List<NS_NhanVienDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {

                var data = from nv in db.NS_NhanVien
                           join qtct in db.NS_QuaTrinhCongTac on nv.ID equals qtct.ID_NhanVien
                           where qtct.ID_DonVi == idDonVi && nv.DaNghiViec != true
                           select new
                           {
                               ID = nv.ID,
                               TenNhanVien = nv.TenNhanVien,
                               MaNhanVien = nv.MaNhanVien,
                               DienThoaiDiDong = nv.DienThoaiDiDong
                           };
                foreach (var item in data)
                {
                    NS_NhanVienDTO dto = new NS_NhanVienDTO
                    {
                        ID = item.ID,
                        TenNhanVien = item.TenNhanVien,
                        MaNhanVien = item.MaNhanVien,
                        DienThoaiDiDong = item.DienThoaiDiDong
                    };
                    lst.Add(dto);
                }
                if (lst != null && lst.Count() > 0)
                {
                    return lst;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Get infor Nhan Vien and qua trinh cong tac
        /// </summary>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        public List<NS_NhanVienDTO> SP_GetStaffWorking(Guid idDonVi)
        {

            List<NS_NhanVienDTO> lst = new List<NS_NhanVienDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var idDonViSeach = string.Empty;
                    if (idDonVi == Guid.Empty)
                    {
                        idDonViSeach = "%%";
                    }
                    else
                    {
                        idDonViSeach = "%" + idDonVi + "%";
                    }
                    SqlParameter param = new SqlParameter("ID_DonVi", idDonViSeach);
                    return db.Database.SqlQuery<NS_NhanVienDTO>("EXEC GetInForStaff_Working_byChiNhanh @ID_DonVi", param).ToList();
                }
                catch (Exception e)
                {
                    CookieStore.WriteLog("SP_GetStaffWorking " + e.InnerException + e.Message);
                    return null;
                }
            }

        }

        public List<NS_NhanVienDTO> SP_GetStaffWorkingND(Guid idDonVi)
        {

            List<NS_NhanVienDTO> lst = new List<NS_NhanVienDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    SqlParameter param = new SqlParameter("ID_DonVi", idDonVi);
                    return db.Database.SqlQuery<NS_NhanVienDTO>("EXEC GetInForStaff_Working_byChiNhanhDaTaoND @ID_DonVi", param).ToList();
                }
                catch (Exception e)
                {
                    CookieStore.WriteLog("SP_GetStaffWorkingND " + e.InnerException + e.Message);
                    return null;
                }
            }

        }

        public NS_NhanVien Get(Expression<Func<NS_NhanVien, bool>> query)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                return db.NS_NhanVien.Where(query).FirstOrDefault();
            }

        }

        public bool NS_NhanVienExists(Guid id)
        {


            if (db == null)
            {
                return false;
            }
            else
            {

                return db.NS_NhanVien.Count(e => e.ID == id) > 0;
            }
        }

        #endregion

        #region insert
        public string Add_NhanVien(NS_NhanVien objAdd)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.NS_NhanVien.Add(objAdd);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in dbEx.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), dbEx);
                }
            }
            return strErr;

        }
        public string Add_QuaTrinhCongTac(NS_QuaTrinhCongTac objAdd)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.NS_QuaTrinhCongTac.Add(objAdd);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in dbEx.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), dbEx);
                }
            }
            return strErr;
        }

        public string Insert_ChietKhauMacDinh(ChietKhauMacDinh_NhanVien objAdd)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    objAdd.ID = Guid.NewGuid();
                    db.ChietKhauMacDinh_NhanVien.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = String.Concat("Insert_ChietKhauMacDinh ", ex.Message);
                }
            }
            return strErr;
        }
        #endregion

        public string GetMaNhanVien()
        {
            string format = "{0:0000}";

            string manhanvien = "NV0";
            string madv = db.NS_NhanVien.Where(p => p.MaNhanVien.Contains(manhanvien)).Where(p => p.MaNhanVien.Length == 7).OrderByDescending(p => p.MaNhanVien).Select(p => p.MaNhanVien).FirstOrDefault();
            if (madv == null)
            {
                manhanvien = manhanvien + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(manhanvien.Length, 4)) + 1;
                manhanvien = manhanvien + string.Format(format, tempstt);
            }
            return manhanvien;
        }


        public bool CheckLoaiMaNhanVien(string input)
        {

            if (!string.IsNullOrWhiteSpace(input))
            {
                input = input.ToUpper();

                if (input.StartsWith("NV"))
                {
                    var result = input.Substring(2, (input.Length - 2));
                    int i;
                    return int.TryParse(result, out i);
                }
                else
                {
                    return false;
                }

            }
            return true;
        }

        #region update
        public string Update_NhanVien(NS_NhanVien obj)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    #region NS_NhanVien
                    NS_NhanVien objUpd = db.NS_NhanVien.Find(obj.ID);
                    string sMaNhanVien = string.Empty;
                    if (obj.MaNhanVien == null)
                    {
                        sMaNhanVien = GetMaNhanVien();
                    }
                    else
                    {
                        sMaNhanVien = obj.MaNhanVien;
                    }
                    objUpd.ID = obj.ID;
                    objUpd.MaNhanVien = sMaNhanVien;
                    objUpd.TenNhanVien = obj.TenNhanVien;
                    objUpd.Email = obj.Email;
                    objUpd.NguyenQuan = obj.NguyenQuan;
                    objUpd.NguoiSua = "ADMIN";
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.ThuongTru = obj.ThuongTru;
                    objUpd.DienThoaiDiDong = obj.DienThoaiDiDong;
                    objUpd.SoBHXH = obj.SoBHXH;
                    objUpd.NgaySinh = obj.NgaySinh;
                    objUpd.SoCMND = obj.SoCMND;
                    objUpd.GioiTinh = obj.GioiTinh;
                    objUpd.GhiChu = obj.GhiChu;
                    objUpd.DaNghiViec = obj.DaNghiViec;
                    #endregion


                    db.Entry(objUpd).State = EntityState.Modified;
                    //
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;

        }

        /// <summary>
        /// update table ChietKhauMacDinh_NhanVien (pass parameter is SQL statement)
        /// </summary>
        /// <param name="stringSQL"></param>
        /// <returns></returns>
        public string Update_ChietKhauNhanVien(string stringSQL)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    // assign SQL here (security)
                    stringSQL = " UPDATE ChietKhauMacDinh_NhanVien SET " + stringSQL;
                    SqlParameter param = new SqlParameter("stringSQL", stringSQL);
                    db.Database.ExecuteSqlCommand("SP_UpdateChietKhauNhanVien_StringSQL @stringSQL", param);
                }
                catch (Exception ex)
                {
                    strErr = string.Concat("Update_ChietKhauNhanVien ", ex.Message, ex.InnerException);
                }
            }
            return strErr;

        }

        #endregion

        #region delete
        public string Delete_QuaTrinhCongTac(Guid ID_NhanVien)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                List<NS_QuaTrinhCongTac> objDel = db.NS_QuaTrinhCongTac.Where(x => x.ID_NhanVien == ID_NhanVien).ToList();
                if (objDel != null)
                {
                    try
                    {
                        db.NS_QuaTrinhCongTac.RemoveRange(db.NS_QuaTrinhCongTac.Where(idHD => idHD.ID_NhanVien == ID_NhanVien));
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        strErr = exxx.Message;
                        return strErr;
                    }
                }
                else
                {
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }

        public string Delete_NhanVien(Guid id)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                NS_NhanVien objDel = db.NS_NhanVien.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        objDel.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
                        // update trangthai nguoidung of nvien
                        db.HT_NguoiDung.Where(x => x.ID_NhanVien == id).ToList().ForEach(x => x.DangHoatDong = false);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        strErr = exxx.Message;
                        return strErr;
                    }
                }
                else
                {
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }

        #endregion
        #region Function Other
        #endregion
    }
    public class NS_NhanVienBK
    {
        public Guid ID { get; set; }
        public string TenNhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string DienThoaiDiDong { get; set; }
        public string Email { get; set; }
        public string ThuongTru { get; set; }
        public string NguyenQuan { get; set; }
        public string SoBHXH { get; set; }
        public string SoCMND { get; set; }
        public string GhiChu { get; set; }
        public DateTime? NgaySinh { get; set; }
        public bool GioiTinh { get; set; }
        public bool DaNghiViec { get; set; }
        public string Image { get; set; }
    }
    public class NS_NhanVienDTO
    {
        public Guid ID { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string TenNhanVienKhongDau { get; set; }
        public string TenNhanVienChuCaiDau { get; set; }
        public string NameFull { get; set; }
        public string DienThoaiDiDong { get; set; }
        public string Email { get; set; }
        public string ThuongTru { get; set; }
        public string NguyenQuan { get; set; }
        public string SoBHXH { get; set; }
        public string SoCMND { get; set; }
        public string GhiChu { get; set; }
        public DateTime? NgaySinh { get; set; }
        public bool? GioiTinh { get; set; }
        public bool DaNghiViec { get; set; }
        public string Image { get; set; }
        public Guid? ID_NguoiDung { get; set; }
        public Guid? ID_DonVi { get; set; }
        public string TaiKhoan { get; set; }
    }
    public class NS_ReportNhanVien
    {
        public Guid ID { get; set; }
        public string TenNhanVien { get; set; }
        public string TenNhanVien_KhongDau { get; set; }
        public string TenNhanVien_ChuCaiDau { get; set; }
        public string MaNhanVien { get; set; }
        public string DienThoaiDiDong { get; set; }
        public bool GioiTinh { get; set; }
        public string SoBHXH { get; set; }
        public string SoCMND { get; set; }
        public string SoDienThoai { get; set; }
        public string GhiChu { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string ThuongTru { get; set; }
        public string NguyenQuan { get; set; }
        public string Email { get; set; }
        public bool DaNghiViec { get; set; }
        public string Image { get; set; }
    }
    public class NS_NhanVienPROC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string NguyenQuan { get; set; }
        public string DienThoaiDiDong { get; set; }
        public string Email { get; set; }
        public string ThuongTru { get; set; }
        public string SoCMND { get; set; }
        public string SoBHXH { get; set; }
        public string GhiChu { get; set; }
        public string DaNghiViec { get; set; }
    }
    public class NS_NhanVien_new : NS_ReportNhanVien
    {

        public DateTime? NgayVaoDangChinhThuc { get; set; }
        public DateTime? NgayVaoDoan { get; set; }
        public DateTime? NgayRoiDang { get; set; }
        public DateTime? NgayVaoDang { get; set; }
        public DateTime? NgayNhapNgu { get; set; }
        public DateTime? NgayXuatNgu { get; set; }
        public string GhiChuThongTinChinhTri { get; set; }
        public string NoiVaoDoan { get; set; }
        public string NoiSinhHoatDang { get; set; }
        public string TinhTrangHonNhan { get; set; }
        public int? HonNhan { get; set; }
        public string NoiSinh { get; set; }
        public string CMND { get; set; }
        public DateTime? CMND_NgayCap { get; set; }
        public string CMND_NoiCap { get; set; }
        public string DanToc { get; set; }
        public string TonGiao { get; set; }
        public string QuocTich { get; set; }
        public string HK_TT { get; set; }
        public string HK_QH { get; set; }
        public string HK_XP { get; set; }
        public string HK_DiaChi { get; set; }
        public string TT_TT { get; set; }
        public string TT_QH { get; set; }
        public string TT_XP { get; set; }
        public string TT_DiaChi { get; set; }
        public string PhongBan { get; set; }
        public DateTime? NgayVaolam { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? ID_NguoiDung { get; set; }
    }
    public class PrintNhanVien
    {
        public PrintCongTy CongTy { get; set; }
        public object NhanVien { get; set; }
        public object HopDong { get; set; }
        public object BaoHiem { get; set; }
        public object KhenThuong { get; set; }
        public object Luong { get; set; }
        public object DaoTao { get; set; }
        public object CongTac { get; set; }
        public object GiaDinh { get; set; }
        public object SucKhoe { get; set; }
        public object Thue { get; set; }
    }

    public class ExportNhanVien
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string NgaySinh { get; set; }
        public bool? GioiTinh { get; set; }
        public string GioiTinhText
        {
            get
            {
                if (GioiTinh == true)
                    return "Nam";
                else
                    return "Nữ";
            }
        }
        public string NoiSinh { get; set; }
        public string DienThoaiDiDong { get; set; }
        public string Email { get; set; }
        public string SoCMND { get; set; }
        public string NgayCap { get; set; }
        public string NoiCap { get; set; }
        public string DanTocTonGiao { get; set; }
        public string TonGiao { get; set; }
        public int? TinhTrangHonNhan { get; set; }
        public string TinhTrangHonNhanText
        {
            get
            {
                if (TinhTrangHonNhan == (int)commonEnumHellper.TypeIsFamily.docthan)
                {
                    return "Độc thân";
                }
                else if (TinhTrangHonNhan == (int)commonEnumHellper.TypeIsFamily.cogiadinh)
                {
                    return "Có gia đình";
                }
                else
                {
                    return "";
                }
            }
        }
        public string TenQuocGia { get; set; }
        public string DiaChiHKTT { get; set; }
        public string HK_TenXaPhuong { get; set; }
        public string HK_TenQuanHuyen { get; set; }
        public string HK_TenTinhThanh { get; set; }
        public string DiaChiTT { get; set; }
        public string TT_TenXaPhuong { get; set; }
        public string TT_TenQuanHuyen { get; set; }
        public string TT_TenTinhThanh { get; set; }
        public string TenPhongBan { get; set; }
        public string NgayVaoLamViec { get; set; }
        public bool? DaNghiViec { get; set; }
        public string DaNghiViecText
        {
            get
            {
                if (DaNghiViec == true)
                {
                    return "Đã nghỉ việc";
                }
                else if (DaNghiViec == false)
                {
                    return "Đang làm việc";
                }
                else
                {
                    return "";
                }
            }
        }
        public string NgayVaoDoan { get; set; }
        public string NoiVaoDoan { get; set; }
        public string NgayNhapNgu { get; set; }
        public string NgayXuatNgu { get; set; }
        public string NgayVaoDang { get; set; }
        public string NgayVaoDangChinhThuc { get; set; }
        public string NgayRoiDang { get; set; }
        public string NoiSinhHoatDang { get; set; }
        public string GhiChuThongTinChinhTri { get; set; }
    }

    public class PrintCongTy
    {
        public string TenCongTy { get; set; }
        public string TenChiNhanh { get; set; }
        public string DiaChiChiNhanh { get; set; }
        public string SDTChiNhanh { get; set; }
    }
    public class NS_PhongBan_PROC
    {
        public double STT { get; set; }
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public Guid? ID_PhongBan { get; set; }
    }
    public class NS_NhanVien_DonVi
    {
        public Guid ID { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string TenNhanVien_GC { get; set; }
        public string TenNhanVien_CV { get; set; }
        public Guid? ID_PhongBan { get; set; }
    }
    public class NS_QuaTrinhCongTac_PRC
    {
        public double ID { get; set; }
        public Guid ID_ChiNhanh { get; set; }
        public Guid? ID_PhongBan { get; set; }
        public bool LaMacDinh { get; set; }
        public string Text_ChiNhanh { get; set; }
        public string Text_PhongBan { get; set; }

        public object listPhongBan { get; set; }
    }
    public class NhanVienFilter
    {
        public string Text { get; set; }

        public Guid? DonViId { get; set; }

        public Guid? PhongBanId { get; set; }

        public bool? GioiTinh { get; set; }

        public List<int> LoaiHopDong { get; set; }

        public List<int> BaoHiem { get; set; }

        public List<string> DanToc { get; set; }

        public List<int> ChinhTri { get; set; }

        public Guid? HK_TT { get; set; }

        public Guid? HK_QH { get; set; }

        public Guid? HK_XP { get; set; }

        public Guid? TT_TT { get; set; }

        public Guid? TT_QH { get; set; }

        public Guid? TT_XP { get; set; }

        public int? TypeTime { get; set; }

        public DateTime? TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }

        public int? TrangThai { get; set; }

        public int pageSize { get; set; }

        public int pageNum { get; set; }

    }
    public class NhanVienFilterExport : NhanVienFilter
    {
        public List<int> ColumnHiden { get; set; }
    }

    public class SP_ChietKhauNV
    {
        public Guid ID { get; set; } // ID_NhanVien
        public Guid ID_NhanVien { get; set; } // ID_NhanVien
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string DienThoaiDiDong { get; set; }
        public double ChietKhau { get; set; }
        public bool LaPhanTram { get; set; }
        public double ChietKhau_TuVan { get; set; }
        public bool LaPhanTram_TuVan { get; set; }
        public double? ChietKhau_BanGoi { get; set; }
        public bool? LaPhanTram_BanGoi { get; set; }
        public double? ChietKhau_YeuCau { get; set; }
        public bool? LaPhanTram_YeuCau { get; set; }
        public int? TheoChietKhau_ThucHien { get; set; }
    }
}
