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
using System.Data.SqlClient;
using System.Data.Entity;
using System.Configuration;
using api.Negocios.Cliente;
using System.Data;

namespace api.Negocios.Card
{
    public class GatewayConciliacaoBancaria
    {

        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoBancaria()
        {
            // _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100,
            TIPO = 101,  // 1 : CONCILIADO, 2 : PRÉ-CONCILIADO, 3 : NÃO-CONCILIADO
            ID_GRUPO = 102,
            NU_CNPJ = 103,

            // RELACIONAMENTOS
            IDOPERADORA = 200,

            CDADQUIRENTE = 300,

            CDCONTACORRENTE = 400,
        };

        public enum TIPO_CONCILIADO
        {
            CONCILIADO = 1,
            PRE_CONCILIADO = 2,
            NAO_CONCILIADO = 3
        };

        private static string TIPO_EXTRATO = "E";
        private static string TIPO_RECEBIMENTO = "R";
        private static decimal TOLERANCIA = new decimal(0.06); // R$0.06 de tolerância para avaliar pré-conciliação
        private static decimal TOLERANCIA_LOTE = new decimal(0.1);


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
                    Conciliado = (int)TIPO_CONCILIADO.NAO_CONCILIADO,
                    ExtratoBancario = item.Tipo != TIPO_EXTRATO ? null : item.Grupo,
                    RecebimentosParcela = item.Tipo != TIPO_RECEBIMENTO ? null : item.Grupo.OrderBy(x => x.Filial).ThenBy(x => x.Bandeira).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                    Adquirente = item.Adquirente,
                    Bandeira = item.Bandeira,
                    Lote = item.Tipo == TIPO_RECEBIMENTO ? item.Lote : 0,
                    Data = item.Data,
                    Antecipado = item.Antecipado != null && item.Antecipado.Value,
                    DataRecebimentoDiferente = item.DataRecebimentoDiferente != null && item.DataRecebimentoDiferente.Value,
                    ValorTotalBruto = item.Tipo == TIPO_RECEBIMENTO ? /*decimal.Round(*/Convert.ToDecimal(item.ValorTotalBruto)/*, 2)*/ : new decimal(0.0),
                    ValorTotalRecebimento = item.Tipo == TIPO_RECEBIMENTO ? /*decimal.Round(*/Convert.ToDecimal(item.ValorTotal)/*, 2)*/ : new decimal(0.0),
                    ValorTotalExtrato = item.Tipo == TIPO_EXTRATO ? /*decimal.Round(*/Convert.ToDecimal(item.ValorTotal)/*, 2)*/ : new decimal(0.0),
                    Memo = item.Memo,
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
        /// Adiciona elementos na lista ao qual foram encontrados registros que "casaram"
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos da conciliação</param>
        /// <param name="recebimento">ConciliacaoBancaria referente ao lado do ICARD</param>
        /// <param name="movimentacao">ConciliacaoBancaria referente ao lado do extrato</param>
        /// <param name="tipo">CONCILIADO ou PRE-CONCILIADO</param>
        /// <returns></returns>
        private static void adicionaElementosConciliadosNaLista(List<dynamic> listaConciliacao,
                                                     ConciliacaoBancaria recebimento,
                                                     ConciliacaoBancaria movimentacao,
                                                     TIPO_CONCILIADO tipo)
        {
            if (recebimento != null && movimentacao != null)
            {
                // Adiciona
                listaConciliacao.Add(new
                {
                    Conciliado = (int)tipo,
                    ExtratoBancario = movimentacao.Grupo,
                    RecebimentosParcela = recebimento.Grupo.OrderBy(x => x.Filial).ThenBy(x => x.Bandeira).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                    Adquirente = recebimento.Adquirente,
                    Bandeira = recebimento.Bandeira,
                    Lote = recebimento.Lote,
                    Memo = movimentacao.Memo,
                    Data = recebimento.Data,
                    Antecipado = recebimento.Antecipado != null && recebimento.Antecipado.Value,
                    DataRecebimentoDiferente = recebimento.DataRecebimentoDiferente != null && recebimento.DataRecebimentoDiferente.Value,
                    ValorTotalBruto = recebimento.ValorTotalBruto != null ? /*decimal.Round(*/recebimento.ValorTotalBruto.Value/*, 2)*/ : new decimal(0.0),
                    ValorTotalRecebimento = /*decimal.Round(*/recebimento.ValorTotal/*, 2)*/,
                    ValorTotalExtrato = /*decimal.Round(*/movimentacao.ValorTotal/*, 2)*/,
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
        /// Concilia
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos candidatos à conciliação</param>
        /// <param name="listaCandidatos">Lista que recebe os itens da conciliação bancária</param>
        /// <param name="listaNaoConciliado">Lista que receberá os elementos candidatos não conciliados</param>
        /// <param name="adicionaNaListaPreConciliado">Set true se os elementos determinados como conciliados devem ser adicionados na listaConciliacao</param>
        /// <returns></returns>
        private static void Concilia(List<dynamic> listaConciliacao,
                                     List<ConciliacaoBancaria> listaCandidatos,
                                     List<ConciliacaoBancaria> listaNaoConciliado,
                                     bool adicionaNaListaPreConciliado)
        {
            for (var k = 0; k < listaCandidatos.Count; k++)
            {
                ConciliacaoBancaria item1 = listaCandidatos[k];
                ConciliacaoBancaria item2 = k < listaCandidatos.Count - 1 ? listaCandidatos[k + 1] : null;


                // Verifica se está conciliado
                if (item2 == null || item1.Tipo.Equals(item2.Tipo) || // 'E' ou 'R'
                    // Data
                   (item1.Data.Year != item2.Data.Year || item1.Data.Month != item2.Data.Month || item1.Data.Day != item2.Data.Day) ||
                    // Adquirente                                                  
                   (item1.Adquirente != null && item2.Adquirente != null && !item1.Adquirente.Equals("") && !item2.Adquirente.Equals("") && !item1.Adquirente.Equals(item2.Adquirente)) ||
                    // Filial                                                  
                   (item1.Filial != null && item2.Filial != null && !item1.Filial.Equals("") && !item2.Filial.Equals("") && !item1.Filial.Equals(item2.Filial)) ||
                    // Bandeira
                   (item1.Bandeira != null && item2.Bandeira != null && !item1.Bandeira.Equals("") && !item2.Bandeira.Equals("") &&
                    // VISA e CABAL podem ser pré-conciliados
                    (!item1.Bandeira.Equals(item2.Bandeira) &&
                     ((!item1.Bandeira.EndsWith("CABAL") && !item1.Bandeira.EndsWith("VISA")) ||
                      (!item2.Bandeira.EndsWith("CABAL") && !item2.Bandeira.EndsWith("VISA"))
                     )
                    )
                   ) ||
                    // Tipo cartão
                   (item1.TipoCartao != null && item2.TipoCartao != null && !item1.TipoCartao.Equals("") && !item2.TipoCartao.Equals("") && !item1.TipoCartao.Equals(item2.TipoCartao)) ||
                    // Valor
                   Math.Abs(item1.ValorTotal - item2.ValorTotal) > TOLERANCIA)
                {
                    // item1 não conciliado com ninguém
                    listaNaoConciliado.Add(item1);
                    continue;
                }

                // CONCILIADO!
                k++;
                if (adicionaNaListaPreConciliado)
                {
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

                    adicionaElementosConciliadosNaLista(listaConciliacao, recebimento, movimentacao, TIPO_CONCILIADO.PRE_CONCILIADO);
                }
            }
        }




        #region SUBGRUPOS => PARCELAS
        /// <summary>
        /// Obtém as combinações de recebimentosparcelas que resultam em um valor sum
        /// </summary>
        /// <param name="grupo">Recebimentos parcelas agrupados</param>
        /// <param name="sum">Valor almejado para conciliação</param>
        /// <param name="maxCombinacoes">Máximo de combinações desejadas => otimização</param>
        /// <param name="values">Lista que armazenará os elementos de grupo que atinjem o valor sum</param>
        /// <returns></returns>
        private static IEnumerable<List<List<ConciliacaoBancaria.ConciliacaoGrupo>>> GetCombinations(List<ConciliacaoBancaria.ConciliacaoGrupo> grupo,
                                                                                  decimal sum,
                                                                                  List<ConciliacaoBancaria.ConciliacaoGrupo> values = null)
        {
            if (values == null) values = new List<ConciliacaoBancaria.ConciliacaoGrupo>();
            for (Int32 i = 0; i < grupo.Count; i++)
            {
                decimal left = sum - grupo[i].Valor;
                List<List<ConciliacaoBancaria.ConciliacaoGrupo>> vals = new List<List<ConciliacaoBancaria.ConciliacaoGrupo>>();
                List<ConciliacaoBancaria.ConciliacaoGrupo> temp = new List<ConciliacaoBancaria.ConciliacaoGrupo>(values);
                //values.CopyItemsTo(temp);
                temp.Add(grupo[i]);
                vals.Add(temp);

                if (Math.Abs(left) <= TOLERANCIA) yield return vals;
                else
                {
                    List<ConciliacaoBancaria.ConciliacaoGrupo> possible = grupo.Take(i).Where(n => n.Valor <= sum + TOLERANCIA).ToList<ConciliacaoBancaria.ConciliacaoGrupo>();
                    if (possible.Count > 0)
                    {
                        foreach (List<List<ConciliacaoBancaria.ConciliacaoGrupo>> g in GetCombinations(possible, left, temp))
                            yield return g;
                    }
                }
            }
        }



        /// <summary>
        /// Procura para cada movimentação de extrato subgrupos dos agrupamentos de recebimento parcela aos quais os valores "casam"
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos candidatos à conciliação</param>
        /// <param name="recebimentosParcelaAgrupados">Lista que contém os recebimentos parcelas agrupados</param>
        /// <param name="extratoBancario">Lista que contém as movimentações bancárias</param>
        /// <param name="adicionaNaListaPreConciliado">Set true se os elementos determinados como conciliados devem ser adicionados na listaConciliacao</param>
        /// <returns></returns>
        private static void conciliaSubGrupos(List<dynamic> listaConciliacao,
                                              List<ConciliacaoBancaria> recebimentosParcelaAgrupados,
                                              List<ConciliacaoBancaria> extratoBancario,
                                              bool adicionaNaListaPreConciliado)
        {
            #region OBTÉM POSSÍVEIS COMBINAÇÕES DE RECEBIMENTOSPARCELAS PARA CADA EXTRATO
            List<dynamic> gruposExtrato = new List<dynamic>();
            foreach (ConciliacaoBancaria extrato in extratoBancario)
            {
                // Obtém todas as combinações para o extrato
                List<dynamic> grupos = new List<dynamic>();
                foreach (ConciliacaoBancaria recebimento in recebimentosParcelaAgrupados)
                {
                    // Valor total dos recebimentos devem ser igual ou superior ao do extrato para ser uma combinação
                    if (recebimento.ValorTotal < extrato.ValorTotal) continue;

                    // Só avalia os de mesma adquirente, bandeira, tipocartao, filial e data
                    if ((extrato.Adquirente.Equals("") || recebimento.Adquirente.Equals(extrato.Adquirente)) &&
                        recebimento.Data.Year == extrato.Data.Year &&
                        recebimento.Data.Month == extrato.Data.Month &&
                        recebimento.Data.Day == extrato.Data.Day &&
                        (extrato.Bandeira.Equals("") || extrato.Bandeira.Equals(recebimento.Bandeira)) &&
                        (extrato.TipoCartao.Equals("") || extrato.TipoCartao.Equals(recebimento.TipoCartao)) &&
                        (extrato.Filial.Equals("") || extrato.Filial.Equals(recebimento.Filial)))
                    {
                        // Pega somente os elementos que o valor é inferior ao total da movimentação bancária
                        List<ConciliacaoBancaria.ConciliacaoGrupo> gs = recebimento.Grupo.Where(g => g.Valor <= extrato.ValorTotal + TOLERANCIA)
                            /*.OrderByDescending(g => g.Valor)*/
                                                                                         .OrderBy(g => Guid.NewGuid()).Take(20) // OTIMIZAÇÃO: encontrar a soma dentro de um grupo é conhecido como Subset Sum Problem, que é um problema NP. Em virtude disso, um conjunto com muitos elementos pode gerar um processamento muito longo e, por isso, foi escolhido apenas os 20 maiores valores para tentar "casar" com o valor total
                                                                                         .ToList<ConciliacaoBancaria.ConciliacaoGrupo>();
                        decimal sumGrupo = gs.Select(g => g.Valor).Sum();
                        if (sumGrupo >= extrato.ValorTotal - TOLERANCIA)
                        {
                            foreach (List<List<ConciliacaoBancaria.ConciliacaoGrupo>> g in GetCombinations(gs, extrato.ValorTotal))
                            //foreach (ConciliacaoBancaria.ConciliacaoGrupo[] item in KnapsackConciliacaoBancaria.MatchTotalConciliacaoBancaria(recebimento.Grupo, extrato.ValorTotal, TOLERANCIA))
                            {
                                foreach (List<ConciliacaoBancaria.ConciliacaoGrupo> item in g)
                                {
                                    grupos.Add(new
                                    {
                                        bandeira = recebimento.Bandeira,
                                        adquirente = recebimento.Adquirente,
                                        tipo = recebimento.TipoCartao,
                                        filial = recebimento.Filial,
                                        diferenca = Math.Abs(item.Sum(t => t.Valor) - extrato.ValorTotal),
                                        grupo = item.OrderBy(gp => gp.Bandeira).ThenBy(gp => gp.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>()
                                    });
                                }
                            }
                        }
                    }
                }
                if (grupos.Count > 0)
                {
                    gruposExtrato.Add(new
                    {
                        idExtrato = extrato.Grupo[0].Id,
                        valorExtrato = extrato.ValorTotal,
                        bandeira = extrato.Bandeira,
                        tipo = extrato.TipoCartao,
                        filial = extrato.Filial,
                        recebimento = grupos.OrderBy(t => t.diferenca).ThenBy(t => t.grupo.Count).ToList()
                    });
                }
            }
            #endregion

            if (gruposExtrato.Count > 0)
            {
                #region PROCESSA COMBINAÇÕES E CONCILIA EVITANDO DUPLICIDADES
                // Ordena por total de combinações encontradas
                //gruposExtrato = gruposExtrato.OrderBy(g => g.recebimento.Count).ToList<dynamic>();
                gruposExtrato = gruposExtrato.OrderByDescending(g => g.valorExtrato).ToList<dynamic>();
                //Dictionary<Int32, bool> recebimentosPreConciliados = new Dictionary<Int32, bool>();
                List<RecebimentosParcela.RecebParcela> recebimentosPreConciliados = new List<RecebimentosParcela.RecebParcela>();
                // Com os grupos que combinados resultam no valor do extrato, 
                // usar combinações que não gerem inconsistências e duplicidades
                foreach (var grupoExtrato in gruposExtrato)
                {
                    ConciliacaoBancaria movimentacao = extratoBancario.Where(e => e.Grupo[0].Id == grupoExtrato.idExtrato).FirstOrDefault();

                    foreach (var receb in grupoExtrato.recebimento)
                    {
                        List<ConciliacaoBancaria.ConciliacaoGrupo> grupo = receb.grupo;

                        bool grupovalido = true;

                        #region AVALIA SE CADA ELEMENTO DO GRUPO NÃO FOI ADICIONADO EM UMA COMBINAÇÃO ANTERIOR
                        //bool value = true;
                        foreach (ConciliacaoBancaria.ConciliacaoGrupo g in grupo)
                        {
                            if (recebimentosPreConciliados.Any(r => r.idRecebimento == g.Id && r.numParcela == g.NumParcela.Value))
                            {
                                grupovalido = false;
                                break;
                            }
                        }
                        #endregion

                        if (grupovalido)
                        {
                            #region ADICIONA O GRUPO COMO PRÉ-CONCILIADO COM A MOVIMENTAÇÃO DO EXTRATO CORRENTE

                            #region ADICIONA OS IDS DOS RECEBIMENTOSPARCELA NA LISTA DE PRÉ-CONCILIADOS
                            foreach (ConciliacaoBancaria.ConciliacaoGrupo g in grupo)
                                //recebimentosPreConciliados.Add(g.Id, true);
                                recebimentosPreConciliados.Add(new RecebimentosParcela.RecebParcela(g.Id, g.NumParcela.Value));
                            #endregion

                            #region ADICIONA COMO PRÉ-CONCILIADO
                            string filial = grupo.Select(g => g.Filial).FirstOrDefault();
                            string bandeira = String.Empty;
                            string dsTipoCartao = String.Empty;
                            if (grupo.GroupBy(g => g.Bandeira).Count() == 1)
                                bandeira = grupo.Select(g => g.Bandeira).FirstOrDefault();
                            if (grupo.GroupBy(g => g.TipoCartao).Count() == 1)
                                dsTipoCartao = grupo.Select(g => g.TipoCartao).FirstOrDefault();
                            bool? antecipado = null;
                            if (grupo.GroupBy(g => g.Antecipado).Count() == 1)
                                antecipado = grupo.Select(g => g.Antecipado).FirstOrDefault();

                            ConciliacaoBancaria recebimento = new ConciliacaoBancaria()
                            {
                                Tipo = TIPO_RECEBIMENTO,
                                Data = movimentacao.Data,
                                Grupo = grupo,
                                Adquirente = receb.adquirente,//movimentacao.Adquirente,
                                Bandeira = bandeira,
                                Filial = filial,
                                Antecipado = antecipado,
                                TipoCartao = dsTipoCartao,
                                ValorTotal = grupo.Sum(g => g.Valor),
                                ValorTotalBruto = grupo.Sum(g => g.ValorBruto),
                            };
                            if (adicionaNaListaPreConciliado)
                                adicionaElementosConciliadosNaLista(listaConciliacao, recebimento, movimentacao, TIPO_CONCILIADO.PRE_CONCILIADO);
                            #endregion

                            #region REMOVE A MOVIMENTAÇÃO DE EXTRATO
                            extratoBancario.Remove(movimentacao);
                            #endregion

                            #region REMOVE OS RECEBIMENTOS PARCELAS DO AGRUPAMENTO
                            ConciliacaoBancaria rp = recebimentosParcelaAgrupados.Where(r => r.Grupo.Any(g => g.Id == grupo[0].Id && g.NumParcela.Value == grupo[0].NumParcela.Value)).FirstOrDefault();

                            // O novo grupo é composto pelos elementos que estavam lá e não foram envolvidos na conciliação corrente
                            List<ConciliacaoBancaria.ConciliacaoGrupo> newGrupo = new List<ConciliacaoBancaria.ConciliacaoGrupo>();
                            foreach (ConciliacaoBancaria.ConciliacaoGrupo gp in rp.Grupo)
                            {
                                if (!grupo.Any(g => g.Id == gp.Id && g.NumParcela.Value == gp.NumParcela.Value)) newGrupo.Add(gp);
                            }
                            recebimentosParcelaAgrupados.Remove(rp);

                            bandeira = String.Empty;
                            dsTipoCartao = String.Empty;
                            antecipado = null;
                            if (newGrupo.GroupBy(g => g.Bandeira).Count() == 1)
                                bandeira = newGrupo.Select(g => g.Bandeira).FirstOrDefault();
                            if (newGrupo.GroupBy(g => g.TipoCartao).Count() == 1)
                                dsTipoCartao = newGrupo.Select(g => g.TipoCartao).FirstOrDefault();
                            if (newGrupo.GroupBy(g => g.Antecipado).Count() == 1)
                                antecipado = newGrupo.Select(g => g.Antecipado).FirstOrDefault();

                            // Só "re-adiciona" o grupo modificado caso possua elementos
                            if (newGrupo.Count > 0)
                            {
                                ConciliacaoBancaria newRp = new ConciliacaoBancaria()
                                {
                                    Adquirente = rp.Adquirente,
                                    Tipo = rp.Tipo,
                                    Filial = rp.Filial,
                                    Antecipado = antecipado,
                                    Bandeira = bandeira,//rp.Bandeira,
                                    TipoCartao = dsTipoCartao,//rp.TipoCartao,
                                    Conta = rp.Conta,
                                    Data = rp.Data,
                                    DataVenda = rp.DataVenda,
                                    Grupo = newGrupo.OrderBy(g => g.Bandeira).ThenBy(g => g.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                    Memo = rp.Memo,
                                    ValorTotal = newGrupo.Sum(g => g.Valor),
                                    ValorTotalBruto = newGrupo.Sum(g => g.ValorBruto),
                                };

                                recebimentosParcelaAgrupados.Add(newRp);
                            }
                            #endregion

                            #endregion

                            break; // vai para a próxima movimentação de extrato
                        }
                    }
                }
                #endregion
            }

        }
        #endregion



        #region SUBGRUPOS => LOTE FINANCEIRO
        private static IEnumerable<List<List<ConciliacaoBancaria.ConciliacaoLote>>> GetCombinations(List<ConciliacaoBancaria.ConciliacaoLote> grupo,
                                                                                  decimal sum,
                                                                                  List<ConciliacaoBancaria.ConciliacaoLote> values = null)
        {
            if (values == null) values = new List<ConciliacaoBancaria.ConciliacaoLote>();
            for (Int32 i = 0; i < grupo.Count; i++)
            {
                decimal left = sum - grupo[i].Valor;
                List<List<ConciliacaoBancaria.ConciliacaoLote>> vals = new List<List<ConciliacaoBancaria.ConciliacaoLote>>();
                List<ConciliacaoBancaria.ConciliacaoLote> temp = new List<ConciliacaoBancaria.ConciliacaoLote>(values);
                //values.CopyItemsTo(temp);
                temp.Add(grupo[i]);
                vals.Add(temp);

                if (Math.Abs(left) <= TOLERANCIA_LOTE) yield return vals;
                else
                {
                    List<ConciliacaoBancaria.ConciliacaoLote> possible = grupo.Take(i).Where(n => n.Valor <= sum + TOLERANCIA_LOTE).ToList<ConciliacaoBancaria.ConciliacaoLote>();
                    if (possible.Count > 0)
                    {
                        foreach (List<List<ConciliacaoBancaria.ConciliacaoLote>> g in GetCombinations(possible, left, temp))
                            yield return g;
                    }
                }
            }
        }

        private static void conciliaPorLotes(List<dynamic> listaConciliacao,
                                              List<ConciliacaoBancaria> recebimentosParcelaAgrupados,
                                              List<ConciliacaoBancaria> extratoBancario,
                                              bool adicionaNaListaPreConciliado)
        {
            #region OBTÉM POSSÍVEIS COMBINAÇÕES DE LOTES PARA CADA EXTRATO
            List<dynamic> gruposExtrato = new List<dynamic>();
            foreach (ConciliacaoBancaria extrato in extratoBancario)
            {
                // Obtém todas as combinações para o extrato
                List<dynamic> grupos = new List<dynamic>();

                // Lotes da mesma data de recebimento, filial, adquirente, bandeira e tipo cartão
                IEnumerable<ConciliacaoBancaria> queryRP = recebimentosParcelaAgrupados
                                                                .Where(t => t.Data.Year == extrato.Data.Year)
                                                                .Where(t => t.Data.Month == extrato.Data.Month)
                                                                .Where(t => t.Data.Day == extrato.Data.Day)
                                                                .Where(t => t.ValorTotal <= extrato.ValorTotal);
                if (!extrato.Filial.Equals(""))
                    queryRP = queryRP.Where(t => t.Filial.Equals(extrato.Filial));
                if (!extrato.Adquirente.Equals(""))
                    queryRP = queryRP.Where(t => t.Adquirente.Equals(extrato.Adquirente));
                if (!extrato.Bandeira.Equals(""))
                    queryRP = queryRP.Where(t => t.Bandeira.Equals(extrato.Bandeira));
                if (!extrato.TipoCartao.Equals(""))
                    queryRP = queryRP.Where(t => t.TipoCartao.Equals(extrato.TipoCartao));

                List<ConciliacaoBancaria.ConciliacaoLote> lotes = queryRP.Select(t => new ConciliacaoBancaria.ConciliacaoLote
                                                                    {
                                                                        Bandeira = t.Bandeira,
                                                                        DtRecebimento = t.Data,
                                                                        DtVenda = t.DataVenda,
                                                                        Filial = t.Filial,
                                                                        Lote = t.Lote,
                                                                        Valor = t.ValorTotal
                                                                    })
                                                                    .OrderBy(g => Guid.NewGuid()).Take(20) // OTIMIZAÇÃO: encontrar a soma dentro de um grupo é conhecido como Subset Sum Problem, que é um problema NP. Em virtude disso, um conjunto com muitos elementos pode gerar um processamento muito longo e, por isso, foi escolhido apenas os 20 maiores valores para tentar "casar" com o valor total
                                                                    .ToList<ConciliacaoBancaria.ConciliacaoLote>();

                decimal sumLote = lotes.Select(g => g.Valor).Sum();
                if (sumLote >= extrato.ValorTotal - TOLERANCIA_LOTE)
                {
                    foreach (List<List<ConciliacaoBancaria.ConciliacaoLote>> g in GetCombinations(lotes, extrato.ValorTotal))
                    {
                        foreach (List<ConciliacaoBancaria.ConciliacaoLote> item in g)
                        {
                            grupos.Add(new
                            {
                                diferenca = Math.Abs(item.Sum(t => t.Valor) - extrato.ValorTotal),
                                grupo = item.OrderBy(gp => gp.Bandeira).ThenBy(gp => gp.Valor).ToList<ConciliacaoBancaria.ConciliacaoLote>()
                            });
                        }
                    }

                }
                if (grupos.Count > 0)
                {
                    gruposExtrato.Add(new
                    {
                        idExtrato = extrato.Grupo[0].Id,
                        valorExtrato = extrato.ValorTotal,
                        bandeira = extrato.Bandeira,
                        tipo = extrato.TipoCartao,
                        filial = extrato.Filial,
                        lotes = grupos.OrderBy(t => t.diferenca).ThenBy(t => t.grupo.Count).ToList()
                    });
                }
            }
            #endregion

            if (gruposExtrato.Count > 0)
            {
                #region PROCESSA COMBINAÇÕES E CONCILIA EVITANDO DUPLICIDADES
                // Ordena por total de combinações encontradas, priorizando aqueles que tem bandeira
                gruposExtrato = gruposExtrato.OrderByDescending(g => g.bandeira).ThenByDescending(g => g.tipo).ThenBy(g => g.lotes[0].diferenca).OrderBy(g => g.lotes.Count).ToList<dynamic>();
                List<ConciliacaoBancaria.ConciliacaoLote> lotesPreConciliados = new List<ConciliacaoBancaria.ConciliacaoLote>();
                // Com os grupos que combinados resultam no valor do extrato, 
                // usar combinações que não gerem inconsistências e duplicidades
                foreach (var grupoExtrato in gruposExtrato)
                {
                    ConciliacaoBancaria movimentacao = extratoBancario.Where(e => e.Grupo[0].Id == grupoExtrato.idExtrato).FirstOrDefault();

                    foreach (var lote in grupoExtrato.lotes)
                    {
                        List<ConciliacaoBancaria.ConciliacaoLote> lotes = lote.grupo;

                        bool grupovalido = true;

                        #region AVALIA SE CADA ELEMENTO DO GRUPO NÃO FOI ADICIONADO EM UMA COMBINAÇÃO ANTERIOR
                        foreach (ConciliacaoBancaria.ConciliacaoLote l in lotes)
                        {
                            if (lotesPreConciliados.Any(r => r.Lote == l.Lote && (r.Lote > 0 ||
                                                            (r.Lote == 0 && // se Lote = 0, avalia pelos outros campos
                                                             r.Bandeira.Equals(l.Bandeira) &&
                                                             r.Filial.Equals(l.Filial) &&
                                                             r.Valor == l.Valor &&
                                                             ((r.DtVenda == null && l.DtVenda == null) || (r.DtVenda != null && l.DtVenda != null && r.DtVenda.Value.Equals(l.DtVenda.Value)))))))
                            {
                                grupovalido = false;
                                break;
                            }
                        }
                        #endregion

                        if (grupovalido)
                        {
                            #region ADICIONA O GRUPO COMO PRÉ-CONCILIADO COM A MOVIMENTAÇÃO DO EXTRATO CORRENTE

                            #region ADICIONA OS LOTES NA LISTA DE PRÉ-CONCILIADOS
                            foreach (ConciliacaoBancaria.ConciliacaoLote l in lotes)
                                lotesPreConciliados.Add(l);
                            #endregion

                            // OBTÉM PARCELAS PERTENCENTES AOS LOTES
                            List<ConciliacaoBancaria> parcelas = recebimentosParcelaAgrupados.Where(l => lotes.Any(r => r.Lote == l.Lote && (r.Lote > 0 ||
                                                                                                                    (// se Lote = 0, avalia pelos outros campos
                                                                                                                     r.Bandeira.Equals(l.Bandeira) &&
                                                                                                                     r.Filial.Equals(l.Filial) &&
                                                                                                                     r.Valor == l.ValorTotal &&
                                                                                                                     ((r.DtVenda == null && l.DataVenda == null) || (r.DtVenda != null && l.DataVenda != null && r.DtVenda.Value.Equals(l.DataVenda.Value)))))))
                                                                                             .ToList<ConciliacaoBancaria>();
                            // Cria o objeto recebimento
                            ConciliacaoBancaria recebimento = new ConciliacaoBancaria();
                            // Adiciona parcelas
                            recebimento.Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo>();
                            foreach (ConciliacaoBancaria p in parcelas)
                                recebimento.Grupo = recebimento.Grupo.Concat(p.Grupo).ToList();
                            recebimento.Filial = recebimento.Grupo.GroupBy(t => t.Filial).Count() == 1 ? recebimento.Grupo.Select(t => t.Filial).FirstOrDefault() : "";
                            recebimento.Antecipado = recebimento.Grupo.GroupBy(t => t.Antecipado).Count() == 1 ? recebimento.Grupo.Select(t => t.Antecipado).FirstOrDefault() : (bool?)null;
                            recebimento.Adquirente = parcelas.Select(t => t.Adquirente).FirstOrDefault() ?? "";
                            recebimento.Bandeira = recebimento.Grupo.GroupBy(t => t.Bandeira).Count() == 1 ? recebimento.Grupo.Select(t => t.Bandeira).FirstOrDefault() : "";
                            recebimento.Data = movimentacao.Data;
                            recebimento.TipoCartao = recebimento.Grupo.GroupBy(t => t.TipoCartao).Count() == 1 ? recebimento.Grupo.Select(t => t.TipoCartao).FirstOrDefault() : "";
                            recebimento.Lote = lotes.Count == 1 ? lotes[0].Lote : 0;
                            recebimento.ValorTotal = recebimento.Grupo.Sum(g => g.Valor);
                            recebimento.ValorTotalBruto = recebimento.Grupo.Sum(g => g.ValorBruto);

                            // ADICIONA COMO PRÉ-CONCILIADO
                            if (adicionaNaListaPreConciliado)
                                adicionaElementosConciliadosNaLista(listaConciliacao, recebimento, movimentacao, TIPO_CONCILIADO.PRE_CONCILIADO);

                            // REMOVE A MOVIMENTAÇÃO DE EXTRATO
                            extratoBancario.Remove(movimentacao);

                            // REMOVE OS LOTES
                            recebimentosParcelaAgrupados.RemoveAll(l => lotes.Any(r => r.Lote == l.Lote && (r.Lote > 0 ||
                                                                                  (r.Lote == 0 && // se Lote = 0, avalia pelos outros campos
                                                                                    r.Bandeira.Equals(l.Bandeira) &&
                                                                                    r.Filial.Equals(l.Filial) &&
                                                                                    r.Valor == l.ValorTotal &&
                                                                                    ((r.DtVenda == null && l.DataVenda == null) || (r.DtVenda != null && l.DataVenda != null && r.DtVenda.Value.Equals(l.DataVenda.Value)))))));

                            #endregion

                            break; // vai para a próxima movimentação de extrato
                        }
                    }
                }
                #endregion
            }

        }
        #endregion





        /// <summary>
        /// Retorna a lista de conciliação bancária
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
                List<dynamic> CollectionConciliacaoBancaria = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringAjustes = new Dictionary<string, string>();
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                Dictionary<string, string> queryStringExtrato = new Dictionary<string, string>();
                // DATA
                string vigencia = String.Empty;
                string data = String.Empty;
                if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                {
                    data = queryString["" + (int)CAMPOS.DATA];
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, data);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, data);
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, data);
                }
                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de conciliação bancária!");
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ, CnpjEmpresa);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.NU_CNPJ, CnpjEmpresa);
                }
                //else throw new Exception("Uma filial deve ser selecionada como filtro de conciliação bancária!");
                // ADQUIRENTE
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE, "0!"); // cdAdquirente != null ou (cdAdquirente == null && (dsTipoCartao != null || flAntecipacao))
                string cdAdquirente = String.Empty;
                if (queryString.TryGetValue("" + (int)CAMPOS.CDADQUIRENTE, out outValue))
                {
                    /*int idOperadora = Convert.ToInt32(queryString["" + (int)CAMPOS.IDOPERADORA]);
                    Operadora operadora = _db.Operadoras.Where(o => o.id == idOperadora).FirstOrDefault();
                    if (operadora != null)
                    {
                        tbAdquirente adquirente = _db.tbAdquirentes.Where(a => a.nmAdquirente.Equals(operadora.nmOperadora)).FirstOrDefault();
                        if (adquirente != null)
                        {
                            queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDADQUIRENTE, adquirente.cdAdquirente.ToString());
                            queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.IDOPERADORA, idOperadora.ToString());
                            queryStringExtrato["" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE] = adquirente.cdAdquirente.ToString();
                            cdAdquirente = adquirente.cdAdquirente.ToString();
                        }
                    }*/
                    cdAdquirente = queryString["" + (int)CAMPOS.CDADQUIRENTE];
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDADQUIRENTE, cdAdquirente);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDADQUIRENTE, cdAdquirente);
                    queryStringExtrato["" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE] = cdAdquirente + "!";

                }

                // Vigência
                vigencia = CnpjEmpresa;
                if (!data.Equals(""))
                {
                    vigencia += "!" + data;
                    if (!cdAdquirente.Equals("")) vigencia += "!" + cdAdquirente;
                }
                else if (!cdAdquirente.Equals("")) vigencia += "!null!" + cdAdquirente;
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.VIGENCIA, vigencia);

                // PARA O EXTRATO, SÓ CONSIDERA OS TIPO CREDIT
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DSTIPO, OFXSharp.OFXTransactionType.CREDIT.ToString());


                // Conta corrente específica
                string contaCorrente = String.Empty;
                if (queryString.TryGetValue("" + (int)CAMPOS.CDCONTACORRENTE, out outValue))
                {
                    contaCorrente = queryString["" + (int)CAMPOS.CDCONTACORRENTE];
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.CDCONTACORRENTE, contaCorrente);
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDCONTACORRENTE, contaCorrente);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDCONTACORRENTE, contaCorrente);
                }
                //else if (cdAdquirente.Trim().Equals(""))
                //    throw new Exception("Uma adquirente ou conta corrente deve ser informada!");



                // FILTRO DE TIPO ?
                bool filtroTipoConciliado = false;
                bool filtroTipoPreConciliado = false;
                bool filtroTipoNaoConciliado = false;
                if (queryString.TryGetValue("" + (int)CAMPOS.TIPO, out outValue))
                {
                    TIPO_CONCILIADO tipo = (TIPO_CONCILIADO)Convert.ToInt32(queryString["" + (int)CAMPOS.TIPO]);
                    if (tipo.Equals(TIPO_CONCILIADO.CONCILIADO)) filtroTipoConciliado = true;
                    else if (tipo.Equals(TIPO_CONCILIADO.PRE_CONCILIADO)) filtroTipoPreConciliado = true;
                    else if (tipo.Equals(TIPO_CONCILIADO.NAO_CONCILIADO)) filtroTipoNaoConciliado = true;
                }


                // Sem ajustes de antecipação
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.SEM_AJUSTES_ANTECIPACAO, true.ToString());

                // CONTA CORRENTE
                //if (!contaCorrente.Equals(""))
                //{
                //    //queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.CDCONTACORRENTE, contaCorrente);
                //    //queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDCONTACORRENTE, contaCorrente);
                //}

                //List<string> filiaisDaConta = null;
                //List<int> adquirentesDaConta = null;

                // OBTÉM AS QUERIES
                //IQueryable<tbRecebimentoAjuste> queryAjustes = GatewayTbRecebimentoAjuste.getQuery(_db, 0, (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, 0, 0, queryStringAjustes);
                //IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(_db, 0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, 0, 0, queryStringRecebimentoParcela);
                //IQueryable<tbExtrato> queryExtrato = GatewayTbExtrato.getQuery(_db, 0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, 0, 0, queryStringExtrato);

                // SE TIVER CONTA CORRENTE ASSOCIADA E NENHUMA FILIAL FOR ESPECIFICADA, SÓ OBTÉM OS DADOS DAS FILIAIS ASSOCIADAS À CONTA (O MESMO VALE PARA ADQUIRENTE)
                //if (!contaCorrente.Equals(""))
                //{
                //    int cdContaCorrente = Convert.ToInt32(contaCorrente);
                //    if (CnpjEmpresa.Equals(""))
                //    {
                //        filiaisDaConta = Permissoes.GetFiliaisDaConta(cdContaCorrente, _db);
                //        //queryAjustes = queryAjustes.Where(e => filiaisDaConta.Contains(e.nrCNPJ)).AsQueryable<tbRecebimentoAjuste>();
                //        //queryRecebimentoParcela = queryRecebimentoParcela.Where(e => filiaisDaConta.Contains(e.Recebimento.cnpj)).AsQueryable<RecebimentoParcela>();
                //    }
                //    if (cdAdquirente.Equals(""))
                //    {
                //        adquirentesDaConta = Permissoes.GetAdquirentesDaConta(cdContaCorrente, _db);
                //        queryAjustes = queryAjustes.Where(e => adquirentesDaConta.Contains(e.tbBandeira.cdAdquirente)).AsQueryable<tbRecebimentoAjuste>();
                //        //queryRecebimentoParcela = queryRecebimentoParcela.Where(e => adquirentesDaConta.Contains(e.Recebimento.tbBandeira.cdAdquirente)).AsQueryable<RecebimentoParcela>();
                //    }
                //}


                // VALOR TOTAL ASSOCIADO A CADA LADO DA CONCILIAÇÃO
                //decimal totalRecebimento = new decimal(0.0);
                //decimal totalExtrato = new decimal(0.0);


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

                    #region OBTÉM COMPONENTES QUERIES
                    SimpleDataBaseQuery dataBaseQueryRP = GatewayRecebimentoParcela.getQuery((int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, queryStringRecebimentoParcela);
                    SimpleDataBaseQuery dataBaseQueryAJ = GatewayTbRecebimentoAjuste.getQuery((int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, queryStringAjustes);
                    SimpleDataBaseQuery dataBaseQueryEX = GatewayTbExtrato.getQuery((int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, queryStringExtrato);

                    // RECEBIMENTO PARCELA
                    // Adiciona join com tbBandeira e tbAdquirente, caso não exista
                    if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimento");
                    if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                    if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                    if (!dataBaseQueryRP.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");
                    if (!dataBaseQueryRP.join.ContainsKey("LEFT JOIN card.tbAntecipacaoBancariaDetalhe " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("LEFT JOIN card.tbAntecipacaoBancariaDetalhe " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe");
                    if (!dataBaseQueryRP.join.ContainsKey("LEFT JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY))
                        dataBaseQueryRP.join.Add("LEFT JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".idAntecipacaoBancaria = " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancaria");

                    // AJUSTES
                    if (!dataBaseQueryAJ.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                        dataBaseQueryAJ.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".cdBandeira");
                    if (!dataBaseQueryAJ.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        dataBaseQueryAJ.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                    if (!dataBaseQueryAJ.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                        dataBaseQueryAJ.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".nrCNPJ = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");
                    if (!dataBaseQueryAJ.join.ContainsKey("LEFT JOIN card.tbAntecipacaoBancariaDetalhe " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY))
                        dataBaseQueryAJ.join.Add("LEFT JOIN card.tbAntecipacaoBancariaDetalhe " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe = " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idAntecipacaoBancariaDetalhe");
                    if (!dataBaseQueryAJ.join.ContainsKey("LEFT JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY))
                        dataBaseQueryAJ.join.Add("LEFT JOIN card.tbAntecipacaoBancaria " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY, " ON " + GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".idAntecipacaoBancaria = " + GatewayTbAntecipacaoBancariaDetalhe.SIGLA_QUERY + ".idAntecipacaoBancaria");


                    // EXTRATO
                    if (!dataBaseQueryEX.join.ContainsKey("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("INNER JOIN card.tbContaCorrente " + GatewayTbContaCorrente.SIGLA_QUERY, " ON " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente = " + GatewayTbExtrato.SIGLA_QUERY + ".cdContaCorrente");
                    if (!dataBaseQueryEX.join.ContainsKey("INNER JOIN card.tbBancoParametro " + GatewayTbBancoParametro.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("INNER JOIN card.tbBancoParametro " + GatewayTbBancoParametro.SIGLA_QUERY, " ON " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdBanco = " + GatewayTbContaCorrente.SIGLA_QUERY + ".cdBanco AND " + GatewayTbBancoParametro.SIGLA_QUERY + ".dsMemo = " + GatewayTbExtrato.SIGLA_QUERY + ".dsDocumento");
                    if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("LEFT JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdAdquirente");
                    if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("LEFT JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayTbBancoParametro.SIGLA_QUERY + ".cdBandeira");
                    if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                        dataBaseQueryEX.join.Add("LEFT JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj = " + GatewayTbBancoParametro.SIGLA_QUERY + ".nrCnpj");



                    // RECEBIMENTO PARCELA
                    dataBaseQueryRP.select = new string[] { GatewayRecebimento.SIGLA_QUERY + ".id",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".numParcela",
                                                          GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                          GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".dsTipo",
                                                          GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                          GatewayRecebimento.SIGLA_QUERY + ".dtaVenda",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo",
                                                          GatewayRecebimento.SIGLA_QUERY + ".nsu",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaLiquida",
                                                          GatewayRecebimento.SIGLA_QUERY + ".idResumoVenda AS lote",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".flAntecipado",
                                                          GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".vlLiquido",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoOriginal"
                                                        };

                    // Sem ordem
                    dataBaseQueryRP.readUncommited = true;
                    dataBaseQueryRP.groupby = null;
                    dataBaseQueryRP.orderby = new string[] { "CASE WHEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL THEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo ELSE " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento END",
                                                       GatewayRecebimento.SIGLA_QUERY + ".dtaVenda"};


                    // AJUSTE
                    dataBaseQueryAJ.select = new string[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idRecebimentoAjuste",
                                                        GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idResumoVenda",
                                                        GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dsMotivo",
                                                        GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlAjuste",
                                                        GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste",
                                                        GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtVenda",
                                                        GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".vlBruto",
                                                        GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".flAntecipacao",
                                                        GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                        GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                        GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                        GatewayTbBandeira.SIGLA_QUERY + ".dsTipo",
                                                        GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                        GatewayTbAntecipacaoBancaria.SIGLA_QUERY + ".vlLiquido"
                                                      };
                    dataBaseQueryAJ.readUncommited = true;
                    dataBaseQueryAJ.groupby = null;
                    dataBaseQueryAJ.orderby = new string[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste" };


                    // EXTRATO
                    dataBaseQueryEX.select = new string[] { GatewayTbExtrato.SIGLA_QUERY + ".idExtrato",
                                                        GatewayTbExtrato.SIGLA_QUERY + ".nrDocumento",
                                                        GatewayTbExtrato.SIGLA_QUERY + ".dtExtrato",
                                                        GatewayTbExtrato.SIGLA_QUERY + ".dsDocumento",
                                                        "vlMovimento = ISNULL(" + GatewayTbExtrato.SIGLA_QUERY + ".vlMovimento, 0)",
                                                        GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                        GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                        GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                        GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                        GatewayTbBancoParametro.SIGLA_QUERY + ".dsTipoCartao",
                                                        GatewayTbBancoParametro.SIGLA_QUERY + ".flAntecipacao",
                                                        GatewayTbContaCorrente.SIGLA_QUERY + ".cdContaCorrente",
                                                        GatewayTbContaCorrente.SIGLA_QUERY + ".nrAgencia",
                                                        GatewayTbContaCorrente.SIGLA_QUERY + ".nrConta",
                                                        GatewayTbContaCorrente.SIGLA_QUERY + ".cdBanco",
                                                      };
                    dataBaseQueryEX.orderby = new[] { GatewayTbExtrato.SIGLA_QUERY + ".dtExtrato" };
                    dataBaseQueryEX.readUncommited = true;
                    dataBaseQueryEX.readDistinct = true;
                    dataBaseQueryEX.groupby = null;

                    #endregion

                    // Só busca por conciliações já concretizadas se não tiver sido requisitado um filtro do tipo PRE-CONCILIADO ou NÃO CONCILIADO
                    if (!filtroTipoPreConciliado && !filtroTipoNaoConciliado)
                    {
                        // Adiciona na cláusula where IDEXTRATO IS NOT NULL
                        SimpleDataBaseQuery queryRpConciliados = new SimpleDataBaseQuery(dataBaseQueryRP);
                        queryRpConciliados.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato IS NOT NULL");
                        // Agrupa por extrato
                        queryRpConciliados.groupby = new string[] { GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato" };
                        queryRpConciliados.orderby = null;
                        queryRpConciliados.select = new string[] { GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato" };

                        #region OBTÉM AS INFORMAÇÕES DE DADOS JÁ CONCILIADOS PREVIAMENTE
                        // EXTRATOS JÁ CONCILIADOS, COM SEUS RESPECTIVOS RECEBIMENTOS CONCILIADOS
                        List<int> idsExtratosConciliados = new List<int>();
                        List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryRpConciliados.Script(), connection);

                        if (resultado != null && resultado.Count > 0)
                        {
                            // Obtém ids extratos
                            idsExtratosConciliados = resultado.Select(t => Convert.ToInt32(t["idExtrato"])).ToList<int>();
                        }

                        // Adiciona também os extratos conciliados com ajustes
                        SimpleDataBaseQuery queryAjConciliados = new SimpleDataBaseQuery(dataBaseQueryAJ);
                        queryAjConciliados.AddWhereClause(GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato IS NOT NULL");
                        if (idsExtratosConciliados.Count > 0)
                            queryAjConciliados.AddWhereClause(GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato NOT IN (" + string.Join(", ", idsExtratosConciliados) + ")");
                        // Agrupa por extrato
                        queryAjConciliados.groupby = new string[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato" };
                        queryAjConciliados.orderby = null;
                        queryAjConciliados.select = new string[] { GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato" };
                        //idsExtratosConciliados.AddRange(queryAjustes.Where(t => t.idExtrato != null)
                        //                                                      .GroupBy(t => t.idExtrato)
                        //                                                      .Where(t => !idsExtratosConciliados.Contains(t.Key.Value))
                        //                                                      .Select(t => t.Key.Value));
                        resultado = DataBaseQueries.SqlQuery(queryAjConciliados.Script(), connection);

                        if (resultado != null && resultado.Count > 0)
                            idsExtratosConciliados.AddRange(resultado.Select(t => Convert.ToInt32(t["idExtrato"])));


                        // Total dos elementos já conciliados
                        if (idsExtratosConciliados.Count > 0)
                        {
                            queryRpConciliados.groupby = null;
                            queryRpConciliados.orderby = dataBaseQueryRP.orderby;
                            queryRpConciliados.select = dataBaseQueryRP.select;

                            queryAjConciliados = new SimpleDataBaseQuery(dataBaseQueryAJ);
                            queryAjConciliados.orderby = dataBaseQueryAJ.orderby;
                            queryAjConciliados.select = dataBaseQueryAJ.select;
                            queryAjConciliados.AddWhereClause(GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato IS NOT NULL");

                            // Adiciona como conciliados
                            foreach (Int32 idExtrato in idsExtratosConciliados)
                            {
                                queryRpConciliados.where[dataBaseQueryRP.where.Length] = GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato = " + idExtrato;
                                queryAjConciliados.where[dataBaseQueryAJ.where.Length] = GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato = " + idExtrato;

                                resultado = DataBaseQueries.SqlQuery(queryRpConciliados.Script(), connection);
                                List<dynamic> rps = new List<dynamic>();

                                if (resultado != null && resultado.Count > 0)
                                {
                                    rps = resultado.Select(t => new
                                    {
                                        Id = Convert.ToInt32(t["id"]),
                                        NumParcela = Convert.ToInt32(t["numParcela"]),
                                        Documento = Convert.ToString(t["nsu"]),
                                        Valor = Convert.ToDecimal(t["valorParcelaLiquida"].Equals(DBNull.Value) ? 0.0 : t["valorParcelaLiquida"]),
                                        ValorBruto = Convert.ToDecimal(t["valorParcelaBruta"]),
                                        Adquirente = Convert.ToString(t["nmAdquirente"]),
                                        Bandeira = Convert.ToString(t["dsBandeira"]),
                                        Lote = t["lote"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(t["lote"]),
                                        AntecipacaoBancaria = Convert.ToDecimal(t["vlLiquido"].Equals(DBNull.Value) ? 0.0 : t["vlLiquido"]),
                                        TipoCartao = Convert.ToString(t["dsTipo"]),
                                        DataVenda = (DateTime)t["dtaVenda"],
                                        DataPrevista = (DateTime)t["dtaRecebimento"],
                                        DataEfetiva = t["dtaRecebimentoEfetivo"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["dtaRecebimentoEfetivo"],
                                        Antecipado = Convert.ToBoolean(t["flAntecipado"]) && !t["dtaRecebimentoEfetivo"].Equals(DBNull.Value) && !((DateTime)t["dtaRecebimentoEfetivo"]).Equals((DateTime)t["dtaRecebimento"]),
                                        DataRecebimentoOriginal = t["dtaRecebimentoOriginal"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["dtaRecebimentoOriginal"],
                                        Filial = Convert.ToString(t["ds_fantasia"]) + (t["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(t["filial"])),
                                    }).ToList<dynamic>();
                                }

                                // Recebimento
                                ConciliacaoBancaria recebimento = rps.GroupBy(r => r.Adquirente) // agrupa pela mesma adquirente registro
                                                                        .Select(r => new ConciliacaoBancaria
                                                                        {
                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                            Grupo = r.Select(x =>
                                                                                new ConciliacaoBancaria.ConciliacaoGrupo
                                                                                {
                                                                                    Id = x.Id,
                                                                                    NumParcela = x.NumParcela,
                                                                                    Documento = x.Documento,
                                                                                    Valor = x.Valor,
                                                                                    ValorBruto = x.ValorBruto,
                                                                                    Bandeira = x.Bandeira.ToUpper(),
                                                                                    Lote = x.Lote,
                                                                                    AntecipacaoBancaria = x.AntecipacaoBancaria,
                                                                                    //DataRecebimentoOriginal = x.DataRecebimentoOriginal,
                                                                                    TipoCartao = x.TipoCartao.ToUpper().TrimEnd(),
                                                                                    DataVenda = x.DataVenda,
                                                                                    DataPrevista = x.DataPrevista,
                                                                                    Antecipado = x.Antecipado,
                                                                                    Filial = x.Filial.ToUpper()
                                                                                })
                                                                                .OrderBy(x => x.Filial)
                                                                                .ThenBy(x => x.Bandeira)
                                                                                .ThenByDescending(x => x.DataVenda)
                                                                                .ThenBy(x => x.DataPrevista)
                                                                                .ThenBy(x => x.Valor)
                                                                                .ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                            ValorTotal = r.Select(x => x.Valor).Cast<decimal>().Sum(),
                                                                            ValorTotalBruto = r.Select(x => x.ValorBruto).Cast<decimal>().Sum(),
                                                                            Data = r.Select(x => x.DataEfetiva ?? x.DataPrevista).FirstOrDefault(),
                                                                            Adquirente = r.Key.ToUpper(),
                                                                            Bandeira = r.GroupBy(x => x.Bandeira).Count() == 1 ? r.Select(x => x.Bandeira.ToUpper()).FirstOrDefault() : "",
                                                                            Lote = r.GroupBy(x => x.Lote).Count() == 1 ? r.Select(x => x.Lote).FirstOrDefault() ?? 0 : 0,
                                                                            AntecipacaoBancaria = r.GroupBy(x => x.AntecipacaoBancaria).Count() == 1 ? r.Select(x => x.AntecipacaoBancaria).FirstOrDefault() ?? 0 : 0,
                                                                            TipoCartao = r.GroupBy(x => x.TipoCartao).Count() == 1 ? r.Select(x => x.TipoCartao.ToUpper().TrimEnd()).FirstOrDefault() : "",
                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                            DataRecebimentoDiferente = r.Where(x => x.DataRecebimentoOriginal != null &&
                                                                                // Data original tem que ser a igual a prevista
                                                                                                                    x.DataRecebimentoOriginal.Equals(x.DataPrevista) &&
                                                                                // Se a data efetiva for diferente da prevista, não considera como recebimento diferente
                                                                                                                    (x.DataEfetiva == null || x.DataPrevista.Equals(x.DataEfetiva)))
                                                                                                        .Count() == r.Count(),
                                                                            Filial = r.GroupBy(x => x.Filial).Count() == 1 ? r.Select(x => x.Filial.ToUpper()).FirstOrDefault() : ""
                                                                        }).FirstOrDefault<ConciliacaoBancaria>();

                                //ConciliacaoBancaria ajustes = _db.tbRecebimentoAjustes
                                //                                        .Where(e => e.idExtrato == idExtrato)
                                //                                        .OrderBy(r => r.dtAjuste)
                                //                                        .GroupBy(r => r.tbExtrato) // agrupa pelo mesmo extrato para se tornar um único registro
                                //                                        .Select(r => new ConciliacaoBancaria
                                //                                        {
                                //                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                //                                            Grupo = r.Select(x =>
                                //                                                new ConciliacaoBancaria.ConciliacaoGrupo
                                //                                                {
                                //                                                    Id = x.idRecebimentoAjuste,
                                //                                                    NumParcela = -1,
                                //                                                    Lote = x.idResumoVenda ?? 0,
                                //                                                    Documento = x.dsMotivo,
                                //                                                    Valor = x.vlAjuste,
                                //                                                    ValorBruto = new decimal(0.0),
                                //                                                    Bandeira = x.tbBandeira.dsBandeira.ToUpper(),
                                //                                                    TipoCartao = x.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                                //                                                    DataVenda = x.dtAjuste,
                                //                                                    DataPrevista = x.dtAjuste,
                                //                                                    Antecipado = x.flAntecipacao,
                                //                                                    Filial = x.empresa.ds_fantasia.ToUpper() + (x.empresa.filial != null ? " " + x.empresa.filial.ToUpper() : "")
                                //                                                })
                                //                                                .OrderBy(x => x.Filial)
                                //                                                .ThenBy(x => x.Bandeira)
                                //                                                .ThenByDescending(x => x.DataVenda)
                                //                                                .ThenBy(x => x.DataPrevista)
                                //                                                .ThenBy(x => x.Valor)
                                //                                                .ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                //                                            ValorTotal = r.Select(x => x.vlAjuste).Sum(),
                                //                                            ValorTotalBruto = new decimal(0.0),
                                //                                            Data = r.Select(x => x.dtAjuste).FirstOrDefault(),
                                //                                            Adquirente = r.Select(x => x.tbBandeira.tbAdquirente.nmAdquirente.ToUpper()).FirstOrDefault(),
                                //                                            Bandeira = r.GroupBy(x => x.tbBandeira.cdBandeira).Count() == 1 ? r.Select(x => x.tbBandeira.dsBandeira.ToUpper()).FirstOrDefault() : "",
                                //                                            Lote = r.GroupBy(x => x.idResumoVenda).Count() == 1 ? r.Select(x => x.idResumoVenda).FirstOrDefault() ?? 0 : 0,
                                //                                            TipoCartao = r.GroupBy(x => x.tbBandeira.dsTipo).Count() == 1 ? r.Select(x => x.tbBandeira.dsTipo.ToUpper().TrimEnd()).FirstOrDefault() : "",
                                //                                            Antecipado = r.GroupBy(x => x.flAntecipacao).Count() == 1 ? r.Select(x => x.flAntecipacao).FirstOrDefault() : (bool?)null,
                                //                                            Filial = r.GroupBy(x => x.empresa).Count() == 1 ? r.Select(x => x.empresa.ds_fantasia.ToUpper() + (x.empresa.filial != null ? " " + x.empresa.filial.ToUpper() : "")).FirstOrDefault() : ""
                                //                                        }).FirstOrDefault<ConciliacaoBancaria>();

                                List<dynamic> ajs = new List<dynamic>();
                                resultado = DataBaseQueries.SqlQuery(queryAjConciliados.Script(), connection);
                                if (resultado != null && resultado.Count > 0)
                                {
                                    ajs = resultado.Select(t => new
                                    {
                                        Id = Convert.ToInt32(t["idRecebimentoAjuste"]),
                                        Documento = Convert.ToString(t["dsMotivo"]),
                                        Valor = Convert.ToDecimal(t["vlAjuste"]),
                                        ValorBruto = Convert.ToDecimal(t["vlBruto"]),
                                        Adquirente = Convert.ToString(t["nmAdquirente"]),
                                        Bandeira = Convert.ToString(t["dsBandeira"]),
                                        Lote = t["idResumoVenda"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(t["idResumoVenda"]),
                                        AntecipacaoBancaria = Convert.ToDecimal(t["vlLiquido"].Equals(DBNull.Value) ? 0.0 : t["vlLiquido"]),
                                        TipoCartao = Convert.ToString(t["dsTipo"]),
                                        Data = (DateTime)t["dtAjuste"],
                                        DataVenda = t["dtVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["dtVenda"],
                                        Antecipado = Convert.ToBoolean(t["flAntecipacao"]),
                                        Filial = Convert.ToString(t["ds_fantasia"]) + (t["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(t["filial"])),
                                    }).ToList<dynamic>();
                                }

                                ConciliacaoBancaria ajustes = ajs.GroupBy(r => r.Adquirente) // agrupa pela mesma adquirente registro
                                                                        .Select(r => new ConciliacaoBancaria
                                                                        {
                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                            Grupo = r.Select(x =>
                                                                                new ConciliacaoBancaria.ConciliacaoGrupo
                                                                                {
                                                                                    Id = x.Id,
                                                                                    NumParcela = -1,
                                                                                    Documento = x.Documento,
                                                                                    Valor = x.Valor,
                                                                                    ValorBruto = x.ValorBruto != new decimal(0.0) ? x.ValorBruto : x.Valor > new decimal(0.0) ? x.Valor : new decimal(0.0),
                                                                                    Bandeira = x.Bandeira.ToUpper(),
                                                                                    Lote = x.Lote,
                                                                                    AntecipacaoBancaria = x.AntecipacaoBancaria,
                                                                                    TipoCartao = x.TipoCartao.ToUpper().TrimEnd(),
                                                                                    DataVenda = x.DataVenda,
                                                                                    DataPrevista = x.Data,
                                                                                    //DataRecebimentoOriginal = null,
                                                                                    Antecipado = x.Antecipado,
                                                                                    Filial = x.Filial.ToUpper()
                                                                                })
                                                                                .OrderBy(x => x.Filial)
                                                                                .ThenBy(x => x.Bandeira)
                                                                                .ThenByDescending(x => x.DataVenda)
                                                                                .ThenBy(x => x.DataPrevista)
                                                                                .ThenBy(x => x.Valor)
                                                                                .ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                            ValorTotal = r.Select(x => x.Valor).Cast<decimal>().Sum(),
                                                                            ValorTotalBruto = (r.Where(x => x.ValorBruto != new decimal(0.0)).Count() > 0 ? r.Where(x => x.ValorBruto != new decimal(0.0)).Select(x => x.ValorBruto).Cast<decimal>().Sum() : new decimal(0.0)) +
                                                                                              (r.Where(x => x.ValorBruto == new decimal(0.0) && x.Valor > new decimal(0.0)).Count() > 0 ? r.Where(x => x.ValorBruto == new decimal(0.0) && x.Valor > new decimal(0.0)).Select(x => x.Valor).Cast<decimal>().Sum() : new decimal(0.0)),
                                                                            Data = r.Select(x => x.Data).FirstOrDefault(),
                                                                            Adquirente = r.Key.ToUpper(),
                                                                            Bandeira = r.GroupBy(x => x.Bandeira).Count() == 1 ? r.Select(x => x.Bandeira.ToUpper()).FirstOrDefault() : "",
                                                                            Lote = r.GroupBy(x => x.Lote).Count() == 1 ? r.Select(x => x.Lote).FirstOrDefault() ?? 0 : 0,
                                                                            AntecipacaoBancaria = r.GroupBy(x => x.AntecipacaoBancaria).Count() == 1 ? r.Select(x => x.AntecipacaoBancaria).FirstOrDefault() ?? 0 : 0,
                                                                            TipoCartao = r.GroupBy(x => x.TipoCartao).Count() == 1 ? r.Select(x => x.TipoCartao.ToUpper().TrimEnd()).FirstOrDefault() : "",
                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                            DataRecebimentoDiferente = false,
                                                                            Filial = r.GroupBy(x => x.Filial).Count() == 1 ? r.Select(x => x.Filial.ToUpper()).FirstOrDefault() : ""
                                                                        }).FirstOrDefault<ConciliacaoBancaria>();

                                if (recebimento == null && ajustes == null) continue; // falha!

                                // Recebimento + Ajuste
                                if (recebimento == null)
                                    recebimento = ajustes;
                                else if (ajustes != null)
                                {
                                    foreach (ConciliacaoBancaria.ConciliacaoGrupo grupo in ajustes.Grupo)
                                        recebimento.Grupo.Add(grupo);
                                    recebimento.Grupo = recebimento.Grupo.OrderBy(x => x.Bandeira).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>();
                                    recebimento.ValorTotal += ajustes.ValorTotal;
                                    if (!recebimento.Bandeira.Equals(ajustes.Bandeira))
                                        recebimento.Bandeira = "";
                                    if (!recebimento.TipoCartao.Equals(ajustes.TipoCartao))
                                        recebimento.TipoCartao = "";
                                }



                                // Movimentação
                                SimpleDataBaseQuery queryEXConciliado = new SimpleDataBaseQuery(dataBaseQueryEX);
                                queryEXConciliado.where = new string[] { GatewayTbExtrato.SIGLA_QUERY + ".idExtrato = " + idExtrato };

                                resultado = DataBaseQueries.SqlQuery(queryEXConciliado.Script(), connection);

                                ConciliacaoBancaria movimentacao = null;
                                if (resultado == null || resultado.Count == 0)
                                    continue; // ERROR!

                                movimentacao = resultado.Select(t => new ConciliacaoBancaria
                                                        {
                                                            Tipo = TIPO_EXTRATO, // extrato
                                                            Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                                            new ConciliacaoBancaria.ConciliacaoGrupo {
                                                                Id = Convert.ToInt32(t["idExtrato"]),
                                                                Documento = Convert.ToString(t["nrDocumento"]),
                                                                Valor = Convert.ToDecimal(t["vlMovimento"]),
                                                                Filial = Convert.ToString(t["ds_fantasia"].Equals(DBNull.Value) ? "" : t["ds_fantasia"] + (t["filial"].Equals(DBNull.Value) ? "" : " " + t["filial"])).ToUpper(),
                                                            }
                                                        },
                                                            ValorTotal = Convert.ToDecimal(t["vlMovimento"]),
                                                            Data = (DateTime)t["dtExtrato"],
                                                            Adquirente = Convert.ToString(t["nmAdquirente"].Equals(DBNull.Value) ? "" : t["nmAdquirente"]).ToUpper(),
                                                            Memo = Convert.ToString(t["dsDocumento"]),
                                                            Conta = new ConciliacaoBancaria.ConciliacaoConta
                                                                    {
                                                                        CdContaCorrente = Convert.ToInt32(t["cdContaCorrente"]),
                                                                        NrAgencia = Convert.ToString(t["nrAgencia"]),
                                                                        NrConta = Convert.ToString(t["nrConta"]),
                                                                        CdBanco = Convert.ToString(t["cdBanco"])
                                                                    },
                                                            Bandeira = Convert.ToString(t["dsBandeira"].Equals(DBNull.Value) ? "" : t["dsBandeira"]).ToUpper(),
                                                            TipoCartao = Convert.ToString(t["dsTipoCartao"].Equals(DBNull.Value) ? "" : t["dsTipoCartao"]).TrimEnd().ToUpper(),
                                                            Antecipado = Convert.ToBoolean(t["flAntecipacao"].Equals(DBNull.Value) ? false : t["flAntecipacao"]),
                                                            Filial = Convert.ToString(t["ds_fantasia"].Equals(DBNull.Value) ? "" : t["ds_fantasia"] + (t["filial"].Equals(DBNull.Value) ? "" : " " + t["filial"])).ToUpper(),
                                                        })
                                                        .FirstOrDefault();

                                //ConciliacaoBancaria movimentacao = _db.tbExtratos.Where(e => e.idExtrato == idExtrato)
                                //    .Select(e => new ConciliacaoBancaria
                                //    {
                                //        Tipo = TIPO_EXTRATO, // extrato
                                //        Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                //            new ConciliacaoBancaria.ConciliacaoGrupo {
                                //                Id = e.idExtrato,
                                //                Documento = e.nrDocumento,
                                //                Valor = e.vlMovimento ?? new decimal(0.0),
                                //                Filial = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => (p.empresa.ds_fantasia.ToUpper() + (p.empresa.filial != null ? " " + p.empresa.filial.ToUpper() : "")) ?? "").FirstOrDefault() ?? "",
                                //            }
                                //        },
                                //        ValorTotal = e.vlMovimento ?? new decimal(0.0),
                                //        Data = e.dtExtrato,
                                //        Adquirente = _db.tbBancoParametro.Where(p => p.dsMemo.Equals(e.dsDocumento)).Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco)).Select(p => p.tbAdquirente.nmAdquirente.ToUpper() ?? "").FirstOrDefault() ?? "",
                                //        Memo = e.dsDocumento,
                                //        Conta = new ConciliacaoBancaria.ConciliacaoConta
                                //                {
                                //                    CdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                                //                    NrAgencia = e.tbContaCorrente.nrAgencia,
                                //                    NrConta = e.tbContaCorrente.nrConta,
                                //                    CdBanco = e.tbContaCorrente.cdBanco
                                //                },
                                //        Bandeira = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.tbBandeira.dsBandeira.ToUpper() ?? "").FirstOrDefault() ?? "",
                                //        TipoCartao = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.dsTipoCartao.ToUpper().TrimEnd() ?? "").FirstOrDefault() ?? "",
                                //        Antecipado = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.flAntecipacao).FirstOrDefault(),
                                //        Filial = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => (p.empresa.ds_fantasia.ToUpper() + (p.empresa.filial != null ? " " + p.empresa.filial.ToUpper() : "")) ?? "").FirstOrDefault() ?? "",
                                //    }).FirstOrDefault();

                                // Adiciona
                                adicionaElementosConciliadosNaLista(CollectionConciliacaoBancaria, recebimento, movimentacao, TIPO_CONCILIADO.CONCILIADO);
                            }
                        }
                        #endregion
                    }


                    // Só busca por possíveis conciliações se não tiver sido requisitado um filtro do tipo CONCILIADO
                    if (!filtroTipoConciliado)
                    {
                        #region OBTÉM AS INFORMAÇÕES DE DADOS NÃO-CONCILIADOS E BUSCA PRÉ-CONCILIAÇÕES

                        #region OBTÉM SOMENTE OS RECEBIMENTOS PARCELAS NÃO-CONCILIADOS
                        // Adiciona na cláusula where IDEXTRATO IS NOT NULL
                        SimpleDataBaseQuery queryRpNaoConciliados = new SimpleDataBaseQuery(dataBaseQueryRP);
                        queryRpNaoConciliados.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato IS NULL");

                        List<ConciliacaoBancaria> recebimentosParcela = new List<ConciliacaoBancaria>();

                        List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryRpNaoConciliados.Script(), connection);
                        if (resultado != null && resultado.Count > 0)
                        {
                            recebimentosParcela = resultado.Select(t => new ConciliacaoBancaria
                            {
                                Tipo = TIPO_RECEBIMENTO, // recebimento
                                Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                new ConciliacaoBancaria.ConciliacaoGrupo {
                                    Id = Convert.ToInt32(t["id"]),
                                    NumParcela =Convert.ToInt32(t["numParcela"]),
                                    Lote = t["lote"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(t["lote"]),
                                    AntecipacaoBancaria = Convert.ToDecimal(t["vlLiquido"].Equals(DBNull.Value) ? 0.0 : t["vlLiquido"]),
                                    Documento = Convert.ToString(t["nsu"]),
                                    Valor = Convert.ToDecimal(t["valorParcelaLiquida"].Equals(DBNull.Value) ? 0.0 : t["valorParcelaLiquida"]),
                                    ValorBruto = Convert.ToDecimal(t["valorParcelaBruta"]),
                                    Bandeira = Convert.ToString(t["dsBandeira"]).ToUpper(),
                                    TipoCartao = Convert.ToString(t["dsTipo"]).ToUpper().TrimEnd(),
                                    DataVenda = (DateTime)t["dtaVenda"],
                                    DataPrevista = (DateTime)t["dtaRecebimento"],
                                    Antecipado = Convert.ToBoolean(t["flAntecipado"]) && !t["dtaRecebimentoEfetivo"].Equals(DBNull.Value) && !((DateTime)t["dtaRecebimentoEfetivo"]).Equals((DateTime)t["dtaRecebimento"]),
                                    //DataRecebimentoOriginal = t["dtaRecebimentoOriginal"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["dtaRecebimentoOriginal"],
                                    Filial = Convert.ToString(t["ds_fantasia"]).ToUpper() + (t["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(t["filial"]).ToUpper())
                                }
                            },
                                ValorTotal = Convert.ToDecimal(t["valorParcelaLiquida"].Equals(DBNull.Value) ? 0.0 : t["valorParcelaLiquida"]),
                                ValorTotalBruto = Convert.ToDecimal(t["valorParcelaBruta"]),
                                Data = t["dtaRecebimentoEfetivo"].Equals(DBNull.Value) ? (DateTime)t["dtaRecebimento"] : (DateTime)t["dtaRecebimentoEfetivo"],
                                DataVenda = (DateTime)t["dtaVenda"],
                                Adquirente = Convert.ToString(t["nmAdquirente"]).ToUpper(),
                                Bandeira = Convert.ToString(t["dsBandeira"]).ToUpper(),
                                Lote = t["lote"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(t["lote"]),
                                AntecipacaoBancaria = Convert.ToDecimal(t["vlLiquido"].Equals(DBNull.Value) ? 0.0 : t["vlLiquido"]),
                                TipoCartao = Convert.ToString(t["dsTipo"]).ToUpper().TrimEnd(),
                                Antecipado = Convert.ToBoolean(t["flAntecipado"]) && !t["dtaRecebimentoEfetivo"].Equals(DBNull.Value) && !((DateTime)t["dtaRecebimentoEfetivo"]).Equals((DateTime)t["dtaRecebimento"]),
                                DataRecebimentoDiferente = !t["dtaRecebimentoOriginal"].Equals(DBNull.Value) &&
                                    // Data original tem que ser a igual a prevista
                                                           ((DateTime)t["dtaRecebimentoOriginal"]).Equals((DateTime)t["dtaRecebimento"]) &&
                                    // Se a data efetiva for diferente da prevista, não considera como recebimento diferente
                                                           (t["dtaRecebimentoEfetivo"].Equals(DBNull.Value) || ((DateTime)t["dtaRecebimento"]).Equals((DateTime)t["dtaRecebimentoEfetivo"])),
                                Filial = Convert.ToString(t["ds_fantasia"]).ToUpper() + (t["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(t["filial"]).ToUpper())
                            }).ToList<ConciliacaoBancaria>();
                        }

                        //List<ConciliacaoBancaria> ajustes = queryAjustes
                        //                                    .Where(r => r.idExtrato == null)
                        //                                    .Select(r => new ConciliacaoBancaria
                        //                                    {
                        //                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                        //                                        Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                        //                                            new ConciliacaoBancaria.ConciliacaoGrupo {
                        //                                                Id = r.idRecebimentoAjuste,
                        //                                                NumParcela = -1,
                        //                                                Lote = r.idResumoVenda ?? 0,
                        //                                                Documento = r.dsMotivo,
                        //                                                Valor = r.vlAjuste,
                        //                                                ValorBruto = new decimal(0.0),
                        //                                                Bandeira = r.tbBandeira.dsBandeira.ToUpper(),
                        //                                                TipoCartao = r.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                        //                                                DataVenda = r.dtAjuste,
                        //                                                DataPrevista = r.dtAjuste,
                        //                                                Antecipado = r.flAntecipacao,
                        //                                                Filial = r.empresa.ds_fantasia + (r.empresa.filial != null ? " " + r.empresa.filial : "")
                        //                                            }
                        //                                        },
                        //                                        ValorTotal = r.vlAjuste,
                        //                                        ValorTotalBruto = new decimal(0.0),
                        //                                        Data = r.dtAjuste,
                        //                                        DataVenda = r.dtAjuste,
                        //                                        Adquirente = r.tbBandeira.tbAdquirente.nmAdquirente.ToUpper(),
                        //                                        Bandeira = r.tbBandeira.dsBandeira.ToUpper(),
                        //                                        Lote = r.idResumoVenda ?? 0,
                        //                                        TipoCartao = r.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                        //                                        Antecipado = r.flAntecipacao,
                        //                                        Filial = r.empresa.ds_fantasia + (r.empresa.filial != null ? " " + r.empresa.filial : "")
                        //                                    }).ToList<ConciliacaoBancaria>();

                        SimpleDataBaseQuery queryAjNaoConciliados = new SimpleDataBaseQuery(dataBaseQueryAJ);
                        queryAjNaoConciliados.AddWhereClause(GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato IS NULL");

                        List<ConciliacaoBancaria> ajustes = new List<ConciliacaoBancaria>();

                        resultado = DataBaseQueries.SqlQuery(queryAjNaoConciliados.Script(), connection);
                        if (resultado != null && resultado.Count > 0)
                        {
                            ajustes = resultado.Select(t => new ConciliacaoBancaria
                                {
                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                    Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                    new ConciliacaoBancaria.ConciliacaoGrupo {
                                        Id = Convert.ToInt32(t["idRecebimentoAjuste"]),
                                        NumParcela = -1,
                                        Lote = t["idResumoVenda"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(t["idResumoVenda"]),
                                        AntecipacaoBancaria = Convert.ToDecimal(t["vlLiquido"].Equals(DBNull.Value) ? 0.0 : t["vlLiquido"]),
                                        Documento = Convert.ToString(t["dsMotivo"]),
                                        Valor = Convert.ToDecimal(t["vlAjuste"]),
                                        ValorBruto = Convert.ToDecimal(t["vlBruto"]) != new decimal(0.0) ? Convert.ToDecimal(t["vlBruto"]) : Convert.ToDecimal(t["vlAjuste"]) > new decimal(0.0) ? Convert.ToDecimal(t["vlAjuste"]) : new decimal(0.0),
                                        Bandeira = Convert.ToString(t["dsBandeira"]).ToUpper(),
                                        TipoCartao = Convert.ToString(t["dsTipo"]).ToUpper().TrimEnd(),
                                        //DataVenda = (DateTime)t["dtAjuste"],
                                        DataVenda = t["dtVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["dtVenda"],
                                        DataPrevista = (DateTime)t["dtAjuste"],
                                        Antecipado = Convert.ToBoolean(t["flAntecipacao"]),
                                        Filial = Convert.ToString(t["ds_fantasia"]).ToUpper() + (t["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(t["filial"]).ToUpper())
                                    }
                                },
                                    ValorTotal = Convert.ToDecimal(t["vlAjuste"]),
                                    ValorTotalBruto = Convert.ToDecimal(t["vlBruto"]) != new decimal(0.0) ? Convert.ToDecimal(t["vlBruto"]) : Convert.ToDecimal(t["vlAjuste"]) > new decimal(0.0) ? Convert.ToDecimal(t["vlAjuste"]) : new decimal(0.0),
                                    Data = (DateTime)t["dtAjuste"],
                                    //DataVenda = (DateTime)t["dtAjuste"],
                                    DataVenda = t["dtVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["dtVenda"],
                                    Adquirente = Convert.ToString(t["nmAdquirente"]).ToUpper(),
                                    Bandeira = Convert.ToString(t["dsBandeira"]).ToUpper(),
                                    Lote = t["idResumoVenda"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(t["idResumoVenda"]),
                                    AntecipacaoBancaria = Convert.ToDecimal(t["vlLiquido"].Equals(DBNull.Value) ? 0 : t["vlLiquido"]),
                                    TipoCartao = Convert.ToString(t["dsTipo"]).ToUpper().TrimEnd(),
                                    Antecipado = Convert.ToBoolean(t["flAntecipacao"]),
                                    DataRecebimentoDiferente = false,
                                    Filial = Convert.ToString(t["ds_fantasia"]).ToUpper() + (t["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(t["filial"]).ToUpper())
                                }).ToList<ConciliacaoBancaria>();
                        }



                        foreach (ConciliacaoBancaria aj in ajustes)
                            recebimentosParcela.Add(aj);
                        #endregion

                        #region OBTÉM OS EXTRATOS NÃO CONCILIADOS QUE TEM ADQUIRENTE ASSOCIADA

                        if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY))
                            dataBaseQueryEX.join.Add("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY, " ON " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato = " + GatewayTbExtrato.SIGLA_QUERY + ".idExtrato");
                        if (!dataBaseQueryEX.join.ContainsKey("LEFT JOIN card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY))
                            dataBaseQueryEX.join.Add("LEFT JOIN card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY, " ON " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato = " + GatewayTbExtrato.SIGLA_QUERY + ".idExtrato");

                        dataBaseQueryEX.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato IS NULL");
                        dataBaseQueryEX.AddWhereClause(GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato IS NULL");

                        List<ConciliacaoBancaria> extratoBancario = new List<ConciliacaoBancaria>();

                        resultado = DataBaseQueries.SqlQuery(dataBaseQueryEX.Script(), connection);
                        if (resultado != null && resultado.Count > 0)
                        {
                            extratoBancario = resultado.Select(t => new ConciliacaoBancaria
                                                        {
                                                            Tipo = TIPO_EXTRATO, // extrato
                                                            Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                                            new ConciliacaoBancaria.ConciliacaoGrupo {
                                                                Id = Convert.ToInt32(t["idExtrato"]),
                                                                Documento = Convert.ToString(t["nrDocumento"]),
                                                                Valor = Convert.ToDecimal(t["vlMovimento"]),
                                                                Filial = Convert.ToString(t["ds_fantasia"].Equals(DBNull.Value) ? "" : t["ds_fantasia"] + (t["filial"].Equals(DBNull.Value) ? "" : " " + t["filial"])).ToUpper(),
                                                            }
                                                        },
                                                            ValorTotal = Convert.ToDecimal(t["vlMovimento"]),
                                                            Data = (DateTime)t["dtExtrato"],
                                                            Adquirente = Convert.ToString(t["nmAdquirente"].Equals(DBNull.Value) ? "" : t["nmAdquirente"]).ToUpper(),
                                                            Memo = Convert.ToString(t["dsDocumento"]),
                                                            Conta = new ConciliacaoBancaria.ConciliacaoConta
                                                                    {
                                                                        CdContaCorrente = Convert.ToInt32(t["cdContaCorrente"]),
                                                                        NrAgencia = Convert.ToString(t["nrAgencia"]),
                                                                        NrConta = Convert.ToString(t["nrConta"]),
                                                                        CdBanco = Convert.ToString(t["cdBanco"])
                                                                    },
                                                            Bandeira = Convert.ToString(t["dsBandeira"].Equals(DBNull.Value) ? "" : t["dsBandeira"]).ToUpper(),
                                                            TipoCartao = Convert.ToString(t["dsTipoCartao"].Equals(DBNull.Value) ? "" : t["dsTipoCartao"]).TrimEnd().ToUpper(),
                                                            Antecipado = Convert.ToBoolean(t["flAntecipacao"].Equals(DBNull.Value) ? false : t["flAntecipacao"]),
                                                            Filial = Convert.ToString(t["ds_fantasia"].Equals(DBNull.Value) ? "" : t["ds_fantasia"] + (t["filial"].Equals(DBNull.Value) ? "" : " " + t["filial"])).ToUpper(),
                                                        })
                                                        .ToList<ConciliacaoBancaria>();
                        }

                        //List<ConciliacaoBancaria> extratoBancario = queryExtrato
                        //    // Só considera os extratos que não estão conciliados
                        //                                        .Where(e => e.RecebimentoParcelas.Count == 0)
                        //                                        .Select(e => new ConciliacaoBancaria
                        //                                        {
                        //                                            Tipo = TIPO_EXTRATO, // extrato
                        //                                            Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                        //                                                            new ConciliacaoBancaria.ConciliacaoGrupo {
                        //                                                                Id = e.idExtrato,
                        //                                                                Documento = e.nrDocumento,
                        //                                                                Valor = e.vlMovimento ?? new decimal(0.0),
                        //                                                                Filial = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => (p.empresa.ds_fantasia + (p.empresa.filial != null ? " " + p.empresa.filial : "")) ?? "").FirstOrDefault() ?? "",
                        //                                                            }
                        //                                                        },
                        //                                            ValorTotal = e.vlMovimento ?? new decimal(0.0),
                        //                                            Data = e.dtExtrato,
                        //                                            Adquirente = _db.tbBancoParametro.Where(p => p.dsMemo.Equals(e.dsDocumento))
                        //                                                                             .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                        //                                                                             .Select(p => p.tbAdquirente.nmAdquirente.ToUpper() ?? "")
                        //                                                                             .FirstOrDefault() ?? "",
                        //                                            Memo = e.dsDocumento,
                        //                                            Conta = new ConciliacaoBancaria.ConciliacaoConta
                        //                                            {
                        //                                                CdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                        //                                                NrAgencia = e.tbContaCorrente.nrAgencia,
                        //                                                NrConta = e.tbContaCorrente.nrConta,
                        //                                                CdBanco = e.tbContaCorrente.cdBanco
                        //                                            },
                        //                                            Bandeira = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.tbBandeira.dsBandeira ?? "").FirstOrDefault() ?? "",
                        //                                            TipoCartao = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.dsTipoCartao.ToUpper().TrimEnd() ?? "").FirstOrDefault() ?? "",
                        //                                            Antecipado = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.flAntecipacao).FirstOrDefault(),
                        //                                            Filial = _db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => (p.empresa.ds_fantasia + (p.empresa.filial != null ? " " + p.empresa.filial : "")) ?? "").FirstOrDefault() ?? "",
                        //                                        }).ToList<ConciliacaoBancaria>();

                        #endregion

                        if (recebimentosParcela.Count > 0)
                        {
                            #region SEPARA PARCELAS COM DATA PREVISTA DE RECEBIMENTO DIFERENTE

                            // Obtém parcelas antecipadas por filial
                            List<ConciliacaoBancaria> parcelasDtRecebOrig = recebimentosParcela.Where(t => t.DataRecebimentoDiferente != null && t.DataRecebimentoDiferente.Value)
                                                                                               .Where(t => t.Antecipado == null || !t.Antecipado.Value)
                                                                                               .GroupBy(t => new { t.Adquirente, t.Data, t.Filial/*, t.DataVenda*/ })
                                                                                               .Select(t => new ConciliacaoBancaria
                                                                                               {
                                                                                                   Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                                   Grupo = t.Select(r => r.Grupo.First()).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                                   ValorTotal = t.Select(r => r.ValorTotal).Sum(),
                                                                                                   ValorTotalBruto = t.Select(r => r.ValorTotalBruto).Sum(),
                                                                                                   Data = t.Key.Data,
                                                                                                   //DataVenda = t.Key.DataVenda,
                                                                                                   DataVenda = t.GroupBy(r => r.DataVenda).Count() == 1 ? t.Select(r => r.DataVenda).FirstOrDefault() : (DateTime?)null,
                                                                                                   Adquirente = t.Key.Adquirente,
                                                                                                   Bandeira = t.GroupBy(r => r.Bandeira).Count() == 1 ? t.Select(r => r.Bandeira).FirstOrDefault() : "",
                                                                                                   Lote = t.GroupBy(r => r.Lote).Count() == 1 ? t.Select(r => r.Lote).FirstOrDefault() : 0,
                                                                                                   AntecipacaoBancaria = t.GroupBy(r => r.AntecipacaoBancaria).Count() == 1 ? t.Select(r => r.AntecipacaoBancaria).FirstOrDefault() : new decimal(0.0),
                                                                                                   TipoCartao = t.GroupBy(r => r.TipoCartao).Count() == 1 ? t.Select(r => r.TipoCartao).FirstOrDefault() : "",
                                                                                                   Antecipado = false,
                                                                                                   DataRecebimentoDiferente = true,
                                                                                                   Filial = t.Key.Filial
                                                                                               })
                                                                                                .ToList<ConciliacaoBancaria>();

                            if (parcelasDtRecebOrig.Count > 0)
                            {
                                // Remove as parcelas para não se envolverem em pré-conciliação
                                recebimentosParcela.RemoveAll(t => t.DataRecebimentoDiferente != null && t.DataRecebimentoDiferente.Value &&
                                                                   (t.Antecipado == null || !t.Antecipado.Value));

                                // Adiciona na lista de não-conciliados
                                if (!filtroTipoPreConciliado)
                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, parcelasDtRecebOrig);
                            }

                            #endregion

                            #region SEPARA PARCELAS ANTECIPADAS, CONCILIANDO COM MOVIMENTAÇÕES BANCÁRIAS DE ANTECIPAÇÃO

                            // Obtém parcelas antecipadas por antecipação bancária
                            List<ConciliacaoBancaria> parcelasAntecipadasBanco = recebimentosParcela.Where(t => t.Antecipado != null && t.Antecipado.Value)
                                                                                               .Where(t => t.AntecipacaoBancaria > new decimal(0.0))
                                                                                               .GroupBy(t => new { t.Adquirente, t.Data, t.AntecipacaoBancaria })
                                                                                               .Select(t => new ConciliacaoBancaria
                                                                                               {
                                                                                                   Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                                   Grupo = t.Select(r => r.Grupo.First()).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                                   ValorTotal = t.Select(r => r.ValorTotal).Sum(),
                                                                                                   ValorTotalBruto = t.Select(r => r.ValorTotalBruto).Sum(),
                                                                                                   Data = t.Key.Data,
                                                                                                   DataVenda = t.GroupBy(r => r.DataVenda).Count() == 1 ? t.Select(r => r.DataVenda).FirstOrDefault() : (DateTime?)null,
                                                                                                   Adquirente = t.Key.Adquirente,
                                                                                                   Bandeira = t.GroupBy(r => r.Bandeira).Count() == 1 ? t.Select(r => r.Bandeira).FirstOrDefault() : "",
                                                                                                   Lote = t.GroupBy(r => r.Lote).Count() == 1 ? t.Select(r => r.Lote).FirstOrDefault() : 0,
                                                                                                   AntecipacaoBancaria = t.Key.AntecipacaoBancaria,
                                                                                                   TipoCartao = t.GroupBy(r => r.TipoCartao).Count() == 1 ? t.Select(r => r.TipoCartao).FirstOrDefault() : "",
                                                                                                   Antecipado = true,
                                                                                                   Filial = t.GroupBy(r => r.Filial).Count() == 1 ? t.Select(r => r.Filial).FirstOrDefault() : "",
                                                                                               })
                                                                                                .ToList<ConciliacaoBancaria>();

                            // Por filial
                            List<ConciliacaoBancaria> parcelasAntecipadas = recebimentosParcela.Where(t => t.Antecipado != null && t.Antecipado.Value)
                                                                                               .Where(t => t.AntecipacaoBancaria == new decimal(0.0))
                                                                                               .GroupBy(t => new { t.Adquirente, t.Data, t.Filial })
                                                                                               .Select(t => new ConciliacaoBancaria
                                                                                               {
                                                                                                   Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                                   Grupo = t.Select(r => r.Grupo.First()).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                                   ValorTotal = t.Select(r => r.ValorTotal).Sum(),
                                                                                                   ValorTotalBruto = t.Select(r => r.ValorTotalBruto).Sum(),
                                                                                                   Data = t.Key.Data,
                                                                                                   DataVenda = t.GroupBy(r => r.DataVenda).Count() == 1 ? t.Select(r => r.DataVenda).FirstOrDefault() : (DateTime?)null,
                                                                                                   Adquirente = t.Key.Adquirente,
                                                                                                   Bandeira = t.GroupBy(r => r.Bandeira).Count() == 1 ? t.Select(r => r.Bandeira).FirstOrDefault() : "",
                                                                                                   Lote = t.GroupBy(r => r.Lote).Count() == 1 ? t.Select(r => r.Lote).FirstOrDefault() : 0,
                                                                                                   AntecipacaoBancaria = new decimal(0.0),
                                                                                                   TipoCartao = t.GroupBy(r => r.TipoCartao).Count() == 1 ? t.Select(r => r.TipoCartao).FirstOrDefault() : "",
                                                                                                   Antecipado = true,
                                                                                                   Filial = t.Key.Filial,
                                                                                               })
                                                                                               .ToList<ConciliacaoBancaria>();

                            List<ConciliacaoBancaria> extratoBancarioAntecipacao = extratoBancario.Where(t => t.Antecipado != null && t.Antecipado.Value)
                                                                                                  .ToList<ConciliacaoBancaria>();

                            if (extratoBancarioAntecipacao.Count > 0)
                                // Tem movimentações bancárias de antecipação
                                extratoBancario.RemoveAll(t => t.Antecipado != null && t.Antecipado.Value);

                            if (parcelasAntecipadas.Count > 0 || parcelasAntecipadasBanco.Count > 0)
                            {
                                // Tem parcelas antecipadas!

                                // Remove todas elas das parcelas a serem pré-conciliadas com as movimentações que não são de antecipação
                                recebimentosParcela.RemoveAll(t => t.Antecipado != null && t.Antecipado.Value);

                                if (extratoBancarioAntecipacao.Count == 0)
                                {
                                    if (!filtroTipoPreConciliado)
                                    {
                                        if (parcelasAntecipadas.Count > 0)
                                            // Adiciona cupom a cupom como não conciliado
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, parcelasAntecipadas);

                                        if (parcelasAntecipadasBanco.Count > 0)
                                            // Adiciona cupom a cupom como não conciliado
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, parcelasAntecipadasBanco);
                                    }
                                }
                                else
                                {
                                    if (parcelasAntecipadasBanco.Count > 0)
                                    {
                                        #region PRÉ-CONCILIA COM MOVIMENTAÇÕES DE ANTECIPAÇÃO, CONSIDERANDO ANTECIPAÇÃO BANCÁRIA
                                        // Tenta pré-conciliar com movimentações de antecipação
                                        for (int k = 0; k < parcelasAntecipadasBanco.Count; k++)
                                        {
                                            ConciliacaoBancaria recebimento = parcelasAntecipadasBanco[k];

                                            // Movimentações da mesma adquirente, data e filial
                                            List<ConciliacaoBancaria> movimentacoes = extratoBancarioAntecipacao.Where(t => t.Adquirente.Equals("") || t.Adquirente.Equals(recebimento.Adquirente))
                                                //.Where(t => t.Filial.Equals("") || recebimento.Filial.Equals("") || t.Filial.Equals(recebimento.Filial))
                                                                                                    .Where(t => t.Data.Year == recebimento.Data.Year)
                                                                                                    .Where(t => t.Data.Month == recebimento.Data.Month)
                                                                                                    .Where(t => t.Data.Day == recebimento.Data.Day)
                                                                                                    .ToList<ConciliacaoBancaria>();
                                            if (movimentacoes.Count == 0)
                                            {
                                                if (!filtroTipoNaoConciliado)
                                                    // Adiciona o cupom como não conciliado
                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, new List<ConciliacaoBancaria>() { recebimento });

                                                continue;
                                            }

                                            decimal menorDiferenca = decimal.MaxValue;
                                            int indice = -1;
                                            // Procura a menor diferença
                                            for (int m = 0; m < movimentacoes.Count; m++)
                                            {
                                                ConciliacaoBancaria mov = movimentacoes[m];
                                                decimal diferenca = Math.Abs(recebimento.AntecipacaoBancaria - mov.ValorTotal);
                                                if (diferenca < menorDiferenca)
                                                {
                                                    indice = m;
                                                    menorDiferenca = diferenca;
                                                }
                                            }

                                            if (menorDiferenca > TOLERANCIA_LOTE)
                                            {
                                                if (!filtroTipoNaoConciliado)
                                                    // Adiciona o cupom como não conciliado
                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, new List<ConciliacaoBancaria>() { recebimento });

                                                continue;
                                            }

                                            // Pega a parcela de menor diferença
                                            ConciliacaoBancaria movimentacao = movimentacoes[indice];

                                            // Deleta a movimentação já pré-conciliada
                                            extratoBancarioAntecipacao.Remove(movimentacao);

                                            // Adiciona na lista de pré-conciliados
                                            if (!filtroTipoNaoConciliado)
                                                adicionaElementosConciliadosNaLista(CollectionConciliacaoBancaria, recebimento, movimentacao, TIPO_CONCILIADO.PRE_CONCILIADO);
                                        }

                                        #endregion

                                        // Clear
                                        parcelasAntecipadasBanco.Clear();
                                    }

                                    if (parcelasAntecipadas.Count > 0)
                                    {
                                        #region PRÉ-CONCILIA COM MOVIMENTAÇÕES DE ANTECIPAÇÃO, CONSIDERANDO ANTECIPAÇÃO DIRETO NA ADQUIRENTE (POR FILIAL)

                                        // Tenta pré-conciliar com movimentações de antecipação
                                        for (int k = 0; k < parcelasAntecipadas.Count; k++)
                                        {
                                            ConciliacaoBancaria recebimento = parcelasAntecipadas[k];

                                            // Movimentações da mesma adquirente, data e filial
                                            List<ConciliacaoBancaria> movimentacoes = extratoBancarioAntecipacao.Where(t => t.Adquirente.Equals("") || t.Adquirente.Equals(recebimento.Adquirente))
                                                                                                    .Where(t => t.Filial.Equals("") || recebimento.Filial.Equals("") || t.Filial.Equals(recebimento.Filial))
                                                                                                    .Where(t => t.Data.Year == recebimento.Data.Year)
                                                                                                    .Where(t => t.Data.Month == recebimento.Data.Month)
                                                                                                    .Where(t => t.Data.Day == recebimento.Data.Day)
                                                                                                    .ToList<ConciliacaoBancaria>();
                                            if (movimentacoes.Count == 0)
                                            {
                                                if (!filtroTipoNaoConciliado)
                                                    // Adiciona o cupom como não conciliado
                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, new List<ConciliacaoBancaria>() { recebimento });

                                                continue;
                                            }

                                            decimal menorDiferenca = decimal.MaxValue;
                                            int indice = -1;
                                            // Procura a menor diferença
                                            for (int m = 0; m < movimentacoes.Count; m++)
                                            {
                                                ConciliacaoBancaria mov = movimentacoes[m];
                                                decimal diferenca = Math.Abs(recebimento.ValorTotal - mov.ValorTotal);
                                                if (diferenca < menorDiferenca)
                                                {
                                                    indice = m;
                                                    menorDiferenca = diferenca;
                                                }
                                            }

                                            if (indice == -1 || (recebimento.ValorTotal <= movimentacoes[indice].ValorTotal && menorDiferenca > TOLERANCIA_LOTE))
                                            {
                                                if (!filtroTipoNaoConciliado)
                                                    // Adiciona o cupom como não conciliado
                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, new List<ConciliacaoBancaria>() { recebimento });

                                                continue;
                                            }

                                            // Pega a parcela de menor diferença
                                            ConciliacaoBancaria movimentacao = movimentacoes[indice];

                                            // SE O VALOR DO RECEBIMENTO ANTECIPADO FOR MAIOR QUE O DO EXTRATO DE MENOR DIFERENÇA
                                            // AVALIA SE TEM OUTRO LANÇAMENTO NO EXTRATO QUE, SOMADOS, IGUALA O VALOR DO RECEBIMENTO...
                                            if (menorDiferenca > TOLERANCIA_LOTE)
                                            {
                                                if (movimentacoes.Count == 1)
                                                {
                                                    // Não pré-conciliou!
                                                    if (!filtroTipoNaoConciliado)
                                                        // Adiciona o cupom como não conciliado
                                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, new List<ConciliacaoBancaria>() { recebimento });

                                                    continue;
                                                }

                                                // Remove o que foi utilizado
                                                movimentacoes.RemoveAt(indice);
                                                // Verifica se a diferença foi complementada com outro lançamento no extrato
                                                ConciliacaoBancaria mov = movimentacoes.Where(t => Math.Abs(t.ValorTotal - menorDiferenca) < TOLERANCIA_LOTE)
                                                                                       .OrderByDescending(t => t.Filial) // prioriza o que tem filial
                                                                                       .FirstOrDefault();

                                                if (mov == null)
                                                {
                                                    // Não pré-conciliou!
                                                    if (!filtroTipoNaoConciliado)
                                                        // Adiciona o cupom como não conciliado
                                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, new List<ConciliacaoBancaria>() { recebimento });

                                                    continue;
                                                }

                                                // Achou => Tenta achar a combinação to conjunto de parcelas para pré-conciliar com essa movimentação

                                                // Obtém as parcelas antecipadas divididas em lotes
                                                List<ConciliacaoBancaria> lotesAntecipados = recebimento.Grupo.GroupBy(r => r.Lote)
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Grupo = r.Select(x => new ConciliacaoBancaria.ConciliacaoGrupo
                                                                        {
                                                                            Antecipado = x.Antecipado,
                                                                            Bandeira = x.Bandeira,
                                                                            DataPrevista = x.DataPrevista,
                                                                            DataVenda = x.DataVenda,
                                                                            Documento = x.Documento,
                                                                            Filial = x.Filial,
                                                                            Id = x.Id,
                                                                            Lote = x.Lote,
                                                                            NumParcela = x.NumParcela,
                                                                            TipoCartao = x.TipoCartao,
                                                                            Valor = x.Valor,
                                                                            ValorBruto = x.ValorBruto
                                                                        }).OrderByDescending(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                        Data = recebimento.Data,
                                                                        ValorTotal = r.Sum(x => x.Valor),
                                                                        ValorTotalBruto = r.Sum(x => x.ValorBruto),
                                                                        Adquirente = recebimento.Adquirente,
                                                                        Bandeira = r.GroupBy(x => x.Bandeira).Count() == 1 ? r.Select(x => x.Bandeira).FirstOrDefault() : "",
                                                                        Lote = r.Key,
                                                                        TipoCartao = r.GroupBy(x => x.TipoCartao).Count() == 1 ? r.Select(x => x.TipoCartao).FirstOrDefault() : "",
                                                                        Antecipado = true,
                                                                        Filial = r.GroupBy(x => x.Filial).Count() == 1 ? r.Select(x => x.Filial).FirstOrDefault() : ""
                                                                    })
                                                                    // Somente os lotes com valor inferior ao da movimentação
                                                                    .Where(t => t.ValorTotal < mov.ValorTotal || Math.Abs(t.ValorTotal - mov.ValorTotal) < TOLERANCIA_LOTE)
                                                                    .ToList<ConciliacaoBancaria>();

                                                // tenta até 5 vezes
                                                List<dynamic> tempList = new List<dynamic>();
                                                int totalTentativas = lotesAntecipados.Count > 20 ? 5 : 1; // se tiver mais de 20 lotes, tenta 5 vezes (pega aletoriamente os lotes), se não, só tenta uma vez
                                                for (int contTentativas = 0; contTentativas < totalTentativas && tempList.Count == 0; contTentativas++)
                                                {
                                                    conciliaPorLotes(tempList, lotesAntecipados, new List<ConciliacaoBancaria>() { mov }, !filtroTipoNaoConciliado);
                                                }

                                                if (tempList.Count == 0)
                                                {
                                                    // Não pré-conciliou
                                                    if (!filtroTipoNaoConciliado)
                                                        // Adiciona o cupom como não conciliado
                                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, new List<ConciliacaoBancaria>() { recebimento });

                                                    continue;
                                                }

                                                // Pré-conciliou!
                                                if (!filtroTipoNaoConciliado)
                                                    CollectionConciliacaoBancaria.AddRange(tempList);

                                                // Remove a movimentação
                                                extratoBancarioAntecipacao.Remove(mov);

                                                // Busca os lotes que foram pré-conciliados
                                                List<int> lotesPreConciliados = new List<int>();
                                                foreach (List<ConciliacaoBancaria.ConciliacaoGrupo> grupo in tempList.Select(t => (List<ConciliacaoBancaria.ConciliacaoGrupo>)t.RecebimentosParcela))
                                                {
                                                    lotesPreConciliados.AddRange(grupo.GroupBy(t => t.Lote).Select(t => t.Key));
                                                }

                                                // Remove possíveis duplicatas
                                                lotesPreConciliados = lotesPreConciliados.Distinct().ToList();

                                                // Remove do grupo os recebíveis dos lotes pré-conciliados
                                                recebimento.Grupo.RemoveAll(t => lotesPreConciliados.Contains(t.Lote));

                                                // Atualiza
                                                recebimento.ValorTotal = recebimento.Grupo.Select(r => r.Valor).Sum();
                                                recebimento.ValorTotalBruto = recebimento.Grupo.Select(r => r.ValorBruto).Sum();
                                                recebimento.DataVenda = recebimento.Grupo.GroupBy(r => r.DataVenda).Count() == 1 ? recebimento.Grupo.Select(r => r.DataVenda).FirstOrDefault() : (DateTime?)null;
                                                recebimento.Bandeira = recebimento.Grupo.GroupBy(r => r.Bandeira).Count() == 1 ? recebimento.Grupo.Select(r => r.Bandeira).FirstOrDefault() : "";
                                                recebimento.Lote = recebimento.Grupo.GroupBy(r => r.Lote).Count() == 1 ? recebimento.Grupo.Select(r => r.Lote).FirstOrDefault() : 0;
                                                recebimento.TipoCartao = recebimento.Grupo.GroupBy(r => r.TipoCartao).Count() == 1 ? recebimento.Grupo.Select(r => r.TipoCartao).FirstOrDefault() : "";

                                            }

                                            // Deleta a movimentação já pré-conciliada
                                            extratoBancarioAntecipacao.Remove(movimentacao);

                                            // Adiciona na lista de pré-conciliados
                                            if (!filtroTipoNaoConciliado)
                                                adicionaElementosConciliadosNaLista(CollectionConciliacaoBancaria, recebimento, movimentacao, TIPO_CONCILIADO.PRE_CONCILIADO);
                                        }
                                        #endregion
                                    }

                                    // Verifica se ainda tem movimentações de antecipação
                                    if (extratoBancarioAntecipacao.Count > 0)
                                    {
                                        if (!filtroTipoPreConciliado)
                                            // Adiciona cupom a cupom como não conciliado
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancarioAntecipacao);

                                        // Limpa a lista
                                        extratoBancarioAntecipacao.Clear();
                                    }
                                }

                                // Limpa a lista
                                parcelasAntecipadas.Clear();

                            }
                            else if (extratoBancarioAntecipacao.Count > 0)
                            {
                                // Não tem parcelas antecipadas, mas tem movimentações bancárias de antecipação

                                // Coloca todas elas como não conciliada
                                if (!filtroTipoPreConciliado)
                                    // Adiciona cupom a cupom como não conciliado
                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancarioAntecipacao);

                                // Limpa a lista
                                extratoBancarioAntecipacao.Clear();
                            }
                            #endregion

                        }

                        // TEM ELEMENTOS PARA CONCILIAR?
                        if (filtroTipoNaoConciliado || recebimentosParcela.Count == 0 || extratoBancario.Count == 0)
                        {
                            if (!filtroTipoPreConciliado)
                            {
                                #region NÃO O QUE CONCILIAR => ADICIONA OS ELEMENTOS COMO NÃO CONCILIADOS
                                if (recebimentosParcela.Count > 0)
                                {
                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria,
                                        // Envia recebimentos agrupados
                                                                recebimentosParcela
                                                                        .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira, r.Lote })
                                                                        .OrderBy(r => r.Key.Data)
                                                                        .ThenBy(r => r.Key.Filial)
                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                        .ThenBy(r => r.Key.Bandeira)
                                                                        .ThenBy(r => r.Key.Lote)
                                                                        .Select(r => new ConciliacaoBancaria
                                                                        {
                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Filial).ThenBy(x => x.Bandeira).ThenByDescending(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                            Data = r.Key.Data,
                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                            ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                            Adquirente = r.Key.Adquirente,
                                                                            Bandeira = r.Key.Bandeira,
                                                                            Lote = r.Key.Lote,
                                                                            TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                            Filial = r.Key.Filial
                                                                        }).ToList<ConciliacaoBancaria>());
                                }
                                if (extratoBancario.Count > 0)
                                {
                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            List<ConciliacaoBancaria> listaCandidatos = new List<ConciliacaoBancaria>();
                            List<ConciliacaoBancaria> listaNaoConciliado = new List<ConciliacaoBancaria>();
                            List<ConciliacaoBancaria> recebimentosParcelaAgrupados = new List<ConciliacaoBancaria>();

                            // Começam conciliando parcelas diretamente
                            // Em seguida, agrupando por Bandeira e Data da Venda
                            #region PASSO 1) CONCILIA DIRETAMENTE RECEBIMENTOPARCELA COM TBEXTRATO

                            // Concatena as duas listas, ordenando por data
                            listaCandidatos = recebimentosParcela.Concat<ConciliacaoBancaria>(extratoBancario)
                                                    .OrderBy(c => c.Data.Year)
                                                    .ThenBy(c => c.Data.Month)
                                                    .ThenBy(c => c.Data.Day)
                                                    .ThenBy(c => c.ValorTotal)
                                                    .ThenByDescending(c => c.Filial)
                                                    .ThenByDescending(c => c.Adquirente)
                                                    .ThenByDescending(c => c.Bandeira)
                                                    .ThenByDescending(c => c.TipoCartao)
                                                    .ToList<ConciliacaoBancaria>();

                            // Faz a conciliação
                            Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado, !filtroTipoNaoConciliado);

                            #endregion

                            // Tem elementos não conciliados?
                            if (listaNaoConciliado.Count > 0)
                            {
                                #region REMOVE DAS LISTAS OS ELEMENTOS JÁ PRÉ-CONCILIADOS
                                recebimentosParcela = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                            .OrderBy(e => e.Data)
                                                                            .ThenBy(e => e.Filial)
                                                                            .ThenBy(e => e.Adquirente)
                                                                            .ThenBy(e => e.Bandeira)
                                                                            .ThenBy(e => e.Lote)
                                                                            .ToList<ConciliacaoBancaria>();

                                extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                        .OrderBy(e => e.Data)
                                                                        .ThenBy(e => e.Adquirente)
                                                                        .ThenBy(e => e.Memo)
                                                                        .ToList<ConciliacaoBancaria>();
                                #endregion

                                // Tem elementos para conciliar?
                                if (recebimentosParcela.Count == 0 || extratoBancario.Count == 0)
                                {
                                    if (!filtroTipoPreConciliado)
                                    {
                                        #region NÃO HÁ MAIS O QUE CONCILIAR => ADICIONA OS ELEMENTOS QUE SOBRARAM COMO NÃO CONCILIADOS
                                        if (recebimentosParcela.Count > 0)
                                        {
                                            //totalRecebimento += recebimentosParcelaAgrupados.Sum(r => r.ValorTotal);
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria,
                                                // Envia recebimentos agrupados
                                                                recebimentosParcela
                                                                        .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira, r.Lote })
                                                                        .OrderBy(r => r.Key.Data)
                                                                        .ThenBy(r => r.Key.Filial)
                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                        .ThenBy(r => r.Key.Bandeira)
                                                                        .ThenBy(r => r.Key.Lote)
                                                                        .Select(r => new ConciliacaoBancaria
                                                                        {
                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderByDescending(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                            Data = r.Key.Data,
                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                            ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                            Adquirente = r.Key.Adquirente,
                                                                            Bandeira = r.Key.Bandeira,
                                                                            Lote = r.Key.Lote,
                                                                            TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                            Filial = r.Key.Filial
                                                                        }).ToList<ConciliacaoBancaria>());
                                        }
                                        else if (extratoBancario.Count > 0)
                                        {
                                            //totalExtrato += extratoBancario.Sum(r => r.ValorTotal);
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region PASSO 2) CONCILIA FAZENDO AGRUPAMENTO POR DATA, DATA VENDA, ADQUIRENTE E BANDEIRA NO RECEBIMENTO PARCELA
                                    // Agrupa por data da venda
                                    recebimentosParcelaAgrupados = recebimentosParcela
                                                                        .GroupBy(r => new { r.Data, r.DataVenda, r.Filial, r.Adquirente, r.Bandeira })
                                                                        .OrderBy(r => r.Key.Data)
                                                                        .ThenBy(r => r.Key.DataVenda)
                                                                        .ThenBy(r => r.Key.Filial)
                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                        .ThenBy(r => r.Key.Bandeira)
                                                                        .Select(r => new ConciliacaoBancaria
                                                                        {
                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Filial).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                            Data = r.Key.Data,
                                                                            DataVenda = r.Key.DataVenda,
                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                            ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                            Adquirente = r.Key.Adquirente,
                                                                            Bandeira = r.Key.Bandeira,
                                                                            Lote = r.GroupBy(x => x.Lote).Count() == 1 ? r.Select(x => x.Lote).FirstOrDefault() : 0,
                                                                            TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                            Filial = r.Key.Filial
                                                                        }).ToList<ConciliacaoBancaria>();

                                    // Concatena as duas listas, ordenando por data
                                    listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                        .OrderBy(c => c.Data.Year)
                                                        .ThenBy(c => c.Data.Month)
                                                        .ThenBy(c => c.Data.Day)
                                                        .ThenBy(c => c.ValorTotal)
                                                        .ThenByDescending(c => c.Filial)
                                                        .ThenByDescending(c => c.Adquirente)
                                                        .ThenByDescending(c => c.Bandeira)
                                                        .ThenByDescending(c => c.TipoCartao)
                                                        .ToList<ConciliacaoBancaria>();

                                    // Faz a conciliação
                                    listaNaoConciliado.Clear();
                                    Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado, !filtroTipoNaoConciliado);

                                    #endregion

                                    #region REMOVE DAS LISTAS OS ELEMENTOS JÁ PRÉ-CONCILIADOS
                                    recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                            .OrderBy(r => r.Data)
                                                                            .ThenBy(r => r.DataVenda)
                                                                            .ThenBy(r => r.Filial)
                                                                            .ThenBy(r => r.Adquirente)
                                                                            .ThenBy(r => r.Bandeira)
                                                                            .ToList<ConciliacaoBancaria>();

                                    recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Grupo.Any(g => g.Id == e.Grupo[0].Id && g.NumParcela == e.Grupo[0].NumParcela)))
                                                                            .OrderBy(e => e.Data)
                                                                            .ThenBy(r => r.Filial)
                                                                            .ThenBy(e => e.Adquirente)
                                                                            .ThenBy(e => e.Bandeira)
                                                                            .ToList<ConciliacaoBancaria>();

                                    extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                            .OrderBy(e => e.Data)
                                                                            .ThenBy(e => e.Adquirente)
                                                                            .ThenBy(e => e.Memo)
                                                                            .ToList<ConciliacaoBancaria>();
                                    #endregion

                                    // Tem elementos não conciliados? 
                                    if (recebimentosParcela.Count == 0 || extratoBancario.Count == 0)
                                    {
                                        if (!filtroTipoPreConciliado)
                                        {
                                            #region NÃO HÁ MAIS O QUE CONCILIAR => ADICIONA OS ELEMENTOS QUE SOBRARAM COMO NÃO CONCILIADOS
                                            if (extratoBancario.Count > 0)
                                            {
                                                //totalExtrato += extratoBancario.Sum(r => r.ValorTotal);
                                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                            }
                                            else if (recebimentosParcela.Count > 0)
                                            {
                                                //totalRecebimento += recebimentosParcelaAgrupados.Sum(r => r.ValorTotal);
                                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria,
                                                    // Envia recebimentos agrupados
                                                                                recebimentosParcela
                                                                                .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira, r.Lote })
                                                                                .OrderBy(r => r.Key.Data)
                                                                                .ThenBy(r => r.Key.Filial)
                                                                                .ThenBy(r => r.Key.Adquirente)
                                                                                .ThenBy(r => r.Key.Bandeira)
                                                                                .ThenBy(r => r.Key.Lote)
                                                                                .Select(r => new ConciliacaoBancaria
                                                                                {
                                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                    Grupo = r.Select(x => x.Grupo[0]).OrderByDescending(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                    Data = r.Key.Data,
                                                                                    ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                    ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                                    Adquirente = r.Key.Adquirente,
                                                                                    Bandeira = r.Key.Bandeira,
                                                                                    Lote = r.Key.Lote,
                                                                                    TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                                    Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                                    Filial = r.Key.Filial
                                                                                }).ToList<ConciliacaoBancaria>());
                                            }
                                            #endregion
                                        }
                                    }
                                    else
                                    {

                                        #region PASSO 3) CONCILIA FAZENDO AGRUPAMENTO POR DATA, ADQUIRENTE E BANDEIRA NO RECEBIMENTO PARCELA (SEM AGRUPAR POR DATA DA VENDA)
                                        recebimentosParcelaAgrupados = recebimentosParcela
                                                                .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira })
                                                                .OrderBy(r => r.Key.Data)
                                                                .ThenBy(r => r.Key.Filial)
                                                                .ThenBy(r => r.Key.Adquirente)
                                                                .ThenBy(r => r.Key.Bandeira)
                                                                .Select(r => new ConciliacaoBancaria
                                                                {
                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                    Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Filial).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                    Data = r.Key.Data,
                                                                    ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                    ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                    Adquirente = r.Key.Adquirente,
                                                                    Bandeira = r.Key.Bandeira,
                                                                    Lote = r.GroupBy(x => x.Lote).Count() == 1 ? r.Select(x => x.Lote).FirstOrDefault() : 0,
                                                                    TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                    Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                    Filial = r.Key.Filial
                                                                }).ToList<ConciliacaoBancaria>();

                                        // Concatena as duas listas, ordenando por data
                                        listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                            .OrderBy(c => c.Data.Year)
                                                            .ThenBy(c => c.Data.Month)
                                                            .ThenBy(c => c.Data.Day)
                                                            .ThenBy(c => c.ValorTotal)
                                                            .ThenByDescending(c => c.Filial)
                                                            .ThenByDescending(c => c.Adquirente)
                                                            .ThenByDescending(c => c.Bandeira)
                                                            .ThenByDescending(c => c.TipoCartao)
                                                            .ToList<ConciliacaoBancaria>();

                                        // Faz a conciliação
                                        listaNaoConciliado.Clear();
                                        Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado, !filtroTipoNaoConciliado);

                                        #endregion

                                        // Ainda tem elementos não conciliados?
                                        if (listaNaoConciliado.Count > 0)
                                        {
                                            #region REMOVE DA LISTA OS ELEMENTOS JÁ PRÉ-CONCILIADOS
                                            recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                                    .OrderBy(r => r.Data)
                                                                                    .ThenBy(r => r.Filial)
                                                                                    .ThenBy(r => r.Adquirente)
                                                                                    .ThenBy(r => r.Bandeira)
                                                                                    .ToList<ConciliacaoBancaria>();

                                            recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Grupo.Any(g => g.Id == e.Grupo[0].Id && g.NumParcela == e.Grupo[0].NumParcela)))
                                                                            .OrderBy(e => e.Data)
                                                                            .ThenBy(r => r.Filial)
                                                                            .ThenBy(e => e.Adquirente)
                                                                            .ThenBy(e => e.Bandeira)
                                                                            .ToList<ConciliacaoBancaria>();

                                            extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                                    .OrderBy(e => e.Data)
                                                                                    .ThenBy(e => e.Adquirente)
                                                                                    .ThenBy(e => e.Memo)
                                                                                    .ToList<ConciliacaoBancaria>();

                                            #endregion

                                            // Tem elementos para conciliar?
                                            if (recebimentosParcelaAgrupados.Count == 0 || extratoBancario.Count == 0)
                                            {
                                                if (!filtroTipoPreConciliado)
                                                {
                                                    #region NÃO HÁ MAIS O QUE CONCILIAR => ADICIONA OS ELEMENTOS QUE SOBRARAM COMO NÃO CONCILIADOS
                                                    if (extratoBancario.Count > 0)
                                                    {
                                                        //totalExtrato += extratoBancario.Sum(r => r.ValorTotal);
                                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                                    }
                                                    else if (recebimentosParcela.Count > 0)
                                                    {
                                                        //totalRecebimento += recebimentosParcelaAgrupados.Sum(r => r.ValorTotal);
                                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria,
                                                            // Envia recebimentos agrupados
                                                                                recebimentosParcela
                                                                                .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira, r.Lote })
                                                                                .OrderBy(r => r.Key.Data)
                                                                                .ThenBy(r => r.Key.Filial)
                                                                                .ThenBy(r => r.Key.Adquirente)
                                                                                .ThenBy(r => r.Key.Bandeira)
                                                                                .ThenBy(r => r.Key.Lote)
                                                                                .Select(r => new ConciliacaoBancaria
                                                                                {
                                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                    Grupo = r.Select(x => x.Grupo[0]).OrderByDescending(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                    Data = r.Key.Data,
                                                                                    ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                    ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                                    Adquirente = r.Key.Adquirente,
                                                                                    Bandeira = r.Key.Bandeira,
                                                                                    Lote = r.Key.Lote,
                                                                                    TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                                    Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                                    Filial = r.Key.Filial
                                                                                }).ToList<ConciliacaoBancaria>());
                                                    }
                                                    #endregion
                                                }
                                            }
                                            else
                                            {

                                                #region PASSO 4) TENTA ENCONTRAR SUBGRUPOS DE CADA AGRUPAMENTO ENVOLVENDO DATA, ADQUIRENTE E BANDEIRA NO RECEBIMENTO PARCELA
                                                //extratoBancario =  extratoBancario.OrderByDescending(t => t.ValorTotal).ToList<ConciliacaoBancaria>();
                                                conciliaSubGrupos(CollectionConciliacaoBancaria, recebimentosParcelaAgrupados, extratoBancario, !filtroTipoNaoConciliado);
                                                #endregion

                                                // Tem elementos para conciliar?
                                                if (recebimentosParcelaAgrupados.Count == 0 || extratoBancario.Count == 0)
                                                {
                                                    if (!filtroTipoPreConciliado)
                                                    {
                                                        #region NÃO HÁ MAIS O QUE CONCILIAR => ADICIONA OS ELEMENTOS QUE SOBRARAM COMO NÃO CONCILIADOS
                                                        if (extratoBancario.Count > 0)
                                                        {
                                                            //totalExtrato += extratoBancario.Sum(r => r.ValorTotal);
                                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                                        }
                                                        else if (recebimentosParcelaAgrupados.Count > 0)
                                                        {
                                                            //totalRecebimento += recebimentosParcelaAgrupados.Sum(r => r.ValorTotal);
                                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria,
                                                                // Envia recebimentos agrupados
                                                                                    recebimentosParcelaAgrupados);
                                                        }
                                                        #endregion
                                                    }
                                                }
                                                else
                                                {
                                                    #region PASSO 5) AGRUPA SEM CONSIDERAR A BANDEIRA (SOMENTE O TIPO)

                                                    recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Grupo.Any(g => g.Id == e.Grupo[0].Id && g.NumParcela == e.Grupo[0].NumParcela)))
                                                                                .OrderBy(e => e.Data)
                                                                                .ThenBy(r => r.Filial)
                                                                                .ThenBy(e => e.Adquirente)
                                                                                .ThenBy(e => e.Bandeira)
                                                                                .ToList<ConciliacaoBancaria>();

                                                    recebimentosParcelaAgrupados = recebimentosParcela
                                                                                        .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.TipoCartao })
                                                                                        .OrderBy(r => r.Key.Data)
                                                                                        .ThenBy(r => r.Key.Filial)
                                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                                        .ThenBy(r => r.Key.TipoCartao)
                                                                                        .Select(r => new ConciliacaoBancaria
                                                                                        {
                                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Filial).ThenBy(x => x.Bandeira).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                            Data = r.Key.Data,
                                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                            ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                                            Adquirente = r.Key.Adquirente,
                                                                                            Filial = r.Key.Filial,
                                                                                            Bandeira = r.GroupBy(x => x.Bandeira).Count() == 1 ? r.Select(x => x.Bandeira).FirstOrDefault() : "",
                                                                                            Lote = r.GroupBy(x => x.Lote).Count() == 1 ? r.Select(x => x.Lote).FirstOrDefault() : 0,
                                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                                            TipoCartao = r.Key.TipoCartao,
                                                                                        }).ToList<ConciliacaoBancaria>();

                                                    // Concatena as duas listas, ordenando por data
                                                    listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                                        .OrderBy(c => c.Data.Year)
                                                                        .ThenBy(c => c.Data.Month)
                                                                        .ThenBy(c => c.Data.Day)
                                                                        .ThenBy(c => c.ValorTotal)
                                                                        .ThenByDescending(c => c.Filial)
                                                                        .ThenByDescending(c => c.Adquirente)
                                                                        .ThenByDescending(c => c.Bandeira)
                                                                        .ThenByDescending(c => c.TipoCartao)
                                                                        .ToList<ConciliacaoBancaria>();

                                                    // Faz a conciliação
                                                    listaNaoConciliado.Clear();
                                                    Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado, !filtroTipoNaoConciliado);

                                                    #endregion

                                                    if (listaNaoConciliado.Count > 0)
                                                    {
                                                        #region REMOVE DA LISTA OS ELEMENTOS JÁ PRÉ-CONCILIADOS
                                                        recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                                                    .OrderBy(r => r.Data)
                                                                                                    .ThenBy(r => r.Filial)
                                                                                                    .ThenBy(r => r.Adquirente)
                                                                                                    .ToList<ConciliacaoBancaria>();

                                                        recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Grupo.Any(g => g.Id == e.Grupo[0].Id && g.NumParcela == e.Grupo[0].NumParcela)))
                                                                                .OrderBy(e => e.Data)
                                                                                .ThenBy(r => r.Filial)
                                                                                .ThenBy(e => e.Adquirente)
                                                                                .ThenBy(e => e.Bandeira)
                                                                                .ToList<ConciliacaoBancaria>();

                                                        recebimentosParcelaAgrupados = recebimentosParcela
                                                                                        .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira, r.Lote })
                                                                                        .OrderBy(r => r.Key.Data)
                                                                                        .ThenBy(r => r.Key.Filial)
                                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                                        .ThenBy(r => r.Key.Bandeira)
                                                                                        .ThenBy(r => r.Key.Lote)
                                                                                        .Select(r => new ConciliacaoBancaria
                                                                                        {
                                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Filial).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                            Data = r.Key.Data,
                                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                            ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                                            Adquirente = r.Key.Adquirente,
                                                                                            Bandeira = r.Key.Bandeira,
                                                                                            Lote = r.Key.Lote,
                                                                                            TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                                            Filial = r.Key.Filial
                                                                                        }).ToList<ConciliacaoBancaria>();

                                                        extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                                                .OrderBy(e => e.Data)
                                                                                                .ThenBy(e => e.Adquirente)
                                                                                                .ThenBy(e => e.Memo)
                                                                                                .ToList<ConciliacaoBancaria>();

                                                        #endregion

                                                        // Tem elementos para conciliar?
                                                        if (recebimentosParcelaAgrupados.Count == 0 || extratoBancario.Count == 0)
                                                        {
                                                            if (!filtroTipoPreConciliado)
                                                            {
                                                                #region NÃO HÁ MAIS O QUE CONCILIAR => ADICIONA OS ELEMENTOS QUE SOBRARAM COMO NÃO CONCILIADOS
                                                                if (extratoBancario.Count > 0)
                                                                {
                                                                    //totalExtrato += extratoBancario.Sum(r => r.ValorTotal);
                                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                                                }
                                                                else if (recebimentosParcelaAgrupados.Count > 0)
                                                                {
                                                                    //totalRecebimento += recebimentosParcelaAgrupados.Sum(r => r.ValorTotal);
                                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria,
                                                                        // Envia recebimentos agrupados
                                                                                            recebimentosParcelaAgrupados);
                                                                }
                                                                #endregion
                                                            }
                                                        }
                                                        else
                                                        {

                                                            if (cdAdquirente.Equals("2") || cdAdquirente.Equals("1"))
                                                            {
                                                                #region PASSO 5.1) CONCILIA FAZENDO AGRUPAMENTO POR DATA, ADQUIRENTE, BANDEIRA E LOTE NO RECEBIMENTO PARCELA (SEM AGRUPAR POR DATA DA VENDA)

                                                                //// Concatena as duas listas, ordenando por data
                                                                //listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                                //                  .OrderBy(c => c.Data.Year)
                                                                //                  .ThenBy(c => c.Data.Month)
                                                                //                  .ThenBy(c => c.Data.Day)
                                                                //                  .ThenBy(c => c.ValorTotal)
                                                                //                  .ThenByDescending(c => c.Filial)
                                                                //                  .ThenByDescending(c => c.Adquirente)
                                                                //                  .ThenByDescending(c => c.Bandeira)
                                                                //                  .ThenByDescending(c => c.TipoCartao)
                                                                //                  .ThenByDescending(c => c.Lote)
                                                                //                  .ToList<ConciliacaoBancaria>();

                                                                conciliaPorLotes(CollectionConciliacaoBancaria, recebimentosParcelaAgrupados, extratoBancario, !filtroTipoNaoConciliado);
                                                                #endregion
                                                            }

                                                            // Adquirente SODEXO concilia por bandeira
                                                            if ((!contaCorrente.Equals("") || cdAdquirente.Equals("11")) && recebimentosParcela.GroupBy(r => r.Filial).Count() > 1)
                                                            {
                                                                #region Conta amarrada => AGRUPA REGISTROS SEM CONSIDERAR FILIAL
                                                                recebimentosParcelaAgrupados = recebimentosParcela
                                                                                        .GroupBy(r => new { r.Data, r.Adquirente, r.Bandeira })
                                                                                        .OrderBy(r => r.Key.Data)
                                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                                        .ThenBy(r => r.Key.Bandeira)
                                                                                        .Select(r => new ConciliacaoBancaria
                                                                                        {
                                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Filial).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                            Data = r.Key.Data,
                                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                            ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                                            Adquirente = r.Key.Adquirente,
                                                                                            Bandeira = r.Key.Bandeira,
                                                                                            TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                                            Filial = ""
                                                                                        }).ToList<ConciliacaoBancaria>();

                                                                // Concatena as duas listas, ordenando por data
                                                                listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                                                    .OrderBy(c => c.Data.Year)
                                                                                    .ThenBy(c => c.Data.Month)
                                                                                    .ThenBy(c => c.Data.Day)
                                                                                    .ThenBy(c => c.ValorTotal)
                                                                    //.ThenByDescending(c => c.Filial)
                                                                                    .ThenByDescending(c => c.Adquirente)
                                                                                    .ThenByDescending(c => c.Bandeira)
                                                                                    .ThenByDescending(c => c.TipoCartao)
                                                                                    .ToList<ConciliacaoBancaria>();

                                                                // Faz a conciliação
                                                                listaNaoConciliado.Clear();
                                                                Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado, !filtroTipoNaoConciliado);

                                                                #region REMOVE DA LISTA OS ELEMENTOS JÁ PRÉ-CONCILIADOS
                                                                recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                                                            .OrderBy(r => r.Data)
                                                                    //.ThenBy(r => r.Filial)
                                                                                                            .ThenBy(r => r.Adquirente)
                                                                                                            .ToList<ConciliacaoBancaria>();

                                                                recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Grupo.Any(g => g.Id == e.Grupo[0].Id && g.NumParcela == e.Grupo[0].NumParcela)))
                                                                                        .OrderBy(e => e.Data)
                                                                                        .ThenBy(r => r.Filial)
                                                                                        .ThenBy(e => e.Adquirente)
                                                                                        .ThenBy(e => e.Bandeira)
                                                                                        .ToList<ConciliacaoBancaria>();

                                                                extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                                                        .OrderBy(e => e.Data)
                                                                                                        .ThenBy(e => e.Adquirente)
                                                                                                        .ThenBy(e => e.Memo)
                                                                                                        .ToList<ConciliacaoBancaria>();

                                                                #endregion

                                                                if (recebimentosParcela.Count > 0 && extratoBancario.Count > 0)
                                                                {
                                                                    #region TENTA PRÉ-CONCILIAR POR TIPO CARTÃO

                                                                    recebimentosParcelaAgrupados = recebimentosParcela
                                                                                        .GroupBy(r => new { r.Data, r.Adquirente, r.TipoCartao })
                                                                                        .OrderBy(r => r.Key.Data)
                                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                                        .ThenBy(r => r.Key.TipoCartao)
                                                                                        .Select(r => new ConciliacaoBancaria
                                                                                        {
                                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Filial).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                            Data = r.Key.Data,
                                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                            ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                                            Adquirente = r.Key.Adquirente,
                                                                                            Bandeira = "",
                                                                                            TipoCartao = r.Key.TipoCartao,
                                                                                            Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                                            Filial = ""
                                                                                        }).ToList<ConciliacaoBancaria>();

                                                                    // Concatena as duas listas, ordenando por data
                                                                    listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                                                        .OrderBy(c => c.Data.Year)
                                                                                        .ThenBy(c => c.Data.Month)
                                                                                        .ThenBy(c => c.Data.Day)
                                                                                        .ThenBy(c => c.ValorTotal)
                                                                        //.ThenByDescending(c => c.Filial)
                                                                                        .ThenByDescending(c => c.Adquirente)
                                                                        //.ThenByDescending(c => c.Bandeira)
                                                                                        .ThenByDescending(c => c.TipoCartao)
                                                                                        .ToList<ConciliacaoBancaria>();

                                                                    // Faz a conciliação
                                                                    listaNaoConciliado.Clear();
                                                                    Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado, !filtroTipoNaoConciliado);

                                                                    #endregion


                                                                    #region REMOVE DA LISTA OS ELEMENTOS JÁ PRÉ-CONCILIADOS
                                                                    recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                                                                .OrderBy(r => r.Data)
                                                                        //.ThenBy(r => r.Filial)
                                                                                                                .ThenBy(r => r.Adquirente)
                                                                                                                .ToList<ConciliacaoBancaria>();

                                                                    recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Grupo.Any(g => g.Id == e.Grupo[0].Id && g.NumParcela == e.Grupo[0].NumParcela)))
                                                                                            .OrderBy(e => e.Data)
                                                                                            .ThenBy(r => r.Filial)
                                                                                            .ThenBy(e => e.Adquirente)
                                                                                            .ThenBy(e => e.Bandeira)
                                                                                            .ToList<ConciliacaoBancaria>();

                                                                    extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                                                            .OrderBy(e => e.Data)
                                                                                                            .ThenBy(e => e.Adquirente)
                                                                                                            .ThenBy(e => e.Memo)
                                                                                                            .ToList<ConciliacaoBancaria>();

                                                                    #endregion

                                                                }

                                                                // Obtém o agrupamento padrão de recebimentos
                                                                recebimentosParcelaAgrupados = recebimentosParcela
                                                                                                .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira, r.Lote })
                                                                                                .OrderBy(r => r.Key.Data)
                                                                                                .ThenBy(r => r.Key.Filial)
                                                                                                .ThenBy(r => r.Key.Adquirente)
                                                                                                .ThenBy(r => r.Key.Bandeira)
                                                                                                .ThenBy(r => r.Key.Lote)
                                                                                                .Select(r => new ConciliacaoBancaria
                                                                                                {
                                                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                                    Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Filial).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Lote).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                                    Data = r.Key.Data,
                                                                                                    ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                                    ValorTotalBruto = r.Sum(x => x.Grupo[0].ValorBruto),
                                                                                                    Adquirente = r.Key.Adquirente,
                                                                                                    Bandeira = r.Key.Bandeira,
                                                                                                    Lote = r.Key.Lote,
                                                                                                    TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                                                    Antecipado = r.GroupBy(x => x.Antecipado).Count() == 1 ? r.Select(x => x.Antecipado).FirstOrDefault() : (bool?)null,
                                                                                                    Filial = r.Key.Filial
                                                                                                }).ToList<ConciliacaoBancaria>();


                                                                #endregion
                                                            }

                                                            if (recebimentosParcelaAgrupados.Count > 0 && extratoBancario.Count > 0)
                                                            {
                                                                #region PASSO 6) TENTA A ÚLTIMA CONCILIAÇÃO, CONSIDERANDO APENAS OS QUE SOBRARAM APÓS TODAS AS ETAPAS ANTERIORES

                                                                // Concatena as duas listas, ordenando por data
                                                                listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                                                    .OrderBy(c => c.Data.Year)
                                                                                    .ThenBy(c => c.Data.Month)
                                                                                    .ThenBy(c => c.Data.Day)
                                                                                    .ThenBy(c => c.ValorTotal)
                                                                                    .ThenByDescending(c => c.Filial)
                                                                                    .ThenByDescending(c => c.Adquirente)
                                                                                    .ThenByDescending(c => c.Bandeira)
                                                                                    .ThenByDescending(c => c.TipoCartao)
                                                                                    .ToList<ConciliacaoBancaria>();

                                                                // Faz a conciliação
                                                                listaNaoConciliado.Clear();
                                                                Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado, !filtroTipoNaoConciliado);

                                                                #endregion

                                                                #region REMOVE DA LISTA OS ELEMENTOS JÁ PRÉ-CONCILIADOS
                                                                recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                                                            .OrderBy(r => r.Data)
                                                                                                            .ThenBy(r => r.Filial)
                                                                                                            .ThenBy(r => r.Adquirente)
                                                                                                            .ThenBy(r => r.Bandeira)
                                                                                                            .ThenBy(r => r.Lote)
                                                                                                            .ToList<ConciliacaoBancaria>();

                                                                extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                                                        .OrderBy(e => e.Data)
                                                                                                        .ThenBy(e => e.Adquirente)
                                                                                                        .ThenBy(e => e.Memo)
                                                                                                        .ToList<ConciliacaoBancaria>();

                                                                #endregion
                                                            }

                                                            if (!filtroTipoPreConciliado)
                                                            {
                                                                #region PASSO 7) ADICIONA OS ELEMENTOS QUE SOBRARAM COMO NÃO CONCILIADOS
                                                                if (recebimentosParcelaAgrupados.Count > 0)
                                                                {
                                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcelaAgrupados);
                                                                }
                                                                if (extratoBancario.Count > 0)
                                                                {
                                                                    //totalExtrato += extratoBancario.Sum(r => r.ValorTotal);
                                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                                                }
                                                                #endregion
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                        }
                        #endregion
                    }
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

                transaction.Commit();

                // Ordena
                CollectionConciliacaoBancaria = CollectionConciliacaoBancaria
                                                                .OrderBy(c => c.Data.Year)
                                                                .ThenBy(c => c.Data.Month)
                                                                .ThenBy(c => c.Data.Day)
                                                                .ThenBy(c => c.RecebimentosParcela != null ? c.ValorTotalRecebimento : c.ValorTotalExtrato)
                                                                .ThenByDescending(c => c.Adquirente)
                                                                .ThenByDescending(c => c.Bandeira)
                                                                .ToList<dynamic>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionConciliacaoBancaria.Count;

                // TOTAL
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valorExtrato", CollectionConciliacaoBancaria.Select(r => r.ValorTotalExtrato).Cast<decimal>().Sum());
                retorno.Totais.Add("valorRecebimento", CollectionConciliacaoBancaria.Select(r => r.ValorTotalRecebimento).Cast<decimal>().Sum());

                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionConciliacaoBancaria = CollectionConciliacaoBancaria.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionConciliacaoBancaria;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar adquirente" : erro);
                }
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


        /// <summary>
        /// Aponta RecebimentoParcela para Extrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Post(string token, List<ConciliaRecebimentoParcela> param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                for (int k = 0; k < param.Count; k++)
                {
                    ConciliaRecebimentoParcela grupoExtrato = param[k];
                    if (grupoExtrato.recebimentosParcela != null)
                    {
                        // Avalia o extrato
                        tbExtrato extrato = null;
                        if (grupoExtrato.idExtrato > 0)
                        {
                            //extrato = _db.tbExtratos.Where(e => e.idExtrato == grupoExtrato.idExtrato).FirstOrDefault();
                            extrato = _db.Database.SqlQuery<tbExtrato>("SELECT E.*" +
                                                                       " FROM card.tbExtrato E (NOLOCK)" +
                                                                       " WHERE E.idExtrato = " + grupoExtrato.idExtrato)
                                                          .FirstOrDefault();
                            if (extrato == null) continue; // extrato inválido!
                        }


                        List<ConciliaRecebimentoParcela.RecebParcela> groupBy = grupoExtrato.recebimentosParcela.GroupBy(t => new { t.idRecebimento, t.numParcela })
                                                                                .Select(t => new ConciliaRecebimentoParcela.RecebParcela
                                                                                {
                                                                                    idRecebimento = t.Key.idRecebimento,
                                                                                    numParcela = t.Key.numParcela
                                                                                }).ToList<ConciliaRecebimentoParcela.RecebParcela>();

                        for (int g = 0; g < grupoExtrato.recebimentosParcela.Count; g++)
                        {
                            ConciliaRecebimentoParcela.RecebParcela recebimentoParcela = grupoExtrato.recebimentosParcela[g];
                            if (recebimentoParcela.numParcela == -1)
                            {
                                // AJUSTE
                                _db.Database.ExecuteSqlCommand(
                                        "UPDATE " + GatewayTbRecebimentoAjuste.SIGLA_QUERY +
                                        " SET " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato = " + (grupoExtrato.idExtrato == -1 ? "NULL" : grupoExtrato.idExtrato.ToString()) +
                                        " FROM card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY +
                                        " WHERE " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idRecebimentoAjuste = " + recebimentoParcela.idRecebimento
                                        );

                            }
                            else
                            {
                                // RECEBIMENTO PARCELA
                                _db.Database.ExecuteSqlCommand(
                                        "UPDATE " + GatewayRecebimentoParcela.SIGLA_QUERY +
                                        " SET " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato = " + (grupoExtrato.idExtrato == -1 ? "NULL" :
                                        grupoExtrato.idExtrato.ToString() + ", " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo = '" + DataBaseQueries.GetDate(extrato.dtExtrato) + "'") +
                                        " FROM pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY +
                                        " WHERE " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimento = " + recebimentoParcela.idRecebimento +
                                            " AND " + GatewayRecebimentoParcela.SIGLA_QUERY + ".numParcela = " + recebimentoParcela.numParcela
                                        );

                            }
                            _db.SaveChanges();
                        }
                    }

                }

                transaction.Commit();

            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar recebimento parcela" : erro);
                }
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



        /// <summary>
        /// Altera data de Recebimento Efetivo do RecebimentoParcela e/ou tbRecebimentoAjuste
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, RecebimentosParcela param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {

                if (param == null || param.dtaRecebimentoEfetivo == null || param.recebimentosParcela == null)
                    throw new Exception("Argumento inválido");

                foreach (RecebimentosParcela.RecebParcela recebimentoParcela in param.recebimentosParcela)
                {
                    if (recebimentoParcela.numParcela == -1)
                    {
                        // AJUSTE
                        _db.Database.ExecuteSqlCommand(
                                "UPDATE " + GatewayTbRecebimentoAjuste.SIGLA_QUERY +
                                " SET " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".dtAjuste = '" + DataBaseQueries.GetDate(param.dtaRecebimentoEfetivo) + "'" +
                                " FROM card.tbRecebimentoAjuste " + GatewayTbRecebimentoAjuste.SIGLA_QUERY +
                                " WHERE " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idRecebimentoAjuste = " + recebimentoParcela.idRecebimento +
                                  " AND " + GatewayTbRecebimentoAjuste.SIGLA_QUERY + ".idExtrato IS NULL"
                                );

                    }
                    else
                    {
                        // RECEBIMENTO PARCELA
                        _db.Database.ExecuteSqlCommand(
                                "UPDATE " + GatewayRecebimentoParcela.SIGLA_QUERY +
                                " SET " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo = '" + DataBaseQueries.GetDate(param.dtaRecebimentoEfetivo) + "'" +
                                " FROM pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY +
                                " WHERE " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimento = " + recebimentoParcela.idRecebimento +
                                    " AND " + GatewayRecebimentoParcela.SIGLA_QUERY + ".numParcela = " + recebimentoParcela.numParcela +
                                    " AND " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idExtrato IS NULL"
                                );

                    }
                    _db.SaveChanges();
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar recebimento parcela" : erro);
                }
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




        /// <summary>
        /// Gera arquivos de baixa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<List<string>> Patch(string token, List<BaixaTitulos> param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                List<List<string>> arquivos = new List<List<String>>();
                foreach (BaixaTitulos baixa in param)
                {
                    arquivos.Add(_db.Database.SqlQuery<string>("EXECUTE [card].[sp_GeraCsvArquivoBaixa_DealerNet] " + baixa.idExtrato).ToList<string>());
                }

                return arquivos;

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao gerar a baixa automática" : erro);
                }
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

    }

}
