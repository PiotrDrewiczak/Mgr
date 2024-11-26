namespace WebApi.CP.Models
{
    public class AppSettingsModel
    {
        public const string ApplicationSettings = "ApplicationSettings";

        public required string PythonPath { get; set; }
        public required string ExcelPath { get; set; }
        public required string PythonScriptPath { get; set; }
        public required string PythonRectangleScriptPath { get; set; }
        public required string PolygonOutput { get; set; }
        public int NumberOfPolygons { get; set; }
        public int NumberOfVertices { get; set; }
    }
}
