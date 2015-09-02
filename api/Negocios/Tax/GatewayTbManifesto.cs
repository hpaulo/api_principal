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
using NFe.ConvertTxt;
using NFe.Components;
using System.Threading;

namespace api.Negocios.Tax
{
    public class GatewayTbManifesto
    {
        static Semaphore semaforo = new Semaphore(1,1);
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
            DTENTREGA = 120,
            IDUSERS = 121,

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
                        if (nmEmitente.Contains("%")) // usa LIKE
                        {
                            string busca = nmEmitente.Replace("%", "").ToString();
                            entity = entity.Where(e => e.nmEmitente.Contains(busca));
                        }else
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
                    case CAMPOS.DTENTREGA:
                        string buscaDtEntrega = item.Value;
                        DateTime dtEntrega = DateTime.ParseExact(buscaDtEntrega + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        entity = entity.Where(e => e.dtEntrega != null && e.dtEntrega.Value.Year == dtEntrega.Year && e.dtEntrega.Value.Month == dtEntrega.Month && e.dtEntrega.Value.Day == dtEntrega.Day).AsQueryable<tbManifesto>();
                        break;
                    case CAMPOS.IDUSERS:
                        int idUsers = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idUsers == idUsers).AsQueryable<tbManifesto>();
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
                    if (orderby == 0)
                    {
                        entity = entity.OrderBy(e => e.dtEmissao.Value.Year)
                                       .ThenBy(e => e.dtEmissao.Value.Month)
                                       .ThenBy(e => e.dtEmissao.Value.Day)
                                       .ThenBy(e => e.nmEmitente)
                                       .AsQueryable<tbManifesto>();
                    }
                    else
                    {
                        entity = entity.OrderByDescending(e => e.dtEmissao.Value.Year)
                                       .ThenByDescending(e => e.dtEmissao.Value.Month)
                                       .ThenByDescending(e => e.dtEmissao.Value.Day)
                                       .ThenBy(e => e.nmEmitente)
                                       .AsQueryable<tbManifesto>();
                    }
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
                case CAMPOS.DTENTREGA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtEntrega).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.dtEntrega).AsQueryable<tbManifesto>();
                    break;
                case CAMPOS.IDUSERS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idUsers).AsQueryable<tbManifesto>();
                    else entity = entity.OrderByDescending(e => e.idUsers).AsQueryable<tbManifesto>();
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
                semaforo.WaitOne();
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                semaforo.Release(1);

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
                
                // Só interessa os registros que tem XML
                if(colecao != 0)
                query = query.Where(e => e.xmlNFe != null).AsQueryable<tbManifesto>();

 
                if (colecao != 4 && colecao != 5)
                {
                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = query.Count();
                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;
                }

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
                else if (colecao == 0) //  [PORTAL] Utilizado para Consulta de NFe GatewayUtilNfe
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
                        dtEntrega = e.dtEntrega,
                        idUsers = e.idUsers,
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
                    CollectionTbManifesto = query
                        .GroupBy(e => new { e.nrEmitenteCNPJCPF, e.nmEmitente })
                        .OrderBy(e => e.Key.nmEmitente)
                        //.ThenBy(e => e.Count())
                        .Select(e => new
                        {
                            nrEmitenteCNPJCPF = e.Key.nrEmitenteCNPJCPF,
                            nmEmitente = e.Key.nmEmitente,
                            UF = "",
                            notas = e.Select(x => new
                            {
                                idManifesto = x.idManifesto,
                                dtEmissao = x.dtEmissao,
                                vlNFe = x.vlNFe,
                                xmlNFe = x.xmlNFe

                            })
                            .OrderBy(x => x.dtEmissao.Value.Year)
                            .ThenBy(x => x.dtEmissao.Value.Month)
                            .ThenBy(x => x.dtEmissao.Value.Day)
                            .ToList<dynamic>()
                        }).ToList<dynamic>();

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = CollectionTbManifesto.Count;

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        CollectionTbManifesto = CollectionTbManifesto.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;

                    List<dynamic> lista = new List<dynamic>();
                    #region OBTÉM DETALHES DE CADA NOTA
                    foreach (var item in CollectionTbManifesto)
                    {
                        //NFe.ConvertTxt.NFe xmlNFe = Bibliotecas.nfeRead.Loader(item.notas[0].xmlNFe);
                        List<dynamic> notas = new List<dynamic>();
                        string uf = String.Empty;
                        foreach (var nota in item.notas)
                        {
                            NFe.ConvertTxt.NFe xmlNFe = Bibliotecas.nfeRead.Loader(nota.xmlNFe);
                            uf = xmlNFe.emit.enderEmit.UF;

                            notas.Add(new
                            {
                                idManifesto = nota.idManifesto,
                                #region NFE
                                nfe = new
                                {
                                    #region INFO
                                    info = new
                                    {
                                        ID = xmlNFe.infNFe.ID,
                                        versao = xmlNFe.infNFe.Versao,
                                    },
                                    #endregion
                                    modelo = (int)xmlNFe.ide.mod,
                                    serie = xmlNFe.ide.serie,
                                    numero = xmlNFe.ide.nNF,
                                    dtEmissao = nota.dtEmissao,//xmlNFe.ide.dEmi,
                                    dtSaiEnt = xmlNFe.ide.dSaiEnt,
                                    vlNFe = nota.vlNFe,
                                    #region FORMATO DE IMPRESSÃO DO DANFE
                                    formatoImpressaoDANFE = new
                                    {
                                        codigo = (int)xmlNFe.ide.tpImp,
                                        descricao = xmlNFe.ide.tpImp.Equals(TpcnTipoImpressao.tiNao) ? "Sem geração de DANFE" :
                                                    xmlNFe.ide.tpImp.Equals(TpcnTipoImpressao.tiRetrato) ? "DANFE normal, Retrato" :
                                                    xmlNFe.ide.tpImp.Equals(TpcnTipoImpressao.tiPaisagem) ? "DANFE normal, Paisagem" :
                                                    xmlNFe.ide.tpImp.Equals(TpcnTipoImpressao.tiDANFESimplificado) ? "DANFE Simplificado" :
                                                    xmlNFe.ide.tpImp.Equals(TpcnTipoImpressao.tiDANFENFCe) ? "DANFE NFC-e" :
                                                    xmlNFe.ide.tpImp.Equals(TpcnTipoImpressao.tiDANFENFCe_em_mensagem_eletrônica) ? "DANFE NFC-e em mensagem eletrônica" : ""
                                    },
                                    #endregion
                                    #region DESTINO DA OPERAÇÃO
                                    destinoOperacao = new
                                    {
                                        codigo = (int)xmlNFe.ide.idDest,
                                        descricao = xmlNFe.ide.idDest.Equals(TpcnDestinoOperacao.doExterior) ? "Operação com exterior" :
                                                    xmlNFe.ide.idDest.Equals(TpcnDestinoOperacao.doInterestadual) ? "Operação interestadual" :
                                                    xmlNFe.ide.idDest.Equals(TpcnDestinoOperacao.doInterna) ? "Operação interna" : "",
                                    },
                                    #endregion
                                    #region CONSUMIDOR FINAL
                                    consumidorFinal = new
                                    {
                                        codigo = (int)xmlNFe.ide.indFinal,
                                        descricao = xmlNFe.ide.indFinal.Equals(TpcnConsumidorFinal.cfNao) ? "Normal" :
                                                    xmlNFe.ide.indFinal.Equals(TpcnConsumidorFinal.cfConsumidorFinal) ? "Consumidor Final" : "",
                                    },
                                    #endregion
                                    #region PRESENÇA COMPRADOR
                                    presencaComprador = new
                                    {
                                        codigo = (int)xmlNFe.ide.indPres,
                                        descricao = xmlNFe.ide.indPres.Equals(TpcnPresencaComprador.pcNao) ? "Não se aplica" :
                                                    xmlNFe.ide.indPres.Equals(TpcnPresencaComprador.pcPresencial) ? "Operação Presencial" :
                                                    xmlNFe.ide.indPres.Equals(TpcnPresencaComprador.pcInternet) ? "Operação não presencial: Internet" :
                                                    xmlNFe.ide.indPres.Equals(TpcnPresencaComprador.pcTeleatendimento) ? "Operação não presencial: teleatendimento" :
                                                    xmlNFe.ide.indPres.Equals(TpcnPresencaComprador.pcOutros) ? "Operação não presencial: outros" : "",
                                    },
                                    #endregion
                                    #region EMISSÃO
                                    emissao = new
                                    {
                                        #region PROCESSO
                                        processo = new
                                        {
                                            codigo = (int)xmlNFe.ide.procEmi,
                                            descricao = xmlNFe.ide.procEmi.Equals(TpcnProcessoEmissao.peAplicativoContribuinte) ? "Emissão de NF-e com aplicativo do contribuinte" :
                                                        xmlNFe.ide.procEmi.Equals(TpcnProcessoEmissao.peAvulsaFisco) ? "Emissão de NF-e avulsa pelo Fisco" :
                                                        xmlNFe.ide.procEmi.Equals(TpcnProcessoEmissao.peAvulsaContribuinte) ? "Emissão de NF-e, pelo contribuinte com seu certificado digital, através do site do Fisco" :
                                                        xmlNFe.ide.procEmi.Equals(TpcnProcessoEmissao.peContribuinteAplicativoFisco) ? "Emissão NF-e pelo contribuinte com o aplicativo fornecido pelo Fisco" : "",
                                        },
                                        #endregion
                                        versaoProcesso = xmlNFe.ide.verProc,
                                        #region TIPO DE EMISSÃO
                                        tipoEmissao = new
                                        {
                                            codigo = (int)xmlNFe.ide.tpEmis,
                                            descricao = xmlNFe.ide.tpEmis.Equals(TipoEmissao.teNormal) ? "Emissão normal (não em contingência)" :
                                                        xmlNFe.ide.tpEmis.Equals(TipoEmissao.teFS) ? "Contingência FS-IA, com impressão do DANFE em formulário de segurança" :
                                                        xmlNFe.ide.tpEmis.Equals(TipoEmissao.teEPECeDPEC) ? "Contingência DPEC (Declaração Prévia de Emissão em Contingência)" :
                                                        xmlNFe.ide.tpEmis.Equals(TipoEmissao.teFSDA) ? "Contingência FS-DA, com impressão do DANFE em formulário de segurança" :
                                                        xmlNFe.ide.tpEmis.Equals(TipoEmissao.teSVCAN) ? "Contingência SVC-AN (SEFAZ Virtual de Contingência do AN)" :
                                                        xmlNFe.ide.tpEmis.Equals(TipoEmissao.teSVCRS) ? "Contingência SVC-RS (SEFAZ Virtual de Contingência do RS)" :
                                                        xmlNFe.ide.tpEmis.Equals(TipoEmissao.teOffLine) ? "Contingência off-line da NFC-e" : "",
                                        },
                                        #endregion
                                        #region FINALIDADE
                                        finalidade = new
                                        {
                                            codigo = (int)xmlNFe.ide.finNFe,
                                            descricao = xmlNFe.ide.finNFe.Equals(TpcnFinalidadeNFe.fnNormal) ? "NF-e normal" :
                                                        xmlNFe.ide.finNFe.Equals(TpcnFinalidadeNFe.fnComplementar) ? "NF-e complementar" :
                                                        xmlNFe.ide.finNFe.Equals(TpcnFinalidadeNFe.fnAjuste) ? "NF-e de ajuste" :
                                                        xmlNFe.ide.finNFe.Equals(TpcnFinalidadeNFe.fnDevolucao) ? "Devolução/Retorno" : ""
                                        },
                                        #endregion
                                        naturezaOperacao = xmlNFe.ide.natOp,
                                        #region TIPO DE OPERAÇÃO
                                        tipoOperacao = new
                                        {
                                            codigo = (int)xmlNFe.ide.tpNF,
                                            descricao = xmlNFe.ide.tpNF.Equals(TpcnTipoNFe.tnEntrada) ? "Entrada" :
                                                        xmlNFe.ide.tpNF.Equals(TpcnTipoNFe.tnSaida) ? "Saída" : ""
                                        },
                                        #endregion
                                        #region FORMA DE PAGAMENTO
                                        formaPagamento = new
                                        {
                                            codigo = (int)xmlNFe.ide.indPag,
                                            descricao = xmlNFe.ide.indPag.Equals(TpcnIndicadorPagamento.ipVista) ? "Pagamento à vista" :
                                                        xmlNFe.ide.indPag.Equals(TpcnIndicadorPagamento.ipPrazo) ? "Pagamento a prazo" :
                                                        xmlNFe.ide.indPag.Equals(TpcnIndicadorPagamento.ipOutras) ? "Outros" : ""
                                        },
                                        #endregion
                                        digestValue = xmlNFe.protNFe.digVal,
                                        /*eventos = new
                                        {
                                            protocolo = xmlNFe.protNFe.nProt,
                                            dataHora = xmlNFe.protNFe.dhRecbto,
                                            //dataHoraAN = ???
                                        }*/
                                    }
                                    #endregion
                                },
                                #endregion
                                #region EMITENTE
                                emitente = new
                                {
                                    razaoSocial = xmlNFe.emit.xNome,
                                    CNPJ = xmlNFe.emit.CNPJ,
                                    CPF = xmlNFe.emit.CPF,
                                    #region ENDEREÇO
                                    endereco = new
                                    {
                                        logradouro = xmlNFe.emit.enderEmit.xLgr,
                                        numero = xmlNFe.emit.enderEmit.nro,
                                        complemento = xmlNFe.emit.enderEmit.xCpl,
                                        bairro = xmlNFe.emit.enderEmit.xBairro,
                                        #region MUNICÍPIO
                                        municipio = new
                                        {
                                            codigo = xmlNFe.emit.enderEmit.cMun,
                                            nome = xmlNFe.emit.enderEmit.xMun,
                                        },
                                        #endregion
                                        UF = xmlNFe.emit.enderEmit.UF,
                                        #region PAÍS
                                        pais = new
                                        {
                                            codigo = xmlNFe.emit.enderEmit.cPais,
                                            nome = xmlNFe.emit.enderEmit.xPais
                                        },
                                        #endregion
                                        cep = xmlNFe.emit.enderEmit.CEP
                                    },
                                    #endregion
                                    telefone = xmlNFe.emit.enderEmit.fone,
                                    inscricaoMunicipal = xmlNFe.emit.IM,
                                    inscricaoEstadual = xmlNFe.emit.IE,
                                    IEST = xmlNFe.emit.IEST,
                                    CNAE = xmlNFe.emit.CNAE,
                                    nomeFantasia = xmlNFe.emit.xFant,
                                    #region CRT
                                    CRT = new
                                    {
                                        codigo = (int)xmlNFe.emit.CRT,
                                        descricao = xmlNFe.emit.CRT.Equals(TpcnCRT.crtSimplesNacional) ? "Simples Nacional" :
                                                    xmlNFe.emit.CRT.Equals(TpcnCRT.crtSimplesExcessoReceita) ? "Simples Nacional, excesso sublimite de receita bruta" :
                                                    xmlNFe.emit.CRT.Equals(TpcnCRT.crtRegimeNormal) ? "Regime normal" : "",
                                    },
                                    #endregion
                                },
                                #endregion
                                #region DESTINATÁRIO
                                destinatario = new
                                {
                                    razaoSocial = xmlNFe.dest.xNome,
                                    CNPJ = xmlNFe.dest.CNPJ,
                                    CPF = xmlNFe.dest.CPF,
                                    #region ENDEREÇO
                                    endereco = new
                                    {
                                        logradouro = xmlNFe.dest.enderDest.xLgr,
                                        numero = xmlNFe.dest.enderDest.nro,
                                        complemento = xmlNFe.dest.enderDest.xCpl,
                                        bairro = xmlNFe.dest.enderDest.xBairro,
                                        #region MUNICÍPIO
                                        municipio = new
                                        {
                                            codigo = xmlNFe.dest.enderDest.cMun,
                                            nome = xmlNFe.dest.enderDest.xMun,
                                        },
                                        #endregion
                                        UF = xmlNFe.dest.enderDest.UF,
                                        #region PAÍS
                                        pais = new
                                        {
                                            codigo = xmlNFe.dest.enderDest.cPais,
                                            nome = xmlNFe.dest.enderDest.xPais
                                        },
                                        #endregion
                                        cep = xmlNFe.dest.enderDest.CEP
                                    },
                                    #endregion
                                    idEstrangeiro = xmlNFe.dest.idEstrangeiro,
                                    telefone = xmlNFe.dest.enderDest.fone,
                                    inscricaoMunicipal = xmlNFe.dest.IM,
                                    inscricaoEstadual = xmlNFe.dest.IE,
                                    #region INDICADOR IE
                                    indicadorIE = new {
                                        codigo = (int)xmlNFe.dest.indIEDest,
                                        descricao = xmlNFe.dest.indIEDest.Equals(TpcnindIEDest.inContribuinte) ? "Contribuinte ICMS (informar a IE do destinatário)" :
                                                    xmlNFe.dest.indIEDest.Equals(TpcnindIEDest.inIsento) ? "Contribuinte isento de Inscrição no cadastro de Contribuintes do ICMS" :
                                                    xmlNFe.dest.indIEDest.Equals(TpcnindIEDest.inNaoContribuinte) ? "Não Contribuinte, que pode ou não possuir Inscrição" : ""
                                    },
                                    #endregion
                                    inscricaoSUFRAMA = xmlNFe.dest.ISUF,
                                    email = xmlNFe.dest.email,
                                },
                                #endregion
                                #region ENTREGA
                                entrega = new
                                {
                                    #region MUNICÍPIO
                                    municipio = new {
                                        codigo = xmlNFe.entrega.cMun,
                                        nome = xmlNFe.entrega.xMun,
                                    },
                                    #endregion
                                    CNPJ = xmlNFe.entrega.CNPJ,
                                    CPF = xmlNFe.entrega.CPF,
                                    logradouro = xmlNFe.entrega.xLgr,
                                    numero = xmlNFe.entrega.nro,
                                    complemento = xmlNFe.entrega.xCpl,
                                    bairro = xmlNFe.entrega.xBairro,
                                    UF = xmlNFe.entrega.UF,
                                },
                                #endregion
                                #region PRODUTOS
                                produtos = xmlNFe.det.Select(x => new
                                {
                                    infoAdicional = x.infAdProd,
                                    #region PRODUTO
                                    produto = new
                                    {
                                        num = x.Prod.nItem,
                                        descricao = x.Prod.xProd,
                                        qtdComercial = x.Prod.qCom,
                                        unidadeComercial = x.Prod.uCom,
                                        valor = x.Prod.vProd,
                                        codigo = x.Prod.cProd,
                                        codNCM = x.Prod.NCM,
                                        codEXTIPI = x.Prod.EXTIPI,
                                        CFOP = x.Prod.CFOP,
                                        outrasDespesas = x.Prod.vOutro,
                                        valorDesconto = x.Prod.vDesc,
                                        valorTotalFrete = x.Prod.vFrete,
                                        valorSeguro = x.Prod.vSeg,
                                        #region INDICADOR COMPOSIÇÃO
                                        indicadorComposicao = new
                                        {
                                            codigo = (int)x.Prod.indTot,
                                            descricao = x.Prod.indTot.Equals(TpcnIndicadorTotal.itNaoSomaTotalNFe) ? "Valor do item não compõe o valor total da NF-e" :
                                                        x.Prod.indTot.Equals(TpcnIndicadorTotal.itSomaTotalNFe) ? "Valor do item compõe o valor total da NF-e" : ""
                                        },
                                        #endregion
                                        codigoEANComercial = x.Prod.cEAN,
                                        unidadeTributaria = x.Prod.uTrib,
                                        qtdTributario = x.Prod.qTrib,
                                        codigoEANTributario = x.Prod.cEANTrib,
                                        valorUnitarioComercializacao = x.Prod.vUnCom,
                                        valorUnitarioTributacao = x.Prod.vUnTrib,
                                        numPedidoCompra = x.Prod.xPed,
                                        itemPedidoCompra = x.Prod.nItemPed,
                                        FCI = x.Prod.nFCI,
                                        //List<Arma> arma;
                                        //Comb comb;
                                        //List<detExport> detExport;
                                        //List<DI> DI;
                                        //List<Med> med;
                                        //string nRECOPI;
                                        //string NVE;
                                        //veicProd veicProd;
                                        //TpcnTipoCampo vUnCom_Tipo;
                                        //TpcnTipoCampo vUnTrib_Tipo;
                                    },
                                    #endregion
                                    #region IMPOSTO
                                    imposto = new
                                    {
                                        valorAproximadoTributos = x.Imposto.vTotTrib,
                                        #region ICMS
                                        ICMS = new
                                        {
                                            origem = new
                                            {
                                                codigo = (int)x.Imposto.ICMS.orig,
                                                descricao = x.Imposto.ICMS.orig.Equals(TpcnOrigemMercadoria.oeNacional) ? "Nacional" :
                                                            x.Imposto.ICMS.orig.Equals(TpcnOrigemMercadoria.oeEstrangeiraImportacaoDireta) ? "Estrangeira - Importação direta" :
                                                            x.Imposto.ICMS.orig.Equals(TpcnOrigemMercadoria.oeEstrangeiraAdquiridaBrasil) ? "Estrangeira - Adquirida no mercado interno" :
                                                            x.Imposto.ICMS.orig.Equals(TpcnOrigemMercadoria.oeNacional_Mercadoria_ou_bem_com_Conteúdo_de_Importação_superior_a_40) ? "Nacional, mercadoria ou bem com Conteúdo de Importação superior a 40% e inferior ou igual a 70%" :
                                                            x.Imposto.ICMS.orig.Equals(TpcnOrigemMercadoria.oeNacional_Cuja_produção_tenha_sido_feita_em_conformidade_com_o_PPB) ? "Nacional, cuja produção tenha sido feita em conformidade com os processos produtivos básicos de que tratam as legislações citadas nos Ajustes" :
                                                            x.Imposto.ICMS.orig.Equals(TpcnOrigemMercadoria.oeNacional_Mercadoria_com_bem_ou_conteúdo_de_importação_inferior_a_40) ? "Nacional, mercadoria ou bem com conteúdo de importação inferior ou igual a 40%" :
                                                            x.Imposto.ICMS.orig.Equals(TpcnOrigemMercadoria.oeEstrangeira_Importação_direta_sem_similar_nacional) ? "Estrangeira - Importação direta, sem similar nacional, constante em lista da CAMEX e gás natural" :
                                                            x.Imposto.ICMS.orig.Equals(TpcnOrigemMercadoria.oeEstrangeira_Adquirida_no_mercado_interno_com_similar_nacional) ? "Estrangeira - Adquirida no mercado interno, sem similiar nacional, constante lista CAMEX e gás natural" : ""
                                            },
                                            tributacao = x.Imposto.ICMS.CST,
                                            modalidadeBC = new
                                            {
                                                codigo = (int)x.Imposto.ICMS.modBC,
                                                descricao = x.Imposto.ICMS.modBC.Equals(TpcnDeterminacaoBaseIcms.dbiMargemValorAgregado) ? "Margem Valor Agregado (%)" :
                                                            x.Imposto.ICMS.modBC.Equals(TpcnDeterminacaoBaseIcms.dbiPauta) ? "Paute (Valor)" :
                                                            x.Imposto.ICMS.modBC.Equals(TpcnDeterminacaoBaseIcms.dbiPrecoTabelado) ? "Preço Tabelado Máx. (valor)" :
                                                            x.Imposto.ICMS.modBC.Equals(TpcnDeterminacaoBaseIcms.dbiValorOperacao) ? "Valor da operação" : ""
                                            },
                                            baseCalculoICMSNormal = x.Imposto.ICMS.vBC,
                                            aliquotaICMSNormal = x.Imposto.ICMS.pICMS,
                                            valorICMSNormal = x.Imposto.ICMS.vICMS,
                                            baseCalculoICMSST = x.Imposto.ICMS.vBCST,
                                            aliquotaICMSST = x.Imposto.ICMS.pICMSST,
                                            valorICMSST = x.Imposto.ICMS.vICMSST,
                                            percentualICMSST = x.Imposto.ICMS.pRedBCST,
                                            percentualMVAICMSST = x.Imposto.ICMS.pMVAST,
                                            modalidadeBCICMSST = new
                                            {
                                                codigo = (int)x.Imposto.ICMS.modBCST,
                                                descricao = x.Imposto.ICMS.modBCST.Equals(TpcnDeterminacaoBaseIcmsST.dbisPrecoTabelado) ? "Preço tabelado ou máximo sugerido" :
                                                            x.Imposto.ICMS.modBCST.Equals(TpcnDeterminacaoBaseIcmsST.dbisListaNegativa) ? "Lista negativa (Valor)" :
                                                            x.Imposto.ICMS.modBCST.Equals(TpcnDeterminacaoBaseIcmsST.dbisListaPositiva) ? "Lista positiva (Valor)" :
                                                            x.Imposto.ICMS.modBCST.Equals(TpcnDeterminacaoBaseIcmsST.dbisListaNeutra) ? "Lista neutra (Valor)" :
                                                            x.Imposto.ICMS.modBCST.Equals(TpcnDeterminacaoBaseIcmsST.dbisMargemValorAgregado) ? "Margem valor agregado (%)" :
                                                            x.Imposto.ICMS.modBCST.Equals(TpcnDeterminacaoBaseIcmsST.dbisPauta) ? "Pauta (valor)" : ""
                                            }
                                        },
                                        #endregion
                                        #region ICMSTot
                                        ICMSTot = new
                                        {
                                            baseCalculoICMS = x.Imposto.ICMSTot.vBC,
                                            baseCalculoICMSST = x.Imposto.ICMSTot.vBCST,
                                            valorCOFINS = x.Imposto.ICMSTot.vCOFINS,
                                            valorTotalDesconto = x.Imposto.ICMSTot.vDesc,
                                            valorTotalFrete = x.Imposto.ICMSTot.vFrete,
                                            valorTotalICMS = x.Imposto.ICMSTot.vICMS,
                                            valorTotalICMSDesonerado = x.Imposto.ICMSTot.vICMSDeson,
                                            valorTotalII = x.Imposto.ICMSTot.vII,
                                            valorTotalIPI = x.Imposto.ICMSTot.vIPI,
                                            valorTotalNfe = x.Imposto.ICMSTot.vNF,
                                            outrasDespesas = x.Imposto.ICMSTot.vOutro,
                                            valorPIS = x.Imposto.ICMSTot.vPIS,
                                            valorTotalProdutos = x.Imposto.ICMSTot.vProd,
                                            valorTotalSeguro = x.Imposto.ICMSTot.vSeg,
                                            valorICMSST = x.Imposto.ICMSTot.vST,
                                            valorAproximadoTotal = x.Imposto.ICMSTot.vTotTrib,
                                        },
                                        #endregion
                                        #region IMPOSTO DE IMPORTAÇÃO (II)
                                        II = new
                                        {
                                            valorBaseCalculo = x.Imposto.II.vBC,
                                            valorDespesas = x.Imposto.II.vDespAdu,
                                            valorII = x.Imposto.II.vII,
                                            valorIOF = x.Imposto.II.vIOF
                                        },
                                        #endregion
                                        #region IPI
                                        IPI = new
                                        {
                                            codigoEnquadramento = x.Imposto.IPI.cEnq,
                                            classeEnquadramento = x.Imposto.IPI.clEnq,
                                            CNPJProdutor = x.Imposto.IPI.CNPJProd,
                                            codigoSelo = x.Imposto.IPI.cSelo,
                                            CST = x.Imposto.IPI.CST,
                                            aliquotaIPI = x.Imposto.IPI.pIPI,
                                            qtdSelo = x.Imposto.IPI.qSelo,
                                            qtdTotalUnidadePadrao = x.Imposto.IPI.qUnid,
                                            baseCalculo = x.Imposto.IPI.vBC,
                                            valorIPI = x.Imposto.IPI.vIPI,
                                            valorUnidade = x.Imposto.IPI.vUnid,
                                        },
                                        #endregion
                                        #region PIS
                                        PIS = new
                                        {
                                            CST = x.Imposto.PIS.CST,
                                            aliquotaPercentual = x.Imposto.PIS.pPIS,
                                            qtdVendida = x.Imposto.PIS.qBCProd,
                                            aliquotaReais = x.Imposto.PIS.vAliqProd,
                                            valorBaseCalculo = x.Imposto.PIS.vBC,
                                            valorPIS = x.Imposto.PIS.vPIS
                                        },
                                        #endregion
                                        #region PISST
                                        PISST = new
                                        {
                                            aliquotaPercentual = x.Imposto.PISST.pPis,
                                            qtdVendida = x.Imposto.PISST.qBCProd,
                                            aliquotaReais = x.Imposto.PISST.vAliqProd,
                                            valorBaseCalculo = x.Imposto.PISST.vBC,
                                            valorPIS = x.Imposto.PISST.vPIS,
                                        },
                                        #endregion
                                        #region COFINS
                                        COFINS = new
                                        {
                                            CST = x.Imposto.COFINS.CST,
                                            aliquotaPercentual = x.Imposto.COFINS.pCOFINS,
                                            qtdVendida = x.Imposto.COFINS.qBCProd,
                                            aliquotaReais = x.Imposto.COFINS.vAliqProd,
                                            valorBaseCalculo = x.Imposto.COFINS.vBC,
                                            valorVendido = x.Imposto.COFINS.vBCProd,
                                            valorCOFINS = x.Imposto.COFINS.vCOFINS,
                                        },
                                        #endregion
                                        #region COFINSST
                                        COFINSST = new
                                        {
                                            aliquotaPercentual = x.Imposto.COFINSST.pCOFINS,
                                            qtdVendida = x.Imposto.COFINSST.qBCProd,
                                            aliquotaReais = x.Imposto.COFINSST.vAliqProd,
                                            valorBaseCalculo = x.Imposto.COFINSST.vBC,
                                            valorCOFINS = x.Imposto.COFINSST.vCOFINS,
                                        },
                                        #endregion
                                        #region RETENÇÃO DE TRIBUTOS
                                        retTrib = new
                                        {
                                            valorBaseCalculoIRRF = x.Imposto.retTrib.vBCIRRF,
                                            valorRetencaoPrevidencia = x.Imposto.retTrib.vBCRetPrev,
                                            valorRetidoIRRF = x.Imposto.retTrib.vIRRF,
                                            valorRetidoCOFINS = x.Imposto.retTrib.vRetCOFINS,
                                            valorRetidoCSLL = x.Imposto.retTrib.vRetCSLL,
                                            valorRetidoPIS = x.Imposto.retTrib.vRetPIS,
                                            valorBaseCalculoPrevidencia = x.Imposto.retTrib.vRetPrev,
                                        },
                                        #endregion
                                    },
                                    #endregion
                                    #region IMPOSTO DEVOLVIDO
                                    impostoDevolvido = new
                                    {
                                        percentualMercadoria = x.impostoDevol.pDevol,
                                        valorIPI = x.impostoDevol.vIPIDevol
                                    },
                                    #endregion
                                }).ToList<dynamic>(),

                                #endregion
                                #region TRANSPORTE
                                transporte = new
                                {
                                    balsa = xmlNFe.Transp.balsa,
                                    #region MODALIDADE
                                    modalidade = new
                                    {
                                        codigo = (int)xmlNFe.Transp.modFrete,
                                        descricao = xmlNFe.Transp.modFrete.Equals(TpcnModalidadeFrete.mfContaEmitente) ? "Por conta do emitente" :
                                                    xmlNFe.Transp.modFrete.Equals(TpcnModalidadeFrete.mfContaDestinatario) ? "Por conta do destinatário/remetente" :
                                                    xmlNFe.Transp.modFrete.Equals(TpcnModalidadeFrete.mfContaTerceiros) ? "Por conta de terceiros" :
                                                    xmlNFe.Transp.modFrete.Equals(TpcnModalidadeFrete.mfSemFrete) ? "Sem frete" : ""
                                    },
                                    #endregion
                                    #region REBOQUES
                                    reboques = xmlNFe.Transp.Reboque.Select(x => new
                                    {
                                        placa = x.placa,
                                        RNTC = x.RNTC,
                                        UF = x.UF
                                    }).ToList<dynamic>(),
                                    #endregion
                                    #region GRUPO RETENÇÃO ICMS Transporte
                                    retTransp = new
                                    {
                                        CFOP = xmlNFe.Transp.retTransp.CFOP,
                                        codigoMunicipioOcorrencia = xmlNFe.Transp.retTransp.cMunFG,
                                        aliquotaRetencao = xmlNFe.Transp.retTransp.pICMSRet,
                                        valorBaseCalculoRetencao = xmlNFe.Transp.retTransp.vBCRet,
                                        valorICMSRetido = xmlNFe.Transp.retTransp.vICMSRet,
                                        valorServico = xmlNFe.Transp.retTransp.vServ,
                                    },
                                    #endregion
                                    #region GRUPO TRANSPORTADOR
                                    grupoTransportador = new
                                    {
                                        CNPJ = xmlNFe.Transp.Transporta.CNPJ,
                                        CPF = xmlNFe.Transp.Transporta.CPF,
                                        IE = xmlNFe.Transp.Transporta.IE,
                                        UF = xmlNFe.Transp.Transporta.UF,
                                        endereco = xmlNFe.Transp.Transporta.xEnder,
                                        municipio = xmlNFe.Transp.Transporta.xMun,
                                        nome = xmlNFe.Transp.Transporta.xNome
                                    },
                                    #endregion
                                    vagao = xmlNFe.Transp.vagao,
                                    #region GRUPO VEÍCULO TRANSPORTE
                                    grupoVeiculoTransporte = new
                                    {
                                        placa = xmlNFe.Transp.veicTransp.placa,
                                        RNTC = xmlNFe.Transp.veicTransp.RNTC,
                                        UF = xmlNFe.Transp.veicTransp.UF,
                                    },
                                    #endregion
                                    #region GRUPO VOLUMES
                                        grupoVolumes = xmlNFe.Transp.Vol.Select(x => new
                                        {
                                            qtd = x.qVol,
                                            especie = x.esp,
                                            marca = x.marca,
                                            numeracao = x.nVol,
                                            pesoLiquido = x.pesoL,
                                            pesoBruto = x.pesoB,
                                            lacres = x.Lacres.Select(l => l.nLacre).ToList<string>()
                                        }).ToList<dynamic>()
                                    #endregion
                                },
                                #endregion
                                #region TOTAIS
                                totais = new
                                {
                                    #region ICMS
                                    ICMS = new
                                    {
                                        valorBaseCalculoICMS = xmlNFe.Total.ICMSTot.vBC,
                                        valorICMS = xmlNFe.Total.ICMSTot.vICMS,
                                        valorICMSDesonerado = xmlNFe.Total.ICMSTot.vICMSDeson,
                                        valorBaseCalculoICMSST = xmlNFe.Total.ICMSTot.vBCST,
                                        valorICMSST = xmlNFe.Total.ICMSTot.vST,
                                        valorProdutos = xmlNFe.Total.ICMSTot.vProd,
                                        valorFrete = xmlNFe.Total.ICMSTot.vFrete,
                                        valorSeguro = xmlNFe.Total.ICMSTot.vSeg,
                                        valorDesconto = xmlNFe.Total.ICMSTot.vDesc,
                                        valorII = xmlNFe.Total.ICMSTot.vII,
                                        valorIPI = xmlNFe.Total.ICMSTot.vIPI,
                                        valorPIS = xmlNFe.Total.ICMSTot.vPIS,
                                        valorCOFINS = xmlNFe.Total.ICMSTot.vCOFINS,
                                        valorOutrasDespesas = xmlNFe.Total.ICMSTot.vOutro,
                                        valorNF = xmlNFe.Total.ICMSTot.vNF,
                                        valorTributos = xmlNFe.Total.ICMSTot.vTotTrib,
                                    },
                                    #endregion
                                    #region ISSQN
                                    ISSQN = new
                                    {
                                        valorServico = xmlNFe.Total.ISSQNtot.vServ,
                                        valorBaseCalculo = xmlNFe.Total.ISSQNtot.vBC,
                                        valorISS = xmlNFe.Total.ISSQNtot.vISS,
                                        valorPIS = xmlNFe.Total.ISSQNtot.vPIS,
                                        valorCOFINS = xmlNFe.Total.ISSQNtot.vCOFINS,
                                        dataPrestacao = xmlNFe.Total.ISSQNtot.dCompet,
                                        valorDeducao = xmlNFe.Total.ISSQNtot.vDeducao,
                                        valorOutras = xmlNFe.Total.ISSQNtot.vOutro,
                                        valorDescontoIncondicionado = xmlNFe.Total.ISSQNtot.vDescIncond,
                                        valorDescontoCondicionado = xmlNFe.Total.ISSQNtot.vDescCond,
                                        valorRetencao = xmlNFe.Total.ISSQNtot.vISSRet,
                                        #region CÓDIGO DO REGIME ESPECIAL DE TRIBUTAÇÃO
                                        codigoRegimeTributacao = new
                                        {
                                            codigo = (int)xmlNFe.Total.ISSQNtot.cRegTrib,
                                            descricao = xmlNFe.Total.ISSQNtot.cRegTrib.Equals(TpcnRegimeTributario.Microempresa_Municipal) ? "Microempresa Municipal" :
                                                        xmlNFe.Total.ISSQNtot.cRegTrib.Equals(TpcnRegimeTributario.Estimativa) ? "Estimativa" :
                                                        xmlNFe.Total.ISSQNtot.cRegTrib.Equals(TpcnRegimeTributario.Sociedade_de_Profissionais) ? "Sociedade de Profissionais" :
                                                        xmlNFe.Total.ISSQNtot.cRegTrib.Equals(TpcnRegimeTributario.Cooperativa) ? "Cooperativa" :
                                                        xmlNFe.Total.ISSQNtot.cRegTrib.Equals(TpcnRegimeTributario.Microempresário_Individual__MEI) ? "Microempresário Individual (MEI)" :
                                                        xmlNFe.Total.ISSQNtot.cRegTrib.Equals(TpcnRegimeTributario.Microempresário_e_Empresa_de_Pequeno_Porte__ME_EPP) ? "Microempresário e Empresa de Pequeno Porte (ME/EPP)" : ""
                                        }
                                        #endregion
                                    },
                                    #endregion
                                    #region RETENÇÃO DE TRIBUTOS
                                    retTrib = new
                                    {
                                        valorPIS = xmlNFe.Total.retTrib.vRetPIS,
                                        valorCOFINS = xmlNFe.Total.retTrib.vRetCOFINS,
                                        valorCSLL = xmlNFe.Total.retTrib.vRetCSLL,
                                        valorBaseCalculoIRRF = xmlNFe.Total.retTrib.vBCIRRF,
                                        valorIRRF = xmlNFe.Total.retTrib.vIRRF,
                                        valorBaseCalculoPrevidencia = xmlNFe.Total.retTrib.vBCRetPrev,
                                        valorPrevidencia = xmlNFe.Total.retTrib.vRetPrev
                                    }
                                    #endregion
                                },
                                #endregion
                                #region COBRANÇA
                                cobranca = new
                                {
                                    #region FATURA
                                    fatura = new
                                    {
                                        numero = xmlNFe.Cobr.Fat.nFat,
                                        valorOriginal = xmlNFe.Cobr.Fat.vOrig,
                                        valorDesconto = xmlNFe.Cobr.Fat.vDesc,
                                        valorLiquido = xmlNFe.Cobr.Fat.vLiq
                                    },
                                    #endregion
                                    #region DUPLICATAS
                                    duplicatas = xmlNFe.Cobr.Dup.Select(x => new
                                    {
                                        numero = x.nDup,
                                        dataVencimento = x.dVenc,
                                        valor = x.vDup
                                    }).ToList<dynamic>()
                                    #endregion
                                },
                                #endregion
                                #region RETIRADA
                                retirada = new
                                {
                                    #region MUNICÍPIO
                                    municipio = new
                                    {
                                        codigo = xmlNFe.entrega.cMun,
                                        nome = xmlNFe.entrega.xMun,
                                    },
                                    #endregion
                                    CNPJ = xmlNFe.retirada.CNPJ,
                                    CPF = xmlNFe.retirada.CPF,
                                    logradouro = xmlNFe.retirada.xLgr,
                                    numero = xmlNFe.retirada.nro,
                                    complemento = xmlNFe.retirada.xCpl,
                                    bairro = xmlNFe.retirada.xBairro,
                                    UF = xmlNFe.retirada.UF,
                                },
                                #endregion
                                #region INFORMAÇÕES ADICIONAIS
                                infoAdicional = new
                                {
                                    fisco = xmlNFe.InfAdic.infAdFisco,
                                    complementar = xmlNFe.InfAdic.infCpl,
                                    #region GRUPO CAMPO DE USO LIVRE DO CONTRIBUINTE
                                    obsContribuinte = xmlNFe.InfAdic.obsCont.Select(x => new
                                    {
                                        identificacao = x.xCampo,
                                        conteudo = x.xTexto
                                    }).ToList<dynamic>(),
                                    #endregion
                                    #region GRUPO CAMPO DE USO LIVRE DO FISCO
                                    obsFisco = xmlNFe.InfAdic.obsFisco.Select(x => new
                                    {
                                        identificacao = x.xCampo,
                                        conteudo = x.xTexto
                                    }).ToList<dynamic>(),
                                    #endregion
                                    #region GRUPO PROCESSADO REFERENCIADO
                                    processoReferenciado = xmlNFe.InfAdic.procRef.Select(x => new
                                    {
                                        identificador = x.nProc,
                                        indicadorOrigem = x.indProc
                                        /*
                                            0 = SEFAZ
                                            1 = Justiça Federal
                                            2 = Justiça Estadual
                                            3 = Secex/RFB
                                            9 = Outros
                                        */
                                    }).ToList<dynamic>(),
                                    #endregion
                                },
                                #endregion
                                #region AVULSA
                                avulsa = new
                                {
                                    CNPJ = xmlNFe.avulsa.CNPJ,
                                    dtEmissao = xmlNFe.avulsa.dEmi,
                                    dtPagamento = xmlNFe.avulsa.dPag,
                                    telefone = xmlNFe.avulsa.fone,
                                    matriculaAgente = xmlNFe.avulsa.matr,
                                    numDAR = xmlNFe.avulsa.nDAR,
                                    reparticaoEmitente = xmlNFe.avulsa.repEmi,
                                    UF = xmlNFe.avulsa.UF,
                                    valorDAR = xmlNFe.avulsa.vDAR,
                                    nomeAgente = xmlNFe.avulsa.xAgente,
                                    orgaoEmitente = xmlNFe.avulsa.xOrgao,
                                }
                                #endregion
                                // List<pag> pag
                                // protNFe protNFe
                                // List<autXML> autXML
                                // Cana cana
                                // Compra compra
                                // Exporta exporta
                            });
                        }

                        var nf = new
                        {
                            nrEmitenteCNPJCPF = item.nrEmitenteCNPJCPF,
                            nmEmitente = item.nmEmitente,
                            UF = uf,
                            notas = notas
                        };

                        lista.Add(nf);
                    }
                    #endregion

                    CollectionTbManifesto.Clear();
                    CollectionTbManifesto = lista;

                }
                else if (colecao == 5) // [PORTAL] Consulta NFe Completa NFe to JSON
                {
                    CollectionTbManifesto = query
                        .GroupBy(e => new { e.nrEmitenteCNPJCPF, e.nmEmitente })
                        .OrderBy(e => e.Key.nmEmitente)
                        //.ThenBy(e => e.Count())
                        .Select(e => new
                        {
                            nrEmitenteCNPJCPF = e.Key.nrEmitenteCNPJCPF,
                            nmEmitente = e.Key.nmEmitente,
                            UF = "",
                            notas = e.Select(x => new {
                                idManifesto = x.idManifesto,
                                dtEmissao = x.dtEmissao,
                                vlNFe = x.vlNFe,
                                nrChave = x.nrChave,
                                dsSituacaoManifesto = x.dsSituacaoManifesto,
                                dsSituacaoErp = "Não Importado",
                                xmlNFe = x.xmlNFe

                            })
                            .OrderBy(x => x.dtEmissao.Value.Year)
                            .ThenBy(x => x.dtEmissao.Value.Month)
                            .ThenBy(x => x.dtEmissao.Value.Day)
                            .ToList<dynamic>()
                        }).ToList<dynamic>();

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = CollectionTbManifesto.Count;

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        CollectionTbManifesto = CollectionTbManifesto.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;


                    List<dynamic> lista = new List<dynamic>();
                    #region OBTÉM INFO GERAL DE CADA NOTA
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
                                idManifesto = notas.idManifesto,
                                modelo = (int)xmlNFe.ide.mod,
                                serie = xmlNFe.ide.serie,
                                numero = xmlNFe.ide.nNF,
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
                    #endregion

                    CollectionTbManifesto.Clear();
                    CollectionTbManifesto = lista;


                }
                else if (colecao == 6) //  [MOBILE]
                {
                    CollectionTbManifesto = query.Select(e => new
                    {
                        idManifesto = e.idManifesto,
                        nmEmitente = e.nmEmitente,
                        dtEmissao = e.dtEmissao,
                        xmlNFe = e.xmlNFe,
                        vlNFe = e.vlNFe,
                        nrChave = e.nrChave,
                    }).ToList<dynamic>();

                    List<dynamic> lista = new List<dynamic>();
                    foreach (var item in CollectionTbManifesto)
                    {
                        NFe.ConvertTxt.NFe xmlNFe = Bibliotecas.nfeRead.Loader(item.xmlNFe);
                        var e = new
                        {
                            idManifesto = item.idManifesto,
                            nmEmitente = item.nmEmitente,
                            dtEmissao = item.dtEmissao,
                            nmDestinatario = xmlNFe.dest.xNome,
                            vlNFe = item.vlNFe,
                            nrChave = item.nrChave,
                        };

                        lista.Add(e);
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
                semaforo.WaitOne();
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
            finally
            {
                semaforo.Release(1);
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
                semaforo.WaitOne();
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbManifesto manifesto = _db.tbManifestos.Where(e => e.idManifesto == idManifesto).FirstOrDefault();

                if (manifesto == null) throw new Exception("Manifesto inexistente!");

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
            finally
            {
                semaforo.Release(1);
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
                semaforo.WaitOne();
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
                if (param.dtEntrega != null && param.dtEntrega != value.dtEntrega)
                {
                    value.dtEntrega = param.dtEntrega;
                    value.idUsers = Bibliotecas.Permissoes.GetIdUser(token);
                }

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
            finally
            {
                semaforo.Release(1);
            }
        }
    }
}
