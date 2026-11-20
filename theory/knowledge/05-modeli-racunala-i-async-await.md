# Computer models (RAM, PRAM, aPRAM) and async/await  (source: 05-modeli-racunala-i-async-await.md, 30 slides)

## Map
- Author of the deck: Marin Aglić Čuvić, based on the lectures of Tonći Dadić. Academic year 2026/2027. Lecture 5.
- The notes of S1 give the goal: ask how to rate a parallel algorithm without a real computer, and why an algorithm that is correct on paper can be wrong on a computer. The last part: async and await, and why this is not parallelism.
- This lecture has two parts. Part 1 (S1-S25) covers three computer models: RAM, PRAM and aPRAM. Part 2 (S26-S29) covers async/await.
- Main idea: a parallel program pays for computation, for shared memory and for waiting. A model must show how much parallelism a problem has, and what that parallelism costs.
- RAM counts steps for one processor. PRAM shows the parallelism of a problem and treats memory and synchronization as free. aPRAM adds the cost of global memory (d) and of the barrier (B).
- The models expose two traps: an algorithm that is correct only in lock step, and a barrier for too many processors. Each trap has a fix: a barrier between steps, and fewer processors with more work each.
- async/await gives concurrency (konkurentnost), not parallelism. It has two traps: an async method that only computes, and blocking inside async code.

## Why a computer model (zašto model računala)   [S2-S3]
**What:** A model (model računala) is a list of what the analysis counts and what each item costs: steps, memory, and later waiting. The lecture presents three models: RAM (one processor), PRAM (an ideal parallel computer) and aPRAM (a parallel computer that pays for memory and waiting).
**Why:**
- We want to rate the complexity of an algorithm without a specific processor, compiler or load.
- For sequential programs a model predicts well: the sum of an array is O(n) on a laptop and on a server.
- For parallel programs the answer depends on what the model ignores.
- A good model is simple enough to calculate with, and real enough that the prediction holds.
- The recap of lectures 2-4: tasks that are too small lose to the overhead (tara). A lock around each increment is 90 times slower than sequential counting. A parallel program pays for computation, for shared memory and for waiting.
**Conclusion:** PRAM answers "how much parallelism does the problem have". aPRAM answers "what does it cost to use that parallelism". The question of the day (S2) is exactly these two questions.
**Link:** Lectures 2, 3 and 4 (the overhead and the lock cost that motivate the models).

## RAM: the model of a sequential computer   [S4]
**What:**
- RAM means random access machine (model računala). It does not mean RAM memory.
- It has one processor with a fixed number of registers (R0, R1, ...).
- It has unlimited memory locations M[0], M[1], ...; the addresses are natural numbers.
- Each machine operation takes 1 unit of time. Each location is 1 unit of space.
**Why:** The cost of an instruction is the same for a register and for memory. The teacher's picture: one worker who does one step after another, and each step takes the same time.
**Conclusion:** The sum of an array takes n steps: time O(n), space O(n). This is the model in which you calculated complexity until now.
**Example:**
```
zbroj := 0
za i := 0 .. n − 1 činiti
    zbroj := zbroj + A[i]
```
Time O(n), space O(n). You computed the same sum on k tasks in exercise 2.
**Figure (S4):** one processor with registers R0 and R1; memory M[0], M[1], M[2], M[3], ...; labels "1 korak" (1 step), "stalno mnogo" (a fixed number), "neograničena memorija" (unlimited memory).
**Pitfall:** Do not confuse RAM (random access machine) with RAM memory.
**Link:** Lecture 2 (SISD, one core). The three RAM names are tied together in "How the models connect".

## The trap of the model: "each access costs 1"   [S5]
**What:** The same number of increments (12,000,000) means the same RAM cost. The measured times from lecture 4 differ a lot.
**Why:** RAM does not see two things. Shared memory is slower than local memory. Waiting for others (synchronization) costs time.
**Conclusion:** From 1 ms to 456 ms is a difference of 450 times with the same number of operations. Therefore we ask the model for two things. First: how much parallelism is possible (PRAM). Second: how much it costs (aPRAM).
**Example:**

| method | time |
| --- | --- |
| sequential, one task | 5 ms |
| 12 tasks, no protection (wrong) | 12 ms |
| Interlocked per increment | 203 ms |
| lock per increment | 456 ms |
| local, then one Interlocked.Add | 1 ms |

**Link:** Lecture 4 (the source of these measurements).

## PRAM: the parallel RAM   [S6-S7]
**What:**
- PRAM has p RAM processors and one shared memory [3].
- It is synchronous (sinkrono). A common clock exists. In each clock tick every processor executes one instruction. This is lock step.
- The instructions can differ between processors. This is MIMD, not SIMD [1]. The clock decides when, not what.
- Memory access takes 1, as in RAM.
- The number of processors p is often a given value.
- Lock step rule: nobody starts step k + 1 until all processors finish step k.
- Pseudocode keyword `paralelno` (parallel). With n processors, processor i works with V[i]. One step takes O(1). In RAM the same work takes O(n).
**Why:**
- PRAM is an ideal computer: no waiting and no expensive memory.
- The teacher's picture: rowers who obey one command. Each rower uses his own oar (his own instruction on his own data). All rowers pull on the same beat. Nobody makes the second stroke until all have made the first.
- Why PRAM exists: Gibbons says it is useful although it is idealized, because shared memory hides communication and synchronization [2]. What remains is the pure question of how much parallelism a problem has.
**Figure (S6):** processors P0, P1, P2, ..., Pp−1 above one shared-memory box, and the label "zajednički sat: takt 1, 2, 3 ..." (common clock: tick 1, 2, 3 ...).
**Conclusion:** PRAM measures the parallelism of a problem. Its value is not a prediction of real time (inferred).
**Example:**
```
paralelno (za i := 0 .. n − 1)
    V[i] := V[i] + 1
```
n processors, one step, O(1). The same notation appears in week 6 (prefix sum): `paralelno (za i := 0 .. n − 1, korak 2^(r+1))`.
PRAM algorithms are building blocks [1]:
- graphs: connectivity, Euler path, distances in a list
- sorting (radix, quicksort), FFT
- tree search (for example chess)
- computational geometry, polynomials, matrices, systems of equations
- prefix sum (week 6): it is still used on a GPU today, for example in radix sort [13]. The notes say the prefix-sum algorithm from 1990 is the basis of the GPU implementation.
The development of PRAM algorithms is a theoretical discipline [1].
**Link:** Lecture 6 (prefix sum uses the same notation). Lecture 2 (MIMD and SIMD).

