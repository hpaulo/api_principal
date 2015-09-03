using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;

namespace api.Negocios.Admin
{
    public class GatewayTbEmpresa
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbEmpresa()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            NRCNPJBASE = 100,
            DSCERTIFICADODIGITAL = 101,
            DSCERTIFICADODIGITALSENHA = 102,
            CDEMPRESAGRUPO = 103,
            DTCADASTRO = 104,
            DTVALIDADE = 105,
            FLSENHAVALIDA = 106,
        };

        /// <summary>
        /// Get TbEmpresa/TbEmpresa
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbEmpresa> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbEmpresas.AsQueryable();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.NRCNPJBASE:
                        string nrCNPJBase = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJBase.Equals(nrCNPJBase)).AsQueryable();
                        break;
                    case CAMPOS.DSCERTIFICADODIGITAL:
                        string dsCertificadoDigital = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsCertificadoDigital.Equals(dsCertificadoDigital)).AsQueryable();
                        break;
                    case CAMPOS.DSCERTIFICADODIGITALSENHA:
                        string dsCertificadoDigitalSenha = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsCertificadoDigitalSenha.Equals(dsCertificadoDigitalSenha)).AsQueryable();
                        break;
                    case CAMPOS.CDEMPRESAGRUPO:
                        Int32 cdEmpresaGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdEmpresaGrupo.Equals(cdEmpresaGrupo)).AsQueryable();
                        break;
                    case CAMPOS.DTCADASTRO:
                        DateTime dtCadastro = DateTime.ParseExact(item.Value + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        entity = entity.Where(e => e.dtCadastro != null && e.dtCadastro.Value.Year == dtCadastro.Year && e.dtCadastro.Value.Month == dtCadastro.Month && e.dtCadastro.Value.Day == dtCadastro.Day).AsQueryable();
                        break;
                    case CAMPOS.DTVALIDADE:
                        DateTime dtValidade = DateTime.ParseExact(item.Value + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        entity = entity.Where(e => e.dtValidade != null && e.dtValidade.Value.Year == dtValidade.Year && e.dtValidade.Value.Month == dtValidade.Month && e.dtValidade.Value.Day == dtValidade.Day).AsQueryable();
                        break;
                    case CAMPOS.FLSENHAVALIDA:
                        bool flSenhaValida = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flSenhaValida == flSenhaValida).AsQueryable();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.NRCNPJBASE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJBase).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.nrCNPJBase).AsQueryable();
                    break;
                case CAMPOS.DSCERTIFICADODIGITAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsCertificadoDigital).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsCertificadoDigital).AsQueryable();
                    break;
                case CAMPOS.DSCERTIFICADODIGITALSENHA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsCertificadoDigitalSenha).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dsCertificadoDigitalSenha).AsQueryable();
                    break;
                case CAMPOS.CDEMPRESAGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEmpresaGrupo).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.cdEmpresaGrupo).AsQueryable();
                    break;
                case CAMPOS.DTCADASTRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtCadastro).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtCadastro).AsQueryable();
                    break;
                case CAMPOS.DTVALIDADE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtValidade).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.dtValidade).AsQueryable();
                    break;
                case CAMPOS.FLSENHAVALIDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flSenhaValida).AsQueryable();
                    else entity = entity.OrderByDescending(e => e.flSenhaValida).AsQueryable();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbEmpresa/TbEmpresa
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbEmpresa = new List<dynamic>();
                Retorno retorno = new Retorno();
                retorno.Totais = new Dictionary<string, object>();

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
                    CollectionTbEmpresa = _db.tbEmpresas
                       .Join(_db.tbEmpresaFiliais, e => e.nrCNPJBase, f => f.nrCNPJBase, (e, f) => new { e, f })
                       .Join(_db.tbEmpresaGrupos, f => f.f.cdEmpresaGrupo, g => g.cdEmpresaGrupo, (f, g) => new { f, g })
                       .Where(e => e.f.e.dsCertificadoDigital != null && e.f.e.dsCertificadoDigitalSenha != null && e.f.f.flAtivo == true && e.g.flTaxServices == true && e.g.flAtivo == true && e.f.e.dtValidade >= DateTime.Now && e.f.e.flSenhaValida != false)
                       .GroupBy(e => new { e.f.e.cdEmpresaGrupo, e.f.e.dsCertificadoDigitalSenha, e.f.f.nrCNPJ, e.f.e.nrCNPJBase })
                       .Select(e => new
                       {
                           cdEmpresaGrupo = e.Key.cdEmpresaGrupo,

                           dsCertificadoDigital = _db.tbEmpresas
                           .Where(t => t.nrCNPJBase.Equals(e.Key.nrCNPJBase))
                           .Select(s => new { s.dsCertificadoDigital }).FirstOrDefault().dsCertificadoDigital,

                           dsCertificadoDigitalSenha = e.Key.dsCertificadoDigitalSenha,
                           nrCNPJ = e.Key.nrCNPJ
                       })
                       .AsQueryable().ToList<dynamic>();

                }
                else if (colecao == 0)
                {
                    CollectionTbEmpresa = query.Select(e => new
                    {
                        nrCNPJBase = e.nrCNPJBase,
                        dsCertificadoDigital = e.dsCertificadoDigital,
                        dsCertificadoDigitalSenha = e.dsCertificadoDigitalSenha,
                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                        dtCadastro = e.dtCadastro,
                        dtValidade = e.dtValidade,
                        flSenhaValida = e.flSenhaValida,
                    }).ToList<dynamic>();
                }

                else if (colecao == 2)
                {
                    CollectionTbEmpresa = query.Select(e => new
                    {

                        nrCNPJBase = e.nrCNPJBase,
                        dsCertificadoDigital = e.dsCertificadoDigital,
                        dsCertificadoDigitalSenha = e.dsCertificadoDigitalSenha,
                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                    }).Take(1).ToList<dynamic>();
                }


                retorno.TotalDeRegistros = CollectionTbEmpresa.Count;
                retorno.Registros = CollectionTbEmpresa;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar TbEmpresa" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova TbEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Add(string token, tbEmpresa param)
        {
            try
            {
                _db.tbEmpresas.Add(param);
                _db.SaveChanges();
                return param.nrCNPJBase;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar TbEmpresa" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, string nrCNPJBase)
        {
            try
            {
                _db.tbEmpresas.Remove(_db.tbEmpresas.Where(e => e.nrCNPJBase.Equals(nrCNPJBase)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar TbEmpresa" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera tbEmpresa
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Retorno Update(string token, tbEmpresa param)
        {
            try
            {
                tbEmpresa value = _db.tbEmpresas
                        .Where(e => e.nrCNPJBase.Equals(param.nrCNPJBase))
                        .First<tbEmpresa>();

                Retorno retorno = new Retorno();

                HttpRequest httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count == 0) throw new Exception("Não foi identificado o certificado digital");
                // Obtém o arquivo
                HttpPostedFile postedFile = httpRequest.Files[0];
                
                // Converte para um array de bytes
                MemoryStream memoryStream = postedFile.InputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    postedFile.InputStream.CopyTo(memoryStream);
                }
                byte[] data = new byte[memoryStream.ToArray().Length];
                memoryStream.Read(data, 0, data.Length);
                memoryStream.Close();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NOS PARAMETROS
                if (data != null && data != value.dsCertificadoDigital)
                    value.dsCertificadoDigital = data;
                if (param.dsCertificadoDigitalSenha != null && param.dsCertificadoDigitalSenha != value.dsCertificadoDigitalSenha)
                    value.dsCertificadoDigitalSenha = param.dsCertificadoDigitalSenha;

                Mensagem mensagem = CertificadoDigital.ValidarCertificado(value.dsCertificadoDigital, value.dsCertificadoDigitalSenha);
                if (mensagem.cdMensagem == 200)
                {
                    value.dtValidade = CertificadoDigital.GetDataValidade(param.dsCertificadoDigital, param.dsCertificadoDigitalSenha);
                    value.flSenhaValida = true;

                    _db.SaveChanges();
                }

                retorno.Registros.Add(mensagem);
                
                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar TbEmpresa" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
