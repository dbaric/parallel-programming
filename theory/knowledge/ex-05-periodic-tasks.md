# Exercise 05: Cancelling Tasks and Periodic Tasks (source: ex-05-periodic-tasks-deck.md, README.md, Vjezba5_upute.html, Zadatak1..5 C# files; 14 slides)

## Map
- This exercise teaches how to stop a task on request, how to repeat work at fixed intervals, and how to return the result of a stopped task.
- Main idea: a task cannot be stopped from outside. The caller makes a cancellation request, and the task must check it and finish.
- Terms: "cancel" is the action, and "stop" is its short form for the same action (source word: "prekid"). A "cancellation request" is the signal that `Cancel()` sets. `Canceled` (capital C) is only the task state. The file does not use "give up" or other names for it.
- Topics connect in a chain. The token (Topic 1) drives the waiting calls (Topic 2). The waiting calls decide how late a stop is. The task state (Topic 3) shows if the stop was real.
- Tasks 2 and 3 use the same schedule rule (Topic 5). Task 4 uses a local list to avoid a data race (Topic 6). Task 5 uses a Barrier with a token (Topic 7).
- Each pitfall has the same method: measure or show the error first, then fix it (Topic 4).

## Setup and tools   [S2, README]
**What:** The exercise needs the .NET 10 SDK. Check with `dotnet --list-sdks`; the output must show version 10.x.
- IDE choices: Visual Studio 2026 (.NET workload), JetBrains Rider, Visual Studio Code with the C# Dev Kit extension.
- Visual Studio and Rider: open `Vjezba5.sln`. Each task is a separate project. Use "Set as Startup Project", then F5.
- VS Code: Open Folder, run configuration "zadatak N". Terminal: `dotnet run` in the task folder.
- README adds: Docker command `docker compose run --rm dotnet dotnet run -c Release --project exercises/05-periodic-tasks/Zadatak2_Periodicno`. The web page runs with `docker compose up web` at http://localhost:8080/05-periodic-tasks/Vjezba5_upute.html.
- README adds: the command line runs from the task folder with `dotnet run`, or from the exercise folder with `dotnet run --project Zadatak1_ImeSvakeSekunde`.
- README adds: in VS Code the Release configuration is "Vježba 5: zadatak N (Release)"; the Debug one has "(Debug)".
- README, task 1: in VS Code the run configurations use the integrated terminal.
- README, Visual Studio: right-click the project, "Set as Startup Project", then F5 or Ctrl+F5. Rider: File > Open, select `Vjezba5.sln`, then choose the task project in the run-configuration list (top right) and run it. VS Code: File > Open Folder with the exercise folder, and install the suggested C# Dev Kit (or C#) extension.
- S2: the download address is dotnet.microsoft.com/en-us/download. Notes S2: "the environment is the same as on earlier exercises" (see the setup in [ex-04-models-async.md](ex-04-models-async.md) and [ex-02-parallel.md](ex-02-parallel.md)). README: each task runs alone, "bez izbornika" (no start menu): there is no menu to choose a task, so start the project of the task directly.
- Real location of the settings: each task `.csproj` is almost empty (`<Project Sdk="Microsoft.NET.Sdk">` and a comment that the settings are in `../Directory.Build.props`). The file `Directory.Build.props` in the exercise folder sets `<OutputType>Exe</OutputType>`, `<TargetFramework>net10.0</TargetFramework>`, `<ImplicitUsings>enable</ImplicitUsings>` and `<Nullable>enable</Nullable>`. Its comment says: with only Visual Studio 2022, change to `net8.0`. So the `net8.0` change is made in `Directory.Build.props` (the deck and README say ".csproj"; the `.csproj` files hold no setting).
- `ImplicitUsings` is enabled, so the code compiles without `using System.Threading.Tasks;`, `using System.Threading;` or `using System.Collections.Generic;`. Only `using System.Diagnostics;` is written in the files, for `Stopwatch`. (inferred from the property)
- Upute: "Projekt cilja .NET 10 (LTS)": the target is .NET 10, a long-term-support version.
- `.vscode/tasks.json` has two build tasks, `build-debug` and `build-release` (`dotnet build` with `-c Debug` or `-c Release`). `.vscode/launch.json` has a Debug and a Release configuration for each task; each one starts `bin/<Debug|Release>/net10.0/<Task>.dll`.
- Project layout (README): each task is one file `ZadatakN_*/ZadatakN_*.cs` that holds the full task with starter commands.
- Source files exercise 5 (README table): Zadatak1_ImeSvakeSekunde, Zadatak2_Periodicno, Zadatak3_PeriodicnoPrekid, Zadatak4_Periodicno2, Zadatak5_OgradaZastoj (optional).

