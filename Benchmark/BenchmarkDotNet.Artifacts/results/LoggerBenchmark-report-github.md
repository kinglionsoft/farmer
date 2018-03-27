``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.0)
Intel Core i5-7600 CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3421873 Hz, Resolution=292.2376 ns, Timer=TSC
.NET Core SDK=2.1.100
  [Host] : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT

Job=Core  Runtime=Core  

```
| Method |   N |      Mean |     Error |   StdDev |    Median | Rank |
|------- |---- |----------:|----------:|---------:|----------:|-----:|
|    **Run** |  **10** |  **38.94 us** | **0.8725 us** | **2.531 us** |  **38.06 us** |    **1** |
|    **Run** | **100** | **377.65 us** | **2.4473 us** | **1.911 us** | **377.17 us** |    **2** |
