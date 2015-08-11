using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Globalization;
using System.Net.Http;
using OFXSharp;
using System.IO;

namespace api.Negocios.Card
{
    public class GatewayTbExtrato
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbExtrato()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDEXTRATO = 100,
            CDCONTACORRENTE = 101,
            NRDOCUMENTO = 102,
            DTEXTRATO = 103,
            DSDOCUMENTO = 104,
            VLMOVIMENTO = 105,

        };

        /// <summary>
        /// Get TbExtrato/TbExtrato
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbExtrato> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbExtratos.AsQueryable<tbExtrato>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDEXTRATO:
                        Int32 idExtrato = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idExtrato == idExtrato).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdContaCorrente == cdContaCorrente).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.NRDOCUMENTO:
                        string nrDocumento = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrDocumento.Equals(nrDocumento)).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.DTEXTRATO:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtaIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => (e.dtExtrato.Year > dtaIni.Year || (e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month > dtaIni.Month) ||
                                                                                          (e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month && e.dtExtrato.Day >= dtaIni.Day))
                                                    && (e.dtExtrato.Year < dtaFim.Year || (e.dtExtrato.Year == dtaFim.Year && e.dtExtrato.Month < dtaFim.Month) ||
                                                                                          (e.dtExtrato.Year == dtaFim.Year && e.dtExtrato.Month == dtaFim.Month && e.dtExtrato.Day <= dtaFim.Day)));
                        }
                        else if (item.Value.Contains(">")) // MAIOR IGUAL
                        {
                            string busca = item.Value.Replace(">", "");
                            DateTime dta = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato >= dta);
                        }
                        else if (item.Value.Contains("<")) // MENOR IGUAL
                        {
                            string busca;
                            if (item.Value.Length == 8)
                            {
                                string dia = item.Value.Substring(6, 1);
                                string anoMes = item.Value.Substring(0, 6);
                                busca = anoMes + "0" + dia;
                            }
                            else
                            {
                                busca = item.Value.Replace("<", "");
                            }
                            //busca = item.Value.Replace("<", "");
                            DateTime dta = DateTime.ParseExact(busca + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato <= dta);
                        }
                        else if (item.Value.Length == 4)
                        {
                            string busca = item.Value + "0101";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year);
                        }
                        else if (item.Value.Length == 6)
                        {
                            string busca = item.Value + "01";
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month);
                        }
                        else if (item.Value.Length == 7)
                        {
                            string dia = item.Value.Substring(6, 1);
                            string anoMes = item.Value.Substring(0, 6);
                            string busca = anoMes + "0" + dia;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month && e.dtExtrato.Day == dtaIni.Day);
                        }
                        else // IGUAL
                        {
                            string busca = item.Value;
                            DateTime dtaIni = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(e => e.dtExtrato.Year == dtaIni.Year && e.dtExtrato.Month == dtaIni.Month && e.dtExtrato.Day == dtaIni.Day);
                        }
                        break;
                    case CAMPOS.DSDOCUMENTO:
                        decimal dsDocumento = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.dsDocumento.Equals(dsDocumento)).AsQueryable<tbExtrato>();
                        break;
                    case CAMPOS.VLMOVIMENTO:
                        decimal vlMovimento = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlMovimento == vlMovimento).AsQueryable<tbExtrato>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDEXTRATO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idExtrato).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.idExtrato).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdContaCorrente).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.cdContaCorrente).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.NRDOCUMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrDocumento).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.nrDocumento).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.DTEXTRATO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtExtrato).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.dtExtrato).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.DSDOCUMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsDocumento).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.dsDocumento).AsQueryable<tbExtrato>();
                    break;
                case CAMPOS.VLMOVIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlMovimento).AsQueryable<tbExtrato>();
                    else entity = entity.OrderByDescending(e => e.vlMovimento).AsQueryable<tbExtrato>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbExtrato/TbExtrato
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbExtrato = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
            var queryTotal = query;

            // TOTAL DE REGISTROS
            retorno.TotalDeRegistros = queryTotal.Count();


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
                CollectionTbExtrato = query.Select(e => new
                {

                    idExtrato = e.idExtrato,
                    cdContaCorrente = e.cdContaCorrente,
                    nrDocumento = e.nrDocumento,
                    dtExtrato = e.dtExtrato,
                    dsDocumento = e.dsDocumento,
                    vlMovimento = e.vlMovimento,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbExtrato = query.Select(e => new
                {

                    idExtrato = e.idExtrato,
                    cdContaCorrente = e.cdContaCorrente,
                    nrDocumento = e.nrDocumento,
                    dtExtrato = e.dtExtrato,
                    dsDocumento = e.dsDocumento,
                    vlMovimento = e.vlMovimento,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbExtrato;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbExtrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbExtrato param)
        {
            // Valida param para não adicionar duplicidades
            tbExtrato extrato = _db.tbExtratos.Where(e => e.cdContaCorrente == param.cdContaCorrente)
                                              .Where(e => e.dtExtrato.Equals(param.dtExtrato))
                                              .Where(e => e.nrDocumento.Equals(param.nrDocumento))
                                              .Where(e => e.vlMovimento == param.vlMovimento)
                                              .Where(e => e.dsDocumento.Equals(param.dsDocumento))
                                              .Where(e => e.dsTipo.Equals(param.dsTipo))
                                              .FirstOrDefault();

            if (extrato != null) throw new Exception("Extrato já existe");
            _db.tbExtratos.Add(param);
            _db.SaveChanges();
            return param.idExtrato;
        }


        /// <summary>
        /// Apaga uma TbExtrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idExtrato)
        {
            tbExtrato extrato = _db.tbExtratos.Where(e => e.idExtrato == idExtrato).FirstOrDefault();
            if (extrato == null) throw new Exception("Extrato inexistente");
            // Remove o arquivo associado do disco, caso não tenha mais nenhum registro referenciando esse arquivo
            if (!extrato.dsArquivo.Equals("") && _db.tbExtratos.Where(e => e.dsArquivo.Equals(extrato.dsArquivo)).FirstOrDefault() == null)
                File.Delete(extrato.dsArquivo);
            // Remove o extrato da base
            _db.tbExtratos.Remove(extrato);
            _db.SaveChanges();
        }
        /*/// <summary>
        /// Altera tbExtrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbExtrato param)
        {
            tbExtrato value = _db.tbExtratos
                    .Where(e => e.idExtrato == param.idExtrato)
                    .First<tbExtrato>();

            if (value == null) throw new Exception("Extrato inexistente");

            if (param.nrDocumento != null && param.nrDocumento != value.nrDocumento)
                value.nrDocumento = param.nrDocumento;
            if (param.dtExtrato != null && param.dtExtrato != value.dtExtrato)
                value.dtExtrato = param.dtExtrato;
            if (param.vlMovimento != null && param.vlMovimento != value.vlMovimento)
                value.vlMovimento = param.vlMovimento;
            _db.SaveChanges();

        }*/


        /// <summary>
        /// Recebe o extrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Patch(string token, Dictionary<string, string> queryString)
        {
            string pastaExtratos = HttpContext.Current.Server.MapPath("~/App_Data/Extratos/");

            Int32 idGrupo = Permissoes.GetIdGrupo(token);
            if (idGrupo == 0) throw new Exception("Grupo inválido");

            string outValue = null;
            if (!queryString.TryGetValue("" + (int)CAMPOS.CDCONTACORRENTE, out outValue))
                throw new Exception("Conta corrente não informada");

            // Conta
            Int32 idContaCorrente = Convert.ToInt32(queryString["" + (int)CAMPOS.CDCONTACORRENTE]);
            tbContaCorrente conta = _db.tbContaCorrentes.Where(e => e.idContaCorrente == idContaCorrente).FirstOrDefault();
            if (conta == null) throw new Exception("Conta corrente inexistente");
            if (!conta.flAtivo) throw new Exception("Conta corrente está inativada");

            // Cria os diretórios, caso não existam
            if (!Directory.Exists(pastaExtratos)) Directory.CreateDirectory(pastaExtratos);
            if (!Directory.Exists(pastaExtratos + idGrupo + "\\")) Directory.CreateDirectory(pastaExtratos + idGrupo + "/");
            // Diretório específico da conta
            string diretorio = pastaExtratos + idGrupo + "\\" + idContaCorrente + "\\";
            if (!Directory.Exists(diretorio)) Directory.CreateDirectory(diretorio);

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                // Arquivo upado
                var postedFile = httpRequest.Files[0];
                // Obtém a extensão
                string extensao = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("."));
                // Obtém o nome do arquivo upado
                string nomeArquivo = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf(".")) + "_0" + extensao;

                // Valida o nome do arquivo dentro do diretório => deve ser único
                int cont = 0;
                while (File.Exists(diretorio + nomeArquivo))
                {
                    // Novo nome
                    nomeArquivo = nomeArquivo.Substring(0, nomeArquivo.LastIndexOf("_") + 1);
                    nomeArquivo += ++cont + extensao;
                }
                // Obtém o caminho completo do arquivo e o salva no disco
                string filePath = diretorio + nomeArquivo;
                postedFile.SaveAs(filePath);
                // Obtém o objeto associado ao extrato
                var parser = new OFXDocumentParser();
                OFXDocument ofxDocument = parser.Import(new FileStream(filePath, FileMode.Open));

                /* 
                    CONTA
                    ofxDocument.Account.BankID : string // código do banco
                    ofxDocument.Account.BranchID : string // número da agência
                    ofxDocument.Account.AccountID : string // número da conta (pode conter também o número da agência)

                    BALANÇO
                    Balance.LedgerBalance : double // valor total do extrato

                    TRANSAÇÕES
                    Transactions : array de objetos
                      => TransType : int // tipo de transação
                      => Date : datetime // (com "T" entre date e time)
                      => Amount : double // valor da transação
                      => TransactionID = CheckNum: string
                      => Memo : string // descrição
                */
                /* VALIDA A CONTA */
                string banco = ofxDocument.Account.BankID;
                string nrAgencia = ofxDocument.Account.BranchID;
                string nrConta = ofxDocument.Account.AccountID;

                // Valida código do banco
                if (banco.Length > 3) banco = banco.Substring(banco.Length - 3, 3); // pega somente os últimos 3 dígitos
                else
                {
                    // Adiciona zeros a esquerda, caso tenha menos de 3 dígitos
                    while(banco.Length < 3) banco = "0" + banco;
                }
                if (!conta.cdBanco.Equals(banco))
                {
                    // Deleta o arquivo
                    File.Delete(filePath);
                    throw new Exception("Extrato upado não corresponde ao banco da conta informada");
                }
                // Valida o número da conta
                if (nrConta.Contains("/"))
                {
                    // Agência e conta estão "juntas"
                    nrAgencia = nrConta.Substring(0, nrConta.IndexOf("/"));
                    nrConta = nrConta.Substring(nrConta.IndexOf("/") + 1);
                }
                if (!conta.nrConta.Equals(nrConta))
                {
                    // Deleta o arquivo
                    File.Delete(filePath);
                    throw new Exception("Extrato upado não corresponde ao número da conta informada");
                }
                // Valida o número da agência
                if (!nrAgencia.Equals("") && !conta.nrAgencia.Equals(nrAgencia))
                {
                    // Deleta o arquivo
                    File.Delete(filePath);
                    throw new Exception("Extrato upado não corresponde ao número da agência informada");
                }


                /* ARMAZENA */
                bool armazenou = false;
                foreach (var transacao in ofxDocument.Transactions)
                {
                    tbExtrato extrato = new tbExtrato();
                    extrato.cdContaCorrente = idContaCorrente;
                    extrato.dtExtrato = new DateTime(transacao.Date.Year, transacao.Date.Month, transacao.Date.Day);
                    extrato.vlMovimento = transacao.Amount;
                    extrato.nrDocumento = transacao.CheckNum;
                    extrato.dsTipo = transacao.TransType.ToString();
                    extrato.dsDocumento = transacao.Memo;
                    extrato.dsArquivo = filePath;
                    // Salva na base
                    try
                    {
                        Add(token, extrato);
                        armazenou = true; // notifica que armazenou o extrato na base
                    }catch(Exception e)
                    {
                        // JÁ EXISTE UM EXTRATO COM ESSAS INFORMAÇÕES
                        ;
                    }
                }

                // Se não armazenou pelo menos um elemento, deleta o arquivo
                if (!armazenou) File.Delete(filePath);

            }
            else throw new Exception("400"); // Bad Request
        }

    }
}
