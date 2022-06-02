using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;
using Riss.Devices;
using System.Data.SqlClient;
using System.Net.Http.Headers;

namespace libNS_NhanVien
{
    public class MayChamCongServices
    {
        private SsoftvnContext _dbcontext;
        public MayChamCongServices(SsoftvnContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public JsonViewModel<string> InsertMayChamCong(NS_MayChamCong model, Guid ID_DonVi, Guid idNhanVien)
        {
            var resul = new JsonViewModel<string> { ErrorCode = false };
            try
            {
                if (_dbcontext.NS_MayChamCong.Any(p => p.SoSeries == model.SoSeries))
                {
                    resul.Data = "Số series máy chấm công đã được đăng ký.";
                }
                else
                {
                    _dbcontext.NS_MayChamCong.Add(model);
                    string noidung = "Thêm mới máy chấm công: " + model.SoSeries;
                    string chitiet = "Thêm mới máy chấm công " + model.MaMCC + " - " + model.TenMCC + " - " + model.TenHienThi;
                    InsertNhatKySuDung("Máy chấm công", noidung, chitiet, ID_DonVi, idNhanVien, 1);
                    _dbcontext.SaveChanges();
                    resul.ErrorCode = true;
                }
            }
            catch (Exception ex)
            {
                resul.Data = ex.Message;
            }
            return resul;
        }

        public JsonViewModel<string> UpdateMayChamCong(NS_MayChamCong model, Guid ID_DonVi, Guid idNhanVien)
        {
            var resul = new JsonViewModel<string> { ErrorCode = false };
            try
            {
                NS_MayChamCong mcc = _dbcontext.NS_MayChamCong.Where(p=> p.ID == model.ID).FirstOrDefault();
                if (mcc != null)
                {
                    mcc.TenMCC = model.TenMCC;
                    mcc.TenHienThi = model.TenHienThi;
                    mcc.ID_ChiNhanh = model.ID_ChiNhanh;
                    mcc.LoaiKetNoi = model.LoaiKetNoi;
                    mcc.IP = model.IP;
                    mcc.Port = model.Port;
                    mcc.SoSeries = model.SoSeries;
                    mcc.MatMa = model.MatMa;
                    mcc.SoDangKy = model.SoDangKy;
                    mcc.LoaiHinh = model.LoaiHinh;
                    string noidung = "Cập nhật máy chấm công: " + model.SoSeries;
                    string chitiet = "Cập nhật máy chấm công " + model.MaMCC + " - " + model.TenMCC + " - " + model.TenHienThi;
                    InsertNhatKySuDung("Máy chấm công", noidung, chitiet, ID_DonVi, idNhanVien, 2);
                    _dbcontext.SaveChanges();
                    resul.ErrorCode = true;
                }
                else
                {
                    resul.Data = "Máy chấm công không tồn tại hoặc đã bị xóa";
                }    
            }
            catch (Exception ex)
            {
                resul.Data = ex.Message;
            }
            return resul;
        }

        public JsonViewModel<string> TaiDuLieuMayChamCong(Guid IDMCC, DateTime TimeStart, DateTime TimeEnd)
        {
            JsonViewModel<string> result = new JsonViewModel<string> { ErrorCode = false };
            try
            {
                NS_MayChamCong mcc = _dbcontext.NS_MayChamCong.Where(p => p.ID == IDMCC).FirstOrDefault();
                if (mcc != null)
                {
                    List<Record> recordList = new List<Record>();
                    Riss.Devices.Device device = new Device();
                    device.IpAddress = mcc.IP;
                    device.IpPort = mcc.Port;
                    device.SerialNumber = mcc.SoSeries;
                    try
                    {
                        device.DN = int.Parse(mcc.SoDangKy);
                    }
                    catch
                    {
                        device.DN = 1;
                    }
                    if (mcc.LoaiKetNoi == 1)
                    {
                        device.CommunicationType = CommunicationType.Tcp;
                    }
                    else if (mcc.LoaiKetNoi == 2)
                    {
                        device.CommunicationType = CommunicationType.P2P;
                    }
                    else
                    {
                        device.CommunicationType = CommunicationType.Tcp;
                    }    
                    device.Password = mcc.MatMa.ToString();
                    device.Model = mcc.TenMCC;
                    device.ConnectionModel = 5;
                    DeviceConnection connection = Riss.Devices.DeviceConnection.CreateConnection(ref device);
                    if (connection.Open() > 0)
                    {
                        int year = DateTime.Now.Year;
                        List<DateTime> dtList = new List<DateTime>();
                        //DateTime firstDay = TimeStart;
                        //DateTime lastDay = TimeEnd;
                        dtList.Add(TimeStart);
                        dtList.Add(TimeEnd);
                        object extraProperty = new object();
                        object extraData = new object();
                        List<bool> boolList = new List<bool>();
                        boolList.Add(false);
                        boolList.Add(false);// remove new log mark.
                        extraProperty = boolList;
                        extraData = dtList;// the beginning date, when get the new log, need to set this parameter, but the parameter has no effect.
                        bool connectionResult = connection.GetProperty(DeviceProperty.AttRecords, extraProperty, ref device, ref extraData);
                        if (connectionResult)
                        {
                            recordList = (List<Record>)extraData;
                        }
                    }
                    connection.Close();
                }
                else
                {
                    result.Data = "Máy chấm công không tồn tại hoặc đã xóa.";
                }    
            }
            catch (Exception ex)
            {
                result.Data = ex.Message;
            }
            return result;
        }

        public List<NS_MayChamCong> GetAll()
        {
            if (_dbcontext == null)
            {
                return null;
            }
            else
            {
                return _dbcontext.NS_MayChamCong.ToList();
            }
        }

        public NS_MayChamCong GetByIdMayChamCong(Guid IdMcc)
        {
            if (_dbcontext == null)
            {
                return null;
            }
            else
            {
                return _dbcontext.NS_MayChamCong.Where(p=>p.ID == IdMcc).FirstOrDefault();
            }
        }

        public List<ObjGetDuLieuCongThoTheoThang> GetDuLieuCongThoTheoThang(Guid IDMayChamCong, int Month, int Year, int PageSize, int PageNum)
        {
            if (_dbcontext == null)
            {
                return null;
            }
            else
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("IDMayChamCong", IDMayChamCong));
                sql.Add(new SqlParameter("InMonth", Month));
                sql.Add(new SqlParameter("InYear", Year));
                sql.Add(new SqlParameter("PageSize", PageSize));
                sql.Add(new SqlParameter("PageNum", PageNum));
                return _dbcontext.Database.SqlQuery<ObjGetDuLieuCongThoTheoThang>("exec GetDuLieuCongThoTheoThang @IDMayChamCong, @InMonth, @InYear, @PageSize, @PageNum", sql.ToArray()).ToList();
            }
        }

