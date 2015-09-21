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

namespace api.Negocios.Admin
{
    public class GatewayTbDispositivoUsuario
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbDispositivoUsuario()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDDISPOSITIVOUSUARIO = 100,
            IDUSER = 101,
            DSPLATAFORMA = 102,
            DSMODELO = 103,
            DSVERSAO = 104,
            IDIONIC = 105,
            IDUSERIONIC = 106,
            CDTOKENVALIDO = 107,
            TMLARGURA = 108,
            TMALTURA = 109,

            IDNEWS = 700,            
        };

        /// <summary>
        /// Get TbDispositivoUsuario/TbDispositivoUsuario
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbDispositivoUsuario> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbDispositivoUsuarios.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDDISPOSITIVOUSUARIO:
                        Int32 idDispositivoUsuario = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idDispositivoUsuario.Equals(idDispositivoUsuario)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.IDUSER:
                        Int32 idUser = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idUser.Equals(idUser)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.DSPLATAFORMA:
                        string dsPlataforma = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsPlataforma.Equals(dsPlataforma)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.DSMODELO:
                        string dsModelo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsModelo.Equals(dsModelo)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.DSVERSAO:
                        string dsVersao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsVersao.Equals(dsVersao)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.IDIONIC:
                        string idIONIC = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.idIONIC.Equals(idIONIC)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.IDUSERIONIC:
                        string idUserIONIC = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.idUserIONIC.Equals(idUserIONIC)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.CDTOKENVALIDO:
                        string cdTokenValido = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdTokenValido.Equals(cdTokenValido)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.TMLARGURA:
                        short tmLargura = short.Parse(item.Value);
                        entity = entity.Where(e => e.tmLargura.Equals(tmLargura)).AsQueryable<tbDispositivoUsuario>();
                        break;
                    case CAMPOS.TMALTURA:
                        short tmAltura = short.Parse(item.Value);
                        entity = entity.Where(e => e.tmAltura.Equals(tmAltura)).AsQueryable<tbDispositivoUsuario>();
                        break;


                    // RELACIONAMENTOS
                    case CAMPOS.IDNEWS:
                        Int32 idNews = Convert.ToInt32(item.Value);
                        tbNews noticia = _db.tbNewss.Where(n => n.idNews == idNews).FirstOrDefault();
                        if (noticia == null)
                            continue;
                        List<int> ids = iMessenger.getIdUsersFromNews(noticia);
                        entity = entity.Where(e => ids.Contains(e.idUser)).OrderByDescending(e => e.idDispositivoUsuario).Take(1).AsQueryable<tbDispositivoUsuario>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDDISPOSITIVOUSUARIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idDispositivoUsuario).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idDispositivoUsuario).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.IDUSER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idUser).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idUser).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.DSPLATAFORMA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsPlataforma).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsPlataforma).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.DSMODELO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsModelo).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsModelo).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.DSVERSAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsVersao).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsVersao).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.IDIONIC:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idIONIC).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idIONIC).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.IDUSERIONIC:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idUserIONIC).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idUserIONIC).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.CDTOKENVALIDO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTokenValido).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.cdTokenValido).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.TMLARGURA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tmLargura).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.tmLargura).AsQueryable<tbDispositivoUsuario>();
                    break;
                case CAMPOS.TMALTURA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tmAltura).AsQueryable<tbDispositivoUsuario>();
                    else entity = entity.OrderByDescending(e => e.tmAltura).AsQueryable<tbDispositivoUsuario>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbDispositivoUsuario/TbDispositivoUsuario
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbDispositivoUsuario = new List<dynamic>();
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
                    CollectionTbDispositivoUsuario = query.Select(e => new
                    {

                        idDispositivoUsuario = e.idDispositivoUsuario,
                        idUser = e.idUser,
                        dsPlataforma = e.dsPlataforma,
                        dsModelo = e.dsModelo,
                        dsVersao = e.dsVersao,
                        idIONIC = e.idIONIC,
                        idUserIONIC = e.idUserIONIC,
                        cdTokenValido = e.cdTokenValido,
                        tmLargura = e.tmLargura,
                        tmAltura = e.tmAltura,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbDispositivoUsuario = query.Select(e => new
                    {

                        idDispositivoUsuario = e.idDispositivoUsuario,
                        idUser = e.idUser,
                        dsPlataforma = e.dsPlataforma,
                        dsModelo = e.dsModelo,
                        dsVersao = e.dsVersao,
                        idIONIC = e.idIONIC,
                        idUserIONIC = e.idUserIONIC,
                        cdTokenValido = e.cdTokenValido,
                        tmLargura = e.tmLargura,
                        tmAltura = e.tmAltura,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbDispositivoUsuario;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbDispositivoUsuario" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbDispositivoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, IonicWebHook param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                tbDispositivoUsuario disUser = new tbDispositivoUsuario();

                if (param.usuario == null)
                {
                    return 1;
                }
                else
                {
                    disUser.dsPlataforma = param.device.platform;
                    disUser.dsModelo = param.device.model;
                    disUser.dsVersao = param.device.version;
                    disUser.tmLargura = param.device.screen_width;
                    disUser.tmAltura = param.device.screen_height;
                    disUser.idIONIC = param.device.uuid;
                    disUser.idUserIONIC = param.user_id;
                    if (param._push.android_tokens.Count > 0)
                        disUser.cdTokenValido = param._push.android_tokens.First();
                    else if (param._push.ios_tokens.Count > 0)
                        disUser.cdTokenValido = param._push.ios_tokens.First();
                    else
                        disUser.cdTokenValido = null;

                    disUser.idUser = Permissoes.GetIdUserPeloLogin(param.usuario);
                    _db.tbDispositivoUsuarios.Add(disUser);
                    _db.SaveChanges();
                    return 1;
                }
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbDispositivoUsuario" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbDispositivoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idDispositivoUsuario)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbDispositivoUsuarios.Remove(_db.tbDispositivoUsuarios.Where(e => e.idDispositivoUsuario.Equals(idDispositivoUsuario)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbDispositivoUsuario" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbDispositivoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbDispositivoUsuario param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbDispositivoUsuario value = _db.tbDispositivoUsuarios
                        .Where(e => e.idDispositivoUsuario.Equals(param.idDispositivoUsuario))
                        .First<tbDispositivoUsuario>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idDispositivoUsuario != null && param.idDispositivoUsuario != value.idDispositivoUsuario)
                    value.idDispositivoUsuario = param.idDispositivoUsuario;
                if (param.idUser != null && param.idUser != value.idUser)
                    value.idUser = param.idUser;
                if (param.dsPlataforma != null && param.dsPlataforma != value.dsPlataforma)
                    value.dsPlataforma = param.dsPlataforma;
                if (param.dsModelo != null && param.dsModelo != value.dsModelo)
                    value.dsModelo = param.dsModelo;
                if (param.dsVersao != null && param.dsVersao != value.dsVersao)
                    value.dsVersao = param.dsVersao;
                if (param.idIONIC != null && param.idIONIC != value.idIONIC)
                    value.idIONIC = param.idIONIC;
                if (param.idUserIONIC != null && param.idUserIONIC != value.idUserIONIC)
                    value.idUserIONIC = param.idUserIONIC;
                if (param.cdTokenValido != null && param.cdTokenValido != value.cdTokenValido)
                    value.cdTokenValido = param.cdTokenValido;
                if (param.tmLargura != null && param.tmLargura != value.tmLargura)
                    value.tmLargura = param.tmLargura;
                if (param.tmAltura != null && param.tmAltura != value.tmAltura)
                    value.tmAltura = param.tmAltura;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbDispositivoUsuario" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}