# Amdahl's Law and Programming Models (source: 03-amdahlov-zakon-i-modeli-programiranja.md, 35 slides)

## Map
- This lecture answers one question: why does PiDemo give speedup 6.3 on 12 logical cores, and not 12?
- Part 1 (S1-S14) gives the limit of speedup (Amdahl), the reason for a lower measured value, and two more tools (Karp-Flatt, Gustafson).
- Part 2 (S15-S19) tells when two program parts can run at the same time (Bernstein), which programming models exist, and what a thread costs.
- Part 3 (S20-S34) shows how to design a parallel program in four steps (PCAM). The examples are wavefront and odd-even sort. The examples have pitfalls that are like the pitfalls of part 1 (inferred). The two examples have the same pitfalls as each other (S31).
- The main idea: parallel is not automatically faster. It is faster when the program avoids the pitfalls in the summary table (S34).

## Speedup and efficiency (ubrzanje i učinkovitost)   [S1-S3]
**What:**
- Speedup S(N) = T(1) / T(N). It tells how many times the parallel program is faster.
- Efficiency (učinkovitost) E(N) = S(N) / N. It tells which part of the cores does useful work.
- Ideal case: S = N and E = 1.

**Why:** Lecture 2 measured the first real speedup and it was lower than expected. These two measures show how far the program is from the ideal. The lecture has three goals. Goal 1, measure: how much speedup is possible, and why the measured value is lower. Goal 2, check: when two parts can run at the same time, and which models exist. Goal 3, design: a parallel program in four steps.

**Conclusion:** Up to 4 tasks the program is almost ideal. From 6 tasks E falls, and the curve leaves the ideal line. Amdahl, Karp-Flatt and the other topics explain why.

**Example:** PiDemo (Monte Carlo pi from lecture 2; i7-9750H, Release). Sequential T(1) = 936 ms. One task takes 927 ms, so S = 1.01 for N = 1 (measurement noise).

| N | T(N) | S | E |
| --- | --- | --- | --- |
| 1 | 927 ms | 1.01 | 1.01 |
| 2 | 469 ms | 2.00 | 1.00 |
| 4 | 236 ms | 3.96 | 0.99 |
| 6 | 175 ms | 5.35 | 0.89 |
| 8 | 164 ms | 5.71 | 0.71 |
| 12 | 148 ms | 6.31 | 0.53 |
| 16 | 149 ms | 6.28 | 0.39 |

The slide also has a chart with two series: measured S and ideal S = N. The teacher shows the table on the right side and compares it with the chart.

**Link:** The slide asks "Why does E fall from 6 tasks?". Answer: Karp-Flatt (topic "Karp-Flatt metric"). The numbers come from lecture 2. S1 names the lecturer (Marin Aglić Čuvić, after Tonći Dadić) and the year 2026/2027. S2 recalls lecture 2: Flynn taxonomy, granularity and overhead (tara). The teacher says the answer to the question comes in about ten minutes (it comes on S9). Exercise: `exercises/02-parallel/Zadatak2_Task` measures S and S/k.

## Amdahl's law (Amdahlov zakon)   [S4-S6]
**What:**
- The sequential part limits the speedup. If the sequential part is 1 − P of the time, the speedup is never more than 1 / (1 − P). This is true for any number of cores [1] (Amdahl, 1967).
- T(N) = T(1) · ((1 − P) + P / N)
- S(N) = 1 / ((1 − P) + P / N)
- P is the part of the run time that can be divided. P is not the part of the statements.
- When N grows, P / N goes to zero. Then S(N) goes to 1 / (1 − P).

**Why:** The program has two parts. The sequential part (slide colour: orange) must run on one core. The parallel part (blue) can be divided. On 4 cores the parallel part takes P / 4 on each core. The sequential part takes the same time as before. The saving exists only in the parallel part. The lecture 1 deck already showed the formula. Here the lecture derives it.

**Conclusion:** If the sequential part is large, more cores do not help. The program must make the sequential part smaller.

**Example 1: five statements.** The notes say this example comes from last year's lecture. Reference [10] is the lecture of T. Dadić, 2025/26. The link between the example and [10] is inferred. The program:
```
S1: x, y, z := ulaz();
S2: a := p(x);
S3: b := q(y);
S4: c := r(z);
S5: ispiši(a, b, c);
```
- S2, S3 and S4 depend only on S1. S5 needs all three.
- Assumption: each statement takes the same time.
- Sequential: S1 and S5, so 2 / 5 = 0.4. Parallel: S2, S3, S4, so 3 / 5 = 0.6.
- S(3) = 1 / (0.4 + 0.6 / 3) = 1 / 0.6 ≈ 1.67.
- The teacher asks the students first: what is the speedup on 2 cores? (The slide gives no answer for N = 2 in this topic. The next topic shows 1.43 from Amdahl and 1.25 in reality.)

**Example 2: same ratio, more cores.** A large program: 40 % of the time is sequential and 60 % can be divided into any number of parts.

| N | S(N) |
| --- | --- |
| 1 | 1.00 |
| 2 | 1.43 |
| 3 | 1.67 |
| 4 | 1.82 |
| 10 | 2.17 |
| 100 | 2.46 |
| ∞ | 2.50 |

100 cores give 2.46. The limit is 1 / 0.4 = 2.5. 100 cores are barely better than 10 cores.

**Pitfall:** Do not read P as the part of the statements. P is the part of the time.

**Link:** The assumption "the parallel part divides into any number of equal parts" fails in the next topic. The notation for P and 1 − P is in the topic "Notation". Teacher demo: `Tjedan 01 - Uvod/Vizualizacije/amdahl.html`, section "Primjer: pet naredbi" (this file is not in this repository; open point 2). Exercise 4 of the lecture 3 practice (S33) implements the five-statement example.

