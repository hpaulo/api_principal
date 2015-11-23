using api.Models;
using api.Models.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace api.Bibliotecas
{
    public class Permissoes
    {

        // ======================== VALIDAÇÃO DE ACESSO AOS MÉTODOS DA API ======================================//

        private static AcessoMetodoAPI acessoMetodosAPIs = new AcessoMetodoAPI();

        /// <summary>
        /// Retorna true se o usuário tem permissão para acessar o método da URL da API
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        /// <param name="metodo"></param>
        /// <returns></returns>
        public static bool usuarioTemPermissaoMetodoURL(string token, string url, string metodo)
        {
            if (acessoMetodosAPIs.Count() == 0) PopulateAcessoMetodosAPIs();

            metodo = metodo.ToUpper();

            string method = metodo.Equals("GET") ? "Leitura" :
                            metodo.Equals("POST") || metodo.Equals("PATCH") ? "Cadastro" :
                            metodo.Equals("PUT") ? "Atualização" :
                            metodo.Equals("DELETE") ? "Remoção" : "";

            if (method.Equals("")) return false; // método HTTP inválido

            Int32 idController = GetIdUltimoControllerAcessado(token);
            if (idController == 0 || !usuarioTemPermissaoMetodoController(token, idController, method)) return false;

            // Controller acessado pode fazer a requisição?
            return acessoMetodosAPIs.IsMetodoControllerPermitidoInURL(url, idController, metodo);
        }

        /// <summary>
        /// Obtém o ID do controller a partir do dsController
        /// </summary>
        /// <param name="dscontrollers"></param> Lista dos dscontrollers, do filho para o pai
        /// <returns></returns>
        private static Int32 GetIdController(List<string> dscontrollers)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                if (dscontrollers.Count == 0) return 0;

                //_db.Configuration.ProxyCreationEnabled = false;
                var query = _db.webpages_Controllers.AsQueryable<webpages_Controllers>();

                // Verifica se o nome é único
                string ds_controller = dscontrollers[0].ToUpper();
                List<webpages_Controllers> list = query.Where(e => e.ds_controller.ToUpper().Equals(ds_controller)).ToList<webpages_Controllers>();
                if (dscontrollers.Count == 1 || list.Count == 1) return list[0].id_controller;

                // Verifica o nome dele com o nome do pai dele
                string ds_controller1 = dscontrollers[1].ToUpper();
                list = query.Where(e => e.ds_controller.ToUpper().Equals(ds_controller))
                            .Where(e => e.webpages_Controllers2.ds_controller.ToUpper().Equals(ds_controller1))
                            .ToList<webpages_Controllers>();

                if (dscontrollers.Count == 2 || list.Count == 1) return list[0].id_controller;

                // Verifica o nome dele com os nomes do pai e avô dele
                string ds_controller2 = dscontrollers[2].ToUpper();
                list = query.Where(e => e.ds_controller.ToUpper().Equals(ds_controller))
                            .Where(e => e.webpages_Controllers2.ds_controller.ToUpper().Equals(ds_controller1))
                            .Where(e => e.webpages_Controllers2.webpages_Controllers2.ds_controller.ToUpper().Equals(ds_controller2))
                            .ToList<webpages_Controllers>();

                if (list.Count > 1) return list[0].id_controller;
                return 0;

            }
        }

        /// <summary>
        /// Inicializa o objeto acessoMetodosAPIs, que armazena para cada API as possíveis origens (telas) da requisição e seus respectivos métodos
        /// </summary>
        private static void PopulateAcessoMetodosAPIs()
        {
            #region POPULA
            List<ControllersOrigem> controllersOrigem = new List<ControllersOrigem>();
            acessoMetodosAPIs.Clear();

            // -------------------------------- CONTROLLERS PORTAL -------------------------------- //
            Int32 idControllerPortalAdministrativoAcessoUsuarios = GetIdController(new List<string>() { "ACESSO DE USUÁRIOS", "LOGS" });
            Int32 idControllerPortalAdministrativoAcoesUsuarios = GetIdController(new List<string>() { "ACÕES DE USUÁRIOS", "LOGS" });
            Int32 idControllerPortalAdministrativoContasCorrentes = GetIdController(new List<string>() { "CONTAS CORRENTES", "DADOS BANCÁRIOS" });
            Int32 idControllerPortalAdministrativoDadosAcesso = GetIdController(new List<string>() { "DADOS DE ACESSO", "GESTÃO DE EMPRESAS" });
            Int32 idControllerPortalAdministrativoEmpresas = GetIdController(new List<string>() { "EMPRESAS", "GESTÃO DE EMPRESAS" });
            Int32 idControllerPortalAdministrativoExtratosBancarios = GetIdController(new List<string>() { "EXTRATOS BANCÁRIOS", "DADOS BANCÁRIOS" });
            Int32 idControllerPortalAdministrativoFiliais = GetIdController(new List<string>() { "FILIAIS", "GESTÃO DE EMPRESAS" });
            Int32 idControllerPortalAdministrativoModulosFuncionalidades = GetIdController(new List<string>() { "MÓDULOS E FUNCIONALIDADES", "GESTÃO DE ACESSOS" });
            Int32 idControllerPortalAdministrativoMonitorCargas = GetIdController(new List<string>() { "MONITOR DE CARGAS", "MONITOR" });
            Int32 idControllerPortalAdministrativoParametrosBancarios = GetIdController(new List<string>() { "PARÂMETROS BANCÁRIOS", "DADOS BANCÁRIOS" });
            Int32 idControllerPortalAdministrativoPrivilegios = GetIdController(new List<string>() { "PRIVILÉGIOS", "GESTÃO DE ACESSOS" });
            Int32 idControllerPortalAdministrativoSenhasInvalidas = GetIdController(new List<string>() { "SENHAS INVÁLIDAS", "GESTÃO DE EMPRESAS" });
            Int32 idControllerPortalAdministrativoUsuarios = GetIdController(new List<string>() { "USUÁRIOS", "GESTÃO DE ACESSOS" });
            Int32 idControllerPortalCardServicesCashFlowRelatorios = GetIdController(new List<string>() { "RELATÓRIOS", "CASH FLOW" });
            Int32 idControllerPortalCardServicesConciliacaoBancaria = GetIdController(new List<string>() { "CONCILIAÇÃO BANCÁRIA", "CONCILIAÇÃO" });
            Int32 idControllerPortalCardServicesConsolidacaoRelatorios = GetIdController(new List<string>() { "RELATÓRIOS", "CONSOLIDAÇÃO" });
            Int32 idControllerPortalMinhaConta = 91;
            Int32 idControllerPortalTaxServicesCadastroCertificadoDigital = GetIdController(new List<string>() { "CADASTRO CERTIFICADO DIGITAL", "NOTA FISCAL ELETRÔNICA" });
            Int32 idControllerPortalTaxServicesImportacaoXML = GetIdController(new List<string>() { "IMPORTAÇÃO XML", "NOTA FISCAL ELETRÔNICA" });
            // ...
            // ----------------------------- FIM - CONTROLLERS PORTAL ----------------------------- //

            // -------------------------------- CONTROLLERS MOBILE -------------------------------- //
            // ...
            // ----------------------------- FIM - CONTROLLERS MOBILE ----------------------------- //


            // ============================= ADMINISTRAÇÃO ======================================= //
            /*                                  LOGACESSO                                           */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > LOGS > ACESSO DE USUÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoAcessoUsuarios, new string[] { "GET" }));
            // MOBILE......
            // Adiciona
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_LOGACESSO, controllersOrigem);
            /*                                   PESSOA                                            */
            controllersOrigem.Clear();
            // Portal não acessa essa API
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_PESSOA, controllersOrigem);
            /*                                  TBCANAL                                            */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBCANAL, controllersOrigem);
            /*                                 TBCATALOGO                                          */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBCATALOGO, controllersOrigem);
            /*                             TBDISPOSITIVOUSUARIO                                    */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBDISPOSITIVOUSUARIO, controllersOrigem);
            /*                                  TBEMPRESA                                          */
            controllersOrigem.Clear();
            // [PORTAL] TAX SERVICES > NOTA FISCAL ELETRÔNICA > CADASTRO CERTIFICADO DIGITAL
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalTaxServicesCadastroCertificadoDigital, new string[] { "GET", "PATCH" }));
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBEMPRESA, controllersOrigem);
            /*                                TBEMPRESAFILIAL                                      */
            controllersOrigem.Clear();
            // PORTAL...
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBEMPRESAFILIAL, controllersOrigem);
            /*                                TBEMPRESAGRUPO                                       */
            controllersOrigem.Clear();
            // PORTAL...
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBEMPRESAGRUPO, controllersOrigem);
            /*                               TBLOGACESSOUSUARIO                                    */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > LOGS > AÇÕES DE USUÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoAcoesUsuarios, new string[] { "GET" }));
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBLOGACESSOUSUARIO, controllersOrigem);
            /*                                   TBLOGERRO                                         */
            controllersOrigem.Clear();
            // PORTAL...
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBLOGERRO, controllersOrigem);
            /*                                 TBLOGMANIFESTO                                      */
            controllersOrigem.Clear();
            // PORTAL...
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBLOGMANIFESTO, controllersOrigem);
            /*                                     TBNEWS                                          */
            controllersOrigem.Clear();
            // PORTAL...
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBNEWS, controllersOrigem);
            /*                                   TBNEWSGRUPO                                       */
            controllersOrigem.Clear();
            // PORTAL...
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBNEWSGRUPO, controllersOrigem);
            /*                                   TBNEWSSTATUS                                      */
            controllersOrigem.Clear();
            // PORTAL...
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_TBNEWSSTATUS, controllersOrigem);
            /*                            WEBPAGESCONTROLLERS                                      */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > MÓDULOS E FUNCIONALIDADES
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoModulosFuncionalidades, new string[] { "GET", "DELETE", "POST", "PUT" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > PRIVILÉGIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoPrivilegios, new string[] { "GET" })); 
            // MOBILE......
            // Adiciona
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_WEBPAGESCONTROLLERS, controllersOrigem);
            /*                             WEBPAGESMEMBERSHIP                                      */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > USUÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoUsuarios, new string[] { "PUT" }));
            // [PORTAL] MINHA CONTA
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalMinhaConta, new string[] { "PUT" }));
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_WEBPAGESMEMBERSHIP, controllersOrigem);
            /*                               WEBPAGESMETHODS                                       */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > MÓDULOS E FUNCIONALIDADES
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoModulosFuncionalidades, new string[] { "DELETE", "POST", "PUT" }));
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_WEBPAGESMETHODS, controllersOrigem);
            /*                             WEBPAGESPERMISSIONS                                     */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > PRIVILÉGIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoPrivilegios, new string[] { "PUT" }));
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_WEBPAGESPERMISSIONS, controllersOrigem);
            /*                             WEBPAGESROLELEVELS                                      */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > PRIVILÉGIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoPrivilegios, new string[] { "GET" }));
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_WEBPAGESROLELEVELS, controllersOrigem);
            /*                                WEBPAGESROLES                                        */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > PRIVILÉGIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoPrivilegios, new string[] { "GET", "DELETE", "POST", "PUT" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > USUÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoUsuarios, new string[] { "GET" }));
            // MOBILE....
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_WEBPAGESROLES, controllersOrigem);
            /*                               WEBPAGESUSERS                                         */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > USUÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoUsuarios, new string[] { "GET", "DELETE", "POST", "PUT" }));
            // [PORTAL] MINHA CONTA
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalMinhaConta, new string[] { "GET", "PUT" }));
            // MOBILE......
            // Adiciona (OBS: ÚNICA RESTRIÇÃO É O "PUT" PARA ALTERAR O GRUPO EMPRESA => PODE VIR DE QUALQUER TELA)
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_WEBPAGESUSERS, controllersOrigem);
            /*                            WEBPAGESUSERSINROLES                                     */
            controllersOrigem.Clear();
            // Portal não acessa essa API
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.ADMINISTRACAO_WEBPAGESUSERSINROLES, controllersOrigem);

            // ================================= CARD ============================================ //
            /*                            CONCILIACAOBANCARIA                                      */
            controllersOrigem.Clear();
            // [PORTAL] CARD SERVICES > CONCILIAÇÃO > CONCILIAÇÃO BANCÁRIA
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesConciliacaoBancaria, new string[] { "GET", "PUT" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_CONCILIACAOBANCARIA, controllersOrigem);
            /*                               TBADQUIRENTE                                          */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > PARÂMETROS BANCÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoParametrosBancarios, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBADQUIRENTE, controllersOrigem);
            /*                             TBBANCOPARAMETRO                                        */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > PARÂMETROS BANCÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoParametrosBancarios, new string[] { "GET", "DELETE", "PUT" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBBANCOPARAMETRO, controllersOrigem);
            /*                              TBBANDEIRATEF                                          */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBBANDEIRATEF, controllersOrigem);
            /*                             TBCONTACORRENTE                                         */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > CONTAS CORRENTES
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoContasCorrentes, new string[] { "GET", "DELETE", "POST", "PUT" }));
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > EXTRATOS BANCÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoExtratosBancarios, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBCONTACORRENTE, controllersOrigem);
            /*                   TBCONTACORRENTETBLOGINADQUIRENTEEMPRESA                           */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > CONTAS CORRENTES
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoContasCorrentes, new string[] { "GET", "POST", "PUT" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBCONTACORRENTETBLOGINADQUIRENTEEMPRESA, controllersOrigem);
            /*                              TBESTADOTRANSACAOTEF                                   */
            controllersOrigem.Clear();
            // PORTAL.....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBESTADOTRANSACAOTEF, controllersOrigem);
            /*                                   TBEXTRATO                                         */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > EXTRATOS BANCÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoExtratosBancarios, new string[] { "GET", "PATCH" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBEXTRATO, controllersOrigem);
            /*                            TBLOGINADQUIRENTEEMPRESA                                 */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > CONTAS CORRENTES
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoContasCorrentes, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBLOGINADQUIRENTEEMPRESA, controllersOrigem);
            /*                                TBMODOENTRADATEF                                     */
            controllersOrigem.Clear();
            // PORTAL.....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBMODOENTRADATEF, controllersOrigem);
            /*                                  TBPRODUTOTEF                                       */
            controllersOrigem.Clear();
            // PORTAL.....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBPRODUTOTEF, controllersOrigem);
            /*                              TBRECEBIMENTORESUMO                                    */
            controllersOrigem.Clear();
            // PORTAL.....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBRECEBIMENTORESUMO, controllersOrigem);
            /*                               TBRECEBIMENTOTEF                                      */
            controllersOrigem.Clear();
            // PORTAL.....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBRECEBIMENTOTEF, controllersOrigem);
            /*                              TBSITUACAOREDETEF                                      */
            controllersOrigem.Clear();
            // PORTAL.....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBSITUACAOREDETEF, controllersOrigem);
            /*                              TBTIPOPRODUTOTEF                                       */
            controllersOrigem.Clear();
            // PORTAL.....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBTIPOPRODUTOTEF, controllersOrigem);
            /*                               TBTRANSACAOTEF                                        */
            controllersOrigem.Clear();
            // PORTAL.....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CARD_TBTRANSACAOTEF, controllersOrigem);

            // ================================ CLIENTE ========================================== //
            /*                                  EMPRESA                                            */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > CONTAS CORRENTES
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoContasCorrentes, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > PARÂMETROS BANCÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoParametrosBancarios, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > USUÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoUsuarios, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE EMPRESAS > DADOS DE ACESSO
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoDadosAcesso, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE EMPRESAS > FILIAIS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoFiliais, new string[] { "GET", "DELETE", "POST", "PUT" }));
            // [PORTAL] ADMINISTRATIVO > MONITOR > MONITOR DE CARGAS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoMonitorCargas, new string[] { "GET" }));
            // [PORTAL] CARD SERVICES > CASH FLOW > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesCashFlowRelatorios, new string[] { "GET" }));
            // [PORTAL] CARD SERVICES > CONCILIAÇÃO > CONCILIAÇÃO BANCÁRIA
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesConciliacaoBancaria, new string[] { "GET" }));
            // [PORTAL] CARD SERVICES > CONSOLIDAÇÃO > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesConsolidacaoRelatorios, new string[] { "GET" }));
            // [PORTAL] TAX SERVICES > NOTA FISCAL ELETRÔNICA > IMPORTAÇÃO XML
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalTaxServicesImportacaoXML, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CLIENTE_EMPRESA, controllersOrigem);
            /*                               GRUPOEMPRESA                                          */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > USUÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoUsuarios, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE EMPRESAS > FILIAIS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoFiliais, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE EMPRESAS > EMPRESAS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoEmpresas, new string[] { "GET", "DELETE", "POST", "PUT" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.CLIENTE_GRUPOEMPRESA, controllersOrigem);

            // ================================== POS ============================================ //
            /*                                ADQUIRENTE                                           */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > PARÂMETROS BANCÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoDadosAcesso, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_ADQUIRENTE, controllersOrigem);
            /*                                   AMEX                                              */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_AMEX, controllersOrigem);
            /*                                 BANDEIRA                                            */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_BANDEIRA, controllersOrigem);
            /*                                BANDEIRAPOS                                          */
            controllersOrigem.Clear();
            // [PORTAL] CARD SERVICES > CASH FLOW > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesCashFlowRelatorios, new string[] { "GET" }));
            // [PORTAL] CARD SERVICES > CONSOLIDAÇÃO > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesConsolidacaoRelatorios, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_BANDEIRAPOS, controllersOrigem);
            /*                                BANESECARD                                           */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_BANESECARD, controllersOrigem);
            /*                                  CIELO                                              */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_CIELO, controllersOrigem);
            /*                           CONCILIACAOPAGAMENTOPOS                                   */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_CONCILIACAOPAGAMENTOPOS, controllersOrigem);
            /*                           CONCILIACAORECEBIMENTO                                    */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_CONCILIACAORECEBIMENTO, controllersOrigem);
            /*                                  FITCARD                                            */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_FITCARD, controllersOrigem);
            /*                              GETNETSANTANDER                                        */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_GETNETSANTANDER, controllersOrigem);
            /*                                 GOODCARD                                            */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_GOODCARD, controllersOrigem);
            /*                                 GREENCARD                                           */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_GREENCARD, controllersOrigem);
            /*                                LOGEXECUTION                                         */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_LOGEXECUTION, controllersOrigem);
            /*                            LOGEXECUTIONEXCEPTION                                    */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_LOGEXECUTIONEXCEPTION, controllersOrigem);
            /*                               LOGINOPERADORA                                        */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > PARÂMETROS BANCÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoParametrosBancarios, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE EMPRESAS > DADOS DE ACESSO
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoDadosAcesso, new string[] { "GET", "DELETE", "POST", "PUT" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE EMPRESAS > SENHAS INVÁLIDAS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoSenhasInvalidas, new string[] { "GET", "DELETE", "PUT" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_LOGINOPERADORA, controllersOrigem);
            /*                                 NUTRICASH                                           */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_NUTRICASH, controllersOrigem);
            /*                                   OMNI                                              */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_OMNI, controllersOrigem);
            /*                                OPERADORA                                            */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE EMPRESAS > DADOS DE ACESSO
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoDadosAcesso, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > MONITOR > MONITOR DE CARGAS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoMonitorCargas, new string[] { "GET" }));
            // [PORTAL] CARD SERVICES > CASH FLOW > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesCashFlowRelatorios, new string[] { "GET" }));
            // [PORTAL] CARD SERVICES > CONSOLIDAÇÃO > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesConsolidacaoRelatorios, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_OPERADORA, controllersOrigem);
            /*                                POLICARD                                             */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_POLICARD, controllersOrigem);
            /*                               RECEBIMENTO                                           */
            controllersOrigem.Clear();
            // [PORTAL] CARD SERVICES > CONSOLIDAÇÃO > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesConsolidacaoRelatorios, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_RECEBIMENTO, controllersOrigem);
            /*                            RECEBIMENTOPARCELA                                       */
            controllersOrigem.Clear();
            // [PORTAL] CARD SERVICES > CASH FLOW > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesCashFlowRelatorios, new string[] { "GET" }));
            // [PORTAL] CARD SERVICES > CONCILIAÇÃO > CONCILIAÇÃO BANCÁRIA
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesConciliacaoBancaria, new string[] { "PUT" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_RECEBIMENTOPARCELA, controllersOrigem);
            /*                                REDECARD                                             */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_REDECARD, controllersOrigem);
            /*                                REDEMED                                              */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_REDEMED, controllersOrigem);
            /*                                SODEXO                                               */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_SODEXO, controllersOrigem);
            /*                            TAXAADMINISTRACAO                                        */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_TAXAADMINISTRACAO, controllersOrigem);
            /*                             TERMINALLOGICO                                          */
            controllersOrigem.Clear();
            // [PORTAL] CARD SERVICES > CONSOLIDAÇÃO > RELATÓRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalCardServicesConsolidacaoRelatorios, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_TERMINALLOGICO, controllersOrigem);
            /*                                TICKETCAR                                            */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_TICKETCAR, controllersOrigem);
            /*                                VALECARD                                             */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.POS_VALECARD, controllersOrigem);

            // ================================== TAX ============================================ //
            /*                               TBCONTROLENSU                                         */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.TAX_TBCONTROLENSU, controllersOrigem);
            /*                                TBMANIFESTO                                          */
            controllersOrigem.Clear();
            // [PORTAL] TAX SERVICES > NOTA FISCAL ELETRONICA > IMPORTAÇÃO XML
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalTaxServicesImportacaoXML, new string[] { "GET" }));
            // MOBILE......
            // OBS: HÁ UMA TELA QUE NÃO TEM IDCONTROLLER MAS FAZ GET PARA ESSA PÁGINA
            acessoMetodosAPIs.Add(UrlAPIs.TAX_TBMANIFESTO, controllersOrigem);

            // ================================== UTIL =========================================== //
            /*                                   BANCOS                                            */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > CONTAS CORRENTES
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoContasCorrentes, new string[] { "GET" }));
            // [PORTAL] ADMINISTRATIVO > DADOS BANCÁRIOS > PARÂMETROS BANCÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalAdministrativoParametrosBancarios, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.UTIL_BANCOS, controllersOrigem);
            /*                                  EXPORTAR                                           */
            controllersOrigem.Clear();
            // PORTAL....
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.UTIL_EXPORTAR, controllersOrigem);
            /*                                  UTILNFE                                            */
            controllersOrigem.Clear();
            // [PORTAL] TAX SERVICES > NOTA FISCAL ELETRONICA > IMPORTAÇÃO XML
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalTaxServicesImportacaoXML, new string[] { "GET" }));
            // MOBILE......
            acessoMetodosAPIs.Add(UrlAPIs.UTIL_UTILNFE, controllersOrigem);
            #endregion
        }


        // ======================== FIM - VALIDAÇÃO DE ACESSO AOS MÉTODOS DA API ======================== //



        /// <summary>
        /// Retorna true se o token informado é válido
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool Autenticado(string token)
        {
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {

                _db.Configuration.ProxyCreationEnabled = false;

                var verify = _db.LoginAutenticacaos.Where(v => v.token.Equals(token)).Select(v => v).FirstOrDefault();

                // Fecha a conexão
                _db.Database.Connection.Close();
                _db.Dispose();

                if (verify != null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// A partir do token, obtém o objeto webpages_Users correspondente
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Null se o token for inválido</returns>
        public static webpages_Users GetUser(string token)
        {
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                webpages_Users user = _db.LoginAutenticacaos.Where(v => v.token.Equals(token))
                            .Select(v => v.webpages_Users)
                            .FirstOrDefault<webpages_Users>();

                // Fecha a conexão
                _db.Database.Connection.Close();
                _db.Dispose();
                return user;
            }
            //return _db.LoginAutenticacaos.Where(v => v.token.Equals(token)).Select(v => v.webpages_Users).FirstOrDefault();
        }

        /// <summary>
        /// A partir do token, obtém o id do usuário correspondente
        /// </summary>
        /// <param name="token"></param>
        /// <returns>0 se o token for inválido</returns>
        public static Int32 GetIdUser(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null) return (Int32) user.id_users;
            return 0;
        }

        /// <summary>
        /// A partir do ds_login, obtém o id do usuário correspondente
        /// </summary>
        /// <param name="ds_login"></param>
        /// <returns>0 se o ds_login for inválido</returns>
        public static Int32 GetIdUserPeloLogin(string ds_login)
        {
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                webpages_Users user = _db.webpages_Users.Where(o => o.ds_login.Equals(ds_login)).Select(o => o).FirstOrDefault();

                // Fecha a conexão
                _db.Database.Connection.Close();
                _db.Dispose();

                if (user != null) return (Int32)user.id_users;
                return 0;
            }
        }

        /// <summary>
        /// A partir do token, obtém o id do grupo que o usuário correspondente está associado
        /// </summary>
        /// <param name="token"></param>
        /// <returns>0 se o token for inválido ou se o usuário não estiver associado a algum grupo</returns>
        public static Int32 GetIdGrupo(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null && user.id_grupo != null) return (Int32) user.id_grupo;
            return 0;
        }

        /// <summary>
        /// A partir do token, obtém o cnpj que o usuário correspondente está associado
        /// </summary>
        /// <param name="token"></param>
        /// <returns>"" (string vazia) se o token for inválido ou se o usuário não estiver associado a alguma filial</returns>
        public static string GetCNPJEmpresa(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null && user.id_grupo != null && user.nu_cnpjEmpresa != null) return user.nu_cnpjEmpresa;
            return "";
        }

        /// <summary>
        /// A partir do token, obtém o objeto webpages_Roles que o usuário correspondente está associado
        /// </summary>
        /// <param name="token"></param>
        /// <returns>null se o token for inválido ou se o usuário não estiver associado a nenhuma role do novo portal (id > 50)</returns>
        public static webpages_Roles GetRole(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null)
            {
                using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    webpages_Roles role = _db.webpages_UsersInRoles
                                            .Where(r => r.UserId == user.id_users)
                                            .Where(r => r.RoleId > 50)
                                            .Select(r => r.webpages_Roles)
                                            .FirstOrDefault();

                    // Fecha a conexão
                    _db.Database.Connection.Close();
                    _db.Dispose();

                    return role;
                }
            }
            return null;
        }

        /// <summary>
        /// A partir do token, obtém o id da role que o usuário correspondente está associado 
        /// </summary>
        /// <param name="token"></param>
        /// <returns>0 se o token for inválido ou se o usuário não estiver associado a nenhuma role do novo portal (id > 50)</returns>
        public static Int32 GetRoleId(string token)
        {
            webpages_Roles role = GetRole(token);
            if (role != null) return role.RoleId;
            return 0;
        }

        /// <summary>
        /// A partir do token, obtém o nome da role que o usuário correspondente está associado 
        /// </summary>
        /// <param name="token"></param>
        /// <returns>"" (string vazia) se o token for inválido ou se o usuário não estiver associado a nenhuma role do novo portal (id > 50)</returns>
        public static String GetRoleName(string token)
        {
            webpages_Roles role = GetRole(token);
            if (role != null) return role.RoleName;
            return "";
        }

        /// <summary>
        /// A partir do token, obtém o nível da role que o usuário correspondente está associado 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Int32 GetRoleLevel(string token)
        {
            webpages_Roles role = GetRole(token);
            if (role != null) return role.RoleLevel;
            return 4;
        }

        /// <summary>
        /// A partir do token, obtém o valor mínimo de nível de role a partir do privilégio que o usuário está associado
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Int32 GetRoleLevelMin(string token)
        {
            Int32 RoleLevel = GetRoleLevel(token);
            if (RoleLevel > 1) return RoleLevel + 1;
            return RoleLevel;
        }

        /// <summary>
        /// Retorna true se o a role associada ao usuário é de um perfil da ATOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool isAtosRole(string token)
        {
            Int32 RoleLevel = GetRoleLevel(token);
            return RoleLevel >= 0 && RoleLevel <= 2;
        }

        /// <summary>
        /// Retorna true se o a role é de um perfil da ATOS
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool isAtosRole(webpages_Roles role)
        {
            if (role == null) return false;
            return role.RoleLevel >= 0 && role.RoleLevel <= 2;
        }

        /// <summary>
        /// Retorna true se a role associado ao usuário é de um perfil vendedor da ATOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool isAtosRoleVendedor(string token)
        {
            string RoleName = GetRoleName(token);
            return isAtosRole(token) && RoleName.ToUpper().Equals("COMERCIAL");
        }

        /// <summary>
        /// Retorna true se a role é de um perfil vendedor da ATOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool isAtosRoleVendedor(webpages_Roles role)
        {
            if (role == null) return false;
            return isAtosRole(role) && role.RoleName.ToUpper().Equals("COMERCIAL");
        }

        /// <summary>
        /// Obtém uma lista contendo os ids dos grupos aos quais o usuário é o vendedor responsável
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static List<Int32> GetIdsGruposEmpresasVendedor(string token)
        {
            List<Int32> lista = new List<Int32>();
 
            Int32 UserId = GetIdUser(token);

            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                lista = _db.grupo_empresa
                        .Where(g => g.id_vendedor == UserId)
                        .Select(g => g.id_grupo)
                        .ToList<Int32>();

                // Fecha a conexão
                _db.Database.Connection.Close();
                _db.Dispose();

                return lista;
            }
        }

        /// <summary>
        /// A partir da descrição do método e do id do controller, obtém o id o método
        /// </summary>
        /// <param name="idController"></param>
        /// <param name="ds_method"></param>
        /// <returns>0 se o método não existe para o controller</returns>
        public static Int32 GetIdMethod(Int32 idController, string ds_method)
        {
            Int32 idMethod = 0;

            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                var method = _db.webpages_Methods.Where(m => m.id_controller == idController)
                                                 .Where(m => m.ds_method.ToUpper().Equals(ds_method.ToUpper()))
                                                 .FirstOrDefault();

                // Fecha a conexão
                _db.Database.Connection.Close();
                _db.Dispose();

                if (method != null)
                    idMethod = method.id_method;

                return idMethod;
            }
        }

        /// <summary>
        /// Retorna o id do último controller (tela) acessado pelo usuário
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Int32 GetIdUltimoControllerAcessado(string token)
        {
            Int32 UserId = GetIdUser(token);

            if (UserId == 0) return 0;

            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                Int32 idController = _db.LogAcesso1
                            .Where(e => e.idUsers == UserId)
                            .OrderByDescending(e => e.dtAcesso)
                            .Select(e => e.idController ?? 0)
                            .FirstOrDefault();

                // Fecha a conexão
                _db.Database.Connection.Close();
                _db.Dispose();

                return idController;
            }
        }


        /// <summary>
        /// Retorna true se o usuário com o token informado possui permissão para o controller
        /// </summary>
        /// <param name="token"></param>
        /// <param name="idController"></param>
        /// <returns></returns>
        public static bool usuarioTemPermissaoController(string token, Int32 idController)
        {
            Int32 idControllerPortalMinhaConta = 91;

            if (idController == idControllerPortalMinhaConta) return true; // Minha conta => todos tem acesso

            Int32 idRole = GetRoleId(token);
            if (idRole == 0) return false;

            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                bool temPermissao = _db.webpages_Permissions.Where(p => p.id_roles == idRole)
                                               .Where(p => p.webpages_Methods.id_controller == idController)
                                               .Count() > 0;

                // Fecha a conexão
                _db.Database.Connection.Close();
                _db.Dispose();

                return temPermissao;
            }
        }


        /// <summary>
        /// Retorna true se o usuário com o token informado possui permissão para o método do controller
        /// </summary>
        /// <param name="token"></param>
        /// <param name="idController"></param>
        /// <returns></returns>
        public static bool usuarioTemPermissaoMetodoController(string token, Int32 idController, string metodo)
        {
            Int32 idRole = GetRoleId(token);
            if (idRole == 0) return false;

            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                metodo = metodo.ToLower();

                bool temPermissao = _db.webpages_Permissions.Where(p => p.id_roles == idRole)
                                               .Where(p => p.webpages_Methods.id_controller == idController)
                                               .Where(p => p.webpages_Methods.ds_method.ToLower().Equals(metodo))
                                               .Count() > 0;

                // Fecha a conexão
                _db.Database.Connection.Close();
                _db.Dispose();

                return temPermissao;
            }
        }

        /// <summary>
        /// Retorna true se o usuário pode se associar ao grupo informado
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id_grupo"></param>
        /// <returns></returns>
        public static Boolean usuarioPodeSeAssociarAoGrupo(string token, Int32 id_grupo)
        {
            bool isAtosVendedor = isAtosRoleVendedor(token);

            // Perfil ATOS não vendedor pode se associar a qualquer grupo
            if (isAtosRole(token) && !isAtosVendedor) return true;

            // Perfil ATOS vendedor pode se associar aos grupos de sua "carteira"
            if (isAtosVendedor)
            {
                List<Int32> list = GetIdsGruposEmpresasVendedor(token);
                return list.Contains(id_grupo);
            }

            // Qualquer outro privilégio não pode mudar de grupo
            return false;
        }
    }
}