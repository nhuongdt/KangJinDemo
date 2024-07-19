using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Aspose.Cells;
using Model;
using System.Text.RegularExpressions;
using libDonViQuiDoi;
using System.Data.Entity.Validation;
using libDM_DoiTuong;
using libDM_NhomDoiTuong;
using libDM_HangHoa;
using System.Data.SqlClient;
using System.Drawing.Printing;
using static libQuy_HoaDon.Class_Report;
using System.Diagnostics.Eventing.Reader;
using libNS_NhanVien;
using System.Runtime.Remoting;

namespace libQuy_HoaDon
{
    public class Class_officeDocument
    {
        private SsoftvnContext db;

        public Class_officeDocument(SsoftvnContext _db)
        {
            db = _db;
        }
        public System.Data.DataTable ToDataTable<T>(List<T> data)
        {

            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (var item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public void listToOfficeExcelLoHang(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time)
        {
            //sourceRowIndex = 3
            //destinationRowIndex = 27
            //rowNumber = 24
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];

            wSheet.Cells[1, 0].Value = "Ngày hết hạn: " + time;
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void ListToOfficeExcel(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu,
            int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, Param_ReportText param)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];

            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }


            // Tạo một đối tượng ImportTableOptions và thiết lập các tùy chọn
            ImportTableOptions importTableOptions = new ImportTableOptions();

            // Nhập DataTable vào worksheet, bắt đầu từ hàng 0 và cột 0
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);

            //wSheet.Cells.ImportDataTable(tblDuLieu,  sourceRowIndex, 0,0);

            if (param.ColumnHide != null && param.ColumnHide.Count > 0)
            {
                for (int i = param.ColumnHide.Count - 1; i >= 0; i--)
                {
                    wSheet.Cells.DeleteColumn(param.ColumnHide[i]);
                }
            }
            wSheet.Cells[0, 0].Value = param.ReportName.ToUpper();
            wSheet.Cells[1, 0].Value = param.ReportBranch;
            wSheet.Cells[2, 0].Value = param.ReportTime;

            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void DataToExcel_WithText(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu,
            int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, List<CellDTO> lstCell)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];

            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);

            foreach (var cell in lstCell)
            {
                wSheet.Cells[cell.Row, cell.Column].Value = cell.Text;
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void listToOfficeExcel(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void listToOfficeExcel_v2(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, List<int> columnsHide)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);

            for (int i = columnsHide.Count - 1; i >= 0; i--)
            {
                wSheet.Cells.DeleteColumn(columnsHide[i]);
            }

            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void listToOfficeExcel_v2(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, List<int> columnsHide, string value1, string value2)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[1, 0].Value = value1;
            wSheet.Cells[2, 0].Value = value2;
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);

            for (int i = columnsHide.Count - 1; i >= 0; i--)
            {
                wSheet.Cells.DeleteColumn(columnsHide[i]);
            }

            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }
        public void listToOfficeExcel_ToSheet1(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[1];
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void listToOfficeExcelNhapHang(string strFileTemplatePath, string exportPath, double giatri, int rown, int columns)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            // định dạng
            wSheet.Cells[rown, columns].Value = giatri;
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void listToOfficeExcel_StypeSQ(string strFileTemplatePath, string exportPath,
            System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber,
            bool checkSum, string columnsHide, string time, string ChiNhanh, double tongthu, double tongchi, double tonquy, double? tonDauKy = 0)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];

            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }

            wSheet.Cells[1, 0].Value = "Thời gian: ";
            wSheet.Cells[1, 1].Value = time;

            wSheet.Cells[2, 0].Value = "Chi nhánh: ";
            wSheet.Cells[2, 1].Value = ChiNhanh;

            wSheet.Cells[3, 0].Value = "Tồn đầu kỳ: ";
            wSheet.Cells[3, 1].Value = tonDauKy;

            wSheet.Cells[4, 0].Value = "Thu trong kỳ: ";
            wSheet.Cells[4, 1].Value = tongthu;

            wSheet.Cells[5, 0].Value = "Chi trong kỳ: ";
            wSheet.Cells[5, 1].Value = tongchi;

            wSheet.Cells[6, 0].Value = "Tồn trong kỳ: ";
            wSheet.Cells[6, 1].Value = tonquy;

            wSheet.Cells[7, 0].Value = "Tồn cuối kỳ: ";
            wSheet.Cells[7, 1].Value = tonDauKy + tonquy;

            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }
        public void listToOfficeExcelChiTiet_Stype(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time, string ChiNhanh, string chitiet, int vitri)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[1, 0].Value = time;
            wSheet.Cells[2, 0].Value = "Chi nhánh: " + ChiNhanh;
            wSheet.Cells[2, vitri].Value = chitiet;
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }
        public string createFolder_Download(string fileSave)
        {
            string[] files = fileSave.Split('\\');
            string str = CookieStore.GetCookieAes("SubDomain");
            string id_nd = CookieStore.GetCookieAes("id_nguoidung");
            string file = files[files.Length - 1].ToString();
            string path = fileSave.Replace(file, @"Download\" + str + @"\" + id_nd + @"\" + file);
            string mapPath = fileSave.Replace(file, @"Download\" + str + @"\" + id_nd);
            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }
            return path;
        }
        public string createFolder_Export(string fileSave)
        {
            string[] files = fileSave.Split('/');
            string str = CookieStore.GetCookieAes("SubDomain");
            string id_nd = CookieStore.GetCookieAes("id_nguoidung");
            string file = files[files.Length - 1].ToString();
            string path = fileSave.Replace(file, "Download/" + str + "/" + id_nd + "/" + file);
            return path;
        }
        public string createFolder_Print(string fileSave)
        {
            string[] files = fileSave.Split('\\');
            string str = CookieStore.GetCookieAes("SubDomain");
            string file = files[files.Length - 1].ToString();
            string path = fileSave.Replace(file, "Print\\" + str + "\\" + file);
            string mapPath = fileSave.Replace(file, "Print\\" + str);
            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }
            return path;
        }
        public string getPath_WebPrint(string fileSave)
        {
            string[] files = fileSave.Split('/');
            string str = CookieStore.GetCookieAes("SubDomain");
            string file = files[files.Length - 1].ToString();
            string path = fileSave.Replace(file, "Print/" + str + "/" + file);
            return path;
        }
        public void listToOfficeExcel_Stype(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu,
            int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time, string ChiNhanh)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];

            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            // write after remove column
            wSheet.Cells[1, 0].Value = time;
            if (ChiNhanh != "" && ChiNhanh != null)
            {
                wSheet.Cells[2, 0].Value = "Chi nhánh: " + ChiNhanh;
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void ExportExcelToFileChamCong(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu,
          int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time, string KyTinhCong,
          string ChiNhanh, int[] ColumnPan, int[] listRowPan, string phongBan = null)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[1, 0].Value = KyTinhCong;
            wSheet.Cells[2, 0].Value = ChiNhanh;
            wSheet.Cells[3, 0].Value = phongBan;
            wSheet.Cells[4, 6].Value = time;
            int firstrow = sourceRowIndex;
            for (int i = 0; i < listRowPan.Length; i++)
            {
                for (int j = 0; j < ColumnPan.Length; j++)
                {
                    wSheet.Cells.Merge(firstrow, ColumnPan[j], listRowPan[i], 1);
                }
                firstrow += listRowPan[i];
            }
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }
        public void ExportExcelToFileBangLuongCT(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu,
          int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string KyTinhCong,
          string TenBangLuong, int[] ColumnPan, int[] listRowPan)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[2, 0].Value = KyTinhCong;
            wSheet.Cells[1, 0].Value = TenBangLuong;
            int firstrow = sourceRowIndex;
            for (int i = 0; i < listRowPan.Length; i++)
            {
                for (int j = 0; j < ColumnPan.Length; j++)
                {
                    wSheet.Cells.Merge(firstrow, ColumnPan[j], listRowPan[i], 1);
                }
                firstrow += listRowPan[i];
            }
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }
        public void ExportExcelToFileBangCong(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu,
                                             int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time,
                                             string KyTinhCong, string NhanVien, int[] ColumnPan, int[] listRowPan)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[1, 0].Value = NhanVien;
            wSheet.Cells[2, 0].Value = KyTinhCong;
            wSheet.Cells[3, 2].Value = time;
            int firstrow = sourceRowIndex;
            for (int i = 0; i < listRowPan.Length; i++)
            {
                for (int j = 0; j < ColumnPan.Length; j++)
                {
                    wSheet.Cells.Merge(firstrow, ColumnPan[j], listRowPan[i], 1);
                }
                firstrow += listRowPan[i];
            }
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void ExportExcel_RemoveColumn(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu,
           int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time, string ChiNhanh)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[1, 0].Value = time;
            if (ChiNhanh != "" && ChiNhanh != null)
            {
                wSheet.Cells[2, 0].Value = "Chi nhánh: " + ChiNhanh;
            }
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                // xóa cột có index lớn nhất trước
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i], false);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void listToOfficeExcel_PhanTichThuChi(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time, string ChiNhanh, int TongThu, int TongChi)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[1, 0].Value = time;
            if (ChiNhanh != "" && ChiNhanh != null)
            {
                wSheet.Cells[2, 0].Value = "Chi nhánh: " + ChiNhanh;
            }
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            Style style = wbook.CreateStyle();
            style.Font.IsBold = true;
            style.Font.IsItalic = false;
            StyleFlag flag = new StyleFlag();
            flag.FontBold = true;
            flag.FontItalic = false;
            wSheet.Cells.Rows[TongThu].ApplyStyle(style, flag);
            wSheet.Cells.Rows[TongChi].ApplyStyle(style, flag);
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }
        public void insert_Values(string strFileTemplatePath, int row, int colum, string giatri)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[row, colum].Value = giatri;
            wbook.Save(strFileTemplatePath);
        }
        public void ConvertToHTML(string strFileTemplatePath, String fileSave)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            wbook.Save(fileSave, Aspose.Cells.SaveFormat.Html);
        }
        public void listToOfficeExcel_Stype_HTML(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time, string ChiNhanh, double a1, double a2, double a3, double a4, double a5, double a6, double a7, double a8, double a9, double a10, double a11, double a12, double a13, double a14, double a15, int table)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[1, 0].Value = time;
            wSheet.Cells[2, 0].Value = "Chi nhánh: " + ChiNhanh;
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            if (table == 1)
            {
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 3].Value = String.Format("{0:N}", a1);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 4].Value = String.Format("{0:N0}", a2);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 5].Value = String.Format("{0:N0}", a3);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 6].Value = String.Format("{0:N0}", a4);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 7].Value = String.Format("{0:N0}", a5);
            }
            else if (table == 2)
            {
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 6].Value = String.Format("{0:N}", a1);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 9].Value = String.Format("{0:N0}", a2);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 10].Value = String.Format("{0:N0}", a3);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 12].Value = String.Format("{0:N0}", a4);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 13].Value = String.Format("{0:N0}", a5);
            }
            else if (table == 3)
            {
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 1].Value = String.Format("{0:N}", a1);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 2].Value = String.Format("{0:N0}", a2);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 3].Value = String.Format("{0:N0}", a3);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 4].Value = String.Format("{0:N0}", a4);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 5].Value = String.Format("{0:N0}", a5);
            }
            else if (table == 4)
            {
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 4].Value = String.Format("{0:N}", a1);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 5].Value = String.Format("{0:N0}", a2);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 6].Value = String.Format("{0:N0}", a3);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 7].Value = String.Format("{0:N0}", a4);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 8].Value = String.Format("{0:N0}", a5);
            }
            else if (table == 5)
            {
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 1].Value = String.Format("{0:N}", a1);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 2].Value = String.Format("{0:N0}", a2);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 3].Value = String.Format("{0:N}", a3);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 4].Value = String.Format("{0:N0}", a4);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 5].Value = String.Format("{0:N0}", a5);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 6].Value = String.Format("{0:N0}", a6);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 7].Value = String.Format("{0:N0}", a7);
            }
            else if (table == 6)
            {
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 5].Value = String.Format("{0:N}", a1);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 6].Value = String.Format("{0:N0}", a2);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 7].Value = String.Format("{0:N0}", a3);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 8].Value = String.Format("{0:N0}", a4);
            }
            else if (table == 7)
            {
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 2].Value = String.Format("{0:N}", a1);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 3].Value = String.Format("{0:N0}", a2);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 4].Value = String.Format("{0:N}", a3);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 5].Value = String.Format("{0:N0}", a4);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 6].Value = String.Format("{0:N0}", a5);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 7].Value = String.Format("{0:N0}", a6);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 8].Value = String.Format("{0:N0}", a7);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 7].Value = String.Format("{0:N0}", a8);
                wSheet.Cells[tblDuLieu.Rows.Count + 4, 8].Value = String.Format("{0:N0}", a9);
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Html);
            //wbook.Save(exportPath, Aspose.Cells.SaveFormat.Pdf);
        }



        public void listToOfficeExcel_StypeSQ(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, string time, string ChiNhanh, int x, double GiaTri)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            wSheet.Cells[1, 0].Value = time;
            wSheet.Cells[2, 0].Value = "Chi nhánh: " + ChiNhanh;
            wSheet.Cells[2, x].Value = GiaTri;
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                //Chèn chữ ký
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(o => o != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(o => o).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }
        public void listToOfficeExcel_Sheet(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex,
            int destinationRowIndex, int rowNumber, bool checkSum, string columnsHide, int sheet, string time, string ChiNhanh)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[sheet];
            // định dạng
            wSheet.Cells[1, 0].Value = time;
            wSheet.Cells[2, 0].Value = "Chi nhánh: " + ChiNhanh;
            //wSheet.Cells[1, 0].Value = "Thời gian: " + time;
            //wSheet.Cells[2, 0].Value = "Chi nhánh: " + ChiNhanh;
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);
            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void listToOfficeExcel_Sheet_KH(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu,
            int sourceRowIndex, int destinationRowIndex, int rowNumber, bool checkSum, int sheet, string columnsHide, string txtValue1, string txtValue2)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[sheet];

            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange >= 1)
            {
                if (checkSum)
                {
                    wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex, tblDuLieu.Rows.Count + sourceRowIndex, 10);
                }
            }
            if (dkrange < 1)
            {
                wSheet.Cells.DeleteRows(sourceRowIndex + tblDuLieu.Rows.Count, rowNumber - tblDuLieu.Rows.Count);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            // Tạo một đối tượng ImportTableOptions và thiết lập các tùy chọn
            // Nhập DataTable vào worksheet, bắt đầu từ hàng 0 và cột 0
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);

            if (columnsHide != null & columnsHide != "null" & columnsHide != "")
            {
                string[] coloumHide = columnsHide.Split('_');
                coloumHide = coloumHide.Where(x => x != "").Distinct().ToArray();
                var columH = Array.ConvertAll(coloumHide, int.Parse).OrderByDescending(x => x).ToArray();
                for (int i = 0; i < columH.Length; i++)
                {
                    wSheet.Cells.DeleteColumn(columH[i]);
                }
            }
            // định dạng
            wSheet.Cells[2, 1].Value = txtValue1;
            wSheet.Cells[3, 1].Value = txtValue2;
            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void downloadFile(string filePath)
        {
            HttpResponse Response = HttpContext.Current.Response;
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "text/plain";
                Response.TransmitFile(file.FullName);
                Response.End();
            }
        }
        public void importExceltoDatabase(string fileExcelImport)
        {
            Workbook workbook = new Workbook(fileExcelImport);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn;
            DataTable dt = worksheet.Cells.ExportDataTable(0, 0, trows, tcool, true);
        }
        public string CheckFileMau(Stream fileInput, int gioihanHH)
        {
            string str = Check_WaitingImport();
            if (str == null)
            {
                Workbook objWorkbook = new Workbook(fileInput);
                Worksheet worksheet = objWorkbook.Worksheets[0];
                //int tcool = worksheet.Cells.MaxColumn;
                //int trows = worksheet.Cells.MaxDataRow;
                //DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool, true);
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                ExportTableOptions options = new ExportTableOptions();
                options.ExportColumnName = true;
                options.IsVertical = true;
                options.ExportAsString = true;
                DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                try
                {
                    if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT DANH MỤC HÀNG HÓA" || worksheet.Cells.Rows[2][3].Value.ToString() != "Tên hàng/dịch vụ")
                    {
                        str = "Vui lòng import file đúng định dạng";
                    }
                    else
                    {
                        if (gioihanHH > 0)
                        {
                            var lst = from hh in db.DM_HangHoa
                                      join dvqd in db.DonViQuiDois on hh.ID equals dvqd.ID_HangHoa
                                      where dvqd.Xoa == null
                                      select new DM_HangHoaDTO
                                      {
                                          ID = hh.ID
                                      };
                            int gioihan = 0;
                            if (lst != null)
                                gioihan = lst.Count();

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string dk = "";
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    if (dt.Rows[i][j].ToString() != "")
                                    {
                                        break;
                                    }
                                    if (j == dt.Columns.Count - 1)
                                    {
                                        dk = "1";
                                    }
                                }
                                if (dk == "")
                                {
                                    string CheckCSDL = ChekMaHangDatabase_Update(dt.Rows[i][2].ToString());
                                    if (CheckCSDL == string.Empty)
                                    {
                                        gioihan = gioihan + 1;
                                    }
                                    if (gioihan > gioihanHH)
                                    {
                                        str = "File dữ liệu vượt quá giới hạn số lượng hàng hóa cho phép";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    str = "Vui lòng import file đúng định dạng";
                }
                if (worksheet.Cells.Rows.Count < 4)
                {
                    str = "File import không có dữ liệu";
                }
            }
            return str;
        }
        public string Check_WaitingImport()
        {
            string rt = null;
            try
            {
                var tb = from hd in db.BH_HoaDon
                         where hd.SoLanIn == -9
                         select new
                         {
                             ID = hd.ID
                         };
                if (tb.Count() > 0)
                    rt = "Tiến trình Import Hàng hóa đang được thực hiện. Vui lòng thử lại sau";
            }
            catch (Exception EX)
            {
                rt = EX.ToString();
            }
            return rt;
        }
        public string CheckFileMau_LoHang(Stream fileInput, int gioihanHH)
        {
            string str = Check_WaitingImport();
            if (str == null)
            {
                Workbook objWorkbook = new Workbook(fileInput);
                Worksheet worksheet = objWorkbook.Worksheets[0];
                //int tcool = worksheet.Cells.MaxColumn;
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;

                ExportTableOptions options = new ExportTableOptions();
                options.ExportColumnName = true;
                options.IsVertical = true;
                options.ExportAsString = true;
                DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                try
                {
                    if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT DANH MỤC HÀNG HÓA" || worksheet.Cells.Rows[2][6].Value.ToString() != "Tên hàng/dịch vụ")
                    {
                        str = "Vui lòng import file đúng định dạng";
                    }
                    else
                    {
                        if (gioihanHH > 0)
                        {
                            var lst = from hh in db.DM_HangHoa
                                      join dvqd in db.DonViQuiDois on hh.ID equals dvqd.ID_HangHoa
                                      where dvqd.Xoa == null
                                      select new DM_HangHoaDTO
                                      {
                                          ID = hh.ID
                                      };
                            int gioihan = 0;
                            if (lst != null)
                                gioihan = lst.Count();

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string dk = "";
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    if (dt.Rows[i][j].ToString() != "")
                                    {
                                        break;
                                    }
                                    if (j == dt.Columns.Count - 1)
                                    {
                                        dk = "1";
                                    }
                                }
                                if (dk == "")
                                {
                                    string CheckCSDL = ChekMaHangDatabase_Update(dt.Rows[i][5].ToString());
                                    if (CheckCSDL == string.Empty)
                                    {
                                        gioihan = gioihan + 1;
                                    }
                                    if (gioihan > gioihanHH)
                                    {
                                        str = "File dữ liệu vượt quá giới hạn số lượng hàng hóa cho phép";
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
                catch
                {
                    str = "Vui lòng import file đúng định dạng";
                }
                if (worksheet.Cells.Rows.Count < 4)
                {
                    str = "File import không có dữ liệu";
                }
            }
            return str;
        }
        public string CheckFileMauBangGia(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn;
            int trows = worksheet.Cells.MaxDataRow;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT BẢNG GIÁ")
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (dt.Rows.Count < 1)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }

        public string CheckFileMau_HangHoaHoaDon(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn;
            int trows = worksheet.Cells.MaxDataRow - 1;
            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool, true);
            string sheet = worksheet.Name.ToString();
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "DANH SÁCH HÀNG HÓA" || tcool != 4)
            {
                str = sheet + "dữ liệu không đúng định dạng theo file mẫu";
            }
            if (dt.Rows.Count < 1)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMau_XuatHuy(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn;
            int trows = worksheet.Cells.MaxDataRow + 1;
            //DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT DANH SÁCH HÀNG XUẤT KHO"/* || tcool != 4*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (trows < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMau_ChietKhau(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn;
            int trows = worksheet.Cells.MaxDataRow + 1;
            //DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            if (worksheet.Cells.Rows[0][0].Value.ToString().Trim() != "MẪU FILE IMPORT CHIẾT KHẤU NHÂN VIÊN THEO HÀNG HÓA"/* || tcool != 4*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (trows < 4)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMau_DieuChinh(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn;
            int trows = worksheet.Cells.MaxDataRow + 1;
            if (worksheet.Cells.Rows[0][0].Value.ToString().Trim() != "MẪU FILE IMPORT DANH SÁCH HÀNG ĐIỀU CHỈNH"/* || tcool != 4*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (trows < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMau_KiemKho(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            Cell cell = worksheet.Cells.Rows[1].LastCell;
            int tcool = cell.Column + 1;
            int trows = worksheet.Cells.MaxDataRow + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            if (dt.Rows[0][1].ToString().Trim() != "Mã hàng hóa" || dt.Rows[0][2].ToString().Trim() != "Số lượng thực tế")
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (trows < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMau_DieuChuyen(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn;
            int trows = worksheet.Cells.MaxDataRow + 1;
            //DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT DANH SÁCH HÀNG ĐIỀU CHUYỂN" /*|| tcool != 4*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (trows < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMau_NhapHang(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn;
            int trows = worksheet.Cells.MaxDataRow;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT DANH SÁCH HÀNG NHẬP" /*|| tcool != 4*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (dt.Rows.Count < 1)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMau_TraHangNhap(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn;
            int trows = worksheet.Cells.MaxDataRow;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT DANH SÁCH TRẢ HÀNG NHẬP"/* || tcool != 4*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (dt.Rows.Count < 1)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMauKhachHang(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn + 1;
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT KHÁCH HÀNG"/* || tcool != 13*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            //if (dt.Rows.Count < 1)
            if (worksheet.Cells.Rows.Count < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMauThongTinNhanVien(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn + 1;
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT THÔNG TIN NHÂN VIÊN"/* || tcool != 13*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            //if (dt.Rows.Count < 1)
            if (worksheet.Cells.Rows.Count < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMauThongTinHopDong(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn + 1;
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT THÔNG TIN HỢP ĐỒNG"/* || tcool != 13*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            //if (dt.Rows.Count < 1)
            if (worksheet.Cells.Rows.Count < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMauThongTinBaoHiem(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn + 1;
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT THÔNG TIN BẢO HIỂM"/* || tcool != 13*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            //if (dt.Rows.Count < 1)
            if (worksheet.Cells.Rows.Count < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMauTheoTieuDe(Stream fileInput, string TieuDe)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn + 1;
            if (worksheet.Cells.Rows[0][0].Value.ToString() != TieuDe)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            //if (dt.Rows.Count < 1)
            if (worksheet.Cells.Rows.Count < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }
        public string CheckFileMauNhaCungCap(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn + 1;
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT NHÀ CUNG CẤP"/* || tcool != 14*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (worksheet.Cells.Rows.Count < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }

        public string CheckFileMauBaoHiem(Stream fileInput)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn + 1;
            if (worksheet.Cells.Rows[0][0].Value.ToString() != "MẪU FILE IMPORT BẢO HIỂM"/* || tcool != 14*/)
            {
                str = "Vui lòng import file đúng định dạng";
            }
            if (worksheet.Cells.Rows.Count < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }

        public List<ErrorDMHangHoa> CheckImportFileDinhLuong(Stream fileInput,
            string lstErr, Guid idDonVi, Guid idNhanVien, int? typeUpdate = 0)
        {
            classDonViQuiDoi classQuyDoi = new classDonViQuiDoi(db);
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            List<ComBo> lstCombo = new List<ComBo>();

            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow - 2;// read start at header (header contain 2 rows)
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(3, 0, trows, tcool);

            string[] mang = lstErr.Split(',');
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(mang[i]))
                {
                    int j = int.Parse(mang[i].ToString());
                    dt.Rows[j].Delete();
                }
            }

            // check quanlytheolo
            var quanlyTheoLo = false;
            var cauhinh = db.HT_CauHinhPhanMem.Where(x => x.ID_DonVi == idDonVi).Select(x => x.LoHang);
            if (cauhinh != null && cauhinh.Count() > 0)
            {
                quanlyTheoLo = (bool)cauhinh.FirstOrDefault();
            }

            if (quanlyTheoLo)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    string rowIndex = "Dòng số: " + (i + 4).ToString();

                    var maDichVu = dr[0].ToString().Trim();
                    var maThanhPhan = dr[1].ToString().Trim();
                    var malohang = dr[2].ToString().Trim();
                    var soluong = dr[3].ToString().Trim();
                    var dongia = dr[4].ToString().Trim();
                    var ghichu = dr[5].ToString().Trim();
                    int? loaiHang = 3;

                    Guid idQuiDoiDV = Guid.Empty;
                    if (!string.IsNullOrEmpty(maDichVu))
                    {
                        maDichVu = maDichVu.ToUpper();
                        var dichvu = from qd in db.DonViQuiDois
                                     join hh in db.DM_HangHoa on qd.ID_HangHoa equals hh.ID
                                     where qd.MaHangHoa == maDichVu
                                     select new
                                     {
                                         ID_DonViQuiDoi = qd.ID,
                                         LoaiHangHoa = hh.LoaiHangHoa != null ? hh.LoaiHangHoa : hh.LaHangHoa == true ? 1 : 2
                                     };
                        if (dichvu == null || dichvu.Count() == 0)
                        {
                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                            {
                                TenTruongDuLieu = "Mã dịch vụ cha",
                                ViTri = rowIndex,
                                ThuocTinh = maDichVu,
                                DienGiai = "Mã dịch vụ cha chưa tồn tại trong hệ thống",
                                rowError = i,
                            };
                            lstError.Add(itemErr);
                        }
                        else
                        {
                            idQuiDoiDV = dichvu.FirstOrDefault().ID_DonViQuiDoi;
                            loaiHang = dichvu.FirstOrDefault().LoaiHangHoa;
                            if (loaiHang == 1)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Mã dịch vụ cha",
                                    ViTri = rowIndex,
                                    ThuocTinh = maDichVu,
                                    DienGiai = "Dịch vụ cha là hàng hóa",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);
                            }
                            else
                            {
                                goto CheckThanhPhan;
                            }
                        }
                    }
                    else
                    {
                        if (lstCombo.Count() > 0)
                        {
                            maDichVu = lstCombo.Last().MaHangHoa;
                            loaiHang = lstCombo.Last().LoaiHangHoa;
                        }
                        goto CheckThanhPhan;
                    }

                CheckThanhPhan:
                    {
                        int? loaiTP = 1;
                        bool tpQuanLyTheoLo = false;
                        if (string.IsNullOrEmpty(maThanhPhan))
                        {
                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                            {
                                TenTruongDuLieu = "Mã thành phần",
                                ViTri = rowIndex,
                                ThuocTinh = "Mã thành phần",
                                DienGiai = "Mã thành phần không được để trống",
                                rowError = i,
                            };
                            lstError.Add(itemErr);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(soluong))
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Số lượng",
                                    ViTri = rowIndex,
                                    ThuocTinh = "Số lượng",
                                    DienGiai = "Số lượng không được để trống",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);

                                soluong = "0";// avoid err exception
                            }
                            else
                            {
                                bool isNumber = IsNumber(soluong);
                                if (!isNumber)
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Số lượng",
                                        ViTri = rowIndex,
                                        ThuocTinh = soluong,
                                        DienGiai = "Số lượng không phải dạng số",
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }

                            }
                            var thanhphan = from qd in db.DonViQuiDois
                                            join hh in db.DM_HangHoa on qd.ID_HangHoa equals hh.ID
                                            where qd.MaHangHoa == maThanhPhan
                                            select new
                                            {
                                                ID_DonViQuiDoi = qd.ID,
                                                ID_HangHoa = qd.ID_HangHoa,
                                                DonGia = qd.GiaBan,
                                                QuanLyTheoLoHang = hh.QuanLyTheoLoHang == null ? false : hh.QuanLyTheoLoHang,
                                                LoaiHangHoa = hh.LoaiHangHoa != null ? hh.LoaiHangHoa : hh.LaHangHoa == true ? 1 : 2
                                            };
                            if (thanhphan == null || thanhphan.Count() == 0)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Mã thành phần",
                                    ViTri = rowIndex,
                                    ThuocTinh = maThanhPhan,
                                    DienGiai = "Mã thành phần chưa tồn tại trong hệ thống",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);
                            }
                            else
                            {
                                loaiTP = thanhphan.FirstOrDefault().LoaiHangHoa;
                                tpQuanLyTheoLo = thanhphan.FirstOrDefault().QuanLyTheoLoHang ?? false;
                                Guid? idLoHang = null;

                                if (tpQuanLyTheoLo)
                                {
                                    if (string.IsNullOrEmpty(malohang))
                                    {
                                        ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                        {
                                            TenTruongDuLieu = "Mã lô hàng",
                                            ViTri = rowIndex,
                                            ThuocTinh = malohang,
                                            DienGiai = "Thành phần quản lý theo lô. Mã lô hàng không được để trống",
                                            rowError = i,
                                        };
                                        lstError.Add(itemErr);
                                    }
                                    else
                                    {
                                        var lohang = from lo in db.DM_LoHang
                                                     where lo.ID_HangHoa == thanhphan.FirstOrDefault().ID_HangHoa
                                                     && lo.MaLoHang.ToUpper() == malohang.ToUpper()
                                                     select lo.ID;
                                        if (lohang != null && lohang.Count() > 0)
                                        {
                                            idLoHang = lohang.FirstOrDefault();
                                        }
                                        else
                                        {
                                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                            {
                                                TenTruongDuLieu = "Mã lô hàng",
                                                ViTri = rowIndex,
                                                ThuocTinh = malohang,
                                                DienGiai = string.Concat("Lô ", malohang, " không thuộc hàng ", maThanhPhan),
                                                rowError = i,
                                            };
                                            lstError.Add(itemErr);
                                        }
                                    }
                                }
                                if (loaiHang == 2 && (loaiTP == 2 || loaiTP == 3))
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Mã dịch vụ cha",
                                        ViTri = rowIndex,
                                        ThuocTinh = maThanhPhan,
                                        DienGiai = string.Concat(maDichVu,
                                        " là dịch vụ. Không được chọn thành phần là ", loaiTP == 2 ? "dịch vụ" : "combo"),
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }

                                if (loaiHang == 3 && loaiTP == 3)
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Mã thành phần",
                                        ViTri = rowIndex,
                                        ThuocTinh = maThanhPhan,
                                        DienGiai = string.Concat("Không được chọn thành phần là combo"),
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }

                                if (!string.IsNullOrEmpty(dongia))
                                {
                                    bool isNumber = IsNumber(dongia);
                                    if (!isNumber)
                                    {
                                        ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                        {
                                            TenTruongDuLieu = "Đơn giá",
                                            ViTri = rowIndex,
                                            ThuocTinh = dongia,
                                            DienGiai = "Đơn giá không phải dạng số",
                                            rowError = i,
                                        };
                                        lstError.Add(itemErr);
                                    }
                                }
                                else
                                {
                                    dongia = thanhphan.FirstOrDefault().DonGia.ToString();
                                }

                                var idThanhPhan = thanhphan.FirstOrDefault().ID_DonViQuiDoi;

                                if (idQuiDoiDV == Guid.Empty)
                                {
                                    if (lstCombo.Count() > 0)
                                    {
                                        idQuiDoiDV = lstCombo.Last().ID_DonViQuiDoi;
                                    }
                                }

                                if (idThanhPhan == idQuiDoiDV)
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Mã thành phần",
                                        ViTri = rowIndex,
                                        ThuocTinh = maThanhPhan,
                                        DienGiai = string.Concat("Mã thành phần con không được trùng mới mã thành phần cha"),
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }

                                TonGoiDichVu_ChiTiet tpNew = new TonGoiDichVu_ChiTiet
                                {
                                    ID_DonViQuiDoi = idThanhPhan,
                                    MaHangHoa = maThanhPhan,
                                    SoLuong = Convert.ToDouble(soluong),
                                    DonGia = Convert.ToDouble(dongia),
                                    ID_LoHang = idLoHang,
                                    GhiChu = ghichu,
                                };

                                if (lstCombo.Count > 0)
                                {
                                    if (idQuiDoiDV == Guid.Empty)
                                    {
                                        // check sameTP in dichvu
                                        var sameTP = lstCombo.Last().ListThanhPhan.Where(x => x.ID_DonViQuiDoi == idThanhPhan && x.ID_LoHang == idLoHang).Count() > 0;
                                        if (sameTP)
                                        {
                                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                            {
                                                TenTruongDuLieu = "Mã thành phần",
                                                ViTri = rowIndex,
                                                ThuocTinh = maThanhPhan,
                                                DienGiai = string.Concat("Dịch vụ ", maDichVu, " có thành phần ", maThanhPhan, " bị trùng lặp"),
                                                rowError = i,
                                            };
                                            lstError.Add(itemErr);
                                        }
                                    }
                                }

                                if (idQuiDoiDV == Guid.Empty)
                                {
                                    if (lstCombo.Count() > 0)
                                    {
                                        lstCombo.Last().ListThanhPhan.Add(tpNew);
                                    }
                                }
                                else
                                {
                                    if (lstCombo.Count > 0)
                                    {
                                        // check exist dvcha
                                        var exDV = lstCombo.Where(x => x.ID_DonViQuiDoi == idQuiDoiDV);
                                        if (exDV.Count() > 0)
                                        {
                                            // remove & add again: đảm bảo luôn là phần tử cuối cùng
                                            foreach (var dvCha in lstCombo)
                                            {
                                                if (dvCha.ID_DonViQuiDoi == idQuiDoiDV)
                                                {
                                                    lstCombo.Remove(dvCha);

                                                    var dvOld = dvCha;
                                                    dvOld.ListThanhPhan.Add(tpNew);
                                                    lstCombo.Add(dvOld);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lstCombo.Add(new ComBo
                                            {
                                                MaHangHoa = maDichVu,
                                                LoaiHangHoa = loaiHang,
                                                ID_DonViQuiDoi = idQuiDoiDV,
                                                ListThanhPhan = new List<TonGoiDichVu_ChiTiet> { tpNew }
                                            });
                                        }
                                    }
                                    else
                                    {
                                        lstCombo.Add(new ComBo
                                        {
                                            MaHangHoa = maDichVu,
                                            LoaiHangHoa = loaiHang,
                                            ID_DonViQuiDoi = idQuiDoiDV,
                                            ListThanhPhan = new List<TonGoiDichVu_ChiTiet> { tpNew }
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    string rowIndex = "Dòng số: " + (i + 4).ToString();

                    var maDichVu = dr[0].ToString().Trim();
                    var maThanhPhan = dr[1].ToString().Trim();
                    var soluong = dr[2].ToString().Trim();
                    var dongia = dr[3].ToString().Trim();
                    var ghichu = dr[4].ToString().Trim();
                    int? loaiHang = 3;

                    Guid idQuiDoiDV = Guid.Empty;
                    if (!string.IsNullOrEmpty(maDichVu))
                    {
                        maDichVu = maDichVu.ToUpper();
                        var dichvu = from qd in db.DonViQuiDois
                                     join hh in db.DM_HangHoa on qd.ID_HangHoa equals hh.ID
                                     where qd.MaHangHoa == maDichVu
                                     select new
                                     {
                                         ID_DonViQuiDoi = qd.ID,
                                         LoaiHangHoa = hh.LoaiHangHoa != null ? hh.LoaiHangHoa : hh.LaHangHoa == true ? 1 : 2
                                     };
                        if (dichvu == null || dichvu.Count() == 0)
                        {
                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                            {
                                TenTruongDuLieu = "Mã dịch vụ cha",
                                ViTri = rowIndex,
                                ThuocTinh = maDichVu,
                                DienGiai = "Mã dịch vụ cha chưa tồn tại trong hệ thống",
                                rowError = i,
                            };
                            lstError.Add(itemErr);
                        }
                        else
                        {
                            idQuiDoiDV = dichvu.FirstOrDefault().ID_DonViQuiDoi;
                            loaiHang = dichvu.FirstOrDefault().LoaiHangHoa;
                            if (loaiHang == 1)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Mã dịch vụ cha",
                                    ViTri = rowIndex,
                                    ThuocTinh = maDichVu,
                                    DienGiai = "Dịch vụ cha là hàng hóa",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);
                            }
                            else
                            {
                                goto CheckThanhPhan;
                            }
                        }
                    }
                    else
                    {
                        if (lstCombo.Count() > 0)
                        {
                            maDichVu = lstCombo.Last().MaHangHoa;
                            loaiHang = lstCombo.Last().LoaiHangHoa;
                        }
                        goto CheckThanhPhan;
                    }

                CheckThanhPhan:
                    {
                        int? loaiTP = 1;
                        if (string.IsNullOrEmpty(maThanhPhan))
                        {
                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                            {
                                TenTruongDuLieu = "Mã thành phần",
                                ViTri = rowIndex,
                                ThuocTinh = "Mã thành phần",
                                DienGiai = "Mã thành phần không được để trống",
                                rowError = i,
                            };
                            lstError.Add(itemErr);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(soluong))
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Số lượng",
                                    ViTri = rowIndex,
                                    ThuocTinh = "Số lượng",
                                    DienGiai = "Số lượng không được để trống",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);

                                soluong = "0";// avoid err exception
                            }
                            else
                            {
                                bool isNumber = IsNumber(soluong);
                                if (!isNumber)
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Số lượng",
                                        ViTri = rowIndex,
                                        ThuocTinh = soluong,
                                        DienGiai = "Số lượng không phải dạng số",
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }

                            }
                            var thanhphan = from qd in db.DonViQuiDois
                                            join hh in db.DM_HangHoa on qd.ID_HangHoa equals hh.ID
                                            where qd.MaHangHoa == maThanhPhan
                                            select new
                                            {
                                                ID_DonViQuiDoi = qd.ID,
                                                ID_HangHoa = qd.ID_HangHoa,
                                                DonGia = qd.GiaBan,
                                                QuanLyTheoLoHang = hh.QuanLyTheoLoHang == null ? false : hh.QuanLyTheoLoHang,
                                                LoaiHangHoa = hh.LoaiHangHoa != null ? hh.LoaiHangHoa : hh.LaHangHoa == true ? 1 : 2
                                            };
                            if (thanhphan == null || thanhphan.Count() == 0)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Mã thành phần",
                                    ViTri = rowIndex,
                                    ThuocTinh = maThanhPhan,
                                    DienGiai = "Mã thành phần chưa tồn tại trong hệ thống",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);
                            }
                            else
                            {
                                loaiTP = thanhphan.FirstOrDefault().LoaiHangHoa;
                                Guid? idLoHang = null;

                                if (loaiHang == 2 && (loaiTP == 2 || loaiTP == 3))
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Mã dịch vụ cha",
                                        ViTri = rowIndex,
                                        ThuocTinh = maThanhPhan,
                                        DienGiai = string.Concat(maDichVu,
                                        " là dịch vụ. Không được chọn thành phần là ", loaiTP == 2 ? "dịch vụ" : "combo"),
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }

                                if (loaiHang == 3 && loaiTP == 3)
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Mã thành phần",
                                        ViTri = rowIndex,
                                        ThuocTinh = maThanhPhan,
                                        DienGiai = string.Concat("Không được chọn thành phần là combo"),
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }

                                if (!string.IsNullOrEmpty(dongia))
                                {
                                    bool isNumber = IsNumber(dongia);
                                    if (!isNumber)
                                    {
                                        ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                        {
                                            TenTruongDuLieu = "Đơn giá",
                                            ViTri = rowIndex,
                                            ThuocTinh = dongia,
                                            DienGiai = "Đơn giá không phải dạng số",
                                            rowError = i,
                                        };
                                        lstError.Add(itemErr);
                                    }
                                }
                                else
                                {
                                    dongia = thanhphan.FirstOrDefault().DonGia.ToString();
                                }

                                var idThanhPhan = thanhphan.FirstOrDefault().ID_DonViQuiDoi;

                                if (idQuiDoiDV == Guid.Empty)
                                {
                                    if (lstCombo.Count() > 0)
                                    {
                                        idQuiDoiDV = lstCombo.Last().ID_DonViQuiDoi;
                                    }
                                }

                                if (idThanhPhan == idQuiDoiDV)
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Mã thành phần",
                                        ViTri = rowIndex,
                                        ThuocTinh = maThanhPhan,
                                        DienGiai = string.Concat("Mã thành phần con không được trùng mới mã thành phần cha"),
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }

                                TonGoiDichVu_ChiTiet tpNew = new TonGoiDichVu_ChiTiet
                                {
                                    ID_DonViQuiDoi = idThanhPhan,
                                    MaHangHoa = maThanhPhan,
                                    SoLuong = Convert.ToDouble(soluong),
                                    DonGia = Convert.ToDouble(dongia),
                                    ID_LoHang = idLoHang,
                                    GhiChu = ghichu,
                                };

                                if (lstCombo.Count > 0)
                                {
                                    // check sameTP in dichvu
                                    if (idQuiDoiDV == Guid.Empty)
                                    {
                                        var sameTP = lstCombo.Last().ListThanhPhan.Where(x => x.ID_DonViQuiDoi == idThanhPhan && x.ID_LoHang == idLoHang).Count() > 0;
                                        if (sameTP)
                                        {
                                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                            {
                                                TenTruongDuLieu = "Mã thành phần",
                                                ViTri = rowIndex,
                                                ThuocTinh = maThanhPhan,
                                                DienGiai = string.Concat("Dịch vụ ", maDichVu, " có thành phần ", maThanhPhan, " bị trùng lặp"),
                                                rowError = i,
                                            };
                                            lstError.Add(itemErr);
                                        }
                                    }
                                }

                                if (idQuiDoiDV == Guid.Empty)
                                {
                                    if (lstCombo.Count() > 0)
                                    {
                                        lstCombo.Last().ListThanhPhan.Add(tpNew);
                                    }
                                }
                                else
                                {
                                    if (lstCombo.Count > 0)
                                    {
                                        // check exist dvcha
                                        var exDV = lstCombo.Where(x => x.ID_DonViQuiDoi == idQuiDoiDV);
                                        if (exDV.Count() > 0)
                                        {
                                            // remove & add again: đảm bảo luôn là phần tử cuối cùng
                                            foreach (var dvCha in lstCombo)
                                            {
                                                if (dvCha.ID_DonViQuiDoi == idQuiDoiDV)
                                                {
                                                    lstCombo.Remove(dvCha);

                                                    var dvOld = dvCha;
                                                    dvOld.ListThanhPhan.Add(tpNew);
                                                    lstCombo.Add(dvOld);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lstCombo.Add(new ComBo
                                            {
                                                MaHangHoa = maDichVu,
                                                LoaiHangHoa = loaiHang,
                                                ID_DonViQuiDoi = idQuiDoiDV,
                                                ListThanhPhan = new List<TonGoiDichVu_ChiTiet> { tpNew }
                                            });
                                        }
                                    }
                                    else
                                    {
                                        lstCombo.Add(new ComBo
                                        {
                                            MaHangHoa = maDichVu,
                                            LoaiHangHoa = loaiHang,
                                            ID_DonViQuiDoi = idQuiDoiDV,
                                            ListThanhPhan = new List<TonGoiDichVu_ChiTiet> { tpNew }
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                lstError = importDinhLuong(lstCombo, idDonVi, idNhanVien, typeUpdate);
            }
            return lstError;
        }

        public List<ErrorDMHangHoa> importDinhLuong(List<ComBo> lstCombo, Guid idDonVi, Guid idNhanVien, int? typeUpdate = 0)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                List<DinhLuongDichVu> lstAdd = new List<DinhLuongDichVu>();
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                try
                {
                    string noidung = string.Empty, sTypeUpdate = string.Empty;
                    int stt = 1;
                    var arrDV = lstCombo.Select(x => x.ID_DonViQuiDoi);
                    if (typeUpdate == 1)// xoa all tpcu + add tpnew
                    {
                        sTypeUpdate = "<br /> - Loại cập nhật: " + "Cập nhật lại thành phần định lượng";
                        var lstDelete = db.DinhLuongDichVus.Where(x => arrDV.Contains(x.ID_DichVu));
                        db.DinhLuongDichVus.RemoveRange(lstDelete);

                        foreach (var itOut in lstCombo)
                        {
                            noidung += string.Concat("<br /> <b> ", stt, ". ", itOut.MaHangHoa, " </b> gồm các thành phần :");
                            string detail = string.Empty;
                            foreach (var item in itOut.ListThanhPhan)
                            {
                                DinhLuongDichVu itNew = new DinhLuongDichVu
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DichVu = itOut.ID_DonViQuiDoi,
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    ID_LoHang = item.ID_LoHang ?? null,
                                    SoLuong = item.SoLuong ?? 0,
                                    DonGia = item.DonGia ?? 0,
                                    GhiChu = item.GhiChu,
                                };
                                lstAdd.Add(itNew);
                                detail = string.Concat(detail, "<br /> ", item.MaHangHoa,
                                    ", Số lượng ", item.SoLuong,
                                     ", Đơn giá ", item.DonGia);
                            }
                            stt += 1;
                            noidung += detail;
                        }
                    }
                    else// giu tp cu, them tp moi
                    {
                        sTypeUpdate = "<br /> - Loại cập nhật: " + "Giữ thành phần cũ, bổ sung thành phần mới";
                        foreach (var itOut in lstCombo)
                        {
                            noidung += string.Concat("<br /> <b> ", stt, ". ", itOut.MaHangHoa, " </b> gồm các thành phần :");
                            string detail = string.Empty;
                            foreach (var item in itOut.ListThanhPhan)
                            {
                                // check tpnew exist in DB
                                var tpOld = db.DinhLuongDichVus.Where(x => x.ID_DichVu == itOut.ID_DonViQuiDoi
                                && x.ID_DonViQuiDoi == item.ID_DonViQuiDoi);
                                if (tpOld == null || tpOld.Count() == 0)
                                {
                                    DinhLuongDichVu itNew = new DinhLuongDichVu
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DichVu = itOut.ID_DonViQuiDoi,
                                        ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                        ID_LoHang = item.ID_LoHang ?? null,
                                        SoLuong = item.SoLuong ?? 0,
                                        DonGia = item.DonGia ?? 0,
                                        GhiChu = item.GhiChu,
                                    };
                                    lstAdd.Add(itNew);
                                    detail = string.Concat(detail, "<br /> ", item.MaHangHoa,
                                        ", Số lượng ", item.SoLuong,
                                        ", Đơn giá ", item.DonGia);
                                }
                            }
                            stt += 1;
                            noidung += detail;
                        }
                    }
                    db.DinhLuongDichVus.AddRange(lstAdd);

                    #region NhatKySuDung
                    HT_NhatKySuDung nky = new HT_NhatKySuDung()
                    {
                        ID = Guid.NewGuid(),
                        ID_DonVi = idDonVi,
                        ID_NhanVien = idNhanVien,
                        LoaiNhatKy = 5,
                        ChucNang = "Import định lượng",
                        NoiDung = "Import định lượng dịch vụ",
                        NoiDungChiTiet = string.Concat("Import định lượng dịch vụ", sTypeUpdate, " <br /><b> Nội dung chi tiết </b>", noidung),
                        ThoiGian = DateTime.Now,
                    };
                    db.HT_NhatKySuDung.Add(nky);
                    db.SaveChanges();
                    trans.Commit();
                    #endregion
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                    {
                        TenTruongDuLieu = "Exception",
                        ViTri = string.Empty,
                        ThuocTinh = "Exception",
                        DienGiai = ex.InnerException + ex.Message,
                        rowError = -1,
                    };
                    lstError.Add(itemErr);
                }
                return lstError;
            }
        }

        public List<ErrorDMHangHoa> checkExcel_XuatHuy(Stream fileInput)
        {
            List<ErrorDMHangHoa> lst = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            dt.Rows[0].Delete();
            dt.Columns[1].ColumnName = "MaHang";
            dt.Columns[0].ColumnName = "MaLo";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                var dtRow = dt.Rows[i];
                string malohang = dtRow[0].ToString().Trim();
                string mahanghoa = dtRow[1].ToString().Trim();
                string soluong = dtRow[2].ToString().Trim();
                string index = (i + 3).ToString();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dtRow[j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    if (malohang != "")
                    {
                        bool checklo = ChekLoHangDatabase(malohang, mahanghoa);
                        if (checklo == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": " + mahanghoa + " không có lô hàng '" + malohang + "' trên hệ thống";
                            lst.Add(DM);
                        }
                    }
                    if (mahanghoa == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool kytudacbiet = kiemtrakitu(mahanghoa);
                        if (kytudacbiet == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được chứa ký tự đặc biệt '" + mahanghoa + "'";
                            lst.Add(DM);
                        }
                        bool trungma = false;
                        if (malohang != "")
                            trungma = GroupData(dt, "MaHang = '" + mahanghoa + "' and MaLo = '" + malohang + "'");
                        else
                            trungma = GroupData(dt, "MaHang = '" + mahanghoa + "'");
                        if (trungma == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            string Lo = string.Empty;
                            if (malohang != "")
                                Lo = " (" + malohang + ")";
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahanghoa + Lo + "' bị trùng lặp";
                            lst.Add(DM);
                        }
                        bool CheckCSDL = ChekMaHangDatabase_DangKinhDoanh(mahanghoa);
                        if (CheckCSDL == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahanghoa + "' không có trên hệ thống hoặc ngừng kinh doanh";
                            lst.Add(DM);
                        }
                        //else
                        //{
                        //    if (malohang != "")
                        //    {
                        //        bool checklo = ChekLoHangDatabase(malohang, mahanghoa);
                        //        if (checklo == false)
                        //        {
                        //            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        //            DM.DienGiai = "Dòng số " + index + ": " + mahanghoa + " không có lô hàng '" + malohang + "' trên hệ thống";
                        //            lst.Add(DM);
                        //        }
                        //    }
                        //}
                    }
                    if (soluong == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Số lượng không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool isNumber7 = IsNumber(soluong);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Số lượng '" + soluong + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                        else
                        {
                            if (float.Parse(soluong) <= 0)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.DienGiai = "Dòng số " + index + ": Số lượng '" + soluong + "'  Phải lớn hơn 0";
                                lst.Add(DM);
                            }
                        }
                    }
                }
            }
            return lst;
        }
        public List<ErrorDMHangHoa> checkExcel_ChietKhau(Stream fileInput)
        {
            List<ErrorDMHangHoa> lst = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            dt.Rows[0].Delete();
            dt.Columns[0].ColumnName = "MaHang";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string a = dt.Rows[i][j].ToString();
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    var maHangHoa = dt.Rows[i][0].ToString().Trim();
                    var rowDoing = (i + 4).ToString();

                    if (maHangHoa == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Mã hàng hóa không được để trống",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        bool kytudacbiet = kiemtrakitu(maHangHoa);
                        if (kytudacbiet == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa
                            {
                                DienGiai = "Dòng số " + rowDoing + ": Mã hàng hóa không được chứa ký tự đặc biệt '" + maHangHoa + "'",
                            };
                            lst.Add(DM);
                        }
                        bool trungma = GroupData(dt, "MaHang = '" + maHangHoa + "'");
                        if (trungma == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa
                            {
                                DienGiai = "Dòng số " + rowDoing + ": Mã hàng '" + maHangHoa + "' bị trùng lặp",
                            };
                            lst.Add(DM);
                        }
                        bool CheckCSDL = ChekMaHangDatabase_DangKinhDoanh(maHangHoa);
                        if (CheckCSDL == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa
                            {
                                DienGiai = "Dòng số " + rowDoing + ": Mã hàng '" + maHangHoa + "' không có trên hệ thống hoặc ngừng kinh doanh",
                            };
                            lst.Add(DM);
                        }
                    }
                    float ThucHienVND = 0, ThucHienPT = 0, TuVanVND = 0, TuVanPT = 0, YeuCauVND = 0, YeuCauPT = 0, BanGoiVND = 0, BanGoiPT = 0;

                    var hoaHongTH_VND = dt.Rows[i][2].ToString().Trim();
                    bool isNumber = IsNumber(hoaHongTH_VND);
                    if (isNumber == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": VNĐ hoa hồng thực hiện '" + hoaHongTH_VND + "'  không phải dạng số",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        if (hoaHongTH_VND != string.Empty)
                            ThucHienVND = float.Parse(hoaHongTH_VND);
                    }

                    var hoaHongTH_Ptram = dt.Rows[i][3].ToString().Trim();
                    isNumber = IsNumber(hoaHongTH_Ptram);
                    if (isNumber == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Phần trăm hoa hồng thực hiện '" + hoaHongTH_Ptram + "'  không phải dạng số",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        if (hoaHongTH_Ptram != string.Empty)
                            ThucHienPT = float.Parse(hoaHongTH_Ptram);
                    }

                    var theoyeucau = dt.Rows[i][4].ToString().Trim();
                    isNumber = IsNumber(theoyeucau);
                    if (isNumber == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": VNĐ hoa hồng theo yêu cầu '" + theoyeucau + "'  không phải dạng số",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        if (theoyeucau != string.Empty)
                            YeuCauVND = float.Parse(theoyeucau);
                    }

                    var theoyc_Ptram = dt.Rows[i][5].ToString().Trim();
                    isNumber = IsNumber(theoyc_Ptram);
                    if (isNumber == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Phần trăm hoa hồng theo yêu cầu '" + theoyc_Ptram + "'  không phải dạng số",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        if (theoyc_Ptram != string.Empty)
                            YeuCauPT = float.Parse(theoyc_Ptram);
                    }

                    var hoahongTV_VND = dt.Rows[i][6].ToString().Trim();
                    isNumber = IsNumber(hoahongTV_VND);
                    if (isNumber == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": VNĐ hoa hồng bán hàng '" + hoahongTV_VND + "'  không phải dạng số",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        if (hoahongTV_VND != string.Empty)
                            TuVanVND = float.Parse(hoahongTV_VND);
                    }

                    var hoahongTV_Ptram = dt.Rows[i][7].ToString().Trim();
                    isNumber = IsNumber(hoahongTV_Ptram);
                    if (isNumber == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Phần trăm hoa hồng bán hàng '" + hoahongTV_Ptram + "'  không phải dạng số",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        if (hoahongTV_Ptram != string.Empty)
                            TuVanPT = float.Parse(hoahongTV_Ptram);
                    }

                    var bangoiVND = dt.Rows[i][8].ToString().Trim();
                    isNumber = IsNumber(bangoiVND);
                    if (isNumber == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Phần trăm hoa hồng bán gói dịch vụ '" + bangoiVND + "'  không phải dạng số",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        if (bangoiVND != string.Empty)
                            BanGoiVND = float.Parse(bangoiVND);
                    }

                    var bangoiPT = dt.Rows[i][9].ToString().Trim();
                    isNumber = IsNumber(bangoiPT);
                    if (isNumber == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Phần trăm hoa hồng bán gói dịch vụ '" + bangoiPT + "'  không phải dạng số",
                        };
                        lst.Add(DM);
                    }
                    else
                    {
                        if (bangoiPT != string.Empty)
                            BanGoiPT = float.Parse(bangoiPT);
                    }

                    if (hoaHongTH_VND == string.Empty && hoaHongTH_Ptram == string.Empty && hoahongTV_VND == string.Empty && hoahongTV_Ptram == string.Empty
                        && bangoiVND == string.Empty && bangoiPT == string.Empty && theoyc_Ptram == string.Empty && theoyeucau == string.Empty)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Mã hàng '" + dt.Rows[i][0].ToString().Trim() + "'" + " chưa được cài đặt hoa hồng chiết khấu",
                        };
                        lst.Add(DM);
                    }

                    if (ThucHienVND != 0 && ThucHienPT != 0)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Hoa hồng thực hiện chỉ được thiết lập theo VNĐ hoặc %",
                        };
                        lst.Add(DM);
                    }
                    if (YeuCauVND != 0 && YeuCauPT != 0)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Hoa hồng theo yêu cầu chỉ được thiết lập theo VNĐ hoặc %",
                        };
                        lst.Add(DM);
                    }
                    if (TuVanVND != 0 && TuVanPT != 0)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Hoa hồng bán hàng chỉ được thiết lập theo VNĐ hoặc %",
                        };
                        lst.Add(DM);
                    }
                    if (BanGoiVND != 0 && BanGoiPT != 0)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa
                        {
                            DienGiai = "Dòng số " + rowDoing + ": Hoa hồng bán gói dịch vụ chỉ được thiết lập theo VNĐ hoặc %",
                        };
                        lst.Add(DM);
                    }
                }
            }
            return lst;
        }

        public string GetDefaultPrinter()
        {
            PrinterSettings settings = new PrinterSettings();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                settings.PrinterName = printer;
                if (settings.IsDefaultPrinter)
                    return printer;
            }
            return string.Empty;
        }
        public List<ErrorDMHangHoa> checkExcel_DieuChinh(Stream fileInput)
        {
            List<ErrorDMHangHoa> lst = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            dt.Rows[0].Delete();
            dt.Columns[0].ColumnName = "MaLo";
            dt.Columns[1].ColumnName = "MaHang";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string a = dt.Rows[i][j].ToString();
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    if (dt.Rows[i][0].ToString().Trim() != "")
                    {
                        bool checklo = ChekLoHangDatabase(dt.Rows[i][0].ToString().Trim(), dt.Rows[i][1].ToString().Trim());
                        if (checklo == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + (i + 3).ToString().Trim() + ": " + dt.Rows[i][1].ToString().Trim() + " không có lô hàng '" + dt.Rows[i][0].ToString().Trim() + "' trên hệ thống";
                            lst.Add(DM);
                        }
                    }
                    if (dt.Rows[i][1].ToString().Trim() == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + (i + 3).ToString().Trim() + ": Mã hàng hóa không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool kytudacbiet = kiemtrakitu(dt.Rows[i][1].ToString().Trim());
                        if (kytudacbiet == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + (i + 3).ToString().Trim() + ": Mã hàng hóa không được chứa ký tự đặc biệt";
                            lst.Add(DM);
                        }
                        else
                        {
                            bool trungma = false;
                            if (dt.Rows[i][0].ToString().Trim() != "")
                                trungma = GroupData(dt, "MaHang = '" + dt.Rows[i][1].ToString().Trim() + "' and MaLo = '" + dt.Rows[i][0].ToString().Trim() + "'");
                            else
                                trungma = GroupData(dt, "MaHang = '" + dt.Rows[i][1].ToString().Trim() + "'");
                            if (trungma == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                string Lo = string.Empty;
                                if (dt.Rows[i][0].ToString().Trim() != "")
                                    Lo = " (" + dt.Rows[i][0].ToString().Trim() + ")";
                                DM.DienGiai = "Dòng số " + (i + 3).ToString() + ": Mã hàng '" + dt.Rows[i][1].ToString().Trim() + Lo + "' bị trùng lặp";
                                lst.Add(DM);
                            }
                            bool CheckCSDL = ChekMaHangDatabase_DangKinhDoanh(dt.Rows[i][1].ToString().Trim());
                            if (CheckCSDL == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.DienGiai = "Dòng số " + (i + 3).ToString() + ": Mã hàng '" + dt.Rows[i][1].ToString().Trim() + "' không có trên hệ thống hoặc ngừng kinh doanh";
                                lst.Add(DM);
                            }
                        }

                    }
                    if (dt.Rows[i][4].ToString().Trim() == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + (i + 3).ToString() + ": giá vốn mới không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool isNumber7 = IsNumber(dt.Rows[i][4].ToString().Trim());
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + (i + 3).ToString() + ": giá vốn mới '" + dt.Rows[i][4].ToString().Trim() + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                }
            }
            return lst;
        }
        public List<ErrorDMHangHoa> checkExcel_KiemKho(Stream fileInput)
        {
            List<ErrorDMHangHoa> lst = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            dt.Rows[0].Delete();
            dt.Columns[0].ColumnName = "MaLo";
            dt.Columns[1].ColumnName = "MaHang";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                DataRow dtRow = dt.Rows[i];
                string malohang = dtRow[0].ToString().Trim();
                string mahanghoa = dtRow[1].ToString().Trim();
                string soluong = dtRow[2].ToString().Trim();
                string rowIndex = (i + 3).ToString().Trim();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string a = dtRow[j].ToString();
                    if (dtRow[j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }

                if (dk == "")
                {
                    if (malohang != "")
                    {
                        bool checklo = ChekLoHangDatabase(malohang, mahanghoa);
                        if (checklo == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + rowIndex + ": Lô hàng '" + malohang + "' không có trên hệ thống";
                            lst.Add(DM);
                        }
                    }
                    if (mahanghoa == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + rowIndex + ": Mã hàng hóa không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool kytudacbiet = kiemtrakitu(mahanghoa);
                        if (kytudacbiet == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + rowIndex + ": Mã hàng hóa không được chứa ký tự đặc biệt";
                            lst.Add(DM);
                        }
                        bool trungma = GroupData(dt, "MaHang = '" + mahanghoa + (malohang != "" ? "' and MaLo = '" + malohang + "'" : "" + "'"));
                        if (trungma == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            string Lo = string.Empty;
                            if (malohang != "")
                                Lo = " (" + mahanghoa + ")";
                            DM.DienGiai = "Dòng số " + rowIndex + ": Mã hàng '" + mahanghoa + Lo + "' bị trùng lặp";
                            lst.Add(DM);
                        }
                        bool CheckCSDL = ChekMaHangDatabase_DangKinhDoanh(mahanghoa);
                        if (CheckCSDL == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + rowIndex + ": Mã hàng '" + mahanghoa + "' không có trên hệ thống hoặc ngừng kinh doanh";
                            lst.Add(DM);
                        }
                        else
                        {
                            bool CheckLaDV = ChekMaHangDatabase_LaDichVu(mahanghoa);
                            if (CheckLaDV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.DienGiai = "Dòng số " + rowIndex + ": Mã hàng '" + mahanghoa + "' không phải là hàng hóa";
                                lst.Add(DM);
                            }
                        }
                    }
                    if (soluong == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + rowIndex + ": Số lượng không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool isNumber7 = IsNumber(soluong);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + rowIndex + ": Số lượng '" + soluong + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                }
            }
            return lst;
        }
        public List<ErrorDMHangHoa> checkExcel_DieuChuyen(Stream fileInput)
        {
            List<ErrorDMHangHoa> lst = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            dt.Rows[0].Delete(); // delete header excel
            dt.Columns[1].ColumnName = "MaHang";
            dt.Columns[0].ColumnName = "MaLo";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                var dtRow = dt.Rows[i];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dtRow[j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    string malo = dtRow[0].ToString().Trim();
                    string mahanghoa = dtRow[1].ToString().Trim();
                    string soluong = dtRow[2].ToString().Trim();
                    string giavon = dtRow[3].ToString().Trim();
                    string giatrichuyen = dtRow[4].ToString().Trim();
                    string index = (i + 3).ToString().Trim();

                    if (malo != "")
                    {
                        bool checklo = ChekLoHangDatabase(malo, mahanghoa);
                        if (checklo == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Lô hàng '" + malo + "' không có trên hệ thống";
                            lst.Add(DM);
                        }
                    }
                    if (mahanghoa == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool kytudacbiet = kiemtrakitu(mahanghoa);
                        if (kytudacbiet == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được chứa ký tự đặc biệt";
                            lst.Add(DM);
                        }
                        bool trungma = GroupData(dt, "MaHang = '" + mahanghoa + (malo != "" ? "' and MaLo = '" + malo + "'" : "" + "'"));
                        if (trungma == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            string Lo = string.Empty;
                            if (malo != "")
                                Lo = " (" + malo + ")";
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahanghoa + Lo + "' bị trùng lặp";
                            lst.Add(DM);
                        }
                        bool CheckCSDL = ChekMaHangDatabase_DangKinhDoanh(mahanghoa);
                        if (CheckCSDL == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahanghoa + "' không có trên hệ thống hoặc ngừng kinh doanh";
                            lst.Add(DM);
                        }
                        bool CheckLaDV = ChekMaHangDatabase_LaDichVu(mahanghoa);
                        if (CheckLaDV == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahanghoa + "' không phải là hàng hóa";
                            lst.Add(DM);
                        }
                    }
                    if (soluong == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Số lượng không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool isNumber7 = IsNumber(soluong);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Số lượng '" + soluong + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                    if (giavon != "")
                    {
                        bool isNumber7 = IsNumber(giavon);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Giá vốn '" + giavon + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                    if (giatrichuyen != "")
                    {
                        bool isNumber = IsNumber(giatrichuyen);
                        if (isNumber == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Giá trị điều chuyển '" + giatrichuyen + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                }
            }
            return lst;
        }
        public List<ErrorDMHangHoa> checkExcel_NhapHang(Stream fileInput)
        {
            List<ErrorDMHangHoa> lst = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn;
            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool, true);
            dt.Columns[3].ColumnName = "MaHang";
            dt.Columns[0].ColumnName = "MaLo";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                var dtRow = dt.Rows[i];

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dtRow[j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    string malo = dtRow[0].ToString().Trim();
                    string ngaysx = dtRow[1].ToString().Trim();
                    string hsd = dtRow[2].ToString().Trim();
                    string mahang = dtRow[3].ToString().Trim();
                    string soluong = dtRow[4].ToString().Trim();
                    string dongia = dtRow[5].ToString().Trim();
                    string giaban = dtRow[6].ToString().Trim();
                    string ptThue = dtRow[7].ToString().Trim();
                    string tienThue = dtRow[8].ToString().Trim();
                    string index = (i + 4).ToString().Trim();

                    if (mahang == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool kytudacbiet = kiemtrakitu(mahang);
                        if (kytudacbiet == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được chứa ký tự đặc biệt";
                            lst.Add(DM);
                        }

                        bool CheckCSDL = ChekMaHangDatabase_DangKinhDoanh(mahang);
                        if (CheckCSDL == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahang + "' không có trên hệ thống hoặc ngừng kinh doanh";
                            lst.Add(DM);
                        }
                        else
                        {
                            bool CheckLaDV = ChekMaHangDatabase_LaDichVu(mahang);
                            if (CheckLaDV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahang + "' không phải là hàng hóa";
                                lst.Add(DM);
                            }
                        }
                    }
                    DateTime? ngaysanxuat = ngaysx != "" ? DateTime.Parse(ngaysx) : (DateTime?)null;
                    DateTime? ngayhethan = hsd != "" ? DateTime.Parse(hsd) : (DateTime?)null;

                    if (ngaysx != "" && hsd != "")
                    {
                        if (ngaysanxuat > ngayhethan)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Ngày hết hạn nhỏ hơn ngày sản xuất";
                            lst.Add(DM);
                        }
                    }

                    if (soluong == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Số lượng không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool isNumber7 = IsNumber(soluong);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Số lượng '" + soluong + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                    if (dongia == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Đơn giá không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool isNumber7 = IsNumber(dongia);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Đơn giá '" + dongia + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                    if (string.IsNullOrEmpty(giaban))
                    {
                        bool isNumber = IsNumber(giaban);
                        if (isNumber == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Giảm giá '" + giaban + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                    if (string.IsNullOrEmpty(ptThue))
                    {
                        bool isNumber = IsNumber(ptThue);
                        if (isNumber == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Phần trăm thuế '" + ptThue + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                    if (string.IsNullOrEmpty(tienThue))
                    {
                        bool isNumber = IsNumber(tienThue);
                        if (isNumber == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Tiền thuế '" + tienThue + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                }
            }
            return lst;
        }
        public List<Report_HangHoa_XuatHuy_Import> getList_DanhSachHangXuatHuy_Khonglo(Stream fileInput)
        {
            List<Report_HangHoa_XuatHuy_Import> lst = new List<Report_HangHoa_XuatHuy_Import>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            dt.Rows[0].Delete();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    List<SqlParameter> prm = new List<SqlParameter>();
                    prm.Add(new SqlParameter("MaHH", dt.Rows[i][0].ToString()));
                    List<Report_HangHoa_XuatHuy_Import> lst1 = db.Database.SqlQuery<Report_HangHoa_XuatHuy_Import>("exec getListImport_HHXuatHuy @MaHH", prm.ToArray()).ToList();
                    Report_HangHoa_XuatHuy_Import DM1 = new Report_HangHoa_XuatHuy_Import();
                    //DM1.id = lst1.FirstOrDefault().id;
                    //DM1.ID = lst1.FirstOrDefault().ID;
                    DM1.ID_DonViQuiDoi = lst1.FirstOrDefault().ID_DonViQuiDoi;
                    if (dt.Rows[i][2].ToString().Trim() != "")
                        DM1.GiaVon = Math.Round(float.Parse(dt.Rows[i][2].ToString().Trim()), 0, MidpointRounding.ToEven);
                    else
                        DM1.GiaVon = Math.Round(lst1.FirstOrDefault().GiaVon, 0, MidpointRounding.ToEven);
                    if (dt.Rows[i][1].ToString().Trim() != "")
                    {
                        DM1.SoLuong = Math.Round(float.Parse(dt.Rows[i][1].ToString().Trim()), 2, MidpointRounding.ToEven);
                        DM1.SoLuongXuatHuy = Math.Round(float.Parse(dt.Rows[i][1].ToString().Trim()), 2, MidpointRounding.ToEven);
                    }
                    else
                    {
                        DM1.SoLuong = Math.Round(lst1.FirstOrDefault().SoLuong, 2, MidpointRounding.ToEven);
                        DM1.SoLuongXuatHuy = Math.Round(lst1.FirstOrDefault().SoLuongXuatHuy, 2, MidpointRounding.ToEven);
                    }
                    DM1.TenHangHoa = lst1.FirstOrDefault().TenHangHoa;
                    DM1.ThuocTinh_GiaTri = lst1.FirstOrDefault().ThuocTinh_GiaTri;
                    DM1.TenDonViTinh = lst1.FirstOrDefault().TenDonViTinh;
                    DM1.TonKho = Math.Round(lst1.FirstOrDefault().TonKho, 2, MidpointRounding.ToEven);
                    DM1.GiaTriHuy = Math.Round(DM1.SoLuong * DM1.GiaVon, 0, MidpointRounding.ToEven);
                    DM1.MaHangHoa = lst1.FirstOrDefault().MaHangHoa;
                    DM1.TrangThaiMoPhieu = lst1.FirstOrDefault().TrangThaiMoPhieu;
                    lst.Add(DM1);
                }
            }
            return lst;
        }
        public List<Report_HangHoa_XuatHuy_Import> getList_DanhSachHangXuatHuy(Stream fileInput, Guid ID_ChiNhanh)
        {
            List<Report_HangHoa_XuatHuy_Import> lst = new List<Report_HangHoa_XuatHuy_Import>();
            List<Report_HangHoa_XuatHuy_Import> lstCT = new List<Report_HangHoa_XuatHuy_Import>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            dt.Rows[0].Delete();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    List<SqlParameter> sqlPRM = new List<SqlParameter>();
                    sqlPRM.Add(new SqlParameter("MaHangHoa", dt.Rows[i][1].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("MaLoHang", dt.Rows[i][0].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("SoLuong", Math.Round(float.Parse(dt.Rows[i][2].ToString().Trim()), 3, MidpointRounding.ToEven)));
                    sqlPRM.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                    //var tbl_timeCSt = from cs in db.ChotSo
                    //                  where cs.ID_DonVi == ID_ChiNhanh
                    //                  select new
                    //                  {
                    //                      cs.NgayChotSo
                    //                  };
                    //string timeCS = string.Empty;
                    //try
                    //{
                    //    timeCS = tbl_timeCSt.FirstOrDefault().NgayChotSo.ToString("yyyy-MM-dd");
                    //    lst = db.Database.SqlQuery<Report_HangHoa_XuatHuy_Import>("exec getListXuatKho_Import_ChotSo @MaHangHoa, @MaLoHang, @SoLuong, @ID_ChiNhanh", sqlPRM.ToArray()).ToList();
                    //}
                    //catch
                    //{
                    lst = db.Database.SqlQuery<Report_HangHoa_XuatHuy_Import>("exec getListXuatKho_Import @MaHangHoa, @MaLoHang, @SoLuong, @ID_ChiNhanh", sqlPRM.ToArray()).ToList();
                    //}
                    Report_HangHoa_XuatHuy_Import DM1 = new Report_HangHoa_XuatHuy_Import();
                    DM1.ID_DonViQuiDoi = lst.FirstOrDefault().ID_DonViQuiDoi;
                    DM1.ID_LoHang = dt.Rows[i][0].ToString().Trim() == "" ? new Guid() : lst.FirstOrDefault().ID_LoHang;
                    DM1.MaHangHoa = lst.FirstOrDefault().MaHangHoa;
                    DM1.TenHangHoa = lst.FirstOrDefault().TenHangHoa;
                    DM1.ThuocTinh_GiaTri = lst.FirstOrDefault().ThuocTinh_GiaTri;
                    DM1.TenDonViTinh = lst.FirstOrDefault().TenDonViTinh;
                    DM1.QuanLyTheoLoHang = lst.FirstOrDefault().QuanLyTheoLoHang;
                    DM1.GiaVon = lst.FirstOrDefault().GiaVon;
                    DM1.GiaBan = lst.FirstOrDefault().GiaBan;
                    DM1.SoLuong = lst.FirstOrDefault().SoLuong;
                    DM1.SoLuongXuatHuy = lst.FirstOrDefault().SoLuongXuatHuy;
                    DM1.TonKho = lst.FirstOrDefault().TonKho;
                    DM1.GiaTriHuy = lst.FirstOrDefault().GiaTriHuy;
                    DM1.TrangThaiMoPhieu = lst.FirstOrDefault().TrangThaiMoPhieu;
                    DM1.TenLoHang = lst.FirstOrDefault().TenLoHang;
                    DM1.NgaySanXuat = lst.FirstOrDefault().NgaySanXuat;
                    DM1.NgayHetHan = lst.FirstOrDefault().NgayHetHan;
                    DM1.SoThuTu = i + 1;
                    lstCT.Add(DM1);
                }
            }
            return lstCT;
        }

        public string insert_ChietKhauMacDinhNhanVien(Stream fileInput, Guid ID_ChiNhanh, Guid ID_NhanVien)
        {
            string str = string.Empty;
            try
            {
                Workbook objWorkbook = new Workbook(fileInput);
                Worksheet worksheet = objWorkbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dt.Rows[0].Delete();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string dk = "";
                    DataRow dr = dt.Rows[i];
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string a = dr[j].ToString().Trim();
                        if (dr[j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dt.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        double ChietKhau = 0, TuVan = 0, YeuCau = 0, BanGoi = 0;
                        bool LaPTChietKhau = false, LaPTTuVan = false, LaPTYeuCau = false, LaPTBanGoi = false;
                        var column2 = dr[2].ToString().Trim();
                        if (column2 != string.Empty && column2 != "0")
                            ChietKhau = float.Parse(column2);
                        else
                        {
                            var column3 = dr[3].ToString().Trim();
                            if (column3 != string.Empty)
                                ChietKhau = float.Parse(column3);
                            LaPTChietKhau = true;
                        }
                        var column4 = dr[4].ToString().Trim();
                        if (column4 != string.Empty && column4 != "0")
                            YeuCau = float.Parse(column4);
                        else
                        {
                            var column5 = dr[5].ToString().Trim();
                            if (column5 != string.Empty)
                                YeuCau = float.Parse(column5);
                            LaPTYeuCau = true;
                        }
                        var column6 = dr[6].ToString().Trim();
                        if (column6 != string.Empty && column6 != "0")
                            TuVan = float.Parse(column6);
                        else
                        {
                            var column7 = dr[7].ToString().Trim();
                            if (column7 != string.Empty)
                                TuVan = float.Parse(column7);
                            LaPTTuVan = true;
                        }
                        var column8 = dr[8].ToString().Trim();
                        if (column8 != string.Empty && column8 != "0")
                            BanGoi = float.Parse(column8);
                        else
                        {
                            var column9 = dr[9].ToString().Trim();
                            if (column9 != string.Empty)
                                BanGoi = float.Parse(column9);
                            LaPTBanGoi = true;
                        }
                        List<SqlParameter> sqlPRM = new List<SqlParameter>();
                        sqlPRM.Add(new SqlParameter("ID_DonVi", ID_ChiNhanh));
                        sqlPRM.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                        sqlPRM.Add(new SqlParameter("MaHH", dr[0].ToString().Trim()));
                        sqlPRM.Add(new SqlParameter("ChietKhau", ChietKhau));
                        sqlPRM.Add(new SqlParameter("LaPTChietKhau", LaPTChietKhau));
                        sqlPRM.Add(new SqlParameter("TuVan", TuVan));
                        sqlPRM.Add(new SqlParameter("LaPTTuVan", LaPTTuVan));
                        sqlPRM.Add(new SqlParameter("YeuCau", YeuCau));
                        sqlPRM.Add(new SqlParameter("LaPTYeuCau", LaPTYeuCau));
                        sqlPRM.Add(new SqlParameter("Timezone", 7));
                        sqlPRM.Add(new SqlParameter("BanGoi", BanGoi));
                        sqlPRM.Add(new SqlParameter("LaPTBanGoi", LaPTBanGoi));
                        sqlPRM.Add(new SqlParameter("TheoCKThucHien", '0'));
                        db.Database.ExecuteSqlCommand("exec insert_ChietKhauMacDinhNhanVien @ID_DonVi, @ID_NhanVien, @MaHH, @ChietKhau, @LaPTChietKhau, @TuVan, @LaPTTuVan,@YeuCau,@LaPTYeuCau, @Timezone," +
                            "@BanGoi,@LaPTBanGoi, @TheoCKThucHien", sqlPRM.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                str = string.Concat("insert_ChietKhauMacDinhNhanVien ", e.InnerException, e.Message);
            }
            return str;
        }

        public List<List_DonViQuiDoi_ID_NhomHang> getList_DanhSachHangDieuChinh(Stream fileInput, Guid ID_ChiNhanh)
        {
            List<List_DonViQuiDoi_ID_NhomHang> lst = new List<List_DonViQuiDoi_ID_NhomHang>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            dt.Rows[0].Delete();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    List<SqlParameter> sqlPRM = new List<SqlParameter>();
                    sqlPRM.Add(new SqlParameter("MaLoHang", dt.Rows[i][0].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("MaHangHoa", dt.Rows[i][1].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("ID_DonVi", ID_ChiNhanh));
                    List<List_DonViQuiDoi_HH_LoHang> lst1 = db.Database.SqlQuery<List_DonViQuiDoi_HH_LoHang>("exec getList_HangHoabyMaHH_LoHang @MaLoHang, @MaHangHoa, @ID_DonVi", sqlPRM.ToArray()).ToList();
                    List_DonViQuiDoi_ID_NhomHang DM1 = new List_DonViQuiDoi_ID_NhomHang();
                    try
                    {
                        DM1.ID_DonViQuiDoi = lst1.FirstOrDefault().ID;
                        DM1.ID_LoHang = lst1.FirstOrDefault().ID_LoHang;
                        DM1.QuanLyTheoLoHang = lst1.FirstOrDefault().QuanLyTheoLoHang;
                        DM1.MaHangHoa = lst1.FirstOrDefault().MaHangHoa;
                        DM1.TenHangHoa = lst1.FirstOrDefault().TenHangHoa;
                        DM1.TenHangHoaFull = lst1.FirstOrDefault().TenHangHoaFull;
                        DM1.ThuocTinh_GiaTri = lst1.FirstOrDefault().ThuocTinh_GiaTri;
                        DM1.TenDonViTinh = lst1.FirstOrDefault().TenDonViTinh;
                        DM1.TenLoHang = lst1.FirstOrDefault().TenLoHang;
                        DM1.NgaySanXuat = lst1.FirstOrDefault().NgaySanXuat;
                        DM1.NgayHetHan = lst1.FirstOrDefault().NgayHetHan;
                        DM1.GiaVonHienTai = lst1.FirstOrDefault().GiaVon;
                        DM1.GiaVonMoi = double.Parse(dt.Rows[i][4].ToString().Trim());
                        double? ChenhLech = DM1.GiaVonMoi - DM1.GiaVonHienTai;
                        if (ChenhLech >= 0)
                        {
                            DM1.GiaVonTang = ChenhLech;
                            DM1.GiaVonGiam = 0;
                        }
                        else
                        {
                            DM1.GiaVonGiam = ChenhLech;
                            DM1.GiaVonTang = 0;
                        }
                        DM1.SoThuTu = i + 1;
                    }
                    catch
                    {
                        string a = dt.Rows[i][1].ToString().Trim();
                        DonViQuiDoi donViQuiDoi = db.DonViQuiDois.Where(x => x.MaHangHoa.Contains(a)).FirstOrDefault();
                        DM_HangHoa dM_HangHoa = db.DM_HangHoa.Where(x => x.ID == donViQuiDoi.ID_HangHoa).FirstOrDefault();
                        DM1.ID_DonViQuiDoi = donViQuiDoi.ID;
                        DM1.ID_LoHang = Guid.NewGuid();
                        DM1.QuanLyTheoLoHang = dM_HangHoa.QuanLyTheoLoHang == null ? false : dM_HangHoa.QuanLyTheoLoHang;
                        DM1.MaHangHoa = dt.Rows[i][1].ToString().Trim();
                        DM1.TenHangHoa = dM_HangHoa.TenHangHoa;
                        DM1.TenHangHoaFull = dM_HangHoa.TenHangHoa;
                        DM1.ThuocTinh_GiaTri = "";
                        DM1.TenDonViTinh = donViQuiDoi.TenDonViTinh;
                        DM1.TenLoHang = "";
                        DM1.NgaySanXuat = null;
                        DM1.NgayHetHan = null;
                        DM1.GiaVonHienTai = 0;
                        DM1.GiaVonMoi = double.Parse(dt.Rows[i][4].ToString().Trim());
                        double? ChenhLech = DM1.GiaVonMoi - DM1.GiaVonHienTai;
                        if (ChenhLech >= 0)
                        {
                            DM1.GiaVonTang = ChenhLech;
                            DM1.GiaVonGiam = 0;
                        }
                        else
                        {
                            DM1.GiaVonGiam = ChenhLech;
                            DM1.GiaVonTang = 0;
                        }
                        DM1.SoThuTu = i + 1;
                    }

                    lst.Add(DM1);
                }
            }
            return lst;
        }
        public List<Report_HangHoa_Chuyenhang_Import> getList_DanhSachHangKiemKho(Stream fileInput, Guid iddonvi, DateTime timeKK)
        {
            var _classDVQD = new classDonViQuiDoi(db);
            List<Report_HangHoa_Chuyenhang_Import> lst = new List<Report_HangHoa_Chuyenhang_Import>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxDisplayRange.ColumnCount;
            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    var maHangHoa = dt.Rows[i][1].ToString().Trim();
                    var soluongThucTe = dt.Rows[i][2].ToString().Trim();
                    List<SqlParameter> sqlPRM = new List<SqlParameter>();
                    sqlPRM.Add(new SqlParameter("MaLoHangIP", dt.Rows[i][0].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("MaHangHoaIP", maHangHoa));
                    sqlPRM.Add(new SqlParameter("ID_DonViIP", iddonvi));
                    sqlPRM.Add(new SqlParameter("TimeIP", timeKK));
                    List<Report_HangHoa_Chuyenhang_Import> lst1 = db.Database.SqlQuery<Report_HangHoa_Chuyenhang_Import>("exec getListDanhSachHHImportKiemKe @MaLoHangIP, @MaHangHoaIP, @ID_DonViIP, @TimeIP", sqlPRM.ToArray()).ToList();
                    if (lst1.Count > 0)
                    {
                        var itFirst = lst1.FirstOrDefault();
                        Report_HangHoa_Chuyenhang_Import DM1 = new Report_HangHoa_Chuyenhang_Import();
                        DM1.MaHangHoa = itFirst.MaHangHoa;
                        DM1.TenHangHoa = itFirst.TenHangHoa;
                        DM1.ThanhTien = Math.Round(double.Parse(soluongThucTe), 3, MidpointRounding.ToEven);
                        DM1.ID = itFirst.ID;
                        DM1.ID_DonViQuiDoi = itFirst.ID_DonViQuiDoi;
                        DM1.TenDonViTinh = itFirst.TenDonViTinh;
                        DM1.QuanLyTheoLoHang = itFirst.QuanLyTheoLoHang;
                        DM1.TyLeChuyenDoi = itFirst.TyLeChuyenDoi;
                        DM1.MaLoHang = itFirst.MaLoHang;
                        DM1.ID_LoHang = itFirst.ID_LoHang;
                        DM1.ThuocTinh_GiaTri = itFirst.ThuocTinh_GiaTri;
                        DM1.DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == DM1.ID && ct.Xoa != true).Select(x => new DonViTinh
                        {
                            ID_HangHoa = DM1.ID,
                            TenDonViTinh = x.TenDonViTinh,
                            ID_DonViQuiDoi = x.ID,
                            QuanLyTheoLoHang = DM1.QuanLyTheoLoHang,
                            Xoa = true,
                            TyLeChuyenDoi = x.TyLeChuyenDoi
                        }).ToList();
                        lst.Add(DM1);
                    }
                }
            }
            return lst;
        }
        public List<DM_HangHoaDTO> getList_DanhSachHangDieuChuyen(Stream fileInput, Guid iddonvi)
        {
            var _classDVQD = new classDonViQuiDoi(db);
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool);
            dt.Rows[0].Delete();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dtRow = dt.Rows[i];
                var giachuyen = dtRow[3].ToString().Trim();
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("MaLoHangIP", dtRow[0].ToString().Trim()));
                sqlPRM.Add(new SqlParameter("MaHangHoaIP", dtRow[1].ToString().Trim()));
                sqlPRM.Add(new SqlParameter("ID_DonViIP", iddonvi));
                sqlPRM.Add(new SqlParameter("TimeIP", DateTime.Now));
                List<DM_HangHoaDTO> lst1 = db.Database.SqlQuery<DM_HangHoaDTO>("exec getListDanhSachHHImport @MaLoHangIP, @MaHangHoaIP, @ID_DonViIP, @TimeIP", sqlPRM.ToArray()).ToList();
                var hhFirst = lst1.FirstOrDefault();
                DM_HangHoaDTO DM1 = new DM_HangHoaDTO();
                DM1.MaHangHoa = dtRow[1].ToString().Trim();
                DM1.TenHangHoa = hhFirst.TenHangHoa;
                DM1.SoLuong = double.Parse(dtRow[2].ToString().Trim());
                DM1.ID = hhFirst.ID;
                DM1.ID_DonViQuiDoi = hhFirst.ID_DonViQuiDoi;
                DM1.DonGia = giachuyen != "" ? double.Parse(giachuyen) : 0;
                DM1.TenDonViTinh = hhFirst.TenDonViTinh;
                DM1.ThanhToan = 0;
                DM1.QuanLyTheoLoHang = hhFirst.QuanLyTheoLoHang;
                DM1.TyLeChuyenDoi = hhFirst.TyLeChuyenDoi;
                DM1.MaLoHang = hhFirst.MaLoHang;
                DM1.ID_LoHang = hhFirst.ID_LoHang;
                DM1.GiaVon = giachuyen != "" ? double.Parse(giachuyen) : hhFirst.GiaVon.Value;
                DM1.ThanhTien = DM1.GiaVon * DM1.SoLuong;
                DM1.TonKho = hhFirst.TonKho;
                DM1.ThuocTinh_GiaTri = hhFirst.ThuocTinh_GiaTri;
                DM1.NgaySanXuat = hhFirst.NgaySanXuat;
                DM1.NgayHetHan = hhFirst.NgayHetHan;
                DM1.ThuocTinh_GiaTri = hhFirst.ThuocTinh_GiaTri;
                DM1.DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == DM1.ID && ct.Xoa != true).Select(x => new DonViTinh
                {
                    ID_HangHoa = DM1.ID,
                    TenDonViTinh = x.TenDonViTinh,
                    ID_DonViQuiDoi = x.ID,
                    QuanLyTheoLoHang = DM1.QuanLyTheoLoHang,
                    Xoa = true,
                    TyLeChuyenDoi = x.TyLeChuyenDoi
                }).ToList();
                lst.Add(DM1);
            }
            return lst;
        }
        public List<DM_HangHoaDTO> getList_DanhSachHangnhap(Stream fileInput, Guid iddonvi)
        {
            var _classDVQD = new classDonViQuiDoi(db);
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxDisplayRange.ColumnCount;
            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool, true);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dtR = dt.Rows[i];
                var ptThue = dtR[7].ToString().Trim();
                var tienThue = dtR[8].ToString().Trim();
                var ngaySX = dtR[1].ToString();
                var ngayhethan = dtR[2].ToString().Trim();
                var giaban = dtR[9].ToString().Trim();

                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("MaLoHangIP", dt.Rows[i][0].ToString().Trim()));
                sqlPRM.Add(new SqlParameter("MaHangHoaIP", dt.Rows[i][3].ToString().Trim()));
                sqlPRM.Add(new SqlParameter("ID_DonViIP", iddonvi));
                sqlPRM.Add(new SqlParameter("TimeIP", DateTime.Now));
                List<DM_HangHoaDTO> lst1 = db.Database.SqlQuery<DM_HangHoaDTO>("exec getListDanhSachHHImport @MaLoHangIP, @MaHangHoaIP, @ID_DonViIP, @TimeIP", sqlPRM.ToArray()).ToList();
                if (lst1 != null && lst1.Count() > 0)
                {
                    var itemFirst = lst1.FirstOrDefault();
                    DM_HangHoaDTO DM1 = new DM_HangHoaDTO();
                    DM1.MaHangHoa = dt.Rows[i][3].ToString();
                    DM1.TenHangHoa = itemFirst.TenHangHoa;
                    DM1.SoLuong = double.Parse(dt.Rows[i][4].ToString());
                    DM1.GiaBan = string.IsNullOrEmpty(giaban) ? itemFirst.GiaBan : double.Parse(giaban);
                    DM1.ChangeGiaBan = string.IsNullOrEmpty(giaban) ? 0 : 1;//0. khong thaydoi bgchung/else. thaydoi
                    DM1.DonGia = double.Parse(dt.Rows[i][5].ToString());
                    DM1.ID = itemFirst.ID;
                    DM1.ID_DonViQuiDoi = itemFirst.ID_DonViQuiDoi;
                    DM1.GiaNhap = itemFirst.GiaNhap;
                    DM1.TienChietKhau = dt.Rows[i][6].ToString() == "" ? 0 : double.Parse(dt.Rows[i][6].ToString());
                    DM1.PTThue = string.IsNullOrEmpty(ptThue) ? 0 : double.Parse(ptThue);
                    DM1.TienThue = string.IsNullOrEmpty(tienThue) ? 0 : double.Parse(tienThue);
                    DM1.TenDonViTinh = itemFirst.TenDonViTinh;
                    DM1.ThanhTien = DM1.SoLuong * (DM1.DonGia - DM1.TienChietKhau);
                    DM1.ThanhToan = DM1.ThanhTien;
                    DM1.TyLeChuyenDoi = itemFirst.TyLeChuyenDoi;
                    DM1.QuanLyTheoLoHang = itemFirst.QuanLyTheoLoHang;
                    DM1.ID_LoHang = itemFirst.ID_LoHang;
                    DM1.GiaVon = itemFirst.GiaVon;
                    DM1.MaLoHang = itemFirst.ID_LoHang == null ? dt.Rows[i][0].ToString() : itemFirst.MaLoHang;
                    DM1.NgaySanXuat = itemFirst.ID_LoHang == null ? (ngaySX != "" ? DateTime.Parse(ngaySX) : (DateTime?)null) : itemFirst.NgaySanXuat;
                    DM1.NgayHetHan = itemFirst.ID_LoHang == null ? (ngayhethan != "" ? DateTime.Parse(ngayhethan) : (DateTime?)null) : itemFirst.NgayHetHan;
                    DM1.TonKho = itemFirst.TonKho;
                    DM1.ThuocTinh_GiaTri = itemFirst.ThuocTinh_GiaTri;
                    DM1.DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == DM1.ID && ct.Xoa != true).Select(x => new DonViTinh
                    {
                        ID_HangHoa = DM1.ID,
                        TenDonViTinh = x.TenDonViTinh,
                        ID_DonViQuiDoi = x.ID,
                        QuanLyTheoLoHang = DM1.QuanLyTheoLoHang,
                        Xoa = true,
                        TyLeChuyenDoi = x.TyLeChuyenDoi
                    }).ToList();
                    lst.Add(DM1);
                }
            }
            return lst;
        }

        public List<DM_HangHoaDTO> getList_HangHoa(string MaHangHoa)
        {
            var _classDMHH = new ClassDM_HangHoa(db);
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            var tbl = db.DonViQuiDois.Where(x => x.MaHangHoa == MaHangHoa.Trim());
            DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
            dM_HangHoaDTO.ID = tbl.FirstOrDefault().ID_HangHoa; //idhanghoa
            dM_HangHoaDTO.QuanLyTheoLoHang = _classDMHH.Select_HangHoa(tbl.FirstOrDefault().ID_HangHoa).QuanLyTheoLoHang;
            dM_HangHoaDTO.TenHangHoa = _classDMHH.Select_HangHoa(tbl.FirstOrDefault().ID_HangHoa).TenHangHoa;
            dM_HangHoaDTO.GiaBan = tbl.FirstOrDefault().GiaBan;
            dM_HangHoaDTO.GiaNhap = tbl.FirstOrDefault().GiaNhap;
            dM_HangHoaDTO.TenDonViTinh = tbl.FirstOrDefault().TenDonViTinh;
            dM_HangHoaDTO.ID_DonViQuiDoi = tbl.FirstOrDefault().ID;
            dM_HangHoaDTO.TyLeChuyenDoi = tbl.FirstOrDefault().TyLeChuyenDoi;
            lst.Add(dM_HangHoaDTO);
            return lst;
        }
        public List<DM_HangHoaDTO> getList_HangHoaChuyenhang(string MaHangHoa)
        {
            var _classDMHH = new ClassDM_HangHoa(db);
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            var tbl = db.DonViQuiDois.Where(x => x.MaHangHoa == MaHangHoa.Trim());
            DM_HangHoaDTO dM_HangHoaDTO = new DM_HangHoaDTO();
            dM_HangHoaDTO.ID = tbl.FirstOrDefault().ID_HangHoa; //idhanghoa
            dM_HangHoaDTO.QuanLyTheoLoHang = _classDMHH.Select_HangHoa(tbl.FirstOrDefault().ID_HangHoa).QuanLyTheoLoHang;
            dM_HangHoaDTO.TyLeChuyenDoi = tbl.FirstOrDefault().TyLeChuyenDoi;
            dM_HangHoaDTO.TenHangHoa = _classDMHH.Select_HangHoa(tbl.FirstOrDefault().ID_HangHoa).TenHangHoa;
            //dM_HangHoaDTO.GiaVon = Math.Round((double)tbl.FirstOrDefault().GiaVon, MidpointRounding.ToEven); // 1* DonGia
            dM_HangHoaDTO.TenDonViTinh = tbl.FirstOrDefault().TenDonViTinh;
            dM_HangHoaDTO.ID_DonViQuiDoi = tbl.FirstOrDefault().ID;
            lst.Add(dM_HangHoaDTO);
            return lst;
        }
        public List<ErrorDMHangHoa> checkExcel_TraHangNhap(Stream fileInput)
        {
            List<ErrorDMHangHoa> lst = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            dt.Columns[1].ColumnName = "MaHang";
            dt.Columns[0].ColumnName = "MaLo";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                var dtRow = dt.Rows[i];
                var index = (i + 3).ToString();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dtRow[j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    string malo = dtRow[0].ToString().Trim();
                    string mahang = dtRow[1].ToString().Trim();
                    string soluong = dtRow[2].ToString().Trim();
                    string dongia = dtRow[3].ToString().Trim();

                    if (malo != "")
                    {
                        bool checklo = ChekLoHangDatabase(malo, mahang);
                        if (checklo == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Lô hàng '" + malo + "' không có trên hệ thống";
                            lst.Add(DM);
                        }
                    }
                    if (mahang == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool kytudacbiet = kiemtrakitu(mahang);
                        if (kytudacbiet == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được chứa ký tự đặc biệt";
                            lst.Add(DM);
                        }
                        bool trungma = GroupData(dt, "MaHang = '" + mahang + (malo != "" ? "' and MaLo = '" + malo + "'" : "" + "'"));
                        if (trungma == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            string Lo = string.Empty;
                            if (mahang != "")
                                Lo = " (" + malo + ")";
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahang + Lo + "' bị trùng lặp";
                            lst.Add(DM);
                        }
                        bool CheckCSDL = ChekMaHangDatabase_DangKinhDoanh(mahang);
                        if (CheckCSDL == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahang + "' không có trên hệ thống hoặc ngừng kinh doanh";
                            lst.Add(DM);
                        }
                        bool CheckLaDV = ChekMaHangDatabase_LaDichVu(mahang);
                        if (CheckLaDV == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahang + "' không phải là hàng hóa";
                            lst.Add(DM);
                        }
                    }
                    if (soluong == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Số lượng không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool isNumber7 = IsNumber(soluong);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Số lượng '" + soluong + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                    if (dongia == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.DienGiai = "Dòng số " + index + ": Giá trả lại không được để trống";
                        lst.Add(DM);
                    }
                    else
                    {
                        bool isNumber7 = IsNumber(dongia);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.DienGiai = "Dòng số " + index + ": Giá trả lại '" + dongia + "'  không phải dạng số";
                            lst.Add(DM);
                        }
                    }
                }
            }
            return lst;
        }
        public List<DM_HangHoaDTO> getList_DanhSachTraHangNhap(Stream fileInput, Guid iddonvi)
        {
            var _classDVQD = new classDonViQuiDoi(db);
            List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var malo = dt.Rows[i][0].ToString().Trim();
                var mahang = dt.Rows[i][1].ToString().Trim();
                var soluong = dt.Rows[i][2].ToString().Trim();
                var dongia = dt.Rows[i][3].ToString().Trim();

                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("MaLoHangIP", malo));
                sqlPRM.Add(new SqlParameter("MaHangHoaIP", mahang));
                sqlPRM.Add(new SqlParameter("ID_DonViIP", iddonvi));
                sqlPRM.Add(new SqlParameter("TimeIP", DateTime.Now));
                List<DM_HangHoaDTO> lst1 = db.Database.SqlQuery<DM_HangHoaDTO>("exec getListDanhSachHHImport @MaLoHangIP, @MaHangHoaIP, @ID_DonViIP, @TimeIP", sqlPRM.ToArray()).ToList();
                DM_HangHoaDTO DM1 = new DM_HangHoaDTO();
                DM1.MaHangHoa = mahang;
                DM1.TenHangHoa = lst1.FirstOrDefault().TenHangHoa;
                DM1.SoLuong = double.Parse(soluong);
                DM1.SoLuongTra = double.Parse(soluong);
                DM1.GiaTraLai = double.Parse(dongia);
                DM1.GiaBan = lst1.FirstOrDefault().GiaNhap == 0 ? lst1.FirstOrDefault().GiaVon : lst1.FirstOrDefault().GiaNhap;
                DM1.DonGia = double.Parse(dongia);
                DM1.ID = lst1.FirstOrDefault().ID;
                DM1.ID_DonViQuiDoi = lst1.FirstOrDefault().ID_DonViQuiDoi;
                DM1.TyLeChuyenDoi = lst1.FirstOrDefault().TyLeChuyenDoi;
                DM1.GiaNhap = lst1.FirstOrDefault().GiaNhap;
                DM1.TenDonViTinh = lst1.FirstOrDefault().TenDonViTinh;
                DM1.ThanhTien = DM1.SoLuongTra * DM1.GiaTraLai;
                DM1.ThanhToan = 0;
                DM1.QuanLyTheoLoHang = lst1.FirstOrDefault().QuanLyTheoLoHang;
                DM1.MaLoHang = lst1.FirstOrDefault().MaLoHang;
                DM1.ID_LoHang = lst1.FirstOrDefault().ID_LoHang;
                DM1.TonKho = lst1.FirstOrDefault().TonKho;
                DM1.GiaVon = lst1.FirstOrDefault().GiaVon;
                DM1.ThuocTinh_GiaTri = lst1.FirstOrDefault().ThuocTinh_GiaTri;
                DM1.DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == DM1.ID && ct.Xoa != true).Select(x => new DonViTinh
                {
                    ID_HangHoa = DM1.ID,
                    TenDonViTinh = x.TenDonViTinh,
                    ID_DonViQuiDoi = x.ID,
                    QuanLyTheoLoHang = DM1.QuanLyTheoLoHang,
                    Xoa = false,
                    TyLeChuyenDoi = x.TyLeChuyenDoi
                }).ToList();
                lst.Add(DM1);
            }
            return lst;
        }

        public List<ErrorDMHangHoa> checkExcel_BangGia(Stream fileInput, Guid ID_DonVi, Guid ID_BangGia)
        {
            List<ErrorDMHangHoa> lst = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            dt.Columns[0].ColumnName = "MaHang";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                var index = (i + 3).ToString();
                DataRow dtRow = dt.Rows[i];

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    var mahanghoa = dtRow[0].ToString().Trim();
                    var giamoi = dtRow[1].ToString().Trim();
                    if (string.IsNullOrEmpty(mahanghoa))
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Mã hàng hóa";
                        DM.ViTri = "Dòng số: " + index;
                        DM.ThuocTinh = mahanghoa;
                        DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được để trống";
                        DM.rowError = i;
                        lst.Add(DM);
                    }
                    else
                    {
                        bool kytudacbiet = kiemtrakitu(mahanghoa);
                        if (kytudacbiet == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng hóa";
                            DM.ViTri = "Dòng số: " + index;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng hóa không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            lst.Add(DM);
                        }
                        bool trungma = GroupData(dt, "MaHang = '" + mahanghoa + "'");
                        if (trungma == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng hóa";
                            DM.ViTri = "Dòng số: " + index;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahanghoa + "' bị trùng lặp";
                            DM.rowError = i;
                            lst.Add(DM);
                        }
                        bool CheckCSDL = ChekMaHangDatabase_DangKinhDoanh(mahanghoa);
                        if (CheckCSDL == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng hóa";
                            DM.ViTri = "Dòng số: " + index;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahanghoa + "' không có trên hệ thống hoặc ngừng kinh doanh";
                            DM.rowError = i;
                            lst.Add(DM);
                        }
                        bool checkHHInBG = ChekMaHangCoTrongBangGia(mahanghoa, ID_BangGia);
                        if (checkHHInBG == true)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng hóa";
                            DM.ViTri = "Dòng số: " + index;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Dòng số " + index + ": Mã hàng '" + mahanghoa + "' đã có trong bảng giá";
                            DM.rowError = i;
                            lst.Add(DM);
                        }
                    }
                    if (giamoi.Trim() != "")
                    {
                        bool isNumber = IsNumberInt(giamoi);
                        if (isNumber == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Giá mới";
                            DM.ViTri = "Dòng số: " + index;
                            DM.ThuocTinh = giamoi;
                            DM.DienGiai = "Dòng số " + index + ": giá mới '" + giamoi + "'  không phải dạng số";
                            DM.rowError = i;
                            lst.Add(DM);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(giamoi))
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Giá mới";
                        DM.ViTri = "Dòng số: " + index;
                        DM.ThuocTinh = giamoi;
                        DM.DienGiai = "Dòng số " + index + ": giá mới không được để trống";
                        DM.rowError = i;
                        lst.Add(DM);
                    }
                }
            }
            if (lst.Count > 0)
            {
                return lst;
            }
            else
            {
                ImportBangGiaDTO(dt, ID_DonVi, ID_BangGia);
                return null;
            }
        }

        public List<ErrorDMHangHoa> checkExcel(Stream fileInput, Guid ID_DonVi, Guid idnhanvien)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;

            ExportTableOptions options = new ExportTableOptions();
            options.ExportColumnName = true;
            options.IsVertical = true;
            options.ExportAsString = true;
            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            List<string> lst = new List<string>();
            for (int i = 16; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Trim() != "…" & dt.Rows[0][i].ToString().Trim() != "")
                    lst.Add(dt.Rows[0][i].ToString().Trim());
            }
            dt.Rows[0].Delete();
            dt.Columns[2].ColumnName = "MaHang";
            dt.Columns[10].ColumnName = "TenDVT";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                DataRow dtRow = dt.Rows[i];
                string rowIndex = "Dòng số: " + (i + 4).ToString();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    var nhomcha = dtRow[0].ToString().Trim();
                    var nhomcon = dtRow[1].ToString().Trim();
                    var mahanghoa = dtRow[2].ToString().Trim();
                    var tenhanghoa = dtRow[3].ToString().Trim();
                    var lahanghoa = dtRow[4].ToString().Trim() != "" ? false : true;
                    var duocbantructiep = dtRow[5].ToString().Trim() != "" ? false : true;
                    var ghichu = dtRow[6].ToString().Trim();
                    var giavon = dtRow[7].ToString().Trim();
                    var giaban = dtRow[8].ToString().Trim();
                    var tonkho = dtRow[9].ToString().Trim();
                    var tenDVT = dtRow[10].ToString().Trim();
                    var quycach = dtRow[11].ToString().Trim();
                    var dvQuyCach = dtRow[12].ToString().Trim();
                    var madvcoban = dtRow[13].ToString().Trim();
                    var ladvchuan = madvcoban != "" ? false : true;
                    var gtriquydoi = dtRow[14].ToString().Trim();
                    var macungloai = dtRow[15].ToString().Trim();

                    var tenhang_KhongDau = CommonStatic.ConvertToUnSign(tenhanghoa).ToLower();
                    var tenhang_KyTuDau = CommonStatic.GetCharsStart(tenhanghoa).ToLower();
                    var nhomcha_KhongDau = CommonStatic.ConvertToUnSign(nhomcha).ToLower();
                    var nhomcha_KyTuDau = CommonStatic.GetCharsStart(nhomcha).ToLower();
                    var nhomcon_KhongDau = CommonStatic.ConvertToUnSign(nhomcon).ToLower();
                    var nhomcon_KyTuDau = CommonStatic.GetCharsStart(nhomcon).ToLower();

                    bool kytudacbiet = kiemtrakitu(mahanghoa);
                    if (kytudacbiet == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Mã hàng/dịch vụ";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = mahanghoa;
                        DM.DienGiai = "Mã hàng hóa không được chứa ký tự đặc biệt";
                        DM.rowError = i;
                        lstError.Add(DM);
                    }
                    else
                    {
                        bool trungma = GroupData(dt, "MaHang = '" + mahanghoa + "'");
                        if (trungma == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng/dịch vụ";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Mã hàng: " + mahanghoa + " bị trùng lặp";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                    bool CheckCSDL = ChekMaHangDatabase(mahanghoa);
                    if (CheckCSDL == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Mã hàng/dịch vụ";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = mahanghoa;
                        DM.DienGiai = "Mã hàng/dịch vụ: " + mahanghoa + " đã tồn tại trong cơ sở dữ liệu";
                        DM.rowError = i;
                        lstError.Add(DM);
                    }

                    if (tenhanghoa == "" | tenhanghoa == "null" | tenhanghoa == null)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = dt.Columns[3].ColumnName;
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = tenhanghoa;
                        DM.DienGiai = "Tên hàng hóa không được để trống";
                        DM.rowError = i;
                        lstError.Add(DM);
                    }
                    if (dtRow[4].ToString() != "x" && dtRow[4].ToString() != "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = dt.Columns[4].ColumnName;
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = dtRow[4].ToString();
                        DM.DienGiai = "Là dịch vụ bạn cần đánh dấu: x";
                        DM.rowError = i;
                        lstError.Add(DM);
                    }
                    if (dtRow[5].ToString() != "x" && dtRow[5].ToString() != "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = dt.Columns[5].ColumnName;
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = dtRow[5].ToString();
                        DM.DienGiai = "Là hàng hóa không được bán trực tiếp bạn cần đánh dấu x";
                        DM.rowError = i;
                        lstError.Add(DM);
                    }
                    if (giavon != "")
                    {
                        bool isNumber7 = IsNumber(giavon);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[7].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = giavon;
                            DM.DienGiai = "Giá vốn không phải dạng số";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                    if (giaban != "")
                    {
                        bool isNumber8 = IsNumber(giaban);
                        if (isNumber8 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[8].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = giaban;
                            DM.DienGiai = "Giá bán không phải dạng số";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                    if (tenDVT != "")
                    {
                        bool kytudacbiet4 = kiemtrakitu(tenDVT);
                        if (kytudacbiet4 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tên Đơn vị tính";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = tenDVT;
                            DM.DienGiai = "Tên đơn vị tính không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                    if (madvcoban != "" && tenDVT == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Tên đơn vị tính";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = tenDVT;
                        DM.DienGiai = "Tên đơn vị tính không được để trống khi có mã đơn vị cơ bản";
                        DM.rowError = i;
                        lstError.Add(DM);
                    }
                    if (madvcoban != "" && gtriquydoi == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Giá trị quy đổi";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = madvcoban;
                        DM.DienGiai = "Giá trị quy đổi không được để trống khi có mã đơn vị cơ bản";
                        DM.rowError = i;
                        lstError.Add(DM);
                    }
                    if (quycach != "")
                    {
                        bool isNumber = IsNumber(quycach);
                        if (isNumber == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[11].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = quycach;
                            DM.DienGiai = "Quy cách không phải dạng số";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                    if (madvcoban != "")
                    {
                        bool kytudacbiet12 = kiemtrakitu(madvcoban);
                        if (kytudacbiet12 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = madvcoban;
                            DM.DienGiai = "Mã đơn vị tính cơ bản không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        bool trungma12 = GroupDataMaDVTCoBan(dt, " TRIM(MaHang) = '" + madvcoban + "' and TenDVT <> ''");
                        if (trungma12 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = madvcoban;
                            DM.DienGiai = "Mã đơn vị tính cơ bản: " + madvcoban + " không tồn tại hoặc chưa có tên đơn vị tính cơ bản";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                    if (gtriquydoi != "")
                    {
                        bool Isnumber13 = IsNumber(gtriquydoi);
                        if (Isnumber13 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Giá trị quy đổi";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = gtriquydoi;
                            DM.DienGiai = "Giá trị quy đổi không phải là dạng số";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }

                    if (!string.IsNullOrEmpty(macungloai))
                    {
                        string existChaCungLoai = ChekMaHangDatabase_Update(macungloai);
                        if (string.IsNullOrEmpty(existChaCungLoai))
                        {
                            // find row by macungloai
                            var rowCungLoai = dt.AsEnumerable().Where(x => x.Field<string>(dt.Columns[2].ColumnName.Trim()) == macungloai);
                            if (rowCungLoai == null || rowCungLoai.Count() == 0)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã cha cùng loại";
                                DM.ViTri = rowIndex;
                                DM.ThuocTinh = string.Concat(macungloai);
                                DM.DienGiai = "Mã cha cùng loại chưa tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                DM.loaiError = 1;
                                lstError.Add(DM);
                            }
                        }
                    }
                }


            }
            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                lstError = ImportDanhMucHangHoaDTO(lst, dt, ID_DonVi, idnhanvien, 1);
                return lstError;
            }
        }
        public List<exportHangHoaCapNhatPRC> export_HangHoaCapNhat(Stream fileInput, Guid ID_DonVi, string RownError, int LoaiUpdate)
        {
            List<exportHangHoaCapNhatPRC> lstExcel = new List<exportHangHoaCapNhatPRC>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;

            ExportTableOptions options = new ExportTableOptions();
            options.ExportColumnName = true;
            options.IsVertical = true;
            options.ExportAsString = true;

            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            dt.Rows[0].Delete();
            if (RownError != "null" & RownError != string.Empty & RownError != null)
            {
                string[] mang = RownError.Split('_');
                for (int i = mang.Length - 1; i >= 0; i--)
                {
                    int j = int.Parse(mang[i].ToString());
                    dt.Rows[j].Delete();
                }
            }
            dt.Columns[2].ColumnName = "MaHang";
            dt.Columns[10].ColumnName = "TenDVT";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string MaHangHoa = dt.Rows[i][2].ToString().Trim();
                if (dt.Rows[i][2].ToString().Trim() != "")
                {
                    DonViQuiDoi donViQuiDoi = db.DonViQuiDois.Where(x => x.MaHangHoa == MaHangHoa).FirstOrDefault();
                    if (donViQuiDoi != null)
                    {
                        DM_HangHoa dM_HangHoa = db.DM_HangHoa.Where(x => x.ID == donViQuiDoi.ID_HangHoa).FirstOrDefault();
                        DM_NhomHangHoa dM_NhomHangHoa = db.DM_NhomHangHoa.Where(x => x.ID == dM_HangHoa.ID_NhomHang).FirstOrDefault();
                        List<SqlParameter> paramlist = new List<SqlParameter>();
                        paramlist.Add(new SqlParameter("MaHH", dt.Rows[i][2].ToString().Trim().ToUpper()));
                        paramlist.Add(new SqlParameter("MaHH_TV", dt.Rows[i][2].ToString().Trim().ToUpper()));
                        paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_DonVi));
                        List<Search_HangHoa_XuatNhapTonPRC> lstTon = db.Database.SqlQuery<Search_HangHoa_XuatNhapTonPRC>("exec Search_DMHangHoa_TonKho @MaHH, @MaHH_TV, @ID_ChiNhanh", paramlist.ToArray()).ToList();
                        double TonKho = float.Parse(dt.Rows[i][9].ToString().Trim().Replace(",", ""));
                        if (LoaiUpdate == 1)
                        {
                            TonKho = lstTon.FirstOrDefault().TonCuoiKy;
                        }
                        if (dM_NhomHangHoa.TenNhomHangHoa != dt.Rows[i][1].ToString().Trim() || (dM_HangHoa.TenHangHoa != dt.Rows[i][3].ToString().Trim() & dt.Rows[i][3].ToString().Trim() != "")
                            || dM_HangHoa.GhiChu != dt.Rows[i][6].ToString().Trim() || donViQuiDoi.GiaBan != float.Parse(dt.Rows[i][8].ToString().Trim().Replace(",", ""))
                            || lstTon.FirstOrDefault().TonCuoiKy != TonKho)
                        {
                            exportHangHoaCapNhatPRC DM = new exportHangHoaCapNhatPRC();
                            DM.MaHangHoa_sql = dt.Rows[i][2].ToString().Trim();
                            DM.NhomHangHoa_sql = dM_NhomHangHoa != null ? dM_NhomHangHoa.TenNhomHangHoa : "";
                            DM.TenHangHoa_sql = dM_HangHoa.TenHangHoa;
                            DM.GhiChu_sql = dM_HangHoa.GhiChu;
                            DM.GiaBan_sql = donViQuiDoi.GiaBan;
                            DM.TonKho_sql = lstTon.FirstOrDefault().TonCuoiKy;
                            DM.NhomHangHoa_excel = dt.Rows[i][1].ToString().Trim();
                            DM.TenHangHoa_excel = dt.Rows[i][3].ToString().Trim() != "" ? dt.Rows[i][3].ToString().Trim() : dM_HangHoa.TenHangHoa;
                            DM.GhiChu_excel = dt.Rows[i][6].ToString().Trim();
                            DM.GiaBan_excel = float.Parse(dt.Rows[i][8].ToString().Trim().Replace(",", ""));
                            DM.TonKho_excel = TonKho;
                            lstExcel.Add(DM);
                        }
                    }
                }
            }
            return lstExcel;
        }

        public List<ErrorDMHangHoa> checkExcel_HangHoa(Stream fileInput, Guid ID_DonVi, Guid idnhanvien, int LoaiUpdate)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;

            ExportTableOptions options = new ExportTableOptions();
            options.ExportColumnName = true;
            options.IsVertical = true;
            options.ExportAsString = true;
            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            List<string> lst = new List<string>();
            for (int i = 16; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Trim() != "…" & dt.Rows[0][i].ToString().Trim() != "")
                    lst.Add(dt.Rows[0][i].ToString().Trim());
            }
            dt.Rows[0].Delete();
            dt.Columns[2].ColumnName = "MaHang";
            dt.Columns[10].ColumnName = "TenDVT";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                DataRow dtRow = dt.Rows[i];
                string rowIndex = "Dòng số: " + (i + 4).ToString();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    var mahanghoa = dtRow[2].ToString().Trim();
                    var tenhanghoa = dtRow[3].ToString().Trim();
                    var mdvtcoban = dtRow[13].ToString().Trim();
                    var gtriquydoi = dtRow[14].ToString().Trim();
                    var machaCungLoai = dtRow[15].ToString().Trim();
                    var loaihang = dtRow[4].ToString().Trim();

                    bool trungma = GroupData(dt, "MaHang = '" + mahanghoa + "'");
                    if (trungma == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Mã hàng/dịch vụ";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = mahanghoa;
                        DM.DienGiai = "Mã hàng: " + mahanghoa + " bị trùng lặp";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }

                    string CheckCSDL = ChekMaHangDatabase_Update(mahanghoa);
                    if (CheckCSDL != string.Empty)
                    {
                        if (CheckCSDL.Trim() != tenhanghoa & tenhanghoa != "" & tenhanghoa != "null" & tenhanghoa != null)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng/dịch vụ";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Mã hàng/dịch vụ: " + mahanghoa + " đã tồn tại trong cơ sở dữ liệu";
                            DM.rowError = i;
                            DM.loaiError = 2;
                            DM.nameHH_excel = tenhanghoa;
                            DM.nameHH_sql = CheckCSDL.Trim();
                            lstError.Add(DM);
                        }
                    }
                    else
                    {
                        if ((string.IsNullOrEmpty(tenhanghoa) || tenhanghoa == "null") && mdvtcoban == "" && string.IsNullOrEmpty(machaCungLoai))
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[3].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = tenhanghoa;
                            DM.DienGiai = "Tên hàng hóa không được để trống";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }

                    if (!string.IsNullOrEmpty(loaihang))
                    {
                        if (loaihang != "Combo" && loaihang != "Dịch vụ")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[4].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = loaihang;
                            DM.DienGiai = "Loại hàng hóa chưa được định nghĩa";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }

                    if (dtRow[5].ToString() != "x" && dtRow[5].ToString() != "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = dt.Columns[5].ColumnName;
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = dtRow[5].ToString();
                        DM.DienGiai = "Là hàng hóa không được bán trực tiếp bạn cần đánh dấu x";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }
                    if (dtRow[7].ToString() != "")
                    {
                        bool isNumber7 = IsNumber(dtRow[7].ToString());
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[7].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = dtRow[7].ToString();
                            DM.DienGiai = "Giá vốn không phải dạng số";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                    if (dtRow[8].ToString() != "")
                    {
                        bool isNumber8 = IsNumber(dtRow[8].ToString());
                        if (isNumber8 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[8].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = dtRow[8].ToString();
                            DM.DienGiai = "Giá bán không phải dạng số";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                    if (dtRow[10].ToString() != "")
                    {
                        bool kytudacbiet4 = kiemtrakitu(dtRow[10].ToString());
                        if (kytudacbiet4 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tên Đơn vị tính";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = dtRow[10].ToString();
                            DM.DienGiai = "Tên đơn vị tính không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                    if (mdvtcoban != "" && dtRow[10].ToString() == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Tên đơn vị tính";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = dtRow[10].ToString();
                        DM.DienGiai = "Tên đơn vị tính không được để trống khi có mã đơn vị cơ bản";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }
                    if (mdvtcoban != "" && gtriquydoi == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Giá trị quy đổi";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = gtriquydoi;
                        DM.DienGiai = "Giá trị quy đổi không được để trống khi có mã đơn vị cơ bản";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }
                    if (dtRow[11].ToString() != "")
                    {
                        bool isNumber = IsNumber(dtRow[11].ToString());
                        if (isNumber == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[11].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = dtRow[11].ToString();
                            DM.DienGiai = "Quy cách không phải dạng số";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                    if (mdvtcoban != "")
                    {
                        if (mdvtcoban == mahanghoa)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = mdvtcoban;
                            DM.DienGiai = "Mã đơn vị tính cơ bản trùng với mã hàng hóa";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                        bool kytudacbiet12 = kiemtrakitu(mdvtcoban);
                        if (kytudacbiet12 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = mdvtcoban;
                            DM.DienGiai = "Mã đơn vị tính cơ bản không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                        bool trungma12 = GroupDataMaDVTCoBan(dt, " TRIM(MaHang) = '" + mdvtcoban + "' and TenDVT <> ''");
                        if (trungma12 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = mdvtcoban;
                            DM.DienGiai = "Mã đơn vị tính cơ bản: " + mdvtcoban + " không tồn tại hoặc chưa có tên đơn vị tính cơ bản";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                    if (gtriquydoi != "")
                    {
                        bool Isnumber13 = IsNumber(gtriquydoi);
                        if (Isnumber13 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Giá trị quy đổi";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = gtriquydoi;
                            DM.DienGiai = "Giá trị quy đổi không phải là dạng số";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                }
            }
            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                lstError = ImportDanhMucHangHoaDTO(lst, dt, ID_DonVi, idnhanvien, LoaiUpdate);
                return lstError;
            }
        }
        public List<ErrorDMHangHoa> checkExcel_HangHoa_LoHang(Stream fileInput, Guid ID_DonVi, Guid idnhanvien, int LoaiUpdate)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;

            ExportTableOptions options = new ExportTableOptions();
            options.ExportColumnName = true;
            options.IsVertical = true;
            options.ExportAsString = true;
            DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            List<string> lst = new List<string>();
            for (int i = 19; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Trim() != "…" & dt.Rows[0][i].ToString().Trim() != "")
                    lst.Add(dt.Rows[0][i].ToString().Trim());
            }
            dt.Rows[0].Delete();
            dt.Columns[2].ColumnName = "MaLo";
            dt.Columns[5].ColumnName = "MaHang";
            dt.Columns[13].ColumnName = "TenDVT";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dk = "";
                DataRow dtRow = dt.Rows[i];
                string rowIndex = "Dòng số: " + (i + 4).ToString();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dt.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    var nhomcha = dtRow[0].ToString().Trim();
                    var nhomcon = dtRow[1].ToString().Trim();
                    var malohang = dtRow[2].ToString().Trim();
                    var ngaysx = dtRow[3].ToString().Trim();
                    var hansd = dtRow[4].ToString().Trim();
                    var mahanghoa = dtRow[5].ToString().Trim();
                    var tenhanghoa = dtRow[6].ToString().Trim();
                    var loaihangEx = dtRow[7].ToString().Trim();
                    var loaihang = 1;
                    switch (loaihangEx)
                    {
                        case "Dịch vụ":
                            loaihang = 2;
                            break;
                        case "Combo":
                            loaihang = 3;
                            break;

                    }
                    var duocbantructiep = dtRow[8].ToString().Trim() != "" ? false : true;
                    var ghichu = dtRow[9].ToString().Trim();
                    var giavon = dtRow[10].ToString().Trim();
                    var giaban = dtRow[11].ToString().Trim();
                    var tonkho = dtRow[12].ToString().Trim();
                    var tenDVT = dtRow[13].ToString().Trim();
                    var quycach = dtRow[14].ToString().Trim();
                    var dvQuyCach = dtRow[15].ToString().Trim();
                    var madvtcoban = dtRow[16].ToString().Trim();
                    var ladvchuan = madvtcoban != "" ? false : true;
                    var gtriquydoi = dtRow[17].ToString().Trim();
                    var macungloai = dtRow[18].ToString().Trim();

                    var tenhang_KhongDau = CommonStatic.ConvertToUnSign(tenhanghoa).ToLower();
                    var tenhang_KyTuDau = CommonStatic.GetCharsStart(tenhanghoa).ToLower();
                    var nhomcha_KhongDau = CommonStatic.ConvertToUnSign(nhomcha).ToLower();
                    var nhomcha_KyTuDau = CommonStatic.GetCharsStart(nhomcha).ToLower();
                    var nhomcon_KhongDau = CommonStatic.ConvertToUnSign(nhomcon).ToLower();
                    var nhomcon_KyTuDau = CommonStatic.GetCharsStart(nhomcon).ToLower();

                    bool MaLodacbiet = kiemtrakitu(malohang);
                    if (MaLodacbiet == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Mã lô hàng";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = malohang;
                        DM.DienGiai = "Mã lô hàng không được chứa ký tự đặc biệt";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }
                    else
                    {
                        if (malohang == "")
                        {
                            bool ML_Emtry = GroupDataLoHang(dt, mahanghoa);
                            if (ML_Emtry)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã hàng hóa";
                                DM.ViTri = rowIndex;
                                DM.ThuocTinh = malohang;
                                DM.DienGiai = "Sản phẩm: '" + mahanghoa + "' bị trùng lặp mã";
                                DM.rowError = i;
                                DM.loaiError = 1;
                                lstError.Add(DM);
                            }
                        }
                        else
                        {
                            bool ML_trung = GroupData(dt, "MaLo = '" + malohang + "' and MaHang = '" + mahanghoa + "'");
                            if (ML_trung == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã lô hàng";
                                DM.ViTri = rowIndex;
                                DM.ThuocTinh = malohang;
                                DM.DienGiai = "sản phẩm: " + mahanghoa + " bị trùng lặp mã lô '" + malohang + "'";
                                DM.rowError = i;
                                DM.loaiError = 1;
                                lstError.Add(DM);
                            }
                        }
                    }
                    try
                    {
                        DateTime? ngaysanxuat = ngaysx != "" ? DateTime.Parse(ngaysx) : (DateTime?)null;

                    }
                    catch
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Ngày sản xuất";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = ngaysx;
                        DM.DienGiai = "Ngày sản xuất không đúng định dạng DateTime";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }
                    try
                    {
                        DateTime? ngaysanxuat = hansd != "" ? DateTime.Parse(hansd) : (DateTime?)null;
                    }
                    catch
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Ngày hết hạn";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = hansd;
                        DM.DienGiai = "Ngày hết hạn không đúng định dạng DateTime";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }
                    if (ngaysx != "" && hansd != "")
                    {
                        try
                        {
                            DateTime? ngaysanxuat = ngaysx != "" ? DateTime.Parse(ngaysx) : (DateTime?)null;
                            DateTime? ngayhethan = hansd != "" ? DateTime.Parse(hansd) : (DateTime?)null;
                            if (ngaysanxuat > ngayhethan)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày hết hạn";
                                DM.ViTri = rowIndex;
                                DM.ThuocTinh = hansd;
                                DM.DienGiai = "Ngày hết hạn nhỏ hơn ngày sản xuất";
                                DM.rowError = i;
                                DM.loaiError = 1;
                                lstError.Add(DM);
                            }
                        }
                        catch
                        {

                        }
                    }

                    bool ML_trung1 = GroupData(dt, "MaHang = '" + mahanghoa + (malohang != "" ? "' and MaLo = '" + malohang + "'" : "" + "'"));
                    if (ML_trung1 == false)
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Mã hàng";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = malohang;
                        string Lo = string.Empty;
                        if (malohang != "")
                            Lo = " (" + dtRow[0].ToString().Trim() + ")";
                        DM.DienGiai = "Mã hàng '" + mahanghoa + "' bị trùng lặp";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }

                    string CheckCSDL = ChekMaHangDatabase_Update(mahanghoa);
                    if (CheckCSDL != string.Empty)
                    {
                        if (CheckCSDL.Trim() != tenhanghoa & tenhanghoa != "" & tenhanghoa != "null" & tenhanghoa != null)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng/dịch vụ";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Sản phẩm: " + mahanghoa + " đã tồn tại trong cơ sở dữ liệu";
                            DM.rowError = i;
                            DM.loaiError = 2;
                            DM.nameHH_excel = tenhanghoa;
                            DM.nameHH_sql = CheckCSDL.Trim();
                            lstError.Add(DM);
                        }
                        bool check_QuanLyTheoLo = ChekMaHang_QuanLyTheoLo(mahanghoa);
                        bool check_GiaoDich = ChekMaHangDatabase_GiaoDich(mahanghoa);
                        if (check_QuanLyTheoLo == false && malohang != "" && check_GiaoDich == true)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng/dịch vụ";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Sản phẩm: " + mahanghoa + " không được quản lý theo lô";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            DM.nameHH_excel = tenhanghoa;
                            DM.nameHH_sql = CheckCSDL.Trim();
                            lstError.Add(DM);
                        }
                        if (check_QuanLyTheoLo == true && malohang == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã hàng/dịch vụ";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = mahanghoa;
                            DM.DienGiai = "Sản phẩm: " + mahanghoa + " được quản lý theo lô hàng";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            DM.nameHH_excel = hansd;
                            DM.nameHH_sql = CheckCSDL.Trim();
                            lstError.Add(DM);
                        }
                    }
                    else
                    {
                        if ((string.IsNullOrEmpty(tenhanghoa) || tenhanghoa == "null")
                            && string.IsNullOrEmpty(madvtcoban) && string.IsNullOrEmpty(macungloai))
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tên hàng/dịch vụ";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = tenhanghoa;
                            DM.DienGiai = "Tên hàng hóa không được để trống";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }

                    if (loaihang != 1 && !string.IsNullOrEmpty(malohang))
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Mã lô hàng";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = malohang;
                        DM.DienGiai = "Là dịch vụ - Không nhập mã lô";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }

                    if (!string.IsNullOrEmpty(macungloai))
                    {
                        if (loaihang != 1)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã cha cùng loại";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = malohang;
                            DM.DienGiai = "Là dịch vụ - Không nhập mã hàng cùng loại";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }

                        string existChaCungLoai = ChekMaHangDatabase_Update(macungloai);
                        if (string.IsNullOrEmpty(existChaCungLoai))
                        {
                            // find row by macungloai
                            var rowCungLoai = dt.AsEnumerable().Where(x => x.Field<string>(dt.Columns[5].ColumnName.Trim()) == macungloai);
                            if (rowCungLoai == null || rowCungLoai.Count() == 0)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã cha cùng loại";
                                DM.ViTri = rowIndex;
                                DM.ThuocTinh = string.Concat(macungloai);
                                DM.DienGiai = "Mã cha cùng loại chưa tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                DM.loaiError = 1;
                                lstError.Add(DM);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(loaihangEx))
                    {
                        if (loaihangEx != "Combo" && loaihangEx != "Dịch vụ")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[4].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = loaihangEx;
                            DM.DienGiai = "Loại hàng hóa chưa được định nghĩa";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }

                    if (dtRow[8].ToString() != "x" && dtRow[8].ToString() != "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = dt.Columns[8].ColumnName;
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = dtRow[8].ToString();
                        DM.DienGiai = "Là hàng hóa không được bán trực tiếp bạn cần đánh dấu x";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }
                    if (giavon != "")
                    {
                        bool isNumber7 = IsNumber(giavon);
                        if (isNumber7 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[10].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = giavon;
                            DM.DienGiai = "Giá vốn không phải dạng số";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                    if (quycach != "")
                    {
                        bool isNumber8 = IsNumber(quycach);
                        if (isNumber8 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[11].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = quycach;
                            DM.DienGiai = "Giá bán không phải dạng số";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                    if (tenDVT != "")
                    {
                        bool kytudacbiet4 = kiemtrakitu(tenDVT);
                        if (kytudacbiet4 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tên Đơn vị tính";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = tenDVT;
                            DM.DienGiai = "Tên đơn vị tính không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }

                    if (madvtcoban != "" && tenDVT == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Tên đơn vị tính";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = tenDVT;
                        DM.DienGiai = "Tên đơn vị tính không được để trống khi có mã đơn vị cơ bản";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }

                    if (madvtcoban != "" && gtriquydoi == "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Giá trị quy đổi";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = gtriquydoi;
                        DM.DienGiai = "Giá trị quy đổi không được để trống khi có mã đơn vị cơ bản";
                        DM.rowError = i;
                        DM.loaiError = 1;
                        lstError.Add(DM);
                    }
                    if (quycach != "")
                    {
                        bool isNumber = IsNumber(quycach);
                        if (isNumber == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = dt.Columns[14].ColumnName;
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = quycach;
                            DM.DienGiai = "Quy cách không phải dạng số";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }

                    if (madvtcoban != "")
                    {
                        if (madvtcoban == mahanghoa)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = madvtcoban;
                            DM.DienGiai = "Mã đơn vị tính cơ bản trùng với mã hàng hóa";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                        bool kytudacbiet12 = kiemtrakitu(madvtcoban);
                        if (kytudacbiet12 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = madvtcoban;
                            DM.DienGiai = "Mã đơn vị tính cơ bản không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                        bool trungma12 = GroupDataMaDVTCoBan(dt, " TRIM(MaHang) = '" + madvtcoban + "' and TenDVT <> ''");
                        if (trungma12 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = madvtcoban;
                            DM.DienGiai = "Mã đơn vị tính cơ bản: " + madvtcoban + " không tồn tại hoặc chưa có tên đơn vị tính cơ bản";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                        if (!string.IsNullOrEmpty(malohang))
                        {
                            bool existMaLo = CheckMaLoDVTCoBan(dt, "MaHang = '" + madvtcoban + "'", malohang);
                            if (existMaLo)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã đơn vị tính cơ bản";
                                DM.ViTri = rowIndex;
                                DM.ThuocTinh = madvtcoban;
                                DM.DienGiai = "Mã đơn vị tính cơ bản: " + madvtcoban + " có mã lô hàng không đúng";
                                DM.rowError = i;
                                DM.loaiError = 1;
                                lstError.Add(DM);
                            }
                        }
                    }
                    if (gtriquydoi != "")
                    {
                        bool Isnumber13 = IsNumber(gtriquydoi);
                        if (Isnumber13 == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Giá trị quy đổi";
                            DM.ViTri = rowIndex;
                            DM.ThuocTinh = gtriquydoi;
                            DM.DienGiai = "Giá trị quy đổi không phải là dạng số";
                            DM.rowError = i;
                            DM.loaiError = 1;
                            lstError.Add(DM);
                        }
                    }
                }
            }
            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                lstError = ImportDanhMucHangHoaDTO_LoHang(lst, dt, ID_DonVi, idnhanvien, LoaiUpdate);
                return lstError;
            }
        }

        public List<ErrorDMHangHoa> checkExcel_HangHoaHoaDon(Stream fileInput)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            try
            {
                for (int k = 0; k < 10; k++)
                {
                    Worksheet worksheet = objWorkbook.Worksheets[k];
                    int trows = worksheet.Cells.MaxDataRow - 1;
                    int tcool = worksheet.Cells.MaxColumn;
                    string sheet = worksheet.Name.ToString();
                    DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, 4, true);
                    dt.Columns[0].ColumnName = "MaHang";
                    if (worksheet.Cells.Rows[2][0].Value.ToString() != "Mã hàng hóa" || worksheet.Cells.Rows[2][3].Value.ToString() != "Số lượng")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Định dạng file excel";
                        DM.DienGiai = sheet + ": dữ liệu không đúng định dạng theo file mẫu";
                        lstError.Add(DM);
                    }
                    else
                    {
                        if (dt.Rows.Count < 1)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Nội dung dữ liệu";
                            DM.DienGiai = sheet + ": không có dữ liệu";
                            lstError.Add(DM);
                        }
                        else
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string dk = "";
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    if (dt.Rows[i][j].ToString() != "")
                                    {
                                        break;
                                    }
                                    if (j == dt.Columns.Count - 1)
                                    {
                                        dk = "1";
                                    }
                                }
                                if (dk == "")
                                {
                                    bool trungma = GroupData(dt, "MaHang = '" + dt.Rows[i][0].ToString() + "'");
                                    if (trungma == false)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = sheet;
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dt.Rows[i][0].ToString();
                                        DM.DienGiai = "Mã hàng: " + dt.Rows[i][0].ToString() + " bị trùng lặp";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                    bool CheckCSDL = ChekMaHangDatabase(dt.Rows[i][0].ToString());
                                    if (CheckCSDL == true)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = sheet;
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dt.Rows[i][0].ToString();
                                        DM.DienGiai = "Mã hàng: " + dt.Rows[i][0].ToString() + " Không có trên hệ thống";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }

                                    if (dt.Rows[i][3].ToString() == "" | dt.Rows[i][3].ToString() == "null" | dt.Rows[i][3].ToString() == null)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = sheet;
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dt.Rows[i][3].ToString();
                                        DM.DienGiai = "Thiếu giá trị số lượng";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                return null;
            }
        }

        public List<ErrorDMHangHoa> CheckImportExcel_HangHoaHoaDon(Stream fileInput)
        {
            var _classDMHH = new ClassDM_HangHoa(db);
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            int countSheets = objWorkbook.Worksheets.Count();
            try
            {
                for (int k = 0; (k < countSheets) && (k < 10); k++)
                {
                    Worksheet worksheet = objWorkbook.Worksheets[k];
                    int trows = worksheet.Cells.MaxDataRow - 1;
                    int tcool = worksheet.Cells.MaxColumn;
                    string sheet = worksheet.Name.ToString();
                    DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, 5, true);
                    dt.Columns[0].ColumnName = "MaLo";
                    dt.Columns[1].ColumnName = "MaHang";

                    if (worksheet.Cells.Rows[2][1].Value.ToString() != "Mã hàng hóa" || worksheet.Cells.Rows[2][4].Value.ToString() != "Số lượng")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Định dạng file excel";
                        DM.DienGiai = sheet + ": dữ liệu không đúng định dạng theo file mẫu";
                        lstError.Add(DM);
                    }
                    else
                    {
                        if (dt.Rows.Count < 1)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Nội dung dữ liệu";
                            DM.DienGiai = sheet + ": không có dữ liệu";
                            lstError.Add(DM);
                        }
                        else
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string dk = "";
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    if (dt.Rows[i][j].ToString() != "")
                                    {
                                        break;
                                    }
                                    if (j == dt.Columns.Count - 1)
                                    {
                                        dk = "1";
                                    }
                                }
                                if (dk == "")
                                {
                                    var maLoHang = dt.Rows[i][0].ToString().Trim();
                                    var maHangHoa = dt.Rows[i][1].ToString().Trim();

                                    if (maHangHoa == "")
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = sheet;
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = maLoHang;
                                        DM.DienGiai = "Mã hàng Không được để trống";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                    else
                                    {
                                        bool CheckCSDL = _classDMHH.SP_CheckHangDangKinhDoanh(maHangHoa);
                                        if (CheckCSDL == false)
                                        {
                                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                            DM.TenTruongDuLieu = sheet;
                                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                            DM.ThuocTinh = maHangHoa;
                                            DM.DienGiai = "Mã hàng '" + maHangHoa + "' không có trên hệ thống hoặc ngừng kinh doanh";
                                            DM.rowError = i;
                                            lstError.Add(DM);
                                        }

                                        // check quan ly theo lo
                                        var qlTheoLo = _classDMHH.SP_CheckHangHoa_QuanLyTheoLo(maHangHoa);
                                        if (qlTheoLo)
                                        {
                                            // check ma lo empty
                                            if (maLoHang == "")
                                            {
                                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                                DM.TenTruongDuLieu = sheet;
                                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                                DM.ThuocTinh = maLoHang;
                                                DM.DienGiai = "Mã hàng '" + maHangHoa + "' quản ý theo lô, nhưng chưa nhập số lô";
                                                DM.rowError = i;
                                                lstError.Add(DM);
                                            }
                                            else
                                            {
                                                // check malo not exist DB
                                                bool checklo = _classDMHH.SP_CheckLoHangExist(maHangHoa, maLoHang);
                                                if (checklo == false)
                                                {
                                                    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                                    DM.TenTruongDuLieu = sheet;
                                                    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                                    DM.ThuocTinh = maLoHang;
                                                    DM.DienGiai = "Lô hàng '" + maLoHang + "' không có trên hệ thống";
                                                    DM.rowError = i;
                                                    lstError.Add(DM);
                                                }
                                                else
                                                {
                                                    // check trung mahang + malo --> cho phep nhap trung MaHang khi Ban
                                                    //bool trungma = GroupData(dt, "MaHang = '" + maHangHoa + "' and MaLo = '" + maLoHang + "'");
                                                    //if (trungma == false)
                                                    //{
                                                    //    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                                    //    string Lo = " (" + maLoHang + ")";
                                                    //    DM.TenTruongDuLieu = sheet;
                                                    //    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                                    //    DM.ThuocTinh = maLoHang;
                                                    //    DM.DienGiai = "Mã hàng '" + maHangHoa + Lo + "' bị trùng lặp";
                                                    //    DM.rowError = i;
                                                    //    lstError.Add(DM);
                                                    //}
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // khong quan ly theo lo, nhung nhap ma lo
                                            if (maLoHang != "")
                                            {
                                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                                DM.TenTruongDuLieu = sheet;
                                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                                DM.ThuocTinh = maLoHang;
                                                DM.DienGiai = "Mã hàng '" + maHangHoa + "' không quản ý theo lô. Không thể nhập mã lô";
                                                DM.rowError = i;
                                                lstError.Add(DM);
                                            }
                                        }
                                    }

                                    if (dt.Rows[i][4].ToString() == "")
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = sheet;
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dt.Rows[i][4].ToString();
                                        DM.DienGiai = "Thiếu giá trị số lượng";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                    else
                                    {
                                        bool isNumber7 = IsNumber(dt.Rows[i][4].ToString());
                                        if (isNumber7 == false)
                                        {
                                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                            DM.TenTruongDuLieu = sheet;
                                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                            DM.ThuocTinh = dt.Rows[i][4].ToString();
                                            DM.DienGiai = "Số lượng không phải dạng số";
                                            DM.rowError = i;
                                            lstError.Add(DM);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("CheckImportExcel_HangHoaHoaDon: " + e.InnerException + e.Message);
            }
            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                return null;
            }
        }

        public List<DM_HangHoaHoaDon> getList_HangHoaHoaDon(Stream fileInput)
        {
            List<DM_HangHoaHoaDon> lst = new List<DM_HangHoaHoaDon>();
            Workbook objWorkbook = new Workbook(fileInput);
            var countSheets = objWorkbook.Worksheets.Count();
            try
            {
                for (int k = 0; (k < countSheets) && (k < 10); k++)
                {
                    Worksheet worksheet = objWorkbook.Worksheets[k];
                    int trows = worksheet.Cells.MaxDataRow - 1;
                    int tcool = worksheet.Cells.MaxColumn;
                    string sheet = worksheet.Name.ToString();
                    DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, 5, true);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string dk = "";
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (dt.Rows[i][j].ToString() != "")
                            {
                                break;
                            }
                            if (j == dt.Columns.Count - 1)
                            {
                                dk = "1";
                            }
                        }
                        if (dk == "")
                        {
                            var dtRow = dt.Rows[i];
                            DM_HangHoaHoaDon DM1 = new DM_HangHoaHoaDon();
                            DM1.MaHangHoa = dtRow[1].ToString().Trim();
                            DM1.MaLoHang = dtRow[0].ToString().Trim();

                            // get idquidoi, idlo by mahang, malo (used to import hoadon at gara
                            var tblQuiDoi = (from qd in db.DonViQuiDois
                                             join lo in db.DM_LoHang on qd.ID_HangHoa equals lo.ID_HangHoa into qdlo
                                             from qd_lo in qdlo.DefaultIfEmpty()
                                             where qd.MaHangHoa.ToUpper().Trim() == DM1.MaHangHoa
                                             select new
                                             {
                                                 ID_DonViQuiDoi = qd.ID,
                                                 ID_LoHang = qd_lo != null ?
                                                 qdlo.Where(x => x.MaLoHang == DM1.MaLoHang).FirstOrDefault().ID : Guid.Empty
                                             }).FirstOrDefault();
                            if (tblQuiDoi != null)
                            {
                                DM1.ID_DonViQuiDoi = tblQuiDoi.ID_DonViQuiDoi;
                                DM1.ID_LoHang = tblQuiDoi.ID_LoHang;
                            }
                            try
                            {
                                DM1.GiaBan = double.Parse(dtRow[2].ToString());
                            }
                            catch
                            {
                                DM1.GiaBan = null;
                            }
                            try
                            {
                                DM1.GiamGia = double.Parse(dtRow[3].ToString());
                            }
                            catch
                            {
                                DM1.GiamGia = null;
                            }
                            DM1.SoLuong = double.Parse(dtRow[4].ToString());
                            DM1.ThuTuHoaDon = k + 1;
                            lst.Add(DM1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("getList_HangHoaHoaDon: " + e.InnerException + e.Message);
            }
            return lst;
        }
        private readonly string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public string LocDau(string str)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }
        // kiểm tra ký tự đặc biệt
        public bool kiemtrakitu(string chuoiCanKiemTra)
        {
            //chuoiCanKiemTra = CommonStatic.ConvertMa(chuoiCanKiemTra);
            chuoiCanKiemTra = CommonStatic.ConvertToUnSign(chuoiCanKiemTra);
            //chuoiCanKiemTra = LocDau(chuoiCanKiemTra);
            string chuoidung = "1234567890_-()[]*QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopas dfghjklzxcvbnm,.";
            bool dung = false;
            if (chuoiCanKiemTra != "")
            {
                foreach (char kiTu in chuoiCanKiemTra)
                {
                    dung = false;
                    foreach (char kitu2 in chuoidung)
                    {
                        if (kiTu == kitu2)
                        {
                            dung = true;
                            break;
                        }
                    }
                    if (dung == false)
                    {
                        break;
                    }
                }
            }
            else
            {
                dung = true;
            }
            return dung;
        }
        private bool kiemtrakituDiaChi(string chuoiCanKiemTra)
        {
            //chuoiCanKiemTra = CommonStatic.ConvertMa(chuoiCanKiemTra);
            chuoiCanKiemTra = LocDau(chuoiCanKiemTra);
            string chuoidung = "1234567890_-/QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopas dfghjklzxcvbnm,.";
            bool dung = false;
            if (chuoiCanKiemTra != "")
            {
                foreach (char kiTu in chuoiCanKiemTra)
                {
                    dung = false;
                    foreach (char kitu2 in chuoidung)
                    {
                        if (kiTu == kitu2)
                        {
                            dung = true;
                            break;
                        }
                    }
                    if (dung == false)
                    {
                        break;
                    }
                }
            }
            else
            {
                dung = true;
            }
            return dung;
        }
        // kiểm tra trùng dữ liệu
        /// <summary>
        /// nếu trùng: trả về false
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public bool GroupData(DataTable pData, string filterExpression)
        {
            // for (int i )
            bool dung = true;
            try
            {
                DataRow[] rows = pData.Select(filterExpression);
                if (rows.Length > 1)
                {
                    dung = false;
                }
            }
            catch (Exception)
            {
                dung = false;
            }
            return dung;
        }
        // getMaLo
        private string getMaLoHang(DataTable pData, string filterExpression)
        {
            string malo = "";
            DataRow[] rows = pData.Select(filterExpression);
            if (rows.Length > 1)
            {
                malo = rows[0][2].ToString();
            }
            return malo;
        }
        private bool GroupData_Lo(DataTable pData, string filterExpression)
        {
            // for (int i )
            bool dung = true;
            DataRow[] rows = pData.Select(filterExpression);
            if (rows.Length > 1)
            {
                dung = false;
            }
            return dung;
        }
        // kiểm tra lỗi chưa tồn tại mã đơn vị cơ bản
        private bool GroupDataMaDVTCoBan(DataTable pData, string filterExpression)
        {
            bool dung = false;
            DataRow[] rows = pData.Select(filterExpression);
            if (rows.Length > 0)
            {
                dung = true;
            }
            return dung;
        }
        // kiểm tra tồn tại lô hàng mã đơn vị cơ bản
        private bool CheckMaLoDVTCoBan(DataTable pData, string filterExpression, string MaLoDVTCoBan)
        {
            bool dung = true;
            DataRow[] rows = pData.Select(filterExpression);
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i][2].ToString() == MaLoDVTCoBan)
                {
                    dung = false;
                    break;
                }
            }
            return dung;
        }
        // kiểm tra lỗi quản lý theo lô hàng
        private bool GroupDataLoHang(DataTable pData, string maHangHoa)
        {
            string filterExpression = "MaHang = '" + maHangHoa + "' and MaLo <> ''";
            bool dung = false;
            try
            {
                DataRow[] rows = pData.Select(filterExpression);
                if (rows.Length > 0)
                {
                    dung = true;
                }
            }
            catch (Exception)
            {
                dung = false;
            }
            return dung;
        }
        // Kiểm tra email có hợp lệ không
        public bool ValidateEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
        // kiểm tra chuỗi dữ liệu có phải là số không
        public bool IsNumberInt(string pText)
        {
            try
            {
                Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
                return regex.IsMatch(pText);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsNumber(string pText)
        {
            //Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            //return regex.IsMatch(pText);
            try
            {
                if (pText.Trim() != string.Empty)
                {
                    double price = Convert.ToDouble(pText);
                    return true;
                }
                else
                    return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        // kiểm tra dữ liệu dạng datetime
        public bool ValidateDateTime(string dateTime)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(dateTime);
                return true;
            }
            catch
            {
                try
                {
                    DateTime dt = Convert.ToDateTime("01/01/" + dateTime.Trim());
                    return true;
                }
                catch
                {
                    return false;
                }

            }
        }
        // kiểm tra tồn tại mã hàng trong CSDL
        public DonViQuiDoi Select_DonViQuiDoi(string maHangHoa)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DonViQuiDois.Where(p => p.MaHangHoa == maHangHoa).FirstOrDefault();
            }
        }
        // kiểm tra tồn tại giá vốn trong CSDL
        public DM_GiaVon SelectGiaVon_DonViQuiDoi(Guid ID_DonViQuiDoi, Guid ID_DonVi, Guid? ID_LoHang)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == ID_DonViQuiDoi && p.ID_DonVi == ID_DonVi && p.ID_LoHang == ID_LoHang).FirstOrDefault();
            }
        }
        // kiểm tra tồn tại mã hàng trong CSDL
        public string SelectHangHoa_Update(string maHangHoa)
        {
            try
            {
                if (db == null)
                {
                    return String.Empty;
                }
                else
                {
                    var tbl = from hh in db.DM_HangHoa
                              join dvqd in db.DonViQuiDois on hh.ID equals dvqd.ID_HangHoa
                              where dvqd.MaHangHoa == maHangHoa
                              select new
                              {
                                  hh.TenHangHoa
                              };
                    if (tbl != null && tbl.Count() > 0)
                    {
                        return tbl.FirstOrDefault().TenHangHoa;
                    }
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        public bool SelectHangHoa_QuanLyTheoLo(string maHangHoa)
        {
            try
            {
                if (db == null)
                {
                    return false;
                }
                else
                {
                    var tbl = from hh in db.DM_HangHoa
                              join dvqd in db.DonViQuiDois on hh.ID equals dvqd.ID_HangHoa
                              where dvqd.MaHangHoa == maHangHoa
                              select new
                              {
                                  hh.QuanLyTheoLoHang
                              };
                    return tbl.FirstOrDefault().QuanLyTheoLoHang.Value;
                }
            }
            catch
            {
                return false;
            }
        }

        //check mã hàng 
        public bool ChekMaHangDatabase(string maHangHoa)
        {
            bool dung = true;
            if (maHangHoa != "")
            {
                DonViQuiDoi objDVT_QuyDoi_New = Select_DonViQuiDoi(maHangHoa.Trim());
                if (objDVT_QuyDoi_New != null)
                {
                    dung = false;
                }
            }
            return dung;
        }

        //check mã hàng 
        public bool CheckQuanLyTheoLo_HangHoa(string maHangHoa)
        {
            var _classDMHH = new ClassDM_HangHoa(db);
            bool qlTheoLo = false;
            if (maHangHoa != "")
            {
                DonViQuiDoi dvqd = Select_DonViQuiDoi(maHangHoa.Trim());
                if (dvqd != null)
                {
                    DM_HangHoa hh = _classDMHH.Get(x => x.ID == dvqd.ID_HangHoa);
                    if (hh != null)
                    {
                        qlTheoLo = hh.QuanLyTheoLoHang ?? false;
                    }
                }
            }
            return qlTheoLo;
        }
        //check giao dịch
        public bool ChekMaHangDatabase_GiaoDich(String MaHangHoa)
        {
            bool dung = false;
            var tbl = from hdct in db.BH_HoaDon_ChiTiet
                      join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                      where dvqd.MaHangHoa == MaHangHoa
                      select new
                      {
                          dvqd.ID
                      };
            if (tbl.Count() > 0)
            {
                dung = true;
            }
            return dung;
        }
        public string ChekMaHangDatabase_Update(string maHangHoa)
        {
            string dung = string.Empty;
            if (maHangHoa != "")
            {
                string objDVT_QuyDoi_New = SelectHangHoa_Update(maHangHoa.Trim());
                if (objDVT_QuyDoi_New != string.Empty)
                {
                    dung = objDVT_QuyDoi_New;
                }
            }
            return dung;
        }
        public bool ChekMaHang_QuanLyTheoLo(string maHangHoa)
        {
            bool dung = false;
            if (maHangHoa != "")
            {
                dung = SelectHangHoa_QuanLyTheoLo(maHangHoa.Trim());
            }
            return dung;
        }
        //check mã hàng đang kinh doanh
        public bool ChekMaHangDatabase_DangKinhDoanh(string maHangHoa)
        {
            bool dung = false;
            if (maHangHoa != "")
            {
                maHangHoa = maHangHoa.Trim();
                DonViQuiDoi objDVT_QuyDoi_New = db.DonViQuiDois.Where(x => x.MaHangHoa.Trim() == maHangHoa & x.Xoa != true).FirstOrDefault();
                if (objDVT_QuyDoi_New != null)
                {
                    DM_HangHoa HH = db.DM_HangHoa.Where(x => x.ID == objDVT_QuyDoi_New.ID_HangHoa & x.TheoDoi == true).FirstOrDefault();
                    if (HH != null)
                        dung = true;
                }
            }
            return dung;
        }
        public bool ChekMaHangDatabase_LaDichVu(string maHangHoa)
        {
            bool dung = false;
            if (maHangHoa != "")
            {
                maHangHoa = maHangHoa.Trim();
                DonViQuiDoi objDVT_QuyDoi_New = db.DonViQuiDois.Where(x => x.MaHangHoa.Trim() == maHangHoa & x.Xoa != true).FirstOrDefault();
                if (objDVT_QuyDoi_New != null)
                {
                    DM_HangHoa HH = db.DM_HangHoa.Where(x => x.ID == objDVT_QuyDoi_New.ID_HangHoa & x.LaHangHoa == true).FirstOrDefault();
                    if (HH != null)
                        dung = true;
                }
            }
            return dung;
        }
        //check mã hàng đang kinh doanh
        public bool ChekLoHangDatabase(string maLoHang, string maHangHoa)
        {
            bool dung = false;
            if (maLoHang != "")
            {
                Guid objDVT_QuyDoi_New = db.DonViQuiDois.Where(x => x.MaHangHoa == maHangHoa & x.Xoa != true).Select(p => p.ID_HangHoa).FirstOrDefault();
                Guid dM_LoHang = db.DM_LoHang.Where(x => x.MaLoHang == maLoHang & x.ID_HangHoa == objDVT_QuyDoi_New).Select(p => p.ID).FirstOrDefault();
                if (dM_LoHang != null && dM_LoHang != Guid.Empty)
                {
                    dung = true;
                }
            }
            return dung;
        }
        //check mã hàng hóa đã có trong bảng giá
        public bool ChekMaHangCoTrongBangGia(string maHangHoa, Guid ID_BangGia)
        {
            bool dung = false;
            if (maHangHoa != "")
            {
                var _classDVQD = new classDonViQuiDoi(db);
                DonViQuiDoi dvqd = _classDVQD.Get(p => p.MaHangHoa == maHangHoa);
                if (dvqd != null)
                {
                    List<DM_GiaBan_ChiTiet> listiddvqd = db.DM_GiaBan_ChiTiet.Where(p => p.ID_GiaBan == ID_BangGia).ToList();
                    bool alreadyExists = listiddvqd.Any(x => x.ID_DonViQuiDoi == dvqd.ID);
                    if (alreadyExists == true)
                    {
                        dung = true;
                    }
                    else
                    {
                        dung = false;
                    }
                }
                else
                {
                    dung = false;
                }
            }
            return dung;
        }
        // check mã khách hàng trong database
        public bool CheckMaKHDatabase(string maKhachHang)
        {
            bool dung = true;
            if (maKhachHang != "")
            {
                DM_DoiTuong dM_DoiTuong = db.DM_DoiTuong.Where(x => x.MaDoiTuong == maKhachHang && x.TheoDoi == false).FirstOrDefault();
                if (dM_DoiTuong != null)
                {
                    dung = false;
                }
            }
            return dung;
        }
        // kiểm tra tồn tại số điện thoại trong CSDL
        public bool checkSDTDatabase(string sodienthoai)
        {
            DM_DoiTuong dM_DoiTuong = db.DM_DoiTuong.Where(x => x.DienThoai == sodienthoai && x.TheoDoi == false).FirstOrDefault();
            if (dM_DoiTuong != null)
                return false;
            else
                return true;
        }
        //check nhom hang hoa
        public DM_NhomHangHoa select_NhomHangHoa(string tenNhomHang)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_NhomHangHoa.Where(p => p.TenNhomHangHoa == tenNhomHang.Trim()).FirstOrDefault();
            }
        }
        public Kho_DonVi select_KhoDonVi(Guid ID_DonVi)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Kho_DonVi.Where(p => p.ID_DonVi == ID_DonVi).FirstOrDefault();
            }
        }
        public string Add_HangHoa(DM_HangHoa objHHAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.DM_HangHoa.Add(objHHAdd);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in dbEx.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), dbEx);
                }
            }
            return strErr;
        }
        //Tạo dữ liệu gốc
        public void Create_DuLieuGoc(string fileInput, Guid ID_DonVi, Guid ID_NhanVien)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet_NhomHangHoa = objWorkbook.Worksheets[0];
            Worksheet worksheet_HangHoa = objWorkbook.Worksheets[1];
            Worksheet worksheet_DonViQuiDoi = objWorkbook.Worksheets[2];
            Worksheet worksheet_DoiTuong = objWorkbook.Worksheets[3];
            Worksheet worksheet_HoaDon = objWorkbook.Worksheets[4];
            Worksheet worksheet_HoaDon_ChiTiet = objWorkbook.Worksheets[5];
            Worksheet worksheet_Quy_HoaDon = objWorkbook.Worksheets[6];
            Worksheet worksheet_Quy_HoaDon_ChiTiet = objWorkbook.Worksheets[7];
            //int trows = worksheet_NhomHangHoa.Cells.MaxDataRow - 1;
            //int tcool = worksheet_NhomHangHoa.Cells.MaxColumn;
            DateTime dateTimeNow = DateTime.Now;
            // insert Nhóm hàng hóa
            DataTable dt_NhomHangHoa = worksheet_NhomHangHoa.Cells.ExportDataTable(0, 0, 6, 3, true);
            for (int i = 0; i < dt_NhomHangHoa.Rows.Count; i++)
            {
                DateTime NewTime_NH = dateTimeNow.AddDays(-50);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_NhomHangHoa.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("MaNhomHangHoa", dt_NhomHangHoa.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("TenNhomHangHoa", dt_NhomHangHoa.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("TenNhomHangHoa_KhongDau", CommonStatic.ConvertToUnSign(dt_NhomHangHoa.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("TenNhomHangHoa_KyTuDau", CommonStatic.GetCharsStart(dt_NhomHangHoa.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_NH));
                db.Database.ExecuteSqlCommand("exec insert_NhomHangHoa @ID, @MaNhomHangHoa, @TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, @timeCreate", paramlist.ToArray());
            }
            // insert hàng hóa
            DataTable dt_HangHoa = worksheet_HangHoa.Cells.ExportDataTable(0, 0, 26, 5, true);
            for (int i = 0; i < dt_HangHoa.Rows.Count; i++)
            {
                DateTime NewTime_HH = dateTimeNow.AddDays(-50);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_HangHoa.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_NhomHang", dt_HangHoa.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("TenHangHoa", dt_HangHoa.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("TenHangHoa_KhongDau", CommonStatic.ConvertToUnSign(dt_HangHoa.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("TenHangHoa_KyTuDau", CommonStatic.GetCharsStart(dt_HangHoa.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("LaHangHoa", dt_HangHoa.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_HH));
                paramlist.Add(new SqlParameter("QuanLyTheoLoHang", false));
                db.Database.ExecuteSqlCommand("exec insert_HangHoa @ID, @ID_NhomHang, @TenHangHoa,@TenHangHoa_KhongDau, @TenHangHoa_KyTuDau,@LaHangHoa, @timeCreate, @QuanLyTheoLoHang", paramlist.ToArray());

                List<SqlParameter> paramlist1 = new List<SqlParameter>();
                paramlist1.Add(new SqlParameter("ID_HangHoa", dt_HangHoa.Rows[i][0].ToString()));
                paramlist1.Add(new SqlParameter("URLAnh", dt_HangHoa.Rows[i][4].ToString()));
                db.Database.ExecuteSqlCommand("exec insert_HangHoa_Anh @ID_HangHoa, @URLAnh", paramlist1.ToArray());
            }
            // insert Đơn vị qui đổi
            DataTable dt_DonViQuiDoi = worksheet_DonViQuiDoi.Cells.ExportDataTable(0, 0, 26, 5, true);
            for (int i = 0; i < dt_DonViQuiDoi.Rows.Count; i++)
            {
                DateTime NewTime_DVQD = dateTimeNow.AddDays(-50);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_DonViQuiDoi.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_HangHoa", dt_DonViQuiDoi.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("ID_LoHang", DBNull.Value));
                paramlist.Add(new SqlParameter("MaHangHoa", dt_DonViQuiDoi.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("GiaVon", dt_DonViQuiDoi.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("GiaBan", dt_DonViQuiDoi.Rows[i][4].ToString()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_DVQD));
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                db.Database.ExecuteSqlCommand("exec insert_DonViQuiDoi @ID, @ID_HangHoa,@ID_DonVi,@ID_LoHang, @MaHangHoa, @GiaVon, @GiaBan, @timeCreate, @ID_NhanVien", paramlist.ToArray());
            }
            // insert Đối tượng
            DataTable dt_DoiTuong = worksheet_DoiTuong.Cells.ExportDataTable(0, 0, 11, 5, true);
            for (int i = 0; i < dt_DoiTuong.Rows.Count; i++)
            {
                DateTime NewTime_DT = dateTimeNow.AddDays(-50 - i);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_DoiTuong.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("MaDoiTuong", dt_DoiTuong.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("TenDoiTuong", dt_DoiTuong.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("TenDoiTuong_KhongDau", CommonStatic.ConvertToUnSign(dt_DoiTuong.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("TenDoiTuong_ChuCaiDau", CommonStatic.GetCharsStart(dt_DoiTuong.Rows[i][2].ToString()).ToLower()));
                //paramlist.Add(new SqlParameter("GioiTinhNam", dt_DoiTuong.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("LoaiDoiTuong", dt_DoiTuong.Rows[i][4].ToString()));
                if (dt_DoiTuong.Rows[i][4].ToString() == "1")
                {
                    paramlist.Add(new SqlParameter("LaCaNhan", "1"));
                    paramlist.Add(new SqlParameter("GioiTinhNam", dt_DoiTuong.Rows[i][3].ToString()));
                }
                else
                {
                    paramlist.Add(new SqlParameter("LaCaNhan", "0"));
                    paramlist.Add(new SqlParameter("GioiTinhNam", "1"));
                }
                paramlist.Add(new SqlParameter("timeCreate", NewTime_DT));
                db.Database.ExecuteSqlCommand("exec insert_DoiTuong @ID,@ID_DonVi, @MaDoiTuong, @TenDoiTuong, @TenDoiTuong_KhongDau, @TenDoiTuong_ChuCaiDau,@GioiTinhNam,@LoaiDoiTuong, @LaCaNhan, @timeCreate", paramlist.ToArray());
            }
            // insert HoaDon
            DataTable dt_HoaDon = worksheet_HoaDon.Cells.ExportDataTable(0, 0, 41, 5, true);
            for (int i = 0; i < dt_HoaDon.Rows.Count; i++)
            {
                DateTime NewTime_HD = dateTimeNow.AddDays(-39 + i);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_HoaDon.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_DoiTuong", dt_HoaDon.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                paramlist.Add(new SqlParameter("MaHoaDon", dt_HoaDon.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("LoaiHoaDon", dt_HoaDon.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("TongTienHang", dt_HoaDon.Rows[i][4].ToString()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_HD));
                db.Database.ExecuteSqlCommand("exec insert_HoaDon @ID, @ID_DoiTuong, @ID_DonVi, @ID_NhanVien, @MaHoaDon, @LoaiHoaDon, @TongTienHang, @timeCreate", paramlist.ToArray());
            }
            // insert HoaDon_ChiTiet
            DataTable dt_HoaDon_ChiTiet = worksheet_HoaDon_ChiTiet.Cells.ExportDataTable(0, 0, 48, 6, true);
            for (int i = 0; i < dt_HoaDon_ChiTiet.Rows.Count; i++)
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", dt_HoaDon_ChiTiet.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_DonViQuiDoi", dt_HoaDon_ChiTiet.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("SoLuong", dt_HoaDon_ChiTiet.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("DonGia", dt_HoaDon_ChiTiet.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("ThanhTien", dt_HoaDon_ChiTiet.Rows[i][4].ToString()));
                paramlist.Add(new SqlParameter("GiaVon", dt_HoaDon_ChiTiet.Rows[i][5].ToString()));
                db.Database.ExecuteSqlCommand("exec insert_HoaDon_ChiTiet @ID_HoaDon, @ID_DonViQuiDoi, @SoLuong, @DonGia, @ThanhTien, @GiaVon", paramlist.ToArray());
            }
            // insert Quy_HoaDon
            DataTable dt_Quy_HoaDon = worksheet_Quy_HoaDon.Cells.ExportDataTable(0, 0, 41, 4, true);
            for (int i = 0; i < dt_Quy_HoaDon.Rows.Count; i++)
            {
                DateTime NewTime_QHD = dateTimeNow.AddDays(-39 + i);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_Quy_HoaDon.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                paramlist.Add(new SqlParameter("MaHoaDon", dt_Quy_HoaDon.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("LoaiHoaDon", dt_Quy_HoaDon.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("TongTienThu", dt_Quy_HoaDon.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_QHD));
                db.Database.ExecuteSqlCommand("exec insert_Quy_HoaDon @ID, @ID_DonVi, @ID_NhanVien, @MaHoaDon, @LoaiHoaDon, @TongTienThu, @timeCreate", paramlist.ToArray());
            }
            // insert Quy_HoaDon_ChiTiet
            DataTable dt_Quy_HoaDon_ChiTiet = worksheet_Quy_HoaDon_ChiTiet.Cells.ExportDataTable(0, 0, 41, 4, true);
            for (int i = 0; i < dt_Quy_HoaDon_ChiTiet.Rows.Count; i++)
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", dt_Quy_HoaDon_ChiTiet.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_HoaDonLienQuan", dt_Quy_HoaDon_ChiTiet.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("ID_DoiTuong", dt_Quy_HoaDon_ChiTiet.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                paramlist.Add(new SqlParameter("TienThu", dt_Quy_HoaDon_ChiTiet.Rows[i][3].ToString()));
                db.Database.ExecuteSqlCommand("exec insert_Quy_HoaDon_ChiTiet @ID_HoaDon, @ID_HoaDonLienQuan, @ID_DoiTuong, @ID_NhanVien, @TienThu", paramlist.ToArray());
            }
            // insert TonKhoKhoiTao
            db.Database.ExecuteSqlCommand("exec insert_TonKhoKhoiTao");
        }
        //Tạo dữ liệu gốc
        public void Create_DuLieuGoc_LoHang(string fileInput, Guid ID_DonVi, Guid ID_NhanVien)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet_NhomHangHoa = objWorkbook.Worksheets[0];
            Worksheet worksheet_HangHoa = objWorkbook.Worksheets[1];
            Worksheet worksheet_LoHang = objWorkbook.Worksheets[2];
            Worksheet worksheet_DonViQuiDoi = objWorkbook.Worksheets[3];
            Worksheet worksheet_DoiTuong = objWorkbook.Worksheets[4];
            Worksheet worksheet_HoaDon = objWorkbook.Worksheets[5];
            Worksheet worksheet_HoaDon_ChiTiet = objWorkbook.Worksheets[6];
            Worksheet worksheet_Quy_HoaDon = objWorkbook.Worksheets[7];
            Worksheet worksheet_Quy_HoaDon_ChiTiet = objWorkbook.Worksheets[8];

            DateTime dateTimeNow = DateTime.Now;
            // insert Nhóm hàng hóa
            DataTable dt_NhomHangHoa = worksheet_NhomHangHoa.Cells.ExportDataTable(0, 0, 6, 3, true);
            for (int i = 0; i < dt_NhomHangHoa.Rows.Count; i++)
            {
                DateTime NewTime_NH = dateTimeNow.AddDays(-50);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_NhomHangHoa.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("MaNhomHangHoa", dt_NhomHangHoa.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("TenNhomHangHoa", dt_NhomHangHoa.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("TenNhomHangHoa_KhongDau", CommonStatic.ConvertToUnSign(dt_NhomHangHoa.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("TenNhomHangHoa_KyTuDau", CommonStatic.GetCharsStart(dt_NhomHangHoa.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_NH));
                db.Database.ExecuteSqlCommand("exec insert_NhomHangHoa @ID, @MaNhomHangHoa, @TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, @timeCreate", paramlist.ToArray());
            }
            // insert hàng hóa
            DataTable dt_HangHoa = worksheet_HangHoa.Cells.ExportDataTable(0, 0, 26, 5, true);
            for (int i = 0; i < dt_HangHoa.Rows.Count; i++)
            {
                DateTime NewTime_HH = dateTimeNow.AddDays(-50);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_HangHoa.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_NhomHang", dt_HangHoa.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("TenHangHoa", dt_HangHoa.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("TenHangHoa_KhongDau", CommonStatic.ConvertToUnSign(dt_HangHoa.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("TenHangHoa_KyTuDau", CommonStatic.GetCharsStart(dt_HangHoa.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("LaHangHoa", dt_HangHoa.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_HH));
                paramlist.Add(new SqlParameter("QuanLyTheoLoHang", true));
                db.Database.ExecuteSqlCommand("exec insert_HangHoa @ID, @ID_NhomHang, @TenHangHoa,@TenHangHoa_KhongDau, @TenHangHoa_KyTuDau,@LaHangHoa, @timeCreate, @QuanLyTheoLoHang", paramlist.ToArray());
                List<SqlParameter> paramlist1 = new List<SqlParameter>();
                paramlist1.Add(new SqlParameter("ID_HangHoa", dt_HangHoa.Rows[i][0].ToString()));
                paramlist1.Add(new SqlParameter("URLAnh", dt_HangHoa.Rows[i][4].ToString()));
                db.Database.ExecuteSqlCommand("exec insert_HangHoa_Anh @ID_HangHoa, @URLAnh", paramlist1.ToArray());
            }
            // insert lô hàng
            DataTable dt_LoHang = worksheet_LoHang.Cells.ExportDataTable(0, 0, 26, 3, true);
            for (int i = 0; i < dt_LoHang.Rows.Count; i++)
            {
                DateTime NgaySanXuat = dateTimeNow.AddDays(-100 + i);
                DateTime NgayHetHan = NgaySanXuat.AddYears(3);
                List<SqlParameter> prmLH = new List<SqlParameter>();
                prmLH.Add(new SqlParameter("ID", dt_LoHang.Rows[i][0].ToString()));
                prmLH.Add(new SqlParameter("ID_HangHoa", dt_LoHang.Rows[i][1].ToString()));
                prmLH.Add(new SqlParameter("MaLoHang", dt_LoHang.Rows[i][2].ToString()));
                prmLH.Add(new SqlParameter("NgaySanXuat", NgaySanXuat));
                prmLH.Add(new SqlParameter("NgayHetHan", NgayHetHan));
                db.Database.ExecuteSqlCommand("exec insert_LoHang @ID, @ID_HangHoa, @MaLoHang,@NgaySanXuat,@NgayHetHan", prmLH.ToArray());
            }
            // insert Đơn vị qui đổi
            DataTable dt_DonViQuiDoi = worksheet_DonViQuiDoi.Cells.ExportDataTable(0, 0, 26, 5, true);
            for (int i = 0; i < dt_DonViQuiDoi.Rows.Count; i++)
            {
                DateTime NewTime_DVQD = dateTimeNow.AddDays(-50);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_DonViQuiDoi.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_HangHoa", dt_DonViQuiDoi.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("ID_LoHang", dt_LoHang.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("MaHangHoa", dt_DonViQuiDoi.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("GiaVon", dt_DonViQuiDoi.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("GiaBan", dt_DonViQuiDoi.Rows[i][4].ToString()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_DVQD));
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                db.Database.ExecuteSqlCommand("exec insert_DonViQuiDoi @ID, @ID_HangHoa,@ID_DonVi, @ID_LoHang, @MaHangHoa, @GiaVon, @GiaBan, @timeCreate, @ID_NhanVien", paramlist.ToArray());
            }
            // insert Đối tượng
            DataTable dt_DoiTuong = worksheet_DoiTuong.Cells.ExportDataTable(0, 0, 11, 5, true);
            for (int i = 0; i < dt_DoiTuong.Rows.Count; i++)
            {
                DateTime NewTime_DT = dateTimeNow.AddDays(-50 - i);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_DoiTuong.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("MaDoiTuong", dt_DoiTuong.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("TenDoiTuong", dt_DoiTuong.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("TenDoiTuong_KhongDau", CommonStatic.ConvertToUnSign(dt_DoiTuong.Rows[i][2].ToString()).ToLower()));
                paramlist.Add(new SqlParameter("TenDoiTuong_ChuCaiDau", CommonStatic.GetCharsStart(dt_DoiTuong.Rows[i][2].ToString()).ToLower()));
                //paramlist.Add(new SqlParameter("GioiTinhNam", dt_DoiTuong.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("LoaiDoiTuong", dt_DoiTuong.Rows[i][4].ToString()));
                if (dt_DoiTuong.Rows[i][4].ToString() == "1")
                {
                    paramlist.Add(new SqlParameter("LaCaNhan", "1"));
                    paramlist.Add(new SqlParameter("GioiTinhNam", dt_DoiTuong.Rows[i][3].ToString()));
                }
                else
                {
                    paramlist.Add(new SqlParameter("LaCaNhan", "0"));
                    paramlist.Add(new SqlParameter("GioiTinhNam", "1"));
                }
                paramlist.Add(new SqlParameter("timeCreate", NewTime_DT));
                db.Database.ExecuteSqlCommand("exec insert_DoiTuong @ID,@ID_DonVi, @MaDoiTuong, @TenDoiTuong, @TenDoiTuong_KhongDau, @TenDoiTuong_ChuCaiDau,@GioiTinhNam,@LoaiDoiTuong, @LaCaNhan, @timeCreate", paramlist.ToArray());
            }
            // insert HoaDon
            DataTable dt_HoaDon = worksheet_HoaDon.Cells.ExportDataTable(0, 0, 41, 5, true);
            for (int i = 0; i < dt_HoaDon.Rows.Count; i++)
            {
                DateTime NewTime_HD = dateTimeNow.AddDays(-39 + i);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_HoaDon.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_DoiTuong", dt_HoaDon.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                paramlist.Add(new SqlParameter("MaHoaDon", dt_HoaDon.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("LoaiHoaDon", dt_HoaDon.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("TongTienHang", dt_HoaDon.Rows[i][4].ToString()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_HD));
                db.Database.ExecuteSqlCommand("exec insert_HoaDon @ID, @ID_DoiTuong, @ID_DonVi, @ID_NhanVien, @MaHoaDon, @LoaiHoaDon, @TongTienHang, @timeCreate", paramlist.ToArray());
            }
            // insert HoaDon_ChiTiet_LoHang
            DataTable dt_HoaDon_ChiTiet = worksheet_HoaDon_ChiTiet.Cells.ExportDataTable(0, 0, 48, 7, true);
            for (int i = 0; i < dt_HoaDon_ChiTiet.Rows.Count; i++)
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", dt_HoaDon_ChiTiet.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_DonViQuiDoi", dt_HoaDon_ChiTiet.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("ID_LoHang", dt_HoaDon_ChiTiet.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("SoLuong", dt_HoaDon_ChiTiet.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("DonGia", dt_HoaDon_ChiTiet.Rows[i][4].ToString()));
                paramlist.Add(new SqlParameter("ThanhTien", dt_HoaDon_ChiTiet.Rows[i][5].ToString()));
                paramlist.Add(new SqlParameter("GiaVon", dt_HoaDon_ChiTiet.Rows[i][6].ToString()));
                db.Database.ExecuteSqlCommand("exec insert_HoaDonLoHang_ChiTiet @ID_HoaDon, @ID_DonViQuiDoi,@ID_LoHang, @SoLuong, @DonGia, @ThanhTien, @GiaVon", paramlist.ToArray());
            }
            // insert Quy_HoaDon
            DataTable dt_Quy_HoaDon = worksheet_Quy_HoaDon.Cells.ExportDataTable(0, 0, 41, 4, true);
            for (int i = 0; i < dt_Quy_HoaDon.Rows.Count; i++)
            {
                DateTime NewTime_QHD = dateTimeNow.AddDays(-39 + i);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID", dt_Quy_HoaDon.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                paramlist.Add(new SqlParameter("MaHoaDon", dt_Quy_HoaDon.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("LoaiHoaDon", dt_Quy_HoaDon.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("TongTienThu", dt_Quy_HoaDon.Rows[i][3].ToString()));
                paramlist.Add(new SqlParameter("timeCreate", NewTime_QHD));
                db.Database.ExecuteSqlCommand("exec insert_Quy_HoaDon @ID, @ID_DonVi, @ID_NhanVien, @MaHoaDon, @LoaiHoaDon, @TongTienThu, @timeCreate", paramlist.ToArray());
            }
            // insert Quy_HoaDon_ChiTiet
            DataTable dt_Quy_HoaDon_ChiTiet = worksheet_Quy_HoaDon_ChiTiet.Cells.ExportDataTable(0, 0, 41, 4, true);
            for (int i = 0; i < dt_Quy_HoaDon_ChiTiet.Rows.Count; i++)
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", dt_Quy_HoaDon_ChiTiet.Rows[i][0].ToString()));
                paramlist.Add(new SqlParameter("ID_HoaDonLienQuan", dt_Quy_HoaDon_ChiTiet.Rows[i][1].ToString()));
                paramlist.Add(new SqlParameter("ID_DoiTuong", dt_Quy_HoaDon_ChiTiet.Rows[i][2].ToString()));
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                paramlist.Add(new SqlParameter("TienThu", dt_Quy_HoaDon_ChiTiet.Rows[i][3].ToString()));
                db.Database.ExecuteSqlCommand("exec insert_Quy_HoaDon_ChiTiet @ID_HoaDon, @ID_HoaDonLienQuan, @ID_DoiTuong, @ID_NhanVien, @TienThu", paramlist.ToArray());
            }
            // insert TonKhoKhoiTao
            db.Database.ExecuteSqlCommand("exec insert_TonKhoKhoiTao");
        }

        public List<ErrorDMHangHoa> ImportDanhMucHangHoaDTO(List<string> lst, DataTable dataTable, Guid ID_DonVi, Guid idnhanvien, int LoaiUpdate)
        {
            var lstErr = new List<ErrorDMHangHoa>();
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                    classDonViQuiDoi classDonViQD = new classDonViQuiDoi(db);
                    ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);

                    List<BH_HoaDon_ChiTiet> lstCTKiemKe = new List<BH_HoaDon_ChiTiet>();
                    List<BH_HoaDon_ChiTiet> lstCTDCGV = new List<BH_HoaDon_ChiTiet>();

                    List<DM_GiaVon> lstDMGV = new List<DM_GiaVon>();
                    List<DM_HangHoa_TonKho> lstDMTonKho = new List<DM_HangHoa_TonKho>();

                    var dtNow = DateTime.Now;

                    BH_HoaDon hdDCGV = new BH_HoaDon
                    {
                        ID = Guid.NewGuid(),
                        MaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(18, ID_DonVi, DateTime.Now),
                        NgayLapHoaDon = dtNow,
                        ID_DonVi = ID_DonVi,
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
                        MaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(9, ID_DonVi, DateTime.Now),
                        NgayLapHoaDon = dtNow.AddSeconds(1),
                        ID_DonVi = ID_DonVi,
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
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {

                        var dtRow = dataTable.Rows[i];
                        string dk = "";
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            if (dtRow[j].ToString().Trim() != "")
                            {
                                break;
                            }
                            if (j == dataTable.Columns.Count - 1)
                            {
                                dk = "1";
                            }
                        }

                        if (string.IsNullOrEmpty(dk))
                        {
                            #region infor excel
                            var nhomcha = dtRow[0].ToString().Trim();
                            var nhomcon = dtRow[1].ToString().Trim();
                            var mahanghoa = dtRow[2].ToString().Trim();
                            var tenhanghoa = dtRow[3].ToString().Trim();
                            var lahanghoa = dtRow[4].ToString().Trim() != "" ? false : true;
                            var loaihangEx = dtRow[4].ToString().Trim();
                            var loaihang = 1;
                            switch (loaihangEx)
                            {
                                case "Dịch vụ":
                                    loaihang = 2;
                                    break;
                                case "Combo":
                                    loaihang = 3;
                                    break;

                            }
                            var duocbantructiep = dtRow[5].ToString().Trim() != "" ? false : true;
                            var ghichu = dtRow[6].ToString().Trim();
                            var giavon = dtRow[7].ToString().Trim();
                            var giaban = dtRow[8].ToString().Trim();
                            var tonkho = dtRow[9].ToString().Trim();
                            var tenDVT = dtRow[10].ToString().Trim();
                            var quycach = dtRow[11].ToString().Trim();
                            var dvQuyCach = dtRow[12].ToString().Trim();
                            var madvcoban = dtRow[13].ToString().Trim();
                            var ladvchuan = madvcoban != "" ? false : true;
                            var gtriquydoi = dtRow[14].ToString().Trim();
                            var macungloai = dtRow[15].ToString().Trim();

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

                            #endregion

                            bool existHang = false, existTK = false, existGV = false;
                            var idQuiDoi = Guid.NewGuid();
                            var idHangHoa = Guid.NewGuid();
                            Guid? idGiaVon = null;

                            if (!string.IsNullOrEmpty(mahanghoa))
                            {
                                mahanghoa = mahanghoa.ToUpper();
                                var dmQuiDoi = Select_DonViQuiDoi(mahanghoa);
                                if (dmQuiDoi != null)
                                {
                                    existHang = true;
                                    idQuiDoi = dmQuiDoi.ID;
                                    idHangHoa = dmQuiDoi.ID_HangHoa;

                                    List<SqlParameter> paramlist = new List<SqlParameter>();
                                    paramlist.Add(new SqlParameter("MaHH", mahanghoa));
                                    paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_DonVi));
                                    paramlist.Add(new SqlParameter("ID_LoHang", DBNull.Value));
                                    // get tonho from DM_HangHoa_TonKho
                                    List<Search_HangHoa_importPRC> lstTon = db.Database.SqlQuery<Search_HangHoa_importPRC>("exec getList_DMHangHoa_Import @MaHH, @ID_ChiNhanh, @ID_LoHang", paramlist.ToArray()).ToList();
                                    if (lstTon != null && lstTon.Count() > 0)
                                    {
                                        tonkhoDB = lstTon.FirstOrDefault().TonCuoiKy;
                                        giavonDB = lstTon.FirstOrDefault().GiaVon;

                                        if (loaihang == 1)
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

                            if (loaihang == 1)
                            {
                                DM_GiaVon dmGV = new DM_GiaVon
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonVi = ID_DonVi,
                                    ID_DonViQuiDoi = idQuiDoi,
                                    GiaVon = giavonThucTe,
                                };

                                DM_HangHoa_TonKho dmTK = new DM_HangHoa_TonKho
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonVi = ID_DonVi,
                                    ID_DonViQuyDoi = idQuiDoi,
                                    TonKho = soluongNew,
                                };

                                // neu nhieuDVT: tonkho = sum (tonkho all DVT)
                                if (!ladvchuan && !string.IsNullOrEmpty(madvcoban))
                                {
                                    var dvqdChuan = lstCTKiemKe.Where(x => x.MaHangHoa == madvcoban.ToUpper());
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
                                        var gvChuan = lstCTDCGV.Where(x => x.MaHangHoa == madvcoban.ToUpper());
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
                                classHangHoa.AddMultiple_DMHangHoaTonKho(ID_DonVi, dmTK);
                                classHangHoa.AddMultiple_DMGiaVon(ID_DonVi, dmGV);

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
                                    TonLuyKe = LoaiUpdate == 2 ? existTK ? tonkhoDB * gtriquydoiNew : 0 : tonkhoDB * gtriquydoiNew,
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
                                    TonLuyKe = LoaiUpdate == 2 || existHang == false ? tonluykeNew : tonkhoDB * gtriquydoiNew,
                                };
                                // chi add kiemke if update tonkho
                                if (LoaiUpdate == 2 || existHang == false)
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
                            prmSQL.Add(new SqlParameter("isUpdateTonKho", LoaiUpdate));// update TonKho (2.yes, 1.no)
                            prmSQL.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                            prmSQL.Add(new SqlParameter("ID_HangHoa", idHangHoa));
                            prmSQL.Add(new SqlParameter("ID_DonViQuiDoi", idQuiDoi));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoaCha", nhomcha));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoaCha_KhongDau", nhomcha_KhongDau));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoaCha_KyTuDau", nhomcha_KyTuDau));
                            prmSQL.Add(new SqlParameter("MaNhomHangHoaCha", DateTime.Now.ToString("yyyyMMddHHmmss")));

                            prmSQL.Add(new SqlParameter("TenNhomHangHoa", nhomcon));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoa_KhongDau", nhomcon_KhongDau));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoa_KyTuDau", nhomcon_KyTuDau));
                            prmSQL.Add(new SqlParameter("MaNhomHangHoa", DateTime.Now.ToString("yyyyMMddHHmmss")));
                            prmSQL.Add(new SqlParameter("LoaiHangHoa", loaihang));
                            prmSQL.Add(new SqlParameter("TenHangHoa", tenhanghoa));
                            prmSQL.Add(new SqlParameter("TenHangHoa_KhongDau", tenhang_KhongDau));
                            prmSQL.Add(new SqlParameter("TenHangHoa_KyTuDau", tenhang_KyTuDau));
                            prmSQL.Add(new SqlParameter("GhiChu", ghichu));
                            prmSQL.Add(new SqlParameter("QuyCach", quyCachNew));
                            prmSQL.Add(new SqlParameter("DuocBanTrucTiep", duocbantructiep));

                            prmSQL.Add(new SqlParameter("MaDonViCoBan", madvcoban));
                            prmSQL.Add(new SqlParameter("MaHangHoa", mahanghoa));
                            prmSQL.Add(new SqlParameter("TenDonViTinh", tenDVT));
                            prmSQL.Add(new SqlParameter("GiaVon", giavonThucTe));
                            prmSQL.Add(new SqlParameter("GiaBan", giaBanNew));
                            prmSQL.Add(new SqlParameter("TonKho", lahanghoa ? tonluykeNew / gtriquydoiNew : 0));
                            prmSQL.Add(new SqlParameter("LaDonViChuan", ladvchuan));
                            prmSQL.Add(new SqlParameter("TyLeChuyenDoi", gtriquydoiNew));
                            prmSQL.Add(new SqlParameter("MaHangHoaChaCungLoai", macungloai));
                            prmSQL.Add(new SqlParameter("DVTQuyCach", dvQuyCach));

                            db.Database.ExecuteSqlCommand("Exec import_DanhMucHangHoa @isUpdateHang, @isUpdateTonKho, @ID_DonVi, @ID_HangHoa, @ID_DonViQuiDoi," +
                                " @TenNhomHangHoaCha,@TenNhomHangHoaCha_KhongDau,@TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, " +
                                "@TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau," +
                                 "@MaNhomHangHoa, @LoaiHangHoa, @TenHangHoa, @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau," +
                                 "@GhiChu, @QuyCach, @DuocBanTrucTiep, @MaDonViCoBan, @MaHangHoa, @TenDonViTinh, @GiaVon, @GiaBan, @TonKho," +
                                 " @LaDonViChuan, @TyLeChuyenDoi, @MaHangHoaChaCungLoai,@DVTQuyCach", prmSQL.ToArray());
                            #endregion

                            #region ThuocTinh HangHoa
                            int m = 0;
                            for (int j = 16; j < dataTable.Columns.Count; j++)
                            {
                                if (dtRow[j].ToString().Trim() != "")
                                {
                                    string TenThuocTinh = string.Empty;
                                    try
                                    {
                                        TenThuocTinh = lst[j - 16].ToString();
                                    }
                                    catch
                                    {
                                        TenThuocTinh = "Thuộc tính " + (j - 15).ToString();
                                    }
                                    List<SqlParameter> parama = new List<SqlParameter>();
                                    parama.Add(new SqlParameter("TenThuocTinh", TenThuocTinh));
                                    parama.Add(new SqlParameter("GiaTri", dtRow[j].ToString().Trim()));
                                    parama.Add(new SqlParameter("ThuTuNhap", m));
                                    parama.Add(new SqlParameter("MaHangHoa", mahanghoa));
                                    m++;
                                    db.Database.ExecuteSqlCommand("Exec import_HangHoaThuocTinh @TenThuocTinh, @GiaTri, @ThuTuNhap, @MaHangHoa", parama.ToArray());
                                }
                            }
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
                return lstErr;
            }
        }

        public List<ErrorDMHangHoa> ImportDanhMucHangHoaDTO_LoHang(List<string> lst, DataTable dataTable, Guid ID_DonVi, Guid idnhanvien, int LoaiUpdate)
        {
            var lstErr = new List<ErrorDMHangHoa>();
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                    classDonViQuiDoi classDonViQD = new classDonViQuiDoi(db);
                    ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);

                    List<BH_HoaDon_ChiTiet> lstCTKiemKe = new List<BH_HoaDon_ChiTiet>();
                    List<BH_HoaDon_ChiTiet> lstCTDCGV = new List<BH_HoaDon_ChiTiet>();

                    List<DM_GiaVon> lstDMGV = new List<DM_GiaVon>();
                    List<DM_HangHoa_TonKho> lstDMTonKho = new List<DM_HangHoa_TonKho>();

                    var dtNow = DateTime.Now;
                    BH_HoaDon hdDCGV = new BH_HoaDon
                    {
                        ID = Guid.NewGuid(),
                        MaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(18, ID_DonVi, DateTime.Now),
                        NgayLapHoaDon = dtNow,
                        ID_DonVi = ID_DonVi,
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
                        MaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(9, ID_DonVi, DateTime.Now),
                        NgayLapHoaDon = dtNow.AddSeconds(1),
                        ID_DonVi = ID_DonVi,
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
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string dk = "";
                        var dtRow = dataTable.Rows[i];

                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            if (dtRow[j].ToString().Trim() != "")
                            {
                                break;
                            }
                            if (j == dataTable.Columns.Count - 1)
                            {
                                dk = "1";
                            }
                        }

                        if (string.IsNullOrEmpty(dk))
                        {
                            #region infor excel
                            var nhomcha = dtRow[0].ToString().Trim();
                            var nhomcon = dtRow[1].ToString().Trim();
                            var malohang = dtRow[2].ToString().Trim();
                            var ngaysx = dtRow[3].ToString().Trim();
                            var hansd = dtRow[4].ToString().Trim();

                            var mahanghoa = dtRow[5].ToString().Trim();
                            var tenhanghoa = dtRow[6].ToString().Trim();
                            var loaihangEx = dtRow[7].ToString().Trim();
                            var loaihang = 1;
                            switch (loaihangEx)
                            {
                                case "Dịch vụ":
                                    loaihang = 2;
                                    break;
                                case "Combo":
                                    loaihang = 3;
                                    break;

                            }
                            var duocbantructiep = dtRow[8].ToString().Trim() != "" ? false : true;
                            var ghichu = dtRow[9].ToString().Trim();
                            var giavon = dtRow[10].ToString().Trim();
                            var giaban = dtRow[11].ToString().Trim();
                            var tonkho = dtRow[12].ToString().Trim();
                            var tenDVT = dtRow[13].ToString().Trim();
                            var quycach = dtRow[14].ToString().Trim();
                            var dvQuyCach = dtRow[15].ToString().Trim();
                            var madvcoban = dtRow[16].ToString().Trim();
                            var ladvchuan = madvcoban != "" ? false : true;
                            var gtriquydoi = dtRow[17].ToString().Trim();
                            var macungloai = dtRow[18].ToString().Trim();

                            var tenhang_KhongDau = CommonStatic.ConvertToUnSign(tenhanghoa).ToLower();
                            var tenhang_KyTuDau = CommonStatic.GetCharsStart(tenhanghoa).ToLower();
                            var nhomcha_KhongDau = CommonStatic.ConvertToUnSign(nhomcha).ToLower();
                            var nhomcha_KyTuDau = CommonStatic.GetCharsStart(nhomcha).ToLower();
                            var nhomcon_KhongDau = CommonStatic.ConvertToUnSign(nhomcon).ToLower();
                            var nhomcon_KyTuDau = CommonStatic.GetCharsStart(nhomcon).ToLower();

                            double tonkhoDB = 0, giavonDB = 0;
                            double soluongNew = 0, tonluykeNew = 0, giavonThucTe = 0, giaBanNew = 0, quyCachNew = 1, gtriquydoiNew = 1;
                            DateTime? ngaysxNew = null, hansdNew = null;
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
                            if (!string.IsNullOrEmpty(ngaysx))
                            {
                                ngaysxNew = DateTime.Parse(ngaysx);
                            }
                            if (!string.IsNullOrEmpty(hansd))
                            {
                                hansdNew = DateTime.Parse(hansd);
                            }
                            #endregion

                            bool existHang = false, existLo = false, existTK = false, existGV = false;
                            Guid idQuiDoi = Guid.NewGuid();
                            Guid idHangHoa = Guid.NewGuid();
                            Guid? idLoHang = null;
                            Guid? idGiaVon = null;

                            if (!string.IsNullOrEmpty(malohang))
                            {
                                idLoHang = Guid.NewGuid();
                            }

                            if (!string.IsNullOrEmpty(mahanghoa))
                            {
                                mahanghoa = mahanghoa.ToUpper();
                                var dmQuiDoi = Select_DonViQuiDoi(mahanghoa);
                                if (dmQuiDoi != null)
                                {
                                    existHang = true;
                                    idQuiDoi = dmQuiDoi.ID;
                                    idHangHoa = dmQuiDoi.ID_HangHoa;

                                    List<Search_HangHoa_importPRC> lstTon = new List<Search_HangHoa_importPRC>();
                                    List<SqlParameter> paramlist = new List<SqlParameter>();
                                    if (!string.IsNullOrEmpty(malohang))
                                    {
                                        paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_DonVi));
                                        paramlist.Add(new SqlParameter("ID_DonViQuiDoi", dmQuiDoi.ID));
                                        paramlist.Add(new SqlParameter("MaLoHang", malohang));
                                        lstTon = db.Database.SqlQuery<Search_HangHoa_importPRC>("exec getList_DMLoHang_TonKho_byMaLoHang @ID_ChiNhanh, @ID_DonViQuiDoi, @MaLoHang", paramlist.ToArray()).ToList();
                                    }
                                    else
                                    {
                                        paramlist.Add(new SqlParameter("MaHH", mahanghoa));
                                        paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_DonVi));
                                        paramlist.Add(new SqlParameter("ID_LoHang", DBNull.Value));
                                        lstTon = db.Database.SqlQuery<Search_HangHoa_importPRC>("exec getList_DMHangHoa_Import @MaHH, @ID_ChiNhanh, @ID_LoHang", paramlist.ToArray()).ToList();
                                    }

                                    if (lstTon != null && lstTon.Count() > 0)
                                    {
                                        tonkhoDB = lstTon.FirstOrDefault().TonCuoiKy;
                                        giavonDB = lstTon.FirstOrDefault().GiaVon;
                                        idLoHang = lstTon.FirstOrDefault().ID_LoHang;
                                        existLo = true;

                                        if (loaihang == 1)
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
                                            existLo = true;
                                        }
                                    }
                                    else
                                    {
                                        existLo = false;
                                    }
                                }
                            }
                            else
                            {
                                mahanghoa = classDonViQD.GetMaHangHoa();
                            }
                            if (loaihang == 1)
                            {
                                DM_GiaVon dmGV = new DM_GiaVon
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonVi = ID_DonVi,
                                    ID_DonViQuiDoi = idQuiDoi,
                                    ID_LoHang = idLoHang,
                                    GiaVon = giavonThucTe,
                                };
                                DM_HangHoa_TonKho dmTK = new DM_HangHoa_TonKho
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonVi = ID_DonVi,
                                    ID_DonViQuyDoi = idQuiDoi,
                                    ID_LoHang = idLoHang,
                                    TonKho = soluongNew,
                                };

                                // neu nhieuDVT: tonkho = sum (tonkho all DVT)
                                if (!ladvchuan && !string.IsNullOrEmpty(madvcoban))
                                {
                                    var dvqdChuan = lstCTKiemKe.Where(x => x.MaHangHoa == madvcoban.ToUpper());
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
                                        var gvChuan = lstCTDCGV.Where(x => x.MaHangHoa == madvcoban.ToUpper());
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
                                classHangHoa.AddMultiple_DMGiaVon(ID_DonVi, dmGV);
                                classHangHoa.AddMultiple_DMHangHoaTonKho(ID_DonVi, dmTK);

                                var lechGV = giavonThucTe - giavonDB;
                                BH_HoaDon_ChiTiet dcGV = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_HoaDon = hdDCGV.ID,
                                    ID_DonViQuiDoi = idQuiDoi,
                                    ID_LoHang = idLoHang,
                                    MaHangHoa = mahanghoa,
                                    DonGia = giavonDB,
                                    GiaVon = giavonThucTe,
                                    PTChietKhau = lechGV > 0 ? lechGV : 0,
                                    ThanhToan = lechGV < 0 ? lechGV : 0,
                                    TonLuyKe = LoaiUpdate == 2 ? existTK ? tonkhoDB * gtriquydoiNew : 0 : tonkhoDB * gtriquydoiNew,
                                };
                                lstCTDCGV.Add(dcGV);

                                var slLech = soluongNew - tonkhoDB;
                                BH_HoaDon_ChiTiet ctKK = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_HoaDon = hdKK.ID,
                                    ID_DonViQuiDoi = idQuiDoi,
                                    ID_LoHang = idLoHang,
                                    ID_HangHoa = idHangHoa,
                                    MaHangHoa = mahanghoa,
                                    SoLuong = slLech,
                                    GiaVon = giavonThucTe,
                                    ThanhTien = soluongNew,
                                    TienChietKhau = tonkhoDB,
                                    ThanhToan = giavonThucTe * slLech,
                                    TonLuyKe = LoaiUpdate == 2 || existHang == false || existLo == false ? tonluykeNew : tonkhoDB * gtriquydoiNew,
                                };
                                // chi add kiemke if update tonkho/ hoac lohang not exists dm_kho
                                if (LoaiUpdate == 2 || existHang == false || existLo == false)
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
                            prmSQL.Add(new SqlParameter("isUpdateTonKho", LoaiUpdate));// update TonKho (2.yes, 1.no)
                            prmSQL.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                            prmSQL.Add(new SqlParameter("ID_HangHoa", idHangHoa));
                            prmSQL.Add(new SqlParameter("ID_DonViQuiDoi", idQuiDoi));
                            prmSQL.Add(new SqlParameter("ID_LoHang", idLoHang ?? (object)DBNull.Value));

                            prmSQL.Add(new SqlParameter("TenNhomHangHoaCha", nhomcha));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoaCha_KhongDau", nhomcha_KhongDau));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoaCha_KyTuDau", nhomcha_KyTuDau));
                            prmSQL.Add(new SqlParameter("MaNhomHangHoaCha", DateTime.Now.ToString("yyyyMMddHHmmss")));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoa", nhomcon));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoa_KhongDau", nhomcon_KhongDau));
                            prmSQL.Add(new SqlParameter("TenNhomHangHoa_KyTuDau", nhomcon_KyTuDau));
                            prmSQL.Add(new SqlParameter("MaNhomHangHoa", DateTime.Now.ToString("yyyyMMddHHmmss")));
                            prmSQL.Add(new SqlParameter("LoaiHangHoa", loaihang));
                            prmSQL.Add(new SqlParameter("TenHangHoa", tenhanghoa));
                            prmSQL.Add(new SqlParameter("TenHangHoa_KhongDau", tenhang_KhongDau));
                            prmSQL.Add(new SqlParameter("TenHangHoa_KyTuDau", tenhang_KyTuDau));
                            prmSQL.Add(new SqlParameter("GhiChu", ghichu));
                            prmSQL.Add(new SqlParameter("QuyCach", quyCachNew));
                            prmSQL.Add(new SqlParameter("DuocBanTrucTiep", duocbantructiep));
                            prmSQL.Add(new SqlParameter("MaDonViCoBan", madvcoban));
                            prmSQL.Add(new SqlParameter("MaHangHoa", mahanghoa));
                            prmSQL.Add(new SqlParameter("TenDonViTinh", tenDVT));
                            prmSQL.Add(new SqlParameter("GiaVon", giavonThucTe));
                            prmSQL.Add(new SqlParameter("GiaBan", giaBanNew));
                            prmSQL.Add(new SqlParameter("TonKho", loaihang == 1 ? tonluykeNew / gtriquydoiNew : 0));
                            prmSQL.Add(new SqlParameter("LaDonViChuan", ladvchuan));
                            prmSQL.Add(new SqlParameter("TyLeChuyenDoi", gtriquydoiNew));
                            prmSQL.Add(new SqlParameter("MaHangHoaChaCungLoai", macungloai));
                            prmSQL.Add(new SqlParameter("DVTQuyCach", dvQuyCach));
                            prmSQL.Add(new SqlParameter("MaLoHang", malohang));
                            prmSQL.Add(new SqlParameter("NgaySanXuat", ngaysxNew ?? (object)DBNull.Value));
                            prmSQL.Add(new SqlParameter("NgayHetHan", hansdNew ?? (object)DBNull.Value));

                            db.Database.ExecuteSqlCommand("Exec import_DanhMucHangHoaLoHang @isUpdateHang, @isUpdateTonKho, @ID_DonVi, @ID_HangHoa, @ID_DonViQuiDoi, " +
                                "@ID_LoHang, @TenNhomHangHoaCha,@TenNhomHangHoaCha_KhongDau,@TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, " +
                                "@TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau," +
                                 "@MaNhomHangHoa, @LoaiHangHoa, @TenHangHoa, @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau," +
                                 "@GhiChu, @QuyCach, @DuocBanTrucTiep, @MaDonViCoBan, @MaHangHoa, @TenDonViTinh, @GiaVon, @GiaBan,@TonKho, @LaDonViChuan, " +
                                 "@TyLeChuyenDoi, @MaHangHoaChaCungLoai,@DVTQuyCach, @MaLoHang, @NgaySanXuat, @NgayHetHan", prmSQL.ToArray());

                            if (loaihang == 1)
                            {
                                #region ThuocTinh HangHoa -- insert after insert hang
                                int m = 0;
                                for (int j = 19; j < dataTable.Columns.Count; j++)
                                {
                                    if (dtRow[j].ToString().Trim() != "")
                                    {
                                        string TenThuocTinh = string.Empty;
                                        try
                                        {
                                            TenThuocTinh = lst[j - 19].ToString();
                                        }
                                        catch
                                        {
                                            TenThuocTinh = "Thuộc tính " + (j - 18).ToString();
                                        }
                                        List<SqlParameter> parama = new List<SqlParameter>();
                                        parama.Add(new SqlParameter("TenThuocTinh", TenThuocTinh));
                                        parama.Add(new SqlParameter("GiaTri", dtRow[j].ToString().Trim()));
                                        parama.Add(new SqlParameter("ThuTuNhap", m));
                                        parama.Add(new SqlParameter("MaHangHoa", mahanghoa));
                                        db.Database.ExecuteSqlCommand("Exec import_HangHoaThuocTinh @TenThuocTinh, @GiaTri, @ThuTuNhap, @MaHangHoa", parama.ToArray());
                                        m++;
                                    }
                                }
                                #endregion
                            }
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
                    db.DM_GiaVon.AddRange(lstDMGV);
                    db.DM_HangHoa_TonKho.AddRange(lstDMTonKho);
                    db.BH_HoaDon_ChiTiet.AddRange(lstCTKiemKe);
                    db.BH_HoaDon_ChiTiet.AddRange(lstCTDCGV);
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
                return lstErr;
            }
        }
        public void ImportBangGiaDTO(DataTable dataTable, Guid ID_DonVi, Guid ID_BangGia)
        {
            var _classDVQD = new classDonViQuiDoi(db);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (dataTable.Rows[i][j].ToString().Trim() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    List<SqlParameter> prm = new List<SqlParameter>();
                    var ID = Guid.NewGuid();
                    var ID_KhoHang = new Guid("01CD02F2-4612-4104-B790-1C0373CBD72D");
                    var ID_NgoaiTe = new Guid("406eed2d-faae-4520-aef2-12912f83dda2");
                    var MaHang = dataTable.Rows[i][0].ToString().Trim();
                    Guid ID_DonViQuiDoi = _classDVQD.Get(p => p.MaHangHoa == MaHang).ID;

                    prm.Add(new SqlParameter("ID", ID));
                    prm.Add(new SqlParameter("ID_KhoHang", ID_KhoHang));
                    prm.Add(new SqlParameter("ID_NgoaiTe", ID_NgoaiTe));

                    prm.Add(new SqlParameter("GiaBan", dataTable.Rows[i][1].ToString().Trim()));
                    prm.Add(new SqlParameter("ID_GiaBan", ID_BangGia));
                    prm.Add(new SqlParameter("ID_DonViQuiDoi", ID_DonViQuiDoi));

                    db.Database.ExecuteSqlCommand("Exec ImportHHBangGia @ID, @ID_KhoHang, @ID_NgoaiTe, @GiaBan, @ID_GiaBan,@ID_DonViQuiDoi", prm.ToArray());
                }
            }
        }

        public List<ErrorDMHangHoa> CheckImport_withError(Stream fileInput, Guid ID_DonVi, Guid ID_NhanVien, string RownError, int LoaiUpdate)
        {
            List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
            try
            {
                Workbook objWorkbook = new Workbook(fileInput);
                Worksheet worksheet = objWorkbook.Worksheets[0];

                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);

                List<string> lst = new List<string>();
                for (int i = 16; i < dt.Columns.Count; i++)
                {
                    if (dt.Rows[0][i].ToString().Trim() != "…" & dt.Rows[0][i].ToString().Trim() != "")
                        lst.Add(dt.Rows[0][i].ToString().Trim());
                }
                lstErr = ImportHangHoa_WithError(lst, dt, ID_DonVi, ID_NhanVien, RownError, LoaiUpdate);
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
            }
            return lstErr;
        }
        public List<ErrorDMHangHoa> CheckImport_LoHang_withError(Stream fileInput, Guid ID_DonVi, Guid ID_NhanVien, string RownError, int LoaiUpdate)
        {
            List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
            try
            {
                Workbook objWorkbook = new Workbook(fileInput);
                Worksheet worksheet = objWorkbook.Worksheets[0];

                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dt = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);

                List<string> lst = new List<string>();
                for (int i = 19; i < dt.Columns.Count; i++)
                {
                    if (dt.Rows[0][i].ToString().Trim() != "…" & dt.Rows[0][i].ToString().Trim() != "")
                        lst.Add(dt.Rows[0][i].ToString().Trim());
                }
                lstErr = ImportHangHoa_LoHang_WithError(lst, dt, ID_DonVi, ID_NhanVien, RownError, LoaiUpdate);
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
            }
            return lstErr;
        }

        public void CheckImportBG_withError(Stream fileInput, Guid ID_DonVi, string RownError, Guid ID_BangGia)
        {
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn;
            DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);
            ImportBangGia_WithError(dt, ID_DonVi, RownError, ID_BangGia);
        }

        public List<ErrorDMHangHoa> ImportHangHoa_WithError(List<string> lst, DataTable dataTable, Guid ID_DonVi, Guid idnhanvien, string RownError, int LoaiUpdate)
        {
            List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
            try
            {
                dataTable.Rows[0].Delete();
                if (!string.IsNullOrEmpty(RownError))
                {
                    string[] mang = RownError.Split('_');
                    for (int i = mang.Length - 1; i >= 0; i--)
                    {
                        int j = int.Parse(mang[i].ToString());
                        dataTable.Rows[j].Delete();
                    }
                }

                lstErr = ImportDanhMucHangHoaDTO(lst, dataTable, ID_DonVi, idnhanvien, LoaiUpdate);
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
            }
            return lstErr;
        }
        public List<ErrorDMHangHoa> ImportHangHoa_LoHang_WithError(List<string> lst, DataTable dataTable, Guid ID_DonVi, Guid idnhanvien, string RownError, int LoaiUpdate)
        {
            List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
            try
            {
                dataTable.Rows[0].Delete();
                if (!string.IsNullOrEmpty(RownError))
                {
                    string[] mang = RownError.Split('_');
                    for (int i = mang.Length - 1; i >= 0; i--)
                    {
                        int j = int.Parse(mang[i].ToString());
                        dataTable.Rows[j].Delete();
                    }
                }
                lstErr = ImportDanhMucHangHoaDTO_LoHang(lst, dataTable, ID_DonVi, idnhanvien, LoaiUpdate);
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
            }
            return lstErr;
        }

        public void ImportBangGia_WithError(DataTable dataTable, Guid ID_DonVi, string RownError, Guid ID_BangGia)
        {
            try
            {
                string[] mang = RownError.Split('_');
                for (int i = mang.Length - 1; i >= 0; i--)
                {
                    int j = int.Parse(mang[i].ToString());
                    dataTable.Rows[j].Delete();
                }
            }
            catch (Exception ex)
            {
                string str = CookieStore.GetCookieAes("SubDomain");
                CookieStore.WriteLog("ImportBangGia_WithError(DataTable dataTable, Guid ID_DonVi, string RownError, Guid ID_BangGia): " + ex.InnerException + ex.Message, str);
            }
            ImportBangGiaDTO(dataTable, ID_DonVi, ID_BangGia);
        }
        // check lỗi file import danh sách nhân viên todo
        public List<ErrorDMHangHoa> checkfileDanhSachNhanVien(Stream inputFileExcel, Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        // kiểm tra mã nhân viên có kí tự đặc biệt không
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        bool valiDateMaKH = kiemtrakitu(MaNhanVien);
                        if (valiDateMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã nhân viên";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = MaNhanVien;
                            DM.DienGiai = "Mã nhân viên không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra trùng lặp mã nhân viên
                        bool duplicateMaKH = GroupData(dataTable, "MaNhanVien = '" + MaNhanVien + "'");
                        if (duplicateMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã nhân viên";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = MaNhanVien;
                            DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " bị trùng lặp";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " đã tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }

                        // kiểm tra tên nhân viên
                        if (dataTable.Rows[i][1].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tên nhân viên";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Tên nhân viên không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][2].ToString() != "x" && dataTable.Rows[i][2].ToString() != "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Giới tính";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Giới tính là Nam: bạn cần đánh dấu x";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][3].ToString() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][3].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày sinh";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                DM.DienGiai = "Ngày sinh không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][3]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày sinh";
                                        DM.ViTri = "Dòng số: " + (i + 3).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                        DM.DienGiai = "Ngày sinh không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        var didong = dataTable.Rows[i][4].ToString().Trim();
                        if (didong != "")
                        {
                            bool isNumber = IsNumberInt(didong);
                            if (isNumber == false /*|| dataTable.Rows[i][8].ToString().Trim().Length > 11*/)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Điện thoại";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = didong;
                                DM.DienGiai = "số điện thoại không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                bool checkSDT = classdoituong.TR_CheckSoDienThoai_Exist(didong);
                                if (checkSDT)
                                {
                                    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                    DM.TenTruongDuLieu = "Di động";
                                    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                    DM.ThuocTinh = didong;
                                    DM.DienGiai = "Số điện thoại: " + didong + " đã tồn tại trong cơ sở dữ liệu";
                                    DM.rowError = i;
                                    lstError.Add(DM);
                                }
                            }
                        }
                        if (dataTable.Rows[i][5].ToString() != "")
                        {
                            bool valiDateDiaChi = ValidateEmail(dataTable.Rows[i][5].ToString());
                            if (valiDateDiaChi == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Emial";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][5].ToString();
                                DM.DienGiai = "Emlai không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                bool checkEMail = classdoituong.SP_CheckEmail_Exist(dataTable.Rows[i][5].ToString());
                                if (checkEMail)
                                {
                                    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                    DM.TenTruongDuLieu = "Email";
                                    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                    DM.ThuocTinh = dataTable.Rows[i][5].ToString();
                                    DM.DienGiai = "Email: " + dataTable.Rows[i][5].ToString() + " đã tồn tại trong cơ sở dữ liệu";
                                    DM.rowError = i;
                                    lstError.Add(DM);
                                }
                            }
                        }
                        if (dataTable.Rows[i][10].ToString() != "x" && dataTable.Rows[i][10].ToString() != "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tình trạng";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][10].ToString();
                            DM.DienGiai = "Đã nghỉ: bạn cần đánh dấu x";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importDanhSachNhanVien(dataTable, idDonVi);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin ca làm việc
        public List<ErrorDMHangHoa> checkfileThongTinCaLamViec(Stream inputFileExcel, string NguoiTao)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(3, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[0].ColumnName = "MaCaLamViec";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        // kiểm tra mã nhân viên có kí tự đặc biệt không
                        var MaCaLamViec = dataTable.Rows[i][0].ToString().Trim();
                        bool valiDateMaKH = kiemtrakitu(MaCaLamViec);
                        if (valiDateMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã ca";
                            DM.ViTri = "Dòng số: " + (i + 5).ToString();
                            DM.ThuocTinh = MaCaLamViec;
                            DM.DienGiai = "Mã ca làm việc không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra trùng lặp mã nhân viên
                        bool duplicateMaKH = GroupData(dataTable, "MaCaLamViec = '" + MaCaLamViec + "'");
                        if (duplicateMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã ca";
                            DM.ViTri = "Dòng số: " + (i + 5).ToString();
                            DM.ThuocTinh = MaCaLamViec;
                            DM.DienGiai = "Mã ca làm việc: " + MaCaLamViec + " bị trùng lặp";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaCaLamViec != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaCaLamViec_Exist(MaCaLamViec);
                            if (ExistMaNV)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã ca";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = MaCaLamViec;
                                DM.DienGiai = "Mã ca làm việc: " + MaCaLamViec + " đã tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        // kiểm tra tên nhân viên
                        if (dataTable.Rows[i][1].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tên ca";
                            DM.ViTri = "Dòng số: " + (i + 5).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Tên ca làm việc không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][2].ToString() != "1" && dataTable.Rows[i][2].ToString() != "2")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Trạng thái";
                            DM.ViTri = "Dòng số: " + (i + 5).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Trạng thái ca làm việc không đúng quy định";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][3].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Giờ vào";
                            DM.ViTri = "Dòng số: " + (i + 5).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                            DM.DienGiai = "Giờ vào không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][3].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Giờ vào";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                DM.DienGiai = "Giờ vào ca làm việc không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }

                        if (dataTable.Rows[i][4].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Giờ ra";
                            DM.ViTri = "Dòng số: " + (i + 5).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                            DM.DienGiai = "Giờ ra không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][4].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Giờ ra";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                                DM.DienGiai = "Giờ ra không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][3].ToString() != "" && dataTable.Rows[i][4].ToString() != "" && dataTable.Rows[i][12].ToString() == "")
                        {
                            string[] ar_time = dataTable.Rows[i][3].ToString().Split(':');
                            DateTime timeToUse = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                            string[] ar_time1 = dataTable.Rows[i][4].ToString().Split(':');
                            DateTime timeToUse1 = new DateTime(2000, 01, 01, int.Parse(ar_time1[0]), int.Parse(ar_time1[1]), 00);
                            if (timeToUse1 <= timeToUse)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Giờ làm việc";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = "";
                                DM.DienGiai = "Giờ vào không được lớn hơn giờ ra";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][5].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][5].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Nghỉ giữa ca";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][5].ToString();
                                DM.DienGiai = "Nghỉ giữa ca từ không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][6].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][6].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Nghỉ giữa ca";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][6].ToString();
                                DM.DienGiai = "Nghỉ giữa ca đến không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][7].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][7].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Giờ làm thêm ban ngày";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][7].ToString();
                                DM.DienGiai = "Giờ làm thêm ban ngày từ không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][8].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][8].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Giờ làm thêm ban ngày";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][8].ToString();
                                DM.DienGiai = "Giờ làm thêm ban ngày đến không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][9].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][9].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Giờ làm thêm ban đêm";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][9].ToString();
                                DM.DienGiai = "Giờ làm thêm ban đêm từ không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][10].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][10].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Giờ làm thêm ban đêm";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][10].ToString();
                                DM.DienGiai = "Giờ làm thêm ban đêm đến không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }

                        if (dataTable.Rows[i][11].ToString() != "1" && dataTable.Rows[i][11].ToString() != "2" && dataTable.Rows[i][11].ToString() != "3")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Cách lấy công";
                            DM.ViTri = "Dòng số: " + (i + 5).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][11].ToString();
                            DM.DienGiai = "cách lấy công không đúng quy định";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][12].ToString() != "x" && dataTable.Rows[i][12].ToString() != "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Là ca đêm";
                            DM.ViTri = "Dòng số: " + (i + 5).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][12].ToString();
                            DM.DienGiai = "Là ca đêm bạn cần đánh dấu: x";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][14].ToString().Trim() != "")
                        {
                            bool isNumber4 = IsNumber(dataTable.Rows[i][14].ToString().Trim());
                            if (isNumber4 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Số phút đi muộn";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][14].ToString().Trim();
                                DM.DienGiai = "Số phút đi muộn: '" + dataTable.Rows[i][14].ToString().Trim() + "'  không phải dạng số";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][15].ToString().Trim() != "")
                        {
                            bool isNumber4 = IsNumber(dataTable.Rows[i][15].ToString().Trim());
                            if (isNumber4 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Số phút về sớm";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][15].ToString().Trim();
                                DM.DienGiai = "Số phút về sớm: '" + dataTable.Rows[i][15].ToString().Trim() + "'  không phải dạng số";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][16].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][16].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Khoảng thời gian tính giờ vào";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][16].ToString();
                                DM.DienGiai = "Khoảng thời gian tính giờ vào từ không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][17].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][17].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Khoảng thời gian tính giờ vào";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][17].ToString();
                                DM.DienGiai = "Khoảng thời gian tính giờ vào đến không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][18].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][18].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Khoảng thời gian tính giờ ra";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][18].ToString();
                                DM.DienGiai = "Khoảng thời gian tính giờ ra từ không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][19].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][19].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Khoảng thời gian tính giờ ra";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][19].ToString();
                                DM.DienGiai = "Khoảng thời gian tính giờ ra đến không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][20].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][20].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Tính giờ làm thêm ban ngày";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][20].ToString();
                                DM.DienGiai = "Tính giờ làm thêm ban ngày từ không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][21].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][21].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Tính giờ làm thêm ban ngày";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][21].ToString();
                                DM.DienGiai = "Tính giờ làm thêm ban ngày đến không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][22].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][22].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Giờ làm thêm ban đêm";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][22].ToString();
                                DM.DienGiai = "Giờ làm thêm ban đêm từ không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][23].ToString() != "")
                        {
                            bool check = classdoituong.check_TimetoDate(dataTable.Rows[i][23].ToString());
                            if (check == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Tính giờ làm thêm ban đêm";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][23].ToString();
                                DM.DienGiai = "Tính giờ làm thêm ban đêm đến không đúng định dạng";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][24].ToString().Trim() != "")
                        {
                            bool isNumber4 = IsNumber(dataTable.Rows[i][24].ToString().Trim());
                            if (isNumber4 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Số giờ làm thêm tối thiểu";
                                DM.ViTri = "Dòng số: " + (i + 5).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][24].ToString().Trim();
                                DM.DienGiai = "Số giờ làm thêm tối thiểu: '" + dataTable.Rows[i][24].ToString().Trim() + "'  không phải dạng số";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinCaLamViec(dataTable, NguoiTao);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin nhân viên
        public List<ErrorDMHangHoa> checkfileThongTinNhanVien(Stream inputFileExcel, Guid ID_ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        // kiểm tra mã nhân viên có kí tự đặc biệt không
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        bool valiDateMaKH = kiemtrakitu(MaNhanVien);
                        if (valiDateMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã nhân viên";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = MaNhanVien;
                            DM.DienGiai = "Mã nhân viên không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra trùng lặp mã nhân viên
                        bool duplicateMaKH = GroupData(dataTable, "MaNhanVien = '" + MaNhanVien + "'");
                        if (duplicateMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã nhân viên";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = MaNhanVien;
                            DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " bị trùng lặp";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " đã tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }

                        // kiểm tra tên nhân viên
                        if (dataTable.Rows[i][1].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tên nhân viên";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Tên nhân viên không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][2].ToString() != "x" && dataTable.Rows[i][2].ToString() != "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Giới tính";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Giới tính là Nam: bạn cần đánh dấu x";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][3].ToString() != "x" && dataTable.Rows[i][3].ToString() != "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tình trạng hôn nhân";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                            DM.DienGiai = "Có gia đình: bạn cần đánh dấu x";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][4].ToString() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][4].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày sinh";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                                DM.DienGiai = "Ngày sinh không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][4]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày sinh";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                                        DM.DienGiai = "Ngày sinh không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][7].ToString() != "")
                        {
                            bool valiDateDiaChi = ValidateEmail(dataTable.Rows[i][7].ToString());
                            if (valiDateDiaChi == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Emial";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][7].ToString();
                                DM.DienGiai = "Emlai không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                bool checkEMail = classdoituong.SP_CheckEmail_Exist(dataTable.Rows[i][7].ToString());
                                if (checkEMail)
                                {
                                    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                    DM.TenTruongDuLieu = "Email";
                                    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                    DM.ThuocTinh = dataTable.Rows[i][7].ToString();
                                    DM.DienGiai = "Email: " + dataTable.Rows[i][7].ToString() + " đã tồn tại trong cơ sở dữ liệu";
                                    DM.rowError = i;
                                    lstError.Add(DM);
                                }
                            }
                        }

                        var didong = dataTable.Rows[i][5].ToString().Trim();
                        if (didong != "")
                        {
                            bool isNumber = IsNumberInt(didong);
                            if (isNumber == false /*|| dataTable.Rows[i][8].ToString().Trim().Length > 11*/)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Di động";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = didong;
                                DM.DienGiai = "số điện thoại di động không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                bool checkSDT = classdoituong.TR_CheckSoDienThoai_Exist(didong);
                                if (checkSDT)
                                {
                                    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                    DM.TenTruongDuLieu = "Di động";
                                    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                    DM.ThuocTinh = didong;
                                    DM.DienGiai = "Số điện thoại: " + didong + " đã tồn tại trong cơ sở dữ liệu";
                                    DM.rowError = i;
                                    lstError.Add(DM);
                                }
                            }
                        }
                        var dienthoai = dataTable.Rows[i][6].ToString().Trim();
                        if (dienthoai != "")
                        {
                            bool isNumber = IsNumberInt(dienthoai);
                            if (isNumber == false /*|| dataTable.Rows[i][8].ToString().Trim().Length > 11*/)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Điện thoại";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dienthoai;
                                DM.DienGiai = "Số điện thoại không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            //bool checkSDT = classdoituong.SP_CheckSoDienThoai_Exist(dienthoai);
                            //if (checkSDT)
                            //{
                            //    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            //    DM.TenTruongDuLieu = "Điện thoại";
                            //    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            //    DM.ThuocTinh = dienthoai;
                            //    DM.DienGiai = "Số điện thoại: " + dienthoai + " đã tồn tại trong cơ sở dữ liệu";
                            //    DM.rowError = i;
                            //    lstError.Add(DM);
                            //}
                        }
                        if (dataTable.Rows[i][13].ToString() != "x" && dataTable.Rows[i][13].ToString() != "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tình trạng";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][13].ToString();
                            DM.DienGiai = "Đã nghỉ: bạn cần đánh dấu x";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        //var MaPhongBan = dataTable.Rows[i][14].ToString().Trim();
                        //if (MaPhongBan != "")
                        //{
                        //    bool ExistMaPB = classdoituong.SP_CheckMaPhongBan_Exist(MaPhongBan);
                        //    if (ExistMaPB == false)
                        //    {
                        //        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        //        DM.TenTruongDuLieu = "Mã phòng ban";
                        //        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                        //        DM.ThuocTinh = MaPhongBan;
                        //        DM.DienGiai = "Mã phòng ban: " + MaPhongBan + " không tồn tại trong cơ sở dữ liệu";
                        //        DM.rowError = i;
                        //        lstError.Add(DM);
                        //    }
                        //}
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinNhanVien(dataTable, ID_ChiNhanh);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin hợp đồng
        public List<ErrorDMHangHoa> checkfileThongTinHopDong(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        // kiểm tra tên nhân viên
                        if (dataTable.Rows[i][1].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Số hợp đồng";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Số hợp đồng không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][2].ToString() != "0" && dataTable.Rows[i][2].ToString() != "1" && dataTable.Rows[i][2].ToString() != "2" && dataTable.Rows[i][2].ToString() != "3" && dataTable.Rows[i][2].ToString() != "4")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Loại hợp đồng";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Loại hợp đồng không đúng quy định";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][3].ToString() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][3].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày ký";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                DM.DienGiai = "Ngày ký không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][3]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày Ký";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                        DM.DienGiai = "Ngày ký không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][4].ToString().Trim() != "" && dataTable.Rows[i][5].ToString().Trim() != "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Thời hạn";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][4].ToString().Trim() + " - " + dataTable.Rows[i][5].ToString().Trim();
                            DM.DienGiai = "Thời hạn hợp đồng chỉ được chọn tháng hoặc năm";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            if (dataTable.Rows[i][4].ToString().Trim() != "")
                            {
                                bool isNumber4 = IsNumber(dataTable.Rows[i][4].ToString().Trim());
                                if (isNumber4 == false)
                                {
                                    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                    DM.TenTruongDuLieu = "Thời hạn";
                                    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                    DM.ThuocTinh = dataTable.Rows[i][4].ToString().Trim();
                                    DM.DienGiai = "Thời hạn theo tháng: '" + dataTable.Rows[i][4].ToString().Trim() + "'  không phải dạng số";
                                    DM.rowError = i;
                                    lstError.Add(DM);
                                }
                            }
                            if (dataTable.Rows[i][4].ToString().Trim() != "")
                            {
                                bool isNumber5 = IsNumber(dataTable.Rows[i][5].ToString().Trim());
                                if (isNumber5 == false)
                                {
                                    ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                    DM.TenTruongDuLieu = "Thời hạn";
                                    DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                    DM.ThuocTinh = dataTable.Rows[i][5].ToString().Trim();
                                    DM.DienGiai = "Thời hạn theo năm: '" + dataTable.Rows[i][5].ToString().Trim() + "'  không phải dạng số";
                                    DM.rowError = i;
                                    lstError.Add(DM);
                                }
                            }

                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinHopDong(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin bảo hiểm
        public List<ErrorDMHangHoa> checkfileThongTinBaoHiem(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        // kiểm tra tên nhân viên
                        if (dataTable.Rows[i][1].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Số bảo hiểm";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Số bảo hiểm không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][2].ToString() != "0" && dataTable.Rows[i][2].ToString() != "1" && dataTable.Rows[i][2].ToString() != "2")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Loại bảo hiểm";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Loại bảo hiểm không đúng quy định";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][3].ToString() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][3].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày cấp";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                DM.DienGiai = "Ngày cấp không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][3]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày cấp";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                        DM.DienGiai = "Ngày cấp không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][4].ToString() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][4].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày hết hạn";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                                DM.DienGiai = "Ngày hết hạn không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][3]);
                                    DateTime dateTimeHetHan = Convert.ToDateTime(dataTable.Rows[i][4]);
                                    if (dateTime > dateTimeHetHan)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày hết hạn";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                                        DM.DienGiai = "Ngày hết hạn không được nhỏ hơn ngày cấp";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinBaoHiem(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin khen thưởng
        public List<ErrorDMHangHoa> checkfileThongTinKhenThuong(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][1].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Hình thức";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Hình thức khen thưởng không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][2].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Số quyết định";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Số quyết định không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }

                        if (dataTable.Rows[i][3].ToString() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][3].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày ký";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                DM.DienGiai = "Ngày ký không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][3]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày ký";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                        DM.DienGiai = "Ngày ký không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        else
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Ngày ký";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                            DM.DienGiai = "Ngày ký không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinKhenThuong(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin khoản lương
        public List<ErrorDMHangHoa> checkfileThongTinKhoanLuong(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][1].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Loại lương";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Loại lương không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            var TenLoaiLuong = dataTable.Rows[i][1].ToString().Trim();
                            NS_LoaiLuong NS_LoaiLuong = db.NS_LoaiLuong.Where(x => x.TenLoaiLuong.Contains(TenLoaiLuong)).FirstOrDefault();
                            if (NS_LoaiLuong == null)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Loại hợp đồng";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                DM.DienGiai = "Loại lương: '" + dataTable.Rows[i][1].ToString() + "' không tồn tại trên hệ thống";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }

                        }
                        if (dataTable.Rows[i][2].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Ngày áp dụng";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Ngày áp dụng không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][2].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày áp dụng";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                DM.DienGiai = "Ngày áp dụng không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][2]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày áp dụng";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                        DM.DienGiai = "Ngày áp dụng không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][3].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Ngày kết thúc";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                            DM.DienGiai = "Ngày kết thúc không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][3].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày kết thúc";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                DM.DienGiai = "Ngày kết thúc không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTimeAP = Convert.ToDateTime(dataTable.Rows[i][2]);
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][3]);
                                    if (dateTimeAP > dateTime)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày kết thúc";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                        DM.DienGiai = "Ngày kết thúc không được nhỏ hơn ngày áp dụng";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][4].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Số tiền";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                            DM.DienGiai = "Số tiền không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool isNumber4 = IsNumber(dataTable.Rows[i][4].ToString().Trim());
                            if (isNumber4 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Thời hạn";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][4].ToString().Trim();
                                DM.DienGiai = "Thời hạn theo tháng: '" + dataTable.Rows[i][4].ToString().Trim() + "'  không phải dạng số";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][5].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Hệ số";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][5].ToString();
                            DM.DienGiai = "Hệ số lương không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool isNumber4 = IsNumber(dataTable.Rows[i][5].ToString().Trim());
                            if (isNumber4 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Thời hạn";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][5].ToString().Trim();
                                DM.DienGiai = "Thời hạn theo tháng: '" + dataTable.Rows[i][5].ToString().Trim() + "'  không phải dạng số";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinKhoanLuong(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin miễn giảm thuế
        public List<ErrorDMHangHoa> checkfileThongTinMienGiamThue(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][1].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Khoản miễn giảm";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Khoản miễn giảm không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][2].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Ngày áp dụng";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Ngày áp dụng không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][2].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày áp dụng";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                DM.DienGiai = "Ngày áp dụng không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][2]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày áp dụng";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                        DM.DienGiai = "Ngày áp dụng không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][3].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Ngày kết thúc";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                            DM.DienGiai = "Ngày kết thúc không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][3].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày kết thúc";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                DM.DienGiai = "Ngày kết thúc không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTimeAP = Convert.ToDateTime(dataTable.Rows[i][2]);
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][3]);
                                    if (dateTimeAP > dateTime)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày kết thúc";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                        DM.DienGiai = "Ngày kết thúc không được nhỏ hơn ngày áp dụng";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][4].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Số tiền";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                            DM.DienGiai = "Số tiền không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool isNumber4 = IsNumber(dataTable.Rows[i][4].ToString().Trim());
                            if (isNumber4 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Số tiền";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][4].ToString().Trim();
                                DM.DienGiai = "Số tiền '" + dataTable.Rows[i][4].ToString().Trim() + "'  không phải dạng số";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinMienGiamThue(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin quy trình đào tạo
        public List<ErrorDMHangHoa> checkfileThongTinQuyTrinhDaoTao(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][1].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Từ ngày";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "'Từ ngày' không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][1].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Từ ngày";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                DM.DienGiai = "'Từ ngày' không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][1]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Từ ngày";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                        DM.DienGiai = "Từ ngày không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][2].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][2].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Đến ngày";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                DM.DienGiai = "'Đến ngày' không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTimeAP = Convert.ToDateTime(dataTable.Rows[i][1]);
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][2]);
                                    if (dateTimeAP > dateTime)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày kết thúc";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                        DM.DienGiai = "'Đến ngày' không được nhỏ hơn 'Từ ngày'";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][3].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Nơi học";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                            DM.DienGiai = "Nơi học không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][4].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Ngành học";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                            DM.DienGiai = "Ngành học không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinQuyTrinhDaoTao(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin quá trình công tác
        public List<ErrorDMHangHoa> checkfileThongTinQuaTrinhCongTac(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][1].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Từ ngày";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "'Từ ngày' không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][1].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Từ ngày";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                DM.DienGiai = "'Từ ngày' không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][1]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Từ ngày";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                        DM.DienGiai = "Từ ngày không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][2].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][2].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Đến ngày";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                DM.DienGiai = "'Đến ngày' không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTimeAP = Convert.ToDateTime(dataTable.Rows[i][1]);
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][2]);
                                    if (dateTimeAP > dateTime)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày kết thúc";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                        DM.DienGiai = "'Đến ngày' không được nhỏ hơn 'Từ ngày'";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][3].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Cơ quan";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                            DM.DienGiai = "Cơ quan không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][4].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Vị trí";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                            DM.DienGiai = "Vị trí không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinQuaTrinhCongTac(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin gia đình
        public List<ErrorDMHangHoa> checkfileThongTinGiaDinh(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][1].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Họ tên";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Họ tên không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        if (dataTable.Rows[i][2].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][2].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày sinh";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                DM.DienGiai = "Ngày sinh không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][2]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày sinh";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                                        DM.DienGiai = "Ngày sinh không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][4].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Quan hệ";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                            DM.DienGiai = "Quan hệ không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinGiaDinh(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin chính trị
        public List<ErrorDMHangHoa> checkfileThongTinChinhTri(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][1].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][1].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày vào đoàn";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                DM.DienGiai = "Ngày vào đoàn không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][1]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày vào đoàn";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                        DM.DienGiai = "Ngày vào đoàn không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][3].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][3].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày nhập ngũ";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                DM.DienGiai = "Ngày nhập ngũ không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][3]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày nhập ngũ";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][3].ToString();
                                        DM.DienGiai = "Ngày nhập ngũ không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][4].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][3].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày xuất ngũ";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                                DM.DienGiai = "Ngày xuất ngũ không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][4]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày xuất ngũ";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][4].ToString();
                                        DM.DienGiai = "Ngày xuất ngũ không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][5].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][5].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày vào đảng";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][5].ToString();
                                DM.DienGiai = "Ngày vào đảng không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][5]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày vào đảng";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][5].ToString();
                                        DM.DienGiai = "Ngày vào đảng không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][6].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][6].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày vào đảng chính thức";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][6].ToString();
                                DM.DienGiai = "Ngày vào đảng chính thức không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][6]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày vào đảng chính thức";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][6].ToString();
                                        DM.DienGiai = "Ngày vào đảng chính thức không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][7].ToString().Trim() != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][7].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày rời đảng";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][7].ToString();
                                DM.DienGiai = "Ngày rời đảng không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][7]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày vào đảng";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][7].ToString();
                                        DM.DienGiai = "Ngày rời đảng không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinChinhTri(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import thông tin sức khỏe
        public List<ErrorDMHangHoa> checkfileThongTinSuKhoe(Stream inputFileExcel)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;
                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[1].ColumnName = "MaNhanVien";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var MaNhanVien = dataTable.Rows[i][0].ToString().Trim();
                        // kiểm tra sự tồn tại của mã nhân viên trong database
                        if (MaNhanVien != "")
                        {
                            bool ExistMaNV = classdoituong.SP_CheckMaNhanVien_Exist(MaNhanVien);
                            if (ExistMaNV == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Mã nhân viên";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = MaNhanVien;
                                DM.DienGiai = "Mã nhân viên: " + MaNhanVien + " không tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][1].ToString().Trim() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Ngày khám";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Ngày khám không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        else
                        {
                            bool valiDateNgaySinh = ValidateDateTime(dataTable.Rows[i][1].ToString());
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày khám";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                DM.DienGiai = "Ngày khám không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(dataTable.Rows[i][1]);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày khám";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                                        DM.DienGiai = "Ngày khám không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        if (dataTable.Rows[i][2].ToString().Trim() != "")
                        {
                            bool isNumber4 = IsNumber(dataTable.Rows[i][2].ToString().Trim());
                            if (isNumber4 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Chiều cao";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][2].ToString().Trim();
                                DM.DienGiai = "Chiều cao '" + dataTable.Rows[i][2].ToString().Trim() + "'  không phải dạng số";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                        if (dataTable.Rows[i][3].ToString().Trim() != "")
                        {
                            bool isNumber4 = IsNumber(dataTable.Rows[i][3].ToString().Trim());
                            if (isNumber4 == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Cân nặng";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][3].ToString().Trim();
                                DM.DienGiai = "Cân nặng '" + dataTable.Rows[i][3].ToString().Trim() + "'  không phải dạng số";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importThongTinSucKhoe(dataTable);
                    return null;
                }
            }
        }
        // check lỗi file import khách hàng
        public List<ErrorDMHangHoa> checkfileKhachHang(Stream inputFileExcel, Guid ID_NhanVien, Guid ID_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow - 2;
                int tcool = worksheet.Cells.MaxColumn + 1;

                DataTable dataTable = worksheet.Cells.ExportDataTable(3, 0, trows, tcool);
                // used to check same Data
                dataTable.Columns[3].ColumnName = "MaKhachHang";
                dataTable.Columns[10].ColumnName = "DienThoai";
                dataTable.Columns[9].ColumnName = "Email";

                int chophepTrungSDT = 0;
                var tlap = db.HT_CauHinhPhanMem.Select(x => new { x.ChoPhepTrungSoDienThoai });
                if (tlap != null && tlap.Count() > 0)
                {
                    chophepTrungSDT = tlap.FirstOrDefault().ChoPhepTrungSoDienThoai ?? 0;
                }

                List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    string rowIndex = "Dòng số: " + (i + 4).ToString();
                    DataRow dr = dataTable.Rows[i];
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dr[j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var maDoiTuong = dr[3].ToString().Trim();
                        var tenDT = dr[4].ToString().Trim();
                        var gtinh = dr[5].ToString().Trim();
                        var loaikhach = dr[6].ToString().Trim();
                        var ngaysinh = dr[7].ToString().Trim();
                        var diachi = dr[8].ToString().Trim();
                        var email = dr[9].ToString().Trim();
                        var dienthoai = dr[10].ToString().Trim();
                        var ghichu = dr[11].ToString().Trim();
                        var masothue = dr[12].ToString().Trim();
                        var nocanthu = dr[13].ToString().Trim();
                        var nocantra = dr[14].ToString().Trim();
                        var tongtichdiem = dr[15].ToString().Trim();
                        var soduthe = dr[16].ToString().Trim();

                        bool valiDateMaKH = kiemtrakitu(maDoiTuong);
                        if (valiDateMaKH == false)
                        {
                            lstErr.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Mã khách hàng",
                                ViTri = rowIndex,
                                ThuocTinh = maDoiTuong,
                                DienGiai = "Mã khách hàng không được chứa ký tự đặc biệt",
                                rowError = i
                            });
                        }
                        // kiểm tra trùng lặp mã khách hàng
                        bool duplicateMaKH = GroupData(dataTable, "MaKhachHang = '" + maDoiTuong + "'");
                        if (duplicateMaKH == false)
                        {
                            lstErr.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Mã khách hàng",
                                ViTri = rowIndex,
                                ThuocTinh = maDoiTuong,
                                DienGiai = "Mã khách hàng: " + maDoiTuong + " bị trùng lặp",
                                rowError = i
                            });
                        }
                        // kiểm tra sự tồn tại của mã khách hàng trong database
                        if (maDoiTuong != "")
                        {
                            DM_DoiTuong objDT = classdoituong.Get(x => x.MaDoiTuong == maDoiTuong);
                            if (objDT != null)
                            {
                                lstErr.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Mã khách hàng",
                                    ViTri = rowIndex,
                                    ThuocTinh = maDoiTuong,
                                    DienGiai = "Mã khách hàng: " + maDoiTuong + " đã tồn tại trong cơ sở dữ liệu",
                                    rowError = i
                                });
                            }
                        }

                        // kiểm tra tên khách hàng
                        if (string.IsNullOrEmpty(tenDT))
                        {
                            lstErr.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Tên khách hàng",
                                ViTri = rowIndex,
                                ThuocTinh = tenDT,
                                DienGiai = "Tên khách hàng không được để trống",
                                rowError = i
                            });
                        }

                        if (gtinh != "x" && gtinh != "")
                        {
                            lstErr.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Giới tính",
                                ViTri = rowIndex,
                                ThuocTinh = gtinh,
                                DienGiai = "Giới tính là Nam: bạn cần đánh dấu x",
                                rowError = i
                            });
                        }
                        if (loaikhach != "x" && loaikhach != "")
                        {
                            lstErr.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Loại khách",
                                ViTri = rowIndex,
                                ThuocTinh = loaikhach,
                                DienGiai = "Là công ty: bạn cần đánh dấu x",
                                rowError = i
                            });
                        }
                        if (ngaysinh != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(ngaysinh);
                            if (valiDateNgaySinh == false)
                            {
                                lstErr.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Ngày sinh/thành lập",
                                    ViTri = rowIndex,
                                    ThuocTinh = ngaysinh,
                                    DienGiai = "Ngày sinh/thành lập không hợp lệ",
                                    rowError = i
                                });
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(ngaysinh);
                                    if (dateTime > DateTime.Now)
                                    {
                                        lstErr.Add(new ErrorDMHangHoa
                                        {
                                            TenTruongDuLieu = "Ngày sinh/thành lập",
                                            ViTri = rowIndex,
                                            ThuocTinh = ngaysinh,
                                            DienGiai = "Ngày sinh/thành lập  không được lớn hơn ngày hiện tại",
                                            rowError = i
                                        });
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }

                        if (email != "")
                        {
                            bool valiDateDiaChi = ValidateEmail(email);
                            if (valiDateDiaChi == false)
                            {
                                lstErr.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Email",
                                    ViTri = rowIndex,
                                    ThuocTinh = email,
                                    DienGiai = "Email không hợp lệ",
                                    rowError = i
                                });
                            }

                            bool duplicateEmail = GroupData(dataTable, "Email = '" + email + "'");
                            if (duplicateEmail == false)
                            {
                                lstErr.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Email",
                                    ViTri = rowIndex,
                                    ThuocTinh = email,
                                    DienGiai = "Email bị trùng lặp",
                                    rowError = i
                                });
                            }

                            var existEmail = classdoituong.Get(x => x.Email.Contains(email));
                            if (existEmail != null)
                            {
                                lstErr.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Email",
                                    ViTri = rowIndex,
                                    ThuocTinh = email,
                                    DienGiai = "Email đã tồn tại",
                                    rowError = i
                                });
                            }
                        }

                        if (dienthoai != "")
                        {
                            //bool isNumber = IsNumberInt(dienthoai);
                            //if (isNumber == false)
                            //{
                            //    lstErr.Add(new ErrorDMHangHoa
                            //    {
                            //        TenTruongDuLieu = "Điện thoại",
                            //        ViTri = rowIndex,
                            //        ThuocTinh = dienthoai,
                            //        DienGiai = "Điện thoại không hợp lệ",
                            //        rowError = i
                            //    });
                            //}

                            if (chophepTrungSDT == 0)
                            {
                                bool duplicateSDT = GroupData(dataTable, "DienThoai = '" + dienthoai + "'");
                                if (duplicateSDT == false)
                                {
                                    lstErr.Add(new ErrorDMHangHoa
                                    {
                                        TenTruongDuLieu = "Điện thoại",
                                        ViTri = rowIndex,
                                        ThuocTinh = dienthoai,
                                        DienGiai = "Số điện thoại: " + dienthoai + " bị trùng lặp",
                                        rowError = i
                                    });
                                }
                            }

                            bool checkSDT = classdoituong.SP_CheckSoDienThoai_Exist(dienthoai);
                            if (checkSDT)
                            {
                                lstErr.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Điện thoại",
                                    ViTri = rowIndex,
                                    ThuocTinh = dienthoai,
                                    DienGiai = "Số điện thoại: " + dienthoai + " đã tồn tại trong cơ sở dữ liệu",
                                    rowError = i
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(nocanthu))
                        {
                            bool isNumber = IsNumberInt(nocanthu);
                            if (!isNumber)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Nợ cần thu",
                                    ViTri = rowIndex,
                                    ThuocTinh = nocanthu,
                                    DienGiai = "Nợ cần thu không phải dạng số",
                                    rowError = i,
                                };
                                lstErr.Add(itemErr);
                            }
                        }

                        if (!string.IsNullOrEmpty(nocantra))
                        {
                            bool isNumber = IsNumberInt(nocantra);
                            if (!isNumber)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Nợ cần trả",
                                    ViTri = rowIndex,
                                    ThuocTinh = nocantra,
                                    DienGiai = "Nợ cần trả không phải dạng số",
                                    rowError = i,
                                };
                                lstErr.Add(itemErr);
                            }
                        }

                        if (!string.IsNullOrEmpty(tongtichdiem))
                        {
                            bool isNumber = IsNumberInt(tongtichdiem);
                            if (!isNumber)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Tổng tích điểm",
                                    ViTri = rowIndex,
                                    ThuocTinh = tongtichdiem,
                                    DienGiai = "Tổng tích điểm không phải dạng số",
                                    rowError = i,
                                };
                                lstErr.Add(itemErr);
                            }
                        }

                        if (!string.IsNullOrEmpty(soduthe))
                        {
                            bool isNumber = IsNumberInt(soduthe);
                            if (!isNumber)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Số dư thẻ",
                                    ViTri = rowIndex,
                                    ThuocTinh = soduthe,
                                    DienGiai = "Số dư thẻ không phải dạng số",
                                    rowError = i,
                                };
                                lstErr.Add(itemErr);
                            }
                        }
                    }
                }
                if (lstErr.Count > 0)
                {
                    return lstErr;
                }
                else
                {
                    importKhachHang(dataTable, ID_NhanVien, ID_DonVi);
                    return null;
                }
            }
        }
        //check nhóm đối tượng
        public DM_NhomDoiTuong selectNhomDoiTuong(string tennhomdoituong)
        {
            DM_NhomDoiTuong dM_NhomDoiTuong = db.DM_NhomDoiTuong.Where(p => p.TenNhomDoiTuong == tennhomdoituong).FirstOrDefault();
            return dM_NhomDoiTuong;
        }
        public void importKhachHang1(DataTable dataTable)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                ClassDM_NhomDoiTuong classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        Guid? ID_NhomDoiTuong = null;
                        if (dataTable.Rows[i][0].ToString() != "")
                        {
                            DM_NhomDoiTuong dM_NhomDoiTuong = selectNhomDoiTuong(dataTable.Rows[i][0].ToString());
                            if (dM_NhomDoiTuong != null)
                            {
                                ID_NhomDoiTuong = dM_NhomDoiTuong.ID;
                            }
                            else // thêm mới
                            {
                                string sMaNhom = string.Empty;
                                Random rd = new Random();
                                sMaNhom = rd.Next().ToString();
                                #region DM_NhomDoiTuong
                                DM_NhomDoiTuong DM_NhomDoiTuong = new DM_NhomDoiTuong();
                                DM_NhomDoiTuong.ID = Guid.NewGuid();
                                DM_NhomDoiTuong.MaNhomDoiTuong = sMaNhom;
                                DM_NhomDoiTuong.TenNhomDoiTuong = dataTable.Rows[i][0].ToString();
                                DM_NhomDoiTuong.LoaiDoiTuong = 1;
                                #endregion
                                string strIns = classNhomDoiTuong.Add_NhomDoiTuong(DM_NhomDoiTuong);
                                ID_NhomDoiTuong = DM_NhomDoiTuong.ID;
                            }
                        }
                        string sMaDoiTuong = string.Empty;
                        if (dataTable.Rows[i][1].ToString() != "")
                        {
                            sMaDoiTuong = dataTable.Rows[i][1].ToString().ToUpper();
                        }
                        else
                        {
                            //mã khach hang tự động
                            sMaDoiTuong = classdoituong.SP_GetautoCode(1);
                        }

                        #region DM_DoiTuong
                        DM_DoiTuong DM_DoiTuong = new DM_DoiTuong { };
                        DM_DoiTuong.ID = Guid.NewGuid();
                        DM_DoiTuong.MaDoiTuong = sMaDoiTuong;
                        DM_DoiTuong.TenDoiTuong = dataTable.Rows[i][2].ToString();
                        DM_DoiTuong.TenDoiTuong_KhongDau = CommonStatic.ConvertToUnSign(dataTable.Rows[i][2].ToString()).ToLower();
                        DM_DoiTuong.TenDoiTuong_ChuCaiDau = CommonStatic.GetCharsStart(dataTable.Rows[i][2].ToString()).ToLower();
                        DM_DoiTuong.GioiTinhNam = dataTable.Rows[i][3].ToString() == "" ? false : true;
                        DM_DoiTuong.LoaiDoiTuong = 1;
                        if (dataTable.Rows[i][5].ToString() != "")
                        {
                            DM_DoiTuong.NgaySinh_NgayTLap = Convert.ToDateTime(dataTable.Rows[i][5].ToString());
                        }
                        DM_DoiTuong.NgayTao = DateTime.Now;
                        DM_DoiTuong.NguoiTao = "admin";
                        DM_DoiTuong.DiaChi = dataTable.Rows[i][6].ToString();
                        DM_DoiTuong.Email = dataTable.Rows[i][7].ToString();
                        DM_DoiTuong.GhiChu = dataTable.Rows[i][9].ToString();
                        DM_DoiTuong.DienThoai = dataTable.Rows[i][8].ToString();
                        DM_DoiTuong.ID_NhomDoiTuong = ID_NhomDoiTuong;
                        DM_DoiTuong.MaSoThue = dataTable.Rows[i][10].ToString();
                        DM_DoiTuong.TenNhomDT = dataTable.Rows[i][0].ToString();
                        DM_DoiTuong.LaCaNhan = dataTable.Rows[i][4].ToString() == "" ? true : false;
                        #endregion
                        string strInsDoiTuong = classdoituong.Add_DoiTuong(DM_DoiTuong);

                    }
                }
            }
        }
        // import danh sách Nhân viên todo
        public void importDanhSachNhanVien(DataTable dataTable, Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                var numberMax = 0;
                var chuoi0 = string.Empty;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {

                        string sMaNhanVien = string.Empty;
                        if (dataTable.Rows[i][0].ToString().Trim() != "")
                            sMaNhanVien = dataTable.Rows[i][0].ToString().ToUpper();
                        else
                        {
                            SqlParameter sql = new SqlParameter("MaNhanVien", "NV00001");
                            sMaNhanVien = db.Database.SqlQuery<string>("exec get_MaNhanVien @MaNhanVien", sql).FirstOrDefault().Trim();
                            if (numberMax != 0)
                            {
                                numberMax = numberMax + 1;
                            }
                            else
                            {
                                numberMax = int.Parse(Regex.Match(sMaNhanVien, @"\d+").Value);
                            }
                            switch (numberMax.ToString().Length)
                            {
                                case 1:
                                    chuoi0 = "0000";
                                    break;
                                case 2:
                                    chuoi0 = "000";
                                    break;
                                case 3:
                                    chuoi0 = "00";
                                    break;
                                case 4:
                                    chuoi0 = "0";
                                    break;
                            }
                            sMaNhanVien = string.Concat("NV", chuoi0, numberMax);
                        }
                        string TenNhanVien_KhongDau = CommonStatic.ConvertToUnSign(dataTable.Rows[i][1].ToString()).ToLower();
                        string TenNhanVien_KyTuDau = CommonStatic.GetCharsStart(dataTable.Rows[i][1].ToString()).ToLower();
                        bool GioiTinh = dataTable.Rows[i][2].ToString() == "" ? false : true;
                        bool TrangThai = dataTable.Rows[i][10].ToString() == "" ? false : true;
                        DateTime NgaySinh;
                        string NgaySinhTL = string.Empty;
                        try
                        {
                            NgaySinh = Convert.ToDateTime(dataTable.Rows[i][3].ToString().Trim());
                            NgaySinhTL = NgaySinh.ToString("MM/dd/yyyy");
                        }
                        catch
                        {
                            try
                            {
                                NgaySinh = Convert.ToDateTime("01/01/" + dataTable.Rows[i][3].ToString().Trim());
                                NgaySinhTL = NgaySinh.ToString("MM/dd/yyyy");
                            }
                            catch
                            {

                            }
                        }
                        List<SqlParameter> sqlparamt = new List<SqlParameter>();
                        sqlparamt.Add(new SqlParameter("MaNhanVien", sMaNhanVien));
                        sqlparamt.Add(new SqlParameter("TenNhanVien", dataTable.Rows[i][1].ToString()));
                        sqlparamt.Add(new SqlParameter("TenNhanVienKhongDau", TenNhanVien_KhongDau));
                        sqlparamt.Add(new SqlParameter("TenNhanVienKyTuDau", TenNhanVien_KyTuDau));
                        sqlparamt.Add(new SqlParameter("GioiTinh", GioiTinh));
                        sqlparamt.Add(new SqlParameter("NgaySinh", NgaySinhTL));
                        sqlparamt.Add(new SqlParameter("DienThoai", dataTable.Rows[i][4].ToString()));
                        sqlparamt.Add(new SqlParameter("Email", dataTable.Rows[i][5].ToString()));
                        sqlparamt.Add(new SqlParameter("NoiSinh", dataTable.Rows[i][6].ToString()));
                        sqlparamt.Add(new SqlParameter("CMND", dataTable.Rows[i][7].ToString()));
                        sqlparamt.Add(new SqlParameter("SoBaoHiem", dataTable.Rows[i][8].ToString()));
                        sqlparamt.Add(new SqlParameter("GhiChu", dataTable.Rows[i][9].ToString()));
                        sqlparamt.Add(new SqlParameter("TrangThai", TrangThai));
                        sqlparamt.Add(new SqlParameter("ID_DonVi", idDonVi));
                        db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_DanhSach @MaNhanVien, @TenNhanVien,@TenNhanVienKhongDau, @TenNhanVienKyTuDau,@GioiTinh, @NgaySinh," +
                        " @DienThoai, @Email, @NoiSinh, @CMND, @SoBaoHiem, @GhiChu, @TrangThai, @ID_DonVi", sqlparamt.ToArray());
                    }
                }
            }
        }
        public void IgnoreErrorDanhSachNhanVien(Stream inputfile, string RowsError, Guid idDonVi)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importDanhSachNhanVien_WithError(dataTable, RowsError, idDonVi);
        }
        public void importDanhSachNhanVien_WithError(DataTable dataTable, string rowsError, Guid idDonVi)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importDanhSachNhanVien(dataTable, idDonVi);
        }
        // import ca làm việc
        public void importThongTinCaLamViec(DataTable dataTable, string NguoiTao)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    string CaLamViec_KhongDau = CommonStatic.ConvertToUnSign(dataTable.Rows[i][1].ToString().Trim()).ToLower();
                    string CaLamViec_ChuCaiDau = CommonStatic.convertchartstart(dataTable.Rows[i][1].ToString().Trim()).ToLower();
                    string sMaCaLamViec = dataTable.Rows[i][0].ToString().Trim();
                    if (sMaCaLamViec == null || sMaCaLamViec == "")
                    {
                        List<SqlParameter> sql = new List<SqlParameter>();
                        sql.Add(new SqlParameter("MaNhanSu", "CA00001"));
                        sql.Add(new SqlParameter("LoaiMa", "1"));
                        sMaCaLamViec = db.Database.SqlQuery<string>("exec get_MaNhanSu @MaNhanSu, @LoaiMa", sql.ToArray()).FirstOrDefault().Trim();
                    }
                    int LaCaDem = 0;
                    if (dataTable.Rows[i][12].ToString().Trim() == "x")
                    {
                        LaCaDem = 1;
                    }
                    string[] ar_GioVao = dataTable.Rows[i][3].ToString().Trim().Split(':');
                    DateTime GioVao = new DateTime(2000, 01, 01, int.Parse(ar_GioVao[0]), int.Parse(ar_GioVao[1]), 00);
                    string[] ar_GioRa = dataTable.Rows[i][4].ToString().Trim().Split(':');
                    DateTime GioRa = new DateTime(2000, 01, 01, int.Parse(ar_GioRa[0]), int.Parse(ar_GioRa[1]), 00);
                    double TongGioCong = 0;
                    if (LaCaDem == 0 || GioRa > GioVao)
                    {
                        TongGioCong = (int.Parse(ar_GioRa[0]) - int.Parse(ar_GioVao[0])) * 60 + int.Parse(ar_GioRa[1]) - int.Parse(ar_GioVao[1]);
                    }
                    else if (GioRa < GioVao)
                    {
                        int TC_Truoc = (24 - int.Parse(ar_GioVao[0])) * 60 - int.Parse(ar_GioVao[1]);
                        int TC_Sau = int.Parse(ar_GioRa[0]) * 60 + int.Parse(ar_GioRa[1]);
                        TongGioCong = TC_Truoc + TC_Sau;
                    }

                    //set thời gian tạo phiếu
                    DateTime? NghiGiuaCaTu = null;
                    if (dataTable.Rows[i][5].ToString().Trim() != "")
                    {
                        string[] ar_NghiGiuaCaTu = dataTable.Rows[i][5].ToString().Trim().Split(':');
                        NghiGiuaCaTu = new DateTime(2000, 01, 01, int.Parse(ar_NghiGiuaCaTu[0]), int.Parse(ar_NghiGiuaCaTu[1]), 00);
                    }
                    DateTime? NghiGiuaCaDen = null;
                    if (dataTable.Rows[i][6].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][6].ToString().Trim().Split(':');
                        NghiGiuaCaDen = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    if (dataTable.Rows[i][5].ToString().Trim() != "" && dataTable.Rows[i][6].ToString().Trim() != "")
                    {
                        string[] ar_NghiGiuaCaTu = dataTable.Rows[i][5].ToString().Trim().Split(':');
                        string[] ar_NghiGiuaCaDen = dataTable.Rows[i][6].ToString().Trim().Split(':');
                        if (LaCaDem == 0 || GioRa > GioVao)
                        {
                            TongGioCong = TongGioCong - ((int.Parse(ar_NghiGiuaCaDen[0]) - int.Parse(ar_NghiGiuaCaTu[0])) * 60 + int.Parse(ar_NghiGiuaCaDen[1]) - int.Parse(ar_NghiGiuaCaTu[1]));
                        }
                        else if (GioRa < GioVao)
                        {
                            if (NghiGiuaCaDen > NghiGiuaCaTu)
                            {
                                TongGioCong = TongGioCong - ((int.Parse(ar_NghiGiuaCaDen[0]) - int.Parse(ar_NghiGiuaCaTu[0])) * 60 + int.Parse(ar_NghiGiuaCaDen[1]) - int.Parse(ar_NghiGiuaCaTu[1]));
                            }
                            else
                            {
                                int Nghi_Truoc = (24 - int.Parse(ar_NghiGiuaCaTu[0])) * 60 - int.Parse(ar_NghiGiuaCaTu[1]);
                                int Nghi_Sau = int.Parse(ar_NghiGiuaCaDen[0]) * 60 + int.Parse(ar_NghiGiuaCaDen[1]);
                                TongGioCong = TongGioCong - (Nghi_Truoc + Nghi_Sau);
                            }
                        }
                    }
                    DateTime? GioOTBanNgayTu = null;
                    if (dataTable.Rows[i][7].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][7].ToString().Trim().Split(':');
                        GioOTBanNgayTu = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? GioOTBanNgayDen = null;
                    if (dataTable.Rows[i][8].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][8].ToString().Trim().Split(':');
                        GioOTBanNgayDen = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? GioOTBanDemTu = null;
                    if (dataTable.Rows[i][9].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][9].ToString().Trim().Split(':');
                        GioOTBanDemTu = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? GioOTBanDemDen = null;
                    if (dataTable.Rows[i][10].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][10].ToString().Trim().Split(':');
                        GioOTBanDemDen = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    int? SoPhutDiMuon = null;
                    if (dataTable.Rows[i][14].ToString().Trim() != "")
                    {
                        SoPhutDiMuon = int.Parse(dataTable.Rows[i][14].ToString().Trim());
                    }
                    int? SoPhutVeSom = null;
                    if (dataTable.Rows[i][15].ToString().Trim() != "")
                    {
                        SoPhutVeSom = int.Parse(dataTable.Rows[i][15].ToString().Trim());
                    }
                    DateTime? GioVaoTu = null;
                    if (dataTable.Rows[i][16].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][16].ToString().Trim().Split(':');
                        GioVaoTu = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? GioVaoDen = null;
                    if (dataTable.Rows[i][17].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][17].ToString().Trim().Split(':');
                        GioVaoDen = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? GioRaTu = null;
                    if (dataTable.Rows[i][18].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][18].ToString().Trim().Split(':');
                        GioRaTu = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? GioRaDen = null;
                    if (dataTable.Rows[i][19].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][19].ToString().Trim().Split(':');
                        GioRaDen = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? TinhOTBanNgayTu = null;
                    if (dataTable.Rows[i][20].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][20].ToString().Trim().Split(':');
                        TinhOTBanNgayTu = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? TinhOTBanNgayDen = null;
                    if (dataTable.Rows[i][21].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][21].ToString().Trim().Split(':');
                        TinhOTBanNgayDen = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? TinhOTBanDemTu = null;
                    if (dataTable.Rows[i][22].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][22].ToString().Trim().Split(':');
                        TinhOTBanDemTu = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }
                    DateTime? TinhOTBanDemDen = null;
                    if (dataTable.Rows[i][23].ToString().Trim() != "")
                    {
                        string[] ar_time = dataTable.Rows[i][23].ToString().Trim().Split(':');
                        TinhOTBanDemDen = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                    }

                    double SoGioOTToiThieu = 0;
                    if (dataTable.Rows[i][24].ToString().Trim() != "")
                    {
                        SoGioOTToiThieu = double.Parse(dataTable.Rows[i][24].ToString().Trim());
                    }
                    TongGioCong = Math.Round(TongGioCong / 60, 1, MidpointRounding.ToEven);
                    List<SqlParameter> sqlPRM = new List<SqlParameter>();
                    sqlPRM.Add(new SqlParameter("MaCa", sMaCaLamViec));
                    sqlPRM.Add(new SqlParameter("TenCa", dataTable.Rows[i][1].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("CaLamViec_KhongDau", CaLamViec_KhongDau));
                    sqlPRM.Add(new SqlParameter("CaLamViec_ChuCaiDau", CaLamViec_ChuCaiDau));
                    sqlPRM.Add(new SqlParameter("GioVao", GioVao));
                    sqlPRM.Add(new SqlParameter("GioRa", GioRa));
                    sqlPRM.Add(new SqlParameter("TongGioCong", TongGioCong));
                    if (NghiGiuaCaTu == null)
                        sqlPRM.Add(new SqlParameter("NghiGiuaCaTu", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("NghiGiuaCaTu", NghiGiuaCaTu));
                    if (NghiGiuaCaDen == null)
                        sqlPRM.Add(new SqlParameter("NghiGiuaCaDen", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("NghiGiuaCaDen", NghiGiuaCaDen));
                    if (GioOTBanNgayTu == null)
                        sqlPRM.Add(new SqlParameter("GioOTBanNgayTu", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("GioOTBanNgayTu", GioOTBanNgayTu));
                    if (GioOTBanNgayDen == null)
                        sqlPRM.Add(new SqlParameter("GioOTBanNgayDen", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("GioOTBanNgayDen", GioOTBanNgayDen));
                    if (GioOTBanDemTu == null)
                        sqlPRM.Add(new SqlParameter("GioOTBanDemTu", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("GioOTBanDemTu", GioOTBanDemTu));
                    if (GioOTBanDemDen == null)
                        sqlPRM.Add(new SqlParameter("GioOTBanDemDen", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("GioOTBanDemDen", GioOTBanDemDen));
                    if (SoPhutDiMuon == null)
                        sqlPRM.Add(new SqlParameter("SoPhutDiMuon", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("SoPhutDiMuon", SoPhutDiMuon));
                    if (SoPhutVeSom == null)
                        sqlPRM.Add(new SqlParameter("SoPhutVeSom", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("SoPhutVeSom", SoPhutVeSom));
                    if (GioVaoTu == null)
                        sqlPRM.Add(new SqlParameter("GioVaoTu", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("GioVaoTu", GioVaoTu));
                    if (GioVaoDen == null)
                        sqlPRM.Add(new SqlParameter("GioVaoDen", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("GioVaoDen", GioVaoDen));
                    if (GioRaTu == null)
                        sqlPRM.Add(new SqlParameter("GioRaTu", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("GioRaTu", GioRaTu));
                    if (GioRaDen == null)
                        sqlPRM.Add(new SqlParameter("GioRaDen", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("GioRaDen", GioRaDen));
                    if (TinhOTBanNgayTu == null)
                        sqlPRM.Add(new SqlParameter("TinhOTBanNgayTu", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("TinhOTBanNgayTu", TinhOTBanNgayTu));
                    if (TinhOTBanNgayDen == null)
                        sqlPRM.Add(new SqlParameter("TinhOTBanNgayDen", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("TinhOTBanNgayDen", TinhOTBanNgayDen));
                    if (TinhOTBanDemTu == null)
                        sqlPRM.Add(new SqlParameter("TinhOTBanDemTu", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("TinhOTBanDemTu", TinhOTBanDemTu));
                    if (TinhOTBanDemDen == null)
                        sqlPRM.Add(new SqlParameter("TinhOTBanDemDen", DBNull.Value));
                    else
                        sqlPRM.Add(new SqlParameter("TinhOTBanDemDen", TinhOTBanDemDen));
                    sqlPRM.Add(new SqlParameter("LaCaDem", LaCaDem));
                    sqlPRM.Add(new SqlParameter("CachLayGioCong", dataTable.Rows[i][11].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("SoGioOTToiThieu", SoGioOTToiThieu));
                    sqlPRM.Add(new SqlParameter("GhiChuCaLamVec", dataTable.Rows[i][13].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("GhiChuTinhGio", dataTable.Rows[i][25].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("TrangThai", dataTable.Rows[i][2].ToString().Trim()));
                    sqlPRM.Add(new SqlParameter("NguoiTao", NguoiTao));
                    db.Database.ExecuteSqlCommand("exec insert_CaLamViec @MaCa, @TenCa, @CaLamViec_KhongDau, @CaLamViec_ChuCaiDau, @GioVao, @GioRa, @TongGioCong, @NghiGiuaCaTu, @NghiGiuaCaDen, @GioOTBanNgayTu, @GioOTBanNgayDen," +
                        "@GioOTBanDemTu, @GioOTBanDemDen, @SoPhutDiMuon, @SoPhutVeSom, @GioVaoTu, @GioVaoDen, @GioRaTu, @GioRaDen, @TinhOTBanNgayTu, @TinhOTBanNgayDen, " +
                        "@TinhOTBanDemTu, @TinhOTBanDemDen, @LaCaDem, @CachLayGioCong, @SoGioOTToiThieu, @GhiChuCaLamVec, @GhiChuTinhGio, @TrangThai, @NguoiTao", sqlPRM.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinCaLamViec(Stream inputfile, string RowsError, string NguoiTao)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(3, 0, trows, tcool);
            importThongTinCaLamViec_WithError(dataTable, RowsError, NguoiTao);
        }
        public void importThongTinCaLamViec_WithError(DataTable dataTable, string rowsError, string NguoiTao)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinCaLamViec(dataTable, NguoiTao);
        }
        // import Nhân viên
        public void importThongTinNhanVien(DataTable dataTable, Guid ID_ChiNhanh)
        {
            ClassNS_NhanVien classNhanVien = new ClassNS_NhanVien(db);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    string sMaNhanVien = string.Empty;
                    if (dataTable.Rows[i][0].ToString().Trim() != "")
                        sMaNhanVien = dataTable.Rows[i][0].ToString().ToUpper();
                    else
                    {
                        sMaNhanVien = classNhanVien.GetMaNhanVien_TheoSubDomain();
                    }
                    string TenNhanVien_KhongDau = CommonStatic.ConvertToUnSign(dataTable.Rows[i][1].ToString()).ToLower();
                    string TenNhanVien_KyTuDau = CommonStatic.GetCharsStart(dataTable.Rows[i][1].ToString()).ToLower();
                    bool GioiTinh = dataTable.Rows[i][2].ToString() == "" ? false : true;
                    bool HonNhan = dataTable.Rows[i][3].ToString() == "" ? false : true;
                    bool TrangThai = dataTable.Rows[i][13].ToString() == "" ? false : true;
                    DateTime NgaySinh;
                    string NgaySinhTL = string.Empty;
                    DateTime NgayCap;
                    string NgayCapCMND = string.Empty;
                    try
                    {
                        NgaySinh = Convert.ToDateTime(dataTable.Rows[i][4].ToString().Trim());
                        NgaySinhTL = NgaySinh.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgaySinh = Convert.ToDateTime("01/01/" + dataTable.Rows[i][4].ToString().Trim());
                            NgaySinhTL = NgaySinh.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    try
                    {
                        NgayCap = Convert.ToDateTime(dataTable.Rows[i][10].ToString().Trim());
                        NgayCapCMND = NgayCap.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayCap = Convert.ToDateTime("01/01/" + dataTable.Rows[i][10].ToString().Trim());
                            NgayCapCMND = NgayCap.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", sMaNhanVien));
                    sqlparamt.Add(new SqlParameter("TenNhanVien", dataTable.Rows[i][1].ToString()));
                    sqlparamt.Add(new SqlParameter("TenNhanVienKhongDau", TenNhanVien_KhongDau));
                    sqlparamt.Add(new SqlParameter("TenNhanVienKyTuDau", TenNhanVien_KyTuDau));
                    sqlparamt.Add(new SqlParameter("GioiTinh", GioiTinh));
                    sqlparamt.Add(new SqlParameter("HonNhan", HonNhan));
                    sqlparamt.Add(new SqlParameter("NgaySinh", NgaySinhTL));
                    sqlparamt.Add(new SqlParameter("DiDong", dataTable.Rows[i][5].ToString()));
                    sqlparamt.Add(new SqlParameter("DienThoai", dataTable.Rows[i][6].ToString()));
                    sqlparamt.Add(new SqlParameter("Email", dataTable.Rows[i][7].ToString()));
                    sqlparamt.Add(new SqlParameter("NoiSinh", dataTable.Rows[i][8].ToString()));
                    sqlparamt.Add(new SqlParameter("CMND", dataTable.Rows[i][9].ToString()));
                    sqlparamt.Add(new SqlParameter("NgayCapCMND", NgayCapCMND));
                    sqlparamt.Add(new SqlParameter("NoiCapCMND", dataTable.Rows[i][11].ToString()));
                    sqlparamt.Add(new SqlParameter("GhiChu", dataTable.Rows[i][12].ToString()));
                    sqlparamt.Add(new SqlParameter("ID_DonVi", ID_ChiNhanh));
                    sqlparamt.Add(new SqlParameter("TrangThai", TrangThai));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinCoBan @MaNhanVien, @TenNhanVien,@TenNhanVienKhongDau, @TenNhanVienKyTuDau,@GioiTinh, @HonNhan, @NgaySinh, @DiDong," +
                    " @DienThoai, @Email, @NoiSinh, @CMND, @NgayCapCMND, @NoiCapCMND, @GhiChu, @ID_DonVi, @TrangThai", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinNhanVien(Stream inputfile, string RowsError, Guid ID_ChiNhanh)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinNhanVien_WithError(dataTable, RowsError, ID_ChiNhanh);
        }
        public void importThongTinNhanVien_WithError(DataTable dataTable, string rowsError, Guid ID_ChiNhanh)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinNhanVien(dataTable, ID_ChiNhanh);
        }
        // import hợp đồng
        public void importThongTinHopDong(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {

                    DateTime NgayKy;
                    string NgayKyTL = string.Empty;
                    try
                    {
                        NgayKy = Convert.ToDateTime(dataTable.Rows[i][3].ToString().Trim());
                        NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKy = Convert.ToDateTime("01/01/" + dataTable.Rows[i][3].ToString().Trim());
                            NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    float ThoiHan = 0;
                    bool DonViThoiGian = false;
                    if (dataTable.Rows[i][4].ToString().Trim() != "")
                    {
                        ThoiHan = float.Parse(dataTable.Rows[i][4].ToString().Trim());
                        DonViThoiGian = false;
                    }
                    if (dataTable.Rows[i][5].ToString().Trim() != "")
                    {
                        ThoiHan = float.Parse(dataTable.Rows[i][5].ToString().Trim());
                        DonViThoiGian = true;
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("SoHopDong", dataTable.Rows[i][1].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("LoaiHopDong", dataTable.Rows[i][2].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NgayKy", NgayKyTL));
                    sqlparamt.Add(new SqlParameter("GhiChu", dataTable.Rows[i][6].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("ThoiHan", ThoiHan));
                    sqlparamt.Add(new SqlParameter("DonViThoiHan", DonViThoiGian));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinHopDong @MaNhanVien, @SoHopDong, @LoaiHopDong, @NgayKy, @GhiChu, @ThoiHan, @DonViThoiHan", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinHopDong(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinHopDong_WithError(dataTable, RowsError);
        }
        public void importThongTinHopDong_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinHopDong(dataTable);
        }
        // import bảo hiểm
        public void importThongTinBaoHiem(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {

                    DateTime NgayKy;
                    string NgayKyTL = string.Empty;
                    try
                    {
                        NgayKy = Convert.ToDateTime(dataTable.Rows[i][3].ToString().Trim());
                        NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKy = Convert.ToDateTime("01/01/" + dataTable.Rows[i][3].ToString().Trim());
                            NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayHetHan;
                    string NgayHetHanTL = string.Empty;
                    try
                    {
                        NgayHetHan = Convert.ToDateTime(dataTable.Rows[i][4].ToString().Trim());
                        NgayHetHanTL = NgayHetHan.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayHetHan = Convert.ToDateTime("01/01/" + dataTable.Rows[i][4].ToString().Trim());
                            NgayHetHanTL = NgayHetHan.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("SoBaoHiem", dataTable.Rows[i][1].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("LoaiBaoHiem", dataTable.Rows[i][2].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NgayCap", NgayKyTL));
                    sqlparamt.Add(new SqlParameter("NgayHetHan", NgayHetHanTL));
                    sqlparamt.Add(new SqlParameter("NoiCap", dataTable.Rows[i][5].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("GhiChu", dataTable.Rows[i][6].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinBaoHiem @MaNhanVien, @SoBaoHiem, @LoaiBaoHiem, @NgayCap, @NgayHetHan, @NoiCap, @GhiChu", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinBaoHiem(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinBaoHiem_WithError(dataTable, RowsError);
        }
        public void importThongTinBaoHiem_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinBaoHiem(dataTable);
        }
        // import khen thưởng
        public void importThongTinKhenThuong(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {

                    DateTime NgayKy;
                    string NgayKyTL = string.Empty;
                    try
                    {
                        NgayKy = Convert.ToDateTime(dataTable.Rows[i][3].ToString().Trim());
                        NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKy = Convert.ToDateTime("01/01/" + dataTable.Rows[i][3].ToString().Trim());
                            NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("HinhThuc", dataTable.Rows[i][1].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("SoQuyetDinh", dataTable.Rows[i][2].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NgayKy", NgayKyTL));
                    sqlparamt.Add(new SqlParameter("NoiDung", dataTable.Rows[i][4].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("GhiChu", dataTable.Rows[i][5].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinKhenThuong @MaNhanVien, @HinhThuc, @SoQuyetDinh, @NgayKy, @NoiDung, @GhiChu", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinKhenThuong(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinKhenThuong_WithError(dataTable, RowsError);
        }
        public void importThongTinKhenThuong_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinKhenThuong(dataTable);
        }
        // import khoản lương
        public void importThongTinKhoanLuong(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {

                    DateTime NgayKy;
                    string NgayKyTL = string.Empty;
                    try
                    {
                        NgayKy = Convert.ToDateTime(dataTable.Rows[i][2].ToString().Trim());
                        NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKy = Convert.ToDateTime("01/01/" + dataTable.Rows[i][3].ToString().Trim());
                            NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayKetThuc;
                    string NgayKetThucTL = string.Empty;
                    try
                    {
                        NgayKetThuc = Convert.ToDateTime(dataTable.Rows[i][3].ToString().Trim());
                        NgayKetThucTL = NgayKetThuc.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKetThuc = Convert.ToDateTime("01/01/" + dataTable.Rows[i][3].ToString().Trim());
                            NgayKetThucTL = NgayKetThuc.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("MaLoaiLuong", dataTable.Rows[i][1].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NgayApDung", NgayKyTL));
                    sqlparamt.Add(new SqlParameter("NgayKetThuc", NgayKetThucTL));
                    sqlparamt.Add(new SqlParameter("SoTien", dataTable.Rows[i][4].ToString().Trim().Replace(",", ".")));
                    sqlparamt.Add(new SqlParameter("HeSo", dataTable.Rows[i][5].ToString().Trim().Replace(",", ".")));
                    sqlparamt.Add(new SqlParameter("Bac", dataTable.Rows[i][6].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NoiDung", dataTable.Rows[i][7].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinKhoanLuong @MaNhanVien, @MaLoaiLuong, @NgayApDung, @NgayKetThuc, @SoTien, @HeSo, @Bac, @NoiDung", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinKhoanLuong(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinKhoanLuong_WithError(dataTable, RowsError);
        }
        public void importThongTinKhoanLuong_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinKhoanLuong(dataTable);
        }
        // import miễn giảm thuế
        public void importThongTinMienGiamThue(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {

                    DateTime NgayKy;
                    string NgayKyTL = string.Empty;
                    try
                    {
                        NgayKy = Convert.ToDateTime(dataTable.Rows[i][2].ToString().Trim());
                        NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKy = Convert.ToDateTime("01/01/" + dataTable.Rows[i][2].ToString().Trim());
                            NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayKetThuc;
                    string NgayKetThucTL = string.Empty;
                    try
                    {
                        NgayKetThuc = Convert.ToDateTime(dataTable.Rows[i][3].ToString().Trim());
                        NgayKetThucTL = NgayKetThuc.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKetThuc = Convert.ToDateTime("01/01/" + dataTable.Rows[i][3].ToString().Trim());
                            NgayKetThucTL = NgayKetThuc.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("KhoanMienGiam", dataTable.Rows[i][1].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NgayApDung", NgayKyTL));
                    sqlparamt.Add(new SqlParameter("NgayKetThuc", NgayKetThucTL));
                    sqlparamt.Add(new SqlParameter("SoTien", dataTable.Rows[i][4].ToString().Trim().Replace(",", ".")));
                    sqlparamt.Add(new SqlParameter("NoiDung", dataTable.Rows[i][5].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinMienGiamThue @MaNhanVien, @KhoanMienGiam, @NgayApDung, @NgayKetThuc, @SoTien, @NoiDung", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinMienGiamThue(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinMienGiamThue_WithError(dataTable, RowsError);
        }
        public void importThongTinMienGiamThue_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinMienGiamThue(dataTable);
        }
        // import quy trình đào tạo
        public void importThongTinQuyTrinhDaoTao(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {

                    DateTime NgayKy;
                    string NgayKyTL = string.Empty;
                    try
                    {
                        NgayKy = Convert.ToDateTime(dataTable.Rows[i][1].ToString().Trim());
                        NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKy = Convert.ToDateTime("01/01/" + dataTable.Rows[i][1].ToString().Trim());
                            NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayKetThuc;
                    string NgayKetThucTL = string.Empty;
                    try
                    {
                        NgayKetThuc = Convert.ToDateTime(dataTable.Rows[i][2].ToString().Trim());
                        NgayKetThucTL = NgayKetThuc.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKetThuc = Convert.ToDateTime("01/01/" + dataTable.Rows[i][2].ToString().Trim());
                            NgayKetThucTL = NgayKetThuc.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("TuNgay", NgayKyTL));
                    sqlparamt.Add(new SqlParameter("DenNgay", NgayKetThucTL));
                    sqlparamt.Add(new SqlParameter("NoiHoc", dataTable.Rows[i][3].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NganhHoc", dataTable.Rows[i][4].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("HeDaoTao", dataTable.Rows[i][5].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("BangCap", dataTable.Rows[i][6].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinQuyTrinhDaoTao @MaNhanVien, @TuNgay, @DenNgay, @NoiHoc, @NganhHoc, @HeDaoTao, @BangCap", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinQuyTrinhDaoTao(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinQuyTrinhDaoTao_WithError(dataTable, RowsError);
        }
        public void importThongTinQuyTrinhDaoTao_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinQuyTrinhDaoTao(dataTable);
        }
        // import quá trình công tác
        public void importThongTinQuaTrinhCongTac(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {

                    DateTime NgayKy;
                    string NgayKyTL = string.Empty;
                    try
                    {
                        NgayKy = Convert.ToDateTime(dataTable.Rows[i][1].ToString().Trim());
                        NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKy = Convert.ToDateTime("01/01/" + dataTable.Rows[i][1].ToString().Trim());
                            NgayKyTL = NgayKy.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayKetThuc;
                    string NgayKetThucTL = string.Empty;
                    try
                    {
                        NgayKetThuc = Convert.ToDateTime(dataTable.Rows[i][2].ToString().Trim());
                        NgayKetThucTL = NgayKetThuc.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKetThuc = Convert.ToDateTime("01/01/" + dataTable.Rows[i][2].ToString().Trim());
                            NgayKetThucTL = NgayKetThuc.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("TuNgay", NgayKyTL));
                    sqlparamt.Add(new SqlParameter("DenNgay", NgayKetThucTL));
                    sqlparamt.Add(new SqlParameter("CoQuan", dataTable.Rows[i][3].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("ViTri", dataTable.Rows[i][4].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("DiaChi", dataTable.Rows[i][5].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinQuaTrinhCongTac @MaNhanVien, @TuNgay, @DenNgay, @CoQuan, @ViTri, @DiaChi", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinQuaTrinhCongTac(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinQuaTrinhCongTac_WithError(dataTable, RowsError);
        }
        public void importThongTinQuaTrinhCongTac_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinQuaTrinhCongTac(dataTable);
        }
        // import Thông tin gia đình
        public void importThongTinGiaDinh(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    DateTime NgaySinh;
                    string NgaySinhTL = string.Empty;
                    try
                    {
                        NgaySinh = Convert.ToDateTime(dataTable.Rows[i][2].ToString().Trim());
                        //NgaySinhTL = NgaySinh.ToString("MM/dd/yyyy");
                        NgaySinhTL = NgaySinh.ToString("yyyyMMdd");
                    }
                    catch
                    {
                        try
                        {
                            NgaySinh = Convert.ToDateTime("01/01/" + dataTable.Rows[i][2].ToString().Trim());
                            NgaySinhTL = NgaySinh.ToString("yyyyMMdd");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("HoTen", dataTable.Rows[i][1].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NgaySinh", NgaySinhTL));
                    sqlparamt.Add(new SqlParameter("NoiO", dataTable.Rows[i][3].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("QuanHe", dataTable.Rows[i][4].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("DiaChi", dataTable.Rows[i][5].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinGiaDinh @MaNhanVien, @HoTen, @NgaySinh, @NoiO, @QuanHe, @DiaChi", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinGiaDinh(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinGiaDinh_WithError(dataTable, RowsError);
        }
        public void importThongTinGiaDinh_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinGiaDinh(dataTable);
        }
        // import thông tin chính trị
        public void importThongTinChinhTri(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    DateTime NgayVaoDoan;
                    string NgayVaoDoanTL = string.Empty;
                    try
                    {
                        NgayVaoDoan = Convert.ToDateTime(dataTable.Rows[i][1].ToString().Trim());
                        NgayVaoDoanTL = NgayVaoDoan.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayVaoDoan = Convert.ToDateTime("01/01/" + dataTable.Rows[i][1].ToString().Trim());
                            NgayVaoDoanTL = NgayVaoDoan.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayNN;
                    string NgayNhapNgu = string.Empty;
                    try
                    {
                        NgayNN = Convert.ToDateTime(dataTable.Rows[i][3].ToString().Trim());
                        NgayNhapNgu = NgayNN.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayNN = Convert.ToDateTime("01/01/" + dataTable.Rows[i][3].ToString().Trim());
                            NgayNhapNgu = NgayNN.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayXN;
                    string NgayXuatNgu = string.Empty;
                    try
                    {
                        NgayXN = Convert.ToDateTime(dataTable.Rows[i][4].ToString().Trim());
                        NgayXuatNgu = NgayXN.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayXN = Convert.ToDateTime("01/01/" + dataTable.Rows[i][4].ToString().Trim());
                            NgayXuatNgu = NgayXN.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayVD;
                    string NgayVaoDang = string.Empty;
                    try
                    {
                        NgayVD = Convert.ToDateTime(dataTable.Rows[i][5].ToString().Trim());
                        NgayVaoDang = NgayVD.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayVD = Convert.ToDateTime("01/01/" + dataTable.Rows[i][5].ToString().Trim());
                            NgayVaoDang = NgayVD.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayVDCT;
                    string NgayVaoDangCT = string.Empty;
                    try
                    {
                        NgayVDCT = Convert.ToDateTime(dataTable.Rows[i][6].ToString().Trim());
                        NgayVaoDangCT = NgayVDCT.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayVDCT = Convert.ToDateTime("01/01/" + dataTable.Rows[i][6].ToString().Trim());
                            NgayVaoDangCT = NgayVDCT.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    DateTime NgayRD;
                    string NgayRoiDang = string.Empty;
                    try
                    {
                        NgayRD = Convert.ToDateTime(dataTable.Rows[i][7].ToString().Trim());
                        NgayRoiDang = NgayRD.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayRD = Convert.ToDateTime("01/01/" + dataTable.Rows[i][7].ToString().Trim());
                            NgayRoiDang = NgayRD.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    if (dataTable.Rows[i][1].ToString().Trim() != "")
                        sqlparamt.Add(new SqlParameter("NgayVaoDoan", NgayVaoDoanTL));
                    else
                        sqlparamt.Add(new SqlParameter("NgayVaoDoan", DBNull.Value));
                    sqlparamt.Add(new SqlParameter("NoiVaoDoan", dataTable.Rows[i][2].ToString().Trim()));
                    if (dataTable.Rows[i][3].ToString().Trim() != "")
                        sqlparamt.Add(new SqlParameter("NgayNhapNgu", NgayNhapNgu));
                    else
                        sqlparamt.Add(new SqlParameter("NgayNhapNgu", DBNull.Value));
                    if (dataTable.Rows[i][4].ToString().Trim() != "")
                        sqlparamt.Add(new SqlParameter("NgayXuatNgu", NgayXuatNgu));
                    else
                        sqlparamt.Add(new SqlParameter("NgayXuatNgu", DBNull.Value));
                    if (dataTable.Rows[i][5].ToString().Trim() != "")
                        sqlparamt.Add(new SqlParameter("NgayVaoDang", NgayVaoDang));
                    else
                        sqlparamt.Add(new SqlParameter("NgayVaoDang", DBNull.Value));
                    if (dataTable.Rows[i][6].ToString().Trim() != "")
                        sqlparamt.Add(new SqlParameter("NgayChinhThucVaoDang", NgayVaoDangCT));
                    else
                        sqlparamt.Add(new SqlParameter("NgayChinhThucVaoDang", DBNull.Value));
                    if (dataTable.Rows[i][7].ToString().Trim() != "")
                        sqlparamt.Add(new SqlParameter("NgayRoiDang", NgayRoiDang));
                    else
                        sqlparamt.Add(new SqlParameter("NgayRoiDang", DBNull.Value));
                    sqlparamt.Add(new SqlParameter("ThongTin", dataTable.Rows[i][8].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("LyDo", dataTable.Rows[i][9].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinChinhTri @MaNhanVien,@NgayVaoDoan, @NoiVaoDoan, @NgayNhapNgu, @NgayXuatNgu, @NgayVaoDang, @NgayChinhThucVaoDang, @NgayRoiDang, @ThongTin, @LyDo", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinChinhTri(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinChinhTri_WithError(dataTable, RowsError);
        }
        public void importThongTinChinhTri_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinChinhTri(dataTable);
        }
        // import thông tin sức khỏe nhân viên
        public void importThongTinSucKhoe(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (dataTable.Rows[i][j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    DateTime NgayKham;
                    string NgayKhamTL = string.Empty;
                    try
                    {
                        NgayKham = Convert.ToDateTime(dataTable.Rows[i][1].ToString().Trim());
                        NgayKhamTL = NgayKham.ToString("MM/dd/yyyy");
                    }
                    catch
                    {
                        try
                        {
                            NgayKham = Convert.ToDateTime("01/01/" + dataTable.Rows[i][1].ToString().Trim());
                            NgayKhamTL = NgayKham.ToString("MM/dd/yyyy");
                        }
                        catch
                        {

                        }
                    }
                    var a = dataTable.Rows[i][2].ToString().Trim().Replace(".", "").Replace(",", ".");
                    var b = dataTable.Rows[i][3].ToString().Trim().Replace(".", "").Replace(",", ".");
                    List<SqlParameter> sqlparamt = new List<SqlParameter>();
                    sqlparamt.Add(new SqlParameter("MaNhanVien", dataTable.Rows[i][0].ToString().Trim()));
                    sqlparamt.Add(new SqlParameter("NgayKham", NgayKhamTL));
                    sqlparamt.Add(new SqlParameter("ChieuCao", a));
                    sqlparamt.Add(new SqlParameter("CanNang", b));
                    sqlparamt.Add(new SqlParameter("TinhTrangSK", dataTable.Rows[i][4].ToString().Trim()));
                    db.Database.ExecuteSqlCommand("Exec importNS_NhanVien_ThongTinSucKhoe @MaNhanVien, @NgayKham, @ChieuCao, @CanNang, @TinhTrangSK", sqlparamt.ToArray());
                }
            }
        }
        public void IgnoreErrorThongTinSucKhoe(Stream inputfile, string RowsError)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importThongTinSucKhoe_WithError(dataTable, RowsError);
        }
        public void importThongTinSucKhoe_WithError(DataTable dataTable, string rowsError)
        {
            string[] mang = rowsError.Split('_');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importThongTinSucKhoe(dataTable);
        }

        // import Khách hàng
        public List<ErrorDMHangHoa> importKhachHang(DataTable dataTable, Guid ID_NhanVien, Guid ID_DonVi, string nguoitao ="admin")
        {
            List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                        classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                        ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);

                        for (int i = 0, len = dataTable.Rows.Count; i < len; i++)
                        {
                            DataRow dr = dataTable.Rows[i];
                            var nhomkhach = dr[0].ToString().Trim();
                            var nguonkhach = dr[1].ToString().Trim();
                            var trangthaiKhach = dr[2].ToString().Trim();
                            var maDoiTuong = dr[3].ToString().Trim();
                            var tenDT = dr[4].ToString().Trim();
                            var gtinh = dr[5].ToString().Trim();
                            var loaikhach = dr[6].ToString().Trim();
                            var ngaysinh = dr[7].ToString().Trim();
                            var diachi = dr[8].ToString().Trim();
                            var email = dr[9].ToString().Trim();
                            var dienthoai = dr[10].ToString().Trim();
                            var ghichu = dr[11].ToString().Trim();
                            var masothue = dr[12].ToString().Trim();
                            var nocanthu = dr[13].ToString().Trim();
                            var nocantra = dr[14].ToString().Trim();
                            var tongdiem = dr[15].ToString().Trim();
                            var soduthe = dr[16].ToString().Trim();

                            string sMaDoiTuong = string.Empty;
                            if (!string.IsNullOrEmpty(maDoiTuong))
                                sMaDoiTuong = maDoiTuong.ToUpper();
                            else
                                sMaDoiTuong = classdoituong.GetMaDoiTuongMax_byTemp(1, ID_DonVi);
                            string sMaNhom = string.Empty;
                            Random rd = new Random();
                            sMaNhom = rd.Next().ToString();
                            bool GioiTinhNam = gtinh != "";
                            bool LaCaNhan = loaikhach == "";
                            string sMaHoaDonThu = _classQHD.SP_GetAutoCode(11);
                            string sMaHoaDonChi = _classQHD.SP_GetAutoCode(12);
                            string sMaHD_DieuChinhDiem = _classQHD.SP_GetAutoCode(15);
                            string sMaHD_DieuChinhThe = classHoaDon.SP_GetMaHoaDon_byTemp(23, ID_DonVi, DateTime.Now);

                            DateTime NgaySinh_ThanhLap;
                            string NgaySinhTL = string.Empty;
                            string DinhDangNS = string.Empty;
                            try
                            {
                                NgaySinh_ThanhLap = Convert.ToDateTime(ngaysinh);
                                NgaySinhTL = NgaySinh_ThanhLap.ToString("dd/MM/yyyy");
                            }
                            catch
                            {
                                try
                                {
                                    NgaySinh_ThanhLap = Convert.ToDateTime("01/01/" + ngaysinh);
                                    NgaySinhTL = NgaySinh_ThanhLap.ToString("dd/MM/yyyy");
                                }
                                catch
                                {

                                }
                            }

                            if (ngaysinh.Length > 0)
                            {
                                string[] arTime = ngaysinh.Split('/');
                                if (arTime.Length == 1)
                                    DinhDangNS = "yyyy";
                                else if (arTime.Length == 2)
                                {
                                    if (arTime[1].Length > 2)
                                        DinhDangNS = "MM/yyyy";
                                    else
                                        DinhDangNS = "dd/MM";
                                }
                                else
                                    DinhDangNS = "dd/MM/yyyy";
                            }

                            List<SqlParameter> sqlparamt = new List<SqlParameter>();
                            sqlparamt.Add(new SqlParameter("MaNhomDoiTuong", sMaNhom));
                            sqlparamt.Add(new SqlParameter("TenNhomDoiTuong", nhomkhach));
                            sqlparamt.Add(new SqlParameter("TenNhomDoiTuong_KhongDau", ""));
                            sqlparamt.Add(new SqlParameter("TenNhomDoiTuong_KyTuDau", ""));
                            sqlparamt.Add(new SqlParameter("MaDoiTuong", sMaDoiTuong));
                            sqlparamt.Add(new SqlParameter("TenDoiTuong", tenDT));
                            sqlparamt.Add(new SqlParameter("TenDoiTuong_KhongDau", ""));
                            sqlparamt.Add(new SqlParameter("TenDoiTuong_ChuCaiDau", ""));
                            sqlparamt.Add(new SqlParameter("GioiTinhNam", GioiTinhNam));
                            sqlparamt.Add(new SqlParameter("LoaiDoiTuong", "1"));
                            sqlparamt.Add(new SqlParameter("LaCaNhan", LaCaNhan));
                            sqlparamt.Add(new SqlParameter("timeCreate", DateTime.Now));
                            sqlparamt.Add(new SqlParameter("NgaySinh_NgayTLap", NgaySinhTL));
                            sqlparamt.Add(new SqlParameter("DinhDangNgaySinh", DinhDangNS));
                            sqlparamt.Add(new SqlParameter("DiaChi", diachi));
                            sqlparamt.Add(new SqlParameter("Email", email));
                            sqlparamt.Add(new SqlParameter("Fax", ""));
                            sqlparamt.Add(new SqlParameter("web", ""));
                            sqlparamt.Add(new SqlParameter("GhiChu", ghichu));
                            sqlparamt.Add(new SqlParameter("DienThoai", dienthoai));
                            sqlparamt.Add(new SqlParameter("MaSoThue", masothue ?? string.Empty));
                            sqlparamt.Add(new SqlParameter("STK", ""));
                            sqlparamt.Add(new SqlParameter("MaHoaDonThu", sMaHoaDonThu));
                            sqlparamt.Add(new SqlParameter("MaHoaDonChi", sMaHoaDonChi));
                            sqlparamt.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                            sqlparamt.Add(new SqlParameter("NguoiTao", nguoitao));
                            sqlparamt.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                            sqlparamt.Add(new SqlParameter("NoCanThu", nocanthu == "" ? 0 : double.Parse(nocanthu)));
                            sqlparamt.Add(new SqlParameter("NoCanTra", nocantra == "" ? 0 : double.Parse(nocantra)));
                            sqlparamt.Add(new SqlParameter("TongTichDiem", tongdiem == "" ? 0 : double.Parse(tongdiem)));
                            sqlparamt.Add(new SqlParameter("MaDieuChinhDiem", sMaHD_DieuChinhDiem));
                            sqlparamt.Add(new SqlParameter("SoDuThe", soduthe == "" ? 0 : double.Parse(soduthe)));
                            sqlparamt.Add(new SqlParameter("MaDieuChinhTheGiaTri", sMaHD_DieuChinhThe));
                            sqlparamt.Add(new SqlParameter("TenNguonKhach", nguonkhach));
                            sqlparamt.Add(new SqlParameter("TenTrangThai", trangthaiKhach));

                            db.Database.ExecuteSqlCommand("Exec import_DoiTuong @MaNhomDoiTuong, @TenNhomDoiTuong,@TenNhomDoiTuong_KhongDau,@TenNhomDoiTuong_KyTuDau, @MaDoiTuong, @TenDoiTuong, @TenDoiTuong_KhongDau," +
                            " @TenDoiTuong_ChuCaiDau, @GioiTinhNam, @LoaiDoiTuong, @LaCaNhan, @timeCreate, @NgaySinh_NgayTLap, @DinhDangNgaySinh, @DiaChi, @Email, @Fax, @web," +
                            "@GhiChu, @DienThoai, @MaSoThue, @STK, @MaHoaDonThu, @MaHoaDonChi, @ID_NhanVien, @NguoiTao, @ID_DonVi, @NoCanThu, @NoCanTra, @TongTichDiem, @MaDieuChinhDiem, @SoDuThe, @MaDieuChinhTheGiaTri, " +
                            "@TenNguonKhach, @TenTrangThai", sqlparamt.ToArray());
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        lstErr.Add(new ErrorDMHangHoa
                        {
                            TenTruongDuLieu = "Exception",
                            ViTri = "0",
                            rowError = -1,
                            loaiError = 1,
                            ThuocTinh = "Exception",
                            DienGiai = ex.Message,
                        });
                    }
                }
            }
            return lstErr;
        }
        public void IgnoreErrorKhachHang(Stream inputfile, string RowsError, Guid ID_NhanVien, Guid ID_DonVi)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importKhachHang_WithError(dataTable, RowsError, ID_NhanVien, ID_DonVi);
        }

        public void importKhachHang_WithError(DataTable dataTable, string rowsError, Guid ID_NhanVien, Guid ID_DonVi)
        {
            string[] mang = rowsError.Split(',');
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importKhachHang(dataTable, ID_NhanVien, ID_DonVi);
        }
        //check lỗi file import nhà cung cấp
        public List<ErrorDMHangHoa> checkfileNhaCungCap(Stream inputFileExcel, Guid ID_NhanVien, Guid ID_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow - 2;
                int tcool = worksheet.Cells.MaxColumn + 1;

                DataTable dataTable = worksheet.Cells.ExportDataTable(3, 0, trows, tcool);
                dataTable.Columns[1].ColumnName = "MaNCC";
                dataTable.Columns[5].ColumnName = "Email";
                dataTable.Columns[7].ColumnName = "DienThoai";

                int chophepTrungSDT = 0;
                var tlap = db.HT_CauHinhPhanMem.Select(x => new { x.ChoPhepTrungSoDienThoai });
                if (tlap != null && tlap.Count() > 0)
                {
                    chophepTrungSDT = tlap.FirstOrDefault().ChoPhepTrungSoDienThoai ?? 0;
                }

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    DataRow dr = dataTable.Rows[i];
                    string rowIndex = "Dòng số: " + (i + 4).ToString();

                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dr[j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        var maNCC = dr[1].ToString().Trim();
                        var ngaysinh = dr[3].ToString().Trim();
                        var email = dr[5].ToString().Trim();
                        var dienthoai = dr[7].ToString().Trim();
                        var nocanthu = dr[12].ToString().Trim();
                        var nocantra = dr[13].ToString().Trim();

                        bool valiDateMaKH = kiemtrakitu(maNCC);
                        if (valiDateMaKH == false)
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Mã NCC",
                                ViTri = rowIndex,
                                ThuocTinh = maNCC,
                                DienGiai = "Mã nhà cung cấp không được chứa ký tự đặc biệt",
                                rowError = i
                            });
                        }

                        // kiểm tra trùng lặp mã khách hàng
                        bool duplicateMaKH = GroupData(dataTable, "MaNCC = '" + maNCC + "'");
                        if (duplicateMaKH == false)
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Mã NCC",
                                ViTri = rowIndex,
                                ThuocTinh = maNCC,
                                DienGiai = "Mã nhà cung cấp: " + maNCC + " bị trùng lặp",
                                rowError = i
                            });
                        }
                        // kiểm tra sự tồn tại của mã khách hàng trong database
                        if (maNCC != "")
                        {
                            bool ExistMaKH = classdoituong.SP_CheckMaDoiTuong_Exist(maNCC);
                            if (ExistMaKH)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Mã NCC",
                                    ViTri = rowIndex,
                                    ThuocTinh = maNCC,
                                    DienGiai = "Mã nhà cung cấp: " + maNCC + " đã tồn tại trong cơ sở dữ liệu",
                                    rowError = i
                                });
                            }
                        }

                        // kiểm tra tên khách hàng
                        if (dr[2].ToString() == "")
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Tên nhà cung cấp",
                                ViTri = rowIndex,
                                ThuocTinh = dr[2].ToString(),
                                DienGiai = "Tên nhà cung cấp không được để trống",
                                rowError = i
                            });
                        }

                        if (ngaysinh != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(ngaysinh);
                            if (valiDateNgaySinh == false)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Ngày sinh/thành lập",
                                    ViTri = rowIndex,
                                    ThuocTinh = ngaysinh,
                                    DienGiai = "Ngày sinh/thành lập không hợp lệ",
                                    rowError = i
                                });
                            }
                            else
                            {
                                DateTime dateTime = Convert.ToDateTime(dr[3]);
                                if (dateTime > DateTime.Now)
                                {
                                    lstError.Add(new ErrorDMHangHoa
                                    {
                                        TenTruongDuLieu = "Ngày sinh/thành lập",
                                        ViTri = rowIndex,
                                        ThuocTinh = ngaysinh,
                                        DienGiai = "Ngày sinh/thành lập không được lớn hơn ngày hiện tại",
                                        rowError = i
                                    });
                                }
                            }
                        }

                        if (email != "")
                        {
                            bool valiDateDiaChi = ValidateEmail(email);
                            if (valiDateDiaChi == false)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Email",
                                    ViTri = rowIndex,
                                    ThuocTinh = email,
                                    DienGiai = "Email không hợp lệ",
                                    rowError = i
                                });
                            }
                        }

                        if (dienthoai != "")
                        {
                            bool isNumber = IsNumberInt(dienthoai);
                            if (isNumber == false)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Điện thoại",
                                    ViTri = rowIndex,
                                    ThuocTinh = dienthoai,
                                    DienGiai = "Điện thoại không hợp lệ",
                                    rowError = i
                                });
                            }

                            if (chophepTrungSDT == 0)
                            {
                                bool duplicateSDT = GroupData(dataTable, "DienThoai = '" + dienthoai + "'");
                                if (duplicateSDT == false)
                                {
                                    lstError.Add(new ErrorDMHangHoa
                                    {
                                        TenTruongDuLieu = "Điện thoại",
                                        ViTri = rowIndex,
                                        ThuocTinh = dienthoai,
                                        DienGiai = "Số điện thoại: " + dienthoai + " bị trùng lặp",
                                        rowError = i
                                    });
                                }
                            }

                            bool checkSDT = classdoituong.SP_CheckSoDienThoai_Exist(dienthoai);
                            if (checkSDT)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Điện thoại",
                                    ViTri = rowIndex,
                                    ThuocTinh = dienthoai,
                                    DienGiai = "Số điện thoại: " + dienthoai + " đã tồn tại trong cơ sở dữ liệu",
                                    rowError = i
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(nocanthu))
                        {
                            bool isNumber = IsNumberInt(nocanthu);
                            if (!isNumber)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Nợ cần thu",
                                    ViTri = rowIndex,
                                    ThuocTinh = nocanthu,
                                    DienGiai = "Nợ cần thu không phải dạng số",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);
                            }
                        }

                        if (!string.IsNullOrEmpty(nocantra))
                        {
                            bool isNumber = IsNumberInt(nocantra);
                            if (!isNumber)
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Nợ cần trả",
                                    ViTri = rowIndex,
                                    ThuocTinh = nocantra,
                                    DienGiai = "Nợ cần trả không phải dạng số",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);
                            }
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    importNhaCungCap(dataTable, ID_NhanVien, ID_DonVi);
                    return null;
                }
            }
        }
        // import nhà cung cấp
        public void importNhaCungCap(DataTable dataTable, Guid ID_NhanVien, Guid ID_DonVi)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                    classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);

                    var nguoitao = "admin";
                    var nd = db.HT_NguoiDung.Where(x => x.ID_NhanVien == ID_NhanVien).Select(x => x.TaiKhoan).ToList();
                    if (nd != null && nd.Count() > 0)
                    {
                        nguoitao = nd.FirstOrDefault().ToString();
                    }

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string dk = "";
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            if (dataTable.Rows[i][j].ToString() != "")
                            {
                                break;
                            }
                            if (j == dataTable.Columns.Count - 1)
                            {
                                dk = "1";
                            }
                        }
                        if (dk == "")
                        {
                            string sMaDoiTuong = string.Empty;
                            if (dataTable.Rows[i][1].ToString() != "")
                                sMaDoiTuong = dataTable.Rows[i][1].ToString().ToUpper();
                            else
                                sMaDoiTuong = classdoituong.SP_GetautoCode(2);
                            string sMaNhom = string.Empty;
                            Random rd = new Random();
                            sMaNhom = rd.Next().ToString();
                            bool GioiTinhNam = true;
                            bool LaCaNhan = false;
                            string sMaHoaDonThu = string.Empty;
                            sMaHoaDonThu = _classQHD.SP_GetAutoCode(11);
                            string sMaHoaDonChi = string.Empty;
                            sMaHoaDonChi = _classQHD.SP_GetAutoCode(12);

                            var nocanthu = dataTable.Rows[i][12].ToString().Trim();
                            var nocantra = dataTable.Rows[i][13].ToString().Trim();

                            List<SqlParameter> sqlparamt = new List<SqlParameter>();
                            sqlparamt.Add(new SqlParameter("MaNhomDoiTuong", sMaNhom));
                            sqlparamt.Add(new SqlParameter("TenNhomDoiTuong", dataTable.Rows[i][0].ToString()));
                            sqlparamt.Add(new SqlParameter("TenNhomDoiTuong_KhongDau", ""));
                            sqlparamt.Add(new SqlParameter("TenNhomDoiTuong_KyTuDau", ""));
                            sqlparamt.Add(new SqlParameter("MaDoiTuong", sMaDoiTuong));
                            sqlparamt.Add(new SqlParameter("TenDoiTuong", dataTable.Rows[i][2].ToString()));
                            sqlparamt.Add(new SqlParameter("TenDoiTuong_KhongDau", ""));
                            sqlparamt.Add(new SqlParameter("TenDoiTuong_ChuCaiDau", ""));
                            sqlparamt.Add(new SqlParameter("GioiTinhNam", GioiTinhNam));
                            sqlparamt.Add(new SqlParameter("LoaiDoiTuong", "2"));
                            sqlparamt.Add(new SqlParameter("LaCaNhan", LaCaNhan));
                            sqlparamt.Add(new SqlParameter("timeCreate", DateTime.Now));
                            sqlparamt.Add(new SqlParameter("NgaySinh_NgayTLap", ""));
                            sqlparamt.Add(new SqlParameter("DinhDangNgaySinh", ""));
                            sqlparamt.Add(new SqlParameter("DiaChi", dataTable.Rows[i][4].ToString()));
                            sqlparamt.Add(new SqlParameter("Email", dataTable.Rows[i][5].ToString()));
                            sqlparamt.Add(new SqlParameter("Fax", dataTable.Rows[i][6].ToString()));
                            sqlparamt.Add(new SqlParameter("web", dataTable.Rows[i][8].ToString()));
                            sqlparamt.Add(new SqlParameter("GhiChu", dataTable.Rows[i][11].ToString()));
                            sqlparamt.Add(new SqlParameter("DienThoai", dataTable.Rows[i][7].ToString()));
                            sqlparamt.Add(new SqlParameter("MaSoThue", dataTable.Rows[i][9].ToString()));
                            sqlparamt.Add(new SqlParameter("STK", dataTable.Rows[i][10].ToString()));
                            sqlparamt.Add(new SqlParameter("MaHoaDonThu", sMaHoaDonThu));
                            sqlparamt.Add(new SqlParameter("MaHoaDonChi", sMaHoaDonChi));
                            sqlparamt.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                            sqlparamt.Add(new SqlParameter("NguoiTao", nguoitao));
                            sqlparamt.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                            sqlparamt.Add(new SqlParameter("NoCanThu", string.IsNullOrEmpty(nocanthu) ? 0 : double.Parse(nocanthu)));
                            sqlparamt.Add(new SqlParameter("NoCanTra", string.IsNullOrEmpty(nocantra) ? 0 : double.Parse(nocantra)));
                            sqlparamt.Add(new SqlParameter("TongTichDiem", string.Empty));
                            sqlparamt.Add(new SqlParameter("MaDieuChinhDiem", string.Empty));
                            sqlparamt.Add(new SqlParameter("SoDuThe", string.Empty));
                            sqlparamt.Add(new SqlParameter("MaDieuChinhTheGiaTri", string.Empty));
                            sqlparamt.Add(new SqlParameter("TenNguonKhach", string.Empty));
                            sqlparamt.Add(new SqlParameter("TenTrangThai", string.Empty));

                            db.Database.ExecuteSqlCommand("Exec import_DoiTuong @MaNhomDoiTuong, @TenNhomDoiTuong,@TenNhomDoiTuong_KhongDau,@TenNhomDoiTuong_KyTuDau, @MaDoiTuong, @TenDoiTuong, @TenDoiTuong_KhongDau," +
                            " @TenDoiTuong_ChuCaiDau, @GioiTinhNam, @LoaiDoiTuong, @LaCaNhan, @timeCreate, @NgaySinh_NgayTLap,@DinhDangNgaySinh, @DiaChi, @Email, @Fax, @web," +
                            "@GhiChu, @DienThoai, @MaSoThue, @STK, @MaHoaDonThu, @MaHoaDonChi, @ID_NhanVien,@NguoiTao, @ID_DonVi, @NoCanThu, @NoCanTra, @TongTichDiem, @MaDieuChinhDiem," +
                            "@SoDuThe, @MaDieuChinhTheGiaTri, @TenNguonKhach, @TenTrangThai", sqlparamt.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("importNhaCungCap " + ex.InnerException + ex.Message);
            }
        }
        public void IgnoreErrorNhaCungCap(Stream inputfile, string RowsError, Guid ID_NhanVien, Guid ID_DonVi)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            importNhaCungCap_WithError(dataTable, RowsError, ID_NhanVien, ID_DonVi);
        }
        public void importNhaCungCap_WithError(DataTable dataTable, string RowError, Guid ID_NhanVien, Guid ID_DonVi)
        {
            dataTable.Rows[0].Delete();
            string[] mang = RowError.Split(',');
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            importNhaCungCap(dataTable, ID_NhanVien, ID_DonVi);
        }

        #region ImportBaoHiem
        public List<ErrorDMHangHoa> checkfileBaoHiem(Stream inputFileExcel, Guid ID_NhanVien, Guid ID_DonVi, bool Continue, bool Update)
        {
            classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            Workbook workbook = new Workbook(inputFileExcel);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            dataTable.Rows[0].Delete();
            dataTable.Columns[0].ColumnName = "MaBH";
            //dataTable.Columns[5].ColumnName = "Email";
            //dataTable.Columns[7].ColumnName = "DienThoai";
            List<int> RowRemove = new List<int>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string dk = "";
                DataRow dr = dataTable.Rows[i];
                string rowIndex = "Dòng số: " + (i + 4).ToString();

                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (dr[j].ToString() != "")
                    {
                        break;
                    }
                    if (j == dataTable.Columns.Count - 1)
                    {
                        dk = "1";
                    }
                }
                if (dk == "")
                {
                    bool isError = false;
                    // kiểm tra mã khách hàng có kí tự đặc biệt không
                    var maNCC = dr[0].ToString().Trim();
                    bool valiDateMaKH = kiemtrakitu(maNCC);
                    if (valiDateMaKH == false)
                    {
                        lstError.Add(new ErrorDMHangHoa
                        {
                            TenTruongDuLieu = "Mã bảo hiểm",
                            ViTri = rowIndex,
                            ThuocTinh = maNCC,
                            DienGiai = "Mã bảo hiểm không được chứa ký tự đặc biệt",
                            rowError = i
                        });
                        isError = true;
                    }
                    // kiểm tra trùng lặp mã khách hàng
                    bool duplicateMaKH = GroupData(dataTable, "MaBH = '" + maNCC + "'");
                    if (duplicateMaKH == false)
                    {
                        lstError.Add(new ErrorDMHangHoa
                        {
                            TenTruongDuLieu = "Mã bảo hiểm",
                            ViTri = rowIndex,
                            ThuocTinh = maNCC,
                            DienGiai = "Mã bảo hiểm: " + maNCC + " bị trùng lặp",
                            rowError = i
                        });
                        isError = true;
                    }
                    if (!Update)
                    {
                        // kiểm tra sự tồn tại của mã khách hàng trong database
                        if (maNCC != "")
                        {
                            bool ExistMaKH = classdoituong.SP_CheckMaDoiTuong_Exist(maNCC);
                            if (ExistMaKH)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Mã bảo hiểm",
                                    ViTri = rowIndex,
                                    ThuocTinh = maNCC,
                                    DienGiai = "Mã bảo hiểm: " + maNCC + " đã tồn tại trong cơ sở dữ liệu",
                                    rowError = i
                                });
                                isError = true;
                            }
                        }
                    }
                    // kiểm tra tên khách hàng
                    string tenbaohiem = dr[1].ToString();
                    if (tenbaohiem == "")
                    {
                        lstError.Add(new ErrorDMHangHoa
                        {
                            TenTruongDuLieu = "Tên bảo hiểm",
                            ViTri = rowIndex,
                            ThuocTinh = tenbaohiem,
                            DienGiai = "Tên bảo hiểm không được để trống",
                            rowError = i
                        });
                        isError = true;
                    }
                    else
                    {
                        DM_DoiTuong bh = db.DM_DoiTuong.Where(p => p.LoaiDoiTuong == 3 && p.TheoDoi == false && p.TenDoiTuong == tenbaohiem).FirstOrDefault();
                        if (bh != null)
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Tên bảo hiểm",
                                ViTri = rowIndex,
                                ThuocTinh = tenbaohiem,
                                DienGiai = "Tên bảo hiểm đã tồn tại.",
                                rowError = i
                            });
                            isError = true;
                        }
                    }
                    var matinhthanh = dr[4].ToString().Trim();
                    var maquanhuyen = dr[5].ToString().Trim();
                    string checktinhthanhquanhuyen = checkTinhThanhQuanHuyen(matinhthanh, maquanhuyen);
                    if (checktinhthanhquanhuyen != "")
                    {
                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                        DM.TenTruongDuLieu = "Tỉnh thành, Quận huyện";
                        DM.ViTri = rowIndex;
                        DM.ThuocTinh = dr[4].ToString() + " - " + dr[5].ToString();
                        DM.DienGiai = checktinhthanhquanhuyen;
                        DM.rowError = i;
                        lstError.Add(DM);
                        isError = true;
                    }
                    string email = dr[7].ToString().Trim();
                    if (email != "")
                    {
                        bool valiDateDiaChi = ValidateEmail(email);
                        if (valiDateDiaChi == false)
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Email",
                                ViTri = rowIndex,
                                ThuocTinh = email,
                                DienGiai = "Email không hợp lệ",
                                rowError = i
                            });
                            isError = true;
                        }
                    }

                    var dienthoai = dr[2].ToString().Trim();
                    if (dienthoai != "")
                    {
                        bool isNumber = IsNumberInt(dienthoai);
                        if (isNumber == false)
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Điện thoại",
                                ViTri = rowIndex,
                                ThuocTinh = dienthoai,
                                DienGiai = "Điện thoại không hợp lệ",
                                rowError = i
                            });
                            isError = true;
                        }

                        var data = db.HT_CauHinhPhanMem.Select(x => new { x.ChoPhepTrungSoDienThoai });
                        // ChoPhepTrungSoDienThoai: setup all chinhanh same 
                        if (data.Count() > 0 && data.FirstOrDefault().ChoPhepTrungSoDienThoai != 1 || data.Count() == 0)
                        {
                            bool duplicateSDT = GroupData(dataTable, "DienThoai = '" + dienthoai + "'");
                            if (duplicateSDT == false)
                            {
                                lstError.Add(new ErrorDMHangHoa
                                {
                                    TenTruongDuLieu = "Điện thoại",
                                    ViTri = rowIndex,
                                    ThuocTinh = dienthoai,
                                    DienGiai = "Số điện thoại: " + dienthoai + " bị trùng lặp",
                                    rowError = i
                                });
                                isError = true;
                            }
                        }

                        bool checkSDT = classdoituong.SP_CheckSoDienThoai_Exist(dienthoai);
                        if (checkSDT)
                        {
                            lstError.Add(new ErrorDMHangHoa
                            {
                                TenTruongDuLieu = "Điện thoại",
                                ViTri = rowIndex,
                                ThuocTinh = dienthoai,
                                DienGiai = "Số điện thoại: " + dienthoai + " đã tồn tại trong cơ sở dữ liệu",
                                rowError = i
                            });
                            isError = true;
                        }
                    }
                    if (Continue)
                    {
                        if (isError)
                        {
                            RowRemove.Add(i);
                            lstError = lstError.Except(lstError.Where(p => p.rowError == i).ToList()).ToList();
                        }
                    }
                }
            }
            foreach (int item in RowRemove)
            {
                dataTable.Rows[item].Delete();
            }
            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                if (importBaoHiem(dataTable, ID_NhanVien, ID_DonVi))
                {
                    lstError.Add(new ErrorDMHangHoa
                    {
                        TenTruongDuLieu = "",
                        ViTri = "",
                        ThuocTinh = "",
                        DienGiai = "Có lỗi xảy ra trong quá trình nhập liệu. Vui lòng thử lại.",
                        rowError = 0
                    });
                    return lstError;
                }

                try
                {
                    HT_NhatKySuDung nhatky = new HT_NhatKySuDung();
                    nhatky.ID = Guid.NewGuid();
                    nhatky.ID_NhanVien = ID_NhanVien;
                    nhatky.ID_DonVi = ID_DonVi;
                    nhatky.ChucNang = "Import bảo hiểm";
                    nhatky.ThoiGian = DateTime.Now;
                    nhatky.NoiDung = "Nhập danh sách bảo hiểm từ file Excel.";
                    nhatky.LoaiNhatKy = 5;
                    nhatky.NoiDungChiTiet = "Nhập danh sách bảo hiểm từ file Excel.";
                    nhatky.LoaiHoaDon = 0;
                    db.HT_NhatKySuDung.Add(nhatky);
                    db.SaveChanges();
                }
                catch
                {

                }
                return null;
            }
        }

        public string checkTinhThanhQuanHuyen(string matinhthanh, string maquanhuyen)
        {
            string result = "";
            if (matinhthanh != "" || maquanhuyen != "")
            {
                DM_QuanHuyen quanHuyen = db.DM_QuanHuyen.Where(p => p.MaQuanHuyen == maquanhuyen).FirstOrDefault();
                DM_TinhThanh tinhthanh = db.DM_TinhThanh.Where(p => p.MaTinhThanh == matinhthanh).FirstOrDefault();
                if (quanHuyen == null && tinhthanh == null && matinhthanh != "" && maquanhuyen != "")
                {
                    result = "Mã tỉnh thành, quận huyện không đúng!";
                }
                else if (quanHuyen == null && maquanhuyen != "")
                {
                    result = "Mã quận huyện không đúng!";
                }
                else if (tinhthanh == null && matinhthanh != "")
                {
                    result = "Mã tỉnh thành không đúng!";
                }
                else if (quanHuyen != null && tinhthanh != null)
                {
                    if (quanHuyen.ID_TinhThanh != tinhthanh.ID)
                    {
                        result = "Quận/Huyện " + quanHuyen.TenQuanHuyen + " (" + quanHuyen.MaQuanHuyen + ") không thuộc tỉnh/thành " +
                            tinhthanh.TenTinhThanh + " (" + tinhthanh.MaTinhThanh + ")";
                    }
                }
            }
            return result;
        }

        public bool importBaoHiem(DataTable dataTable, Guid ID_NhanVien, Guid ID_DonVi)
        {
            bool Error = false;
            try
            {
                List<DM_DoiTuong> lstDoiTuongInsert = new List<DM_DoiTuong>();
                List<DM_DoiTuong> lstUpdate = new List<DM_DoiTuong>();
                List<Quy_HoaDon> lstDieuChinhCongNo = new List<Quy_HoaDon>();
                List<Quy_HoaDon_ChiTiet> lstDieuChinhCongNoChiTiet = new List<Quy_HoaDon_ChiTiet>();

                HT_NguoiDung nguoiDung = db.HT_NguoiDung.Where(p => p.ID_NhanVien == ID_NhanVien).FirstOrDefault();
                string nguoitao = nguoiDung != null ? nguoiDung.TaiKhoan : "ImportExcel";
                DateTime ngaytao = DateTime.Now;
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        string maquanhuyen = dataTable.Rows[i][5].ToString();
                        string matinhthanh = dataTable.Rows[i][4].ToString();
                        DM_QuanHuyen quanHuyen = db.DM_QuanHuyen.Where(p => p.MaQuanHuyen == maquanhuyen).FirstOrDefault();
                        DM_TinhThanh tinhthanh = db.DM_TinhThanh.Where(p => p.MaTinhThanh == matinhthanh).FirstOrDefault();
                        Guid? IdTinhThanh = null;
                        if (tinhthanh != null)
                        {
                            IdTinhThanh = tinhthanh.ID;
                        }
                        Guid? IdQuanHuyen = null;
                        if (quanHuyen != null)
                        {
                            IdQuanHuyen = quanHuyen.ID;
                        }
                        DM_DoiTuong doituong = new DM_DoiTuong();

                        bool isCreate = true;
                        string sMaDoiTuong = string.Empty;
                        if (dataTable.Rows[i][0].ToString() != "")
                        {
                            sMaDoiTuong = dataTable.Rows[i][0].ToString().ToUpper();
                            DM_DoiTuong dtExits = db.DM_DoiTuong.Where(p => p.MaDoiTuong == sMaDoiTuong && p.TheoDoi == false).FirstOrDefault();
                            if (dtExits != null)
                            {
                                doituong = dtExits;
                                isCreate = false;
                            }
                        }
                        else
                            sMaDoiTuong = classdoituong.SP_GetautoCode(3);
                        doituong.MaDoiTuong = sMaDoiTuong;
                        doituong.TenDoiTuong = dataTable.Rows[i][1].ToString();
                        doituong.DienThoai = dataTable.Rows[i][2].ToString();
                        doituong.DiaChi = dataTable.Rows[i][3].ToString();
                        doituong.MaSoThue = dataTable.Rows[i][6].ToString();
                        doituong.Email = dataTable.Rows[i][7].ToString();
                        doituong.GhiChu = dataTable.Rows[i][8].ToString();
                        doituong.ID_TinhThanh = IdTinhThanh;
                        doituong.ID_QuanHuyen = IdQuanHuyen;
                        if (isCreate)
                        {
                            doituong.ID = Guid.NewGuid();
                            doituong.NgayTao = ngaytao;
                            doituong.NguoiTao = nguoitao;
                            doituong.LoaiDoiTuong = 3;
                            doituong.ID_DonVi = ID_DonVi;
                            lstDoiTuongInsert.Add(doituong);
                        }
                        else
                        {
                            doituong.NgaySua = ngaytao;
                            doituong.NguoiSua = nguoitao;
                            lstUpdate.Add(doituong);
                        }

                        string congnocanthu = dataTable.Rows[i][9].ToString();
                        string congnocantra = dataTable.Rows[i][10].ToString();
                        if (congnocanthu != "")
                        {
                            double congnocanthuvalue = double.Parse(congnocanthu.Replace(',', '.'));
                            if (congnocanthuvalue > 0)
                            {
                                Quy_HoaDon qhd = new Quy_HoaDon();
                                qhd.ID = Guid.NewGuid();
                                qhd.MaHoaDon = _classQHD.SP_GetAutoCode(12);
                                qhd.NgayLapHoaDon = ngaytao;
                                qhd.ID_NhanVien = ID_NhanVien;
                                qhd.NguoiNopTien = doituong.TenDoiTuong;
                                qhd.TongTienThu = congnocanthuvalue;
                                qhd.ThuCuaNhieuDoiTuong = false;
                                qhd.NguoiTao = nguoitao;
                                qhd.NgayTao = ngaytao;
                                qhd.ID_DonVi = ID_DonVi;
                                qhd.LoaiHoaDon = 12;
                                qhd.HachToanKinhDoanh = true;
                                qhd.PhieuDieuChinhCongNo = 1;
                                qhd.TrangThai = true;

                                Quy_HoaDon_ChiTiet qhdct = new Quy_HoaDon_ChiTiet();
                                qhdct.ID = Guid.NewGuid();
                                qhdct.ID_HoaDon = qhd.ID;
                                qhdct.ID_DoiTuong = doituong.ID;
                                qhdct.ThuTuThe = 0;
                                qhdct.TienMat = congnocanthuvalue;
                                qhdct.TienGui = 0;
                                qhdct.TienThu = congnocanthuvalue;
                                qhdct.HinhThucThanhToan = 1;
                                qhdct.GhiChu = "Phiếu điều chỉnh công nợ được tạo khi import bảo hiểm";
                                lstDieuChinhCongNo.Add(qhd);
                                lstDieuChinhCongNoChiTiet.Add(qhdct);
                            }
                        }

                        if (congnocantra != "")
                        {
                            double congnocantravalue = double.Parse(congnocantra.Replace(',', '.'));
                            if (congnocantravalue > 0)
                            {
                                Quy_HoaDon qhd = new Quy_HoaDon();
                                qhd.ID = Guid.NewGuid();
                                qhd.MaHoaDon = _classQHD.SP_GetAutoCode(11);
                                qhd.NgayLapHoaDon = ngaytao;
                                qhd.ID_NhanVien = ID_NhanVien;
                                qhd.NguoiNopTien = doituong.TenDoiTuong;
                                qhd.TongTienThu = congnocantravalue;
                                qhd.ThuCuaNhieuDoiTuong = false;
                                qhd.NguoiTao = nguoitao;
                                qhd.NgayTao = ngaytao;
                                qhd.ID_DonVi = ID_DonVi;
                                qhd.LoaiHoaDon = 11;
                                qhd.HachToanKinhDoanh = true;
                                qhd.PhieuDieuChinhCongNo = 1;
                                qhd.TrangThai = true;
                                Quy_HoaDon_ChiTiet qhdct = new Quy_HoaDon_ChiTiet();
                                qhdct.ID = Guid.NewGuid();
                                qhdct.ID_HoaDon = qhd.ID;
                                qhdct.ID_DoiTuong = doituong.ID;
                                qhdct.ThuTuThe = 0;
                                qhdct.TienMat = congnocantravalue;
                                qhdct.TienGui = 0;
                                qhdct.TienThu = congnocantravalue;
                                qhdct.HinhThucThanhToan = 1;
                                qhdct.GhiChu = "Phiếu điều chỉnh công nợ được tạo khi import bảo hiểm";
                                lstDieuChinhCongNo.Add(qhd);
                                lstDieuChinhCongNoChiTiet.Add(qhdct);
                            }
                        }
                    }
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        db.DM_DoiTuong.AddRange(lstDoiTuongInsert);

                        foreach (DM_DoiTuong item in lstUpdate)
                        {
                            DM_DoiTuong dt = db.DM_DoiTuong.Where(p => p.ID == item.ID).FirstOrDefault();
                            dt.TenDoiTuong = item.TenDoiTuong;
                            dt.DienThoai = item.DienThoai;
                            dt.DiaChi = item.DiaChi;
                            dt.ID_TinhThanh = item.ID_TinhThanh;
                            dt.ID_QuanHuyen = item.ID_QuanHuyen;
                            dt.MaSoThue = item.MaSoThue;
                            dt.Email = item.Email;
                            dt.GhiChu = item.GhiChu;
                        }
                        db.Quy_HoaDon.AddRange(lstDieuChinhCongNo);
                        db.Quy_HoaDon_ChiTiet.AddRange(lstDieuChinhCongNoChiTiet);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Error = true;
                        transaction.Rollback();
                        CookieStore.WriteLog("importNhaCungCap - Transaction " + ex.InnerException + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Error = true;
                CookieStore.WriteLog("importNhaCungCap " + ex.InnerException + ex.Message);
            }
            return Error;
        }
        #endregion

        #region DM_LienHe (Import)

        public string Check_FormatTypeExcel(Stream fileInput, string nghiepVu)
        {
            string str = null;
            Workbook objWorkbook = new Workbook(fileInput);
            Worksheet worksheet = objWorkbook.Worksheets[0];
            int tcool = worksheet.Cells.MaxColumn + 1;
            if (worksheet.Cells.Rows.Count < 3)
            {
                str = "File import không có dữ liệu";
            }
            return str;
        }

        // check mã khách hàng trong database
        public bool CheckMaLienHe_Exits(string sCode)
        {
            bool dung = true;
            if (sCode != "")
            {
                DM_LienHe objFind = db.DM_LienHe.Where(x => x.MaLienHe == sCode && (x.TrangThai == 1 || x.TrangThai == null)).FirstOrDefault();
                if (objFind != null)
                {
                    dung = false;
                }
            }
            return dung;
        }

        // check Ma/TenKH not exist in db
        public DM_DoiTuong FindDoiTuong_byCodeOrName(string sInput)
        {
            if (sInput != "")
            {
                var inputLower = sInput.Trim().ToLower();
                var objFind = db.DM_DoiTuong.Where(x => x.MaDoiTuong == sInput || x.TenDoiTuong.Trim().ToLower() == sInput && (x.TheoDoi == false));
                if (objFind != null && objFind.Count() > 0)
                {
                    return objFind.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public bool CheckDoiTuong_byCodeOrName(string sInput)
        {
            bool exist = false;
            if (sInput != "")
            {
                var inputLower = sInput.Trim().ToLower();
                var objFind = db.DM_DoiTuong.Where(x => x.MaDoiTuong == sInput || x.TenDoiTuong.Trim().ToLower() == sInput && (x.TheoDoi == false)).FirstOrDefault();
                if (objFind != null)
                {
                    exist = false;
                }
            }
            return exist;
        }

        public List<ErrorDMHangHoa> CheckImport_DMLienHe(Stream inputFileExcel, string nguoitao)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);

                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                Workbook workbook = new Workbook(inputFileExcel);
                Worksheet worksheet = workbook.Worksheets[0];
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxColumn + 1;

                DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
                dataTable.Rows[0].Delete();
                dataTable.Columns[0].ColumnName = "MaLienHe";
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string dk = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].ToString() != "")
                        {
                            break;
                        }
                        if (j == dataTable.Columns.Count - 1)
                        {
                            dk = "1";
                        }
                    }
                    if (dk == "")
                    {
                        // kiểm tra mã liên hệ có kí tự đặc biệt không
                        var malienhe = dataTable.Rows[i][0].ToString().Trim();
                        bool valiDateMaKH = kiemtrakitu(malienhe);
                        if (valiDateMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã liên hệ";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = malienhe;
                            DM.DienGiai = "Mã liên hệ không được chứa ký tự đặc biệt";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra trùng lặp mã liên hệ
                        bool duplicateMaKH = GroupData(dataTable, "MaLienHe = '" + malienhe + "'");
                        if (duplicateMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã liên hệ";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = malienhe;
                            DM.DienGiai = "Mã liên hệ: " + malienhe + " bị trùng lặp";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra sự tồn tại của mã liên hệ trong database
                        bool ExistMaKH = CheckMaLienHe_Exits(malienhe);
                        if (ExistMaKH == false)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Mã liên hệ";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = malienhe;
                            DM.DienGiai = "Mã liên hệ: " + malienhe + " đã tồn tại trong cơ sở dữ liệu";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }
                        // kiểm tra tên liên hệ
                        if (dataTable.Rows[i][1].ToString() == "")
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Tên liên hệ";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][1].ToString();
                            DM.DienGiai = "Tên liên hệ không được để trống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }

                        // KH not exist
                        DM_DoiTuong objFind = FindDoiTuong_byCodeOrName(dataTable.Rows[i][2].ToString());
                        if (objFind == null)
                        {
                            ErrorDMHangHoa DM = new ErrorDMHangHoa();
                            DM.TenTruongDuLieu = "Khách hàng";
                            DM.ViTri = "Dòng số: " + (i + 4).ToString();
                            DM.ThuocTinh = dataTable.Rows[i][2].ToString();
                            DM.DienGiai = "Khách hàng chưa tồn tại trong hệ thống";
                            DM.rowError = i;
                            lstError.Add(DM);
                        }

                        var dienthoai = dataTable.Rows[i][3].ToString();
                        if (dienthoai != "")
                        {
                            bool isNumber = IsNumberInt(dienthoai);
                            if (isNumber == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Điện thoại";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dienthoai;
                                DM.DienGiai = "Điện thoại không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            var checkSDT = classLienHe.Gets(x => x.SoDienThoai == dienthoai && (x.TrangThai == 1 || x.TrangThai == null));
                            if (checkSDT != null && checkSDT.Count() > 0)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "ĐT di động";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dienthoai;
                                DM.DienGiai = "Số điện thoại: " + dienthoai + " đã tồn tại trong cơ sở dữ liệu";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }

                        var dienthoaiCoDinh = dataTable.Rows[i][4].ToString();
                        if (dienthoaiCoDinh != "")
                        {
                            bool isNumber = IsNumberInt(dienthoaiCoDinh);
                            if (isNumber == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Điện thoại cố định";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dienthoaiCoDinh;
                                DM.DienGiai = "Điện thoại cố định không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }

                        var ngaysinh = dataTable.Rows[i][5].ToString();
                        if (ngaysinh != "")
                        {
                            bool valiDateNgaySinh = ValidateDateTime(ngaysinh);
                            if (valiDateNgaySinh == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Ngày sinh/thành lập";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = ngaysinh;
                                DM.DienGiai = "Ngày sinh/thành lập không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                            else
                            {
                                try
                                {
                                    DateTime dateTime = Convert.ToDateTime(ngaysinh);
                                    if (dateTime > DateTime.Now)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Ngày sinh";
                                        DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                        DM.ThuocTinh = ngaysinh;
                                        DM.DienGiai = "Ngày sinh không được lớn hơn ngày hiện tại";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }

                        if (dataTable.Rows[i][6].ToString() != "")
                        {
                            bool valiDateDiaChi = ValidateEmail(dataTable.Rows[i][6].ToString());
                            if (valiDateDiaChi == false)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Email";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = dataTable.Rows[i][6].ToString();
                                DM.DienGiai = "Email không hợp lệ";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }

                        // check district
                        var sDistrict = dataTable.Rows[i][9].ToString();
                        if (sDistrict != "")
                        {
                            var objDistrict = classdoituong.Get_DMQuanHuyen(x => x.TenQuanHuyen.Trim().ToLower() == sDistrict);
                            if (objDistrict == null)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Quận huyện";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = sDistrict;
                                DM.DienGiai = "Quận huyện chưa tồn tại trong hệ thống";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }

                        // check province
                        var sProvince = dataTable.Rows[i][10].ToString();
                        if (sProvince != "")
                        {
                            var objProvince = classdoituong.Get_DMTinhThanh(x => x.TenTinhThanh.Trim().ToLower() == sProvince);
                            if (objProvince == null)
                            {
                                ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                DM.TenTruongDuLieu = "Tỉnh thành";
                                DM.ViTri = "Dòng số: " + (i + 4).ToString();
                                DM.ThuocTinh = sProvince;
                                DM.DienGiai = "Tỉnh thành chưa tồn tại trong hệ thống";
                                DM.rowError = i;
                                lstError.Add(DM);
                            }
                        }
                    }
                }
                if (lstError.Count > 0)
                {
                    return lstError;
                }
                else
                {
                    Import_DMLienHe(dataTable, nguoitao);
                    return null;
                }
            }
        }

        public void Import_DMLienHe(DataTable dataTable, string nguoitao)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classDM_DoiTuong classdoituong = new classDM_DoiTuong(db);
                    ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string dk = "";
                        for (int j = 0; j < dataTable.Columns.Count; j++) // check row empty
                        {
                            if (dataTable.Rows[i][j].ToString() != "")
                            {
                                break;
                            }
                            if (j == dataTable.Columns.Count - 1)
                            {
                                dk = "1";
                            }
                        }
                        if (dk == "")
                        {
                            string sMaLienHe = string.Empty;
                            if (dataTable.Rows[i][0].ToString().Trim() != "")
                                sMaLienHe = dataTable.Rows[i][0].ToString().ToUpper();
                            else
                                sMaLienHe = classLienHe.SP_GetautoCode();
                            DateTime NgaySinh;
                            string NgaySinhTL = string.Empty;
                            string DinhDangNS = string.Empty;
                            try
                            {
                                NgaySinh = Convert.ToDateTime(dataTable.Rows[i][5].ToString().Trim());
                                NgaySinhTL = NgaySinh.ToString("dd/MM/yyyy");
                            }
                            catch
                            {
                                NgaySinh = Convert.ToDateTime("01/01/" + dataTable.Rows[i][5].ToString().Trim());
                                NgaySinhTL = NgaySinh.ToString("dd/MM/yyyy");
                            }

                            // customer/vendor
                            var objDoiTuong = FindDoiTuong_byCodeOrName(dataTable.Rows[i][2].ToString());
                            var doituongID = objDoiTuong.ID;

                            // district
                            var sDistrict = dataTable.Rows[i][9].ToString().Trim();
                            Guid? districtID = null;
                            var district = classdoituong.Get_DMQuanHuyen(x => x.TenQuanHuyen.Trim().ToLower() == sDistrict.ToLower());
                            if (district != null && district.Count() > 0)
                            {
                                districtID = district.FirstOrDefault().ID;
                            }

                            // province
                            var sProvince = dataTable.Rows[i][10].ToString().Trim();
                            Guid? provinceID = null;
                            var province = classdoituong.Get_DMTinhThanh(x => x.TenTinhThanh.Trim().ToLower() == sProvince.ToLower());
                            if (province != null && province.Count() > 0)
                            {
                                provinceID = province.FirstOrDefault().ID;
                            }

                            List<SqlParameter> sqlparamt = new List<SqlParameter>();
                            sqlparamt.Add(new SqlParameter("MaLienHe", sMaLienHe));
                            sqlparamt.Add(new SqlParameter("TenLienHe", dataTable.Rows[i][1].ToString()));
                            sqlparamt.Add(new SqlParameter("ID_DoiTuong", doituongID));
                            sqlparamt.Add(new SqlParameter("SoDienThoai", dataTable.Rows[i][3].ToString()));
                            sqlparamt.Add(new SqlParameter("DienThoaiCoDinh", dataTable.Rows[i][4].ToString()));
                            sqlparamt.Add(new SqlParameter("NgaySinh", NgaySinhTL));
                            sqlparamt.Add(new SqlParameter("Email", dataTable.Rows[i][6].ToString()));
                            sqlparamt.Add(new SqlParameter("ChucVu", dataTable.Rows[i][7].ToString()));
                            sqlparamt.Add(new SqlParameter("DiaChi", dataTable.Rows[i][8].ToString()));
                            sqlparamt.Add(new SqlParameter("ID_QuanHuyen", districtID ?? (object)DBNull.Value));
                            sqlparamt.Add(new SqlParameter("ID_TinhThanh", provinceID ?? (object)DBNull.Value));
                            sqlparamt.Add(new SqlParameter("GhiChu", dataTable.Rows[i][11].ToString()));
                            sqlparamt.Add(new SqlParameter("NguoiTao", nguoitao));
                            db.Database.ExecuteSqlCommand("Exec Insert_DMLienHe @MaLienHe, @TenLienHe,@ID_DoiTuong,@SoDienThoai, @DienThoaiCoDinh, @NgaySinh, @Email," +
                            "@ChucVu, @DiaChi, @ID_QuanHuyen, @ID_TinhThanh, @GhiChu, @NguoiTao", sqlparamt.ToArray());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("Import_DMLienHe: " + e.InnerException + e.Message);
            }
        }

        // continue import with error
        public void ImportDMLienHe_PassError(Stream inputfile, string RowsError, string nguoitao)
        {
            Workbook workbook = new Workbook(inputfile);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow;
            int tcool = worksheet.Cells.MaxColumn + 1;
            DataTable dataTable = worksheet.Cells.ExportDataTable(2, 0, trows, tcool);
            string[] mang = RowsError.Split('_');
            string a = dataTable.Rows[0][10].ToString();
            dataTable.Rows[0].Delete();
            for (int i = mang.Length - 1; i >= 0; i--)
            {
                int j = int.Parse(mang[i].ToString());
                dataTable.Rows[j].Delete();
            }
            Import_DMLienHe(dataTable, nguoitao);
        }
        #endregion

        #region Import TonGoiDV
        public List<ErrorDMHangHoa> ReadFileExcel(Stream fileInput, Guid idDonVi, Guid idNhanVien, string nguoitao)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            List<TonGoiDichVus> lstGoiDV = new List<TonGoiDichVus>();

            try
            {
                classDonViQuiDoi classDonViQuiDoi = new classDonViQuiDoi(db);
                classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);

                Workbook objWorkbook = new Workbook(fileInput);
                int countSheets = objWorkbook.Worksheets.Count();

                for (int k = 0; (k < countSheets) && (k < 10); k++)
                {
                    Worksheet worksheet = objWorkbook.Worksheets[k];
                    var f = objWorkbook.Worksheets;
                    int trows = worksheet.Cells.MaxDataRow;
                    int tcool = worksheet.Cells.MaxDataColumn + 1; // ?? khong hieu tai sao no khong lay dc cot cuoi dung, nen phai + 1 
                    string sheet = worksheet.Name.ToString();
                    DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);// export bao gom header -> bat dau tu dong 1

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        string rowIndex = "Dòng số: " + (i + 3).ToString();
                        var soluong = dr[4].ToString().Trim();
                        if (soluong != string.Empty)
                        {
                            if (IsNumber(soluong))
                            {
                                TonGoiDichVus itemGDV = new TonGoiDichVus();
                                var maKH = dr[0].ToString().Trim();
                                var maGoi = dr[1].ToString().Trim();
                                var ngayhethan = dr[2].ToString().Trim();
                                var madv = dr[3].ToString().Trim();
                                var dongia = dr[5].ToString().Trim();

                                if (maKH != string.Empty)
                                {
                                    var itemKH = db.DM_DoiTuong.Where(x => x.MaDoiTuong == maKH).Select(x => new { x.ID });
                                    if (itemKH.Count() == 0)
                                    {
                                        ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                        {
                                            TenTruongDuLieu = "Mã khách hàng",
                                            ViTri = rowIndex,
                                            ThuocTinh = maKH,
                                            DienGiai = "Khách hàng chưa tồn tại trong hệ thống",
                                            rowError = i,
                                        };
                                        lstError.Add(itemErr);
                                    }
                                    else
                                    {
                                        // check exist in list
                                        //var exList = lstGoiDV.Where(x => x.ID_DoiTuong == itemKH.FirstOrDefault().ID);
                                        //if (exList.Count() == 0)
                                        //{
                                        //add KH to list
                                        itemGDV.ID_DoiTuong = itemKH.FirstOrDefault().ID;
                                        itemGDV.MaDoiTuong = maKH;
                                        lstGoiDV.Add(itemGDV);
                                        //}
                                        //else
                                        //{
                                        //    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                        //    {
                                        //        TenTruongDuLieu = "Mã khách hàng",
                                        //        ViTri = rowIndex,
                                        //        ThuocTinh = maKH,
                                        //        DienGiai = "Mã khách hàng bị trùng lặp",
                                        //        rowError = i,
                                        //    };
                                        //    lstError.Add(itemErr);
                                        //}
                                    }
                                }

                                if (maKH != string.Empty && maGoi == string.Empty)
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Số thẻ",
                                        ViTri = rowIndex,
                                        ThuocTinh = maKH,
                                        DienGiai = "Số thẻ không được để trống",
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }
                                else
                                {
                                    if (maGoi != string.Empty)
                                    {
                                        var exGoi = lstGoiDV.Where(x => x.GoiDVs.Where(o => o.MaHoaDon == maGoi).Count() > 0);
                                        if (exGoi.Count() == 0)
                                        {
                                            // add goidv to list
                                            var date3 = DateTime.Now.AddYears(3);
                                            if (ngayhethan != string.Empty)
                                            {
                                                if (ValidateDateTime(ngayhethan) == false)
                                                {
                                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                                    {
                                                        TenTruongDuLieu = "Ngày hết hạn",
                                                        ViTri = rowIndex,
                                                        ThuocTinh = ngayhethan,
                                                        DienGiai = "Ngày hết hạn chưa đúng định dang ngày tháng năm ",
                                                        rowError = i,
                                                    };
                                                    lstError.Add(itemErr);
                                                }
                                                else
                                                {
                                                    date3 = Convert.ToDateTime(ngayhethan);
                                                }
                                            }
                                            lstGoiDV.Last().GoiDVs.Add(new GoiDV() { MaHoaDon = maGoi, NgayHetHan = date3 });
                                        }
                                        else
                                        {
                                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                            {
                                                TenTruongDuLieu = "Số thẻ",
                                                ViTri = rowIndex,
                                                ThuocTinh = maGoi,
                                                DienGiai = "Số thẻ bị trùng lặp",
                                                rowError = i,
                                            };
                                            lstError.Add(itemErr);
                                        }
                                    }

                                    if (madv != string.Empty)
                                    {
                                        var itemQD = classDonViQuiDoi.Select_DonViQuiDoi(madv);
                                        if (itemQD == null)
                                        {
                                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                            {
                                                TenTruongDuLieu = "Mã hàng hóa/ dịch vụ",
                                                ViTri = rowIndex,
                                                ThuocTinh = madv,
                                                DienGiai = "Mã hàng hóa/ dịch vụ chưa tồn tại trong hệ thống",
                                                rowError = i,
                                            };
                                            lstError.Add(itemErr);
                                        }
                                        else
                                        {
                                            var giabanDB = itemQD.GiaBan;
                                            if (dongia != string.Empty)
                                            {
                                                giabanDB = Convert.ToDouble(dongia);
                                            }
                                            lstGoiDV.Last().GoiDVs.Last().ListDichVu.Add(new TonGoiDichVu_ChiTiet
                                            {
                                                ID_DonViQuiDoi = itemQD.ID,
                                                MaHangHoa = itemQD.MaHangHoa,
                                                SoLuong = Convert.ToDouble(soluong),
                                                DonGia = giabanDB,
                                                GhiChu = dr[6].ToString().Trim()
                                            });
                                        }
                                    }
                                    else
                                    {
                                        ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                        {
                                            TenTruongDuLieu = "Mã hàng hóa/ dịch vụ",
                                            ViTri = rowIndex,
                                            ThuocTinh = madv,
                                            DienGiai = "Mã hàng hóa/ dịch vụ không được để trống",
                                            rowError = i,
                                        };
                                        lstError.Add(itemErr);
                                    }
                                }
                            }
                            else
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Số lượng",
                                    ViTri = rowIndex,
                                    ThuocTinh = soluong,
                                    DienGiai = "Số lượng không phải dang số",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                {
                    TenTruongDuLieu = "Exception",
                    ThuocTinh = "Exception",
                    DienGiai = e.InnerException + e.Message
                };
                lstError.Add(itemErr);
            }
            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                lstError = ImportGoiDV(lstGoiDV, idDonVi, idNhanVien, nguoitao);
                return lstError;
            }
        }

        public List<ErrorDMHangHoa> ImportGoiDV(List<TonGoiDichVus> lstGDV, Guid idDonVi, Guid idNhanVien, string nguoitao)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<BH_HoaDon> lstHD = new List<BH_HoaDon>();
                    List<BH_HoaDon_ChiTiet> lstCTHD = new List<BH_HoaDon_ChiTiet>();
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    string noidungCT = string.Empty;

                    foreach (var kh in lstGDV)
                    {
                        noidungCT += noidungCT != string.Empty ? string.Concat(" <br /> ", kh.MaDoiTuong, ": <br /> ") : string.Concat(kh.MaDoiTuong, ": <br /> ");

                        string styleMaHD = string.Empty;
                        foreach (var goi in kh.GoiDVs)
                        {
                            BH_HoaDon hd = new BH_HoaDon();
                            hd.ID = Guid.NewGuid();
                            hd.LoaiHoaDon = 19;
                            hd.ID_DonVi = idDonVi;
                            hd.ID_DoiTuong = kh.ID_DoiTuong;
                            hd.ID_NhanVien = idNhanVien;
                            if (goi.MaHoaDon == string.Empty)
                            {
                                hd.MaHoaDon = "Import_" + classhoadon.SP_GetMaHoaDon_byTemp(19, idDonVi, DateTime.Now);
                            }
                            else
                            {
                                hd.MaHoaDon = "Import_" + goi.MaHoaDon;
                            }
                            hd.NgayLapHoaDon = DateTime.Now;
                            hd.HanSuDungGoiDV = goi.NgayHetHan;
                            hd.ChoThanhToan = false;
                            hd.TongTienHang = 0;
                            hd.PhaiThanhToan = 0;
                            hd.TongChietKhau = 0;
                            hd.TongGiamGia = 0;
                            hd.TongChiPhi = 0;
                            hd.TongTienThue = 0;
                            hd.DienGiai = "Import tồn đầu kỳ gói dịch vụ";
                            hd.NguoiTao = nguoitao;
                            lstHD.Add(hd);
                            styleMaHD += hd.MaHoaDon;
                            noidungCT += styleMaHD == hd.MaHoaDon ? string.Concat(" - ", hd.MaHoaDon, ": ") : string.Concat(" <br /> - ", hd.MaHoaDon, ": ");

                            int stt = 1;
                            foreach (var ct in goi.ListDichVu)
                            {
                                BH_HoaDon_ChiTiet cthd = new BH_HoaDon_ChiTiet();
                                cthd.ID = Guid.NewGuid();
                                cthd.ID_HoaDon = hd.ID;
                                cthd.ID_DonViQuiDoi = ct.ID_DonViQuiDoi;
                                cthd.SoLuong = ct.SoLuong ?? 0;
                                cthd.DonGia = ct.DonGia ?? 0;
                                cthd.TienChietKhau = ct.DonGia ?? 0;
                                cthd.GhiChu = ct.GhiChu;
                                cthd.ThanhTien = 0;
                                cthd.PTChietKhau = 0;
                                cthd.TienThue = 0;
                                cthd.PTThue = 0;
                                cthd.PTChiPhi = 0;
                                cthd.TienChiPhi = 0;
                                cthd.An_Hien = true;
                                cthd.SoThuTu = stt;
                                stt++;
                                noidungCT += string.Concat(ct.MaHangHoa, ", ");

                                lstCTHD.Add(cthd);
                            }
                        }
                    }

                    db.BH_HoaDon.AddRange(lstHD);
                    db.BH_HoaDon_ChiTiet.AddRange(lstCTHD);
                    db.SaveChanges();

                    #region NhatKySuDung
                    HT_NhatKySuDung nky = new HT_NhatKySuDung()
                    {
                        ID = Guid.NewGuid(),
                        ID_DonVi = idDonVi,
                        LoaiHoaDon = 19,
                        ID_NhanVien = idNhanVien,
                        ChucNang = "Gói dịch vụ - Import",
                        LoaiNhatKy = 5,
                        NoiDung = "Import tồn đầu kỳ gói dịch vụ",
                        NoiDungChiTiet = noidungCT,
                        ThoiGian = DateTime.Now,
                    };
                    SaveDiary.add_Diary(nky);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                {
                    TenTruongDuLieu = "Exception",
                    ThuocTinh = "Exception",
                    DienGiai = ex.InnerException + ex.Message
                };
                lstError.Add(itemErr);
            }
            return lstError;
        }
        #endregion 
        #region Import TonDau TheGiaTri
        public List<ErrorDMHangHoa> CheckFile_TonDauTGT(Stream fileInput, Guid idDonVi, Guid idNhanVien, string nguoitao, string indexErrs)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            List<ImportDto_TonDauTGT> lstData = new List<ImportDto_TonDauTGT>();

            try
            {
                classDonViQuiDoi classDonViQuiDoi = new classDonViQuiDoi(db);
                classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);

                Workbook objWorkbook = new Workbook(fileInput);
                Worksheet worksheet = objWorkbook.Worksheets[0];
                var f = objWorkbook.Worksheets;
                int trows = worksheet.Cells.MaxDataRow;
                int tcool = worksheet.Cells.MaxDataColumn + 1; // ?? khong hieu tai sao no khong lay dc cot cuoi dung, nen phai + 1 
                string sheet = worksheet.Name.ToString();
                DataTable dt = worksheet.Cells.ExportDataTable(1, 0, trows, tcool, true);// export bao gom header -> bat dau tu dong 1

                // bo qua dong loi
                string[] mang = indexErrs.Split(',');
                for (int i = mang.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(mang[i]))
                    {
                        int j = int.Parse(mang[i].ToString());
                        dt.Rows[j].Delete();
                    }
                }
                dt.Columns[0].ColumnName = "MaKhachHang";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var dr = dt.Rows[i];
                    string rowIndex = "Dòng số: " + (i + 3).ToString();
                    var tondauTGT = dr[1].ToString().Trim();
                    if (tondauTGT != string.Empty)
                    {
                        if (IsNumber(tondauTGT))
                        {
                            ImportDto_TonDauTGT itemGDV = new ImportDto_TonDauTGT();
                            var maKH = dr[0].ToString().Trim();
                            if (maKH != string.Empty)
                            {
                                var itemKH = db.DM_DoiTuong.Where(x => x.MaDoiTuong == maKH).Select(x => new { x.ID });
                                if (itemKH.Count() == 0)
                                {
                                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                    {
                                        TenTruongDuLieu = "Mã khách hàng",
                                        ViTri = rowIndex,
                                        ThuocTinh = maKH,
                                        DienGiai = "Khách hàng chưa tồn tại trong hệ thống",
                                        rowError = i,
                                    };
                                    lstError.Add(itemErr);
                                }
                                else
                                {
                                    bool trungma = GroupData(dt, "MaKhachHang = '" + maKH + "'");
                                    if (trungma == false)
                                    {
                                        ErrorDMHangHoa DM = new ErrorDMHangHoa();
                                        DM.TenTruongDuLieu = "Mã khách hàng";
                                        DM.ViTri = rowIndex;
                                        DM.ThuocTinh = maKH;
                                        DM.DienGiai = "Mã khách hàng " + maKH + " bị trùng lặp";
                                        DM.rowError = i;
                                        lstError.Add(DM);
                                    }
                                    else
                                    {
                                        //add to list
                                        itemGDV.ID_DoiTuong = itemKH.FirstOrDefault().ID;
                                        itemGDV.MaDoiTuong = maKH;
                                        itemGDV.GiaTri = Convert.ToDouble(tondauTGT);
                                        lstData.Add(itemGDV);
                                    }
                                }
                            }
                            else
                            {
                                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = "Mã khách hàng",
                                    ViTri = rowIndex,
                                    ThuocTinh = "",
                                    DienGiai = "Vui lòng nhập mã khách hàng",
                                    rowError = i,
                                };
                                lstError.Add(itemErr);
                            }
                        }
                        else
                        {
                            ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                            {
                                TenTruongDuLieu = "Tồn đầu thẻ",
                                ViTri = rowIndex,
                                ThuocTinh = tondauTGT,
                                DienGiai = "Tồn đầu thẻ không phải dang số",
                                rowError = i,
                            };
                            lstError.Add(itemErr);
                        }
                    }
                    else
                    {
                        ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                        {
                            TenTruongDuLieu = "Tồn đầu thẻ",
                            ViTri = rowIndex,
                            ThuocTinh = tondauTGT,
                            DienGiai = "Tồn đầu thẻ không được để trống",
                            rowError = i,
                        };
                        lstError.Add(itemErr);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                {
                    TenTruongDuLieu = "Exception",
                    ThuocTinh = "Exception",
                    DienGiai = e.InnerException + e.Message
                };
                lstError.Add(itemErr);
            }
            if (lstError.Count > 0)
            {
                return lstError;
            }
            else
            {
                lstError = Import_TonDauTGT(lstData, idDonVi, idNhanVien, nguoitao);
                return lstError;
            }
        }

        public List<ErrorDMHangHoa> Import_TonDauTGT(List<ImportDto_TonDauTGT> lstData, Guid idDonVi, Guid idNhanVien, string nguoitao)
        {
            List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<BH_HoaDon> lstHD = new List<BH_HoaDon>();
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    string noidungCT = string.Empty;

                    double maxMaHD = classhoadon.GetMaxMaHoaDon(23);
                    string kihieuChungTu = classhoadon.GetKiHieuMaChungTu_byLoaiHoaDon(23);

                    string maHoaDon = string.Empty;

                    foreach (var kh in lstData)
                    {
                        if (maxMaHD < 10)
                        {
                            maHoaDon = string.Concat(kihieuChungTu, "00", maxMaHD);
                        }
                        else
                        {
                            if (maxMaHD < 100)
                            {
                                maHoaDon = string.Concat(kihieuChungTu, "0", maxMaHD);
                            }
                            else
                            {
                                maHoaDon = string.Concat(kihieuChungTu, maxMaHD);
                            }
                        }
                        BH_HoaDon hd = new BH_HoaDon();
                        hd.ID = Guid.NewGuid();
                        hd.LoaiHoaDon = 23;
                        hd.ID_DonVi = idDonVi;
                        hd.ID_DoiTuong = kh.ID_DoiTuong;
                        hd.ID_NhanVien = idNhanVien;
                        hd.MaHoaDon = maHoaDon;
                        hd.NgayLapHoaDon = DateTime.Now;
                        hd.ChoThanhToan = false;
                        hd.TongTienHang = kh.GiaTri;
                        hd.TongChiPhi = kh.GiaTri;
                        hd.TongTienThue = kh.GiaTri;
                        hd.PhaiThanhToan = 0;
                        hd.TongChietKhau = 0;
                        hd.TongGiamGia = 0;
                        hd.DienGiai = "Import tồn đầu thẻ giá trị";
                        hd.NguoiTao = nguoitao;
                        lstHD.Add(hd);
                        noidungCT += string.Concat(" <br /> ", kh.MaDoiTuong, ", Tồn đầu: ", kh.GiaTri, " (", hd.MaHoaDon, ")");
                        maxMaHD += 1;
                    }

                    db.BH_HoaDon.AddRange(lstHD);
                    db.SaveChanges();

                    #region NhatKySuDung
                    HT_NhatKySuDung nky = new HT_NhatKySuDung()
                    {
                        ID = Guid.NewGuid(),
                        ID_DonVi = idDonVi,
                        LoaiHoaDon = 23,
                        ID_NhanVien = idNhanVien,
                        ChucNang = "Thẻ giá trị",
                        LoaiNhatKy = 5,
                        NoiDung = "Import tồn đầu thẻ giá trị",
                        NoiDungChiTiet = noidungCT,
                        ThoiGian = DateTime.Now,
                    };
                    SaveDiary.add_Diary(nky);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                {
                    TenTruongDuLieu = "Exception",
                    ThuocTinh = "Exception",
                    DienGiai = ex.InnerException + ex.Message
                };
                lstError.Add(itemErr);
            }
            return lstError;
        }
        #endregion

    }

    public class Report_HangHoa_XuatHuy_Import
    {
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public double GiaVon { get; set; }
        public double GiaBan { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongXuatHuy { get; set; }
        public double TonKho { get; set; }
        public double GiaTriHuy { get; set; }
        public double TrangThaiMoPhieu { get; set; }
        public string TenLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public int SoThuTu { get; set; }
    }

    public class Report_HangHoa_Chuyenhang_Import
    {
        public Guid ID { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public double? ThanhTien { get; set; }
        public string MaHangHoa { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public double TyLeChuyenDoi { get; set; }
        public string MaLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public List<DonViTinh> DonViTinh { get; set; }
    }
}
