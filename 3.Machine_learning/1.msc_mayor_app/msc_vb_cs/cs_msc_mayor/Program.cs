using System;
using System.Linq;
using Accord.Statistics.Models.Regression.Linear;
using Accord.IO;
using Accord.Math;
using System.Data;
using System.Collections.Generic;
using Accord.Controls;
using Accord.Math.Optimization.Losses;

namespace cs_msc_mayor
{
    class Program
    {
       
        static void Main(string[] args)
        {

            int traintPos = 18;
            int testPos = 22;
            int allData = testPos + (testPos - traintPos);


            //for correct symbol of float point
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;


            //read data (If you use linux do not forget to correct the path to the files)
            string CsvFilePath = @"msc_appel_data.csv";
            DataTable mscTable = new CsvReader(CsvFilePath, true).ToTable();


            Dictionary<string, double> monthNames = new Dictionary<string, double>
            {
                ["January"] = 1,
                ["February"] = 2,
                ["March"] = 3,
                ["April"] = 4,
                ["May"] = 5,
                ["June"] = 6,
                ["July"] = 7,
                ["August"] = 8,
                ["September"] = 9,
                ["October"] = 10,
                ["November"] = 11,
                ["December"] = 12

            };


            string[] months = mscTable.Columns["month"].ToArray<String>();
            double[] dMonths= new double[months.Length];

            for (int i=0; i< months.Length; i++)
            {
                dMonths[i] = monthNames[months[i]];
                //Console.WriteLine(dMonths[i]);
            }
            double[] totalAppeals = mscTable.Columns["total_appeals"].ToArray();
            double[] years = mscTable.Columns["year"].ToArray();
            double[] OutResPositive = mscTable.Columns["res_positive"].ToArray();

            double[] OutResPositiveTrain = OutResPositive.Get(0, traintPos);
            double[] OutResPositiveTest = OutResPositive.Get(traintPos, testPos);

            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };

            mscTable.Columns.Remove("month");
            mscTable.Columns.Remove("res_positive");
            mscTable.Columns.Remove("year");
            double[][] inputs = mscTable.ToArray();

            double[][] inputsTrain= inputs.Get(0, traintPos);
            double[][] inputsTest = inputs.Get(traintPos, testPos);


          

           MultipleLinearRegression regression = ols.Learn(inputsTrain, OutResPositiveTrain);

            double[] predicted = regression.Transform(inputsTest);

            //console output
            for (int i = 0; i < testPos - traintPos; i++)
            {
                Console.WriteLine("predicted: {0}   real: {1}", predicted[i], OutResPositiveTest[i]);
            }
            // And the squared error using the SquareLoss class:
            Console.WriteLine("error = {0}", new SquareLoss(OutResPositiveTest).Loss(predicted));

            double r2 = new RSquaredLoss(numberOfInputs: 29, expected: OutResPositiveTest).Loss(predicted); // should be 1
            Console.WriteLine("R^2 = {0}", r2);
            

            Console.WriteLine("Press enter to exit");

            // for chart 

            int[] classes = new int[allData];
            double[] mountX = new double[allData];
            for (int i = 0; i < allData; i++)
            {
                if (i<testPos)
                {
                   // for csv data
                    mountX[i] = i+1;
                    classes[i] = 0; //csv data is class 0
                }
                else
                {
                    //for predicted
                    mountX[i] = i- (testPos - traintPos)+1;
                    classes[i] = 1; //predicted is class 1
                }

                
            }
            List<double> OutChart = new List<double>();
            OutChart.AddRange(OutResPositive);
            OutChart.AddRange(predicted);

           


            ScatterplotBox.Show("res_positive from months", mountX, OutChart.ToArray(), classes).Hold();


            Console.ReadLine();
        }
    }
}
