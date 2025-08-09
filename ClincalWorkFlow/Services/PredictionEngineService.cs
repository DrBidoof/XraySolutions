using ClincalWorkFlow.MLModels;
using Microsoft.ML;

namespace ClincalWorkFlow.Services
{
    public class PredictionEngineService
    {
        private readonly MLContext _ml;
        private readonly PredictionEngine<ModelInput, ModelOutput> _engine;
        private readonly object _lock = new();

        public PredictionEngineService()
        {
            _ml = new MLContext();

            var modelPath = Path.Combine(AppContext.BaseDirectory, "MLModels", "XrayModel.zip");
            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"ML model not found at {modelPath}");

            var model = _ml.Model.Load(modelPath, out _);
            _engine = _ml.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
        }

        public ModelOutput Predict(string imagePath)
        {
            var input = new ModelInput { ImagePath = imagePath };
            lock (_lock)
            {
                return _engine.Predict(input);
            }
        }
    }
}
