using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using api.Negocios.Pos;
using Microsoft.Ajax.Utilities;
using api.Negocios.Util;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace api.Negocios.Card
{
    public class GatewayConciliacaoRelatorios
    {

        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoRelatorios()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static decimal TOLERANCIA_PRE_CONCILIADO = new decimal(0.06);

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100, 
            ID_GRUPO = 101,
            NU_CNPJ = 102, 
        };

        /// <summary>
        /// Retorna a lista de conciliação bancária
        /// </summary>
        /// <returns></returns>        
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            // Abre conexão
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionConciliacaoRelatorios = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                Dictionary<string, string> queryStringTbRecebimentoAjuste = new Dictionary<string, string>();
                Dictionary<string, string> queryStringExtrato = new Dictionary<string, string>();
                // DATA (yyyyMM)
                string data = String.Empty;
                if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                {
                    data = queryString["" + (int)CAMPOS.DATA];
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, data);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDBANDEIRA, "0");
                    queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, data);
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, data);
                }
                else throw new Exception("Uma data deve ser selecionada como filtro de conciliação bancária!");
                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de relatório de conciliação!");
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ, CnpjEmpresa);
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.NU_CNPJ, CnpjEmpresa);
                }
                //else throw new Exception("Uma filial deve ser selecionada como filtro de conciliação bancária!");

                // Vigência
                string vigencia = CnpjEmpresa;
                if (!data.Equals(""))
                {
                    if (data.Length == 6)
                    {
                        int dia = 28;
                        int mes = Convert.ToInt32(data.Substring(4, 2));
                        int ano = Convert.ToInt32(data.Substring(0, 4));
                        while (true)
                        {
                            try
                            {
                                Convert.ToDateTime((dia + 1) + "/" + mes + "/" + ano);
                                //DateTime.ParseExact(data + "" + dia + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                dia++;
                            } catch
                            {
                                break;
                            }                            
                        }
                        data = data + "01|" + data + dia;
                    }
                    vigencia += "!" + data;
                }            
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.VIGENCIA, vigencia);
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE, "0!"); // Somente movimentações que tem adquirente/tipo cartão associado
                // PARA O EXTRATO, SÓ CONSIDERA OS TIPO CREDIT
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DSTIPO, OFXSharp.OFXTransactionType.CREDIT.ToString());

                // Sem ajustes de antecipação
                queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.SEM_AJUSTES_ANTECIPACAO, true.ToString());


                // OBTÉM AS QUERIES                
                //IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(_db, 0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, 0, 0, queryStringRecebimentoParcela);
                //IQueryable<tbRecebimentoAjuste> queryTbRecebimentoAjuste = GatewayTbRecebimentoAjuste.getQuery(_db, 0, (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, 0, 0, queryStringTbRecebimentoAjuste);
                //IQueryable<tbExtrato> queryExtrato = GatewayTbExtrato.getQuery(_db, 0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, 0, 0, queryStringExtrato);


                //List<ConciliacaoRelatorios> rRecebimentoParcela = queryRecebimentoParcela.Select(t => new ConciliacaoRelatorios
                //{
                //    tipo = "R",
                //    adquirente = t.Recebimento.tbBandeira.tbAdquirente.nmAdquirente,
                //    bandeira = t.Recebimento.tbBandeira.dsBandeira,
                //    tipocartao = t.Recebimento.tbBandeira.dsTipo,
                //    competencia = t.dtaRecebimentoEfetivo != null ? t.dtaRecebimentoEfetivo.Value : t.dtaRecebimento,
                //    valorDescontado = t.valorDescontado,
                //    valorDescontadoAntecipacao = t.vlDescontadoAntecipacao,
                //    valorBruto = t.valorParcelaBruta,
                //    valorLiquido = t.valorParcelaBruta - t.valorDescontado,//t.valorParcelaLiquida.Value,
                //    idExtrato = t.idExtrato,
                //    taxaCashFlow = (t.valorDescontado * new decimal(100.0)) / t.valorParcelaBruta
                //}).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();

                //List<ConciliacaoRelatorios> rRecebimentoAjuste = queryTbRecebimentoAjuste.Select(t => new ConciliacaoRelatorios
                //{
                //    tipo = "A",
                //    adquirente = t.tbBandeira.tbAdquirente.nmAdquirente,
                //    bandeira = t.tbBandeira.dsBandeira,
                //    tipocartao = t.tbBandeira.dsTipo,
                //    competencia = t.dtAjuste,
                //    valorLiquido = t.vlAjuste,
                //    valorDescontadoAntecipacao = new decimal(0.0),
                //    valorBruto = t.vlAjuste > new decimal(0.0) ? t.vlAjuste : new decimal(0.0),
                //    valorDescontado = t.vlAjuste < new decimal(0.0) ? new decimal(-1.0) * t.vlAjuste : new decimal(0.0),
                //    idExtrato = t.idExtrato,
                //    taxaCashFlow = new decimal(0.0) 
                //}).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();

                // CONEXÃO
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

                try
                {
                    connection.Open();
                }
                catch
                {
                    throw new Exception("Não foi possível estabelecer conexão com a base de dados");
                }

                List<ConciliacaoRelatorios> rRecebimentoParcela = new List<ConciliacaoRelatorios>();
                List<ConciliacaoRelatorios> rRecebimentoAjuste = new List<ConciliacaoRelatorios>();
                List<ConciliacaoRelatorios> rMovimentacoesBancarias = new List<ConciliacaoRelatorios>();

                try
                {

                    #region OBTÉM COMPONENTES QUERIES
                    SimpleDataBaseQuery dataBaseQueryRP = GatewayRecebimentoParcela.getQuery((int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, queryStringRecebimentoParcela);
                    SimpleDataBaseQuery dataBaseQueryAJ = GatewayTbRecebimentoAjuste.getQuery((int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, queryStringTbRecebimentoAjuste);
                    SimpleDataBaseQuery dataBaseQueryEX = GatewayTbExtrato.getQuery((int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, queryStringExtrato);

                    // Adiciona JOINS, caso não tenham

                    // RECEBIMENTO PARCELA
                    if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimento");
                    if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                    if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");

                    // AJUSTES
                    if (!dataBaseQueryAJ.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                        dataBaseQueryAJ.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".cdBandeira");
                    if (!dataBaseQueryAJ.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        dataBaseQueryAJ.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");

                    // EXTRATO
                    if (!dataBaseQueryEX.join.ContainsKey("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + GatewayTbExtrato.SIGLA_QUERY + ".cdContaCorrente");
                    if (!dataBaseQueryEX.join.ContainsKey("INNER JOIN card.tbBancoParametro " + GatewayTbBancoParametro.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("INNER JOIN card.tbBancoParametro " + GatewayTbBancoParametro.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdGrupo = " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdGrupo AND " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdBanco = " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdBanco AND " + GatewayTbBancoParametro.SIGLA_QUERY + ".dsMemo = " + GatewayTbExtrato.SIGLA_QUERY + ".dsDocumento");
                    if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("LEFT JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente");
                    if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("LEFT JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdBandeira");
                    //if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY))
                    //    dataBaseQueryEX.join.Add("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY, " ON " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato = " + GatewayTbExtrato.SIGLA_QUERY + ".idExtrato");
                    //if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY))
                    //    dataBaseQueryEX.join.Add("LEFT JOIN card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY, " ON " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato = " + GatewayTbExtrato.SIGLA_QUERY + ".idExtrato");


                    // Obtém o select de cada um

                    // RECEBIMENTO PARCELA
                    dataBaseQueryRP.select = new string[] { "tipo = 'R'",
                                                        GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente AS adquirente",
                                                        GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira AS bandeira",
                                                        GatewayTbBandeira.SIGLA_QUERY + ".dsTipo AS tipocartao",
                                                        "competencia = ISNULL(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo, " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento)",
                                                        "valorBruto = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta)",
                                                        "valorDescontado = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorDescontado)",
                                                        "valorDescontadoAntecipacao = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".vlDescontadoAntecipacao)",
                                                        "valorLiquido = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta - "  + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorDescontado)",
                                                       // "idExtrato = CASE WHEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato IS NULL THEN 0 ELSE 1 END",
                                                       "idExtrato = CASE WHEN COUNT (CASE WHEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato IS NULL THEN 1 ELSE 0 END) > 0 THEN 0 ELSE 1 END",
                                                       "taxaCashFlow = CASE WHEN COUNT (*) = 0 THEN 0 ELSE SUM ((" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorDescontado * 100.0) / (CASE WHEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta = 0 THEN 1 ELSE " + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta END)) / COUNT (*) END"
                                                      };
                    dataBaseQueryRP.orderby = null;
                    dataBaseQueryRP.groupby = new[] { "ISNULL(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo, " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento)",
                                                  GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                  GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                  GatewayTbBandeira.SIGLA_QUERY + ".dsTipo",
                                                };

                    // AJUSTE
                    //    tipo = "A",
                    //    adquirente = t.tbBandeira.tbAdquirente.nmAdquirente,
                    //    bandeira = t.tbBandeira.dsBandeira,
                    //    tipocartao = t.tbBandeira.dsTipo,
                    //    competencia = t.dtAjuste,
                    //    valorLiquido = t.vlAjuste,
                    //    valorDescontadoAntecipacao = new decimal(0.0),
                    //    valorBruto = t.vlAjuste > new decimal(0.0) ? t.vlAjuste : new decimal(0.0),
                    //    valorDescontado = t.vlAjuste < new decimal(0.0) ? new decimal(-1.0) * t.vlAjuste : new decimal(0.0),
                    //    idExtrato = t.idExtrato,
                    //    taxaCashFlow = new decimal(0.0) 
                    dataBaseQueryAJ.select = new string[] { "tipo = 'A'",
                                                        GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente AS adquirente",
                                                        GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira AS bandeira",
                                                        GatewayTbBandeira.SIGLA_QUERY + ".dsTipo AS tipocartao",
                                                        GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste AS competencia",
                                                        //"valorBruto = SUM(CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste > 0.0 THEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste ELSE 0.0 END)",
                                                        "valorBruto = SUM(CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlBruto != 0.0 THEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlBruto ELSE CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste > 0.0 THEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste ELSE 0.0 END END)",
                                                        "valorDescontado = SUM(CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlBruto != 0.0 THEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlBruto - " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste ELSE CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste < 0.0 THEN -1 * " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste ELSE 0.0 END END)",
                                                        "valorDescontadoAntecipacao = 0.0",
                                                        "valorLiquido = SUM(" + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste)",
                                                        "idExtrato = CASE WHEN COUNT(CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato IS NULL THEN 1 ELSE 0 END) > 0 THEN 0 ELSE 1 END",
                                                        "taxaCashFlow = 0.0"
                                                      };
                    dataBaseQueryAJ.orderby = null;
                    dataBaseQueryAJ.groupby = new[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste",
                                                  GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                  GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                  GatewayTbBandeira.SIGLA_QUERY + ".dsTipo",
                                                };

                    // EXTRATO
                    dataBaseQueryEX.select = new string[] { "tipo = 'E'",
                                                        "adquirente = ISNULL(" + GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente, 'INDEFINIDA')",
                                                        "bandeira = ISNULL(" + GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira, 'INDEFINIDA')",
                                                        "tipocartao = ISNULL(" + GatewayTbBandeira.SIGLA_QUERY + ".dsTipo, '')",
                                                        "competencia = " + GatewayTbExtrato.SIGLA_QUERY + ".dtExtrato",
                                                        //"valorBruto = SUM(ISNULL(" + GatewayTbExtrato.SIGLA_QUERY + ".vlMovimento, 0))",
                                                        "valorBruto = ISNULL(" + GatewayTbExtrato.SIGLA_QUERY + ".vlMovimento, 0)",
                                                        "valorDescontado = 0.0",
                                                        "valorDescontadoAntecipacao = 0.0",
                                                        //"valorLiquido = SUM(ISNULL(" + GatewayTbExtrato.SIGLA_QUERY + ".vlMovimento, 0))",
                                                        "valorLiquido = ISNULL(" + GatewayTbExtrato.SIGLA_QUERY + ".vlMovimento, 0)",
                                                        //"idExtrato = COUNT(CASE WHEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato IS NOT NULL OR " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato IS NOT NULL THEN 1 ELSE 0 END)",
                                                        "idExtrato = " + GatewayTbExtrato.SIGLA_QUERY + ".idExtrato",
                                                        "taxaCashFlow = 0.0"
                                                      };
                    dataBaseQueryEX.orderby = new[] { GatewayTbExtrato.SIGLA_QUERY + ".dtExtrato",
                                                  "ISNULL(" + GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente, 'INDEFINIDA')",
                                                  "ISNULL(" + GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira, 'INDEFINIDA')"
                                                  };
                    //dataBaseQueryEX.groupby = new[] { GatewayTbExtrato.SIGLA_QUERY + ".dtExtrato",
                    //                                  GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                    //                                  GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                    //                                  GatewayTbBandeira.SIGLA_QUERY + ".dsTipo",
                    //                                  GatewayTbExtrato.SIGLA_QUERY + ".idExtrato",
                    //                                };


                    dataBaseQueryRP.readUncommited = true;
                    dataBaseQueryAJ.readUncommited = true;
                    dataBaseQueryEX.readUncommited = true;
                    dataBaseQueryEX.readDistinct = true;

                    string scriptRP = dataBaseQueryRP.Script();
                    string scriptAJ = dataBaseQueryAJ.Script();
                    string scriptEX = dataBaseQueryEX.Script();
                    #endregion

                    List<IDataRecord> resultado = DataBaseQueries.SqlQuery(scriptRP, connection);
                    if (resultado != null && resultado.Count > 0)
                    {
                        rRecebimentoParcela = resultado.Select(t => new ConciliacaoRelatorios
                        {
                            adquirente = Convert.ToString(t["adquirente"]),
                            bandeira = Convert.ToString(t["bandeira"]),
                            tipocartao = Convert.ToString(t["tipocartao"]),
                            competencia = (DateTime)t["competencia"],
                            valorBruto = Convert.ToDecimal(t["valorBruto"]),
                            valorDescontado = Convert.ToDecimal(t["valorDescontado"]),
                            valorDescontadoAntecipacao = Convert.ToDecimal(t["valorDescontadoAntecipacao"]),
                            valorLiquido = Convert.ToDecimal(t["valorLiquido"]),
                            idExtrato = Convert.ToInt32(t["idExtrato"]),
                            taxaCashFlow = Convert.ToDecimal(t["taxaCashFlow"]),
                            tipo = Convert.ToString(t["tipo"]),
                        }).ToList<ConciliacaoRelatorios>();
                    }
                    resultado = DataBaseQueries.SqlQuery(scriptAJ, connection);
                    if (resultado != null && resultado.Count > 0)
                    {
                        rRecebimentoAjuste = resultado.Select(t => new ConciliacaoRelatorios
                        {
                            adquirente = Convert.ToString(t["adquirente"]),
                            bandeira = Convert.ToString(t["bandeira"]),
                            tipocartao = Convert.ToString(t["tipocartao"]),
                            competencia = (DateTime)t["competencia"],
                            valorBruto = Convert.ToDecimal(t["valorBruto"]),
                            valorDescontado = Math.Abs(Convert.ToDecimal(t["valorDescontado"])),
                            valorDescontadoAntecipacao = Convert.ToDecimal(t["valorDescontadoAntecipacao"]),
                            valorLiquido = Convert.ToDecimal(t["valorLiquido"]),
                            idExtrato = Convert.ToInt32(t["idExtrato"]),
                            taxaCashFlow = Convert.ToDecimal(t["taxaCashFlow"]),
                            tipo = Convert.ToString(t["tipo"]),
                        }).ToList<ConciliacaoRelatorios>();
                    }
                    resultado = DataBaseQueries.SqlQuery(scriptEX, connection);
                    if (resultado != null && resultado.Count > 0)
                    {
                        rMovimentacoesBancarias = resultado.Select(r => new ConciliacaoRelatorios
                        {
                            adquirente = Convert.ToString(r["adquirente"]),
                            bandeira = Convert.ToString(r["bandeira"]),
                            tipocartao = Convert.ToString(r["tipocartao"]),
                            competencia = (DateTime)r["competencia"],
                            valorBruto = Convert.ToDecimal(r["valorBruto"]),
                            valorDescontado = Convert.ToDecimal(r["valorDescontado"]),
                            valorDescontadoAntecipacao = Convert.ToDecimal(r["valorDescontadoAntecipacao"]),
                            valorLiquido = Convert.ToDecimal(r["valorLiquido"]),
                            idExtrato = Convert.ToInt32(r["idExtrato"]),
                            taxaCashFlow = Convert.ToDecimal(r["taxaCashFlow"]),
                            tipo = Convert.ToString(r["tipo"]),
                        }).ToList<ConciliacaoRelatorios>();
                        //for (int k = 0; k < resultado.Count; k++)
                        //{
                        //    IDataRecord record = resultado[k];
                        //    int idExtrato = Convert.ToInt32(record["idExtrato"]);
                        //    int parcelasConciliadas = _db.Database.SqlQuery<int>("SELECT COUNT(*)" +
                        //                                                         " FROM pos.RecebimentoParcela P (nolock)" +
                        //                                                         " WHERE P.idExtrato = " + idExtrato
                        //                                                        ).FirstOrDefault();
                        //    int ajustesConciliados = 0;
                        //    if (parcelasConciliadas == 0)
                        //    {
                        //        // Vê os ajustes
                        //        ajustesConciliados = _db.Database.SqlQuery<int>("SELECT COUNT(*)" +
                        //                                                         " FROM card.tbRecebimentoAjuste A (nolock)" +
                        //                                                         " WHERE A.idExtrato = " + idExtrato
                        //                                                        ).FirstOrDefault();
                        //    }
                        //    // else => não precisa buscar ajustes pois já sabe que a movimentação bancária está conciliada
                        //    idExtrato = parcelasConciliadas + ajustesConciliados;

                        //    // Procura parcelas 
                        //    rMovimentacoesBancarias.Add(new ConciliacaoRelatorios
                        //    {
                        //        adquirente = Convert.ToString(record["adquirente"]),
                        //        bandeira = Convert.ToString(record["bandeira"]),
                        //        tipocartao = Convert.ToString(record["tipocartao"]),
                        //        competencia = (DateTime)record["competencia"],
                        //        valorBruto = Convert.ToDecimal(record["valorBruto"]),
                        //        valorDescontado = Convert.ToDecimal(record["valorDescontado"]),
                        //        valorDescontadoAntecipacao = Convert.ToDecimal(record["valorDescontadoAntecipacao"]),
                        //        valorLiquido = Convert.ToDecimal(record["valorLiquido"]),
                        //        idExtrato = idExtrato,
                        //        taxaCashFlow = Convert.ToDecimal(record["taxaCashFlow"]),
                        //        tipo = Convert.ToString(record["tipo"]),
                        //    });
                        //}

                        //// Agrupa
                        //rMovimentacoesBancarias = rMovimentacoesBancarias
                        //                                    .GroupBy(t => new { t.competencia, t.adquirente, t.bandeira, t.tipocartao })
                        //                                    .Select(t => new ConciliacaoRelatorios
                        //                                    {
                        //                                        adquirente = t.Key.adquirente,
                        //                                        bandeira = t.Key.bandeira,
                        //                                        tipocartao = t.Key.tipocartao,
                        //                                        competencia = t.Key.competencia,
                        //                                        valorBruto = t.Sum(x => x.valorBruto),
                        //                                        valorDescontado = t.Sum(x => x.valorDescontado),
                        //                                        valorDescontadoAntecipacao = t.Sum(x => x.valorDescontadoAntecipacao),
                        //                                        valorLiquido = t.Sum(x => x.valorLiquido),
                        //                                        idExtrato = t.Where(x => x.idExtrato == 0).Count() > 0 ? 0 : 1,
                        //                                        taxaCashFlow = new decimal(0.0),
                        //                                        tipo = "E",
                        //                                    })
                        //                                    .ToList<ConciliacaoRelatorios>();
                    }
                }
                catch (Exception e)
                {
                    if (e is DbEntityValidationException)
                    {
                        string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                        throw new Exception(erro.Equals("") ? "Falha ao listar recebimento parcela" : erro);
                    }
                    throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
                }
                finally
                {
                    try
                    {
                        connection.Close();
                    }
                    catch { }
                }

                // Agrupa
                //List<dynamic> listaFinal = rRecebimentoParcela.Concat(rRecebimentoAjuste)//.Concat(rMovimentacoesBancarias)
                //                                    .GroupBy(t => t.competencia)
                //                                    .OrderBy(t => t.Key)
                //                                    .Select(t => new
                //                                    {
                //                                        competencia = t.Key.ToShortDateString(),
                //                                        taxaMedia = t.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado * new decimal(100.0) / x.valorBruto)) / t.Where(x => x.tipo.Equals("R")).Count(),
                //                                        vendas = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                //                                        valorDescontadoTaxaADM = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                //                                        ajustesCredito = t.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                //                                        ajustesDebito = t.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                //                                        valorLiquido = t.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                //                                        valorDescontadoAntecipacao = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                //                                        valorLiquidoTotal = t.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                //                                        extratoBancario = t.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                //                                        diferenca = Math.Abs(t.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao) - t.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)),
                //                                        status = t.Where(x => x.idExtrato == 0).Count() > 0 ? Math.Abs(t.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao) - t.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)) <= TOLERANCIA_PRE_CONCILIADO ? "Pré-Conciliado" : "Não conciliado" : "Conciliado",
                //                                        adquirentes = t.GroupBy(c => c.adquirente)
                //                                                       .OrderBy(c => c.Key)
                //                                                       .Select(c => new
                //                                                       {
                //                                                           adquirente = c.Key,
                //                                                           taxaMedia = c.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado * new decimal(100.0) / x.valorBruto)) / c.Where(x => x.tipo.Equals("R")).Count(),
                //                                                           vendas = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                //                                                           valorDescontadoTaxaADM = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                //                                                           ajustesCredito = c.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                //                                                           ajustesDebito = c.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                //                                                           valorLiquido = c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                //                                                           valorDescontadoAntecipacao = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                //                                                           valorLiquidoTotal = c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                //                                                           extratoBancario = c.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                //                                                           diferenca = Math.Abs(c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao) - c.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)),
                //                                                           status = c.Where(x => x.idExtrato == 0).Count() > 0 ? Math.Abs(c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao) - c.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)) <= TOLERANCIA_PRE_CONCILIADO ? "Pré-Conciliado" : "Não conciliado" : "Conciliado",
                //                                                           bandeiras = c.GroupBy(b => new { b.bandeira, b.tipocartao })
                //                                                                        .OrderBy(b => b.Key.bandeira)
                //                                                                        .ThenBy(b => b.Key.tipocartao)
                //                                                                        .Select(b => new
                //                                                                        {
                //                                                                            bandeira = b.Key.bandeira,
                //                                                                            taxaMedia = b.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado * new decimal(100.0) / x.valorBruto)) / b.Where(x => x.tipo.Equals("R")).Count(),
                //                                                                            vendas = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                //                                                                            valorDescontadoTaxaADM = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                //                                                                            ajustesCredito = b.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                //                                                                            ajustesDebito = b.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                //                                                                            valorLiquido = b.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                //                                                                            valorDescontadoAntecipacao = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                //                                                                            valorLiquidoTotal = b.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                //                                                                            extratoBancario = b.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                //                                                                            diferenca = Math.Abs(b.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao) - b.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)),
                //                                                                            status = b.Where(x => x.idExtrato == 0).Count() > 0 ? Math.Abs(b.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao) - b.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)) <= TOLERANCIA_PRE_CONCILIADO ? "Pré-Conciliado" : "Não conciliado" : "Conciliado",
                //                                                                        }).ToList<dynamic>()
                //                                                       })
                //                                                       .ToList<dynamic>()
                //                                    })
                //                                    .ToList<dynamic>();
                // Agrupa
                List<dynamic> listaFinal = rRecebimentoParcela.Concat(rRecebimentoAjuste)
                                                           .GroupBy(t => t.competencia)
                                                           .OrderBy(t => t.Key)
                                                           .Select(t => new
                                                           {
                                                               competencia = t.Key.ToShortDateString(),
                                                               taxaMedia = t.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado * new decimal(100.0) / x.valorBruto)) / t.Where(x => x.tipo.Equals("R")).Count(),
                                                               vendas = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                               valorDescontadoTaxaADM = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                               ajustesCredito = t.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                               ajustesDebito = t.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                               valorLiquido = t.Sum(x => x.valorLiquido),
                                                               valorDescontadoAntecipacao = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                               valorLiquidoTotal = t.Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                               extratoBancario = new decimal(0.0),
                                                               diferenca = t.Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                               status = t.Where(x => x.idExtrato == /*null*/0).Count() > 0 ? "Não conciliado" : "Conciliado",

                                                               adquirentes = t.GroupBy(c => c.adquirente)
                                                                               .OrderBy(c => c.Key)
                                                                               .Select(c => new
                                                                               {
                                                                                   adquirente = c.Key,
                                                                                   taxaMedia = c.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado * new decimal(100.0) / x.valorBruto)) / c.Where(x => x.tipo.Equals("R")).Count(),
                                                                                   vendas = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                                                   valorDescontadoTaxaADM = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                                                   ajustesCredito = c.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                                                   ajustesDebito = c.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                                                   valorLiquido = c.Sum(x => x.valorLiquido),
                                                                                   valorDescontadoAntecipacao = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                   valorLiquidoTotal = c.Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                   extratoBancario = new decimal(0.0),
                                                                                   diferenca = c.Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                   status = c.Where(x => x.idExtrato == /*null*/0).Count() > 0 ? "Não conciliado" : "Conciliado",

                                                                                   bandeiras = c.GroupBy(b => new { b.bandeira, b.tipocartao })
                                                                                                .OrderBy(b => b.Key.bandeira)
                                                                                                .ThenBy(b => b.Key.tipocartao)
                                                                                                .Select(b => new
                                                                                                {
                                                                                                    bandeira = b.Key.bandeira,
                                                                                                    taxaMedia = b.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado * new decimal(100.0) / x.valorBruto)) / b.Where(x => x.tipo.Equals("R")).Count(),
                                                                                                    vendas = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                                                                    valorDescontadoTaxaADM = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                                                                    ajustesCredito = b.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                                                                    ajustesDebito = b.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                                                                    valorLiquido = b.Sum(x => x.valorLiquido),
                                                                                                    valorDescontadoAntecipacao = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                                    valorLiquidoTotal = b.Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                                    extratoBancario = new decimal(0.0),
                                                                                                    diferenca = b.Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                                    status = b.Where(x => x.idExtrato == /*null*/0).Count() > 0 ? "Não conciliado" : "Conciliado",

                                                                                                }).ToList<dynamic>()
                                                                               }).ToList<dynamic>(),

                                                           })
                                                           .ToList<dynamic>();



                //List<tbExtrato> extratos = queryExtrato.ToList<tbExtrato>();
                int competenciaListaFinal = 0;
                //foreach (tbExtrato extrato in extratos)
                foreach (ConciliacaoRelatorios e in rMovimentacoesBancarias)
                {
                    //ConciliacaoRelatorios e = new ConciliacaoRelatorios();
                    //e.tipo = "E";
                    //e.valorBruto = extrato.vlMovimento != null ? extrato.vlMovimento.Value : new decimal(0.0);
                    //e.valorDescontado = new decimal(0.0);
                    //e.valorDescontadoAntecipacao = new decimal(0.0);
                    //e.valorLiquido = extrato.vlMovimento != null ? extrato.vlMovimento.Value : new decimal(0.0);
                    //e.competencia = extrato.dtExtrato;
                    //e.idExtrato = extrato.RecebimentoParcelas.Count + extrato.tbRecebimentoAjustes.Count;
                    //e.taxaCashFlow = new decimal(0.0);

                    List<ConciliacaoRelatorios.TotalPorBandeira> conciliadosPorBandeiras = _db.Database.SqlQuery<ConciliacaoRelatorios.TotalPorBandeira>(
                                                                             "SELECT T.nmAdquirente AS adquirente" + 
                                                                             ", T.dsBandeira AS bandeira" +
                                                                             ", T.dsTipo AS tipocartao" +
                                                                             ", SUM(T.total) as total" +
                                                                             ", SUM(T.valor) as valor" +
                                                                             " FROM (SELECT O.nmAdquirente, B.dsBandeira, B.dsTipo, COUNT(*) as total, SUM(ISNULL(P.valorParcelaLiquida, 0.0)) as valor" +
                                                                             " FROM pos.RecebimentoParcela P (nolock)" +
                                                                             " JOIN pos.Recebimento R ON R.id = P.idRecebimento" +
                                                                             " JOIN card.tbBandeira B ON B.cdBandeira = R.cdBandeira" +
                                                                             " JOIN card.tbAdquirente O ON O.cdAdquirente = B.cdAdquirente" +
                                                                             " WHERE P.idExtrato = " + e.idExtrato +
                                                                             " GROUP BY O.nmAdquirente, B.dsBandeira, B.dsTipo" +
                                                                             " UNION " +
                                                                             " SELECT O.nmAdquirente, B.dsBandeira, B.dsTipo, COUNT(*) as total, SUM(A.vlAjuste) as valor" +
                                                                             " FROM card.tbRecebimentoAjuste A (nolock)" +
                                                                             " JOIN card.tbBandeira B ON B.cdBandeira = A.cdBandeira" +
                                                                             " JOIN card.tbAdquirente O ON O.cdAdquirente = B.cdAdquirente" +
                                                                             " WHERE A.idExtrato = " + e.idExtrato +
                                                                             " GROUP BY O.nmAdquirente, B.dsBandeira, B.dsTipo) T" +
                                                                             " GROUP BY T.nmAdquirente, T.dsBandeira, T.dsTipo" +
                                                                             " ORDER BY T.nmAdquirente, T.dsBandeira, T.dsTipo"
                                                                            ).ToList<ConciliacaoRelatorios.TotalPorBandeira>();
                    if (conciliadosPorBandeiras.Count > 0)
                    {
                        e.idExtrato = conciliadosPorBandeiras.Sum(t => t.total);
                        if (e.adquirente.ToUpper().Equals("INDEFINIDA"))
                            e.adquirente = conciliadosPorBandeiras.Select(t => t.adquirente).First();
                    }
                    else
                    {
                        e.idExtrato = 0;
                        conciliadosPorBandeiras.Add(new ConciliacaoRelatorios.TotalPorBandeira
                        {
                            adquirente = e.adquirente,
                            bandeira = e.bandeira,
                            tipocartao = e.tipocartao,
                            total = 0,
                            valor = new decimal(0.0)
                        });
                    }
                    

                    bool extratoConciliado = e.idExtrato > 0;//extrato.RecebimentoParcelas.Count > 0 || extrato.tbRecebimentoAjustes.Count > 0;

                    #region COMPETENCIA
                    // Lista agrupada
                    var competencia = listaFinal[competenciaListaFinal];
                    //var item = listaFinal[competenciaListaFinal];
                    while (Convert.ToDateTime(competencia.competencia) < e.competencia && competenciaListaFinal < listaFinal.Count - 1)
                        competencia = listaFinal[++competenciaListaFinal];

                    // TEMP
                    //if (competenciaListaFinal > 2)
                    //    break;

                    bool competenciaExistente = Convert.ToDateTime(competencia.competencia) == e.competencia;

                    var competenciaAtualizada = new
                    {
                        competencia = e.competencia.ToShortDateString(),
                        taxaMedia = !competenciaExistente ? new decimal(0.0) : competencia.taxaMedia,
                        vendas = !competenciaExistente ? new decimal(0.0) : competencia.vendas,
                        valorDescontadoTaxaADM = !competenciaExistente ? new decimal(0.0) : competencia.valorDescontadoTaxaADM,
                        ajustesCredito = !competenciaExistente ? new decimal(0.0) : competencia.ajustesCredito,
                        ajustesDebito = !competenciaExistente ? new decimal(0.0) : competencia.ajustesDebito,
                        valorLiquido = !competenciaExistente ? new decimal(0.0) : competencia.valorLiquido,
                        valorDescontadoAntecipacao = !competenciaExistente ? new decimal(0.0) : competencia.valorDescontadoAntecipacao,
                        valorLiquidoTotal = !competenciaExistente ? new decimal(0.0) : competencia.valorLiquidoTotal,
                        extratoBancario = competenciaExistente ? competencia.extratoBancario + e.valorLiquido : e.valorLiquido,
                        diferenca = competenciaExistente ? Math.Abs(competencia.valorLiquidoTotal - competencia.extratoBancario - e.valorLiquido) : e.valorLiquido,
                        status = !competenciaExistente ? extratoConciliado ? "Conciliado" : "Não Conciliado" :
                                        competencia.status.Equals("Não conciliado") || !extratoConciliado ?
                                             Math.Abs(competencia.valorLiquidoTotal - competencia.extratoBancario - e.valorLiquido) <= TOLERANCIA_PRE_CONCILIADO ? "Pré-Conciliado" : "Não conciliado" :
                                            "Conciliado",

                        adquirentes = !competenciaExistente ? new List<dynamic>() : competencia.adquirentes
                    };


                    // Verifica se a data consta na lista
                    if (!competenciaExistente)
                    {
                        if (Convert.ToDateTime(competencia.competencia) < e.competencia)
                        {
                            if (competenciaListaFinal < listaFinal.Count - 1)
                                listaFinal.Insert(competenciaListaFinal + 1, competenciaAtualizada);
                            else
                                listaFinal.Add(competenciaAtualizada);
                        }
                        else
                            listaFinal.Insert(competenciaListaFinal, competenciaAtualizada);
                    }
                    else
                    {
                        // Remove a antiga e re-adiciona
                        if (listaFinal.Count > competenciaListaFinal)
                            listaFinal.RemoveAt(competenciaListaFinal);
                        listaFinal.Insert(competenciaListaFinal, competenciaAtualizada);
                    }

                    #endregion

                    #region ADQUIRENTE
                    int adquirenteListaFinal = 0;
                    // Procura a adquirente
                    var adquirente = competencia.adquirentes.Count > 0 ? competencia.adquirentes[adquirenteListaFinal] :
                                                new
                                                {
                                                    adquirente = e.adquirente,
                                                    taxaMedia = new decimal(0.0),
                                                    vendas = new decimal(0.0),
                                                    valorDescontadoTaxaADM = new decimal(0.0),
                                                    ajustesCredito = new decimal(0.0),
                                                    ajustesDebito = new decimal(0.0),
                                                    valorLiquido = new decimal(0.0),
                                                    valorDescontadoAntecipacao = new decimal(0.0),
                                                    valorLiquidoTotal = new decimal(0.0),
                                                    extratoBancario = new decimal(0.0),
                                                    diferenca = new decimal(0.0),
                                                    status = "Não conciliado",

                                                    bandeiras = new List<dynamic>()
                                                };

                    while (string.Compare(e.adquirente, adquirente.adquirente, StringComparison.Ordinal) > 0 && adquirenteListaFinal < competencia.adquirentes.Count - 1)
                        adquirente = competencia.adquirentes[++adquirenteListaFinal];


                    bool adquirenteExistente = adquirente.adquirente.Equals(e.adquirente);

                    var adquirenteAtualizada = new
                    {
                        adquirente = e.adquirente,
                        taxaMedia = !adquirenteExistente ? new decimal(0.0) : adquirente.taxaMedia,
                        vendas = !adquirenteExistente ? new decimal(0.0) : adquirente.vendas,
                        valorDescontadoTaxaADM = !adquirenteExistente ? new decimal(0.0) : adquirente.valorDescontadoTaxaADM,
                        ajustesCredito = !adquirenteExistente ? new decimal(0.0) : adquirente.ajustesCredito,
                        ajustesDebito = !adquirenteExistente ? new decimal(0.0) : adquirente.ajustesDebito,
                        valorLiquido = !adquirenteExistente ? new decimal(0.0) : adquirente.valorLiquido,
                        valorDescontadoAntecipacao = !adquirenteExistente ? new decimal(0.0) : adquirente.valorDescontadoAntecipacao,
                        valorLiquidoTotal = !adquirenteExistente ? new decimal(0.0) : adquirente.valorLiquidoTotal,
                        extratoBancario = adquirenteExistente ? adquirente.extratoBancario + e.valorLiquido : e.valorLiquido,
                        diferenca = adquirenteExistente ? Math.Abs(adquirente.valorLiquidoTotal - adquirente.extratoBancario - e.valorLiquido) : e.valorLiquido,
                        status = !adquirenteExistente ? extratoConciliado ? "Conciliado" : "Não Conciliado" :
                                        adquirente.status.Equals("Não conciliado") || !extratoConciliado ?
                                            Math.Abs(adquirente.valorLiquidoTotal - adquirente.extratoBancario - e.valorLiquido) <= TOLERANCIA_PRE_CONCILIADO ? "Pré-Conciliado" : "Não conciliado" :
                                            "Conciliado",

                        bandeiras = !adquirenteExistente ? new List<dynamic>() : adquirente.bandeiras
                    };


                    // Verifica se a adquirente consta na competência
                    if (!adquirenteExistente)
                    {
                        if (string.Compare(e.adquirente, adquirente.adquirente, StringComparison.Ordinal) > 0)
                        {
                            if (adquirenteListaFinal < competencia.adquirentes.Count - 1)
                                competencia.adquirentes.Insert(adquirenteListaFinal + 1, adquirenteAtualizada);
                            else
                                competencia.adquirentes.Add(adquirenteAtualizada);
                        }
                        else
                            competencia.adquirentes.Insert(adquirenteListaFinal, adquirenteAtualizada);
                    }
                    else
                    {
                        // Remove a antiga e re-adiciona
                        if (competencia.adquirentes.Count > adquirenteListaFinal)
                            competencia.adquirentes.RemoveAt(adquirenteListaFinal);
                        competencia.adquirentes.Insert(adquirenteListaFinal, adquirenteAtualizada);
                    }

                    #endregion

                    #region BANDEIRA
                    //decimal valorLiquidoRestante = e.valorLiquido;
                    decimal valorTotalConciliadoDeParcelas = conciliadosPorBandeiras.Sum(t => t.valor);
                    //foreach (var bd in bandeiras)
                    foreach (ConciliacaoRelatorios.TotalPorBandeira bd in conciliadosPorBandeiras)
                    {
                        decimal valorConciliadoExtratoParaBandeira = e.valorLiquido;

                        //if (bd.bandeira.EndsWith("VISA EL") && e.competencia.Month == 1 && (e.competencia.Day == 8 || e.competencia.Day == 18))
                        //{
                        //    valorConciliadoExtratoParaBandeira += new decimal(0.0);
                        //}

                        if (conciliadosPorBandeiras.Count > 1)
                        {
                            if (valorTotalConciliadoDeParcelas == new decimal(0.0))
                                valorConciliadoExtratoParaBandeira = bd.valor;
                            else
                            {
                                // Contribuição percentual do valor de parcelas da bandeira corrente em relação ao total utilizado para a conciliação com a movimentação bancária
                                decimal contribuicaoDaConciliacao = bd.valor / valorTotalConciliadoDeParcelas;
                                valorConciliadoExtratoParaBandeira = decimal.Round(contribuicaoDaConciliacao * e.valorLiquido, 3);
                            }
                        }

                        // Procura a bandeira
                        int bandeiraListaFinal = 0;
                        var band = adquirente.bandeiras.Count > 0 ? adquirente.bandeiras[bandeiraListaFinal] :
                                                    new
                                                    {
                                                        bandeira = bd.bandeira + (!bd.bandeira.ToUpper().Equals("INDEFINIDA") || bd.tipocartao.Trim().Equals("") ? "" : " (" + bd.tipocartao + ")"),
                                                        taxaMedia = new decimal(0.0),
                                                        vendas = new decimal(0.0),
                                                        valorDescontadoTaxaADM = new decimal(0.0),
                                                        ajustesCredito = new decimal(0.0),
                                                        ajustesDebito = new decimal(0.0),
                                                        valorLiquido = new decimal(0.0),
                                                        valorDescontadoAntecipacao = new decimal(0.0),
                                                        valorLiquidoTotal = new decimal(0.0),
                                                        extratoBancario = new decimal(0.0),
                                                        diferenca = new decimal(0.0),
                                                        status = "Não conciliado",
                                                    };

                        while (string.Compare(bd.bandeira, band.bandeira, StringComparison.Ordinal) > 0 && bandeiraListaFinal < adquirente.bandeiras.Count - 1)
                            band = adquirente.bandeiras[++bandeiraListaFinal];


                        bool bandeiraExistente = band.bandeira.Equals(bd.bandeira);
                        //bool extratoMaiorQueRecebiveis = valorLiquidoRestante > band.diferenca;

                        var bandeiraAtualizada = new
                        {
                            bandeira = bd.bandeira + (!bd.bandeira.ToUpper().Equals("INDEFINIDA") || bd.tipocartao.Trim().Equals("") ? "" : " (" + bd.tipocartao + ")"),
                            taxaMedia = !bandeiraExistente ? new decimal(0.0) : band.taxaMedia,
                            vendas = !bandeiraExistente ? new decimal(0.0) : band.vendas,
                            valorDescontadoTaxaADM = !bandeiraExistente ? new decimal(0.0) : band.valorDescontadoTaxaADM,
                            ajustesCredito = !bandeiraExistente ? new decimal(0.0) : band.ajustesCredito,
                            ajustesDebito = !bandeiraExistente ? new decimal(0.0) : band.ajustesDebito,
                            valorLiquido = !bandeiraExistente ? new decimal(0.0) : band.valorLiquido,
                            valorDescontadoAntecipacao = !bandeiraExistente ? new decimal(0.0) : band.valorDescontadoAntecipacao,
                            valorLiquidoTotal = !bandeiraExistente ? new decimal(0.0) : band.valorLiquidoTotal,
                            extratoBancario = !bandeiraExistente ? /*e.valorLiquido*/ valorConciliadoExtratoParaBandeira :
                                                    band.extratoBancario + valorConciliadoExtratoParaBandeira,
                            diferenca = !bandeiraExistente ? /*e.valorLiquido*/ valorConciliadoExtratoParaBandeira :
                                                Math.Abs(band.valorLiquidoTotal - band.extratoBancario - valorConciliadoExtratoParaBandeira),
                            status = !bandeiraExistente ? extratoConciliado ? "Conciliado" : "Não Conciliado" :
                                        !band.status.Equals("Não conciliado") && extratoConciliado ? "Conciliado" :
                                            Math.Abs(band.valorLiquidoTotal - band.extratoBancario - valorConciliadoExtratoParaBandeira) <= TOLERANCIA_PRE_CONCILIADO ? "Pré-Conciliado" : "Não conciliado",
                        };

                        // Decrementa valor usado
                        //if (bandeiras.Count > 1 && extratoMaiorQueRecebiveis)
                        //    valorLiquidoRestante -= band.diferenca;

                        // Verifica se a adquirente consta na competência
                        if (!bandeiraExistente)
                        {
                            if (string.Compare(bd.bandeira, band.bandeira, StringComparison.Ordinal) > 0)
                            {
                                if (bandeiraListaFinal < adquirente.bandeiras.Count - 1)
                                    adquirente.bandeiras.Insert(bandeiraListaFinal + 1, bandeiraAtualizada);
                                else
                                    adquirente.bandeiras.Add(bandeiraAtualizada);
                            }
                            else
                                adquirente.bandeiras.Insert(bandeiraListaFinal, bandeiraAtualizada);
                        }
                        else
                        {
                            // Remove a antiga e re-adiciona
                            if (adquirente.bandeiras.Count > bandeiraListaFinal)
                                adquirente.bandeiras.RemoveAt(bandeiraListaFinal);
                            adquirente.bandeiras.Insert(bandeiraListaFinal, bandeiraAtualizada);
                        }

                    }

                    #endregion
                }

                // Ordena
                CollectionConciliacaoRelatorios = listaFinal;

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionConciliacaoRelatorios.Count;

                // TOTAL
                /*
                totalRecebimento = CollectionConciliacaoRelatorios.Select(r => r.ValorTotalRecebimento).Cast<decimal>().Sum();
                totalExtrato = CollectionConciliacaoRelatorios.Select(r => r.ValorTotalExtrato).Cast<decimal>().Sum();
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valorExtrato", totalExtrato);
                retorno.Totais.Add("valorRecebimento", totalRecebimento);
                */

                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionConciliacaoRelatorios = CollectionConciliacaoRelatorios.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionConciliacaoRelatorios;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar adquirente" : erro);
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
