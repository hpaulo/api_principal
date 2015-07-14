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
    public class GatewayCielo
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayCielo()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            DTAVENDA = 101,
            DTAPREVISTAPAGTO = 102,
            DESCRICAO = 103,
            RESUMO = 104,
            CNPJ = 105,
            NUMCARTAO = 106,
            NSU = 107,
            CDAUTORIZADOR = 108,
            VALORTOTAL = 109,
            VALORBRUTO = 110,
            REJEITADO = 111,
            VALORSAQUE = 112,
            IDOPERADORA = 113,
            IDBANDEIRA = 114,
            DTARECEBIMENTO = 115,
            IDTERMINALLOGICO = 116,

        };

        /// <summary>
        /// Get Cielo/Cielo
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<Cielo> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Cieloes.AsQueryable<Cielo>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.DTAVENDA:
                        DateTime dtaVenda = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaVenda.Equals(dtaVenda)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.DTAPREVISTAPAGTO:
                        DateTime dtaPrevistaPagto = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaPrevistaPagto.Equals(dtaPrevistaPagto)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.DESCRICAO:
                        string descricao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.descricao.Equals(descricao)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.RESUMO:
                        string resumo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.resumo.Equals(resumo)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nsu.Equals(nsu)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string cdAutorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizador.Equals(cdAutorizador)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.VALORTOTAL:
                        decimal valorTotal = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorTotal.Equals(valorTotal)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.VALORBRUTO:
                        decimal valorBruto = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorBruto.Equals(valorBruto)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.REJEITADO:
                        string rejeitado = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.rejeitado.Equals(rejeitado)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.VALORSAQUE:
                        decimal valorSaque = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorSaque.Equals(valorSaque)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<Cielo>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<Cielo>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<Cielo>();
                    break;
                case CAMPOS.DTAVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaVenda).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.dtaVenda).AsQueryable<Cielo>();
                    break;
                case CAMPOS.DTAPREVISTAPAGTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaPrevistaPagto).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.dtaPrevistaPagto).AsQueryable<Cielo>();
                    break;
                case CAMPOS.DESCRICAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.descricao).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.descricao).AsQueryable<Cielo>();
                    break;
                case CAMPOS.RESUMO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.resumo).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.resumo).AsQueryable<Cielo>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<Cielo>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<Cielo>();
                    break;
                case CAMPOS.NSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nsu).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.nsu).AsQueryable<Cielo>();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizador).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizador).AsQueryable<Cielo>();
                    break;
                case CAMPOS.VALORTOTAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorTotal).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.valorTotal).AsQueryable<Cielo>();
                    break;
                case CAMPOS.VALORBRUTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorBruto).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.valorBruto).AsQueryable<Cielo>();
                    break;
                case CAMPOS.REJEITADO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.rejeitado).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.rejeitado).AsQueryable<Cielo>();
                    break;
                case CAMPOS.VALORSAQUE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorSaque).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.valorSaque).AsQueryable<Cielo>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<Cielo>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<Cielo>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<Cielo>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<Cielo>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<Cielo>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Cielo/Cielo
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionCielo = new List<dynamic>();
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
                CollectionCielo = query.Select(e => new
                {

                    id = e.id,
                    dtaVenda = e.dtaVenda,
                    dtaPrevistaPagto = e.dtaPrevistaPagto,
                    descricao = e.descricao,
                    resumo = e.resumo,
                    cnpj = e.cnpj,
                    numCartao = e.numCartao,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    valorTotal = e.valorTotal,
                    valorBruto = e.valorBruto,
                    rejeitado = e.rejeitado,
                    valorSaque = e.valorSaque,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionCielo = query.Select(e => new
                {

                    id = e.id,
                    dtaVenda = e.dtaVenda,
                    dtaPrevistaPagto = e.dtaPrevistaPagto,
                    descricao = e.descricao,
                    resumo = e.resumo,
                    cnpj = e.cnpj,
                    numCartao = e.numCartao,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    valorTotal = e.valorTotal,
                    valorBruto = e.valorBruto,
                    rejeitado = e.rejeitado,
                    valorSaque = e.valorSaque,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionCielo;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Cielo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Cielo param)
        {
            _db.Cieloes.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma Cielo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.Cieloes.Remove(_db.Cieloes.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera Cielo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Cielo param)
        {
            Cielo value = _db.Cieloes
                    .Where(e => e.id.Equals(param.id))
                    .First<Cielo>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.dtaVenda != null && param.dtaVenda != value.dtaVenda)
                value.dtaVenda = param.dtaVenda;
            if (param.dtaPrevistaPagto != null && param.dtaPrevistaPagto != value.dtaPrevistaPagto)
                value.dtaPrevistaPagto = param.dtaPrevistaPagto;
            if (param.descricao != null && param.descricao != value.descricao)
                value.descricao = param.descricao;
            if (param.resumo != null && param.resumo != value.resumo)
                value.resumo = param.resumo;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.nsu != null && param.nsu != value.nsu)
                value.nsu = param.nsu;
            if (param.cdAutorizador != null && param.cdAutorizador != value.cdAutorizador)
                value.cdAutorizador = param.cdAutorizador;
            if (param.valorTotal != null && param.valorTotal != value.valorTotal)
                value.valorTotal = param.valorTotal;
            if (param.valorBruto != null && param.valorBruto != value.valorBruto)
                value.valorBruto = param.valorBruto;
            if (param.rejeitado != null && param.rejeitado != value.rejeitado)
                value.rejeitado = param.rejeitado;
            if (param.valorSaque != null && param.valorSaque != value.valorSaque)
                value.valorSaque = param.valorSaque;
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
