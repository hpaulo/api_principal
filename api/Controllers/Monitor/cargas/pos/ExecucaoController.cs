using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Models.Object;

namespace api.Controllers.Monitor.cargas.pos
{
    public class ExecucaoController : ApiController
    {
        // GET api/execucao
        public List<api.Models.Object.LogExecucao> Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            

            try
            {

                using (var _db = new painel_taxservices_dbContext())
                {
                    _db.Configuration.ProxyCreationEnabled = false;



                    string script = @"
                                        SELECT
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
	                                        pos.LogExecution.statusExecution = 0
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

        // GET api/execucao/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/execucao
        public void Post([FromBody]string value)
        {
        }

        // PUT api/execucao/5
        public HttpResponseMessage Put(string token, [FromBody]Int32 id)
        {
            try
            {
                using (var _db = new painel_taxservices_dbContext())
                {
                    //_db.Configuration.ProxyCreationEnabled = false;

                    LogExecution log = _db.LogExecutions.Where(s => s.id == id).FirstOrDefault<LogExecution>();
                    log.statusExecution = "3";

                    _db.SaveChanges();
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

        }

        // DELETE api/execucao/5
        public void Delete(int id)
        {
        }
    }
}
