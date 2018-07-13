using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;

namespace AppMobile.Controllers
{
    public class ManualController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task Feed()
        {
            var id = "";
            var serviceClient = ServiceClient.CreateFromConnectionString("asdfja");
            var bytes = Encoding.UTF8.GetBytes("Feed");
            var message = new Message(bytes);

            await serviceClient.SendAsync(id, message);
        }

        public async Task Foto()
        {
            var id = "";
            var serviceClient = ServiceClient.CreateFromConnectionString("asdfja");
            var bytes = Encoding.UTF8.GetBytes("Foto");
            var message = new Message(bytes);

            await serviceClient.SendAsync(id, message);
        }

        public async Task<JsonResult> GetUrlFoto()
        {
            var id = "";
            var rm = RegistryManager.CreateFromConnectionString("");
            var twin = await rm.GetTwinAsync(id);

            var valore = twin.Properties.Desired["imageUrl"];

            return Json(new { value = valore });
        }
    }
}