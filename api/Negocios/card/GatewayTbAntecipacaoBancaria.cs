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
using System.Globalization;

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

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDANTECIPACAOBANCARIA = 100,
            DTANTECIPACAOBANCARIA = 101,
            DTVENCIMENTO = 102,
            VLANTECIPACAO = 103,
            VLANTECIPACAOLIQUIDA = 104,
            CDADQUIRENTE = 105,
            CDBANDEIRA = 106,
            CDCONTACORRENTE = 107,
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
                        entity = entity.Where(e => e.idAntecipacaoBancaria.Equals(idAntecipacaoBancaria)).AsQueryable<tbAntecipacaoBancaria>();
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
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVencimento.Year == dtaIni.Year && e.dtVencimento.Month == dtaIni.Month && e.dtVencimento.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.VLANTECIPACAO:
                        decimal vlAntecipacao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlAntecipacao.Equals(vlAntecipacao)).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.VLANTECIPACAOLIQUIDA:
                        decimal vlAntecipacaoLiquida = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlAntecipacaoLiquida.Equals(vlAntecipacaoLiquida)).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira.Equals(cdBandeira)).AsQueryable<tbAntecipacaoBancaria>();
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdContaCorrente.Equals(cdContaCorrente)).AsQueryable<tbAntecipacaoBancaria>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtAntecipacaoBancaria).ThenBy(e => e.tbAdquirente.nmAdquirente).ThenBy(e => e.dtVencimento).ThenBy(e => e.vlAntecipacao).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.dtAntecipacaoBancaria).ThenByDescending(e => e.tbAdquirente.nmAdquirente).ThenByDescending(e => e.dtVencimento).ThenByDescending(e => e.vlAntecipacao).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.DTVENCIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVencimento).ThenBy(e => e.tbAdquirente.nmAdquirente).ThenBy(e => e.dtAntecipacaoBancaria).ThenBy(e => e.vlAntecipacao).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.dtVencimento).ThenByDescending(e => e.tbAdquirente.nmAdquirente).ThenByDescending(e => e.dtAntecipacaoBancaria).ThenByDescending(e => e.vlAntecipacao).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.VLANTECIPACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlAntecipacao).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.vlAntecipacao).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.VLANTECIPACAOLIQUIDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlAntecipacaoLiquida).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.vlAntecipacaoLiquida).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbAntecipacaoBancaria>();
                    break;
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdContaCorrente).AsQueryable<tbAntecipacaoBancaria>();
                    else entity = entity.OrderByDescending(e => e.cdContaCorrente).AsQueryable<tbAntecipacaoBancaria>();
                    break;
            }
            #endregion

            return entity;


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
                        dtVencimento = e.dtVencimento,
                        vlAntecipacao = e.vlAntecipacao,
                        vlAntecipacaoLiquida = e.vlAntecipacaoLiquida,
                        cdAdquirente = e.cdAdquirente,
                        cdBandeira = e.cdBandeira,
                        cdContaCorrente = e.cdContaCorrente,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbAntecipacaoBancaria = query.Select(e => new
                    {

                        idAntecipacaoBancaria = e.idAntecipacaoBancaria,
                        dtAntecipacaoBancaria = e.dtAntecipacaoBancaria,
                        dtVencimento = e.dtVencimento,
                        vlAntecipacao = e.vlAntecipacao,
                        vlAntecipacaoLiquida = e.vlAntecipacaoLiquida,
                        cdAdquirente = e.cdAdquirente,
                        cdBandeira = e.cdBandeira,
                        cdContaCorrente = e.cdContaCorrente,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionTbAntecipacaoBancaria = query.Select(e => new
                    {
                        idAntecipacaoBancaria = e.idAntecipacaoBancaria,
                        dtAntecipacaoBancaria = e.dtAntecipacaoBancaria,
                        dtVencimento = e.dtVencimento,
                        vlAntecipacao = e.vlAntecipacao,
                        vlAntecipacaoLiquida = e.vlAntecipacaoLiquida,
                        tbAdquirente = new { cdAdquirente = e.cdAdquirente,
                                             nmAdquirente = e.tbAdquirente.nmAdquirente
                                           },
                        tbBandeira = e.cdBandeira == null ? null : new { cdBandeira = e.cdBandeira,
                                                                         dsBandeira = e.tbBandeira.dsBandeira
                                                                       },
                        cdContaCorrente = e.cdContaCorrente,
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
                tbAntecipacaoBancaria tbAntecipacaoBancaria = new tbAntecipacaoBancaria();
                foreach (AntecipacaoBancariaVencimentos antecipacao in param.antecipacoes)
                {
                    tbAntecipacaoBancaria = new tbAntecipacaoBancaria();
                    tbAntecipacaoBancaria.cdAdquirente = param.cdAdquirente;
                    tbAntecipacaoBancaria.cdContaCorrente = param.cdContaCorrente;
                    tbAntecipacaoBancaria.dtAntecipacaoBancaria = param.dtAntecipacaoBancaria;

                    tbAntecipacaoBancaria.cdBandeira = antecipacao.cdBandeira;
                    tbAntecipacaoBancaria.dtVencimento = antecipacao.dtVencimento;
                    tbAntecipacaoBancaria.vlAntecipacao = antecipacao.vlAntecipacao;
                    tbAntecipacaoBancaria.vlAntecipacaoLiquida = antecipacao.vlAntecipacaoLiquida;

                    _db.tbAntecipacaoBancarias.Add(tbAntecipacaoBancaria);
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
        public static void Update(string token, tbAntecipacaoBancaria param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbAntecipacaoBancaria value = _db.tbAntecipacaoBancarias
                                .Where(e => e.idAntecipacaoBancaria.Equals(param.idAntecipacaoBancaria))
                                .First<tbAntecipacaoBancaria>();


                if (param.dtAntecipacaoBancaria != null && param.dtAntecipacaoBancaria != value.dtAntecipacaoBancaria)
                    value.dtAntecipacaoBancaria = param.dtAntecipacaoBancaria;
                if (param.dtVencimento != null && param.dtVencimento != value.dtVencimento)
                    value.dtVencimento = param.dtVencimento;
                if (param.vlAntecipacao != null && param.vlAntecipacao != value.vlAntecipacao)
                    value.vlAntecipacao = param.vlAntecipacao;
                if (param.vlAntecipacaoLiquida != null && param.vlAntecipacaoLiquida != value.vlAntecipacaoLiquida)
                    value.vlAntecipacaoLiquida = param.vlAntecipacaoLiquida;
                if (param.cdAdquirente != null && param.cdAdquirente != value.cdAdquirente)
                    value.cdAdquirente = param.cdAdquirente;
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
