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
    public class GatewayBandeiraPos
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayBandeiraPos()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            DESBANDEIRA = 101,
            IDOPERADORA = 102,

        };

        /// <summary>
        /// Get BandeiraPos/BandeiraPos
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<BandeiraPos> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.BandeiraPos.AsQueryable<BandeiraPos>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<BandeiraPos>();
                        break;
                    case CAMPOS.DESBANDEIRA:
                        string desBandeira = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.desBandeira.Equals(desBandeira)).AsQueryable<BandeiraPos>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<BandeiraPos>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<BandeiraPos>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<BandeiraPos>();
                    break;
                case CAMPOS.DESBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.desBandeira).AsQueryable<BandeiraPos>();
                    else entity = entity.OrderByDescending(e => e.desBandeira).AsQueryable<BandeiraPos>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<BandeiraPos>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<BandeiraPos>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna BandeiraPos/BandeiraPos
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionBandeiraPos = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Implementar o filtro por Grupo apartir do TOKEN do Usuário
                Int32 IdGrupo = 0;
                IdGrupo = Permissoes.GetIdGrupo(token);
                

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
                    CollectionBandeiraPos = query.Select(e => new
                    {

                        id = e.id,
                        desBandeira = e.desBandeira,
                        idOperadora = e.idOperadora,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionBandeiraPos = query.Select(e => new
                    {

                        id = e.id,
                        desBandeira = e.desBandeira,
                        idOperadora = e.idOperadora,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionBandeiraPos = query.Where(e => e.Operadora.idGrupoEmpresa == IdGrupo).Select(
                                                                                                            e => new { e.Operadora.nmOperadora }
                                                                                                            ).ToList<dynamic>();
                }

                retorno.Registros = CollectionBandeiraPos;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar bandeira pos" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Adiciona nova BandeiraPos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, BandeiraPos param)
        {
            try
            {
                _db.BandeiraPos.Add(param);
                _db.SaveChanges();
                return param.id;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar bandeira pos" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma BandeiraPos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            try
            {
                _db.BandeiraPos.Remove(_db.BandeiraPos.Where(e => e.id.Equals(id)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar bandeira pos" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Altera BandeiraPos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, BandeiraPos param)
        {
            try
            {
                BandeiraPos value = _db.BandeiraPos
                        .Where(e => e.id.Equals(param.id))
                        .First<BandeiraPos>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.id != null && param.id != value.id)
                    value.id = param.id;
                if (param.desBandeira != null && param.desBandeira != value.desBandeira)
                    value.desBandeira = param.desBandeira;
                if (param.idOperadora != null && param.idOperadora != value.idOperadora)
                    value.idOperadora = param.idOperadora;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar bandeira pos" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
