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
using System.Web.Http;
using api.Negocios.Card;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using api.Negocios.Util;
using api.Negocios.Cliente;

namespace api.Negocios.Pos
{
    public class GatewayRecebimentoParcela
    {
        //public static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRecebimentoParcela()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "RP";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDRECEBIMENTO = 100,
            NUMPARCELA = 101,
            VALORPARCELABRUTA = 102,
            VALORPARCELALIQUIDA = 103,
            DTARECEBIMENTO = 104,
            VALORDESCONTADO = 105,
            IDEXTRATO = 106, // -1 para = null, 0 para != null
            DTARECEBIMENTOEFETIVO = 107,
            VLDESCONTADOANTECIPACAO = 108,
            IDRECEBIMENTOTITULO = 109, // -1 para = null, 0 para != null
            FLANTECIPADO = 110,

            // EMPRESA
            NU_CNPJ = 300,
            DS_FANTASIA = 304,
            ID_GRUPO = 316,

            // OPERADORA (ADQUIRENTE)
            IDOPERADORA = 400,
            NMOPERADORA = 401,

            // BANDEIRA
            IDBANDEIRA = 500,
            DESBANDEIRA = 501,

            // RECEBIMENTO
            NSU = 603,
            DTAVENDA = 605,
            CODRESUMOVENDA = 613,

            // TBADQUIRENTE
            CDADQUIRENTE = 700,

            // TBBANDEIRA
            CDBANDEIRA = 800,
            DSTIPO = 803,

            CDCONTACORRENTE = 900,

            //EXPORTAR
            EXPORTAR = 9999

        };

        public enum MES
        {
            Janeiro = 1, Fevereiro = 2, Março, Abril, Maio, Junho, Julho, Agosto, Setembro,
            Outubro, Novembro, Dezembro
        };

