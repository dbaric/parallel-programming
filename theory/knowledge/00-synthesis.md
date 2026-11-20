# Parallel Programming: Combined Knowledge File (synthesis of 7 lectures and 6 exercises)

## 1. How to use this file and file map {#sec-1}

**How to use.** Read section 2 first. It gives the whole argument in one page. Then read chapters A to G in order. Each chapter has a core idea, concept cards and a link block. Section 3 holds chapters A to G. Use sections 4 to 6 as lookup tables. Section 7 holds the numbers. Section 8 lists where the sources disagree.

**Exam scope.** Exam 1 is in week 8 and covers weeks 1 to 7. Exercise 7 (not in the repo) has philosophers with two fixes and a measurement, a car park for at most 3 cars, a `Wait`/`PulseAll` queue to repair, and five errors from weeks 1 to 6 to find (`07-zastoj-semafori-konkurentne-kolekcije.md`, "First exam and exercise 7").

**Card format.** Each card has What, Why, Conclusion, Proof (a number or example from the course) and Source. A card id (for example `#d4`) is stable. A notebook can link to it.

**Tags.** The tag (inferred) marks a link that this file adds. The 13 source files do not state it. Croatian terms appear in section 9. All numbers come from the sources. They come from one machine (i7-9750H, 6 cores, 12 logical) unless the card or source says otherwise. Numbers keep the form of the source file, and the files differ: the Croatian form uses a dot for thousands and a comma for decimals (1.981.047, 0,51). The English form uses a comma for thousands (1,236 ns; 5,504; 184,756 in ex-03). Compare numbers only within one source. "Lock step" is one term for all processors that act in one step.

| Lecture file | Exercise file | Main concept |
|---|---|---|
| `01-uvod-u-paralelno-programiranje.md` | `ex-01-delegates.md` | Why parallel, hardware, Amdahl intro, what can go wrong. Delegates and lambda as task bodies. |
| `02-flynn-granularnost-monte-carlo-pi.md` | `ex-02-parallel.md` | Flynn, granularity, overhead, Monte Carlo pi, four traps. Tasks, speedup, four traps A to D. |
| `03-amdahlov-zakon-i-modeli-programiranja.md` | (none in repo, see section 8) | Amdahl, Karp-Flatt, Gustafson, Bernstein, PCAM, wavefront, odd-even sort. |
| `04-data-race-lock-interlocked.md` | `ex-03-data-race.md` | Data race, lock, Interlocked, compare-and-swap, check, then change. |
| `05-modeli-racunala-i-async-await.md` | `ex-04-models-async.md`, `ex-05-periodic-tasks.md` | RAM, PRAM, aPRAM, barrier, async/await. Cancellation and periodic tasks. |
| `06-algoritam-sume-prefiksa.md` | `ex-06-prefix-sum.md` | Scan, Blelloch, blocks, Brent rule, seven traps. |
| `07-zastoj-semafori-konkurentne-kolekcije.md` | (exercise 7 not in repo) | Deadlock, livelock, semaphore, Wait/Pulse, concurrent collections. |

The folder numbers do not equal the week numbers. Exercise folder 03 is the data race exercise of week 4. Folder 04 goes with lecture 5. Folder 05 teaches cancellation.

## 2. The big picture in one page {#sec-2}

```
A why/hardware -> B measure -> C split -> D what breaks and fixes
   -> E models, async -> F prefix sum -> G deadlock, collections
```

1. **Why.** Power and heat stop the clock rate, so extra transistors become cores, so the programmer must split work (A1).
2. **Hardware.** Hardware parallelism is free at bit and instruction level. Data and task parallelism are the programmer's job. Memory model decides the failure (A3, A6).
3. **Measure.** The sequential part caps S (B2). Overhead and barrier waiting act like extra sequential time (inferred), so Karp-Flatt e grows (B4).
4. **Split.** Parts must be independent (C3), about equal, and larger than their overhead (C1, B6).
5. **What breaks.** Multi-step operations on shared data lose updates, silently (D1, D2).
6. **Fixes and cost.** Isolation is free. Lock and Interlocked cost time, so synchronize rarely (D3, D5). Locks add traps (D4, D6, G1).
7. **Models and async.** A lock step algorithm needs a barrier on a real machine, and barriers cost more with more tasks (E3, E5). Brent links steps and work (E6). async is concurrency (E7). Stop is an agreement (E8).
8. **Worked family.** The prefix sum tree gives log steps. Operation, order, padding and blocks decide success (F1 to F5).
9. **Deadlock and collections.** Remove one of four conditions. Semaphore, Wait/Pulse and BlockingCollection each have a trap (G1 to G6).

The recurring cost law (inferred): every parallel tool pays in waiting, in extra work or in risk. The best design shares nothing and merges once.

---

## 3. Chapters A to G {#sec-3}

## A. Why and what {#ch-a}

**Core idea.** Frequency stopped growing, so cores multiply. Hardware parallelism at bit and instruction level is free. Data and task parallelism are the programmer's job. Parallelism needs different cores at the same time. Concurrency is one core in turns. Flynn classes name what the hardware offers. Shared or separate memory decides the typical failure.

### A1. Why parallel now {#a1}
- **What:** Moore law: transistors double about every two years. The clock rate grows very slowly from about the middle of the 2000s.
- **Why:** A higher clock means more power and heat. So designers put the extra transistors into more cores.
- **Conclusion:** A faster program no longer comes from a faster processor. The program must use the cores.
- **Proof:** Single-core growth: 52 % per year (1986-2003), then about 3.5 % (from 2015), a reconstructed curve. Measured: 40 % (1988-2003), 8 % (2015-2021). Llama 3 405B needs 3.8 · 10²⁵ operations: one core about 1.2 billion years, one H100 about 3,000 years, 16,384 GPUs about 67 days. The GPUs run at about 40 % of peak.
- **Also:** Parallel programming pays when the problem has a large data set or many independent computations. The real world is parallel, a second reason beyond speed. A desktop has many functional units, cores and hardware threads. Libraries give tools, but they do not protect against these errors.
- **Source:** `01-uvod-u-paralelno-programiranje.md`, "Why now: Moore law and the frequency wall"; "Large language model: why it needs many cores".

### A2. Parallelism versus concurrency {#a2}
- **What:** Parallelism: different cores run at the same time. Concurrency: the operating system switches tasks on one core.
- **Why:** Concurrency does not speed up the computation. It keeps a program responsive while it waits for network or disk.
- **Conclusion:** A data race can occur in both, because the switch can happen in the middle of an operation.
- **Proof:** async/await gives 100 waits in 1004 ms with no extra core (E7).
- **Source:** `01-uvod-u-paralelno-programiranje.md`, "Parallelism versus concurrency".

### A3. Levels of parallelism, data and task {#a3}
- **What:** Bit and instruction level: hardware does it. Data parallelism: same operation on different data. Task parallelism: different jobs at the same time.
- **Why:** The hardware levels need no programmer. Data parallelism scales with more data. Task parallelism is limited by the number of jobs and by waiting.
- **Conclusion:** Many equal elements: data parallelism. A few different jobs: task parallelism. Real programs mix both.
- **Proof:** A pipeline of 4 instructions with 5 phases takes 5 + 3 = 8 cycles, not 20. A climate model uses task parallelism between components and data parallelism inside each. Six segments through four filters take 4 + 5 = 9 steps, not 24, and the slowest filter sets the pace.
- **Source:** `01-uvod-u-paralelno-programiranje.md`, "Levels of parallelism"; "Pipeline example: signal processing"; "Climate modeling".

### A4. Process, thread, scheduler, thread pool {#a4}
- **What:** Threads of one process share memory and have their own stack. The operating system scheduler switches a core to another thread at the end of a time slice, at a wait, or for a higher priority.
- **Why:** Shared memory makes communication easy. It also lets two threads change one variable. The program does not control the switch. A context switch costs 3.8 µs directly (Linux 2.6.17, 2 × Xeon 2.0 GHz, not the i7) and a few µs to over 1000 µs for the refilled cache.
- **Conclusion:** A task is the job and a thread is the worker. `Task.Run` is the recommended start; `StartNew` is not used in the course. Keep computing threads about equal to cores. The .NET thread pool decides which task gets which thread. The operating system decides when the thread gets a core.
- **Proof:** An empty .NET program has 8 threads. Exercise 2 task 2 has 25. 16 tasks run as fast as 12 (149 and 148 ms).
- **Source:** `01-uvod-u-paralelno-programiranje.md`, "Process and thread"; `03-amdahlov-zakon-i-modeli-programiranja.md`, "Thread states and the context switch"; `ex-02-parallel.md`, "Thread, ThreadPool, Task".

