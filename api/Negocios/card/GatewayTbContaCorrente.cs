using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using api.Negocios.Util;

namespace api.Negocios.Card
{
    public class GatewayTbContaCorrente
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbContaCorrente()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDCONTACORRENTE = 100,
            CDGRUPO = 101,
            NRCNPJ = 102,
            CDBANCO = 103,
            NRAGENCIA = 104,
            NRCONTA = 105,

        };

        /// <summary>
        /// Get TbContaCorrente/TbContaCorrente
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbContaCorrente> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbContaCorrentes.AsQueryable<tbContaCorrente>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDCONTACORRENTE:
                        Int32 idContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idContaCorrente.Equals(idContaCorrente)).AsQueryable<tbContaCorrente>();
                        break;
                    case CAMPOS.CDGRUPO:
                        Int32 cdGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdGrupo.Equals(cdGrupo)).AsQueryable<tbContaCorrente>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCnpj.Equals(nrCnpj)).AsQueryable<tbContaCorrente>();
                        break;
                    case CAMPOS.CDBANCO:
                        string cdBanco = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdBanco.Equals(cdBanco)).AsQueryable<tbContaCorrente>();
                        break;
                    case CAMPOS.NRAGENCIA:
                        string nrAgencia = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrAgencia.Equals(nrAgencia)).AsQueryable<tbContaCorrente>();
                        break;
                    case CAMPOS.NRCONTA:
                        string nrConta = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrConta.Equals(nrConta)).AsQueryable<tbContaCorrente>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idContaCorrente).AsQueryable<tbContaCorrente>();
                    else entity = entity.OrderByDescending(e => e.idContaCorrente).AsQueryable<tbContaCorrente>();
                    break;
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdGrupo).ThenBy(e => e.nrAgencia).ThenBy(e => e.nrConta).ThenBy(e => e.nrCnpj).AsQueryable<tbContaCorrente>();
                    else entity = entity.OrderByDescending(e => e.cdGrupo).ThenByDescending(e => e.nrAgencia).ThenByDescending(e => e.nrConta).ThenByDescending(e => e.nrCnpj).AsQueryable<tbContaCorrente>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCnpj).AsQueryable<tbContaCorrente>();
                    else entity = entity.OrderByDescending(e => e.nrCnpj).AsQueryable<tbContaCorrente>();
                    break;
                case CAMPOS.CDBANCO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBanco).AsQueryable<tbContaCorrente>();
                    else entity = entity.OrderByDescending(e => e.cdBanco).AsQueryable<tbContaCorrente>();
                    break;
                case CAMPOS.NRAGENCIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrAgencia).AsQueryable<tbContaCorrente>();
                    else entity = entity.OrderByDescending(e => e.nrAgencia).AsQueryable<tbContaCorrente>();
                    break;
                case CAMPOS.NRCONTA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrConta).AsQueryable<tbContaCorrente>();
                    else entity = entity.OrderByDescending(e => e.nrConta).AsQueryable<tbContaCorrente>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbContaCorrente/TbContaCorrente
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            // Implementar o filtro por Grupo apartir do TOKEN do Usuário
            string outValue = null;
            Int32 IdGrupo = 0;
            IdGrupo = Permissoes.GetIdGrupo(token);
            if (IdGrupo != 0)
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.CDGRUPO, out outValue))
                    queryString["" + (int)CAMPOS.CDGRUPO] = IdGrupo.ToString();
                else
                    queryString.Add("" + (int)CAMPOS.CDGRUPO, IdGrupo.ToString());
            }
            string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
            if (CnpjEmpresa != "")
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.NRCNPJ, out outValue))
                    queryString["" + (int)CAMPOS.NRCNPJ] = CnpjEmpresa;
                else
                    queryString.Add("" + (int)CAMPOS.NRCNPJ, CnpjEmpresa);
            }

            //DECLARAÇÕES
            List<dynamic> CollectionTbContaCorrente = new List<dynamic>();
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
                CollectionTbContaCorrente = query.Select(e => new
                {

                    idContaCorrente = e.idContaCorrente,
                    cdGrupo = e.cdGrupo,
                    nrCnpj = e.nrCnpj,
                    cdBanco = e.cdBanco,
                    nrAgencia = e.nrAgencia,
                    nrConta = e.nrConta,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbContaCorrente = query.Select(e => new
                {

                    idContaCorrente = e.idContaCorrente,
                    cdGrupo = e.cdGrupo,
                    nrCnpj = e.nrCnpj,
                    cdBanco = e.cdBanco,
                    nrAgencia = e.nrAgencia,
                    nrConta = e.nrConta,
                }).ToList<dynamic>();
            }
            else if (colecao == 2) // [WEB] 
            {
                List<dynamic> contas = query.Select(e => new
                {
                    idContaCorrente = e.idContaCorrente,
                    cdGrupo = e.cdGrupo,
                    empresa = new { nu_cnpj = e.nrCnpj, ds_fantasia = e.empresa.ds_fantasia },
                    banco = new { Codigo = e.cdBanco, NomeExtenso = "" }, // Não dá para chamar a função direto daqui pois esse código é convertido em SQL e não acessa os dados de um objeto em memória
                    nrAgencia = e.nrAgencia,
                    nrConta = e.nrConta,
                    vigentes = e.tbContaCorrente_tbLoginAdquirenteEmpresas
                                            .OrderBy(l => l.tbLoginAdquirenteEmpresa.empresa.ds_fantasia)
                                            .ThenBy(l => l.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente)
                                            .Where(l => l.dtFim == null) // somente as que estão vigentes
                                            .Select(l => new
                                                        {
                                                            dtInicio = l.dtInicio,
                                                            //dtFim = l.dtFim, // sempre NULL
                                                            loginAdquirenteEmpresa = new
                                                            {
                                                                cdLoginAdquirenteEmpresa = l.tbLoginAdquirenteEmpresa.cdLoginAdquirenteEmpresa,
                                                                adquirente = new
                                                                {
                                                                    cdAdquirente = l.tbLoginAdquirenteEmpresa.tbAdquirente.cdAdquirente,
                                                                    nmAdquirente = l.tbLoginAdquirenteEmpresa.tbAdquirente.nmAdquirente,
                                                                    dsAdquirente = l.tbLoginAdquirenteEmpresa.tbAdquirente.dsAdquirente,
                                                                    stAdquirente = l.tbLoginAdquirenteEmpresa.tbAdquirente.stAdquirente,
                                                                },
                                                                empresa = new {
                                                                    nu_cnpj = l.tbLoginAdquirenteEmpresa.empresa.nu_cnpj,
                                                                    ds_fantasia = l.tbLoginAdquirenteEmpresa.empresa.ds_fantasia
                                                                },
                                                                stLoginAdquirente = l.tbLoginAdquirenteEmpresa.stLoginAdquirente, 
                                                                //stLoginAdquirenteEmpresa = l.tbLoginAdquirenteEmpresa.stLoginAdquirenteEmpresa // controle de bruno
                                                            }
                                                        }
                                            ).ToList<dynamic>()
                }).ToList<dynamic>();

                // Após transformar em lista (isto é, trazer para a memória), atualiza o valor do NomeExtenso associado ao banco
                foreach( var conta in contas )
                {
                    CollectionTbContaCorrente.Add(new
                    {
                        idContaCorrente = conta.idContaCorrente,
                        cdGrupo = conta.cdGrupo,
                        empresa = conta.empresa,
                        banco = new { Codigo = conta.banco.Codigo, NomeExtenso = GatewayBancos.Get(conta.banco.Codigo) },
                        nrAgencia = conta.nrAgencia,
                        nrConta = conta.nrConta,
                        vigentes = conta.vigentes,
                    });
                }
            }

            retorno.Registros = CollectionTbContaCorrente;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbContaCorrente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbContaCorrente param)
        {
            var verify = _db.tbContaCorrentes
                            .Where(e => e.cdGrupo == param.cdGrupo)
                            .Where(e => e.nrCnpj.Equals(param.nrCnpj))
                            .Where(e => e.cdBanco.Equals(param.cdBanco))
                            .Where(e => e.nrAgencia.Equals(param.nrAgencia))
                            .Where(e => e.nrConta.Equals(param.nrConta))
                            .FirstOrDefault();

            if (verify == null)
            {
                _db.tbContaCorrentes.Add(param);
                _db.SaveChanges();
                return param.idContaCorrente;
            }
               
            throw new Exception("Conta já cadastrada!");
        }


        /// <summary>
        /// Apaga uma TbContaCorrente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idContaCorrente)
        {
            // Deletar todos os Extratos?
            _db.tbContaCorrentes.Remove(_db.tbContaCorrentes.Where(e => e.idContaCorrente.Equals(idContaCorrente)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbContaCorrente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbContaCorrente param)
        {
            tbContaCorrente value = _db.tbContaCorrentes
                    .Where(e => e.idContaCorrente == param.idContaCorrente)
                    .First<tbContaCorrente>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS
            
            if (param.nrCnpj != null && param.nrCnpj != value.nrCnpj)
                value.nrCnpj = param.nrCnpj;
            if (param.cdBanco != null && param.cdBanco != value.cdBanco)
                value.cdBanco = param.cdBanco;
            if (param.nrAgencia != null && param.nrAgencia != value.nrAgencia)
                value.nrAgencia = param.nrAgencia;
            if (param.nrConta != null && param.nrConta != value.nrConta)
                value.nrConta = param.nrConta;
            _db.SaveChanges();

        }

    }
}