## EREW, CREW and CRCW   [S8]
**What:** These are the PRAM variants. They differ in what is allowed when several processors access the same location in the same step.

| model | concurrent read of the same location | concurrent write to the same location |
| --- | --- | --- |
| EREW (exclusive read, exclusive write) | no | no |
| CREW (concurrent read, exclusive write) | yes | no |
| CRCW (concurrent read, concurrent write) | yes | yes, with a rule (next section) |

- EREW: no two processors can read or write the same cell at the same time.
- CREW: concurrent reads are allowed. Concurrent writes are not [3].
**Why:** A stronger model allows faster algorithms: EREW ≤ CREW ≤ CRCW [3].
**Conclusion:** The problem collection (PMF) states the model for each algorithm. Check which variant an algorithm needs (inferred).
**Link:** Demo: Vizualizacije/modeli.html, section 1 (ready examples). "How the models connect" explains the strength order.

## CRCW: three write rules   [S9]
**What:** When several processors write to the same cell in one step, CRCW needs a rule that decides who succeeds. Concurrent reads are always allowed [3].

| rule | what holds [3] | P1 writes 3, P2 writes 9 to M[1] | P1 and P2 write 1 to M[1] |
| --- | --- | --- | --- |
| COMMON | the write succeeds only if all write the same value; otherwise the algorithm is not correct and the state is undefined | not allowed | M[1] = 1 |
| ARBITRARY | one randomly chosen processor succeeds; the algorithm must not assume which | M[1] = 3 or 9, unknown | M[1] = 1 |
| PRIORITY | processors have fixed and different priorities; the one with the highest priority succeeds (for example the lowest index) | M[1] = 3, if P1 has the higher priority | M[1] = 1 |

**Why:** The rules are ordered by strength: PRIORITY ≥ ARBITRARY ≥ COMMON. An algorithm written for COMMON runs without change under ARBITRARY and under PRIORITY [3]. If both processors write 1, all three rules give 1.
**Conclusion:** An algorithm written for COMMON runs under every rule. Write for the weakest rule when possible (inferred).
**Example:** COMMON example: "is there a zero in the array" (see the topic on the 1-step vs log n algorithm): all processors that see a zero write 1.
**Link:** Demo: Vizualizacije/modeli.html, section 1, "dvojica pišu istu vrijednost" (two write the same value) and "dvojica pišu različito" (two write different values).

## How the models connect   [S10]
**What:**
- RAM: one processor, no concurrent access. PRAM generalizes RAM [3]. PRAM = p RAM processors, shared memory, lock step [3].
- EREW, CREW and CRCW are not new models next to PRAM. They are three variants of PRAM. They differ only in the rule for several processors that touch the same location in one step. They are ordered from weak to strong.
- An algorithm for a weaker model runs on a stronger model without change and in the same time [3], [9]. An EREW algorithm is also a CREW and a CRCW algorithm.
- The reverse is not free. One CRCW step on EREW takes O(log p) steps [3], [10]. A PRAM algorithm with p processors and t steps takes at most p · t steps on one processor [9].
**Why:** One CRCW step must be simulated on EREW, and the simulation costs O(log p). PRAM is physically impossible [10] because memory access cannot take the same time for any number of processors.

| model | where it is in reality |
| --- | --- |
| RAM | random access machine, not RAM memory: a model of a sequential computer [3], one core (SISD) |
| PRAM (EREW, CREW, CRCW) | an idealization of a computer with shared memory [3] (MIMD [1]); physically impossible [10] |
| aPRAM | a more practical model [2]: each core at its own speed, global memory and barrier are paid for (see "aPRAM") |

**Conclusion:** Use a weaker-model algorithm on a stronger model for free. Do not expect the reverse to be free.
**Link:** Lecture 2 (SISD, MIMD). Demo: Vizualizacije/modeli.html, section 1.

## CRCW takes 1 step, EREW takes log n   [S11]
**What:** Problem: does array A contain at least one zero?
**Why:**
- CRCW (common): every processor that sees a zero writes 1 to the same variable. All write the same value, so COMMON allows it. The time is O(1).
- EREW: two writes to the same place are not allowed. Therefore combine in pairs: eight values, then four, then two, then one. This is a tree and takes O(log n) steps.
**Conclusion:** CRCW needs one step and EREW needs a tree. A stronger model can remove the tree (inferred). The tree pattern is called reduction (reduciranje) in week 6.
**Example:** A = 3 1 7 0 4 1 6 3; the result 1 means "has a zero".
```
ima := 0
paralelno (za i := 0 .. n − 1)
    ako A[i] = 0 onda ima := 1
```
For n = 8, EREW needs 3 OR steps = log₂ 8.
The figure on the slide is a tree of OR values (read from the .pptx shapes). Pairs join level by level:
- leaves: 0 0 0 1 0 0 0 0 (the 1 is at index 3, where A = 0)
- level 1 (label "1."): 0 1 0 0
- level 2 (label "2."): 1 0
- level 3 (label "3."): 1 (the root, the answer "there is a zero")
**Link:** Lecture 6 (reduction). Demo: Vizualizacije/modeli.html, section 2 (the default array is the one on the slide).

