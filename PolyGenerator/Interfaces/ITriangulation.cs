using PolyGenerator.Models.Polygon;
using PolyGenerator.Models.Triangle;

namespace PolyGenerator.Interfaces
{
    public interface ITriangulation
    {
        List<TriangulationModel> GenerateTriangulation(PolygonModel[] polygons);
    }
}
