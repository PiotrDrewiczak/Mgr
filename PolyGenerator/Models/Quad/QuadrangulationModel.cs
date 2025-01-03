﻿using PolyGenerator.Models.Triangle;

namespace PolyGenerator.Models.Quad
{
    public class QuadrangulationModel
    {
        public List<QuadrangleModel> Quadrangles { get; set; } = new List<QuadrangleModel>();
        public List<TriangleModel> UnpairedTriangles { get; set; } = new List<TriangleModel>();
    }
}