### A5. Flynn taxonomy {#a5}
- **What:** Two questions: one or many instruction streams, one or many data streams. Answers give SISD, SIMD, MISD, MIMD.
- **Why:** An instruction stream is the commands one control unit fetches. A data stream is the operands. The class tells which parallelism the hardware offers. A program cannot use what the hardware lacks.
- **Conclusion:** Choose the construct that fits the class: SIMD gives data parallelism, MIMD gives tasks. A computer is MIMD with SIMD units. Every C# `Task` runs on MIMD. SISD is the baseline for speedup.
- **Proof:** A 256-bit vector command adds 8 floats at once. A GPU warp runs one command on 32 threads. A branch slows SIMD, because some units wait. MISD is rare.
- **Source:** `02-flynn-granularnost-monte-carlo-pi.md`, "The four classes"; "SIMD: the same command, many data"; "Where is your computer".

### A6. Shared memory against separate memory {#a6}
- **What:** Shared memory: cores see the same RAM. Network of computers: each has its own memory and sends messages. Three programming models exist: shared memory with threads, message passing, data parallelism.
- **Why:** Shared memory is fast and gives data races. Separate memory is slower, scales, and gives deadlock, lost messages and wrong message order.
- **Conclusion:** Expect data races with shared memory, and deadlock and message order with messages. The model is not the hardware: students send messages on one laptop. Part 1 teaches shared memory in C#. Part 2 teaches messages.
- **Proof:** IBM Blue Gene/Q: 18 cores, 32 MB shared L2 cache, 16 compute cores.
- **Source:** `01-uvod-u-paralelno-programiranje.md`, "Computer architectures"; `03-amdahlov-zakon-i-modeli-programiranja.md`, "Programming models".

**How this connects.**
- The frame of failures (data race, deadlock, nondeterminism; small granularity, bad division, sequential part) is in `01-uvod-u-paralelno-programiranje.md`, section "What can go wrong". Each later chapter treats one row.
- A1 leads to B, because the cores must give a measurable gain.
- A6 leads to D (shared memory gives data races) and to G (separate memory and locks give deadlock).
- A2 leads to E7, because async/await is concurrency.
- A5 and A3 lead to C: MIMD gives task parallelism, SIMD gives data parallelism.

---

## B. Measuring {#ch-b}

**Core idea.** Speedup S = T(1)/T(N) and efficiency E = S/N measure gain. Amdahl gives the upper limit from the sequential part. Real results are lower for six causes. Karp-Flatt tells which cause dominates. Gustafson changes the question. Overhead and granularity decide whether small tasks pay off.

### B1. Speedup and efficiency {#b1}
- **What:** S(N) = T(1)/T(N). E(N) = S(N)/N. Ideal: S = N, E = 1. Lecture 1 writes U for S.
- **Why:** S says how many times faster. E says which part of the cores does useful work. Together they show how far the program is from ideal.
- **Conclusion:** Measure both. A fall of E shows a limit.
- **Proof:** Monte Carlo pi, T(1) = 936 ms. N = 2: S 2.00, E 1.00. N = 12: 148 ms, S 6.31, E 0.53.
- **Source:** `03-amdahlov-zakon-i-modeli-programiranja.md`, "Speedup and efficiency".

### B2. Amdahl law {#b2}
- **What:** S(N) = 1 / ((1 − P) + P/N). P is the part of the run time that can be divided. The limit is 1/(1 − P).
- **Why:** The sequential part always takes the same time. Only the parallel part shrinks. When N grows, P/N goes to 0, so S goes to 1/(1 − P).
- **Conclusion:** Before you add cores, reduce the sequential part. P is a share of time, not of statements.
- **Proof:** Limits: 2× (P = 50 %), 4×, 10×, 20×, 100× (99 %). P = 90 %, N = 10 gives 5.3. N = 100 gives about 9.2. Five statements, three parallel: S(3) = 1/(0.4 + 0.6/3) ≈ 1.67. 40 % sequential: 100 cores give 2.46, the limit is 2.5.
- **Source:** `01-uvod-u-paralelno-programiranje.md`, "Amdahl law: the limit of speedup"; `03-amdahlov-zakon-i-modeli-programiranja.md`, "Amdahl's law".

### B3. Why measured speedup is lower {#b3}
- **What:** Six causes: sequential part, overhead, bad division, indivisible tasks, hardware, memory.
- **Why:** Amdahl contains only the first cause, and a real program can have several. Hardware: 12 logical cores share 6 physical cores. Memory: cores share cache and the path to RAM.
- **Conclusion:** Amdahl is the best case.
- **Proof:** Five-statement program has only three independent tasks. On 2 cores real S = 5/4 = 1.25, Amdahl says 1.43. Speedup stops near 12 tasks (6.31 at 12, 6.28 at 16).
- **Source:** `03-amdahlov-zakon-i-modeli-programiranja.md`, "Indivisible tasks"; "Why the measured speedup is lower than Amdahl".

### B4. Karp-Flatt metric {#b4}
- **What:** e = (1/S − 1/N) / (1 − 1/N). It estimates the sequential fraction from measured S.
- **Why:** e is Amdahl solved for the sequential part 1 − P, with the measured S. If e stays constant, a real sequential part limits the program. If e grows with N, something that grows with tasks limits it: overhead or hardware.
- **Conclusion:** Use it after measuring. It does not predict and does not name the overhead.
- **Proof:** Pi: e = 0.000 at N = 2, 0.024 at N = 6, 0.103 at N = 16. Built-in f = 0.5: e stays near 0.54 to 0.596. For f = 0, N = 2, S = 2.11 gives e = −0.054 (small N is noisy, inferred).
- **Source:** `03-amdahlov-zakon-i-modeli-programiranja.md`, "Karp-Flatt metric".

### B5. Gustafson and which law to use {#b5}
- **What:** S(N) = N − s·(N − 1). The work grows with N and the time stays fixed.
- **Why:** Set the time on N cores to 1, with sequential part s. One core needs s + N(1 − s) for the same larger work, so S = N − s(N − 1). Amdahl fixes the work (strong scalability). Gustafson fixes the time (weak scalability). It is one law with a different starting point. The symbols s and 1 − P are measured in different places.
- **Conclusion:** Amdahl before code, Karp-Flatt after measuring, Gustafson when the work can grow.
- **Proof:** s = 0.1, N = 10: Gustafson 9.1, Amdahl 5.3. N = 64: 57.7 against 8.77.
- **Source:** `03-amdahlov-zakon-i-modeli-programiranja.md`, "Gustafson's law"; "Which law to use"; "Notation".

### B6. Overhead and granularity {#b6}
- **What:** Overhead (tara) is the cost that a sequential program does not pay: start tasks, wait, merge. T(k) ≈ T(seq)/k + overhead(k). Granularity is work per communication.
- **Why:** The pool starts threads one by one, fast tasks wait for the slowest, and results are merged. So overhead grows with the number of tasks. It does not shrink with cores. A task is a separate part of work, communication is data exchange, synchronization is waiting. Waiting leaves a core idle.
- **Conclusion:** Each task needs much more work than its overhead. Cost per operation also counts (see F5).
- **Proof:** 1000 units in 4 tasks take about 410, not 250. Pi with 12 tasks: N = 100: 1,9 µs sequential, 21,0 µs parallel. N = 10.000.000: 100.996,3 µs against 14.766,2 µs. The first clear gain (about 3 times) is at N = 100.000 (inferred; the source gives no exact threshold).
- **Source:** `02-flynn-granularnost-monte-carlo-pi.md`, "Granularity"; "Overhead (tara)"; "Trap 1: tiny tasks".

### B7. Measure correctly {#b7}
- **What:** Use the Release build. Warm up. Take the shortest of 3 runs (or the median of 21). Compare the parallel result with the sequential result.
- **Why:** Debug switches off optimizations, so its times have no meaning. The first call is slow, so a single run misleads. A wrong parallel program can look fast, so always check against the sequential result.
- **Conclusion:** A time without a correctness check has no value. F5 starts a debugger, even in Release.
- **Proof:** Exercise 2 prints "ispravan" or "POGREŠKA" for each k.
- **Source:** `ex-02-parallel.md`, "Time measurement"; `02-flynn-granularnost-monte-carlo-pi.md`, "Speedup"; `01-uvod-u-paralelno-programiranje.md`, "Course organization" (Debug reason); `ex-05-periodic-tasks.md`, "Setup and tools" (F5 debugger).

