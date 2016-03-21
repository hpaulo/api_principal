using api.Bibliotecas;
using api.Models;
using api.Models.Object;
using api.Negocios.Pos;
using api.Negocios.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Negocios.Card
{
    public class GatewayRelatorioVendas
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRelatorioVendas()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100, // da venda
            ID_GRUPO = 101,
            NU_CNPJ = 102,
            CDADQUIRENTE = 103,
        };


        /// <summary>
        /// Retorna a lista de conciliação bancária
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionRelatorioVendas = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringAjustes = new Dictionary<string, string>();
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                Dictionary<string, string> queryStringTbLogCarga = new Dictionary<string, string>();

                // DATA DA VENDA => OBRIGATÓRIO
                DateTime dataNow = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                DateTime dataInicial, dataFinal;
                if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                {
                    // Não permite que o período seja superior ou igual a data corrente
                    string data = queryString["" + (int)CAMPOS.DATA];
                    if (data.Contains("|"))
                    {
                        dataInicial = DateTime.ParseExact(data.Substring(0, data.IndexOf("|")) + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        //if (dataInicial >= dataNow)
                        //    throw new Exception("Data inicial do período de vendas deve ser inferior a data corrente (" + dataNow.ToShortDateString() + ")");
                        dataFinal = DateTime.ParseExact(data.Substring(data.IndexOf("|") + 1) + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        //if (dataFinal >= dataNow)
                        //    throw new Exception("Data final do período de vendas deve ser inferior a data corrente (" + dataNow.ToShortDateString() + ")");
                        if (dataInicial > dataFinal)
                            throw new Exception("Período de vendas informado é inválido!");
                    }
                    else if(data.Length == 6)
                    {    
                        dataInicial = DateTime.ParseExact(data + "01" + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        dataFinal = Convert.ToDateTime(DateTime.DaysInMonth(dataInicial.Year, dataInicial.Month) + "/" + dataInicial.Month + "/" + dataInicial.Year);
                    }
                    else
                    {
                        dataFinal = dataInicial = DateTime.ParseExact(data + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    }
                    //else
                    //{
                    //    DateTime dataVenda = DateTime.ParseExact(data + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    //    if (dataVenda >= dataNow)
                    //        throw new Exception("Data da venda informada deve ser inferior a data corrente (" + dataNow.ToShortDateString() + ")");
                    //}
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTAVENDA, data);
                    queryStringTbLogCarga.Add("" + (int)GatewayTbLogCarga.CAMPOS.DTCOMPETENCIA, data);
                }
                else throw new Exception("Data ou período de vendas deve ser informado!");

                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringTbLogCarga.Add("" + (int)GatewayTbLogCarga.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de recebíveis futuros!");
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ, CnpjEmpresa);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    queryStringTbLogCarga.Add("" + (int)GatewayTbLogCarga.CAMPOS.NRCNPJ, CnpjEmpresa);
                }
                // ADQUIRENTE
                if (queryString.TryGetValue("" + (int)CAMPOS.CDADQUIRENTE, out outValue))
                {
                    string cdAdquirente = queryString["" + (int)CAMPOS.CDADQUIRENTE];
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDADQUIRENTE, cdAdquirente);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDADQUIRENTE, cdAdquirente);
                    queryStringTbLogCarga.Add("" + (int)GatewayTbLogCarga.CAMPOS.CDADQUIRENTE, cdAdquirente);
                }


                // OBTÉM A QUERY
                //IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(_db, 0, (int)GatewayRecebimentoParcela.CAMPOS.DTAVENDA, 0, 0, 0, queryStringRecebimentoParcela);

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

                #region OBTÉM AS QUERIES
                SimpleDataBaseQuery dataBaseQueryRP = GatewayRecebimentoParcela.getQuery((int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, queryStringRecebimentoParcela);
                SimpleDataBaseQuery dataBaseQueryLC = GatewayTbLogCarga.getQuery((int)GatewayTbLogCarga.CAMPOS.DTCOMPETENCIA, 0, queryStringTbLogCarga);

                // RECEBIMENTO PARCELA
                if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                    dataBaseQueryRP.join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimento");
                if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                    dataBaseQueryRP.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                    dataBaseQueryRP.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                //if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                //    dataBaseQueryRP.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");


                // TBLOGCARGA
                if (!dataBaseQueryLC.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                    dataBaseQueryLC.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbLogCarga.SIGLA_QUERY + ".cdAdquirente");
                

                // RECEBIMENTO PARCELA
                dataBaseQueryRP.select = new string[] { GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento",
                                                      GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo",
                                                      GatewayRecebimento.SIGLA_QUERY + ".dtaVenda AS dataVenda",
                                                      "adquirente = UPPER(" + GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente)",
                                                      GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira AS bandeira",
                                                      "valorBruto = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta",
                                                      "valorDescontado = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorDescontado",
                                                      "valorLiquido = ISNULL(" + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaLiquida, 0)"
                                                    };


                // TBLOGCARGA
                dataBaseQueryLC.select = new string[] { GatewayTbLogCarga.SIGLA_QUERY + ".dtCompetencia",
                                                        "adquirente = UPPER(" + GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente)",
                                                        "processouVendas = CASE WHEN SUM(CASE WHEN " + GatewayTbLogCarga.SIGLA_QUERY + ".flStatusVendasCredito = 0 OR " + GatewayTbLogCarga.SIGLA_QUERY + ".flStatusVendasDebito = 0 THEN 1 ELSE 0 END) > 0 THEN 0 ELSE 1 END",
                                                        "valorSite = SUM(" + GatewayTbLogCarga.SIGLA_QUERY + ".vlVendaCredito + " + GatewayTbLogCarga.SIGLA_QUERY + ".vlVendaDebito)"
                                                      };

                dataBaseQueryLC.groupby = new string[] { GatewayTbLogCarga.SIGLA_QUERY + ".dtCompetencia", 
                                                         GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente"
                                                       };


                dataBaseQueryRP.readUncommited = true;
                dataBaseQueryLC.readUncommited = true;
                #endregion

                string script = dataBaseQueryRP.Script();
                List<IDataRecord> resultado = DataBaseQueries.SqlQuery(script, connection);

                List<RelatorioVendas> vendas = new List<RelatorioVendas>();
                if (resultado != null && resultado.Count > 0)
                {
                    vendas = resultado.Select(r => new RelatorioVendas
                    {
                        dataVenda = Convert.ToDateTime(((DateTime)r["dataVenda"]).ToShortDateString()), // remove horário
                        dataRecebimento = r["dtaRecebimentoEfetivo"].Equals(DBNull.Value) ? (DateTime)r["dtaRecebimento"] : (DateTime)r["dtaRecebimentoEfetivo"],
                        recebeu = r["dtaRecebimentoEfetivo"].Equals(DBNull.Value) ? (DateTime)r["dtaRecebimento"] < dataNow : (DateTime)r["dtaRecebimentoEfetivo"] < dataNow,
                        valorBruto = Convert.ToDecimal(r["valorBruto"]),
                        valorLiquido = Convert.ToDecimal(r["valorLiquido"]),
                        valorDescontado = Convert.ToDecimal(r["valorDescontado"]),
                        bandeira = Convert.ToString(r["bandeira"]),
                        adquirente = Convert.ToString(r["adquirente"]),
                    }).OrderBy(r => r.dataVenda).ToList<RelatorioVendas>();
                }

                // Obtém os valores lidos do site
                script = dataBaseQueryLC.Script();
                resultado = DataBaseQueries.SqlQuery(script, connection);
                List<LogCargaValorSite> valoresSite = new List<LogCargaValorSite>();
                if (resultado != null && resultado.Count > 0)
                {
                    valoresSite = resultado.Select(t => new LogCargaValorSite
                                           {
                                               //nrCNPJ = Convert.ToString(t["nrCNPJ"]),
                                               adquirente = Convert.ToString(t["adquirente"]),
                                               dtCompetencia = Convert.ToDateTime(((DateTime)t["dtCompetencia"]).ToShortDateString()),
                                               processouModalidade = Convert.ToBoolean(t["processouVendas"]),
                                               valorSite = Convert.ToDecimal(t["valorSite"])
                                           })
                                           .ToList<LogCargaValorSite>();
                }

                //List<RelatorioVendas> vendas = queryRecebimentoParcela.Select(r => new RelatorioVendas
                //{
                //    dataVenda = r.Recebimento.dtaVenda,
                //    dataRecebimento = r.dtaRecebimentoEfetivo != null ? r.dtaRecebimentoEfetivo.Value : r.dtaRecebimento,
                //    recebeu = (r.dtaRecebimentoEfetivo != null && r.dtaRecebimentoEfetivo.Value < dataNow) || (r.dtaRecebimentoEfetivo == null && r.dtaRecebimento < dataNow),
                //    valorBruto = r.valorParcelaBruta,
                //    valorLiquido = r.valorParcelaLiquida != null ? r.valorParcelaLiquida.Value : new decimal(0.0),
                //    valorDescontado = r.valorDescontado,
                //    bandeira = r.Recebimento.cdBandeira != null ? r.Recebimento.tbBandeira.dsBandeira : r.Recebimento.BandeiraPos.desBandeira,
                //    adquirente = r.Recebimento.cdBandeira != null ? r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente : r.Recebimento.BandeiraPos.Operadora.nmOperadora
                //}).OrderBy(r => r.dataVenda).ToList<RelatorioVendas>();

                transaction.Commit();

                List<dynamic> vendasAgrupadas = vendas
                                                .GroupBy(r => r.dataVenda)
                                                .Select(r => new
                                                {
                                                    data = r.Key,
                                                    valorSite = valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                                                                           .Where(t => r.GroupBy(f => f.adquirente).Select(f => f.Key).Contains(t.adquirente))
                                                                           .Sum(t => t.valorSite),
                                                    processouVenda = valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                                                                                .Where(t => r.GroupBy(f => f.adquirente).Select(f => f.Key).Contains(t.adquirente))
                                                                                .Count() > 0 && 
                                                                     valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                                                                                .Where(t => r.GroupBy(f => f.adquirente).Select(f => f.Key).Contains(t.adquirente))
                                                                                .Where(t => !t.processouModalidade)
                                                                                .Count() == 0,
                                                    valorBruto = r.Sum(f => f.valorBruto),
                                                    valorDescontado = r.Sum(f => f.valorDescontado),
                                                    valorLiquido = r.Sum(f => f.valorLiquido),
                                                    valorRecebido = r.Where(f => f.recebeu == true).Sum(f => f.valorLiquido),
                                                    valorAReceber = r.Where(f => f.recebeu == false).Sum(f => f.valorLiquido),
                                                    adquirentes = r.GroupBy(f => f.adquirente)
                                                    .OrderBy(f => f.Key)
                                                    .Select(f => new
                                                    {
                                                        adquirente = f.Key,
                                                        valorSite = valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                                                                               .Where(t => t.adquirente.Equals(f.Key))
                                                                               .Sum(t => t.valorSite),
                                                        processouVenda = valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                                                                                    .Where(t => t.adquirente.Equals(f.Key))
                                                                                    .Count() > 0 && 
                                                                         valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                                                                                    .Where(t => t.adquirente.Equals(f.Key))
                                                                                    .Where(t => !t.processouModalidade)
                                                                                    .Count() == 0,
                                                        valorBruto = f.Sum(x => x.valorBruto),
                                                        valorDescontado = f.Sum(x => x.valorDescontado),
                                                        valorLiquido = f.Sum(x => x.valorLiquido),
                                                        valorRecebido = f.Where(x => x.recebeu == true).Sum(x => x.valorLiquido),
                                                        valorAReceber = f.Where(x => x.recebeu == false).Sum(x => x.valorLiquido),
                                                        bandeiras = f.GroupBy(x => x.bandeira)
                                                        .OrderBy(x => x.Key)
                                                        .Select(x => new
                                                        {
                                                            bandeira = x.Key,
                                                            valorSite = new decimal(0.0),
                                                            valorBruto = x.Sum(y => y.valorBruto),
                                                            valorDescontado = x.Sum(y => y.valorDescontado),
                                                            valorLiquido = x.Sum(y => y.valorLiquido),
                                                            valorRecebido = x.Where(y => y.recebeu == true).Sum(y => y.valorLiquido),
                                                            valorAReceber = x.Where(y => y.recebeu == false).Sum(y => y.valorLiquido),
                                                        }).ToList<dynamic>(),
                                                    }).ToList<dynamic>(),
                                                }).ToList<dynamic>();

                for (DateTime dt = dataInicial; dt <= dataFinal; dt = dt.AddDays(1))
                {
                    var v = vendasAgrupadas.Where(t => t.data.Equals(dt)).FirstOrDefault();
                    var s = valoresSite.Where(t => t.dtCompetencia.Equals(dt)).FirstOrDefault();

                    if (v != null)
                    {
                        CollectionRelatorioVendas.Add(new
                        {
                            diaVenda = dt.ToShortDateString(),
                            valorSite = v.valorSite,
                            processouVenda = v.processouVenda,
                            valorBruto = v.valorBruto,
                            valorDescontado = v.valorDescontado,
                            valorLiquido = v.valorLiquido,
                            valorRecebido = v.valorRecebido,
                            valorAReceber = v.valorRecebido,
                            adquirentes = v.adquirentes
                        });
                    }
                    else if (s != null)
                    {
                        CollectionRelatorioVendas.Add(new
                        {
                            diaVenda = dt.ToShortDateString(),
                            valorSite = valoresSite.Where(t => t.dtCompetencia.Equals(dt))
                                                   .Sum(t => t.valorSite),
                            processouVenda = valoresSite.Where(t => t.dtCompetencia.Equals(dt))
                                                        .Count() > 0 &&
                                             valoresSite.Where(t => t.dtCompetencia.Equals(dt))
                                                        .Where(t => !t.processouModalidade)
                                                        .Count() == 0,
                            valorBruto = new decimal(0.0),
                            valorDescontado = new decimal(0.0),
                            valorLiquido = new decimal(0.0),
                            valorRecebido = new decimal(0.0),
                            valorAReceber = new decimal(0.0),
                            adquirentes = valoresSite.Where(f => f.dtCompetencia.Equals(dt))
                                                     .GroupBy(f => f.adquirente)
                                                     .Select(f => new
                                                     {
                                                         adquirente = f.Key,
                                                         valorSite = f.Sum(t => t.valorSite),
                                                         processouVenda = f.Count() > 0 && f.Where(t => !t.processouModalidade).Count() == 0,
                                                         valorBruto = new decimal(0.0),
                                                         valorDescontado = new decimal(0.0),
                                                         valorLiquido = new decimal(0.0),
                                                         valorRecebido = new decimal(0.0),
                                                         valorAReceber = new decimal(0.0),
                                                         bandeiras = new List<dynamic>()
                                                     })
                                                     .ToList<dynamic>()
                        });
                    }
                }

                //CollectionRelatorioVendas = vendas//.GroupBy(r => new { r.dataVenda.Day, r.dataVenda.Month, r.dataVenda.Year })
                //                                .GroupBy(r => r.dataVenda)
                //                                .Select(r => new
                //                                {
                //                                    //diaVenda = (r.Key.Day < 10 ? "0" : "") + r.Key.Day + "/" + (r.Key.Month < 10 ? "0" : "") + r.Key.Month + "/" + r.Key.Year,
                //                                    diaVenda = r.Key.ToShortDateString(),
                //                                    valorSite = valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                //                                                           .Where(t => r.GroupBy(f => f.adquirente).Select(f => f.Key).Contains(t.adquirente))
                //                                                           .Sum(t => t.valorSite),
                //                                    processouVenda = valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                //                                                                .Where(t => r.GroupBy(f => f.adquirente).Select(f => f.Key).Contains(t.adquirente))
                //                                                                .Count() > 0 && 
                //                                                     valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                //                                                                .Where(t => r.GroupBy(f => f.adquirente).Select(f => f.Key).Contains(t.adquirente))
                //                                                                .Where(t => !t.processouModalidade)
                //                                                                .Count() == 0,
                //                                    valorBruto = r.Sum(f => f.valorBruto),
                //                                    valorDescontado = r.Sum(f => f.valorDescontado),
                //                                    valorLiquido = r.Sum(f => f.valorLiquido),
                //                                    valorRecebido = r.Where(f => f.recebeu == true).Sum(f => f.valorLiquido),
                //                                    valorAReceber = r.Where(f => f.recebeu == false).Sum(f => f.valorLiquido),
                //                                    adquirentes = r.GroupBy(f => f.adquirente)
                //                                    .OrderBy(f => f.Key)
                //                                    .Select(f => new
                //                                    {
                //                                        adquirente = f.Key,
                //                                        valorSite = valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                //                                                               .Where(t => t.adquirente.Equals(f.Key))
                //                                                               .Sum(t => t.valorSite),
                //                                        processouVenda = valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                //                                                                    .Where(t => t.adquirente.Equals(f.Key))
                //                                                                    .Count() > 0 && 
                //                                                         valoresSite.Where(t => t.dtCompetencia.Equals(r.Key))
                //                                                                    .Where(t => t.adquirente.Equals(f.Key))
                //                                                                    .Where(t => !t.processouModalidade)
                //                                                                    .Count() == 0,
                //                                        valorBruto = f.Sum(x => x.valorBruto),
                //                                        valorDescontado = f.Sum(x => x.valorDescontado),
                //                                        valorLiquido = f.Sum(x => x.valorLiquido),
                //                                        valorRecebido = f.Where(x => x.recebeu == true).Sum(x => x.valorLiquido),
                //                                        valorAReceber = f.Where(x => x.recebeu == false).Sum(x => x.valorLiquido),
                //                                        bandeiras = f.GroupBy(x => x.bandeira)
                //                                        .OrderBy(x => x.Key)
                //                                        .Select(x => new
                //                                        {
                //                                            bandeira = x.Key,
                //                                            valorSite = new decimal(0.0),
                //                                            valorBruto = x.Sum(y => y.valorBruto),
                //                                            valorDescontado = x.Sum(y => y.valorDescontado),
                //                                            valorLiquido = x.Sum(y => y.valorLiquido),
                //                                            valorRecebido = x.Where(y => y.recebeu == true).Sum(y => y.valorLiquido),
                //                                            valorAReceber = x.Where(y => y.recebeu == false).Sum(y => y.valorLiquido),
                //                                        }).ToList<dynamic>(),
                //                                    }).ToList<dynamic>(),
                //                                }).ToList<dynamic>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRelatorioVendas.Count;

                // TOTAL
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valorSite", CollectionRelatorioVendas.Select(r => r.valorSite).Cast<decimal>().Sum());
                retorno.Totais.Add("valorBruto", CollectionRelatorioVendas.Select(r => r.valorBruto).Cast<decimal>().Sum());
                retorno.Totais.Add("valorDescontado", CollectionRelatorioVendas.Select(r => r.valorDescontado).Cast<decimal>().Sum());
                retorno.Totais.Add("valorLiquido", CollectionRelatorioVendas.Select(r => r.valorLiquido).Cast<decimal>().Sum());
                retorno.Totais.Add("valorRecebido", CollectionRelatorioVendas.Select(r => r.valorRecebido).Cast<decimal>().Sum());
                retorno.Totais.Add("valorAReceber", CollectionRelatorioVendas.Select(r => r.valorAReceber).Cast<decimal>().Sum());
         
                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionRelatorioVendas = CollectionRelatorioVendas.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionRelatorioVendas;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
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
