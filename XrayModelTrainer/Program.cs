using Microsoft.ML;
using XrayModelTrainer.Models;
using XrayModelTrainer.Services;

namespace XrayModelTrainer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var trainer = new ModelTrainer();
            trainer.Train();

            //models are placed in bin folder
        }
    }
}
