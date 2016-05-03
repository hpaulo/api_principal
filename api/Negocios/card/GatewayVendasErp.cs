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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace api.Negocios.Card
{
    public class GatewayVendasErp
    {
       // public static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayVendasErp()
        {
           // _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100,
            ID_GRUPO = 101,
            NRCNPJ = 102,
        };

        private readonly static string DOMINIO = System.Configuration.ConfigurationManager.AppSettings["DOMINIO"];



        private static Retorno carregaVendas(painel_taxservices_dbContext _db, string token, string dsAPI, string data, string nrCNPJ)
        {
            if (data == null)
                return null;

            // Coloca a data no padrão de sql
            data = data.Substring(0, 4) + "-" + data.Substring(4, 2) + "-" + data.Substring(6, 2);

            string url = "http://" + dsAPI + DOMINIO;
            //string url = "http://localhost:50939";
            string complemento = "vendas/consultavendas/" + token + "?" + ("" + (int)CAMPOS.DATA) + "=" + data;

            if(nrCNPJ != null)
                complemento += "&" + ("" + (int)CAMPOS.NRCNPJ) + "=" + nrCNPJ;


            HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromMinutes(5); // 5 minutos de timeout
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            System.Net.Http.HttpResponseMessage response = client.GetAsync(complemento).Result;

            //se retornar com sucesso busca os dados
            if (response.IsSuccessStatusCode)
            {
                //Pegando os dados do Rest e armazenando na variável retorno
                Retorno retorno = response.Content.ReadAsAsync<Retorno>().Result;
                retorno.TotalDeRegistros = retorno.Registros.Count;
                return retorno;
            }
            // Falhou!
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



        /// <summary>
        /// Retorna Vendas ERP
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            DbContextTransaction transaction = _db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
            try
            {
                //DECLARAÇÕES
                string outValue = null;

                // DATA
                string data = String.Empty;
                if (!queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                    throw new Exception("A data deve ser informada!");

                data = queryString["" + (int)CAMPOS.DATA];

                string nrCNPJ = null;
                if (queryString.TryGetValue("" + (int)CAMPOS.NRCNPJ, out outValue))
                    nrCNPJ = queryString["" + (int)CAMPOS.NRCNPJ];

                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo == 0) throw new Exception("Um grupo deve ser selecionado como para a listagem das vendas!");

                grupo_empresa grupo_empresa = _db.Database.SqlQuery<grupo_empresa>("SELECT G.*" +
                                                                                   " FROM cliente.grupo_empresa G (NOLOCK)" +
                                                                                   " WHERE G.id_grupo = " + IdGrupo)
                                                          .FirstOrDefault();

                if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                    throw new Exception("Permissão negada! Empresa não possui o serviço ativo");

                // Obtém as vendas
                Retorno retorno = carregaVendas(_db, token, grupo_empresa.dsAPI, data, nrCNPJ);

                // Obtém os registros
                List<dynamic> vendas = new List<dynamic>();
                foreach (dynamic registro in retorno.Registros)
                {
                    nrCNPJ = registro.nrCNPJ;
                    Int32 cdAdquirente = Convert.ToInt32(registro.cdAdquirente);

                    var empresa = _db.empresas.Where(f => f.nu_cnpj.Equals(nrCNPJ)).Select(f => new
                                                {
                                                    f.nu_cnpj,
                                                    f.ds_fantasia,
                                                    f.filial,
                                                    f.id_grupo
                                                }).FirstOrDefault();

                    string cdSacado = null;
                    try
                    {
                        cdSacado = registro.cdSacado;
                        cdSacado = cdSacado.Trim();
                    }
                    catch { }
                    //string cdERPPagamento = null;
                    //try
                    //{
                    //    cdERPPagamento = registro.cdERPPagamento;
                    //}
                    //catch { }

                    vendas.Add(new
                    {
                        empresa = new {
                            empresa.nu_cnpj,
                            empresa.ds_fantasia,
                            empresa.filial,
                        },
                        nrNSU = registro.nrNSU,
                        dtVenda = registro.dtVenda,
                        //tbAdquirente = _db.tbAdquirentes.Where(a => a.cdAdquirente == cdAdquirente).Select(a => new
                        //{
                        //    a.cdAdquirente,
                        //    a.nmAdquirente
                        //}).FirstOrDefault(),
                        tbAdquirente = cdSacado == null ? null : _db.tbBandeiraSacados.Where(t => t.cdSacado.Equals(cdSacado) && t.cdGrupo == empresa.id_grupo)
                                                           .Select(t => new
                                                           {
                                                               cdAdquirente = t.tbBandeira.cdAdquirente,
                                                               nmAdquirente = t.tbBandeira.tbAdquirente.nmAdquirente
                                                           })
                                                           .FirstOrDefault(),
                        cdSacado = cdSacado,
                        dsBandeira = registro.dsBandeira,
                        vlVenda = registro.vlVenda,
                        qtParcelas = registro.qtParcelas,
                        cdERP = registro.cdERP,
                        //cdERPPagamento = cdERPPagamento,
                        //dtCorrecaoERP = registro.dtCorrecaoERP,
                    });
                }


                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (vendas.Count > pageSize && pageNumber > 0 && pageSize > 0)
                {
                    vendas = vendas.OrderBy(r => r.empresa.ds_fantasia)
                                     .ThenBy(r => r.dtVenda)
                                     //.ThenBy(r => r.tbAdquirente.nmAdquirente)
                                     .ThenBy(r => r.dsBandeira)
                                     .Skip(skipRows).Take(pageSize)
                                     .ToList<dynamic>();
                }
                else
                {
                    pageNumber = 1;
                    vendas = vendas.OrderBy(r => r.empresa.ds_fantasia)
                                     .ThenBy(r => r.dtVenda)
                                     //.ThenBy(r => r.tbAdquirente.nmAdquirente)
                                     .ThenBy(r => r.dsBandeira)
                                     .ToList<dynamic>();
                }

                transaction.Commit();

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = vendas;

                return retorno;

            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar vendas ERP" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha a conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }

        // GET "vendas/consultavendas"
        public static void ImportaVendas(string token, ImportaVendas param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {

                if (param != null) 
                { 
                    // GRUPO EMPRESA => OBRIGATÓRIO!
                    Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                    //if (IdGrupo == 0 && param.id_grupo != 0) IdGrupo = param.id_grupo;
                    if (IdGrupo == 0) throw new Exception("Um grupo deve ser selecionado como para a importação das vendas!");

                    grupo_empresa grupo_empresa = _db.Database.SqlQuery<grupo_empresa>("SELECT G.*" +
                                                                                   " FROM cliente.grupo_empresa G (NOLOCK)" +
                                                                                   " WHERE G.id_grupo = " + IdGrupo)
                                                          .FirstOrDefault();

                    if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                        throw new Exception("Permissão negada! Empresa não possui o serviço ativo");

                    Retorno retorno = carregaVendas(_db, token, grupo_empresa.dsAPI, param.data, param.nrCNPJ);

                    Semaphore semaforo = new Semaphore(0, 1);

                    BackgroundWorker bw = new BackgroundWorker();
                    bw.WorkerReportsProgress = false;
                    bw.WorkerSupportsCancellation = false;
                    bw.DoWork += bw_DoWork;
                    List<object> args = new List<object>();
                    args.Add(_db);
                    args.Add(semaforo);
                    args.Add(retorno);
                    bw.RunWorkerAsync(args);

                    semaforo.WaitOne();

                    // Teve erro?
                    object outValue = null;
                    if (retorno.Totais != null && retorno.Totais.TryGetValue("erro", out outValue))
                        throw new Exception(retorno.Totais["erro"].ToString());
                }
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao importar vendas ERP" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha a conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }


        private static void bw_DoWork(object sender, DoWorkEventArgs a)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<object> args = a.Argument as List<object>;
            painel_taxservices_dbContext _db = args[0] as painel_taxservices_dbContext;
            Semaphore semaforo = args[1] as Semaphore;
            Retorno retorno = args[2] as Retorno;

            List<dynamic> Registros = retorno.Registros as List<dynamic>;

            //List<dynamic> test = Registros.Where(t => Convert.ToString(t.nrCNPJ).Equals("08297710000480")).ToList();

            for (var k = 0; k < Registros.Count; k++)
            {
                dynamic vd = Registros[k];
                DbContextTransaction transaction = _db.Database.BeginTransaction();
                try
                {
                    string dsBandeira = vd.dsBandeira;
                    if (dsBandeira != null && dsBandeira.Length > 50) dsBandeira = dsBandeira.Substring(0, 50);

                    string cdSacado = null;
                    try
                    {
                        cdSacado = vd.cdSacado;
                        cdSacado = cdSacado.Trim();
                    }
                    catch { }
                    //string cdERPPagamento = null;
                    //try
                    //{
                    //    cdERPPagamento = vd.cdERPPagamento;
                    //    cdERPPagamento = cdERPPagamento.Trim();
                    //}
                    //catch { }
                    if (cdSacado != null)
                    {
                        if (cdSacado.Equals(""))
                            cdSacado = null;
                        else if (cdSacado.Length > 10)
                            throw new Exception("Sacado '" + cdSacado + "' com mais de 10 caracteres!");
                    }

                    string nrCNPJ = vd.nrCNPJ;
                    //if (nrCNPJ.Equals("08297710000480"))
                    //    nrCNPJ += "";

                    tbRecebimentoVenda tbRecebimentoVenda = new tbRecebimentoVenda
                    {
                        dsBandeira = dsBandeira,
                        //cdAdquirente = vd.cdAdquirente,
                        cdERP = vd.cdERP,
                        dtVenda = vd.dtVenda,
                        nrCNPJ = nrCNPJ,
                        nrNSU = vd.nrNSU != null && !vd.nrNSU.ToString().Trim().Equals("") ? vd.nrNSU : "T" + vd.cdERP,
                        vlVenda = Convert.ToDecimal(vd.vlVenda),
                        cdSacado = cdSacado,
                        qtParcelas = Convert.ToByte(vd.qtParcelas),
                        //cdERPPagamento = cdERPPagamento,
                    };

                    tbRecebimentoVenda venda = _db.Database.SqlQuery<tbRecebimentoVenda>("SELECT V.*" +
                                                                                         " FROM card.tbRecebimentoVenda V (NOLOCK)" +
                                                                                         " WHERE V.nrCNPJ = '" + tbRecebimentoVenda.nrCNPJ + "'" +
                                                                                         " AND V.nrNSU = '" + tbRecebimentoVenda.nrNSU + "'" +
                                                                                         " AND V.dtVenda = '" + DataBaseQueries.GetDate(tbRecebimentoVenda.dtVenda) + "'" +
                                                                                         " AND V.cdERP = '" + tbRecebimentoVenda.cdERP + "'"
                                                                                         //" AND (V.cdERPPagamento IS NULL" + (cdERPPagamento == null ? "" : " OR V.cdERPPagamento = '" + tbRecebimentoVenda.cdERPPagamento + "'") + ")"
                                                                                        )
                                                             .FirstOrDefault();

                    if (venda == null)
                    {
                        _db.Database.ExecuteSqlCommand("INSERT INTO card.tbRecebimentoVenda" +
                                                       " (nrCNPJ, nrNSU, cdERP, dtVenda, dsBandeira, vlVenda, qtParcelas, cdSacado)" + //, cdERPPagamento)" +
                                                       " VALUES ('" + tbRecebimentoVenda.nrCNPJ + "'" +
                                                       ", '" + tbRecebimentoVenda.nrNSU + "'" +
                                                       ", '" +  tbRecebimentoVenda.cdERP + "'" +
                                                       ", '" + DataBaseQueries.GetDate(tbRecebimentoVenda.dtVenda) + "'" +
                                                       //", " + tbRecebimentoVenda.cdAdquirente +
                                                       ", " + (tbRecebimentoVenda.dsBandeira == null ? "NULL" : "'" + tbRecebimentoVenda.dsBandeira + "'") +
                                                       ", " + tbRecebimentoVenda.vlVenda.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                                                       ", " + tbRecebimentoVenda.qtParcelas +
                                                       ", " + (tbRecebimentoVenda.cdSacado != null ? "'" + tbRecebimentoVenda.cdSacado + "'" : "NULL") +
                                                       //", " + (tbRecebimentoVenda.cdERPPagamento != null ? "'" + tbRecebimentoVenda.cdERPPagamento + "'" : "NULL") +
                                                       ")");
                    }
                    else
                    {
                        _db.Database.ExecuteSqlCommand("UPDATE V" +
                                                       " SET V.dsBandeira = " + (tbRecebimentoVenda.dsBandeira == null ? "NULL" : "'" + tbRecebimentoVenda.dsBandeira + "'") +
                                                       //", V.nrNSU = '" + tbRecebimentoVenda.nrNSU + "'" +
                                                       ", V.vlVenda = " + tbRecebimentoVenda.vlVenda.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                                                       ", V.qtParcelas = " + tbRecebimentoVenda.qtParcelas +
                                                       ", V.cdSacado = " + (tbRecebimentoVenda.cdSacado != null ? "'" + tbRecebimentoVenda.cdSacado + "'" : "NULL") +
                                                       //", V.cdERPPagamento = " + (tbRecebimentoVenda.cdERPPagamento != null ? "'" + tbRecebimentoVenda.cdERPPagamento + "'" : "NULL") +
                                                       " FROM card.tbRecebimentoVenda V" +
                                                       " WHERE V.idRecebimentoVenda = " + venda.idRecebimentoVenda);

                    }
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    string json = JsonConvert.SerializeObject(vd);
                    string erro = String.Empty;
                    if (e is DbEntityValidationException)
                        erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    else
                        erro = e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message;
                    //throw new Exception("Venda: " + json + ". Erro: " + erro);
                    // Reporta o erro
                    retorno.Totais = new Dictionary<string, object>();
                    retorno.Totais.Add("erro", "Venda: " + json + ". Erro: " + erro);
                    break;
                }
            }

            semaforo.Release();
        }



        public static void Patch(string token, tbLogAcessoUsuario log, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                string pastaCSVs = HttpContext.Current.Server.MapPath("~/App_Data/Vendas_ERP/");

                // Tem que estar associado a um grupo
                Int32 idGrupo = Permissoes.GetIdGrupo(token, _db);
                if (idGrupo == 0) throw new Exception("Grupo inválido");

                // Tem que informar por filtro a conta corrente
                //string outValue = null;
                //if (!queryString.TryGetValue("" + (int)CAMPOS.CDCONTACORRENTE, out outValue))
                //    throw new Exception("Conta corrente não informada");

                #region OBTÉM O DIRETÓRIO A SER SALVO O CSV
                if (!Directory.Exists(pastaCSVs)) Directory.CreateDirectory(pastaCSVs);
                string diretorio = pastaCSVs + idGrupo + "\\";
                if (!Directory.Exists(diretorio)) Directory.CreateDirectory(diretorio);
                #endregion

                HttpRequest httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    #region OBTÉM NOME ÚNICO PARA O ARQUIVO UPADO
                    // Arquivo upado
                    HttpPostedFile postedFile = httpRequest.Files[0];
                    // Obtém a extensão
                    string extensao = postedFile.FileName.LastIndexOf(".") > -1 ? postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(".")) : ".csv";
                    if (!extensao.ToLower().Equals(".csv"))
                        throw new Exception("Só são aceitos arquivos do tipo CSV");


                    // Obtém o nome do arquivo upado
                    string nomeArquivo = (postedFile.FileName.LastIndexOf(".") > -1 ? postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf(".")) : postedFile.FileName) + "_0" + extensao;

                    // Remove caracteres inválidos para nome de arquivo
                    nomeArquivo = Path.GetInvalidFileNameChars().Aggregate(nomeArquivo, (current, c) => current.Replace(c.ToString(), string.Empty));

                    // Valida o nome do arquivo dentro do diretório => deve ser único
                    int cont = 0;
                    while (File.Exists(diretorio + nomeArquivo))
                    {
                        // Novo nome
                        nomeArquivo = nomeArquivo.Substring(0, nomeArquivo.LastIndexOf("_") + 1);
                        nomeArquivo += ++cont + extensao;
                    }
                    #endregion

                    #region SALVA ARQUIVO NO DISCO
                    string filePath = diretorio + nomeArquivo;
                    // Salva o arquivo
                    try
                    {
                        postedFile.SaveAs(filePath);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Não foi possível salvar o arquivo '" + filePath + "'! " + e.Message);
                    }
                    #endregion

                    // Loga o nome do arquivo
                    if (log != null) log.dsJson = filePath;

                    // CNPJs pertencentes ao grupo
                    List<string> CNPJSEmpresa = _db.empresas.Where(e => e.id_grupo == idGrupo).Select(e => e.nu_cnpj).ToList<string>();

                    Retorno retorno = new Retorno();

                    List<dynamic> vendasERPCSV = new List<dynamic>();
                    // Lê arquivo e preenche lista
                    using (CSVReader leitor = new CSVReader(filePath))
                    {
                        int contLinha = 1;
                        CSVFileira fileira = new CSVFileira();
                        while (leitor.LerLinha(fileira))
                        {
                            if (fileira == null || fileira.Count < 10)
                                throw new Exception("Linha " + contLinha + " do arquivo é inválida!");

                            // CNPJ
                            string nrCNPJ = fileira[0].Trim();
                            if (nrCNPJ.Equals(""))
                                throw new Exception("CNPJ não informado na linha " + contLinha + "!");

                            // CNPJ do grupo?
                            if (!CNPJSEmpresa.Contains(nrCNPJ))
                                throw new Exception("CNPJ " + nrCNPJ + " não está cadastrado no grupo " + _db.Database.SqlQuery<grupo_empresa>("SELECT UPPER(G.ds_nome)" +
                                                                                                                                               " FROM cliente.grupo_empresa G (NOLOCK)" +
                                                                                                                                               " WHERE G.id_grupo = " + idGrupo)
                                                                                                                      .FirstOrDefault());

                            // NSU
                            string nrNSU = fileira[1].Trim();
                            if (nrNSU.Equals(""))
                            {
                                if (fileira.Count < 7)
                                    throw new Exception("NSU e código do ERP não informados na linha " + contLinha + "!");

                                nrNSU = "T" + fileira[7]; // "T" + cdERP
                            }

                            DateTime dtVenda = DateTime.Now;
                            try
                            {
                                dtVenda = Convert.ToDateTime(formataDataDoCSV(fileira[2]));
                            }
                            catch
                            {
                                throw new Exception("Data da venda não está no formato esperado (linha " + contLinha + ")!");
                            }

                            vendasERPCSV.Add(new
                            {
                                nrCNPJ = nrCNPJ,
                                nrNSU = nrNSU,
                                dtVenda = dtVenda,
                                cdSacado = fileira[3],
                                dsBandeira = fileira[4],
                                vlVenda = Convert.ToDouble(fileira[5]),
                                qtParcelas = Convert.ToInt32(fileira[6]),
                                cdERP = fileira.Count < 7 ? (string)null : fileira[7],
                            });

                            contLinha++;

                        }
                    }

                    if (vendasERPCSV.Count > 0)
                    {
                        // Importa as vendas em background
                        retorno.Registros = vendasERPCSV;
                        retorno.TotalDeRegistros = vendasERPCSV.Count;

                        Semaphore semaforo = new Semaphore(0, 1);

                        BackgroundWorker bw = new BackgroundWorker();
                        bw.WorkerReportsProgress = false;
                        bw.WorkerSupportsCancellation = false;
                        bw.DoWork += bw_DoWork;
                        List<object> args = new List<object>();
                        args.Add(_db);
                        args.Add(semaforo);
                        args.Add(retorno);
                        bw.RunWorkerAsync(args);

                        semaforo.WaitOne();
                    }

                    // Teve erro?
                    object outValue = null;
                    if (retorno.Totais != null && retorno.Totais.TryGetValue("erro", out outValue))
                        throw new Exception(retorno.Totais["erro"].ToString());
                }
            }
            catch (Exception e)
            {
                // Rollback
                //transaction.Rollback();
                /*if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao enviar extrato" : erro);
                }*/
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


        private static string formataDataDoCSV(string data)
        {
            if (data == null || data.Length < 7 || data.Length > 8)
                return data;


            if (data.Length == 7) data = "0" + data;

            return data.Substring(0, 2) + "/" + data.Substring(2, 2) + "/" + data.Substring(4, 4);
        }
    }
}
