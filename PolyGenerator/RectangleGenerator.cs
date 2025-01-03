﻿using PolyGenerator.Interfaces;
using PolyGenerator.Models;
using PolyGenerator.Models.Quad;

namespace PolyGenerator
{
    public class RectangleGenerator : IRectangleGenerator
    {
        public List<List<RectanglesModel>> GenerateQuadrilateral(string pythonPath, string pythonRectangleScriptPath, List<List<QuadrangulationModel>> quadranglesModels)
        {
            return PythonRunner.ProcessQuadrilaterals(
                pythonPath,
                pythonRectangleScriptPath,
                quadranglesModels
                );
        }
    }
}
