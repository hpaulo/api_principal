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
    public class GatewayConciliacaoTitulos
    { static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoTitulos()
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
            CONSIDERA_NSU = 104, // 0 ou 1

            // RELACIONAMENTOS
            CDADQUIRENTE = 200,
        };

        public enum TIPO_CONCILIADO
        {
            CONCILIADO = 1,
            PRE_CONCILIADO = 2,
            NAO_CONCILIADO = 3
        };

        private static string TIPO_TITULO = "T";
        private static string TIPO_RECEBIMENTO = "R";


        /// <summary>
        /// Adiciona elementos da listaNaoConciliado na lista
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos da conciliação</param>
        /// <param name="listaNaoConciliado">Lista que contém elementos não conciliados</param>
        /// <returns></returns>
        private static void adicionaElementosNaoConciliadosNaLista(List<dynamic> listaConciliacao,
                                                                   List<ConciliacaoTitulos> listaNaoConciliado)
        {
            foreach (var item in listaNaoConciliado)
            {
                listaConciliacao.Add(new
                {
                    Conciliado = (int)TIPO_CONCILIADO.NAO_CONCILIADO,
                    Titulo = item.Tipo != TIPO_TITULO ? null : new
                    {
                        Id = item.Id,
                        NumParcela = item.NumParcela,
                        Nsu = item.Nsu,
                        DataVenda = item.DataVenda,
                        Valor = item.Valor,
                        Bandeira = item.Bandeira,
                    },
                    RecebimentoParcela = item.Tipo != TIPO_RECEBIMENTO ? null : new
                    {
                        Id = item.Id,
                        NumParcela = item.NumParcela,
                        Nsu = item.Nsu,
                        CodResumoVendas = item.CodResumoVendas,
                        DataVenda = item.DataVenda,
                        Valor = item.Valor,
                        Bandeira = item.Bandeira,
                    },
                    Adquirente = item.Adquirente,
                    Data = item.Data,
                    Filial = item.Filial,
                    Valor = item.Valor,
                    Bandeira = item.Bandeira,
                });
            }
        }

        /// <summary>
        /// Adiciona elementos na lista ao qual foram encontrados registros que "casaram"
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos da conciliação</param>
        /// <param name="recebimento">ConciliacaoBancaria referente ao lado do ICARD</param>
        /// <param name="movimentacao">ConciliacaoBancaria referente ao lado do título</param>
        /// <param name="tipo">CONCILIADO ou PRE-CONCILIADO</param>
        /// <returns></returns>
        private static void adicionaElementosConciliadosNaLista(List<dynamic> listaConciliacao,
                                                     ConciliacaoTitulos recebimento,
                                                     ConciliacaoTitulos titulo,
                                                     TIPO_CONCILIADO tipo)
        {
            if (recebimento != null && titulo != null)
            {
                // Adiciona
                listaConciliacao.Add(new
                {
                    Conciliado = (int)tipo,
                    Titulo = new
                    {
                        Id = titulo.Id,
                        NumParcela = titulo.NumParcela,
                        Nsu = titulo.Nsu,
                        DataVenda = titulo.DataVenda,
                        Valor = titulo.Valor,
                        Bandeira = titulo.Bandeira,
                    },
                    RecebimentoParcela = new
                    {
                        Id = recebimento.Id,
                        NumParcela = recebimento.NumParcela,
                        Nsu = recebimento.Nsu,
                        CodResumoVendas = recebimento.CodResumoVendas,
                        DataVenda = recebimento.DataVenda,
                        Valor = recebimento.Valor,
                        Bandeira = recebimento.Bandeira,
                    },
                    Adquirente = recebimento.Adquirente,
                    Filial = recebimento.Filial,
                    Data = titulo.Data,
                    Valor = titulo.Valor,
                    Bandeira = recebimento.Bandeira,
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
                                     List<ConciliacaoTitulos> listaCandidatos,
                                     List<ConciliacaoTitulos> listaNaoConciliado,
                                     bool adicionaNaListaPreConciliado, bool consideraNSU)
        {
            for (var k = 0; k < listaCandidatos.Count; k++)
            {
                ConciliacaoTitulos item1 = listaCandidatos[k];
                ConciliacaoTitulos item2 = k < listaCandidatos.Count - 1 ? listaCandidatos[k + 1] : null;

                // Verifica se está conciliado
                if (item2 == null || item1.Tipo.Equals(item2.Tipo) || // 'T' ou 'R'
                   // Data
                   (item1.Data.Year != item2.Data.Year || item1.Data.Month != item2.Data.Month || item1.Data.Day != item2.Data.Day) ||
                   // Adquirente                                                  
                   !item1.Adquirente.Equals(item2.Adquirente) ||
                   // Filial                                                  
                   !item1.Filial.Equals(item2.Filial) ||
                   // Valor
                   item1.Valor != item2.Valor)
                {
                    // item1 não conciliado com ninguém
                    listaNaoConciliado.Add(item1);
                    continue;
                }

                if (consideraNSU)
                {
                    // NSUs com mesmo comprimento
                    string nsu1 = item1.Nsu;
                    string nsu2 = item2.Nsu;
                    while (nsu1.Length < nsu2.Length) nsu1 = "0" + nsu1;
                    while (nsu2.Length < nsu1.Length) nsu2 = "0" + nsu2;

                    if (!nsu1.Equals(nsu2))
                    {
                        // NSU diferente!
                        listaNaoConciliado.Add(item1);
                        continue;
                    }
                }

                // CONCILIADO!
                k++;
                if (adicionaNaListaPreConciliado)
                {
                    ConciliacaoTitulos recebimento = null;
                    ConciliacaoTitulos titulo = null;
                    if (item1.Tipo.Equals(TIPO_TITULO))
                    {
                        titulo = item1;
                        recebimento = item2;
                    }
                    else
                    {
                        recebimento = item1;
                        titulo = item2;
                    }

                    adicionaElementosConciliadosNaLista(listaConciliacao, recebimento, titulo, TIPO_CONCILIADO.PRE_CONCILIADO);
                }
            }
        }




       
        /// <summary>
        /// Retorna a lista de conciliação de títulos
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionConciliacaoTitulos = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                Dictionary<string, string> queryStringTbRecebimentoTitulo = new Dictionary<string, string>();
                // DATA
                string vigencia = String.Empty;
                string data = String.Empty;
                if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                {
                    data = queryString["" + (int)CAMPOS.DATA];
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, data);
                    queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.DTTITULO, data);
                }
                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de conciliação de títulos!");
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.NRCNPJ, CnpjEmpresa);
                }
                // ADQUIRENTE
                string cdAdquirente = String.Empty;
                if (queryString.TryGetValue("" + (int)CAMPOS.CDADQUIRENTE, out outValue))
                {
                    cdAdquirente = queryString["" + (int)CAMPOS.CDADQUIRENTE];
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDADQUIRENTE, cdAdquirente);
                    queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.CDADQUIRENTE, cdAdquirente);

                }
                // CONSIDERA NSU?
                bool consideraNSU = true;
                if (queryString.TryGetValue("" + (int)CAMPOS.CONSIDERA_NSU, out outValue))
                    consideraNSU = Convert.ToBoolean(queryString["" + (int)CAMPOS.CONSIDERA_NSU]);


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
                IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);
                IQueryable<tbRecebimentoTitulo> queryTbRecebimentoTitulo = GatewayTbRecebimentoTitulo.getQuery(0, (int)GatewayTbRecebimentoTitulo.CAMPOS.DTTITULO, 0, 0, 0, queryStringTbRecebimentoTitulo);


                // Só busca por conciliações já concretizadas se não tiver sido requisitado um filtro do tipo PRE-CONCILIADO ou NÃO CONCILIADO
                if (!filtroTipoPreConciliado && !filtroTipoNaoConciliado)
                {
                    #region OBTÉM AS INFORMAÇÕES DE DADOS JÁ CONCILIADOS PREVIAMENTE
                    // TÍTULOS JÁ CONCILIADOS, COM SEUS RESPECTIVOS RECEBIMENTOS CONCILIADOS
                    List<ConciliacaoTitulos> titulosConciliados = queryTbRecebimentoTitulo
                                                .Where(e => e.RecebimentoParcelas.Count > 0)
                                                .Select(e => new ConciliacaoTitulos
                                                {
                                                    Tipo = TIPO_TITULO, // título
                                                    Id = e.idRecebimentoTitulo,
                                                    NumParcela = e.nrParcela,
                                                    Nsu = e.nrNSU,
                                                    //CodResumoVendas = null,
                                                    Bandeira = e.dsBandeira.ToUpper(),
                                                    DataVenda = e.dtVenda,
                                                    Data = e.dtTitulo,
                                                    Filial = e.empresa.ds_fantasia + (e.empresa.filial != null ? " " + e.empresa.filial : ""),
                                                    Valor = e.vlParcela,
                                                    Adquirente = e.tbAdquirente.nmAdquirente.ToUpper(),
                                                }).ToList<ConciliacaoTitulos>();

                    // Total dos elementos já conciliados
                    if (titulosConciliados.Count > 0)
                    {
                        // Adiciona como conciliados
                        foreach (ConciliacaoTitulos titulo in titulosConciliados)
                        {
                            // Recebimento
                            Int32 idRecebimentoTitulo = Convert.ToInt32(titulo.Id);
                            ConciliacaoTitulos recebimento = _db.RecebimentoParcelas
                                                                    .Where(r => r.idRecebimentoTitulo == idRecebimentoTitulo)
                                                                    .OrderBy(r => r.dtaRecebimentoEfetivo ?? r.dtaRecebimento)
                                                                    //.ThenBy(r => r.Recebimento.dtaVenda)
                                                                    .Select(r => new ConciliacaoTitulos
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Id = r.idRecebimento,
                                                                        NumParcela = r.numParcela,
                                                                        Nsu = r.Recebimento.nsu,
                                                                        CodResumoVendas = r.Recebimento.codResumoVenda,
                                                                        Bandeira = r.Recebimento.tbBandeira.dsBandeira.ToUpper(),
                                                                        DataVenda = r.Recebimento.dtaVenda,
                                                                        Data = r.dtaRecebimentoEfetivo ?? r.dtaRecebimento,
                                                                        Filial = r.Recebimento.empresa.ds_fantasia + (r.Recebimento.empresa.filial != null ? " " + r.Recebimento.empresa.filial : ""),
                                                                        Valor = r.valorParcelaLiquida ?? new decimal(0.0),
                                                                        Adquirente = r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente.ToUpper(),
                                                                    }).FirstOrDefault<ConciliacaoTitulos>();

                            if (recebimento == null) continue; // falha!

                            // Adiciona
                            adicionaElementosConciliadosNaLista(CollectionConciliacaoTitulos, recebimento, titulo, TIPO_CONCILIADO.CONCILIADO);
                        }
                    }
                    #endregion
                }


                // Só busca por possíveis conciliações se não tiver sido requisitado um filtro do tipo CONCILIADO
                if (!filtroTipoConciliado)
                {
                    #region OBTÉM AS INFORMAÇÕES DE DADOS NÃO-CONCILIADOS E BUSCA PRÉ-CONCILIAÇÕES

                    #region OBTÉM SOMENTE OS RECEBIMENTOS PARCELAS NÃO-CONCILIADOS
                    List<ConciliacaoTitulos> recebimentosParcela = queryRecebimentoParcela
                                                        .Where(r => r.idRecebimentoTitulo == null)
                                                        .Select(r => new ConciliacaoTitulos
                                                        {
                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                            Id = r.idRecebimento,
                                                            NumParcela = r.numParcela,
                                                            Nsu = r.Recebimento.nsu,
                                                            CodResumoVendas = r.Recebimento.codResumoVenda,
                                                            Bandeira = r.Recebimento.tbBandeira.dsBandeira.ToUpper(),
                                                            DataVenda = r.Recebimento.dtaVenda,
                                                            Data = r.dtaRecebimentoEfetivo ?? r.dtaRecebimento,
                                                            Filial = r.Recebimento.empresa.ds_fantasia + (r.Recebimento.empresa.filial != null ? " " + r.Recebimento.empresa.filial : ""),
                                                            Valor = r.valorParcelaLiquida ?? new decimal(0.0),
                                                            Adquirente = r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente.ToUpper(),
                                                        }).ToList<ConciliacaoTitulos>();

                    #endregion

                    #region OBTÉM OS TÍTULOS NÃO CONCILIADOS
                    List<ConciliacaoTitulos> titulos = queryTbRecebimentoTitulo
                                                            // Só considera os títulos que não estão conciliados
                                                            .Where(e => e.RecebimentoParcelas.Count == 0)
                                                            .Select(e => new ConciliacaoTitulos
                                                            {
                                                                Tipo = TIPO_TITULO, // título
                                                                Id = e.idRecebimentoTitulo,
                                                                NumParcela = e.nrParcela,
                                                                Nsu = e.nrNSU,
                                                                //CodResumoVendas = null,
                                                                Bandeira = e.dsBandeira.ToUpper(),
                                                                DataVenda = e.dtVenda,
                                                                Data = e.dtTitulo,
                                                                Filial = e.empresa.ds_fantasia + (e.empresa.filial != null ? " " + e.empresa.filial : ""),
                                                                Valor = e.vlParcela,
                                                                Adquirente = e.tbAdquirente.nmAdquirente.ToUpper(),
                                                            }).ToList<ConciliacaoTitulos>();
                    #endregion


                    // TEM ELEMENTOS PARA CONCILIAR?
                    if (recebimentosParcela.Count == 0 || titulos.Count == 0)
                    {
                        if (!filtroTipoPreConciliado)
                        {
                            #region NÃO O QUE CONCILIAR => ADICIONA OS ELEMENTOS COMO NÃO CONCILIADOS
                            if (recebimentosParcela.Count > 0)
                            {
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoTitulos, recebimentosParcela);
                            }
                            else if (titulos.Count > 0)
                            {
                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoTitulos, titulos);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        #region CONCILIA DIRETAMENTE RECEBIMENTOPARCELA COM TBRECEBIMENTOTITULO

                        List<ConciliacaoTitulos> listaCandidatos = new List<ConciliacaoTitulos>();
                        List<ConciliacaoTitulos> listaNaoConciliado = new List<ConciliacaoTitulos>();

                        // Concatena as duas listas, ordenando por data
                        listaCandidatos = recebimentosParcela.Concat<ConciliacaoTitulos>(titulos)
                                              .OrderBy(c => c.Data.Year)
                                              .ThenBy(c => c.Data.Month)
                                              .ThenBy(c => c.Data.Day)
                                              .ThenBy(c => c.Valor)
                                              .ThenByDescending(c => c.Filial)
                                              .ThenByDescending(c => c.Adquirente)
                                              .ThenByDescending(c => c.Bandeira)
                                              .ToList<ConciliacaoTitulos>();

                        // Faz a conciliação
                        Concilia(CollectionConciliacaoTitulos, listaCandidatos, listaNaoConciliado, !filtroTipoNaoConciliado, consideraNSU);

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
                                                                        .ToList<ConciliacaoTitulos>();

                            titulos = listaNaoConciliado.Where(e => e.Tipo.Equals(TIPO_TITULO))
                                                                    .OrderBy(e => e.Data)
                                                                    .ThenBy(e => e.Filial)
                                                                    .ThenBy(e => e.Adquirente)
                                                                    .ThenBy(e => e.Bandeira)
                                                                    .ToList<ConciliacaoTitulos>();
                            #endregion

                            

                            if (!filtroTipoPreConciliado)
                            {
                                #region PASSO 7) ADICIONA OS ELEMENTOS QUE SOBRARAM COMO NÃO CONCILIADOS
                                if (recebimentosParcela.Count > 0)
                                {
                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoTitulos, recebimentosParcela);
                                }
                                if (titulos.Count > 0)
                                {
                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoTitulos, titulos);
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                }

                // Ordena
                CollectionConciliacaoTitulos = CollectionConciliacaoTitulos
                                                                .OrderBy(c => c.Data.Year)
                                                                .ThenBy(c => c.Data.Month)
                                                                .ThenBy(c => c.Data.Day)
                                                                .ThenBy(c => c.Valor)
                                                                .ThenByDescending(c => c.Adquirente)
                                                                .ThenByDescending(c => c.Bandeira)
                                                                .ToList<dynamic>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionConciliacaoTitulos.Count;

                // TOTAL
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valor", CollectionConciliacaoTitulos.Select(r => r.Valor).Cast<decimal>().Sum());

                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionConciliacaoTitulos = CollectionConciliacaoTitulos.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionConciliacaoTitulos;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar adquirente" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Aponta RecebimentoParcela para TbRecebimentoTitulo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, List<ConciliaRecebimentoParcelaTitulo> param)
        {
            try
            {
                foreach (ConciliaRecebimentoParcelaTitulo conciliaTitulo in param)
                {
                    if (conciliaTitulo.recebimentoParcela != null)
                    {
                        // Avalia o título
                        tbRecebimentoTitulo tbRecebimentoTitulo = null;
                        if (conciliaTitulo.idRecebimentoTitulo > 0)
                        {
                            tbRecebimentoTitulo = _db.tbRecebimentoTitulos.Where(e => e.idRecebimentoTitulo == conciliaTitulo.idRecebimentoTitulo).FirstOrDefault();
                            if (tbRecebimentoTitulo == null) continue; // título inválido!
                        }

                        // RECEBIMENTO PARCELA
                        RecebimentoParcela value = _db.RecebimentoParcelas
                                                                .Where(e => e.idRecebimento == conciliaTitulo.recebimentoParcela.idRecebimento)
                                                                .Where(e => e.numParcela == conciliaTitulo.recebimentoParcela.numParcela)
                                                                .FirstOrDefault();
                        if (value != null)
                        {
                            if (conciliaTitulo.idRecebimentoTitulo == -1) value.idRecebimentoTitulo = null;
                            else value.idRecebimentoTitulo = conciliaTitulo.idRecebimentoTitulo;
                            _db.SaveChanges();
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
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}