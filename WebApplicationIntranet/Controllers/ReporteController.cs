using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Entity.Reportes;
using WebApplication.Models;
using Microsoft.Reporting.WebForms;
using WebApplication.App_Code;
using System.Globalization;
using OfficeOpenXml;
using ClosedXML.Excel;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SelectPdf;
using Spire.Xls;


namespace WebApplication.Controllers
{
    public class ReporteController : BaseController<PorcentajeEncuestaEstadistica>
    {
        public ActionResult IVFMensual()
        {
            return View();
        }

        public ActionResult IFVTable(int decimales, int year, int nivel, int inicio, int fin)
        {
            string formatDecimal = "";

            for (int i = 0; i < decimales; ++i)
            {
                formatDecimal += "0";
            }

            bool showSector = true;
            bool showSubsector = true;
            bool showDosDigitos = true;
            bool showTresDigitos = true;
            bool showCuatroDigitos = true;

            if (nivel == 0)
            {
                showSector = true;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 1)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 2)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 3)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 4)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = true;
            }

            if (nivel == 5)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 6)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 7)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = true;
            }

            List<IVFDB> ivfsBD = Manager.ReporteManager.GetVD_VP_IVF(year);

            for (int i = inicio; i <= fin; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year, i, 1)));
            }

            List<IVFM> ivfs = new List<IVFM>();
            ivfs.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            int Id = 4;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];
                Id = Id + i;
                if (ciiu.sub_sector == 2)
                {
                    var ivfm = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);
                }
            }

            ivfs.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));

            Id = Id + 1;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];

                Id = Id + i;
                if (ciiu.sub_sector == 1)
                {
                    var ivfm = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);
                }
            }

            FillPrimarioNoPrimarioAndTotal(ivfs);

            var ivfsCodCiiuNotNull = ivfs.Where(t => t.CodigoCiiu != null);

            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 2, showDosDigitos);
            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 3, showTresDigitos);

            List<IVFMReport> ivfsReport = new List<IVFMReport>();

            foreach (var ivf in ivfs)
            {
                if (ivf.Visible)
                {
                    string espacio = "";
                    for (int i = 0; i < ivf.Level - 1; ++i)
                    {
                        espacio = espacio + "&nbsp;&nbsp;&nbsp;";
                    }

                    espacio = espacio + "- ";

                    IVFMReport ivfr = new IVFMReport();
                    ivfr.Texto = espacio + ivf.Texto;
                    ivfr.Enero = string.Format("{0:0." + formatDecimal + "}", ivf.Enero);
                    ivfr.Febrero = string.Format("{0:0." + formatDecimal + "}", ivf.Febrero);
                    ivfr.Marzo = string.Format("{0:0." + formatDecimal + "}", ivf.Marzo);
                    ivfr.Abril = string.Format("{0:0." + formatDecimal + "}", ivf.Abril);
                    ivfr.Mayo = string.Format("{0:0." + formatDecimal + "}", ivf.Mayo);
                    ivfr.Junio = string.Format("{0:0." + formatDecimal + "}", ivf.Junio);
                    ivfr.Julio = string.Format("{0:0." + formatDecimal + "}", ivf.Julio);
                    ivfr.Agosto = string.Format("{0:0." + formatDecimal + "}", ivf.Agosto);
                    ivfr.Setiembre = string.Format("{0:0." + formatDecimal + "}", ivf.Setiembre);
                    ivfr.Octubre = string.Format("{0:0." + formatDecimal + "}", ivf.Octubre);
                    ivfr.Noviembre = string.Format("{0:0." + formatDecimal + "}", ivf.Noviembre);
                    ivfr.Diciembre = string.Format("{0:0." + formatDecimal + "}", ivf.Diciembre);

                    ivfsReport.Add(ivfr);
                }
            }

            List<int> listMonths = new List<int>();
            for (int i = inicio; i <= fin; ++i)
            {
                listMonths.Add(i);
            }

            ViewBag.listMonths = listMonths;


            return PartialView("_IVFTable", ivfsReport);
        }

        private List<IVFM> GetIVFMensualPorAnio3digitos(int year, int inicio, int fin)
        {
            int decimales = 2;
            string formatDecimal = "";

            for (int i = 0; i < decimales; ++i)
            {
                formatDecimal += "0";
            }

            bool showSector = true;
            bool showSubsector = true;
            bool showDosDigitos = true;
            bool showTresDigitos = true;
            bool showCuatroDigitos = false;

            List<IVFDB> ivfsBD = Manager.ReporteManager.GetVD_VP_IVF(year);

            for (int i = inicio; i <= fin; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year, i, 1)));
            }

            List<IVFM> ivfs = new List<IVFM>();
            ivfs.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            int Id = 4;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];
                Id = Id + i;
                if (ciiu.sub_sector == 2)
                {
                    var ivfm = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);
                }
            }

            ivfs.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));

            Id = Id + 1;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];

                Id = Id + i;
                if (ciiu.sub_sector == 1)
                {
                    var ivfm = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);
                }
            }

            FillPrimarioNoPrimarioAndTotal(ivfs);

            var ivfsCodCiiuNotNull = ivfs.Where(t => t.CodigoCiiu != null);

            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 2, showDosDigitos);
            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 3, showTresDigitos);
            return ivfs;
        }

        private void FillCodPorDigitos(List<IVFM> ivfs, IEnumerable<IVFM> ivfsCodCiiuNotNull, int digitos, bool visible)
        {
            var cods = ivfsCodCiiuNotNull.GroupBy(t => new { y = t.CodigoCiiu.Substring(0, digitos), t.ParentId })
                .Select(grp => new {
                    ciiu = grp.First().CodigoCiiu.Substring(0, digitos),
                    parentId = grp.First().ParentId
                }).ToList();

            for (int i = 0; i < cods.Count; ++i)
            {
                var cod = cods[i];

                var ivfsCiius = ivfsCodCiiuNotNull.Where(t => t.CodigoCiiu != null).Where(t => t.CodigoCiiu.Substring(0, digitos) == cod.ciiu && t.ParentId == cod.parentId);
                var codCiiuMin = ivfsCiius.Min(t => int.Parse(t.CodigoCiiu));

                int inxInsert = ivfs.FindIndex(t => t.CodigoCiiu != null && int.Parse(t.CodigoCiiu) == codCiiuMin && t.ParentId == cod.parentId);

                var ivfm = new IVFM(-1, -1, digitos + 1, cods[i].ciiu);
                ivfm.Enero = ivfsCiius.Sum(t => t.Enero);
                ivfm.Febrero = ivfsCiius.Sum(t => t.Febrero);
                ivfm.Marzo = ivfsCiius.Sum(t => t.Marzo);
                ivfm.Abril = ivfsCiius.Sum(t => t.Abril);
                ivfm.Mayo = ivfsCiius.Sum(t => t.Mayo);
                ivfm.Junio = ivfsCiius.Sum(t => t.Junio);
                ivfm.Julio = ivfsCiius.Sum(t => t.Julio);
                ivfm.Agosto = ivfsCiius.Sum(t => t.Agosto);
                ivfm.Setiembre = ivfsCiius.Sum(t => t.Setiembre);
                ivfm.Octubre = ivfsCiius.Sum(t => t.Octubre);
                ivfm.Noviembre = ivfsCiius.Sum(t => t.Noviembre);
                ivfm.Diciembre = ivfsCiius.Sum(t => t.Diciembre);
                ivfm.Visible = visible;
                ivfs.Insert(inxInsert, ivfm);
            }
        }

        private void FillPrimarioNoPrimarioAndTotal(List<IVFM> ivfs)
        {
            var ivfNoPrimario = ivfs.Where(t => t.Id == 2).FirstOrDefault();
            var ivfPrimario = ivfs.Where(t => t.Id == 3).FirstOrDefault();
            var ivfTotal = ivfs.Where(t => t.Id == 1).FirstOrDefault();

            List<IVFM> ivfsNoPrimario = ivfs.Where(t => t.ParentId == 2).ToList();
            List<IVFM> ivfsPrimario = ivfs.Where(t => t.ParentId == 3).ToList();

            ivfNoPrimario.Enero = ivfsNoPrimario.Sum(t => t.Enero);
            ivfNoPrimario.Febrero = ivfsNoPrimario.Sum(t => t.Febrero);
            ivfNoPrimario.Marzo = ivfsNoPrimario.Sum(t => t.Marzo);
            ivfNoPrimario.Abril = ivfsNoPrimario.Sum(t => t.Abril);
            ivfNoPrimario.Mayo = ivfsNoPrimario.Sum(t => t.Mayo);
            ivfNoPrimario.Junio = ivfsNoPrimario.Sum(t => t.Junio);
            ivfNoPrimario.Julio = ivfsNoPrimario.Sum(t => t.Julio);
            ivfNoPrimario.Agosto = ivfsNoPrimario.Sum(t => t.Agosto);
            ivfNoPrimario.Setiembre = ivfsNoPrimario.Sum(t => t.Setiembre);
            ivfNoPrimario.Octubre = ivfsNoPrimario.Sum(t => t.Octubre);
            ivfNoPrimario.Noviembre = ivfsNoPrimario.Sum(t => t.Noviembre);
            ivfNoPrimario.Diciembre = ivfsNoPrimario.Sum(t => t.Diciembre);

            ivfPrimario.Enero = ivfsPrimario.Sum(t => t.Enero);
            ivfPrimario.Febrero = ivfsPrimario.Sum(t => t.Febrero);
            ivfPrimario.Marzo = ivfsPrimario.Sum(t => t.Marzo);
            ivfPrimario.Abril = ivfsPrimario.Sum(t => t.Abril);
            ivfPrimario.Mayo = ivfsPrimario.Sum(t => t.Mayo);
            ivfPrimario.Junio = ivfsPrimario.Sum(t => t.Junio);
            ivfPrimario.Julio = ivfsPrimario.Sum(t => t.Julio);
            ivfPrimario.Agosto = ivfsPrimario.Sum(t => t.Agosto);
            ivfPrimario.Setiembre = ivfsPrimario.Sum(t => t.Setiembre);
            ivfPrimario.Octubre = ivfsPrimario.Sum(t => t.Octubre);
            ivfPrimario.Noviembre = ivfsPrimario.Sum(t => t.Noviembre);
            ivfPrimario.Diciembre = ivfsPrimario.Sum(t => t.Diciembre);

            ivfTotal.Enero = ivfNoPrimario.Enero + ivfPrimario.Enero;
            ivfTotal.Febrero = ivfNoPrimario.Febrero + ivfPrimario.Febrero;
            ivfTotal.Marzo = ivfNoPrimario.Marzo + ivfPrimario.Marzo;
            ivfTotal.Abril = ivfNoPrimario.Abril + ivfPrimario.Abril;
            ivfTotal.Mayo = ivfNoPrimario.Mayo + ivfPrimario.Mayo;
            ivfTotal.Junio = ivfNoPrimario.Junio + ivfPrimario.Junio;
            ivfTotal.Julio = ivfNoPrimario.Julio + ivfPrimario.Julio;
            ivfTotal.Agosto = ivfNoPrimario.Agosto + ivfPrimario.Agosto;
            ivfTotal.Setiembre = ivfNoPrimario.Setiembre + ivfPrimario.Setiembre;
            ivfTotal.Octubre = ivfNoPrimario.Octubre + ivfPrimario.Octubre;
            ivfTotal.Noviembre = ivfNoPrimario.Noviembre + ivfPrimario.Noviembre;
            ivfTotal.Diciembre = ivfNoPrimario.Diciembre + ivfPrimario.Diciembre;
        }

        private void FillIVFMWithIVFDB(List<IVFDB> ivfsBD, IVFM ivfm, int year)
        {
            foreach (var ivfbd in ivfsBD.Where(t => t.IdCiiu == ivfm.IdCiiu))
            {
                if (new DateTime(year, 1, 1) == ivfbd.Fecha)
                    ivfm.Enero = ivfbd.IVF;
                if (new DateTime(year, 2, 1) == ivfbd.Fecha)
                    ivfm.Febrero = ivfbd.IVF;
                if (new DateTime(year, 3, 1) == ivfbd.Fecha)
                    ivfm.Marzo = ivfbd.IVF;
                if (new DateTime(year, 4, 1) == ivfbd.Fecha)
                    ivfm.Abril = ivfbd.IVF;
                if (new DateTime(year, 5, 1) == ivfbd.Fecha)
                    ivfm.Mayo = ivfbd.IVF;
                if (new DateTime(year, 6, 1) == ivfbd.Fecha)
                    ivfm.Junio = ivfbd.IVF;
                if (new DateTime(year, 7, 1) == ivfbd.Fecha)
                    ivfm.Julio = ivfbd.IVF;
                if (new DateTime(year, 8, 1) == ivfbd.Fecha)
                    ivfm.Agosto = ivfbd.IVF;
                if (new DateTime(year, 9, 1) == ivfbd.Fecha)
                    ivfm.Setiembre = ivfbd.IVF;
                if (new DateTime(year, 10, 1) == ivfbd.Fecha)
                    ivfm.Octubre = ivfbd.IVF;
                if (new DateTime(year, 11, 1) == ivfbd.Fecha)
                    ivfm.Noviembre = ivfbd.IVF;
                if (new DateTime(year, 12, 1) == ivfbd.Fecha)
                    ivfm.Diciembre = ivfbd.IVF;

                ivfm.ValorAgregado = ivfbd.valor_agregado;
                ivfm.Peso = ivfbd.peso;
            }
        }

        public ActionResult IVFPorEstablecimiento()
        {
            return View();
        }

        public ActionResult IVFPorEstablecimientoTable(int year, int inicio, int fin)
        {
            List<IVFEstDB> ivfsBD = Manager.ReporteManager.GetEMP_VD_VP_IVF(year);

            for (int i = inicio; i <= fin; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetEMPCA_IVF(new DateTime(year, i, 1)));
            }

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            List<Establecimiento> establecimientos = Manager.Establecimiento.GetByFilter().OrderBy(t => t.Id).ToList();

            List<IVFMEstReport> ivfsReport = new List<IVFMEstReport>();

            foreach (var ivf in ivfsBD)
            {
                bool existe = ivfsReport.Where(t => t.IdCiiu == ivf.IdCiiu && t.IdEstablecimiento == ivf.IdEstablecimiento).FirstOrDefault() != null;
                if (existe == false)
                {
                    IVFMEstReport ivfr = new IVFMEstReport();

                    ivfr.IdCiiu = ivf.IdCiiu;
                    ivfr.IdEstablecimiento = ivf.IdEstablecimiento;

                    var estab = establecimientos.Where(t => t.Id == ivf.IdEstablecimiento).FirstOrDefault();
                    ivfr.CodigoCiiu = ciius.Where(t => t.Id == ivf.IdCiiu).FirstOrDefault().Codigo;
                    ivfr.CodigoEst = estab.IdentificadorInterno;
                    ivfr.NomEst = estab.Nombre;

                    FillIVFMPorEstabWithIVFDB(ivfsBD, ivfr, year);

                    ivfsReport.Add(ivfr);
                }
            }

            List<int> listMonths = new List<int>();
            for (int i = inicio; i <= fin; ++i)
            {
                listMonths.Add(i);
            }

            ViewBag.listMonths = listMonths;

            return PartialView("_IVFPorEstabTable", ivfsReport);
        }

        #region Elmer
        private List<IVFMEstReport> GetIVFporAnio(int year)
        {
            List<IVFEstDB> ivfsBD = Manager.ReporteManager.GetEMP_VD_VP_IVF(year);

            for (int i = 1; i <= 12; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetEMPCA_IVF(new DateTime(year, i, 1)));
            }

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            List<Establecimiento> establecimientos = Manager.Establecimiento.GetByFilter().OrderBy(t => t.Id).ToList();

            List<IVFMEstReport> ivfsReport = new List<IVFMEstReport>();

            foreach (var ivf in ivfsBD)
            {
                bool existe = ivfsReport.Where(t => t.IdCiiu == ivf.IdCiiu && t.IdEstablecimiento == ivf.IdEstablecimiento).FirstOrDefault() != null;
                if (existe == false)
                {
                    IVFMEstReport ivfr = new IVFMEstReport();

                    ivfr.IdCiiu = ivf.IdCiiu;
                    ivfr.IdEstablecimiento = ivf.IdEstablecimiento;

                    var estab = establecimientos.Where(t => t.Id == ivf.IdEstablecimiento).FirstOrDefault();
                    ivfr.CodigoCiiu = ciius.Where(t => t.Id == ivf.IdCiiu).FirstOrDefault().Codigo;
                    ivfr.CodigoEst = estab.IdentificadorInterno;
                    ivfr.NomEst = estab.Nombre;

                    FillIVFMPorEstabWithIVFDB(ivfsBD, ivfr, year);

                    ivfsReport.Add(ivfr);
                }
            }

            return ivfsReport;
        }

        public ActionResult IVFPorEstablecimientoVariacion()
        {
            return View();
        }

        public ActionResult IVFPorEstablecimientoVariacionTable2(int year, int inicio, int fin, int ciiu)
        {
            var ivfsYearX = GetIVFporAnio(year);
            var ivfsYearY = GetIVFporAnio(year - 1);
            if (ciiu > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
            }
            List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

            foreach (var ivfX in ivfsYearX)
            {
                setDefaultIVFMEstReport(ivfX);
                foreach (var ivfY in ivfsYearY)
                {
                    setDefaultIVFMEstReport(ivfY);
                    if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
                    {
                        IVFMEstVarReport rep = new IVFMEstVarReport();
                        rep.IdCiiu = ivfX.IdCiiu;
                        rep.IdEstablecimiento = ivfX.IdEstablecimiento;
                        rep.CodigoCiiu = ivfX.CodigoCiiu;
                        rep.CodigoEst = ivfX.CodigoEst;
                        rep.NomEst = ivfX.NomEst;
                        rep.Meses = new List<MesVariacion>();
                        for (int i = inicio; i <= fin; i++)
                        {
                            MesVariacion mv = new MesVariacion();
                            mv.Nro = i;
                            setMesVar(mv, ivfX, ivfY, i);
                            rep.Meses.Add(mv);
                        }
                        reps.Add(rep);
                    }
                }
            }
            ViewBag.MesX = year;
            ViewBag.MesY = year - 1;
            return PartialView("_IVFPorEstabVarTable", reps);
        }

        private void setMesVar(MesVariacion mv, IVFMEstReport ivfX, IVFMEstReport ivfY, int mes)
        {
            switch (mes)
            {
                case 1: { mv.MesX = ivfX.Enero; mv.MesY = ivfY.Enero; mv.VariacionPorcentual = getVariacionStr(ivfX.Enero, ivfY.Enero); } break;
                case 2: { mv.MesX = ivfX.Febrero; mv.MesY = ivfY.Febrero; mv.VariacionPorcentual = getVariacionStr(ivfX.Febrero, ivfY.Febrero); } break;
                case 3: { mv.MesX = ivfX.Marzo; mv.MesY = ivfY.Marzo; mv.VariacionPorcentual = getVariacionStr(ivfX.Marzo, ivfY.Marzo); } break;
                case 4: { mv.MesX = ivfX.Abril; mv.MesY = ivfY.Abril; mv.VariacionPorcentual = getVariacionStr(ivfX.Abril, ivfY.Abril); } break;
                case 5: { mv.MesX = ivfX.Mayo; mv.MesY = ivfY.Mayo; mv.VariacionPorcentual = getVariacionStr(ivfX.Mayo, ivfY.Mayo); } break;
                case 6: { mv.MesX = ivfX.Junio; mv.MesY = ivfY.Junio; mv.VariacionPorcentual = getVariacionStr(ivfX.Junio, ivfY.Junio); } break;
                case 7: { mv.MesX = ivfX.Julio; mv.MesY = ivfY.Julio; mv.VariacionPorcentual = getVariacionStr(ivfX.Julio, ivfY.Julio); } break;
                case 8: { mv.MesX = ivfX.Agosto; mv.MesY = ivfY.Agosto; mv.VariacionPorcentual = getVariacionStr(ivfX.Agosto, ivfY.Agosto); } break;
                case 9: { mv.MesX = ivfX.Setiembre; mv.MesY = ivfY.Setiembre; mv.VariacionPorcentual = getVariacionStr(ivfX.Setiembre, ivfY.Setiembre); } break;
                case 10: { mv.MesX = ivfX.Octubre; mv.MesY = ivfY.Octubre; mv.VariacionPorcentual = getVariacionStr(ivfX.Octubre, ivfY.Octubre); } break;
                case 11: { mv.MesX = ivfX.Noviembre; mv.MesY = ivfY.Noviembre; mv.VariacionPorcentual = getVariacionStr(ivfX.Noviembre, ivfY.Noviembre); } break;
                case 12: { mv.MesX = ivfX.Diciembre; mv.MesY = ivfY.Diciembre; mv.VariacionPorcentual = getVariacionStr(ivfX.Diciembre, ivfY.Diciembre); } break;
                default:
                    break;
            }
        }

        private void setDefaultIVFMEstReport(IVFMEstReport rep)
        {
            rep.Enero = string.IsNullOrEmpty(rep.Enero) ? "0.00" : rep.Enero;
            rep.Febrero = string.IsNullOrEmpty(rep.Febrero) ? "0.00" : rep.Febrero;
            rep.Marzo = string.IsNullOrEmpty(rep.Marzo) ? "0.00" : rep.Marzo;
            rep.Abril = string.IsNullOrEmpty(rep.Abril) ? "0.00" : rep.Abril;
            rep.Mayo = string.IsNullOrEmpty(rep.Mayo) ? "0.00" : rep.Mayo;
            rep.Junio = string.IsNullOrEmpty(rep.Junio) ? "0.00" : rep.Junio;
            rep.Julio = string.IsNullOrEmpty(rep.Julio) ? "0.00" : rep.Julio;
            rep.Agosto = string.IsNullOrEmpty(rep.Agosto) ? "0.00" : rep.Agosto;
            rep.Setiembre = string.IsNullOrEmpty(rep.Setiembre) ? "0.00" : rep.Setiembre;
            rep.Octubre = string.IsNullOrEmpty(rep.Octubre) ? "0.00" : rep.Octubre;
            rep.Noviembre = string.IsNullOrEmpty(rep.Noviembre) ? "0.00" : rep.Noviembre;
            rep.Diciembre = string.IsNullOrEmpty(rep.Diciembre) ? "0.00" : rep.Diciembre;
        }

        public ActionResult IVFMensualVariacion()
        {
            return View();
        }

        public ActionResult IVFVariacionTable(int decimales, int year, int nivel, int inicio, int fin)
        {
            string formatDecimal = "";

            for (int i = 0; i < decimales; ++i)
            {
                formatDecimal += "0";
            }

            bool showSector = true;
            bool showSubsector = true;
            bool showDosDigitos = true;
            bool showTresDigitos = true;
            bool showCuatroDigitos = true;

            if (nivel == 0)
            {
                showSector = true;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 1)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 2)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 3)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 4)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = true;
            }

            if (nivel == 5)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 6)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 7)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = true;
            }

            List<IVFDB> ivfsBD = Manager.ReporteManager.GetVD_VP_IVF(year);
            List<IVFDB> ivfsBD2 = Manager.ReporteManager.GetVD_VP_IVF(year - 1);

            for (int i = inicio; i <= fin; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year, i, 1)));
                ivfsBD2.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year - 1, i, 1)));
            }

            List<IVFM> ivfs = new List<IVFM>();
            ivfs.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<IVFM> ivfs2 = new List<IVFM>();
            ivfs2.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs2.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            int Id = 4;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];
                Id = Id + i;
                if (ciiu.sub_sector == 2)
                {
                    var ivfm = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);

                    var ivfm2 = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm2.IdCiiu = ciiu.Id;
                    ivfm2.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD2, ivfm2, year - 1);
                    ivfs2.Add(ivfm2);
                }
            }

            ivfs.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));
            ivfs2.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));

            Id = Id + 1;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];

                Id = Id + i;
                if (ciiu.sub_sector == 1)
                {
                    var ivfm = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);

                    var ivfm2 = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm2.IdCiiu = ciiu.Id;
                    ivfm2.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD2, ivfm2, year - 1);
                    ivfs2.Add(ivfm2);
                }
            }

            FillPrimarioNoPrimarioAndTotal(ivfs);
            FillPrimarioNoPrimarioAndTotal(ivfs2);

            var ivfsCodCiiuNotNull = ivfs.Where(t => t.CodigoCiiu != null);
            var ivfsCodCiiuNotNull2 = ivfs2.Where(t => t.CodigoCiiu != null);

            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 2, showDosDigitos);
            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 3, showTresDigitos);

            FillCodPorDigitos(ivfs2, ivfsCodCiiuNotNull2, 2, showDosDigitos);
            FillCodPorDigitos(ivfs2, ivfsCodCiiuNotNull2, 3, showTresDigitos);

            List<IVFMReport> ivfsReport = new List<IVFMReport>();
            List<IVFMReport> ivfsReport2 = new List<IVFMReport>();

            //foreach (var ivf in ivfs)
            //{
            //    if (ivf.Visible)
            //    {
            //        string espacio = "";
            //        for (int i = 0; i < ivf.Level - 1; ++i)
            //        {
            //            espacio = espacio + "&nbsp;&nbsp;&nbsp;";
            //        }

            //        espacio = espacio + "- ";

            //        IVFMReport ivfr = new IVFMReport();
            //        ivfr.Texto = espacio + ivf.Texto;
            //        ivfr.Enero = string.Format("{0:0." + formatDecimal + "}", ivf.Enero);
            //        ivfr.Febrero = string.Format("{0:0." + formatDecimal + "}", ivf.Febrero);
            //        ivfr.Marzo = string.Format("{0:0." + formatDecimal + "}", ivf.Marzo);
            //        ivfr.Abril = string.Format("{0:0." + formatDecimal + "}", ivf.Abril);
            //        ivfr.Mayo = string.Format("{0:0." + formatDecimal + "}", ivf.Mayo);
            //        ivfr.Junio = string.Format("{0:0." + formatDecimal + "}", ivf.Junio);
            //        ivfr.Julio = string.Format("{0:0." + formatDecimal + "}", ivf.Julio);
            //        ivfr.Agosto = string.Format("{0:0." + formatDecimal + "}", ivf.Agosto);
            //        ivfr.Setiembre = string.Format("{0:0." + formatDecimal + "}", ivf.Setiembre);
            //        ivfr.Octubre = string.Format("{0:0." + formatDecimal + "}", ivf.Octubre);
            //        ivfr.Noviembre = string.Format("{0:0." + formatDecimal + "}", ivf.Noviembre);
            //        ivfr.Diciembre = string.Format("{0:0." + formatDecimal + "}", ivf.Diciembre);

            //        ivfsReport.Add(ivfr);
            //    }
            //}

            for (int i = 0; i < ivfs.Count; i++)
            {
                if (ivfs[i].Visible)
                {
                    string espacio = "";
                    for (int j = 0; j < ivfs[i].Level - 1; ++j)
                    {
                        espacio = espacio + "&nbsp;&nbsp;&nbsp;";
                    }

                    espacio = espacio + "- ";

                    IVFMReport ivfr = new IVFMReport();
                    ivfr.Texto = espacio + ivfs[i].Texto;

                    ivfr.Enero = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Enero, ivfs2[i].Enero));
                    ivfr.Febrero = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Febrero, ivfs2[i].Febrero));
                    ivfr.Marzo = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Marzo, ivfs2[i].Marzo));
                    ivfr.Abril = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Abril, ivfs2[i].Abril));
                    ivfr.Mayo = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Mayo, ivfs2[i].Mayo));
                    ivfr.Junio = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Junio, ivfs2[i].Junio));
                    ivfr.Julio = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Julio, ivfs2[i].Julio));
                    ivfr.Agosto = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Agosto, ivfs2[i].Agosto));
                    ivfr.Setiembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Setiembre, ivfs2[i].Setiembre));
                    ivfr.Octubre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Octubre, ivfs2[i].Octubre));
                    ivfr.Noviembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Noviembre, ivfs2[i].Noviembre));
                    ivfr.Diciembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Enero, ivfs2[i].Diciembre));

                    ivfsReport.Add(ivfr);
                }
            }

            List<int> listMonths = new List<int>();
            for (int i = inicio; i <= fin; ++i)
            {
                listMonths.Add(i);
            }

            ViewBag.listMonths = listMonths;

            return PartialView("_IVFVariacionTable", ivfsReport);
        }

        public ActionResult IVFPorEstablecimientoVariacionMes()
        {
            return View();
        }

        //public ActionResult IVFPorEstablecimientoVariacionTableMes(int year, int month, int ciiu)
        //{
        //    var ivfsYearX = GetIVFporAnio(year);
        //    var ivfsYearY = GetIVFporAnio(year - 1);
        //    if (ciiu > -1)
        //    {
        //        ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
        //        ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
        //    }
        //    List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

        //    foreach (var ivfX in ivfsYearX)
        //    {
        //        setDefaultIVFMEstReport(ivfX);
        //        foreach (var ivfY in ivfsYearY)
        //        {
        //            setDefaultIVFMEstReport(ivfY);
        //            if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
        //            {
        //                IVFMEstVarReport rep = new IVFMEstVarReport();
        //                rep.IdCiiu = ivfX.IdCiiu;
        //                rep.IdEstablecimiento = ivfX.IdEstablecimiento;
        //                rep.CodigoCiiu = ivfX.CodigoCiiu;
        //                rep.CodigoEst = ivfX.CodigoEst;
        //                rep.NomEst = ivfX.NomEst;
        //                rep.Meses = new List<MesVariacion>();
        //                MesVariacion mv = new MesVariacion();
        //                mv.Nro = month;
        //                setMesVar(mv, ivfX, ivfY, month);
        //                rep.Meses.Add(mv);
        //                reps.Add(rep);
        //            }
        //        }
        //    }
        //    ViewBag.MesX = year;
        //    ViewBag.MesY = year - 1;

        //    return PartialView("_IVFPorEstabVarTableMes", reps);
        //}

        public ActionResult IVFPorEstablecimientoVariacionTableMes(int year, int month, int ciiu, int estb)
        {
            var ivfsYearX = GetIVFporAnio(year);
            var ivfsYearY = GetIVFporAnio(year - 1);
            if (ciiu > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
            }
            //estb
            if (estb > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdEstablecimiento == estb).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdEstablecimiento == estb).ToList();
            }

            List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

            foreach (var ivfX in ivfsYearX)
            {
                setDefaultIVFMEstReport(ivfX);
                foreach (var ivfY in ivfsYearY)
                {
                    setDefaultIVFMEstReport(ivfY);
                    if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
                    {
                        IVFMEstVarReport rep = new IVFMEstVarReport();
                        rep.IdCiiu = ivfX.IdCiiu;
                        rep.IdEstablecimiento = ivfX.IdEstablecimiento;
                        rep.CodigoCiiu = ivfX.CodigoCiiu;
                        rep.CodigoEst = ivfX.CodigoEst;
                        rep.NomEst = ivfX.NomEst;
                        rep.Meses = new List<MesVariacion>();
                        MesVariacion mv = new MesVariacion();
                        mv.Nro = month;
                        setMesVar(mv, ivfX, ivfY, month);
                        rep.Meses.Add(mv);
                        reps.Add(rep);
                    }
                }
            }
            ViewBag.MesX = year;
            ViewBag.MesY = year - 1;

            return PartialView("_IVFPorEstabVarTableMes", reps);
        }

        private double getVariacionIVF(double x, double y)
        {
            if (x == 0)
            {
                return 0;
            }
            return ((x - y) * 100) / x;
        }

        private string getVariacionIVFstr(string x, string y)
        {
            double valorX;
            if (double.TryParse(y, out valorX))
            {
                double valorY;
                if (double.TryParse(y, out valorY))
                {
                    return (((valorX - valorY) * 100) / valorX).ToString();
                }
                return x;
            }
            return string.Empty;
        }

        private string getVariacionStr(string x, string y)
        {
            if (string.IsNullOrEmpty(x))
            {
                x = "0.00";
            }
            if (string.IsNullOrEmpty(y))
            {
                y = "0.00";
            }
            if (x == "0.00" && y == "0.00")
            {
                return "0.00";
            }
            if (y == "0.00")
            {
                return "100.00";
            }
            double valorX;
            if (double.TryParse(y, out valorX))
            {
                double valorY;
                if (double.TryParse(y, out valorY) == false)
                {
                    valorY = 0.00;
                }
                if (valorX > 0)
                {
                    return (((valorX - valorY) * 100) / valorX).ToString();
                }
                return y;
            }
            return string.Empty;
        }

        public ActionResult CobeturaIVF()
        {
            return View();
        }

        public ActionResult IVFCuadroResumen()
        {
            int anioActual = DateTime.Now.Year;
            int anioAnterior = DateTime.Now.Year - 1;
            int anioAnterior2 = DateTime.Now.Year - 1;
            int mesActual = DateTime.Now.Month;
            int mesAnterior = DateTime.Now.Month - 1;
            bool mesActualEsEnero = false;
            if (mesActual == 1)
            {
                mesAnterior = 12;
                mesActualEsEnero = true;
            }

            List<IVFMReport> reportAnioActual = GetIVFreportByNivelByYear(3, anioActual);
            List<IVFMReport> reportAnioAnterior = GetIVFreportByNivelByYear(3, anioAnterior);
            List<IVFMReport> reportAnioAnterior2 = GetIVFreportByNivelByYear(3, anioAnterior2);

            List<IVFResumenReport> report = new List<IVFResumenReport>();

            if (reportAnioActual.Count == reportAnioAnterior.Count)
            {
                for (int i = 0; i < reportAnioActual.Count; i++)
                {
                    IVFResumenReport r = new IVFResumenReport();
                    r.Texto = reportAnioActual[i].Texto;

                    r.MesX_anioX = getIVFMReportValorByMes(reportAnioActual[i], mesActual);
                    r.MesX_anioY = getIVFMReportValorByMes(reportAnioAnterior[i], mesActual);
                    r.MesX_variacion = getVariacionStr(r.MesX_anioX, r.MesX_anioY);

                    if (mesActualEsEnero)
                    {
                        r.MesY_anioX = getIVFMReportValorByMes(reportAnioAnterior[i], mesAnterior);
                        r.MesY_anioY = getIVFMReportValorByMes(reportAnioAnterior2[i], mesAnterior);
                    }
                    else
                    {
                        r.MesY_anioX = getIVFMReportValorByMes(reportAnioActual[i], mesAnterior);
                        r.MesY_anioY = getIVFMReportValorByMes(reportAnioAnterior[i], mesAnterior);
                    }
                    r.MesY_variacion = getVariacionStr(r.MesY_anioX, r.MesY_anioY);

                    double desdeEnero_anioX = 0;
                    double desdeEnero_anioY = 0;
                    double anual_anioX = 0;
                    double anual_anioY = 0;

                    for (int m = 1; m <= 12; m++)
                    {
                        if (m <= mesActual)
                        {
                            desdeEnero_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioActual[i], m));
                            desdeEnero_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                            anual_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioActual[i], m));
                            anual_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                        }
                        else
                        {
                            anual_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                            anual_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior2[i], m));
                        }
                    }

                    r.Enero_Mes_anioX = string.Format("{0:0.00}", desdeEnero_anioX / mesActual);
                    r.Enero_Mes_anioY = string.Format("{0:0.00}", desdeEnero_anioY / mesActual);
                    r.Enero_Mes_variacion = getVariacionStr(r.Enero_Mes_anioX, r.Enero_Mes_anioY);

                    r.Anual_anioX = string.Format("{0:0.00}", anual_anioX / 12);
                    r.Anual_anioY = string.Format("{0:0.00}", anual_anioY / 12);
                    r.Anual_variacion = getVariacionStr(r.Anual_anioX, r.Anual_anioY);

                    report.Add(r);
                }
            }

            ViewBag.AnioX = DateTime.Now.Year;
            ViewBag.MesX = GetMesByIndex(DateTime.Now.Month);
            ViewBag.AnioY = DateTime.Now.Year - 1;
            ViewBag.MesY = GetMesByIndex(DateTime.Now.Month - 1);

            return View(report);
        }

        private string getIVFMReportValorByMes(IVFMReport rep, int mes)
        {
            switch (mes)
            {
                case 1: return rep.Enero;
                case 2: return rep.Febrero;
                case 3: return rep.Marzo;
                case 4: return rep.Abril;
                case 5: return rep.Mayo;
                case 6: return rep.Junio;
                case 7: return rep.Julio;
                case 8: return rep.Agosto;
                case 9: return rep.Setiembre;
                case 10: return rep.Octubre;
                case 11: return rep.Noviembre;
                case 12: return rep.Diciembre;
                default: return string.Empty;
            }
        }

        public ActionResult IVFPMIncidenciaActividad()
        {
            return View();
        }

        public ActionResult IVFPMIncidenciaActividadTable(int year, int mes)
        {
            int anioActual = year;
            int anioAnterior = year - 1;
            int anioAnterior2 = year - 2;
            int mesActual = mes;
            int mesAnterior = mes - 1;
            bool mesActualEsEnero = false;
            if (mesActual == 1)
            {
                mesAnterior = 12;
                mesActualEsEnero = true;
            }

            List<IVFMReport> reportAnioActual = GetIVFreportByNivelByYear(4, anioActual);
            List<IVFMReport> reportAnioAnterior = GetIVFreportByNivelByYear(4, anioAnterior);
            List<IVFMReport> reportAnioAnterior2 = GetIVFreportByNivelByYear(4, anioAnterior2);

            List<IVFResumenReport> report = new List<IVFResumenReport>();

            if (reportAnioActual.Count == reportAnioAnterior.Count)
            {
                for (int i = 0; i < reportAnioActual.Count; i++)
                {
                    IVFResumenReport r = new IVFResumenReport();
                    r.Texto = reportAnioActual[i].Texto;

                    r.MesX_anioX = getIVFMReportValorByMes(reportAnioActual[i], mesActual);
                    r.MesX_anioY = getIVFMReportValorByMes(reportAnioAnterior[i], mesActual);
                    r.MesX_variacion = getVariacionStr(r.MesX_anioX, r.MesX_anioY);

                    if (mesActualEsEnero)
                    {
                        r.MesY_anioX = getIVFMReportValorByMes(reportAnioAnterior[i], mesAnterior);
                        r.MesY_anioY = getIVFMReportValorByMes(reportAnioAnterior2[i], mesAnterior);
                    }
                    else
                    {
                        r.MesY_anioX = getIVFMReportValorByMes(reportAnioActual[i], mesAnterior);
                        r.MesY_anioY = getIVFMReportValorByMes(reportAnioAnterior[i], mesAnterior);
                    }
                    r.MesY_variacion = getVariacionStr(r.MesY_anioX, r.MesY_anioY);

                    double desdeEnero_anioX = 0;
                    double desdeEnero_anioY = 0;
                    double anual_anioX = 0;
                    double anual_anioY = 0;

                    for (int m = 1; m <= 12; m++)
                    {
                        if (m <= mesActual)
                        {
                            desdeEnero_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioActual[i], m));
                            desdeEnero_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                            anual_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioActual[i], m));
                            anual_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                        }
                        else
                        {
                            anual_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                            anual_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior2[i], m));
                        }
                    }

                    r.Enero_Mes_anioX = string.Format("{0:0.00}", desdeEnero_anioX / mesActual);
                    r.Enero_Mes_anioY = string.Format("{0:0.00}", desdeEnero_anioY / mesActual);
                    r.Enero_Mes_variacion = getVariacionStr(r.Enero_Mes_anioX, r.Enero_Mes_anioY);

                    r.Anual_anioX = string.Format("{0:0.00}", anual_anioX / 12);
                    r.Anual_anioY = string.Format("{0:0.00}", anual_anioY / 12);
                    r.Anual_variacion = getVariacionStr(r.Anual_anioX, r.Anual_anioY);

                    r.ValorAgregado = reportAnioActual[i].ValorAgregado;
                    r.Peso = reportAnioActual[i].Peso;

                    report.Add(r);
                }
            }

            ViewBag.AnioX = year;
            ViewBag.MesX = GetMesByIndex(mes);
            ViewBag.AnioY = year - 1;
            ViewBag.MesY = GetMesByIndex(mes - 1);

            return PartialView("_IVFPMIncidenciaActividadTable", report);
        }

        public ActionResult CoberturadeIVF()
        {
            return View();
        }

        public ActionResult CoberturadeIVFTable(int year, int month, int ciiu)
        {
            List<CoberturaIvf> report = new List<CoberturaIvf>();
            report = Manager.ReporteManager.Get_COB_IVF_BY_YEAR_MONTH(year, month);
            if (ciiu > 0 && report != null)
            {
                report = report.Where(x => x.ciuu_id_ciiu == ciiu).ToList();
            }
            ViewBag.Mes = month;
            return PartialView("_CoberturadeIVFTable", report);

            //var ciuuManager = Manager.GetManager<Ciiu>();
            //var ciuuList = ciuuManager.Get().OrderBy(x => x.Codigo).ToList();
            //if (id_ciiu != null)
            //{
            //    ciuuList = ciuuList.Where(x => x.Id == id_ciiu.Value).OrderBy(x => x.Codigo).ToList();
            //}

            var ivfsYearX = GetIVFporAnio(year);
            var ivfsYearY = GetIVFporAnio(year - 1);
            if (ciiu > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
            }
            List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

            foreach (var ivfX in ivfsYearX)
            {
                setDefaultIVFMEstReport(ivfX);
                foreach (var ivfY in ivfsYearY)
                {
                    setDefaultIVFMEstReport(ivfY);
                    if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
                    {
                        IVFMEstVarReport rep = new IVFMEstVarReport();
                        rep.IdCiiu = ivfX.IdCiiu;
                        rep.IdEstablecimiento = ivfX.IdEstablecimiento;
                        rep.CodigoCiiu = ivfX.CodigoCiiu;
                        rep.CodigoEst = ivfX.CodigoEst;
                        rep.NomEst = ivfX.NomEst;
                        rep.Meses = new List<MesVariacion>();
                        MesVariacion mv = new MesVariacion();
                        mv.Nro = month;
                        setMesVar(mv, ivfX, ivfY, month);
                        rep.Meses.Add(mv);
                        reps.Add(rep);
                    }
                }
            }
            ViewBag.MesX = year;
            ViewBag.MesY = year - 1;

            return PartialView("_CoberturadeIVFTable", reps);
        }

        //
        private List<IVFMReport> GetIVFreportByNivelByYear(int nivel, int year)
        {
            int inicio = 1;
            int fin = 12;
            int decimales = 2;

            string formatDecimal = "";

            for (int i = 0; i < decimales; ++i)
            {
                formatDecimal += "0";
            }

            bool showSector = true;
            bool showSubsector = true;
            bool showDosDigitos = true;
            bool showTresDigitos = true;
            bool showCuatroDigitos = true;

            if (nivel == 0)
            {
                showSector = true;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 1)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 2)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 3)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 4)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = true;
            }

            if (nivel == 5)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 6)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 7)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = true;
            }

            List<IVFDB> ivfsBD = Manager.ReporteManager.GetVD_VP_IVF(year);

            for (int i = inicio; i <= fin; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year, i, 1)));
            }

            List<IVFM> ivfs = new List<IVFM>();
            ivfs.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            int Id = 4;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];
                Id = Id + i;
                if (ciiu.sub_sector == 2)
                {
                    var ivfm = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);
                }
            }

            ivfs.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));

            Id = Id + 1;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];

                Id = Id + i;
                if (ciiu.sub_sector == 1)
                {
                    var ivfm = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);
                }
            }

            FillPrimarioNoPrimarioAndTotal(ivfs);

            var ivfsCodCiiuNotNull = ivfs.Where(t => t.CodigoCiiu != null);

            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 2, showDosDigitos);
            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 3, showTresDigitos);

            List<IVFMReport> ivfsReport = new List<IVFMReport>();

            foreach (var ivf in ivfs)
            {
                if (ivf.Visible)
                {
                    string espacio = "";
                    for (int i = 0; i < ivf.Level - 1; ++i)
                    {
                        espacio = espacio + "&nbsp;&nbsp;&nbsp;";
                    }

                    espacio = espacio + "- ";

                    IVFMReport ivfr = new IVFMReport();
                    ivfr.Texto = espacio + ivf.Texto;
                    ivfr.Enero = string.Format("{0:0." + formatDecimal + "}", ivf.Enero);
                    ivfr.Febrero = string.Format("{0:0." + formatDecimal + "}", ivf.Febrero);
                    ivfr.Marzo = string.Format("{0:0." + formatDecimal + "}", ivf.Marzo);
                    ivfr.Abril = string.Format("{0:0." + formatDecimal + "}", ivf.Abril);
                    ivfr.Mayo = string.Format("{0:0." + formatDecimal + "}", ivf.Mayo);
                    ivfr.Junio = string.Format("{0:0." + formatDecimal + "}", ivf.Junio);
                    ivfr.Julio = string.Format("{0:0." + formatDecimal + "}", ivf.Julio);
                    ivfr.Agosto = string.Format("{0:0." + formatDecimal + "}", ivf.Agosto);
                    ivfr.Setiembre = string.Format("{0:0." + formatDecimal + "}", ivf.Setiembre);
                    ivfr.Octubre = string.Format("{0:0." + formatDecimal + "}", ivf.Octubre);
                    ivfr.Noviembre = string.Format("{0:0." + formatDecimal + "}", ivf.Noviembre);
                    ivfr.Diciembre = string.Format("{0:0." + formatDecimal + "}", ivf.Diciembre);

                    ivfsReport.Add(ivfr);
                }
            }

            return ivfsReport;
        }
        //

        public ActionResult EmpresaEnvioInformacion()
        {
            return View();
        }

        public ActionResult EmpresaEnvioInformacionTable(int Year, int Month, int IdCiiu, int IdAnalista)
        {
            var rep = Manager.ReporteManager.Get_EMP_ENV_INFO_BY_YEAR_MONTH(Year, Month)
                .OrderByDescending(x => x.encuesta_fecha).ToList();
            if (IdCiiu > 0)
            {
                rep = rep.Where(x => x.ciiu_id == IdCiiu).ToList();
            }
            if (IdAnalista > 0)
            {
                rep = rep.Where(x => x.usuario_id == IdAnalista).ToList();
            }
            return PartialView("_EmpresaEnvioInformacionTable", rep);
        }

        // Seguimieento Encuestas
        public ActionResult SeguimientoEncuesta()
        {
            ViewBag.Fecha1 = DateTime.Now.AddDays(-7).ToString("dd-MM-yyyy");
            ViewBag.Fecha2 = DateTime.Now.ToString("dd-MM-yyyy");
            return View();
        }

        public ActionResult SeguimientoEncuestaTable(string fechaInicio, string fechaFin)
        {
            SegAuxModel model = getSeguimientoEncuesta(fechaInicio, fechaFin);
            return PartialView("_SeguimientoEncuestaTable", model);
        }

        public ActionResult ExportarSeguimientoEncuesta_Excel(string fechaInicio, string fechaFin)
        {
            SegAuxModel model = getSeguimientoEncuesta(fechaInicio, fechaFin);
            Response.ContentType = "application/vnd.ms-excel";
            //var view = View("_SeguimientoEncuestaTable", model);
            //view.ContentType = MediaTypeHeaderValue.Parse("application/vnd.ms-excel");
            return View("_SeguimientoEncuestaTable", model);
        }
        public FileResult ExportarSeguimientoEncuesta_PDf(string fechaInicio, string fechaFin)
        {
            SegAuxModel model = getSeguimientoEncuesta(fechaInicio, fechaFin);
            var html = RenderRazorViewToString("_SeguimientoEncuestaTable", model);
            var stream = new MemoryStream();

            string pdf_page_size = "letter";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            int webPageWidth = 1024;
            int webPageHeight = 0;

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(html);

            // save pdf document
            //(Response, false, "Sample.pdf");

            // close pdf document
            //doc.Close();

            return File(doc.Save(), "application/pdf");
            // return ExportContent("<h1>Hola Elmer</h1>");
        }

        public void ExportToPdf(DataTable dt)
        {
            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("c://sample.pdf", FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);

            PdfPTable table = new PdfPTable(dt.Columns.Count);
            PdfPRow row = null;
            float[] widths = new float[] { 4f, 4f, 4f, 4f };

            table.SetWidths(widths);

            table.WidthPercentage = 100;
            int iCol = 0;
            string colname = "";
            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = dt.Columns.Count;

            foreach (DataColumn c in dt.Columns)
            {

                table.AddCell(new Phrase(c.ColumnName, font5));
            }

            foreach (DataRow r in dt.Rows)
            {
                if (dt.Rows.Count > 0)
                {
                    table.AddCell(new Phrase(r[0].ToString(), font5));
                    table.AddCell(new Phrase(r[1].ToString(), font5));
                    table.AddCell(new Phrase(r[2].ToString(), font5));
                    table.AddCell(new Phrase(r[3].ToString(), font5));
                }
            }
            document.Add(table);
            document.Close();

            Response.Write(document);
            Response.End();
        }

        private SegAuxModel getSeguimientoEncuesta(string fechaInicio, string fechaFin)
        {
            DateTime dtInicio = DateTime.Now;
            DateTime dtFin = DateTime.Now;
            List<SegAux> rep = new List<SegAux>();
            try
            {
                dtInicio = DateTime.ParseExact(fechaInicio, "dd-MM-yyyy", null);
                dtFin = DateTime.ParseExact(fechaFin, "dd-MM-yyyy", null);
            }
            catch (Exception)
            {

            }
            List<DateTime> fechas = new List<DateTime>();

            for (DateTime date = dtInicio; date <= dtFin; date = date.AddDays(1))
            {
                fechas.Add(date);
            }
            //
            int estado_enviado = 2;
            int estado_noenviado = 1;
            int estado_consolidado = 3;
            //
            List<decimal> analistas = new List<decimal>();

            var encuestaAnalistaList = Manager.ReporteManager.Get_SEG_ENC_ANA();
            encuestaAnalistaList = encuestaAnalistaList.Where(x => x.encuesta_fecha >= dtInicio && x.encuesta_fecha <= dtFin).ToList();
            foreach (var encana in encuestaAnalistaList)
            {
                var analista = analistas.Where(x => x == encana.usuario_id).FirstOrDefault();
                if (analista == 0)
                {
                    analistas.Add(encana.usuario_id);
                }
            }

            foreach (var analista in analistas)
            {
                SegAux seg = new SegAux() { IdAnalista = analista };
                var encanaList = encuestaAnalistaList.Where(x => x.usuario_id == analista);
                int cantidadEncuestas = encanaList.Count();
                int cantidadNoEnviados = cantidadEncuestas;
                for (int f = 0; f < fechas.Count; f++)
                {
                    var encanaListPorFecha = encanaList.Where(x => x.encuesta_fecha == fechas[f]);
                    int cantidadEncuestasPorFecha = 0;
                    int cantidadEnviadosPorFecha = 0;
                    int cantidadNoEnviadosPorFecha = 0;
                    int cantidadConsolidadosPorFecha = 0;
                    if (encanaListPorFecha != null)
                    {
                        cantidadEncuestasPorFecha = encanaListPorFecha.Count();
                        var encanaListPorFecha_enviado = encanaListPorFecha.Where(x => x.encuesta_estadoEncuesta == estado_enviado);
                        var encanaListPorFecha_noenviado = encanaListPorFecha.Where(x => x.encuesta_estadoEncuesta == estado_noenviado);
                        var encanaListPorFecha_consolidado = encanaListPorFecha.Where(x => x.encuesta_estadoEncuesta == estado_consolidado);
                        if (encanaListPorFecha_enviado != null) cantidadEnviadosPorFecha = encanaListPorFecha_enviado.Count();
                        if (encanaListPorFecha_noenviado != null) cantidadNoEnviadosPorFecha = encanaListPorFecha_noenviado.Count();
                        if (encanaListPorFecha_consolidado != null) cantidadConsolidadosPorFecha = encanaListPorFecha_consolidado.Count();
                    }
                    //cantidadNoEnviadosPorFecha = cantidadNoEnviados - cantidadEnviadosPorFecha - cantidadConsolidadosPorFecha;
                    seg.Cantidad_Enviados.Add(cantidadEnviadosPorFecha.ToString());
                    seg.Cantidad_NoEnviados.Add(cantidadNoEnviadosPorFecha.ToString());
                    seg.Cantidad_Consolidados.Add(cantidadConsolidadosPorFecha.ToString());
                    seg.Cantidad_Total.Add((cantidadEnviadosPorFecha + cantidadNoEnviadosPorFecha + cantidadConsolidadosPorFecha).ToString());
                }
                rep.Add(seg);
            }

            var usuariosIntranet = Manager.Usuario.GetUsuariosIntranet();

            foreach (var r in rep)
            {
                foreach (var usu in usuariosIntranet)
                {
                    if (r.IdAnalista == usu.Identificador)
                    {
                        r.Analista = usu.Nombres + " " + usu.Apellidos;
                    }
                }
            }

            SegAuxModel model = new SegAuxModel();

            for (int i = 0; i < fechas.Count; i++)
            {
                int cantidad_enviados_total = 0;
                int cantidad_noenviados_total = 0;
                int cantidad_consolidados_total = 0;
                int cantidad_total_total = 0;
                for (int j = 0; j < rep.Count; j++)
                {
                    cantidad_enviados_total += Convert.ToInt32(rep[j].Cantidad_Enviados[i]);
                    cantidad_noenviados_total += Convert.ToInt32(rep[j].Cantidad_NoEnviados[i]);
                    cantidad_consolidados_total += Convert.ToInt32(rep[j].Cantidad_Consolidados[i]);
                    cantidad_total_total += Convert.ToInt32(rep[j].Cantidad_Total[i]);
                }
                model.Totales.Cantidad_Enviados_Totales.Add(cantidad_enviados_total.ToString());
                model.Totales.Cantidad_NoEnviados_Totales.Add(cantidad_noenviados_total.ToString());
                model.Totales.Cantidad_Consolidados_Totales.Add(cantidad_consolidados_total.ToString());
                model.Totales.Cantidad_Total_Totales.Add(cantidad_total_total.ToString());
            }

            for (int i = 0; i < rep.Count; i++)
            {
                for (int j = 0; j < fechas.Count; j++)
                {
                    // segun analista
                    int a = Convert.ToInt32(rep[i].Cantidad_Enviados[j]) * 100;
                    int b = Convert.ToInt32(model.Totales.Cantidad_Enviados_Totales[j]);
                    if (b == 0) b = 1;
                    rep[i].PorcSegunAnalista_Enviados.Add(
                        string.Format("{0:0.0}", a / b));

                    int c = Convert.ToInt32(rep[i].Cantidad_NoEnviados[j]) * 100;
                    int d = Convert.ToInt32(model.Totales.Cantidad_NoEnviados_Totales[j]);
                    if (d == 0) d = 1;
                    rep[i].PorcSegunAnalista_NoEnviados.Add(
                        string.Format("{0:0.0}", c / d));

                    int e = Convert.ToInt32(rep[i].Cantidad_Consolidados[j]) * 100;
                    int f = Convert.ToInt32(model.Totales.Cantidad_Consolidados_Totales[j]);
                    if (f == 0) f = 1;
                    rep[i].PorcSegunAnalista_Consolidados.Add(
                        string.Format("{0:0.0}", e / f));

                    int k = Convert.ToInt32(rep[i].Cantidad_Total[j]) * 100;
                    int l = Convert.ToInt32(model.Totales.Cantidad_Total_Totales[j]);
                    if (l == 0) l = 1;
                    rep[i].PorcSegunAnalista_Total.Add(
                        string.Format("{0:0.0}", k / l));

                    // segun estado
                    int m = Convert.ToInt32(rep[i].Cantidad_Enviados[j]) * 100;
                    int n = Convert.ToInt32(rep[i].Cantidad_Total[j]);
                    if (n == 0) n = 1;
                    rep[i].PorcSegunEstado_Enviados.Add(
                        string.Format("{0:0.0}", m / n));

                    int o = Convert.ToInt32(rep[i].Cantidad_NoEnviados[j]) * 100;
                    int p = Convert.ToInt32(rep[i].Cantidad_Total[j]);
                    if (p == 0) p = 1;
                    rep[i].PorcSegunEstado_NoEnviados.Add(
                        string.Format("{0:0.0}", o / p));

                    int q = Convert.ToInt32(rep[i].Cantidad_Consolidados[j]) * 100;
                    int r = Convert.ToInt32(rep[i].Cantidad_Total[j]);
                    if (r == 0) r = 1;
                    rep[i].PorcSegunEstado_Consolidados.Add(
                        string.Format("{0:0.0}", q / r));

                    int s = Convert.ToInt32(rep[i].Cantidad_Total[j]) * 100;
                    int t = Convert.ToInt32(rep[i].Cantidad_Total[j]);
                    if (t == 0) t = 1;
                    rep[i].PorcSegunEstado_Total.Add(
                        string.Format("{0:0.0}", s / t));
                }
            }

            for (int i = 0; i < fechas.Count; i++)
            {
                double cantidad_enviados_total = 0;
                double cantidad_noenviados_total = 0;
                double cantidad_consolidados_total = 0;
                double cantidad_total_total = 0;

                double porcSegunEstado_enviados_total = 0;
                double porcSegunEstado_noenviados_total = 0;
                double porcSegunEstado_consolidados_total = 0;
                double porcSegunEstado_total_total = 0;

                for (int j = 0; j < rep.Count; j++)
                {
                    cantidad_enviados_total += Convert.ToDouble(rep[j].PorcSegunAnalista_Enviados[i]);
                    cantidad_noenviados_total += Convert.ToDouble(rep[j].PorcSegunAnalista_NoEnviados[i]);
                    cantidad_consolidados_total += Convert.ToDouble(rep[j].PorcSegunAnalista_Consolidados[i]);
                    cantidad_total_total += Convert.ToDouble(rep[j].PorcSegunAnalista_Total[i]);

                    porcSegunEstado_enviados_total += Convert.ToDouble(rep[j].PorcSegunEstado_Enviados[i]);
                    porcSegunEstado_noenviados_total += Convert.ToDouble(rep[j].PorcSegunEstado_NoEnviados[i]);
                    porcSegunEstado_consolidados_total += Convert.ToDouble(rep[j].PorcSegunEstado_Consolidados[i]);
                    porcSegunEstado_total_total += Convert.ToDouble(rep[j].PorcSegunEstado_Total[i]);
                }
                model.Totales.PorcSegunAnalista_Enviados_Totales.Add(cantidad_enviados_total.ToString());
                model.Totales.PorcSegunAnalista_NoEnviados_Totales.Add(cantidad_noenviados_total.ToString());
                model.Totales.PorcSegunAnalista_Consolidados_Totales.Add(cantidad_consolidados_total.ToString());
                model.Totales.PorcSegunAnalista_Total_Totales.Add(cantidad_total_total.ToString());

                model.Totales.PorcSegunEstado_Enviados_Totales.Add(rep.Count == 0 ? "0" : (porcSegunEstado_enviados_total / rep.Count).ToString());
                model.Totales.PorcSegunEstado_NoEnviados_Totales.Add(rep.Count == 0 ? "0" : (porcSegunEstado_noenviados_total / rep.Count).ToString());
                model.Totales.PorcSegunEstado_Consolidados_Totales.Add(rep.Count == 0 ? "0" : (porcSegunEstado_consolidados_total / rep.Count).ToString());
                model.Totales.PorcSegunEstado_Total_Totales.Add(rep.Count == 0 ? "0" : (porcSegunEstado_total_total / rep.Count).ToString());
            }

            model.Fechas = fechas;
            model.Seguimiento = rep;
            return model;
        }

        public ActionResult ExportarExcel_EmpresaEnvioInformacion(int Year, int Month, int IdCiiu, int IdAnalista)
        {
            var workbook = new XLWorkbook();
            var rep = Manager.ReporteManager.Get_EMP_ENV_INFO_BY_YEAR_MONTH(Year, Month)
                .OrderByDescending(x => x.encuesta_fecha).ToList();
            if (IdCiiu > 0)
            {
                rep = rep.Where(x => x.ciiu_id == IdCiiu).ToList();
            }
            if (IdAnalista > 0)
            {
                rep = rep.Where(x => x.usuario_id == IdAnalista).ToList();
            }

            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Nro.");
            dt.Columns.Add("Codigo");
            dt.Columns.Add("Establecimiento");
            dt.Columns.Add("CIIU/Analista");
            dt.Columns.Add("EntraCalculo");

            for (int i = 0; i < rep.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["Nro."] = (i + 1).ToString();
                row["Codigo"] = rep[i].establecimiento_ruc;
                row["Establecimiento"] = rep[i].establecimiento_nombre;
                row["CIIU/Analista"] = rep[i].establecimiento_ruc;
                row["EntraCalculo"] = rep[i].establecimiento_ruc;
                dt.Rows.Add(row);
            }
            dt.TableName = "EmpresaEnvioInformacion";
            workbook.Worksheets.Add(dt);
            return new ExcelResult(workbook, "EmpresaEnvioInformacion");
        }

        public ActionResult ExportarExcel_IVFMensualVariacion(int decimales, int year, int nivel, int inicio, int fin)
        {

            //List
            string formatDecimal = "";

            for (int i = 0; i < decimales; ++i)
            {
                formatDecimal += "0";
            }

            bool showSector = true;
            bool showSubsector = true;
            bool showDosDigitos = true;
            bool showTresDigitos = true;
            bool showCuatroDigitos = true;

            if (nivel == 0)
            {
                showSector = true;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 1)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 2)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 3)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 4)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = true;
            }

            if (nivel == 5)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 6)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 7)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = true;
            }

            List<IVFDB> ivfsBD = Manager.ReporteManager.GetVD_VP_IVF(year);
            List<IVFDB> ivfsBD2 = Manager.ReporteManager.GetVD_VP_IVF(year - 1);

            for (int i = inicio; i <= fin; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year, i, 1)));
                ivfsBD2.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year - 1, i, 1)));
            }

            List<IVFM> ivfs = new List<IVFM>();
            ivfs.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<IVFM> ivfs2 = new List<IVFM>();
            ivfs2.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs2.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            int Id = 4;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];
                Id = Id + i;
                if (ciiu.sub_sector == 2)
                {
                    var ivfm = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);

                    var ivfm2 = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm2.IdCiiu = ciiu.Id;
                    ivfm2.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD2, ivfm2, year - 1);
                    ivfs2.Add(ivfm2);
                }
            }

            ivfs.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));
            ivfs2.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));

            Id = Id + 1;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];

                Id = Id + i;
                if (ciiu.sub_sector == 1)
                {
                    var ivfm = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);

                    var ivfm2 = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm2.IdCiiu = ciiu.Id;
                    ivfm2.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD2, ivfm2, year - 1);
                    ivfs2.Add(ivfm2);
                }
            }

            FillPrimarioNoPrimarioAndTotal(ivfs);
            FillPrimarioNoPrimarioAndTotal(ivfs2);

            var ivfsCodCiiuNotNull = ivfs.Where(t => t.CodigoCiiu != null);
            var ivfsCodCiiuNotNull2 = ivfs2.Where(t => t.CodigoCiiu != null);

            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 2, showDosDigitos);
            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 3, showTresDigitos);

            FillCodPorDigitos(ivfs2, ivfsCodCiiuNotNull2, 2, showDosDigitos);
            FillCodPorDigitos(ivfs2, ivfsCodCiiuNotNull2, 3, showTresDigitos);

            List<IVFMReport> ivfsReport = new List<IVFMReport>();
            List<IVFMReport> ivfsReport2 = new List<IVFMReport>();

            for (int i = 0; i < ivfs.Count; i++)
            {
                if (ivfs[i].Visible)
                {
                    string espacio = "  ";
                    for (int j = 0; j < ivfs[i].Level - 1; ++j)
                    {
                        espacio = espacio + "      ";
                    }

                    espacio = espacio + "- ";

                    IVFMReport ivfr = new IVFMReport();
                    ivfr.Texto = espacio + ivfs[i].Texto;

                    ivfr.Enero = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Enero, ivfs2[i].Enero));
                    ivfr.Febrero = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Febrero, ivfs2[i].Febrero));
                    ivfr.Marzo = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Marzo, ivfs2[i].Marzo));
                    ivfr.Abril = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Abril, ivfs2[i].Abril));
                    ivfr.Mayo = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Mayo, ivfs2[i].Mayo));
                    ivfr.Junio = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Junio, ivfs2[i].Junio));
                    ivfr.Julio = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Julio, ivfs2[i].Julio));
                    ivfr.Agosto = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Agosto, ivfs2[i].Agosto));
                    ivfr.Setiembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Setiembre, ivfs2[i].Setiembre));
                    ivfr.Octubre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Octubre, ivfs2[i].Octubre));
                    ivfr.Noviembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Noviembre, ivfs2[i].Noviembre));
                    ivfr.Diciembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Enero, ivfs2[i].Diciembre));

                    ivfsReport.Add(ivfr);
                }
            }

            //Datatable

            var workbook = new XLWorkbook();
            DataTable dt = new DataTable();
            //Columnas
            dt.Columns.Add("SECTOR - DIVISIÓN - GRUPO - CLASE", typeof(string));
            dt.Columns.Add("ENE", typeof(string));
            dt.Columns.Add("FEB", typeof(string));
            dt.Columns.Add("MAR", typeof(string));
            dt.Columns.Add("ABR", typeof(string));
            dt.Columns.Add("MAY", typeof(string));
            dt.Columns.Add("JUN", typeof(string));
            dt.Columns.Add("JUL", typeof(string));
            dt.Columns.Add("AGO", typeof(string));
            dt.Columns.Add("SET", typeof(string));
            dt.Columns.Add("OCT", typeof(string));
            dt.Columns.Add("NOV", typeof(string));
            dt.Columns.Add("DIC", typeof(string));
            dt.Columns.Add("PROMEDIO", typeof(string));


            //Fila
            foreach (var item in ivfsReport)
            {
                string[] data = new string[14];
                data[0] = item.Texto;
                data[1] = item.Enero;
                data[2] = item.Febrero;
                data[3] = item.Marzo;
                data[4] = item.Abril;
                data[5] = item.Mayo;
                data[6] = item.Junio;
                data[7] = item.Julio;
                data[8] = item.Agosto;
                data[9] = item.Setiembre;
                data[10] = item.Octubre;
                data[11] = item.Noviembre;
                data[12] = item.Diciembre;
                data[13] = string.Format("{0:0." + formatDecimal + "}", promediar(data));
                dt.Rows.Add(data);
            }

            dt.TableName = "Variación %";
            workbook.Worksheets.Add(dt);

            return new ExcelResult(workbook, "Variación Mensual");

        }
        #endregion

        private void FillIVFMPorEstabWithIVFDB(List<IVFEstDB> ivfsBD, IVFMEstReport ivfr, int year)
        {
            string formatoDecimal = "00";

            foreach (var ivfbd in ivfsBD.Where(t => t.IdCiiu == ivfr.IdCiiu && t.IdEstablecimiento == ivfr.IdEstablecimiento))
            {
                if (new DateTime(year, 1, 1) == ivfbd.Fecha)
                    ivfr.Enero = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 2, 1) == ivfbd.Fecha)
                    ivfr.Febrero = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 3, 1) == ivfbd.Fecha)
                    ivfr.Marzo = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 4, 1) == ivfbd.Fecha)
                    ivfr.Abril = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 5, 1) == ivfbd.Fecha)
                    ivfr.Mayo = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 6, 1) == ivfbd.Fecha)
                    ivfr.Junio = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 7, 1) == ivfbd.Fecha)
                    ivfr.Julio = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 8, 1) == ivfbd.Fecha)
                    ivfr.Agosto = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 9, 1) == ivfbd.Fecha)
                    ivfr.Setiembre = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 10, 1) == ivfbd.Fecha)
                    ivfr.Octubre = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 11, 1) == ivfbd.Fecha)
                    ivfr.Noviembre = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                if (new DateTime(year, 12, 1) == ivfbd.Fecha)
                    ivfr.Diciembre = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
            }
        }

        private void FillIVFMPorEstabWithIVFDB_Variacion(List<IVFEstDB> ivfsBD, IVFMEstVarReport ivfr, int year, string opcion)
        {
            string formatoDecimal = "00";
            for (int i = 0; i < 12; i++)
            {
                foreach (var ivfbd in ivfsBD.Where(t => t.IdCiiu == ivfr.IdCiiu && t.IdEstablecimiento == ivfr.IdEstablecimiento))
                {
                    if (new DateTime(year, i + 1, 1) == ivfbd.Fecha)
                    {
                        if (opcion == "X")
                        {
                            ivfr.Meses[i].MesX = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                        }
                        if (opcion == "Y")
                        {
                            ivfr.Meses[i].MesY = string.Format("{0:0." + formatoDecimal + "}", ivfbd.IVF);
                            if (ivfr.Meses.Where(x => x.Nro == i + 1).FirstOrDefault().MesX != "")
                            {
                                ivfr.Meses[i].VariacionPorcentual
                                    = (((Convert.ToDouble(ivfr.Meses[i].MesX) - Convert.ToDouble(ivfr.Meses[i].MesY)) * 100) / Convert.ToDouble(ivfr.Meses[i].MesX)).ToString();
                            }
                        }
                    }
                }
            }
        }

        public ActionResult DescargaArchivos()
        {
            return View();
        }

        [HttpPost]
        public FileStreamResult DescargaArchivosTable(int year, int inicio, int fin, int idCiiu)
        {
            List<DescargaEncuesta> descargas = Manager.ReporteManager.GetDescargaArchivo(new DateTime(year, inicio, 1), new DateTime(year, fin, 1), idCiiu);
            var fileDownloadName = "consultaAnio" + year.ToString() + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            ExcelPackage package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Descarga de Archivos para Consulta Año " + year.ToString());
            ws.PrinterSettings.PaperSize = ePaperSize.A4;
            ws.PrinterSettings.Orientation = eOrientation.Landscape;
            ws.PrinterSettings.FitToPage = true;

            ws.Cells[1, 1].Value = "Año: " + year.ToString();
            string mes = "";
            for (int x = inicio; x <= fin; ++x)
            {
                if (x == inicio)
                {
                    mes = Tools.GetMonthText(x);
                }
                else
                {
                    mes = mes + " " + Tools.GetMonthText(x);
                }

            }

            ws.Cells[1, 2].Value = "Mes: " + mes;
            string ciiu = (idCiiu == 0) ? "TODOS" : Manager.Ciiu.Get(t => t.Id == idCiiu).FirstOrDefault().Codigo;
            ws.Cells[2, 1, 2, 2].Merge = true;
            ws.Cells[2, 1, 2, 2].Value = "CIIU: " + ciiu;

            int fila = 4;

            ws.Cells[fila, 1].Value = "Código Establecimiento";
            ws.Cells[fila, 2].Value = "Razón Social";
            ws.Cells[fila, 3].Value = "Mes";
            ws.Cells[fila, 4].Value = "CIIU";
            ws.Cells[fila, 5].Value = "Código Producto";
            ws.Cells[fila, 6].Value = "Producto";
            ws.Cells[fila, 7].Value = "Unidad Medida";
            ws.Cells[fila, 8].Value = "Valor Unitario";
            ws.Cells[fila, 9].Value = "Existencia";
            ws.Cells[fila, 10].Value = "Venta Interior";
            ws.Cells[fila, 11].Value = "Venta Exterior";
            ws.Cells[fila, 12].Value = "Otros";

            int i = 0;

            fila += 1;

            foreach (var descarga in descargas)
            {
                ws.Cells[i + fila, 1].Value = descarga.CodigoEstab;
                ws.Cells[i + fila, 2].Value = descarga.NombreEstab;
                ws.Cells[i + fila, 3].Value = descarga.Mes;
                ws.Cells[i + fila, 4].Value = descarga.CodigoCiiu;
                ws.Cells[i + fila, 5].Value = descarga.CodigoProd;
                ws.Cells[i + fila, 6].Value = descarga.NombreProd;
                ws.Cells[i + fila, 7].Value = descarga.UM;
                ws.Cells[i + fila, 8].Value = descarga.ValorUnitario;
                ws.Cells[i + fila, 9].Value = descarga.Existencia;
                ws.Cells[i + fila, 10].Value = descarga.VentaPais;
                ws.Cells[i + fila, 11].Value = descarga.VentaExtranjero;
                ws.Cells[i + fila, 12].Value = descarga.OtrosIngresos;

                i = i + 1;
            }

            var fileStream = new MemoryStream();
            package.SaveAs(fileStream);

            fileStream.Position = 0;
            return File(fileStream, contentType, fileDownloadName);
        }

        private FileContentResult Export(string format, ReportViewer rpt)
        {
            string mimeType;
            string encoding;
            string fileNameExtension;
            Warning[] warnings;
            string[] streamids;
            byte[] exportBytes = rpt.LocalReport.Render(format, null, out mimeType, out encoding, out fileNameExtension, out streamids, out warnings);

            return File(exportBytes, mimeType);
        }

        //[HttpGet]
        //public JsonResult BuscarEmpresaEnvioInformacion(EmpresaEnvioInformacionFilter filter)
        //{
        //    return Json(Manager.ReporteManager.GetEmpresaEnvioInformacion(filter),
        //        JsonRequestBehavior.AllowGet);
        //}

        //private ReportViewer FillRptEmpresaEnvioInformacion(EmpresaEnvioInformacionFilter filter, string Mes, string Ciiu, string Analista)
        //{
        //    ReportViewer rpt = new ReportViewer();

        //    rpt.ProcessingMode = ProcessingMode.Local;
        //    rpt.LocalReport.ReportPath = "Rpt\\RptEmpresaEnvioInformacion.rdlc";

        //    ReportDataSource source = new ReportDataSource();
        //    source.Name = "DsEmpresaEnvioInformacion";
        //    source.Value = Manager.ReporteManager.GetEmpresaEnvioInformacion(filter);

        //    TextInfo myTI = new CultureInfo("es-PE", false).TextInfo;

        //    ReportParameter[] parameters = new ReportParameter[5];
        //    parameters[0] = new ReportParameter("rpFecha", myTI.ToTitleCase(DateTime.Now.ToString("dddd, dd")) + " de " + myTI.ToTitleCase(DateTime.Now.ToString("MMMM")) + " del " + DateTime.Now.ToString("yyyy"));
        //    parameters[1] = new ReportParameter("rpAnio", filter.Year.ToString());
        //    parameters[2] = new ReportParameter("rpMes", Mes);
        //    parameters[3] = new ReportParameter("rpCIIU", Ciiu);
        //    parameters[4] = new ReportParameter("rpAnalista", Analista);

        //    rpt.LocalReport.SetParameters(parameters);
        //    rpt.LocalReport.DataSources.Clear();
        //    rpt.LocalReport.DataSources.Add(source);
        //    rpt.LocalReport.Refresh();

        //    return rpt;
        //}

        //public ActionResult EmpresaEnvioInformacionPDF(EmpresaEnvioInformacionFilter filter, string Mes, string Ciiu, string Analista)
        //{
        //    ReportViewer rpt = FillRptEmpresaEnvioInformacion(filter, Mes, Ciiu, Analista);

        //    return Export("PDF", rpt);
        //}

        //public ActionResult EmpresaEnvioInformacionExcel(EmpresaEnvioInformacionFilter filter, string Mes, string Ciiu, string Analista)
        //{
        //    ReportViewer rpt = FillRptEmpresaEnvioInformacion(filter, Mes, Ciiu, Analista);

        //    return Export("Excel", rpt);
        //}

        private string GetMesByIndex(int index)
        {
            switch (index)
            {
                case 1: return "ENERO";
                case 2: return "FEBRERO";
                case 3: return "MARZO";
                case 4: return "ABRIL";
                case 5: return "MAYO";
                case 6: return "JUNIO";
                case 7: return "JULIO";
                case 8: return "AGOSTO";
                case 9: return "SETIEMBRE";
                case 10: return "OCTUBRE";
                case 11: return "NOVIEMBRE";
                case 12: return "DICIEMBRE";
                default:
                    break;
            }
            return string.Empty;
        }

        private double promediar(string[] data)
        {
            double prom = 0;
            prom = (Double.Parse(data[1]) + Double.Parse(data[2]) + Double.Parse(data[3]) + Double.Parse(data[4]) + Double.Parse(data[5]) + Double.Parse(data[6]) +
                Double.Parse(data[7]) + Double.Parse(data[8]) + Double.Parse(data[9]) + Double.Parse(data[10]) + Double.Parse(data[11]) + Double.Parse(data[12])) / 12;

            return prom;
        }

        // EXCEL
        public ActionResult ExportarExcelEstablecimiento(int year, int inicio, int fin)
        {
            var workbook = new XLWorkbook();
            //List
            var ivfsYearX = GetIVFporAnio(year);
            var ivfsYearY = GetIVFporAnio(year - 1);
            List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

            foreach (var ivfX in ivfsYearX)
            {
                foreach (var ivfY in ivfsYearY)
                {
                    if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
                    {
                        IVFMEstVarReport rep = new IVFMEstVarReport();
                        rep.IdCiiu = ivfX.IdCiiu;
                        rep.IdEstablecimiento = ivfX.IdEstablecimiento;
                        rep.CodigoCiiu = ivfX.CodigoCiiu;
                        rep.CodigoEst = ivfX.CodigoEst;
                        rep.NomEst = ivfX.NomEst;
                        rep.Meses = new List<MesVariacion>();
                        for (int i = inicio; i <= fin; i++)
                        {
                            MesVariacion mv = new MesVariacion();
                            mv.Nro = i;
                            setMesVar(mv, ivfX, ivfY, i);
                            rep.Meses.Add(mv);
                        }
                        reps.Add(rep);
                    }
                }
            }
            //Datatable
            DataTable dt = new DataTable();
            int columnas = 3;
            //Columnas
            string var = "Var%";
            dt.Columns.Add("CIIU", typeof(string));
            dt.Columns.Add("Código", typeof(string));
            dt.Columns.Add("Establecimiento", typeof(string));
            if (reps.Count > 0)
            {
                foreach (var mes in reps[0].Meses)
                {
                    dt.Columns.Add(GetMesAbreviaturaByIndex(mes.Nro) + "-" + year.ToString(), typeof(string));
                    dt.Columns.Add(GetMesAbreviaturaByIndex(mes.Nro) + "-" + (year - 1).ToString(), typeof(string));
                    dt.Columns.Add(var, typeof(string));
                    var = var + " ";
                    columnas = columnas + 3;
                }
                //Fila
                foreach (var item in reps)
                {
                    string[] data = new string[columnas];
                    data[0] = item.CodigoCiiu;
                    data[1] = item.CodigoEst;
                    data[2] = item.NomEst;
                    int indice = 3;
                    foreach (var mes in item.Meses)
                    {
                        data[indice] = mes.MesX;
                        data[indice + 1] = mes.MesY;
                        data[indice + 2] = mes.VariacionPorcentual;
                        indice = indice + 3;
                    }
                    dt.Rows.Add(data);
                }
            }


            dt.TableName = "Variación %";
            workbook.Worksheets.Add(dt);

            return new ExcelResult(workbook, "Variación por Establecimiento");
        }

        //public ActionResult ExportarExcelEstablecimientoMes(int year, int month, int ciiu)
        //{

        //    var workbook = new XLWorkbook();

        //    //List
        //    var ivfsYearX = GetIVFporAnio(year);
        //    var ivfsYearY = GetIVFporAnio(year - 1);
        //    if (ciiu > -1)
        //    {
        //        ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
        //        ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
        //    }
        //    List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

        //    foreach (var ivfX in ivfsYearX)
        //    {
        //        foreach (var ivfY in ivfsYearY)
        //        {
        //            if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
        //            {
        //                IVFMEstVarReport rep = new IVFMEstVarReport();
        //                rep.IdCiiu = ivfX.IdCiiu;
        //                rep.IdEstablecimiento = ivfX.IdEstablecimiento;
        //                rep.CodigoCiiu = ivfX.CodigoCiiu;
        //                rep.CodigoEst = ivfX.CodigoEst;
        //                rep.NomEst = ivfX.NomEst;
        //                rep.Meses = new List<MesVariacion>();
        //                MesVariacion mv = new MesVariacion();
        //                mv.Nro = month;
        //                setMesVar(mv, ivfX, ivfY, month);
        //                rep.Meses.Add(mv);
        //                reps.Add(rep);
        //            }
        //        }
        //    }

        //    //Datatable
        //    DataTable dt = new DataTable();
        //    string var = "Var%";
        //    int columnas = 3;
        //    //Columnas
        //    dt.Columns.Add("CIIU", typeof(string));
        //    dt.Columns.Add("Código", typeof(string));
        //    dt.Columns.Add("Establecimiento", typeof(string));
        //    if (reps.Count>0)
        //    {
        //        foreach (var mes in reps[0].Meses)
        //        {
        //            dt.Columns.Add(GetMesAbreviaturaByIndex(mes.Nro) + "-" + year.ToString(), typeof(string));
        //            dt.Columns.Add(GetMesAbreviaturaByIndex(mes.Nro) + "-" + (year - 1).ToString(), typeof(string));
        //            dt.Columns.Add(var, typeof(string));
        //            var += " ";
        //            columnas += 3;
        //        }
        //        //Fila
        //        foreach (var item in reps)
        //        {
        //            string[] data = new string[columnas];
        //            data[0] = item.CodigoCiiu;
        //            data[1] = item.CodigoEst;
        //            data[2] = item.NomEst;
        //            int indice = 3;
        //            foreach (var mes in item.Meses)
        //            {
        //                data[indice] = mes.MesX;
        //                data[indice + 1] = mes.MesY;
        //                data[indice + 2] = mes.VariacionPorcentual;
        //                indice = indice + 3;
        //            }
        //            dt.Rows.Add(data);
        //        }
        //    }


        //    dt.TableName = "Variación %";
        //    workbook.Worksheets.Add(dt);

        //    return new ExcelResult(workbook, "Variación por Establecimiento y Mes");
        //}

        public ActionResult ExportarExcelEstablecimientoMes(int year, int month, int ciiu, int estb)
        {

            //List
            var ivfsYearX = GetIVFporAnio(year);
            var ivfsYearY = GetIVFporAnio(year - 1);
            if (ciiu > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
            }

            if (estb > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdEstablecimiento == estb).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdEstablecimiento == estb).ToList();
            }

            List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

            foreach (var ivfX in ivfsYearX)
            {
                foreach (var ivfY in ivfsYearY)
                {
                    if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
                    {
                        IVFMEstVarReport rep = new IVFMEstVarReport();
                        rep.IdCiiu = ivfX.IdCiiu;
                        rep.IdEstablecimiento = ivfX.IdEstablecimiento;
                        rep.CodigoCiiu = ivfX.CodigoCiiu;
                        rep.CodigoEst = ivfX.CodigoEst;
                        rep.NomEst = ivfX.NomEst;
                        rep.Meses = new List<MesVariacion>();
                        MesVariacion mv = new MesVariacion();
                        mv.Nro = month;
                        setMesVar(mv, ivfX, ivfY, month);
                        rep.Meses.Add(mv);
                        reps.Add(rep);
                    }
                }
            }

            //Datatable
            DataTable dt = new DataTable();
            string var = "Var%";
            int columnas = 3;
            //Columnas
            dt.Columns.Add("CIIU", typeof(string));
            dt.Columns.Add("Código", typeof(string));
            dt.Columns.Add("Establecimiento", typeof(string));
            if (reps.Count > 0)
            {
                foreach (var mes in reps[0].Meses)
                {
                    dt.Columns.Add(GetMesAbreviaturaByIndex(mes.Nro) + "-" + year.ToString(), typeof(string));
                    dt.Columns.Add(GetMesAbreviaturaByIndex(mes.Nro) + "-" + (year - 1).ToString(), typeof(string));
                    dt.Columns.Add(var, typeof(string));
                    var += " ";
                    columnas += 3;
                }
                //Fila
                foreach (var item in reps)
                {
                    string[] data = new string[columnas];
                    data[0] = item.CodigoCiiu;
                    data[1] = item.CodigoEst;
                    data[2] = item.NomEst;
                    int indice = 3;
                    foreach (var mes in item.Meses)
                    {
                        data[indice] = mes.MesX;
                        data[indice + 1] = mes.MesY;
                        data[indice + 2] = mes.VariacionPorcentual;
                        indice = indice + 3;
                    }
                    dt.Rows.Add(data);
                }
            }

            dt.TableName = "Variación %";

            // ******

            Workbook wk = new Workbook();
            Worksheet ws = wk.Worksheets[0];
            Worksheet ws2 = wk.Worksheets[1];
            ws.InsertDataTable(dt, true, 1, 1);
            ws.Name = "Variación %";
            
            //Sets header style
            CellStyle styleHeader = ws.Rows[0].Style;
            styleHeader.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
            styleHeader.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
            styleHeader.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
            styleHeader.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
            styleHeader.VerticalAlignment = VerticalAlignType.Center;
            styleHeader.KnownColor = ExcelColors.Green;
            styleHeader.Font.KnownColor = ExcelColors.White;
            styleHeader.Font.IsBold = true;

            //set chart
            var rangoValores = "D2:D"+(reps.Count+1).ToString();
            var rangoSeries = "C2:C" + (reps.Count + 1).ToString();
            ws2.Name = "Gráfico";
            ws2.GridLinesVisible = false;

            Chart chart = ws2.Charts.Add();


            chart.DataRange = ws.Range[rangoValores];
            chart.SeriesDataFromRange = false;

            Spire.Xls.Charts.ChartSerie cs = chart.Series[0];
            cs.CategoryLabels = ws.Range[rangoSeries];
            cs.Values = ws.Range[rangoValores];
            cs.DataPoints.DefaultDataPoint.DataLabels.HasValue = true;

            chart.LeftColumn = 1;
            chart.TopRow = 8;
            chart.RightColumn = 11;
            chart.BottomRow = 29;
            chart.ChartType = ExcelChartType.ColumnStacked;

            chart.ChartTitle = "Reporte de variación % CIIU - Establecimiento";
            chart.ChartTitleArea.IsBold = true;
            chart.ChartTitleArea.Size = 12;

            chart.Legend.Position = LegendPositionType.Top;
            //ws.Range[rangoValores].Style.NumberFormat = "######,##";

            return new ExcelResult2(wk, "Variación por Establecimiento y Mes");
        }

        
        public ActionResult ExportarExcelCoberturaIVF(int year, int month, int ciiu)
        {

            var workbook = new XLWorkbook();

            //List
            var ivfsYearX = GetIVFporAnio(year);
            var ivfsYearY = GetIVFporAnio(year - 1);
            if (ciiu > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
            }
            List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

            foreach (var ivfX in ivfsYearX)
            {
                foreach (var ivfY in ivfsYearY)
                {
                    if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
                    {
                        IVFMEstVarReport rep = new IVFMEstVarReport();
                        rep.IdCiiu = ivfX.IdCiiu;
                        rep.IdEstablecimiento = ivfX.IdEstablecimiento;
                        rep.CodigoCiiu = ivfX.CodigoCiiu;
                        rep.CodigoEst = ivfX.CodigoEst;
                        rep.NomEst = ivfX.NomEst;
                        rep.Meses = new List<MesVariacion>();
                        MesVariacion mv = new MesVariacion();
                        mv.Nro = month;
                        setMesVar(mv, ivfX, ivfY, month);
                        rep.Meses.Add(mv);
                        reps.Add(rep);
                    }
                }
            }

            //Datatable
            DataTable dt = new DataTable();
            string var = "Var%";
            int columnas = 3;
            //Columnas
            dt.Columns.Add("CIIU", typeof(string));
            dt.Columns.Add("Código", typeof(string));
            dt.Columns.Add("Establecimiento", typeof(string));
            foreach (var mes in reps[0].Meses)
            {
                dt.Columns.Add(GetMesAbreviaturaByIndex(mes.Nro) + "-" + year.ToString(), typeof(string));
                dt.Columns.Add(GetMesAbreviaturaByIndex(mes.Nro) + "-" + (year - 1).ToString(), typeof(string));
                dt.Columns.Add(var, typeof(string));
                var += " ";
                columnas += 3;
            }
            //Fila
            foreach (var item in reps)
            {
                string[] data = new string[columnas];
                data[0] = item.CodigoCiiu;
                data[1] = item.CodigoEst;
                data[2] = item.NomEst;
                int indice = 3;
                foreach (var mes in item.Meses)
                {
                    data[indice] = mes.MesX;
                    data[indice + 1] = mes.MesY;
                    data[indice + 2] = mes.VariacionPorcentual;
                    indice = indice + 3;
                }
                dt.Rows.Add(data);
            }

            dt.TableName = "Variación %";
            workbook.Worksheets.Add(dt);

            return new ExcelResult(workbook, "Cobertura IVF");
        }

        public ActionResult ExportarExcelMensual(int decimales, int year, int nivel, int inicio, int fin)
        {
            string formatDecimal = "";

            for (int i = 0; i < decimales; ++i)
            {
                formatDecimal += "0";
            }

            bool showSector = true;
            bool showSubsector = true;
            bool showDosDigitos = true;
            bool showTresDigitos = true;
            bool showCuatroDigitos = true;

            if (nivel == 0)
            {
                showSector = true;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 1)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 2)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 3)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 4)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = true;
            }

            if (nivel == 5)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 6)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 7)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = true;
            }

            List<IVFDB> ivfsBD = Manager.ReporteManager.GetVD_VP_IVF(year);

            for (int i = inicio; i <= fin; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year, i, 1)));
            }

            List<IVFM> ivfs = new List<IVFM>();
            ivfs.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            int Id = 4;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];
                Id = Id + i;
                if (ciiu.sub_sector == 2)
                {
                    var ivfm = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);
                }
            }

            ivfs.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));

            Id = Id + 1;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];

                Id = Id + i;
                if (ciiu.sub_sector == 1)
                {
                    var ivfm = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);
                }
            }

            FillPrimarioNoPrimarioAndTotal(ivfs);

            var ivfsCodCiiuNotNull = ivfs.Where(t => t.CodigoCiiu != null);

            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 2, showDosDigitos);
            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 3, showTresDigitos);

            List<IVFMReport> ivfsReport = new List<IVFMReport>();

            foreach (var ivf in ivfs)
            {
                if (ivf.Visible)
                {
                    string espacio = "";
                    for (int i = 0; i < ivf.Level - 1; ++i)
                    {
                        espacio = espacio + "   ";
                    }

                    espacio = espacio + "- ";

                    IVFMReport ivfr = new IVFMReport();
                    ivfr.Texto = espacio + ivf.Texto;
                    ivfr.Enero = string.Format("{0:0." + formatDecimal + "}", ivf.Enero);
                    ivfr.Febrero = string.Format("{0:0." + formatDecimal + "}", ivf.Febrero);
                    ivfr.Marzo = string.Format("{0:0." + formatDecimal + "}", ivf.Marzo);
                    ivfr.Abril = string.Format("{0:0." + formatDecimal + "}", ivf.Abril);
                    ivfr.Mayo = string.Format("{0:0." + formatDecimal + "}", ivf.Mayo);
                    ivfr.Junio = string.Format("{0:0." + formatDecimal + "}", ivf.Junio);
                    ivfr.Julio = string.Format("{0:0." + formatDecimal + "}", ivf.Julio);
                    ivfr.Agosto = string.Format("{0:0." + formatDecimal + "}", ivf.Agosto);
                    ivfr.Setiembre = string.Format("{0:0." + formatDecimal + "}", ivf.Setiembre);
                    ivfr.Octubre = string.Format("{0:0." + formatDecimal + "}", ivf.Octubre);
                    ivfr.Noviembre = string.Format("{0:0." + formatDecimal + "}", ivf.Noviembre);
                    ivfr.Diciembre = string.Format("{0:0." + formatDecimal + "}", ivf.Diciembre);

                    ivfsReport.Add(ivfr);
                }
            }

            //Datatable

            var workbook = new XLWorkbook();
            DataTable dt = new DataTable();
            //Columnas
            dt.Columns.Add("SECTOR - DIVISIÓN - GRUPO - CLASE", typeof(string));
            dt.Columns.Add("ENE", typeof(string));
            dt.Columns.Add("FEB", typeof(string));
            dt.Columns.Add("MAR", typeof(string));
            dt.Columns.Add("ABR", typeof(string));
            dt.Columns.Add("MAY", typeof(string));
            dt.Columns.Add("JUN", typeof(string));
            dt.Columns.Add("JUL", typeof(string));
            dt.Columns.Add("AGO", typeof(string));
            dt.Columns.Add("SET", typeof(string));
            dt.Columns.Add("OCT", typeof(string));
            dt.Columns.Add("NOV", typeof(string));
            dt.Columns.Add("DIC", typeof(string));
            dt.Columns.Add("PROMEDIO", typeof(string));


            //Fila
            foreach (var item in ivfsReport)
            {
                string[] data = new string[14];
                data[0] = item.Texto;
                data[1] = item.Enero;
                data[2] = item.Febrero;
                data[3] = item.Marzo;
                data[4] = item.Abril;
                data[5] = item.Mayo;
                data[6] = item.Junio;
                data[7] = item.Julio;
                data[8] = item.Agosto;
                data[9] = item.Setiembre;
                data[10] = item.Octubre;
                data[11] = item.Noviembre;
                data[12] = item.Diciembre;
                data[13] = string.Format("{0:0." + formatDecimal + "}", promediar(data));
                dt.Rows.Add(data);
            }

            dt.TableName = "Variación %";
            workbook.Worksheets.Add(dt);

            return new ExcelResult(workbook, "Variación Mensual");

        }

        public ActionResult ExportarExcelMensualVariacion(int decimales, int year, int nivel, int inicio, int fin)
        {

            //List
            string formatDecimal = "";

            for (int i = 0; i < decimales; ++i)
            {
                formatDecimal += "0";
            }

            bool showSector = true;
            bool showSubsector = true;
            bool showDosDigitos = true;
            bool showTresDigitos = true;
            bool showCuatroDigitos = true;

            if (nivel == 0)
            {
                showSector = true;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 1)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 2)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 3)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 4)
            {
                showSector = true;
                showSubsector = true;
                showDosDigitos = true;
                showTresDigitos = true;
                showCuatroDigitos = true;
            }

            if (nivel == 5)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = true;
                showTresDigitos = false;
                showCuatroDigitos = false;
            }

            if (nivel == 6)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = true;
                showCuatroDigitos = false;
            }

            if (nivel == 7)
            {
                showSector = false;
                showSubsector = false;
                showDosDigitos = false;
                showTresDigitos = false;
                showCuatroDigitos = true;
            }

            List<IVFDB> ivfsBD = Manager.ReporteManager.GetVD_VP_IVF(year);
            List<IVFDB> ivfsBD2 = Manager.ReporteManager.GetVD_VP_IVF(year - 1);

            for (int i = inicio; i <= fin; i++)
            {
                ivfsBD.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year, i, 1)));
                ivfsBD2.AddRange(Manager.ReporteManager.GetCA_IVF(new DateTime(year - 1, i, 1)));
            }

            List<IVFM> ivfs = new List<IVFM>();
            ivfs.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<IVFM> ivfs2 = new List<IVFM>();
            ivfs2.Add(new IVFM(1, 0, 1, "SECTOR FARRIL TOTAL", showSector));
            ivfs2.Add(new IVFM(2, 1, 2, "SUB-SECTOR FARRIL NO PRIMARIO", showSubsector));

            List<Ciiu> ciius = Manager.Ciiu.GetByFilter().OrderBy(t => t.Codigo).ToList();
            int Id = 4;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];
                Id = Id + i;
                if (ciiu.sub_sector == 2)
                {
                    var ivfm = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);

                    var ivfm2 = new IVFM(Id, 2, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm2.IdCiiu = ciiu.Id;
                    ivfm2.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD2, ivfm2, year - 1);
                    ivfs2.Add(ivfm2);
                }
            }

            ivfs.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));
            ivfs2.Add(new IVFM(3, 1, 2, "SUB-SECTOR PRIMARIO", showSubsector));

            Id = Id + 1;
            for (var i = 0; i < ciius.Count; ++i)
            {
                var ciiu = ciius[i];

                Id = Id + i;
                if (ciiu.sub_sector == 1)
                {
                    var ivfm = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm.IdCiiu = ciiu.Id;
                    ivfm.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD, ivfm, year);
                    ivfs.Add(ivfm);

                    var ivfm2 = new IVFM(Id, 3, 5, ciiu.Nombre, showCuatroDigitos);
                    ivfm2.IdCiiu = ciiu.Id;
                    ivfm2.CodigoCiiu = ciiu.Codigo;
                    FillIVFMWithIVFDB(ivfsBD2, ivfm2, year - 1);
                    ivfs2.Add(ivfm2);
                }
            }

            FillPrimarioNoPrimarioAndTotal(ivfs);
            FillPrimarioNoPrimarioAndTotal(ivfs2);

            var ivfsCodCiiuNotNull = ivfs.Where(t => t.CodigoCiiu != null);
            var ivfsCodCiiuNotNull2 = ivfs2.Where(t => t.CodigoCiiu != null);

            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 2, showDosDigitos);
            FillCodPorDigitos(ivfs, ivfsCodCiiuNotNull, 3, showTresDigitos);

            FillCodPorDigitos(ivfs2, ivfsCodCiiuNotNull2, 2, showDosDigitos);
            FillCodPorDigitos(ivfs2, ivfsCodCiiuNotNull2, 3, showTresDigitos);

            List<IVFMReport> ivfsReport = new List<IVFMReport>();
            List<IVFMReport> ivfsReport2 = new List<IVFMReport>();

            for (int i = 0; i < ivfs.Count; i++)
            {
                if (ivfs[i].Visible)
                {
                    string espacio = "  ";
                    for (int j = 0; j < ivfs[i].Level - 1; ++j)
                    {
                        espacio = espacio + "      ";
                    }

                    espacio = espacio + "- ";

                    IVFMReport ivfr = new IVFMReport();
                    ivfr.Texto = espacio + ivfs[i].Texto;

                    ivfr.Enero = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Enero, ivfs2[i].Enero));
                    ivfr.Febrero = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Febrero, ivfs2[i].Febrero));
                    ivfr.Marzo = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Marzo, ivfs2[i].Marzo));
                    ivfr.Abril = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Abril, ivfs2[i].Abril));
                    ivfr.Mayo = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Mayo, ivfs2[i].Mayo));
                    ivfr.Junio = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Junio, ivfs2[i].Junio));
                    ivfr.Julio = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Julio, ivfs2[i].Julio));
                    ivfr.Agosto = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Agosto, ivfs2[i].Agosto));
                    ivfr.Setiembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Setiembre, ivfs2[i].Setiembre));
                    ivfr.Octubre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Octubre, ivfs2[i].Octubre));
                    ivfr.Noviembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Noviembre, ivfs2[i].Noviembre));
                    ivfr.Diciembre = string.Format("{0:0." + formatDecimal + "}", getVariacionIVF(ivfs[i].Enero, ivfs2[i].Diciembre));

                    ivfsReport.Add(ivfr);
                }
            }

            //Datatable

            var workbook = new XLWorkbook();
            DataTable dt = new DataTable();
            //Columnas
            dt.Columns.Add("SECTOR - DIVISIÓN - GRUPO - CLASE", typeof(string));
            dt.Columns.Add("ENE", typeof(string));
            dt.Columns.Add("FEB", typeof(string));
            dt.Columns.Add("MAR", typeof(string));
            dt.Columns.Add("ABR", typeof(string));
            dt.Columns.Add("MAY", typeof(string));
            dt.Columns.Add("JUN", typeof(string));
            dt.Columns.Add("JUL", typeof(string));
            dt.Columns.Add("AGO", typeof(string));
            dt.Columns.Add("SET", typeof(string));
            dt.Columns.Add("OCT", typeof(string));
            dt.Columns.Add("NOV", typeof(string));
            dt.Columns.Add("DIC", typeof(string));
            dt.Columns.Add("PROMEDIO", typeof(string));


            //Fila
            foreach (var item in ivfsReport)
            {
                string[] data = new string[14];
                data[0] = item.Texto;
                data[1] = item.Enero;
                data[2] = item.Febrero;
                data[3] = item.Marzo;
                data[4] = item.Abril;
                data[5] = item.Mayo;
                data[6] = item.Junio;
                data[7] = item.Julio;
                data[8] = item.Agosto;
                data[9] = item.Setiembre;
                data[10] = item.Octubre;
                data[11] = item.Noviembre;
                data[12] = item.Diciembre;
                data[13] = string.Format("{0:0." + formatDecimal + "}", promediar(data));
                dt.Rows.Add(data);
            }

            dt.TableName = "Variación %";
            workbook.Worksheets.Add(dt);

            return new ExcelResult(workbook, "Variación Mensual");

        }

        public ActionResult ExportarIncidenciaActividad(int year, int mes)
        {
            int anioActual = year;
            int anioAnterior = year - 1;
            int anioAnterior2 = year - 2;
            int mesActual = mes;
            int mesAnterior = mes - 1;
            bool mesActualEsEnero = false;
            if (mesActual == 1)
            {
                mesAnterior = 12;
                mesActualEsEnero = true;
            }

            List<IVFMReport> reportAnioActual = GetIVFreportByNivelByYear(4, anioActual);
            List<IVFMReport> reportAnioAnterior = GetIVFreportByNivelByYear(4, anioAnterior);
            List<IVFMReport> reportAnioAnterior2 = GetIVFreportByNivelByYear(4, anioAnterior2);

            List<IVFResumenReport> report = new List<IVFResumenReport>();

            if (reportAnioActual.Count == reportAnioAnterior.Count)
            {
                for (int i = 0; i < reportAnioActual.Count; i++)
                {
                    IVFResumenReport r = new IVFResumenReport();
                    r.Texto = reportAnioActual[i].Texto;

                    r.MesX_anioX = getIVFMReportValorByMes(reportAnioActual[i], mesActual);
                    r.MesX_anioY = getIVFMReportValorByMes(reportAnioAnterior[i], mesActual);
                    r.MesX_variacion = getVariacionStr(r.MesX_anioX, r.MesX_anioY);

                    if (mesActualEsEnero)
                    {
                        r.MesY_anioX = getIVFMReportValorByMes(reportAnioAnterior[i], mesAnterior);
                        r.MesY_anioY = getIVFMReportValorByMes(reportAnioAnterior2[i], mesAnterior);
                    }
                    else
                    {
                        r.MesY_anioX = getIVFMReportValorByMes(reportAnioActual[i], mesAnterior);
                        r.MesY_anioY = getIVFMReportValorByMes(reportAnioAnterior[i], mesAnterior);
                    }
                    r.MesY_variacion = getVariacionStr(r.MesY_anioX, r.MesY_anioY);

                    double desdeEnero_anioX = 0;
                    double desdeEnero_anioY = 0;
                    double anual_anioX = 0;
                    double anual_anioY = 0;

                    for (int m = 1; m <= 12; m++)
                    {
                        if (m <= mesActual)
                        {
                            desdeEnero_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioActual[i], m));
                            desdeEnero_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                            anual_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioActual[i], m));
                            anual_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                        }
                        else
                        {
                            anual_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                            anual_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior2[i], m));
                        }
                    }

                    r.Enero_Mes_anioX = string.Format("{0:0.00}", desdeEnero_anioX / mesActual);
                    r.Enero_Mes_anioY = string.Format("{0:0.00}", desdeEnero_anioY / mesActual);
                    r.Enero_Mes_variacion = getVariacionStr(r.Enero_Mes_anioX, r.Enero_Mes_anioY);

                    r.Anual_anioX = string.Format("{0:0.00}", anual_anioX / 12);
                    r.Anual_anioY = string.Format("{0:0.00}", anual_anioY / 12);
                    r.Anual_variacion = getVariacionStr(r.Anual_anioX, r.Anual_anioY);

                    r.ValorAgregado = reportAnioActual[i].ValorAgregado;
                    r.Peso = reportAnioActual[i].Peso;

                    report.Add(r);
                }
            }

            var workbook = new XLWorkbook();
            DataTable dt = new DataTable();
            dt.Columns.Add("SECTOR-DIVISION-GRUPO");
            dt.Columns.Add("V.Agre");
            dt.Columns.Add("Peso");
            dt.Columns.Add("Mes-" + (year - 1));
            dt.Columns.Add("Mes-" + year);
            dt.Columns.Add("Var%");

            foreach (var rep in report)
            {
                DataRow row = dt.NewRow();
                row["SECTOR-DIVISION-GRUPO"] = rep.Texto.Replace("&nbsp;", " ");
                row["V.Agre"] = rep.ValorAgregado;
                row["Peso"] = rep.Peso;
                row["Mes-" + (year - 1)] = rep.MesX_anioX;
                row["Mes-" + year] = rep.MesX_anioY;
                row["Var%"] = rep.MesX_variacion;
                dt.Rows.Add(row);
            }

            dt.TableName = "IncidenciaActividad";
            workbook.Worksheets.Add(dt);

            return new ExcelResult(workbook, "IncidenciaActividad");
        }

        public ActionResult ExportIVFPorEstablecimientoVariacion(int year, int inicio, int fin, int ciiu)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CIIU");
            dt.Columns.Add("Codigo");
            dt.Columns.Add("Establecimiento");
            int aux = 0;
            for (int i = inicio; i <= fin; i++)
            {
                dt.Columns.Add(GetMesAbreviaturaByIndex(i) + "-" + year);
                dt.Columns.Add(GetMesAbreviaturaByIndex(i) + "-" + (year - 1));
                dt.Columns.Add("Var%" + i);
                aux++;
            }

            var ivfsYearX = GetIVFporAnio(year);
            var ivfsYearY = GetIVFporAnio(year - 1);
            if (ciiu > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
            }
            List<IVFMEstVarReport> reps = new List<IVFMEstVarReport>();

            foreach (var ivfX in ivfsYearX)
            {
                setDefaultIVFMEstReport(ivfX);
                foreach (var ivfY in ivfsYearY)
                {
                    setDefaultIVFMEstReport(ivfY);
                    if (ivfX.IdCiiu == ivfY.IdCiiu && ivfX.IdEstablecimiento == ivfY.IdEstablecimiento)
                    {
                        IVFMEstVarReport rep = new IVFMEstVarReport();
                        rep.IdCiiu = ivfX.IdCiiu;
                        rep.IdEstablecimiento = ivfX.IdEstablecimiento;
                        rep.CodigoCiiu = ivfX.CodigoCiiu;
                        rep.CodigoEst = ivfX.CodigoEst;
                        rep.NomEst = ivfX.NomEst;
                        rep.Meses = new List<MesVariacion>();
                        for (int i = inicio; i <= fin; i++)
                        {
                            MesVariacion mv = new MesVariacion();
                            mv.Nro = i;
                            setMesVar(mv, ivfX, ivfY, i);
                            rep.Meses.Add(mv);
                        }
                        reps.Add(rep);
                    }
                }
            }
            ViewBag.MesX = year;
            ViewBag.MesY = year - 1;

            for (int i = 0; i < reps.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["CIIU"] = reps[i].CodigoCiiu;
                row["Codigo"] = reps[i].CodigoEst;
                row["Establecimiento"] = reps[i].NomEst;
                int aux2 = 0;
                for (int f = inicio; f <= fin; f++)
                {
                    row[GetMesAbreviaturaByIndex(f) + "-" + year] = reps[i].Meses[aux2].MesX;
                    row[GetMesAbreviaturaByIndex(f) + "-" + (year - 1)] = reps[i].Meses[aux2].MesY;
                    row["Var%" + f] = reps[i].Meses[aux2].VariacionPorcentual;
                    aux2++;
                }
                dt.Rows.Add(row);
            }

            dt.TableName = "VariaciónIVFEstablecimiento";
            var workbook = new XLWorkbook();
            workbook.Worksheets.Add(dt);
            return new ExcelResult(workbook, "VariaciónIVFEstablecimiento");
        }

        public ActionResult ExportCoberturadeIVF(int year, int month, int ciiu)
        {
            List<CoberturaIvf> report = new List<CoberturaIvf>();
            report = Manager.ReporteManager.Get_COB_IVF_BY_YEAR_MONTH(year, month);
            if (ciiu > 0 && report != null)
            {
                report = report.Where(x => x.ciuu_id_ciiu == ciiu).ToList();
            }
            ViewBag.Mes = month;

            DataTable dt = new DataTable();
            dt.Columns.Add("CIIU");
            dt.Columns.Add("Establecimiento");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Ponderacion");
            dt.Columns.Add(GetMesAbreviaturaByIndex(month));

            decimal count_peso = 0;
            decimal count_peso_llego = 0;
            for (int i = 0; i < report.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["CIIU"] = report[i].ciiu_codigo;
                row["Establecimiento"] = report[i].establecimiento_ruc;
                row["Nombre"] = report[i].establecimiento_nombre;
                row["Ponderacion"] = report[i].establecimiento_peso;
                row[GetMesAbreviaturaByIndex(month)] = report[i].establecimiento_peso_llego;
                dt.Rows.Add(row);
                count_peso += report[i].establecimiento_peso;
                count_peso_llego += report[i].establecimiento_peso_llego;
            }
            DataRow row2 = dt.NewRow();
            row2["CIIU"] = "";
            row2["Establecimiento"] = "";
            row2["Nombre"] = "Total";
            row2["Ponderacion"] = count_peso.ToString();
            row2[GetMesAbreviaturaByIndex(month)] = count_peso_llego.ToString();
            dt.Rows.Add(row2);
            dt.TableName = "CoberturadeIVF";
            var workbook = new XLWorkbook();
            workbook.Worksheets.Add(dt);
            return new ExcelResult(workbook, "CoberturadeIVF");
        }

        public ActionResult ExportIVFCuadroResumen()
        {
            int anioActual = DateTime.Now.Year;
            int anioAnterior = DateTime.Now.Year - 1;
            int anioAnterior2 = DateTime.Now.Year - 1;
            int mesActual = DateTime.Now.Month;
            int mesAnterior = DateTime.Now.Month - 1;
            bool mesActualEsEnero = false;
            if (mesActual == 1)
            {
                mesAnterior = 12;
                mesActualEsEnero = true;
            }

            List<IVFMReport> reportAnioActual = GetIVFreportByNivelByYear(3, anioActual);
            List<IVFMReport> reportAnioAnterior = GetIVFreportByNivelByYear(3, anioAnterior);
            List<IVFMReport> reportAnioAnterior2 = GetIVFreportByNivelByYear(3, anioAnterior2);

            List<IVFResumenReport> report = new List<IVFResumenReport>();

            if (reportAnioActual.Count == reportAnioAnterior.Count)
            {
                for (int i = 0; i < reportAnioActual.Count; i++)
                {
                    IVFResumenReport r = new IVFResumenReport();
                    r.Texto = reportAnioActual[i].Texto;

                    r.MesX_anioX = getIVFMReportValorByMes(reportAnioActual[i], mesActual);
                    r.MesX_anioY = getIVFMReportValorByMes(reportAnioAnterior[i], mesActual);
                    r.MesX_variacion = getVariacionStr(r.MesX_anioX, r.MesX_anioY);

                    if (mesActualEsEnero)
                    {
                        r.MesY_anioX = getIVFMReportValorByMes(reportAnioAnterior[i], mesAnterior);
                        r.MesY_anioY = getIVFMReportValorByMes(reportAnioAnterior2[i], mesAnterior);
                    }
                    else
                    {
                        r.MesY_anioX = getIVFMReportValorByMes(reportAnioActual[i], mesAnterior);
                        r.MesY_anioY = getIVFMReportValorByMes(reportAnioAnterior[i], mesAnterior);
                    }
                    r.MesY_variacion = getVariacionStr(r.MesY_anioX, r.MesY_anioY);

                    double desdeEnero_anioX = 0;
                    double desdeEnero_anioY = 0;
                    double anual_anioX = 0;
                    double anual_anioY = 0;

                    for (int m = 1; m <= 12; m++)
                    {
                        if (m <= mesActual)
                        {
                            desdeEnero_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioActual[i], m));
                            desdeEnero_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                            anual_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioActual[i], m));
                            anual_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                        }
                        else
                        {
                            anual_anioX += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior[i], m));
                            anual_anioY += Convert.ToDouble(getIVFMReportValorByMes(reportAnioAnterior2[i], m));
                        }
                    }

                    r.Enero_Mes_anioX = string.Format("{0:0.00}", desdeEnero_anioX / mesActual);
                    r.Enero_Mes_anioY = string.Format("{0:0.00}", desdeEnero_anioY / mesActual);
                    r.Enero_Mes_variacion = getVariacionStr(r.Enero_Mes_anioX, r.Enero_Mes_anioY);

                    r.Anual_anioX = string.Format("{0:0.00}", anual_anioX / 12);
                    r.Anual_anioY = string.Format("{0:0.00}", anual_anioY / 12);
                    r.Anual_variacion = getVariacionStr(r.Anual_anioX, r.Anual_anioY);

                    report.Add(r);
                }
            }

            ViewBag.AnioX = DateTime.Now.Year;
            ViewBag.MesX = GetMesByIndex(DateTime.Now.Month);
            ViewBag.AnioY = DateTime.Now.Year - 1;
            ViewBag.MesY = GetMesByIndex(DateTime.Now.Month - 1);

            //
            DataTable dt = new DataTable();
            dt.Columns.Add("SECTOR-DIVISION-GRUPO");
            dt.Columns.Add(GetMesByIndex(mesAnterior) + "-" + anioAnterior);
            dt.Columns.Add(GetMesByIndex(mesAnterior) + "-" + anioActual);
            dt.Columns.Add("Var1");
            dt.Columns.Add(GetMesByIndex(mesActual) + "-" + anioAnterior);
            dt.Columns.Add(GetMesByIndex(mesActual) + "-" + anioActual);
            dt.Columns.Add("Var2");
            dt.Columns.Add("Enero-" + GetMesByIndex(mesAnterior) + "-" + anioAnterior);
            dt.Columns.Add("Enero-" + GetMesByIndex(mesActual) + "-" + anioAnterior);
            dt.Columns.Add("Var3");
            dt.Columns.Add("Anual-" + GetMesByIndex(mesActual) + "-" + anioAnterior);
            dt.Columns.Add("Anual-" + GetMesByIndex(mesActual) + "-" + anioActual);
            dt.Columns.Add("Var4");
            for (int i = 0; i < report.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["SECTOR-DIVISION-GRUPO"] = report[i].Texto.Replace("&nbsp;", " ");
                row[GetMesByIndex(mesAnterior) + "-" + anioAnterior] = report[i].MesX_anioX;
                row[GetMesByIndex(mesAnterior) + "-" + anioActual] = report[i].MesX_anioY;
                row["Var1"] = report[i].MesX_variacion;
                row[GetMesByIndex(mesActual) + "-" + anioAnterior] = report[i].MesY_anioX;
                row[GetMesByIndex(mesActual) + "-" + anioActual] = report[i].MesY_anioY;
                row["Var2"] = report[i].MesY_variacion;
                row["Enero-" + GetMesByIndex(mesAnterior) + "-" + anioAnterior] = report[i].Enero_Mes_anioX;
                row["Enero-" + GetMesByIndex(mesActual) + "-" + anioAnterior] = report[i].Enero_Mes_anioY;
                row["Var3"] = report[i].Enero_Mes_variacion;
                row["Anual-" + GetMesByIndex(mesActual) + "-" + anioAnterior] = report[i].Anual_anioX;
                row["Anual-" + GetMesByIndex(mesActual) + "-" + anioActual] = report[i].Anual_anioY;
                row["Var4"] = report[i].Anual_variacion;
                dt.Rows.Add(row);
            }
            dt.TableName = "IVFCuadroResumen";
            var workbook = new XLWorkbook();
            workbook.Worksheets.Add(dt);
            return new ExcelResult(workbook, "IVFCuadroResumen");
        }

        public JsonResult ValidarData(int year, int month, int ciiu)
        {
            int cantidad = 0;
            var ivfsYearX = GetIVFporAnio(year);
            var ivfsYearY = GetIVFporAnio(year - 1);
            if (ciiu > -1)
            {
                ivfsYearX = ivfsYearX.Where(x => x.IdCiiu == ciiu).ToList();
                ivfsYearY = ivfsYearY.Where(x => x.IdCiiu == ciiu).ToList();
            }
            if (ivfsYearX.Count > 1)
            {
                cantidad = ivfsYearX.Count;
            }
            var data = new
            {
                Cantidad = cantidad
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public static string GetMesAbreviaturaByIndex(int index)
        {
            switch (index)
            {
                case 1: return "ENE";
                case 2: return "FEB";
                case 3: return "MAR";
                case 4: return "ABR";
                case 5: return "MAY";
                case 6: return "JUN";
                case 7: return "JUL";
                case 8: return "AGO";
                case 9: return "SET";
                case 10: return "OCT";
                case 11: return "NOV";
                case 12: return "DIC";
                default:
                    break;
            }
            return string.Empty;
        }
    }
}