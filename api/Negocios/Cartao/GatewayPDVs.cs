using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Cartao
{
    public class GatewayPDVs
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayPDVs()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDPDV = 100,
            CNPJJFILIAL = 101,
            DECRICAOPDV = 102,
            CODPDVERP = 103,
            CDEMPRESATEF = 104,
            CODPDVHOSTPAGAMENTO = 105,
            CDGRUPO = 106,

        };

        /// <summary>
        /// Get PDV/PDV
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<PDV> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.PDVs.AsQueryable<PDV>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDPDV:
                        Int32 IdPDV = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.IdPDV == IdPDV).AsQueryable<PDV>();
                        break;
                    case CAMPOS.CNPJJFILIAL:
                        string CNPJjFilial = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.CNPJjFilial.Equals(CNPJjFilial)).AsQueryable<PDV>();
                        break;
                    case CAMPOS.DECRICAOPDV:
                        string DecricaoPdv = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.DecricaoPdv.Equals(DecricaoPdv)).AsQueryable<PDV>();
                        break;
                    case CAMPOS.CODPDVERP:
                        string CodPdvERP = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.CodPdvERP.Equals(CodPdvERP)).AsQueryable<PDV>();
                        break;
                    case CAMPOS.CDEMPRESATEF:
                        string cdEmpresaTEF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdEmpresaTEF.Equals(cdEmpresaTEF)).AsQueryable<PDV>();
                        break;
                    case CAMPOS.CODPDVHOSTPAGAMENTO:
                        string CodPdvHostPagamento = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.CodPdvHostPagamento.Equals(CodPdvHostPagamento)).AsQueryable<PDV>();
                        break;
                    case CAMPOS.CDGRUPO:
                        byte cdGrupo = Convert.ToByte(item.Value);
                        //entity = entity.Where(e => e.cdGrupo.Equals(cdGrupo)).AsQueryable<PDV>();
                        entity = entity.Where(e => e.empresa.id_grupo == cdGrupo).AsQueryable<PDV>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDPDV:
                    if (orderby == 0) entity = entity.OrderBy(e => e.IdPDV).AsQueryable<PDV>();
                    else entity = entity.OrderByDescending(e => e.IdPDV).AsQueryable<PDV>();
                    break;
                case CAMPOS.CNPJJFILIAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.CNPJjFilial).AsQueryable<PDV>();
                    else entity = entity.OrderByDescending(e => e.CNPJjFilial).AsQueryable<PDV>();
                    break;
                case CAMPOS.DECRICAOPDV:
                    if (orderby == 0) entity = entity.OrderBy(e => e.DecricaoPdv).AsQueryable<PDV>();
                    else entity = entity.OrderByDescending(e => e.DecricaoPdv).AsQueryable<PDV>();
                    break;
                case CAMPOS.CODPDVERP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.CodPdvERP).AsQueryable<PDV>();
                    else entity = entity.OrderByDescending(e => e.CodPdvERP).AsQueryable<PDV>();
                    break;
                case CAMPOS.CDEMPRESATEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEmpresaTEF).AsQueryable<PDV>();
                    else entity = entity.OrderByDescending(e => e.cdEmpresaTEF).AsQueryable<PDV>();
                    break;
                case CAMPOS.CODPDVHOSTPAGAMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.CodPdvHostPagamento).AsQueryable<PDV>();
                    else entity = entity.OrderByDescending(e => e.CodPdvHostPagamento).AsQueryable<PDV>();
                    break;
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdGrupo).AsQueryable<PDV>();
                    else entity = entity.OrderByDescending(e => e.cdGrupo).AsQueryable<PDV>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna PDV/PDV
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionPDV = new List<dynamic>();
                Retorno retorno = new Retorno();

                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.CDGRUPO, out outValue))
                        queryString["" + (int)CAMPOS.CDGRUPO] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.CDGRUPO, IdGrupo.ToString());
                }
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (!CnpjEmpresa.Equals(""))
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.CNPJJFILIAL, out outValue))
                        queryString["" + (int)CAMPOS.CNPJJFILIAL] = CnpjEmpresa;
                    else
                        queryString.Add("" + (int)CAMPOS.CNPJJFILIAL, CnpjEmpresa);
                }

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);


                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = query.Count();


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
                    CollectionPDV = query.Select(e => new
                    {

                        IdPDV = e.IdPDV,
                        CNPJjFilial = e.CNPJjFilial,
                        DecricaoPdv = e.DecricaoPdv,
                        CodPdvERP = e.CodPdvERP,
                        cdEmpresaTEF = e.cdEmpresaTEF,
                        CodPdvHostPagamento = e.CodPdvHostPagamento,
                        cdGrupo = e.cdGrupo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionPDV = query.Select(e => new
                    {

                        IdPDV = e.IdPDV,
                        CNPJjFilial = e.CNPJjFilial,
                        DecricaoPdv = e.DecricaoPdv,
                        CodPdvERP = e.CodPdvERP,
                        cdEmpresaTEF = e.cdEmpresaTEF,
                        CodPdvHostPagamento = e.CodPdvHostPagamento,
                        cdGrupo = e.cdGrupo,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionPDV = query.GroupBy(e => e.CodPdvHostPagamento)
                        .Select(e => new
                    {
                        CodPdvHostPagamento = e.Key,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionPDV;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao consultar pdvs" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova PDV
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, PDV param)
        {
            try
            {
                _db.PDVs.Add(param);
                _db.SaveChanges();
                return param.IdPDV;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar adicionar pdv" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma PDV
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 IdPDV)
        {
            try
            {
                _db.PDVs.Remove(_db.PDVs.Where(e => e.IdPDV == IdPDV).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar remover pdv" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera PDV
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, PDV param)
        {
            try
            {
                PDV value = _db.PDVs
                                .Where(e => e.IdPDV == param.IdPDV)
                                .First<PDV>();


                if (param.CNPJjFilial != null && param.CNPJjFilial != value.CNPJjFilial)
                    value.CNPJjFilial = param.CNPJjFilial;
                if (param.DecricaoPdv != null && param.DecricaoPdv != value.DecricaoPdv)
                    value.DecricaoPdv = param.DecricaoPdv;
                if (param.CodPdvERP != null && param.CodPdvERP != value.CodPdvERP)
                    value.CodPdvERP = param.CodPdvERP;
                if (param.cdEmpresaTEF != null && param.cdEmpresaTEF != value.cdEmpresaTEF)
                    value.cdEmpresaTEF = param.cdEmpresaTEF;
                if (param.CodPdvHostPagamento != null && param.CodPdvHostPagamento != value.CodPdvHostPagamento)
                    value.CodPdvHostPagamento = param.CodPdvHostPagamento;
                if (param.cdGrupo != null && param.cdGrupo != value.cdGrupo)
                    value.cdGrupo = param.cdGrupo;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar atualizar pdv" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
