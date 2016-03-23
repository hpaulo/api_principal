using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Globalization;
using System.Data.Entity.Validation;
using System.Data.Entity;
using api.Negocios.Util;
using api.Negocios.Cliente;
using System.Data;

namespace api.Negocios.Card
{
    public class GatewayTbRecebimentoAjuste
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRecebimentoAjuste()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "AJ";

        public static string[] AJUSTES_ANTECIPACAO_AMEX = { "DEBITO ANTECIPACAO A VISTA",
                                                            "DEBITO ANTECIPACAO PARCELADO" }; // STARTS WITH!
        public static string[] AJUSTES_ANTECIPACAO_BANESE = { "DÉBITO ANTECIPAÇÃO",
                                                              "PAGAMENTO ANTECIPAÇÃO" };
        public static string[] AJUSTES_ANTECIPACAO_CIELO = { "AJUSTE DEB ARV",
                                                             "AJUSTE DEB ARV (EC)",
                                                             "CRÉDITO ARV",
                                                             "CREDITO DE ANTECIPAÇÃO",
                                                             "CRÉDITO DE ANTECIPAÇÃO" };

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDRECEBIMENTOAJUSTE = 100,
            DTAJUSTE = 101,
            NRCNPJ = 102,
            CDBANDEIRA = 103,
            DSMOTIVO = 104,
            VLAJUSTE = 105,
            IDEXTRATO = 106,
            IDRESUMOVENDA = 107,
            FLANTECIPACAO = 108,
            IDANTECIPACAOBANCARIADETALHE = 109,
            DTVENDA = 110,
            VLBRUTO = 111,

            // RELACIONAMENTOS
            ID_GRUPO = 216,

            CDADQUIRENTE = 302,
            DSTIPO = 303,

            // EXTRA
            SEM_AJUSTES_ANTECIPACAO = 400,
            //AJUSTES_VENDA = 401,