**Why:** The tasks measure time. Use the Release configuration, not Debug. (Notes S2: "Zadaci mjere vrijeme, zato Release.")
**Pitfall:** Task 1 waits for the Enter key. Run it in a terminal.
**Pitfall:** F5 starts the program with a debugger, also in Release. Measure with `dotnet run -c Release` or Ctrl+F5 (Vjezba5_upute, task 3 step 2).
**Pitfall:** With only Visual Studio 2022, change `net10.0` to `net8.0`. The deck says ".csproj of each task"; the real setting is in `Directory.Build.props` (see the list above). In VS Code, change it also in `.vscode/launch.json`. The tasks work the same. The README says: change `<TargetFramework>` and the program paths in `launch.json`.
**Link:** Before the exercise, read the exercise 5 deck and the end of lecture 5 (async/await, blocking threads): [05-modeli-racunala-i-async-await.md](05-modeli-racunala-i-async-await.md).
**Other facts (Vjezba5_upute):** Course: Paralelno programiranje, week 5. Credit: attendance at 70 % of exercise sessions at least. Two homework assignments are graded, "discussed later". Sample outputs come from Release on a CPU with 6 cores and 12 logical cores (macOS). Times change from run to run.
**Other facts (deck S1):** Title "Vježba 5: kanceliranje taskova". Authors: Marin Aglić Čuvić, after lectures by Tonći Dadić. Academic year 2026/2027. Notes S1: until now each task ran until it finished. Today the student stops it on request, measures how late the stop is, and makes the stopped work still return its result.

## Goal of the exercise   [S3]
**What:** Three skills.
1. Stop (Prekinuti) a task on request with a token, and measure how late the stop is.
2. Repeat (Ponavljati) the work every `ms` milliseconds, without the schedule getting later and later.
3. Return (Vratiti) the result of the stopped work without shared data.

**What (new in code):** `CancellationTokenSource`, `CancellationToken`, `CancelAfter`, a wait that can be stopped, and the task state (`Status`).
**Link:** Skill 1 is Topics 1-3 and task 1, 3. Skill 2 is Topic 5 and tasks 2, 3. Skill 3 is Topic 6 and task 4.

## Topic 1: Cancellation is an agreement (CancellationToken)   [S4]
**What:** A task cannot be stopped from outside. The main thread owns a `CancellationTokenSource` (the source) and uses it to request a stop. The task receives a `CancellationToken` and checks it itself.
**Why:** `Cancel()` only sets a request. If the task does not look at the token, the task continues to work (Notes S4).
**Conclusion:** A stop is an agreement. The source asks and the task answers by finishing.
**Example (S4, Vjezba5_upute):**
```csharp
var izvor = new CancellationTokenSource();
CancellationToken token = izvor.Token;

Task t = Task.Run(() =>
{
    while (!token.IsCancellationRequested)
    {
        // posao
    }
}, token);

Console.ReadLine();
izvor.Cancel();   // samo zatraži prekid
t.Wait();
```
In the Vjezba5_upute version the comment is `// postavi zahtjev za prekid`.
**What (API list from S4):**
- `Cancel()` sets the request.
- `CancelAfter(ms)` sets the request after the given time. Reference [2].
- A token passed to `Task.Run`: if the stop was requested before the task starts, the task does not start. Reference [1].
**Pitfall:** `Cancel()` does not stop the task. If the task code does not check the token, the task runs on.
**Pitfall (inferred):** The cancellation is as late as the time between two token checks. The S13 question 2 asks for this bound.
**Link:** The token must be checked also while the task waits. See Topic 2.
**References on S14:** [1] Microsoft Learn "Task cancellation" (learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-cancellation). [2] "CancellationTokenSource.CancelAfter" (learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.cancelafter). [3] "Barrier.SignalAndWait" (learn.microsoft.com/en-us/dotnet/api/system.threading.barrier.signalandwait). [4] "List<T> Class", section Thread Safety (learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1).

## Topic 2: A wait that can be stopped   [S5]
**What:** What each call does when the main thread requests a stop (`token` is the token of the task):

| call | when a stop is requested | without a stop |
| --- | --- | --- |
| `Thread.Sleep(ms)` | nothing: sleeps to the end | returns after ms |
| `token.WaitHandle.WaitOne(ms)` | returns at once, returns `true` | after ms returns `false` |
| `token.ThrowIfCancellationRequested()` | throws `OperationCanceledException` | does nothing |
| `await Task.Delay(ms, token)` | at once throws `TaskCanceledException` (a kind of `OperationCanceledException`) | returns after ms |
| `ograda.SignalAndWait(token)` | stops waiting, throws `OperationCanceledException` | waits for the others |

