using ClincalWorkFlow.MLModels;
using Microsoft.ML;

namespace ClincalWorkFlow.Services
{
    public class PredictionEngineService
    {
        private readonly MLContext mlContext;
        private readonly PredictionEngine<ModelInput, ModelOutput> predictionEngine;

        public PredictionEngineService()
        {
            mlContext = new MLContext();

            var modelPath = Path.Combine("MLModels", "XrayModel.zip");
            var model = mlContext.Model.Load(modelPath, out _);

            predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
        }

        public ModelOutput Predict(string imagePath)
        {
            var input = new ModelInput { ImagePath = imagePath };
            return predictionEngine.Predict(input);
        }
    }
}
