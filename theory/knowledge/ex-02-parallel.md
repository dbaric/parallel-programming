# Exercise 2: Tasks and the First Traps of a Parallel Program (source: ex-02-parallel-deck.md, 18 slides; also README.md, Vjezba2_upute.html, Zadatak1..3 C# files)

## Map
- This file covers the intro deck of exercise 2 and its three assignments: `Paralelno<T, U>`, the sum on k tasks, and four traps.
- Main idea: a task (Task) is a unit of work, a thread (nit) is a worker. Tasks that write to shared data cause wrong results.
- The topics connect in this order: start tasks, divide a range, measure time, compute speedup, then find the cases where the result is wrong.
- Until lecture 4 the fix is to avoid sharing (own copy, own array element). Lecture 4 (`04-data-race-lock-interlocked.md`) adds lock and atomic operations. The solution code of trap B and C already uses `Interlocked`.
- Word use in this file: "job" means the work that a task does. "Part" means one piece of a divided range.
- Terms: "task" = a `Task` object (taskovi). "Assignment" = zadatak, one numbered job of the exercise (Assignment 1, 2, 3). "Trap" = zamka. "Speedup" = ubrzanje. "Efficiency" = učinkovitost.

## Course context and setup   [S1-S2]
**What:** Exercise 2 is in week 2. Authors: Marin Aglić Čuvić, based on lectures by Tonći Dadić. Academic year 2026/2027. Subtitle: "tasks and the first traps of a parallel program".
- Setup has three steps. Step 1: install the .NET 10 SDK from dotnet.microsoft.com/en-us/download. The command `dotnet --list-sdks` must print version 10.x.
- Step 2: select an environment. The choices are Visual Studio 2026 (.NET workload), JetBrains Rider, or Visual Studio Code with the C# Dev Kit extension.
- Step 3: open and run an assignment.
- Visual Studio and Rider: open `Vjezba2.sln`. Each assignment is a separate project. Select the assignment project (Set as Startup Project) and run with F5.
- VS Code: Open Folder, configuration "zadatak N". In a terminal: `dotnet run` in the assignment folder.
- Rider: select the assignment project in the run configuration list (README).
- VS Code (README): install the suggested extension C# Dev Kit (or C#). Press F5 and select the configuration "Vježba 2: zadatak N (Debug)".
- README structure: each assignment is a separate project in the solution `Vjezba2.sln`, so it runs alone, without a menu. The whole assignment, with the starting statements, is in one file. The instructions page names the layout: folder `ZadatakN_*` with file `ZadatakN_*.cs`.

| Assignment | Project folder | File |
|---|---|---|
| 1 | `Zadatak1_Paralelno/` | `Zadatak1_Paralelno/Zadatak1_Paralelno.cs` |
| 2 | `Zadatak2_Task/` | `Zadatak2_Task/Zadatak2_Task.cs` |
| 3 | `Zadatak3_Zamke/` | `Zadatak3_Zamke/Zadatak3_Zamke.cs` |

- The instructions page says: the project targets .NET 10 (LTS).
- Command line from the exercise folder: `dotnet run --project Zadatak1_Paralelno`. For measurements: `dotnet run -c Release` (README).
- VS Code: for timing (assignments 1 and 2) select the **Release** configuration (README).
- Tool detail from the notes: Visual Studio uses Release and Ctrl+F5. Rider uses the Release configuration in the top bar. VS Code runs the configuration "Release".
- The instructions page adds: in Visual Studio and Rider select the configuration Release. In VS Code select the launch configuration "(Release)". On the command line use `dotnet run -c Release`.
- Only Visual Studio 2022? In `Directory.Build.props` change `net10.0` to `net8.0`. Also change it in `.vscode/launch.json` (in the program paths). The assignments work the same.
- `Directory.Build.props` is the shared file for all assignment projects: `OutputType` Exe, `TargetFramework` `net10.0`, `ImplicitUsings` enable, `Nullable` enable.
**Why:** Each assignment is a separate project in the same solution (since 12.10.2026, as in 2025/26). `README.md` has the steps. The `.vscode` folder has a Debug and a Release configuration for each assignment.
**Conclusion:** Measure time only in the Release configuration. Debug is much slower and not reliable (see slide 8 below).
**Pitfall:** The notes say: check that the classroom has the .NET 10 SDK. If not, use `net8.0`.
**Link:** Time measurement section below.

