namespace PolyGenerator.Models
{
    public class QuadrangulationModel
    {
        public List<QuadrangleModel> Quadrangles { get; set; } = new List<QuadrangleModel>();
        public double Area { get; set; }
    }
}
