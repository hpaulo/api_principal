﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Globalization;

namespace api.Negocios.Card
{
    public class GatewayTbRecebimentoTEF
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRecebimentoTEF()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDRECEBIMENTOTEF = 100,
            CDGRUPO = 101,
            NRCNPJ = 102,
            CDEMPRESATEF = 103,
            NRPDVTEF = 104,
            NRNSUHOST = 105,
            NRNSUTEF = 106,
            CDAUTORIZACAO = 107,
            CDSITUACAOREDETEF = 108,
            DTVENDA = 109,
            HRVENDA = 110,
            VLVENDA = 111,
            QTPARCELAS = 112,
            NRCARTAO = 113,
            CDBANDEIRA = 114,
            NMOPERADORA = 115,
            DTHRVENDA = 116,
            CDESTADOTRANSACAOTEF = 117,
            CDTRASACAOTEF = 119,
            CDMODOENTRADATEF = 120,
            CDREDETEF = 121,
            CDPRODUTOTEF = 122,
            CDBANDEIRATEF = 123,
            CDESTABELECIMENTOHOST = 124,

        };

        /// <summary>
        /// Get TbRecebimentoTEF/TbRecebimentoTEF
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbRecebimentoTEF> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbRecebimentoTEFs.AsQueryable<tbRecebimentoTEF>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDRECEBIMENTOTEF:
                        Int32 idRecebimentoTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRecebimentoTEF.Equals(idRecebimentoTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDGRUPO:
                        Int32 cdGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdGrupo.Equals(cdGrupo)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDEMPRESATEF:
                        string cdEmpresaTEF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdEmpresaTEF.Equals(cdEmpresaTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRPDVTEF:
                        string nrPDVTEF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrPDVTEF.Equals(nrPDVTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRNSUHOST:
                        string nrNSUHost = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrNSUHost.Equals(nrNSUHost)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRNSUTEF:
                        string nrNSUTEF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrNSUTEF.Equals(nrNSUTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDAUTORIZACAO:
                        string cdAutorizacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizacao.Equals(cdAutorizacao)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDSITUACAOREDETEF:
                        Int32 cdSituacaoRedeTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdSituacaoRedeTEF.Equals(cdSituacaoRedeTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.DTVENDA:
                        //DateTime dtVenda = Convert.ToDateTime(item.Value);
                        //DateTime dtVenda = Convert.ToDateTime(item.Value);
                        //entity = entity.Where(e => e.dtVenda.Equals(dtVenda)).AsQueryable();
                        //break;

                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            //entity = entity.Where(e => e.dtVenda >= dtaIni && e.dtVenda <= dtaFim);
                            entity = entity.Where(e => (e.dtVenda.Year > dtaIni.Year || (e.dtVenda.Year == dtaIni.Year && e.dtVenda.Month > dtaIni.Month) ||
                                                                                          (e.dtVenda.Year == dtaIni.Year && e.dtVenda.Month == dtaIni.Month && e.dtVenda.Day >= dtaIni.Day))
                                                    && (e.dtVenda.Year < dtaFim.Year || (e.dtVenda.Year == dtaFim.Year && e.dtVenda.Month < dtaFim.Month) ||
                                                                                          (e.dtVenda.Year == dtaFim.Year && e.dtVenda.Month == dtaFim.Month && e.dtVenda.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda >= dta);
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca;
                            if (item.Value.Length == 10)
                            {
                                string ano = item.Value.Substring(0, 4);
                                string mes = item.Value.Substring(5, 2);
                                string dia = item.Value.Substring(7, 2);
                                busca = ano + mes + dia;
                            }
                            else if (item.Value.Length == 8)
                            {
                                string dia = item.Value.Substring(6, 1);
                                string anoMes = item.Value.Substring(0, 6);
                                busca = anoMes + "0" + dia;
                            }
                            else
                            {
                                busca = item.Value.Replace("<", "");
                            }
                            //busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda <= dta);
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda.Year == dtaIni.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda.Year == dtaIni.Year && e.dtVenda.Month == dtaIni.Month);
                        }
                        else if (item.Value.Length == 7)
                        {
                            string dia = item.Value.Substring(6, 1);
                            string anoMes = item.Value.Substring(0, 6);
                            string busca = anoMes + "0" + dia;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda.Year == dtaIni.Year && e.dtVenda.Month == dtaIni.Month && e.dtVenda.Day == dtaIni.Day);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda.Year == dtaIni.Year && e.dtVenda.Month == dtaIni.Month && e.dtVenda.Day == dtaIni.Day);
                        }
                        break;
                        //entity = entity.Where(e => e.dtVenda.Equals(dtVenda)).AsQueryable<tbRecebimentoTEF>();
                        //break;
                    case CAMPOS.HRVENDA:
                        TimeSpan hrVenda = TimeSpan.Parse(item.Value);
                        entity = entity.Where(e => e.hrVenda.Equals(hrVenda)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.VLVENDA:
                        decimal vlVenda = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlVenda.Equals(vlVenda)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.QTPARCELAS:
                        Int32 qtParcelas = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.qtParcelas.Equals(qtParcelas)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRCARTAO:
                        string nrCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCartao.Equals(nrCartao)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira.Equals(cdBandeira)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NMOPERADORA:
                        string nmOperadora = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmOperadora.Equals(nmOperadora)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.DTHRVENDA:
                        DateTime dthrVenda = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dthrVenda.Equals(dthrVenda)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDESTADOTRANSACAOTEF:
                        Int32 cdEstadoTransacaoTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdEstadoTransacaoTEF.Equals(cdEstadoTransacaoTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDTRASACAOTEF:
                        Int32 cdTrasacaoTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdTrasacaoTEF.Equals(cdTrasacaoTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDMODOENTRADATEF:
                        Int32 cdModoEntradaTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdModoEntradaTEF.Equals(cdModoEntradaTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDREDETEF:
                        Int32 cdRedeTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdRedeTEF.Equals(cdRedeTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDPRODUTOTEF:
                        Int32 cdProdutoTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdProdutoTEF.Equals(cdProdutoTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDBANDEIRATEF:
                        Int32 cdBandeiraTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeiraTEF.Equals(cdBandeiraTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDESTABELECIMENTOHOST:
                        string cdEstabelecimentoHost = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdEstabelecimentoHost.Equals(cdEstabelecimentoHost)).AsQueryable<tbRecebimentoTEF>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDRECEBIMENTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRecebimentoTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.idRecebimentoTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdGrupo).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdGrupo).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDEMPRESATEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEmpresaTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdEmpresaTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRPDVTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrPDVTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrPDVTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRNSUHOST:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrNSUHost).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrNSUHost).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRNSUTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrNSUTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrNSUTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDAUTORIZACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizacao).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizacao).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDSITUACAOREDETEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSituacaoRedeTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdSituacaoRedeTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVenda).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.dtVenda).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.HRVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.hrVenda).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.hrVenda).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.VLVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlVenda).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.vlVenda).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.QTPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtParcelas).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.qtParcelas).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCartao).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrCartao).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NMOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmOperadora).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nmOperadora).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.DTHRVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dthrVenda).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.dthrVenda).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDESTADOTRANSACAOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEstadoTransacaoTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdEstadoTransacaoTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDTRASACAOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTrasacaoTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdTrasacaoTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDMODOENTRADATEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdModoEntradaTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdModoEntradaTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDREDETEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdRedeTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdRedeTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDPRODUTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdProdutoTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdProdutoTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDBANDEIRATEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeiraTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdBandeiraTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDESTABELECIMENTOHOST:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEstabelecimentoHost).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdEstabelecimentoHost).AsQueryable<tbRecebimentoTEF>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbRecebimentoTEF/TbRecebimentoTEF
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbRecebimentoTEF = new List<dynamic>();
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
                    CollectionTbRecebimentoTEF = query.Select(e => new
                    {

                        idRecebimentoTEF = e.idRecebimentoTEF,
                        cdGrupo = e.cdGrupo,
                        nrCNPJ = e.nrCNPJ,
                        cdEmpresaTEF = e.cdEmpresaTEF,
                        nrPDVTEF = e.nrPDVTEF,
                        nrNSUHost = e.nrNSUHost,
                        nrNSUTEF = e.nrNSUTEF,
                        cdAutorizacao = e.cdAutorizacao,
                        cdSituacaoRedeTEF = e.cdSituacaoRedeTEF,
                        dtVenda = e.dtVenda,
                        hrVenda = e.hrVenda,
                        vlVenda = e.vlVenda,
                        qtParcelas = e.qtParcelas,
                        nrCartao = e.nrCartao,
                        cdBandeira = e.cdBandeira,
                        nmOperadora = e.nmOperadora,
                        dthrVenda = e.dthrVenda,
                        cdEstadoTransacaoTEF = e.cdEstadoTransacaoTEF,
                        cdTrasacaoTEF = e.cdTrasacaoTEF,
                        cdModoEntradaTEF = e.cdModoEntradaTEF,
                        cdRedeTEF = e.cdRedeTEF,
                        cdProdutoTEF = e.cdProdutoTEF,
                        cdBandeiraTEF = e.cdBandeiraTEF,
                        cdEstabelecimentoHost = e.cdEstabelecimentoHost,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbRecebimentoTEF = query.Select(e => new
                    {

                        idRecebimentoTEF = e.idRecebimentoTEF,
                        cdGrupo = e.cdGrupo,
                        nrCNPJ = e.nrCNPJ,
                        cdEmpresaTEF = e.cdEmpresaTEF,
                        nrPDVTEF = e.nrPDVTEF,
                        nrNSUHost = e.nrNSUHost,
                        nrNSUTEF = e.nrNSUTEF,
                        cdAutorizacao = e.cdAutorizacao,
                        cdSituacaoRedeTEF = e.cdSituacaoRedeTEF,
                        dtVenda = e.dtVenda,
                        hrVenda = e.hrVenda,
                        vlVenda = e.vlVenda,
                        qtParcelas = e.qtParcelas,
                        nrCartao = e.nrCartao,
                        cdBandeira = e.cdBandeira,
                        nmOperadora = e.nmOperadora,
                        dthrVenda = e.dthrVenda,
                        cdEstadoTransacaoTEF = e.cdEstadoTransacaoTEF,
                        cdTrasacaoTEF = e.cdTrasacaoTEF,
                        cdModoEntradaTEF = e.cdModoEntradaTEF,
                        cdRedeTEF = e.cdRedeTEF,
                        cdProdutoTEF = e.cdProdutoTEF,
                        cdBandeiraTEF = e.cdBandeiraTEF,
                        cdEstabelecimentoHost = e.cdEstabelecimentoHost,
                    }).ToList<dynamic>();
                }
                else if (colecao == 3) // Resumo Movimento
                {
                    CollectionTbRecebimentoTEF = query
                        .GroupBy(x => new { x.cdBandeira, x.cdTrasacaoTEF})
                        .Select(e => new
                    {
                        
                        quantidade = e.Count(),
                        vlVenda = (e.Sum(p => p.vlVenda)),
                        cdBandeira = e.Key.cdBandeira,
                        cdTrasacaoTEF = e.Key.cdTrasacaoTEF,
                    }).ToList<dynamic>();
                }
                else if (colecao == 4) // Movimento
                {
                    CollectionTbRecebimentoTEF = query
                         //.GroupBy(x => new { x.cdGrupo })
                        .Select(e => new
                        {

                            hrVenda = e.hrVenda,
                            filial = e.grupo_empresa.ds_nome,
                            nmOperadora = e.nmOperadora,
                            nrCartao = e.nrCartao,
                            vlVenda = e.vlVenda,
                            qtParcelas = e.qtParcelas,
                            tipoProtuto = e.tbProdutoTef.tbTipoProdutoTef.dsTipoProdutoTef,
                            nrNSUTEF = e.nrNSUTEF,
                            Status = e.tbEstadoTransacaoTef.dsEstadoTransacaoTef,
                        }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionTbRecebimentoTEF = query
                        .GroupBy(x => new { x.cdGrupo })
                        .Select(e => new
                        {
                            cdGrupo = e.Key.cdGrupo,
                            nmGrupo = _db.grupo_empresa.Where(g => g.id_grupo == e.Key.cdGrupo).Select(g => g.ds_nome).FirstOrDefault(),
                            dtVenda = (e.Max(p => p.dthrVenda)),
                        }).ToList<dynamic>();

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = CollectionTbRecebimentoTEF.Count();


                    // PAGINAÇÃO
                    skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;

                }
                

                retorno.Registros = CollectionTbRecebimentoTEF;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar recebimento tef" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbRecebimentoTEF
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbRecebimentoTEF param)
        {
            try
            {
                _db.tbRecebimentoTEFs.Add(param);
                _db.SaveChanges();
                return param.idRecebimentoTEF;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar recebimento tef" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbRecebimentoTEF
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRecebimentoTEF)
        {
            try
            {
                _db.tbRecebimentoTEFs.Remove(_db.tbRecebimentoTEFs.Where(e => e.idRecebimentoTEF.Equals(idRecebimentoTEF)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar recebimento tef" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Altera tbRecebimentoTEF
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbRecebimentoTEF param)
        {
            try
            {
                tbRecebimentoTEF value = _db.tbRecebimentoTEFs
                        .Where(e => e.idRecebimentoTEF.Equals(param.idRecebimentoTEF))
                        .First<tbRecebimentoTEF>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idRecebimentoTEF != null && param.idRecebimentoTEF != value.idRecebimentoTEF)
                    value.idRecebimentoTEF = param.idRecebimentoTEF;
                if (param.cdGrupo != null && param.cdGrupo != value.cdGrupo)
                    value.cdGrupo = param.cdGrupo;
                if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                    value.nrCNPJ = param.nrCNPJ;
                if (param.cdEmpresaTEF != null && param.cdEmpresaTEF != value.cdEmpresaTEF)
                    value.cdEmpresaTEF = param.cdEmpresaTEF;
                if (param.nrPDVTEF != null && param.nrPDVTEF != value.nrPDVTEF)
                    value.nrPDVTEF = param.nrPDVTEF;
                if (param.nrNSUHost != null && param.nrNSUHost != value.nrNSUHost)
                    value.nrNSUHost = param.nrNSUHost;
                if (param.nrNSUTEF != null && param.nrNSUTEF != value.nrNSUTEF)
                    value.nrNSUTEF = param.nrNSUTEF;
                if (param.cdAutorizacao != null && param.cdAutorizacao != value.cdAutorizacao)
                    value.cdAutorizacao = param.cdAutorizacao;
                if (param.cdSituacaoRedeTEF != null && param.cdSituacaoRedeTEF != value.cdSituacaoRedeTEF)
                    value.cdSituacaoRedeTEF = param.cdSituacaoRedeTEF;
                if (param.dtVenda != null && param.dtVenda != value.dtVenda)
                    value.dtVenda = param.dtVenda;
                if (param.hrVenda != null && param.hrVenda != value.hrVenda)
                    value.hrVenda = param.hrVenda;
                if (param.vlVenda != null && param.vlVenda != value.vlVenda)
                    value.vlVenda = param.vlVenda;
                if (param.qtParcelas != null && param.qtParcelas != value.qtParcelas)
                    value.qtParcelas = param.qtParcelas;
                if (param.nrCartao != null && param.nrCartao != value.nrCartao)
                    value.nrCartao = param.nrCartao;
                if (param.cdBandeira != null && param.cdBandeira != value.cdBandeira)
                    value.cdBandeira = param.cdBandeira;
                if (param.nmOperadora != null && param.nmOperadora != value.nmOperadora)
                    value.nmOperadora = param.nmOperadora;
                if (param.dthrVenda != null && param.dthrVenda != value.dthrVenda)
                    value.dthrVenda = param.dthrVenda;
                if (param.cdEstadoTransacaoTEF != null && param.cdEstadoTransacaoTEF != value.cdEstadoTransacaoTEF)
                    value.cdEstadoTransacaoTEF = param.cdEstadoTransacaoTEF;
                if (param.cdTrasacaoTEF != null && param.cdTrasacaoTEF != value.cdTrasacaoTEF)
                    value.cdTrasacaoTEF = param.cdTrasacaoTEF;
                if (param.cdModoEntradaTEF != null && param.cdModoEntradaTEF != value.cdModoEntradaTEF)
                    value.cdModoEntradaTEF = param.cdModoEntradaTEF;
                if (param.cdRedeTEF != null && param.cdRedeTEF != value.cdRedeTEF)
                    value.cdRedeTEF = param.cdRedeTEF;
                if (param.cdProdutoTEF != null && param.cdProdutoTEF != value.cdProdutoTEF)
                    value.cdProdutoTEF = param.cdProdutoTEF;
                if (param.cdBandeiraTEF != null && param.cdBandeiraTEF != value.cdBandeiraTEF)
                    value.cdBandeiraTEF = param.cdBandeiraTEF;
                if (param.cdEstabelecimentoHost != null && param.cdEstabelecimentoHost != value.cdEstabelecimentoHost)
                    value.cdEstabelecimentoHost = param.cdEstabelecimentoHost;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar recebimento tef" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
