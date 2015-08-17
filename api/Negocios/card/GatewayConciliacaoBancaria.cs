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

namespace api.Negocios.Card
{
    public class GatewayConciliacaoBancaria
    {

        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoBancaria()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100, 
            TIPO = 101,  // 1 : CONCILIADO, 2 : NÃO-CONCILIADO
        };


        /// <summary>
        /// Retorna 
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionConciliacaoBancaria = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                Dictionary<string, string> queryStringExtrato = new Dictionary<string, string>();
                // DATA
                if (!queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                    throw new Exception("A conciliação bancária só é realizada para um determinado intervalo de data");
                
                string data = queryString["" + (int)CAMPOS.DATA];
                queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, data);
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, data);
                
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo != 0)
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (CnpjEmpresa != "")
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.NU_CNPJ, CnpjEmpresa);
                }
                // PARA O EXTRATO, SÓ CONSIDERA OS TIPO CRÉDITO
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DSTIPO, OFXSharp.OFXTransactionType.CREDIT.ToString());
                
                // OBTÉM AS QUERIES
                var queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 1, 0, 0, queryStringRecebimentoParcela);
                var queryExtrato = GatewayTbExtrato.getQuery(0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 1, 0, 0, queryStringExtrato);


                // CONCILIAÇÃO BANCÁRIA

                List<ConciliacaoBancaria> recebimentosParcela = queryRecebimentoParcela
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = 'R', // recebimento
                                                                        Id = r.idRecebimento,
                                                                        Data = r.dtaRecebimento,
                                                                        Valor = r.valorParcelaLiquida ?? new decimal(0.0), // r.valorParcelaBruta - r.valorDescontado
                                                                        Adquirente = r.Recebimento.BandeiraPos.Operadora.nmOperadora,
                                                                    }).ToList<ConciliacaoBancaria>();
                List<ConciliacaoBancaria> extratoBancario = queryExtrato
                                                        .Select(e => new ConciliacaoBancaria
                                                        {
                                                            Tipo = 'E', // extrato
                                                            Id = e.idExtrato,
                                                            Data = e.dtExtrato,
                                                            Valor = e.vlMovimento ?? new decimal(0.0),
                                                            //bandeira = e.dsDocumento,
                                                            Adquirente = _db.tbBancoParametro.Where(p => p.dsMemo.Equals(e.dsDocumento))
                                                                                             .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                                             .Select(p => p.cdAdquirente != null ? p.tbAdquirente.nmAdquirente : e.dsDocumento)
                                                                                             .FirstOrDefault() ?? e.dsDocumento
                                                        }).ToList<ConciliacaoBancaria>();
                // Concatena as duas listas, ordenando por data
                List<ConciliacaoBancaria> list1 = new List<ConciliacaoBancaria>();
                List<ConciliacaoBancaria> list2 = new List<ConciliacaoBancaria>();

                if (recebimentosParcela.Count == 0 || extratoBancario.Count > 0)
                {
                    // NÃO HÁ O QUE CONCILIAR!
                    if(recebimentosParcela.Count > 0) list1 = extratoBancario;
                    else if (extratoBancario.Count == 0) list1 = recebimentosParcela;
                }
                else
                {
                    // PASSO 1) Concilia diretamente RecebimentoParcela com extratos
                    list1 = recebimentosParcela.Concat(extratoBancario)
                                          .OrderByDescending(c => c.Data)
                                          .ThenBy(c => c.Adquirente)
                                          .ThenBy(c => c.Valor)
                                          .ToList<ConciliacaoBancaria>();

                    // CONCILIA ....

                    // PASSO 2) => SEM OS ELEMENTOS DO PASSO 1, FAZER O GROUP BY DE ADQUIRENTE
                    /*list2 = recebimentosParcela.DistinctBy(e => e.Data && e.Adquirente)
                                          .OrderByDescending(c => c.Data)
                                          .ThenBy(c => c.Adquirente)
                                          .ThenBy(c => c.Valor)
                                          .ToList<ConciliacaoBancaria>();*/
                }

                // CONCILIA
                // ....




                if (recebimentosParcela.Count > 0)
                {
                    if (extratoBancario.Count == 0)
                        list2 = recebimentosParcela;
                    else
                        list1 = recebimentosParcela.Concat(extratoBancario)
                                              .OrderByDescending(c => c.Data)
                                              .ThenBy(c => c.Adquirente)
                                              .ThenBy(c => c.Valor)
                                              .ToList<ConciliacaoBancaria>();
                }
                else if (extratoBancario.Count > 0)
                    list1 = extratoBancario;

                // PASSO 3)






                // TOTAL DE REGISTROS
                //retorno.TotalDeRegistros = query.Count();


                // PAGINAÇÃO
                /*int skipRows = (pageNumber - 1) * pageSize;
                 if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                     query = query.Skip(skipRows).Take(pageSize);
                 else
                     pageNumber = 1;*/

                retorno.PaginaAtual = pageNumber;
                 retorno.ItensPorPagina = pageSize;

                 retorno.Registros = CollectionConciliacaoBancaria;

                 return  retorno;
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
