using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Pos
{
    public class GatewayOperadora
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayOperadora()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "OP";

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            NMOPERADORA = 101,
            IDGRUPOEMPRESA = 102,

            NU_CNPJ = 300,

        };

        /// <summary>
        /// Get Operadora/Operadora
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<Operadora> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Operadoras.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.ID:
                        Int32 id = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id == id).AsQueryable();
                        break;
                    case CAMPOS.NMOPERADORA:
                        string nmOperadora = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmOperadora.Equals(nmOperadora)).AsQueryable();
                        break;
                    case CAMPOS.IDGRUPOEMPRESA:
                        Int32 idGrupoEmpresa = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idGrupoEmpresa == idGrupoEmpresa).AsQueryable();
                        break;

                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => _db.LoginOperadoras.Where( l => l.cnpj.Equals(nu_cnpj) ).Select( l => l.idOperadora ).ToList().Contains( e.id )).AsQueryable();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.ID:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<Operadora>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<Operadora>();
                    break;
                case CAMPOS.NMOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmOperadora).AsQueryable<Operadora>();
                    else entity = entity.OrderByDescending(e => e.nmOperadora).AsQueryable<Operadora>();
                    break;
                case CAMPOS.IDGRUPOEMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idGrupoEmpresa).AsQueryable<Operadora>();
                    else entity = entity.OrderByDescending(e => e.idGrupoEmpresa).AsQueryable<Operadora>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Operadora/Operadora
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                // Implementar o filtro por Grupo apartir do TOKEN do Usuário
                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.IDGRUPOEMPRESA, out outValue))
                        queryString["" + (int)CAMPOS.IDGRUPOEMPRESA] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.IDGRUPOEMPRESA, IdGrupo.ToString());
                }
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (!CnpjEmpresa.Equals(""))
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                        queryString["" + (int)CAMPOS.NU_CNPJ] = CnpjEmpresa;
                    else
                        queryString.Add("" + (int)CAMPOS.NU_CNPJ, CnpjEmpresa);
                }

                //DECLARAÇÕES
                List<dynamic> CollectionOperadora = new List<dynamic>();
                Retorno retorno = new Retorno();

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
                var queryTotal = query;

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = queryTotal.Count();


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
                    CollectionOperadora = query.Select(e => new
                    {

                        id = e.id,
                        nmOperadora = e.nmOperadora,
                        idGrupoEmpresa = e.idGrupoEmpresa,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionOperadora = query.Select(e => new
                    {

                        id = e.id,
                        nmOperadora = e.nmOperadora,
                        idGrupoEmpresa = e.idGrupoEmpresa,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionOperadora;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar operadora" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova Operadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Operadora param)
        {
            try
            {
                _db.Operadoras.Add(param);
                _db.SaveChanges();
                return param.id;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar operadora" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma Operadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            try
            {
                Operadora op = _db.Operadoras.Where(e => e.id == id).FirstOrDefault();
                if (op == null) throw new Exception("Operadora inexistente");
                _db.Operadoras.Remove(op);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar operadora" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera Operadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Operadora param)
        {
            try
            {
                Operadora value = _db.Operadoras
                        .Where(e => e.id == param.id)
                        .First<Operadora>();

                if (value == null) throw new Exception("Operadora inexistente");

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.id != null && param.id != value.id)
                    value.id = param.id;
                if (param.nmOperadora != null && param.nmOperadora != value.nmOperadora)
                    value.nmOperadora = param.nmOperadora;
                if (param.idGrupoEmpresa != null && param.idGrupoEmpresa != value.idGrupoEmpresa)
                    value.idGrupoEmpresa = param.idGrupoEmpresa;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar operadora" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
