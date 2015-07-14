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
    public class GatewayFitCard
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayFitCard()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            NUMERO = 101,
            DATA = 102,
            HORA = 103,
            COMBUSTIVEL = 104,
            VALORTOTALLITROS = 105,
            VALOR = 106,
            VALORLITRO = 107,
            CNPJ = 108,
            IDOPERADORA = 109,
            IDBANDEIRA = 110,
            DTARECEBIMENTO = 111,
            IDTERMINALLOGICO = 112,

        };

        /// <summary>
        /// Get FitCard/FitCard
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<FitCard> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.FitCards.AsQueryable<FitCard>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.NUMERO:
                        Int32 numero = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.numero.Equals(numero)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.DATA:
                        DateTime data = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data.Equals(data)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.HORA:
                        string hora = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.hora.Equals(hora)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.COMBUSTIVEL:
                        string combustivel = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.combustivel.Equals(combustivel)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.VALORTOTALLITROS:
                        decimal valorTotalLitros = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorTotalLitros.Equals(valorTotalLitros)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.VALOR:
                        decimal valor = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valor.Equals(valor)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.VALORLITRO:
                        decimal valorLitro = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorLitro.Equals(valorLitro)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<FitCard>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<FitCard>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<FitCard>();
                    break;
                case CAMPOS.NUMERO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numero).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.numero).AsQueryable<FitCard>();
                    break;
                case CAMPOS.DATA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.data).AsQueryable<FitCard>();
                    break;
                case CAMPOS.HORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.hora).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.hora).AsQueryable<FitCard>();
                    break;
                case CAMPOS.COMBUSTIVEL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.combustivel).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.combustivel).AsQueryable<FitCard>();
                    break;
                case CAMPOS.VALORTOTALLITROS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorTotalLitros).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.valorTotalLitros).AsQueryable<FitCard>();
                    break;
                case CAMPOS.VALOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valor).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.valor).AsQueryable<FitCard>();
                    break;
                case CAMPOS.VALORLITRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorLitro).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.valorLitro).AsQueryable<FitCard>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<FitCard>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<FitCard>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<FitCard>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<FitCard>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<FitCard>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<FitCard>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna FitCard/FitCard
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionFitCard = new List<dynamic>();
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
                CollectionFitCard = query.Select(e => new
                {

                    id = e.id,
                    numero = e.numero,
                    data = e.data,
                    hora = e.hora,
                    combustivel = e.combustivel,
                    valorTotalLitros = e.valorTotalLitros,
                    valor = e.valor,
                    valorLitro = e.valorLitro,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionFitCard = query.Select(e => new
                {

                    id = e.id,
                    numero = e.numero,
                    data = e.data,
                    hora = e.hora,
                    combustivel = e.combustivel,
                    valorTotalLitros = e.valorTotalLitros,
                    valor = e.valor,
                    valorLitro = e.valorLitro,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionFitCard;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova FitCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, FitCard param)
        {
            _db.FitCards.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma FitCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.FitCards.Remove(_db.FitCards.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera FitCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, FitCard param)
        {
            FitCard value = _db.FitCards
                    .Where(e => e.id.Equals(param.id))
                    .First<FitCard>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.numero != null && param.numero != value.numero)
                value.numero = param.numero;
            if (param.data != null && param.data != value.data)
                value.data = param.data;
            if (param.hora != null && param.hora != value.hora)
                value.hora = param.hora;
            if (param.combustivel != null && param.combustivel != value.combustivel)
                value.combustivel = param.combustivel;
            if (param.valorTotalLitros != null && param.valorTotalLitros != value.valorTotalLitros)
                value.valorTotalLitros = param.valorTotalLitros;
            if (param.valor != null && param.valor != value.valor)
                value.valor = param.valor;
            if (param.valorLitro != null && param.valorLitro != value.valorLitro)
                value.valorLitro = param.valorLitro;
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
