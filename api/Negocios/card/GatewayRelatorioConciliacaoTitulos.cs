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
using api.Negocios.Cliente;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace api.Negocios.Card
{
    public class GatewayRelatorioConciliacaoTitulos
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRelatorioConciliacaoTitulos()
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
            NU_CNPJ_TITULO = 103,
            TIPO = 104, // 1 : FILIAL DIFERENTE | 2 : NÃO CONCILIADOS

            // RELACIONAMENTOS
            CDADQUIRENTE = 200,
        };

        private const int TIPO_FILIAL_DIFERENTE = 1;
        private const int TIPO_NAO_CONCILIADO = 2;

        /// <summary>
        /// Retorna o relatório de conciliação de títulos
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
                List<dynamic> CollectionRelatorioConciliacaoTitulos = new List<dynamic>();
                Retorno retorno = new Retorno();


                // QUERIES DE FILTRO
                string outValue = null;
                Dictionary<string, string> queryStringRecebimentoParcela = new Dictionary<string, string>();
                //Dictionary<string, string> queryStringTbRecebimentoTitulo = new Dictionary<string, string>();
                // DATA
                string data = String.Empty;
                if (queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                {
                    data = queryString["" + (int)CAMPOS.DATA];
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, data);
                    //queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.DTTITULO, data);
                }
                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo != 0)
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    //queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                else throw new Exception("Um grupo deve ser selecionado como filtro de relatório de conciliação de títulos!");
                // FILIAL
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (CnpjEmpresa.Equals("") && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    CnpjEmpresa = queryString["" + (int)CAMPOS.NU_CNPJ];
                if (!CnpjEmpresa.Equals(""))
                {
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.NU_CNPJ, CnpjEmpresa);
                    //queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.NRCNPJ, CnpjEmpresa);
                }
                else throw new Exception("Uma filial deve ser selecionada como filtro de relatório de conciliação de títulos!");
                // ADQUIRENTE
                string cdAdquirente = String.Empty;
                if (queryString.TryGetValue("" + (int)CAMPOS.CDADQUIRENTE, out outValue))
                {
                    cdAdquirente = queryString["" + (int)CAMPOS.CDADQUIRENTE];
                    queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.CDADQUIRENTE, cdAdquirente);
                    // queryStringTbRecebimentoTitulo.Add("" + (int)GatewayTbRecebimentoTitulo.CAMPOS.CDADQUIRENTE, cdAdquirente);
                }

                // SOMENTE AS PARCELAS QUE TEM TÍTULOS CONCILIADOS
                //queryStringRecebimentoParcela.Add("" + (int)GatewayRecebimentoParcela.CAMPOS.IDRECEBIMENTOTITULO, "0");

                // CONEXÃO
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

                try
                {
                    connection.Open();
                }
                catch
                {
                    throw new Exception("Não foi possível estabelecer conexão com a base de dados");
                }


                #region OBTÉM A QUERY
                //IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(_db, 0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);
                SimpleDataBaseQuery dataBaseQuery = GatewayRecebimentoParcela.getQuery((int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTOEFETIVO, 0, queryStringRecebimentoParcela);

                // RECEBIMENTO PARCELA
                if (!dataBaseQuery.join.ContainsKey("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY))
                    dataBaseQuery.join.Add("INNER JOIN pos.Recebimento " + GatewayRecebimento.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".id = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimento");
                if (!dataBaseQuery.join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                    dataBaseQuery.join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira = " + GatewayRecebimento.SIGLA_QUERY + ".cdBandeira");
                if (!dataBaseQuery.join.ContainsKey("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY))
                    dataBaseQuery.join.Add("INNER JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY, " ON " + GatewayTbAdquirente.SIGLA_QUERY + ".cdAdquirente = " + GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente");
                if (!dataBaseQuery.join.ContainsKey("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY))
                    dataBaseQuery.join.Add("INNER JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY, " ON " + GatewayRecebimento.SIGLA_QUERY + ".cnpj = " + GatewayEmpresa.SIGLA_QUERY + ".nu_cnpj");
                // JOINS para Título
                if (!dataBaseQuery.join.ContainsKey("LEFT JOIN card.tbRecebimentoTitulo " + GatewayTbRecebimentoTitulo.SIGLA_QUERY))
                    dataBaseQuery.join.Add("LEFT JOIN card.tbRecebimentoTitulo " + GatewayTbRecebimentoTitulo.SIGLA_QUERY, " ON " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".idRecebimentoTitulo = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo");
                if (!dataBaseQuery.join.ContainsKey("LEFT JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY + "2"))
                    dataBaseQuery.join.Add("LEFT JOIN cliente.empresa " + GatewayEmpresa.SIGLA_QUERY + "2", " ON " + GatewayEmpresa.SIGLA_QUERY + "2.nu_cnpj = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrCNPJ");
                if (!dataBaseQuery.join.ContainsKey("LEFT JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY + "2"))
                    dataBaseQuery.join.Add("LEFT JOIN card.tbAdquirente " + GatewayTbAdquirente.SIGLA_QUERY + "2", " ON " + GatewayTbAdquirente.SIGLA_QUERY + "2.cdAdquirente = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".cdAdquirente");
                
                // SELECT
                dataBaseQuery.select = new string[] { // RECEBIMENTO PARCELA
                                                      "R_Id = " + GatewayRecebimento.SIGLA_QUERY + ".id", 
                                                      "R_NumParcela = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".numParcela",
                                                      "R_Nsu = " + GatewayRecebimento.SIGLA_QUERY + ".nsu",
                                                      "R_ValorLiquido = ISNULL(" +  GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaLiquida, 0.0)",
                                                      "R_DataVenda = " + GatewayRecebimento.SIGLA_QUERY + ".dtaVenda",
                                                      "R_DataPrevista = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento",
                                                      "R_DataEfetiva = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo",
                                                      "R_Filial = UPPER(" + GatewayEmpresa.SIGLA_QUERY + ".ds_fantasia + CASE WHEN " + GatewayEmpresa.SIGLA_QUERY + ".filial IS NOT NULL THEN ' ' + " + GatewayEmpresa.SIGLA_QUERY + ".filial ELSE '' END)",
                                                      "R_Valor = " + GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta",
                                                      "R_ValorVenda = " + GatewayRecebimento.SIGLA_QUERY + ".valorVendaBruta",
                                                      "R_Adquirente = " + GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente",
                                                      // TÍTULO
                                                      "T_Id = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".idRecebimentoTitulo", 
                                                      "T_NumParcela = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrParcela",
                                                      "T_Nsu = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrNSU",
                                                      "T_DataVenda = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dtVenda",
                                                      "T_Data = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".dtTitulo",
                                                      "T_Filial = UPPER(" + GatewayEmpresa.SIGLA_QUERY + "2.ds_fantasia + CASE WHEN " + GatewayEmpresa.SIGLA_QUERY + "2.filial IS NOT NULL THEN ' ' + " + GatewayEmpresa.SIGLA_QUERY + "2.filial ELSE '' END)",
                                                      "T_Valor = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".vlParcela",
                                                      "T_ValorVenda = " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".vlVenda",
                                                      "T_Adquirente = " + GatewayTbAdquirente.SIGLA_QUERY + "2.nmAdquirente",
                                                      };
                dataBaseQuery.orderby = new string[] { "CASE WHEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo IS NOT NULL THEN 0 ELSE 1 END", // prioriza os que tem títulos
			                                           "CASE WHEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo IS NOT NULL THEN " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimentoEfetivo ELSE " + GatewayRecebimentoParcela.SIGLA_QUERY + ".dtaRecebimento END", 
                                                       GatewayTbAdquirente.SIGLA_QUERY + ".nmAdquirente", 
                                                       GatewayRecebimento.SIGLA_QUERY + ".dtaVenda", 
                                                       GatewayRecebimentoParcela.SIGLA_QUERY + ".valorParcelaBruta"
                                                     };

                // Tipo
                if (queryString.TryGetValue("" + (int)CAMPOS.TIPO, out outValue))
                {
                    Int32 tipo = Convert.ToInt32(queryString["" + (int)CAMPOS.TIPO]);
                    if (tipo == TIPO_FILIAL_DIFERENTE)
                    {   // Somente as parcelas que foram conciliados com um título de filial diferente
                        //queryRecebimentoParcela = queryRecebimentoParcela
                        //                        .Where(t => t.idRecebimentoTitulo != null && !t.Recebimento.cnpj.Equals(t.tbRecebimentoTitulo.nrCNPJ));
                        dataBaseQuery.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo IS NOT NULL AND " + GatewayRecebimento.SIGLA_QUERY + ".cnpj != " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrCNPJ");
                    }
                    else if (tipo == TIPO_NAO_CONCILIADO)
                    {   // Somente as parcelas que não foram conciliadas
                        //queryRecebimentoParcela = queryRecebimentoParcela
                        //                        .Where(t => t.idRecebimentoTitulo == null);
                        dataBaseQuery.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo IS NULL");
                    }
                    else throw new Exception("Filtro de tipo informado é inválido!");
                }
                else if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ_TITULO, out outValue))
                {
                    string cnpjTitulo = queryString["" + (int)CAMPOS.NU_CNPJ_TITULO];
                    //queryRecebimentoParcela = queryRecebimentoParcela
                    //    // Somente as parcelas que foram conciliados com um título de filial igual ao cnpj enviado
                    //                            .Where(t => t.idRecebimentoTitulo != null && t.tbRecebimentoTitulo.nrCNPJ.Equals(cnpjTitulo));
                    dataBaseQuery.AddWhereClause(GatewayRecebimentoParcela.SIGLA_QUERY + ".idRecebimentoTitulo IS NOT NULL AND " + GatewayTbRecebimentoTitulo.SIGLA_QUERY + ".nrCNPJ = '" + cnpjTitulo + "'");
                }
                /*else
                {
                    queryRecebimentoParcela = queryRecebimentoParcela
                                                // Somente as parcelas que não foram conciliadas ou aquelas que foram conciliados com um título de filial diferente
                                                .Where(t => t.idRecebimentoTitulo == null || (t.idRecebimentoTitulo != null && !t.Recebimento.cnpj.Equals(t.tbRecebimentoTitulo.nrCNPJ)));
                }*/

                dataBaseQuery.readUncommited = true;
                #endregion

                List<IDataRecord> resultado = DataBaseQueries.SqlQuery(dataBaseQuery.Script(), connection);

                if (resultado != null && resultado.Count > 0)
                {
                    CollectionRelatorioConciliacaoTitulos = resultado.Select(t => new
                    {
                        Titulo = t["T_Id"].Equals(DBNull.Value) ? null : new
                        {
                            Id = Convert.ToInt32(t["T_Id"]),
                            NumParcela = Convert.ToInt32(t["T_NumParcela"]),
                            Nsu = Convert.ToString(t["T_Nsu"].Equals(DBNull.Value) ? "" : t["T_Nsu"]),
                            DataVenda = t["T_DataVenda"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["T_DataVenda"],
                            Valor = Convert.ToDecimal(t["T_Valor"]),
                            Data = (DateTime)t["T_Data"],
                            Filial = Convert.ToString(t["T_Filial"]),
                            ValorVenda = Convert.ToDecimal(t["T_ValorVenda"].Equals(DBNull.Value) ? 0.0 : t["T_ValorVenda"]),
                        },
                        RecebimentoParcela = new
                        {
                            Id = Convert.ToInt32(t["R_Id"]),
                            NumParcela = Convert.ToInt32(t["R_NumParcela"]),
                            Nsu = Convert.ToString(t["R_Nsu"]),
                            //CodResumoVendas = recebimento.CodResumoVendas,
                            DataVenda = (DateTime)t["R_DataVenda"],
                            Valor = Convert.ToDecimal(t["R_Valor"]),
                            ValorLiquido = Convert.ToDecimal(t["R_ValorLiquido"]),
                            //Bandeira = recebimento.Bandeira,
                            Data = (DateTime)t["R_DataPrevista"],
                            DataEfetiva = t["R_DataEfetiva"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)t["R_DataEfetiva"],
                            Filial = Convert.ToString(t["R_Filial"]),
                            ValorVenda = Convert.ToDecimal(t["R_ValorVenda"]),
                        },
                        Adquirente = Convert.ToString(t["R_Adquirente"]),
                    }).ToList<dynamic>();
                }

                try
                {
                    connection.Close();
                }
                catch { }

                //List<dynamic> rRecebimentoParcela = queryRecebimentoParcela
                //                                            .Select(t => new
                //                                            {
                //                                                idRecebimento = t.idRecebimento,
                //                                                dtaRecebimento = t.dtaRecebimentoEfetivo != null ? t.dtaRecebimentoEfetivo.Value : t.dtaRecebimento,
                //                                                filial = t.Recebimento.empresa.ds_fantasia.ToUpper() + (t.Recebimento.empresa.filial != null ? " " + t.Recebimento.empresa.filial.ToUpper() : ""),
                //                                                adquirente = t.Recebimento.tbBandeira.tbAdquirente.nmAdquirente.ToUpper(),
                //                                                //bandeira = t.Recebimento.tbBandeira.dsBandeira,
                //                                                valorBruto = t.valorParcelaBruta,
                //                                                valorLiquido = t.valorParcelaLiquida != null ? t.valorParcelaLiquida.Value : new decimal(0.0),
                //                                                valorVenda = t.Recebimento.valorVendaBruta,
                //                                                idRecebimentoTitulo = t.idRecebimentoTitulo,
                //                                                dtaVenda = t.Recebimento.dtaVenda,
                //                                                nsu = t.Recebimento.nsu,
                //                                                numParcela = t.numParcela,
                //                                                //codResumoVenda = t.Recebimento.codResumoVenda,
                //                                            }).OrderBy(t => t.filial)
                //                                              // prioriza os conciliados
                //                                              .ThenByDescending(t => t.idRecebimentoTitulo.HasValue)
                //                                              .ThenBy(t => t.dtaRecebimento)
                //                                              .ThenBy(t => t.adquirente)
                //                                              .ThenBy(t => t.dtaVenda.Year)
                //                                              .ThenBy(t => t.dtaVenda.Month)
                //                                              .ThenBy(t => t.dtaVenda.Day)
                //                                              .ThenBy(t => t.valorBruto)
                //                                              .ToList<dynamic>();

                // TOTAL DE REGISTROS
                //retorno.TotalDeRegistros = rRecebimentoParcela.Count;

                //// PAGINAÇÃO
                //int skipRows = (pageNumber - 1) * pageSize;
                //if (rRecebimentoParcela.Count > pageSize && pageNumber > 0 && pageSize > 0)
                //    rRecebimentoParcela = rRecebimentoParcela.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                //else
                //    pageNumber = 1;
                retorno.TotalDeRegistros = CollectionRelatorioConciliacaoTitulos.Count;

                // TOTAL
                retorno.Totais = new Dictionary<string, object>();
                retorno.Totais.Add("valorTotalBruto", CollectionRelatorioConciliacaoTitulos.Select(r => r.RecebimentoParcela.Valor).Cast<decimal>().Sum());
                retorno.Totais.Add("valorTotalERP", CollectionRelatorioConciliacaoTitulos.Where(r => r.Titulo != null).Select(r => r.Titulo.Valor).Cast<decimal>().Sum());
                List<dynamic> filiais = CollectionRelatorioConciliacaoTitulos.Where(t => t.Titulo != null)
                                                    .GroupBy(t => t.Titulo.Filial)
                                                    .OrderBy(t => t.Key)
                                                    .Select(t => new
                                                    {
                                                        filial = t.Key,
                                                        valor = t.Select(x => x.RecebimentoParcela.Valor).Cast<decimal>().Sum(),
                                                        valorERP = t.Select(x => x.Titulo.Valor).Cast<decimal>().Sum()
                                                    }).ToList<dynamic>();

                if (filiais.Count > 0)
                {
                    foreach (var filial in filiais)
                    {
                        retorno.Totais.Add("(valorTotalBruto) " + filial.filial, filial.valor);
                        retorno.Totais.Add("(valorTotalERP) " + filial.filial, filial.valorERP);
                    }
                }

                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (CollectionRelatorioConciliacaoTitulos.Count > pageSize && pageNumber > 0 && pageSize > 0)
                    CollectionRelatorioConciliacaoTitulos = CollectionRelatorioConciliacaoTitulos.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                else
                    pageNumber = 1;


                //foreach (var recebParcela in rRecebimentoParcela)
                //{
                //    ConciliacaoTitulos recebimento = new ConciliacaoTitulos
                //    {
                //        Tipo = GatewayConciliacaoTitulos.TIPO_RECEBIMENTO, // recebimento
                //        Id = recebParcela.idRecebimento,
                //        NumParcela = recebParcela.numParcela,
                //        Nsu = recebParcela.nsu,
                //        ValorLiquido = /*decimal.Round(*/recebParcela.valorLiquido/*, 2)*/,
                //        //CodResumoVendas = recebParcela.codResumoVenda,
                //        //Bandeira = recebParcela.bandeira,
                //        DataVenda = recebParcela.dtaVenda,
                //        Data = recebParcela.dtaRecebimento,
                //        Filial = recebParcela.filial,
                //        Valor = /*decimal.Round(*/recebParcela.valorBruto/*, 2)*/,
                //        ValorVenda = /*decimal.Round(*/recebParcela.valorVenda/*, 2)*/,
                //        Adquirente = recebParcela.adquirente,
                //    };

                //    if (recebParcela.idRecebimentoTitulo != null)
                //    {
                //        // PARCELA CONCILIADA
                //        Int32 idRecebimentoTitulo = recebParcela.idRecebimentoTitulo;

                //        ConciliacaoTitulos titulo = _db.tbRecebimentoTitulos
                //                                                .Where(e => e.idRecebimentoTitulo == idRecebimentoTitulo)
                //                                                .Select(e => new ConciliacaoTitulos
                //                                                {
                //                                                    Tipo = GatewayConciliacaoTitulos.TIPO_TITULO, // título
                //                                                    Id = e.idRecebimentoTitulo,
                //                                                    NumParcela = e.nrParcela,
                //                                                    Nsu = e.nrNSU,
                //                                                    //Bandeira = e.dsBandeira.ToUpper(),
                //                                                    DataVenda = e.dtVenda,
                //                                                    Data = e.dtTitulo,
                //                                                    Filial = e.empresa.ds_fantasia.ToUpper() + (e.empresa.filial != null ? " " + e.empresa.filial.ToUpper() : ""),
                //                                                    Valor = e.vlParcela,
                //                                                    ValorVenda = e.vlVenda,
                //                                                    Adquirente = e.tbAdquirente.nmAdquirente.ToUpper(),
                //                                                }).FirstOrDefault<ConciliacaoTitulos>();

                //        if (titulo == null)
                //            continue; // falha!

                //        // Adiciona
                //        CollectionRelatorioConciliacaoTitulos.Add(new
                //        {
                //            Titulo = new
                //            {
                //                Id = titulo.Id,
                //                NumParcela = titulo.NumParcela,
                //                Nsu = titulo.Nsu,
                //                DataVenda = titulo.DataVenda,
                //                Valor = titulo.Valor,
                //                //Bandeira = titulo.Bandeira,
                //                Data = titulo.Data,
                //                Filial = titulo.Filial,
                //                ValorVenda = titulo.ValorVenda,
                //            },
                //            RecebimentoParcela = new
                //            {
                //                Id = recebimento.Id,
                //                NumParcela = recebimento.NumParcela,
                //                Nsu = recebimento.Nsu,
                //                //CodResumoVendas = recebimento.CodResumoVendas,
                //                DataVenda = recebimento.DataVenda,
                //                Valor = recebimento.Valor,
                //                ValorLiquido = recebimento.ValorLiquido,
                //                //Bandeira = recebimento.Bandeira,
                //                Data = recebimento.Data,
                //                Filial = recebimento.Filial,
                //                ValorVenda = recebimento.ValorVenda,
                //            },
                //            Adquirente = recebimento.Adquirente,
                //        });
                //    }
                //    else
                //    {
                //        // PARCELA NÃO CONCILIADA
                //        CollectionRelatorioConciliacaoTitulos.Add(new
                //        {
                //            Titulo = (string)null,
                //            RecebimentoParcela = new
                //            {
                //                Id = recebimento.Id,
                //                NumParcela = recebimento.NumParcela,
                //                Nsu = recebimento.Nsu,
                //                //CodResumoVendas = recebimento.CodResumoVendas,
                //                DataVenda = recebimento.DataVenda,
                //                Valor = recebimento.Valor,
                //                ValorLiquido = recebimento.ValorLiquido,
                //                //Bandeira = recebimento.Bandeira,
                //                Data = recebimento.Data,
                //                Filial = recebimento.Filial,
                //                ValorVenda = recebimento.ValorVenda,
                //            },
                //            Adquirente = recebimento.Adquirente,
                //        });
                //    }
                //}

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = CollectionRelatorioConciliacaoTitulos;

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