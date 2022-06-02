using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
    public class CuaHangDangKyService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<CuaHangDangKy> _CuahangService;
        IRepository<Contract> _Contract;
        IRepository<LichSuGiaHanCuaHang> _LichSuGiaHanCuaHang;
        IRepository<DM_GoiDichVu> _DM_GoiDichVu;
        IRepository<CuaHangNapTienDichVu> _CuaHangNapTienDichVu;

        public CuaHangDangKyService()
        {
            _CuahangService = unitOfWork.GetRepository<CuaHangDangKy>();
            _Contract = unitOfWork.GetRepository<Contract>();
            _LichSuGiaHanCuaHang = unitOfWork.GetRepository<LichSuGiaHanCuaHang>();
            _CuaHangNapTienDichVu = unitOfWork.GetRepository<CuaHangNapTienDichVu>();
            _DM_GoiDichVu = unitOfWork.GetRepository<DM_GoiDichVu>();
        }

        public IQueryable<CuaHangDangKy> Query { get { return _CuahangService.All().OrderByDescending(o => o.NgayTao).AsQueryable(); } }

        public IQueryable<ContractView> SearchContract(string search)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                return from a in _Contract.All()
                       join b in _CuahangService.All()
                       on a.StoreOpen.ToUpper() equals b.SubDomain.ToUpper()
                       into ps
                       from p in ps.DefaultIfEmpty()
                       where a.StoreOpen.ToUpper().Contains(search)
                                     || a.Phone.ToUpper().Contains(search)
                                     || a.Name.ToUpper().Contains(search)
                       select new ContractView
                       {
                           ID = a.ID,
                           StoreOpen = a.StoreOpen,
                           CreatedDate = a.CreatedDate,
                           IT_Name = p != null ? p.HoTen : string.Empty,
                           IT_Phone = p != null ? p.SoDienThoai : string.Empty,
                           ModifiedBy = a.ModifiedBy,
                           ModifiedDate = a.ModifiedDate,
                           Name = a.Name,
                           Phone = a.Phone,
                           Status = a.Status
                       };
            }
            return from a in _Contract.All()
                   join b in _CuahangService.All()
                   on a.StoreOpen.ToUpper() equals b.SubDomain.ToUpper()
                   into ps
                   from p in ps.DefaultIfEmpty()
                   select new ContractView
                   {
                       ID = a.ID,
                       StoreOpen = a.StoreOpen,
                       CreatedDate = a.CreatedDate,
                       IT_Name = p != null ? p.HoTen : string.Empty,
                       IT_Phone = p != null ? p.SoDienThoai : string.Empty,
                       ModifiedBy = a.ModifiedBy,
                       ModifiedDate = a.ModifiedDate,
                       Name = a.Name,
                       Phone = a.Phone,
                       Status = a.Status
                   };
        }

        public IQueryable<LichSuGiaHanCuaHang> GetBySubDomain(string subdomain)
        {
            return _LichSuGiaHanCuaHang.Filter(o => o.SubDomain.Equals(subdomain));

        }

        public IQueryable<CuaHangNapTienDichVu> GetAllCuaHangNapTien
        {
            get {
                return _CuaHangNapTienDichVu.All().OrderByDescending(o=>o.NgayTao).AsQueryable(); 
            }
        }

        public IQueryable<DM_GoiDichVu> GetAllGoiDichVu()
        {
            return unitOfWork.GetRepository<DM_GoiDichVu>().All();

        }

        public IQueryable<DichVuNapTien> SearchDichvuNaptien(string text)
        {
            var data = from o in _CuaHangNapTienDichVu.All()
                       join b in _CuahangService.All()
                       on o.SoDienThoaiCuaHang equals b.SoDienThoai
                       orderby o.NgayTao
                       where o.TrangThai != (int)Notification.DichVuNapTien.xoa 
                       select  new DichVuNapTien
                       {
                           GhiChu=o.GhiChu,
                           TenKhachNap=o.TenKhachHang,
                           SoDienThoai=b.SoDienThoai,
                           ID=o.ID,
                           NgayTao=o.NgayTao,
                           SoTien=o.SoTien,
                           SubDoamin=b.SubDomain,
                           TrangThai=o.TrangThai,
                           TenCuaHang=b.TenCuaHang
                       };
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToUpper();
                data =data.Where(o => o.SoDienThoai.Contains(text)
                                       || o.TenKhachNap.ToUpper().Contains(text));
            }
            return data;
        }

        public bool InsertGoiDichVu(DM_GoiDichVu model)
        {
            if(_DM_GoiDichVu.All().Any(o=>o.TenGoi.ToUpper().Equals(model.TenGoi.ToUpper())))
            {
                return false;
            }
            var keyId = "GDV_" + StaticVariable.GetCharInput(model.TenGoi.ToUpper());
            var count = _DM_GoiDichVu.Filter(o => o.ID.Contains(keyId)).Count();
            if (count == 0)
            {
                model.ID = keyId;
            }
            else
            {
                model.ID = keyId+"_"+count;
            }
            _DM_GoiDichVu.Create(model);
            unitOfWork.Save();
            return true;
        }

        public string GetSubdomainForDomain(string domain)
        {
            var result = _CuahangService.Filter(o => o.Domain!=null&& o.Domain.ToLower() == domain.ToLower()).FirstOrDefault();
            if (result == null)
                return string.Empty;
            return result.SubDomainWeb;
        }

        public string getNoteHistory(string subdomain)
        {
           var data= _LichSuGiaHanCuaHang.Filter(o => o.SubDomain.Equals(subdomain)).OrderByDescending(o=>o.NgayTao).FirstOrDefault();
            return data != null ? data.GhiChu : string.Empty;
        }

        public JsonViewModel<string> UpdateGoiDichVu(DM_GoiDichVu model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.error };
            var data = _DM_GoiDichVu.Find(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Không lấy được thông tin cần cập nhật";
            }
             else  if (_DM_GoiDichVu.All().Any(o => o.ID != model.ID && o.TenGoi.ToUpper().Equals(model.TenGoi.ToUpper())))
            {
                result.Data = "Tên gói dịch vụ đã tồn tại";
            }
            else
            {
                data.TenGoi = model.TenGoi;
                data.SLChiNhanh = model.SLChiNhanh;
                data.SLMatHang = model.SLMatHang;
                data.SLNguoiDung = model.SLNguoiDung;
                data.Gia = model.Gia;
                data.GhiChu = model.GhiChu;
                data.TrangThai = model.TrangThai;
                unitOfWork.Save();
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;
        }
        public void RemoveDichvuNaptien(Guid ID)
        {
            var model = _CuaHangNapTienDichVu.Find(o => o.ID.Equals(ID));
            if (model != null)
            {
                model.TrangThai = (int)Notification.DichVuNapTien.xoa;
                unitOfWork.Save();
            }
        }
        public void InsertDichvuNaptien(CuaHangNapTienDichVu model)
        {
            if (string.IsNullOrWhiteSpace(model.TenKhachHang))
            {
                model.TenKhachHang = model.SoDienThoaiCuaHang;
            }
            _CuaHangNapTienDichVu.Create(model);
            unitOfWork.Save();
        }
        public JsonViewModel<object> UpdateDichvuNaptien(CuaHangNapTienDichVu model)
        {
            var result = new JsonViewModel<object>() { ErrorCode=(int)Notification.ErrorCode.error};
            var data = _CuaHangNapTienDichVu.Find(o => o.ID == model.ID);
            if(data==null)
            {
                return result;
            }
            if (!string.IsNullOrWhiteSpace(model.TenKhachHang))
            {
                data.TenKhachHang = model.TenKhachHang;
            }
            data.GhiChu = model.GhiChu;
            data.SoTien = model.SoTien;
            data.TrangThai = model.TrangThai;
            data.NgayXacNhan = DateTime.Now;
            unitOfWork.Save();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            var sub = _CuahangService.Find(o => o.SoDienThoai.Equals(data.SoDienThoaiCuaHang));
            result.Data = new {
                ID_PhieuNhan =data.ID_PhieuNhan,
                TrangThai=data.TrangThai,
                Subdomain= sub!=null?sub.SubDomain:string.Empty
            };
            return result;
        }

        public JsonViewModel<string> ChangeStatusContract(Contract model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            var data = _Contract.Filter(o => o.ID == model.ID).FirstOrDefault();
            if (data == null)
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Bản ghi không tồn tại hoặc đã bị xóa, vui lòng tải lại sau";
            }
            else
            {
                data.Status = model.Status;
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = model.ModifiedBy;
                unitOfWork.Save();
            }
            return result;
        }
        public int GetCountForMonth(int year,int month)
        {
           return _CuahangService.Filter(o => o.NgayTao != null && o.NgayTao.Value.Year == year && o.NgayTao.Value.Month == month).Count() ;
        }

        public int GetCountForStage(DateTime tuthang, DateTime denthang)
        {
            var data = _CuahangService.Filter(o => o.NgayTao != null
                                                 && ((o.NgayTao.Value.Day >= tuthang.Day
                                                 && o.NgayTao.Value.Month == tuthang.Month
                                                 && o.NgayTao.Value.Year == tuthang.Year)
                                                 || (o.NgayTao.Value.Month > tuthang.Month
                                                    && o.NgayTao.Value.Year == tuthang.Year)
                                                     || o.NgayTao.Value.Year > tuthang.Year)
                                                );
            return data.Where(o => (o.NgayTao.Value.Month < denthang.Month
                                    && o.NgayTao.Value.Year == denthang.Year)
                                     || (o.NgayTao.Value.Year == denthang.Year
                                        && o.NgayTao.Value.Month == denthang.Month
                                        && o.NgayTao.Value.Day <= denthang.Day)
                                    || o.NgayTao.Value.Year < denthang.Year).Count();
        }

        public long GetCountStoreagrea(DateTime date)
        {
            return _CuahangService.Filter(o => o.NgayTao != null
                                                && (o.NgayTao.Value.Day == date.Day
                                                && o.NgayTao.Value.Month == date.Month
                                                && o.NgayTao.Value.Year == date.Year)).Count();
        }

        public int GetCountForQuy(int year, int quy)
        {
            if(quy==1)
            {
                return _CuahangService.Filter(o => o.NgayTao != null 
                                                && o.NgayTao.Value.Year == year 
                                                && o.NgayTao.Value.Month >=1
                                                && o.NgayTao.Value.Month<=3).Count();
            }
            else if(quy==2)
            {
                return _CuahangService.Filter(o => o.NgayTao != null
                                               && o.NgayTao.Value.Year == year
                                               && o.NgayTao.Value.Month >= 4
                                               && o.NgayTao.Value.Month <= 6).Count();
            }
            else if (quy == 3)
            {
                return _CuahangService.Filter(o => o.NgayTao != null
                                               && o.NgayTao.Value.Year == year
                                               && o.NgayTao.Value.Month >= 7
                                               && o.NgayTao.Value.Month <= 9).Count();
            }
            else
            {
                return _CuahangService.Filter(o => o.NgayTao != null
                                               && o.NgayTao.Value.Year == year
                                               && o.NgayTao.Value.Month >= 10
                                               && o.NgayTao.Value.Month <= 12).Count();
            }
               
        }

        public int GetCountForYear(int year)
        {
            return _CuahangService.Filter(o => o.NgayTao != null 
                                            && o.NgayTao.Value.Year == year ).Count();
        }
        

        public JsonViewModel<string> SaveContract(Contract model)
        {
            var result = new JsonViewModel<string>() { ErrorCode=(int)Notification.ErrorCode.success};
            var subdomain = model.StoreOpen.Split('.')[0].ToUpper();
            if (!_CuahangService.Filter(o=>o.SubDomain.ToUpper().Equals(subdomain)).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Tên gian hàng không tồn tại";
            }
            else
            {
                model.Status = (int)Notification.StatusContract.TaoMoi;
                model.StoreOpen = model.StoreOpen.Split('.')[0];
                model.CreatedDate = DateTime.Now;
                _Contract.Create(model);
                unitOfWork.Save();
            }
            return result;
        }

        public object GetCharWeeks(int take)
        {
            var dateNow = DateTime.Now.AddDays(-1);
            var dateStat = DateTime.Now.AddDays((-take));
            DateTime endDate = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 23, 59, 59);
            DateTime toDate  = new DateTime(dateStat.Year, dateStat.Month, dateStat.Day, 0, 0, 01);
            var data = _CuahangService.Filter(o => o.NgayTao != null
                                                && o.NgayTao.Value>=toDate
                                                && o.NgayTao.Value<=endDate).Select(o=>o.NgayTao).AsEnumerable();
            var result = new List<Chart>();
            for(int i=0;i<take;i++)
            {
                var key = dateStat.AddDays(i);
                var model = data.Where(o => o.Value.Year == key.Year
                                            &&o.Value.Month==key.Month
                                            &&o.Value.Day==key.Day);
                if(model.Any())
                {
                    result.Add(new Chart { label = key.ToString("dd-MM-yyyy"), y = model.Count() });
                }
                else
                {
                    result.Add(new Chart { label = key.ToString("dd-MM-yyyy"), y = 0 });
                }

            }
            return result;
        }

        public object GetChartCity()
        {
            var data = _CuahangService.Filter(o => o.TinhThanh_QuanHuyen != null).Select(o => o.TinhThanh_QuanHuyen.TinhThanh).AsEnumerable();
            var result = data.GroupBy(o => o).Select(o=>new Chart {label=o.Key,y=o.Count() });
            return result;
        }

        public IQueryable<CuaHangDangKy> Search(string text, int? typeHsd, bool? status,int? Version)
        {
            var data = Query;
            var datenow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01);
            if (typeHsd == (int)Notification.expiryDate.conhan)
            {

                data = data.Where(o => o.HanSuDung.HasValue
                                    && (o.HanSuDung.Value>= datenow));
            }
            else if (typeHsd == (int)Notification.expiryDate.hethan)
            {

                data = data.Where(o => o.HanSuDung.HasValue
                                    && (o.HanSuDung.Value< datenow));
            }

            if (status.HasValue)
            {
                data = data.Where(o => o.TrangThai == status);

            }
            if (Version != null && Version != -1)
            {
                data = data.Where(o => o.version == Version);
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = StringExtensions.ConvertToUnSign(text.Trim());
                data = data.AsEnumerable().Where(o => StringExtensions.ConvertToUnSign(o.SoDienThoai).Contains(text)
                                      || StringExtensions.ConvertToUnSign(o.HoTen).Contains(text)
                                      || StringExtensions.ConvertToUnSign(o.SubDomain).Contains(text)
                                      || (o.NganhNgheKinhDoanh != null
                                         && StringExtensions.ConvertToUnSign(o.NganhNgheKinhDoanh.TenNganhNghe).Contains(text))).AsQueryable();
            }
            return data.AsQueryable();
        }

        public CuaHangDangKy SelectBySubdomain(string subdomain)
        {
            return _CuahangService.Filter(p => p.SubDomain.ToLower() == subdomain.ToLower()).FirstOrDefault();
        }

        public bool checkPhoneExist(string phone)
        {
            if (!string.IsNullOrWhiteSpace(phone))
            {
                return _CuahangService.Filter(o => o.SoDienThoai.ToLower().Equals(phone.Trim().ToLower()) && o.version != (int)Notification.VersionStore.chuadangky).Any();
            }
            return true;
        }

        public bool checkSubdomainExist(string subdomain)
        {
            if (!string.IsNullOrWhiteSpace(subdomain))
            {
                return _CuahangService.Filter(o => o.SubDomain.ToLower().Equals(subdomain.Trim().ToLower()) && o.version != (int)Notification.VersionStore.chuadangky).Any();
            }
            return true;
        }

        public bool checkEmailExist(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return _CuahangService.Filter(o => o.Email.ToLower().Equals(email.Trim().ToLower()) && o.version!=(int)Notification.VersionStore.chuadangky).Any();
            }
            return false;
        }

        public bool InsertLichSuGiaHan(string sodienthoai, string DienThoaiNhanVien, string TenNhanVien)
        {
            try
            {
                var data = _CuahangService.Find(sodienthoai);
                var lichsu = new LichSuGiaHanCuaHang()
                {
                    GhiChu = DienThoaiNhanVien + " - " + TenNhanVien,
                    LoaiGiaHan = 0,
                    NgayTao = DateTime.Now,
                    NguoiTao = DienThoaiNhanVien,
                    SubDomain = data.SubDomain,
                    ID_GoiDichVu = data.ID_GoiDichVu ?? KeyGoiDichVu.TietKiem,
                    GiaGoiHienTai = data.DM_GoiDichVu != null ? data.DM_GoiDichVu.Gia : 0
                };
                _LichSuGiaHanCuaHang.Create(lichsu);
                unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public JsonViewModel<string> UpdateStore(StoreRegistrationView model,string user)
        {
            var data = _CuahangService.Find(model.Mobile);
            var result = new JsonViewModel<string>
            {
                ErrorCode = (int)Notification.ErrorCode.success,
                Data = ""
            };
            if (data == null)
            {
                result.ErrorCode = (int)Notification.ErrorCode.notfound;
                result.Data = "Không tìm thấy cửa hàng cần cập nhật, vui lòng thử lại sau.";
            }
            else
            {
                var lichsu = new LichSuGiaHanCuaHang()
                {
                    GhiChu = model.GhiChu,
                    LoaiGiaHan = model.Version,
                    NgayTao=DateTime.Now,
                    NguoiTao=user,
                    SubDomain=data.SubDomain,
                    ID_GoiDichVu=data.ID_GoiDichVu??KeyGoiDichVu.TietKiem,
                    GiaGoiHienTai=data.DM_GoiDichVu!=null?data.DM_GoiDichVu.Gia:0
                };
                if (!string.IsNullOrWhiteSpace(model.ThoGianGiaHan))
                {
                    var obj = model.ThoGianGiaHan.Split('_');
                    if (obj.Length == 3) {
                        if (!string.IsNullOrWhiteSpace(obj[0]) && obj[0].ToString() != "0")
                        {
                            lichsu.ThoiGianGiaHan = obj[0] + " năm ";
                        }
                        if (!string.IsNullOrWhiteSpace(obj[1]) && obj[1].ToString() != "0")
                        {
                            lichsu.ThoiGianGiaHan += obj[1] + " tháng ";
                        }
                        if (!string.IsNullOrWhiteSpace(obj[2]) && obj[2].ToString() != "0")
                        {
                            lichsu.ThoiGianGiaHan += obj[2] + " ngày ";
                        }
                    }
                }
                _LichSuGiaHanCuaHang.Create(lichsu);
                data.HanSuDung = model.ExpiryDate;
                data.ID_GoiDichVu = model.ID_GoiDichVu;
                data.version = model.Version;
                data.TrangThai = model.Status;
                unitOfWork.Save();
            }
            return result;
        }

        #region tỉnh thành quận huyện
        public static IQueryable<TinhThanh_QuanHuyen> GetsTinhThanhQuanHuyen(Expression<Func<TinhThanh_QuanHuyen, bool>> query)
        {
            BanHang24vnContext dbcontext = new BanHang24vnContext();
            if (query == null)
                return dbcontext.TinhThanh_QuanHuyen;
            else
                return dbcontext.TinhThanh_QuanHuyen.Where(query);
        }

        public static bool CheckPhoneNumber(string phone)
        {
            try
            {
                BanHang24vnContext dbcontext = new BanHang24vnContext();
                CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p => p.SoDienThoai == phone).FirstOrDefault();
                if (chdk != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool CheckSubdomain(string sub)
        {
            try
            {
                BanHang24vnContext dbcontext = new BanHang24vnContext();
                CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p => p.SubDomain.ToLower() == sub).FirstOrDefault();
                if (chdk != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool CheckEmail(string email)
        {
            try
            {
                BanHang24vnContext dbcontext = new BanHang24vnContext();
                CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p => p.Email.ToLower() == email).FirstOrDefault();
                if (chdk != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static CuaHangDangKy CheckEmailReturnString(string subdomain, string email)
        {
            try
            {
                BanHang24vnContext dbcontext = new BanHang24vnContext();
                CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p=>p.SubDomain.ToLower().Trim() == subdomain).Where(p => p.Email.ToLower().Trim() == email).FirstOrDefault();
                if (chdk != null)
                {
                    return chdk;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static CuaHangDangKy Get(string subdomain)
        {
            try
            {
                BanHang24vnContext dbcontext = new BanHang24vnContext();
                CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p => p.SubDomain.ToLower().Trim() == subdomain).FirstOrDefault();
                if (chdk != null)
                {
                    return chdk;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static int GetGioiHanMatHang(string subdomain)
        {
            try
            {
                BanHang24vnContext dbcontext = new BanHang24vnContext();
                CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p => p.SubDomain.ToLower().Trim() == subdomain).FirstOrDefault();
                if (chdk != null)
                {
                    return dbcontext.DM_GoiDichVu.Where(p => p.ID == chdk.ID_GoiDichVu).Select(p => p.SLMatHang).FirstOrDefault();
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static int GetGioiHanChiNhanh(string subdomain)
        {
            try
            {
                BanHang24vnContext dbcontext = new BanHang24vnContext();
                CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p => p.SubDomain.ToLower().Trim() == subdomain).FirstOrDefault();
                if (chdk != null)
                {
                    return dbcontext.DM_GoiDichVu.Where(p => p.ID == chdk.ID_GoiDichVu).Select(p => p.SLChiNhanh).FirstOrDefault();
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static int GetGioiHanNguoiDung(string subdomain)
        {
            try
            {
                BanHang24vnContext dbcontext = new BanHang24vnContext();
                CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p => p.SubDomain.ToLower().Trim() == subdomain).FirstOrDefault();
                if (chdk != null)
                {
                    return dbcontext.DM_GoiDichVu.Where(p => p.ID == chdk.ID_GoiDichVu).Select(p => p.SLNguoiDung).FirstOrDefault();
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static string GetAddress(string sodienthoai)
        {
            BanHang24vnContext dbcontext = new BanHang24vnContext();
            CuaHangDangKy chdk = dbcontext.CuaHangDangKies.Where(p => p.SoDienThoai == sodienthoai).FirstOrDefault();
            if (chdk != null)
            {
                if (chdk.ID_TinhThanh_QuanHuyen != null)
                {
                    TinhThanh_QuanHuyen ttqh = dbcontext.TinhThanh_QuanHuyen.Where(p => p.ID == chdk.ID_TinhThanh_QuanHuyen).FirstOrDefault();
                    return ttqh.TinhThanh + " - " + ttqh.QuanHuyen;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public bool UpdateCreateDatabase(string subdomain)
        {
            try
            {
                var model = Query.FirstOrDefault(o => o.SubDomain.ToUpper().Equals(subdomain.ToUpper()));
                if (model != null && model.IsCreateDatabase == false)
                {
                    model.IsCreateDatabase = true;
                    unitOfWork.Save();
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }
        #endregion
    }
}
