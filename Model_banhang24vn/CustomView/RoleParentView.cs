using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
    public class RoleParentView
    {
        public string id { get; set; }
        public string text { get; set; }
        public List<RoleParentView> children { get; set; }

    }
    public class CategoryParentView
    {
        public int id { get; set; }
        public string text { get; set; }
        public string edit { get; set; }
        public string delete { get; set; }
        public string[] tags  { get { return new string[] { delete,edit }; } }
        public List<CategoryParentView> children { get; set; }
        public List<CategoryParentView> nodes { get { return (children != null && children.Count > 0) ? children : null; } }
    }
    public class LH_TinhNangParentView
    {
        public long id { get; set; }
        public string text { get; set; }
        public string edit { get; set; }
        public string delete { get; set; }
        public string[] tags { get { return new string[] { delete, edit }; } }
        public List<LH_TinhNangParentView> children { get; set; }
        public List<LH_TinhNangParentView> nodes { get { return (children != null && children.Count > 0) ? children : null; } }
        public string noidung { get; set; }
        public string video { get; set; }
        public bool IsCha { get; set; }
        public string Title { get; set; }

    }
}
