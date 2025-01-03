﻿namespace PolyGenerator.Models.Quad
{
    public class QuadrangleModel
    {
        public PointModel A { get; set; }
        public PointModel B { get; set; }
        public PointModel C { get; set; }
        public PointModel D { get; set; }
        public QuadrangleModel(PointModel a, PointModel b, PointModel c, PointModel d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }
        public List<PointModel> GetVertices()
        {
            return new List<PointModel> { A, B, C, D };
        }
    }
}