**How this connects.**
- Chain (inferred): granularity sets overhead (B6). Overhead and barrier waiting act like an extra sequential part, so Amdahl bounds them (B2) and Karp-Flatt e grows with N (B4). A barrier per level (44 for n = 2²²) is such a cost (F3). Agglomeration into p blocks removes it (F5). Brent's t is the unshrinkable part, like 1 − P (E6).
- B3 to D and E: lock and barrier waiting is overhead (D5, E5).
- B5 and B6: bigger work per task lowers the overhead share, which is the Gustafson case (inferred).
- B6 to C1 (agglomeration), C6 (tiny tasks), F5 (blocks), E5 (fewer processors).
- B2 to C7: the fix for a sequential chain is a new algorithm, not more cores.

---

## C. Splitting work {#ch-c}

**Core idea.** Find independent parts, divide them evenly, check correctness, then measure. Bernstein conditions define independence. The library schedules but never checks. Isolation (each task owns its data) removes most traps. A dependency chain needs a new algorithm.

### C1. Design steps: three questions and PCAM {#c1}
- **What:** Ask: what can run at the same time, how to divide, is it correct and faster. Foster PCAM: partitioning, communication, agglomeration, mapping.
- **Why:** The first two PCAM steps look for parallelism. The last two fit the machine. Compilers rarely prove independence. With a library, the programmer is responsible.
- **Conclusion:** Each course pitfall belongs to one PCAM step.
- **Proof:** Divide and conquer (merge sort): run sequentially below a threshold, because tiny tasks cost overhead. Prime count in k equal ranges: the last range runs longest, so use another division. Sum of 10⁹ numbers. Partitioning: one task per addition gives a data race. Agglomeration: k blocks of N/k, too small blocks mean overhead. Mapping: the remainder N/k is lost.
- **Source:** `01-uvod-u-paralelno-programiranje.md`, "Designing a parallel program"; `03-amdahlov-zakon-i-modeli-programiranja.md`, "Designing a parallel program: PCAM"; "Practice 3 preview"; "More examples of parallelization".

### C2. Domain and functional decomposition {#c2}
- **What:** Domain: each core gets part of the data. Functional: each task does a different function.
- **Why:** Neighbour cells need border exchange, so a smaller border means less communication. The slowest component sets the speed of a functional split.
- **Conclusion:** Choose the division with the smaller border. Give more cores to the slow component.
- **Proof:** An 8 × 8 array on 4 cores: strips have a border of 2 · 8 = 16 cells, blocks 2 · 4 = 8. The course gives no answer for 100 cores.
- **Source:** `03-amdahlov-zakon-i-modeli-programiranja.md`, "Domain decomposition"; "Functional decomposition".

### C3. Dependences and Bernstein conditions {#c3}
- **What:** I = variables read, O = variables written. Parts 1 and 2 may run together if I₁∩O₂ = ∅, O₁∩I₂ = ∅ and O₁∩O₂ = ∅.
- **Why:** If a condition breaks, the result depends on who is first. Shared reading is allowed.
- **Conclusion:** No part may write what another reads or writes.
- **Proof:** `a = x + y`, `b = x * 2`: run together. `x = 5` with either: not allowed. `zbroj += a[i]` in a loop breaks O₁∩O₂.
- **Source:** `03-amdahlov-zakon-i-modeli-programiranja.md`, "Bernstein's conditions".

### C4. A function as argument, and closure {#c4}
- **What:** A delegate is a type for a method signature. A lambda is a nameless function. `Task.Run` and `Parallel.For` take a delegate. A lambda keeps the variable, not its value.
- **Why:** The library decides where and when the body runs. If the variable changes before then, the lambda sees the new value.
- **Conclusion:** Copy the loop variable into a local variable inside the loop body.
- **Proof:** `faktor = 3` then 10: `puta(5)` gives 15 then 50. Loop with tasks: 40 40 40 40 instead of 0 10 20 30. Fix: `int j = i;`. A multicast delegate returns only the last value (5000, list length 2). `foreach` makes a new variable per iteration, `for` does not.
- **Also:** `Action` returns nothing, `Func` returns the last type argument, `Predicate` returns `bool`. LINQ `Where`, `Select`, `Aggregate` are Filter, Map, Reduce and take delegates. A generic `Stog<T>` takes delegates and throws `InvalidOperationException` on an empty pop.
- **Source:** `ex-01-delegates.md`, "Closure: a lambda captures a variable"; "Action, Func and Predicate"; "Generic class: Stog<T>"; "Multicast delegate"; `ex-02-parallel.md`, "Trap A".

### C5. Task, Result and Parallel.For {#c5}
- **What:** `Task.Run` starts the delegate on a pool thread and returns at once. `Result` blocks until the task ends. `Task.WaitAll` waits for many. `Parallel.For(0, n, ...)` ends only when all iterations end. The end index is excluded.
- **Why:** The return at once lets two tasks run together. The end of the loop acts as a barrier, so results are complete after it. A local variable per part removes the shared write.
- **Conclusion:** Start all tasks before the first wait. A loop is safe if each iteration writes only its own element.
- **Proof:** `Paralelno(f, g)`: f = 4500000001500000000, g = 216816, 1056 ms sequential, 833 ms parallel. Time is about max(tf, tg). Without `ToArray()`, the lazy `Select` starts tasks one by one (inferred). Isolation idiom: one result element per part, `Sum()` after the loop. `Parallel.For` with local state has three lambdas: start, body, end (one `Interlocked.Add` per task).
- **Source:** `ex-02-parallel.md`, "Task.Run and Result"; "Assignment 1"; "Assignment 2"; `04-data-race-lock-interlocked.md`, "Fix: synchronize less often".

### C6. Monte Carlo pi and the four traps {#c6}
- **What:** π ≈ 4K/N, with K points inside the quarter circle. Error falls as 1/√N, so a 10 times smaller error needs 100 times more points. Points are independent.
- **Why:** Each task can throw its own points. The traps break this simple split.
- **Conclusion:** Isolate: own `Random`, own counter, merge once. Give the last task the remainder.
- **Proof:** Trap 1, tiny tasks (B6). Trap 2, shared counter: pi = 0,30134 (871 ms), about 90 % of increments lost. Trap 3, shared `Random`: pi = 3,99995, 4203 ms against 998 ms sequential, because `Random` returns 0 without synchronization. Trap 4: N = 1.000.000.007, k = 8 loses 7 points. Isolation: pi = 3,14168 in 144 ms.
- **Source:** `02-flynn-granularnost-monte-carlo-pi.md`, "Trap 2" to "Trap 4"; "Fix: isolation".

### C7. Dependence chains: change the algorithm {#c7}
- **What:** A chain (Fibonacci, wavefront, odd-even sort) is not parallelized loop by loop. Group independent work into steps.
- **Why:** A sequential run time is a sum of steps, and a parallel time is set by the longest part and the waiting (inferred). If each step needs the previous result, extra cores do not help. Inside a step the tasks are independent. Between steps all cores wait.
- **Conclusion:** Look for what is independent. Larger tasks (a block per core) mean less waiting.
- **Proof:** Fibonacci is a prefix sum of 2 × 2 matrices (F1). Wavefront and odd-even sort: C8.
- **Source:** `03-amdahlov-zakon-i-modeli-programiranja.md`, "Dependency chain"; "Levenshtein distance and the wavefront"; "Odd-even sort"; "Wavefront and odd-even: the same pattern"; `01-uvod-u-paralelno-programiranje.md`, "Sequential program, parallel program, parallel problem".

### C8. Wavefront and odd-even sort {#c8}
- **What:** Wavefront: cell D[i][j] reads left, upper and upper-left cells, so one antidiagonal is independent. Odd-even sort: all pairs of one phase compare at once. Odd phase (1, 2), (3, 4). Even phase (0, 1), (2, 3).
- **Why:** An element moves one place per phase, so phases must alternate to move the pair borders. Pairs of one phase share no element (Bernstein). Wavefront waits for a real dependency. Odd-even waits because phases touch the same elements. Odd-even always runs n phases, also on a sorted array.
- **Conclusion:** Both are steps with a wait between. A block per core fixes both (sort a block, then merge-split with the neighbour).
- **Proof:** 6 × 6 table: m + n − 1 = 11 steps instead of 36. Seven reversed elements: 7 phases. 21 comparisons: 21 steps (bubble sort), 7 steps (odd-even, 3 cores). Work O(n²) against O(n log n). Other cases: N-body (all need new positions), Floyd (row k and column k), game trees (unequal branches).
- **Source:** `03-amdahlov-zakon-i-modeli-programiranja.md`, "Levenshtein distance and the wavefront"; "Odd-even sort"; "Wavefront and odd-even: the same pattern"; "More examples of parallelization".

