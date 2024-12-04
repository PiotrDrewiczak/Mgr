using Newtonsoft.Json;
using MLModel.Models;
using System.Diagnostics;

namespace MLModel.TrainingModel
{
    public class PythonLightGbm
    {
        public static void TrainModel(List<PolygonInput> data, string pythonPath, string pythonScriptPath, string modelOutputPath)
        {
            string tempJsonFile = Path.GetTempFileName();
            File.WriteAllText(tempJsonFile, JsonConvert.SerializeObject(data));

            var start = new ProcessStartInfo()
            {
                FileName = pythonPath,
                Arguments = $"{pythonScriptPath} \"{tempJsonFile}\" \"{modelOutputPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(start))
            {
                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine("PYTHON OUTPUT: " + args.Data);
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine("PYTHON ERROR: " + args.Data);
                    }
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }

            File.Delete(tempJsonFile);
        }
        public static PolygonInput Prediction(PolygonInput inputData, string pythonPath, string scriptPath, string modelPath)
        {
            string tempInputJson = Path.GetTempFileName();
            string tempOutputJson = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempInputJson, JsonConvert.SerializeObject(inputData));

                var start = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"{scriptPath} \"{modelPath}\" \"{tempInputJson}\" \"{tempOutputJson}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process? process = Process.Start(start))
                {
                    string? error = process?.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception($"Błąd w skrypcie Python: {error}");
                    }

                    process?.WaitForExit();

                    if (process?.ExitCode != 0)
                    {
                        throw new Exception("Proces Python zakończył się błędem.");
                    }
                }

                if (File.Exists(tempOutputJson))
                {
                    string outputJson = File.ReadAllText(tempOutputJson);
                    var result = JsonConvert.DeserializeObject<PolygonInput>(outputJson);
                    if (result == null)
                        throw new Exception("Nie udało się odczytać wynikowych danych JSON.");
                    return result;
                }
                else
                {
                    throw new FileNotFoundException("Plik wynikowy JSON nie został wygenerowany.");
                }
            }
            finally
            {
                if (File.Exists(tempInputJson))
                    File.Delete(tempInputJson);
                if (File.Exists(tempOutputJson))
                    File.Delete(tempOutputJson);
            }
        }

    }
}
