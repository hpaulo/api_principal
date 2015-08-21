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
            ID_GRUPO = 102,
            NU_CNPJ = 103,

            // RELACIONAMENTOS
            IDOPERADORA = 200,
        };

        public enum TIPO_CONCILIADO
        {
            CONCILIADO = 1,
            NAO_CONCILIADO = 2
        };

        private static string TIPO_EXTRATO = "E";
        private static string TIPO_RECEBIMENTO = "R";
        private static decimal TOLERANCIA = new decimal(0.01); // R$0.01 de tolerância


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
                    RecebimentosParcela = item.Tipo != TIPO_RECEBIMENTO ? null : item.Grupo,
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
                    RecebimentosParcela = recebimento.Grupo,
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
                   Math.Abs(item1.ValorTotal - item2.ValorTotal) > TOLERANCIA)
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

                adicionaElementosConciliadosNaLista(listaConciliacao, recebimento, movimentacao, TIPO_CONCILIADO.NAO_CONCILIADO);
            }
        }



        private static IEnumerable<List<List<ConciliacaoBancaria.ConciliacaoGrupo>>> GetCombinations(List<ConciliacaoBancaria.ConciliacaoGrupo> grupo, 
                                                                                  decimal sum, 
                                                                                  List<ConciliacaoBancaria.ConciliacaoGrupo> values = null)
        {
            if (values == null) values = new List<ConciliacaoBancaria.ConciliacaoGrupo>();
            for (Int32 i = 0; i < grupo.Count; i++)
            {
                decimal left = sum - grupo[i].Valor;
                List<List<ConciliacaoBancaria.ConciliacaoGrupo>> vals = new List<List<ConciliacaoBancaria.ConciliacaoGrupo>>();
                List<ConciliacaoBancaria.ConciliacaoGrupo> temp = new List<ConciliacaoBancaria.ConciliacaoGrupo>();
                values.CopyItemsTo(temp);
                temp.Add(grupo[i]);
                vals.Add(temp);

                if(Math.Abs(left) <= TOLERANCIA) yield return vals;
                else
                {
                    List<ConciliacaoBancaria.ConciliacaoGrupo> possible = grupo.Take(i).Where(n => n.Valor <= sum).ToList<ConciliacaoBancaria.ConciliacaoGrupo>();
                    if (possible.Count > 0)
                    {
                        foreach(List<List<ConciliacaoBancaria.ConciliacaoGrupo>> g in GetCombinations(possible, left, temp))
                            yield return g;
                    }
                }
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

                string vigencia = String.Empty;
                string data = queryString["" + (int)CAMPOS.DATA];
                queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, data);
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, data);
                // GRUPO EMPRESA
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    //queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    vigencia = CnpjEmpresa + "!" + data;
                }
                // ADQUIRENTE
                if (queryString.TryGetValue("" + (int)CAMPOS.IDOPERADORA, out outValue))
                {
                    int idOperadora = Convert.ToInt32(queryString["" + (int)CAMPOS.IDOPERADORA]);
                    Operadora operadora = _db.Operadoras.Where(o => o.id == idOperadora).FirstOrDefault();
                    if (operadora != null)
                    {
                        tbAdquirente adquirente = _db.tbAdquirentes.Where(a => a.nmAdquirente.Equals(operadora.nmOperadora)).FirstOrDefault();
                        if (adquirente != null)
                        {
                            queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.IDOPERADORA, idOperadora.ToString());
                            queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE, adquirente.cdAdquirente.ToString());
                            vigencia += "!" + adquirente.cdAdquirente.ToString();
                        }
                    }
                }
                // Vigência
                if (!vigencia.Equals("")) queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.VIGENCIA, vigencia);
                // PARA O EXTRATO, SÓ CONSIDERA OS TIPO CRÉDITO
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DSTIPO, OFXSharp.OFXTransactionType.CREDIT.ToString());



                // FILTRO DE TIPO ?
                bool filtroTipoConciliado = false;
                bool filtroTipoNaoConciliado = false;
                if (queryString.TryGetValue("" + (int)CAMPOS.TIPO, out outValue))
                {
                    int t = Convert.ToInt32(queryString["" + (int)CAMPOS.TIPO]);
                    TIPO_CONCILIADO tipo = (TIPO_CONCILIADO)t;
                    if (tipo.Equals(TIPO_CONCILIADO.CONCILIADO)) filtroTipoConciliado = true;
                    else if (tipo.Equals(TIPO_CONCILIADO.NAO_CONCILIADO)) filtroTipoNaoConciliado = true;
                }


                // OBTÉM AS QUERIES
                var queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);
                var queryExtrato = GatewayTbExtrato.getQuery(0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, 0, 0, queryStringExtrato);

                // VALOR TOTAL ASSOCIADO A CADA LADO DA CONCILIAÇÃO
                decimal totalRecebimento = new decimal(0.0);
                decimal totalExtrato = new decimal(0.0);


                // Só busca por conciliações já concretizadas se não tiver sido requisitado um filtro do tipo NÃO CONCILIADO
                if (!filtroTipoNaoConciliado) {
                    #region OBTÉM AS INFORMAÇÕES DE DADOS JÁ CONCILIADOS PREVIAMENTE
                    // EXTRATOS JÁ CONCILIADOS, COM SEUS RESPECTIVOS RECEBIMENTOS CONCILIADOS
                    List<dynamic> extratosBancariosConciliados = queryExtrato.Where(e => e.RecebimentoParcelas.Count > 0)
                                                .Select(e => new {
                                                    idExtrato = e.idExtrato,
                                                    dtExtrato = e.dtExtrato,
                                                    nrDocumento = e.nrDocumento,
                                                    vlMovimento = e.vlMovimento ?? new decimal(0.0),
                                                    dsDocumento = e.dsDocumento,
                                                    Conta = new ConciliacaoBancaria.ConciliacaoConta
                                                    {
                                                        CdContaCorrente = e.tbContaCorrente.cdContaCorrente,
                                                        NrAgencia = e.tbContaCorrente.nrAgencia,
                                                        NrConta = e.tbContaCorrente.nrConta,
                                                        CdBanco = e.tbContaCorrente.cdBanco
                                                    },
                                                    Recebimentos = e.RecebimentoParcelas
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
                                                                                    Valor = x.valorParcelaLiquida ?? new decimal(0.0)
                                                                                })/*.OrderBy(x => x.Valor) // não funciona aqui!*/
                                                                                .ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                            ValorTotal = r.Select(x => x.valorParcelaLiquida ?? new decimal(0.0)).Sum(),
                                                                            Data = r.Select(x => x.dtaRecebimento).FirstOrDefault(),
                                                                            Adquirente = r.Select(x => x.Recebimento.BandeiraPos.Operadora.nmOperadora.ToUpper()).FirstOrDefault(),
                                                                            Bandeira = r.Select(x => x.Recebimento.BandeiraPos.desBandeira.ToUpper()).FirstOrDefault(),
                                                                        }).FirstOrDefault<ConciliacaoBancaria>()
                                                }).ToList<dynamic>();

                    // RECEBIMENTOS PARCELAS JÁ CONCILIADOS
                    List<ConciliacaoBancaria> recebimentosParcelaConciliados = new List<ConciliacaoBancaria>();

                    // Total dos elementos já conciliados
                    if (extratosBancariosConciliados.Count > 0)
                    {
                        // Total do extrato
                        totalExtrato += Convert.ToDecimal(extratosBancariosConciliados.Select(e => e.vlMovimento).Cast<decimal>().Sum());

                        // Adiciona como conciliados
                        foreach (var extrato in extratosBancariosConciliados)
                        {
                            // Movimentação
                            ConciliacaoBancaria movimentacao = new ConciliacaoBancaria
                            {
                                Tipo = TIPO_EXTRATO, // extrato
                                Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                    new ConciliacaoBancaria.ConciliacaoGrupo {
                                        Id = extrato.idExtrato,
                                        Documento = extrato.nrDocumento,
                                        Valor = extrato.vlMovimento,
                                    }
                                },
                                ValorTotal = extrato.vlMovimento,
                                Data = extrato.dtExtrato,
                                Adquirente = extrato.Recebimentos.Adquirente,
                                Memo = extrato.dsDocumento,
                                Conta = extrato.Conta,
                            };
                            // Recebimento
                            ConciliacaoBancaria recebimento = extrato.Recebimentos;
                            recebimento = new ConciliacaoBancaria
                            {
                                Tipo = TIPO_RECEBIMENTO, // recebimento
                                Grupo = recebimento.Grupo.OrderBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(), // ordena
                                ValorTotal = recebimento.ValorTotal,
                                Data = recebimento.Data,
                                Adquirente = recebimento.Adquirente,
                                Bandeira = recebimento.Bandeira,
                            };
                            // Total dos recebimentos
                            totalRecebimento += recebimento.ValorTotal;
                            // Adiciona
                            adicionaElementosConciliadosNaLista(CollectionConciliacaoBancaria, recebimento, movimentacao, TIPO_CONCILIADO.CONCILIADO);
                        }
                    }
                    #endregion
                }


                // Só busca por possíveis conciliações se não tiver sido requisitado um filtro do tipo CONCILIADO
                if (!filtroTipoConciliado)
                {
                    #region OBTÉM AS INFORMAÇÕES DE DADOS NÃO-CONCILIADOS 

                    #region OBTÉM SOMENTE OS RECEBIMENTOS PARCELAS NÃO-CONCILIADOS
                    List<ConciliacaoBancaria> recebimentosParcela = queryRecebimentoParcela
                                                        .Where(r => r.idExtrato == null)
                                                        .Select(r => new ConciliacaoBancaria
                                                        {
                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                            Grupo = new List<ConciliacaoBancaria.ConciliacaoGrupo> {
                                                                new ConciliacaoBancaria.ConciliacaoGrupo {
                                                                    Id = r.idRecebimento,
                                                                    Documento = r.Recebimento.nsu,
                                                                    Valor = r.valorParcelaLiquida ?? new decimal(0.0)
                                                                }
                                                            },
                                                            ValorTotal = r.valorParcelaLiquida ?? new decimal(0.0),
                                                            Data = r.dtaRecebimento,
                                                            DataVenda = r.Recebimento.dtaVenda,
                                                            Adquirente = r.Recebimento.BandeiraPos.Operadora.nmOperadora.ToUpper(),
                                                            Bandeira = r.Recebimento.BandeiraPos.desBandeira.ToUpper(),
                                                        }).ToList<ConciliacaoBancaria>();
                    #endregion

                    if (recebimentosParcela.Count > 0) totalRecebimento += recebimentosParcela.Sum(r => r.ValorTotal);

                    #region OBTÉM OS EXTRATOS NÃO CONCILIADOS QUE TEM ADQUIRENTE ASSOCIADA
                    List<ConciliacaoBancaria> extratoBancario = queryExtrato
                                                            // Só considera os extratos que não estão conciliados
                                                            .Where(e => e.RecebimentoParcelas.Count == 0)
                                                            // Para garantir, não pode ser os que estão na lista já preenchida acima
                                                            //.Where(e => extratosBancariosConciliados.Count == 0 || (extratosBancariosConciliados.Count > 0 && !extratosBancariosConciliados.Any(b => b.Grupo.Any(g => g.Id == e.idExtrato))))
                                                            // Só considera os extratos que tem adquirente associada
                                                            .Where(e => GatewayTbExtrato._db.tbBancoParametro
                                                                                                .Where(p => p.cdAdquirente != null)
                                                                                                .Where(p => p.dsMemo.Equals(e.dsDocumento))
                                                                                                .Where(p => p.cdBanco.Equals(e.tbContaCorrente.cdBanco))
                                                                                                .Count() > 0)
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
                                                            }).ToList<ConciliacaoBancaria>();
                    #endregion

                    if (extratoBancario.Count > 0) totalExtrato += extratoBancario.Sum(r => r.ValorTotal);


                    // TEM ELEMENTOS PARA CONCILIAR?
                    if (recebimentosParcela.Count == 0 || extratoBancario.Count == 0)
                    {
                        // NÃO HÁ O QUE CONCILIAR!
                        if (recebimentosParcela.Count > 0)
                            #region ADICIONA NA COLEÇÃO DE RETORNO DADOS NÃO-CONCILIADOS DO LADO DO ICARD (AGRUPADOS)
                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria,
                                                        // Envia recebimentos agrupados
                                                        recebimentosParcela
                                                                .GroupBy(r => new { r.Data, r.Adquirente, r.Bandeira })
                                                                .OrderBy(r => r.Key.Data)
                                                                .ThenBy(r => r.Key.Adquirente)
                                                                .ThenBy(r => r.Key.Bandeira)
                                                                .Select(r => new ConciliacaoBancaria
                                                                {
                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                    Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                    Data = r.Key.Data,
                                                                    ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                    Adquirente = r.Key.Adquirente,
                                                                    Bandeira = r.Key.Bandeira,
                                                                }).ToList<ConciliacaoBancaria>());
                        #endregion
                        else if (extratoBancario.Count > 0)
                            #region ADICIONA NA COLEÇÃO DE RETORNO DADOS NÃO-CONCILIADOS DO LADO DO EXTRATO BANCÁRIO
                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                            #endregion
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
                                              .ToList<ConciliacaoBancaria>();

                        // Faz a conciliação
                        Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado);

                        #endregion

                        // Tem elementos não conciliados?
                        if (listaNaoConciliado.Count > 0)
                        {
                            #region REMOVE DAS LISTAS OS ELEMENTOS JÁ CONCILIADOS
                            recebimentosParcela = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                        .OrderBy(e => e.Data)
                                                                        .ThenBy(e => e.Adquirente)
                                                                        .ThenBy(e => e.Bandeira)
                                                                        .ToList<ConciliacaoBancaria>();

                            extratoBancario = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_EXTRATO))
                                                                    .OrderBy(e => e.Data)
                                                                    .ThenBy(e => e.Adquirente)
                                                                    .ThenBy(e => e.Memo)
                                                                    .ToList<ConciliacaoBancaria>();

                            List<ConciliacaoBancaria> recebimentosParcelaAgrupados = recebimentosParcela
                                                                .GroupBy(r => new { r.Data, r.DataVenda, r.Adquirente, r.Bandeira })
                                                                .OrderBy(r => r.Key.Data)
                                                                .ThenBy(r => r.Key.DataVenda)
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
                                                                }).ToList<ConciliacaoBancaria>();
                            #endregion

                            // Tem elementos para conciliar?
                            if (recebimentosParcela.Count == 0 || extratoBancario.Count == 0)
                            {
                                // NÃO HÁ MAIS O QUE CONCILIAR!
                                if (recebimentosParcela.Count > 0)
                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcelaAgrupados);
                                else if (extratoBancario.Count > 0)
                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                            }
                            else
                            {
                                #region PASSO 2) CONCILIA FAZENDO AGRUPAMENTO POR DATA, DATA VENDA, ADQUIRENTE E BANDEIRA NO RECEBIMENTO PARCELA

                                // Concatena as duas listas, ordenando por data
                                listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                  .OrderBy(c => c.Data.Year)
                                                  .ThenBy(c => c.Data.Month)
                                                  .ThenBy(c => c.Data.Day)
                                                  .ThenBy(c => c.Adquirente)
                                                  .ThenBy(c => c.ValorTotal)
                                                  .ToList<ConciliacaoBancaria>();

                                // Faz a conciliação
                                listaNaoConciliado.Clear();
                                Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado);

                                #endregion

                                // Ainda tem elementos não conciliados?
                                if (listaNaoConciliado.Count > 0)
                                {
                                    #region REMOVE DAS LISTAS OS ELEMENTOS JÁ CONCILIADOS
                                    recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                            .OrderBy(r => r.Data)
                                                                            .ThenBy(r => r.DataVenda)
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
                                        // NÃO HÁ MAIS O QUE CONCILIAR!
                                        if (extratoBancario.Count > 0)
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                        else if (recebimentosParcelaAgrupados.Count > 0)
                                        {
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcelaAgrupados);
                                        }
                                    }
                                    else
                                    {
                                        #region PASSO 3) CONCILIA FAZENDO AGRUPAMENTO POR DATA, ADQUIRENTE E BANDEIRA NO RECEBIMENTO PARCELA (SEM AGRUPAR POR DATA DA VENDA)
                                        recebimentosParcela = recebimentosParcela.Where(e => recebimentosParcelaAgrupados.Any(p => p.Grupo.Any(g => g.Id == e.Grupo[0].Id)))
                                                                            .OrderBy(e => e.Data)
                                                                            .ThenBy(e => e.Adquirente)
                                                                            .ThenBy(e => e.Bandeira)
                                                                            .ToList<ConciliacaoBancaria>();
                                        recebimentosParcelaAgrupados = recebimentosParcela
                                                                .GroupBy(r => new { r.Data, r.Adquirente, r.Bandeira })
                                                                .OrderBy(r => r.Key.Data)
                                                                .ThenBy(r => r.Key.Adquirente)
                                                                .ThenBy(r => r.Key.Bandeira)
                                                                .Select(r => new ConciliacaoBancaria
                                                                {
                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                    Grupo = r.Select(x => x.Grupo[0]).OrderBy(x => x.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
                                                                    Data = r.Key.Data,
                                                                    ValorTotal = r.Sum(x => x.Grupo[0].Valor),
                                                                    Adquirente = r.Key.Adquirente,
                                                                    Bandeira = r.Key.Bandeira,
                                                                }).ToList<ConciliacaoBancaria>();

                                        // Concatena as duas listas, ordenando por data
                                        listaCandidatos = recebimentosParcelaAgrupados.Concat<ConciliacaoBancaria>(extratoBancario)
                                                          .OrderBy(c => c.Data.Year)
                                                          .ThenBy(c => c.Data.Month)
                                                          .ThenBy(c => c.Data.Day)
                                                          .ThenBy(c => c.Adquirente)
                                                          .ThenBy(c => c.ValorTotal)
                                                          .ToList<ConciliacaoBancaria>();

                                        // Faz a conciliação
                                        listaNaoConciliado.Clear();
                                        Concilia(CollectionConciliacaoBancaria, listaCandidatos, listaNaoConciliado);

                                        #endregion

                                        // Ainda tem elementos não conciliados?
                                        if (listaNaoConciliado.Count > 0)
                                        {
                                            #region REMOVE DA LISTA OS ELEMENTOS JÁ CONCILIADOS
                                            recebimentosParcelaAgrupados = listaNaoConciliado.Where(r => r.Tipo.Equals(TIPO_RECEBIMENTO))
                                                                                    .OrderBy(r => r.Data)
                                                                                    .ThenBy(r => r.DataVenda)
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
                                                // NÃO HÁ MAIS O QUE CONCILIAR!
                                                if (extratoBancario.Count > 0)
                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                                else if (recebimentosParcelaAgrupados.Count > 0)
                                                {
                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcelaAgrupados);
                                                }
                                            }
                                            else
                                            {
                                                #region PASSO 4) TENTA ENCONTRAR SUBGRUPOS DE CADA AGRUPAMENTO

                                                #region OBTÉM POSSÍVEIS COMBINAÇÕES DE RECEBIMENTOSPARCELAS PARA CADA EXTRATO
                                                List<dynamic> gruposExtrato = new List<dynamic>();
                                                foreach (ConciliacaoBancaria extrato in extratoBancario)
                                                {
                                                    // Obtém todas as combinações para o extrato
                                                    List<dynamic> grupos = new List<dynamic>();
                                                    foreach (ConciliacaoBancaria recebimento in recebimentosParcelaAgrupados)
                                                    {
                                                        // Só avalia os de mesma data
                                                        if (recebimento.Data.Year == extrato.Data.Year &&
                                                           recebimento.Data.Month == extrato.Data.Month &&
                                                           recebimento.Data.Day == extrato.Data.Day)
                                                        {
                                                            foreach (List<List<ConciliacaoBancaria.ConciliacaoGrupo>> g in GetCombinations(recebimento.Grupo, extrato.ValorTotal))
                                                            {
                                                                foreach (List<ConciliacaoBancaria.ConciliacaoGrupo> item in g)
                                                                {
                                                                    grupos.Add(new
                                                                    {
                                                                        bandeira = recebimento.Bandeira,
                                                                        grupo = item.OrderBy(gp => gp.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>()
                                                                    });
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
                                                    Dictionary<Int32, bool> recebimentosConciliados = new Dictionary<Int32, bool>();
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
                                                            bool value = true;
                                                            foreach (ConciliacaoBancaria.ConciliacaoGrupo g in grupo)
                                                            {
                                                                if (recebimentosConciliados.TryGetValue(g.Id, out value)) { 
                                                                    grupovalido = false;
                                                                    break;
                                                                }
                                                            }
                                                            #endregion

                                                            if (grupovalido)
                                                            {
                                                                #region ADICIONA O GRUPO COMO "CONCILIADO" COM A MOVIMENTAÇÃO DO EXTRATO CORRENTE

                                                                #region ADICIONA OS IDS DOS RECEBIMENTOSPARCELA NA LISTA DE JÁ "CONCILIADOS"
                                                                foreach (ConciliacaoBancaria.ConciliacaoGrupo g in grupo)
                                                                    recebimentosConciliados.Add(g.Id, true);
                                                                #endregion

                                                                #region ADICIONA COMO "CONCILIADO"
                                                                ConciliacaoBancaria recebimento = new ConciliacaoBancaria()
                                                                {
                                                                    Tipo = TIPO_RECEBIMENTO,
                                                                    Data = movimentacao.Data,
                                                                    Grupo = grupo,
                                                                    Adquirente = movimentacao.Adquirente,
                                                                    Bandeira = receb.bandeira,
                                                                    ValorTotal = grupo.Sum(g => g.Valor),
                                                                };
                                                                adicionaElementosConciliadosNaLista(CollectionConciliacaoBancaria, recebimento, movimentacao, TIPO_CONCILIADO.NAO_CONCILIADO);
                                                                #endregion

                                                                #region REMOVE A MOVIMENTAÇÃO DE EXTRATO
                                                                extratoBancario.Remove(movimentacao);
                                                                #endregion

                                                                #region REMOVE OS RECEBIMENTOS PARCELAS DO AGRUPAMENTO
                                                                ConciliacaoBancaria rp = recebimentosParcelaAgrupados.Where(r => r.Grupo.Any(g => g.Id == grupo[0].Id)).FirstOrDefault();

                                                                // O novo grupo é composto pelos elementos que estavam lá e não foram envolvidos na conciliação corrente
                                                                List<ConciliacaoBancaria.ConciliacaoGrupo> newGrupo = new List<ConciliacaoBancaria.ConciliacaoGrupo>();
                                                                foreach (ConciliacaoBancaria.ConciliacaoGrupo gp in rp.Grupo)
                                                                {
                                                                    if (!grupo.Any(g => g.Id == gp.Id)) newGrupo.Add(gp);
                                                                }
                                                                recebimentosParcelaAgrupados.Remove(rp);

                                                                // Só "re-adiciona" o grupo modificado caso possua elementos
                                                                if (newGrupo.Count > 0)
                                                                {
                                                                    ConciliacaoBancaria newRp = new ConciliacaoBancaria()
                                                                    {
                                                                        Adquirente = rp.Adquirente,
                                                                        Tipo = rp.Tipo,
                                                                        Bandeira = rp.Bandeira,
                                                                        Conta = rp.Conta,
                                                                        Data = rp.Data,
                                                                        DataVenda = rp.DataVenda,
                                                                        Grupo = newGrupo.OrderBy(g => g.Valor).ToList<ConciliacaoBancaria.ConciliacaoGrupo>(),
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
                                                #endregion

                                                #region PASSO 5) ADICIONA OS ELEMENTOS QUE SOBRARAM COMO NÃO CONCILIADOS
                                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, recebimentosParcelaAgrupados);
                                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoBancaria, extratoBancario);
                                                #endregion
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
                                                                .ThenBy(c => c.ValorTotalExtrato) // mais confiável
                                                                .ThenBy(c => c.Bandeira)
                                                                .ToList<dynamic>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionConciliacaoBancaria.Count;

                // TOTAL
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

    }
}
