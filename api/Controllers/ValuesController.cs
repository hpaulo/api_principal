using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public  HttpResponseMessage Get()
        {
            return Request.CreateResponse<IEnumerable<string>>(HttpStatusCode.OK, new string[] { "value1", "value2" });
        }

        // GET api/values/5
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse<IEnumerable<string>>(HttpStatusCode.OK, new string[] { "value" });
        }

        // POST api/values
        public HttpResponseMessage Post([FromBody]dynamic value)
        {
            return Request.CreateResponse<Int32>(HttpStatusCode.OK, 100);
        }

        // PUT api/values/5
        public HttpResponseMessage Put(int id, [FromBody]dynamic value)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/values/5
        public HttpResponseMessage Delete(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}