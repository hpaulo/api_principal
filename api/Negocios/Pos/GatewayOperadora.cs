using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

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

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            NMOPERADORA = 101,
            IDGRUPOEMPRESA = 102,

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
            var entity = _db.Operadoras.AsQueryable<Operadora>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<Operadora>();
                        break;
                    case CAMPOS.NMOPERADORA:
                        string nmOperadora = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmOperadora.Equals(nmOperadora)).AsQueryable<Operadora>();
                        break;
                    case CAMPOS.IDGRUPOEMPRESA:
                        Int32 idGrupoEmpresa = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idGrupoEmpresa.Equals(idGrupoEmpresa)).AsQueryable<Operadora>();
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
        /// <summary>
        /// Adiciona nova Operadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Operadora param)
        {
            _db.Operadoras.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma Operadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.Operadoras.Remove(_db.Operadoras.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera Operadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Operadora param)
        {
            Operadora value = _db.Operadoras
                    .Where(e => e.id.Equals(param.id))
                    .First<Operadora>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.nmOperadora != null && param.nmOperadora != value.nmOperadora)
                value.nmOperadora = param.nmOperadora;
            if (param.idGrupoEmpresa != null && param.idGrupoEmpresa != value.idGrupoEmpresa)
                value.idGrupoEmpresa = param.idGrupoEmpresa;
            _db.SaveChanges();

        }

    }
}
