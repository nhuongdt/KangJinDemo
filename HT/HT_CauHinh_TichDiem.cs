using System;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using Model;

namespace HT
{
    public class HT_CauHinh_TichDiem
    {
        private SsoftvnContext db;

        public HT_CauHinh_TichDiem(SsoftvnContext _db)
        {
            db = _db;
        }
        public  IQueryable<Object> GetHT_CauHinh_TichDiem(Guid idDonVi)
        {
            if (db != null)
            {
                var data = from ct in db.HT_CauHinh_TichDiemChiTiet
                           join ad in db.HT_CauHinh_TichDiemApDung on ct.ID equals ad.ID_TichDiem into CT_AD
                           from ct_ad in CT_AD.DefaultIfEmpty()
                           join ch in db.HT_CauHinhPhanMem on ct.ID_CauHinh equals ch.ID
                           where ch.ID_DonVi == idDonVi
                           select new
                           {
                               ID_TichDiem = ct.ID,
                               ID_ApDung = ct_ad == null ? Guid.Empty : ct_ad.ID,
                               ID_NhomDoiTuong = ct_ad == null ? Guid.Empty : ct_ad.ID_NhomDoiTuong,
                               TyLeDoiDiem = ct.TyLeDoiDiem,
                               ThanhToanBangDiem = ct.ThanhToanBangDiem,
                               KhoiTaoTichDiem = ct.KhoiTaoTichDiem, // chua dung
                               TichDiemGiamGia = ct.TichDiemGiamGia,
                               TichDiemHoaDonDiemThuong = ct.TichDiemHoaDonDiemThuong,
                               TienThanhToan = ct.TienThanhToan,
                               DiemThanhToan = ct.DiemThanhToan,
                               ToanBoKhachHang = ct.ToanBoKhachHang,
                               TichDiemHoaDonGiamGia = ct.TichDiemHoaDonGiamGia,
                               SoLanMua = ct.SoLanMua,
                           };

                return data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// get cau hinh tich diem chi tiet
        /// </summary>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        public  List<SP_HT_TichDiem_ChiTiet> SP_GetHT_CauHinh_TichDiem(Guid idDonVi)
        {
            if (db != null)
            {
                SqlParameter param = new SqlParameter("ID_DonVi", idDonVi);
                var data = db.Database.SqlQuery<SP_HT_TichDiem_ChiTiet>("SP_GetHT_CauHinh_TichDiem @ID_DonVi", param).ToList();
                return data;
            }
            else
            {
                return null;
            }
        }
    }

    public class SP_HT_TichDiem_ChiTiet
    {
        public Guid ID_TichDiem { get; set; }
        public Guid? ID_ApDung { get; set; }
        public Guid? ID_NhomDoiTuong { get; set; }
        public double TyLeDoiDiem { get; set; }
        public bool ThanhToanBangDiem { get; set; }
        public bool KhoiTaoTichDiem { get; set; }
        public bool TichDiemGiamGia { get; set; }
        public bool TichDiemHoaDonDiemThuong { get; set; }
        public double TienThanhToan { get; set; }
        public int DiemThanhToan { get; set; }
        public bool ToanBoKhachHang { get; set; }
        public bool TichDiemHoaDonGiamGia { get; set; }
        public int SoLanMua { get; set; }
    }
}
