# Exercise 4: computer models and async/await (source: exercises/04-models-async/, no slide deck extracted, 3 tasks, 4 visualization pages including the index)

## Map
This file covers exercise 4 (Vjezba 4). The exercise has three tasks and four visualization pages (index, glava, modeli, async).
The main idea: an algorithm that is correct on the ideal PRAM can fail on a real computer, and `async` does not mean parallel.
Topics 2 to 4 explain the linked-list head problem and the barrier (ograda) fix. Topics 5 to 8 explain the models (RAM, PRAM, aPRAM) and the price of a barrier.
Topics 9 to 11 explain async/await, the trap of an `async` method that only computes, and blocking inside async code.
The lecture is in [05-modeli-racunala-i-async-await.md](05-modeli-racunala-i-async-await.md). The `.pptx` in this folder equals lecture 05, so this file does not extract it.

## 1. Setup and how to run   [README.md, Vjezba4_upute.html "Priprema", Directory.Build.props, .csproj, .sln, .vscode/, .editorconfig, .gitignore]
**What:** The README says: "Modeli računala (RAM, PRAM, aPRAM) i async/await. Početni projekti za studente." The tasks are in `Vjezba4_upute.html`. The material is in the presentation and in the folder `Vizualizacije/` (`index.html`).
Each task is a separate project in the solution `Vjezba4.sln`. Each runs alone, without a menu.
| Task | Project folder | File |
|---|---|---|
| 1 | `Zadatak1_GlavaListe/` | `Zadatak1_GlavaListe/Zadatak1_GlavaListe.cs` |
| 2 | `Zadatak2_Async/` | `Zadatak2_Async/Zadatak2_Async.cs` |
| 3 | `Zadatak3_Blokiranje/` | `Zadatak3_Blokiranje/Zadatak3_Blokiranje.cs` |
The three `.csproj` files are empty SDK projects (`Microsoft.NET.Sdk`). Their comment says: "Postavke (TargetFramework i ostalo) su u ../Directory.Build.props."
`Directory.Build.props` holds the shared settings for all projects:
- `<OutputType>Exe</OutputType>`: each project is a console program.
- `<TargetFramework>net10.0</TargetFramework>`. The comment says: "Ako imate samo Visual Studio 2022, promijenite u net8.0".
- `<ImplicitUsings>enable</ImplicitUsings>`: this is why the `.cs` files have no `using System.Linq;` or `using System.Threading.Tasks;` (inferred from the missing usings).
- `<Nullable>enable</Nullable>`.
The solution `Vjezba4.sln` has the three projects. It has the configurations Debug and Release for Any CPU, x64 and x86. The x64 and x86 configurations map to Any CPU.
**Why:** The exercise measures time. A Debug build gives wrong times (inferred from the instruction to use Release).
**Conclusion:** Measure times in the Release configuration: `dotnet run -c Release`.
**Example:** Ways to run the tasks (from README.md):
- Docker (recommended in this repository: "preporučeno u ovom repozitoriju"), from the repository root: `docker compose run --rm dotnet dotnet run -c Release --project exercises/04-models-async/Zadatak1_GlavaListe`
- HTML pages: `docker compose up web`, then http://localhost:8080/04-models-async/Vjezba4_upute.html and http://localhost:8080/04-models-async/Vizualizacije/index.html
- Visual Studio or Rider: open `Vjezba4.sln`, select the task project, run it.
- Visual Studio Code: File > Open Folder, select this folder, use the configuration "Vježba 4: zadatak N". Select **Release** for timing.
- Command line: in the task folder, `dotnet run -c Release`.
Preparation (from `Vjezba4_upute.html`):
- Open `Vjezba4.sln`. Each task is a separate project (`ZadatakN_*/ZadatakN_*.cs`). Measure times in the **Release** configuration (`dotnet run -c Release`).
- Before the exercise, look at the presentation and at the Vizualizacije (list head, models, async/await).
Editor files:
- `.vscode/launch.json`: six launch configurations, "Vježba 4: zadatak N (Debug)" and "Vježba 4: zadatak N (Release)" for N = 1, 2, 3. They use `preLaunchTask` `build-debug` or `build-release`. The program path is `bin/<Debug or Release>/net10.0/<project>.dll`. The console is `integratedTerminal`.
- `.vscode/tasks.json`: `build-debug` and `build-release` run `dotnet build ${workspaceFolder}` with `-c Debug` or `-c Release`.
- `.vscode/extensions.json`: recommended extensions `ms-dotnettools.csharp` and `ms-dotnettools.csdevkit`.
- `.editorconfig`: UTF-8, LF line ends, final newline, 4 spaces. For `*.{csproj,json,sln}` it uses 2 spaces.
- `.gitignore`: `bin/`, `obj/`, `.vs/`, `.idea/`, `*.user`.
**Pitfall:** Time measurement in a non-Release build is not valid. The task file for task 2 says "VAŽNO: vrijeme mjerite u Release konfiguraciji." With only Visual Studio 2022, the .NET 10 target does not build, so change it to `net8.0` (inferred from the props comment).
**Link:** Time measurement in general: [02-flynn-granularnost-monte-carlo-pi.md](02-flynn-granularnost-monte-carlo-pi.md) (inferred link, lecture 2 covers parallel speed-up measurement).

