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
    public class GatewayConciliacaoTitulos
    { 
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayConciliacaoTitulos()
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

            IDRECEBIMENTO = 300,
            NUMPARCELA = 301,
        };

        public enum TIPO_CONCILIADO
        {
            CONCILIADO = 1,
            PRE_CONCILIADO = 2,
            NAO_CONCILIADO = 3
        };

        public static string TIPO_TITULO = "T";
        public static string TIPO_RECEBIMENTO = "R";
        // Pré-Conciliação
        private const int RANGE_DIAS_ANTERIOR = 10;
        private const int RANGE_DIAS_POSTERIOR = 5;
        private const int RANGE_VENDA_DIAS_ANTERIOR = 1;
        private const int RANGE_VENDA_DIAS_POSTERIOR = 1;
        private static decimal TOLERANCIA = new decimal(0.1); // R$0,10
        private static decimal TOLERANCIA_VENDA = new decimal(0.1); // R$0,10
        private static decimal TOLERANCIA_VLPARCELA = new decimal(0.05); // R$0,02


        /// <summary>
        /// Adiciona elementos da listaNaoConciliado na lista
        /// </summary>
        /// <param name="listaConciliacao">Lista com os elementos da conciliação</param>
        /// <param name="listaNaoConciliado">Lista que contém elementos não conciliados</param>
        /// <returns></returns>
        private static void adicionaElementosNaoConciliadosNaLista(List<dynamic> listaConciliacao,
                                                                   List<ConciliacaoTitulos> listaNaoConciliado)
        {
            foreach (ConciliacaoTitulos item in listaNaoConciliado)
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
                        Valor = /*decimal.Round(*/item.Valor/*, 2)*/,
                        Bandeira = item.Bandeira,
                        Data = item.Data,
                        Filial = item.Filial,
                        ValorVenda = item.ValorVenda,
                    },
                    RecebimentoParcela = item.Tipo != TIPO_RECEBIMENTO ? null : new
                    {
                        Id = item.Id,
                        NumParcela = item.NumParcela,
                        Nsu = item.Nsu,
                        CodResumoVendas = item.CodResumoVendas,
                        DataVenda = item.DataVenda,
                        Valor = /*decimal.Round(*/item.Valor/*, 2)*/,
                        Bandeira = item.Bandeira,
                        Data = item.Data,
                        DataEfetiva = item.DataEfetiva,
                        Filial = item.Filial,
                        ValorVenda = item.ValorVenda,
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
                        Valor = /*decimal.Round(*/titulo.Valor/*, 2)*/,
                        Bandeira = titulo.Bandeira,
                        Data = titulo.Data,
                        ValorVenda = titulo.ValorVenda,
                        Filial = titulo.Filial,
                    },
                    RecebimentoParcela = new
                    {
                        Id = recebimento.Id,
                        NumParcela = recebimento.NumParcela,
                        Nsu = recebimento.Nsu,
                        CodResumoVendas = recebimento.CodResumoVendas,
                        DataVenda = recebimento.DataVenda,
                        Valor = /*decimal.Round(*/recebimento.Valor/*, 2)*/,
                        Bandeira = recebimento.Bandeira,
                        Data = recebimento.Data,
                        DataEfetiva = recebimento.DataEfetiva,
                        Filial = recebimento.Filial,
                        ValorVenda = recebimento.ValorVenda,
                    },
                    Adquirente = recebimento.Adquirente,
                });
            }
        }




       
        /// <summary>
        /// Retorna a lista de conciliação de títulos
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
                List<dynamic> CollectionConciliacaoTitulos = new List<dynamic>();
                Retorno retorno = new Retorno();

                string outValue = null;

                if (colecao == 0)
                {
                    #region COLEÇÃO 0
                    // QUERIES DE FILTRO
                    Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                    Dictionary<string, string> queryStringTbRecebimentoTitulo = new Dictionary<string, string>();

                    // PRÉ-CONCILIAÇÃO CONSIDERANDO TODO O GRUPO?
                    bool preConciliaComGrupo = false;
                    if (queryString.TryGetValue("" + (int)CAMPOS.PRECONCILIA_GRUPO, out outValue))
                        preConciliaComGrupo = Convert.ToBoolean(queryString["" + (int)CAMPOS.PRECONCILIA_GRUPO]);


                    // DATA
                    string data = String.Empty;
                    if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                    {
                        data = queryString["" + (int)CAMPOS.DATA];
                        queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, data);
                        //queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.DTTITULO, data);
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
                        //queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.NRCNPJ, CnpjEmpresa);
                    }
                    // ADQUIRENTE
                    string cdAdquirente = String.Empty;
                    if (queryString.TryGetValue("" + (int)CAMPOS.CDADQUIRENTE, out outValue))
                    {
                        cdAdquirente = queryString["" + (int)CAMPOS.CDADQUIRENTE];
                        queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDADQUIRENTE, cdAdquirente);
                        queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.CDADQUIRENTE, cdAdquirente);
                    }

                    // NSU
                    if (queryString.TryGetValue("" + (int)CAMPOS.NSU, out outValue))
                    {
                        queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NSU, queryString["" + (int)CAMPOS.NSU]);
                        //queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.NRNSU, nsu);
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
                    //IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(_db, 0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);
                    //IQueryable<tbRecebimentoTitulo> queryTbRecebimentoTitulo = GatewayTbRecebimentoTitulo.getQuery(_db, 0, (int)GatewayTbRecebimentoTitulo.CAMPOS.DTTITULO, 0, 0, 0, queryStringTbRecebimentoTitulo);

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
                        SimpleDataBaseQuery dataBaseQueryRP = GatewayRecebimentoParcela.getQuery((int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, queryStringRecebimentoParcela);
                        SimpleDataBaseQuery dataBaseQueryTI = GatewayTbRecebimentoTitulo.getQuery((int)GatewayTbRecebimentoTitulo.CAMPOS.DTTITULO, 0, queryStringTbRecebimentoTitulo);

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

                        dataBaseQueryRP.select = new string[] { GatewayRecebimento.SIGLA_QUERY + ".id as idRecebimento",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".numParcela",
                                                          GatewayRecebimento.SIGLA_QUERY + ".nsu",
                                                          GatewayRecebimento.SIGLA_QUERY + ".codResumoVenda",
                                                          GatewayTbBandeira.SIGLA_QUERY + ".dsBandeira",
                                                          GatewayRecebimento.SIGLA_QUERY + ".dtaVenda",
                                                          GatewayRecebimento.SIGLA_QUERY + ".valorVendaBruta",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo",
                                                          GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                          GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                          GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta",
                                                          GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",                                                         
                                                        };

                        // Sem ordem
                        dataBaseQueryRP.groupby = null;
                        dataBaseQueryRP.readUncommited = true;
                        dataBaseQueryRP.orderby = new string[] { "CASE WHEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL THEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo ELSE " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento END",
                                                           GatewayRecebimento.SIGLA_QUERY + ".dtaVenda",
                                                           GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta"};


                        // TITULO
                        // Adiciona join com tbAdquirente, caso não exista
                        //if (!dataBaseQueryTI.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                        //    dataBaseQueryTI.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".cdAdquirente");
                        if (!dataBaseQueryTI.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                            dataBaseQueryTI.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrCNPJ = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");

                        dataBaseQueryTI.select = new string[] { GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".idRecebimentoTitulo",
                                                          GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrParcela",
                                                          GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrNSU",
                                                          "dsBandeira = UPPER(" + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dsBandeira)",
                                                          GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dtVenda",
                                                          GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".vlVenda",
                                                          GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dtTitulo",
                                                          GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".vlParcela",
                                                          GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia",
                                                          GatewayEmpresa.SIGLA_QUERY + ".filial",
                                                          //GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",  
                                                          "nmAdquirente = (SELECT TOP 1 UPPER(nmAdquirente) FROM card.tbAdquirente (NOLOCK) WHERE cdAdquirente = CASE WHEN " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".cdAdquirente IS NOT NULL THEN " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".cdAdquirente ELSE " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".cdAdquirenteNew END)"
                                                        };
                        dataBaseQueryTI.groupby = null;
                        dataBaseQueryTI.readUncommited = true;

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
                            SimpleDataBaseQuery queryRpConciliados = new SimpleDataBaseQuery(dataBaseQueryRP);
                            queryRpConciliados.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo IS NOT NULL");

                            List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryRpConciliados.Script(), connection);

                            List<dynamic> recebimentosConciliados = new List<dynamic>();

                            if (resultado != null && resultado.Count > 0)
                            {
                                recebimentosConciliados = resultado.Select(r => new
                                                            {
                                                                Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                Id = Convert.ToInt32(r["idRecebimento"]),
                                                                IdRecebimentoTitulo = Convert.ToInt32(r["idRecebimentoTitulo"]),
                                                                NumParcela = Convert.ToInt32(r["numParcela"]),
                                                                Nsu = Convert.ToString(r["nsu"]),
                                                                CodResumoVendas = r["codResumoVenda"].Equals(DBNull.Value) ? "" : Convert.ToString(r["codResumoVenda"]),
                                                                Bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                                                DataVenda = (DateTime)r["dtaVenda"],
                                                                Data = (DateTime)r["dtaRecebimento"],
                                                                DataEfetiva = r["dtaRecebimentoEfetivo"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)r["dtaRecebimentoEfetivo"],
                                                                Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                Valor = Convert.ToDecimal(r["valorParcelaBruta"]),
                                                                ValorVenda = Convert.ToDecimal(r["valorVendaBruta"]),
                                                                Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
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
                            for (int k = 0; k < recebimentosConciliados.Count && (pageSize == 0 || CollectionConciliacaoTitulos.Count < pageSize); k++)
                            {
                                var recebParcela = recebimentosConciliados[k];
                                // Recebimento
                                Int32 idRecebimentoTitulo = Convert.ToInt32(recebParcela.IdRecebimentoTitulo);

                                ConciliacaoTitulos recebimento = new ConciliacaoTitulos
                                                                {
                                                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                    Id = recebParcela.Id,
                                                                    NumParcela = recebParcela.NumParcela,
                                                                    Nsu = recebParcela.Nsu,
                                                                    CodResumoVendas = recebParcela.CodResumoVendas,
                                                                    Bandeira = recebParcela.Bandeira,
                                                                    DataVenda = recebParcela.DataVenda,
                                                                    Data = recebParcela.Data,
                                                                    DataEfetiva = recebParcela.DataEfetiva,
                                                                    Filial = recebParcela.Filial,
                                                                    Valor = recebParcela.Valor,
                                                                    ValorVenda = recebParcela.ValorVenda,
                                                                    Adquirente = recebParcela.Adquirente,
                                                                };

                                SimpleDataBaseQuery queryTIConciliado = new SimpleDataBaseQuery(dataBaseQueryTI);
                                queryTIConciliado.AddWhereClause(GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".idRecebimentoTitulo = " + idRecebimentoTitulo);

                                resultado = DataBaseQueries.SqlQuery(queryTIConciliado.Script(), connection);

                                ConciliacaoTitulos titulo = null;

                                if (resultado != null && resultado.Count > 0)
                                {
                                    titulo = resultado.Select(r => new ConciliacaoTitulos
                                                                {
                                                                    Tipo = TIPO_TITULO,
                                                                    Id = Convert.ToInt32(r["idRecebimentoTitulo"]),
                                                                    NumParcela = Convert.ToInt32(r["nrParcela"]),
                                                                    Nsu = Convert.ToString(r["nrNSU"]),
                                                                    Bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]),
                                                                    DataVenda = r["dtVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)r["dtVenda"],
                                                                    Data = (DateTime)r["dtTitulo"],
                                                                    Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                    Valor = Convert.ToDecimal(r["vlParcela"]),
                                                                    ValorVenda = r["vlVenda"].Equals(DBNull.Value) ? new decimal(0.0) : Convert.ToDecimal(r["vlVenda"]),
                                                                    Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                                                }).FirstOrDefault();
                                }

                                if (titulo == null)
                                    continue; // falha!

                                // Adiciona
                                adicionaElementosConciliadosNaLista(CollectionConciliacaoTitulos, recebimento, titulo, TIPO_CONCILIADO.CONCILIADO);
                            }
                            #endregion
                        }

                        // Só busca por possíveis conciliações se não tiver sido requisitado um filtro do tipo CONCILIADO
                        if (!filtroTipoConciliado)
                        {

                            // NÃO CONCILIADOS
                            // Adiciona na cláusula where IDEXTRATO IS NOT NULL
                            SimpleDataBaseQuery queryRpNaoConciliados = new SimpleDataBaseQuery(dataBaseQueryRP);
                            queryRpNaoConciliados.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo IS NULL");

                            List<IDataRecord> resultado = DataBaseQueries.SqlQuery(queryRpNaoConciliados.Script(), connection);

                            List<ConciliacaoTitulos> recebimentosParcela = new List<ConciliacaoTitulos>();

                            if (resultado != null && resultado.Count > 0)
                            {
                                recebimentosParcela = resultado.Select(r => new ConciliacaoTitulos
                                {
                                    Tipo = TIPO_RECEBIMENTO, // recebimento
                                    Id = Convert.ToInt32(r["idRecebimento"]),
                                    NumParcela = Convert.ToInt32(r["numParcela"]),
                                    Nsu = Convert.ToString(r["nsu"]),
                                    CodResumoVendas = r["codResumoVenda"].Equals(DBNull.Value) ? "" : Convert.ToString(r["codResumoVenda"]),
                                    Bandeira = Convert.ToString(r["dsBandeira"]).ToUpper(),
                                    DataVenda = (DateTime)r["dtaVenda"],
                                    Data = (DateTime)r["dtaRecebimento"],
                                    DataEfetiva = r["dtaRecebimentoEfetivo"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)r["dtaRecebimentoEfetivo"],
                                    Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                    Valor = Convert.ToDecimal(r["valorParcelaBruta"]),
                                    ValorVenda = Convert.ToDecimal(r["valorVendaBruta"]),
                                    Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                }).ToList<ConciliacaoTitulos>();
                            }

                            int totalNaoConciliados = recebimentosParcela.Count;

                            retorno.TotalDeRegistros += totalNaoConciliados;

                            if (pageSize == 0 || CollectionConciliacaoTitulos.Count < pageSize)
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
                                        recebimentosParcela = recebimentosParcela.Skip(skipRowsNaoConciliados).Take(take).ToList<ConciliacaoTitulos>();
                                    else if (!filtroTipoNaoConciliado)
                                        pageNumber = 1;
                                }
                                #endregion

                                #endregion

                                // Somente os não conciliados
                                if (!dataBaseQueryTI.join.ContainsKey("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY))
                                    dataBaseQueryTI.join.Add("LEFT JOIN pos.RecebimentoParcela " + GatewayRecebimentoParcela.SIGLA_QUERY, " ON " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".idRecebimentoTitulo = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo");
                                dataBaseQueryTI.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo IS NULL");

                                // Títulos                            
                                if (!preConciliaComGrupo && !CnpjEmpresa.Equals(""))
                                    dataBaseQueryTI.AddWhereClause(GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrCNPJ = '" + CnpjEmpresa + "'");
                                //queryTbRecebimentoTitulo = queryTbRecebimentoTitulo.Where(e => e.nrCNPJ.Equals(CnpjEmpresa));


                                List<int> idsPreConciliados = new List<int>();
                                /*string filialConsulta = String.Empty;
                                if(!CnpjEmpresa.Equals("")){ 
                                    filialConsulta = _db.empresas.Where(e => e.nu_cnpj.Equals(CnpjEmpresa)).Select(e =>  e.ds_fantasia + (e.filial != null ? " " + e.filial : "")).FirstOrDefault();
                                    if(filialConsulta == null) filialConsulta = String.Empty;
                                }*/

                                int contSkips = 0;
                                for (int k = 0; k < recebimentosParcela.Count && (pageSize == 0 || CollectionConciliacaoTitulos.Count < pageSize); k++)
                                {
                                    ConciliacaoTitulos recebParcela = recebimentosParcela[k];

                                    // SE FOR ENVIADO O FILTRO DE "NÃO CONCILIADO", EXIBE TODOS OS QUE NÃO TIVEREM TÍTULOS ASSOCIADOS, INDEPENDENTEMENTE SE PUDER SER PRÉ-CONCILIADO
                                    if (filtroTipoNaoConciliado)
                                    {
                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoTitulos, new List<ConciliacaoTitulos>() { recebParcela });
                                        continue;
                                    }

                                    List<ConciliacaoTitulos> titulos = new List<ConciliacaoTitulos>();

                                    
                                    //string filialConsulta = recebParcela.Filial;

                                    DateTime dataIni = recebParcela.Data.AddDays(RANGE_DIAS_ANTERIOR * -1);
                                    DateTime dataFim = recebParcela.Data.AddDays(RANGE_DIAS_POSTERIOR);
                                    //string nsu = "" + Convert.ToInt32(recebParcela.Nsu);

                                    //if (recebParcela.Nsu.Contains("749176220"))
                                    //    recebParcela.Nsu += "";

                                    SimpleDataBaseQuery queryTINaoConciliado = new SimpleDataBaseQuery(dataBaseQueryTI);

                                    // WHERE
                                    queryTINaoConciliado.AddWhereClause(GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dtTitulo BETWEEN '" + DataBaseQueries.GetDate(dataIni) + "' AND '" + DataBaseQueries.GetDate(dataFim) + " 23:59:00'");
                                    // Número da parcela igual
                                    queryTINaoConciliado.AddWhereClause(GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrParcela " + (recebParcela.NumParcela == 0 ? "IN (0, 1)" : " = " + recebParcela.NumParcela));
                                    // Data da venda (+-)
                                    queryTINaoConciliado.AddWhereClause(GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dtVenda IS NULL OR " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dtVenda BETWEEN '" + DataBaseQueries.GetDate(recebParcela.DataVenda.Value.AddDays(-1 * RANGE_VENDA_DIAS_ANTERIOR)) + "' AND '" + DataBaseQueries.GetDate(recebParcela.DataVenda.Value.AddDays(RANGE_VENDA_DIAS_POSTERIOR)) + " 23:59:00'");
                                    // Valor da venda igual
                                    queryTINaoConciliado.AddWhereClause("ABS(" + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".vlVenda - " + decimal.Round(recebParcela.ValorVenda.Value, 2).ToString(CultureInfo.GetCultureInfo("en-GB")) + ") <= " + TOLERANCIA_VENDA.ToString(CultureInfo.GetCultureInfo("en-GB")));
                                    // Valor da parcela igual
                                    queryTINaoConciliado.AddWhereClause("ABS(" + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".vlParcela - " + decimal.Round(recebParcela.Valor, 2).ToString(CultureInfo.GetCultureInfo("en-GB")) + ") <= " + TOLERANCIA_VLPARCELA.ToString(CultureInfo.GetCultureInfo("en-GB")));
                                    if (idsPreConciliados.Count > 0)
                                        queryTINaoConciliado.AddWhereClause(GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".idRecebimentoTitulo NOT IN (" + string.Join(", ", idsPreConciliados) + ")");

                                    ConciliacaoTitulos titPreConciliado = null;
                                    resultado = null;

                                    // Gera o resultado
                                    resultado = DataBaseQueries.SqlQuery(queryTINaoConciliado.Script(), connection);

                                    if (resultado != null && resultado.Count > 1)
                                    {
                                        // Tenta pela NSU
                                        if (!recebParcela.Adquirente.Equals("POLICARD") && !recebParcela.Adquirente.Equals("GETNET") &&
                                            !recebParcela.Adquirente.Equals("SODEXO") && !recebParcela.Adquirente.Equals("VALECARD"))
                                        {
                                            // Tenta usando a NSU
                                            SimpleDataBaseQuery queryTINaoConciliadoNSU = new SimpleDataBaseQuery(queryTINaoConciliado);
                                            queryTINaoConciliadoNSU.AddWhereClause("SUBSTRING('000000000000' + CONVERT(VARCHAR(12), " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrNSU), LEN(" + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrNSU) + 1, 12) = SUBSTRING('000000000000' + CONVERT(VARCHAR(12), '" + recebParcela.Nsu + "'), LEN('" + recebParcela.Nsu + "') + 1, 12)");
                                            //queryTINaoConciliado.AddWhereClause(GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrNSU LIKE '%" + nsu + "'");

                                            // Para cada tbRecebimentoVenda, procurar
                                            resultado = DataBaseQueries.SqlQuery(queryTINaoConciliadoNSU.Script(), connection);
                                        }
                                    }
                                         
                                    if (resultado != null && resultado.Count > 0)
                                    {
                                        titulos = resultado.Select(r => new ConciliacaoTitulos
                                                            {
                                                                Tipo = TIPO_TITULO,
                                                                Id = Convert.ToInt32(r["idRecebimentoTitulo"]),
                                                                NumParcela = Convert.ToInt32(r["nrParcela"]),
                                                                Nsu = Convert.ToString(r["nrNSU"]),
                                                                Bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]),
                                                                DataVenda = r["dtVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)r["dtVenda"],
                                                                Data = (DateTime)r["dtTitulo"],
                                                                Filial = Convert.ToString(r["ds_fantasia"]) + (r["filial"].Equals(DBNull.Value) ? "" : " " + Convert.ToString(r["filial"])),
                                                                Valor = Convert.ToDecimal(r["vlParcela"]),
                                                                ValorVenda = r["vlVenda"].Equals(DBNull.Value) ? new decimal(0.0) : Convert.ToDecimal(r["vlVenda"]),
                                                                Adquirente = Convert.ToString(r["nmAdquirente"]).ToUpper(),
                                                            })
                                                            .ToList<ConciliacaoTitulos>();

                                        if (titulos.Count == 1)
                                        {
                                            titPreConciliado = titulos.First();
                                        }
                                        else
                                        {
                                            // Avalia valor da parcela
                                            decimal valorParcela = decimal.Round(recebParcela.Valor, 2);
                                            List<ConciliacaoTitulos> titVlParcela = titulos.Where(e => e.Valor == valorParcela).ToList<ConciliacaoTitulos>();
                                            if (titVlParcela.Count == 1)
                                                titPreConciliado = titVlParcela.First();
                                            else
                                            {
                                                // Valor da venda
                                                decimal valorVenda = decimal.Round(recebParcela.ValorVenda.Value, 2);
                                                List<ConciliacaoTitulos> titVlVenda;
                                                if (titVlParcela.Count > 0)
                                                    // Só considera os de mesmo valor de parcela
                                                    titVlVenda = titVlParcela.Where(e => e.ValorVenda == valorVenda).ToList<ConciliacaoTitulos>();
                                                else
                                                    titVlVenda = titulos.Where(e => e.ValorVenda == valorVenda).ToList<ConciliacaoTitulos>();

                                                if (titVlVenda.Count == 1)
                                                    titPreConciliado = titVlVenda.First();
                                                else
                                                {
                                                    // Mesma filial da venda
                                                    List<ConciliacaoTitulos> titFilial;
                                                    if (titVlVenda.Count > 0)
                                                        // Só considera os de mesmo valor de venda
                                                        titFilial = titVlVenda.Where(e => e.Filial.Equals(recebParcela.Filial)).ToList<ConciliacaoTitulos>();
                                                    else if (titVlParcela.Count > 0)
                                                        // Só considera os de mesmo valor de parcela
                                                        titFilial = titVlParcela.Where(e => e.Filial.Equals(recebParcela.Filial)).ToList<ConciliacaoTitulos>();
                                                    else
                                                        titFilial = titulos.Where(e => e.Filial.Equals(recebParcela.Filial)).ToList<ConciliacaoTitulos>();

                                                    if (titFilial.Count == 1)
                                                        titPreConciliado = titFilial.First();
                                                }
                                            }
                                        }
                                    }

                                    // Conseguiu pré-conciliar?
                                    if (titPreConciliado != null)
                                    {
                                        // Pré-conciliado
                                        idsPreConciliados.Add(titPreConciliado.Id);
                                        if (!filtroTipoNaoConciliado)
                                        {
                                            if (!filtroTipoPreConciliado || contSkips >= skipRows)
                                                adicionaElementosConciliadosNaLista(CollectionConciliacaoTitulos, recebParcela, titPreConciliado, TIPO_CONCILIADO.PRE_CONCILIADO);
                                                //if (filtroTipoPreConciliado) retorno.TotalDeRegistros++;
                                            contSkips++;
                                        }
                                    }
                                    else
                                    {
                                        // Não conciliado
                                        if (!filtroTipoPreConciliado)
                                        {
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoTitulos, new List<ConciliacaoTitulos>() { recebParcela });
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

                    // Ordena
                    CollectionConciliacaoTitulos = CollectionConciliacaoTitulos
                                                                    .OrderBy(c => c.RecebimentoParcela.DataEfetiva ?? c.RecebimentoParcela.Data)
                                                                    //.ThenBy(c => c.RecebimentoParcela.Data.Month)
                                                                    //.ThenBy(c => c.RecebimentoParcela.Data.Day)
                                                                    .ThenBy(c => c.RecebimentoParcela.Valor)
                                                                    .ThenBy(c => c.Adquirente)
                                                                    .ThenBy(c => c.RecebimentoParcela.Bandeira)
                                                                    .ThenBy(c => c.RecebimentoParcela.DataVenda)
                                                                    .ToList<dynamic>();

                    // TOTAL
                    retorno.Totais = new Dictionary<string, object>();
                    retorno.Totais.Add("valor", CollectionConciliacaoTitulos.Select(r => r.RecebimentoParcela.Valor).Cast<decimal>().Sum());

                    #endregion
                }
                else if (colecao == 1)
                {
                    
                    #region BUSCA TÍTULOS
                    if (!queryString.TryGetValue("" + (int)CAMPOS.IDRECEBIMENTO, out outValue) ||
                        !queryString.TryGetValue("" + (int)CAMPOS.NUMPARCELA, out outValue))
                        throw new Exception("Para consultar títulos, deve ser enviado dados da parcela!");

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


                        Int32 idRecebimento = Convert.ToInt32(queryString["" + (int)CAMPOS.IDRECEBIMENTO]);
                        Int32 numParcela = Convert.ToInt32(queryString["" + (int)CAMPOS.NUMPARCELA]);
                        Int32 numParcela2 = numParcela;
                        if (numParcela2 == 0) numParcela2 = 1;

                        List<IDataRecord> resultado = DataBaseQueries.SqlQuery("SELECT R.cnpj, R.nsu, B.cdAdquirente, R.valorVendaBruta, R.dtaVenda, P.valorParcelaBruta" +
                                                             " FROM pos.RecebimentoParcela P (NOLOCK)" +
                                                             " JOIN pos.Recebimento R (NOLOCK)  ON R.id = P.idRecebimento" +
                                                             " JOIN card.tbBandeira B (NOLOCK)  ON R.cdBandeira = B.cdBandeira" +
                                                             " WHERE P.idRecebimento = " + idRecebimento +
                                                             " AND P.numParcela = " + numParcela, connection);
                        if (resultado == null || resultado.Count == 0)
                        {
                            try
                            {
                                connection.Close();
                            }
                            catch { }
                            throw new Exception("Parcela inválida!");
                        }
                        var recebimento = resultado.Select(r => new
                                         {
                                             nsu = Convert.ToString(r["nsu"]),
                                             cnpj = Convert.ToString(r["cnpj"]),
                                             cdAdquirente = Convert.ToInt32(r["cdAdquirente"]),
                                             valorVendaBruta = Convert.ToDecimal(r["valorVendaBruta"]),
                                             dtaVenda = (DateTime)r["dtaVenda"],
                                             valorParcelaBruta = Convert.ToDecimal(r["valorParcelaBruta"]),
                                         }).FirstOrDefault();

                        // Pode ter enviado de uma filial diferente
                        string nrCNPJ = recebimento.cnpj;//recebimento.Recebimento.cnpj;
                        if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                            nrCNPJ = queryString["" + (int)CAMPOS.NU_CNPJ];

                        int cdAdquirente = recebimento.cdAdquirente; // recebimento.Recebimento.tbBandeira.cdAdquirente
                        decimal valorVendaBruta = recebimento.valorVendaBruta;
                        decimal valorParcelaBruta = recebimento.valorParcelaBruta;
                        string nsu = recebimento.nsu;

                        DateTime dtaVenda = recebimento.dtaVenda;
                        DateTime data = Convert.ToDateTime(dtaVenda.ToShortDateString());
                        DateTime dataIni = data.AddDays(RANGE_DIAS_ANTERIOR * -1);
                        DateTime dataFim = data.AddDays(RANGE_DIAS_POSTERIOR);

                        // Consulta títulos com a mesma data da venda
                        string script = "SELECT T.idRecebimentoTitulo, T.nrParcela, T.nrNSU, T.dsBandeira" +
                                        ", T.dtVenda, T.dtTitulo, E.ds_fantasia, E.filial" + //, A.nmAdquirente" +
                                        ", nmAdquirente = (SELECT TOP 1 UPPER(nmAdquirente) FROM card.tbAdquirente (NOLOCK) WHERE cdAdquirente = CASE WHEN T.cdAdquirente IS NOT NULL THEN T.cdAdquirente ELSE T.cdAdquirenteNew END)" +
                                        ", T.vlParcela, T.vlVenda, diferencaValorVenda = ABS(T.vlVenda - " + valorVendaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ")" +
                                        ", diferencaValorParcela = ABS(T.vlParcela - " + valorParcelaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ")" +
                                        ", diferencaDtVenda = ABS(DATEDIFF(DAY, T.dtVenda, '" + DataBaseQueries.GetDate(dtaVenda) + "'))" +
                                        " FROM card.tbRecebimentoTitulo T (NOLOCK)" +
                                        " JOIN cliente.empresa E (NOLOCK) ON E.nu_cnpj = T.nrCNPJ" +
                                        //" JOIN card.tbAdquirente A (NOLOCK) ON A.cdAdquirente = T.cdAdquirente" +
                                        " LEFT JOIN pos.RecebimentoParcela P (NOLOCK) ON P.idRecebimentoTitulo = T.idRecebimentoTitulo" +
                                        " WHERE P.idRecebimentoTitulo IS NULL" +
                                        " AND T.dtVenda BETWEEN '" + DataBaseQueries.GetDate(dataIni) + "' AND '" + DataBaseQueries.GetDate(dataFim) + " 23:59:00'" +
                                        " AND T.nrCNPJ = '" + nrCNPJ + "'" +
                                        //" AND T.cdAdquirente = " + cdAdquirente +
                                        " AND (CASE WHEN T.cdAdquirente IS NOT NULL THEN T.cdAdquirente ELSE ISNULL(T.cdAdquirenteNew, 0) END) = " + cdAdquirente +
                                        " AND (ABS(T.vlVenda - " + valorVendaBruta.ToString(CultureInfo.GetCultureInfo("en-GB")) + ") <= " + TOLERANCIA.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                                        "      OR (SUBSTRING('000000000000' + CONVERT(VARCHAR(12), T.nrNSU), LEN(T.nrNSU) + 1, 12) = SUBSTRING('000000000000' + CONVERT(VARCHAR(12), '" + nsu + "'), LEN('" + nsu + "') + 1, 12)))"; // nsu
                        resultado = DataBaseQueries.SqlQuery(script, connection);
                        List<dynamic> titulos = new List<dynamic>();
                        if (resultado != null && resultado.Count > 0)
                        {
                            titulos = resultado.Select(r => new
                                                {
                                                    idRecebimentoTitulo = Convert.ToInt32(r["idRecebimentoTitulo"]),
                                                    nrParcela = Convert.ToInt32(r["nrParcela"]),
                                                    nrNSU = Convert.ToString(r["nrNSU"]),
                                                    bandeira = Convert.ToString(r["dsBandeira"].Equals(DBNull.Value) ? "" : r["dsBandeira"]).ToUpper(),
                                                    dtVenda = r["dtVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)r["dtVenda"],
                                                    dtTitulo = (DateTime)r["dtTitulo"],
                                                    empresa = Convert.ToString(r["ds_fantasia"].ToString() + (r["filial"].Equals(DBNull.Value) ? "" : " " + r["filial"].ToString())).ToUpper(),
                                                    vlParcela = Convert.ToDecimal(r["vlParcela"]),
                                                    vlVenda = Convert.ToDecimal(r["vlVenda"].Equals(DBNull.Value) ? 0.0 : r["vlVenda"]),
                                                    tbAdquirente = Convert.ToString(r["nmAdquirente"].Equals(DBNull.Value) ? "" : r["nmAdquirente"]).ToUpper(),
                                                    // Malandragens
                                                    diferencaValorVenda = Convert.ToDecimal(r["diferencaValorVenda"]),
                                                    diferencaValorParcela = Convert.ToDecimal(r["diferencaValorParcela"]),
                                                    diferencaDtVenda = Convert.ToInt32(r["diferencaDtVenda"]),
                                                })
                                                .OrderBy(e => e.diferencaDtVenda)
                                                .ThenBy(e => e.diferencaValorVenda)
                                                .ThenBy(e => e.diferencaValorParcela)
                                                .ThenByDescending(e => e.nrNSU.StartsWith("T"))
                                                .ToList<dynamic>();
                        }

                        // Mesma parcela
                        List<dynamic> titulosParcela;
                        if (numParcela2 != numParcela)
                            titulosParcela = titulos.Where(e => e.nrParcela == numParcela || e.nrParcela == numParcela2).ToList<dynamic>();
                        else
                            titulosParcela = titulos.Where(e => e.nrParcela == numParcela).ToList<dynamic>();
                        if (titulosParcela.Count > 0)
                        {
                            // Envia somente eles
                            foreach (var t in titulosParcela)
                            {
                                CollectionConciliacaoTitulos.Add(new ConciliacaoTitulos
                                                        {
                                                            Tipo = TIPO_TITULO, // título
                                                            Id = t.idRecebimentoTitulo,
                                                            NumParcela = t.nrParcela,
                                                            Nsu = t.nrNSU,
                                                            //CodResumoVendas = null,
                                                            Bandeira = t.bandeira,
                                                            DataVenda = t.dtVenda,
                                                            Data = t.dtTitulo,
                                                            Filial = t.empresa,
                                                            Valor = t.vlParcela,
                                                            ValorVenda = t.vlVenda,
                                                            Adquirente = t.tbAdquirente,
                                                        });
                            }
                        }
                        else
                        {
                            // Envia todos
                            foreach (var t in titulos)
                            {
                                CollectionConciliacaoTitulos.Add(new ConciliacaoTitulos
                                {
                                    Tipo = TIPO_TITULO, // título
                                    Id = t.idRecebimentoTitulo,
                                    NumParcela = t.nrParcela,
                                    Nsu = t.nrNSU,
                                    //CodResumoVendas = null,
                                    Bandeira = t.bandeira,
                                    DataVenda = t.dtVenda,
                                    Data = t.dtTitulo,
                                    Filial = t.empresa,
                                    Valor = t.vlParcela,
                                    ValorVenda = t.vlVenda,
                                    Adquirente = t.tbAdquirente,
                                });
                            }
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

                    retorno.TotalDeRegistros = CollectionConciliacaoTitulos.Count;

                    #endregion
                }

                

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
        /// Aponta RecebimentoParcela para TbRecebimentoTitulo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, List<ConciliaRecebimentoParcelaTitulo> param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction(); // tudo ou nada
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
                            tbRecebimentoTitulo = _db.Database.SqlQuery<tbRecebimentoTitulo>("SELECT T.*" +
                                                                                             " FROM card.tbRecebimentoTitulo T (NOLOCK)" +
                                                                                             " WHERE T.idRecebimentoTitulo = " + conciliaTitulo.idRecebimentoTitulo
                                                                                            )
                                                              .FirstOrDefault();
                            //tbRecebimentoTitulo = _db.tbRecebimentoTitulos.Where(e => e.idRecebimentoTitulo == conciliaTitulo.idRecebimentoTitulo).FirstOrDefault();
                            if (tbRecebimentoTitulo == null) 
                                continue; // título inválido!

                            // Desconcilia parcelas que estavam apontando para o título
                            _db.Database.ExecuteSqlCommand("UPDATE P" +
                                                           " SET P.idRecebimentoTitulo = NULL" +
                                                           " FROM pos.RecebimentoParcela P" +
                                                           " WHERE P.idRecebimentoTitulo = " + conciliaTitulo.idRecebimentoTitulo
                                                          );
                            _db.SaveChanges();

                            // Concilia parcela
                            _db.Database.ExecuteSqlCommand("UPDATE P" +
                                                           " SET P.idRecebimentoTitulo = " + conciliaTitulo.idRecebimentoTitulo +
                                                           " FROM pos.RecebimentoParcela P" +
                                                           " WHERE P.idRecebimento = " + conciliaTitulo.recebimentoParcela.idRecebimento +
                                                           " AND P.numParcela = " + conciliaTitulo.recebimentoParcela.numParcela
                                                          );
                            _db.SaveChanges();
                        }

                        // RECEBIMENTO PARCELA
                        //RecebimentoParcela value = _db.RecebimentoParcelas
                        //                                        .Where(e => e.idRecebimento == conciliaTitulo.recebimentoParcela.idRecebimento)
                        //                                        .Where(e => e.numParcela == conciliaTitulo.recebimentoParcela.numParcela)
                        //                                        .FirstOrDefault();
                        //if (value != null)
                        //{
                        //    if (conciliaTitulo.idRecebimentoTitulo == -1) value.idRecebimentoTitulo = null;
                        //    else value.idRecebimentoTitulo = conciliaTitulo.idRecebimentoTitulo;
                        //    _db.SaveChanges();
                        //}
                        else
                        {
                            // Desconcilia
                            _db.Database.ExecuteSqlCommand("UPDATE P" +
                                                           " SET P.idRecebimentoTitulo = NULL" +
                                                           " FROM pos.RecebimentoParcela P" +
                                                           " WHERE P.idRecebimento = " + conciliaTitulo.recebimentoParcela.idRecebimento +
                                                           " AND P.numParcela = " + conciliaTitulo.recebimentoParcela.numParcela
                                                          );
                            _db.SaveChanges();
                        }
                        
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
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a conciliação de títulos" : erro);
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
        /// Usa a SP para realizar a conciliação de títulos TEF
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

                foreach (string data in datas)
                {
                    //_db.Database.SqlQuery<string>("EXECUTE [card].[sp_upd_ConciliaTitulos] '" + 
                    //                               param.nrCNPJ  + "', '" + data + "', " + param.cdAdquirente);
                    _db.Database.ExecuteSqlCommand("EXECUTE [card].[sp_upd_ConciliaTitulos] '" + 
                                                    param.nrCNPJ  + "', '" + data + "', " + param.cdAdquirente);
                }

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a conciliação de títulos TEF" : erro);
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