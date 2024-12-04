using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PolyGenerator.Interfaces;
using PolyGenerator.Models.Polygon;
using WebApi.CP.Models;

namespace WebApi.CP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PolygonController : ControllerBase
    {
        private IOptions<AppSettingsModel> settings;
        private readonly IPolygonGenerator polygonGenerator;

        public PolygonController(IOptions<AppSettingsModel> settings, IPolygonGenerator polygonGenerator)
        {
            this.settings = settings;
            this.polygonGenerator = polygonGenerator;
        }

        [HttpGet(Name = "GetPolygons")]
        public List<PolygonModel> GetPolygons()
        {

            return polygonGenerator.GeneratePolygon(settings.Value.PythonPath,
                                                   settings.Value.PythonScriptPath,
                                                   settings.Value.PolygonOutput,
                                                   settings.Value.NumberOfVertices,
                                                   settings.Value.NumberOfPolygons);
        }
    }
}
