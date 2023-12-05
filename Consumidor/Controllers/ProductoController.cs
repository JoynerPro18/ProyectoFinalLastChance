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
    public class ProductoController : Controller
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

            HttpResponseMessage response = httpClient.GetAsync("/api/productos").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Token");
            }
            else
            {
                string data = response.Content.ReadAsStringAsync().Result;
                List<Producto> productos = JsonConvert.DeserializeObject<List<Producto>>(data);

                return Json(
                    new
                    {
                        success = true,
                        data = productos,
                        message = "done"
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        public ActionResult Guardar(int IdProducto, string Nombre, int? Precio, string Categoria)
        {
            try
            {
                if (!UsuarioAutenticado() || !TokenValido())
                {
                    Metodos metodos = new Metodos();
                    HttpContext.Session.Add("token", metodos.ObtenerToken());
                    HttpContext.Session.Add("horaToken", DateTime.Now);
                }
                Producto producto = new Producto();
                producto.IdProducto = IdProducto;
                producto.Nombre = Nombre;
                producto.Precio = Precio;
                producto.Categoria = Categoria;

                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(UrlApi);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session["token"].ToString());

                string productoJson = JsonConvert.SerializeObject(producto);
                HttpContent body = new StringContent(productoJson, Encoding.UTF8, "application/json");

                HttpResponseMessage findIdResponse = httpClient.GetAsync($"/api/productos/{IdProducto}").Result;

                if (!findIdResponse.IsSuccessStatusCode)
                {
                    HttpResponseMessage response = httpClient.PostAsync("/api/productos", body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(
                            new
                            {
                                success = true,
                                message = "Producto creado satisfactoriamente"
                            }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Token");
                    }
                }
                else
                {
                    HttpResponseMessage response = httpClient.PutAsync($"/api/productos/{IdProducto}", body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(
                            new
                            {
                                success = true,
                                message = "Producto modificado satisfactoriamente"
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

        public JsonResult Eliminar(int IdProducto)
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

            HttpResponseMessage response = httpClient.DeleteAsync($"/api/productos/{IdProducto}").Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(
                    new
                    {
                        success = true,
                        message = "Producto eliminado satisfactoriamente"
                    }, JsonRequestBehavior.AllowGet);
            }

            throw new Exception("Error al eliminar");
        }
    }
}
