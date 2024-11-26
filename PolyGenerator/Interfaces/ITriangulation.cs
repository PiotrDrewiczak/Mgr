using PolyGenerator.Models;
using PolyGenerator.Models.Triangle;

namespace PolyGenerator.Interfaces
{
    public interface ITriangulation
    {
        List<TriangulationModel> GenerateTriangulation(PolygonModel[] polygons);
    }
}
