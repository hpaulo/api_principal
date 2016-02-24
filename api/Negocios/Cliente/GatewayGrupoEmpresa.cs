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


namespace api.Negocios.Cliente
{
    public class GatewayGrupoEmpresa
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayGrupoEmpresa()
        {
           //_db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID_GRUPO = 100,
            DS_NOME = 101,
            DT_CADASTRO = 102,
            TOKEN = 103,
            FL_CARDSERVICES = 104,
            FL_TAXSERVICES = 105,
            FL_PROINFO = 106,
            CDPRIORIDADE = 107,

        };

        /// <summary>
        /// Get Grupo_empresa/Grupo_empresa
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<grupo_empresa> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.grupo_empresa.AsQueryable<grupo_empresa>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_grupo.Equals(id_grupo)).AsQueryable<grupo_empresa>();
                        break;
                    case CAMPOS.DS_NOME:
                        string ds_nome = Convert.ToString(item.Value);
                        if (ds_nome.Contains("%")) // usa LIKE
                        {
                            string busca = ds_nome.Replace("%", "").ToString();
                            entity = _db.grupo_empresa.Where(e => e.ds_nome.Contains(busca));
                        }
                        else
                            entity = entity.Where(e => e.ds_nome.Equals(ds_nome)).AsQueryable<grupo_empresa>();
                        break;
                    case CAMPOS.DT_CADASTRO:
                        DateTime dt_cadastro = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dt_cadastro.Equals(dt_cadastro)).AsQueryable<grupo_empresa>();
                        break;
                    case CAMPOS.TOKEN:
                        string token = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.token.Equals(token)).AsQueryable<grupo_empresa>();
                        break;
                    case CAMPOS.FL_CARDSERVICES:
                        Boolean fl_cardservices = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.fl_cardservices.Equals(fl_cardservices)).AsQueryable<grupo_empresa>();
                        break;
                    case CAMPOS.FL_TAXSERVICES:
                        Boolean fl_taxservices = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.fl_taxservices.Equals(fl_taxservices)).AsQueryable<grupo_empresa>();
                        break;
                    case CAMPOS.FL_PROINFO:
                        Boolean fl_proinfo = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.fl_proinfo.Equals(fl_proinfo)).AsQueryable<grupo_empresa>();
                        break;
                    case CAMPOS.CDPRIORIDADE:
                        byte cdPrioridade = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.cdPrioridade == cdPrioridade).AsQueryable<grupo_empresa>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.ID_GRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_grupo).AsQueryable<grupo_empresa>();
                    else entity = entity.OrderByDescending(e => e.id_grupo).AsQueryable<grupo_empresa>();
                    break;
                case CAMPOS.DS_NOME:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_nome).AsQueryable<grupo_empresa>();
                    else entity = entity.OrderByDescending(e => e.ds_nome).AsQueryable<grupo_empresa>();
                    break;
                case CAMPOS.DT_CADASTRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dt_cadastro).AsQueryable<grupo_empresa>();
                    else entity = entity.OrderByDescending(e => e.dt_cadastro).AsQueryable<grupo_empresa>();
                    break;
                case CAMPOS.TOKEN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.token).AsQueryable<grupo_empresa>();
                    else entity = entity.OrderByDescending(e => e.token).AsQueryable<grupo_empresa>();
                    break;
                case CAMPOS.FL_CARDSERVICES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.fl_cardservices).AsQueryable<grupo_empresa>();
                    else entity = entity.OrderByDescending(e => e.fl_cardservices).AsQueryable<grupo_empresa>();
                    break;
                case CAMPOS.FL_TAXSERVICES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.fl_taxservices).AsQueryable<grupo_empresa>();
                    else entity = entity.OrderByDescending(e => e.fl_taxservices).AsQueryable<grupo_empresa>();
                    break;
                case CAMPOS.FL_PROINFO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.fl_proinfo).AsQueryable<grupo_empresa>();
                    else entity = entity.OrderByDescending(e => e.fl_proinfo).AsQueryable<grupo_empresa>();
                    break;
                case CAMPOS.CDPRIORIDADE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdPrioridade).AsQueryable<grupo_empresa>();
                    else entity = entity.OrderByDescending(e => e.cdPrioridade).AsQueryable<grupo_empresa>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Grupo_empresa/Grupo_empresa
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
            try
            {
                // Se for uma consulta por um nome de grupo específico na coleção 0, não força filtro por empresa
                string outValue = null;
                Boolean FiltroNome = false;

                // Esse filtro só acontecerá para perfis que tem a funcionalidade FILTRO EMPRESA
                /*if (colecao == 0 && queryString.TryGetValue("" + (int)CAMPOS.DS_NOME, out outValue))
                    FiltroNome = !queryString["" + (int)CAMPOS.DS_NOME].Contains("%");*/

                // Só filtra o grupo do usuário logado se não for um filtro de nome
                Int32 IdGrupo = 0;
                if (!FiltroNome)
                {
                    IdGrupo = Permissoes.GetIdGrupo(token, _db);
                    if (IdGrupo != 0)
                    {
                        if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                            queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                        else
                            queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    }
                }

                //DECLARAÇÕES
                List<dynamic> CollectionGrupo_empresa = new List<dynamic>();
                Retorno retorno = new Retorno();

                // GET QUERY
                var query = getQuery(_db, colecao, campo, orderBy, pageSize, pageNumber, queryString);


                if (!FiltroNome)
                {
                    // Restringe consulta pelo perfil do usuário logado
                    //String RoleName = Permissoes.GetRoleName(token).ToUpper();
                    if (IdGrupo == 0 && Permissoes.isAtosRoleVendedor(token, _db))//RoleName.Equals("COMERCIAL"))
                    {
                        // Perfil Comercial tem uma carteira de clientes específica
                        List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(token, _db);
                        query = query.Where(e => listaIdsGruposEmpresas.Contains(e.id_grupo)).AsQueryable<grupo_empresa>();
                    }
                }

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
                    CollectionGrupo_empresa = query.Select(e => new
                    {
                        id_grupo = e.id_grupo,
                        ds_nome = e.ds_nome,
                        dt_cadastro = e.dt_cadastro,
                        token = e.token,
                        fl_ativo = e.fl_ativo,
                        fl_cardservices = e.fl_cardservices,
                        fl_taxservices = e.fl_taxservices,
                        fl_proinfo = e.fl_proinfo,
                        vendedor = e.id_vendedor != null ? new { e.Vendedor.id_users, e.Vendedor.ds_login, e.Vendedor.pessoa.nm_pessoa } : null,
                        cdPrioridade = e.cdPrioridade,
                        dsAPI = e.dsAPI,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionGrupo_empresa = query.Select(e => new
                    {
                        id_grupo = e.id_grupo,
                        ds_nome = e.ds_nome,
                        dt_cadastro = e.dt_cadastro,
                        token = e.token,
                        fl_ativo = e.fl_ativo,
                        fl_cardservices = e.fl_cardservices,
                        fl_taxservices = e.fl_taxservices,
                        fl_proinfo = e.fl_proinfo,
                        id_vendedor = e.id_vendedor,
                        cdPrioridade = e.cdPrioridade,
                        dsAPI = e.dsAPI,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionGrupo_empresa = query.Select(e => new
                    {
                        id_grupo = e.id_grupo,
                        ds_nome = e.ds_nome,
                        dt_cadastro = e.dt_cadastro,
                        token = e.token,
                        fl_ativo = e.fl_ativo,
                        fl_cardservices = e.fl_cardservices,
                        fl_taxservices = e.fl_taxservices,
                        fl_proinfo = e.fl_proinfo,
                        vendedor = e.id_vendedor != null ? new { e.Vendedor.id_users, e.Vendedor.ds_login, e.Vendedor.pessoa.nm_pessoa } : null,
                        login_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).OrderByDescending(l => l.dtAcesso).Take(1).Select(l => l.webpages_Users.ds_login).FirstOrDefault(),
                        dt_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).OrderByDescending(l => l.dtAcesso).Take(1).Select(l => l.dtAcesso).FirstOrDefault(),
                        podeExcluir = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).Count() == 0,
                        cdPrioridade = e.cdPrioridade,
                        dsAPI = e.dsAPI,
                    }

                    ).ToList<dynamic>();

                    // a diferença entre a colecao 2 e a 3 é que a 2 sempre ordena decrescente por dt ultimo acesso
                    // A coleção 2 é usada no mobile, já a 3 é usada no portal web
                    CollectionGrupo_empresa = CollectionGrupo_empresa.OrderByDescending(d => d.dt_ultimoAcesso).ToList();

                }
                else if (colecao == 3)
                {
                    //IQueryable iq = query.Select(e => new
                    //{
                    //    id_grupo = e.id_grupo,
                    //    ds_nome = e.ds_nome,
                    //    dt_cadastro = e.dt_cadastro,
                    //    token = e.token,
                    //    fl_ativo = e.fl_ativo,
                    //    fl_cardservices = e.fl_cardservices,
                    //    fl_taxservices = e.fl_taxservices,
                    //    fl_proinfo = e.fl_proinfo,
                    //    vendedor = e.id_vendedor != null ? new { e.Vendedor.id_users, e.Vendedor.ds_login, e.Vendedor.pessoa.nm_pessoa } : null,
                    //    //ultimoAcesso = (string)null,
                    //    ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).OrderByDescending(l => l.dtAcesso).Take(1)
                    //                                 .Select(l => new
                    //                                 {
                    //                                     login_ultimoAcesso = l.webpages_Users.ds_login,
                    //                                     dt_ultimoAcesso = l.dtAcesso,
                    //                                 }).FirstOrDefault(),
                    //    //login_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).OrderByDescending(l => l.dtAcesso).Select(l => l.webpages_Users.ds_login).Take(1).FirstOrDefault(),
                    //    //dt_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).OrderByDescending(l => l.dtAcesso).Select(l => l.dtAcesso).Take(1).FirstOrDefault(),
                    //    //podeExcluir = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).Count() == 0,
                    //    cdPrioridade = e.cdPrioridade,
                    //    dsAPI = e.dsAPI,
                    //});

                    CollectionGrupo_empresa = query.Select(e => new
                    {
                        id_grupo = e.id_grupo,
                        ds_nome = e.ds_nome,
                        dt_cadastro = e.dt_cadastro,
                        token = e.token,
                        fl_ativo = e.fl_ativo,
                        fl_cardservices = e.fl_cardservices,
                        fl_taxservices = e.fl_taxservices,
                        fl_proinfo = e.fl_proinfo,
                        vendedor = e.id_vendedor != null ? new { e.Vendedor.id_users, e.Vendedor.ds_login, e.Vendedor.pessoa.nm_pessoa } : null,
                        //ultimoAcesso = (string)null,
                        ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).OrderByDescending(l => l.dtAcesso).Take(1)
                                                     .Select(l => new
                                                     {
                                                         login_ultimoAcesso = l.webpages_Users.ds_login,
                                                         dt_ultimoAcesso = l.dtAcesso,
                                                     }).FirstOrDefault(),
                        //login_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).OrderByDescending(l => l.dtAcesso).Select(l => l.webpages_Users.ds_login).Take(1).FirstOrDefault(),
                        //dt_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).OrderByDescending(l => l.dtAcesso).Select(l => l.dtAcesso).Take(1).FirstOrDefault(),
                        //podeExcluir = _db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == e.id_grupo).Count() == 0,
                        cdPrioridade = e.cdPrioridade,
                        dsAPI = e.dsAPI,
                    }

                    ).ToList<dynamic>();

                }

                transaction.Commit();

                retorno.Registros = CollectionGrupo_empresa;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar grupo empresa" : erro);
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
        /// Adiciona nova Grupo_empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, grupo_empresa param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction(); // tudo ou nada
            try
            {
                param.id_grupo = -1;
                param.dt_cadastro = DateTime.Now;
                param.token = "null";
                param.fl_ativo = true;
                // Verificar se usuário logado é de perfil comercial
                if (Permissoes.isAtosRoleVendedor(token, _db))//Permissoes.isAtosRole(token) && Permissoes.GetRoleName(token).ToUpper().Equals("COMERCIAL"))
                    // Perfil Comercial tem uma carteira de clientes específica
                    param.id_vendedor = Permissoes.GetIdUser(token, _db);

                _db.grupo_empresa.Add(param);
                _db.SaveChanges();
                //transaction.Commit();
                return param.id_grupo;
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar grupo empresa" : erro);
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
        /// Apaga uma Grupo_empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id_grupo, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction(); // tudo ou nada
            try
            {
                if (_db.LogAcesso1.Where(l => l.webpages_Users.id_grupo == id_grupo).ToList().Count == 0)
                {
                    _db.grupo_empresa.Remove(_db.grupo_empresa.Where(e => e.id_grupo.Equals(id_grupo)).First());
                    _db.SaveChanges();
                    //transaction.Commit();
                }
                else
                    throw new Exception("Grupo empresa não pode ser deletado!");
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar grupo empresa" : erro);
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
        /// Altera grupo_empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, grupo_empresa param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction(); // tudo ou nada
            try
            {
                grupo_empresa value = _db.grupo_empresa
                        .Where(e => e.id_grupo.Equals(param.id_grupo))
                        .First<grupo_empresa>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                //if (param.id_grupo != null && param.id_grupo != value.id_grupo)
                //    value.id_grupo = param.id_grupo;
                if (param.ds_nome != null && param.ds_nome != value.ds_nome)
                    value.ds_nome = param.ds_nome;
                //if (param.token != null && param.token != value.token)
                //    value.token = param.token;
                if (param.fl_ativo != value.fl_ativo)
                {
                    value.fl_ativo = param.fl_ativo;
                }
                if (param.fl_cardservices != value.fl_cardservices)
                    value.fl_cardservices = param.fl_cardservices;
                if (param.fl_taxservices != value.fl_taxservices)
                    value.fl_taxservices = param.fl_taxservices;
                if (param.fl_proinfo != value.fl_proinfo)
                    value.fl_proinfo = param.fl_proinfo;
                if (param.dsAPI != null && param.dsAPI != value.dsAPI)
                    value.dsAPI = param.dsAPI;
                //if (param.cdPrioridade != value.cdPrioridade) // não faz alterações pela API!
                //    value.cdPrioridade = param.cdPrioridade;
                _db.SaveChanges();
                //transaction.Commit();
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar grupo empresa" : erro);
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