## Indivisible tasks (nedjeljivi zadaci)   [S7]
**What:** Amdahl assumes that the parallel part divides into any number of equal parts. The five-statement program has only three independent tasks (S2, S3, S4). Real speedup stops at 1.67.

**Why:** On 2 cores one core must run two of the three tasks. The time is then 4 units, and the speedup is 5 / 4 = 1.25. With 3 cores each task has its own core, and the time is 3 units. A fourth core has no task to run (inferred).

**Conclusion:** When the assumption is false, the real speedup is lower than Amdahl. The fix is to divide the work into more, smaller tasks. The tasks must not be too small, because then the overhead (tara) grows (lecture 2).

**Example:** Sequential time T = 5 (each statement is 1 unit).

| N | Amdahl | real |
| --- | --- | --- |
| 2 | 1.43 | 5 / 4 = 1.25 |
| 3 | 1.67 | 5 / 3 = 1.67 |
| 4 | 1.82 | 1.67 |
| 100 | 2.46 | 1.67 |

- N = 2: core 1 runs S1, S2, S4, S5 (T = 4). Core 2 runs S3.
- N = 3 and larger: core 1 runs S1, S2, S5 (T = 3). Core 2 runs S3. Core 3 runs S4.

**Link:** Same row in the summary table (S34). Overhead: lecture 2 (granularity, tara).

## Why the measured speedup is lower than Amdahl   [S8]
**What:** Amdahl is an upper limit. The measured time is closer to T(N) ≈ T(1) · ((1 − P) + P / N) + tara(N). Six causes exist:

| Cause | Meaning |
| --- | --- |
| Sequential part | Merging results, input and output. Only this cause is in Amdahl. |
| Overhead (tara) | Creating and scheduling tasks, waiting, merging. It grows with the number of tasks. |
| Bad division (loša podjela) | All tasks wait for the slowest task (lecture 2, slide 13). |
| Indivisible tasks | Fewer tasks than cores (previous topic). |
| Hardware | 12 logical cores share 6 physical cores. Two threads on one core are not twice as fast. |
| Memory | The cores share the cache (priručna memorija) and the path to main memory. |

**Why:** The formula has one cause only. A real program can have several of the six causes. The slide ends with the question: how can we see the cause from the measurement? The next topic answers it.

**Conclusion:** Amdahl gives the best case. Overhead, bad division, indivisible tasks, hardware and memory make the real result lower.

**Link:** Karp-Flatt separates the sequential part from overhead and hardware. Row "Tara, previše niti" in S34.

## Karp-Flatt metric (Karp–Flattova metrika)   [S9-S10]
**What:**
- The metric estimates the sequential fraction e from the measured speedup S on N cores. It shows whether the sequential part or the overhead limits the program [3] (Karp and Flatt, 1990).
- e = (1/S − 1/N) / (1 − 1/N)
- e is the experimentally determined sequential fraction. It is Amdahl's law solved for (1 − P) and used with the measured S.

**Why:** If e is the same for each N, a real sequential part is the cause. If e grows with N, something that grows with the number of tasks is the cause: overhead or hardware. The method needs only the measured times, not the code.

**Conclusion:** For PiDemo, e is about 0 up to 4 tasks and grows from 6 tasks. The computer has 6 physical cores. The answer to the lecture question: 6.3 and not 12, because of hardware and overhead, not because of the sequential part. The students compute e for their own program in the practice.

**Example 1: PiDemo.**

| N | S (PiDemo) | e |
| --- | --- | --- |
| 2 | 2.00 | 0.000 |
| 4 | 3.96 | 0.003 |
| 6 | 5.35 | 0.024 |
| 8 | 5.71 | 0.057 |
| 12 | 6.31 | 0.082 |
| 16 | 6.28 | 0.103 |

Live calculation for N = 8 (teacher): 1/5.71 = 0.175; minus 1/8 gives 0.050; divided by 0.875 gives 0.057.

**Example 2: one calculation step by step.** f = 0.5, N = 4, measured S = 1.53:
e = (1/1.53 − 1/4) / (1 − 1/4) = (0.654 − 0.25) / 0.75 ≈ 0.54.
(The notes give the intermediate value 0.404 for the numerator.)

**Example 3: two programs of practice 3 on the same computer.** The only change is f, the sequential fraction that the student builds into the program. The slide also has a chart with two series (f = 0 and f = 0.5). The chart shows the same e values as the tables, as a percentage (for example −5.4, 0.4, 9.3, 14.8 for f = 0).

f = 0 (no sequential part):

| N | S | e |
| --- | --- | --- |
| 2 | 2.11 | −0.054 |
| 4 | 3.95 | 0.004 |
| 8 | 4.85 | 0.093 |
| 16 | 4.97 | 0.148 |

f = 0.5 (half sequential):

| N | S | e |
| --- | --- | --- |
| 2 | 1.28 | 0.560 |
| 4 | 1.53 | 0.540 |
| 8 | 1.61 | 0.567 |
| 16 | 1.61 | 0.596 |

- f = 0: e grows with N. Hardware and overhead limit the program.
- f = 0.5: e stays near 0.55. The sequential part limits the program (the built-in 50 %).
- The teacher says e grows "from zero to 0.15 at 16 tasks" for f = 0.

**Pitfall:** For a small N, e is sensitive to noise. The source shows e = −0.054 at N = 2 for f = 0, where S = 2.11 is above 2. The link between this value and noise is inferred. Karp-Flatt does not predict. It does not say which overhead exists (slide 13).

**Link:** Notation f and e: topic "Notation". Practice 3, task 1 (S33). Demo: `Vizualizacije/zakoni.html` (teacher mentions section 1, preset PiDemo and a calculator; the file is not in this repository, open point 2). In practice 3 the number of tasks is k.

