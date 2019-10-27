using Microsoft.ML.Data;

namespace app_to_mayor_mlnet
{
    class MayorAppel
    {

        [LoadColumn(0)]
        public float Year;

        [LoadColumn(1)]
        public string Month;

        [LoadColumn(2)]
        public float TotalAppeals;

        [LoadColumn(3)]
        public float AppealsToMayor;

        [LoadColumn(4)]
        public float ResPositive;

        [LoadColumn(5)]
        public float ResExplained;

        [LoadColumn(6)]
        public float ResNegative;

        [LoadColumn(7)]
        public float ElFormToMayor;

        [LoadColumn(8)]
        public float PapFormToMayor;

        [LoadColumn(9)]
        public float To10KTotalVAO;

        [LoadColumn(10)]
        public float To10KMayorVAO;

        [LoadColumn(11)]
        public float To10KTotalZAO;

        [LoadColumn(12)]
        public float To10KMayorZAO;

        [LoadColumn(13)]
        public float To10KTotalZelAO;

        [LoadColumn(14)]
        public float To10KMayorZelAO;

        [LoadColumn(6)]
        public float To10KTotalSAO;

        [LoadColumn(15)]
        public float To10KMayorSAO;

        [LoadColumn(16)]
        public float To10KTotalSVAO;

        [LoadColumn(17)]
        public float To10KMayorSVAO;

        [LoadColumn(18)]
        public float To10KTotalSZAO;

        [LoadColumn(19)]
        public float To10KMayorSZAO;

        [LoadColumn(20)]
        public float To10KTotalTiNAO;

        [LoadColumn(21)]
        public float To10KMayorTiNAO;

        [LoadColumn(22)]
        public float To10KTotalCAO;

        [LoadColumn(23)]
        public float To10KMayorCAO;

        [LoadColumn(24)]
        public float To10KTotalYUAO;

        [LoadColumn(25)]
        public float To10KMayorYUAO;

        [LoadColumn(26)]
        public float To10KTotalYUVAO;

        [LoadColumn(27)]
        public float To10KMayorYUVAO;

        [LoadColumn(28)]
        public float To10KTotalYUZAO;

        [LoadColumn(29)]
        public float To10KMayorYUZAO;
    }

    
    public class MayorAppelPrediction
    {
        [ColumnName("Score")]
        public float ResPositive;
    }

}
