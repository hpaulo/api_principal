using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Bibliotecas;

namespace api.Controllers.Login
{
    public class StatusController : ApiController
    {
        // GET: /Status/
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse<object>(HttpStatusCode.OK, Device.Info());
        }


    }
}
