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
using System.Data.SqlClient;
using System.Configuration;
using api.Negocios.Pos;

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

            SEM_PARCELAS_AJUSTES_ASSOCIADO = 400, // se true, não considera os vencimentos que provocaram antecipação de parcelas e/ou ajustes
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
                    case CAMPOS.SEM_PARCELAS_AJUSTES_ASSOCIADO:
                        if (Convert.ToBoolean(item.Value))
                        {
                            entity = entity.Where(e => e.RecebimentoParcelas.Count == 0 && e.tbRecebimentoAjustes.Count == 0).AsQueryable<tbAntecipacaoBancariaDetalhe>();
                        }
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
                    case CAMPOS.SEM_PARCELAS_AJUSTES_ASSOCIADO:
                        if (Convert.ToBoolean(item.Value))
                        {
                            if (!join.ContainsKey("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY))
                                join.Add("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY, " ON " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe = " + SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe");
                            if (!join.ContainsKey("LEFT JOIN card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY))
                                join.Add("LEFT JOIN card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY, " ON " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe = " + SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe");

                            where.Add(GatewayRecebimentoParcela.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe IS NULL");
                            where.Add(GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe IS NULL");
                        }
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
                        vlIOF = e.vlIOF,
                        vlIOFAdicional = e.vlIOFAdicional,
                        vlJuros = e.vlJuros,
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
                        vlIOF = e.vlIOF,
                        vlIOFAdicional = e.vlIOFAdicional,
                        vlJuros = e.vlJuros,
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





        public static void AntecipaParcelas(string token, AntecipacaoBancariaAnteciparParcelas param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                if(param == null || param.idsAntecipacaoBancariaDetalhe == null)
                    throw new Exception("Parâmetro inválido!");

                if (param.desfazerAntecipacoes)
                {
                    if (param.idsAntecipacaoBancariaDetalhe.Count > 0)
                    {
                        // Remove ajustes
                        _db.Database.ExecuteSqlCommand("DELETE A" +
                                                       " FROM card.tbRecebimentoAjuste A" +
                                                       " WHERE A.idAntecipacaoBancariaDetalhe IN (" + string.Join(", ", param.idsAntecipacaoBancariaDetalhe) + ")"
                                                      );
                        _db.SaveChanges();

                        _db.Database.ExecuteSqlCommand("UPDATE P" +
                                                       " SET P.flAntecipado = 0" +
                                                       ", P.vlDescontadoAntecipacao = 0" +
                                                       ", P.idAntecipacaoBancariaDetalhe = NULL" +
                                                       " FROM pos.RecebimentoParcela P" +
                                                       " WHERE P.idAntecipacaoBancariaDetalhe IN (" + string.Join(", ", param.idsAntecipacaoBancariaDetalhe) + ")"
                                                      );
                        _db.SaveChanges();
                    }
                }
                else
                {
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

                    try
                    {
                        connection.Open();
                    }
                    catch
                    {
                        throw new Exception("Não foi possível estabelecer conexão com a base de dados");
                    }

                    //List<dynamic> listaTeste = new List<dynamic>();

                    for (int i = 0; i < param.idsAntecipacaoBancariaDetalhe.Count; i++)
                    {
                        int idAntecipacaoBancariaDetalhe = param.idsAntecipacaoBancariaDetalhe[i];
                        // Obtém o vencimento
                        string script = "SELECT C.cdContaCorrente, C.cdBanco, A.dtAntecipacaoBancaria, D.dtVencimento, D.cdBandeira, D.vlAntecipacao, D.vlAntecipacaoLiquida" +
                                        " FROM card.tbAntecipacaoBancariaDetalhe D (NOLOCK)" +
                                        " JOIN card.tbAntecipacaoBancaria A ON A.idAntecipacaoBancaria = D.idAntecipacaoBancaria" +
                                        " JOIN card.tbContaCorrente C ON A.cdContaCorrente = C.cdContaCorrente" +
                                        " WHERE D.idAntecipacaoBancariaDetalhe = " + idAntecipacaoBancariaDetalhe;

                        List<IDataRecord> resultado = DataBaseQueries.SqlQuery(script, connection);

                        if (resultado == null || resultado.Count == 0)
                            throw new Exception("Vencimento " + idAntecipacaoBancariaDetalhe + " é inválido!");

                        IDataRecord antecipacao = resultado.FirstOrDefault();

                        string cdBanco = Convert.ToString(antecipacao["cdBanco"]);

                        if (!cdBanco.Equals("047")) // TEM QUE SER BANCO BANESE
                            continue;

                        int cdContaCorrente = Convert.ToInt32(antecipacao["cdContaCorrente"]);
                        DateTime dtAntecipacaoBancaria = (DateTime)antecipacao["dtAntecipacaoBancaria"];
                        DateTime dtVencimento = (DateTime)antecipacao["dtVencimento"];
                        int cdBandeira = antecipacao["cdBandeira"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(antecipacao["cdBandeira"]);
                        decimal vlAntecipacao = Convert.ToDecimal(antecipacao["vlAntecipacao"].Equals(DBNull.Value) ? 0.0 : antecipacao["vlAntecipacao"]);
                        decimal vlAntecipacaoLiquida = Convert.ToDecimal(antecipacao["vlAntecipacaoLiquida"].Equals(DBNull.Value) ? 0.0 : antecipacao["vlAntecipacaoLiquida"]);

                        //decimal taxaAntecipacao = decimal.Truncate(((vlAntecipacao - vlAntecipacaoLiquida) * new decimal(10000.0)) / vlAntecipacao) / new decimal(10000.0);
                        //decimal taxaAntecipacao = decimal.Round((vlAntecipacao - vlAntecipacaoLiquida) / vlAntecipacao, 6);
                        decimal taxaAntecipacao = (vlAntecipacao - vlAntecipacaoLiquida) / vlAntecipacao;

                        string[] cnpjsConta = Permissoes.GetFiliaisDaConta(cdContaCorrente, connection).ToArray();
                        string filiaisDaConta = "'" + string.Join("', '", cnpjsConta) + "'";

                        //var teste = new
                        //            {
                        //                dtAntecipacaoBancaria = dtAntecipacaoBancaria,
                        //                dtVencimento = dtVencimento,
                        //                vlAntecipacao = vlAntecipacao,
                        //                vlAntecipacaoLiquida = vlAntecipacaoLiquida,
                        //                parcelas = new List<dynamic>(),
                        //            };

                        // Busca parcelas
                        script = "SELECT P.idRecebimento, P.numParcela, P.valorParcelaBruta, P.valorDescontado, R.nsu, R.cnpj, R.cdBandeira" +
                                 " FROM pos.RecebimentoParcela P (NOLOCK)" +
                                 " JOIN pos.Recebimento R ON R.id = P.idRecebimento" +
                                 " JOIN card.tbBandeira B ON B.cdBandeira = R.cdBandeira" +
                                 // Procura estornos de vendas associados
                                 " LEFT JOIN ( SELECT A.nrCNPJ, A.dtAjuste, A.dsMotivo" +
                                 "             FROM card.tbRecebimentoAjuste A (NOLOCK)" +
                                 "           ) T ON T.nrCNPJ = R.cnpj" +
                                 "                  AND T.dtAjuste = P.dtaRecebimento" +
			                     "                  AND T.dsMotivo LIKE 'ESTORNO (OPERAÇÃO ASSOCIADA: ' + R.codResumoVenda + '%'" +
                                 " WHERE B.cdAdquirente = 7" + // Adquirente BANESE
                                 " AND T.dsMotivo IS NULL" + // SEM ESTORNO ASSOCIADO
                                 // Somente com data de venda inferior ao da operação
                                 " AND R.dtaVenda < '" + DataBaseQueries.GetDate(dtAntecipacaoBancaria) + "'" +
                                 //" AND R.dtaVenda >= '2016-02-19'" + // TEMP
                                 // Parcela com recebimento previsto para a data do vencimento
                                 " AND P.dtaRecebimento BETWEEN '" + DataBaseQueries.GetDate(dtVencimento) + "' AND '" + DataBaseQueries.GetDate(dtVencimento) + " 23:59:00'" +
                                 // Parcela não antecipada (e não conciliada) ou antecipada da antecipação bancária corrente
                                 " AND ((P.idAntecipacaoBancariaDetalhe IS NULL AND P.idExtrato IS NULL) OR P.idAntecipacaoBancariaDetalhe = " + idAntecipacaoBancariaDetalhe + ")" +
                                 // Parcelas das filiais com vigência para a conta corrente
                                 " AND R.cnpj in (" + filiaisDaConta + ")" +
                                 // Bandeira determinada do detalhe da antecipação. Se não determinada, somente as bandeiras à crédito
                                 " AND " + (cdBandeira > 0 ? "R.cdBandeira = " + cdBandeira : "B.dsTipo like 'CRÉDITO%'") +
                                 " ORDER BY CASE WHEN P.idAntecipacaoBancariaDetalhe IS NOT NULL THEN 0 ELSE 1 END" + // PRIORIZA OS QUE JÁ ESTÃO ASSOCIADOS A ANTECIPAÇÃO
                                         ", R.dtaVenda DESC, R.codResumoVenda, R.nsu"; // PRIORIZA OS DE VENDA MAIS RECENTE EM RELAÇÃO A DATA DA ANTECIPAÇÃO


                        resultado = DataBaseQueries.SqlQuery(script, connection);
                        if (resultado == null)// || resultado.Count == 0)
                            continue;

                        decimal valorUtilizado = new decimal(0.0);
                        decimal valorLiquidoUtilizado = new decimal(0.0);

                        for (int r = 0; r < resultado.Count && valorUtilizado < vlAntecipacao; r++)
                        {
                            IDataRecord parcela = resultado[r];
                            int idRecebimento = Convert.ToInt32(parcela["idRecebimento"]);
                            int numParcela = Convert.ToInt32(parcela["numParcela"]);
                            string nsu = Convert.ToString(parcela["nsu"]);
                            string cnpj = Convert.ToString(parcela["cnpj"]);
                            decimal valorParcelaBruta = Convert.ToDecimal(parcela["valorParcelaBruta"]);
                            decimal valorDescontado = Convert.ToDecimal(parcela["valorDescontado"]);
                            decimal valorDisponivel = valorParcelaBruta - valorDescontado;
                            int cdBandeiraParcela = Convert.ToInt32(parcela["cdBandeira"]);

                            // Obtém desconto de antecipação
                            decimal vlDescontadoAntecipacao = decimal.Round(valorDisponivel * taxaAntecipacao, 4);

                            decimal valorNecessario;
                            decimal ajuste = new decimal(0.0);

                            if (valorUtilizado + valorDisponivel <= vlAntecipacao)
                            {
                                // Usa parcela por completo
                                valorNecessario = valorDisponivel;
                                valorLiquidoUtilizado += valorDisponivel - vlDescontadoAntecipacao;
                            }
                            else
                            {
                                // Usa "parte" da parcela => cria um ajuste
                                valorNecessario = vlAntecipacao - valorUtilizado;

                                ajuste = decimal.Round(vlAntecipacaoLiquida - valorLiquidoUtilizado - (valorDisponivel - vlDescontadoAntecipacao), 2);
                            }
                            valorUtilizado += valorNecessario;

                            //teste.parcelas.Add(new
                            //{
                            //    idRecebimento = idRecebimento,
                            //    numParcela = numParcela,
                            //    valorDisponivel = valorDisponivel,
                            //    vlDescontadoAntecipacao = vlDescontadoAntecipacao,
                            //    valorNecessario = valorNecessario,
                            //    ajuste = ajuste,
                            //});

                            _db.Database.ExecuteSqlCommand("UPDATE P" +
                                                           " SET P.flAntecipado = 1" +
                                                           ", P.dtaRecebimentoEfetivo = '" + DataBaseQueries.GetDate(dtAntecipacaoBancaria) + "'" +
                                                           ", P.vlDescontadoAntecipacao = " + vlDescontadoAntecipacao.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                                                           ", P.idAntecipacaoBancariaDetalhe = " + idAntecipacaoBancariaDetalhe +
                                                           " FROM pos.RecebimentoParcela P" +
                                                           " WHERE P.numParcela = " + numParcela +
                                                           " AND P.idRecebimento = " + idRecebimento
                                                           );
                            _db.SaveChanges();


                            if (ajuste != new decimal(0.0))
                            {
                                // Cria ajuste, caso não exista
                                string dsMotivo = "SALDO ANTECIPAÇÃO BANCÁRIA NSU " + nsu + " PARCELA " + numParcela + " VENCIMENTO " + dtVencimento.ToShortDateString();
                                script = "SELECT A.*" +
                                         " FROM card.tbRecebimentoAjuste A (NOLOCK)" +
                                         " WHERE A.idAntecipacaoBancariaDetalhe = " + idAntecipacaoBancariaDetalhe;
                                resultado = DataBaseQueries.SqlQuery(script, connection);
                                if (resultado == null || resultado.Count == 0)
                                {
                                    // Cria
                                    _db.Database.ExecuteSqlCommand("INSERT INTO card.tbRecebimentoAjuste" +
                                                                   " (dsMotivo, cdBandeira, nrCNPJ, dtAjuste, vlAjuste, flAntecipacao, idAntecipacaoBancariaDetalhe)" +
                                                                   " VALUES ('" + dsMotivo + "'" +
                                                                   ", " + cdBandeiraParcela +
                                                                   ", '" + cnpj + "', '" + DataBaseQueries.GetDate(dtAntecipacaoBancaria) + "'" +
                                                                   ", " + ajuste.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                                                                   ", 1" +
                                                                   ", " + idAntecipacaoBancariaDetalhe + ")");
                                }
                                else
                                {
                                    // Atualiza valor
                                    int idRecebimentoAjuste = Convert.ToInt32(resultado[0]["idRecebimentoAjuste"]);
                                    _db.Database.ExecuteSqlCommand("UPDATE A" +
                                                                   " SET A.dsMotivo = '" + dsMotivo + "'" +
                                                                   ", A.cdBandeira = " + cdBandeiraParcela +
                                                                   ", A.nrCNPJ = '" + cnpj + "'" +
                                                                   ", A.dtAjuste = '" + DataBaseQueries.GetDate(dtAntecipacaoBancaria) + "'" +
                                                                   ", A.vlAjuste = " + ajuste.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                                                                   ", A.flAntecipacao = 1" +
                                                                   " FROM card.tbRecebimentoAjuste A" +
                                                                   " WHERE A.idRecebimentoAjuste = " + idRecebimentoAjuste);
                                }
                                _db.SaveChanges();
                            }

                        }

                        if (valorUtilizado < vlAntecipacao)
                        {
                            // Cria ajuste de crédito
                            decimal vlAjusteUtilizadoAntecipacaoAnterior = new decimal(0.0);
                            DateTime dtAntecipacaoBancariaAntecipacaoAnterior = dtAntecipacaoBancaria;
                            string dsMotivo = "SALDO ANTECIPAÇÃO BANCÁRIA VENCIMENTO " + dtVencimento.ToShortDateString();
                            string cnpj = cnpjsConta[0];
                            int cdBandeiraAjuste = 20;

                            // Procura a última antecipação para procurar a parcela que foi utilizada "em parte"
                            // Para isso, um ajuste à débito teve que ser criado
                            script = " SELECT TOP(1) D.idAntecipacaoBancariaDetalhe, D.vlAntecipacaoLiquida" +
                                     " FROM card.tbAntecipacaoBancariaDetalhe D (NOLOCK)" +
                                     " JOIN card.tbAntecipacaoBancaria A ON A.idAntecipacaoBancaria = D.idAntecipacaoBancaria" +
                                     //" LEFT JOIN pos.RecebimentoParcela P ON P.idAntecipacaoBancariaDetalhe = D.idAntecipacaoBancariaDetalhe" +
                                     " LEFT JOIN card.tbRecebimentoAjuste T ON T.idAntecipacaoBancariaDetalhe = D.idAntecipacaoBancariaDetalhe" +
                                     " WHERE A.dtAntecipacaoBancaria < '" + DataBaseQueries.GetDate(dtAntecipacaoBancaria) + "'" +
                                     " AND T.idAntecipacaoBancariaDetalhe IS NOT NULL" +
                                     " AND A.cdContaCorrente = " + cdContaCorrente +
                                     " AND A.cdAdquirente = 7" +
                                     " AND D.dtVencimento BETWEEN '" + DataBaseQueries.GetDate(dtVencimento) + "' AND '" + DataBaseQueries.GetDate(dtVencimento) + " 23:59:00'" +
                                     " ORDER BY A.dtAntecipacaoBancaria DESC";
                            resultado = DataBaseQueries.SqlQuery(script, connection);
                            if (resultado != null && resultado.Count > 0)
                            {
                                IDataRecord tbAntecipacaoBancariaDetalhe = resultado[0];
                                int idAD = Convert.ToInt32(tbAntecipacaoBancariaDetalhe["idAntecipacaoBancariaDetalhe"]);
                                decimal vlLiquidaAD = Convert.ToDecimal(tbAntecipacaoBancariaDetalhe["vlAntecipacaoLiquida"]);
                                
                                // Busca o ajuste à débito
                                script = "SELECT TOP(1) A.vlAjuste, A.dsMotivo, A.nrCNPJ, A.cdBandeira, T.dtAntecipacaoBancaria" +
                                         " FROM card.tbRecebimentoAjuste A (NOLOCK)" +
                                         " JOIN card.tbAntecipacaoBancariaDetalhe D ON A.idAntecipacaoBancariaDetalhe = D.idAntecipacaoBancariaDetalhe" +
                                         " JOIN card.tbAntecipacaoBancaria T ON T.idAntecipacaoBancaria = D.idAntecipacaoBancaria" +
                                         " WHERE A.idAntecipacaoBancariaDetalhe = " + idAD;
                                resultado = DataBaseQueries.SqlQuery(script, connection);
                                if (resultado == null || resultado.Count == 0)
                                    throw new Exception("Falha de comunicação com o servidor (AJ)");

                                IDataRecord tbRecebimentoAjuste = resultado[0];
                                vlAjusteUtilizadoAntecipacaoAnterior = Convert.ToDecimal(tbRecebimentoAjuste["vlAjuste"]);
                                dtAntecipacaoBancariaAntecipacaoAnterior = (DateTime)tbRecebimentoAjuste["dtAntecipacaoBancaria"];
                                dsMotivo = Convert.ToString(tbRecebimentoAjuste["dsMotivo"]);
                                cnpj = Convert.ToString(tbRecebimentoAjuste["nrCNPJ"]);
                                cdBandeiraAjuste = Convert.ToInt32(tbRecebimentoAjuste["cdBandeira"]);
                            }
                            // else não há antecipações anteriores....

                            decimal ajuste = decimal.Round(vlAntecipacaoLiquida - valorLiquidoUtilizado, 2);

                            if (vlAjusteUtilizadoAntecipacaoAnterior != new decimal(0.0) && Math.Abs(vlAjusteUtilizadoAntecipacaoAnterior) + new decimal(0.01) < ajuste)
                            {
                                throw new Exception("Ajuste à crédito de " + ajuste.ToString("C") + " não pode ser criado, pois a parcela que não foi utilizada por completo na antecipação bancária do dia " +
                                                    dtAntecipacaoBancariaAntecipacaoAnterior.ToShortDateString() + ", vencimento em " + dtVencimento.ToShortDateString() + ", tinha valor disponível de " +
                                                    (Math.Abs(vlAjusteUtilizadoAntecipacaoAnterior) + new decimal(0.01)).ToString("C"));
                            }

                            //teste.parcelas.Add(new
                            //{
                            //    idRecebimento = 0,
                            //    numParcela = -1,
                            //    valorDisponivel = new decimal(0.0),
                            //    vlDescontadoAntecipacao = new decimal(0.0),
                            //    valorNecessario = vlAntecipacao - valorUtilizado,
                            //    ajuste = ajuste,
                            //});

                            // Cria ajuste, caso não exista
                            script = "SELECT A.*" +
                                     " FROM card.tbRecebimentoAjuste A (NOLOCK)" +
                                     " WHERE A.idAntecipacaoBancariaDetalhe = " + idAntecipacaoBancariaDetalhe;
                            resultado = DataBaseQueries.SqlQuery(script, connection);
                            if (resultado == null || resultado.Count == 0)
                            {
                                // Cria
                                _db.Database.ExecuteSqlCommand("INSERT INTO card.tbRecebimentoAjuste" +
                                                               " (dsMotivo, cdBandeira, nrCNPJ, dtAjuste, vlAjuste, flAntecipacao, idAntecipacaoBancariaDetalhe)" +
                                                               " VALUES ('" + dsMotivo + "'" +
                                                               ", " + cdBandeiraAjuste +
                                                               ", '" + cnpj + "', '" + DataBaseQueries.GetDate(dtAntecipacaoBancaria) + "'" +
                                                               ", " + ajuste.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                                                               ", 1" +
                                                               ", " + idAntecipacaoBancariaDetalhe + ")");
                            }
                            else
                            {
                                // Atualiza valor
                                int idRecebimentoAjuste = Convert.ToInt32(resultado[0]["idRecebimentoAjuste"]);
                                _db.Database.ExecuteSqlCommand("UPDATE A" +
                                                               " SET A.dsMotivo = '" + dsMotivo + "'" +
                                                               ", A.cdBandeira = " + cdBandeiraAjuste +
                                                               ", A.nrCNPJ = '" + cnpj + "'" +
                                                               ", A.dtAjuste = '" + DataBaseQueries.GetDate(dtAntecipacaoBancaria) + "'" +
                                                               ", A.vlAjuste = " + ajuste.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                                                               ", A.flAntecipacao = 1" +
                                                               " FROM card.tbRecebimentoAjuste A" +
                                                               " WHERE A.idRecebimentoAjuste = " + idRecebimentoAjuste);
                            }
                            _db.SaveChanges();

                        }


                        //decimal totalDisponivel = teste.parcelas.Select(t => t.valorDisponivel).Cast<decimal>().Sum();
                        //decimal totalDescontado = teste.parcelas.Select(t => t.vlDescontadoAntecipacao).Cast<decimal>().Sum();
                        //decimal totalAjustes = teste.parcelas.Select(t => t.ajuste).Cast<decimal>().Sum();
                        //decimal totalLiquido = teste.parcelas.Select(t => t.valorDisponivel - t.vlDescontadoAntecipacao + t.ajuste).Cast<decimal>().Sum();

                        //decimal result = totalDisponivel - totalDescontado + totalAjustes;

                        //listaTeste.Add(teste);
                    }

                    try
                    {
                        connection.Close();
                    }
                    catch { }
                }

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