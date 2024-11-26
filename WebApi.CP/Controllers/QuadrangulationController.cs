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
    public class QuadrangulationController : ControllerBase
    {
        private readonly IOptions<AppSettingsModel> settings;
        private readonly IQuadrangulation quadrulation;
        private readonly IRectangleGenerator rectangle;


        public QuadrangulationController(IOptions<AppSettingsModel> settings, IQuadrangulation quadrulation, IRectangleGenerator rectangle)
        {
            this.settings = settings;
            this.quadrulation = quadrulation;
            this.rectangle = rectangle;
        }

        [HttpPost("quadrangulate")]
        public List<List<RectanglesModel>> Quadrangulation([FromBody] TriangulationModel[] triangulations)
        {
            var quadrangles = quadrulation.GenerateQuadrangulations(triangulations.ToList());

            return rectangle.GenerateQuadrilateral(
                     settings.Value.PythonPath,
                     settings.Value.PythonRectangleScriptPath,
                     quadrangles);
        }
    }
}
