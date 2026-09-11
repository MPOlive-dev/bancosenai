using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    public class DocumentoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
