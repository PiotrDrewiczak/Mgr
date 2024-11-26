namespace PolyGenerator.Models.Triangle
{
    public class TriangleModel
    {
        public TriangleModel(PointModel a, PointModel b, PointModel c)
        {
            A = a; B = b; C = c;
        }
        public PointModel A { get; set; }
        public PointModel B { get; set; }
        public PointModel C { get; set; }

        public List<PointModel> GetVertices()
        {
            return new List<PointModel> { A, B, C };
        }
    }
}
