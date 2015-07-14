using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Pos
{
    public class GatewayConciliacaoRecebimento
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoRecebimento()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            ID_USERS = 101,
            IDGRUPO = 102,
            MES = 103,
            ANO = 104,
            DATA = 105,
            QUANTIDADE = 106,
            VALOR = 107,
            OBSERVACAO = 108,
            STATUS = 109,

        };

        /// <summary>
        /// Get ConciliacaoRecebimento/ConciliacaoRecebimento
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<ConciliacaoRecebimento> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.ConciliacaoRecebimentoes.AsQueryable<ConciliacaoRecebimento>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.ID_USERS:
                        Int32 id_users = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_users.Equals(id_users)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.IDGRUPO:
                        Int32 idGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idGrupo.Equals(idGrupo)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.MES:
                        byte mes = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.mes.Equals(mes)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.ANO:
                        Int32 ano = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.ano.Equals(ano)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.DATA:
                        DateTime data = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data.Equals(data)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.QUANTIDADE:
                        Int32 quantidade = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.quantidade.Equals(quantidade)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.VALOR:
                        decimal valor = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valor.Equals(valor)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.OBSERVACAO:
                        string observacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.observacao.Equals(observacao)).AsQueryable<ConciliacaoRecebimento>();
                        break;
                    case CAMPOS.STATUS:
                        byte status = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.status.Equals(status)).AsQueryable<ConciliacaoRecebimento>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.ID_USERS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_users).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.id_users).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.IDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idGrupo).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.idGrupo).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.MES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.mes).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.mes).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.ANO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ano).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.ano).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.DATA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.data).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.QUANTIDADE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.quantidade).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.quantidade).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.VALOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valor).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.valor).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.OBSERVACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.observacao).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.observacao).AsQueryable<ConciliacaoRecebimento>();
                    break;
                case CAMPOS.STATUS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.status).AsQueryable<ConciliacaoRecebimento>();
                    else entity = entity.OrderByDescending(e => e.status).AsQueryable<ConciliacaoRecebimento>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna ConciliacaoRecebimento/ConciliacaoRecebimento
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionConciliacaoRecebimento = new List<dynamic>();
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
                CollectionConciliacaoRecebimento = query.Select(e => new
                {

                    id = e.id,
                    id_users = e.id_users,
                    idGrupo = e.idGrupo,
                    mes = e.mes,
                    ano = e.ano,
                    data = e.data,
                    quantidade = e.quantidade,
                    valor = e.valor,
                    observacao = e.observacao,
                    status = e.status,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionConciliacaoRecebimento = query.Select(e => new
                {

                    id = e.id,
                    id_users = e.id_users,
                    idGrupo = e.idGrupo,
                    mes = e.mes,
                    ano = e.ano,
                    data = e.data,
                    quantidade = e.quantidade,
                    valor = e.valor,
                    observacao = e.observacao,
                    status = e.status,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionConciliacaoRecebimento;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova ConciliacaoRecebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, ConciliacaoRecebimento param)
        {
            _db.ConciliacaoRecebimentoes.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma ConciliacaoRecebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.ConciliacaoRecebimentoes.Remove(_db.ConciliacaoRecebimentoes.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera ConciliacaoRecebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, ConciliacaoRecebimento param)
        {
            ConciliacaoRecebimento value = _db.ConciliacaoRecebimentoes
                    .Where(e => e.id.Equals(param.id))
                    .First<ConciliacaoRecebimento>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.id_users != null && param.id_users != value.id_users)
                value.id_users = param.id_users;
            if (param.idGrupo != null && param.idGrupo != value.idGrupo)
                value.idGrupo = param.idGrupo;
            if (param.mes != null && param.mes != value.mes)
                value.mes = param.mes;
            if (param.ano != null && param.ano != value.ano)
                value.ano = param.ano;
            if (param.data != null && param.data != value.data)
                value.data = param.data;
            if (param.quantidade != null && param.quantidade != value.quantidade)
                value.quantidade = param.quantidade;
            if (param.valor != null && param.valor != value.valor)
                value.valor = param.valor;
            if (param.observacao != null && param.observacao != value.observacao)
                value.observacao = param.observacao;
            if (param.status != null && param.status != value.status)
                value.status = param.status;
            _db.SaveChanges();

        }

    }
}
