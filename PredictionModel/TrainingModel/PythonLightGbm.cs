using Newtonsoft.Json;
using PredictionModel.Models;
using System.Diagnostics;

namespace PredictionModel.TrainingModel
{
    public class PythonLightGbm
    {
        public static void TrainModel(List<PolygonInput> data, string pythonPath, string pythonScriptPath, string modelOutputPath)
        {
            // Zapisanie danych do tymczasowego pliku JSON
            string tempJsonFile = Path.GetTempFileName();
            File.WriteAllText(tempJsonFile, JsonConvert.SerializeObject(data));

            // Konfiguracja procesu
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
                // Rejestracja zdarzeń do odczytywania wyjścia i błędów
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

                // Rozpoczęcie asynchronicznego odczytywania wyjścia i błędów
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Oczekiwanie na zakończenie procesu
                process.WaitForExit();
            }

            // Usunięcie tymczasowego pliku JSON po zakończeniu pracy
            File.Delete(tempJsonFile);
        }
        public static PolygonInput Prediction(PolygonInput inputData, string pythonPath, string scriptPath, string modelPath)
        {
            string tempInputJson = Path.GetTempFileName(); // Plik tymczasowy na dane wejściowe
            string tempOutputJson = Path.GetTempFileName(); // Plik tymczasowy na dane wyjściowe

            try
            {
                // Zapisanie danych wejściowych do pliku JSON
                File.WriteAllText(tempInputJson, JsonConvert.SerializeObject(inputData));

                // Konfiguracja procesu
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
                    // Czytanie błędów z konsoli
                    string? error = process?.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception($"Błąd w skrypcie Python: {error}");
                    }

                    process?.WaitForExit();

                    // Sprawdzenie, czy proces zakończył się sukcesem
                    if (process?.ExitCode != 0)
                    {
                        throw new Exception("Proces Python zakończył się błędem.");
                    }
                }

                // Wczytanie wynikowego pliku JSON z predykcjami
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
                // Czyszczenie plików tymczasowych
                if (File.Exists(tempInputJson))
                    File.Delete(tempInputJson);
                if (File.Exists(tempOutputJson))
                    File.Delete(tempOutputJson);
            }
        }

    }
}