**How this connects.**
- C3 to D: a broken Bernstein condition without synchronization is a data race (D2).
- C6 to D1 (shared counter is the data race) and to D3 (isolation is fix 1).
- C5 to E3 and F6: the end of `Parallel.For` is the barrier that those cards use.
- C2 to F5 and C8: a block per core is the domain split (inferred).
- C7 to F: the dependency chain is solved by the prefix sum tree (F1).
- C1 to E5: agglomeration equals "fewer processors, more work each" (inferred).

---

## D. What breaks {#ch-d}

**Core idea.** A multi-step operation on shared data interleaves, so an update is lost. The error is silent. Fix by not sharing. If sharing is necessary, make the whole operation indivisible, and do it rarely.

### D1. The data race mechanism {#d1}
- **What:** `brojac++` is three steps: load, add 1, write.
- **Why:** Each task has its own register. Two tasks load 5, both write 6. One increment is lost. The scheduler chooses the order, so the result changes per run.
- **Conclusion:** The result is wrong, different each run, and no error appears. One core can hide it (inferred).
- **Proof:** 12 tasks × 1.000.000 increments: 2.636.084, 2.201.987, 1.790.437 (78 to 85 % lost). The notes say one core is almost always correct.
- **Source:** `04-data-race-lock-interlocked.md`, "Trap: data race on a counter"; "Why a data race happens".

### D2. Data race against race condition {#d2}
- **What:** Data race: two threads access one location, one access writes, no synchronization. Race condition: correctness depends on the order or timing of steps.
- **Why:** A data race is unordered access to one location. A race condition is a rule over several steps that needs one order. A race condition can occur with every step protected (the source gives no example of the reverse). The counter is both.
- **Conclusion:** Protect each operation that must stay whole, not each variable.
- **Proof:** Account with 10.000 EUR, each step atomic: `stanje` −5, `isplata` 10.005.
- **Source:** `04-data-race-lock-interlocked.md`, "Data race and race condition"; "Trap: check, then change".

### D3. Four fixes and the choice {#d3}
- **What:** Isolation, immutability, mutual exclusion (lock), atomic operations (Interlocked).
- **Why:** Isolation and immutability give no write to shared data, so the Bernstein conditions hold (C3) and nothing needs a wait. Lock and Interlocked synchronize, so they cost time.
- **Conclusion:** Try isolation first. One variable and one operation: Interlocked. Several steps or variables: lock or compare-and-swap loop. Collections: lock, local collections or concurrent collections.
- **Proof:** Sum, pi, counter: isolation. Input array: immutability. Counter: Interlocked. Account: lock or CAS. List of results: local lists.
- **Source:** `04-data-race-lock-interlocked.md`, "Four fixes"; "What to choose"; `ex-03-data-race.md`, "What the student should know after week 4".

### D4. Lock, critical section, lock rules {#d4}
- **What:** A critical section lets one task in at a time. `lock (o)` compiles to `Monitor.Enter` and `Monitor.Exit` in `try/finally`.
- **Why:** Waiting is the price. A lock protects only against code that locks the same object.
- **Conclusion:** Use a private lock object. Never `lock (this)`, `lock (typeof(X))` or a string. Keep the section short. Do not wait inside it. No `await` inside.
- **Proof:** Two tasks, two increments each (exercise 3 page): 924 orders without protection, 6 with a shared lock, 184,756 with a lock per task (the lock removes no order; inferred from the step counts in `ex-03-data-race.md`). Each task its own lock object: 1.981.047. One shared lock object: 12.000.000, but 427 to 449 ms against 5 ms sequential.
- **Source:** `ex-03-data-race.md`, "Counting all possible orders"; `04-data-race-lock-interlocked.md`, "Rules for the lock statement"; "Trap: each task its own lock object"; "Fix with the lock statement".

### D5. Interlocked and the cost of synchronizing {#d5}
- **What:** Interlocked makes one operation on one variable indivisible: Increment, Decrement, Add, Exchange, CompareExchange, Read.
- **Why:** The processor does it. It still makes all cores compete for one memory location (more cores then add waiting, inferred). Per-step Interlocked is about 40 times slower than sequential (203 against 5 ms). All per-step methods are 40 to 140 times slower.
- **Conclusion:** Synchronize once per task, not per step.
- **Proof:** Best of 3, 12 tasks, 12.000.000 increments: sequential 5.0 ms, no protection 12.0 (wrong), lock 456.0, `Lock` type 711.0, Interlocked 203.0, local then one `Add` 1.0, `Parallel.For` with local state 8.0. The exercise author's machine: lock about 155 ms, Interlocked about 246 ms. Measure on your computer.
- **Source:** `04-data-race-lock-interlocked.md`, "Cost of synchronization"; "Fix: synchronize less often"; `ex-03-data-race.md`, "Interlocked and local counter".

### D6. Check, then change, and compare-and-swap {#d6}
- **What:** `CompareExchange(ref x, novo, staro)` writes `novo` only if `x` still equals `staro`, and returns what it found.
- **Why:** If another task was faster, nothing is written, and the loop reads again.
- **Conclusion:** Lock is simpler and works for several variables. CAS needs no lock object but needs a retry loop.
- **Proof:** Lock around both steps or CAS loop: `stanje` 0, `isplata` 10.000. The same pattern appears for "add key if absent" and "reserve if free".
- **Source:** `04-data-race-lock-interlocked.md`, "Fix: check and change together".

### D7. Collections and the lost node {#d7}
- **What:** `List<T>.Add` has three steps. A linked stack `Push` has two: link, then set top.
- **Why:** Two tasks read the same top. The second write overwrites the first. A node is lost.
- **Conclusion:** Ordinary collections are not safe for concurrent writes. Reads together are safe.
- **Proof:** List: 159.341 of 1.200.000, sometimes `ArgumentException`. Stack without protection: 188.117 nodes. CAS push: 1.200.000, none lost. `ConcurrentStack<T>` uses CAS.
- **Source:** `04-data-race-lock-interlocked.md`, "Trap: List<T> from many tasks"; "How an element is lost"; `ex-03-data-race.md`, "Zadatak 3".

**How this connects.**
- D1 builds on C6 and C3. D5 and F5 share one rule: one synchronization per task, which a block gives (inferred).
- D4 leads to G1: two locks in different order give deadlock. D4 leads to G5: `Monitor.Wait` releases the lock and retakes it, so a `while` is needed.
- D7 leads to G6, the ready-made concurrent collections.
- D6 returns in G6 as "check, then take".
- D4 and E7: `await` is not allowed inside `lock` (E7).

---

## E. Models and async {#ch-e}

**Core idea.** A model lists what an analysis counts. RAM counts steps. PRAM counts steps and treats memory and waiting as free. aPRAM adds d and B. The gap between model and machine causes the lock step trap. A barrier fixes it and costs more with more tasks. async/await is waiting without a thread. Cancellation is an agreement.

### E1. RAM and the model's trap {#e1}
- **What:** RAM (random access machine, not RAM memory): one processor, each operation takes 1.
- **Why:** RAM does not see slower shared memory or waiting.
- **Conclusion:** Equal RAM cost can mean very different time.
- **Proof:** 12,000,000 increments: 1 ms against 456 ms, 450 times.
- **Source:** `05-modeli-racunala-i-async-await.md`, "RAM: the model of a sequential computer"; "The trap of the model".

### E2. PRAM and its variants {#e2}
- **What:** p RAM processors, one shared memory, a common clock (lock step). EREW, CREW, CRCW differ in allowed concurrent access. CRCW write rules: COMMON, ARBITRARY, PRIORITY.
- **Why:** PRAM hides communication so it shows the pure parallelism of a problem. A weaker model runs on a stronger one unchanged. The reverse costs O(log p) per CRCW step.
- **Conclusion:** Order: PRIORITY ≥ ARBITRARY ≥ COMMON ≥ CREW ≥ EREW. PRAM with p processors and t steps takes at most p·t steps on one processor. PRAM is physically impossible. Its value is not real time.
- **Proof:** "Is there a zero?": CRCW 1 step, EREW tree log₂ 8 = 3 steps. A PRAM step has read, compute and write phases. Five ready PRAM access examples are in `ex-04-models-async.md`, "One PRAM step".
- **Source:** `05-modeli-racunala-i-async-await.md`, "PRAM"; "EREW, CREW and CRCW"; "How the models connect"; `ex-04-models-async.md`, "One PRAM step"; "6. Is there a zero in the array? CRCW in one step, EREW in log n".

