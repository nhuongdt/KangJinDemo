using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libHT
{
    class ClassHTObject
    {
    }
    public class LichSuThaoTac
    {
        public string TenNhanVien { get; set; }
        public string ChucNang_CV { get; set; }
        public string ChucNang_GC { get; set; }
        public string ChucNang { get; set; }
        public DateTime ThoiGian { get; set; }
        public string NoiDung { get; set; }
        public string NoiDungChiTiet { get; set; }
        public string NoiDung_CV { get; set; }
        public string NoiDung_GC { get; set; }
    }

    public class ObjectHTThongBao
    {
        public bool DaDoc { get; set; }
        public string NoiDungThongBao { get; set; }
        public string NgayTao { get; set; }
        public string Image { get; set; }
    }

    public class GetListThongBao
    {
        public Guid ID { get; set; }
        public Guid ID_DonVi { get; set; }
        public int LoaiThongBao { get; set; }
        public string NoiDungThongBao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiDungDaDoc { get; set; }
        public int ChuaDoc { get; set; }
    }
}
