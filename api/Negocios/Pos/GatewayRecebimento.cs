using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Globalization;
using System.Data.Entity.Validation;

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

            // OPERADORA (ADQUIRENTE)
            IDOPERADORA = 300,
            NMOPERADORA = 301,

            // EMPRESA
            DS_FANTASIA = 404,
            ID_GRUPO = 416,

            // BANDEIRA
            DESBANDEIRA = 501,

            // TERMINAL LÓGICO
            DSTERMINALLOGICO = 601,

        };

        public enum MES
        {
            Janeiro = 1, Fevereiro = 2, Março, Abril, Maio, Junho, Julho, Agosto, Setembro,
            Outubro, Novembro, Dezembro
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
            var entity = _db.Recebimentoes.AsQueryable();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable();
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        if (nsu.Contains("%")) // usa LIKE => STARTS WITH
                        {
                            string busca = nsu.Replace("%", "").ToString();
                            entity = entity.Where(e => e.nsu.StartsWith(busca));
                        }
                        else
                            entity = entity.Where(e => e.nsu.Equals(nsu)).AsQueryable();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string cdAutorizador = Convert.ToString(item.Value);
                        if (cdAutorizador.Contains("%")) // usa LIKE => STARTS WITH
                        {
                            string busca = cdAutorizador.Replace("%", "").ToString();
                            entity = entity.Where(e => e.cdAutorizador.StartsWith(busca));
                        }
                        else
                            entity = entity.Where(e => e.cdAutorizador.Equals(cdAutorizador)).AsQueryable();
                        break;
                    case CAMPOS.DTAVENDA:
                        //DateTime dtaVenda = Convert.ToDateTime(item.Value);
                        //entity = entity.Where(e => e.dtaVenda.Equals(dtaVenda)).AsQueryable();
                        //break;

                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            //entity = entity.Where(e => e.dtaVenda >= dtaIni && e.dtaVenda <= dtaFim);
                            entity = entity.Where(e => (e.dtaVenda.Year > dtaIni.Year || (e.dtaVenda.Year == dtaIni.Year && e.dtaVenda.Month > dtaIni.Month) ||
                                                                                          (e.dtaVenda.Year == dtaIni.Year && e.dtaVenda.Month == dtaIni.Month && e.dtaVenda.Day >= dtaIni.Day))
                                                    && (e.dtaVenda.Year < dtaFim.Year || (e.dtaVenda.Year == dtaFim.Year && e.dtaVenda.Month < dtaFim.Month) ||
                                                                                          (e.dtaVenda.Year == dtaFim.Year && e.dtaVenda.Month == dtaFim.Month && e.dtaVenda.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaVenda >= dta);
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca;
                            if (item.Value.Length == 8)
                            {
                                string dia = item.Value.Substring(6, 1);
                                string anoMes = item.Value.Substring(0, 6);
                                busca = anoMes + "0" + dia;
                            }
                            else
                            {
                                busca = item.Value.Replace("<", "");
                            }
                            //busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaVenda <= dta);
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaVenda.Year == dtaIni.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaVenda.Year == dtaIni.Year && e.dtaVenda.Month == dtaIni.Month);
                        }
                        else if (item.Value.Length == 7)
                        {
                            string dia = item.Value.Substring(6, 1);
                            string anoMes = item.Value.Substring(0, 6);
                            string busca = anoMes + "0" + dia;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaVenda.Year == dtaIni.Year && e.dtaVenda.Month == dtaIni.Month && e.dtaVenda.Day == dtaIni.Day);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaVenda.Year == dtaIni.Year && e.dtaVenda.Month == dtaIni.Month && e.dtaVenda.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.VALORVENDABRUTA:
                        decimal valorVendaBruta = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorVendaBruta.Equals(valorVendaBruta)).AsQueryable();
                        break;
                    case CAMPOS.VALORVENDALIQUIDA:
                        decimal valorVendaLiquida = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorVendaLiquida.Equals(valorVendaLiquida)).AsQueryable();
                        break;
                    case CAMPOS.LOTEIMPORTACAO:
                        string loteImportacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.loteImportacao.Equals(loteImportacao)).AsQueryable();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable();
                        break;
                    case CAMPOS.IDLOGICOTERMINAL:
                        Int32 idLogicoTerminal = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogicoTerminal.Equals(idLogicoTerminal)).AsQueryable();
                        break;
                    case CAMPOS.CODTITULOERP:
                        string codTituloERP = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.codTituloERP.Equals(codTituloERP)).AsQueryable();
                        break;
                    case CAMPOS.CODVENDAERP:
                        string codVendaERP = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.codVendaERP.Equals(codVendaERP)).AsQueryable();
                        break;
                    case CAMPOS.CODRESUMOVENDA:
                        string codResumoVenda = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.codResumoVenda.Equals(codResumoVenda)).AsQueryable();
                        break;
                    case CAMPOS.NUMPARCELATOTAL:
                        Int32 numParcelaTotal = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.numParcelaTotal.Equals(numParcelaTotal)).AsQueryable();
                        break;


                    // PERSONALIZADO

                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.BandeiraPos.Operadora.id == idOperadora).AsQueryable();
                        break;
                    case CAMPOS.NMOPERADORA:
                        string nmOperadora = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.BandeiraPos.Operadora.nmOperadora.Equals(nmOperadora)).AsQueryable();
                        break;

                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.empresa.id_grupo == id_grupo).AsQueryable();
                        break;
                    case CAMPOS.DS_FANTASIA:
                        string dsfantasia = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.empresa.ds_fantasia.Equals(dsfantasia)).AsQueryable();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable();
                    break;
                case CAMPOS.NSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nsu).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nsu).AsQueryable();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizador).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdAutorizador).AsQueryable();
                    break;
                case CAMPOS.DTAVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaVenda).ThenBy(e => e.empresa.ds_fantasia).ThenBy(e => e.empresa.filial).ThenBy(e => e.BandeiraPos.desBandeira).ThenBy(e => e.TerminalLogico.dsTerminalLogico).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtaVenda).ThenBy(e => e.empresa.ds_fantasia).ThenBy(e => e.empresa.filial).ThenBy(e => e.BandeiraPos.desBandeira).ThenBy(e => e.TerminalLogico.dsTerminalLogico).AsQueryable();
                    break;
                case CAMPOS.VALORVENDABRUTA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorVendaBruta).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.valorVendaBruta).AsQueryable();
                    break;
                case CAMPOS.VALORVENDALIQUIDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorVendaLiquida).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.valorVendaLiquida).AsQueryable();
                    break;
                case CAMPOS.LOTEIMPORTACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.loteImportacao).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.loteImportacao).AsQueryable();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable();
                    break;
                case CAMPOS.IDLOGICOTERMINAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogicoTerminal).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.idLogicoTerminal).AsQueryable();
                    break;
                case CAMPOS.CODTITULOERP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codTituloERP).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.codTituloERP).AsQueryable();
                    break;
                case CAMPOS.CODVENDAERP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codVendaERP).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.codVendaERP).AsQueryable();
                    break;
                case CAMPOS.CODRESUMOVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codResumoVenda).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.codResumoVenda).AsQueryable();
                    break;
                case CAMPOS.NUMPARCELATOTAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numParcelaTotal).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.numParcelaTotal).AsQueryable();
                    break;


                // PERSONALIZADO

                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.BandeiraPos.Operadora.id).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.BandeiraPos.Operadora.id).AsQueryable();
                    break;
                case CAMPOS.NMOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.BandeiraPos.Operadora.nmOperadora).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.BandeiraPos.Operadora.nmOperadora).AsQueryable();
                    break;
                case CAMPOS.DS_FANTASIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.empresa.ds_fantasia).ThenBy(e => e.empresa.filial).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.empresa.ds_fantasia).ThenByDescending(e => e.empresa.filial).AsQueryable();
                    break;
                case CAMPOS.DESBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.BandeiraPos.desBandeira).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.BandeiraPos.desBandeira).AsQueryable();
                    break;
                case CAMPOS.DSTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.TerminalLogico.dsTerminalLogico).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.TerminalLogico.dsTerminalLogico).AsQueryable();
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
            try
            {
                // Implementar o filtro por Grupo apartir do TOKEN do Usuário

                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                        queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (CnpjEmpresa != "")
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.CNPJ, out outValue))
                        queryString["" + (int)CAMPOS.CNPJ] = CnpjEmpresa;
                    else
                        queryString.Add("" + (int)CAMPOS.CNPJ, CnpjEmpresa);
                }


                //DECLARAÇÕES
                List<dynamic> CollectionRecebimento = new List<dynamic>();
                Retorno retorno = new Retorno();
                retorno.Totais = new Dictionary<string, object>();

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
                var queryTotal = query;


                // PAGINAÇÃO
                if (colecao != 3 && colecao != 4 && // relatório terminal lógico e relatório sintético => Por causa do GroupBy
                    colecao != 11 && colecao != 12) // NSUS e Cod Autorizador de todo o filtro (sem paginação)
                {
                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = queryTotal.Count();

                    retorno.Totais.Add("valorVendaBruta", query.Count() > 0 ? Convert.ToDecimal(query.Sum(r => r.valorVendaBruta)) : 0);

                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;
                }
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
                    var subQuery = query
                        .GroupBy(x => new { x.dtaVenda.Year, x.dtaVenda.Month, x.empresa.id_grupo })
                        .Select(e => new
                        {

                            nrAno = e.Key.Year,
                            nmMes = ((MES)e.Key.Month).ToString(),
                            nrMes = e.Key.Month,
                            cdGrupo = e.Key.id_grupo,
                            vlVenda = e.Sum(l => l.valorVendaBruta)
                        });

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

                    CollectionRecebimento = subQuery.OrderByDescending(o => new { o.nrAno, o.nrMes }).ToList<dynamic>();

                }
                else if (colecao == 3) // Portal/RelatorioTerminalLogico
                {
                    var subQuery = query
                    .GroupBy(e => new { e.empresa, e.TerminalLogico, e.BandeiraPos })
                    .OrderBy(e => e.Key.empresa.ds_fantasia)
                    .ThenBy(e => e.Key.empresa.filial)
                    .ThenBy(e => e.Key.TerminalLogico.dsTerminalLogico)
                    .ThenBy(e => e.Key.BandeiraPos.desBandeira)
                    .Select(e => new
                    {
                        empresa = new
                        {
                            nu_cnpj = e.Key.empresa.nu_cnpj,
                            ds_fantasia = e.Key.empresa.ds_fantasia,
                            filial = e.Key.empresa.filial
                        },
                        terminal = new
                        {
                            idTerminalLogico = e.Key.TerminalLogico.idTerminalLogico,
                            dsTerminalLogico = e.Key.TerminalLogico.dsTerminalLogico.Equals("0") ? "-" : e.Key.TerminalLogico.dsTerminalLogico,
                        },
                        idOperadora = e.Key.TerminalLogico.idOperadora,
                        bandeira = new
                        {
                            e.Key.BandeiraPos.id,
                            e.Key.BandeiraPos.desBandeira
                        },
                        totalTransacoes = e.Count(),
                        valorBruto = e.Sum(p => p.valorVendaBruta)

                    });

                    retorno.TotalDeRegistros = subQuery.Count();

                    retorno.Totais.Add("totalTransacoes", subQuery.Count() > 0 ? Convert.ToInt32(subQuery.Sum(r => r.totalTransacoes)) : 0);
                    retorno.Totais.Add("valorBruto", subQuery.Count() > 0 ? Convert.ToDecimal(subQuery.Sum(r => r.valorBruto)) : 0);

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimento = subQuery.ToList<dynamic>();
                }
                else if (colecao == 4) // Portal/RelatorioSintetico
                {
                    var subQuery = query
                     .GroupBy(e => new { e.empresa, e.BandeiraPos })
                     .OrderBy(e => e.Key.empresa.ds_fantasia)
                     .ThenBy(e => e.Key.empresa.filial)
                     .ThenBy(e => e.Key.BandeiraPos.desBandeira)
                     .Select(e => new
                     {
                         empresa = new
                         {
                             nu_cnpj = e.Key.empresa.nu_cnpj,
                             ds_fantasia = e.Key.empresa.ds_fantasia,
                             filial = e.Key.empresa.filial
                         },
                         bandeira = new
                         {
                             e.Key.BandeiraPos.id,
                             e.Key.BandeiraPos.desBandeira
                         },
                         idOperadora = e.Key.BandeiraPos.idOperadora,
                         totalTransacoes = e.Count(),
                         valorBruto = e.Sum(p => p.valorVendaBruta)

                     });

                    retorno.TotalDeRegistros = subQuery.Count();

                    retorno.Totais.Add("totalTransacoes", subQuery.Count() > 0 ? Convert.ToInt32(subQuery.Sum(r => r.totalTransacoes)) : 0);
                    retorno.Totais.Add("valorBruto", subQuery.Count() > 0 ? Convert.ToDecimal(subQuery.Sum(r => r.valorBruto)) : 0);

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimento = subQuery.ToList<dynamic>();
                }
                else if (colecao == 5) // Portal/RelatorioAnalitico
                {
                    CollectionRecebimento = query

                     .Select(e => new
                     {
                         e.cnpj,
                         e.dtaVenda,
                         dsFantasia = e.empresa.ds_fantasia + (e.empresa.filial != null ? e.empresa.filial : ""),
                         dsTerminalLogico = e.TerminalLogico.dsTerminalLogico.Equals("0") ? "-" : e.TerminalLogico.dsTerminalLogico,
                         e.BandeiraPos.desBandeira,
                         e.nsu,
                         e.cdAutorizador,
                         e.valorVendaBruta
                     }).ToList<dynamic>();


                }
                else if (colecao == 6) // [mobile]/Vendas/Tempo
                {
                    var subQuery = query
                        .GroupBy(x => new { x.dtaVenda.Day, x.empresa.id_grupo })
                        .Select(e => new
                        {

                            nrDia = e.Key.Day,
                            cdGrupo = e.Key.id_grupo,
                            //nrCNPJ = e.Key.cnpj,
                            vlVenda = e.Sum(l => l.valorVendaBruta)
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;

                    CollectionRecebimento = subQuery.OrderBy(o => o.nrDia).ToList<dynamic>();

                }
                else if (colecao == 7) // [mobile]/Vendas/Adquirente
                {
                    var subQuery = query
                        .GroupBy(x => new { x.empresa.id_grupo, x.BandeiraPos.Operadora.nmOperadora })
                        .Select(e => new
                        {

                            cdGrupo = e.Key.id_grupo,
                            //nrCNPJ = e.Key.cnpj,
                            //idAdquirente = e.Key.id,
                            dsAdquirente = e.Key.nmOperadora,
                            vlVenda = e.Sum(l => l.valorVendaBruta)
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;

                    CollectionRecebimento = subQuery.ToList<dynamic>();

                }
                else if (colecao == 8) // [mobile]/Vendas/Adquirente/tempo
                {
                    var subQuery = query
                        .GroupBy(x => new { x.dtaVenda.Day, x.BandeiraPos.Operadora.nmOperadora })
                        .Select(e => new
                        {

                            nrDia = e.Key.Day,
                            //cdGrupo = e.Key.id_grupo,
                            //nrCNPJ = e.Key.cnpj,
                            //idAdquirente = e.Key.id,
                            dsAdquirente = e.Key.nmOperadora,
                            vlVenda = e.Sum(l => l.valorVendaBruta)
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;

                    CollectionRecebimento = subQuery.OrderBy(o => o.nrDia).ToList<dynamic>();

                }
                else if (colecao == 9) // [mobile]/Filial/Tempo
                {
                    var subQuery = query
                        .GroupBy(x => new { x.empresa.id_grupo, x.empresa })
                        .OrderBy(e => e.Key.empresa.ds_fantasia)
                        .Select(e => new
                        {

                            nmNome = e.Key.empresa.ds_fantasia,
                            cdGrupo = e.Key.id_grupo,
                            nrCNPJ = e.Key.empresa.nu_cnpj,
                            vlVenda = e.Sum(l => l.valorVendaBruta)
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;

                    CollectionRecebimento = subQuery.ToList<dynamic>();

                }
                else if (colecao == 10) // [mobile]/Filial/Tempo
                {
                    var subQuery = query
                        .GroupBy(x => new { x.dtaVenda.Day, x.empresa.id_grupo })
                        .Select(e => new
                        {

                            nrDia = e.Key.Day,
                            cdGrupo = e.Key.id_grupo,
                            //nrCNPJ = e.Key.cnpj,
                            vlVenda = e.Sum(l => l.valorVendaBruta)
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;

                    CollectionRecebimento = subQuery.OrderBy(o => o.nrDia).ToList<dynamic>();

                }
                else if (colecao == 11) // Portal/RelatorioAnalitico => Listagem dos NSUs
                {
                    CollectionRecebimento = query
                     .OrderBy(e => e.nsu)
                     .Select(e => e.nsu)
                     .ToList<dynamic>();

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = CollectionRecebimento.Count;
                }
                else if (colecao == 12) // Portal/RelatorioAnalitico => Listagem dos cod. autorizador
                {
                    CollectionRecebimento = query
                     .OrderBy(e => e.cdAutorizador)
                     .Select(e => e.cdAutorizador)
                     .ToList<dynamic>();

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = CollectionRecebimento.Count;
                }


                retorno.Registros = CollectionRecebimento;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar recebimento" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova Recebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Recebimento param)
        {
            try
            {
                _db.Recebimentoes.Add(param);
                _db.SaveChanges();
                return param.id;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar recebimento" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma Recebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            try
            {
                _db.Recebimentoes.Remove(_db.Recebimentoes.Where(e => e.id.Equals(id)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar recebimento" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera Recebimento
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Recebimento param)
        {
            try
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
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar recebimento" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
