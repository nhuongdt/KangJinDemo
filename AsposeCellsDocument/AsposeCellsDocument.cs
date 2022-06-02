using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsposeCellsDocument
{
    public class AsposeCellsDocument
    {
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

        public DataTable ToDataTable(Stream filestream, int firstRow, int firstColumn)
        {
            Workbook workbook = new Workbook(filestream);
            Worksheet worksheet = workbook.Worksheets[0];
            int trows = worksheet.Cells.MaxDataRow - 1;
            int tcool = worksheet.Cells.MaxColumn+1;

            DataTable dataTable = worksheet.Cells.ExportDataTable(firstRow, firstColumn, trows, tcool, true);
            return dataTable;
        }
    }
}
