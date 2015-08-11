using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Globalization;

namespace api.Negocios.Card
{
    public class GatewayTbExtrato
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbExtrato()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDEXTRATO = 100,
            CDCONTACORRENTE = 101,
            NRDOCUMENTO = 102,
            DTEXTRATO = 103,
            DSDOCUMENTO = 104,
            VLMOVIMENTO = 105,

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
        private static IQueryable<tbExtrato> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
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
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato >= dta);
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
                            entity = entity.Where(e => e.dtExtrato <= dta);
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month);
                        }
                        else if (item.Value.Length == 7)
                        {
                            string dia = item.Value.Substring(6, 1);
                            string anoMes = item.Value.Substring(0, 6);
                            string busca = anoMes + "0" + dia;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month && e.dtExtrato.Day == dtaIni.Day);
                        }
                        else // IGUAL
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
                        entity = entity.Where(e => e.vlMovimento.Equals(vlMovimento)).AsQueryable<tbExtrato>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtExtrato).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.dtExtrato).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.DSDOCUMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsDocumento).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.dsDocumento).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.VLMOVIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlMovimento).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.vlMovimento).AsQueryable<tbExtrato>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbExtrato/TbExtrato
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbExtrato = new List<dynamic>();
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
                CollectionTbExtrato = query.Select(e => new
                {

                    idExtrato = e.idExtrato,
                    cdContaCorrente = e.cdContaCorrente,
                    nrDocumento = e.nrDocumento,
                    dtExtrato = e.dtExtrato,
                    dsDocumento = e.dsDocumento,
                    vlMovimento = e.vlMovimento,
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
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbExtrato;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbExtrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbExtrato param)
        {
            // Valida param para não adicionar duplicidades
            tbExtrato extrato = _db.tbExtratos.Where(e => e.cdContaCorrente == param.cdContaCorrente)
                                              .Where(e => e.dtExtrato.Equals(param.dtExtrato))
                                              .Where(e => e.nrDocumento.Equals(param.nrDocumento))
                                              .Where(e => e.vlMovimento == param.vlMovimento)
                                              .Where(e => e.dsDocumento.Equals(param.dsDocumento)) // DESCRIÇÃO PODE MUDAR ?
                                              .FirstOrDefault();

            if (extrato != null) throw new Exception("Extrato já existe");

            _db.tbExtratos.Add(param);
            _db.SaveChanges();
            return param.idExtrato;
        }


        /// <summary>
        /// Apaga uma TbExtrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idExtrato)
        {
            tbExtrato extrato = _db.tbExtratos.Where(e => e.idExtrato == idExtrato).FirstOrDefault();
            if (extrato == null) throw new Exception("Extrato inexistente");
            _db.tbExtratos.Remove(extrato);
            _db.SaveChanges();
        }
        /// <summary>
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

        }

    }
}
