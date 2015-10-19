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
    public class GatewayTbRecebimentoAjuste
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRecebimentoAjuste()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDRECEBIMENTOAJUSTE = 100,
            DTAJUSTE = 101,
            NRCNPJ = 102,
            CDBANDEIRA = 103,
            DSMOTIVO = 104,
            VLAJUSTE = 105,
            IDEXTRATO = 106,

            // RELACIONAMENTOS
            ID_GRUPO = 216,

            CDADQUIRENTE = 302,

            DSTIPO = 403,
        };

        /// <summary>
        /// Get TbRecebimentoAjuste/TbRecebimentoAjuste
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static IQueryable<tbRecebimentoAjuste> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbRecebimentoAjustes.AsQueryable<tbRecebimentoAjuste>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDRECEBIMENTOAJUSTE:
                        Int32 idRecebimentoAjuste = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRecebimentoAjuste == idRecebimentoAjuste).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.DTAJUSTE:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtAjuste.Year > dtaIni.Year || (e.dtAjuste.Year == dtaIni.Year && e.dtAjuste.Month > dtaIni.Month) ||
                                                                                          (e.dtAjuste.Year == dtaIni.Year && e.dtAjuste.Month == dtaIni.Month && e.dtAjuste.Day >= dtaIni.Day))
                                                    && (e.dtAjuste.Year < dtaFim.Year || (e.dtAjuste.Year == dtaFim.Year && e.dtAjuste.Month < dtaFim.Month) ||
                                                                                          (e.dtAjuste.Year == dtaFim.Year && e.dtAjuste.Month == dtaFim.Month && e.dtAjuste.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtAjuste >= dta);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtAjuste.Year == dtaIni.Year && e.dtAjuste.Month == dtaIni.Month && e.dtAjuste.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira == cdBandeira).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.DSMOTIVO:
                        string dsMotivo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsMotivo.Equals(dsMotivo)).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.VLAJUSTE:
                        decimal vlAjuste = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlAjuste == vlAjuste).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.IDEXTRATO:
                        Int32 idExtrato = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idExtrato == idExtrato).AsQueryable<tbRecebimentoAjuste>();
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.empresa.id_grupo == id_grupo).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbBandeira.cdAdquirente == cdAdquirente).AsQueryable<tbRecebimentoAjuste>();
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value).TrimEnd();
                        entity = entity.Where(e => e.tbBandeira.dsTipo.TrimEnd().Equals(dsTipo)).AsQueryable<tbRecebimentoAjuste>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDRECEBIMENTOAJUSTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRecebimentoAjuste).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.idRecebimentoAjuste).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.DTAJUSTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtAjuste).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.dtAjuste).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.DSMOTIVO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsMotivo).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.dsMotivo).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.VLAJUSTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlAjuste).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.vlAjuste).AsQueryable<tbRecebimentoAjuste>();
                    break;
                case CAMPOS.IDEXTRATO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idExtrato).AsQueryable<tbRecebimentoAjuste>();
                    else entity = entity.OrderByDescending(e => e.idExtrato).AsQueryable<tbRecebimentoAjuste>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbRecebimentoAjuste/TbRecebimentoAjuste
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbRecebimentoAjuste = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);


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

            // COLEÇÃO DE RETORNO
            if (colecao == 1)
            {
                CollectionTbRecebimentoAjuste = query.Select(e => new
                {

                    idRecebimentoAjuste = e.idRecebimentoAjuste,
                    dtAjuste = e.dtAjuste,
                    nrCNPJ = e.nrCNPJ,
                    cdBandeira = e.cdBandeira,
                    dsMotivo = e.dsMotivo,
                    vlAjuste = e.vlAjuste,
                    idExtrato = e.idExtrato,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbRecebimentoAjuste = query.Select(e => new
                {

                    idRecebimentoAjuste = e.idRecebimentoAjuste,
                    dtAjuste = e.dtAjuste,
                    nrCNPJ = e.nrCNPJ,
                    cdBandeira = e.cdBandeira,
                    dsMotivo = e.dsMotivo,
                    vlAjuste = e.vlAjuste,
                    idExtrato = e.idExtrato,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbRecebimentoAjuste;

            return retorno;
        }
        ///// <summary>
        ///// Adiciona nova TbRecebimentoAjuste
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public static Int32 Add(string token, tbRecebimentoAjuste param)
        //{
        //    _db.tbRecebimentoAjustes.Add(param);
        //    _db.SaveChanges();
        //    return param.idRecebimentoAjuste;
        //}


        ///// <summary>
        ///// Apaga uma TbRecebimentoAjuste
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public static void Delete(string token, Int32 idRecebimentoAjuste)
        //{
        //    _db.tbRecebimentoAjustes.Remove(_db.tbRecebimentoAjustes.Where(e => e.idRecebimentoAjuste.Equals(idRecebimentoAjuste)).First());
        //    _db.SaveChanges();
        //}
        ///// <summary>
        ///// Altera tbRecebimentoAjuste
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public static void Update(string token, tbRecebimentoAjuste param)
        //{
        //    tbRecebimentoAjuste value = _db.tbRecebimentoAjustes
        //            .Where(e => e.idRecebimentoAjuste.Equals(param.idRecebimentoAjuste))
        //            .First<tbRecebimentoAjuste>();


        //    if (param.idRecebimentoAjuste != 0 && param.idRecebimentoAjuste != value.idRecebimentoAjuste)
        //        value.idRecebimentoAjuste = param.idRecebimentoAjuste;
        //    if (param.dtAjuste != null && param.dtAjuste != value.dtAjuste)
        //        value.dtAjuste = param.dtAjuste;
        //    if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
        //        value.nrCNPJ = param.nrCNPJ;
        //    if (param.cdBandeira != 0 && param.cdBandeira != value.cdBandeira)
        //        value.cdBandeira = param.cdBandeira;
        //    if (param.dsMotivo != null && param.dsMotivo != value.dsMotivo)
        //        value.dsMotivo = param.dsMotivo;
        //    if (param.vlAjuste != 0 && param.vlAjuste != value.vlAjuste)
        //        value.vlAjuste = param.vlAjuste;
        //    if (param.idExtrato != null && param.idExtrato != value.idExtrato)
        //        value.idExtrato = param.idExtrato;
        //    _db.SaveChanges();

        //}

    }
}
