using PolyGenerator.Models.Polygon;

namespace PolyGenerator.Interfaces
{
    public interface IPolygonGenerator
    {
        public List<PolygonModel> GeneratePolygon(string _pythonPath, string _pythonScriptPath, string _polygonOutput, int _numberOfVertices, int _numberOfPolygons);
    }
}