## 2. Head of a linked list on a PRAM   [glava.html §1, Zadatak1 header]
**What:** A linked list has n nodes. `next[i]` is the successor (sljedbenik) of node i. It is -1 if the node has none. The head (glava) is the only node that has no predecessor (prethodnik).
The PRAM algorithm has three steps. Processor i works for node i. All processors run the same step at the same time. This is lock step.
1. `M[i] := 0`
2. `M[next[i]] := 1` (only if node i has a successor)
3. If `M[i] = 0`, then node i is the head.
M is an array with one cell for each node.
**Why:** After step 2, every node that has a predecessor has `M = 1`. The head has no predecessor, so nobody writes 1 into its cell. Only the head cell stays 0.
Each step takes one time unit, for any number of nodes. The total time is O(1).
No two processors write into the same cell, because each node has at most one predecessor. No two processors read the same cell. The algorithm is therefore EREW (see topic 5).
**Conclusion:** On the PRAM the algorithm finds the head in three steps, O(1), and it is correct. The correctness depends on lock step.
**Example:** The lecture list (slide 12) is 3 → 0 → 4 → 1 → 2. The head is node 3. After step 1: all `M` are 0. After step 2: `M[0]`, `M[4]`, `M[1]`, `M[2]` are 1 and `M[3]` is 0. In step 3 only node 3 reads 0.
The page `glava.html` §1 shows this step by step. The user can also choose a random list with 4 to 12 nodes.
**Link:** Lecture 05 slides 12 to 14 (see [05-modeli-racunala-i-async-await.md](05-modeli-racunala-i-async-await.md)). Task 1 in the "Exercise tasks" section.

## 3. The trap without lock step, and the barrier fix   [glava.html §2, Zadatak1 header]
**What:** On a real computer, tasks do not run in lock step. Each task runs its own three steps one after another, without waiting for other tasks.
The page shows three orders of operations:
- **for loop:** i = 0 runs all three steps, then i = 1, and so on.
- **tasks without barrier:** operations of different tasks mix in random order.
- **tasks with barrier:** random mixing again, but nobody starts step k + 1 before all tasks finish step k (`Barrier`).
**Why:** In the PRAM, "parallel" means: step 1 for all, then step 2 for all. If each node i runs all three steps at once, two things can happen to node i.
- The predecessor ran before i. The predecessor wrote 1 into `M[i]`. Then i writes 0 over it. Node i reads 0 and reports itself as head.
- The predecessor runs after i. Node i reads `M[i]` before the predecessor writes 1. Node i reads 0 and reports itself as head.
In both cases node i reports itself as head. In a for loop this happens to every node, so every node is a "head".
The barrier restores lock step where it is needed. Each step starts only after all tasks finish the step before.
**Conclusion:** An algorithm that is correct only in lock step must have a barrier between its steps. In C# use three consecutive `Parallel.For` loops (the end of a loop waits for all iterations) or `Barrier.SignalAndWait`.
**Example:** From the lecture: 1000 heads for 1000 nodes, in `Parallel.For` and in a plain for loop. With barrier: 1 head ("1 glava (654)") in every try.
The page runs each order 1000 times and shows a histogram of the number of heads found. The for loop gives exactly n heads for every list. Tasks without barrier give a number that changes from run to run. Tasks with barrier give 1.
**Pitfall:** A common mistake is to read what another processor writes in the same phase. The aPRAM phase rule (lecture 05 slide 16) forbids this. The page asks which steps break the rule if they share one phase, and gives no answer. Step 2 writes cells that step 1 writes and step 3 reads. So these steps conflict in one phase (inferred).
**Demo (page §2):** For the lecture list, go operation by operation in the for-loop order and follow node 0. Watch who writes `M[0]` and in which order. Then run each order 1000 times and compare how many heads are found. The conclusion is in the Why paragraph above.
**References:** Lecture 5 slides 12 to 16; archive lecture "Modeli paralelnih računala" (T. Dadić), slides 10 to 11; numbers from ModeliDemo (Tjedan 05/Predavanje/ModeliDemo, ispis_2026-10-31.txt).
**Link:** Data race (utrka podataka) causes: [04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md). Task 1.

## 4. aPRAM timeline of the head algorithm   [glava.html §3]
**What:** The page draws the same algorithm as a timeline. Each processor is one row. Time goes to the right (lecture 05 slide 19).
aPRAM rules used by the page:
- A local operation takes 1.
- An access to global memory takes d.
- A phase ends when the slowest processor ends. Then a barrier of cost B follows.
- The last instruction is a barrier.
- The processor holds `next[i]` in local memory.
One processor is late by a chosen number of units before step 2.
**Why:** With barriers, all processors wait for the slowest one. Without barriers, nobody waits, so the algorithm is faster. But it can be wrong.
**Conclusion:** Without barriers the result can be correct by chance, when the delay is small. A program that is correct only under such an assumption is not a valid aPRAM program (inferred; the page asks this as a question).
**Demo (page §3):** Start from the default numbers (d = 4, B = 16, P3 late by 10). With barriers, follow who waits for whom. The page text says: "Svi čekaju najsporijeg na ogradi." Then switch the barriers off. The algorithm is faster, but is it correct? The page text says: "Bez ograda nitko nikoga ne čeka." Reduce the delay and find the limit under which the result is correct by chance.
**Example:** The default numbers are d = 4, B = 16, and P3 late by 10. Without delay and with barriers the total time is 3d + 3B + 1 = 3·4 + 3·16 + 1 = 61 (computed from the page formula).
The page rule for the display without barriers: a read sees the writes that end before the read ends. If a write and a read end at the same instant, the write comes first.
**Pitfall:** The numbers d and B are invented, as on the slides. The measured barrier cost is in topic 8.
**References:** Lecture 5 slides 19 and 20; archive lecture (T. Dadić), slides 10 to 11.
**Link:** The cost formulas are in topic 7.

