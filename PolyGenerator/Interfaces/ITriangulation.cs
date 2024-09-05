using PolyGenerator.Models;

namespace PolyGenerator.Interfaces
{
    public interface ITriangulation
    {
        List<TriangulationModel> GenerateTriangulation(PolygonModel[] polygons);
    }
}
