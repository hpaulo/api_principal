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
    public class GatewayTbModoEntradaTef
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbModoEntradaTef()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDMODOENTRADATEF = 100,
            DSMODOENTRADATEF = 101,

        };

        /// <summary>
        /// Get TbModoEntradaTef/TbModoEntradaTef
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbModoEntradaTef> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbModoEntradaTefs.AsQueryable<tbModoEntradaTef>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDMODOENTRADATEF:
                        short cdModoEntradaTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdModoEntradaTef.Equals(cdModoEntradaTef)).AsQueryable<tbModoEntradaTef>();
                        break;
                    case CAMPOS.DSMODOENTRADATEF:
                        string dsModoEntradaTef = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsModoEntradaTef.Equals(dsModoEntradaTef)).AsQueryable<tbModoEntradaTef>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDMODOENTRADATEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdModoEntradaTef).AsQueryable<tbModoEntradaTef>();
                    else entity = entity.OrderByDescending(e => e.cdModoEntradaTef).AsQueryable<tbModoEntradaTef>();
                    break;
                case CAMPOS.DSMODOENTRADATEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsModoEntradaTef).AsQueryable<tbModoEntradaTef>();
                    else entity = entity.OrderByDescending(e => e.dsModoEntradaTef).AsQueryable<tbModoEntradaTef>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbModoEntradaTef/TbModoEntradaTef
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbModoEntradaTef = new List<dynamic>();
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
                    CollectionTbModoEntradaTef = query.Select(e => new
                    {

                        cdModoEntradaTef = e.cdModoEntradaTef,
                        dsModoEntradaTef = e.dsModoEntradaTef,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbModoEntradaTef = query.Select(e => new
                    {

                        cdModoEntradaTef = e.cdModoEntradaTef,
                        dsModoEntradaTef = e.dsModoEntradaTef,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbModoEntradaTef;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar modo entrada tef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbModoEntradaTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbModoEntradaTef param)
        {
            try
            {
                _db.tbModoEntradaTefs.Add(param);
                _db.SaveChanges();
                return param.cdModoEntradaTef;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar modo entrada tef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbModoEntradaTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdModoEntradaTef)
        {
            try
            {
                _db.tbModoEntradaTefs.Remove(_db.tbModoEntradaTefs.Where(e => e.cdModoEntradaTef.Equals(cdModoEntradaTef)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar modo entrada tef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera tbModoEntradaTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbModoEntradaTef param)
        {
            try
            {
                tbModoEntradaTef value = _db.tbModoEntradaTefs
                        .Where(e => e.cdModoEntradaTef.Equals(param.cdModoEntradaTef))
                        .First<tbModoEntradaTef>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.cdModoEntradaTef != null && param.cdModoEntradaTef != value.cdModoEntradaTef)
                    value.cdModoEntradaTef = param.cdModoEntradaTef;
                if (param.dsModoEntradaTef != null && param.dsModoEntradaTef != value.dsModoEntradaTef)
                    value.dsModoEntradaTef = param.dsModoEntradaTef;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar modo entrada tef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
