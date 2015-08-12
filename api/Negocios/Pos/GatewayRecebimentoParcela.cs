using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Globalization;

namespace api.Negocios.Pos
{
    public class GatewayRecebimentoParcela
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRecebimentoParcela()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

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

            // EMPRESA
            NU_CNPJ = 300,
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
        private static IQueryable<RecebimentoParcela> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
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
                    
                    /// PERSONALIZADO
                    
                    case CAMPOS.DTARECEBIMENTO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtaRecebimento.Year > dtaIni.Year || (e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month > dtaIni.Month) ||
                                                                                          (e.dtaRecebimento.Year == dtaIni.Year && e.dtaRecebimento.Month == dtaIni.Month && e.dtaRecebimento.Day >= dtaIni.Day))
                                                    && (e.dtaRecebimento.Year < dtaFim.Year || (e.dtaRecebimento.Year == dtaFim.Year && e.dtaRecebimento.Month < dtaFim.Month) ||
                                                                                          (e.dtaRecebimento.Year == dtaFim.Year && e.dtaRecebimento.Month == dtaFim.Month && e.dtaRecebimento.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtaRecebimento >= dta);
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
                        entity = entity.Where(e => e.valorDescontado.Equals(valorDescontado));
                        break;




                    // PERSONALIZADO

                    case CAMPOS.ID_GRUPO:
                        int id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.Recebimento.empresa.id_grupo.Equals(id_grupo));
                        break;
                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.Recebimento.empresa.nu_cnpj.Equals(nu_cnpj));
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
                            entity = entity.Where(e => e.Recebimento.nsu.StartsWith(busca));
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira).ThenBy(e => e.Recebimento.dtaVenda);
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira).ThenBy(e => e.Recebimento.dtaVenda);
                    break;
                case CAMPOS.VALORDESCONTADO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorDescontado);
                    else entity = entity.OrderByDescending(e => e.valorDescontado);
                    break;


                // PERSONALIZADO
                case CAMPOS.DTAVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Recebimento.dtaVenda).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira).ThenBy(e => e.dtaRecebimento);
                    else entity = entity.OrderByDescending(e => e.Recebimento.dtaVenda).ThenBy(e => e.Recebimento.BandeiraPos.desBandeira).ThenBy(e => e.dtaRecebimento);
                    break;
                case CAMPOS.DESBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Recebimento.BandeiraPos.desBandeira);
                    else entity = entity.OrderByDescending(e => e.Recebimento.BandeiraPos.desBandeira);
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna RecebimentoParcela/RecebimentoParcela
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
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
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

            

            var queryTotal = query;

            // TOTAL DE REGISTROS
            retorno.TotalDeRegistros = queryTotal.Count();

            if (colecao != 9) // relatório sintético
            {
                retorno.Totais.Add("valorBruto", query.Count() > 0 ? Convert.ToDecimal(query.Sum(r => r.Recebimento.valorVendaBruta)) : 0);
                retorno.Totais.Add("valorDescontado", query.Count() > 0 ? Convert.ToDecimal(query.Sum(r => r.valorDescontado)) : 0);
                retorno.Totais.Add("valorParcelaBruta", query.Count() > 0 ? Convert.ToDecimal(query.Sum(r => r.valorParcelaBruta)) : 0);
                retorno.Totais.Add("valorParcelaLiquida", query.Count() > 0 ? Convert.ToDecimal(query.Sum(r => r.valorParcelaLiquida)) : 0);
            }

            if (colecao == 0 || colecao == 8)
            {   // coleções que não fazem groupby
                // PAGINAÇÃO
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
                CollectionRecebimentoParcela = query
                .Select(e => new
                {
                    idRecebimento = e.idRecebimento,
                    numParcela = e.numParcela,
                    valorParcelaBruta = e.valorParcelaBruta,
                    valorParcelaLiquida = e.valorParcelaLiquida,
                    dtaRecebimento = e.dtaRecebimento,
                    valorDescontado = e.valorDescontado

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
                    valorVendaBruta = e.Recebimento.valorVendaBruta,
                    valorVendaLiquida = e.Recebimento.valorVendaLiquida,
                    loteImportacao = e.Recebimento.loteImportacao,
                    //dtaRecebimento = projecao.recebimento.dtaRecebimento,
                    idLogicoTerminal = e.Recebimento.idLogicoTerminal,
                    codTituloERP = e.Recebimento.codTituloERP,
                    codVendaERP = e.Recebimento.codVendaERP,
                    codResumoVenda = e.Recebimento.codResumoVenda,
                    numParcelaTotal = e.Recebimento.numParcelaTotal,


                    idRecebimento = e.idRecebimento,
                    numParcela = e.numParcela,
                    valorParcelaBruta = e.valorParcelaBruta,
                    valorParcelaLiquida = e.valorParcelaLiquida,
                    dtaRecebimento = e.dtaRecebimento,
                    valorDescontado = e.valorDescontado,


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
                        vlParcela = (e.Sum(p => p.valorParcelaBruta)),
                        vlDescontado = (e.Sum(p => p.valorDescontado)),
                        vlLiquido = (e.Sum(p => p.valorParcelaLiquida)),
                        nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                    });

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebimentoParcela.Count();


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
                        vlParcela = (e.Sum(p => p.valorParcelaBruta)),
                        vlDescontado = (e.Sum(p => p.valorDescontado)),
                        vlLiquido = (e.Sum(p => p.valorParcelaLiquida)),
                        nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                    });

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebimentoParcela.Count();


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
                        vlParcela = (e.Sum(p => p.valorParcelaBruta)),
                        vlDescontado = (e.Sum(p => p.valorDescontado)),
                        vlLiquido = (e.Sum(p => p.valorParcelaLiquida)),
                        nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                    });

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebimentoParcela.Count();


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
                        vlParcela = (e.Sum(p => p.valorParcelaBruta)),
                        vlDescontado = (e.Sum(p => p.valorDescontado)),
                        vlLiquido = (e.Sum(p => p.valorParcelaLiquida)),
                        nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                    });

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebimentoParcela.Count();


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
                        vlParcela = (e.Sum(p => p.valorParcelaBruta)),
                        vlDescontado = (e.Sum(p => p.valorDescontado)),
                        vlLiquido = (e.Sum(p => p.valorParcelaLiquida)),
                        nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                    });

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebimentoParcela.Count();


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
                        vlParcela = (e.Sum(p => p.valorParcelaBruta)),
                        vlDescontado = (e.Sum(p => p.valorDescontado)),
                        vlLiquido = (e.Sum(p => p.valorParcelaLiquida)),
                        nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                    });

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebimentoParcela.Count();


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
                        vlParcela = (e.Sum(p => p.valorParcelaBruta)),
                        vlDescontado = (e.Sum(p => p.valorDescontado)),
                        vlLiquido = (e.Sum(p => p.valorParcelaLiquida)),
                        nrTaxa = ((e.Sum(p => p.valorDescontado)) / (e.Sum(p => p.valorParcelaBruta))) * 100
                    });

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebimentoParcela.Count();


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
            else if (colecao == 8) // [web]cashflow/Analitico
            {
                CollectionRecebimentoParcela = query
                .Select(e => new
                {

                    cnpj = e.Recebimento.cnpj,
                    desBandeira = e.Recebimento.BandeiraPos.desBandeira,
                    dtaVenda = e.Recebimento.dtaVenda,
                    dtaRecebimento = e.dtaRecebimento,
                    codResumoVenda = e.Recebimento.codResumoVenda,
                    nsu = e.Recebimento.nsu,
                    numParcela = e.numParcela + " de " + e.Recebimento.numParcelaTotal,
                    valorBruto = e.Recebimento.valorVendaBruta,
                    valorParcela = e.valorParcelaBruta,
                    valorLiquida = e.valorParcelaLiquida,
                    valorDescontado = e.valorDescontado
                }).ToList<dynamic>();
            }
            else if (colecao == 9) // [web]/cashflow/Sintético
            {
                var subQuery = query
                    .GroupBy(x => new { x.Recebimento.BandeiraPos })
                    .OrderBy(e => e.Key.BandeiraPos.desBandeira)
                    .Select(e => new
                    {
                        bandeira = new {
                                            desBandeira = e.Key.BandeiraPos.desBandeira,
                                            id = e.Key.BandeiraPos.id,
                                            idOperadora = e.Key.BandeiraPos.idOperadora
                        },
                        valorBruto = e.Sum(p => p.Recebimento.valorVendaBruta),
                        valorParcela = e.Sum(p => p.valorParcelaBruta),
                        valorLiquida = e.Sum(p => p.valorParcelaLiquida),
                        valorDescontado = e.Sum(p => p.valorDescontado),
                        totalTransacoes = e.Count()
                    });

                retorno.TotalDeRegistros = subQuery.Count();

                retorno.Totais.Add("valorBruto", subQuery.Count() > 0 ? Convert.ToDecimal(subQuery.Sum(r => r.valorBruto)) : 0);
                retorno.Totais.Add("valorDescontado", subQuery.Count() > 0 ? Convert.ToDecimal(subQuery.Sum(r => r.valorDescontado)) : 0);
                retorno.Totais.Add("valorLiquida", subQuery.Count() > 0 ? Convert.ToDecimal(subQuery.Sum(r => r.valorLiquida)) : 0);
                retorno.Totais.Add("valorParcela", subQuery.Count() > 0 ? Convert.ToDecimal(subQuery.Sum(r => r.valorParcela)) : 0);
                retorno.Totais.Add("totalTransacoes", subQuery.Count() > 0 ? Convert.ToDecimal(subQuery.Sum(r => r.totalTransacoes)) : 0);



                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    subQuery = subQuery.Skip(skipRows).Take(pageSize);
                else
                    pageNumber = 1;

                CollectionRecebimentoParcela = subQuery.ToList<dynamic>();
            }



            retorno.Registros = CollectionRecebimentoParcela;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova RecebimentoParcela
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, RecebimentoParcela param)
        {
            _db.RecebimentoParcelas.Add(param);
            _db.SaveChanges();
            return param.idRecebimento;
        }


        /// <summary>
        /// Apaga uma RecebimentoParcela
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRecebimento)
        {
            _db.RecebimentoParcelas.Remove(_db.RecebimentoParcelas.Where(e => e.idRecebimento.Equals(idRecebimento)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera RecebimentoParcela
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, RecebimentoParcela param)
        {
            RecebimentoParcela value = _db.RecebimentoParcelas
                    .Where(e => e.idRecebimento.Equals(param.idRecebimento))
                    .First<RecebimentoParcela>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.idRecebimento != null && param.idRecebimento != value.idRecebimento)
                value.idRecebimento = param.idRecebimento;
            if (param.numParcela != null && param.numParcela != value.numParcela)
                value.numParcela = param.numParcela;
            if (param.valorParcelaBruta != null && param.valorParcelaBruta != value.valorParcelaBruta)
                value.valorParcelaBruta = param.valorParcelaBruta;
            if (param.valorParcelaLiquida != null && param.valorParcelaLiquida != value.valorParcelaLiquida)
                value.valorParcelaLiquida = param.valorParcelaLiquida;
            if (param.dtaRecebimento != null && param.dtaRecebimento != value.dtaRecebimento)
                value.dtaRecebimento = param.dtaRecebimento;
            if (param.valorDescontado != null && param.valorDescontado != value.valorDescontado)
                value.valorDescontado = param.valorDescontado;
            _db.SaveChanges();

        }

    }

    public class RecebimentoToRecebimentoParcelas
    {
        public Recebimento recebimento;
        public RecebimentoParcela parcela;
    }
}
