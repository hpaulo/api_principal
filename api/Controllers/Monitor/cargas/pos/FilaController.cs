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
	                                        TOP (" + pageSize + @") *
                                        FROM
	                                        (
		                                        SELECT
			                                        pos.LogExecution.id AS idLogExecucao,
			                                        pos.LogExecution.qtdTransacoes AS quantodade,
			                                        pos.LogExecution.vlTotalTransacoes AS valor_total,
			                                        pos.LogExecution.statusExecution AS status,
			                                        CONVERT (
				                                        nvarchar (MAX),
				                                        pos.LogExecution.dtaFiltroTransacoes,
				                                        103
			                                        ) AS data_carga,
			                                        CONVERT (
				                                        nvarchar (MAX),
				                                        pos.LogExecution.dtaExecucaoInicio,
				                                        103
			                                        ) AS data_inicio,
			                                        cliente.empresa.ds_fantasia AS empresa,
			                                        pos.Operadora.nmOperadora AS adquirente,
			                                        pos.Operadora.id AS idAdquirente,
			                                        row_number () OVER (

				                                        ORDER BY
					                                        pos.LogExecution.dtaExecucaoProxima ASC
			                                        ) AS [row_number]
		                                        FROM
			                                        pos.LogExecution
		                                        INNER JOIN pos.LoginOperadora ON pos.LogExecution.idLoginOperadora = pos.LoginOperadora.id
		                                        INNER JOIN cliente.empresa ON pos.LoginOperadora.cnpj = cliente.empresa.nu_cnpj
		                                        INNER JOIN pos.Operadora ON pos.LoginOperadora.idOperadora = pos.Operadora.id
		                                        WHERE
			                                        pos.LogExecution.statusExecution = 7
		                                        AND pos.LogExecution.dtaExecucaoProxima <= (
			                                        CONVERT (VARCHAR(25), GETDATE(), 112) + ' 23:59:00'
		                                        )
	                                        ) AS [Extent1]
                                        WHERE
	                                        [Extent1].[row_number] > " + (pageNumber * pageSize);

                    if (colecao == 0)
                    {
                        if (orderBy == 0)
                        {
                            script += " ORDER BY [Extent1].row_number ASC ";
                        }
                        else
                        {
                            script += " ORDER BY [Extent1].row_number DESC ";
                        }

                    }
                    else {
                        if (orderBy == 0)
                        {
                            script += " ORDER BY [Extent1].idLogExecucao ASC ";
                        }
                        else
                        {
                            script += " ORDER BY [Extent1].idLogExecucao DESC ";
                        }
                    }
                    
                    
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
