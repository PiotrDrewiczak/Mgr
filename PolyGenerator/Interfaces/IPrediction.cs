using PolyGenerator.Models.Polygon;

namespace PolyGenerator.Interfaces
{
    public interface IPrediction
    {
        public List<PolygonWithCenterModel> CalculateCenter(PolygonModel[] polygons,string pythonPath, string scriptPath, string modelPath);
    }
}
