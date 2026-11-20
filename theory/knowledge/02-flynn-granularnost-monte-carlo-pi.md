# Lecture 2: Flynn, granularity and Monte Carlo π (source: 02-flynn-granularnost-monte-carlo-pi, 23 slides)

## Map
- This file covers three themes: the Flynn taxonomy (Flynnova taksonomija), the size of a task (granularnost) and the overhead of parallelization (tara), and the first full parallel program (Monte Carlo π).
- Main idea: a parallel program is faster only when each task does much more work than the overhead it causes. Isolation (tasks share nothing) fixes two of the traps.
- The Flynn classes tell what kind of parallelism the hardware offers. A C# `Task` runs on MIMD hardware.
- MIMD freedom has a price: cores that share data must synchronize. This price causes the traps at the end (shared counter, shared `Random`).
- The lecture shows four traps: tiny tasks (measured), shared counter, shared generator, lost remainder. The fix for the middle two is isolation.
- Terms: "task" = zadatak. "Overhead" = tara (parallel overhead). This file uses the word "overhead" after it defines it. "Trap" = zamka.

## Goals of the lecture   [S1-S2]
**What:** Slide 1 gives the title "Lecture 2: Flynn, granularity and Monte Carlo π". The author is Marin Aglić Čuvić, based on the lectures of Tonći Dadić. The academic year is 2026/2027. Lecture 2 has three goals. (1) Classify computers by Flynn: SISD, SIMD, MISD, MIMD. (2) Measure how much work to give one task: granularity and overhead. (3) Compute π on k tasks and recognize four traps.
**Why:** The previous lecture showed that parallel programming is necessary and that it can go wrong. It covered data and task parallelism, shared and distributed memory, and Amdahl's law (Amdahlov zakon). This lecture writes the first full parallel program and shows how it fails.
**Conclusion:** The first full parallel program is Monte Carlo π (slide 1 notes). It is the example on which the four traps appear. The four traps are: tiny tasks, shared counter, shared random-number generator, lost remainder.
**Example:** Exercise 2 does the same work on a sum of numbers.
**Demo (notes):** Open `Vizualizacije/tjedan2.html`. It follows the slides in order. Run the demo `Predavanje/PiDemo` with `dotnet run -c Release`.
**Link:** Exercise 2 (see the cross-reference). Amdahl's law returns in "Speedup".

## What a taxonomy classifies   [S3]
**What:** A taxonomy (taksonomija) puts things into classes by shared properties, as in biology. Michael Flynn proposed in 1966 to classify computers by two streams [1, 2].
- Instruction stream (tok instrukcija): the sequence of commands that one control unit fetches and executes. Example: LOAD, ADD, STORE.
- Data stream (tok podataka): the sequence of operands on which the commands work.
- Control unit (upravljačka jedinica): fetches and decodes commands and tells what to do.
- Arithmetic logic unit (ALU, aritmetičko-logička jedinica): only computes on data. Both units are parts of a core.
**Why:** Two questions, each with two answers, give four classes. Question 1: one or many instruction streams at the same time? Question 2: one or many data streams at the same time?
**Conclusion:** The class tells the programmer which parallelism the hardware offers. A program cannot use what the hardware does not have.
**Example:** The slide shows a control unit that sends LOAD, ADD, STORE, LOAD, ADD to an ALU. The ALU works on data a₀ to a₄.
**Demo (notes):** In `tjedan2.html`, section 1, switch on "more data streams", then "more instruction streams".
**Link:** "The four classes" for the table.

## The four classes   [S4-S5]
**What:** The table has instruction streams in rows and data streams in columns. Read each letter: S = single, M = multiple, I = instruction, D = data.

| | one data stream | many data streams |
|---|---|---|
| one instruction stream | SISD: one core, sequential | SIMD: vector instructions, GPU |
| many instruction streams | MISD: rare in practice | MIMD: multicore CPU, cluster |

Summary on slide 5:

