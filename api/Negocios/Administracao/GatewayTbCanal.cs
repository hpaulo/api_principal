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
		public class GatewayTbCanal
		{
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbCanal()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
                CDCANAL = 100,
                DSCANAL = 101,

       };

        /// <summary>
        /// Get TbCanal/TbCanal
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbCanal> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbCanals.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

                // ADICIONA OS FILTROS A QUERY
                foreach (var item in queryString)
                {
                    int key = Convert.ToInt16(item.Key);
                    CAMPOS filtroEnum = (CAMPOS)key;
                    switch (filtroEnum)
                    {
				

								case CAMPOS.CDCANAL:
									short cdCanal = short.Parse(item.Value);
									entity = entity.Where(e => e.cdCanal.Equals(cdCanal)).AsQueryable<tbCanal>();
								break;
								case CAMPOS.DSCANAL:
									string dsCanal = Convert.ToString(item.Value);
									entity = entity.Where(e => e.dsCanal.Equals(dsCanal)).AsQueryable<tbCanal>();
								break;

                    }
                }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
                // ADICIONA A ORDENAÇÃO A QUERY
                CAMPOS filtro = (CAMPOS)campo;
                switch (filtro)
                {

						case CAMPOS.CDCANAL: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.cdCanal).AsQueryable<tbCanal>();
							else entity = entity.OrderByDescending(e =>  e.cdCanal).AsQueryable<tbCanal>();
						break;
						case CAMPOS.DSCANAL: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.dsCanal).AsQueryable<tbCanal>();
							else entity = entity.OrderByDescending(e =>  e.dsCanal).AsQueryable<tbCanal>();
						break;

                }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbCanal/TbCanal
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
			try
			{   
				//DECLARAÇÕES
				List<dynamic> CollectionTbCanal = new List<dynamic>();
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
					CollectionTbCanal = query.Select(e => new
					{
	
						cdCanal = e.cdCanal,
						dsCanal = e.dsCanal,
					}).ToList<dynamic>();
				}
				else if (colecao == 0)
				{
					CollectionTbCanal = query.Select(e => new
					{
	
						cdCanal = e.cdCanal,
						dsCanal = e.dsCanal,
					}).ToList<dynamic>();
				}

				retorno.Registros = CollectionTbCanal;

				return retorno;
			}
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbCanal" : erro);
                }
                throw new Exception(e.Message);
            }
        }


	
        /// <summary>
        /// Adiciona nova TbCanal
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbCanal param)
        {
			try
			{
			     // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				_db.tbCanals.Add(param);
				_db.SaveChanges();
				return param.cdCanal;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbCanal" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbCanal
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdCanal)
        {
            try
            {
            	// Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				_db.tbCanals.Remove(_db.tbCanals.Where(e => e.cdCanal.Equals(cdCanal)).First());
				_db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbCanal" : erro);
                }
                throw new Exception(e.Message);
            }
        }
	


        /// <summary>
        /// Altera tbCanal
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbCanal param)
        {
			try
			{
				// Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				tbCanal value = _db.tbCanals
						.Where(e => e.cdCanal.Equals(param.cdCanal))
						.First<tbCanal>();

				// OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS
	            
				
					if (param.cdCanal != null && param.cdCanal != value.cdCanal)
						value.cdCanal = param.cdCanal;
					if (param.dsCanal != null && param.dsCanal != value.dsCanal)
						value.dsCanal = param.dsCanal;
				_db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbCanal" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}