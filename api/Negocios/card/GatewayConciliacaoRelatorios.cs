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
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, data);
                    queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, data);
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, data);
                }
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


                // OBTÉM AS QUERIES                
                IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);
                IQueryable<tbRecebimentoAjuste> queryTbRecebimentoAjuste = GatewayTbRecebimentoAjuste.getQuery(0, (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, 0, 0, queryStringTbRecebimentoAjuste);
                IQueryable<tbExtrato> queryExtrato = GatewayTbExtrato.getQuery(0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, 0, 0, queryStringExtrato);


                List<ConciliacaoBancaria> rps = queryRecebimentoParcela.Select(t => new ConciliacaoBancaria
                {
                    Tipo = "R",
                    Bandeira = t.Recebimento.tbBandeira.dsBandeira,
                    Data = t.dtaRecebimentoEfetivo != null ? t.dtaRecebimentoEfetivo.Value : t.dtaRecebimento,
                     
                }).OrderBy(t => t.Data).ToList<ConciliacaoBancaria>();

                List<ConciliacaoBancaria> ajustes = queryTbRecebimentoAjuste.Select(t => new ConciliacaoBancaria
                {
                    Tipo = "A",
                    Bandeira = t.tbBandeira.dsBandeira,
                    Data = t.dtAjuste,
                    ValorTotal = t.vlAjuste
                }).OrderBy(t => t.Data).ToList<ConciliacaoBancaria>();

                List<ConciliacaoBancaria> listCompleta = rps.Concat(ajustes).OrderBy(t => t.Data).ToList<ConciliacaoBancaria>();

                List<dynamic> listaCompleta2 = listCompleta.GroupBy(t => t.Data)
                                                           .Select(t => new
                                                           {
                                                               competencia = t.Key.Data,
                                                               taxaMedia = t.Select(x => (x.ValorDescontado * new decimal(100.0))/x.ValorBruto).Sum() / t.Count(),
                                                               vendas = t.Where(x => x.Tipo.Equals("R")).Sum(x => x.ValorBruto),
                                                               taxaADM = new decimal(0.0),
                                                               ajustesCredito = t.Where(x => x.Tipo.Equals("A")).Where(x => x.Valor > new decimal(0.0)).Sum(x => x.Valor),
                                                               ajustesDebito = t.Where(x => x.Tipo.Equals("A")).Where(x => x.Valor < new decimal(0.0)).Sum(x => x.Valor),
                                                               valorLiquido = t.Sum(x => x.ValorLiquido),
                                                               extratos = t.Where(x => x.Tipo.Equals("E")).Sum(x => x.Valor),
                                                               diferenca = new decimal(0.0),
                                                               conciliados = t.Where(x => x.Tipo.Equals("R")).Where(x => x.idExtrato != null).Count(),
                                                               totalRps = t.Where(x => x.Tipo.Equals("R")).Count(),
                                                           })
                                                           .ToList<dynamic>();
                //for(int n = 0; n < listCompleta.Count; n++)
                //{
                //    ConciliacaoBancaria item1 = listCompleta[n];
                //    ConciliacaoBancaria item1 = listCompleta[n];
                //    if ()
                //}


                // Ordena
                CollectionConciliacaoRelatorios = CollectionConciliacaoRelatorios
                                                                .OrderBy(c => c.Data.Year)
                                                                .ThenBy(c => c.Data.Month)
                                                                .ThenBy(c => c.Data.Day)
                                                                .ThenBy(c => c.Adquirente)
                                                                .ThenBy(c => c.RecebimentosParcela != null ? c.ValorTotalRecebimento : c.ValorTotalExtrato)
                                                                .ThenBy(c => c.Bandeira)
                                                                .ToList<dynamic>();

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
