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
    public class GatewayTbTerminalLogico
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbTerminalLogico()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDTERMINALLOGICO = 100,
            CDADQUIRENTE = 101,
            NRCNPJ = 102,

        };

        /// <summary>
        /// Get TbTerminalLogico/TbTerminalLogico
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbTerminalLogico> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbTerminalLogicos.AsQueryable<tbTerminalLogico>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.CDTERMINALLOGICO:
                        string cdTerminalLogico = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdTerminalLogico.Equals(cdTerminalLogico)).AsQueryable<tbTerminalLogico>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbTerminalLogico>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbTerminalLogico>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.CDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTerminalLogico).AsQueryable<tbTerminalLogico>();
                    else entity = entity.OrderByDescending(e => e.cdTerminalLogico).AsQueryable<tbTerminalLogico>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbTerminalLogico>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbTerminalLogico>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbTerminalLogico>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbTerminalLogico>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbTerminalLogico/TbTerminalLogico
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbTerminalLogico = new List<dynamic>();
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
                CollectionTbTerminalLogico = query.Select(e => new
                {

                    cdTerminalLogico = e.cdTerminalLogico,
                    cdAdquirente = e.cdAdquirente,
                    nrCNPJ = e.nrCNPJ,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbTerminalLogico = query.Select(e => new
                {

                    cdTerminalLogico = e.cdTerminalLogico,
                    cdAdquirente = e.cdAdquirente,
                    nrCNPJ = e.nrCNPJ,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbTerminalLogico;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova TbTerminalLogico
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Add(string token, tbTerminalLogico param)
        {
            _db.tbTerminalLogicos.Add(param);
            _db.SaveChanges();
            return param.cdTerminalLogico;
        }


        /// <summary>
        /// Apaga uma TbTerminalLogico
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, string cdTerminalLogico)
        {
            _db.tbTerminalLogicos.Remove(_db.tbTerminalLogicos.Where(e => e.cdTerminalLogico.Equals(cdTerminalLogico)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera tbTerminalLogico
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbTerminalLogico param)
        {
            tbTerminalLogico value = _db.tbTerminalLogicos
                    .Where(e => e.cdTerminalLogico.Equals(param.cdTerminalLogico))
                    .First<tbTerminalLogico>();


            if (param.cdTerminalLogico != null && param.cdTerminalLogico != value.cdTerminalLogico)
                value.cdTerminalLogico = param.cdTerminalLogico;
            if (param.cdAdquirente != value.cdAdquirente)
                value.cdAdquirente = param.cdAdquirente;
            if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                value.nrCNPJ = param.nrCNPJ;
            _db.SaveChanges();

        }

    }
}