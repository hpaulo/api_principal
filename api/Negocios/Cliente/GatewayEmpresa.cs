using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Data.Entity;
using api.Negocios.Util;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace api.Negocios.Cliente
{
    public class GatewayEmpresa
    {
        //static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayEmpresa()
        {
           //_db.Configuration.ProxyCreationEnabled = false;
        }

        public static string SIGLA_QUERY = "EM";

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
        private static IQueryable<empresa> getQuery(painel_taxservices_dbContext _db, int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_fantasia).ThenBy(e => e.filial).AsQueryable<empresa>();
                    else entity = entity.OrderByDescending(e => e.ds_fantasia).ThenByDescending(e => e.filial).AsQueryable<empresa>();
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
        /// Get Empresa/Empresa
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static SimpleDataBaseQuery getQuery(int campo, int orderby, Dictionary<string, string> queryString)
        {
            Dictionary<string, string> join = new Dictionary<string, string>();
            List<string> where = new List<string>();
            List<string> order = new List<string>();

            #region WHERE - ADICIONA OS FILTROS A QUERY
            // ADICIONA OS FILTROS A QUERY
            foreach (KeyValuePair<string, string> item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nu_cnpj = '" + nu_cnpj + "'");
                        break;
                    case CAMPOS.NU_BASECNPJ:
                        string nu_BaseCnpj = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nu_BaseCnpj = '" + nu_BaseCnpj + "'");
                        break;
                    case CAMPOS.NU_SEQUENCIACNPJ:
                        string nu_SequenciaCnpj = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nu_SequenciaCnpj = '" + nu_SequenciaCnpj + "'");
                        break;
                    case CAMPOS.NU_DIGITOCNPJ:
                        string nu_DigitoCnpj = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nu_DigitoCnpj = '" + nu_DigitoCnpj + "'");
                        break;
                    case CAMPOS.DS_FANTASIA:
                        string ds_fantasia = Convert.ToString(item.Value);
                        if (ds_fantasia.Contains("%")) // usa LIKE
                        {
                            string busca = ds_fantasia.Replace("%", "").ToString();
                            where.Add(SIGLA_QUERY + ".ds_fantasia like '%" + ds_fantasia + "%'");
                        }
                        else
                            where.Add(SIGLA_QUERY + ".ds_fantasia = '" + ds_fantasia + "'");                        
                        break;
                    case CAMPOS.DS_RAZAOSOCIAL:
                        string ds_razaoSocial = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".ds_razaoSocial = '" + ds_razaoSocial + "'");
                        break;
                    case CAMPOS.DS_ENDERECO:
                        string ds_endereco = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".ds_endereco = '" + ds_endereco + "'");
                        break;
                    case CAMPOS.DS_CIDADE:
                        string ds_cidade = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".ds_cidade = '" + ds_cidade + "'");
                        break;
                    case CAMPOS.SG_UF:
                        string sg_uf = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".sg_uf = '" + sg_uf + "'");
                        break;
                    case CAMPOS.NU_CEP:
                        string nu_cep = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nu_cep = '" + nu_cep + "'");
                        break;
                    case CAMPOS.NU_TELEFONE:
                        string nu_telefone = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".nu_telefone = '" + nu_telefone + "'");
                        break;
                    case CAMPOS.DS_BAIRRO:
                        string ds_bairro = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".ds_bairro = '" + ds_bairro + "'");
                        break;
                    case CAMPOS.DS_EMAIL:
                        string ds_email = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".ds_email = '" + ds_email + "'");
                        break;
                    case CAMPOS.DT_CADASTRO:
                        DateTime dt_cadastro = Convert.ToDateTime(item.Value);
                        where.Add(SIGLA_QUERY + ".dt_cadastro = '" + DataBaseQueries.GetDate(dt_cadastro) + "'");
                        break;
                    case CAMPOS.FL_ATIVO:
                        Int32 fl_ativo = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".fl_ativo = " + fl_ativo);
                        break;
                    case CAMPOS.TOKEN:
                        string token = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".token = '" + token + "'");
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".id_grupo = " + id_grupo);
                        break;
                    case CAMPOS.FILIAL:
                        string filial = Convert.ToString(item.Value);
                        where.Add(SIGLA_QUERY + ".filial = '" + filial + "'");
                        break;
                    case CAMPOS.NU_INSCESTADUAL:
                        Int32 nu_inscEstadual = Convert.ToInt32(item.Value);
                        where.Add(SIGLA_QUERY + ".nu_inscEstadual = " + nu_inscEstadual);
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.NU_CNPJ:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nu_cnpj ASC");
                    else order.Add(SIGLA_QUERY + ".nu_cnpj DESC");
                    break;
                case CAMPOS.NU_BASECNPJ:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nu_BaseCnpj ASC");
                    else order.Add(SIGLA_QUERY + ".nu_BaseCnpj DESC");
                    break;
                case CAMPOS.NU_SEQUENCIACNPJ:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nu_SequenciaCnpj ASC");
                    else order.Add(SIGLA_QUERY + ".nu_SequenciaCnpj DESC");
                    break;
                case CAMPOS.NU_DIGITOCNPJ:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nu_DigitoCnpj ASC");
                    else order.Add(SIGLA_QUERY + ".nu_DigitoCnpj DESC");
                    break;
                case CAMPOS.DS_FANTASIA:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".ds_fantasia ASC");
                    else order.Add(SIGLA_QUERY + ".ds_fantasia DESC");
                    break;
                case CAMPOS.DS_RAZAOSOCIAL:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".ds_razaoSocial ASC");
                    else order.Add(SIGLA_QUERY + ".ds_razaoSocial DESC");
                    break;
                case CAMPOS.DS_ENDERECO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".ds_endereco ASC");
                    else order.Add(SIGLA_QUERY + ".ds_endereco DESC");
                    break;
                case CAMPOS.DS_CIDADE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".ds_cidade ASC");
                    else order.Add(SIGLA_QUERY + ".ds_cidade DESC");
                    break;
                case CAMPOS.SG_UF:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".sg_uf ASC");
                    else order.Add(SIGLA_QUERY + ".sg_uf DESC");
                    break;
                case CAMPOS.NU_CEP:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nu_cep ASC");
                    else order.Add(SIGLA_QUERY + ".nu_cep DESC");
                    break;
                case CAMPOS.NU_TELEFONE:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nu_telefone ASC");
                    else order.Add(SIGLA_QUERY + ".nu_telefone DESC");
                    break;
                case CAMPOS.DS_BAIRRO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".ds_bairro ASC");
                    else order.Add(SIGLA_QUERY + ".ds_bairro DESC");
                    break;
                case CAMPOS.DS_EMAIL:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".ds_email ASC");
                    else order.Add(SIGLA_QUERY + ".ds_email DESC");
                    break;
                case CAMPOS.DT_CADASTRO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".dt_cadastro ASC");
                    else order.Add(SIGLA_QUERY + ".dt_cadastro DESC");
                    break;
                case CAMPOS.FL_ATIVO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".fl_ativo ASC");
                    else order.Add(SIGLA_QUERY + ".fl_ativo DESC");
                    break;
                case CAMPOS.TOKEN:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".token ASC");
                    else order.Add(SIGLA_QUERY + ".token DESC");
                    break;
                case CAMPOS.ID_GRUPO:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".id_grupo ASC");
                    else order.Add(SIGLA_QUERY + ".id_grupo DESC");
                    break;
                case CAMPOS.FILIAL:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".filial ASC");
                    else order.Add(SIGLA_QUERY + ".filial DESC");
                    break;
                case CAMPOS.NU_INSCESTADUAL:
                    if (orderby == 0) order.Add(SIGLA_QUERY + ".nu_inscEstadual ASC");
                    else order.Add(SIGLA_QUERY + ".nu_inscEstadual DESC");
                    break;
            }
            #endregion

            return new SimpleDataBaseQuery(null, "cliente.empresa " + SIGLA_QUERY,
                                           join, where.ToArray(), null, order.ToArray());


        }


        /// <summary>
        /// Retorna Empresa/Empresa
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            DbContextTransaction transaction = _db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
            try
            {
                // Se for uma consulta por um cnpj específico na coleção 0, não força filtro por empresa, filial e rolelevel
                string outValue = null;
                Boolean FiltroCNPJ = false;

                if (colecao == 0 && queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    FiltroCNPJ = !queryString["" + (int)CAMPOS.NU_CNPJ].Contains("%");


                //DECLARAÇÕES
                List<dynamic> CollectionEmpresa = new List<dynamic>();
                Retorno retorno = new Retorno();

                // Implementar o filtro por Grupo apartir do TOKEN do Usuário
                Int32 IdGrupo = 0;
                if (!FiltroCNPJ)
                {
                    IdGrupo = Permissoes.GetIdGrupo(token, _db);
                    if (IdGrupo != 0)
                    {
                        if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                            queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                        else
                            queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
                    }
                    string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token, _db);
                    if (!CnpjEmpresa.Equals(""))
                    {
                        if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                            queryString["" + (int)CAMPOS.NU_CNPJ] = CnpjEmpresa;
                        else
                            queryString.Add("" + (int)CAMPOS.NU_CNPJ, CnpjEmpresa);
                    }
                }


                // GET QUERY
                var query = getQuery(_db, colecao, campo, orderBy, pageSize, pageNumber, queryString);

                string whereVendedor = String.Empty;

                // Se não for uma consulta de CNPJ na coleção 0, restringe consulta pelo perfil Comercial que não estiver "amarrado" a um grupo
                if (!FiltroCNPJ)
                {
                    // Restringe consulta pelo perfil do usuário logado
                    //String RoleName = Permissoes.GetRoleName(token).ToUpper();
                    if (IdGrupo == 0 && Permissoes.isAtosRoleVendedor(token, _db))//RoleName.Equals("COMERCIAL"))
                    {
                        // Perfil Comercial tem uma carteira de clientes específica
                        List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(token, _db);
                        query = query.Where(e => listaIdsGruposEmpresas.Contains(e.id_grupo)).AsQueryable<empresa>();
                        whereVendedor = SIGLA_QUERY + ".id_grupo IN (" + string.Join(", ", listaIdsGruposEmpresas) + ")";
                    }
                }

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = query.Count();


                if (colecao != 4)
                {
                    // PAGINAÇÃO
                    int skipRows = (pageNumber - 1) * pageSize;
                    if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                        query = query.Skip(skipRows).Take(pageSize);
                    else
                        pageNumber = 1;
                }

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
                        filial = e.filial,
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
                        login_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa.Equals(e.nu_cnpj)).OrderByDescending(l => l.dtAcesso).Select(l => l.webpages_Users.ds_login).Take(1).FirstOrDefault(),
                        dt_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa.Equals(e.nu_cnpj)).OrderByDescending(l => l.dtAcesso).Select(l => l.dtAcesso).Take(1).FirstOrDefault(),
                        podeExcluir = _db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa.Equals(e.nu_cnpj)).Count() == 0
                    }).ToList<dynamic>();
                }
                else if (colecao == 3)
                {
                    CollectionEmpresa = query.Select(e => new
                    {
                        nu_cnpj = e.nu_cnpj,
                        ds_fantasia = e.ds_fantasia,
                        filial = e.filial,
                        estabelecimentos = e.LoginOperadoras
                                                .Select(l => new
                                                {
                                                    estabelecimento = l.estabelecimento,
                                                    operadora = new
                                                    {
                                                        id = l.idOperadora,
                                                        nmOperadora = l.Operadora.nmOperadora
                                                    }
                                                }).ToList<dynamic>()
                    }).ToList<dynamic>();
                }
                else if (colecao == 4)
                {
                    //CollectionEmpresa = query.Select(e => new
                    //{

                    //    nu_cnpj = e.nu_cnpj,
                    //    ds_fantasia = e.ds_fantasia,
                    //    ds_razaoSocial = e.ds_razaoSocial,
                    //    ds_endereco = e.ds_endereco,
                    //    ds_cidade = e.ds_cidade,
                    //    sg_uf = e.sg_uf,
                    //    nu_cep = e.nu_cep,
                    //    nu_telefone = e.nu_telefone,
                    //    ds_bairro = e.ds_bairro,
                    //    ds_email = e.ds_email,
                    //    dt_cadastro = e.dt_cadastro,
                    //    fl_ativo = e.fl_ativo,
                    //    id_grupo = e.id_grupo,
                    //    filial = e.filial,
                    //    nu_inscEstadual = e.nu_inscEstadual,
                    //    //ultimoAcesso = (string) null,
                    //    ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa.Equals(e.nu_cnpj)).OrderByDescending(l => l.dtAcesso).Take(1)
                    //                                 .Select(l => new
                    //                                 {
                    //                                     login_ultimoAcesso = l.webpages_Users.ds_login,
                    //                                     dt_ultimoAcesso = l.dtAcesso,
                    //                                 }).FirstOrDefault(),
                    //    //login_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa.Equals(e.nu_cnpj)).OrderByDescending(l => l.dtAcesso).Select(l => l.webpages_Users.ds_login).Take(1).FirstOrDefault(),
                    //    //dt_ultimoAcesso = _db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa.Equals(e.nu_cnpj)).OrderByDescending(l => l.dtAcesso).Select(l => l.dtAcesso).Take(1).FirstOrDefault(),
                    //    //podeExcluir = _db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa.Equals(e.nu_cnpj)).Count() == 0
                    //}).ToList<dynamic>();

                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

                    try
                    {
                        connection.Open();
                    }
                    catch
                    {
                        throw new Exception("Não foi possível estabelecer conexão com a base de dados");
                    }

                    try
                    {

                        SimpleDataBaseQuery databaseQuery = getQuery(campo, orderBy, queryString);

                        string scriptWhere = databaseQuery.ScriptForWhereClause();
                        string scriptOrderBy = databaseQuery.ScriptForOrderBy();
                        string script = "SELECT DISTINCT T.nu_cnpj" +
                                        ", T.ds_fantasia" +
                                        ", T.ds_razaoSocial" +
                                        ", T.ds_endereco" +
                                        ", T.ds_cidade" +
                                        ", T.sg_uf" +
                                        ", T.nu_cep" +
                                        ", T.nu_telefone" +
                                        ", T.ds_bairro" +
                                        ", T.ds_email" +
                                        ", T.dt_cadastro" +
                                        ", T.fl_ativo" +
                                        ", T.id_grupo" +
                                        ", T.filial" +
                                        ", T.nu_inscEstadual" +
                                        ", T.login_ultimoAcesso" +
                                        ", T.dt_ultimoAcesso" +
                                        ", podeExcluir = CASE WHEN T.dt_ultimoAcesso IS NULL AND L.nrCNPJ IS NULL THEN 1 ELSE 0 END" +
                                        " FROM (" +
                                        " SELECT DISTINCT " + SIGLA_QUERY + ".nu_cnpj" +
                                        ", " + SIGLA_QUERY + ".ds_fantasia" +
                                        ", " + SIGLA_QUERY + ".ds_razaoSocial" +
                                        ", " + SIGLA_QUERY + ".ds_endereco" +
                                        ", " + SIGLA_QUERY + ".ds_cidade" +
                                        ", " + SIGLA_QUERY + ".sg_uf" +
                                        ", " + SIGLA_QUERY + ".nu_cep" +
                                        ", " + SIGLA_QUERY + ".nu_telefone" +
                                        ", " + SIGLA_QUERY + ".ds_bairro" +
                                        ", " + SIGLA_QUERY + ".ds_email" +
                                        ", " + SIGLA_QUERY + ".dt_cadastro" +
                                        ", " + SIGLA_QUERY + ".fl_ativo" +
                                        ", " + SIGLA_QUERY + ".id_grupo" +
                                        ", " + SIGLA_QUERY + ".filial" +
                                        ", " + SIGLA_QUERY + ".nu_inscEstadual" +
                                        ", login_ultimoAcesso = U.ds_login" +
                                        ", dt_ultimoAcesso = L.dtAcesso" +
                                        " FROM log.logAcesso L (NOLOCK)" +
                                        " JOIN dbo.webpages_Users U (NOLOCK) ON U.id_users = L.idUsers" +
                                        " RIGHT JOIN cliente.empresa " + SIGLA_QUERY + " (NOLOCK) ON " + SIGLA_QUERY + ".nu_cnpj = U.nu_cnpjEmpresa" +
                                        " WHERE	L.dtAcesso IS NULL"
                                        + (scriptWhere.Trim().Equals("") ? "" : " AND " + scriptWhere)
                                        + (whereVendedor.Trim().Equals("") ? "" : " AND " + whereVendedor) +
                                        " UNION ALL " +
                                        " SELECT DISTINCT " + SIGLA_QUERY + ".nu_cnpj" +
                                        ", " + SIGLA_QUERY + ".ds_fantasia" +
                                        ", " + SIGLA_QUERY + ".ds_razaoSocial" +
                                        ", " + SIGLA_QUERY + ".ds_endereco" +
                                        ", " + SIGLA_QUERY + ".ds_cidade" +
                                        ", " + SIGLA_QUERY + ".sg_uf" +
                                        ", " + SIGLA_QUERY + ".nu_cep" +
                                        ", " + SIGLA_QUERY + ".nu_telefone" +
                                        ", " + SIGLA_QUERY + ".ds_bairro" +
                                        ", " + SIGLA_QUERY + ".ds_email" +
                                        ", " + SIGLA_QUERY + ".dt_cadastro" +
                                        ", " + SIGLA_QUERY + ".fl_ativo" +
                                        ", " + SIGLA_QUERY + ".id_grupo" +
                                        ", " + SIGLA_QUERY + ".filial" +
                                        ", " + SIGLA_QUERY + ".nu_inscEstadual" +
                                        ", login_ultimoAcesso = U.ds_login" +
                                        ", dt_ultimoAcesso = L.dtAcesso" +
                                        " FROM log.logAcesso L (NOLOCK)" +
                                        " JOIN dbo.webpages_Users U (NOLOCK) ON U.id_users = L.idUsers" +
                                        " RIGHT JOIN cliente.empresa " + SIGLA_QUERY + " (NOLOCK) ON " + SIGLA_QUERY + ".nu_cnpj = U.nu_cnpjEmpresa" +
                                        " WHERE	U.nu_cnpjEmpresa = " + SIGLA_QUERY + ".nu_cnpj AND L.dtAcesso in (SELECT MAX(L.dtAcesso) FROM log.logAcesso L (NOLOCK) JOIN dbo.webpages_Users U (NOLOCK) ON U.id_users = L.idUsers WHERE U.nu_cnpjEmpresa = " + SIGLA_QUERY + ".nu_cnpj)"
                                        + (scriptWhere.Trim().Equals("") ? "" : " AND " + scriptWhere)
                                        + (whereVendedor.Trim().Equals("") ? "" : " AND " + whereVendedor) +
                                        ") T" +
                                        " LEFT JOIN card.tbLoginAdquirenteEmpresa L (NOLOCK) ON L.nrCNPJ = T.nu_cnpj" +
                                        " ORDER BY T.ds_fantasia";

                        List<IDataRecord> resultado = DataBaseQueries.SqlQuery(script, connection);

                        if (resultado != null && resultado.Count > 0)
                        {
                            CollectionEmpresa = resultado.Select(t => new
                            {
                                nu_cnpj = Convert.ToString(t["nu_cnpj"]),
                                ds_fantasia = Convert.ToString(t["ds_fantasia"]),
                                ds_razaoSocial = t["ds_razaoSocial"].Equals(DBNull.Value) ? (string)null : Convert.ToString(t["ds_razaoSocial"]),
                                ds_endereco = t["ds_endereco"].Equals(DBNull.Value) ? (string)null : Convert.ToString(t["ds_endereco"]),
                                ds_cidade = t["ds_cidade"].Equals(DBNull.Value) ? (string)null : Convert.ToString(t["ds_cidade"]),
                                sg_uf = t["sg_uf"].Equals(DBNull.Value) ? (string)null : Convert.ToString(t["sg_uf"]),
                                nu_cep = t["nu_cep"].Equals(DBNull.Value) ? (string)null : Convert.ToString(t["nu_cep"]),
                                nu_telefone = t["nu_telefone"].Equals(DBNull.Value) ? (string)null : Convert.ToString(t["nu_telefone"]),
                                ds_bairro = t["ds_bairro"].Equals(DBNull.Value) ? (string)null : Convert.ToString(t["ds_bairro"]),
                                ds_email = Convert.ToString(t["ds_email"]),
                                dt_cadastro = (DateTime)t["dt_cadastro"],
                                fl_ativo = Convert.ToInt32(t["fl_ativo"]),
                                filial = t["filial"].Equals(DBNull.Value) ? (string)null : Convert.ToString(t["filial"]),
                                id_grupo = Convert.ToInt32(t["id_grupo"]),
                                nu_inscEstadual = t["nu_inscEstadual"].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(t["nu_inscEstadual"]),
                                ultimoAcesso = t["dt_ultimoAcesso"].Equals(DBNull.Value) ? (object)null :
                                               new
                                               {
                                                   login_ultimoAcesso = Convert.ToString(t["login_ultimoAcesso"]),
                                                   dt_ultimoAcesso = (DateTime)t["dt_ultimoAcesso"]
                                               },
                                podeExcluir = Convert.ToBoolean(t["podeExcluir"])
                            }).ToList<dynamic>();

                            int skipRows = (pageNumber - 1) * pageSize;
                            if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                                CollectionEmpresa = CollectionEmpresa.Skip(skipRows).Take(pageSize).ToList();
                            else
                                pageNumber = 1;
                        }

                    }
                    catch (Exception e)
                    {
                        if (e is DbEntityValidationException)
                        {
                            string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                            throw new Exception(erro.Equals("") ? "Falha ao listar recebimento parcela" : erro);
                        }
                        throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
                    }
                    finally
                    {
                        try
                        {
                            connection.Close();
                        }
                        catch { }
                    }
                }

                transaction.Commit();

                retorno.Registros = CollectionEmpresa;

                return retorno;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar empresa" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }



        /// <summary>
        /// Adiciona nova Empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Add(string token, empresa param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction(); // tudo ou nada
            try
            {
                param.dt_cadastro = DateTime.Now;
                param.fl_ativo = 1;
                param.token = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
                param.nu_BaseCnpj = param.nu_cnpj.Substring(0, 8);
                param.nu_SequenciaCnpj = param.nu_cnpj.Substring(8, 4);
                param.nu_DigitoCnpj = param.nu_cnpj.Substring(12, 2);
                if (param.filial == null)
                    param.filial = " ";


                _db.empresas.Add(param);
                _db.SaveChanges();
                //transaction.Commit();
                return param.nu_cnpj;
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar empresa" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }


        /// <summary>
        /// Apaga uma Empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, string nu_cnpj, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction(); // tudo ou nada
            try
            {
                //if (_db.LogAcesso1.Where(l => l.webpages_Users.nu_cnpjEmpresa == nu_cnpj).ToList().Count == 0)
                //{
                _db.empresas.Remove(_db.empresas.Where(e => e.nu_cnpj.Equals(nu_cnpj)).First());
                _db.SaveChanges();
                //transaction.Commit();
                //}
                //else
                //    throw new Exception("Empresa não pode ser deletada!");
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar empresa" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }



        /// <summary>
        /// Altera empresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, EmpresaAtualizar param, painel_taxservices_dbContext _dbContext = null)
        {
            painel_taxservices_dbContext _db;
            if (_dbContext == null) _db = new painel_taxservices_dbContext();
            else _db = _dbContext;
            //DbContextTransaction transaction = _db.Database.BeginTransaction(); // tudo ou nada
            try
            {
                empresa filial = _db.empresas
                        .Where(e => e.nu_cnpj.Equals(param.nu_cnpj))
                        .First<empresa>();


                if (param.novo_cnpj != null && param.novo_cnpj != filial.nu_cnpj)
                {
                    filial.nu_cnpj = param.novo_cnpj;
                    filial.nu_BaseCnpj = param.novo_cnpj.Substring(0, 8);
                    filial.nu_SequenciaCnpj = param.novo_cnpj.Substring(8, 4);
                    filial.nu_DigitoCnpj = param.novo_cnpj.Substring(12, 2);
                }

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
                //transaction.Commit();
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar empresa" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
            finally
            {
                if (_dbContext == null)
                {
                    // Fecha conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();
                }
            }
        }

    }
}