## Example: the head of a linked list in PRAM   [S12]
**What:** There are n nodes. next[i] is the successor of node i. The head (glava, the first node) is not known. Processor i works for node i. The list is 3 → 0 → 4 → 1 → 2.
```
paralelno (za i := 0 .. n − 1)
  1:  M[i] := 0
  2:  M[next[i]] := 1   (ako postoji)
  3:  ako M[i] = 0 onda glava := i
```

| i | 0 | 1 | 2 | 3 | 4 |
| --- | --- | --- | --- | --- | --- |
| M after step 1 | 0 | 0 | 0 | 0 | 0 |
| M after step 2 | 1 | 1 | 1 | 0 | 1 |

**Why:**
- Step 3: M[3] = 0, so the head is 3.
- The first node is the only node without a predecessor. Nobody writes 1 to M[3].
- The model is EREW. Each node has at most one predecessor, so nobody writes to the same M cell.
**Conclusion:** Three PRAM steps, each O(1): the total is O(1). This is the main example of the lecture. The teacher then asks: does it also work on your computer? The next section shows that it does not.
**Link:** Demo: Vizualizacije/glava.html, section 1. Exercise 4, task 1.

## Trap: the same algorithm without lock step   [S13]
**What:** The same algorithm in C#, with three steps for each i in one loop body.
**Example:**
```csharp
Parallel.For(0, n, i =>
{
    M[i] = 0;
    if (next[i] >= 0) M[next[i]] = 1;
    if (M[i] == 0) glave.Add(i);
});
```
Output (n = 1000):
```
Lista od 1000 čvorova, prava glava: 654
Parallel.For, bez ograde (1): 1000 glava
Parallel.For, bez ograde (2): 1000 glava
for petlja, bez taskova:      1000 glava
```
**Why:** Look at node 3, the predecessor of node 0.
- Case 1: node 3 writes 1 to M[0] before node 0 starts. Then i = 0 overwrites it with 0 and reads 0.
- Case 2: node 3 writes 1 after i = 0 reads. Then i = 0 has already read 0.
- The figure on the slide has a time axis (vrijeme). Case 1: row "i = 3" shows M[3] := 0 then M[0] := 1, and then row "i = 0" shows M[0] := 0. The figure draws only case 1. Case 2 (node 3 writes 1 after i = 0 has read 0) is in the caption text of the slide and in the notes.
- In both cases node 0 sees 0 and reports itself as a head.
- In PRAM, "parallel" means: step 1 for all, then step 2 for all. It does not mean all three steps for one i.
**Conclusion:** The algorithm is correct only in lock step. Every head is wrong, every time, even in a plain `for` loop with no tasks. The `for` loop is wrong because it runs all three steps for one i before the next i (inferred).
**Pitfall:** A correct PRAM algorithm can be wrong on a real computer. The source states: the algorithm is correct only in lock step. The comparison with a data race is not in the source (inferred).
**Link:** Demo: ModeliDemo, Primjer1_GlavaBezOgrade; Vizualizacije/glava.html, section 2, mode "for petlja". Exercise 4, task 1.

## Fix: a barrier between steps   [S14]
**What:** A barrier (ograda, barijera) is a place where everybody waits until everybody arrives. Nobody continues until all arrive. It restores lock step only where it is needed.
**Why:** Two methods restore lock step:
- Three `Parallel.For` loops. The end of `Parallel.For` waits for all iterations.
- `Barrier`: the same tasks, with `SignalAndWait` between steps.

**Cost:** Each barrier waits for the slowest task and synchronizes all. How much? aPRAM counts this.
**Example:**
```csharp
Parallel.For(0, n, i => M[i] = 0);
Parallel.For(0, n, i =>
    { if (next[i] >= 0) M[next[i]] = 1; });
Parallel.For(0, n, i => { if (M[i] == 0) glava = i; });
```
```csharp
using var ograda = new Barrier(p);
// u svakom tasku, za svoje čvorove i:
M[i] = 0 …;            ograda.SignalAndWait();
M[next[i]] = 1 …;      ograda.SignalAndWait();
ako M[i] == 0: glava = i
```
Outputs:
```
Tri Parallel.For (1): 1 glava, glava = 654
Tri Parallel.For (2): 1 glava, glava = 654
Barrier (1): 1 glava, glava = 654
```
**Conclusion:** A barrier restores lock step only where it is needed. Put a barrier between the steps that depend on each other (inferred).
**Link:** Demo: ModeliDemo, Primjer2_TriPetlje and Primjer3_Barrier; Vizualizacije/glava.html, section 2, "taskovi s ogradom", then "Pokreni 1000 puta" (run 1000 times). Exercise 4, task 1. Reference [4] (Barrier.SignalAndWait).

## aPRAM: the asynchronous PRAM   [S15]
**What:**
- Gibbons introduced aPRAM (asinkroni PRAM) in 1989 [2].
- p processors, each with local memory. All share the global memory.
- No common clock. Each processor runs its own program.
- Four instruction types: global read, global write, local operation, barrier.
- A program consists of phases (faze) separated by barriers.
- The program must be correct with any delay of any processor.
- Costs: local access 1, global access d (d > 1), barrier B.
**Why:** Gibbons observed that real MIMD computers have more limited communication than PRAM [2], and have no common clock. Therefore processors work asynchronously, global access costs d and the barrier costs B. The teacher's picture: hikers. Each hiker walks at his own speed. At the viewpoint (the barrier) all wait for the last hiker. Each wait is paid for.
**Figure (S15):** global memory on top; CPU1, CPU2, CPU3 below; the label "lokalna" (local) under each CPU; the cost labels d (> 1) for global and 1 for local access.
**Conclusion:** aPRAM measures what parallelism costs. Correctness must not depend on speed.
**Link:** "The phase rule" (what the phases forbid). The figure on the slide shows memory, CPU1-CPU3, cost labels 1 and d.

## The phase rule: no data race   [S16]
**What:** Inside one phase no processor can read a location that another processor writes. A barrier must separate them.

