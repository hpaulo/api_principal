using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Admin
{
    public class GatewayTbLogAcessoUsuario
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbLogAcessoUsuario()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDLOGACESSOUSUARIO = 100,
            IDUSER = 101,
            DSURL = 102,
            IDCONTROLLER = 103,
            IDMETHOD = 104,
            DSPARAMETROS = 105,
            DSFILTROS = 106,
            DTACESSO = 107,
            DSAPLICACAO = 108,
            CODRESPOSTA = 109,
            MSGERRO = 110,
            DSJSON = 111,

        };

        /// <summary>
        /// Get TbLogAcessoUsuario/TbLogAcessoUsuario
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbLogAcessoUsuario> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbLogAcessoUsuarios.AsQueryable<tbLogAcessoUsuario>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDLOGACESSOUSUARIO:
                        Int32 idLogAcessoUsuario = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogAcessoUsuario.Equals(idLogAcessoUsuario)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.IDUSER:
                        Int32 idUser = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idUser.Equals(idUser)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSURL:
                        string dsUrl = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsUrl.Equals(dsUrl)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.IDCONTROLLER:
                        Int32 idController = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idController.Equals(idController)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.IDMETHOD:
                        Int32 idMethod = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idMethod.Equals(idMethod)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSPARAMETROS:
                        string dsParametros = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsParametros.Equals(dsParametros)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSFILTROS:
                        string dsFiltros = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsFiltros.Equals(dsFiltros)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DTACESSO:
                        DateTime dtAcesso = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtAcesso.Equals(dtAcesso)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSAPLICACAO:
                        string dsAplicacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsAplicacao.Equals(dsAplicacao)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.CODRESPOSTA:
                        Int32 codResposta = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.codResposta.Equals(codResposta)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.MSGERRO:
                        string msgErro = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.msgErro.Equals(msgErro)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSJSON:
                        string dsJson = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsJson.Equals(dsJson)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDLOGACESSOUSUARIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogAcessoUsuario).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idLogAcessoUsuario).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.IDUSER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idUser).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idUser).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSURL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsUrl).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsUrl).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.IDCONTROLLER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idController).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idController).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.IDMETHOD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idMethod).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idMethod).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSPARAMETROS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsParametros).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsParametros).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSFILTROS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsFiltros).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsFiltros).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DTACESSO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSAPLICACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsAplicacao).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsAplicacao).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.CODRESPOSTA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codResposta).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.codResposta).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.MSGERRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.msgErro).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.msgErro).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSJSON:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsJson).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsJson).AsQueryable<tbLogAcessoUsuario>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbLogAcessoUsuario/TbLogAcessoUsuario
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbLogAcessoUsuario = new List<dynamic>();
            Retorno retorno = new Retorno();

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
                CollectionTbLogAcessoUsuario = query.Select(e => new
                {

                    idLogAcessoUsuario = e.idLogAcessoUsuario,
                    idUser = e.idUser,
                    dsUrl = e.dsUrl,
                    idController = e.idController,
                    idMethod = e.idMethod,
                    dsParametros = e.dsParametros,
                    dsFiltros = e.dsFiltros,
                    dtAcesso = e.dtAcesso,
                    dsAplicacao = e.dsAplicacao,
                    codResposta = e.codResposta,
                    msgErro = e.msgErro,
                    dsJson = e.dsJson,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbLogAcessoUsuario = query.Select(e => new
                {

                    idLogAcessoUsuario = e.idLogAcessoUsuario,
                    idUser = e.idUser,
                    dsUrl = e.dsUrl,
                    idController = e.idController,
                    idMethod = e.idMethod,
                    dsParametros = e.dsParametros,
                    dsFiltros = e.dsFiltros,
                    dtAcesso = e.dtAcesso,
                    dsAplicacao = e.dsAplicacao,
                    codResposta = e.codResposta,
                    msgErro = e.msgErro,
                    dsJson = e.dsJson,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbLogAcessoUsuario;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbLogAcessoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbLogAcessoUsuario param)
        {
            _db.tbLogAcessoUsuarios.Add(param);
            _db.SaveChanges();
            return param.idLogAcessoUsuario;
        }


        /// <summary>
        /// Apaga uma TbLogAcessoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idLogAcessoUsuario)
        {
            _db.tbLogAcessoUsuarios.Remove(_db.tbLogAcessoUsuarios.Where(e => e.idLogAcessoUsuario.Equals(idLogAcessoUsuario)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbLogAcessoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbLogAcessoUsuario param)
        {
            tbLogAcessoUsuario value = _db.tbLogAcessoUsuarios
                    .Where(e => e.idLogAcessoUsuario.Equals(param.idLogAcessoUsuario))
                    .First<tbLogAcessoUsuario>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.idLogAcessoUsuario != null && param.idLogAcessoUsuario != value.idLogAcessoUsuario)
                value.idLogAcessoUsuario = param.idLogAcessoUsuario;
            if (param.idUser != null && param.idUser != value.idUser)
                value.idUser = param.idUser;
            if (param.dsUrl != null && param.dsUrl != value.dsUrl)
                value.dsUrl = param.dsUrl;
            if (param.idController != null && param.idController != value.idController)
                value.idController = param.idController;
            if (param.idMethod != null && param.idMethod != value.idMethod)
                value.idMethod = param.idMethod;
            if (param.dsParametros != null && param.dsParametros != value.dsParametros)
                value.dsParametros = param.dsParametros;
            if (param.dsFiltros != null && param.dsFiltros != value.dsFiltros)
                value.dsFiltros = param.dsFiltros;
            if (param.dtAcesso != null && param.dtAcesso != value.dtAcesso)
                value.dtAcesso = param.dtAcesso;
            if (param.dsAplicacao != null && param.dsAplicacao != value.dsAplicacao)
                value.dsAplicacao = param.dsAplicacao;
            if (param.codResposta != null && param.codResposta != value.codResposta)
                value.codResposta = param.codResposta;
            if (param.msgErro != null && param.msgErro != value.msgErro)
                value.msgErro = param.msgErro;
            if (param.dsJson != null && param.dsJson != value.dsJson)
                value.dsJson = param.dsJson;
            _db.SaveChanges();

        }

    }
}
