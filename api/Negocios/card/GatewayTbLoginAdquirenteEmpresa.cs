using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Card
{
    public class GatewayTbLoginAdquirenteEmpresa
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbLoginAdquirenteEmpresa()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDLOGINADQUIRENTEEMPRESA = 100,
            CDADQUIRENTE = 101,
            CDGRUPO = 102,
            NRCNPJ = 103,
            DSLOGIN = 104,
            DSSENHA = 105,
            CDESTABELECIMENTO = 106,
            DTALTERACAO = 107,
            STLOGINADQUIRENTE = 108,
            STLOGINADQUIRENTEEMPRESA = 109,

            // TBADQUIRENTE
            DSADQUIRENTE = 202,

            // EMPRESA
            DS_FANTASIA = 304,

        };

        /// <summary>
        /// Get TbAdquirente/TbAdquirente
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbLoginAdquirenteEmpresa> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbLoginAdquirenteEmpresas.AsQueryable<tbLoginAdquirenteEmpresa>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDLOGINADQUIRENTEEMPRESA:
                        Int32 cdLoginAdquirenteEmpresa = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente == cdLoginAdquirenteEmpresa).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente == cdAdquirente).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.CDGRUPO:
                        Int32 cdGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdGrupo == cdGrupo).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCnpj.Equals(nrCnpj)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.DSLOGIN:
                        string dsLogin = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsLogin.Equals(dsLogin)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.DSSENHA:
                        string dsSenha = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsSenha.Equals(dsSenha)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.CDESTABELECIMENTO:
                        string cdEstabelecimento = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdEstabelecimento.Equals(cdEstabelecimento)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.DTALTERACAO:
                        DateTime dtAlteracao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtAlteracao.Equals(dtAlteracao)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.STLOGINADQUIRENTE:
                        byte stLoginAdquirente = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.stLoginAdquirente.Equals(stLoginAdquirente)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.STLOGINADQUIRENTEEMPRESA:
                        byte stLoginAdquirenteEmpresa = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.stLoginAdquirenteEmpresa.Equals(stLoginAdquirenteEmpresa)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;

                    // PERSONALIZADO
                    case CAMPOS.DSADQUIRENTE:
                        string dsAdquirente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tbAdquirente.dsAdquirente.Equals(dsAdquirente)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                    case CAMPOS.DS_FANTASIA:
                        string ds_fantasia = Convert.ToString(item.Value);
                        if (ds_fantasia.Contains("%")) // usa LIKE
                        {
                            string busca = ds_fantasia.Replace("%", "").ToString();
                            entity = entity.Where(e => e.empresa.ds_fantasia.Contains(busca)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        }
                        else
                            entity = entity.Where(e => e.empresa.ds_fantasia.Equals(ds_fantasia)).AsQueryable<tbLoginAdquirenteEmpresa>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDLOGINADQUIRENTEEMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdLoginAdquirenteEmpresa).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdLoginAdquirenteEmpresa).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdGrupo).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdGrupo).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCnpj).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.nrCnpj).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.DSLOGIN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsLogin).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dsLogin).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.DSSENHA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsSenha).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dsSenha).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.CDESTABELECIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEstabelecimento).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdEstabelecimento).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.DTALTERACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtAlteracao).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dtAlteracao).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.STLOGINADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.stLoginAdquirente).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.stLoginAdquirente).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.STLOGINADQUIRENTEEMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.stLoginAdquirenteEmpresa).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.stLoginAdquirenteEmpresa).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;

                // PERSONALIZADO
                case CAMPOS.DSADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tbAdquirente.dsAdquirente).ThenBy(e => e.dsLogin).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.tbAdquirente.dsAdquirente).ThenByDescending(e => e.dsLogin).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
                case CAMPOS.DS_FANTASIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.empresa.ds_fantasia).ThenBy(e => e.tbAdquirente.dsAdquirente).ThenBy(e => e.dsLogin).AsQueryable<tbLoginAdquirenteEmpresa>();
                    else entity = entity.OrderByDescending(e => e.empresa.ds_fantasia).ThenByDescending(e => e.tbAdquirente.dsAdquirente).ThenByDescending(e => e.dsLogin).AsQueryable<tbLoginAdquirenteEmpresa>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbAdquirente/TbAdquirente
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbLoginAdquirenteEmpresas = new List<dynamic>();
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
                    CollectionTbLoginAdquirenteEmpresas = query.Select(e => new
                    {
                        cdLoginAdquirenteEmpresa = e.cdLoginAdquirenteEmpresa,
                        cdAdquirente = e.cdAdquirente,
                        cdGrupo = e.cdGrupo,
                        nrCnpj = e.nrCnpj,
                        dsLogin = e.dsLogin,
                        dsSenha = e.dsSenha,
                        cdEstabelecimento = e.cdEstabelecimento,
                        dtAlteracao = e.dtAlteracao,
                        stLoginAdquirente = e.stLoginAdquirente,
                        //stLoginAdquirenteEmpresa = e.stLoginAdquirenteEmpresa // controle de bruno
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbLoginAdquirenteEmpresas = query.Select(e => new
                    {
                        cdLoginAdquirenteEmpresa = e.cdLoginAdquirenteEmpresa,
                        cdAdquirente = e.cdAdquirente,
                        cdGrupo = e.cdGrupo,
                        nrCnpj = e.nrCnpj,
                        dsLogin = e.dsLogin,
                        dsSenha = e.dsSenha,
                        cdEstabelecimento = e.cdEstabelecimento,
                        dtAlteracao = e.dtAlteracao,
                        stLoginAdquirente = e.stLoginAdquirente,
                        //stLoginAdquirenteEmpresa = e.stLoginAdquirenteEmpresa  // controle de bruno
                    }).ToList<dynamic>();
                }
                else if (colecao == 2) // [WEB] 
                {
                    CollectionTbLoginAdquirenteEmpresas = query.Select(e => new
                    {
                        cdLoginAdquirenteEmpresa = e.cdLoginAdquirenteEmpresa,
                        adquirente = new
                        {
                            cdAdquirente = e.tbAdquirente.cdAdquirente,
                            nmAdquirente = e.tbAdquirente.nmAdquirente,
                            dsAdquirente = e.tbAdquirente.dsAdquirente,
                            stAdquirente = e.tbAdquirente.stAdquirente,
                        },
                        grupo_empresa = new
                        {
                            id_grupo = e.grupo_empresa.id_grupo,
                            ds_nome = e.grupo_empresa.ds_nome
                        },
                        empresa = new
                        {
                            nu_cnpj = e.empresa.nu_cnpj,
                            ds_fantasia = e.empresa.ds_fantasia
                        },
                        //dsLogin = e.dsLogin,
                        //dsSenha = e.dsSenha,
                        //cdEstabelecimento = e.cdEstabelecimento,
                        //dtAlteracao = e.dtAlteracao,
                        stLoginAdquirente = e.stLoginAdquirente,
                        //stLoginAdquirenteEmpresa = e.stLoginAdquirenteEmpresa  // controle de bruno
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbLoginAdquirenteEmpresas;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar login aquirente empresa" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbLoginAdquirenteEmpresa param)
        {
            try
            {
                _db.tbLoginAdquirenteEmpresas.Add(param);
                _db.SaveChanges();
                return param.cdLoginAdquirenteEmpresa;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao dalvar login aquirente empresa" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdLoginAdquirenteEmpresa)
        {
            try
            {
                tbLoginAdquirenteEmpresa value = _db.tbLoginAdquirenteEmpresas
                                                        .Where(e => e.cdLoginAdquirenteEmpresa == cdLoginAdquirenteEmpresa)
                                                        .FirstOrDefault();
                if (value == null) throw new Exception("Registro inexistente");

                _db.tbLoginAdquirenteEmpresas.Remove(value);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar login aquirente empresa" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Altera TbLoginAdquirenteEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbLoginAdquirenteEmpresa param)
        {
            try
            {
                tbLoginAdquirenteEmpresa value = _db.tbLoginAdquirenteEmpresas
                        .Where(e => e.cdLoginAdquirenteEmpresa == param.cdLoginAdquirenteEmpresa)
                        .FirstOrDefault();

                if (value == null) throw new Exception("Registro inexistente");

                // OBSERVAÇÂO: NÃO ALTERA GRUPO, CNPJ E ADQUIRENTE

                if (param.dsLogin != null && param.dsLogin != value.dsLogin)
                    value.dsLogin = param.dsLogin;
                if (param.dsSenha != null && param.dsSenha != value.dsSenha)
                    value.dsSenha = param.dsSenha;
                if (param.cdEstabelecimento != null && param.cdEstabelecimento != value.cdEstabelecimento)
                    value.cdEstabelecimento = param.cdEstabelecimento;
                if (param.stLoginAdquirente != value.stLoginAdquirente)
                    value.stLoginAdquirente = param.stLoginAdquirente;
                if (param.stLoginAdquirenteEmpresa != value.stLoginAdquirenteEmpresa)
                    value.stLoginAdquirenteEmpresa = param.stLoginAdquirenteEmpresa;
                value.dtAlteracao = DateTime.Now;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar login aquirente empresa" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
