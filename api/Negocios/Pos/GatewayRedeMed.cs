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
    public class GatewayRedeMed
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRedeMed()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            CDAUTORIZADOR = 101,
            EMPRESA = 102,
            NOME = 103,
            DATA = 104,
            NUMCARTAO = 105,
            VALOR = 106,
            PARCELA = 107,
            CANCELADA = 108,
            CNPJ = 109,
            IDOPERADORA = 110,
            IDBANDEIRA = 111,
            DTARECEBIMENTO = 112,
            IDTERMINALLOGICO = 113,

        };

        /// <summary>
        /// Get RedeMed/RedeMed
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<RedeMed> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.RedeMeds.AsQueryable<RedeMed>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string cdAutorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizador.Equals(cdAutorizador)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.EMPRESA:
                        string empresa = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.empresa.Equals(empresa)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.NOME:
                        string nome = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nome.Equals(nome)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.DATA:
                        DateTime data = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data.Equals(data)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.VALOR:
                        decimal valor = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valor.Equals(valor)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.PARCELA:
                        string parcela = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.parcela.Equals(parcela)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.CANCELADA:
                        string cancelada = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cancelada.Equals(cancelada)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<RedeMed>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<RedeMed>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizador).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizador).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.EMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.empresa).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.empresa).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.NOME:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nome).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.nome).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.DATA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.data).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.VALOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valor).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.valor).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.PARCELA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.parcela).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.parcela).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.CANCELADA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cancelada).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.cancelada).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<RedeMed>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<RedeMed>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<RedeMed>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna RedeMed/RedeMed
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionRedeMed = new List<dynamic>();
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
                CollectionRedeMed = query.Select(e => new
                {

                    id = e.id,
                    cdAutorizador = e.cdAutorizador,
                    empresa = e.empresa,
                    nome = e.nome,
                    data = e.data,
                    numCartao = e.numCartao,
                    valor = e.valor,
                    parcela = e.parcela,
                    cancelada = e.cancelada,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionRedeMed = query.Select(e => new
                {

                    id = e.id,
                    cdAutorizador = e.cdAutorizador,
                    empresa = e.empresa,
                    nome = e.nome,
                    data = e.data,
                    numCartao = e.numCartao,
                    valor = e.valor,
                    parcela = e.parcela,
                    cancelada = e.cancelada,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionRedeMed;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova RedeMed
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, RedeMed param)
        {
            _db.RedeMeds.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma RedeMed
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.RedeMeds.Remove(_db.RedeMeds.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera RedeMed
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, RedeMed param)
        {
            RedeMed value = _db.RedeMeds
                    .Where(e => e.id.Equals(param.id))
                    .First<RedeMed>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.cdAutorizador != null && param.cdAutorizador != value.cdAutorizador)
                value.cdAutorizador = param.cdAutorizador;
            if (param.empresa != null && param.empresa != value.empresa)
                value.empresa = param.empresa;
            if (param.nome != null && param.nome != value.nome)
                value.nome = param.nome;
            if (param.data != null && param.data != value.data)
                value.data = param.data;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.valor != null && param.valor != value.valor)
                value.valor = param.valor;
            if (param.parcela != null && param.parcela != value.parcela)
                value.parcela = param.parcela;
            if (param.cancelada != null && param.cancelada != value.cancelada)
                value.cancelada = param.cancelada;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.idOperadora != null && param.idOperadora != value.idOperadora)
                value.idOperadora = param.idOperadora;
            if (param.idBandeira != null && param.idBandeira != value.idBandeira)
                value.idBandeira = param.idBandeira;
            if (param.dtaRecebimento != null && param.dtaRecebimento != value.dtaRecebimento)
                value.dtaRecebimento = param.dtaRecebimento;
            if (param.idTerminalLogico != null && param.idTerminalLogico != value.idTerminalLogico)
                value.idTerminalLogico = param.idTerminalLogico;
            _db.SaveChanges();

        }

    }
}
