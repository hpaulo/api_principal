using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace api.Negocios.Admin
{
    public class GatewayTbLogManifesto
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbLogManifesto()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDLOG = 100,
            DTLOGINICIO = 101,
            DSXMLENTRADA = 102,
            CDRETORNO = 103,
            DSRETORNO = 104,
            DSMETODO = 105,
            DSXMLRETORNO = 106,
            DTLOGFIM = 107,

        };

        /// <summary>
        /// Get TbLogManifesto/TbLogManifesto
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbLogManifesto> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbLogManifestos.AsQueryable<tbLogManifesto>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDLOG:
                        Int32 idLog = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLog.Equals(idLog)).AsQueryable<tbLogManifesto>();
                        break;
                    case CAMPOS.DTLOGINICIO:
                        DateTime dtLogInicio = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtLogInicio.Equals(dtLogInicio)).AsQueryable<tbLogManifesto>();
                        break;
                    case CAMPOS.DSXMLENTRADA:
                        string dsXmlEntrada = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsXmlEntrada.Equals(dsXmlEntrada)).AsQueryable<tbLogManifesto>();
                        break;
                    case CAMPOS.CDRETORNO:
                        short cdRetorno = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdRetorno.Equals(cdRetorno)).AsQueryable<tbLogManifesto>();
                        break;
                    case CAMPOS.DSRETORNO:
                        string dsRetorno = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsRetorno.Equals(dsRetorno)).AsQueryable<tbLogManifesto>();
                        break;
                    case CAMPOS.DSMETODO:
                        string dsMetodo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsMetodo.Equals(dsMetodo)).AsQueryable<tbLogManifesto>();
                        break;
                    case CAMPOS.DSXMLRETORNO:
                        string dsXmlRetorno = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsXmlRetorno.Equals(dsXmlRetorno)).AsQueryable<tbLogManifesto>();
                        break;
                    case CAMPOS.DTLOGFIM:
                        DateTime dtLogFim = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtLogFim.Equals(dtLogFim)).AsQueryable<tbLogManifesto>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDLOG:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLog).AsQueryable<tbLogManifesto>();
                    else entity = entity.OrderByDescending(e => e.idLog).AsQueryable<tbLogManifesto>();
                    break;
                case CAMPOS.DTLOGINICIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtLogInicio).AsQueryable<tbLogManifesto>();
                    else entity = entity.OrderByDescending(e => e.dtLogInicio).AsQueryable<tbLogManifesto>();
                    break;
                case CAMPOS.DSXMLENTRADA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsXmlEntrada).AsQueryable<tbLogManifesto>();
                    else entity = entity.OrderByDescending(e => e.dsXmlEntrada).AsQueryable<tbLogManifesto>();
                    break;
                case CAMPOS.CDRETORNO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdRetorno).AsQueryable<tbLogManifesto>();
                    else entity = entity.OrderByDescending(e => e.cdRetorno).AsQueryable<tbLogManifesto>();
                    break;
                case CAMPOS.DSRETORNO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsRetorno).AsQueryable<tbLogManifesto>();
                    else entity = entity.OrderByDescending(e => e.dsRetorno).AsQueryable<tbLogManifesto>();
                    break;
                case CAMPOS.DSMETODO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsMetodo).AsQueryable<tbLogManifesto>();
                    else entity = entity.OrderByDescending(e => e.dsMetodo).AsQueryable<tbLogManifesto>();
                    break;
                case CAMPOS.DSXMLRETORNO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsXmlRetorno).AsQueryable<tbLogManifesto>();
                    else entity = entity.OrderByDescending(e => e.dsXmlRetorno).AsQueryable<tbLogManifesto>();
                    break;
                case CAMPOS.DTLOGFIM:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtLogFim).AsQueryable<tbLogManifesto>();
                    else entity = entity.OrderByDescending(e => e.dtLogFim).AsQueryable<tbLogManifesto>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbLogManifesto/TbLogManifesto
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbLogManifesto = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
                var queryTotal = query;

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = queryTotal.Count();


                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    query = query.Skip(skipRows).Take(pageSize);
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                // COLEÇÃO DE RETORNO
                if (colecao == 1)
                {
                    CollectionTbLogManifesto = query.Select(e => new
                    {

                        idLog = e.idLog,
                        dtLogInicio = e.dtLogInicio,
                        dsXmlEntrada = e.dsXmlEntrada,
                        cdRetorno = e.cdRetorno,
                        dsRetorno = e.dsRetorno,
                        dsMetodo = e.dsMetodo,
                        dsXmlRetorno = e.dsXmlRetorno,
                        dtLogFim = e.dtLogFim,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbLogManifesto = query.Select(e => new
                    {

                        idLog = e.idLog,
                        dtLogInicio = e.dtLogInicio,
                        dsXmlEntrada = e.dsXmlEntrada,
                        cdRetorno = e.cdRetorno,
                        dsRetorno = e.dsRetorno,
                        dsMetodo = e.dsMetodo,
                        dsXmlRetorno = e.dsXmlRetorno,
                        dtLogFim = e.dtLogFim,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbLogManifesto;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbLogManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbLogManifesto
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbLogManifesto param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbLogManifestos.Add(param);
                _db.SaveChanges();
                return param.idLog;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbLogManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbLogManifesto
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idLog)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbLogManifestos.Remove(_db.tbLogManifestos.Where(e => e.idLog.Equals(idLog)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbLogManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbLogManifesto
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbLogManifesto param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbLogManifesto value = _db.tbLogManifestos
                        .Where(e => e.idLog.Equals(param.idLog))
                        .First<tbLogManifesto>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idLog != null && param.idLog != value.idLog)
                    value.idLog = param.idLog;
                if (param.dtLogInicio != null && param.dtLogInicio != value.dtLogInicio)
                    value.dtLogInicio = param.dtLogInicio;
                if (param.dsXmlEntrada != null && param.dsXmlEntrada != value.dsXmlEntrada)
                    value.dsXmlEntrada = param.dsXmlEntrada;
                if (param.cdRetorno != null && param.cdRetorno != value.cdRetorno)
                    value.cdRetorno = param.cdRetorno;
                if (param.dsRetorno != null && param.dsRetorno != value.dsRetorno)
                    value.dsRetorno = param.dsRetorno;
                if (param.dsMetodo != null && param.dsMetodo != value.dsMetodo)
                    value.dsMetodo = param.dsMetodo;
                if (param.dsXmlRetorno != null && param.dsXmlRetorno != value.dsXmlRetorno)
                    value.dsXmlRetorno = param.dsXmlRetorno;
                if (param.dtLogFim != null && param.dtLogFim != value.dtLogFim)
                    value.dtLogFim = param.dtLogFim;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbLogManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