## 5. One PRAM step: who may do what   [modeli.html §1]
**What:** A PRAM step has three phases: all reads, then local computation, then all writes. All processors run them at the same time (Moreno Maza, slide 7).
The variants differ in what they allow when several processors touch the same cell:
| Variant | Rule |
|---|---|
| EREW | No two processors may read or write the same cell at the same time. |
| CREW | Concurrent reads of the same cell are allowed. Concurrent writes are not. |
| CRCW | Concurrent reads are allowed. A rule decides concurrent writes. |
CRCW write rules:
- **COMMON:** allowed only if all processors write the same value. Otherwise the state is undefined.
- **ARBITRARY:** one write succeeds. It is not known which one.
- **PRIORITY:** the processor with the highest priority succeeds. The page uses the smallest index.
**Why:** The reads happen in a phase before the writes. A reader that reads a cell which another processor writes in the same step gets the old value.
A stronger model allows more. An algorithm for a weaker model runs on a stronger model without change and in the same time. The order is: PRIORITY ≥ ARBITRARY ≥ COMMON ≥ CREW ≥ EREW (Moreno Maza, slide 49).
The reverse needs a simulation. One PRIORITY CRCW step on EREW takes O(log p) steps (slide 51; Prins, slide 7).
**Conclusion:** More allowed access gives a stronger model and often a faster algorithm.
**Example:** The page has five ready examples. The memory before the step is M[0]=5, M[1]=6, M[2]=7, M[3]=8.
- Each processor uses its own cell: P0 reads M[0], P1 reads M[1], P2 writes 1 to M[2], P3 writes 1 to M[3].
- All four read M[0].
- Two write the same value 1 to M[1].
- Two write different values (3 and 9) to M[1]. COMMON is not allowed. ARBITRARY leaves M[1] = ? (one succeeds, unknown which). PRIORITY gives the smallest index, so 3. (The 3 follows from the page code: `Math.min` of the writer indexes.)
- One reads M[2] and another writes 9 to M[2]. The reader gets the old value, because the read phase is before the write phase.
**Pitfall:** EREW forbids even one read plus one write on the same cell (the page code tests `r + w > 1`). CREW forbids only two writes.
**Detail:** The button "Novo polje" is on the zero-array page (topic 6). On this page the user can also build an own step with the menus.
**Link:** Lecture 05 slides 4 to 10 ([05-modeli-racunala-i-async-await.md](05-modeli-racunala-i-async-await.md)). The head algorithm (topic 2) is EREW.
**References:** M. Moreno Maza, Parallel Random-Access Machines, CS3101, UWO (slides 7, 23 to 24, 49, 51); J. F. Prins, PRAM (4), COMP 633, UNC (slide 7).
**Questions on the page:** What is the weakest model that allows "all read M[0]"? And "two write the same value"? (From the table: CREW and COMMON, inferred.)

## 6. Is there a zero in the array? CRCW in one step, EREW in log n   [modeli.html §2]
**What:** The array has n numbers. On COMMON CRCW each processor that sees a zero writes 1 into the same variable. All write the same value, so this is one step.
On EREW two processors may not write to one cell. The results are combined in pairs, level by level, like a tree. Each level is one EREW step (OR of two neighbours).
**Why:** The model allows or forbids the concurrent write. This changes the step count from 1 to ⌈log2 n⌉.
**Conclusion:** A stronger model gives a faster algorithm. When n doubles, CRCW still takes 1 step. EREW takes one more level.
**Example:** The default array is from slide 11: 3 1 7 0 4 1 6 3. For n = 8 the EREW tree has 3 levels. The result is "yes", because A[3] = 0. The page draws the tree up to n = 16 and the slider has n from 2 to 64. "Novo polje" gives random numbers from 0 to 5. The same tree pattern is the reduction in week 6.
**Pitfall:** On EREW the algorithm cannot write the result into one variable in one step (question on the page). Two writers would collide.
**Link:** Reduction pattern: [06-algoritam-sume-prefiksa.md](06-algoritam-sume-prefiksa.md). The page says the array also comes from the archive lecture on prefix sum.

## 7. Cost of one PRAM step on the aPRAM   [modeli.html §3]
**What:** The aPRAM has no common clock. A local operation costs 1. A global access costs d. A barrier for p processors costs B (slide 17). The assumption is 2 ≤ d ≤ B ≤ p.
One PRAM step runs as: read, barrier, compute, write, barrier. Its cost is **2B + 2d + 1** (slide 18).
If each aPRAM processor works for B PRAM processors, the step costs **5B + 2d − 2**. There are B times fewer processors (slide 21).
**Why:** In the first case each processor waits at a barrier twice per step, and the barrier is costly. In the second case one barrier covers more work, so the total work stays O(p · t) as on the PRAM (Gibbons, Lemma 1).
**Conclusion:** Use fewer processors with more work for each processor. The time stays O(B · t). The total work is (p / B) · O(B · t) = O(p · t).
**Example:** Numbers from the slides: p = 1024, d = 4, B = 16.
| Model | Processors | Time per step | Work (processors × time) |
|---|---|---|---|
| PRAM (everything costs 1) | 1024 | 1 | 1024 |
| aPRAM, p processors | 1024 | 2·16 + 2·4 + 1 = 41 | 41984 |
| aPRAM, p / B processors | 64 | 5·16 + 2·4 − 2 = 86 | 5504 |
(Table values computed from the formulas.) The work is about 7.6 times smaller in the second case ("u primjeru sa slajda oko 7,6 puta manji").
The page shows the segments: with p processors: read d, barrier B, compute 1, write d, barrier B. With p / B processors: read d+B−1, barrier B, compute B, write d+B−1, barrier B.
**Detail:** The page slider for p is a power of two, from 2^4 to 2^12. The default is 2^10 = 1024. The page keeps 2 ≤ d ≤ B ≤ p. It lowers B to p and d to B when needed. Its warnings are "B smanjen na p" and "d smanjen na B".
**Demo:** Raise B and watch how the share of the barrier in the step grows. Compare the work (processors × time) in both columns. In the formula 2B + 2d + 1 the barrier part is 2B, so its share goes towards 1 when B grows (inferred).
**Pitfall:** The numbers d and B are invented, as on the slides. The formulas come from the archive lecture, which counts a global read and a global write as d. Gibbons (1989) counts a read as 2d and a write as d. Then the step costs 2B + 3d + 1 and 5B + 3d − 2. The order of magnitude does not change.
**References:** Archive lecture "Modeli paralelnih računala" (T. Dadić), slides 14 to 18; P. B. Gibbons, A More Practical PRAM Model, SPAA 1989 (Lemma 1); lecture 5 slides 17, 18, 21.
**Pitfall (question on the page):** When B grows, the step time with p / B processors does not shrink, although there are fewer processors. The reason is in the formula: the term 5B grows with B (inferred).
**Link:** The measured value of B is in topic 8.

