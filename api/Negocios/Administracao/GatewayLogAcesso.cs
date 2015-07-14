using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Administracao
{
		public class GatewayLogAcesso
		{
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayLogAcesso()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
                IDUSERS = 100,
                IDCONTROLLER = 101,
                IDMETHOD = 102,
                DTACESSO = 103,

       };

        /// <summary>
        /// Get LogAcesso/LogAcesso
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<LogAcesso1> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.LogAcesso1.AsQueryable<LogAcesso1>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

                // ADICIONA OS FILTROS A QUERY
                foreach (var item in queryString)
                {
                    int key = Convert.ToInt16(item.Key);
                    CAMPOS filtroEnum = (CAMPOS)key;
                    switch (filtroEnum)
                    {
				

								case CAMPOS.IDUSERS:
									Int32 idUsers = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.idUsers.Equals(idUsers)).AsQueryable<LogAcesso1>();
								break;
								case CAMPOS.IDCONTROLLER:
									Int32 idController = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.idController.Equals(idController)).AsQueryable<LogAcesso1>();
								break;
								case CAMPOS.IDMETHOD:
									Int32 idMethod = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.idMethod.Equals(idMethod)).AsQueryable<LogAcesso1>();
								break;
								case CAMPOS.DTACESSO:
									DateTime dtAcesso = Convert.ToDateTime(item.Value);
									entity = entity.Where(e => e.dtAcesso.Equals(dtAcesso)).AsQueryable<LogAcesso1>();
								break;

                    }
                }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
                // ADICIONA A ORDENAÇÃO A QUERY
                CAMPOS filtro = (CAMPOS)campo;
                switch (filtro)
                {

						case CAMPOS.IDUSERS: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.idUsers).AsQueryable<LogAcesso1>();
							else entity = entity.OrderByDescending(e =>  e.idUsers).AsQueryable<LogAcesso1>();
						break;
						case CAMPOS.IDCONTROLLER: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.idController).AsQueryable<LogAcesso1>();
							else entity = entity.OrderByDescending(e =>  e.idController).AsQueryable<LogAcesso1>();
						break;
						case CAMPOS.IDMETHOD: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.idMethod).AsQueryable<LogAcesso1>();
							else entity = entity.OrderByDescending(e =>  e.idMethod).AsQueryable<LogAcesso1>();
						break;
						case CAMPOS.DTACESSO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.dtAcesso).AsQueryable<LogAcesso1>();
							else entity = entity.OrderByDescending(e =>  e.dtAcesso).AsQueryable<LogAcesso1>();
						break;

                }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna LogAcesso/LogAcesso
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionLogAcesso = new List<dynamic>();
            Retorno retorno = new Retorno();

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
                CollectionLogAcesso = query.Select(e => new
                {

					idUsers = e.idUsers,
					idController = e.idController,
					idMethod = e.idMethod,
					dtAcesso = e.dtAcesso,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionLogAcesso = query.Select(e => new
                {

					idUsers = e.idUsers,
					idController = e.idController,
					idMethod = e.idMethod,
					dtAcesso = e.dtAcesso,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionLogAcesso;

            return retorno;
        }


	
        /// <summary>
        /// Adiciona nova LogAcesso
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, LogAcesso1 param)
        {
            _db.LogAcesso1.Add(param);
            _db.SaveChanges();
            return (Int32)param.idUsers;
        }


        /// <summary>
        /// Apaga uma LogAcesso
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idUsers, Int32 idController, Int32 idMethod)
        {
            _db.LogAcesso1.Remove(
                                    _db.LogAcesso1.Where(e => e.idUsers.Equals(idUsers))
                                                .Where(e => e.idController.Equals(idController))
                                                .Where(e => e.idMethod.Equals(idMethod)).First()
                                 );
            _db.SaveChanges();
        }
	


        /// <summary>
        /// Altera LogAcesso
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, LogAcesso1 param)
        {
            LogAcesso1 value = _db.LogAcesso1
                    .Where(e => e.idUsers.Equals(param.idUsers))
                    .Where(e => e.idController.Equals(param.idController))
                    .Where(e => e.idMethod.Equals(param.idMethod))
                    .First<LogAcesso1>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS
            
            
                if (param.idUsers != null && param.idUsers != value.idUsers)
					value.idUsers = param.idUsers;
                if (param.idController != null && param.idController != value.idController)
					value.idController = param.idController;
                if (param.idMethod != null && param.idMethod != value.idMethod)
					value.idMethod = param.idMethod;
                if (param.dtAcesso != null && param.dtAcesso != value.dtAcesso)
					value.dtAcesso = param.dtAcesso;
            _db.SaveChanges();

        }

    }
}