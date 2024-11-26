using OfficeOpenXml;
using PolyGenerator.Interfaces;
using PolyGenerator.Models;

namespace PolyGenerator
{
    public class ExcelGenerator : IExcelGenerator
    {
        public string SaveToExcel(ExportDataModel exportData, string excelPath)
        {
            // Ustawienie kontekstu licencji (jeśli nie było wcześniej ustawione)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                // Sprawdzamy, czy plik już istnieje
                bool fileExists = File.Exists(excelPath);

                // Użyj 'using', aby upewnić się, że plik zostanie prawidłowo zamknięty
                using (var package = new ExcelPackage())
                {
                    if (fileExists)
                    {
                        // Otwórz istniejący plik, aby dodać nowe dane
                        var existingFile = new FileInfo(excelPath);
                        using (var stream = existingFile.OpenRead())
                        {
                            package.Load(stream); // Wczytaj istniejący plik Excel
                        }
                    }

                    // Pobierz lub dodaj arkusz
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Polygons and Rectangles");

                    // Znajdź pierwszy wolny wiersz
                    int row = worksheet.Dimension?.Rows + 1 ?? 1;

                    // Jeśli plik nie istniał, dodajemy nagłówki
                    if (row == 1)
                    {
                        var polygonVerticesCount = exportData.Polygons.FirstOrDefault()?.Vertices.Count ?? 0;

                        // Dodajemy nagłówki na podstawie liczby wierzchołków
                        for (int i = 0; i < polygonVerticesCount; i++)
                        {
                            worksheet.Cells[1, 1 + i * 2].Value = $"x{i + 1}";
                            worksheet.Cells[1, 2 + i * 2].Value = $"y{i + 1}";
                        }

                        // Dodanie nagłówków dla środka prostokąta
                        worksheet.Cells[1, polygonVerticesCount * 2 + 1].Value = "sx1";  // środek X
                        worksheet.Cells[1, polygonVerticesCount * 2 + 2].Value = "sy1";  // środek Y

                        row++; // Następny wiersz do wpisania danych
                    }

                    // Dodawanie danych
                    for (int i = 0; i < exportData.Polygons.Count; i++)
                    {
                        var polygon = exportData.Polygons[i];
                        var rectangleCenter = exportData.RectangleCenters.ElementAtOrDefault(i);

                        // Wpisujemy dane wierzchołków wielokątów
                        for (int j = 0; j < polygon.Vertices.Count; j++)
                        {
                            worksheet.Cells[row, 1 + j * 2].Value = polygon.Vertices[j].X;
                            worksheet.Cells[row, 2 + j * 2].Value = polygon.Vertices[j].Y;
                        }

                        // Wpisujemy dane o centrum największego prostokąta
                        if (rectangleCenter != null)
                        {
                            int rectangleColumnStart = polygon.Vertices.Count * 2 + 1;
                            worksheet.Cells[row, rectangleColumnStart].Value = rectangleCenter.CenterX;
                            worksheet.Cells[row, rectangleColumnStart + 1].Value = rectangleCenter.CenterY;
                        }

                        row++; // Przejdź do kolejnego wiersza
                    }

                    // Zapisz plik, jeśli nie istniał, lub nadpisz istniejący plik z nowymi danymi
                    var fileInfo = new FileInfo(excelPath);
                    package.SaveAs(fileInfo); // Zapisz zmiany do pliku
                }
            }
            catch (IOException ioEx)
            {
                // Obsługa wyjątków związanych z plikami
                Console.WriteLine($"IO Exception: {ioEx.Message}");
                throw; // Możesz obsłużyć błąd lub rzucić dalej, aby poinformować użytkownika
            }
            catch (Exception ex)
            {
                // Inna ogólna obsługa wyjątków
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }

            return excelPath; // Zwracamy ścieżkę do pliku
        }
    }
}
