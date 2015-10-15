using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using System.Globalization;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Card
{
    public class GatewayTbAdquirente
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbAdquirente()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDADQUIRENTE = 100,
            NMADQUIRENTE = 101,
            DSADQUIRENTE = 102,
            STADQUIRENTE = 103,
            HREXECUCAO = 104,

            /**
              * Relacionamentos
              */

            // pos.LoginOperadoras
            CNPJ = 305,
            ID_GRUPO = 316,

            // Várias Tabelas
            DATA = 904
        };

        /// <summary>
        /// Get TbAdquirente/TbAdquirente
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbAdquirente> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbAdquirentes.AsQueryable<tbAdquirente>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.NMADQUIRENTE:
                        string nmAdquirente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmAdquirente.Equals(nmAdquirente)).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.DSADQUIRENTE:
                        string dsAdquirente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsAdquirente.Equals(dsAdquirente)).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.STADQUIRENTE:
                        byte stAdquirente = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.stAdquirente.Equals(stAdquirente)).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.HREXECUCAO:
                        DateTime hrExecucao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.hrExecucao.Equals(hrExecucao)).AsQueryable<tbAdquirente>();
                        break;

                    // Relacionamento
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        List<string> nmOperadoras = _db.Operadoras.Where(o => o.LoginOperadoras.Where(l => l.cnpj.Equals(cnpj)).Count() > 0).Select(o => o.nmOperadora).ToList<string>();
                        entity = entity.Where(e => nmOperadoras.Contains(e.dsAdquirente)).AsQueryable<tbAdquirente>();
                        break;
                    case CAMPOS.ID_GRUPO:
                        int id_grupo = Convert.ToInt32(item.Value);
                        List<string> nmOperadorasG = _db.Operadoras.Where(o => o.LoginOperadoras.Where(l => l.empresa.id_grupo == id_grupo).Count() > 0).Select(o => o.nmOperadora).ToList<string>();
                        entity = entity.Where(e => nmOperadorasG.Contains(e.dsAdquirente)).AsQueryable<tbAdquirente>();
                        break;

                    case CAMPOS.DATA:                        
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            
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
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbAdquirente>();
                    break;
                case CAMPOS.NMADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmAdquirente).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.nmAdquirente).AsQueryable<tbAdquirente>();
                    break;
                case CAMPOS.DSADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsAdquirente).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.dsAdquirente).AsQueryable<tbAdquirente>();
                    break;
                case CAMPOS.STADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.stAdquirente).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.stAdquirente).AsQueryable<tbAdquirente>();
                    break;
                case CAMPOS.HREXECUCAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.hrExecucao).AsQueryable<tbAdquirente>();
                    else entity = entity.OrderByDescending(e => e.hrExecucao).AsQueryable<tbAdquirente>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbAdquirente/TbAdquirente
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbAdquirente = new List<dynamic>();
            Retorno retorno = new Retorno();

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
            if (!CnpjEmpresa.Equals(""))
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.CNPJ, out outValue))
                    queryString["" + (int)CAMPOS.CNPJ] = CnpjEmpresa;
                else
                    queryString.Add("" + (int)CAMPOS.CNPJ, CnpjEmpresa);
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
                CollectionTbAdquirente = query.Select(e => new
                {

                    cdAdquirente = e.cdAdquirente,
                    nmAdquirente = e.nmAdquirente,
                    dsAdquirente = e.dsAdquirente,
                    stAdquirente = e.stAdquirente,
                    hrExecucao = e.hrExecucao,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbAdquirente = query.Select(e => new
                {

                    cdAdquirente = e.cdAdquirente,
                    nmAdquirente = e.nmAdquirente,
                    dsAdquirente = e.dsAdquirente,
                    stAdquirente = e.stAdquirente,
                    hrExecucao = e.hrExecucao,
                }).ToList<dynamic>();
            }
            else if (colecao == 2) // Conciliação Relatórios
            {
                CollectionTbAdquirente = query.Select(e => new
                {

                    cdAdquirente = e.cdAdquirente,
                    nmAdquirente = e.nmAdquirente,
                    bandeiras = e.tbBandeiras.Select(b => new
                    {
                        cdBandeira = b.cdBandeira,
                        dsBandeira = b.dsBandeira,

                        ajustesCredito = b.tbRecebimentoAjustes.Where(a => a.vlAjuste > new decimal(0.0)).Count() > 0 ?
                                        b.tbRecebimentoAjustes
                                            .Where(a => a.vlAjuste > new decimal(0.0))
                                            .Sum(a => a.vlAjuste) : new decimal(0.0),
                        ajustesDebito = b.tbRecebimentoAjustes.Where(a => a.vlAjuste > new decimal(0.0)).Count() > 0 ?
                                        b.tbRecebimentoAjustes
                                            .Where(a => a.vlAjuste < new decimal(0.0))
                                            .Sum(a => a.vlAjuste) : new decimal(0.0),                        
                    }),                    
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbAdquirente;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova TbAdquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbAdquirente param)
        {
            _db.tbAdquirentes.Add(param);
            _db.SaveChanges();
            return param.cdAdquirente;
        }


        /// <summary>
        /// Apaga uma TbAdquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdAdquirente)
        {
            _db.tbAdquirentes.Remove(_db.tbAdquirentes.Where(e => e.cdAdquirente.Equals(cdAdquirente)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera tbAdquirente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbAdquirente param)
        {
            tbAdquirente value = _db.tbAdquirentes
                    .Where(e => e.cdAdquirente.Equals(param.cdAdquirente))
                    .First<tbAdquirente>();


            if (param.cdAdquirente != value.cdAdquirente)
                value.cdAdquirente = param.cdAdquirente;
            if (param.nmAdquirente != null && param.nmAdquirente != value.nmAdquirente)
                value.nmAdquirente = param.nmAdquirente;
            if (param.dsAdquirente != null && param.dsAdquirente != value.dsAdquirente)
                value.dsAdquirente = param.dsAdquirente;
            if (param.stAdquirente != value.stAdquirente)
                value.stAdquirente = param.stAdquirente;
            if (param.hrExecucao != null && param.hrExecucao != value.hrExecucao)
                value.hrExecucao = param.hrExecucao;
            _db.SaveChanges();

        }

    }
}