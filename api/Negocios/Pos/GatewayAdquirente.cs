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
    public class GatewayAdquirente
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayAdquirente()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            NOME = 101,
            DESCRICAO = 102,
            STATUS = 103,
            HRAEXECUCAO = 104,

        };

        /// <summary>
        /// Get Adquirente/Adquirente
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<api.Models.Adquirente> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Adquirentes.AsQueryable<api.Models.Adquirente>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<api.Models.Adquirente>();
                        break;
                    case CAMPOS.NOME:
                        string nome = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nome.Equals(nome)).AsQueryable<api.Models.Adquirente>();
                        break;
                    case CAMPOS.DESCRICAO:
                        string descricao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.descricao.Equals(descricao)).AsQueryable<api.Models.Adquirente>();
                        break;
                    case CAMPOS.STATUS:
                        byte status = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.status.Equals(status)).AsQueryable<api.Models.Adquirente>();
                        break;
                    case CAMPOS.HRAEXECUCAO:
                        DateTime hraExecucao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.hraExecucao.Equals(hraExecucao)).AsQueryable<api.Models.Adquirente>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<api.Models.Adquirente>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<api.Models.Adquirente>();
                    break;
                case CAMPOS.NOME:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nome).AsQueryable<api.Models.Adquirente>();
                    else entity = entity.OrderByDescending(e => e.nome).AsQueryable<api.Models.Adquirente>();
                    break;
                case CAMPOS.DESCRICAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.descricao).AsQueryable<api.Models.Adquirente>();
                    else entity = entity.OrderByDescending(e => e.descricao).AsQueryable<api.Models.Adquirente>();
                    break;
                case CAMPOS.STATUS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.status).AsQueryable<api.Models.Adquirente>();
                    else entity = entity.OrderByDescending(e => e.status).AsQueryable<api.Models.Adquirente>();
                    break;
                case CAMPOS.HRAEXECUCAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.hraExecucao).AsQueryable<api.Models.Adquirente>();
                    else entity = entity.OrderByDescending(e => e.hraExecucao).AsQueryable<api.Models.Adquirente>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Adquirente/Adquirente
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionAdquirente = new List<dynamic>();
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
                    CollectionAdquirente = query.Select(e => new
                    {

                        id = e.id,
                        nome = e.nome,
                        descricao = e.descricao,
                        status = e.status,
                        hraExecucao = e.hraExecucao,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0) // [web]
                {
                    CollectionAdquirente = query.Select(e => new
                    {

                        id = e.id,
                        nome = e.nome,
                        descricao = e.descricao,
                        status = e.status,
                        hraExecucao = e.hraExecucao,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionAdquirente;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar adquirente" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova Adquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, api.Models.Adquirente param)
        {
            try
            {
                _db.Adquirentes.Add(param);
                _db.SaveChanges();
                return param.id;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar adquirente" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma Adquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            try
            {
                _db.Adquirentes.Remove(_db.Adquirentes.Where(e => e.id.Equals(id)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar adquirente" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera Adquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, api.Models.Adquirente param)
        {
            try
            {
                api.Models.Adquirente value = _db.Adquirentes
                        .Where(e => e.id.Equals(param.id))
                        .First<api.Models.Adquirente>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.id != null && param.id != value.id)
                    value.id = param.id;
                if (param.nome != null && param.nome != value.nome)
                    value.nome = param.nome;
                if (param.descricao != null && param.descricao != value.descricao)
                    value.descricao = param.descricao;
                if (param.status != null && param.status != value.status)
                    value.status = param.status;
                if (param.hraExecucao != null && param.hraExecucao != value.hraExecucao)
                    value.hraExecucao = param.hraExecucao;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar adquirente" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