### E3. The lock step trap and the barrier {#e3}
- **What:** Head of a linked list: step 1 `M[i] := 0`, step 2 `M[next[i]] := 1`, step 3 head where `M[i] = 0`. It is correct on EREW PRAM in 3 steps, O(1).
- **Why:** In C# a task runs all three steps for its node. A predecessor can write 1 before or after node i writes 0. Either way node i reports itself as head.
- **Conclusion:** The algorithm is correct only in lock step. Put a barrier between dependent steps.
- **Proof:** n = 1000: 1000 heads, also in a plain `for`. Three `Parallel.For` or `Barrier`: 1 head (654).
- **Source:** `05-modeli-racunala-i-async-await.md`, "Example: the head of a linked list in PRAM"; "Trap"; "Fix: a barrier between steps"; `ex-04-models-async.md`, "Task 1".

### E4. aPRAM: phases, d and B {#e4}
- **What:** No common clock. Local access 1, global access d, barrier B (2 ≤ d ≤ B ≤ p). Phases split by barriers. No read of a location that another writes in the same phase.
- **Why:** The phase rule forbids a data race. Consecutive global accesses overlap: k accesses take d + k − 1.
- **Conclusion:** aPRAM shows what parallelism costs. Correctness must hold for any delay.
- **Proof:** d = 4, B = 16 (invented): one PRAM step costs 2B + 2d + 1 = 41. Trace with P3 late by 10: 71 with barriers (head 3), 23 without (heads 0 and 3, wrong). BSP and LogP also count communication.
- **Source:** `05-modeli-racunala-i-async-await.md`, "aPRAM"; "The phase rule"; "Tracing execution on aPRAM"; "After PRAM: BSP and LogP".

### E5. The barrier gets expensive: fewer processors {#e5}
- **What:** Barrier time grows with p. One aPRAM processor can simulate B PRAM processors.
- **Why:** In the invented example one PRAM step costs 41 units, and 32 of them are barriers (E4). More processors make each barrier longer.
- **Conclusion:** Use fewer tasks with more work. This is agglomeration (inferred name, from C1).
- **Proof:** Barrier: 52 ns (1 task), 3,113 (12), 19,037 (24). About 60 times from 1 to 12. With p/B processors: 5B + 2d − 2 = 86 per step, work 5,504 against 41,984, about 7.6 times less. `Parallel.For` gives one task a range.
- **Source:** `05-modeli-racunala-i-async-await.md`, "Trap: the barrier gets expensive"; "Fix: p / B processors"; "In C#: one task for many iterations".

### E6. Work, steps and the Brent rule {#e6}
- **What:** t = steps with enough processors. q = total operations (work). With p processors: at most t + (q − t)/p steps.
- **Why:** The t steps cannot shrink, like the sequential part 1 − P (inferred). The other q − t operations spread over p processors. So time is t + (q − t)/p. The rule needs no new algorithm. It ignores task overhead.
- **Conclusion:** Steps alone do not decide. Work matters. (The word "span" for t is inferred.)
- **Proof:** Blelloch: 44 + 8.388.562/12 ≈ 699.091. Step-by-step prefix sum: about 7,3 million, worse than sequential 4.194.303.
- **Source:** `06-algoritam-sume-prefiksa.md`, "15. Brent rule".

### E7. async/await {#e7}
- **What:** `await` frees the thread to the pool during a wait. It adds no core.
- **Why:** Waiting needs no core. An `async` method without `await` runs to the end in the caller's thread. `Thread.Sleep`, `Wait`, `Result` hold a thread.
- **Conclusion:** Use async/await to wait. Use `Task.Run` or `Parallel.For` to compute. Never `await` inside `lock`, because the thread may change.
- **Proof:** Three waits of 1 s: 3003 ms one by one, 1001 ms with `WhenAll`. 100 waits: 1004 ms. `Racunaj` without `Task.Run`: 1686 ms, with: 513 ms. 100 × `Sleep`: 7006 ms.
- **Source:** `05-modeli-racunala-i-async-await.md`, "async/await: concurrency"; "Trap: async does not mean parallel"; "Trap: blocking in the middle of async code".

### E8. Cancellation is an agreement {#e8}
- **What:** A task cannot be stopped from outside. `Cancel()` and `CancelAfter(ms)` set a request. The task checks the token. `Sleep` ignores it. `WaitOne(ms)`, `Task.Delay(ms, token)` and `SignalAndWait(token)` react.
- **Why:** A task that only leaves its loop has status `RanToCompletion`. To get `Canceled`, throw `OperationCanceledException` and pass the token also at creation. Without it the status is `Faulted`. A token already canceled means the task never starts.
- **Conclusion:** Wait with a token-aware call. `Wait()` on a canceled task throws `AggregateException`.
- **Proof:** `Sleep`: stop 592 ms late. `WaitOne`: about 1 ms. 100 `Task.Delay` with `CancelAfter(300)`: 324 ms, 100 of 100 canceled.
- **Source:** `ex-05-periodic-tasks.md`, "Topic 1"; "Topic 2"; "Topic 3"; "Zadatak 1"; `ex-04-models-async.md`, "Task 3".

### E9. Periodic tasks, result return, slow barrier {#e9}
- **What:** The k-th call must run at (k − 1)·ms. Sleep `Math.Max(0, k·ms − elapsed)`. `Periodicno2<U>` keeps a local list and returns it as `Result`. A barrier waits for the slowest task.
- **Why:** A fixed sleep adds the action time, so the schedule drifts. A shared list read while written is a data race. A correct barrier program can still be slow.
- **Conclusion:** Return data after the task ends. Give a barrier wait a token and a time limit.
- **Proof:** Action of 300 ms every 1000 ms: 10 calls in 9.5 s (call 10 at 9007 ms). Stalled barrier task: 10016 ms; with a 2000 ms token: 2010 ms.
- **Source:** `ex-05-periodic-tasks.md`, "Topic 5"; "Topic 6"; "Topic 7"; "Zadatak 2"; "Zadatak 5".

**How this connects.**
- E3 and E4 to F6: the same lock step trap appears in the prefix sum across levels.
- E5 to B6 and F5: tasks must carry work.
- E7 to A2 (concurrency) and to D4 (no `await` in `lock`).
- E9 to D7: a shared list needs a lock on both sides (inferred).
- G6 and E8 (inferred): `BlockingCollection` ends by `CompleteAdding`, a task ends by a token. Both are end signals.
- E2 to F1: the tree algorithm is EREW.

---

## F. Prefix sum family {#ch-f}

**Core idea.** A problem that looks sequential can have a log-step parallel algorithm. The method needs an associative operation and a tree. Correct and fast needs work-efficiency, a correct operation, a barrier per level and enough work per task.

### F1. Scan, prescan, reduction {#f1}
- **What:** Scan: a₀, a₀⊕a₁, ... Prescan: I, a₀, ..., the sum without the element itself. Reduction: one total. I is the neutral element: 0 for +, 1 for ·, the smallest number for max.
- **Why:** The tree writes the sum of everything left of i without aᵢ. So it gives a prescan. A scan is a prescan shifted left, plus the total.
- **Conclusion:** Know the three outputs. Different uses change only ⊕.
- **Proof:** Input 3 1 7 0 4 1 6 3: scan 3 4 11 11 15 16 22 25, prescan 0 3 4 11 11 15 16 22, reduction 25.
- **Source:** `06-algoritam-sume-prefiksa.md`, "1. The prefix sum problem".

### F2. Three failed attempts {#f2}
- **What:** (1) `Parallel.For` over `b[i] = b[i-1] + a[i]`. (2) Step by step (Hillis and Steele) in one array. (3) Step by step with two arrays.
- **Why:** (1) is a loop-carried dependency. (2) reads and writes one array in one step: a data race. (3) is correct but does n log₂ n − (n − 1) additions.
- **Conclusion:** Few steps are not enough. The goal is O(log n) steps and O(n) work. On a PRAM, (2) is correct, because all read before any write. Two processors read `b[1]` in one step, so it is CREW. C# has no lock step.
- **Proof:** n = 2²². (1) 4.162.572 wrong of 4.194.304. (2) 4.194.302 wrong. (3) 0 wrong, 88.080.385 additions (21 times more), 405,3 ms against 56,5 ms.
- **Source:** `06-algoritam-sume-prefiksa.md`, "2. Trap 1"; "6. Trap 2"; "7. Trap 3".

