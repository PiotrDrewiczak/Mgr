using Newtonsoft.Json;
using PolyGenerator.Models;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace PolyGenerator
{
    internal static class PythonRunner
    {
        public static List<PolygonModel>? Generate(string pythonPath, string pythonScriptPath, string polygonOutput, int numberOfVertices, int numberOfPolygons)
        {
            var start = new ProcessStartInfo()
            {
                FileName = pythonPath,
                Arguments = $"{pythonScriptPath} {numberOfPolygons} {numberOfVertices} \"{polygonOutput}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process? process = Process.Start(start))
            {
                using (StreamReader? reader = process?.StandardOutput)
                {
                    string? result = reader?.ReadToEnd();
                    Console.WriteLine(result);
                }

                string? error = process?.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("ERROR:");
                    Console.WriteLine(error);
                }

                process?.WaitForExit();
            }

            var polygons = new List<PolygonModel>();

            try
            {
                string json = File.ReadAllText(polygonOutput);
                dynamic? data = JsonConvert.DeserializeObject<dynamic>(json);

                List<List<List<double>>>? polygonPointsList = data?.polygons.ToObject<List<List<List<double>>>>();

                polygons = polygonPointsList?.Select(polygonPoints =>
                {
                    // Tworzenie listy wierzchołków dla wielokąta
                    var vertices = polygonPoints
                        .Select(pointList => new PointModel(pointList[0], pointList[1]))
                        .ToList();

                    // Tworzenie obiektu PolygonModel i obliczanie pola
                    var polygonModel = new PolygonModel
                    {
                        Vertices = vertices,
                        Area = 0
                    };

                    return polygonModel;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd przy odczycie pliku: {ex.Message}");
            }

            return polygons;
        }

        public static List<RectanglesModel> ProcessQuadrilaterals(string pythonPath, string pythonScriptPath, List<List<QuadrangulationModel>> allQuadrilateralCombinations)
        {
            var rectanglesModelList = new List<RectanglesModel>();

            var start = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = pythonScriptPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };

            foreach (var quadrangulation in allQuadrilateralCombinations)
            {
                RectanglesModel? bestRectangleModel = null;
                double maxArea = 0;

                foreach (var configuration in quadrangulation)
                {
                    // Przekształć dane czworokątów do formatu JSON
                    string quadrilateralJson = JsonConvert.SerializeObject(new
                    {
                        quadrilaterals = configuration.Quadrangles.Select(quad => new List<List<double>>
                          {
                          new List<double>{ quad.A.X, quad.A.Y },
                          new List<double>{ quad.B.X, quad.B.Y },
                          new List<double>{ quad.C.X, quad.C.Y },
                          new List<double>{ quad.D.X, quad.D.Y }}).ToList()
                    });

                    using (Process? process = Process.Start(start))
                    {
                        if (process != null)
                        {
                            using (StreamWriter writer = process.StandardInput)
                            {
                                writer.WriteLine(quadrilateralJson);
                            }

                            string result = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                            process.WaitForExit();

                            Console.WriteLine($"Result from Python: {result}");
                            Console.WriteLine($"Error from Python: {error}");

                            try
                            {
                                // Deserializacja wyników z Pythona
                                var data = JsonConvert.DeserializeObject<Dictionary<string, List<List<double>>>>(result ?? string.Empty);

                                if (data != null && data.ContainsKey("rectangles"))
                                {
                                    var rectanglesData = data["rectangles"];
                                    var rectangles = rectanglesData.Select(rect => new RectangleModel(
                                        rect[0],  // X
                                        rect[1],  // Y
                                        rect[2],  // Width
                                        rect[3]   // Height
                                    )).ToList();

                                    double totalArea = rectangles.Sum(r => r.Width * r.Height);

                                    if (totalArea > maxArea)
                                    {
                                        maxArea = totalArea;
                                        bestRectangleModel = new RectanglesModel { Rectangles = rectangles };
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Otrzymano pustą listę prostokątów.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Wystąpił błąd przy odczycie wyników: {ex.Message}");
                            }
                        }
                    }
                }

                if (bestRectangleModel != null)
                {
                    rectanglesModelList.Add(bestRectangleModel);
                }
            }

            return rectanglesModelList;
        }
    }
}

