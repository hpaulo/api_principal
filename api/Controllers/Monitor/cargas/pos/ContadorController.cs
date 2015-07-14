using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.Controllers.Monitor.cargas.pos
{
    public class ContadorController : ApiController
    {
        // GET api/contador
        public api.Models.Object.Contador Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            try
            {

                using (var _db = new painel_taxservices_dbContext())
                {
                    _db.Configuration.ProxyCreationEnabled = false;



                    string script = @"
                                        
                                    SELECT

                                    (
                                    SELECT
	                                    COUNT(1)
                                    FROM
	                                    pos.LogExecution
                                    INNER JOIN pos.LoginOperadora ON pos.LogExecution.idLoginOperadora = pos.LoginOperadora.id
                                    INNER JOIN cliente.empresa ON pos.LoginOperadora.cnpj = cliente.empresa.nu_cnpj
                                    INNER JOIN pos.Operadora ON pos.LoginOperadora.idOperadora = pos.Operadora.id
                                    WHERE
	                                    pos.LogExecution.statusExecution = 0
                                    ) AS execucao
                                    ,
                                    (
                                    SELECT
	                                    COUNT(1)
                                    FROM
	                                    pos.LogExecution
                                    INNER JOIN pos.LoginOperadora ON pos.LogExecution.idLoginOperadora = pos.LoginOperadora.id
                                    INNER JOIN cliente.empresa ON pos.LoginOperadora.cnpj = cliente.empresa.nu_cnpj
                                    INNER JOIN pos.Operadora ON pos.LoginOperadora.idOperadora = pos.Operadora.id
                                    WHERE
	                                    pos.LogExecution.statusExecution = 3
	                                    AND MONTH(pos.LogExecution.dtaExecucaoProxima) = MONTH(GETDATE())
	                                    AND YEAR(pos.LogExecution.dtaExecucaoProxima) = YEAR(GETDATE())
                                    ) as erro
                                    ,
                                    (
                                    SELECT
                                    COUNT(1)
                                    FROM
                                    pos.LogExecution
                                    INNER JOIN pos.LoginOperadora ON pos.LogExecution.idLoginOperadora = pos.LoginOperadora.id
                                    INNER JOIN cliente.empresa ON pos.LoginOperadora.cnpj = cliente.empresa.nu_cnpj
                                    INNER JOIN pos.Operadora ON pos.LoginOperadora.idOperadora = pos.Operadora.id
                                    WHERE
                                    pos.LogExecution.statusExecution = 7
	                                    AND  pos.LogExecution.dtaExecucaoProxima  <= (CONVERT(VARCHAR(25), GETDATE(), 112) + ' 23:59:59')
                                    ) fila
                                    ,
                                    (SELECT
	                                        COUNT(1)
                                    FROM
	                                    pos.LogExecution
                                    INNER JOIN pos.LoginOperadora ON pos.LogExecution.idLoginOperadora = pos.LoginOperadora.id
                                    INNER JOIN cliente.empresa ON pos.LoginOperadora.cnpj = cliente.empresa.nu_cnpj
                                    INNER JOIN pos.Operadora ON pos.LoginOperadora.idOperadora = pos.Operadora.id
                                    WHERE
	                                    pos.LogExecution.statusExecution = 1
	                                    AND MONTH(pos.LogExecution.dtaExecucaoProxima) = MONTH(GETDATE())
	                                    AND YEAR(pos.LogExecution.dtaExecucaoProxima) = YEAR(GETDATE())
                                    ) AS sucesso
                                    ";

                    api.Models.Object.Contador log = _db.Database.SqlQuery<api.Models.Object.Contador>(script).FirstOrDefault();

                    return log;
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // GET api/contador/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/contador
        public void Post([FromBody]string value)
        {
        }

        // PUT api/contador/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/contador/5
        public void Delete(int id)
        {
        }
    }
}