## 8. Measured: a barrier costs more with more tasks   [modeli.html §4]
**What:** Cost of one barrier (`Barrier.SignalAndWait`) for p tasks that do nothing else. Program ModeliDemo, example 6, 20,000 barriers, the shortest of 5 measurements. Machine: i7-9750H (6 cores, 12 logical), .NET 10, Release, 31 October 2026. This is B of topic 7 measured on a real computer (slide 20).
| p (tasks) | ns per barrier |
|---|---|
| 1 | 52 |
| 2 | 242 |
| 4 | 499 |
| 8 | 1236 |
| 12 | 3113 |
| 24 | 19037 |
**Detail:** The bar length on the page is logarithmic (page text: "duljina trake je logaritamska"). The bars for p above 12 are marked as bad.
**Why:** An algorithm with one processor per element and a barrier after each step waits at the barrier almost all the time. The barrier is more costly when there are more processors. At p = 24 there are more tasks than logical cores.
**Conclusion:** From p = 1 to p = 12 the barrier cost rises about 60 times. At p = 24 it costs six times more than at p = 12. Use fewer tasks with more work for each task. In C#, `Parallel.For` does this: one task processes a range of iterations.
**Pitfall:** A task count that is too large makes the program wait at the barrier. This is the same trap as tasks that are too small (lecture 2) and a tare that grows with N (lecture 3). The page states this link.
**Question on the page:** What changed when there are more tasks than logical cores? (The table shows the cost jump. The page does not give the cause.)
**Link:** Task 1, part 3 uses `Barrier` with p = 12 tasks. Granularity: [02-flynn-granularnost-monte-carlo-pi.md](02-flynn-granularnost-monte-carlo-pi.md). Overhead and Amdahl: [03-amdahlov-zakon-i-modeli-programiranja.md](03-amdahlov-zakon-i-modeli-programiranja.md).

## 9. Waiting without a busy thread   [async.html §1]
**What:** `await` waits and does not hold a thread. This speeds up waiting for something outside the program. It does not speed up computation.
`Dohvati` pretends to wait for a network: `await Task.Delay(1000)`. Call it N times either one by one (each `await` waits for the previous) or all at once with `await Task.WhenAll(...)`.
**Why:** While `await` waits, the thread goes back to the thread pool (fond niti) and does other work. The waits overlap in time and no core is added. This is concurrency (konkurentnost), from lecture 1. Waiting needs no core.
**Conclusion:** 100 waits at the same time take as long as 3. The computer has 12 logical cores, but the waits use 0 threads. The page shows "Niti zauzetih čekanjem" (threads busy with waiting) as 0. The page draws at most 10 lanes and then says "... i još N čekanja" (and N more waits).
**Example:** Measured (ModeliDemo, example 4):
| Run | Time |
|---|---|
| one by one, 3 × Delay | 3003 ms |
| Task.WhenAll, 3 × Delay | 1001 ms |
| Task.WhenAll, 100 × Delay | 1004 ms |
Ideal time for N waits of 1 s: one by one N s, all at once 1 s.
**Link:** Task 2, part 1. Blocking version in topic 11.
**Question on the page:** How many cores do 100 simultaneous waits need? Answer: none (inferred from the value 0 of the page indicator).
**References:** The page lead dates the measurements 31 October 2026. Microsoft Learn: Asynchronous programming with async and await.

## 10. Trap: an async method that computes   [async.html §2]
**What:** `Racunaj` is marked `async`, but it has no `await`. It only computes. The compiler warns (CS1998) that such a method runs synchronously.
**Why:** A method without `await` runs to the end in the thread of the caller. The three calls run one after the other, before the code reaches `WhenAll`. `WhenAll` then gets three finished tasks and ends at once. One thread did all the work.
`Task.Run` gives the computation to the thread pool. The main thread goes on. Three threads compute at the same time.
**Conclusion:** `async` does not mean "in another thread" or "in parallel". Start computing work with `Task.Run` (or `Parallel.For`). Keep `async`/`await` for waiting.
**Example:** Code on the page (step by step in the visualization):
```csharp
static async Task<long> Racunaj(long n)
{
    long s = 0;
    for (long i = 0; i < n; i++) s += i % 7;
    return s;
}
var a = Racunaj(N);
var b = Racunaj(N);
var c = Racunaj(N);
await Task.WhenAll(a, b, c);
```
The second version uses `var a = Task.Run(() => Racunaj(N));` and the same for b and c, then `await Task.WhenAll(a, b, c);`.
Measured (ModeliDemo, example 5): 3 calls one by one 1915 ms. `Task.WhenAll` without `Task.Run` 1686 ms. `Task.WhenAll` with `Task.Run` 513 ms.
The bars are schematic ("thread 1 to 3" are pool threads). The times are measured.
Step narration of the page, without `Task.Run`: the main thread calls `Racunaj`. There is no `await` in the method, so the main thread runs the whole loop itself. Only then does it get the task, and the task is already finished ("Tek tada dobije task, i to već završen"). The second and third calls do the same. `WhenAll` gets three finished tasks and ends at once. One thread did all the work, one after the other.
With `Task.Run`: `Task.Run` gives the calculation to the thread pool and the main thread goes on. For the second calculation a free thread takes it ("slobodna nit ga preuzme"). The third does the same. `WhenAll` waits for all three (the main thread shows "await (čeka)"). The calculations run at the same time on pool threads. The page says: measured about three times faster.
**Pitfall:** `WhenAll` without `Task.Run` takes almost as long as three calls one by one.
**References:** Microsoft Learn: Resolve errors and warnings in async methods (CS1998). Measurements: ModeliDemo examples 4 and 5 (ispis_2026-10-31.txt; i7-9750H, .NET 10, Release).
**Questions on the page:** With `Task.Run`, three calculations take about one third of the time. Would 30 calculations be ten times faster than 3? What limits this? (The page gives no answer. The cores limit it, inferred.) Why must `await` not appear inside `lock` (lecture 4, and slide 28 of lecture 5)? (The page gives no answer.)
**Link:** Task 2, part 2. `lock`: [04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md).

