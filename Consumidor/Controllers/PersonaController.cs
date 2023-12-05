using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Configuration;
using Consumidor.Models;

namespace APIConsumption.Controllers
{
    public class PersonaController : Controller
    {
        private string UrlApi = ConfigurationManager.AppSettings["UrlApi"];
        private int DuracionToken = int.Parse(ConfigurationManager.AppSettings["DuracionToken"]);
        private DateTime HoraToken;

        private bool UsuarioAutenticado()
        {
            return HttpContext.Session["token"] != null;
        }

        private bool TokenValido()
        {
            if (UsuarioAutenticado())
            {
                HoraToken = (DateTime)HttpContext.Session["horaToken"];
                return DuracionToken >= DateTime.Now.Subtract(HoraToken).Minutes;
            }
            return false;
        }

        public ActionResult Index()
        {
            if (!UsuarioAutenticado() || !TokenValido())
            {
                Metodos metodos = new Metodos();
                HttpContext.Session.Add("token", metodos.ObtenerToken());
                HttpContext.Session.Add("horaToken", DateTime.Now);
            }
            return View();
        }

        public ActionResult Lista()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(UrlApi);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session["token"].ToString());

            HttpResponseMessage response = httpClient.GetAsync("/api/personas").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Token");
            }
            else
            {
                string data = response.Content.ReadAsStringAsync().Result;
                List<Persona> personas = JsonConvert.DeserializeObject<List<Persona>>(data);

                return Json(
                    new
                    {
                        success = true,
                        data = personas,
                        message = "done"
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        public ActionResult Guardar(int IdUsuario, string Nombre, string Apellido, int? Telefono)
        {
            try
            {
                if (!UsuarioAutenticado() || !TokenValido())
                {
                    Metodos metodos = new Metodos();
                    HttpContext.Session.Add("token", metodos.ObtenerToken());
                    HttpContext.Session.Add("horaToken", DateTime.Now);
                }
                Persona persona = new Persona();
                persona.IdUsuario = IdUsuario;
                persona.Nombre = Nombre;
                persona.Apellido = Apellido;
                persona.Telefono = Telefono;

                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(UrlApi);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session["token"].ToString());

                string personaJson = JsonConvert.SerializeObject(persona);
                HttpContent body = new StringContent(personaJson, Encoding.UTF8, "application/json");

                HttpResponseMessage findIdResponse = httpClient.GetAsync($"/api/personas/{IdUsuario}").Result;

                if (!findIdResponse.IsSuccessStatusCode)
                {
                    HttpResponseMessage response = httpClient.PostAsync("/api/personas", body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(
                            new
                            {
                                success = true,
                                message = "Persona creada satisfactoriamente"
                            }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Token");
                    }
                }
                else
                {
                    HttpResponseMessage response = httpClient.PutAsync($"/api/personas/{IdUsuario}", body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(
                            new
                            {
                                success = true,
                                message = "Persona modificada satisfactoriamente"
                            }, JsonRequestBehavior.AllowGet);
                    }
                }
                throw new Exception("Error al guardar");
            }
            catch (Exception ex)
            {
                return Json(
                    new
                    {
                        success = false,
                        message = ex.InnerException
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Eliminar(int IdUsuario)
        {
            if (!UsuarioAutenticado() || !TokenValido())
            {
                Metodos metodos = new Metodos();
                HttpContext.Session.Add("token", metodos.ObtenerToken());
                HttpContext.Session.Add("horaToken", DateTime.Now);
            }

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(UrlApi);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session["token"].ToString());

            HttpResponseMessage response = httpClient.DeleteAsync($"/api/personas/{IdUsuario}").Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(
                    new
                    {
                        success = true,
                        message = "Persona eliminada satisfactoriamente"
                    }, JsonRequestBehavior.AllowGet);
            }

            throw new Exception("Error al eliminar");
        }
    }
}
