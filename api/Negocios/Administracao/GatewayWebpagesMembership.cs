using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using WebMatrix.WebData;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace api.Negocios.Administracao
{
    public class GatewayWebpagesMembership
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesMembership()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            USERID = 100,
            CREATEDATE = 101,
            CONFIRMATIONTOKEN = 102,
            ISCONFIRMED = 103,
            LASTPASSWORDFAILUREDATE = 104,
            PASSWORDFAILURESSINCELASTSUCCESS = 105,
            PASSWORD = 106,
            PASSWORDCHANGEDDATE = 107,
            PASSWORDSALT = 108,
            PASSWORDVERIFICATIONTOKEN = 109,
            PASSWORDVERIFICATIONTOKENEXPIRATIONDATE = 110,

        };

        /// <summary>
        /// Get Webpages_Membership/Webpages_Membership
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_Membership> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_Membership.AsQueryable<webpages_Membership>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.USERID:
                        Int32 UserId = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.UserId.Equals(UserId)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.CREATEDATE:
                        DateTime CreateDate = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.CreateDate.Equals(CreateDate)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.CONFIRMATIONTOKEN:
                        string ConfirmationToken = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ConfirmationToken.Equals(ConfirmationToken)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.ISCONFIRMED:
                        Boolean IsConfirmed = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.IsConfirmed.Equals(IsConfirmed)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.LASTPASSWORDFAILUREDATE:
                        DateTime LastPasswordFailureDate = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.LastPasswordFailureDate.Equals(LastPasswordFailureDate)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.PASSWORDFAILURESSINCELASTSUCCESS:
                        Int32 PasswordFailuresSinceLastSuccess = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.PasswordFailuresSinceLastSuccess.Equals(PasswordFailuresSinceLastSuccess)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.PASSWORD:
                        string Password = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.Password.Equals(Password)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.PASSWORDCHANGEDDATE:
                        DateTime PasswordChangedDate = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.PasswordChangedDate.Equals(PasswordChangedDate)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.PASSWORDSALT:
                        string PasswordSalt = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.PasswordSalt.Equals(PasswordSalt)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.PASSWORDVERIFICATIONTOKEN:
                        string PasswordVerificationToken = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.PasswordVerificationToken.Equals(PasswordVerificationToken)).AsQueryable<webpages_Membership>();
                        break;
                    case CAMPOS.PASSWORDVERIFICATIONTOKENEXPIRATIONDATE:
                        DateTime PasswordVerificationTokenExpirationDate = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.PasswordVerificationTokenExpirationDate.Equals(PasswordVerificationTokenExpirationDate)).AsQueryable<webpages_Membership>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.USERID:
                    if (orderby == 0) entity = entity.OrderBy(e => e.UserId).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.UserId).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.CREATEDATE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.CreateDate).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.CreateDate).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.CONFIRMATIONTOKEN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ConfirmationToken).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.ConfirmationToken).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.ISCONFIRMED:
                    if (orderby == 0) entity = entity.OrderBy(e => e.IsConfirmed).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.IsConfirmed).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.LASTPASSWORDFAILUREDATE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.LastPasswordFailureDate).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.LastPasswordFailureDate).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.PASSWORDFAILURESSINCELASTSUCCESS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.PasswordFailuresSinceLastSuccess).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.PasswordFailuresSinceLastSuccess).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.PASSWORD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Password).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.Password).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.PASSWORDCHANGEDDATE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.PasswordChangedDate).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.PasswordChangedDate).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.PASSWORDSALT:
                    if (orderby == 0) entity = entity.OrderBy(e => e.PasswordSalt).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.PasswordSalt).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.PASSWORDVERIFICATIONTOKEN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.PasswordVerificationToken).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.PasswordVerificationToken).AsQueryable<webpages_Membership>();
                    break;
                case CAMPOS.PASSWORDVERIFICATIONTOKENEXPIRATIONDATE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.PasswordVerificationTokenExpirationDate).AsQueryable<webpages_Membership>();
                    else entity = entity.OrderByDescending(e => e.PasswordVerificationTokenExpirationDate).AsQueryable<webpages_Membership>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_Membership/Webpages_Membership
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
                //DECLARAÇÕES
                List<dynamic> CollectionWebpages_Membership = new List<dynamic>();
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
                    CollectionWebpages_Membership = query.Select(e => new
                    {

                        UserId = e.UserId,
                        CreateDate = e.CreateDate,
                        ConfirmationToken = e.ConfirmationToken,
                        IsConfirmed = e.IsConfirmed,
                        LastPasswordFailureDate = e.LastPasswordFailureDate,
                        PasswordFailuresSinceLastSuccess = e.PasswordFailuresSinceLastSuccess,
                        Password = e.Password,
                        PasswordChangedDate = e.PasswordChangedDate,
                        PasswordSalt = e.PasswordSalt,
                        PasswordVerificationToken = e.PasswordVerificationToken,
                        PasswordVerificationTokenExpirationDate = e.PasswordVerificationTokenExpirationDate,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionWebpages_Membership = query.Select(e => new
                    {

                        UserId = e.UserId,
                        CreateDate = e.CreateDate,
                        ConfirmationToken = e.ConfirmationToken,
                        IsConfirmed = e.IsConfirmed,
                        LastPasswordFailureDate = e.LastPasswordFailureDate,
                        PasswordFailuresSinceLastSuccess = e.PasswordFailuresSinceLastSuccess,
                        Password = e.Password,
                        PasswordChangedDate = e.PasswordChangedDate,
                        PasswordSalt = e.PasswordSalt,
                        PasswordVerificationToken = e.PasswordVerificationToken,
                        PasswordVerificationTokenExpirationDate = e.PasswordVerificationTokenExpirationDate,
                    }).ToList<dynamic>();
                }

                transaction.Commit();

                retorno.Registros = CollectionWebpages_Membership;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar membership" : erro);
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
        /// Adiciona nova Webpages_Membership
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, webpages_Membership param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                _db.webpages_Membership.Add(param);
                _db.SaveChanges();
                return param.UserId;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao adicionar membership" : erro);
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
        /// Apaga uma Webpages_Membership
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 UserId, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                _db.webpages_Membership.RemoveRange(_db.webpages_Membership.Where(e => e.UserId == UserId));
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar membership" : erro);
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
        /// Altera webpages_Membership
        /// Reseta Senha de usuário
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// 
        public static void Update(string token, webpages_Membership param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                var value = _db.webpages_Users
                        .Where(e => e.id_users.Equals(param.UserId))
                        .FirstOrDefault();


                string resetToken = WebSecurity.GeneratePasswordResetToken(value.ds_login, 2);
                if (param.Password == "")
                    WebSecurity.ResetPassword(resetToken, "atos123");
                else
                    WebSecurity.ResetPassword(resetToken, param.Password);
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar membership" : erro);
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
        /// Altera webpages_Membership
        /// Alterar senha do usuário logado
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Models.Object.AlterarSenha param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                Int32 idUser = 0;

                if (param.UserId == -1)
                    idUser = Permissoes.GetIdUser(token);
                else
                    idUser = param.UserId;

                var value = _db.webpages_Users
                        .Where(e => e.id_users.Equals(idUser))
                        .FirstOrDefault();

                if (value != null)
                {
                    string resetToken = WebSecurity.GeneratePasswordResetToken(value.ds_login, 2);
                    if (param.NovaSenha == "")
                        WebSecurity.ResetPassword(resetToken, "atos123");
                    else if (param.SenhaAtual != null)
                    {
                        if (WebSecurity.Login(value.ds_login, param.SenhaAtual, persistCookie: false))
                            WebSecurity.ResetPassword(resetToken, param.NovaSenha);
                        else
                            throw new Exception("Senha inválida!");
                    }
                    else
                        throw new Exception("Operação inválida!");
                }
                else
                    throw new Exception("Usuário inválido!");
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar membership" : erro);
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
