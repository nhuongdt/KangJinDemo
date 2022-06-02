using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace banhang24.Models
{
    public class NhanVienModel
    {
        public NS_NhanVien nhanvien { get; set; }

        public List<QuaTrinhCongTacModel> QuaTrinhCongTac { get; set; }
    }

    public class QuaTrinhCongTacModel
    {
        public int ID { get; set; }
        public Guid? ID_ChiNhanh { get; set; }

        public Guid? ID_PhongBan { get; set; }
        public string Text_ChiNhanh { get; set; }

        public string Text_PhongBan { get; set; }

        public object listPhongBan { get; set; }

        public bool LaMacDinh { get; set; }
    }

    public class NhanVienGiaDinh : NS_NhanVien_GiaDinh
    {
        public DateTime? NgaySinhDate { get; set; }
        public int? TypeNgaySinh { get; set; }

    }
    public class NhanVienRoleModel
    {
        public bool IsHRM { get; set; }
        public bool RolePhongBan_Insert { get; set; }
        public bool RolePhongBan_Update { get; set; }
        public bool RolePhongBan_Delete { get; set; }
        public bool RoleLoaiLuong_Insert { get; set; }
        public bool RoleLoaiLuong_Update { get; set; }
        public bool RoleLoaiLuong_Delete { get; set; }
        public bool RoleImport { get; set; }
        public bool RoleExport { get; set; }
        public bool UserRoleInsert { get; set; }
        public bool UserRoleUpdate { get; set; }
        public bool UserRoleDelete { get; set; }
    }
    

}