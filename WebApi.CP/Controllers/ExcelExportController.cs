using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PolyGenerator.Interfaces;
using PolyGenerator.Models;
using WebApi.CP.Models;

namespace WebApi.CP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExcelExportController : ControllerBase
    {
        private IOptions<AppSettingsModel> settings;
        private IExcelGenerator excelGenerator;
        public ExcelExportController(IOptions<AppSettingsModel> settings, IExcelGenerator excelGenerator)
        {
            this.settings = settings;
            this.excelGenerator = excelGenerator;
        }
        [HttpPost("export")]
        public IActionResult Export([FromBody] ExportDataModel data)
        {

            excelGenerator.SaveToExcel(data, settings.Value.ExcelPath);

            return Ok("Export completed successfully.");
        }
    }
}
