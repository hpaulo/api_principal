using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Admin
{
    public class GatewayTbEmpresaGrupo
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbEmpresaGrupo()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDEMPRESAGRUPO = 100,
            DSEMPRESAGRUPO = 101,
            DTCADASTRO = 102,
            FLCARDSERVICES = 103,
            FLTAXSERVICES = 104,
            FLPROINFO = 105,
            CDVENDEDOR = 106,
            FLATIVO = 107,

        };

        /// <summary>
        /// Get TbEmpresaGrupo/TbEmpresaGrupo
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbEmpresaGrupo> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbEmpresaGrupos.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.CDEMPRESAGRUPO:
                        Int32 cdEmpresaGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdEmpresaGrupo.Equals(cdEmpresaGrupo)).AsQueryable();
                        break;
                    case CAMPOS.DSEMPRESAGRUPO:
                        string dsEmpresaGrupo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsEmpresaGrupo.Equals(dsEmpresaGrupo)).AsQueryable();
                        break;
                    case CAMPOS.DTCADASTRO:
                        DateTime dtCadastro = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtCadastro.Equals(dtCadastro)).AsQueryable();
                        break;
                    case CAMPOS.FLCARDSERVICES:
                        Boolean flCardServices = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flCardServices.Equals(flCardServices)).AsQueryable();
                        break;
                    case CAMPOS.FLTAXSERVICES:
                        Boolean flTaxServices = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flTaxServices.Equals(flTaxServices)).AsQueryable();
                        break;
                    case CAMPOS.FLPROINFO:
                        Boolean flProinfo = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flProinfo.Equals(flProinfo)).AsQueryable();
                        break;
                    case CAMPOS.CDVENDEDOR:
                        Int32 cdVendedor = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdVendedor.Equals(cdVendedor)).AsQueryable();
                        break;
                    case CAMPOS.FLATIVO:
                        Boolean flAtivo = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flAtivo.Equals(flAtivo)).AsQueryable();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.CDEMPRESAGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEmpresaGrupo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdEmpresaGrupo).AsQueryable();
                    break;
                case CAMPOS.DSEMPRESAGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsEmpresaGrupo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsEmpresaGrupo).AsQueryable();
                    break;
                case CAMPOS.DTCADASTRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtCadastro).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtCadastro).AsQueryable();
                    break;
                case CAMPOS.FLCARDSERVICES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flCardServices).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.flCardServices).AsQueryable();
                    break;
                case CAMPOS.FLTAXSERVICES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flTaxServices).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.flTaxServices).AsQueryable();
                    break;
                case CAMPOS.FLPROINFO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flProinfo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.flProinfo).AsQueryable();
                    break;
                case CAMPOS.CDVENDEDOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdVendedor).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdVendedor).AsQueryable();
                    break;
                case CAMPOS.FLATIVO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flAtivo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.flAtivo).AsQueryable();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbEmpresaGrupo/TbEmpresaGrupo
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbEmpresaGrupo = new List<dynamic>();
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
                    CollectionTbEmpresaGrupo = query.Select(e => new
                    {

                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                        dsEmpresaGrupo = e.dsEmpresaGrupo,
                        dtCadastro = e.dtCadastro,
                        flCardServices = e.flCardServices,
                        flTaxServices = e.flTaxServices,
                        flProinfo = e.flProinfo,
                        cdVendedor = e.cdVendedor,
                        flAtivo = e.flAtivo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbEmpresaGrupo = query.Select(e => new
                    {

                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                        dsEmpresaGrupo = e.dsEmpresaGrupo,
                        dtCadastro = e.dtCadastro,
                        flCardServices = e.flCardServices,
                        flTaxServices = e.flTaxServices,
                        flProinfo = e.flProinfo,
                        cdVendedor = e.cdVendedor,
                        flAtivo = e.flAtivo,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbEmpresaGrupo;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbEmpresaGrupo" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbEmpresaGrupo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbEmpresaGrupo param)
        {
            try
            {
                _db.tbEmpresaGrupos.Add(param);
                _db.SaveChanges();
                return param.cdEmpresaGrupo;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbEmpresaGrupo" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbEmpresaGrupo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdEmpresaGrupo)
        {
            try
            {
                _db.tbEmpresaGrupos.Remove(_db.tbEmpresaGrupos.Where(e => e.cdEmpresaGrupo.Equals(cdEmpresaGrupo)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbEmpresaGrupo" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Altera tbEmpresaGrupo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbEmpresaGrupo param)
        {
            try
            {
                tbEmpresaGrupo value = _db.tbEmpresaGrupos
                        .Where(e => e.cdEmpresaGrupo.Equals(param.cdEmpresaGrupo))
                        .First<tbEmpresaGrupo>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.cdEmpresaGrupo != null && param.cdEmpresaGrupo != value.cdEmpresaGrupo)
                    value.cdEmpresaGrupo = param.cdEmpresaGrupo;
                if (param.dsEmpresaGrupo != null && param.dsEmpresaGrupo != value.dsEmpresaGrupo)
                    value.dsEmpresaGrupo = param.dsEmpresaGrupo;
                if (param.dtCadastro != null && param.dtCadastro != value.dtCadastro)
                    value.dtCadastro = param.dtCadastro;
                if (param.flCardServices != null && param.flCardServices != value.flCardServices)
                    value.flCardServices = param.flCardServices;
                if (param.flTaxServices != null && param.flTaxServices != value.flTaxServices)
                    value.flTaxServices = param.flTaxServices;
                if (param.flProinfo != null && param.flProinfo != value.flProinfo)
                    value.flProinfo = param.flProinfo;
                if (param.cdVendedor != null && param.cdVendedor != value.cdVendedor)
                    value.cdVendedor = param.cdVendedor;
                if (param.flAtivo != null && param.flAtivo != value.flAtivo)
                    value.flAtivo = param.flAtivo;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbEmpresaGrupo" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
