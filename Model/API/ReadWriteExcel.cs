using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Model
{
    public class ReadWriteExcel
    {
        public static void Main()
        {
            //ApplicationClass app = new ApplicationClass(); // the Excel application.
            //                                               // the reference to the workbook,
            //                                               // which is the xls document to read from.
            //Workbook book = null;
            //// the reference to the worksheet,
            //// we'll assume the first sheet in the book.
            //Worksheet sheet = null;
            //Range range = null;

            //app.Visible = false;
            //app.ScreenUpdating = false;
            //app.DisplayAlerts = false;

            ////string execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            //book = app.Workbooks.Open(pathFile,
            //       Missing.Value, Missing.Value, Missing.Value,
            //       Missing.Value, Missing.Value, Missing.Value, Missing.Value,
            //       Missing.Value, Missing.Value, Missing.Value, Missing.Value,
            //       Missing.Value, Missing.Value, Missing.Value);
            //sheet = (Worksheet)book.Worksheets[1];

            //range = sheet.get_Range("A4", Missing.Value);

            //object[,] values = (object[,])range.Value;

            //Console.WriteLine("Row Count: " + values.GetLength(0).ToString());
            //Console.WriteLine("Col Count: " + values.GetLength(1).ToString());

            //for (int j = 1; j <= values.GetLength(1); j++)
            //{
            //    Console.Write("{0}", j);
            //}


            Application xlApp;
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            Range range1;

            string str;
            int rCnt;
            int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Application();
            xlWorkBook = xlApp.Workbooks.Open(@"d:\FileImport_KhachHang_winniebeautyspa1", 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range1 = xlWorkSheet.UsedRange;
            rw = range1.Rows.Count;
            cl = range1.Columns.Count;


            for (rCnt = 1; rCnt <= rw; rCnt++)
            {
                for (cCnt = 1; cCnt <= cl; cCnt++)
                {
                    str = (string)(range1.Cells[rCnt, cCnt] as Range).Value2;
                    //MessageBox.Show(str);
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
