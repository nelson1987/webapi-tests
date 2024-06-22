using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

//BenchmarkRunner.Run(typeof(TheirBenchmarks));
BenchmarkRunner.Run(typeof(TasksBenchmarks));

internal class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
}

[MemoryDiagnoser]
//[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net472)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
public class TasksBenchmarks
{
    private List<Item> _list;

    [Params(1_000, 10_000, 100_000, 1_000_000)]
    public int ListLength;

    [GlobalSetup]
    public void Setup()
    {
        _list = new List<Item>();
        for (int i = 0; i < ListLength; i++)
        {
            _list.Add(new Item() { Id = i, IsActive = i % 2 == 0, Name = $"Name{i}" });
        }
    }

    [Benchmark]
    public void GoodMethod()
    {
        // Method Syntax
        var methodSyntaxQuery = _list.Where(item => item.IsActive)
                                      .Select(item => new { item.Name, item.Id });
        foreach (var item in methodSyntaxQuery)
        {
            //item.Id = 0;
            //Console.WriteLine($"Name: {item.Name}, Id: {item.Id}");
        }
    }

    [Benchmark(Baseline = true)]
    public void BadMethod()
    {
        var querySyntaxQuery = from item in _list
                               where item.IsActive
                               select new { item.Name, item.Id };
        foreach (var item in querySyntaxQuery)
        {
            //Console.WriteLine($"Name: {item.Name}, Id: {item.Id}");
        }
    }
}

[MemoryDiagnoser]
//[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net472)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
public class TheirBenchmarks
{
    private List<int> _list;

    [Params(1_000, 10_000, 100_000, 1_000_000)]
    public int ListLength;

    [GlobalSetup]
    public void Setup()
    {
        _list = new List<int>();
        for (int i = 0; i < ListLength; i++)
        {
            _list.Add(i);
        }
    }

    [Benchmark(Baseline = true)]
    public void OurBenchmark()
    {
        _list!.Sort();
    }

    [Benchmark]
    public void TheirBenchmark()
    {
        _list!.Sort((x, y) => x.CompareTo(y));
    }
}