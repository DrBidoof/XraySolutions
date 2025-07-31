using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrayModelTrainer.Models;

namespace XrayModelTrainer.Services
{
    internal class ModelTrainer
    {
        private readonly MLContext mlContext;

        public ModelTrainer()
        {
            mlContext = new MLContext();
        }

        public void Train()
        {
            Console.WriteLine("📁 Loading data...");
            var dataPath = Path.Combine("Data", "normal_images.csv");
            var imageData = mlContext.Data.LoadFromTextFile<ModelInput>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',');

            Console.WriteLine("🛠️ Building pipeline...");
            var pipeline = mlContext.Transforms.LoadImages(
                    outputColumnName: "input", imageFolder: "", inputColumnName: nameof(ModelInput.ImagePath))
                .Append(mlContext.Transforms.ResizeImages(
                    outputColumnName: "input", imageWidth: 224, imageHeight: 224))
                .Append(mlContext.Transforms.ExtractPixels("input"))
                .Append(mlContext.Clustering.Trainers.KMeans(
                    featureColumnName: "input", numberOfClusters: 5));

            Console.WriteLine("🧠 Training model...");
            var model = pipeline.Fit(imageData);

            var outputPath = Path.Combine("TrainedModels", "XrayModel.zip");

            if (File.Exists(outputPath))
            {
                Console.WriteLine($"⚠️  Model already exists at: {outputPath}");
                Console.Write("Do you want to overwrite it? (y/n): ");
                var response = Console.ReadLine()?.Trim().ToLower();

                if (response != "y" && response != "yes")
                {
                    Console.WriteLine("❌ Training aborted by user.");
                    return;
                }

                Console.WriteLine("🔁 Overwriting existing model...");
            }
            Directory.CreateDirectory("TrainedModels");
            mlContext.Model.Save(model, imageData.Schema, outputPath);

            Console.WriteLine($"✅ Model training complete! Saved to: {outputPath}");
        }
    }
}
