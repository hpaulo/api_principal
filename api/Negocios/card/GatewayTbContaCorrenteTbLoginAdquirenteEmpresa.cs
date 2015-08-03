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
    public class GatewayTbContaCorrenteTbLoginAdquirenteEmpresa
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbContaCorrenteTbLoginAdquirenteEmpresa()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDCONTACORRENTE = 100,
            CDLOGINADQUIRENTEEMPRESA = 101,
            DTINICIO = 102,
            DTFIM = 103,

        };

        /// <summary>
        /// Get TbContaCorrente_tbLoginAdquirenteEmpresa/TbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbContaCorrente_tbLoginAdquirenteEmpresas.AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdContaCorrente.Equals(cdContaCorrente)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.CDLOGINADQUIRENTEEMPRESA:
                        Int32 cdLoginAdquirenteEmpresa = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdLoginAdquirenteEmpresa.Equals(cdLoginAdquirenteEmpresa)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.DTINICIO:
                        DateTime dtInicio = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtInicio.Equals(dtInicio)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.DTFIM:
                        DateTime dtFim = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtFim.Equals(dtFim)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdContaCorrente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdContaCorrente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.CDLOGINADQUIRENTEEMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdLoginAdquirenteEmpresa).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdLoginAdquirenteEmpresa).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.DTINICIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtInicio).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dtInicio).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.DTFIM:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtFim).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dtFim).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbContaCorrente_tbLoginAdquirenteEmpresa/TbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = new List<dynamic>();
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
                CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = query.Select(e => new
                {

                    cdContaCorrente = e.cdContaCorrente,
                    cdLoginAdquirenteEmpresa = e.cdLoginAdquirenteEmpresa,
                    dtInicio = e.dtInicio,
                    dtFim = e.dtFim,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = query.Select(e => new
                {

                    cdContaCorrente = e.cdContaCorrente,
                    cdLoginAdquirenteEmpresa = e.cdLoginAdquirenteEmpresa,
                    dtInicio = e.dtInicio,
                    dtFim = e.dtFim,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbContaCorrente_tbLoginAdquirenteEmpresa;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbContaCorrente_tbLoginAdquirenteEmpresa param)
        {
            _db.tbContaCorrente_tbLoginAdquirenteEmpresas.Add(param);
            _db.SaveChanges();
            return param.cdContaCorrente;
        }


        /// <summary>
        /// Apaga uma TbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdContaCorrente)
        {
            _db.tbContaCorrente_tbLoginAdquirenteEmpresas.Remove(_db.tbContaCorrente_tbLoginAdquirenteEmpresas.Where(e => e.cdContaCorrente.Equals(cdContaCorrente)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbContaCorrente_tbLoginAdquirenteEmpresa param)
        {
            tbContaCorrente_tbLoginAdquirenteEmpresa value = _db.tbContaCorrente_tbLoginAdquirenteEmpresas
                    .Where(e => e.cdContaCorrente.Equals(param.cdContaCorrente))
                    .First<tbContaCorrente_tbLoginAdquirenteEmpresa>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.cdContaCorrente != null && param.cdContaCorrente != value.cdContaCorrente)
                value.cdContaCorrente = param.cdContaCorrente;
            if (param.cdLoginAdquirenteEmpresa != null && param.cdLoginAdquirenteEmpresa != value.cdLoginAdquirenteEmpresa)
                value.cdLoginAdquirenteEmpresa = param.cdLoginAdquirenteEmpresa;
            if (param.dtInicio != null && param.dtInicio != value.dtInicio)
                value.dtInicio = param.dtInicio;
            if (param.dtFim != null && param.dtFim != value.dtFim)
                value.dtFim = param.dtFim;
            _db.SaveChanges();

        }

    }
}
