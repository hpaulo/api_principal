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
    public class GatewayTbCatalogo
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbCatalogo()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDCATALOGO = 100,
            DSCATALOGO = 101,

            // Relacionamentos
            ID_USERS = 201

        };

        /// <summary>
        /// Get TbCatalogo/TbCatalogo
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbCatalogo> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbCatalogos.AsQueryable();

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
                        entity = entity.Where(e => e.cdCatalogo.Equals(cdCatalogo)).AsQueryable<tbCatalogo>();
                        break;
                    case CAMPOS.DSCATALOGO:
                        string dsCatalogo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsCatalogo.Equals(dsCatalogo)).AsQueryable<tbCatalogo>();
                        break;

                    // RELACIONAMENTOS
                    /*
                    case CAMPOS.ID_USERS:
                        string id_users = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tbAssinantes).AsQueryable<tbCatalogo>();
                        break;
                    */


                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.CDCATALOGO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdCatalogo).AsQueryable<tbCatalogo>();
                    else entity = entity.OrderByDescending(e => e.cdCatalogo).AsQueryable<tbCatalogo>();
                    break;
                case CAMPOS.DSCATALOGO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsCatalogo).AsQueryable<tbCatalogo>();
                    else entity = entity.OrderByDescending(e => e.dsCatalogo).AsQueryable<tbCatalogo>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbCatalogo/TbCatalogo
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbCatalogo = new List<dynamic>();
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

                // Pegar id do usuário pelo token
                var idUsers = Permissoes.GetIdUser(token);

                // COLEÇÃO DE RETORNO
                if (colecao == 1)
                {
                    CollectionTbCatalogo = query.Select(e => new
                    {

                        cdCatalogo = e.cdCatalogo,
                        dsCatalogo = e.dsCatalogo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbCatalogo = query.Select(e => new
                    {

                        cdCatalogo = e.cdCatalogo,
                        dsCatalogo = e.dsCatalogo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionTbCatalogo = query
                        // .Where(e => _db.tbAssinantes.Where(a => a.id_users == idUsers).Select(a => a.cdCatalogo).Contains(e.cdCatalogo))
                        .Select(e => new
                        {

                            cdCatalogo = e.cdCatalogo,
                            dsCatalogo = e.dsCatalogo,
                            flInscrito = _db.tbAssinantes
                                            .Where(a => a.id_users == idUsers)
                                            .Select(a => a.cdCatalogo)
                                            .Contains(e.cdCatalogo)
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbCatalogo;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbCatalogo" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbCatalogo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbCatalogo param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbCatalogos.Add(param);
                _db.SaveChanges();
                return param.cdCatalogo;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbCatalogo" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbCatalogo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdCatalogo)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbCatalogos.Remove(_db.tbCatalogos.Where(e => e.cdCatalogo.Equals(cdCatalogo)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbCatalogo" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbCatalogo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbCatalogo param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbCatalogo value = _db.tbCatalogos
                        .Where(e => e.cdCatalogo.Equals(param.cdCatalogo))
                        .First<tbCatalogo>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.cdCatalogo != null && param.cdCatalogo != value.cdCatalogo)
                    value.cdCatalogo = param.cdCatalogo;
                if (param.dsCatalogo != null && param.dsCatalogo != value.dsCatalogo)
                    value.dsCatalogo = param.dsCatalogo;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbCatalogo" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}