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
    public class GatewayTaxaAdministracao
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTaxaAdministracao()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            IDADQUIRENTE = 101,
            IDBANDEIRA = 102,
            CNPJ = 103,
            PLANO = 104,
            NUMPARCELA = 105,
            NUMBANCO = 106,
            NUMAGENCIA = 107,
            NUMCONTACORRENTE = 108,
            TAXA = 109,
            DTAATUALIZACAO = 110,

        };

        /// <summary>
        /// Get TaxaAdministracao/TaxaAdministracao
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<TaxaAdministracao> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.TaxaAdministracaos.AsQueryable<TaxaAdministracao>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.IDADQUIRENTE:
                        Int32 idAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idAdquirente.Equals(idAdquirente)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.PLANO:
                        string plano = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.plano.Equals(plano)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.NUMPARCELA:
                        Int32 numParcela = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.numParcela.Equals(numParcela)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.NUMBANCO:
                        string numBanco = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numBanco.Equals(numBanco)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.NUMAGENCIA:
                        string numAgencia = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numAgencia.Equals(numAgencia)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.NUMCONTACORRENTE:
                        string numContaCorrente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numContaCorrente.Equals(numContaCorrente)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.TAXA:
                        decimal taxa = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.taxa.Equals(taxa)).AsQueryable<TaxaAdministracao>();
                        break;
                    case CAMPOS.DTAATUALIZACAO:
                        DateTime dtaAtualizacao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaAtualizacao.Equals(dtaAtualizacao)).AsQueryable<TaxaAdministracao>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.IDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idAdquirente).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.idAdquirente).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.PLANO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.plano).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.plano).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.NUMPARCELA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numParcela).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.numParcela).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.NUMBANCO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numBanco).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.numBanco).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.NUMAGENCIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numAgencia).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.numAgencia).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.NUMCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numContaCorrente).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.numContaCorrente).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.TAXA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.taxa).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.taxa).AsQueryable<TaxaAdministracao>();
                    break;
                case CAMPOS.DTAATUALIZACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaAtualizacao).AsQueryable<TaxaAdministracao>();
                    else entity = entity.OrderByDescending(e => e.dtaAtualizacao).AsQueryable<TaxaAdministracao>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TaxaAdministracao/TaxaAdministracao
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTaxaAdministracao = new List<dynamic>();
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
                CollectionTaxaAdministracao = query.Select(e => new
                {

                    id = e.id,
                    idAdquirente = e.idAdquirente,
                    idBandeira = e.idBandeira,
                    cnpj = e.cnpj,
                    plano = e.plano,
                    numParcela = e.numParcela,
                    numBanco = e.numBanco,
                    numAgencia = e.numAgencia,
                    numContaCorrente = e.numContaCorrente,
                    taxa = e.taxa,
                    dtaAtualizacao = e.dtaAtualizacao,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTaxaAdministracao = query.Select(e => new
                {

                    id = e.id,
                    idAdquirente = e.idAdquirente,
                    idBandeira = e.idBandeira,
                    cnpj = e.cnpj,
                    plano = e.plano,
                    numParcela = e.numParcela,
                    numBanco = e.numBanco,
                    numAgencia = e.numAgencia,
                    numContaCorrente = e.numContaCorrente,
                    taxa = e.taxa,
                    dtaAtualizacao = e.dtaAtualizacao,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTaxaAdministracao;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TaxaAdministracao
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, TaxaAdministracao param)
        {
            _db.TaxaAdministracaos.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma TaxaAdministracao
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.TaxaAdministracaos.Remove(_db.TaxaAdministracaos.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera TaxaAdministracao
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, TaxaAdministracao param)
        {
            TaxaAdministracao value = _db.TaxaAdministracaos
                    .Where(e => e.id.Equals(param.id))
                    .First<TaxaAdministracao>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.idAdquirente != null && param.idAdquirente != value.idAdquirente)
                value.idAdquirente = param.idAdquirente;
            if (param.idBandeira != null && param.idBandeira != value.idBandeira)
                value.idBandeira = param.idBandeira;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.plano != null && param.plano != value.plano)
                value.plano = param.plano;
            if (param.numParcela != null && param.numParcela != value.numParcela)
                value.numParcela = param.numParcela;
            if (param.numBanco != null && param.numBanco != value.numBanco)
                value.numBanco = param.numBanco;
            if (param.numAgencia != null && param.numAgencia != value.numAgencia)
                value.numAgencia = param.numAgencia;
            if (param.numContaCorrente != null && param.numContaCorrente != value.numContaCorrente)
                value.numContaCorrente = param.numContaCorrente;
            if (param.taxa != null && param.taxa != value.taxa)
                value.taxa = param.taxa;
            if (param.dtaAtualizacao != null && param.dtaAtualizacao != value.dtaAtualizacao)
                value.dtaAtualizacao = param.dtaAtualizacao;
            _db.SaveChanges();

        }

    }
}
