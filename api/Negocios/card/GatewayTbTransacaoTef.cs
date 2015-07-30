using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Card
{
    public class GatewayTbTransacaoTef
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbTransacaoTef()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDTRANSACAOTEF = 100,
            DSTRANSACAOTEF = 101,
            DSABREVIADATRANSACAOTEF = 102,

        };

        /// <summary>
        /// Get TbTransacaoTef/TbTransacaoTef
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbTransacaoTef> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbTransacaoTefs.AsQueryable<tbTransacaoTef>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDTRANSACAOTEF:
                        Int32 cdTransacaoTef = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdTransacaoTef.Equals(cdTransacaoTef)).AsQueryable<tbTransacaoTef>();
                        break;
                    case CAMPOS.DSTRANSACAOTEF:
                        string dsTransacaoTef = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsTransacaoTef.Equals(dsTransacaoTef)).AsQueryable<tbTransacaoTef>();
                        break;
                    case CAMPOS.DSABREVIADATRANSACAOTEF:
                        string dsAbreviadaTransacaoTef = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsAbreviadaTransacaoTef.Equals(dsAbreviadaTransacaoTef)).AsQueryable<tbTransacaoTef>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDTRANSACAOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTransacaoTef).AsQueryable<tbTransacaoTef>();
                    else entity = entity.OrderByDescending(e => e.cdTransacaoTef).AsQueryable<tbTransacaoTef>();
                    break;
                case CAMPOS.DSTRANSACAOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsTransacaoTef).AsQueryable<tbTransacaoTef>();
                    else entity = entity.OrderByDescending(e => e.dsTransacaoTef).AsQueryable<tbTransacaoTef>();
                    break;
                case CAMPOS.DSABREVIADATRANSACAOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsAbreviadaTransacaoTef).AsQueryable<tbTransacaoTef>();
                    else entity = entity.OrderByDescending(e => e.dsAbreviadaTransacaoTef).AsQueryable<tbTransacaoTef>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbTransacaoTef/TbTransacaoTef
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbTransacaoTef = new List<dynamic>();
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
                CollectionTbTransacaoTef = query.Select(e => new
                {

                    cdTransacaoTef = e.cdTransacaoTef,
                    dsTransacaoTef = e.dsTransacaoTef,
                    dsAbreviadaTransacaoTef = e.dsAbreviadaTransacaoTef,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbTransacaoTef = query.Select(e => new
                {

                    cdTransacaoTef = e.cdTransacaoTef,
                    dsTransacaoTef = e.dsTransacaoTef,
                    dsAbreviadaTransacaoTef = e.dsAbreviadaTransacaoTef,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbTransacaoTef;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbTransacaoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbTransacaoTef param)
        {
            _db.tbTransacaoTefs.Add(param);
            _db.SaveChanges();
            return param.cdTransacaoTef;
        }


        /// <summary>
        /// Apaga uma TbTransacaoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdTransacaoTef)
        {
            _db.tbTransacaoTefs.Remove(_db.tbTransacaoTefs.Where(e => e.cdTransacaoTef.Equals(cdTransacaoTef)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbTransacaoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbTransacaoTef param)
        {
            tbTransacaoTef value = _db.tbTransacaoTefs
                    .Where(e => e.cdTransacaoTef.Equals(param.cdTransacaoTef))
                    .First<tbTransacaoTef>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.cdTransacaoTef != null && param.cdTransacaoTef != value.cdTransacaoTef)
                value.cdTransacaoTef = param.cdTransacaoTef;
            if (param.dsTransacaoTef != null && param.dsTransacaoTef != value.dsTransacaoTef)
                value.dsTransacaoTef = param.dsTransacaoTef;
            if (param.dsAbreviadaTransacaoTef != null && param.dsAbreviadaTransacaoTef != value.dsAbreviadaTransacaoTef)
                value.dsAbreviadaTransacaoTef = param.dsAbreviadaTransacaoTef;
            _db.SaveChanges();

        }

    }
}
