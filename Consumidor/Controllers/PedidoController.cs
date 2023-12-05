using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Consumidor.Models;
using Newtonsoft.Json;
using System.Text;
using System.Configuration;

namespace Consumidor.Controllers
{
    public class PedidoController : Controller
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

        // Assuming PedidoCLS is equivalent to LibroCLS but for Pedido
        public ActionResult Lista()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(UrlApi);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session["token"].ToString());

            HttpResponseMessage response = httpClient.GetAsync("/api/pedidos").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Token");
            }
            else
            {
                string data = response.Content.ReadAsStringAsync().Result;
                List<Pedido> pedidos = JsonConvert.DeserializeObject<List<Pedido>>(data);

                return Json(
                    new
                    {
                        success = true,
                        data = pedidos,
                        message = "done"
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        public ActionResult Guardar(int IdPedido, int? IdUsuario, int? IdProducto, int? Cantidad)
        {
            try
            {
                if (!UsuarioAutenticado() || !TokenValido())
                {
                    Metodos metodos = new Metodos();
                    HttpContext.Session.Add("token", metodos.ObtenerToken());
                    HttpContext.Session.Add("horaToken", DateTime.Now);
                }
                Pedido pedido = new Pedido();
                pedido.IdPedido = IdPedido;
                pedido.IdUsuario = IdUsuario;
                pedido.IdProducto = IdProducto;
                pedido.Cantidad = Cantidad;

                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(UrlApi);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session["token"].ToString());

                string pedidoJson = JsonConvert.SerializeObject(pedido);
                HttpContent body = new StringContent(pedidoJson, Encoding.UTF8, "application/json");

                HttpResponseMessage findIdResponse = httpClient.GetAsync($"/api/pedidos/{IdPedido}").Result;

                if (!findIdResponse.IsSuccessStatusCode)
                {
                    HttpResponseMessage response = httpClient.PostAsync("/api/pedidos", body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(
                            new
                            {
                                success = true,
                                message = "Pedido creado satisfactoriamente"
                            }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Token");
                    }
                }
                else
                {
                    HttpResponseMessage response = httpClient.PutAsync($"/api/pedidos/{IdPedido}", body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(
                            new
                            {
                                success = true,
                                message = "Pedido modificado satisfactoriamente"
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

        public JsonResult Eliminar(int IdPedido)
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

            HttpResponseMessage response = httpClient.DeleteAsync($"/api/pedidos/{IdPedido}").Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(
                    new
                    {
                        success = true,
                        message = "Pedido eliminado satisfactoriamente"
                    }, JsonRequestBehavior.AllowGet);
            }

            throw new Exception("Error al eliminar");
        }
    }
}
