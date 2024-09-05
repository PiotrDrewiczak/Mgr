using PolyGenerator.Interfaces;
using PolyGenerator.Models;

namespace PolyGenerator.Scripts
{
    public class Quadrulation : IQuadrulation
    {
        public List<List<QuadrangulationModel>> GenerateQuadrangulations(List<TriangulationModel> triangulations)
        {
            var allQuadrangulations = new List<List<QuadrangulationModel>>();

            foreach (var triangulation in triangulations)
            {
                var triangles = triangulation.Triangles;
                var pairs = FindAllPairs(triangles);
                var quadrangulations = new List<List<QuadrangleModel>>();

                GenerateAllConfigurations(pairs, new HashSet<TriangleModel>(), new List<QuadrangleModel>(), quadrangulations);

                var quadrangulationModels = quadrangulations.Select(qList => new QuadrangulationModel { Quadrangles = qList }).ToList();
                allQuadrangulations.Add(quadrangulationModels);
            }

            return allQuadrangulations;
        }

        private List<(TriangleModel, TriangleModel)> FindAllPairs(List<TriangleModel> triangles)
        {
            var pairs = new List<(TriangleModel, TriangleModel)>();

            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = i + 1; j < triangles.Count; j++)
                {
                    var sharedVertices = GetSharedVertices(triangles[i], triangles[j]);
                    if (sharedVertices.Count == 2)
                    {
                        pairs.Add((triangles[i], triangles[j]));
                    }
                }
            }

            return pairs;
        }

        private void GenerateAllConfigurations(List<(TriangleModel, TriangleModel)> pairs,
                                               HashSet<TriangleModel> usedTriangles,
                                               List<QuadrangleModel> currentQuadrangulation,
                                               List<List<QuadrangleModel>> allQuadrangulations)
        {
            bool addedNew = false;

            for (int i = 0; i < pairs.Count; i++)
            {
                var (t1, t2) = pairs[i];

                if (!usedTriangles.Contains(t1) && !usedTriangles.Contains(t2))
                {
                    var sharedVertices = GetSharedVertices(t1, t2);
                    var quadrangle = CreateQuadrilateral(t1, t2, sharedVertices);

                    if (quadrangle != null)
                    {
                        currentQuadrangulation.Add(quadrangle);
                        usedTriangles.Add(t1);
                        usedTriangles.Add(t2);
                        addedNew = true;

                        GenerateAllConfigurations(pairs, usedTriangles, currentQuadrangulation, allQuadrangulations);

                        currentQuadrangulation.RemoveAt(currentQuadrangulation.Count - 1);
                        usedTriangles.Remove(t1);
                        usedTriangles.Remove(t2);
                    }
                }
            }

            if (!addedNew && currentQuadrangulation.Count > 0)
            {
                allQuadrangulations.Add(new List<QuadrangleModel>(currentQuadrangulation));
            }
        }

        private List<PointModel> GetSharedVertices(TriangleModel t1, TriangleModel t2)
        {
            var vertices1 = t1.GetVertices();
            var vertices2 = t2.GetVertices();
            return vertices1.Where(v1 => vertices2.Any(v2 => v2.X == v1.X && v2.Y == v1.Y)).ToList();
        }

        private QuadrangleModel CreateQuadrilateral(TriangleModel t1, TriangleModel t2, List<PointModel> sharedVertices)
        {
            var uniqueVerticesT1 = t1.GetVertices().Except(sharedVertices).ToList();
            var uniqueVerticesT2 = t2.GetVertices().Except(sharedVertices).ToList();

            if (uniqueVerticesT1.Count == 1 && uniqueVerticesT2.Count == 1)
            {
                return new QuadrangleModel(
                    uniqueVerticesT1[0],
                    sharedVertices[0],
                    uniqueVerticesT2[0],
                    sharedVertices[1]
                );
            }

            throw new InvalidOperationException("Nie można stworzyć czworokąta z podanych trójkątów.");
        }
    }
}
