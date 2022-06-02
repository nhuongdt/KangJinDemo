using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libNS_NhanVien
{
    class MayChamCongObject
    {
    }

    public class GridMayChamCong
    {
        public Guid ID { get; set; }
        public string MaMCC { get; set; }
        public string TenMCC { get; set; }
        public string TenHienThi { get; set; }
        public string IP { get; set; }
        public Guid IDChiNhanh { get; set; }
        public int LoaiKetNoi { get; set; }
        public int CongCOM { get; set; }
        public int TocDoCom { get; set; }
        public string MatMa { get; set; }
        public int Port { get; set; }
        public string MaChiNhanh { get; set; }
        public string TenChiNhanh { get; set; }
        public string SoSeries { get; set; }
        public string GhiChu { get; set; }
        public int LoaiHinh { get; set; }
        public string IDMay { get; set; }
    }

    public class ObjGetDuLieuCongThoTheoThang
    {
        public string MaChamCong { get; set; }
        public string TenMayChamCong { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string IPDomain { get; set; }
        public DateTime ThoiGian { get; set; }
        public int RowNum { get; set; }
        public int MaxRow { get; set; }
    }

    public class ObjCongTho
    {
        public string MaChamCong { get; set; }
        public DateTime ThoiGian { get; set; }
        public Guid ID_MCC { get; set; }
        public int VaoRa { get; set; }
        public int TrangThai { get; set; }
    }
}
