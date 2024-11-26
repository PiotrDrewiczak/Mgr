using Microsoft.ML;
using Microsoft.ML.AutoML;
using PredictionModel.Models;

namespace PredictionModel.TrainingModel
{
    public class LightGbm
    {
        private readonly MLContext mlContext;

        public LightGbm()
        {
            mlContext = new MLContext();
        }

        public void TrainModel(List<PolygonInput> data, string modelPathX, string modelPathY)
        {
            var trainingDataView = mlContext.Data.LoadFromEnumerable(data);

            var experimentSettings = new RegressionExperimentSettings
            {
                MaxExperimentTimeInSeconds = 3600,
                Trainers = { RegressionTrainer.LightGbm }
            };

            Console.WriteLine("Rozpoczęcie AutoML dla CenterX...");
            var experimentX = mlContext.Auto().CreateRegressionExperiment(experimentSettings);
            var resultX = experimentX.Execute(trainingDataView, labelColumnName: nameof(PolygonInput.CenterX));

            Console.WriteLine(resultX.BestRun.ValidationMetrics.RSquared);
            Console.WriteLine(resultX.BestRun.ValidationMetrics.MeanSquaredError);

            var bestModelX = resultX.BestRun.Model;

            Console.WriteLine("Rozpoczęcie kroswalidacji dla CenterX...");
            var pipelineX = resultX.BestRun.Estimator;

            var pipelineWithLabelX = mlContext.Transforms.CopyColumns("Label", nameof(PolygonInput.CenterX))
                                        .Append(pipelineX);

            var cvResultsX = mlContext.Regression.CrossValidate(trainingDataView, pipelineWithLabelX, numberOfFolds: 5);

            double avgRSquaredX = 0;
            double avgMAEX = 0;
            foreach (var result in cvResultsX)
            {
                Console.WriteLine($"Fold R-Squared dla CenterX: {result.Metrics.RSquared}");
                Console.WriteLine($"Fold MAE dla CenterX: {result.Metrics.MeanAbsoluteError}");
                avgRSquaredX += result.Metrics.RSquared;
                avgMAEX += result.Metrics.MeanAbsoluteError;
            }
            avgRSquaredX /= cvResultsX.Count;
            avgMAEX /= cvResultsX.Count;

            Console.WriteLine($"Średni R-Squared dla CenterX: {avgRSquaredX}, Średni MAE dla CenterX: {avgMAEX}");

            mlContext.Model.Save(bestModelX, trainingDataView.Schema, modelPathX);

            Console.WriteLine("Rozpoczęcie AutoML dla CenterY...");
            var experimentY = mlContext.Auto().CreateRegressionExperiment(experimentSettings);
            var resultY = experimentY.Execute(trainingDataView, labelColumnName: nameof(PolygonInput.CenterY));

            Console.WriteLine(resultY.BestRun.ValidationMetrics.RSquared);
            Console.WriteLine(resultY.BestRun.ValidationMetrics.MeanSquaredError);

            var bestModelY = resultY.BestRun.Model;

            Console.WriteLine("Rozpoczęcie kroswalidacji dla CenterY...");
            var pipelineY = resultY.BestRun.Estimator;

            var pipelineWithLabelY = mlContext.Transforms.CopyColumns("Label", nameof(PolygonInput.CenterY))
                                        .Append(pipelineY);

            var cvResultsY = mlContext.Regression.CrossValidate(trainingDataView, pipelineWithLabelY, numberOfFolds: 5);

            double avgRSquaredY = 0;
            double avgMAEY = 0;
            foreach (var result in cvResultsY)
            {
                Console.WriteLine($"Fold R-Squared dla CenterY: {result.Metrics.RSquared}");
                Console.WriteLine($"Fold MAE dla CenterY: {result.Metrics.MeanAbsoluteError}");
                avgRSquaredY += result.Metrics.RSquared;
                avgMAEY += result.Metrics.MeanAbsoluteError;
            }
            avgRSquaredY /= cvResultsY.Count;
            avgMAEY /= cvResultsY.Count;

            Console.WriteLine($"Średni R-Squared dla CenterY: {avgRSquaredY}, Średni MAE dla CenterY: {avgMAEY}");

            mlContext.Model.Save(bestModelY, trainingDataView.Schema, modelPathY);
        }
    }
}
