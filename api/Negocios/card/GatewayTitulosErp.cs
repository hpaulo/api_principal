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

namespace api.Negocios.Card
{
    public class GatewayTitulosErp
    {
       // public static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTitulosErp()
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
        };

        private readonly static string DOMINIO = System.Configuration.ConfigurationManager.AppSettings["DOMINIO"];



        private static Retorno carregaTitulos(painel_taxservices_dbContext _db, string token, string dsAPI, string data)
        {
            // Coloca a data no padrão de sql
            data = data.Substring(0, 4) + "-" + data.Substring(4, 2) + "-" + data.Substring(6, 2);

            string url = "http://" + dsAPI + DOMINIO;
            string complemento = "titulos/consultatitulos/" + token + "?" + ("" + (int)CAMPOS.DATA) + "=" + data;


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
            string resp = response.Content.ReadAsAsync<string>().Result;
            if (resp != null && !resp.Trim().Equals(""))
                throw new Exception(((int)response.StatusCode) + " - " + resp);
            throw new Exception(((int)response.StatusCode) + "");

        }



        /// <summary>
        /// Retorna Títulos ERP
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                //try
                //{
                //    ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                //}
                //catch { }

                //DECLARAÇÕES
                string outValue = null;

                // DATA
                string data = String.Empty;
                if (!queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                    throw new Exception("O identificador da movimentação bancária deve ser informada para a baixa automática!");

                data = queryString["" + (int)CAMPOS.DATA];

                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo == 0) throw new Exception("Um grupo deve ser selecionado como para a baixa automática!");

                grupo_empresa grupo_empresa = _db.grupo_empresa.Where(e => e.id_grupo == IdGrupo).FirstOrDefault();

                if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                    throw new Exception("Permissão negada! Empresa não possui o serviço ativo");

                // Obtém os títulos
                Retorno retorno = carregaTitulos(_db, token, grupo_empresa.dsAPI, data);

                // Obtém os registros
                List<dynamic> titulos = new List<dynamic>();
                foreach (dynamic registro in retorno.Registros)
                {
                    string nrCNPJ = registro.nrCNPJ;
                    Int32 cdAdquirente = Convert.ToInt32(registro.cdAdquirente);

                    titulos.Add(new
                    {
                        empresa = _db.empresas.Where(f => f.nu_cnpj.Equals(nrCNPJ)).Select(f => new
                        {
                            f.nu_cnpj,
                            f.ds_fantasia,
                            f.filial
                        }).FirstOrDefault(),
                        nrNSU = registro.nrNSU,
                        dtVenda = registro.dtVenda,
                        tbAdquirente = _db.tbAdquirentes.Where(a => a.cdAdquirente == cdAdquirente).Select(a => new
                        {
                            a.cdAdquirente,
                            a.nmAdquirente
                        }).FirstOrDefault(),
                        dsBandeira = registro.dsBandeira,
                        vlVenda = registro.vlVenda,
                        qtParcelas = registro.qtParcelas,
                        dtTitulo = registro.dtTitulo,
                        vlParcela = registro.vlParcela,
                        nrParcela = registro.nrParcela,
                        cdERP = registro.cdERP,
                        dtBaixaERP = registro.dtBaixaERP,
                    });
                }


                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (titulos.Count > pageSize && pageNumber > 0 && pageSize > 0)
                {
                    titulos = titulos.OrderBy(r => r.empresa.ds_fantasia)
                                     .ThenBy(r => r.dtVenda)
                                     .ThenBy(r => r.tbAdquirente.nmAdquirente)
                                     .ThenBy(r => r.dsBandeira)
                                     .ThenBy(r => r.dtTitulo)
                                     .Skip(skipRows).Take(pageSize)
                                     .ToList<dynamic>();
                }
                else
                {
                    pageNumber = 1;
                    titulos = titulos.OrderBy(r => r.empresa.ds_fantasia)
                                     .ThenBy(r => r.dtVenda)
                                     .ThenBy(r => r.tbAdquirente.nmAdquirente)
                                     .ThenBy(r => r.dsBandeira)
                                     .ThenBy(r => r.dtTitulo)
                                     .ToList<dynamic>();
                }

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = titulos;

