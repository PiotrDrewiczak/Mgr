using PolyGenerator.Models.Quad;
using PolyGenerator.Models.Triangle;

namespace PolyGenerator.Interfaces
{
    public interface IQuadrangulation
    {
        public List<List<QuadrangulationModel>> GenerateQuadrangulations(List<TriangulationModel> triangulations);
    }
}
