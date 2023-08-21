using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Engines;
using Bogus;

namespace DataTableFromList;

[MemoryDiagnoser]
public class DataTableBenchmark
{
    private readonly List<Student> students;

    public DataTableBenchmark()
    {
        Console.WriteLine("Generating sample data");

        students = GenerateSampleData(10000);
        Console.WriteLine("Generated sample data");

    }

    public static List<Student> GenerateSampleData(int numberOfItems)
    {
        Randomizer.Seed = new Random(8675309);
        var faker = new Faker<Student>("ro")
            .RuleFor(u => u.Id, f => f.IndexFaker)
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.DateOfBirth, f => f.Person.DateOfBirth)
            .RuleFor(u => u.IsActive, f => f.Random.Bool())
            .RuleFor(u => u.Balance, f => f.Finance.Amount())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.PhoneNumber, f => f.Person.Phone)
            .RuleFor(u => u.Company, f => f.Company.CompanyName())
            .RuleFor(u => u.Latitude, f => f.Address.Latitude())
            .RuleFor(u => u.Longitude, f => f.Address.Longitude());

        return faker.Generate(numberOfItems);
    }


    [Benchmark]
    public void UsingReflection()
    {
        students.ToDataTableUsingReflection();
    }

    [Benchmark]
    public void UsingCompiledExpressions()
    {
        students.ToDataTableWithExpressions();
    }

    [Benchmark]
    public void UsingManualInitialization()
    {
        students.ToDataTableUsingManualInitialization();
    }    
}