## 11. Trap: blocking in the middle of async code   [async.html §3]
**What:** The same wait of 1 s, 100 times at the same time: once with `await Task.Delay`, once with `Thread.Sleep` inside `Task.Run`. `Thread.Sleep` stops the thread that runs it. Each of the 100 waits holds its own thread. The computer has 12 logical cores.
| Blocks the thread | Does not block |
|---|---|
| `Thread.Sleep(1000)` | `await Task.Delay(1000)` |
| `t.Wait()`, `t.Result` | `await t` |
| `Task.WaitAll(ts)` | `await Task.WhenAll(ts)` |
**Why:** A blocking wait holds a thread although the thread does nothing.
**Conclusion:** In async code, wait with `await`.
**Example:** Measured (ModeliDemo, example 4): 100 × `Task.Delay` 1004 ms. 100 × `Thread.Sleep` 7006 ms. So 100 such waits took 7 s and not 1 s.
**Link:** Task 3. The page says that on exercise 5 the same wait runs with a cancellation token (CancellationToken).

## 12. Index page: learning goals and mapping   [index.html]
**What:** Week 5 covers computer models (RAM, PRAM, aPRAM) and cancellation. The lecture asks how to judge a parallel algorithm without a real computer, and why an algorithm correct on the PRAM can be wrong on a computer. Last topic: async/await is not parallelism.
**Cards of the index page.** Each card has a pill and a line of pitfalls:
- glava (pill "predavanje 5"): the three PRAM steps, the same steps operation by operation (for loop, tasks without and with barrier), 1000 runs and the distribution of the heads found, and the aPRAM timeline with d and B. Pitfalls: "algoritam točan samo u lock stepu; čitanje onoga što drugi piše u istoj fazi".
- modeli (pill "predavanje 5"): one PRAM step with four processors, "has zero" in 1 step (CRCW) or log n steps (EREW), the cost of a PRAM step on the aPRAM (2B + 2d + 1 and 5B + 2d − 2), the measured barrier cost. Pitfalls: "model kaže da sve košta 1; ograda za previše procesora".
- async (pill "predavanje 5, vježba 5"): waits with await one by one and all at once, an async method without await step by step, with and without `Task.Run`, `Thread.Sleep` against `Task.Delay`. "Sve brojke izmjerene". Pitfalls: "async metoda koja računa; blokiranje usred async koda".
**Example:** Which visualization serves which slide of lecture 5:
| Slides | Topic | Page |
|---|---|---|
| 4 to 10 | RAM, PRAM, pseudocode, EREW/CREW/CRCW, three CRCW rules, how the models relate | modeli, §1 |
| 11 | CRCW 1 step, EREW log n | modeli, §2 |
| 12 to 14 | list head, trap without lock step, barrier fix | glava |
| 15 to 16 | aPRAM, phase rule | glava §2 (barrier), modeli §1 |
| 17 to 18 | cost d and B, PRAM on aPRAM | modeli §3 |
| 19 | trace of execution on aPRAM | glava §3 |
| 20 to 21 | barrier cost, p / B processors | modeli §3 and §4 |
| 26 to 28 | async/await, async without parallelism, blocking | async |
Which exercise 5 task uses which page: task 1 "Ime svake sekunde" uses async §3 (Sleep holds a thread). Task 5 "Ograda i task koji zastane" (optional) uses glava §2 (tasks with barrier) and modeli §4.
The page `zajednicko.js` holds only navigation and helper functions. It has no course content.
**Goals after week 5** (list from the page):
- Explain RAM and PRAM and the difference between EREW, CREW and CRCW (COMMON, ARBITRARY, PRIORITY).
- Recognize an algorithm that is correct only in lock step, and fix it with a barrier.
- Explain what aPRAM adds to PRAM (d and B) and why fewer processors with more work each is better.
- Tell async/await (waiting without a busy thread) from parallelism (`Task.Run`, `Parallel.For`).
- Cancel a task with a token and check its state (exercise 5).
**Question on the glava page, not in the other topics (five questions in total on that page):** Is a barrier needed between steps 1 and 2, or is the one between steps 2 and 3 enough? Try in your mind the order where the predecessor writes 1 before i writes 0. (Answer from topic 3: both are needed, inferred.) How many barriers does an algorithm of t PRAM steps need? (The page gives no answer.) How long may P3 be late without barriers so that the result stays correct? (The page gives no answer.)

## Exercise tasks
Source of the statements: `Vjezba4_upute.html`. The page lead says: after the exercise the student can fix an algorithm that is correct only in lock step, tell concurrency from parallelism, and recognize blocking in the middle of async code.

