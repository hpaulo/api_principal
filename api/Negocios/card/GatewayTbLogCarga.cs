using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using api.Negocios.Util;
using System.Globalization;
using System.Data.Entity;
using System.Data;
using api.Negocios.Cliente;

namespace api.Negocios.Card
{
    public class GatewayTbLogCarga
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbLogCarga()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "LC";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDLOGCARGA = 100,
            DTCOMPETENCIA = 101,
            NRCNPJ = 102,
            CDADQUIRENTE = 103,
            FLSTATUSVENDASCREDITO = 104,
            FLSTATUSVENDASDEBITO = 105,
            FLSTATUSPAGOSCREDITO = 106,
            FLSTATUSPAGOSDEBITO = 107,
            FLSTATUSPAGOSANTECIPACAO = 108,
            FLSTATUSRECEBER = 109,
            VLVENDACREDITO = 110,
            VLVENDADEBITO = 111,
            VLPAGOSCREDITO = 112,
            VLPAGOSDEBITO = 113,
            VLPAGOSANTECIPACAO = 114,

            // PERSONALIZADO
            ID_GRUPO = 216,

        };

        /// <summary>
        /// Get TbLogCarga/TbLogCarga
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbLogCarga> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbLogCargas.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDLOGCARGA:
                        Int32 idLogCarga = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogCarga.Equals(idLogCarga)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.DTCOMPETENCIA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtCompetencia > dtaIni && e.dtCompetencia < dtaFim).AsQueryable<tbLogCarga>();
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtCompetencia.Year == dtaIni.Year && e.dtCompetencia.Month == dtaIni.Month);
                        }
                        else
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtCompetencia.Year == dtaIni.Year && e.dtCompetencia.Month == dtaIni.Month && e.dtCompetencia.Day == dtaIni.Day).AsQueryable<tbLogCarga>();
                        }
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSVENDASCREDITO:
                        Boolean flStatusVendasCredito = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusVendasCredito.Equals(flStatusVendasCredito)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSVENDASDEBITO:
                        Boolean flStatusVendasDebito = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusVendasDebito.Equals(flStatusVendasDebito)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSPAGOSCREDITO:
                        Boolean flStatusPagosCredito = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusPagosCredito.Equals(flStatusPagosCredito)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSPAGOSDEBITO:
                        Boolean flStatusPagosDebito = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusPagosDebito.Equals(flStatusPagosDebito)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSPAGOSANTECIPACAO:
                        Boolean flStatusPagosAntecipacao = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusPagosAntecipacao.Equals(flStatusPagosAntecipacao)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.FLSTATUSRECEBER:
                        Boolean flStatusReceber = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flStatusReceber.Equals(flStatusReceber)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.VLVENDACREDITO:
                        decimal vlVendaCredito = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlVendaCredito.Equals(vlVendaCredito)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.VLVENDADEBITO:
                        decimal vlVendaDebito = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlVendaDebito.Equals(vlVendaDebito)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.VLPAGOSCREDITO:
                        decimal vlPagosCredito = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlPagosCredito.Equals(vlPagosCredito)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.VLPAGOSDEBITO:
                        decimal vlPagosDebito = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlPagosDebito.Equals(vlPagosDebito)).AsQueryable<tbLogCarga>();
                        break;
                    case CAMPOS.VLPAGOSANTECIPACAO:
                        decimal vlPagosAntecipacao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlPagosAntecipacao.Equals(vlPagosAntecipacao)).AsQueryable<tbLogCarga>();
                        break;

                    // PERSONALIZADO
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.empresa.id_grupo == id_grupo).AsQueryable<tbLogCarga>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDLOGCARGA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogCarga).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.idLogCarga).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.DTCOMPETENCIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtCompetencia).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.dtCompetencia).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSVENDASCREDITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusVendasCredito).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusVendasCredito).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSVENDASDEBITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusVendasDebito).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusVendasDebito).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSPAGOSCREDITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusPagosCredito).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusPagosCredito).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSPAGOSDEBITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusPagosDebito).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusPagosDebito).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSPAGOSANTECIPACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusPagosAntecipacao).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusPagosAntecipacao).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.FLSTATUSRECEBER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatusReceber).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.flStatusReceber).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.VLVENDACREDITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlVendaCredito).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.vlVendaCredito).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.VLVENDADEBITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlVendaDebito).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.vlVendaDebito).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.VLPAGOSCREDITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlPagosCredito).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.vlPagosCredito).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.VLPAGOSDEBITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlPagosDebito).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.vlPagosDebito).AsQueryable<tbLogCarga>();
                    break;
                case CAMPOS.VLPAGOSANTECIPACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlPagosAntecipacao).AsQueryable<tbLogCarga>();
                    else entity = entity.OrderByDescending(e => e.vlPagosAntecipacao).AsQueryable<tbLogCarga>();
                    break;

            }
            #endregion

            return entity;


        }

        /// <summary>
        /// Get TbLogCarga/TbLogCarga
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
                    case CAMPOS.IDLOGCARGA:
                        Int32 idLogCarga = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idLogCarga = " + idLogCarga);
                        break;
                    case CAMPOS.DTCOMPETENCIA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(SIGLA_QUERY + ".dtCompetencia BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("DATEPART(YEAR, " + SIGLA_QUERY + ".dtCompetencia) = " + data.Year + " AND DATEPART(MONTH, " + SIGLA_QUERY + ".dtCompetencia) = " + data.Month);
                        }
                        else
                        {
                            string busca = item.Value;
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(data);
                            where.Add(SIGLA_QUERY + ".dtCompetencia BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nrCNPJ = '" + nrCNPJ + "'");
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente);
                        break;
                    case CAMPOS.FLSTATUSVENDASCREDITO:
                        Boolean flStatusVendasCredito = Convert.ToBoolean(item.Value);
                        where.Add(SIGLA_QUERY + ".flStatusVendasCredito = " + DataBaseQueries.GetBoolean(flStatusVendasCredito));
                        break;
                    case CAMPOS.FLSTATUSVENDASDEBITO:
                        Boolean flStatusVendasDebito = Convert.ToBoolean(item.Value);
                        where.Add(SIGLA_QUERY + ".flStatusVendasDebito = " + DataBaseQueries.GetBoolean(flStatusVendasDebito));
                        break;
                    case CAMPOS.FLSTATUSPAGOSCREDITO:
                        Boolean flStatusPagosCredito = Convert.ToBoolean(item.Value);
                        where.Add(SIGLA_QUERY + ".flStatusPagosCredito = " + DataBaseQueries.GetBoolean(flStatusPagosCredito));
                        break;
                    case CAMPOS.FLSTATUSPAGOSDEBITO:
                        Boolean flStatusPagosDebito = Convert.ToBoolean(item.Value);
                        where.Add(SIGLA_QUERY + ".flStatusPagosDebito = " + DataBaseQueries.GetBoolean(flStatusPagosDebito));
                        break;
                    case CAMPOS.FLSTATUSPAGOSANTECIPACAO:
                        Boolean flStatusPagosAntecipacao = Convert.ToBoolean(item.Value);
                        where.Add(SIGLA_QUERY + ".flStatusPagosAntecipacao = " + DataBaseQueries.GetBoolean(flStatusPagosAntecipacao));
                        break;
                    case CAMPOS.FLSTATUSRECEBER:
                        Boolean flStatusReceber = Convert.ToBoolean(item.Value);
                        where.Add(SIGLA_QUERY + ".flStatusReceber = " + DataBaseQueries.GetBoolean(flStatusReceber));
                        break;
                    case CAMPOS.VLVENDACREDITO:
                        decimal vlVendaCredito = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlVendaCredito = " + vlVendaCredito.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.VLVENDADEBITO:
                        decimal vlVendaDebito = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlVendaDebito = " + vlVendaDebito.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.VLPAGOSCREDITO:
                        decimal vlPagosCredito = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlPagosCredito = " + vlPagosCredito.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.VLPAGOSDEBITO:
                        decimal vlPagosDebito = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlPagosDebito = " + vlPagosDebito.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.VLPAGOSANTECIPACAO:
                        decimal vlPagosAntecipacao = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlPagosAntecipacao = " + vlPagosAntecipacao.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;

                    // PERSONALIZADO
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj = " + SIGLA_QUERY + ".nrCNPJ");
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
                case CAMPOS.IDLOGCARGA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idLogCarga ASC");
                    else order.Add(SIGLA_QUERY + ".idLogCarga DESC");
                    break;
                case CAMPOS.DTCOMPETENCIA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dtCompetencia ASC");
                    else order.Add(SIGLA_QUERY + ".dtCompetencia DESC");
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nrCNPJ ASC");
                    else order.Add(SIGLA_QUERY + ".nrCNPJ DESC");
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdAdquirente ASC");
                    else order.Add(SIGLA_QUERY + ".cdAdquirente DESC");
                    break;
                case CAMPOS.FLSTATUSVENDASCREDITO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".flStatusVendasCredito ASC");
                    else order.Add(SIGLA_QUERY + ".flStatusVendasCredito DESC");
                    break;
                case CAMPOS.FLSTATUSVENDASDEBITO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".flStatusVendasDebito ASC");
                    else order.Add(SIGLA_QUERY + ".flStatusVendasDebito DESC");
                    break;
                case CAMPOS.FLSTATUSPAGOSCREDITO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".flStatusPagosCredito ASC");
                    else order.Add(SIGLA_QUERY + ".flStatusPagosCredito DESC");
                    break;
                case CAMPOS.FLSTATUSPAGOSDEBITO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".flStatusPagosDebito ASC");
                    else order.Add(SIGLA_QUERY + ".flStatusPagosDebito DESC");
                    break;
                case CAMPOS.FLSTATUSPAGOSANTECIPACAO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".flStatusPagosAntecipacao ASC");
                    else order.Add(SIGLA_QUERY + ".flStatusPagosAntecipacao DESC");
                    break;
                case CAMPOS.FLSTATUSRECEBER:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".flStatusReceber ASC");
                    else order.Add(SIGLA_QUERY + ".flStatusReceber DESC");
                    break;
                case CAMPOS.VLVENDACREDITO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlVendaCredito ASC");
                    else order.Add(SIGLA_QUERY + ".vlVendaCredito DESC");
                    break;
                case CAMPOS.VLVENDADEBITO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlVendaDebito ASC");
                    else order.Add(SIGLA_QUERY + ".vlVendaDebito DESC");
                    break;
                case CAMPOS.VLPAGOSCREDITO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlPagosCredito ASC");
                    else order.Add(SIGLA_QUERY + ".vlPagosCredito DESC");
                    break;
                case CAMPOS.VLPAGOSDEBITO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlPagosDebito ASC");
                    else order.Add(SIGLA_QUERY + ".vlPagosDebito DESC");
                    break;
                case CAMPOS.VLPAGOSANTECIPACAO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlPagosAntecipacao ASC");
                    else order.Add(SIGLA_QUERY + ".vlPagosAntecipacao DESC");
                    break;
            }
            #endregion

            return new SimpleDataBaseQuery(null, "card.tbLogCarga " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());


        }


        /// <summary>
        /// Retorna TbLogCarga/TbLogCarga
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
                List<dynamic> CollectionTbLogCarga = new List<dynamic>();
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
                    CollectionTbLogCarga = query.Select(e => new
                    {

                        idLogCarga = e.idLogCarga,
                        dtCompetencia = e.dtCompetencia,
                        nrCNPJ = e.nrCNPJ,
                        cdAdquirente = e.cdAdquirente,
                        flStatusVendasCredito = e.flStatusVendasCredito,
                        flStatusVendasDebito = e.flStatusVendasDebito,
                        flStatusPagosCredito = e.flStatusPagosCredito,
                        flStatusPagosDebito = e.flStatusPagosDebito,
                        flStatusPagosAntecipacao = e.flStatusPagosAntecipacao,
                        flStatusReceber = e.flStatusReceber,
                        vlVendaCredito = e.vlVendaCredito,
                        vlVendaDebito = e.vlVendaDebito,
                        vlPagosCredito = e.vlPagosCredito,
                        vlPagosDebito = e.vlPagosDebito,
                        vlPagosAntecipacao = e.vlPagosAntecipacao,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbLogCarga = query.Select(e => new
                    {

                        idLogCarga = e.idLogCarga,
                        dtCompetencia = e.dtCompetencia,
                        nrCNPJ = e.nrCNPJ,
                        cdAdquirente = e.cdAdquirente,
                        flStatusVendasCredito = e.flStatusVendasCredito,
                        flStatusVendasDebito = e.flStatusVendasDebito,
                        flStatusPagosCredito = e.flStatusPagosCredito,
                        flStatusPagosDebito = e.flStatusPagosDebito,
                        flStatusPagosAntecipacao = e.flStatusPagosAntecipacao,
                        flStatusReceber = e.flStatusReceber,
                        vlVendaCredito = e.vlVendaCredito,
                        vlVendaDebito = e.vlVendaDebito,
                        vlPagosCredito = e.vlPagosCredito,
                        vlPagosDebito = e.vlPagosDebito,
                        vlPagosAntecipacao = e.vlPagosAntecipacao,
                    }).ToList<dynamic>();
                }

                transaction.Commit();

                retorno.Registros = CollectionTbLogCarga;

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
        /// Adiciona nova TbLogCarga
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbLogCarga param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.tbLogCargas.Add(param);
                _db.SaveChanges();
                transaction.Commit();
                return param.idLogCarga;
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
        /// Apaga uma TbLogCarga
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idLogCarga, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.tbLogCargas.Remove(_db.tbLogCargas.Where(e => e.idLogCarga.Equals(idLogCarga)).First());
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
        /// Altera tbLogCarga
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbLogCarga param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbLogCarga value = _db.tbLogCargas
                                .Where(e => e.idLogCarga.Equals(param.idLogCarga))
                                .First<tbLogCarga>();
                if (param.idLogCarga != null && param.idLogCarga != value.idLogCarga)
                    value.idLogCarga = param.idLogCarga;
                if (param.dtCompetencia != null && param.dtCompetencia != value.dtCompetencia)
                    value.dtCompetencia = param.dtCompetencia;
                if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                    value.nrCNPJ = param.nrCNPJ;
                if (param.cdAdquirente != null && param.cdAdquirente != value.cdAdquirente)
                    value.cdAdquirente = param.cdAdquirente;
                if (param.flStatusVendasCredito != null && param.flStatusVendasCredito != value.flStatusVendasCredito)
                    value.flStatusVendasCredito = param.flStatusVendasCredito;
                if (param.flStatusVendasDebito != null && param.flStatusVendasDebito != value.flStatusVendasDebito)
                    value.flStatusVendasDebito = param.flStatusVendasDebito;
                if (param.flStatusPagosCredito != null && param.flStatusPagosCredito != value.flStatusPagosCredito)
                    value.flStatusPagosCredito = param.flStatusPagosCredito;
                if (param.flStatusPagosDebito != null && param.flStatusPagosDebito != value.flStatusPagosDebito)
                    value.flStatusPagosDebito = param.flStatusPagosDebito;
                if (param.flStatusPagosAntecipacao != null && param.flStatusPagosAntecipacao != value.flStatusPagosAntecipacao)
                    value.flStatusPagosAntecipacao = param.flStatusPagosAntecipacao;
                if (param.flStatusReceber != null && param.flStatusReceber != value.flStatusReceber)
                    value.flStatusReceber = param.flStatusReceber;
                if (param.vlVendaCredito != null && param.vlVendaCredito != value.vlVendaCredito)
                    value.vlVendaCredito = param.vlVendaCredito;
                if (param.vlVendaDebito != null && param.vlVendaDebito != value.vlVendaDebito)
                    value.vlVendaDebito = param.vlVendaDebito;
                if (param.vlPagosCredito != null && param.vlPagosCredito != value.vlPagosCredito)
                    value.vlPagosCredito = param.vlPagosCredito;
                if (param.vlPagosDebito != null && param.vlPagosDebito != value.vlPagosDebito)
                    value.vlPagosDebito = param.vlPagosDebito;
                if (param.vlPagosAntecipacao != null && param.vlPagosAntecipacao != value.vlPagosAntecipacao)
                    value.vlPagosAntecipacao = param.vlPagosAntecipacao;
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