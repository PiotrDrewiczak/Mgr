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
                var pairs = FindAllPairs(triangles);  // Znalezienie wszystkich par trójkątów
                var quadrangulations = new List<List<(QuadrangleModel, TriangleModel, TriangleModel)>>();

                // HashSet do przechowywania unikalnych konfiguracji
                var uniqueConfigurations = new HashSet<string>();

                // Generowanie wszystkich konfiguracji czworokątów
                GenerateAllConfigurations(pairs, new HashSet<TriangleModel>(), new List<(QuadrangleModel, TriangleModel, TriangleModel)>(), quadrangulations, uniqueConfigurations);

                // Dodawanie niesparowanych trójkątów do każdej konfiguracji
                var quadrangulationModels = quadrangulations.Select(qList => new QuadrangulationModel
                {
                    Quadrangles = qList.Select(q => q.Item1).ToList(), // Przekształcamy listę czworokątów
                    UnpairedTriangles = triangles.Except(qList.SelectMany(q => new List<TriangleModel> { q.Item2, q.Item3 })).ToList()  // Znajdujemy niesparowane trójkąty
                }).ToList();

                allQuadrangulations.Add(quadrangulationModels);
            }

            return allQuadrangulations;
        }

        private void GenerateAllConfigurations(List<(TriangleModel, TriangleModel)> pairs,
                                               HashSet<TriangleModel> usedTriangles,
                                               List<(QuadrangleModel, TriangleModel, TriangleModel)> currentQuadrangulation,
                                               List<List<(QuadrangleModel, TriangleModel, TriangleModel)>> allQuadrangulations,
                                               HashSet<string> uniqueConfigurations)
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
                        currentQuadrangulation.Add((quadrangle, t1, t2));
                        usedTriangles.Add(t1);
                        usedTriangles.Add(t2);
                        addedNew = true;

                        // Rekurencja - generowanie kolejnych konfiguracji
                        GenerateAllConfigurations(pairs, usedTriangles, currentQuadrangulation, allQuadrangulations, uniqueConfigurations);

                        // Cofnięcie ostatniego kroku (backtracking)
                        currentQuadrangulation.RemoveAt(currentQuadrangulation.Count - 1);
                        usedTriangles.Remove(t1);
                        usedTriangles.Remove(t2);
                    }
                }
            }

            // Jeśli nie dodano nowych czworokątów, zapisz bieżącą konfigurację
            if (!addedNew && currentQuadrangulation.Count > 0)
            {
                // Tworzymy klucz na podstawie wierzchołków czworokątów (sortowanie czworokątów i ich wierzchołków)
                var configurationKey = GenerateConfigurationKey(currentQuadrangulation.Select(q => q.Item1).ToList());

                // Sprawdzamy unikalność konfiguracji
                if (!uniqueConfigurations.Contains(configurationKey))
                {
                    uniqueConfigurations.Add(configurationKey);
                    allQuadrangulations.Add(new List<(QuadrangleModel, TriangleModel, TriangleModel)>(currentQuadrangulation));
                }
            }
        }

        // Tworzenie unikalnego klucza na podstawie całej konfiguracji czworokątów
        private string GenerateConfigurationKey(List<QuadrangleModel> quadrangles)
        {
            var sortedQuadrangles = quadrangles.Select(q =>
            {
                var vertices = new List<PointModel> { q.A, q.B, q.C, q.D };
                // Sortujemy wierzchołki według współrzędnych
                vertices.Sort((p1, p2) =>
                {
                    if (p1.X == p2.X)
                        return p1.Y.CompareTo(p2.Y);
                    return p1.X.CompareTo(p2.X);
                });
                return vertices;
            }).OrderBy(q => q.First().X).ThenBy(q => q.First().Y); // Sortowanie czworokątów po pierwszym wierzchołku

            // Tworzymy unikalny klucz na podstawie uporządkowanych wierzchołków czworokątów
            return string.Join("|", sortedQuadrangles.Select(v => string.Join(",", v.Select(p => $"{p.X},{p.Y}"))));
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
                        pairs.Add((triangles[i], triangles[j]));  // Dodajemy pary trójkątów z dwoma wspólnymi wierzchołkami
                    }
                }
            }

            return pairs;
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
                    uniqueVerticesT1[0],  // Wierzchołek z t1
                    sharedVertices[0],    // Wspólny wierzchołek
                    uniqueVerticesT2[0],  // Wierzchołek z t2
                    sharedVertices[1]     // Wspólny wierzchołek
                );
            }

            return null;  // Nie można utworzyć czworokąta
        }
    }
    }