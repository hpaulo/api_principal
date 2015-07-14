﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Administracao
{
		public class GatewayWebpagesPermissions
		{
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesPermissions()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
                ID_ROLES = 100,
                ID_METHOD = 101,
                FL_PRINCIPAL = 102,

       };

        /// <summary>
        /// Get Webpages_Permissions/Webpages_Permissions
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_Permissions> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_Permissions.AsQueryable<webpages_Permissions>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

                // ADICIONA OS FILTROS A QUERY
                foreach (var item in queryString)
                {
                    int key = Convert.ToInt16(item.Key);
                    CAMPOS filtroEnum = (CAMPOS)key;
                    switch (filtroEnum)
                    {
				

								case CAMPOS.ID_ROLES:
									Int32 id_roles = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.id_roles.Equals(id_roles)).AsQueryable<webpages_Permissions>();
								break;
								case CAMPOS.ID_METHOD:
									Int32 id_method = Convert.ToInt32(item.Value);
									entity = entity.Where(e => e.id_method.Equals(id_method)).AsQueryable<webpages_Permissions>();
								break;
								case CAMPOS.FL_PRINCIPAL:
									Boolean fl_principal = Convert.ToBoolean(item.Value);
									entity = entity.Where(e => e.fl_principal.Equals(fl_principal)).AsQueryable<webpages_Permissions>();
								break;

                    }
                }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
                // ADICIONA A ORDENAÇÃO A QUERY
                CAMPOS filtro = (CAMPOS)campo;
                switch (filtro)
                {

						case CAMPOS.ID_ROLES: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.id_roles).AsQueryable<webpages_Permissions>();
							else entity = entity.OrderByDescending(e =>  e.id_roles).AsQueryable<webpages_Permissions>();
						break;
						case CAMPOS.ID_METHOD: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.id_method).AsQueryable<webpages_Permissions>();
							else entity = entity.OrderByDescending(e =>  e.id_method).AsQueryable<webpages_Permissions>();
						break;
						case CAMPOS.FL_PRINCIPAL: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.fl_principal).AsQueryable<webpages_Permissions>();
							else entity = entity.OrderByDescending(e =>  e.fl_principal).AsQueryable<webpages_Permissions>();
						break;

                }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_Permissions/Webpages_Permissions
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionWebpages_Permissions = new List<dynamic>();
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
                CollectionWebpages_Permissions = query.Select(e => new
                {

					id_roles = e.id_roles,
					id_method = e.id_method,
					fl_principal = e.fl_principal,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionWebpages_Permissions = query.Select(e => new
                {

					id_roles = e.id_roles,
					id_method = e.id_method,
					fl_principal = e.fl_principal,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionWebpages_Permissions;

            return retorno;
        }


	
        /// <summary>
        /// Adiciona nova Webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, webpages_Permissions param)
        {
            _db.webpages_Permissions.Add(param);
            _db.SaveChanges();
            return param.id_roles;
        }


        /// <summary>
        /// Apaga uma Webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id_roles)
        {
            _db.webpages_Permissions.RemoveRange(
                                                _db.webpages_Permissions.Where(e => e.id_roles == id_roles)
                                            );
            _db.SaveChanges();
        }

        /// <summary>
        /// Apaga uma Webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id_roles, Int32 id_method)
        {
            _db.webpages_Permissions.RemoveRange(
                                                    _db.webpages_Permissions.Where(e => e.id_roles.Equals(id_roles)).Where(e => e.id_method.Equals(id_method))
                                                );
            _db.SaveChanges();
        }


        /// <summary>
        /// Apaga uma Webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void DeleteMethod(string token, Int32 id_method)
        {
            _db.webpages_Permissions.RemoveRange(_db.webpages_Permissions.Where(e => e.id_method == id_method) );
            _db.SaveChanges();
        }


        /// <summary>
        /// Altera webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, webpages_Permissions param)
        {
            webpages_Permissions value = _db.webpages_Permissions
                    .Where(e => e.id_roles.Equals(param.id_roles))
                    .Where(e => e.id_method.Equals(param.id_method))
                    .First<webpages_Permissions>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS



            if (param.fl_principal != value.fl_principal)
                value.fl_principal = param.fl_principal;
            _db.SaveChanges();

        }



        /// <summary>
        /// Altera webpages_Permissions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Models.Object.RolesPermissions param)
        {
            if ((param.Inserir != null) && (param.Inserir.Count > 0))
            {
                foreach (var item in param.Inserir)
                {
                    _db.webpages_Permissions.Add(new webpages_Permissions { id_roles = param.Id_roles, id_method = (int)item });
                }
            }

            if ((param.Deletar != null) && (param.Deletar.Count > 0))
            {
                foreach (var item in param.Inserir)
                {
                    _db.webpages_Permissions.Remove(
                                                        _db.webpages_Permissions
                                                        .Where(e => e.id_roles == param.Id_roles)
                                                        .Where(e => e.id_method == item).First()
                                            );
                }
            }
            _db.SaveChanges();
            // set o controller principal
            if (param.Id_controller_principal != null){
                // procura por um possível controller principal
                List<webpages_Permissions> permissoes = _db.webpages_Permissions
                    .Where(p => p.id_roles == param.Id_roles)
                    .Where(p => p.fl_principal == true).ToList<webpages_Permissions>();
                foreach (var permissao in permissoes)
                {
                    permissao.fl_principal = false;
                    Update(token, permissao);
                }

                // set os metodos do controller para ser o principal
                permissoes = _db.webpages_Permissions
                    .Where(p => p.id_roles == param.Id_roles)
                    .Where(p => p.webpages_Methods.id_controller == param.Id_controller_principal).ToList<webpages_Permissions>();
                foreach (var permissao in permissoes)
                {
                    permissao.fl_principal = true;
                    Update(token, permissao);
                }
            }
            

            // set o controller principal
            if (param.Id_controller_principal != null)
            {
                // procura por um possível controller principal
                List<webpages_Permissions> permissoes = _db.webpages_Permissions
                    .Where(p => p.id_roles == param.Id_roles)
                    .Where(p => p.fl_principal == true).ToList<webpages_Permissions>();
                foreach (var permissao in permissoes)
                {
                    permissao.fl_principal = false;
                    Update(token, permissao);
                }

                // set os metodos do controller para ser o principal
                permissoes = _db.webpages_Permissions
                    .Where(p => p.id_roles == param.Id_roles)
                    .Where(p => p.webpages_Methods.id_controller == param.Id_controller_principal).ToList<webpages_Permissions>();
                foreach (var permissao in permissoes)
                {
                    permissao.fl_principal = true;
                    Update(token, permissao);
                }
            }
        }

    }
}
