using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Bogus;

namespace DataTableFromList;

public static class DataTableExtensions
{
    // private static readonly Func<Student, object[]> StudentToItemArray;
    // private static readonly List<string> StudentFieldNames;

    // static DataTableExtensions()
    // {


    //     var properties = typeof(Student).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //     var parameterExpression = Expression.Parameter(typeof(Student), "s");
    //     var newArrayExpression = Expression.NewArrayInit(
    //         typeof(object),
    //         properties.Select(p => Expression.Convert(Expression.Property(parameterExpression, p), typeof(object))).ToArray()
    //     );
    //     StudentToItemArray = Expression.Lambda<Func<Student, object[]>>(newArrayExpression, parameterExpression).Compile();
    // }

    public static DataTable ToDataTableUsingManualInitialization(this List<Student> students)
    {
        var table = new DataTable(nameof(Student));

        // Define columns
        table.Columns.Add(nameof(Student.Id), typeof(int));
        table.Columns.Add(nameof(Student.FirstName), typeof(string));
        table.Columns.Add(nameof(Student.LastName), typeof(string));
        table.Columns.Add(nameof(Student.DateOfBirth), typeof(DateTime));
        table.Columns.Add(nameof(Student.IsActive), typeof(bool));
        table.Columns.Add(nameof(Student.Balance), typeof(decimal));
        table.Columns.Add(nameof(Student.Email), typeof(string));
        table.Columns.Add(nameof(Student.PhoneNumber), typeof(string));
        table.Columns.Add(nameof(Student.Company), typeof(string));
        table.Columns.Add(nameof(Student.Latitude), typeof(double));
        table.Columns.Add(nameof(Student.Longitude), typeof(double));

        // Populate rows
        foreach (var student in students)
        {
            table.Rows.Add(
                student.Id, 
                student.FirstName, 
                student.LastName, 
                student.DateOfBirth,
                student.IsActive, 
                student.Balance, 
                student.Email, 
                student.PhoneNumber, 
                student.Company,
                student.Latitude, 
                student.Longitude
            );
        }

        return table;
    }

    public static DataTable ToDataTableWithExpressions<T>(this IEnumerable<T> collection)
    {
        var table = new DataTable();

        if (!collection.Any()) return table;

        List<(string name, Type type)> properties = TypeReflectionCache.GetOrCreatePropertyDetails<T>();
        foreach (var prop in properties)
        {
            table.Columns.Add(prop.name, prop.type);
        }

        var convertToRowArray = TypeReflectionCache.GetOrCreateObjectArrayConverter<T>();

        foreach (var item in collection)
        {
            table.Rows.Add(convertToRowArray(item));
        }

        return table;
    }

    public static DataTable ToDataTableUsingReflection<T>(this List<T> items)
    {
        var table = new DataTable(typeof(T).Name);

        // Get properties of T
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => p.CanRead)
                                .ToList();

        // Create columns from property names
        foreach (var prop in properties)
        {
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        // Add rows to DataTable
        foreach (var item in items)
        {
            var values = properties.Select(prop => prop.GetValue(item, null)).ToArray();
            table.Rows.Add(values);
        }

        return table;
    }
}