### Task 1: head of a linked list   [Zadatak1_GlavaListe/Zadatak1_GlavaListe.cs]
**Task statement:** Given: a random list of 1000 nodes and an implementation without barrier. It finds a wrong number of heads.
1. Run the given implementation. How many heads does it find, and why? (see glava.html)
2. Write an implementation with three consecutive `Parallel.For` loops. How many heads does it find?
3. Write an implementation with `p` tasks and `Barrier`. Task `id` processes nodes `id, id + p, ...`. The barrier is between the steps.
**Concept tested:** Lock step, the PRAM three-step algorithm and the barrier (topics 2 and 3). Header of the file: `next[i]` is the successor of node i (-1 if none). The head is the only node without predecessor. PRAM algorithm: 1) `M[i] := 0` 2) `M[next[i]] := 1` 3) if `M[i] = 0`, node i is the head.
**Solution code idea:**
The list builder shuffles the node order and links each node to the next one in the shuffled order. It returns `next` and the true head `redoslijed[0]`:
```csharp
var redoslijed = Enumerable.Range(0, n).OrderBy(_ => Random.Shared.Next()).ToArray();
var next = new int[n];
for (int k = 0; k < n; k++) next[redoslijed[k]] = k + 1 < n ? redoslijed[k + 1] : -1;
return (next, redoslijed[0]);
```
Part 1, wrong: all three steps in one loop, no barrier. The heads go into a `ConcurrentBag<int>` (thread-safe collection, inferred reason). Full method:
```csharp
    static int BezOgrade(int[] next)
    {
        int n = next.Length;
        var M = new int[n];
        var glave = new System.Collections.Concurrent.ConcurrentBag<int>();
        Parallel.For(0, n, i =>
        {
            M[i] = 0;
            if (next[i] >= 0) M[next[i]] = 1;
            if (M[i] == 0) glave.Add(i);
        });
        return glave.Count;
    }
```
Part 2, three loops. The end of each `Parallel.For` is a barrier:
```csharp
    static int TriPetlje(int[] next)
    {
        int n = next.Length;
        var M = new int[n];
        Parallel.For(0, n, i => M[i] = 0);   // korak 1 za sve
        Parallel.For(0, n, i => { if (next[i] >= 0) M[next[i]] = 1; });   // korak 2 za sve; svaki čvor ima najviše jednog prethodnika, pa nema sudara
        return M.Count(m => m == 0);   // korak 3: ostane 0 samo ćelija glave; 1
    }
```
Part 3, barrier with p tasks. Each task takes nodes `id, id + p, ...` (cyclic split):
```csharp
    static int SOgradom(int[] next, int p)
    {
        int n = next.Length;
        var M = new int[n];
        int glave = 0;
        using var ograda = new Barrier(p);
        Task.WaitAll(Enumerable.Range(0, p).Select(id => Task.Run(() =>
        {
            for (int i = id; i < n; i += p) M[i] = 0;
            ograda.SignalAndWait();   // nitko ne piše jedinice prije nego svi upišu nule
            for (int i = id; i < n; i += p) if (next[i] >= 0) M[next[i]] = 1;
            ograda.SignalAndWait();   // nitko ne čita M prije nego svi upišu jedinice
            for (int i = id; i < n; i += p) if (M[i] == 0) Interlocked.Increment(ref glave);
        })).ToArray());
        return glave;   // 1
    }
```
Output code (`Pokreni`):
```csharp
var (next, glava) = NapraviListu(N);
Console.WriteLine($"Lista od {N} čvorova, prava glava: {glava}");
Console.WriteLine($"Parallel.For, bez ograde:  {BezOgrade(next)} glava");   // 1000 glava (krivo: čvor prepiše jedinicu prethodnika ili pročita prerano)
Console.WriteLine($"Tri Parallel.For:          {TriPetlje(next)} glava");   // 1 glava
Console.WriteLine($"Barrier, 12 taskova:       {SOgradom(next, 12)} glava");   // 1 glava
```
The class has `const int N = 1000;`. Expected: 1000 heads (wrong) without barrier, 1 head with three `Parallel.For`, 1 head with `Barrier` and 12 tasks. `Console.OutputEncoding` is set to UTF8 for Croatian letters.
**Reasoning:**
- Part 1 gives 1000: "čvor prepiše jedinicu prethodnika ili pročita prerano" (a node overwrites the 1 of its predecessor, or reads too early). See topic 3.
- Part 2 gives 1. The loop end waits for all iterations, so step 1 is complete before step 2 starts, and step 2 before step 3. There is no collision in step 2, because each node has at most one predecessor, so no two iterations write the same cell. After step 2 only the head cell is 0.
- Part 3 gives 1. The first barrier stops anybody from writing 1 before everybody wrote 0. The second barrier stops anybody from reading `M` before everybody wrote 1. `Interlocked.Increment(ref glave)` is needed because many tasks add to one counter (inferred). There is no third barrier, because `Task.WaitAll` waits for all tasks (inferred).
- `p` is 12 in the program. The visualization says its machine has 12 logical cores, so 12 tasks equal the core count (inferred). More tasks than logical cores makes the barrier much more costly (topic 8, 3113 ns at 12 and 19037 ns at 24).
**Pitfall:** Without a barrier the result is wrong because of the order of operations, not because of a lost update on one variable (inferred). If the student puts only one barrier (between steps 2 and 3), a task can still write 1 before another task wrote 0, and the 1 is lost (inferred from the glava.html question). The barrier needs `p` equal to the number of tasks (`new Barrier(p)`), or the tasks wait forever (inferred).
**Link:** [04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md) for the race and `Interlocked`; [06-algoritam-sume-prefiksa.md](06-algoritam-sume-prefiksa.md) for algorithms with barriers between steps (inferred).

