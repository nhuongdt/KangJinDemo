using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Validation;
using libDM_DoiTuong;
using System.Data.SqlClient;
using libDonViQuiDoi;
using Model.Service.common;

namespace libDM_HangHoa
{
    public class ClassDM_HangHoa
    {
        private SsoftvnContext db;

        public ClassDM_HangHoa(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select

        public List<GetListHangHoaDatLichCheckinResult> GetListHangHoaDatLich()
        {
            return db.Database.SqlQuery<GetListHangHoaDatLichCheckinResult>("exec GetListHangHoaDatLichCheckin").ToList();
        }

        public List<DieuChinhGiaVon_HangHoaDTO> GetListHangHoa_byNhomHang(ParamSearchNhomHang param)
        {
            string idNhoms = string.Empty;
            string loaiHangs = "1,2,3";
            if (param.IDNhomHangs != null && param.IDNhomHangs.Count > 0)
            {
                idNhoms = string.Join(",", param.IDNhomHangs);
            }
            List<SqlParameter> prm = new List<SqlParameter>();
            prm.Add(new SqlParameter("ID_DonVi", param.ID_DonVi));
            prm.Add(new SqlParameter("IDNhomHangs", idNhoms ?? (object)DBNull.Value));
            prm.Add(new SqlParameter("LoaiHangHoas", loaiHangs));
            var xx = db.Database.SqlQuery<DieuChinhGiaVon_HangHoaDTO>("exec dbo.getListHangHoaBy_IDNhomHang @ID_DonVi, @IDNhomHangs," +
                " @LoaiHangHoas", prm.ToArray()).ToList();
            return xx;
        }
        public List<NhomHangHoa_SanPhamHoTroDTO> NhomHang_GetListSanPhamHoTro(Guid idNhom)
        {
            SqlParameter prm = new SqlParameter("ID_NhomHang", idNhom);
            var xx = db.Database.SqlQuery<NhomHangHoa_SanPhamHoTroDTO>("exec dbo.NhomHang_GetListSanPhamHoTro @ID_NhomHang", prm).ToList();
            return xx;
        }

        public List<NhomHangHoa_KhoangApDungDTO> GetListNhomHang_SetupHoTro(CommonParamSearch param)
        {
            string idDonVis = string.Empty;
            if (param.IDChiNhanhs != null)
            {
                idDonVis = string.Join(",", string.Empty);
            }
            SqlParameter prm = new SqlParameter("@IDDonVis", idDonVis ?? (object)DBNull.Value);
            var xx = db.Database.SqlQuery<NhomHangHoa_KhoangApDungDTO>("exec dbo.GetListNhomHang_SetupHoTro @IDDonVis", prm).ToList();
            return xx;
        }

        public List<DieuChinhGiaVon_HangHoaDTO> JqAutoHangHoa_withGiaVonTieuChuan(CommonParamSearch param)
        {
            string idChiNhanhs = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            List<SqlParameter> prm = new List<SqlParameter>();
            prm.Add(new SqlParameter("ID_DonVi", idChiNhanhs));
            prm.Add(new SqlParameter("TextSearch", param.TextSearch));
            prm.Add(new SqlParameter("DateTo", param.DateFrom ?? DateTime.Now));
            prm.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? 0));
            prm.Add(new SqlParameter("PageSize", param.PageSize ?? 500));
            var xx = db.Database.SqlQuery<DieuChinhGiaVon_HangHoaDTO>("exec dbo.SearchHangHoa_withGiaVonTieuChuan @ID_DonVi, @TextSearch," +
                " @DateTo, @CurrentPage, @PageSize", prm.ToArray()).ToList();
            return xx;
        }
        public List<SP_DM_HangHoaDTO> Gara_JqAutoHangHoa(Gara_ParamSearchHangHoa param)
        {
            List<SqlParameter> prm = new List<SqlParameter>();
            prm.Add(new SqlParameter("ID_DonVi", param.ID_ChiNhanh));
            prm.Add(new SqlParameter("ID_BangGia", param.ID_BangGia));
            prm.Add(new SqlParameter("TextSearch", param.TextSearch));
            prm.Add(new SqlParameter("LaHangHoa", param.LaHangHoa));
            prm.Add(new SqlParameter("QuanLyTheoLo", param.QuanLyTheoLo));
            prm.Add(new SqlParameter("ConTonKho", param.ConTonKho));
            prm.Add(new SqlParameter("Form", param.Form ?? 0));// 1.nhaphang, 0.other (0.bán hàng: chỉ hiện nếu có mã lô, và chưa hết hạn - nhập hàng: show all)
            prm.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? 0));
            prm.Add(new SqlParameter("PageSize", param.PageSize ?? 500));
            var xx = db.Database.SqlQuery<SP_DM_HangHoaDTO>("exec Gara_JqAutoHangHoa @ID_DonVi, @ID_BangGia, @TextSearch," +
                " @LaHangHoa, @QuanLyTheoLo, @ConTonKho, @Form, @CurrentPage, @PageSize", prm.ToArray()).ToList();
            return xx;
        }

        public void AddMultiple_DMGiaVon(Guid idDonVi, DM_GiaVon gv)
        {
            List<DM_GiaVon> lstGV = new List<DM_GiaVon>();
            var lstDonVi = db.DM_DonVi.Where(x => x.TrangThai != false && x.ID != idDonVi).Select(x => new { x.ID });
            foreach (var item in lstDonVi)
            {
                var itemGV = db.DM_GiaVon.Where(x => x.ID_DonVi == item.ID
                && x.ID_DonViQuiDoi == gv.ID_DonViQuiDoi && x.ID_LoHang == gv.ID_LoHang).Select(x => new { x.ID });

                if (itemGV == null || (itemGV != null && itemGV.Count() == 0))
                {
                    DM_GiaVon dmGV = new DM_GiaVon
                    {
                        ID = Guid.NewGuid(),
                        ID_DonVi = item.ID,
                        ID_DonViQuiDoi = gv.ID_DonViQuiDoi,
                        ID_LoHang = gv.ID_LoHang,
                        GiaVon = 0,
                    };
                    lstGV.Add(dmGV);
                }
            }
            db.DM_GiaVon.AddRange(lstGV);
        }
        public void AddMultiple_DMHangHoaTonKho(Guid idDonVi, DM_HangHoa_TonKho tk)
        {
            List<DM_HangHoa_TonKho> lstDMTK = new List<DM_HangHoa_TonKho>();
            var lstDonVi = db.DM_DonVi.Where(x => x.TrangThai != false && x.ID != idDonVi).Select(x => new { x.ID });
            foreach (var item in lstDonVi)
            {
                var itemTK = db.DM_HangHoa_TonKho.Where(x => x.ID_DonVi == item.ID
               && x.ID_DonViQuyDoi == tk.ID_DonViQuyDoi && x.ID_LoHang == tk.ID_LoHang).Select(x => new { x.ID });

                if (itemTK == null || (itemTK != null && itemTK.Count() == 0))
                {
                    DM_HangHoa_TonKho dmTK = new DM_HangHoa_TonKho
                    {
                        ID = Guid.NewGuid(),
                        ID_DonVi = item.ID,
                        ID_DonViQuyDoi = tk.ID_DonViQuyDoi,
                        ID_LoHang = tk.ID_LoHang,
                        TonKho = 0,
                    };
                    lstDMTK.Add(dmTK);
                }
            }
            db.DM_HangHoa_TonKho.AddRange(lstDMTK);// dont'save change (avoid douple primary key)
        }

        public List<SP_DM_HangHoaDTO> Gara_GetListHangHoa_ByIDQuiDoi(Gara_ParamSearchHangHoa param)
        {
            var idQuyDois = string.Empty;
            if (param.LstIDQuiDois != null && param.LstIDQuiDois.Count > 0)
            {
                idQuyDois = string.Join(",", param.LstIDQuiDois);
            }
            List<SqlParameter> prm = new List<SqlParameter>();
            prm.Add(new SqlParameter("ID_DonVi", param.ID_ChiNhanh));
            prm.Add(new SqlParameter("ID_BangGia", param.ID_BangGia));
            prm.Add(new SqlParameter("IDQuiDois", idQuyDois));
            return db.Database.SqlQuery<SP_DM_HangHoaDTO>("exec Gara_GetListHangHoa_ByIDQuiDoi @ID_DonVi, @ID_BangGia, @IDQuiDois ", prm.ToArray()).ToList();
        }

        public List<SP_ThanhPhan_DinhLuong> SP_GetInfor_TPDinhLuong(Guid idDonVi, Guid idDichVu)
        {
            try
            {
                // use when get all TP dinh luong of ChiNhanh
                var idDichVuSearch = "%%";
                if (idDichVu != Guid.Empty)
                {
                    idDichVuSearch = idDichVu.ToString();
                }
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_DonVi", idDonVi));
                prm.Add(new SqlParameter("ID_DichVu", idDichVuSearch));
                return db.Database.SqlQuery<SP_ThanhPhan_DinhLuong>("exec GetAllDinhLuongDichVu @ID_DonVi, @ID_DichVu", prm.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetInfor_TPDinhLuong: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        // used to assign tonkho = thucte at kiemhangchitiet
        public List<KiemKho_HangHoaTonKho> GetTonKho_byIDQuyDois(KiemKhoParamSearch param)
        {
            if (param.ListIDQuyDoi != null && param.ListIDQuyDoi.Count() > 0)
            {
                var idQuyDois = string.Join(",", param.ListIDQuyDoi);
                var idLoHangs = string.Empty;
                if (param.ListIDLoHang != null && param.ListIDLoHang.Count > 0)
                {
                    idLoHangs = string.Join(",", param.ListIDLoHang);
                }
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_ChiNhanh", param.ID_ChiNhanh));
                prm.Add(new SqlParameter("ToDate", param.ToDate));
                prm.Add(new SqlParameter("IDDonViQuyDois", idQuyDois));
                prm.Add(new SqlParameter("IDLoHangs", idLoHangs));
                return db.Database.SqlQuery<KiemKho_HangHoaTonKho>("exec GetTonKho_byIDQuyDois @ID_ChiNhanh, @ToDate, @IDDonViQuyDois, @IDLoHangs", prm.ToArray()).ToList();
            }
            else
            {
                return new List<KiemKho_HangHoaTonKho>();
            }
        }

        /// <summary>
        /// Check hang hoa dang kinh doanh and chua bi xoa
        /// </summary>
        /// <param name="maHangHoa"></param>
        /// <returns>true/false</returns>
        public bool SP_CheckHangDangKinhDoanh(string maHangHoa)
        {
            try
            {
                SqlParameter param = new SqlParameter("MaHangHoa", maHangHoa);
                var objReturn = db.Database.SqlQuery<SP_ReturnBool>("EXEC SP_CheckHangHoa_DangKinhDoanh @MaHangHoa", param).ToList();
                if (objReturn != null)
                {
                    return objReturn.FirstOrDefault().Exist;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_CheckHangDangKinhDoanh: " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }

        /// <summary>
        /// check hang exist and quan ly theo lo 
        /// </summary>
        /// <param name="maHangHoa"></param>
        /// <returns>true/false</returns>
        public bool SP_CheckHangHoa_QuanLyTheoLo(string maHangHoa)
        {
            try
            {
                SqlParameter param = new SqlParameter("MaHangHoa", maHangHoa);
                var objReturn = db.Database.SqlQuery<SP_ReturnBool>("EXEC SP_CheckHangHoa_QuanLyTheoLo @MaHangHoa", param).ToList();
                if (objReturn != null)
                {
                    return objReturn.FirstOrDefault().Exist;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_CheckHangHoa_QuanLyTheoLo: " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }

        /// <summary>
        /// check HangHoa with LoHang exist
        /// </summary>
        /// <param name="maHangHoa"></param>
        /// <param name="maLoHang"></param>
        /// <returns>true/false</returns>
        public bool SP_CheckLoHangExist(string maHangHoa, string maLoHang)
        {
            try
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("MaHangHoa", maHangHoa));
                paramlist.Add(new SqlParameter("MaLoHang", maLoHang));

                var objReturn = db.Database.SqlQuery<SP_ReturnBool>("EXEC SP_CheckLoHangExist @MaHangHoa, @MaLoHang", paramlist.ToArray()).ToList();
                if (objReturn != null)
                {
                    return objReturn.FirstOrDefault().Exist;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_CheckLoHangExist: " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }

        public class SP_ThanhPhan_DinhLuong
        {
            public Guid ID { get; set; }
            public Guid ID_DichVu { get; set; }
            public Guid ID_DonViQuiDoi { get; set; }
            public Guid? ID_LoHang { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaThayThe { get; set; }
            public double GiaVon { get; set; }
            public double SoLuong { get; set; }
            public double? SoLuongMacDinh { get; set; }// soluong setup bandau
            public double? SoLuongDinhLuong_BanDau { get; set; }
            public double? SoLuongQuyCach { get; set; }
            public double QuyCach { get; set; }
            public double TyLeChuyenDoi { get; set; }
            public string DonViTinhQuyCach { get; set; }
            public string TenDonViTinh { get; set; }
            public double? TonKho { get; set; }
            public double? ThanhTien { get; set; }
            public int? LoaiHangHoa { get; set; }
            public double? DonGia { get; set; }// dongia tpcombo (setup at dm_hanghoa)
            public double? GiaBan { get; set; }// giabanle of hanghoa
            public string MaLoHang { get; set; }
        }

        public List<DM_HangHoaPRG> Select_HangHoaPRG(Guid id)
        {
            List<SqlParameter> prm = new List<SqlParameter>();
            prm.Add(new SqlParameter("ID_HangHoa", id));
            List<DM_HangHoaPRG> lst = new List<DM_HangHoaPRG>();
            lst = db.Database.SqlQuery<DM_HangHoaPRG>("exec getList_ChiTietHangHoaXuatHuy @ID_HangHoa", prm.ToArray()).ToList();
            return lst;
        }
        public class DM_HangHoaPRG
        {
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
        }
        public DM_HangHoa Select_HangHoa(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_HangHoa.Find(id);
            }
        }

        public string AddDM_GiaVon(DM_GiaVon objAdd)
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
                    db.DM_GiaVon.Add(objAdd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("AddDM_GiaVon: " + ex.InnerException + ex.Message);
                        strErr = "AddDM_GiaVon: " + ex.InnerException + ex.Message;
                    }
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

        public DM_ThuocTinh Select_ThuocTinh(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_ThuocTinh.Find(id);
            }
        }
        public List<List_DonViQuiDoi_HH> getListHangHoaBy_MaHangHoa(string maHangHoa)
        {
            List<List_DonViQuiDoi_HH> lst = new List<List_DonViQuiDoi_HH>();
            var tbl = from dvqd in db.DonViQuiDois
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      where dvqd.Xoa == null & hh.TheoDoi == true
                      select new List_DonViQuiDoi_HH
                      {
                          ID = dvqd.ID,
                          ID_HangHoa = dvqd.ID_HangHoa,
                          MaHangHoa = dvqd.MaHangHoa,
                          TenHangHoa = hh.TenHangHoa,
                          TenDonViTinh = dvqd.TenDonViTinh,
                          GiaVon = dvqd.GiaVon
                      };
            var tb_fomat = tbl.AsEnumerable().Select(t => new List_DonViQuiDoi_HH
            {
                ID = t.ID,
                ID_HangHoa = t.ID_HangHoa,
                MaHangHoa = t.MaHangHoa,
                TenDonViTinh = t.TenDonViTinh,
                TenHangHoa = t.TenHangHoa,
                ThuocTinh_GiaTri = Select_HangHoaPRG(t.ID_HangHoa).FirstOrDefault().ThuocTinh_GiaTri
            });
            try
            {
                lst = tb_fomat.ToList();
            }
            catch
            {
                lst = null;
            }
            return lst;
        }

        public List<List_TenDonViTinh> getdonviquidoibymahanghoa(string maHangHoa)
        {
            List<List_TenDonViTinh> lst = new List<List_TenDonViTinh>();
            var tbl = from dvqd in db.DonViQuiDois.Where(x => x.MaHangHoa == maHangHoa)
                      join dvhh in db.DonViQuiDois on dvqd.ID_HangHoa equals dvhh.ID_HangHoa
                      orderby dvhh.LaDonViChuan descending
                      select new List_TenDonViTinh
                      {
                          ID = dvhh.ID,
                          MaHangHoa = dvhh.MaHangHoa,
                          TenDonViTinh = dvhh.TenDonViTinh
                      };
            try
            {
                lst = tbl.ToList();
            }
            catch
            {
                lst = null;
            }
            return lst;
        }

        public List<DM_HangHoaInMaVach> GetHangHoaByID_BangGIa(Guid idgiaban, Guid iddvqd)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("ID_GiaBan", idgiaban));
                param.Add(new SqlParameter("ID_DonViQuiDoi", iddvqd));
                List<DM_HangHoaInMaVach> listTon = db.Database.SqlQuery<DM_HangHoaInMaVach>("exec GetHangHoaByID_BangGia @ID_GiaBan,@ID_DonViQuiDoi", param.ToArray()).ToList();
                return listTon;
            }
        }

        public List<DM_HangHoaDTO> GetHangHoaByID_BangGIaNotHH(Guid iddvqd)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl =
                          from dvqd in db.DonViQuiDois
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          where dvqd.ID == iddvqd
                          select new DM_HangHoaDTO
                          {
                              ID = dvqd.ID_HangHoa,
                              ID_DonViQuiDoi = dvqd.ID,
                              GiaBan = dvqd.GiaBan,
                              MaHangHoa = dvqd.MaHangHoa,
                              TenHangHoa = hh.TenHangHoa,
                              TenDonViTinh = dvqd.TenDonViTinh
                          };
                return tbl.ToList();
            }
        }

        //public List<DM_HangHoa> GetListHangHoaDatLich()
        //{
        //    List<DM_HangHoa> lstresult = db.DM_HangHoa.Take(10).ToList();
        //    return lstresult;
        //}

        public HangHoa_ThuocTinh SelectHH_ThuocTinh(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HangHoa_ThuocTinh.Find(id);
            }
        }

        public List<DM_HangHoa> GetIDHangHoaByID_CungLoai(Guid idcungloai)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from hh in db.DM_HangHoa
                          where hh.ID_HangHoaCungLoai == idcungloai
                          select new
                          {
                              hh.ID
                          };
                List<DM_HangHoa> lst = new List<DM_HangHoa>();
                foreach (var item in tbl)
                {
                    DM_HangHoa DM = new DM_HangHoa();
                    DM.ID = item.ID;
                    lst.Add(DM);
                }
                return lst;
            }
        }


        public List<DM_NhomHangHoa> getNhomHangHoa()
        {
            var tbl = from nh in db.DM_NhomHangHoa
                      select new
                      {
                          nh.ID,
                          nh.MaNhomHangHoa,
                          nh.TenNhomHangHoa,
                          nh.NgayTao
                      };
            List<DM_NhomHangHoa> lst = new List<DM_NhomHangHoa>();
            foreach (var item in tbl)
            {
                DM_NhomHangHoa DM = new DM_NhomHangHoa();
                DM.ID = item.ID;
                DM.MaNhomHangHoa = item.MaNhomHangHoa;
                DM.TenNhomHangHoa = item.TenNhomHangHoa;
                DM.NgayTao = item.NgayTao;
                lst.Add(DM);
            }
            if (lst != null)
            {
                return lst.OrderByDescending(p => p.NgayTao).ToList();
            }
            else
            {
                return null;
            }
        }
        public List<DM_NhomHangHoa> seachNhomHangHoa(string TenNhom)
        {
            TenNhom = CommonStatic.ConvertToUnSign(TenNhom).ToLower();
            var tbl = from nh in db.DM_NhomHangHoa
                          //where nh.TenNhomHangHoa.Contains(@TenNhom)
                      orderby nh.NgayTao
                      select new
                      {
                          nh.ID,
                          nh.MaNhomHangHoa,
                          nh.TenNhomHangHoa
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new DM_NhomHangHoa
            {
                ID = t.ID,
                MaNhomHangHoa = t.MaNhomHangHoa,
                TenNhomHangHoa = t.TenNhomHangHoa,
                NguoiSua = CommonStatic.ConvertToUnSign(t.TenNhomHangHoa).ToLower(),
                GhiChu = CommonStatic.GetCharsStart(t.TenNhomHangHoa).ToLower(),
            });
            //tbl1 = tbl1.Where(x => x.TenNhomHangHoa.Contains(@TenNhom) || x.GhiChu.Contains(@TenNhom));
            List<DM_NhomHangHoa> lst = new List<DM_NhomHangHoa>();
            lst = tbl1.Where(x => x.NguoiSua.Contains(@TenNhom) || x.GhiChu.Contains(@TenNhom)).ToList();
            //foreach (var item in tbl)
            //{
            //    DM_NhomHangHoa DM = new DM_NhomHangHoa();
            //    DM.ID = item.ID;
            //    DM.MaNhomHangHoa = item.MaNhomHangHoa;
            //    DM.TenNhomHangHoa = item.TenNhomHangHoa;
            //    lst.Add(DM);
            //}
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<DM_HangHoa> Gets(Expression<Func<DM_HangHoa, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_HangHoa;
                else
                    return db.DM_HangHoa.Where(query);
            }
        }

        public List<DM_HangHoa_Anh> GetsAnh(Expression<Func<DM_HangHoa_Anh, bool>> query)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_HangHoa_Anh.ToList();
                else
                    return db.DM_HangHoa_Anh.Where(query).ToList();
            }
        }

        public List<HangHoa_ThuocTinh> GetHH_ThuocTinh(Expression<Func<HangHoa_ThuocTinh, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.HangHoa_ThuocTinh.ToList();
                else
                    return db.HangHoa_ThuocTinh.Where(query).ToList();
            }
        }

        public List<HangHoa_ThuocTinh> GetHangHoaThuocTinh(Guid idthuoctinh)
        {
            List<HangHoa_ThuocTinh> lst = new List<HangHoa_ThuocTinh>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from hhtt in db.HangHoa_ThuocTinh
                          where hhtt.ID_ThuocTinh == idthuoctinh
                          select new
                          {
                              ID = hhtt.ID,
                              ID_ThuocTinh = hhtt.ID_ThuocTinh,
                              GiaTri = hhtt.GiaTri,
                              ThuTuNhap = hhtt.ThuTuNhap
                          };
                foreach (var item in tbl)
                {
                    HangHoa_ThuocTinh hhtt = new HangHoa_ThuocTinh();
                    hhtt.ID = item.ID;
                    hhtt.ID_ThuocTinh = item.ID_ThuocTinh;
                    hhtt.GiaTri = item.GiaTri;
                    hhtt.ThuTuNhap = item.ThuTuNhap;
                    lst.Add(hhtt);
                }
                return lst.GroupBy(p => p.GiaTri.ToLower()).Select(t => new HangHoa_ThuocTinh
                {
                    ID = t.FirstOrDefault().ID,
                    ID_ThuocTinh = t.FirstOrDefault().ID_ThuocTinh,
                    GiaTri = t.FirstOrDefault().GiaTri
                }).OrderBy(p => p.ThuTuNhap).ToList();
            }
        }

        public List<DM_HangHoa> gethanghoacungloainotxoa(Guid idcungloai)
        {
            List<DM_HangHoa> lst = new List<DM_HangHoa>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from dvqd in db.DonViQuiDois
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          where hh.ID_HangHoaCungLoai == idcungloai && dvqd.Xoa != true && hh.TheoDoi == true && dvqd.LaDonViChuan == true
                          select new
                          {
                              ID = dvqd.ID_HangHoa,
                              ID_HangHoaCungLoai = hh.ID_HangHoaCungLoai,
                              NgayTao = dvqd.NgayTao
                          };
                foreach (var item in tbl)
                {
                    DM_HangHoa objHH = new DM_HangHoa();
                    objHH.ID = item.ID;
                    objHH.ID_HangHoaCungLoai = item.ID_HangHoaCungLoai;
                    objHH.NgayTao = item.NgayTao;
                    lst.Add(objHH);
                }
                return lst;
            }
        }


        public List<DemCheck> gethanghoacungloainotxoaDEMCL(Guid idcungloai)
        {
            List<DemCheck> lst = new List<DemCheck>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from dvqd in db.DonViQuiDois
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          where hh.ID_HangHoaCungLoai == idcungloai && dvqd.Xoa != true
                          orderby hh.NgayTao descending
                          select new DemCheck
                          {
                              ID_DonViQuiDoi = dvqd.ID,
                              ID_HangHoaCungLoai = hh.ID_HangHoaCungLoai,
                              ID = dvqd.ID_HangHoa
                          };
                foreach (var item in tbl)
                {
                    DemCheck objHH = new DemCheck();
                    objHH.ID = item.ID;
                    objHH.ID_HangHoaCungLoai = item.ID_HangHoaCungLoai;
                    objHH.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                    lst.Add(objHH);
                }
                return lst;
            }
        }

        public List<DemCheck> gethanghoacungloainotxoaKoCoCL(Guid idcungloai)
        {
            List<DemCheck> lst = new List<DemCheck>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from dvqd in db.DonViQuiDois
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          where dvqd.ID_HangHoa == idcungloai && dvqd.Xoa != true
                          select new DemCheck
                          {
                              ID_DonViQuiDoi = dvqd.ID,
                              ID_HangHoaCungLoai = hh.ID_HangHoaCungLoai,
                              ID = dvqd.ID_HangHoa
                          };
                foreach (var item in tbl)
                {
                    DemCheck objHH = new DemCheck();
                    objHH.ID = item.ID;
                    objHH.ID_HangHoaCungLoai = item.ID_HangHoaCungLoai;
                    objHH.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                    lst.Add(objHH);
                }
                return lst;
            }
        }

        public List<DemCheck> gethanghoacungloainotdemCL(Guid id_hanghoa)
        {
            List<DemCheck> lst = new List<DemCheck>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from hh in db.DM_HangHoa
                          where hh.ID == id_hanghoa
                          group new { hh } by new
                          {
                              hh.ID,
                              hh.ID_HangHoaCungLoai
                          } into g
                          select new DemCheck
                          {
                              ID = g.Key.ID,
                              ID_HangHoaCungLoai = g.Key.ID_HangHoaCungLoai
                          };
                foreach (var item in tbl)
                {
                    DemCheck objHH = new DemCheck();
                    objHH.ID = item.ID;
                    objHH.ID_HangHoaCungLoai = item.ID_HangHoaCungLoai;
                    lst.Add(objHH);
                }
                return lst;
            }
        }

        public List<HangHoa_ThuocTinh> SelectHangHoa_ThuocTinh(Guid id)
        {
            List<HangHoa_ThuocTinh> lst = new List<HangHoa_ThuocTinh>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from tthh in db.HangHoa_ThuocTinh
                          where tthh.ID_HangHoa == id
                          select new
                          {
                              tthh.ID,
                              tthh.ID_HangHoa,
                              tthh.ID_ThuocTinh,
                              tthh.GiaTri,
                              tthh.ThuTuNhap
                          };
                foreach (var item in tbl)
                {
                    HangHoa_ThuocTinh objHH = new HangHoa_ThuocTinh();
                    objHH.ID = item.ID;
                    objHH.ID_HangHoa = item.ID_HangHoa;
                    objHH.ID_ThuocTinh = item.ID_ThuocTinh;
                    objHH.GiaTri = item.GiaTri;
                    objHH.ThuTuNhap = item.ThuTuNhap;
                    lst.Add(objHH);
                }
                return lst;
            }
        }

        //Thuộc tính hàng hóa
        public List<DM_ThuocTinh> GetAllThuocTinh(Expression<Func<DM_ThuocTinh, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_ThuocTinh.ToList();
                else
                    return db.DM_ThuocTinh.Where(query).ToList();
            }
        }


        public DM_HangHoa Get(Expression<Func<DM_HangHoa, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_HangHoa.Where(query).FirstOrDefault();
            }
        }

        public DM_HangHoa GetID_QuiDoi(Expression<Func<DM_HangHoa, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_HangHoa.Where(query).FirstOrDefault();
            }
        }

        public bool DM_HangHoaExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {

                return db.DM_HangHoa.Count(e => e.ID == id) > 0;
            }
        }

        public bool Check_MaHangHoaExist(string maHangHoa, Guid? idQuiDoi = null)
        {
            if (db == null || string.IsNullOrEmpty(maHangHoa))
            {
                return false;
            }
            else
            {
                maHangHoa = maHangHoa.Trim();
                if (idQuiDoi == null || idQuiDoi == Guid.Empty)
                {
                    return db.DonViQuiDois.Where(p => p.ID != idQuiDoi && p.MaHangHoa == maHangHoa).Count() > 0;
                }
                else
                {
                    return db.DonViQuiDois.Where(p => p.MaHangHoa == maHangHoa).Count() > 0;
                }
            }
        }

        public bool Check_TenDVTExist(string tendvt)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DonViQuiDois.Count(e => e.TenDonViTinh == tendvt) > 0;
            }
        }

        public bool Check_MaHangHoaExistDVT(string maHangHoa, Guid idhangHoa)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DonViQuiDois.Count(e => e.MaHangHoa == maHangHoa && e.ID_HangHoa != idhangHoa) > 0;
            }
        }

        public bool Check_HangHoaLaThanhPhanDichVu(Guid idhangHoa)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                var tbl = from dldv in db.DinhLuongDichVus
                          join dvqd in db.DonViQuiDois on dldv.ID_DonViQuiDoi equals dvqd.ID
                          where dvqd.ID_HangHoa == idhangHoa
                          select new
                          {
                              ID = dldv.ID
                          };
                if (tbl.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //public static List<DM_HangHoaDTO> GetHangCungLoai(Guid id_cungloai, Guid iddonvi)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();

        //    //var tbl_hanghoa = (from hh in db.DM_HangHoa
        //    //                   join nhh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nhh.ID into listHH
        //    //                   from list in listHH.DefaultIfEmpty()
        //    //                   join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa
        //    //                   where dvt.Xoa != true && dvt.LaDonViChuan == true && hh.ID_HangHoaCungLoai == id_cungloai
        //    //                   orderby hh.NgayTao ascending
        //    //                   select new
        //    //                   {
        //    //                       ID = dvt.ID_HangHoa, // ID_dvquydoi
        //    //                       ID_HangHoaCungLoai = hh.ID_HangHoaCungLoai,
        //    //                       MaHangHoa = dvt.MaHangHoa,
        //    //                       TenHangHoa = hh.TenHangHoa,
        //    //                       TenHangHoaUnsign = hh.TenHangHoa_KhongDau,
        //    //                       TenHangHoaCharStart = hh.TenHangHoa_KyTuDau,
        //    //                       GiaBan = dvt.GiaBan,
        //    //                       GiaVon = dvt.GiaVon,
        //    //                       ThanhTien = dvt.GiaBan, // 1* DonGia
        //    //                       TenDonViTinh = dvt.TenDonViTinh,
        //    //                       ID_NhomHangHoa = hh.ID_NhomHang,
        //    //                       ID_DonViQuiDoi = dvt.ID,
        //    //                       NhomHangHoa = list.TenNhomHangHoa,
        //    //                       LaHangHoa = hh.LaHangHoa,
        //    //                       TrangThai = hh.TheoDoi,
        //    //                       DuocBanTrucTiep = hh.DuocBanTrucTiep,
        //    //                       Xoa = dvt.Xoa,
        //    //                       NgayTao = hh.NgayTao,
        //    //                   });
        //    // var tbl = tbl_hanghoa.AsEnumerable().Select(hh => new DM_HangHoaDTO
        //    //{
        //    //    ID = hh.ID,
        //    //    ID_HangHoaCungLoai = hh.ID_HangHoaCungLoai,
        //    //    MaHangHoa = hh.MaHangHoa,
        //    //    TenHangHoa = hh.TenHangHoa,
        //    //    TenHangHoaUnsign = hh.TenHangHoaUnsign,
        //    //    TenHangHoaCharStart = hh.TenHangHoaCharStart,
        //    //    TonKho = TinhSLTonHH(hh.ID, iddonvi),
        //    //    GiaBan = hh.GiaBan,
        //    //    GiaVon = hh.GiaVon,
        //    //    TenDonViTinh = hh.TenDonViTinh,
        //    //    ID_NhomHangHoa = hh.ID_NhomHangHoa,
        //    //    ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
        //    //    NhomHangHoa = hh.NhomHangHoa,
        //    //    LaHangHoa = hh.LaHangHoa,
        //    //    TrangThai = hh.TrangThai,
        //    //    DuocBanTrucTiep = hh.DuocBanTrucTiep,
        //    //    NgayTao = hh.NgayTao
        //    //});
        //    List<SqlParameter> paramlist = new List<SqlParameter>();
        //    Guid ID_ChiNhanh = iddonvi;
        //    paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
        //    paramlist.Add(new SqlParameter("ID_CungLoai", id_cungloai));
        //    List<DM_HangHoaDTO> listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec LoadHangHoaCungLoai @ID_ChiNhanh,@ID_CungLoai", paramlist.ToArray()).ToList();
        //    return listTon.OrderBy(p => p.NgayTao).ToList();
        //}

        public List<DM_HangHoaDTO> GetHangHoa_ByIDNhom(Guid id)
        {

            var tbl = from hh in db.DM_HangHoa
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa
                      where hh.ID_NhomHang == id
                      select new
                      {
                          dvt.ID,
                          hh.TenHangHoa,
                          hh.TheoDoi,
                          dvt.MaHangHoa,
                          dvt.GiaVon,
                          dvt.GiaBan,
                          dvt.TenDonViTinh
                      };

            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            foreach (var item in tbl)
            {
                DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                dM_HangHoaDTO.ID = item.ID; // ID_dvquydoi
                dM_HangHoaDTO.MaHangHoa = item.MaHangHoa;
                dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                dM_HangHoaDTO.SoLuong = 1;
                dM_HangHoaDTO.GiaBan = item.GiaBan;
                dM_HangHoaDTO.GiaVon = Math.Round((double)item.GiaVon, MidpointRounding.ToEven); ; // 1* DonGia
                dM_HangHoaDTO.GiamGia = 0;
                dM_HangHoaDTO.TenDonViTinh = item.TenDonViTinh;

                lst.Add(dM_HangHoaDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }

        public List<DM_HangHoaDTO> GetHangHoa_ByIDDVTThaoTac(Guid id)
        {
            var tbl = from hh in db.DM_HangHoa
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa
                      where dvt.ID == id
                      select new
                      {
                          hh.ID,
                          dvt.MaHangHoa,
                          hh.TenHangHoa,
                          hh.ID_NhomHang,
                          hh.LaHangHoa,
                          hh.GhiChu,
                          hh.QuyCach,
                          hh.DuocBanTrucTiep
                      };

            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            foreach (var item in tbl)
            {
                DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                dM_HangHoaDTO.ID = item.ID; // ID_dvquydoi
                dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                dM_HangHoaDTO.MaHangHoa = item.MaHangHoa;
                dM_HangHoaDTO.ID_NhomHangHoa = item.ID_NhomHang;
                dM_HangHoaDTO.LaHangHoa = item.LaHangHoa;
                dM_HangHoaDTO.GhiChu = item.GhiChu; // 1* DonGia
                dM_HangHoaDTO.QuyCach = item.QuyCach;
                dM_HangHoaDTO.DuocBanTrucTiep = item.DuocBanTrucTiep;

                lst.Add(dM_HangHoaDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }

        public List<DM_HangHoaDTO> GetAllHangHoa(string maHoaDon, int tonkho, int kinhdoanh,
            int trangThai)
        {
            var tbl = from hh in db.DM_HangHoa
                      join nhh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nhh.ID
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa
                      select new DM_HangHoaDTO
                      {
                          ID = dvt.ID_HangHoa, // ID_dvquydoi
                          ID_NhomHangHoa = hh.ID_NhomHang,
                          NhomHangHoa = nhh.TenNhomHangHoa,
                          MaHangHoa = dvt.MaHangHoa,
                          TenHangHoa = hh.TenHangHoa,
                          SoLuong = 1,
                          GiaBan = dvt.GiaBan,
                          GiaVon = dvt.GiaVon, // 1* DonGia
                          GiamGia = 0,
                          TenDonViTinh = dvt.TenDonViTinh,
                          ID_DonViQuiDoi = dvt.ID,
                      };

            if (maHoaDon != string.Empty && maHoaDon != null)
            {
                tbl = tbl.Where(hd => hd.MaHangHoa.Contains(maHoaDon) || hd.TenHangHoa.Contains(maHoaDon) || hd.NhomHangHoa.Contains(maHoaDon));
            }
            // loại hàng
            switch (trangThai)
            {
                case 1: // HT
                    tbl = tbl.Where(hd => hd.LaHangHoa == true);
                    break;
                case 2: // Huy
                    tbl = tbl.Where(hd => hd.LaHangHoa == false);
                    break;
                case 3: // HT + Huy
                    break;
                default: // tam luu
                    tbl = tbl.Where(hd => hd.LaHangHoa == true);
                    break;
            }
            //tồn kho

            switch (kinhdoanh)
            {
                case 1: // HT
                    tbl = tbl.Where(hd => hd.TrangThai == false);
                    break;
                case 2: // Huy
                    tbl = tbl.Where(hd => hd.TrangThai == true);
                    break;
                case 3: // HT + Huy
                    break;
                default: // tam luu
                    tbl = tbl.Where(hd => hd.TrangThai == true);
                    break;
            }
            return tbl.ToList();
        }

        public List<DM_HangHoaDTO> GetHangHoa_ByIDNhomKK(Guid id, Guid iddonvi)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
            paramlist.Add(new SqlParameter("ID_NhomHangHoa", id));
            var listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec AddHHByNhomHangHoaKiemKho @ID_ChiNhanh, @ID_NhomHangHoa", paramlist.ToArray()).ToList();
            return listTon;
        }

        public List<DM_HangHoaDTO> GetHangHoa_QuyDoiMH(string mahh, Guid iddonvi)
        {
            var tbl = from hh in db.DM_HangHoa
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa into HH_QD
                      from hh_qd in HH_QD.DefaultIfEmpty()
                      where hh_qd.MaHangHoa == mahh && hh.LaHangHoa == true && hh_qd.Xoa == null && hh.TheoDoi == true
                      select new
                      {
                          ID = hh_qd.ID_HangHoa,
                          TenHangHoa = hh.TenHangHoa,
                          MaHangHoa = hh_qd.MaHangHoa,
                          SoLuong = 1,
                          GiaBan = hh_qd.GiaBan,
                          GiaNhap = hh_qd.GiaNhap,
                          GiaVon = hh_qd.GiaVon,
                          GiamGia = 0,
                          TenDonViTinh = hh_qd.TenDonViTinh,
                          ID_DonViQuiDoi = hh_qd.ID,
                          TyLeChuyenDoi = hh_qd.TyLeChuyenDoi,
                          QuanLyTheoLoHang = hh.QuanLyTheoLoHang
                      };

            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            if (tbl != null && tbl.Count() > 0)
            {
                foreach (var item in tbl)
                {
                    DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                    dM_HangHoaDTO.ID = item.ID;
                    dM_HangHoaDTO.MaHangHoa = item.MaHangHoa;
                    dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                    dM_HangHoaDTO.SoLuong = 1;
                    dM_HangHoaDTO.TonKho = Math.Round(TinhSLTonHH(item.ID, iddonvi).Value / item.TyLeChuyenDoi, 3, MidpointRounding.ToEven);
                    dM_HangHoaDTO.GiaBan = item.GiaBan;
                    dM_HangHoaDTO.GiaVon = Math.Round((double)item.GiaVon, MidpointRounding.ToEven); // 1* DonGia
                    dM_HangHoaDTO.GiaNhap = item.GiaNhap;
                    dM_HangHoaDTO.GiamGia = 0;
                    dM_HangHoaDTO.TenDonViTinh = item.TenDonViTinh;
                    dM_HangHoaDTO.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                    dM_HangHoaDTO.QuanLyTheoLoHang = item.QuanLyTheoLoHang;
                    dM_HangHoaDTO.HangHoa_ThuocTinh = SelectHangHoa_ThuocTinh(item.ID).Select(x => new HangHoa_ThuocTinh
                    {
                        GiaTri = x.GiaTri,
                        ThuTuNhap = x.ThuTuNhap
                    }).OrderBy(c => c.ThuTuNhap).ToList();
                    lst.Add(dM_HangHoaDTO);
                }
            }
            if (lst.Count > 0)
                return lst;
            else
                return new List<DM_HangHoaDTO>();
        }

        public List<DM_HangHoaDTO> GetHangHoa_QuyDoi(Guid id, Guid iddonvi)
        {
            ClassBH_HoaDon _classBHHD = new ClassBH_HoaDon(db);
            var tbl = from hh in db.DM_HangHoa
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa into HH_QD
                      from hh_qd in HH_QD.DefaultIfEmpty()
                      where hh_qd.ID == id
                      //new Guid("6eb8edd4-1ed2-46a3-a38f-353bfb566ce6")
                      select new
                      {
                          //hh_qd.ID,
                          //hh.TenHangHoa,
                          //hh.TheoDoi,
                          //hh_qd.MaHangHoa,
                          //hh_qd.GiaVon,
                          //hh_qd.GiaBan,
                          //hh_qd.TenDonViTinh
                          ID = hh_qd.ID_HangHoa,
                          TenHangHoa = hh.TenHangHoa,
                          MaHangHoa = hh_qd.MaHangHoa,
                          SoLuong = 1,
                          GiaBan = hh_qd.GiaBan,
                          GiaNhap = hh_qd.GiaNhap,
                          GiaVon = hh_qd.GiaVon,
                          GiamGia = 0,
                          TenDonViTinh = hh_qd.TenDonViTinh,
                          ID_DonViQuiDoi = hh_qd.ID,
                          TyLeChuyenDoi = hh_qd.TyLeChuyenDoi,
                      };

            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            foreach (var item in tbl)
            {
                DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                dM_HangHoaDTO.ID = item.ID;
                dM_HangHoaDTO.MaHangHoa = item.MaHangHoa;
                dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                dM_HangHoaDTO.SoLuong = 1;
                dM_HangHoaDTO.TonKho = Math.Round(_classBHHD.TinhSLTonHHKK(item.ID, iddonvi, DateTime.Now).Value / item.TyLeChuyenDoi, 3, MidpointRounding.ToEven);
                dM_HangHoaDTO.GiaBan = item.GiaBan;
                dM_HangHoaDTO.GiaVon = Math.Round((double)item.GiaVon, MidpointRounding.ToEven); // 1* DonGia
                dM_HangHoaDTO.GiaNhap = item.GiaNhap;
                dM_HangHoaDTO.GiamGia = 0;
                dM_HangHoaDTO.TenDonViTinh = item.TenDonViTinh;
                dM_HangHoaDTO.ID_DonViQuiDoi = item.ID_DonViQuiDoi;

                lst.Add(dM_HangHoaDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }


        public List<DM_HangHoaDTO> GetHangHoa_ByIDDonViQuiDoi(Guid id)
        {
            var tbl = from dvt in db.DonViQuiDois
                      where dvt.ID == id
                      //new Guid("6eb8edd4-1ed2-46a3-a38f-353bfb566ce6")
                      select new DM_HangHoaDTO
                      {
                          MaHangHoa = dvt.MaHangHoa,
                          ID_DonViQuiDoi = dvt.ID,
                      };

            return tbl.ToList();
        }

        public List<DM_HangHoaDTO> GetHangHoa_QuyDoiDVT(Guid id, Guid iddonvi)
        {
            classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
            var tbl = from hh in db.DM_HangHoa
                      join nhh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nhh.ID into listHH
                      from list in listHH.DefaultIfEmpty()
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa
                      join gv in db.DM_GiaVon.Where(p => p.ID_DonVi == iddonvi) on dvt.ID equals gv.ID_DonViQuiDoi into listGV
                      from dmgv in listGV.DefaultIfEmpty()
                      join xe in db.Gara_DanhMucXe on hh.ID_Xe equals xe.ID into HHXe
                      from hhxe in HHXe.DefaultIfEmpty()
                      where dvt.ID == id
                      select new
                      {
                          ID = dvt.ID_HangHoa,
                          ID_HangHoaCungLoai = hh.ID_HangHoaCungLoai,
                          LaChaCungLoai = hh.LaChaCungLoai,
                          TenHangHoa = hh.TenHangHoa,
                          NhomHangHoa = list.TenNhomHangHoa,
                          MaHangHoa = dvt.MaHangHoa,
                          SoLuong = 1,
                          GiaBan = dvt.GiaBan,
                          GiaNhap = dvt.GiaNhap,
                          GiaVon = dmgv != null ? dmgv.GiaVon : 0,
                          GiamGia = 0,
                          TenDonViTinh = dvt.TenDonViTinh,
                          ID_DonViQuiDoi = dvt.ID,
                          LaHangHoa = hh.LaHangHoa,
                          ID_LoHang = dmgv != null ? dmgv.ID_LoHang : null,
                          ID_DonVi = dmgv != null ? dmgv.ID_DonVi : Guid.Empty,
                          TrangThai = hh.TheoDoi,
                          DuocBanTrucTiep = hh.DuocBanTrucTiep,
                          TyLeChuyenDoi = dvt.TyLeChuyenDoi,
                          QuanLyTheoLoHang = hh.QuanLyTheoLoHang,
                          ThuocTinhGiaTri = dvt.ThuocTinhGiaTri,
                          QuanLyBaoDuong = hh.QuanLyBaoDuong,
                          LoaiHangHoa = hh.LoaiHangHoa,
                          GhiChu = dvt.GhiChu,
                          HoaHongTruocChietKhau = hh.HoaHongTruocChietKhau != null ? hh.HoaHongTruocChietKhau : 0,
                          ID_Xe = hhxe != null ? hhxe.ID : Guid.Empty,
                          BienSo = hhxe != null ? hhxe.BienSo : string.Empty,
                          ChietKhauMD_NV = hh.ChietKhauMD_NV ?? 0,
                          ChietKhauMD_NVTheoPT = hh.ChietKhauMD_NVTheoPT ?? true,
                      };
            tbl = tbl.Where(p => (p.ID_DonVi == iddonvi || p.ID_DonVi == Guid.Empty));
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            foreach (var item in tbl)
            {
                DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                dM_HangHoaDTO.ID = item.ID;
                dM_HangHoaDTO.ID_DonViQuiDoi = item.ID_DonViQuiDoi; // ID_dvquydoi
                dM_HangHoaDTO.LaChaCungLoai = item.LaChaCungLoai;
                dM_HangHoaDTO.ID_HangHoaCungLoai = item.ID_HangHoaCungLoai;
                dM_HangHoaDTO.MaHangHoa = /*item.LaChaCungLoai == true ? (ClassDM_HangHoa.Gets(p => p.ID_HangHoaCungLoai == item.ID_HangHoaCungLoai).Count() > 1 ? "(" + ClassDM_HangHoa.Gets(p => p.ID_HangHoaCungLoai == item.ID_HangHoaCungLoai).Count().ToString() + ") " + "Mã hàng" : item.MaHangHoa) :*/ item.MaHangHoa;
                dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                dM_HangHoaDTO.SoLuong = 1;
                dM_HangHoaDTO.TonKho = Math.Round(TinhSLTonHH(item.ID, iddonvi).Value / item.TyLeChuyenDoi, 3, MidpointRounding.ToEven);
                dM_HangHoaDTO.GiaBan = item.GiaBan;
                dM_HangHoaDTO.GiaVon = item.GiaVon; // 1* DonGia
                dM_HangHoaDTO.GiaNhap = item.GiaNhap;
                dM_HangHoaDTO.GiamGia = 0;
                dM_HangHoaDTO.TenDonViTinh = item.TenDonViTinh;
                dM_HangHoaDTO.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                dM_HangHoaDTO.NhomHangHoa = item.NhomHangHoa;
                dM_HangHoaDTO.LaHangHoa = item.LaHangHoa;
                dM_HangHoaDTO.sLoaiHangHoa = item.LaHangHoa != null ? item.LaHangHoa.Value == true ? "Hàng hóa" : "Dịch vụ" : "Combo - đóng gói";
                dM_HangHoaDTO.TrangThai = item.TrangThai;
                dM_HangHoaDTO.DonViTinhChuan = item.TenDonViTinh;
                dM_HangHoaDTO.DuocBanTrucTiep = item.DuocBanTrucTiep;
                dM_HangHoaDTO.QuanLyTheoLoHang = item.QuanLyTheoLoHang;
                dM_HangHoaDTO.QuanLyBaoDuong = item.QuanLyBaoDuong;
                dM_HangHoaDTO.HoaHongTruocChietKhau = item.HoaHongTruocChietKhau;
                dM_HangHoaDTO.LoaiHangHoa = item.LoaiHangHoa;
                dM_HangHoaDTO.GhiChu = item.GhiChu;
                dM_HangHoaDTO.ID_Xe = item.ID_Xe;
                dM_HangHoaDTO.BienSo = item.BienSo;
                dM_HangHoaDTO.ChietKhauMD_NV = item.ChietKhauMD_NV;
                dM_HangHoaDTO.ChietKhauMD_NVTheoPT = item.ChietKhauMD_NVTheoPT;
                dM_HangHoaDTO.DonViTinh = _classDVQD.Gets(p => p.ID_HangHoa == item.ID && p.Xoa != true).Select(p => new DonViTinh
                {
                    ID = p.ID,
                    ID_HangHoa = p.ID_HangHoa,
                    ID_DonViQuiDoi = p.ID,
                    MaHangHoa = p.MaHangHoa,
                    TenDonViTinh = p.TenDonViTinh,
                    TyLeChuyenDoi = p.TyLeChuyenDoi,
                    GiaBan = p.GiaBan,
                    GiaVon = item.GiaVon * p.TyLeChuyenDoi,
                    LaDonViChuan = p.LaDonViChuan,
                    DonViTinhChuan = dM_HangHoaDTO.DonViTinhChuan,
                    QuanLyTheoLoHang = item.QuanLyTheoLoHang
                }).ToList();
                dM_HangHoaDTO.ThuocTinhGiaTri = item.ThuocTinhGiaTri;
                dM_HangHoaDTO.ListChildren = new List<DM_HangHoaDTO>();
                lst.Add(dM_HangHoaDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }

        public List<DM_HangHoaDTO> GetHangHoa_ByIDQuyDoiDVTByLo(Guid id, Guid iddonvi)
        {
            classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
            var tbl = from hh in db.DM_HangHoa
                      join nhh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nhh.ID into listHH
                      from list in listHH.DefaultIfEmpty()
                      join dvt in db.DonViQuiDois on hh.ID equals dvt.ID_HangHoa
                      join xe in db.Gara_DanhMucXe on hh.ID_Xe equals xe.ID into HHXe
                      from hhxe in HHXe.DefaultIfEmpty()
                      where dvt.ID == id
                      select new
                      {
                          ID = dvt.ID_HangHoa,
                          ID_HangHoaCungLoai = hh.ID_HangHoaCungLoai,
                          LaChaCungLoai = hh.LaChaCungLoai,
                          TenHangHoa = hh.TenHangHoa,
                          NhomHangHoa = list.TenNhomHangHoa,
                          MaHangHoa = dvt.MaHangHoa,
                          SoLuong = 1,
                          GiaBan = dvt.GiaBan,
                          GiaNhap = dvt.GiaNhap,
                          GiamGia = 0,
                          TenDonViTinh = dvt.TenDonViTinh,
                          ID_DonViQuiDoi = dvt.ID,
                          LaHangHoa = hh.LaHangHoa,
                          TrangThai = hh.TheoDoi,
                          DuocBanTrucTiep = hh.DuocBanTrucTiep,
                          TyLeChuyenDoi = dvt.TyLeChuyenDoi,
                          QuanLyTheoLoHang = hh.QuanLyTheoLoHang,
                          ThuocTinhGiaTri = dvt.ThuocTinhGiaTri,
                          HoaHongTruocChietKhau = hh.HoaHongTruocChietKhau != null ? hh.HoaHongTruocChietKhau : 0,
                          GhiChu = dvt.GhiChu,
                          ID_Xe = hhxe != null ? hhxe.ID : Guid.Empty,
                          BienSo = hhxe != null ? hhxe.BienSo : "",
                          ChietKhauMD_NV = hh.ChietKhauMD_NV ?? 0,
                          ChietKhauMD_NVTheoPT = hh.ChietKhauMD_NVTheoPT ?? true,
                      };

            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            foreach (var item in tbl)
            {
                DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                dM_HangHoaDTO.ID = item.ID;
                dM_HangHoaDTO.ID_DonViQuiDoi = item.ID_DonViQuiDoi; // ID_dvquydoi
                dM_HangHoaDTO.LaChaCungLoai = item.LaChaCungLoai;
                dM_HangHoaDTO.ID_HangHoaCungLoai = item.ID_HangHoaCungLoai;
                dM_HangHoaDTO.MaHangHoa = /*item.LaChaCungLoai == true ? (ClassDM_HangHoa.Gets(p => p.ID_HangHoaCungLoai == item.ID_HangHoaCungLoai).Count() > 1 ? "(" + ClassDM_HangHoa.Gets(p => p.ID_HangHoaCungLoai == item.ID_HangHoaCungLoai).Count().ToString() + ") " + "Mã hàng" : item.MaHangHoa) :*/ item.MaHangHoa;
                dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                dM_HangHoaDTO.SoLuong = 1;
                dM_HangHoaDTO.TonKho = Math.Round(TinhSLTonHH(item.ID, iddonvi).Value / item.TyLeChuyenDoi, 3, MidpointRounding.ToEven);
                dM_HangHoaDTO.GiaBan = item.GiaBan;
                dM_HangHoaDTO.GiaNhap = item.GiaNhap;
                dM_HangHoaDTO.GiamGia = 0;
                dM_HangHoaDTO.TenDonViTinh = item.TenDonViTinh;
                dM_HangHoaDTO.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                dM_HangHoaDTO.NhomHangHoa = item.NhomHangHoa;
                dM_HangHoaDTO.LaHangHoa = item.LaHangHoa;
                dM_HangHoaDTO.sLoaiHangHoa = item.LaHangHoa != null ? item.LaHangHoa.Value == true ? "Hàng hóa" : "Dịch vụ" : "Combo - đóng gói";
                dM_HangHoaDTO.TrangThai = item.TrangThai;
                dM_HangHoaDTO.DonViTinhChuan = item.TenDonViTinh;
                dM_HangHoaDTO.DuocBanTrucTiep = item.DuocBanTrucTiep;
                dM_HangHoaDTO.QuanLyTheoLoHang = item.QuanLyTheoLoHang;
                dM_HangHoaDTO.HoaHongTruocChietKhau = item.HoaHongTruocChietKhau;
                dM_HangHoaDTO.GhiChu = item.GhiChu;
                dM_HangHoaDTO.ID_Xe = item.ID_Xe;
                dM_HangHoaDTO.BienSo = item.BienSo;
                dM_HangHoaDTO.ChietKhauMD_NV = item.ChietKhauMD_NV;
                dM_HangHoaDTO.ChietKhauMD_NVTheoPT = item.ChietKhauMD_NVTheoPT;
                dM_HangHoaDTO.DonViTinh = _classDVQD.Gets(p => p.ID_HangHoa == item.ID && p.Xoa != true).Select(p => new DonViTinh
                {
                    ID = p.ID,
                    ID_HangHoa = p.ID_HangHoa,
                    ID_DonViQuiDoi = p.ID,
                    MaHangHoa = p.MaHangHoa,
                    TenDonViTinh = p.TenDonViTinh,
                    TyLeChuyenDoi = p.TyLeChuyenDoi,
                    GiaBan = p.GiaBan,
                    LaDonViChuan = p.LaDonViChuan,
                    DonViTinhChuan = dM_HangHoaDTO.DonViTinhChuan,
                    QuanLyTheoLoHang = item.QuanLyTheoLoHang
                }).ToList();
                dM_HangHoaDTO.ThuocTinhGiaTri = item.ThuocTinhGiaTri;
                dM_HangHoaDTO.ListChildren = new List<DM_HangHoaDTO>();
                lst.Add(dM_HangHoaDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }

        public List<DM_HangHoaDTO> GetListHangHoas_Where(string maHoaDon, int tonkho, int kinhdoanh,
            int trangThai, string idnhomhang, Guid iddonvi, string listthuoctinh)
        {
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();

            if (db != null)
            {
                string lstIDNHH = "";
                if (idnhomhang != null)
                {
                    var arrIDNHH = idnhomhang.Split(',');
                    for (int i = 0; i < arrIDNHH.Length; i++)
                    {
                        lstIDNHH = lstIDNHH + arrIDNHH[i] + ",";
                    }
                }
                if (idnhomhang != null)
                {
                    lstIDNHH = lstIDNHH.Remove(lstIDNHH.Length - 1);
                }
                else
                {
                    lstIDNHH = "%%";
                }
                string filterKD = "";
                if (kinhdoanh == 1)
                {
                    filterKD = "1";
                }
                else
                {
                    if (kinhdoanh == 2)
                    {
                        filterKD = "0";
                    }
                    else
                    {
                        filterKD = "%%";
                    }
                }
                string filterLaHH = "";
                if (trangThai == 1)
                {
                    filterLaHH = "1";
                }
                else
                {
                    if (trangThai == 2)
                    {
                        filterLaHH = "0";
                    }
                    else
                    {
                        filterLaHH = "%%";
                    }
                }
                if (listthuoctinh == null)
                {
                    listthuoctinh = "";
                }
                List<SqlParameter> paramlist = new List<SqlParameter>();
                Guid ID_ChiNhanh = iddonvi;
                if (maHoaDon == null)
                {
                    maHoaDon = string.Empty;
                }
                char[] whitespace = new char[] { ' ', '\t' };
                string[] textFilter = maHoaDon.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                paramlist.Add(new SqlParameter("MaHH", string.Join(",", utf)));
                paramlist.Add(new SqlParameter("MaHHCoDau", string.Join(",", utf8)));
                paramlist.Add(new SqlParameter("ListID_NhomHang", lstIDNHH.Replace("%", "")));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("KinhDoanhFilter", filterKD));
                paramlist.Add(new SqlParameter("LaHangHoaFilter", filterLaHH));
                paramlist.Add(new SqlParameter("List_ThuocTinh", listthuoctinh));
                List<DM_HangHoaDTO> listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec XuatFileDanhMucHH @MaHH,@MaHHCoDau,@ListID_NhomHang, @ID_ChiNhanh, @KinhDoanhFilter,@LaHangHoaFilter, @List_ThuocTinh", paramlist.ToArray()).ToList();

                switch (tonkho)
                {
                    case 1: // HT
                        listTon = listTon.Where(hd => hd.TonKho > 0).ToList();
                        break;
                    case 2: // Huy
                        listTon = listTon.Where(hd => hd.TonKho <= 0).ToList();
                        break;
                    default: // HT + Huy
                        break;
                }
                return listTon.OrderByDescending(p => p.NgayTao).GroupBy(x => x.ID_DonViQuiDoi).Select(t => new DM_HangHoaDTO
                {
                    ID = t.FirstOrDefault().ID,
                    ID_DonViQuiDoi = t.FirstOrDefault().ID_DonViQuiDoi,
                    ID_HangHoaCungLoai = t.FirstOrDefault().ID_HangHoaCungLoai,
                    ID_NhomHangHoa = t.FirstOrDefault().ID_NhomHangHoa,
                    TenHangHoa = t.FirstOrDefault().TenHangHoa,
                    LaChaCungLoai = t.FirstOrDefault().LaChaCungLoai,
                    MaHangHoa = t.FirstOrDefault().MaHangHoa,
                    NhomHangHoa = t.FirstOrDefault().NhomHangHoa,
                    PhanLoaiHangHoa = t.FirstOrDefault().PhanLoaiHangHoa,
                    GiaBan = t.FirstOrDefault().GiaBan,
                    GiaVon = t.FirstOrDefault().GiaVon,
                    TonKho = t.FirstOrDefault().TonKho,
                    TrangThai = t.FirstOrDefault().TrangThai,
                    LoaiHangHoa = t.FirstOrDefault().LoaiHangHoa,
                    DuocBanTrucTiep = t.FirstOrDefault().DuocBanTrucTiep,
                    NgayTao = t.FirstOrDefault().NgayTao,
                    TenDonViTinh = t.FirstOrDefault().TenDonViTinh,
                    LaHangHoa = t.FirstOrDefault().LaHangHoa,
                }).ToList();
                //return listTon.OrderByDescending(p=>p.NgayTao).ToList();
            }
            else
            {
                return null;
            }
        }

        public List<BCDM_LoHangDTO> GetlistDM_LoHang(int currentPage, int pageSize, string maHoaDon, int tonkho, string idnhomhang, Guid iddonvi, string listthuoctinh, DateTime? dayStart, DateTime? dayEnd, ref int total, ref int pagecount, int check)
        {
            if (db != null)
            {
                //exec
                string lstIDNHH = "";
                if (idnhomhang != null)
                {
                    var arrIDNHH = idnhomhang.Split(',');
                    for (int i = 0; i < arrIDNHH.Length; i++)
                    {
                        lstIDNHH = lstIDNHH + arrIDNHH[i] + ",";
                    }
                }
                if (idnhomhang != null)
                {
                    lstIDNHH = lstIDNHH.Remove(lstIDNHH.Length - 1);
                }
                else
                {
                    lstIDNHH = "%%";
                }
                if (listthuoctinh == null)
                {
                    listthuoctinh = "";
                }

                List<SqlParameter> paramlist = new List<SqlParameter>();
                Guid ID_ChiNhanh = iddonvi;
                if (maHoaDon == null)
                {
                    maHoaDon = string.Empty;
                }
                char[] whitespace = new char[] { ' ', '\t' };
                string[] textFilter = maHoaDon.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                paramlist.Add(new SqlParameter("MaHH", string.Join(",", utf)));
                paramlist.Add(new SqlParameter("MaHHCoDau", string.Join(",", utf8)));
                paramlist.Add(new SqlParameter("ListID_NhomHang", lstIDNHH.Replace("%", "")));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("List_ThuocTinh", listthuoctinh));
                List<BCDM_LoHangDTO> listTon = db.Database.SqlQuery<BCDM_LoHangDTO>("exec LoadDanhMucLoHangBaoCao @MaHH,@MaHHCoDau,@ListID_NhomHang, @ID_ChiNhanh, @List_ThuocTinh", paramlist.ToArray()).ToList();

                // NgayLapHoaDon
                if (dayStart != null && dayEnd != null)
                {
                    if (dayStart == dayEnd && dayStart != null)
                    {
                        listTon = listTon.Where(hd => (hd.NgayHetHan != null && hd.NgayHetHan < dayStart)).ToList();
                    }
                    else
                    {
                        listTon = listTon.Where(hd => hd.NgayHetHan != null && hd.NgayHetHan >= dayStart && hd.NgayHetHan <= dayEnd).ToList();
                    }
                }

                switch (tonkho)
                {
                    case 1: // tồn > 0
                        listTon = listTon.Where(hd => hd.TonKho > 0).ToList();
                        break;
                    case 2: // tồn < 0
                        listTon = listTon.Where(hd => hd.TonKho <= 0).ToList();
                        break;
                    case 4: // Dưới định mức tồn
                        listTon = listTon.Where(hd => hd.TonKho < hd.TonToiThieu).ToList();
                        break;
                    case 5: // Vượt định mức tồn
                        listTon = listTon.Where(hd => hd.TonKho > hd.TonToiDa).ToList();
                        break;
                    default: // HT + Huy
                        break;
                }

                listTon = listTon.OrderByDescending(p => p.NgayTao).ToList();
                listTon = listTon.GroupBy(x => x.ID_LoHang).Select(t => new BCDM_LoHangDTO
                {
                    ID = t.FirstOrDefault().ID,
                    ID_DonViQuiDoi = t.FirstOrDefault().ID_DonViQuiDoi,
                    ID_NhomHangHoa = t.FirstOrDefault().ID_NhomHangHoa,
                    ID_LoHang = t.FirstOrDefault().ID_LoHang,
                    TenHangHoa = t.FirstOrDefault().TenHangHoa,
                    MaHangHoa = t.FirstOrDefault().MaHangHoa,
                    MaLoHang = t.FirstOrDefault().MaLoHang,
                    NhomHangHoa = t.FirstOrDefault().NhomHangHoa,
                    GiaBan = t.FirstOrDefault().GiaBan,
                    GiaVon = Math.Round(t.FirstOrDefault().GiaVon, MidpointRounding.ToEven),
                    TonKho = Math.Round(t.FirstOrDefault().TonKho.Value, 2, MidpointRounding.ToEven),
                    NgaySanXuat = t.FirstOrDefault().NgaySanXuat,
                    NgayHetHan = t.FirstOrDefault().NgayHetHan,
                    TrangThai = (t.FirstOrDefault().NgayHetHan != null) ? (DateTime.Now < t.FirstOrDefault().NgayHetHan ? "Còn hạn" : "Hết hạn") : "",
                    SoNgayConHan = (t.FirstOrDefault().NgayHetHan != null) ? Math.Round(t.FirstOrDefault().NgayHetHan.Value.Subtract(DateTime.Now).TotalDays, MidpointRounding.ToEven) + 1 : (double?)null,
                    TonToiThieu = t.FirstOrDefault().TonToiThieu,
                    TonToiDa = t.FirstOrDefault().TonToiDa,
                    NgayTao = t.FirstOrDefault().NgayTao,
                    NgayTaoLo = t.FirstOrDefault().NgayTaoLo,
                    TenDonViTinh = t.FirstOrDefault().TenDonViTinh,
                }).ToList();

                if (check != 0)
                {
                    listTon = listTon.Where(p => p.SoNgayConHan == check).ToList();
                }


                total = listTon.Count;
                pagecount = (int)Math.Ceiling((double)listTon.Count / pageSize);
                return listTon.OrderByDescending(p => p.NgayTaoLo).Skip(currentPage * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return new List<BCDM_LoHangDTO>();
            }
        }

        public List<BCDM_LoHangDTO> XuatFileDMLoHang(string maHoaDon, int tonkho, string idnhomhang, Guid iddonvi, string listthuoctinh, DateTime? dayStart, DateTime? dayEnd)
        {
            if (db != null)
            {
                //exec
                string lstIDNHH = "";
                if (idnhomhang != null)
                {
                    var arrIDNHH = idnhomhang.Split(',');
                    for (int i = 0; i < arrIDNHH.Length; i++)
                    {
                        lstIDNHH = lstIDNHH + arrIDNHH[i] + ",";
                    }
                }
                if (idnhomhang != null)
                {
                    lstIDNHH = lstIDNHH.Remove(lstIDNHH.Length - 1);
                }
                else
                {
                    lstIDNHH = "%%";
                }
                if (listthuoctinh == null)
                {
                    listthuoctinh = "";
                }

                List<SqlParameter> paramlist = new List<SqlParameter>();
                Guid ID_ChiNhanh = iddonvi;
                if (maHoaDon == null)
                {
                    maHoaDon = string.Empty;
                }
                char[] whitespace = new char[] { ' ', '\t' };
                string[] textFilter = maHoaDon.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                paramlist.Add(new SqlParameter("MaHH", string.Join(",", utf)));
                paramlist.Add(new SqlParameter("MaHHCoDau", string.Join(",", utf8)));
                paramlist.Add(new SqlParameter("ListID_NhomHang", lstIDNHH.Replace("%", "")));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("List_ThuocTinh", listthuoctinh));
                List<BCDM_LoHangDTO> listTon = db.Database.SqlQuery<BCDM_LoHangDTO>("exec LoadDanhMucLoHangBaoCao @MaHH,@MaHHCoDau,@ListID_NhomHang, @ID_ChiNhanh, @List_ThuocTinh", paramlist.ToArray()).ToList();

                // NgayLapHoaDon
                if (dayStart != null && dayEnd != null)
                {
                    if (dayStart == dayEnd && dayStart != null)
                    {
                        listTon = listTon.Where(hd => (hd.NgayHetHan != null && hd.NgayHetHan < dayStart)).ToList();
                    }
                    else
                    {
                        listTon = listTon.Where(hd => hd.NgayHetHan != null && hd.NgayHetHan >= dayStart && hd.NgayHetHan < dayEnd).ToList();
                    }
                }

                switch (tonkho)
                {
                    case 1: // tồn > 0
                        listTon = listTon.Where(hd => hd.TonKho > 0).ToList();
                        break;
                    case 2: // tồn < 0
                        listTon = listTon.Where(hd => hd.TonKho <= 0).ToList();
                        break;
                    case 4: // Dưới định mức tồn
                        listTon = listTon.Where(hd => hd.TonKho < hd.TonToiThieu).ToList();
                        break;
                    case 5: // Vượt định mức tồn
                        listTon = listTon.Where(hd => hd.TonKho > hd.TonToiDa).ToList();
                        break;
                    default: // HT + Huy
                        break;
                }
                listTon = listTon.OrderByDescending(p => p.NgayTao).ToList();
                listTon = listTon.GroupBy(x => x.ID_LoHang).Select(t => new BCDM_LoHangDTO
                {
                    ID = t.FirstOrDefault().ID,
                    ID_DonViQuiDoi = t.FirstOrDefault().ID_DonViQuiDoi,
                    ID_NhomHangHoa = t.FirstOrDefault().ID_NhomHangHoa,
                    ID_LoHang = t.FirstOrDefault().ID_LoHang,
                    TenHangHoa = t.FirstOrDefault().TenHangHoa,
                    MaHangHoa = t.FirstOrDefault().MaHangHoa,
                    MaLoHang = t.FirstOrDefault().MaLoHang,
                    NhomHangHoa = t.FirstOrDefault().NhomHangHoa,
                    GiaBan = t.FirstOrDefault().GiaBan,
                    GiaVon = Math.Round(t.FirstOrDefault().GiaVon, MidpointRounding.ToEven),
                    TonKho = Math.Round(t.FirstOrDefault().TonKho.Value, 2, MidpointRounding.ToEven),
                    NgaySanXuat = t.FirstOrDefault().NgaySanXuat,
                    NgayHetHan = t.FirstOrDefault().NgayHetHan,
                    TrangThai = (t.FirstOrDefault().NgayHetHan != null) ? (DateTime.Now < t.FirstOrDefault().NgayHetHan ? "Còn hạn" : "Hết hạn") : "",
                    SoNgayConHan = (t.FirstOrDefault().NgayHetHan != null) ? Math.Round(t.FirstOrDefault().NgayHetHan.Value.Subtract(DateTime.Now).TotalDays, MidpointRounding.ToEven) + 1 : (double?)null,
                    TonToiThieu = t.FirstOrDefault().TonToiThieu,
                    TonToiDa = t.FirstOrDefault().TonToiDa,
                    NgayTao = t.FirstOrDefault().NgayTao,
                    NgayTaoLo = t.FirstOrDefault().NgayTaoLo,
                    TenDonViTinh = t.FirstOrDefault().TenDonViTinh,
                }).ToList();

                return listTon.OrderByDescending(p => p.NgayTaoLo).ToList();
            }
            else
            {
                return new List<BCDM_LoHangDTO>();
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas_WhereNew(int currentPage, int pageSize, string maHoaDon, int tonkho, int kinhdoanh,
         string loaihangs, string[] idnhomhang, Guid iddonvi, string listthuoctinh, string columsort, string sort, List<ColumSearch> listColumsearch, ref double tongton, ref int total, ref int pagecount)
        {
            if (db != null)
            {

                //exec
                string lstIDNHH = "";
                if (idnhomhang.Count() > 0)
                {
                    lstIDNHH = string.Join(",", idnhomhang);

                }
                else
                {
                    lstIDNHH = "%%";
                }

                string filterKD = "";
                if (kinhdoanh == 1)
                {
                    filterKD = "1";
                }
                else
                {
                    if (kinhdoanh == 2)
                    {
                        filterKD = "0";
                    }
                    else
                    {
                        filterKD = "%%";
                    }
                }

                if (listthuoctinh == null)
                {
                    listthuoctinh = "";
                }
                List<SqlParameter> paramlist = new List<SqlParameter>();
                Guid ID_ChiNhanh = iddonvi;
                if (maHoaDon == null || maHoaDon == "")
                {
                    maHoaDon = string.Empty;
                }
                char[] whitespace = new char[] { ' ', '\t' };
                string[] textFilter = maHoaDon.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                paramlist.Add(new SqlParameter("MaHH", string.Join(",", utf)));
                paramlist.Add(new SqlParameter("MaHHCoDau", string.Join(",", utf8)));
                paramlist.Add(new SqlParameter("ListID_NhomHang", lstIDNHH.Replace("%", "")));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("KinhDoanhFilter", filterKD));
                paramlist.Add(new SqlParameter("LoaiHangHoas", loaihangs));
                paramlist.Add(new SqlParameter("List_ThuocTinh", listthuoctinh));
                List<DM_HangHoaDTO> listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec LoadDanhMucHangHoa @MaHH,@MaHHCoDau,@ListID_NhomHang, @ID_ChiNhanh, @KinhDoanhFilter,@LoaiHangHoas, @List_ThuocTinh", paramlist.ToArray()).ToList();

                switch (tonkho)
                {
                    case 1: // tồn > 0
                        listTon = listTon.Where(hd => hd.TonKho > 0).ToList();
                        break;
                    case 2: // tồn <= 0
                        listTon = listTon.Where(hd => hd.TonKho <= 0).ToList();
                        break;
                    case 3: // Dưới định mức tồn
                        listTon = listTon.Where(hd => hd.TonKho < hd.TonToiThieu).ToList();
                        break;
                    case 4: // Vượt định mức tồn
                        listTon = listTon.Where(hd => hd.TonKho > hd.TonToiDa).ToList();
                        break;
                    case 5: // Hàng âm kho
                        listTon = listTon.Where(hd => hd.TonKho < 0).ToList();
                        break;
                    case 6: // TonKho = 0
                        listTon = listTon.Where(hd => hd.TonKho == 0).ToList();
                        break;
                    default: // all 
                        break;
                }


                listTon = searchColumn(listColumsearch, listTon);
                listTon = listTon.GroupBy(x => x.ID_DonViQuiDoi).Select(t => new DM_HangHoaDTO
                {
                    ID = t.FirstOrDefault().ID,
                    ID_DonViQuiDoi = t.FirstOrDefault().ID_DonViQuiDoi,
                    ID_HangHoaCungLoai = t.FirstOrDefault().ID_HangHoaCungLoai,
                    ID_NhomHangHoa = t.FirstOrDefault().ID_NhomHangHoa,
                    TenHangHoa = t.FirstOrDefault().TenHangHoa,
                    LaChaCungLoai = t.FirstOrDefault().LaChaCungLoai,
                    MaHangHoa = t.FirstOrDefault().MaHangHoa,
                    NhomHangHoa = t.FirstOrDefault().NhomHangHoa,
                    PhanLoaiHangHoa = t.FirstOrDefault().PhanLoaiHangHoa,
                    GiaBan = t.FirstOrDefault().GiaBan,
                    GiaVon = t.FirstOrDefault().GiaVon,
                    TonKho = t.Sum(o => o.TonKho ?? 0),
                    TonToiThieu = t.FirstOrDefault().TonToiThieu,
                    TonToiDa = t.FirstOrDefault().TonToiDa,
                    TrangThai = t.FirstOrDefault().TrangThai,
                    sLoaiHangHoa = t.FirstOrDefault().sLoaiHangHoa,
                    DuocBanTrucTiep = t.FirstOrDefault().DuocBanTrucTiep,
                    NgayTao = t.FirstOrDefault().NgayTao,
                    TenDonViTinh = t.FirstOrDefault().TenDonViTinh,
                    LaHangHoa = t.FirstOrDefault().LaHangHoa,
                    GhiChu = t.FirstOrDefault().GhiChu,
                    QuanLyTheoLoHang = t.FirstOrDefault().QuanLyTheoLoHang,
                    ThuocTinhGiaTri = t.FirstOrDefault().ThuocTinhGiaTri,
                    SoPhutThucHien = t.FirstOrDefault().SoPhutThucHien,
                    DuocTichDiem = t.FirstOrDefault().DuocTichDiem,
                    DichVuTheoGio = t.FirstOrDefault().DichVuTheoGio,
                    QuanLyBaoDuong = t.FirstOrDefault().QuanLyBaoDuong,
                    LoaiBaoDuong = t.FirstOrDefault().LoaiBaoDuong,
                    LoaiBaoHanh = t.FirstOrDefault().LoaiBaoHanh,
                    LoaiHangHoa = t.FirstOrDefault().LoaiHangHoa,
                    SoKmBaoHanh = t.FirstOrDefault().SoKmBaoHanh,
                    HoaHongTruocChietKhau = t.FirstOrDefault().HoaHongTruocChietKhau,
                    Xoa = t.FirstOrDefault().Xoa,
                    ID_Xe = t.FirstOrDefault().ID_Xe,
                    BienSo = t.FirstOrDefault().BienSo,
                    ChietKhauMD_NV = t.FirstOrDefault().ChietKhauMD_NV,
                    ChietKhauMD_NVTheoPT = t.FirstOrDefault().ChietKhauMD_NVTheoPT,
                }).ToList();
                listTon = listTon.GroupBy(o => o.ID_HangHoaCungLoai).Select(t => new DM_HangHoaDTO
                {
                    ID = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.ID).FirstOrDefault(),
                    ID_DonViQuiDoi = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.ID_DonViQuiDoi).FirstOrDefault(),
                    ID_HangHoaCungLoai = t.Key,
                    ID_NhomHangHoa = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.ID_NhomHangHoa).FirstOrDefault(),
                    TenHangHoa = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.TenHangHoa).FirstOrDefault(),
                    LaChaCungLoai = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.LaChaCungLoai).FirstOrDefault(),
                    MaHangHoa = t.Count() == 1 ? t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.MaHangHoa).FirstOrDefault() : "(" + t.Count().ToString() + ") Mã hàng",
                    NhomHangHoa = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.NhomHangHoa).FirstOrDefault(),
                    PhanLoaiHangHoa = t.Where(o => o.LaHangHoa != null).Where(o => o.LaHangHoa.Value).Select(c => c.PhanLoaiHangHoa).FirstOrDefault(),
                    GiaBan = t.Sum(o => o.GiaBan ?? 0) / t.Count(),
                    GiaVon = t.Sum(o => o.GiaVon ?? 0) / t.Count(),
                    TonKho = t.Sum(o => o.TonKho ?? 0),
                    TonToiThieu = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.TonToiThieu).FirstOrDefault(),
                    TonToiDa = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.TonToiDa).FirstOrDefault(),
                    TrangThai = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.TrangThai).FirstOrDefault(),
                    sLoaiHangHoa = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.sLoaiHangHoa).FirstOrDefault(),
                    DuocBanTrucTiep = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.DuocBanTrucTiep).FirstOrDefault(),
                    NgayTao = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.NgayTao).FirstOrDefault(),
                    TenDonViTinh = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.TenDonViTinh).FirstOrDefault(),
                    LaHangHoa = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.LaHangHoa).FirstOrDefault(),
                    GhiChu = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.GhiChu).FirstOrDefault(),
                    QuanLyTheoLoHang = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.QuanLyTheoLoHang).FirstOrDefault(),
                    ThuocTinhGiaTri = t.OrderByDescending(c => c.LaChaCungLoai).Select(c => c.ThuocTinhGiaTri).FirstOrDefault(),
                    SoPhutThucHien = t.FirstOrDefault().SoPhutThucHien,
                    DuocTichDiem = t.FirstOrDefault().DuocTichDiem,
                    DichVuTheoGio = t.FirstOrDefault().DichVuTheoGio,
                    QuanLyBaoDuong = t.FirstOrDefault().QuanLyBaoDuong,
                    LoaiBaoDuong = t.FirstOrDefault().LoaiBaoDuong,
                    LoaiBaoHanh = t.FirstOrDefault().LoaiBaoHanh,
                    LoaiHangHoa = t.FirstOrDefault().LoaiHangHoa,
                    SoKmBaoHanh = t.FirstOrDefault().SoKmBaoHanh,
                    HoaHongTruocChietKhau = t.FirstOrDefault().HoaHongTruocChietKhau,
                    ListChildren = t.OrderByDescending(c => c.LaChaCungLoai).ToList(),
                    Xoa = t.FirstOrDefault().Xoa,
                    ID_Xe = t.FirstOrDefault().ID_Xe,
                    BienSo = t.FirstOrDefault().BienSo,
                    ChietKhauMD_NV = t.FirstOrDefault().ChietKhauMD_NV,
                    ChietKhauMD_NVTheoPT = t.FirstOrDefault().ChietKhauMD_NVTheoPT,
                }).ToList();
                listTon = listTon.OrderByDescending(p => p.NgayTao).ToList();
                if (sort == "0")
                {
                    switch (columsort)
                    {
                        case "MaHang":
                            listTon = listTon.OrderBy(p => p.MaHangHoa).ToList();
                            break;
                        case "TenHang":
                            listTon = listTon.OrderBy(p => p.TenHangHoa).ToList();
                            break;
                        case "TenNhomHang":
                            listTon = listTon.OrderBy(p => p.NhomHangHoa).ToList();
                            break;
                        case "GiaBan":
                            listTon = listTon.OrderBy(p => p.GiaBan).ToList();
                            break;
                        case "GiaVon":
                            listTon = listTon.OrderBy(p => p.GiaVon).ToList();
                            break;
                        case "TonKho":
                            listTon = listTon.OrderBy(p => p.TonKho).ToList();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (columsort)
                    {
                        case "MaHang":
                            listTon = listTon.OrderByDescending(p => p.MaHangHoa).ToList();
                            break;
                        case "TenHang":
                            listTon = listTon.OrderByDescending(p => p.TenHangHoa).ToList();
                            break;
                        case "TenNhomHang":
                            listTon = listTon.OrderByDescending(p => p.NhomHangHoa).ToList();
                            break;
                        case "GiaBan":
                            listTon = listTon.OrderByDescending(p => p.GiaBan).ToList();
                            break;
                        case "GiaVon":
                            listTon = listTon.OrderByDescending(p => p.GiaVon).ToList();
                            break;
                        case "TonKho":
                            listTon = listTon.OrderByDescending(p => p.TonKho).ToList();
                            break;
                        default:
                            break;
                    }
                }
                total = listTon.Count;
                pagecount = (int)Math.Ceiling((double)listTon.Count / pageSize);
                tongton = listTon.GroupBy(x => x.ID_DonViQuiDoi).Select(t => new DM_HangHoaDTO
                {
                    TonKho = t.FirstOrDefault().TonKho,
                }).AsEnumerable().Sum(p => p.TonKho ?? 0);
                return listTon.Skip(currentPage * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return new List<DM_HangHoaDTO>();
            }
        }

        public List<DM_HangHoaDTO> searchColumn(List<ColumSearch> listColumsearch, List<DM_HangHoaDTO> model)
        {
            var listTon = model;
            char[] whitespace = new char[] { ' ', '\t' };
            foreach (var item in listColumsearch)
            {
                string[] textFilter;
                string[] utf8;
                string[] utf;
                switch (item.Key)
                {
                    case (int)GridHellper.ColumnHangHoa.giaban:
                        var gia = double.Parse(item.Value.ToString().Replace(",", ""));
                        switch (item.type)
                        {
                            case (int)commonEnumHellper.KeyCompare.bang:
                                listTon = listTon.Where(o => o.GiaBan == gia).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                listTon = listTon.Where(o => o.GiaBan > gia).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                listTon = listTon.Where(o => o.GiaBan >= gia).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                listTon = listTon.Where(o => o.GiaBan < gia).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                listTon = listTon.Where(o => o.GiaBan <= gia).ToList();
                                break;
                            default:
                                break;
                        }

                        break;
                    case (int)GridHellper.ColumnHangHoa.giavon:
                        var giavon = double.Parse(item.Value.ToString().Replace(",", ""));
                        switch (item.type)
                        {
                            case (int)commonEnumHellper.KeyCompare.bang:
                                listTon = listTon.Where(o => o.GiaVon == giavon).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                listTon = listTon.Where(o => o.GiaVon > giavon).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                listTon = listTon.Where(o => o.GiaVon >= giavon).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                listTon = listTon.Where(o => o.GiaVon < giavon).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                listTon = listTon.Where(o => o.GiaVon <= giavon).ToList();
                                break;
                            default:
                                break;
                        }
                        break;
                    case (int)GridHellper.ColumnHangHoa.loaihang:
                        int loaihang = int.Parse(item.Value.ToString());
                        switch (loaihang)
                        {
                            case 1:
                                listTon = listTon.Where(o => o.LoaiHangHoa == loaihang || o.LaHangHoa == true).ToList();
                                break;
                            case 2:
                                listTon = listTon.Where(o => o.LoaiHangHoa == loaihang || (o.LoaiHangHoa == null && o.LaHangHoa == false)).ToList();
                                break;
                            case 3:
                                listTon = listTon.Where(o => o.LoaiHangHoa == loaihang).ToList();
                                break;
                        }
                        break;
                    case (int)GridHellper.ColumnHangHoa.mahanghoa:
                        listTon = listTon.Where(o => o.MaHangHoa != null && o.MaHangHoa.ToUpper().Contains(item.Value.ToString().Normalize(System.Text.NormalizationForm.FormC).ToUpper().Trim())).ToList();
                        break;
                    case (int)GridHellper.ColumnHangHoa.nhomhang:
                        textFilter = item.Value.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Trim().Split(whitespace);
                        utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                        utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                        listTon = listTon.Where(o => o.NhomHangHoa != null
                                                    && utf8.All(c => o.NhomHangHoa.ToLower().Contains(c))
                                                    && (utf.All(c => CommonStatic.RemoveSign4VietnameseString(o.NhomHangHoa.ToLower()).Contains(c))
                                                        || utf.All(c => CommonStatic.convertchartstart(o.NhomHangHoa.ToLower()).Contains(c)))).ToList();
                        break;
                    case (int)GridHellper.ColumnHangHoa.tenhanghoa:
                        textFilter = item.Value.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Trim().Split(whitespace);
                        utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                        utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                        listTon = listTon.Where(o => o.TenHangHoa != null
                                            && utf8.All(c => o.TenHangHoa.ToLower().Contains(c))
                                            && (utf.All(c => CommonStatic.RemoveSign4VietnameseString(o.TenHangHoa.ToLower()).Contains(c))
                                             || utf.All(c => CommonStatic.convertchartstart(o.TenHangHoa.ToLower()).Contains(c)))).ToList();
                        break;
                    case (int)GridHellper.ColumnHangHoa.tonkho:
                        var tonkho = double.Parse(item.Value.ToString().Replace(",", ""));
                        switch (item.type)
                        {
                            case (int)commonEnumHellper.KeyCompare.bang:
                                listTon = listTon.Where(o => o.TonKho == tonkho).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                listTon = listTon.Where(o => o.TonKho > tonkho).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                listTon = listTon.Where(o => o.TonKho >= tonkho).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                listTon = listTon.Where(o => o.TonKho < tonkho).ToList();
                                break;
                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                listTon = listTon.Where(o => o.TonKho <= tonkho).ToList();
                                break;
                            default:
                                break;
                        }
                        break;
                    case (int)GridHellper.ColumnHangHoa.trangthaiXoa:
                        listTon = listTon.Where(o => o.Xoa == bool.Parse(item.Value.ToString())).ToList();
                        break;
                    default:
                        listTon = listTon.Where(o => o.TrangThai == bool.Parse(item.Value.ToString())).ToList();
                        break;

                }
            }
            return listTon;
        }
        public double? TinhSLTonHH(Guid id_HangHoa, Guid iddonvi)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            DateTime timeEnd = DateTime.Now;
            Guid ID_ChiNhanh = iddonvi;
            Guid ID_HangHoa = id_HangHoa;

            paramlist.Add(new SqlParameter("timeEnd", timeEnd));
            paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            paramlist.Add(new SqlParameter("ID_HangHoa", ID_HangHoa));
            List<DM_Ton> listTon = db.Database.SqlQuery<DM_Ton>("exec TinhSLTon @timeEnd, @ID_ChiNhanh,@ID_HangHoa", paramlist.ToArray()).ToList();
            return listTon.Count > 0 ? Math.Round(listTon.FirstOrDefault().TonKho.Value, 3, MidpointRounding.ToEven) : 0;
        }

        public List<DM_HangHoaDTO> GetListHangHoaKiemKho(Guid iddonvi)
        {
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();

            if (db != null)
            {
                var tbl_hanghoa = from hh in db.DM_HangHoa
                                  join dvt in db.DonViQuiDois.Where(x => x.Xoa != true) on hh.ID equals dvt.ID_HangHoa
                                  where hh.TheoDoi == true
                                  //into hh_dvt from qd in hh_dvt.DefaultIfEmpty() where qd==null
                                  select new
                                  {
                                      ID = dvt.ID_HangHoa,
                                      TenHangHoa = hh.TenHangHoa,
                                      TheoDoi = hh.TheoDoi,
                                      MaHangHoa = dvt.MaHangHoa,
                                      GiaBan = dvt.GiaBan,
                                      GiaVon = dvt.GiaVon,
                                      TenDonViTinh = dvt.TenDonViTinh,
                                      ID_NhomHang = hh.ID_NhomHang,
                                      SoLuong = 0,
                                      ID_DonViQuiDoi = dvt.ID,
                                      TyLeChuyenDoi = dvt.TyLeChuyenDoi,
                                  };
                var tbl_tinhton = from hd in db.BH_HoaDon
                                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                                  where hd.ChoThanhToan == false & (dv.ID == iddonvi || hd.ID_CheckIn == iddonvi)
                                  group new { dvqd, hd, bhct } by new
                                  {
                                      dvqd.ID_HangHoa,
                                  } into g
                                  select new
                                  {
                                      ID_HangHoa = g.Key.ID_HangHoa,
                                      SoLuongNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 4 || x.hd.LoaiHoaDon == 9 || x.hd.LoaiHoaDon == 6 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "3")).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongNhanChuyen = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value == iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongXuat = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "1") || x.hd.LoaiHoaDon == 7 || x.hd.LoaiHoaDon == 8).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongChuyenXuat = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value != iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                  };

                var tbl_gop = from hh in tbl_hanghoa
                              join dvt in tbl_tinhton on hh.ID equals dvt.ID_HangHoa into listHH
                              from listhh in listHH.DefaultIfEmpty()

                              select new DM_HangHoaDTO()
                              {
                                  ID = hh.ID,
                                  TenHangHoa = hh.TenHangHoa,
                                  TheoDoi = hh.TheoDoi,
                                  MaHangHoa = hh.MaHangHoa,
                                  TonKho = (double?)(listhh.SoLuongNhap + listhh.SoLuongNhanChuyen - listhh.SoLuongXuat - listhh.SoLuongChuyenXuat) / (hh.TyLeChuyenDoi) ?? 0,
                                  GiaBan = hh.GiaBan,
                                  GiaVon = hh.GiaVon,
                                  TenDonViTinh = hh.TenDonViTinh,
                                  ID_NhomHangHoa = hh.ID_NhomHang,
                                  SoLuong = 0,
                                  ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                              };
                var tbl = tbl_gop.AsEnumerable().Select(hh => new DM_HangHoaDTO
                {
                    ID = hh.ID,
                    TenHangHoa = hh.TenHangHoa,
                    TheoDoi = hh.TheoDoi,
                    MaHangHoa = hh.MaHangHoa,
                    TonKho = Math.Round(hh.TonKho.Value, 3, MidpointRounding.ToEven),
                    GiaBan = hh.GiaBan,
                    GiaVon = hh.GiaVon,
                    TenDonViTinh = hh.TenDonViTinh,
                    ID_NhomHangHoa = hh.ID_NhomHangHoa,
                    SoLuong = 0,
                    ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                });
                return tbl.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas_QuyDoiNH(Guid iddonvi)
        {
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();

            if (db != null)
            {
                var tbl_hanghoa = from hh in db.DM_HangHoa
                                  join dvt in db.DonViQuiDois.Where(x => x.Xoa != true) on hh.ID equals dvt.ID_HangHoa
                                  where hh.TheoDoi == true
                                  //into hh_dvt from qd in hh_dvt.DefaultIfEmpty() where qd==null
                                  select new
                                  {
                                      ID = dvt.ID_HangHoa,
                                      TenHangHoa = hh.TenHangHoa,
                                      TheoDoi = hh.TheoDoi,
                                      MaHangHoa = dvt.MaHangHoa,
                                      GiaBan = dvt.GiaBan,
                                      GiaVon = dvt.GiaVon,
                                      TenDonViTinh = dvt.TenDonViTinh,
                                      ID_NhomHang = hh.ID_NhomHang,
                                      SoLuong = 0,
                                      ID_DonViQuiDoi = dvt.ID,
                                      TyLeChuyenDoi = dvt.TyLeChuyenDoi,
                                      LaHangHoa = hh.LaHangHoa,
                                  };
                var tbl_tinhton = from hd in db.BH_HoaDon
                                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                                  where hd.ChoThanhToan == false & (dv.ID == iddonvi || hd.ID_CheckIn == iddonvi)
                                  group new { dvqd, hd, bhct } by new
                                  {
                                      dvqd.ID_HangHoa,
                                  } into g
                                  select new
                                  {
                                      ID_HangHoa = g.Key.ID_HangHoa,
                                      SoLuongNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 4 || x.hd.LoaiHoaDon == 9 || x.hd.LoaiHoaDon == 6).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongNhanChuyen = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value == iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongXuat = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "1") || x.hd.LoaiHoaDon == 7 || x.hd.LoaiHoaDon == 8).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongChuyenXuat = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value != iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                  };

                var tbl_gop = from hh in tbl_hanghoa
                              join dvt in tbl_tinhton on hh.ID equals dvt.ID_HangHoa into listHH
                              from listhh in listHH.DefaultIfEmpty()

                              select new DM_HangHoaDTO()
                              {
                                  ID = hh.ID,
                                  TenHangHoa = hh.TenHangHoa,
                                  TheoDoi = hh.TheoDoi,
                                  MaHangHoa = hh.MaHangHoa,
                                  TonKho = (double?)(listhh.SoLuongNhap + listhh.SoLuongNhanChuyen - listhh.SoLuongXuat - listhh.SoLuongChuyenXuat) / (hh.TyLeChuyenDoi) ?? 0,
                                  GiaBan = hh.GiaBan,
                                  GiaVon = hh.GiaVon,
                                  TenDonViTinh = hh.TenDonViTinh,
                                  ID_NhomHangHoa = hh.ID_NhomHang,
                                  SoLuong = 0,
                                  ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                                  LaHangHoa = hh.LaHangHoa,
                              };
                var tbl = tbl_gop.AsEnumerable().Select(hh => new DM_HangHoaDTO
                {
                    ID = hh.ID,
                    TenHangHoa = hh.TenHangHoa,
                    TenHangHoaUnsign = CommonStatic.ConvertToUnSign(hh.TenHangHoa).ToLower(),
                    TenHangHoaCharStart = CommonStatic.GetCharsStart(hh.TenHangHoa).ToLower(),
                    MaHangHoaUnsign = CommonStatic.ConvertToUnSign(hh.MaHangHoa).ToLower(),
                    MaHangHoaCharStart = CommonStatic.GetCharsStart(hh.MaHangHoa).ToLower(),
                    TheoDoi = hh.TheoDoi,
                    MaHangHoa = hh.MaHangHoa,
                    TonKho = Math.Round(hh.TonKho.Value, 3, MidpointRounding.ToEven),
                    GiaBan = hh.GiaBan,
                    GiaVon = hh.GiaVon,
                    TenDonViTinh = hh.TenDonViTinh,
                    ID_NhomHangHoa = hh.ID_NhomHangHoa,
                    SoLuong = 0,
                    ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                    LaHangHoa = hh.LaHangHoa,
                });

                return tbl.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas_QuyDoiNH_Anh(Guid iddonvi)
        {
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();

            if (db != null)
            {
                var tbl_hanghoa = from hh in db.DM_HangHoa
                                  join dvt in db.DonViQuiDois.Where(x => x.Xoa != true) on hh.ID equals dvt.ID_HangHoa
                                  where hh.TheoDoi == true && hh.DuocBanTrucTiep == true
                                  select new
                                  {
                                      ID = dvt.ID_HangHoa,
                                      TenHangHoa = hh.TenHangHoa,
                                      TheoDoi = hh.TheoDoi,
                                      MaHangHoa = dvt.MaHangHoa,
                                      GiaBan = dvt.GiaBan,
                                      GiaVon = dvt.GiaVon,
                                      TenDonViTinh = dvt.TenDonViTinh,
                                      ID_NhomHang = hh.ID_NhomHang,
                                      SoLuong = 0,
                                      ID_DonViQuiDoi = dvt.ID,
                                      TyLeChuyenDoi = dvt.TyLeChuyenDoi,
                                      LaHangHoa = hh.LaHangHoa
                                  };

                var tbl_tinhton = from hd in db.BH_HoaDon
                                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                                  where hd.ChoThanhToan == false & (dv.ID == iddonvi || hd.ID_CheckIn == iddonvi)
                                  group new { dvqd, hd, bhct } by new
                                  {
                                      dvqd.ID_HangHoa,
                                  } into g
                                  select new
                                  {
                                      ID_HangHoa = g.Key.ID_HangHoa,
                                      SoLuongNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 4 || x.hd.LoaiHoaDon == 9 || x.hd.LoaiHoaDon == 6 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "3")).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongNhanChuyen = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value == iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongXuat = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "1") || x.hd.LoaiHoaDon == 7 || x.hd.LoaiHoaDon == 8).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongChuyenXuat = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value != iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                  };

                var tbl_gop1 = from hh in tbl_hanghoa
                               join hh_anh in db.DM_HangHoa_Anh on hh.ID equals hh_anh.ID_HangHoa into HH_Anh
                               from anh in HH_Anh.DefaultIfEmpty()

                               select new
                               {
                                   ID = hh.ID,
                                   TenHangHoa = hh.TenHangHoa,
                                   TheoDoi = hh.TheoDoi,
                                   MaHangHoa = hh.MaHangHoa,
                                   //TonKho = (double?)(listhh.SoLuongNhap + listhh.SoLuongNhanChuyen - listhh.SoLuongXuat - listhh.SoLuongChuyenXuat) / (hh.TyLeChuyenDoi) ?? 0,
                                   GiaBan = hh.GiaBan,
                                   GiaVon = hh.GiaVon,
                                   TenDonViTinh = hh.TenDonViTinh,
                                   ID_NhomHangHoa = hh.ID_NhomHang,
                                   SoLuong = 0,
                                   ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                                   LaHangHoa = hh.LaHangHoa,
                                   SrcImage = anh.URLAnh,
                                   TyLeChuyenDoi = hh.TyLeChuyenDoi
                               };
                var tbl_gop = from hh in tbl_gop1
                              join dvt in tbl_tinhton on hh.ID equals dvt.ID_HangHoa into listHH
                              from listhh in listHH.DefaultIfEmpty()
                              select new DM_HangHoaDTO()
                              {
                                  ID = hh.ID,
                                  TenHangHoa = hh.TenHangHoa,
                                  TheoDoi = hh.TheoDoi,
                                  MaHangHoa = hh.MaHangHoa,
                                  TonKho = (double?)(listhh.SoLuongNhap + listhh.SoLuongNhanChuyen - listhh.SoLuongXuat - listhh.SoLuongChuyenXuat) / (hh.TyLeChuyenDoi) ?? 0,
                                  GiaBan = hh.GiaBan,
                                  GiaVon = hh.GiaVon,
                                  TenDonViTinh = hh.TenDonViTinh,
                                  ID_NhomHangHoa = hh.ID_NhomHangHoa,
                                  SoLuong = 0,
                                  ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                                  LaHangHoa = hh.LaHangHoa,
                                  SrcImage = hh.SrcImage,
                              };
                var tbl = tbl_gop.AsEnumerable().Select(hh => new DM_HangHoaDTO
                {
                    ID = hh.ID,
                    TenHangHoa = hh.TenHangHoa,
                    TheoDoi = hh.TheoDoi,
                    MaHangHoa = hh.MaHangHoa,
                    TonKho = Math.Round(hh.TonKho.Value, 3, MidpointRounding.ToEven),
                    GiaBan = hh.GiaBan,
                    GiaVon = hh.GiaVon,
                    TenDonViTinh = hh.TenDonViTinh,
                    ID_NhomHangHoa = hh.ID_NhomHangHoa,
                    SoLuong = 0,
                    ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                    SrcImage = hh.SrcImage,
                    LaHangHoa = hh.LaHangHoa
                });
                return tbl.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<SP_DM_HangHoaDTO> Sp_GetListHangHoas_QuyDoiNH_IEnumerable(Guid idDonVi)
        {
            try
            {
                SqlParameter paraSql = new SqlParameter("ID_ChiNhanh", idDonVi);
                List<SP_DM_HangHoaDTO> data = db.Database.SqlQuery<SP_DM_HangHoaDTO>("exec Load_DMHangHoa_TonKho @ID_ChiNhanh", paraSql).ToList();
                db.Database.CommandTimeout = 3000;
                return data;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Sp_GetListHangHoas_QuyDoiNH_IEnumerable " + ex.InnerException + ex.Message);
                return new List<SP_DM_HangHoaDTO>();
            }
        }

        public List<SP_DM_HangHoaDTO> Sp_GetListHangHoas_QuyDoiNH_Anh(Guid idDonVi)
        {
            try
            {
                var tbl_timeCSt = from cs in db.ChotSo
                                  where cs.ID_DonVi == idDonVi
                                  select cs;

                SqlParameter paraSql = new SqlParameter("ID_ChiNhanh", idDonVi);

                List<SP_DM_HangHoaDTO> lst = null;
                if (tbl_timeCSt.Count() > 0)
                {
                    lst = db.Database.SqlQuery<SP_DM_HangHoaDTO>("exec Load_DMHangHoa_TonKho_ChotSo @ID_ChiNhanh", paraSql).ToList();
                }
                else
                {
                    lst = db.Database.SqlQuery<SP_DM_HangHoaDTO>("exec Load_DMHangHoa_TonKho @ID_ChiNhanh", paraSql).ToList();
                }
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Sp_GetListHangHoas_QuyDoiNH_Anh: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas_QuyDoiBangGia(Guid iddonvi)
        {
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();

            if (db != null)
            {
                var tbl_hanghoa = from hh in db.DM_HangHoa
                                  join dvt in db.DonViQuiDois.Where(x => x.Xoa != true) on hh.ID equals dvt.ID_HangHoa
                                  join tonk in db.Kho_TonKhoKhoiTao on dvt.ID equals tonk.ID_DonViQuiDoi into listHH
                                  from list in listHH.DefaultIfEmpty()
                                  where hh.TheoDoi != false
                                  select new
                                  {
                                      ID = dvt.ID_HangHoa,
                                      TenHangHoa = hh.TenHangHoa,
                                      TheoDoi = hh.TheoDoi,
                                      MaHangHoa = dvt.MaHangHoa,
                                      GiaBan = dvt.GiaBan,
                                      GiaVon = dvt.GiaVon,
                                      TenDonViTinh = dvt.TenDonViTinh,
                                      ID_NhomHang = hh.ID_NhomHang,
                                      SoLuong = 0,
                                      ID_DonViQuiDoi = dvt.ID,
                                      Xoa = dvt.Xoa,
                                      TyLeChuyenDoi = dvt.TyLeChuyenDoi
                                  };
                var tbl_tinhton = from hd in db.BH_HoaDon
                                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                                  where hd.ChoThanhToan == false & (dv.ID == iddonvi || hd.ID_CheckIn == iddonvi)
                                  group new { dvqd, hd, bhct } by new
                                  {
                                      dvqd.ID_HangHoa,
                                  } into g
                                  select new
                                  {
                                      ID_HangHoa = g.Key.ID_HangHoa,
                                      SoLuongNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 4 || x.hd.LoaiHoaDon == 9 || x.hd.LoaiHoaDon == 6 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "3")).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongNhanChuyen = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value == iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongXuat = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "1") || x.hd.LoaiHoaDon == 7 || x.hd.LoaiHoaDon == 8).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongChuyenXuat = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value != iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                  };

                var tbl_gop = from hh in tbl_hanghoa
                              join dvt in tbl_tinhton on hh.ID equals dvt.ID_HangHoa into listHH
                              from listhh in listHH.DefaultIfEmpty()

                              select new DM_HangHoaDTO()
                              {
                                  ID = hh.ID,
                                  TenHangHoa = hh.TenHangHoa,
                                  TheoDoi = hh.TheoDoi,
                                  MaHangHoa = hh.MaHangHoa,
                                  TonKho = (double?)(listhh.SoLuongNhap + listhh.SoLuongNhanChuyen - listhh.SoLuongXuat - listhh.SoLuongChuyenXuat) / (hh.TyLeChuyenDoi) ?? 0,
                                  GiaBan = hh.GiaBan,
                                  GiaVon = hh.GiaVon,
                                  TenDonViTinh = hh.TenDonViTinh,
                                  ID_NhomHangHoa = hh.ID_NhomHang,
                                  SoLuong = 0,
                                  ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                              };
                var tbl = tbl_gop.AsEnumerable().Select(hh => new DM_HangHoaDTO
                {
                    ID = hh.ID,
                    TenHangHoa = hh.TenHangHoa,
                    TheoDoi = hh.TheoDoi,
                    MaHangHoa = hh.MaHangHoa,
                    TonKho = Math.Round(hh.TonKho.Value, 3, MidpointRounding.ToEven),
                    GiaBan = hh.GiaBan,
                    GiaVon = hh.GiaVon,
                    TenDonViTinh = hh.TenDonViTinh,
                    ID_NhomHangHoa = hh.ID_NhomHangHoa,
                    SoLuong = 0,
                    ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                });
                return tbl.ToList();

            }
            else
            {
                return null;
            }
        }

        public List<DM_TheKhoDTO> GetListTheKho(Guid id, Guid iddonvi)
        {
            List<DM_TheKhoDTO> lst = new List<DM_TheKhoDTO>();
            if (db != null)
            {

                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HangHoa", id));
                paramlist.Add(new SqlParameter("IDChiNhanh", iddonvi));
                return db.Database.SqlQuery<DM_TheKhoDTO>("Exec ListHangHoaTheKho @ID_HangHoa, @IDChiNhanh", paramlist.ToArray()).ToList();
            }
            return lst;
        }

        public List<DM_TheKhoDTO> GetListTheKhoByMaLoHang(Guid idlohang, Guid iddonvi, Guid idhanghoa)
        {
            List<DM_TheKhoDTO> lst = new List<DM_TheKhoDTO>();
            if (db != null)
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HangHoa", idhanghoa));
                paramlist.Add(new SqlParameter("IDChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("ID_LoHang", idlohang));
                return db.Database.SqlQuery<DM_TheKhoDTO>("Exec ListHangHoaTheKhoTheoLoHang @ID_HangHoa, @IDChiNhanh, @ID_LoHang", paramlist.ToArray()).ToList();

            }
            return lst;
        }

        public List<DM_HangHoaDTO> GetListHangHoas_QuyDoiTH(Guid iddonvi)
        {
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();

            if (db != null)
            {
                var tbl_hanghoa = from hh in db.DM_HangHoa
                                  join dvt in db.DonViQuiDois.Where(p => p.Xoa != true) on hh.ID equals dvt.ID_HangHoa
                                  join tonk in db.Kho_TonKhoKhoiTao on dvt.ID equals tonk.ID_DonViQuiDoi into listHH
                                  from list in listHH.DefaultIfEmpty()
                                  where hh.TheoDoi == true
                                  //into hh_dvt from qd in hh_dvt.DefaultIfEmpty() where qd==null
                                  select new
                                  {
                                      ID = dvt.ID_HangHoa,
                                      TenHangHoa = hh.TenHangHoa,
                                      TheoDoi = hh.TheoDoi,
                                      MaHangHoa = dvt.MaHangHoa,
                                      GiaBan = dvt.GiaBan,
                                      GiaVon = dvt.GiaVon,
                                      GiaNhap = dvt.GiaNhap,
                                      TenDonViTinh = dvt.TenDonViTinh,
                                      ID_NhomHang = hh.ID_NhomHang,
                                      SoLuong = 0,
                                      ID_DonViQuiDoi = dvt.ID,
                                      TyLeChuyenDoi = dvt.TyLeChuyenDoi,
                                      Xoa = dvt.Xoa
                                  };
                var tbl_tinhton = from hd in db.BH_HoaDon
                                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                                  where hd.ChoThanhToan == false & (dv.ID == iddonvi || hd.ID_CheckIn == iddonvi)
                                  group new { dvqd, hd, bhct } by new
                                  {
                                      dvqd.ID_HangHoa,
                                  } into g
                                  select new
                                  {
                                      ID_HangHoa = g.Key.ID_HangHoa,
                                      SoLuongNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 4 || x.hd.LoaiHoaDon == 9 || x.hd.LoaiHoaDon == 6 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "3")).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongNhanChuyen = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value == iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongXuat = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "1") || x.hd.LoaiHoaDon == 7 || x.hd.LoaiHoaDon == 8).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                                      SoLuongChuyenXuat = (double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value != iddonvi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0,
                                  };
                var tbl_gop = from hh in tbl_hanghoa
                              join dvt in tbl_tinhton on hh.ID equals dvt.ID_HangHoa into listHH
                              from listhh in listHH.DefaultIfEmpty()

                              select new DM_HangHoaDTO()
                              {
                                  ID = hh.ID,
                                  TenHangHoa = hh.TenHangHoa,
                                  TheoDoi = hh.TheoDoi,
                                  MaHangHoa = hh.MaHangHoa,
                                  TonKho = (double?)(listhh.SoLuongNhap + listhh.SoLuongNhanChuyen - listhh.SoLuongXuat - listhh.SoLuongChuyenXuat) / (hh.TyLeChuyenDoi) ?? 0,
                                  GiaBan = hh.GiaBan,
                                  GiaVon = hh.GiaVon,
                                  GiaNhap = hh.GiaNhap,
                                  TenDonViTinh = hh.TenDonViTinh,
                                  ID_NhomHangHoa = hh.ID_NhomHang,
                                  SoLuong = 0,
                                  ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                              };
                var tbl = tbl_gop.AsEnumerable().Select(hh => new DM_HangHoaDTO
                {
                    ID = hh.ID,
                    TenHangHoa = hh.TenHangHoa,
                    TheoDoi = hh.TheoDoi,
                    MaHangHoa = hh.MaHangHoa,
                    TonKho = Math.Round(hh.TonKho.Value, 3, MidpointRounding.ToEven),
                    GiaBan = hh.GiaBan,
                    GiaVon = hh.GiaVon,
                    GiaNhap = hh.GiaNhap,
                    TenDonViTinh = hh.TenDonViTinh,
                    ID_NhomHangHoa = hh.ID_NhomHangHoa,
                    SoLuong = 0,
                    ID_DonViQuiDoi = hh.ID_DonViQuiDoi,
                }).Where(p => p.TonKho > 0);
                return tbl.ToList();
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;

        }
        #endregion

        #region insert
        //thuộc tính
        public string add_ThuocTinh(DM_ThuocTinh objThuocTinh)
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
                    db.DM_ThuocTinh.Add(objThuocTinh);
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

        //hàng hóa vị trí
        public string addViTriHangHoa(DM_HangHoa_ViTri objnewHH)
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
                    db.DM_HangHoa_ViTri.Add(objnewHH);
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

        public string AddViTriChoHangHoa(DM_ViTriHangHoa objnewHH)
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
                    db.DM_ViTriHangHoa.Add(objnewHH);
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

        public string add_HangHoa_ThuocTinh(HangHoa_ThuocTinh objHH_ThuocTinh)
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
                    db.HangHoa_ThuocTinh.Add(objHH_ThuocTinh);
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

        public string UpdateHH_thuocTinh(HangHoa_ThuocTinh objNew)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
                return strErr;
            }
            else
            {
                try
                {
                    HangHoa_ThuocTinh objUpd = db.HangHoa_ThuocTinh.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.ID_ThuocTinh = objNew.ID_ThuocTinh;
                        objUpd.GiaTri = objNew.GiaTri;
                        objUpd.ThuTuNhap = objNew.ThuTuNhap;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        #endregion
                        db.SaveChanges();
                    }
                    else
                    {
                        #region insert new
                        return "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string UpdateViTriHangHoa(DM_HangHoa_ViTri objNew)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
                return strErr;
            }
            else
            {
                try
                {
                    DM_HangHoa_ViTri objUpd = db.DM_HangHoa_ViTri.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.TenViTri = objNew.TenViTri;
                        objUpd.NgaySua = DateTime.Now;
                        db.Entry(objUpd).State = EntityState.Modified;
                        #endregion
                        db.SaveChanges();
                    }
                    else
                    {
                        #region insert new
                        return "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }


        public string Update_thuocTinh(DM_ThuocTinh objNew)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
                return strErr;
            }
            else
            {
                try
                {
                    DM_ThuocTinh objUpd = db.DM_ThuocTinh.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.TenThuocTinh = objNew.TenThuocTinh;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        #endregion
                        db.SaveChanges();
                    }
                    else
                    {
                        #region insert new
                        return "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Add_HangHoa(DM_HangHoa objHHAdd)
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
                    db.DM_HangHoa.Add(objHHAdd);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    strErr = "Add_HangHoa " + e.InnerException + e.Message;
                }
            }
            return strErr;
        }

        public string Add_Image(DM_HangHoa_Anh objHH_AnhAdd)
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
                    db.DM_HangHoa_Anh.Add(objHH_AnhAdd);
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

        public string DeleteHH_ThuocTinh(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                HangHoa_ThuocTinh objDel = db.HangHoa_ThuocTinh.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        db.HangHoa_ThuocTinh.Remove(objDel);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        return exxx.Message;
                    }
                }
            }
            return string.Empty;
        }

        public string delete_Anh(Guid id)
        {
            string strErr = string.Empty;
            if (db != null)
            {
                DM_HangHoa_Anh objDel = db.DM_HangHoa_Anh.Find(id);
                db.DM_HangHoa_Anh.Remove(objDel);
                db.SaveChanges();
                return strErr;
            }
            else
            {
                return "Lỗi";
            }
        }

        public string Delete_ThuocTinh(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_ThuocTinh objDel = db.DM_ThuocTinh.Find(id);
                if (objDel != null)
                {
                    // find in HangHoa_ThuocTinh & delete
                    var data = db.HangHoa_ThuocTinh.Where(x => x.ID_ThuocTinh == id);
                    if (data != null && data.Count() > 0)
                    {
                        db.HangHoa_ThuocTinh.RemoveRange(data);
                    }
                    db.DM_ThuocTinh.Remove(objDel);
                    db.SaveChanges();
                }
            }
            return strErr;
        }
        #endregion

        #region update
        public string Update_HangHoa_KD(DM_HangHoa objHHNew, List<CongDoan_DichVu> lstCongDoans)
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
                    #region DM_HangHoa
                    DM_HangHoa objUpd = db.DM_HangHoa.Find(objHHNew.ID);
                    objUpd.GhiChu = objHHNew.GhiChu;
                    objUpd.TheoDoi = objHHNew.TheoDoi;
                    objUpd.QuyCach = objHHNew.QuyCach;
                    objUpd.TonToiThieu = objHHNew.TonToiThieu;
                    objUpd.TonToiDa = objHHNew.TonToiDa;
                    objUpd.DuocBanTrucTiep = objHHNew.DuocBanTrucTiep;
                    objUpd.ID_NhomHang = objHHNew.ID_NhomHang;
                    objUpd.NgaySua = objHHNew.NgaySua;
                    objUpd.NguoiSua = objHHNew.NguoiSua;
                    objUpd.TenHangHoa = objHHNew.TenHangHoa;
                    #endregion

                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Update_HangHoa(DM_HangHoa objHHNew, List<CongDoan_DichVu> lstCongDoans)
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
                    DM_HangHoa objUpd = db.DM_HangHoa.Find(objHHNew.ID);
                    objUpd.GhiChu = objHHNew.GhiChu;
                    objUpd.ID_HangHoaCungLoai = objHHNew.ID_HangHoaCungLoai != null ? objHHNew.ID_HangHoaCungLoai : Guid.NewGuid();
                    objUpd.QuyCach = objHHNew.QuyCach;
                    objUpd.TonToiThieu = objHHNew.TonToiThieu;
                    objUpd.TonToiDa = objHHNew.TonToiDa;
                    objUpd.DuocBanTrucTiep = objHHNew.DuocBanTrucTiep;
                    objUpd.QuanLyTheoLoHang = objHHNew.QuanLyTheoLoHang;
                    objUpd.ID_NhomHang = objUpd.LaHangHoa == true ? (objHHNew.ID_NhomHang == null ? Guid.Empty : objHHNew.ID_NhomHang) : (objHHNew.ID_NhomHang == null ? new Guid("00000000-0000-0000-0000-000000000001") : objHHNew.ID_NhomHang);
                    objUpd.NgaySua = objHHNew.NgaySua;
                    objUpd.NguoiSua = objHHNew.NguoiSua;
                    objUpd.TenHangHoa = objHHNew.TenHangHoa;
                    objUpd.ThoiGianBaoHanh = objHHNew.ThoiGianBaoHanh;
                    objUpd.LoaiBaoHanh = objHHNew.LoaiBaoHanh;
                    objUpd.DonViTinhQuyCach = objHHNew.DonViTinhQuyCach;
                    objUpd.DuocTichDiem = objHHNew.DuocTichDiem;
                    objUpd.QuanLyBaoDuong = objHHNew.QuanLyBaoDuong;
                    objUpd.LoaiBaoDuong = objHHNew.LoaiBaoDuong;
                    objUpd.LoaiHangHoa = objHHNew.LoaiHangHoa;
                    objUpd.SoKmBaoDuong = objHHNew.SoKmBaoDuong;
                    objUpd.HoaHongTruocChietKhau = objHHNew.HoaHongTruocChietKhau;
                    objUpd.ID_Xe = objHHNew.ID_Xe;
                    objUpd.ChietKhauMD_NV = objHHNew.ChietKhauMD_NV;
                    objUpd.ChietKhauMD_NVTheoPT = objHHNew.ChietKhauMD_NVTheoPT;

                    objUpd.TenHangHoa_KhongDau = CommonStatic.ConvertToUnSign(objHHNew.TenHangHoa).ToLower();
                    objUpd.TenHangHoa_KyTuDau = CommonStatic.GetCharsStart(objHHNew.TenHangHoa).ToLower();
                    if (objUpd.LaHangHoa == false)
                    {
                        objUpd.ChiPhiThucHien = objHHNew.ChiPhiThucHien;
                        objUpd.ChiPhiTinhTheoPT = objHHNew.ChiPhiTinhTheoPT;
                        objUpd.SoPhutThucHien = objHHNew.SoPhutThucHien;
                        objUpd.DichVuTheoGio = objHHNew.DichVuTheoGio;
                    }
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }


        public string UpdateHHKhiXoaNhomHH(Guid idnhom, bool lanhomhh)
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
                    if (lanhomhh == true)
                    {
                        db.DM_HangHoa.Where(p => p.ID_NhomHang == idnhom).ToList().ForEach(p => p.ID_NhomHang = Guid.Empty);
                    }
                    else
                    {
                        db.DM_HangHoa.Where(p => p.ID_NhomHang == idnhom).ToList().ForEach(p => p.ID_NhomHang = new Guid("00000000-0000-0000-0000-000000000001"));
                    }
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        public string Update_HangHoaDELETE(DM_HangHoa objHHNew, List<CongDoan_DichVu> lstCongDoans)
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
                    #region DM_HangHoa
                    DM_HangHoa objUpd = db.DM_HangHoa.Find(objHHNew.ID);
                    objUpd.GhiChu = objHHNew.GhiChu;
                    objUpd.ID_HangHoaCungLoai = objHHNew.ID_HangHoaCungLoai != null ? objHHNew.ID_HangHoaCungLoai : Guid.NewGuid();
                    objUpd.LaChaCungLoai = objHHNew.LaChaCungLoai;
                    objUpd.QuyCach = objHHNew.QuyCach;
                    objUpd.DuocBanTrucTiep = objHHNew.DuocBanTrucTiep;
                    objUpd.ID_NhomHang = objHHNew.ID_NhomHang;
                    objUpd.NgaySua = objHHNew.NgaySua;
                    objUpd.NgayTao = objHHNew.NgayTao;
                    objUpd.NguoiSua = objHHNew.NguoiSua;
                    objUpd.TenHangHoa = objHHNew.TenHangHoa;
                    objUpd.TenHangHoa_KhongDau = CommonStatic.ConvertToUnSign(objHHNew.TenHangHoa).ToLower();
                    objUpd.TenHangHoa_KyTuDau = CommonStatic.GetCharsStart(objHHNew.TenHangHoa).ToLower();
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Update_HangHoaKhiNgungKD(DM_HangHoa objHHNew, List<CongDoan_DichVu> lstCongDoans)
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
                    DM_HangHoa objUpd = db.DM_HangHoa.Find(objHHNew.ID);
                    objUpd.LaChaCungLoai = objHHNew.LaChaCungLoai;
                    objUpd.NgayTao = objHHNew.NgayTao;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public string Update_HangHoaKhiChoKD(DM_HangHoa objHHNew, List<CongDoan_DichVu> lstCongDoans)
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
                    DM_HangHoa objUpd = db.DM_HangHoa.Find(objHHNew.ID);
                    objUpd.LaChaCungLoai = objHHNew.LaChaCungLoai;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion

        #region delete
        string CheckDelete_HangHoa(SsoftvnContext db, DM_HangHoa objHHDel)
        {
            string strCheck = string.Empty;
            if (objHHDel.DonViQuiDois != null && objHHDel.DonViQuiDois.Count > 0)
            {
                foreach (DonViQuiDoi objDVT in objHHDel.DonViQuiDois)
                {
                    if (objDVT.BH_HoaDon_ChiTiet != null && objDVT.BH_HoaDon_ChiTiet.Count > 0)
                    {
                        strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để lập Hóa đơn bán.";
                        return strCheck;
                    }
                    if (objDVT.DinhLuongDichVus != null && objDVT.DinhLuongDichVus.Count > 0)
                    {
                        strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để lập Định lượng cho hàng hóa/dịch vụ khác.";
                        return strCheck;
                    }
                    if (objDVT.Kho_HoaDon_ChiTiet != null && objDVT.Kho_HoaDon_ChiTiet.Count > 0)
                    {
                        strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để lập phiếu xuất/nhập kho";
                        return strCheck;
                    }
                    if (objDVT.Kho_TonKhoKhoiTao != null && objDVT.Kho_TonKhoKhoiTao.Count > 0)
                    {
                        strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để khai báo Tồn kho";
                        return strCheck;
                    }
                    if (objDVT.The_TheKhachHang_ChiTiet != null && objDVT.The_TheKhachHang_ChiTiet.Count > 0)
                    {
                        strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để khai báo Thẻ khách hàng";
                        return strCheck;
                    }
                }
            }

            List<CongDoan_DichVu> lstCongDoans = db.CongDoan_DichVu.Where(p => p.ID_CongDoan == objHHDel.ID).ToList();
            if (lstCongDoans != null && lstCongDoans.Count > 0)
            {
                strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để lập danh mục công đoạn cho hàng hóa/dịch vụ khác.";
                return strCheck;
            }

            return strCheck;
        }

        public string Delete_HangHoa(Guid id)
        {
            string strErr = string.Empty;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Kết nối CSDL không hợp lệ";
                }
                else
                {
                    DM_HangHoa objDel = db.DM_HangHoa.Find(id);
                    if (objDel != null)
                    {
                        string strCheck = CheckDelete_HangHoa(db, objDel);
                        if (strCheck == string.Empty)
                        {
                            try
                            {
                                List<DM_LoHang> lstLoHangs = db.DM_LoHang.Where(p => p.ID_HangHoa == objDel.ID).ToList();
                                if (lstLoHangs != null && lstLoHangs.Count > 0)
                                    db.DM_LoHang.RemoveRange(lstLoHangs.ToList());
                                //
                                List<DM_MaVach> lstMaVachs = db.DM_MaVach.Where(p => p.ID_HangHoa == objDel.ID).ToList();
                                if (lstMaVachs != null && lstMaVachs.Count > 0)
                                    db.DM_MaVach.RemoveRange(lstMaVachs.ToList());
                                //
                                if (objDel.DonViQuiDois != null && objDel.DonViQuiDois.Count > 0)
                                {
                                    foreach (DonViQuiDoi deletedOrderDetail in objDel.DonViQuiDois)
                                    {
                                        if (deletedOrderDetail.ChietKhauMacDinh_NhanVien != null && deletedOrderDetail.ChietKhauMacDinh_NhanVien.Count > 0)
                                            db.ChietKhauMacDinh_NhanVien.RemoveRange(deletedOrderDetail.ChietKhauMacDinh_NhanVien.ToList());
                                        //
                                        if (deletedOrderDetail.DinhLuongDichVus != null && deletedOrderDetail.DinhLuongDichVus.Count > 0)
                                            db.DinhLuongDichVus.RemoveRange(deletedOrderDetail.DinhLuongDichVus.ToList());
                                        //
                                        if (deletedOrderDetail.DM_GiaBan_ChiTiet != null && deletedOrderDetail.DM_GiaBan_ChiTiet.Count > 0)
                                            db.DM_GiaBan_ChiTiet.RemoveRange(deletedOrderDetail.DM_GiaBan_ChiTiet.ToList());
                                    }
                                    db.DonViQuiDois.RemoveRange(objDel.DonViQuiDois.ToList());
                                }

                                List<CongDoan_DichVu> lstCongDoans = db.CongDoan_DichVu.Where(p => p.ID_DichVu == id).ToList();
                                if (lstCongDoans != null && lstCongDoans.Count > 0)
                                {
                                    db.CongDoan_DichVu.RemoveRange(lstCongDoans);
                                }
                                //
                                db.DM_HangHoa.Remove(objDel);
                                //
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
                            strErr = strCheck;
                            return strErr;
                        }
                    }
                    else
                    {
                        strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                        return strErr;
                    }
                }
            }
            return strErr;
        }

        public List<DM_HangHoaDTO> GetChiTietHangHoa(Guid id)
        {
            classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
            var tbl2 = from hh in db.DM_HangHoa.Where(hh => hh.ID == id) select hh;

            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            foreach (var item in tbl2)
            {
                DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
                var ctHH = _classDVQD.Select_DonViQuiDois_IDHangHoa(item.ID).FirstOrDefault();
                dM_HangHoaDTO.ID = item.ID;
                dM_HangHoaDTO.ID_NhomHangHoa = item.ID_NhomHang;
                dM_HangHoaDTO.MaHangHoa = item.DonViQuiDois == null ? "" : ctHH.MaHangHoa;
                dM_HangHoaDTO.TenHangHoa = item.TenHangHoa;
                dM_HangHoaDTO.SoLuong = 1;
                dM_HangHoaDTO.GiaBan = ctHH.GiaBan;
                dM_HangHoaDTO.ThanhTien = ctHH.GiaBan; // 1* DonGia
                dM_HangHoaDTO.GiamGia = 0;
                dM_HangHoaDTO.ID_DonViQuiDoi = ctHH.ID;
                lst.Add(dM_HangHoaDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }

        #endregion
    }
    public class DM_TheKhoDTO
    {
        public Guid ID_HangHoa { get; set; }
        public Guid ID_HoaDon { get; set; }
        public Guid ID_DoiTuong { get; set; }
        public Guid ID_DonVi { get; set; }
        public Guid? ID_CheckIn { get; set; }
        public Guid ID_BangGia { get; set; }
        public virtual List<BH_HoaDon_ChiTietDTO> BH_HoaDon_ChiTiet { get; set; }
        public int LoaiHoaDon { get; set; }
        public int SoThuTu { get; set; }
        public string MaHoaDon { get; set; }
        public string TenDonViChuyen { get; set; }
        public string TenDonViNhan { get; set; }
        public string LoaiChungTu { get; set; }
        public string YeuCau { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double TienChietKhau { get; set; }
        public double TyLeChuyenDoi { get; set; }
        public double? GiaVon { get; set; }
        public double? GiaVon_NhanChuyenHang { get; set; }
        public double? TonKho { get; set; }
        public double? TonCuoi { get; set; }
        public bool LaDonViChuan { get; set; }
        public double? TonLuyKe { get; set; }
        public double? TonLuyKe_NhanChuyenHang { get; set; }
    }

    public class List_TenDonViTinh
    {
        public Guid ID { get; set; }
        public string MaHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
    }
    public class List_DonViQuiDoi_HH
    {
        public Guid ID { get; set; }
        public Guid ID_HangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public double GiaVon { get; set; }
    }
    public class List_DonViQuiDoi_HH_LoHang
    {
        public Guid ID { get; set; }
        public Guid ID_HangHoa { get; set; }
        public Guid? ID_LoHang { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double GiaVon { get; set; }
    }
    public class List_DonViQuiDoi_ID_NhomHang
    {
        public double? SoThuTu { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string MaHangHoa { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double? GiaVonHienTai { get; set; }
        public double? GiaVonMoi { get; set; }
        public double? GiaVonTang { get; set; }
        public double? GiaVonGiam { get; set; }
    }
    public class listHangHoa_DieuChinh
    {
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string MaHangHoa { get; set; }
        public double GiaVonHienTai { get; set; }
        public double GiaVonMoi { get; set; }
        public double GiaVonTang { get; set; }
        public double GiaVonGiam { get; set; }
        public string GhiChu { get; set; }
    }

    // used to add hang hoa at banhang
    public class NewHangHoaBasic
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoa_KhongDau { get; set; }
        public string TenHangHoa_KyTuDau { get; set; }
        public string TenDonViTinh { get; set; }
        public double GiaBan { get; set; }
        public double GiaVon { get; set; }
        public double TonKho { get; set; }
        public bool LaHangHoa { get; set; }
        public bool QuanLyTheoLoHang { get; set; }
        public Guid ID_DonVi { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public Guid ID_NhanVien { get; set; }
        public string NguoiTao { get; set; }
        public Guid? ID_Xe { get; set; }
    }

    public class KiemKhoParamSearch
    {
        public Guid ID_ChiNhanh { get; set; }
        public string ToDate { get; set; }
        public List<string> ListIDQuyDoi { get; set; }
        public List<string> ListIDLoHang { get; set; }
    }
    public class KiemKho_HangHoaTonKho
    {
        public Guid ID_HangHoa { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public double? TonKho { get; set; }
        public double? GiaVon { get; set; }
        public double? GiaNhap { get; set; }// used to NhapHangChiTiet: change LoaiHoaDon --> get again GiaNhap from DB
        public double? GiaVonTieuChuan { get; set; }
    }
    public class Gara_ParamSearchHangHoa
    {
        public Guid ID_ChiNhanh { get; set; }
        public Guid? ID_BangGia { get; set; }
        public string TextSearch { get; set; }
        public string LaHangHoa { get; set; }
        public bool? ConTonKho { get; set; }
        public string QuanLyTheoLo { get; set; }
        public int? Form { get; set; } = 0;// default=0, 1.nhaphang
        public List<string> LstIDQuiDois { get; set; }
        public int? CurrentPage { get; set; } = 0;
        public int? PageSize { get; set; } = 500;
    }
}
