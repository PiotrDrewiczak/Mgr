namespace PolyGenerator.Models
{
    public class ExportDataModel
    {
        public required List<PolygonModel> Polygons { get; set; }
        public required List<RectangleCenterModel> RectangleCenters { get; set; }
    }
}
