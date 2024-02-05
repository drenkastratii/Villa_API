using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Villa_API.Data;
using Villa_API.Models;
using Villa_API.Models.DTO;

namespace Villa_API.Controllers
{
    [Route("api/Villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<VillaDTO> GetVillas()
        {
            return VillaStore.villaList;
        }
    }
}