## Sources and history of the assignments   [S1]
**What (notes):** Assignment 1 (Paralelno) and trap D (Entitet) come from exercise 2 of 2020/21 (PP_20-21/Vj02). Assignments 2 and 3 (A-C) are the former assignments 6 and 7 of exercise 1. The old assignments with `CancellationToken` (Periodicno) move to exercise 5.
- Related material: `Vizualizacije/zamke.html`, `Tjedan 01 - Uvod/Vizualizacije/amdahl.html`. Instructions file: `Vjezba2_upute.html`.
- Before the exercise, the student reads the presentation for exercise 2 and `zamke.html`.
- Grading (instructions page): the exercise counts as attendance (at least 70 % of the exercise terms). Two homework assignments receive grades. Details come later.
**Link:** `ex-05-periodic-tasks.md` for the `CancellationToken` assignments.

## Plan of the exercise   [S3]
**What:** Three goals: run (Pokrenuti) work on more tasks and wait for the result; measure (Izmjeriti) how much faster the program is and whether it pays off; recognize (Prepoznati) the traps that cause a wrong result.
- Five things are needed: Task, time measurement, range division, `Parallel.For`, and the term data race (utrka za podatke).
**Conclusion:** The deck covers the five things in this order.

## Thread, ThreadPool, Task   [S4]
**What:**
| Term | Meaning |
|---|---|
| Thread (nit) | A thread with its own stack. Each new thread has a creation cost. Own thread is rarely needed, for example for a long blocking job [5]. |
| ThreadPool (fond niti) | A finished thread returns to the pool and is used again. `QueueUserWorkItem` returns no result. It works "under" tasks [5]. |
| Task | A job that runs on a pool thread. It has a result (`Result`) and waiting (`WaitAll`). Use `Task.Run` [6]. |
**Why (notes):** Microsoft [5]: pool threads return to a queue after the job and run again. Quote: "This reuse enables applications to avoid the cost of creating a new thread for each task". The TPL types `Task` and `Task<TResult>` use pool threads by default. An own thread is useful for work that blocks for a long time.
**Conclusion:** A task is the job and a thread is the worker. Many tasks can pass in turn through the same thread.
**Conclusion (2):** `Task.Run` is the recommended way to create and start a task [6]. `Task.Factory.StartNew` and `new Task(...).Start()` exist, but the course does not use them.
**Link:** The notes take slides 5-8 from exercise 2 of 2020/21. Interactive view: `Tjedan 01 - Uvod/Vizualizacije/paralelizam.html`, section 1 (more threads than cores, thread pool). Lecture: `02-flynn-granularnost-monte-carlo-pi.md` (task as MIMD work); `05-modeli-racunala-i-async-await.md` (async and thread pool).

## Task.Run and Result   [S5]
**What:** `Task.Run` takes a delegate (lambda) and starts it on a thread from the thread pool. The call returns at once. The job runs in the background. `Task<long>` is a promise of a result of type `long`. `t.Result` waits until the task ends, then returns the result.
**Why:** The lambda `() => Zbroji(1, 100)` is the same concept as in exercise 1: a function passed as an argument.
**Example:**
```csharp
Task<long> t = Task.Run(() =>
    Zbroji(1, 100));

long r = t.Result;
```
**Notes:** `Task<T>` is for a job that returns a value. `Task` (without T) is for a job without a result.
**Link:** `ex-01-delegates.md` (delegate and lambda). Interactive view: `paralelizam.html`, section 1.

## Waiting for many tasks   [S6]
**What:** `Task.WaitAll(a, b)` waits until all named tasks end. Both tasks run at the same time. Read the results only when the tasks are done. A task without a result has only `WaitAll`, without `Result`. The slide says "Task bez rezultata (Action)": the `Action` delegate (a function without a result) creates a `Task` without T (see Open point 9).
**Example:**
```csharp
var a = Task.Run(() => Zbroji(1, 50));
var b = Task.Run(() => Zbroji(51, 100));

Task.WaitAll(a, b);
long zbroj = a.Result + b.Result;
```
**Why (notes):** `WaitAll` and `Result` are used in all three assignments. Interactive view: `Vizualizacije/tjedan2.html`, section 3 (synchronization: B waits for the result of A).
**Link:** Exercise assignments 1, 2 and 3 below.

## Division of a range   [S7]
**What:** Divide the sum of the numbers 1 to N into k equal parts. Example: N = 12, k = 3.
- part 0: from 1 to 4
- part 1: from 5 to 8
- part 2: from 9 to 12
**Example:**
```csharp
long velicina = N / k;
long od = j * velicina + 1;
long do_ = (j + 1) * velicina;
```
`j` is the index of the part (0 ... k - 1). `N / k` is integer division: the remainder is discarded.
**Pitfall:** For N = 14 and k = 3 the parts cover only 12 numbers. The slide asks: what happens to the two numbers that remain? (The slide gives no answer. The trap is "lost remainder", `Vizualizacije/zamke.html`, section 4.)
**Why:** Integer division gives a size that is too small when N is not a multiple of k (inferred). In this exercise N = 10^9, and 2, 4, 8, 16 divide 10^9 without remainder, so no number is lost (inferred).
**Link:** `02-flynn-granularnost-monte-carlo-pi.md` (trap "lost remainder").