**Why:** The task checks the token itself. But a sleeping task does not check it. `Thread.Sleep` does not react to a stop. The other calls take the token or wait on its handle, so they react at once. `WaitOne` waits at most the given time and returns as soon as a stop is requested. The return value tells what happened (Notes S5). `ThrowIfCancellationRequested` throws if the stop was requested, otherwise it does nothing. `Task.Delay` and `SignalAndWait` accept a token and stop waiting with an exception.
**Example (measured):** Wait 2000 ms, stop after 300 ms. `Thread.Sleep` takes 2000 ms. The others take 300-336 ms. The notes say the others end 300 ms "nakon prekida" (after the stop), because 300 ms passed until `Cancel` (Notes S5). The wording "about 300 ms after the start" is an inference (inferred). See Open points.
**Conclusion:** Code that does not look at the token does not notice the stop. In a periodic task, replace `Thread.Sleep` with `token.WaitHandle.WaitOne(ms)` (the Zadatak 1, 3 and 4 solution files do this).
**Pitfall:** `Thread.Sleep` holds the thread and ignores the token. The late stop is the main error of task 1. See Zadatak 1.
**Link:** Which call to use and when is the decision of the student in the tasks (Notes S5). `await Task.Delay` is the async form from lecture 5: [05-modeli-racunala-i-async-await.md](05-modeli-racunala-i-async-await.md). The `SignalAndWait(token)` call is used in Zadatak 5.
**Demo (S8 notes):** After task 1, show section 3 of `async.html` ("Sleep drži nit": Sleep holds the thread). The page is in `exercises/04-models-async/Vizualizacije/`, not in exercise 05. Its content is in [ex-04-models-async.md](ex-04-models-async.md), topic 11. That topic shows 100 waits: `await Task.Delay` takes 1004 ms and `Thread.Sleep` takes 7006 ms. The rule: in async code wait with `await`, and exercise 5 does the same wait with a token.

## Topic 3: Task state after a stop   [S6]
**What:**

| what the delegate does | `t.Status` | `t.Wait()` |
| --- | --- | --- |
| ends normally or only leaves the loop | `RanToCompletion` | returns |
| throws `OperationCanceledException` (token also passed to the method that creates the task) | `Canceled` | `AggregateException` |
| the same, but without a token at creation (`Run`, `StartNew`, `new Task`) | `Faulted` | `AggregateException` |
| did not start: stop before start (token passed) | `Canceled` | `AggregateException` |
| throws another exception | `Faulted` | `AggregateException` |

**Why:** For the runtime, a task that only leaves the loop ended normally. For a delegate, `Canceled` comes from `OperationCanceledException` with the token that the task knows. `Canceled` also comes from a task that did not start. `Status` shows if the task was really stopped or only ended. Reference [1].
**Conclusion:** To get `Canceled`, pass the token to two places: the delegate checks it, and the method that creates the task receives it. Then throw with `token.ThrowIfCancellationRequested()`.
**Pitfall:** Without the token at creation, the state is `Faulted`. Pass the token also to `Task.Run`, `StartNew` or `new Task`.
**Pitfall:** `Wait()` and `Result` wrap the exception from the task in `AggregateException`. The real exception is in `InnerException`. After `await` (lecture 5) the student gets the plain exception, without the wrapper.
**Pitfall:** For a stopped task, `Wait()` throws. A program that must not end with an exception needs `try { t.Wait(); } catch (AggregateException) { }`.
**Link:** Zadatak 3 asks for the state `Canceled` and a program that does not end with an exception. Await: [05-modeli-racunala-i-async-await.md](05-modeli-racunala-i-async-await.md).

## Topic 4: Four pitfalls of the day   [S7]
**What:**
| pitfall | meaning | task |
| --- | --- | --- |
| Prekid kasni (stop is late) | the task sleeps just when the request arrives | 1 |
| Raspored klizi (the schedule drifts) | the gap between calls is larger than given | 2 |
| Krivo stanje (wrong state) | the task is stopped but `Status` says otherwise | 3 |
| Dijeljena lista (shared list) | two threads use the same list at the same time | 4 |

**Why:** Notes S7: one pitfall in each task. The rule for each: first measure or show the error, then fix it.
**Conclusion:** Measure first, fix second.
**Link:** Pitfall 4 is the data race of lecture 4: [04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md) and [ex-03-data-race.md](ex-03-data-race.md).

## Topic 5: Periodic work without drift   [S9, S10, Zadatak 2 and 3]
**What:** The k-th call must run at (k - 1) * ms from the start. The task must not sleep a fixed `ms` after each call.
**Why:** The action takes 300 ms and `ms` is 1000. Assume `Sleep(ms)` after each action. Then the gap is 1300 ms, and the calls run at about 0, 1300, 2600 ... (inferred: the source shows only the corrected output). This gives 8 calls in 9.5 s, not 10 (inferred). With the target time `k * ms` and a stopwatch, the sleep time is `k * ms - elapsed`. The delay does not add up (inferred from the code).
**Conclusion:** Compute the sleep time from the start time, with `Math.Max(0, ...)` so that it is never negative.
**Link:** See Zadatak 2 and Zadatak 3.