| variant | allowed in the same phase |
| --- | --- |
| EREW | nobody shares a location |
| CREW | several read, nobody writes |
| CRCW | several write (common, arbitrary), nobody reads |

**Why:** Therefore the model has no data race. This links to earlier lectures:
- Bernstein (lecture 3): two statements can run at the same time if neither reads or writes what the other writes.
- Data race (lecture 4): at the same time, at least one write, no synchronization.
- aPRAM forbids this inside a phase.
**Conclusion:** Linked-list head: step 2 writes M[j], step 3 reads M[i]. Therefore they belong to different phases with a barrier between them.
**Link:** Lecture 3 (Bernstein conditions). Lecture 4 (data race).

## The cost in aPRAM: d and B   [S17]
**What:**

| instruction | duration |
| --- | --- |
| local operation | 1 |
| global read or write | d |
| k consecutive global accesses | d + k − 1 |
| barrier for p processors | B (grows with p) |

- Assumption: 2 ≤ d ≤ B ≤ p.
- Typically d grows as log p or √p (the path through the network). B grows as d or d · log p [2].
**Why:** Consecutive global accesses overlap, like instruction pipelining (protočna obrada, pipeline) in lecture 1.
**Example:** k = 4 global reads, d = 4. The reads finish at time 7 = d + k − 1, not at k · d = 16. The figure shows four bars (čitanje 1 to čitanje 4). Each bar is d = 4 wide, and each starts 1 unit after the previous one. The time axis has the ticks 0, 4 and 7.
**Conclusion:** A barrier is not a constant. It grows with p.
**Link:** Lecture 1 (pipeline).

## A PRAM step on aPRAM, p processors   [S18]
**What:** The simplest way to make a PRAM algorithm correct on aPRAM is a barrier after each read and write. One PRAM step becomes: global read, barrier, local computation, global write, barrier.
**Example:** With d = 4 and B = 16: d + B + 1 + d + B = 41 units per step. In general: 2B + 2d + 1 per step. On PRAM one step costs 1. Of the 41 units, only 1 is computation and 32 are barriers.
**Conclusion:** A PRAM algorithm of t steps costs O(B · t), because B ≥ d.
**Pitfall:** The numbers d = 4 and B = 16 are invented. They only satisfy 2 ≤ d ≤ B ≤ p.
**Link:** Demo: Vizualizacije/modeli.html, section 3.

## Tracing execution on aPRAM   [S19]
**What:** The example is the list head from S12, with processor Pi for node i, d = 4 and B = 16 (invented). P3 is late by 10 units before step 2. The picture shows each processor as a row in time. Orange is a global access (d units). Blue is a local comparison (1 unit). Grey is a barrier (B).
**Why:** A phase ends when the slowest processor ends. Then comes the barrier.
- With barriers: done at 71 and head = 3 (correct). All wait for P3. Without the delay it would be 61 = 3d + 3B + 1.
- Without barriers: done at 23, but the heads are 0 and 3 (wrong). P0 reads M[0] at time 12. P3 writes 1 only at time 18.

**Example:** The figure, rebuilt from the .pptx shapes. The list is 3 → 0 → 4 → 1 → 2, so the successor writes are: P0 writes M[4], P1 writes M[2], P3 writes M[0], P4 writes M[1], and P2 has no successor. The mapping of the bars to operations is inferred from the shape positions. Time axis ticks: 0, 20, 40, 60, 71.
With barriers (d = 4, B = 16, P3 late by 10):
- Phase 1: every Pi writes M[i] := 0 (d). Then a barrier (B). Time 0 to 20.
- Phase 2: P0, P1, P4 write the successor cell (d). P2 has no write and waits ("čeka najsporijeg"). P3 is late (kasni, 10 units) and then writes M[0] := 1 (d). All others wait for P3. Then a barrier (B). Time 20 to 50.
- Phase 3: every Pi reads M[i] (d) and compares (1 unit). Then a barrier (B). Time 50 to 71.
- Arithmetic: 71 = 10 + 61, and 61 = 3d + 3B + 1 = 12 + 48 + 1.

Without barriers (axis ticks 0, 10, 20, 23):
- P0, P1, P4: three global accesses (d each) in a row: write M[i], write the successor cell, read M[i]. P2: two accesses. No waits.
- P3: d, then late (10), then d (writes M[0] := 1, bar from time 14 to 18), then d (read, time 18 to 22).
- Arithmetic: P3 takes 4 + 10 + 4 + 4 = 22, plus 1 for the local comparison = 23 (inferred).
- P0 has already read M[0] (time 12) when P3 finishes writing 1 to M[0] (time 18). The notes say "at 12" and "at 18"; both are the end times of the bars (inferred from the shapes). Therefore the heads are 0 and 3.

Legend of the figure: global access d, local 1, late (kasni), waits for the slowest, barrier B.
**Conclusion:** Without barriers the program is correct only if nobody is late by more than d. aPRAM demands correctness with any delay.
**Link:** Demo: Vizualizacije/glava.html, section 3 (same example; change the delay and switch off the barriers).

## Trap: the barrier gets expensive   [S20]
**What:** Measured: B grows with p. The tasks do nothing except the barrier.

| p | time of one barrier |
| --- | --- |
| 1 | 52 ns |
| 2 | 242 ns |
| 4 | 499 ns |
| 8 | 1,236 ns |
| 12 | 3,113 ns |
| 24 | 19,037 ns |

Measurement: `Barrier.SignalAndWait`, 20,000 barriers, best of 5; i7-9750H, .NET 10, Release.
**Why:** The numbers show the growth of B with p. From 1 to 12 tasks the barrier costs about 60 times more (52 → 3,113 ns). With more tasks than cores it costs 6 times more again (12 → 24).
**Conclusion:** Gibbons (1989): PRAM algorithms are "far too fine-grained for real machines". Do not use one processor per element (inferred from S29).
**Pitfall:** This is the same trap for the third time: too-small tasks (lecture 2), the overhead c · N and the best N (lecture 3), and `Parallel.For` per element (exercise 3).
**Link:** Demo: ModeliDemo, Primjer6_CijenaOgrade (about 5 s); Vizualizacije/modeli.html, section 4.

