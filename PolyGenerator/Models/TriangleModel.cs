namespace PolyGenerator.Models
{
    public class TriangleModel
    {
        public TriangleModel()
        {
            
        }
        public TriangleModel(PointModel a, PointModel b, PointModel c)
        {
            this.A = a; this.B = b; this.C = c;
        }
        public  PointModel A { get; set; }
        public  PointModel B { get; set; }
        public  PointModel C { get; set; }

        public List<PointModel> GetVertices()
        {
            return new List<PointModel> { A, B, C };
        }
    }
}
