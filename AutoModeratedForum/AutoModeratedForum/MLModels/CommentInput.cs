namespace AutoModeratedForum.MLModels
{
    public class CommentInput
    {
        public string Text { get; set; } = null!;
        public bool Label { get; set; }
    }
}