## Fix: p / B processors   [S21]
**What:** Each aPRAM processor simulates B PRAM processors. Example: p = 1024, d = 4, B = 16 (invented). One simulated step:

| part | duration |
| --- | --- |
| B reads (pipelined) | d + B − 1 |
| barrier | B |
| B local instructions | B |
| B writes (pipelined) | d + B − 1 |
| barrier | B |
| total | 5B + 2d − 2 = O(B) |

| | processors | per step | work |
| --- | --- | --- | --- |
| same number of processors | 1024 | 41 | 41,984 |
| p / B processors | 64 | 86 | 5,504 |

**Why:**
- 16 times fewer processors, and the same O(B · t).
- Work (processors × time) is about 7.6 times smaller. Total work is O(p · t), as on PRAM (Gibbons, Lemma 1).
- 86 > 41: the constants are larger, the order of magnitude is the same.
- Assumption: the barrier for p / B processors costs at most B.
**Conclusion:** Use fewer processors and give each processor more work. This reduces total work. A smaller barrier cost follows from the assumption above (inferred).
**Link:** Demo: Vizualizacije/modeli.html, section 3 (compare the work in the table).

## In C#: one task for many iterations   [S22]
**What:** Mapping of aPRAM to C#:

| aPRAM | C# |
| --- | --- |
| PRAM processor per element | iteration i |
| aPRAM processor simulates B PRAM processors | a task handles a range of iterations (Partitioner) |
| barrier | end of Parallel.For, Barrier, Task.WaitAll |
| phase without reading foreign writes | each task writes to its own part of the array |

**Why:** `Parallel.For` does not create a task per iteration. One task handles a range, as the Partitioner did in exercise 3.
**Example:** Example 3 from the lecture: 12 tasks for 1000 nodes. Each task handles its own nodes (i = id, id + 12, ...) and waits twice at the barrier. This is an aPRAM with 12 processors that simulate 1000 PRAM processors.
**Link:** You already saw phases in odd-even sort (par-nepar: phase, wait, next phase) and in the wavefront (valna fronta: anti-diagonal by anti-diagonal). Exercise 3, task 3: larger ranges are faster.

## After PRAM: BSP and LogP   [S23]
**What:** Critique of PRAM [12]: it is unrealistic. It assumes that all processors run synchronously and that communication is free. Later models count exactly these costs.

| model | what it counts | synchronization |
| --- | --- | --- |
| aPRAM (Gibbons, 1989) [2] | global access d, barrier B | phases separated by a barrier |
| BSP (Valiant, 1990) [11] | components that compute, a router that delivers messages, synchronization every L time units | a series of supersteps; a barrier at the end of each |
| LogP (Culler et al., 1993) [12] | P processors, L message latency, o overhead of sending and receiving, g smallest gap between messages | asynchronous, no lock step |

**Why:** Notes on BSP: in each superstep the processors compute and send messages, and at the end there is a barrier. Notes on LogP: it has no lock step and describes communication with four numbers: how many processors, how late a message is, how much time a processor spends on sending, and how long it must wait between two messages.
**Conclusion:** In all these models communication and synchronization are paid for.
**Link:** MPI (weeks 9-11) has the same ingredients: local computation, messages and `comm.Barrier()`.

## Next week: prefix sum on PRAM   [S24]
**What:** Input and output of the prefix sum:
```
ulaz:  3  1  7  0  4  1  6  3
izlaz: 3  4 11 11 15 16 22 25
```
- Sequential: O(n).
- PRAM (EREW): a tree, log₂ n steps, each O(1).
- aPRAM: a barrier after each tree level, O(B · log n).
- Gibbons: a tree with B branches per node, O(B · log n / log B).
**Conclusion:** Each tree level is one PRAM step. Question for next week: what happens in C# if you omit the wait between levels? The source gives no answer. By the logic of the list head trap, the result would be wrong (inferred).
**Link:** Lecture 6.

## Summary of the models   [S25]
| model | clock | memory access | synchronization | purpose |
| --- | --- | --- | --- | --- |
| RAM | one processor | 1 | — | sequential complexity |
| PRAM | common (lock step) | 1 | free, in every step | how much parallelism the problem has |
| aPRAM | each processor its own | local 1, global d | barrier, costs B | what parallelism costs |

**Conclusion:** Trap: an algorithm correct only in lock step. Fix: a barrier. Trap: a barrier for too many processors. Fix: fewer processors, more work per processor.

## async/await: concurrency   [S26]
**What:**
- `await` waits without holding a thread. The thread returns to the thread pool (fond niti) during the wait.
- It is useful when the program waits for something outside itself: network, disk, user, clock.
- It adds no cores. It does not speed up computation.
- `Task.WhenAll` waits for all tasks and does not block a thread (lecture 1, slide 21).
**Why:** Waiting needs no core. Therefore 100 waits take as long as 1 wait. This is concurrency (konkurentnost), not parallelism.
**Example:**
```csharp
static async Task<int> Dohvati(int id)
{
    await Task.Delay(1000);   // npr. mreža
    return id;
}
```
Output:
```
jedno po jedno, 3 × Delay:    3003 ms
Task.WhenAll, 3 × Delay:      1001 ms
Task.WhenAll, 100 × Delay:    1004 ms
```
Three waits of 1 s one by one: 3 s. With WhenAll: 1 s, without extra cores. The figure shows three 1 s bars one after another (3 s) against three 1 s bars side by side (1 s, "WhenAll: 1 s, bez dodatnih jezgri").
**Link:** Lecture 1 (`Task.WhenAll`, no blocking). Demo: ModeliDemo, Primjer4_AsyncCekanje (about 12 s); Vizualizacije/async.html, section 1. Exercise 4, task 2. Reference [5].

