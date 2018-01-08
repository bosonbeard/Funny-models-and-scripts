'http://converter.telerik.com/
'accods control, accord io

Imports System
Imports System.Linq
Imports Accord.Statistics.Models.Regression.Linear
Imports Accord.IO
Imports Accord.Math
Imports System.Data
Imports System.Collections.Generic
Imports Accord.Controls
Imports Accord.Math.Optimization.Losses

Module Program
    Sub Main()
        'for separating the training and test samples
        Dim traintPos As Integer = 18
        Dim testPos As Integer = 22
        Dim allData As Integer = testPos + (testPos - traintPos)

        'for correct reading symbol of float point in csv
        Dim customCulture As System.Globalization.CultureInfo = CType(System.Threading.Thread.CurrentThread.CurrentCulture.Clone(), System.Globalization.CultureInfo)
        customCulture.NumberFormat.NumberDecimalSeparator = "."
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture

        'read data
        Dim CsvFilePath As String = "msc_appel_data.csv"
        Dim mscTable As DataTable = New CsvReader(CsvFilePath, True).ToTable()

        'for encoding the string values of months into numerical values
        Dim monthNames As Dictionary(Of String, Double) = New Dictionary(Of String, Double) From
            {{"January", 1}, {"February", 2}, {"March", 3}, {"April", 4}, {"May", 5}, {"June", 6},
            {"July", 7}, {"August", 8}, {"September", 9},
            {"October", 10}, {"November", 11}, {"December", 12}}
        Dim months As String() = mscTable.Columns("month").ToArray(Of String)()
        Dim dMonths As Double() = New Double(months.Length - 1) {}
        For i As Integer = 0 To months.Length - 1
            dMonths(i) = monthNames(months(i))
        Next

        'select the target column
        Dim OutResPositive As Double() = mscTable.Columns("res_positive").ToArray()

        'separation of the test and train target sample
        Dim OutResPositiveTrain As Double() = OutResPositive.[Get](0, traintPos)
        Dim OutResPositiveTest As Double() = OutResPositive.[Get](traintPos, testPos)

        'deleting unneeded columns
        mscTable.Columns.Remove("total_appeals")
        mscTable.Columns.Remove("month")
        mscTable.Columns.Remove("res_positive")
        mscTable.Columns.Remove("year")

        'receiving input data from a table
        Dim inputs As Double()() = mscTable.ToArray()

        'separation of the test and train sample
        Dim inputsTrain As Double()() = inputs.[Get](0, traintPos)
        Dim inputsTest As Double()() = inputs.[Get](traintPos, testPos)

        'simple linear regression model
        Dim ols = New OrdinaryLeastSquares() With {.UseIntercept = True}

        'linear regression model for several features
        Dim regression As MultipleLinearRegression = ols.Learn(inputsTrain, OutResPositiveTrain)

        'make a prediction
        Dim predicted As Double() = regression.Transform(inputsTest)

        'console output
        For i As Integer = 0 To testPos - traintPos - 1
            Console.WriteLine("predicted: {0}   real: {1}", predicted(i), OutResPositiveTest(i))
        Next

        'And  print the squared error using the SquareLoss class
        Console.WriteLine("error = {0}", New SquareLoss(OutResPositiveTest).Loss(predicted))

        'print the coefficient of determination
        Dim r2 As Double = New RSquaredLoss(numberOfInputs:=29, expected:=OutResPositiveTest).Loss(predicted)
        Console.WriteLine("R^2 = {0}", r2)

        'alternative print the coefficient of determination
        Dim ur2 As Double = regression.CoefficientOfDetermination(inputs, OutResPositiveTest, adjust:=True)
        Console.WriteLine("alternative version of R2 = {0}", r2)


        Console.WriteLine("Press enter and close chart to exit")

        'for chart 
        Dim classes As Integer() = New Integer(allData - 1) {}
        Dim mountX As Double() = New Double(allData - 1) {}
        For i As Integer = 0 To allData - 1
            If i < testPos Then
                mountX(i) = i + 1
                classes(i) = 0 'csv data is class 0
            Else
                mountX(i) = i - (testPos - traintPos) + 1
                classes(i) = 1 'predicted is class 1
            End If
        Next

        'make points of chart
        Dim OutChart As List(Of Double) = New List(Of Double)()
        OutChart.AddRange(OutResPositive)
        OutChart.AddRange(predicted)

        'plot chart
        ScatterplotBox.Show("res_positive from months", mountX, OutChart.ToArray(), classes).Hold()

        'for pause
        Console.ReadLine()
    End Sub

End Module
