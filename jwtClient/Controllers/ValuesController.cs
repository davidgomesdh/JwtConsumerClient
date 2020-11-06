using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using jwtClient.Model;

namespace jwtClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private static string _urlBase;

        private static void ConsultarApi(HttpClient client)
        {
            HttpResponseMessage response = client.GetAsync(
                _urlBase + "employee").Result;

            Console.WriteLine();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string resultado = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(resultado);
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                Console.WriteLine("Usuario não autorizado");
            }
            else
                Console.WriteLine("Token provavelmente expirado!");

            HttpResponseMessage response2 = client.GetAsync(
               _urlBase + "manager").Result;

            Console.WriteLine();
            if (response2.StatusCode == HttpStatusCode.OK)
            {
                string resultado = response2.Content.ReadAsStringAsync().Result;
                Console.WriteLine(resultado);
            }
            else if (response2.StatusCode == HttpStatusCode.Forbidden)
            {
                Console.WriteLine("Usuario não autorizado");
            }
            else
                Console.WriteLine("Token provavelmente expirado!");

            HttpResponseMessage response3 = client.GetAsync(
               _urlBase + "authenticated").Result;

            Console.WriteLine();
            if (response3.StatusCode == HttpStatusCode.OK)
            {
                string resultado = response3.Content.ReadAsStringAsync().Result;
                Console.WriteLine(resultado);
            }
            else if (response3.StatusCode == HttpStatusCode.Forbidden)
            {
                Console.WriteLine("Usuario não autorizado");
            }
            else
                Console.WriteLine("Token provavelmente expirado!");

            Console.ReadKey();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile($"appsettings.json");
            var config = builder.Build();

            _urlBase = config.GetSection("API_Access:UrlBase").Value;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage respToken = client.PostAsync(
                    _urlBase + "login", new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            Username = config.GetSection("API_Access:UserID").Value,
                            Password = config.GetSection("API_Access:AccessKey").Value
                        }), Encoding.UTF8, "application/json")).Result;

                string conteudo =
                    respToken.Content.ReadAsStringAsync().Result;
                Console.WriteLine(conteudo);

                if (respToken.StatusCode == HttpStatusCode.OK)
                {
                    Token token = JsonConvert.DeserializeObject<Token>(conteudo);
                    if (token.Authenticated)
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token.AccessToken);

                        ConsultarApi(client);
                        
                    }
                }
                else if(respToken.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Usuario ou senha incorretos");
                }
            }

            Console.WriteLine("\nFinalizado!");
            Console.ReadKey();
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