### Task 2: async/await   [Zadatak2_Async/Zadatak2_Async.cs]
**Task statement:** Given: `Dohvati` (waits 1 s) called three times one by one, and the method `Racunaj`.
1. Call `Dohvati` with `Task.WhenAll` for 3 and for 100 calls. Why is 100 as fast as 3?
2. Call `Racunaj` three times with `Task.WhenAll`, without `Task.Run`, then with `Task.Run`. Explain the difference. (see async.html)
**Concept tested:** Concurrency is not parallelism. Waiting needs no core (topic 9). An `async` method without `await` runs synchronously (topic 10). The file header: "async/await je konkurentnost, a ne paralelizam. Dohvati glumi čekanje na mrežu. Racunaj samo računa."
**Solution code idea:**
```csharp
static async Task<int> Dohvati(int id)
{
    await Task.Delay(1000);   // npr. mreža
    return id;
}

static async Task<long> Racunaj(long n)
{
    long s = 0;
    for (long i = 0; i < n; i++) s += i % 7;
    return s;
}

const long N = 600_000_000;
```
The file starts with `using System.Diagnostics;`. One by one (baseline):
```csharp
var sat = Stopwatch.StartNew();
for (int i = 0; i < 3; i++) await Dohvati(i);
sat.Stop();
Console.WriteLine($"jedno po jedno, 3 × Delay:  {sat.ElapsedMilliseconds} ms");   // ≈ 3003 ms
```
All at once, for k = 3 and k = 100:
```csharp
foreach (int k in new[] { 3, 100 })
{
    sat.Restart();
    await Task.WhenAll(Enumerable.Range(0, k).Select(Dohvati));   // sva čekanja se preklapaju, nit se za to vrijeme vraća u fond
    Console.WriteLine($"Task.WhenAll, {k} × Delay:  {sat.ElapsedMilliseconds} ms");   // ≈ 1001 ms za 3, ≈ 1004 ms za 100: čekanje ne treba jezgre
}
```
`Racunaj` one by one:
```csharp
sat.Restart();
for (int i = 0; i < 3; i++) await Racunaj(N);
Console.WriteLine($"Racunaj, jedan po jedan:     {sat.ElapsedMilliseconds} ms");   // npr. 1030 ms
```
Without `Task.Run`:
```csharp
sat.Restart();
await Task.WhenAll(Racunaj(N), Racunaj(N), Racunaj(N));   // bez await u metodi, svaki poziv se izvede do kraja u ovoj niti
Console.WriteLine($"WhenAll, bez Task.Run:       {sat.ElapsedMilliseconds} ms");   // npr. 1017 ms ≈ jedan po jedan
```
With `Task.Run`:
```csharp
sat.Restart();
await Task.WhenAll(Task.Run(() => Racunaj(N)), Task.Run(() => Racunaj(N)), Task.Run(() => Racunaj(N)));   // tri izračuna na tri niti iz fonda
Console.WriteLine($"WhenAll, s Task.Run:         {sat.ElapsedMilliseconds} ms");   // npr. 348 ms ≈ trećina; async služi za čekanje, a posao koji računa ide na Task.Run
```
The code above sits in `public static async Task Pokreni()`. The top-level line is `await Zadatak2_Async.Pokreni();`.
**Reasoning:**
- 100 waits equal 3 waits because a wait does not use a core. The thread returns to the pool during the wait. The 100 waits overlap. The time is the longest single wait, about 1 s.
- `Racunaj` has no `await`, so each call runs to the end in the calling thread. The three calls run one after the other, and `WhenAll` gets finished tasks. The time is about the same as one by one (1017 ms against 1030 ms).
- `Task.Run` puts each calculation on a pool thread. The three run at the same time. The time is about one third (348 ms).
**Pitfall:** A common mistake is to believe that `async` means parallel. The measured result: no speed-up without `Task.Run`. Do not time in Debug. The sample times in the code (1030, 1017, 348 ms) differ from the visualization (1915, 1686, 513 ms). The cause is not stated (see Open points). The speed-up with `Task.Run` is limited by the number of cores (inferred).
**Link:** Topics 9 and 10. Concurrency against parallelism: [01-uvod-u-paralelno-programiranje.md](01-uvod-u-paralelno-programiranje.md) (the visualization cites lecture 1).

### Task 3: blocking in the middle of async code   [Zadatak3_Blokiranje/Zadatak3_Blokiranje.cs]
**Task statement:** Given: 100 waits of 1 s with `await Task.Delay`.
1. The same wait with `Thread.Sleep(1000)` in `Task.Run`. How long does it take and why?
2. The same wait with cancellation: `CancellationTokenSource`, cancel after 300 ms. In which state do the tasks end?
**Concept tested:** Blocking a pool thread (topic 11) and cancellation of a task with a token. Header: "Sto istih čekanja od 1 s. Zadano: ispravno čekanje s await Task.Delay."
**Solution code idea:**
`const int K = 100;` The program prints `Jezgri (logičkih): {Environment.ProcessorCount}`.
Given, correct wait:
```csharp
var sat = Stopwatch.StartNew();
await Task.WhenAll(Enumerable.Range(0, K).Select(_ => Task.Delay(1000)));
sat.Stop();
Console.WriteLine($"Task.WhenAll, {K} × Delay:  {sat.ElapsedMilliseconds} ms");   // ≈ 1004 ms
```
The file starts with `using System.Diagnostics;`.
Part 1, blocking wait:
```csharp
sat.Restart();
await Task.WhenAll(Enumerable.Range(0, K).Select(_ => Task.Run(() => Thread.Sleep(1000))));   // svako čekanje drži svoju nit
Console.WriteLine($"Task.WhenAll, {K} × Sleep:  {sat.ElapsedMilliseconds} ms");   // npr. 7528 ms: fond niti dodaje niti postupno, a ne sve odjednom
```
Part 2, cancellation:
```csharp
using var izvor = new CancellationTokenSource();
var cekanja = Enumerable.Range(0, K).Select(_ => Task.Delay(1000, izvor.Token)).ToArray();
sat.Restart();
izvor.CancelAfter(300);   // zatraži prekid nakon 300 ms
try { await Task.WhenAll(cekanja); }
catch (OperationCanceledException) { }   // WhenAll baca iznimku prvog prekinutog taska
Console.WriteLine($"Prekid nakon 300 ms: {sat.ElapsedMilliseconds} ms, prekinuto {cekanja.Count(t => t.IsCanceled)} od {K}");   // npr. 324 ms, prekinuto 100 od 100 (Task.Delay prekida i dok spava; Thread.Sleep ne bi)
```