            CDCONTACORRENTE = 500
        };

        /// <summary>
        /// Get TbRecebimentoAjuste/TbRecebimentoAjuste
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static IQueryable<tbRecebimentoAjuste> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL
            var entity = _db.tbRecebimentoAjustes.AsQueryable<tbRecebimentoAjuste>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDRECEBIMENTOAJUSTE:
                        Int32 idRecebimentoAjuste = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRecebimentoAjuste == idRecebimentoAjuste).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.DTAJUSTE:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtAjuste.Year > dtaIni.Year || (e.dtAjuste.Year == dtaIni.Year && e.dtAjuste.Month > dtaIni.Month) ||
                                                                                          (e.dtAjuste.Year == dtaIni.Year && e.dtAjuste.Month == dtaIni.Month && e.dtAjuste.Day >= dtaIni.Day))
                                                    && (e.dtAjuste.Year < dtaFim.Year || (e.dtAjuste.Year == dtaFim.Year && e.dtAjuste.Month < dtaFim.Month) ||
                                                                                          (e.dtAjuste.Year == dtaFim.Year && e.dtAjuste.Month == dtaFim.Month && e.dtAjuste.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtAjuste.Year == dtaIni.Year && e.dtAjuste.Month == dtaIni.Month);
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtAjuste >= dta);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtAjuste.Year == dtaIni.Year && e.dtAjuste.Month == dtaIni.Month && e.dtAjuste.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira == cdBandeira).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.DSMOTIVO:
                        string dsMotivo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsMotivo.Equals(dsMotivo)).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.VLAJUSTE:
                        decimal vlAjuste = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlAjuste == vlAjuste).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.IDEXTRATO:
                        Int32 idExtrato = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idExtrato == idExtrato).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.IDANTECIPACAOBANCARIADETALHE:
                        Int32 idAntecipacaoBancariaDetalhe = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idAntecipacaoBancariaDetalhe == idAntecipacaoBancariaDetalhe).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.DTVENDA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda != null && (e.dtVenda.Value.Year > dtaIni.Year || (e.dtVenda.Value.Year == dtaIni.Year && e.dtVenda.Value.Month > dtaIni.Month) ||
                                                                                          (e.dtVenda.Value.Year == dtaIni.Year && e.dtVenda.Value.Month == dtaIni.Month && e.dtVenda.Value.Day >= dtaIni.Day))
                                                    && (e.dtVenda.Value.Year < dtaFim.Year || (e.dtVenda.Value.Year == dtaFim.Year && e.dtVenda.Value.Month < dtaFim.Month) ||
                                                                                          (e.dtVenda.Value.Year == dtaFim.Year && e.dtVenda.Value.Month == dtaFim.Month && e.dtVenda.Value.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda != null && e.dtVenda.Value.Year == dtaIni.Year && e.dtVenda.Value.Month == dtaIni.Month);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda != null && e.dtVenda.Value.Year == dtaIni.Year && e.dtVenda.Value.Month == dtaIni.Month && e.dtVenda.Value.Day == dtaIni.Day);
                        }
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.empresa.id_grupo == id_grupo).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbBandeira.cdAdquirente == cdAdquirente).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value).TrimEnd();
                        entity = entity.Where(e => e.tbBandeira.dsTipo.TrimEnd().Equals(dsTipo)).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.SEM_AJUSTES_ANTECIPACAO:
                        if (Convert.ToBoolean(item.Value))
                            // Por enquanto, só trata da Cielo e Banese
                            entity = entity.Where(e => (e.tbBandeira.cdAdquirente != 7 && e.tbBandeira.cdAdquirente != 2) ||
                                                       (e.tbBandeira.cdAdquirente == 2 && !AJUSTES_ANTECIPACAO_CIELO.Contains(e.dsMotivo)) ||
                                                       (e.tbBandeira.cdAdquirente == 7 && !AJUSTES_ANTECIPACAO_BANESE.Contains(e.dsMotivo))
                                                 ).AsQueryable<tbRecebimentoAjuste>();
                            //entity = entity.Where(e =>  e.tbBandeira.cdAdquirente != 2 || !AJUSTES_ANTECIPACAO_CIELO.Contains(e.dsMotivo)).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    //case CAMPOS.AJUSTES_VENDA:
                    //    if (Convert.ToBoolean(item.Value))
                    //        // Por enquanto, só trata do Banese
                    //        entity = entity.Where(e => e.tbBandeira.cdAdquirente == 7 && AJUSTES_VENDA_BANESE.Contains(e.dsMotivo)).AsQueryable<tbRecebimentoAjuste>();
                    //    break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        if (cdContaCorrente > 0)
                        {
                            // Obtém as filiais da conta
                            string[] filiaisDaConta = Permissoes.GetFiliaisDaConta(cdContaCorrente, _db);
                            int[] adquirentesDaConta = Permissoes.GetAdquirentesDaConta(cdContaCorrente, _db);
                            entity = entity.Where(e => filiaisDaConta.Contains(e.nrCNPJ))
                                           .Where(e => adquirentesDaConta.Contains(e.tbBandeira.cdAdquirente))
                                           .AsQueryable<tbRecebimentoAjuste>();
                            
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
                case CAMPOS.IDRECEBIMENTOAJUSTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRecebimentoAjuste).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.idRecebimentoAjuste).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.DTAJUSTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtAjuste).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.dtAjuste).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.DSMOTIVO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsMotivo).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.dsMotivo).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.VLAJUSTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlAjuste).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.vlAjuste).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.IDEXTRATO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idExtrato).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.idExtrato).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVenda).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.dtVenda).AsQueryable<tbRecebimentoAjuste>();
                    break;
            }
            #endregion

            return entity;


        }

        /// <summary>
        /// Get TbRecebimentoAjuste/TbRecebimentoAjuste
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
                    case CAMPOS.IDRECEBIMENTOAJUSTE:
                        Int32 idRecebimentoAjuste = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idRecebimentoAjuste = " + idRecebimentoAjuste);
                        break;
                    case CAMPOS.DTAJUSTE:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(SIGLA_QUERY + ".dtAjuste BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtAjuste >= '" + dt + "'");
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtAjuste <= '" + dt + " 23:59:00'");
                        }
                        //else if (item.Value.Length == 4)
                        //{
                        //    string busca = item.Value + "0101";
                        //    DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        //    where.Add("DATEPART(YEAR, " + SIGLA_QUERY + ".dtAjuste) = " + data.Year);
                        //}
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("DATEPART(YEAR, " + SIGLA_QUERY + ".dtAjuste) = " + data.Year + " AND DATEPART(MONTH, " + SIGLA_QUERY + ".dtAjuste) = " + data.Month);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(data);
                            where.Add(SIGLA_QUERY + ".dtAjuste BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nrCNPJ = '" + nrCNPJ + "'");
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".cdBandeira = " + cdBandeira);
                        break;
                    case CAMPOS.DSMOTIVO:
                        string dsMotivo = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".dsMotivo = '" + dsMotivo + "'");
                        break;
                    case CAMPOS.VLAJUSTE:
                        decimal vlAjuste = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlAjuste = " + vlAjuste.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.IDEXTRATO:
                        Int32 idExtrato = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idExtrato = " + idExtrato);
                        break;
                    case CAMPOS.IDRESUMOVENDA:
                        Int32 idResumoVenda = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idResumoVenda = " + idResumoVenda);
                        break;
                    case CAMPOS.FLANTECIPACAO:
                        Boolean flAntecipacao = Convert.ToBoolean(item.Value);
                        where.Add(SIGLA_QUERY + ".flAntecipacao = " + DataBaseQueries.GetBoolean(flAntecipacao));
                        break;
                    case CAMPOS.IDANTECIPACAOBANCARIADETALHE:
                        Int32 idAntecipacaoBancariaDetalhe = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe = " + idAntecipacaoBancariaDetalhe);
                        break;
                    case CAMPOS.DTVENDA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(SIGLA_QUERY + ".dtVenda BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtVenda >= '" + dt + "'");
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtVenda <= '" + dt + " 23:59:00'");
                        }
                        //else if (item.Value.Length == 4)
                        //{
                        //    string busca = item.Value + "0101";
                        //    DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        //    where.Add("DATEPART(YEAR, " + SIGLA_QUERY + ".dtVenda) = " + data.Year);
                        //}
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("DATEPART(YEAR, " + SIGLA_QUERY + ".dtVenda) = " + data.Year + " AND DATEPART(MONTH, " + SIGLA_QUERY + ".dtVenda) = " + data.Month);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(data);
                            where.Add(SIGLA_QUERY + ".dtVenda BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.ID_GRUPO:
                        // Adiciona os joins
                        if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj = " + SIGLA_QUERY + ".nrCNPJ");
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        where.Add(GatewayEmpresa.SIGLA_QUERY + ".id_grupo = " + id_grupo);
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        // Adiciona os joins
                        if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + SIGLA_QUERY + ".cdBandeira");
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        where.Add(GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente);
                        break;
                    case CAMPOS.DSTIPO:
                        // Adiciona os joins
                        if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + SIGLA_QUERY + ".cdBandeira");
                        string dsTipo = Convert.ToString(item.Value).TrimEnd();
                        where.Add(GatewayTbBandeira.SIGLA_QUERY + ".dsTipo like '" + dsTipo + "%'");
                        break;
                    case CAMPOS.SEM_AJUSTES_ANTECIPACAO:
                        if (Convert.ToBoolean(item.Value))
                        {
                            // Por enquanto, só trata da Cielo e do Banese
                            string ajustesCielo = "'" + string.Join("', '", AJUSTES_ANTECIPACAO_CIELO) + "'";
                            string ajustesBanese = "'" + string.Join("', '", AJUSTES_ANTECIPACAO_BANESE) + "'"; 
                            // Adiciona os joins
                            if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                                join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + SIGLA_QUERY + ".cdBandeira");
                            where.Add("(" + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente NOT IN (2, 7))" +
                                      " OR (" + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente = 2 AND " + SIGLA_QUERY + ".dsMotivo NOT IN (" + ajustesCielo + "))" +
                                      " OR (" + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente = 7 AND " + SIGLA_QUERY + ".dsMotivo NOT IN (" + ajustesBanese + "))");
                            //where.Add(GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente != 2" + " OR " + SIGLA_QUERY + ".dsMotivo NOT IN (" + ajustesCielo + ")");
                            
                        }
                        break;
                    //case CAMPOS.AJUSTES_VENDA:
                    //    if (Convert.ToBoolean(item.Value))
                    //    {
                    //        string ajustes = string.Join("', '", AJUSTES_VENDA_BANESE);
                    //        if (!ajustes.Equals(""))
                    //        {
                    //            // Por enquanto, só trata do Banese
                    //            ajustes = "'" + ajustes + "'";
                    //            // Adiciona os joins
                    //            if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                    //                join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + SIGLA_QUERY + ".cdBandeira");
                    //            where.Add(GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente = 7 AND " + SIGLA_QUERY + ".dsMotivo IN (" + ajustes + ")");
                    //        }
                    //    }
                    //    break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        if (cdContaCorrente > 0)
                        {
                            // Obtém as filiais da conta
                            string filiaisDaConta = "'" + string.Join("', '", Permissoes.GetFiliaisDaConta(cdContaCorrente, (painel_taxservices_dbContext) null)) + "'";
                            string adquirentesDaConta = string.Join(", ", Permissoes.GetAdquirentesDaConta(cdContaCorrente, (painel_taxservices_dbContext) null));
                            // Adiciona os joins
                            if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                                join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + SIGLA_QUERY + ".cdBandeira");
                            where.Add(GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente in (" + adquirentesDaConta + ")");
                            where.Add(SIGLA_QUERY + ".nrCNPJ in (" + filiaisDaConta + ")");    

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
                case CAMPOS.IDRECEBIMENTOAJUSTE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idRecebimentoAjuste ASC");
                    else order.Add(SIGLA_QUERY + ".idRecebimentoAjuste DESC");
                    break;
                case CAMPOS.DTAJUSTE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dtAjuste ASC");
                    else order.Add(SIGLA_QUERY + ".dtAjuste DESC");
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nrCNPJ ASC");
                    else order.Add(SIGLA_QUERY + ".nrCNPJ DESC");
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdBandeira ASC");
                    else order.Add(SIGLA_QUERY + ".cdBandeira DESC");
                    break;
                case CAMPOS.DSMOTIVO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dsMotivo ASC");
                    else order.Add(SIGLA_QUERY + ".dsMotivo DESC");
                    break;
                case CAMPOS.VLAJUSTE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlAjuste ASC");
                    else order.Add(SIGLA_QUERY + ".vlAjuste DESC");
                    break;
                case CAMPOS.IDEXTRATO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idExtrato ASC");
                    else order.Add(SIGLA_QUERY + ".idExtrato DESC");
                    break;
                case CAMPOS.IDRESUMOVENDA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idResumoVenda ASC");
                    else order.Add(SIGLA_QUERY + ".idResumoVenda DESC");
                    break;
                case CAMPOS.FLANTECIPACAO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".flAntecipacao ASC");
                    else order.Add(SIGLA_QUERY + ".flAntecipacao DESC");
                    break;
                case CAMPOS.IDANTECIPACAOBANCARIADETALHE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe ASC");
                    else order.Add(SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe DESC");
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dtVenda ASC");
                    else order.Add(SIGLA_QUERY + ".dtVenda DESC");
                    break;
            }
            #endregion

            return new SimpleDataBaseQuery(null, "card.tbRecebimentoAjuste " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());


        }


        /// <summary>
        /// Retorna TbRecebimentoAjuste/TbRecebimentoAjuste
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
                List<dynamic> CollectionTbRecebimentoAjuste = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Implementar o filtro por Grupo apartir do TOKEN do Usuário
                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                        queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (!CnpjEmpresa.Equals(""))
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.NRCNPJ, out outValue))
                        queryString["" + (int)CAMPOS.NRCNPJ] = CnpjEmpresa;
                    else
                        queryString.Add("" + (int)CAMPOS.NRCNPJ, CnpjEmpresa);
                }

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
                    CollectionTbRecebimentoAjuste = query.Select(e => new
                    {

                        idRecebimentoAjuste = e.idRecebimentoAjuste,
                        dtAjuste = e.dtAjuste,
                        nrCNPJ = e.nrCNPJ,
                        cdBandeira = e.cdBandeira,
                        dsMotivo = e.dsMotivo,
                        vlAjuste = e.vlAjuste,
                        idExtrato = e.idExtrato,
                        idResumoVenda = e.idResumoVenda,
                        flAntecipacao = e.flAntecipacao,
                        idAntecipacaoBancariaDetalhe = e.idAntecipacaoBancariaDetalhe,
                        dtVenda = e.dtVenda,
                        vlBruto = e.vlBruto,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbRecebimentoAjuste = query.Select(e => new
                    {

                        idRecebimentoAjuste = e.idRecebimentoAjuste,
                        dtAjuste = e.dtAjuste,
                        nrCNPJ = e.nrCNPJ,
                        cdBandeira = e.cdBandeira,
                        dsMotivo = e.dsMotivo,
                        vlAjuste = e.vlAjuste,
                        idExtrato = e.idExtrato,
                        idResumoVenda = e.idResumoVenda,
                        flAntecipacao = e.flAntecipacao,
                        idAntecipacaoBancariaDetalhe = e.idAntecipacaoBancariaDetalhe,
                        dtVenda = e.dtVenda,
                        vlBruto = e.vlBruto,
                    }).ToList<dynamic>();
                }

                transaction.Commit();

                retorno.Registros = CollectionTbRecebimentoAjuste;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar recebimento" : erro);
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
        /// Adiciona nova TbRecebimentoAjuste
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbRecebimentoAjuste param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.tbRecebimentoAjustes.Add(param);
                _db.SaveChanges();
                // Commit
                //transaction.Commit();
                return param.idRecebimentoAjuste;
            }
            catch (Exception e)
            {
                // Rollback
               // transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar recebimento" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbRecebimentoAjuste
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRecebimentoAjuste, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
           // DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbRecebimentoAjuste tbRecebimentoAjuste = _db.tbRecebimentoAjustes.Where(e => e.idRecebimentoAjuste == idRecebimentoAjuste).FirstOrDefault();
                if (tbRecebimentoAjuste == null) throw new Exception("Ajuste inexistente!");
                _db.tbRecebimentoAjustes.Remove(tbRecebimentoAjuste);
                _db.SaveChanges();
                // Commit
                //transaction.Commit();
            }
            catch (Exception e)
            {
                // Rollback
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar recebimento" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Altera tbRecebimentoAjuste
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbRecebimentoAjuste param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbRecebimentoAjuste value = _db.tbRecebimentoAjustes
                        .Where(e => e.idRecebimentoAjuste == param.idRecebimentoAjuste)
                        .First<tbRecebimentoAjuste>();


                if (param.dtAjuste != null && param.dtAjuste != value.dtAjuste)
                    value.dtAjuste = param.dtAjuste;
                if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                    value.nrCNPJ = param.nrCNPJ;
                if (param.cdBandeira != 0 && param.cdBandeira != value.cdBandeira)
                    value.cdBandeira = param.cdBandeira;
                if (param.dsMotivo != null && param.dsMotivo != value.dsMotivo)
                    value.dsMotivo = param.dsMotivo;
                if (param.vlAjuste != 0 && param.vlAjuste != value.vlAjuste)
                    value.vlAjuste = param.vlAjuste;
                if (param.idExtrato != null && param.idExtrato != value.idExtrato)
                    value.idExtrato = param.idExtrato;
                _db.SaveChanges();
                // Commit
                //transaction.Commit();
            }
            catch (Exception e)
            {
                // Rollback
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar recebimento" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }

        }

    }
}
