namespace AutoModeratedForum.MLModels
{
    public class CommentPrediction
    {
        public bool isRude { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
        public bool PredictedLabel { get; set; }
    }
}
