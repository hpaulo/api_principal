using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Models;

namespace api.Controllers.Monitor.cargas.pos
{
    public class NovaCargaController : ApiController
    {
        // GET api/novacarga
        public List<api.Models.Object.GrupoEmpresa> Get()
        {

            //DateTime teste = Convert.ToDateTime("Fri Jun 05 2015 00:00:00 GMT-0300 (BRT)");
            using(var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                List<api.Models.Object.GrupoEmpresa> grupo = (from g in _db.grupo_empresa
                                                              where g.fl_cardservices.Equals(true)
                                                              select new api.Models.Object.GrupoEmpresa { empresa = g.ds_nome, empresaId = g.id_grupo }
                                                              ).ToList();

                return grupo;
            
            }
        }

        // GET api/novacarga/5
        public List<api.Models.Object.GrupoFilial> Get(int id)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                List<api.Models.Object.GrupoFilial> grupoFiliais = (from e in _db.empresas
                                                                     where e.id_grupo.Equals(id)
                                                                     select new api.Models.Object.GrupoFilial { filial = e.ds_fantasia, cnpj = e.nu_cnpj }
                                                                    ).ToList();

                return grupoFiliais;
            }
        }


        // GET api/novacarga/5
        public List<api.Models.Object.GrupoAdquirente> Get(int id, string cnpj)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                List<api.Models.Object.GrupoAdquirente> grupoAdquirentes = (from o in _db.LoginOperadoras
                                                                            where o.idGrupo.Equals(id) && o.cnpj.Equals(cnpj)
                                                                            select new api.Models.Object.GrupoAdquirente { adquirente = o.Operadora.nmOperadora, idAdquirente = o.idOperadora }
                                                                    ).ToList();

                return grupoAdquirentes;
            }
        }

        // POST api/novacarga
        public int Post([FromBody]api.Models.Object.NovaCarga value)
        {
            try
            {
                int retorno = 0;
                string date = value.data.Substring(0, 10);
                DateTime dt = Convert.ToDateTime(date);
                DateTime dtNow = dt.AddDays(1);

                using (var _db = new painel_taxservices_dbContext())
                {
                    _db.Configuration.ProxyCreationEnabled = false;


                    var lo = (from l in _db.LoginOperadoras
                              where l.idOperadora.Equals(value.adquirente)
                              select l).FirstOrDefault();
                    int idLogin = lo.id;


                    api.Models.LogExecution verify = (from a in _db.LogExecutions
                                                      join l in _db.LoginOperadoras on a.idLoginOperadora equals l.id
                                                      where l.idGrupo.Equals(value.empresa)
                                                            && l.cnpj.Equals(value.filial)
                                                            && l.idOperadora.Equals(value.adquirente)
                                                            && a.dtaFiltroTransacoes.Equals(dt)
                                                      select (api.Models.LogExecution)a
                                  ).FirstOrDefault();


                    if (verify == null)
                    {

                        LogExecution log = new LogExecution();
                        log.idOperadora = value.adquirente;
                        log.idLoginOperadora = lo.id;
                        log.statusExecution = "7";
                        log.dtaExecucaoProxima = dtNow;
                        log.dtaFiltroTransacoes = dt;
                        log.dtaFiltroTransacoesFinal = dt;

                        _db.LogExecutions.Add(log);
                        _db.SaveChanges();

                        retorno = log.id;
                    }
                    else
                    {
                        string script = "exec up_ReprocessarCarga " + verify.id;
                        int ret = _db.Database.ExecuteSqlCommand(script);
                        retorno = verify.id;
                    }
                    return retorno;
                }
            }
            catch(Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/novacarga/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/novacarga/5
        public void Delete(int id)
        {
        }
    }
}
