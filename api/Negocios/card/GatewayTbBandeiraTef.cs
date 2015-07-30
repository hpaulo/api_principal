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
    public class GatewayTbBandeiraTef
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbBandeiraTef()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDBANDEIRATEF = 100,
            DSBANDEIRATEF = 101,

        };

        /// <summary>
        /// Get TbBandeiraTef/TbBandeiraTef
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbBandeiraTef> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbBandeiraTefs.AsQueryable<tbBandeiraTef>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDBANDEIRATEF:
                        short cdBandeiraTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdBandeiraTef.Equals(cdBandeiraTef)).AsQueryable<tbBandeiraTef>();
                        break;
                    case CAMPOS.DSBANDEIRATEF:
                        string dsBandeiraTef = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsBandeiraTef.Equals(dsBandeiraTef)).AsQueryable<tbBandeiraTef>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDBANDEIRATEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeiraTef).AsQueryable<tbBandeiraTef>();
                    else entity = entity.OrderByDescending(e => e.cdBandeiraTef).AsQueryable<tbBandeiraTef>();
                    break;
                case CAMPOS.DSBANDEIRATEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsBandeiraTef).AsQueryable<tbBandeiraTef>();
                    else entity = entity.OrderByDescending(e => e.dsBandeiraTef).AsQueryable<tbBandeiraTef>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbBandeiraTef/TbBandeiraTef
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbBandeiraTef = new List<dynamic>();
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
                CollectionTbBandeiraTef = query.Select(e => new
                {

                    cdBandeiraTef = e.cdBandeiraTef,
                    dsBandeiraTef = e.dsBandeiraTef,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbBandeiraTef = query.Select(e => new
                {

                    cdBandeiraTef = e.cdBandeiraTef,
                    dsBandeiraTef = e.dsBandeiraTef,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbBandeiraTef;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbBandeiraTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbBandeiraTef param)
        {
            _db.tbBandeiraTefs.Add(param);
            _db.SaveChanges();
            return param.cdBandeiraTef;
        }


        /// <summary>
        /// Apaga uma TbBandeiraTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdBandeiraTef)
        {
            _db.tbBandeiraTefs.Remove(_db.tbBandeiraTefs.Where(e => e.cdBandeiraTef.Equals(cdBandeiraTef)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbBandeiraTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbBandeiraTef param)
        {
            tbBandeiraTef value = _db.tbBandeiraTefs
                    .Where(e => e.cdBandeiraTef.Equals(param.cdBandeiraTef))
                    .First<tbBandeiraTef>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.cdBandeiraTef != null && param.cdBandeiraTef != value.cdBandeiraTef)
                value.cdBandeiraTef = param.cdBandeiraTef;
            if (param.dsBandeiraTef != null && param.dsBandeiraTef != value.dsBandeiraTef)
                value.dsBandeiraTef = param.dsBandeiraTef;
            _db.SaveChanges();

        }

    }
}
