using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entity
{
    public class EncuestaEstadisticaUploadModel
    {
        public string IdInternoEstablecimiento { get; set; }
        public DateTime Fecha { get; set; }
        public string CodigoCIIU { get; set; }
        public string CodigoLineaProducto { get; set; }
        public string AbreviaturaUM { get; set; }
        public Nullable<decimal> ValorUnitario { get; set; }
        public Nullable<decimal> Existencia { get; set; }
        public Nullable<decimal> Produccion { get; set; }
        public Nullable<decimal> VentasPais { get; set; }
        public Nullable<decimal> VentasExtranjero { get; set; }
        public Nullable<decimal> OtrasSalidas { get; set; }
        public string Materia { get; set; }
    }

    public class EncuestaEstadisticaUploadModel_2
    {
        public string IdInternoEstablecimiento { get; set; }
        public DateTime Fecha { get; set; }
        public string CodigoCIIU { get; set; }
        public Nullable<decimal> ValorTercero { get; set; }
        public Nullable<decimal> ValorVentaPais { get; set; }
        public Nullable<decimal> ValorVentaExtranjero { get; set; }
    }

    public class EncuestaEstadisticaUploadModel_3
    {
        public string IdInternoEstablecimiento { get; set; }
        public DateTime Fecha { get; set; }
        public string CodigoCIIU { get; set; }        
        public Nullable<decimal> VentaPais { get; set; }
        public Nullable<decimal> VentaExtranjero { get; set; }
    }

    public class EncuestaEstadisticaUploadModel_4
    {
        public string IdInternoEstablecimiento { get; set; }
        public DateTime Fecha { get; set; }
        public Int32 DiasTrabajados { get; set; }
        public Int32 Trabajadores { get; set; }
        public Int32 Administrativos { get; set; }
    }

    public class EncuestaEstadisticaUploadModel_5
    {
        public string IdInternoEstablecimiento { get; set; }
        public DateTime Fecha { get; set; }

        public Boolean AumentoDemandaInterna { get; set; }
        public Boolean AumentoCapacidadInstalada { get; set; }
        public Boolean CambiosTecnologicos { get; set; }
        public Boolean CampaniaEstacionalidadProducto { get; set; }
        public Boolean IncrementoExportacion { get; set; }
        public Boolean ReposicionExistencias { get; set; }

        public Boolean CompetenciaDesleal { get; set; }
        public Boolean ContrabandoPirateria { get; set; }
        public Boolean DemandaInternaLimitada { get; set; }
        public Boolean DificultadAccesoFinanciamiento { get; set; }
        public Boolean DificultadAbastecimientoInsumos { get; set; }
        public Boolean DisminucionExportaciones { get; set; }
        public Boolean FaltaCapitalTrabajo { get; set; }
        public Boolean FaltaEnergia { get; set; }
        public Boolean FaltaPersonalCalificado { get; set; }
        public Boolean MantenimientoEquipos { get; set; }
        public Boolean VacacionesColectivas { get; set; }
        public Boolean AltasExistencias { get; set; }
        public Boolean HuelgaParos { get; set; }
    }
}