## Gustafson's law (Gustafsonov zakon)   [S11-S12]
**What:**
- If the work grows with the number of cores and the time stays the same, the speedup grows almost as N: S(N) = N − s · (N − 1) [2] (Gustafson, 1988).
- s is the sequential fraction measured on N cores. Amdahl's 1 − P is measured on one core.

**Why:** Amdahl is pessimistic: with 10 % sequential part the speedup is never more than 10. Gustafson asks another question. We do not want to solve the same problem faster. We want to solve a larger problem in the same time. Derivation:
1. Measure on N cores. Set the time there to 1.
2. The sequential part is s. Each of the N cores runs the part 1 − s.
3. The same, larger work on one core takes s + N · (1 − s). The sequential part is the same. One core must do the parallel part N times.
4. S = (s + N · (1 − s)) / 1 = N − s · (N − 1).

**Conclusion:** Amdahl and Gustafson are the same law with a different starting point. Amdahl fixes the work. Gustafson fixes the time on N cores. They do not conflict because they answer different questions: Amdahl is strong scalability (jaka skalabilnost): how much faster do we solve the same problem. Gustafson is weak scalability (slaba skalabilnost): how much larger a problem do we solve in the same time.

**Example:**
- s = 0.1 and N = 10: S = 9.1. Amdahl with P = 0.9 and N = 10: S ≈ 5.3.
- s = 0.1 and N = 4: S = 0.1 + 4 · 0.9 = 3.7.
- Check with Amdahl: on one core the sequential part is 0.1 / 3.7 ≈ 0.027. Then 1 / (0.027 + 0.973 / 4) = 3.7.
- N = 64: Amdahl 8.77, Gustafson 57.7. Amdahl levels off below 10. Gustafson grows almost on a straight line.
- Chart data (from the .pptx; the N values 1, 2, 4, 8, 16, 32, 64 are inferred):

| N | 1 | 2 | 4 | 8 | 16 | 32 | 64 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Amdahl, P = 0.9 | 1.0 | 1.82 | 3.08 | 4.71 | 6.4 | 7.8 | 8.77 |
| Gustafson, s = 0.1 | 1.0 | 1.9 | 3.7 | 7.3 | 14.5 | 28.9 | 57.7 |
- Analogy: a climate model with more cores does not compute faster. It computes a finer grid of cells in the same time (lecture 1).
- Slide pictures: Amdahl "same problem, shorter time" (N = 1 and N = 4: shorter). Gustafson "same time, larger problem" (N = 4: 4 times more work).

**Pitfall:** Do not compare s = 0.1 in Gustafson with a sequential part of 0.1 in Amdahl. They are measured in different places.

**Link:** Demo: `Vizualizacije/zakoni.html`, section 2 (s = 0.1, N up to 64). The file is not in this repository (open point 2). Comparison table: next topic.

## Which law to use   [S13]
**What:** Three laws answer three questions.

| | Amdahl | Karp-Flatt | Gustafson |
| --- | --- | --- | --- |
| Question | What is the largest speedup for the same work? | What limits my measured program? | How much larger a work do I solve in the same time? |
| Needs | the part P (an estimate, before measuring) | measured times for several N | s measured on the parallel computer |
| Strength | a simple limit; usable before writing code | separates the sequential part (e is the same) from overhead and hardware (e grows) [3] | realistic when the work grows with the computer, for example simulations [2] |
| Weakness | P is hard to know in advance; it does not show overhead or hardware | only after measuring; it does not predict; noise for a small N; it does not say which overhead | not valid for fixed work (one query); the sequential part must not grow |

**Why:** The notes add detail. Amdahl also does not show indivisible tasks. Karp-Flatt only says that overhead exists. Gustafson is not valid for fixed work such as one query or one image.

**Conclusion:** Order in practice: Amdahl before writing code (is it worth it?), Karp-Flatt after measuring (what to fix?), Gustafson when the work can grow.

## Notation   [S14]
**What:**

| Symbol | Meaning | Where |
| --- | --- | --- |
| N | number of cores (or tasks) | all laws |
| k | number of tasks; the same as N | PiDemo, practice 2 and 3 (there N can be the number of points or numbers) |
| T(1), T(N) | time on one core and on N cores | measurements |
| S | speedup T(1) / T(N); marked U in lecture 1 | |
| E | efficiency S / N | slide 3 |
| P | part of the time that can be divided, measured on one core | Amdahl |
| 1 − P | sequential fraction, measured on one core | Amdahl |
| f | sequential fraction that we build into the program | practice 3, task 1 |
| e | sequential fraction computed from the measured S | Karp-Flatt |
| s | sequential fraction measured on N cores | Gustafson |

**Why:** There are four symbols for the sequential fraction. They differ in how each is obtained: 1 − P is assumed (measured on one core). f is built in by the student. e is computed from the measured S. s is measured on N cores. This is why Gustafson s = 0.1 is not the same as the Amdahl sequential part 0.1.

**Conclusion:** If e ≈ f, the sequential part explains everything. If e grows with N, the cause is overhead or hardware.

**Link:** Demo: `Vizualizacije/zakoni.html`, section 1, preset of practice 3 (compare e and f). Not in this repository (open point 2).

## Bernstein's conditions (Bernsteinovi uvjeti)   [S15-S16]
**What:**
- I = the variables that a program part reads. O = the variables that it writes.
- Parts S1 and S2 can run at the same time if all three conditions are true [4] (Bernstein, 1966):
  - I₁ ∩ O₂ = ∅
  - O₁ ∩ I₂ = ∅
  - O₁ ∩ O₂ = ∅
- Shared reading is allowed.

