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
                            metodo.Equals("DELETE") ? "Remoção" : "Unknown";

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
                List<webpages_Controllers> list = query.Where(e => e.ds_controller.ToUpper().Equals(dscontrollers[0].ToUpper())).ToList<webpages_Controllers>();
                if (dscontrollers.Count == 1 || list.Count == 1) return list[0].id_controller;

                // Verifica o nome dele com o nome do pai dele
                list = query.Where(e => e.ds_controller.ToUpper().Equals(dscontrollers[0].ToUpper()))
                            .Where(e => e.webpages_Controllers2.ds_controller.ToUpper().Equals(dscontrollers[1].ToUpper()))
                            .ToList<webpages_Controllers>();

                if (dscontrollers.Count == 2 || list.Count == 1) return list[0].id_controller;

                // Verifica o nome dele com os nomes do pai e avô dele
                list = query.Where(e => e.ds_controller.ToUpper().Equals(dscontrollers[0].ToUpper()))
                            .Where(e => e.webpages_Controllers2.ds_controller.ToUpper().Equals(dscontrollers[1].ToUpper()))
                            .Where(e => e.webpages_Controllers2.webpages_Controllers2.ds_controller.ToUpper().Equals(dscontrollers[2].ToUpper()))
                            .ToList<webpages_Controllers>();

                if (list.Count > 1) return list[0].id_controller;
                return 0;

            }
        }

        private static void PopulateAcessoMetodosAPIs(){

            List<ControllersOrigem> controllersOrigem = new List<ControllersOrigem>();
            acessoMetodosAPIs.Clear();

            // --------------------------------- PORTAL ------------------------------------------ //

            // IDS CONTROLLERS
            Int32 idControllerPortalModulosFuncionalidades = GetIdController(new List<string>() { "MÓDULOS E FUNCIONALIDADES", "GESTÃO DE ACESSOS" });
            Int32 idControllerPortalPrivilegios = GetIdController(new List<string>() { "PRIVILÉGIOS", "GESTÃO DE ACESSOS" });
            Int32 idControllerPortalUsuarios = GetIdController(new List<string>() { "USUÁRIOS", "GESTÃO DE ACESSOS" });
            Int32 idControllerPortalMinhaConta = 91;

            // ============================= ADMINISTRAÇÃO ======================================= //
            /*                            WEBPAGESCONTROLLERS                                      */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > MÓDULOS E FUNCIONALIDADES
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalModulosFuncionalidades, new List<string>() { "GET", "DELETE", "POST", "PUT" }));
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > PRIVILÉGIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalPrivilegios, new List<string>() { "GET" })); 
            // ÚNICA RESTRIÇÃO É O PUT PARA ALTERAR O GRUPO EMPRESA => PODE VIR DE QUALQUER TELA
            acessoMetodosAPIs.Add("administracao/webpagescontrollers", controllersOrigem);
            /*                               WEBPAGESUSERS                                         */
            controllersOrigem.Clear();
            // [PORTAL] ADMINISTRATIVO > GESTÃO DE ACESSOS > USUÁRIOS
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalUsuarios, new List<string>() { "GET", "DELETE", "POST", "PUT" }));
            // [PORTAL] MINHA CONTA
            controllersOrigem.Add(new ControllersOrigem(idControllerPortalMinhaConta, new List<string>() { "GET", "PUT" }));
            // ÚNICA RESTRIÇÃO É O PUT PARA ALTERAR O GRUPO EMPRESA => PODE VIR DE QUALQUER TELA
            acessoMetodosAPIs.Add("administracao/webpagesusers", controllersOrigem);

            // --------------------------------- FIM - PORTAL ---------------------------------------- //

        }


        // ======================== FIM - VALIDAÇÃO DE ACESSO AOS MÉTODOS DA API ======================== //




        public static bool Autenticado(string token)
        {
            using (var _db = new painel_taxservices_dbContext()){

                _db.Configuration.ProxyCreationEnabled = false;

                var verify = _db.LoginAutenticacaos.Where(v => v.token.Equals(token)).Select(v => v).FirstOrDefault();

                if (verify != null)
                    return true;
            }
            return false;
        }


        public static webpages_Users GetUser(string token)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                return _db.LoginAutenticacaos.Where(v => v.token.Equals(token))
                            .Select(v => v.webpages_Users)
                            .FirstOrDefault<webpages_Users>();
            }
            //return _db.LoginAutenticacaos.Where(v => v.token.Equals(token)).Select(v => v.webpages_Users).FirstOrDefault();
        }

        public static Int32 GetIdUser(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null) return (Int32) user.id_users;
            return 0;
        }

        public static Int32 GetIdGrupo(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null && user.id_grupo != null) return (Int32) user.id_grupo;
            return 0;
        }

        public static string GetCNPJEmpresa(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null && user.id_grupo != null && user.nu_cnpjEmpresa != null) return user.nu_cnpjEmpresa;
            return "";
        }

        public static webpages_Roles GetRole(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null)
            {
                using (var _db = new painel_taxservices_dbContext())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    return _db.webpages_UsersInRoles
                                            .Where(r => r.UserId == user.id_users)
                                            .Where(r => r.RoleId > 50)
                                            .Select(r => r.webpages_Roles)
                                            .FirstOrDefault();
                }
            }
            return null;
        }

        public static Int32 GetRoleId(string token)
        {
            webpages_Roles role = GetRole(token);
            if (role != null) return role.RoleId;
            return 0;
        }

        public static String GetRoleName(string token)
        {
            webpages_Roles role = GetRole(token);
            if (role != null) return role.RoleName;
            return "";
        }

        public static Int32 GetRoleLevel(string token)
        {
            webpages_Roles role = GetRole(token);
            if (role != null) return role.RoleLevel;
            return 4;
        }

        /**
         * Retorna o valor mínimo de nível de privilégio a partir do privilégio do usuário logado
         */
        public static Int32 GetRoleLevelMin(string token)
        {
            Int32 RoleLevel = GetRoleLevel(token);
            if (RoleLevel > 1) return RoleLevel + 1;
            return RoleLevel;
        }

        /**
         * Retorna true se o privilégio do usuário logado é de alguém da ATOS
         */
        public static bool isAtosRole(string token)
        {
            Int32 RoleLevel = GetRoleLevel(token);
            return RoleLevel >= 0 && RoleLevel <= 2;
        }

        /**
         * Retorna true se o privilégio é de alguém da ATOS
         */
        public static bool isAtosRole(webpages_Roles role)
        {
            if (role == null) return false;
            return role.RoleLevel >= 0 && role.RoleLevel <= 2;
        }

        /**
         * Retorna true se o privilégio do usuário logado é de vendedor da ATOS
         */
        public static bool isAtosRoleVendedor(string token)
        {
            string RoleName = GetRoleName(token);
            return isAtosRole(token) && RoleName.ToUpper().Equals("COMERCIAL");
        }

        /**
         * Retorna true se o privilégio é de vendedor da ATOS
         */
        public static bool isAtosRoleVendedor(webpages_Roles role)
        {
            if (role == null) return false;
            return isAtosRole(role) && role.RoleName.ToUpper().Equals("COMERCIAL");
        }

        public static List<Int32> GetIdsGruposEmpresasVendedor(string token)
        {
            List<Int32> lista = new List<Int32>();
 
            Int32 UserId = GetIdUser(token);

            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                lista = _db.grupo_empresa
                        .Where(g => g.id_vendedor == UserId)
                        .Select(g => g.id_grupo)
                        .ToList<Int32>();

                return lista;
            }
        }

        public static Int32 GetIdMethod(Int32 idController, string ds_method)
        {
            Int32 idMethod = 0;

            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                var method = _db.webpages_Methods.Where(m => m.id_controller == idController)
                                                    .Where(m => m.ds_method.ToUpper().Equals(ds_method.ToUpper()))
                                                    .FirstOrDefault();

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

            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                return _db.LogAcesso1
                            .Where(e => e.idUsers == UserId)
                            .Select(e => e.idController ?? 0)
                            .FirstOrDefault();
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
            Int32 idRole = GetRoleId(token);
            if (idRole == 0) return false;

            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                return _db.webpages_Permissions.Where(p => p.id_roles == idRole)
                                               .Where(p => p.webpages_Methods.id_controller == idController)
                                               .Count() > 0;
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

            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                metodo = metodo.ToLower();

                return _db.webpages_Permissions.Where(p => p.id_roles == idRole)
                                               .Where(p => p.webpages_Methods.ds_method.ToLower().Equals(metodo))
                                               .Count() > 0;
            }
        }

        public static Boolean GetPermissionMethod(string token, string ds_method)
        {
            //using (var _db = new painel_taxservices_dbContext())
            //{
            //    Int32 IdUser = Permissoes.GetIdUser(token);


            //    List<dynamic> List = _db.webpages_UsersInRoles.Where(r => r.UserId == IdUser)
            //                                        .Where(r => r.RoleId > 50)
            //                                        .Select(r => new
            //                                        {
            //                                            methods = _db.webpages_Permissions
            //                                                             .Where(p => p.id_roles == r.RoleId)
            //                                                             .Where(p => p.webpages_Methods.webpages_Controllers.id_controller == IdController)
            //                                                             .Select(p => new { ds_method = p.webpages_Methods.ds_method, id_method = p.webpages_Methods.id_method })
            //                                                             .ToList<dynamic>()
            //                                        }
            //                                                ).ToList<dynamic>();


                return false;
            //}
        }




       /* public static bool Autenticad(string token)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                var verify = (from v in _db.LoginAutenticacaos
                              where v.token.Equals(token)
                              select v
                             ).FirstOrDefault();

                            if (verify != null)

                            #region Log de Acesso ao Sistema
                            Log log = new api.Models.Object.Log();
                            log.IdUser = verify.idUsers;
                            log.IdController = 45;
                            log.IdMethod = 51;
                            log.DtAcesso = DateTime.Now;

                            LogAcesso.New(log);
                            #endregion

                                return true;
            }
            return false;
        }*/


    }
}