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
    public class GatewayConciliacaoPagamentosPos
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoPagamentosPos()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDCONCILIACAOPAGAMENTO = 100,
            NU_CNPJ = 101,
            IDOPERADORA = 103,
            DTMOVIMENTOPAGTO = 104,
            VLVENDA = 105,
            CDAUTORIZADOR = 106,
            NUMPARCELA = 107,
            TOTALPARCELAS = 108,

        };

        /// <summary>
        /// Get ConciliacaoPagamentosPos/ConciliacaoPagamentosPos
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<ConciliacaoPagamentosPos> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.ConciliacaoPagamentosPos.AsQueryable<ConciliacaoPagamentosPos>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDCONCILIACAOPAGAMENTO:
                        Int32 IdConciliacaoPagamento = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.IdConciliacaoPagamento.Equals(IdConciliacaoPagamento)).AsQueryable<ConciliacaoPagamentosPos>();
                        break;
                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_cnpj.Equals(nu_cnpj)).AsQueryable<ConciliacaoPagamentosPos>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 IdOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.IdOperadora.Equals(IdOperadora)).AsQueryable<ConciliacaoPagamentosPos>();
                        break;
                    case CAMPOS.DTMOVIMENTOPAGTO:
                        DateTime DtMovimentoPagto = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.DtMovimentoPagto.Equals(DtMovimentoPagto)).AsQueryable<ConciliacaoPagamentosPos>();
                        break;
                    case CAMPOS.VLVENDA:
                        decimal VlVenda = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.VlVenda.Equals(VlVenda)).AsQueryable<ConciliacaoPagamentosPos>();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string CdAutorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.CdAutorizador.Equals(CdAutorizador)).AsQueryable<ConciliacaoPagamentosPos>();
                        break;
                    case CAMPOS.NUMPARCELA:
                        Int32 NumParcela = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.NumParcela.Equals(NumParcela)).AsQueryable<ConciliacaoPagamentosPos>();
                        break;
                    case CAMPOS.TOTALPARCELAS:
                        Int32 TotalParcelas = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.TotalParcelas.Equals(TotalParcelas)).AsQueryable<ConciliacaoPagamentosPos>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDCONCILIACAOPAGAMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.IdConciliacaoPagamento).AsQueryable<ConciliacaoPagamentosPos>();
                    else entity = entity.OrderByDescending(e => e.IdConciliacaoPagamento).AsQueryable<ConciliacaoPagamentosPos>();
                    break;
                case CAMPOS.NU_CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_cnpj).AsQueryable<ConciliacaoPagamentosPos>();
                    else entity = entity.OrderByDescending(e => e.nu_cnpj).AsQueryable<ConciliacaoPagamentosPos>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.IdOperadora).AsQueryable<ConciliacaoPagamentosPos>();
                    else entity = entity.OrderByDescending(e => e.IdOperadora).AsQueryable<ConciliacaoPagamentosPos>();
                    break;
                case CAMPOS.DTMOVIMENTOPAGTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.DtMovimentoPagto).AsQueryable<ConciliacaoPagamentosPos>();
                    else entity = entity.OrderByDescending(e => e.DtMovimentoPagto).AsQueryable<ConciliacaoPagamentosPos>();
                    break;
                case CAMPOS.VLVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.VlVenda).AsQueryable<ConciliacaoPagamentosPos>();
                    else entity = entity.OrderByDescending(e => e.VlVenda).AsQueryable<ConciliacaoPagamentosPos>();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.CdAutorizador).AsQueryable<ConciliacaoPagamentosPos>();
                    else entity = entity.OrderByDescending(e => e.CdAutorizador).AsQueryable<ConciliacaoPagamentosPos>();
                    break;
                case CAMPOS.NUMPARCELA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.NumParcela).AsQueryable<ConciliacaoPagamentosPos>();
                    else entity = entity.OrderByDescending(e => e.NumParcela).AsQueryable<ConciliacaoPagamentosPos>();
                    break;
                case CAMPOS.TOTALPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.TotalParcelas).AsQueryable<ConciliacaoPagamentosPos>();
                    else entity = entity.OrderByDescending(e => e.TotalParcelas).AsQueryable<ConciliacaoPagamentosPos>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna ConciliacaoPagamentosPos/ConciliacaoPagamentosPos
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionConciliacaoPagamentosPos = new List<dynamic>();
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
                CollectionConciliacaoPagamentosPos = query.Select(e => new
                {

                    IdConciliacaoPagamento = e.IdConciliacaoPagamento,
                    nu_cnpj = e.nu_cnpj,
                    IdOperadora = e.IdOperadora,
                    DtMovimentoPagto = e.DtMovimentoPagto,
                    VlVenda = e.VlVenda,
                    CdAutorizador = e.CdAutorizador,
                    NumParcela = e.NumParcela,
                    TotalParcelas = e.TotalParcelas,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionConciliacaoPagamentosPos = query.Select(e => new
                {

                    IdConciliacaoPagamento = e.IdConciliacaoPagamento,
                    nu_cnpj = e.nu_cnpj,
                    IdOperadora = e.IdOperadora,
                    DtMovimentoPagto = e.DtMovimentoPagto,
                    VlVenda = e.VlVenda,
                    CdAutorizador = e.CdAutorizador,
                    NumParcela = e.NumParcela,
                    TotalParcelas = e.TotalParcelas,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionConciliacaoPagamentosPos;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova ConciliacaoPagamentosPos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, ConciliacaoPagamentosPos param)
        {
            _db.ConciliacaoPagamentosPos.Add(param);
            _db.SaveChanges();
            return param.IdConciliacaoPagamento;
        }


        /// <summary>
        /// Apaga uma ConciliacaoPagamentosPos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 IdConciliacaoPagamento)
        {
            _db.ConciliacaoPagamentosPos.Remove(_db.ConciliacaoPagamentosPos.Where(e => e.IdConciliacaoPagamento.Equals(IdConciliacaoPagamento)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera ConciliacaoPagamentosPos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, ConciliacaoPagamentosPos param)
        {
            ConciliacaoPagamentosPos value = _db.ConciliacaoPagamentosPos
                    .Where(e => e.IdConciliacaoPagamento.Equals(param.IdConciliacaoPagamento))
                    .First<ConciliacaoPagamentosPos>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.IdConciliacaoPagamento != null && param.IdConciliacaoPagamento != value.IdConciliacaoPagamento)
                value.IdConciliacaoPagamento = param.IdConciliacaoPagamento;
            if (param.nu_cnpj != null && param.nu_cnpj != value.nu_cnpj)
                value.nu_cnpj = param.nu_cnpj;
            if (param.IdOperadora != null && param.IdOperadora != value.IdOperadora)
                value.IdOperadora = param.IdOperadora;
            if (param.DtMovimentoPagto != null && param.DtMovimentoPagto != value.DtMovimentoPagto)
                value.DtMovimentoPagto = param.DtMovimentoPagto;
            if (param.VlVenda != null && param.VlVenda != value.VlVenda)
                value.VlVenda = param.VlVenda;
            if (param.CdAutorizador != null && param.CdAutorizador != value.CdAutorizador)
                value.CdAutorizador = param.CdAutorizador;
            if (param.NumParcela != null && param.NumParcela != value.NumParcela)
                value.NumParcela = param.NumParcela;
            if (param.TotalParcelas != null && param.TotalParcelas != value.TotalParcelas)
                value.TotalParcelas = param.TotalParcelas;
            _db.SaveChanges();

        }

    }
}
