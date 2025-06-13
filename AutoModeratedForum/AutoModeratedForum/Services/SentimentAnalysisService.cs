using AutoModeratedForum.MLModels;
using Microsoft.ML;
using System.IO;

namespace AutoModeratedForum.Services
{
    public class SentimentAnalysisService
    {
        private readonly PredictionEngine<CommentInput, CommentPrediction> _predictionEngine;

        public SentimentAnalysisService(MLContext mlContext, string modelPath)
        {
            if (!File.Exists(modelPath))
                throw new FileNotFoundException("ML model not found at: " + modelPath);

            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            _predictionEngine = mlContext.Model.CreatePredictionEngine<CommentInput, CommentPrediction>(mlModel);
        }

        public CommentPrediction Predict(string commentText)
        {
            var input = new CommentInput { Text = commentText };
            return _predictionEngine.Predict(input);
        }
    }
}
