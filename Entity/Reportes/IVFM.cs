using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Reportes
{
    /// <summary>
    /// Indice Volumen Físico Mensual Por Ciiu Desde bd
    /// </summary>
    public class IVFDB
    {
        public double IVF { get; set; }
        public DateTime Fecha { get; set; }
        public long IdCiiu { get; set; }
        /*public string Codigo { get; set; }
        public int SubSector { get; set; }*/
        public double valor_agregado { get; set; }
        public double peso { get; set; }
    }

    /// <summary>
    /// Indice Volumen Físico Mensual Por Establecimiento y Ciiu Desde bd
    /// </summary>
    public class IVFEstDB
    {
        public double IVF { get; set; }
        public DateTime Fecha { get; set; }
        public long IdCiiu { get; set; }
        public long IdEstablecimiento { get; set; }
    }

    public class IVFM
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int Level { get; set; }
        public long IdCiiu { get; set; }
        public string CodigoCiiu { get; set; }
        public string Texto { get; set; }
        public double Enero { get; set; }
        public double Febrero { get; set; }
        public double Marzo { get; set; }
        public double Abril { get; set; }
        public double Mayo { get; set; }
        public double Junio { get; set; }
        public double Julio { get; set; }
        public double Agosto { get; set; }
        public double Setiembre { get; set; }
        public double Octubre { get; set; }
        public double Noviembre { get; set; }
        public double Diciembre { get; set; }
        public bool Visible { get; set; }

        public double ValorAgregado { get; set; }
        public double Peso { get; set; }

        public IVFM(int id, int parentId, int level, string texto)
        {
            Id = id;
            ParentId = parentId;
            Level = level;
            Texto = texto;
        }

        public IVFM(int id, int parentId, int level, string texto, bool visible)
        {
            Id = id;
            ParentId = parentId;
            Level = level;
            Texto = texto;
            Visible = visible;
        }

    }

    public class IVFMReport
    {
        public string Texto { get; set; }
        public string Enero { get; set; }
        public string Febrero { get; set; }
        public string Marzo { get; set; }
        public string Abril { get; set; }
        public string Mayo { get; set; }
        public string Junio { get; set; }
        public string Julio { get; set; }
        public string Agosto { get; set; }
        public string Setiembre { get; set; }
        public string Octubre { get; set; }
        public string Noviembre { get; set; }
        public string Diciembre { get; set; }
        //
        public double ValorAgregado { get; set; }
        public double Peso { get; set; }
    }

    public class IVFMEstReport
    {
        public long IdCiiu { get; set; }
        public long IdEstablecimiento { get; set; }
        public string CodigoCiiu { get; set; }
        public string CodigoEst { get; set; }
        public string NomEst { get; set; }
        public string Enero { get; set; }
        public string Febrero { get; set; }
        public string Marzo { get; set; }
        public string Abril { get; set; }
        public string Mayo { get; set; }
        public string Junio { get; set; }
        public string Julio { get; set; }
        public string Agosto { get; set; }
        public string Setiembre { get; set; }
        public string Octubre { get; set; }
        public string Noviembre { get; set; }
        public string Diciembre { get; set; }
    }

    #region Elmer
    public class IVFMEstVarReport
    {
        public long IdCiiu { get; set; }
        public long IdEstablecimiento { get; set; }
        public string CodigoCiiu { get; set; }
        public string CodigoEst { get; set; }
        public string NomEst { get; set; }
        public List<MesVariacion> Meses { get; set; }

        public IVFMEstVarReport()
        {
            Meses = new List<MesVariacion>();
            for (int i = 1; i <= 12; i++)
            {
                Meses.Add(new MesVariacion() { 
                    Nro = i
                });
            }
        }
    }

    public class MesVariacion
    {
        public int Nro { get; set; }
        public string MesX { get; set; }
        public string MesY { get; set; }
        public string VariacionPorcentual { get; set; }

        public MesVariacion()
        {
            MesX = "0.00";
            MesY = "0.00";
            VariacionPorcentual = "0.00";
        }
    }

    public class IVFResumenReport 
    {
        public string Texto { get; set; }
        public string MesX_anioX { get; set; }
        public string MesX_anioY { get; set; }
        public string MesX_variacion { get; set; }
        public string MesY_anioX { get; set; }
        public string MesY_anioY { get; set; }
        public string MesY_variacion { get; set; }
        public string Enero_Mes_anioX { get; set; }
        public string Enero_Mes_anioY { get; set; }
        public string Enero_Mes_variacion { get; set; }
        public string Anual_anioX { get; set; }
        public string Anual_anioY { get; set; }
        public string Anual_variacion { get; set; }

        public double ValorAgregado { get; set; }
        public double Peso { get; set; }
    }

    public class ReporteMes
    {
        public int Nro { get; set; }
        public string GlsMes { get; set; }
        public string Abreviatura { get; set; }
        public string Valor { get; set; }

        public long IdCiiu { get; set; }
        public long IdEstablecimiento { get; set; }
        public string CodigoCiiu { get; set; }
        public string CodigoEst { get; set; }
        public string NomEst { get; set; }
    }

    public class EmpresaInfo 
    {
        public int Nro { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Ciiu { get; set; }
        public string Analista { get; set; }
    }

    public class EmpresaEnvioInfo
    {
        public int establecimiento_id { get; set; }
        public string establecimiento_ruc { get; set; }
        public string establecimiento_nombre { get; set; }
        public int ciiu_id { get; set; }
        public string ciiu_codigo { get; set; }
        public int usuario_id { get; set; }
        public string usuario_login { get; set; }
        public int encuesta_id { get; set; }
        public int encuesta_estadoEncuesta { get; set; }
        public DateTime encuesta_fecha { get; set; }
        public int entraCalculo { get; set; }
    }

    public class SegEncuestaAnalista
    {
        public int usuario_id { get; set; }
        public string usuario_login { get; set; }
        public int encuesta_id { get; set; }
        public int encuesta_estadoEncuesta { get; set; }
        public DateTime encuesta_fecha { get; set; }
        public DateTime? encuesta_fecha_ultimo_envio { get; set; }
    }

    public class SegAux
    {
        public decimal IdAnalista { get; set; }
        public string Analista { get; set; }

        public List<string> Cantidad_Enviados { get; set; }
        public List<string> Cantidad_NoEnviados { get; set; }
        public List<string> Cantidad_Consolidados { get; set; }
        public List<string> Cantidad_Total { get; set; }

        public List<string> PorcSegunAnalista_Enviados { get; set; }
        public List<string> PorcSegunAnalista_NoEnviados { get; set; }
        public List<string> PorcSegunAnalista_Consolidados { get; set; }
        public List<string> PorcSegunAnalista_Total { get; set; }

        public List<string> PorcSegunEstado_Enviados { get; set; }
        public List<string> PorcSegunEstado_NoEnviados { get; set; }
        public List<string> PorcSegunEstado_Consolidados { get; set; }
        public List<string> PorcSegunEstado_Total { get; set; }

        public SegAux()
        {
            Cantidad_Enviados = new List<string>();
            Cantidad_NoEnviados = new List<string>();
            Cantidad_Consolidados = new List<string>();
            Cantidad_Total = new List<string>();

            PorcSegunAnalista_Enviados = new List<string>();
            PorcSegunAnalista_NoEnviados = new List<string>();
            PorcSegunAnalista_Consolidados = new List<string>();
            PorcSegunAnalista_Total = new List<string>();

            PorcSegunEstado_Enviados = new List<string>();
            PorcSegunEstado_NoEnviados = new List<string>();
            PorcSegunEstado_Consolidados = new List<string>();
            PorcSegunEstado_Total = new List<string>();
        }
    }

    public class SegAuxTotal
    {
        public List<string> Cantidad_Enviados_Totales { get; set; }
        public List<string> Cantidad_NoEnviados_Totales { get; set; }
        public List<string> Cantidad_Consolidados_Totales { get; set; }
        public List<string> Cantidad_Total_Totales { get; set; }

        public List<string> PorcSegunAnalista_Enviados_Totales { get; set; }
        public List<string> PorcSegunAnalista_NoEnviados_Totales { get; set; }
        public List<string> PorcSegunAnalista_Consolidados_Totales { get; set; }
        public List<string> PorcSegunAnalista_Total_Totales { get; set; }

        public List<string> PorcSegunEstado_Enviados_Totales { get; set; }
        public List<string> PorcSegunEstado_NoEnviados_Totales { get; set; }
        public List<string> PorcSegunEstado_Consolidados_Totales { get; set; }
        public List<string> PorcSegunEstado_Total_Totales { get; set; }

        public SegAuxTotal()
        {
            Cantidad_Enviados_Totales = new List<string>();
            Cantidad_NoEnviados_Totales = new List<string>();
            Cantidad_Consolidados_Totales = new List<string>();
            Cantidad_Total_Totales = new List<string>();
            PorcSegunAnalista_Enviados_Totales = new List<string>();
            PorcSegunAnalista_NoEnviados_Totales = new List<string>();
            PorcSegunAnalista_Consolidados_Totales = new List<string>();
            PorcSegunAnalista_Total_Totales = new List<string>();
            PorcSegunEstado_Enviados_Totales = new List<string>();
            PorcSegunEstado_NoEnviados_Totales = new List<string>();
            PorcSegunEstado_Consolidados_Totales = new List<string>();
            PorcSegunEstado_Total_Totales = new List<string>();
        }
    }

    public class SegAuxModel
    {
        public List<DateTime> Fechas { get; set; }
        public List<SegAux> Seguimiento { get; set; }
        public SegAuxTotal Totales { get; set; }
        public SegAuxModel()
        {
            Fechas = new List<DateTime>();
            Seguimiento = new List<SegAux>();
            Totales = new SegAuxTotal();
        }
    }

    public class CoberturaIvf
    {
        public int ciuu_id_ciiu { get; set; }
        public string ciiu_codigo { get; set; }
        public string establecimiento_ruc { get; set; }
        public string establecimiento_nombre { get; set; }
        public decimal establecimiento_peso { get; set; }
        public decimal establecimiento_peso_llego { get; set; }
    }
    #endregion

    public class DescargaEncuesta
    {
        public string CodigoEstab { get; set; }
        public string NombreEstab { get; set; }
        public string Mes { get; set; }
        public string CodigoCiiu { get; set; }
        public string CodigoProd { get; set; }
        public string NombreProd { get; set; }
        public string UM { get; set; }
        public double ValorUnitario { get; set; }
        public double Existencia { get; set; }
        public double Produccion { get; set; }
        public double VentaPais { get; set; }
        public double VentaExtranjero { get; set; }
        public double OtrosIngresos { get; set; }
    }
}
