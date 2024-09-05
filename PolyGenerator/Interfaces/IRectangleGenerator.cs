using PolyGenerator.Models;

namespace PolyGenerator.Interfaces
{
    public interface IRectangleGenerator
    {
        public List<RectanglesModel> GenerateQuadrilateral(string pythonPath, string pythonRectangleScriptPath, List<List<QuadrangulationModel>> quadranglesModels);
    }
}
