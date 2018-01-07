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
        Dim traintPos As Integer = 18
        Dim testPos As Integer = 22
        Dim allData As Integer = testPos + (testPos - traintPos)
        Dim customCulture As System.Globalization.CultureInfo = CType(System.Threading.Thread.CurrentThread.CurrentCulture.Clone(), System.Globalization.CultureInfo)
        customCulture.NumberFormat.NumberDecimalSeparator = "."
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture
        Dim CsvFilePath As String = "msc_appel_data.csv"
        Dim mscTable As DataTable = New CsvReader(CsvFilePath, True).ToTable()
        Dim monthNames As Dictionary(Of String, Double) = New Dictionary(Of String, Double) From {{"January", 1}, {"February", 2}, {"March", 3}, {"April", 4}, {"May", 5}, {"June", 6}, {"July", 7}, {"August", 8}, {"September", 9}, {"October", 10}, {"November", 11}, {"December", 12}}
        Dim months As String() = mscTable.Columns("month").ToArray(Of String)()
        Dim dMonths As Double() = New Double(months.Length - 1) {}
        For i As Integer = 0 To months.Length - 1
            dMonths(i) = monthNames(months(i))
        Next

        Dim totalAppeals As Double() = mscTable.Columns("total_appeals").ToArray()
        Dim years As Double() = mscTable.Columns("year").ToArray()
        Dim OutResPositive As Double() = mscTable.Columns("res_positive").ToArray()
        Dim OutResPositiveTrain As Double() = OutResPositive.[Get](0, traintPos)
        Dim OutResPositiveTest As Double() = OutResPositive.[Get](traintPos, testPos)
        Dim ols = New OrdinaryLeastSquares() With {.UseIntercept = True}
        mscTable.Columns.Remove("month")
        mscTable.Columns.Remove("res_positive")
        mscTable.Columns.Remove("year")
        Dim inputs As Double()() = mscTable.ToArray()
        Dim inputsTrain As Double()() = inputs.[Get](0, traintPos)
        Dim inputsTest As Double()() = inputs.[Get](traintPos, testPos)
        Dim regression As MultipleLinearRegression = ols.Learn(inputsTrain, OutResPositiveTrain)
        Dim predicted As Double() = regression.Transform(inputsTest)
        For i As Integer = 0 To testPos - traintPos - 1
            Console.WriteLine("predicted: {0}   real: {1}", predicted(i), OutResPositiveTest(i))
        Next

        Console.WriteLine("error = {0}", New SquareLoss(OutResPositiveTest).Loss(predicted))
        Dim r2 As Double = New RSquaredLoss(numberOfInputs:=29, expected:=OutResPositiveTest).Loss(predicted)
        Console.WriteLine("R^2 = {0}", r2)
        Console.WriteLine("Press enter to exit")
        Dim classes As Integer() = New Integer(allData - 1) {}
        Dim mountX As Double() = New Double(allData - 1) {}
        For i As Integer = 0 To allData - 1
            If i < testPos Then
                mountX(i) = i + 1
                classes(i) = 0
            Else
                mountX(i) = i - (testPos - traintPos) + 1
                classes(i) = 1
            End If
        Next

        Dim OutChart As List(Of Double) = New List(Of Double)()
        OutChart.AddRange(OutResPositive)
        OutChart.AddRange(predicted)
        ScatterplotBox.Show("res_positive from months", mountX, OutChart.ToArray(), classes).Hold()
        Console.ReadLine()
    End Sub

End Module
