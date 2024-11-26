namespace PolyGenerator.Models.Triangle
{
    public class TriangulationModel
    {
        public List<TriangleModel> Triangles { get; set; } = new List<TriangleModel>();
        public double Area { get; set; }
    }
}