**Why:** If a condition is broken, the result depends on the order of execution (who was first).

**Conclusion:** Parts may run together only when no part writes what the other part reads or writes.

**Example 1: four access cases.** In each case both parts use the same variable x. The slide draws x in all four cases and labels them "smije" (allowed) or "ne smije" (not allowed). Only the first case is allowed.

| S1 | S2 | allowed? |
| --- | --- | --- |
| read | read | yes |
| read | write | no |
| write | read | no |
| write | write | no |

**Example 2: check the conditions.**
```
S1: a = x + y;
S2: b = x * 2;
S3: x = 5;
S4: c = a + b;
```

| S | reads (I) | writes (O) |
| --- | --- | --- |
| S1 | x, y | a |
| S2 | x | b |
| S3 | – | x |
| S4 | a, b | c |

| pair | at the same time? | why |
| --- | --- | --- |
| S1, S2 | yes | both only read x |
| S1, S3 | no | S1 reads x, S3 writes x |
| S2, S3 | no | S2 reads x, S3 writes x |
| S1, S4 | no | S4 reads a |
| S2, S4 | no | S4 reads b |
| S3, S4 | yes | no shared variables |

The teacher lets the students fill the table first (two minutes) and then shows the right side.

**Pitfall:** If S1 and S3 start together, a is the sum of the old x or of the new x. The output changes from run to run (non-determinism, nedeterminizam). When the condition is broken in a loop, for example `zbroj += a[i]`, the result is a data race. Protection comes in lecture 4.

**Link:** Odd-even sort uses the conditions (topic "Odd-even sort"). Demo: `Vizualizacije/parnepar.html`, section 2: pairs that share an element break O₁ ∩ O₂ = ∅ (not in this repository, open point 2). Exercise: `exercises/02-parallel/Zadatak3_Zamke`, trap C. The summary table (S34) names the fix: isolation, lock (lecture 4).

## Programming models (modeli paralelnog programiranja)   [S17]
**What:** A programming model is how the programmer sees the parallel program, independent of the hardware. The course has three models.

| Model | How data is shared | In the course | Typical pitfall |
| --- | --- | --- | --- |
| Shared memory and threads | All threads see the same variables; they communicate by reading and writing. | C#: Task, Parallel.For (weeks 1-7) | data race, deadlock (zastoj) |
| Message passing (razmjena poruka) | Each process has its own memory; data goes in messages (send, receive). | MPI, Python mpi4py (weeks 9-12) | deadlock, message order |
| Data parallelism (paralelizam podataka) | The same operation on parts (partitions) of the data. | Parallel.For, PLINQ, GPU (week 13) | bad division, too small granularity |

**Why:** The model is not the hardware.

**Conclusion:** In practices 9-12 the students send messages (mpi4py) on one laptop with shared memory [5].

## Thread states and the context switch (nit i promjena konteksta)   [S18-S19]
**What:**
- A thread has states: New (Nova), Ready (Spremna), Running (Izvodi se), Waiting (Čeka), Finished (Završena).
- Transitions: a new thread becomes ready. The scheduler (raspoređivač) of the operating system gives it a core, and it runs. When its time ends (istek vremena), it returns to ready. When it waits (for `task.Result`, disk, network), it goes to waiting. When the data arrives (podatak stigao), it is ready again. At the end it finishes.
- The operating system changes the states, not the program.
- A waiting thread does not use a core.
- A context switch (promjena konteksta) happens each time a core goes from one thread to another. The system saves the program counter (programsko brojilo, address of the next instruction), the processor registers and the stack pointer. It loads them for the other thread.

**Why:** A context switch has two costs [6] (Li, Ding, Shen, 2007; Linux 2.6.17, 2 × Intel Xeon 2.0 GHz):
- Direct: 3.8 µs per context switch.
- Indirect: the new thread finds a cache full of data of other threads. The cache fills again. This takes from a few µs to more than 1000 µs, depending on the data.
- On another computer the numbers differ. The order of magnitude is a microsecond.

**Conclusion:** Keep the number of computing threads about equal to the number of cores. The thread pool (Task.Run, Parallel.For) does this itself. Do not create your own threads (`new Thread`) in a loop.

**Example:** PiDemo (lecture 2): 16 tasks are as fast as 12 (149 and 148 ms), but not faster. A task is not a thread. The tasks wait in the queue of the thread pool (fond niti). The pool keeps about as many threads as there are cores.

**Pitfall:** More active threads than cores. The threads take turns on the same core. The program pays for context switches, and data leaves the cache.

**Link:** Row "Tara, previše niti" in S34. The cache cause appears in "Why the measured speedup is lower".

## Designing a parallel program: PCAM   [S20-S21]
**What:** Ian Foster (1995) proposed four steps [7]. The English name is PCAM.

| Step | Question or action |
| --- | --- |
| Partitioning (particioniranje) | Divide the work into as many small tasks as possible. |
| Communication (komunikacija) | Which tasks need data of other tasks? |
| Agglomeration (aglomeracija) | Join small tasks into larger tasks (granularity). |
| Mapping (mapiranje) | Assign the tasks to cores. |

**Why:** The first two steps look for parallelism and do not depend on the computer. The last two adapt the program to the real computer. Lecture 1 asked three questions: what at the same time, how to divide, is it worth it. This is the same procedure, step by step.

**Conclusion:** Each course pitfall belongs to one step.

**Example:** Sum of 10⁹ numbers (the same program as task 2 of practice 2; now the lecture names the steps the students already did).

