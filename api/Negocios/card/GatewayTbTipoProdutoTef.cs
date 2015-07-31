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
    public class GatewayTbTipoProdutoTef
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbTipoProdutoTef()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDTIPOPRODUTOTEF = 100,
            DSTIPOPRODUTOTEF = 101,

        };

        /// <summary>
        /// Get TbTipoProdutoTef/TbTipoProdutoTef
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbTipoProdutoTef> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbTipoProdutoTefs.AsQueryable<tbTipoProdutoTef>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDTIPOPRODUTOTEF:
                        short cdTipoProdutoTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdTipoProdutoTef.Equals(cdTipoProdutoTef)).AsQueryable<tbTipoProdutoTef>();
                        break;
                    case CAMPOS.DSTIPOPRODUTOTEF:
                        string dsTipoProdutoTef = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsTipoProdutoTef.Equals(dsTipoProdutoTef)).AsQueryable<tbTipoProdutoTef>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDTIPOPRODUTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTipoProdutoTef).AsQueryable<tbTipoProdutoTef>();
                    else entity = entity.OrderByDescending(e => e.cdTipoProdutoTef).AsQueryable<tbTipoProdutoTef>();
                    break;
                case CAMPOS.DSTIPOPRODUTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsTipoProdutoTef).AsQueryable<tbTipoProdutoTef>();
                    else entity = entity.OrderByDescending(e => e.dsTipoProdutoTef).AsQueryable<tbTipoProdutoTef>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbTipoProdutoTef/TbTipoProdutoTef
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbTipoProdutoTef = new List<dynamic>();
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
                CollectionTbTipoProdutoTef = query.Select(e => new
                {

                    cdTipoProdutoTef = e.cdTipoProdutoTef,
                    dsTipoProdutoTef = e.dsTipoProdutoTef,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbTipoProdutoTef = query.Select(e => new
                {

                    cdTipoProdutoTef = e.cdTipoProdutoTef,
                    dsTipoProdutoTef = e.dsTipoProdutoTef,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbTipoProdutoTef;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbTipoProdutoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbTipoProdutoTef param)
        {
            _db.tbTipoProdutoTefs.Add(param);
            _db.SaveChanges();
            return param.cdTipoProdutoTef;
        }


        /// <summary>
        /// Apaga uma TbTipoProdutoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdTipoProdutoTef)
        {
            _db.tbTipoProdutoTefs.Remove(_db.tbTipoProdutoTefs.Where(e => e.cdTipoProdutoTef.Equals(cdTipoProdutoTef)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbTipoProdutoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbTipoProdutoTef param)
        {
            tbTipoProdutoTef value = _db.tbTipoProdutoTefs
                    .Where(e => e.cdTipoProdutoTef.Equals(param.cdTipoProdutoTef))
                    .First<tbTipoProdutoTef>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.cdTipoProdutoTef != null && param.cdTipoProdutoTef != value.cdTipoProdutoTef)
                value.cdTipoProdutoTef = param.cdTipoProdutoTef;
            if (param.dsTipoProdutoTef != null && param.dsTipoProdutoTef != value.dsTipoProdutoTef)
                value.dsTipoProdutoTef = param.dsTipoProdutoTef;
            _db.SaveChanges();

        }

    }
}
