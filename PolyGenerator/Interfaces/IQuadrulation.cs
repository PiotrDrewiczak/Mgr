using PolyGenerator.Models;

namespace PolyGenerator.Interfaces
{
    public interface IQuadrulation
    {
        public List<List<QuadrangulationModel>> GenerateQuadrangulations(List<TriangulationModel> triangulations);
    }
}