                return retorno;

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar títulos ERP" : erro);
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

        /// <summary>
        /// Importa títulos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void ImportaTitulos(string token, ImportaTitulos param, painel_taxservices_dbContext _dbContext = null)
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
                    if (IdGrupo == 0) throw new Exception("Um grupo deve ser selecionado como para a baixa automática!");

                    grupo_empresa grupo_empresa = _db.grupo_empresa.Where(e => e.id_grupo == IdGrupo).FirstOrDefault();

                    if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                        throw new Exception("Permissão negada! Empresa não possui o serviço ativo");

                    Retorno retorno = carregaTitulos(_db, token, grupo_empresa.dsAPI, param.data);

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
                    throw new Exception(erro.Equals("") ? "Falha ao importar títulos ERP" : erro);
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

            //List<dynamic> grupo = Registros
            //                             .GroupBy(e => new { e.nrCNPJ, e.nrNSU, e.dtTitulo, e.nrParcela })
            //                             .Where(e => e.Count() > 1)
            //                             .Select(e => new
            //                             {
            //                                 e.Key.nrCNPJ,
            //                                 e.Key.nrNSU,
            //                                 e.Key.nrParcela,
            //                                 e.Key.dtTitulo,
            //                                 count = e.Count()
            //                             })
            //                             .OrderByDescending(e => e.count)
            //                             .ToList<dynamic>();

