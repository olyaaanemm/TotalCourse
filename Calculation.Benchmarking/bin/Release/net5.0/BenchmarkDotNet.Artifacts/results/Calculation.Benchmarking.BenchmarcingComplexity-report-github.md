``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.0.1 (21A559) [Darwin 21.1.0]
Apple M1 2.40GHz, 1 CPU, 8 logical and 8 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  Job-QPCVGH : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT

IterationCount=5  RunStrategy=Throughput  WarmupCount=5  

```
|          Method | Dimension |         Mean |        Error |       StdDev |     Gen 0 |     Gen 1 | Allocated |
|---------------- |---------- |-------------:|-------------:|-------------:|----------:|----------:|----------:|
| **ComplexThreaded** |         **5** |     **98.41 μs** |     **15.26 μs** |     **3.964 μs** |    **5.0049** |    **2.4414** |     **12 KB** |
| **ComplexThreaded** |        **10** |    **355.54 μs** |    **417.28 μs** |   **108.366 μs** |         **-** |         **-** |     **21 KB** |
| **ComplexThreaded** |        **25** |    **479.35 μs** |    **196.84 μs** |    **30.461 μs** |         **-** |         **-** |     **45 KB** |
| **ComplexThreaded** |        **50** |    **737.53 μs** |    **238.88 μs** |    **36.967 μs** |         **-** |         **-** |     **85 KB** |
| **ComplexThreaded** |       **100** |  **1,626.66 μs** |  **3,802.59 μs** |   **588.456 μs** |         **-** |         **-** |    **165 KB** |
| **ComplexThreaded** |       **150** |  **2,212.86 μs** |  **2,078.81 μs** |   **321.699 μs** |         **-** |         **-** |    **245 KB** |
| **ComplexThreaded** |       **250** |  **3,808.01 μs** |  **4,589.20 μs** |   **710.183 μs** | **1000.0000** |         **-** |    **396 KB** |
| **ComplexThreaded** |       **300** |  **7,681.88 μs** |  **7,104.00 μs** | **1,844.886 μs** | **1000.0000** |         **-** |    **510 KB** |
| **ComplexThreaded** |       **450** | **14,527.18 μs** | **28,452.29 μs** | **4,403.024 μs** | **1000.0000** |         **-** |    **715 KB** |
| **ComplexThreaded** |       **500** | **11,333.67 μs** | **10,477.67 μs** | **2,721.017 μs** | **1000.0000** |         **-** |    **851 KB** |
| **ComplexThreaded** |      **1000** | **34,723.86 μs** | **44,558.16 μs** | **6,895.424 μs** | **2000.0000** | **1000.0000** |  **1,574 KB** |
| **ComplexThreaded** |      **1500** | **51,563.05 μs** | **31,257.71 μs** | **4,837.165 μs** | **2000.0000** | **1000.0000** |  **2,442 KB** |