## Topic 6: Return a result without sharing data   [S11, Zadatak 4]
**What:** `Periodicno2<U>` keeps every value in a list that only the task knows. After the stop, the list is the result of the task (`Result`).
**Why:** The list is local. No other thread sees it before the task ends. A shared list is read by the main thread while the task writes to it, which is a data race. The slide gives only the title of reference [4] ("List<T> Class", section Thread Safety). That `List<T>` is not thread-safe for a write at the same time as a read is (inferred) from the title.
**Conclusion:** Return the data after the end of the task instead of sharing it while the task runs.
**Link:** [04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md) (lock and data race) and [ex-03-data-race.md](ex-03-data-race.md) (exercise on data race, lock, Interlocked). See Zadatak 4. Lecture 7 has concurrent collections: [07-zastoj-semafori-konkurentne-kolekcije.md](07-zastoj-semafori-konkurentne-kolekcije.md).

## Topic 7: Barrier waits for the slowest task   [S12, Zadatak 5]
**What:** `Barrier` makes all tasks wait until the last task arrives. One slow task delays all others. Reference [3] (`Barrier.SignalAndWait`) is listed only on S14. S12 does not cite it. The link to S12 is an inference (inferred).
**Why:** The aPRAM model from lecture 5: the program must be correct for any delay of any processor. Correct does not mean fast: the barrier waits for the slowest (Vjezba5_upute note).
**Why (cost grows with tasks):** The barrier cost grows with the number of tasks. Measured: 3113 ns at 12 tasks and 19037 ns at 24 tasks, more tasks than logical cores ([ex-04-models-async.md](ex-04-models-async.md), topic 8).
**Conclusion:** A correct program can still be slow. With a token, `SignalAndWait(token)` lets all tasks cancel.
**Link:** [05-modeli-racunala-i-async-await.md](05-modeli-racunala-i-async-await.md) (head of list, aPRAM model). Demo (S12 notes): `glava.html`, section 2 (tasks with a barrier). The upute footer also names `modeli.html`, `async.html` (see after task 1) and `glava.html` (barrier, with task 5). All three pages are in `exercises/04-models-async/Vizualizacije/`. Their content is in [ex-04-models-async.md](ex-04-models-async.md): topics 2-4 (head of list, lock step, aPRAM timeline), topics 5-8 (PRAM model, cost of a step, measured barrier cost) and topics 9-11 (async).

## Exercise tasks
Code note: every `Main` starts with `Console.OutputEncoding = System.Text.Encoding.UTF8;` (so the Croatian letters print correctly), and every file starts with `using System.Diagnostics;`. The code blocks keep the key lines of each solution. They omit setup lines such as `Stopwatch.StartNew`, counters, constants (for example `const string Ime = "Ana";`) and the test header `WriteLine` calls. The solution files contain Croatian comments with sample output; the blocks copy them.

### Zadatak 1: name every second   [S8; Zadatak1_ImeSvakeSekunde.cs]
**Task statement:** Given: a task prints the name every second (`Thread.Sleep(1000)`) until the main thread requests a stop (Enter). The program measures the time from `Cancel()` to the end of the task.
1. Write your name in the constant `Ime`. Run the program five times and press Enter at different times. Write down the times. What do they depend on?
2. Change the task so that less than 50 ms passes from `Cancel()` to the end, and the name still prints every second.

Sample output of the given code (Enter after 3.5 s):
```
Za prekid pritisnite Enter.
Ana
Ana
Ana
Ana
Task završio 592 ms nakon Cancel().
```
**Concept tested:** A late stop caused by `Thread.Sleep` (Topic 2).
**Solution idea:** Replace the sleep with a wait on the token handle.
```csharp
while (!token.IsCancellationRequested)
{
    Console.WriteLine(Ime);
    token.WaitHandle.WaitOne(1000);   // budi se čim se zatraži prekid
}
```
Main thread:
```csharp
var sat = Stopwatch.StartNew();
izvor.Cancel();
t.Wait();
Console.WriteLine($"Task završio {sat.ElapsedMilliseconds} ms nakon Cancel().");   // npr. 1 ms (bilo do ≈ 1000 ms sa Thread.Sleep)
```
**Reasoning:** The time depends on where the task is in its 1000 ms sleep when `Cancel()` arrives. It is between 0 and about 1000 ms (the sample shows 592 ms). `WaitOne(1000)` returns at once on a stop, so the loop condition ends the loop. The measured time is about 1 ms.
**Pitfall:** `Thread.Sleep` ignores the token. The task is late by the rest of the sleep.
**Note:** The solution does not pass the token to `Task.Run`, and the task only leaves the loop. The state is `RanToCompletion`; the task does not need `Canceled` here.

### Zadatak 2: Periodicno(akcija, ms)   [S9; Zadatak2_Periodicno.cs]
**Task statement:** Given: a test in `Main`. The action prints the call number and the time from the start, and takes 300 ms. The main thread waits 9.5 s and ends.
1. Write `static void Periodicno(Action akcija, int ms)`: in a new task it calls the action every `ms` milliseconds, without end.
2. Run the test (ms = 1000). How many calls do you expect in 9.5 s and when? How many were there?
3. If it differs: change the method so that the 10th call is in the 9th second (+/- 50 ms), although the action takes 300 ms.

