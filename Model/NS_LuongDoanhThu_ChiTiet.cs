using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_LuongDoanhThu_ChiTiet")]
    public partial class NS_LuongDoanhThu_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_LuongDoanhThu { get; set; }

        [Column(TypeName = "float")]
        public double DoanhThu_Min { get; set; }

        [Column(TypeName = "float")]
        public double? DoanhThu_Max { get; set; }

        [Column(TypeName = "float")]
        public double LuongDuocNhan { get; set; }

        [Column(TypeName = "bit")]
        public bool TheoPT { get; set; }

        [Column(TypeName = "bit")]
        public bool LaThoChinh { get; set; }

        public virtual NS_LuongDoanhThu NS_LuongDoanhThu { get; set; }
    }
}
