using Microsoft.Extensions.Configuration;
using PredictionModel.Models.Config;
using PredictionModel.TrainingModel;

namespace PredictionModel
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var modelsSettings = configuration.GetSection("ModelsSettings").Get<ModelsSettings>();
            var pythonSettings = configuration.GetSection("PythonSettings").Get<PythonSettings>(); 
            var dataFileSettings = configuration.GetSection("DataFileSettings").Get<DataFileSettings>();


            //PythonLightGbmTraining(dataFileSettings,modelsSettings,pythonSettings);
            PythonLightGbmPrediction(dataFileSettings, modelsSettings, pythonSettings);
        }
        static void PythonLightGbmTraining(DataFileSettings dataFileSettings,ModelsSettings modelSettings,PythonSettings pythonSettings)
        {
            var filePath = dataFileSettings.TrainingExcelPath;
            var data = PolygonDataLoader.LoadDataWithCenters(filePath);


            PythonLightGbm.TrainModel
                (data, 
                pythonSettings.PythonPath, 
                pythonSettings.ScriptPathLightGBM, 
                modelSettings.PythonModelPath);
        }

        static void PythonLightGbmPrediction(DataFileSettings dataFileSettings, ModelsSettings modelSettings, PythonSettings pythonSettings)
        {
            var filePath = dataFileSettings.PredictionExcelPath;
            var data = PolygonDataLoader.LoadDataWithoutCenters(filePath);
           
           var result = PythonLightGbm.Prediction
                (data[0], 
                pythonSettings.PythonPath, 
                pythonSettings.ScriptPathPrediction, 
                modelSettings.PythonModelPath);
        }
       
    }
 }
