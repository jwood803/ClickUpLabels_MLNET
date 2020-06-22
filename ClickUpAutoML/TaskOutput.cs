using Microsoft.ML.Data;

namespace ClickUpAutoML
{
    public class TaskOutput
    {
        [ColumnName("PredictedLabel")]
        public string Prediction { get; set; }
        public float[] Score { get; set; }
    }
}