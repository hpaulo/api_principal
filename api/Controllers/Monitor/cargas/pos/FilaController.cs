using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.Controllers.Monitor.cargas.pos
{
    public class FilaController : ApiController
    {
        // GET api/fila
        public List<api.Models.Object.LogExecucao> Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {


            try
            {

                using (var _db = new painel_taxservices_dbContext())
                {
                    _db.Configuration.ProxyCreationEnabled = false;



                    string script = @"
                                        SELECT
                                            TOP (" + pageSize + @")
	                                        pos.LogExecution.id as idLogExecucao,
	                                        pos.LogExecution.qtdTransacoes as quantodade,
	                                        pos.LogExecution.vlTotalTransacoes as valor_total,
	                                        pos.LogExecution.statusExecution as status,
	                                        CONVERT(nvarchar(MAX), pos.LogExecution.dtaFiltroTransacoes, 103) as data_carga,
	                                        CONVERT(nvarchar(MAX), pos.LogExecution.dtaExecucaoInicio, 103) as data_inicio,
	                                        cliente.empresa.ds_fantasia as empresa,
	                                        pos.Operadora.nmOperadora as adquirente,
                                            pos.Operadora.id AS idAdquirente
                                        FROM
	                                        pos.LogExecution
                                        INNER JOIN pos.LoginOperadora ON pos.LogExecution.idLoginOperadora = pos.LoginOperadora.id
                                        INNER JOIN cliente.empresa ON pos.LoginOperadora.cnpj = cliente.empresa.nu_cnpj
                                        INNER JOIN pos.Operadora ON pos.LoginOperadora.idOperadora = pos.Operadora.id
                                        WHERE
	                                        pos.LogExecution.statusExecution = 7
                                            AND  pos.LogExecution.dtaExecucaoProxima  <= (CONVERT(VARCHAR(25), GETDATE(), 112) + ' 23:59:59')
                                            AND pos.LogExecution.[row_number] > " + pageNumber + @"
                                        ORDER BY pos.LogExecution.dtaExecucaoProxima ASC

                                    ";

                    List<api.Models.Object.LogExecucao> log = _db.Database.SqlQuery<api.Models.Object.LogExecucao>(script).ToList();

                    return log;
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

        }

        // GET api/fila/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/fila
        public void Post([FromBody]string value)
        {
        }

        // PUT api/fila/5
        public void Put(int id, [FromBody]string value)
        {
            //.Database.SqlQuery<YourEntityType>("storedProcedureName",params);
        }

        // DELETE api/fila/5
        public void Delete(int id)
        {
        }
    }
}
