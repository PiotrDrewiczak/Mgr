namespace MLModel.Models.Config
{
    public class PythonSettings
    {
        public required string PythonPath { get; set; }
        public required string ScriptPathLightGBM { get; set; }
        public required string ScriptPathPrediction { get; set; }

    }
}
