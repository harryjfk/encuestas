using System.Net.Mime;
using Data;
using Data.Contratos;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Managers
{
    public class Manager
    {
        public string UsuarioAutenticado { get; set; }
        public RolManager Rol { get; set; }
        public UsuarioManager Usuario { get; set; }
        public CiiuManager Ciiu { get; set; }
        public LineaProductoManager LineaProducto { get; set; }
        public FactorManager Factor { get; set; }
        public UnidadMedidaManager UnidadMedida { get; set; }
        public EstablecimientoManager Establecimiento { get; set; }
        public ContactoManager Contacto { get; set; }
        public CargoManager Cargo { get; set; }
        public DepartamentoManager Departamento { get; set; }
        public ProvinciaManager Provincia { get; set; }
        public DistritoManager Distrito { get; set; }
        public UbigeoManager Ubigeo { get; set; }
        public PreguntaManager Pregunta { get; set; }
        public ValorManager Valor { get; set; }
        public PosibleRespuestaManager PosibleRespuesta { get; set; }
        public EncuestaManager Encuesta { get; set; }
        public LineaProductoEstablecimientoManager LineaProductoEstablecimiento { get; set; }
        public EncuestaEmpresarialManager EncuestaEmpresarial { get; set; }
        public EncuestaEstadisticaManager EncuestaEstadistica { get; set; }
        public VolumenProduccionManager VolumenProduccionManager { get; set; }
        public TrabajadoresDiasTrabajadosManager TrabajadoresDiasTrabajadosManager { get; set; }
        public FactorProducccionManager FactorProducccionManager { get; set; }
        public ValorProduccionManager ValorProduccionManager { get; set; }
        public VentasProductosEstablecimientoManager VentasProductosEstablecimientoManager { get; set; }
        public VentasPaisExtranjeroManager VentasPaisExtranjeroManager { get; set; }
        public VentaServicioManufacturaManager VentaServicioManufacturaManager { get; set; }
        public MateriaTercerosManager MateriaTercerosManager { get; set; }
        public MateriaPropiaManager MateriaPropiaManager { get; set; }
        public TipoCambioManager TipoCambioManager { get; set; }
        public IpmIppManager IpmIppManager { get; set; }
        public ImportacionHarinaTrigoManager ImportacionHarinaTrigoManager { get; set; }
        public ExportacionHarinaTrigoManager ExportacionHarinaTrigoManager { get; set; }

        public ConsumoHarinaFideoManager ConsumoHarinaFideoManager { get; set; }

        public Manager(IRepositorioUsuario repositorioUsuario,IRepositorioDepartamento departamentoRepository,IRepositorioProvincia provinciaRepository,IRepositorioDistrito distritoRepository,IRepositorioUbigeo ubigeoRepository)
        {
            var context = new Entities();
            Rol = new RolManager(context, this);
            Usuario = new UsuarioManager(context, this, repositorioUsuario);
            Ciiu = new CiiuManager(context, this);
            LineaProducto = new LineaProductoManager(context, this);
            Factor = new FactorManager(context, this);
            UnidadMedida = new UnidadMedidaManager(context, this);
            Establecimiento = new EstablecimientoManager(context, this);
            Pregunta = new PreguntaManager(context, this);
            Contacto = new ContactoManager(context, this);
            Cargo = new CargoManager(context, this);
            Departamento = new DepartamentoManager(departamentoRepository, this);
            Provincia = new ProvinciaManager(provinciaRepository, this);
            Distrito = new DistritoManager(distritoRepository, this);
            Ubigeo = new UbigeoManager(ubigeoRepository, this);
            LineaProductoEstablecimiento = new LineaProductoEstablecimientoManager(context, this);
            Valor = new ValorManager(context, this);
            Encuesta = new EncuestaManager(context, this);
            PosibleRespuesta = new PosibleRespuestaManager(context, this);
            EncuestaEmpresarial = new EncuestaEmpresarialManager(context, this);
            EncuestaEstadistica = new EncuestaEstadisticaManager(context,this);
            VolumenProduccionManager = new VolumenProduccionManager(context, this);
            TrabajadoresDiasTrabajadosManager = new TrabajadoresDiasTrabajadosManager(context, this);
            FactorProducccionManager = new FactorProducccionManager(context, this);
            ValorProduccionManager = new ValorProduccionManager(context, this);
            VentasProductosEstablecimientoManager = new VentasProductosEstablecimientoManager(context, this);
            VentasPaisExtranjeroManager = new VentasPaisExtranjeroManager(context, this);
            VentaServicioManufacturaManager = new VentaServicioManufacturaManager(context, this);
            MateriaTercerosManager = new MateriaTercerosManager(context, this);
            MateriaPropiaManager = new MateriaPropiaManager(context, this);
            TipoCambioManager = new TipoCambioManager(context, this);
            IpmIppManager = new IpmIppManager(context, this);
            ConsumoHarinaFideoManager = new ConsumoHarinaFideoManager(context, this);
            ImportacionHarinaTrigoManager = new ImportacionHarinaTrigoManager(context, this);
            ExportacionHarinaTrigoManager = new ExportacionHarinaTrigoManager(context, this);
        }

        public void Seed()
        {
            var properties = GetType().GetProperties().Where(t => t.PropertyType.GetMethod("Seed") != null);
            foreach (var propertyInfo in properties)
            {
                propertyInfo.PropertyType.GetMethod("Seed").Invoke(propertyInfo.GetMethod.Invoke(this, null), null);
            }
        }

        public GenericManager<T> GetManager<T>() where T : class
        {
            var property = GetType().GetProperties().FirstOrDefault(t =>
                t.PropertyType.IsSubclassOf(typeof(GenericManager<T>)));
            if (property != null)
                return property.GetMethod.Invoke(this, null) as GenericManager<T>;
            return null;
        }

    }
}