## Time measurement   [S8]
**What:** `Stopwatch` measures elapsed milliseconds. The number of cores is in `Environment.ProcessorCount`.
**Example:**
```csharp
var sat = Stopwatch.StartNew();
long zbroj = Zbroji(1, N);
sat.Stop();
Console.WriteLine(sat.ElapsedMilliseconds);
```
**Why:** Release is the correct configuration because Debug is much slower and not reliable.
**Conclusion:** Repeat the measurement a few times. The first run is often slower.
**Link:** Interactive view: `tjedan2.html`, section 8. Reference [3] (Microsoft Learn, Stopwatch) probably belongs here (inferred; the slide has no citation).

## Speedup and efficiency   [S9]
**What:**
- S = T(sequential) / T(k tasks). S shows how many times the program is faster.
- E = S / k.
- Ideal: S = k and E = 1 (100 %).
**Why:** The real S is smaller than k, because work is divided, and tasks start and wait. When k is larger than the number of cores, S does not grow any more.
**Example:** Sequential 256 ms, 4 tasks 70 ms. S = 256 / 70 ≈ 3,66. E = 3,66 / 4 ≈ 0,91.
**Notes:** The numbers are measured from the solution of assignment 2 (Release, i7-9750H, 6 cores / 12 logical, 5.10.2026). Interactive views: `Tjedan 01 - Uvod/Vizualizacije/amdahl.html`; `zamke.html`, section 2 (granularity); `tjedan2.html`, section 8 (measured and ideal speedup).
**Link:** `03-amdahlov-zakon-i-modeli-programiranja.md` (Amdahl's law limits S); `02-flynn-granularnost-monte-carlo-pi.md` (granularity and overhead).

## Parallel.For   [S10]
**What:** `Parallel.For(0, n, i => { ... })`. The first argument is the start (included). The second is the end (excluded). The body is a lambda with parameter `i`. Iterations run on several threads, so the order is not guaranteed. The call ends only when all iterations are done, so the program can read the results after the loop.
**Example:**
```csharp
Parallel.For(0, n, i =>
{
    // tijelo za svaki i
});
```
Comparison, the normal loop (iterations run one after the other):
```csharp
for (int i = 0; i < n; i++)
    // tijelo za svaki i
```
**Why (notes):** `Parallel.For` is a higher-order function (it takes a delegate). `Task.Run` is the same case.
**Link:** Reference [2] (Microsoft Learn, data parallelism) probably belongs here (inferred; the slide has no citation). Interactive view: `zamke.html`, section 3.

## Data race   [S11]
**What:** `brojac++` looks like one statement, but it has three steps: read, increase, write.
| Task A | Task B |
|---|---|
| reads brojac: 5 | reads brojac: 5 |
| computes 5 + 1 = 6 | computes 5 + 1 = 6 |
| writes 6 | writes 6 |
**Why:** Both tasks read 5 before anyone wrote a result. After two increases the counter is 6, not 7. One increase is lost. The result depends on how the steps interleave, so it changes from run to run.
**Conclusion:** A data race (utrka za podatke) gives a result that changes between runs.
**Notes:** Only explain the effect here. The protection tools (lock, atomic operations, semaphores) come in the lecture on data race (week 4). Until then the fix is to avoid sharing.
**Link:** `04-data-race-lock-interlocked.md`. Interactive views: `tjedan2.html`, section 6 (step by step); `zamke.html`, section 3.

## When Parallel.For is safe   [S12]
**What:** The question before parallelization: may the iterations run at the same time and in any order?
| Independent iterations | Dependent iterations |
|---|---|
| Each iteration writes to its own element. | All iterations read and write the same variable: data race. |
**Example:**
```csharp
Parallel.For(0, n, i =>
{
    b[i] = a[i] * 2;
});
```
```csharp
long zbroj = 0;
Parallel.For(0, n, i =>
{
    zbroj += a[i];
});
```
**Conclusion:** The first loop is safe because "nobody disturbs anybody". The second has a data race.
**Link:** Exercise assignment 3, trap C. Interactive view: `zamke.html`, section 3.

## Local variable and result array   [S13]
**What:** A variable declared inside the lambda is separate for each iteration. The array has one element for each part, so nobody writes into the element of another part. Add the results after the loop with `Sum()` or a normal loop. `od(j)` and `do_(j)` are the limits of part j, computed as on slide 7.
**Example (a different problem: count even numbers in an array, by parts):**
```csharp
var rez = new int[k];
Parallel.For(0, k, j =>
{
    int brojim = 0;          // lokalna za ovaj dio
    for (int i = od(j); i < do_(j); i++)
        if (niz[i] % 2 == 0) brojim++;
    rez[j] = brojim;         // piše u svoj element
});
int ukupno = rez.Sum();
```
**Why (notes):** The teacher chose another problem on purpose, so the deck gives no ready solution of the assignments. `Sum()` is a LINQ method (`using System.Linq;` is implicit in the project).
**Conclusion:** Isolation (a local variable plus one array element per part) removes the data race without a lock.
**Link:** Assignment 2 solution (`d[j]`). Interactive views: `zamke.html`, section 3 (solution); `tjedan2.html`, section 5 (isolation).

## References   [S18]
**What:** The deck lists seven Microsoft Learn references.
1. [1] Task.Run: learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run
2. [2] Data parallelism (TPL): learn.microsoft.com/en-us/dotnet/standard/parallel-programming/data-parallelism-task-parallel-library
3. [3] Stopwatch: learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch
4. [4] foreach and variable capture: learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements
5. [5] The managed thread pool: learn.microsoft.com/en-us/dotnet/standard/threading/the-managed-thread-pool
6. [6] Task-based asynchronous programming: learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming
7. [7] Random constructor, default seed: learn.microsoft.com/en-us/dotnet/api/system.random.-ctor

## Exercise assignments   [S14-S17]

### Assignment 1: Paralelno<T, U>   (`exercises/02-parallel/Zadatak1_Paralelno/Zadatak1_Paralelno.cs`)   [S14]
**Assignment statement:** The code gives function f (sum of 1 to 3·10^9, type `long`) and function g (count of primes up to 3·10^6, type `int`) and their sequential call with timing. The student does three things.
1. Write the method `static (T, U) Paralelno<T, U>(Func<T> f, Func<U> g)`. It runs f and g at the same time, each in its own task, and returns both results.
2. Call it with f and g. The results must equal the sequential results.
3. Compare the time with the sequential time. What is the maximum speedup for two jobs, and what does it depend on?
- Do not name the method `Parallel`. This is the name of a class in `System.Threading.Tasks`.
- Example output (Release, 6 cores, 12 logical):
```
Slijedno:   f = 4500000001500000000, g = 216816  (1687 ms)
Paralelno:  f = 4500000001500000000, g = 216816  (831 ms)
```
- Times depend on the computer. The results of f and g must be the same.
**Concept tested:** Task parallelism (paralelizam zadataka): two different operations. Assignment 2 tests data parallelism (paralelizam podataka). `Task.Run`, `Result`, generic method with `Func<T>` (inferred).
**Solution idea:** Start both jobs with `Task.Run`, then read both `Result` values.
```csharp
static (T, U) Paralelno<T, U>(Func<T> f, Func<U> g)
{
    var tf = Task.Run(f);   // f ide na nit iz fonda
    var tg = Task.Run(g);   // g ide istodobno na drugu nit
    return (tf.Result, tg.Result);   // Result blokira dok taskovi ne završe; vraća (rf, rg)
}
```
Helper functions in the file (comments: `Zbroji` sums the numbers in [od, do]; `BrojProstih` counts the primes in [2, n]):
```csharp
static long Zbroji(long od, long @do)
{
    long s = 0;
    for (long i = od; i <= @do; i++) s += i;
    return s;
}

static int BrojProstih(int n)
{
    int broj = 0;
    for (int x = 2; x <= n; x++)
    {
        bool prost = true;
        for (int d = 2; (long)d * d <= x; d++)
            if (x % d == 0) { prost = false; break; }
        if (prost) broj++;
    }
    return broj;
}
```
Calls: `Func<long> f = () => Zbroji(1, 3_000_000_000L);` and `Func<int> g = () => BrojProstih(3_000_000);`. The sequential part uses `Stopwatch.StartNew()`, `sat.Stop()`, then `sat.Restart()` for the parallel part: `var (pf, pg) = Paralelno(f, g);`.
**Reasoning:** Both `Task.Run` calls return at once, so f and g run at the same time. Only then does `Result` block. If the code called `.Result` right after the first `Task.Run`, the jobs would run in sequence (inferred). The parallel time is about max(tf, tg), not tf + tg. The code comment says the speedup is at most 2 for two jobs, limited by the longer job and by the number of cores. The slide output gives 1687 / 831 ≈ 2,03 (see Open point 1). The code comment says the parallel result is 833 ms ≈ max(tf, tg).
**Pitfall:** (1) Naming the method `Parallel` clashes with the class `Parallel`. (2) (inferred) The speedup is lower when one job is much longer, because the longer job sets the total time. The comment states the limit as "longer job and number of cores". (3) `(long)d * d` in `BrojProstih` can avoid `int` overflow in the test `d * d <= x` for larger n (inferred). No overflow occurs with n = 3·10^6.
**Link:** `03-amdahlov-zakon-i-modeli-programiranja.md` (limit on speedup); `01-uvod-u-paralelno-programiranje.md` (task versus data parallelism). Interactive view: `paralelizam.html`, section 3 (data or tasks).

### Assignment 2: sum on k tasks   (`exercises/02-parallel/Zadatak2_Task/Zadatak2_Task.cs`)   [S15]
**Assignment statement:** The code gives a sequential and a one-task sum of the numbers 1 to 10^9. The student does three things.
1. Divide the work into 2, 4, 8 and 16 tasks. Check that the sum is always the same.
2. Print the number of cores (`Environment.ProcessorCount`). For each k take the shortest of three measurements. Compute S = T(sequential) / T(k tasks) and efficiency S / k.
3. Do the same with `Parallel.For`. Why is S not equal to k?
- The sum must always be 500000000500000000. The exact values in the example need not match, but the form of the result must.
- Example output (Release, 6 cores, 12 logical):
```
Jezgri (logičkih): 12
Slijedno:   500000000500000000  (256 ms)
1 task:     500000000500000000  (259 ms)

Task.Run
k     vrijeme     S     S/k   zbroj
2      131 ms    1,95   0,98   ispravan
4       70 ms    3,66   0,91   ispravan
8       46 ms    5,57   0,70   ispravan
16      49 ms    5,22   0,33   ispravan

Parallel.For
k     vrijeme     S     S/k   zbroj
2      127 ms    2,02   1,01   ispravan
4       64 ms    4,00   1,00   ispravan
8       46 ms    5,57   0,70   ispravan
16      51 ms    5,02   0,31   ispravan
```
**Concept tested:** Data parallelism: range division, `Task.Run` and `Parallel.For`, `Stopwatch`, speedup and efficiency, the limit by core count.
**Solution idea:**
```csharp
static long ZbrojTask(int k) =>
    Enumerable.Range(0, k).Select(j => Task.Run(() => Zbroji(j * (N / k) + 1, (j + 1) * (N / k)))).ToArray().Sum(z => z.Result);

static long ZbrojFor(int k)
{
    var d = new long[k];
    Parallel.For(0, k, j => d[j] = Zbroji(j * (N / k) + 1, (j + 1) * (N / k)));
    return d.Sum();
}

static double Najkraće(Action a) => Enumerable.Range(0, 3).Min(_ => { var sat = Stopwatch.StartNew(); a(); return sat.Elapsed.TotalMilliseconds; });
```
The main loop runs the same code for both variants. It loops over an array of pairs (name, method), and over `k` in `{ 2, 4, 8, 16 }`:
```csharp
foreach (var (naziv, zbrojK) in new (string, Func<int, long>)[] { ("Task.Run", ZbrojTask), ("Parallel.For", ZbrojFor) })
{
    Console.WriteLine($"\n{naziv}\nk     vrijeme     S     S/k   zbroj");
    foreach (int k in new[] { 2, 4, 8, 16 })
    {
        long z = 0;
        double t = Najkraće(() => z = zbrojK(k));
```
The timed lambda stores the sum in `z`. The row then compares `z` with the sequential sum `zbroj`. The program prints each row with `$"{k,-4} {t,6:F0} ms {ts / t,5:F2} {ts / t / k,6:F2}   {(z == zbroj ? "ispravan" : "POGREŠKA")}"`. `ts` is `Najkraće(() => Zbroji(1, N))`. The one-task test uses `Task<long> jedan = Task.Run(() => Zbroji(1, N)); long r1 = jedan.Result;`. The code has `const long N = 1_000_000_000L;`.
**Reasoning:**
- `ToArray()` starts all tasks before the first wait. `Select` is lazy. Without `ToArray()`, `Sum(z => z.Result)` would start one task, wait for it, then start the next one. The tasks would then run in sequence (inferred from the code comment "ToArray pokreće sve taskove prije čekanja").
- In `ZbrojFor` each iteration writes its own element `d[j]`. This is the isolation from slide 13. `d.Sum()` runs after the loop, when all parts are done.
- The 1-task time (259 ms) is about the sequential time (256 ms). One task gives no speedup.
- S is not equal to k because of overhead (režija), memory and shared cores (code comment: "S < k zbog režije, memorije i dijeljenih jezgri"). Slide 9 adds: tasks start and wait, and for k above the number of cores S stops growing.
- The table shows this: S grows to 5,57 at k = 8 and falls to 5,22 at k = 16. The machine has 6 physical and 12 logical cores. Efficiency S/k falls to 0,33 at k = 16.
**Pitfall:** (1) Taking one measurement instead of the shortest of three: the first run is often slower. (2) Measuring in Debug. (3) (inferred) A wrong division gives a wrong sum. The check `z == zbroj` prints "POGREŠKA" in that case. (4) `N / k` is an integer division. It is safe here because k divides 10^9.
**Link:** `03-amdahlov-zakon-i-modeli-programiranja.md`; `02-flynn-granularnost-monte-carlo-pi.md` (granularity, same measurement on π). Interactive views: `zamke.html`, sections 2 and 4; `tjedan2.html`, section 8.

### Assignment 3: four traps   (`exercises/02-parallel/Zadatak3_Zamke/Zadatak3_Zamke.cs`)   [S16-S17]
**Assignment statement:** The code gives four programs that compile but do not do what the student expects. For each: run the program several times, explain the output, fix the code.
- Trap A: tasks in a `for` loop compute `i * 10`.
- Trap B: two tasks increase the same counter 10 million times each. Write down how many updates are lost. Fix it and measure the time of both versions.
- Trap C: compare `b[i] = a[i] * 2` with `zbroj += a[i]`. Which loop is always correct, and why? Fix the other loop.
- Trap D: create 20 objects `Entitet` with a unique Id (0 to 19) and a random number. Print them in order of Id. The program has two errors.
- Header comment of the file: all programs compile and run without error, but they do not give the expected result. The aim is to see the problem, explain it, and fix it.
- The aim is not to learn all of synchronization today. The aim is to see that correctness is not guaranteed. Protection tools come in the lecture on data race (week 4).
- The slide asks about the numbers: "Why do the left numbers (the "Given code" column of the table) change at each run? And why is B slower without the fix?"
**Output of the given code (instructions page):**
```
Zamka A   Očekivano: 0 10 20 30
          Dobiveno:  40 40 40 40
Zamka B   Očekivano: 20000000
          Dobiveno:  10238038  (22,6 ms)
Zamka C   Petlja 1, b[i] = a[i] * 2: zbroj = 2000000, očekivano 2000000
          Petlja 2, zbroj += a[i]:   zbroj = 148596, očekivano 1000000
Zamka D   Očekivano: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19
          Dobiveno:  20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20
```
The numbers in the "Dobiveno" rows are examples. After the fix the output must equal the "Expected" rows.
**Slide 17 comparison (one run of the solution, Release, i7-9750H, 5.10.2026):**
| Trap | Given code | After the fix |
|---|---|---|
| A | 40 40 40 40 | 0 10 20 30 |
| B | 10070437 (21,9 ms) | 20000000 (3,0 ms) |
| C | loop 2: zbroj = 86391 | loop 2: zbroj = 1000000 |
| D | 20 20 20 ... 20 | 0 1 2 ... 19 |
Notes: in four runs, B gave 10,06 to 10,24 million and C gave 86 000 to 161 000. B without the fix is about 7 times slower because both tasks write all the time to the same memory location. More detail in the lecture on data race (week 4).
**File structure (all three files):** Each file starts with top-level statements: `Console.OutputEncoding = System.Text.Encoding.UTF8;` and a call such as `Zadatak3_Zamke.Pokreni();`. The code is in a `static class` with a public method `Pokreni`, which holds the run steps. The source gives no reason for `OutputEncoding`. It probably makes the Croatian characters print correctly (inferred). `Pokreni` of assignment 3 calls `ZamkaA();` to `ZamkaD();` in this order.
**Header comments of the files:** Assignment 1 and 2: "VAŽNO: vrijeme mjerite u Release konfiguraciji (Build > Configuration: Release, Ctrl+F5)." Assignment 2 adds: "Task.Run prima delegat (lambdu) i izvodi ga na niti iz fonda niti." Assignment 3: "Svi programi se prevedu i pokrenu bez greške, ali ne daju ono što očekujete. Cilj: uočiti problem, objasniti ga i popraviti."
**Common class:**
```csharp
class Entitet
{
    public int Id { get; }
    public int Br { get; }

    public Entitet(int id, int br)
    {
        Id = id;
        Br = br;
    }
}
```

#### Trap A: closure over the loop variable
**Concept tested:** Closure (hvatanje varijable). A lambda captures the variable, not its value.
**Solution idea:**
```csharp
for (int i = 0; i < 4; i++)
{
    int j = i;   // vlastita kopija po iteraciji; lambda je prije hvatala zajedničko i (koje je na kraju 4)
    zadaci.Add(Task.Run(() => j * 10));   // 0 10 20 30
}
```
The result is printed with `string.Join(" ", zadaci.Select(z => z.Result))`. The tasks go into `var zadaci = new List<Task<int>>();`. The program first prints `"Zamka A   Očekivano: 0 10 20 30"` and then the line "Dobiveno".
**Reasoning:** The `for` loop has one variable `i` for all iterations. The tasks start later and read `i` when the loop is already finished. Then `i` is 4, so each task computes 4 * 10 = 40. The local `j` is a new variable in each iteration, so each task keeps its own value.
**Pitfall:** The output "40 40 40 40" changes with timing: a task that starts early can read a smaller `i` (inferred; the instructions say the printed numbers change between runs). Reference [4] (variable capture in `foreach`) probably belongs here (inferred; no citation in the source).
**Link:** `ex-01-delegates.md` (lambda and captured variables). Interactive views: `zamke.html`, section 1; `tjedan2.html`, section 6.

#### Trap B: lost updates on a shared counter
**Concept tested:** Data race on `brojac++` (read, increase, write; see slide 11). Cost of protection.
**Solution idea:**
```csharp
const int koraka = 10_000_000;
long brojac = 0;
var a = Task.Run(() => { for (int i = 0; i < koraka; i++) Interlocked.Increment(ref brojac); });
var b = Task.Run(() => { for (int i = 0; i < koraka; i++) Interlocked.Increment(ref brojac); });
Task.WaitAll(a, b);
```
The code comment: atomic increase, no lost updates. `brojac = 20000000`. The program starts `var sat = Stopwatch.StartNew();` before the tasks and `sat.Stop();` after `Task.WaitAll(a, b);`. It prints `$"Zamka B   Očekivano: {2L * koraka}"` and then the counter with the time `sat.Elapsed.TotalMilliseconds:F1`. The format `F1` gives one decimal place (for example 22,6 ms). The comment gives about 80 ms for the fixed version against about 23 ms for the unprotected one, which loses about half of the updates.
**Reasoning:** When two tasks read the same value, both write value + 1, and one update is lost. About half of 20 000 000 is lost (given runs: 10 238 038, 10 070 437). `Interlocked.Increment` is an atomic increase (code comment). It probably does read, increase and write as one step, so no update is lost (inferred). See Open point 2 for the conflict in the timings.
**Pitfall:** The deck notes say the unprotected version is about 7 times slower (21,9 ms against 3,0 ms) because both tasks write all the time to the same memory location. The code comment says the opposite order (80 ms fixed against 23 ms unprotected). See Open points.
**Link:** `04-data-race-lock-interlocked.md` (`Interlocked`, lock).

#### Trap C: independent versus dependent iterations
**Concept tested:** Which `Parallel.For` loop is safe (slide 12).
**Solution idea:**
```csharp
const int n = 1_000_000;
var a = new int[n];
Array.Fill(a, 1);
var b = new int[n];

Parallel.For(0, n, i => { b[i] = a[i] * 2; });

long zbroj = 0;
Parallel.For(0, n, i => Interlocked.Add(ref zbroj, a[i]));   // atomarno zbrajanje; zbroj = 1000000
```
The check for loop 1 uses `b.Sum(x => (long)x)` and expects `2L * n` = 2000000.
**Reasoning:** Loop 1 is always correct: each iteration writes its own `b[i]`. Loop 2 has all iterations write the same variable `zbroj`, which is a data race. `Interlocked.Add` makes the addition atomic. The comment in the code states: loop 1 is always correct because each iteration writes its own `b[i]`.
**Pitfall:** `zbroj += a[i]` gives 148596 or 86 000 to 161 000 instead of 1000000 (given runs). The sum of `b` uses `(long)x`. This can avoid `int` overflow for larger n (inferred). No overflow occurs with n = 1 000 000. An alternative fix without atomic operations is the isolation of slide 13: a local sum and one array element per part, then `Sum()` (inferred from slide 13; the code does not use it).
**Link:** Slides 12 and 13 above. `04-data-race-lock-interlocked.md`.

#### Trap D: two errors in the creation of Entitet objects
**Concept tested:** Closure (error 1), shared random generator and print order (error 2). Waiting for all tasks.
**Solution idea:**
```csharp
var zadaci = new List<Task<Entitet>>();
for (int i = 0; i < 20; i++)
{
    int j = i;   // greška 1: zatvaranje nad i, svaki Entitet je dobio isti Id
    zadaci.Add(Task.Run(() => new Entitet(j, Random.Shared.Next(100))));   // greška 2: new Random() u istoj milisekundi daje iste nizove; Random.Shared je siguran za niti
}
Task.WaitAll(zadaci.ToArray());
Console.WriteLine(string.Join(" ", zadaci.Select(z => z.Result).OrderBy(e => e.Id).Select(e => e.Id)));
```
The program prints the expected line first, then `Console.Write("          Dobiveno:  ")` before it starts the tasks. The expected line is built with `string.Join(" ", Enumerable.Range(0, 20))`. The final comment: the order is by Id, not by the end of the task.
**Reasoning:** Error 1 is the closure over `i` (all Id = 20, same as trap A). Error 2 is the print order. The notes say the second error shows only after the first is fixed. (inferred) Tasks can end in any order, so a print in order of completion can give a random order. The code sorts with `OrderBy(e => e.Id)`. The list `zadaci` keeps the creation order, so the sort is not strictly needed in the shipped code.
**Pitfall:** The 2020/21 error with `new Random()` in a loop does not appear on .NET 10. Since .NET Core the default seed does not come from the clock [7]. The code comment still says `new Random()` in the same millisecond gives equal sequences, and `Random.Shared` is thread safe. The notes and the code comment disagree on the cause of error 2 (see Open points).
**Link:** `02-flynn-granularnost-monte-carlo-pi.md` (shared `Random` trap). The notes say trap D (order) has no interactive view yet. Views for the other traps: `zamke.html`, sections 1 and 3; `tjedan2.html`, section 6.

## Open points
1. Slide 14 and the instructions page differ from the code comments in the numbers. Assignment 1: slide and page show 1687 ms sequential and 831 ms parallel. The code comment shows 1056 ms and 833 ms. Different runs (inferred). Also, 1687 / 831 ≈ 2,03 is above the stated maximum speedup of 2. The comment values give 1056 / 833 ≈ 1,27. The source does not explain the 2,03.
2. Trap B timing: the code comment says about 80 ms (fixed) against about 23 ms (unprotected). Slide 17 shows 21,9 ms (unprotected) and 3,0 ms (after the fix), and the notes say the unprotected version is about 7 times slower. The two sources conflict on which version is faster. The slide data comes from a different fix (inferred: perhaps one counter per task) but the source does not say. The slide does not show the fix.
3. Trap D error 2: the notes say the second error is the print order. The code comment says the second error is `new Random()` in the same millisecond. The notes add that this error does not appear on .NET 10. Both are recorded above.
4. Slide 7 asks "what to do with the two remaining numbers?" and gives no answer.
5. Slide 18 lists references [1]-[4], but the deck cites only [5], [6] on slide 4 and [7] in the notes of slide 16. References [1]-[4] have no citation point. The .pptx has no pictures, so this is a gap in the source. The links to [2], [3], [4] in this file are inferred.
6. Slides 2 and 3 use a layout of boxes. The text export can lose the order of the boxes. The match of text to boxes follows the reading order only.
7. The instructions page says "10^9" and "3·10^9" with superscripts. The code uses `1_000_000_000L` and `3_000_000_000L`.
8. The deck says "i7-9750H, 6 cores / 12 logical". Results on other machines differ.
9. Slide 6 says "Task bez rezultata (Action)". `Action` is a delegate type, not a task type. The file uses the term `Task` without T.
10. Slide 17 notes say the full output is in `Vjezba2_upute.html`. The Assignment 3 output above comes from that page.

## Coverage ledger
| slide | topic section where it is covered | items |
|---|---|---|
| 1 | Course context; Sources and history | 9 (title, author, year, sources of assignments, moved assignments, instruction file, visual aids) |
| 2 | Course context and setup | 18 (3 steps, SDK check, 3 IDEs, VS/Rider run, VS Code run, Release, net8.0 fallback, notes) |
| 3 | Plan of the exercise | 4 (3 goals, five things) |
| 4 | Thread, ThreadPool, Task | 9 (3 table columns, quote, StartNew note, analogy, visual aid) |
| 5 | Task.Run and Result | 6 (code, 4 rules, T versus no T) |
| 6 | Waiting for many tasks | 5 (code, WaitAll, Result rule, Action rule, notes) |
| 7 | Division of a range | 7 (example 12/3, code, integer division, N = 14 question) |
| 8 | Time measurement | 6 (code, Stopwatch, Release, ProcessorCount, repeat, IDE notes) |
| 9 | Speedup and efficiency | 7 (S, E, ideal, why smaller, example, measured source, views) |
| 10 | Parallel.For | 7 (code, argument rules, order, end of call, comparison loop, higher-order note) |
| 11 | Data race | 6 (three steps, table, lost update, non-determinism, notes on scope) |
| 12 | When Parallel.For is safe | 5 (two code blocks, question, conclusion) |
| 13 | Local variable and result array | 6 (code, local variable, array, Sum after loop, od/do_, LINQ note) |
| 14 | Exercise assignments: Assignment 1 | 14 (statement, 3 steps, naming rule, output, concepts, notes) |
| 15 | Exercise assignments: Assignment 2 | 14 (statement, 3 steps, both tables, check rule, foreach over (name, method) pairs, `z` set in timed lambda) |
| 16 | Exercise assignments: Assignment 3 | 13 (4 traps, aims, repeat rule, notes on D and Random, file structure with `Pokreni` and `OutputEncoding`, header comments, `F1` format) |
| 17 | Exercise assignments: Assignment 3, slide 17 comparison | 8 (expected line, two columns, questions, notes numbers) |
| 18 | References | 7 |
