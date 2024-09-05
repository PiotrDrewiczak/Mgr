using Newtonsoft.Json;
using Poly2Tri;
using Poly2Tri.Triangulation.Polygon;
using PolyGenerator.Interfaces;
using PolyGenerator.Models;

namespace PolyGenerator
{
    public class Trangulation : ITriangulation
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

                triangulationModel.Area = CalculatePolygonArea(polygonModel.Vertices);

                // Convert points to Poly2Tri format
                var poly2TriPoints = new List<PolygonPoint>();
                foreach (var point in polygonModel.Vertices)
                {
                    poly2TriPoints.Add(new PolygonPoint(point.X, point.Y));
                }

                // Create polygon with points
                var polygon = new Polygon(poly2TriPoints);

                // Perform the initial triangulation
                P2T.Triangulate(polygon);


                foreach (var triangle in polygon.Triangles)
                {
                    var triangleModel = new TriangleModel
                    {
                        A = new PointModel(triangle.Points[0].X, triangle.Points[0].Y),
                        B = new PointModel(triangle.Points[1].X, triangle.Points[1].Y),
                        C = new PointModel(triangle.Points[2].X, triangle.Points[2].Y)
                    };

                    triangleModel.GetVertices();

                    triangulationModel.Triangles.Add(triangleModel);
                }

                // Add the triangulation model for this polygon to the master list
                allTriangulations.Add(triangulationModel);
            }

            // Optional: Save all triangulations to a file
            string filePath = "C:\\Users\\piotr\\Desktop\\dane2.json";
            SaveAllTriangulationsToJson(filePath, allTriangulations);

            return allTriangulations;
        }

        private void SaveAllTriangulationsToJson(string filePath, List<TriangulationModel> allTriangulations)
        {
            var json = JsonConvert.SerializeObject(allTriangulations, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public double CalculatePolygonArea(List<PointModel> vertices)
        {
            int n = vertices.Count;
            double area = 0;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n; // następny wierzchołek
                area += vertices[i].X * vertices[j].Y;
                area -= vertices[j].X * vertices[i].Y;
            }

            area = Math.Abs(area) / 2.0;
            return area;
        }
    }
}
