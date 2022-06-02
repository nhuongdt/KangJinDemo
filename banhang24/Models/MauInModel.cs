using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace banhang24.Models
{
    public class MauInModel
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public object listTypeKhoGiay { get; set; }
        public object listMauIn { get; set; }
        public Guid? SelectedMauIn { get; set; }
        public int? selectedKhoGiay { get; set; }
    }

    public class ListTypeMauIn
    {
        public Guid? Key { get; set; }
        public string Value { get; set; }
        public int? ChungTuId { get; set; }
    }
    public class MacDinhMauInView
    {
        public string Key { get; set; }
        public List<ListTypeMauIn> ListSelectMauIn { get; set; }
        public Guid? selected { get; set; }
    }
    public class AddNewMauInModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int? KhoGiayId { get; set; }
        public string MaChungTu { get; set; }
        public string DuLieuMauIn { get; set; }
    }

    public class DefaultMauIn
    {
        public Guid? MauInID { get; set; }
        public string MaChungTu { get; set; }
    }
    public class PermissionMauIn
    {
        public bool DatHang { get; set; }
        public bool HoaDon { get; set; }
        public bool TraHang { get; set; }
        public bool DoiTraHang { get; set; }
        public bool GoiDichVu { get; set; }
        public bool NhapHang { get; set; }
        public bool TraHangNhap { get; set; }
        public bool PhieuThu { get; set; }
        public bool PhieuChi { get; set; }
        public bool XuatHuy { get; set; }
        public bool ChuyenHang { get; set; }
        public bool IsInsert{ get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsCopy { get; set; }
        public bool IsExits { get; set; }
        public bool DieuChinh { get; set; }
        public bool TheGiaTri { get; set; }
    }
    public class ModelMauIn
    {
        public HT_CongTy Model { get; set; }
        public PermissionMauIn RolePermission { get; set; }
    }
    public class InMaVach
    {
        public Guid ID_HangHoa { get; set; }
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string GiaSP { get; set; }
        public string Link { get; set; }
        public string Table { get; set; }
        public int Item { get; set; }
        public int SoBanGhi { get; set; }
        public Guid MauInId { get; set; }
        
    }
    public class InListMaVach
    {
        public Guid BangGiaId { get; set; }
        public int Item { get; set; }
        public List<InMaVach> data { get; set; }

    }
}