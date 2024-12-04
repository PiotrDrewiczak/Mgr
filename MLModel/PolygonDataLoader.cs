using OfficeOpenXml;
using MLModel.Models;

namespace MLModel
{
    public class PolygonDataLoader
    {
        public static List<PolygonInput> LoadDataWithCenters(string filePath)
        {
            return LoadData(filePath, true);
        }

        public static List<PolygonInput> LoadDataWithoutCenters(string filePath)
        {
            return LoadData(filePath, false);
        }

        private static List<PolygonInput> LoadData(string filePath, bool includeCenters)
        {
            var polygons = new List<PolygonInput>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var vertices = new float[16];
                    for (int i = 0; i < 16; i++)
                    {
                        vertices[i] = float.Parse(worksheet.Cells[row, i + 1].Text);
                    }

                    float? centerX = null;
                    float? centerY = null;

                    if (includeCenters)
                    {
                        centerX = float.Parse(worksheet.Cells[row, 17].Text);
                        centerY = float.Parse(worksheet.Cells[row, 18].Text);
                    }

                    polygons.Add(new PolygonInput
                    {
                        Features = vertices,
                        CenterX = centerX ?? 0,
                        CenterY = centerY ?? 0
                    });
                }
            }

            return polygons;
        }
    }
}
