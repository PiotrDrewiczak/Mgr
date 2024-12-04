using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PolyGenerator.Interfaces;
using PolyGenerator.Models.Polygon;
using WebApi.CP.Models;

namespace WebApi.CP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictionController : ControllerBase
    {
     private IOptions<AppSettingsModel> settings;
     private IPrediction prediction;

     public PredictionController(IOptions<AppSettingsModel> settings, IPrediction prediction)
     {
      this.settings = settings;
      this.prediction = prediction;
     }
     
     [HttpPost("calculatecenter")]
     public List<PolygonWithCenterModel> CalculateCenter([FromBody] PolygonModel[] polygons)
     {
        return prediction.CalculateCenter(polygons,
            settings.Value.PythonPath,
            settings.Value.ScriptPathPrediction,
            settings.Value.PythonModelPath);
     }
    }
}
