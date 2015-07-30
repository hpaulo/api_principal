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
    public class GatewayTbProdutoTef
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbProdutoTef()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDPRODUTOTEF = 100,
            CDTIPOPRODUTOTEF = 101,
            DSPRODUTOTEF = 102,

        };

        /// <summary>
        /// Get TbProdutoTef/TbProdutoTef
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbProdutoTef> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbProdutoTefs.AsQueryable<tbProdutoTef>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDPRODUTOTEF:
                        short cdProdutoTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdProdutoTef.Equals(cdProdutoTef)).AsQueryable<tbProdutoTef>();
                        break;
                    case CAMPOS.CDTIPOPRODUTOTEF:
                        short cdTipoProdutoTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdTipoProdutoTef.Equals(cdTipoProdutoTef)).AsQueryable<tbProdutoTef>();
                        break;
                    case CAMPOS.DSPRODUTOTEF:
                        string dsProdutoTef = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsProdutoTef.Equals(dsProdutoTef)).AsQueryable<tbProdutoTef>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDPRODUTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdProdutoTef).AsQueryable<tbProdutoTef>();
                    else entity = entity.OrderByDescending(e => e.cdProdutoTef).AsQueryable<tbProdutoTef>();
                    break;
                case CAMPOS.CDTIPOPRODUTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTipoProdutoTef).AsQueryable<tbProdutoTef>();
                    else entity = entity.OrderByDescending(e => e.cdTipoProdutoTef).AsQueryable<tbProdutoTef>();
                    break;
                case CAMPOS.DSPRODUTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsProdutoTef).AsQueryable<tbProdutoTef>();
                    else entity = entity.OrderByDescending(e => e.dsProdutoTef).AsQueryable<tbProdutoTef>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbProdutoTef/TbProdutoTef
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbProdutoTef = new List<dynamic>();
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
                CollectionTbProdutoTef = query.Select(e => new
                {

                    cdProdutoTef = e.cdProdutoTef,
                    cdTipoProdutoTef = e.cdTipoProdutoTef,
                    dsProdutoTef = e.dsProdutoTef,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbProdutoTef = query.Select(e => new
                {

                    cdProdutoTef = e.cdProdutoTef,
                    cdTipoProdutoTef = e.cdTipoProdutoTef,
                    dsProdutoTef = e.dsProdutoTef,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbProdutoTef;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbProdutoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbProdutoTef param)
        {
            _db.tbProdutoTefs.Add(param);
            _db.SaveChanges();
            return param.cdProdutoTef;
        }


        /// <summary>
        /// Apaga uma TbProdutoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdProdutoTef)
        {
            _db.tbProdutoTefs.Remove(_db.tbProdutoTefs.Where(e => e.cdProdutoTef.Equals(cdProdutoTef)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbProdutoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbProdutoTef param)
        {
            tbProdutoTef value = _db.tbProdutoTefs
                    .Where(e => e.cdProdutoTef.Equals(param.cdProdutoTef))
                    .First<tbProdutoTef>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.cdProdutoTef != null && param.cdProdutoTef != value.cdProdutoTef)
                value.cdProdutoTef = param.cdProdutoTef;
            if (param.cdTipoProdutoTef != null && param.cdTipoProdutoTef != value.cdTipoProdutoTef)
                value.cdTipoProdutoTef = param.cdTipoProdutoTef;
            if (param.dsProdutoTef != null && param.dsProdutoTef != value.dsProdutoTef)
                value.dsProdutoTef = param.dsProdutoTef;
            _db.SaveChanges();

        }

    }
}
