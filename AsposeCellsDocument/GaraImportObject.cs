using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsposeCellsDocument
{
    class GaraImportObject
    {
    }
    public class DanhSachXeImport
    {
        public string BienSo { get; set; }
        public string TenMauXe { get; set; }
        public string TenHangXe { get; set; }
        public string TenLoaiXe { get; set; }
        public string SoMay { get; set; }
        public string SoKhung { get; set; }
        public int NamSanXuat { get; set; }
        public string MauSon { get; set; }
        public string DungTich { get; set; }
        public string HopSo { get; set; }
        public string MaKhachHang { get; set; }
        public string GhiChu { get; set; }
        public int index { get; set; }
    }

    public class ImportErrorList
    {
        public int index { get; set; }
        public string ViTri { get; set; }
        public string MoTa { get; set; }
    }
}
