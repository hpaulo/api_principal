using OFXSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace api.Controllers.Card
{
    public class TesteUploadController : ApiController
    {
        // GET: api/TesteUpload
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/TesteUpload/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TesteUpload
        public HttpResponseMessage Post()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];

                    if (!File.Exists("~/App_Data/" + postedFile.FileName))
                    {
                        var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);

                        var parser = new OFXDocumentParser();
                        OFXDocument ofxDocument = parser.Import(new FileStream(filePath, FileMode.Open));

                        docfiles.Add(filePath);
                        result = Request.CreateResponse(HttpStatusCode.Created, ofxDocument);
                    }
                }
                
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }

        // PUT: api/TesteUpload/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/TesteUpload/5
        public void Delete(int id)
        {
        }
    }
}
