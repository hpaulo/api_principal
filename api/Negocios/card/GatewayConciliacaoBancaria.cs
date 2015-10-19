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
            TIPO = 101,  // 1 : CONCILIADO, 2 : PRÉ-CONCILIADO, 3 : NÃO-CONCILIADO
            ID_GRUPO = 102,
            NU_CNPJ = 103,

            // RELACIONAMENTOS
            IDOPERADORA = 200,

            CDADQUIRENTE = 300,
        };

        public enum TIPO_CONCILIADO
        {
            CONCILIADO = 1,
            PRE_CONCILIADO = 2,
            NAO_CONCILIADO = 3
        };

        private static string TIPO_EXTRATO = "E";
        private static string TIPO_RECEBIMENTO = "R";
        private static decimal TOLERANCIA = new decimal(0.03); // R$0.03 de tolerância para avaliar pré-conciliação


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
                    RecebimentosParcela = item.Tipo != TIPO_RECEBIMENTO ? null : item.Grupo.OrderBy(x => x.Bandeira).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                    Adquirente = item.Adquirente,
                    Bandeira = item.Bandeira,
                    Data = item.Data,
                    ValorTotalRecebimento = item.Tipo == TIPO_RECEBIMENTO ? item.ValorTotal : new decimal(0.0),
                    ValorTotalExtrato = item.Tipo == TIPO_EXTRATO ? item.ValorTotal : new decimal(0.0),
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
                    RecebimentosParcela = recebimento.Grupo.OrderBy(x => x.Bandeira).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                    Adquirente = movimentacao.Adquirente,
                    Bandeira = recebimento.Bandeira,
                    Memo = movimentacao.Memo,
                    Data = recebimento.Data,
                    ValorTotalRecebimento = recebimento.ValorTotal,
                    ValorTotalExtrato = movimentacao.ValorTotal,
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
                if (item2 == null || item1.Tipo.Equals(item2.Tipo) || // CREDIT/DEBIT
                   // Adquirente
                   !item1.Adquirente.Equals(item1.Adquirente) || 
                   // Bandeira
                   (item1.Bandeira != null && item2.Bandeira != null && !item1.Bandeira.Equals("") && !item2.Bandeira.Equals("") && !item1.Bandeira.Equals(item2.Bandeira)) ||
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




        // SUBGRUPOS
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

                    // Só avalia os de mesma adquirente e mesma data
                    if (recebimento.Adquirente.Equals(extrato.Adquirente) &&
                        recebimento.Data.Year == extrato.Data.Year &&
                        recebimento.Data.Month == extrato.Data.Month &&
                        recebimento.Data.Day == extrato.Data.Day &&
                        (extrato.Bandeira.Equals("") || extrato.Bandeira.Equals(recebimento.Bandeira)) &&
                        (extrato.TipoCartao.Equals("") || extrato.TipoCartao.Equals(recebimento.TipoCartao)))
                    {
                        // Pega somente os elementos que o valor é inferior ao total da movimentação bancária
                        List<ConciliacaoBancaria.ConciliacaoGrupo> gs = recebimento.Grupo.Where(g => g.Valor <= extrato.ValorTotal + TOLERANCIA)
                                                                                         .OrderByDescending(g => g.Valor).Take(20) // OTIMIZAÇÃO: encontrar a soma dentro de um grupo é conhecido como Subset Sum Problem, que é um problema NP. Em virtude disso, um conjunto com muitos elementos pode gerar um processamento muito longo e, por isso, foi escolhido apenas os 20 maiores valores para tentar "casar" com o valor total
                                                                                         .ToList<ConciliacaoBancaria.ConciliacaoGrupo>();
                        if (gs.Select(g => g.Valor).Sum() >= extrato.ValorTotal + TOLERANCIA)
                        {
                            foreach (List<List<ConciliacaoBancaria.ConciliacaoGrupo>> g in GetCombinations(gs, extrato.ValorTotal))
                            //foreach (ConciliacaoBancaria.ConciliacaoGrupo[] item in KnapsackConciliacaoBancaria.MatchTotalConciliacaoBancaria(recebimento.Grupo, extrato.ValorTotal, TOLERANCIA))
                            {
                                foreach (List<ConciliacaoBancaria.ConciliacaoGrupo> item in g)
                                {
                                    grupos.Add(new
                                    {
                                        bandeira = recebimento.Bandeira,
                                        tipo = recebimento.TipoCartao,
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
                        recebimento = grupos
                    });
                }
            }
            #endregion

            if (gruposExtrato.Count > 0)
            {
                #region PROCESSA COMBINAÇÕES E CONCILIA EVITANDO DUPLICIDADES
                // Ordena por total de combinações encontradas
                gruposExtrato = gruposExtrato.OrderBy(g => g.recebimento.Count).ToList<dynamic>();
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
                            string bandeira = String.Empty;
                            string dsTipoCartao = String.Empty;
                            if (grupo.GroupBy(g => g.Bandeira).Count() == 1)
                                bandeira = grupo.Select(g => g.Bandeira).FirstOrDefault();
                            if (grupo.GroupBy(g => g.TipoCartao).Count() == 1)
                                dsTipoCartao = grupo.Select(g => g.TipoCartao).FirstOrDefault();

                            ConciliacaoBancaria recebimento = new ConciliacaoBancaria()
                            {
                                Tipo = TIPO_RECEBIMENTO,
                                Data = movimentacao.Data,
                                Grupo = grupo,
                                Adquirente = movimentacao.Adquirente,
                                Bandeira = bandeira,
                                TipoCartao = dsTipoCartao,
                                ValorTotal = grupo.Sum(g => g.Valor),
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
                            if (newGrupo.GroupBy(g => g.Bandeira).Count() == 1)
                                bandeira = newGrupo.Select(g => g.Bandeira).FirstOrDefault();
                            if (newGrupo.GroupBy(g => g.TipoCartao).Count() == 1)
                                dsTipoCartao = newGrupo.Select(g => g.TipoCartao).FirstOrDefault();

                            // Só "re-adiciona" o grupo modificado caso possua elementos
                            if (newGrupo.Count > 0)
                            {
                                ConciliacaoBancaria newRp = new ConciliacaoBancaria()
                                {
                                    Adquirente = rp.Adquirente,
                                    Tipo = rp.Tipo,
                                    Bandeira = bandeira,//rp.Bandeira,
                                    TipoCartao = dsTipoCartao,//rp.TipoCartao,
                                    Conta = rp.Conta,
                                    Data = rp.Data,
                                    DataVenda = rp.DataVenda,
                                    Grupo = newGrupo.OrderBy(g => g.Bandeira).ThenBy(g => g.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                    Memo = rp.Memo,
                                    ValorTotal = newGrupo.Sum(g => g.Valor),
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




        /// <summary>
        /// Retorna a lista de conciliação bancária
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
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
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
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
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
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE, "0"); // != null
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
                    queryStringExtrato["" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE] = cdAdquirente;
                    
                }

                // Vigência
                vigencia = CnpjEmpresa;
                if (!data.Equals("")) {
                    vigencia += "!" + data;
                    if (!cdAdquirente.Equals("")) vigencia += "!" + cdAdquirente;
                }else if (!cdAdquirente.Equals("")) vigencia += "!null!" + cdAdquirente;
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.VIGENCIA, vigencia);
                
                // PARA O EXTRATO, SÓ CONSIDERA OS TIPO CREDIT
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DSTIPO, OFXSharp.OFXTransactionType.CREDIT.ToString());



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


                // OBTÉM AS QUERIES
                IQueryable<tbRecebimentoAjuste> queryAjustes = GatewayTbRecebimentoAjuste.getQuery(0, (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, 0, 0, queryStringAjustes);
                IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);
                IQueryable<tbExtrato> queryExtrato = GatewayTbExtrato.getQuery(0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, 0, 0, queryStringExtrato);

                // VALOR TOTAL ASSOCIADO A CADA LADO DA CONCILIAÇÃO
                decimal totalRecebimento = new decimal(0.0);
                decimal totalExtrato = new decimal(0.0);


                // Só busca por conciliações já concretizadas se não tiver sido requisitado um filtro do tipo PRE-CONCILIADO ou NÃO CONCILIADO
                if (!filtroTipoPreConciliado && !filtroTipoNaoConciliado) {
                    #region OBTÉM AS INFORMAÇÕES DE DADOS JÁ CONCILIADOS PREVIAMENTE
                    // EXTRATOS JÁ CONCILIADOS, COM SEUS RESPECTIVOS RECEBIMENTOS CONCILIADOS
                    List<dynamic> extratosBancariosConciliados = queryExtrato
                                                .Where(e => e.RecebimentoParcelas.Count > 0)
                                                .Select(e => new {
                                                    idExtrato = e.idExtrato,
                                                    dtExtrato = e.dtExtrato,
                                                    nrDocumento = e.nrDocumento,
                                                    vlMovimento = e.vlMovimento ?? new decimal(0.0),
                                                    dsDocumento = e.dsDocumento,
                                                    bandeira = GatewayTbExtrato._db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.tbBandeira.dsBandeira ?? "").FirstOrDefault() ?? "",
                                                    tipo = GatewayTbExtrato._db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.dsTipoCartao ?? "").FirstOrDefault() ?? "",
                                                    Conta = new ConciliacaoBancaria.ConciliacaoConta
                                                    {
                                                        CdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                                                        NrAgencia = e.tbContaCorrente.nrAgencia,
                                                        NrConta = e.tbContaCorrente.nrConta,
                                                        CdBanco = e.tbContaCorrente.cdBanco
                                                    },
                                                    /*Recebimentos = e.RecebimentoParcelas
                                                                        .OrderBy(r => r.dtaRecebimento)
                                                                        .ThenBy(r => r.Recebimento.dtaVenda)
                                                                        .GroupBy(r => r.tbExtrato) // agrupa pelo mesmo extrato para se tornar um único registro
                                                                        .Select(r => new ConciliacaoBancaria
                                                                        {
                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                            Grupo = r.Select(x =>
                                                                                new ConciliacaoBancaria.ConciliacaoGrupo
                                                                                {
                                                                                    Id = x.idRecebimento,
                                                                                    Documento = x.Recebimento.nsu,
                                                                                    Valor = x.valorParcelaLiquida ?? new decimal(0.0),
                                                                                    Bandeira = x.Recebimento.BandeiraPos.desBandeira.ToUpper()
                                                                                })
                                                                                .ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                            ValorTotal = r.Select(x => x.valorParcelaLiquida ?? new decimal(0.0)).Sum(),
                                                                            Data = r.Select(x => x.dtaRecebimento).FirstOrDefault(),
                                                                            Adquirente = r.Select(x => x.Recebimento.BandeiraPos.Operadora.nmOperadora.ToUpper()).FirstOrDefault(),
                                                                            Bandeira = r.Select(x => x.Recebimento.BandeiraPos.desBandeira.ToUpper()).FirstOrDefault(),
                                                                        }).FirstOrDefault<ConciliacaoBancaria>()*/ // => TIMEOUT!
                                                }).ToList<dynamic>();

                    // RECEBIMENTOS PARCELAS JÁ CONCILIADOS
                    //List<ConciliacaoBancaria> recebimentosParcelaConciliados = new List<ConciliacaoBancaria>();

                    // Total dos elementos já conciliados
                    if (extratosBancariosConciliados.Count > 0)
                    {
                        // Adiciona como conciliados
                        foreach (var extrato in extratosBancariosConciliados)
                        {
                            // Recebimento
                            Int32 idExtrato = Convert.ToInt32(extrato.idExtrato);
                            ConciliacaoBancaria recebimento = _db.RecebimentoParcelas
                                                                    .Where(e => e.idExtrato == idExtrato)
                                                                    .OrderBy(r => r.dtaRecebimentoEfetivo ?? r.dtaRecebimento)
                                                                    .ThenBy(r => r.Recebimento.dtaVenda)
                                                                    .GroupBy(r => r.tbExtrato) // agrupa pelo mesmo extrato para se tornar um único registro
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Grupo = r.Select(x =>
                                                                            new ConciliacaoBancaria.ConciliacaoGrupo
                                                                            {
                                                                                Id = x.idRecebimento,
                                                                                NumParcela = x.numParcela,
                                                                                Documento = x.Recebimento.nsu,
                                                                                Valor = x.valorParcelaLiquida ?? new decimal(0.0),
                                                                                Bandeira = x.Recebimento.tbBandeira.dsBandeira.ToUpper(),
                                                                                TipoCartao = x.Recebimento.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                                                                                DataVenda = x.Recebimento.dtaVenda,
                                                                                DataPrevista = x.dtaRecebimento,
                                                                                Filial = x.Recebimento.empresa.ds_fantasia + (x.Recebimento.empresa.filial != null ? " " + x.Recebimento.empresa.filial : "")
                                                                            })
                                                                            .OrderBy(x => x.Bandeira)
                                                                            .ThenByDescending(x => x.DataVenda)
                                                                            .ThenBy(x => x.DataPrevista)
                                                                            .ThenBy(x => x.Valor)
                                                                            .ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                        ValorTotal = r.Select(x => x.valorParcelaLiquida ?? new decimal(0.0)).Sum(),
                                                                        Data = r.Select(x => x.dtaRecebimentoEfetivo ?? x.dtaRecebimento).FirstOrDefault(),
                                                                        Adquirente = r.Select(x => x.Recebimento.tbBandeira.tbAdquirente.nmAdquirente.ToUpper()).FirstOrDefault(),
                                                                        Bandeira = r.GroupBy(x => x.Recebimento.cdBandeira).Count() == 1 ? r.Select(x => x.Recebimento.tbBandeira.dsBandeira.ToUpper()).FirstOrDefault() : "",
                                                                        TipoCartao = r.GroupBy(x => x.Recebimento.tbBandeira.dsTipo).Count() == 1 ? r.Select(x => x.Recebimento.tbBandeira.dsTipo.ToUpper().TrimEnd()).FirstOrDefault() : "",
                                                                        Filial = r.GroupBy(x => x.Recebimento.empresa).Count() == 1 ? r.Select(x => x.Recebimento.empresa.ds_fantasia + (x.Recebimento.empresa.filial != null ? " " + x.Recebimento.empresa.filial : "")).FirstOrDefault() : ""
                                                                    }).FirstOrDefault<ConciliacaoBancaria>();

                            ConciliacaoBancaria ajustes = _db.tbRecebimentoAjustes
                                                                    .Where(e => e.idExtrato == idExtrato)
                                                                    .OrderBy(r => r.dtAjuste)
                                                                    .GroupBy(r => r.tbExtrato) // agrupa pelo mesmo extrato para se tornar um único registro
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Grupo = r.Select(x =>
                                                                            new ConciliacaoBancaria.ConciliacaoGrupo
                                                                            {
                                                                                Id = x.idRecebimentoAjuste,
                                                                                NumParcela = -1,
                                                                                Documento = x.dsMotivo,
                                                                                Valor = x.vlAjuste,
                                                                                Bandeira = x.tbBandeira.dsBandeira.ToUpper(),
                                                                                TipoCartao = x.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                                                                                DataVenda = x.dtAjuste,
                                                                                DataPrevista = x.dtAjuste,
                                                                                Filial = x.empresa.ds_fantasia + (x.empresa.filial != null ? " " + x.empresa.filial : "")
                                                                            })
                                                                            .OrderBy(x => x.Bandeira)
                                                                            .ThenByDescending(x => x.DataVenda)
                                                                            .ThenBy(x => x.DataPrevista)
                                                                            .ThenBy(x => x.Valor)
                                                                            .ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                        ValorTotal = r.Select(x => x.vlAjuste).Sum(),
                                                                        Data = r.Select(x => x.dtAjuste).FirstOrDefault(),
                                                                        Adquirente = r.Select(x => x.tbBandeira.tbAdquirente.nmAdquirente.ToUpper()).FirstOrDefault(),
                                                                        Bandeira = r.GroupBy(x => x.tbBandeira.cdBandeira).Count() == 1 ? r.Select(x => x.tbBandeira.dsBandeira.ToUpper()).FirstOrDefault() : "",
                                                                        TipoCartao = r.GroupBy(x => x.tbBandeira.dsTipo).Count() == 1 ? r.Select(x => x.tbBandeira.dsTipo.ToUpper().TrimEnd()).FirstOrDefault() : "",
                                                                        Filial = r.GroupBy(x => x.empresa).Count() == 1 ? r.Select(x => x.empresa.ds_fantasia + (x.empresa.filial != null ? " " + x.empresa.filial : "")).FirstOrDefault() : ""
                                                                    }).FirstOrDefault<ConciliacaoBancaria>();
                            /* ConciliacaoBancaria recebimento = extrato.Recebimentos;
                            recebimento = new ConciliacaoBancaria
                            {
                                Tipo = TIPO_RECEBIMENTO, // recebimento
                                Grupo = recebimento.Grupo.OrderBy(x => x.Bandeira).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(), // ordena
                                ValorTotal = recebimento.ValorTotal,
                                Data = recebimento.Data,
                                Adquirente = recebimento.Adquirente,
                                Bandeira = recebimento.Bandeira,
                            };*/

                            if (recebimento == null && ajustes == null) continue; // falha!

                            // Recebimento + Ajuste
                            if (recebimento == null)
                                recebimento = ajustes;
                            else if(ajustes != null)
                            {
                                foreach (ConciliacaoBancaria.ConciliacaoGrupo grupo in ajustes.Grupo)
                                    recebimento.Grupo.Add(grupo);
                                recebimento.Grupo = recebimento.Grupo.OrderBy(x => x.Bandeira).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>();
                                recebimento.ValorTotal += ajustes.ValorTotal;
                                if (!recebimento.Bandeira.Equals(ajustes.Bandeira))
                                    recebimento.Bandeira = "";
                                if (!recebimento.TipoCartao.Equals(ajustes.TipoCartao))
                                    recebimento.TipoCartao = "";
                            }

                            // Movimentação
                            ConciliacaoBancaria movimentacao = new ConciliacaoBancaria
                            {
                                Tipo = TIPO_EXTRATO, // extrato
                                Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                    new ConciliacaoBancaria.ConciliacaoGrupo {
                                        Id = extrato.idExtrato,
                                        Documento = extrato.nrDocumento,
                                        Valor = extrato.vlMovimento,
                                        //Bandeira = extrato.bandeira,
                                        //TipoCartao = extrato.tipo
                                    }
                                },
                                ValorTotal = extrato.vlMovimento,
                                Data = extrato.dtExtrato,
                                Adquirente = recebimento.Adquirente,
                                Memo = extrato.dsDocumento,
                                Conta = extrato.Conta,
                                Bandeira = extrato.bandeira,
                                TipoCartao = extrato.tipo
                            };
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
                    List<ConciliacaoBancaria> recebimentosParcela = queryRecebimentoParcela
                                                        .Where(r => r.idExtrato == null)
                                                        .Select(r => new ConciliacaoBancaria
                                                        {
                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                            Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                                                new ConciliacaoBancaria.ConciliacaoGrupo {
                                                                    Id = r.idRecebimento,
                                                                    NumParcela = r.numParcela,
                                                                    Documento = r.Recebimento.nsu,
                                                                    Valor = r.valorParcelaLiquida ?? new decimal(0.0),
                                                                    Bandeira = r.Recebimento.tbBandeira.dsBandeira.ToUpper(),
                                                                    TipoCartao = r.Recebimento.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                                                                    DataVenda = r.Recebimento.dtaVenda,
                                                                    DataPrevista = r.dtaRecebimento,
                                                                    Filial = r.Recebimento.empresa.ds_fantasia + (r.Recebimento.empresa.filial != null ? " " + r.Recebimento.empresa.filial : "")
                                                                }
                                                            },
                                                            ValorTotal = r.valorParcelaLiquida ?? new decimal(0.0),
                                                            Data = r.dtaRecebimentoEfetivo ?? r.dtaRecebimento,
                                                            DataVenda = r.Recebimento.dtaVenda,
                                                            Adquirente = r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente.ToUpper(),
                                                            Bandeira = r.Recebimento.tbBandeira.dsBandeira.ToUpper(),
                                                            TipoCartao = r.Recebimento.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                                                            Filial = r.Recebimento.empresa.ds_fantasia + (r.Recebimento.empresa.filial != null ? " " + r.Recebimento.empresa.filial : "")
                                                        }).ToList<ConciliacaoBancaria>();

                    Util.GatewayProxy proxy = new GatewayProxy();

                    foreach (var itemrecebimentosParcela in recebimentosParcela)
                    {
                        foreach (var itemGrupo in itemrecebimentosParcela.Grupo)
                        {
                            var numTitulo = Util.GatewayProxy.Get("http://localhost:50939/pgsql/tabtituloreceber/eNR59cwLDBMqiSvY6qvasoFFXTZAStWKfVq88zMlZVQVShkOHmEMurHbiYZYyKgAlDEexUxtiiQU7Cy54WMrQlNc0aBAFWtKciZhnCcVdznEIqPoObptiGLh57oA/0?177=" + itemGrupo.Documento + "&106=" + itemGrupo.DataVenda.Value.ToString("yyyyMMdd") + "|" + itemGrupo.DataVenda.Value.ToString("yyyyMMdd"));
                            itemGrupo.NumTituloErp = numTitulo.ToString();
                        }
                    }

                    List<ConciliacaoBancaria> ajustes = queryAjustes
                                                        .Where(r => r.idExtrato == null)
                                                        .Select(r => new ConciliacaoBancaria
                                                        {
                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                            Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                                                new ConciliacaoBancaria.ConciliacaoGrupo {
                                                                    Id = r.idRecebimentoAjuste,
                                                                    NumParcela = -1,
                                                                    Documento = r.dsMotivo,
                                                                    Valor = r.vlAjuste,
                                                                    Bandeira = r.tbBandeira.dsBandeira.ToUpper(),
                                                                    TipoCartao = r.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                                                                    DataVenda = r.dtAjuste,
                                                                    DataPrevista = r.dtAjuste,
                                                                    Filial = r.empresa.ds_fantasia + (r.empresa.filial != null ? " " + r.empresa.filial : ""),
                                                                    NumTituloErp = ""
                                                                }
                                                            },
                                                            ValorTotal = r.vlAjuste,
                                                            Data = r.dtAjuste,
                                                            DataVenda = r.dtAjuste,
                                                            Adquirente = r.tbBandeira.tbAdquirente.nmAdquirente.ToUpper(),
                                                            Bandeira = r.tbBandeira.dsBandeira.ToUpper(),
                                                            TipoCartao = r.tbBandeira.dsTipo.ToUpper().TrimEnd(),
                                                            Filial = r.empresa.ds_fantasia + (r.empresa.filial != null ? " " + r.empresa.filial : "")
                                                        }).ToList<ConciliacaoBancaria>();

                    foreach (ConciliacaoBancaria aj in ajustes)
                        recebimentosParcela.Add(aj);
                    #endregion

                    #region OBTÉM OS EXTRATOS NÃO CONCILIADOS QUE TEM ADQUIRENTE ASSOCIADA
                    List <ConciliacaoBancaria> extratoBancario = queryExtrato
                                                            // Só considera os extratos que não estão conciliados
                                                            .Where(e => e.RecebimentoParcelas.Count == 0)
                                                            .Select(e => new ConciliacaoBancaria
                                                            {
                                                                Tipo = TIPO_EXTRATO, // extrato
                                                                Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                                                                new ConciliacaoBancaria.ConciliacaoGrupo {
                                                                                    Id = e.idExtrato,
                                                                                    Documento = e.nrDocumento,
                                                                                    Valor = e.vlMovimento ?? new decimal(0.0),
                                                                                }
                                                                            },
                                                                ValorTotal = e.vlMovimento ?? new decimal(0.0),
                                                                Data = e.dtExtrato,
                                                                Adquirente = GatewayTbExtrato._db.tbBancoParametro.Where(p => p.dsMemo.Equals(e.dsDocumento))
                                                                                                 .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                                                 .Select(p => p.tbAdquirente.nmAdquirente.ToUpper())
                                                                                                 .FirstOrDefault() ?? "",
                                                                Memo = e.dsDocumento,
                                                                Conta = new ConciliacaoBancaria.ConciliacaoConta
                                                                {
                                                                    CdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                                                                    NrAgencia = e.tbContaCorrente.nrAgencia,
                                                                    NrConta = e.tbContaCorrente.nrConta,
                                                                    CdBanco = e.tbContaCorrente.cdBanco
                                                                },
                                                                Bandeira = GatewayTbExtrato._db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.tbBandeira.dsBandeira ?? "").FirstOrDefault() ?? "",
                                                                TipoCartao = GatewayTbExtrato._db.tbBancoParametro.Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco) && p.dsMemo.Equals(e.dsDocumento)).Select(p => p.dsTipoCartao.ToUpper().TrimEnd() ?? "").FirstOrDefault() ?? "",

                                                            }).ToList<ConciliacaoBancaria>();
                    #endregion


                    // TEM ELEMENTOS PARA CONCILIAR?
                    if (recebimentosParcela.Count == 0 || extratoBancario.Count == 0)
                    {
                        if (!filtroTipoPreConciliado)
                        {
                            #region NÃO O QUE CONCILIAR => ADICIONA OS ELEMENTOS COMO NÃO CONCILIADOS
                            if (recebimentosParcela.Count > 0)
                            {
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria,
                                                            // Envia recebimentos agrupados
                                                            recebimentosParcela
                                                                    .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira })
                                                                    .OrderBy(r => r.Key.Data)
                                                                    .ThenBy(r => r.Key.Filial)
                                                                    .ThenBy(r => r.Key.Adquirente)
                                                                    .ThenBy(r => r.Key.Bandeira)
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Bandeira).ThenByDescending(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                        Data = r.Key.Data,
                                                                        ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                        Adquirente = r.Key.Adquirente,
                                                                        Bandeira = r.Key.Bandeira,
                                                                        TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                        Filial = r.Key.Filial
                                                                    }).ToList<ConciliacaoBancaria>());
                            }
                            else if (extratoBancario.Count > 0)
                            {
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        #region PASSO 1) CONCILIA DIRETAMENTE RECEBIMENTOPARCELA COM TBEXTRATO

                        List<ConciliacaoBancaria> listaCandidatos = new List<ConciliacaoBancaria>();
                        List<ConciliacaoBancaria> listaNaoConciliado = new List<ConciliacaoBancaria>();

                        // Concatena as duas listas, ordenando por data
                        listaCandidatos = recebimentosParcela.Concat<ConciliacaoBancaria>(extratoBancario)
                                              .OrderBy(c => c.Data.Year)
                                              .ThenBy(c => c.Data.Month)
                                              .ThenBy(c => c.Data.Day)
                                              .ThenBy(c => c.Adquirente)
                                              .ThenBy(c => c.ValorTotal)
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
                                                                    .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira })
                                                                    .OrderBy(r => r.Key.Data)
                                                                    .ThenBy(r => r.Key.Filial)
                                                                    .ThenBy(r => r.Key.Adquirente)
                                                                    .ThenBy(r => r.Key.Bandeira)
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Grupo = r.Select(x => x.Grupo[0]).OrderByDescending(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                        Data = r.Key.Data,
                                                                        ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                        Adquirente = r.Key.Adquirente,
                                                                        Bandeira = r.Key.Bandeira,
                                                                        TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
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
                                List<ConciliacaoBancaria> recebimentosParcelaAgrupados = recebimentosParcela
                                                                    .GroupBy(r => new { r.Data, r.DataVenda, r.Filial, r.Adquirente, r.Bandeira })
                                                                    .OrderBy(r => r.Key.Data)
                                                                    .ThenBy(r => r.Key.DataVenda)
                                                                    .ThenBy(r => r.Key.Filial)
                                                                    .ThenBy(r => r.Key.Adquirente)
                                                                    .ThenBy(r => r.Key.Bandeira)
                                                                    .Select(r => new ConciliacaoBancaria
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                        Data = r.Key.Data,
                                                                        DataVenda = r.Key.DataVenda,
                                                                        ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                        Adquirente = r.Key.Adquirente,
                                                                        Bandeira = r.Key.Bandeira,
                                                                        TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                        Filial = r.Key.Filial
                                                                    }).ToList<ConciliacaoBancaria>();

                                // Concatena as duas listas, ordenando por data
                                listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                  .OrderBy(c => c.Data.Year)
                                                  .ThenBy(c => c.Data.Month)
                                                  .ThenBy(c => c.Data.Day)
                                                  .ThenBy(c => c.Adquirente)
                                                  .ThenBy(c => c.ValorTotal)
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
                                                                                recebimentosParcela
                                                                                .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira })
                                                                                .OrderBy(r => r.Key.Data)
                                                                                .ThenBy(r => r.Key.Filial)
                                                                                .ThenBy(r => r.Key.Adquirente)
                                                                                .ThenBy(r => r.Key.Bandeira)
                                                                                .Select(r => new ConciliacaoBancaria
                                                                                {
                                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                    Grupo = r.Select(x => x.Grupo[0]).OrderByDescending(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                    Data = r.Key.Data,
                                                                                    ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                    Adquirente = r.Key.Adquirente,
                                                                                    Bandeira = r.Key.Bandeira,
                                                                                    TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
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
                                                                    Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                    Data = r.Key.Data,
                                                                    ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                    Adquirente = r.Key.Adquirente,
                                                                    Bandeira = r.Key.Bandeira,
                                                                    TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
                                                                    Filial = r.Key.Filial
                                                                }).ToList<ConciliacaoBancaria>();

                                        // Concatena as duas listas, ordenando por data
                                        listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                          .OrderBy(c => c.Data.Year)
                                                          .ThenBy(c => c.Data.Month)
                                                          .ThenBy(c => c.Data.Day)
                                                          .ThenBy(c => c.Adquirente)
                                                          .ThenBy(c => c.ValorTotal)
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
                                                #region PASSO 4) TENTA ENCONTRAR SUBGRUPOS DE CADA AGRUPAMENTO ENVOLVENDO DATA, ADQUIRENTE E BANDEIRA NO RECEBIMENTO PARCELA
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
                                                    #region PASSO 5) AGRUPA SEM CONSIDERAR A BANDEIRA

                                                    recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Grupo.Any(g => g.Id == e.Grupo[0].Id && g.NumParcela == e.Grupo[0].NumParcela)))
                                                                                .OrderBy(e => e.Data)
                                                                                .ThenBy(r => r.Filial)
                                                                                .ThenBy(e => e.Adquirente)
                                                                                .ThenBy(e => e.Bandeira)
                                                                                .ToList<ConciliacaoBancaria>();

                                                    recebimentosParcelaAgrupados = recebimentosParcela
                                                                                        .GroupBy(r => new { r.Data, r.Filial, r.Adquirente })
                                                                                        .OrderBy(r => r.Key.Data)
                                                                                        .ThenBy(r => r.Key.Filial)
                                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                                        .Select(r => new ConciliacaoBancaria
                                                                                        {
                                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Bandeira).ThenBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                            Data = r.Key.Data,
                                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                            Adquirente = r.Key.Adquirente,
                                                                                            Filial = r.Key.Filial,
                                                                                            Bandeira = r.GroupBy(x => x.Bandeira).Count() == 1 ? r.Select(x => x.Bandeira).FirstOrDefault() : "",
                                                                                            TipoCartao = r.GroupBy(x => x.TipoCartao).Count() == 1 ? r.Select(x => x.TipoCartao).FirstOrDefault() : "",
                                                                                        }).ToList<ConciliacaoBancaria>();

                                                    // Concatena as duas listas, ordenando por data
                                                    listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                                      .OrderBy(c => c.Data.Year)
                                                                      .ThenBy(c => c.Data.Month)
                                                                      .ThenBy(c => c.Data.Day)
                                                                      .ThenBy(c => c.Adquirente)
                                                                      .ThenBy(c => c.ValorTotal)
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
                                                                                        .GroupBy(r => new { r.Data, r.Filial, r.Adquirente, r.Bandeira })
                                                                                        .OrderBy(r => r.Key.Data)
                                                                                        .ThenBy(r => r.Key.Filial)
                                                                                        .ThenBy(r => r.Key.Adquirente)
                                                                                        .ThenBy(r => r.Key.Bandeira)
                                                                                        .Select(r => new ConciliacaoBancaria
                                                                                        {
                                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                                            Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.DataVenda).ThenBy(x => x.DataPrevista).ThenBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                                            Data = r.Key.Data,
                                                                                            ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                                            Adquirente = r.Key.Adquirente,
                                                                                            Bandeira = r.Key.Bandeira,
                                                                                            TipoCartao = r.Select(x => x.TipoCartao).FirstOrDefault(),
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

                                                            #region PASSO 6) TENTA A ÚLTIMA CONCILIAÇÃO, CONSIDERANDO APENAS OS QUE SOBRARAM APÓS TODAS AS ETAPAS ANTERIORES

                                                            // Concatena as duas listas, ordenando por data
                                                            listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                                              .OrderBy(c => c.Data.Year)
                                                                              .ThenBy(c => c.Data.Month)
                                                                              .ThenBy(c => c.Data.Day)
                                                                              .ThenBy(c => c.Adquirente)
                                                                              .ThenBy(c => c.ValorTotal)
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
                                                                                                            .ThenBy(r => r.Bandeira)
                                                                                                            .ToList<ConciliacaoBancaria>();

                                                                extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                                                        .OrderBy(e => e.Data)
                                                                                                        .ThenBy(e => e.Adquirente)
                                                                                                        .ThenBy(e => e.Memo)
                                                                                                        .ToList<ConciliacaoBancaria>();

                                                                #endregion

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
                        }
                    }
                    #endregion
                }

                // Ordena
                CollectionConciliacaoBancaria = CollectionConciliacaoBancaria
                                                                .OrderBy(c => c.Data.Year)
                                                                .ThenBy(c => c.Data.Month)
                                                                .ThenBy(c => c.Data.Day)
                                                                .ThenBy(c => c.Adquirente)
                                                                .ThenBy(c => c.RecebimentosParcela != null ? c.ValorTotalRecebimento : c.ValorTotalExtrato)
                                                                .ThenBy(c => c.Bandeira)
                                                                .ToList<dynamic>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionConciliacaoBancaria.Count;

                // TOTAL
                totalRecebimento = CollectionConciliacaoBancaria.Select(r => r.ValorTotalRecebimento).Cast<decimal>().Sum();
                totalExtrato = CollectionConciliacaoBancaria.Select(r => r.ValorTotalExtrato).Cast<decimal>().Sum();
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valorExtrato", totalExtrato);
                retorno.Totais.Add("valorRecebimento", totalRecebimento);

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
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar adquirente" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Aponta RecebimentoParcela para Extrato
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, List<ConciliaRecebimentoParcela> param)
        {
            try
            {
                foreach (ConciliaRecebimentoParcela grupoExtrato in param)
                {
                    if (grupoExtrato.recebimentosParcela != null)
                    {
                        // Avalia o extrato
                        tbExtrato extrato = null;
                        if (grupoExtrato.idExtrato > 0)
                        {
                            extrato = _db.tbExtratos.Where(e => e.idExtrato == grupoExtrato.idExtrato).FirstOrDefault();
                            if (extrato == null) continue; // extrato inválido!
                        }


                        foreach (ConciliaRecebimentoParcela.RecebParcela recebimentoParcela in grupoExtrato.recebimentosParcela)
                        {
                            if(recebimentoParcela.numParcela == -1)
                            {
                                // AJUSTE
                                tbRecebimentoAjuste value = _db.tbRecebimentoAjustes
                                                                        .Where(e => e.idRecebimentoAjuste == recebimentoParcela.idRecebimento)
                                                                        .FirstOrDefault();
                                if (value != null)
                                {
                                    if (grupoExtrato.idExtrato == -1) value.idExtrato = null;
                                    else
                                    {
                                        value.idExtrato = extrato.idExtrato;
                                        //value.dtAjuste = extrato.dtExtrato; // atualiza data efetiva do ajuste
                                    }
                                    _db.SaveChanges();
                                }
                            }
                            else
                            {
                                // RECEBIMENTO PARCELA
                                RecebimentoParcela value = _db.RecebimentoParcelas
                                                                        .Where(e => e.idRecebimento == recebimentoParcela.idRecebimento)
                                                                        .Where(e => e.numParcela == recebimentoParcela.numParcela)
                                                                        .FirstOrDefault();
                                if (value != null)
                                {
                                    if (grupoExtrato.idExtrato == -1) value.idExtrato = null;
                                    else
                                    {
                                        value.idExtrato = extrato.idExtrato;
                                        value.dtaRecebimentoEfetivo = extrato.dtExtrato; // atualiza data efetiva de recebimento
                                    }
                                    _db.SaveChanges();
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar recebimento parcela" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }

}
