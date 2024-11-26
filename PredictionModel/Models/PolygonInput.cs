using Microsoft.ML.Data;
namespace PredictionModel.Models
{
    public class PolygonInput
    {
        [ColumnName("Features")]
        [LoadColumn(0, 15)]
        [VectorType(16)]
        public float[] Features { get; set; }

        [LoadColumn(16)]
        public float CenterX { get; set; }

        [LoadColumn(17)]
        public float CenterY { get; set; }
    }
}