### F3. Blelloch up-sweep and down-sweep {#f3}
- **What:** Up-sweep: the sum of a pair goes to the right child. Down-sweep: root gets I, left child copies the parent, right child = parent ⊕ the old left value.
- **Why:** The reduction keeps left subtree sums. The down-sweep gives each node everything left of it: A for the left child, A ⊕ B for the right child.
- **Conclusion:** 2 log₂ n steps and 2(n − 1) operations, so work-efficient. Parallelize inside a level only.
- **Proof:** Array after reduction: 3 4 7 11 4 5 6 25. Down-sweep ends with 0 3 4 11 11 15 16 22. n = 2²²: 8.388.606 operations, 44 barriers.
- **Source:** `06-algoritam-sume-prefiksa.md`, "8. Blelloch phase 1"; "9. Blelloch phase 2"; "10. Steps and work".

### F4. Traps in the operation {#f4}
- **What:** The tree changes the brackets, so ⊕ must be associative. It never swaps operands, so ⊕ need not be commutative. Pad with I.
- **Why:** Float addition is not exactly associative. Swapped down-sweep operands break strings. Padding with 0 puts a value in a max.
- **Conclusion:** Always compute left ⊕ right. Pad with the neutral element. Do not compare floats with `==`.
- **Proof:** Subtraction: −5 instead of −19. Float sums: 2.097.121,0 sequential, 2.097.140,8 tree, 2.097.140,73 double. Strings: "cab" instead of "abc". Sum for n = 20 without padding: 74, not 210. Exercise 6 names four traps: different result, works for 16 not for 20, parallel is slower, works for + not for your operation.
- **Source:** `06-algoritam-sume-prefiksa.md`, "11. Trap 4"; "12. Trap 5"; "17. When n is not a power of 2"; `ex-06-prefix-sum.md`, "Zadatak 0"; "The four traps of the day".

### F5. Granularity: three forms and blocks {#f5}
- **What:** Task per pair, `Parallel.For` per level, p blocks. Blocks: sum each block, prescan the p sums, prescan each block from its offset.
- **Why:** The overhead of a task is larger than one addition. An expensive ⊕ makes tasks pay off.
- **Conclusion:** Use blocks. The tree then runs on p sums only. Block sizes differ by at most 1.
- **Proof:** n = 2¹⁸: sequential 0,51 ms, per level 4,91, task per pair 918,05. Block time 2(⌈n/p⌉ + ⌈lg p⌉) = 699.060 for n = 2²², p = 12. Paper task n = 20, p = 8: block sums 12 7 18 17 3 6 9 7, total 79 (offsets 0 12 19 37 54 57 63 72 are a worked answer, inferred).
- **Source:** `06-algoritam-sume-prefiksa.md`, "13. Trap 6"; "14. Fewer processors: blocks"; "16. Blocks in C#"; `ex-06-prefix-sum.md`, "Zadatak 3"; "Paper task for home".

### F6. Barrier between levels, uses {#f6}
- **What:** `Parallel.For` over the levels runs levels together. One `Parallel.For` per level uses its end as a barrier.
- **Why:** Level r + 1 reads a sum that level r has not written.
- **Conclusion:** Parallelize inside a level, never across levels. The scan is a building block, and only ⊕ changes: compaction (+), radix sort (+), line of sight (max), string comparison ("first different"), recurrence xᵢ = aᵢ·xᵢ₋₁ + bᵢ (on pairs).
- **Proof:** Correct 207.616.572. Across levels: 204.141, 6.034.845, 8.537.133. Barriers for n = 2²⁰: 40 (inferred count in the exercise file).
- **Source:** `06-algoritam-sume-prefiksa.md`, "19. Link to the models: the barrier"; "18. Where the prefix sum is used"; `ex-06-prefix-sum.md`, "Closing questions".

### F7. Applications: GCD and binary numbers {#f7}
- **What:** GCD of rows: tree reduction with a guard for a missing partner, so length need not be 2^k. Binary numbers: each bit is a part (value, length). `Spoji(x, y) = ((x.v << y.l) | y.v, x.l + y.l)`, neutral (0, 0).
- **Why:** GCD is associative and commutative (inferred). Joining bit strings is associative but not commutative (inferred), so operand order matters. Nested parallel rows with a tree inside each row make many small tasks (inferred).
- **Conclusion:** Parallelize at one level with enough work per task. Scan is not only for +.
- **Proof:** GCD times (sequential, tree per row, sequential in row): 12 × 1.000.000: 42,6 / 33,2 / 7,8 ms. 100.000 × 10: 4,8 / 117,2 / 2,1 ms. `Prefiksi` of `10100100`: 1 2 5 10 20 41 82 164.
- **Source:** `ex-06-prefix-sum.md`, "Zadatak 4"; "Zadatak 5".

**How this connects.**
- F1 solves the chain of C7 and Fibonacci.
- F2 reuses D1 and C3. F6 reuses E3 and E4.
- Odd-even sort (O(n²) work, C7) and the step-by-step scan (F2) both fail on work-efficiency. Wavefront and odd-even steps are PRAM-like steps with a barrier (inferred).
- F7 shows the non-commutative case of F4.
- F5 uses B6 and E5. F5 and E6 give the work-and-steps view.
- F4 shows that exercise results depend on the operation, so test with a non-commutative one.

---

## G. Deadlock, semaphores, concurrent collections {#ch-g}

**Core idea.** Each tool has a trap, and each trap has a fix. Deadlock needs four conditions, so removing one prevents it. Livelock and starvation are other failures. A semaphore allows at most K. Wait/Pulse and BlockingCollection solve producer and consumer.

### G1. Deadlock and the four conditions {#g1}
- **What:** Deadlock: tasks wait for each other forever. Conditions (Coffman): mutual exclusion, hold and wait, no preemption, circular wait.
- **Why:** All four must hold. The circle forms only if each task takes its first lock before any task takes its second, so deadlock does not appear in every run.
- **Conclusion:** Remove one condition: mutual exclusion cannot go for real resources (avoid sharing); hold and wait, request all at once (one lock for both); no preemption, return what you hold (`Monitor.TryEnter`, then `Exit`); circular wait, same order (the cheapest). The program shows no exception and uses no processor time.
- **Proof:** Task 1 A then B, task 2 B then A: stop. With the same order: 201 ms.
- **Source:** `07-zastoj-semafori-konkurentne-kolekcije.md`, "Four necessary conditions of deadlock (Coffman)"; "Prevention: remove one condition"; `04-data-race-lock-interlocked.md`, "New trap: deadlock".

### G2. Dining philosophers and three fixes {#g2}
- **What:** Five forks, each philosopher takes left then right. Fixes: lower fork first (order), at most four at the table (waiter), return and retry.
- **Why:** Order and waiter remove circular wait. Return and retry removes no preemption, but gives livelock.
- **Conclusion:** "Return and try again" is not a fix. The order is.
- **Proof:** 5 of 5 attempts deadlock after 3 s. Order: 11798 ms, 100000 meals each. Waiter (`SemaphoreSlim(4)`): 12294 ms. Livelock in the same rhythm: 0 meals, 2.9 to 3.8 million attempts. State check: default 82 states, 1 deadlock; order 70, 0; waiter 491, 0.
- **Source:** `07-zastoj-semafori-konkurentne-kolekcije.md`, "Trap: everybody takes the left fork"; "Fix 1"; "Fix 2"; "Livelock"; "Fixes and Coffman conditions".

### G3. Livelock, starvation, fairness {#g3}
- **What:** Livelock: tasks run but make no progress. Starvation: others progress, one never gets the resource.
- **Why:** `lock` gives no order guarantee, so starvation is allowed. The ratio 1.1 shows it did not occur. Starvation-freedom implies deadlock-freedom, not the reverse.
- **Conclusion:** Starvation is possible and rare. A semaphore gives no FIFO or LIFO.
- **Proof:** 12 tasks, 1 s: fewest 23,007 entries, most 26,002, ratio 1.1. Nobody stayed without an entry.
- **Source:** `07-zastoj-semafori-konkurentne-kolekcije.md`, "Starvation"; "Is lock fair? Measured".

