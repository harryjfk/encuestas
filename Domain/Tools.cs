using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using Data.Contratos;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Domain
{
    public static class Tools
    {
        public static void UpdateKey<T>(this T element, long value) where T : class
        {
            try
            {
                var property = typeof(T).GetProperty("Id");
                if (property != null)
                {
                    property.SetMethod.Invoke(element, new object[] { value });
                }
            }
            catch
            {

            }
        }

        public static Func<T, long> GeKey<T>(this T element)
        {
            var type = typeof(T);
            var property = type.GetProperty("Id");
            if (property == null) return null;
            var parameter = Expression.Parameter(typeof(T), "t");
            Expression expression = parameter;
            expression = Expression.Property(expression, property);
            return Expression.Lambda<Func<T, long>>(expression, new ParameterExpression[] { parameter }).Compile();
        }

        public static long GetKeyValue<T>(T element)
        {
            var type = typeof(T);
            var prop = type.GetProperty("Id");
            if (prop == null) return 0;
            var value = (long)prop.GetMethod.Invoke(element, null);
            return value;
        }

        public static double Varianza(this List<double> data)
        {
            var media = data.Average();
            if (data.Count < 2) return 0;
            return data.Sum(t => Math.Pow(t - media, 2)) / data.Count - 1;
        }

        public static double DesviacionEstandar(this List<double> data)
        {
            var varianza = data.Varianza();
            if (varianza <= 0) return 0;

            return Math.Sqrt(varianza);
        }

        public static string GetMonthText(this int number)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, number, now.Day);
            var cInfo = CultureInfo.GetCultureInfo("es");
            var textInfo = cInfo.TextInfo;
            var month = date.ToString("MMMM", cInfo);
            return textInfo.ToTitleCase(month);
        }

        public static bool ExportToExcel<T>(this IList<T> source,string nombreHoja,string nombreReporte,string direccion)
        {
            var dt = ExcelUtility.ConvertToDataTable(source);
           return ExcelUtility.WriteDataTableToExcel(dt, nombreHoja, direccion, nombreReporte);
          
        }

        //public static Func<T, bool> GetFilter<T>(this string text) where T : class
        //{
        //    var type = typeof (T);
        //    if (type.IsSubclassOf(typeof (Filtered<T>)))
        //    {
        //        var method = type.GetMethod("GetFilter",BindingFlags.Static);
        //        if (method != null)
        //        {

        //        }b
        //    }
        //}
    }

    public static class ExcelUtility
    {
        public static void FormattingExcelCells(Microsoft.Office.Interop.Excel.Range range, string HTMLcolorCode, System.Drawing.Color fontColor, bool IsFontbool)
        {
            range.Interior.Color = System.Drawing.ColorTranslator.FromHtml(HTMLcolorCode);
            range.Font.Color = System.Drawing.ColorTranslator.ToOle(fontColor);
            if (IsFontbool == true)
            {
                range.Font.Bold = IsFontbool;
            }
        }
        public static bool WriteDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation, string ReporType)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Range excelCellrange;
                var excel = new Microsoft.Office.Interop.Excel.Application
                {
                    Visible = false,
                    DisplayAlerts = false
                };
                var excelworkBook = excel.Workbooks.Add(Type.Missing);
                var excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = worksheetName;
                excelSheet.Cells[1, 1] = ReporType;
                excelSheet.Cells[1, 2] = "Date : " + DateTime.Now.ToShortDateString();
                var rowcount = 2;
                foreach (DataRow datarow in dataTable.Rows)
                {
                    rowcount += 1;
                    for (var i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        if (rowcount == 3)
                        {
                            excelSheet.Cells[2, i] = dataTable.Columns[i - 1].ColumnName;
                            excelSheet.Cells.Font.Color = System.Drawing.Color.Black;

                        }
                        excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                        if (rowcount <= 3) continue;
                        if (i != dataTable.Columns.Count) continue;
                        if (rowcount % 2 != 0) continue;
                        excelCellrange = excelSheet.Range[excelSheet.Cells[rowcount, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                        FormattingExcelCells(excelCellrange, "#CCCCFF", System.Drawing.Color.Black, false);
                    }

                }
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                excelCellrange.EntireColumn.AutoFit();
                var border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[2, dataTable.Columns.Count]];
                FormattingExcelCells(excelCellrange, "#000099", System.Drawing.Color.White, true);
                excelworkBook.SaveAs(saveAsLocation);
                excelworkBook.Close();
                excel.Quit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            var properties =
            TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (data.Any(t => prop.GetValue(t) != null))
                {
                    table.Columns.Add(prop.Name.Replace("_", ""),
                    Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }
            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (true)
                    {
                         row[prop.Name.Replace("_", "")] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
