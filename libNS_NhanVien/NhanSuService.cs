using Model;
using Model.Infrastructure;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libNS_NhanVien
{
    public class NhanSuService
    {
        private SsoftvnContext _dbcontext;
        public NhanSuService(SsoftvnContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IQueryable<NS_CaLamViec> QueryCaLamViec()
        {
            return _dbcontext.NS_CaLamViec.OrderByDescending(o => o.NgayTao).AsQueryable();
        }

        public List<NS_NhanVienDTO> GetNhanVien_CoBangLuong(DateTime fromDate, DateTime toDate, Guid idChiNhanh)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            paramlist.Add(new SqlParameter("FromDate", fromDate));
            paramlist.Add(new SqlParameter("ToDate", toDate));
            return _dbcontext.Database.SqlQuery<NS_NhanVienDTO>("exec GetNhanVien_CoBangLuong @ID_ChiNhanh, @FromDate, @ToDate", paramlist.ToArray()).ToList();
        }

        public void SaoChepThietLapLuong(SaoChep_ThietLapLuong lstParam)
        {
            var idNhanViens = string.Join(",", lstParam.LstIDNhanVien);
            var kieuluongs = string.Join(",", lstParam.LstKieuLuong);
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("ID_ChiNhanh", lstParam.ID_ChiNhanh));
            paramlist.Add(new SqlParameter("ID_NhanVien", lstParam.ID_NhanVien));
            paramlist.Add(new SqlParameter("KieuLuongs", kieuluongs));
            paramlist.Add(new SqlParameter("IDNhanViens", idNhanViens));
            paramlist.Add(new SqlParameter("UpdateNVSetup", lstParam.UpdateNVSetup));
            paramlist.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            _dbcontext.Database.SqlQuery<NS_NhanVienDTO>("exec SaoChepThietLapLuong @ID_ChiNhanh, @ID_NhanVien, @KieuLuongs, @IDNhanViens," +
                " @UpdateNVSetup, @ID_NhanVienLogin", paramlist.ToArray()).ToList();
        }

        // get all -- allow update from not active --> active
        public IQueryable<NS_CaLamViec> GetListCatheoChiNhanh(Guid chinhanhid)
        {
            return from ca in _dbcontext.NS_CaLamViec
                   join cadv in _dbcontext.NS_CaLamViec_DonVi
                   on ca.ID equals cadv.ID_CaLamViec
                   where cadv.ID_DonVi == chinhanhid //&& ca.TrangThai == (int)commonEnumHellper.TrangThaiCaLamViec.dangapdung
                   orderby ca.NgayTao descending
                   select ca;
        }

        public IQueryable<NS_CaLamViec> GetCaLamViecFilter(SearchFilter model)
        {
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);

            var data = QueryCaLamViec().Where(o => o.TrangThai != (int)commonEnumHellper.TrangThaiCaLamViec.xoa);
            if (model.ListDonVi != null && model.ListDonVi.Count > 0)
            {
                data = (from dv in _dbcontext.NS_CaLamViec_DonVi
                        join ca in _dbcontext.NS_CaLamViec on dv.ID_CaLamViec equals ca.ID
                        where model.ListDonVi.Contains(dv.ID_DonVi)
                        select ca).Distinct();
            }
            else
            {
                data = (from ca in _dbcontext.NS_CaLamViec
                        join dv in _dbcontext.NS_CaLamViec_DonVi on ca.ID equals dv.ID_CaLamViec
                        join nvdv in _dbcontext.NS_QuaTrinhCongTac on dv.ID_DonVi equals nvdv.ID_DonVi
                        where nvdv.ID_NhanVien == model.IDNhanVien
                        select ca).Distinct();
            }
            if (!string.IsNullOrWhiteSpace(model.Text))
            {
                data = data.Where(o => o.TenCa.ToUpper().Contains(model.Text)
                                       || o.CaLamViec_ChuCaiDau.Contains(model.Text)
                                       || o.CaLamViec_KhongDau.Contains(model.Text)
                                       || o.MaCa.Contains(model.Text)
                                    );
            }
            if (IsfilterTime)
            {
                data = data.Where(x =>
                 x.NgayTao >= startday
                && x.NgayTao <= Endday);
            }
            if (model.TrangThai != null && model.TrangThai.Count > 0)
            {
                data = data.Where(x => model.TrangThai.Contains(x.TrangThai));
            }

            return data.OrderByDescending(o => o.NgayTao);
        }

        public void InsertCaLamViec(NS_CaLamViec model, Guid idnhanvien, List<Guid> listdonvi)
        {
            var nguoitao = GetNguoiTao(idnhanvien);
            model.NguoiTao = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            model.SoGioOTToiThieu = Math.Round(model.SoGioOTToiThieu, 1);
            model.TongGioCong = Math.Round(model.TongGioCong, 1);
            _dbcontext.NS_CaLamViec.Add(model);
            if (listdonvi != null)
            {
                var modeldonvica = listdonvi.Select(o => new NS_CaLamViec_DonVi
                {
                    ID = Guid.NewGuid(),
                    ID_CaLamViec = model.ID,
                    ID_DonVi = o
                });
                _dbcontext.NS_CaLamViec_DonVi.AddRange(modeldonvica);
            }
            _dbcontext.SaveChanges();
        }

        public bool DeleteCaLamViec(NS_CaLamViec model, Guid idnhanvien, Guid iddonvi)
        {
            var data = _dbcontext.NS_CaLamViec.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                return false;
            }
            data.TrangThai = (int)commonEnumHellper.TrangThaiCaLamViec.xoa;
            HT_NhatKySuDung nhatky = new HT_NhatKySuDung
            {
                ID = Guid.NewGuid(),
                ID_NhanVien = idnhanvien,
                ID_DonVi = iddonvi,
                ChucNang = "Danh mục ca làm việc",
                ThoiGian = DateTime.Now,
                NoiDung = "Xóa ca làm việc - Mã ca: " + model.MaCa,
                NoiDungChiTiet = "Xóa ca làm việc: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" >" + model.MaCa + "</a>",
                LoaiNhatKy = 3
            };
            var listDelete = _dbcontext.NS_CaLamViec_DonVi.Where(o => o.ID_CaLamViec == data.ID).AsEnumerable();
            _dbcontext.NS_CaLamViec_DonVi.RemoveRange(listDelete);
            _dbcontext.HT_NhatKySuDung.Add(nhatky);
            _dbcontext.SaveChanges();
            return true;
        }

        public bool UpdateCaLamViec(NS_CaLamViec model, Guid idnhanvien, Guid iddonvi, List<Guid> listdonvi)
        {
            var data = _dbcontext.NS_CaLamViec.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                return false;
            }
            var trangthai = commonEnumHellper.ListTrangThaiCaLamViec.FirstOrDefault(o => o.Key == data.TrangThai);
            var trangthainew = commonEnumHellper.ListTrangThaiCaLamViec.FirstOrDefault(o => o.Key == model.TrangThai);
            HT_NhatKySuDung nhatky = new HT_NhatKySuDung
            {
                ID = Guid.NewGuid(),
                ID_NhanVien = idnhanvien,
                ID_DonVi = iddonvi,
                ChucNang = "Danh mục ca làm việc",
                ThoiGian = DateTime.Now,
                NoiDung = "Cập nhật ca làm việc - Mã ca: " + model.MaCa,
                NoiDungChiTiet = "Cập nhật ca làm việc: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" >" + model.MaCa + "</a>, Giờ vào: " + model.GioVao + ", giờ ra: " + model.GioRa + ". Trạng thái: " + trangthainew.Value,
                LoaiNhatKy = 2
            };
            _dbcontext.HT_NhatKySuDung.Add(nhatky);

            var nguoitao = GetNguoiTao(idnhanvien);
            var maca = model.MaCa;
            if (string.IsNullOrEmpty(maca))
            {
                maca = GetMaCa();
            }
            data.MaCa = maca;
            data.NguoiSua = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            data.CaLamViec_KhongDau = CommonStatic.RemoveSign4VietnameseString(model.TenCa.Trim()).ToLower();
            data.CaLamViec_ChuCaiDau = CommonStatic.convertchartstart(model.TenCa.Trim()).ToLower();
            data.NgaySua = DateTime.Now;
            data.CachLayGioCong = model.CachLayGioCong;
            data.GhiChuCaLamViec = model.GhiChuCaLamViec;
            data.GioOTBanDemDen = model.GioOTBanDemDen;
            data.GhiChuTinhGio = model.GhiChuTinhGio;
            data.GioOTBanDemTu = model.GioOTBanDemTu;
            data.GioOTBanNgayDen = model.GioOTBanNgayDen;
            data.GioOTBanNgayTu = model.GioOTBanNgayTu;
            data.GioRa = model.GioRa;
            data.GioRaDen = model.GioRaDen;
            data.GioRaTu = model.GioRaTu;
            data.GioVao = model.GioVao;
            data.GioVaoDen = model.GioVaoDen;
            data.GioVaoTu = model.GioVaoTu;
            data.LaCaDem = model.LaCaDem;
            data.NghiGiuaCaDen = model.NghiGiuaCaDen;
            data.NghiGiuaCaTu = model.NghiGiuaCaTu;
            data.SoGioOTToiThieu = Math.Round(model.SoGioOTToiThieu, 1);
            data.SoPhutDiMuon = model.SoPhutDiMuon;
            data.SoPhutVeSom = model.SoPhutVeSom;
            data.TenCa = model.TenCa;
            data.ThoiGianDiMuonVeSom = model.ThoiGianDiMuonVeSom;
            data.TinhOTBanDemDen = model.TinhOTBanDemDen;
            data.TinhOTBanDemTu = model.TinhOTBanDemTu;
            data.TinhOTBanNgayDen = model.TinhOTBanNgayDen;
            data.TinhOTBanNgayTu = model.TinhOTBanNgayTu;
            data.TongGioCong = Math.Round(model.TongGioCong, 1);
            data.TrangThai = model.TrangThai;
            if (listdonvi == null)
            {
                listdonvi = new List<Guid>();
            }
            var listdelete = _dbcontext.NS_CaLamViec_DonVi.Where(o => !listdonvi.Contains(o.ID_DonVi) && o.ID_CaLamViec == data.ID).AsEnumerable();
            var listOld = _dbcontext.NS_CaLamViec_DonVi.Where(o => listdonvi.Contains(o.ID_DonVi) && o.ID_CaLamViec == data.ID).Select(o => o.ID_DonVi).ToList();
            var listInert = listdonvi.Where(o => !listOld.Contains(o)).Select(o => new NS_CaLamViec_DonVi
            {
                ID = Guid.NewGuid(),
                ID_CaLamViec = data.ID,
                ID_DonVi = o
            });
            _dbcontext.NS_CaLamViec_DonVi.RemoveRange(listdelete);
            _dbcontext.NS_CaLamViec_DonVi.AddRange(listInert);
            _dbcontext.SaveChanges();
            return true;
        }

        public string GetMaCa()
        {
            return _dbcontext.Database.SqlQuery<string>("SELECT CONCAT('CA0000', MAX(CAST (dbo.udf_GetNumeric(MaCa) AS float)) + 1) FROM NS_CaLamViec WHERE CHARINDEX('CA', MaCa) > 0").First().ToString();
        }

        public bool CheckExitMaCa(string maca, Guid? id = null)
        {
            if (id == null)
                return _dbcontext.NS_CaLamViec.Any(o => o.MaCa.ToUpper().Equals(maca.ToUpper().Trim()));
            else
                return _dbcontext.NS_CaLamViec.Any(o => o.ID != id && o.MaCa.ToUpper().Equals(maca.ToUpper().Trim()));
        }

        public bool CheckExitTenCa(string tenca, Guid? id = null)
        {
            if (id == null)
                return _dbcontext.NS_CaLamViec.Any(o => o.TenCa.ToUpper().Equals(tenca.ToUpper().Trim()));
            else
                return _dbcontext.NS_CaLamViec.Any(o => o.ID != id && o.TenCa.ToUpper().Equals(tenca.ToUpper().Trim()));
        }

        public void InsertNhatKy(HT_NhatKySuDung model)
        {
            _dbcontext.HT_NhatKySuDung.Add(model);
            _dbcontext.SaveChanges();
        }

        public List<NS_CaLamViecExport> GetExportExcelToCaLamViec(SearchFilter model, ref string time)
        {
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);
            if (IsfilterTime)
            {
                time = "Từ ngày: " + startday.ToString("dd/MM/yyyy") + " - Đến ngày: " + Endday.ToString("dd/MM/yyyy");
            }
            else
            {
                time = "Toàn thời gian";
            }

            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("Text", model.Text ?? string.Empty));
            paramlist.Add(new SqlParameter("NgayBatDau", startday.ToString("dd/MM/yyyy")));
            paramlist.Add(new SqlParameter("NgayKetThuc", Endday.AddDays(1).ToString("dd/MM/yyyy")));
            paramlist.Add(new SqlParameter("ListTrangThai", string.Join(",", model.TrangThai ?? new List<int?>())));
            paramlist.Add(new SqlParameter("TimThoiGian", IsfilterTime == true ? 1 : 0));
            paramlist.Add(new SqlParameter("ListDonvi", string.Join(",", model.ListDonVi ?? new List<Guid>())));
            paramlist.Add(new SqlParameter("ID_NhanVien", model.IDNhanVien));
            var lst = _dbcontext.Database.SqlQuery<NS_CaLamViecExport>("exec GetAllDanhSachCaLamViec @Text,@NgayBatDau,@NgayKetThuc,@ListTrangThai,@TimThoiGian,@ListDonvi,@ID_NhanVien", paramlist.ToArray()).ToList();
            return lst;
        }

        public void ImprtToDataCaLamViec(List<NS_CaLamViec> model, Guid idnhanvien, Guid iddonvi, int count, int error)
        {
            var nguoitao = GetNguoiTao(idnhanvien);
            var nguoitaotext = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            var modelMaNotEmty = model.Where(o => !string.IsNullOrWhiteSpace(o.MaCa));
            var modelMaEmty = model.Where(o => string.IsNullOrWhiteSpace(o.MaCa));
            if (modelMaNotEmty.Any())
            {
                foreach (var item in modelMaNotEmty)
                {
                    item.ID = Guid.NewGuid();
                    item.NguoiTao = nguoitaotext;
                    _dbcontext.NS_CaLamViec.Add(item);
                    _dbcontext.NS_CaLamViec_DonVi.Add(new NS_CaLamViec_DonVi()
                    {
                        ID = Guid.NewGuid(),
                        ID_CaLamViec = item.ID,
                        ID_DonVi = iddonvi
                    });
                }
                _dbcontext.SaveChanges();
            }
            foreach (var item in modelMaEmty)
            {
                item.ID = Guid.NewGuid();
                item.MaCa = GetMaCa();
                item.NguoiTao = nguoitaotext;
                _dbcontext.NS_CaLamViec.Add(item);
                _dbcontext.NS_CaLamViec_DonVi.Add(new NS_CaLamViec_DonVi()
                {
                    ID = Guid.NewGuid(),
                    ID_CaLamViec = item.ID,
                    ID_DonVi = iddonvi
                });
                _dbcontext.SaveChanges();
            }
            HT_NhatKySuDung nhatky = new HT_NhatKySuDung
            {
                ID = Guid.NewGuid(),
                ID_NhanVien = idnhanvien,
                ID_DonVi = iddonvi,
                ChucNang = "Danh mục ca làm việc",
                ThoiGian = DateTime.Now,
                NoiDung = "Import danh sách ca làm viêc: Tổng số " + count +
                           " Thành công: " + model.Count +
                           " Lỗi: " + error,
                NoiDungChiTiet = "",
                LoaiNhatKy = 4
            };
            _dbcontext.HT_NhatKySuDung.Add(nhatky);
            _dbcontext.SaveChanges();
        }

        public IQueryable<DM_DonVi> GetDonViByNhanVien(Guid idnhanvien)
        {
            var data = from ct in _dbcontext.NS_QuaTrinhCongTac
                       join dv in _dbcontext.DM_DonVi on ct.ID_DonVi equals dv.ID
                       where ct.ID_NhanVien == idnhanvien
                       select dv;
            return data.OrderBy(o => o.TenDonVi);
        }

        public string GetListDonViById(List<Guid> listdonvi)
        {
            var data = _dbcontext.DM_DonVi.Where(o => listdonvi.Contains(o.ID)).Select(o => o.TenDonVi).ToList();
            return string.Join(",", data);
        }

        public IQueryable<NS_NhanVien_GiaDinh> getListForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_NhanVien_GiaDinh.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public IQueryable<NS_NhanVien_DaoTao> getListNvQtDaoTaoForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_NhanVien_DaoTao.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public IQueryable<NS_NhanVien_CongTac> getListNvQtCongTacForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_NhanVien_CongTac.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public IQueryable<NS_NhanVien_SucKhoe> getListNvTTSucKhoecForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_NhanVien_SucKhoe.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public IQueryable<NS_HopDong> getListNvHopDongForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_HopDong.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public IQueryable<NS_BaoHiem> getListNvBaoHiemForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_BaoHiem.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public IQueryable<NS_MienGiamThue> getListNvMienGiamForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_MienGiamThue.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public IQueryable<NS_KhenThuong> getListNvKhenThuongForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_KhenThuong.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public IQueryable<NS_Luong_PhuCap> getListNvLuongForByNnId(Guid nhanvienId)
        {
            return _dbcontext.NS_Luong_PhuCap.Where(o => o.ID_NhanVien.Equals(nhanvienId));
        }

        public List<ThietLapLuongDTO> GetThietLapLuong_ofNhanVien(Guid idNhanVien, Guid idChiNhanh, int currentPage, int pageSize)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            sql.Add(new SqlParameter("ID_NhanVien", idNhanVien));
            sql.Add(new SqlParameter("CurrentPage", currentPage));
            sql.Add(new SqlParameter("PageSize", pageSize));
            return _dbcontext.Database.SqlQuery<ThietLapLuongDTO>("EXEC GetThietLapLuong_ofNhanVien @ID_ChiNhanh, @ID_NhanVien, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }

        public IQueryable<NS_PhongBan> getAllNvPhongban()
        {
            return _dbcontext.NS_PhongBan.AsQueryable();
        }

        public List<PhongBanChiNhanhView> GetListPhongBanChiNhanh(Guid? chinhanhid)
        {
            var data = getAllNvPhongban();
            if (chinhanhid != null)
            {
                data = data.Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa && (o.ID_DonVi == chinhanhid || o.ID_DonVi == null));
            }
            var result = data.ToList();
            var json = result.Where(o => o.ID_PhongBanCha == null).Select(o =>
                            new PhongBanChiNhanhView
                            {
                                id = o.ID,
                                parentId = o.ID_PhongBanCha,
                                ID_DonVi = o.ID_DonVi,
                                text = o.TenPhongBan,
                                active = true,
                                children = GetChildren(result, o.ID, o.TenPhongBan)
                            });
            return json.ToList();
        }

        public List<Guid> GetListIDPhongBanByID(Guid? idphongban, Guid iddonvi)
        {
            if (idphongban == null)
            {
                return _dbcontext.NS_PhongBan.Where(p => p.ID_DonVi == iddonvi).Select(p => p.ID).ToList();
            }
            else
            {
                List<Guid> lstIDPhongBanTemp = new List<Guid>();
                List<Guid> lstIDPhongBan = new List<Guid>();
                lstIDPhongBanTemp.Add(idphongban.Value);
                lstIDPhongBan.Add(idphongban.Value);
                List<NS_PhongBan> lstNSPhongBan = _dbcontext.NS_PhongBan.Where(p => p.ID_DonVi == iddonvi).Where(p => p.TrangThai == 1).ToList();
                int flag = 1;
                while (flag == 1)
                {
                    flag = lstNSPhongBan.Where(p => p.ID_PhongBanCha != null).Where(p => lstIDPhongBanTemp.Contains(p.ID_PhongBanCha.Value)).Count();
                    if (flag != 0)
                    {
                        lstIDPhongBanTemp.AddRange(lstNSPhongBan.Where(p => p.ID_PhongBanCha != null).Where(p => lstIDPhongBanTemp.Contains(p.ID_PhongBanCha.Value)).Select(p => p.ID));
                        lstIDPhongBanTemp = lstIDPhongBanTemp.Except(lstIDPhongBan).ToList();
                        lstIDPhongBan.AddRange(lstIDPhongBanTemp);
                    }
                }
                return lstIDPhongBan;
            }
        }

        public List<PhongBanChiNhanhView> GetChildren(List<NS_PhongBan> data, Guid roleKey, string parentText)
        {
            return data.Where(o => o.ID_PhongBanCha != null && o.ID_PhongBanCha.Equals(roleKey)).Select(o =>
                       new PhongBanChiNhanhView
                       {
                           id = o.ID,
                           text = o.TenPhongBan,
                           ID_DonVi = o.ID_DonVi,
                           parentText = parentText,
                           parentId = o.ID_PhongBanCha,
                           active = true,
                           children = GetChildren(data, o.ID, o.TenPhongBan)
                       }).ToList();
        }

        public IQueryable<NS_LoaiLuong> getAllNvLoaiLuong()
        {
            return _dbcontext.NS_LoaiLuong.AsQueryable();
        }

        public IQueryable<NS_ThietLapLuongChiTiet> GetThietLapLuongChiTiet()
        {
            return _dbcontext.NS_ThietLapLuongChiTiet.AsQueryable();
        }

        public List<NV_CaLamViecDTO> GetListCaLamViec_ofDonVi(Guid idDonVi)
        {
            SqlParameter param = new SqlParameter("ID_DonVi", idDonVi);
            var data = _dbcontext.Database.SqlQuery<NV_CaLamViecDTO>("GetListCaLamViec_ofDonVi @ID_DonVi", param).ToList();
            return data;
        }

        public JsonViewModel<Object> GetListNhanVien_HadSetupSalary(Guid idDonVi)
        {
            var result = new JsonViewModel<Object>() { ErrorCode = true };
            var data = (from tl in _dbcontext.NS_Luong_PhuCap
                        join nv in _dbcontext.NS_NhanVien on tl.ID_NhanVien equals nv.ID
                        where tl.ID_DonVi == idDonVi && tl.TrangThai != (int)commonEnumHellper.TrangThaiCaLamViec.xoa
                        select new
                        {
                            nv.ID,
                            nv.MaNhanVien,
                            nv.TenNhanVien,
                            Active = false,
                        }).Distinct().OrderBy(x => x.MaNhanVien).ToList();
            result.Data = data;
            result.ErrorCode = false;
            return result;
        }

        public IQueryable<NS_NhanVien> getListNhanVien_HRM(NhanVienFilter model)
        {
            var listPhongban = new List<Guid>();
            if (model.PhongBanId != null)
            {
                var phongban = _dbcontext.NS_PhongBan.FirstOrDefault(o => o.ID == model.PhongBanId);
                if (phongban != null && phongban.ID_PhongBanCha != null)
                {
                    listPhongban = _dbcontext.NS_PhongBan.Where(o => o.ID_PhongBanCha == phongban.ID || o.ID == phongban.ID).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                }
                else if (phongban != null)
                {
                    listPhongban = _dbcontext.NS_PhongBan.Where(o => o.ID_PhongBanCha == phongban.ID || o.ID == phongban.ID).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                    var listPhongbannew = _dbcontext.NS_PhongBan.Where(o => listPhongban.Contains(o.ID_PhongBanCha ?? new Guid())).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                    listPhongban.AddRange(listPhongbannew);
                    listPhongban = listPhongban.Distinct().ToList();
                }
            }
            var tb_fm = _dbcontext.NS_NhanVien.Where(o => (o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa || o.TrangThai == null)).AsQueryable();

            if (listPhongban.Count > 0)
            {
                tb_fm = (from o in tb_fm
                         join p in _dbcontext.NS_QuaTrinhCongTac.AsQueryable()
                            on o.ID equals p.ID_NhanVien
                         where listPhongban.Contains(p.ID_PhongBan ?? new Guid()) && p.ID_DonVi == model.DonViId
                         select o).Distinct().AsQueryable();
            }
            else
            {
                tb_fm = (from o in tb_fm
                         join p in _dbcontext.NS_QuaTrinhCongTac.AsQueryable()
                            on o.ID equals p.ID_NhanVien
                         where p.ID_DonVi == model.DonViId
                         select o).Distinct().AsQueryable();
            }

            if (model.HK_TT != null)
            {
                tb_fm = tb_fm.Where(o => o.ID_TinhThanhHKTT == model.HK_TT).AsQueryable();
                if (model.HK_QH != null)
                {
                    tb_fm = tb_fm.Where(o => o.ID_QuanHuyenHKTT == model.HK_QH).AsQueryable();
                    if (model.HK_XP != null)
                    {
                        tb_fm = tb_fm.Where(o => o.ID_XaPhuongHKTT == model.HK_XP).AsQueryable();

                    }
                }

            }
            if (model.TT_TT != null)
            {
                tb_fm = tb_fm.Where(o => o.ID_TinhThanhTT == model.TT_TT).AsQueryable();
                if (model.TT_QH != null)
                {
                    tb_fm = tb_fm.Where(o => o.ID_QuanHuyenTT == model.TT_QH).AsQueryable();
                    if (model.TT_XP != null)
                    {
                        tb_fm = tb_fm.Where(o => o.ID_XaPhuongTT == model.TT_XP).AsQueryable();

                    }
                }

            }
            if (model.ChinhTri != null && model.ChinhTri.Any())
            {
                if (model.ChinhTri.Contains((int)commonEnumHellper.TypeTTChinhTri.ketnapdoan))
                {
                    tb_fm = tb_fm.Where(o => o.NgayVaoDoan != null).AsQueryable();
                }
                if (model.ChinhTri.Contains((int)commonEnumHellper.TypeTTChinhTri.ketnapdang))
                {
                    var dateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                    tb_fm = tb_fm.Where(o => o.NgayVaoDoan != null & o.NgayVaoDangChinhThuc != null && (o.NgayRoiDang == null || o.NgayRoiDang > dateNow)).AsQueryable();
                }
                if (model.ChinhTri.Contains((int)commonEnumHellper.TypeTTChinhTri.danhapngu))
                {
                    tb_fm = tb_fm.Where(o => o.NgayNhapNgu != null).AsQueryable();
                }
            }
            if (model.DanToc != null && model.DanToc.Any())
            {
                tb_fm = tb_fm.Where(o => model.DanToc.Contains(o.DanTocTonGiao)).AsQueryable();
            }

            if (model.GioiTinh != null)
            {
                tb_fm = tb_fm.Where(o => o.GioiTinh == model.GioiTinh).AsQueryable();
            }
            if (model.TrangThai == 1)
            {
                tb_fm = tb_fm.Where(x => x.DaNghiViec == true);
            }
            if (model.TrangThai == 2)
            {
                tb_fm = tb_fm.Where(x => x.DaNghiViec == false);
            }
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);
            if (IsfilterTime)
            {
                // same month
                var monthFrom = startday.Month;
                var monthTo = Endday.Month;
                var dateFrom = startday.Day;
                var dateTo = Endday.Day;
                tb_fm = tb_fm.Where(x => x.NgaySinh != null);
                if (monthFrom == monthTo)
                {
                    tb_fm = tb_fm.Where(x => x.NgaySinh.Value.Month == monthFrom && x.NgaySinh.Value.Day >= dateFrom && x.NgaySinh.Value.Day <= dateTo);
                }
                else
                {
                    tb_fm = tb_fm.Where(x => (x.NgaySinh.Value.Month > monthFrom && x.NgaySinh.Value.Month < monthTo && x.NgaySinh.Value.Day <= dateTo)
                    || (x.NgaySinh.Value.Month == monthFrom && x.NgaySinh.Value.Day >= dateFrom)
                    || (x.NgaySinh.Value.Month == monthTo && x.NgaySinh.Value.Day <= dateTo));
                }
            }
            if (model.LoaiHopDong != null && model.LoaiHopDong.Any() && model.BaoHiem != null && model.BaoHiem.Any())
            {
                tb_fm = (from a in tb_fm
                         join b in _dbcontext.NS_HopDong.AsQueryable()
                         on a.ID equals b.ID_NhanVien
                         join c in _dbcontext.NS_BaoHiem.AsQueryable()
                         on a.ID equals c.ID_NhanVien
                         where model.LoaiHopDong.Contains(b.LoaiHopDong) &&
                          model.BaoHiem.Contains(c.LoaiBaoHiem)
                         select a).AsEnumerable().Distinct().AsQueryable();
            }
            else if (model.BaoHiem != null && model.BaoHiem.Any())
            {
                tb_fm = (from a in tb_fm
                         join b in _dbcontext.NS_BaoHiem.AsQueryable()
                         on a.ID equals b.ID_NhanVien
                         where model.BaoHiem.Contains(b.LoaiBaoHiem)
                         select a).AsEnumerable().Distinct().AsQueryable();
            }
            else if (model.LoaiHopDong != null && model.LoaiHopDong.Any())
            {
                tb_fm = (from a in tb_fm
                         join b in _dbcontext.NS_HopDong.AsQueryable()
                         on a.ID equals b.ID_NhanVien
                         where model.LoaiHopDong.Contains(b.LoaiHopDong)
                         select a).AsEnumerable().Distinct().AsQueryable();
            }
            if (!string.IsNullOrWhiteSpace(model.Text))
            {
                string MaNVSearch = CommonStatic.ConvertToUnSign(model.Text).ToLower();
                tb_fm = tb_fm.Where(x => x.MaNhanVien.ToLower().Contains(MaNVSearch)
                || x.TenNhanVien.ToLower().Contains(model.Text.ToLower())
                || (x.TenNhanVienKhongDau != null && x.TenNhanVienKhongDau.Contains(MaNVSearch))
                || (x.TenNhanVienChuCaiDau != null && x.TenNhanVienChuCaiDau.Contains(MaNVSearch)));
            }
            return tb_fm.OrderByDescending(o => o.NgayTao);
        }

        public IQueryable<NS_PhongBan> GetListPhongBanByDonVi(Guid? donviid)
        {
            return _dbcontext.NS_PhongBan.Where(o => o.ID_DonVi == donviid && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa);
        }



        public HT_NguoiDung GetNguoiTao(Guid idNhanVien)
        {
            return _dbcontext.HT_NguoiDung.FirstOrDefault(o => o.ID_NhanVien == idNhanVien);
        }

        #region Phân ca

        public IEnumerable<NhanVienPhanCaModel> GetNhanVienByPhongBan(Guid phongbanid)
        {
            var listPhongban = new List<Guid>();
            var phongban = _dbcontext.NS_PhongBan.FirstOrDefault(o => o.ID == phongbanid);
            if (phongban != null && phongban.ID_PhongBanCha != null)
            {
                listPhongban = _dbcontext.NS_PhongBan.Where(o => o.ID_PhongBanCha == phongban.ID || o.ID == phongban.ID).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
            }
            else if (phongban != null)
            {
                listPhongban = _dbcontext.NS_PhongBan.Where(o => o.ID_PhongBanCha == phongban.ID || o.ID == phongban.ID).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                var listPhongbannew = _dbcontext.NS_PhongBan.Where(o => listPhongban.Contains(o.ID_PhongBanCha ?? new Guid())).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                listPhongban.AddRange(listPhongbannew);
                listPhongban = listPhongban.Distinct().ToList();
            }

            return (from nv in _dbcontext.NS_NhanVien
                    join ct in _dbcontext.NS_QuaTrinhCongTac
                    on nv.ID equals ct.ID_NhanVien
                    where listPhongban.Contains(ct.ID_PhongBan ?? new Guid())
                    && (nv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa || nv.TrangThai == null)
                    select new NhanVienPhanCaModel
                    {
                        Id = nv.ID,
                        Ma = nv.MaNhanVien,
                        Ten = nv.TenNhanVien,
                        Checked = false,
                        Active = true,
                        IsNew = true,
                    }).AsEnumerable().Distinct();
        }

        public IEnumerable<NhanVienPhanCaModel> GetNhanVienByChiNhanh(Guid chinhanhid)
        {
            return (from nv in _dbcontext.NS_NhanVien
                    join ct in _dbcontext.NS_QuaTrinhCongTac
                    on nv.ID equals ct.ID_NhanVien
                    where ct.ID_DonVi == chinhanhid
                     && (nv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa || nv.TrangThai == null)
                    select new NhanVienPhanCaModel
                    {
                        Id = nv.ID,
                        Ma = nv.MaNhanVien,
                        Ten = nv.TenNhanVien,
                        Checked = false,
                        Active = true,
                        IsNew = true,
                    }).AsEnumerable().Distinct();
        }

        public List<NhanVienSamePhieu> CheckSameTime_CaLamViec(PhanCaModel model)
        {
            var sDate = "0";
            var idCas = string.Empty;
            var idNhanViens = string.Join(",", model.NhanVien);
            if (model.CaTuan != null && model.CaTuan.Where(x => x.value != null).Count() > 0)
            {
                var lstDate = model.CaTuan.Select(x => x.key);
                sDate = string.Join(",", lstDate);
                var listCa = model.CaTuan.Where(x => x.value != null).Select(x => x.value[0]).Distinct();
                idCas = string.Join(",", listCa);
            }
            else
            {
                idCas = string.Join(",", model.CaCoDinh);
            }

            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_DonVi", model.PhieuPhanCa.ID_DonVi));
            lstParam.Add(new SqlParameter("ID_PhieuPhanCa", model.PhieuPhanCa.ID));
            lstParam.Add(new SqlParameter("LoaiPhanCa", model.PhieuPhanCa.LoaiPhanCa));
            lstParam.Add(new SqlParameter("DateOfWeeks", sDate));
            lstParam.Add(new SqlParameter("IDNhanViens", idNhanViens));
            lstParam.Add(new SqlParameter("IDCaLamViecs", idCas));
            lstParam.Add(new SqlParameter("TuNgay", model.PhieuPhanCa.TuNgay));
            lstParam.Add(new SqlParameter("DenNgay", model.PhieuPhanCa.DenNgay ?? (object)DBNull.Value));
            List<NhanVienSamePhieu> data = _dbcontext.Database.SqlQuery<NhanVienSamePhieu>("EXEC CheckSameTime_CaLamViec @ID_DonVi, @ID_PhieuPhanCa, @LoaiPhanCa, @DateOfWeeks," +
                " @IDNhanViens, @IDCaLamViecs, @TuNgay, @DenNgay", lstParam.ToArray()).ToList();
            return data;
        }

        public NhanVienSamePhieu UpdatePhieuPhanCa_CheckExistCong(Guid idPhieuPhanCa)
        {

            SqlParameter sql = new SqlParameter("ID_PhieuPhanCa", idPhieuPhanCa);
            return _dbcontext.Database.SqlQuery<NhanVienSamePhieu>("UpdatePhieuPhanCa_CheckExistCong @ID_PhieuPhanCa", sql).ToList().FirstOrDefault();
        }

        /// <summary>
        /// update NS_CongBoSung (TrangThai = 0) if nhanvien not exist phieu phanca
        /// </summary>
        /// <param name="model"></param>
        public void UpdatePhieuPhanCa_RemoveCongNhanVien(PhanCaModel model)
        {
            var idNhanViens = string.Join(",", model.NhanVien.Select(x => x));
            var idCas = string.Empty;
            if (model.PhieuPhanCa.LoaiPhanCa == (int)commonEnumHellper.LoaiCa.cacodinh)
            {
                idCas = string.Join(",", model.CaCoDinh.Select(x => x));
            }
            else
            {
                idCas = string.Join(",", model.CaTuan.Where(x => x.value != null).Select(x => x.value[0]));
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_PhieuPhanCa", model.PhieuPhanCa.ID));
            sql.Add(new SqlParameter("IDNhanVienNews", idNhanViens));
            sql.Add(new SqlParameter("IDCaNews", idCas));
            sql.Add(new SqlParameter("FromDateNew", model.PhieuPhanCa.TuNgay.ToString("yyyy-MM-dd")));
            sql.Add(new SqlParameter("ToDateNew", model.PhieuPhanCa.DenNgay == null ? (object)DBNull.Value : model.PhieuPhanCa.DenNgay.Value.ToString("yyyy-MM-dd")));
            _dbcontext.Database.ExecuteSqlCommand("UpdatePhieuPhanCa_RemoveCongNhanVien @ID_PhieuPhanCa, @IDNhanVienNews, @IDCaNews, @FromDateNew, @ToDateNew", sql.ToArray());
        }
        public bool InsertPhanCaLamViec(PhanCaModel model)
        {
            var nguoitao = GetNguoiTao(model.PhieuPhanCa.ID_NhanVienTao);
            model.PhieuPhanCa.NguoiTao = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            model.PhieuPhanCa.TrangThai = (int)commonEnumHellper.TrangThaiPhanCa.taomoi;
            var phieu = model.PhieuPhanCa;
            if (string.IsNullOrWhiteSpace(phieu.MaPhieu))
            {
                phieu.MaPhieu = GetMaPhieuPhanCa();
            }
            else if (_dbcontext.NS_PhieuPhanCa.Any(o => o.MaPhieu.ToUpper().Equals(phieu.MaPhieu.ToUpper()) && o.TrangThai != (int)commonEnumHellper.TrangThaiPhanCa.xoa))
            {
                return false;
            }
            phieu.ID = Guid.NewGuid();
            _dbcontext.NS_PhieuPhanCa.Add(phieu);
            var listcacodinhct = new List<NS_PhieuPhanCa_NhanVien>();
            if (model.NhanVien != null && model.NhanVien.Count > 0)
            {
                listcacodinhct = model.NhanVien.Select(o => new NS_PhieuPhanCa_NhanVien
                {
                    ID = Guid.NewGuid(),
                    ID_NhanVien = o,
                    ID_PhieuPhanCa = phieu.ID
                }).ToList();
                _dbcontext.NS_PhieuPhanCa_NhanVien.AddRange(listcacodinhct);
            }
            InsertPhanCaChiTiet(phieu, model.CaCoDinh, model.CaTuan);
            return true;
        }

        public JsonViewModel<string> UpdatePhanCaLamViec(PhanCaModel model)
        {
            var resul = new JsonViewModel<string>() { ErrorCode = false };
            var modelPhieu = _dbcontext.NS_PhieuPhanCa.FirstOrDefault(o => o.ID == model.PhieuPhanCa.ID);
            if (modelPhieu == null)
            {
                resul.Data = "Phiếu phân ca đã bị xóa hoặc không tồn tại";
                return resul;
            }
            var nguoitao = GetNguoiTao(model.PhieuPhanCa.ID_NhanVienTao);
            modelPhieu.NguoiSua = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            modelPhieu.NgaySua = DateTime.Now;
            modelPhieu.DenNgay = model.PhieuPhanCa.DenNgay;
            modelPhieu.GhiChu = model.PhieuPhanCa.GhiChu;
            modelPhieu.TuNgay = model.PhieuPhanCa.TuNgay;
            modelPhieu.ID_DonVi = model.PhieuPhanCa.ID_DonVi;
            modelPhieu.LoaiPhanCa = model.PhieuPhanCa.LoaiPhanCa;
            if (model.NhanVien != null && model.NhanVien.Count > 0)
            {
                UpdatePhieuPhanCa_RemoveCongNhanVien(model);

                // Xóa những nhân viên bị loại khỏi phiêu
                var lstNVDelete = _dbcontext.NS_PhieuPhanCa_NhanVien.Where(o => o.ID_PhieuPhanCa == modelPhieu.ID && !model.NhanVien.Contains(o.ID_NhanVien)).Select(x => x.ID_NhanVien);
                var phieuphancachitietdelete = _dbcontext.NS_PhieuPhanCa_NhanVien.Where(o => o.ID_PhieuPhanCa == modelPhieu.ID && !model.NhanVien.Contains(o.ID_NhanVien)).AsEnumerable();
                _dbcontext.NS_PhieuPhanCa_NhanVien.RemoveRange(phieuphancachitietdelete);

                // ko thêm mới những nhân viên giữ lại
                var listIdOld = _dbcontext.NS_PhieuPhanCa_NhanVien.Where(o => o.ID_PhieuPhanCa == modelPhieu.ID && model.NhanVien.Contains(o.ID_NhanVien)).Select(o => o.ID_NhanVien).ToList();
                var phieuNhanVien = model.NhanVien.Where(o => !listIdOld.Contains(o)).Select(o => new NS_PhieuPhanCa_NhanVien
                {
                    ID = Guid.NewGuid(),
                    ID_NhanVien = o,
                    ID_PhieuPhanCa = modelPhieu.ID
                }).ToList();
                _dbcontext.NS_PhieuPhanCa_NhanVien.AddRange(phieuNhanVien);
            }
            else /*if (modelPhieu.TrangThai == (int)commonEnumHellper.TrangThaiPhanCa.taomoi) */// ca không có nhân viên => xóa toàn bộ
            {
                var phieuphancachitietdelete = _dbcontext.NS_PhieuPhanCa_NhanVien.Where(o => o.ID_PhieuPhanCa == modelPhieu.ID).AsEnumerable();
                _dbcontext.NS_PhieuPhanCa_NhanVien.RemoveRange(phieuphancachitietdelete);
            }
            // xóa toàn bộ chi tiết ca phiếu khi phiếu chưa tính công
            var listDelete = _dbcontext.NS_PhieuPhanCa_CaLamViec.Where(o => o.ID_PhieuPhanCa == modelPhieu.ID).AsEnumerable();
            _dbcontext.NS_PhieuPhanCa_CaLamViec.RemoveRange(listDelete);
            InsertPhanCaChiTiet(modelPhieu, model.CaCoDinh, model.CaTuan);

            resul.ErrorCode = true;
            return resul;
        }

        private void InsertPhanCaChiTiet(NS_PhieuPhanCa model, List<Guid> listCacodinh, List<CaTuanModel> listCatuan)
        {
            if (model.LoaiPhanCa == (int)commonEnumHellper.LoaiCa.cacodinh)
            {
                if (listCacodinh != null && listCacodinh.Count > 0)
                {
                    var data = listCacodinh.Select(o => new NS_PhieuPhanCa_CaLamViec
                    {
                        ID = Guid.NewGuid(),
                        GiaTri = 0,
                        ID_CaLamViec = o,
                        ID_PhieuPhanCa = model.ID

                    });
                    _dbcontext.NS_PhieuPhanCa_CaLamViec.AddRange(data);
                }
            }
            else if (model.LoaiPhanCa == (int)commonEnumHellper.LoaiCa.catuan)
            {
                if (listCatuan != null && listCatuan.Count > 0)
                {
                    foreach (var ituan in listCatuan)
                    {
                        if (ituan.value != null)
                        {
                            var data = ituan.value.Select(o => new NS_PhieuPhanCa_CaLamViec
                            {
                                ID = Guid.NewGuid(),
                                GiaTri = ituan.key,
                                ID_CaLamViec = o,
                                ID_PhieuPhanCa = model.ID,
                            });
                            _dbcontext.NS_PhieuPhanCa_CaLamViec.AddRange(data);
                        }
                    }

                }
            }
        }

        public void InsertNhatKySuDung(string tieude, string noidung, string chitiet, Guid donviid, Guid nhanvienid, int loai)
        {
            HT_NhatKySuDung model = new HT_NhatKySuDung
            {
                ID = Guid.NewGuid(),
                ID_NhanVien = nhanvienid,
                ID_DonVi = donviid,
                ChucNang = tieude,
                ThoiGian = DateTime.Now,
                NoiDung = noidung,
                NoiDungChiTiet = chitiet,
                LoaiNhatKy = loai
            };
            _dbcontext.HT_NhatKySuDung.Add(model);
        }

        /// <summary>
        /// Tìm kiếm danh sách phân ca
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IQueryable<NS_PhieuPhanCa> GetPhanCaFilter(PhieuPhanCaFilter model)
        {
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            var ngaytaoTu = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var ngaytaoDen = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool filterNgayTao = CommonStatic.CheckTimeFilter(model.TypeTimeNgayTao, model.NgayTaoTu, model.NgayTaoDen, ref ngaytaoTu, ref ngaytaoDen);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);

            var data = _dbcontext.NS_PhieuPhanCa.AsQueryable();
            if (model.ListDonVi != null && model.ListDonVi.Count > 0)
            {
                data = data.Where(o => model.ListDonVi.Contains(o.ID_DonVi));
            }
            else
            {
                data = (from pc in _dbcontext.NS_PhieuPhanCa.AsQueryable()
                        join ct in _dbcontext.NS_QuaTrinhCongTac
                        on pc.ID_DonVi equals ct.ID_DonVi
                        where ct.ID_NhanVien == model.IDNhanVien
                        select pc).Distinct();
            }
            if (model.LoaiCa != null && model.LoaiCa.Count > 0)
            {
                data = data.Where(o => model.LoaiCa.Contains(o.LoaiPhanCa));
            }
            if (!string.IsNullOrWhiteSpace(model.Text))
            {
                data = data.Where(o => o.MaPhieu.ToUpper().Contains(model.Text.ToUpper()));
            }
            if (filterNgayTao)
            {
                data = data.Where(x =>
                 x.NgayTao >= ngaytaoTu
                && x.NgayTao <= ngaytaoDen);
            }

            if (IsfilterTime)
            {
                // lấy phiếu nếu có khoảng giao from - to
                var ddd = Endday.ToString("yyyy-MM-dd");
                data = data.Where(x => (Endday >= x.TuNgay
                && (x.DenNgay == null || string.Compare(Endday.ToString("yyyy-MM-dd"), x.DenNgay.Value.ToString("yyyy-MM-dd")) < 0))
                || ((string.Compare(startday.ToString("yyyy-MM-dd"), x.TuNgay.ToString("yyyy-MM-dd")) > 0)
                && (string.Compare(startday.ToString("yyyy-MM-dd"), x.DenNgay.Value.ToString("yyyy-MM-dd")) < 0)));
                //|| (startday >= x.TuNgay && startday <= x.DenNgay));
            }
            if (model.TrangThai != null && model.TrangThai.Count > 0)
            {
                data = data.Where(x => model.TrangThai.Contains(x.TrangThai));
            }
            data = data.Where(o => o.TrangThai != (int)commonEnumHellper.TrangThaiPhanCa.xoa);
            return data.OrderByDescending(o => o.NgayTao);
        }

        public List<NS_PhieuPhanCaDTO> GetListPhieuPhanCa(PhieuPhanCaFilter model, ref string time)
        {
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            var ngaytaoTu = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var ngaytaoDen = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool filterNgayTao = CommonStatic.CheckTimeFilter(model.TypeTimeNgayTao, model.NgayTaoTu, model.NgayTaoDen, ref ngaytaoTu, ref ngaytaoDen);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);

            var text = string.Empty;
            if (model.Text != null)
            {
                text = model.Text;
            }
            var idDonVis = string.Join(",", model.ListDonVi);
            var loaicas = string.Empty;
            if (model.LoaiCa != null)
            {
                loaicas = string.Join(",", model.LoaiCa);
            }
            var trangthai = string.Empty;
            if (model.TrangThai != null)
            {
                trangthai = string.Join(",", model.TrangThai);
            }

            if (filterNgayTao == false)
            {
                ngaytaoTu = new DateTime(2016, 1, 1);
            }

            if (IsfilterTime == false)
            {
                startday = new DateTime(2016, 1, 1);
                Endday = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.Now.Day);
                time = string.Concat(startday.ToString("dd/MM/yyyy"), " - ", Endday.ToString("dd/MM/yyyy"));
            }

            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idDonVis));
            sql.Add(new SqlParameter("LoaiCa", loaicas));
            sql.Add(new SqlParameter("NgayTaoFrom", ngaytaoTu.ToString("yyyy-MM-dd")));
            sql.Add(new SqlParameter("NgayTaoTo", ngaytaoDen.ToString("yyyy-MM-dd")));
            sql.Add(new SqlParameter("FromDate", startday.ToString("yyyy-MM-dd")));
            sql.Add(new SqlParameter("ToDate", Endday.ToString("yyyy-MM-dd")));
            sql.Add(new SqlParameter("TrangThai", trangthai));
            sql.Add(new SqlParameter("TextSearch", text));
            sql.Add(new SqlParameter("CurrentPage", model.pageNow - 1));
            sql.Add(new SqlParameter("PageSize", model.pageSize));
            var data = _dbcontext.Database.SqlQuery<NS_PhieuPhanCaDTO>("exec GetListPhieuPhanCa_Paging @IDChiNhanhs, @LoaiCa, @NgayTaoFrom, @NgayTaoTo," +
                "@FromDate, @ToDate, @TrangThai, @TextSearch, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            return data;
        }

        public IQueryable<NS_PhieuPhanCa> GetPhanCaChiNhanh(PhieuPhanCaFilter model)
        {
            var startday = new DateTime(model.TuNgay.Value.Year, model.TuNgay.Value.Month, model.TuNgay.Value.Day, 00, 00, 00);
            var Endday = new DateTime(model.DenNgay.Value.Year, model.DenNgay.Value.Month, model.DenNgay.Value.Day, 23, 59, 59);
            var data = _dbcontext.NS_PhieuPhanCa.AsQueryable();
            if (model.ListDonVi != null && model.ListDonVi.Count > 0)
            {
                data = data.Where(o => model.ListDonVi.Contains(o.ID_DonVi));
            }
            else
            {
                data = (from nv in _dbcontext.NS_QuaTrinhCongTac
                        join pc in _dbcontext.NS_PhieuPhanCa
                        on nv.ID_DonVi equals pc.ID_DonVi
                        where nv.ID_NhanVien == model.IDNhanVien
                        select pc).Distinct();
            }

            data = data.Where(x =>
            (startday >= x.TuNgay || x.TuNgay < Endday)
             && (x.DenNgay == null || x.DenNgay > startday));


            data = data.Where(o => o.TrangThai != (int)commonEnumHellper.TrangThaiPhanCa.xoa);
            return data.OrderByDescending(o => o.NgayTao);
        }

        /// <summary>
        /// Lấy danh sách chi tiết từng phiếu phân ca
        /// </summary>
        /// <param name="idphieu"></param>
        /// <returns></returns>
        public IEnumerable<object> GetChiTietCaOfPhieu(Guid idphieu)
        {

            var phieuCaLamViec = _dbcontext.NS_PhieuPhanCa_CaLamViec.FirstOrDefault(o => o.ID_PhieuPhanCa == idphieu);
            if (phieuCaLamViec != null)
            {
                if (phieuCaLamViec.NS_PhieuPhanCa != null && phieuCaLamViec.NS_PhieuPhanCa.LoaiPhanCa == (int)commonEnumHellper.LoaiCa.cacodinh)
                {
                    return (from ca in _dbcontext.NS_CaLamViec
                            join ct in _dbcontext.NS_PhieuPhanCa_CaLamViec
                            on ca.ID equals ct.ID_CaLamViec
                            where ct.ID_PhieuPhanCa == idphieu
                            orderby ca.GioVao descending
                            select new ChiTietCaOfPhieu
                            {
                                GioRa = ca.GioRa,
                                GioVao = ca.GioVao,
                                IdCa = ca.ID,
                                MaCa = ca.MaCa,
                                TenCa = ca.TenCa
                            }).AsEnumerable();
                }
                else if (phieuCaLamViec.NS_PhieuPhanCa != null && phieuCaLamViec.NS_PhieuPhanCa.LoaiPhanCa == (int)commonEnumHellper.LoaiCa.catuan)
                {
                    return (from ca in _dbcontext.NS_CaLamViec
                            join ct in _dbcontext.NS_PhieuPhanCa_CaLamViec
                            on ca.ID equals ct.ID_CaLamViec
                            where ct.ID_PhieuPhanCa == idphieu
                            select new ChiTietCaOfPhieu
                            {
                                GiaTri = ct.GiaTri,
                                GioRa = ca.GioRa,
                                GioVao = ca.GioVao,
                                IdCa = ca.ID,
                                MaCa = ca.MaCa,
                                TenCa = ca.TenCa,
                            }).AsEnumerable().OrderBy(o => o.GiaTri).ThenBy(o => o.GioVao).GroupBy(o => o.GiaTri).Select(o => new
                            {
                                GiaTri = o.Key,
                                TenGiaTri = commonEnumHellper.ListWeek.FirstOrDefault(c => c.Key == o.Key).Value,
                                listCa = o.ToList()

                            }).AsEnumerable();
                }
            }
            return new List<ChiTietCaOfPhieu>();
        }

        public IQueryable<NS_NhanVien> GetChiTietNhanVienOfPhieu(Guid idphieu)
        {
            return (from nv in _dbcontext.NS_NhanVien
                    join ct in _dbcontext.NS_PhieuPhanCa_NhanVien
                    on nv.ID equals ct.ID_NhanVien
                    where ct.ID_PhieuPhanCa == idphieu
                    select nv).AsQueryable();


        }

        public NS_PhieuPhanCa GetPhieuPhanCaById(Guid id)
        {
            return _dbcontext.NS_PhieuPhanCa.FirstOrDefault(o => o.ID == id);
        }

        /// <summary>
        ///  Xóa vật lý
        /// </summary>
        /// <param name="idPhieu"></param>
        /// <returns></returns>
        public JsonViewModel<string> DeletePhanCa(Guid idPhieu)
        {
            var resul = new JsonViewModel<string>() { ErrorCode = false };
            if (_dbcontext.NS_PhieuPhanCa.Any(o => o.ID == idPhieu))
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("IdPhieu", idPhieu));
                var checkStore = _dbcontext.Database.SqlQuery<int>("exec DeletePhieuPhanCa @IdPhieu", sql.ToArray()).FirstOrDefault();
                if (checkStore < 0)
                {
                    resul.Data = "Đã xảy ra lỗi vui lòng thử lại sau";
                }
                else
                {
                    resul.ErrorCode = true;
                }
            }
            else
            {
                resul.Data = "Phiếu phân ca đã xóa hoặc không tồn tại";
            }
            return resul;
        }

        /// <summary>
        /// Xóa trạng thái
        /// </summary>
        /// <param name="idPhieu"></param>
        /// <returns></returns>
        public bool DeletePhieuPhanCa(Guid idPhieu)
        {
            var model = _dbcontext.NS_PhieuPhanCa.FirstOrDefault(o => o.ID == idPhieu);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TrangThaiPhanCa.xoa;

                var sql = @"update bs1 set bs1.TrangThai= 0
	                from NS_CongBoSung bs1
	                join (
		                select bs.ID
		                from NS_CongBoSung bs
		                join NS_PhieuPhanCa_NhanVien phieunv on bs.ID_NhanVien = phieunv.ID_NhanVien
		                join NS_PhieuPhanCa_CaLamViec phieuca on phieunv.ID_PhieuPhanCa = phieuca.ID_PhieuPhanCa and  bs.ID_CaLamViec = phieuca.ID_CaLamViec
		                join NS_PhieuPhanCa phieu on phieunv.ID_PhieuPhanCa = phieu.ID
		                where phieu.ID = '" + idPhieu + @"'" +
                        @")bs2 on bs1.ID = bs2.ID";
                _dbcontext.Database.ExecuteSqlCommand(sql);
                return true;
            }
            return false;
        }

        public bool ChamCong_UpdatePhieuPhanCa(Guid idPhieu)
        {
            var model = _dbcontext.NS_PhieuPhanCa.FirstOrDefault(o => o.ID == idPhieu);
            if (model != null)
            {
                model.TrangThai = (int)commonEnumHellper.TrangThaiPhanCa.dangapdung;
                _dbcontext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<ExportPhieuPhanCa> GetListPhieuPhanCaExport(PhieuPhanCaFilter model, ref string time)
        {
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);
            if (IsfilterTime)
            {
                time = "Từ ngày: " + startday.ToString("dd/MM/yyyy") + " - Đến ngày: " + Endday.ToString("dd/MM/yyyy");
            }
            else
            {
                time = "Toàn thời gian";
            }

            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("Ma", model.Text ?? string.Empty));
            paramlist.Add(new SqlParameter("NgayBatDau", startday.ToString("dd/MM/yyyy")));
            paramlist.Add(new SqlParameter("NgayKetThuc", Endday.AddDays(1).ToString("dd/MM/yyyy")));
            paramlist.Add(new SqlParameter("ListTrangThai", string.Join(",", model.TrangThai ?? new List<int?>())));
            paramlist.Add(new SqlParameter("TimThoiGian", IsfilterTime == true ? 1 : 0));
            paramlist.Add(new SqlParameter("ListDonvi", string.Join(",", model.ListDonVi ?? new List<Guid>())));
            paramlist.Add(new SqlParameter("ListLoaiCa", string.Join(",", model.LoaiCa ?? new List<int?>())));
            paramlist.Add(new SqlParameter("ID_NhanVien", model.IDNhanVien));
            var lst = _dbcontext.Database.SqlQuery<ExportPhieuPhanCa>("exec GetListPhieuPhanCa @Ma,@NgayBatDau,@NgayKetThuc,@ListTrangThai,@TimThoiGian,@ListDonvi,@ListLoaiCa,@ID_NhanVien", paramlist.ToArray()).ToList();
            return lst;
        }

        public string GetMaPhieuPhanCa()
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("inputVar", "PPC00001"));
            return _dbcontext.Database.SqlQuery<string>("exec RandomMaPhieuPhanCa @inputVar", sql.ToArray()).FirstOrDefault().Trim();
        }

        #endregion

        #region Kỳ tính công

        public IQueryable<NS_KyTinhCong> GetAllKyTinhCongActive()
        {
            return _dbcontext.NS_KyTinhCong.Where(o => o.TrangThai != (int)commonEnumHellper.TrangThaiPhanCa.xoa);
        }

        public IQueryable<NS_KyTinhCong> GetKyTinhCongFilter(SearchFilter model)
        {
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);

            var data = GetAllKyTinhCongActive().AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Text))
            {
                int ky = 0;
                if (CommonStatic.IsValidNumberFormat(model.Text, ref ky))
                {
                    data = data.Where(o => o.Ky == ky);
                }
                else
                {
                    return new List<NS_KyTinhCong>().AsQueryable().OrderByDescending(o => o.NgayTao);
                }
            }
            if (IsfilterTime)
            {
                data = data.Where(x =>
                 x.NgayTao >= startday
                && x.NgayTao <= Endday);
            }
            if (model.TrangThai != null && model.TrangThai.Count > 0)
            {
                data = data.Where(x => model.TrangThai.Contains(x.TrangThai));
            }
            return data.OrderByDescending(o => o.NgayTao);
        }

        public JsonViewModel<string> InsertKyTinhCong(NS_KyTinhCong model, Guid idNhanVien)
        {
            var resul = new JsonViewModel<string> { ErrorCode = false };
            if (_dbcontext.NS_KyTinhCong.Any(o => o.TrangThai != (int)commonEnumHellper.TrangThaiKyTinhCong.xoa &&
                                                ((o.TuNgay <= model.TuNgay && model.TuNgay < o.DenNgay)
                                                 || (o.TuNgay < model.DenNgay && model.DenNgay <= o.DenNgay))))
            {
                resul.Data = "Khoảng thời gian của kỳ tính công đã bị trùng";
            }
            else
            {
                if (model.Ky == 0)
                {
                    if (_dbcontext.NS_KyTinhCong.Any())
                    {
                        model.Ky = _dbcontext.NS_KyTinhCong.Max(o => o.Ky) + 1;
                    }
                    else
                    {
                        model.Ky = 1;
                    }
                }
                else if (_dbcontext.NS_KyTinhCong.Any(o => o.TrangThai != (int)commonEnumHellper.TrangThaiKyTinhCong.xoa &&
                                           o.Ky == model.Ky))
                {
                    resul.Data = "Kỳ đã tồn tại vui lòng nhập kỳ khác";
                    return resul;
                }
                var nguoitao = GetNguoiTao(idNhanVien);
                model.NguoiTao = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
                model.TrangThai = (int)commonEnumHellper.TrangThaiKyTinhCong.taomoi;
                _dbcontext.NS_KyTinhCong.Add(model);
                resul.ErrorCode = true;
            }
            return resul;
        }

        public JsonViewModel<string> UpdateKyTinhCong(NS_KyTinhCong model, Guid idNhanVien, Guid idDonVi)
        {
            var resul = new JsonViewModel<string> { ErrorCode = false };
            if (_dbcontext.NS_KyTinhCong.Any(o => o.ID != model.ID && o.TrangThai != (int)commonEnumHellper.TrangThaiKyTinhCong.xoa &&
                                                    ((o.TuNgay <= model.TuNgay && model.TuNgay < o.DenNgay)
                                                 || (o.TuNgay < model.DenNgay && model.DenNgay <= o.DenNgay))))
            {
                resul.Data = "Khoảng thời gian của kỳ tính công đã bị trùng";
            }
            else
            {
                var data = _dbcontext.NS_KyTinhCong.FirstOrDefault(o => o.ID == model.ID);
                if (data == null)
                {
                    resul.Data = "Kỳ tính công không tồn tại hoặc đã bị xóa";
                    return resul;
                }
                string noidung = "Cập nhật kỳ tính công - kỳ: " + data.Ky;
                string chitiet = "Cập nhật kỳ tính công: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > kỳ: " + model.Ky +
                                   "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                   ", : Từ ngày" + data.TuNgay.ToString("dd/MM/yyyy") + " => " + model.TuNgay.ToString("dd/MM/yyyy") +
                                  ", Đến ngày: " + data.DenNgay.ToString("dd/MM/yyyy") + " => " + model.TuNgay.ToString("dd/MM/yyyy") +
                                   ", Trạng thái: " + (data.TrangThai == 1 ? "Đang áp dụng" : "Đã chốt công") + " => " + (model.TrangThai == 1 ? "Đang áp dụng" : "Đã chốt công");
                InsertNhatKySuDung("Kỳ tính công", noidung, chitiet, idDonVi, idNhanVien, 2);
                var nguoitao = GetNguoiTao(idNhanVien);
                data.Ky = model.Ky;
                data.NguoiSua = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
                data.DenNgay = model.DenNgay;
                data.TuNgay = model.TuNgay;
                _dbcontext.SaveChanges();
                resul.ErrorCode = true;
            }
            return resul;
        }

        public bool CheckExitKyTinhCong(int soky, Guid? id = null)
        {
            if (id == null)
                return _dbcontext.NS_KyTinhCong.Any(o => o.TrangThai != (int)commonEnumHellper.TrangThaiKyTinhCong.xoa && o.Ky == soky);
            else
                return _dbcontext.NS_KyTinhCong.Any(o => o.ID != id && o.TrangThai != (int)commonEnumHellper.TrangThaiKyTinhCong.xoa && o.Ky == soky);
        }

        public JsonViewModel<string> DeleteKyTinhCong(NS_KyTinhCong model, Guid idNhanVien)
        {
            var resul = new JsonViewModel<string> { ErrorCode = false };
            var data = _dbcontext.NS_KyTinhCong.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                resul.Data = "Kỳ tính công không tồn tại";
                return resul;
            }
            data.TrangThai = (int)commonEnumHellper.TrangThaiKyTinhCong.xoa;
            resul.ErrorCode = true;

            return resul;
        }

        public NS_KyTinhCong GetKyTinhCongById(Guid id)
        {
            return _dbcontext.NS_KyTinhCong.FirstOrDefault(o => o.ID == id);
        }

        #endregion

        #region Ký hiệu công - ngày nghỉ lễ

        public IQueryable<NS_KyHieuCong> GetAllKyHieuCong(Guid? idDonVi = null)
        {
            return _dbcontext.NS_KyHieuCong.Where(o => o.ID_DonVi == idDonVi && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsQueryable();
        }

        public JsonViewModel<string> InsertKyHieuCong(NS_KyHieuCong model, Guid idNhanVien)
        {
            var resul = new JsonViewModel<string> { ErrorCode = true };
            if (_dbcontext.NS_KyHieuCong.Any(o => o.KyHieu.Equals(model.KyHieu.ToUpper()) && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa
            && o.ID_DonVi == model.ID_DonVi))
            {
                resul.Data = "Ký hiệu đã tồn tại";
                return resul;
            }

            //if (_dbcontext.NS_KyHieuCong.Any(o => o.CongQuyDoi == model.CongQuyDoi && o.ID_DonVi == model.ID_DonVi && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa))
            //{
            //    resul.Data = "Trùng lặp công quy đổi";
            //    return resul;
            //}
            model.KyHieu = model.KyHieu.ToUpper();
            model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
            if (model.LayGioMacDinh)
            {
                model.GioVao = null;
                model.GioRa = null;
            }
            _dbcontext.NS_KyHieuCong.Add(model);
            resul.ErrorCode = false;
            return resul;
        }

        public JsonViewModel<string> UpdateKyHieuCong(NS_KyHieuCong model, Guid idNhanVien, Guid idDonVi)
        {
            var resul = new JsonViewModel<string> { ErrorCode = true };
            var data = _dbcontext.NS_KyHieuCong.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                resul.Data = "Ký hiệu không tồn tại hoặc đã bị xóa";
                return resul;
            }
            string noidung = "Cập nhật ký hiệu công - ký hiệu: " + model.KyHieu;
            string chitiet = "Cập nhật ký hiệu công: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > ký hiệu: " + model.KyHieu +
                               "</a><br /> Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                               "<br /> Mô tả: " + data.MoTa + " => " + model.MoTa +
                              "<br /> Công quy đổi: " + data.CongQuyDoi + " => " + model.CongQuyDoi +
                               "<br /> Lấy giờ theo ca: " + (data.LayGioMacDinh == true ? "Có" : "Không") + " => " + (model.LayGioMacDinh == true ? "Có" : "Không");
            if (!model.LayGioMacDinh)
            {
                chitiet += "<br /> Giờ vào: " + model.GioVao.Value.ToString("dd/MM") +
                              "<br /> Giờ ra: " + model.GioRa.Value.ToString("dd/MM");
            }
            InsertNhatKySuDung("Danh mục ký hiệu công", noidung, chitiet, idDonVi, idNhanVien, 1);

            data.MoTa = model.MoTa;
            data.LayGioMacDinh = model.LayGioMacDinh;
            var kyhieuold = data.KyHieu;
            if (model.LayGioMacDinh)
            {
                data.GioVao = null;
                data.GioRa = null;
            }
            else
            {
                data.GioVao = model.GioVao;
                data.GioRa = model.GioRa;
            }
            if (data.CongQuyDoi != model.CongQuyDoi || kyhieuold != model.KyHieu)
            {
                data.CongQuyDoi = model.CongQuyDoi;
                data.KyHieu = model.KyHieu;
                _dbcontext.SaveChanges();

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("ID_DonVi", idDonVi));
                sql.Add(new SqlParameter("ID_KyHieuCong", model.ID));
                sql.Add(new SqlParameter("KyHieuCongOld", kyhieuold));
                _dbcontext.Database.ExecuteSqlCommand("exec UpdateChamCongKhiThayDoiHeSo @ID_DonVi,@ID_KyHieuCong,@KyHieuCongOld", sql.ToArray());
                UpdateStatusBangLuong_whenChangeCong(idDonVi, DateTime.Now);
            }
            else
            {
                _dbcontext.SaveChanges();
            }
            resul.ErrorCode = false;
            return resul;
        }

        public JsonViewModel<string> DeleteKyHieuCong(NS_KyHieuCong model)
        {
            var resul = new JsonViewModel<string> { ErrorCode = false };
            var data = _dbcontext.NS_KyHieuCong.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                resul.Data = "Ký hiệu công không tồn tại";
                return resul;
            }
            if (_dbcontext.NS_CongBoSung.Any(o => o.KyHieuCong.Equals(data.KyHieu)))
            {
                resul.Data = "Ký hiệu công đã được áp dụng, không cho phép xóa";
                return resul;
            }
            data.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
            resul.ErrorCode = true;
            return resul;
        }

        public IEnumerable<object> GetAllNgayNghiLe()
        {
            return _dbcontext.NS_NgayNghiLe
                            .Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable()
                .Select(o => new
                {
                    o.ID,
                    o.Thu,
                    ThuText = o.Thu >= 0 ? commonEnumHellper.ListWeek.FirstOrDefault(c => c.Key == o.Thu).Value : string.Empty,
                    o.Ngay,
                    o.LoaiNgay,
                    LoaiNgayText = commonEnumHellper.ListLoaiNgaynghiLe.FirstOrDefault(c => c.Key == o.LoaiNgay).Value,
                    o.TrangThai,
                    o.NguoiTao,
                    o.NgayTao,
                    o.MoTa,
                }).OrderBy(o => o.Thu).ThenBy(o => o.Ngay).AsEnumerable();
        }

        public bool IsExitNgayNghiLe(DateTime? ngay, Guid? id = null)
        {
            if (id == null)
                return _dbcontext.NS_NgayNghiLe.Any(o => o.Ngay == ngay && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa);
            else
                return _dbcontext.NS_NgayNghiLe.Any(o => o.ID != id && o.Ngay == ngay && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa);
        }

        public JsonViewModel<string> InsertNgayNghiLe(NS_NgayNghiLe model, Guid idNhanVien, Guid idDonVi)
        {
            var resul = new JsonViewModel<string> { ErrorCode = true };
            if (model.Thu != -1)
            {
                if (_dbcontext.NS_NgayNghiLe.Any(x => x.Thu == model.Thu && x.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa))
                {
                    resul.Data = string.Concat(commonEnumHellper.ListWeek.Where(x => x.Key == model.Thu).FirstOrDefault().Value, " đã tồn tại trong danh mục ngày");
                    return resul;
                }
            }
            else
            {
                if (IsExitNgayNghiLe(model.Ngay))
                {
                    resul.Data = "Ngày nghỉ đã tồn tại";
                    return resul;
                }
            }

            var nguoitao = GetNguoiTao(idNhanVien);
            model.NguoiTao = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            model.ID = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
            _dbcontext.NS_NgayNghiLe.Add(model);

            if (model.Ngay != null)
            {
                UpdateCong_WhenChangeNgayNghiLe(idDonVi, (int)model.Ngay.Value.DayOfWeek, (int)commonEnumHellper.LoaiNgaynghiLe.ngaythuong, model.LoaiNgay, model.Ngay);
            }
            else
            {
                UpdateCong_WhenChangeNgayNghiLe(idDonVi, model.Thu, model.LoaiNgay, (int)commonEnumHellper.LoaiNgaynghiLe.ngaythuong, null);
            }

            resul.ErrorCode = false;
            return resul;
        }

        public JsonViewModel<string> UpdateNgayNghiLe(NS_NgayNghiLe model, Guid idNhanVien, Guid idDonVi)
        {
            var resul = new JsonViewModel<string> { ErrorCode = true };
            var data = _dbcontext.NS_NgayNghiLe.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                resul.Data = "Ngày nghỉ lễ này không tồn tại hoặc đã bị xóa";
                return resul;
            }
            if (data.Thu < 0 && IsExitNgayNghiLe(model.Ngay, model.ID))
            {
                resul.Data = "Ngày nghỉ đã tồn tại";
                return resul;
            }
            string noidung = "Cập nhật Ngày nghỉ lễ -" + (data.Thu < 0 ? " Ngày: " + data.Ngay.Value.ToString("dd/MM/yyyy") : " Thứ: " + commonEnumHellper.ListWeek.FirstOrDefault(o => o.Key == data.Thu).Value);
            string chitiet = "Cập nhật Ngày nghỉ lễ: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > " + (data.Thu < 0 ? " Ngày: " + data.Ngay.Value.ToString("dd/MM/yyyy") : " Thứ: " + commonEnumHellper.ListWeek.FirstOrDefault(o => o.Key == data.Thu).Value) +
                               "</a>,<br /> Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                ",<br />" + (data.Thu < 0 ? " Ngày: " + data.Ngay.Value.ToString("dd/MM/yyyy") : " Thứ: " + commonEnumHellper.ListWeek.FirstOrDefault(o => o.Key == data.Thu).Value) +
                                 " => " + (data.Thu < 0 ? " Ngày: " + model.Ngay.Value.ToString("dd/MM/yyyy") : " Thứ: " + commonEnumHellper.ListWeek.FirstOrDefault(o => o.Key == data.Thu).Value) +
                               ",<br /> Mô tả: " + data.MoTa + " => " + model.MoTa +
                               ",<br /> Loại ngày: " + commonEnumHellper.ListLoaiNgaynghiLe.FirstOrDefault(o => o.Key == data.LoaiNgay).Value +
                               " => " + commonEnumHellper.ListLoaiNgaynghiLe.FirstOrDefault(o => o.Key == model.LoaiNgay).Value;

            InsertNhatKySuDung("Danh mục ngày nghỉ lễ", noidung, chitiet, idDonVi, idNhanVien, 2);

            if (data.Ngay != null)
            {
                if (data.Ngay != model.Ngay)
                {
                    // update cong in ngaycu (ngayle --> ngaythuong)
                    UpdateCong_WhenChangeNgayNghiLe(idDonVi, (int)data.Ngay.Value.DayOfWeek, data.LoaiNgay, (int)commonEnumHellper.LoaiNgaynghiLe.ngaythuong, data.Ngay);

                    // update cong in ngaymoi (ngaythuong --> ngayle)
                    UpdateCong_WhenChangeNgayNghiLe(idDonVi, (int)model.Ngay.Value.DayOfWeek, (int)commonEnumHellper.LoaiNgaynghiLe.ngaythuong, model.LoaiNgay, model.Ngay);
                }
                else
                {
                    // update cong in ngaymoi
                    UpdateCong_WhenChangeNgayNghiLe(idDonVi, (int)model.Ngay.Value.DayOfWeek, data.LoaiNgay, model.LoaiNgay, model.Ngay);
                }
            }
            else
            {
                UpdateCong_WhenChangeNgayNghiLe(idDonVi, data.Thu, data.LoaiNgay, model.LoaiNgay, null);
            }

            data.LoaiNgay = model.LoaiNgay;
            data.MoTa = model.MoTa;
            var nguoitao = GetNguoiTao(idNhanVien);
            data.NguoiSua = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            data.NgaySua = DateTime.Now;
            if (data.Thu < 0)
            {
                data.Ngay = model.Ngay;
            }
            _dbcontext.SaveChanges();

            resul.ErrorCode = false;
            return resul;
        }

        public JsonViewModel<string> DeleteNgayNghiLe(NS_NgayNghiLe model, Guid idNhanVien, Guid idDonVi)
        {
            var resul = new JsonViewModel<string> { ErrorCode = true };
            var data = _dbcontext.NS_NgayNghiLe.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                resul.Data = "Ngày nghỉ lễ này không tồn tại hoặc đã bị xóa";
                return resul;
            }
            if (data.Thu >= 0)
            {
                resul.Data = "Cảnh báo, bạn đang cố tình xóa dữ liệu mặc định thứ trong tuần";
                return resul;
            }
            var nguoitao = GetNguoiTao(idNhanVien);
            data.NguoiSua = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            data.TrangThai = (int)commonEnumHellper.TypeIsDelete.daxoa;
            data.NgaySua = DateTime.Now;
            resul.ErrorCode = false;
            // delete --> convert to normal day
            UpdateCong_WhenChangeNgayNghiLe(idDonVi, (int)data.Ngay.Value.DayOfWeek, data.LoaiNgay, (int)commonEnumHellper.LoaiNgaynghiLe.ngaythuong, data.Ngay);

            return resul;
        }

        public AddCongModel CheckKyHieuCong(AddCongModel model)
        {
            var kyhieu = _dbcontext.NS_KyHieuCong.FirstOrDefault(o => o.ID == model.ID_KyHieuCong);
            model.Cong = kyhieu.CongQuyDoi;
            model.KyHieuCong = kyhieu.KyHieu;
            return model;
        }
        #endregion

        #region ChamCong

        public void UpdateStatusCongBoSung_WhenCreatBangLuong(Guid idBangLuong, DateTime tungay, DateTime denngay)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_BangLuong", idBangLuong));
            lstParam.Add(new SqlParameter("FromDate", tungay));
            lstParam.Add(new SqlParameter("ToDate", denngay));
            _dbcontext.Database.ExecuteSqlCommand("EXEC UpdateStatusCongBoSung_WhenCreatBangLuong @ID_BangLuong, @FromDate, @ToDate", lstParam.ToArray());
        }

        public JsonViewModel<string> HuyBangLuong(Guid idBangLuong, HT_NhatKySuDung diary)
        {
            // update sattus bangluong
            var result = new JsonViewModel<string> { ErrorCode = true };
            NS_BangLuong bl = _dbcontext.NS_BangLuong.Find(idBangLuong);
            bl.TrangThai = (int)commonEnumHellper.eBangLuong.xoa;
            var blct = _dbcontext.NS_BangLuong_ChiTiet.Where(x => x.ID_BangLuong == idBangLuong);
            blct.ToList().ForEach(x => x.TrangThai = (int)commonEnumHellper.eBangLuong.xoa);

            // huyphieuthu
            var ltsQuyCT = from quyct in _dbcontext.Quy_HoaDon_ChiTiet
                           join ctluong in blct on quyct.ID_BangLuongChiTiet equals ctluong.ID
                           select quyct;

            (from quyct in ltsQuyCT
             join qhd in _dbcontext.Quy_HoaDon on quyct.ID_HoaDon equals qhd.ID
             select qhd).ToList().ForEach(x => x.TrangThai = false);

            // update lai congnoluong
            HuyPhieuThu_UpdateCongNoTamUngLuong(bl.ID_DonVi, ltsQuyCT.Select(x => x.ID).ToList(), false);

            // save diary
            diary.ChucNang = "Bảng lương";
            diary.NoiDung = "Hủy bảng lương : " + bl.TenBangLuong
                      + " với mã bảng lương: " + bl.MaBangLuong;
            diary.NoiDungChiTiet = "Thông tin bảng lương:"
                                    + "<br/> Mã: " + bl.MaBangLuong
                                    + "<br/> Tên: " + bl.TenBangLuong
                                    + "<br/> Nhân viên hủy: " + _dbcontext.NS_NhanVien.Where(x => x.ID == diary.ID_NhanVien).FirstOrDefault().TenNhanVien;
            diary.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
            _dbcontext.SaveChanges();
            UpdateStatusCongBoSung_WhenCreatBangLuong(idBangLuong, bl.TuNgay ?? DateTime.Now, bl.DenNgay ?? DateTime.Now);
            result.ErrorCode = false;
            return result;
        }

        public List<QuyChiTietPhieuLuong> GetListDebitSalaryDetail(Guid idBangLuong, string textSearch, int currentPage, int pageSize)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_BangLuong", idBangLuong));
            lstParam.Add(new SqlParameter("TextSearch", textSearch));
            lstParam.Add(new SqlParameter("Currentpage", currentPage));
            lstParam.Add(new SqlParameter("PageSize", pageSize));
            return _dbcontext.Database.SqlQuery<QuyChiTietPhieuLuong>("EXEC GetListDebitSalaryDetail @ID_BangLuong,@TextSearch, @Currentpage,@PageSize", lstParam.ToArray()).ToList();
        }

        public void UpdateCongNo_TamUngLuong(Guid? idChiNhanh, List<Guid> lstQuyCT, bool laPhieuTamUng)
        {
            try
            {
                var idQuyCT = string.Join(",", lstQuyCT);
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
                lstParam.Add(new SqlParameter("IDQuyChiTiets", idQuyCT));
                lstParam.Add(new SqlParameter("LaPhieuTamUng", laPhieuTamUng));
                _dbcontext.Database.ExecuteSqlCommand("EXEC UpdateCongNo_TamUngLuong @ID_ChiNhanh, @IDQuyChiTiets, @LaPhieuTamUng", lstParam.ToArray());
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("UpdateCongNo_TamUngLuong " + e);
            }
        }

        public void HuyPhieuThu_UpdateCongNoTamUngLuong(Guid? idChiNhanh, List<Guid> lstQuyCT, bool laPhieuTamUng)
        {
            try
            {
                var idQuyCT = string.Join(",", lstQuyCT);
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
                lstParam.Add(new SqlParameter("IDQuyChiTiets", idQuyCT));
                lstParam.Add(new SqlParameter("LaPhieuTamUng", laPhieuTamUng));
                _dbcontext.Database.ExecuteSqlCommand("EXEC HuyPhieuThu_UpdateCongNoTamUngLuong @ID_ChiNhanh, @IDQuyChiTiets, @LaPhieuTamUng", lstParam.ToArray());
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("HuyPhieuThu_UpdateCongNoTamUngLuong " + e);
            }
        }
        public void HuyPhieuThu_UpdateTrangThaiCong(Guid idQuyCT)
        {
            try
            {
                SqlParameter param = new SqlParameter("ID_QuyChiTiet", idQuyCT);
                _dbcontext.Database.ExecuteSqlCommand("EXEC HuyPhieuThu_UpdateTrangThaiCong @ID_QuyChiTiet", param);
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("HuyPhieuThu_UpdateCongNoTamUngLuong " + e);
            }
        }

        public double CheckGetCong(string kyhieu)
        {
            if (!string.IsNullOrWhiteSpace(kyhieu))
            {
                var modelKyCong = _dbcontext.NS_KyHieuCong.FirstOrDefault(o =>
                                                     //o.ID_KyTinhCong == null &&
                                                     o.KyHieu.ToUpper().Equals(kyhieu.ToUpper())
                                                    && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa);
                return modelKyCong != null ? modelKyCong.CongQuyDoi : 0;
            }
            return 0;
        }

        public NS_CongBoSung GetCongBoSungById(Guid Id)
        {
            return _dbcontext.NS_CongBoSung.FirstOrDefault(o => o.ID == Id);
        }

        public IEnumerable<ChamCongModel> GetChamCongFilter(ChamCongFilter model)
        {
            try
            {
                var txt = string.Empty;
                var idPhongs = string.Empty;
                if (model.Text != null)
                {
                    txt = model.Text;
                }
                var trangthaiNVs = "0,1";
                if (model.TrangThai != null)
                {
                    trangthaiNVs = string.Join(",", model.TrangThai);
                }
                var idChiNhanhs = string.Join(",", model.ListDonVi);
                if (model.PhonBanId != null)
                {
                    var listPhongBan = GetListPhongbanById(model.PhonBanId ?? new Guid());
                    idPhongs = string.Join(",", listPhongBan);
                }
                var idCas = "%%";
                if (model.ListCa != null && model.ListCa.Count() > 0)
                {
                    idCas = string.Join(",", model.ListCa);
                }
                var from = model.TuNgay ?? DateTime.Now;
                var to = model.DenNgay ?? DateTime.Now;

                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Clear();
                lstParam.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs));
                lstParam.Add(new SqlParameter("IDPhongBans", idPhongs));
                lstParam.Add(new SqlParameter("IDCaLamViecs", idCas));
                lstParam.Add(new SqlParameter("TextSearch", txt));
                lstParam.Add(new SqlParameter("FromDate", from.ToString("yyyy-MM-dd")));
                lstParam.Add(new SqlParameter("ToDate", to.ToString("yyyy-MM-dd")));
                lstParam.Add(new SqlParameter("CurrentPage", model.pageNow));
                lstParam.Add(new SqlParameter("PageSize", model.pageSize));
                lstParam.Add(new SqlParameter("TrangThaiNV", trangthaiNVs));
                var result = _dbcontext.Database.SqlQuery<ChamCongModel>("GetDuLieuChamCong @IDChiNhanhs, @IDPhongBans, @IDCaLamViecs, @TextSearch, " +
                    "@FromDate, @ToDate, @CurrentPage, @PageSize, @TrangThaiNV ", lstParam.ToArray()).AsEnumerable();
                return result.ToList();
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("GetChamCongFilter " + e.InnerException + e.Message);
                return new List<ChamCongModel>();
            }
        }

        public List<BangCongDTO> GetBangCongNhanVien(ChamCongFilter model)
        {
            try
            {
                var whereSql = string.Empty;
                var txt = string.Empty;
                var idPhongs = string.Empty;
                if (model.Text != null)
                {
                    txt = model.Text;
                }
                var trangthaiNVs = "0,1";
                if (model.TrangThai != null)
                {
                    trangthaiNVs = string.Join(",", model.TrangThai);
                }
                var idChiNhanhs = string.Join(",", model.ListDonVi);
                if (model.PhonBanId != null)
                {
                    var listPhongBan = GetListPhongbanById(model.PhonBanId ?? new Guid());
                    idPhongs = string.Join(",", listPhongBan);
                }
                var from = model.TuNgay ?? DateTime.Now;
                var to = model.DenNgay ?? DateTime.Now;

                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs));
                lstParam.Add(new SqlParameter("ID_NhanVienLogin", model.IDNhanVien));
                lstParam.Add(new SqlParameter("IDPhongBans", idPhongs));
                lstParam.Add(new SqlParameter("IDCaLamViecs", "%%"));
                lstParam.Add(new SqlParameter("TextSearch", txt));
                lstParam.Add(new SqlParameter("FromDate", from.ToString("yyyy-MM-dd")));
                lstParam.Add(new SqlParameter("ToDate", to.ToString("yyyy-MM-dd")));
                lstParam.Add(new SqlParameter("CurrentPage", model.pageNow));
                lstParam.Add(new SqlParameter("PageSize", model.pageSize));
                lstParam.Add(new SqlParameter("TrangThaiNV", trangthaiNVs));
                var result = _dbcontext.Database.SqlQuery<BangCongDTO>("GetBangCongNhanVien @IDChiNhanhs, @ID_NhanVienLogin, @IDPhongBans, @IDCaLamViecs, @TextSearch, " +
                    "@FromDate, @ToDate, @CurrentPage, @PageSize, @TrangThaiNV", lstParam.ToArray()).AsEnumerable();
                return result.ToList();
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("GetBangCongNhanVien " + e.InnerException + e.Message);
                return new List<BangCongDTO>();
            }
        }

        public JsonViewModel<string> InsertHoSoChamCong(HoSoChamCongModel model, Guid idNhanVien, Guid idDonVi)
        {
            var resul = new JsonViewModel<string> { ErrorCode = false };
            var listPhieu = (from pnv in _dbcontext.NS_PhieuPhanCa_NhanVien
                             join p in _dbcontext.NS_PhieuPhanCa
                             on pnv.ID_PhieuPhanCa equals p.ID
                             join pc in _dbcontext.NS_PhieuPhanCa_CaLamViec
                             on p.ID equals pc.ID_PhieuPhanCa
                             where model.ListPhieuPhanCa.Contains(p.ID)
                             select p);
            if (!listPhieu.Any())
            {
                resul.Data = "Phiếu phân ca chưa chọn ca hoặc chưa chọn nhân viên";
                return resul;
            }
            var nguoitao = GetNguoiTao(idNhanVien);
            var KyTinhCong = _dbcontext.NS_KyTinhCong.FirstOrDefault(o => o.ID == model.IDKyTinhCong);
            if (KyTinhCong == null)
            {
                resul.Data = "Kỳ Tính công không tồn tại, vui lòng thử lại sau";
                return resul;
            }

            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("KyTinhCong", KyTinhCong.ID));
            sql.Add(new SqlParameter("Thang", KyTinhCong.TuNgay.Month));
            sql.Add(new SqlParameter("Nam", KyTinhCong.TuNgay.Year));
            sql.Add(new SqlParameter("TaiKhoan", nguoitao.TaiKhoan));
            sql.Add(new SqlParameter("ListPhieuPhanCa", string.Join(",", listPhieu.Select(o => o.ID).Distinct().ToList())));
            int CheckStore = _dbcontext.Database.SqlQuery<int>("exec InsertHoSoChamCong @KyTinhCong, @Thang,@Nam,@TaiKhoan,@ListPhieuPhanCa", sql.ToArray()).FirstOrDefault();
            if (CheckStore >= 0)
            {
                var listMaPhieu = listPhieu.Select(o => o.MaPhieu).Distinct().ToList();
                string noidung = "Thêm hồ sơ châm công - Kỳ tính công: " + KyTinhCong.Ky;
                string chitiet = "Thêm hồ sơ châm công:  Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                   "<br /> Thời gian kỳ tính công: Từ " + KyTinhCong.TuNgay.ToString("dd/MM/yyyy") + " đến " + KyTinhCong.DenNgay.ToString("dd/MM/yyyy") +
                                  "<br /> Loại công: Chấm thủ công " +
                                   "<br /> Danh sách phiếu phân ca áp dụng: " + string.Join(",", listMaPhieu) +
                                    "<br /> Trạng thái: Áp dụng ";

                InsertNhatKySuDung("Chấm công", noidung, chitiet, idDonVi, idNhanVien, 1);
                resul.ErrorCode = true;
            }
            else
            {
                resul.Data = "Đã xảy ra lỗi trong quá trình xử lý, vui lòng thử lại sau";
            }

            return resul;
        }

        public IEnumerable<ChamCongModel> GetListForChamCong(AddCongModel model, bool IsNew)
        {
            var listSearch = GetChamCongFilter(model.Search);
            switch (model.Ngay)
            {
                case 1:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay1 == null || o.Ngay1 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay1 != null && o.Ngay1 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay1 == "0"); // get những phiếu đã dc phân ca trong khoảng from - to
                    break;
                case 2:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay2 == null || o.Ngay2 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay2 != null && o.Ngay2 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay2 == "0");
                    break;
                case 3:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay3 == null || o.Ngay3 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay3 != null && o.Ngay3 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay3 == "0");
                    break;
                case 4:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay4 == null || o.Ngay4 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay4 != null && o.Ngay4 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay4 == "0");
                    break;
                case 5:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay5 == null || o.Ngay5 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay5 != null && o.Ngay5 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay5 == "0");
                    break;
                case 6:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay6 == null || o.Ngay6 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay6 != null && o.Ngay6 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay6 == "0");
                    break;
                case 7:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay7 == null || o.Ngay7 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay7 != null && o.Ngay7 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay7 == "0");
                    break;
                case 8:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay8 == null || o.Ngay8 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay8 != null && o.Ngay8 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay8 == "0");
                    break;
                case 9:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay9 == null || o.Ngay9 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay9 != null && o.Ngay9 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay9 == "0");
                    break;
                case 10:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay10 == null || o.Ngay10 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay10 != null && o.Ngay10 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay10 == "0");
                    break;
                case 11:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay11 == null || o.Ngay11 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay11 != null && o.Ngay11 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay11 == "0");
                    break;
                case 12:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay12 == null || o.Ngay12 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay12 != null && o.Ngay12 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay12 == "0");
                    break;
                case 13:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay13 == null || o.Ngay13 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay13 != null && o.Ngay13 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay13 == "0");
                    break;
                case 14:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay14 == null || o.Ngay14 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay14 != null && o.Ngay14 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay14 == "0");
                    break;
                case 15:
                    if (model.ApDungToanNhanVien == false)
                    {

                    }
                    if (IsNew)
                    {
                        listSearch = listSearch.Where(o => o.Ngay15 == null || o.Ngay15 == "");
                    }
                    else
                    {
                        listSearch = listSearch.Where(o => o.Ngay15 != null && o.Ngay15 != "");
                    }
                    listSearch = listSearch.Where(x => x.DisNgay15 == "0");
                    break;
                case 16:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay16 == null || o.Ngay16 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay16 != null && o.Ngay16 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay16 == "0");
                    break;
                case 17:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay17 == null || o.Ngay17 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay17 != null && o.Ngay17 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay17 == "0");
                    break;
                case 18:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay18 == null || o.Ngay18 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay18 != null && o.Ngay18 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay18 == "0");
                    break;
                case 19:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay19 == null || o.Ngay19 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay19 != null && o.Ngay19 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay19 == "0");
                    break;
                case 20:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay20 == null || o.Ngay20 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay20 != null && o.Ngay20 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay20 == "0");
                    break;
                case 21:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay21 == null || o.Ngay21 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay21 != null && o.Ngay21 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay21 == "0");
                    break;
                case 22:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay22 == null || o.Ngay22 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay22 != null && o.Ngay22 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay22 == "0");
                    break;
                case 23:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay23 == null || o.Ngay23 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay23 != null && o.Ngay23 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay23 == "0");
                    break;
                case 24:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay24 == null || o.Ngay24 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay24 != null && o.Ngay24 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay24 == "0");
                    break;
                case 25:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay25 == null || o.Ngay25 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay25 != null && o.Ngay25 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay25 == "0");
                    break;
                case 26:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay26 == null || o.Ngay26 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay26 != null && o.Ngay26 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay26 == "0");
                    break;
                case 27:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay27 == null || o.Ngay27 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay27 != null && o.Ngay27 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay27 == "0");
                    break;
                case 28:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay28 == null || o.Ngay28 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay28 != null && o.Ngay28 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay28 == "0");
                    break;
                case 29:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay29 == null || o.Ngay29 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay29 != null && o.Ngay29 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay29 == "0");
                    break;
                case 30:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay30 == null || o.Ngay30 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay30 != null && o.Ngay30 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay30 == "0");
                    break;
                case 31:
                    if (model.ApDungToanNhanVien == false)
                    {
                        if (IsNew)
                        {
                            listSearch = listSearch.Where(o => o.Ngay31 == null || o.Ngay31 == "");
                        }
                        else
                        {
                            listSearch = listSearch.Where(o => o.Ngay31 != null && o.Ngay31 != "");
                        }
                    }
                    listSearch = listSearch.Where(x => x.DisNgay31 == "0");
                    break;
            }
            return listSearch;
        }

        public CongQuyDoiDTO GetCongQuyDoi_ByIDCaLam_ofNhanVien(NgayNghiLeDTO ngayle, Guid idNhanVien, Guid idCaLamViec, Guid idDonVi)
        {
            double congQD = 1;
            double congOTQD = 1;
            var congquydoi = GetCongQuyDoi3(ngayle.DateOfWeek, ngayle.LoaiNgay, idNhanVien.ToString(), idDonVi, ngayle.NgayChamCong);
            if (congquydoi.Count() > 0)
            {
                var congTheoCa = congquydoi.Where(x => x.ID_CaLamViec == idCaLamViec && x.NgayApDung <= ngayle.NgayChamCong && (x.NgayKetThuc == null || x.NgayKetThuc >= ngayle.NgayChamCong)).FirstOrDefault();
                if (congTheoCa == null)
                {
                    congTheoCa = congquydoi.Where(x => x.ID_CaLamViec == Guid.Empty && x.NgayApDung <= ngayle.NgayChamCong && (x.NgayKetThuc == null || x.NgayKetThuc >= ngayle.NgayChamCong)).FirstOrDefault();
                }
                if (congTheoCa != null)
                {
                    congQD = congTheoCa.CongQuyDoi;
                    congOTQD = congTheoCa.CongOTQuyDoi;
                }
            }
            return new CongQuyDoiDTO
            {
                CongQuyDoi = congQD,
                CongOTQuyDoi = congOTQD,
                ID_CaLamViec = idCaLamViec,
                ID_NhanVien = idNhanVien,
            };
        }

        public NgayNghiLeDTO GetNgayNghiLe(AddCongModel model)
        {
            DateTime ngayCham = new DateTime(model.Nam, model.Thang, model.Ngay);
            int dateOfWeek = (int)ngayCham.DayOfWeek;
            int loaingay = 0;
            var ngaynghi = _dbcontext.NS_NgayNghiLe.Where(x => x.Ngay != null
               && x.Ngay.Value.Year == model.Nam && x.Ngay.Value.Month == model.Thang && x.Ngay.Value.Day == model.Ngay);
            if (ngaynghi == null || ngaynghi.Count() == 0)
            {
                ngaynghi = _dbcontext.NS_NgayNghiLe.Where(x => x.Ngay == null && x.Thu == (int)ngayCham.DayOfWeek);
                if (ngaynghi != null && ngaynghi.Count() > 0)
                {
                    loaingay = ngaynghi.FirstOrDefault().LoaiNgay;
                }
            }
            else
            {
                loaingay = ngaynghi.FirstOrDefault().LoaiNgay;
            }
            return new NgayNghiLeDTO
            {
                NgayChamCong = ngayCham,
                DateOfWeek = dateOfWeek,
                LoaiNgay = loaingay,
            };
        }

        public void UpdateStatusBangLuong_whenChangeCong(Guid idDonVi, DateTime ngayCham)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
            lstParam.Add(new SqlParameter("NgayChamCong", ngayCham));
            _dbcontext.Database.SqlQuery<CongQuyDoiDTO>("UpdateStatusBangLuong_whenChangeCong @ID_DonVi, @NgayChamCong", lstParam.ToArray()).ToList();
        }

        public List<CongBoSungQuyDoiDTO> GetQuyDoi_ofCongBoSung(Guid idPhuCap, Guid idNhanVien, Guid idDonVi)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_PhuCap", idPhuCap));
            lstParam.Add(new SqlParameter("ID_NhanVien", idNhanVien));
            lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
            var data = _dbcontext.Database.SqlQuery<CongBoSungQuyDoiDTO>("GetQuyDoi_ofCongBoSung @ID_PhuCap, @ID_NhanVien, @ID_DonVi", lstParam.ToArray()).ToList();
            return data;
        }


        public List<CongQuyDoiDTO> GetCongQuyDoi3(int dateOfWeek, int loaingay, string idNhanVien, Guid idDonVi, DateTime? ngayCham)
        {
            List<CongQuyDoiDTO> data = new List<CongQuyDoiDTO>();
            if (ngayCham != null)
            {
                var staffID = "%%";
                if (idNhanVien != string.Empty)
                {
                    staffID = idNhanVien;
                }
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("DateOfWeek", dateOfWeek));
                lstParam.Add(new SqlParameter("LoaiNgay", loaingay));
                lstParam.Add(new SqlParameter("NgayCham", ngayCham));
                lstParam.Add(new SqlParameter("ID_NhanVien", staffID));
                lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                data = _dbcontext.Database.SqlQuery<CongQuyDoiDTO>("GetCongQuyDoi @DateOfWeek, @LoaiNgay, @NgayCham, @ID_NhanVien, @ID_DonVi", lstParam.ToArray()).ToList();
            }
            return data;
        }

        public void ChangeCong_UpdateNSCongBoSung(AddCongModel model, DateTime ngayCham, int dateOfWeek, int loaingay, string nguoitao)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_DonVi", model.ID_DonVi));
            lstParam.Add(new SqlParameter("NgayChamCong", ngayCham));
            lstParam.Add(new SqlParameter("ID_CaLamViec", model.Search.ID_CaLamViec));
            lstParam.Add(new SqlParameter("DateOfWeek", dateOfWeek));
            lstParam.Add(new SqlParameter("LoaiNgay", loaingay));
            lstParam.Add(new SqlParameter("KyHieuCong", model.KyHieuCong));
            lstParam.Add(new SqlParameter("Cong", model.Cong));
            lstParam.Add(new SqlParameter("SoGioOT", model.SoGioOT));
            lstParam.Add(new SqlParameter("SoPhutDiMuon", model.SoPhutDiMuon));
            lstParam.Add(new SqlParameter("GhiChu", model.GhiChu == null ? string.Empty : model.GhiChu));
            lstParam.Add(new SqlParameter("NguoiTao", nguoitao));
            lstParam.Add(new SqlParameter("TrangThai", (int)commonEnumHellper.eBoSungCong.taomoi));
            _dbcontext.Database.ExecuteSqlCommand("EXEC ChangeCong_UpdateNSCongBoSung @ID_DonVi, @NgayChamCong, @ID_CaLamViec, @DateOfWeek, @LoaiNgay, @KyHieuCong, @Cong," +
                  "@SoGioOT, @SoPhutDiMuon, @GhiChu, @NguoiTao, @TrangThai", lstParam.ToArray());
        }

        public ChamCongDTO GetCongQuyDoi(AddCongModel model, Guid idNhanVien)
        {
            var result = new JsonViewModel<Object> { ErrorCode = true };
            try
            {
                var ngay = model.Ngay;
                var thang = model.Thang;
                var nam = model.Nam;
                var kyhieucong = model.KyHieuCong.ToUpper();
                var loaingaynghi = 0;
                double congQuyDoi = 1;
                double congOTQuyDoi = 0;
                double ptramluong = 1;
                double ptramluongOT = 1;

                var nguoiTao = GetNguoiTao(idNhanVien);
                var ngayCham = new DateTime(nam, thang, ngay);
                var dateOfWeek = (int)ngayCham.DayOfWeek;

                var ngaynghi = _dbcontext.NS_NgayNghiLe.Where(x => x.Ngay != null
                && x.Ngay.Value.Year == nam && x.Ngay.Value.Month == thang && x.Ngay.Value.Day == ngay);
                if (ngaynghi == null || ngaynghi.Count() == 0)
                {
                    ngaynghi = _dbcontext.NS_NgayNghiLe.Where(x => x.Ngay == null && x.Thu == dateOfWeek);
                }
                if (ngaynghi != null && ngaynghi.Count() > 0)
                {
                    var itFirst = ngaynghi.FirstOrDefault();
                    congQuyDoi = model.Cong * ptramluong;// làm 1 công thì dc? công
                    congOTQuyDoi = model.Cong == 0 ? 0 : model.SoGioOT * ptramluongOT;// không liên quan đến congQD: OT 1 giờ thì dc ? % công
                    loaingaynghi = itFirst.LoaiNgay;
                }

                ChamCongDTO obj = new ChamCongDTO
                {
                    Ngay = ngay,
                    Thang = thang,
                    Nam = nam,
                    KyHieuCong = kyhieucong,
                    CongQuyDoi = congQuyDoi,
                    GioOTQuyDoi = congOTQuyDoi,
                    LoaiNgay = loaingaynghi,
                    NgayChamCong = ngayCham,
                    DateOfWeek = dateOfWeek,
                    Cong = model.Cong,
                    SoGioOT = model.SoGioOT,
                    SoPhutDiMuon = model.SoPhutDiMuon,
                    GhiChu = model.GhiChu,
                    NguoiTao = nguoiTao.TaiKhoan,
                    PhanTramLuong = ptramluong, // use save at NS_CongBoSung (%luong ngay nghi le)
                    PhanTramLuongOT = ptramluongOT,
                };
                return obj;
            }
            catch (Exception)
            {
                return new ChamCongDTO();
            }
        }

        public void UpdateCong_WhenChangeThietLapLuong(Guid idPhuCap, Guid idNhanVien, Guid idDonVi)
        {
            List<Guid> lstIDCongBS = new List<Guid>();

            List<CongBoSungQuyDoiDTO> congBS = GetQuyDoi_ofCongBoSung(idPhuCap, idNhanVien, idDonVi);
            foreach (var item in congBS)
            {
                if (lstIDCongBS.Any(x => x == item.ID_CongBoSung) == false)
                {
                    var objUp = _dbcontext.NS_CongBoSung.Find(item.ID_CongBoSung);
                    double congquydoi = 1;
                    double otquydoi = 1;
                    var quydoiByCa = congBS.Where(x => x.ID_CongBoSung == objUp.ID && x.ID_CaLamViec == objUp.ID_CaLamViec);
                    CongBoSungQuyDoiDTO itFirst = new CongBoSungQuyDoiDTO();
                    if (quydoiByCa != null && quydoiByCa.Count() > 0)
                    {
                        itFirst = quydoiByCa.FirstOrDefault();
                        if (itFirst.LaPTCongQuyDoi == 1)
                        {
                            congquydoi = itFirst.CongQuyDoi / 100 * objUp.Cong;
                        }
                        else
                        {
                            congquydoi = objUp.Cong;
                        }
                        if (itFirst.LaPhanTramOTQuyDoi == 1)
                        {
                            otquydoi = itFirst.CongOTQuyDoi / 100 * objUp.SoGioOT;
                        }
                        else
                        {
                            otquydoi = objUp.SoGioOT;
                        }
                    }
                    else
                    {
                        var quydoiDefault = congBS.Where(x => x.ID_CongBoSung == objUp.ID && x.ID_CaLamViec == Guid.Empty);
                        itFirst = quydoiDefault.FirstOrDefault();

                        if (itFirst.LaPTCongQuyDoi == 1)
                        {
                            congquydoi = itFirst.CongQuyDoi / 100 * objUp.Cong;
                        }
                        else
                        {
                            congquydoi = objUp.Cong;
                        }
                        if (itFirst.LaPhanTramOTQuyDoi == 1)
                        {
                            otquydoi = itFirst.CongOTQuyDoi / 100 * objUp.SoGioOT;
                        }
                        else
                        {
                            otquydoi = objUp.SoGioOT;
                        }
                    }
                    objUp.CongQuyDoi = congquydoi;
                    objUp.GioOTQuyDoi = otquydoi;
                    lstIDCongBS.Add(objUp.ID);
                }
            }
            _dbcontext.SaveChanges();
        }

        public void UpdateCong_WhenChangeNgayNghiLe(Guid idDonVi, int dateOfWeekOld, int loaiNgayOld, int loaiNgayNew, DateTime? ngay)
        {
            var cong = (from bs in _dbcontext.NS_CongBoSung
                        where bs.ID_DonVi == idDonVi
                        && bs.Thu == dateOfWeekOld && bs.LoaiNgay == loaiNgayOld
                        && (bs.TrangThai == 1 || bs.TrangThai == 2)
                        select new
                        {
                            bs.ID,
                            bs.NgayCham,
                            bs.Cong,
                            bs.SoGioOT,
                            bs.ID_CaLamViec,
                            bs.ID_NhanVien,
                        }).ToList();
            if (ngay != null)
            {
                // get cong in thisday
                cong = cong.Where(x => x.NgayCham.Year == ngay.Value.Year && x.NgayCham.Month == ngay.Value.Month && x.NgayCham.Day == ngay.Value.Day).ToList();
            }
            var nvExist = cong.Select(x => x.ID_NhanVien);
            var quydoi = GetCongQuyDoi3(dateOfWeekOld, loaiNgayNew, string.Empty, idDonVi, ngay).Where(x => nvExist.Contains(x.ID_NhanVien));
            foreach (var item in cong)
            {
                // get quydoi with ca + nv
                double congquydoi = 1;
                double otquydoi = 1;
                var quydoiOfNV = quydoi.Where(x => x.ID_NhanVien == item.ID_NhanVien);
                if (quydoiOfNV != null && quydoiOfNV.Count() > 0)
                {
                    var quydoiCa = quydoiOfNV.Where(x => x.ID_CaLamViec == item.ID_CaLamViec && x.NgayApDung <= item.NgayCham && (x.NgayKetThuc == null || x.NgayKetThuc >= item.NgayCham));
                    if (quydoiCa != null && quydoiCa.Count() > 0)
                    {
                        congquydoi = quydoiCa.FirstOrDefault().CongQuyDoi * item.Cong;
                        otquydoi = quydoiCa.FirstOrDefault().CongOTQuyDoi * item.SoGioOT;
                    }
                    else
                    {
                        var qdDefault = quydoiOfNV.Where(x => x.ID_CaLamViec == Guid.Empty && x.NgayApDung <= item.NgayCham && (x.NgayKetThuc == null || x.NgayKetThuc >= item.NgayCham)).FirstOrDefault();
                        if (qdDefault != null)
                        {
                            congquydoi = qdDefault.CongQuyDoi;
                            otquydoi = qdDefault.CongOTQuyDoi;
                        }
                    }
                }

                var objUp = _dbcontext.NS_CongBoSung.Find(item.ID);
                objUp.CongQuyDoi = congquydoi;
                objUp.GioOTQuyDoi = otquydoi;
                objUp.LoaiNgay = loaiNgayNew;
            }
        }

        public JsonViewModel<string> ApplyAllChamCong(AddCongModel model, Guid idNhanVien, Guid idDonVi)
        {
            var result = new JsonViewModel<string> { ErrorCode = false };
            try
            {
                var nguoiTao = GetNguoiTao(idNhanVien).TaiKhoan;
                var ngayle = GetNgayNghiLe(model);
                List<CongQuyDoiDTO> congquydoi = GetCongQuyDoi3(ngayle.DateOfWeek, ngayle.LoaiNgay, string.Empty, idDonVi, ngayle.NgayChamCong);

                List<NS_CongBoSung> bangcong = new List<NS_CongBoSung>();
                List<ChamCongModel> hosocong = GetListForChamCong(model, model.IsNew).ToList();

                // get list congqydoi with this calamviec
                var quydoihasCa = congquydoi.Where(x => x.ID_CaLamViec == model.Search.ID_CaLamViec);
                var nvExist = quydoihasCa.Select(x => x.ID_NhanVien);
                // only get if not exist nvien
                var quydoiNotThisCa = congquydoi.Where(x => x.ID_CaLamViec != model.Search.ID_CaLamViec && nvExist.Contains(x.ID_NhanVien) == false).ToList();
                var dataUnion = quydoihasCa.Union(quydoiNotThisCa);

                // delete NS_CongBoSung with same NgayCham
                var lstCong = _dbcontext.NS_CongBoSung.Where(x => x.ID_DonVi == idDonVi && x.ID_CaLamViec == model.Search.ID_CaLamViec
                 && x.NgayCham.Year == model.Nam && x.NgayCham.Month == model.Thang && x.NgayCham.Day == model.Ngay
                 && x.TrangThai != (int)commonEnumHellper.eBoSungCong.duyet && x.TrangThai != (int)commonEnumHellper.eBoSungCong.dathanhtoan);
                _dbcontext.NS_CongBoSung.RemoveRange(lstCong);

                // quydoicong
                var dataX = (from hs in hosocong
                             join qd in dataUnion on hs.ID_NhanVien equals qd.ID_NhanVien
                             into CongQD
                             from hsqd in CongQD.DefaultIfEmpty()
                             select new
                             {
                                 ID_NhanVien = hs.ID_NhanVien,
                                 ID_CaLamViec = hs.ID_CaLamViec,
                                 CongQuyDoi = hsqd == null ? 1 :
                                 hs.ID_CaLamViec == hsqd.ID_CaLamViec && hsqd.NgayApDung <= ngayle.NgayChamCong && (hsqd.NgayKetThuc == null || hsqd.NgayKetThuc >= ngayle.NgayChamCong) ? hsqd.CongQuyDoi :
                                 CongQD.Where(x => x.ID_CaLamViec == Guid.Empty && x.NgayApDung <= ngayle.NgayChamCong && (x.NgayKetThuc == null || x.NgayKetThuc >= ngayle.NgayChamCong)).Count() > 0 ?
                                 CongQD.Where(x => x.ID_CaLamViec == Guid.Empty && x.NgayApDung <= ngayle.NgayChamCong && (x.NgayKetThuc == null || x.NgayKetThuc >= ngayle.NgayChamCong)).FirstOrDefault().CongQuyDoi : 1
                                                ,
                                 CongOTQuyDoi = hsqd == null ? 1 : hs.ID_CaLamViec == hsqd.ID_CaLamViec && hsqd.NgayApDung <= ngayle.NgayChamCong && (hsqd.NgayKetThuc == null || hsqd.NgayKetThuc >= ngayle.NgayChamCong) ? hsqd.CongOTQuyDoi :
                                 CongQD.Where(x => x.ID_CaLamViec == Guid.Empty && x.NgayApDung <= ngayle.NgayChamCong && (x.NgayKetThuc == null || x.NgayKetThuc >= ngayle.NgayChamCong)).Count() > 0 ?
                                                 CongQD.Where(x => x.ID_CaLamViec == Guid.Empty && x.NgayApDung <= ngayle.NgayChamCong && (x.NgayKetThuc == null || x.NgayKetThuc >= ngayle.NgayChamCong)).FirstOrDefault().CongOTQuyDoi : 1,
                             }).Distinct().ToList();

                // insert again cong
                foreach (var item in dataX)
                {
                    NS_CongBoSung congnv = new NS_CongBoSung()
                    {
                        ID = Guid.NewGuid(),
                        ID_ChamCongChiTiet = Guid.NewGuid(), // tam gán để tránh bị lỗi: not use
                        ID_NhanVien = item.ID_NhanVien,
                        ID_CaLamViec = item.ID_CaLamViec,
                        ID_DonVi = model.ID_DonVi,
                        ID_MayChamCong = null,
                        GioVao = null,
                        GioRa = null,
                        GioVaoOT = null,
                        GioRaOT = null,
                        Cong = model.Cong,
                        KyHieuCong = model.KyHieuCong,
                        NgayCham = ngayle.NgayChamCong,
                        NgayTao = DateTime.Now,
                        SoGioOT = model.SoGioOT,
                        SoPhutDiMuon = model.SoPhutDiMuon,
                        LoaiNgay = ngayle.LoaiNgay,
                        GioOTQuyDoi = model.SoGioOT * item.CongOTQuyDoi,
                        CongQuyDoi = model.Cong * item.CongQuyDoi,
                        Thu = ngayle.DateOfWeek,
                        TrangThai = (int)commonEnumHellper.eBoSungCong.taomoi,
                        NguoiTao = nguoiTao,
                        GhiChu = model.GhiChu,
                    };
                    bangcong.Add(congnv);
                }
                _dbcontext.NS_CongBoSung.AddRange(bangcong);
                UpdateStatusBangLuong_whenChangeCong(idDonVi, ngayle.NgayChamCong);
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
                result.ErrorCode = true;
                result.Data = e.InnerException + e.Message + e.Data + e.Source;
            }
            return result;
        }

        public void UpdateKyHieuCong_NSChamCong(Guid id, int ngay, string kyhieu)
        {
            var Sql = string.Empty;
            switch (ngay)
            {
                case 1:
                    Sql += " Ngay1 =";
                    break;
                case 2:
                    Sql += " Ngay2=";
                    break;
                case 3:
                    Sql += " Ngay3=";
                    break;
                case 4:
                    Sql += " Ngay4=";
                    break;
                case 5:
                    Sql += " Ngay5=";
                    break;
                case 6:
                    Sql += " Ngay6=";
                    break;
                case 7:
                    Sql += " Ngay7=";
                    break;
                case 8:
                    Sql += " Ngay8=";
                    break;
                case 9:
                    Sql += " Ngay9=";
                    break;
                case 10:
                    Sql += " Ngay10=";
                    break;
                case 11:
                    Sql += " Ngay11=";
                    break;
                case 12:
                    Sql += " Ngay12=";
                    break;
                case 13:
                    Sql += " Ngay13=";
                    break;
                case 14:
                    Sql += " Ngay14=";
                    break;
                case 15:
                    Sql += " Ngay15=";
                    break;
                case 16:
                    Sql += " Ngay16=";
                    break;
                case 17:
                    Sql += " Ngay17=";
                    break;
                case 18:
                    Sql += " Ngay18=";
                    break;
                case 19:
                    Sql += " Ngay19=";
                    break;
                case 20:
                    Sql += " Ngay20=";
                    break;
                case 21:
                    Sql += " Ngay21=";
                    break;
                case 22:
                    Sql += " Ngay22=";
                    break;
                case 23:
                    Sql += " Ngay23=";
                    break;
                case 24:
                    Sql += " Ngay24=";
                    break;
                case 25:
                    Sql += " Ngay25=";
                    break;
                case 26:
                    Sql += " Ngay26=";
                    break;
                case 27:
                    Sql += " Ngay27=";
                    break;
                case 28:
                    Sql += " Ngay28=";
                    break;
                case 29:
                    Sql += " Ngay29=";
                    break;
                case 30:
                    Sql += " Ngay30=";
                    break;
                default:
                    Sql += " Ngay31=";
                    break;
            }
            Sql = string.Concat("Update NS_ChamCong_ChiTiet SET ", Sql, "'", kyhieu, "' WHERE ID= '", id, "'");
            _dbcontext.Database.ExecuteSqlCommand(Sql);
        }

        public void UpDateSqlComand(AddCongModel model, int loaiNgay)
        {
            string Sql = string.Empty;
            switch (model.Ngay)
            {
                case 1:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay1=";
                    break;
                case 2:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay2=";
                    break;
                case 3:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay3=";
                    break;
                case 4:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay4=";
                    break;
                case 5:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay5=";
                    break;
                case 6:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay6=";
                    break;
                case 7:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay7=";
                    break;
                case 8:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay8=";
                    break;
                case 9:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay9=";
                    break;
                case 10:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay10=";
                    break;
                case 11:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay11=";
                    break;
                case 12:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay12=";
                    break;
                case 13:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay13=";
                    break;
                case 14:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay14=";
                    break;
                case 15:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay15=";
                    break;
                case 16:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay16=";
                    break;
                case 17:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay17=";
                    break;
                case 18:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay18=";
                    break;
                case 19:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay19=";
                    break;
                case 20:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay20=";
                    break;
                case 21:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay21=";
                    break;
                case 22:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay22=";
                    break;
                case 23:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay23=";
                    break;
                case 24:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay24=";
                    break;
                case 25:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay25=";
                    break;
                case 26:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay26=";
                    break;
                case 27:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay27=";
                    break;
                case 28:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay28=";
                    break;
                case 29:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay29=";
                    break;
                case 30:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay30=";
                    break;
                default:
                    Sql += "Update NS_ChamCong_ChiTiet Set Ngay31=";
                    break;
            }
            string SumSql = "";
            if (loaiNgay == (int)commonEnumHellper.LoaiNgaynghiLe.ngaythuong)
            {
                SumSql = "Update NS_ChamCong_ChiTiet Set TongCong=(select SUM(bsc.Cong) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay = 0 ) ," +
                             "TongCongQuyDoi=(select SUM(bsc.CongQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay = 0 )," +
                             "PhutDiMuon=(select SUM(bsc.SoPhutDiMuon) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID)," +
                             "SoGioOT=(select SUM(bsc.SoGioOT) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID), " +
                             "TongGioOTQuyDoi=(select SUM(bsc.GioOTQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID) ";

            }
            else
            {
                SumSql = "Update NS_ChamCong_ChiTiet Set  TongCongOTNgayNghi=(select SUM(bsc.Cong) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay != 0 )," +
                           "TongCongOTNgayNghiQuyDoi=(select SUM(bsc.CongQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay != 0 )," +
                           "PhutDiMuon=(select SUM(bsc.SoPhutDiMuon) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID)," +
                           "SoGioOT=(select SUM(bsc.SoGioOT) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID), " +
                           "TongGioOTQuyDoi=(select SUM(bsc.GioOTQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID) ";

            }

            Sql += "'" + model.KyHieuCong + "' Where ID in (SELECT  cc.ID FROM NS_ChamCong_ChiTiet cc" +
                                     " join (select DISTINCT nv.ID  from NS_NhanVien nv" +
                                        " join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien ";
            SumSql += "Where ID in (SELECT  cc.ID FROM NS_ChamCong_ChiTiet cc" +
                                     " join (select DISTINCT nv.ID  from NS_NhanVien nv" +
                                        " join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien ";
            if (model.Search.PhonBanId == null)
            {
                Sql += "Where ct.ID_DonVi='" + model.Search.ListDonVi.FirstOrDefault().ToString() + "'";
                SumSql += "Where ct.ID_DonVi='" + model.Search.ListDonVi.FirstOrDefault().ToString() + "'";

            }
            else
            {
                var listPhongBan = GetListPhongbanById(model.Search.PhonBanId ?? new Guid());

                Sql += "Where ct.ID_PhongBan in (" + string.Join(",", listPhongBan.Select(o => "'" + o + "'")) + ")";
                SumSql += "Where ct.ID_PhongBan in (" + string.Join(",", listPhongBan.Select(o => "'" + o + "'")) + ")";

            }
            if (!string.IsNullOrWhiteSpace(model.Search.Text))
            {
                Sql += " and (UPPER(nv.MaNhanVien) like N'%" + model.Search.Text + "%'" +
                                             "or UPPER(nv.TenNhanVien) like N'%" + model.Search.Text + "%'" +
                                             "or UPPER(nv.TenNhanVienChuCaiDau) like N'%" + model.Search.Text + "%'" +
                                             "or UPPER(nv.TenNhanVienKhongDau) like N'%" + model.Search.Text + "%')";
                SumSql += " and (UPPER(nv.MaNhanVien) like N'%" + model.Search.Text + "%'" +
                                             "or UPPER(nv.TenNhanVien) like N'%" + model.Search.Text + "%'" +
                                             "or UPPER(nv.TenNhanVienChuCaiDau) like N'%" + model.Search.Text + "%'" +
                                             "or UPPER(nv.TenNhanVienKhongDau) like N'%" + model.Search.Text + "%')";
            }
            Sql += ") us on cc.ID_NhanVien = us.ID where cc.ID_KyTinhCong ='" + model.Search.KyTinhCongId.ToString() + "' and cc.TrangThai != 0)";
            SumSql += ") us on cc.ID_NhanVien = us.ID where cc.ID_KyTinhCong ='" + model.Search.KyTinhCongId.ToString() + "' and cc.TrangThai != 0)";
            _dbcontext.Database.ExecuteSqlCommand(Sql);
            _dbcontext.Database.ExecuteSqlCommand(SumSql);
        }

        public JsonViewModel<string> AddChamThuCong(AddCongModel model, Guid idNhanVien, Guid idDonVi)
        {
            var result = new JsonViewModel<string> { ErrorCode = false };
            try
            {
                var nguoiTao = GetNguoiTao(idNhanVien).TaiKhoan;
                var id_nhanvien = model.Search.IDNhanVien ?? Guid.Empty;
                var id_calam = model.Search.ID_CaLamViec ?? Guid.Empty;
                NgayNghiLeDTO ngayle = GetNgayNghiLe(model);

                var congquydoi = GetCongQuyDoi_ByIDCaLam_ofNhanVien(ngayle, id_nhanvien, id_calam, idDonVi);
                double congQD = congquydoi.CongQuyDoi;
                double congOTQD = congquydoi.CongOTQuyDoi;

                // remove cong: used to xoacong sau do add lai
                var lstCong = _dbcontext.NS_CongBoSung.Where(x => x.ID_NhanVien == id_nhanvien && x.ID_DonVi == idDonVi
                  && x.NgayCham.Year == model.Nam && x.NgayCham.Month == model.Thang && x.NgayCham.Day == model.Ngay
                && x.ID_CaLamViec == model.ID_CaLamViec
                && x.TrangThai != (int)commonEnumHellper.eBoSungCong.duyet && x.TrangThai != (int)commonEnumHellper.eBoSungCong.dathanhtoan);
                _dbcontext.NS_CongBoSung.RemoveRange(lstCong);

                // insert into NS_CongBoSung
                NS_CongBoSung congnv = new NS_CongBoSung()
                {
                    ID = Guid.NewGuid(),
                    ID_ChamCongChiTiet = Guid.NewGuid(),
                    ID_NhanVien = model.ID_NhanVien,
                    ID_CaLamViec = model.ID_CaLamViec,
                    ID_DonVi = model.ID_DonVi,
                    ID_MayChamCong = null,
                    GioVao = null,
                    GioRa = null,
                    GioVaoOT = null,
                    GioRaOT = null,
                    Cong = model.Cong,
                    KyHieuCong = model.KyHieuCong,
                    NgayCham = ngayle.NgayChamCong,
                    NgayTao = DateTime.Now,
                    SoGioOT = model.SoGioOT,
                    SoPhutDiMuon = model.SoPhutDiMuon,
                    LoaiNgay = ngayle.LoaiNgay,
                    GioOTQuyDoi = congOTQD * model.SoGioOT,
                    CongQuyDoi = congQD * model.Cong,
                    Thu = ngayle.DateOfWeek,
                    TrangThai = (int)commonEnumHellper.eBoSungCong.taomoi,
                    NguoiTao = nguoiTao,
                    GhiChu = model.GhiChu,
                };
                _dbcontext.NS_CongBoSung.Add(congnv);
                UpdateStatusBangLuong_whenChangeCong(idDonVi, ngayle.NgayChamCong);
                _dbcontext.SaveChanges();
            }
            catch (Exception e)
            {
                result.ErrorCode = true;
                result.Data = e.InnerException + e.Message;
            }
            return result;
        }

        public JsonViewModel<Object> XoaCong_CheckExistBangLuong(Guid idNhanVien, DateTime fromdate, DateTime todate)
        {
            var result = new JsonViewModel<Object>() { ErrorCode = true };
            var data = (from bl in _dbcontext.NS_BangLuong
                        join ct in _dbcontext.NS_BangLuong_ChiTiet on bl.ID equals ct.ID_BangLuong
                        where ct.ID_NhanVien == idNhanVien
                        && bl.TrangThai != (int)commonEnumHellper.eBangLuong.xoa
                         && ((fromdate <= bl.TuNgay && (todate >= bl.TuNgay || todate > bl.DenNgay))
                                || (todate >= bl.DenNgay && (fromdate <= bl.DenNgay || fromdate <= bl.TuNgay)))
                        select new { bl.MaBangLuong, bl.TrangThai }).ToList();
            result.Data = data;
            result.ErrorCode = false;
            return result;
        }

        public void RemoveCong_ofNhanVien(Guid idChiNhanh, Guid idNhanVien, Guid idCaLamViec, string fromdate, string todate)
        {
            List<SqlParameter> paramSQL = new List<SqlParameter>();
            paramSQL.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            paramSQL.Add(new SqlParameter("ID_NhanVien", idNhanVien));
            paramSQL.Add(new SqlParameter("ID_CaLamViec", idCaLamViec));
            paramSQL.Add(new SqlParameter("FromDate", fromdate));
            paramSQL.Add(new SqlParameter("ToDate", todate));
            _dbcontext.Database.ExecuteSqlCommand("EXEC RemoveCong_ofNhanVien @ID_ChiNhanh, @ID_NhanVien, @ID_CaLamViec, @FromDate, @ToDate", paramSQL.ToArray());
        }

        public bool CheckCong_ExistBangLuong(Guid idChiNhanh, int ngay, int thang, int nam)
        {
            DateTime ngayCham = new DateTime(nam, thang, ngay);
            var exist = (from bl in _dbcontext.NS_BangLuong
                         where bl.ID_DonVi == idChiNhanh
                         && bl.TuNgay <= ngayCham && bl.DenNgay >= ngayCham
                         && (bl.TrangThai == (int)commonEnumHellper.eBangLuong.tamluu || bl.TrangThai == (int)commonEnumHellper.eBangLuong.cantinhlai)
                         select bl.ID).Count() > 0;
            return exist;
        }

        public string CheckNhanVienExist_ChamCong(Guid idNhanVien, Guid idDonVi)
        {
            var mes = string.Empty;
            var exist = (from bl in _dbcontext.NS_BangLuong
                         join ct in _dbcontext.NS_BangLuong_ChiTiet on bl.ID equals ct.ID_BangLuong
                         where ct.ID_NhanVien == idNhanVien
                         && bl.ID_DonVi == idDonVi
                           && (bl.TrangThai == (int)commonEnumHellper.eBangLuong.tamluu || bl.TrangThai == (int)commonEnumHellper.eBangLuong.cantinhlai)
                         select bl.ID).Count() > 0;
            if (exist)
            {
                mes = "Các thay đổi sẽ ảnh hưởng đến bảng lương tạm. Bạn có chắc chắn muốn cập nhật không?";
            }
            //else
            //{
            //    exist = (from bs in _dbcontext.NS_CongBoSung
            //             where bs.ID_NhanVien == idNhanVien && bs.ID_DonVi == idDonVi
            //               && (bs.TrangThai == (int)commonEnumHellper.eBoSungCong.chuaduyet || bs.TrangThai == (int)commonEnumHellper.eBoSungCong.taomoi)
            //             select bs.ID).Count() > 0;
            //    if (exist)
            //    {
            //        mes = "Các thay đổi sẽ ảnh hưởng đến phiếu chấm công. Bạn có chắc chắn muốn cập nhật không?";
            //    }
            //}
            return mes;
        }

        // not use
        public void UpdateTongCong_SqlCmd(string sqlCmd, int loaicong)
        {
            var SumSql = string.Empty;
            if (loaicong == (int)commonEnumHellper.LoaiNgaynghiLe.ngaythuong)
            {
                SumSql = @" Update ct
		                        Set TongCong=(select SUM(bsc.Cong) 
		                        from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID and bsc.LoaiNgay = 0 ) ,
		                        TongCongQuyDoi=(select SUM(bsc.CongQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID and bsc.LoaiNgay = 0 ),
		                        PhutDiMuon=(select SUM(bsc.SoPhutDiMuon) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID),
		                        SoGioOT=(select SUM(bsc.SoGioOT) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID), 
		                        TongGioOTQuyDoi=(select SUM(bsc.GioOTQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID) 
	                        from NS_ChamCong_ChiTiet ct
	                        join @tblID tbl on ct.ID = tbl.ID ";
            }
            else
            {
                SumSql = @" Update ct		
		                    Set  TongCongOTNgayNghi=(select SUM(bsc.Cong) 
		                    from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet = ct.ID and bsc.LoaiNgay != 0 ),
                            TongCongOTNgayNghiQuyDoi=(select SUM(bsc.CongQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID and bsc.LoaiNgay != 0 ),
                            PhutDiMuon =(select SUM(bsc.SoPhutDiMuon) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID),
                            SoGioOT=(select SUM(bsc.SoGioOT) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID),
                            TongGioOTQuyDoi=(select SUM(bsc.GioOTQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=ct.ID)
	                    from NS_ChamCong_ChiTiet ct
	                    join @tblID tbl on ct.ID = tbl.ID";
            }
            sqlCmd = sqlCmd + SumSql;
            _dbcontext.Database.ExecuteSqlCommand(sqlCmd);
        }

        public JsonViewModel<string> UpdateChamThuCong(AddCongModel model, Guid idNhanVien, Guid idDonVi)
        {
            var result = new JsonViewModel<string> { ErrorCode = false };
            try
            {
                var CongBoSung = _dbcontext.NS_CongBoSung.FirstOrDefault(o => o.ID == model.ID_CongBoSung);
                var nguoitao = GetNguoiTao(idNhanVien);

                if (CongBoSung == null)
                {
                    result.Data = "Chi tiết châm công của nhân viên không tồn tại";
                    return result;
                }

                var ngayle = GetNgayNghiLe(model);
                var congquydoi = GetCongQuyDoi_ByIDCaLam_ofNhanVien(ngayle, model.Search.IDNhanVien ?? Guid.Empty, model.Search.ID_CaLamViec ?? Guid.Empty, idDonVi);
                double congQD = congquydoi.CongQuyDoi;
                double congOTQD = congquydoi.CongOTQuyDoi;

                if (!string.IsNullOrWhiteSpace(model.KyHieuCong))
                {
                    model.KyHieuCong = model.KyHieuCong.ToUpper();
                }
                CongBoSung.Cong = Math.Round(model.Cong, 1);
                CongBoSung.LoaiNgay = ngayle.LoaiNgay;
                CongBoSung.KyHieuCong = model.KyHieuCong;
                CongBoSung.NgaySua = DateTime.Now;
                CongBoSung.SoGioOT = model.SoGioOT;
                CongBoSung.SoPhutDiMuon = model.SoPhutDiMuon;
                CongBoSung.TrangThai = (int)commonEnumHellper.eBoSungCong.taomoi;
                CongBoSung.NguoiSua = nguoitao.TaiKhoan;
                CongBoSung.GhiChu = model.GhiChu;
                CongBoSung.GioOTQuyDoi = congOTQD * model.SoGioOT;
                CongBoSung.CongQuyDoi = congQD * model.Cong;
                UpdateStatusBangLuong_whenChangeCong(idDonVi, ngayle.NgayChamCong);
            }
            catch (Exception e)
            {
                result.ErrorCode = true;
                result.Data = e.InnerException + e.Message;
            }
            return result;
        }

        public IQueryable<NS_CongBoSung> GetConBoSungByCong(Guid idDonVi, Guid idNhanVien, Guid idCaLamViec, DateTime date)
        {
            return _dbcontext.NS_CongBoSung.Where(o => o.ID_DonVi == idDonVi && o.ID_NhanVien == idNhanVien && o.ID_CaLamViec == idCaLamViec
            && o.NgayCham.Year == date.Year && o.NgayCham.Month == date.Month && o.NgayCham.Day == date.Day);
        }

        public List<ChamCongModel> GetExportExcelToChamCong(ChamCongFilter model)
        {
            if (!string.IsNullOrWhiteSpace(model.Text))
            {
                model.Text = model.Text.ToUpper();
            }
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("Text", model.Text ?? string.Empty));
            paramlist.Add(new SqlParameter("ID_KyTinhCong", model.KyTinhCongId));
            paramlist.Add(new SqlParameter("ID_DonVi", model.ListDonVi.FirstOrDefault()));
            paramlist.Add(new SqlParameter("ListPhongBan", model.PhonBanId != null ? string.Join(",", GetListPhongbanById(model.PhonBanId ?? new Guid())) : string.Empty));
            var lst = _dbcontext.Database.SqlQuery<ChamCongModel>("exec GetDanhSachChamCong @Text,@ID_KyTinhCong,@ID_DonVi,@ListPhongBan", paramlist.ToArray()).ToList();
            return lst;
        }

        public string GetDonViExportChamCong(ChamCongFilter model)
        {
            var donVi = _dbcontext.DM_DonVi.FirstOrDefault(o => model.ListDonVi.Contains(o.ID));
            var chiNhanh = "Chi nhánh: " + (donVi != null ? donVi.TenDonVi : string.Empty);
            if (model.PhonBanId != null)
            {
                var phongBan = _dbcontext.NS_PhongBan.FirstOrDefault(o => o.ID == model.PhonBanId);
                chiNhanh += " - Phòng ban: " + (phongBan != null ? phongBan.TenPhongBan : string.Empty);
            }

            return chiNhanh;
        }

        public List<BangCongBSChiTietModel> GetChiTietCongBoSung(List<Guid> listIdCong)
        {
            if (listIdCong == null || listIdCong.Count == 0)
            {
                return new List<BangCongBSChiTietModel>();
            }
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("ListCongId", string.Join(",", listIdCong)));
            var lst = _dbcontext.Database.SqlQuery<BangCongBSChiTietModel>("exec GetCongBoSungByListIdCong @ListCongId", paramlist.ToArray()).ToList();
            return lst;
        }

        public List<BangCongBSChiTietModel> GetBangCongChiTiet(ChamCongFilter model)
        {
            var idChiNhanhs = string.Join(",", model.ListDonVi);
            var idCas = string.Empty;
            if (model.ListCa != null && model.ListCa.Count > 0)
            {
                idCas = string.Join(",", model.ListCa);
            }

            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("ID_NhanVien", model.IDNhanVien));
            paramlist.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs));
            paramlist.Add(new SqlParameter("IDCaLamViecs", idCas));
            paramlist.Add(new SqlParameter("FromDate", model.TuNgay));
            paramlist.Add(new SqlParameter("ToDate", model.DenNgay));
            paramlist.Add(new SqlParameter("CurrentPage", model.pageNow));
            paramlist.Add(new SqlParameter("PageSize", model.pageSize));
            var lst = _dbcontext.Database.SqlQuery<BangCongBSChiTietModel>("exec GetBangCongChiTiet @ID_NhanVien, @IDChiNhanhs, @IDCaLamViecs, @FromDate, @ToDate," +
                "@CurrentPage, @PageSize", paramlist.ToArray()).ToList();
            return lst;
        }

        public List<ExportBangCong> GetExportBangCong(ChamCongFilter model)
        {
            var idDonVis = string.Join(",", model.ListDonVi);
            List<SqlParameter> lstPr = new List<SqlParameter>();
            lstPr.Add(new SqlParameter("ID_NhanVien", model.IDNhanVien));
            lstPr.Add(new SqlParameter("IDDonVis", idDonVis));
            lstPr.Add(new SqlParameter("FromDate", model.TuNgay.Value));
            lstPr.Add(new SqlParameter("ToDate", model.DenNgay.Value));
            var cong = _dbcontext.Database.SqlQuery<ExportBangCong>("exec ExportBangCongNhanVien @ID_NhanVien, @IDDonVis, @FromDate, @ToDate", lstPr.ToArray()).ToList();
            return cong;
        }

        public JsonViewModel<string> KhoiTaoThamSoTinhCong(HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            history.NoiDung = "Khởi tạo tham số phục vụ tính công và lương";
            history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
            var nguoitao = GetNguoiTao(history.ID_NhanVien ?? new Guid());
            _dbcontext.HT_NhatKySuDung.Add(history);

            List<SqlParameter> lstPr = new List<SqlParameter>();
            lstPr.Add(new SqlParameter("ID_DonVi", history.ID_DonVi));
            lstPr.Add(new SqlParameter("NguoiTao", nguoitao.TaiKhoan));
            _dbcontext.Database.ExecuteSqlCommand("KhoiTaoDuLieuChamCong @ID_DonVi, @NguoiTao", lstPr.ToArray());
            _dbcontext.SaveChanges();

            result.ErrorCode = true;
            return result;
        }

        #endregion

        #region Quyền nhân sự
        public IQueryable<HT_Quyen_Nhom> GetListQuyen(Guid idNguoiDung)
        {
            var data = (from nd in _dbcontext.HT_NguoiDung
                        join ndn in _dbcontext.HT_NguoiDung_Nhom
                        on nd.ID equals ndn.IDNguoiDung
                        join qn in _dbcontext.HT_Quyen_Nhom
                        on ndn.IDNhomNguoiDung equals qn.ID_NhomNguoiDung
                        where nd.ID == idNguoiDung
                        select qn).Distinct();
            return data;
        }

        #endregion

        #region Nhân viên

        public bool GetDangKySuDungHRM()
        {
            return _dbcontext.HT_CongTy.FirstOrDefault().DangKyNhanSu ?? false;
        }

        public NS_NhanVien GetNhanVienById(Guid id)
        {
            return _dbcontext.NS_NhanVien.FirstOrDefault(o => o.ID == id);
        }

        public IQueryable<NS_Luong_PhuCap> GetLuongPhuCapById(Guid id)
        {
            return _dbcontext.NS_Luong_PhuCap.Where(o => o.ID_NhanVien == id);
        }
        #endregion

        #region Bảng lương

        public IQueryable<NS_BangLuong> GetKyBangLuongFilter(BangLuongFilter model)
        {
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);

            var data = _dbcontext.NS_BangLuong.AsQueryable();
            if (IsfilterTime)
            {
                data = data.Where(x =>
                 x.NgayTao >= startday
                && x.NgayTao <= Endday);
            }
            if (!string.IsNullOrWhiteSpace(model.Text))
            {
                model.Text = model.Text.ToUpper();
                data = data.Where(o => o.MaBangLuong.ToUpper().Contains(model.Text) || o.TenBangLuong.ToUpper().Contains(model.Text));
            }
            if (model.TrangThai != null && model.TrangThai.Count > 0)
            {
                data = data.Where(o => model.TrangThai.Contains(o.TrangThai));
            }
            return data.OrderByDescending(o => o.NgayTao);
        }

        public List<BangLuongDTO> GetAllBangLuong(BangLuongFilter model)
        {
            var trangthais = string.Join(",", model.TrangThai);
            var isDonVis = string.Join(",", model.ListDonVi);
            var txt = string.Empty;
            if (model.Text != null)
            {
                txt = model.Text;
            }
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref endday);
            if (IsfilterTime == false)
            {
                startday = new DateTime(2019, 1, 1);
                endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", isDonVis));
            sql.Add(new SqlParameter("TxtSearch", txt));
            sql.Add(new SqlParameter("FromDate", startday));
            sql.Add(new SqlParameter("ToDate", endday));
            sql.Add(new SqlParameter("TrangThais", trangthais));
            sql.Add(new SqlParameter("CurrentPage", model.pageNow - 1));
            sql.Add(new SqlParameter("PageSize", model.pageSize));
            return _dbcontext.Database.SqlQuery<BangLuongDTO>("exec GetAllBangLuong @IDChiNhanhs, @TxtSearch, @FromDate, @ToDate, @TrangThais, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }

        public List<BangLuongChiTietModel> GetBangLuongChiTiet_ofNhanVien(ParamSearchLuong model)
        {
            var isDonVis = string.Join(",", model.LstIDChiNhanh);
            var idNhanVien = string.Join(",", model.LstIDNhanVien);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", isDonVis));
            sql.Add(new SqlParameter("IDNhanVien", idNhanVien));
            sql.Add(new SqlParameter("CurrentPage", model.CurrentPage));
            sql.Add(new SqlParameter("PageSize", model.PageSize));
            return _dbcontext.Database.SqlQuery<BangLuongChiTietModel>("exec GetBangLuongChiTiet_ofNhanVien @IDChiNhanhs, @IDNhanVien, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }

        public List<BangLuongChiTietModel> GetAllBangLuongChiTiet(Guid id, int currentPage, int pageSize)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("id", id));
            sql.Add(new SqlParameter("CurrentPage", currentPage));
            sql.Add(new SqlParameter("PageSize", pageSize));
            return _dbcontext.Database.SqlQuery<BangLuongChiTietModel>("exec GetAllBangLuongChiTietById @id, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }

        public string GetMaBangLuong()
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("inputVar", "BL00001"));
            return _dbcontext.Database.SqlQuery<string>("exec RandomMaBangLuong @inputVar", sql.ToArray()).FirstOrDefault().Trim();
        }

        public string GetMaBangLuongMax_byTemp(Guid idDonVi)
        {
            var salaryCode = _dbcontext.Database.SqlQuery<string>("select dbo.GetMaBangLuongMax_byTemp('" + idDonVi + "')").First();
            return salaryCode;
        }

        public string GetChuoiSoKhong(string maxCode)
        {
            var chuoi0 = string.Empty;
            switch (maxCode.Length)
            {
                case 1:
                    chuoi0 = "0000";
                    break;
                case 2:
                    chuoi0 = "000";
                    break;
                case 3:
                    chuoi0 = "00";
                    break;
                case 4:
                    chuoi0 = "0";
                    break;
            }
            return chuoi0;
        }

        public double GetMaPhieuLuongChiTiet_Max(Guid idBangLuong)
        {
            return _dbcontext.Database.SqlQuery<double>("select ISNULL(MAX(CAST (dbo.udf_GetNumeric(MaBangLuongChiTiet) AS float)),0) + 1  from NS_BangLuong_ChiTiet where ID_BangLuong !='" + idBangLuong + "'").First();
        }

        public double GetCongChuan(Guid IdKyTinhCong)
        {
            var modelKyTinhCong = _dbcontext.NS_KyTinhCong.FirstOrDefault(o => o.ID == IdKyTinhCong);
            var congSetUp = _dbcontext.HT_CongTy.FirstOrDefault().NgayCongChuan;
            double cong = congSetUp ?? 0;
            if (congSetUp == null)
            {
                var soNgay = (modelKyTinhCong.DenNgay - modelKyTinhCong.TuNgay).TotalDays;
                int thu2 = 0, thu3 = 0, thu4 = 0, thu5 = 0, thu6 = 0, thu7 = 0, cn = 0;
                for (int i = 0; i <= soNgay; i++)
                {
                    switch (modelKyTinhCong.TuNgay.AddDays(i).DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            thu2 += 1;
                            break;
                        case DayOfWeek.Tuesday:
                            thu3 += 1;
                            break;
                        case DayOfWeek.Wednesday:
                            thu4 += 1;
                            break;
                        case DayOfWeek.Thursday:
                            thu5 += 1;
                            break;
                        case DayOfWeek.Friday:
                            thu6 += 1;
                            break;
                        case DayOfWeek.Saturday:
                            thu7 += 1;
                            break;
                        case DayOfWeek.Sunday:
                            cn += 1;
                            break;
                    }
                }
                var ngaynghile = _dbcontext.NS_NgayNghiLe.Where(o => o.Thu >= 0 /*&& o.ID_KyTinhCong == IdKyTinhCong*/);
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Monday).CongQuyDoi * thu2;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Tuesday).CongQuyDoi * thu3;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Wednesday).CongQuyDoi * thu4;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Thursday).CongQuyDoi * thu5;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Friday).CongQuyDoi * thu6;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Saturday).CongQuyDoi * thu7;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Sunday).CongQuyDoi * cn;
            }
            return cong;

        }

        // get congnhanvien sau khi quydoi
        public double GetCongChuan2(DateTime fromdate, DateTime todate)
        {
            var congSetUp = _dbcontext.HT_CongTy.FirstOrDefault().NgayCongChuan;
            double cong = congSetUp ?? 0;
            if (congSetUp == null)
            {
                var soNgay = (todate - fromdate).TotalDays;
                int thu2 = 0, thu3 = 0, thu4 = 0, thu5 = 0, thu6 = 0, thu7 = 0, cn = 0;
                for (int i = 0; i <= soNgay; i++)
                {
                    switch (fromdate.AddDays(i).DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            thu2 += 1;
                            break;
                        case DayOfWeek.Tuesday:
                            thu3 += 1;
                            break;
                        case DayOfWeek.Wednesday:
                            thu4 += 1;
                            break;
                        case DayOfWeek.Thursday:
                            thu5 += 1;
                            break;
                        case DayOfWeek.Friday:
                            thu6 += 1;
                            break;
                        case DayOfWeek.Saturday:
                            thu7 += 1;
                            break;
                        case DayOfWeek.Sunday:
                            cn += 1;
                            break;
                    }
                }
                //var ngaynghile = _dbcontext.NS_NgayNghiLe.Where(o => o.Thu >= 0 && o.ID_KyTinhCong == IdKyTinhCong);
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Monday).CongQuyDoi * thu2;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Tuesday).CongQuyDoi * thu3;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Wednesday).CongQuyDoi * thu4;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Thursday).CongQuyDoi * thu5;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Friday).CongQuyDoi * thu6;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Saturday).CongQuyDoi * thu7;
                //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Sunday).CongQuyDoi * cn;
            }
            return cong;

        }

        public double GetNgayCongThang(Guid IdKyTinhCong)
        {
            var modelKyTinhCong = _dbcontext.NS_KyTinhCong.FirstOrDefault(o => o.ID == IdKyTinhCong);
            double cong = 0;
            var soNgay = (modelKyTinhCong.DenNgay - modelKyTinhCong.TuNgay).TotalDays;
            int thu2 = 0, thu3 = 0, thu4 = 0, thu5 = 0, thu6 = 0, thu7 = 0, cn = 0;
            for (int i = 0; i <= soNgay; i++)
            {
                switch (modelKyTinhCong.TuNgay.AddDays(i).DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        thu2 += 1;
                        break;
                    case DayOfWeek.Tuesday:
                        thu3 += 1;
                        break;
                    case DayOfWeek.Wednesday:
                        thu4 += 1;
                        break;
                    case DayOfWeek.Thursday:
                        thu5 += 1;
                        break;
                    case DayOfWeek.Friday:
                        thu6 += 1;
                        break;
                    case DayOfWeek.Saturday:
                        thu7 += 1;
                        break;
                    case DayOfWeek.Sunday:
                        cn += 1;
                        break;
                }
            }
            var ngaynghile = _dbcontext.NS_NgayNghiLe.Where(o => o.Thu >= 0 /*&& o.ID_KyTinhCong == IdKyTinhCong*/);
            //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Monday).CongQuyDoi * thu2;
            //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Tuesday).CongQuyDoi * thu3;
            //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Wednesday).CongQuyDoi * thu4;
            //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Thursday).CongQuyDoi * thu5;
            //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Friday).CongQuyDoi * thu6;
            //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Saturday).CongQuyDoi * thu7;
            //cong += ngaynghile.FirstOrDefault(o => o.Thu == (int)DayOfWeek.Sunday).CongQuyDoi * cn;

            return cong;

        }

        public JsonViewModel<string> CreateBangLuong(NS_BangLuong model, Guid idNhanVien, Guid idDonVi)
        {
            var result = new JsonViewModel<string> { ErrorCode = false };
            //var modelKyTinhCong = _dbcontext.NS_KyTinhCong.FirstOrDefault(o => o.ID == model.ID_KyTinhCong);
            //if (modelKyTinhCong == null)
            //{
            //    result.Data = "Kỳ tính công không tồn tại hoặc đã bị xóa";
            //    return result;

            //}
            //if (modelKyTinhCong.TrangThai != (int)commonEnumHellper.TrangThaiKyTinhCong.chotky)
            //{
            //    result.Data = "Kỳ tính công chưa chốt chưa thể tính lương";
            //    return result;

            //}
            //if (_dbcontext.NS_BangLuong.Any(o => o.ID_KyTinhCong == model.ID_KyTinhCong && o.TrangThai != (int)commonEnumHellper.eBangLuong.xoa))
            //{
            //    result.Data = "Kỳ tính công đã được tính lương";
            //    return result;
            //}
            if (string.IsNullOrWhiteSpace(model.MaBangLuong))
            {
                model.MaBangLuong = GetMaBangLuong().ToUpper();
            }
            else if (_dbcontext.NS_BangLuong.Any(o => o.MaBangLuong.Equals(model.MaBangLuong.ToUpper())))
            {
                result.Data = "Mã bảng lương đã tồn tại trong hệ thống";
                return result;
            }

            var nguoitao = GetNguoiTao(idNhanVien);
            model.NguoiTao = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;

            if (model.TrangThai == (int)commonEnumHellper.eBangLuong.daduyet)
            {
                if (model.NgayThanhToanLuong == null)
                {
                    result.Data = "Vui lòng chọn ngày thanh toán lương trước khi phê duyệt";
                    return result;
                }
                model.ID_NhanVienDuyet = nguoitao.ID_NhanVien;
            }
            model.SuDungHRM = _dbcontext.HT_CongTy.FirstOrDefault().DangKyNhanSu ?? false;
            _dbcontext.NS_BangLuong.Add(model);
            //TinhLuongNhanVien(model.ID, model.ID_KyTinhCong, model.NguoiTao);
            string noidung = "Thêm mới Bảng lương - Mã bảng lương: " + model.MaBangLuong;
            string chitiet = "Thêm mới Bảng lương: <a style= \"cursor: pointer\" onclick = \"loadChamCong('" + model.ID + "')\" > Mã: " + model.MaBangLuong +
                               "</a><br /> Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                               "<br />Tên bảng lương: " + model.TenBangLuong +
                                 //"<br />Kỳ tính công: " + modelKyTinhCong.Ky + "( " + modelKyTinhCong.TuNgay.ToString("dd/MM/yyyy") + " - " + modelKyTinhCong.DenNgay.ToString("dd/MM/yyyy") + " )" +
                                 "<br />Trạng thái: " + (model.TrangThai == (int)commonEnumHellper.eBangLuong.tamluu ? "Tạm lưu" : "Đã duyệt") +
                              "<br /> Ghi chú: " + model.GhiChu;
            InsertNhatKySuDung("Bảng lương", noidung, chitiet, idDonVi, idNhanVien, 1);
            result.ErrorCode = true;
            return result;
        }

        public JsonViewModel<string> CheckBangLuongExist(Guid idDonVi, DateTime fromDate, DateTime toDate)
        {
            var result = new JsonViewModel<string> { ErrorCode = true, Data = string.Empty };

            try
            {

                // check from - to thuoc khoang giao voi bangluongtam
                var lstSalary = from bl in _dbcontext.NS_BangLuong
                                where bl.ID_DonVi == idDonVi
                                && (bl.TrangThai == (int)commonEnumHellper.eBangLuong.tamluu || bl.TrangThai == (int)commonEnumHellper.eBangLuong.cantinhlai)
                                && ((fromDate <= bl.TuNgay && (toDate >= bl.TuNgay || toDate > bl.DenNgay))
                                || (toDate >= bl.DenNgay && (fromDate <= bl.DenNgay || fromDate <= bl.TuNgay)))
                                select bl.ID;
                if (lstSalary.Count() > 0)
                {
                    result.Data = lstSalary.First().ToString(); // id bangluontam
                    result.ErrorCode = false;
                    return result;
                }
                result.ErrorCode = false;
            }
            catch (Exception e)
            {
                result.ErrorCode = true;
                result.Data = e.InnerException + e.Message;
            }
            return result;
        }

        public JsonViewModel<string> UpdateBangLuong(NS_BangLuong model, Guid idNhanVien, Guid idDonVi)
        {
            var result = new JsonViewModel<string> { ErrorCode = false };
            var data = _dbcontext.NS_BangLuong.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bảng lương đã tồn tại trong hệ thống";
                return result;
            }

            var nguoitao = GetNguoiTao(idNhanVien);
            data.NguoiSua = nguoitao != null ? nguoitao.TaiKhoan : string.Empty;
            data.NgaySua = DateTime.Now;
            data.GhiChu = model.GhiChu;
            data.TenBangLuong = model.TenBangLuong;
            data.NgayThanhToanLuong = model.NgayThanhToanLuong;
            if (model.TrangThai == (int)commonEnumHellper.eBangLuong.daduyet && model.TrangThai != data.TrangThai)
            {
                if (data.NgayThanhToanLuong == null)
                {
                    result.Data = "Vui lòng chọn ngày thanh toán lương trước khi phê duyệt";
                    return result;
                }
                model.ID_NhanVienDuyet = nguoitao.ID_NhanVien;
            }
            data.TrangThai = model.TrangThai;

            string noidung = "Cập nhật bảng lương - Mã bảng lương: " + model.MaBangLuong;
            string chitiet = "Cập nhật bảng lương: <a style= \"cursor: pointer\" onclick = \"loadChamCong('" + model.ID + "')\" > Mã: " + model.MaBangLuong +
                               "</a><br /> Ngày cập nhật: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                               "<br />Tên bảng lương: " + data.TenBangLuong + " => " + model.TenBangLuong +
                                 "<br />Trạng thái: " + (data.TrangThai == (int)commonEnumHellper.eBangLuong.tamluu ? "Lưu tạm" : "Đã duyệt") +
                                 " => " + (model.TrangThai == (int)commonEnumHellper.eBangLuong.tamluu ? "Lưu tạm" : "Đã duyệt") +
                              "<br /> Ghi chú: " + data.GhiChu + " => " + model.GhiChu;
            InsertNhatKySuDung("Bảng lương", noidung, chitiet, idDonVi, idNhanVien, 2);
            result.ErrorCode = true;
            return result;
        }

        public void UpdateTrangThai_CongBoSung_BangLuongChiTiet_IfBangLuongExist(Guid idBangLuong, int trangthai)
        {
            _dbcontext.NS_BangLuong_ChiTiet.Where(x => x.ID_BangLuong == idBangLuong).ToList().ForEach(x => x.TrangThai = trangthai);
            if (trangthai == 1)
            {
                trangthai = 2;
            }
            (from cong in _dbcontext.NS_CongBoSung
             join blct in _dbcontext.NS_BangLuong_ChiTiet on cong.ID_BangLuongChiTiet equals blct.ID
             where blct.ID_BangLuong == idBangLuong
             select cong).ToList().ForEach(x => x.TrangThai = trangthai);
        }

        public JsonViewModel<string> PheDuyetBangLuong(Guid Id, HT_NhatKySuDung History)
        {
            var result = new JsonViewModel<string> { ErrorCode = false };
            var data = _dbcontext.NS_BangLuong.FirstOrDefault(o => o.ID == Id);
            if (data == null)
            {
                result.Data = "Bảng lương không tồn tại hoặc đã bị xóa";
            }
            else if (data.TrangThai == (int)commonEnumHellper.eBangLuong.daduyet)
            {
                result.Data = "Bảng lương đã phê duyệt";
            }
            else
            {
                data.TrangThai = (int)commonEnumHellper.eBangLuong.daduyet;
                data.ID_NhanVienDuyet = History.ID_NhanVien;
                UpdateTrangThai_CongBoSung_BangLuongChiTiet_IfBangLuongExist(Id, (int)commonEnumHellper.eBangLuong.daduyet);

                History.NoiDung = "Phê duyệt bảng lương : " + data.TenBangLuong
                       + " với mã bảng lương: " + data.MaBangLuong;
                History.NoiDungChiTiet = "Thông tin bảng lương phê duyệt"
                                        + "<br/> Tên: " + data.TenBangLuong
                                        + "<br/> Nhân viên duyệt: " + _dbcontext.NS_NhanVien.Where(x => x.ID == data.ID_NhanVienDuyet).FirstOrDefault().TenNhanVien
                                        + "<br/> Sử dụng tính năng nhân sự: " + (data.SuDungHRM ? "có" : "Không");
                History.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                _dbcontext.HT_NhatKySuDung.Add(History);
                result.ErrorCode = true;
            }
            return result;
        }

        public JsonViewModel<string> MoLaiBangLuongDaChot(Guid Id, HT_NhatKySuDung History)
        {
            var result = new JsonViewModel<string> { ErrorCode = false };
            var data = _dbcontext.NS_BangLuong.FirstOrDefault(o => o.ID == Id);
            if (data == null)
            {
                result.Data = "Bảng lương không tồn tại hoặc đã bị xóa";
            }
            else
            {
                data.TrangThai = (int)commonEnumHellper.eBangLuong.tamluu;
                UpdateTrangThai_CongBoSung_BangLuongChiTiet_IfBangLuongExist(Id, (int)commonEnumHellper.eBangLuong.tamluu);
                History.NoiDung = "Mở lại bảng lương đã chốt : " + data.TenBangLuong
                + " với mã bảng lương: " + data.MaBangLuong;
                History.NoiDungChiTiet = "Thông tin bảng lương được mở lại"
                                        + "<br/> Tên: " + data.TenBangLuong
                                        + "<br/> Kỳ tính lương: " + data.TuNgay.Value.ToString("dd/MM/yyyy") + " - " + data.DenNgay.Value.ToString("dd/MM/yyyy")
                                        + "<br/> Nhân viên thực hiện: " + _dbcontext.NS_NhanVien.Where(x => x.ID == History.ID_NhanVien).FirstOrDefault().TenNhanVien;
                History.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                _dbcontext.HT_NhatKySuDung.Add(History);
                result.ErrorCode = true;
            }
            return result;
        }

        public void TinhLaiBangLuong(Guid IdBangLuong, string nguoiSua)
        {
            List<SqlParameter> pramSql = new List<SqlParameter>();
            pramSql.Add(new SqlParameter("ID_BangLuong", IdBangLuong));
            pramSql.Add(new SqlParameter("NguoiSua", nguoiSua));
            _dbcontext.Database.ExecuteSqlCommand("EXEC TinhLaiBangLuong @ID_BangLuong, @NguoiSua", pramSql.ToArray());
        }

        public List<BangLuongChiTietDTO> TinhLuongNhanVien(ParamSearchLuong prSearch)
        {

            var kieuluongs = string.Join(",", prSearch.LstKieuLuong);
            var idDonVis = string.Join(",", prSearch.LstIDChiNhanh);
            var idNhanViens = string.Empty;
            if (prSearch.LstIDNhanVien != null && prSearch.LstIDNhanVien.Count > 0)
            {
                idNhanViens = string.Join(",", prSearch.LstIDNhanVien);
            }
            List<SqlParameter> pramSql = new List<SqlParameter>();
            pramSql.Add(new SqlParameter("IDChiNhanhs", idDonVis));
            pramSql.Add(new SqlParameter("IDNhanViens", idNhanViens));
            pramSql.Add(new SqlParameter("FromDate", prSearch.FromDate));
            pramSql.Add(new SqlParameter("ToDate", prSearch.ToDate));
            pramSql.Add(new SqlParameter("KieuLuongs", kieuluongs));
            pramSql.Add(new SqlParameter("CurrentPage", prSearch.CurrentPage));
            pramSql.Add(new SqlParameter("PageSize", prSearch.PageSize));
            List<BangLuongChiTietDTO> data = _dbcontext.Database.SqlQuery<BangLuongChiTietDTO>("exec TinhLuongNhanVien @IDChiNhanhs, @IDNhanViens, @FromDate, @ToDate,@KieuLuongs,@CurrentPage,@PageSize ", pramSql.ToArray()).ToList();
            return data;
        }

        public List<LuongChinhDTO> GetLuongChinh_ofNhanVien(Guid idChiNhanh, Guid idNhanVien, DateTime tungay, DateTime denngay, int ngaycongchuan)
        {
            List<SqlParameter> pramSql = new List<SqlParameter>();
            pramSql.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            pramSql.Add(new SqlParameter("IDNhanViens", idNhanVien));
            pramSql.Add(new SqlParameter("FromDate", tungay));
            pramSql.Add(new SqlParameter("ToDate", denngay));
            pramSql.Add(new SqlParameter("NgayCongChuan", ngaycongchuan));
            List<LuongChinhDTO> data = _dbcontext.Database.SqlQuery<LuongChinhDTO>("exec GetLuongChinh_ofNhanVien @ID_ChiNhanh,@IDNhanViens, @FromDate, @ToDate, @NgayCongChuan", pramSql.ToArray()).ToList();
            return data;
        }

        public List<LuongOT> GetLuongOT_ofNhanVien(Guid idChiNhanh, List<string> lstID, DateTime tungay, DateTime denngay, int ngaycongchuan)
        {
            var idNhanViens = string.Join(",", lstID);
            List<SqlParameter> pramSql = new List<SqlParameter>();
            pramSql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            pramSql.Add(new SqlParameter("IDNhanViens", idNhanViens));
            pramSql.Add(new SqlParameter("FromDate", tungay));
            pramSql.Add(new SqlParameter("ToDate", denngay));
            pramSql.Add(new SqlParameter("NgayCongChuan", ngaycongchuan));
            List<LuongOT> data = _dbcontext.Database.SqlQuery<LuongOT>("exec GetLuongOT_ofNhanVien @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate, @NgayCongChuan", pramSql.ToArray()).ToList();
            return data;
        }

        public List<PhuCap> PhuCap_ofNhanVien(Guid idChiNhanh, List<string> lstID, DateTime tungay, DateTime denngay)
        {
            var idNhanViens = string.Join(",", lstID);
            List<SqlParameter> pramSql = new List<SqlParameter>();
            pramSql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            pramSql.Add(new SqlParameter("FromDate", tungay));
            pramSql.Add(new SqlParameter("ToDate", denngay));
            pramSql.Add(new SqlParameter("IDNhanViens", idNhanViens));
            List<PhuCap> data = _dbcontext.Database.SqlQuery<PhuCap>("exec GetPhuCapLuongChiTiet @IDChiNhanhs, @FromDate, @ToDate, @IDNhanViens", pramSql.ToArray()).ToList();
            return data;
        }

        public List<GiamTru> GiamTru_ofNhanVien(Guid idChiNhanh, List<string> lstIDNhanVien, DateTime tungay, DateTime denngay)
        {
            var idNhanViens = string.Join(",", lstIDNhanVien);
            List<SqlParameter> pramSql = new List<SqlParameter>();
            pramSql.Add(new SqlParameter("IDChiNhanhs", idChiNhanh));
            pramSql.Add(new SqlParameter("FromDate", tungay));
            pramSql.Add(new SqlParameter("ToDate", denngay));
            pramSql.Add(new SqlParameter("IDNhanViens", idNhanViens));
            List<GiamTru> data = _dbcontext.Database.SqlQuery<GiamTru>("exec GetGiamTruLuongChiTiet @IDChiNhanhs, @FromDate, @ToDate, @IDNhanViens", pramSql.ToArray()).ToList();
            return data;
        }

        public List<NS_NhanVien_DonVi> CheckSameTime_CaLamViec(Param_GetChietKhauHoaDon lstParam)
        {
            string strErr = string.Empty;
            try
            {
                var idNhanViens = lstParam.ID_NhanViens;
                var idDonVi = lstParam.ID_DonVi;
                var tuNgay = lstParam.ApDungTuNgay;
                var denNgay = lstParam.ApDungDenNgay;
                var idChietKhau = lstParam.ID_ChietKhauHoaDon;
                var tinhCKTheo = lstParam.TinhChietKhauTheo;

                List<SqlParameter> sqlParam = new List<SqlParameter>();
                sqlParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                sqlParam.Add(new SqlParameter("ID_NhanViens", idNhanViens));
                sqlParam.Add(new SqlParameter("TuNgay", tuNgay));
                sqlParam.Add(new SqlParameter("DenNgay", denNgay));

                var data = _dbcontext.Database.SqlQuery<NS_NhanVien_DonVi>("EXEC CheckSameTime_CaLamViec @ID_DonVi, @ID_NhanViens," +
                    "@TuNgay,@DenNgay", sqlParam.ToArray()).ToList();
                return data;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("classNS_NhanVien_ChiTiet.CheckExist_ChietKhauDoanhThu_NhanVien " + ex.InnerException + ex.Message);
                return null;
            }
        }

        public void DeleteBangLuongChiTietBy(Guid idBangLuong)
        {
            SqlParameter sql = new SqlParameter("IdBangLuong", idBangLuong);
            _dbcontext.Database.ExecuteSqlCommand("exec DeleteBangLuongChiTietById @IdBangLuong", sql);
        }

        //public JsonViewModel<string> TinhLaiBangLuong(Guid Id, HT_NhatKySuDung History)
        //{
        //    var result = new JsonViewModel<string> { ErrorCode = false };
        //    var data = _dbcontext.NS_BangLuong.FirstOrDefault(o => o.ID == Id);
        //    if (data == null)
        //    {
        //        result.Data = "Bảng lương không tồn tại hoặc đã bị xóa";
        //    }
        //    else if (data.TrangThai == (int)commonEnumHellper.eBangLuong.daduyet)
        //    {
        //        result.Data = "Bảng lương đã phê duyệt";
        //    }
        //    else
        //    {
        //        List<SqlParameter> sql = new List<SqlParameter>();
        //        sql.Add(new SqlParameter("IdBangLuong", data.ID));
        //        var checkStore = _dbcontext.Database.SqlQuery<int>("exec DeleteBangLuongChiTietById @IdBangLuong", sql.ToArray()).FirstOrDefault();
        //        if (checkStore >= 0)
        //        {
        //            var nguoitao = GetNguoiTao(History.ID_NhanVien ?? new Guid());
        //            History.NoiDung = "Tính lại bảng lương : " + data.TenBangLuong
        //                   + " với mã bảng lương: " + data.MaBangLuong;
        //            History.NoiDungChiTiet = "Thông tin bảng lương tính lại"
        //                                    + "<br/> Tên: " + data.TenBangLuong
        //                                    + "<br/> Ngày thanh toán: " + (data.NgayThanhToanLuong != null ? data.NgayThanhToanLuong.Value.ToString("dd/MM/yyyy") : string.Empty)
        //                                    + "<br/> Sử dụng tính năng nhân sự: " + (data.SuDungHRM ? "có" : "Không");
        //            History.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
        //            _dbcontext.HT_NhatKySuDung.Add(History);
        //            data.NguoiSua = nguoitao.TaiKhoan;
        //            data.NgaySua = DateTime.Now;
        //            //TinhLuongNhanVien(data.ID, data.ID_KyTinhCong, nguoitao.TaiKhoan);
        //            result.ErrorCode = true;
        //        }
        //        else
        //        {
        //            result.Data = "Đã xảy ra lỗi trong quá trình cập nhật";
        //        }
        //    }
        //    return result;
        //}

        #endregion

        #region Bảo hiểm, khen thưởng

        public IQueryable<NS_LoaiBaoHiem> GetSearchLoaiBaoHiem(SearchFilter model)
        {
            var data = _dbcontext.NS_LoaiBaoHiem.Where(o => o.TrangThai != (int)commonEnumHellper.eLoaiBaoHiem.xoa).AsQueryable();
            var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);
            if (!string.IsNullOrWhiteSpace(model.Text))
            {
                data = data.Where(o => o.TenBaoHiem.ToUpper().Contains(model.Text.ToUpper()));
            }
            if (model.TrangThai != null && model.TrangThai.Count > 0)
            {
                data = data.Where(o => model.TrangThai.Contains(o.TrangThai));
            }
            if (IsfilterTime)
            {
                data = data.Where(x =>
                 x.NgayApDung >= startday
                && x.NgayApDung <= Endday);
            }
            return data.OrderBy(o => o.NgayTao);

        }

        public JsonViewModel<string> InsertLoaiBaoHiem(NS_LoaiBaoHiem model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            if (_dbcontext.NS_LoaiBaoHiem.Any(o => o.TenBaoHiem.ToUpper().Equals(model.TenBaoHiem.ToUpper())
                                                    && o.TrangThai != (int)commonEnumHellper.eLoaiBaoHiem.xoa))
            {
                result.Data = "Tên loại bảo hiểm đã tồn tại";
                return result;
            }
            var nguoitao = GetNguoiTao(history.ID_NhanVien ?? new Guid());
            model.NguoiTao = nguoitao.TaiKhoan;
            model.TyLeCongTy = Math.Round(model.TyLeCongTy, 1);
            model.TyLeNhanVien = Math.Round(model.TyLeNhanVien, 1);
            _dbcontext.NS_LoaiBaoHiem.Add(model);
            history.NoiDung = "Thêm mới loại bảo hiểm : " + model.TenBaoHiem
                      + " Ngày áp dụng: " + model.NgayApDung.ToString("dd/MM/yyyy");
            history.NoiDungChiTiet = "Thông tin thêm mới:"
                                    + "<br/> Tên loại bảo hiểm: " + model.TenBaoHiem
                                    + "<br/> Ngày áp dụng: " + model.NgayApDung.ToString("dd/MM/yyyy")
                                     + "<br/> Tỷ lệ công ty đóng: " + model.TyLeCongTy + "%"
                                     + "<br/> Tỷ lệ nhân viên đóng: " + model.TyLeNhanVien + "%"
                                     + "<br/> Trạng thái: " + commonEnumHellper.ListeLoaiBaoHiem.FirstOrDefault(o => o.Key == model.TrangThai).Value
                                     /*+ "<br/> Ghi chú: " +model.ghichu*/;
            history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
            _dbcontext.HT_NhatKySuDung.Add(history);
            result.ErrorCode = true;
            return result;
        }

        public JsonViewModel<string> UpdateLoaiBaoHiem(NS_LoaiBaoHiem model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = _dbcontext.NS_LoaiBaoHiem.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Loại bảo hiểm không tồn tại hoặc đã bị xóa";
                return result;

            }
            if (_dbcontext.NS_LoaiBaoHiem.Any(o => o.ID != model.ID
            && o.TenBaoHiem.ToUpper().Equals(model.TenBaoHiem.ToUpper())
            && o.TrangThai != (int)commonEnumHellper.eLoaiBaoHiem.xoa))
            {
                result.Data = "Tên loại bảo hiểm đã tồn tại";
                return result;

            }
            history.NoiDung = "Cập nhật loại bảo hiểm : " + data.TenBaoHiem
                     + " Ngày áp dụng: " + data.NgayApDung.ToString("dd/MM/yyyy");
            history.NoiDungChiTiet = "Thông tin cập nhật:"
                                    + "<br/> Tên loại bảo hiểm: " + data.TenBaoHiem + " => " + model.TenBaoHiem
                                    + "<br/> Ngày áp dụng: " + data.NgayApDung.ToString("dd/MM/yyyy") + " => " + model.NgayApDung.ToString("dd/MM/yyyy")
                                     + "<br/> Tỷ lệ công ty đóng: " + data.TyLeCongTy + " => " + model.TyLeCongTy + "%"
                                     + "<br/> Tỷ lệ nhân viên đóng: " + data.TyLeNhanVien + " => " + model.TyLeNhanVien + "%"
                                     + "<br/> Trạng thái: " + commonEnumHellper.ListeLoaiBaoHiem.FirstOrDefault(o => o.Key == data.TrangThai).Value + " => " + commonEnumHellper.ListeLoaiBaoHiem.FirstOrDefault(o => o.Key == model.TrangThai).Value
                                     /*+ "<br/> Ghi chú: " +model.ghichu*/;
            history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
            _dbcontext.HT_NhatKySuDung.Add(history);

            var nguoitao = GetNguoiTao(history.ID_NhanVien ?? new Guid());
            data.NguoiSua = nguoitao.TaiKhoan;
            data.NgaySua = DateTime.Now;
            data.TyLeCongTy = Math.Round(model.TyLeCongTy, 1);
            data.TyLeNhanVien = Math.Round(model.TyLeNhanVien, 1);
            data.TenBaoHiem = model.TenBaoHiem;
            data.NgayApDung = model.NgayApDung;
            data.TrangThai = model.TrangThai;
            data.GhiChu = model.GhiChu;
            result.ErrorCode = true;
            return result;
        }

        public JsonViewModel<string> DeleteLoaiBaoHiem(NS_LoaiBaoHiem model, HT_NhatKySuDung history)
        {
            var result = new JsonViewModel<string>() { ErrorCode = false };
            var data = _dbcontext.NS_LoaiBaoHiem.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Loại bảo hiểm không tồn tại hoặc đã bị xóa";
                return result;

            }
            var nguoitao = GetNguoiTao(history.ID_NhanVien ?? new Guid());
            data.NguoiSua = nguoitao.TaiKhoan;
            data.NgaySua = DateTime.Now;
            data.TrangThai = (int)commonEnumHellper.eLoaiBaoHiem.xoa;
            history.NoiDung = "Xóa loại bảo hiểm : " + data.TenBaoHiem
                      + " Ngày áp dụng: " + data.NgayApDung.ToString("dd/MM/yyyy");
            history.NoiDungChiTiet = "Thông tin xóa:"
                                    + "<br/> Tên loại bảo hiểm: " + data.TenBaoHiem
                                    + "<br/> Ngày áp dụng: " + data.NgayApDung.ToString("dd/MM/yyyy")
                                     + "<br/> Tỷ lệ công ty đóng: " + data.TyLeCongTy + "%"
                                     + "<br/> Tỷ lệ nhân viên đóng: " + data.TyLeNhanVien + "%"
                                     + "<br/> Trạng thái: " + commonEnumHellper.ListeLoaiBaoHiem.FirstOrDefault(o => o.Key == data.TrangThai).Value
                                     /*+ "<br/> Ghi chú: " +model.ghichu*/;
            history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.delete;
            _dbcontext.HT_NhatKySuDung.Add(history);
            result.ErrorCode = true;
            return result;
        }

        #endregion
        public List<Guid> GetListPhongbanById(Guid idPhongBan)
        {
            var listPhongban = new List<Guid>();
            listPhongban.Add(idPhongBan);
            GetListPhongbanchildren(listPhongban, ref listPhongban);
            return listPhongban;
        }

        public void GetListPhongbanchildren(List<Guid> listphongban, ref List<Guid> result)
        {
            var data = _dbcontext.NS_PhongBan.Where(o => listphongban.Contains(o.ID_PhongBanCha ?? new Guid())).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
            if (data.Count > 0)
            {
                result.AddRange(data);
                GetListPhongbanchildren(data, ref result);
            }

        }
    }

    public class AddCongModel
    {
        public Guid ID_KyHieuCong { get; set; }
        public Guid ID_ChiTietCong { get; set; } // not use
        public Guid ID_NhanVien { get; set; }
        public Guid ID_CaLamViec { get; set; }
        public Guid ID_DonVi { get; set; }
        public int Ngay { get; set; }
        public string KyHieuCong { get; set; }
        public double SoGioOT { get; set; }
        public int SoPhutDiMuon { get; set; }
        public double Cong { get; set; }
        public bool ApDungToanNhanVien { get; set; }
        public bool IsNew { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }
        public Guid ID_CongBoSung { get; set; }
        public ChamCongFilter Search { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string MaCa { get; set; }
        public string TenCa { get; set; }
    }

    public class NS_CaLamViecModel : NS_CaLamViec
    {
        public string CachLayGioCongText
        {
            get
            {
                return commonEnumHellper.ListCachLayGioCong.FirstOrDefault(o => o.Key == CachLayGioCong).Value;
            }
        }

        public string TrangThaiText
        {
            get
            {
                return commonEnumHellper.ListTrangThaiCaLamViec.FirstOrDefault(o => o.Key == TrangThai).Value;
            }
        }

        public bool IsShowEdit
        {
            get
            {
                return TrangThai != (int)commonEnumHellper.TrangThaiCaLamViec.xoa ? true : false;

            }
        }

        public bool DaApDungPhanCa { get; set; }

        public List<Guid> ListIdDonVI { get; set; }
    }
    public class PhongBanChiNhanhView
    {
        public Guid id { get; set; }
        public string text { get; set; }
        public Guid? parentId { get; set; }
        public Guid? ID_DonVi { get; set; }
        public string parentText { get; set; }
        public bool active { get; set; }
        public List<PhongBanChiNhanhView> children { get; set; }

    }
    public class ChiTietCaOfPhieu
    {
        public string MaCa { get; set; }
        public Guid IdCa { get; set; }
        public string TenCa { get; set; }
        public DateTime GioVao { get; set; }
        public DateTime GioRa { get; set; }
        public int GiaTri { get; set; }
        public string TenGiaTri
        {
            get; set;
        }
    }

    public class CaTuanModel
    {
        public int key { get; set; }

        public List<Guid> value { get; set; }
    }
    public class BangCongBSChiTietModel
    {
        public Guid ID { get; set; }
        public double Cong { get; set; }
        public string GhiChu { get; set; }
        public string KyHieuCong { get; set; }
        public int? LoaiNgay { get; set; }
        public DateTime NgayCham { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public double SoGioOT { get; set; }
        public int SoPhutDiMuon { get; set; }
        public double TongSoGioOT { get; set; }
        public double TongSoPhutDiMuon { get; set; }
        public string TenCa { get; set; }
        public string MaCa { get; set; }
        public Guid ID_Ca { get; set; }
        public DateTime GioVao { get; set; }
        public DateTime GioRa { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }
    public class PhanCaModel
    {
        public NS_PhieuPhanCa PhieuPhanCa { get; set; }

        public List<Guid> CaCoDinh { get; set; }

        public List<CaTuanModel> CaTuan { get; set; }

        public List<Guid> NhanVien { get; set; }

    }
    public class NhanVienPhanCaModel
    {
        public Guid Id { get; set; }

        public bool Active { get; set; }

        public string Ten { get; set; }

        public string Ma { get; set; }

        public string TenPhong { get; set; }

        public bool Checked { get; set; }

        public bool IsNew { get; set; }
    }

    public class HoSoChamCongModel
    {
        public Guid IDKyTinhCong { get; set; }
        public List<Guid> ListPhieuPhanCa { get; set; }
    }

    public class NS_CaLamViecExport
    {

        public Guid ID { get; set; }
        public string MaCa { get; set; }
        public string TenCa { get; set; }
        public int TrangThai { get; set; }
        public string TrangThaiText
        {
            get
            {
                return commonEnumHellper.ListTrangThaiCaLamViec.FirstOrDefault(o => o.Key == TrangThai).Value;
            }
        }
        public string GioVao { get; set; }
        public string GioRa { get; set; }
        public string NghiGiuaCaTu { get; set; }
        public string NghiGiuaCaDen { get; set; }
        public string GioOTBanNgayTu { get; set; }
        public string GioOTBanNgayDen { get; set; }
        public string GioOTBanDemTu { get; set; }
        public string GioOTBanDemDen { get; set; }
        public double TongGioCong { get; set; }
        public int CachLayGioCong { get; set; }
        public string CachLayGioCongText
        {
            get
            {
                return commonEnumHellper.ListCachLayGioCong.FirstOrDefault(o => o.Key == CachLayGioCong).Value;
            }
        }
        public string LaCaDem { get; set; }
        public string NgayTaoText { get; set; }
        public string NguoiTao { get; set; }
        public string GhiChuCaLamViec { get; set; }
        public int? SoPhutDiMuon { get; set; }
        public int? SoPhutVeSom { get; set; }
        public string GioVaoTu { get; set; }
        public string GioVaoDen { get; set; }
        public string GioRaTu { get; set; }
        public string GioRaDen { get; set; }
        public string TinhOTBanNgayTu { get; set; }
        public string TinhOTBanNgayDen { get; set; }
        public string TinhOTBanDemTu { get; set; }
        public string TinhOTBanDemDen { get; set; }
        public double SoGioOTToiThieu { get; set; }
        public string GhiChuTinhGio { get; set; }
        public DateTime NgayTao { get; set; }
    }
    public class ExportCaLamViecError
    {
        public string MaCa { get; set; }
        public string TenCa { get; set; }
        public string TrangThai { get; set; }
        public string GioVao { get; set; }
        public string GioRa { get; set; }
        public string NghiGiuaCaTu { get; set; }
        public string NghiGiuaCaDen { get; set; }
        public string GioOTBanNgayTu { get; set; }
        public string GioOTBanNgayDen { get; set; }
        public string GioOTBanDemTu { get; set; }
        public string GioOTBanDemDen { get; set; }
        public string CachLayGioCong { get; set; }
        public string LaCaDem { get; set; }
        public string GhiChuCaLamViec { get; set; }
        public string SoPhutDiMuon { get; set; }
        public string SoPhutVeSom { get; set; }
        public string GioVaoTu { get; set; }
        public string GioVaoDen { get; set; }
        public string GioRaTu { get; set; }
        public string GioRaDen { get; set; }
        public string TinhOTBanNgayTu { get; set; }
        public string TinhOTBanNgayDen { get; set; }
        public string TinhOTBanDemTu { get; set; }
        public string TinhOTBanDemDen { get; set; }
        public string SoGioOTToiThieu { get; set; }
        public string GhiChuTinhGio { get; set; }
        public string Error { get; set; }
    }
    public class CaLamViecModelInsert
    {
        public NS_CaLamViec Model { get; set; }

        public List<Guid> ListDonVi { get; set; }
    }
    public class SearchFilter
    {
        public string Text { get; set; }

        public List<Guid> ListDonVi { get; set; }

        public DateTime? TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }

        public List<int?> TrangThai { get; set; }

        public int pageSize { get; set; }

        public int? TypeTime { get; set; }

        public int pageNow { get; set; }

        public int? Order { get; set; }

        public bool Sort { get; set; }

        public Guid? IDNhanVien { get; set; }

        public string Where { get; set; }
    }

    public class PhieuPhanCaFilter : SearchFilter
    {
        public List<int?> LoaiCa { get; set; }

        public int? TypeTimeNgayTao { get; set; }

        public DateTime? NgayTaoTu { get; set; }

        public DateTime? NgayTaoDen { get; set; }
    }
    public class ChamCongFilter : SearchFilter
    {
        public Guid? KyTinhCongId { get; set; }

        public Guid? PhonBanId { get; set; }

        public Guid? ID_CaLamViec { get; set; }

        public List<string> ListCa { get; set; }
    }
    public class BangLuongFilter : SearchFilter
    {
        public List<Guid> KyTinhCongs { get; set; }

        public Guid? PhonBanId { get; set; }
    }
    public class ExportPhieuPhanCa
    {
        public string MaPhieu { get; set; }
        public string LoaiPhanCaText { get; set; }
        public string TrangThaiText { get; set; }
        public string TuNgayText { get; set; }
        public string DenNgayText { get; set; }
        public string NgayTaoText { get; set; }
        public string NguoiTao { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
    }
    public class ListNhanVienChietKhau
    {
        public Guid ID_nhanVien { get; set; }
        public double TienChietKhau { get; set; }
    }
    public class ChamCongModel
    {
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
        public Guid ID_CaLamViec { get; set; }
        public Guid ID_NhanVien { get; set; }
        public Guid ID_PhieuPhanCa { get; set; }
        public int? LoaiCong { get; set; } // not use + not bind: 0.ngaythuong, 2.ngaynghile
        public int? Nam { get; set; }
        public int? Thang { get; set; }
        public string MaCa { get; set; }
        public string TenCa { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public int? TrangThaiNV { get; set; }
        public double TongCongNV { get; set; }// sumcong nv by calamviec
        public int? SoPhutDiMuon { get; set; }
        public double? SoPhutOT { get; set; }
        public string Ngay1 { get; set; }
        public string Ngay2 { get; set; }
        public string Ngay3 { get; set; }
        public string Ngay4 { get; set; }
        public string Ngay5 { get; set; }
        public string Ngay6 { get; set; }
        public string Ngay7 { get; set; }
        public string Ngay8 { get; set; }
        public string Ngay9 { get; set; }
        public string Ngay10 { get; set; }
        public string Ngay11 { get; set; }
        public string Ngay12 { get; set; }
        public string Ngay13 { get; set; }
        public string Ngay14 { get; set; }
        public string Ngay15 { get; set; }
        public string Ngay16 { get; set; }
        public string Ngay17 { get; set; }
        public string Ngay18 { get; set; }
        public string Ngay19 { get; set; }
        public string Ngay20 { get; set; }
        public string Ngay21 { get; set; }
        public string Ngay22 { get; set; }
        public string Ngay23 { get; set; }
        public string Ngay24 { get; set; }
        public string Ngay25 { get; set; }
        public string Ngay26 { get; set; }
        public string Ngay27 { get; set; }
        public string Ngay28 { get; set; }
        public string Ngay29 { get; set; }
        public string Ngay30 { get; set; }
        public string Ngay31 { get; set; }
        public string NguoiTao { get; set; } // not bind
        public string GioVao { get; set; } // giovao ca
        public string GioRa { get; set; }
        public string TuNgay { get; set; } // phieuphanca tungay - denngay
        public string DenNgay { get; set; } // sử dụng khi 1tháng có > 1phiếu phân ca (khác thời gian), và trùng ca, trùng nhân viên
        public string DisNgay1 { get; set; }
        public string DisNgay2 { get; set; }
        public string DisNgay3 { get; set; }
        public string DisNgay4 { get; set; }
        public string DisNgay5 { get; set; }
        public string DisNgay6 { get; set; }
        public string DisNgay7 { get; set; }
        public string DisNgay8 { get; set; }
        public string DisNgay9 { get; set; }
        public string DisNgay10 { get; set; }
        public string DisNgay11 { get; set; }
        public string DisNgay12 { get; set; }
        public string DisNgay13 { get; set; }
        public string DisNgay14 { get; set; }
        public string DisNgay15 { get; set; }
        public string DisNgay16 { get; set; }
        public string DisNgay17 { get; set; }
        public string DisNgay18 { get; set; }
        public string DisNgay19 { get; set; }
        public string DisNgay20 { get; set; }
        public string DisNgay21 { get; set; }
        public string DisNgay22 { get; set; }
        public string DisNgay23 { get; set; }
        public string DisNgay24 { get; set; }
        public string DisNgay25 { get; set; }
        public string DisNgay26 { get; set; }
        public string DisNgay27 { get; set; }
        public string DisNgay28 { get; set; }
        public string DisNgay29 { get; set; }
        public string DisNgay30 { get; set; }
        public string DisNgay31 { get; set; }
    }
    public class ExportBangCong
    {
        public string MaCa { get; set; }
        public string TenCa { get; set; }
        public DateTime? GioVao { get; set; }
        public DateTime? GioRa { get; set; }
        public string GioVaoText
        {
            get
            {
                if (GioVao.HasValue)
                {
                    return GioVao.Value.ToString("HH:mm");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string GioRaText
        {
            get
            {
                if (GioRa.HasValue)
                {
                    return GioRa.Value.ToString("HH:mm");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Ngay1 { get; set; }
        public string Ngay2 { get; set; }
        public string Ngay3 { get; set; }
        public string Ngay4 { get; set; }
        public string Ngay5 { get; set; }
        public string Ngay6 { get; set; }
        public string Ngay7 { get; set; }
        public string Ngay8 { get; set; }
        public string Ngay9 { get; set; }
        public string Ngay10 { get; set; }
        public string Ngay11 { get; set; }
        public string Ngay12 { get; set; }
        public string Ngay13 { get; set; }
        public string Ngay14 { get; set; }
        public string Ngay15 { get; set; }
        public string Ngay16 { get; set; }
        public string Ngay17 { get; set; }
        public string Ngay18 { get; set; }
        public string Ngay19 { get; set; }
        public string Ngay20 { get; set; }
        public string Ngay21 { get; set; }
        public string Ngay22 { get; set; }
        public string Ngay23 { get; set; }
        public string Ngay24 { get; set; }
        public string Ngay25 { get; set; }
        public string Ngay26 { get; set; }
        public string Ngay27 { get; set; }
        public string Ngay28 { get; set; }
        public string Ngay29 { get; set; }
        public string Ngay30 { get; set; }
        public string Ngay31 { get; set; }
        public double? TongCong { get; set; }
        public double? TongCongNgayNghi { get; set; }
        public double? TongOT { get; set; }
        public double? TongPhutDiMuon { get; set; }
    }

    public class BangLuongChiTietModel
    {
        public string MaBangLuongChiTiet { get; set; }
        public Guid ID_BangLuong_ChiTiet { get; set; }
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double NgayCongThuc { get; set; }
        public double NgayCongChuan { get; set; }
        public double LuongCoBan { get; set; }
        public double LuongChinh { get; set; }
        public double LuongOT { get; set; }
        public double PhuCapCoBan { get; set; }
        public double PhuCapKhac { get; set; }
        public double KhenThuong { get; set; }
        public double KyLuat { get; set; }
        public double ChietKhau { get; set; }
        public double LuongTruocGiamTru { get; set; }
        public double TongTienPhat { get; set; }
        public double LuongSauGiamTru { get; set; }
        public double? TruTamUngLuong { get; set; }
        public double? ThanhToan { get; set; }
        public double DaTra { get; set; }
        public double ConLai { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public bool? DaNghiViec { get; set; }

        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
        public double TongNgayCongThuc { get; set; }
        public double TongLuongChinh { get; set; }
        public double TongLuongOT { get; set; }
        public double TongPhuCapCoBan { get; set; }
        public double TongPhuCapKhac { get; set; }
        public double TongKhenThuong { get; set; }
        public double TongKyLuat { get; set; }
        public double TongChietKhau { get; set; }
        public double TongLuongTruocGiamTru { get; set; }
        public double TongTienPhatAll { get; set; }
        public double TongLuongSauGiamTru { get; set; }
        public double? TongTamUng { get; set; }
        public double? TongThanhToan { get; set; }
        public double TongDaTra { get; set; }
        public double TongConLai { get; set; }
    }

    public class ChamCongDTO
    {
        public double Cong { get; set; }
        public int Ngay { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public DateTime NgayChamCong { get; set; }
        public int DateOfWeek { get; set; }
        public string KyHieuCong { get; set; }
        public double SoGioOT { get; set; }
        public int SoPhutDiMuon { get; set; }
        public double CongQuyDoi { get; set; }
        public double GioOTQuyDoi { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public int LoaiNgay { get; set; }
        public double PhanTramLuong { get; set; }
        public double PhanTramLuongOT { get; set; }
    }

    public class CongQuyDoiDTO
    {
        public double CongQuyDoi { get; set; }
        public double CongOTQuyDoi { get; set; }
        public Guid ID_NhanVien { get; set; }
        public Guid ID_CaLamViec { get; set; }
        public DateTime NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }

    public class CongBoSungQuyDoiDTO
    {
        public Guid ID_CongBoSung { get; set; }
        public Guid? ID_CaLamViec { get; set; }
        public double CongQuyDoi { get; set; }
        public int LaPTCongQuyDoi { get; set; }
        public double CongOTQuyDoi { get; set; }
        public int LaPhanTramOTQuyDoi { get; set; }
    }


    public class BangLuongChiTietDTO
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public int LoaiLuong { get; set; }
        public double LuongCoBan { get; set; }
        public double NgayCongThuc { get; set; }
        public double NgayCongChuan { get; set; }
        public double SoGioOT { get; set; }
        public double SoLanDiMuon { get; set; }
        public double LuongChinh { get; set; }
        public double LuongOT { get; set; }
        public double PhuCapCoBan { get; set; }
        public double PhuCapKhac { get; set; }
        public double GiamTruCoDinhVND { get; set; }
        public double PhatDiMuon { get; set; }
        public double ChietKhau { get; set; }
        public double HoaHongHangHoa { get; set; }
        public double HoaHongHoaDon { get; set; }
        public double HoaHongDoanhThu { get; set; }
        public double TongDoanhThu { get; set; }
        public double KhenThuong { get; set; }
        public double PhuCap { get; set; }
        public double TongTienPhat { get; set; }
        public double GiamTruCoDinh_TheoPTram { get; set; }
        public double TongLuongNhan { get; set; } // = sum luong
        public double LuongThucNhan { get; set; } // = sum luong - phat + phucap ...
        public double PhuCapCoDinh_TheoPtramLuong { get; set; }
    }

    public class LuongChinhDTO
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public Guid ID_NhanVien { get; set; }
        public int LoaiLuong { get; set; }
        public string LuongCoBan { get; set; } // format
        public string LuongCoBanQuyDoi { get; set; } // format
        public DateTime NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public double SoNgayDiLam { get; set; }
        public int NgayCongChuan { get; set; }
        public double ThanhTien { get; set; }
        public Guid ID_CaLamViec { get; set; }
        public string TenCa { get; set; }
        public double? TongGioCong1Ca { get; set; }
        public int LoaiNgayThuong_Nghi { get; set; }
        public int HeSoLuong { get; set; }
        public int LaPhanTram { get; set; }
        public double SoCaApDung { get { return SoNgayDiLam; } }
    }

    public class LuongOT
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public Guid ID_NhanVien { get; set; }
        public int LoaiLuong { get; set; }
        public double Luong1GioCongCoBan { get; set; }
        public string Luong1GioCongQuyDoi { get; set; } // format
        public DateTime NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int HeSoLuong { get; set; }
        public int LoaiNgayThuong_Nghi { get; set; }
        public int LaPhanTram { get; set; }
        public Guid ID_CaLamViec { get; set; }
        public string TenCa { get; set; }
        public double TongGioCong1Ca { get; set; }
        public double SoGioOT { get; set; }
        public string ThanhTien { get; set; }// format
        public double SoGioOTApDung { get { return SoGioOT; } }
    }

    public class PhuCap
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string TenPhuCap { get; set; }
        public DateTime NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string PhuCapCoDinh { get; set; }
        public string ThanhTien { get; set; }
        public string PhuCapTheoNgay { get; set; }
        public double SoNgayDiLam { get; set; }
        public string ThanhTienPC_TheoNgay { get; set; }// format
        public double LoaiPhuCap { get; set; } // 51.theongay, 52.codinh vnd, 53.codinh %
    }
    public class GiamTru
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string TenPhuCap { get; set; }
        public DateTime NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string GiamTruCoDinh { get; set; }
        public string ThanhTien { get; set; }
        public string GiamTruTheoLan { get; set; }
        public double SoLanDiMuon { get; set; }
        public string ThanhTienGiamTruTheoLan { get; set; }// format
        public double LoaiGiamTru { get; set; } // 61.theolan, 62.codinh vnd, 63.codinh %
    }

    public class BangLuongDTO
    {
        public Guid? ID { get; set; }
        public Guid ID_DonVi { get; set; }
        public Guid ID_NhanVienDuyet { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string NguoiTao { get; set; }
        public string MaBangLuong { get; set; }
        public string TenBangLuong { get; set; }
        public int TrangThai { get; set; }
        public string GhiChu { get; set; }
        public string DienThoaiChiNhanh { get; set; }// use when print
        public string DiaChiChiNhanh { get; set; }

        // used to get list data bangluong
        public string NguoiDuyet { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayThanhToanLuong { get; set; }
        public double? LuongThucNhan { get; set; }
        public double? TruTamUngLuong { get; set; }
        public double? ThanhToan { get; set; }
        public double? DaTra { get; set; }
        public double? ConLai { get; set; }

        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public double? TongPhaiTra { get; set; }
        public double? TongTamUng { get; set; }
        public double? TongThanhToan { get; set; }
        public double? TongDaTra { get; set; }
        public double? TongConLai { get; set; }
    }

    public class BangCongDTO
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double CongChinh { get; set; }
        public double CongLamThem { get; set; }
        public double SoGioOT { get; set; }
        public double SoPhutDiMuon { get; set; }
        public double TongCong { get; set; }
        public double TongCongNgayNghi { get; set; }
        public double TongOT { get; set; }
        public double TongDiMuon { get; set; }
        public double TotalPage { get; set; }
        public int TotalRow { get; set; }
        public int? TrangThaiNV { get; set; }
    }
    public class NV_CaLamViecDTO
    {
        public Guid ID_CaLamViec { get; set; }
        public string MaCa { get; set; }
        public string TenCa { get; set; }
        public Guid ID_NhanVien { get; set; }
    }

    public class NgayNghiLeDTO
    {
        public DateTime NgayChamCong { get; set; }
        public int DateOfWeek { get; set; }
        public int LoaiNgay { get; set; }
    }

    public class ParamSearchLuong
    {
        public List<string> LstIDChiNhanh { get; set; }
        public List<string> LstIDNhanVien { get; set; }
        public List<string> LstKieuLuong { get; set; }
        public Guid? ID_PhieuPhanCa { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CurrentPage { get; set; }
        public float PageSize { get; set; }
    }

    public class NhanVienSamePhieu
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
    }

    public class QuyChiTietPhieuLuong
    {
        public Guid ID_NhanVien { get; set; }
        public Guid ID_BangLuongChiTiet { get; set; }
        public string TenNhanVien { get; set; }
        public string MaBangLuongChiTiet { get; set; }
        public double LuongThucNhan { get; set; }
        public double DaTra { get; set; }
        public double ConCanTra { get; set; }
        public string TienTra { get; set; } // = concantra: format 000,000 
        public double TamUngLuong { get; set; }
        public string TienTamUng { get; set; } // = TamUngLuong: format 000,000
        public double CanTraSauTamUng { get; set; }

        public double TongLuongNhan { get; set; }
        public double TongDaTra { get; set; }
        public double TongCanTra { get; set; }
        public double TongTamUng { get; set; }
        public double TongTruTamUngThucTe { get; set; }
        public double TongCanTraSauTamUng { get; set; }
    }

    public class SaoChep_ThietLapLuong
    {
        public Guid ID_ChiNhanh { get; set; }
        public Guid ID_NhanVien { get; set; }// saochep tu
        public List<string> LstIDNhanVien { get; set; } // saochep den
        public List<string> LstKieuLuong { get; set; }
        public Guid ID_NhanVienLogin { get; set; }
        public bool UpdateNVSetup { get; set; }
    }

    public class NS_PhieuPhanCaDTO
    {
        public Guid ID { get; set; }
        public Guid ID_DonVi { get; set; }
        public string MaPhieu { get; set; }
        public int LoaiPhanCa { get; set; }
        public string LoaiPhieuPhanCa { get; set; }
        public int TrangThai { get; set; }
        public string TrangThaiText { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string GhiChu { get; set; }
        public string TenNhanViens { get; set; }// ds NV thuocphieu
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class ThietLapLuongDTO
    {
        public Guid ID { get; set; }
        public Guid ID_NhanVien { get; set; }
        public Guid? ID_LoaiLuong { get; set; }
        public DateTime NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public double? SoTien { get; set; }
        public double? HeSo { get; set; }
        public int? LoaiLuong { get; set; }
        public string Bac { get; set; }
        public string NoiDung { get; set; }
        public int TrangThai { get; set; }
        public Guid? ID_DonVi { get; set; }
        public int? LoaiPhuCap_GiamTru { get; set; }
        public string TenLoaiLuong { get; set; }
    }
}
