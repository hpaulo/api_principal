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
		public class GatewaytbNew
		{
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewaytbNew()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
                IDNEWS = 100,
                DSNEWS = 101,
                DTNEWS = 102,
                CDEMPRESAGRUPO = 103,
                CDCATALOGO = 104,
                CDCANAL = 105,
                CDREPORTER = 106,
                DTENVIO = 107,

       };

        /// <summary>
        /// Get tbNew/tbNew
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbNew> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbNews.AsQueryable();

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
									entity = entity.Where(e => e.idNews.Equals(idNews)).AsQueryable<tbNew>();
								break;
								case CAMPOS.DSNEWS:
									string dsNews = Convert.ToString(item.Value);
									entity = entity.Where(e => e.dsNews.Equals(dsNews)).AsQueryable<tbNew>();
								break;
								case CAMPOS.DTNEWS:
									DateTime dtNews = Convert.ToDateTime(item.Value);
									entity = entity.Where(e => e.dtNews.Equals(dtNews)).AsQueryable<tbNew>();
								break;
								case CAMPOS.CDEMPRESAGRUPO:
									Int32 cdEmpresaGrupo = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.cdEmpresaGrupo.Equals(cdEmpresaGrupo)).AsQueryable<tbNew>();
								break;
								case CAMPOS.CDCATALOGO:
									short cdCatalogo = short.Parse(item.Value);
									entity = entity.Where(e => e.cdCatalogo.Equals(cdCatalogo)).AsQueryable<tbNew>();
								break;
								case CAMPOS.CDCANAL:
                                short cdCanal = short.Parse(item.Value);
									entity = entity.Where(e => e.cdCanal.Equals(cdCanal)).AsQueryable<tbNew>();
								break;
								case CAMPOS.CDREPORTER:
									string cdReporter = Convert.ToString(item.Value);
									entity = entity.Where(e => e.cdReporter.Equals(cdReporter)).AsQueryable<tbNew>();
								break;
								case CAMPOS.DTENVIO:
									DateTime dtEnvio = Convert.ToDateTime(item.Value);
									entity = entity.Where(e => e.dtEnvio.Equals(dtEnvio)).AsQueryable<tbNew>();
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
							if (orderby == 0)  entity = entity.OrderBy(e => e.idNews).AsQueryable<tbNew>();
							else entity = entity.OrderByDescending(e =>  e.idNews).AsQueryable<tbNew>();
						break;
						case CAMPOS.DSNEWS: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.dsNews).AsQueryable<tbNew>();
							else entity = entity.OrderByDescending(e =>  e.dsNews).AsQueryable<tbNew>();
						break;
						case CAMPOS.DTNEWS: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.dtNews).AsQueryable<tbNew>();
							else entity = entity.OrderByDescending(e =>  e.dtNews).AsQueryable<tbNew>();
						break;
						case CAMPOS.CDEMPRESAGRUPO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.cdEmpresaGrupo).AsQueryable<tbNew>();
							else entity = entity.OrderByDescending(e =>  e.cdEmpresaGrupo).AsQueryable<tbNew>();
						break;
						case CAMPOS.CDCATALOGO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.cdCatalogo).AsQueryable<tbNew>();
							else entity = entity.OrderByDescending(e =>  e.cdCatalogo).AsQueryable<tbNew>();
						break;
						case CAMPOS.CDCANAL: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.cdCanal).AsQueryable<tbNew>();
							else entity = entity.OrderByDescending(e =>  e.cdCanal).AsQueryable<tbNew>();
						break;
						case CAMPOS.CDREPORTER: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.cdReporter).AsQueryable<tbNew>();
							else entity = entity.OrderByDescending(e =>  e.cdReporter).AsQueryable<tbNew>();
						break;
						case CAMPOS.DTENVIO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.dtEnvio).AsQueryable<tbNew>();
							else entity = entity.OrderByDescending(e =>  e.dtEnvio).AsQueryable<tbNew>();
						break;

                }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna tbNew/tbNew
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
			try
			{   
				//DECLARAÇÕES
				List<dynamic> CollectiontbNew = new List<dynamic>();
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
					CollectiontbNew = query.Select(e => new
					{
	
						idNews = e.idNews,
						dsNews = e.dsNews,
						dtNews = e.dtNews,
						cdEmpresaGrupo = e.cdEmpresaGrupo,
						cdCatalogo = e.cdCatalogo,
						cdCanal = e.cdCanal,
						cdReporter = e.cdReporter,
						dtEnvio = e.dtEnvio,
					}).ToList<dynamic>();
				}
				else if (colecao == 0)
				{
					CollectiontbNew = query.Select(e => new
					{
	
						idNews = e.idNews,
						dsNews = e.dsNews,
						dtNews = e.dtNews,
						cdEmpresaGrupo = e.cdEmpresaGrupo,
						cdCatalogo = e.cdCatalogo,
						cdCanal = e.cdCanal,
						cdReporter = e.cdReporter,
						dtEnvio = e.dtEnvio,
					}).ToList<dynamic>();
				}

				retorno.Registros = CollectiontbNew;

				return retorno;
			}
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar tbNew" : erro);
                }
                throw new Exception(e.Message);
            }
        }


	
        /// <summary>
        /// Adiciona nova tbNew
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbNew param)
        {
			try
			{
			     // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				_db.tbNews.Add(param);
				_db.SaveChanges();
				return param.idNews;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar tbNew" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma tbNew
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idNews)
        {
            try
            {
            	// Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				_db.tbNews.Remove(_db.tbNews.Where(e => e.idNews.Equals(idNews)).First());
				_db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar tbNew" : erro);
                }
                throw new Exception(e.Message);
            }
        }
	


        /// <summary>
        /// Altera tbNew
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbNew param)
        {
			try
			{
				// Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));
                
				tbNew value = _db.tbNews
						.Where(e => e.idNews.Equals(param.idNews))
						.First<tbNew>();

				// OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS
	            
				
					if (param.idNews != null && param.idNews != value.idNews)
						value.idNews = param.idNews;
					if (param.dsNews != null && param.dsNews != value.dsNews)
						value.dsNews = param.dsNews;
					if (param.dtNews != null && param.dtNews != value.dtNews)
						value.dtNews = param.dtNews;
					if (param.cdEmpresaGrupo != null && param.cdEmpresaGrupo != value.cdEmpresaGrupo)
						value.cdEmpresaGrupo = param.cdEmpresaGrupo;
					if (param.cdCatalogo != null && param.cdCatalogo != value.cdCatalogo)
						value.cdCatalogo = param.cdCatalogo;
					if (param.cdCanal != null && param.cdCanal != value.cdCanal)
						value.cdCanal = param.cdCanal;
					if (param.cdReporter != null && param.cdReporter != value.cdReporter)
						value.cdReporter = param.cdReporter;
					if (param.dtEnvio != null && param.dtEnvio != value.dtEnvio)
						value.dtEnvio = param.dtEnvio;
				_db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar tbNew" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}