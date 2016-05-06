using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using api.Negocios.Util;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data;

namespace api.Negocios.Card
{
    public class GatewayTbBancoParametro
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbBancoParametro()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "BP";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDBANCO = 100,
            DSMEMO = 101,
            CDADQUIRENTE = 102,
            DSTIPO = 103,
            FLVISIVEL = 104,
            NRCNPJ = 105,
            DSTIPOCARTAO = 106,
            CDBANDEIRA = 107,
            FLANTECIPACAO = 108,
            CDGRUPO = 109,

            // RELACIONAMENTOS
            DSADQUIRENTE = 201,

            ID_GRUPO = 316

        };

        /// <summary>
        /// Get TbBancoParametro/TbBancoParametro
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbBancoParametro> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL
            var entity = _db.tbBancoParametros.AsQueryable<tbBancoParametro>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDBANCO:
                        string cdBanco = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdBanco.Equals(cdBanco)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.DSMEMO:
                        string dsMemo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsMemo.Equals(dsMemo)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        if(cdAdquirente == -1)
                            // Somente os registros sem adquirentes associados
                            entity = entity.Where(e => e.cdAdquirente == null).AsQueryable<tbBancoParametro>();
                        else
                            entity = entity.Where(e => e.cdAdquirente == cdAdquirente).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsTipo.Equals(dsTipo)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.FLVISIVEL:
                        bool flVisivel = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flVisivel == flVisivel).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCnpj = Convert.ToString(item.Value);
                        if(nrCnpj.Equals("")) entity = entity.Where(e => e.nrCnpj == null).AsQueryable<tbBancoParametro>();
                        else entity = entity.Where(e => e.nrCnpj != null && e.nrCnpj.Equals(nrCnpj)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.DSTIPOCARTAO:
                        string dsTipoCartao = Convert.ToString(item.Value);
                        if (dsTipoCartao.Equals("")) entity = entity.Where(e => e.dsTipoCartao == null).AsQueryable<tbBancoParametro>();
                        else entity = entity.Where(e => e.dsTipoCartao != null && e.dsTipoCartao.TrimEnd().Equals(dsTipoCartao)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        if (cdBandeira == -1)
                            // Somente os registros sem adquirentes associados
                            entity = entity.Where(e => e.cdBandeira == null).AsQueryable<tbBancoParametro>();
                        else
                            entity = entity.Where(e => e.cdBandeira == cdBandeira).AsQueryable<tbBancoParametro>();
                        break;
                        

                    // PERSONALIZADO
                    case CAMPOS.DSADQUIRENTE:
                        string nome = Convert.ToString(item.Value);
                        if (nome.Contains("%")) // usa LIKE
                        {
                            string busca = nome.Replace("%", "").ToString();
                            entity = entity.Where(e => e.tbAdquirente.dsAdquirente.Contains(busca)).AsQueryable<tbBancoParametro>();
                        }
                        else
                            entity = entity.Where(e => e.tbAdquirente.dsAdquirente.Equals(nome)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        grupo_empresa grupo = _db.grupo_empresa.Where(g => g.id_grupo == id_grupo).FirstOrDefault();
                        if (grupo == null) continue;
                        // Armazena todos os parâmetros existentes para o grupo, baseado nos extratos "upados"
                        //List<ParametroBancario> parametros = new List<ParametroBancario>();
                        List<string> memos = new List<string>();
                        foreach (tbContaCorrente tbContaCorrente in grupo.tbContaCorrentes) {
                            foreach (tbExtrato tbExtrato in tbContaCorrente.tbExtratos)
                            {
                                //if (!parametros.Any(p => p.CdBanco.Equals(tbExtrato.tbContaCorrente.cdBanco) && 
                                                         //p.DsMemo.Equals(tbExtrato.dsDocumento)))
                                if(!memos.Contains(tbExtrato.dsDocumento))
                                {
                                    memos.Add(tbExtrato.dsDocumento);
                                    /*parametros.Add(new ParametroBancario()
                                    {
                                        CdBanco = tbExtrato.tbContaCorrente.cdBanco,
                                        DsMemo = tbExtrato.dsDocumento,
                                        DsTipo = tbExtrato.dsTipo
                                    });*/
                                }
                            }
                        }
                        // Para os parâmetros que não estão associados à uma filial, só são exibidos os que constam nos extratos bancários associados ao grupo
                        entity = entity.Where(e => (e.cdGrupo != null && e.cdGrupo == id_grupo) ||
                                                   (e.nrCnpj == null && memos.Contains(e.dsMemo))//grupo.tbContaCorrentes.Where(c => c.tbExtratos.Where(x => x.tbContaCorrente.cdBanco.Equals(e.cdBanco) && x.dsDocumento.Equals(e.dsMemo)).Count() > 0).Count() > 0) 
                                                   || (e.nrCnpj != null && e.empresa.id_grupo == id_grupo)).AsQueryable<tbBancoParametro>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDBANCO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBanco).ThenBy(e => e.dsMemo).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.cdBanco).ThenByDescending(e => e.dsMemo).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.DSMEMO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsMemo).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.dsMemo).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.DSTIPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsTipo).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.dsTipo).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.FLVISIVEL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flVisivel).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.flVisivel).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCnpj).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.nrCnpj).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.DSTIPOCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsTipoCartao).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.dsTipoCartao).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbBancoParametro>();
                    break;

                // PERSONALIZADO
                case CAMPOS.DSADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tbAdquirente.dsAdquirente).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.tbAdquirente.dsAdquirente).AsQueryable<tbBancoParametro>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbBancoParametro/TbBancoParametro
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
                List<dynamic> CollectionTbBancoParametro = new List<dynamic>();
                Retorno retorno = new Retorno();

                // FILTRO DE FILIAL
                string outValue = null;
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (!CnpjEmpresa.Equals(""))
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.NRCNPJ, out outValue))
                        queryString["" + (int)CAMPOS.NRCNPJ] = CnpjEmpresa;
                    else
                        queryString.Add("" + (int)CAMPOS.NRCNPJ, CnpjEmpresa);
                }
                else
                {
                    // NÃO ESTÁ ASSOCIADO A UM FILIAL => VERIFICA SE ESTÁ ASSOCIADO A UM GRUPO
                    Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                    if (IdGrupo != 0)
                    {
                        if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                            queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                        else
                            queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    }
                }

                // POR DEFAULT, LISTA SOMENTE OS QUE ESTÃO VISÍVEIS
                //if (!queryString.TryGetValue("" + (int)CAMPOS.FLVISIVEL, out outValue))
                //    queryString.Add("" + (int)CAMPOS.FLVISIVEL, Convert.ToString(true));

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
                    CollectionTbBancoParametro = query.Select(e => new
                    {

                        cdBanco = e.cdBanco,
                        dsMemo = e.dsMemo,
                        cdAdquirente = e.cdAdquirente,
                        dsTipo = e.dsTipo,
                        flVisivel = e.flVisivel,
                        nrCnpj = e.nrCnpj,
                        dsTipoCartao = e.dsTipoCartao.TrimEnd(),
                        cdBandeira = e.cdBandeira,
                        flAntecipacao = e.flAntecipacao,
                        cdGrupo = e.cdGrupo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbBancoParametro = query.Select(e => new
                    {

                        cdBanco = e.cdBanco,
                        dsMemo = e.dsMemo,
                        cdAdquirente = e.cdAdquirente,
                        dsTipo = e.dsTipo,
                        flVisivel = e.flVisivel,
                        nrCnpj = e.nrCnpj,
                        dsTipoCartao = e.dsTipoCartao.TrimEnd(),
                        cdBandeira = e.cdBandeira,
                        flAntecipacao = e.flAntecipacao,
                        cdGrupo = e.cdGrupo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2) // [WEB] 
                {
                    List<dynamic> bancoParametros = query.Select(e => new
                    {
                        dsMemo = e.dsMemo,
                        adquirente = e.cdAdquirente == null ? null : new 
                                        {
                                            cdAdquirente = e.tbAdquirente.cdAdquirente,
                                            nmAdquirente = e.tbAdquirente.nmAdquirente,
                                            dsAdquirente = e.tbAdquirente.dsAdquirente,
                                            stAdquirente = e.tbAdquirente.stAdquirente,
                                        },
                        dsTipo = e.dsTipo,
                        flVisivel = e.flVisivel,
                        flAntecipacao = e.flAntecipacao,
                        dsTipoCartao = e.dsTipoCartao.TrimEnd(),
                        empresa = e.nrCnpj == null ? null : new
                        {
                            nu_cnpj = e.empresa.nu_cnpj,
                            ds_fantasia = e.empresa.ds_fantasia,
                            filial = e.empresa.filial
                        },
                        grupoempresa = e.nrCnpj != null ? new
                        {
                            id_grupo = e.empresa.id_grupo,
                            ds_nome = e.empresa.grupo_empresa.ds_nome
                        } : e.cdGrupo != 0 ? _db.grupo_empresa.Where(t => t.id_grupo == e.cdGrupo).Select(t => new
                        {
                            id_grupo = t.id_grupo,
                            ds_nome = t.ds_nome
                        }).FirstOrDefault() : null,
                        bandeira = e.cdBandeira == null ? null : new
                        {
                            cdBandeira = e.tbBandeira.cdBandeira,
                            dsBandeira = e.tbBandeira.dsBandeira,
                            dsTipo = e.tbBandeira.dsTipo
                        },
                        banco = new { Codigo = e.cdBanco, NomeExtenso = "" }, // Não dá para chamar a função direto daqui pois esse código é convertido em SQL e não acessa os dados de um objeto em memória

                    }).ToList<dynamic>();

                    // Após transformar em lista (isto é, trazer para a memória), atualiza o valor do NomeExtenso associado ao banco
                    foreach (var bancoParametro in bancoParametros)
                    {
                        CollectionTbBancoParametro.Add(new
                        {
                            dsMemo = bancoParametro.dsMemo,
                            adquirente = bancoParametro.adquirente,
                            dsTipo = bancoParametro.dsTipo,
                            flVisivel = bancoParametro.flVisivel,
                            flAntecipacao = bancoParametro.flAntecipacao,
                            empresa = bancoParametro.empresa,
                            grupoempresa = bancoParametro.grupoempresa,
                            bandeira = bancoParametro.bandeira,
                            dsTipoCartao = bancoParametro.dsTipoCartao,
                            banco = new { Codigo = bancoParametro.banco.Codigo, NomeExtenso = GatewayBancos.Get(bancoParametro.banco.Codigo) },
                        });
                    }
                }

                transaction.Commit();

                retorno.Registros = CollectionTbBancoParametro;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar parâmetros bancários" : erro);
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
        /// Adiciona nova TbBancoParametro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Add(string token, tbBancoParametro param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try 
            { 
                tbBancoParametro parametro = _db.tbBancoParametros.Where(e => e.cdBanco.Equals(param.cdBanco))
                                                                 .Where(e => e.dsMemo.Equals(param.dsMemo))
                                                                 .Where(e => e.cdGrupo == param.cdGrupo)
                                                                 .FirstOrDefault();

                if (parametro != null) throw new Exception("Parâmetro bancário já existe");

                if (param.cdAdquirente == -1) param.cdAdquirente = null;
                if (param.nrCnpj != null && param.nrCnpj.Equals("")) param.nrCnpj = null;
                if (param.dsTipoCartao != null && param.dsTipoCartao.Equals("")) param.dsTipoCartao = null;
                if (param.cdBandeira == -1) param.cdBandeira = null;

                param.flVisivel = true;

                _db.tbBancoParametros.Add(param);
                _db.SaveChanges();
                transaction.Commit();
                return param.cdBanco;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar parâmetro bancário" : erro);
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
        /// Apaga uma TbBancoParametro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, string cdBanco, string dsMemo, int cdGrupo, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbBancoParametro parametro = _db.tbBancoParametros.Where(e => e.cdBanco.Equals(cdBanco))
                                                                 .Where(e => e.dsMemo.Equals(dsMemo))
                                                                 .Where(e => e.cdGrupo == cdGrupo)
                                                                 .FirstOrDefault();

                if (parametro == null) throw new Exception("Parâmetro bancário inexistente");

                _db.tbBancoParametros.Remove(parametro);
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao remover parâmetro bancário" : erro);
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
        /// Altera tbBancoParametro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, ParametrosBancarios param, painel_taxservices_dbContext _dbContext = null)//tbBancoParametro param)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                Int32 idGrupo = Permissoes.GetIdGrupo(token, _db);

                foreach (ParametroBancario parametro in param.Parametros)
                {
                    tbBancoParametro value = _db.tbBancoParametros.Where(e => e.cdBanco.Equals(parametro.CdBanco))
                                                                 .Where(e => e.dsMemo.Equals(parametro.DsMemo))
                                                                 .Where(e => e.cdGrupo == parametro.CdGrupo)
                                                                 .FirstOrDefault();
                    if (value != null)
                    {
                        if (param.Deletar)
                        {
                            // Remove
                            _db.tbBancoParametros.Remove(value);
                        }
                        else
                        {
                            // Grupo
                            if (parametro.CdGrupo == 0 && idGrupo != 0 && value.nrCnpj == null && (param.NrCnpj == null || param.NrCnpj.Trim().Equals("")))
                            {
                                // Estava como um parâmetro que serve para todos os grupos
                                // Não tem filial associada
                                // => Marca a parametrização como sendo específica do grupo

                                // Cria o parâmetro
                                tbBancoParametro parametroG = new tbBancoParametro();
                                parametroG.cdBanco = value.cdBanco;
                                parametroG.dsMemo = value.dsMemo;
                                parametroG.cdGrupo = idGrupo;
                                parametroG.cdAdquirente = param.CdAdquirente == null ? value.cdAdquirente : param.CdAdquirente == -1 ? null : param.CdAdquirente;
                                parametroG.dsTipo = parametro.DsTipo == null ? value.dsTipo : parametro.DsTipo;
                                parametroG.dsTipoCartao = param.DsTipoCartao == null ? value.dsTipoCartao : param.DsTipoCartao.Equals("") ? null : param.DsTipoCartao;
                                parametroG.cdBandeira = param.CdBandeira == null ? value.cdBandeira : param.CdBandeira == -1 ? null : param.CdBandeira;
                                parametroG.flVisivel = param.FlVisivel;
                                parametroG.flAntecipacao = param.FlAntecipacao == null ? value.flAntecipacao : param.FlAntecipacao.Value;
                                // Avalia cnpj
                                if (param.NrCnpj != null)
                                {
                                    if (param.NrCnpj.Equals("")) parametroG.nrCnpj = null;
                                    else 
                                    {
                                        int cdGrupo = _db.empresas.Where(t => t.nu_cnpj.Equals(param.NrCnpj)).Select(t => t.id_grupo).FirstOrDefault();
                                        if (cdGrupo == idGrupo)
                                            parametroG.nrCnpj = param.NrCnpj;
                                        else
                                            parametroG.nrCnpj = null;
                                    }
                                }

                                // Add
                                _db.tbBancoParametros.Add(parametroG);
                            }
                            else
                            {
                                // TIPO
                                if (parametro.DsTipo != null && parametro.DsTipo != value.dsTipo)
                                    value.dsTipo = parametro.DsTipo;
                                // Adquirente
                                if (param.CdAdquirente != null && param.CdAdquirente != value.cdAdquirente)
                                {
                                    if (param.CdAdquirente == -1) value.cdAdquirente = null;
                                    else value.cdAdquirente = param.CdAdquirente;
                                }
                                // Filial
                                if (param.NrCnpj != null && (value.nrCnpj == null || !param.NrCnpj.Equals(value.nrCnpj)))
                                {
                                    if (param.NrCnpj.Equals("")) value.nrCnpj = null;
                                    else value.nrCnpj = param.NrCnpj;
                                }
                                // Tipo Cartão
                                if (param.DsTipoCartao != null && (value.dsTipoCartao == null || !param.DsTipoCartao.Equals(value.dsTipoCartao.TrimEnd())))
                                {
                                    if (param.DsTipoCartao.Equals("")) value.dsTipoCartao = null;
                                    else value.dsTipoCartao = param.DsTipoCartao;
                                }
                                // Bandeira
                                if (param.CdBandeira != null && param.CdBandeira != value.cdBandeira)
                                {
                                    if (param.CdBandeira == -1) value.cdBandeira = null;
                                    else value.cdBandeira = param.CdBandeira;
                                }
                                // Visibilidade
                                if (param.FlVisivel != value.flVisivel) value.flVisivel = param.FlVisivel;
                                // Antecipação
                                if (param.FlAntecipacao != null && param.FlAntecipacao.Value != value.flAntecipacao)
                                    value.flAntecipacao = param.FlAntecipacao.Value;
                            }
                        }
                        // Salva
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
                    throw new Exception(erro.Equals("") ? "Falha ao atualizar parâmetros bancários" : erro);
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
