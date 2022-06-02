using Model_banhang24vn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Open24.Areas.AdminPage.Models
{
    public class BussinessDetailModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public bool? Status { get; set; }
        public int? STT { get; set; }
        public Guid? NganhNgheId { get; set; }
        public List<AnhTinhNangNghanhNghe> Images { get; set; }

    }
    public class BussinessClitentModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public bool? Status { get; set; }
        public string srcImage { get; set; }
    }

    public class NhomNganhModel 
    {
        public long ID { get; set; }
        public string Ten { get; set; }
        public Nullable<bool> TrangThai { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string Icon { get; set; }
        public string GhiChu { get; set; }
        public List<long> ListTinhNang { get; set; }
        public int? ViTri { get; set; }
    }

    public class SearchInputHoTro
    {
        public long ID { get; set; }
        public string Ten { get; set; }
        public string Mota { get; set; }
        public  DateTime? NgayTao{get;set;}
        public string Title { get; set; }

    }
    public class SearchInputHoTroModel
    {
        public string Input  { get; set; }
        public int PageLength { get; set; }
        public List<SearchInputHoTro>  model { get; set; }

    }
}