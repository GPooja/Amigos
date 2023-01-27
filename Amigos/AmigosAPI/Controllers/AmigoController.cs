using AmigosAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmigosAPI.Controllers
{
    [Route("api/Amigo")]
    [ApiController]
    public class AmigoController : ControllerBase
    {
        [Route("GetAmigos")]
        [HttpGet]
        public Amigo GetAmigos()
        {
            return new Amigo();
        }

        [Route("GetAmigoName/{id}")]
        [HttpGet]
        public string GetAmigoName(int amigoID)
        {
            var amigo = new Amigo();
            amigo.Name = "Pooja";
            return amigo.Name;
        }
    }
}