| Class | How | Example |
|---|---|---|
| SISD | one command, one datum per step | a plain loop on one core |
| SIMD | the same command on many data at once | vector instruction, GPU |
| MISD | different commands on the same datum | rare; many algorithms on the same message |
| MIMD | each core has its own commands and its own data | your laptop, a cluster |

**Why:** The picture (notes) uses orange lines for instruction streams, dark squares for computing units, blue squares for data. SISD: one line, one unit, one datum per step. SIMD: one line branches to three units, each with its own datum. MISD: three lines and three units, all on the same datum. MIMD: each unit has its own line and its own datum.
**Conclusion:** SIMD = data parallelism (paralelizam podataka). MIMD = task parallelism (paralelizam zadataka), and it can also be data parallelism. SISD = sequential execution.
**Demo (notes):** In `tjedan2.html`, section 2, compare the number of cycles for the four classes.
**Link:** The next four topics describe each class.

## SISD: one command, one datum   [S6]
**What:** In each clock step, one command works on one datum. The next datum waits.
**Why:** Every sequential program is SISD. Its time is the reference T(slijedno) for speedup. A core works as a pipeline inside, as the previous lecture showed. At program level it is still SISD.
**Conclusion:** SISD is the baseline. All speedup is measured against it.
**Example:** A loop adds an array element by element on one core: `s += a[0]; s += a[1]; …`. The picture shows 8 elements a0 to a7; only one is active per step.
**Demo (notes):** `tjedan2.html`, section 2, class SISD: 8 elements, 8 cycles.
**Link:** "Speedup" uses T(slijedno).

## SIMD: the same command, many data   [S7]
**What:** One control unit sends the same command to many units. Each unit applies it to its own datum in the same step [3]. This is data parallelism.
**Why:** The hardware has one instruction stream, so all data must pass the same command (inferred). A branch (`if`) slows SIMD, because some units wait while the others work.
**Conclusion:** Branching slows SIMD. SIMD suits work where all data pass the same command (inferred).
**Example:**
- The slide shows one "+" command that adds 8 pairs at once: 3+2, 1+7, and so on. Row a = 3 1 4 1 5 9 2 6. Row b = 2 7 1 8 2 8 1 8. Result a+b = 5 8 5 9 7 17 3 14.
- A 256-bit vector command adds 8 numbers of type float at once.
- A GPU runs the same command for a group of 32 threads (niti), called a warp. NVIDIA calls it SIMT [4].
- In C#, use `System.Numerics.Vector<T>` and `Vector256<T>` [3]. GPUs come in week 13.
**Pitfall:** The CPU and the GPU handle branches differently. On a CPU, the program works with the whole vector and sees its width, for example 8 numbers. The programmer solves the branch. On a GPU, the programmer writes code for one thread, and the hardware runs 32 such threads together. The GPU solves the branch alone: it runs both branches in turn, each for part of the threads.
**Demo (notes):** `tjedan2.html`, section 2, class SIMD: 8 elements, 2 cycles.
**Link:** "Where is your computer" for the levels. Week 13 for GPUs.

## MISD: different commands, the same datum   [S8]
**What:** Many units run different commands on the same data stream.
**Why:** The class exists because the 2×2 table must be complete. LLNL writes that there are "few, if any, real examples" [6].
**Conclusion:** Do not expect MISD in practice.
**Example:** Many cryptographic algorithms try to break the same encrypted message at the same time [5]. The picture shows algorithms A, B, C and one encrypted message.
**Demo (notes):** `tjedan2.html`, section 2, class MISD: three results per cycle, but it is not faster.
**Link:** "The four classes".

