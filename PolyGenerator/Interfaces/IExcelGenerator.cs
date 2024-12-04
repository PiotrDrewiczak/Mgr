using PolyGenerator.Models;

namespace PolyGenerator.Interfaces
{
    public interface IExcelGenerator
    {
        public string SaveToExcel(ExportDataModel exportData, string excelPath);
    }
}
