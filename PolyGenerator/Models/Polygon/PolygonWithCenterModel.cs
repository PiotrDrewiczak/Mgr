namespace PolyGenerator.Models.Polygon
{
    public class PolygonWithCenterModel
    {
        public List<PointModel> Vertices { get; set; } = new List<PointModel>();
        public double X { get; set; }
        public double Y { get; set; }
    }
}