## MIMD: different commands and different data   [S9]
**What:** Each core has its own control unit. It runs its own program on its own data, independent of the others.
**Why:** The freedom has a price: cores that share data must coordinate. Two traps below come from this: the shared counter and the shared generator.
**Conclusion:** Every `Task` in C# runs on a MIMD computer.
**Example:** The picture shows four cores. Core 1 adds part of an array. Core 2 looks for the maximum. Core 3 draws the user interface. Core 4 reads a file. Examples of MIMD: a multicore processor, and a cluster of computers joined by a network [6].
**Demo (notes):** `tjedan2.html`, section 2, class MIMD.
**Link:** "Shared counter" and "Shared generator".

## Where is your computer   [S10]
**What:** A real computer is several classes on different levels.

| Level | Class | In C# |
|---|---|---|
| Between CPU cores | MIMD: each core runs its own program | `Task`, `Parallel.For` (exercise 2) |
| Inside one core | SIMD: one command on 8 numbers | `Vector<T>`, vectorized methods [3] |
| Graphics card | SIMD (SIMT): warp of 32 threads [4] | week 13 |

**Why:** Each CPU core has its own control unit, so each core can run its own program. Inside each core, a SIMD unit processes 8 float numbers with one command. A GPU has many multiprocessors. Each has a scheduler, and a warp of 32 threads inside runs the same command.
**Conclusion:** Your computer is MIMD with SIMD units.
**Example:** The CPU on the slide is an i7-9750H with 6 cores. The notes say the measured examples come from a computer with 6 cores.
**Question (from the slide):** Which class has the program from exercise 2 (sum on k tasks)? Answer: MIMD, because each task runs on its own core.
**Pitfall:** Memory waits differ. When a warp waits for memory, the multiprocessor switches at once to another warp. A CPU hides the wait with a large cache.
**Link:** "Summary".

## Task, communication, synchronization   [S11]
**What:** Three terms for the rest of the course.
- Task (zadatak): a separate part of work that can run alone. In C#: `Task`.
- Communication (komunikacija): data exchange between tasks, by a shared variable or by a message.
- Synchronization (sinkronizacija): waiting until another task finishes. In C#: `Task.WaitAll`, or reading `Result`.
**Why:** The picture shows task A computing, then communicating. Task B computes, then waits (grey) because it needs the result of A, then computes on. Waiting is time in which the core does nothing useful.
**Conclusion:** A parallel program must give the same result as the sequential program.
**Demo (notes):** `tjedan2.html`, section 3: move the moment when B needs the result.
**Link:** "Granularity" (communication is a cost). "Shared counter" (communication through a shared variable).

## Granularity: how much work per task   [S12]
**What:** Granularity (granularnost) is the ratio of work done to communication between tasks [5, 6].
- Coarse (krupna): much computation between two communications.
- Fine (sitna): little computation and frequent communication.
**Why:** Both bars on the picture have the same computation (blue). Each communication and synchronization costs time (orange). A fine bar takes longer for the same work.
**Conclusion:** A task must not be too small.
**Demo (notes):** `tjedan2.html`, section 4.
**Link:** "Overhead" gives the formula. "Trap 1: tiny tasks" gives the measurement.

## Overhead (tara)   [S13]
**What:** Overhead (tara; English: parallel overhead) is the extra cost of parallelization. It is everything that a parallel program does and a sequential program does not [5, 6]: create and schedule tasks, wait, synchronize, merge results.
Formula: `T(k) ≈ T(slijedno) / k + overhead(k)`
**Why:** The word tara comes from trade: gross = net + tare, and tare is the weight of the packaging. In a program, net is the useful computation. Overhead does not fall with the number of cores. It grows with the number of tasks.
**Conclusion:** Overhead grows with the number of tasks (slide). So too many small tasks make the program slower (inferred).
**Example:** A sequential program takes 1000 units. Split it into 4 tasks of 250. The tasks do not start at the same time, because the thread pool (fond niti) starts threads one by one. The faster tasks wait for the slowest. Then the results are merged. The total is about 410, not the ideal 1000 / 4 = 250. The picture marks: start-up cost, waiting for the slowest, merge of results, useful computation (net).
**Demo (notes):** `tjedan2.html`, section 4: work of 10 µs and 32 tasks, then 100 000 µs.
**Link:** Amdahl's law (lecture 1) for the sequential part. "Speedup".

