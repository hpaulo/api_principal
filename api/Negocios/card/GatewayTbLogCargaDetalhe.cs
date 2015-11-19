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

namespace api.Negocios.Card
{
    public class GatewayTbLogCargaDetalhe
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbLogCargaDetalhe()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDLOGCARGADETALHE = 100,
            IDLOGCARGA = 101,
            DTEXECUCAOINI = 102,
            DTEXECUCAOFIM = 103,
            FLSTATUS = 104,
            DSMENSAGEM = 105,
            DSMODALIDADE = 106,
            QTTRANSACOES = 107,
            VLTOTALPROCESSADO = 108,

        };

        /// <summary>
        /// Get TbLogCargaDetalhe/TbLogCargaDetalhe
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbLogCargaDetalhe> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbLogCargaDetalhes.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDLOGCARGADETALHE:
                        Int32 idLogCargaDetalhe = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogCargaDetalhe.Equals(idLogCargaDetalhe)).AsQueryable<tbLogCargaDetalhe>();
                        break;
                    case CAMPOS.IDLOGCARGA:
                        Int32 idLogCarga = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogCarga.Equals(idLogCarga)).AsQueryable<tbLogCargaDetalhe>();
                        break;
                    case CAMPOS.DTEXECUCAOINI:
                        DateTime dtExecucaoIni = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtExecucaoIni.Equals(dtExecucaoIni)).AsQueryable<tbLogCargaDetalhe>();
                        break;
                    case CAMPOS.DTEXECUCAOFIM:
                        DateTime dtExecucaoFim = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtExecucaoFim.Equals(dtExecucaoFim)).AsQueryable<tbLogCargaDetalhe>();
                        break;
                    case CAMPOS.FLSTATUS:
                        byte flStatus = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.flStatus.Equals(flStatus)).AsQueryable<tbLogCargaDetalhe>();
                        break;
                    case CAMPOS.DSMENSAGEM:
                        string dsMensagem = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsMensagem.Equals(dsMensagem)).AsQueryable<tbLogCargaDetalhe>();
                        break;
                    case CAMPOS.DSMODALIDADE:
                        string dsModalidade = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsModalidade.Equals(dsModalidade)).AsQueryable<tbLogCargaDetalhe>();
                        break;
                    case CAMPOS.QTTRANSACOES:
                        Int32 qtTransacoes = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.qtTransacoes.Equals(qtTransacoes)).AsQueryable<tbLogCargaDetalhe>();
                        break;
                    case CAMPOS.VLTOTALPROCESSADO:
                        decimal vlTotalProcessado = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlTotalProcessado.Equals(vlTotalProcessado)).AsQueryable<tbLogCargaDetalhe>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDLOGCARGADETALHE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogCargaDetalhe).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.idLogCargaDetalhe).AsQueryable<tbLogCargaDetalhe>();
                    break;
                case CAMPOS.IDLOGCARGA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogCarga).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.idLogCarga).AsQueryable<tbLogCargaDetalhe>();
                    break;
                case CAMPOS.DTEXECUCAOINI:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtExecucaoIni).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.dtExecucaoIni).AsQueryable<tbLogCargaDetalhe>();
                    break;
                case CAMPOS.DTEXECUCAOFIM:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtExecucaoFim).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.dtExecucaoFim).AsQueryable<tbLogCargaDetalhe>();
                    break;
                case CAMPOS.FLSTATUS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flStatus).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.flStatus).AsQueryable<tbLogCargaDetalhe>();
                    break;
                case CAMPOS.DSMENSAGEM:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsMensagem).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.dsMensagem).AsQueryable<tbLogCargaDetalhe>();
                    break;
                case CAMPOS.DSMODALIDADE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsModalidade).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.dsModalidade).AsQueryable<tbLogCargaDetalhe>();
                    break;
                case CAMPOS.QTTRANSACOES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtTransacoes).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.qtTransacoes).AsQueryable<tbLogCargaDetalhe>();
                    break;
                case CAMPOS.VLTOTALPROCESSADO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlTotalProcessado).AsQueryable<tbLogCargaDetalhe>();
                    else entity = entity.OrderByDescending(e => e.vlTotalProcessado).AsQueryable<tbLogCargaDetalhe>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbLogCargaDetalhe/TbLogCargaDetalhe
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbLogCargaDetalhe = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

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
                    CollectionTbLogCargaDetalhe = query.Select(e => new
                    {

                        idLogCargaDetalhe = e.idLogCargaDetalhe,
                        idLogCarga = e.idLogCarga,
                        dtExecucaoIni = e.dtExecucaoIni,
                        dtExecucaoFim = e.dtExecucaoFim,
                        flStatus = e.flStatus,
                        dsMensagem = e.dsMensagem,
                        dsModalidade = e.dsModalidade,
                        qtTransacoes = e.qtTransacoes,
                        vlTotalProcessado = e.vlTotalProcessado,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbLogCargaDetalhe = query.Select(e => new
                    {

                        idLogCargaDetalhe = e.idLogCargaDetalhe,
                        idLogCarga = e.idLogCarga,
                        dtExecucaoIni = e.dtExecucaoIni,
                        dtExecucaoFim = e.dtExecucaoFim,
                        flStatus = e.flStatus,
                        dsMensagem = e.dsMensagem,
                        dsModalidade = e.dsModalidade,
                        qtTransacoes = e.qtTransacoes,
                        vlTotalProcessado = e.vlTotalProcessado,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbLogCargaDetalhe;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbLogCargaDetalhe" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbLogCargaDetalhe
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbLogCargaDetalhe param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbLogCargaDetalhes.Add(param);
                _db.SaveChanges();
                return param.idLogCargaDetalhe;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbLogCargaDetalhe" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbLogCargaDetalhe
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idLogCargaDetalhe)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbLogCargaDetalhes.Remove(_db.tbLogCargaDetalhes.Where(e => e.idLogCargaDetalhe.Equals(idLogCargaDetalhe)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbLogCargaDetalhe" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Altera tbLogCargaDetalhe
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbLogCargaDetalhe param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbLogCargaDetalhe value = _db.tbLogCargaDetalhes
                        .Where(e => e.idLogCargaDetalhe.Equals(param.idLogCargaDetalhe))
                        .First<tbLogCargaDetalhe>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idLogCargaDetalhe != null && param.idLogCargaDetalhe != value.idLogCargaDetalhe)
                    value.idLogCargaDetalhe = param.idLogCargaDetalhe;
                if (param.idLogCarga != null && param.idLogCarga != value.idLogCarga)
                    value.idLogCarga = param.idLogCarga;
                if (param.dtExecucaoIni != null && param.dtExecucaoIni != value.dtExecucaoIni)
                    value.dtExecucaoIni = param.dtExecucaoIni;
                if (param.dtExecucaoFim != null && param.dtExecucaoFim != value.dtExecucaoFim)
                    value.dtExecucaoFim = param.dtExecucaoFim;
                if (param.flStatus != null && param.flStatus != value.flStatus)
                    value.flStatus = param.flStatus;
                if (param.dsMensagem != null && param.dsMensagem != value.dsMensagem)
                    value.dsMensagem = param.dsMensagem;
                if (param.dsModalidade != null && param.dsModalidade != value.dsModalidade)
                    value.dsModalidade = param.dsModalidade;
                if (param.qtTransacoes != null && param.qtTransacoes != value.qtTransacoes)
                    value.qtTransacoes = param.qtTransacoes;
                if (param.vlTotalProcessado != null && param.vlTotalProcessado != value.vlTotalProcessado)
                    value.vlTotalProcessado = param.vlTotalProcessado;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbLogCargaDetalhe" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}