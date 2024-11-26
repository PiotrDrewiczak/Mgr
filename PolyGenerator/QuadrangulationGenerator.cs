using PolyGenerator.Interfaces;
using PolyGenerator.Models;
using PolyGenerator.Models.Quad;
using PolyGenerator.Models.Triangle;

namespace PolyGenerator.Scripts
{
    public class QuadrangulationGenerator : IQuadrangulation
    {
            public List<List<QuadrangulationModel>> GenerateQuadrangulations(List<TriangulationModel> triangulations)
            {
                return triangulations.Select(GenerateQuadrangulationsForSingleTriangulation).ToList();
            }

            private List<QuadrangulationModel> GenerateQuadrangulationsForSingleTriangulation(TriangulationModel triangulation)
            {
                var triangles = triangulation.Triangles;
                var pairs = FindTrianglePairs(triangles);
                var quadrangulations = GenerateUniqueQuadrangulations(pairs, triangles);

                return quadrangulations.Select(q => new QuadrangulationModel
                {
                    Quadrangles = q.Select(quadrangle => quadrangle.Quadrangle).ToList(),
                    UnpairedTriangles = GetUnpairedTriangles(triangles, q)
                }).ToList();
            }

            private List<List<Quadrangulation>> GenerateUniqueQuadrangulations(
                List<(TriangleModel, TriangleModel)> pairs,
                List<TriangleModel> allTriangles)
            {
                var uniqueConfigurations = new HashSet<string>();
                var result = new List<List<Quadrangulation>>();

                var stack = new Stack<State>();
                stack.Push(new State(new HashSet<TriangleModel>(), new List<Quadrangulation>(), 0));

                while (stack.Count > 0)
                {
                    var state = stack.Pop();

                    if (state.CurrentPairIndex >= pairs.Count)
                    {
                        var configurationKey = GenerateConfigurationKey(state.CurrentQuadrangulation);
                        if (uniqueConfigurations.Add(configurationKey))
                        {
                            result.Add(new List<Quadrangulation>(state.CurrentQuadrangulation));
                        }
                        continue;
                    }

                    var (t1, t2) = pairs[state.CurrentPairIndex];
                    stack.Push(new State(state.UsedTriangles, state.CurrentQuadrangulation, state.CurrentPairIndex + 1));

                    if (!state.UsedTriangles.Contains(t1) && !state.UsedTriangles.Contains(t2))
                    {
                        var quadrangle = CreateQuadrilateral(t1, t2);
                        if (quadrangle != null)
                        {
                            var newUsedTriangles = new HashSet<TriangleModel>(state.UsedTriangles) { t1, t2 };
                            var newConfiguration = new List<Quadrangulation>(state.CurrentQuadrangulation)
                    {
                        new Quadrangulation(quadrangle, t1, t2)
                    };
                            stack.Push(new State(newUsedTriangles, newConfiguration, state.CurrentPairIndex + 1));
                        }
                    }
                }

                return result;
            }

            private List<(TriangleModel, TriangleModel)> FindTrianglePairs(List<TriangleModel> triangles)
            {
                return triangles.SelectMany((t1, i) => triangles.Skip(i + 1)
                    .Where(t2 => HaveTwoSharedVertices(t1, t2))
                    .Select(t2 => (t1, t2)))
                    .ToList();
            }

            private bool HaveTwoSharedVertices(TriangleModel t1, TriangleModel t2)
            {
                return GetSharedVertices(t1, t2).Count == 2;
            }

            private List<PointModel> GetSharedVertices(TriangleModel t1, TriangleModel t2)
            {
                return t1.GetVertices()
                    .Intersect(t2.GetVertices())
                    .ToList();
            }

            private QuadrangleModel? CreateQuadrilateral(TriangleModel t1, TriangleModel t2)
            {
                var sharedVertices = GetSharedVertices(t1, t2);
                if (sharedVertices.Count != 2) return null;

                var uniqueVerticesT1 = t1.GetVertices().Except(sharedVertices).ToList();
                var uniqueVerticesT2 = t2.GetVertices().Except(sharedVertices).ToList();

                return uniqueVerticesT1.Count == 1 && uniqueVerticesT2.Count == 1
                    ? new QuadrangleModel(uniqueVerticesT1[0], sharedVertices[0], uniqueVerticesT2[0], sharedVertices[1])
                    : null;
            }

            private string GenerateConfigurationKey(List<Quadrangulation> configuration)
            {
                return string.Join("|", configuration
                    .Select(q => q.Quadrangle.GetVertices()
                        .OrderBy(v => v.X)
                        .ThenBy(v => v.Y)
                        .Select(v => $"{v.X},{v.Y}"))
                    .Select(v => string.Join(",", v)));
            }

            private List<TriangleModel> GetUnpairedTriangles(List<TriangleModel> allTriangles, List<Quadrangulation> quadrangulation)
            {
                var usedTriangles = quadrangulation.SelectMany(q => new[] { q.Triangle1, q.Triangle2 });
                return allTriangles.Except(usedTriangles).ToList();
            }

            private record Quadrangulation(QuadrangleModel Quadrangle, TriangleModel Triangle1, TriangleModel Triangle2);

            private class State
            {
                public HashSet<TriangleModel> UsedTriangles { get; }
                public List<Quadrangulation> CurrentQuadrangulation { get; }
                public int CurrentPairIndex { get; }

                public State(HashSet<TriangleModel> usedTriangles, List<Quadrangulation> currentQuadrangulation, int currentPairIndex)
                {
                    UsedTriangles = usedTriangles;
                    CurrentQuadrangulation = currentQuadrangulation;
                    CurrentPairIndex = currentPairIndex;
                }
            }
        }
    }