## Trap 1: tiny tasks   [S14]
**What:** If each task has little work, the overhead is larger than the work, and the parallel program is slower than the sequential one.
**Why:** The demo computes π for different N, sequentially and on k = 12 tasks. It takes the median of 21 measurements. The program warms up before measuring, because the first calls take much longer (notes).

| N | sequential | parallel (12 tasks) |
|---|---|---|
| 100 | 1,9 µs | 21,0 µs |
| 1.000 | 12,2 µs | 9,7 µs |
| 10.000 | 141,3 µs | 134,7 µs |
| 100.000 | 989,9 µs | 320,8 µs |
| 10.000.000 | 100.996,3 µs | 14.766,2 µs |

For N = 100, parallel is 11 times slower: overhead (about 20 µs) is larger than the work (2 µs). Up to N = 10.000, parallel is not much faster. From N = 100.000, the gain is 3 to 7 times. The notes say about three times at 100.000 points and almost seven times at ten million.
**Conclusion:** Parallelize only above a threshold. Give each task a large piece of work.
**Demo (notes):** `PiDemo`, menu 6 (about 5 s). `tjedan2.html`, section 4. `Vizualizacije/zamke.html`, section 2.
**Link:** "Overhead". Slide 22 asks: how many points does one task need to make the parallel version worth the overhead?

## Monte Carlo π: the idea   [S15]
**What:** Take a square with side 1 and a quarter of a circle with radius 1 inside it.
- Area ratio: quarter circle / square = π / 4.
- Throw N random points. Count K points with `x² + y² ≤ 1`.
- `π ≈ 4 · K / N`
**Why:** The area of the quarter circle is π/4 and the area of the square is 1. The share of points inside the circle is about π/4. The points are independent of each other.
**Conclusion:** Independent points give ideal data parallelism. Each task can throw its own points.
**Demo (notes):** `tjedan2.html`, section 5, mode "correct".
**Link:** "Sequential program". The traps below break this simple split.

## Sequential program and accuracy   [S16]
**What:** The sequential version uses one generator, one counter and a loop of N steps.
**Example:**
```csharp
var rnd = new Random();
long K = 0;
for (long i = 0; i < N; i++)
{
    double x = rnd.NextDouble();
    double y = rnd.NextDouble();
    if (x * x + y * y <= 1) K++;
}
double pi = 4.0 * K / N;
```
Output: `Slijedno: N = 100.000.000, pi = 3,14158 (998 ms)`.
**Chart data (pptx):** XY chart with lines, both axes logarithmic. Series 1, the simulated error |π̂ − π|, has 21 points from N = 100 to N = 10.000.000 (N = 100, 178, 316, 562, 1000, 1778, 3162, 5623, 10000, 17783, 31623, 56234, 100000, 177828, 316228, 562341, 1000000, 1778279, 3162278, 5623413, 10000000). All 21 points:

| N | error |
|---|---|
| 100 | 0.258407 |
| 178 | 0.18425 |
| 316 | 0.174863 |
| 562 | 0.0327846 |
| 1000 | 0.0504073 |
| 1778 | 0.00576393 |
| 3162 | 0.00831247 |
| 5623 | 0.000502314 |
| 10000 | 0.00239265 |
| 17783 | 0.00803814 |
| 31623 | 0.00421894 |
| 56234 | 0.00241275 |
| 100000 | 0.00204735 |
| 177828 | 0.00181714 |
| 316228 | 0.00142796 |
| 562341 | 0.000237659 |
| 1000000 | 0.00191665 |
| 1778279 | 0.00164442 |
| 3162278 | 0.00101362 |
| 5623413 | 0.000327376 |
| 10000000 | 0.000141346 |

