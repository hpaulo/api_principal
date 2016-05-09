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

namespace api.Negocios.Card
{
    public class GatewayBaixaAutomaticaERP
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayBaixaAutomaticaERP()
        {
           // _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDEXTRATO = 100,
            ID_GRUPO = 101,
        };

        private readonly static string DOMINIO = System.Configuration.ConfigurationManager.AppSettings["DOMINIO"];

        // GET "titulos/baixaautomatica"      
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                //DECLARAÇÕES
                Retorno retorno = new Retorno();
                string outValue = null;

                // ID EXTRATO
                Int32 idExtrato = -1;
                if(!queryString.TryGetValue("" + (int)CAMPOS.IDEXTRATO, out outValue))
                    throw new Exception("O identificador da movimentação bancária deve ser informada para a baixa automática!");

                idExtrato = Convert.ToInt32(queryString["" + (int)CAMPOS.IDEXTRATO]);
                int? cdGrupo = _db.Database.SqlQuery<int>("SELECT C.cdGrupo" +
                                                                       " FROM card.tbExtrato E (NOLOCK)" +
                                                                       " JOIN card.tbContaCorrente C (NOLOCK) ON C.cdContaCorrente = E.cdContaCorrente" +
                                                                       " WHERE E.idExtrato = " + idExtrato)
                                                          .FirstOrDefault();
                if (cdGrupo == null)
                    throw new Exception("Extrato inexistente!");

                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo == 0) throw new Exception("Um grupo deve ser selecionado como para a baixa automática!");

                if(cdGrupo.Value != IdGrupo)
                    throw new Exception("Permissão negada! Movimentação bancária informada não se refere ao grupo associado ao usuário.");

                grupo_empresa grupo_empresa = _db.Database.SqlQuery<grupo_empresa>("SELECT G.*" +
                                                                                   " FROM cliente.grupo_empresa G (NOLOCK)" +
                                                                                   " WHERE G.id_grupo = " + IdGrupo)
                                                          .FirstOrDefault();

                if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                    throw new Exception("Permissão negada! Empresa não possui o serviço ativo");


                #region AVALIA SE POSSUI ALGUM TÍTULO CONCILIADO COM MAIS DE UM RECEBÍVEL
                List<int> idsRecebimentoTitulo = _db.Database.SqlQuery<int>("SELECT P.idRecebimentoTitulo" +
                                                                            " FROM pos.RecebimentoParcela P (NOLOCK)" +
                                                                            " WHERE P.idRecebimentoTitulo IS NOT NULL" +
                                                                            " AND P.idRecebimentoTitulo IN" +
                                                                            " (SELECT P.idRecebimentoTitulo" +
                                                                            "  FROM pos.RecebimentoParcela P (NOLOCK)" +
                                                                            "  WHERE P.idExtrato = " + idExtrato +
                                                                            " )" +
                                                                            " GROUP BY P.idRecebimentoTitulo" +
                                                                            " HAVING COUNT(*) > 1")
                                                             .ToList();
                if (idsRecebimentoTitulo.Count > 0)
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

                    try
                    {
                        string error = "Há " + idsRecebimentoTitulo.Count +
                                       (idsRecebimentoTitulo.Count == 1 ? " título que está conciliado" :
                                                                          " títulos que estão conciliados")
                                      + " com mais de um recebível! Essa relação deve ser de um para um."
                                      + Environment.NewLine
                                      + (idsRecebimentoTitulo.Count == 1 ? " Segue o título e as correspondentes parcelas conciliadas com ele:" :
                                                                          " Seguem os títulos e as correspondentes parcelas conciliadas com cada um deles")
                                      + Environment.NewLine;
                        // Reporta os títulos e as parcelas....
                        foreach (int idRecebimentoTitulo in idsRecebimentoTitulo)
                        {
                            // Obtém as informações da base
                            string script = "SELECT R.dtaVenda AS P_dtVenda" +
                                            ", R.nsu AS P_nsu" +
                                            ", R.valorVendaBruta AS P_vlVenda" +
                                            ", P_filial = UPPER(ER.ds_fantasia + CASE WHEN ER.filial IS NULL THEN '' ELSE ' ' + ER.filial END)" +
                                            ", B.dsBandeira AS P_dsBandeira" +
                                            ", AAR.nmAdquirente AS P_nmAdquirente" +
                                            ", R.numParcelaTotal AS P_qtParcelas" +
                                            ", P.numParcela AS P_nrParcela" +
                                            ", P.dtaRecebimento AS P_dtRecebimentoPrevisto" +
                                            ", P.dtaRecebimentoEfetivo AS P_dtRecebimentoEfetivo" +
                                            ", P.flAntecipado AS P_flAntecipado" +
                                            ", P.valorParcelaBruta AS P_vlParcela" +
                                            ", T.dtVenda AS T_dtVenda" +
                                            ", T.nrNSU AS T_nsu" +
                                            ", T.vlVenda AS T_vlVenda" +
                                            ", T_filial = UPPER(ET.ds_fantasia + CASE WHEN ET.filial IS NULL THEN '' ELSE ' ' + ET.filial END)" +
                                            ", T.dsBandeira AS T_dsBandeira" +
                                            //", AAT.nmAdquirente AS T_nmAdquirente" +
                                            ", T_nmAdquirente = (SELECT TOP 1 nmAdquirente FROM card.tbAdquirente (NOLOCK) WHERE cdAdquirente = CASE WHEN T.cdAdquirente IS NOT NULL THEN T.cdAdquirente ELSE T.cdAdquirenteNew END)" +
                                            ", T.qtParcelas AS T_qtParcelas" +
                                            ", T.nrParcela AS T_nrParcela" +
                                            ", T.dtTitulo AS T_dtRecebimentoPrevisto" +
                                            ", T.vlParcela AS T_vlParcela" +
                                            " FROM pos.RecebimentoParcela P (NOLOCK)" +
                                            " JOIN pos.Recebimento R (NOLOCK) ON R.id = P.idRecebimento" +
                                            " JOIN cliente.empresa ER (NOLOCK) ON ER.nu_cnpj = R.cnpj" +
                                            " JOIN card.tbBandeira B (NOLOCK) ON B.cdBandeira = R.cdBandeira" +
                                            " JOIN card.tbAdquirente AAR (NOLOCK) ON AAR.cdAdquirente = B.cdAdquirente" +
                                            " JOIN card.tbRecebimentoTitulo T (NOLOCK) ON T.idRecebimentoTitulo = P.idRecebimentoTitulo" +
                                            " JOIN cliente.empresa ET (NOLOCK) ON ET.nu_cnpj = T.nrCNPJ" +
                                            //" JOIN card.tbAdquirente AAT (NOLOCK) ON AAT.cdAdquirente = T.cdAdquirente" +
                                            " WHERE P.idRecebimentoTitulo = " + idRecebimentoTitulo;
                            List<IDataRecord> resultado = DataBaseQueries.SqlQuery(script, connection);

                            error += Environment.NewLine + "==========TÍTULO=========";
                            if (resultado == null || resultado.Count == 0)
                                error += Environment.NewLine + " " + idRecebimentoTitulo;
                            else
                            {
                                IDataRecord t = resultado[0];

                                DateTime? T_dtVenda = t["T_dtVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["T_dtVenda"];
                                string T_nsu = Convert.ToString(t["T_nsu"]);
                                decimal? T_vlVenda = t["T_vlVenda"].Equals(DBNull.Value) ? (decimal?)null : Convert.ToDecimal(t["T_vlVenda"]);
                                string T_filial = Convert.ToString(t["T_filial"]);
                                string T_bandeira = Convert.ToString(t["T_dsBandeira"].Equals(DBNull.Value) ? "" : t["T_dsBandeira"]);
                                string T_adquirente = Convert.ToString(t["T_nmAdquirente"].Equals(DBNull.Value) ? "" : t["T_nmAdquirente"]);
                                byte T_qtParcelas = Convert.ToByte(t["T_qtParcelas"].Equals(DBNull.Value) ? 0 : t["T_qtParcelas"]);
                                byte T_nrParcela = Convert.ToByte(t["T_nrParcela"]);
                                DateTime T_dtTitulo = (DateTime)t["T_dtRecebimentoPrevisto"];
                                Decimal T_vlParcela = Convert.ToDecimal(t["T_vlParcela"]);

                                error += Environment.NewLine + "Adquirente: " + T_adquirente;
                                error += Environment.NewLine + "Bandeira: " + T_bandeira;
                                error += Environment.NewLine + "Filial: " + T_filial;
                                if (T_dtVenda != null)
                                {
                                    error += Environment.NewLine + "Venda em " + T_dtVenda.Value.ToShortDateString();
                                    if (T_vlVenda != null)
                                        error += Environment.NewLine + " no valor de " + T_vlVenda.Value.ToString("C");
                                }
                                else if (T_vlVenda != null)
                                    error += Environment.NewLine + "Valor da venda: " + T_vlVenda.Value.ToString("C");
                                error += Environment.NewLine + "NSU: " + T_nsu;
                                error += Environment.NewLine + "Parcela " + T_nrParcela + (T_qtParcelas > 0 ? " de " + T_qtParcelas : "");
                                error += Environment.NewLine + "Dt Prevista Recebimento: " + T_dtTitulo;
                                error += Environment.NewLine + "Valor Bruto: " + T_vlParcela.ToString("C");

                                error += Environment.NewLine;


                                foreach (IDataRecord r in resultado)
                                {
                                    DateTime P_dtVenda = (DateTime)r["P_dtVenda"];
                                    string P_nsu = Convert.ToString(r["P_nsu"]);
                                    decimal P_vlVenda = Convert.ToDecimal(r["P_vlVenda"]);
                                    string P_filial = Convert.ToString(r["P_filial"]);
                                    string P_bandeira = Convert.ToString(r["P_dsBandeira"]);
                                    string P_adquirente = Convert.ToString(r["P_nmAdquirente"]);
                                    int P_qtParcelas = Convert.ToInt32(r["P_qtParcelas"].Equals(DBNull.Value) ? 0 : r["P_qtParcelas"]);
                                    int P_nrParcela = Convert.ToInt32(r["P_nrParcela"]);
                                    DateTime P_dtRecebimentoPrevisto = (DateTime)r["P_dtRecebimentoPrevisto"];
                                    DateTime? P_dtRecebimentoEfetivo = r["P_dtRecebimentoEfetivo"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)r["P_dtRecebimentoEfetivo"];
                                    bool P_flAntecipado = Convert.ToBoolean(r["P_flAntecipado"]);
                                    Decimal P_vlParcela = Convert.ToDecimal(r["P_vlParcela"]);

                                    error += Environment.NewLine + "=> RECEBÍVEL";
                                    error += Environment.NewLine + "   Adquirente: " + P_adquirente;
                                    error += Environment.NewLine + "   Bandeira: " + P_bandeira;
                                    error += Environment.NewLine + "   Filial: " + P_filial;
                                    error += Environment.NewLine + "   Venda em " + P_dtVenda.ToShortDateString() + " no valor de " + P_vlVenda.ToString("C");
                                    error += Environment.NewLine + "   NSU: " + P_nsu;
                                    error += Environment.NewLine + "   Parcela " + (P_nrParcela == 0 ? 1 : P_nrParcela) + (P_qtParcelas > 0 ? " de " + P_qtParcelas : "");
                                    error += Environment.NewLine + "   Dt Prevista Recebimento: " + P_dtRecebimentoPrevisto;
                                    if (P_dtRecebimentoEfetivo != null && !P_dtRecebimentoEfetivo.Value.Equals(P_dtRecebimentoPrevisto))
                                        error += Environment.NewLine + " Dt Efetiva Recebimento: " + P_dtRecebimentoEfetivo.Value.ToShortDateString() + (P_flAntecipado ? " (ANTECIPAÇÃO)" : "");
                                    error += Environment.NewLine + "   Valor Bruto: " + P_vlParcela.ToString("C");

                                    error += Environment.NewLine;
                                }
                            }

                        }

                        throw new Exception(error);
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

                    
                }
                #endregion


                string url = "http://" + grupo_empresa.dsAPI + DOMINIO;
                //string url = "http://localhost:60049/";
                string complemento = "titulos/baixaautomatica/" + token + "?" + ("" + (int)CAMPOS.IDEXTRATO) + "=" + idExtrato;


                HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromMinutes(30); // 30 minutos de timeout
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                System.Net.Http.HttpResponseMessage response = client.GetAsync(complemento).Result;

                //se retornar com sucesso busca os dados
                if (response.IsSuccessStatusCode)
                    //Pegando os dados do Rest e armazenando na variável retorno
                    retorno = response.Content.ReadAsAsync<Retorno>().Result;
                else
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
 

                return retorno;

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a baixa automática" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}