using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Card
{
    public class GatewayTbAdquirente
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbAdquirente()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDADQUIRENTE = 100,
            NMADQUIRENTE = 101,
            DSADQUIRENTE = 102,
            STADQUIRENTE = 103,
            HREXECUCAO = 104,

        };

        /// <summary>
        /// Get TbAdquirente/TbAdquirente
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbAdquirente> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbAdquirentes.AsQueryable<tbAdquirente>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente == cdAdquirente).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.NMADQUIRENTE:
                        string nmAdquirente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmAdquirente.Equals(nmAdquirente)).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.DSADQUIRENTE:
                        string dsAdquirente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsAdquirente.Equals(dsAdquirente)).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.STADQUIRENTE:
                        byte stAdquirente = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.stAdquirente.Equals(stAdquirente)).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.HREXECUCAO:
                        DateTime hrExecucao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.hrExecucao.Equals(hrExecucao)).AsQueryable<tbAdquirente>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbAdquirente>();
                    break;
                case CAMPOS.NMADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmAdquirente).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.nmAdquirente).AsQueryable<tbAdquirente>();
                    break;
                case CAMPOS.DSADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsAdquirente).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.dsAdquirente).AsQueryable<tbAdquirente>();
                    break;
                case CAMPOS.STADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.stAdquirente).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.stAdquirente).AsQueryable<tbAdquirente>();
                    break;
                case CAMPOS.HREXECUCAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.hrExecucao).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.hrExecucao).AsQueryable<tbAdquirente>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbAdquirente/TbAdquirente
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbAdquirente = new List<dynamic>();
                Retorno retorno = new Retorno();

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
                    CollectionTbAdquirente = query.Select(e => new
                    {

                        cdAdquirente = e.cdAdquirente,
                        nmAdquirente = e.nmAdquirente,
                        dsAdquirente = e.dsAdquirente,
                        stAdquirente = e.stAdquirente,
                        hrExecucao = e.hrExecucao,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbAdquirente = query.Select(e => new
                    {

                        cdAdquirente = e.cdAdquirente,
                        nmAdquirente = e.nmAdquirente,
                        dsAdquirente = e.dsAdquirente,
                        stAdquirente = e.stAdquirente,
                        hrExecucao = e.hrExecucao,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbAdquirente;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar adquirente" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbAdquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbAdquirente param)
        {
            try
            {
                _db.tbAdquirentes.Add(param);
                _db.SaveChanges();
                return param.cdAdquirente;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar adquirente" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbAdquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdAdquirente)
        {
            try
            {
                _db.tbAdquirentes.Remove(_db.tbAdquirentes.Where(e => e.cdAdquirente == cdAdquirente).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar adquirente" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Altera tbAdquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbAdquirente param)
        {
            try
            {
                tbAdquirente value = _db.tbAdquirentes
                        .Where(e => e.cdAdquirente == param.cdAdquirente)
                        .First<tbAdquirente>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS

                if (param.nmAdquirente != null && param.nmAdquirente != value.nmAdquirente)
                    value.nmAdquirente = param.nmAdquirente;
                if (param.dsAdquirente != null && param.dsAdquirente != value.dsAdquirente)
                    value.dsAdquirente = param.dsAdquirente;
                if (param.stAdquirente != value.stAdquirente)
                    value.stAdquirente = param.stAdquirente;
                if (param.hrExecucao != null && param.hrExecucao != value.hrExecucao)
                    value.hrExecucao = param.hrExecucao;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar adquirente" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
