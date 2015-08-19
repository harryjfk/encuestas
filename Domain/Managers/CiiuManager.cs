using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class CiiuManager:GenericManager<Ciiu>
    {
        public CiiuManager(GenericRepository<Ciiu> repository, Manager manager) : base(repository, manager)
        {
        }

        public CiiuManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public override OperationResult<Ciiu> Delete(Ciiu element)
        {
            if (element.LineasProducto.Count > 0)
                return new OperationResult<Ciiu>(element) { Errors = new List<string>() { "Hay líneas de producto relacionadas" }, Success = false };
            if (element.Establecimientos.Count > 0)
                return new OperationResult<Ciiu>(element) { Errors = new List<string>() { "Hay establecimientos relacionados" }, Success = false };
            return base.Delete(element);
        }

        public override void Seed()
        {
            #region CIIU LIST
            var ciiuList = new List<Ciiu>()
            {
                new Ciiu() {Codigo = "0111", Nombre = "Cultivo de cereales y otros cultivos n.c.p.",Activado = true},
                new Ciiu()
                {
                    Codigo = "0112",
                    Nombre = "Cultivo de hortalizas y legumbres, especialidades horticolas y productos de vivero",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "0113",
                    Nombre =
                        "Cultivo de frutas, nueces, plantas cuyas hojas o frutas se utilizan para preparar bebidas y especias",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "0121",
                    Nombre =
                        "Cría de ganado vacuno y de ovejas, cabras, caballos, asnos, mulas y burdeganos; cría de ganado lechero",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "0122",
                    Nombre = "Cría de otros animales; elaboración de productos animales n.c.p.",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "0130",
                    Nombre =
                        "Cultivo de productos agricolas en combinación con la cría de animales (explotación mixta)",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "0140",
                    Nombre = "Actividades de servicios agricolas y ganaderos, excepto las actividades veterinarias",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "0150",
                    Nombre =
                        "Caza ordinaría y mediante trampas, y repoblación de animales de caza, incluso las actividades de servicios conexas",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "0200",
                    Nombre = "Silvicultura, extracción de madera y actividades de servicios conexas",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "0500",
                    Nombre =
                        "Pesca, explotación de criaderos de peces y granjas pisciicolas; actividades de servicios relacionadas con la pesca",Activado = true
                },
                new Ciiu() {Codigo = "1010", Nombre = "Extracción y aglomeración de carbon de piedra",Activado = true},
                new Ciiu() {Codigo = "1020", Nombre = "Extracción y aglomeración de lignito",Activado = true},
                new Ciiu() {Codigo = "1030", Nombre = "Extracción y aglomeración de lignito",Activado = true},
                new Ciiu() {Codigo = "1110", Nombre = "Extracción de petróleo crudo y de gas natural",Activado = true},
                new Ciiu()
                {
                    Codigo = "1120",
                    Nombre =
                        "Actividades de servicios relaciónados con la extracción de petróleo y de gas, excepto las actividades de prospección",Activado = true
                },
                new Ciiu() {Codigo = "1200", Nombre = "Extracción de minerales de uranio y torio",Activado = true},
                new Ciiu() {Codigo = "1310", Nombre = "Extracción de minerales de hierro",Activado = true},
                new Ciiu()
                {
                    Codigo = "1320",
                    Nombre = "Extracción de minerales metaliferos no ferrosos, excepto minerales de uranio y torio",Activado = true
                },
                new Ciiu() {Codigo = "1410", Nombre = "Extracción de piedra, arena y arcilla",Activado = true},
                new Ciiu()
                {
                    Codigo = "1421",
                    Nombre = "Extracción de minerales para la fabricación de abonos y productos quimicos",Activado = true
                },
                new Ciiu() {Codigo = "1422", Nombre = "Extracción de sal",Activado = true},
                new Ciiu() {Codigo = "1429", Nombre = "Explotación de otras minas y canteras n.c.p.",Activado = true},
                new Ciiu()
                {
                    Codigo = "1511",
                    Nombre = "Produccion, procesamiento y conservación de carne y de productos carnicos",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "1512",
                    Nombre = "Elaboración y conservación de pescado y productos de pescado",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "1513",
                    Nombre = "Elaboración y conservación de frutas, legumbres y hortalizas",Activado = true
                },
                new Ciiu() {Codigo = "1514", Nombre = "Elaboración de aceite y grasas de origen vegetal y animal",Activado = true},
                new Ciiu() {Codigo = "1515", Nombre = "Elaboración de aceite de pescado",Activado = true},
                new Ciiu() {Codigo = "1520", Nombre = "Elaboración de productos lácteos",Activado = true},
                new Ciiu() {Codigo = "1531", Nombre = "Elaboración de productos de molinería",Activado = true},
                new Ciiu() {Codigo = "1532", Nombre = "Elaboración de almidones y productos derivados del almidon",Activado = true},
                new Ciiu() {Codigo = "1533", Nombre = "Elaboración de alimentos preparados para animales",Activado = true},
                new Ciiu() {Codigo = "1541", Nombre = "Elaboración de productos de panadería",Activado = true},
                new Ciiu() {Codigo = "1542", Nombre = "Elaboración de azúcar",Activado = true},
                new Ciiu()
                {
                    Codigo = "1543",
                    Nombre = "Elaboración de cacao y chocolate y de productos de confitería",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "1544",
                    Nombre = "Elaboración de macarrones, fideos, alcuzcuz y productos farinaceos similares",Activado = true
                },
                new Ciiu() {Codigo = "1549", Nombre = "Elaboración de otros productos alimenticios n.c.p.",Activado = true},
                new Ciiu()
                {
                    Codigo = "1551",
                    Nombre =
                        "Destilacion, rectificación y mezcla de bebidas alcohólicas; producción de alcohol etílico a partir de sustancias fermentadas",Activado = true
                },
                new Ciiu() {Codigo = "1552", Nombre = "Elaboración de vinos",Activado = true},
                new Ciiu() {Codigo = "1553", Nombre = "Elaboración de bebidas malteadas y de malta",Activado = true},
                new Ciiu()
                {
                    Codigo = "1554",
                    Nombre = "Elaboración de bebidas no alcohólicas; producción de aguas minerales",Activado = true
                },
                new Ciiu() {Codigo = "1600", Nombre = "Elaboración de productos de tabaco",Activado = true},
                new Ciiu()
                {
                    Codigo = "1711",
                    Nombre = "Preparación e hilatura de fibras textiles; tejedura de productos textiles",Activado = true
                },
                new Ciiu() {Codigo = "1712", Nombre = "Acabado de productos textiles",Activado = true},
                new Ciiu()
                {
                    Codigo = "1721",
                    Nombre =
                        "Fabricación de artículos confeccionados de materiales textiles, excepto prendas de vestir",Activado = true
                },
                new Ciiu() {Codigo = "1722", Nombre = "Fabricación de tapices y alfombras",Activado = true},
                new Ciiu() {Codigo = "1723", Nombre = "Fabricación de cuerdas, cordeles, bramantes y redes",Activado = true},
                new Ciiu() {Codigo = "1729", Nombre = "Fabricación de otros productos textiles n.c.p.",Activado = true},
                new Ciiu() {Codigo = "1730", Nombre = "Fabricación de tejidos y artículos de punto y ganchillo",Activado = true},
                new Ciiu() {Codigo = "1810", Nombre = "Fabricación de prendas de vestir; excepto prendas de piel",Activado = true},
                new Ciiu() {Codigo = "1820", Nombre = "Adobo y teñido de pieles; fabricación de artículos de piel",Activado = true},
                new Ciiu() {Codigo = "1911", Nombre = "Curtido y adobo de cueros",Activado = true},
                new Ciiu()
                {
                    Codigo = "1912",
                    Nombre =
                        "Fabricación de maletas, bolsos de mano y artículos similares, y de artículos de talabartería y guarnicionería",Activado = true
                },
                new Ciiu() {Codigo = "1920", Nombre = "Fabricación de calzado",Activado = true},
                new Ciiu() {Codigo = "2010", Nombre = "Aserrado y acepilladura de madera",Activado = true},
                new Ciiu()
                {
                    Codigo = "2021",
                    Nombre =
                        "Fabricación de hojas de madera para enchapados; fabricación de tableros contrachapados, tableros laminados, tableros de particulas y otros tableros y paneles",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2022",
                    Nombre = "Fabricación de partes y piezas de carpintería para edificios y construcciones",Activado = true
                },
                new Ciiu() {Codigo = "2023", Nombre = "Fabricación de recipientes de madera",Activado = true},
                new Ciiu()
                {
                    Codigo = "2029",
                    Nombre =
                        "Fabricación de otros productos de madera; fabricación de artículos de corcho, paja y materiales trenzables",Activado = true
                },
                new Ciiu() {Codigo = "2101", Nombre = "Fabricación de pasta de madera, papel y carton",Activado = true},
                new Ciiu()
                {
                    Codigo = "2102",
                    Nombre = "Fabricación de papel y carton ondulado y de envases de papel y carton",Activado = true
                },
                new Ciiu() {Codigo = "2109", Nombre = "Fabricación de otros artículos de papel y carton",Activado = true},
                new Ciiu()
                {
                    Codigo = "2211",
                    Nombre = "Edición de libros, folletos, partituras y otras publicaciones",Activado = true
                },
                new Ciiu() {Codigo = "2212", Nombre = "Edición de periodicos, revistas y publicaciones periodicas",Activado = true},
                new Ciiu() {Codigo = "2213", Nombre = "Edición de grabaciones",Activado = true},
                new Ciiu() {Codigo = "2219", Nombre = "Otras actividades de edición",Activado = true},
                new Ciiu() {Codigo = "2221", Nombre = "Actividades de impresión",Activado = true},
                new Ciiu() {Codigo = "2222", Nombre = "Actividades de servicios relacionados con la impresión",Activado = true},
                new Ciiu() {Codigo = "2230", Nombre = "Reproducción de grabaciones",Activado = true},
                new Ciiu() {Codigo = "2310", Nombre = "Fabricación de productos de hornos de coque",Activado = true},
                new Ciiu() {Codigo = "2320", Nombre = "Fabricación de productos de la refinación del petróleo",Activado = true},
                new Ciiu() {Codigo = "2330", Nombre = "Elaboración de combustible nuclear",Activado = true},
                new Ciiu()
                {
                    Codigo = "2411",
                    Nombre = "Fabricación de sustancias quimicas basicas, excepto abonos y compuestos de nitrogeno",Activado = true
                },
                new Ciiu() {Codigo = "2412", Nombre = "Fabricación de abonos y compuestos de nitrogeno",Activado = true},
                new Ciiu()
                {
                    Codigo = "2413",
                    Nombre = "Fabricación de plasticos en formas primarias y de caucho sintetico",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2421",
                    Nombre = "Fabricación de plagulcidas y otros productos quimicos de uso agropecuario",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2422",
                    Nombre =
                        "Fabricación de pinturas, barnices y productos de revestimiento similares, tintas de imprenta y masillas",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2423",
                    Nombre =
                        "Fabricación de productos farmaceuticos, sustancias quimicas medicinales y productos botanicos",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2424",
                    Nombre =
                        "Fabricación de jabones y detergentes, preparados para limpiar y pulir, perfumes y preparados de tocador",Activado = true
                },
                new Ciiu() {Codigo = "2429", Nombre = "Fabricación de otros productos quimicos n.c.p.",Activado = true},
                new Ciiu() {Codigo = "2430", Nombre = "Fabricación de fibras manufacturadas",Activado = true},
                new Ciiu()
                {
                    Codigo = "2511",
                    Nombre =
                        "Fabricación de cubiertas y camaras de caucho; recauchado y renovación de cubiertas de caucho",Activado = true
                },
                new Ciiu() {Codigo = "2519", Nombre = "Fabricación de otros productos de caucho",Activado = true},
                new Ciiu() {Codigo = "2520", Nombre = "Fabricación de productos de plástico",Activado = true},
                new Ciiu() {Codigo = "2610", Nombre = "Fabricación de vidrio y productos de vidrio",Activado = true},
                new Ciiu()
                {
                    Codigo = "2691",
                    Nombre = "Fabricación de productos de ceramica no refractaría para uso no estructural",Activado = true
                },
                new Ciiu() {Codigo = "2692", Nombre = "Fabricación de productos de ceramica refractaría ",Activado = true},
                new Ciiu()
                {
                    Codigo = "2693",
                    Nombre = "Fabricación de productos de arcilla y ceramica no refractarias para uso estructural",Activado = true
                },
                new Ciiu() {Codigo = "2694", Nombre = "Fabricación de cemento, cal y yeso",Activado = true},
                new Ciiu() {Codigo = "2695", Nombre = "Fabricación de artículos de hormigon, cemento y yeso",Activado = true},
                new Ciiu() {Codigo = "2696", Nombre = "Corte, tallado y acabado de la piedra",Activado = true},
                new Ciiu()
                {
                    Codigo = "2699",
                    Nombre = "Fabricación de otros productos minerales no metalicos n.c.p.",Activado = true
                },
                new Ciiu() {Codigo = "2710", Nombre = "Industrias básicas de hierro y acero",Activado = true},
                new Ciiu()
                {
                    Codigo = "2720",
                    Nombre = "Fabricación de productos primarios de metales preciosos y metales no ferrosos",Activado = true
                },
                new Ciiu() {Codigo = "2731", Nombre = "Fundición de hierro y acero",Activado = true},
                new Ciiu() {Codigo = "2732", Nombre = "Fundición de metales no ferrosos",Activado = true},
                new Ciiu() {Codigo = "2811", Nombre = "Fabricación de productos metalicos para uso estructural",Activado = true},
                new Ciiu() {Codigo = "2812", Nombre = "Fabricación de tanques, depositos y recipientes de metal",Activado = true},
                new Ciiu()
                {
                    Codigo = "2813",
                    Nombre =
                        "Fabricación de generadores de vapor, excepto calderas de agua caliente para calefacción central",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2891",
                    Nombre = "Forja, prensado, estampado y laminado de metales; pulvimetalurgia",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2892",
                    Nombre =
                        "Tratamiento y revestimiento de metales; obras de ingeniería mecanica en general realizadas a cambio de una retribución o por contrata",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2893",
                    Nombre =
                        "Fabricación de artículos de cuchilleria, herramientas de mano y artículos de ferretería",Activado = true
                },
                new Ciiu() {Codigo = "2899", Nombre = "Fabricación de otros productos elaborados de metal n.c.p.",Activado = true},
                new Ciiu()
                {
                    Codigo = "2911",
                    Nombre =
                        "Fabricación de motores y turbinas,excepto motores para aeronaves, vehiculos automotres y motocicletas",Activado = true
                },
                new Ciiu() {Codigo = "2912", Nombre = "Fabricación de bombas, compresores, grifos y valvulas",Activado = true},
                new Ciiu()
                {
                    Codigo = "2913",
                    Nombre = "Fabricación de cojinetes, engranajes, trenes de engranajes y piezas de transmisión.",Activado = true
                },
                new Ciiu() {Codigo = "2914", Nombre = "Fabricación de hornos, hogares y quemadores",Activado = true},
                new Ciiu() {Codigo = "2915", Nombre = "Fabricación de equipo de elevación y manipulación",Activado = true},
                new Ciiu() {Codigo = "2919", Nombre = "Fabricación de otros tipos de maquinaria de uso general",Activado = true},
                new Ciiu() {Codigo = "2921", Nombre = "Fabricación de maquinaria agropecuaría y forestal",Activado = true},
                new Ciiu() {Codigo = "2922", Nombre = "Fabricación de maquinas herramienta",Activado = true},
                new Ciiu() {Codigo = "2923", Nombre = "Fabricación de maquinaria metalúrgica",Activado = true},
                new Ciiu()
                {
                    Codigo = "2924",
                    Nombre =
                        "Fabricación de maquinaria para la explotación de minas y canteras y para obras de construcción",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2925",
                    Nombre = "Fabricación de maquinaria para la elaboración de alimentos, bebidas y tabaco",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "2926",
                    Nombre =
                        "Fabricación de maquinaria para la elaboración de productos textiles, prendas de vestir y cueros",Activado = true
                },
                new Ciiu() {Codigo = "2927", Nombre = "Fabricación de armas y municiones",Activado = true},
                new Ciiu() {Codigo = "2929", Nombre = "Fabricación de otros tipos de maquinaria de uso especial",Activado = true},
                new Ciiu() {Codigo = "2930", Nombre = "Fabricación de aparatos de uso domestico n.c.p.",Activado = true},
                new Ciiu()
                {
                    Codigo = "3000",
                    Nombre =
                        "Fabricación de maquinaria de oficina, fabricación de maquinaria de oficina, contabilidad e informática",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "3110",
                    Nombre = "Fabricación de motores, generadores y transformadores eléctricos",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "3120",
                    Nombre = "Fabricación de aparatos de distribución y control de la energía eléctrica",Activado = true
                },
                new Ciiu() {Codigo = "3130", Nombre = "Fabricación de hilos y cables aislados",Activado = true},
                new Ciiu()
                {
                    Codigo = "3140",
                    Nombre = "Fabricación de acumuladores, de pilas y de baterías primarias",Activado = true
                },
                new Ciiu() {Codigo = "3150", Nombre = "Fabricación de lamparas eléctricas y equipo de iluminación",Activado = true},
                new Ciiu() {Codigo = "3190", Nombre = "Fabricación de otros tipos de equipo eléctrico n.c.p.",Activado = true},
                new Ciiu()
                {
                    Codigo = "3210",
                    Nombre = "Fabricación de tubos y válvulas electrónicas y de otros componentes electrónicos",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "3220",
                    Nombre =
                        "Fabricación de transmisores de radio y televisión y de aparatos para telefonía y telegrafía con hilos.",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "3230",
                    Nombre =
                        "Fabricación de receptores de radio y televisión, aparatos de grabación y reproducción de sonido y video, y productos conexos",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "3311",
                    Nombre = "Fabricación de equipo medico y quirúrgico y de aparatos ortopédicos",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "3312",
                    Nombre =
                        "Fabricación de instrumentos y aparatos para medir, verificar, ensayar, navegar y otros fines, excepto el equipo de control de procesos industriales",Activado = true
                },
                new Ciiu() {Codigo = "3313", Nombre = "Fabricación de equipo de control de procesos industriales",Activado = true},
                new Ciiu() {Codigo = "3320", Nombre = "Fabricación de instrumentos de óptica y equipo fotográfico",Activado = true},
                new Ciiu() {Codigo = "3330", Nombre = "Fabricación de relojes",Activado = true},
                new Ciiu() {Codigo = "3410", Nombre = "Fabricación de vehículos automotores",Activado = true},
                new Ciiu()
                {
                    Codigo = "3420",
                    Nombre =
                        "Fabricación de carrocerías para vehículos automotores; fabricación de remolques y semirremolques",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "3430",
                    Nombre = "Fabricación de partes, piezas y accesorios para vehículos automotores y sus motores",Activado = true
                },
                new Ciiu() {Codigo = "3511", Nombre = "Construcción y reparación de buques ",Activado = true},
                new Ciiu()
                {
                    Codigo = "3512",
                    Nombre = "Construcción y reparación de embarcaciones de recreo y de deporte",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "3520",
                    Nombre = "Fabricación de locomotoras y de material rodante para ferrocarriles y tranvías",Activado = true
                },
                new Ciiu() {Codigo = "3530", Nombre = "Fabricación de aeronaves y naves espaciales",Activado = true},
                new Ciiu() {Codigo = "3591", Nombre = "Fabricación de motocicletas",Activado = true},
                new Ciiu()
                {
                    Codigo = "3592",
                    Nombre = "Fabricación de bicicletas y de sillones de ruedas para inválidos",Activado = true
                },
                new Ciiu() {Codigo = "3599", Nombre = "Fabricación de otros tipos de equipo de transporte n.c.p.",Activado = true},
                new Ciiu() {Codigo = "3610", Nombre = "Fabricación de muebles",Activado = true},
                new Ciiu() {Codigo = "3691", Nombre = "Fabricación de joyas y artículos conexos",Activado = true},
                new Ciiu() {Codigo = "3692", Nombre = "Fabricación de instrumentos de música",Activado = true},
                new Ciiu() {Codigo = "3693", Nombre = "Fabricación de artículos de deporte",Activado = true},
                new Ciiu() {Codigo = "3694", Nombre = "Fabricación de juegos y juguetes",Activado = true},
                new Ciiu() {Codigo = "3699", Nombre = "Otras industrias manufactureras n.c.p.",Activado = true},
                new Ciiu() {Codigo = "3710", Nombre = "Reciclamiento de desperdicios y desechos metálicos",Activado = true},
                new Ciiu() {Codigo = "3720", Nombre = "Reciclamiento de desperdicios y desechos no metálicos",Activado = true},
                new Ciiu() {Codigo = "4010", Nombre = "Generacion, captación y distribución de energía electrica",Activado = true},
                new Ciiu()
                {
                    Codigo = "4020",
                    Nombre = "Fabricación de gas; distribución de combustibles gaseosos por tuberias",Activado = true
                },
                new Ciiu() {Codigo = "4030", Nombre = "Suministro de vapor y agua caliente",Activado = true},
                new Ciiu() {Codigo = "4100", Nombre = "Captacion, depuración y distribución de agua",Activado = true},
                new Ciiu() {Codigo = "4510", Nombre = "Preparación del terreno",Activado = true},
                new Ciiu()
                {
                    Codigo = "4520",
                    Nombre =
                        "Construcción de edificios completos y de partes de edificios; obras de ingeniería civil",Activado = true
                },
                new Ciiu() {Codigo = "4530", Nombre = "Acondicionamiento de edificios",Activado = true},
                new Ciiu() {Codigo = "4540", Nombre = "Terminación de edificios",Activado = true},
                new Ciiu()
                {
                    Codigo = "4550",
                    Nombre = "Alquiler de equipo de construcción y demolición dotado de operarios",Activado = true
                },
                new Ciiu() {Codigo = "5010", Nombre = "Venta de vehiculos automotores",Activado = true},
                new Ciiu() {Codigo = "5020", Nombre = "Mantenimiento y reparación de vehiculos automotores",Activado = true},
                new Ciiu()
                {
                    Codigo = "5030",
                    Nombre = "Venta de partes, piezas y accesorios de vehiculos automotores",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5040",
                    Nombre =
                        "En esta clase se incluyen la venta al por mayor y al por menor de motocicletas y trineos motorizados y la de sus parteas, piezas y accesorios. también se incluyen las actividades de mantenimiento y reparación.",Activado = true
                },
                new Ciiu() {Codigo = "5050", Nombre = "Venta al por menor de combustible para automotores",Activado = true},
                new Ciiu()
                {
                    Codigo = "5110",
                    Nombre = "Venta al por mayor a cambio de una retribución o por contrata",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5121",
                    Nombre = "Venta al por mayor de materias primas agropecuarias y de animales vivos",Activado = true
                },
                new Ciiu() {Codigo = "5122", Nombre = "Venta al por mayor de alimentos, bebidas y tabaco",Activado = true},
                new Ciiu()
                {
                    Codigo = "5131",
                    Nombre = "Venta al por mayor de productos textiles, prendas de vestir y calzado",Activado = true
                },
                new Ciiu() {Codigo = "5139", Nombre = "Venta al por mayor de otros enseres domesticos",Activado = true},
                new Ciiu()
                {
                    Codigo = "5141",
                    Nombre =
                        "Venta al por mayor de combustibles solidos, liquidos y gaseosos y de productos conexos",Activado = true
                },
                new Ciiu() {Codigo = "5142", Nombre = "Venta al por mayor de metales y minerales metaliferos",Activado = true},
                new Ciiu()
                {
                    Codigo = "5143",
                    Nombre =
                        "Venta al por mayor de materiales de contrucción, artículos de ferretería y equipo y materiales de fontanería y calefacción",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5149",
                    Nombre = "Venta al por mayor de otros productos intermedios, desperdicios y desechos",Activado = true
                },
                new Ciiu() {Codigo = "5150", Nombre = "Venta al por mayor de maquinaria, equipo y materiales",Activado = true},
                new Ciiu() {Codigo = "5190", Nombre = "Venta al por mayor de otros productos",Activado = true},
                new Ciiu()
                {
                    Codigo = "5211",
                    Nombre =
                        "Venta al por menor en almacenes no especializados con surtido compuesto principalemente de alimentos, bebidas y tabaco",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5219",
                    Nombre = "Venta al por menor de otros productos en almacenes no especializados",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5220",
                    Nombre = "Venta al por menor de alimentos, bebidas y tabaco en almacenes especializados",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5231",
                    Nombre =
                        "Venta al por menor de productos farmaceuticos y medicinales, cosmeticos y artículos de tocador",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5232",
                    Nombre =
                        "Venta al por menor de productos textiles, prendas de vestir, calzado y artículos de cuero",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5233",
                    Nombre = "Venta al por menor de aparatos, artículos y equipo de uso domestico",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5234",
                    Nombre = "Venta al por menor de artículos de ferreteria, pinturas y productos de vidrio",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "5239",
                    Nombre = "Venta al por menor de otros productos en almacenes especializados",Activado = true
                },
                new Ciiu() {Codigo = "5240", Nombre = "Venta al por menor en almacenes de artículos usados",Activado = true},
                new Ciiu() {Codigo = "5251", Nombre = "Venta al por menor de casas de venta por correo",Activado = true},
                new Ciiu() {Codigo = "5252", Nombre = "Venta al por menor en puestos de venta y mercados",Activado = true},
                new Ciiu()
                {
                    Codigo = "5259",
                    Nombre = "Otros tipos de venta al por menor no realizado en almacenes",Activado = true
                },
                new Ciiu() {Codigo = "5260", Nombre = "Reparación de efectos personales y enseres domesticos",Activado = true},
                new Ciiu() {Codigo = "5270", Nombre = "Venta al por menor no especificado",Activado = true},
                new Ciiu() {Codigo = "5510", Nombre = "Hoteles; campamentos y otros tipos de hospedaje temporal",Activado = true},
                new Ciiu() {Codigo = "5520", Nombre = "Restaurantes, bares y cantinas",Activado = true},
                new Ciiu() {Codigo = "6010", Nombre = "Transporte por vía férrea",Activado = true},
                new Ciiu()
                {
                    Codigo = "6021",
                    Nombre = "Otros tipos de transporte regular de pasajeros por vía terrestre ",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "6022",
                    Nombre = "Otros tipos de transporte no regular de pasajeros por vía terrestre",Activado = true
                },
                new Ciiu() {Codigo = "6023", Nombre = "Transporte de carga por carretera",Activado = true},
                new Ciiu() {Codigo = "6030", Nombre = "Transporte por tuberías",Activado = true},
                new Ciiu() {Codigo = "6110", Nombre = "Transporte marítimo y de cabotaje",Activado = true},
                new Ciiu() {Codigo = "6120", Nombre = "Transporte por vias de navegación interiores",Activado = true},
                new Ciiu() {Codigo = "6210", Nombre = "Transporte regular por vía aérea",Activado = true},
                new Ciiu() {Codigo = "6220", Nombre = "Transporte no regular por vía aérea",Activado = true},
                new Ciiu() {Codigo = "6301", Nombre = "Manipulación de la carga",Activado = true},
                new Ciiu() {Codigo = "6302", Nombre = "Almacenamiento y depósito",Activado = true},
                new Ciiu() {Codigo = "6303", Nombre = "Otras actividades de transportes complementarias",Activado = true},
                new Ciiu()
                {
                    Codigo = "6304",
                    Nombre =
                        "Actividades de agencias de viajes y organizadores de viajes; actividades de asistencía a turistas n.c.p.",Activado = true
                },
                new Ciiu() {Codigo = "6309", Nombre = "Actividades de otras agencias de transporte",Activado = true},
                new Ciiu() {Codigo = "6411", Nombre = "Actividades postales nacionales",Activado = true},
                new Ciiu()
                {
                    Codigo = "6412",
                    Nombre = "Actividades de correo distintas de las actividades postales nacionales",Activado = true
                },
                new Ciiu() {Codigo = "6420", Nombre = "Telecomunicaciones",Activado = true},
                new Ciiu() {Codigo = "6511", Nombre = "Banca central",Activado = true},
                new Ciiu() {Codigo = "6519", Nombre = "Otros tipos de intermediación monetaria",Activado = true},
                new Ciiu() {Codigo = "6591", Nombre = "Arrendamiento financiero",Activado = true},
                new Ciiu() {Codigo = "6592", Nombre = "Otros tipos de crédito",Activado = true},
                new Ciiu() {Codigo = "6599", Nombre = "Otros tipos de intermediación financiera n.c.p.",Activado = true},
                new Ciiu() {Codigo = "6601", Nombre = "Planes de seguros de vida",Activado = true},
                new Ciiu() {Codigo = "6602", Nombre = "Planes de pensiones",Activado = true},
                new Ciiu() {Codigo = "6603", Nombre = "Planes de seguros generales",Activado = true},
                new Ciiu() {Codigo = "6711", Nombre = "Administración de mercados financieros",Activado = true},
                new Ciiu() {Codigo = "6712", Nombre = "Actividades bursátiles",Activado = true},
                new Ciiu()
                {
                    Codigo = "6719",
                    Nombre = "Actividades auxiliares de la intermediación financiera n.c.p.",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "6720",
                    Nombre = "Actividades auxiliares de la financiación de planes de seguros y de pensiones",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "7010",
                    Nombre = "Actividades inmobiliarias realizadas con bienes propios o arrendados",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "7020",
                    Nombre = "Actividades inmobiliarias realizadas a cambio de una retribución o por contrata ",Activado = true
                },
                new Ciiu() {Codigo = "7111", Nombre = "Alquiler de equipo de transporte por vía terrestre ",Activado = true},
                new Ciiu() {Codigo = "7112", Nombre = "Alquiler de equipo de transporte por vía acuática",Activado = true},
                new Ciiu() {Codigo = "7113", Nombre = "Alquiler de equipo de transporte por vía aérea",Activado = true},
                new Ciiu() {Codigo = "7121", Nombre = "Alquiler de maquinaria y equipo agropecuario",Activado = true},
                new Ciiu()
                {
                    Codigo = "7122",
                    Nombre = "Alquiler de maquinaria y equipo de construcción e ingeniería civil",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "7123",
                    Nombre = "Alquiler de maquinaria y equipo de oficina (incluso computadoras)",Activado = true
                },
                new Ciiu() {Codigo = "7129", Nombre = "Alquiler de otros tipos de maquinaria y equipo n.c.p.",Activado = true},
                new Ciiu() {Codigo = "7130", Nombre = "Alquiler de efectos personales y enseres domésticos n.c.p.",Activado = true},
                new Ciiu() {Codigo = "7210", Nombre = "Consultores en equipo de informática",Activado = true},
                new Ciiu()
                {
                    Codigo = "7220",
                    Nombre = "Consultores en programas de informática y suministros de programas de informática",Activado = true
                },
                new Ciiu() {Codigo = "7230", Nombre = "Procesamiento de datos",Activado = true},
                new Ciiu() {Codigo = "7240", Nombre = "Procesamiento de datos",Activado = true},
                new Ciiu()
                {
                    Codigo = "7250",
                    Nombre = "Mantenimiento y reparación de maquinaria de oficina, contabilidad e informática",Activado = true
                },
                new Ciiu() {Codigo = "7290", Nombre = "Otras actividades de informática",Activado = true},
                new Ciiu()
                {
                    Codigo = "7310",
                    Nombre =
                        "Investigaciones y desarrollo experimental en el campo de las ciencias naturales y la ingeniería",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "7320",
                    Nombre =
                        "Investigaciones y desarrollo experimental en el campo de las ciencias sociales y las humanidades",Activado = true
                },
                new Ciiu() {Codigo = "7411", Nombre = "Actividades jurídicas",Activado = true},
                new Ciiu()
                {
                    Codigo = "7412",
                    Nombre =
                        "Actividades de contabilidad, teneduría de libros y auditoría; asesoramiento en matería de impuestos",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "7413",
                    Nombre = "Investigación de mercados y realización de encuestas de opinión pública",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "7414",
                    Nombre = "Actividades de asesoramiento empresarial y en matería de gestión",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "7421",
                    Nombre =
                        "Actividades de arquitectura e ingeniería y actividades conexas de asesoramiento técnico",Activado = true
                },
                new Ciiu() {Codigo = "7422", Nombre = "Ensayos y análisis técnicos",Activado = true},
                new Ciiu() {Codigo = "7430", Nombre = "Publicidad",Activado = true},
                new Ciiu() {Codigo = "7491", Nombre = "Obtención y dotación de personal",Activado = true},
                new Ciiu() {Codigo = "7492", Nombre = "Actividades de investigación y seguridad",Activado = true},
                new Ciiu() {Codigo = "7493", Nombre = "Actividades de limpieza de edificios",Activado = true},
                new Ciiu() {Codigo = "7494", Nombre = "Actividades de fotografía",Activado = true},
                new Ciiu() {Codigo = "7495", Nombre = "Actividades de envase y empaque",Activado = true},
                new Ciiu() {Codigo = "7499", Nombre = "Otras actividades empresariales n.c.p.",Activado = true},
                new Ciiu() {Codigo = "7511", Nombre = "Actividades de la administración pública en general",Activado = true},
                new Ciiu()
                {
                    Codigo = "7512",
                    Nombre =
                        "Regulación de las actividades de organismos que prestan servicios sanitarios, educativos, culturales y otros servicios sociales, excepto servicios de seguridad social.",Activado = true
                },
                new Ciiu() {Codigo = "7513", Nombre = "Regulación y facilitación de la actividad económica",Activado = true},
                new Ciiu()
                {
                    Codigo = "7514",
                    Nombre = "Actividades de servicios auxiliares para la administración pública en general",Activado = true
                },
                new Ciiu() {Codigo = "7521", Nombre = "Relaciones exteriores",Activado = true},
                new Ciiu() {Codigo = "7522", Nombre = "Actividades de defensa",Activado = true},
                new Ciiu()
                {
                    Codigo = "7523",
                    Nombre = "Actividades de mantenimiento del orden público y de seguridad ",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "7530",
                    Nombre = "Actividades de planes de seguridad social de afiliación obligatoria",Activado = true
                },
                new Ciiu() {Codigo = "8010", Nombre = "Enseñanza primaria",Activado = true},
                new Ciiu() {Codigo = "8021", Nombre = "Enseñanza secundaría de formación general",Activado = true},
                new Ciiu() {Codigo = "8022", Nombre = "Enseñanza secundaría de formación técnica y profesional",Activado = true},
                new Ciiu() {Codigo = "8030", Nombre = "Enseñanza superior",Activado = true},
                new Ciiu() {Codigo = "8090", Nombre = "Enseñanza de adultos y otros tipos de enseñanza",Activado = true},
                new Ciiu() {Codigo = "8511", Nombre = "Actividades de hospitales",Activado = true},
                new Ciiu() {Codigo = "8512", Nombre = "Actividades de médicos y odontólogos",Activado = true},
                new Ciiu() {Codigo = "8519", Nombre = "Otras actividades relacionadas con la salud humana",Activado = true},
                new Ciiu() {Codigo = "8520", Nombre = "Actividades veterinarias",Activado = true},
                new Ciiu() {Codigo = "8531", Nombre = "Servicios sociales con alojamiento",Activado = true},
                new Ciiu() {Codigo = "8532", Nombre = "Servicios sociales sin alojamiento",Activado = true},
                new Ciiu()
                {
                    Codigo = "9000",
                    Nombre = "Eliminación de desperdicios y aguas residuales, saneamiento y actividades similares",Activado = true
                },
                new Ciiu() {Codigo = "9111", Nombre = "Actividades de organización empresariales y de empleadores",Activado = true},
                new Ciiu() {Codigo = "9112", Nombre = "Actividades de organizaciones profesionales",Activado = true},
                new Ciiu() {Codigo = "9120", Nombre = "Actividades de sindicatos",Activado = true},
                new Ciiu() {Codigo = "9191", Nombre = "Actividades de organizaciones religiosas",Activado = true},
                new Ciiu() {Codigo = "9192", Nombre = "Actividades de organizaciones políticas",Activado = true},
                new Ciiu() {Codigo = "9199", Nombre = "Actividades de otras asociaciones n.c.p.",Activado = true},
                new Ciiu() {Codigo = "9211", Nombre = "Producción y distribución de filmes y videocintas",Activado = true},
                new Ciiu() {Codigo = "9212", Nombre = "Exhibición de filmes y videocintas",Activado = true},
                new Ciiu() {Codigo = "9213", Nombre = "Actividades de radio y televisión",Activado = true},
                new Ciiu()
                {
                    Codigo = "9214",
                    Nombre = "Actividades teatrales y musicales y otras actividades artísticas",Activado = true
                },
                new Ciiu() {Codigo = "9219", Nombre = "Otras actividades de entretenimiento n.c.p.",Activado = true},
                new Ciiu() {Codigo = "9220", Nombre = "Actividades de agencias de noticias",Activado = true},
                new Ciiu() {Codigo = "9231", Nombre = "Actividades de bibliotecas y archivos",Activado = true},
                new Ciiu()
                {
                    Codigo = "9232",
                    Nombre = "Actividades de museos y preservación de lugares y edificios históricos",Activado = true
                },
                new Ciiu()
                {
                    Codigo = "9233",
                    Nombre = "Actividades de jardines botánicos y zoológicos y de parques nacionales",Activado = true
                },
                new Ciiu() {Codigo = "9241", Nombre = "Actividades deportivas",Activado = true},
                new Ciiu() {Codigo = "9249", Nombre = "Otras actividades de esparcimiento",Activado = true},
                new Ciiu()
                {
                    Codigo = "9301",
                    Nombre = "Lavado y limpieza de prendas de tela y de piel, incluso la limpieza en seco ",Activado = true
                },
                new Ciiu() {Codigo = "9302", Nombre = "Peluquería y otros tratamientos de belleza",Activado = true},
                new Ciiu() {Codigo = "9303", Nombre = "Pompas fúnebres y actividades conexas",Activado = true},
                new Ciiu() {Codigo = "9309", Nombre = "Otras actividades de servicios n.c.p.",Activado = true},
                new Ciiu() {Codigo = "9500", Nombre = "Hogares privados con servicio doméstico",Activado = true},
                new Ciiu() {Codigo = "9900", Nombre = "Organizaciones y órganos extraterritoriales",Activado = true},
                new Ciiu() {Codigo = "9999", Nombre = "Otras actividades no especificadas",Activado = true},
            };

#endregion

            var cont = 1;
            ciiuList.ForEach(t=>t.Id=cont++);
            AddOrUpdate(t => t.Codigo,ciiuList.ToArray());
        }

        public override List<string> Validate(Ciiu element)
        {
            var list=base.Validate(element);
            list.Required(element, t => t.Nombre,"Descripción");
            list.Required(element, t => t.Codigo, "Codigo");
            list.Required(element, t => t.sub_sector, "Sub Sector");
            list.Required(element, t => t.id_metodo_calculo, "Tipo de Cálculo");
            if(element.id_metodo_calculo==0)            
                list.Add("Debe seleccionar un método de cálculo");
            if (element.sub_sector == 0)
                list.Add("Debe seleccionar un sub-sector");
            
            if (element.sub_sector == 2)
            {
                if (element.rubro == 0)
                    list.Add("Debe seleccionar un rubro");               
            }

            list.MaxLength(element, t => t.Codigo,4, "Codigo");
            list.MaxLength(element, t => t.Nombre,255, "Nombre");
            list.MaxLength(element, t => t.Revision, 10, "Revision");

            list.Number(element,t=>t.Codigo,"Código");


            return list;
        }
    }
}
