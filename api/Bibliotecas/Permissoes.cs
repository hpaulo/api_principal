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

        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        // Dicionário para gerenciamento de segurança de métodos POST, PUT e DELETE
        // A partir da url da API, retorna o ID do controller associado
        /*private static Dictionary<string, Int32> dicionarioPortal = new Dictionary<string, Int32>
                            {
                            // ADMINISTRAÇÃO
                            {"administracao/pessoa", GetIdController(new List<string>() { "USUÁRIOS", "GESTÃO DE ACESSOS" })},
                            //{"administracao/tblogacessousuario", GetIdController(new List<string>() { "USUÁRIOS", "GESTÃO DE ACESSOS" })}, // usuário jamais poderá cadastrar ou alterar! Exclusão deve ser avaliada....
                            {"administracao/webpagescontrollers", GetIdController(new List<string>() { "MÓDULOS E FUNCIONALIDADES", "GESTÃO DE ACESSOS" })},
                            //{"administracao/webpagesmembership", GetIdController(new List<string>() { "USUÁRIOS", "GESTÃO DE ACESSOS"})}, // o cara pode resetar a própria senha, mas pode não ter permissão para resetar a de outros usuários
                            {"administracao/webpagesmethods", GetIdController(new List<string>() { "MÓDULOS E FUNCIONALIDADES", "GESTÃO DE ACESSOS" })},
                            {"administracao/webpagespermissions", GetIdController(new List<string>() { "PRIVILÉGIOS", "GESTÃO DE ACESSOS" })},
                            //{"administracao/webpagesrolelevels", GetIdController(new List<string>() { "PRIVILÉGIOS", "GESTÃO DE ACESSOS" })}, // por enquanto, só é possível manipular role levels direto no banco
                            {"administracao/webpagesroles", GetIdController(new List<string>() { "PRIVILÉGIOS", "GESTÃO DE ACESSOS" })},
                            { "administracao/webpagesusers", GetIdController(new List<string>() { "USUÁRIOS", "GESTÃO DE ACESSOS"})},
                            { "administracao/webpagesusersinroles", GetIdController(new List<string>() { "USUÁRIOS", "GESTÃO DE ACESSOS"})}, // avaliar se o usuário logado pode atribuir a permissão ao usuário informado como parâmetro
                            // CARD
                            // ....
                            // CLIENTE
                            {"cliente/empresa", GetIdController(new List<string>() { "FILIAIS", "GESTÃO DE EMPRESAS" })},
                            {"cliente/grupoempresa", GetIdController(new List<string>() { "EMPRESAS", "GESTÃO DE EMPRESAS" })},
                            // POS
                            //{"pos/adquirente", GetIdController(new List<string>() { "", "" })}, // por enquanto, só é possível manipular adquirentes (associadas a ATOS) direto no banco
                            //{"pos/bandeirapos", GetIdController(new List<string>() { "", "" })}, // por enquanto, só é possível manipular bandeiras direto no banco
                            {"pos/loginoperadora", GetIdController(new List<string>() { "DADOS DE ACESSO", "CONSOLIDAÇÃO" })}, // tem um controller SENHAS INVÁLIDAS, que faz UPDATE. Mas sempre quem tem acesso a esse controller tem acesso a DADOS DE ACESSO
                            //{"pos/operadora", GetIdController(new List<string>() { "", "" })}, // por enquanto, só é possível manipular adquirentes direto no banco
                            //{"pos/recebimento", GetIdController(new List<string>() { "RELATÓRIOS", "CONSOLIDAÇÃO" })}, // só é possível ler
                            //{"pos/recebimentoparcela", GetIdController(new List<string>() { "RELATÓRIOS", "CASH FLOW" })}, // só é possível ler
                            //{"pos/terminallogico", GetIdController(new List<string>() { "", "" })}, // por enquanto, só é possível manipular terminais lógicos direto no banco

                        };*/

        private static Int32 GetIdController(List<string> dscontrollers)
        {
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




        public static bool Autenticado(string token)
        {
            //using (_db = new painel_taxservices_dbContext()){

            //_db.Configuration.ProxyCreationEnabled = false;

            var verify = _db.LoginAutenticacaos.Where(v => v.token.Equals(token)).Select(v => v).FirstOrDefault();

            if (verify != null)
                return true;
            //}
            return false;
        }


        public static webpages_Users GetUser(string token)
        {
            _db.Configuration.ProxyCreationEnabled = false;

            return _db.LoginAutenticacaos.Where(v => v.token.Equals(token)).Select(v => v.webpages_Users).FirstOrDefault();
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
                _db.Configuration.ProxyCreationEnabled = false;
                return _db.webpages_UsersInRoles
                                        .Where(r => r.UserId == user.id_users)
                                        .Where(r => r.RoleId > 50)
                                        .Select(r => r.webpages_Roles)
                                        .FirstOrDefault();
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
        public static Boolean isAtosRole(string token)
        {
            Int32 RoleLevel = GetRoleLevel(token);
            return RoleLevel >= 0 && RoleLevel <= 2;
        }

        public static List<Int32> GetIdsGruposEmpresasVendedor(string token)
        {
            List<Int32> lista = new List<Int32>();
 
            Int32 UserId = GetIdUser(token);
            lista = _db.grupo_empresa
                        .Where(g => g.id_vendedor == UserId)
                        .Select(g => g.id_grupo)
                        .ToList<Int32>();

            return lista;
        }

        public static Int32 GetIdMethod(Int32 idController, string ds_method)
        {
            Int32 idMethod = 0;
   
            _db.Configuration.ProxyCreationEnabled = false;

            var method = _db.webpages_Methods.Where(m => m.id_controller == idController)
                                                .Where(m => m.ds_method.ToUpper().Equals(ds_method.ToUpper()))
                                                .FirstOrDefault();

            if (method != null)
                idMethod = method.id_method;

            return idMethod;
        }

        public static bool usuarioTemPermissao(string token, Int32 idController, string ds_method)
        {
            webpages_Users user = GetUser(token);
            if (user == null) return false;

            Int32 idRole = GetRoleId(token);
            Int32 idMethod = GetIdMethod(idController, ds_method);

            _db.Configuration.ProxyCreationEnabled = false;

            return _db.webpages_Permissions.Where(p => p.id_roles == idRole)
                                            .Where(p => p.id_method == idMethod)
                                            .Count() > 0;
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