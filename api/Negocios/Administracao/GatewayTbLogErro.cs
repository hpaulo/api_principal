using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Admin
{
    public class GatewayTbLogErro
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbLogErro()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDLOGERRO = 100,
            DSAPLICACAO = 101,
            DSVERSAO = 102,
            DTERRO = 103,
            DSNOMECOMPUTADOR = 104,
            DSNOMEUSUARIO = 105,
            DSVERSAOSO = 106,
            DSCULTURA = 107,
            DSMENSAGEM = 108,
            DSSTACKTRACE = 109,
            DSXMLENTRADA = 111,

        };

        /// <summary>
        /// Get tbLogErro/tbLogErro
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbLogErro> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbLogErros.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDLOGERRO:
                        Int32 idLogErro = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogErro.Equals(idLogErro)).AsQueryable();
                        break;
                    case CAMPOS.DSAPLICACAO:
                        string dsAplicacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsAplicacao.Equals(dsAplicacao)).AsQueryable();
                        break;
                    case CAMPOS.DSVERSAO:
                        string dsVersao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsVersao.Equals(dsVersao)).AsQueryable();
                        break;
                    case CAMPOS.DTERRO:
                        DateTime dtErro = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtErro.Equals(dtErro)).AsQueryable();
                        break;
                    case CAMPOS.DSNOMECOMPUTADOR:
                        string dsNomeComputador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsNomeComputador.Equals(dsNomeComputador)).AsQueryable();
                        break;
                    case CAMPOS.DSNOMEUSUARIO:
                        string dsNomeUsuario = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsNomeUsuario.Equals(dsNomeUsuario)).AsQueryable();
                        break;
                    case CAMPOS.DSVERSAOSO:
                        string dsVersaoSO = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsVersaoSO.Equals(dsVersaoSO)).AsQueryable();
                        break;
                    case CAMPOS.DSCULTURA:
                        string dsCultura = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsCultura.Equals(dsCultura)).AsQueryable();
                        break;
                    case CAMPOS.DSMENSAGEM:
                        string dsMensagem = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsMensagem.Equals(dsMensagem)).AsQueryable();
                        break;
                    case CAMPOS.DSSTACKTRACE:
                        string dsStackTrace = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsStackTrace.Equals(dsStackTrace)).AsQueryable();
                        break;
                    case CAMPOS.DSXMLENTRADA:
                        string dsXmlEntrada = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsXmlEntrada.Equals(dsXmlEntrada)).AsQueryable();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDLOGERRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogErro).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.idLogErro).AsQueryable();
                    break;
                case CAMPOS.DSAPLICACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsAplicacao).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsAplicacao).AsQueryable();
                    break;
                case CAMPOS.DSVERSAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsVersao).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsVersao).AsQueryable();
                    break;
                case CAMPOS.DTERRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtErro).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtErro).AsQueryable();
                    break;
                case CAMPOS.DSNOMECOMPUTADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsNomeComputador).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsNomeComputador).AsQueryable();
                    break;
                case CAMPOS.DSNOMEUSUARIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsNomeUsuario).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsNomeUsuario).AsQueryable();
                    break;
                case CAMPOS.DSVERSAOSO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsVersaoSO).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsVersaoSO).AsQueryable();
                    break;
                case CAMPOS.DSCULTURA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsCultura).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsCultura).AsQueryable();
                    break;
                case CAMPOS.DSMENSAGEM:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsMensagem).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsMensagem).AsQueryable();
                    break;
                case CAMPOS.DSSTACKTRACE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsStackTrace).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsStackTrace).AsQueryable();
                    break;
                case CAMPOS.DSXMLENTRADA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsXmlEntrada).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsXmlEntrada).AsQueryable();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna tbLogErro/tbLogErro
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbLogErro = new List<dynamic>();
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
                    CollectionTbLogErro = query.Select(e => new
                    {

                        idLogErro = e.idLogErro,
                        dsAplicacao = e.dsAplicacao,
                        dsVersao = e.dsVersao,
                        dtErro = e.dtErro,
                        dsNomeComputador = e.dsNomeComputador,
                        dsNomeUsuario = e.dsNomeUsuario,
                        dsVersaoSO = e.dsVersaoSO,
                        dsCultura = e.dsCultura,
                        dsMensagem = e.dsMensagem,
                        dsStackTrace = e.dsStackTrace,
                        dsXmlEntrada = e.dsXmlEntrada,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbLogErro = query.Select(e => new
                    {

                        idLogErro = e.idLogErro,
                        dsAplicacao = e.dsAplicacao,
                        dsVersao = e.dsVersao,
                        dtErro = e.dtErro,
                        dsNomeComputador = e.dsNomeComputador,
                        dsNomeUsuario = e.dsNomeUsuario,
                        dsVersaoSO = e.dsVersaoSO,
                        dsCultura = e.dsCultura,
                        dsMensagem = e.dsMensagem,
                        dsStackTrace = e.dsStackTrace,
                        dsXmlEntrada = e.dsXmlEntrada,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbLogErro;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar tbLogErro" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova tbLogErro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbLogErro param)
        {
            try
            {
                _db.tbLogErros.Add(param);
                _db.SaveChanges();
                return param.idLogErro;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar tbLogErro" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma tbLogErro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idLogErro)
        {
            try
            {
                _db.tbLogErros.Remove(_db.tbLogErros.Where(e => e.idLogErro.Equals(idLogErro)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar tbLogErro" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbLogErro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbLogErro param)
        {
            try
            {
                tbLogErro value = _db.tbLogErros
                        .Where(e => e.idLogErro.Equals(param.idLogErro))
                        .First<tbLogErro>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idLogErro != null && param.idLogErro != value.idLogErro)
                    value.idLogErro = param.idLogErro;
                if (param.dsAplicacao != null && param.dsAplicacao != value.dsAplicacao)
                    value.dsAplicacao = param.dsAplicacao;
                if (param.dsVersao != null && param.dsVersao != value.dsVersao)
                    value.dsVersao = param.dsVersao;
                if (param.dtErro != null && param.dtErro != value.dtErro)
                    value.dtErro = param.dtErro;
                if (param.dsNomeComputador != null && param.dsNomeComputador != value.dsNomeComputador)
                    value.dsNomeComputador = param.dsNomeComputador;
                if (param.dsNomeUsuario != null && param.dsNomeUsuario != value.dsNomeUsuario)
                    value.dsNomeUsuario = param.dsNomeUsuario;
                if (param.dsVersaoSO != null && param.dsVersaoSO != value.dsVersaoSO)
                    value.dsVersaoSO = param.dsVersaoSO;
                if (param.dsCultura != null && param.dsCultura != value.dsCultura)
                    value.dsCultura = param.dsCultura;
                if (param.dsMensagem != null && param.dsMensagem != value.dsMensagem)
                    value.dsMensagem = param.dsMensagem;
                if (param.dsStackTrace != null && param.dsStackTrace != value.dsStackTrace)
                    value.dsStackTrace = param.dsStackTrace;
                if (param.dsXmlEntrada != null && param.dsXmlEntrada != value.dsXmlEntrada)
                    value.dsXmlEntrada = param.dsXmlEntrada;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar tbLogErro" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
