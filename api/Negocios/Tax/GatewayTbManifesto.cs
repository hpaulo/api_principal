using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

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
            var entity = _db.tbManifestos.AsQueryable();

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
                        entity = entity.Where(e => e.idManifesto.Equals(idManifesto)).AsQueryable();
                        break;
                    case CAMPOS.NRCHAVE:
                        string nrChave = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrChave.Equals(nrChave)).AsQueryable();
                        break;
                    case CAMPOS.NRNSU:
                        string nrNSU = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrNSU.Equals(nrNSU)).AsQueryable();
                        break;
                    case CAMPOS.CDGRUPO:
                        Int32 cdGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdGrupo.Equals(cdGrupo)).AsQueryable();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable();
                        break;
                    case CAMPOS.NREMITENTECNPJCPF:
                        string nrEmitenteCNPJCPF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrEmitenteCNPJCPF.Equals(nrEmitenteCNPJCPF)).AsQueryable();
                        break;
                    case CAMPOS.NMEMITENTE:
                        string nmEmitente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmEmitente.Equals(nmEmitente)).AsQueryable();
                        break;
                    case CAMPOS.NREMITENTEIE:
                        string nrEmitenteIE = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrEmitenteIE.Equals(nrEmitenteIE)).AsQueryable();
                        break;
                    case CAMPOS.DTEMISSAO:
                        DateTime dtEmissao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtEmissao.Equals(dtEmissao)).AsQueryable();
                        break;
                    case CAMPOS.TPOPERACAO:
                        string tpOperacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tpOperacao.Equals(tpOperacao)).AsQueryable();
                        break;
                    case CAMPOS.VLNFE:
                        decimal vlNFe = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlNFe.Equals(vlNFe)).AsQueryable();
                        break;
                    case CAMPOS.DTRECEBIMENTO:
                        DateTime dtRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtRecebimento.Equals(dtRecebimento)).AsQueryable();
                        break;
                    case CAMPOS.CDSITUACAONFE:
                        string cdSituacaoNFe = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdSituacaoNFe.Equals(cdSituacaoNFe)).AsQueryable();
                        break;
                    case CAMPOS.CDSITUACAOMANIFESTO:
                        short cdSituacaoManifesto = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdSituacaoManifesto.Equals(cdSituacaoManifesto)).AsQueryable();
                        break;
                    case CAMPOS.DSSITUACAOMANIFESTO:
                        string dsSituacaoManifesto = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsSituacaoManifesto.Equals(dsSituacaoManifesto)).AsQueryable();
                        break;
                    case CAMPOS.NRPROTOCOLOMANIFESTO:
                        string nrProtocoloManifesto = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrProtocoloManifesto.Equals(nrProtocoloManifesto)).AsQueryable();
                        break;
                    case CAMPOS.XMLNFE:
                        string xmlNFe = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.xmlNFe.Equals(xmlNFe)).AsQueryable();
                        break;
                    case CAMPOS.NRPROTOCOLODOWNLOAD:
                        string nrProtocoloDownload = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrProtocoloDownload.Equals(nrProtocoloDownload)).AsQueryable();
                        break;
                    case CAMPOS.CDSITUACAODOWNLOAD:
                        short cdSituacaoDownload = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdSituacaoDownload.Equals(cdSituacaoDownload)).AsQueryable();
                        break;
                    case CAMPOS.DSSITUACAODOWNLOAD:
                        string dsSituacaoDownload = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsSituacaoDownload.Equals(dsSituacaoDownload)).AsQueryable();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.idManifesto).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.idManifesto).AsQueryable();
                    break;
                case CAMPOS.NRCHAVE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrChave).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrChave).AsQueryable();
                    break;
                case CAMPOS.NRNSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrNSU).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrNSU).AsQueryable();
                    break;
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdGrupo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdGrupo).AsQueryable();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable();
                    break;
                case CAMPOS.NREMITENTECNPJCPF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrEmitenteCNPJCPF).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrEmitenteCNPJCPF).AsQueryable();
                    break;
                case CAMPOS.NMEMITENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmEmitente).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nmEmitente).AsQueryable();
                    break;
                case CAMPOS.NREMITENTEIE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrEmitenteIE).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrEmitenteIE).AsQueryable();
                    break;
                case CAMPOS.DTEMISSAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtEmissao).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtEmissao).AsQueryable();
                    break;
                case CAMPOS.TPOPERACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tpOperacao).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.tpOperacao).AsQueryable();
                    break;
                case CAMPOS.VLNFE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlNFe).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.vlNFe).AsQueryable();
                    break;
                case CAMPOS.DTRECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtRecebimento).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtRecebimento).AsQueryable();
                    break;
                case CAMPOS.CDSITUACAONFE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSituacaoNFe).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdSituacaoNFe).AsQueryable();
                    break;
                case CAMPOS.CDSITUACAOMANIFESTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSituacaoManifesto).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdSituacaoManifesto).AsQueryable();
                    break;
                case CAMPOS.DSSITUACAOMANIFESTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsSituacaoManifesto).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsSituacaoManifesto).AsQueryable();
                    break;
                case CAMPOS.NRPROTOCOLOMANIFESTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrProtocoloManifesto).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrProtocoloManifesto).AsQueryable();
                    break;
                case CAMPOS.XMLNFE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.xmlNFe).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.xmlNFe).AsQueryable();
                    break;
                case CAMPOS.NRPROTOCOLODOWNLOAD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrProtocoloDownload).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrProtocoloDownload).AsQueryable();
                    break;
                case CAMPOS.CDSITUACAODOWNLOAD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSituacaoDownload).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdSituacaoDownload).AsQueryable();
                    break;
                case CAMPOS.DSSITUACAODOWNLOAD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsSituacaoDownload).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsSituacaoDownload).AsQueryable();
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
                    var max = query.Select(e => new
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
                    }).Max(e => e.nrNSU);

                    CollectionTbManifesto.Add(max);
                }
                else if (colecao == 0)
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
            {
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
                _db.tbManifestos.Remove(_db.tbManifestos.Where(e => e.idManifesto.Equals(idManifesto)).First());
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
                tbManifesto value = _db.tbManifestos
                        .Where(e => e.idManifesto.Equals(param.idManifesto))
                        .First<tbManifesto>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idManifesto != null && param.idManifesto != value.idManifesto)
                    value.idManifesto = param.idManifesto;
                if (param.nrChave != null && param.nrChave != value.nrChave)
                    value.nrChave = param.nrChave;
                if (param.nrNSU != null && param.nrNSU != value.nrNSU)
                    value.nrNSU = param.nrNSU;
                if (param.cdGrupo != null && param.cdGrupo != value.cdGrupo)
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
