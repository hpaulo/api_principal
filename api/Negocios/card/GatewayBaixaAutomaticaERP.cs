using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using api.Negocios.Pos;
using Microsoft.Ajax.Utilities;
using api.Negocios.Util;
using System.Globalization;
using System.Net.Http;

namespace api.Negocios.Card
{
    public class GatewayBaixaAutomaticaERP
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayBaixaAutomaticaERP()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDEXTRATO = 100,
            ID_GRUPO = 101,
        };

        private const string DOMINIO = ".atoscapital.com.br/";

        /// <summary>
        /// Requisita à API correspondente a baixa automática
        /// </summary>
        /// <returns></returns>        
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                Retorno retorno = new Retorno();
                string outValue = null;

                // ID EXTRATO
                Int32 idExtrato = -1;
                if(!queryString.TryGetValue("" + (int)CAMPOS.IDEXTRATO, out outValue))
                    throw new Exception("O identificador da movimentação bancária deve ser informada para a baixa automática!");

                idExtrato = Convert.ToInt32(queryString["" + (int)CAMPOS.IDEXTRATO]);
                tbExtrato tbExtrato = _db.tbExtratos.Where(e => e.idExtrato == idExtrato).FirstOrDefault();
                if(tbExtrato == null)
                    throw new Exception("Extrato inexistente!");

                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo == 0) throw new Exception("Um grupo deve ser selecionado como para a baixa automática!");

                if(tbExtrato.tbContaCorrente.cdGrupo != IdGrupo)
                    throw new Exception("Permissão negada! Movimentação bancária informada não se refere ao grupo associado ao usuário.");

                grupo_empresa grupo_empresa = _db.grupo_empresa.Where(e => e.id_grupo == IdGrupo).FirstOrDefault();

                if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                    throw new Exception("Permissão negada! Empresa não possui o serviço ativo");

                string url = "http://" + grupo_empresa.dsAPI + DOMINIO;
                string complemento = "titulos/baixaautomatica/" + token + "?" + ("" + (int)CAMPOS.IDEXTRATO) + "=" + idExtrato;


                HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                System.Net.Http.HttpResponseMessage response = client.GetAsync(complemento).Result;

                //se retornar com sucesso busca os dados
                if (response.IsSuccessStatusCode)
                    //Pegando os dados do Rest e armazenando na variável retorno
                    retorno = response.Content.ReadAsAsync<Retorno>().Result;
                else
                    throw new Exception(((int) response.StatusCode) + " - " + response.Content.ReadAsAsync<string>().Result);
 

                return retorno;

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao realizar a baixa automática" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}