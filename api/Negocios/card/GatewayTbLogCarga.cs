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

namespace api.Negocios.Card
{
    public class GatewayTbLogCarga
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbLogCarga()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDLOGCARGA = 100,
            DTCOMPETENCIA = 101,
            NRCNPJ = 102,
            CDADQUIRENTE = 103,
            FLSTATUSVENDA = 104,
            FLSTATUSPAGOS = 105,
            FLSTATUSRECEBER = 106,

        };

        /// <summary>
        /// Get TbLogCarga/TbLogCarga
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbLogCarga> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbLogCargas.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDLOGCARGA:
                        Int32 idLogCarga = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogCarga.Equals(idLogCarga)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.DTCOMPETENCIA:
                        DateTime dtCompetencia = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtCompetencia.Equals(dtCompetencia)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSVENDA:
                        Boolean flStatusVenda = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusVenda.Equals(flStatusVenda)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSPAGOS:
                        Boolean flStatusPagos = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusPagos.Equals(flStatusPagos)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSRECEBER:
                        Boolean flStatusReceber = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusReceber.Equals(flStatusReceber)).AsQueryable<tbLogCarga>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDLOGCARGA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogCarga).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.idLogCarga).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.DTCOMPETENCIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtCompetencia).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.dtCompetencia).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusVenda).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusVenda).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSPAGOS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusPagos).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusPagos).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSRECEBER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusReceber).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusReceber).AsQueryable<tbLogCarga>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbLogCarga/TbLogCarga
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbLogCarga = new List<dynamic>();
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
                    CollectionTbLogCarga = query.Select(e => new
                    {

                        idLogCarga = e.idLogCarga,
                        dtCompetencia = e.dtCompetencia,
                        nrCNPJ = e.nrCNPJ,
                        cdAdquirente = e.cdAdquirente,
                        flStatusVenda = e.flStatusVenda,
                        flStatusPagos = e.flStatusPagos,
                        flStatusReceber = e.flStatusReceber,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbLogCarga = query.Select(e => new
                    {

                        idLogCarga = e.idLogCarga,
                        dtCompetencia = e.dtCompetencia,
                        nrCNPJ = e.nrCNPJ,
                        cdAdquirente = e.cdAdquirente,
                        flStatusVenda = e.flStatusVenda,
                        flStatusPagos = e.flStatusPagos,
                        flStatusReceber = e.flStatusReceber,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbLogCarga;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbLogCarga" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbLogCarga
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbLogCarga param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbLogCargas.Add(param);
                _db.SaveChanges();
                return param.idLogCarga;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbLogCarga" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbLogCarga
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idLogCarga)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbLogCargas.Remove(_db.tbLogCargas.Where(e => e.idLogCarga.Equals(idLogCarga)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbLogCarga" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Altera tbLogCarga
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbLogCarga param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbLogCarga value = _db.tbLogCargas
                        .Where(e => e.idLogCarga.Equals(param.idLogCarga))
                        .First<tbLogCarga>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idLogCarga != null && param.idLogCarga != value.idLogCarga)
                    value.idLogCarga = param.idLogCarga;
                if (param.dtCompetencia != null && param.dtCompetencia != value.dtCompetencia)
                    value.dtCompetencia = param.dtCompetencia;
                if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                    value.nrCNPJ = param.nrCNPJ;
                if (param.cdAdquirente != null && param.cdAdquirente != value.cdAdquirente)
                    value.cdAdquirente = param.cdAdquirente;
                if (param.flStatusVenda != null && param.flStatusVenda != value.flStatusVenda)
                    value.flStatusVenda = param.flStatusVenda;
                if (param.flStatusPagos != null && param.flStatusPagos != value.flStatusPagos)
                    value.flStatusPagos = param.flStatusPagos;
                if (param.flStatusReceber != null && param.flStatusReceber != value.flStatusReceber)
                    value.flStatusReceber = param.flStatusReceber;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbLogCarga" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}