| Step | Decision | Pitfall if wrong |
| --- | --- | --- |
| Partitioning | Each addition a[i] is a task (10⁹ small tasks). | `zbroj += a[i]` in each iteration: data race |
| Communication | The partial sums must be merged into one result. | merging is a sequential part (Amdahl) |
| Agglomeration | k blocks of N / k numbers, each with its own local sum. | too small blocks: overhead bigger than the work |
| Mapping | One block per core; Task.Run or Parallel.For and the thread pool. | the remainder N / k is lost; too many threads |

**Link:** Exercise: `exercises/02-parallel/Zadatak2_Task`. Agglomeration is the granularity of lecture 2. Odd-even sort fix (block per core) is agglomeration.

## Domain decomposition (dekompozicija domene)   [S22]
**What:**
- Partitioning has two forms. The first is domain decomposition: each core gets its own part of the data.
- Example: an 8 × 8 array on 4 cores, by rows or by blocks (four colours are four cores).
- If a cell needs its neighbours (for example the cell grid of a climate model), the border cells must be exchanged.
- Fewer border cells mean less communication.

**Example:** Layout in the slide picture (read from the .pptx): "by rows" is 4 horizontal strips of 2 full rows, one colour per core. "By blocks" is four 4 × 4 quadrants, one colour per core. Inner strip border: 2 · 8 = 16 cells. Block border: 2 · 4 = 8 cells. The block has fewer border cells (inferred from the two numbers). Fewer border cells mean less communication (S22).

**Why:** Border cells are the communication of the PCAM step 2 (inferred).

**Conclusion:** Choose the division with the smaller border. The teacher asks: which division is better for 100 cores? The source gives no answer (open point 3).

## Functional decomposition (funkcionalna dekompozicija)   [S23]
**What:** Each task runs a different function of the whole work, not the same work on other data.

**Example 1:** A climate model. The components atmosphere, land, ocean and ice are tasks. They exchange data in each simulation step [5].

**Example 2:** A pipeline (protočna obrada): load, filter, compute, write. Each step is its own task.

**Pitfall:** The slowest component sets the speed of the whole system (bad division).
**Fix:** Give more cores to the slower component, or divide it more by data.

**Link:** Domain decomposition (previous topic); bad division in S8.

## Dependency chain (lanac ovisnosti)   [S24]
**What:** In the Fibonacci sequence F(n) = F(n − 1) + F(n − 2), each step needs the result of the previous two. A direct loop is a chain, and no step can start before the previous one. The same is true for each loop in which an iteration reads the result of the previous one.

**Why:** A dependency does not mean "impossible". A linear recurrence can be written as a prefix sum (suma prefiksa) with an associative operation. For Fibonacci the operation is the multiplication of 2 × 2 matrices. Then the computation can run in parallel [8] (Blelloch, 1990). The prefix sum algorithm is in week 6.

**Conclusion:** We do not parallelize the loop. We change the algorithm. The teacher says that the slide of last year ("it cannot be parallelized") was too strict.

**Link:** Lecture 6 (prefix sum); no matching exercise task found (open point 4). Row "Lanac ovisnosti" in S34.

## Levenshtein distance and the wavefront (valna fronta)   [S25-S26]
**What (distance):** The Levenshtein distance of two words is the smallest number of one-character edits that change the first word into the second. An edit is insert, delete or replace a character [11]. It is used for spell checking [12] and for text search with typing errors.

**What (table):** D[i][j] is the distance of the first i characters of the first word and the first j characters of the second word. The result is in the bottom right corner [12].
- Border: 0, 1, 2, ... (from an empty word, j insertions give a word of length j).
- Same characters (A and A): copy the number from the upper left cell.
- Different characters: 1 + the smallest of the left, upper and upper left cell. Left means insert, upper means delete, upper left means replace.
- D[i][j] = min(D[i−1][j] + 1, D[i][j−1] + 1, D[i−1][j−1] + c). c = 0 if the characters are equal, c = 1 otherwise (replace).

**Example 1: words.**
- MAČKA → MAZGA: Č→Z, K→G = 2.
- KUĆA → KUĆICA: insert I, C = 2.
- PARALELNO → PARABOLA = 4.

**Example 2: table for MAČKA and MAZGA.**

| | | M | A | Z | G | A |
| --- | --- | --- | --- | --- | --- | --- |
| | 0 | 1 | 2 | 3 | 4 | 5 |
| M | 1 | 0 | 1 | 2 | 3 | 4 |
| A | 2 | 1 | 0 | 1 | 2 | 3 |
| Č | 3 | 2 | 1 | 1 | 2 | 3 |
| K | 4 | 3 | 2 | 2 | 2 | 3 |
| A | 5 | 4 | 3 | 3 | 3 | 2 |

The teacher shows one cell: D[3][3] (MAČ against MAZ) = 1, because the diagonal is 0 (MA against MA) plus the replace Č→Z.

**Why (wavefront):** Each cell depends on the left, upper and upper left cell. Cells on the same antidiagonal (the same i + j) do not depend on each other, so they compute at the same time. The last year slide said that Levenshtein cannot be parallelized. That was too strict.

**Example 3: step numbers.** The number in a cell is the step in which it is computed. In a 6 × 6 table the steps are 1 to 11, so 11 steps instead of 36. Cells per step: 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1.

**Pitfall:** At the start and the end there is little work (one or two cells). After each step all cores wait.

**Link:** Demo: `Vizualizacije/valna.html`: step by step, p = 4; then section 2 (without waiting). Not in this repository (open point 2). Comparison with odd-even sort: topic "Wavefront and odd-even: the same pattern".

## Odd-even sort (par-nepar sortiranje)   [S27-S30]
**What:**
- It is like bubble sort, but in one phase all pairs are compared at the same time. A pair in the wrong order is swapped.
- Odd phase (neparna faza): pairs (1, 2), (3, 4). Even phase (parna faza): pairs (0, 1), (2, 3), (4, 5). The phases differ only in where the pairs start.
- For n elements, n phases are enough [9].
- Example array in the slide picture: 3 1 4 2 6 5.