Output after step 3:
```
poziv  1:     10 ms
poziv  2:   1006 ms
poziv  3:   2007 ms
…
poziv 10:   9007 ms
Kraj nakon 9509 ms, poziva: 10
```
**Concept tested:** Schedule drift (Topic 5).
**Solution idea:**
```csharp
static void Periodicno(Action akcija, int ms)
{
    Task.Run(() =>
    {
        var sat = Stopwatch.StartNew();
        for (int k = 1; ; k++)
        {
            akcija();
            Thread.Sleep((int)Math.Max(0, k * ms - sat.ElapsedMilliseconds));   // k-ti poziv u (k − 1) · ms, bez nakupljanja kašnjenja
        }
    });
}
```
Test:
```csharp
Action akcija = () =>
{
    poziv++;
    Console.WriteLine($"poziv {poziv,2}: {sat.ElapsedMilliseconds,6} ms");   // poziv  1:      2 ms … poziv 10:   9007 ms
    Thread.Sleep(300);   // posao akcije
};

Periodicno(akcija, 1000);
Thread.Sleep(9_500);
Console.WriteLine($"Kraj nakon {sat.ElapsedMilliseconds} ms, poziva: {poziv}");   // Kraj nakon 9507 ms, poziva: 10 (pozivi u 2, 1004, 2003, …, 9007 ms)
```
**Reasoning:** After call k the elapsed time is about (k - 1) * ms + 300. The sleep is `k * ms - elapsed`, so the next call starts at `k * ms` on the stopwatch. The action time is part of the interval, not added to it (inferred from the code). The delay never adds up. The tasks of the thread pool end together with the program (file header), so no stop is needed here.
**Pitfall:** `Thread.Sleep(ms)` after each action gives a gap of ms + 300 ms, and the schedule drifts (inferred). The 10th call is then not in the 9th second.
**Pitfall:** The sleep time must not be negative; `Math.Max(0, ...)` protects against it (if the action takes longer than `ms`).
**Pitfall:** The sample comments in the code show slightly different numbers than the deck (2 ms and 9507 ms versus 10 ms and 9509 ms). The times change in each run.

### Zadatak 3: Periodicno with a stop   [S10; Zadatak3_PeriodicnoPrekid.cs]
**Task statement:** Given: test 1 requests a stop after 3.5 s and measures the delay. Test 2 requests a stop before the method call. Both print the task state. `Main` can be changed.
1. Write `static Task Periodicno(Action akcija, int ms, CancellationToken token)`. It calls the action every `ms` milliseconds until a cancellation is requested. It returns the task, so the main thread can wait for its end. The calls follow the schedule of task 2: the k-th call is at (k - 1) * ms.
2. The task must end at most 100 ms after `Cancel()`. Measure in Release without a debugger: `dotnet run -c Release` or Ctrl+F5.
3. In both tests the state must be `Canceled`, and the program must not end with an exception.
4. What happened to the action in test 2? Why?

Expected output:
```
Test 1: prekid nakon 3,5 s
poziv 1:    25 ms
poziv 2:  1024 ms
poziv 3:  2025 ms
poziv 4:  3024 ms
Task završio 53 ms nakon Cancel().
Status: Canceled
Test 2: prekid zatražen prije poziva
Status: Canceled
```
**Concept tested:** Waiting that stops (Topic 2), state `Canceled` (Topic 3), the schedule of Topic 5.
**File header:** `Periodicno` calls the action every `ms` milliseconds until the main thread requests a cancellation with the token. It returns a `Task`, so the main thread can wait for the end and read the state (`Status`).
**Solution idea:**
```csharp
static Task Periodicno(Action akcija, int ms, CancellationToken token)
{
    return Task.Run(() =>
    {
        var sat = Stopwatch.StartNew();
        for (int k = 1; !token.IsCancellationRequested; k++)
        {
            akcija();
            token.WaitHandle.WaitOne((int)Math.Max(0, k * ms - sat.ElapsedMilliseconds));
        }
        token.ThrowIfCancellationRequested();   // Status = Canceled
    }, token);   // token i ovdje: prekid prije početka, task se ne pokrene
}
```
Test 1 and 2 in `Main` (full, from `Main`):
```csharp
var sat = Stopwatch.StartNew();
int poziv = 0;
Action akcija = () =>
{
    poziv++;
    Console.WriteLine($"poziv {poziv}: {sat.ElapsedMilliseconds,5} ms");   // poziv 1: 3 ms, poziv 2: 1008 ms, poziv 3: 2005 ms, poziv 4: 3005 ms
};

Console.WriteLine("Test 1: prekid nakon 3,5 s");
var izvor = new CancellationTokenSource();
Task t = Periodicno(akcija, 1000, izvor.Token);
Thread.Sleep(3500);
var doKraja = Stopwatch.StartNew();
izvor.Cancel();
try { t.Wait(); } catch (AggregateException) { }   // Wait() na prekinutom tasku baca iznimku
Console.WriteLine($"Task završio {doKraja.ElapsedMilliseconds} ms nakon Cancel().");   // 40 ms
Console.WriteLine($"Status: {t.Status}");   // Canceled

Console.WriteLine("Test 2: prekid zatražen prije poziva");
var izvor2 = new CancellationTokenSource();
izvor2.Cancel();
Task t2 = Periodicno(akcija, 1000, izvor2.Token);
try { t2.Wait(); } catch (AggregateException) { }
Console.WriteLine($"Status: {t2.Status}");   // Canceled; akcija se nije pozvala jer se task s već prekinutim tokenom ne pokrene
```
**Reasoning:**
- `WaitOne` returns at once on a stop, so the delay is small (40-53 ms in the samples, limit 100 ms).
- After the loop, `ThrowIfCancellationRequested()` throws `OperationCanceledException` with the known token. Because the token is also passed to `Task.Run`, the state is `Canceled`. Without the second argument, the state would be `Faulted`.
- Answer to question 4: in test 2 the action was never called. The token was already canceled before `Task.Run`, so the task does not start (state `Canceled`).
- `Wait()` on a `Canceled` task throws `AggregateException`. The `try`/`catch` stops the program from ending with an exception.
**Pitfall:** Only leaving the loop gives `RanToCompletion`, not `Canceled`.
**Pitfall:** Missing token in `Task.Run` gives `Faulted`.
**Pitfall (inferred):** `Thread.Sleep` in place of `WaitOne` breaks the 100 ms limit, because the task does not wake on the request.
**Pitfall:** F5 uses a debugger and makes the time wrong. Use `dotnet run -c Release` or Ctrl+F5.

