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
    public class GatewayTbBandeira
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbBandeira()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDBANDEIRA = 100,
            DSBANDEIRA = 101,
            CDADQUIRENTE = 102,
            DSTIPO = 103,

        };

        /// <summary>
        /// Get TbBandeira/TbBandeira
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbBandeira> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbBandeiras.AsQueryable<tbBandeira>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira.Equals(cdBandeira)).AsQueryable<tbBandeira>();
                        break;
                    case CAMPOS.DSBANDEIRA:
                        string dsBandeira = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsBandeira.Equals(dsBandeira)).AsQueryable<tbBandeira>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbBandeira>();
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value).TrimEnd();
                        entity = entity.Where(e => e.dsTipo.TrimEnd().Equals(dsTipo)).AsQueryable<tbBandeira>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbBandeira>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbBandeira>();
                    break;
                case CAMPOS.DSBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsBandeira).AsQueryable<tbBandeira>();
                    else entity = entity.OrderByDescending(e => e.dsBandeira).AsQueryable<tbBandeira>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbBandeira>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbBandeira>();
                    break;
                case CAMPOS.DSTIPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsTipo).AsQueryable<tbBandeira>();
                    else entity = entity.OrderByDescending(e => e.dsTipo).AsQueryable<tbBandeira>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbBandeira/TbBandeira
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbBandeira = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);


            // TOTAL DE REGISTROS
            retorno.TotalDeRegistros = query.Count();


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
                CollectionTbBandeira = query.Select(e => new
                {

                    cdBandeira = e.cdBandeira,
                    dsBandeira = e.dsBandeira,
                    cdAdquirente = e.cdAdquirente,
                    dsTipo = e.dsTipo,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbBandeira = query.Select(e => new
                {

                    cdBandeira = e.cdBandeira,
                    dsBandeira = e.dsBandeira,
                    cdAdquirente = e.cdAdquirente,
                    dsTipo = e.dsTipo,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbBandeira;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova TbBandeira
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbBandeira param)
        {
            _db.tbBandeiras.Add(param);
            _db.SaveChanges();
            return param.cdBandeira;
        }


        /// <summary>
        /// Apaga uma TbBandeira
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdBandeira)
        {
            _db.tbBandeiras.Remove(_db.tbBandeiras.Where(e => e.cdBandeira == cdBandeira).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera tbBandeira
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbBandeira param)
        {
            tbBandeira value = _db.tbBandeiras
                    .Where(e => e.cdBandeira == param.cdBandeira)
                    .First<tbBandeira>();


            if (param.cdBandeira != value.cdBandeira)
                value.cdBandeira = param.cdBandeira;
            if (param.dsBandeira != null && param.dsBandeira != value.dsBandeira)
                value.dsBandeira = param.dsBandeira;
            if (param.cdAdquirente != value.cdAdquirente)
                value.cdAdquirente = param.cdAdquirente;
            if (param.dsTipo != null && param.dsTipo != value.dsTipo)
                value.dsTipo = param.dsTipo;
            _db.SaveChanges();

        }

    }
}