using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Pos
{
    public class GatewayLoginOperadora
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayLoginOperadora()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            LOGIN = 101,
            SENHA = 102,
            DATA_ALTERACAO = 103,
            STATUS = 104,
            CNPJ = 105,
            IDOPERADORA = 106,
            IDGRUPO = 107,
            ESTABELECIMENTO = 108,
            NRCNPJCENTRALIZADORA = 109,
            CDESTABELECIMENTOCONSULTA = 110,

            // PERSONALIZADO
            NMOPERADORA = 201,

        };

        /// <summary>
        /// Get LoginOperadora/LoginOperadora
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<LoginOperadora> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.LoginOperadoras.AsQueryable<LoginOperadora>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.ID:
                        Int32 id = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.LOGIN:
                        string login = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.login.Equals(login)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.SENHA:
                        string senha = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.senha.Equals(senha)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.DATA_ALTERACAO:
                        DateTime data_alteracao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data_alteracao.Equals(data_alteracao)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.STATUS:
                        Boolean status = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.status.Equals(status)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.IDGRUPO:
                        Int32 idGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idGrupo.Equals(idGrupo)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.ESTABELECIMENTO:
                        string estabelecimento = Convert.ToString(item.Value);
                        if (estabelecimento.Contains("%")) // usa LIKE
                        {
                            string busca = estabelecimento.Replace("%", "").ToString();
                            // Remove os zeros a esquerda
                            while (busca.StartsWith("0")) busca = busca.Substring(1);
                            // Consult
                            entity = entity.Where(e => e.estabelecimento.Contains(busca)).AsQueryable<LoginOperadora>();
                        }
                        else
                            entity = entity.Where(e => e.estabelecimento.Equals(estabelecimento)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.NRCNPJCENTRALIZADORA:
                        string nrCNPJCentralizadora = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJCentralizadora != null && e.nrCNPJCentralizadora.Equals(nrCNPJCentralizadora)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.CDESTABELECIMENTOCONSULTA:
                        string cdEstabelecimentoConsulta = Convert.ToString(item.Value);
                        if (cdEstabelecimentoConsulta.Contains("%")) // usa LIKE
                        {
                            string busca = cdEstabelecimentoConsulta.Replace("%", "").ToString();
                            // Remove os zeros a esquerda
                            while (busca.StartsWith("0")) busca = busca.Substring(1);
                            // Consult
                            entity = entity.Where(e => e.cdEstabelecimentoConsulta != null && e.cdEstabelecimentoConsulta.Contains(busca)).AsQueryable<LoginOperadora>();
                        }
                        else
                            entity = entity.Where(e => e.cdEstabelecimentoConsulta != null && e.cdEstabelecimentoConsulta.Equals(cdEstabelecimentoConsulta)).AsQueryable<LoginOperadora>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.ID:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.LOGIN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.login).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.login).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.SENHA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.senha).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.senha).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.DATA_ALTERACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data_alteracao).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.data_alteracao).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.STATUS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.status).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.status).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.IDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idGrupo).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.idGrupo).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.ESTABELECIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.estabelecimento).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.estabelecimento).AsQueryable<LoginOperadora>();
                    break;
                
                // PERONALIZADO
                case CAMPOS.NMOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Operadora.nmOperadora).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.Operadora.nmOperadora).AsQueryable<LoginOperadora>();
                    break;


            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna LoginOperadora/LoginOperadora
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {

            try
            {
                // Filtro de grupo empresa e filial
                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (colecao != 3 || 
                    !queryString.TryGetValue("" + (int)CAMPOS.ESTABELECIMENTO, out outValue) || // se for a coleção 3, tem que ter filtro de estabelecimento
                    !Permissoes.isAtosRole(token)) // Como coleção 3 é para consultar estabelecimento, quem for de perfil atos
                {
                    if (IdGrupo != 0)
                    {
                        if (queryString.TryGetValue("" + (int)CAMPOS.IDGRUPO, out outValue))
                            queryString["" + (int)CAMPOS.IDGRUPO] = IdGrupo.ToString();
                        else
                            queryString.Add("" + (int)CAMPOS.IDGRUPO, IdGrupo.ToString());
                    }
                    string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                    if (!CnpjEmpresa.Equals(""))
                    {
                        if (queryString.TryGetValue("" + (int)CAMPOS.CNPJ, out outValue))
                            queryString["" + (int)CAMPOS.CNPJ] = CnpjEmpresa;
                        else
                            queryString.Add("" + (int)CAMPOS.CNPJ, CnpjEmpresa);
                    }
                }


                //DECLARAÇÕES
                List<dynamic> CollectionLoginOperadora = new List<dynamic>();
                Retorno retorno = new Retorno();

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

                // Perfil vendedor ?
                if (IdGrupo == 0 && Permissoes.isAtosRoleVendedor(token))
                {
                    // Perfil Comercial tem uma carteira de clientes específica
                    List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(token);
                    query = query.Where(e => listaIdsGruposEmpresas.Contains(e.empresa.id_grupo)).AsQueryable<LoginOperadora>();
                }


                // PAGINAÇÃO
                if (colecao != 4) // senhas inválidas
                {
                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = query.Count();

                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;
                }

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                // COLEÇÃO DE RETORNO
                if (colecao == 1)
                {
                    CollectionLoginOperadora = query.Select(e => new
                    {

                        id = e.id,
                        login = e.login,
                        senha = e.senha,
                        data_alteracao = e.data_alteracao,
                        status = e.status,
                        cnpj = e.cnpj,
                        idOperadora = e.idOperadora,
                        idGrupo = e.idGrupo,
                        estabelecimento = e.estabelecimento,
                        nrCNPJCentralizadora = e.nrCNPJCentralizadora,
                        cdEstabelecimentoConsulta = e.cdEstabelecimentoConsulta
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionLoginOperadora = query.Select(e => new
                    {

                        id = e.id,
                        login = e.login,
                        senha = e.senha,
                        data_alteracao = e.data_alteracao,
                        status = e.status,
                        cnpj = e.cnpj,
                        idOperadora = e.idOperadora,
                        idGrupo = e.idGrupo,
                        estabelecimento = e.estabelecimento,
                        nrCNPJCentralizadora = e.nrCNPJCentralizadora,
                        cdEstabelecimentoConsulta = e.cdEstabelecimentoConsulta
                    }).ToList<dynamic>();
                }
                else if (colecao == 2) // [mobile]
                {
                    CollectionLoginOperadora = query.Select(e => new
                    {

                        id = e.id,
                        login = e.login,
                        senha = e.senha,
                        status = e.status,
                        estabelecimento = e.estabelecimento,
                        operadora = e.Operadora.nmOperadora,
                        idOperadora = e.idOperadora,
                        nrCNPJCentralizadora = e.nrCNPJCentralizadora,
                        cdEstabelecimentoConsulta = e.cdEstabelecimentoConsulta
                    }).ToList<dynamic>();
                }
                else if (colecao == 3) // [web]/Dados de Acesso/Grid
                {
                    CollectionLoginOperadora = query.Select(e => new
                    {

                        id = e.id,
                        login = e.login,
                        senha = e.senha,
                        status = e.status,
                        empresa = new { nu_cnpj = e.empresa.nu_cnpj, ds_fantasia = e.empresa.ds_fantasia, filial = e.empresa.filial },
                        grupoempresa = new { id_grupo = e.empresa.id_grupo, ds_nome = e.empresa.grupo_empresa.ds_nome },
                        //operadora = new { id = e.Operadora.id ,desOperadora = e.Operadora.nmOperadora },
                        operadora = new { id = e.Operadora.id, desOperadora = _db.Adquirentes.Where(a => a.nome.Equals(e.Operadora.nmOperadora)).Select(a => a.descricao).FirstOrDefault() },
                        estabelecimento = e.estabelecimento,
                        empresaCentralizadora = e.nrCNPJCentralizadora == null ? null : new { nu_cnpj = e.empresaCentralizadora.nu_cnpj, ds_fantasia = e.empresaCentralizadora.ds_fantasia, filial = e.empresaCentralizadora.filial },
                        cdEstabelecimentoConsulta = e.cdEstabelecimentoConsulta
                    }).ToList<dynamic>();

                    CollectionLoginOperadora = CollectionLoginOperadora.OrderBy(l => l.operadora.desOperadora).ToList();
                }
                else if (colecao == 4) // [web]/Senhas Inválidas/Grid
                {
                    var subQuery = query
                        .OrderBy(e => e.grupo_empresa.ds_nome).ThenBy(e => e.empresa.ds_fantasia).ThenBy(e => e.Operadora.nmOperadora)
                        .Where(e => e.status == false)
                        .Select(e => new
                                    {

                                        id = e.id,
                                        login = e.login,
                                        senha = e.senha,
                                        status = e.status,
                                        empresa = e.empresa.ds_fantasia + (e.empresa.filial != null ? " " + e.empresa.filial : ""),
                                        grupoempresa = e.grupo_empresa.ds_nome,
                                        //operadora = e.Operadora.nmOperadora,
                                        operadora = _db.Adquirentes.Where(a => a.nome.Equals(e.Operadora.nmOperadora)).Select(a => a.descricao).FirstOrDefault(),
                                        estabelecimento = e.estabelecimento,
                                        empresaCentralizadora = e.nrCNPJCentralizadora == null ? null : new { nu_cnpj = e.empresaCentralizadora.nu_cnpj, ds_fantasia = e.empresaCentralizadora.ds_fantasia, filial = e.empresaCentralizadora.filial },
                                        cdEstabelecimentoConsulta = e.cdEstabelecimentoConsulta
                                    });

                    retorno.TotalDeRegistros = subQuery.Count();

                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionLoginOperadora = subQuery.ToList<dynamic>();
                }



                retorno.Registros = CollectionLoginOperadora;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar login operadora" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova LoginOperadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, LoginOperadora param)
        {
            try
            {
                // Avalia adquirente
                if (param.Operadora.nmOperadora == null) throw new Exception("Adquirente inválida");
                Models.Adquirente op = _db.Adquirentes.Where(o => o.nome.Equals(param.Operadora.nmOperadora)).FirstOrDefault(); // o que é enviado é o nome e não a descrição
                if (op == null) throw new Exception("Adquirente inválida");

                // Busca possível registro da adquirente para a filial
                LoginOperadora loginOperadora = _db.LoginOperadoras
                                                        .Where(l => l.cnpj.Equals(param.cnpj))
                                                        .Where(l => l.Operadora.nmOperadora.Equals(param.Operadora.nmOperadora))
                                                        .FirstOrDefault();

                if (loginOperadora == null)
                {
                    // Cria um novo registro de loginoperadora para a filial

                    // Procura pela operadora
                    Operadora operadora = _db.Operadoras
                                                /*.Where(e => _db.LoginOperadoras
                                                                    .Where(l => l.cnpj.Equals(param.cnpj))
                                                                    .Select(l => l.idOperadora)
                                                                    .ToList().Contains(e.id)
                                                      )*/
                                                .Where(e => e.idGrupoEmpresa == param.idGrupo)
                                                .Where(e => e.nmOperadora.Equals(param.Operadora.nmOperadora))
                                                .FirstOrDefault();

                    if (operadora == null)
                    {
                        // Cria um novo registro de operadora para a filial
                        Operadora newOperadora = new Operadora();
                        newOperadora.nmOperadora = param.Operadora.nmOperadora;
                        newOperadora.idGrupoEmpresa = param.idGrupo;
                        _db.Operadoras.Add(newOperadora);
                        _db.SaveChanges();
                        // Obtém o id da nova operadora
                        param.idOperadora = newOperadora.id;
                    }
                    else
                        // Já existe operadora com nmOperadora para a filial
                        param.idOperadora = operadora.id;

                    // Salva na base
                    param.Operadora.idGrupoEmpresa = param.idGrupo;
                    param.status = true;
                    try
                    {
                        _db.LoginOperadoras.Add(param);
                        _db.SaveChanges();
                    }
                    catch(Exception e)
                    {
                        // Remove a operadora criada
                        GatewayOperadora.Delete(token, param.idOperadora);
                        // Reporta a falha
                        throw new Exception("500");
                    }
                }
                else
                {
                    // Já existe uma operadora registrada (nmOperadora) para aquela filial
                    param.idOperadora = loginOperadora.idOperadora;
                    param.id = loginOperadora.id;
                    // Atualiza o status para true
                    if (!loginOperadora.status)
                    {
                        loginOperadora.status = true;
                        _db.SaveChanges();
                    }
                }

                // Adiciona log execution
                try
                {
                    adicionaLogExecution(param.id, param.idOperadora);
                }
                catch (Exception e)
                {
                    // Remove LoginOperadora e Operadora possivelmente criados
                    Delete(token, param.id);
                    // Reporta a falha
                    if (e is DbEntityValidationException)
                    {
                        string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                        throw new Exception(erro.Equals("") ? "Houve uma falha ao armazenar logexecution!" : erro);
                    }
                    throw new Exception("Houve uma falha ao armazenar logexecution! " + e.Message);
                }

                return param.id;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar login operadora" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma LoginOperadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            try
            {
                LoginOperadora loginOperadora = _db.LoginOperadoras.Where(e => e.id == id).FirstOrDefault();
                if (loginOperadora == null) throw new Exception("Login Operadora inexistente");

                // Remove logexecutions
                _db.LogExecutions.RemoveRange(_db.LogExecutions.Where(l => l.idLoginOperadora == loginOperadora.id));

                // Remove operadora
                _db.Operadoras.RemoveRange(_db.Operadoras.Where(o => o.id == loginOperadora.idOperadora));

                // Por fim, remove login operadora
                _db.LoginOperadoras.Remove(loginOperadora);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar login operadora" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera LoginOperadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, LoginOperadora param)
        {
            try
            {
                LoginOperadora value = _db.LoginOperadoras
                        .Where(e => e.id == param.id)
                        .First<LoginOperadora>();


                if (param.login != null && param.login != value.login)
                    value.login = param.login;
                if (param.senha != null && param.senha != value.senha)
                {
                    value.senha = param.senha;
                    value.status = true;
                    value.data_alteracao = DateTime.Now;
                }
                if (param.estabelecimento != null && param.estabelecimento != value.estabelecimento)
                    value.estabelecimento = param.estabelecimento;
                if (param.cdEstabelecimentoConsulta != null && param.cdEstabelecimentoConsulta != value.cdEstabelecimentoConsulta)
                {
                    if (param.nrCNPJCentralizadora != null && param.nrCNPJCentralizadora != value.nrCNPJCentralizadora)
                    {
                        if (param.cdEstabelecimentoConsulta.Equals(""))
                            value.cdEstabelecimentoConsulta = null;
                        else
                            value.cdEstabelecimentoConsulta = param.cdEstabelecimentoConsulta;
                    }
                }
                if (param.nrCNPJCentralizadora != null && param.nrCNPJCentralizadora != value.nrCNPJCentralizadora)
                {
                    if (param.nrCNPJCentralizadora.Equals(""))
                        value.nrCNPJCentralizadora = null;
                    else
                        value.nrCNPJCentralizadora = param.nrCNPJCentralizadora;
                }
                _db.SaveChanges();

                try
                {
                    adicionaLogExecution(value.id, value.idOperadora);
                }
                catch
                {
                    // Falha! Como logar isso sem lançar excessão aqui?
                }
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar login operadora" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /**
          * Armazena o logexecution, caso não exista
          */
        private static void adicionaLogExecution(Int32 idLoginOperadora, Int32 idOperadora)
        {
            // Armazena o logexecution, caso não exista
            // Verifica se já existe logExecution para o registro corrente
            LogExecution logExecution = _db.LogExecutions
                                                .Where(l => l.idLoginOperadora == idLoginOperadora)
                                                .Where(l => l.idOperadora == idOperadora)
                                                .FirstOrDefault();

            if (logExecution == null)
            {
                Operadora op = _db.Operadoras.Where(o => o.id == idOperadora).FirstOrDefault(); // o que é enviado é o nome e não a descrição
                if (op == null) return;
                Models.Adquirente adquirente = _db.Adquirentes.Where(o => o.nome.Equals(op.nmOperadora)).FirstOrDefault();
                if (adquirente == null) return;
                DateTime hrExec = (DateTime)adquirente.hraExecucao;

                LogExecution newLogExecution = new LogExecution();
                newLogExecution.idLoginOperadora = idLoginOperadora;
                newLogExecution.idOperadora = idOperadora;
                newLogExecution.statusExecution = "7"; //0 = Em execução; 1 = Executado com Sucesso; 2 = Erro na Execução; 3 = Re-Executar; 7 = Elegivel
                                                       //newLogExecution.dtaFiltroTransacoes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, hrExec.Hour, hrExec.Minute, hrExec.Second);
                                                       //newLogExecution.dtaFiltroTransacoesFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, hrExec.Hour, hrExec.Minute, hrExec.Second);
                newLogExecution.dtaFiltroTransacoes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                newLogExecution.dtaFiltroTransacoesFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                newLogExecution.dtaExecucaoProxima = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2, hrExec.Hour, hrExec.Minute, hrExec.Second);
                newLogExecution.qtdTransacoes = 0;
                newLogExecution.vlTotalTransacoes = new decimal(0.0);

                _db.LogExecutions.Add(newLogExecution);
                _db.SaveChanges();
            }

        }

    }
}