**Reasoning:**
- Part 1 takes about 7.5 s (7528 ms in the code, 7006 ms in the visualization), not 1 s. Each `Thread.Sleep` holds one pool thread. There are 12 logical cores. The thread pool adds threads step by step, not all at once. So the 100 waits do not all start at the same time.
- Part 2: all 100 tasks end in the Canceled state (`IsCanceled` is true for 100 of 100). The total time is about 300 ms (324 ms), not 1 s. `Task.Delay` with a token stops while it sleeps.
- `try ... catch (OperationCanceledException)` is needed because `WhenAll` throws the exception of the first canceled task.
**Pitfall:**
- `Thread.Sleep` in `Task.Run` makes a wait slow and holds a thread without work. The fix is `await Task.Delay`.
- `Thread.Sleep` cannot be canceled by the token. `Task.Delay` can.
- Without the `try/catch` the program stops with an exception (inferred). The code comment says only that `WhenAll` throws the exception of the first canceled task.
**Link:** Topic 11 and topic 9. Cancellation continues in exercise 5 (the visualization index says the exercise 5 task cancels with a token).

## Open points
- The times in the task code comments (Task 2: 1030, 1017, 348 ms; Task 3: 7528 ms) differ from the visualization (1915, 1686, 513 ms; 7006 ms). The visualization cites ModeliDemo on an i7-9750H, 31 October 2026. The code comments do not name a machine. The cause is not stated.
- `glava.html` says "1 glava (654)" for the barrier run. The meaning of 654 is not stated. It may be the time or the node number from the lecture demo.
- The source states the 1000-heads result for `Parallel.For` without barrier ("1000 glava od 1000 čvorova"). The source does not explain why a parallel loop gives exactly 1000 and not a random number. The page gives a random number only for the simulated "tasks without barrier".
- The reason for the jump of the barrier cost at p = 24 is not given in the source. The page only says that 24 is more than the 12 logical cores.
- The index page links to a previous week folder `Tjedan 04 - Data race`, which is not in this repository.
- Pictures: the page diagrams are drawn by JavaScript (no static images). All values were read from the code, so no picture-only content was missed.
- The "questions for thought" have no answers in the source (glava 5, modeli 4, async 4). Where this file gives an answer, it is marked "inferred".
- `stil.css` and `zajednicko.js` hold style and navigation only.

## Coverage ledger
No slides exist in this stream. The ledger lists source files and sections.
| Source location | Topic section where covered | Items |
|---|---|---|
| README.md (structure table, run ways, .NET 10) | 1 | 7 |
| Vjezba4_upute.html: lead and "Priprema" (both bullets) | 1, Exercise tasks | 4 |
| Vjezba4_upute.html: Zadatak 1 statement | Task 1 | 3 |
| Vjezba4_upute.html: Zadatak 2 statement | Task 2 | 2 |
| Vjezba4_upute.html: Zadatak 3 statement | Task 3 | 2 |
| Zadatak1_GlavaListe.cs (header, NapraviListu, BezOgrade, TriPetlje, SOgradom, Pokreni) | Task 1 | 5 code blocks, 3 results |
| Zadatak2_Async.cs (Dohvati, Racunaj, N, Pokreni) | Task 2 | 6 code blocks, 6 times |
| Zadatak3_Blokiranje.cs | Task 3 | 3 code blocks, 3 times |
| Directory.Build.props | 1 | 5 (OutputType, TargetFramework, net8.0 comment, ImplicitUsings, Nullable) |
| Zadatak*.csproj (3 files) | 1 | 2 (empty SDK projects, comment pointing to the props file) |
| Vjezba4.sln | 1 | 2 (three projects, Debug and Release configurations) |
| .vscode/launch.json, tasks.json, extensions.json | 1 | 6 launch configurations, 2 build tasks, 2 extensions |
| .editorconfig, .gitignore | 1 | 5 settings, 5 ignore entries |
| Vizualizacije/index.html | 12 | 3 cards, 2 mapping tables, 5 goals |
| Vizualizacije/glava.html §1 | 2 | 6 |
| Vizualizacije/glava.html §2 | 3 | 8 |
| Vizualizacije/glava.html §3 | 4 | 7 |
| Vizualizacije/glava.html questions and footer | 3, 4, 12 | 5 questions, 3 references |
| Vizualizacije/modeli.html §1 | 5 | 11 |
| Vizualizacije/modeli.html §2 | 6 | 5 |
| Vizualizacije/modeli.html §3 | 7 | 9 |
| Vizualizacije/modeli.html §4 | 8 | 9 |
| Vizualizacije/modeli.html questions and footer | 5, 7, 8 | 4 questions, 4 citations (Moreno Maza, Prins, Gibbons, Dadić) |
| Vizualizacije/async.html §1 | 9 | 5 |
| Vizualizacije/async.html §2 | 10 | 8 |
| Vizualizacije/async.html §3 | 11 | 6 |
| Vizualizacije/async.html questions and footer | 9, 10 | 4 questions, 2 Microsoft Learn citations, ModeliDemo source |
| Vizualizacije/zajednicko.js, stil.css | 12 | title only: navigation and style, no course content |
| .pptx deck | not extracted | equals lecture 05, covered by another agent |
