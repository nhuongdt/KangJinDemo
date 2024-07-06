using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Aspose.Cells;
using Microsoft.Office.Interop.Excel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace libQuy_HoaDon
{
    public class NPOIExporExcel
    {
        // các hàng, cột trong NPOI bắt đầu từ vị trí = 0
        private void ImportDataTableToSheet(System.Data.DataTable dataTable, ISheet sheet, int startRow)
        {
            // Tạo danh sách các CellStyle (same style + format of start row)
            List<ICellStyle> cellStyles = new List<ICellStyle>();
            IRow headerRow = sheet.GetRow(startRow);
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                ICell templateCell = headerRow.GetCell(col);
                cellStyles.Add(templateCell?.CellStyle);
            }

            // Điền dữ liệu từ DataTable vào sheet, giữ nguyên định dạng
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                IRow excelRow = sheet.CreateRow(startRow + row);
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    ICell cell = excelRow.CreateCell(col);
                    cell.CellStyle = cellStyles[col];

                    var value = dataTable.Rows[row][col];
                    switch (value)
                    {
                        case int intValue:
                            cell.SetCellValue(intValue);
                            break;
                        case double doubleValue:
                            cell.SetCellValue(doubleValue);
                            break;
                        case DateTime dateValue:
                            cell.SetCellValue(dateValue);
                            break;
                        default:
                            cell.SetCellValue(value.ToString());
                            break;
                    }
                }
            }
        }

        private void SetValueToHeaderCells(ISheet sheet, List<ClassExcel_CellData> lstDataCell = null)
        {
            if (lstDataCell != null)
            {
                foreach (var item in lstDataCell)
                {
                    // Thiết lập giá trị cho ô tại cột x, hàng y
                    IRow row = sheet.GetRow(item.RowIndex); // Lấy dòng y
                    if (row == null)
                    {
                        row = sheet.CreateRow(item.RowIndex); // Nếu dòng chưa tồn tại, tạo mới
                    }
                    ICell cell = row.GetCell(item.ColumnIndex) ?? row.CreateCell(item.ColumnIndex); // Lấy ô tại cột x, nếu ô chưa tồn tại, tạo mới
                    cell.SetCellValue(item.CellValue);
                }
            }
        }
        private void RemoveHideColumns(ISheet sheet, string columnsToRemove, int startRow)
        {
            int[] arrColumn = null;
            if (!string.IsNullOrEmpty(columnsToRemove) && columnsToRemove != "null")
            {
                string[] coloumHide = columnsToRemove.Split('_');
                coloumHide = coloumHide.Where(x => !string.IsNullOrEmpty(x) && x != "null").Distinct().ToArray(); columnsToRemove.Split('_');
                arrColumn = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
            }
            if (arrColumn != null)
            {
                foreach (int columnIndex in arrColumn)
                {
                    // Dịch chuyển các ô phía bên phải của columnIndex sang trái để lấp đầy ô bị xóa
                    int lastColumnIndex = sheet.GetRow(startRow).LastCellNum - 1;
                    foreach (IRow row in sheet)
                    {
                        for (int i = columnIndex; i < lastColumnIndex; i++)
                        {
                            ICell oldCell = row.GetCell(i + 1);
                            ICell newCell = row.GetCell(i);

                            if (oldCell != null)
                            {
                                if (newCell == null)
                                {
                                    newCell = row.CreateCell(i, oldCell.CellType);
                                }

                                CopyCell(oldCell, newCell);
                                row.RemoveCell(oldCell);
                            }
                        }

                        // Xóa ô cuối cùng
                        ICell lastCell = row.GetCell(lastColumnIndex);
                        if (lastCell != null)
                        {
                            row.RemoveCell(lastCell);
                        }
                    }
                }
            }
        }
        private void CopyCell(ICell oldCell, ICell newCell)
        {
            newCell.CellStyle = oldCell.CellStyle;
            if (oldCell.CellComment != null)
            {
                newCell.CellComment = oldCell.CellComment;
            }
            if (oldCell.Hyperlink != null)
            {
                newCell.Hyperlink = oldCell.Hyperlink;
            }
            newCell.SetCellType(oldCell.CellType);

            switch (oldCell.CellType)
            {
                case CellType.Boolean:
                    newCell.SetCellValue(oldCell.BooleanCellValue);
                    break;
                case CellType.Error:
                    newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                    break;
                case CellType.Formula:
                    newCell.SetCellFormula(oldCell.CellFormula);
                    break;
                case CellType.Numeric:
                    newCell.SetCellValue(oldCell.NumericCellValue);
                    break;
                case CellType.String:
                    newCell.SetCellValue(oldCell.RichStringCellValue);
                    break;
                default:
                    break;
            }
        }

        public void ExportDataToExcel(string templatePath, System.Data.DataTable dt, int startRow, string columnsHide,
            List<ClassExcel_CellData> lstDataCell = null, int indexTotalRow = 0)
        {
            // Bước 2: Mở file mẫu bằng NPOI
            IWorkbook workbook;
            using (var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }

            // Bước 4: Ghi dữ liệu vào sheet đầu tiên của file mẫu
            ISheet sheet = workbook.GetSheetAt(0);

            //// Điền dữ liệu từ DataTable vào các hàng tiếp theo
            ImportDataTableToSheet(dt, sheet, startRow);

            RemoveHideColumns(sheet, columnsHide, startRow);

            SetValueToHeaderCells(sheet, lstDataCell);

            // Bước 5: Lưu workbook vào MemoryStream, và trả về Http cho client để download
            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment");
                HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                HttpContext.Current.Response.End();
            }
        }
    }
}
