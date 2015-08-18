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
                    ExtratoBancario = item.Tipo == 'E' ? item.Ids : new List<Int32>(),
                    RecebimentosParcela = item.Tipo == 'R' ? item.Ids : new List<Int32>(),
                    Adquirente = item.Adquirente,
                    Data = item.Data,
                    Valores = item.Valores,
                    Conciliado = TIPO_CONCILIADO.NAO_CONCILIADO,
                    Conta = item.Tipo == 'R' ? null :
                    new
                    {
                        item.Conta.cdContaCorrente,
                        item.Conta.nrAgencia,
                        item.Conta.nrConta,
                        banco = new
                        {
                            Codigo = item.Conta.cdBanco,
                            NomeExtenso = GatewayBancos.Get(item.Conta.cdBanco)
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
            for (var k = 0; k < listaCandidatos.Count - 1; k++)
            {
                ConciliacaoBancaria item1 = listaCandidatos[k];
                ConciliacaoBancaria item2 = listaCandidatos[k + 1];

                // Adquirente: pega somente o nome dela, sem considerar o nome da bandeira (que vem em recebimento parcela)
                string adquirente1 = item1.Tipo == 'E' ? item1.Adquirente : item1.Adquirente.Substring(0, item1.Adquirente.IndexOf(" - "));
                string adquirente2 = item1.Tipo == 'E' ? item2.Adquirente : item2.Adquirente.Substring(0, item2.Adquirente.IndexOf(" - "));

                // Verifica o tipo
                if (item1.Tipo == item2.Tipo ||
                   !adquirente1.Equals(adquirente2) ||
                   item1.Valores.Sum() != item2.Valores.Sum())
                {
                    // item1 não conciliado com ninguém
                    listaNaoConciliado.Add(item1);
                    continue;
                }

                // CONCILIADO!
                ConciliacaoBancaria recebimento = null;
                ConciliacaoBancaria movimentacao = null;
                if (item1.Tipo.Equals('E'))
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
                    ExtratoBancario = movimentacao.Ids,
                    RecebimentosParcela = recebimento.Ids,
                    Adquirente = recebimento.Adquirente,
                    Data = recebimento.Data,
                    Valores = recebimento.Valores,
                    Conta = new
                    {
                        movimentacao.Conta.cdContaCorrente,
                        movimentacao.Conta.nrAgencia,
                        movimentacao.Conta.nrConta,
                        banco = new
                        {
                            Codigo = movimentacao.Conta.cdBanco,
                            NomeExtenso = GatewayBancos.Get(movimentacao.Conta.cdBanco)
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
                var queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 1, 0, 0, queryStringRecebimentoParcela);
                var queryExtrato = GatewayTbExtrato.getQuery(0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 1, 0, 0, queryStringExtrato);


                // CONCILIAÇÃO BANCÁRIA

                List<ConciliacaoBancaria> recebimentosParcela = queryRecebimentoParcela
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = 'R', // recebimento
                                                                        Ids = new List<Int32>() { r.idRecebimento },
                                                                        Data = r.dtaRecebimento,
                                                                        Valores = new List<decimal>() { r.valorParcelaLiquida ?? new decimal(0.0) }, // r.valorParcelaBruta - r.valorDescontado
                                                                        Adquirente = r.Recebimento.BandeiraPos.Operadora.nmOperadora.ToUpper() + " - " +
                                                                                     r.Recebimento.BandeiraPos.desBandeira.ToUpper(),
                                                                        Conta = null,
                                                                    }).ToList<ConciliacaoBancaria>();
                List<ConciliacaoBancaria> extratoBancario = queryExtrato
                                                        .Select(e => new ConciliacaoBancaria
                                                        {
                                                            Tipo = 'E', // extrato
                                                            Ids = new List<Int32>() { e.idExtrato },
                                                            Data = e.dtExtrato,
                                                            Valores = new List<decimal>() { e.vlMovimento ?? new decimal(0.0) },
                                                            Adquirente = _db.tbBancoParametro.Where(p => p.dsMemo.Equals(e.dsDocumento))
                                                                                             .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                                             .Select(p => p.tbAdquirente.nmAdquirente.ToUpper() ?? e.dsDocumento)
                                                                                             .FirstOrDefault() ?? e.dsDocumento,
                                                            Conta = new tbContaCorrente
                                                            {
                                                                cdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                                                                nrAgencia = e.tbContaCorrente.nrAgencia,
                                                                nrConta = e.tbContaCorrente.nrConta,
                                                                cdBanco = e.tbContaCorrente.cdBanco
                                                            },
                                                        }).ToList<ConciliacaoBancaria>();
  
                if (recebimentosParcela.Count == 0 || extratoBancario.Count > 0)
                {
                    // NÃO HÁ O QUE CONCILIAR!
                    if (recebimentosParcela.Count > 0)
                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcela);
                    else if (extratoBancario.Count == 0)
                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                }
                else
                {
                    List<ConciliacaoBancaria> listaCandidatos = new List<ConciliacaoBancaria>();
                    List<ConciliacaoBancaria> listaNaoConciliado = new List<ConciliacaoBancaria>();

                    // PASSO 1) Concilia diretamente RecebimentoParcela com extratos

                    // Concatena as duas listas, ordenando por data
                    listaCandidatos = recebimentosParcela.Concat(extratoBancario)
                                          .OrderByDescending(c => c.Data)
                                          .ThenBy(c => c.Adquirente)
                                          .ThenBy(c => c.Valores)
                                          .ToList<ConciliacaoBancaria>();

                    // Faz a conciliação
                    Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado);

                    // Tem elementos não conciliados?
                    if (listaNaoConciliado.Count > 0)
                    {
                        // PASSO 2) => SEM OS ELEMENTOS DO PASSO 1, FAZER O GROUP BY DE ADQUIRENTE E BANDEIRA NO RECEBIMENTO PARCELA
                        recebimentosParcela = queryRecebimentoParcela
                                                        .Where(r => listaNaoConciliado.Any(p => p.Tipo == 'R' && p.Ids.Contains(r.idRecebimento)))
                                                        .GroupBy(r => new { r.dtaRecebimento, r.Recebimento.BandeiraPos })
                                                        .Select(r => new ConciliacaoBancaria
                                                        {
                                                            Tipo = 'R', // recebimento
                                                            Ids = r.Select(x => x.idRecebimento).ToList<Int32>(),
                                                            Data = r.Key.dtaRecebimento,
                                                            Valores = r.Select(x => x.valorParcelaLiquida ?? new decimal(0.0)).ToList<decimal>(),
                                                            Adquirente = r.Key.BandeiraPos.Operadora.nmOperadora.ToUpper() + " - " +
                                                                         r.Key.BandeiraPos.desBandeira.ToUpper(),
                                                            Conta = null,
                                                        }).ToList<ConciliacaoBancaria>();

                        extratoBancario = queryExtrato
                                                .Where(r => listaNaoConciliado.Any(p => p.Tipo == 'E' && p.Ids.Contains(r.idExtrato)))
                                                .Select(e => new ConciliacaoBancaria
                                                {
                                                    Tipo = 'E', // extrato
                                                    Ids = new List<Int32>() { e.idExtrato },
                                                    Data = e.dtExtrato,
                                                    Valores = new List<decimal>() { e.vlMovimento ?? new decimal(0.0) },
                                                    Adquirente = _db.tbBancoParametro.Where(p => p.dsMemo.Equals(e.dsDocumento))
                                                                                        .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                                        .Select(p => p.tbAdquirente.nmAdquirente.ToUpper() ?? e.dsDocumento)
                                                                                        .FirstOrDefault() ?? e.dsDocumento,
                                                    Conta = new tbContaCorrente
                                                    {
                                                        cdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                                                        nrAgencia = e.tbContaCorrente.nrAgencia,
                                                        nrConta = e.tbContaCorrente.nrConta,
                                                        cdBanco = e.tbContaCorrente.cdBanco
                                                    },
                                                }).ToList<ConciliacaoBancaria>();


                        if (recebimentosParcela.Count == 0 || extratoBancario.Count > 0)
                        {
                            // NÃO HÁ MAIS O QUE CONCILIAR!
                            if (recebimentosParcela.Count > 0)
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcela);
                            else if (extratoBancario.Count == 0)
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                        }
                        else
                        {

                            // Concatena as duas listas, ordenando por data
                            listaCandidatos = recebimentosParcela.Concat(extratoBancario)
                                              .OrderByDescending(c => c.Data)
                                              .ThenBy(c => c.Adquirente)
                                              .ThenBy(c => c.Valores)
                                              .ToList<ConciliacaoBancaria>();
                            listaNaoConciliado.Clear();

                            // Faz a conciliação
                            Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado);


                            // Ainda tem elementos não conciliados?
                            if (listaNaoConciliado.Count > 0)
                            {
                                // PASSO 3) AGRUPA TAMBÉM O EXTRATO BANCÁRIO 
                                // .........


                                //if (listaNaoConciliado.Count > 0)
                                // PASSO 4) Os elementos que sobraram, foram os não conciliados
                                recebimentosParcela = queryRecebimentoParcela
                                                            .Where(r => listaNaoConciliado.Any(p => p.Tipo == 'R' && p.Ids.Contains(r.idRecebimento)))
                                                            .Select(r => new ConciliacaoBancaria
                                                            {
                                                                Tipo = 'R', // recebimento
                                                                Ids = new List<Int32>() { r.idRecebimento },
                                                                Data = r.dtaRecebimento,
                                                                Valores = new List<decimal>() { r.valorParcelaLiquida ?? new decimal(0.0) },
                                                                Adquirente = r.Recebimento.BandeiraPos.Operadora.nmOperadora.ToUpper() + " - " +
                                                                                         r.Recebimento.BandeiraPos.desBandeira.ToUpper(),
                                                                Conta = null,
                                                            }).ToList<ConciliacaoBancaria>();

                                extratoBancario = queryExtrato
                                                        .Where(r => listaNaoConciliado.Any(p => p.Tipo == 'E' && p.Ids.Contains(r.idExtrato)))
                                                        .Select(e => new ConciliacaoBancaria
                                                        {
                                                            Tipo = 'E', // extrato
                                                            Ids = new List<Int32>() { e.idExtrato },
                                                            Data = e.dtExtrato,
                                                            Valores = new List<decimal>() { e.vlMovimento ?? new decimal(0.0) },
                                                            Adquirente = _db.tbBancoParametro.Where(p => p.dsMemo.Equals(e.dsDocumento))
                                                                                                .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                                                .Select(p => p.tbAdquirente.nmAdquirente.ToUpper() ?? e.dsDocumento)
                                                                                                .FirstOrDefault() ?? e.dsDocumento,
                                                            Conta = new tbContaCorrente
                                                            {
                                                                cdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                                                                nrAgencia = e.tbContaCorrente.nrAgencia,
                                                                nrConta = e.tbContaCorrente.nrConta,
                                                                cdBanco = e.tbContaCorrente.cdBanco
                                                            },
                                                        }).ToList<ConciliacaoBancaria>();
                                // Adiciona eles como não conciliados
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcela);
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                            }
                        }
                        // POR FIM, agrupa ordena por data
                        CollectionConciliacaoBancaria = CollectionConciliacaoBancaria.OrderBy(c => c.Data).ToList<dynamic>();
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
