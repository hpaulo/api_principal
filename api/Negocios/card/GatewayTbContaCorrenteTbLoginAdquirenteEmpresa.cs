﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Globalization;
using System.Data.Entity.Validation;
using api.Negocios.Util;
using System.Data.Entity;
using System.Data;

namespace api.Negocios.Card
{
    public class GatewayTbContaCorrenteTbLoginAdquirenteEmpresa
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbContaCorrenteTbLoginAdquirenteEmpresa()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "VG";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDCONTACORRENTE = 100,
            CDLOGINADQUIRENTEEMPRESA = 101,
            DTINICIO = 102,
            DTFIM = 103,

            // RELACIONAMENTOS
            DS_FANTASIA = 204,

            CDADQUIRENTE = 300,
            //NMADQUIRENTE = 301,

            DTVIGENCIA = 400,

            NU_CNPJ = 500,
            ID_GRUPO = 516,

            STLOGINADQUIRENTE = 608,

        };

        /// <summary>
        /// Get TbContaCorrente_tbLoginAdquirenteEmpresa/TbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbContaCorrente_tbLoginAdquirenteEmpresas.AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdContaCorrente == cdContaCorrente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.CDLOGINADQUIRENTEEMPRESA:
                        Int32 cdLoginAdquirenteEmpresa = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdLoginAdquirenteEmpresa == cdLoginAdquirenteEmpresa).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.DTINICIO:
                        DateTime dtInicio = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtInicio.Equals(dtInicio)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.DTFIM:
                        DateTime dtFim = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtFim.Equals(dtFim)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;

                    // PERSONALIZADO
                    case CAMPOS.DS_FANTASIA:
                        string ds_fantasia = Convert.ToString(item.Value);
                        if (ds_fantasia.Contains("%")) // usa LIKE
                        {
                            string busca = ds_fantasia.Replace("%", "").ToString();
                            entity = entity.Where(e => e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia.Contains(busca)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        }
                        else
                            entity = entity.Where(e => e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia.Equals(ds_fantasia)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    //case CAMPOS.NMADQUIRENTE:
                    //    string nome = Convert.ToString(item.Value);
                    //    if (nome.Contains("%")) // usa LIKE
                    //    {
                    //        string busca = nome.Replace("%", "").ToString();
                    //        entity = entity.Where(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente.Contains(busca)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    //    }
                    //    else
                    //        entity = entity.Where(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente.Equals(nome)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    //    break;
                    case CAMPOS.DTVIGENCIA:
                        if (item.Value.Contains("|")) // BETWEEN
                        {
                            string[] busca = item.Value.Split('|');
                            DateTime dtIni = DateTime.ParseExact(busca[0] + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            DateTime dtaFim = DateTime.ParseExact(busca[1] + " 23:59:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(v => (v.dtInicio.Year < dtIni.Year || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month < dtIni.Month) || (v.dtInicio.Year == dtIni.Year && v.dtInicio.Month == dtIni.Month && v.dtInicio.Day <= dtIni.Day))
                                                        && (v.dtFim == null || (v.dtFim.Value.Year > dtIni.Year || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month > dtIni.Month) || (v.dtFim.Value.Year == dtIni.Year && v.dtFim.Value.Month == dtIni.Month && v.dtFim.Value.Day >= dtIni.Day)))
                                                        && (v.dtInicio.Year < dtaFim.Year || (v.dtInicio.Year == dtaFim.Year && v.dtInicio.Month < dtaFim.Month) || (v.dtInicio.Year == dtaFim.Year && v.dtInicio.Month == dtaFim.Month && v.dtInicio.Day <= dtaFim.Day))
                                                        && (v.dtFim == null || (v.dtFim.Value.Year > dtaFim.Year || (v.dtFim.Value.Year == dtaFim.Year && v.dtFim.Value.Month > dtaFim.Month) || (v.dtFim.Value.Year == dtaFim.Year && v.dtFim.Value.Month == dtaFim.Month && v.dtFim.Value.Day >= dtaFim.Day)))
                                                        ).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        }
                        else // ANO + MES + DIA
                        {
                            string busca = item.Value;
                            DateTime data = DateTime.ParseExact(busca + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            entity = entity.Where(v => (v.dtInicio.Year < data.Year || (v.dtInicio.Year == data.Year && v.dtInicio.Month < data.Month) || (v.dtInicio.Year == data.Year && v.dtInicio.Month == data.Month && v.dtInicio.Day <= data.Day))
                                                        && (v.dtFim == null || (v.dtFim.Value.Year > data.Year || (v.dtFim.Value.Year == data.Year && v.dtFim.Value.Month > data.Month) || (v.dtFim.Value.Year == data.Year && v.dtFim.Value.Month == data.Month && v.dtFim.Value.Day >= data.Day)))
                                                        && (v.dtInicio.Year < data.Year || (v.dtInicio.Year == data.Year && v.dtInicio.Month < data.Month) || (v.dtInicio.Year == data.Year && v.dtInicio.Month == data.Month && v.dtInicio.Day <= data.Day))
                                                        && (v.dtFim == null || (v.dtFim.Value.Year > data.Year || (v.dtFim.Value.Year == data.Year && v.dtFim.Value.Month > data.Month) || (v.dtFim.Value.Year == data.Year && v.dtFim.Value.Month == data.Month && v.dtFim.Value.Day >= data.Day)))
                                                        ).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        }
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbLoginAdquirenteEmpresa.empresa.id_grupo == id_grupo).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tbLoginAdquirenteEmpresa.nrCnpj.Equals(nu_cnpj)).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.STLOGINADQUIRENTE:
                        byte stLoginAdquirente = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.tbLoginAdquirenteEmpresa.stLoginAdquirente == stLoginAdquirente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbLoginAdquirenteEmpresa.cdAdquirente == cdAdquirente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdContaCorrente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdContaCorrente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.CDLOGINADQUIRENTEEMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdLoginAdquirenteEmpresa).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdLoginAdquirenteEmpresa).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.DTINICIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtInicio).ThenBy(e => e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia).ThenBy(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dtInicio).ThenBy(e => e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia).ThenBy(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.DTFIM:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtFim).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dtFim).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;

                // PERSONALIZADO
                case CAMPOS.DS_FANTASIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia).ThenBy(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente).ThenByDescending(e => e.dtInicio).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia).ThenByDescending(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente).ThenByDescending(e => e.dtInicio).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                    break;
                //case CAMPOS.DSADQUIRENTE:
                //    if (orderby == 0) entity = entity.OrderBy(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente).ThenBy(e => e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia).ThenByDescending(e => e.dtInicio).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                //    else entity = entity.OrderByDescending(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente).ThenByDescending(e => e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia).ThenByDescending(e => e.dtInicio).AsQueryable<tbContaCorrente_tbLoginAdquirenteEmpresa>();
                //    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbContaCorrente_tbLoginAdquirenteEmpresa/TbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null)
                _db = new painel_taxservices_dbContext();
            else
                _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Implementar o filtro por Grupo apartir do TOKEN do Usuário
                string outValue = null;
                Int32 IdGrupo = 0;
                IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                        queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
                }
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                if (!CnpjEmpresa.Equals(""))
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                        queryString["" + (int)CAMPOS.NU_CNPJ] = CnpjEmpresa;
                    else
                        queryString.Add("" + (int)CAMPOS.NU_CNPJ, CnpjEmpresa);
                }

                // GET QUERY
                var query = getQuery(_db, colecao, campo, orderBy, pageSize, pageNumber, queryString);

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = query.Count();


                if (colecao != 5)
                {
                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;
                }

               

                // COLEÇÃO DE RETORNO
                if (colecao == 1)
                {
                    CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = query.Select(e => new
                    {

                        cdContaCorrente = e.cdContaCorrente,
                        cdLoginAdquirenteEmpresa = e.cdLoginAdquirenteEmpresa,
                        dtInicio = e.dtInicio,
                        dtFim = e.dtFim,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = query.Select(e => new
                    {

                        cdContaCorrente = e.cdContaCorrente,
                        cdLoginAdquirenteEmpresa = e.cdLoginAdquirenteEmpresa,
                        dtInicio = e.dtInicio,
                        dtFim = e.dtFim,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2) // [WEB] Vigência da Conta Corrente
                {
                    CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = query.Select(e => new
                    {

                        dtInicio = e.dtInicio,
                        dtFim = e.dtFim,
                        cdLoginAdquirenteEmpresa = e.tbLoginAdquirenteEmpresa.cdLoginAdquirenteEmpresa,
                        adquirente = new
                        {
                            cdAdquirente = e.tbLoginAdquirenteEmpresa.tbAdquirente.cdAdquirente,
                            nmAdquirente = e.tbLoginAdquirenteEmpresa.tbAdquirente.nmAdquirente,
                            dsAdquirente = e.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente,
                            stAdquirente = e.tbLoginAdquirenteEmpresa.tbAdquirente.stAdquirente,
                        },
                        empresa = new
                        {
                            nu_cnpj = e.tbLoginAdquirenteEmpresa.empresa.nu_cnpj,
                            ds_fantasia = e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia,
                            filial = e.tbLoginAdquirenteEmpresa.empresa.filial
                        },
                        stLoginAdquirente = e.tbLoginAdquirenteEmpresa.stLoginAdquirente,
                        //stLoginAdquirenteEmpresa = l.tbLoginAdquirenteEmpresa.stLoginAdquirenteEmpresa // controle de bruno
                    }).ToList<dynamic>();
                }
                else if (colecao == 3) // [WEB] Filiais
                {
                    CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = query
                        .Where(e => e.tbLoginAdquirenteEmpresa.empresa.fl_ativo == 1)
                        .GroupBy(e => e.tbLoginAdquirenteEmpresa.empresa.nu_cnpj)
                        .Select(e => new
                    {
                        nu_cnpj = e.Key,
                        ds_fantasia = e.Select(f => f.tbLoginAdquirenteEmpresa.empresa.ds_fantasia).FirstOrDefault(),
                        filial = e.Select(f => f.tbLoginAdquirenteEmpresa.empresa.filial).FirstOrDefault(),
                        adquirentes = e.Select(f => new
                        {
                            cdAdquirente = f.tbLoginAdquirenteEmpresa.tbAdquirente.cdAdquirente,
                            nmAdquirente = f.tbLoginAdquirenteEmpresa.tbAdquirente.nmAdquirente,
                            stAdquirente = f.tbLoginAdquirenteEmpresa.tbAdquirente.stAdquirente,
                        }).OrderBy(f => f.nmAdquirente).ToList<dynamic>(),
                    }).OrderBy(e => e.ds_fantasia).ThenBy(e => e.filial).ToList<dynamic>();
                }
                else if (colecao == 4) // [WEB] Adquirentes
                {
                    CollectionTbContaCorrente_tbLoginAdquirenteEmpresa = query
                        .Where(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.stAdquirente == 1)
                        .GroupBy(e => e.tbLoginAdquirenteEmpresa.tbAdquirente.cdAdquirente)
                        .Select(e => new
                        {
                            cdAdquirente = e.Key,
                            nmAdquirente = e.Select(f => f.tbLoginAdquirenteEmpresa.tbAdquirente.nmAdquirente).FirstOrDefault(),
                            stAdquirente = e.Select(f => f.tbLoginAdquirenteEmpresa.tbAdquirente.stAdquirente).FirstOrDefault(),
                        }).OrderBy(e => e.nmAdquirente).ToList<dynamic>();
                }
                else if (colecao == 5) // [WEB] Relação adquirente-filial por conta
                {
                    List<dynamic> tbLoginAdquirenteEmpresas = query
                        .Where(e => e.tbLoginAdquirenteEmpresa.empresa.fl_ativo == 1)
                        .Select(e => new
                        {
                            tbContaCorrente = new
                            {
                                e.tbContaCorrente.cdContaCorrente,
                                //e.tbContaCorrente.cdBanco,
                                banco = new { Codigo = e.tbContaCorrente.cdBanco, NomeExtenso = "" },
                                e.tbContaCorrente.nrAgencia,
                                e.tbContaCorrente.nrConta
                            },
                            empresa = new
                            {
                                e.tbLoginAdquirenteEmpresa.empresa.nu_cnpj,
                                e.tbLoginAdquirenteEmpresa.empresa.ds_fantasia,
                                e.tbLoginAdquirenteEmpresa.empresa.filial
                            },
                            tbAdquirente = new
                            {
                                e.tbLoginAdquirenteEmpresa.tbAdquirente.cdAdquirente,
                                e.tbLoginAdquirenteEmpresa.tbAdquirente.nmAdquirente
                            },
                            cdEstabelecimento = e.tbLoginAdquirenteEmpresa.cdEstabelecimento,
                            cdEstabelecimentoConsulta = e.tbLoginAdquirenteEmpresa.cdEstabelecimentoConsulta,
                            status = e.tbLoginAdquirenteEmpresa.stLoginAdquirente,
                            dtInicio = e.dtInicio,
                            dtFim = e.dtFim
                        })
                        .OrderBy(e => e.empresa.nu_cnpj)
                        .ThenBy(e => e.tbAdquirente.nmAdquirente)
                        .ThenBy(e => e.tbContaCorrente.banco.Codigo)
                        .ThenBy(e => e.tbContaCorrente.nrAgencia)
                        .ThenBy(e => e.tbContaCorrente.nrConta)
                        .ToList<dynamic>();

                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (tbLoginAdquirenteEmpresas.Count > pageSize && pageNumber > 0 && pageSize > 0)
                        tbLoginAdquirenteEmpresas = tbLoginAdquirenteEmpresas.Skip(skipRows).Take(pageSize).ToList<dynamic>();
                    else
                        pageNumber = 1;

                    retorno.PaginaAtual = pageNumber;
                    retorno.ItensPorPagina = pageSize;

                    // Após transformar em lista (isto é, trazer para a memória), atualiza o valor do NomeExtenso associado ao banco
                    foreach (var tbLoginAdquirenteEmpresa in tbLoginAdquirenteEmpresas)
                    {
                        CollectionTbContaCorrente_tbLoginAdquirenteEmpresa.Add(new
                        {
                            tbContaCorrente = new {
                                cdContaCorrente = tbLoginAdquirenteEmpresa.tbContaCorrente.cdContaCorrente,
                                //e.tbContaCorrente.cdBanco,
                                banco = new { Codigo = tbLoginAdquirenteEmpresa.tbContaCorrente.banco.Codigo, NomeExtenso = GatewayBancos.Get(tbLoginAdquirenteEmpresa.tbContaCorrente.banco.Codigo) },
                                nrAgencia = tbLoginAdquirenteEmpresa.tbContaCorrente.nrAgencia,
                                nrConta = tbLoginAdquirenteEmpresa.tbContaCorrente.nrConta,
                            },
                            empresa = tbLoginAdquirenteEmpresa.empresa,
                            tbAdquirente = tbLoginAdquirenteEmpresa.tbAdquirente,
                            cdEstabelecimento = tbLoginAdquirenteEmpresa.cdEstabelecimento,
                            cdEstabelecimentoConsulta = tbLoginAdquirenteEmpresa.cdEstabelecimentoConsulta,
                            status = tbLoginAdquirenteEmpresa.status,
                            dtInicio = tbLoginAdquirenteEmpresa.dtInicio,
                            dtFim = tbLoginAdquirenteEmpresa.dtFim
                        });
                    }
                }

                transaction.Commit();

                retorno.Registros = CollectionTbContaCorrente_tbLoginAdquirenteEmpresa;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar adquirente empresa" : erro);
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


        /**
          * Procura por conflitos de período de vigência a partir do parâmetro
          * Retorna null se não houve conflito. Caso contrário, retorna o registro ao qual o período conflita
          */
        private static tbContaCorrente_tbLoginAdquirenteEmpresa conflitoPeriodoVigencia(tbContaCorrente_tbLoginAdquirenteEmpresa param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null)
                _db = new painel_taxservices_dbContext();
            else
                _db = _dbContext;
            try
            {
                // Avalia se, para o mesmo cdContaCorrente e cdLoginAdquirenteEmpresa
                // o período informado já consta
                List<tbContaCorrente_tbLoginAdquirenteEmpresa> values = _db.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                                .Where(e => e.cdContaCorrente == param.cdContaCorrente)
                                                                                .Where(e => e.cdLoginAdquirenteEmpresa == param.cdLoginAdquirenteEmpresa)
                                                                                .ToList<tbContaCorrente_tbLoginAdquirenteEmpresa>();

                foreach (var value in values)
                {
                    // Não avalia o período se o parâmetro for mesmo elemento "value" corrente
                    if (value.cdContaCorrente == param.cdContaCorrente &&
                       value.cdLoginAdquirenteEmpresa == param.cdLoginAdquirenteEmpresa &&
                       value.dtInicio.Equals(param.dtInicio))
                        continue;

                    if (value.dtFim != null)
                    {
                        // Não é o vigente de dtFim nulo

                        if (param.dtFim == null)
                        {
                            // Data início do parâmetro deve ser superior a dtFim do encontrado na base
                            if (param.dtInicio.Year < Convert.ToDateTime(value.dtFim).Year ||
                               (param.dtInicio.Year == Convert.ToDateTime(value.dtFim).Year && param.dtInicio.Month < Convert.ToDateTime(value.dtFim).Month) ||
                               (param.dtInicio.Year == Convert.ToDateTime(value.dtFim).Year && param.dtInicio.Month == Convert.ToDateTime(value.dtFim).Month && param.dtInicio.Day <= Convert.ToDateTime(value.dtFim).Day))
                                return value;
                        }
                        else
                        {
                            // Avalia intervalos por completo => compara desconsiderando os horários
                            DateTime paramInicio = new DateTime(param.dtInicio.Year, param.dtInicio.Month, param.dtInicio.Day);
                            DateTime paramFim = new DateTime(Convert.ToDateTime(param.dtInicio).Year, Convert.ToDateTime(param.dtFim).Month, Convert.ToDateTime(param.dtFim).Day);
                            DateTime valueInicio = new DateTime(value.dtInicio.Year, value.dtInicio.Month, value.dtInicio.Day);
                            DateTime valueFim = new DateTime(Convert.ToDateTime(value.dtInicio).Year, Convert.ToDateTime(value.dtFim).Month, Convert.ToDateTime(value.dtFim).Day);

                            // Início de um não pode estar dentro do intervalo do outro
                            if (paramInicio.Ticks >= valueInicio.Ticks && paramInicio.Ticks <= valueFim.Ticks)
                                return value;
                            if (valueInicio.Ticks >= paramInicio.Ticks && valueInicio.Ticks <= paramFim.Ticks)
                                return value;

                            // Fim de um não pode estar dentro do intervalo do outro
                            if (paramFim.Ticks >= valueInicio.Ticks && paramFim.Ticks <= valueFim.Ticks)
                                return value;
                            if (valueFim.Ticks >= paramInicio.Ticks && valueFim.Ticks <= paramFim.Ticks)
                                return value;
                        }

                    }
                    else
                    {
                        // Já existe um vigente com dtFim nulo => período de dtInicio até oo

                        if (param.dtFim == null)
                            return value; // Só pode existir um com dtFim nulo

                        // Data início do parâmetro deve ser inferior a do encontrado na base
                        if (param.dtInicio.Year > value.dtInicio.Year ||
                           (param.dtInicio.Year == value.dtInicio.Year && param.dtInicio.Month > value.dtInicio.Month) ||
                           (param.dtInicio.Year == value.dtInicio.Year && param.dtInicio.Month == value.dtInicio.Month && param.dtInicio.Day >= value.dtInicio.Day) ||
                            // Data fim do parâmetro deve ser inferior a dtInício do encontrado na base
                           Convert.ToDateTime(param.dtFim).Year > value.dtInicio.Year ||
                           (Convert.ToDateTime(param.dtFim).Year == value.dtInicio.Year && Convert.ToDateTime(param.dtFim).Month > value.dtInicio.Month) ||
                           (Convert.ToDateTime(param.dtFim).Year == value.dtInicio.Year && Convert.ToDateTime(param.dtFim).Month == value.dtInicio.Month && Convert.ToDateTime(param.dtFim).Day >= value.dtInicio.Day))
                            return value;
                    }
                }
                // Não teve conflito de período de vigência
                return null;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao verificar conflito em adquirente empresa" : erro);
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
        /// Adiciona nova TbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbContaCorrente_tbLoginAdquirenteEmpresa param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null)
                _db = new painel_taxservices_dbContext();
            else
                _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbContaCorrente_tbLoginAdquirenteEmpresa value = _db.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                        .Where(e => e.cdContaCorrente == param.cdContaCorrente)
                                                                        .Where(e => e.cdLoginAdquirenteEmpresa == param.cdLoginAdquirenteEmpresa)
                                                                        .Where(e => e.dtInicio.Equals(param.dtInicio))
                                                                        .FirstOrDefault();
                if (value != null) throw new Exception("Vigência já existe!");
                if (conflitoPeriodoVigencia(param) != null) throw new Exception("Conflito de período de vigência");
                _db.tbContaCorrente_tbLoginAdquirenteEmpresas.Add(param);
                _db.SaveChanges();
                transaction.Commit();
                return param.cdContaCorrente;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar adquirente empresa" : erro);
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
        /// Apaga todos as vigências da conta
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdContaCorrente, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null)
                _db = new painel_taxservices_dbContext();
            else
                _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                _db.tbContaCorrente_tbLoginAdquirenteEmpresas.RemoveRange(_db.tbContaCorrente_tbLoginAdquirenteEmpresas.Where(e => e.cdContaCorrente == cdContaCorrente));
                _db.SaveChanges();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar adquirente empresa" : erro);
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
        /// Apaga uma TbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdContaCorrente, Int32 cdLoginAdquirenteEmpresa, DateTime dtInicio, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null)
                _db = new painel_taxservices_dbContext();
            else
                _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbContaCorrente_tbLoginAdquirenteEmpresa value = _db.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                        .Where(e => e.cdContaCorrente == cdContaCorrente)
                                                                        .Where(e => e.cdLoginAdquirenteEmpresa == cdLoginAdquirenteEmpresa)
                                                                        .Where(e => e.dtInicio.Equals(dtInicio))
                                                                        .FirstOrDefault();
                if (value == null) throw new Exception("Vigência inválida!");
                _db.tbContaCorrente_tbLoginAdquirenteEmpresas.Remove(value);
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar adquirente empresa" : erro);
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
        /// Altera tbContaCorrente_tbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbContaCorrente_tbLoginAdquirenteEmpresa param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null)
                _db = new painel_taxservices_dbContext();
            else
                _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                tbContaCorrente_tbLoginAdquirenteEmpresa value = _db.tbContaCorrente_tbLoginAdquirenteEmpresas
                                                                       .Where(e => e.cdContaCorrente == param.cdContaCorrente)
                                                                       .Where(e => e.cdLoginAdquirenteEmpresa == param.cdLoginAdquirenteEmpresa)
                                                                       .Where(e => e.dtInicio.Equals(param.dtInicio))
                                                                       .FirstOrDefault();
                if (value == null) throw new Exception("Vigência inválida!");

                if (conflitoPeriodoVigencia(param) != null) throw new Exception("Conflito de período de vigência");

                // Só altera a data fim => SEMPRE TEM QUE SER ENVIADO A DATA FIM, POIS ESTÁ PODE SER SETADA PARA NULL
                if (!param.dtFim.Equals(value.dtFim))
                {
                    value.dtFim = param.dtFim;
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar adquirente empresa" : erro);
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
