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

namespace api.Negocios.Administracao
{
    public class GatewayWebpagesPermissions
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesPermissions()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID_ROLES = 100,
            ID_METHOD = 101,
            FL_PRINCIPAL = 102,

            // PERSONALIZADO
            ID_CONTROLLER = 200,

        };

        /// <summary>
        /// Get Webpages_Permissions/Webpages_Permissions
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_Permissions> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_Permissions.AsQueryable<webpages_Permissions>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.ID_ROLES:
                        Int32 id_roles = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_roles.Equals(id_roles)).AsQueryable<webpages_Permissions>();
                        break;
                    case CAMPOS.ID_METHOD:
                        Int32 id_method = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_method.Equals(id_method)).AsQueryable<webpages_Permissions>();
                        break;
                    case CAMPOS.FL_PRINCIPAL:
                        Boolean fl_principal = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.fl_principal.Equals(fl_principal)).AsQueryable<webpages_Permissions>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.ID_ROLES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_roles).AsQueryable<webpages_Permissions>();
                    else entity = entity.OrderByDescending(e => e.id_roles).AsQueryable<webpages_Permissions>();
                    break;
                case CAMPOS.ID_METHOD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_method).AsQueryable<webpages_Permissions>();
                    else entity = entity.OrderByDescending(e => e.id_method).AsQueryable<webpages_Permissions>();
                    break;
                case CAMPOS.FL_PRINCIPAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.fl_principal).AsQueryable<webpages_Permissions>();
                    else entity = entity.OrderByDescending(e => e.fl_principal).AsQueryable<webpages_Permissions>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_Permissions/Webpages_Permissions
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
                List<dynamic> CollectionWebpages_Permissions = new List<dynamic>();
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
                    CollectionWebpages_Permissions = query.Select(e => new
                    {

                        id_roles = e.id_roles,
                        id_method = e.id_method,
                        fl_principal = e.fl_principal,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionWebpages_Permissions = query.Select(e => new
                    {

                        id_roles = e.id_roles,
                        id_method = e.id_method,
                        fl_principal = e.fl_principal,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    // Retorna os métodos permitidos para o usuário logado em relação ao controller informado
                    Int32 userId = Permissoes.GetIdUser(token);
                    Int32 controllerId = Convert.ToInt32(queryString[((int)CAMPOS.ID_CONTROLLER).ToString()]);


                    var permissoes = _db.webpages_UsersInRoles
                                                .Where(r => r.UserId == userId)
                                                .Where(r => r.RoleId > 50)
                                                .Select(r => new
                                                        {
                                                            metodos = _db.webpages_Permissions
                                                                                .Where(p => p.id_roles == r.RoleId)
                                                                                .Where(p => p.webpages_Methods.webpages_Controllers.id_controller == controllerId)
                                                                                .Select(p => new
                                                                                {
                                                                                    id_method = p.webpages_Methods.id_method,
                                                                                    ds_method = p.webpages_Methods.ds_method,
                                                                                    nm_method = p.webpages_Methods.nm_method,
                                                                                    id_controller = p.webpages_Methods.id_controller
                                                                                }
                                                                                ).ToList<dynamic>(),
                                                        }
                                                    ).FirstOrDefault();

                    if (permissoes != null)
                    {
                        CollectionWebpages_Permissions = permissoes.metodos;
                    }

                    retorno.TotalDeRegistros = CollectionWebpages_Permissions.Count;
                }

                transaction.Commit();

                retorno.Registros = CollectionWebpages_Permissions;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar permissões" : erro);
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
        /// Adiciona nova Webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, webpages_Permissions param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            try
            {
                _db.webpages_Permissions.Add(param);
                _db.SaveChanges();
                return param.id_roles;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar permissões" : erro);
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
        /// Apaga uma Webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id_roles, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            try
            {
                _db.webpages_Permissions.RemoveRange(
                                                    _db.webpages_Permissions.Where(e => e.id_roles == id_roles)
                                                );
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar permissões" : erro);
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
        /// Apaga uma Webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id_roles, Int32 id_method, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            try
            {
                _db.webpages_Permissions.RemoveRange(
                                                        _db.webpages_Permissions.Where(e => e.id_roles.Equals(id_roles)).Where(e => e.id_method.Equals(id_method))
                                                    );
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar permissões" : erro);
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
        /// Apaga uma Webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void DeleteMethod(string token, Int32 id_method, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            try
            {
                _db.webpages_Permissions.RemoveRange(_db.webpages_Permissions.Where(e => e.id_method == id_method));
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar permissões" : erro);
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
        /// Altera webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, webpages_Permissions param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            try
            {
                webpages_Permissions value = _db.webpages_Permissions
                        .Where(e => e.id_roles.Equals(param.id_roles))
                        .Where(e => e.id_method.Equals(param.id_method))
                        .First<webpages_Permissions>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS



                if (param.fl_principal != value.fl_principal)
                    value.fl_principal = param.fl_principal;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar permissões" : erro);
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
        /// Altera webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Models.Object.RolesPermissions param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            try
            {
                if ((param.Inserir != null) && (param.Inserir.Count > 0))
                {
                    foreach (var item in param.Inserir)
                    {
                        _db.webpages_Permissions.Add(new webpages_Permissions { id_roles = param.Id_roles, id_method = (int)item });
                    }
                }

                if ((param.Deletar != null) && (param.Deletar.Count > 0))
                {
                    foreach (var item in param.Deletar)
                    {
                        _db.webpages_Permissions.Remove(
                                                            _db.webpages_Permissions
                                                            .Where(e => e.id_roles == param.Id_roles)
                                                            .Where(e => e.id_method == item).First()
                                                );
                    }
                }
                _db.SaveChanges();
                // set o controller principal
                if (param.Id_controller_principal != null)
                {
                    // procura por um possível controller principal
                    List<webpages_Permissions> permissoes = _db.webpages_Permissions
                        .Where(p => p.id_roles == param.Id_roles)
                        .Where(p => p.fl_principal == true).ToList<webpages_Permissions>();
                    foreach (var permissao in permissoes)
                    {
                        permissao.fl_principal = false;
                        Update(token, permissao);
                    }

                    // set os metodos do controller para ser o principal
                    permissoes = _db.webpages_Permissions
                        .Where(p => p.id_roles == param.Id_roles)
                        .Where(p => p.webpages_Methods.id_controller == param.Id_controller_principal).ToList<webpages_Permissions>();
                    foreach (var permissao in permissoes)
                    {
                        permissao.fl_principal = true;
                        Update(token, permissao);
                    }
                }


                // set o controller principal
                if (param.Id_controller_principal != null)
                {
                    // procura por um possível controller principal
                    List<webpages_Permissions> permissoes = _db.webpages_Permissions
                        .Where(p => p.id_roles == param.Id_roles)
                        .Where(p => p.fl_principal == true).ToList<webpages_Permissions>();
                    foreach (var permissao in permissoes)
                    {
                        permissao.fl_principal = false;
                        Update(token, permissao);
                    }

                    // set os metodos do controller para ser o principal
                    permissoes = _db.webpages_Permissions
                        .Where(p => p.id_roles == param.Id_roles)
                        .Where(p => p.webpages_Methods.id_controller == param.Id_controller_principal).ToList<webpages_Permissions>();
                    foreach (var permissao in permissoes)
                    {
                        permissao.fl_principal = true;
                        Update(token, permissao);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar pessoa" : erro);
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
