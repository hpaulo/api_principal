using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;
using System.Globalization;

namespace api.Negocios.Tax
{
    public class GatewayTbManifesto
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbManifesto()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDMANIFESTO = 100,
            NRCHAVE = 101,
            NRNSU = 102,
            CDGRUPO = 103,
            NRCNPJ = 104,
            NREMITENTECNPJCPF = 105,
            NMEMITENTE = 106,
            NREMITENTEIE = 107,
            DTEMISSAO = 108,
            TPOPERACAO = 109,
            VLNFE = 110,
            DTRECEBIMENTO = 111,
            CDSITUACAONFE = 112,
            CDSITUACAOMANIFESTO = 113,
            DSSITUACAOMANIFESTO = 114,
            NRPROTOCOLOMANIFESTO = 115,
            XMLNFE = 116,
            NRPROTOCOLODOWNLOAD = 117,
            CDSITUACAODOWNLOAD = 118,
            DSSITUACAODOWNLOAD = 119,

        };

        /// <summary>
        /// Get TbManifesto/TbManifesto
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbManifesto> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbManifestos.AsQueryable<tbManifesto>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDMANIFESTO:
                        Int32 idManifesto = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idManifesto.Equals(idManifesto)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.NRCHAVE:
                        string nrChave = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrChave.Equals(nrChave)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.NRNSU:
                        string nrNSU = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrNSU.Equals(nrNSU)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.CDGRUPO:
                        Int32 cdGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdGrupo.Equals(cdGrupo)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.NREMITENTECNPJCPF:
                        string nrEmitenteCNPJCPF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrEmitenteCNPJCPF.Equals(nrEmitenteCNPJCPF)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.NMEMITENTE:
                        string nmEmitente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmEmitente.Equals(nmEmitente)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.NREMITENTEIE:
                        string nrEmitenteIE = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrEmitenteIE.Equals(nrEmitenteIE)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.DTEMISSAO:
                        //DateTime dtEmissao = DateTime.ParseExact(item.Value + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture); //Convert.ToDateTime(item.Value);
                        //entity = entity.Where(e => e.dtEmissao.Equals(dtEmissao)).AsQueryable<tbManifesto>();

                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            //entity = entity.Where(e => e.dtaVenda >= dtaIni && e.dtaVenda <= dtaFim);
                            entity = entity.Where(e => e.dtEmissao != null && (e.dtEmissao.Value.Year > dtaIni.Year || (e.dtEmissao.Value.Year == dtaIni.Year && e.dtEmissao.Value.Month > dtaIni.Month) ||
                                                                                          (e.dtEmissao.Value.Year == dtaIni.Year && e.dtEmissao.Value.Month == dtaIni.Month && e.dtEmissao.Value.Day >= dtaIni.Day))
                                                    && (e.dtEmissao.Value.Year < dtaFim.Year || (e.dtEmissao.Value.Year == dtaFim.Year && e.dtEmissao.Value.Month < dtaFim.Month) ||
                                                                                          (e.dtEmissao.Value.Year == dtaFim.Year && e.dtEmissao.Value.Month == dtaFim.Month && e.dtEmissao.Value.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtEmissao >= dta);
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
                            entity = entity.Where(e => e.dtEmissao <= dta);
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtEmissao != null && e.dtEmissao.Value.Year == dtaIni.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtEmissao != null && e.dtEmissao.Value.Year == dtaIni.Year && e.dtEmissao.Value.Month == dtaIni.Month);
                        }
                        else if (item.Value.Length == 7)
                        {
                            string dia = item.Value.Substring(6, 1);
                            string anoMes = item.Value.Substring(0, 6);
                            string busca = anoMes + "0" + dia;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtEmissao != null && e.dtEmissao.Value.Year == dtaIni.Year && e.dtEmissao.Value.Month == dtaIni.Month && e.dtEmissao.Value.Day == dtaIni.Day);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtEmissao != null && e.dtEmissao.Value.Year == dtaIni.Year && e.dtEmissao.Value.Month == dtaIni.Month && e.dtEmissao.Value.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.TPOPERACAO:
                        string tpOperacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tpOperacao.Equals(tpOperacao)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.VLNFE:
                        decimal vlNFe = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlNFe.Equals(vlNFe)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.DTRECEBIMENTO:
                        DateTime dtRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtRecebimento.Equals(dtRecebimento)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.CDSITUACAONFE:
                        string cdSituacaoNFe = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdSituacaoNFe.Equals(cdSituacaoNFe)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.CDSITUACAOMANIFESTO:
                        short cdSituacaoManifesto = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdSituacaoManifesto.Equals(cdSituacaoManifesto)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.DSSITUACAOMANIFESTO:
                        string dsSituacaoManifesto = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsSituacaoManifesto.Equals(dsSituacaoManifesto)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.NRPROTOCOLOMANIFESTO:
                        string nrProtocoloManifesto = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrProtocoloManifesto.Equals(nrProtocoloManifesto)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.XMLNFE:
                        string xmlNFe = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.xmlNFe.Equals(xmlNFe)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.NRPROTOCOLODOWNLOAD:
                        string nrProtocoloDownload = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrProtocoloDownload.Equals(nrProtocoloDownload)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.CDSITUACAODOWNLOAD:
                        short cdSituacaoDownload = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdSituacaoDownload.Equals(cdSituacaoDownload)).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.DSSITUACAODOWNLOAD:
                        string dsSituacaoDownload = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsSituacaoDownload.Equals(dsSituacaoDownload)).AsQueryable<tbManifesto>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDMANIFESTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idManifesto).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.idManifesto).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.NRCHAVE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrChave).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.nrChave).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.NRNSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrNSU).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.nrNSU).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdGrupo).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.cdGrupo).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.NREMITENTECNPJCPF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrEmitenteCNPJCPF).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.nrEmitenteCNPJCPF).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.NMEMITENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmEmitente).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.nmEmitente).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.NREMITENTEIE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrEmitenteIE).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.nrEmitenteIE).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.DTEMISSAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtEmissao).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.dtEmissao).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.TPOPERACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tpOperacao).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.tpOperacao).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.VLNFE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlNFe).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.vlNFe).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.DTRECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtRecebimento).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.dtRecebimento).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.CDSITUACAONFE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSituacaoNFe).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.cdSituacaoNFe).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.CDSITUACAOMANIFESTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSituacaoManifesto).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.cdSituacaoManifesto).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.DSSITUACAOMANIFESTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsSituacaoManifesto).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.dsSituacaoManifesto).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.NRPROTOCOLOMANIFESTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrProtocoloManifesto).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.nrProtocoloManifesto).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.XMLNFE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.xmlNFe).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.xmlNFe).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.NRPROTOCOLODOWNLOAD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrProtocoloDownload).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.nrProtocoloDownload).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.CDSITUACAODOWNLOAD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSituacaoDownload).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.cdSituacaoDownload).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.DSSITUACAODOWNLOAD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsSituacaoDownload).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.dsSituacaoDownload).AsQueryable<tbManifesto>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbManifesto/TbManifesto
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbManifesto = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));


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
                if (colecao == 1) // [iTAX] Consulta a última NSU do CNPJ informado
                {
                    CollectionTbManifesto = query
                        .GroupBy(e => new { e.nrCNPJ })
                        .Select(e => new
                    {
                            nrCNPJ = e.Key.nrCNPJ,
                            ultNSU = e.Max(m => m.nrNSU),
                        }).ToList<dynamic>();

                }
                else if (colecao == 0) //  [iTAX] 
                {
                    CollectionTbManifesto = query.Select(e => new
                    {

                        idManifesto = e.idManifesto,
                        nrChave = e.nrChave,
                        nrNSU = e.nrNSU,
                        cdGrupo = e.cdGrupo,
                        nrCNPJ = e.nrCNPJ,
                        nrEmitenteCNPJCPF = e.nrEmitenteCNPJCPF,
                        nmEmitente = e.nmEmitente,
                        nrEmitenteIE = e.nrEmitenteIE,
                        dtEmissao = e.dtEmissao,
                        tpOperacao = e.tpOperacao,
                        vlNFe = e.vlNFe,
                        dtRecebimento = e.dtRecebimento,
                        cdSituacaoNFe = e.cdSituacaoNFe,
                        cdSituacaoManifesto = e.cdSituacaoManifesto,
                        dsSituacaoManifesto = e.dsSituacaoManifesto,
                        nrProtocoloManifesto = e.nrProtocoloManifesto,
                        xmlNFe = e.xmlNFe,
                        nrProtocoloDownload = e.nrProtocoloDownload,
                        cdSituacaoDownload = e.cdSituacaoDownload,
                        dsSituacaoDownload = e.dsSituacaoDownload,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2) // [iTAX] Consulta as notas disponíveis para manifestação
                {
                    CollectionTbManifesto = _db.tbManifestos
                        .Join(_db.tbEmpresaFiliais, m => m.nrCNPJ, f => f.nrCNPJ, (m, f) => new { m, f })
                        .Join(_db.tbEmpresas, j => j.f.nrCNPJBase, e => e.nrCNPJBase, (j, e) => new { j, e })
                        .Where(e => e.j.m.cdSituacaoManifesto == null && e.j.m.cdSituacaoDownload == null)
                        .Select(e => new
                        {
                            idManifesto = e.j.m.idManifesto,
                            cdGrupo = e.j.m.cdGrupo,
                            nrChave = e.j.m.nrChave,
                            nrCNPJ = e.j.m.nrCNPJ,
                            dsCertificadoDigital = e.e.dsCertificadoDigital,
                            dsCertificadoDigitalSenha =e.e.dsCertificadoDigitalSenha,
                        }).ToList<dynamic>();
                }
                else if (colecao == 3) // [iTAX] Consulta as notas disponíveis para download
                {
                    CollectionTbManifesto = _db.tbManifestos
                        .Join(_db.tbEmpresaFiliais, m => m.nrCNPJ, f => f.nrCNPJ, (m, f) => new { m, f })
                        .Join(_db.tbEmpresas, j => j.f.nrCNPJBase, e => e.nrCNPJBase, (j, e) => new { j, e })
                        .Where(e => (e.j.m.cdSituacaoManifesto == 135 || e.j.m.cdSituacaoManifesto == 573 || e.j.m.cdSituacaoManifesto == 655) && e.j.m.cdSituacaoDownload == null)
                        .Select(e => new
                        {
                            idManifesto = e.j.m.idManifesto,
                            cdGrupo = e.j.m.cdGrupo,
                            nrChave = e.j.m.nrChave,
                            nrCNPJ = e.j.m.nrCNPJ,
                            dsCertificadoDigital = e.e.dsCertificadoDigital,
                            dsCertificadoDigitalSenha = e.e.dsCertificadoDigitalSenha,
                        }).ToList<dynamic>();
                }
                else if (colecao == 4) // [PORTAL] Consulta NFe Completa NFe to JSON
                {
                    /*
                    List<dynamic> lista = new List<dynamic>();

                    CollectionTbManifesto = query.Select(e => new
                    {
                        idManifesto = e.idManifesto,
                        nrChave = e.nrChave,
                        nrNSU = e.nrNSU,
                        cdGrupo = e.cdGrupo,
                        nrCNPJ = e.nrCNPJ,
                        nrEmitenteCNPJCPF = e.nrEmitenteCNPJCPF,
                        nmEmitente = e.nmEmitente,
                        nrEmitenteIE = e.nrEmitenteIE,
                        dtEmissao = e.dtEmissao,
                        tpOperacao = e.tpOperacao,
                        vlNFe = e.vlNFe,
                        dtRecebimento = e.dtRecebimento,
                        cdSituacaoNFe = e.cdSituacaoNFe,
                        cdSituacaoManifesto = e.cdSituacaoManifesto,
                        dsSituacaoManifesto = e.dsSituacaoManifesto,
                        nrProtocoloManifesto = e.nrProtocoloManifesto,
                        xmlNFe = e.xmlNFe,
                        nrProtocoloDownload = e.nrProtocoloDownload,
                        cdSituacaoDownload = e.cdSituacaoDownload,
                        dsSituacaoDownload = e.dsSituacaoDownload,
                    }).ToList<dynamic>();

                    foreach (var item in CollectionTbManifesto)
                    {
                        NFe.ConvertTxt.NFe xmlNFe = Bibliotecas.nfeRead.Loader(item.xmlNFe);
                        var nf = new
                        {

                            idManifesto = item.idManifesto,
                            nrChave = item.nrChave,
                            nrNSU = item.nrNSU,
                            cdGrupo = item.cdGrupo,
                            nrCNPJ = item.nrCNPJ,
                            nrEmitenteCNPJCPF = item.nrEmitenteCNPJCPF,
                            nmEmitente = item.nmEmitente,
                            nrEmitenteIE = item.nrEmitenteIE,
                            dtEmissao = item.dtEmissao,
                            tpOperacao = item.tpOperacao,
                            vlNFe = item.vlNFe,
                            dtRecebimento = item.dtRecebimento,
                            cdSituacaoNFe = item.cdSituacaoNFe,
                            cdSituacaoManifesto = item.cdSituacaoManifesto,
                            dsSituacaoManifesto = item.dsSituacaoManifesto,
                            nrProtocoloManifesto = item.nrProtocoloManifesto,
                            xmlNFe = xmlNFe,
                            nrProtocoloDownload = item.nrProtocoloDownload,
                            cdSituacaoDownload = item.cdSituacaoDownload,
                            dsSituacaoDownload = item.dsSituacaoDownload
                        };

                        lista.Add(nf);
                    }

                    CollectionTbManifesto.Clear();
                    CollectionTbManifesto = lista;
                    */

                    List<dynamic> lista = new List<dynamic>();

                    CollectionTbManifesto = query
                        .GroupBy(e => new { e.nrEmitenteCNPJCPF, e.nmEmitente })
                        .OrderBy(e => e.Key.nmEmitente)
                        .Select(e => new
                        {
                            nrEmitenteCNPJCPF = e.Key.nrEmitenteCNPJCPF,
                            nmEmitente = e.Key.nmEmitente,
                            UF = "",
                            notas = e.Select(x => new
                            {

                                dtEmissao = x.dtEmissao,
                                //modelo = 0,
                                //numero = 0,
                                //serie = 0,
                                vlNFe = x.vlNFe,
                                nrChave = x.nrChave,
                                //nfe = "",
                                dsSituacaoManifesto = x.dsSituacaoManifesto,
                                dsSituacaoErp = "Não Importado",
                                xmlNFe = x.xmlNFe

                            }).OrderBy(x => x.dtEmissao).ToList<dynamic>()
                        }).ToList<dynamic>();


                    foreach (var item in CollectionTbManifesto)
                    {
                        //NFe.ConvertTxt.NFe xmlNFe = Bibliotecas.nfeRead.Loader(item.notas[0].xmlNFe);


                        List<dynamic> n = new List<dynamic>();
                        string uf = String.Empty;
                        foreach (var notas in item.notas)
                        {
                            NFe.ConvertTxt.NFe xmlNFe = Bibliotecas.nfeRead.Loader(notas.xmlNFe);
                            uf = xmlNFe.emit.enderEmit.UF;
                            var e = new
                            {
                                /*dtEmissao = notas.dtEmissao,

                                mod = xmlNFe.ide.mod,
                                serie = xmlNFe.ide.serie,
                                nNF = xmlNFe.ide.nNF,
                                vlNFe = notas.vlNFe,
                                nrChave = notas.nrChave,
                                nfe = xmlNFe.protNFe.xMotivo,
                                dsSituacaoManifesto = notas.dsSituacaoManifesto,
                                dsSituacaoErp = notas.dsSituacaoErp*/
                                xmlNFe
                            };

                            n.Add(e);
                        }

                        var nf = new
                        {
                            nrEmitenteCNPJCPF = item.nrEmitenteCNPJCPF,
                            nmEmitente = item.nmEmitente,
                            UF = uf,
                            notas = n
                        };

                        lista.Add(nf);
                    }

                    CollectionTbManifesto.Clear();
                    CollectionTbManifesto = lista;



                }
                else if (colecao == 5) // [PORTAL] Consulta NFe Completa NFe to JSON
                {
                    List<dynamic> lista = new List<dynamic>();

                    CollectionTbManifesto = query
                        .GroupBy(e => new { e.nrEmitenteCNPJCPF, e.nmEmitente })
                        .OrderBy(e => e.Key.nmEmitente)
                        .Select(e => new
                    {
                        nrEmitenteCNPJCPF = e.Key.nrEmitenteCNPJCPF,
                        nmEmitente = e.Key.nmEmitente,
                        UF = "",
                        notas = e.Select(x => new { 
                        
                            dtEmissao = x.dtEmissao,
                            //modelo = 0,
                            //numero = 0,
                            //serie = 0,
                            vlNFe = x.vlNFe,
                            nrChave = x.nrChave,
                            //nfe = "",
                            dsSituacaoManifesto = x.dsSituacaoManifesto,
                            dsSituacaoErp = "Não Importado",
                            xmlNFe = x.xmlNFe
                        
                        } ).OrderBy(x => x.dtEmissao).ToList<dynamic>()
                    }).ToList<dynamic>();
                        
                    
                    foreach (var item in CollectionTbManifesto)
                    {
                        //NFe.ConvertTxt.NFe xmlNFe = Bibliotecas.nfeRead.Loader(item.notas[0].xmlNFe);


                        List<dynamic> n = new List<dynamic>();
                        string uf = String.Empty;
                        foreach (var notas in item.notas)
                        {
                            NFe.ConvertTxt.NFe xmlNFe = Bibliotecas.nfeRead.Loader(notas.xmlNFe);
                            uf = xmlNFe.emit.enderEmit.UF;
                            var e = new
                            {
                                dtEmissao = notas.dtEmissao,
                                
                                mod = xmlNFe.ide.mod,
                                serie = xmlNFe.ide.serie,
                                nNF = xmlNFe.ide.nNF,
                                vlNFe = notas.vlNFe,
                                nrChave = notas.nrChave,
                                nfe = xmlNFe.protNFe.xMotivo,
                                dsSituacaoManifesto = notas.dsSituacaoManifesto,
                                dsSituacaoErp = notas.dsSituacaoErp
                            };

                            n.Add(e);
                        }

                        var nf = new
                        {
                            nrEmitenteCNPJCPF = item.nrEmitenteCNPJCPF,
                            nmEmitente = item.nmEmitente,
                            UF = uf,
                            notas = n
                        };

                        lista.Add(nf);
                    }

                    CollectionTbManifesto.Clear();
                    CollectionTbManifesto = lista;


                }

                retorno.TotalDeRegistros = CollectionTbManifesto.Count();
                retorno.Registros = CollectionTbManifesto;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbManifesto
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbManifesto param)
        {
            try
            {    // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbManifestos.Add(param);
                _db.SaveChanges();
                return param.idManifesto;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbManifesto
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idManifesto)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbManifesto manifesto = _db.tbManifestos.Where(e => e.idManifesto == idManifesto).FirstOrDefault();

                if(manifesto == null) throw new Exception("Manifesto inexistente!");

                _db.tbManifestos.Remove(manifesto);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbManifesto
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbManifesto param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbManifesto value = _db.tbManifestos
                        .Where(e => e.idManifesto == param.idManifesto)
                        .First<tbManifesto>();

                if (value == null) throw new Exception("Manifesto inexistente!");


                if (param.nrChave != null && param.nrChave != value.nrChave)
                    value.nrChave = param.nrChave;
                if (param.nrNSU != null && param.nrNSU != value.nrNSU)
                    value.nrNSU = param.nrNSU;
                if (param.cdGrupo != 0 && param.cdGrupo != value.cdGrupo)
                    value.cdGrupo = param.cdGrupo;
                if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                    value.nrCNPJ = param.nrCNPJ;
                if (param.nrEmitenteCNPJCPF != null && param.nrEmitenteCNPJCPF != value.nrEmitenteCNPJCPF)
                    value.nrEmitenteCNPJCPF = param.nrEmitenteCNPJCPF;
                if (param.nmEmitente != null && param.nmEmitente != value.nmEmitente)
                    value.nmEmitente = param.nmEmitente;
                if (param.nrEmitenteIE != null && param.nrEmitenteIE != value.nrEmitenteIE)
                    value.nrEmitenteIE = param.nrEmitenteIE;
                if (param.dtEmissao != null && param.dtEmissao != value.dtEmissao)
                    value.dtEmissao = param.dtEmissao;
                if (param.tpOperacao != null && param.tpOperacao != value.tpOperacao)
                    value.tpOperacao = param.tpOperacao;
                if (param.vlNFe != null && param.vlNFe != value.vlNFe)
                    value.vlNFe = param.vlNFe;
                if (param.dtRecebimento != null && param.dtRecebimento != value.dtRecebimento)
                    value.dtRecebimento = param.dtRecebimento;
                if (param.cdSituacaoNFe != null && param.cdSituacaoNFe != value.cdSituacaoNFe)
                    value.cdSituacaoNFe = param.cdSituacaoNFe;
                if (param.cdSituacaoManifesto != null && param.cdSituacaoManifesto != value.cdSituacaoManifesto)
                    value.cdSituacaoManifesto = param.cdSituacaoManifesto;
                if (param.dsSituacaoManifesto != null && param.dsSituacaoManifesto != value.dsSituacaoManifesto)
                    value.dsSituacaoManifesto = param.dsSituacaoManifesto;
                if (param.nrProtocoloManifesto != null && param.nrProtocoloManifesto != value.nrProtocoloManifesto)
                    value.nrProtocoloManifesto = param.nrProtocoloManifesto;
                if (param.xmlNFe != null && param.xmlNFe != value.xmlNFe)
                    value.xmlNFe = param.xmlNFe;
                if (param.nrProtocoloDownload != null && param.nrProtocoloDownload != value.nrProtocoloDownload)
                    value.nrProtocoloDownload = param.nrProtocoloDownload;
                if (param.cdSituacaoDownload != null && param.cdSituacaoDownload != value.cdSituacaoDownload)
                    value.cdSituacaoDownload = param.cdSituacaoDownload;
                if (param.dsSituacaoDownload != null && param.dsSituacaoDownload != value.dsSituacaoDownload)
                    value.dsSituacaoDownload = param.dsSituacaoDownload;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }
    }
}
