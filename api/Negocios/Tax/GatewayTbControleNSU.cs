using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;

namespace api.Negocios.Tax
{
    public class GatewayTbControleNSU
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbControleNSU()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDCONTROLE = 100,
            NRCNPJ = 101,
            ULTNSU = 102,

        };

        /// <summary>
        /// Get TbControleNSU/TbControleNSU
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbControleNSU> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbControleNSUs.AsQueryable<tbControleNSU>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDCONTROLE:
                        Int32 idControle = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idControle.Equals(idControle)).AsQueryable<tbControleNSU>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbControleNSU>();
                        break;
                    case CAMPOS.ULTNSU:
                        string ultNSU = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ultNSU.Equals(ultNSU)).AsQueryable<tbControleNSU>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDCONTROLE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idControle).AsQueryable<tbControleNSU>();
                    else entity = entity.OrderByDescending(e => e.idControle).AsQueryable<tbControleNSU>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbControleNSU>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbControleNSU>();
                    break;
                case CAMPOS.ULTNSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ultNSU).AsQueryable<tbControleNSU>();
                    else entity = entity.OrderByDescending(e => e.ultNSU).AsQueryable<tbControleNSU>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbControleNSU/TbControleNSU
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {

                //DECLARAÇÕES
                List<dynamic> CollectionTbControleNSU = new List<dynamic>();
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
                    CollectionTbControleNSU = query.Select(e => new
                    {
                        idControle = e.idControle,
                        nrCNPJ = e.nrCNPJ,
                        ultNSU = e.ultNSU,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbControleNSU = query.Select(e => new
                    {
                        idControle = e.idControle,
                        nrCNPJ = e.nrCNPJ,
                        ultNSU = e.ultNSU,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbControleNSU;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar tbControleNSU" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbControleNSU
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbControleNSU param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbControleNSUs.Add(param);
                _db.SaveChanges();
                return param.idControle;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar tbControleNSU" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbControleNSU
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idControle)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                _db.tbControleNSUs.Remove(_db.tbControleNSUs.Where(e => e.idControle.Equals(idControle)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbManifesto" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbControleNSU
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbControleNSU param)
        {
            try
            {
                // Atualiza o contexto
                ((IObjectContextAdapter)_db).ObjectContext.Refresh(RefreshMode.StoreWins, _db.ChangeTracker.Entries().Select(c => c.Entity));

                tbControleNSU value = _db.tbControleNSUs
                        .Where(e => e.nrCNPJ.Equals(param.nrCNPJ))
                        .First<tbControleNSU>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS

                if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                    value.nrCNPJ = param.nrCNPJ;
                if (param.ultNSU != null && param.ultNSU != value.ultNSU)
                    value.ultNSU = param.ultNSU;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar tbControleNSU" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
