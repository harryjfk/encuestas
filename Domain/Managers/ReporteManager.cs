using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Entity.Reportes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Domain.Managers
{
    public class ReporteManager
    {
        private DbContext Context { get; set; }

        public ReporteManager(DbContext context)
        {
            Context = context;
        }

        public List<EmpresaEnvioInformacion> GetEmpresaEnvioInformacion(EmpresaEnvioInformacionFilter filter)
        {
            var pAnio = new OracleParameter("pAnio", OracleDbType.Double, filter.Year, ParameterDirection.Input);
            var pMes = new OracleParameter("pMes", OracleDbType.Double, filter.Month, ParameterDirection.Input);
            var pIdCiiu = new OracleParameter("pIdCiiu", OracleDbType.Double, filter.IdCiiu, ParameterDirection.Input);
            var pIdUsuario = new OracleParameter("pIdUsuario", OracleDbType.Double, filter.IdAnalista, ParameterDirection.Input);
            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);

            return Context.Database.SqlQuery<EmpresaEnvioInformacion>("BEGIN SP_ESTABLECIMIENTO_ENVIO(:pAnio, :pMes, :pIdCiiu, :pIdUsuario, :pResult); end;", pAnio, pMes, pIdCiiu, pIdUsuario, pResult).ToList();
        }

        public List<VariacionEstablecimiento> GetVariacionEstablecimiento(VariacionEstablecimientoFilter filter)
        {
            return new List<VariacionEstablecimiento>()
            {
                new VariacionEstablecimiento()
                {
                    Level = 1,
                    Descripcion = "SECTOR FARRIL TOTAL"
                },
                new VariacionEstablecimiento()
                {
                    Level = 2,
                    Descripcion = "MANUFACTURA PRIMARIA"
                },
                new VariacionEstablecimiento()
                {
                    Level = 2,
                    Descripcion = "MANUFACTURA NO PRIMARIA"
                },
                new VariacionEstablecimiento()
                {
                    Level = 3,
                    Descripcion = "BIENES DE CONSUMO"
                },
                new VariacionEstablecimiento()
                {
                    Level = 3,
                    Descripcion = "BIENES INTERMEDIOS"
                },
                new VariacionEstablecimiento()
                {
                    Level = 3,
                    Descripcion = "BIENES DE CAPITAL"
                },
                new VariacionEstablecimiento()
                {
                    Level = 3,
                    Descripcion = "SERVICIO"
                }
            };
        }

        public List<IVFDB> GetVD_VP_IVF(int year)
        {
            var pYear = new OracleParameter("pYear", OracleDbType.Double, year, ParameterDirection.Input);
            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);

            //return Context.Database.SqlQuery<IVFDB>("BEGIN SP_OBTENER_VP_VD_BY_YEAR(:pYear, :pResult); END;", pYear, pResult).ToList();
            return Context.Database.SqlQuery<IVFDB>("BEGIN SP_OBTENER_VP_VD_BY_YEAR_2(:pYear, :pResult); END;", pYear, pResult).ToList();
        }

        public List<IVFDB> GetCA_IVF(DateTime fecha)
        {
            var pFecha = new OracleParameter("pFecha", OracleDbType.Date, fecha, ParameterDirection.Input);
            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);

            return Context.Database.SqlQuery<IVFDB>("BEGIN SP_OBTENER_CA_BY_DATE(:pFecha, :pResult); END;", pFecha, pResult).ToList();
        }

        public List<IVFEstDB> GetEMP_VD_VP_IVF(int year)
        {
            var pYear = new OracleParameter("pYear", OracleDbType.Double, year, ParameterDirection.Input);
            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);

            return Context.Database.SqlQuery<IVFEstDB>("BEGIN SP_OBTENER_VP_VD_BY_YEAR_EST(:pYear, :pResult); END;", pYear, pResult).ToList();
        }

        public List<IVFEstDB> GetEMPCA_IVF(DateTime fecha)
        {
            var pFecha = new OracleParameter("pFecha", OracleDbType.Date, fecha, ParameterDirection.Input);
            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);

            return Context.Database.SqlQuery<IVFEstDB>("BEGIN SP_OBTENER_CA_BY_DATE_EST(:pFecha, :pResult); END;", pFecha, pResult).ToList();
        }

        public List<DescargaEncuesta> GetDescargaArchivo(DateTime fechaDesde, DateTime fechaHasta, int idCiiu)
        {
            var pFechaDesde = new OracleParameter("pFechaDesde", OracleDbType.Date, fechaDesde, ParameterDirection.Input);
            var pFechaHasta = new OracleParameter("pFechaHasta", OracleDbType.Date, fechaHasta, ParameterDirection.Input);
            var pIdCiiu = new OracleParameter("pIdCiiu", OracleDbType.Int32, idCiiu, ParameterDirection.Input);

            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);

            return Context.Database.SqlQuery<DescargaEncuesta>("BEGIN SP_DESCARGA_ENCUESTA(:pFechaDesde, :pFechaHasta, :pIdCiiu, :pResult); END;", pFechaDesde, pFechaHasta, pIdCiiu, pResult).ToList();
        }

        #region Elmer
        public List<EmpresaInfo> Get_EMP_INFO_BY_YEAR(int year)
        {
            var pYear = new OracleParameter("pYear", OracleDbType.Double, year, ParameterDirection.Input);
            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);

            return Context.Database.SqlQuery<EmpresaInfo>("BEGIN SP_OBTENER_EMP_INFO_BY_YEAR(:pYear, :pResult); END;", pYear, pResult).ToList();
        }

        public List<EmpresaEnvioInfo> Get_EMP_ENV_INFO_BY_YEAR_MONTH(int year, int month)
        {
            var pYear = new OracleParameter("pYear", OracleDbType.Double, year, ParameterDirection.Input);
            var pMonth = new OracleParameter("pMonth", OracleDbType.Double, month, ParameterDirection.Input);
            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);
            return Context.Database.SqlQuery<EmpresaEnvioInfo>("BEGIN SP_EMP_ENV_INFO_BY_YEAR_MONTH(:pYear, :pMonth, :pResult); END;", pYear, pMonth, pResult).ToList();
        }

        public List<SegEncuestaAnalista> Get_SEG_ENC_ANA()
        {
            try
            {
                var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);
                return Context.Database.SqlQuery<SegEncuestaAnalista>("BEGIN SP_SEG_ENC_ANA(:pResult); END;", pResult).ToList();
            }
            catch (Exception)
            {
                
                throw;
            }
            return new List<SegEncuestaAnalista>();
        }

        public List<CoberturaIvf> Get_COB_IVF_BY_YEAR_MONTH(int year, int month)
        {
            var pYear = new OracleParameter("pYear", OracleDbType.Double, year, ParameterDirection.Input);
            var pMonth = new OracleParameter("pMonth", OracleDbType.Double, month, ParameterDirection.Input);
            var pResult = new OracleParameter("pResult", OracleDbType.RefCursor, ParameterDirection.Output);
            return Context.Database.SqlQuery<CoberturaIvf>("BEGIN SP_COB_IVF_BY_YEAR_MONTH(:pYear, :pMonth, :pResult); END;", pYear, pMonth, pResult).ToList();
        }

        #endregion
    }
}