**Why (alternation):** In one phase an element moves at most one place and cannot cross the border of its pair. If only the even phase repeated, nothing would change after the first phase. The next phase moves the borders, so the elements travel through the whole array.

**Why (parallel):** Pairs of the same phase share no element. The Bernstein conditions are true, so the cores run at the same time. The teacher asks: what happens if two pairs share an element? (Demo: `Vizualizacije/parnepar.html`, section 2: pairs must not share an element.)

**Example 1: worst case (S28).** A reversed array of 7 elements needs 7 phases. The first row holds the indices. The source highlights the swapped elements. Highlighted indices (read from the .pptx): phases 1, 3, 5, 7: indices 1-6 (the element at index 0 has no partner). Phases 2, 4, 6: indices 0-5 (the element at index 6 has no partner).

| phase | 0 | 1 | 2 | 3 | 4 | 5 | 6 | pairs |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| start | 6 | 5 | 4 | 3 | 2 | 1 | 0 | |
| 1 | 6 | 4 | 5 | 2 | 3 | 0 | 1 | odd |
| 2 | 4 | 6 | 2 | 5 | 0 | 3 | 1 | even |
| 3 | 4 | 2 | 6 | 0 | 5 | 1 | 3 | odd |
| 4 | 2 | 4 | 0 | 6 | 1 | 5 | 3 | even |
| 5 | 2 | 0 | 4 | 1 | 6 | 3 | 5 | odd |
| 6 | 0 | 2 | 1 | 4 | 3 | 6 | 5 | even |
| 7 | 0 | 1 | 2 | 3 | 4 | 5 | 6 | odd |

Phase 1: the 6 stays, the others swap. Follow the 6: from phase 2 it moves one place right in each phase. After 7 phases the array is sorted. The teacher shows the first three phases slowly and the others fast (demo `parnepar.html`, section 1, n = 7, reversed).

**Example 2: comparison with bubble sort (S29).** Array 4 3 2 1 (the table starts with 3 4 2 1 after step 1).

Bubble sort, 1 core, one comparison per step:

| step | compare | array |
| --- | --- | --- |
| 1 | (0, 1) | 3 4 2 1 |
| 2 | (1, 2) | 3 2 4 1 |
| 3 | (2, 3) | 3 2 1 4 |
| 4 | (0, 1) | 2 3 1 4 |
| 5 | (1, 2) | 2 1 3 4 |
| 6 | (0, 1) | 1 2 3 4 |

Odd-even, 2 cores, one whole phase per step:

| step | phase | pairs at the same time | array |
| --- | --- | --- | --- |
| 1 | odd | (1, 2) | 4 2 3 1 |
| 2 | even | (0, 1) (2, 3) | 2 4 1 3 |
| 3 | odd | (1, 2) | 2 1 4 3 |
| 4 | even | (0, 1) (2, 3) | 1 2 3 4 |

The comparisons are the same (6), but there are 4 steps instead of 6, because in the even phases two comparisons run at the same time. For the array of slide 28 (6 5 4 3 2 1 0): bubble sort 21 comparisons in 21 steps; odd-even 21 comparisons in 7 steps on 3 cores. Same work, fewer steps. (The notes name the example "4 3 2 1"; the demo uses n = 4, reversed, 3 2 1 0, with the same phases and swaps.)

**Pitfall (always n phases):** Odd-even always runs n phases, even when the array is sorted. The array 3 1 4 2 is sorted after phase 3, and phase 4 still runs.

**Example 3: cost (S30).**

| | time | total work |
| --- | --- | --- |
| sequential, for example merge sort | O(n log n) | O(n log n) |
| odd-even, n / 2 cores | n phases: O(n) | n phases · n / 2 comparisons: O(n²) |

**Pitfalls:**
1. More work: faster on many cores, but the total work is larger than for a good sequential algorithm.
2. Synchronization: after each phase all cores wait, so there are n waits.
3. Too small tasks: one comparison per task is too little work (overhead).

**Fix (p cores, p ≪ n):** Each core sorts its own block of n / p elements sequentially. Then p phases follow. In each phase neighbouring cores merge their blocks and split them into the smaller and the larger half [9]. This is the agglomeration step of PCAM.

**Variant:** Windows of 5 elements instead of pairs, with shifts 0 and 2. For n = 16, at most 8 phases instead of 16, with fewer and larger tasks. A script checked this for n ≤ 16. Windows of the same phase must also not share an element. The notes explain the idea. Each core sorts a window of five neighbouring elements. The windows alternate with shifts 0 and 2, so that elements cross borders.

**Link:** Bernstein conditions (topic above). Demo: `parnepar.html`, section 3 (block per core). Same pattern as wavefront (next topic).

## Wavefront and odd-even: the same pattern   [S31]
**What:** Both turn a sequential chain into steps. Inside a step the tasks are independent and run together. Between steps all cores wait.

| | Wavefront (Levenshtein) | Odd-even sort |
| --- | --- | --- |
| one task | one cell D[i][j] | one pair (i, i + 1) |
| at the same time | cells of one antidiagonal (same i + j) | pairs of one phase |
| why allowed | a cell reads only earlier antidiagonals | pairs share no element (Bernstein) |
| why wait | the next antidiagonal reads the results of this one | the next phase touches the same elements |
| number of steps | m + n − 1 (6 × 6 table: 11) | n phases (7 elements: 7) |
| work per step | grows and falls: 1, 2, ..., 6, ..., 2, 1 | constant: about n / 2 comparisons |
| total work | same as sequential: each cell once | more than sequential: n² / 2 against n log n |
| fix | block of cells per core | block of elements per core (S30) |