### Zadatak 4: Periodicno2<U>   [S11; Zadatak4_Periodicno2.cs]
**Task statement:** Given: tests in `Main` and the method `PeriodicnoUZajednickuListu`, which adds values to a list received as an argument while the main thread reads the same list (test 2).
1. Write `static Task<List<U>> Periodicno2<U>(Func<U> f, int ms, CancellationToken token)`: calls `f` every `ms` milliseconds, keeps each return value, and after the stop the list of all values is the result of the task (`Result`).
2. Run test 1 (stop after 3.5 s with `CancelAfter`).
3. Test 2 uses the given method. What happens? Why can it not happen with your `Periodicno2`?
4. Fix test 2 so that the list stays shared and the program runs without error. You can change test 2 and the given method.

Output of test 1:
```
Test 1: Periodicno2, prekid nakon 3,5 s
Elemenata: 4: 6, 1006, 2006, 3006
```
The output of test 2 is not given on purpose: run it and describe what happened.
**Concept tested:** A data race on a shared list ([04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md), [ex-03-data-race.md](ex-03-data-race.md)). The notes of S11 give a hint: "Sjetite se predavanja 4" (remember lecture 4), that is, use `lock` (inferred: the note says only "remember lecture 4"). The solution gives a local list as the result.
**Solution idea:**
```csharp
static Task<List<U>> Periodicno2<U>(Func<U> f, int ms, CancellationToken token)
{
    return Task.Run(() =>
    {
        var lista = new List<U>();   // lokalna lista, nitko drugi je ne vidi dok task ne završi
        var sat = Stopwatch.StartNew();
        for (int k = 1; !token.IsCancellationRequested; k++)
        {
            lista.Add(f());
            token.WaitHandle.WaitOne((int)Math.Max(0, k * ms - sat.ElapsedMilliseconds));
        }
        return lista;
    });
}
```
Test 1: `izvor.CancelAfter(3500);` then `Periodicno2(() => sat.ElapsedMilliseconds, 1000, izvor.Token).Result;` and the sample comment `// Elemenata: 4: 2, 1005, 2007, 3005`.
The given method as it is in the solution file. It has `lock` (inferred: the slide text does not show if the starter version had `lock`):
```csharp
static Task PeriodicnoUZajednickuListu<U>(Func<U> f, int ms, List<U> lista, CancellationToken token)
{
    return Task.Run(() =>
    {
        while (!token.IsCancellationRequested)
        {
            lock (lista) lista.Add(f());
            Thread.Sleep(ms);
        }
    });
}
```
Test 2 (fixed with `lock`), with `ms = 0` (inferred: the writer then adds values very fast and the conflict is likely):
```csharp
Console.WriteLine("Test 2: zajednička lista, glavna nit je čita dok se puni");
var lista = new List<long>();
var izvor2 = new CancellationTokenSource();
Task t = PeriodicnoUZajednickuListu(() => sat.ElapsedMilliseconds, 0, lista, izvor2.Token);
var dok = Stopwatch.StartNew();
long citanja = 0;
while (dok.ElapsedMilliseconds < 2000)
{
    long zbroj = 0;
    lock (lista) foreach (long x in lista) zbroj += x;
    citanja++;
}
izvor2.Cancel();
t.Wait();
Console.WriteLine($"Pročitano {citanja} puta, elemenata na kraju: {lista.Count}");   // npr. Pročitano 471719 puta, elemenata na kraju: 13523; bez lock-a: InvalidOperationException (lista se promijenila tijekom foreach)
```
**Reasoning:**
- Without `lock`, the main thread runs `foreach` over the list while the task calls `Add`. The list changes during the enumeration, and `InvalidOperationException` is thrown. See reference [4] (title only; the claim is inferred, see Topic 6).
- With your `Periodicno2` this cannot happen: the list is local, and the main thread gets it only through `Result`, after the task ended. No thread uses it at the same time.
- The fix keeps the list shared. Both the writer and the reader take `lock (lista)`, so only one thread uses the list at a time.
- The deck question (S13): returning the list after the end has no data race. A shared list has one. Answer (inferred): one writer works first, then one reader reads, and the end of the task separates them.
**Pitfall (inferred):** `lock` must be in both places, the writer and the reader. One side alone does not protect the list.
**Pitfall:** The sample values differ (6, 1006, 2006, 3006 in the deck; 2, 1005, 2007, 3005 in the code comment). The times change in each run.
**Link:** [07-zastoj-semafori-konkurentne-kolekcije.md](07-zastoj-semafori-konkurentne-kolekcije.md) (concurrent collections), as another way to share a collection (inferred).

