using Model_banhang24vn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Open24.App_Start.App_API
{ 
    public class MailHelper
    {
        public static void SendMail(Model_banhang24vn.CuaHangDangKy cuahang)
        {
            if (!string.IsNullOrWhiteSpace(cuahang.Email))
            {
                MailMessage Msg = new MailMessage();
                Msg.Subject = "[Đăng ký open24] " + cuahang.HoTen + " - " + cuahang.TenCuaHang;
                Msg.From = new MailAddress("dangky@open24.vn", "Open24.vn");
                Msg.To.Add(cuahang.Email);
                Msg.Body = "Họ tên: " + cuahang.HoTen + ".<br />Số điện thoại: " + cuahang.SoDienThoai + ".<br />Email: " + cuahang.Email
                    + ".<br />Địa chỉ: " + Model_banhang24vn.DAL.CuaHangDangKyService.GetAddress(cuahang.SoDienThoai) + ".<br />Ngành nghề kinh doanh: "
                    + Model_banhang24vn.M_NganhNgheKinhDoanh.Select_ID(cuahang.ID_NganhKinhDoanh.GetValueOrDefault()).TenNganhNghe + ".<br />Trang quản lý: " + cuahang.SubDomain + ".open24.vn.<br />"
                    + "Tên đăng nhập: " + cuahang.UserKT + ".<br />Mật khẩu: " + cuahang.MatKhauKT;
                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential("dangky@open24.vn", "Ssoftvn20182018");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(Msg);
            }
        }

        public static void SendMailToDangKy(Model_banhang24vn.CuaHangDangKy cuahang)
        {
            MailMessage Msg = new MailMessage();
            Msg.Subject = "[Khách hàng đăng ký Open24] " + cuahang.HoTen + " - " + cuahang.TenCuaHang;
            Msg.From = new MailAddress("dangky@open24.vn", "Open24.vn");
            Msg.To.Add("dangky@open24.vn");

            Msg.Body = "Họ tên: " + cuahang.HoTen + ".<br />Số điện thoại: " + cuahang.SoDienThoai + ".<br />Email: " + cuahang.Email
                    + ".<br />Địa chỉ: " + Model_banhang24vn.DAL.CuaHangDangKyService.GetAddress(cuahang.SoDienThoai) + ".<br />Ngành nghề kinh doanh: "
                    + Model_banhang24vn.M_NganhNgheKinhDoanh.Select_ID(cuahang.ID_NganhKinhDoanh.GetValueOrDefault()).TenNganhNghe + ".<br />Trang quản lý: " + cuahang.SubDomain + ".open24.vn.<br />"
                    + "Tên đăng nhập: " + cuahang.UserKT + ".<br />Mật khẩu: " + cuahang.MatKhauKT+"<br />"
                    + "Địa chỉ IPV4: "+ cuahang.DiaChiIP_DK+"<br />"
                    + "Thiết bị truy cập: " + cuahang.ThietBi_DK + "<br />"
                    + "Hệ điều hành: " + cuahang.HeDieuHanh_DK + "<br />"
                    + "Trình duyệt: " + cuahang.TrinhDuyet_DK + "<br />"
                    + "Khu vực: " + cuahang.KhuVuc_DK + "<br />";
            Msg.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential networkCredential = new NetworkCredential("dangky@open24.vn", "Ssoftvn20182018");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Port = 587;
            smtp.Send(Msg);
        }
    }
}