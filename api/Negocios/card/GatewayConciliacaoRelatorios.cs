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

namespace api.Negocios.Card
{
    public class GatewayConciliacaoRelatorios
    {

        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoRelatorios()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100, // pos.RecebimentoParcela
            NU_CNPJ = 101, // pos.Recebimento
            ID_GRUPO = 102
        };

        /// <summary>
        /// Retorna a lista de conciliação bancária
        /// </summary>
        /// <returns></returns>        
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
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
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de conciliação bancária!");
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ, CnpjEmpresa);
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.NU_CNPJ, CnpjEmpresa);
                }
                else throw new Exception("Uma filial deve ser selecionada como filtro de conciliação bancária!");

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
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE, "0"); // Somente movimentações que tem adquirente associado
                // PARA O EXTRATO, SÓ CONSIDERA OS TIPO CREDIT
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DSTIPO, OFXSharp.OFXTransactionType.CREDIT.ToString());


                // OBTÉM AS QUERIES                
                IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, 0, 0, queryStringRecebimentoParcela);
                IQueryable<tbRecebimentoAjuste> queryTbRecebimentoAjuste = GatewayTbRecebimentoAjuste.getQuery(0, (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, 0, 0, queryStringTbRecebimentoAjuste);
                IQueryable<tbExtrato> queryExtrato = GatewayTbExtrato.getQuery(0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, 0, 0, queryStringExtrato);


                List<ConciliacaoRelatorios> rRecebimentoParcela = queryRecebimentoParcela.Select(t => new ConciliacaoRelatorios
                {
                    tipo = "R",
                    adquirente = t.Recebimento.tbBandeira.tbAdquirente.nmAdquirente,
                    bandeira = t.Recebimento.tbBandeira.dsBandeira,
                    competencia = t.dtaRecebimentoEfetivo != null ? t.dtaRecebimentoEfetivo.Value : t.dtaRecebimento,
                    valorDescontado = t.valorDescontado,
                    valorParcelaBruta = t.valorParcelaBruta,
                    idExtrato = t.idExtrato
                }).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();

                List<ConciliacaoRelatorios> rRecebimentoAjuste = queryTbRecebimentoAjuste.Select(t => new ConciliacaoRelatorios
                {
                    tipo = "A",
                    adquirente = t.tbBandeira.tbAdquirente.nmAdquirente,
                    bandeira = t.tbBandeira.dsBandeira,
                    competencia = t.dtAjuste,
                    vlAjuste = t.vlAjuste
                }).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();

                List<ConciliacaoRelatorios> rRecebimentoExtrato = queryExtrato.Select(t => new ConciliacaoRelatorios
                {
                    tipo = "E",
                    adquirente = GatewayTbExtrato._db.tbBancoParametro.Where(p => p.dsMemo.Equals(t.dsDocumento))
                                                                                                 .Where(p => p.cdBanco.Equals(t.tbContaCorrente.cdBanco))
                                                                                                 .Select(p => p.tbAdquirente.nmAdquirente.ToUpper())
                                                                                                 .FirstOrDefault() ?? "",
                    bandeira = GatewayTbExtrato._db.tbBancoParametro.
                                                    Where(p => p.cdBanco.Equals(t.tbContaCorrente.cdBanco)
                                                    && p.dsMemo.Equals(t.dsDocumento)).Select(p => p.tbBandeira.dsBandeira ?? "Indefinida"
                                                    ).FirstOrDefault() ?? "Indefinida",
                    extratoBancario = t.vlMovimento ?? new decimal(0.0),
                    competencia = t.dtExtrato
                }).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();

                List<ConciliacaoRelatorios> listaCompleta = rRecebimentoParcela.Concat(rRecebimentoAjuste).Concat(rRecebimentoExtrato).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();

                List<dynamic> listaFinal = listaCompleta.GroupBy(t => t.competencia)
                                                           .Select(t => new
                                                           {
                                                               competencia = t.Key.ToShortDateString(),
                                                               //bandeira = t.Key.bandeira,
                                                               //competencia = t.Key.competencia,
                                                               taxaMedia = t.Select(x => (x.valorDescontado * new decimal(100.0))/(x.valorParcelaBruta > new decimal(0.0) ? x.valorParcelaBruta : new decimal(1.0) )).Sum() / (t.Count() > 0 ? t.Count() : 1),
                                                               vendas = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorParcelaBruta),
                                                               taxaADM = new decimal(0.0),
                                                               ajustesCredito = t.Where(x => x.tipo.Equals("A")).Where(x => x.vlAjuste > new decimal(0.0)).Sum(x => x.vlAjuste),
                                                               ajustesDebito = t.Where(x => x.tipo.Equals("A")).Where(x => x.vlAjuste < new decimal(0.0)).Sum(x => x.vlAjuste),
                                                               valorLiquido = t.Sum(x => x.valorLiquido),
                                                               extratoBancario = t.Where(x => x.tipo.Equals("E")).Sum(x => x.extratoBancario),
                                                               diferenca = new decimal(0.0),                                                               

                                                               adquirentes = t.GroupBy(c => c.adquirente)
                                                                               .OrderBy(c => c.Key)
                                                                               .Select(c => new {
                                                                                   adquirente = c.Key,
                                                                                   taxaMedia = c.Select(x => (x.valorDescontado * new decimal(100.0)) / (x.valorParcelaBruta > new decimal(0.0) ? x.valorParcelaBruta : new decimal(1.0))).Sum() / (c.Count() > 0 ? c.Count() : 1),
                                                                                   vendas = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorParcelaBruta),
                                                                                   taxaADM = new decimal(0.0),
                                                                                   ajustesCredito = c.Where(x => x.tipo.Equals("A")).Where(x => x.vlAjuste > new decimal(0.0)).Sum(x => x.vlAjuste),
                                                                                   ajustesDebito = c.Where(x => x.tipo.Equals("A")).Where(x => x.vlAjuste < new decimal(0.0)).Sum(x => x.vlAjuste),
                                                                                   valorLiquido = c.Sum(x => x.valorLiquido),
                                                                                   extratoBancario = c.Where(x => x.tipo.Equals("E")).Sum(x => x.extratoBancario),
                                                                                   diferenca = new decimal(0.0),
                                                                                   
                                                                                   bandeiras = c.GroupBy(b => b.bandeira)
                                                                                                .OrderBy(b => b.Key)
                                                                                                .Select(b => new {
                                                                                                    bandeira = b.Key,
                                                                                                    taxaMedia = b.Select(x => (x.valorDescontado * new decimal(100.0)) / (x.valorParcelaBruta > new decimal(0.0) ? x.valorParcelaBruta : new decimal(1.0))).Sum() / (b.Count() > 0 ? b.Count() : 1),
                                                                                                    vendas = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorParcelaBruta),
                                                                                                    taxaADM = new decimal(0.0),
                                                                                                    ajustesCredito = b.Where(x => x.tipo.Equals("A")).Where(x => x.vlAjuste > new decimal(0.0)).Sum(x => x.vlAjuste),
                                                                                                    ajustesDebito = b.Where(x => x.tipo.Equals("A")).Where(x => x.vlAjuste < new decimal(0.0)).Sum(x => x.vlAjuste),
                                                                                                    valorLiquido = b.Sum(x => x.valorLiquido),
                                                                                                    extratoBancario = b.Where(x => x.tipo.Equals("E")).Sum(x => x.extratoBancario),
                                                                                                    diferenca = new decimal(0.0),                                                                                                    
                                                                                                }).ToList<dynamic>()
                                                                               }).ToList<dynamic>(),

                                                           })
                                                           .ToList<dynamic>();
                //for(int n = 0; n < listCompleta.Count; n++)
                //{
                //    ConciliacaoBancaria item1 = listCompleta[n];
                //    ConciliacaoBancaria item1 = listCompleta[n];
                //    if ()
                //}


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
                throw new Exception(e.Message);
            }
        }

    }

}
