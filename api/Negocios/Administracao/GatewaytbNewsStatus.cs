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
		public class GatewaytbNewsStatus
		{
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewaytbNewsStatus()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
                IDNEWS = 100,
                ID_USERS = 101,
                FLRECEBIDO = 102,
                FLLIDO = 103,

       };

        /// <summary>
        /// Get tbNewsStatu/tbNewsStatu
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbNewsStatus> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbNewsStatuss.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

                // ADICIONA OS FILTROS A QUERY
                foreach (var item in queryString)
                {
                    int key = Convert.ToInt16(item.Key);
                    CAMPOS filtroEnum = (CAMPOS)key;
                    switch (filtroEnum)
                    {
				

								case CAMPOS.IDNEWS:
									Int32 idNews = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.idNews.Equals(idNews)).AsQueryable<tbNewsStatus>();
								break;
								case CAMPOS.ID_USERS:
									Int32 id_users = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.id_users.Equals(id_users)).AsQueryable<tbNewsStatus>();
								break;
								case CAMPOS.FLRECEBIDO:
									Boolean flRecebido = Convert.ToBoolean(item.Value);
									entity = entity.Where(e => e.flRecebido.Equals(flRecebido)).AsQueryable<tbNewsStatus>();
								break;
								case CAMPOS.FLLIDO:
									Boolean flLido = item.Value.ToString() == "1" ? true : false;
                                    entity = entity.Where(e => e.flLido == flLido).AsQueryable<tbNewsStatus>();
								break;

                    }
                }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
                // ADICIONA A ORDENAÇÃO A QUERY
                CAMPOS filtro = (CAMPOS)campo;
                switch (filtro)
                {

						case CAMPOS.IDNEWS: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.idNews).AsQueryable<tbNewsStatus>();
							else entity = entity.OrderByDescending(e =>  e.idNews).AsQueryable<tbNewsStatus>();
						break;
						case CAMPOS.ID_USERS: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.id_users).AsQueryable<tbNewsStatus>();
							else entity = entity.OrderByDescending(e =>  e.id_users).AsQueryable<tbNewsStatus>();
						break;
						case CAMPOS.FLRECEBIDO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.flRecebido).AsQueryable<tbNewsStatus>();
							else entity = entity.OrderByDescending(e =>  e.flRecebido).AsQueryable<tbNewsStatus>();
						break;
						case CAMPOS.FLLIDO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.flLido).AsQueryable<tbNewsStatus>();
							else entity = entity.OrderByDescending(e =>  e.flLido).AsQueryable<tbNewsStatus>();
						break;

                }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna tbNewsStatu/tbNewsStatu
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
			try
			{   
				//DECLARAÇÕES
				List<dynamic> CollectiontbNewsStatu = new List<dynamic>();
				Retorno retorno = new Retorno();

                // Atualiza o contexto
                //((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                string outValue = null;
                Int32 idUsers = Permissoes.GetIdUser(token);
                //if(Permissoes.isAtosRole(token))
                if (queryString.TryGetValue("" + (int)CAMPOS.ID_USERS, out outValue))
                {
                    if (idUsers == 330) // Força o usuário IMESSENGER a acesso a todos os registros
                        queryString["" + (int)CAMPOS.ID_USERS] = outValue.ToString();
                    else
                        queryString["" + (int)CAMPOS.ID_USERS] = idUsers.ToString();
                }                
                else if (idUsers != 330) 
                    queryString.Add("" + (int)CAMPOS.ID_USERS, idUsers.ToString());

                // GET QUERY
                var query = getQuery( colecao, campo, orderBy, pageSize, pageNumber, queryString);
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
					CollectiontbNewsStatu = query.Select(e => new
					{
	
						idNews = e.idNews,
						id_users = e.id_users,
						flRecebido = e.flRecebido,
						flLido = e.flLido,
					}).ToList<dynamic>();
				}
				else if (colecao == 0)
				{
					CollectiontbNewsStatu = query.Select(e => new
					{
	
						idNews = e.idNews,
						id_users = e.id_users,
						flRecebido = e.flRecebido,
						flLido = e.flLido,
					}).ToList<dynamic>();
				}

				retorno.Registros = CollectiontbNewsStatu;

				return retorno;
			}
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar tbNewsStatu" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


	
        /// <summary>
        /// Adiciona nova tbNewsStatu
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbNewsStatus param)
        {
			try
			{
                // Atualiza o contexto
                //((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                using (var db = new painel_taxservices_dbContext())
                {
                    db.tbNewsStatuss.Add(param);
				    db.SaveChanges();
                    db.Dispose();
				    return param.idNews;
                }
                    
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar tbNewsStatu" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma tbNewsStatu
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idNews)
        {
            try
            {
            	// Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				_db.tbNewsStatuss.Remove(_db.tbNewsStatuss.Where(e => e.idNews.Equals(idNews)).First());
				_db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar tbNewsStatu" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
	


        /// <summary>
        /// Altera tbNewsStatu
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbNewsStatus param)
        {
			try
			{
				// Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                // token
                Int32 idUsers = Permissoes.GetIdUser(token);
                tbNewsStatus value;
                if (idUsers != 330)
                {
                    value = _db.tbNewsStatuss
                           .Where(e => e.idNews == param.idNews && e.id_users == idUsers)
                           .First<tbNewsStatus>();
                }
                else
                {
                    value = _db.tbNewsStatuss
                               .Where(e => e.idNews == param.idNews && e.id_users == param.id_users)
                               .First<tbNewsStatus>();
                }
                                 

				

				// OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS
	            
				
					//if (param.idNews != null && param.idNews != value.idNews)
					//	value.idNews = param.idNews;
					//if (param.id_users != null && param.id_users != value.id_users)
					//	value.id_users = param.id_users;
					if (param.flRecebido != null && param.flRecebido != value.flRecebido)
						value.flRecebido = param.flRecebido;
					if (param.flLido != null && param.flLido != value.flLido)
						value.flLido = param.flLido;
				_db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar tbNewsStatu" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}