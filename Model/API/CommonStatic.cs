using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace Model
{
    public class CommonStatic
    {
        public const string CONNECT_ERROR = "Kết nối databse lỗi";

        public static DateTime AddTimeNow_forDate(DateTime dt)
        {
            //var dtParam = dt;
            // 2022-11-24 11:02:37.787 --> 37.787 --> 37 and 787
            string dtNow = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            var stSplit = dtNow.Split(':');
            var last = stSplit[stSplit.Length - 1].Split('.');
            var senconds = Convert.ToDouble(last[0]);
            var milisenconds = Convert.ToDouble(last[1]);
            dt = dt.AddSeconds(senconds).AddMilliseconds(milisenconds);
            return dt;
        }

        public static string GetCharsStart(string stInput)
        {
            string sReturn = string.Empty;
            if (!string.IsNullOrWhiteSpace(stInput) && stInput != null)
            {
                Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
                string strFormD = stInput.Normalize(System.Text.NormalizationForm.FormD);
                strFormD = regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Trim();

                var arrStr = System.Text.RegularExpressions.Regex.Split(strFormD, @"\s+");
                for (int i = 0; i < arrStr.Length; i++)
                {
                    sReturn += arrStr[i][0];
                }
            }
            return sReturn;
        }
        /// <summary>
        /// Dãy tiếng việt có dấu cần chuyển đổi
        /// </summary>
        public static readonly string[] VietnameseSigns = new string[]

      { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
        "đ",
        "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
        "í","ì","ỉ","ĩ","ị",
        "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
        "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
        "ý","ỳ","ỷ","ỹ","ỵ",};

        /// <summary>
        /// Dãy tiếng việt ko dấu cần chuyển đổi
        /// </summary>
        public static readonly string[] EnglishSigns = new string[]
        { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};

        /// <summary>
        /// chuyển tiếng việt có dấu sang không dấu
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSign4VietnameseString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            for (int i = 0; i < VietnameseSigns.Length; i++)
            {
                str = str.Replace(VietnameseSigns[i], EnglishSigns[i]);
                str = str.Replace(VietnameseSigns[i].ToUpper(), EnglishSigns[i].ToUpper());
            }
            return str;

        }
        public static string convertchartstart(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            char[] whitespace = new char[] { ' ', '\t' };
            return string.Join("", RemoveSign4VietnameseString(input.Normalize(NormalizationForm.FormC).ToLower()).Split(whitespace).Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Substring(0, 1)));

        }

        public static string ConvertToUnSign(string text)
        {
            if (text != null && !string.IsNullOrWhiteSpace(text))
            {
                text = text.Trim();
                for (int i = 33; i < 48; i++)
                {
                    if (i != 45)
                    {
                        text = text.Replace(((char)i).ToString(), "");
                    }
                }

                for (int i = 58; i < 65; i++)
                {
                    text = text.Replace(((char)i).ToString(), "");
                }

                for (int i = 91; i < 97; i++)
                {
                    text = text.Replace(((char)i).ToString(), "");
                }
                for (int i = 123; i < 127; i++)
                {
                    text = text.Replace(((char)i).ToString(), "");
                }
                //text = text.Replace(" ", "-");
                Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
                string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);
                return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            else
            {
                return "";
            }
        }

        public static string GetTempPath()
        {
            var path = Path.GetTempPath();
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// remove douple, tripler comma
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Remove_LastComma(string str)
        {
            if (str != null && str.Length > 1)
            {
                return Regex.Replace(str, @"/(^[,\s]+)|([,\s]+$)/g", "");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        ///  Đưa ra bộ lọc từ ngày đến ngày với điều kiện lọc dateime trên view
        /// </summary>
        /// <param name="TypeTime"></param>
        /// <param name="TuNgay"></param>
        /// <param name="DenNgay"></param>
        /// <param name="startday"></param>
        /// <param name="Endday"></param>
        /// <returns></returns>
        public static bool CheckTimeFilter(int? TypeTime, DateTime? TuNgay, DateTime? DenNgay, ref DateTime startday, ref DateTime Endday)
        {
            var date = DateTime.Now;
            bool IsfilterTime = true;
            if (TypeTime != null)
            {
                switch (TypeTime)
                {
                    case (int)commonEnumHellper.TypeBirthDay.homnay:
                        break;
                    case (int)commonEnumHellper.TypeBirthDay.homqua:
                        startday = startday.AddDays(-1);
                        Endday = Endday.AddDays(-1);
                        break;
                    case (int)commonEnumHellper.TypeBirthDay.namnay:
                        startday = new DateTime(DateTime.Now.Year, 1, 1, 00, 00, 00);
                        Endday = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeBirthDay.namtruoc:
                        date = date.AddYears(-1);
                        startday = new DateTime(date.Year, 1, 1, 00, 00, 00);
                        Endday = new DateTime(date.Year, 12, 31, 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeBirthDay.quynay:
                        if (date.Month <= 3)
                        {
                            startday = new DateTime(DateTime.Now.Year, 1, 1, 00, 00, 00);
                            Endday = new DateTime(DateTime.Now.Year, 3, 31, 23, 59, 59);
                        }
                        else if (date.Month <= 6)
                        {
                            startday = new DateTime(DateTime.Now.Year, 4, 1, 00, 00, 00);
                            Endday = new DateTime(DateTime.Now.Year, 6, 30, 23, 59, 59);
                        }
                        else if (date.Month <= 9)
                        {
                            startday = new DateTime(DateTime.Now.Year, 7, 1, 00, 00, 00);
                            Endday = new DateTime(DateTime.Now.Year, 9, 30, 23, 59, 59);
                        }
                        else
                        {
                            startday = new DateTime(DateTime.Now.Year, 10, 1, 00, 00, 00);
                            Endday = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);

                        }

                        break;
                    case (int)commonEnumHellper.TypeBirthDay.quytruoc:
                        if (date.Month <= 3)
                        {
                            date = date.AddYears(-1);
                            startday = new DateTime(date.Year, 10, 1, 00, 00, 00);
                            Endday = new DateTime(date.Year, 12, 31, 23, 59, 59);
                        }
                        else if (date.Month <= 6)
                        {
                            startday = new DateTime(DateTime.Now.Year, 1, 1, 00, 00, 00);
                            Endday = new DateTime(DateTime.Now.Year, 3, 31, 23, 59, 59);
                        }
                        else if (date.Month <= 9)
                        {
                            startday = new DateTime(DateTime.Now.Year, 4, 1, 00, 00, 00);
                            Endday = new DateTime(DateTime.Now.Year, 6, 30, 23, 59, 59);
                        }
                        else
                        {
                            startday = new DateTime(DateTime.Now.Year, 7, 1, 00, 00, 00);
                            Endday = new DateTime(DateTime.Now.Year, 9, 30, 23, 59, 59);

                        }
                        break;
                    case (int)commonEnumHellper.TypeBirthDay.thangnay:
                        startday = new DateTime(date.Year, date.Month, 1, 00, 00, 00);
                        Endday = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeBirthDay.thangtruoc:
                        date = date.AddMonths(-1);
                        startday = new DateTime(date.Year, date.Month, 1, 00, 00, 00);
                        Endday = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeBirthDay.tuannay:
                        Endday = Endday.AddDays(7 - (int)DateTime.Now.DayOfWeek);
                        startday = startday.AddDays(-((int)DateTime.Now.DayOfWeek - 1));
                        break;
                    case (int)commonEnumHellper.TypeBirthDay.tuantruoc:
                        Endday = Endday.AddDays(7 - (int)DateTime.Now.DayOfWeek).AddDays(-7);
                        startday = startday.AddDays(-((int)DateTime.Now.DayOfWeek - 1)).AddDays(-7);
                        break;
                    default:
                        IsfilterTime = false;
                        break;
                }
            }
            else
            {
                TuNgay = TuNgay ?? DateTime.Now;
                DenNgay = DenNgay ?? DateTime.Now;
                startday = new DateTime(TuNgay.Value.Year, TuNgay.Value.Month, TuNgay.Value.Day, 00, 00, 00);
                Endday = new DateTime(DenNgay.Value.Year, DenNgay.Value.Month, DenNgay.Value.Day, 23, 59, 59);

            }
            return IsfilterTime;
        }


        public static bool IsValidNumberFormat(string input, ref int number)
        {
            return int.TryParse(input, out number);
        }

        public static bool IsValidTimeFormat(string input, out TimeSpan dummyOutput)
        {
            return TimeSpan.TryParse(input, out dummyOutput);
        }

        public static bool IsValiddoubleFormat(string input, out double dummyOutput)
        {
            return double.TryParse(input, out dummyOutput);
        }

        public static string FormatVND(double? input)
        {
            double number = input ?? 0;
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return number.ToString("#,###", cul.NumberFormat);
        }
        /// <summary>
        /// kiểm tra số nguyên
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            return Regex.IsMatch(input, @"^\d+$");
        }
        /// <summary>
        /// kiểm tra số thực
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDouble(string input)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    double price = Convert.ToDouble(input);
                    return true;
                }
                else
                    return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        /// <summary>
        /// tách chuỗi x_x_x thành mảng kiểu int, và sắp xếp giảm dần
        /// </summary>
        /// <param name="strInput"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        public static int[] GetArrIntDesc_fromString(string strInput, char splitChar = '_')
        {
            int[] arrInt = Array.Empty<int>();
            if (!string.IsNullOrEmpty(strInput) && strInput != "null")
            {
                string[] arrStr = strInput.Split(splitChar);
                arrStr = arrStr.Where(x => !string.IsNullOrEmpty(x) && x != "null").Distinct().ToArray();
                arrInt = Array.ConvertAll(arrStr, int.Parse).OrderByDescending(x => x).ToArray();
            }
            return arrInt;
        }
        public static bool CheckCharSpecial(string chuoiCanKiemTra)
        {
            chuoiCanKiemTra = CommonStatic.ConvertToUnSign(chuoiCanKiemTra);
            string chuoidung = "1234567890_-()[]*QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopas dfghjklzxcvbnm,.";
            bool dung = false;
            if (chuoiCanKiemTra != "")
            {
                foreach (char kiTu in chuoiCanKiemTra)
                {
                    dung = false;
                    foreach (char kitu2 in chuoidung)
                    {
                        if (kiTu == kitu2)
                        {
                            dung = true;
                            break;
                        }
                    }
                    if (dung == false)
                    {
                        break;
                    }
                }
            }
            else
            {
                dung = true;
            }
            return dung;
        }

        public enum HinhThucKM
        {
            [Description("Mua hàng giảm giá hàng")]
            MuaHangGiamHang = 11,
            [Description("Mua hàng tặng hàng")]
            MuaHangTangHang = 12,
            [Description("Mua hàng tặng điểm")]
            MuaHangTangDiem = 13,
            [Description("Giá bán theo số lượng mua")]
            GiaBanTheoSLMua = 14,

            [Description("Giảm giá hóa đơn")]
            HDGiamGiaHD = 21,
            [Description("Hóa đơn - Tặng hàng")]
            HDTangHang = 22,
            [Description("Hóa đơn - Giảm giá hàng")]
            HDGiamGiaHang = 23,
            [Description("Hóa đơn - Tặng điểm")]
            HDTangDiem = 24,
        }
    }

    public class GridHellper
    {
        public enum ColumnHangHoa
        {
            mahanghoa = 0,
            tenhanghoa,
            nhomhang,
            loaihang,
            giaban,
            giavon,
            tonkho,
            trangthai,//trangthai kinhdoanh
            trangthaiXoa,
            nhomhotro
        }


        public enum ColumnLoHangHoa
        {
            nhomhang = 0,
            mahang,
            tenhang,
            tendonvitinh,
            giaban,
            soluongton,
            giavon,
            giatriton,
            solo,
            ngaysx,
            ngayhh,
            trangthai,
            tyle
        }

    }

    public static class commonEnumHellper
    {
        public enum NVPhuTrach_VaiTro
        {
            OTHER = 0,
            TU_VAN_PHU,
            TU_VAN_CHINH,
            TELESALE
        }
        public enum TypeHoatDong
        {
            insert = 1,
            update,
            delete,
            huy,
            import,
            export,
            login,
        }
        public enum KeyCompare
        {
            bang = 0,
            nhohon,
            nhohonhoacbang,
            lonhon,
            lonhonhoacbang
        }
        public enum TypeThoiHan
        {
            thang = 0,
            nam,
        }
        public enum TypeIsDelete
        {
            daxoa = 0,
            hoatdong,
        }
        public enum TypeIsFamily
        {
            docthan = 0,
            cogiadinh,
        }
        public enum TypeLoaiHopDong
        {
            xacdinh = 0,
            khongxacdinh,
            thuviec,
            hocviec,
            ctv
        }
        public enum TypeLoaiBaoHiem
        {
            baohiemxh = 0,
            baohiemyte,
            baohiemthatnghiep
        }
        public enum TypeBirthDay
        {
            toanthoigian = 0,
            homnay,
            homqua,
            tuannay,
            tuantruoc,

            thangnay,
            thangtruoc,
            quynay,
            quytruoc,
            namnay,
            namtruoc,
        }
        public enum TypeTTChinhTri
        {
            ketnapdoan = 0,
            ketnapdang,
            danhapngu
        }
        public enum TypeTimeChart
        {
            homnay = 0,
            homqua,
            thangnay,
            thangtruoc,
            namnay,
            namtruoc
        }
        public enum TypeCongViec
        {
            daxoa = 0,
            dangxuly,
            hoanthanh,
            huy,
        }

        // Trạng thái NS_CaLamViec
        public enum TrangThaiCaLamViec
        {
            xoa = 0,
            dangapdung,
            khongapdung,
        }
        public enum CachLayGioCong
        {
            giovaogiocuoi = 1,
            tatcacaccap,
            giomay
        }
        public static Dictionary<int, string> ListCachLayGioCong = new Dictionary<int, string>
        {
            {(int)CachLayGioCong.giovaogiocuoi,"Giờ vào đầu tiên và giờ ra cuối cùng" },
             {(int)CachLayGioCong.tatcacaccap,"Sử dụng tất cả cặp chấm công" },
               {(int)CachLayGioCong.giomay,"Lấy giờ theo loại máy" },
        };
        public static Dictionary<int, string> ListTrangThaiCaLamViec = new Dictionary<int, string>
        {
            {(int)TrangThaiCaLamViec.dangapdung,"Đang áp dụng" },
             {(int)TrangThaiCaLamViec.khongapdung,"Không áp dụng" },
               {(int)TrangThaiCaLamViec.xoa,"Đã xóa" },
        };
        public enum TrangThaiPhanCa
        {
            xoa = 0,
            taomoi,
            dangapdung,
        }
        public enum LoaiNgaynghiLe
        {
            ngaythuong = 0,
            ngaynghi,
            ngayle,
            khac
        }
        public static Dictionary<int, string> ListLoaiNgaynghiLe = new Dictionary<int, string>
        {
            {(int)LoaiNgaynghiLe.ngaythuong,"Ngày thường" },
             {(int)LoaiNgaynghiLe.ngaynghi,"Ngày nghỉ" },
              {(int)LoaiNgaynghiLe.ngayle,"Ngày lễ" },
               {(int)LoaiNgaynghiLe.khac,"Khác" },
        };
        public enum TrangThaiKyTinhCong
        {
            xoa = 0,
            taomoi,
            chotky,
            daapdung
        }
        public static Dictionary<int, string> ListTrangThaiKyTinhCong = new Dictionary<int, string>
        {
            {(int)TrangThaiKyTinhCong.taomoi,"Tạo mới" },
             {(int)TrangThaiKyTinhCong.chotky,"Đã chốt" },
            { (int)TrangThaiKyTinhCong.daapdung,"Đã áp dụng" },
        };

        public enum LoaiCa
        {
            catuan = 1,
            cathang,
            cacodinh
        }
        public static Dictionary<int, string> ListLoaiCa = new Dictionary<int, string>
        {
            {(int)LoaiCa.catuan,"Ca tuần" },
             //{(int)LoaiCa.cathang,"Ca tháng" },
               {(int)LoaiCa.cacodinh,"Ca cố định" },
        };

        public enum TrangThaiChamCong
        {
            xoa = 0,
            apdung,
            huy,
        }
        public enum eBoSungCong
        {
            xoa = 0,
            taomoi,
            chuaduyet,
            duyet,
            dathanhtoan,
        }

        public enum eLoaiCong
        {
            ChamThuCong = 1,
            ChamMay,
        }

        public enum eBangLuongChiTiet
        {
            xoa = 0,
            taomoi,
            CapNhat
        }
        public enum eLoaiLuongPhuCap
        {
            luongcoban = 1,
            luongthucnhan,
            phucap100,
            phucapngay
        }
        public static Dictionary<int, string> ListLoaiLuongPhuCap = new Dictionary<int, string>
        {
            {(int)eLoaiLuongPhuCap.luongcoban,"Lương cơ bản" },
             {(int)eLoaiLuongPhuCap.luongthucnhan,"Lương thực nhận" },
            {(int)eLoaiLuongPhuCap.phucap100,"Phụ cấp cố định" },
             {(int)eLoaiLuongPhuCap.phucapngay,"Phụ cấp hưởng theo ngày đi làm" },
        };

        public enum eKieuTinhLuong
        {
            luongcoban = 0,
            luongcodinh,
            luongtheongay,
            luongtheoca,
            luongtheogio,
        }

        public enum ELoaiPhuCap_GiamTru
        {
            PC_TheoNgay = 51,
            PC_CoDinhVND = 52,
            PC_CoDinhPtram = 53,// ptram luongchinh

            GT_TheoLan = 61,
            GT_CoDinhVND = 62,
            GT_CoDinhPtram = 63 // ptram tongluongnhan
        }

        public static Dictionary<int, string> KieuTinhLuong = new Dictionary<int, string>
        {
            {(int)eKieuTinhLuong.luongcodinh,"Lương cố định" },
             {(int)eKieuTinhLuong.luongtheongay,"Lương theo ngày" },
            {(int)eKieuTinhLuong.luongtheoca,"Lương theo ca" },
             {(int)eKieuTinhLuong.luongtheogio,"Lương theo giờ" },
        };

        public static Dictionary<int, string> PhuCap_GiamTru = new Dictionary<int, string>
        {
            {(int)ELoaiPhuCap_GiamTru.PC_TheoNgay,"Phụ cấp theo ngày" },
             {(int)ELoaiPhuCap_GiamTru.PC_CoDinhVND,"Phụ cấp cố định VND" },
            {(int)ELoaiPhuCap_GiamTru.PC_CoDinhPtram,"Phụ cấp cố định theo % lương chính" },
             {(int)ELoaiPhuCap_GiamTru.GT_TheoLan,"Giảm trừ theo lần" },
             {(int)ELoaiPhuCap_GiamTru.GT_CoDinhVND,"Giảm trừ cố định VND" },
             {(int)ELoaiPhuCap_GiamTru.GT_CoDinhPtram,"Giảm trừ cố định theo % tổng lương nhận" },
        };
        public enum eKhenThuongKyLuat
        {
            kyluat = 1,
            khenthuong
        }
        public enum eLoaiBaoHiem
        {
            xoa = 0,
            dangapdung,
            ngungapdung
        }
        public static Dictionary<int, string> ListeLoaiBaoHiem = new Dictionary<int, string>
        {
            {(int)eLoaiBaoHiem.dangapdung,"Đang áp dụng" },
             {(int)eLoaiBaoHiem.ngungapdung,"Ngừng áp dụng" },
        };
        public enum eBangLuong
        {
            xoa = 0,
            tamluu, // = luu tam
            cantinhlai, // 2. Cần tính lại bảng lương (do công thuộc bảng lương tạm, nhưng lại bị cập nhật )
            daduyet, // = dachot
            dathanhtoan,
            huy
        }
        public static Dictionary<int, string> ListeBangLuong = new Dictionary<int, string>
        {
            {(int)eBangLuong.tamluu,"Lưu tạm" },
            {(int)eBangLuong.cantinhlai,"Bảng lương tạm đã bị thay đổi" },
             {(int)eBangLuong.daduyet,"Đã duyệt" },
            {(int)eBangLuong.dathanhtoan,"Đã thanh toán lương" },
        };

        public static Dictionary<int, string> ListWeek = new Dictionary<int, string>
        {
            {(int)DayOfWeek.Monday,"Thứ 2" },
             {(int)DayOfWeek.Tuesday,"Thứ 3" },
            {(int)DayOfWeek.Wednesday,"Thứ 4" },
             {(int)DayOfWeek.Thursday,"Thứ 5" },
             {(int)DayOfWeek.Friday,"Thứ 6" },
             {(int)DayOfWeek.Saturday,"Thứ 7" },
             {(int)DayOfWeek.Sunday,"Chủ nhật" },
        };
        public static Dictionary<int, string> ListCongViec = new Dictionary<int, string>
        {
            {(int)TypeCongViec.dangxuly,"Đang xử lý" },
             {(int)TypeCongViec.hoanthanh,"Hoàn thành" },
               {(int)TypeCongViec.huy,"Hủy" },
        };
        public enum SMSLoaiTin
        {
            giaodich = 1,
            sinhnhat,
            tinthuong,
            lichhen,
            baoduong,
        }

        public static Dictionary<int, string> ListLoaiTinSMS = new Dictionary<int, string>
        {
            {(int)SMSLoaiTin.giaodich,"Giao dịch" },
             {(int)SMSLoaiTin.sinhnhat,"Sinh nhật" },
               {(int)SMSLoaiTin.tinthuong,"Tin thường" },
               {(int)SMSLoaiTin.lichhen,"Lịch hẹn" },
               {(int)SMSLoaiTin.baoduong,"Nhắc bảo dưỡng" },
        };
        public static Dictionary<int, string> ListTimeChart = new Dictionary<int, string>
        {
            {(int)TypeTimeChart.homnay,"Hôm nay" },
            {(int)TypeTimeChart.homqua,"Hôm qua" },
             {(int)TypeTimeChart.thangnay,"Tháng này" },
               {(int)TypeTimeChart.thangtruoc,"Tháng trước" },
            {(int)TypeTimeChart.namnay,"Năm nay" },
             {(int)TypeTimeChart.namtruoc,"Năm trước" },
        };
        public static Dictionary<int, string> ListChinhTri = new Dictionary<int, string>
        {
            {(int)TypeTTChinhTri.ketnapdoan,"Kết nạp đoàn" },
            {(int)TypeTTChinhTri.ketnapdang,"Kết nạp đảng" },
             {(int)TypeTTChinhTri.danhapngu,"Đã nhập ngũ" },
        };
        public static Dictionary<int, string> ListLoaiBaoHiem = new Dictionary<int, string>
        {
            {(int)TypeLoaiBaoHiem.baohiemxh,"Bảo hiểm xã hội" },
            {(int)TypeLoaiBaoHiem.baohiemyte,"Bảo hiểm y tế" },
             {(int)TypeLoaiBaoHiem.baohiemthatnghiep,"Bảo hiểm thất nghiệp" },
        };
        public static Dictionary<int, string> ListFamily = new Dictionary<int, string>
        {
            {(int)TypeIsFamily.docthan,"Độc thân" },
            {(int)TypeIsFamily.cogiadinh,"Có gia đình" },
        };
        public static Dictionary<int, string> ListLoaiHopDong = new Dictionary<int, string>
        {
             {(int)TypeLoaiHopDong.ctv,"Cộng tác viên" },
              {(int)TypeLoaiHopDong.hocviec,"Học việc" },
             {(int)TypeLoaiHopDong.thuviec,"Thử việc" },
            {(int)TypeLoaiHopDong.xacdinh,"Xác định thời gian" },
            {(int)TypeLoaiHopDong.khongxacdinh,"Không xác định thời gian" },
        };
        public static Dictionary<int, string> ListThoiHan = new Dictionary<int, string>
        {
            {(int)TypeThoiHan.thang,"Tháng" },
            {(int)TypeThoiHan.nam,"Năm" },
        };
        public static Dictionary<int, string> ListCompare = new Dictionary<int, string>
        {
            {(int)KeyCompare.bang,"= : Bằng" },
            {(int)KeyCompare.nhohon,"< : Nhỏ hơn" },
            {(int)KeyCompare.nhohonhoacbang,"≤ : Nhỏ hơn hoặc bằng" },
            {(int)KeyCompare.lonhon,"> : Lớn hơn" },
            {(int)KeyCompare.lonhonhoacbang,"≥ : Lớn hơn hoặc bằng" },
        };
        public static Dictionary<string, string> ListDanToc = new Dictionary<string, string>
        {
            {"01","Kinh" },
             {"02","Tày" },
              {"03","Thái" },
               {"04","Hoa" },
                {"05","Khơ-me" },
                 {"06","Mường" },
                 {"07","Nùng" },
                 {"08","Hmông" },
                 {"09","Dao" },
                 {"10","Gia-rai" },
                 {"11","Ngái" },
                 {"12","Ê-đê" },
                 {"13","Ba-na" },
                 {"14","Xơ-đăng" },
                 {"15","Sán Chay" },
                 {"16","Cơ-ho" },
              {"17","Chăm" },
              {"18","Sán Dìu" },
              {"19","Hrê" },
              {"20","Mnông" },
               {"21","Ra-glai" },
             {"22","Xtiêng" },
              {"23","Bru-Vân Kiều" },
               {"24","Thổ" },
                {"25","Giáy" },
                 {"26","Cơ-tu" },
                 {"27","Gié-Triêng" },
                 {"28","Mạ" },
                 {"29","Khơ-mú" },
                 {"30","Co" },
                 {"31","Ta-ôi" },
                 {"32","Chơ-ro" },
                 {"33","Kháng" },
                 {"34","Xinh-mun" },
                 {"35","Hà Nhì" },
                 {"36","Chu-ru" },
              {"37","Lào" },
              {"38","La Chi" },
              {"39","La Ha" },
              {"40","Phù Lá" },
               {"41","La Hủ" },
              {"42","Lự" },
               {"43","Lô Lô" },
                {"44","Chứt" },
                 {"45","Mảng" },
                 {"46","Pà Thẻn" },
                 {"47","Cơ Lao" },
                 {"48","Cống" },
                 {"49","Bố Y" },
                 {"50","Si La" },
                 {"51","Pu Péo" },
                 {"52","Brâu" },
                 {"53","Ơ Đu" },
                 {"54","Rơ-măm" }
        };

        #region Lịch hẹn, Công việc
        // Lặp lại theo: Ngày, Tuần, Thág, Năm, Không lặp
        public enum RepeatType
        {
            khong = 0,
            ngay,
            tuan,
            thang,
            nam,
        }
        // Nhắc trước x Phút, Tiếng, Ngày, Tuần
        public enum RemindBefore
        {
            phut = 0,
            tieng,
            ngay,
            tuan,
        }

        public static Dictionary<int, string> ListRepeatType = new Dictionary<int, string>
        {
            {(int)RepeatType.khong,"Không lặp lại" },
            {(int)RepeatType.ngay,"Ngày" },
            {(int)RepeatType.tuan,"Tuần" },
            {(int)RepeatType.thang,"Tháng" },
            {(int)RepeatType.nam,"Năm" },
        };

        public static Dictionary<int, string> ListRemindBefore = new Dictionary<int, string>
        {
            {(int)RemindBefore.phut,"Phút" },
            {(int)RemindBefore.tieng,"Tiếng" },
            {(int)RemindBefore.ngay,"Ngày" },
            {(int)RemindBefore.tuan,"Tuần" },
        };
        #endregion
    }

    public class ColumSearch
    {
        public int Key { get; set; }
        public object Value { get; set; }
        public int? type { get; set; }
    }

    public class JsonViewModel<T>
    {
        public bool ErrorCode { get; set; }
        public T Data { get; set; }
    }

    public class SP_ReturnBool
    {
        public bool Exist { get; set; }
    }

    public class TreeItem<T>
    {
        public T Item { get; set; }
        public IEnumerable<TreeItem<T>> Children { get; set; }
    }

    public static class GenericHelpers
    {
        /// <summary>
        /// Generates tree of items from item list
        /// </summary>
        /// 
        /// <typeparam name="T">Type of item in collection</typeparam>
        /// <typeparam name="K">Type of parent_id</typeparam>
        /// 
        /// <param name="collection">Collection of items</param>
        /// <param name="id_selector">Function extracting item's id</param>
        /// <param name="parent_id_selector">Function extracting item's parent_id</param>
        /// <param name="root_id">Root element id</param>
        /// 
        /// <returns>Tree of items</returns>
        public static IEnumerable<TreeItem<T>> GenerateTree<T, K>(
            this IEnumerable<T> collection,
            Func<T, K> id_selector,
            Func<T, K> parent_id_selector,
            K root_id = default(K))
        {
            foreach (var c in collection.Where(c => parent_id_selector(c).Equals(root_id)))
            {
                yield return new TreeItem<T>
                {
                    Item = c,
                    Children = collection.GenerateTree(id_selector, parent_id_selector, id_selector(c))
                };
            }
        }
    }
}