The curve is not smooth. Series 2, the reference line 1/√N, has 2 points: (100; 0,16) and (10.000.000; 0,000506).
**Why:** The error falls as 1/√N [8]. For a 10 times smaller error, the program needs 100 times more points. The graph on the slide is a simulation, and both axes are logarithmic. The graph compares |π̂ − π| with 1/√N.
**Conclusion:** A small error needs many points, so it is useful to speed up the calculation.
**Demo (notes):** `PiDemo`, menu 8 (sequential and all traps in one run, about 6 s).
**Link:** "Speedup".

## Trap 2: shared counter   [S17]
**What:** All tasks increment the same counter `K`. The result is wrong and changes from run to run.
**Example:**
```csharp
long K = 0;          // jedan brojač za sve
for (int j = 0; j < k; j++)
    zadaci[j] = Task.Run(() =>
    {
        var rnd = new Random();
        for (long i = 0; i < N / k; i++)
        {
            ...
            if (x * x + y * y <= 1) K++;
        }
    });
```
Output: `Dijeljeni brojač, k = 12: pi = 0,30134 (871 ms)`. The correct value is 3,14.
**Why:** `K++` has three steps: read, add one, write. Task A reads 5. Task B reads 5. Both compute 6. Both write 6. K is 6 and not 7: one increment is lost. This is a data race (utrka podataka). Here about 90 % of the increments are lost.
**Pitfall:** The result changes from run to run. The compiler reports nothing.
**Conclusion:** A shared variable that many tasks write gives a wrong result without any error message.
**Demo (notes):** `PiDemo`, menu 2, run two times (the result changes). `tjedan2.html`, section 6, step by step.
**Link:** Same as trap B in exercise 2 (Zadatak 3, Zamka B). Fix: "Fix: isolation". Lecture on data race (week 4).

## Trap 3: shared generator (Random)   [S18]
**What:** Each task has its own counter, but all tasks use one `Random`. The result is almost exactly 4, and the program is slower.
**Example:**
```csharp
var rnd = new Random(42); // jedan za sve
for (int j = 0; j < k; j++)
    zadaci[j] = Task.Run(() =>
    {
        long K = 0;          // svoj brojač
        for (long i = 0; i < N / k; i++)
        {
            double x = rnd.NextDouble();
            ...
        }
        return K;
    });
```
Output: `Dijeljeni Random, k = 12: pi = 3,99995, parova (0, 0): 99.993.788 (4203 ms)`.

| Version | Time (ms) |
|---|---|
| sequential | 998.0 |
| shared Random | 4203.0 |
| isolation | 144.0 |

**Why:** `Random` is not safe for concurrent access. Without synchronization, its methods return 0 [9]. Almost all points become (0, 0). That point is inside the circle, so the ratio goes to 4. The program is also 4 times slower than sequential, because all cores constantly write to the same generator state.
**Pitfall:** The seed 42 is set on purpose. Without a seed, the error is less visible.
**Conclusion:** Do not share a `Random` between tasks.
**Demo (notes):** `PiDemo`, menu 3. `tjedan2.html`, section 5, mode "one Random for all".
**Link:** Exercise 2, Zamka D (`Random.Shared`). Summary question: why is the shared `Random` also slower?

