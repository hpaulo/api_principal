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
    public class GatewayTbRecebimentoTitulo
    {
        public static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRecebimentoTitulo()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDRECEBIMENTOTITULO = 100,
            NRCNPJ = 101,
            NRNSU = 102,
            DTVENDA = 103,
            CDADQUIRENTE = 104,
            DSBANDEIRA = 106,
            VLVENDA = 107,
            QTPARCELAS = 108,
            DTTITULO = 109,
            VLPARCELA = 110,
            NRPARCELA = 111,

            // RELACIONAMENTOS
            ID_GRUPO = 216
        };

        /// <summary>
        /// Get TbRecebimentoTitulo/TbRecebimentoTitulo
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static IQueryable<tbRecebimentoTitulo> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbRecebimentoTitulos.AsQueryable<tbRecebimentoTitulo>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDRECEBIMENTOTITULO:
                        Int32 idRecebimentoTitulo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRecebimentoTitulo.Equals(idRecebimentoTitulo)).AsQueryable<tbRecebimentoTitulo>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbRecebimentoTitulo>();
                        break;
                    case CAMPOS.NRNSU:
                        string nrNSU = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrNSU.Equals(nrNSU)).AsQueryable<tbRecebimentoTitulo>();
                        break;
                    case CAMPOS.DTVENDA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda != null && e.dtVenda.Value >= dtaIni && e.dtVenda.Value <= dtaFim).AsQueryable<tbRecebimentoTitulo>();
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtVenda != null && e.dtVenda.Value.Year == dtaIni.Year && e.dtVenda.Value.Month == dtaIni.Month && e.dtVenda.Value.Day == dtaIni.Day).AsQueryable<tbRecebimentoTitulo>();
                        }
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbRecebimentoTitulo>();
                        break;
                    case CAMPOS.DSBANDEIRA:
                        string dsBandeira = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsBandeira.Equals(dsBandeira)).AsQueryable<tbRecebimentoTitulo>();
                        break;
                    case CAMPOS.VLVENDA:
                        decimal vlVenda = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlVenda.Equals(vlVenda)).AsQueryable<tbRecebimentoTitulo>();
                        break;
                    case CAMPOS.QTPARCELAS:
                        byte qtParcelas = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.qtParcelas.Equals(qtParcelas)).AsQueryable<tbRecebimentoTitulo>();
                        break;
                    case CAMPOS.DTTITULO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtTitulo >= dtaIni && e.dtTitulo <= dtaFim).AsQueryable<tbRecebimentoTitulo>();
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtTitulo.Year == dtaIni.Year && e.dtTitulo.Month == dtaIni.Month && e.dtTitulo.Day == dtaIni.Day).AsQueryable<tbRecebimentoTitulo>();
                        }
                        break;
                    case CAMPOS.VLPARCELA:
                        decimal vlParcela = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlParcela.Equals(vlParcela)).AsQueryable<tbRecebimentoTitulo>();
                        break;
                    case CAMPOS.NRPARCELA:
                        byte nrParcela = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.nrParcela.Equals(nrParcela)).AsQueryable<tbRecebimentoTitulo>();
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.empresa.id_grupo == id_grupo).AsQueryable<tbRecebimentoTitulo>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDRECEBIMENTOTITULO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRecebimentoTitulo).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.idRecebimentoTitulo).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.NRNSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrNSU).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.nrNSU).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVenda).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.dtVenda).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.DSBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsBandeira).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.dsBandeira).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.VLVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlVenda).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.vlVenda).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.QTPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtParcelas).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.qtParcelas).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.DTTITULO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtTitulo).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.dtTitulo).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.VLPARCELA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlParcela).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.vlParcela).AsQueryable<tbRecebimentoTitulo>();
                    break;
                case CAMPOS.NRPARCELA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrParcela).AsQueryable<tbRecebimentoTitulo>();
                    else entity = entity.OrderByDescending(e => e.nrParcela).AsQueryable<tbRecebimentoTitulo>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbRecebimentoTitulo/TbRecebimentoTitulo
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbRecebimentoTitulo = new List<dynamic>();
            Retorno retorno = new Retorno();

            // Implementar o filtro por Grupo apartir do TOKEN do Usuário
            string outValue = null;
            Int32 IdGrupo = 0;
            IdGrupo = Permissoes.GetIdGrupo(token);
            if (IdGrupo != 0)
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                else
                    queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
            }
            string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
            if (!CnpjEmpresa.Equals(""))
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.NRCNPJ, out outValue))
                    queryString["" + (int)CAMPOS.NRCNPJ] = CnpjEmpresa;
                else
                    queryString.Add("" + (int)CAMPOS.NRCNPJ, CnpjEmpresa);
            }

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
                CollectionTbRecebimentoTitulo = query.Select(e => new
                {

                    idRecebimentoTitulo = e.idRecebimentoTitulo,
                    nrCNPJ = e.nrCNPJ,
                    nrNSU = e.nrNSU,
                    dtVenda = e.dtVenda,
                    cdAdquirente = e.cdAdquirente,
                    dsBandeira = e.dsBandeira,
                    vlVenda = e.vlVenda,
                    qtParcelas = e.qtParcelas,
                    dtTitulo = e.dtTitulo,
                    vlParcela = e.vlParcela,
                    nrParcela = e.nrParcela,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbRecebimentoTitulo = query.Select(e => new
                {

                    idRecebimentoTitulo = e.idRecebimentoTitulo,
                    nrCNPJ = e.nrCNPJ,
                    nrNSU = e.nrNSU,
                    dtVenda = e.dtVenda,
                    cdAdquirente = e.cdAdquirente,
                    dsBandeira = e.dsBandeira,
                    vlVenda = e.vlVenda,
                    qtParcelas = e.qtParcelas,
                    dtTitulo = e.dtTitulo,
                    vlParcela = e.vlParcela,
                    nrParcela = e.nrParcela,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbRecebimentoTitulo;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbRecebimentoTitulo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbRecebimentoTitulo param)
        {
            _db.tbRecebimentoTitulos.Add(param);
            _db.SaveChanges();
            return param.idRecebimentoTitulo;
        }


        /// <summary>
        /// Apaga uma TbRecebimentoTitulo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRecebimentoTitulo)
        {
            _db.tbRecebimentoTitulos.Remove(_db.tbRecebimentoTitulos.Where(e => e.idRecebimentoTitulo.Equals(idRecebimentoTitulo)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbRecebimentoTitulo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbRecebimentoTitulo param)
        {
            tbRecebimentoTitulo value = _db.tbRecebimentoTitulos
                    .Where(e => e.idRecebimentoTitulo.Equals(param.idRecebimentoTitulo))
                    .First<tbRecebimentoTitulo>();


            if (param.idRecebimentoTitulo != null && param.idRecebimentoTitulo != value.idRecebimentoTitulo)
                value.idRecebimentoTitulo = param.idRecebimentoTitulo;
            if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                value.nrCNPJ = param.nrCNPJ;
            if (param.nrNSU != null && param.nrNSU != value.nrNSU)
                value.nrNSU = param.nrNSU;
            if (param.dtVenda != null && param.dtVenda != value.dtVenda)
                value.dtVenda = param.dtVenda;
            if (param.cdAdquirente != null && param.cdAdquirente != value.cdAdquirente)
                value.cdAdquirente = param.cdAdquirente;
            if (param.dsBandeira != null && param.dsBandeira != value.dsBandeira)
                value.dsBandeira = param.dsBandeira;
            if (param.vlVenda != null && param.vlVenda != value.vlVenda)
                value.vlVenda = param.vlVenda;
            if (param.qtParcelas != null && param.qtParcelas != value.qtParcelas)
                value.qtParcelas = param.qtParcelas;
            if (param.dtTitulo != null && param.dtTitulo != value.dtTitulo)
                value.dtTitulo = param.dtTitulo;
            if (param.vlParcela != null && param.vlParcela != value.vlParcela)
                value.vlParcela = param.vlParcela;
            if (param.nrParcela != null && param.nrParcela != value.nrParcela)
                value.nrParcela = param.nrParcela;
            _db.SaveChanges();

        }

    }
}