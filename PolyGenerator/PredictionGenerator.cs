using MLModel.Models;
using MLModel.TrainingModel;
using PolyGenerator.Interfaces;
using PolyGenerator.Models;
using PolyGenerator.Models.Polygon;



namespace PolyGenerator
{
    public class PredictionGenerator : IPrediction
    {
        public List<PolygonWithCenterModel> CalculateCenter(PolygonModel[] polygons, string pythonPath, string scriptPath, string modelPath)
        {
            if (polygons == null || polygons.Length == 0)
            {
                return new List<PolygonWithCenterModel>();
            }

            return polygons.Select(polygon =>
            {
                var normalizedPolygon = NormalizePolygon(polygon);

                var input = new PolygonInput
                {
                    Features = ConvertToFeatures(normalizedPolygon),
                    CenterX = 0,
                    CenterY = 0
                };

                var predictionResult = PythonLightGbm.Prediction(input, pythonPath, scriptPath, modelPath);

                return new PolygonWithCenterModel
                {
                    Vertices = polygon.Vertices,
                    X = predictionResult.CenterX,
                    Y = predictionResult.CenterY
                };
            }).ToList();
        }

        private PolygonModel NormalizePolygon(PolygonModel polygon)
        {
            if (polygon == null || polygon.Vertices.Count == 0)
                throw new ArgumentException("Polygon must have vertices.");

            double minX = polygon.Vertices.Min(v => v.X);
            double minY = polygon.Vertices.Min(v => v.Y);
            double maxX = polygon.Vertices.Max(v => v.X);
            double maxY = polygon.Vertices.Max(v => v.Y);

            return new PolygonModel
            {
                Vertices = polygon.Vertices.Select(v => new PointModel(
                    (v.X - minX) / (maxX - minX),
                    (v.Y - minY) / (maxY - minY))
                ).ToList(),
                Area = polygon.Area
            };
        }

        private float[] ConvertToFeatures(PolygonModel polygon)
        {
            var features = new List<float>();

            foreach (var vertex in polygon.Vertices)
            {
                features.Add((float)vertex.X);
                features.Add((float)vertex.Y);
            }

            while (features.Count < 16)
            {
                features.Add(0f);
            }

            return features.Take(16).ToArray();
        }
    }
}
