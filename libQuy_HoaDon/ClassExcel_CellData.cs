using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libQuy_HoaDon
{
    public class ClassExcel_CellData
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string CellValue { get; set; }// tiêu đề Data của file xuất ra (mặc định ghi ở dòng Cell[1,1]
    }
}