## Trap: async does not mean parallel   [S27]
**What:** A method marked `async` with no `await` inside only computes. It runs to the end in the caller's thread, before the program reaches `WhenAll`.
**Why:** The three calls run one after another. `WhenAll` is then as slow as one by one. The compiler warns (CS1998) [6].
**Example:**
```csharp
static async Task<long> Racunaj(long n)
{
    long s = 0;
    for (long i = 0; i < n; i++) s += i % 7;
    return s;
}
await Task.WhenAll(Racunaj(N),
                   Racunaj(N), Racunaj(N));
```
Output:
```
3 poziva, jedan po jedan:            1915 ms
Task.WhenAll, async bez Task.Run:    1686 ms
Task.WhenAll, s Task.Run:             513 ms
```
With `Task.Run`: 513 ms instead of 1686 ms.
**Conclusion:** Start work that computes with `Task.Run` or `Parallel.For`. Use async/await for waiting.
**Link:** Demo: ModeliDemo, Primjer5_AsyncBezParalelizma; Vizualizacije/async.html, section 2. Exercise 4, task 2.

## Trap: blocking in the middle of async code   [S28]
**What:**

| blocks the thread | does not block |
| --- | --- |
| `Thread.Sleep(1000)` | `await Task.Delay(1000)` |
| `t.Wait()`, `t.Result` | `await t` |
| `Task.WaitAll(ts)` | `await Task.WhenAll(ts)` |

**Why:**
- `Thread.Sleep`, `Wait` and `Result` hold the thread while they wait.
- 100 waits of 1 s (12 logical cores): `Task.WhenAll` with `Delay` takes 1004 ms. With `Sleep` it takes 7006 ms.
- The thread pool must add threads, and this takes time. `await` does not hold a thread.
- `await` is not allowed inside `lock`. The lock (Monitor) must be released by the same thread that took it [7]. The code after `await` can run on another thread (lecture 4). This answers the question from lecture 4.
**Conclusion:** In async code, use the right column of the table. Never use `await` inside `lock`.
**Pitfall:** The pool adds threads gradually, not all at once. This comes from a code comment of exercise 4, task 3, and not from the lecture.
**Link:** Exercise 5: the same wait with cancellation, `CancellationToken`. Question from the notes: what if a task sleeps exactly when you request the cancel? `Sleep` holds the thread; `Task.Delay` with a token stops while it sleeps. Demo: Vizualizacije/async.html, section 3. Exercise 4, task 3. Reference [7].

## Summary: traps and fixes   [S29]
| trap | how you see it | fix |
| --- | --- | --- |
| The model says everything costs 1 | same number of operations, 450 times different time | aPRAM: d for global access, B for barrier |
| Algorithm correct only in lock step | list head: 1000 heads instead of one | barrier between steps: three Parallel.For or Barrier |
| Reading what another writes in the same phase | the result depends on the order | split into two phases |
| Processor per element, barrier for all | barrier 60 times more expensive for 12 tasks | p / B processors, B elements each |
| async method that computes | WhenAll as slow as one by one | Task.Run or Parallel.For |
| Blocking in async code | Sleep: 7 s instead of 1 s | await Task.Delay, await, Task.WhenAll |

**Conclusion:** The table summarizes all traps of the lecture. Each trap links to a section above.

## References   [S30]
References on the slide:
- [1] T. Dadić: Modeli paralelnih računala, lecture, 2022/23.
- [2] P. B. Gibbons: A More Practical PRAM Model. Proc. 1st ACM SPAA, 1989, 158-168.
- [3] M. Moreno Maza: Parallel Random-Access Machines, slides, CS3101, University of Western Ontario.
- [4] Microsoft Learn: Barrier.SignalAndWait Method.
- [5] Microsoft Learn: Asynchronous programming with async and await.
- [6] Microsoft Learn: Resolve errors and warnings in async methods (CS1998).
- [7] Microsoft Learn: System.Threading.Monitor class.
- [8] Zadaci iz kolegija Paralelno programiranje v1.3, PMF, 2017.
- [9] S. Rajasekaran: Randomization in Computing, CSE 6512, lecture 16, University of Connecticut, 2011.
- [10] J. F. Prins: PRAM (4), slides, COMP 633, University of North Carolina at Chapel Hill.
- [11] L. G. Valiant: A bridging model for parallel computation. Communications of the ACM 33(8), 1990.
- [12] D. Culler et al.: LogP: Towards a Realistic Model of Parallel Computation. 4th ACM SIGPLAN PPoPP, 1993.
- [13] M. Harris, S. Sengupta, J. D. Owens: Parallel Prefix Sum (Scan) with CUDA. GPU Gems 3, chapter 39, NVIDIA.

Notes: the sources are the lecture of Dr. Tonći Dadić, the Gibbons paper, JaJa's book on parallel algorithms, the Microsoft documentation, and the problem collection. Next week: prefix sum.

## Exercise cross-reference
**Exercise 4 (`exercises/04-models-async/`, `Vjezba4_upute.html`, `Vizualizacije/`)**
How to run: the projects target .NET 10 (`Directory.Build.props`). Run in the task folder with `dotnet run -c Release`, because the tasks measure time. Docker: `docker compose run --rm dotnet dotnet run -c Release --project exercises/04-models-async/Zadatak1_GlavaListe`. With Visual Studio 2022 only, change `net10.0` to `net8.0` (stated in `Directory.Build.props`, and in the exercise 5 text for exercise 5).
Instruction lines of the exercise text:
- Task 1: run the given version. How many heads does it find, and why? Write three consecutive `Parallel.For` loops. How many heads? Write p tasks with `Barrier`: task id handles nodes id, id + p, ... and the barrier is between the steps.
- Task 2: call `Dohvati` with `Task.WhenAll` for 3 and 100 calls. Why is 100 as fast as 3? Call `Racunaj` three times with `Task.WhenAll` without `Task.Run`, then with `Task.Run`. Explain the difference.
- Task 3: the given code has 100 waits with `await Task.Delay`. Write the same wait with `Thread.Sleep(1000)` in `Task.Run`: how long, and why? Write the same wait with a cancel: `CancellationTokenSource`, cancel after 300 ms. In which state do the tasks end? Answer from the code: canceled (`IsCanceled` is true for 100 of 100).

