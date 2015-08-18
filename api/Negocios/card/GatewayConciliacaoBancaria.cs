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

        public enum TIPO_CONCILIADO
        {
            CONCILIADO = 1,
            NAO_CONCILIADO = 2
        };

        private static string TIPO_EXTRATO = "E";
        private static string TIPO_RECEBIMENTO = "R";


        /// <summary>
        /// Adiciona elementos da listaNaoConciliado na lista
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos da conciliação</param>
        /// <param name="listaNaoConciliado">Lista que contém elementos não conciliados</param>
        /// <returns></returns>
        private static void adicionaElementosNaoConciliadosNaLista(List<dynamic> listaConciliacao, 
                                                                   List<ConciliacaoBancaria> listaNaoConciliado)
        {
            foreach (var item in listaNaoConciliado)
            {
                listaConciliacao.Add(new
                {
                    Conciliado = TIPO_CONCILIADO.NAO_CONCILIADO,
                    ExtratoBancario = item.Tipo != TIPO_EXTRATO ? null : new
                    {
                        Ids = item.Ids,
                        Valores = item.Valores
                    },
                    RecebimentosParcela = item.Tipo != TIPO_RECEBIMENTO ? null : new
                    {
                        Ids = item.Ids,
                        Valores = item.Valores
                    },
                    Adquirente = item.Adquirente,
                    Bandeira = item.Bandeira,
                    Data = item.Data,
                    ValorTotal = item.Valores.Sum(),
                    Conta = item.Tipo == TIPO_RECEBIMENTO ? null :
                    new
                    {
                        cdContaCorrente = item.Conta.CdContaCorrente,
                        nrAgencia = item.Conta.NrAgencia,
                        nrConta = item.Conta.NrConta,
                        banco = new
                        {
                            Codigo = item.Conta.CdBanco,
                            NomeExtenso = GatewayBancos.Get(item.Conta.CdBanco)
                        }
                    },
                });
            }
        }




        /// <summary>
        /// Concilia
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos candidatos à conciliação</param>
        /// <param name="listaCandidatos">Lista que recebe os itens da conciliação bancária</param>
        /// <param name="listaNaoConciliado">Lista que receberá os elementos candidatos não conciliados</param>
        /// <returns></returns>
        private static void Concilia(List<dynamic> listaConciliacao, 
                                     List<ConciliacaoBancaria> listaCandidatos, 
                                     List<ConciliacaoBancaria> listaNaoConciliado)
        {
            // CONCILIA ....
            for (var k = 0; k < listaCandidatos.Count; k++)
            {
                ConciliacaoBancaria item1 = listaCandidatos[k];
                ConciliacaoBancaria item2 = k < listaCandidatos.Count - 1 ? listaCandidatos[k + 1] : null;


                // Verifica se está conciliado
                if (item2 == null || item1.Tipo.Equals(item2.Tipo) ||
                   !item1.Adquirente.Equals(item1.Adquirente) ||
                   item1.Valores.Sum() != item2.Valores.Sum())
                {
                    // item1 não conciliado com ninguém
                    listaNaoConciliado.Add(item1);
                    continue;
                }

                // CONCILIADO!
                k++;
                ConciliacaoBancaria recebimento = null;
                ConciliacaoBancaria movimentacao = null;
                if (item1.Tipo.Equals(TIPO_EXTRATO))
                {
                    movimentacao = item1;
                    recebimento = item2;
                }
                else
                {
                    recebimento = item1;
                    movimentacao = item2;
                }

                // Adiciona
                listaConciliacao.Add(new
                {
                    Conciliado = TIPO_CONCILIADO.CONCILIADO,
                    ExtratoBancario = new
                    {
                        Ids = movimentacao.Ids,
                        Valores = movimentacao.Valores
                    },
                    RecebimentosParcela = new
                    {
                        Ids = recebimento.Ids,
                        Valores = recebimento.Valores
                    },
                    Adquirente = movimentacao.Adquirente,
                    Bandeira = recebimento.Bandeira,
                    Data = recebimento.Data,
                    ValorTotal = recebimento.Valores.Sum(),
                    Conta = new
                    {
                        cdContaCorrente = movimentacao.Conta.CdContaCorrente,
                        nrAgencia = movimentacao.Conta.NrAgencia,
                        nrConta = movimentacao.Conta.NrConta,
                        banco = new
                        {
                            Codigo = movimentacao.Conta.CdBanco,
                            NomeExtenso = GatewayBancos.Get(movimentacao.Conta.CdBanco)
                        }
                    },
                });
            }
        }


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
                var queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);
                var queryExtrato = GatewayTbExtrato.getQuery(0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, 0, 0, queryStringExtrato);


                // CONCILIAÇÃO BANCÁRIA
                List<ConciliacaoBancaria> recebimentosParcela = queryRecebimentoParcela
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Ids = new List<Int32>() { r.idRecebimento },
                                                                        Data = r.dtaRecebimento,
                                                                        Valores = new List<decimal>() { r.valorParcelaLiquida ?? new decimal(0.0) }, // r.valorParcelaBruta - r.valorDescontado
                                                                        Adquirente = r.Recebimento.BandeiraPos.Operadora.nmOperadora.ToUpper(),
                                                                        Bandeira = r.Recebimento.BandeiraPos.desBandeira.ToUpper(),
                                                                        //Conta = null,
                                                                    }).ToList<ConciliacaoBancaria>();
                List<ConciliacaoBancaria> extratoBancario = queryExtrato
                                                        // Só considera os extratos que tem adquirente associada
                                                        .Where( e => GatewayTbExtrato._db.tbBancoParametro
                                                                                             .Where(p => p.cdAdquirente != null )
                                                                                             .Where(p => p.dsMemo.Equals(e.dsDocumento))
                                                                                             .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                                             .Count() > 0)
                                                        .Select(e => new ConciliacaoBancaria
                                                        {
                                                            Tipo = TIPO_EXTRATO, // extrato
                                                            Ids = new List<Int32>() { e.idExtrato },
                                                            Data = e.dtExtrato,
                                                            Valores = new List<decimal>() { e.vlMovimento ?? new decimal(0.0) },
                                                            Adquirente = GatewayTbExtrato._db.tbBancoParametro.Where(p => p.dsMemo.Equals(e.dsDocumento))
                                                                                             .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                                             .Select(p => p.tbAdquirente.nmAdquirente.ToUpper())
                                                                                             .FirstOrDefault() ?? "",
                                                            Conta = new ConciliacaoBancaria.ConciliacaoConta
                                                            {
                                                                CdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                                                                NrAgencia = e.tbContaCorrente.nrAgencia,
                                                                NrConta = e.tbContaCorrente.nrConta,
                                                                CdBanco = e.tbContaCorrente.cdBanco
                                                            },
                                                        }).ToList<ConciliacaoBancaria>();
  
                if (recebimentosParcela.Count == 0 || extratoBancario.Count == 0)
                {
                    // NÃO HÁ O QUE CONCILIAR!
                    if (recebimentosParcela.Count > 0)
                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcela);
                    else if (extratoBancario.Count > 0)
                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                }
                else
                {
                    List<ConciliacaoBancaria> listaCandidatos = new List<ConciliacaoBancaria>();
                    List<ConciliacaoBancaria> listaNaoConciliado = new List<ConciliacaoBancaria>();

                    // PASSO 1) Concilia diretamente RecebimentoParcela com tbExtrato

                    // Concatena as duas listas, ordenando por data
                    listaCandidatos = recebimentosParcela.Concat<ConciliacaoBancaria>(extratoBancario)
                                          .OrderBy(c => c.Data.Year)
                                          .ThenBy(c => c.Data.Month)
                                          .ThenBy(c => c.Data.Day)
                                          .ThenBy(c => c.Adquirente)
                                          .ThenBy(c => c.Valores.Sum())
                                          .ToList<ConciliacaoBancaria>();

                    // Faz a conciliação
                    Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado);

                    // Tem elementos não conciliados?
                    if (listaNaoConciliado.Count > 0)
                    {
                        // REMOVE OS ELEMENTOS JÁ CONCILIADOS
                        recebimentosParcela = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                    .OrderBy(e => e.Data)
                                                                    .ThenBy(e => e.Adquirente)
                                                                    .ToList<ConciliacaoBancaria>();

                        extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                .OrderBy(e => e.Data)
                                                                .ThenBy(e => e.Adquirente)
                                                                .ToList<ConciliacaoBancaria>();

                        if (recebimentosParcela.Count == 0 || extratoBancario.Count == 0)
                        {
                            // NÃO HÁ MAIS O QUE CONCILIAR!
                            if (recebimentosParcela.Count > 0)
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcela);
                            else if (extratoBancario.Count > 0)
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                        }else {

                            // PASSO 2) => FAZER O GROUP BY DE ADQUIRENTE E BANDEIRA NO RECEBIMENTO PARCELA
                            List<ConciliacaoBancaria> recebimentosParcelaAgrupados = recebimentosParcela
                                                            .GroupBy(r => new { r.Data, r.Adquirente, r.Bandeira })
                                                            .OrderBy(r => r.Key.Data)
                                                            .ThenBy(r => r.Key.Adquirente)
                                                            .ThenBy(r => r.Key.Bandeira)
                                                            .Select(r => new ConciliacaoBancaria
                                                            {
                                                                Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                Ids = r.Select(x => x.Ids.Sum()).ToList<Int32>(),
                                                                Data = r.Key.Data,
                                                                Valores = r.Select(x => x.Valores.Sum()).ToList<decimal>(),
                                                                Adquirente = r.Key.Adquirente,
                                                                Bandeira = r.Key.Bandeira,
                                                                //Conta = null,
                                                            }).ToList<ConciliacaoBancaria>();


                            // Concatena as duas listas, ordenando por data
                            listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                              .OrderBy(c => c.Data.Year)
                                              .ThenBy(c => c.Data.Month)
                                              .ThenBy(c => c.Data.Day)
                                              .ThenBy(c => c.Adquirente)
                                              .ThenBy(c => c.Valores.Sum())
                                              .ToList<ConciliacaoBancaria>();

                            // Faz a conciliação
                            listaNaoConciliado.Clear();
                            Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado);


                            // Ainda tem elementos não conciliados?
                            if (listaNaoConciliado.Count > 0)
                            {
                                // REMOVE OS ELEMENTOS JÁ CONCILIADOS
                                recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                        .OrderBy(r => r.Data)
                                                                        .ThenBy(r => r.Adquirente)
                                                                        .ThenBy(r => r.Bandeira)
                                                                        .ToList<ConciliacaoBancaria>();

                                extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                        .OrderBy(e => e.Data)
                                                                        .ThenBy(e => e.Adquirente)
                                                                        .ToList<ConciliacaoBancaria>();

                                if (recebimentosParcelaAgrupados.Count == 0 || extratoBancario.Count == 0)
                                {
                                    // NÃO HÁ MAIS O QUE CONCILIAR!
                                    if (extratoBancario.Count > 0)
                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                    else if (recebimentosParcelaAgrupados.Count > 0)
                                    {
                                        // OBTEM OS ELEMENTOS QUE FORAM AGRUPADOS MAS NÃO FORAM CONCILIADOS
                                        recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Ids.Contains(e.Ids.Sum())))
                                                                        .OrderBy(e => e.Data)
                                                                        .ThenBy(e => e.Adquirente)
                                                                        .ThenBy(e => e.Bandeira)
                                                                        .ToList<ConciliacaoBancaria>();
                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcela);
                                    }
                                }
                                else
                                {

                                    // PASSO 3) AGRUPA TAMBÉM O EXTRATO BANCÁRIO 
                                    List<ConciliacaoBancaria> extratoAgrupado = extratoBancario
                                                                .GroupBy(r => new { r.Data, r.Adquirente/*, r.Conta*/ })
                                                                .OrderBy(r => r.Key.Data)
                                                                .ThenBy(r => r.Key.Adquirente)
                                                                .Select(r => new ConciliacaoBancaria
                                                                {
                                                                    Tipo = TIPO_EXTRATO,
                                                                    Ids = r.Select(x => x.Ids.Sum()).ToList<Int32>(),
                                                                    Data = r.Key.Data,
                                                                    Valores = r.Select(x => x.Valores.Sum()).ToList<decimal>(),
                                                                    Adquirente = r.Key.Adquirente,
                                                                    //Bandeira = r.Key.Bandeira,
                                                                    //Conta = r.Key.Conta,
                                                                }).ToList<ConciliacaoBancaria>();

                                    // Concatena as duas listas, ordenando por data
                                    listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoAgrupado)
                                                      .OrderBy(c => c.Data.Year)
                                                      .ThenBy(c => c.Data.Month)
                                                      .ThenBy(c => c.Data.Day)
                                                      .ThenBy(c => c.Adquirente)
                                                      .ThenBy(c => c.Valores.Sum())
                                                      .ToList<ConciliacaoBancaria>();

                                    // Faz a conciliação
                                    listaNaoConciliado.Clear();
                                    Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado);

                                    if (listaNaoConciliado.Count > 0)
                                    {
                                        // REMOVE OS ELEMENTOS JÁ CONCILIADOS
                                        recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                                .OrderBy(r => r.Data)
                                                                                .ThenBy(r => r.Adquirente)
                                                                                .ThenBy(r => r.Bandeira)
                                                                                .ToList<ConciliacaoBancaria>();

                                        extratoAgrupado = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                                .OrderBy(e => e.Data)
                                                                                .ThenBy(e => e.Adquirente)
                                                                                .ToList<ConciliacaoBancaria>();

                                        // PASSO 4) Adiciona os elementos que sobraram como não conciliados
                                        recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Ids.Contains(e.Ids.Sum())))
                                                                        .OrderBy(e => e.Data)
                                                                        .ThenBy(e => e.Adquirente)
                                                                        .ThenBy(e => e.Bandeira)
                                                                        .ToList<ConciliacaoBancaria>();

                                        extratoBancario = extratoBancario.Where(e => extratoAgrupado.Any(p => p.Ids.Contains(e.Ids.Sum())))
                                                                       .OrderBy(e => e.Data)
                                                                       .ThenBy(e => e.Adquirente)
                                                                       .ToList<ConciliacaoBancaria>();

                                        // Adiciona
                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcela);
                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                    }
                                }
                            }
                        }
                        // POR FIM, ordena
                        CollectionConciliacaoBancaria = CollectionConciliacaoBancaria
                                                                    .OrderBy(c => c.Data.Year)
                                                                    .ThenBy(c => c.Data.Month)
                                                                    .ThenBy(c => c.Data.Day)
                                                                    .ThenBy(c => c.Adquirente)
                                                                    .ThenBy(c => c.ValorTotal)
                                                                    .ToList<dynamic>();
                    }
                }

                // FILTRO DE TIPO ?
                if (queryString.TryGetValue("" + (int)CAMPOS.TIPO, out outValue))
                {
                    TIPO_CONCILIADO tipo = (TIPO_CONCILIADO)Convert.ToInt16(queryString["" + (int)CAMPOS.TIPO]);
                    if(tipo.Equals(TIPO_CONCILIADO.CONCILIADO))
                        CollectionConciliacaoBancaria = CollectionConciliacaoBancaria
                                                                    .Where(c => c.Conciliado == TIPO_CONCILIADO.CONCILIADO)
                                                                    .ToList<dynamic>();
                    else if (tipo.Equals(TIPO_CONCILIADO.NAO_CONCILIADO))
                        CollectionConciliacaoBancaria = CollectionConciliacaoBancaria
                                                                    .Where(c => c.Conciliado == TIPO_CONCILIADO.NAO_CONCILIADO)
                                                                    .ToList<dynamic>();
                }

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionConciliacaoBancaria.Count;


                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionConciliacaoBancaria = CollectionConciliacaoBancaria.Skip(skipRows)
                                                                                 .Take(pageSize)
                                                                                 .ToList<dynamic>();
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionConciliacaoBancaria;

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
