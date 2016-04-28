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
    public class GatewayTbRecebimentoVenda
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRecebimentoVenda()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "VD";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDRECEBIMENTOVENDA = 100,
            NRCNPJ = 101,
            NRNSU = 102,
            DTVENDA = 103,
            CDADQUIRENTE = 104,
            DSBANDEIRA = 105,
            VLVENDA = 106,
            QTPARCELAS = 107,
            CDERP = 108,

            // RELACIONAMENTOS
            ID_GRUPO = 216,
        };

        /// <summary>
        /// Get TbRecebimentoVenda/TbRecebimentoVenda
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbRecebimentoVenda> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbRecebimentoVendas.AsQueryable<tbRecebimentoVenda>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDRECEBIMENTOVENDA:
                        Int32 idRecebimentoVenda = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRecebimentoVenda.Equals(idRecebimentoVenda)).AsQueryable<tbRecebimentoVenda>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbRecebimentoVenda>();
                        break;
                    case CAMPOS.NRNSU:
                        string nrNSU = Convert.ToString(item.Value);
                        if (nrNSU.Contains("%")) // usa LIKE => ENDS WITH
                        {
                            string busca = nrNSU.Replace("%", "").ToString();
                            entity = entity.Where(e => e.nrNSU.EndsWith(busca)).AsQueryable<tbRecebimentoVenda>();
                        }
                        else
                            entity = entity.Where(e => e.nrNSU.Equals(nrNSU)).AsQueryable<tbRecebimentoVenda>();
                        break;
                    case CAMPOS.DTVENDA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda >= dtaIni && e.dtVenda <= dtaFim).AsQueryable<tbRecebimentoVenda>();
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda.Year == dtaIni.Year && e.dtVenda.Month == dtaIni.Month && e.dtVenda.Day == dtaIni.Day).AsQueryable<tbRecebimentoVenda>();
                        }
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        //entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbRecebimentoVenda>();
                        entity = entity.Where(e => _db.tbBandeiraSacados.Where(t => t.cdSacado.Equals(e.cdSacado) && t.cdGrupo == e.empresa.id_grupo)
                                                                        .Where(t => t.tbBandeira.cdAdquirente == cdAdquirente).Count() > 0).AsQueryable<tbRecebimentoVenda>();
                        break;
                    case CAMPOS.DSBANDEIRA:
                        string dsBandeira = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsBandeira.Equals(dsBandeira)).AsQueryable<tbRecebimentoVenda>();
                        break;
                    case CAMPOS.VLVENDA:
                        decimal vlVenda = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlVenda.Equals(vlVenda)).AsQueryable<tbRecebimentoVenda>();
                        break;
                    case CAMPOS.QTPARCELAS:
                        byte qtParcelas = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.qtParcelas.Equals(qtParcelas)).AsQueryable<tbRecebimentoVenda>();
                        break;
                    case CAMPOS.CDERP:
                        string cdERP = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdERP.Equals(cdERP)).AsQueryable<tbRecebimentoVenda>();
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.empresa.id_grupo == id_grupo).AsQueryable<tbRecebimentoVenda>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDRECEBIMENTOVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRecebimentoVenda).AsQueryable<tbRecebimentoVenda>();
                    else entity = entity.OrderByDescending(e => e.idRecebimentoVenda).AsQueryable<tbRecebimentoVenda>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbRecebimentoVenda>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbRecebimentoVenda>();
                    break;
                case CAMPOS.NRNSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrNSU).AsQueryable<tbRecebimentoVenda>();
                    else entity = entity.OrderByDescending(e => e.nrNSU).AsQueryable<tbRecebimentoVenda>();
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVenda).AsQueryable<tbRecebimentoVenda>();
                    else entity = entity.OrderByDescending(e => e.dtVenda).AsQueryable<tbRecebimentoVenda>();
                    break;
                //case CAMPOS.CDADQUIRENTE:
                //    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbRecebimentoVenda>();
                //    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbRecebimentoVenda>();
                //    break;
                case CAMPOS.DSBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsBandeira).AsQueryable<tbRecebimentoVenda>();
                    else entity = entity.OrderByDescending(e => e.dsBandeira).AsQueryable<tbRecebimentoVenda>();
                    break;
                case CAMPOS.VLVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlVenda).AsQueryable<tbRecebimentoVenda>();
                    else entity = entity.OrderByDescending(e => e.vlVenda).AsQueryable<tbRecebimentoVenda>();
                    break;
                case CAMPOS.QTPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtParcelas).AsQueryable<tbRecebimentoVenda>();
                    else entity = entity.OrderByDescending(e => e.qtParcelas).AsQueryable<tbRecebimentoVenda>();
                    break;
                case CAMPOS.CDERP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdERP).AsQueryable<tbRecebimentoVenda>();
                    else entity = entity.OrderByDescending(e => e.cdERP).AsQueryable<tbRecebimentoVenda>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Get TbRecebimentoVenda/TbRecebimentoVenda
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
                    case CAMPOS.IDRECEBIMENTOVENDA:
                        Int32 idRecebimentoVenda = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idRecebimentoVenda = " + idRecebimentoVenda);
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nrCNPJ = '" + nrCNPJ + "'");
                        break;
                    case CAMPOS.NRNSU:
                        string nrNSU = Convert.ToString(item.Value);
                        if (nrNSU.Contains("%")) // usa LIKE => ENDS WITH
                        {
                            string busca = nrNSU.Replace("%", "").ToString();
                            where.Add(SIGLA_QUERY + ".nrNSU like '%" + busca + "'");
                        }
                        else
                            where.Add(SIGLA_QUERY + ".nrNSU = '" + nrNSU + "'");
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
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(data);
                            where.Add(SIGLA_QUERY + ".dtVenda BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj = " + SIGLA_QUERY + ".nrCNPJ");
                        if (!join.ContainsKey("LEFT JOIN card.tbBandeiraSacado " + GatewayTbBandeiraSacado.SIGLA_QUERY))
                            join.Add("LEFT JOIN card.tbBandeiraSacado " + GatewayTbBandeiraSacado.SIGLA_QUERY, " ON " + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdSacado = " + SIGLA_QUERY + ".cdSacado AND "  + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdGrupo = " + GatewayEmpresa.SIGLA_QUERY + ".id_grupo");
                        if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdBandeira");
                        where.Add(GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente);
                        break;
                    case CAMPOS.DSBANDEIRA:
                        string dsBandeira = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".dsBandeira = '" + dsBandeira + "'");
                        break;
                    case CAMPOS.VLVENDA:
                        decimal vlVenda = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlVenda = " + vlVenda.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.QTPARCELAS:
                        byte qtParcelas = Convert.ToByte(item.Value);
                        where.Add(SIGLA_QUERY + ".qtParcelas = " + qtParcelas);
                        break;
                    case CAMPOS.CDERP:
                        string cdERP = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".cdERP = '" + cdERP + "'");
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDRECEBIMENTOVENDA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idRecebimentoVenda ASC");
                    else order.Add(SIGLA_QUERY + ".idRecebimentoVenda DESC");
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nrCNPJ ASC");
                    else order.Add(SIGLA_QUERY + ".nrCNPJ DESC");
                    break;
                case CAMPOS.NRNSU:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nrNSU ASC");
                    else order.Add(SIGLA_QUERY + ".nrNSU DESC");
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dtVenda ASC");
                    else order.Add(SIGLA_QUERY + ".dtVenda DESC");
                    break;
                //case CAMPOS.CDADQUIRENTE:
                //    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdAdquirente ASC");
                //    else order.Add(SIGLA_QUERY + ".cdAdquirente DESC");
                //    break;
                case CAMPOS.DSBANDEIRA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dsBandeira ASC");
                    else order.Add(SIGLA_QUERY + ".dsBandeira DESC");
                    break;
                case CAMPOS.VLVENDA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlVenda ASC");
                    else order.Add(SIGLA_QUERY + ".vlVenda DESC");
                    break;
                case CAMPOS.QTPARCELAS:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".qtParcelas ASC");
                    else order.Add(SIGLA_QUERY + ".qtParcelas DESC");
                    break;
                case CAMPOS.CDERP:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdERP ASC");
                    else order.Add(SIGLA_QUERY + ".cdERP DESC");
                    break;
            }
            #endregion

            return new SimpleDataBaseQuery(null, "card.tbRecebimentoVenda " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());


        }


        /// <summary>
        /// Retorna TbRecebimentoVenda/TbRecebimentoVenda
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
                List<dynamic> CollectionTbRecebimentoVenda = new List<dynamic>();
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

                List<int> vendasConciliadas = new List<int>();
                if (colecao == 2)
                {
                    // Obtém totais
                    retorno.Totais = new Dictionary<string, object>();
                    if (retorno.TotalDeRegistros > 0)
                    {
                        retorno.Totais.Add("valorTotal", query.Select(e => e.vlVenda).Cast<decimal>().Sum());
                        retorno.Totais.Add("totalCorrigidos", query.Where(e => e.dtAjuste != null).Count());
                        string script = "SELECT DISTINCT R.idRecebimentoVenda" +
                                                               " FROM pos.Recebimento R (NOLOCK)" +
                                                               " WHERE R.idRecebimentoVenda IN (" + string.Join(", ", query.Select(e => e.idRecebimentoVenda)) + ")";
                        vendasConciliadas = _db.Database.SqlQuery<int>(script).ToList();

                        retorno.Totais.Add("totalConciliados", vendasConciliadas.Count);
                    }
                    else
                    {
                        retorno.Totais.Add("valorTotal", new decimal(0.0));
                        retorno.Totais.Add("totalCorrigidos", 0);
                        retorno.Totais.Add("totalConciliados", 0);

                    }
 
                    query = query.OrderBy(e => e.dtVenda).ThenBy(e => e.empresa.ds_fantasia)/*.ThenBy(e => e.tbAdquirente.nmAdquirente)*/.ThenBy(e => e.vlVenda).ThenBy(e => e.nrNSU);
                }

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
                    CollectionTbRecebimentoVenda = query.Select(e => new
                    {

                        idRecebimentoVenda = e.idRecebimentoVenda,
                        nrCNPJ = e.nrCNPJ,
                        nrNSU = e.nrNSU,
                        dtVenda = e.dtVenda,
                        //cdAdquirente = e.cdAdquirente,
                        dsBandeira = e.dsBandeira,
                        vlVenda = e.vlVenda,
                        qtParcelas = e.qtParcelas,
                        cdERP = e.cdERP,
                        cdSacado = e.cdSacado,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbRecebimentoVenda = query.Select(e => new
                    {

                        idRecebimentoVenda = e.idRecebimentoVenda,
                        nrCNPJ = e.nrCNPJ,
                        nrNSU = e.nrNSU,
                        dtVenda = e.dtVenda,
                        //cdAdquirente = e.cdAdquirente,
                        dsBandeira = e.dsBandeira,
                        vlVenda = e.vlVenda,
                        qtParcelas = e.qtParcelas,
                        cdERP = e.cdERP,
                        cdSacado = e.cdSacado,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2) // PORTAL: Consulta Vendas ERP
                {
                    CollectionTbRecebimentoVenda = query.Select(e => new
                    {
                        idRecebimentoVenda = e.idRecebimentoVenda,
                        nrNSU = e.nrNSU,
                        empresa = _db.empresas.Where(f => f.nu_cnpj.Equals(e.nrCNPJ))
                                              .Select(f => new
                                              {
                                                  f.nu_cnpj,
                                                  f.ds_fantasia,
                                                  f.filial
                                              })
                                              .FirstOrDefault(),
                        dtVenda = e.dtVenda,
                        //tbAdquirente = _db.tbAdquirentes.Where(a => a.cdAdquirente == e.cdAdquirente)
                        //                                .Select(a => new
                        //                                {
                        //                                    a.cdAdquirente,
                        //                                    a.nmAdquirente
                        //                                })
                        //                                .FirstOrDefault(),
                        tbAdquirente = _db.tbBandeiraSacados.Where(t => t.cdSacado.Equals(e.cdSacado) && t.cdGrupo == e.empresa.id_grupo)
                                                            .Select(t => new {
                                                                cdAdquirente = t.tbBandeira.cdAdquirente, 
                                                                nmAdquirente = t.tbBandeira.tbAdquirente.nmAdquirente
                                                            })
                                                            .FirstOrDefault(),
                        dsBandeira = e.dsBandeira,
                        vlVenda = e.vlVenda,
                        qtParcelas = e.qtParcelas,
                        cdERP = e.cdERP,
                        cdSacado = e.cdSacado,
                        dtAjuste = e.dtAjuste,
                        conciliado = vendasConciliadas.Contains(e.idRecebimentoVenda)
                    }).ToList<dynamic>();
                }

                transaction.Commit();

                retorno.Registros = CollectionTbRecebimentoVenda;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao consultar vendas" : erro);
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
        /// Adiciona nova TbRecebimentoVenda
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbRecebimentoVenda param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.tbRecebimentoVendas.Add(param);
                _db.SaveChanges();
                //transaction.Commit();
                return param.idRecebimentoVenda;
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar venda" : erro);
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
        /// Apaga uma TbRecebimentoVenda
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRecebimentoVenda, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.tbRecebimentoVendas.Remove(_db.tbRecebimentoVendas.Where(e => e.idRecebimentoVenda.Equals(idRecebimentoVenda)).First());
                _db.SaveChanges();
                //transaction.Commit();
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao remover venda" : erro);
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
        /// Altera tbRecebimentoVenda
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbRecebimentoVenda param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbRecebimentoVenda value = _db.tbRecebimentoVendas
                                .Where(e => e.idRecebimentoVenda.Equals(param.idRecebimentoVenda))
                                .First<tbRecebimentoVenda>();

                if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                    value.nrCNPJ = param.nrCNPJ;
                if (param.nrNSU != null && param.nrNSU != value.nrNSU)
                    value.nrNSU = param.nrNSU;
                if (param.dtVenda != null && param.dtVenda != value.dtVenda)
                    value.dtVenda = param.dtVenda;
                //if (param.cdAdquirente != null && param.cdAdquirente != value.cdAdquirente)
                //    value.cdAdquirente = param.cdAdquirente;
                if (param.dsBandeira != null && param.dsBandeira != value.dsBandeira)
                    value.dsBandeira = param.dsBandeira;
                if (param.vlVenda != null && param.vlVenda != value.vlVenda)
                    value.vlVenda = param.vlVenda;
                if (param.qtParcelas != null && param.qtParcelas != value.qtParcelas)
                    value.qtParcelas = param.qtParcelas;
                if (param.cdERP != null && param.cdERP != value.cdERP)
                    value.cdERP = param.cdERP;
                _db.SaveChanges();
                //transaction.Commit();
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao atualizar venda" : erro);
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
