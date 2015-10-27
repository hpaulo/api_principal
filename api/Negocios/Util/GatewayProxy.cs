using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Routing;

namespace api.Negocios.Util
{
    public class GatewayProxy
    {
        public static HttpClient client;
        //public Uri usuarioUri;
        //public HttpRequestContext Request;
        //public string URL = System.Configuration.ConfigurationManager.AppSettings["URLPROXY"];
        //public Boolean STATUS = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["STATUSPROXY"]);

        // GET: /Proxy/
        public static object Get(string URI)
        {
            try
            {
                    //Request = RequestContext;
                    string dados = "";
                    object ListDados = new object();
                    //string URI = Request.Url.Request.RequestUri.AbsoluteUri;///Verifica.Url(Request.Url.Request.RequestUri.AbsoluteUri, Request.Url.Request.RequestUri.PathAndQuery);
                    //if (client == null)
                    //{
                        client = new HttpClient();
                        client.BaseAddress = new Uri(URI); //URL
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("User-Agent", "ApiPortal/1.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.71 Safari/537.36");

                        HttpResponseMessage response = client.GetAsync(URI).Result; // Chamando a api pela url
                        if (response.IsSuccessStatusCode) // Se retornar com sucesso busca os dados
                        {
                            //usuarioUri = response.Headers.Location; //pegando o cabeçalho
                            dados = response.Content.ReadAsStringAsync().Result; //Pegando os dados do Rest e armazenando na variável usuários
                            ListDados = JsonConvert.DeserializeObject(dados);
                            return ListDados;
                        }
                        else
                            throw new HttpResponseException(response);
                    /*}
                    else
                        return new HttpResponseException(HttpStatusCode.InternalServerError);*/
            }
            catch (System.AggregateException ex)
            {
                throw new HttpResponseException(0);
            }
            catch (HttpResponseException e)
            {
                if (e.InnerException != null && e.InnerException.Message == "An error occurred while sending the request.")
                    throw new HttpResponseException(HttpStatusCode.GatewayTimeout);
                else
                    throw new HttpResponseException(e.Response.StatusCode);
            }
        }
    }
}
