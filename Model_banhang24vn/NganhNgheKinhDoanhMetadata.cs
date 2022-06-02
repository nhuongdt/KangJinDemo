using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn
{
    [MetadataTypeAttribute(typeof(NganhNgheKinhDoanhMetadata))]
    public partial class NganhNgheKinhDoanh
    {
        internal sealed class NganhNgheKinhDoanhMetadata
        {

            public System.Guid ID { get; set; }

            [Required(ErrorMessage = "Không để trống 'Mã ngành nghề kinh doanh'")]
            [Display(Name = "Mã ngành nghề kinh doanh")]
            public string MaNganhNghe { get; set; }

            [Required(ErrorMessage = "Không để trống 'Tên ngành nghề kinh doanh'")]
            [Display(Name = "Tên ngành nghề kinh doanh")]
            public string TenNganhNghe { get; set; }

        }
    }
}