## Fix: isolation   [S19]
**What:** Isolation (izolacija): share nothing. Each task has its own `Random` and its own counter `K` and returns its own result. Sum the results after all tasks finish, through `Result`.
**Example:**
```csharp
for (int j = 0; j < k; j++)
{
    long dio = (j == k - 1)
        ? N - (N / k) * (k - 1)   // zadnji: ostatak
        : N / k;
    zadaci[j] = Task.Run(() =>
    {
        var rnd = new Random();   // svoj generator
        long K = 0;               // svoj brojač
        for (long i = 0; i < dio; i++) { ... }
        return K;
    });
}
long ukupno = zadaci.Sum(z => z.Result);
```
Output: `Izolacija, k = 12: pi = 3,14168 (144 ms)`. Sequential takes 998 ms in the same run, so the fix is almost 7 times faster. The last task also gets the remainder (next trap).
**Why:** Isolation removes the cause of both traps: no shared write, no synchronization during the work. The program adds only at the end.
**Pitfall:** `Random.Shared` is safe for threads [10], but it is again one shared object.
**Pitfall (notes, a question that may come):** Do generators created at the same moment give the same numbers? In current .NET: no, because `new Random()` takes its seed from an internal generator. In old .NET Framework the seed came from the clock. Then all tasks compute the same points. The value of π is then correct, but its precision is that of N / k points (source wording). The experiment is in `PiDemo`, menu 9.
**Demo (notes):** `PiDemo`, menu 4.
**Conclusion:** Isolate data per task. Merge once, at the end.
**Link:** Exercise 2 sum: `Task.Run` returns a value and the program reads `Result`.

## Trap 4: lost remainder N / k   [S20]
**What:** Integer division of N by k loses the remainder. No task processes the lost points. The program runs without error.
**Example:** N = 1.000.000.007 and k = 8. Each task gets N / 8 = 125.000.000 points. The tasks process 1.000.000.000 points. Nobody processes 7.
**Why:** Integer division cuts the remainder, and no task owns it (notes). `N / k` rounds down (inferred).
**Pitfall:** For π the error is small. For a sum (exercise 2), the result is simply wrong.
**Conclusion:** The last task gets the remainder (code on slide 19). Always compare the parallel result with the sequential result.
**Demo (notes):** `PiDemo`, menu 7. `tjedan2.html`, section 7.
**Link:** "Fix: isolation" (the `dio` code).

## Speedup   [S21]
**What:** Speedup S (ubrzanje) = sequential time / parallel time. Ideal: S = k.

| k | time | S |
|---|---|---|
| 1 | 927 ms | 1,01 |
| 2 | 469 ms | 2,00 |
| 4 | 236 ms | 3,96 |
| 6 | 175 ms | 5,35 |
| 8 | 164 ms | 5,71 |
| 12 | 148 ms | 6,31 |
| 16 | 149 ms | 6,28 |

Sequential time is 936 ms. Above 12 tasks, the speedup does not grow.
The chart (pptx) shows measured S against the ideal line S = k, which goes from (1; 1) to (16; 16).
**Why:** For 2 tasks S = 2, and for 4 tasks S is almost 4. After that, the curve leaves the ideal line. For 12 tasks S is 6,3. Question: why not 12? Reasons: (1) the computer has 6 physical cores and 12 logical cores; hyperthreading does not double the computation. (2) Overhead. (3) The sequential part of the program (Amdahl).
**Conclusion:** Speedup is lower than k, and it stops at the number of logical cores.
**Pitfall (measurement):** The measurement uses Release, warm-up and the best of three runs (slide 21 notes). The exercise text adds that times in other configurations are unreliable (source: exercise instructions, not this slide).
**Demo (notes):** `PiDemo`, menu 5 (about 15 s). `tjedan2.html`, section 8.
**Link:** Exercise 2, Zadatak 2 measures the same S and S/k.

## Summary   [S22]
**What:** Four messages.
- Flynn: classify by the number of instruction and data streams. Your computer is MIMD with SIMD units.
- Granularity and overhead: work per task must be much larger than the overhead. If not, the parallel program is slower.
- Monte Carlo π: π ≈ 4K/N. Points are independent, so they split easily into tasks.
- Traps: a shared counter and a shared `Random` give a wrong π. The remainder N / k is lost. Fix: isolation and careful division.
**Questions for thought:** (1) Why is π with a shared `Random` not only wrong but also slower? (2) How many points does one task need to make the parallel version worth the overhead?
**Demo (notes):** The questions are at the bottom of `tjedan2.html`.
**Link:** Answer (1) is in "Trap 3". Answer (2) is in "Trap 1". The source says the parallel version is "not much faster" up to N = 10.000 and 3 to 7 times faster from N = 100.000. The source gives no exact threshold. Criterion used here: a clear gain (about 3 times) appears at N = 100.000 for k = 12 (inferred).

