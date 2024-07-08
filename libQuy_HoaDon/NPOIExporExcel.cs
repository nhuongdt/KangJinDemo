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
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace libQuy_HoaDon
{
    public class NPOIExporExcel
    {
        // các hàng, cột trong NPOI bắt đầu từ vị trí = 0

        /// <summary>
        /// chỉ áp dụng cho mẫu có dòng bảng dữ liệu bắt đầu từ 4
        /// </summary>
        /// <param name="tenChiNhanh"></param>
        /// <param name="thoigian"></param>
        /// <returns></returns>
        public List<ClassExcel_CellData> GetValue_forCell(string tenChiNhanh, string thoigian)
        {
            List<ClassExcel_CellData> lstCell = new List<ClassExcel_CellData>();
            lstCell.Add(new ClassExcel_CellData { RowIndex = 1, ColumnIndex = 0, CellValue = "Thời gian: " + thoigian });
            lstCell.Add(new ClassExcel_CellData { RowIndex = 2, ColumnIndex = 0, CellValue = "Chi nhánh: " + tenChiNhanh });
            return lstCell;
        }
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

        private static ICellStyle CreateBoldStyle(IWorkbook workbook)
        {
            ICellStyle boldStyle = workbook.CreateCellStyle();
            NPOI.SS.UserModel.IFont boldFont = workbook.CreateFont();
            boldFont.IsBold = true;
            boldStyle.SetFont(boldFont);
            return boldStyle;
        }

        private static void ApplySumFormula(IWorkbook workbook, ISheet sheet, int startRow, int endRow)
        {
            if (startRow > endRow || startRow < 0 || endRow >= sheet.LastRowNum)
            {
                throw new ArgumentException("Invalid row range specified.");
            }

            // Tạo style với font in đậm
            ICellStyle cellStyle = CreateBoldStyle(workbook);

            // Tạo dòng mới để chứa công thức tổng
            IRow sumRow = sheet.CreateRow(endRow + 1);
            sumRow.CreateCell(0).SetCellValue("Tổng cộng");
            sumRow.GetCell(0).CellStyle = cellStyle;

            // Duyệt qua từng cột
            var lastCell = sheet.GetRow(startRow).LastCellNum;
            for (int colIndex = 1; colIndex < lastCell; colIndex++)
            {
                bool isNumericColumn = true;

                // Kiểm tra kiểu dữ liệu của cột
                for (int rowIndex = startRow; rowIndex <= endRow; rowIndex++)
                {
                    ICell cell = sheet.GetRow(rowIndex).GetCell(colIndex);
                    if (cell == null || (cell.CellType != CellType.Numeric && cell.CellType != CellType.Formula))
                    {
                        isNumericColumn = false;
                        break;
                    }

                    // Kiểm tra xem ô có chứa kiểu datetime không
                    if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                    {
                        isNumericColumn = false;
                        break;
                    }
                }

                // Nếu tất cả các ô trong cột là kiểu số, áp dụng công thức tổng
                if (isNumericColumn)
                {
                    ICell sumCell = sumRow.CreateCell(colIndex, CellType.Formula);
                    string columnLetter = CellReference.ConvertNumToColString(colIndex);
                    sumCell.CellFormula = $"SUM({columnLetter}${startRow + 1}:{columnLetter}${endRow + 1})";
                    sumCell.CellStyle = cellStyle;

                    // Sao chép style từ ô tương ứng trong startRow
                    ICell sourceCell = sheet.GetRow(startRow).GetCell(colIndex);
                    ICellStyle newCellStyle = workbook.CreateCellStyle();
                    newCellStyle.CloneStyleFrom(sourceCell.CellStyle);

                    // Kết hợp với style in đậm
                    NPOI.SS.UserModel.IFont boldFont = workbook.CreateFont();
                    boldFont.IsBold = true;
                    newCellStyle.SetFont(boldFont);

                    sumCell.CellStyle = newCellStyle;
                }
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

            // Điền dữ liệu từ DataTable vào các hàng tiếp theo
            ImportDataTableToSheet(dt, sheet, startRow);

            RemoveHideColumns(sheet, columnsHide, startRow);

            SetValueToHeaderCells(sheet, lstDataCell);

            if (indexTotalRow != -1)
            {
                ApplySumFormula(workbook, sheet, startRow, dt.Rows.Count + startRow - 1);
                // Tính toán lại công thức trong workbook
                XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook);
            }

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
