﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace api.Negocios.Administracao
{
    public class GatewayPessoa
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayPessoa()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID_PESSSOA = 100,
            NM_PESSOA = 101,
            DT_NASCIMENTO = 102,
            NU_TELEFONE = 103,
            NU_RAMAL = 104,

        };

        /// <summary>
        /// Get Pessoa/Pessoa
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<pessoa> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.pessoas.AsQueryable<pessoa>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.ID_PESSSOA:
                        Int32 id_pesssoa = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_pesssoa.Equals(id_pesssoa)).AsQueryable<pessoa>();
                        break;
                    case CAMPOS.NM_PESSOA:
                        string nm_pessoa = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nm_pessoa.Equals(nm_pessoa)).AsQueryable<pessoa>();
                        break;
                    case CAMPOS.DT_NASCIMENTO:
                        DateTime dt_nascimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dt_nascimento.Equals(dt_nascimento)).AsQueryable<pessoa>();
                        break;
                    case CAMPOS.NU_TELEFONE:
                        string nu_telefone = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_telefone.Equals(nu_telefone)).AsQueryable<pessoa>();
                        break;
                    case CAMPOS.NU_RAMAL:
                        string nu_ramal = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_ramal.Equals(nu_ramal)).AsQueryable<pessoa>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.ID_PESSSOA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_pesssoa).AsQueryable<pessoa>();
                    else entity = entity.OrderByDescending(e => e.id_pesssoa).AsQueryable<pessoa>();
                    break;
                case CAMPOS.NM_PESSOA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nm_pessoa).AsQueryable<pessoa>();
                    else entity = entity.OrderByDescending(e => e.nm_pessoa).AsQueryable<pessoa>();
                    break;
                case CAMPOS.DT_NASCIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dt_nascimento).AsQueryable<pessoa>();
                    else entity = entity.OrderByDescending(e => e.dt_nascimento).AsQueryable<pessoa>();
                    break;
                case CAMPOS.NU_TELEFONE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_telefone).AsQueryable<pessoa>();
                    else entity = entity.OrderByDescending(e => e.nu_telefone).AsQueryable<pessoa>();
                    break;
                case CAMPOS.NU_RAMAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_ramal).AsQueryable<pessoa>();
                    else entity = entity.OrderByDescending(e => e.nu_ramal).AsQueryable<pessoa>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Pessoa/Pessoa
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;

            DbContextTransaction transaction = _db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionPessoa = new List<dynamic>();
                Retorno retorno = new Retorno();

                // GET QUERY
                var query = getQuery(_db, colecao, campo, orderBy, pageSize, pageNumber, queryString);


                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = query.Count();


                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    query = query.Skip(skipRows).Take(pageSize);
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                // COLEÇÃO DE RETORNO
                if (colecao == 1)
                {
                    CollectionPessoa = query.Select(e => new
                    {

                        id_pesssoa = e.id_pesssoa,
                        nm_pessoa = e.nm_pessoa,
                        dt_nascimento = e.dt_nascimento,
                        nu_telefone = e.nu_telefone,
                        nu_ramal = e.nu_ramal,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionPessoa = query.Select(e => new
                    {

                        id_pesssoa = e.id_pesssoa,
                        nm_pessoa = e.nm_pessoa,
                        dt_nascimento = e.dt_nascimento,
                        nu_telefone = e.nu_telefone,
                        nu_ramal = e.nu_ramal,
                    }).ToList<dynamic>();
                }

                transaction.Commit();

                retorno.Registros = CollectionPessoa;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar pessoa" : erro);
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
        /// Adiciona nova Pessoa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, pessoa param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                _db.pessoas.Add(param);
                _db.SaveChanges();
                return param.id_pesssoa;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao adicionar pessoa" : erro);
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
        /// Apaga uma Pessoa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id_pesssoa, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                pessoa pessoa = _db.pessoas.Where(e => e.id_pesssoa == id_pesssoa).FirstOrDefault();

                if (pessoa == null) throw new Exception("Pessoa inexistente");

                _db.pessoas.Remove(pessoa);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar pessoa" : erro);
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
        /// Altera pessoa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, pessoa param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            try
            {
                pessoa value = _db.pessoas
                            .Where(e => e.id_pesssoa == param.id_pesssoa)
                            .First<pessoa>();


                if (value == null) throw new Exception("Pessoa inexistente");

                if (param.nm_pessoa != null && param.nm_pessoa != value.nm_pessoa)
                    value.nm_pessoa = param.nm_pessoa;
                if (param.dt_nascimento != null && param.dt_nascimento != value.dt_nascimento)
                    value.dt_nascimento = param.dt_nascimento;
                if (param.nu_telefone != null && param.nu_telefone != value.nu_telefone)
                {
                    if (param.nu_telefone == "")
                        value.nu_telefone = null;
                    else
                        value.nu_telefone = param.nu_telefone;
                }
                if (param.nu_ramal != null && param.nu_ramal != value.nu_ramal)
                {
                    if (param.nu_ramal == "")
                        value.nu_ramal = null;
                    else
                        value.nu_ramal = param.nu_ramal;
                }
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar pessoa" : erro);
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
