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
    public class GatewayTbEmpresaFilial
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbEmpresaFilial()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            NRCNPJ = 100,
            NRCNPJBASE = 101,
            NRCNPJSEQUENCIA = 102,
            NRCNPJDIGITO = 103,
            NMFANTASIA = 104,
            NMRAZAOSOCIAL = 105,
            DSENDERECO = 106,
            DSCIDADE = 107,
            SGUF = 108,
            NRCEP = 109,
            NRTELEFONE = 110,
            DSBAIRRO = 111,
            DSEMAIL = 112,
            DTCADASTRO = 113,
            FLATIVO = 114,
            CDEMPRESAGRUPO = 115,
            NRFILIAL = 116,
            NRINSCESTADUAL = 117,
            TOKEN = 118,

        };

        /// <summary>
        /// Get TbEmpresaFilial/TbEmpresaFilial
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbEmpresaFilial> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbEmpresaFiliais.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable();
                        break;
                    case CAMPOS.NRCNPJBASE:
                        string nrCNPJBase = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJBase.Equals(nrCNPJBase)).AsQueryable();
                        break;
                    case CAMPOS.NRCNPJSEQUENCIA:
                        string nrCNPJSequencia = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJSequencia.Equals(nrCNPJSequencia)).AsQueryable();
                        break;
                    case CAMPOS.NRCNPJDIGITO:
                        string nrCNPJDigito = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJDigito.Equals(nrCNPJDigito)).AsQueryable();
                        break;
                    case CAMPOS.NMFANTASIA:
                        string nmFantasia = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmFantasia.Equals(nmFantasia)).AsQueryable();
                        break;
                    case CAMPOS.NMRAZAOSOCIAL:
                        string nmRazaoSocial = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmRazaoSocial.Equals(nmRazaoSocial)).AsQueryable();
                        break;
                    case CAMPOS.DSENDERECO:
                        string dsEndereco = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsEndereco.Equals(dsEndereco)).AsQueryable();
                        break;
                    case CAMPOS.DSCIDADE:
                        string dsCidade = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsCidade.Equals(dsCidade)).AsQueryable();
                        break;
                    case CAMPOS.SGUF:
                        string sgUF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.sgUF.Equals(sgUF)).AsQueryable();
                        break;
                    case CAMPOS.NRCEP:
                        string nrCEP = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCEP.Equals(nrCEP)).AsQueryable();
                        break;
                    case CAMPOS.NRTELEFONE:
                        string nrTelefone = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrTelefone.Equals(nrTelefone)).AsQueryable();
                        break;
                    case CAMPOS.DSBAIRRO:
                        string dsBairro = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsBairro.Equals(dsBairro)).AsQueryable();
                        break;
                    case CAMPOS.DSEMAIL:
                        string dsEmail = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsEmail.Equals(dsEmail)).AsQueryable();
                        break;
                    case CAMPOS.DTCADASTRO:
                        DateTime dtCadastro = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtCadastro.Equals(dtCadastro)).AsQueryable();
                        break;
                    case CAMPOS.FLATIVO:
                        Boolean flAtivo = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flAtivo.Equals(flAtivo)).AsQueryable();
                        break;
                    case CAMPOS.CDEMPRESAGRUPO:
                        Int32 cdEmpresaGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdEmpresaGrupo.Equals(cdEmpresaGrupo)).AsQueryable();
                        break;
                    case CAMPOS.NRFILIAL:
                        string nrFilial = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrFilial.Equals(nrFilial)).AsQueryable();
                        break;
                    case CAMPOS.NRINSCESTADUAL:
                        string nrInscEstadual = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrInscEstadual.Equals(nrInscEstadual)).AsQueryable();
                        break;
                    case CAMPOS.TOKEN:
                        string token = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.token.Equals(token)).AsQueryable();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable();
                    break;
                case CAMPOS.NRCNPJBASE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJBase).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrCNPJBase).AsQueryable();
                    break;
                case CAMPOS.NRCNPJSEQUENCIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJSequencia).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrCNPJSequencia).AsQueryable();
                    break;
                case CAMPOS.NRCNPJDIGITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJDigito).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrCNPJDigito).AsQueryable();
                    break;
                case CAMPOS.NMFANTASIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmFantasia).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nmFantasia).AsQueryable();
                    break;
                case CAMPOS.NMRAZAOSOCIAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmRazaoSocial).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nmRazaoSocial).AsQueryable();
                    break;
                case CAMPOS.DSENDERECO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsEndereco).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsEndereco).AsQueryable();
                    break;
                case CAMPOS.DSCIDADE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsCidade).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsCidade).AsQueryable();
                    break;
                case CAMPOS.SGUF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.sgUF).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.sgUF).AsQueryable();
                    break;
                case CAMPOS.NRCEP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCEP).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrCEP).AsQueryable();
                    break;
                case CAMPOS.NRTELEFONE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrTelefone).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrTelefone).AsQueryable();
                    break;
                case CAMPOS.DSBAIRRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsBairro).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsBairro).AsQueryable();
                    break;
                case CAMPOS.DSEMAIL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsEmail).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsEmail).AsQueryable();
                    break;
                case CAMPOS.DTCADASTRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtCadastro).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtCadastro).AsQueryable();
                    break;
                case CAMPOS.FLATIVO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flAtivo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.flAtivo).AsQueryable();
                    break;
                case CAMPOS.CDEMPRESAGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEmpresaGrupo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdEmpresaGrupo).AsQueryable();
                    break;
                case CAMPOS.NRFILIAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrFilial).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrFilial).AsQueryable();
                    break;
                case CAMPOS.NRINSCESTADUAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrInscEstadual).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrInscEstadual).AsQueryable();
                    break;
                case CAMPOS.TOKEN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.token).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.token).AsQueryable();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbEmpresaFilial/TbEmpresaFilial
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbEmpresaFilial = new List<dynamic>();
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
                    CollectionTbEmpresaFilial = query.Select(e => new
                    {

                        nrCNPJ = e.nrCNPJ,
                        nrCNPJBase = e.nrCNPJBase,
                        nrCNPJSequencia = e.nrCNPJSequencia,
                        nrCNPJDigito = e.nrCNPJDigito,
                        nmFantasia = e.nmFantasia,
                        nmRazaoSocial = e.nmRazaoSocial,
                        dsEndereco = e.dsEndereco,
                        dsCidade = e.dsCidade,
                        sgUF = e.sgUF,
                        nrCEP = e.nrCEP,
                        nrTelefone = e.nrTelefone,
                        dsBairro = e.dsBairro,
                        dsEmail = e.dsEmail,
                        dtCadastro = e.dtCadastro,
                        flAtivo = e.flAtivo,
                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                        nrFilial = e.nrFilial,
                        nrInscEstadual = e.nrInscEstadual,
                        token = e.token,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbEmpresaFilial = query.Select(e => new
                    {

                        nrCNPJ = e.nrCNPJ,
                        nrCNPJBase = e.nrCNPJBase,
                        nrCNPJSequencia = e.nrCNPJSequencia,
                        nrCNPJDigito = e.nrCNPJDigito,
                        nmFantasia = e.nmFantasia,
                        nmRazaoSocial = e.nmRazaoSocial,
                        dsEndereco = e.dsEndereco,
                        dsCidade = e.dsCidade,
                        sgUF = e.sgUF,
                        nrCEP = e.nrCEP,
                        nrTelefone = e.nrTelefone,
                        dsBairro = e.dsBairro,
                        dsEmail = e.dsEmail,
                        dtCadastro = e.dtCadastro,
                        flAtivo = e.flAtivo,
                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                        nrFilial = e.nrFilial,
                        nrInscEstadual = e.nrInscEstadual,
                        token = e.token,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbEmpresaFilial;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbEmpresaFilial" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbEmpresaFilial
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Add(string token, tbEmpresaFilial param)
        {
            try
            {
                _db.tbEmpresaFiliais.Add(param);
                _db.SaveChanges();
                return param.nrCNPJ;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbEmpresaFilial" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbEmpresaFilial
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, string nrCNPJ)
        {
            try
            {
                _db.tbEmpresaFiliais.Remove(_db.tbEmpresaFiliais.Where(e => e.nrCNPJ.Equals(nrCNPJ)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbEmpresaFilial" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }



        /// <summary>
        /// Altera tbEmpresaFilial
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbEmpresaFilial param)
        {
            try
            {
                tbEmpresaFilial value = _db.tbEmpresaFiliais
                        .Where(e => e.nrCNPJ.Equals(param.nrCNPJ))
                        .First<tbEmpresaFilial>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                    value.nrCNPJ = param.nrCNPJ;
                if (param.nrCNPJBase != null && param.nrCNPJBase != value.nrCNPJBase)
                    value.nrCNPJBase = param.nrCNPJBase;
                if (param.nrCNPJSequencia != null && param.nrCNPJSequencia != value.nrCNPJSequencia)
                    value.nrCNPJSequencia = param.nrCNPJSequencia;
                if (param.nrCNPJDigito != null && param.nrCNPJDigito != value.nrCNPJDigito)
                    value.nrCNPJDigito = param.nrCNPJDigito;
                if (param.nmFantasia != null && param.nmFantasia != value.nmFantasia)
                    value.nmFantasia = param.nmFantasia;
                if (param.nmRazaoSocial != null && param.nmRazaoSocial != value.nmRazaoSocial)
                    value.nmRazaoSocial = param.nmRazaoSocial;
                if (param.dsEndereco != null && param.dsEndereco != value.dsEndereco)
                    value.dsEndereco = param.dsEndereco;
                if (param.dsCidade != null && param.dsCidade != value.dsCidade)
                    value.dsCidade = param.dsCidade;
                if (param.sgUF != null && param.sgUF != value.sgUF)
                    value.sgUF = param.sgUF;
                if (param.nrCEP != null && param.nrCEP != value.nrCEP)
                    value.nrCEP = param.nrCEP;
                if (param.nrTelefone != null && param.nrTelefone != value.nrTelefone)
                    value.nrTelefone = param.nrTelefone;
                if (param.dsBairro != null && param.dsBairro != value.dsBairro)
                    value.dsBairro = param.dsBairro;
                if (param.dsEmail != null && param.dsEmail != value.dsEmail)
                    value.dsEmail = param.dsEmail;
                if (param.dtCadastro != null && param.dtCadastro != value.dtCadastro)
                    value.dtCadastro = param.dtCadastro;
                if (param.flAtivo != null && param.flAtivo != value.flAtivo)
                    value.flAtivo = param.flAtivo;
                if (param.cdEmpresaGrupo != null && param.cdEmpresaGrupo != value.cdEmpresaGrupo)
                    value.cdEmpresaGrupo = param.cdEmpresaGrupo;
                if (param.nrFilial != null && param.nrFilial != value.nrFilial)
                    value.nrFilial = param.nrFilial;
                if (param.nrInscEstadual != null && param.nrInscEstadual != value.nrInscEstadual)
                    value.nrInscEstadual = param.nrInscEstadual;
                if (param.token != null && param.token != value.token)
                    value.token = param.token;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbEmpresaFilial" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