| task | file | concept | key insight | what goes wrong if ignored |
| --- | --- | --- | --- | --- |
| 1: list head | `exercises/04-models-async/Zadatak1_GlavaListe/Zadatak1_GlavaListe.cs` | Algorithm correct only in lock step; barrier (S12-S14, S16) | `BezOgrade` gives 1000 heads. `TriPetlje` uses two `Parallel.For` loops and counts the cells where `M[i] == 0`; the result is 1. `SOgradom` uses p = 12 tasks. Task id handles the nodes `i = id, id + p, ...`. The code has two `SignalAndWait` calls. The last phase counts the heads with `Interlocked.Increment`. The barriers give this order: nobody writes ones before all write zeros, and nobody reads M before all write ones. Each node has at most one predecessor, so step 2 has no collision. | Without the barrier a node overwrites the 1 of its predecessor or reads too early. The program reports 1000 heads instead of 1. |
| 2: async (code comment: "VAŽNO: vrijeme mjerite u Release konfiguraciji", measure time in Release) | `exercises/04-models-async/Zadatak2_Async/Zadatak2_Async.cs` | Concurrency, not parallelism (S26-S27) | One by one 3 × `Delay`: about 3003 ms. `WhenAll` for 3 and 100: about 1001 ms and 1004 ms, because waiting needs no cores. `Racunaj` with N = 600,000,000: one by one about 1030 ms; `WhenAll` without `Task.Run` about 1017 ms (a method without `await` runs in the calling thread); with `Task.Run` about 348 ms, about one third. | Without `Task.Run` the computation is sequential and `WhenAll` gives no speed-up. |
| 3: blocking | `exercises/04-models-async/Zadatak3_Blokiranje/Zadatak3_Blokiranje.cs` | Blocking in async code; cancellation (S28) | 100 × `Delay`: about 1004 ms. 100 × `Thread.Sleep` in `Task.Run`: for example 7528 ms, because the pool adds threads gradually. Cancel after 300 ms with `CancellationTokenSource.CancelAfter(300)`: about 324 ms and 100 of 100 tasks are canceled, because `Task.Delay` with a token stops while it sleeps (`Thread.Sleep` would not). `WhenAll` throws the exception of the first canceled task. | Blocking calls hold threads. The wait grows from 1 s to about 7 s, and a sleeping thread cannot react to a cancel request. |

**Exercise 5 (`exercises/05-periodic-tasks/`), related by S28 ("Na vježbi 5")**
- `Zadatak1_ImeSvakeSekunde/Zadatak1_ImeSvakeSekunde.cs`: uses `token.WaitHandle.WaitOne(1000)` instead of a sleep. The task wakes at once on cancel: about 1 ms, against up to about 1000 ms with `Thread.Sleep`. This answers the question "what if a task sleeps when you request the cancel" (inferred).
- `Zadatak3_PeriodicnoPrekid/Zadatak3_PeriodicnoPrekid.cs`: the same wait with cancellation. `ThrowIfCancellationRequested` gives `Status = Canceled`. A task started with an already-canceled token does not start, so the action is never called. The task ended about 40 ms after `Cancel()`.
- `Zadatak5_OgradaZastoj/Zadatak5_OgradaZastoj.cs` (optional): the list head of this lecture (example 3) with p = 8 tasks and a `Barrier`. One randomly chosen task is late by 10 s before the second barrier, like a late processor. Starter (commit 4dc41e7): the late task calls `Thread.Sleep(10_000)`, which a cancel cannot wake, the barrier is `ograda.SignalAndWait()` with no token, and there is no `CancellationTokenSource`. Solution (the file in the repo): the late task calls `token.WaitHandle.WaitOne(10_000)`. Tasks start with `TaskCreationOptions.LongRunning`. `Random(1)` gives the real head 654 ("Prava glava: 654; 8 taskova, task 6 zastane 10 s" is the sample output; the late task is random). The solution makes the tasks give up after 2 s: `new CancellationTokenSource(2000)` and `SignalAndWait(token)`. The solution output is "Rezultat nije dobiven (2010 ms)". The insight: with barriers, all wait for the slowest (S19). A barrier without a cancel token can wait without end (inferred).
Instruction lines of exercise 5 (`Vjezba5_upute.html`; run with `dotnet run -c Release`; .NET 10, change to `net8.0` for Visual Studio 2022 only; task 1 waits for Enter, so run it in a terminal):
- Task 1: write your name in the constant `Ime`. Run five times and press Enter at different times. Write the measured times. What do they depend on? Change the task so that less than 50 ms pass from `Cancel()` to the end, and the name still prints every second. The given sample: "Task završio 592 ms nakon Cancel()".
- Task 2: write `static void Periodicno(Action akcija, int ms)`. Run the test with ms = 1000. How many calls and at which times do you expect in 9.5 s? Change the method so that the 10th call is in the 9th second (± 50 ms) although the action takes 300 ms.
- Task 3: write `static Task Periodicno(Action akcija, int ms, CancellationToken token)`. The schedule is as in task 2. The task must end at most 100 ms after `Cancel()`. Measure in Release without a debugger (`dotnet run -c Release` or Ctrl+F5; F5 starts a debugger, also in Release). In both tests the status must be `Canceled` and the program must not end with an exception. What happened to the action in test 2, and why?
- Task 4: write `static Task<List<U>> Periodicno2<U>(Func<U> f, int ms, CancellationToken token)`. After the cancel, the list of all values is the `Result`. Run test 1 (`CancelAfter(3500)`). What happens in test 2, and why can it not happen with your method? Fix test 2 so that the shared list stays and the program runs without error.
- Rule from the exercise text: a task cannot be stopped from outside. The main thread only requests a cancel. The task must check the token itself and finish. This is the reason behind the S28 question: a task that sleeps with `Thread.Sleep` when the cancel is requested does not see the request until the sleep ends.
- Task 5 (optional): run the program. How long do the other tasks wait? Is the result correct? Change the program so that the tasks give up 2 s after the start and main prints that there is no result. The text of the exercise says: "Točan ne znači brz: ograda čeka najsporijeg" (correct does not mean fast: the barrier waits for the slowest).
- Wait table from the exercise text (call: when cancel is requested / without cancel): `Thread.Sleep(ms)`: nothing, sleeps to the end / returns after ms. `token.WaitHandle.WaitOne(ms)`: returns at once with true / returns false after ms. `token.ThrowIfCancellationRequested()`: throws `OperationCanceledException` / nothing. `await Task.Delay(ms, token)`: throws `TaskCanceledException` at once / returns after ms. `ograda.SignalAndWait(token)`: stops waiting and throws `OperationCanceledException` / waits for the others. Measured (wait 2000 ms, cancel after 300 ms): `Thread.Sleep` 2000 ms, the others 300-336 ms.
- Status after cancel: `RanToCompletion` if the delegate only leaves the loop, `Canceled` if it throws `OperationCanceledException`. Without a token passed to the method that creates the task, the status is `Faulted` in that case. `Wait()` on a canceled task throws `AggregateException`.
- The starter code of task 5 (not the solution) prints "Glava: 654 (10016 ms)": the result is correct, and all other tasks wait about 10 s for the slow task. The sample output of the exercise text for task 5 says "task 2 zastane 10 s"; the code comment says "task 6". The late task is random.

