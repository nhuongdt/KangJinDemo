﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_DoiTuong_Anh")]
    public partial class DM_DoiTuong_Anh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_DoiTuong_Anh()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int SoThuTu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DoiTuong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string URLAnh { get; set; } = string.Empty;

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }
    }
}