**Why:** The reasons for waiting differ. In the wavefront the wait is a real data dependency. In odd-even the phases must not overlap because they touch the same elements. The pitfalls are the same: too small tasks, and waiting after each step. The fix is the same: a block per core.

**Conclusion:** We do not parallelize the loop. We look for what is independent and group it in steps. Larger tasks mean less waiting (agglomeration).

**Link:** Demos: `valna.html` and `parnepar.html`, section 2 (not in this repository, open point 2).

## More examples of parallelization   [S32]
For each problem the question is: what is divided, and where is the pitfall?

| Problem | Division and pitfall |
| --- | --- |
| N-body problem (problem n tijela) | The force on each body depends on all other bodies. A core computes the forces for its bodies. After a step all cores need the new positions. |
| Floyd's algorithm | Shortest paths between all pairs. n steps. In each step the n × n matrix is divided among cores. After step k all cores need row k and column k [9]. |
| Game trees (stabla igre) | Branches are searched at the same time. Pitfall: branches have unequal sizes (bad division). |
| Divide and conquer (podijeli pa vladaj) | Example: merge sort, two recursive calls at the same time. Pitfall: too small tasks near the bottom of the recursion. Fix: run sequentially below a threshold. |

## Practice 3 preview   [S33]
The deck announces four tasks for practice 3 of its numbering. The numbering does not match the repository folders (open point 1).
1. Measured speedup and Amdahl: a program with an adjustable sequential part. Compare S with the Amdahl prediction and compute e (Karp-Flatt).
2. Bad division: count prime numbers in k equal ranges. The last task runs longest. Fix: another division.
3. Too small tasks: Parallel.For with a very small loop body calls a delegate for each element. Too small ranges are slower than sequential. Fix: ranges of the right size.
4. Five statements: the example of this lecture as a program (Parallel.Invoke). Measure the speedup against 1.67.
Without lock and Interlocked: these come in lecture 4 (the notes say "next week").

## Summary table: pitfalls and fixes   [S34]

| Pitfall | How to see it | Fix |
| --- | --- | --- |
| Sequential part | the speedup levels off; e stays the same | make it smaller; or a larger problem (Gustafson) |
| Indivisible tasks | more cores give nothing | more, smaller tasks |
| Overhead, too many threads | e grows with N; more tasks give nothing (PiDemo: 16 as 12) | agglomeration; threads ≈ cores |
| Broken Bernstein condition | the result changes from run to run | isolation; lock (lecture 4) |
| Bad division | cores wait for the slowest | smaller blocks, more even division |
| Dependency chain | the loop cannot be divided | change the algorithm (wavefront, prefix sum) |

The teacher says: parallel is not automatically faster. It is faster when we avoid these pitfalls. Next week: data race, lock and Interlocked.

## References   [S35]
Full citations, as on the slide:
1. [1] G. M. Amdahl: Validity of the single processor approach to achieving large scale computing capabilities. AFIPS Conference Proceedings 30, 1967., 483–485.
2. [2] J. L. Gustafson: Reevaluating Amdahl’s law. Communications of the ACM 31(5), 1988., 532–533.
3. [3] A. H. Karp, H. P. Flatt: Measuring parallel processor performance. Communications of the ACM 33(5), 1990., 539–543.
4. [4] A. J. Bernstein: Analysis of programs for parallel processing. IEEE Trans. Electronic Computers EC-15(5), 1966., 757–763.
5. [5] LLNL HPC: Introduction to Parallel Computing Tutorial — hpc.llnl.gov/documentation/tutorials
6. [6] C. Li, C. Ding, K. Shen: Quantifying the cost of context switch. ExpCS 2007 — cs.rochester.edu/~kshen/papers/expcs2007.pdf
7. [7] I. Foster: Designing and Building Parallel Programs. Addison-Wesley, 1995. — mcs.anl.gov/dbpp
8. [8] G. E. Blelloch: Prefix sums and their applications. Technical report CMU-CS-90-190, 1990.
9. [9] A. Grama, A. Gupta, G. Karypis, V. Kumar: Introduction to Parallel Computing, 2nd ed. Addison-Wesley, 2003.
10. [10] T. Dadić: Amdahlov zakon i paralelizacija, lecture, 2025/26.
11. [11] V. I. Levenshtein: Binary codes capable of correcting deletions, insertions, and reversals. Soviet Physics Doklady 10, 1966., 707–710.
12. [12] R. A. Wagner, M. J. Fischer: The string-to-string correction problem. Journal of the ACM 21(1), 1974., 168–173.

The notes of S35 also name the lecturer, Dr. Tonći Dadić, as a source. No slide text cites [10]. The notes of S5 say the five-statement example comes from last year's lecture. The link to [10] is inferred.

## Exercise cross-reference
The practice that matches this lecture (Amdahl, Karp-Flatt, prime counting, small loop bodies, five statements) is not in `exercises/`. Folder `03-data-race` is the data-race practice (lecture 4). The tasks below are the closest matches.

- `exercises/02-parallel/Zadatak2_Task/Zadatak2_Task.cs`
  - Concept: S and E for 2, 4, 8, 16 tasks; PCAM on the sum of 10⁹ numbers (S21 says this is the same program).
  - Key insight: the solution splits [1, N] into k blocks, one Task per block, and sums the results. `ToArray` starts all tasks before the wait. Parallel.For writes `d[j]` per block (no shared variable). Sample in `Vjezba2_upute.html` (lines 54-57, Task.Run): k=2 S 1.95; k=4 3.66, 0.91; k=8 5.57, 0.70; k=16 5.22, 0.33. The .cs comment gives only k=4: 70 ms, 3.66, 0.91.
  - If ignored: S below k from hardware and overhead, and the student blames the code (inferred). With Karp-Flatt, e grows with k (inferred: computed from the sample S). A shared `zbroj +=` is a data race.
