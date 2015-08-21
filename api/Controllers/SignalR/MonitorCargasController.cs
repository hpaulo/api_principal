using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.Controllers.SignalR
{
    public class MonitorCargasController : ApiController
    {
        // GET: api/MonitorCargas
        public IEnumerable<string> Get()
        {


            return new string[] { "value1", "value2" };
        }

        // GET: api/MonitorCargas/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MonitorCargas
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/MonitorCargas/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MonitorCargas/5
        public void Delete(int id)
        {
        }
    }
}
