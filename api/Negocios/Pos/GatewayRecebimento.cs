using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Globalization;

namespace api.Negocios.Pos
{
    public class GatewayRecebimento
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRecebimento()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            IDBANDEIRA = 101,
            CNPJ = 102,
            NSU = 103,
            CDAUTORIZADOR = 104,
            DTAVENDA = 105,
            VALORVENDABRUTA = 106,
            VALORVENDALIQUIDA = 107,
            LOTEIMPORTACAO = 108,
            DTARECEBIMENTO = 109,
            IDLOGICOTERMINAL = 110,
            CODTITULOERP = 111,
            CODVENDAERP = 112,
            CODRESUMOVENDA = 113,
            NUMPARCELATOTAL = 114,

            ID_GRUPO = 316,

        };

        /// <summary>
        /// Get Recebimento/Recebimento
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<Recebimento> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Recebimentoes.AsQueryable<Recebimento>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nsu.Equals(nsu)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string cdAutorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizador.Equals(cdAutorizador)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.VALORVENDABRUTA:
                        decimal valorVendaBruta = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorVendaBruta.Equals(valorVendaBruta)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.VALORVENDALIQUIDA:
                        decimal valorVendaLiquida = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorVendaLiquida.Equals(valorVendaLiquida)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.LOTEIMPORTACAO:
                        string loteImportacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.loteImportacao.Equals(loteImportacao)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.IDLOGICOTERMINAL:
                        Int32 idLogicoTerminal = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogicoTerminal.Equals(idLogicoTerminal)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.CODTITULOERP:
                        string codTituloERP = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.codTituloERP.Equals(codTituloERP)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.CODVENDAERP:
                        string codVendaERP = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.codVendaERP.Equals(codVendaERP)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.CODRESUMOVENDA:
                        string codResumoVenda = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.codResumoVenda.Equals(codResumoVenda)).AsQueryable<Recebimento>();
                        break;
                    case CAMPOS.NUMPARCELATOTAL:
                        Int32 numParcelaTotal = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.numParcelaTotal.Equals(numParcelaTotal)).AsQueryable<Recebimento>();
                        break;

                    /// PERSONALIZADO

                    case CAMPOS.DTAVENDA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0], "yyyyMMdd", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                            if (dtaIni == dtaFim)
                                entity = entity.Where(e => e.dtaVenda.Month == dtaIni.Month && e.dtaVenda.Year == dtaIni.Year);
                            else
                                entity = entity.Where(e => e.dtaVenda >= dtaIni && e.dtaVenda <= dtaFim);
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca, "yyyyMMdd", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaVenda >= dta);
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca, "yyyyMMdd", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaVenda <= dta);
                        }
                        else // IGUAL
                        {
                            DateTime dtvenda = Convert.ToDateTime(item.Value);
                            entity = entity.Where(e => e.dtaVenda.Equals(dtvenda));
                        }
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 idGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.empresa.id_grupo.Equals(idGrupo));
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.NSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nsu).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.nsu).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizador).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizador).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.DTAVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaVenda).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.dtaVenda).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.VALORVENDABRUTA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorVendaBruta).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.valorVendaBruta).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.VALORVENDALIQUIDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorVendaLiquida).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.valorVendaLiquida).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.LOTEIMPORTACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.loteImportacao).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.loteImportacao).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.IDLOGICOTERMINAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogicoTerminal).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.idLogicoTerminal).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.CODTITULOERP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codTituloERP).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.codTituloERP).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.CODVENDAERP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codVendaERP).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.codVendaERP).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.CODRESUMOVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codResumoVenda).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.codResumoVenda).AsQueryable<Recebimento>();
                    break;
                case CAMPOS.NUMPARCELATOTAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numParcelaTotal).AsQueryable<Recebimento>();
                    else entity = entity.OrderByDescending(e => e.numParcelaTotal).AsQueryable<Recebimento>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Recebimento/Recebimento
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionRecebimento = new List<dynamic>();
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
                CollectionRecebimento = query.Select(e => new
                {

                    id = e.id,
                    idBandeira = e.idBandeira,
                    cnpj = e.cnpj,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    dtaVenda = e.dtaVenda,
                    valorVendaBruta = e.valorVendaBruta,
                    valorVendaLiquida = e.valorVendaLiquida,
                    loteImportacao = e.loteImportacao,
                    dtaRecebimento = e.dtaRecebimento,
                    idLogicoTerminal = e.idLogicoTerminal,
                    codTituloERP = e.codTituloERP,
                    codVendaERP = e.codVendaERP,
                    codResumoVenda = e.codResumoVenda,
                    numParcelaTotal = e.numParcelaTotal,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionRecebimento = query.Select(e => new
                {

                    id = e.id,
                    idBandeira = e.idBandeira,
                    cnpj = e.cnpj,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    dtaVenda = e.dtaVenda,
                    valorVendaBruta = e.valorVendaBruta,
                    valorVendaLiquida = e.valorVendaLiquida,
                    loteImportacao = e.loteImportacao,
                    dtaRecebimento = e.dtaRecebimento,
                    idLogicoTerminal = e.idLogicoTerminal,
                    codTituloERP = e.codTituloERP,
                    codVendaERP = e.codVendaERP,
                    codResumoVenda = e.codResumoVenda,
                    numParcelaTotal = e.numParcelaTotal,
                }).ToList<dynamic>();
            }
            else if (colecao == 2)
            {
                var queryRecebimento = query
                    .GroupBy(x => new { x.dtaVenda.Year, x.dtaVenda.Month, x.empresa.id_grupo })
                    .Select(e => new
                    {

                        nrAno = e.Key.Year,
                        nrMes = e.Key.Month,
                        cdGrupo = e.Key.id_grupo,
                        vlVenda = e.Sum(l => l.valorVendaBruta)
                    });

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = queryTotal.Count();

                // PAGINAÇÃO
                skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    query = query.Skip(skipRows).Take(pageSize);
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                CollectionRecebimento = queryRecebimento.OrderBy(o => new { o.nrAno, o.nrMes }).ToList<dynamic>();

            }
            else if (colecao == 3) // Portal/RelatorioTerminalLogico
            {
                var teste = query
                .GroupBy(e => new { e.TerminalLogico, e.BandeiraPos })
                .Select(e => new
                {
                    terminal = new {
                                        e.Key.TerminalLogico.idTerminalLogico,
                                        e.Key.TerminalLogico.dsTerminalLogico,
                                        e.Key.TerminalLogico.idOperadora
                    },
                    bandeira = new {
                                        e.Key.BandeiraPos.id,
                                        e.Key.BandeiraPos.desBandeira,
                                        e.Key.BandeiraPos.idOperadora
                    },
                    totalTransacoes = e.Count(),
                    valorBruto = e.Sum( p => p.valorVendaBruta )


                    /*
                    id = e.id,
                    idBandeira = e.idBandeira,
                    cnpj = e.cnpj,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    dtaVenda = e.dtaVenda,
                    valorVendaBruta = e.valorVendaBruta,
                    valorVendaLiquida = e.valorVendaLiquida,
                    loteImportacao = e.loteImportacao,
                    dtaRecebimento = e.dtaRecebimento,
                    idLogicoTerminal = e.idLogicoTerminal,
                    codTituloERP = e.codTituloERP,
                    codVendaERP = e.codVendaERP,
                    codResumoVenda = e.codResumoVenda,
                    numParcelaTotal = e.numParcelaTotal,*/
                });///.ToList<dynamic>();
                   ///
                CollectionRecebimento = teste.ToList<dynamic>();
                retorno.TotalDeRegistros = CollectionRecebimento.Count;
            }


            retorno.Registros = CollectionRecebimento;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Recebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Recebimento param)
        {
            _db.Recebimentoes.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma Recebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.Recebimentoes.Remove(_db.Recebimentoes.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera Recebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Recebimento param)
        {
            Recebimento value = _db.Recebimentoes
                    .Where(e => e.id.Equals(param.id))
                    .First<Recebimento>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.idBandeira != null && param.idBandeira != value.idBandeira)
                value.idBandeira = param.idBandeira;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.nsu != null && param.nsu != value.nsu)
                value.nsu = param.nsu;
            if (param.cdAutorizador != null && param.cdAutorizador != value.cdAutorizador)
                value.cdAutorizador = param.cdAutorizador;
            if (param.dtaVenda != null && param.dtaVenda != value.dtaVenda)
                value.dtaVenda = param.dtaVenda;
            if (param.valorVendaBruta != null && param.valorVendaBruta != value.valorVendaBruta)
                value.valorVendaBruta = param.valorVendaBruta;
            if (param.valorVendaLiquida != null && param.valorVendaLiquida != value.valorVendaLiquida)
                value.valorVendaLiquida = param.valorVendaLiquida;
            if (param.loteImportacao != null && param.loteImportacao != value.loteImportacao)
                value.loteImportacao = param.loteImportacao;
            if (param.dtaRecebimento != null && param.dtaRecebimento != value.dtaRecebimento)
                value.dtaRecebimento = param.dtaRecebimento;
            if (param.idLogicoTerminal != null && param.idLogicoTerminal != value.idLogicoTerminal)
                value.idLogicoTerminal = param.idLogicoTerminal;
            if (param.codTituloERP != null && param.codTituloERP != value.codTituloERP)
                value.codTituloERP = param.codTituloERP;
            if (param.codVendaERP != null && param.codVendaERP != value.codVendaERP)
                value.codVendaERP = param.codVendaERP;
            if (param.codResumoVenda != null && param.codResumoVenda != value.codResumoVenda)
                value.codResumoVenda = param.codResumoVenda;
            if (param.numParcelaTotal != null && param.numParcelaTotal != value.numParcelaTotal)
                value.numParcelaTotal = param.numParcelaTotal;
            _db.SaveChanges();

        }

    }
}
