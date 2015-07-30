using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Cliente
{
    public class GatewayEmpresa
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayEmpresa()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            NU_CNPJ = 100,
            NU_BASECNPJ = 101,
            NU_SEQUENCIACNPJ = 102,
            NU_DIGITOCNPJ = 103,
            DS_FANTASIA = 104,
            DS_RAZAOSOCIAL = 105,
            DS_ENDERECO = 106,
            DS_CIDADE = 107,
            SG_UF = 108,
            NU_CEP = 109,
            NU_TELEFONE = 110,
            DS_BAIRRO = 111,
            DS_EMAIL = 112,
            DT_CADASTRO = 113,
            FL_ATIVO = 114,
            TOKEN = 115,
            ID_GRUPO = 116,
            FILIAL = 117,
            NU_INSCESTADUAL = 118,

        };

        /// <summary>
        /// Get Empresa/Empresa
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<empresa> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.empresas.AsQueryable<empresa>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_cnpj.Equals(nu_cnpj)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.NU_BASECNPJ:
                        string nu_BaseCnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_BaseCnpj.Equals(nu_BaseCnpj)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.NU_SEQUENCIACNPJ:
                        string nu_SequenciaCnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_SequenciaCnpj.Equals(nu_SequenciaCnpj)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.NU_DIGITOCNPJ:
                        string nu_DigitoCnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_DigitoCnpj.Equals(nu_DigitoCnpj)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.DS_FANTASIA:
                        string ds_fantasia = Convert.ToString(item.Value);
                        if (ds_fantasia.Contains("%")) // usa LIKE
                        {
                            string busca = ds_fantasia.Replace("%", "").ToString();
                            entity = entity.Where(e => e.ds_fantasia.Contains(busca)).AsQueryable<empresa>();
                        }
                        else
                            entity = entity.Where(e => e.ds_fantasia.Equals(ds_fantasia)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.DS_RAZAOSOCIAL:
                        string ds_razaoSocial = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ds_razaoSocial.Equals(ds_razaoSocial)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.DS_ENDERECO:
                        string ds_endereco = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ds_endereco.Equals(ds_endereco)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.DS_CIDADE:
                        string ds_cidade = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ds_cidade.Equals(ds_cidade)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.SG_UF:
                        string sg_uf = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.sg_uf.Equals(sg_uf)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.NU_CEP:
                        string nu_cep = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_cep.Equals(nu_cep)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.NU_TELEFONE:
                        string nu_telefone = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_telefone.Equals(nu_telefone)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.DS_BAIRRO:
                        string ds_bairro = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ds_bairro.Equals(ds_bairro)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.DS_EMAIL:
                        string ds_email = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ds_email.Equals(ds_email)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.DT_CADASTRO:
                        DateTime dt_cadastro = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dt_cadastro.Equals(dt_cadastro)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.FL_ATIVO:
                        Int32 fl_ativo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.fl_ativo.Equals(fl_ativo)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.TOKEN:
                        string token = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.token.Equals(token)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_grupo.Equals(id_grupo)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.FILIAL:
                        string filial = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.filial.Equals(filial)).AsQueryable<empresa>();
                        break;
                    case CAMPOS.NU_INSCESTADUAL:
                        Int32 nu_inscEstadual = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.nu_inscEstadual.Equals(nu_inscEstadual)).AsQueryable<empresa>();
                        break;

                }
            }
            #endregion

            #region ORDER BY -ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.NU_CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_cnpj).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.nu_cnpj).AsQueryable<empresa>();
                    break;
                case CAMPOS.NU_BASECNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_BaseCnpj).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.nu_BaseCnpj).AsQueryable<empresa>();
                    break;
                case CAMPOS.NU_SEQUENCIACNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_SequenciaCnpj).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.nu_SequenciaCnpj).AsQueryable<empresa>();
                    break;
                case CAMPOS.NU_DIGITOCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_DigitoCnpj).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.nu_DigitoCnpj).AsQueryable<empresa>();
                    break;
                case CAMPOS.DS_FANTASIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_fantasia).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.ds_fantasia).AsQueryable<empresa>();
                    break;
                case CAMPOS.DS_RAZAOSOCIAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_razaoSocial).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.ds_razaoSocial).AsQueryable<empresa>();
                    break;
                case CAMPOS.DS_ENDERECO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_endereco).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.ds_endereco).AsQueryable<empresa>();
                    break;
                case CAMPOS.DS_CIDADE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_cidade).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.ds_cidade).AsQueryable<empresa>();
                    break;
                case CAMPOS.SG_UF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.sg_uf).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.sg_uf).AsQueryable<empresa>();
                    break;
                case CAMPOS.NU_CEP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_cep).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.nu_cep).AsQueryable<empresa>();
                    break;
                case CAMPOS.NU_TELEFONE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_telefone).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.nu_telefone).AsQueryable<empresa>();
                    break;
                case CAMPOS.DS_BAIRRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_bairro).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.ds_bairro).AsQueryable<empresa>();
                    break;
                case CAMPOS.DS_EMAIL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_email).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.ds_email).AsQueryable<empresa>();
                    break;
                case CAMPOS.DT_CADASTRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dt_cadastro).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.dt_cadastro).AsQueryable<empresa>();
                    break;
                case CAMPOS.FL_ATIVO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.fl_ativo).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.fl_ativo).AsQueryable<empresa>();
                    break;
                case CAMPOS.TOKEN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.token).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.token).AsQueryable<empresa>();
                    break;
                case CAMPOS.ID_GRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_grupo).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.id_grupo).AsQueryable<empresa>();
                    break;
                case CAMPOS.FILIAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.filial).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.filial).AsQueryable<empresa>();
                    break;
                case CAMPOS.NU_INSCESTADUAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_inscEstadual).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.nu_inscEstadual).AsQueryable<empresa>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Empresa/Empresa
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            // Implementar o filtro por Grupo apartir do TOKEN do Usuário
            string outValue = null;
            Int32 IdGrupo = Permissoes.GetIdGrupo(token);
            if (IdGrupo != 0)
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                else
                    queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
            }

            //DECLARAÇÕES
            List<dynamic> CollectionEmpresa = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

            // Restringe consulta pelo perfil do usuário logado
            Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
            String RoleName = Permissoes.GetRoleName(token).ToUpper();
            if (IdGrupo == 0 && RoleName.Equals("COMERCIAL"))
            {
                // Perfil Comercial tem uma carteira de clientes específica
                List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(token);
                query = query.Where(e => listaIdsGruposEmpresas.Contains(e.id_grupo)).AsQueryable<empresa>();
            }


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
                CollectionEmpresa = query.Select(e => new
                {

                    nu_cnpj = e.nu_cnpj,
                    nu_BaseCnpj = e.nu_BaseCnpj,
                    nu_SequenciaCnpj = e.nu_SequenciaCnpj,
                    nu_DigitoCnpj = e.nu_DigitoCnpj,
                    ds_fantasia = e.ds_fantasia,
                    ds_razaoSocial = e.ds_razaoSocial,
                    ds_endereco = e.ds_endereco,
                    ds_cidade = e.ds_cidade,
                    sg_uf = e.sg_uf,
                    nu_cep = e.nu_cep,
                    nu_telefone = e.nu_telefone,
                    ds_bairro = e.ds_bairro,
                    ds_email = e.ds_email,
                    dt_cadastro = e.dt_cadastro,
                    fl_ativo = e.fl_ativo,
                    token = e.token,
                    id_grupo = e.id_grupo,
                    filial = e.filial,
                    nu_inscEstadual = e.nu_inscEstadual,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionEmpresa = query.Select(e => new
                {
                    nu_cnpj = e.nu_cnpj,
                    ds_fantasia = e.ds_fantasia,
                    id_grupo = e.id_grupo,
                }).ToList<dynamic>();
            }
            else if (colecao == 2)
            {
                CollectionEmpresa = query.Select(e => new
                {

                    nu_cnpj = e.nu_cnpj,
                    ds_fantasia = e.ds_fantasia,
                    ds_razaoSocial = e.ds_razaoSocial,
                    ds_endereco = e.ds_endereco,
                    ds_cidade = e.ds_cidade,
                    sg_uf = e.sg_uf,
                    nu_cep = e.nu_cep,
                    nu_telefone = e.nu_telefone,
                    ds_bairro = e.ds_bairro,
                    ds_email = e.ds_email,
                    dt_cadastro = e.dt_cadastro,
                    fl_ativo = e.fl_ativo,
                    id_grupo = e.id_grupo,
                    filial = e.filial,
                    nu_inscEstadual = e.nu_inscEstadual,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionEmpresa;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Add(string token, empresa param)
        {
            param.dt_cadastro = DateTime.Now;
            param.fl_ativo = 1;
            param.token = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
            param.nu_BaseCnpj = param.nu_cnpj.Substring(0,8);
            param.nu_SequenciaCnpj = param.nu_cnpj.Substring(8,4);
            param.nu_DigitoCnpj = param.nu_cnpj.Substring(12,2);
            if (param.filial == null)
                param.filial = " ";            


            _db.empresas.Add(param);
            _db.SaveChanges();
            return param.nu_cnpj;
        }


        /// <summary>
        /// Apaga uma Empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, string nu_cnpj)
        {
            if (_db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa == nu_cnpj).ToList().Count == 0)
            {
                _db.empresas.Remove(_db.empresas.Where(e => e.nu_cnpj.Equals(nu_cnpj)).First());
                _db.SaveChanges();
            }
            else
                throw new Exception("Empresa não pode ser deletada!");
        }



        /// <summary>
        /// Altera empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, empresa param)
        {
            empresa filial = _db.empresas
                    .Where(e => e.nu_cnpj.Equals(param.nu_cnpj))
                    .First<empresa>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS



            if (param.ds_fantasia != null && param.ds_fantasia != filial.ds_fantasia)
                filial.ds_fantasia = param.ds_fantasia;
            if (param.ds_razaoSocial != null && param.ds_razaoSocial != filial.ds_razaoSocial)
                filial.ds_razaoSocial = param.ds_razaoSocial;
            if (param.ds_endereco != null && param.ds_endereco != filial.ds_endereco)
                filial.ds_endereco = param.ds_endereco;
            if (param.ds_cidade != null && param.ds_cidade != filial.ds_cidade)
                filial.ds_cidade = param.ds_cidade;
            if (param.sg_uf != null && param.sg_uf != filial.sg_uf)
                filial.sg_uf = param.sg_uf;
            if (param.nu_cep != null && param.nu_cep != filial.nu_cep)
                filial.nu_cep = param.nu_cep;
            if (param.nu_telefone != null && param.nu_telefone != filial.nu_telefone)
                filial.nu_telefone = param.nu_telefone;
            if (param.ds_bairro != null && param.ds_bairro != filial.ds_bairro)
                filial.ds_bairro = param.ds_bairro;
            if (param.ds_email != null && param.ds_email != filial.ds_email)
                filial.ds_email = param.ds_email;
            //if (param.dt_cadastro != null && param.dt_cadastro != filial.dt_cadastro)
            //    filial.dt_cadastro = param.dt_cadastro;
            if (param.fl_ativo != null && param.fl_ativo != filial.fl_ativo)
                filial.fl_ativo = param.fl_ativo;
            //if (param.token != null && param.token != filial.token)
            //    filial.token = param.token;
            //if (param.id_grupo != null && param.id_grupo != filial.id_grupo)
            //    filial.id_grupo = param.id_grupo;
            if (param.filial != null && param.filial != filial.filial)
                filial.filial = param.filial;
            if (param.nu_inscEstadual != null && param.nu_inscEstadual != filial.nu_inscEstadual)
                filial.nu_inscEstadual = param.nu_inscEstadual;
            _db.SaveChanges();

        }

    }
}