- `exercises/02-parallel/Zadatak1_Paralelno/Zadatak1_Paralelno.cs`
  - Concept: two independent jobs f and g (Bernstein: no shared variables (inferred)); maximum speedup 2.
  - Key insight: the time is about max(tf, tg), so the longer job and the number of cores limit it (`Vjezba2_upute.html` sample: 1687 ms to 831 ms; .cs comment: 1056 ms to 833 ms). Amdahl with two parts of unequal size (inferred).
  - If ignored: the student expects speedup 2 when one job is longer.
- `exercises/02-parallel/Zadatak3_Zamke/Zadatak3_Zamke.cs`
  - Concept: trap C is Bernstein. `b[i] = a[i] * 2` is always correct because each iteration writes its own element. `zbroj += a[i]` breaks O₁ ∩ O₂ = ∅ (S15-S16).
  - Key insight: the fix uses `Interlocked.Add`. Trap B (counter) has the same cause. Lost updates are about half.
  - If ignored: the result changes from run to run (non-determinism).
- `exercises/03-data-race/Zadatak1_Brojac/Zadatak1_Brojac.cs`
  - Concept: the broken Bernstein condition in a loop (S16 and S34 refer to lecture 4).
  - Key insight: `brojac++` is three steps. A local counter with one `Interlocked.Add` per task is fastest (about 6 ms) because it has one synchronization per task (code comment). This is agglomeration (inferred).
  - If ignored: the result is below 12 000 000 and differs per run.
- Search result for the whole `exercises/` folder (Amdahl, Karp, Levenshtein, Fibonacci, par-nepar, Bernstein, PCAM, Foster): no task matches S24 (Fibonacci, prefix sum). No task matches the wavefront or odd-even topics.
- Four text files mention Amdahl:
  - `02-parallel/Vjezba2_upute.html` has a link to the week 1 `amdahl.html`.
  - `03-data-race/Vizualizacije/index.html` (line 45) has a footer link to "Tjedan 03 - Amdahlov zakon".
  - `03-data-race/Vizualizacije/zajednicko.js` has a link to the same folder.
  - `01-delegates/Vizualizacije/zajednicko.js` has a menu entry `amdahl.html`. That file is not in that folder.
- The intro deck `02-parallel/Vjezba2 - prezentacija 2026-27.pptx` also names `Tjedan 01 - Uvod/Vizualizacije/amdahl.html` (slide and notes).
- The demo files and the practice for this lecture are not in the repository.

## Open points
1. S33 and S10 say "practice 3" (Amdahl, Karp-Flatt, f, k). The repository folder `03-data-race` is a different practice. The practice for this lecture does not exist in `exercises/`.
2. Demo files named in the notes (`Tjedan 01 - Uvod/Vizualizacije/amdahl.html`, `Vizualizacije/zakoni.html`, `valna.html`, `parnepar.html`) are not in this repository. I did not see them.
3. S22: "Which division is better for 100 cores?" has no answer in the source. The S22 colour layout is in the Domain decomposition example.
4. The search of `exercises/` found no Fibonacci or prefix-sum task for S24. The link to lecture 6 comes from slide S24 only.
5. S5: the notes ask "what is the speedup on 2 cores?" before the answer; the answer appears on S7 (Amdahl 1.43, real 1.25).
6. S29 notes use array "4 3 2 1" but the slide table starts from 3 4 2 1 after step 1, and the demo note says "3 2 1 0". I kept both as written.
7. No slide text cites reference [10]. The link between [10] and the five-statement example (S5, "last year's lecture") is inferred.

## Coverage ledger

| slide | topic section where it is covered | items |
| --- | --- | --- |
| 1 | Map; Speedup and efficiency (lecture goals, lecturer, year) | 4 |
| 2 | Speedup and efficiency; Map | 6 |
| 3 | Speedup and efficiency | 8 |
| 4 | Amdahl's law | 6 |
| 5 | Amdahl's law (example 1, origin in last year's lecture) | 7 |
| 6 | Amdahl's law (example 2) | 3 |
| 7 | Indivisible tasks | 6 |
| 8 | Why the measured speedup is lower | 7 |
| 9 | Karp-Flatt metric (example 1) | 8 |
| 10 | Karp-Flatt metric (examples 2 and 3, chart in percent) | 8 |
| 11 | Gustafson's law | 8 |
| 12 | Gustafson's law (derivation, checks, chart data) | 8 |
| 13 | Which law to use | 5 |
| 14 | Notation | 11 |
| 15 | Bernstein's conditions | 5 |
| 16 | Bernstein's conditions (examples) | 4 |
| 17 | Programming models | 4 |
| 18 | Thread states and the context switch | 6 |
| 19 | Thread states and the context switch | 9 |
| 20 | Designing a parallel program: PCAM | 5 |
| 21 | Designing a parallel program: PCAM (example) | 5 |
| 22 | Domain decomposition (with picture layout) | 6 |
| 23 | Functional decomposition | 5 |
| 24 | Dependency chain | 5 |
| 25 | Levenshtein distance and the wavefront | 8 |
| 26 | Levenshtein distance and the wavefront | 6 |
| 27 | Odd-even sort | 7 |
| 28 | Odd-even sort (example 1, highlights) | 4 |
| 29 | Odd-even sort (example 2, pitfall) | 5 |
| 30 | Odd-even sort (cost, pitfalls, fix, variant) | 7 |
| 31 | Wavefront and odd-even: the same pattern | 8 |
| 32 | More examples of parallelization | 4 |
| 33 | Practice 3 preview | 4 |
| 34 | Summary table | 6 |
| 35 | References | 12 |
