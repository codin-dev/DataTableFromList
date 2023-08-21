using BenchmarkDotNet.Running;
using System.Data;
using Spectre.Console;
using DataTableFromList;

var students = DataTableBenchmark.GenerateSampleData(10);
DisplayDataTable(students.ToDataTableWithExpressions());
var summary = BenchmarkRunner.Run<DataTableBenchmark>();

static void DisplayDataTable(DataTable table)
{
    var tableData = new Table();

    // Add columns
    foreach (DataColumn col in table.Columns)
    {
        tableData.AddColumn(new TableColumn(col.ColumnName));
    }

    // Add rows
    foreach (DataRow row in table.Rows)
    {
        tableData.AddRow(row.ItemArray.Select(item => item!.ToString()).ToArray()!);
    }

    // Render the table
    AnsiConsole.Write(tableData);
}