## References   [S23]
- [1] M. J. Flynn: Very high-speed computing systems. Proceedings of the IEEE 54(12), 1966, 1901-1909.
- [2] M. J. Flynn: Some Computer Organizations and Their Effectiveness. IEEE Trans. Computers C-21(9), 1972, 948-960.
- [3] Microsoft Learn: Use SIMD and hardware intrinsics in .NET (learn.microsoft.com/dotnet/standard/simd).
- [4] NVIDIA: CUDA Programming Guide, SIMT and warp (docs.nvidia.com/cuda).
- [5] T. Dadić: Flynnova taksonomija, lecture, 2025/26.
- [6] LLNL HPC: Introduction to Parallel Computing Tutorial (hpc.llnl.gov/documentation/tutorials).
- [7] T. Dadić: Računanje broja Pi Monte Carlo pristupom, 2025/26.
- [8] S. Weinzierl: Introduction to Monte Carlo methods, arXiv:hep-ph/0006269, 2000.
- [9] Microsoft Learn: System.Random, thread safety.
- [10] Microsoft Learn: Random.Shared.
Notes: the sources are Flynn's papers of 1966 and 1972, Microsoft and NVIDIA documentation, LLNL, and the lectures of Dr. Tonći Dadić. This week the exercise does the same on a sum of numbers: k tasks, speedup measurement and the traps.

## Exercise cross-reference
All tasks are in `exercises/02-parallel/` (instructions: `Vjezba2_upute.html`; code: `Zadatak*/Zadatak*.cs`).

| Task | Concept tested | Key insight of the solution | If the student ignores the concept |
|---|---|---|---|
| `Zadatak1_Paralelno/Zadatak1_Paralelno.cs` | MIMD: two different jobs (`f` sums, `g` counts primes) on two tasks | `Paralelno<T,U>` starts `Task.Run(f)` and `Task.Run(g)`, then reads both `Result` values. `Result` blocks until the tasks finish. Time is about max(tf, tg). Maximum speedup is 2, limited by the longer job and by the number of cores. Sample: f = 4500000001500000000, g = 216816, in 1056 ms sequential and 833 ms parallel. The parallel result equals the sequential result (slide 11: same result as sequential). | If `Result` is read between the two `Task.Run` calls, f and g run one after the other (inferred). A name `Parallel` clashes with the class `System.Threading.Tasks.Parallel` (task text). |
| `Zadatak2_Task/Zadatak2_Task.cs` | Task splitting, speedup S = T(slijedno)/T(k), efficiency S/k, overhead | Split [1, N] into k parts with `N / k`. `ToArray()` starts all tasks before the wait. Take the best of three runs. N = 10⁹ divides by 2, 4, 8, 16, so no remainder is lost here (inferred). The program prints `ispravan` or `POGREŠKA` by comparing each sum with the sequential sum (slide 20: always compare with sequential). A second variant, `ZbrojFor`, uses `Parallel.For(0, k, j => d[j] = Zbroji(...))` and `d.Sum()`. Each iteration writes its own element `d[j]`, so there is no data race (same idea as Zamka C, loop 1). Sample line in the code: `4 70 ms 3,66 0,91 ispravan`. S < k because of overhead, memory and shared cores. Sample S/k: 0,98 at k = 2, 0,33 at k = 16 (Task.Run). | A wrong split gives a wrong sum (correct is 500000000500000000). Times from a non-Release configuration are unreliable (exercise text). Without `ToArray()`, the lazy `Select` starts a task only when `Sum` reads it, so tasks run one by one (inferred from the code comment). |
| `Zadatak3_Zamke/Zadatak3_Zamke.cs`, Zamka A | Closure over the loop variable | Copy `i` to `int j` per iteration. Without it, all tasks read the final `i` (output `40 40 40 40`). | Wrong values with no error. |
| Same file, Zamka B | Shared counter (lecture trap 2) | `Interlocked.Increment(ref brojac)` gives 20000000 in about 80 ms. The unprotected version takes about 23 ms and loses about half of the updates. | Lost updates. Sample: 10238038 instead of 20000000. |
| Same file, Zamka C | Writes to separate elements versus writes to one variable | Loop 1 `b[i] = a[i] * 2` is always correct, because each iteration writes its own `b[i]`. Loop 2 `zbroj += a[i]` needs `Interlocked.Add`. | Loop 2 gives 148596 instead of 1000000 (sample). |
| Same file, Zamka D | Closure over `i` and shared `Random` (lecture trap 3) | Two errors, as the code comment states: closure over `i` gives the same Id (20 20 …), and `new Random()` created in the same millisecond gives the same sequences. The second claim holds for .NET Framework. Slide 19 notes say that current .NET does not do this (see Open points). Use `int j = i` and `Random.Shared`, which is safe for threads. Output must be 0 1 … 19, sorted by Id. | Duplicate Ids and equal random numbers. |

