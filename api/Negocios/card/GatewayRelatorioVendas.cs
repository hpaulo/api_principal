using api.Bibliotecas;
using api.Models;
using api.Models.Object;
using api.Negocios.Pos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Negocios.Card
{
    public class GatewayRelatorioVendas
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRelatorioVendas()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100, // da venda
            ID_GRUPO = 101,
            NU_CNPJ = 102,
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
                List<dynamic> CollectionRelatorioVendas = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringAjustes = new Dictionary<string, string>();
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();

                // DATA DA VENDA => OBRIGATÓRIO
                DateTime dataNow = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                {
                    // Não permite que o período seja superior ou igual a data corrente
                    string data = queryString["" + (int)CAMPOS.DATA];
                    if (data.Contains("|"))
                    {
                        DateTime dataInicial = DateTime.ParseExact(data.Substring(0, data.IndexOf("|")) + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        //if (dataInicial >= dataNow)
                        //    throw new Exception("Data inicial do período de vendas deve ser inferior a data corrente (" + dataNow.ToShortDateString() + ")");
                        DateTime dataFinal = DateTime.ParseExact(data.Substring(data.IndexOf("|") + 1) + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        //if (dataFinal >= dataNow)
                        //    throw new Exception("Data final do período de vendas deve ser inferior a data corrente (" + dataNow.ToShortDateString() + ")");
                        if (dataInicial > dataFinal)
                            throw new Exception("Período de vendas informado é inválido!");
                    }
                    //else
                    //{
                    //    DateTime dataVenda = DateTime.ParseExact(data + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    //    if (dataVenda >= dataNow)
                    //        throw new Exception("Data da venda informada deve ser inferior a data corrente (" + dataNow.ToShortDateString() + ")");
                    //}
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTAVENDA, data);
                }
                else throw new Exception("Data ou período de vendas deve ser informado!");

                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de recebíveis futuros!");
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ, CnpjEmpresa);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                }


                // OBTÉM A QUERY
                IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTAVENDA, 0, 0, 0, queryStringRecebimentoParcela);

                List<RelatorioVendas> vendas = queryRecebimentoParcela.Select(r => new RelatorioVendas
                {
                    dataVenda = r.Recebimento.dtaVenda,
                    dataRecebimento = r.dtaRecebimentoEfetivo != null ? r.dtaRecebimentoEfetivo.Value : r.dtaRecebimento,
                    recebeu = (r.dtaRecebimentoEfetivo != null && r.dtaRecebimentoEfetivo.Value < dataNow) || (r.dtaRecebimentoEfetivo == null && r.dtaRecebimento < dataNow),
                    valorBruto = r.valorParcelaBruta,
                    valorLiquido = r.valorParcelaLiquida.Value,
                    valorDescontado = r.valorDescontado,
                    bandeira = r.Recebimento.cdBandeira != null ? r.Recebimento.tbBandeira.dsBandeira : r.Recebimento.BandeiraPos.desBandeira,
                    adquirente = r.Recebimento.cdBandeira != null ? r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente : r.Recebimento.BandeiraPos.Operadora.nmOperadora
                }).OrderBy(r => r.dataVenda).ToList<RelatorioVendas>();


                CollectionRelatorioVendas = vendas.GroupBy(r => new { r.dataVenda.Day, r.dataVenda.Month, r.dataVenda.Year })
                                                .Select(r => new
                                                {
                                                    diaVenda = (r.Key.Day < 10 ? "0" : "") + r.Key.Day + "/" + (r.Key.Month < 10 ? "0" : "") + r.Key.Month + "/" + r.Key.Year,
                                                    valorBruto = r.Sum(f => f.valorBruto),
                                                    valorDescontado = r.Sum(f => f.valorDescontado),
                                                    valorLiquido = r.Sum(f => f.valorLiquido),
                                                    valorRecebido = r.Where(f => f.recebeu == true).Sum(f => f.valorLiquido),
                                                    valorAReceber = r.Where(f => f.recebeu == false).Sum(f => f.valorLiquido),
                                                    adquirentes = r.GroupBy(f => f.adquirente)
                                                    .OrderBy(f => f.Key)
                                                    .Select(f => new
                                                    {
                                                        adquirente = f.Key,
                                                        valorBruto = f.Sum(x => x.valorBruto),
                                                        valorDescontado = f.Sum(x => x.valorDescontado),
                                                        valorLiquido = f.Sum(x => x.valorLiquido),
                                                        valorRecebido = f.Where(x => x.recebeu == true).Sum(x => x.valorLiquido),
                                                        valorAReceber = f.Where(x => x.recebeu == false).Sum(x => x.valorLiquido),
                                                        bandeiras = f.GroupBy(x => x.bandeira)
                                                        .OrderBy(x => x.Key)
                                                        .Select(x => new
                                                        {
                                                            bandeira = x.Key,
                                                            valorBruto = x.Sum(y => y.valorBruto),
                                                            valorDescontado = x.Sum(y => y.valorDescontado),
                                                            valorLiquido = x.Sum(y => y.valorLiquido),
                                                            valorRecebido = x.Where(y => y.recebeu == true).Sum(y => y.valorLiquido),
                                                            valorAReceber = x.Where(y => y.recebeu == false).Sum(y => y.valorLiquido),
                                                        }).ToList<dynamic>(),
                                                    }).ToList<dynamic>(),
                                                }).ToList<dynamic>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRelatorioVendas.Count;

                // Ajustes
                //List<RelatorioVendas> ajustes = new List<RelatorioVendas>();
                //if (vendas.Count > 0)
                //{
                //    // Orderna as vendas por data de recebimento para obter o intervalo de data de recebimento da consulta de vendas
                //    List<DateTime> recebimentos = vendas.Select(r => r.dataRecebimento).OrderBy(r => r).ToList<DateTime>();
                //    DateTime dtInicioRecebimento = recebimentos.First();
                //    DateTime dtFinalRecebimento = recebimentos.Last();

                //    string filtroDtAjuste = dtInicioRecebimento.Year +
                //                            (dtInicioRecebimento.Month < 10 ? "0" : "") + dtInicioRecebimento.Month +
                //                            (dtInicioRecebimento.Day < 10 ? "0" : "") + dtInicioRecebimento.Day + "|" +
                //                            dtFinalRecebimento.Year +
                //                            (dtFinalRecebimento.Month < 10 ? "0" : "") + dtFinalRecebimento.Month +
                //                            (dtFinalRecebimento.Day < 10 ? "0" : "") + dtFinalRecebimento.Day;

                //    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, filtroDtAjuste);

                //    // Obtém ajustes
                //    IQueryable<tbRecebimentoAjuste> queryAjustes = GatewayTbRecebimentoAjuste.getQuery(0, (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, 0, 0, queryStringAjustes);
                //    ajustes = queryAjustes.Select(a => new RelatorioVendas
                //    {
                //        dataVenda = a.dtAjuste,
                //        dataRecebimento = a.dtAjuste,
                //        recebeu = a.dtAjuste < dataNow,
                //        valorBruto = a.vlAjuste > new decimal(0.0) ? a.vlAjuste : new decimal(0.0),
                //        valorLiquido = a.vlAjuste,
                //        valorDescontado = a.vlAjuste < new decimal(0.0) ? new decimal(-1.0) * a.vlAjuste : new decimal(0.0),
                //        bandeira = a.tbBandeira.dsBandeira,
                //        adquirente = a.tbBandeira.tbAdquirente.nmAdquirente
                //    }).OrderBy(r => r.dataRecebimento).ToList<RelatorioVendas>();
                //}

                // TOTAL
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valorBruto", CollectionRelatorioVendas.Select(r => r.valorBruto).Cast<decimal>().Sum());
                retorno.Totais.Add("valorDescontado", CollectionRelatorioVendas.Select(r => r.valorDescontado).Cast<decimal>().Sum());
                retorno.Totais.Add("valorLiquido", CollectionRelatorioVendas.Select(r => r.valorLiquido).Cast<decimal>().Sum());
                retorno.Totais.Add("valorRecebido", CollectionRelatorioVendas.Select(r => r.valorRecebido).Cast<decimal>().Sum());
                retorno.Totais.Add("valorAReceber", CollectionRelatorioVendas.Select(r => r.valorAReceber).Cast<decimal>().Sum());
                //retorno.Totais.Add("valorAjustes", ajustes.Select(r => r.valorLiquido).Cast<decimal>().Sum());
                //retorno.Totais.Add("valorAjustesRecebido", ajustes.Where(a => a.recebeu == true).Select(r => r.valorLiquido).Cast<decimal>().Sum());
                //retorno.Totais.Add("valorAjustesAReceber", ajustes.Where(a => a.recebeu == false).Select(r => r.valorLiquido).Cast<decimal>().Sum());

                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionRelatorioVendas = CollectionRelatorioVendas.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionRelatorioVendas;

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
