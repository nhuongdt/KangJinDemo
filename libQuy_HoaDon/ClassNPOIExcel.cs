using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using Aspose.Cells;
using libDM_DoiTuong;
using libDM_HangHoa;
using libDonViQuiDoi;
using Microsoft.Office.Interop.Excel;
using Model;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using static libQuy_HoaDon.Class_Report;

namespace libQuy_HoaDon
{
    public class ClassNPOIExcel
    {
        // các hàng, cột trong NPOI bắt đầu từ vị trí = 0
        #region Export to Excel
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

                    if (item.IsNumber)
                    {
                        try
                        {
                            if (double.TryParse(item.CellValue, out double numericValue))
                            {
                                cell.SetCellValue(numericValue);
                            }
                        }
                        catch (Exception)
                        {
                            cell.SetCellValue(item.CellValue);
                        }
                    }
                    else
                    {
                        cell.SetCellValue(item.CellValue);
                    }
                }
            }
        }
        private void RemoveHideColumns(ISheet sheet, string columnsToRemove, int startRow)
        {
            int[] arrColumn = CommonStatic.GetArrIntDesc_fromString(columnsToRemove);
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
            //  Đặt chiều cao cho hàng (đơn vị là điểm, 1/20 của một point, 20 = 1 point)
            sumRow.Height = 20 * 18;// 18 điểm

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
            // Mở file mẫu bằng NPOI
            IWorkbook workbook;
            using (var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }

            // Ghi dữ liệu vào sheet đầu tiên của file mẫu
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

            // Lưu workbook vào MemoryStream, và trả về Http cho client để download
            ReturnFileExcel_ToBrower(workbook);
        }

        private void ReturnFileExcel_ToBrower(IWorkbook workbook)
        {
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

        public void ExportMultipleSheet(string templatePath, List<System.Data.DataTable> lstDataTable, List<Excel_ParamExport> lstPr = null)
        {
            // Mở file mẫu bằng NPOI
            IWorkbook workbook;
            using (var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }
            foreach (var pr in lstPr)
            {
                ExportDetailData_ToExcel(workbook, pr.SheetIndex, lstDataTable[pr.SheetIndex], pr.CellData, pr.StartRow, pr.EndRow ?? 30);
            }
        }
        private void RemoveAndShiftRow(ISheet sheet, int startIndexRowDelete, int endRow)
        {
            // Xóa các dòng từ startRow đến endRow
            for (int rowIndex = startIndexRowDelete; rowIndex < endRow; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    sheet.RemoveRow(row);
                }
            }

            int numRowsToShift = endRow - startIndexRowDelete;
            int rowCount = sheet.LastRowNum;
            sheet.ShiftRows(endRow, rowCount, -numRowsToShift);
        }
        public void ExportDetailData_ToExcel(IWorkbook workbook, int sheetIndex, System.Data.DataTable dt, List<ClassExcel_CellData> lstDataCell = null, int startRow = 3, int endRow = 30)
        {
            // Ghi dữ liệu vào sheet (indexSheet)
            ISheet sheet = workbook.GetSheetAt(sheetIndex);

            // Điền dữ liệu từ DataTable vào các hàng tiếp theo
            ImportDataTableToSheet(dt, sheet, startRow);

            // Xóa các dòng từ startRow đến endRow, và thực hiện dịch chuyển lên trên
            RemoveAndShiftRow(sheet, startRow + dt.Rows.Count, endRow);

            // set giá trị đến 1 số cell mặc định
            SetValueToHeaderCells(sheet, lstDataCell);

            // Lưu workbook vào MemoryStream, và trả về Http cho client để download
            ReturnFileExcel_ToBrower(workbook);
        }

        private void GetCell_HasMerger(ISheet sheet, int startRow, int endRow)
        {
            List<CellRangeAddress> mergedRegions = new List<CellRangeAddress>();
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                CellRangeAddress region = sheet.GetMergedRegion(i);
                if (region.FirstRow >= startRow && region.LastRow <= endRow)
                {
                    // Nếu ô gộp nằm hoàn toàn trong khoảng dòng bị xóa, bỏ qua
                    continue;
                }
                mergedRegions.Add(region);
            }
        }
        private void SetAgainFormulas_ToCell(ISheet sheet, int startRow, int endRow)
        {
            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    foreach (ICell cell in row.Cells)
                    {
                        if (cell.CellType == CellType.Formula)
                        {
                            string formula = cell.CellFormula;
                            string updatedFormula = UpdateFormula(formula, startRow, endRow);
                            cell.SetCellFormula(updatedFormula);
                        }
                    }
                }
            }
        }
        private string UpdateFormula(string formula, int startRow, int endRow)
        {
            return UpdateFormula_Sum(formula, startRow, endRow);
        }

        private string UpdateFormula_Sum(string formula, int startRow, int endRow)
        {
            // Tạo pattern để bắt các công thức dạng SUM(A$4:A20)
            string pattern = @"SUM\(([A-Z]+)\$(\d+):([A-Z]+)(\d+)\)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.Replace(formula, match =>
            {
                string columnStart = match.Groups[1].Value;
                int rowStart = int.Parse(match.Groups[2].Value);
                string columnEnd = match.Groups[3].Value;
                int rowEnd = int.Parse(match.Groups[4].Value);

                // Nếu khoảng dòng bị xóa nằm trong khoảng dòng của công thức
                if (rowStart <= endRow && rowEnd >= startRow)
                {
                    rowEnd = startRow - 1;
                }

                return $"SUM({columnStart}${rowStart}:{columnEnd}${rowEnd})";
            });
        }

        #endregion

        #region Import from Excel
        /// <summary>
        /// Kiểm tra file mẫu không đúng định dạng/hoặc không có dữ liệu
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="headerTitle">tiêu đề của file</param>
        /// <param name="startRow">vị trí bắt đầu của dòng chứa dữ liệu</param>
        /// <returns></returns>
        public string CheckFileMau(ISheet sheet, string headerTitle, int startRow)
        {
            try
            {
                IRow row = sheet.GetRow(0);
                if (row != null)
                {
                    var txtHeadear = row.GetCell(0)?.ToString();
                    if (txtHeadear != headerTitle)
                    {
                        return "Vui lòng import file đúng định dạng";
                    }
                    else
                    {
                        if (sheet.PhysicalNumberOfRows < startRow)
                        {
                            return "File import không có dữ liệu";
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
                return "File import không có dữ liệu";
            }
            catch (Exception)
            {
                return "Vui lòng import file đúng định dạng";
            }
        }
        static System.Data.DataTable ConvertExcelToDataTable(Stream inputStream, int startRow)
        {
            IWorkbook workbook = new XSSFWorkbook(inputStream);
            ISheet sheet = workbook.GetSheetAt(0);
            System.Data.DataTable dataTable = new System.Data.DataTable();

            // Thêm các cột vào DataTable
            IRow headerRow = sheet.GetRow(0);
            for (int j = 0; j < headerRow.LastCellNum; j++)
            {
                ICell headerCell = headerRow.GetCell(j);
                string columnName = headerCell != null ? headerCell.ToString() : $"Column{j + 1}";
                dataTable.Columns.Add(columnName);
            }

            // Đọc dữ liệu từ các hàng
            for (int rowIndex = startRow; rowIndex < sheet.PhysicalNumberOfRows; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int colIndex = 0; colIndex < row.LastCellNum; colIndex++)
                    {
                        ICell cell = row.GetCell(colIndex);
                        dataRow[colIndex] = cell != null ? cell.ToString() : null;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
        public List<ErrorDMHangHoa> CheckData_FileImportHangHoa(ISheet sheet)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Dictionary<string, List<int>> maHangHoaTracker = new Dictionary<string, List<int>>();

            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                //classDM_DoiTuong classDMHang = new classDM_DoiTuong(db);
                //var check = classDMHang.SP_CheckMaNhanVien_Exist("KH0000001");
                //var check1 = classDMHang.SP_CheckSoDienThoai_Exist("0989861122");

                // duyệt bắt đầu từ dòng số 3
                var lastRow = sheet.PhysicalNumberOfRows;
                for (int rowIndex = 3; rowIndex < lastRow; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);

                    if (row != null)
                    {
                        string tenNhomHang = row.GetCell(1)?.ToString();
                        string maHangHoa = row.GetCell(2)?.ToString();
                        string tenHangHoa = row.GetCell(3)?.ToString();
                        string loaihang = row.GetCell(4)?.ToString().Trim();
                        string khongDuocBan = row.GetCell(5)?.ToString().Trim();
                        string giaVon = row.GetCell(7)?.ToString().Trim();
                        string giaBan = row.GetCell(8)?.ToString().Trim();
                        string tonKho = row.GetCell(9)?.ToString().Trim();
                        string tenDonViTinh = row.GetCell(10)?.ToString().Trim();
                        string quyCach = row.GetCell(11)?.ToString().Trim();
                        string mdvtcoban = row.GetCell(13)?.ToString().Trim();
                        string gtriquydoi = row.GetCell(14)?.ToString().Trim();

                        // bỏ qua dòng có dữ liệu trống
                        if (string.IsNullOrEmpty(tenNhomHang) && string.IsNullOrEmpty(maHangHoa) && string.IsNullOrEmpty(tenHangHoa)
                            && string.IsNullOrEmpty(loaihang) && string.IsNullOrEmpty(giaVon)
                            && string.IsNullOrEmpty(giaBan) && string.IsNullOrEmpty(tonKho))
                        {
                            continue;
                        }

                        // Kiểm tra trùng lặp MaHangHoa
                        if (!string.IsNullOrEmpty(maHangHoa))
                        {
                            if (maHangHoaTracker.ContainsKey(maHangHoa))
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Mã hàng/dịch vụ",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = maHangHoa,
                                    DienGiai = "Mã hàng: " + maHangHoa + " bị trùng lặp",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                            else
                            {
                                maHangHoaTracker[maHangHoa] = new List<int> { rowIndex + 1 };
                            }

                            var checkExist = classDMHangHoa.Check_MaHangHoaExist(maHangHoa);
                            if (checkExist)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Mã hàng/dịch vụ",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = maHangHoa,
                                    DienGiai = "Mã hàng: " + maHangHoa + " đã tồn tại",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }

                        if (string.IsNullOrEmpty(tenHangHoa))
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Tên hàng hóa",
                                ViTri = (rowIndex + 1).ToString(),
                                ThuocTinh = maHangHoa,
                                DienGiai = "Tên hàng hóa không được để trống",
                                rowError = rowIndex,
                                loaiError = 1
                            };
                            lstError.Add(DM);
                        }
                        if (!string.IsNullOrEmpty(loaihang))
                        {
                            if (loaihang != "Combo" && loaihang != "Dịch vụ")
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Loại hàng hóa",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = loaihang,
                                    DienGiai = "Loại hàng hóa chưa được định nghĩa",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }

                        if (!string.IsNullOrEmpty(khongDuocBan) && khongDuocBan != "x")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Không bán trực tiếp",
                                ViTri = (rowIndex + 1).ToString(),
                                ThuocTinh = khongDuocBan,
                                DienGiai = "Là hàng hóa không được bán trực tiếp bạn cần đánh dấu x",
                                rowError = rowIndex,
                                loaiError = 1
                            };
                            lstError.Add(DM);
                        }
                        if (!string.IsNullOrEmpty(giaVon))
                        {
                            bool isNumber7 = CommonStatic.IsDouble(giaVon);
                            if (isNumber7 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Giá vốn",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = giaVon,
                                    DienGiai = "Giá vốn không phải dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }
                        if (!string.IsNullOrEmpty(giaBan))
                        {
                            bool isNumber8 = CommonStatic.IsDouble(giaBan);
                            if (isNumber8 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Giá bán",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = giaBan,
                                    DienGiai = "Giá bán không phải dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }
                        if (!string.IsNullOrEmpty(tonKho))
                        {
                            bool isNumber8 = CommonStatic.IsDouble(tonKho);
                            if (isNumber8 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Tồn kho",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = tonKho,
                                    DienGiai = "Tồn kho không phải dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }
                        if (!string.IsNullOrEmpty(mdvtcoban) && string.IsNullOrEmpty(tenDonViTinh))
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Tên đơn vị tính",
                                ViTri = (rowIndex + 1).ToString(),
                                ThuocTinh = tenDonViTinh,
                                DienGiai = "Tên đơn vị tính không được để trống khi có mã đơn vị cơ bản",
                                rowError = rowIndex,
                                loaiError = 1
                            };
                            lstError.Add(DM);
                        }
                        if (!string.IsNullOrEmpty(mdvtcoban) && string.IsNullOrEmpty(gtriquydoi))
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Giá trị quy đổi",
                                ViTri = (rowIndex + 1).ToString(),
                                ThuocTinh = gtriquydoi,
                                DienGiai = "Giá trị quy đổi không được để trống khi có mã đơn vị cơ bản",
                                rowError = rowIndex,
                                loaiError = 1
                            };
                            lstError.Add(DM);
                        }
                        if (!string.IsNullOrEmpty(quyCach))
                        {
                            bool isNumber = CommonStatic.IsDouble(quyCach);
                            if (isNumber == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Quy cách",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = quyCach,
                                    DienGiai = "Quy cách không phải dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }
                        if (!string.IsNullOrEmpty(mdvtcoban))
                        {
                            if (mdvtcoban == maHangHoa)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Mã đơn vị tính cơ bản",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = mdvtcoban,
                                    DienGiai = "Mã đơn vị tính cơ bản trùng với mã hàng hóa",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }
                        if (!string.IsNullOrEmpty(gtriquydoi))
                        {
                            bool Isnumber13 = CommonStatic.IsDouble(gtriquydoi);
                            if (Isnumber13 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Giá trị quy đổi",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = gtriquydoi,
                                    DienGiai = "Giá trị quy đổi không phải là dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }
                    }
                }
            }
            return lstError;
        }

        public List<ErrorDMHangHoa> ImportDanhMucHangHoa_toDB(ISheet sheet, Guid idDonVi,
            Guid idnhanvien, int loaiUpdate = 1, string rowsErr = null)
        {
            var lstErr = new List<ErrorDMHangHoa>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                classDonViQuiDoi classDonViQD = new classDonViQuiDoi(db);
                ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);

                int[] arrRow = CommonStatic.GetArrIntDesc_fromString(rowsErr);

                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<BH_HoaDon_ChiTiet> lstCTKiemKe = new List<BH_HoaDon_ChiTiet>();
                        List<BH_HoaDon_ChiTiet> lstCTDCGV = new List<BH_HoaDon_ChiTiet>();

                        List<DM_GiaVon> lstDMGV = new List<DM_GiaVon>();
                        List<DM_HangHoa_TonKho> lstDMTonKho = new List<DM_HangHoa_TonKho>();

                        var dtNow = DateTime.Now;

                        BH_HoaDon hdDCGV = new BH_HoaDon
                        {
                            ID = Guid.NewGuid(),
                            MaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(18, idDonVi, DateTime.Now),
                            NgayLapHoaDon = dtNow,
                            ID_DonVi = idDonVi,
                            ID_NhanVien = idnhanvien,
                            NguoiTao = "admin",
                            NgayTao = DateTime.Now,
                            TongTienHang = 0,
                            TongChietKhau = 0,
                            TongTienThue = 0,
                            TongGiamGia = 0,
                            TongChiPhi = 0,
                            PhaiThanhToan = 0,
                            LoaiHoaDon = 18,
                            ChoThanhToan = false,
                            YeuCau = "Hoàn thành",
                            DienGiai = "Phiếu điều chỉnh được tạo tự động khi import hàng hóa",
                        };

                        BH_HoaDon hdKK = new BH_HoaDon
                        {
                            ID = Guid.NewGuid(),
                            MaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(9, idDonVi, DateTime.Now),
                            NgayLapHoaDon = dtNow.AddSeconds(1),
                            ID_DonVi = idDonVi,
                            ID_NhanVien = idnhanvien,
                            NguoiTao = "admin",
                            NgayTao = DateTime.Now,
                            TongTienHang = 0,
                            TongChietKhau = 0,
                            TongTienThue = 0,
                            TongGiamGia = 0,
                            TongChiPhi = 0,
                            PhaiThanhToan = 0,
                            LoaiHoaDon = 9,
                            ChoThanhToan = false,
                            DienGiai = "Phiếu kiểm kê được tạo tự động khi import hàng hóa",
                        };

                        double sumSLLech = 0, sumSLTang = 0, sumSLGiam = 0, sumGtriGiam = 0, sumGtriTang = 0, sumGtriLech = 0;

                        // duyệt bắt đầu từ dòng số 3
                        for (int rowIndex = 3; rowIndex < sheet.PhysicalNumberOfRows; rowIndex++)
                        {
                            if (arrRow.Contains(rowIndex))
                            {
                                continue;// nếu thuộc dòng lỗi: bỏ qua và next đến dòng tiếp theo
                            }
                            IRow row = sheet.GetRow(rowIndex);

                            if (row != null)
                            {
                                string nhomcha = row.GetCell(0)?.ToString();
                                string nhomcon = row.GetCell(1)?.ToString();
                                string mahanghoa = row.GetCell(2)?.ToString();
                                string tenhanghoa = row.GetCell(3)?.ToString();
                                string loaihang = row.GetCell(4)?.ToString().Trim();
                                string khongDuocBan = row.GetCell(5)?.ToString().Trim();
                                string ghichu = row.GetCell(6)?.ToString().Trim();
                                string giavon = row.GetCell(7)?.ToString().Trim();
                                string giaban = row.GetCell(8)?.ToString().Trim();
                                string tonkho = row.GetCell(9)?.ToString().Trim();
                                string tenDonViTinh = row.GetCell(10)?.ToString().Trim();
                                string quycach = row.GetCell(11)?.ToString().Trim();
                                string dvQuyCach = row.GetCell(12)?.ToString().Trim();
                                string mdvtcoban = row.GetCell(13)?.ToString().Trim();
                                string gtriquydoi = row.GetCell(14)?.ToString().Trim();
                                string macungloai = row.GetCell(15)?.ToString().Trim();

                                // bỏ qua dòng có dữ liệu trống
                                if (string.IsNullOrEmpty(nhomcon) && string.IsNullOrEmpty(mahanghoa)
                                    && string.IsNullOrEmpty(tenhanghoa)
                                    && string.IsNullOrEmpty(loaihang) && string.IsNullOrEmpty(giavon)
                                    && string.IsNullOrEmpty(giaban) && string.IsNullOrEmpty(tonkho))
                                {
                                    continue;
                                }

                                int loaiHangDB = 1;
                                switch (loaihang)
                                {
                                    case "Dịch vụ":
                                        loaiHangDB = 2;
                                        break;
                                    case "Combo":
                                        loaiHangDB = 3;
                                        break;
                                }

                                var duocbantructiep = string.IsNullOrEmpty(khongDuocBan);
                                var ladvchuan = string.IsNullOrEmpty(mdvtcoban);
                                var lahanghoa = string.IsNullOrEmpty(loaihang);
                                var tenhang_KhongDau = CommonStatic.ConvertToUnSign(tenhanghoa).ToLower();
                                var tenhang_KyTuDau = CommonStatic.GetCharsStart(tenhanghoa).ToLower();
                                var nhomcha_KhongDau = CommonStatic.ConvertToUnSign(nhomcha).ToLower();
                                var nhomcha_KyTuDau = CommonStatic.GetCharsStart(nhomcha).ToLower();
                                var nhomcon_KhongDau = CommonStatic.ConvertToUnSign(nhomcon).ToLower();
                                var nhomcon_KyTuDau = CommonStatic.GetCharsStart(nhomcon).ToLower();

                                double tonkhoDB = 0, giavonDB = 0;
                                double soluongNew = 0, tonluykeNew = 0, giavonThucTe = 0, giaBanNew = 0, quyCachNew = 1, gtriquydoiNew = 1;

                                if (!string.IsNullOrEmpty(tonkho))
                                {
                                    soluongNew = double.Parse(tonkho);
                                    tonluykeNew = soluongNew;
                                }
                                if (!string.IsNullOrEmpty(giaban))
                                {
                                    giaBanNew = double.Parse(giaban);
                                }
                                if (!string.IsNullOrEmpty(quycach))
                                {
                                    quyCachNew = double.Parse(quycach);
                                }
                                if (!string.IsNullOrEmpty(gtriquydoi))
                                {
                                    if (!ladvchuan)
                                    {
                                        gtriquydoiNew = double.Parse(gtriquydoi);
                                    }
                                }

                                if (!string.IsNullOrEmpty(giavon))
                                {
                                    giavonThucTe = double.Parse(giavon);
                                }

                                bool existHang = false, existTK = false, existGV = false;
                                var idQuiDoi = Guid.NewGuid();
                                var idHangHoa = Guid.NewGuid();
                                Guid? idGiaVon = null;

                                if (!string.IsNullOrEmpty(mahanghoa))
                                {
                                    mahanghoa = mahanghoa.ToUpper();
                                    DonViQuiDoi dmQuiDoi = classDonViQD.Select_DonViQuiDoi(mahanghoa);
                                    if (dmQuiDoi != null)
                                    {
                                        existHang = true;
                                        idQuiDoi = dmQuiDoi.ID;
                                        idHangHoa = dmQuiDoi.ID_HangHoa;

                                        List<SqlParameter> paramlist = new List<SqlParameter>();
                                        paramlist.Add(new SqlParameter("MaHH", mahanghoa));
                                        paramlist.Add(new SqlParameter("ID_ChiNhanh", idDonVi));
                                        paramlist.Add(new SqlParameter("ID_LoHang", DBNull.Value));
                                        // get tonho from DM_HangHoa_TonKho
                                        List<Search_HangHoa_importPRC> lstTon = db.Database.SqlQuery<Search_HangHoa_importPRC>("exec getList_DMHangHoa_Import @MaHH, @ID_ChiNhanh, @ID_LoHang", paramlist.ToArray()).ToList();
                                        if (lstTon != null && lstTon.Count() > 0)
                                        {
                                            tonkhoDB = lstTon.FirstOrDefault().TonCuoiKy;
                                            giavonDB = lstTon.FirstOrDefault().GiaVon;

                                            if (loaiHangDB == 1)
                                            {
                                                if (lstTon.FirstOrDefault().ID_GiaVon != null)
                                                {
                                                    existGV = true;
                                                    idGiaVon = lstTon.FirstOrDefault().ID_GiaVon;
                                                }
                                                if (lstTon.FirstOrDefault().ID_TonKho != null)
                                                {
                                                    existTK = true;
                                                }
                                            }
                                            else
                                            {
                                                existTK = true;
                                                existGV = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    mahanghoa = classDonViQD.GetMaHangHoa();
                                }

                                if (loaiHangDB == 1)
                                {
                                    DM_GiaVon dmGV = new DM_GiaVon
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DonVi = idDonVi,
                                        ID_DonViQuiDoi = idQuiDoi,
                                        GiaVon = giavonThucTe,
                                    };

                                    DM_HangHoa_TonKho dmTK = new DM_HangHoa_TonKho
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DonVi = idDonVi,
                                        ID_DonViQuyDoi = idQuiDoi,
                                        TonKho = soluongNew,
                                    };

                                    // neu nhieuDVT: tonkho = sum (tonkho all DVT)
                                    if (!ladvchuan && !string.IsNullOrEmpty(mdvtcoban))
                                    {
                                        var dvqdChuan = lstCTKiemKe.Where(x => x.MaHangHoa == mdvtcoban.ToUpper());
                                        var idQDChuan = Guid.Empty;
                                        if (dvqdChuan != null && dvqdChuan.Count() > 0)
                                        {
                                            tonluykeNew = dvqdChuan.FirstOrDefault().TonLuyKe ?? 0;
                                            idQDChuan = dvqdChuan.FirstOrDefault().ID_DonViQuiDoi;
                                        }
                                        tonluykeNew += soluongNew * gtriquydoiNew;

                                        // update again tonkho for DVT chuan
                                        lstDMTonKho.Where(x => x.ID_DonViQuyDoi == idQDChuan).ToList()
                                            .ForEach(x => x.TonKho = tonluykeNew);
                                        lstCTKiemKe.Where(x => x.ID_DonViQuiDoi == idQDChuan).ToList()
                                            .ForEach(x => x.TonLuyKe = tonluykeNew);
                                        dmTK.TonKho = tonluykeNew / gtriquydoiNew;

                                        // update again tonkho for dvt # (not dvChuan)
                                        var lstDVT = db.DonViQuiDois.Where(x => x.ID_HangHoa == idHangHoa && x.ID != idQDChuan)
                                            .Select(x => new { x.ID, x.TyLeChuyenDoi }).ToList();

                                        foreach (var item in lstDVT)
                                        {
                                            var tonkhoQD = tonluykeNew / item.TyLeChuyenDoi;
                                            lstDMTonKho.Where(x => x.ID_DonViQuyDoi == item.ID).ToList()
                                                 .ForEach(x => x.TonKho = tonkhoQD);
                                            lstCTKiemKe.Where(x => x.ID_DonViQuiDoi == item.ID).ToList()
                                                 .ForEach(x => x.TonLuyKe = tonkhoQD);
                                        }

                                        if (giavonThucTe == 0)
                                        {
                                            // get giavon dvChuan --> assign giavon for dvt #
                                            var gvChuan = lstCTDCGV.Where(x => x.MaHangHoa == mdvtcoban.ToUpper());
                                            if (gvChuan != null && gvChuan.Count() > 0)
                                            {
                                                giavonThucTe = (gvChuan.FirstOrDefault().GiaVon ?? 0) * gtriquydoiNew;
                                            }
                                            dmGV.GiaVon = giavonThucTe;
                                        }
                                    }

                                    if (!existGV)
                                    {
                                        lstDMGV.Add(dmGV);
                                    }
                                    else
                                    {
                                        DM_GiaVon gvEx = db.DM_GiaVon.Find(idGiaVon);
                                        gvEx.GiaVon = giavonThucTe;
                                    }
                                    if (!existTK)
                                    {
                                        lstDMTonKho.Add(dmTK);
                                    }
                                    // add to other chinhanh
                                    classHangHoa.AddMultiple_DMHangHoaTonKho(idDonVi, dmTK);
                                    classHangHoa.AddMultiple_DMGiaVon(idDonVi, dmGV);

                                    var lechGV = giavonThucTe - giavonDB;
                                    BH_HoaDon_ChiTiet dcGV = new BH_HoaDon_ChiTiet
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_HoaDon = hdDCGV.ID,
                                        ID_DonViQuiDoi = idQuiDoi,
                                        MaHangHoa = mahanghoa,
                                        DonGia = giavonDB,
                                        GiaVon = giavonThucTe,
                                        PTChietKhau = lechGV > 0 ? lechGV : 0,
                                        ThanhToan = lechGV < 0 ? lechGV : 0,
                                        // tonluyke in bh_chitiet: luu tonkho cua dvChuan
                                        TonLuyKe = loaiUpdate == 2 ? existTK ? tonkhoDB * gtriquydoiNew : 0 : tonkhoDB * gtriquydoiNew,
                                    };
                                    lstCTDCGV.Add(dcGV);

                                    var slLech = soluongNew - tonkhoDB;
                                    BH_HoaDon_ChiTiet ctKK = new BH_HoaDon_ChiTiet
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_HoaDon = hdKK.ID,
                                        ID_DonViQuiDoi = idQuiDoi,
                                        ID_HangHoa = idHangHoa,
                                        MaHangHoa = mahanghoa,
                                        SoLuong = slLech,
                                        GiaVon = giavonThucTe,
                                        ThanhTien = soluongNew,
                                        TienChietKhau = tonkhoDB,
                                        ThanhToan = giavonThucTe * slLech,
                                        TonLuyKe = loaiUpdate == 2 || existHang == false ? tonluykeNew : tonkhoDB * gtriquydoiNew,
                                    };
                                    // chi add kiemke if update tonkho
                                    if (loaiUpdate == 2 || existHang == false)
                                    {
                                        lstCTKiemKe.Add(ctKK);

                                        sumSLLech += slLech;
                                        sumSLTang += slLech > 0 ? slLech : 0;
                                        sumSLGiam += slLech < 0 ? slLech : 0;
                                        sumGtriTang += slLech > 0 ? slLech * giavonThucTe : 0;
                                        sumGtriGiam += slLech < 0 ? slLech * giavonThucTe : 0;
                                        sumGtriLech += slLech * giavonThucTe;
                                    }
                                }

                                #region param sql
                                List<SqlParameter> prmSQL = new List<SqlParameter>();
                                prmSQL.Add(new SqlParameter("isUpdateHang", existHang ? 1 : 0));
                                prmSQL.Add(new SqlParameter("isUpdateTonKho", loaiUpdate));// update TonKho (2.yes, 1.no)
                                prmSQL.Add(new SqlParameter("ID_DonVi", idDonVi));
                                prmSQL.Add(new SqlParameter("ID_HangHoa", idHangHoa));
                                prmSQL.Add(new SqlParameter("ID_DonViQuiDoi", idQuiDoi));
                                prmSQL.Add(new SqlParameter("TenNhomHangHoaCha", nhomcha ?? string.Empty));
                                prmSQL.Add(new SqlParameter("TenNhomHangHoaCha_KhongDau", nhomcha_KhongDau ?? string.Empty));
                                prmSQL.Add(new SqlParameter("TenNhomHangHoaCha_KyTuDau", nhomcha_KyTuDau ?? string.Empty));
                                prmSQL.Add(new SqlParameter("MaNhomHangHoaCha", DateTime.Now.ToString("yyyyMMddHHmmss")));

                                prmSQL.Add(new SqlParameter("TenNhomHangHoa", nhomcon ?? string.Empty));
                                prmSQL.Add(new SqlParameter("TenNhomHangHoa_KhongDau", nhomcon_KhongDau));
                                prmSQL.Add(new SqlParameter("TenNhomHangHoa_KyTuDau", nhomcon_KyTuDau));
                                prmSQL.Add(new SqlParameter("MaNhomHangHoa", DateTime.Now.ToString("yyyyMMddHHmmss")));
                                prmSQL.Add(new SqlParameter("LoaiHangHoa", loaiHangDB));
                                prmSQL.Add(new SqlParameter("TenHangHoa", tenhanghoa));
                                prmSQL.Add(new SqlParameter("TenHangHoa_KhongDau", tenhang_KhongDau));
                                prmSQL.Add(new SqlParameter("TenHangHoa_KyTuDau", tenhang_KyTuDau));
                                prmSQL.Add(new SqlParameter("GhiChu", ghichu ?? string.Empty));
                                prmSQL.Add(new SqlParameter("QuyCach", quyCachNew));
                                prmSQL.Add(new SqlParameter("DuocBanTrucTiep", duocbantructiep));

                                prmSQL.Add(new SqlParameter("MaDonViCoBan", mdvtcoban ?? string.Empty));
                                prmSQL.Add(new SqlParameter("MaHangHoa", mahanghoa));
                                prmSQL.Add(new SqlParameter("TenDonViTinh", tenDonViTinh ?? string.Empty));
                                prmSQL.Add(new SqlParameter("GiaVon", giavonThucTe));
                                prmSQL.Add(new SqlParameter("GiaBan", giaBanNew));
                                prmSQL.Add(new SqlParameter("TonKho", lahanghoa ? tonluykeNew / gtriquydoiNew : 0));
                                prmSQL.Add(new SqlParameter("LaDonViChuan", ladvchuan));
                                prmSQL.Add(new SqlParameter("TyLeChuyenDoi", gtriquydoiNew));
                                prmSQL.Add(new SqlParameter("MaHangHoaChaCungLoai", macungloai ?? string.Empty));
                                prmSQL.Add(new SqlParameter("DVTQuyCach", dvQuyCach ?? string.Empty));

                                db.Database.ExecuteSqlCommand("Exec import_DanhMucHangHoa @isUpdateHang, @isUpdateTonKho, @ID_DonVi, @ID_HangHoa, @ID_DonViQuiDoi," +
                                    " @TenNhomHangHoaCha,@TenNhomHangHoaCha_KhongDau,@TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, " +
                                    "@TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau," +
                                     "@MaNhomHangHoa, @LoaiHangHoa, @TenHangHoa, @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau," +
                                     "@GhiChu, @QuyCach, @DuocBanTrucTiep, @MaDonViCoBan, @MaHangHoa, @TenDonViTinh, @GiaVon, @GiaBan, @TonKho," +
                                     " @LaDonViChuan, @TyLeChuyenDoi, @MaHangHoaChaCungLoai,@DVTQuyCach", prmSQL.ToArray());
                                #endregion

                                #region ThuocTinh HangHoa: nên tách ra thành 1 file # để import riêng (todo)
                                //int m = 0;
                                //for (int j = 16; j < dataTable.Columns.Count; j++)
                                //{
                                //    if (dtRow[j].ToString().Trim() != "")
                                //    {
                                //        string TenThuocTinh = string.Empty;
                                //        try
                                //        {
                                //            TenThuocTinh = lst[j - 16].ToString();
                                //        }
                                //        catch
                                //        {
                                //            TenThuocTinh = "Thuộc tính " + (j - 15).ToString();
                                //        }
                                //        List<SqlParameter> parama = new List<SqlParameter>();
                                //        parama.Add(new SqlParameter("TenThuocTinh", TenThuocTinh));
                                //        parama.Add(new SqlParameter("GiaTri", dtRow[j].ToString().Trim()));
                                //        parama.Add(new SqlParameter("ThuTuNhap", m));
                                //        parama.Add(new SqlParameter("MaHangHoa", mahanghoa));
                                //        m++;
                                //        db.Database.ExecuteSqlCommand("Exec import_HangHoaThuocTinh @TenThuocTinh, @GiaTri, @ThuTuNhap, @MaHangHoa", parama.ToArray());
                                //    }
                                //}
                                #endregion
                            }
                        }

                        hdKK.TongGiamGia = sumSLLech;
                        hdKK.TongChiPhi = sumSLTang;
                        hdKK.TongTienHang = sumSLGiam;
                        hdKK.TongChietKhau = sumGtriGiam;
                        hdKK.PhaiThanhToan = sumGtriTang;
                        hdKK.TongTienThue = sumGtriLech;

                        if (lstCTDCGV.Count() > 0)
                        {
                            db.BH_HoaDon.Add(hdDCGV);
                        }
                        if (lstCTKiemKe.Count() > 0)
                        {
                            db.BH_HoaDon.Add(hdKK);
                        }

                        db.BH_HoaDon_ChiTiet.AddRange(lstCTKiemKe);
                        db.BH_HoaDon_ChiTiet.AddRange(lstCTDCGV);
                        db.DM_HangHoa_TonKho.AddRange(lstDMTonKho);
                        db.DM_GiaVon.AddRange(lstDMGV);
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        lstErr.Add(new ErrorDMHangHoa()
                        {
                            TenTruongDuLieu = "Exception",
                            ViTri = "0",
                            rowError = -1,
                            loaiError = 1,
                            ThuocTinh = "Exception",
                            DienGiai = ex.InnerException + ex.Message,
                        });
                        trans.Rollback();
                    }
                }
            }

            return lstErr;
        }
        #endregion
        #region Import Customer
        public List<ErrorDMHangHoa> CheckData_FileImportCustomer(ISheet sheet, Guid idDonvi, Guid idnhanvien, int loaiUpdate = 1, string rowsErr = null)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Dictionary<string, List<int>> maKHTracker = new Dictionary<string, List<int>>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classDMDoiTuong = new classDM_DoiTuong(db);
                Class_officeDocument class_OfficeDocument = new Class_officeDocument(db);
                var lastRow = sheet.PhysicalNumberOfRows;
                System.Data.DataTable dataTable = new System.Data.DataTable();
                for (int col = 0; col < sheet.GetRow(2).LastCellNum; col++)
                {
                    dataTable.Columns.Add("Column" + col);
                }
                for (int rowIndex = 3; rowIndex < lastRow; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row != null)
                    {
                        string tenNhomKhachHang = row.GetCell(1)?.ToString();
                        string nguonKhach = row.GetCell(2)?.ToString();
                        string statusKhach = row.GetCell(3)?.ToString();
                        string maKH = row.GetCell(4)?.ToString();
                        string tenKhachHang = row.GetCell(5)?.ToString();
                        string gender = row.GetCell(6)?.ToString().Trim();
                        string loaiKhach = row.GetCell(7)?.ToString().Trim();
                        string ngaySinh = row.GetCell(8)?.ToString().Trim();
                        string diachi = row.GetCell(9)?.ToString().Trim();
                        string email = row.GetCell(10)?.ToString().Trim();
                        string soDienThoai = row.GetCell(11)?.ToString().Trim();
                        string note = row.GetCell(12)?.ToString().Trim();
                        string maSoThue = row.GetCell(13)?.ToString().Trim();
                        string noCanThu = row.GetCell(14)?.ToString().Trim();
                        string noCanTra = row.GetCell(15)?.ToString().Trim();
                        string sumTichDiem = row.GetCell(16)?.ToString().Trim();
                        string soDu = row.GetCell(17)?.ToString().Trim();

                        // Kiểm tra nếu tất cả các giá trị đều rỗng hoặc null
                        if (string.IsNullOrEmpty(tenNhomKhachHang) && string.IsNullOrEmpty(nguonKhach) &&
                            string.IsNullOrEmpty(statusKhach) && string.IsNullOrEmpty(maKH) &&
                            string.IsNullOrEmpty(tenKhachHang) && string.IsNullOrEmpty(gender) &&
                            string.IsNullOrEmpty(soDienThoai))
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(maKH))
                        {
                            if (maKHTracker.ContainsKey(maKH))
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Mã khách hàng",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = maKH,
                                    DienGiai = "Mã khách hàng: " + maKH + " bị trùng lặp",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                            else
                            {
                                maKHTracker[maKH] = new List<int> { rowIndex + 1 };
                            }

                            // Kiểm tra sự tồn tại của mã khách hàng trong cơ sở dữ liệu
                            var checkExist = classDMDoiTuong.Get(x => x.MaDoiTuong == maKH && x.ID_DonVi == idDonvi) != null;
                            if (checkExist)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Mã khách hàng",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = maKH,
                                    DienGiai = "Mã khách hàng: " + maKH + " đã tồn tại",
                                    rowError = rowIndex,
                                    loaiError = 1
                                };
                                lstError.Add(DM);
                            }
                        }

                        // Kiểm tra tên khách hàng
                        if (string.IsNullOrEmpty(tenKhachHang))
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Tên khách hàng",
                                ViTri = (rowIndex + 1).ToString(),
                                ThuocTinh = tenKhachHang,
                                DienGiai = "Tên khách hàng không được để trống",
                                rowError = rowIndex,
                                loaiError = 1
                            });
                        }

                        // Kiểm tra giới tính
                        if (gender != "x" && gender != "")
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Giới tính",
                                ViTri = (rowIndex + 1).ToString(),
                                ThuocTinh = gender,
                                DienGiai = "Giới tính là Nam: bạn cần đánh dấu x",
                                rowError = rowIndex,
                                loaiError = 1
                            });
                        }

                        // Kiểm tra loại khách
                        if (loaiKhach != "x" && loaiKhach != "")
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Loại khách",
                                ViTri = (rowIndex + 1).ToString(),
                                ThuocTinh = loaiKhach,
                                DienGiai = "Là công ty: bạn cần đánh dấu x",
                                rowError = rowIndex,
                                loaiError = 1
                            });
                        }

                        // Kiểm tra ngày sinh
                        if (!string.IsNullOrEmpty(ngaySinh))
                        {
                            bool valiDateNgaySinh = class_OfficeDocument.ValidateDateTime(ngaySinh);
                            if (!valiDateNgaySinh)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Ngày sinh/thành lập",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = ngaySinh,
                                    DienGiai = "Ngày sinh/thành lập không hợp lệ",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(ngaySinh);
                                    if (dateTime > DateTime.Now)
                                    {
                                        lstError.Add(new ErrorDMHangHoa
                                        {
                                            TenTruongDuLieu = "Ngày sinh/thành lập",
                                            ViTri = (rowIndex + 1).ToString(),
                                            ThuocTinh = ngaySinh,
                                            DienGiai = "Ngày sinh/thành lập không được lớn hơn ngày hiện tại",
                                            rowError = rowIndex,
                                            loaiError = 1
                                        });
                                    }
                                }
                                catch
                                {
                                    lstError.Add(new ErrorDMHangHoa
                                    {
                                        TenTruongDuLieu = "Ngày sinh/thành lập",
                                        ViTri = (rowIndex + 1).ToString(),
                                        ThuocTinh = ngaySinh,
                                        DienGiai = "Ngày sinh/thành lập không hợp lệ",
                                        rowError = rowIndex,
                                        loaiError = 1
                                    });
                                }
                            }
                        }

                        // Kiểm tra email
                        if (!string.IsNullOrEmpty(email))
                        {
                            bool valiDateEmail = class_OfficeDocument.ValidateEmail(email);
                            if (!valiDateEmail)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Email",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = email,
                                    DienGiai = "Email không hợp lệ",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }
                            bool duplicateEmail = class_OfficeDocument.GroupData(dataTable, "Email = '" + email + "'");
                            if (!duplicateEmail)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Email",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = email,
                                    DienGiai = "Email bị trùng lặp",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }

                            var existEmail = classDMDoiTuong.Get(x => x.Email.Contains(email));
                            if (existEmail != null)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Email",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = email,
                                    DienGiai = "Email đã tồn tại",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }
                        }
                        int chophepTrungSDT = 0;
                        // Kiểm tra số điện thoại
                        if (!string.IsNullOrEmpty(soDienThoai))
                        {
                            if (chophepTrungSDT == 0)
                            {
                                bool duplicateSDT = class_OfficeDocument.GroupData(dataTable, "DienThoai = '" + soDienThoai + "'");
                                if (!duplicateSDT)
                                {
                                    lstError.Add(new ErrorDMHangHoa
                                    {
                                        TenTruongDuLieu = "Điện thoại",
                                        ViTri = (rowIndex + 1).ToString(),
                                        ThuocTinh = soDienThoai,
                                        DienGiai = "Số điện thoại: " + soDienThoai + " bị trùng lặp",
                                        rowError = rowIndex,
                                        loaiError = 1
                                    });
                                }
                            }

                            bool checkSDT = classDMDoiTuong.SP_CheckSoDienThoai_Exist(soDienThoai);
                            if (checkSDT)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Điện thoại",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = soDienThoai,
                                    DienGiai = "Số điện thoại: " + soDienThoai + " đã tồn tại trong cơ sở dữ liệu",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }
                        }

                        // Kiểm tra nợ cần thu
                        if (!string.IsNullOrEmpty(noCanThu))
                        {
                            bool isNumber = class_OfficeDocument.IsNumberInt(noCanThu);
                            if (!isNumber)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Nợ cần thu",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = noCanThu,
                                    DienGiai = "Nợ cần thu không phải dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }
                        }

                        // Kiểm tra nợ cần trả
                        if (!string.IsNullOrEmpty(noCanTra))
                        {
                            bool isNumber = class_OfficeDocument.IsNumberInt(noCanTra);
                            if (!isNumber)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Nợ cần trả",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = noCanTra,
                                    DienGiai = "Nợ cần trả không phải dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }
                        }

                        // Kiểm tra tổng tích điểm
                        if (!string.IsNullOrEmpty(sumTichDiem))
                        {
                            bool isNumber = class_OfficeDocument.IsNumberInt(sumTichDiem);
                            if (!isNumber)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Tổng tích điểm",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = sumTichDiem,
                                    DienGiai = "Tổng tích điểm không phải dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }
                        }

                        // Kiểm tra số dư thẻ
                        if (!string.IsNullOrEmpty(soDu))
                        {
                            bool isNumber = class_OfficeDocument.IsNumberInt(soDu);
                            if (!isNumber)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Số dư thẻ",
                                    ViTri = (rowIndex + 1).ToString(),
                                    ThuocTinh = soDu,
                                    DienGiai = "Số dư thẻ không phải dạng số",
                                    rowError = rowIndex,
                                    loaiError = 1
                                });
                            }
                        }
                    }
                }
            }
            return lstError;
        }
        public List<ErrorDMHangHoa> ImportDangMucKhachHang_toDB(ISheet sheet, Guid idDonVi,
    Guid idnhanvien, int loaiUpdate = 1, string rowsErr = null)
        {
            var lstErr = new List<ErrorDMHangHoa>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa classDMHangHoa = new ClassDM_HangHoa(db);
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                classDonViQuiDoi classDonViQD = new classDonViQuiDoi(db);
                ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);
                classDM_DoiTuong classDM_DoiTuong = new classDM_DoiTuong(db);
                int[] arrRow = CommonStatic.GetArrIntDesc_fromString(rowsErr);
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<BH_HoaDon_ChiTiet> lstCTKiemKe = new List<BH_HoaDon_ChiTiet>();
                        List<BH_HoaDon_ChiTiet> lstCTDCGV = new List<BH_HoaDon_ChiTiet>();

                        List<DM_GiaVon> lstDMGV = new List<DM_GiaVon>();
                        List<DM_HangHoa_TonKho> lstDMTonKho = new List<DM_HangHoa_TonKho>();

                        var dtNow = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        lstErr.Add(new ErrorDMHangHoa()
                        {
                            TenTruongDuLieu = "Exception",
                            ViTri = "0",
                            rowError = -1,
                            loaiError = 1,
                            ThuocTinh = "Exception",
                            DienGiai = ex.InnerException + ex.Message,
                        });
                        trans.Rollback();
                    }
                }
            }
            return lstErr;
            #endregion
        }
    }
}
