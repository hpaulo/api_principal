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
    public class GatewayRecebiveisFuturos
    {

        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRecebiveisFuturos()
        {
            _db.Configuration.ProxyCreationEnabled = false;
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
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionRecebiveisFuturos = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringAjustes = new Dictionary<string, string>();
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                // DATA
                DateTime dataNow = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, ""); // dtaRecebimentoEfetivo is null
                if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                {
                    // Não permite que o período seja inferior a data corrente
                    string data = queryString["" + (int)CAMPOS.DATA];
                    if (data.Contains(">")){
                        DateTime dataInicial = DateTime.ParseExact(data.Replace(">", "") + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if(dataInicial < dataNow)
                            throw new Exception("Período deve ser igual ou superior a data corrente (" + dataNow.ToShortDateString() + ")");
                        //data = ">" + Convert.ToDateTime(dataNow.ToShortDateString());
                    }
                    else if (data.Contains("|"))
                    {
                        DateTime dataInicial = DateTime.ParseExact(data.Substring(0, data.IndexOf("|")) + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (dataInicial < dataNow)
                            //data = Convert.ToDateTime(dataNow.ToShortDateString()) + data.Substring(data.IndexOf("|"));
                            throw new Exception("Data inicial do período deve ser igual ou superior a data corrente (" + dataNow.ToShortDateString() + ")");
                        DateTime dataFinal = DateTime.ParseExact(data.Substring(data.IndexOf("|") + 1) + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (dataFinal < dataNow)
                            //data = data.Substring(0, data.IndexOf("|") + 1) + Convert.ToDateTime(dataNow.ToShortDateString());
                            throw new Exception("Data final do período deve ser igual ou superior a data corrente (" + dataNow.ToShortDateString() + ")");
                        if (dataInicial > dataFinal)
                            throw new Exception("Período informado é inválido!");
                    }
                    else
                    {
                        DateTime dataInicial = DateTime.ParseExact(data + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (dataInicial < dataNow)
                            //data = ">" + Convert.ToDateTime(dataNow.ToShortDateString());
                            throw new Exception("Data informada deve ser igual ou superior a data corrente (" + dataNow.ToShortDateString() + ")");
                    }
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, data);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, data);
                }
                else
                {
                    // Período todo => começa a partir da data corrente
                    string data = ">" + dataNow.ToShortDateString();
                    queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, data);
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, data);
                }
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

                // SOMENTE PARCELAS E AJUSTES DE BANDEIRAS TIPO CRÉDITO
                queryStringAjustes.Add("" + (int)GatewayTbRecebimentoAjuste.CAMPOS.DSTIPO, "CRÉDITO");
                queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DSTIPO, "CRÉDITO");                


                // OBTÉM AS QUERIES
                IQueryable<tbRecebimentoAjuste> queryAjustes = GatewayTbRecebimentoAjuste.getQuery(0, (int)GatewayTbRecebimentoAjuste.CAMPOS.DTAJUSTE, 0, 0, 0, queryStringAjustes);
                IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);


                List<RecebiveisFuturos> ajustes = queryAjustes.Select(a => new RecebiveisFuturos
                {
                    data = a.dtAjuste,
                    valorBruto = a.vlAjuste > new decimal(0.0) ? a.vlAjuste : new decimal(0.0),
                    valorLiquido = a.vlAjuste,
                    valorDescontado = a.vlAjuste < new decimal(0.0) ? new decimal(-1.0) * a.vlAjuste : new decimal(0.0),
                    bandeira = a.tbBandeira.dsBandeira,
                    adquirente = a.tbBandeira.tbAdquirente.nmAdquirente
                }).OrderBy(r => r.data).ToList<RecebiveisFuturos>();

                List<RecebiveisFuturos> recebiveisFuturos = queryRecebimentoParcela.Select(r => new RecebiveisFuturos
                {
                    data = r.dtaRecebimento,
                    valorBruto = r.valorParcelaBruta,
                    valorLiquido = r.valorParcelaLiquida.Value,
                    valorDescontado = r.valorDescontado,
                    bandeira = r.Recebimento.cdBandeira != null ? r.Recebimento.tbBandeira.dsBandeira : r.Recebimento.BandeiraPos.desBandeira,
                    adquirente = r.Recebimento.cdBandeira != null ? r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente : r.Recebimento.BandeiraPos.Operadora.nmOperadora
                }).OrderBy(r => r.data).ToList<RecebiveisFuturos>();

                if (ajustes.Count > 0) recebiveisFuturos = recebiveisFuturos.Concat(ajustes).OrderBy(r => r.data).ToList<RecebiveisFuturos>();

                CollectionRecebiveisFuturos = recebiveisFuturos.GroupBy(r => new { r.data.Value.Month, r.data.Value.Year })
                                                               .Select(r => new
                                                               {
                                                                   competencia = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(r.Key.Month) + "/" + r.Key.Year,
                                                                   valorBruto = r.Sum(f => f.valorBruto),
                                                                   valorDescontado = r.Sum(f => f.valorDescontado),
                                                                   valorLiquido = r.Sum(f => f.valorLiquido),
                                                                   adquirentes = r.GroupBy(f => f.adquirente)
                                                                   .OrderBy(f => f.Key)
                                                                   .Select(f => new
                                                                   {
                                                                       adquirente = f.Key,
                                                                       valorBruto = f.Sum(x => x.valorBruto),
                                                                       valorDescontado = f.Sum(x => x.valorDescontado),
                                                                       valorLiquido = f.Sum(x => x.valorLiquido),
                                                                       bandeiras = f.GroupBy(x => x.bandeira)
                                                                       .OrderBy(x => x.Key)
                                                                       .Select(x => new
                                                                       {
                                                                           bandeira = x.Key,
                                                                           valorBruto = x.Sum(y => y.valorBruto),
                                                                           valorDescontado = x.Sum(y => y.valorDescontado),
                                                                           valorLiquido = x.Sum(y => y.valorLiquido),
                                                                       }).ToList<dynamic>(),
                                                                   }).ToList<dynamic>(),
                                                               }).ToList<dynamic>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionRecebiveisFuturos.Count;

                // TOTAL
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valorBruto", CollectionRecebiveisFuturos.Select(r => r.valorBruto).Cast<decimal>().Sum());
                retorno.Totais.Add("valorDescontado", CollectionRecebiveisFuturos.Select(r => r.valorDescontado).Cast<decimal>().Sum());
                retorno.Totais.Add("valorLiquido", CollectionRecebiveisFuturos.Select(r => r.valorLiquido).Cast<decimal>().Sum());

                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionRecebiveisFuturos = CollectionRecebiveisFuturos.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionRecebiveisFuturos;

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
