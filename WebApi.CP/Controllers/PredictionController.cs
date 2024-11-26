using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PolyGenerator.Interfaces;
using WebApi.CP.Models;

namespace WebApi.CP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictionController : Controller
    {
            private IOptions<AppSettingsModel> settings;
            private IExcelGenerator excelGenerator;
            public PredictionController(IOptions<AppSettingsModel> settings, IExcelGenerator excelGenerator)
            {
                this.settings = settings;
                this.excelGenerator = excelGenerator;
            }



    }
}
