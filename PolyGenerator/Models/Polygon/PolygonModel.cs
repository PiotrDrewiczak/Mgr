﻿namespace PolyGenerator.Models.Polygon
{
    public class PolygonModel
    {
        public List<PointModel> Vertices { get; set; } = new List<PointModel>();
        public double Area { get; set; }
    }
}
