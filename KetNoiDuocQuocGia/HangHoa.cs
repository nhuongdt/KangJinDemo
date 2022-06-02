using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Model_banhang24vn;
using Model_banhang24vn.DAL;
using libDM_HangHoa;

namespace KetNoiDuocQuocGia
{
    public class HangHoa
    {
        public int progress = 0;
        public int total = 0;
        private SsoftvnContext db;

        public HangHoa(SsoftvnContext ssoftvnContext)
        {
            db = ssoftvnContext;
        }

        public bool SyncBy(int option)
        {
            return false;
        }

        public bool SyncByFull()
        {
            IQueryable<DanhMucThuocQuocGia> lstThuoc;
            DanhMucThuocQuocGiaService dmtService = new DanhMucThuocQuocGiaService();
            lstThuoc = dmtService.GetAll();
            IQueryable<Model.DM_HangHoa> lstHangHoa;
            ClassDM_HangHoa chh = new ClassDM_HangHoa(db);
            //lstHangHoa = chh.GetAll().Select(p => new DanhMucThuocQuocGia
            //{
            //    MaThuoc = "",
            //    TenThuoc = p.TenHangHoa,
            //    SoDangKy = "",
            //    HoatChatChinh = "",
            //    HamLuong = "",
            //    QuyCachDongGoi = "",
            //    HangSanXuat = "",
            //    NuocSanXuat = "",
            //    DonViTinh = ""
            //});
            //lstHangHoa = chh.GetAll();

            //IEnumerable<Model.DM_HangHoa> query = from thuoc in lstThuoc
            //            join hanghoa in lstHangHoa
            //            on thuoc.TenThuoc equals hanghoa.TenHangHoa
            //            select new Model.DM_HangHoa
            //            {
            //                ID = hanghoa.ID,
            //                TenKhac = thuoc.MaThuoc
            //            };
            //chh.DongBoMaThuocQuocGia(query.ToList());
            
            return false;
        }
    }

    //public class HangHoaDuocQG
    //{
    //    public string MaQuocGia { get; set; }
    //    public string TenThuoc { get; set; }
    //    public string HoatChat { get; set; }
    //    public string HamLuong { get; set; }
    //    public string HangSanXuat { get; set; }
    //    public string QuyCachDongGoi { get; set; }
    //    public string NuocSanXuat { get; set; }
    //    public string DonViTinh { get; set; }
    //}
    public class ThuocDongBo
    {
        public Guid ID { get; set; }
        public string MaThuoc { get; set; }
        
    }
}
