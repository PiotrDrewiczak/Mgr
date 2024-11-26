using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PolyGenerator.Interfaces;
using PolyGenerator.Models;
using PolyGenerator.Models.Triangle;
using WebApi.CP.Models;

namespace WebApi.CP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TriangulationController : Controller
    {
        private readonly IOptions<AppSettingsModel> settings;

        private readonly ITriangulation triangulation;

        public TriangulationController(IOptions<AppSettingsModel> settings, ITriangulation triangulation)
        {
            this.settings = settings;
            this.triangulation = triangulation;
        }

        [HttpPost("triangulate")]
        public List<TriangulationModel> Triangulate([FromBody] PolygonModel[] polygons)
        {
            return triangulation.GenerateTriangulation(polygons);
        }
    }
}
