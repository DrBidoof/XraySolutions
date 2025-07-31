using Microsoft.ML.Data;

namespace ClincalWorkFlow.MLModels
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public string ImagePath { get; set; }
    }
}