### G4. Semaphore {#g4}
- **What:** `SemaphoreSlim(K)`: `Wait` takes a permit, `Release` returns it. At most K tasks inside.
- **Why:** An exception skips `Release`, so the permit is lost. Anybody can release, and extra releases raise the count above K.
- **Conclusion:** Put `Release` in `finally`. Use a semaphore for "at most K". Use a lock for data.
- **Proof:** 20 tasks of 50 ms: no semaphore 12 at once, 109 ms. `SemaphoreSlim(3)`: 3 at once, 361 ms. Release skipped: 7 of 15 finished; with `finally`: 15 of 15. The car park of exercise 7 must free its permit also when a car breaks down (inferred link to `finally`).
- **Source:** `07-zastoj-semafori-konkurentne-kolekcije.md`, "Semaphore: at most K at the same time"; "Trap: Release is skipped"; "A semaphore is not a lock".

### G5. Waiting for a condition {#g5}
- **What:** Consumer: `lock` then `while (queue empty) Monitor.Wait`. Producer: enqueue and `PulseAll`. `Wait` releases the lock and retakes it before it returns.
- **Why:** After wake-up the condition may not hold. A pulse is not remembered.
- **Conclusion:** Use `while`, not `if`. Check the condition before `Wait`.
- **Proof:** 4 consumers, 200,000 items: `if` gave 26 `Dequeue` calls on an empty queue, `while` 0. A pulse before `Wait`: the `Wait` expired after 1 s.
- **Source:** `07-zastoj-semafori-konkurentne-kolekcije.md`, "Waiting for a condition"; "Trap: if instead of while"; "Trap: a pulse is not remembered".

### G6. Concurrent collections and BlockingCollection {#g6}
- **What:** ConcurrentQueue, Stack, Bag, Dictionary, BlockingCollection synchronize themselves. BlockingCollection has a capacity, `CompleteAdding` and `GetConsumingEnumerable`.
- **Why:** Capacity stops a fast producer from filling memory. `while (!IsCompleted) Take()` is "check, then take", an example of "check, then change".
- **Conclusion:** Use `GetConsumingEnumerable`. It waits and takes as one operation.
- **Proof:** 4 consumers, 20 runs: `Take` threw 54 times with the check, 0 with `foreach`. Capacity 100: at most 100 elements.
- **Source:** `07-zastoj-semafori-konkurentne-kolekcije.md`, "Concurrent collections"; "BlockingCollection and capacity"; "Trap: check, then take".

**How this connects.**
- G1 extends D4 (locks) and D6 (check, then change reappears in G6).
- G4 is the waiter of G2. G5 and G6 are two forms of the same producer and consumer problem.
- Part 2 of the course: message passing brings the same deadlock and order problems (A6).

---

## 4. Connection map {#sec-4}

