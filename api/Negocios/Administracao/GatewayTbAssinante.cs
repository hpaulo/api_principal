using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace api.Negocios.Admin
{
    public class GatewayTbAssinante
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbAssinante()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDCATALOGO = 100,
            ID_USERS = 101,

        };

        /// <summary>
        /// Get TbAssinante/TbAssinante
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbAssinante> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbAssinantes.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.CDCATALOGO:
                        short cdCatalogo = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdCatalogo.Equals(cdCatalogo)).AsQueryable<tbAssinante>();
                        break;
                    case CAMPOS.ID_USERS:
                        Int32 id_users = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_users.Equals(id_users)).AsQueryable<tbAssinante>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.CDCATALOGO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdCatalogo).AsQueryable<tbAssinante>();
                    else entity = entity.OrderByDescending(e => e.cdCatalogo).AsQueryable<tbAssinante>();
                    break;
                case CAMPOS.ID_USERS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_users).AsQueryable<tbAssinante>();
                    else entity = entity.OrderByDescending(e => e.id_users).AsQueryable<tbAssinante>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbAssinante/TbAssinante
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbAssinante = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

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
                    CollectionTbAssinante = query.Select(e => new
                    {

                        cdCatalogo = e.cdCatalogo,
                        id_users = e.id_users,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbAssinante = query.Select(e => new
                    {

                        cdCatalogo = e.cdCatalogo,
                        id_users = e.id_users,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbAssinante;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbAssinante" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbAssinante
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbAssinante param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbAssinantes.Add(param);
                _db.SaveChanges();
                return param.cdCatalogo;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbAssinante" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbAssinante
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdCatalogo, int id_users)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbAssinantes.Remove(_db.tbAssinantes.Where(e => e.cdCatalogo.Equals(cdCatalogo) && e.id_users.Equals(id_users)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbAssinante" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbAssinante
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, List<Assinante> param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
                foreach (Assinante p in param)
                {
                    // Se encontrar no banco e o usuário desabilitar, delete o registro
                    tbAssinante valor = _db.tbAssinantes
                                                    .Where(a => a.cdCatalogo == p.cdCatalogo && a.id_users == p.id_users)
                                                    .FirstOrDefault();
                    if (valor != null)
                    {
                        if (!p.flInscrito)
                        {
                            _db.tbAssinantes.Remove(valor);
                            _db.SaveChanges();
                        }
                    }
                    // Se não encontrar e o usuário habilitar, grave o registro
                    else if(p.flInscrito)
                    {
                        valor = new tbAssinante();
                        valor.id_users = p.id_users;
                        valor.cdCatalogo = p.cdCatalogo;

                        _db.tbAssinantes.Add(valor);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbAssinante" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}