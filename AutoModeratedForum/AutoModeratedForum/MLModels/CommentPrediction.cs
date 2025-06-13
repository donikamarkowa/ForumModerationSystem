namespace AutoModeratedForum.MLModels
{
    public class CommentPrediction
    {
        public bool PredictedLabel { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
