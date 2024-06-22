using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run(typeof(TheirBenchmarks));

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