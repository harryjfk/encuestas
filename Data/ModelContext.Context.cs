﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
    using Entity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Cargo> CAT_CARGO { get; set; }
        public DbSet<Ciiu> CAT_CIIU { get; set; }
        public DbSet<Contacto> CAT_CONTACTO { get; set; }
        public DbSet<Encuesta> CAT_ENCUESTA { get; set; }
        public DbSet<LineaProductoEstablecimiento> CAT_ESTAB_LINEA_PROD { get; set; }
        public DbSet<Establecimiento> CAT_ESTABLECIMIENTO { get; set; }
        public DbSet<LineaProducto> CAT_LINEA_PRODUCTO { get; set; }
        public DbSet<MateriaPropia> CAT_MATERIA_PROPIA { get; set; }
        public DbSet<MateriaTerceros> CAT_MATERIA_TERCEROS { get; set; }
        public DbSet<PosibleRespuesta> CAT_POSIBLE_RESPUESTA { get; set; }
        public DbSet<Pregunta> CAT_PREGUNTA { get; set; }
        public DbSet<TrabajadoresDiasTrabajados> CAT_TRABAJADORES_DIAS_TRAB { get; set; }
        public DbSet<UnidadMedida> CAT_UNIDAD_MEDIDA { get; set; }
        public DbSet<Valor> CAT_VALOR { get; set; }
        public DbSet<ValorProduccion> CAT_VALOR_PROD_MENSUAL { get; set; }
        public DbSet<VentasServicioManufactura> CAT_VENTA_SERV_MANUF { get; set; }
        public DbSet<VentasPaisExtranjero> CAT_VENTAS_PAIS_EXTRANJERO { get; set; }
        public DbSet<VentasProductosEstablecimientos> CAT_VENTAS_PROD_ESTAB { get; set; }
        public DbSet<VolumenProduccion> CAT_VOLUMEN_PRODUCCION { get; set; }
        public DbSet<Rol> SEG_ROL { get; set; }
        public DbSet<Usuario> SEG_USUARIO { get; set; }
        public DbSet<Factor> CAT_FACTOR { get; set; }
        public DbSet<FactorProducccion> CAT_FACTOR_PRODUCCION { get; set; }
        public DbSet<TipoCambio> CAT_TIPO_CAMBIO { get; set; }
        public DbSet<IpmIpp> CAT_IPM_IPP { get; set; }
        public DbSet<ConsumoHarinaFideo> CAT_CONSUMO_HARINA_FIDEO { get; set; }
        public DbSet<ExportacionHarinaTrigo> CAT_EXPORTACION_HARINA_TRIGO { get; set; }
        public DbSet<ImportacionHarinaTrigo> CAT_IMPORTACION_HARINA_TRIGO { get; set; }
        public DbSet<MetodoCalculo> CAT_METODO_CALCULO { get; set; }
        public DbSet<LineaProductoUnidadMedida> CAT_LINEA_PRODTO_UNIDAD_MEDIDA { get; set; }
    }
}
