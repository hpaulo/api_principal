using api.Bibliotecas;
using api.Models;
using api.Models.Object;
using api.Negocios.Cliente;
using api.Negocios.Pos;
using api.Negocios.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Negocios.Card
{
    public class GatewayRecebiveisFuturos
    {

        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRecebiveisFuturos()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100,
            ID_GRUPO = 101,
            CDCONTACORRENTE = 102,
            CDADQUIRENTE = 103,
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
                List<dynamic> CollectionRecebiveisFuturos = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringAjustes = new Dictionary<string, string>();
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                Dictionary<string, string> queryStringTbAntecipacaoBancariaDetalhe = new Dictionary<string, string>();
                // DATA
                DateTime dataMin = Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString());
                queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, ""); // dtaRecebimentoEfetivo is null
                if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                {
                    // Não permite que o período seja inferior a data corrente + 1
                    string data = queryString["" + (int)CAMPOS.DATA];
                    if (data.Contains(">")){
                        DateTime dataInicial = DateTime.ParseExact(data.Replace(">", "") + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if(dataInicial < dataMin)
                            throw new Exception("Período deve ser igual ou superior ao dia seguinte da data corrente (" + dataMin.ToShortDateString() + ")");
                        //data = ">" + Convert.ToDateTime(dataNow.ToShortDateString());
                    }
                    else if (data.Contains("|"))
                    {
                        DateTime dataInicial = DateTime.ParseExact(data.Substring(0, data.IndexOf("|")) + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (dataInicial < dataMin)
                            //data = Convert.ToDateTime(dataNow.ToShortDateString()) + data.Substring(data.IndexOf("|"));
                            throw new Exception("Data inicial do período deve ser igual ou superior ao dia seguinte da data corrente (" + dataMin.ToShortDateString() + ")");
                        DateTime dataFinal = DateTime.ParseExact(data.Substring(data.IndexOf("|") + 1) + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (dataFinal < dataMin)
                            //data = data.Substring(0, data.IndexOf("|") + 1) + Convert.ToDateTime(dataNow.ToShortDateString());
                            throw new Exception("Data final do período deve ser igual ou superior ao dia seguinte da data corrente (" + dataMin.ToShortDateString() + ")");
                        if (dataInicial > dataFinal)
                            throw new Exception("Período informado é inválido!");
                    }
                    else
                    {
                        DateTime dataInicial = DateTime.ParseExact(data + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (dataInicial < dataMin)
                            //data = ">" + Convert.ToDateTime(dataNow.ToShortDateString());
                            throw new Exception("Data informada deve ser igual ou superior ao dia seguinte da data corrente (" + dataMin.ToShortDateString() + ")");
                    }
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, data);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, data);
                    queryStringTbAntecipacaoBancariaDetalhe.Add("" + (int)GatewayTbAntecipacaoBancariaDetalhe.CAMPOS.DTVENCIMENTO, data);
                }
                else
                {
                    // Período todo => começa a partir da data corrente + 1
                    string data = ">" + dataMin.ToShortDateString();
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, data);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, data);
                    queryStringTbAntecipacaoBancariaDetalhe.Add("" + (int)GatewayTbAntecipacaoBancariaDetalhe.CAMPOS.DTVENCIMENTO, data);
                }
                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringTbAntecipacaoBancariaDetalhe.Add("" + (int)GatewayTbAntecipacaoBancariaDetalhe.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de recebíveis futuros!");
                // FILIAL
                //string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                //if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                //    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                //if (!CnpjEmpresa.Equals(""))
                //{
                //    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ, CnpjEmpresa);
                //    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                //    //queryStringTbAntecipacaoBancariaDetalhe.Add("" + (int)GatewayTbAntecipacaoBancariaDetalhe.CAMPOS.NR, IdGrupo.ToString());
                //}

                // SOMENTE PARCELAS E AJUSTES DE BANDEIRAS TIPO CRÉDITO
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DSTIPO, "CRÉDITO");
                queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DSTIPO, "CRÉDITO");

                // Sem ajustes de antecipação
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.SEM_AJUSTES_ANTECIPACAO, true.ToString());

                // CONTA
                if (queryString.TryGetValue("" + (int)CAMPOS.CDCONTACORRENTE, out outValue))
                {
                    Int32 cdContaCorrente = Convert.ToInt32(queryString["" + (int)CAMPOS.CDCONTACORRENTE]);
                    if (cdContaCorrente > 0)
                    {
                        queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDCONTACORRENTE, cdContaCorrente.ToString());
                        queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDCONTACORRENTE, cdContaCorrente.ToString());
                        queryStringTbAntecipacaoBancariaDetalhe.Add("" + (int)GatewayTbAntecipacaoBancariaDetalhe.CAMPOS.CDCONTACORRENTE, cdContaCorrente.ToString());
                    }
                }

                // ADQUIRENTE
                if (queryString.TryGetValue("" + (int)CAMPOS.CDADQUIRENTE, out outValue))
                {
                    Int32 cdAdquirente = Convert.ToInt32(queryString["" + (int)CAMPOS.CDADQUIRENTE]);
                    if (cdAdquirente > 0)
                    {
                        queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDADQUIRENTE, cdAdquirente.ToString());
                        queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDADQUIRENTE, cdAdquirente.ToString());
                        queryStringTbAntecipacaoBancariaDetalhe.Add("" + (int)GatewayTbAntecipacaoBancariaDetalhe.CAMPOS.CDADQUIRENTE, cdAdquirente.ToString());
                    }
                }
                
                // Sem parcelas e/ou ajustes antecipados por causa do vencimento da antecipação bancária
                queryStringTbAntecipacaoBancariaDetalhe.Add("" + (int)GatewayTbAntecipacaoBancariaDetalhe.CAMPOS.SEM_PARCELAS_AJUSTES_ASSOCIADO, true.ToString());


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

                SimpleDataBaseQuery dataBaseQueryRP = GatewayRecebimentoParcela.getQuery((int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, queryStringRecebimentoParcela);
                SimpleDataBaseQuery dataBaseQueryAJ = GatewayTbRecebimentoAjuste.getQuery((int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, queryStringAjustes);
                SimpleDataBaseQuery dataBaseQueryAB = GatewayTbAntecipacaoBancariaDetalhe.getQuery((int)GatewayTbAntecipacaoBancariaDetalhe.CAMPOS.DTVENCIMENTO, 0, queryStringTbAntecipacaoBancariaDetalhe);

                #region Adiciona JOINS, caso não tenham

                // RECEBIMENTO PARCELA
                if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                    dataBaseQueryRP.join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimento");
                if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                    dataBaseQueryRP.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                    dataBaseQueryRP.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                //if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                //    dataBaseQueryRP.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                // AJUSTES
                if (!dataBaseQueryAJ.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                    dataBaseQueryAJ.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".cdBandeira");
                if (!dataBaseQueryAJ.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                    dataBaseQueryAJ.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                //if (!dataBaseQueryAJ.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                //    dataBaseQueryAJ.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".nrCNPJ = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                // ANTECIPACAO BANCÁRIA
                if (!dataBaseQueryAB.join.ContainsKey("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY))
                    dataBaseQueryAB.join.Add("INNER JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".idAntecipacaoBancaria = " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancaria");
                if (!dataBaseQueryAB.join.ContainsKey("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY))
                    dataBaseQueryAB.join.Add("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY, " ON " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe = " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe");
                if (!dataBaseQueryAB.join.ContainsKey("LEFT JOIN card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY))
                    dataBaseQueryAB.join.Add("LEFT JOIN card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY, " ON " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe = " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe");

                dataBaseQueryRP.readUncommited = true;
                dataBaseQueryAJ.readUncommited = true;
                dataBaseQueryAB.readUncommited = true;
                #endregion

                List<IDataRecord> resultado;
                List<RecebiveisFuturos> recebiveisFuturos = new List<RecebiveisFuturos>(); 
                if (colecao == 1)
                {
                    #region DIA
                    //if (!dataBaseQueryAB.join.ContainsKey("LEFT JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                    //    dataBaseQueryAB.join.Add("LEFT JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".cdBandeira");

                    #region OBTÉM OS SELECTS
                    // Obtém o select de cada um

                    // RECEBIMENTO PARCELA
                    dataBaseQueryRP.select = new string[] { GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento AS dtRecebimento",
                                                        //GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                        "vlParcela = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta)",
                                                        "vlDescontado = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorDescontado)",
                                                        "vlParcelaLiquida = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaLiquida)",
                                                        "vlAntecipacaoBancaria = 0"
                                                      };
                    dataBaseQueryRP.orderby = null;
                    dataBaseQueryRP.groupby = new[] { GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento",
                                                      //GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira"
                                                    };

                    // AJUSTE
                    dataBaseQueryAJ.select = new string[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste AS dtRecebimento",
                                                         //GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                        "vlParcela = SUM(CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste > 0.0 THEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste ELSE 0.0 END)",
                                                        "vlDescontado = SUM(CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste < 0.0 THEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste ELSE 0.0 END)",
                                                        "vlParcelaLiquida = SUM(" + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste)",
                                                        "vlAntecipacaoBancaria = 0"
                                                      };
                    dataBaseQueryAJ.orderby = null;
                    dataBaseQueryAJ.groupby = new[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste",
                                                     // GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira"
                                                    };

                    // ANTECIPAÇÃO BANCÁRIA
                    dataBaseQueryAB.select = new string[] { GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".dtVencimento AS dtRecebimento",
                                                        //"dsBandeira = ISNULL(" + GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira, 'INDEFINIDA')",
                                                        "vlParcela = 0",
                                                        "vlDescontado = 0",
                                                        "vlParcelaLiquida = 0",
                                                        "vlAntecipacaoBancaria = SUM(" + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".vlAntecipacao)",
                                                      };
                    dataBaseQueryAB.orderby = null;
                    dataBaseQueryAB.groupby = new[] { GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".dtVencimento",
                                                      //"ISNULL(" + GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira, 'INDEFINIDA')"
                                                    };

                    string scriptRP = dataBaseQueryRP.Script();
                    string scriptAJ = dataBaseQueryAJ.Script();
                    string scriptAB = dataBaseQueryAB.Script();

                    string script = "SELECT T.dtRecebimento" +
                                    //", T.dsBandeira" +
                                    ", vlParcela = SUM(T.vlParcela)" +
                                    ", vlDescontado = SUM(T.vlDescontado)" +
                                    ", vlParcelaLiquida = SUM(T.vlParcelaLiquida)" +
                                    ", vlAntecipacaoBancaria = SUM(T.vlAntecipacaoBancaria)" +
                                    " FROM (" + scriptRP + " UNION " + scriptAJ + " UNION " + scriptAB + ") T" +
                                    " GROUP BY T.dtRecebimento" +//, T.dsBandeira" +
                                    " ORDER BY T.dtRecebimento";//, T.dsBandeira";
                    #endregion

                    resultado = DataBaseQueries.SqlQuery(script, connection);

                    recebiveisFuturos = new List<RecebiveisFuturos>();
                    if (resultado != null && resultado.Count > 0)
                    {
                        recebiveisFuturos = resultado.Select(t => new RecebiveisFuturos
                        {
                            //bandeira = Convert.ToString(t["dsBandeira"]),
                            data = (DateTime)t["dtRecebimento"],
                            valorBruto = Convert.ToDecimal(t["vlParcela"]),
                            valorDescontado = Convert.ToDecimal(t["vlDescontado"]),
                            valorAntecipacaoBancaria = Convert.ToDecimal(t["vlAntecipacaoBancaria"]),
                            valorLiquido = Convert.ToDecimal(t["vlParcelaLiquida"]) - Convert.ToDecimal(t["vlAntecipacaoBancaria"]),
                        }).ToList<RecebiveisFuturos>();

                        CollectionRecebiveisFuturos = recebiveisFuturos.GroupBy(r => r.data)
                                                                       .Select(r => new
                                                                       {
                                                                           competencia = r.Key,
                                                                           valorBruto = r.Sum(f => f.valorBruto),
                                                                           valorDescontado = r.Sum(f => f.valorDescontado),
                                                                           valorAntecipacaoBancaria = r.Sum(f => f.valorAntecipacaoBancaria),
                                                                           valorLiquido = r.Sum(f => f.valorLiquido),
                                                                           /*bandeiras = r.GroupBy(f => f.bandeira)
                                                                                           .OrderBy(f => f.Key)
                                                                                           .Select(f => new
                                                                                           {
                                                                                               bandeira = f.Key,
                                                                                               valorBruto = f.Sum(x => x.valorBruto),
                                                                                               valorDescontado = f.Sum(x => x.valorDescontado),
                                                                                               valorAntecipacaoBancaria = f.Sum(x => x.valorAntecipacaoBancaria),
                                                                                               valorLiquido = f.Sum(x => x.valorLiquido),
                                                                                           }).ToList<dynamic>(),*/
                                                                       }).ToList<dynamic>();

                    }

                    #endregion
                }
                else
                {
                    #region COMPETENCIA / ADQUIRENTE / DIA
                    if (!dataBaseQueryAB.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        dataBaseQueryAB.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".cdAdquirente");
                

                    #region OBTÉM OS SELECTS
                    // Obtém o select de cada um

                    // RECEBIMENTO PARCELA
                    dataBaseQueryRP.select = new string[] { GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento AS dtRecebimento",
                                                        GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                        "vlParcela = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta)",
                                                        "vlDescontado = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorDescontado)",
                                                        "vlParcelaLiquida = SUM(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaLiquida)",
                                                        "vlAntecipacaoBancaria = 0"
                                                      };
                    dataBaseQueryRP.orderby = null;
                    dataBaseQueryRP.groupby = new[] { GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento",
                                                      GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente"
                                                    };

                    // AJUSTE
                    dataBaseQueryAJ.select = new string[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste AS dtRecebimento",
                                                        GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                        "vlParcela = SUM(CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste > 0.0 THEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste ELSE 0.0 END)",
                                                        "vlDescontado = SUM(CASE WHEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste < 0.0 THEN " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste ELSE 0.0 END)",
                                                        "vlParcelaLiquida = SUM(" + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste)",
                                                        "vlAntecipacaoBancaria = 0"
                                                      };
                    dataBaseQueryAJ.orderby = null;
                    dataBaseQueryAJ.groupby = new[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste",
                                                      GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente"
                                                    };

                    // ANTECIPAÇÃO BANCÁRIA
                    dataBaseQueryAB.select = new string[] { GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".dtVencimento AS dtRecebimento",
                                                        GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                        "vlParcela = 0",
                                                        "vlDescontado = 0",
                                                        "vlParcelaLiquida = 0",
                                                        "vlAntecipacaoBancaria = SUM(" + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".vlAntecipacao)",
                                                      };
                    dataBaseQueryAB.orderby = null;
                    dataBaseQueryAB.groupby = new[] { GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".dtVencimento",
                                                      GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente"
                                                    };


                    string scriptRP = dataBaseQueryRP.Script();
                    string scriptAJ = dataBaseQueryAJ.Script();
                    string scriptAB = dataBaseQueryAB.Script();

                    string script = "SELECT T.dtRecebimento" +
                        //", T.cdAdquirente" +
                                    ", T.nmAdquirente" +
                        //", T.cdBandeira" +
                        //", T.dsBandeira" +
                                    ", vlParcela = SUM(T.vlParcela)" +
                                    ", vlDescontado = SUM(T.vlDescontado)" +
                                    ", vlParcelaLiquida = SUM(T.vlParcelaLiquida)" +
                                    ", vlAntecipacaoBancaria = SUM(T.vlAntecipacaoBancaria)" +
                                    " FROM (" + scriptRP + " UNION " + scriptAJ + " UNION " + scriptAB + ") T" +
                                    " GROUP BY T.dtRecebimento, T.nmAdquirente" +//, T.dsBandeira" +
                                    " ORDER BY T.dtRecebimento, T.nmAdquirente";//, T.dsBandeira";
                    #endregion

                    resultado = DataBaseQueries.SqlQuery(script, connection);

                    recebiveisFuturos = new List<RecebiveisFuturos>();
                    if (resultado != null && resultado.Count > 0)
                    {
                        recebiveisFuturos = resultado.Select(t => new RecebiveisFuturos
                        {
                            adquirente = Convert.ToString(t["nmAdquirente"]),
                            //bandeira = Convert.ToString(t["dsBandeira"]),
                            data = (DateTime)t["dtRecebimento"],
                            valorBruto = Convert.ToDecimal(t["vlParcela"]),
                            valorDescontado = Convert.ToDecimal(t["vlDescontado"]),
                            valorAntecipacaoBancaria = Convert.ToDecimal(t["vlAntecipacaoBancaria"]),
                            valorLiquido = Convert.ToDecimal(t["vlParcelaLiquida"]) - Convert.ToDecimal(t["vlAntecipacaoBancaria"]),
                        }).ToList<RecebiveisFuturos>();

                        CollectionRecebiveisFuturos = recebiveisFuturos.GroupBy(r => new { r.data.Month, r.data.Year })
                                                                       .Select(r => new
                                                                       {
                                                                           competencia = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(r.Key.Month) + "/" + r.Key.Year,
                                                                           valorBruto = r.Sum(f => f.valorBruto),
                                                                           valorDescontado = r.Sum(f => f.valorDescontado),
                                                                           valorAntecipacaoBancaria = r.Sum(f => f.valorAntecipacaoBancaria),
                                                                           valorLiquido = r.Sum(f => f.valorLiquido),
                                                                           adquirentes = r.GroupBy(f => f.adquirente)
                                                                                           .OrderBy(f => f.Key)
                                                                                           .Select(f => new
                                                                                           {
                                                                                               adquirente = f.Key,
                                                                                               valorBruto = /*decimal.Round(*/f.Sum(x => x.valorBruto)/*, 2)*/,
                                                                                               valorDescontado = /*decimal.Round(*/f.Sum(x => x.valorDescontado)/*, 2)*/,
                                                                                               valorAntecipacaoBancaria = f.Sum(x => x.valorAntecipacaoBancaria),
                                                                                               valorLiquido = /*decimal.Round(*/f.Sum(x => x.valorLiquido)/*, 2)*/,
                                                                                               dias = f.GroupBy(x => x.data)
                                                                                                        .OrderBy(x => x.Key)
                                                                                                        .Select(x => new
                                                                                                        {
                                                                                                            data = x.Key,
                                                                                                            valorBruto = /*decimal.Round(*/x.Sum(y => y.valorBruto)/*, 2)*/,
                                                                                                            valorDescontado = /*decimal.Round(*/x.Sum(y => y.valorDescontado)/*, 2)*/,
                                                                                                            valorAntecipacaoBancaria = /*decimal.Round(*/x.Sum(y => y.valorAntecipacaoBancaria)/*, 2)*/,
                                                                                                            valorLiquido = x.Sum(y => y.valorLiquido),
                                                                                                        }).ToList<dynamic>(),
                                                                                           }).ToList<dynamic>(),
                                                                       }).ToList<dynamic>();

                    }
                    #endregion
                }

                try
                {
                    connection.Close();
                }
                catch { }


                //List<RecebiveisFuturos> ajustes = queryAjustes.Select(a => new RecebiveisFuturos
                //{
                //    data = a.dtAjuste,
                //    valorBruto = a.vlAjuste > new decimal(0.0) ? a.vlAjuste : new decimal(0.0),
                //    valorLiquido = a.vlAjuste,
                //    valorDescontado = a.vlAjuste < new decimal(0.0) ? new decimal(-1.0) * a.vlAjuste : new decimal(0.0),
                //    bandeira = a.tbBandeira.dsBandeira,
                //    adquirente = a.tbBandeira.tbAdquirente.nmAdquirente
                //}).OrderBy(r => r.data).ToList<RecebiveisFuturos>();

                //List<RecebiveisFuturos> recebiveisFuturos = queryRecebimentoParcela.Select(r => new RecebiveisFuturos
                //{
                //    data = r.dtaRecebimento,
                //    valorBruto = r.valorParcelaBruta,
                //    valorLiquido = r.valorParcelaLiquida != null ? r.valorParcelaLiquida.Value : new decimal(0.0),
                //    valorDescontado = r.valorDescontado,
                //    bandeira = r.Recebimento.cdBandeira != null ? r.Recebimento.tbBandeira.dsBandeira : r.Recebimento.BandeiraPos.desBandeira,
                //    adquirente = r.Recebimento.cdBandeira != null ? r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente : r.Recebimento.BandeiraPos.Operadora.nmOperadora
                //}).OrderBy(r => r.data).ToList<RecebiveisFuturos>();

                //if (ajustes.Count > 0) recebiveisFuturos = recebiveisFuturos.Concat(ajustes).OrderBy(r => r.data).ToList<RecebiveisFuturos>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebiveisFuturos.Count;

                // TOTAL
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valorBruto", CollectionRecebiveisFuturos.Select(r => r.valorBruto).Cast<decimal>().Sum());
                retorno.Totais.Add("valorDescontado", CollectionRecebiveisFuturos.Select(r => r.valorDescontado).Cast<decimal>().Sum());
                retorno.Totais.Add("valorAntecipacaoBancaria", CollectionRecebiveisFuturos.Select(r => r.valorAntecipacaoBancaria).Cast<decimal>().Sum());
                retorno.Totais.Add("valorLiquido", CollectionRecebiveisFuturos.Select(r => r.valorLiquido).Cast<decimal>().Sum());

                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionRecebiveisFuturos = CollectionRecebiveisFuturos.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionRecebiveisFuturos;

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