The exercise text says Zamka B and the protection mechanisms come in the data race lecture (week 4). The exercise goal is to see that correctness is not guaranteed. The slide link in S17 ("Zamka B in exercise 2") matches Zamka B.

## Open points
- S3, S5-S13: the figures are native shapes in the pptx, with no picture objects. The notes describe them and this file uses those descriptions.
- S16: the reference line in the chart goes from 0,16 at N = 100 to 0,000506 at N = 10.000.000. These values are about 1,6/√N, not exactly 1/√N (inferred). The source does not explain this.
- Exercise Zamka D comment says `new Random()` in the same millisecond gives equal sequences. Slide 19 notes say this is false in current .NET (seed from an internal generator) and true in old .NET Framework. The exercise targets .NET 10, so the comment is probably outdated (inferred). The source does not resolve this.
- Sequential time of π differs between slides: 998 ms (S16, S18 chart, S19 notes), 936 ms (S21), 927 ms for k = 1 (S21). The source does not explain this. Probably they come from different runs (inferred).
- S14 uses the median of 21 runs. S21 uses the best of three. The source states both and does not reconcile them.
- S10 says "i7-9750H: 6 cores". The source does not state the exact CPU for the S14 and S21 measurements, only "6 physical and 12 logical cores".
- S17: "about 90 % of increments are lost" comes from the slide. Zamka B in the exercise reports about half lost. The two cases differ (12 tasks versus 2 tasks).
- S23 reference [7] (Dadić, Monte Carlo π) is not cited on any slide text.

## Coverage ledger
| Slide | Topic section where it is covered | Items |
|---|---|---|
| 1 | Goals of the lecture | 6 (title, author, year, three themes, Monte Carlo as first program, demo) |
| 2 | Goals of the lecture | 5 (previous lecture, three goals, four traps list, exercise 2) |
| 3 | What a taxonomy classifies | 8 |
| 4 | The four classes | 6 |
| 5 | The four classes | 7 |
| 6 | SISD | 5 |
| 7 | SIMD | 10 |
| 8 | MISD | 4 |
| 9 | MIMD | 6 |
| 10 | Where is your computer | 8 |
| 11 | Task, communication, synchronization | 6 |
| 12 | Granularity | 5 |
| 13 | Overhead (tara) | 8 |
| 14 | Trap 1: tiny tasks | 9 |
| 15 | Monte Carlo π: the idea | 6 |
| 16 | Sequential program and accuracy | 8 (includes all 21 chart points) |
| 17 | Trap 2: shared counter | 8 |
| 18 | Trap 3: shared generator | 9 |
| 19 | Fix: isolation | 9 |
| 20 | Trap 4: lost remainder | 6 |
| 21 | Speedup | 10 (includes ideal line) |
| 22 | Summary | 6 |
| 23 | References | 10 references and closing note |
