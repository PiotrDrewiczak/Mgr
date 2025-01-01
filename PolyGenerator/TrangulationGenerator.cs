using Poly2Tri;
using Poly2Tri.Triangulation.Polygon;
using PolyGenerator.Interfaces;
using PolyGenerator.Models;
using PolyGenerator.Models.Polygon;
using PolyGenerator.Models.Triangle;

namespace PolyGenerator
{
    public class TrangulationGenerator : ITriangulation
    {
        public List<TriangulationModel> GenerateTriangulation(PolygonModel[] polygons)
        {
            var allTriangulations = new List<TriangulationModel>();

            if (polygons == null || polygons.Length == 0)
            {
                Console.WriteLine("No polygons provided.");
                return allTriangulations;
            }

            foreach (var polygonModel in polygons)
            {
                var triangulationModel = new TriangulationModel();

                if (polygonModel.Vertices == null || polygonModel.Vertices.Count < 3)
                {
                    Console.WriteLine("Not enough vertices to form a triangulation for a polygon.");
                    continue;
                }

                var poly2TriPoints = new List<PolygonPoint>();
                foreach (var point in polygonModel.Vertices)
                {
                    poly2TriPoints.Add(new PolygonPoint(point.X, point.Y));
                }

                var polygon = new Polygon(poly2TriPoints);

                P2T.Triangulate(polygon);

                foreach (var triangle in polygon.Triangles)
                {
                    var triangleModel = new TriangleModel
                    (   new PointModel(triangle.Points[0].X, triangle.Points[0].Y),
                        new PointModel(triangle.Points[1].X, triangle.Points[1].Y),
                        new PointModel(triangle.Points[2].X, triangle.Points[2].Y)
                    );
                 
                    triangleModel.GetVertices();

                    triangulationModel.Triangles.Add(triangleModel);
                }

                allTriangulations.Add(triangulationModel);
            }

            return allTriangulations;
        }
    }
}
