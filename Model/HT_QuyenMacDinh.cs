using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_QuyenMacDinh")]
    public partial class HT_QuyenMacDinh
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IDNguoiDung { get; set; }

        [Column(TypeName = "bit")]
        public bool? NhapGiaBan { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? NhapChietKhau { get; set; } = false;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_HDB { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_HDB { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_HDBL { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_HDBL { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_NhapKho { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_NhapKho { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_XuatKho { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_XuatKho { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_MuaHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_MuaHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_HBTL { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_HBTL { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_TraLaiNCC { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_TraLaiNCC { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_PhieuThu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_PhieuChi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_DieuChuyen { get; set; }

        [Column(TypeName = "bit")]
        public bool? SuaNgayChungTu { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? SuaSoChungTu { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? ThayDoiNhanVien { get; set; } = false;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomDichVu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomDoiTuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomHangHoa { get; set; }

        [Column(TypeName = "bit")]
        public bool? NhapChietKhauChung { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? NhapGiamGia { get; set; } = false;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_BaoGia { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDDoiTuong_DonDatMua { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_BaoGia { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IDKho_DonDatMua { get; set; }

        public virtual HT_NguoiDung HT_NguoiDung { get; set; }
    }
}
