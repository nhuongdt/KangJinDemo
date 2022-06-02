using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_ThongBao_CatDatThoiGian")]
    public class HT_ThongBao_CatDatThoiGian
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int LoaiThongBao { get; set; } = 0;// 0. Tồn kho, 1. Điều chuyển, 2. Lô hàng, 3. Sinh nhat KH, 4. Bảo dưỡng

        [Column(TypeName = "int")]
        public int NhacTruocThoiGian { get; set; } = 0; //Giá trị thời gian nhắc trước

        [Column(TypeName = "int")]
        public int NhacTruocLoaiThoiGian { get; set; } = 0; //0. Ko cài đặt, 1. Phút, 2. Giờ, 3. Ngày, 4. Tháng, 5. Năm

        [Column(TypeName = "int")]
        public int SoLanLapLai { get; set; } = 0;

        [Column(TypeName = "int")]
        public int LoaiThoiGianLapLai { get; set; } = 0; //0. Ko lặp lại, 1. Phút, 2. Giờ, 3. Ngày, 4. Tháng, 5. Năm
    }
}
