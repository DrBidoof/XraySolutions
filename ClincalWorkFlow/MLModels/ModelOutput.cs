using Microsoft.ML.Data;

namespace ClincalWorkFlow.MLModels
{
    public class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId { get; set; }

        public float[] Score { get; set; } // Distance from each cluster centroid
    }
}
