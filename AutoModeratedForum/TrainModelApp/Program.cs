using System;
using System.IO;

class Program
{
    static void Main()
    {
        string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "comments.csv");

        string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "nas-bert-model.zip");

        ModelTrainer.TrainAndSaveModel(dataPath, modelPath);
    }
}
