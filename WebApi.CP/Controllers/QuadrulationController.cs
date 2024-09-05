using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PolyGenerator;
using PolyGenerator.Interfaces;
using PolyGenerator.Models;
using WebApi.CP.Models;

namespace WebApi.CP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuadrulationController : ControllerBase
    {
        private IOptions<AppSettingsModel> settings;
        private readonly IQuadrulation quadrulation;
        private readonly IRectangleGenerator rectangle;


        public QuadrulationController(IOptions<AppSettingsModel> settings, IQuadrulation quadrulation, IRectangleGenerator rectangle)
        {
            this.settings = settings;
            this.quadrulation = quadrulation;
            this.rectangle = rectangle;
        }

        [HttpPost("quadrulate")]
        public List<RectanglesModel> Quadrangle([FromBody] TriangulationModel[] triangulations)
        {
            var quadrangles = quadrulation.GenerateQuadrangulations(triangulations.ToList());

            return rectangle.GenerateQuadrilateral(
                     settings.Value.PythonPath,
                     settings.Value.PythonRectangleScriptPath,
                     quadrangles);
        }
    }
}
