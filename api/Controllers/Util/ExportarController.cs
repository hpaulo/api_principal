using api.Bibliotecas;
using api.Negocios.Pos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace api.Controllers.Util
{
    public class ExportarController : ApiController
    {
        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //[HttpGet]
        //public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        //{
        //    Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

        //    MediaTypeHeaderValue mediaType = MediaTypeHeaderValue.Parse("application/octet-stream");
        //    List<Models.pessoa> list = Negocios.Administracao.GatewayPessoa.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString);
        //    byte[] excelFile = Negocios.Util.GatewayExportar.Excel();
        //    string fileName = "Orders.xlsx";
        //    MemoryStream memoryStream = new MemoryStream(excelFile);
        //    HttpResponseMessage response = response = Request.CreateResponse(HttpStatusCode.OK);
        //    response.Content = new StreamContent(memoryStream);
        //    response.Content.Headers.ContentType = mediaType;
        //    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("fileName") { FileName = fileName };
        //    return response;
        //}

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post(string token, [FromBody]string value)
        {

        }

        public HttpResponseMessage Post(string token, object param)
        {
            try
            {
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<object>(HttpStatusCode.OK, param);
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}