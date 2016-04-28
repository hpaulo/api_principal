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

            //IDRECEBIMENTO = 300
            IDRECEBIMENTOVENDA = 300
        };

        public enum TIPO_CONCILIADO
        {
            CONCILIADO = 1,
            PRE_CONCILIADO = 2,
            NAO_CONCILIADO = 3,
            CONCILIADO_DIVERGENTE = 4,
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
                        Sacado = item.Sacado,
                        Bandeira = item.Bandeira,
                        Data = item.Data,
                        Filial = item.Filial.Trim(),
                        Parcelas = item.Parcelas,
                        Adquirente = item.Adquirente,
                    },
                    Recebimento = item.Tipo != TIPO_RECEBIMENTO ? null : new
                    {
                        Id = item.Id,
                        Nsu = item.Nsu,
                        CodResumoVendas = item.CodResumoVendas,
                        Valor = item.Valor,
                        Sacado = item.Sacado,
                        Bandeira = item.Bandeira,
                        Data = item.Data,
                        Filial = item.Filial.Trim(),
                        Parcelas = item.Parcelas,
                        Adquirente = item.Adquirente,
                    },        
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
                                                     TIPO_CONCILIADO tipo, bool filtroTipoConciliadoDivergente = false)
        {
            if (recebimento != null && venda != null)
            {
                if (tipo.Equals(TIPO_CONCILIADO.CONCILIADO) && ConciliacaoVendas.possuiDivergenciasNaVenda(recebimento, venda))
                {
                    tipo = TIPO_CONCILIADO.CONCILIADO_DIVERGENTE;
                }
                else if (filtroTipoConciliadoDivergente)
                    return;

                // Adiciona
                listaConciliacao.Add(new
                {
                    Conciliado = (int)tipo,
                    Venda = new
                    {
                        Id = venda.Id,
                        Nsu = venda.Nsu,
                        Valor = venda.Valor,
                        Sacado = venda.Sacado,
                        Bandeira = venda.Bandeira,
                        Data = venda.Data,
                        Filial = venda.Filial.Trim(),
                        Parcelas = venda.Parcelas,
                        Adquirente = venda.Adquirente,
                    },
                    Recebimento = new
                    {
                        Id = recebimento.Id,
                        Nsu = recebimento.Nsu,
                        CodResumoVendas = recebimento.CodResumoVendas,
                        Valor = recebimento.Valor,
                        Sacado = recebimento.Sacado,
                        Bandeira = recebimento.Bandeira,
                        Data = recebimento.Data,
                        Filial = recebimento.Filial.Trim(),
                        Parcelas = recebimento.Parcelas,
                        Adquirente = recebimento.Adquirente,
                    },
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
                        //queryStringRecebimento.Add("" + (int)GatewayRecebimento.CAMPOS.DTAVENDA, data);
                        queryStringTbRecebimentoVenda.Add("" + (int)GatewayTbRecebimentoVenda.CAMPOS.DTVENDA, data);
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
                        //queryStringRecebimento.Add("" + (int)GatewayRecebimento.CAMPOS.CNPJ, CnpjEmpresa);
                        queryStringTbRecebimentoVenda.Add("" + (int)GatewayTbRecebimentoVenda.CAMPOS.NRCNPJ, CnpjEmpresa);
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
                        //queryStringRecebimento.Add("" + (int)GatewayRecebimento.CAMPOS.NSU, queryString["" + (int)CAMPOS.NSU]);
                        queryStringTbRecebimentoVenda.Add("" + (int)GatewayTbRecebimentoVenda.CAMPOS.NRNSU, queryString["" + (int)CAMPOS.NSU]);
                    }

                    // FILTRO DE TIPO ?
                    bool filtroTipoConciliado = false;
                    bool filtroTipoPreConciliado = false;
                    bool filtroTipoNaoConciliado = false;
                    bool filtroTipoConciliadoDivergente = false;
                    if (queryString.TryGetValue("" + (int)CAMPOS.TIPO, out outValue))
                    {
                        TIPO_CONCILIADO tipo = (TIPO_CONCILIADO)Convert.ToInt32(queryString["" + (int)CAMPOS.TIPO]);
                        if (tipo.Equals(TIPO_CONCILIADO.CONCILIADO)) filtroTipoConciliado = true;
                        else if (tipo.Equals(TIPO_CONCILIADO.PRE_CONCILIADO)) filtroTipoPreConciliado = true;
                        else if (tipo.Equals(TIPO_CONCILIADO.NAO_CONCILIADO)) filtroTipoNaoConciliado = true;
                        else if (tipo.Equals(TIPO_CONCILIADO.CONCILIADO_DIVERGENTE)) filtroTipoConciliadoDivergente = true;
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
                        if (!dataBaseQueryRB.join.ContainsKey("LEFT JOIN card.tbBandeiraSacado " + GatewayTbBandeiraSacado.SIGLA_QUERY))
                            dataBaseQueryRB.join.Add("LEFT JOIN card.tbBandeiraSacado " + GatewayTbBandeiraSacado.SIGLA_QUERY, " ON " + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira AND " +  GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdGrupo = " + GatewayEmpresa.SIGLA_QUERY + ".id_grupo");

                        dataBaseQueryRB.select = new string[] { GatewayRecebimento.SIGLA_QUERY + ".id as idRecebimento",
                                                          GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda",
                                                          GatewayRecebimento.SIGLA_QUERY + ".nsu",
                                                          GatewayRecebimento.SIGLA_QUERY + ".codResumoVenda",
                                                          //GatewayTbBandeira.SIGLA_QUERY + ".cdSacado",
                                                          GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdSacado",
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
                        //if (!dataBaseQueryVD.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        //    dataBaseQueryVD.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".cdAdquirente");
                        if (!dataBaseQueryVD.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            dataBaseQueryVD.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrCNPJ = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");
                        if (!dataBaseQueryVD.join.ContainsKey("LEFT JOIN card.tbBandeiraSacado " + GatewayTbBandeiraSacado.SIGLA_QUERY))
                            dataBaseQueryVD.join.Add("LEFT JOIN card.tbBandeiraSacado " + GatewayTbBandeiraSacado.SIGLA_QUERY, " ON " + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdSacado = " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".cdSacado AND " + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdGrupo = " + GatewayEmpresa.SIGLA_QUERY + ".id_grupo");
                        if (!dataBaseQueryVD.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                            dataBaseQueryVD.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdBandeira");
                        if (!dataBaseQueryVD.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                            dataBaseQueryVD.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                        

                        dataBaseQueryVD.select = new string[] { GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda",
                                                          GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrNSU",
                                                          GatewayTbRecebimentoVenda.SIGLA_QUERY + ".cdSacado",
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

                            // Adiciona na cláusula where IDRECEBIMENTOVENDA IS NOT NULL
                            SimpleDataBaseQuery queryVdConciliados = new SimpleDataBaseQuery(dataBaseQueryVD);
                            if (!queryVdConciliados.join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                                queryVdConciliados.join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda = " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda");

                            //if (filtroTipoConciliadoDivergente)
                            //{
                            //    queryVdConciliados.AddWhereClause(" CONVERT(VARCHAR(10), " + GatewayRecebimento.SIGLA_QUERY + ".dtaVenda, 120) <> " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".dtVenda" +
                            //                                      " OR (" + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdSacado IS NOT NULL AND " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".cdSacado <> " + GatewayTbBandeiraSacado.SIGLA_QUERY + ".cdSacado)" +
                            //                                      " OR (" + GatewayRecebimento.SIGLA_QUERY + ".numParcelaTotal IS NOT NULL AND " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".qtParcelas <> " + GatewayRecebimento.SIGLA_QUERY + ".numParcelaTotal" +
                            //                                      " OR " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".vlVenda <> " + GatewayRecebimento.SIGLA_QUERY + ".valorVendaBruta" +
                            //                                      " OR SUBSTRING('000000000000' + CONVERT(VARCHAR(12), " + GatewayRecebimento.SIGLA_QUERY + ".nsu), LEN(" + GatewayRecebimento.SIGLA_QUERY + ".nsu) + 1, 12) <> SUBSTRING('000000000000' + CONVERT(VARCHAR(12), " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrNSU), LEN(" + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrNSU) + 1, 12)"
                            //                                      );
                            //}

                            //SimpleDataBaseQuery queryRbConciliados = new SimpleDataBaseQuery(dataBaseQueryRB);
                            //queryRbConciliados.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda IS NOT NULL");

                            //List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryRbConciliados.Script(), connection);

                            List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryVdConciliados.Script(), connection);

                            //List<dynamic> recebimentosConciliados = new List<dynamic>();
                            List<ConciliacaoVendas> vendasConciliadas = new List<ConciliacaoVendas>();

                            if (resultado != null && resultado.Count > 0)
                            {
                                //recebimentosConciliados = resultado.Select(r => new
                                //                            {
                                //                                Tipo = TIPO_RECEBIMENTO, // recebimento
                                //                                Id = Convert.ToInt32(r["idRecebimento"]),
                                //                                IdRecebimentoVenda = Convert.ToInt32(r["idRecebimentoVenda"]),
                                //                                Nsu = Convert.ToString(r["nsu"]),
                                //                                CodResumoVendas = r["codResumoVenda"].Equals(DBNull.Value) ? "" : Convert.ToString(r["codResumoVenda"]),
                                //                                Bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                //                                Data = (DateTime)r["dtaVenda"],
                                //                                Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                //                                Valor = Convert.ToDecimal(r["valorVendaBruta"]),
                                //                                Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                //                                Parcelas = Convert.ToInt32(r["numParcelaTotal"].Equals(DBNull.Value) ? 1 : r["numParcelaTotal"]),
                                //                            }).ToList<dynamic>();
                                vendasConciliadas = resultado.Select(r => new ConciliacaoVendas
                                                            {
                                                                Tipo = TIPO_VENDA, // venda
                                                                Id = Convert.ToInt32(r["idRecebimentoVenda"]),
                                                                Sacado = Convert.ToString(r["cdSacado"].Equals(DBNull.Value) ? "" : r["cdSacado"]),
                                                                Nsu = Convert.ToString(r["nrNSU"]),
                                                                Bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]),
                                                                Data = (DateTime)r["dtVenda"],
                                                                Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                Valor = Convert.ToDecimal(r["vlVenda"]),
                                                                Adquirente = Convert.ToString(r["nmAdquirente"].Equals(DBNull.Value) ? "" : r["nmAdquirente"]).ToUpper(),
                                                                Parcelas = Convert.ToInt32(r["qtParcelas"]),
                                                            }).ToList<ConciliacaoVendas>();
                            }

                            //totalConciliados = vendasConciliadas.Count;

                            // Total Conciliados
                            //retorno.TotalDeRegistros = totalConciliados;

                            // PAGINAÇÃO
                            if (totalConciliados > 0 && pageNumber > 0 && pageSize > 0 && (skipRows >= totalConciliados || totalConciliados > pageSize))
                            {
                                if (skipRows >= totalConciliados)
                                    vendasConciliadas = new List<ConciliacaoVendas>();//recebimentosConciliados.Skip(totalConciliados).Take(0); // pega nenhum
                                else
                                {
                                    int take = skipRows + pageSize >= totalConciliados ? totalConciliados - skipRows : pageSize;
                                    vendasConciliadas = vendasConciliadas.Skip(skipRows).Take(take).ToList();
                                }
                            }
                            else if (filtroTipoConciliado)
                                pageNumber = 1;

                            // Cláusula where
                            //string whereDtVenda = String.Empty;
                            //if (data.Contains("|")) // BETWEEN
                            //{
                            //    string[] busca = data.Split('|');
                            //    DateTime dtIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            //    DateTime dtFim = DateTime.ParseExact(busca[1] + " 23:59:59.999", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            //    whereDtVenda = GatewayRecebimento.SIGLA_QUERY + ".dtaVenda BETWEEN '" + DataBaseQueries.GetDate(dtIni) + "' AND '" + DataBaseQueries.GetDate(dtFim) + " 23:59:00'";
                            //}
                            //else
                            //{
                            //    DateTime dt = DateTime.ParseExact(data + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            //    whereDtVenda = GatewayRecebimento.SIGLA_QUERY + ".dtaVenda = '" + DataBaseQueries.GetDate(dt) + "'";
                            //}


                            // Adiciona como conciliados
                            for (int k = 0; k < vendasConciliadas.Count && (pageSize == 0 || CollectionConciliacaoVendas.Count < pageSize); k++)
                            {
                                ConciliacaoVendas venda = vendasConciliadas[k];
                                // Recebimento
                                Int32 idRecebimentoVenda = venda.Id;

                                SimpleDataBaseQuery queryRbConciliado = new SimpleDataBaseQuery(dataBaseQueryRB);
                                queryRbConciliado.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda = " + idRecebimentoVenda);
                                //queryRbConciliado.AddWhereClause(whereDtVenda);

                                resultado = DataBaseQueries.SqlQuery(queryRbConciliado.Script(), connection);

                                ConciliacaoVendas recebimento = null;

                                if (resultado != null && resultado.Count > 0)
                                {
                                    recebimento = resultado.Select(r => new ConciliacaoVendas
                                                                {
                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                    Id = Convert.ToInt32(r["idRecebimento"]),
                                                                    Sacado = Convert.ToString(r["cdSacado"].Equals(DBNull.Value) ? "" : r["cdSacado"]),
                                                                    Nsu = Convert.ToString(r["nsu"]),
                                                                    CodResumoVendas = r["codResumoVenda"].Equals(DBNull.Value) ? "" : Convert.ToString(r["codResumoVenda"]),
                                                                    Bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                                                    Data = (DateTime)r["dtaVenda"],
                                                                    Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                    Valor = Convert.ToDecimal(r["valorVendaBruta"]),
                                                                    Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                                                    Parcelas = Convert.ToInt32(r["numParcelaTotal"].Equals(DBNull.Value) ? 1 : r["numParcelaTotal"]),
                                                                }).FirstOrDefault();
                                }

                                if (recebimento == null)
                                    continue; // falha!


                                //ConciliacaoVendas recebimento = new ConciliacaoVendas
                                //                                {
                                //                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                //                                    Id = receb.Id,
                                //                                    Nsu = receb.Nsu,
                                //                                    CodResumoVendas = receb.CodResumoVendas,
                                //                                    Bandeira = receb.Bandeira,
                                //                                    Data = receb.Data,
                                //                                    Filial = receb.Filial,
                                //                                    Valor = receb.Valor,
                                //                                    Adquirente = receb.Adquirente,
                                //                                    Parcelas = receb.Parcelas,
                                //                                };

                                //SimpleDataBaseQuery queryVDConciliado = new SimpleDataBaseQuery(dataBaseQueryVD);
                                //queryVDConciliado.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda = " + idRecebimentoVenda);

                                //resultado = DataBaseQueries.SqlQuery(queryVDConciliado.Script(), connection);

                                //ConciliacaoVendas venda = null;

                                //if (resultado != null && resultado.Count > 0)
                                //{
                                //    venda = resultado.Select(r => new ConciliacaoVendas
                                //                                {
                                //                                    Tipo = TIPO_VENDA,
                                //                                    Id = Convert.ToInt32(r["idRecebimentoVenda"]),
                                //                                    Nsu = Convert.ToString(r["nrNSU"]),
                                //                                    Bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]),
                                //                                    Data = (DateTime)r["dtVenda"],
                                //                                    Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                //                                    Valor = Convert.ToDecimal(r["vlVenda"]),
                                //                                    Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                //                                    Parcelas = Convert.ToInt32(r["qtParcelas"]),
                                //                                }).FirstOrDefault();
                                //}

                                //if (venda == null)
                                //    continue; // falha!

                                // Adiciona
                                adicionaElementosConciliadosNaLista(CollectionConciliacaoVendas, recebimento, venda, TIPO_CONCILIADO.CONCILIADO, filtroTipoConciliadoDivergente);
                            }

                            // Total Conciliados
                            totalConciliados = CollectionConciliacaoVendas.Count;
                            retorno.TotalDeRegistros = totalConciliados;
                            #endregion
                        }

                        // Só busca por possíveis conciliações se não tiver sido requisitado um filtro do tipo CONCILIADO
                        if (!filtroTipoConciliado && !filtroTipoConciliadoDivergente)
                        {
                            // NÃO CONCILIADOS
                            // Adiciona na cláusula where IDRECEBIMENTOVENDA IS NULL
                            SimpleDataBaseQuery queryVdNaoConciliados = new SimpleDataBaseQuery(dataBaseQueryVD);
                            if (!queryVdNaoConciliados.join.ContainsKey("LEFT JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                                queryVdNaoConciliados.join.Add("LEFT JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda = " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda");
                            queryVdNaoConciliados.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda IS NULL");

                            //SimpleDataBaseQuery queryRbNaoConciliados = new SimpleDataBaseQuery(dataBaseQueryRB);
                            //queryRbNaoConciliados.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda IS NULL");

                            //List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryRbNaoConciliados.Script(), connection);
                            List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryVdNaoConciliados.Script(), connection);

                            List<ConciliacaoVendas> vendas = new List<ConciliacaoVendas>();
                            //List<ConciliacaoVendas> recebimentos = new List<ConciliacaoVendas>();

                            if (resultado != null && resultado.Count > 0)
                            {
                                vendas = resultado.Select(r => new ConciliacaoVendas
                                                            {
                                                                Tipo = TIPO_VENDA, // venda
                                                                Id = Convert.ToInt32(r["idRecebimentoVenda"]),
                                                                Sacado = Convert.ToString(r["cdSacado"].Equals(DBNull.Value) ? "" : r["cdSacado"]),
                                                                Nsu = Convert.ToString(r["nrNSU"]),
                                                                Bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]),
                                                                Data = (DateTime)r["dtVenda"],
                                                                Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                Valor = Convert.ToDecimal(r["vlVenda"]),
                                                                Adquirente = Convert.ToString(r["nmAdquirente"].Equals(DBNull.Value) ? "" : r["nmAdquirente"]).ToUpper(),
                                                                Parcelas = Convert.ToInt32(r["qtParcelas"]),
                                                            }).ToList<ConciliacaoVendas>();
                                //recebimentos = resultado.Select(r => new ConciliacaoVendas
                                //{
                                //    Tipo = TIPO_RECEBIMENTO, // recebimento
                                //    Id = Convert.ToInt32(r["idRecebimento"]),
                                //    Nsu = Convert.ToString(r["nsu"]),
                                //    CodResumoVendas = r["codResumoVenda"].Equals(DBNull.Value) ? "" : Convert.ToString(r["codResumoVenda"]),
                                //    Bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                //    Data = (DateTime)r["dtaVenda"],
                                //    Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                //    Valor = Convert.ToDecimal(r["valorVendaBruta"]),
                                //    Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                //    Parcelas = Convert.ToInt32(r["numParcelaTotal"].Equals(DBNull.Value) ? 1 : r["numParcelaTotal"]),
                                //}).ToList<ConciliacaoVendas>();
                            }

                            int totalNaoConciliados = vendas.Count;

                            retorno.TotalDeRegistros += totalNaoConciliados;

                            if (pageSize == 0 || CollectionConciliacaoVendas.Count < pageSize)
                            {
                                #region OBTÉM AS INFORMAÇÕES DE DADOS NÃO-CONCILIADOS E BUSCA PRÉ-CONCILIAÇÕES

                                #region OBTÉM SOMENTE AS VENDAS NÃO-CONCILIADAS

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
                                        vendas = vendas.Skip(skipRowsNaoConciliados).Take(take).ToList<ConciliacaoVendas>();
                                    else if (!filtroTipoNaoConciliado)
                                        pageNumber = 1;
                                }
                                #endregion

                                #endregion

                                // Somente os não conciliados
                                dataBaseQueryRB.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda IS NULL");
                                //if (!dataBaseQueryVD.join.ContainsKey("LEFT JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                                //    dataBaseQueryVD.join.Add("LEFT JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda = " + GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda");
                                //dataBaseQueryVD.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".idRecebimentoVenda IS NULL");


                                // Vendas                            
                                if (!preConciliaComGrupo && !CnpjEmpresa.Equals(""))
                                    dataBaseQueryRB.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".cnpj = '" + CnpjEmpresa + "'");
                                    //dataBaseQueryVD.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrCNPJ = '" + CnpjEmpresa + "'");
                                //queryTbRecebimentoVenda = queryTbRecebimentoVenda.Where(e => e.nrCNPJ.Equals(CnpjEmpresa));


                                List<int> idsPreConciliados = new List<int>();

                                int contSkips = 0;
                                for (int k = 0; k < vendas.Count && (pageSize == 0 || CollectionConciliacaoVendas.Count < pageSize); k++)
                                {
                                    //ConciliacaoVendas receb = recebimentos[k];
                                    ConciliacaoVendas venda = vendas[k];

                                    //List<ConciliacaoVendas> vendas = new List<ConciliacaoVendas>();
                                    List<ConciliacaoVendas> recebimentos = new List<ConciliacaoVendas>();

                                    // SE FOR ENVIADO O FILTRO DE "NÃO CONCILIADO", EXIBE TODOS OS QUE NÃO TIVEREM TÍTULOS ASSOCIADOS, INDEPENDENTEMENTE SE PUDER SER PRÉ-CONCILIADO
                                    if (!filtroTipoNaoConciliado)
                                    {
                                        string nsu = venda.Nsu;//"" + Convert.ToInt32(venda.Nsu);
                                        
                                        SimpleDataBaseQuery queryRbNaoConciliado = new SimpleDataBaseQuery(dataBaseQueryRB);

                                        // WHERE
                                        queryRbNaoConciliado.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".dtaVenda BETWEEN '" + DataBaseQueries.GetDate(venda.Data) + "' AND '" + DataBaseQueries.GetDate(venda.Data) + " 23:59:00'");
                                        queryRbNaoConciliado.AddWhereClause("SUBSTRING('000000000000' + CONVERT(VARCHAR(12), " + GatewayRecebimento.SIGLA_QUERY + ".nsu), LEN(" + GatewayRecebimento.SIGLA_QUERY + ".nsu) + 1, 12) = SUBSTRING('000000000000' + CONVERT(VARCHAR(12), '" + nsu + "'), LEN('" + nsu + "') + 1, 12)");
                                        //queryVDNaoConciliado.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".nrNSU LIKE '%" + nsu + "'");
                                        
                                        if (idsPreConciliados.Count > 0)
                                            queryRbNaoConciliado.AddWhereClause(GatewayRecebimento.SIGLA_QUERY + ".id NOT IN (" + string.Join(", ", idsPreConciliados) + ")");
                                            //queryVDNaoConciliado.AddWhereClause(GatewayTbRecebimentoVenda.SIGLA_QUERY + ".idRecebimentoVenda NOT IN (" + string.Join(", ", idsPreConciliados) + ")");

                                        // Para cada tbRecebimentoVenda, procurar
                                        resultado = DataBaseQueries.SqlQuery(queryRbNaoConciliado.Script(), connection);

                                        if (resultado != null && resultado.Count > 0)
                                        {
                                            recebimentos = resultado.Select(r => new ConciliacaoVendas
                                                                    {
                                                                        Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                        Id = Convert.ToInt32(r["idRecebimento"]),
                                                                        Sacado = Convert.ToString(r["cdSacado"].Equals(DBNull.Value) ? "" : r["cdSacado"]),
                                                                        Nsu = Convert.ToString(r["nsu"]),
                                                                        CodResumoVendas = r["codResumoVenda"].Equals(DBNull.Value) ? "" : Convert.ToString(r["codResumoVenda"]),
                                                                        Bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                                                        Data = (DateTime)r["dtaVenda"],
                                                                        Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                        Valor = Convert.ToDecimal(r["valorVendaBruta"]),
                                                                        Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                                                        Parcelas = Convert.ToInt32(r["numParcelaTotal"].Equals(DBNull.Value) ? 1 : r["numParcelaTotal"]),
                                                                    }).ToList<ConciliacaoVendas>();
                                            //vendas = resultado.Select(r => new ConciliacaoVendas
                                            //                    {
                                            //                        Tipo = TIPO_VENDA,
                                            //                        Id = Convert.ToInt32(r["idRecebimentoVenda"]),
                                            //                        Nsu = Convert.ToString(r["nrNSU"]),
                                            //                        Bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]),
                                            //                        Data = (DateTime)r["dtVenda"],
                                            //                        Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                            //                        Valor = Convert.ToDecimal(r["vlVenda"]),
                                            //                        Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                            //                        Parcelas = Convert.ToInt32(r["qtParcelas"])
                                            //                    })
                                            //                   .ToList<ConciliacaoVendas>();
                                        }
                                    }

                                    if (recebimentos.Count > 0)
                                    {
                                        ConciliacaoVendas rbPreConciliado = null;
                                        //ConciliacaoVendas vdPreConciliado = null;

                                        // Mesma filial da parcela
                                        List<ConciliacaoVendas> rbFilial = recebimentos.Where(e => e.Filial.Equals(venda.Filial)).ToList<ConciliacaoVendas>();
                                        if (rbFilial.Count > 0)
                                        {
                                            if (rbFilial.Count == 1)
                                            {
                                                rbPreConciliado = rbFilial.First();
                                            }
                                            else
                                            {
                                                // Não pré-concilia pois tem mais de um
                                                if (!filtroTipoPreConciliado)
                                                {
                                                    adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoVendas, new List<ConciliacaoVendas>() { venda });
                                                    //if (filtroTipoNaoConciliado) retorno.TotalDeRegistros++;
                                                }
                                                continue;
                                            }
                                        }
                                        else if(recebimentos.Count == 1)
                                            rbPreConciliado = recebimentos.First();

                                        if (rbPreConciliado != null)
                                        {
                                            // Pré-conciliado
                                            idsPreConciliados.Add(rbPreConciliado.Id);
                                            if (!filtroTipoNaoConciliado)
                                            {
                                                if (!filtroTipoPreConciliado || contSkips >= skipRows)
                                                    adicionaElementosConciliadosNaLista(CollectionConciliacaoVendas, rbPreConciliado, venda, TIPO_CONCILIADO.PRE_CONCILIADO);
                                                //if (filtroTipoPreConciliado) retorno.TotalDeRegistros++;
                                                contSkips++;
                                            }
                                        }
                                        else
                                        {
                                            // Não conciliado
                                            if (!filtroTipoPreConciliado)
                                            {
                                                adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoVendas, new List<ConciliacaoVendas>(){ venda });
                                                //if (filtroTipoNaoConciliado) retorno.TotalDeRegistros++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Não encontrado!
                                        if (!filtroTipoPreConciliado)
                                        {
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoVendas, new List<ConciliacaoVendas>() { venda });
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
                            throw new Exception(erro.Equals("") ? "Falha ao obter conciliação de vendas" : erro);
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
                                                                    .OrderBy(c => c.Venda.Data)
                                                                    .ThenBy(c => c.Venda.Valor)
                                                                    .ThenBy(c => c.Venda.Adquirente)
                                                                    .ThenBy(c => c.Venda.Bandeira)
                                                                    .ToList<dynamic>();

                    // TOTAL
                    retorno.Totais = new Dictionary<string, object>();
                    retorno.Totais.Add("valor", CollectionConciliacaoVendas.Select(r => r.Venda.Valor).Cast<decimal>().Sum());

                    #endregion
                }
                else if (colecao == 1)
                {
                    #region BUSCA VENDAS
                    if (!queryString.TryGetValue("" + (int)CAMPOS.IDRECEBIMENTOVENDA, out outValue))
                        throw new Exception("Para consultar vendas, deve ser enviado dados de uma venda!");

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


                        Int32 idRecebimentoVenda = Convert.ToInt32(queryString["" + (int)CAMPOS.IDRECEBIMENTOVENDA]);

                        //List<IDataRecord> resultado = DataBaseQueries.SqlQuery("SELECT R.cnpj, B.cdAdquirente, R.valorVendaBruta, R.dtaVenda, R.numParcelaTotal, B.dsBandeira" +
                        //                                                       " FROM pos.Recebimento R (NOLOCK)" +
                        //                                                       " JOIN card.tbBandeira B (NOLOCK) ON R.cdBandeira = B.cdBandeira" +
                        //                                                       " WHERE R.id = " + idRecebimento, connection);
                        List<IDataRecord> resultado = DataBaseQueries.SqlQuery("SELECT V.nrCNPJ, B.cdAdquirente, V.vlVenda, V.dtVenda, V.qtParcelas, dsBandeira = CASE WHEN V.dsBandeira IS NULL THEN '' ELSE V.dsBandeira END" +
                                                                               " FROM card.tbRecebimentovenda V (NOLOCK)" +
                                                                               " JOIN cliente.empresa E (NOLOCK) ON E.nu_cnpj = V.nrCNPJ" +
                                                                               " LEFT JOIN card.tbBandeiraSacado BS ON BS.cdSacado = V.cdSacado AND E.id_grupo = BS.cdGrupo" +
                                                                               " JOIN card.tbBandeira B ON B.cdBandeira = BS.cdBandeira" +
                                                                               " WHERE V.idRecebimentoVenda = " + idRecebimentoVenda, connection);
                        if (resultado == null || resultado.Count == 0)
                            throw new Exception("Recebimento inválido!");

                        var venda = resultado.Select(r => new
                                            {
                                                cnpj = Convert.ToString(r["nrCNPJ"]),
                                                cdAdquirente = Convert.ToInt32(r["cdAdquirente"]),
                                                bandeira = Convert.ToString(r["dsBandeira"]),
                                                valorVendaBruta = Convert.ToDecimal(r["vlVenda"]),
                                                dtaVenda = (DateTime)r["dtVenda"],
                                                numParcelaTotal = Convert.ToInt32(r["qtParcelas"])
                                            }).FirstOrDefault();

                        //var recebimento = resultado.Select(r => new
                        //                 {
                        //                     cnpj = Convert.ToString(r["cnpj"]),
                        //                     cdAdquirente = Convert.ToInt32(r["cdAdquirente"]),
                        //                     bandeira = Convert.ToString(r["dsBandeira"]),
                        //                     valorVendaBruta = Convert.ToDecimal(r["valorVendaBruta"]),
                        //                     dtaVenda = (DateTime)r["dtaVenda"],
                        //                     numParcelaTotal = Convert.ToInt32(r["numParcelaTotal"].Equals(DBNull.Value) ? 1 : r["numParcelaTotal"])
                        //                 }).FirstOrDefault();

                        // Pode ter enviado de uma filial diferente
                        string nrCNPJ = venda.cnpj;
                        if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                            nrCNPJ = queryString["" + (int)CAMPOS.NU_CNPJ];

                        int cdAdquirente = venda.cdAdquirente; // recebimento.Recebimento.tbBandeira.cdAdquirente
                        decimal valorVendaBruta = venda.valorVendaBruta;
                        int numParcelas = venda.numParcelaTotal;
                        //string bandeira = venda.bandeira.Substring(venda.bandeira.IndexOf("-") + 2).Trim(); // bandeira de pos.Recebimento => pega o que vem após o hífen

                        DateTime dtaVenda = venda.dtaVenda;
                        DateTime data = Convert.ToDateTime(dtaVenda.ToShortDateString());
                        DateTime dataIni = data.AddDays(RANGE_DIAS_ANTERIOR * -1);
                        DateTime dataFim = data.AddDays(RANGE_DIAS_POSTERIOR);

                        // Consulta vendas num intervalo possível de data da venda com valores dentro da margem tolerável
                        // Ordena considerando menor intervalo de data, nsu começando com T e bandeira 
                        string script = "SELECT R.id, R.numParcelaTotal, R.nsu, R.cnpj, B.dsBandeira, B.cdSacado" +
                                        ", R.dtaVenda, E.ds_fantasia, E.filial, A.nmAdquirente" +
                                        ", R.valorVendaBruta, diferencaValorVenda = ABS(R.valorVendaBruta - " + valorVendaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ")" +
                                        ", diferencaDtVenda = ABS(DATEDIFF(DAY, R.dtaVenda, '" + DataBaseQueries.GetDate(dtaVenda) + "'))" +
                                        " FROM pos.Recebimento R (NOLOCK)" +
                                        " JOIN cliente.empresa E (NOLOCK) ON E.nu_cnpj = R.cnpj" +
                                        " JOIN card.tbBandeira B (NOLOCK) ON B.cdBandeira = R.cdBandeira" +
                                        " JOIN card.tbAdquirente A (NOLOCK) ON A.cdAdquirente = B.cdAdquirente" +
                                        " WHERE R.idRecebimentoVenda IS NULL" +
                                        " AND R.dtaVenda BETWEEN '" + DataBaseQueries.GetDate(dataIni) + "' AND '" + DataBaseQueries.GetDate(dataFim) + " 23:59:00'" +
                                        " AND R.cnpj = '" + nrCNPJ + "'" +
                                        " AND ABS(R.valorVendaBruta - " + valorVendaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ") <= " + TOLERANCIA.ToString(CultureInfo.GetCultureInfo("en-GB"))
                                        ;
                        //string script = "SELECT V.idRecebimentoVenda, V.qtParcelas, V.nrNSU, V.dsBandeira" +
                        //                ", V.dtVenda, E.ds_fantasia, E.filial, A.nmAdquirente" +
                        //                ", V.vlVenda, diferencaValorVenda = ABS(V.vlVenda - " + valorVendaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ")" +
                        //                ", diferencaDtVenda = ABS(DATEDIFF(DAY, V.dtVenda, '" + DataBaseQueries.GetDate(dtaVenda) + "'))" +
                        //                ", diferencaBandeira = CASE WHEN V.dsBandeira IS NULL THEN 1 ELSE" + 
                        //                                       " CASE WHEN '" + bandeira + "' LIKE '%' + " +
                        //                                              // Bandeira de card.tbRecebimentoVenda => pega o que vem antes do espaço (se houver)
                        //                                              "(CASE WHEN CHARINDEX(' ', V.dsBandeira) > 0 THEN SUBSTRING(V.dsBandeira, 0, CHARINDEX(' ', V.dsBandeira)) ELSE V.dsBandeira END)" +
                        //                                              " + '%'" + 
                        //                                       " THEN 0 ELSE 2 END" + 
                        //                             " END" +
                        //                " FROM card.tbRecebimentoVenda V (NOLOCK)" +
                        //                " JOIN cliente.empresa E (NOLOCK) ON E.nu_cnpj = V.nrCNPJ" +
                        //                " JOIN card.tbAdquirente A (NOLOCK) ON A.cdAdquirente = V.cdAdquirente" +
                        //                " LEFT JOIN pos.Recebimento R (NOLOCK) ON R.idRecebimentoVenda = V.idRecebimentoVenda" +
                        //                " WHERE R.idRecebimentoVenda IS NULL" +
                        //                " AND V.dtVenda BETWEEN '" + DataBaseQueries.GetDate(dataIni) + "' AND '" + DataBaseQueries.GetDate(dataFim) + " 23:59:00'" +
                        //                " AND V.nrCNPJ = '" + nrCNPJ + "'" +
                        //                " AND V.cdAdquirente = " + cdAdquirente +
                        //                " AND ABS(V.vlVenda - " + valorVendaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ") <= " + TOLERANCIA.ToString(CultureInfo.GetCultureInfo("en-GB"))
                        //                ;

                        resultado = DataBaseQueries.SqlQuery(script, connection);
                        List<dynamic> recebimentos = new List<dynamic>();
                        if (resultado != null && resultado.Count > 0)
                        {
                            recebimentos = resultado.Select(r => new
                                                {
                                                    id = Convert.ToInt32(r["id"]),
                                                    sacado = Convert.ToString(r["cdSacado"].Equals(DBNull.Value) ? "" : r["cdSacado"]),
                                                    qtParcelas = Convert.ToInt32(r["numParcelaTotal"]),
                                                    nrNSU = Convert.ToString(r["nsu"]),
                                                    bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                                    dtVenda = (DateTime)r["dtaVenda"],
                                                    empresa = Convert.ToString(r["ds_fantasia"].ToString() + (r["filial"].Equals(DBNull.Value) ? "" : " " + r["filial"].ToString())).ToUpper(),
                                                    vlVenda = Convert.ToDecimal(r["valorVendaBruta"]),
                                                    tbAdquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                                    // Malandragens
                                                    diferencaValorVenda = Convert.ToDecimal(r["diferencaValorVenda"]),
                                                    diferencaDtVenda = Convert.ToInt32(r["diferencaDtVenda"]),
                                                    //diferencaBandeira = Convert.ToInt32(r["diferencaBandeira"])
                                                })
                                                .OrderBy(e => e.diferencaDtVenda)
                                                .ThenBy(e => e.diferencaValorVenda)
                                                .ThenByDescending(e => e.nrNSU.StartsWith("T"))
                                                //.ThenBy(e => e.diferencaBandeira)
                                                .ToList<dynamic>();
                            //vendas = resultado.Select(r => new
                            //                    {
                            //                        idRecebimentoVenda = Convert.ToInt32(r["idRecebimentoVenda"]),
                            //                        qtParcelas = Convert.ToInt32(r["qtParcelas"]),
                            //                        nrNSU = Convert.ToString(r["nrNSU"]),
                            //                        bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]).ToUpper(),
                            //                        dtVenda = (DateTime)r["dtVenda"],
                            //                        empresa = Convert.ToString(r["ds_fantasia"].ToString() + (r["filial"].Equals(DBNull.Value) ? "" : " " + r["filial"].ToString())).ToUpper(),
                            //                        vlVenda = Convert.ToDecimal(r["vlVenda"]),
                            //                        tbAdquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                            //                        // Malandragens
                            //                        diferencaValorVenda = Convert.ToDecimal(r["diferencaValorVenda"]),
                            //                        diferencaDtVenda = Convert.ToInt32(r["diferencaDtVenda"]),
                            //                        //diferencaBandeira = Convert.ToInt32(r["diferencaBandeira"])
                            //                    })
                            //                    .OrderBy(e => e.diferencaDtVenda)
                            //                    .ThenBy(e => e.diferencaValorVenda)
                            //                    .ThenByDescending(e => e.nrNSU.StartsWith("T"))
                            //                    //.ThenBy(e => e.diferencaBandeira)
                            //                    .ToList<dynamic>();
                        }

                        
                        // Envia todos
                        foreach (var r in recebimentos)
                        {
                            CollectionConciliacaoVendas.Add(new ConciliacaoVendas
                            {
                                Tipo = TIPO_RECEBIMENTO, //TIPO_VENDA,
                                Id = r.id,
                                Parcelas = r.qtParcelas,
                                Nsu = r.nrNSU,
                                Bandeira = r.bandeira,
                                Data = r.dtVenda,
                                Filial = r.empresa,
                                Valor = r.vlVenda,
                                Adquirente = r.tbAdquirente,
                            });
                        }
                        
                    }
                    catch (Exception e)
                    {
                        if (e is DbEntityValidationException)
                        {
                            string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                            throw new Exception(erro.Equals("") ? "Falha ao listar vendas erp" : erro);
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

                    retorno.TotalDeRegistros = CollectionConciliacaoVendas.Count;

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
                    throw new Exception(erro.Equals("") ? "Falha ao exibir as recebíveis conciliados" : erro);
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
                if (param == null || param.data == null || param.nrCNPJ == null)// || param.cdAdquirente == 0)
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

                foreach (string data in datas)
                {
                    _db.Database.ExecuteSqlCommand("EXECUTE [card].[sp_upd_ConciliaVendas] '" + param.nrCNPJ + "', '" + data + "'");
                }

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