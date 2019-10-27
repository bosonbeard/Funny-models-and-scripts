using System;
using System.IO;
using Microsoft.ML;

namespace app_to_mayor_mlnet
{
    class Program
    {

        static readonly string _trainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "train_data.csv");
        static readonly string _testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "test_data.csv");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");

        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext(seed: 0);
            var model = Train(mlContext, _trainDataPath);
            Evaluate(mlContext, model);
            TestSinglePrediction(mlContext, model);
        }

        public static ITransformer Train(MLContext mlContext, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<MayorAppel>(dataPath, hasHeader: true, separatorChar: ',');
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "ResPositive")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "MonthEncoded", inputColumnName: "Month"))
                .Append(mlContext.Transforms.Concatenate("Features", "Year", "MonthEncoded", "TotalAppeals", "AppealsToMayor",
                "ResExplained", "ResNegative", "ElFormToMayor", "PapFormToMayor", "To10KTotalVAO", "To10KMayorVAO",
                "To10KTotalZAO", "To10KMayorZAO", "To10KTotalZelAO", "To10KMayorZelAO", "To10KTotalSAO", "To10KMayorSAO"
                , "To10KTotalSVAO", "To10KMayorSVAO", "To10KTotalSZAO", "To10KMayorSZAO", "To10KTotalTiNAO", "To10KMayorTiNAO"
                , "To10KTotalCAO", "To10KMayorCAO", "To10KTotalYUAO", "To10KMayorYUAO", "To10KTotalYUVAO", "To10KMayorYUVAO"
                , "To10KTotalYUZAO", "To10KMayorYUZAO")).Append(mlContext.Regression.Trainers.FastTree());
            var model = pipeline.Fit(dataView);
            return model;
                       

        }

        private static void Evaluate(MLContext mlContext, ITransformer model)
        {
            
            IDataView dataView = mlContext.Data.LoadFromTextFile<MayorAppel>(_testDataPath, hasHeader: true, separatorChar: ',');
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");

        }

        private static void TestSinglePrediction(MLContext mlContext, ITransformer model)
        {

            var predictionFunction = mlContext.Model.CreatePredictionEngine<MayorAppel, MayorAppelPrediction>(model);
            var MayorAppelSampleMinData = new MayorAppel()
            {
                Year = 2019,
                Month = "August",
                ResPositive = 0
            };


            var MayorAppelSampleMediumData = new MayorAppel()
            {
                Year = 2019,
                Month = "August",
                TotalAppeals = 111340,
                AppealsToMayor = 17932,
                ResExplained = 66858,
                ResNegative = 8945,
                ElFormToMayor = 14931,
                PapFormToMayor = 2967,
                ResPositive = 0
            };

            var MayorAppelSampleMaxData = new MayorAppel()
            {
                Year = 2019,
                Month = "August",
                TotalAppeals = 111340,
                AppealsToMayor = 17932,
                ResExplained = 66858,
                ResNegative = 8945,
                ElFormToMayor = 14931,
                PapFormToMayor = 2967,
                To10KTotalVAO = 67,
                To10KMayorVAO = 13,
                To10KTotalZAO = 57,
                To10KMayorZAO = 13,
                To10KTotalZelAO = 49,
                To10KMayorZelAO = 9,
                To10KTotalSAO = 71,
                To10KMayorSAO  = 14,
                To10KTotalSVAO = 86,
                To10KMayorSVAO = 27,
                To10KTotalSZAO = 68,
                To10KMayorSZAO = 12,
                To10KTotalTiNAO = 93,
                To10KMayorTiNAO = 36,
                To10KTotalCAO = 104,
                To10KMayorCAO = 24,
                To10KTotalYUAO = 56,
                To10KMayorYUAO = 12,
                To10KTotalYUVAO = 59,
                To10KMayorYUVAO = 13,
                To10KTotalYUZAO = 78,
                To10KMayorYUZAO = 23,
                ResPositive = 0
                
            };

            var predictionMin = predictionFunction.Predict(MayorAppelSampleMinData);
            var predictionMed = predictionFunction.Predict(MayorAppelSampleMediumData);
            var predictionMax = predictionFunction.Predict(MayorAppelSampleMaxData);

            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Prediction for August 2019");
            Console.WriteLine($"Predicted Positive decisions (Minimum Features): {predictionMin.ResPositive:0.####}, actual res_positive : 22313");
            Console.WriteLine($"Predicted Positive decisions (Medium Features: {predictionMed.ResPositive:0.####}, actual res_positive : 22313");
            Console.WriteLine($"Predicted Positive decisions (Maximum Features): {predictionMax.ResPositive:0.####}, actual res_positive : 22313");
            Console.WriteLine($"**********************************************************************");
        }

    }
}