            for (var k = 0; k < Registros.Count; k++)
            {
                dynamic tit = Registros[k];
                DbContextTransaction transaction = _db.Database.BeginTransaction();
                try
                {
                    string dsBandeira = tit.dsBandeira;
                    if (dsBandeira.Length > 50) dsBandeira = dsBandeira.Substring(0, 50);

                    tbRecebimentoTitulo tbRecebimentoTitulo = new tbRecebimentoTitulo
                    {
                        dsBandeira = dsBandeira,
                        cdAdquirente = tit.cdAdquirente,//tit.tbAdquirente.cdAdquirente,
                        cdERP = tit.cdERP,
                        dtBaixaERP = tit.dtBaixaERP,
                        dtTitulo = tit.dtTitulo,
                        dtVenda = tit.dtVenda,
                        nrCNPJ = tit.nrCNPJ,//tit.empresa.nu_cnpj,
                        nrNSU = tit.nrNSU,// != null ? tit.nrNSU : "",
                        nrParcela = Convert.ToByte(tit.nrParcela),
                        qtParcelas = Convert.ToByte(tit.qtParcelas),
                        vlParcela = Convert.ToDecimal(tit.vlParcela),
                        vlVenda = Convert.ToDecimal(tit.vlVenda),
                    };

                    tbRecebimentoTitulo titulo = _db.tbRecebimentoTitulos
                                                            // Unique
                                                            .Where(e => e.nrCNPJ.Equals(tbRecebimentoTitulo.nrCNPJ))
                                                            .Where(e => e.nrNSU.Equals(tbRecebimentoTitulo.nrNSU))
                                                            .Where(e => e.dtTitulo.Equals(tbRecebimentoTitulo.dtTitulo))
                                                            .Where(e => e.nrParcela == tbRecebimentoTitulo.nrParcela)
                                                            .Where(e => e.cdERP == tbRecebimentoTitulo.cdERP)
                                                            .FirstOrDefault();

                    if (titulo == null)
                    {
                        //GatewayTbRecebimentoTitulo.Add(token, tbRecebimentoTitulo, _db);
                        _db.tbRecebimentoTitulos.Add(tbRecebimentoTitulo);
                        //try
                        //{
                        //    _db.SaveChanges();
                        //}
                        //catch(Exception e)
                        //{
                        //    ((IObjectContextAdapter)_db).ObjectContext.Detach(tbRecebimentoTitulo);
                        //    if (e is DbEntityValidationException)
                        //        throw new Exception(MensagemErro.getMensagemErro((DbEntityValidationException)e));
                        //    else
                        //        throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
                        //}
                    }
                    else
                    {
                        //tbRecebimentoTitulo.idRecebimentoTitulo = titulo.idRecebimentoTitulo;
                        //GatewayTbRecebimentoTitulo.Update(token, tbRecebimentoTitulo, _db);
                        titulo.dtVenda = tbRecebimentoTitulo.dtVenda;
                        titulo.cdAdquirente = tbRecebimentoTitulo.cdAdquirente;
                        titulo.dsBandeira = tbRecebimentoTitulo.dsBandeira;
                        titulo.vlVenda = tbRecebimentoTitulo.vlVenda;
                        titulo.qtParcelas = tbRecebimentoTitulo.qtParcelas;
                        titulo.vlParcela = tbRecebimentoTitulo.vlParcela;
                        //titulo.cdERP = tbRecebimentoTitulo.cdERP;
                        titulo.dtBaixaERP = tbRecebimentoTitulo.dtBaixaERP;
                        //try
                        //{
                        //    _db.SaveChanges();
                        //}
                        //catch(Exception e)
                        //{
                        //    _db.Entry(titulo).Reload();
                        //    if (e is DbEntityValidationException)
                        //        throw new Exception(MensagemErro.getMensagemErro((DbEntityValidationException)e));
                        //    else
                        //        throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
                        //}
                    }
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    string json = JsonConvert.SerializeObject(tit);
                    string erro = String.Empty;
                    if (e is DbEntityValidationException)
                        erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    else
                        erro = e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message;
                    //throw new Exception("Título: " + json + ". Erro: " + erro);
                    // Reporta o erro
                    retorno.Totais = new Dictionary<string, object>();
                    retorno.Totais.Add("erro", "Título: " + json + ". Erro: " + erro);
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
                string pastaCSVs = HttpContext.Current.Server.MapPath("~/App_Data/Titulos_ERP/");

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

                    List<dynamic> titulosERPCSV = new List<dynamic>();
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
                                throw new Exception("CNPJ " + nrCNPJ + " não está cadastrado no grupo " + _db.grupo_empresa.Where(e => e.id_grupo == idGrupo).Select(e => e.ds_nome.ToUpper()).First());

                            // NSU
                            string nrNSU = fileira[1].Trim();
                            if (nrNSU.Equals(""))
                            {
                                if(fileira.Count < 11)
                                    throw new Exception("NSU e código do ERP não informados na linha " + contLinha + "!");

                                nrNSU = "T" + fileira[10]; // "T" + cdERP
                            }

                            DateTime dtTitulo = DateTime.Now;
                            try
                            {
                                dtTitulo = Convert.ToDateTime(formataDataDoCSV(fileira[7]));
                            }
                            catch
                            {
                                throw new Exception("Data do título não está no formato esperado (linha " + contLinha + ")!");
                            }
                            int nrParcela = 0;
                            try
                            {
                                nrParcela = Convert.ToInt32(fileira[9]);
                            }
                            catch
                            {
                                throw new Exception("Número da parcela não está no formato esperado (linha " + contLinha + ")!");
                            };

                            titulosERPCSV.Add(new {
                                nrCNPJ = nrCNPJ,
                                nrNSU = nrNSU,
                                dtVenda = Convert.ToDateTime(formataDataDoCSV(fileira[2])),
                                cdAdquirente = Convert.ToInt32(fileira[3].Trim()),
                                dsBandeira = fileira[4],
                                vlVenda = Convert.ToDouble(fileira[5]),
                                qtParcelas = Convert.ToInt32(fileira[6]),
                                dtTitulo = dtTitulo,
                                vlParcela = Convert.ToDouble(fileira[8]),
                                nrParcela = nrParcela,
                                cdERP = fileira.Count < 11 ? (string) null : fileira[10],
                                dtBaixaERP = fileira.Count < 12 ? (DateTime?)null : Convert.ToDateTime(formataDataDoCSV(fileira[11]))
                            });

                            contLinha++;        
                            
                        }
                    }

                    if(titulosERPCSV.Count > 0)
                    {
                        // Importa os títulos em background
                        retorno.Registros = titulosERPCSV;
                        retorno.TotalDeRegistros = titulosERPCSV.Count;

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
