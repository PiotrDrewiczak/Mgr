namespace PolyGenerator.Models
{
    public class TriangleModel
    {
        public required PointModel A { get; set; }
        public required PointModel B { get; set; }
        public required PointModel C { get; set; }

        public List<PointModel> GetVertices()
        {
            return new List<PointModel> { A, B, C };
        }
    }
}
