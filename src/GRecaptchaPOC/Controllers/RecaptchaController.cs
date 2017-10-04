using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GRecaptchaPOC.Controllers
{
    public class RecaptchaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult PostData(RecaptchaModel model)
        {
            var client = new HttpClient();
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("secret", Startup.Configuration["GRecaptcha:Secret"]));
            values.Add(new KeyValuePair<string, string>("response", model.Response));
            var request = new HttpRequestMessage(HttpMethod.Post, Startup.Configuration["GRecaptcha:URL"]) { Content = new FormUrlEncodedContent(values) };
            using (HttpResponseMessage result = client.SendAsync(request).Result)
            {
                var json = result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<RecaptchaResultModel>(json);
                return Json(response);
            }

        }

        public class RecaptchaModel
        {
            public string Response { get; set; }
            public string Value { get; set; }
        }

        public class RecaptchaResultModel
        {
            public bool success { get; set; }
            public string challenge_ts { get; set; }
            public string hostname { get; set; }
        }
    }
}
