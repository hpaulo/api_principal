using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Data;
using api.Negocios.Util;
using System.Globalization;
using api.Negocios.Cliente;

namespace api.Negocios.Card
{
    public class GatewayTbAntecipacaoBancariaDetalhe
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbAntecipacaoBancariaDetalhe()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "ATD";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDANTECIPACAOBANCARIADETALHE = 100,
            IDANTECIPACAOBANCARIA = 101,
            DTVENCIMENTO = 102,
            VLANTECIPACAO = 103,
            VLANTECIPACAOLIQUIDA = 104,
            CDBANDEIRA = 105,

            // RELACIONAMENTOS
            CDCONTACORRENTE = 201,
            DTANTECIPACAOBANCARIA = 202,
            CDADQUIRENTE = 203,

            ID_GRUPO = 301,
        };

        /// <summary>
        /// Get TbAntecipacaoBancariaDetalhe/TbAntecipacaoBancariaDetalhe
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbAntecipacaoBancariaDetalhe> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbAntecipacaoBancariaDetalhes.AsQueryable<tbAntecipacaoBancariaDetalhe>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDANTECIPACAOBANCARIADETALHE:
                        Int32 idAntecipacaoBancariaDetalhe = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idAntecipacaoBancariaDetalhe.Equals(idAntecipacaoBancariaDetalhe)).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        break;
                    case CAMPOS.IDANTECIPACAOBANCARIA:
                        Int32 idAntecipacaoBancaria = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idAntecipacaoBancaria.Equals(idAntecipacaoBancaria)).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        break;
                    case CAMPOS.DTVENCIMENTO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtVencimento.Year > dtaIni.Year || (e.dtVencimento.Year == dtaIni.Year && e.dtVencimento.Month > dtaIni.Month) ||
                                                                                          (e.dtVencimento.Year == dtaIni.Year && e.dtVencimento.Month == dtaIni.Month && e.dtVencimento.Day >= dtaIni.Day))
                                                    && (e.dtVencimento.Year < dtaFim.Year || (e.dtVencimento.Year == dtaFim.Year && e.dtVencimento.Month < dtaFim.Month) ||
                                                                                          (e.dtVencimento.Year == dtaFim.Year && e.dtVencimento.Month == dtaFim.Month && e.dtVencimento.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVencimento >= dta);
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVencimento <= dta);
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVencimento.Year == dtaIni.Year && e.dtVencimento.Month == dtaIni.Month && e.dtVencimento.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.VLANTECIPACAO:
                        decimal vlAntecipacao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlAntecipacao.Equals(vlAntecipacao)).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        break;
                    case CAMPOS.VLANTECIPACAOLIQUIDA:
                        decimal vlAntecipacaoLiquida = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlAntecipacaoLiquida.Equals(vlAntecipacaoLiquida)).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira.Equals(cdBandeira)).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.DTANTECIPACAOBANCARIA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Year > dtaIni.Year || (e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Year == dtaIni.Year && e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Month > dtaIni.Month) ||
                                                                                          (e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Year == dtaIni.Year && e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Month == dtaIni.Month && e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Day >= dtaIni.Day))
                                                    && (e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Year < dtaFim.Year || (e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Year == dtaFim.Year && e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Month < dtaFim.Month) ||
                                                                                          (e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Year == dtaFim.Year && e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Month == dtaFim.Month && e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Day <= dtaFim.Day)));
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Year == dtaIni.Year && e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Month == dtaIni.Month && e.tbAntecipacaoBancaria.dtAntecipacaoBancaria.Day == dtaIni.Day);
                        }
                        break;

                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbAntecipacaoBancaria.cdAdquirente == cdAdquirente).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbAntecipacaoBancaria.cdContaCorrente == cdContaCorrente).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbAntecipacaoBancaria.tbContaCorrente.empresa.id_grupo == id_grupo).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDANTECIPACAOBANCARIADETALHE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idAntecipacaoBancariaDetalhe).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.idAntecipacaoBancariaDetalhe).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    break;
                case CAMPOS.IDANTECIPACAOBANCARIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idAntecipacaoBancaria).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.idAntecipacaoBancaria).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    break;
                case CAMPOS.DTVENCIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVencimento).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.dtVencimento).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    break;
                case CAMPOS.VLANTECIPACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlAntecipacao).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.vlAntecipacao).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    break;
                case CAMPOS.VLANTECIPACAOLIQUIDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlAntecipacaoLiquida).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.vlAntecipacaoLiquida).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Get TbAntecipacaoBancariaDetalhe/TbAntecipacaoBancariaDetalhe
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static SimpleDataBaseQuery getQuery(int campo, int orderby, Dictionary<string, string> queryString)
        {
            Dictionary<string, string> join = new Dictionary<string, string>();
            List<string> where = new List<string>();
            List<string> order = new List<string>();

            #region WHERE - ADICIONA OS FILTROS A QUERY
            // ADICIONA OS FILTROS A QUERY
            foreach (KeyValuePair<string, string> item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDANTECIPACAOBANCARIADETALHE:
                        Int32 idAntecipacaoBancariaDetalhe = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe = " + idAntecipacaoBancariaDetalhe);
                        break;
                    case CAMPOS.IDANTECIPACAOBANCARIA:
                        Int32 idAntecipacaoBancaria = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idAntecipacaoBancaria = " + idAntecipacaoBancaria);
                        break;
                    case CAMPOS.DTVENCIMENTO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(SIGLA_QUERY + ".dtVencimento BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtVencimento >= '" + dt + "'");
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtVencimento <= '" + dt + "'");
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtVencimento BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;
                    case CAMPOS.VLANTECIPACAO:
                        decimal vlAntecipacao = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlAntecipacao = " + vlAntecipacao.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.VLANTECIPACAOLIQUIDA:
                        decimal vlAntecipacaoLiquida = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlAntecipacaoLiquida = " + vlAntecipacaoLiquida.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".cdBandeira = " + cdBandeira);
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.CDADQUIRENTE:
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".idAntecipacaoBancaria = " + SIGLA_QUERY + ".idAntecipacaoBancaria");
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        where.Add(GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente);
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".idAntecipacaoBancaria = " + SIGLA_QUERY + ".idAntecipacaoBancaria");
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        where.Add(GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".cdContaCorrente = " + cdContaCorrente);
                        break;
                    case CAMPOS.DTANTECIPACAOBANCARIA:
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".idAntecipacaoBancaria = " + SIGLA_QUERY + ".idAntecipacaoBancaria");
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".dtAntecipacaoBancaria BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".dtAntecipacaoBancaria BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;
                    case CAMPOS.ID_GRUPO:
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".idAntecipacaoBancaria = " + SIGLA_QUERY + ".idAntecipacaoBancaria");
                        if (!join.ContainsKey("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".cdContaCorrente");
                        if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj = " + GatewayTbContaCorrente.SIGLA_QUERY + ".nrCnpj");
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        where.Add(GatewayEmpresa.SIGLA_QUERY + ".id_grupo = " + id_grupo);
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDANTECIPACAOBANCARIADETALHE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe ASC");
                    else order.Add(SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe DESC");
                    break;
                case CAMPOS.IDANTECIPACAOBANCARIA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idAntecipacaoBancaria ASC");
                    else order.Add(SIGLA_QUERY + ".idAntecipacaoBancaria DESC");
                    break;
                case CAMPOS.DTVENCIMENTO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dtVencimento ASC");
                    else order.Add(SIGLA_QUERY + ".dtVencimento DESC");
                    break;
                case CAMPOS.VLANTECIPACAO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlAntecipacao ASC");
                    else order.Add(SIGLA_QUERY + ".vlAntecipacao DESC");
                    break;
                case CAMPOS.VLANTECIPACAOLIQUIDA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlAntecipacaoLiquida ASC");
                    else order.Add(SIGLA_QUERY + ".vlAntecipacaoLiquida DESC");
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdBandeira ASC");
                    else order.Add(SIGLA_QUERY + ".cdBandeira DESC");
                    break;
            }
            #endregion

            return new SimpleDataBaseQuery(null, "card.tbAntecipacaoBancariaDetalhe " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());


        }


        /// <summary>
        /// Retorna TbAntecipacaoBancariaDetalhe/TbAntecipacaoBancariaDetalhe
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbAntecipacaoBancariaDetalhe = new List<dynamic>();
                Retorno retorno = new Retorno();

                // GET QUERY
                var query = getQuery(_db, colecao, campo, orderBy, pageSize, pageNumber, queryString);


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
                    CollectionTbAntecipacaoBancariaDetalhe = query.Select(e => new
                    {

                        idAntecipacaoBancariaDetalhe = e.idAntecipacaoBancariaDetalhe,
                        idAntecipacaoBancaria = e.idAntecipacaoBancaria,
                        dtVencimento = e.dtVencimento,
                        vlAntecipacao = e.vlAntecipacao,
                        vlAntecipacaoLiquida = e.vlAntecipacaoLiquida,
                        cdBandeira = e.cdBandeira,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbAntecipacaoBancariaDetalhe = query.Select(e => new
                    {

                        idAntecipacaoBancariaDetalhe = e.idAntecipacaoBancariaDetalhe,
                        idAntecipacaoBancaria = e.idAntecipacaoBancaria,
                        dtVencimento = e.dtVencimento,
                        vlAntecipacao = e.vlAntecipacao,
                        vlAntecipacaoLiquida = e.vlAntecipacaoLiquida,
                        cdBandeira = e.cdBandeira,
                    }).ToList<dynamic>();
                }

                transaction.Commit();

                retorno.Registros = CollectionTbAntecipacaoBancariaDetalhe;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a baixa automática" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }
        /// <summary>
        /// Adiciona nova TbAntecipacaoBancariaDetalhe
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbAntecipacaoBancariaDetalhe param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.tbAntecipacaoBancariaDetalhes.Add(param);
                _db.SaveChanges();
                transaction.Commit();
                return param.idAntecipacaoBancariaDetalhe;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a baixa automática" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }


        /// <summary>
        /// Apaga uma TbAntecipacaoBancariaDetalhe
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idAntecipacaoBancariaDetalhe, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.tbAntecipacaoBancariaDetalhes.Remove(_db.tbAntecipacaoBancariaDetalhes.Where(e => e.idAntecipacaoBancariaDetalhe.Equals(idAntecipacaoBancariaDetalhe)).First());
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a baixa automática" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }
        /// <summary>
        /// Altera tbAntecipacaoBancariaDetalhe
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbAntecipacaoBancariaDetalhe param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbAntecipacaoBancariaDetalhe value = _db.tbAntecipacaoBancariaDetalhes
                                .Where(e => e.idAntecipacaoBancariaDetalhe.Equals(param.idAntecipacaoBancariaDetalhe))
                                .First<tbAntecipacaoBancariaDetalhe>();
                if (param.idAntecipacaoBancariaDetalhe != null && param.idAntecipacaoBancariaDetalhe != value.idAntecipacaoBancariaDetalhe)
                    value.idAntecipacaoBancariaDetalhe = param.idAntecipacaoBancariaDetalhe;
                if (param.idAntecipacaoBancaria != null && param.idAntecipacaoBancaria != value.idAntecipacaoBancaria)
                    value.idAntecipacaoBancaria = param.idAntecipacaoBancaria;
                if (param.dtVencimento != null && param.dtVencimento != value.dtVencimento)
                    value.dtVencimento = param.dtVencimento;
                if (param.vlAntecipacao != null && param.vlAntecipacao != value.vlAntecipacao)
                    value.vlAntecipacao = param.vlAntecipacao;
                if (param.vlAntecipacaoLiquida != null && param.vlAntecipacaoLiquida != value.vlAntecipacaoLiquida)
                    value.vlAntecipacaoLiquida = param.vlAntecipacaoLiquida;
                if (param.cdBandeira != null && param.cdBandeira != value.cdBandeira)
                    value.cdBandeira = param.cdBandeira;
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a baixa automática" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }

    }
}