### Zadatak 5 (optional): barrier and a task that stalls   [S12; Zadatak5_OgradaZastoj.cs]
**Task statement:** Given: the head of a linked list from lecture 5 (8 tasks, `Barrier` between the steps). One randomly chosen task stalls for 10 s before the second barrier.
1. Run the program. How long do the other tasks wait? Is the result correct?
2. Change the program so that 2 s after the start the tasks cancel, and the main thread prints that the result was not obtained (instead of waiting 10 s or printing a wrong head).

Output of the given code:
```
Prava glava: 654; 8 taskova, task 2 zastane 10 s
Glava: 654 (10016 ms)
```
Note in Vjezba5_upute: aPRAM model from lecture 5: the program must be correct with any delay of any processor. Correct does not mean fast: the barrier waits for the slowest.
**File header:** The program is the head of a linked list from lecture 5 (example 3): `p` tasks, each for its own nodes, with a barrier (`Barrier`) between the steps. One random task stalls for 10 s (the file header says `Thread.Sleep`; the solution uses `WaitOne` on the token handle) before the second barrier, like a processor that is late.
**Concept tested:** A barrier waits for the slowest task. A token with a timeout lets all tasks cancel (Topics 1, 2, 7).
**Solution idea:**
```csharp
int[] next = NapraviListu(out int pravaGlava);
int p = 8;
int sporiTask = new Random().Next(p);
Console.WriteLine($"Prava glava: {pravaGlava}; {p} taskova, task {sporiTask} zastane 10 s");   // Prava glava: 654; 8 taskova, task 6 zastane 10 s

var sat = Stopwatch.StartNew();
var izvor = new CancellationTokenSource(2000);   // taskovi odustaju 2 s nakon početka
var token = izvor.Token;
int[] M = new int[N];
int glava = -1;
using var ograda = new Barrier(p);
var taskovi = new Task[p];
for (int id = 0; id < p; id++)
{
    int moj = id;
    taskovi[id] = Task.Factory.StartNew(() =>
    {
        for (int i = moj; i < N; i += p) M[i] = 0;
        ograda.SignalAndWait();
        for (int i = moj; i < N; i += p) if (next[i] >= 0) M[next[i]] = 1;
        if (moj == sporiTask) token.WaitHandle.WaitOne(10_000);
        ograda.SignalAndWait(token);
        for (int i = moj; i < N; i += p) if (M[i] == 0) glava = i;
    }, TaskCreationOptions.LongRunning);
}
try
{
    Task.WaitAll(taskovi);
    Console.WriteLine($"Glava: {glava} ({sat.ElapsedMilliseconds} ms)");
}
catch (AggregateException)
{
    Console.WriteLine($"Rezultat nije dobiven ({sat.ElapsedMilliseconds} ms)");   // Rezultat nije dobiven (2010 ms)
}
```
The list builder and the constant:
```csharp
const int N = 1000;

// Nasumična lista od N čvorova: redoslijed[k] je k-ti čvor liste, next[i] je sljedbenik od i (−1 za zadnji).
static int[] NapraviListu(out int glava)
{
    var rnd = new Random(1);
    int[] redoslijed = Enumerable.Range(0, N).OrderBy(_ => rnd.Next()).ToArray();
    int[] next = new int[N];
    for (int k = 0; k < N - 1; k++) next[redoslijed[k]] = redoslijed[k + 1];
    next[redoslijed[N - 1]] = -1;
    glava = redoslijed[0];
    return next;
}
```
Reason: sorting by random keys shuffles the nodes (`OrderBy(_ => rnd.Next())`), so the list order is random. The fixed seed `new Random(1)` gives the same list each run, so the head is always 654 (inferred). The head is `redoslijed[0]`. The slow task is `new Random().Next(p)` with `p = 8`; the sample comment says task 6 stalls.
**Reasoning:**
- Step 1: all 8 tasks wait for the slow one at the second barrier, about 10 s (10016 ms). The result is correct (654), but slow.
- The source cancels itself after 2000 ms. The slow task uses `WaitOne(10_000)` on the token handle, so it wakes at 2 s too, instead of `Thread.Sleep(10_000)`.
- `SignalAndWait(token)` throws `OperationCanceledException` in all waiting tasks. `Task.WaitAll` throws `AggregateException`, and the main thread prints "Rezultat nije dobiven" (2010 ms in the sample).
- `TaskCreationOptions.LongRunning` gives each task its own thread, so all 8 can wait at the barrier together (inferred: the source does not state the reason).
**Pitfall (inferred):** `Thread.Sleep(10_000)` for the slow task would not wake on a cancellation. The slow task would then not stop on time.
**Pitfall (inferred):** A barrier needs all participants. If one task does not arrive, the others wait until it arrives or until the token is canceled.
**Question (S13):** The barrier waits for the slowest. What does this mean for a program with 1000 tasks? Answer (inferred): all 1000 tasks wait for the slowest one, so one slow task delays all of them. The measured barrier cost also grows with the number of tasks (Topic 7).
**Demo (S12 notes):** `Vizualizacije/glava.html`, section 2 (tasks with a barrier).
**Link:** [05-modeli-racunala-i-async-await.md](05-modeli-racunala-i-async-await.md) (example 3, head of list, aPRAM).

