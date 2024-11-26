using PolyGenerator.Models;
using PolyGenerator.Models.Quad;

namespace PolyGenerator.Interfaces
{
    public interface IRectangleGenerator
    {
        public List<List<RectanglesModel>> GenerateQuadrilateral(string pythonPath, string pythonRectangleScriptPath, List<List<QuadrangulationModel>> quadranglesModels);
    }
}
