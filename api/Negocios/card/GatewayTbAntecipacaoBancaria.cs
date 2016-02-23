﻿using System;
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
using System.Globalization;
using api.Negocios.Util;
using api.Negocios.Cliente;

namespace api.Negocios.Card
{
    public class GatewayTbAntecipacaoBancaria
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbAntecipacaoBancaria()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "AB";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDANTECIPACAOBANCARIA = 100,
            CDCONTACORRENTE = 101,
            DTANTECIPACAOBANCARIA = 102,
            CDADQUIRENTE = 103,
            VLOPERACAO = 104,
            VLLIQUIDO = 105,

            // RELACIONAMENTOS
            DTVENCIMENTO = 202,
            CDBANDEIRA = 205,

            ID_GRUPO = 301,
        };

        /// <summary>
        /// Get TbAntecipacaoBancaria/TbAntecipacaoBancaria
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbAntecipacaoBancaria> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbAntecipacaoBancarias.AsQueryable<tbAntecipacaoBancaria>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDANTECIPACAOBANCARIA:
                        Int32 idAntecipacaoBancaria = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idAntecipacaoBancaria == idAntecipacaoBancaria).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.DTANTECIPACAOBANCARIA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtAntecipacaoBancaria.Year > dtaIni.Year || (e.dtAntecipacaoBancaria.Year == dtaIni.Year && e.dtAntecipacaoBancaria.Month > dtaIni.Month) ||
                                                                                          (e.dtAntecipacaoBancaria.Year == dtaIni.Year && e.dtAntecipacaoBancaria.Month == dtaIni.Month && e.dtAntecipacaoBancaria.Day >= dtaIni.Day))
                                                    && (e.dtAntecipacaoBancaria.Year < dtaFim.Year || (e.dtAntecipacaoBancaria.Year == dtaFim.Year && e.dtAntecipacaoBancaria.Month < dtaFim.Month) ||
                                                                                          (e.dtAntecipacaoBancaria.Year == dtaFim.Year && e.dtAntecipacaoBancaria.Month == dtaFim.Month && e.dtAntecipacaoBancaria.Day <= dtaFim.Day)));
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtAntecipacaoBancaria.Year == dtaIni.Year && e.dtAntecipacaoBancaria.Month == dtaIni.Month && e.dtAntecipacaoBancaria.Day == dtaIni.Day);
                        }
                        break;
                   
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente == cdAdquirente).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdContaCorrente == cdContaCorrente).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.VLOPERACAO:
                        decimal vlOperacao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlOperacao == vlOperacao).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.VLLIQUIDO:
                        decimal vlLiquido = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlLiquido == vlLiquido).AsQueryable<tbAntecipacaoBancaria>();
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.DTVENCIMENTO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(t => t.tbAntecipacaoBancariaDetalhes.Any(e => (e.dtVencimento.Year > dtaIni.Year || (e.dtVencimento.Year == dtaIni.Year && e.dtVencimento.Month > dtaIni.Month) || (e.dtVencimento.Year == dtaIni.Year && e.dtVencimento.Month == dtaIni.Month && e.dtVencimento.Day >= dtaIni.Day))
                                                                                                && (e.dtVencimento.Year < dtaFim.Year || (e.dtVencimento.Year == dtaFim.Year && e.dtVencimento.Month < dtaFim.Month) || (e.dtVencimento.Year == dtaFim.Year && e.dtVencimento.Month == dtaFim.Month && e.dtVencimento.Day <= dtaFim.Day))));
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(t => t.tbAntecipacaoBancariaDetalhes.Any(e => e.dtVencimento.Year == dtaIni.Year && e.dtVencimento.Month == dtaIni.Month && e.dtVencimento.Day == dtaIni.Day));
                        }
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(t => t.tbAntecipacaoBancariaDetalhes.Any(e => e.cdBandeira == cdBandeira)).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(t => t.tbContaCorrente.empresa.id_grupo == id_grupo).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDANTECIPACAOBANCARIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idAntecipacaoBancaria).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.idAntecipacaoBancaria).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.DTANTECIPACAOBANCARIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtAntecipacaoBancaria).ThenBy(e => e.tbAdquirente.nmAdquirente).ThenBy(e => e.vlOperacao).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.dtAntecipacaoBancaria).ThenByDescending(e => e.tbAdquirente.nmAdquirente).ThenByDescending(e => e.vlOperacao).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdContaCorrente).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.cdContaCorrente).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.VLOPERACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlOperacao).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.vlOperacao).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.VLLIQUIDO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlLiquido).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.vlLiquido).AsQueryable<tbAntecipacaoBancaria>();
                    break;
            }
            #endregion

            return entity;


        }


        private static SimpleDataBaseQuery getQuery(int campo, int orderby, Dictionary<string, string> queryString)
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
                    case CAMPOS.IDANTECIPACAOBANCARIA:
                        Int32 idAntecipacaoBancaria = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idAntecipacaoBancaria = " + idAntecipacaoBancaria);
                        break;
                    case CAMPOS.DTANTECIPACAOBANCARIA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(SIGLA_QUERY + ".dtAntecipacaoBancaria BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtAntecipacaoBancaria BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;

                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente);
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".cdContaCorrente = " + cdContaCorrente);
                        break;
                    case CAMPOS.VLOPERACAO:
                        decimal vlOperacao = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlOperacao = " + vlOperacao.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.VLLIQUIDO:
                        decimal vlLiquido = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlLiquido = " + vlLiquido.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.DTVENCIMENTO:
                        // Adiciona o join
                        if (!join.ContainsKey("LEFT JOIN card.tbAntecipacaoBancariaDetalhe " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY))
                            join.Add("LEFT JOIN card.tbAntecipacaoBancariaDetalhe " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancaria = " + SIGLA_QUERY + ".idAntecipacaoBancaria");
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".dtVencimento BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".dtVencimento BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;
                    case CAMPOS.CDBANDEIRA:
                        // Adiciona o join
                        if (!join.ContainsKey("LEFT JOIN card.tbAntecipacaoBancariaDetalhe " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY))
                            join.Add("LEFT JOIN card.tbAntecipacaoBancariaDetalhe "+ GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancaria = " + SIGLA_QUERY + ".idAntecipacaoBancaria");
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        where.Add(GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".cdBandeira = " + cdBandeira);
                        break;
                    case CAMPOS.ID_GRUPO:
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbContaCorrente "+ GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + SIGLA_QUERY + ".cdContaCorrente");
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
                case CAMPOS.IDANTECIPACAOBANCARIA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idAntecipacaoBancaria ASC");
                    else order.Add(SIGLA_QUERY + ".idAntecipacaoBancaria DESC");
                    break;
                case CAMPOS.DTANTECIPACAOBANCARIA:
                    if (!join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                       join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + SIGLA_QUERY + ".cdAdquirente");
                    if (orderby == 0)
                    { 
                        order.Add(SIGLA_QUERY + ".dtAntecipacaoBancaria ASC");
                        order.Add(GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente ASC");
                        order.Add(SIGLA_QUERY + ".vlOperacao ASC");
                    }
                    else
                    { 
                        order.Add(SIGLA_QUERY + ".dtAntecipacaoBancaria DESC");
                        order.Add(GatewayTbAdquirente.SIGLA_QUERY + ".idAntecipacaoBancaria DESC");
                        order.Add(SIGLA_QUERY + ".vlOperacao DESC");
                    }
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdAdquirente ASC");
                    else order.Add(SIGLA_QUERY + ".cdAdquirente DESC");
                    break;
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdContaCorrente ASC");
                    else order.Add(SIGLA_QUERY + ".cdContaCorrente DESC");
                    break;
                case CAMPOS.VLOPERACAO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlOperacao ASC");
                    else order.Add(SIGLA_QUERY + ".vlOperacao DESC");
                    break;
                case CAMPOS.VLLIQUIDO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlLiquido ASC");
                    else order.Add(SIGLA_QUERY + ".vlLiquido DESC");
                    break;
            }
            #endregion

            return new SimpleDataBaseQuery(null, "card.tbAntecipacaoBancaria " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());

        }


        /// <summary>
        /// Retorna TbAntecipacaoBancaria/TbAntecipacaoBancaria
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
                List<dynamic> CollectionTbAntecipacaoBancaria = new List<dynamic>();
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
                    CollectionTbAntecipacaoBancaria = query.Select(e => new
                    {
                        idAntecipacaoBancaria = e.idAntecipacaoBancaria,
                        dtAntecipacaoBancaria = e.dtAntecipacaoBancaria,
                        vlOperacao = e.vlOperacao,
                        vlLiquido = e.vlLiquido,
                        txJuros = e.txJuros,
                        txIOF = e.txIOF,
                        txIOFAdicional = e.txIOFAdicional,
                        cdAdquirente = e.cdAdquirente,
                        cdContaCorrente = e.cdContaCorrente,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbAntecipacaoBancaria = query.Select(e => new
                    {
                        idAntecipacaoBancaria = e.idAntecipacaoBancaria,
                        dtAntecipacaoBancaria = e.dtAntecipacaoBancaria,
                        vlOperacao = e.vlOperacao ?? new decimal(0.0),
                        vlLiquido = e.vlLiquido ?? new decimal(0.0),
                        txJuros = e.txJuros,
                        txIOF = e.txIOF,
                        txIOFAdicional = e.txIOFAdicional,
                        cdAdquirente = e.cdAdquirente,
                        cdContaCorrente = e.cdContaCorrente,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionTbAntecipacaoBancaria = query.Select(e => new
                    {
                        idAntecipacaoBancaria = e.idAntecipacaoBancaria,
                        cdContaCorrente = e.cdContaCorrente,
                        dtAntecipacaoBancaria = e.dtAntecipacaoBancaria,
                        tbAdquirente = new
                        {
                            cdAdquirente = e.tbAdquirente.cdAdquirente,
                            nmAdquirente = e.tbAdquirente.nmAdquirente
                        },
                        vlOperacao = e.vlOperacao ?? new decimal(0.0),
                        vlLiquido = e.vlLiquido ?? new decimal(0.0),
                        txJuros = e.txJuros,
                        txIOF = e.txIOF,
                        txIOFAdicional = e.txIOFAdicional,
                        antecipacoes = e.tbAntecipacaoBancariaDetalhes.Select(t => new {
                            idAntecipacaoBancariaDetalhe = t.idAntecipacaoBancariaDetalhe,
                            dtVencimento = t.dtVencimento,
                            vlAntecipacao = t.vlAntecipacao,
                            vlAntecipacaoLiquida = t.vlAntecipacaoLiquida,
                            vlIOF = t.vlIOF,
                            vlIOFAdicional = t.vlIOFAdicional,
                            vlJuros = t.vlJuros,
                            tbBandeira = t.cdBandeira == null ? null : new { cdBandeira = t.cdBandeira,
													                         dsBandeira = t.tbBandeira.dsBandeira 
													                       },
                        }).OrderBy(t => t.dtVencimento).ThenBy(t => t.vlAntecipacao).ToList<dynamic>()
                        
                    }).ToList<dynamic>();
                }

                transaction.Commit();

                retorno.Registros = CollectionTbAntecipacaoBancaria;

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
        /// Adiciona nova TbAntecipacaoBancaria
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, AntecipacaoBancaria param, painel_taxservices_dbContext _dbContext = null)
        {
            if (param == null || param.antecipacoes == null || param.antecipacoes.Count == 0)
                throw new Exception("Parâmetro inválido!");

            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                decimal taxa = (param.vlOperacao - param.vlLiquido) / param.vlOperacao;

                tbAntecipacaoBancaria tbAntecipacaoBancaria = new tbAntecipacaoBancaria();
                tbAntecipacaoBancaria.cdAdquirente = param.cdAdquirente;
                tbAntecipacaoBancaria.cdContaCorrente = param.cdContaCorrente;
                tbAntecipacaoBancaria.dtAntecipacaoBancaria = param.dtAntecipacaoBancaria;
                tbAntecipacaoBancaria.txIOF = param.txIOF;
                tbAntecipacaoBancaria.txIOFAdicional = param.txIOFAdicional;
                tbAntecipacaoBancaria.txJuros = param.txJuros;
                //tbAntecipacaoBancaria.vlOperacao = param.antecipacoes.Select(t => t.vlAntecipacao).Sum();
                //tbAntecipacaoBancaria.vlLiquido = param.antecipacoes.Select(t => t.vlAntecipacaoLiquida).Sum();
                _db.tbAntecipacaoBancarias.Add(tbAntecipacaoBancaria);
                _db.SaveChanges();

                foreach (AntecipacaoBancariaVencimentos antecipacao in param.antecipacoes)
                {
                    tbAntecipacaoBancariaDetalhe tbAntecipacaoBancariaDetalhe = new tbAntecipacaoBancariaDetalhe();
                    tbAntecipacaoBancariaDetalhe.idAntecipacaoBancaria = tbAntecipacaoBancaria.idAntecipacaoBancaria;
                    tbAntecipacaoBancariaDetalhe.cdBandeira = antecipacao.cdBandeira;
                    tbAntecipacaoBancariaDetalhe.dtVencimento = antecipacao.dtVencimento;
                    tbAntecipacaoBancariaDetalhe.vlAntecipacao = antecipacao.vlAntecipacao;
                    //tbAntecipacaoBancariaDetalhe.vlAntecipacaoLiquida = decimal.Round(antecipacao.vlAntecipacao * (new decimal(1.0) - taxa), 3);
                    tbAntecipacaoBancariaDetalhe.vlIOF = antecipacao.vlIOF;
                    tbAntecipacaoBancariaDetalhe.vlIOFAdicional = antecipacao.vlIOFAdicional;
                    tbAntecipacaoBancariaDetalhe.vlJuros = antecipacao.vlJuros;
                    tbAntecipacaoBancariaDetalhe.vlAntecipacaoLiquida = antecipacao.vlAntecipacao - antecipacao.vlJuros - antecipacao.vlIOF - antecipacao.vlIOFAdicional;
                        
                    _db.tbAntecipacaoBancariaDetalhes.Add(tbAntecipacaoBancariaDetalhe);
                    _db.SaveChanges();
                }
                transaction.Commit();
                return tbAntecipacaoBancaria.idAntecipacaoBancaria;
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
        /// Apaga uma TbAntecipacaoBancaria
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idAntecipacaoBancaria, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbAntecipacaoBancaria tbAntecipacaoBancaria = _db.tbAntecipacaoBancarias.Where(e => e.idAntecipacaoBancaria.Equals(idAntecipacaoBancaria)).FirstOrDefault();
                if(tbAntecipacaoBancaria == null)
                    throw new Exception("Antecipação bancária inexistente!");

                // Remove todos os vencimentos associados
                _db.tbAntecipacaoBancariaDetalhes.RemoveRange(_db.tbAntecipacaoBancariaDetalhes.Where(t => t.idAntecipacaoBancaria == idAntecipacaoBancaria));
                _db.SaveChanges();

                _db.tbAntecipacaoBancarias.Remove(tbAntecipacaoBancaria);
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
        /// Altera tbAntecipacaoBancaria
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, AntecipacaoBancaria param, painel_taxservices_dbContext _dbContext = null)
        {
            if (param == null || param.antecipacoes == null)
                throw new Exception("Parâmetro inválido!");

            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbAntecipacaoBancaria value = _db.tbAntecipacaoBancarias
                                .Where(e => e.idAntecipacaoBancaria == param.idAntecipacaoBancaria)
                                .FirstOrDefault();

                if (value == null)
                    throw new Exception("Antecipação bancária inexistente!");

                //decimal taxa = (param.vlOperacao - param.vlLiquido) / param.vlOperacao;

                if (param.dtAntecipacaoBancaria != value.dtAntecipacaoBancaria)
                    value.dtAntecipacaoBancaria = param.dtAntecipacaoBancaria;
                if (param.cdAdquirente != 0 && param.cdAdquirente != value.cdAdquirente)
                    value.cdAdquirente = param.cdAdquirente;
                value.txIOF = param.txIOF;
                value.txIOFAdicional = param.txIOFAdicional;
                value.txJuros = param.txJuros;
                _db.SaveChanges();

                // Salva antecipações
                foreach (AntecipacaoBancariaVencimentos antecipacao in param.antecipacoes)
                {
                    tbAntecipacaoBancariaDetalhe tbAntecipacaoBancariaDetalhe;

                    if (antecipacao.idAntecipacaoBancariaDetalhe == -1)
                    {
                        // Novo registro
                        tbAntecipacaoBancariaDetalhe = new tbAntecipacaoBancariaDetalhe();
                        tbAntecipacaoBancariaDetalhe.idAntecipacaoBancaria = param.idAntecipacaoBancaria;
                        tbAntecipacaoBancariaDetalhe.cdBandeira = antecipacao.cdBandeira;
                        tbAntecipacaoBancariaDetalhe.dtVencimento = antecipacao.dtVencimento;
                        tbAntecipacaoBancariaDetalhe.vlAntecipacao = antecipacao.vlAntecipacao;
                        //tbAntecipacaoBancariaDetalhe.vlAntecipacaoLiquida = decimal.Round(antecipacao.vlAntecipacao * (new decimal(1.0) - taxa), 3);
                        tbAntecipacaoBancariaDetalhe.vlIOF = antecipacao.vlIOF;
                        tbAntecipacaoBancariaDetalhe.vlIOFAdicional = antecipacao.vlIOFAdicional;
                        tbAntecipacaoBancariaDetalhe.vlJuros = antecipacao.vlJuros;
                        tbAntecipacaoBancariaDetalhe.vlAntecipacaoLiquida = antecipacao.vlAntecipacao - antecipacao.vlJuros - antecipacao.vlIOF - antecipacao.vlIOFAdicional;
                        // Adiciona
                        _db.tbAntecipacaoBancariaDetalhes.Add(tbAntecipacaoBancariaDetalhe);
                    }
                    else
                    {
                        tbAntecipacaoBancariaDetalhe = _db.tbAntecipacaoBancariaDetalhes.Where(t => t.idAntecipacaoBancariaDetalhe == antecipacao.idAntecipacaoBancariaDetalhe).FirstOrDefault();

                        if (tbAntecipacaoBancariaDetalhe == null)
                            throw new Exception("Vencimento " + antecipacao.idAntecipacaoBancariaDetalhe + " inválido da antecipação bancária");

                        if (tbAntecipacaoBancariaDetalhe.cdBandeira != antecipacao.cdBandeira)
                            tbAntecipacaoBancariaDetalhe.cdBandeira = antecipacao.cdBandeira;
                        if (!tbAntecipacaoBancariaDetalhe.dtVencimento.Equals(antecipacao.dtVencimento))
                            tbAntecipacaoBancariaDetalhe.dtVencimento = antecipacao.dtVencimento;
                        if (tbAntecipacaoBancariaDetalhe.vlAntecipacao != antecipacao.vlAntecipacao)
                            tbAntecipacaoBancariaDetalhe.vlAntecipacao = antecipacao.vlAntecipacao;
                        //if (tbAntecipacaoBancariaDetalhe.vlAntecipacaoLiquida != antecipacao.vlAntecipacaoLiquida)
                            //tbAntecipacaoBancariaDetalhe.vlAntecipacaoLiquida = antecipacao.vlAntecipacaoLiquida;
                        //tbAntecipacaoBancariaDetalhe.vlAntecipacaoLiquida = decimal.Round(antecipacao.vlAntecipacao * (new decimal(1.0) - taxa), 3);
                        tbAntecipacaoBancariaDetalhe.vlIOF = antecipacao.vlIOF;
                        tbAntecipacaoBancariaDetalhe.vlIOFAdicional = antecipacao.vlIOFAdicional;
                        tbAntecipacaoBancariaDetalhe.vlJuros = antecipacao.vlJuros;
                        tbAntecipacaoBancariaDetalhe.vlAntecipacaoLiquida = antecipacao.vlAntecipacao - antecipacao.vlJuros - antecipacao.vlIOF - antecipacao.vlIOFAdicional;
                    }
                    _db.SaveChanges();
                }

                // Deleta vencimentos
                if (param.deletar != null && param.deletar.Count > 0)
                {
                    foreach (int idAntecipacaoBancariaDetalhe in param.deletar)
                    {
                        tbAntecipacaoBancariaDetalhe tbAntecipacaoBancariaDetalhe = _db.tbAntecipacaoBancariaDetalhes.Where(t => t.idAntecipacaoBancariaDetalhe == idAntecipacaoBancariaDetalhe).FirstOrDefault();

                        if (tbAntecipacaoBancariaDetalhe == null)
                            throw new Exception("Vencimento " + idAntecipacaoBancariaDetalhe + " inválido da antecipação bancária");

                        _db.tbAntecipacaoBancariaDetalhes.Remove(tbAntecipacaoBancariaDetalhe);
                        _db.SaveChanges();
                    }
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
