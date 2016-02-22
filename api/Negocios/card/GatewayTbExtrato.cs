using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Globalization;
using System.Net.Http;
using OFXSharp;
using System.IO;
using System.Data.Entity.Validation;
using api.Controllers.Util;
using System.Data.Entity;
using api.Negocios.Util;

namespace api.Negocios.Card
{
    public class GatewayTbExtrato
    {
        //public static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbExtrato()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "EX";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDEXTRATO = 100,
            CDCONTACORRENTE = 101,
            DTEXTRATO = 102,
            NRDOCUMENTO = 103,
            DSDOCUMENTO = 104,
            VLMOVIMENTO = 105,
            DSTIPO = 106,
            DSARQUIVO = 107,

            // EMPRESA
            NU_CNPJ = 200,
            ID_GRUPO = 216,

            // TBADQUIRENTE
            CDADQUIRENTE = 300, // -1 para = null, 0 para != null

            // VIGÊNCIA
            VIGENCIA = 400, // CNPJ!DATA!CDADQUIRENTE
        };

        /// <summary>
        /// Get TbExtrato/TbExtrato
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static IQueryable<tbExtrato> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbExtratos.AsQueryable<tbExtrato>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDEXTRATO:
                        Int32 idExtrato = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idExtrato == idExtrato).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdContaCorrente == cdContaCorrente).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.NRDOCUMENTO:
                        string nrDocumento = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrDocumento.Equals(nrDocumento)).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.DTEXTRATO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtExtrato.Year > dtaIni.Year || (e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month > dtaIni.Month) ||
                                                                                          (e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month && e.dtExtrato.Day >= dtaIni.Day))
                                                    && (e.dtExtrato.Year < dtaFim.Year || (e.dtExtrato.Year == dtaFim.Year && e.dtExtrato.Month < dtaFim.Month) ||
                                                                                          (e.dtExtrato.Year == dtaFim.Year && e.dtExtrato.Month == dtaFim.Month && e.dtExtrato.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Length == 6) // ANO + MES
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month);
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month && e.dtExtrato.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.DSDOCUMENTO:
                        decimal dsDocumento = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.dsDocumento.Equals(dsDocumento)).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.VLMOVIMENTO:
                        decimal vlMovimento = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlMovimento == vlMovimento).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsTipo.Equals(dsTipo)).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.DSARQUIVO:
                        string dsArquivo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsArquivo.Equals(dsArquivo)).AsQueryable<tbExtrato>();
                        break;

                    // PERSONALIZADO
                    case CAMPOS.ID_GRUPO:
                        int id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbContaCorrente.cdGrupo == id_grupo).AsQueryable<tbExtrato>(); // somente as movimentações referentes às contas do grupo
                        break;
                    case CAMPOS.NU_CNPJ:
                        string nrCnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => _db.tbBancoParametro.Where(b => b.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                       .Where(b => b.dsMemo.Equals(e.dsDocumento))
                                                                       .Where(b => b.nrCnpj == null || b.nrCnpj.Equals(nrCnpj)) // somente as movimentações que se referem a filial => os memos "genéricos" ou específicos da filial (identificado pelo estabelecimento)
                                                                       .Count() > 0).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        if (item.Value.Contains("!"))
                        {
                            // Considera também os que tem dsTipo != null
                            int cdAdquirente = Convert.ToInt32(item.Value.Replace("!",""));
                            if (cdAdquirente == -1)
                                entity = entity.Where(e => _db.tbBancoParametro.Where(b => b.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                               .Where(b => b.dsMemo.Equals(e.dsDocumento))
                                                                               .Where(b => b.cdAdquirente == null && (b.dsTipoCartao != null || b.flAntecipacao))
                                                                               .Count() > 0).AsQueryable<tbExtrato>();
                            else if (cdAdquirente == 0)
                                entity = entity.Where(e => _db.tbBancoParametro.Where(b => b.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                               .Where(b => b.dsMemo.Equals(e.dsDocumento))
                                                                               .Where(b => b.cdAdquirente != null || (b.cdAdquirente == null && (b.dsTipoCartao != null || b.flAntecipacao)))
                                                                               .Count() > 0).AsQueryable<tbExtrato>();
                            else
                                entity = entity.Where(e => _db.tbBancoParametro.Where(b => b.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                               .Where(b => b.dsMemo.Equals(e.dsDocumento))
                                                                               .Where(b => b.cdAdquirente == cdAdquirente || (b.cdAdquirente == null && (b.dsTipoCartao != null || b.flAntecipacao)))
                                                                               .Count() > 0).AsQueryable<tbExtrato>();
                        }
                        else
                        {
                            int cdAdquirente = Convert.ToInt32(item.Value);
                            if (cdAdquirente == -1)
                                entity = entity.Where(e => _db.tbBancoParametro.Where(b => b.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                               .Where(b => b.dsMemo.Equals(e.dsDocumento))
                                                                               .Where(b => b.cdAdquirente == null)
                                                                               .Count() > 0).AsQueryable<tbExtrato>();
                            else if (cdAdquirente == 0)
                                entity = entity.Where(e => _db.tbBancoParametro.Where(b => b.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                               .Where(b => b.dsMemo.Equals(e.dsDocumento))
                                                                               .Where(b => b.cdAdquirente != null)
                                                                               .Count() > 0).AsQueryable<tbExtrato>();
                            else
                                entity = entity.Where(e => _db.tbBancoParametro.Where(b => b.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                               .Where(b => b.dsMemo.Equals(e.dsDocumento))
                                                                               .Where(b => b.cdAdquirente == cdAdquirente)
                                                                               .Count() > 0).AsQueryable<tbExtrato>();
                        }
                        break;
                    case CAMPOS.VIGENCIA:
                        string[] vigencia = item.Value.Split('!');
                        if (vigencia.Length < 1) continue;

                        string cnpj = vigencia[0].Trim();
                        string dt = vigencia.Length > 1 ? vigencia[1] : "null";

                        if (dt.Equals("null"))
                        {
                            // Independente do período de vigência
                            if (vigencia.Length > 2)
                            {
                                int cdadquirente = Convert.ToInt32(vigencia[2]);
                                if(cnpj.Equals(""))
                                    // Todas as filiais, com a adquirente
                                    entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.cdAdquirente == cdadquirente).Count() > 0).AsQueryable<tbExtrato>();
                                else
                                    // Filial e adquirente específicas
                                    entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.nrCnpj.Equals(cnpj))
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.cdAdquirente == cdadquirente).Count() > 0).AsQueryable<tbExtrato>();
                            }
                            else if(!cnpj.Equals(""))
                            {
                                entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.nrCnpj.Equals(cnpj)).Count() > 0).AsQueryable<tbExtrato>();
                            }

                        }
                        else if (dt.Contains("|"))
                        {
                            string[] dts = dt.Split('|');
                            DateTime dtIni = DateTime.ParseExact(dts[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtFim = DateTime.ParseExact(dts[1] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            // Para o LoginAdquirenteEmpresa, a data do filtro tem que estar na validade da vigência
                            if (vigencia.Length > 2)
                            {
                                int cdadquirente = Convert.ToInt32(vigencia[2]);
                                if (cnpj.Equals(""))
                                    // Período e adquirente específicos
                                    entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.cdAdquirente == cdadquirente)
                                                                                .Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))
                                                                                     && (v.dtInicio.Year < dtFim.Year || (v.dtInicio.Year == dtFim.Year && v.dtInicio.Month < dtFim.Month) || (v.dtInicio.Year == dtFim.Year && v.dtInicio.Month == dtFim.Month && v.dtInicio.Day <= dtFim.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtFim.Year || (v.dtFim.Value.Year == dtFim.Year && v.dtFim.Value.Month > dtFim.Month) || (v.dtFim.Value.Year == dtFim.Year && v.dtFim.Value.Month == dtFim.Month && v.dtFim.Value.Day >= dtFim.Day)))).Count() > 0).AsQueryable<tbExtrato>();
                                else
                                    // Filial, período e adquirente específicos
                                    entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.nrCnpj.Equals(cnpj))
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.cdAdquirente == cdadquirente)
                                                                                .Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))
                                                                                     && (v.dtInicio.Year < dtFim.Year || (v.dtInicio.Year == dtFim.Year && v.dtInicio.Month < dtFim.Month) || (v.dtInicio.Year == dtFim.Year && v.dtInicio.Month == dtFim.Month && v.dtInicio.Day <= dtFim.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtFim.Year || (v.dtFim.Value.Year == dtFim.Year && v.dtFim.Value.Month > dtFim.Month) || (v.dtFim.Value.Year == dtFim.Year && v.dtFim.Value.Month == dtFim.Month && v.dtFim.Value.Day >= dtFim.Day)))).Count() > 0).AsQueryable<tbExtrato>();
                            }
                            else if(cnpj.Equals(""))
                            {
                                // Somente período específico
                                entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))
                                                                                     && (v.dtInicio.Year < dtFim.Year || (v.dtInicio.Year == dtFim.Year && v.dtInicio.Month < dtFim.Month) || (v.dtInicio.Year == dtFim.Year && v.dtInicio.Month == dtFim.Month && v.dtInicio.Day <= dtFim.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtFim.Year || (v.dtFim.Value.Year == dtFim.Year && v.dtFim.Value.Month > dtFim.Month) || (v.dtFim.Value.Year == dtFim.Year && v.dtFim.Value.Month == dtFim.Month && v.dtFim.Value.Day >= dtFim.Day)))).Count() > 0).AsQueryable<tbExtrato>();
                            }
                            else
                            {
                                // Considera filial e período específicos
                                entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.nrCnpj.Equals(cnpj))
                                                                                .Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))
                                                                                     && (v.dtInicio.Year < dtFim.Year || (v.dtInicio.Year == dtFim.Year && v.dtInicio.Month < dtFim.Month) || (v.dtInicio.Year == dtFim.Year && v.dtInicio.Month == dtFim.Month && v.dtInicio.Day <= dtFim.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtFim.Year || (v.dtFim.Value.Year == dtFim.Year && v.dtFim.Value.Month > dtFim.Month) || (v.dtFim.Value.Year == dtFim.Year && v.dtFim.Value.Month == dtFim.Month && v.dtFim.Value.Day >= dtFim.Day)))).Count() > 0).AsQueryable<tbExtrato>();
                            }
                        }
                        else
                        {
                            DateTime dtIni = DateTime.ParseExact(dt + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            // Para o LoginAdquirenteEmpresa, a data do filtro tem que estar na validade da vigência
                            if (vigencia.Length > 2)
                            {
                                int cdadquirente = Convert.ToInt32(vigencia[2]);
                                if (cnpj.Equals(""))
                                    // Data e adquirente específicas
                                    entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.cdAdquirente == cdadquirente)
                                                                                .Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))).Count() > 0).AsQueryable<tbExtrato>();
                                else
                                    // Filial, data e adquirente específicas
                                    entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.nrCnpj.Equals(cnpj))
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.cdAdquirente == cdadquirente)
                                                                                .Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))).Count() > 0).AsQueryable<tbExtrato>();

                            }
                            else if(cnpj.Equals(""))
                            {
                                // Considera apenas a data
                                entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))).Count() > 0).AsQueryable<tbExtrato>();
                            }
                            else
                            {
                                // Data e filial específicas
                                entity = entity.Where(e => e.tbContaCorrente.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(v => v.tbLoginAdquirenteEmpresa.nrCnpj.Equals(cnpj))
                                                                                .Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                                                     && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))).Count() > 0).AsQueryable<tbExtrato>();
                            }
                        }
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDEXTRATO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idExtrato).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.idExtrato).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdContaCorrente).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.cdContaCorrente).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.NRDOCUMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrDocumento).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.nrDocumento).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.DTEXTRATO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtExtrato).ThenBy(e => e.dsTipo).ThenBy(e => e.dsDocumento).ThenBy(e => e.vlMovimento).ThenBy(e => e.idExtrato).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.dtExtrato).ThenByDescending(e => e.dsTipo).ThenByDescending(e => e.dsDocumento).ThenByDescending(e => e.vlMovimento).ThenByDescending(e => e.idExtrato).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.DSDOCUMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsDocumento).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.dsDocumento).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.VLMOVIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlMovimento).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.vlMovimento).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.DSTIPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsTipo).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.dsTipo).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.DSARQUIVO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsArquivo).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.dsArquivo).AsQueryable<tbExtrato>();
                    break;
            }
            #endregion

            return entity;


        }


        public static SimpleDataBaseQuery getQuery(int campo, int orderby, Dictionary<string, string> queryString)
        {
            Dictionary<string, string> join = new Dictionary<string, string>();
            List<string> where = new List<string>();
            List<string> order = new List<string>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (KeyValuePair<string, string> item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDEXTRATO:
                        Int32 idExtrato = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idExtrato = " + idExtrato);
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".cdContaCorrente = " + cdContaCorrente);
                        break;
                    case CAMPOS.NRDOCUMENTO:
                        string nrDocumento = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nrDocumento = '" + nrDocumento + "'");
                        break;
                    case CAMPOS.DTEXTRATO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(SIGLA_QUERY + ".dtExtrato >= '" + dtInicio + "' AND " + SIGLA_QUERY + ".dtExtrato <= '" + dtFim + "'");
                        }
                        else if (item.Value.Length == 6) // ANO + MES
                        {
                            string busca = item.Value + "01";
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("DATEPART(YEAR, " + SIGLA_QUERY + ".dtExtrato) = " + data.Year + " AND DATEPART(MONTH, " + SIGLA_QUERY + ".dtExtrato) = " + data.Month);
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(data);
                            where.Add(SIGLA_QUERY + ".dtExtrato = '" + dt + "'");
                        }
                        break;
                    case CAMPOS.DSDOCUMENTO:
                        decimal dsDocumento = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".dsDocumento = '" + dsDocumento + "'");
                        break;
                    case CAMPOS.VLMOVIMENTO:
                        decimal vlMovimento = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlMovimento = " + vlMovimento.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".dsTipo = '" + dsTipo + "'");
                        break;
                    case CAMPOS.DSARQUIVO:
                        string dsArquivo = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".dsArquivo = '" + dsArquivo + "'");
                        break;

                    // PERSONALIZADO
                    case CAMPOS.ID_GRUPO:
                        int id_grupo = Convert.ToInt32(item.Value);
                        if (!join.ContainsKey("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + SIGLA_QUERY + ".cdContaCorrente");
                        where.Add(GatewayTbContaCorrente.SIGLA_QUERY + ".cdGrupo = " + id_grupo); // somente as movimentações referentes às contas do grupo
                        break;
                    case CAMPOS.NU_CNPJ:
                        string nrCnpj = Convert.ToString(item.Value);
                        if (!join.ContainsKey("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + SIGLA_QUERY + ".cdContaCorrente");
                        if (!join.ContainsKey("INNER JOIN card.tbBancoParametro " + GatewayTbBancoParametro.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbBancoParametro " + GatewayTbBancoParametro.SIGLA_QUERY, " ON " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdBanco = " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdBanco AND " + GatewayTbBancoParametro.SIGLA_QUERY + ".dsMemo = " + SIGLA_QUERY + ".dsDocumento");
                        where.Add(GatewayTbBancoParametro.SIGLA_QUERY + ".nrCnpj IS NULL OR " + GatewayTbBancoParametro.SIGLA_QUERY + ".nrCnpj = '" + nrCnpj +  "'"); // somente as movimentações que se referem a filial => os memos "genéricos" ou específicos da filial (identificado pelo estabelecimento)
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        if (!join.ContainsKey("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + SIGLA_QUERY + ".cdContaCorrente");
                        if (!join.ContainsKey("INNER JOIN card.tbBancoParametro " + GatewayTbBancoParametro.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbBancoParametro " + GatewayTbBancoParametro.SIGLA_QUERY, " ON " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdBanco = " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdBanco AND " + GatewayTbBancoParametro.SIGLA_QUERY + ".dsMemo = " + SIGLA_QUERY + ".dsDocumento");
                        if (item.Value.Contains("!"))
                        {
                            // Considera também os que tem dsTipo != null
                            int cdAdquirente = Convert.ToInt32(item.Value.Replace("!", ""));
                            if (cdAdquirente == -1)
                                where.Add(GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente IS NULL AND " + GatewayTbBancoParametro.SIGLA_QUERY + ".dsTipoCartao IS NOT NULL");
                            else if (cdAdquirente == 0)
                                where.Add(GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente IS NOT NULL OR (" + GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente IS NULL AND " + GatewayTbBancoParametro.SIGLA_QUERY + ".dsTipoCartao IS NOT NULL)");
                            else
                                where.Add(GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente + " OR (" + GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente IS NULL AND " + GatewayTbBancoParametro.SIGLA_QUERY + ".dsTipoCartao IS NOT NULL)");
                        }
                        else
                        {
                            int cdAdquirente = Convert.ToInt32(item.Value);
                            if (cdAdquirente == -1)
                                where.Add(GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente IS NULL");
                            else if (cdAdquirente == 0)
                                where.Add(GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente IS NOT NULL");
                            else
                                where.Add(GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente);
                        }
                        break;
                    case CAMPOS.VIGENCIA:
                        string[] vigencia = item.Value.Split('!');
                        if (vigencia.Length < 1) continue;

                        if (!join.ContainsKey("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + SIGLA_QUERY + ".cdContaCorrente");
                        if (!join.ContainsKey("INNER JOIN card.tbContaCorrente_tbLoginAdquirenteEmpresa " + GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbContaCorrente_tbLoginAdquirenteEmpresa " + GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY, " ON " + GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".cdContaCorrente = " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente");
                        if (!join.ContainsKey("INNER JOIN card.tbLoginAdquirenteEmpresa " + GatewayTbLoginAdquirenteEmpresa.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbLoginAdquirenteEmpresa " + GatewayTbLoginAdquirenteEmpresa.SIGLA_QUERY, " ON " + GatewayTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".cdLoginAdquirenteEmpresa = " + GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".cdLoginAdquirenteEmpresa");
                        

                        string cnpj = vigencia[0].Trim();
                        string dtv = vigencia.Length > 1 ? vigencia[1] : "null";

                        // Filial
                        if (!cnpj.Equals(""))
                            where.Add(GatewayTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".nrCnpj = '" + cnpj + "'");

                        // Adquirente
                        if (vigencia.Length > 2)
                            where.Add(GatewayTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".cdAdquirente = " + Convert.ToInt32(vigencia[2]));
                        
                        // Período de vigência
                        if (dtv.Contains("|"))
                        {
                            string[] dts = dtv.Split('|');
                            DateTime dtIni = DateTime.ParseExact(dts[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtFim = DateTime.ParseExact(dts[1] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            where.Add(GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtInicio <= '" + DataBaseQueries.GetDate(dtIni) + "'");
                            where.Add(GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtFim IS NULL OR (" + GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtFim >= '" + DataBaseQueries.GetDate(dtIni) + "')");
                            where.Add(GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtInicio <= '" + DataBaseQueries.GetDate(dtFim) + "'");
                            where.Add(GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtFim IS NULL OR (" + GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtFim >= '" + DataBaseQueries.GetDate(dtFim) + "')");
                        }
                        else if (!dtv.Equals("null"))
                        {
                            DateTime dtIni = DateTime.ParseExact(dtv + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            where.Add(GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtInicio <= '" + DataBaseQueries.GetDate(dtIni) + "'");
                            where.Add(GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtFim IS NULL OR (" + GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.SIGLA_QUERY + ".dtFim >= '" + DataBaseQueries.GetDate(dtIni) + "')");
                        }
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDEXTRATO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idExtrato ASC");
                    else order.Add(SIGLA_QUERY + ".idExtrato DESC");
                    break;
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdContaCorrente ASC");
                    else order.Add(SIGLA_QUERY + ".cdContaCorrente DESC");
                    break;
                case CAMPOS.NRDOCUMENTO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nrDocumento ASC");
                    else order.Add(SIGLA_QUERY + ".nrDocumento DESC");
                    break;
                case CAMPOS.DTEXTRATO:
                    if (orderby == 0)
                    {
                        order.Add(SIGLA_QUERY + ".dtExtrato ASC");
                        order.Add(SIGLA_QUERY + ".dsTipo ASC");
                        order.Add(SIGLA_QUERY + ".dsDocumento ASC");
                        order.Add(SIGLA_QUERY + ".vlMovimento ASC");
                        order.Add(SIGLA_QUERY + ".idExtrato ASC");
                    }
                    else
                    {
                        order.Add(SIGLA_QUERY + ".dtExtrato DESC");
                        order.Add(SIGLA_QUERY + ".dsTipo DESC");
                        order.Add(SIGLA_QUERY + ".dsDocumento DESC");
                        order.Add(SIGLA_QUERY + ".vlMovimento DESC");
                        order.Add(SIGLA_QUERY + ".idExtrato DESC");
                    }
                    break;
                case CAMPOS.DSDOCUMENTO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dsDocumento ASC");
                    else order.Add(SIGLA_QUERY + ".dsDocumento DESC");
                    break;
                case CAMPOS.VLMOVIMENTO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlMovimento ASC");
                    else order.Add(SIGLA_QUERY + ".vlMovimento DESC");
                    break;
                case CAMPOS.DSTIPO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dsTipo ASC");
                    else order.Add(SIGLA_QUERY + ".dsTipo DESC");
                    break;
                case CAMPOS.DSARQUIVO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dsArquivo ASC");
                    else order.Add(SIGLA_QUERY + ".dsArquivo DESC");
                    break;
            }
            #endregion

            return new SimpleDataBaseQuery(null, "card.tbExtrato " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());

        }


        /// <summary>
        /// Retorna TbExtrato/TbExtrato
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbExtrato = new List<dynamic>();
                Retorno retorno = new Retorno();

                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                        queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (!CnpjEmpresa.Equals(""))
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                        queryString["" + (int)CAMPOS.NU_CNPJ] = CnpjEmpresa;
                    else
                        queryString.Add("" + (int)CAMPOS.NU_CNPJ, CnpjEmpresa);
                }

                // GET QUERY
                var query = getQuery(_db, colecao, campo, orderBy, pageSize, pageNumber, queryString);

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = query.Count();

                if (colecao == 2)
                {
                    retorno.Totais = new Dictionary<string, object>();
                    retorno.Totais.Add("valor", retorno.TotalDeRegistros > 0 ? query.Sum(t => t.vlMovimento) : new decimal(0.0));
                }

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
                    CollectionTbExtrato = query.Select(e => new
                    {

                        idExtrato = e.idExtrato,
                        cdContaCorrente = e.cdContaCorrente,
                        nrDocumento = e.nrDocumento,
                        dtExtrato = e.dtExtrato,
                        dsDocumento = e.dsDocumento,
                        vlMovimento = e.vlMovimento,
                        dsTipo = e.dsTipo,
                        dsArquivo = e.dsArquivo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbExtrato = query.Select(e => new
                    {

                        idExtrato = e.idExtrato,
                        cdContaCorrente = e.cdContaCorrente,
                        nrDocumento = e.nrDocumento,
                        dtExtrato = e.dtExtrato,
                        dsDocumento = e.dsDocumento,
                        vlMovimento = e.vlMovimento,
                        dsTipo = e.dsTipo,
                        dsArquivo = e.dsArquivo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionTbExtrato = query.Select(e => new
                    {

                        idExtrato = e.idExtrato,
                        cdContaCorrente = e.cdContaCorrente,
                        nrDocumento = e.nrDocumento,
                        dtExtrato = e.dtExtrato,
                        dsDocumento = e.dsDocumento,
                        vlMovimento = e.vlMovimento,
                        dsTipo = e.dsTipo,
                        dsArquivo = e.dsArquivo,
                        conciliado = e.RecebimentoParcelas.Count > 0 || e.tbRecebimentoAjustes.Count > 0
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbExtrato;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar extrato" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }
        /// <summary>
        /// Adiciona nova TbExtrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbExtrato param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                // Valida param para não adicionar duplicidades
                tbExtrato extrato = _db.tbExtratos.Where(e => e.cdContaCorrente == param.cdContaCorrente)
                                                  .Where(e => e.dtExtrato.Equals(param.dtExtrato))
                                                  .Where(e => e.nrDocumento.Equals(param.nrDocumento))
                                                  .Where(e => e.vlMovimento == param.vlMovimento)
                                                  .Where(e => e.dsDocumento.Equals(param.dsDocumento))
                                                  .Where(e => e.dsTipo.Equals(param.dsTipo))
                                                  .FirstOrDefault();

                if (extrato != null) throw new Exception("Extrato já existe");
                _db.tbExtratos.Add(param);
                _db.SaveChanges();
                // Commit
                transaction.Commit();
                return param.idExtrato;
            }
            catch (Exception e)
            {
                // Rollback
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar extrato" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }


        /// <summary>
        /// Apaga uma TbExtrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idExtrato, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbExtrato extrato = _db.tbExtratos.Where(e => e.idExtrato == idExtrato).FirstOrDefault();
                if (extrato == null) throw new Exception("Extrato inexistente");
                if(extrato.RecebimentoParcelas.Count > 0 || extrato.tbRecebimentoAjustes.Count > 0)
                    throw new Exception("Movimentação bancária está envolvida em uma conciliação bancária!");

                // Remove o arquivo associado do disco, caso não tenha mais nenhum registro referenciando esse arquivo
                if (!extrato.dsArquivo.Equals("") && _db.tbExtratos.Where(e => e.dsArquivo.Equals(extrato.dsArquivo)).FirstOrDefault() == null)
                {
                    try
                    {
                        File.Delete(extrato.dsArquivo);
                    }catch { }
                }
                // Remove o extrato da base
                _db.tbExtratos.Remove(extrato);
                _db.SaveChanges();
                // Commit
                transaction.Commit();
            }
            catch (Exception e)
            {
                // Rollback
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar estrato" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }
        /*/// <summary>
        /// Altera tbExtrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbExtrato param)
        {
            tbExtrato value = _db.tbExtratos
                    .Where(e => e.idExtrato == param.idExtrato)
                    .First<tbExtrato>();

            if (value == null) throw new Exception("Extrato inexistente");

            if (param.nrDocumento != null && param.nrDocumento != value.nrDocumento)
                value.nrDocumento = param.nrDocumento;
            if (param.dtExtrato != null && param.dtExtrato != value.dtExtrato)
                value.dtExtrato = param.dtExtrato;
            if (param.vlMovimento != null && param.vlMovimento != value.vlMovimento)
                value.vlMovimento = param.vlMovimento;
            _db.SaveChanges();

        }*/


        /// <summary>
        /// Recebe o extrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object Patch(string token, Dictionary<string, string> queryString, tbLogAcessoUsuario log, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                string pastaExtratos = HttpContext.Current.Server.MapPath("~/App_Data/Extratos/");

                // Tem que estar associado a um grupo
                Int32 idGrupo = Permissoes.GetIdGrupo(token, _db);
                if (idGrupo == 0) throw new Exception("Grupo inválido");

                // Tem que informar por filtro a conta corrente
                string outValue = null;
                if (!queryString.TryGetValue("" + (int)CAMPOS.CDCONTACORRENTE, out outValue))
                    throw new Exception("Conta corrente não informada");

                #region OBTÉM E AVALIA CONTA INFORMADA
                Int32 cdContaCorrente = Convert.ToInt32(queryString["" + (int)CAMPOS.CDCONTACORRENTE]);
                tbContaCorrente conta = _db.tbContaCorrentes.Where(e => e.cdContaCorrente == cdContaCorrente).FirstOrDefault();
                if (conta == null) throw new Exception("Conta corrente inexistente");
                if (!conta.flAtivo) throw new Exception("Conta corrente está inativada");
                #endregion

                #region OBTÉM O DIRETÓRIO A SER SALVO O EXTRATO
                if (!Directory.Exists(pastaExtratos)) Directory.CreateDirectory(pastaExtratos);
                if (!Directory.Exists(pastaExtratos + idGrupo + "\\")) Directory.CreateDirectory(pastaExtratos + idGrupo + "/");
                // Diretório específico da conta
                string diretorio = pastaExtratos + idGrupo + "\\" + cdContaCorrente + "\\";
                if (!Directory.Exists(diretorio)) Directory.CreateDirectory(diretorio);
                #endregion

                HttpRequest httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    #region OBTÉM NOME ÚNICO PARA O ARQUIVO UPADO
                    // Arquivo upado
                    HttpPostedFile postedFile = httpRequest.Files[0];
                    // Obtém a extensão
                    string extensao = postedFile.FileName.LastIndexOf(".") > -1 ? postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(".")) : ".ofx";
                    // Obtém o nome do arquivo upado
                    string nomeArquivo = (postedFile.FileName.LastIndexOf(".") > -1 ? postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf(".")) : postedFile.FileName) + "_0" + extensao;

                    // Remove caracteres inválidos para nome de arquivo
                    nomeArquivo = Path.GetInvalidFileNameChars().Aggregate(nomeArquivo, (current, c) => current.Replace(c.ToString(), string.Empty));

                    // Valida o nome do arquivo dentro do diretório => deve ser único
                    int cont = 0;
                    while (File.Exists(diretorio + nomeArquivo))
                    {
                        // Novo nome
                        nomeArquivo = nomeArquivo.Substring(0, nomeArquivo.LastIndexOf("_") + 1);
                        nomeArquivo += ++cont + extensao;
                    }
                    #endregion
                    
                    #region SALVA ARQUIVO NO DISCO
                    string filePath = diretorio + nomeArquivo;
                    // Salva o arquivo
                    try
                    {
                        postedFile.SaveAs(filePath);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Não foi possível salvar o arquivo '" + filePath + "'! " + e.Message);
                    }
                    #endregion

                    // Loga o nome do arquivo
                    if (log != null) log.dsJson = filePath;

                    #region OBTÉM OBJETO ASSOCIADO AO EXTRATO
                    // VERIFICA EXTENSÃO DO ARQUIVO
                    OFXDocument ofxDocument = null;
                    if (extensao.ToLower().Equals(".ofx"))
                    {
                        // OFX
                        OFXDocumentParser parser = new OFXDocumentParser();
                        try
                        {
                            ofxDocument = parser.Import(new FileStream(filePath, FileMode.Open));
                        }
                        catch (Exception e)
                        {
                            // Deleta o arquivo
                            File.Delete(filePath);
                            throw new Exception("Arquivo não é um .ofx válido. " + e.Message);
                        }
                    }
                    else if (extensao.ToLower().Equals(".pdf"))
                    {
                        // PDF
                        if (!conta.cdBanco.Equals("033") && !conta.cdBanco.Equals("004"))
                        {
                            File.Delete(filePath); // Deleta o arquivo
                            throw new Exception("Só é aceito PDF de contas do banco do Nordeste (004) e do Santander (033) !");
                        }
                        // Parser
                        try
                        {
                            if (conta.cdBanco.Equals("033"))
                                ofxDocument = ExtratoPDFSantander.Import(filePath);
                            else
                                ofxDocument = ExtratoPDFBNB.Import(filePath);
                        }
                        catch (Exception e)
                        {
                            // Deleta o arquivo
                            File.Delete(filePath);
                            throw new Exception("Arquivo não é um .pdf válido! " + e.Message);
                        }

                    }
                    else
                    {
                        File.Delete(filePath); // Deleta o arquivo
                        throw new Exception("Formato de arquivo inválido!");
                    }
                    #endregion

                    /* 
                        CONTA
                        ofxDocument.Account.BankID : string // código do banco
                        ofxDocument.Account.BranchID : string // número da agência
                        ofxDocument.Account.AccountID : string // número da conta (pode conter também o número da agência)

                        BALANÇO
                        Balance.LedgerBalance : double // valor total do extrato

                        TRANSAÇÕES
                        Transactions : array de objetos
                          => TransType : int // tipo de transação
                          => Date : datetime // (com "T" entre date e time)
                          => Amount : double // valor da transação
                          => TransactionID = CheckNum: string
                          => Memo : string // descrição
                    */

                    #region VALIDA A CONTA QUE CONSTA NO EXTRATO
                    string banco = ofxDocument.Account.BankID.Trim();
                    string nrAgencia = ofxDocument.Account.BranchID.Trim();
                    string nrConta = ofxDocument.Account.AccountID.Trim();

                    #region VALIDA CÓDIGO DO BANCO
                    if (banco.Length > 3) banco = banco.Substring(banco.Length - 3, 3); // pega somente os últimos 3 dígitos
                    else
                    {
                        // Adiciona zeros a esquerda, caso tenha menos de 3 dígitos
                        while (banco.Length < 3) banco = "0" + banco;
                    }
                    if (!conta.cdBanco.Equals(banco))
                    {
                        // Deleta o arquivo
                        File.Delete(filePath);
                        throw new Exception("Extrato upado não corresponde ao banco da conta informada");
                    }
                    #endregion

                    #region VALIDA NÚMERO DA CONTA
                    if (!validaConta(conta.cdBanco, nrConta, conta.nrConta)) 
                    {
                        // Deleta o arquivo
                        File.Delete(filePath);
                        throw new Exception("Extrato upado não corresponde ao número da conta informada");
                    }
                    #endregion

                    // Valida o número da agência
                    /*if (!nrAgencia.Equals("") && !conta.nrAgencia.Equals(nrAgencia))
                    {
                        // Deleta o arquivo
                        File.Delete(filePath);
                        throw new Exception("Extrato upado não corresponde ao número da agência informada");
                    }*/

                    #endregion


                    bool armazenou = false;
                    DateTime? dataArmazenada = null;

                    // Tamanho máximo de dsArquivo é de 255 bytes
                    string dsArquivo = filePath.Length > 255 ? filePath.Substring(filePath.Length - 255) : filePath;
                        
                    DateTime dataGeracaoExtrato = Convert.ToDateTime(ofxDocument.SignOn.DTServer.ToShortDateString());
                    if (dataGeracaoExtrato.Year == 1 && dataGeracaoExtrato.Month == 1 && dataGeracaoExtrato.Day == 1)
                        // Não veio no documento => Usa a data atual
                        dataGeracaoExtrato = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                    //List<Transaction> transacoes = ofxDocument.Transactions.Where(t => t.Date.Year == 1 && t.Date.Month == 1 && t.Date.Day == 1).ToList();

                    #region ARMAZENA MOVIMENTAÇÕES BANCÁRIAS
                    foreach (var transacao in ofxDocument.Transactions)
                    {
                        #region OBTÉM O OBJETO CORRESPONDENTE A MOVIMENTAÇÃO
                        tbExtrato extrato = new tbExtrato();
                        extrato.cdContaCorrente = cdContaCorrente;
                        extrato.dtExtrato = new DateTime(transacao.Date.Year, transacao.Date.Month, transacao.Date.Day);
                        extrato.vlMovimento = transacao.Amount;
                        extrato.nrDocumento = transacao.CheckNum;
                        extrato.dsArquivo = dsArquivo;//filePath;
                        // OTHER => TRANSFORMA PARA CREDIT OU DEBIT
                        //if (transacao.TransType.Equals(OFXTransactionType.OTHER))
                        //    transacao.TransType = extrato.vlMovimento < 0 ? OFXTransactionType.DEBIT : OFXTransactionType.CREDIT;
                        extrato.dsTipo = extrato.vlMovimento < 0 ? OFXTransactionType.DEBIT.ToString() : OFXTransactionType.CREDIT.ToString();//transacao.TransType.ToString();

                        bool memo = true;
                        extrato.dsDocumento = "";
                        if (transacao.Memo != null && !transacao.Memo.Equals(""))
                            extrato.dsDocumento = transacao.Memo;
                        else if (transacao.Name != null)
                        {
                            extrato.dsDocumento = transacao.Name;
                            memo = false;
                        }

                        #endregion
                        
                        // Não importa movimentações em que a data é igual a data da geração do extrato
                        if (dataGeracaoExtrato.Year < extrato.dtExtrato.Year ||
                            (dataGeracaoExtrato.Year == extrato.dtExtrato.Year && dataGeracaoExtrato.Month < extrato.dtExtrato.Month) ||
                            (dataGeracaoExtrato.Year == extrato.dtExtrato.Year && dataGeracaoExtrato.Month == extrato.dtExtrato.Month && dataGeracaoExtrato.Day <= extrato.dtExtrato.Day))
                            //dataGeracaoExtrato <= extrato.dtExtrato)
                                continue; // Não importa

                        DbContextTransaction transaction = _db.Database.BeginTransaction();

                        try
                        {

                            #region OBTÉM O TOTAL DE MOVIMENTAÇÕES REPETIDAS PARA A MOVIMENTAÇÃO CORRENTE
                            IEnumerable<Transaction> trans = ofxDocument.Transactions.Where(t => t.Amount == extrato.vlMovimento)
                                                                                     .Where(t => t.Date.Year == extrato.dtExtrato.Year)
                                                                                     .Where(t => t.Date.Month == extrato.dtExtrato.Month)
                                                                                     .Where(t => t.Date.Day == extrato.dtExtrato.Day)
                                //.Where(t => t.TransType.ToString().Equals(extrato.dsTipo))
                                                                                     .Where(t => t.CheckNum.Equals(extrato.nrDocumento));
                            if (!memo) trans = trans.Where(t => t.Name.Equals(extrato.dsDocumento));
                            else trans = trans.Where(t => t.Memo.Equals(extrato.dsDocumento));
                            Int32 contMovimentacoesRepetidas = trans.Count();
                            #endregion

                            #region OBTÉM AS MOVIMENTAÇÕES JÁ ARMAZENADAS NA BASE QUE POSSUEM MESMAS INFORMAÇÕES DA MOVIMENTAÇÃO CORRENTE
                            IQueryable<tbExtrato> olds = _db.tbExtratos.Where(e => e.cdContaCorrente == extrato.cdContaCorrente)
                                //.Where(e => e.dtExtrato.Equals(extrato.dtExtrato))
                                                                 .Where(e => e.dtExtrato.Year == extrato.dtExtrato.Year)
                                                                 .Where(e => e.dtExtrato.Month == extrato.dtExtrato.Month)
                                                                 .Where(e => e.dtExtrato.Day == extrato.dtExtrato.Day)
                                                                 .Where(e => e.nrDocumento.Equals(extrato.nrDocumento))
                                                                 .Where(e => e.vlMovimento == extrato.vlMovimento)
                                                                 .Where(e => e.dsDocumento.Equals(extrato.dsDocumento))
                                                                 .OrderBy(e => e.dtExtrato);
                            Int32 contExtratosBD = olds.Count();
                            #endregion

                            if (contExtratosBD >= contMovimentacoesRepetidas)
                            {
                                // Já existe o(s) registro(s) com essas informações!
                                #region SE O ARQUIVO ATUAL TEM MAIS MOVIMENTAÇÕES, ATUALIZA A REFERÊNCIA DE ARQUIVO DOS EXTRATOS QUE JÁ ESTÃO NA BASE
                                string arquivoAntigo = olds.Select(o => o.dsArquivo).FirstOrDefault();
                                int totalTransacoesOld = _db.tbExtratos.Where(e => e.dsArquivo.Equals(arquivoAntigo)).Count();
                                // Verifica se o extrato atual possui mais movimentações que o anterior
                                if (ofxDocument.Transactions.Count >= totalTransacoesOld)
                                {
                                    // Atualiza o arquivo
                                    for (int k = 0; k < contExtratosBD; k++)
                                    {
                                        tbExtrato ext = olds.Skip(k).Take(1).FirstOrDefault();
                                        ext.dsArquivo = extrato.dsArquivo;
                                        ext.dsTipo = extrato.dsTipo;
                                        try
                                        {
                                            _db.SaveChanges();
                                        }
                                        catch(Exception e) 
                                        {
                                            _db.Entry(ext).Reload();
                                        }
                                    }
                                    // Ainda tem movimentações referenciando o arquivo antigo?
                                    if (totalTransacoesOld <= 1)
                                    {
                                        try
                                        {
                                            File.Delete(arquivoAntigo); // Deleta o arquivo antigo
                                        }
                                        catch { }
                                    }
                                }
                                else
                                {
                                    // Ajusta o tipo, que poderia estar como OTHER ou DEP
                                    for (int k = 0; k < contExtratosBD; k++)
                                    {
                                        tbExtrato ext = olds.Skip(k).Take(1).FirstOrDefault();
                                        ext.dsTipo = extrato.dsTipo;
                                        try
                                        {
                                            _db.SaveChanges();
                                        }
                                        catch(Exception e)
                                        {
                                            _db.Entry(ext).Reload();
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                // Salva uma nova movimentação
                                _db.tbExtratos.Add(extrato);
                                _db.SaveChanges();
                                armazenou = true;
                                if (dataArmazenada == null) dataArmazenada = extrato.dtExtrato;
                            }

                            transaction.Commit();

                            #region SALVA PARÂMETRO BANCÁRIO
                            tbBancoParametro parametro = new tbBancoParametro();
                            parametro.cdAdquirente = null;
                            parametro.cdBandeira = null;
                            parametro.dsTipoCartao = null;
                            parametro.nrCnpj = null;
                            parametro.cdBanco = conta.cdBanco;
                            parametro.dsMemo = extrato.dsDocumento;
                            parametro.dsTipo = extrato.dsTipo;
                            parametro.flVisivel = true;
                            try
                            {
                                GatewayTbBancoParametro.Add(token, parametro, _db);
                            }
                            catch (Exception e)
                            { }
                            #endregion
                        }
                        catch(Exception e)
                        {
                            transaction.Rollback();
                            if (e is DbEntityValidationException)
                            {
                                string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                                throw new Exception(erro.Equals("") ? "Falha ao salvar movimentação bancária" : erro);
                            }
                            throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
                        }
                    }
                    #endregion

                    // Commit
                    //transaction.Commit();

                    // Se não armazenou pelo menos um elemento, deleta o arquivo do disco
                    if (!armazenou)
                    {
                        try
                        {
                            File.Delete(filePath); // Deleta o arquivo
                        }
                        catch { }
                    }

                    if (dataArmazenada == null) dataArmazenada = ofxDocument.StatementStart;

                    return new
                    {
                        ano = dataArmazenada != null ? dataArmazenada.Value.Year : 0,
                        mes = dataArmazenada != null ? dataArmazenada.Value.Month : 0
                    };

                }
                else throw new Exception("400"); // Bad Request
            }
            catch (Exception e)
            {
                // Rollback
                //transaction.Rollback();
                /*if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao enviar extrato" : erro);
                }*/
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }


        /// <summary>
        /// Valida se a conta fornecida no documento é a mesma constatada no banco
        /// </summary>
        /// <param name="codBanco">Código do banco</param>
        /// <param name="contaOFX">Número da conta fornecido pelo documento</param>
        /// <param name="contaBD">Número da conta salvo na base</param>
        /// <returns></returns>
        private static bool validaConta(string codBanco, string contaOFX, string contaBD)
        {
            string codBrasil = "001";
            string codBanese = "047"; // conta + hífen + dígito
            string codCaixa = "104";  // conta precedidas de zeros
            string codItau = "341";   // agencia + conta
            string codBradesco = "237";
            //string codSantander = "033"; // conta + hífen + dígito

            string contaDocumento = contaOFX;
            // BRADESCO INCLUI NO <ACCTID> nrAgencia/nrConta
            if (contaDocumento.Contains("/"))
            {
                // Agência e conta estão "juntas"
                //agencia = contaDocumento.Substring(0, contaDocumento.IndexOf("/"));
                contaDocumento = contaDocumento.Substring(contaDocumento.IndexOf("/") + 1);
            }
            
            // BANESE INCLUI UM "(<num>)" no final do <ACCTID>
            if (contaDocumento.Contains("("))
                contaDocumento = contaDocumento.Substring(0, contaDocumento.IndexOf("("));

            // ITAU => Campo da Conta vem com a AGÊNCIA sendo os 4 primeiros dígitos
            if (codBanco.Equals(codItau) && contaDocumento.Length > 5)
            {
                //agencia = contaDocumento.Substring(0, 4);
                contaDocumento = contaDocumento.Substring(4);
            }

            // Remove os possíveis zeros a esquerda
            string contaBDSemZeros = contaBD;
            while (contaBDSemZeros[0] == '0')
                contaBDSemZeros = contaBDSemZeros.Substring(1);

            // Deixa os nomes com mesmo comprimento
            string contaBDZeros = contaBD;
            while (contaBDZeros.Length < contaDocumento.Length)
                contaBDZeros = "0" + contaBDZeros; // adiciona zeros a esquerda

            // Adiciona zeros a esquerda
            while (contaDocumento.Length < contaBDZeros.Length)
                contaDocumento = "0" + contaDocumento; // adiciona zeros a esquerda

            // Verifica se as contas são iguais
            if (contaBDZeros.Equals(contaDocumento)) return true;

            // Tem bancos que coloca no campo associado a conta a agência seguida da conta
            if (contaDocumento.EndsWith(contaBDSemZeros)) return true;


            // BANESE e BANCO DO BRASIL => COM DÍGITO VERIFICADOR, EXPLICITADO COM HÍFEN
            if (codBanco.Equals(codBrasil) || codBanco.Equals(codBanese))
            {
                // Se já tiver o dígito verificador, então de fato a conta está errada
                if (contaBDSemZeros.Contains("-")) return false; 
                // Coloca o hífen antes do último dígito
                string contaBDComHifen = contaBDSemZeros.Substring(0, contaBDSemZeros.Length - 1) + "-" + contaBDSemZeros.Substring(contaBDSemZeros.Length - 1);
                return contaDocumento.EndsWith(contaBDComHifen);
            }

            // CAIXA E BRADESCO => SEM HÍFEN E DÍGITO VERIFICADOR
            if (codBanco.Equals(codCaixa) || codBradesco.Equals(codBradesco))
            {
                // Se a conta já não tem hifen, então de fato está errada
                if (!contaBDSemZeros.Contains("-")) return false; 
                // Remove o hífen e o dígito verificador
                string contaBDSemHifenEDigito = contaBDSemZeros.Substring(0, contaBDSemZeros.IndexOf('-'));
                string contaBDSemHifen = contaBDSemZeros.Substring(0, contaBDSemZeros.IndexOf('-')) + contaBDSemZeros.Substring(contaBDSemZeros.IndexOf('-') + 1);
                return contaDocumento.EndsWith(contaBDSemHifenEDigito) || contaDocumento.EndsWith(contaBDSemHifen);
            }

            // ITAU => COM DÍGITO VERIFICADOR, MAS SEM HIFEN
            if (codBanco.Equals(codItau))
            {
                // Se a conta já não tem hifen, então de fato está errada
                if (!contaBDSemZeros.Contains("-")) return false;
                // Remove o hífen, mas mantém o dígito verificador
                string contaBDSemHifen = contaBDSemZeros.Substring(0, contaBDSemZeros.IndexOf('-')) + contaBDSemZeros.Substring(contaBDSemZeros.IndexOf('-') + 1);
                return contaDocumento.EndsWith(contaBDSemHifen);
            }

            // SANTANDER => POSSUI DOIS HÍFENS : <CODOPERACAO>-<CONTA>-<DIGITO>
            // OBS: O parser do PDF já trata de considerar apenas o conteúdo após o primeiro hífen
            /*if (codBanco.Equals(codSantander))
            {

            }*/

            return false;
        }

    }
}
