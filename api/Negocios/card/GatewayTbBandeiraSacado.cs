using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Data;
using api.Negocios.Util;

namespace api.Negocios.Card
{
    public class GatewayTbBandeiraSacado
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbBandeiraSacado()
        {
            //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "BS";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDGRUPO = 100,
            CDBANDEIRA = 101,
            QTPARCELAS = 102,
            CDSACADO = 103,

            // RELACIONAMENTOS
            CDADQUIRENTE = 202
        };

        /// <summary>
        /// Get TbBandeiraSacado/TbBandeiraSacado
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbBandeiraSacado> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbBandeiraSacados.AsQueryable<tbBandeiraSacado>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDGRUPO:
                        Int32 cdGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdGrupo == cdGrupo).AsQueryable<tbBandeiraSacado>();
                        break;
                    case CAMPOS.CDSACADO:
                        string cdSacado = Convert.ToString(item.Value);
                        if (cdSacado.Contains("%"))
                        {
                            string busca = cdSacado.Replace("%", "");
                            entity = entity.Where(e => e.cdSacado.StartsWith(busca)).AsQueryable<tbBandeiraSacado>(); // STARTS WITH
                        }
                        else
                            entity = entity.Where(e => e.cdSacado.Equals(cdSacado)).AsQueryable<tbBandeiraSacado>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira == cdBandeira).AsQueryable<tbBandeiraSacado>();
                        break;
                    case CAMPOS.QTPARCELAS:
                        byte qtParcelas = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.qtParcelas == qtParcelas).AsQueryable<tbBandeiraSacado>();
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.tbBandeira.cdAdquirente == cdAdquirente).AsQueryable<tbBandeiraSacado>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdGrupo).AsQueryable<tbBandeiraSacado>();
                    else entity = entity.OrderByDescending(e => e.cdGrupo).AsQueryable<tbBandeiraSacado>();
                    break;
                case CAMPOS.CDSACADO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSacado).AsQueryable<tbBandeiraSacado>();
                    else entity = entity.OrderByDescending(e => e.cdSacado).AsQueryable<tbBandeiraSacado>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbBandeiraSacado>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbBandeiraSacado>();
                    break;
                case CAMPOS.QTPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtParcelas).AsQueryable<tbBandeiraSacado>();
                    else entity = entity.OrderByDescending(e => e.qtParcelas).AsQueryable<tbBandeiraSacado>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Get TbBandeiraSacado/TbBandeiraSacado
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static SimpleDataBaseQuery getQuery(int campo, int orderby, Dictionary<string, string> queryString)
        {
            Dictionary<string, string> join = new Dictionary<string, string>();
            List<string> where = new List<string>();
            List<string> order = new List<string>();

            #region WHERE - ADICIONA OS FILTROS A QUERY
            // ADICIONA OS FILTROS A QUERY
            foreach (KeyValuePair<string, string> item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDGRUPO:
                        Int32 cdGrupo = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".cdGrupo = " + cdGrupo);
                        break;
                    case CAMPOS.CDSACADO:
                        string cdSacado = Convert.ToString(item.Value);
                        if (cdSacado.Contains("%"))
                        {
                            string busca = cdSacado.Replace("%", "");
                            where.Add(SIGLA_QUERY + ".cdSacado LIKE '" + busca + "%'");
                        }
                        else
                            where.Add(SIGLA_QUERY + ".cdSacado = '" + cdSacado + "'");
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".cdBandeira = " + cdBandeira);
                        break;
                    case CAMPOS.QTPARCELAS:
                        byte qtParcelas = Convert.ToByte(item.Value);
                        where.Add(SIGLA_QUERY + ".qtParcelas = " + qtParcelas);
                        break;
                    // RELACIONAMENTOS
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        // JOIN
                        if (!join.ContainsKey("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY))
                            join.Add("INNER JOIN card.tbBandeira " + GatewayTbBandeira.SIGLA_QUERY, " ON " + SIGLA_QUERY + ".cdBandeira = " + GatewayTbBandeira.SIGLA_QUERY + ".cdBandeira");
                        where.Add(GatewayTbBandeira.SIGLA_QUERY + ".cdAdquirente = " + cdAdquirente);
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdGrupo ASC");
                    else order.Add(SIGLA_QUERY + ".cdGrupo DESC");
                    break;
                case CAMPOS.CDSACADO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdSacado ASC");
                    else order.Add(SIGLA_QUERY + ".cdSacado DESC");
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".cdBandeira ASC");
                    else order.Add(SIGLA_QUERY + ".cdBandeira DESC");
                    break;
                case CAMPOS.QTPARCELAS:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".qtParcelas ASC");
                    else order.Add(SIGLA_QUERY + ".qtParcelas DESC");
                    break;
            }
            #endregion

            return new SimpleDataBaseQuery(null, "card.tbBandeiraSacado " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());


        }


        /// <summary>
        /// Retorna TbBandeiraSacado/TbBandeiraSacado
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                // Implementar o filtro por Grupo apartir do TOKEN do Usuário
                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token, _db);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.CDGRUPO, out outValue))
                        queryString["" + (int)CAMPOS.CDGRUPO] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.CDGRUPO, IdGrupo.ToString());
                }

                //DECLARAÇÕES
                List<dynamic> CollectionTbBandeiraSacado = new List<dynamic>();
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
                    CollectionTbBandeiraSacado = query.Select(e => new
                    {

                        cdGrupo = e.cdGrupo,
                        cdSacado = e.cdSacado,
                        cdBandeira = e.cdBandeira,
                        qtParcelas = e.qtParcelas,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbBandeiraSacado = query.Select(e => new
                    {

                        cdGrupo = e.cdGrupo,
                        cdSacado = e.cdSacado,
                        cdBandeira = e.cdBandeira,
                        qtParcelas = e.qtParcelas,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionTbBandeiraSacado = query.Select(e => new
                    {
                        cdGrupo = e.cdGrupo,
                        cdSacado = e.cdSacado,
                        qtParcelas = e.qtParcelas,
                        tbBandeira = new { 
                                            e.cdBandeira, 
                                            e.tbBandeira.dsBandeira 
                                         }
                    }).ToList<dynamic>();
                }

                transaction.Commit();

                retorno.Registros = CollectionTbBandeiraSacado;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao consultar bandeira-sacado" : erro);
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
        /// Adiciona nova TbBandeiraSacado
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbBandeiraSacado param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                Int32 cdGrupo = Permissoes.GetIdGrupo(token, _db);

                if (param.cdGrupo != 0 && cdGrupo != param.cdGrupo)
                    throw new Exception("O usuário não está associado ao grupo empresarial cuja relação bandeira-sacado corresponde!");

                param.cdGrupo = cdGrupo;

                // Consulta bandeira
                tbBandeira tbBandeira = _db.tbBandeiras.Where(t => t.cdBandeira == param.cdBandeira).FirstOrDefault();
                if (tbBandeira == null)
                    throw new Exception("Bandeira inexistente!");

                if (tbBandeira.dsTipo.StartsWith("DÉBITO"))
                    param.qtParcelas = 0;
                else if (param.qtParcelas <= 0)
                    param.qtParcelas = 36; // default

                _db.tbBandeiraSacados.Add(param);
                _db.SaveChanges();

                //transaction.Commit();
                return param.cdGrupo;
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao adicionar bandeira-sacado" : erro);
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
        /// Apaga uma TbBandeiraSacado
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdGrupo, Int32 cdBandeira, byte qtParcelas, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                Int32 idGrupo = Permissoes.GetIdGrupo(token, _db);

                if (cdGrupo != 0 && idGrupo != cdGrupo)
                    throw new Exception("O usuário não está associado ao grupo empresarial cuja relação bandeira-sacado corresponde!");

                cdGrupo = idGrupo;

                _db.tbBandeiraSacados.RemoveRange(_db.tbBandeiraSacados.Where(e => e.cdGrupo == cdGrupo && e.cdBandeira == cdBandeira && e.qtParcelas == qtParcelas));
                _db.SaveChanges();
                //transaction.Commit();
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao remover bandeira-sacado" : erro);
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
        /// Altera tbBandeiraSacado
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, TbBandeiraSacadoUpdate param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction();
            try
            {
                Int32 cdGrupo = Permissoes.GetIdGrupo(token, _db);

                if (cdGrupo != 0 && cdGrupo != param.cdGrupo)
                    throw new Exception("O usuário não está associado ao grupo empresarial cuja relação bandeira-sacado corresponde!");

                param.cdGrupo = cdGrupo;

                tbBandeiraSacado value = _db.Database.SqlQuery<tbBandeiraSacado>("SELECT *" +
                                                                                 " FROM card.tbBandeiraSacado B (NOLOCK)" +
                                                                                 " WHERE B.cdGrupo = " + param.cdGrupo +
                                                                                 " AND B.cdBandeira = " + param.oldCdBandeira +
                                                                                 " AND B.qtParcelas = " + param.oldQtParcelas).FirstOrDefault();

                if (value == null)
                    throw new Exception("Bandeira-Sacado inexistente!");


                List<string> update = new List<string>();

                if (param.newCdBandeira != value.cdBandeira)
                {
                    // Consulta bandeira
                    tbBandeira tbBandeira = _db.tbBandeiras.Where(t => t.cdBandeira == param.newCdBandeira).FirstOrDefault();
                    if (tbBandeira == null)
                        throw new Exception("Bandeira inexistente!");

                    update.Add("B.cdBandeira = " + param.newCdBandeira);

                    if (tbBandeira.dsTipo.StartsWith("DÉBITO"))
                        update.Add("B.qtParcelas = 0");
                    else if (param.newQtParcelas == 0)
                        update.Add("B.qtParcelas = 36"); // default
                    else
                        update.Add("B.qtParcelas = " + param.newQtParcelas);
                }
                if (param.newCdSacado != null && !param.newCdSacado.Equals(value.cdSacado))
                    update.Add("B.cdSacado = '" + param.newCdSacado + "'");


                if (update.Count > 0)
                {
                    string script = "UPDATE B" +
                                    " SET " + string.Join(", ", update) +
                                    " FROM card.tbBandeiraSacado B (NOLOCK)" +
                                    " WHERE B.cdGrupo = " + param.cdGrupo +
                                    " AND B.cdBandeira = " + param.oldCdBandeira +
                                    " AND B.qtParcelas = " + param.oldQtParcelas;
                    _db.Database.ExecuteSqlCommand(script);
                    _db.SaveChanges();
                    //transaction.Commit();
                }
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao atualizar bandeira-sacado" : erro);
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
