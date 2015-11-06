using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Pos
{
    public class GatewayTerminalLogico
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTerminalLogico()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDTERMINALLOGICO = 100,
            DSTERMINALLOGICO = 101,
            IDOPERADORA = 102,

            // RELACIONAMENTOS
            IDGRUPOEMPRESA = 200,

            CDADQUIRENTE = 300
        };

        /// <summary>
        /// Get TerminalLogico/TerminalLogico
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<TerminalLogico> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.TerminalLogicoes.AsQueryable<TerminalLogico>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<TerminalLogico>();
                        break;
                    case CAMPOS.DSTERMINALLOGICO:
                        string dsTerminalLogico = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsTerminalLogico.Equals(dsTerminalLogico)).AsQueryable<TerminalLogico>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<TerminalLogico>();
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.CDADQUIRENTE: // Não vem a correta!
                    //    string outValue = null;
                    //    // tem que estar associado a um grupo empresa
                    //    if (!queryString.TryGetValue("" + (int)CAMPOS.IDGRUPOEMPRESA, out outValue)) continue;
                    //    Int32 idGrupoEmpresa = Convert.ToInt32(queryString["" + (int)CAMPOS.IDGRUPOEMPRESA]);
                    //    Int32 cdAdquirente = Convert.ToInt32(item.Value);
                    //    tbAdquirente tbAdquirente = _db.tbAdquirentes.Where(a => a.cdAdquirente == cdAdquirente).FirstOrDefault();
                    //    if (tbAdquirente == null) continue;
                    //    Operadora operadora = _db.Operadoras.Where(o => o.idGrupoEmpresa == idGrupoEmpresa)
                    //                                        .Where(o => o.nmOperadora.Equals(tbAdquirente.nmAdquirente))
                    //                                        .FirstOrDefault();
                    //    if (operadora == null) continue;
                    //    entity = entity.Where(e => e.idOperadora == operadora.id).AsQueryable<TerminalLogico>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<TerminalLogico>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<TerminalLogico>();
                    break;
                case CAMPOS.DSTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsTerminalLogico).AsQueryable<TerminalLogico>();
                    else entity = entity.OrderByDescending(e => e.dsTerminalLogico).AsQueryable<TerminalLogico>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<TerminalLogico>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<TerminalLogico>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TerminalLogico/TerminalLogico
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTerminalLogico = new List<dynamic>();
                Retorno retorno = new Retorno();

                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.IDGRUPOEMPRESA, out outValue))
                        queryString["" + (int)CAMPOS.IDGRUPOEMPRESA] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.IDGRUPOEMPRESA, IdGrupo.ToString());
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
                    CollectionTerminalLogico = query.Select(e => new
                    {

                        idTerminalLogico = e.idTerminalLogico,
                        dsTerminalLogico = e.dsTerminalLogico,
                        idOperadora = e.idOperadora,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTerminalLogico = query.Select(e => new
                    {

                        idTerminalLogico = e.idTerminalLogico,
                        dsTerminalLogico = e.dsTerminalLogico,
                        idOperadora = e.idOperadora,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionTerminalLogico = query.Select(e => new
                    {

                        idTerminalLogico = e.idTerminalLogico,
                        dsTerminalLogico = e.dsTerminalLogico.Equals("0") ? "-" : e.dsTerminalLogico,
                        idOperadora = e.idOperadora,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTerminalLogico;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar terminal logico" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }

        }
        /// <summary>
        /// Adiciona nova TerminalLogico
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, TerminalLogico param)
        {
            try
            {
                _db.TerminalLogicoes.Add(param);
                _db.SaveChanges();
                return param.idTerminalLogico;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar terminal logico" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TerminalLogico
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idTerminalLogico)
        {
            try
            {
                _db.TerminalLogicoes.Remove(_db.TerminalLogicoes.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar terminal logico" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera TerminalLogico
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, TerminalLogico param)
        {
            try
            {
                TerminalLogico value = _db.TerminalLogicoes
                        .Where(e => e.idTerminalLogico.Equals(param.idTerminalLogico))
                        .First<TerminalLogico>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idTerminalLogico != null && param.idTerminalLogico != value.idTerminalLogico)
                    value.idTerminalLogico = param.idTerminalLogico;
                if (param.dsTerminalLogico != null && param.dsTerminalLogico != value.dsTerminalLogico)
                    value.dsTerminalLogico = param.dsTerminalLogico;
                if (param.idOperadora != null && param.idOperadora != value.idOperadora)
                    value.idOperadora = param.idOperadora;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar terminal logico" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
