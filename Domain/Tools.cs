using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace Domain
{
    public static class Tools
    {

        public static List<object> Convert(this IEnumerable<MateriaPropia> list)
        {
            return new List<object>();
            //return list.Select(t => new
            //{
            //    Cod_Establecimiento=t.VolumenProduccion.Encuesta.Establecimiento.IdentificadorInterno,
            //    Razon_Social = t.VolumenProduccion.Encuesta.Establecimiento.RazonSocial,
            //    Mes=t.VolumenProduccion.Encuesta.Fecha.Month.GetMonthText(),
            //    CIIU=t.LineaProducto.Ciiu.Codigo,
            //    Cod_Producto=t.LineaProducto.Codigo,
            //    Descripcion=t.LineaProducto.Nombre,
            //    UM=t.UnidadMedida.Abreviatura,
            //    t.Produccion,
            //    Valor_Unitario=t.ValorUnitario,
            //    Otros_Ingresos=t.OtrosIngresos,
            //    Otras_Salidas=t.OtrasSalidas,
            //    Ventas_extranjero=t.VentasExtranjero,
            //    Ventas_Pais=t.VentasPais,
            //}).ToList();
        }
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
            var date = new DateTime(now.Year, number, 1);
            var cInfo = CultureInfo.GetCultureInfo("es");
            var textInfo = cInfo.TextInfo;
            var month = date.ToString("MMMM", cInfo);
            return textInfo.ToTitleCase(month);
        }

        public static string GetVariableNames(this string value)
        {
            switch (value.ToUpper())
            {
                case "VPMP":
                    return "Volumen de Producción con Materia Prima";
                case "VPMT":
                    return "Volumen de Producción con Materia de terceros";
                case "VAPMP":
                    return "Valor de Producción con Materia Prima";
                case "VAPMT":
                    return "Valor de Producción con Materia de terceros";
                case "VVP":
                    return "Valor de Ventas en el país";
                case "VVE":
                    return "Valor de Ventas en el extranjero";
                case "NT":
                    return "Número de Trabajadores";
            }
            return "";
        }
        public static string GetAmbito(this string value)
        {
            switch (value.ToUpper())
            {
                case "LINEAPRODUCTO":
                    return "Línea de Producto";
                default:
                    return value;
            }
            
        }
       

        public static bool ExportToExcel<T>(this IList<T> source,string nombreHoja,string nombreReporte,string direccion)
        {
            var dt = ExcelUtility.ConvertToDataTable(source);
           return ExcelUtility.WriteDataTableToExcel(dt, nombreHoja, direccion, nombreReporte);
          
        }

      
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
                var pack = new ExcelPackage(new FileInfo(saveAsLocation));
                pack.Workbook.Worksheets.Add(worksheetName);
                var ws = pack.Workbook.Worksheets[1];
                ws.Name = ReporType;
                ws.Cells.Style.Font.Size = 11; 
                ws.Cells.Style.Font.Name = "Calibri";
              
               
                ws.Cells[1, 1].Value = ReporType.ToUpper();
                ws.Cells[1, 2].Value = "DATE : " + DateTime.Now.ToShortDateString().ToUpper();
                var rowcount = 2;

                foreach (DataRow datarow in dataTable.Rows)
                {
                   
                    rowcount += 1;
                    for (var i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        ws.Column(i).AutoFit();
                        if (rowcount == 3)
                        {
                            ws.Cells[2, i].Value = dataTable.Columns[i - 1].ColumnName;
                            var fill = ws.Cells[2, i].Style.Fill;
                            fill.PatternType = ExcelFillStyle.Solid;
                            fill.BackgroundColor.SetColor(Color.Gray);
                        }
                        ws.Cells[rowcount, i].Value = datarow[i - 1].ToString();
                        if (rowcount <= 3) continue;
                        if (i != dataTable.Columns.Count) continue;
                        if (rowcount % 2 != 0) continue;
                        
                    }

                }
             
                pack.Save();
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
                    table.Columns.Add(prop.Name.Replace("_", " "),
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
                         row[prop.Name.Replace("_", " ")] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
