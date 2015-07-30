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
    public class GatewayTbRedeTef
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRedeTef()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDREDETEF = 100,
            DSREDETEF = 101,

        };

        /// <summary>
        /// Get TbRedeTef/TbRedeTef
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbRedeTef> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbRedeTefs.AsQueryable<tbRedeTef>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDREDETEF:
                        short cdRedeTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdRedeTef.Equals(cdRedeTef)).AsQueryable<tbRedeTef>();
                        break;
                    case CAMPOS.DSREDETEF:
                        string dsRedeTef = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsRedeTef.Equals(dsRedeTef)).AsQueryable<tbRedeTef>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDREDETEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdRedeTef).AsQueryable<tbRedeTef>();
                    else entity = entity.OrderByDescending(e => e.cdRedeTef).AsQueryable<tbRedeTef>();
                    break;
                case CAMPOS.DSREDETEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsRedeTef).AsQueryable<tbRedeTef>();
                    else entity = entity.OrderByDescending(e => e.dsRedeTef).AsQueryable<tbRedeTef>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbRedeTef/TbRedeTef
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbRedeTef = new List<dynamic>();
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
                CollectionTbRedeTef = query.Select(e => new
                {

                    cdRedeTef = e.cdRedeTef,
                    dsRedeTef = e.dsRedeTef,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbRedeTef = query.Select(e => new
                {

                    cdRedeTef = e.cdRedeTef,
                    dsRedeTef = e.dsRedeTef,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbRedeTef;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbRedeTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbRedeTef param)
        {
            _db.tbRedeTefs.Add(param);
            _db.SaveChanges();
            return param.cdRedeTef;
        }


        /// <summary>
        /// Apaga uma TbRedeTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdRedeTef)
        {
            _db.tbRedeTefs.Remove(_db.tbRedeTefs.Where(e => e.cdRedeTef.Equals(cdRedeTef)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbRedeTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbRedeTef param)
        {
            tbRedeTef value = _db.tbRedeTefs
                    .Where(e => e.cdRedeTef.Equals(param.cdRedeTef))
                    .First<tbRedeTef>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.cdRedeTef != null && param.cdRedeTef != value.cdRedeTef)
                value.cdRedeTef = param.cdRedeTef;
            if (param.dsRedeTef != null && param.dsRedeTef != value.dsRedeTef)
                value.dsRedeTef = param.dsRedeTef;
            _db.SaveChanges();

        }

    }
}
