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
        private const int RANGE_DIAS_ANTERIOR = 5;
        private const int RANGE_DIAS_POSTERIOR = 5;
        private static decimal TOLERANCIA = new decimal(0.1); // R$0,10 


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
                        Data = item.Data,
                        Filial = item.Filial,
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
                        Data = item.Data,
                        Filial = item.Filial,
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
                        Valor = titulo.Valor,
                        Bandeira = titulo.Bandeira,
                        Data = titulo.Data,
                        Filial = titulo.Filial,
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
                        Data = recebimento.Data,
                        Filial = recebimento.Filial,
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
                    IQueryable<RecebimentoParcela> queryRecebimentoParcela = GatewayRecebimentoParcela.getQuery(_db, 0, (int)GatewayRecebimentoParcela.CAMPOS.DTARECEBIMENTO, 0, 0, 0, queryStringRecebimentoParcela);
                    IQueryable<tbRecebimentoTitulo> queryTbRecebimentoTitulo = GatewayTbRecebimentoTitulo.getQuery(_db, 0, (int)GatewayTbRecebimentoTitulo.CAMPOS.DTTITULO, 0, 0, 0, queryStringTbRecebimentoTitulo);

                    // Para a paginação
                    int totalConciliados = 0;
                    int skipRows = (pageNumber - 1) * pageSize;

                    retorno.TotalDeRegistros = 0;

                    // Só busca por conciliações já concretizadas se não tiver sido requisitado um filtro do tipo PRE-CONCILIADO ou NÃO CONCILIADO
                    if (!filtroTipoPreConciliado && !filtroTipoNaoConciliado)
                    {
                        #region OBTÉM AS INFORMAÇÕES DE DADOS JÁ CONCILIADOS PREVIAMENTE

                        IQueryable<RecebimentoParcela> queryRecebimentoParcelaConciliados = queryRecebimentoParcela
                                                                                                .Where(r => r.idRecebimentoTitulo != null)
                                                                                                .OrderBy(r => r.dtaRecebimentoEfetivo ?? r.dtaRecebimento)
                                                                                                .ThenBy(r => r.Recebimento.dtaVenda);

                        totalConciliados = queryRecebimentoParcelaConciliados.Count();

                        // Total Conciliados
                        retorno.TotalDeRegistros = totalConciliados;

                        // PAGINAÇÃO
                        if (totalConciliados > 0 && pageNumber > 0 && pageSize > 0 && (skipRows >= totalConciliados || totalConciliados > pageSize))
                        {
                            if(skipRows >= totalConciliados)
                                queryRecebimentoParcelaConciliados = queryRecebimentoParcelaConciliados.Skip(totalConciliados).Take(0); // pega nenhum
                            else
                            {
                                int take =  skipRows + pageSize >= totalConciliados ? totalConciliados - skipRows : pageSize; 
                                queryRecebimentoParcelaConciliados = queryRecebimentoParcelaConciliados.Skip(skipRows).Take(take);
                            }
                        }
                        else if (filtroTipoConciliado)
                            pageNumber = 1;



                        List<dynamic> recebimentosConciliados = queryRecebimentoParcelaConciliados
                                                                        .Select(r => new
                                                                        {
                                                                            Tipo = TIPO_RECEBIMENTO, // recebimento
                                                                            Id = r.idRecebimento,
                                                                            IdRecebimentoTitulo = r.idRecebimentoTitulo,
                                                                            NumParcela = r.numParcela,
                                                                            Nsu = r.Recebimento.nsu,
                                                                            CodResumoVendas = r.Recebimento.codResumoVenda,
                                                                            Bandeira = r.Recebimento.tbBandeira.dsBandeira.ToUpper(),
                                                                            DataVenda = r.Recebimento.dtaVenda,
                                                                            Data = r.dtaRecebimentoEfetivo ?? r.dtaRecebimento,
                                                                            Filial = r.Recebimento.empresa.ds_fantasia + (r.Recebimento.empresa.filial != null ? " " + r.Recebimento.empresa.filial : ""),
                                                                            Valor = r.valorParcelaBruta,//r.valorParcelaLiquida ?? new decimal(0.0),
                                                                            Adquirente = r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente.ToUpper(),
                                                                        }).ToList<dynamic>();


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
                                                                Filial = recebParcela.Filial,
                                                                Valor = recebParcela.Valor,
                                                                Adquirente = recebParcela.Adquirente,
                                                            };

                            ConciliacaoTitulos titulo = _db.tbRecebimentoTitulos
                                                                    .Where(e => e.idRecebimentoTitulo == idRecebimentoTitulo)
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
                                                                    }).FirstOrDefault<ConciliacaoTitulos>();

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
                        IQueryable<RecebimentoParcela> queryRecebimentoParcelaNaoConciliados = queryRecebimentoParcela
                                                            .Where(r => r.idRecebimentoTitulo == null)
                                                            .OrderBy(r => r.dtaRecebimentoEfetivo ?? r.dtaRecebimento)
                                                            .ThenBy(r => r.Recebimento.dtaVenda);

                        int totalNaoConciliados = queryRecebimentoParcelaNaoConciliados.Count();

                        retorno.TotalDeRegistros += totalNaoConciliados;

                        if(pageSize == 0 || CollectionConciliacaoTitulos.Count < pageSize)
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
                                    queryRecebimentoParcelaNaoConciliados = queryRecebimentoParcelaNaoConciliados.Skip(skipRowsNaoConciliados).Take(take);
                                else if (!filtroTipoNaoConciliado)
                                    pageNumber = 1;
                            }
                            #endregion

                            List<ConciliacaoTitulos> recebimentosParcela = queryRecebimentoParcelaNaoConciliados
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
                                                                    Valor = r.valorParcelaBruta,//r.valorParcelaLiquida ?? new decimal(0.0),
                                                                    Adquirente = r.Recebimento.tbBandeira.tbAdquirente.nmAdquirente.ToUpper(),
                                                                }).ToList<ConciliacaoTitulos>();


                            #endregion

                            // Títulos
                            if (!preConciliaComGrupo && !CnpjEmpresa.Equals(""))
                                queryTbRecebimentoTitulo = queryTbRecebimentoTitulo.Where(e => e.nrCNPJ.Equals(CnpjEmpresa));


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

                                List<ConciliacaoTitulos> titulos = new List<ConciliacaoTitulos>();

                                // SE FOR ENVIADO O FILTRO DE "NÃO CONCILIADO", EXIBE TODOS OS QUE NÃO TIVEREM TÍTULOS ASSOCIADOS, INDEPENDENTEMENTE SE PUDER SER PRÉ-CONCILIADO
                                if (!filtroTipoNaoConciliado)
                                {
                                    //string filialConsulta = recebParcela.Filial;

                                    DateTime dataIni = recebParcela.Data.Subtract(new TimeSpan(RANGE_DIAS_ANTERIOR, 0, 0, 0));
                                    DateTime dataFim = recebParcela.Data.AddDays(RANGE_DIAS_POSTERIOR);
                                    string nsu = "" + Convert.ToInt32(recebParcela.Nsu);
                                    // Para cada recebimento Parcela, procurar
                                    titulos = queryTbRecebimentoTitulo
                                                                        // Só considera os títulos que não estão conciliados
                                                                        .Where(e => e.RecebimentoParcelas.Count == 0)
                                                                        // Com data no intervalo esperado
                                                                        .Where(e => e.dtTitulo >= dataIni && e.dtTitulo <= dataFim)
                                                                        // NSU
                                                                        .Where(e => e.nrNSU.EndsWith(nsu))
                                                                        // Não pode ter sido pré-conciliado
                                                                        .Where(e => !idsPreConciliados.Contains(e.idRecebimentoTitulo))
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
                                }

                                if (titulos.Count > 0)
                                {
                                    ConciliacaoTitulos titPreConciliado = null;

                                    // Mesma filial da parcela
                                    List<ConciliacaoTitulos> titFilial = titulos.Where(e => e.Filial.Equals(recebParcela.Filial)).ToList<ConciliacaoTitulos>();
                                    if (titFilial.Count > 0)
                                    {
                                        // Tem pra mesma filial
                                        foreach (ConciliacaoTitulos tit in titFilial)
                                        {
                                            // NSUs com mesmo comprimento
                                            string nsu1 = tit.Nsu;
                                            string nsu2 = recebParcela.Nsu;
                                            while (nsu1.Length < nsu2.Length) nsu1 = "0" + nsu1;
                                            while (nsu2.Length < nsu1.Length) nsu2 = "0" + nsu2;

                                            if (nsu1.Equals(nsu2) && Math.Abs(tit.Valor - recebParcela.Valor) <= TOLERANCIA)
                                            {
                                                titPreConciliado = tit;
                                                break;
                                            }
                                        }
                                        // Se não achou, pega somente os que não são da mesma filial
                                        if (titPreConciliado == null)
                                            titulos = titulos.Where(e => !e.Filial.Equals(recebParcela.Filial)).ToList<ConciliacaoTitulos>();
                                    }

                                    if (titPreConciliado == null)
                                    {
                                        // Não achou na mesma filial => Busca em outra filial do mesmo grupo
                                        foreach (ConciliacaoTitulos tit in titulos)
                                        {
                                            // NSUs com mesmo comprimento
                                            string nsu1 = tit.Nsu;
                                            string nsu2 = recebParcela.Nsu;
                                            while (nsu1.Length < nsu2.Length) nsu1 = "0" + nsu1;
                                            while (nsu2.Length < nsu1.Length) nsu2 = "0" + nsu2;

                                            if (nsu1.Equals(nsu2) && Math.Abs(tit.Valor - recebParcela.Valor) <= TOLERANCIA)
                                            {
                                                titPreConciliado = tit;
                                                break;
                                            }
                                        }
                                    }

                                    if (titPreConciliado != null)
                                    {
                                        // Pré-conciliado
                                        idsPreConciliados.Add(titPreConciliado.Id);
                                        if (!filtroTipoNaoConciliado)
                                        {
                                            if(!filtroTipoPreConciliado || contSkips >= skipRows)
                                                adicionaElementosConciliadosNaLista(CollectionConciliacaoTitulos, recebParcela, titPreConciliado, TIPO_CONCILIADO.PRE_CONCILIADO);
                                            //if (filtroTipoPreConciliado) retorno.TotalDeRegistros++;
                                            contSkips++;
                                        }
                                    }
                                    else
                                    {
                                        // Não conciliado
                                        List<ConciliacaoTitulos> rps = new List<ConciliacaoTitulos>();
                                        rps.Add(recebParcela);
                                        if (!filtroTipoPreConciliado)
                                        {
                                            adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoTitulos, rps);
                                            //if (filtroTipoNaoConciliado) retorno.TotalDeRegistros++;
                                        }
                                    }
                                }
                                else
                                {
                                    // Não encontrado!
                                    List<ConciliacaoTitulos> rps = new List<ConciliacaoTitulos>();
                                    rps.Add(recebParcela);
                                    if (!filtroTipoPreConciliado)
                                    {
                                        adicionaElementosNaoConciliadosNaLista(CollectionConciliacaoTitulos, rps);
                                        //if (filtroTipoNaoConciliado) retorno.TotalDeRegistros++;
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    // Ordena
                    CollectionConciliacaoTitulos = CollectionConciliacaoTitulos
                                                                    .OrderBy(c => c.RecebimentoParcela.Data.Year)
                                                                    .ThenBy(c => c.RecebimentoParcela.Data.Month)
                                                                    .ThenBy(c => c.RecebimentoParcela.Data.Day)
                                                                    .ThenBy(c => c.RecebimentoParcela.Valor)
                                                                    .ThenBy(c => c.Adquirente)
                                                                    .ThenBy(c => c.RecebimentoParcela.Bandeira)
                                                                    .ThenBy(c => c.RecebimentoParcela.DataVenda)
                                                                    .ToList<dynamic>();

                    // TOTAL
                    retorno.Totais = new Dictionary<string, object>();
                    retorno.Totais.Add("valor", CollectionConciliacaoTitulos.Select(r => r.RecebimentoParcela.Valor).Cast<decimal>().Sum());

                }
                else if (colecao == 1)
                {
                    #region BUSCA TÍTULOS
                    if (!queryString.TryGetValue("" + (int)CAMPOS.IDRECEBIMENTO, out outValue) ||
                        !queryString.TryGetValue("" + (int)CAMPOS.NUMPARCELA, out outValue))
                        throw new Exception("Para consultar títulos, deve ser enviado dados da parcela!");

                    Int32 idRecebimento = Convert.ToInt32(queryString["" + (int)CAMPOS.IDRECEBIMENTO]);
                    Int32 numParcela = Convert.ToInt32(queryString["" + (int)CAMPOS.NUMPARCELA]);
                    Int32 numParcela2 = numParcela;
                    if (numParcela2 == 0) numParcela2 = 1;
                    RecebimentoParcela recebimento = _db.RecebimentoParcelas.Where(e => e.idRecebimento == idRecebimento)
                                                                            .Where(e => e.numParcela == numParcela)
                                                                            .FirstOrDefault();
                    if (recebimento == null) throw new Exception("Parcela inválida!");

                    // Pode ter enviado de uma filial diferente
                    string nrCNPJ = recebimento.Recebimento.cnpj;
                    if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                        nrCNPJ = queryString["" + (int)CAMPOS.NU_CNPJ];

                    DateTime data = Convert.ToDateTime(recebimento.Recebimento.dtaVenda.ToShortDateString());
                    DateTime dataIni = data.Subtract(new TimeSpan(RANGE_DIAS_ANTERIOR, 0, 0, 0));
                    DateTime dataFim = data.AddDays(RANGE_DIAS_POSTERIOR);

                    // Consulta títulos com a mesma data da venda
                    List<dynamic> titulos = _db.tbRecebimentoTitulos.Where(e => e.dtVenda != null && /*(e.dtVenda.Value.Year == recebimento.Recebimento.dtaVenda.Year &&
                                                                                                                  e.dtVenda.Value.Month == recebimento.Recebimento.dtaVenda.Month &&
                                                                                                                  e.dtVenda.Value.Day == recebimento.Recebimento.dtaVenda.Day))*/
                                                                                            e.dtVenda.Value >= dataIni && e.dtVenda.Value <= dataFim)
                                                                                .Where(e => e.nrCNPJ.Equals(nrCNPJ))
                                                                                .Where(e => e.cdAdquirente == recebimento.Recebimento.tbBandeira.cdAdquirente)
                                                                                .Where(e => (e.vlVenda >= recebimento.Recebimento.valorVendaBruta && (e.vlVenda - recebimento.Recebimento.valorVendaBruta <= TOLERANCIA)) ||
                                                                                            (e.vlVenda < recebimento.Recebimento.valorVendaBruta && (recebimento.Recebimento.valorVendaBruta - e.vlVenda <= TOLERANCIA)))
                                                                                .Where(e => e.RecebimentoParcelas.Count == 0)
                                                                                //.Where(e => (e.vlParcela >= recebimento.valorParcelaLiquida && (e.vlParcela - recebimento.valorParcelaLiquida <= TOLERANCIA)) ||
                                                                                //            (e.vlParcela < recebimento.valorParcelaLiquida && (recebimento.valorParcelaLiquida - e.vlParcela <= TOLERANCIA)))
                                                                                .Select(e => new
                                                                                {
                                                                                    e.idRecebimentoTitulo,
                                                                                    e.nrParcela,
                                                                                    e.nrNSU,
                                                                                    bandeira = e.dsBandeira.ToUpper(),
                                                                                    e.dtVenda,
                                                                                    e.dtTitulo,
                                                                                    empresa = e.empresa.ds_fantasia + (e.empresa.filial != null ? " " + e.empresa.filial : ""),
                                                                                    e.vlParcela,
                                                                                    tbAdquirente = e.tbAdquirente.nmAdquirente.ToUpper(),
                                                                                    // Malandragens
                                                                                    diferencaValorVenda = e.vlVenda >= recebimento.Recebimento.valorVendaBruta ? e.vlVenda - recebimento.Recebimento.valorVendaBruta :
                                                                                                                                                            recebimento.Recebimento.valorVendaBruta - e.vlVenda,
                                                                                    diferencaValorParcela = e.vlParcela >= recebimento.valorParcelaBruta ? e.vlParcela - recebimento.valorParcelaBruta :
                                                                                                                                                          recebimento.valorParcelaBruta - e.vlParcela,
                                                                                    diferencaDtVenda = e.dtVenda < recebimento.Recebimento.dtaVenda ? DbFunctions.DiffDays(e.dtVenda, recebimento.Recebimento.dtaVenda) :
                                                                                                                                                      DbFunctions.DiffDays(recebimento.Recebimento.dtaVenda, e.dtVenda),
                                                                                })
                                                                                .OrderBy(e => e.diferencaDtVenda)
                                                                                .ThenBy(e => e.diferencaValorVenda)
                                                                                .ThenBy(e => e.diferencaValorParcela)
                                                                                .ThenByDescending(e => e.nrNSU.StartsWith("T"))
                                                                                .ToList<dynamic>();

                    // Mesma parcela
                    List<dynamic> titulosParcela; 
                    if(numParcela2 != numParcela)
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
                                Adquirente = t.tbAdquirente,
                            });
                        }
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
                    for (DateTime dt = dtIni; dt <= dtFim; dt.AddDays(1))
                        datas.Add(dt.Year + (dt.Month < 10 ? "0" : "") + dt.Month + (dt.Day < 10 ? "0" : "") + dt.Day);
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