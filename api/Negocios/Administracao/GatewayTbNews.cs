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
    public class GatewayTbNews
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbNews()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDNEWS = 100,
            DSNEWS = 101,
            DTNEWS = 102,
            CDEMPRESAGRUPO = 103,
            CDCATALOGO = 104,
            CDCANAL = 105,
            dsReporter = 106,
            DTENVIO = 107,

            // PERSONALIZADO
            ID_USERS = 201,
            FLLIDO = 203
        };

        /// <summary>
        /// Get TbNews/TbNews
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbNews> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbNewss.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDNEWS:
                        Int32 idNews = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idNews.Equals(idNews)).AsQueryable<tbNews>();
                        break;
                    case CAMPOS.DSNEWS:
                        string dsNews = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsNews.Equals(dsNews)).AsQueryable<tbNews>();
                        break;
                    case CAMPOS.DTNEWS:
                        DateTime dtNews = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtNews.Equals(dtNews)).AsQueryable<tbNews>();
                        break;
                    case CAMPOS.CDEMPRESAGRUPO:
                        Int32 cdEmpresaGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdEmpresaGrupo.Equals(cdEmpresaGrupo)).AsQueryable<tbNews>();
                        break;
                    case CAMPOS.CDCATALOGO:
                        short cdCatalogo = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdCatalogo.Equals(cdCatalogo)).AsQueryable<tbNews>();
                        break;
                    case CAMPOS.CDCANAL:
                        short cdCanal = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdCanal.Equals(cdCanal)).AsQueryable<tbNews>();
                        break;
                    case CAMPOS.dsReporter:
                        string dsReporter = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsReporter.Equals(dsReporter)).AsQueryable<tbNews>();
                        break;
                    case CAMPOS.DTENVIO:
                        DateTime dtEnvio = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtEnvio.Equals(dtEnvio)).AsQueryable<tbNews>();
                        break;


                    // PERSONALIZADO
                    case CAMPOS.ID_USERS:
                        Int32 id_users = Convert.ToInt32(item.Value);
						entity = entity.Where(e => e.tbNewsStatus.Where(n => n.id_users == id_users).Count() > 0).AsQueryable<tbNews>();
					    break;

                    case CAMPOS.FLLIDO:
                        Boolean flLido = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.tbNewsStatus.Where(n => n.flLido == flLido).Count() > 0).AsQueryable<tbNews>();
                        break;


                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDNEWS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idNews).AsQueryable<tbNews>();
                    else entity = entity.OrderByDescending(e => e.idNews).AsQueryable<tbNews>();
                    break;
                case CAMPOS.DSNEWS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsNews).AsQueryable<tbNews>();
                    else entity = entity.OrderByDescending(e => e.dsNews).AsQueryable<tbNews>();
                    break;
                case CAMPOS.DTNEWS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtNews).AsQueryable<tbNews>();
                    else entity = entity.OrderByDescending(e => e.dtNews).AsQueryable<tbNews>();
                    break;
                case CAMPOS.CDEMPRESAGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEmpresaGrupo).AsQueryable<tbNews>();
                    else entity = entity.OrderByDescending(e => e.cdEmpresaGrupo).AsQueryable<tbNews>();
                    break;
                case CAMPOS.CDCATALOGO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdCatalogo).AsQueryable<tbNews>();
                    else entity = entity.OrderByDescending(e => e.cdCatalogo).AsQueryable<tbNews>();
                    break;
                case CAMPOS.CDCANAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdCanal).AsQueryable<tbNews>();
                    else entity = entity.OrderByDescending(e => e.cdCanal).AsQueryable<tbNews>();
                    break;
                case CAMPOS.dsReporter:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsReporter).AsQueryable<tbNews>();
                    else entity = entity.OrderByDescending(e => e.dsReporter).AsQueryable<tbNews>();
                    break;
                case CAMPOS.DTENVIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtEnvio).AsQueryable<tbNews>();
                    else entity = entity.OrderByDescending(e => e.dtEnvio).AsQueryable<tbNews>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbNews/TbNews
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbNews = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Atualiza o contexto
                //((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                string outValue = null;
                Int32 idUsers = Permissoes.GetIdUser(token);
                //if(Permissoes.isAtosRole(token))
                if (queryString.TryGetValue("" + (int)CAMPOS.ID_USERS, out outValue))
                    queryString["" + (int)CAMPOS.ID_USERS] = idUsers.ToString();
                else if(idUsers != 330) // Força o usuário IMESSENGER a acesso a todos os registros
                    queryString.Add("" + (int)CAMPOS.ID_USERS, idUsers.ToString());

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

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
                    CollectionTbNews = query.Select(e => new
                    {

                        idNews = e.idNews,
                        dsNews = e.dsNews,
                        dtNews = e.dtNews,
                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                        cdCatalogo = e.cdCatalogo,
                        cdCanal = e.cdCanal,
                        dsReporter = e.dsReporter,
                        dtEnvio = e.dtEnvio,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbNews = query.Select(e => new
                    {

                        idNews = e.idNews,
                        dsNews = e.dsNews,
                        dtNews = e.dtNews,
                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                        cdCatalogo = e.cdCatalogo,
                        cdCanal = e.cdCanal,
                        dsReporter = e.dsReporter,
                        dtEnvio = e.dtEnvio,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionTbNews = query.Select(e => new
                    {

                        idNews = e.idNews,
                        dsNews = e.dsNews,
                        dtNews = e.dtNews,
                        //cdEmpresaGrupo = e.cdEmpresaGrupo,
                        //cdCatalogo = e.cdCatalogo,
                        //cdCanal = e.cdCanal,
                        //dsReporter = e.dsReporter,
                        //dtEnvio = e.dtEnvio,
                        flLido = e.tbNewsStatus.Where(s => s.id_users == idUsers).Select(s => s.flLido).FirstOrDefault()
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbNews;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbNews" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbNews
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbNews param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbNewss.Add(param);
                _db.SaveChanges();
                return param.idNews;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbNews" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbNews
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idNews)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbNewss.Remove(_db.tbNewss.Where(e => e.idNews.Equals(idNews)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbNews" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbNews
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbNews param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbNews value = _db.tbNewss
                        .Where(e => e.idNews.Equals(param.idNews))
                        .First<tbNews>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idNews != null && param.idNews != value.idNews)
                    value.idNews = param.idNews;
                if (param.dsNews != null && param.dsNews != value.dsNews)
                    value.dsNews = param.dsNews;
                if (param.dtNews != null && param.dtNews != value.dtNews)
                    value.dtNews = param.dtNews;
                if (param.cdEmpresaGrupo != null && param.cdEmpresaGrupo != value.cdEmpresaGrupo)
                    value.cdEmpresaGrupo = param.cdEmpresaGrupo;
                if (param.cdCatalogo != null && param.cdCatalogo != value.cdCatalogo)
                    value.cdCatalogo = param.cdCatalogo;
                if (param.cdCanal != null && param.cdCanal != value.cdCanal)
                    value.cdCanal = param.cdCanal;
                if (param.dsReporter != null && param.dsReporter != value.dsReporter)
                    value.dsReporter = param.dsReporter;
                if (param.dtEnvio != null && param.dtEnvio != value.dtEnvio)
                    value.dtEnvio = param.dtEnvio;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbNews" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}