| If you see | Think | Because (source) |
|---|---|---|
| Speedup far below k | Karp-Flatt e against N | e constant means sequential part, e growing means overhead or hardware (it does not name the overhead) (`03-...`, Karp-Flatt metric) |
| Speedup flat above 12 tasks | Logical cores and pool | 6 physical, 12 logical cores (`02-...`, Speedup) |
| Parallel slower than sequential on small input | Granularity | Overhead exceeds work: N = 100 gives 21,0 against 1,9 µs (`02-...`, Trap 1: tiny tasks) |
| Result changes per run, no error | Data race | Three-step update interleaves (`04-...`, Why a data race happens: brojac++ is three steps) |
| Counter too small | Lost update | Both tasks load the same old value (`04-...`, Trap: data race on a counter) |
| `+=` on a shared variable in `Parallel.For` | Local sum per task | O₁∩O₂ is not empty (`01-...`, Designing a parallel program; `03-...`, Bernstein's conditions) |
| Same Id or 40 40 40 40 | Closure over loop variable | Lambda keeps the variable (`ex-01-...`, Closure: a lambda captures a variable) |
| pi near 4 and slow | Shared `Random` | `Random` is not thread safe and gives 0 (`02-...`, Trap 3: shared generator (Random)) |
| Lock exists, result wrong | Lock object per task | Lock protects only against the same object (`04-...`, Trap: each task its own lock object) |
| Every step atomic, result wrong | Race condition | Check and change are two steps (`04-...`, Data race and race condition) |
| Elements vanish from a list or stack | Collection not thread safe | `Add` and `Push` have several steps (`04-...`, Trap: List<T> from many tasks) |
| Program stands still, no CPU | Deadlock | Four Coffman conditions (`07-...`, Four necessary conditions of deadlock (Coffman)) |
| Program runs hot, no progress | Livelock | Return and retry in the same rhythm (`07-...`, Livelock: "return and try again") |
| Semaphore blocks forever after exceptions | `Release` in `finally` | Exception skips `Release` (`07-...`, Trap: Release is skipped) |
| `Dequeue` on empty queue after wake-up | `while`, not `if` | Condition may not hold after `PulseAll` (`07-...`, Trap: if instead of while) |
| `Take` throws at the end | `GetConsumingEnumerable` | Check, then take is not atomic (`07-...`, Trap: check, then take) |
| 1000 heads from the list algorithm | Barrier between steps | Correct only in lock step (`05-...`, Trap: the same algorithm without lock step) |
| Barrier cost jumps at 24 tasks | Fewer tasks, more work each | B grows with p (`05-...`, Trap: the barrier gets expensive) |
| `async` method with no `await` | `Task.Run` | Runs in the caller's thread (`05-...`, Trap: async does not mean parallel) |
| 100 × `Thread.Sleep` takes 7 s | `await Task.Delay` | Sleep holds a thread (`05-...`, Trap: blocking in the middle of async code) |
| `Cancel()` does not stop the task | Task must check the token | Cancellation is an agreement (`ex-05-...`, Topic 1: Cancellation is an agreement (CancellationToken)) |
| Periodic job drifts | Sleep `k·ms − elapsed` | Fixed sleep adds the action time (`ex-05-...`, Topic 5: Periodic work without drift) |
| Loop reads `b[i-1]` | Prefix sum tree | Associativity gives log steps (`06-...`, 3. The idea: associativity and the tree) |
| Scan wrong at all positions | Two arrays or Blelloch | One array in a step is a data race (`06-...`, 6. Trap 2: one array inside a step) |
| 21 times more additions | Not work-efficient | n log₂ n against 2(n − 1) (`06-...`, 7. Trap 3: correct, but too much work) |
| Wrong string or subtraction result | Associativity and operand order | Tree changes brackets, never swaps operands (`06-...`, 11. Trap 4: associativity; 12. Trap 5: operand order) |
| Max over negatives gives 0 | Neutral element | Padding with 0 adds a value (`06-...`, 17. When n is not a power of 2: padding and trap 7) |
| Result wrong for n = 20, right for 16 | Padding or blocks | Tree needs n = 2^k (`ex-06-...`, Zadatak 0: tree indexes) |

## 5. Decision guide: a new parallel problem {#sec-5}

1. **Is the problem worth it?** Estimate P and apply Amdahl.
2. **What is independent?** Check Bernstein conditions on the parts.
3. **Is there a dependence chain?** If yes, look for a changed algorithm: wavefront, tree, prefix sum.
4. **Data or task parallelism?** Many equal elements: data. Few different jobs: task.
5. **Which unit?** Compute work: `Task.Run` or `Parallel.For`. Waiting work: `async`/`await`.
6. **How to divide?** Equal blocks, about one per core, last block takes the remainder.
7. **Can tasks avoid sharing?** Give each task its own variable, array element or list. Merge once.
8. **If sharing is needed, which tool?** One variable, one operation: Interlocked. Several steps: lock or CAS loop. At most K: semaphore. Wait for a condition: `while` plus `Wait`, or BlockingCollection.
9. **Do steps depend on earlier steps?** Add a barrier between them, and parallelize inside a step only. Reason: lock step does not exist in C# (E3, F6).
10. **Do you take two locks?** Always in the same order. Reason: this removes circular wait (G1).
11. **Does the task need to stop?** Pass a token, check it, and wait with token-aware calls. Reason: stop is an agreement (E8).
12. **Measure.** Release, warm-up, shortest of 3, compare the result with sequential, compute S, E and e. Reason: a wrong program can look fast, and e shows whether the sequential part or overhead and hardware limit S (B7, B4).

## 6. Pitfall catalogue {#sec-6}

Section 4 holds the see-think rows. This table lists symptoms with their fix. Traps already in section 4 are not repeated here.

| Symptom | Cause | Fix | Where taught |
|---|---|---|---|
| Unequal parts, one core works | Bad division | Equal blocks | B3, C1 |
| Debug times | Optimizations off | Release | B7 |
| No gain: indivisible tasks | Fewer tasks than cores | More, smaller tasks | B3 |
| Correct but slower than sequential | Sync per iteration | Local, then once | D5 |
| Model says all costs 1 | RAM ignores memory | aPRAM | E1, E4 |
| Read of foreign write in a phase | Phase rule broken | Two phases | E4 |
| `await` in `lock` | Thread may change | Not allowed | E7, D4 |
| Status `Faulted`, not `Canceled` | Token not passed at creation | Pass token twice | E8 |
| Shared list read while written | Data race | Return list at end, or lock both sides (inferred) | E9, D7 |
| Barrier waits for slow task | Slowest sets time | Token with timeout | E9 |
| Task per pair 918,05 ms (trap 6) | Tiny tasks | Blocks | F5 |
| Nested parallel rows, slow (ex. 6 task 4) | Many small tasks (inferred) | One parallel level | `ex-06-prefix-sum.md` |
| Parallel scan across levels: 204.141 | Levels run together | One `Parallel.For` per level | F6 |
| Float sum differs from sequential | Float add is not associative | Do not compare with `==` | F4 |
| Amdahl and Gustafson mixed | s and 1 − P differ | Name where each is measured | B5 |
| Karp-Flatt e negative at N = 2 | Noise at small N (inferred) | No fix in source | B4 |
| One core hides the data race | Few real overlaps (inferred) | Test on many cores (inferred) | D1 |
| Pulse before `Wait` | Pulse not remembered | Check condition first | G5 |

## 7. Numbers and formulas worth remembering {#sec-7}

| Formula | Example from the course | Source file |
|---|---|---|
| S = T(1)/T(N), E = S/N | S 6.31, E 0.53 at N = 12 | `03-...` |
| Amdahl: 1/((1−P)+P/N) | P = 90 %, N = 10: 5.3; limit 10 | `01-...`, `03-...` |
| PRAM step on aPRAM: 2B + 2d + 1 | 41 for d = 4, B = 16 | `05-...` |
| Overhead: T(k) ≈ T(seq)/k + overhead(k) | 1000 units, 4 tasks: about 410 | `02-...` |
| Karp-Flatt: e = (1/S − 1/N)/(1 − 1/N) | N = 8, S = 5.71: 0.057 | `03-...` |
| Gustafson: S = N − s(N−1) | s = 0.1, N = 64: 57.7 | `03-...` |
| Brent: t + (q − t)/p | 44 + 8.388.562/12 ≈ 699.091 | `06-...` |
| Step-by-step scan work: n log₂ n − (n−1) | 88.080.385 for n = 2²² | `06-...` |
| Blelloch: 2 log₂ n steps, 2(n−1) ops | 44 steps, 8.388.606 | `06-...` |

## 8. Conflicts and open points across sources {#sec-8}

| Point | What is known |
|---|---|
| Sequential pi time | 998 ms (L2 S16, S18, S19 notes), 936 ms (L2 S21, L3), 927 ms for k = 1. The source does not explain. Different runs (inferred). |
| Median or best | L2 S14 uses median of 21. L2 S21 and ex. 2 use shortest of 3. Both stated. |
| Growth curves | L1 S18 reconstructed 52 % (1986-2003), S19 measured 40 % (1988-2003). Do not compare directly. |
| Lock against Interlocked | Lecture: lock 456 ms, Interlocked 203 ms (about two times faster). Exercise comments: lock about 155 ms, Interlocked about 246 ms. L4 S9 says about 440 ms (427, 443, 449) while S13 says 456. No explanation. Measure on your computer. |
| Once-per-task claim | L4 S13 says faster than sequential. `Parallel.For` with local state is 8 ms, slower than 5 ms. Only the 1 ms row is faster. |
| Unprotected counter time | L4: 12 ms, slower than sequential 5 ms. Ex. 2 trap B: code comment about 80 ms fixed against about 23 ms unprotected. Slide: 21,9 ms unprotected, 3,0 ms fixed, "about 7 times slower". Conflict on which is faster. |
| One core and data race | L4 S3 says the error is "rare". Notes say the result is "almost always correct". |
| Share of lost updates | L2: about 90 % (12 tasks). Ex. 2: about half (2 tasks). L4: 78 to 85 %. Different cases. |
| Ex. 2 task 1 times | Slide: 1687 and 831 ms (ratio 2,03, above the stated maximum 2). Code comment: 1056 and 833 ms. Unexplained. |
| Random seeding in .NET | L2 notes: current .NET does not give equal sequences from `new Random()` in one millisecond. Old .NET Framework did. Ex. 2 trap D code comment says it does. The exercise targets .NET 10, so the comment is probably outdated (inferred). The ex. 2 notes say error 2 is print order. |
| Six or seven traps | L6 says seven traps, and its table has seven. `ex-06-prefix-sum.md` S1 note says six. L2 says four traps. Ex. 2 has four (A to D). L4, L5 table six rows. L1 S45 lists six problems. Counts differ by deck. |
| Missing week-3 exercise | L1 S11 and S12 put "Parallel.For and speedup factor" in week 3. It is not in the repo. L3 calls it "practice 3". Folder `03-data-race` is week 4. Demo pages (`amdahl.html`, `zakoni.html`, `valna.html`, `parnepar.html`) and exercise 7 are not in the repo. |
| String comparison exercise | L6 S30 and S33 say it is in exercise 6. No such project exists. |
| Fibonacci and prefix exercise | No task matches L3 S24. |
| Speedup of prefix sum | L6 S28: best S about 1,7 (n = 2²⁴). Exercise comment: S up to 4,23. Cause unknown. |
| Block formulas | L6 S26: first n % p processors get one more element. Exercise code: `b·n/p`. Both differ by at most 1. |
| Async timings | Visualization: 1915, 1686, 513 ms; 7006 ms. Code comments: 1030, 1017, 348 ms; 7528 ms. Not explained. |
| Coffman class of "return and try again" | Slides: removes no preemption. Also removes hold and wait (inferred). |
| Orders against chance | `ex-03-data-race.md` counts orders (924, 6, 184,756). They are not probabilities. |
| Ex. 5 stop time | Notes say 300 ms "after the stop". It can mean 300 ms after the start of the wait. Open. |

## 9. Glossary {#sec-9}

| English | Croatian | Source |
| --- | --- | --- |
| parallelism | paralelizam | `01-...` |
| concurrency | konkurentnost | `01-...` |
| data race | utrka za podatke | `04-...` |
| race condition | utrka | `04-...` |
| speedup | ubrzanje | `03-...` |
| efficiency | učinkovitost | `03-...` |
| overhead | tara | `02-...` |
| granularity | granularnost | `02-...` |
| Flynn taxonomy | Flynnova taksonomija | `02-...` |
| Amdahl law | Amdahlov zakon | `03-...` |
| Karp-Flatt metric | Karp-Flattova metrika | `03-...` |
| Bernstein conditions | Bernsteinovi uvjeti | `03-...` |
| thread pool | fond niti | `01-...` |
| scheduler | raspoređivač | `01-...` |
| context switch | promjena konteksta | `03-...` |
| delegate | delegat | `ex-01-...` |
| closure | zatvaranje nad varijablom | `ex-01-...` |
| isolation | izolacija | `04-...` |
| immutability | nepromjenjivost | `04-...` |
| critical section | kritični odsječak | `04-...` |
| mutual exclusion | međusobno isključivanje | `04-...` |
| atomic operation | atomarna operacija | `04-...` |
| deadlock | zastoj, potpuni zastoj | `07-...` |
| livelock | živo blokiranje | `07-...` |
| starvation | izgladnjivanje | `07-...` |
| semaphore | semafor | `07-...` |
| concurrent collections | konkurentne kolekcije | `07-...` |
| barrier | ograda, barijera | `05-...` |
| lock step | (not translated) | `05-...` |
| PRAM, aPRAM | PRAM, asinkroni PRAM | `05-...` |
| Brent rule | Brentovo pravilo | `06-...` |
| prefix sum (scan) | suma prefiksa | `06-...` |
| prescan | isključivi scan | `06-...` |
| reduction | reduciranje | `06-...` |
| neutral element | neutralni element | `06-...` |
| associativity | asocijativnost | `06-...` |
| cancellation | prekid | `ex-05-...` |
| task, communication, synchronization | zadatak, komunikacija, sinkronizacija | `02-...` |
| wavefront | valna fronta | `03-...` |
| domain decomposition | dekompozicija domene | `03-...` |
| strong, weak scalability | jaka, slaba skalabilnost | `03-...` |
| Gustafson law | Gustafsonov zakon | `03-...` |
| PCAM | (not translated) | `03-...` |
| Blelloch algorithm | Blellochov algoritam | `ex-06-...` |
| work-efficient | (not translated) | `06-...` |