        public List<GridMayChamCong> GetByListIDChiNhanh(List<string> IdChiNhanhs)
        {
            if (_dbcontext == null)
            {
                return null;
            }
            else
            {
                string sqlIdChiNhanh = "";
                if(IdChiNhanhs != null)
                {
                    sqlIdChiNhanh = string.Join(",", IdChiNhanhs);
                }    

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("ListIDChiNhanh", sqlIdChiNhanh));
                return _dbcontext.Database.SqlQuery<GridMayChamCong>("exec GetDanhSachMayChamCongTheoChiNhanh @ListIDChiNhanh", sql.ToArray()).ToList();
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

        public bool TaiDuLieuFromDevice(NS_MayChamCong mcc, DateTime firstDay, DateTime lastDay, HT_NguoiDung nguoidung, ref string mess)
        {
            CookieStore.WriteProgress("Đang kết nối máy chấm công...", "TaiDuLieuMayChamCong");
            Device device = new Device();
            DeviceConnection connection = DeviceConnection.CreateConnection(ref device);
            bool ConnectionStatus = ConnectMayChamCong(mcc.IP, mcc.SoSeries, mcc.MatMa, mcc.TenMCC, int.Parse(mcc.SoDangKy), mcc.Port, ref connection, ref device);
            if (ConnectionStatus)
            {
                CookieStore.WriteProgress("Đang tải dữ liệu...", "TaiDuLieuMayChamCong");
                List<Record> recordList = new List<Record>();
                int year = DateTime.Now.Year;
                List<DateTime> dtList = new List<DateTime>();
                dtList.Add(firstDay);
                dtList.Add(lastDay);
                object extraProperty = new object();
                object extraData = new object();
                List<bool> boolList = new List<bool>();
                boolList.Add(false);
                boolList.Add(false);// remove new log mark.
                extraProperty = boolList;
                extraData = dtList;// the beginning date, when get the new log, need to set this parameter, but the parameter has no effect.
                bool result = connection.GetProperty(DeviceProperty.AttRecords, extraProperty, ref device, ref extraData);
                if (result)
                {
                    recordList = (List<Record>)extraData;
                    List<ObjCongTho> lstCongThoDeivce = recordList.Select(p => new ObjCongTho
                    {
                        MaChamCong = p.DIN.ToString(),
                        ThoiGian = p.Clock,
                        ID_MCC = mcc.ID,
                        VaoRa = p.DoorStatus,
                        TrangThai = 1
                    }).ToList();

                    List<ObjCongTho> lstCongTho = _dbcontext.NS_DuLieuCongTho.Where(p => p.ID_MCC == mcc.ID && p.ThoiGian >= firstDay && p.ThoiGian <= lastDay)
                        .Select(p => new ObjCongTho
                        {
                            MaChamCong = p.MaChamCong,
                            ThoiGian = p.ThoiGian,
                            ID_MCC = p.ID_MCC,
                            VaoRa = p.VaoRa,
                            TrangThai = p.TrangThai
                        }).ToList();
                    DateTime dateTimeNow = DateTime.Now;

                    List<NS_DuLieuCongTho> lstCongThoNew = lstCongThoDeivce.Where(p => !lstCongTho.Any(x => x.MaChamCong == p.MaChamCong && x.ThoiGian == p.ThoiGian
                     && x.ID_MCC == p.ID_MCC && x.VaoRa == p.VaoRa && x.TrangThai == p.TrangThai)).Select(p => new NS_DuLieuCongTho
                     {
                         ID = Guid.NewGuid(),
                         MaChamCong = p.MaChamCong,
                         ThoiGian = p.ThoiGian,
                         ID_MCC = p.ID_MCC,
                         VaoRa = p.VaoRa,
                         TrangThai = p.TrangThai,
                         NguoiTao = nguoidung.TaiKhoan,
                         NgayTao = dateTimeNow
                     }).ToList();
                    if (lstCongThoNew.Count > 0)
                    {
                        _dbcontext.NS_DuLieuCongTho.AddRange(lstCongThoNew);
                        _dbcontext.SaveChanges();
                    }
                    string noidung = "Tải dữ liệu máy chấm công: " + mcc.TenHienThi;
                    string chitiet = "Tải dữ liệu từ máy chấm công " + mcc.MaMCC + " - " + mcc.TenMCC + " - " + mcc.TenHienThi;
                    InsertNhatKySuDung("Máy chấm công", noidung, chitiet, mcc.ID_ChiNhanh, nguoidung.ID_NhanVien.Value, 1);
                    mess = "Tải dữ liệu thành công.";
                    return true;
                }
                else
                {
                    mess = "Có lỗi xảy ra trong quá trình tải dữ liệu máy chấm công. Vui lòng kiểm tra lại kết nối máy chấm công.";
                    return false;
                }    
            }
            else
            {
                mess = "Kết nối máy chấm công thất bại.";
                return false;
            }
        }

        public bool ConnectMayChamCong(string IpDomain, string SerialNumber, string Password, string Model, int DN, int port, ref DeviceConnection connection, ref Device device)
        {
            device.IpAddress = IpDomain;
            device.IpPort = port;
            device.SerialNumber = SerialNumber;
            device.DN = DN;
            device.CommunicationType = CommunicationType.Tcp;
            device.Password = Password;
            device.Model = Model;
            device.ConnectionModel = 5;
            connection = Riss.Devices.DeviceConnection.CreateConnection(ref device);
            if(connection.Open() > 1)
            {
                CookieStore.WriteProgress("Kết nối thành công máy chấm công " + IpDomain + ":" + Model, "TaiDuLieuMayChamCong");
                return true;
            }    
            else
            {
                CookieStore.WriteProgress("Không thể kết nối máy chấm công " + IpDomain + ":" + Model, "TaiDuLieuMayChamCong");
                return false;
            }
        }
    }
}