Details of each task code:
- `Zadatak2_Periodicno/Zadatak2_Periodicno.cs`: `Periodicno(Action, ms)` starts `Task.Run` and calls the action every ms, forever. The wait is `Thread.Sleep(max(0, k * ms - elapsed))`, so the k-th call starts at (k − 1) · ms with no accumulated delay. The action takes 300 ms. Main waits 9.5 s. Output: "poziv 1: 2 ms" to "poziv 10: 9007 ms", then "Kraj nakon 9507 ms, poziva: 10". There is no cancel. The pool task ends with the program. Link to S28: `Thread.Sleep` holds a pool thread and a cancel request cannot wake it. This is the base case for the S28 question (inferred).
- `Zadatak4_Periodicno2/Zadatak4_Periodicno2.cs`: `Periodicno2<U>` returns `Task<List<U>>` with a local list that nobody else sees. It waits with `token.WaitHandle.WaitOne(max(0, k * ms - elapsed))`. Test 1, `CancelAfter(3500)`, gives "Elemenata: 4: 2, 1005, 2007, 3005". The given `PeriodicnoUZajednickuListu` uses a shared list with `lock(lista)` and `Thread.Sleep`. Test 2 reads the list under `lock`: for example 471719 reads and 13523 elements. Without the lock, `InvalidOperationException` occurs because the list changes during `foreach`. Link to S28 (a wait that a cancel request can wake, instead of `Sleep`) and to lecture 4 (lock and data race).

## Open points
- S19: the mapping of the bars to operations and the sum 4 + 10 + 4 + 4 + 1 = 23 are rebuilt from shape positions and sizes (inferred). The source states only the totals. The notes say P0 reads at 12. In the shapes the third bar of P0 ends at 12. I used that.
- S2: the slide says the lock is 90 times slower than sequential counting. The values 456 ms and 5 ms (S5) give 91.2 (my own division, inferred).
- S10 says an EREW algorithm runs on CREW and CRCW "in the same time". This was taken as written.
- S20: the Croatian text writes 1.236 ns with a point as a thousands separator. I wrote 1,236 ns.
- S29 "exam-style questions": the source has no separate exam questions. The only open questions are S24 (omit the wait between levels) and S28 (sleep during cancel).
- The demos (ModeliDemo Primjer1-6) refer to code that is not in the repo folders I read.

## Coverage ledger
Note (round 3): the cancel rule and the given-code result of task 5 were added to the exercise 5 block. Figure labels of S4, S6, S13, S15 and S26 are now recorded in their sections. Exercise instruction lines are in the cross-reference.
| slide | topic section where it is covered | items |
| --- | --- | --- |
| 1 | Map (author, basis, year, goals) | 4 |
| 2 | Why a computer model | 5 |
| 3 | Why a computer model (including steps, memory, waiting) | 7 |
| 4 | RAM | 7 |
| 5 | The trap of the model | 7 |
| 6 | PRAM | 9 |
| 7 | PRAM | 9 |
| 8 | EREW, CREW and CRCW | 6 |
| 9 | CRCW: three write rules | 7 |
| 10 | How the models connect | 9 |
| 11 | CRCW 1 step, EREW log n (tree values from the .pptx) | 9 |
| 12 | Example: head of a linked list | 7 |
| 13 | Trap: lock step | 8 |
| 14 | Fix: a barrier | 8 |
| 15 | aPRAM | 9 |
| 16 | The phase rule | 6 |
| 17 | The cost in aPRAM (with figure ticks) | 9 |
| 18 | A PRAM step on aPRAM | 6 |
| 19 | Tracing execution (per-processor rebuild from the .pptx) | 14 |
| 20 | Trap: the barrier gets expensive | 9 |
| 21 | Fix: p / B processors | 9 |
| 22 | In C#: one task for many iterations | 7 |
| 23 | After PRAM: BSP and LogP | 8 |
| 24 | Next week: prefix sum | 6 |
| 25 | Summary of the models | 5 |
| 26 | async/await: concurrency | 8 |
| 27 | Trap: async does not mean parallel | 7 |
| 28 | Trap: blocking | 9 |
| 29 | Summary: traps and fixes | 6 |
| 30 | References | 13 |
