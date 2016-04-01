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
using System.Data.Entity;
using System.Globalization;
using api.Negocios.Cliente;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace api.Negocios.Card
{
    public class GatewayConciliacaoVendas
    { 
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoVendas()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
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
            PRECONCILIA_GRUPO = 104,
            NSU = 105,

            // RELACIONAMENTOS
            CDADQUIRENTE = 200,

            IDRECEBIMENTO = 300
        };

        public enum TIPO_CONCILIADO
        {
            CONCILIADO = 1,
            PRE_CONCILIADO = 2,
            NAO_CONCILIADO = 3
        };

        public static string TIPO_VENDA = "V";
        public static string TIPO_RECEBIMENTO = "R";
        // Pré-Conciliação
        private const int RANGE_DIAS_ANTERIOR = 1;
        private const int RANGE_DIAS_POSTERIOR = 1;
        private static decimal TOLERANCIA = new decimal(0.1); // R$0,10 


        /// <summary>
        /// Adiciona elementos da listaNaoConciliado na lista
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos da conciliação</param>
        /// <param name="listaNaoConciliado">Lista que contém elementos não conciliados</param>
        /// <returns></returns>
        private static void adicionaElementosNaoConciliadosNaLista(List<dynamic> listaConciliacao,
                                                                   List<ConciliacaoVendas> listaNaoConciliado)
        {
            foreach (ConciliacaoVendas item in listaNaoConciliado)
            {
                listaConciliacao.Add(new
                {
                    Conciliado = (int)TIPO_CONCILIADO.NAO_CONCILIADO,
                    Venda = item.Tipo != TIPO_VENDA ? null : new
                    {
                        Id = item.Id,
                        Nsu = item.Nsu,
                        Valor = item.Valor,
                        Bandeira = item.Bandeira,
                        Data = item.Data,
                        Filial = item.Filial,
                        Parcelas = item.Parcelas,
                    },
                    Recebimento = item.Tipo != TIPO_RECEBIMENTO ? null : new
                    {
                        Id = item.Id,
                        Nsu = item.Nsu,
                        CodResumoVendas = item.CodResumoVendas,
                        Valor = item.Valor,
                        Bandeira = item.Bandeira,
                        Data = item.Data,
                        Filial = item.Filial,
                        Parcelas = item.Parcelas,
                    },
                    Adquirente = item.Adquirente,         
                });
            }
        }

        /// <summary>
        /// Adiciona elementos na lista ao qual foram encontrados registros que "casaram"
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos da conciliação</param>
        /// <param name="recebimento">ConciliacaoBancaria referente ao lado do ICARD</param>
        /// <param name="movimentacao">ConciliacaoBancaria referente ao lado do venda</param>
        /// <param name="tipo">CONCILIADO ou PRE-CONCILIADO</param>
        /// <returns></returns>
        private static void adicionaElementosConciliadosNaLista(List<dynamic> listaConciliacao,
                                                     ConciliacaoVendas recebimento,
                                                     ConciliacaoVendas venda,
                                                     TIPO_CONCILIADO tipo)
        {
            if (recebimento != null && venda != null)
            {
                // Adiciona
                listaConciliacao.Add(new
                {
                    Conciliado = (int)tipo,
                    Venda = new
                    {
                        Id = venda.Id,
                        Nsu = venda.Nsu,
                        Valor = venda.Valor,
                        Bandeira = venda.Bandeira,
                        Data = venda.Data,
                        Filial = venda.Filial,
                        Parcelas = venda.Parcelas,
                    },
                    Recebimento = new
                    {
                        Id = recebimento.Id,
                        Nsu = recebimento.Nsu,
                        CodResumoVendas = recebimento.CodResumoVendas,
                        Valor = recebimento.Valor,
                        Bandeira = recebimento.Bandeira,
                        Data = recebimento.Data,
                        Filial = recebimento.Filial,
                        Parcelas = recebimento.Parcelas,
                    },
                    Adquirente = recebimento.Adquirente,
                });
            }
        }




       
        /// <summary>
        /// Retorna a lista de conciliação de vendas
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionConciliacaoVendas = new List<dynamic>();
                Retorno retorno = new Retorno();

                string outValue = null;

                if (colecao == 0)
                {
                    #region COLEÇÃO 0
                    // QUERIES DE FILTRO
                    Dictionary<string, string> queryStringRecebimento = new Dictionary<string, string>();
                    Dictionary<string, string> queryStringTbRecebimentoVenda = new Dictionary<string, string>();

                    // PRÉ-CONCILIAÇÃO CONSIDERANDO TODO O GRUPO?
                    bool preConciliaComGrupo = false;
                    if (queryString.TryGetValue("" + (int)CAMPOS.PRECONCILIA_GRUPO, out outValue))
                        preConciliaComGrupo = Convert.ToBoolean(queryString["" + (int)CAMPOS.PRECONCILIA_GRUPO]);


                    // DATA
                    string data = String.Empty;
                    if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                    {
                        data = queryString["" + (int)CAMPOS.DATA];
                        queryStringRecebimento.Add("" + (int)GatewayRecebimento.CAMPOS.DTAVENDA, data);
                        //queryStringTbRecebimentoVenda.Add("" + (int)GatewayTbRecebimentoVenda.CAMPOS.DTTITULO, data);
                    }
                    // GRUPO EMPRESA => OBRIGATÓRIO!
                    Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                    if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                        IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                    if (IdGrupo != 0)
                    {
                        queryStringRecebimento.Add("" + (int)GatewayRecebimento.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                        queryStringTbRecebimentoVenda.Add("" + (int)GatewayTbRecebimentoVenda.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    }
                    else throw new Exception("Um grupo deve ser selecionado como filtro de conciliação de vendas!");
                    // FILIAL
                    string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                    if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                        CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                    if (!CnpjEmpresa.Equals(""))
                    {
                        queryStringRecebimento.Add("" + (int)GatewayRecebimento.CAMPOS.CNPJ, CnpjEmpresa);
                        //queryStringTbRecebimentoVenda.Add("" + (int)GatewayTbRecebimentoVenda.CAMPOS.NRCNPJ, CnpjEmpresa);
                    }
                    // ADQUIRENTE
                    string cdAdquirente = String.Empty;
                    if (queryString.TryGetValue("" + (int)CAMPOS.CDADQUIRENTE, out outValue))
                    {
                        cdAdquirente = queryString["" + (int)CAMPOS.CDADQUIRENTE];
                        queryStringRecebimento.Add("" + (int)GatewayRecebimento.CAMPOS.CDADQUIRENTE, cdAdquirente);
                        queryStringTbRecebimentoVenda.Add("" + (int)GatewayTbRecebimentoVenda.CAMPOS.CDADQUIRENTE, cdAdquirente);
                    }

                    // NSU
                    if (queryString.TryGetValue("" + (int)CAMPOS.NSU, out outValue))
                    {
                        queryStringRecebimento.Add("" + (int)GatewayRecebimento.CAMPOS.NSU, queryString["" + (int)CAMPOS.NSU]);
                        //queryStringTbRecebimentoVenda.Add("" + (int)GatewayTbRecebimentoVenda.CAMPOS.NRNSU, nsu);
                    }

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
                    //IQueryable<Recebimento> queryRecebimento = GatewayRecebimento.getQuery(_db, 0, (int)GatewayRecebimento.CAMPOS.DTAVENDA, 0, 0, 0, queryStringRecebimento);
                    //IQueryable<tbRecebimentoVenda> queryTbRecebimentoVenda = GatewayTbRecebimentoVenda.getQuery(_db, 0, (int)GatewayTbRecebimentoVenda.CAMPOS.DTTITULO, 0, 0, 0, queryStringTbRecebimentoVenda);

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

                        #region OBTÉM COMPONENTE QUERY
                        SimpleDataBaseQuery dataBaseQueryRB = GatewayRecebimento.getQuery((int)GatewayRecebimento.CAMPOS.DTAVENDA, 0, queryStringRecebimento);
                        SimpleDataBaseQuery dataBaseQueryVD = GatewayTbRecebimentoVenda.getQuery((int)GatewayTbRecebimentoVenda.CAMPOS.DTVENDA, 0, queryStringTbRecebimentoVenda);

                        // RECEBIMENTO
                        // Adiciona join com tbBandeira e tbAdquirente, caso não exista
                        if (!dataBaseQueryRB.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                            dataBaseQueryRB.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                        if (!dataBaseQueryRB.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                            dataBaseQueryRB.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                        if (!dataBaseQueryRB.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            dataBaseQueryRB.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                        dataBaseQueryRB.select = new string[] { GatewayRecebimento.SIGLA_QUERY + ".id as idRecebimento",
                                                          GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda",
                                                          GatewayRecebimento.SIGLA_QUERY + ".nsu",
                                                          GatewayRecebimento.SIGLA_QUERY + ".codResumoVenda",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                          GatewayRecebimento.SIGLA_QUERY + ".dtaVenda",
                                                          GatewayRecebimento.SIGLA_QUERY + ".valorVendaBruta",
                                                          GatewayRecebimento.SIGLA_QUERY + ".numParcelaTotal",
                                                          GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                          GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                          GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",                                                         
                                                        };

                        // Sem ordem
                        dataBaseQueryRB.groupby = null;
                        dataBaseQueryRB.readUncommited = true;
                        dataBaseQueryRB.orderby = new string[] { GatewayRecebimento.SIGLA_QUERY + ".dtaVenda",
                                                           GatewayRecebimento.SIGLA_QUERY + ".valorVendaBruta"};


                        // TITULO
                        // Adiciona join com tbAdquirente, caso não exista
                        if (!dataBaseQueryVD.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                            dataBaseQueryVD.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".cdAdquirente");
                        if (!dataBaseQueryVD.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            dataBaseQueryVD.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrCNPJ = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                        dataBaseQueryVD.select = new string[] { GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda",
                                                          GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrNSU",
                                                          "dsBandeira = UPPER(" + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".dsBandeira)",
                                                          GatewayTbRecebimentoVenda.SIGLA_QUERY + ".dtVenda",
                                                          GatewayTbRecebimentoVenda.SIGLA_QUERY + ".vlVenda",
                                                          GatewayTbRecebimentoVenda.SIGLA_QUERY + ".qtParcelas",
                                                          GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                          GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                          GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",                                                         
                                                        };
                        dataBaseQueryVD.groupby = null;
                        dataBaseQueryVD.readUncommited = true;

                        #endregion

                        // Para a paginação
                        int totalConciliados = 0;
                        int skipRows = (pageNumber - 1) * pageSize;

                        retorno.TotalDeRegistros = 0;

                        // Só busca por conciliações já concretizadas se não tiver sido requisitado um filtro do tipo PRE-CONCILIADO ou NÃO CONCILIADO
                        if (!filtroTipoPreConciliado && !filtroTipoNaoConciliado)
                        {
                            #region OBTÉM AS INFORMAÇÕES DE DADOS JÁ CONCILIADOS PREVIAMENTE

                            // Adiciona na cláusula where IDEXTRATO IS NOT NULL
                            SimpleDataBaseQuery queryRbConciliados = new SimpleDataBaseQuery(dataBaseQueryRB);
                            queryRbConciliados.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda IS NOT NULL");

                            List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryRbConciliados.Script(), connection);

                            List<dynamic> recebimentosConciliados = new List<dynamic>();

                            if (resultado != null && resultado.Count > 0)
                            {
                                recebimentosConciliados = resultado.Select(r => new
                                                            {
                                                                Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                Id = Convert.ToInt32(r["idRecebimento"]),
                                                                IdRecebimentoVenda = Convert.ToInt32(r["idRecebimentoVenda"]),
                                                                Nsu = Convert.ToString(r["nsu"]),
                                                                CodResumoVendas = r["codResumoVenda"].Equals(DBNull.Value) ? "" : Convert.ToString(r["codResumoVenda"]),
                                                                Bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                                                Data = (DateTime)r["dtaVenda"],
                                                                Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                Valor = Convert.ToDecimal(r["valorVendaBruta"]),
                                                                Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                                                Parcelas = Convert.ToInt32(r["numParcelaTotal"].Equals(DBNull.Value) ? 0 : r["numParcelaTotal"]),
                                                            }).ToList<dynamic>();
                            }

                            totalConciliados = recebimentosConciliados.Count;

                            // Total Conciliados
                            retorno.TotalDeRegistros = totalConciliados;

                            // PAGINAÇÃO
                            if (totalConciliados > 0 && pageNumber > 0 && pageSize > 0 && (skipRows >= totalConciliados || totalConciliados > pageSize))
                            {
                                if (skipRows >= totalConciliados)
                                    recebimentosConciliados = new List<dynamic>();//recebimentosConciliados.Skip(totalConciliados).Take(0); // pega nenhum
                                else
                                {
                                    int take = skipRows + pageSize >= totalConciliados ? totalConciliados - skipRows : pageSize;
                                    recebimentosConciliados = recebimentosConciliados.Skip(skipRows).Take(take).ToList<dynamic>();
                                }
                            }
                            else if (filtroTipoConciliado)
                                pageNumber = 1;


                            // Adiciona como conciliados
                            for (int k = 0; k < recebimentosConciliados.Count && (pageSize == 0 || CollectionConciliacaoVendas.Count < pageSize); k++)
                            {
                                var receb = recebimentosConciliados[k];
                                // Recebimento
                                Int32 idRecebimentoVenda = Convert.ToInt32(receb.IdRecebimentoVenda);

                                ConciliacaoVendas recebimento = new ConciliacaoVendas
                                                                {
                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                    Id = receb.Id,
                                                                    Nsu = receb.Nsu,
                                                                    CodResumoVendas = receb.CodResumoVendas,
                                                                    Bandeira = receb.Bandeira,
                                                                    Data = receb.Data,
                                                                    Filial = receb.Filial,
                                                                    Valor = receb.Valor,
                                                                    Adquirente = receb.Adquirente,
                                                                    Parcelas = receb.Parcelas,
                                                                };

                                SimpleDataBaseQuery queryVDConciliado = new SimpleDataBaseQuery(dataBaseQueryVD);
                                queryVDConciliado.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda = " + idRecebimentoVenda);

                                resultado = DataBaseQueries.SqlQuery(queryVDConciliado.Script(), connection);

                                ConciliacaoVendas venda = null;

                                if (resultado != null && resultado.Count > 0)
                                {
                                    venda = resultado.Select(r => new ConciliacaoVendas
                                                                {
                                                                    Tipo = TIPO_VENDA,
                                                                    Id = Convert.ToInt32(r["idRecebimentoVenda"]),
                                                                    Nsu = Convert.ToString(r["nrNSU"]),
                                                                    Bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]),
                                                                    Data = (DateTime)r["dtVenda"],
                                                                    Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                    Valor = Convert.ToDecimal(r["vlVenda"]),
                                                                    Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                                                    Parcelas = Convert.ToInt32(r["qtParcelas"]),
                                                                }).FirstOrDefault();
                                }

                                if (venda == null)
                                    continue; // falha!

                                // Adiciona
                                adicionaElementosConciliadosNaLista(CollectionConciliacaoVendas, recebimento, venda, TIPO_CONCILIADO.CONCILIADO);
                            }
                            #endregion
                        }

                        // Só busca por possíveis conciliações se não tiver sido requisitado um filtro do tipo CONCILIADO
                        if (!filtroTipoConciliado)
                        {

                            // NÃO CONCILIADOS
                            // Adiciona na cláusula where IDEXTRATO IS NOT NULL
                            SimpleDataBaseQuery queryRbNaoConciliados = new SimpleDataBaseQuery(dataBaseQueryRB);
                            queryRbNaoConciliados.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda IS NULL");

                            List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryRbNaoConciliados.Script(), connection);

                            List<ConciliacaoVendas> recebimentosParcela = new List<ConciliacaoVendas>();

                            if (resultado != null && resultado.Count > 0)
                            {
                                recebimentosParcela = resultado.Select(r => new ConciliacaoVendas
                                {
                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                    Id = Convert.ToInt32(r["idRecebimento"]),
                                    Nsu = Convert.ToString(r["nsu"]),
                                    CodResumoVendas = r["codResumoVenda"].Equals(DBNull.Value) ? "" : Convert.ToString(r["codResumoVenda"]),
                                    Bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                    Data = (DateTime)r["dtaVenda"],
                                    Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                    Valor = Convert.ToDecimal(r["valorVendaBruta"]),
                                    Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                    Parcelas = Convert.ToInt32(r["numParcelaTotal"].Equals(DBNull.Value) ? 0 : r["numParcelaTotal"]),
                                }).ToList<ConciliacaoVendas>();
                            }

                            int totalNaoConciliados = recebimentosParcela.Count;

                            retorno.TotalDeRegistros += totalNaoConciliados;

                            if (pageSize == 0 || CollectionConciliacaoVendas.Count < pageSize)
                            {
                                #region OBTÉM AS INFORMAÇÕES DE DADOS NÃO-CONCILIADOS E BUSCA PRÉ-CONCILIAÇÕES

                                #region OBTÉM SOMENTE OS RECEBIMENTOS PARCELAS NÃO-CONCILIADOS

                                int skipRowsNaoConciliados = 0;

                                #region PAGINA DIRETO PELA BASE SE NÃO FOR PELOS PRÉ-CONCILIADOS
                                if (!filtroTipoPreConciliado)
                                {
                                    int take = 0;

                                    if (skipRows > totalConciliados)
                                    {
                                        skipRowsNaoConciliados = skipRows - totalConciliados;
                                        if (skipRowsNaoConciliados >= totalNaoConciliados)
                                            // pega nenhum
                                            skipRowsNaoConciliados = totalNaoConciliados;
                                        else
                                        {
                                            if (totalNaoConciliados - skipRowsNaoConciliados >= pageSize) take = pageSize;
                                            else take = totalNaoConciliados - skipRowsNaoConciliados;
                                        }
                                    }
                                    else
                                    {
                                        take = pageSize - (totalConciliados - skipRows);
                                        if (take > totalNaoConciliados) take = totalNaoConciliados;
                                    }

                                    // PAGINAÇÃO
                                    if (pageNumber > 0 && pageSize > 0)
                                        recebimentosParcela = recebimentosParcela.Skip(skipRowsNaoConciliados).Take(take).ToList<ConciliacaoVendas>();
                                    else if (!filtroTipoNaoConciliado)
                                        pageNumber = 1;
                                }
                                #endregion

                                #endregion

                                // Somente os não conciliados
                                if (!dataBaseQueryVD.join.ContainsKey("LEFT JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                                    dataBaseQueryVD.join.Add("LEFT JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda = " + GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda");
                                dataBaseQueryVD.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda IS NULL");

                                // Vendas                            
                                if (!preConciliaComGrupo && !CnpjEmpresa.Equals(""))
                                    dataBaseQueryVD.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrCNPJ = '" + CnpjEmpresa + "'");
                                //queryTbRecebimentoVenda = queryTbRecebimentoVenda.Where(e => e.nrCNPJ.Equals(CnpjEmpresa));


                                List<int> idsPreConciliados = new List<int>();
                                /*string filialConsulta = String.Empty;
                                if(!CnpjEmpresa.Equals("")){ 
                                    filialConsulta = _db.empresas.Where(e => e.nu_cnpj.Equals(CnpjEmpresa)).Select(e =>  e.ds_fantasia + (e.filial != null ? " " + e.filial : "")).FirstOrDefault();
                                    if(filialConsulta == null) filialConsulta = String.Empty;
                                }*/

                                int contSkips = 0;
                                for (int k = 0; k < recebimentosParcela.Count && (pageSize == 0 || CollectionConciliacaoVendas.Count < pageSize); k++)
                                {
                                    ConciliacaoVendas receb = recebimentosParcela[k];

                                    List<ConciliacaoVendas> vendas = new List<ConciliacaoVendas>();

                                    // SE FOR ENVIADO O FILTRO DE "NÃO CONCILIADO", EXIBE TODOS OS QUE NÃO TIVEREM TÍTULOS ASSOCIADOS, INDEPENDENTEMENTE SE PUDER SER PRÉ-CONCILIADO
                                    if (!filtroTipoNaoConciliado)
                                    {
                                        //string filialConsulta = receb.Filial;

                                        DateTime dataIni = receb.Data.AddDays(RANGE_DIAS_ANTERIOR * -1);
                                        DateTime dataFim = receb.Data.AddDays(RANGE_DIAS_POSTERIOR);
                                        string nsu = "" + Convert.ToInt32(receb.Nsu);

                                        SimpleDataBaseQuery queryVDNaoConciliado = new SimpleDataBaseQuery(dataBaseQueryVD);

                                        // WHERE
                                        queryVDNaoConciliado.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".dtVenda BETWEEN '" + DataBaseQueries.GetDate(dataIni) + "' AND '" + DataBaseQueries.GetDate(dataFim) + " 23:59:00'");
                                        queryVDNaoConciliado.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrNSU LIKE '%" + nsu + "'");
                                        if (idsPreConciliados.Count > 0)
                                            queryVDNaoConciliado.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda NOT IN (" + string.Join(", ", idsPreConciliados) + ")");

                                        // Para cada recebimento Parcela, procurar
                                        resultado = DataBaseQueries.SqlQuery(queryVDNaoConciliado.Script(), connection);
                                        vendas = new List<ConciliacaoVendas>();
                                        if (resultado != null && resultado.Count > 0)
                                        {
                                            vendas = resultado.Select(r => new ConciliacaoVendas
                                                                {
                                                                    Tipo = TIPO_VENDA,
                                                                    Id = Convert.ToInt32(r["idRecebimentoVenda"]),
                                                                    Nsu = Convert.ToString(r["nrNSU"]),
                                                                    Bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]),
                                                                    Data = (DateTime)r["dtVenda"],
                                                                    Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                    Valor = Convert.ToDecimal(r["vlVenda"]),
                                                                    Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                                                    Parcelas = Convert.ToInt32(r["qtParcelas"])
                                                                })
                                                               .ToList<ConciliacaoVendas>();
                                        }
                                    }

                                    if (vendas.Count > 0)
                                    {
                                        ConciliacaoVendas vdPreConciliado = null;

                                        // Mesma filial da parcela
                                        List<ConciliacaoVendas> vendFilial = vendas.Where(e => e.Filial.Equals(receb.Filial)).ToList<ConciliacaoVendas>();
                                        if (vendFilial.Count > 0)
                                        {
                                            // Tem pra mesma filial
                                            foreach (ConciliacaoVendas vd in vendFilial)
                                            {
                                                // NSUs com mesmo comprimento
                                                string nsu1 = vd.Nsu;
                                                string nsu2 = receb.Nsu;
                                                while (nsu1.Length < nsu2.Length) nsu1 = "0" + nsu1;
                                                while (nsu2.Length < nsu1.Length) nsu2 = "0" + nsu2;

                                                if (nsu1.Equals(nsu2) && Math.Abs(vd.Valor - receb.Valor) <= TOLERANCIA)
                                                {
                                                    vdPreConciliado = vd;
                                                    break;
                                                }
                                            }
                                            // Se não achou, pega somente os que não são da mesma filial
                                            if (vdPreConciliado == null)
                                                vendas = vendas.Where(e => !e.Filial.Equals(receb.Filial)).ToList<ConciliacaoVendas>();
                                        }

                                        if (vdPreConciliado == null)
                                        {
                                            // Não achou na mesma filial => Busca em outra filial do mesmo grupo
                                            foreach (ConciliacaoVendas vd in vendas)
                                            {
                                                // NSUs com mesmo comprimento
                                                string nsu1 = vd.Nsu;
                                                string nsu2 = receb.Nsu;
                                                while (nsu1.Length < nsu2.Length) nsu1 = "0" + nsu1;
                                                while (nsu2.Length < nsu1.Length) nsu2 = "0" + nsu2;

                                                if (nsu1.Equals(nsu2) && Math.Abs(vd.Valor - receb.Valor) <= TOLERANCIA)
                                                {
                                                    vdPreConciliado = vd;
                                                    break;
                                                }
                                            }
                                        }

                                        if (vdPreConciliado != null)
                                        {
                                            // Pré-conciliado
                                            idsPreConciliados.Add(vdPreConciliado.Id);
                                            if (!filtroTipoNaoConciliado)
                                            {
                                                if (!filtroTipoPreConciliado || contSkips >= skipRows)
                                                    adicionaElementosConciliadosNaLista(CollectionConciliacaoVendas, receb, vdPreConciliado, TIPO_CONCILIADO.PRE_CONCILIADO);
                                                //if (filtroTipoPreConciliado) retorno.TotalDeRegistros++;
                                                contSkips++;
                                            }
                                        }
                                        else
                                        {
                                            // Não conciliado
                                            List<ConciliacaoVendas> rps = new List<ConciliacaoVendas>();
                                            rps.Add(receb);
                                            if (!filtroTipoPreConciliado)
                                            {
                                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoVendas, rps);
                                                //if (filtroTipoNaoConciliado) retorno.TotalDeRegistros++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Não encontrado!
                                        List<ConciliacaoVendas> rps = new List<ConciliacaoVendas>();
                                        rps.Add(receb);
                                        if (!filtroTipoPreConciliado)
                                        {
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoVendas, rps);
                                            //if (filtroTipoNaoConciliado) retorno.TotalDeRegistros++;
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is DbEntityValidationException)
                        {
                            string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                            throw new Exception(erro.Equals("") ? "Falha ao listar recebimento" : erro);
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

                    // Ordena
                    CollectionConciliacaoVendas = CollectionConciliacaoVendas
                                                                    .OrderBy(c => c.Recebimento.Data)
                                                                    .ThenBy(c => c.Recebimento.Valor)
                                                                    .ThenBy(c => c.Adquirente)
                                                                    .ThenBy(c => c.Recebimento.Bandeira)
                                                                    .ToList<dynamic>();

                    // TOTAL
                    retorno.Totais = new Dictionary<string, object>();
                    retorno.Totais.Add("valor", CollectionConciliacaoVendas.Select(r => r.Recebimento.Valor).Cast<decimal>().Sum());

                    #endregion
                }
                else if (colecao == 1)
                {
                    #region BUSCA VENDAS
                    //if (!queryString.TryGetValue("" + (int)CAMPOS.IDRECEBIMENTO, out outValue))
                    //    throw new Exception("Para consultar vendas, deve ser enviado dados da parcela!");

                    //SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

                    //try
                    //{
                    //    connection.Open();
                    //}
                    //catch
                    //{
                    //    throw new Exception("Não foi possível estabelecer conexão com a base de dados");
                    //}

                    //try
                    //{


                    //    Int32 idRecebimento = Convert.ToInt32(queryString["" + (int)CAMPOS.IDRECEBIMENTO]);
                       
                    //    //Recebimento recebimento = _db.Recebimentos.Where(e => e.idRecebimento == idRecebimento)
                    //    //                                          .FirstOrDefault();
                    //    List<IDataRecord> resultado = DataBaseQueries.SqlQuery("SELECT R.cnpj, B.cdAdquirente, R.valorVendaBruta, R.dtaVenda" +
                    //                                         " FROM pos.Recebimento P (NOLOCK)" +
                    //                                         " JOIN card.tbBandeira B ON R.cdBandeira = B.cdBandeira" +
                    //                                         " WHERE R.id = " + idRecebimento, connection);
                    //    if (resultado == null || resultado.Count == 0)
                    //    {
                    //        try
                    //        {
                    //            connection.Close();
                    //        }
                    //        catch { }
                    //        throw new Exception("Recebimento inválido!");
                    //    }
                    //    var recebimento = resultado.Select(r => new
                    //                     {
                    //                         cnpj = Convert.ToString(r["cnpj"]),
                    //                         cdAdquirente = Convert.ToInt32(r["cdAdquirente"]),
                    //                         valorVendaBruta = Convert.ToDecimal(r["valorVendaBruta"]),
                    //                         dtaVenda = (DateTime)r["dtaVenda"],
                    //                     }).FirstOrDefault();

                    //    // Pode ter enviado de uma filial diferente
                    //    string nrCNPJ = recebimento.cnpj;//recebimento.Recebimento.cnpj;
                    //    if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    //        nrCNPJ = queryString["" + (int)CAMPOS.NU_CNPJ];

                    //    int cdAdquirente = recebimento.cdAdquirente; // recebimento.Recebimento.tbBandeira.cdAdquirente
                    //    decimal valorVendaBruta = recebimento.valorVendaBruta;

                    //    DateTime dtaVenda = recebimento.dtaVenda;
                    //    DateTime data = Convert.ToDateTime(dtaVenda.ToShortDateString());
                    //    DateTime dataIni = data.AddDays(RANGE_DIAS_ANTERIOR * -1);
                    //    DateTime dataFim = data.AddDays(RANGE_DIAS_POSTERIOR);

                    //    // Consulta vendas com a mesma data da venda
                    //    string script = "SELECT T.idRecebimentoVenda, T.nrParcela, T.nrNSU, T.dsBandeira" +
                    //                    ", T.dtVenda, T.dtVenda, E.ds_fantasia, E.filial, A.nmAdquirente" +
                    //                    ", T.vlParcela, T.vlVenda, diferencaValorVenda = ABS(T.vlVenda - " + valorVendaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ")" +
                    //                    ", diferencaValorParcela = ABS(T.vlParcela - " + valorParcelaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ")" +
                    //                    ", diferencaDtVenda = ABS(DATEDIFF(DAY, T.dtVenda, '" + DataBaseQueries.GetDate(dtaVenda) + "'))" +
                    //                    " FROM card.tbRecebimentoVenda T (NOLOCK)" +
                    //                    " JOIN cliente.empresa E ON E.nu_cnpj = T.nrCNPJ" +
                    //                    " JOIN card.tbAdquirente A ON A.cdAdquirente = T.cdAdquirente" +
                    //                    " LEFT JOIN pos.Recebimento P ON P.idRecebimentoVenda = T.idRecebimentoVenda" +
                    //                    " WHERE P.idRecebimentoVenda IS NULL" +
                    //                    " AND T.dtVenda BETWEEN '" + DataBaseQueries.GetDate(dataIni) + "' AND '" + DataBaseQueries.GetDate(dataFim) + " 23:59:00'" +
                    //                    " AND T.nrCNPJ = '" + nrCNPJ + "'" +
                    //                    " AND T.cdAdquirente = " + cdAdquirente +
                    //                    " AND ABS(T.vlVenda - " + valorVendaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ") <= " + TOLERANCIA.ToString(CultureInfo.GetCultureInfo("en-GB"))
                    //                    ;
                    //    resultado = DataBaseQueries.SqlQuery(script, connection);
                    //    List<dynamic> vendas = new List<dynamic>();
                    //    if (resultado != null && resultado.Count > 0)
                    //    {
                    //        vendas = resultado.Select(r => new
                    //                            {
                    //                                idRecebimentoVenda = Convert.ToInt32(r["idRecebimentoVenda"]),
                    //                                nrParcela = Convert.ToInt32(r["nrParcela"]),
                    //                                nrNSU = Convert.ToString(r["nrNSU"]),
                    //                                bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]).ToUpper(),
                    //                                dtVenda = r["dtVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)r["dtVenda"],
                    //                                dtVenda = (DateTime)r["dtVenda"],
                    //                                empresa = Convert.ToString(r["ds_fantasia"].ToString() + (r["filial"].Equals(DBNull.Value) ? "" : " " + r["filial"].ToString())).ToUpper(),
                    //                                vlParcela = Convert.ToDecimal(r["vlParcela"]),
                    //                                vlVenda = Convert.ToDecimal(r["vlVenda"].Equals(DBNull.Value) ? 0.0 : r["vlVenda"]),
                    //                                tbAdquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                    //                                // Malandragens
                    //                                diferencaValorVenda = Convert.ToDecimal(r["diferencaValorVenda"]),
                    //                                diferencaValorParcela = Convert.ToDecimal(r["diferencaValorParcela"]),
                    //                                diferencaDtVenda = Convert.ToInt32(r["diferencaDtVenda"]),
                    //                            })
                    //                            .OrderBy(e => e.diferencaDtVenda)
                    //                            .ThenBy(e => e.diferencaValorVenda)
                    //                            .ThenBy(e => e.diferencaValorParcela)
                    //                            .ThenByDescending(e => e.nrNSU.StartsWith("T"))
                    //                            .ToList<dynamic>();
                    //    }
                    //    //List<dynamic> vendas = _db.tbRecebimentoVendas.Where(e => e.dtVenda != null && /*(e.dtVenda.Value.Year == recebimento.Recebimento.dtaVenda.Year &&
                    //    //                                                                                              e.dtVenda.Value.Month == recebimento.Recebimento.dtaVenda.Month &&
                    //    //                                                                                              e.dtVenda.Value.Day == recebimento.Recebimento.dtaVenda.Day))*/
                    //    //                                                                        e.dtVenda.Value >= dataIni && e.dtVenda.Value <= dataFim)
                    //    //                                                            .Where(e => e.nrCNPJ.Equals(nrCNPJ))
                    //    //                                                            .Where(e => e.cdAdquirente == cdAdquirente)
                    //    //                                                            .Where(e => (e.vlVenda >= valorVendaBruta && (e.vlVenda - valorVendaBruta <= TOLERANCIA)) ||
                    //    //                                                                        (e.vlVenda < valorVendaBruta && (valorVendaBruta - e.vlVenda <= TOLERANCIA)))
                    //    //                                                            .Where(e => e.Recebimentos.Count == 0)
                    //    //                                                            //.Where(e => (e.vlParcela >= recebimento.valorParcelaLiquida && (e.vlParcela - recebimento.valorParcelaLiquida <= TOLERANCIA)) ||
                    //    //                                                            //            (e.vlParcela < recebimento.valorParcelaLiquida && (recebimento.valorParcelaLiquida - e.vlParcela <= TOLERANCIA)))
                    //    //                                                            .Select(e => new
                    //    //                                                            {
                    //    //                                                                e.idRecebimentoVenda,
                    //    //                                                                e.nrParcela,
                    //    //                                                                e.nrNSU,
                    //    //                                                                bandeira = e.dsBandeira.ToUpper(),
                    //    //                                                                e.dtVenda,
                    //    //                                                                e.dtVenda,
                    //    //                                                                empresa = e.empresa.ds_fantasia + (e.empresa.filial != null ? " " + e.empresa.filial : ""),
                    //    //                                                                e.vlParcela,
                    //    //                                                                vlVenda = e.vlVenda != null ? e.vlVenda : new decimal(0.0),
                    //    //                                                                tbAdquirente = e.tbAdquirente.nmAdquirente.ToUpper(),
                    //    //                                                                // Malandragens
                    //    //                                                                diferencaValorVenda = e.vlVenda >= valorVendaBruta ? e.vlVenda - valorVendaBruta : valorVendaBruta - e.vlVenda,
                    //    //                                                                diferencaValorParcela = e.vlParcela >= valorParcelaBruta ? e.vlParcela - valorParcelaBruta : valorParcelaBruta - e.vlParcela,
                    //    //                                                                diferencaDtVenda = e.dtVenda < dtaVenda ? DbFunctions.DiffDays(e.dtVenda, dtaVenda) : DbFunctions.DiffDays(dtaVenda, e.dtVenda),
                    //    //                                                            })
                    //    //                                                            .OrderBy(e => e.diferencaDtVenda)
                    //    //                                                            .ThenBy(e => e.diferencaValorVenda)
                    //    //                                                            .ThenBy(e => e.diferencaValorParcela)
                    //    //                                                            .ThenByDescending(e => e.nrNSU.StartsWith("T"))
                    //    //                                                            .ToList<dynamic>();

                    //    // Mesma parcela
                    //    List<dynamic> vendasParcela;
                    //    if (numParcela2 != numParcela)
                    //        vendasParcela = vendas.Where(e => e.nrParcela == numParcela || e.nrParcela == numParcela2).ToList<dynamic>();
                    //    else
                    //        vendasParcela = vendas.Where(e => e.nrParcela == numParcela).ToList<dynamic>();
                    //    if (vendasParcela.Count > 0)
                    //    {
                    //        // Envia somente eles
                    //        foreach (var t in vendasParcela)
                    //        {
                    //            CollectionConciliacaoVendas.Add(new ConciliacaoVendas
                    //                                    {
                    //                                        Tipo = TIPO_VENDA, // venda
                    //                                        Id = t.idRecebimentoVenda,
                    //                                        NumParcela = t.nrParcela,
                    //                                        Nsu = t.nrNSU,
                    //                                        //CodResumoVendas = null,
                    //                                        Bandeira = t.bandeira,
                    //                                        DataVenda = t.dtVenda,
                    //                                        Data = t.dtVenda,
                    //                                        Filial = t.empresa,
                    //                                        Valor = t.vlParcela,
                    //                                        ValorVenda = t.vlVenda,
                    //                                        Adquirente = t.tbAdquirente,
                    //                                    });
                    //        }
                    //    }
                    //    else
                    //    {
                    //        // Envia todos
                    //        foreach (var t in vendas)
                    //        {
                    //            CollectionConciliacaoVendas.Add(new ConciliacaoVendas
                    //            {
                    //                Tipo = TIPO_VENDA, // venda
                    //                Id = t.idRecebimentoVenda,
                    //                NumParcela = t.nrParcela,
                    //                Nsu = t.nrNSU,
                    //                //CodResumoVendas = null,
                    //                Bandeira = t.bandeira,
                    //                DataVenda = t.dtVenda,
                    //                Data = t.dtVenda,
                    //                Filial = t.empresa,
                    //                Valor = t.vlParcela,
                    //                ValorVenda = t.vlVenda,
                    //                Adquirente = t.tbAdquirente,
                    //            });
                    //        }
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    if (e is DbEntityValidationException)
                    //    {
                    //        string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    //        throw new Exception(erro.Equals("") ? "Falha ao listar recebimento parcela" : erro);
                    //    }
                    //    throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
                    //}
                    //finally
                    //{
                    //    try
                    //    {
                    //        connection.Close();
                    //    }
                    //    catch { }
                    //}

                    //retorno.TotalDeRegistros = CollectionConciliacaoVendas.Count;

                    #endregion
                }



                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionConciliacaoVendas;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao exibir as parcelas conciliadas" : erro);
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
        /// Aponta Recebimento para TbRecebimentoVenda
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, List<ConciliaRecebimentoVenda> param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction(); // tudo ou nada
            try
            {
                foreach (ConciliaRecebimentoVenda conciliaVenda in param)
                {
                    // Avalia o venda
                    tbRecebimentoVenda tbRecebimentoVenda = null;
                    if (conciliaVenda.idRecebimentoVenda > 0)
                    {
                        tbRecebimentoVenda = _db.Database.SqlQuery<tbRecebimentoVenda>("SELECT T.*" +
                                                                                       " FROM card.tbRecebimentoVenda T (NOLOCK)" +
                                                                                       " WHERE T.idRecebimentoVenda = " + conciliaVenda.idRecebimentoVenda
                                                                                       )
                                                         .FirstOrDefault();
                        if (tbRecebimentoVenda == null)
                            continue; // venda inválida!

                        // Desconcilia recebíveis que estavam apontando para a venda
                        _db.Database.ExecuteSqlCommand("UPDATE R" +
                                                        " SET R.idRecebimentoVenda = NULL" +
                                                        " FROM pos.Recebimento R" +
                                                        " WHERE R.idRecebimentoVenda = " + conciliaVenda.idRecebimentoVenda
                                                        );
                        _db.SaveChanges();

                        // Concilia parcela
                        _db.Database.ExecuteSqlCommand("UPDATE R" +
                                                       " SET R.idRecebimentoVenda = " + conciliaVenda.idRecebimentoVenda +
                                                       " FROM pos.Recebimento R" +
                                                       " WHERE R.id = " + conciliaVenda.idRecebimento
                                                       );
                        _db.SaveChanges();
                    }
                    else
                    {
                        // Desconcilia
                        _db.Database.ExecuteSqlCommand("UPDATE R" +
                                                        " SET R.idRecebimentoVenda = NULL" +
                                                        " FROM pos.Recebimento R" +
                                                        " WHERE R.id = " + conciliaVenda.idRecebimento
                                                       );
                        _db.SaveChanges();
                    }
                }
                // Commit
                transaction.Commit();

            }
            catch (Exception e)
            {
                // Rollback
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a conciliação de vendas" : erro);
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
        /// Usa a SP para realizar a conciliação de vendas TEF
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void ConciliaTefNsu(string token, ConciliaTefNsu param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                if (param == null || param.data == null || param.nrCNPJ == null || param.cdAdquirente == 0)
                    throw new Exception("Parâmetro inválido!");

                List<string> datas = new List<string>();

                if(param.data.Contains("|"))
                {
                    string[] dts = param.data.Split('|');
                    DateTime dtIni = DateTime.ParseExact(dts[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    DateTime dtFim = DateTime.ParseExact(dts[1] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    if (dtIni < dtFim)
                    {
                        for (DateTime dt = dtIni; dt <= dtFim; dt = dt.AddDays(1))
                            datas.Add(dt.Year + (dt.Month < 10 ? "0" : "") + dt.Month + (dt.Day < 10 ? "0" : "") + dt.Day);
                    }
                    else
                        datas.Add(dtIni.Year + (dtIni.Month < 10 ? "0" : "") + dtIni.Month + (dtIni.Day < 10 ? "0" : "") + dtIni.Day);
                }
                else
                    datas.Add(param.data);

                //foreach (string data in datas)
                //{
                //    //_db.Database.SqlQuery<string>("EXECUTE [card].[sp_upd_ConciliaVendas] '" + 
                //    //                               param.nrCNPJ  + "', '" + data + "', " + param.cdAdquirente);
                //    _db.Database.ExecuteSqlCommand("EXECUTE [card].[sp_upd_ConciliaVendas] '" + 
                //                                    param.nrCNPJ  + "', '" + data + "', " + param.cdAdquirente);
                //}

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a conciliação de vendas TEF" : erro);
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