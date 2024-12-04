using PolyGenerator.Interfaces;
using PolyGenerator.Models;
using PolyGenerator.Models.Polygon;

namespace PolyGenerator
{
    public class PolygonGenerator :  IPolygonGenerator
    {
        public List<PolygonModel> GeneratePolygon(string pythonPath, string pythonScriptPath, string polygonOutput, int numberOfVertices, int numberOfPolygons)
        {
            var polygons = PythonRunner.Generate(
                pythonPath,
                pythonScriptPath,
                polygonOutput,
                numberOfVertices,
                numberOfPolygons
                );

            if (polygons != null)
            {
                foreach (var polygon in polygons)
                {
                    polygon.Area = CalculatePolygonArea(polygon.Vertices);
                }
                return polygons;
            }

            return new List<PolygonModel>();
        }

        private double CalculatePolygonArea(List<PointModel> vertices)
        {
            int n = vertices.Count;
            double area = 0;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;  
                area += vertices[i].X * vertices[j].Y;
                area -= vertices[j].X * vertices[i].Y;
            }

            area = Math.Abs(area) / 2.0;
            return area;
        }
    }
}