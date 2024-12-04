using Microsoft.Extensions.Configuration;
using MLModel.Models.Config;
using MLModel.TrainingModel;

namespace MLModel
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


            PythonLightGbmTraining(dataFileSettings, modelsSettings, pythonSettings);
            LightGbmTraining(dataFileSettings, modelsSettings);
        }
        static void PythonLightGbmTraining(DataFileSettings dataFileSettings, ModelsSettings modelSettings, PythonSettings pythonSettings)
        {
            var filePath = dataFileSettings.TrainingExcelPath;
            var data = PolygonDataLoader.LoadDataWithCenters(filePath);


            PythonLightGbm.TrainModel
                (data,
                pythonSettings.PythonPath,
                pythonSettings.ScriptPathLightGBM,
                modelSettings.PythonModelPath);
        }
        static void LightGbmTraining(DataFileSettings dataFileSettings, ModelsSettings modelSettings)
        {
            var filePath = dataFileSettings.TrainingExcelPath;
            var data = PolygonDataLoader.LoadDataWithCenters(filePath);

            var lightGbm = new LightGbm();
            lightGbm.TrainModel(data, modelSettings.ModelXPath, modelSettings.ModelYPath);
        }
    }
}