        /// <summary>
        /// Get RecebimentoParcela/RecebimentoParcela
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static IQueryable<RecebimentoParcela> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.RecebimentoParcelas.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDRECEBIMENTO:
                        Int32 idRecebimento = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRecebimento.Equals(idRecebimento));
                        break;
                    case CAMPOS.NUMPARCELA:
                        Int32 numParcela = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.numParcela.Equals(numParcela));
                        break;
                    case CAMPOS.VALORPARCELABRUTA:
                        decimal valorParcelaBruta = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorParcelaBruta.Equals(valorParcelaBruta));
                        break;
                    case CAMPOS.VALORPARCELALIQUIDA:
                        decimal valorParcelaLiquida = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorParcelaLiquida.Equals(valorParcelaLiquida));
                        break;
                    case CAMPOS.IDEXTRATO:
                        Int32 idExtrato = Convert.ToInt32(item.Value);
                        if (idExtrato == -1) entity = entity.Where(e => e.idExtrato == null);
                        else if (idExtrato == 0) entity = entity.Where(e => e.idExtrato != null);
                        else entity = entity.Where(e => e.idExtrato == idExtrato);
                        break;
                    case CAMPOS.IDRECEBIMENTOTITULO:
                        Int32 idRecebimentoTitulo = Convert.ToInt32(item.Value);
                        if (idRecebimentoTitulo == -1) entity = entity.Where(e => e.idRecebimentoTitulo == null);
                        else if (idRecebimentoTitulo == 0) entity = entity.Where(e => e.idRecebimentoTitulo != null);
                        else entity = entity.Where(e => e.idRecebimentoTitulo == idRecebimentoTitulo);
                        break;

                    /// PERSONALIZADO

                    case CAMPOS.DTARECEBIMENTO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            /*
                            entity = entity.Where(e => (e.dtaRecebimento.Year > dtaIni.Year || (e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month > dtaIni.Month) ||
                                                                                          (e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month == dtaIni.Month && e.dtaRecebimento.Day >= dtaIni.Day))
                                                    && (e.dtaRecebimento.Year < dtaFim.Year || (e.dtaRecebimento.Year == dtaFim.Year && e.dtaRecebimento.Month < dtaFim.Month) ||
                                                                                          (e.dtaRecebimento.Year == dtaFim.Year && e.dtaRecebimento.Month == dtaFim.Month && e.dtaRecebimento.Day <= dtaFim.Day)));
                            */
                            entity = entity.Where(e => e.dtaRecebimento > dtaIni && e.dtaRecebimento < dtaFim);
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaRecebimento >= dta && e.dtaRecebimentoEfetivo == null);
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaRecebimento <= dta);
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaRecebimento.Year == dtaIni.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month == dtaIni.Month);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month == dtaIni.Month && e.dtaRecebimento.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.VALORDESCONTADO:
                        decimal valorDescontado = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorDescontado == valorDescontado);
                        break;
                    case CAMPOS.VLDESCONTADOANTECIPACAO:
                        decimal vlDescontadoAntecipacao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlDescontadoAntecipacao == vlDescontadoAntecipacao);
                        break;
                    case CAMPOS.DTARECEBIMENTOEFETIVO: // Para os que este campo for null, pega o dtaRecebimento
                        if (item.Value.Trim().Equals(""))
                        {
                            entity = entity.Where(e => e.dtaRecebimentoEfetivo == null);
                        }
                        else if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtaRecebimentoEfetivo != null &&
                                                        (e.dtaRecebimentoEfetivo.Value.Year > dtaIni.Year || (e.dtaRecebimentoEfetivo.Value.Year == dtaIni.Year && e.dtaRecebimentoEfetivo.Value.Month > dtaIni.Month) || (e.dtaRecebimentoEfetivo.Value.Year == dtaIni.Year && e.dtaRecebimentoEfetivo.Value.Month == dtaIni.Month && e.dtaRecebimentoEfetivo.Value.Day >= dtaIni.Day)) &&
                                                        (e.dtaRecebimentoEfetivo.Value.Year < dtaFim.Year || (e.dtaRecebimentoEfetivo.Value.Year == dtaFim.Year && e.dtaRecebimentoEfetivo.Value.Month < dtaFim.Month) || (e.dtaRecebimentoEfetivo.Value.Year == dtaFim.Year && e.dtaRecebimentoEfetivo.Value.Month == dtaFim.Month && e.dtaRecebimentoEfetivo.Value.Day <= dtaFim.Day))
                                                       ) ||
                                                       (e.dtaRecebimentoEfetivo == null && e.flAntecipado == false &&
                                                        (e.dtaRecebimento.Year > dtaIni.Year || (e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month > dtaIni.Month) || (e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month == dtaIni.Month && e.dtaRecebimento.Day >= dtaIni.Day)) &&
                                                        (e.dtaRecebimento.Year < dtaFim.Year || (e.dtaRecebimento.Year == dtaFim.Year && e.dtaRecebimento.Month < dtaFim.Month) || (e.dtaRecebimento.Year == dtaFim.Year && e.dtaRecebimento.Month == dtaFim.Month && e.dtaRecebimento.Day <= dtaFim.Day))
                                                       ));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtaRecebimentoEfetivo != null && e.dtaRecebimentoEfetivo.Value >= dta) || (e.dtaRecebimentoEfetivo == null && e.flAntecipado == false && e.dtaRecebimento >= dta));
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtaRecebimentoEfetivo != null && e.dtaRecebimentoEfetivo.Value <= dta) || (e.dtaRecebimentoEfetivo == null && e.flAntecipado == false && e.dtaRecebimento <= dta));
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtaRecebimentoEfetivo != null && e.dtaRecebimentoEfetivo.Value.Year == dtaIni.Year) || (e.dtaRecebimentoEfetivo == null && e.flAntecipado == false && e.dtaRecebimento.Year == dtaIni.Year));
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtaRecebimentoEfetivo != null && e.dtaRecebimentoEfetivo.Value.Year == dtaIni.Year && e.dtaRecebimentoEfetivo.Value.Month == dtaIni.Month) || (e.dtaRecebimentoEfetivo == null && e.flAntecipado == false && e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month == dtaIni.Month));
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtaRecebimentoEfetivo != null && e.dtaRecebimentoEfetivo.Value.Year == dtaIni.Year && e.dtaRecebimentoEfetivo.Value.Month == dtaIni.Month && e.dtaRecebimentoEfetivo.Value.Day == dtaIni.Day) || (e.dtaRecebimentoEfetivo == null && e.flAntecipado == false && e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month == dtaIni.Month && e.dtaRecebimento.Day == dtaIni.Day));
                        }
                        break;
                    case CAMPOS.FLANTECIPADO:
                        bool flAntecipado = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flAntecipado == flAntecipado);
                        break;




                    // PERSONALIZADO

                    case CAMPOS.ID_GRUPO:
                        int id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.Recebimento.empresa.id_grupo == id_grupo);
                        break;
                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.Recebimento.empresa.nu_cnpj.Equals(nu_cnpj));
                        break;
                    case CAMPOS.DS_FANTASIA:
                        string ds_fantasia = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.Recebimento.empresa.ds_fantasia.Equals(ds_fantasia));
                        break;

                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.Recebimento.BandeiraPos.Operadora.id == idOperadora).AsQueryable();
                        break;
                    case CAMPOS.NMOPERADORA:
                        string nmOperadora = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.Recebimento.BandeiraPos.Operadora.nmOperadora.Equals(nmOperadora));
                        break;

                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.Recebimento.BandeiraPos.id == idBandeira);
                        break;

                    case CAMPOS.DESBANDEIRA:
                        string desBandeira = Convert.ToString(item.Value).ToUpper();
                        entity = entity.Where(e => e.Recebimento.BandeiraPos.desBandeira.ToUpper().Equals(desBandeira));
                        break;

                    case CAMPOS.DTAVENDA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.Recebimento.dtaVenda.Year > dtaIni.Year || (e.Recebimento.dtaVenda.Year == dtaIni.Year && e.Recebimento.dtaVenda.Month > dtaIni.Month) ||
                                                                                          (e.Recebimento.dtaVenda.Year == dtaIni.Year && e.Recebimento.dtaVenda.Month == dtaIni.Month && e.Recebimento.dtaVenda.Day >= dtaIni.Day))
                                                    && (e.Recebimento.dtaVenda.Year < dtaFim.Year || (e.Recebimento.dtaVenda.Year == dtaFim.Year && e.Recebimento.dtaVenda.Month < dtaFim.Month) ||
                                                                                          (e.Recebimento.dtaVenda.Year == dtaFim.Year && e.Recebimento.dtaVenda.Month == dtaFim.Month && e.Recebimento.dtaVenda.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.Recebimento.dtaVenda >= dta);
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.Recebimento.dtaVenda <= dta);
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.Recebimento.dtaVenda.Year == dtaIni.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.Recebimento.dtaVenda.Year == dtaIni.Year && e.Recebimento.dtaVenda.Month == dtaIni.Month);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.Recebimento.dtaVenda.Year == dtaIni.Year && e.Recebimento.dtaVenda.Month == dtaIni.Month && e.Recebimento.dtaVenda.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        if (nsu.Contains("%")) // usa LIKE => STARTS WITH
                        {
                            string busca = nsu.Replace("%", "").ToString();
                            entity = entity.Where(e => e.Recebimento.nsu.EndsWith(busca));
                        }
                        else
                            entity = entity.Where(e => e.Recebimento.nsu.Equals(nsu)).AsQueryable();
                        break;
                    case CAMPOS.CODRESUMOVENDA:
                        string codResumoVenda = Convert.ToString(item.Value);
                        if (codResumoVenda.Contains("%")) // usa LIKE => STARTS WITH
                        {
                            string busca = codResumoVenda.Replace("%", "").ToString();
                            entity = entity.Where(e => e.Recebimento.codResumoVenda.StartsWith(busca));
                        }
                        else
                            entity = entity.Where(e => e.Recebimento.codResumoVenda.Equals(codResumoVenda)).AsQueryable();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.Recebimento.cdBandeira != null && e.Recebimento.tbBandeira.cdAdquirente == cdAdquirente).AsQueryable();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        if (cdBandeira == -1)
                            entity = entity.Where(e => e.Recebimento.cdBandeira == null).AsQueryable();
                        else if (cdBandeira == 0)
                            entity = entity.Where(e => e.Recebimento.cdBandeira != null).AsQueryable();
                        else
                            entity = entity.Where(e => e.Recebimento.cdBandeira == cdBandeira).AsQueryable();
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value).TrimEnd();
                        entity = entity.Where(e => e.Recebimento.cdBandeira != null && e.Recebimento.tbBandeira.dsTipo.TrimEnd().Equals(dsTipo)).AsQueryable();
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        if (cdContaCorrente > 0)
                        {
                            // Obtém as filiais da conta
                            string[] filiaisDaConta = Permissoes.GetFiliaisDaConta(cdContaCorrente, _db);
                            int[] adquirentesDaConta = Permissoes.GetAdquirentesDaConta(cdContaCorrente, _db);
                            entity = entity.Where(e => filiaisDaConta.Contains(e.Recebimento.cnpj))
                                           .Where(e => adquirentesDaConta.Contains(e.Recebimento.tbBandeira.cdAdquirente))
                                           .AsQueryable();

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

                case CAMPOS.IDRECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRecebimento);
                    else entity = entity.OrderByDescending(e => e.idRecebimento);
                    break;
                case CAMPOS.NUMPARCELA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numParcela);
                    else entity = entity.OrderByDescending(e => e.numParcela);
                    break;
                case CAMPOS.VALORPARCELABRUTA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorParcelaBruta);
                    else entity = entity.OrderByDescending(e => e.valorParcelaBruta);
                    break;
                case CAMPOS.VALORPARCELALIQUIDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorParcelaLiquida);
                    else entity = entity.OrderByDescending(e => e.valorParcelaLiquida);
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Recebimento.empresa.ds_fantasia).ThenBy(e => e.Recebimento.empresa.filial).ThenBy(e => e.dtaRecebimento).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira).ThenBy(e => e.Recebimento.dtaVenda);
                    else entity = entity.OrderBy(e => e.Recebimento.empresa.ds_fantasia).ThenBy(e => e.Recebimento.empresa.filial).ThenByDescending(e => e.dtaRecebimento).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira).ThenBy(e => e.Recebimento.dtaVenda);
                    break;
                case CAMPOS.VALORDESCONTADO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorDescontado);
                    else entity = entity.OrderByDescending(e => e.valorDescontado);
                    break;
                case CAMPOS.VLDESCONTADOANTECIPACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlDescontadoAntecipacao);
                    else entity = entity.OrderByDescending(e => e.vlDescontadoAntecipacao);
                    break;

                // PERSONALIZADO
                case CAMPOS.DTAVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Recebimento.empresa.ds_fantasia).ThenBy(e => e.Recebimento.empresa.filial).ThenBy(e => e.Recebimento.dtaVenda).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira).ThenBy(e => e.dtaRecebimento);
                    else entity = entity.OrderBy(e => e.Recebimento.empresa.ds_fantasia).ThenBy(e => e.Recebimento.empresa.filial).ThenByDescending(e => e.Recebimento.dtaVenda).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira).ThenBy(e => e.dtaRecebimento);
                    break;
                case CAMPOS.DS_FANTASIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Recebimento.empresa.ds_fantasia).ThenBy(e => e.Recebimento.empresa.filial).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira);
                    else entity = entity.OrderByDescending(e => e.Recebimento.empresa.ds_fantasia).ThenByDescending(e => e.Recebimento.empresa.filial).ThenByDescending(e => e.Recebimento.BandeiraPos.desBandeira); ;
                    break;
                case CAMPOS.DESBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Recebimento.BandeiraPos.desBandeira);
                    else entity = entity.OrderByDescending(e => e.Recebimento.BandeiraPos.desBandeira);
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


                    case CAMPOS.IDRECEBIMENTO:
                        Int32 idRecebimento = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".idRecebimento = " + idRecebimento);
                        break;
                    case CAMPOS.NUMPARCELA:
                        Int32 numParcela = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".numParcela = " + numParcela);
                        break;
                    case CAMPOS.VALORPARCELABRUTA:
                        decimal valorParcelaBruta = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".valorParcelaBruta = " + valorParcelaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.VALORPARCELALIQUIDA:
                        decimal valorParcelaLiquida = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".valorParcelaLiquida = " + valorParcelaLiquida.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.IDEXTRATO:
                        Int32 idExtrato = Convert.ToInt32(item.Value);
                        if (idExtrato == -1) where.Add("idExtrato IS NULL");
                        else if (idExtrato == 0) where.Add("idExtrato IS NOT NULL");
                        else where.Add(SIGLA_QUERY + ".idExtrato = " + idExtrato);
                        break;
                    case CAMPOS.IDRECEBIMENTOTITULO:
                        Int32 idRecebimentoTitulo = Convert.ToInt32(item.Value);
                        if (idRecebimentoTitulo == -1) where.Add("idRecebimentoTitulo IS NULL");
                        else if (idRecebimentoTitulo == 0) where.Add("idRecebimentoTitulo IS NOT NULL");
                        else where.Add(SIGLA_QUERY + ".idRecebimentoTitulo = " + idRecebimentoTitulo);
                        break;

                    /// PERSONALIZADO

                    case CAMPOS.DTARECEBIMENTO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add(SIGLA_QUERY + ".dtaRecebimento BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtaRecebimento >= '" + dt + "' AND " + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NULL");
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add(SIGLA_QUERY + ".dtaRecebimento <= '" + dt + " 23:59:00'");
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("DATEPART(YEAR, " + SIGLA_QUERY + ".dtaRecebimento) = " + data.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("DATEPART(YEAR, " + SIGLA_QUERY + ".dtaRecebimento) = " + data.Year + " AND DATEPART(MONTH, " + SIGLA_QUERY + ".dtaRecebimento) = " + data.Month);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(data);
                            where.Add(SIGLA_QUERY + ".dtaRecebimento BETWEEN '" + dt + "' AND '" + dt + " 23:59:00'");
                        }
                        break;
                    case CAMPOS.VALORDESCONTADO:
                        decimal valorDescontado = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".valorDescontado = " + valorDescontado.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.VLDESCONTADOANTECIPACAO:
                        decimal vlDescontadoAntecipacao = Convert.ToDecimal(item.Value);
                        where.Add(SIGLA_QUERY + ".vlDescontadoAntecipacao = " + vlDescontadoAntecipacao.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        break;
                    case CAMPOS.DTARECEBIMENTOEFETIVO: // Para os que este campo for null, pega o dtaRecebimento
                        if (item.Value.Trim().Equals(""))
                        {
                            where.Add(SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NULL");
                        }
                        else if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);
                            where.Add("(" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL AND " +
                                            SIGLA_QUERY + ".dtaRecebimentoEfetivo BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00')" +
                                      " OR (" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NULL AND " + SIGLA_QUERY + ".flAntecipado = 0 AND " +
                                                SIGLA_QUERY + ".dtaRecebimento BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00')");
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add("(" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL AND " +
                                            SIGLA_QUERY + ".dtaRecebimentoEfetivo >= '" + dt + "')" +
                                      " OR (" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NULL AND " + SIGLA_QUERY + ".flAntecipado = 0 AND " +
                                                SIGLA_QUERY + ".dtaRecebimento >= '" + dt + "')");
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add("(" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL AND " +
                                            SIGLA_QUERY + ".dtaRecebimentoEfetivo <= '" + dt + " 23:59:00')" +
                                      " OR (" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NULL AND " + SIGLA_QUERY + ".flAntecipado = 0 AND " +
                                                SIGLA_QUERY + ".dtaRecebimento <= '" + dt + " 23:59:00')");
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("(" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL AND " +
                                            "DATEPART(YEAR, " + SIGLA_QUERY + ".dtaRecebimentoEfetivo) = " + dta.Year + ")" +
                                      " OR (" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NULL AND " + SIGLA_QUERY + ".flAntecipado = 0 AND " +
                                                "DATEPART(YEAR, " + SIGLA_QUERY + ".dtaRecebimento) = " + dta.Year + ")");

                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("(" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL AND " +
                                            "DATEPART(YEAR, " + SIGLA_QUERY + ".dtaRecebimentoEfetivo) = " + dta.Year + " AND " +
                                            "DATEPART(MONTH, " + SIGLA_QUERY + ".dtaRecebimentoEfetivo) = " + dta.Month + ")" +
                                      " OR (" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NULL AND " + SIGLA_QUERY + ".flAntecipado = 0 AND " +
                                                "DATEPART(MONTH, " + SIGLA_QUERY + ".dtaRecebimento) = " + dta.Month + " AND " +
                                                "DATEPART(MONTH, " + SIGLA_QUERY + ".dtaRecebimento) = " + dta.Month + ")");
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string dt = DataBaseQueries.GetDate(dta);
                            where.Add("(" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL AND " +
                                            SIGLA_QUERY + ".dtaRecebimentoEfetivo BETWEEN '" + dt + "' AND '" + dt + " 23:59:00')" +
                                      " OR (" + SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NULL AND " + SIGLA_QUERY + ".flAntecipado = 0 AND " +
                                                SIGLA_QUERY + ".dtaRecebimento BETWEEN '" + dt + "' AND '" + dt + " 23:59:00')");
                        }
                        break;
                    case CAMPOS.FLANTECIPADO:
                        bool flAntecipado = Convert.ToBoolean(item.Value);
                        where.Add(SIGLA_QUERY + ".flAntecipado = " + DataBaseQueries.GetBoolean(flAntecipado));
                        break;




                    // PERSONALIZADO

                    case CAMPOS.ID_GRUPO:
                        int id_grupo = Convert.ToInt32(item.Value);
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");
                        where.Add(GatewayEmpresa.SIGLA_QUERY + ".id_grupo = " + id_grupo);
                        break;
                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        where.Add(GatewayRecebimento.SIGLA_QUERY + ".cnpj = '" + nu_cnpj + "'");
                        break;
                    case CAMPOS.DS_FANTASIA:
                        string ds_fantasia = Convert.ToString(item.Value);
                        // Adiciona os joins
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");
                        where.Add(GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia = '" + ds_fantasia + "'");
                        break;

                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        // Adiciona o joins
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        if (!join.ContainsKey("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY, " ON " + GatewayBandeiraPos.SIGLA_QUERY + ".id = " + GatewayRecebimento.SIGLA_QUERY + ".idBandeira");
                        where.Add(GatewayBandeiraPos.SIGLA_QUERY + ".idOperadora = " + idOperadora);
                        break;
                    case CAMPOS.NMOPERADORA:
                        string nmOperadora = Convert.ToString(item.Value);
                        // Adiciona o joins
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        if (!join.ContainsKey("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY, " ON " + GatewayBandeiraPos.SIGLA_QUERY + ".id = " + GatewayRecebimento.SIGLA_QUERY + ".idBandeira");
                        if (!join.ContainsKey("INNER JOIN pos.Operadora " + GatewayOperadora.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Operadora " + GatewayOperadora.SIGLA_QUERY, " ON " + GatewayOperadora.SIGLA_QUERY + ".id = " + GatewayBandeiraPos.SIGLA_QUERY + ".idOperadora");
                        where.Add(GatewayOperadora.SIGLA_QUERY + ".nmOperadora = '" + nmOperadora + "'");
                        break;

                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        where.Add(GatewayRecebimento.SIGLA_QUERY + ".idBandeira = " + idBandeira);
                        break;

                    case CAMPOS.DESBANDEIRA:
                        string desBandeira = Convert.ToString(item.Value).ToUpper();
                        // Adiciona o joins
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        if (!join.ContainsKey("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY, " ON " + GatewayBandeiraPos.SIGLA_QUERY + ".id = " + GatewayRecebimento.SIGLA_QUERY + ".idBandeira");
                        where.Add(GatewayBandeiraPos.SIGLA_QUERY + ".desBandeira = '" + desBandeira + "'");
                        break;

                    case CAMPOS.DTAVENDA:
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                            
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string dtInicio = DataBaseQueries.GetDate(dtaIni);
                            string dtFim = DataBaseQueries.GetDate(dtaFim);

                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda BETWEEN '" + dtInicio + "' AND '" + dtFim + " 23:59:00'");
                            //where.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda >= '" + dtInicio + "' AND " + GatewayRecebimento.SIGLA_QUERY + ".dtaVenda <= '" + dtFim + "'");
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string data = DataBaseQueries.GetDate(dta);
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda >= '" + data + "'");
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            string data = DataBaseQueries.GetDate(dta);
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda <= '" + data + " 23:59:00'");
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            where.Add("DATEPART(YEAR, " + GatewayRecebimento.SIGLA_QUERY + ".dtaVenda) = " + dta.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                            where.Add("DATEPART(YEAR, " + GatewayRecebimento.SIGLA_QUERY + ".dtaVenda) = " + dta.Year + " AND " +
                                      "DATEPART(MONTH, " + GatewayRecebimento.SIGLA_QUERY + ".dtaVenda) = " + dta.Month);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            string data = DataBaseQueries.GetDate(dta);
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda BETWEEN '" + data + "' AND '" + data + " 23:59:00'");
                            //WHERE R.dtaVenda BETWEEN '2016-01-29' AND '2016-01-29 23:59:00'
                        }
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");

                        if (nsu.Contains("%")) // usa LIKE => STARTS WITH
                        {
                            string busca = nsu.Replace("%", "").ToString();
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".nsu like '%" + busca + "'");
                        }
                        else
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".nsu = '" + nsu + "'");
                        break;
                    case CAMPOS.CODRESUMOVENDA:
                        string codResumoVenda = Convert.ToString(item.Value);
                        // Adiciona o join
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");

                        if (codResumoVenda.Contains("%")) // usa LIKE => STARTS WITH
                        {
                            string busca = codResumoVenda.Replace("%", "").ToString();
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".codResumoVenda like '%" + busca + "'");
                        }
                        else
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".codResumoVenda = '" + codResumoVenda + "'");
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        // Adiciona o joins
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                        where.Add(GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente);
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        // Adiciona o joins
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");

                        if (cdBandeira == -1)
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".cdBandeira IS NULL");
                        else if (cdBandeira == 0)
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".cdBandeira IS NOT NULL");
                        else
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".cdBandeira = " + cdBandeira);
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value).TrimEnd();
                        // Adiciona o joins
                        if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                            join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                        if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                        where.Add(GatewayTbBandeira.SIGLA_QUERY + ".dsTipo like '" + dsTipo + "%'");
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        if (cdContaCorrente > 0)
                        {
                            // Obtém as filiais da conta
                            string filiaisDaConta = "'" + string.Join("', '", Permissoes.GetFiliaisDaConta(cdContaCorrente, (painel_taxservices_dbContext) null)) + "'";
                            string adquirentesDaConta = string.Join(", ", Permissoes.GetAdquirentesDaConta(cdContaCorrente, (painel_taxservices_dbContext) null));
                            // Adiciona os joins
                            if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                                join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                            if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                                join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                            where.Add(GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente in (" + adquirentesDaConta + ")");
                            where.Add(GatewayRecebimento.SIGLA_QUERY + ".cnpj in (" + filiaisDaConta + ")");

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

                case CAMPOS.IDRECEBIMENTO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".idRecebimento ASC");
                    else order.Add(SIGLA_QUERY + ".idRecebimento DESC");
                    break;
                case CAMPOS.NUMPARCELA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".numParcela ASC");
                    else order.Add(SIGLA_QUERY + ".numParcela DESC");
                    break;
                case CAMPOS.VALORPARCELABRUTA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".valorParcelaBruta ASC");
                    else order.Add(SIGLA_QUERY + ".valorParcelaBruta DESC");
                    break;
                case CAMPOS.VALORPARCELALIQUIDA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".valorParcelaLiquida ASC");
                    else order.Add(SIGLA_QUERY + ".valorParcelaLiquida DESC");
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    // Adiciona o join
                    if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                        join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                    if (!join.ContainsKey("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY))
                        join.Add("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY, " ON " + GatewayBandeiraPos.SIGLA_QUERY + ".id = " + GatewayRecebimento.SIGLA_QUERY + ".idBandeira");
                    if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                        join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");
                        
                    order.Add(GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia ASC");
                    order.Add(GatewayEmpresa.SIGLA_QUERY + ".filial ASC");
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dtaRecebimento ASC");
                    else order.Add(SIGLA_QUERY + ".dtaRecebimento DESC");
                    order.Add(GatewayBandeiraPos.SIGLA_QUERY + ".desBandeira ASC");
                    order.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda ASC");
                    break;
                case CAMPOS.VALORDESCONTADO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".valorDescontado ASC");
                    else order.Add(SIGLA_QUERY + ".valorDescontado DESC");
                    break;
                case CAMPOS.VLDESCONTADOANTECIPACAO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".vlDescontadoAntecipacao ASC");
                    else order.Add(SIGLA_QUERY + ".vlDescontadoAntecipacao DESC");
                    break;

                // PERSONALIZADO
                case CAMPOS.DTAVENDA:

                    // Adiciona o join
                    if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                        join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                    if (!join.ContainsKey("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY))
                        join.Add("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY, " ON " + GatewayBandeiraPos.SIGLA_QUERY + ".id = " + GatewayRecebimento.SIGLA_QUERY + ".idBandeira");
                    if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                        join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                    order.Add(GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia ASC");
                    order.Add(GatewayEmpresa.SIGLA_QUERY + ".filial ASC");
                    if (orderby == 0) order.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda ASC");
                    else order.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda DESC");
                    order.Add(GatewayBandeiraPos.SIGLA_QUERY + ".desBandeira ASC");
                    order.Add(SIGLA_QUERY + ".dtaRecebimento ASC");
                    break;
                case CAMPOS.DS_FANTASIA:

                    // Adiciona o join
                    if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                        join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                    if (!join.ContainsKey("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY))
                        join.Add("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY, " ON " + GatewayBandeiraPos.SIGLA_QUERY + ".id = " + GatewayRecebimento.SIGLA_QUERY + ".idBandeira");
                    if (!join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                        join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                    order.Add(GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia ASC");
                    order.Add(GatewayEmpresa.SIGLA_QUERY + ".filial ASC");
                    if (orderby == 0) order.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda ASC");
                    else order.Add(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda DESC");
                    order.Add(GatewayBandeiraPos.SIGLA_QUERY + ".desBandeira ASC");
                    order.Add(SIGLA_QUERY + ".dtaRecebimento ASC");

                    if (orderby == 0)
                    {
                        order.Add(GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia ASC");
                        order.Add(GatewayEmpresa.SIGLA_QUERY + ".filial ASC");
                        order.Add(GatewayBandeiraPos.SIGLA_QUERY + ".desBandeira ASC");
                    }
                    else
                    {
                        order.Add(GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia DESC");
                        order.Add(GatewayEmpresa.SIGLA_QUERY + ".filial DESC");
                        order.Add(GatewayBandeiraPos.SIGLA_QUERY + ".desBandeira DESC");
                    }
                    break;
                case CAMPOS.DESBANDEIRA:

                    // Adiciona o join
                    if (!join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                        join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                    if (!join.ContainsKey("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY))
                        join.Add("INNER JOIN pos.BandeiraPos " + GatewayBandeiraPos.SIGLA_QUERY, " ON " + GatewayBandeiraPos.SIGLA_QUERY + ".id = " + GatewayRecebimento.SIGLA_QUERY + ".idBandeira");

                    if (orderby == 0) order.Add(GatewayBandeiraPos.SIGLA_QUERY + ".desBandeira ASC");
                    else order.Add(GatewayBandeiraPos.SIGLA_QUERY + ".desBandeira DESC");
                    break;

            }
            #endregion

            return new SimpleDataBaseQuery(null, "pos.RecebimentoParcela " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());
        }



        // RELAÇÃO COM OS AJUSTES

        private static int getCampoAjustes(int campo)
        {
            CAMPOS filtro = (CAMPOS)campo;
            switch ((CAMPOS)campo)
            {
                case CAMPOS.CDADQUIRENTE: return (int)GatewayTbRecebimentoAjuste.CAMPOS.CDADQUIRENTE;
                case CAMPOS.CDBANDEIRA: return (int)GatewayTbRecebimentoAjuste.CAMPOS.CDBANDEIRA;
                case CAMPOS.DTARECEBIMENTOEFETIVO: return (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE;
                case CAMPOS.DTARECEBIMENTO: return (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE;
                case CAMPOS.IDEXTRATO: return (int)GatewayTbRecebimentoAjuste.CAMPOS.IDEXTRATO;
                case CAMPOS.ID_GRUPO: return (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO;
                case CAMPOS.NU_CNPJ: return (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ;
                case CAMPOS.VALORPARCELALIQUIDA: return (int)GatewayTbRecebimentoAjuste.CAMPOS.VLAJUSTE;
                default: return 0;
            }

        }

        private static Dictionary<string, string> getQueryStringAjustes(Dictionary<string, string> queryStringRecebimentoParcela, bool semAjustesAntecipacao = true)//, bool ajustesVenda = false)
        {
            if (queryStringRecebimentoParcela == null) return null;

            Dictionary<string, string> queryStringAjustes = new Dictionary<string, string>();
            string outValue = null;

            if (semAjustesAntecipacao)
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.SEM_AJUSTES_ANTECIPACAO, true.ToString());

            //if(ajustesVenda)
            //    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.AJUSTES_VENDA, true.ToString());

            if (queryStringRecebimentoParcela.TryGetValue("" + (int)CAMPOS.CDADQUIRENTE, out outValue))
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDADQUIRENTE, queryStringRecebimentoParcela["" + (int)CAMPOS.CDADQUIRENTE]);

            if (queryStringRecebimentoParcela.TryGetValue("" + (int)CAMPOS.CDBANDEIRA, out outValue))
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDBANDEIRA, queryStringRecebimentoParcela["" + (int)CAMPOS.CDBANDEIRA]);

            if (queryStringRecebimentoParcela.TryGetValue("" + (int)CAMPOS.DTARECEBIMENTOEFETIVO, out outValue))
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, queryStringRecebimentoParcela["" + (int)CAMPOS.DTARECEBIMENTOEFETIVO]);
            else if (queryStringRecebimentoParcela.TryGetValue("" + (int)CAMPOS.DTARECEBIMENTO, out outValue))
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, queryStringRecebimentoParcela["" + (int)CAMPOS.DTARECEBIMENTO]);

            if (queryStringRecebimentoParcela.TryGetValue("" + (int)CAMPOS.IDEXTRATO, out outValue))
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.IDEXTRATO, queryStringRecebimentoParcela["" + (int)CAMPOS.IDEXTRATO]);

            if (queryStringRecebimentoParcela.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO, queryStringRecebimentoParcela["" + (int)CAMPOS.ID_GRUPO]);

            if (queryStringRecebimentoParcela.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ, queryStringRecebimentoParcela["" + (int)CAMPOS.NU_CNPJ]);

            if (queryStringRecebimentoParcela.TryGetValue("" + (int)CAMPOS.VALORPARCELALIQUIDA, out outValue))
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.VLAJUSTE, queryStringRecebimentoParcela["" + (int)CAMPOS.VALORPARCELALIQUIDA]);


            return queryStringAjustes;
        }



        /// <summary>
        /// Retorna RecebimentoParcela/RecebimentoParcela
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            DbContextTransaction transaction = _db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
            try
            {
                // Implementar o filtro por Grupo apartir do TOKEN do Usuário
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

                //DECLARAÇÕES
                List<dynamic> CollectionRecebimentoParcela = new List<dynamic>();
                Retorno retorno = new Retorno();
                retorno.Totais = new Dictionary<string, object>();

                // GET QUERY
                var query = getQuery(_db, colecao, campo, orderBy, pageSize, pageNumber, queryString);

                bool exportar = queryString.TryGetValue("" + (int)CAMPOS.EXPORTAR, out outValue);

                if (colecao != 9 && colecao != 8)
                {
                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = query.Count();

                    //if (colecao != 9) // relatório sintético
                    //{
                    retorno.Totais.Add("valorBruto", retorno.TotalDeRegistros > 0 ? /*decimal.Round(*/Convert.ToDecimal(query.GroupBy(r => r.Recebimento).Sum(r => r.Key.valorVendaBruta))/*, 2)*/ : 0);
                    retorno.Totais.Add("valorDescontado", retorno.TotalDeRegistros > 0 ? /*decimal.Round(*/Convert.ToDecimal(query.Sum(r => r.valorDescontado))/*, 2)*/ : 0);
                    retorno.Totais.Add("vlDescontadoAntecipacao", retorno.TotalDeRegistros > 0 ? /*decimal.Round(*/Convert.ToDecimal(query.Sum(r => r.vlDescontadoAntecipacao))/*, 2)*/ : 0);
                    retorno.Totais.Add("valorParcelaBruta", retorno.TotalDeRegistros > 0 ? /*decimal.Round(*/Convert.ToDecimal(query.Sum(r => r.valorParcelaBruta))/*, 2)*/ : 0);
                    retorno.Totais.Add("valorParcelaLiquida", retorno.TotalDeRegistros > 0 ? /*decimal.Round(*/Convert.ToDecimal(query.Sum(r => r.valorParcelaLiquida))/*, 2)*/ : 0);
                    retorno.Totais.Add("taxaCashFlow", retorno.TotalDeRegistros > 0 ? Convert.ToDecimal((query.Select(r => (r.valorDescontado * new decimal(100.0)) / r.valorParcelaBruta).Sum()) / (decimal)retorno.TotalDeRegistros) : 0);
                    //}

                    if (colecao == 0 || colecao == 1 || colecao == 8)
                    {   // coleção que não faz groupby
                        // PAGINAÇÃO
                        int skipRows = (pageNumber - 1) * pageSize;
                        if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                            query = query.Skip(skipRows).Take(pageSize);
                        else
                            pageNumber = 1;
                    }
                }
                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;


                // COLEÇÃO DE RETORNO
                if (colecao == 1)
                {
                    CollectionRecebimentoParcela = query
                    .Select(e => new
                    {
                        idRecebimento = e.idRecebimento,
                        numParcela = e.numParcela,
                        valorParcelaBruta = /*decimal.Round(*/e.valorParcelaBruta/*, 2)*/,
                        valorParcelaLiquida = e.valorParcelaLiquida != null ? /*decimal.Round(*/e.valorParcelaLiquida.Value/*, 2)*/ : new decimal(0.0),
                        dtaRecebimento = e.dtaRecebimento,
                        valorDescontado = /*decimal.Round(*/e.valorDescontado/*, 2)*/,
                        vlDescontadoAntecipacao = /*decimal.Round(*/e.vlDescontadoAntecipacao/*, 2)*/,
                        flAntecipado = e.flAntecipado
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionRecebimentoParcela = query
                    .Select(e => new
                    {
                        id = e.Recebimento.id,
                        idBandeira = e.Recebimento.idBandeira,
                        cnpj = e.Recebimento.cnpj,
                        nsu = e.Recebimento.nsu,
                        cdAutorizador = e.Recebimento.cdAutorizador,
                        dtaVenda = e.Recebimento.dtaVenda,
                        valorVendaBruta = /*decimal.Round(*/e.Recebimento.valorVendaBruta/*, 2)*/,
                        valorVendaLiquida = e.Recebimento.valorVendaLiquida,
                        loteImportacao = e.Recebimento.loteImportacao,
                        //dtaRecebimento = projecao.recebimento.dtaRecebimento,
                        idLogicoTerminal = e.Recebimento.idLogicoTerminal,
                        codTituloERP = e.Recebimento.codTituloERP,
                        codVendaERP = e.Recebimento.codVendaERP,
                        codResumoVenda = e.Recebimento.codResumoVenda,
                        numParcelaTotal = e.Recebimento.numParcelaTotal,
                        nrCartao = e.Recebimento.nrCartao,


                        idRecebimento = e.idRecebimento,
                        numParcela = e.numParcela,
                        valorParcelaBruta = /*decimal.Round(*/e.valorParcelaBruta/*, 2)*/,
                        valorParcelaLiquida = e.valorParcelaLiquida != null ? /*decimal.Round(*/e.valorParcelaLiquida.Value/*, 2)*/ : new decimal(0.0),
                        dtaRecebimento = e.dtaRecebimento,
                        valorDescontado = /*decimal.Round(*/e.valorDescontado/*, 2)*/,
                        vlDescontadoAntecipacao = /*decimal.Round(*/e.vlDescontadoAntecipacao/*, 2)*/,
                        flAntecipado = e.flAntecipado

                    }).ToList<dynamic>();
                }
                else if (colecao == 2) // [mobile]/cashflow
                {
                    var subQuery = query
                        .GroupBy(x => new { x.dtaRecebimento.Year, x.dtaRecebimento.Month, x.Recebimento.empresa.id_grupo })
                        .Select(e => new
                        {
                            cdGrupo = e.Key.id_grupo,
                            nrAno = e.Key.Year,
                            nmMes = ((MES)e.Key.Month).ToString(),
                            nrMes = e.Key.Month,
                            vlParcela = /*decimal.Round(*/e.Sum(p => p.valorParcelaBruta)/*, 2)*/,
                            vlDescontado = /*decimal.Round(*/e.Sum(p => p.valorDescontado)/*, 2)*/,
                            vlLiquido = /*decimal.Round(*/e.Sum(p => p.valorParcelaLiquida != null ? p.valorParcelaLiquida.Value : new decimal(0.0))/*, 2)*/,
                            vlDescontadoAntecipacao = /*decimal.Round(*/e.Sum(p => p.vlDescontadoAntecipacao)/*, 2)*/,
                            nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimentoParcela = subQuery.OrderBy(r => r.nrAno).ThenBy(r => r.nrMes).ToList<dynamic>();

                }
                else if (colecao == 3) // [mobile]/cashflow/tempo
                {
                    var subQuery = query
                        .GroupBy(x => new { x.dtaRecebimento.Day, x.Recebimento.empresa.id_grupo })
                        .Select(e => new
                        {
                            cdGrupo = e.Key.id_grupo,
                            nrDia = e.Key.Day,
                            vlParcela = /*decimal.Round(*/e.Sum(p => p.valorParcelaBruta)/*, 2)*/,
                            vlDescontado = /*decimal.Round(*/e.Sum(p => p.valorDescontado)/*, 2)*/,
                            vlDescontadoAntecipacao = /*decimal.Round(*/e.Sum(p => p.vlDescontadoAntecipacao)/*, 2)*/,
                            vlLiquido = /*decimal.Round(*/e.Sum(p => p.valorParcelaLiquida != null ? p.valorParcelaLiquida.Value : new decimal(0.0))/*, 2)*/,
                            nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimentoParcela = subQuery.OrderBy(r => r.nrDia).ToList<dynamic>();
                }
                else if (colecao == 33) // [mobile]/cashflow/dias
                {
                    var subQuery = query
                        .GroupBy(x => new { x.dtaRecebimento.Day, x.Recebimento.empresa.id_grupo, x.Recebimento.cnpj })
                        .Select(e => new
                        {
                            cdGrupo = e.Key.id_grupo,
                            nuCnpj = e.Key.cnpj,
                            nrDia = e.Key.Day,
                            vlParcela = /*decimal.Round(*/e.Sum(p => p.valorParcelaBruta)/*, 2)*/,
                            vlDescontado = /*decimal.Round(*/e.Sum(p => p.valorDescontado)/*, 2)*/,
                            vlDescontadoAntecipacao = /*decimal.Round(*/e.Sum(p => p.vlDescontadoAntecipacao)/*, 2)*/,
                            vlLiquido = /*decimal.Round(*/e.Sum(p => p.valorParcelaLiquida != null ? p.valorParcelaLiquida.Value : new decimal(0.0))/*, 2)*/,
                            nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimentoParcela = subQuery.OrderBy(r => r.nrDia).ToList<dynamic>();
                }
                else if (colecao == 4) // [mobile]/cashflow/adquirente
                {
                    var subQuery = query
                        .GroupBy(x => new { x.Recebimento.empresa.id_grupo, x.Recebimento.BandeiraPos.Operadora.nmOperadora })
                        .Select(e => new
                        {
                            cdGrupo = e.Key.id_grupo,
                            dsAdquirente = e.Key.nmOperadora,
                            vlParcela = /*decimal.Round(*/e.Sum(p => p.valorParcelaBruta)/*, 2)*/,
                            vlDescontado = /*decimal.Round(*/e.Sum(p => p.valorDescontado)/*, 2)*/,
                            vlDescontadoAntecipacao = /*decimal.Round(*/e.Sum(p => p.vlDescontadoAntecipacao)/*, 2)*/,
                            vlLiquido = /*decimal.Round(*/e.Sum(p => p.valorParcelaLiquida != null ? p.valorParcelaLiquida.Value : new decimal(0.0))/*, 2)*/,
                            nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimentoParcela = subQuery.OrderBy(r => r.cdGrupo).ToList<dynamic>();
                }

                else if (colecao == 5) // [mobile]/cashflow/adquirente/tempo
                {
                    var subQuery = query
                        .GroupBy(x => new { x.dtaRecebimento.Day, x.Recebimento.empresa.id_grupo, x.Recebimento.BandeiraPos.Operadora.nmOperadora })
                        .Select(e => new
                        {
                            cdGrupo = e.Key.id_grupo,
                            nrDia = e.Key.Day,
                            dsAdquirente = e.Key.nmOperadora,
                            vlParcela = /*decimal.Round(*/e.Sum(p => p.valorParcelaBruta)/*, 2)*/,
                            vlDescontado = /*decimal.Round(*/e.Sum(p => p.valorDescontado)/*, 2)*/,
                            vlDescontadoAntecipacao = /*decimal.Round(*/e.Sum(p => p.vlDescontadoAntecipacao)/*, 2)*/,
                            vlLiquido = /*decimal.Round(*/e.Sum(p => p.valorParcelaLiquida != null ? p.valorParcelaLiquida.Value : new decimal(0.0))/*, 2)*/,
                            nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimentoParcela = subQuery.OrderBy(r => r.nrDia).ToList<dynamic>();
                }
                else if (colecao == 6) // [mobile]/cashflow/filial
                {
                    var subQuery = query
                        .GroupBy(x => new { x.Recebimento.empresa.id_grupo, x.Recebimento.cnpj, x.Recebimento.empresa.ds_fantasia, x.Recebimento.empresa.filial })
                        .Select(e => new
                        {
                            cdGrupo = e.Key.id_grupo,
                            nuCnpj = e.Key.cnpj,
                            dsfantasia = e.Key.ds_fantasia,
                            nrFilial = e.Key.filial,
                            vlParcela = /*decimal.Round(*/e.Sum(p => p.valorParcelaBruta)/*, 2)*/,
                            vlDescontado = /*decimal.Round(*/e.Sum(p => p.valorDescontado)/*, 2)*/,
                            vlDescontadoAntecipacao = /*decimal.Round(*/e.Sum(p => p.vlDescontadoAntecipacao)/*, 2)*/,
                            vlLiquido = /*decimal.Round(*/e.Sum(p => p.valorParcelaLiquida != null ? p.valorParcelaLiquida.Value : new decimal(0.0))/*, 2)*/,
                            nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                        });

                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimentoParcela = subQuery.OrderBy(r => r.cdGrupo).ThenBy(r => r.nuCnpj).ToList<dynamic>();
                }
                else if (colecao == 7) // [mobile]/cashflow/filial/tempo
                {
                    var subQuery = query
                        .GroupBy(x => new { x.dtaRecebimento.Day, x.Recebimento.empresa.id_grupo, x.Recebimento.cnpj })
                        .Select(e => new
                        {
                            nrDia = e.Key.Day,
                            cdGrupo = e.Key.id_grupo,
                            nuCnpj = e.Key.cnpj,
                            vlParcela = /*decimal.Round(*/e.Sum(p => p.valorParcelaBruta)/*, 2)*/,
                            vlDescontado = /*decimal.Round(*/e.Sum(p => p.valorDescontado)/*, 2)*/,
                            vlDescontadoAntecipacao = /*decimal.Round(*/e.Sum(p => p.vlDescontadoAntecipacao)/*, 2)*/,
                            vlLiquido = /*decimal.Round(*/e.Sum(p => p.valorParcelaLiquida != null ? p.valorParcelaLiquida.Value : new decimal(0.0))/*, 2)*/,
                            nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                        });


                    // TOTAL DE REGISTROS
                    retorno.TotalDeRegistros = subQuery.Count();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        subQuery = subQuery.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    CollectionRecebimentoParcela = subQuery.OrderBy(r => r.nrDia).ToList<dynamic>();
                }
                else if (colecao == 8 || colecao == 9) // [web]cashflow/Analitico ou [web]cashflow/Sintético
                {
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

                    try
                    {
                        connection.Open();
                    }
                    catch
                    {
                        throw new Exception("Não foi possível estabelecer conexão com a base de dados");
                    }

                    // Obtém componentes da query
                    SimpleDataBaseQuery dataBaseQuery = getQuery(campo, orderBy, queryString);

                    List<IDataRecord> resultado;

                    // Adiciona join com Recebimento, tbBandeira, tbAdquirente e cliente, caso não exista
                    if (!dataBaseQuery.join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                        dataBaseQuery.join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + SIGLA_QUERY + ".idRecebimento");
                    if (!dataBaseQuery.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                        dataBaseQuery.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                    if (!dataBaseQuery.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        dataBaseQuery.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                    if (!dataBaseQuery.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                        dataBaseQuery.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                    // Leitura suja
                    dataBaseQuery.readUncommited = true;

                    if (colecao == 8)
                    {
                        #region ANALÍTICO

                        if (exportar)
                        {
                            // Left Joins com Titulo e Extrato
                            if (!dataBaseQuery.join.ContainsKey("LEFT JOIN card.tbRecebimentoTitulo " + GatewayTbRecebimentoTitulo.SIGLA_QUERY))
                                dataBaseQuery.join.Add("LEFT JOIN card.tbRecebimentoTitulo " + GatewayTbRecebimentoTitulo.SIGLA_QUERY, " ON " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".idRecebimentoTitulo = " + SIGLA_QUERY + ".idRecebimentoTitulo");
                            if (!dataBaseQuery.join.ContainsKey("LEFT JOIN card.tbExtrato " + GatewayTbExtrato.SIGLA_QUERY))
                                dataBaseQuery.join.Add("LEFT JOIN card.tbExtrato " + GatewayTbExtrato.SIGLA_QUERY, " ON " + GatewayTbExtrato.SIGLA_QUERY + ".idExtrato = " + SIGLA_QUERY + ".idExtrato");
                            // Join de Extrato para Conta
                            if (!dataBaseQuery.join.ContainsKey("LEFT JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                                dataBaseQuery.join.Add("LEFT JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + GatewayTbExtrato.SIGLA_QUERY + ".cdContaCorrente");

                            dataBaseQuery.select = new string[] { GatewayRecebimento.SIGLA_QUERY + ".cnpj AS nrCNPJ",
                                                          GatewayRecebimento.SIGLA_QUERY + ".nsu AS nrNSU",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente AS cdAdquirente",
                                                          GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente AS dsAdquirente",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira AS dsBandeira",
                                                          GatewayRecebimento.SIGLA_QUERY + ".dtaVenda AS dtVenda",
                                                          GatewayRecebimento.SIGLA_QUERY + ".valorVendaBruta AS vlVendaBruta",
                                                          GatewayRecebimento.SIGLA_QUERY + ".valorVendaLiquida AS vlVendaLiquida",
                                                          GatewayRecebimento.SIGLA_QUERY + ".numParcelaTotal AS qtTotalParcelas",
                                                          SIGLA_QUERY + ".valorParcelaBruta AS vlParcelaBruta",
                                                          SIGLA_QUERY + ".valorParcelaLiquida AS vlParcelaLiquida",
                                                          SIGLA_QUERY + ".numParcela AS nrParcela",
                                                          GatewayRecebimento.SIGLA_QUERY + ".codTituloERP AS cdTituloERP",
                                                          GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dtBaixaERP AS dtBaixaERP",
                                                          GatewayTbContaCorrente.SIGLA_QUERY + ".cdBanco AS cdBanco",
                                                          GatewayTbContaCorrente.SIGLA_QUERY + ".nrAgencia AS nrAgencia",
                                                          GatewayTbContaCorrente.SIGLA_QUERY + ".nrConta AS nrConta",
                                                          SIGLA_QUERY + ".dtaRecebimento AS dtPrevista",
                                                          SIGLA_QUERY + ".dtaRecebimentoEfetivo AS dtEfetiva"
                                                        };

                            resultado = DataBaseQueries.SqlQuery(dataBaseQuery.Script(), connection);

                            if (resultado != null)
                            {
                                foreach (IDataRecord record in resultado)
                                {
                                    //string nrCNPJ = Convert.ToString(record["nrCNPJ"]);
                                    //string nrNSU = Convert.ToString(record["nrNSU"]);
                                    //Int32 cdAdquirente = Convert.ToInt32(record["cdAdquirente"]);
                                    //string dsAdquirente = Convert.ToString(record["dsAdquirente"]);
                                    //string dsBandeira = Convert.ToString(record["dsBandeira"]);
                                    //DateTime dtVenda = (DateTime)record["dtVenda"];
                                    //decimal vlVendaBruta = Convert.ToDecimal(record["vlVendaBruta"]);
                                    //decimal vlVendaLiquida = Convert.ToDecimal(record["vlVendaLiquida"].Equals(DBNull.Value) ? 0.0 : record["vlVendaBruta"]);
                                    //Int32 qtTotalParcelas = Convert.ToInt32(record["qtTotalParcelas"]);
                                    //DateTime dtPagamento = record["dtEfetiva"].Equals(DBNull.Value) ? (DateTime)record["dtPrevista"] : (DateTime)record["dtEfetiva"];
                                    //decimal vlParcelaBruta = Convert.ToDecimal(record["vlParcelaBruta"]);
                                    //decimal vlParcelaLiquida = Convert.ToDecimal(record["vlParcelaLiquida"].Equals(DBNull.Value) ? 0.0 : record["vlParcelaLiquida"]);
                                    //Int32 nrParcela = Convert.ToInt32(record["nrParcela"]);
                                    //string cdTituloERP = record["cdTituloERP"].Equals(DBNull.Value) ? "" : Convert.ToString(record["cdTituloERP"]);
                                    //DateTime? dtBaixaERP = record["dtBaixaERP"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)record["dtBaixaERP"];
                                    //string cdBanco = record["cdBanco"].Equals(DBNull.Value) ? "" : Convert.ToString(record["cdBanco"]);
                                    //string nrAgencia = record["nrAgencia"].Equals(DBNull.Value) ? "" : Convert.ToString(record["nrAgencia"]);
                                    //string nrConta = record["nrConta"].Equals(DBNull.Value) ? "" : Convert.ToString(record["nrConta"]);
                                    //DateTime dtPrevista = (DateTime)record["dtPrevista"];
                                    //DateTime? dtEfetiva = record["dtEfetiva"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)record["dtEfetiva"];
                                    CollectionRecebimentoParcela.Add(new
                                    {
                                        nrCNPJ = Convert.ToString(record["nrCNPJ"]),
                                        nrNSU = Convert.ToString(record["nrNSU"]),
                                        cdAdquirente = Convert.ToInt32(record["cdAdquirente"]),
                                        dsAdquirente = Convert.ToString(record["dsAdquirente"]),
                                        dsBandeira = Convert.ToString(record["dsBandeira"]),
                                        dtVenda = (DateTime)record["dtVenda"],
                                        vlVendaBruta = Convert.ToDecimal(record["vlVendaBruta"]),
                                        vlVendaLiquida = Convert.ToDecimal(record["vlVendaLiquida"].Equals(DBNull.Value) ? 0.0 : record["vlVendaBruta"]),
                                        qtTotalParcelas = Convert.ToInt32(record["qtTotalParcelas"].Equals(DBNull.Value) ? 0 : record["qtTotalParcelas"]),
                                        dtPagamento = record["dtEfetiva"].Equals(DBNull.Value) ? (DateTime)record["dtPrevista"] : (DateTime)record["dtEfetiva"],
                                        vlParcelaBruta = Convert.ToDecimal(record["vlParcelaBruta"]),
                                        vlParcelaLiquida = Convert.ToDecimal(record["vlParcelaLiquida"].Equals(DBNull.Value) ? 0.0 : record["vlParcelaLiquida"]),
                                        nrParcela = Convert.ToInt32(record["nrParcela"]),
                                        cdTituloERP = record["cdTituloERP"].Equals(DBNull.Value) ? "" : Convert.ToString(record["cdTituloERP"]),
                                        dtBaixaERP = record["dtBaixaERP"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)record["dtBaixaERP"],
                                        cdBanco = record["cdBanco"].Equals(DBNull.Value) ? "" : Convert.ToString(record["cdBanco"]),
                                        nrAgencia = record["nrAgencia"].Equals(DBNull.Value) ? "" : Convert.ToString(record["nrAgencia"]),
                                        nrConta = record["nrConta"].Equals(DBNull.Value) ? "" : Convert.ToString(record["nrConta"]),
                                        dtPrevista = (DateTime)record["dtPrevista"],
                                        dtEfetiva = record["dtEfetiva"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)record["dtEfetiva"],
                                    });
                                }

                            }

                        }
                        else
                        {
                            // Salva o order by e o retira para poder fazer agrupamentos
                            string[] currentOrderBy = dataBaseQuery.orderby;
                            dataBaseQuery.orderby = null;

                            #region OBTÉM TOTAIS DAS PARCELAS
                            dataBaseQuery.select = new string[] { "SUM(" + SIGLA_QUERY + ".valorDescontado) as valorDescontado",
                                                          "SUM(" + SIGLA_QUERY + ".vlDescontadoAntecipacao) as vlDescontadoAntecipacao",
                                                          "SUM(" + SIGLA_QUERY + ".valorParcelaBruta) as valorParcelaBruta",
                                                          "SUM(" + SIGLA_QUERY + ".valorParcelaLiquida) as valorParcelaLiquida",
                                                          "SUM((" + SIGLA_QUERY + ".valorDescontado * 100.0) / CASE WHEN "+ SIGLA_QUERY + ".valorParcelaBruta != 0 THEN "+ SIGLA_QUERY + ".valorParcelaBruta ELSE 1 END) as taxaCashFlow",
                                                          "COUNT(*) as totalRegistros" };

                            //retorno.Totais.Add("valorBruto", retorno.TotalDeRegistros > 0 ? Convert.ToDecimal(query.GroupBy(r => r.Recebimento).Sum(r => r.Key.valorVendaBruta))/*, 2)*/ : 0); 

                            resultado = DataBaseQueries.SqlQuery(dataBaseQuery.Script(), connection);

                            retorno.TotalDeRegistros = 0;
                            retorno.Totais.Add("valorDescontado", new decimal(0.0));
                            retorno.Totais.Add("vlDescontadoAntecipacao", new decimal(0.0));
                            retorno.Totais.Add("valorParcelaBruta", new decimal(0.0));
                            retorno.Totais.Add("valorParcelaLiquida", new decimal(0.0));
                            retorno.Totais.Add("taxaCashFlow", new decimal(0.0));

                            if (resultado != null && resultado.Count > 0)
                            {
                                retorno.TotalDeRegistros = Convert.ToInt32(resultado[0]["totalRegistros"]);
                                retorno.Totais["valorDescontado"] = Convert.ToDecimal(retorno.TotalDeRegistros > 0 ? resultado[0]["valorDescontado"] : 0.0);
                                retorno.Totais["vlDescontadoAntecipacao"] = Convert.ToDecimal(retorno.TotalDeRegistros > 0 ? resultado[0]["vlDescontadoAntecipacao"] : 0.0);
                                retorno.Totais["valorParcelaBruta"] = Convert.ToDecimal(retorno.TotalDeRegistros > 0 ? resultado[0]["valorParcelaBruta"] : 0.0);
                                retorno.Totais["valorParcelaLiquida"] = Convert.ToDecimal(retorno.TotalDeRegistros > 0 ? resultado[0]["valorParcelaLiquida"] : 0.0);
                                retorno.Totais["taxaCashFlow"] = retorno.TotalDeRegistros > 0 ? Convert.ToDecimal(resultado[0]["taxaCashFlow"]) / ((decimal)retorno.TotalDeRegistros) : new decimal(0.0);
                            }
                            #endregion

                            #region OBTÉM VALOR BRUTO DE VENDA
                            dataBaseQuery.select = new string[] { "SUM(" + GatewayRecebimento.SIGLA_QUERY + ".valorVendaBruta) as valorBruto" };
                            dataBaseQuery.groupby = new string[] { GatewayRecebimento.SIGLA_QUERY + ".id" };
                            
                            resultado = DataBaseQueries.SqlQuery("SELECT SUM(T.valorBruto) AS valorBruto FROM (" + dataBaseQuery.Script() + ") as T", connection);

                            if (resultado != null && resultado.Count > 0)
                            {
                                retorno.Totais.Add("valorBruto", Convert.ToDecimal(resultado[0]["valorBruto"].Equals(DBNull.Value) ? 0.0 : resultado[0]["valorBruto"]));
                            }
                            #endregion

                            // Retoma ordenação sem agrupamento
                            dataBaseQuery.groupby = null;
                            dataBaseQuery.orderby = currentOrderBy;

                            dataBaseQuery.select = new string[] { GatewayRecebimento.SIGLA_QUERY + ".id AS idRecebimento",
                                                          SIGLA_QUERY + ".numParcela AS numParcela",
                                                          GatewayRecebimento.SIGLA_QUERY + ".cnpj AS cnpj",
                                                          GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia AS dsFantasia",
                                                          GatewayEmpresa.SIGLA_QUERY + ".filial AS filial",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira AS dsBandeira",
                                                          GatewayRecebimento.SIGLA_QUERY + ".dtaVenda AS dtaVenda",
                                                          SIGLA_QUERY + ".dtaRecebimento AS dtaRecebimento",
                                                          SIGLA_QUERY + ".dtaRecebimentoEfetivo AS dtaRecebimentoEfetivo",
                                                          GatewayRecebimento.SIGLA_QUERY + ".codResumoVenda AS codResumoVenda",
                                                          GatewayRecebimento.SIGLA_QUERY + ".nsu AS nsu",
                                                          GatewayRecebimento.SIGLA_QUERY + ".cdAutorizador AS cdAutorizador",
                                                          GatewayRecebimento.SIGLA_QUERY + ".numParcelaTotal AS numParcelaTotal",
                                                          GatewayRecebimento.SIGLA_QUERY + ".valorVendaBruta AS valorBruto",
                                                          SIGLA_QUERY + ".valorParcelaBruta AS valorParcela",
                                                          SIGLA_QUERY + ".valorParcelaLiquida AS valorLiquida",
                                                          SIGLA_QUERY + ".valorDescontado AS valorDescontado",
                                                          SIGLA_QUERY + ".vlDescontadoAntecipacao AS vlDescontadoAntecipacao",
                                                          GatewayRecebimento.SIGLA_QUERY + ".nrCartao AS nrCartao",
                                                          SIGLA_QUERY + ".flAntecipado AS flAntecipado",
                                                          GatewayRecebimento.SIGLA_QUERY + ".idResumoVenda AS lote"                                                          
                                                        };

                            resultado = DataBaseQueries.SqlQuery(dataBaseQuery.Script(), connection);

                            if (resultado != null)
                            {
                                foreach (IDataRecord record in resultado)
                                {
                                    CollectionRecebimentoParcela.Add(new
                                    {
                                        parcela = new
                                        {
                                            idRecebimento = Convert.ToInt32(record["idRecebimento"]),
                                            numParcela = Convert.ToInt32(record["numParcela"]),
                                        },
                                        cnpj = Convert.ToString(record["cnpj"]),
                                        dsFantasia = Convert.ToString(record["dsFantasia"]) + (record["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(record["filial"])),
                                        dsBandeira = Convert.ToString(record["dsBandeira"]),
                                        dtaVenda = (DateTime)record["dtaVenda"],
                                        dtaRecebimento = (DateTime)record["dtaRecebimento"],
                                        dtaRecebimentoEfetivo = record["dtaRecebimentoEfetivo"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)record["dtaRecebimentoEfetivo"],
                                        codResumoVenda = Convert.ToString(record["codResumoVenda"]),
                                        nsu = Convert.ToString(record["nsu"]),
                                        cdAutorizador = Convert.ToString(record["cdAutorizador"].Equals(DBNull.Value) ? "" : record["cdAutorizador"]),
                                        numParcela = Convert.ToInt32(record["numParcela"]) + " de " + Convert.ToInt32(record["numParcelaTotal"].Equals(DBNull.Value) ? 0 : record["numParcelaTotal"]),
                                        valorBruto = Convert.ToDecimal(record["valorBruto"]),
                                        valorParcela = Convert.ToDecimal(record["valorParcela"]),
                                        valorLiquida = Convert.ToDecimal(record["valorLiquida"].Equals(DBNull.Value) ? 0.0 : record["valorLiquida"]),
                                        valorDescontado = Convert.ToDecimal(record["valorDescontado"]),
                                        vlDescontadoAntecipacao = Convert.ToDecimal(record["vlDescontadoAntecipacao"]),
                                        nrCartao = record["nrCartao"].Equals(DBNull.Value) ? "" : Convert.ToString(record["nrCartao"]),
                                        flAntecipado = Convert.ToBoolean(record["flAntecipado"]),
                                        lote = record["lote"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(record["lote"]),
                                    });
                                }
                            }

                            // Obtém os ajustes se teve filtro de data de recebimento
                            if (!queryString.TryGetValue("" + (int)CAMPOS.NSU, out outValue) &&
                                !queryString.TryGetValue("" + (int)CAMPOS.CODRESUMOVENDA, out outValue) &&
                                (queryString.TryGetValue("" + (int)CAMPOS.DTARECEBIMENTO, out outValue) ||
                                 queryString.TryGetValue("" + (int)CAMPOS.DTARECEBIMENTOEFETIVO, out outValue)))
                            {
                                List<dynamic> ajustes = GatewayTbRecebimentoAjuste.getQuery(_db, 1, getCampoAjustes(campo), orderBy, pageSize, pageNumber, getQueryStringAjustes(queryString))
                                                    .Select(e => new
                                                    {
                                                        ajuste = new { idRecebimentoAjuste = e.idRecebimentoAjuste },
                                                        cnpj = e.nrCNPJ,
                                                        dsFantasia = e.empresa.ds_fantasia + (e.empresa.filial != null ? " " + e.empresa.filial : ""),
                                                        dsBandeira = e.tbBandeira.dsBandeira,
                                                        dtaVenda = e.dtAjuste,
                                                        dtaRecebimento = e.dtAjuste,
                                                        dtaRecebimentoEfetivo = e.dtAjuste,
                                                        codResumoVenda = String.Empty,
                                                        nsu = e.dsMotivo,
                                                        numParcela = 0,
                                                        valorBruto = new decimal(0.0),
                                                        valorParcela = e.vlAjuste > new decimal(0.0) ? e.vlAjuste : new decimal(0.0),
                                                        valorLiquida = e.vlAjuste,
                                                        valorDescontado = e.vlAjuste < new decimal(0.0) ? new decimal(-1.0) * e.vlAjuste : new decimal(0.0),
                                                        vlDescontadoAntecipacao = new decimal(0.0),
                                                    }).ToList<dynamic>();

                                if (ajustes.Count > 0)
                                {
                                    retorno.TotalDeRegistros += ajustes.Count;

                                    // Atualiza total líquido de parcela
                                    retorno.Totais["valorParcelaBruta"] = (decimal)retorno.Totais["valorParcelaBruta"] + (ajustes.Count > 0 ? Convert.ToDecimal(ajustes.Select(r => r.valorParcela).Cast<decimal>().Sum()) : new decimal(0.0));
                                    retorno.Totais["valorParcelaLiquida"] = (decimal)retorno.Totais["valorParcelaLiquida"] + (ajustes.Count > 0 ? Convert.ToDecimal(ajustes.Select(r => r.valorLiquida).Cast<decimal>().Sum()) : new decimal(0.0));
                                    retorno.Totais["valorDescontado"] = (decimal)retorno.Totais["valorDescontado"] + (ajustes.Count > 0 ? Convert.ToDecimal(ajustes.Select(r => r.valorDescontado).Cast<decimal>().Sum()) : new decimal(0.0));


                                    // Armazena os ajustes
                                    foreach (var ajuste in ajustes) CollectionRecebimentoParcela.Add(ajuste);

                                    // Ordena e refaz a paginação
                                    CAMPOS filtro = (CAMPOS)campo;
                                    if (filtro.Equals(CAMPOS.DTARECEBIMENTOEFETIVO))
                                    {
                                        if (orderBy == 0)
                                            CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderBy(e => e.dtaRecebimentoEfetivo).ToList<dynamic>();
                                        else
                                            CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderByDescending(e => e.dtaRecebimentoEfetivo).ToList<dynamic>();
                                    }
                                    else if (filtro.Equals(CAMPOS.DTARECEBIMENTO))
                                    {
                                        if (orderBy == 0)
                                            CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderBy(e => e.dtaRecebimento).ToList<dynamic>();
                                        else
                                            CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderByDescending(e => e.dtaRecebimento).ToList<dynamic>();
                                    }
                                    else
                                    {
                                        if (orderBy == 0)
                                            CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderBy(e => e.dtaVenda).ToList<dynamic>();
                                        else
                                            CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderByDescending(e => e.dtaVenda).ToList<dynamic>();
                                    }
                                }

                            }

                            // Paginação
                            int skipRows = (pageNumber - 1) * pageSize;
                            if (CollectionRecebimentoParcela.Count > pageSize && pageNumber > 0 && pageSize > 0)
                                CollectionRecebimentoParcela = CollectionRecebimentoParcela.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                            
                        }
                        #endregion
                    }
                    else
                    {
                        #region SINTÉTICO
                        List<dynamic> ajustes = new List<dynamic>();
                        // Join com cliente
                        if (!dataBaseQuery.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            dataBaseQuery.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                        // ORDER
                        dataBaseQuery.orderby = new string[] {  GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                                GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                                GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira"
                                                             };

                        if (exportar)
                        {
                            dataBaseQuery.select = new string[] { 
                                                          GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                          GatewayEmpresa.SIGLA_QUERY + ".filial ",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                          //"SUM(" + // valorBruto da venda!,
                                                          "SUM(" + SIGLA_QUERY + ".valorParcelaBruta) as valorParcela",
                                                          "SUM(" + SIGLA_QUERY + ".valorParcelaLiquida) as valorLiquida",
                                                          "SUM(" + SIGLA_QUERY + ".valorDescontado) as valorDescontado",
                                                          "SUM(" + SIGLA_QUERY + ".vlDescontadoAntecipacao) as vlDescontadoAntecipacao",
                                                          "COUNT(*) as totalTransacoes"                                                       
                                                        };

                            dataBaseQuery.groupby = new string[] {  GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj",
                                                                    GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                                    GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                                    GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira",
                                                                    GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira"
                                                                 };

                            resultado = DataBaseQueries.SqlQuery(dataBaseQuery.Script(), connection);

                            if (resultado != null)
                            {
                                foreach (IDataRecord record in resultado)
                                {
                                    CollectionRecebimentoParcela.Add(new
                                    {
                                        empresa = Convert.ToString(record["ds_fantasia"]) + (record["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(record["filial"])),
                                        bandeira = Convert.ToString(record["dsBandeira"]),
                                        valorBruto = Convert.ToDecimal(record["valorParcela"]), // POR ENQUANTO
                                        valorParcela = Convert.ToDecimal(record["valorParcela"]),
                                        valorLiquida = Convert.ToDecimal(record["valorLiquida"]),
                                        valorDescontado = Convert.ToDecimal(record["valorDescontado"]),
                                        vlDescontadoAntecipacao =Convert.ToDecimal(record["vlDescontadoAntecipacao"]),
                                        //vlLiquido = Convert.ToDecimal(record["vlLiquido"]),
                                        totalTransacoes = Convert.ToInt32(record["totalTransacoes"])
                                    });
                                }
                            }

                            // Obtém os ajustes
                            // Obtém os ajustes se teve filtro de data de recebimento
                            if (!queryString.TryGetValue("" + (int)CAMPOS.NSU, out outValue) &&
                                !queryString.TryGetValue("" + (int)CAMPOS.CODRESUMOVENDA, out outValue) &&
                                (queryString.TryGetValue("" + (int)CAMPOS.DTARECEBIMENTO, out outValue) ||
                                 queryString.TryGetValue("" + (int)CAMPOS.DTARECEBIMENTOEFETIVO, out outValue)))
                            {
                                ajustes = GatewayTbRecebimentoAjuste.getQuery(_db, 1, getCampoAjustes(campo), orderBy, pageSize, pageNumber, getQueryStringAjustes(queryString))
                                                        .GroupBy(x => new { x.empresa, x.tbBandeira })
                                                        .OrderBy(e => e.Key.empresa.ds_fantasia)
                                                        .ThenBy(e => e.Key.empresa.filial)
                                                        .ThenBy(e => e.Key.tbBandeira.dsBandeira)
                                                        .Select(e => new
                                                        {
                                                            empresa = e.Key.empresa.ds_fantasia + (e.Key.empresa.filial ?? ""),
                                                            bandeira = e.Key.tbBandeira.dsBandeira,
                                                            valorBruto = new decimal(0.0),
                                                            valorParcela = e.Sum(p => p.vlAjuste) > new decimal(0.0) ? e.Sum(p => p.vlAjuste) : new decimal(0.0),
                                                            valorLiquida = e.Sum(p => p.vlAjuste),
                                                            valorDescontado = e.Sum(p => p.vlAjuste) < new decimal(0.0) ? new decimal(-1.0) * e.Sum(p => p.vlAjuste) : new decimal(0.0),
                                                            vlDescontadoAntecipacao = new decimal(0.0),
                                                            totalTransacoes = e.Count()
                                                        }).ToList<dynamic>();
                            }
                        }
                        else
                        {

                            dataBaseQuery.select = new string[] { GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj",
                                                          GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                          GatewayEmpresa.SIGLA_QUERY + ".filial ",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente",
                                                          //"SUM(" + // valorBruto da venda!,
                                                          "SUM(" + SIGLA_QUERY + ".valorParcelaBruta) as valorParcela",
                                                          "SUM(" + SIGLA_QUERY + ".valorParcelaLiquida) as valorLiquida",
                                                          "SUM(" + SIGLA_QUERY + ".valorDescontado) as valorDescontado",
                                                          "SUM(" + SIGLA_QUERY + ".vlDescontadoAntecipacao) as vlDescontadoAntecipacao",
                                                          "COUNT(*) as totalTransacoes"                                                        
                                                        };

                            dataBaseQuery.groupby = new string[] {  GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj",
                                                                    GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                                    GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                                    GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira",
                                                                    GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                                    GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente"
                                                                 };

                            resultado = DataBaseQueries.SqlQuery(dataBaseQuery.Script(), connection);

                            if (resultado != null)
                            {
                                foreach (IDataRecord record in resultado)
                                {
                                    CollectionRecebimentoParcela.Add(new
                                    {
                                        empresa = new
                                        {
                                            nu_cnpj = Convert.ToString(record["nu_cnpj"]),
                                            ds_fantasia = Convert.ToString(record["ds_fantasia"]),
                                            filial = record["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(record["filial"])
                                        },
                                        bandeira = new
                                        {
                                            dsBandeira = Convert.ToString(record["dsBandeira"]),
                                            cdBandeira = Convert.ToInt32(record["cdBandeira"]),
                                            cdAdquirente = Convert.ToInt32(record["cdAdquirente"])
                                        },                                        
                                        valorBruto = Convert.ToDecimal(record["valorParcela"]), // POR ENQUANTO
                                        valorParcela = Convert.ToDecimal(record["valorParcela"]),
                                        valorDescontado = Convert.ToDecimal(record["valorDescontado"]),
                                        vlDescontadoAntecipacao =Convert.ToDecimal(record["vlDescontadoAntecipacao"]),
                                        valorLiquida = Convert.ToDecimal(record["valorLiquida"]),
                                        totalTransacoes = Convert.ToInt32(record["totalTransacoes"])
                                    });
                                }
                            }

                            // Obtém os ajustes se teve filtro de data de recebimento
                            if (!queryString.TryGetValue("" + (int)CAMPOS.NSU, out outValue) &&
                                !queryString.TryGetValue("" + (int)CAMPOS.CODRESUMOVENDA, out outValue) &&
                                (queryString.TryGetValue("" + (int)CAMPOS.DTARECEBIMENTO, out outValue) ||
                                 queryString.TryGetValue("" + (int)CAMPOS.DTARECEBIMENTOEFETIVO, out outValue)))
                            {
                                ajustes = GatewayTbRecebimentoAjuste.getQuery(_db, 1, getCampoAjustes(campo), orderBy, pageSize, pageNumber, getQueryStringAjustes(queryString))
                                                        .GroupBy(x => new { x.empresa, x.tbBandeira })
                                                        .OrderBy(e => e.Key.empresa.ds_fantasia)
                                                        .ThenBy(e => e.Key.empresa.filial)
                                                        .ThenBy(e => e.Key.tbBandeira.dsBandeira)
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
                                                                dsBandeira = e.Key.tbBandeira.dsBandeira,
                                                                cdBandeira = e.Key.tbBandeira.cdBandeira,
                                                                cdAdquirente = e.Key.tbBandeira.cdAdquirente
                                                            },
                                                            valorBruto = new decimal(0.0),
                                                            valorParcela = e.Sum(p => p.vlAjuste) > new decimal(0.0) ? e.Sum(p => p.vlAjuste) : new decimal(0.0),
                                                            valorLiquida = e.Sum(p => p.vlAjuste),
                                                            valorDescontado = e.Sum(p => p.vlAjuste) < new decimal(0.0) ? new decimal(-1.0) * e.Sum(p => p.vlAjuste) : new decimal(0.0),
                                                            vlDescontadoAntecipacao = new decimal(0.0),
                                                            totalTransacoes = e.Count()
                                                        }).ToList<dynamic>();
                            }
                        }

                        foreach (var ajuste in ajustes)
                        {
                            // Busca a relação empresa-bandeira nos recebimentos
                            var rp = exportar ? CollectionRecebimentoParcela.Where(e => e.empresa.Equals(ajuste.empresa))
                                                                            .Where(e => e.bandeira.Equals(ajuste.bandeira))
                                                                            .FirstOrDefault() :
                                                CollectionRecebimentoParcela.Where(e => e.empresa.nu_cnpj.Equals(ajuste.empresa.nu_cnpj))
                                                                            .Where(e => e.bandeira.cdBandeira == ajuste.bandeira.cdBandeira)
                                                                            .FirstOrDefault();
                            if (rp == null)
                                // Adiciona
                                CollectionRecebimentoParcela.Add(ajuste);
                            else
                            {
                                // Atualiza
                                var newRp = new
                                {
                                    empresa = rp.empresa,
                                    bandeira = rp.bandeira,
                                    valorBruto = rp.valorBruto,
                                    valorParcela = rp.valorParcela + ajuste.valorParcela,
                                    valorLiquida = rp.valorLiquida + ajuste.valorLiquida,
                                    valorDescontado = rp.valorDescontado + ajuste.valorDescontado,
                                    vlDescontadoAntecipacao = rp.vlDescontadoAntecipacao,
                                    totalTransacoes = rp.totalTransacoes + ajuste.totalTransacoes
                                };
                                CollectionRecebimentoParcela.Remove(rp);
                                CollectionRecebimentoParcela.Add(newRp);
                            }

                        }

                        // Ordena e refaz a paginação
                        CAMPOS filtro = (CAMPOS)campo;
                        if (filtro.Equals(CAMPOS.DTARECEBIMENTOEFETIVO))
                        {
                            if (orderBy == 0)
                                CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderBy(e => e.dtaRecebimentoEfetivo).ToList<dynamic>();
                            else
                                CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderByDescending(e => e.dtaRecebimentoEfetivo).ToList<dynamic>();
                        }
                        else if (filtro.Equals(CAMPOS.DTARECEBIMENTO))
                        {
                            if (orderBy == 0)
                                CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderBy(e => e.dtaRecebimento).ToList<dynamic>();
                            else
                                CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderByDescending(e => e.dtaRecebimento).ToList<dynamic>();
                        }
                        else if (filtro.Equals(CAMPOS.DTAVENDA))
                        {
                            if (orderBy == 0)
                                CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderBy(e => e.dtaVenda).ToList<dynamic>();
                            else
                                CollectionRecebimentoParcela = CollectionRecebimentoParcela.OrderByDescending(e => e.dtaVenda).ToList<dynamic>();
                        }


                        retorno.TotalDeRegistros = CollectionRecebimentoParcela.Count;

                        retorno.Totais.Add("valorBruto", CollectionRecebimentoParcela.Count > 0 ? Convert.ToDecimal(CollectionRecebimentoParcela.Select(r => r.valorBruto).Cast<decimal>().Sum()) : 0);
                        retorno.Totais.Add("valorDescontado", CollectionRecebimentoParcela.Count > 0 ? Convert.ToDecimal(CollectionRecebimentoParcela.Select(r => r.valorDescontado).Cast<decimal>().Sum()) : 0);
                        retorno.Totais.Add("vlDescontadoAntecipacao", CollectionRecebimentoParcela.Count > 0 ? Convert.ToDecimal(CollectionRecebimentoParcela.Select(r => r.vlDescontadoAntecipacao).Cast<decimal>().Sum()) : 0);
                        retorno.Totais.Add("valorLiquida", CollectionRecebimentoParcela.Count > 0 ? Convert.ToDecimal(CollectionRecebimentoParcela.Select(r => r.valorLiquida).Cast<decimal>().Sum()) : 0);
                        retorno.Totais.Add("valorParcela", CollectionRecebimentoParcela.Count > 0 ? Convert.ToDecimal(CollectionRecebimentoParcela.Select(r => r.valorParcela).Cast<decimal>().Sum()) : 0);
                        retorno.Totais.Add("totalTransacoes", CollectionRecebimentoParcela.Count > 0 ? Convert.ToDecimal(CollectionRecebimentoParcela.Select(r => r.totalTransacoes).Cast<int>().Sum()) : 0);

                        // PAGINAÇÃO
                        int skipRows = (pageNumber - 1) * pageSize;
                        if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                            CollectionRecebimentoParcela = CollectionRecebimentoParcela.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                        else
                            pageNumber = 1;
                        #endregion
                    }


                    try
                    {
                        connection.Close();
                    }
                    catch { }
                }
                



                retorno.Registros = CollectionRecebimentoParcela;


                transaction.Commit();

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar recebimento parcela" : erro);
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
        /// Adiciona nova RecebimentoParcela
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, RecebimentoParcela param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.RecebimentoParcelas.Add(param);
                _db.SaveChanges();
                // Commit
                transaction.Commit();
                return param.idRecebimento;
            }
            catch (Exception e)
            {
                // Rollback
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar recebimento parcela" : erro);
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
        /// Apaga uma RecebimentoParcela
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRecebimento, Int32 numParcela, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                RecebimentoParcela recebimentoparcela = _db.RecebimentoParcelas.Where(e => e.idRecebimento == idRecebimento)
                                                                               .Where(e => e.numParcela == numParcela)
                                                                               .FirstOrDefault();

                if (recebimentoparcela == null) throw new Exception("Recebimento Parcela inexistente");

                // Se só tiver uma parcela, significa que a venda ficará sem parcelas => Remove a venda também
                bool removeVenda = recebimentoparcela.Recebimento.RecebimentoParcelas.Count == 1;

                _db.RecebimentoParcelas.Remove(recebimentoparcela);
                _db.SaveChanges();

                // Commit
                transaction.Commit();

                // Remove a venda toda
                if (removeVenda) GatewayRecebimento.Delete(token, idRecebimento, _db);
            }
            catch (Exception e)
            {
                // Rollback
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar recebimento parcela" : erro);
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
        /// Altera data de Recebimento Efetivo do RecebimentoParcela
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, RecebimentoParcela param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {

                RecebimentoParcela value = _db.RecebimentoParcelas
                    .Where(e => e.idRecebimento == param.idRecebimento)
                    .First<RecebimentoParcela>();

                if (param.valorParcelaBruta != new decimal(0.0) && param.valorParcelaBruta != value.valorParcelaBruta)
                    value.valorParcelaBruta = param.valorParcelaBruta;
                if (param.dtaRecebimento != null && param.dtaRecebimento != value.dtaRecebimento)
                    value.dtaRecebimento = param.dtaRecebimento;
                if (param.valorDescontado != new decimal(0.0) && param.valorDescontado != value.valorDescontado)
                    value.valorDescontado = param.valorDescontado;
                //if (param.idExtrato != null && param.idExtrato != value.idExtrato)
                //    value.idExtrato = param.idExtrato;
                if (param.dtaRecebimentoEfetivo != null && param.dtaRecebimentoEfetivo != value.dtaRecebimentoEfetivo)
                    value.dtaRecebimentoEfetivo = param.dtaRecebimentoEfetivo;
                if (param.vlDescontadoAntecipacao != new decimal(0.0) && param.vlDescontadoAntecipacao != value.vlDescontadoAntecipacao)
                    value.vlDescontadoAntecipacao = param.vlDescontadoAntecipacao;
                //if (param.idRecebimentoTitulo != null && param.idRecebimentoTitulo != value.idRecebimentoTitulo)
                //    value.idRecebimentoTitulo = param.idRecebimentoTitulo;
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
                    throw new Exception(erro.Equals("") ? "Falha ao alterar recebimento parcela" : erro);
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

    }

}
