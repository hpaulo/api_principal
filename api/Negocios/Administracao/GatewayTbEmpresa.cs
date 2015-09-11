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

            // RELACIONAMENTOS
            DSEMPRESAGRUPO = 201,
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
            var entity = _db.tbEmpresas.AsQueryable<tbEmpresa>();

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
                        entity = entity.Where(e => e.nrCNPJBase.Equals(nrCNPJBase)).AsQueryable<tbEmpresa>();
                        break;
                    case CAMPOS.DSCERTIFICADODIGITAL:
                        string dsCertificadoDigital = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsCertificadoDigital.Equals(dsCertificadoDigital)).AsQueryable<tbEmpresa>();
                        break;
                    case CAMPOS.DSCERTIFICADODIGITALSENHA:
                        string dsCertificadoDigitalSenha = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsCertificadoDigitalSenha.Equals(dsCertificadoDigitalSenha)).AsQueryable<tbEmpresa>();
                        break;
                    case CAMPOS.CDEMPRESAGRUPO:
                        Int32 cdEmpresaGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdEmpresaGrupo.Equals(cdEmpresaGrupo)).AsQueryable<tbEmpresa>();
                        break;
                    case CAMPOS.DTCADASTRO:
                        DateTime dtCadastro = DateTime.ParseExact(item.Value + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        entity = entity.Where(e => e.dtCadastro != null && e.dtCadastro.Value.Year == dtCadastro.Year && e.dtCadastro.Value.Month == dtCadastro.Month && e.dtCadastro.Value.Day == dtCadastro.Day).AsQueryable<tbEmpresa>();
                        break;
                    case CAMPOS.DTVALIDADE:
                        DateTime dtValidade = DateTime.ParseExact(item.Value + " 00:00:00.000", "yyyyMMdd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        entity = entity.Where(e => e.dtValidade != null && e.dtValidade.Value.Year == dtValidade.Year && e.dtValidade.Value.Month == dtValidade.Month && e.dtValidade.Value.Day == dtValidade.Day).AsQueryable<tbEmpresa>();
                        break;
                    case CAMPOS.FLSENHAVALIDA:
                        bool flSenhaValida = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.flSenhaValida == flSenhaValida).AsQueryable<tbEmpresa>();
                        break;

                    // RELACIONAMENTOS
                    case CAMPOS.DSEMPRESAGRUPO:
                        string dsEmpresaGrupo = Convert.ToString(item.Value);
                        if (dsEmpresaGrupo.Contains("%"))
                        {
                            string busca = dsEmpresaGrupo.Replace("%", "").ToString();
                            entity = entity.Where(e => e.tbEmpresaGrupo.dsEmpresaGrupo.Contains(busca)).AsQueryable<tbEmpresa>();
                        }
                        else
                            entity = entity.Where(e => e.tbEmpresaGrupo.dsEmpresaGrupo.Equals(dsEmpresaGrupo)).AsQueryable<tbEmpresa>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJBase).AsQueryable<tbEmpresa>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJBase).AsQueryable<tbEmpresa>();
                    break;
                case CAMPOS.DSCERTIFICADODIGITAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsCertificadoDigital).AsQueryable<tbEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dsCertificadoDigital).AsQueryable<tbEmpresa>();
                    break;
                case CAMPOS.DSCERTIFICADODIGITALSENHA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsCertificadoDigitalSenha).AsQueryable<tbEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dsCertificadoDigitalSenha).AsQueryable<tbEmpresa>();
                    break;
                case CAMPOS.CDEMPRESAGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEmpresaGrupo).AsQueryable<tbEmpresa>();
                    else entity = entity.OrderByDescending(e => e.cdEmpresaGrupo).AsQueryable<tbEmpresa>();
                    break;
                case CAMPOS.DTCADASTRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtCadastro).AsQueryable<tbEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dtCadastro).AsQueryable<tbEmpresa>();
                    break;
                case CAMPOS.DTVALIDADE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtValidade).AsQueryable<tbEmpresa>();
                    else entity = entity.OrderByDescending(e => e.dtValidade).AsQueryable<tbEmpresa>();
                    break;
                case CAMPOS.FLSENHAVALIDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.flSenhaValida).AsQueryable<tbEmpresa>();
                    else entity = entity.OrderByDescending(e => e.flSenhaValida).AsQueryable<tbEmpresa>();
                    break;

                // RELACIONAMENTOS
                case CAMPOS.DSEMPRESAGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tbEmpresaGrupo.dsEmpresaGrupo).ThenBy(e => e.nrCNPJBase).AsQueryable<tbEmpresa>();
                    else entity = entity.OrderByDescending(e => e.tbEmpresaGrupo.dsEmpresaGrupo).ThenByDescending(e => e.nrCNPJBase).AsQueryable<tbEmpresa>();
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
                // FILTRO
                string outValue = null;
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.CDEMPRESAGRUPO, out outValue))
                        queryString["" + (int)CAMPOS.CDEMPRESAGRUPO] = IdGrupo.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.CDEMPRESAGRUPO, IdGrupo.ToString());
                }
                string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
                if (!CnpjEmpresa.Equals(""))
                {
                    string CnpjBaseEmpresa = CnpjEmpresa.Substring(0, 8);
                    if (queryString.TryGetValue("" + (int)CAMPOS.NRCNPJBASE, out outValue))
                        queryString["" + (int)CAMPOS.NRCNPJBASE] = CnpjBaseEmpresa;
                    else
                        queryString.Add("" + (int)CAMPOS.NRCNPJBASE, CnpjBaseEmpresa);
                }

                //DECLARAÇÕES
                List<dynamic> CollectionTbEmpresa = new List<dynamic>();
                Retorno retorno = new Retorno();
                retorno.Totais = new Dictionary<string, object>();

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

                // Vendedor ATOS sem estar associado com um grupo empresa?
                if (IdGrupo == 0 && Permissoes.isAtosRoleVendedor(token))
                {
                    // Perfil Comercial tem uma carteira de clientes específica
                    List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(token);
                    query = query.Where(e => listaIdsGruposEmpresas.Contains(e.cdEmpresaGrupo)).AsQueryable<tbEmpresa>();
                }

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
                if (colecao == 1) // Consulta de todas as empresas aptas a importação XML
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
                else if (colecao == 2) // Verificar WEBAPI
                {
                    CollectionTbEmpresa = query.Select(e => new
                    {
                        nrCNPJBase = e.nrCNPJBase,
                        dsCertificadoDigital = e.dsCertificadoDigital,
                        dsCertificadoDigitalSenha = e.dsCertificadoDigitalSenha,
                        cdEmpresaGrupo = e.cdEmpresaGrupo,
                    }).Take(1).ToList<dynamic>();
                }
                else if (colecao == 3) // [PORTAL] Cadastro Certificado Digital
                {
                    CollectionTbEmpresa = query.Select(e => new
                    {
                        nrCNPJBase = e.nrCNPJBase,
                        certificadoDigitalPresente = e.dsCertificadoDigital != null,
                        senhaPresente = e.dsCertificadoDigitalSenha != null && !e.dsCertificadoDigitalSenha.Equals(""),
                        tbempresagrupo = new { cdEmpresaGrupo = e.cdEmpresaGrupo,
                                               dsEmpresaGrupo = e.tbEmpresaGrupo.dsEmpresaGrupo },
                        dtCadastro = e.dtCadastro,
                        dtValidade = e.dtValidade,
                        flSenhaValida = e.flSenhaValida,
                    }).ToList<dynamic>();
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



        /*/// <summary>
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
        }*/



        /*/// <summary>
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

                // VERIFICAR SE EXISTE ALTERAÇÃO NOS PARAMETROS
                if (data != null && (value.dsCertificadoDigital == null || !data.SequenceEqual(value.dsCertificadoDigital)))
                    value.dsCertificadoDigital = data;
                if (param.dsCertificadoDigitalSenha != null && (value.dsCertificadoDigitalSenha == null || !param.dsCertificadoDigitalSenha.Equals(value.dsCertificadoDigitalSenha)))
                    value.dsCertificadoDigitalSenha = param.dsCertificadoDigitalSenha;

                Mensagem mensagem = CertificadoDigital.ValidarCertificado(value.dsCertificadoDigital, value.dsCertificadoDigitalSenha);
                if (mensagem.cdMensagem == 200)
                {
                    value.dtCadastro = DateTime.Now;
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
        }*/



        /// <summary>
        /// Altera certificado e senha
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Mensagem Patch(string token, Dictionary<string, string> queryString)
        {
            try
            {
                // TEM QUE TER ENVIADO VIA QUERYSTRING nrCNPJBase e dsCertificadoDigitalSenha
                string outValue = null;
                if (!queryString.TryGetValue("" + (int)GatewayTbEmpresa.CAMPOS.NRCNPJBASE, out outValue) ||
                    !queryString.TryGetValue("" + (int)GatewayTbEmpresa.CAMPOS.DSCERTIFICADODIGITALSENHA, out outValue))
                    throw new Exception("CNPJ base e Senha são obrigatórios!");

                string nrCNPJBase = queryString["" + (int)GatewayTbEmpresa.CAMPOS.NRCNPJBASE];
                string dsCertificadoDigitalSenha = queryString["" + (int)GatewayTbEmpresa.CAMPOS.DSCERTIFICADODIGITALSENHA];

                // Obtém o objet
                tbEmpresa value = _db.tbEmpresas.Where(e => e.nrCNPJBase.Equals(nrCNPJBase)).FirstOrDefault();

                if (value == null) throw new Exception("CNPJ Base inexistente!");

                // TEM QUE TER ENVIADO O ARQUIVO
                HttpRequest httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count == 0) throw new Exception("Não foi identificado o certificado digital");
                
                // Obtém o arquivo
                HttpPostedFile postedFile = httpRequest.Files[0];
                // Valida a extensão
                string extensao = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("."));
                if (!extensao.ToLower().Equals(".pfx")) throw new Exception("Formato do arquivo deve ser PFX!");

                // Converte para um array de bytes
                BinaryReader binaryReader = new BinaryReader(postedFile.InputStream);
                byte[] data = binaryReader.ReadBytes(postedFile.ContentLength);

                // VERIFICAR SE EXISTE ALTERAÇÃO NOS PARAMETROS
                if (value.dsCertificadoDigital == null || !data.SequenceEqual(value.dsCertificadoDigital))
                    value.dsCertificadoDigital = data;
                if (value.dsCertificadoDigitalSenha == null || !dsCertificadoDigitalSenha.Equals(value.dsCertificadoDigitalSenha))
                    value.dsCertificadoDigitalSenha = dsCertificadoDigitalSenha;

                //Decodifica a senha
                string senha = CertificadoDigital.DecodeFrom64(dsCertificadoDigitalSenha);

                Mensagem mensagem = CertificadoDigital.ValidarCertificado(data, senha);//value.dsCertificadoDigital, value.dsCertificadoDigitalSenha);
                if (mensagem.cdMensagem == 200)
                {
                    value.dtCadastro = DateTime.Now;
                    value.dtValidade = CertificadoDigital.GetDataValidade(data, senha);//value.dsCertificadoDigital, value.dsCertificadoDigitalSenha);
                    value.flSenhaValida = true;

                    _db.SaveChanges();
                }

                return mensagem;
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