## Closing questions   [S13]
1. Why does `Cancel()` not stop the task, but only ask it? (Topic 1)
2. How late can a cancellation be if the task works 1 s between two token checks? Answer (inferred): up to about 1 s. See Topic 1.
3. When is a periodic job with a token better than an infinite loop that ends with the program? (Zadatak 2 ends only with the program; Zadatak 3 lets the caller stop the task and read its state.) (inferred)
4. Task 4: why has returning the list after the end no data race, while a shared list has one? (Topic 6)
5. Task 5: the barrier waits for the slowest. What does it mean for a program with 1000 tasks? (Topic 7 and Zadatak 5; the answer is inferred)

## Open points
- Topic 2 example: the notes (S5) say the others end 300 ms "nakon prekida" (after the stop). The text can also mean 300 ms after the start of the wait, because the stop came at 300 ms. The source does not say which. The two readings give the same number only if the stop comes at 300 ms.
- The deck gives no answer to the closing questions. The answers above marked (inferred) come from the task code.
- The visualization pages `async.html`, `glava.html` and `modeli.html` belong to exercise 04 (`exercises/04-models-async/Vizualizacije/`). This file does not cover them. See [ex-04-models-async.md](ex-04-models-async.md).
- Sample numbers differ between the deck, the HTML page and the code comments (for example Zadatak 2: 10 ms / 9509 ms in the deck, 2 ms / 9507 ms in the code; Zadatak 3: 40 ms in the code, 53 ms in the deck; Zadatak 4: 6, 1006... versus 2, 1005...). The cause is run-to-run time change.
- The .pptx has no pictures on its 14 slides (a judge checked with python-pptx). The text export is complete.
- Zadatak 5 slide says "task 2 stalls" and the code comment says "task 6". The slow task is random.
- The code files contain solutions, not starter code. The task 1 solution ends with state `RanToCompletion`, because it does not pass a token to `Task.Run`.


## Coverage ledger
| slide | topic section where it is covered | items |
| --- | --- | --- |
| S1 | Setup and tools (other facts) | 4 (title, authors, year, notes) |
| S2 | Setup and tools | 9 |
| S3 | Goal of the exercise | 4 |
| S4 | Topic 1 | 6 (code, 4 bullets, notes) |
| S5 | Topic 2 | 8 (5 rows, measurement, notes, demo link); the "after the start" wording is inferred (Open points) |
| S6 | Topic 3 | 8 (5 rows, 3 bullets) |
| S7 | Topic 4 | 5 (4 pitfalls, method) |
| S8 | Zadatak 1, Topic 2 demo | 6 |
| S9 | Zadatak 2, Topic 5 | 6 |
| S10 | Zadatak 3 | 7 |
| S11 | Zadatak 4, Topic 6 | 7 |
| S12 | Zadatak 5, Topic 7 | 7 |
| S13 | Closing questions | 5 |
| S14 | Topic 1 (references) | 4 references |
| README.md | Setup and tools | 10 |
| .editorconfig, .gitignore, stil.css, Vjezba5.sln | none: no learning content (editor style, ignore list, page style, solution list of the 5 projects); outside the scope rule | 0 |
| Directory.Build.props, .csproj, .vscode | Setup and tools | 5 (settings location, ImplicitUsings, LTS, build tasks, launch configurations) |
| Vjezba5_upute.html | Setup, Topics 1-3, Zadatak 1-5 | all sections, plus submission rule |
| Zadatak1..5 .cs | Exercise tasks | 5 files, all code lines |
| async.html, glava.html, modeli.html | not in this exercise: in exercise 04, see ex-04-models-async.md (links in Topic 2 demo and Topic 7) | 0 (3 links) |
