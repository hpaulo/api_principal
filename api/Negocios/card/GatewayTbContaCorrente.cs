using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using api.Negocios.Util;
using System.Data.Entity.Validation;

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
            CDCONTACORRENTE = 100,
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
                    case CAMPOS.CDCONTACORRENTE:
                        Int32 cdContaCorrente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdContaCorrente == cdContaCorrente).AsQueryable<tbContaCorrente>();
                        break;
                    case CAMPOS.CDGRUPO:
                        Int32 cdGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdGrupo == cdGrupo).AsQueryable<tbContaCorrente>();
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
                case CAMPOS.CDCONTACORRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdContaCorrente).AsQueryable<tbContaCorrente>();
                    else entity = entity.OrderByDescending(e => e.cdContaCorrente).AsQueryable<tbContaCorrente>();
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
            try
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

                        cdContaCorrente = e.cdContaCorrente,
                        cdGrupo = e.cdGrupo,
                        nrCnpj = e.nrCnpj,
                        cdBanco = e.cdBanco,
                        nrAgencia = e.nrAgencia,
                        nrConta = e.nrConta,
                        flAtivo = e.flAtivo
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbContaCorrente = query.Select(e => new
                    {

                        cdContaCorrente = e.cdContaCorrente,
                        cdGrupo = e.cdGrupo,
                        nrCnpj = e.nrCnpj,
                        cdBanco = e.cdBanco,
                        nrAgencia = e.nrAgencia,
                        nrConta = e.nrConta,
                        flAtivo = e.flAtivo
                    }).ToList<dynamic>();
                }
                else if (colecao == 2) // [WEB] 
                {
                    List<dynamic> contas = query.Select(e => new
                    {
                        cdContaCorrente = e.cdContaCorrente,
                        cdGrupo = e.cdGrupo,
                        empresa = new
                        {
                            nu_cnpj = e.nrCnpj,
                            ds_fantasia = e.empresa.ds_fantasia,
                            filial = e.empresa.filial
                        },
                        banco = new { Codigo = e.cdBanco, NomeExtenso = "" }, // Não dá para chamar a função direto daqui pois esse código é convertido em SQL e não acessa os dados de um objeto em memória
                        nrAgencia = e.nrAgencia,
                        nrConta = e.nrConta,
                        flAtivo = e.flAtivo,
                        podeAtualizar = e.tbContaCorrente_tbLoginAdquirenteEmpresas.Count() == 0 && e.tbExtratos.Count() == 0
                    }).ToList<dynamic>();

                    // Após transformar em lista (isto é, trazer para a memória), atualiza o valor do NomeExtenso associado ao banco
                    foreach (var conta in contas)
                    {
                        CollectionTbContaCorrente.Add(new
                        {
                            cdContaCorrente = conta.cdContaCorrente,
                            cdGrupo = conta.cdGrupo,
                            empresa = conta.empresa,
                            banco = new { Codigo = conta.banco.Codigo, NomeExtenso = GatewayBancos.Get(conta.banco.Codigo) },
                            nrAgencia = conta.nrAgencia,
                            nrConta = conta.nrConta,
                            flAtivo = conta.flAtivo,
                            podeAtualizar = conta.podeAtualizar
                        });
                    }
                }

                retorno.Registros = CollectionTbContaCorrente;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar conta corrente" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbContaCorrente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbContaCorrente param)
        {
            try
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
                    param.flAtivo = true;
                    _db.tbContaCorrentes.Add(param);
                    _db.SaveChanges();
                    return param.cdContaCorrente;
                }

                throw new Exception("Conta já cadastrada!");
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar conta corrente" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbContaCorrente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 cdContaCorrente)
        {
            try
            {
                tbContaCorrente conta = _db.tbContaCorrentes.Where(e => e.cdContaCorrente == cdContaCorrente).FirstOrDefault();
                if (conta == null) throw new Exception("Conta inexistente!");
                // Remove as vigências
                /*GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.Delete(token, conta.cdContaCorrente);
                // Remove os extratos e os arquivos associados
                List<tbExtrato> extratos = _db.tbExtratos.Where(e => e.cdContaCorrente == conta.cdContaCorrente).ToList<tbExtrato>();
                foreach (var extrato in extratos) GatewayTbExtrato.Delete(token, extrato.idExtrato);*/
                // Remove a conta
                _db.tbContaCorrentes.Remove(conta);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar conta corrente" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Altera tbContaCorrente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbContaCorrente param)
        {
            try
            {
                tbContaCorrente value = _db.tbContaCorrentes
                        .Where(e => e.cdContaCorrente == param.cdContaCorrente)
                        .First<tbContaCorrente>();

                if (value == null) throw new Exception("Conta inexistente!");

                if (param.nrCnpj != null && param.nrCnpj != value.nrCnpj)
                    value.nrCnpj = param.nrCnpj;
                if (param.cdBanco != null && param.cdBanco != value.cdBanco)
                    value.cdBanco = param.cdBanco;
                if (param.nrAgencia != null && param.nrAgencia != value.nrAgencia)
                    value.nrAgencia = param.nrAgencia;
                if (param.nrConta != null && param.nrConta != value.nrConta)
                    value.nrConta = param.nrConta;
                if (param.flAtivo != value.flAtivo)
                    value.flAtivo = param.flAtivo;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar conta corrente" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
