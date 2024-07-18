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
        public bool IsNumber { get; set; }
    }

    public class Excel_ParamExport
    {
        public int SheetIndex { get; set; }
        public List<ClassExcel_CellData> CellData { get; set; }
        public int StartRow { get; set; }// vị trí bắt đầu của dòng dữ liệu (trong file mẫu)
        public int? EndRow { get; set; } = 30;// vị trí kết thúc của dòng dữ liệu (trong file mẫu)
        public string ColumnsHide { get; set; } // mảng các cột bị xóa: ngăn cách = dấu gạch dưới (4_8_3_)
    }
}
