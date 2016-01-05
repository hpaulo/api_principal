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

        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoRelatorios()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100, 
            ID_GRUPO = 101,
            NU_CNPJ = 102, 
        };

        /// <summary>
        /// Retorna a lista de conciliação bancária
        /// </summary>
        /// <returns></returns>        
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            // Abre conexão
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
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
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de relatório de conciliação!");
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    queryStringTbRecebimentoAjuste.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.NRCNPJ, CnpjEmpresa);
                    queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.NU_CNPJ, CnpjEmpresa);
                }
                //else throw new Exception("Uma filial deve ser selecionada como filtro de conciliação bancária!");

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
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.CDADQUIRENTE, "0!"); // Somente movimentações que tem adquirente/tipo cartão associado
                // PARA O EXTRATO, SÓ CONSIDERA OS TIPO CREDIT
                queryStringExtrato.Add("" + (int)GatewayTbExtrato.CAMPOS.DSTIPO, OFXSharp.OFXTransactionType.CREDIT.ToString());


                // OBTÉM AS QUERIES                
                IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(_db, 0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, 0, 0, queryStringRecebimentoParcela);
                IQueryable<tbRecebimentoAjuste> queryTbRecebimentoAjuste = GatewayTbRecebimentoAjuste.getQuery(_db, 0, (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, 0, 0, queryStringTbRecebimentoAjuste);
                IQueryable<tbExtrato> queryExtrato = GatewayTbExtrato.getQuery(_db, 0, (int)GatewayTbExtrato.CAMPOS.DTEXTRATO, 0, 0, 0, queryStringExtrato);


                List<ConciliacaoRelatorios> rRecebimentoParcela = queryRecebimentoParcela.Select(t => new ConciliacaoRelatorios
                {
                    tipo = "R",
                    adquirente = t.Recebimento.tbBandeira.tbAdquirente.nmAdquirente,
                    bandeira = t.Recebimento.tbBandeira.dsBandeira,
                    tipocartao = t.Recebimento.tbBandeira.dsTipo,
                    competencia = t.dtaRecebimentoEfetivo != null ? t.dtaRecebimentoEfetivo.Value : t.dtaRecebimento,
                    valorDescontado = t.valorDescontado,
                    valorDescontadoAntecipacao = t.vlDescontadoAntecipacao,
                    valorBruto = t.valorParcelaBruta,
                    valorLiquido = t.valorParcelaBruta - t.valorDescontado,//t.valorParcelaLiquida.Value,
                    idExtrato = t.idExtrato,
                    taxaCashFlow = (t.valorDescontado * new decimal(100.0)) / t.valorParcelaBruta
                }).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();

                List<ConciliacaoRelatorios> rRecebimentoAjuste = queryTbRecebimentoAjuste.Select(t => new ConciliacaoRelatorios
                {
                    tipo = "A",
                    adquirente = t.tbBandeira.tbAdquirente.nmAdquirente,
                    bandeira = t.tbBandeira.dsBandeira,
                    tipocartao = t.tbBandeira.dsTipo,
                    competencia = t.dtAjuste,
                    valorLiquido = t.vlAjuste,
                    valorDescontadoAntecipacao = new decimal(0.0),
                    valorBruto = t.vlAjuste > new decimal(0.0) ? t.vlAjuste : new decimal(0.0),
                    valorDescontado = t.vlAjuste < new decimal(0.0) ? new decimal(-1.0) * t.vlAjuste : new decimal(0.0),
                    idExtrato = t.idExtrato,
                    taxaCashFlow = new decimal(0.0) 
                }).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();


                // Agrupa
                List<dynamic> listaFinal = rRecebimentoParcela.Concat(rRecebimentoAjuste).GroupBy(t => t.competencia)
                                                           .Select(t => new
                                                           {
                                                               competencia = t.Key.ToShortDateString(),
                                                               taxaMedia = t.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado) * new decimal(100.0)) / t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),//t.Where(x => x.tipo.Equals("R")).Sum(x => x.taxaCashFlow) / t.Where(x => x.tipo.Equals("R")).Count(),
                                                               vendas = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                               valorDescontadoTaxaADM = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                               ajustesCredito = t.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                               ajustesDebito = t.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                               valorLiquido = t.Sum(x => x.valorLiquido),
                                                               valorDescontadoAntecipacao = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                               valorLiquidoTotal = t.Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                               extratoBancario = new decimal(0.0),
                                                               diferenca = t.Sum(x => x.valorLiquido),
                                                               status = t.Where(x => x.idExtrato == null).Count() > 0 ? "Não conciliado" : "Conciliado",

                                                               adquirentes = t.GroupBy(c => c.adquirente)
                                                                               .OrderBy(c => c.Key)
                                                                               .Select(c => new
                                                                               {
                                                                                   adquirente = c.Key,
                                                                                   taxaMedia = c.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado) * new decimal(100.0)) / c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),//c.Where(x => x.tipo.Equals("R")).Sum(x => x.taxaCashFlow) / c.Where(x => x.tipo.Equals("R")).Count(),
                                                                                   vendas = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                                                   valorDescontadoTaxaADM = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                                                   ajustesCredito = c.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                                                   ajustesDebito = c.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                                                   valorLiquido = c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                                                                                   valorDescontadoAntecipacao = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                   valorLiquidoTotal = c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                   extratoBancario = new decimal(0.0),
                                                                                   diferenca = c.Sum(x => x.valorLiquido),
                                                                                   status = c.Where(x => x.idExtrato == null).Count() > 0 ? "Não conciliado" : "Conciliado",

                                                                                   bandeiras = c.GroupBy(b => new { b.bandeira, b.tipocartao })
                                                                                                .OrderBy(b => b.Key.bandeira)
                                                                                                .ThenBy(b => b.Key.tipocartao)
                                                                                                .Select(b => new
                                                                                                {
                                                                                                    bandeira = b.Key.bandeira,
                                                                                                    taxaMedia = b.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado) * new decimal(100.0)) / b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),//b.Where(x => x.tipo.Equals("R")).Sum(x => x.taxaCashFlow) / b.Where(x => x.tipo.Equals("R")).Count(),
                                                                                                    vendas = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                                                                    valorDescontadoTaxaADM = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                                                                    ajustesCredito = b.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                                                                    ajustesDebito = b.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                                                                    valorLiquido = b.Sum(x => x.valorLiquido),
                                                                                                    valorDescontadoAntecipacao = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                                    valorLiquidoTotal = b.Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                                    extratoBancario = new decimal(0.0),
                                                                                                    diferenca = b.Sum(x => x.valorLiquido),
                                                                                                    status = b.Where(x => x.idExtrato == null).Count() > 0 ? "Não conciliado" : "Conciliado",

                                                                                                }).ToList<dynamic>()
                                                                               }).ToList<dynamic>(),

                                                           })
                                                           .ToList<dynamic>();



                List<tbExtrato> extratos = queryExtrato.ToList<tbExtrato>();
                int competenciaListaFinal = 0;
                foreach (tbExtrato extrato in extratos)
                {
                    ConciliacaoRelatorios e = new ConciliacaoRelatorios();
                    e.tipo = "E";
                    e.valorBruto = extrato.vlMovimento != null ? extrato.vlMovimento.Value : new decimal(0.0);
                    e.valorDescontado = new decimal(0.0);
                    e.valorDescontadoAntecipacao = new decimal(0.0);
                    e.valorLiquido = extrato.vlMovimento != null ? extrato.vlMovimento.Value : new decimal(0.0);
                    e.competencia = extrato.dtExtrato;
                    e.idExtrato = extrato.RecebimentoParcelas.Count + extrato.tbRecebimentoAjustes.Count;
                    e.taxaCashFlow = new decimal(0.0);

                    bool extratoConciliado = extrato.RecebimentoParcelas.Count > 0 || extrato.tbRecebimentoAjustes.Count > 0;

                    #region COMPETENCIA
                    // Lista agrupada
                    var competencia = listaFinal[competenciaListaFinal];
                    //var item = listaFinal[competenciaListaFinal];
                    while (Convert.ToDateTime(competencia.competencia) < e.competencia && competenciaListaFinal < listaFinal.Count - 1)
                        competencia = listaFinal[++competenciaListaFinal];
                    
                    // TEMP
                    //if (competenciaListaFinal > 2)
                    //    break;

                    bool competenciaExistente = Convert.ToDateTime(competencia.competencia) == e.competencia;

                    var competenciaAtualizada = new
                    {
                        competencia = e.competencia.ToShortDateString(),
                        taxaMedia = !competenciaExistente ? new decimal(0.0) : competencia.taxaMedia,
                        vendas = !competenciaExistente ?  new decimal(0.0) : competencia.vendas,
                        valorDescontadoTaxaADM = !competenciaExistente ?  new decimal(0.0) : competencia.valorDescontadoTaxaADM,
                        ajustesCredito = !competenciaExistente ?  new decimal(0.0) : competencia.ajustesCredito,
                        ajustesDebito = !competenciaExistente ?  new decimal(0.0) : competencia.ajustesDebito,
                        valorLiquido = !competenciaExistente ?  new decimal(0.0) : competencia.valorLiquido,
                        valorDescontadoAntecipacao = !competenciaExistente ?  new decimal(0.0) : competencia.valorDescontadoAntecipacao,
                        valorLiquidoTotal = !competenciaExistente ?  new decimal(0.0) : competencia.valorLiquidoTotal,
                        extratoBancario = competenciaExistente ? competencia.extratoBancario + e.valorLiquido : e.valorLiquido,
                        diferenca = competenciaExistente ? Math.Abs(competencia.valorLiquido - competencia.extratoBancario - e.valorLiquido) : e.valorLiquido,
                        status = !competenciaExistente ? extratoConciliado ? "Conciliado" : "Não Conciliado" :
                                        competencia.status.Equals("Não conciliado") || !extratoConciliado ?
                                            competencia.diferenca <= new decimal(0.3) ? "Pré-Conciliado" : "Não conciliado" : 
                                            "Conciliado",

                        adquirentes = !competenciaExistente ? new List<dynamic>() : competencia.adquirentes
                    };

                   
                    // Verifica se a data consta na lista
                    if (!competenciaExistente)
                    {
                        if (Convert.ToDateTime(competencia.competencia) < e.competencia)
                        {
                            if (competenciaListaFinal < listaFinal.Count - 1)
                                listaFinal.Insert(competenciaListaFinal + 1, competenciaAtualizada);
                            else
                                listaFinal.Add(competenciaAtualizada);
                        }
                        else
                            listaFinal.Insert(competenciaListaFinal, competenciaAtualizada);
                    }
                    else
                    { 
                        // Remove a antiga e re-adiciona
                        listaFinal.RemoveAt(competenciaListaFinal);
                        listaFinal.Insert(competenciaListaFinal, competenciaAtualizada);
                    }

                    #endregion

                    #region PARÂMETRO BANCÁRIO
                    tbBancoParametro parametro = _db.tbBancoParametro.Where(p => p.dsMemo.Equals(extrato.dsDocumento))
                                                                     .Where(p => p.cdBanco.Equals(extrato.tbContaCorrente.cdBanco))
                                                                     .FirstOrDefault();

                    if (parametro == null)
                    {
                        e.adquirente = "Indefinida";
                        e.bandeira = "Indefinida";
                        e.tipocartao = "";
                    }
                    else
                    {
                        e.adquirente = parametro.cdAdquirente != null ? parametro.tbAdquirente.nmAdquirente : "Indefinida";
                        e.bandeira = parametro.cdBandeira != null ? parametro.tbBandeira.dsBandeira : "Indefinida";
                        e.tipocartao = parametro.dsTipoCartao != null ? parametro.dsTipoCartao : "";
                    }

                    List<dynamic> bandeiras = new List<dynamic>() { new { bandeira = e.bandeira, tipocartao = e.tipocartao } };

                    // Analisa pela conciliação a adquirente e bandeira
                    if (extratoConciliado)
                    {
                        // Memo sem adquirente?
                        if (e.adquirente.Equals("Indefinida"))
                        {
                            if (extrato.RecebimentoParcelas.Count > 0)
                                e.adquirente = extrato.RecebimentoParcelas.First().Recebimento.tbBandeira.tbAdquirente.nmAdquirente;
                            else
                                e.adquirente = extrato.tbRecebimentoAjustes.First().tbBandeira.tbAdquirente.nmAdquirente;
                        }

                        // Memo sem bandeira?
                        if (e.bandeira.Equals("Indefinida"))
                        {
                            // Obtém as bandeiras
                            List<dynamic> bandeirasConciliadasParcelas = extrato.RecebimentoParcelas.GroupBy(r => new { r.Recebimento.tbBandeira.dsBandeira, r.Recebimento.tbBandeira.dsTipo }).Select(r => new { bandeira = r.Key.dsBandeira, tipocartao = r.Key.dsTipo } ).ToList<dynamic>();
                            List<dynamic> bandeirasConciliadasAjustes = extrato.tbRecebimentoAjustes.GroupBy(r => new { r.tbBandeira.dsBandeira, r.tbBandeira.dsTipo }).Select(r => new { bandeira = r.Key.dsBandeira, tipocartao = r.Key.dsTipo }).ToList<dynamic>();
                            bandeiras = bandeirasConciliadasParcelas.Concat(bandeirasConciliadasAjustes).OrderBy(r => r.bandeira).ToList<dynamic>();
                        }
                    }
                    #endregion

                    #region ADQUIRENTE
                    int adquirenteListaFinal = 0;
                    // Procura a adquirente
                    var adquirente = competencia.adquirentes.Count > 0 ? competencia.adquirentes[adquirenteListaFinal] : 
                                                new
                                                {
                                                    adquirente = e.adquirente,
                                                    taxaMedia = new decimal(0.0),
                                                    vendas = new decimal(0.0),
                                                    valorDescontadoTaxaADM = new decimal(0.0),
                                                    ajustesCredito = new decimal(0.0),
                                                    ajustesDebito = new decimal(0.0),
                                                    valorLiquido = new decimal(0.0),
                                                    valorDescontadoAntecipacao = new decimal(0.0),
                                                    valorLiquidoTotal = new decimal(0.0),
                                                    extratoBancario = new decimal(0.0),
                                                    diferenca = new decimal(0.0),
                                                    status = "Não conciliado",

                                                    bandeiras = new List<dynamic>()
                                                };

                    while (string.Compare(e.adquirente, adquirente.adquirente, StringComparison.Ordinal) > 0 && adquirenteListaFinal < competencia.adquirentes.Count - 1)
                        adquirente = competencia.adquirentes[++adquirenteListaFinal];


                    bool adquirenteExistente = adquirente.adquirente.Equals(e.adquirente);

                    var adquirenteAtualizada = new
                    {
                        adquirente = e.adquirente,
                        taxaMedia =  !adquirenteExistente ? new decimal(0.0) : adquirente.taxaMedia,
                        vendas = !adquirenteExistente ? new decimal(0.0) : adquirente.vendas,
                        valorDescontadoTaxaADM = !adquirenteExistente ? new decimal(0.0) : adquirente.valorDescontadoTaxaADM,
                        ajustesCredito = !adquirenteExistente ? new decimal(0.0) : adquirente.ajustesCredito,
                        ajustesDebito = !adquirenteExistente ?  new decimal(0.0) : adquirente.ajustesDebito,
                        valorLiquido = !adquirenteExistente ? new decimal(0.0) : adquirente.valorLiquido,
                        valorDescontadoAntecipacao = !adquirenteExistente ? new decimal(0.0) : adquirente.valorDescontadoAntecipacao,
                        valorLiquidoTotal = !adquirenteExistente ? new decimal(0.0) : adquirente.valorLiquidoTotal,
                        extratoBancario = adquirenteExistente ? adquirente.extratoBancario + e.valorLiquido : e.valorLiquido,
                        diferenca = adquirenteExistente ? Math.Abs(adquirente.valorLiquido - adquirente.extratoBancario - e.valorLiquido) : e.valorLiquido,
                        status = !adquirenteExistente ? extratoConciliado ? "Conciliado" : "Não Conciliado" :
                                        adquirente.status.Equals("Não conciliado") || !extratoConciliado ?
                                            adquirente.diferenca <= new decimal(0.3) ? "Pré-Conciliado" : "Não conciliado" :
                                            "Conciliado",

                        bandeiras = !adquirenteExistente ? new List<dynamic>() : adquirente.bandeiras
                    };


                    // Verifica se a adquirente consta na competência
                    if (!adquirenteExistente)
                    {
                        if (string.Compare(e.adquirente, adquirente.adquirente, StringComparison.Ordinal) > 0)
                        {
                            if (adquirenteListaFinal < competencia.adquirentes.Count - 1)
                                competencia.adquirentes.Insert(adquirenteListaFinal + 1, adquirenteAtualizada);
                            else
                                competencia.adquirentes.Add(adquirenteAtualizada);
                        }
                        else
                            competencia.adquirentes.Insert(adquirenteListaFinal, adquirenteAtualizada);
                    }
                    else
                    {
                        // Remove a antiga e re-adiciona
                        competencia.adquirentes.RemoveAt(adquirenteListaFinal);
                        competencia.adquirentes.Insert(adquirenteListaFinal, adquirenteAtualizada);
                    }

                    #endregion

                    #region BANDEIRA
                    decimal valorLiquidoRestante = e.valorLiquido;
                    foreach (var bd in bandeiras)
                    {
                        // Procura a bandeira
                        int bandeiraListaFinal = 0;
                        var band = adquirente.bandeiras.Count > 0 ? adquirente.bandeiras[bandeiraListaFinal] :
                                                    new
                                                    {
                                                        bandeira = bd.bandeira + (!bd.bandeira.Equals("Indefinida") || bd.tipocartao.Equals("") ? "" : " (" + bd.tipocartao + ")"),
                                                        taxaMedia = new decimal(0.0),
                                                        vendas = new decimal(0.0),
                                                        valorDescontadoTaxaADM = new decimal(0.0),
                                                        ajustesCredito = new decimal(0.0),
                                                        ajustesDebito = new decimal(0.0),
                                                        valorLiquido = new decimal(0.0),
                                                        valorDescontadoAntecipacao = new decimal(0.0),
                                                        valorLiquidoTotal = new decimal(0.0),
                                                        extratoBancario = new decimal(0.0),
                                                        diferenca = new decimal(0.0),
                                                        status = "Não conciliado",
                                                    };

                        while (string.Compare(bd.bandeira, band.bandeira, StringComparison.Ordinal) > 0 && bandeiraListaFinal < adquirente.bandeiras.Count - 1)
                            band = adquirente.bandeiras[++bandeiraListaFinal];


                        bool bandeiraExistente = band.bandeira.Equals(bd.bandeira);
                        bool extratoMaiorQueRecebiveis = valorLiquidoRestante > band.diferenca;

                        var bandeiraAtualizada = new
                        {
                            bandeira = bd.bandeira + (!bd.bandeira.Equals("Indefinida") || bd.tipocartao.Equals("") ? "" : " (" + bd.tipocartao + ")"),
                            taxaMedia = !bandeiraExistente ? new decimal(0.0) : band.taxaMedia,
                            vendas = !bandeiraExistente ? new decimal(0.0) : band.vendas,
                            valorDescontadoTaxaADM = !bandeiraExistente ? new decimal(0.0) : band.valorDescontadoTaxaADM,
                            ajustesCredito = !bandeiraExistente ?  new decimal(0.0) : band.ajustesCredito,
                            ajustesDebito = !bandeiraExistente ? new decimal(0.0) : band.ajustesDebito,
                            valorLiquido = !bandeiraExistente ? new decimal(0.0) : band.valorLiquido,
                            valorDescontadoAntecipacao = !bandeiraExistente ? new decimal(0.0) : band.valorDescontadoAntecipacao,
                            valorLiquidoTotal = !bandeiraExistente ? new decimal(0.0) : band.valorLiquidoTotal,
                            extratoBancario = !bandeiraExistente ? e.valorLiquido :
                                                 bandeiras.Count == 1 || !extratoMaiorQueRecebiveis ?
                                                    band.extratoBancario + valorLiquidoRestante :
                                                    band.extratoBancario + band.valorLiquido,
                            diferenca = !bandeiraExistente ? e.valorLiquido :
                                            bandeiras.Count == 1 || !extratoMaiorQueRecebiveis ?
                                                Math.Abs(band.diferenca - valorLiquidoRestante) :
                                                new decimal(0.0),
                            status = !bandeiraExistente ? extratoConciliado ? "Conciliado" : "Não Conciliado" :
                                        !band.status.Equals("Não conciliado") && extratoConciliado ? "Conciliado" :
                                            bandeiras.Count == 1 || !extratoMaiorQueRecebiveis ?
                                                 Math.Abs(band.diferenca - valorLiquidoRestante) <= new decimal(0.3) ? "Pré-Conciliado" : "Não conciliado" : 
                                                 "Pré-Conciliado"
                        };

                        // Decrementa valor usado
                        if(bandeiras.Count > 1 && extratoMaiorQueRecebiveis)
                            valorLiquidoRestante -= band.diferenca;


                        // Verifica se a adquirente consta na competência
                        if (!bandeiraExistente)
                        {
                            if (string.Compare(bd.bandeira, band.bandeira, StringComparison.Ordinal) > 0)
                            {
                                if (bandeiraListaFinal < adquirente.bandeiras.Count - 1)
                                    adquirente.bandeiras.Insert(bandeiraListaFinal + 1, bandeiraAtualizada);
                                else
                                    adquirente.bandeiras.Add(bandeiraAtualizada);
                            }
                            else
                                adquirente.bandeiras.Insert(bandeiraListaFinal, bandeiraAtualizada);
                        }
                        else
                        {
                            // Remove a antiga e re-adiciona
                            adquirente.bandeiras.RemoveAt(bandeiraListaFinal);
                            adquirente.bandeiras.Insert(bandeiraListaFinal, bandeiraAtualizada);
                        }
                        
                    }

                    #endregion
                }

                //List<dynamic> temp = queryExtrato.Select(t => new
                //{
                //    t.idExtrato,
                //    t.dsDocumento,
                //    conta = new { t.tbContaCorrente.cdContaCorrente, t.tbContaCorrente.cdBanco, t.tbContaCorrente.nrAgencia, t.tbContaCorrente.nrConta },
                //    adquirente = _db.tbBancoParametro.Where(p => p.dsMemo.Equals(t.dsDocumento))
                //                                                                                 .Where(p => p.cdBanco.Equals(t.tbContaCorrente.cdBanco))
                //                                                                                 .Select(p => p.tbAdquirente.nmAdquirente.ToUpper())
                //                                                                                 .FirstOrDefault() ?? "Indefinida",
                //    bandeira = _db.tbBancoParametro.
                //                                    Where(p => p.cdBanco.Equals(t.tbContaCorrente.cdBanco)
                //                                    && p.dsMemo.Equals(t.dsDocumento)).Select(p => p.tbBandeira.dsBandeira ?? "Indefinida"
                //                                    ).FirstOrDefault() ?? "Indefinida",
                //    tipocartao = _db.tbBancoParametro.
                //                                    Where(p => p.cdBanco.Equals(t.tbContaCorrente.cdBanco)
                //                                    && p.dsMemo.Equals(t.dsDocumento)).Select(p => p.dsTipoCartao ?? ""
                //                                    ).FirstOrDefault() ?? "",
                //    valorBruto = t.vlMovimento ?? new decimal(0.0),
                //    valorDescontado = new decimal(0.0),
                //    valorDescontadoAntecipacao = new decimal(0.0),
                //    valorLiquido = t.vlMovimento ?? new decimal(0.0),
                //    competencia = t.dtExtrato,
                //    taxaCashFlow = new decimal(0.0)
                //}).Where(t => t.adquirente.Equals("AMEX")).OrderByDescending(t => t.competencia).ThenBy(t => t.adquirente).ToList<dynamic>();

                /*List<ConciliacaoRelatorios> listaCompleta = rRecebimentoParcela.Concat(rRecebimentoAjuste).Concat(rRecebimentoExtrato).OrderBy(t => t.competencia).ToList<ConciliacaoRelatorios>();

                List<dynamic> listaFinal = listaCompleta.GroupBy(t => t.competencia)
                                                           .Select(t => new
                                                           {
                                                               competencia = t.Key.ToShortDateString(),
                                                               //bandeira = t.Key.bandeira,
                                                               //competencia = t.Key.competencia,
                                                               taxaMedia = t.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado) * new decimal(100.0)) / t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),//t.Where(x => x.tipo.Equals("R")).Sum(x => x.taxaCashFlow) / t.Where(x => x.tipo.Equals("R")).Count(),
                                                               vendas = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                               valorDescontadoTaxaADM = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                               ajustesCredito = t.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                               ajustesDebito = t.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                               valorLiquido = t.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                                                               valorDescontadoAntecipacao = t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                               valorLiquidoTotal = t.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                               extratoBancario = t.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                                                               diferenca = Math.Abs(t.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)),
                                                               status = t.Where(x => !x.tipo.Equals("E")).Where(x => x.idExtrato == null).Count() > 0 || t.Where(x => x.tipo.Equals("E")).Where(x => x.idExtrato == 0).Count() > 0 ? Math.Abs(t.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - t.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)) <= new decimal(0.3) ? "Pré-Conciliado" : "Não conciliado" : "Conciliado",

                                                               adquirentes = t.GroupBy(c => c.adquirente)
                                                                               .OrderBy(c => c.Key)
                                                                               .Select(c => new
                                                                               {
                                                                                   adquirente = c.Key,
                                                                                   taxaMedia = c.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado) * new decimal(100.0)) / c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),//c.Where(x => x.tipo.Equals("R")).Sum(x => x.taxaCashFlow) / c.Where(x => x.tipo.Equals("R")).Count(),
                                                                                   vendas = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                                                   valorDescontadoTaxaADM = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                                                   ajustesCredito = c.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                                                   ajustesDebito = c.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                                                   valorLiquido = c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                                                                                   valorDescontadoAntecipacao = c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                   valorLiquidoTotal = c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                   extratoBancario = c.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                                                                                   diferenca = Math.Abs(c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)),
                                                                                   status = c.Where(x => !x.tipo.Equals("E")).Where(x => x.idExtrato == null).Count() > 0 || c.Where(x => x.tipo.Equals("E")).Where(x => x.idExtrato == 0).Count() > 0 ? Math.Abs(c.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - c.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)) <= new decimal(0.3) ? "Pré-Conciliado" : "Não conciliado" : "Conciliado",

                                                                                   bandeiras = c.GroupBy(b => new { b.bandeira, b.tipocartao })
                                                                                                .OrderBy(b => b.Key.bandeira)
                                                                                                .ThenBy(b => b.Key.tipocartao)
                                                                                                .Select(b => new
                                                                                                {
                                                                                                    bandeira = b.Key.bandeira + (!b.Key.bandeira.Equals("Indefinida") || b.Key.tipocartao.Equals("") ? "" : " (" + b.Key.tipocartao + ")"),
                                                                                                    taxaMedia = b.Where(x => x.tipo.Equals("R")).Count() == 0 ? new decimal(0.0) : (b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado) * new decimal(100.0)) / b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),//b.Where(x => x.tipo.Equals("R")).Sum(x => x.taxaCashFlow) / b.Where(x => x.tipo.Equals("R")).Count(),
                                                                                                    vendas = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorBruto),
                                                                                                    valorDescontadoTaxaADM = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontado),
                                                                                                    ajustesCredito = b.Where(x => x.tipo.Equals("A")).Sum(x => x.valorBruto),
                                                                                                    ajustesDebito = b.Where(x => x.tipo.Equals("A")).Sum(x => x.valorDescontado),
                                                                                                    valorLiquido = b.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                                                                                                    valorDescontadoAntecipacao = b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                                    valorLiquidoTotal = b.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("R")).Sum(x => x.valorDescontadoAntecipacao),
                                                                                                    extratoBancario = b.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido),
                                                                                                    diferenca = Math.Abs(b.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)),
                                                                                                    status = b.Where(x => !x.tipo.Equals("E")).Where(x => x.idExtrato == null).Count() > 0 || b.Where(x => x.tipo.Equals("E")).Where(x => x.idExtrato == 0).Count() > 0 ? Math.Abs(b.Where(x => !x.tipo.Equals("E")).Sum(x => x.valorLiquido) - b.Where(x => x.tipo.Equals("E")).Sum(x => x.valorLiquido)) <= new decimal(0.3) ? "Pré-Conciliado" : "Não conciliado" : "Conciliado",

                                                                                                }).ToList<dynamic>()
                                                                               }).ToList<dynamic>(),

                                                           })
                                                           .ToList<dynamic>();*/
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
