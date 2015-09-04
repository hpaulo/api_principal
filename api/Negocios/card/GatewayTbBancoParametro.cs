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
    public class GatewayTbBancoParametro
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbBancoParametro()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDBANCO = 100,
            DSMEMO = 101,
            CDADQUIRENTE = 102,
            DSTIPO = 103,
            FLVISIVEL = 104,
            NRCNPJ = 105,

            // RELACIONAMENTOS
            DSADQUIRENTE = 201,

        };

        /// <summary>
        /// Get TbBancoParametro/TbBancoParametro
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbBancoParametro> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL
            var entity = _db.tbBancoParametro.AsQueryable<tbBancoParametro>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDBANCO:
                        string cdBanco = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdBanco.Equals(cdBanco)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.DSMEMO:
                        string dsMemo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsMemo.Equals(dsMemo)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        if(cdAdquirente == -1)
                            // Somente os registros sem adquirentes associados
                            entity = entity.Where(e => e.cdAdquirente == null).AsQueryable<tbBancoParametro>();
                        else
                            entity = entity.Where(e => e.cdAdquirente == cdAdquirente).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.DSTIPO:
                        string dsTipo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsTipo.Equals(dsTipo)).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.FLVISIVEL:
                        bool flVisivel = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flVisivel == flVisivel).AsQueryable<tbBancoParametro>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCnpj = Convert.ToString(item.Value);
                        if(nrCnpj.Equals("")) entity = entity.Where(e => e.nrCnpj == null).AsQueryable<tbBancoParametro>();
                        else entity = entity.Where(e => e.nrCnpj.Equals(nrCnpj)).AsQueryable<tbBancoParametro>();
                        break;

                    // PERSONALIZADO
                    case CAMPOS.DSADQUIRENTE:
                        string nome = Convert.ToString(item.Value);
                        if (nome.Contains("%")) // usa LIKE
                        {
                            string busca = nome.Replace("%", "").ToString();
                            entity = entity.Where(e => e.tbAdquirente.dsAdquirente.Contains(busca)).AsQueryable<tbBancoParametro>();
                        }
                        else
                            entity = entity.Where(e => e.tbAdquirente.dsAdquirente.Equals(nome)).AsQueryable<tbBancoParametro>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDBANCO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBanco).ThenBy(e => e.dsMemo).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.cdBanco).ThenByDescending(e => e.dsMemo).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.DSMEMO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsMemo).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.dsMemo).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.DSTIPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsTipo).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.dsTipo).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.FLVISIVEL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flVisivel).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.flVisivel).AsQueryable<tbBancoParametro>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCnpj).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.nrCnpj).AsQueryable<tbBancoParametro>();
                    break;

                // PERSONALIZADO
                case CAMPOS.DSADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tbAdquirente.dsAdquirente).AsQueryable<tbBancoParametro>();
                    else entity = entity.OrderByDescending(e => e.tbAdquirente.dsAdquirente).AsQueryable<tbBancoParametro>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbBancoParametro/TbBancoParametro
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbBancoParametro = new List<dynamic>();
            Retorno retorno = new Retorno();

            // POR DEFAULT, LISTA SOMENTE OS QUE ESTÃO VISÍVEIS
            string outValue = null;
            if (!queryString.TryGetValue("" + (int)CAMPOS.FLVISIVEL, out outValue))
                queryString.Add("" + (int)CAMPOS.FLVISIVEL, Convert.ToString(true));

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
                CollectionTbBancoParametro = query.Select(e => new
                {

                    cdBanco = e.cdBanco,
                    dsMemo = e.dsMemo,
                    cdAdquirente = e.cdAdquirente,
                    dsTipo = e.dsTipo,
                    flVisivel = e.flVisivel,
                    nrCnpj = e.nrCnpj
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbBancoParametro = query.Select(e => new
                {

                    cdBanco = e.cdBanco,
                    dsMemo = e.dsMemo,
                    cdAdquirente = e.cdAdquirente,
                    dsTipo = e.dsTipo,
                    flVisivel = e.flVisivel,
                    nrCnpj = e.nrCnpj
                }).ToList<dynamic>();
            }
            else if (colecao == 2) // [WEB] 
            {
                List<dynamic> bancoParametros = query.Select(e => new
                {
                    dsMemo = e.dsMemo,
                    adquirente = e.cdAdquirente == null ? null : new 
                                    {
                                        cdAdquirente = e.tbAdquirente.cdAdquirente,
                                        nmAdquirente = e.tbAdquirente.nmAdquirente,
                                        dsAdquirente = e.tbAdquirente.dsAdquirente,
                                        stAdquirente = e.tbAdquirente.stAdquirente,
                                    },
                    dsTipo = e.dsTipo,
                    flVisivel = e.flVisivel,
                    empresa = e.nrCnpj == null ? null : new
                    {
                        nu_cnpj = e.empresa.nu_cnpj,
                        ds_fantasia = e.empresa.ds_fantasia,
                        filial = e.empresa.filial
                    },
                    grupoempresa = e.nrCnpj == null ? null : new
                    {
                        id_grupo = e.empresa.id_grupo,
                        ds_nome = e.empresa.grupo_empresa.ds_nome
                    },
                    banco = new { Codigo = e.cdBanco, NomeExtenso = "" }, // Não dá para chamar a função direto daqui pois esse código é convertido em SQL e não acessa os dados de um objeto em memória
                }).ToList<dynamic>();

                // Após transformar em lista (isto é, trazer para a memória), atualiza o valor do NomeExtenso associado ao banco
                foreach (var bancoParametro in bancoParametros)
                {
                    CollectionTbBancoParametro.Add(new
                    {
                        dsMemo = bancoParametro.dsMemo,
                        adquirente = bancoParametro.adquirente,
                        dsTipo = bancoParametro.dsTipo,
                        flVisivel = bancoParametro.flVisivel,
                        empresa = bancoParametro.empresa,
                        grupoempresa = bancoParametro.grupoempresa,
                        banco = new { Codigo = bancoParametro.banco.Codigo, NomeExtenso = GatewayBancos.Get(bancoParametro.banco.Codigo) },
                    });
                }
            }

            retorno.Registros = CollectionTbBancoParametro;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbBancoParametro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Add(string token, tbBancoParametro param)
        {
            tbBancoParametro parametro = _db.tbBancoParametro.Where(e => e.cdBanco.Equals(param.cdBanco))
                                                             .Where(e => e.dsMemo.Equals(param.dsMemo))
                                                             .FirstOrDefault();

            if (parametro != null) throw new Exception("Parâmetro bancário já existe");

            if (param.cdAdquirente == -1) param.cdAdquirente = null;
            if (param.nrCnpj != null && param.nrCnpj.Equals("")) param.nrCnpj = null;

            param.flVisivel = true;

            _db.tbBancoParametro.Add(param);
            _db.SaveChanges();
            return param.cdBanco;
        }


        /// <summary>
        /// Apaga uma TbBancoParametro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, string cdBanco, string dsMemo)
        {
            tbBancoParametro parametro = _db.tbBancoParametro.Where(e => e.cdBanco.Equals(cdBanco))
                                                             .Where(e => e.dsMemo.Equals(dsMemo))
                                                             .FirstOrDefault();

            if (parametro == null) throw new Exception("Parâmetro bancário inexistente");

            _db.tbBancoParametro.Remove(parametro);
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbBancoParametro
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, ParametrosBancarios param)//tbBancoParametro param)
        {
            foreach (ParametroBancario parametro in param.Parametros)
            {
                if (param.Deletar)
                {
                    try { Delete(token, parametro.CdBanco, parametro.DsMemo); } catch { }
                }
                else
                {

                    tbBancoParametro value = _db.tbBancoParametro.Where(e => e.cdBanco.Equals(parametro.CdBanco))
                                                                 .Where(e => e.dsMemo.Equals(parametro.DsMemo))
                                                                 .FirstOrDefault();

                    if (value != null)
                    {
                        // TIPO
                        if (parametro.DsTipo != null && parametro.DsTipo != value.dsTipo)
                            value.dsTipo = parametro.DsTipo;
                        // Adquirente
                        if (param.CdAdquirente != null && param.CdAdquirente != value.cdAdquirente)
                        {
                            if (param.CdAdquirente == -1) value.cdAdquirente = null;
                            else value.cdAdquirente = param.CdAdquirente;
                        }
                        // Filial
                        if (param.NrCnpj != null && param.NrCnpj != value.nrCnpj)
                        {
                            if (param.NrCnpj.Equals("")) value.nrCnpj = null;
                            else value.nrCnpj = param.NrCnpj;
                        }
                        // Visibilidade
                        if (param.FlVisivel != value.flVisivel) value.flVisivel = param.FlVisivel;
                        // Salva
                        _db.SaveChanges();
                    }
                }
            }

            /*tbBancoParametro value = _db.tbBancoParametro.Where(e => e.cdBanco.Equals(param.cdBanco))
                                                             .Where(e => e.dsMemo.Equals(param.dsMemo))
                                                             .FirstOrDefault();

            if (value == null) throw new Exception("Parâmetro bancário inexistente");


            //if (param.cdBanco != null && param.cdBanco != value.cdBanco)
            //    value.cdBanco = param.cdBanco;
            //if (param.dsMemo != null && param.dsMemo != value.dsMemo)
            //    value.dsMemo = param.dsMemo;
            if (param.cdAdquirente != null && param.cdAdquirente != value.cdAdquirente)
            {
                if (param.cdAdquirente == -1) value.cdAdquirente = null;
                else value.cdAdquirente = param.cdAdquirente;
            }
            if (param.NrCnpj != null && param.nrCnpj != value.nrCnpj)
            {
                if (param.nrCnpj.Equals("")) value.nrCnpj = null;
                else value.nrCnpj = param.nrCnpj;
            }
            if (param.dsTipo != null && param.dsTipo != value.dsTipo)
                value.dsTipo = param.dsTipo;
            if (param.flVisivel != value.flVisivel)
                value.flVisivel = param.flVisivel;
            _db.SaveChanges();*/

        }

    }
}
