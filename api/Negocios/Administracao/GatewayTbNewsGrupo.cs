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
		public class GatewayTbNewsGrupo
		{
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbNewsGrupo()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
                CDNEWSGRUPO = 100,
                CDEMPRESAGRUPO = 101,
                DSNEWSGRUPO = 102,

       };

        /// <summary>
        /// Get tbNewsGrupo/tbNewsGrupo
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbNewsGrupo> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbNewsGrupos.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

                // ADICIONA OS FILTROS A QUERY
                foreach (var item in queryString)
                {
                    int key = Convert.ToInt16(item.Key);
                    CAMPOS filtroEnum = (CAMPOS)key;
                    switch (filtroEnum)
                    {
				

								case CAMPOS.CDNEWSGRUPO:
									Int32 cdNewsGrupo = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.cdNewsGrupo.Equals(cdNewsGrupo)).AsQueryable<tbNewsGrupo>();
								break;
								case CAMPOS.CDEMPRESAGRUPO:
									Int32 cdEmpresaGrupo = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.cdEmpresaGrupo.Equals(cdEmpresaGrupo)).AsQueryable<tbNewsGrupo>();
								break;
								case CAMPOS.DSNEWSGRUPO:
									string dsNewsGrupo = Convert.ToString(item.Value);
									entity = entity.Where(e => e.dsNewsGrupo.Equals(dsNewsGrupo)).AsQueryable<tbNewsGrupo>();
								break;

                    }
                }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
                // ADICIONA A ORDENAÇÃO A QUERY
                CAMPOS filtro = (CAMPOS)campo;
                switch (filtro)
                {

						case CAMPOS.CDNEWSGRUPO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.cdNewsGrupo).AsQueryable<tbNewsGrupo>();
							else entity = entity.OrderByDescending(e =>  e.cdNewsGrupo).AsQueryable<tbNewsGrupo>();
						break;
						case CAMPOS.CDEMPRESAGRUPO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.cdEmpresaGrupo).AsQueryable<tbNewsGrupo>();
							else entity = entity.OrderByDescending(e =>  e.cdEmpresaGrupo).AsQueryable<tbNewsGrupo>();
						break;
						case CAMPOS.DSNEWSGRUPO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.dsNewsGrupo).AsQueryable<tbNewsGrupo>();
							else entity = entity.OrderByDescending(e =>  e.dsNewsGrupo).AsQueryable<tbNewsGrupo>();
						break;

                }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna tbNewsGrupo/tbNewsGrupo
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
			try
			{   
				//DECLARAÇÕES
				List<dynamic> CollectionTbNewsGrupo = new List<dynamic>();
				Retorno retorno = new Retorno();

                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

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
					CollectionTbNewsGrupo = query.Select(e => new
					{
	
						cdNewsGrupo = e.cdNewsGrupo,
						cdEmpresaGrupo = e.cdEmpresaGrupo,
						dsNewsGrupo = e.dsNewsGrupo,
					}).ToList<dynamic>();
				}
				else if (colecao == 0)
				{
					CollectionTbNewsGrupo = query.Select(e => new
					{
	
						cdNewsGrupo = e.cdNewsGrupo,
						cdEmpresaGrupo = e.cdEmpresaGrupo,
						dsNewsGrupo = e.dsNewsGrupo,
					}).ToList<dynamic>();
				}

				retorno.Registros = CollectionTbNewsGrupo;

				return retorno;
			}
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar tbNewsGrupo" : erro);
                }
                throw new Exception(e.Message);
            }
        }


	
        /// <summary>
        /// Adiciona nova tbNewsGrupo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbNewsGrupo param)
        {
			try
			{
			     // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				_db.tbNewsGrupos.Add(param);
				_db.SaveChanges();
				return param.cdNewsGrupo;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar tbNewsGrupo" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma tbNewsGrupo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdNewsGrupo)
        {
            try
            {
            	// Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				_db.tbNewsGrupos.Remove(_db.tbNewsGrupos.Where(e => e.cdNewsGrupo.Equals(cdNewsGrupo)).First());
				_db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar tbNewsGrupo" : erro);
                }
                throw new Exception(e.Message);
            }
        }
	


        /// <summary>
        /// Altera tbNewsGrupo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbNewsGrupo param)
        {
			try
			{
				// Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				tbNewsGrupo value = _db.tbNewsGrupos
						.Where(e => e.cdNewsGrupo.Equals(param.cdNewsGrupo))
						.First<tbNewsGrupo>();

				// OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS
	            
				
					if (param.cdNewsGrupo != null && param.cdNewsGrupo != value.cdNewsGrupo)
						value.cdNewsGrupo = param.cdNewsGrupo;
					if (param.cdEmpresaGrupo != null && param.cdEmpresaGrupo != value.cdEmpresaGrupo)
						value.cdEmpresaGrupo = param.cdEmpresaGrupo;
					if (param.dsNewsGrupo != null && param.dsNewsGrupo != value.dsNewsGrupo)
						value.dsNewsGrupo = param.dsNewsGrupo;
				_db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar tbNewsGrupo" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}