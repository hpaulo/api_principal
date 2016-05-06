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
using System.Net.Http;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;
using System.Text;

namespace api.Negocios.Card
{
    public class GatewayCorrecaoVendaErp
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayCorrecaoVendaErp()
        {
            // _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            //IDEXTRATO = 100,
        };

        private readonly static string DOMINIO = System.Configuration.ConfigurationManager.AppSettings["DOMINIO"];



        // PUT  "vendas/corrigevendaserp"
        // JSON : { idsRecebimento [int] }      
        public static void CorrigeVenda(string token, CorrigeVendasErp param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;


            try
            {
                if (param == null)
                    throw new Exception("Nenhum recebimento foi informado!");

                //string outValue = null;

                string script = String.Empty;

                if (param.idsRecebimento == null || param.idsRecebimento.Count == 0)
                {
                    if (param.data == null)
                        throw new Exception("O período deve ser informado!");

                    // Obtém os recebíveis conciliados baseadas no filtro
                    DateTime dtIni, dtFim;

                    // Usa outros dados
                    if (param.data.Contains("|"))
                    {
                        string[] dts = param.data.Split('|');
                        dtIni = DateTime.ParseExact(dts[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        dtFim = DateTime.ParseExact(dts[1] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    }
                    else
                        dtIni = dtFim = DateTime.ParseExact(param.data + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);


                    script = "SELECT R.id" +
                            " FROM pos.Recebimento R (NOLOCK)" +
                            " JOIN card.tbBandeira B (NOLOCK) ON B.cdBandeira = R.cdBandeira" +
                            " JOIN cliente.empresa ER (NOLOCK) ON ER.nu_cnpj = R.cnpj" +
                            " JOIN card.tbRecebimentoVenda V (NOLOCK) ON V.idRecebimentoVenda = R.idRecebimentoVenda" +
                            //" LEFT JOIN card.tbBandeiraSacado BS on	BS.cdGrupo = ER.id_grupo and BS.cdBandeira = R.cdBandeira" +
                            " WHERE R.dtaVenda BETWEEN '" + DataBaseQueries.GetDate(dtIni) + "' AND '" + DataBaseQueries.GetDate(dtFim) + " 23:59:00'" +
                            (param.nrCNPJ != null ? " AND R.cnpj = '" + param.nrCNPJ + "'" : "") +
                            " AND V.dtAjuste IS NULL AND (" +
                            " CONVERT(VARCHAR(10), R.dtaVenda, 120) <> V.dtVenda" +
                            " OR (V.cdAdquirente IS NOT NULL AND R.cdSacado IS NOT NULL AND V.cdSacado IS NOT NULL AND V.cdSacado <> R.cdSacado)" +
                            " OR (R.numParcelaTotal IS NOT NULL AND V.qtParcelas <> R.numParcelaTotal)" +
                            " OR V.vlVenda <> R.valorVendaBruta" +
                            " OR (B.cdAdquirente NOT IN (5, 6, 11, 14) AND SUBSTRING('000000000000' + CONVERT(VARCHAR(12), R.nsu), LEN(R.nsu) + 1, 12) <> SUBSTRING('000000000000' + CONVERT(VARCHAR(12), V.nrNSU), LEN(V.nrNSU) + 1, 12))" +
                            ")";

                    // Obtém os recebíveis conciliados com divergência que respeitam o filtro
                    param.idsRecebimento = _db.Database.SqlQuery<int>(script).ToList();

                    if (param.idsRecebimento == null || param.idsRecebimento.Count == 0)
                    {
                        throw new Exception("Não há vendas a serem corrigadas " + (dtIni.Equals(dtFim) ? " em " + dtIni.ToShortDateString() : " no período de " + dtIni.ToShortDateString() + " a " + dtFim.ToShortDateString()) +
                                            (param.nrCNPJ != null ? " para a empresa " + param.nrCNPJ : "") + ".");
                    }

                }
                else
                {
                    #region DESCOBRE AS VENDAS QUE PRECISAM SER CORRIGIDAS
                    script = "SELECT R.id" +
                            " FROM pos.Recebimento R (NOLOCK)" +
                            " JOIN card.tbBandeira B (NOLOCK) ON B.cdBandeira = R.cdBandeira" +
                            " JOIN cliente.empresa ER (NOLOCK) ON ER.nu_cnpj = R.cnpj" +
                            " JOIN card.tbRecebimentoVenda V (NOLOCK) ON V.idRecebimentoVenda = R.idRecebimentoVenda" +
                            //" LEFT JOIN card.tbBandeiraSacado BS on	BS.cdGrupo = ER.id_grupo and BS.cdBandeira = R.cdBandeira" +
                            " WHERE R.id IN (" + string.Join(", ", param.idsRecebimento) + ")" +
                            " AND V.dtAjuste IS NULL AND (" +
                            " CONVERT(VARCHAR(10), R.dtaVenda, 120) <> V.dtVenda" +
                            " OR (V.cdAdquirente IS NOT NULL AND R.cdSacado IS NOT NULL AND V.cdSacado IS NOT NULL AND V.cdSacado <> R.cdSacado)" +
                            " OR (R.numParcelaTotal IS NOT NULL AND V.qtParcelas <> R.numParcelaTotal)" +
                            " OR V.vlVenda <> R.valorVendaBruta" +
                            // POLICARD, GETNET, SODEXO e VALECARD não trazem NSU do sitef
                            " OR (B.cdAdquirente NOT IN (5, 6, 11, 14) AND SUBSTRING('000000000000' + CONVERT(VARCHAR(12), R.nsu), LEN(R.nsu) + 1, 12) <> SUBSTRING('000000000000' + CONVERT(VARCHAR(12), V.nrNSU), LEN(V.nrNSU) + 1, 12))" +
                            ")";
                    param.idsRecebimento = _db.Database.SqlQuery<int>(script).ToList();
                    #endregion

                    if (param.idsRecebimento == null || param.idsRecebimento.Count == 0)
                    {
                        throw new Exception("Não há vendas a serem corrigadas!");
                    }
                }

                


                string idsRecebimento = string.Join(", ", param.idsRecebimento);

                int[] gruposRecebimentos = _db.Database.SqlQuery<int>("SELECT DISTINCT E.id_grupo" +
                                                                      " FROM pos.Recebimento R (NOLOCK)" +
                                                                      " JOIN cliente.empresa E (NOLOCK) ON E.nu_cnpj = R.cnpj" +
                                                                      " WHERE R.id IN (" + idsRecebimento + ")")
                                                            .ToArray();

                if (gruposRecebimentos == null || gruposRecebimentos.Length == 0)
                    throw new Exception(param.idsRecebimento.Count == 1 ? "Identificador de recebível inexistente!" : "Identificadores de recebíveis inexistentes!");

                //if (gruposRecebimentos.Length < param.idsRecebimento.Count)
                //    throw new Exception("Há " + (param.idsRecebimento.Count - gruposRecebimentos.Length) + ((param.idsRecebimento.Count - gruposRecebimentos.Length) == 1 ? " identificador de recebível inexistente!" : " identificadores de recebíveis inexistentes!"));


                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0)
                    throw new Exception("Um grupo deve ser selecionado como para a correção das vendas no ERP!");

                if (gruposRecebimentos.Any(t => t != IdGrupo))
                    throw new Exception("Permissão negada! " + (gruposRecebimentos.Length == 1 ? "Recebível informado não se refere" : "Recebíveis informados não se referem") + " ao grupo associado ao usuário.");

                grupo_empresa grupo_empresa = _db.Database.SqlQuery<grupo_empresa>("SELECT G.*" +
                                                                                   " FROM cliente.grupo_empresa G (NOLOCK)" +
                                                                                   " WHERE G.id_grupo = " + IdGrupo)
                                                          .FirstOrDefault();

                if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                    throw new Exception("Permissão negada! Empresa não possui o serviço ativo");


                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);
                
                List<IDataRecord> resultado;
                //List<int> idsReceb = new List<int>();
                try
                {
                    connection.Open();
                }
                catch
                {
                    throw new Exception("Não foi possível estabelecer conexão com a base de dados");
                }

                try
                {
                    #region AVALIA SE POSSUI ALGUM VENDA CONCILIADO COM MAIS DE UM RECEBÍVEL
                    List<int> idsRecebimentoVenda = _db.Database.SqlQuery<int>("SELECT R.idRecebimentoVenda" +
                                                                               " FROM pos.Recebimento R (NOLOCK)" +
                                                                               " WHERE R.idRecebimentoVenda IS NOT NULL" +
                                                                               " AND R.idRecebimentoVenda IN" +
                                                                               " (SELECT R.idRecebimentoVenda" +
                                                                               "  FROM pos.Recebimento R (NOLOCK)" +
                                                                               "  WHERE R.id IN (" + idsRecebimento + ")" +
                                                                               " )" +
                                                                               " GROUP BY R.idRecebimentoVenda" +
                                                                               " HAVING COUNT(*) > 1")
                                                                 .ToList();
                    if (idsRecebimentoVenda.Count > 0)
                    {
                        string error = "Há " + idsRecebimentoVenda.Count +
                                       (idsRecebimentoVenda.Count == 1 ? " venda que está conciliada" :
                                                                         " vendas que estão conciliadas")
                                      + " com mais de um recebível! Essa relação deve ser de um para um."
                                      + Environment.NewLine
                                      + (idsRecebimentoVenda.Count == 1 ? " Segue a venda e os correspondentes recebíveis conciliados com ele:" :
                                                                          " Seguem as vendas e os correspondentes recebíveis conciliados com cada uma delas")
                                      + Environment.NewLine;
                        // Reporta os vendas e as parcelas....
                        foreach (int idRecebimentoVenda in idsRecebimentoVenda)
                        {
                            // Obtém as informações da base
                            script = "SELECT R.dtaVenda AS R_dtVenda" +
                                            ", R.nsu AS R_nsu" +
                                            ", R.valorVendaBruta AS R_vlVenda" +
                                            ", R_filial = UPPER(ER.ds_fantasia + CASE WHEN ER.filial IS NULL THEN '' ELSE ' ' + ER.filial END)" +
                                            ", B.dsBandeira AS R_dsBandeira" +
                                            ", AAR.nmAdquirente AS R_nmAdquirente" +
                                            ", R.numParcelaTotal AS R_qtParcelas" +
                                            ", V.dtVenda AS V_dtVenda" +
                                            ", V.nrNSU AS V_nsu" +
                                            ", V.vlVenda AS V_vlVenda" +
                                            ", V_filial = UPPER(EV.ds_fantasia + CASE WHEN EV.filial IS NULL THEN '' ELSE ' ' + EV.filial END)" +
                                            ", V.dsBandeira AS V_dsBandeira" +
                                            ", AAV.nmAdquirente AS V_nmAdquirente" +
                                            ", V.qtParcelas AS V_qtParcelas" +
                                            " FROM pos.Recebimento R (NOLOCK)" +
                                            " JOIN cliente.empresa ER (NOLOCK) ON ER.nu_cnpj = R.cnpj" +
                                            " JOIN card.tbBandeira B (NOLOCK) ON B.cdBandeira = R.cdBandeira" +
                                            " JOIN card.tbAdquirente AAR (NOLOCK) ON AAR.cdAdquirente = B.cdAdquirente" +
                                            " JOIN card.tbRecebimentoVenda V (NOLOCK) ON V.idRecebimentoVenda = R.idRecebimentoVenda" +
                                            " JOIN cliente.empresa EV (NOLOCK) ON EV.nu_cnpj = V.nrCNPJ" +
                                            //" JOIN card.tbAdquirente AAV (NOLOCK) ON AAV.cdAdquirente = V.cdAdquirente" +
                                            " LEFT JOIN card.tbBandeiraSacado BS ON BS.cdSacado = V.cdSacado AND EV.id_grupo = BS.cdGrupo" +
                                            " JOIN card.tbBandeira BV ON BV.cdBandeira = BS.cdBandeira" +
                                            " JOIN card.tbAdquirente AAV ON AAV.cdAdquirente = BV.cdAdquirente" +
                                            " WHERE R.idRecebimentoVenda = " + idRecebimentoVenda;
                            resultado = DataBaseQueries.SqlQuery(script, connection);

                            error += Environment.NewLine + "==========VENDA=========";
                            if (resultado == null || resultado.Count == 0)
                                error += Environment.NewLine + " " + idRecebimentoVenda;
                            else
                            {
                                IDataRecord v = resultado[0];

                                DateTime V_dtVenda = (DateTime)v["V_dtVenda"];
                                string V_nsu = Convert.ToString(v["V_nsu"]);
                                decimal V_vlVenda = Convert.ToDecimal(v["V_vlVenda"]);
                                string V_filial = Convert.ToString(v["V_filial"]);
                                string V_bandeira = Convert.ToString(v["V_dsBandeira"].Equals(DBNull.Value) ? "" : v["V_dsBandeira"]);
                                string V_adquirente = Convert.ToString(v["V_nmAdquirente"]);
                                byte V_qtParcelas = Convert.ToByte(v["V_qtParcelas"].Equals(DBNull.Value) ? 0 : v["V_qtParcelas"]);

                                error += Environment.NewLine + "Adquirente: " + V_adquirente;
                                error += Environment.NewLine + "Bandeira: " + V_bandeira;
                                error += Environment.NewLine + "Filial: " + V_filial;
                                error += Environment.NewLine + "Venda em " + V_dtVenda.ToShortDateString() + " no valor de " + V_vlVenda.ToString("C");
                                error += Environment.NewLine + "NSU: " + V_nsu;
                                error += Environment.NewLine + "Parcelas: " + V_qtParcelas;

                                error += Environment.NewLine;


                                foreach (IDataRecord r in resultado)
                                {
                                    DateTime R_dtVenda = (DateTime)r["R_dtVenda"];
                                    string R_nsu = Convert.ToString(r["R_nsu"]);
                                    decimal R_vlVenda = Convert.ToDecimal(r["R_vlVenda"]);
                                    string R_filial = Convert.ToString(r["R_filial"]);
                                    string R_bandeira = Convert.ToString(r["R_dsBandeira"]);
                                    string R_adquirente = Convert.ToString(r["R_nmAdquirente"]);
                                    int R_qtParcelas = Convert.ToInt32(r["R_qtParcelas"].Equals(DBNull.Value) ? 1 : r["R_qtParcelas"]);

                                    error += Environment.NewLine + "=> RECEBÍVEL";
                                    error += Environment.NewLine + "   Adquirente: " + R_adquirente;
                                    error += Environment.NewLine + "   Bandeira: " + R_bandeira;
                                    error += Environment.NewLine + "   Filial: " + R_filial;
                                    error += Environment.NewLine + "   Venda em " + R_dtVenda.ToShortDateString() + " no valor de " + R_vlVenda.ToString("C");
                                    error += Environment.NewLine + "   NSU: " + R_nsu;
                                    error += Environment.NewLine + "   Parcelas: " + R_qtParcelas;

                                    error += Environment.NewLine;
                                }
                            }

                        }

                        throw new Exception(error);
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    if (e is DbEntityValidationException)
                    {
                        string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                        throw new Exception(erro.Equals("") ? "Falha ao listar realizar a correção das vendas" : erro);
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

                if(param.idsRecebimento == null || param.idsRecebimento.Count == 0)
                //if (idsReceb.Count == 0)
                    throw new Exception("Não há nenhuma venda para ser corrigida!");


                string url = "http://" + grupo_empresa.dsAPI + DOMINIO;
                //string url = "http://localhost:50939";
                string complemento = "vendas/corrigevendas/" + token;

                CorrigeVendaERP o = new CorrigeVendaERP(param.idsRecebimento);//idsReceb);

                HttpContent json = new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
                HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromMinutes(5); // 5 minutos de timeout
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                System.Net.Http.HttpResponseMessage response = client.PutAsync(complemento, json).Result;

                //se não retornar com sucesso busca os dados
                if (!response.IsSuccessStatusCode)
                {
                    string resp = String.Empty;
                    try
                    {
                        resp = response.Content.ReadAsAsync<string>().Result;
                    }
                    catch
                    {
                        throw new Exception("Serviço indisponível no momento");
                    }
                    if (resp != null && !resp.Trim().Equals(""))
                        throw new Exception(((int)response.StatusCode) + " - " + resp);
                    throw new Exception(((int)response.StatusCode) + "");
                }

                // Avalia se alguma venda teve que inserir dados
                script = "SELECT V.*" +
                         " FROM pos.Recebimento R (NOLOCK)" +
                         " JOIN card.tbRecebimentoVenda V ON R.idRecebimentoVenda = V.idRecebimentoVenda" +
                         " WHERE R.id IN (" + idsRecebimento + ")" +
                         " AND V.dsMensagem IS NOT NULL";
                try
                {
                    tbRecebimentoVenda[] vendas = _db.Database.SqlQuery<tbRecebimentoVenda>(script).ToArray();
                    if (vendas.Length > 0)
                    {
                        string result = String.Empty;
                        //foreach (tbRecebimentoVenda venda in vendas)
                        //{
                        //    result += "NSU: " + venda.nrNSU + Environment.NewLine +
                        //              (venda.cdSacado != null ? "Sacado : " + venda.cdSacado + Environment.NewLine : "") +
                        //              "Valor: " + venda.vlVenda.ToString("C") + Environment.NewLine +
                        //              "Parcelas: " + venda.qtParcelas + Environment.NewLine + 
                        //              "Mensagem: '" + venda.dsMensagem + "'" +
                        //              Environment.NewLine + Environment.NewLine;
                        //}
                        //throw new Exception("Vendas corrigidas que precisam ser corrigidas manualmente no sistema do cliente: " + 
                        //                    Environment.NewLine + Environment.NewLine + result);

                        if (vendas.Length == 1)
                            result = "Há 1 venda que precisa ser corrigida manualmente no sistema do cliente.";
                        else
                            result = "Há " + vendas.Length + " vendas que precisam ser corrigidas manualmente no sistema do cliente.";

                        result += Environment.NewLine + "Por favor, acesse a tela ADMINISTRATIVO > INTEGRAÇÃO ERP > VENDAS" +
                                                        " e seleciono como filtro de TIPO a opção CORREÇÃO MANUAL " +
                                                        " usando como filtro de data o mesmo período selecionado aqui na Conciliação de Vendas.";

                        throw new Exception(result);
                    }
                }
                catch(Exception e)
                {
                    if (e.Message.StartsWith("Há") && e.Message.Contains("Por favor, acesse a tela ADMINISTRATIVO > INTEGRAÇÃO ERP > VENDAS"))
                        throw new Exception(e.Message);
                    throw new Exception("Falha de comunicação com o servidor");
                }

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a correção das vendas" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}