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
    public class GatewayLoginOperadora
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayLoginOperadora()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            LOGIN = 101,
            SENHA = 102,
            DATA_ALTERACAO = 103,
            STATUS = 104,
            CNPJ = 105,
            IDOPERADORA = 106,
            IDGRUPO = 107,
            ESTABELECIMENTO = 108,

            // PERSONALIZADO
            NMOPERADORA = 201,

        };

        /// <summary>
        /// Get LoginOperadora/LoginOperadora
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<LoginOperadora> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.LoginOperadoras.AsQueryable<LoginOperadora>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.LOGIN:
                        string login = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.login.Equals(login)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.SENHA:
                        string senha = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.senha.Equals(senha)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.DATA_ALTERACAO:
                        DateTime data_alteracao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data_alteracao.Equals(data_alteracao)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.STATUS:
                        Boolean status = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.status.Equals(status)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.IDGRUPO:
                        Int32 idGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idGrupo.Equals(idGrupo)).AsQueryable<LoginOperadora>();
                        break;
                    case CAMPOS.ESTABELECIMENTO:
                        string estabelecimento = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.estabelecimento.Equals(estabelecimento)).AsQueryable<LoginOperadora>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.LOGIN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.login).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.login).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.SENHA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.senha).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.senha).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.DATA_ALTERACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data_alteracao).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.data_alteracao).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.STATUS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.status).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.status).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.IDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idGrupo).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.idGrupo).AsQueryable<LoginOperadora>();
                    break;
                case CAMPOS.ESTABELECIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.estabelecimento).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.estabelecimento).AsQueryable<LoginOperadora>();
                    break;
                
                // PERONALIZADO
                case CAMPOS.NMOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Operadora.nmOperadora).AsQueryable<LoginOperadora>();
                    else entity = entity.OrderByDescending(e => e.Operadora.nmOperadora).AsQueryable<LoginOperadora>();
                    break;


            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna LoginOperadora/LoginOperadora
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {

            // Implementar o filtro por Grupo apartir do TOKEN do Usuário

            string outValue = null;
            Int32 IdGrupo = Permissoes.GetIdGrupo(token);
            if (IdGrupo != 0)
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.IDGRUPO, out outValue))
                    queryString["" + (int)CAMPOS.IDGRUPO] = IdGrupo.ToString();
                else
                    queryString.Add("" + (int)CAMPOS.IDGRUPO, IdGrupo.ToString());
            }


            //DECLARAÇÕES
            List<dynamic> CollectionLoginOperadora = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
            var queryTotal = query;


            // PAGINAÇÃO
            if (colecao != 4) // senhas inválidas
            {
                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = queryTotal.Count();

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
                CollectionLoginOperadora = query.Select(e => new
                {

                    id = e.id,
                    login = e.login,
                    senha = e.senha,
                    data_alteracao = e.data_alteracao,
                    status = e.status,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idGrupo = e.idGrupo,
                    estabelecimento = e.estabelecimento,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionLoginOperadora = query.Select(e => new
                {

                    id = e.id,
                    login = e.login,
                    senha = e.senha,
                    data_alteracao = e.data_alteracao,
                    status = e.status,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idGrupo = e.idGrupo,
                    estabelecimento = e.estabelecimento,
                }).ToList<dynamic>();
            }
            else if (colecao == 2) // [mobile]
            {
                CollectionLoginOperadora = query.Select(e => new
                {

                    id = e.id,
                    login = e.login,
                    senha = e.senha,
                    status = e.status,
                    estabelecimento = e.estabelecimento,
                    operadora = e.Operadora.nmOperadora,
                    idOperadora = e.idOperadora
                }).ToList<dynamic>();
            }
            else if (colecao == 3) // [web]/Dados de Acesso/Grid
            {
                CollectionLoginOperadora = query.Select(e => new
                {

                    id = e.id,
                    login = e.login,
                    senha = e.senha,
                    status = e.status,
                    //empresa = new { cnpj = e.empresa.nu_cnpj, ds_fantasia = e.empresa.ds_fantasia },
                    operadora = new { id = e.Operadora.id ,desOperadora = e.Operadora.nmOperadora },
                    estabelecimento = e.estabelecimento
                }).ToList<dynamic>();

                CollectionLoginOperadora = CollectionLoginOperadora.OrderBy(l => l.operadora.desOperadora).ToList();
            }
            else if (colecao == 4) // [web]/Senhas Inválidas/Grid
            {
                var subQuery = query
                    .OrderBy(e => e.grupo_empresa.ds_nome).ThenBy(e => e.empresa.ds_fantasia).ThenBy(e => e.Operadora.nmOperadora)
                    .Where(e => e.status == false)
                    .Select(e => new
                                {

                                    id = e.id,
                                    login = e.login,
                                    senha = e.senha,
                                    status = e.status,
                                    empresa = e.empresa.ds_fantasia,
                                    grupoempresa = e.grupo_empresa.ds_nome,
                                    operadora = e.Operadora.nmOperadora,
                                    estabelecimento = e.estabelecimento
                                });

                retorno.TotalDeRegistros = subQuery.Count();

                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    subQuery = subQuery.Skip(skipRows).Take(pageSize);
                else
                    pageNumber = 1;

                CollectionLoginOperadora = subQuery.ToList<dynamic>();
            }


            
            retorno.Registros = CollectionLoginOperadora;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova LoginOperadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, LoginOperadora param)
        {
            Operadora newOperadora = new Operadora();
            newOperadora.nmOperadora = param.Operadora.nmOperadora;
            param.Operadora.idGrupoEmpresa = param.idGrupo;
            newOperadora.idGrupoEmpresa = param.idGrupo;
            _db.Operadoras.Add(newOperadora);
            _db.SaveChanges();

            param.status = true;
            param.idOperadora = newOperadora.id;
            _db.LoginOperadoras.Add(param);
            _db.SaveChanges();

            /*

{ 
    login: "meulogin", 
    senha: "123456", 
    cnpj: "22388768000117", 
    idGrupo: 41, 
    estabelecimento: "meuestabelecimento",
    operadora :
                {
                    nmOperadora: "BANESE CARD"
                }
}
             */
            var op = _db.Adquirentes.Where(o => o.descricao.Equals(newOperadora.nmOperadora)).Select(o => o).FirstOrDefault();
            DateTime hrExec = (DateTime)op.hraExecucao;

            LogExecution newLogExecution = new LogExecution();
            newLogExecution.idLoginOperadora = param.id;
            newLogExecution.idOperadora = param.idOperadora;
            newLogExecution.statusExecution = "7"; //0 = Em execução; 1 = Executado com Sucesso; 2 = Erro na Execução; 3 = Re-Executar; 7 = Elegivel
            newLogExecution.dtaFiltroTransacoes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, hrExec.Hour, hrExec.Minute, hrExec.Second); ;
            newLogExecution.dtaFiltroTransacoesFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, hrExec.Hour, hrExec.Minute, hrExec.Second);
            newLogExecution.dtaExecucaoProxima = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2, hrExec.Hour, hrExec.Minute, hrExec.Second);

            _db.LogExecutions.Add(newLogExecution);
            _db.SaveChanges();

            return param.id;
        }


        /// <summary>
        /// Apaga uma LoginOperadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.LoginOperadoras.Remove(_db.LoginOperadoras.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera LoginOperadora
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, LoginOperadora param)
        {
            LoginOperadora value = _db.LoginOperadoras
                    .Where(e => e.id.Equals(param.id))
                    .First<LoginOperadora>();


            if (param.login != null && param.login != value.login)
                value.login = param.login;
            if (param.senha != null && param.senha != value.senha)
            {
                value.senha = param.senha;
                value.status = true;
                value.data_alteracao = DateTime.Now;
            }
            if (param.estabelecimento != null && param.estabelecimento != value.estabelecimento)
                value.estabelecimento = param.estabelecimento;
            _db.SaveChanges();

        }

    }
}
