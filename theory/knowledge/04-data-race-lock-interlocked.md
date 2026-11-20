# Lecture 4: Data race, lock and Interlocked (source: 04-data-race-lock-interlocked.md, 23 slides)
Author on slide 1: Marin Aglić Čuvić, after the lectures of Tonći Dadić. Academic year 2026/2027.

## Map
- This file covers the problem of shared mutable data, four fixes, the cost of the fixes, and the new traps that the fixes create.
- Main idea: first try not to share mutable data. Synchronize only what you must, and as rarely as you can.
- Words: a "trap" is a typical error that a student can make. A "pitfall" label lists a warning about a topic. A "lock object" is the object that a task locks. The "lock statement" is the C# `lock (...)` code. The "Lock type" is `System.Threading.Lock`.
- Flow: the trap (data race) -> why it happens (three steps) -> four fixes -> lock statement -> Interlocked -> cost -> synchronize less often -> traps that remain (check-then-act, collections, deadlock) -> choice table.
- Terms: data race (utrka za podatke) is a technical term. Race condition (utrka) is a wider term. One can exist without the other.
- Previous lectures: lecture 2 gave the shared counter and the fix "isolation". Lecture 3 gave the Bernstein conditions and the task state "waiting".

## Context from earlier lectures   [S1-S2]
**What:** Until now the course avoided shared data: each task had its own data. This lecture asks what to do when a datum must be shared, how to protect it, and what the protection costs.
**Why:** In lecture 2 the shared counter K gave pi = 0,30, because K++ from many tasks loses increments. This is a data race. The fix then was isolation. In lecture 3 the Bernstein conditions said that two parts can run at the same time only if they do not write what the other part reads or writes.
**Conclusion:** The question for today: how to share a datum and lose neither correctness nor speed?
**Example:** The lecture has three parts: (1) Trap: data race when a datum must be shared, and the difference between data race and race condition. (2) Fixes: critical section, lock statement, atomic operations (Interlocked), and their cost. (3) New traps: wrong locks, "check, then change", collections, deadlock (zastoj).
**Link:** Lecture 2 (shared counter, isolation). Lecture 3 (Bernstein, Foster agglomeration, task states). Week 7 (deadlock, semaphores, Monitor, starvation, concurrent collections).

## Trap: data race on a counter   [S3]
**What:** 12 tasks each increment one shared `long` counter 1.000.000 times. The expected result is 12.000.000.
**Why:** The tasks write the same variable at the same time without protection (see "Why a data race happens"). On one core the error would be rare. It becomes clear only when the tasks really run at the same time.
**Conclusion:** A data race gives a wrong result, a different result on each run, no compiler message, and no exception. You cannot find it by looking at one run (inferred).
**Example:**
```csharp
long brojac = 0;   // jedan za sve
var zadaci = new Task[k];
for (int j = 0; j < k; j++)
    zadaci[j] = Task.Run(() =>
    {
        for (int i = 0; i < Povecanja; i++)
            brojac++;
    });
Task.WaitAll(zadaci);
```
Output (k = 12, Povecanja = 1.000.000, expected 12.000.000):

| Attempt | Result | Lost |
| --- | --- | --- |
| 1 | 2.636.084 | 78 % |
| 2 | 2.201.987 | 82 % |
| 3 | 1.790.437 | 85 % |

The teacher says: 78 to 85 percent of the increments are lost, and the number is different each time.
**Pitfall:** The compiler reports nothing and the program throws no exception. The slide says that on one core the error would be rare. The notes say that the result would almost always be correct. Therefore a test on one core can hide the error (inferred).
**Link:** It is the shared counter of pi from lecture 2 and trap B of exercise 2, without random numbers: only the counting remains. Demo: DataRaceDemo, Primjer1_DataRace. Run it two times. The result changes. Exercise: Zadatak1_Brojac, step 1.

## Why a data race happens: brojac++ is three steps   [S4]
**What:** The processor runs `brojac++` as three steps: load the value into a register, increase the register, write the register back to memory (sources [2], [3]).
**Why:** Each task has its own register. The memory location is shared. If two tasks load the value before one of them writes, both compute from the same old value. The second write overwrites the first.
**Conclusion:** Two increments can add only one. The scheduler (raspoređivač) chooses the order of the steps. The order does not repeat. Therefore the result is different each time (nondeterminism, nedeterminizam). On many cores the steps also overlap in real time.
**Example:** Two tasks, the counter is 5.

| Step | Task A (register) | Task B (register) | brojac in memory |
| --- | --- | --- | --- |
| 1 | loads 5 | | 5 |
| 2 | | loads 5 | 5 |
| 3 | 5 + 1 = 6 | | 5 |
| 4 | | 5 + 1 = 6 | 5 |
| 5 | writes 6 | | 6 |
| 6 | | writes 6 | 6 |

Two increments, and the counter grew by one. Task B writes a value computed from the stale 5 and overwrites the write of task A.
**Link:** Demo: Week 02, Vizualizacije/tjedan2.html, section 6, step by step. Exercise visualization: brojac.html. This lost update repeats in List<T> (see "Trap: List<T>") and in the linked stack (see "How an element is lost").

## Data race and race condition   [S5]
**What:**

| Term | Definition (source [4]) | Example |
| --- | --- | --- |
| Data race (utrka za podatke) | Two accesses to the same memory location, from two threads, at the same time. At least one access is a write. No access is synchronized. | `brojac++` from many tasks |
| Race condition (utrka) | The correctness of the program depends on the order or the timing of the steps of the threads. | "Check, then change": each step is protected, but the result is wrong |

**Why:** One can exist without the other (source [4]). A program can have every step protected, so no data race, and still depend on timing. Therefore it is not enough to protect each variable. You must protect each operation that must be complete.
**Conclusion:** Protect every operation that must stay whole, not only every variable. The counter is both a data race and a race condition.
**Link:** Bernstein (lecture 3): a data race is a violated Bernstein condition (O1 intersect O2 not empty, or I intersect O not empty) without synchronization. The example without a data race is in "Trap: check, then change". Source [4]: J. Regehr, "Race Condition vs. Data Race". Exercise: Zadatak2_Racun, step 1 asks to explain why this is a race condition and not a data race.

## Four fixes   [S6]
**What:**
1. Isolation (izolacija): each task has its own data. We merge the results after the wait (PiDemo, lecture 2).
2. Immutability (nepromjenjivost): after creation the datum is only read. Reading at the same time is allowed (Bernstein).
3. Mutual exclusion (međusobno isključivanje): the lock statement. In a critical section at most one task runs. The others wait.
4. Atomic operations (atomarne operacije): `Interlocked`. One operation on one variable runs as one indivisible step.

**Why:** Fixes 1 and 2 do not share a mutable datum, so they cost nothing. Fixes 3 and 4 synchronize the access, and this has a cost (see "Cost of synchronization").
**Conclusion:** Always try fixes 1 and 2 first. The lecture treats fixes 3 and 4 in most detail.
**Link:** Choice table in "What to choose". Isolation is also the base of "Fix: synchronize less often".

## Critical section   [S7]
**What:** A critical section (kritični odsječak) is the part of the code that accesses a shared datum. Mutual exclusion means that at most one task is in the critical section at a time.
**Why:** In the lecture drawing, task 2 holds the lock object (brava) and runs `brojac++`. Tasks 1, 3 and 4 wait at the entry, with one line each into the section (the entry lines have no arrowheads). One arrow on the right leaves the section, with the label "izlaz: otpusti bravu, uđe idući" (exit: release the lock object, the next task enters). (Checked in the .pptx shapes.) When task 2 leaves and releases the lock object, the next task enters.
**Conclusion:** Waiting is the price. The waiting tasks are idle, in the state "waiting" from lecture 3. Therefore the critical section must be as short as possible.
**Link:** Lecture 3 (task states). Rule "keep the section short" in "Rules for the lock statement".

## The lock statement in C#   [S8]
**What:** The `lock` statement writes a critical section. It needs one lock object, common to all tasks. For a lock object of type `object`, the compiler translates the lock statement to `Monitor.Enter` and `Monitor.Exit` (source [1]).
**Why:** The `finally` block releases the lock object even when the code in the section throws an exception.
**Conclusion:** The lock statement is a short form of `Monitor.Enter` and `Monitor.Exit` when the lock object is an `object`. You must not write `await` inside the lock statement [1]. From .NET 9 and C# 13 the recommended lock object is a special object of the Lock type (`System.Threading.Lock`): `var brava = new Lock();` [1]. For net8.0 use `object`.
(Inferred, not in the source: for a lock object of the Lock type, the C# 13 compiler uses `Lock.EnterScope()` and `Dispose()`, not `Monitor`. The slide shows only the `Monitor` expansion. The source does not say this. See Open points.)
**Example:** You write:
```csharp
var brava = new object(); // jedna
...
lock (brava)
{
    brojac++;   // kritični odsječak
}
```
The compiler makes this [1]:
```csharp
bool uzeto = false;
try
{
    Monitor.Enter(brava, ref uzeto);
    brojac++;
}
finally
{
    if (uzeto) Monitor.Exit(brava);
}
```
**Link:** Monitor is treated in detail in week 7. The Lock type is slower than `object` in the measurement (see "Cost of synchronization").

## Fix with the lock statement   [S9]
**What:** Put each increment inside `lock (brava)`.
**Why:** Only one task changes the counter at a time, so the three steps never interleave. The result is correct. But 12 tasks now run one by one, and they also compete all the time for the lock object.
**Conclusion:** Correct, but slow: about 440 ms. The same work run sequentially in one task takes 5 ms.
**Example:**
```csharp
for (int i = 0; i < 1_000_000; i++)
{
    lock (brava)
    {
        brojac++;
    }
}
```
Output (k = 12, expected 12.000.000):

| Attempt | Result | Time |
| --- | --- | --- |
| 1 | 12.000.000 | 427 ms |
| 2 | 12.000.000 | 443 ms |
| 3 | 12.000.000 | 449 ms |

**Link:** Demo: DataRaceDemo, Primjer2_Lock. Exercise: Zadatak1_Brojac, step 2. Cost comparison in "Cost of synchronization" (slide 13 gives 456 ms for the best of 3 measurements. It is a different figure from the runs on slide 9. The source does not say why. See Open points).

## Rules for the lock statement   [S10]
**What and Why:**

| Rule | Reason |
| --- | --- |
| Use a special private lock object only for the lock statement (`object`; from .NET 9 the Lock type). | Other code cannot lock it [1]. |
| Do not use `lock (this)`, `lock (typeof(X))`, `lock ("tekst")`. | Other code can see them. The same text is often the same object [1], [2]. In the notes: you would wait for the code of another party. |
| Use the same lock object for the same datum, for reading and for writing. | A lock object protects only against code that locks the same object (see "Trap: each task its own lock"). |
| Do as little work as possible in the critical section. | The other tasks wait for that time [1]. |
| Inside the lock statement, do not wait for other tasks or for another lock object. | This is how a deadlock starts (see "New trap: deadlock"). `await` is not even allowed [1]. |

**Conclusion:** One datum, one private lock object, one short critical section.
**Link:** Rule 3 leads to the trap in the next section. Rule 5 leads to the deadlock section.

## Trap: each task its own lock object   [S11]
**What:** The lock object is created inside the task. Each task then has its own lock object.
**Why:** A lock object protects only against code that locks the same object. Here each task locks its own object, so nobody waits for anybody.
**Conclusion:** The lock statement exists and the result is still wrong. Fix: create the lock object once, before the tasks, and all tasks lock it.
**Example:**
```csharp
zadaci[j] = Task.Run(() =>
{
    var brava = new object(); // svoja!
    for (int i = 0; i < 1_000_000; i++)
        lock (brava) { brojac++; }
});
```
Output (expected 12.000.000):

| Variant | Result |
| --- | --- |
| Each task its own lock object | 1.981.047 |
| One shared lock object | 12.000.000 |

**Pitfall:** The same error in another form: one part of the code locks, and another part reads or writes the same datum without the lock statement.
**Link:** Demo: DataRaceDemo, Primjer5_RazliciteBrave. Exercise: Zadatak1_Brojac, step 2 (show the trap) and brojac.html, mode "each task its own lock object" (the page uses "svaki task svoja brava").

## Interlocked: atomic operations   [S12]
**What:** `Interlocked` makes one operation on one variable as one indivisible (atomic) step [3].

| Method | What it does, as one indivisible step |
| --- | --- |
| Increment, Decrement | increase or decrease by 1 |
| Add | add a number |
| Exchange | write the new value, return the old value |
| CompareExchange | write the new value only if the current value equals the expected value (see "Fix: check and change together") |
| Read | read a `long` (64 bits) in one step |

**Why:** `Interlocked.Increment` loads, increases and writes in one indivisible step. The processor itself does this.
**Conclusion:** The result is correct. The time is about 220 ms, about two times faster than the lock statement (203 against 456 ms, slide 13). The limit: one operation on one variable. If you need several steps together, Interlocked is not enough.
**Example:**
```csharp
for (int i = 0; i < 1_000_000; i++)
    Interlocked.Increment(ref brojac);
```
Output (expected 12.000.000):

| Attempt | Result | Time |
| --- | --- | --- |
| 1 | 12.000.000 | 223 ms |
| 2 | 12.000.000 | 225 ms |
| 3 | 12.000.000 | 215 ms |

**Link:** Demo: DataRaceDemo, Primjer3_Interlocked. Exercise: Zadatak1_Brojac, step 3. The limit leads to "Trap: check, then change".

## Cost of synchronization   [S13]
**What:** The chart shows all methods on the same job: 12 tasks, 12.000.000 increments in total, best of 3 measurements, i7-9750H, .NET 10, Release. The axis is logarithmic.

| Method | ms |
| --- | --- |
| sequential, one task | 5.0 |
| no protection (data race) | 12.0 |
| lock statement, `object` lock object, per increment | 456.0 |
| lock statement, Lock type, per increment | 711.0 |
| Interlocked per increment | 203.0 |
| local, then one Interlocked.Add | 1.0 |
| Parallel.For with local state | 8.0 |

**Why:**
- Synchronization at each increment is 40 to 140 times slower than sequential. The tasks run one by one and compete for the same lock object or the same memory location.
- The unprotected version (12 ms) is slower than sequential (5 ms) and wrong. All cores write to the same memory location.
- The slide says: synchronization once per task is faster than sequential. This is true for "local, then one Interlocked.Add" (1 ms against 5 ms). It is not true for `Parallel.For` with local state: 8 ms is slower than 5 ms (see Open points).

**Conclusion:** Synchronization has a high cost when you do it at each step. Synchronization once per task is cheap: 1 ms and 8 ms against 203 to 711 ms.
**Pitfall:** The new Lock type (.NET 9) was slower than `object` here (711 against 456 ms), although Microsoft recommends it [1]. Measure on your own computer.
**Link:** Demo: DataRaceDemo, Primjer4_Cijena (about 10 s). Next section gives the fix. Exercise: Zadatak1_Brojac, step 4 (compare the times).

## Fix: synchronize less often   [S14]
**What:** Count in a local variable. Synchronize once, at the end.
**Why:** This is one synchronization per task, not per increment: 1 ms and 8 ms instead of 203 to 711 ms (slide 13). It is isolation (lecture 2) and agglomeration from the Foster method (lecture 3).
**Conclusion:** Synchronize only when you merge the results.
**Example:** With tasks (local counter, then one Add):
```csharp
Task.Run(() =>
{
    long lokalno = 0;
    for (int i = 0; i < 1_000_000; i++)
        lokalno++;
    Interlocked.Add(ref brojac, lokalno);
});
```
With `Parallel.For` and local state [5]:
```csharp
Parallel.For(0L, 12_000_000,
    () => 0L,                  // početak
    (i, stanje, lokalno) =>
        lokalno + 1,           // tijelo
    lokalno => Interlocked.Add(
        ref brojac, lokalno)); // kraj
```
The notes explain the three lambdas: the first sets the local state to zero. The second is the loop body. The third runs once per task and adds the local result.
**Link:** Demo: DataRaceDemo, Primjer4_Cijena, rows "lokalno" and "Parallel.For". Exercise: Zadatak1_Brojac, step 4. Lecture 2 (isolation), lecture 3 (Foster).

## Trap: check, then change   [S15]
**What:** An account has 10.000 EUR. 12 tasks each pay out 1 EUR while there is money. Each step is atomic (Interlocked).
**Why:** Between the check and the change, another task pays out the last euro. Each step is protected, but the whole of two steps is not. The state goes below zero although there is no data race.
**Conclusion:** This is a race condition without a data race (see "Data race and race condition").
**Example:**
```csharp
if (Interlocked.Read(ref stanje) >= 1)  // provjeri
{
    Interlocked.Decrement(ref stanje);  // promijeni
    Interlocked.Increment(ref isplata);
}
```
Output:

| Run | stanje | isplata |
| --- | --- | --- |
| 1 | -5 | 10.005 |
| 2 | -4 | 10.004 |
| 3 | -5 | 10.005 |

**Pitfall:** The same pattern appears in other forms: "if the key is not in the dictionary, add it" and "if the place is free, reserve it".
**Link:** Demo: DataRaceDemo, Primjer6_ProvjeriPaPromijeni. Exercise: Zadatak2_Racun. Visualization racun_stog.html, section 1.

## Fix: check and change together   [S16]
**What:** Two fixes. (1) The lock statement around both steps. (2) `CompareExchange` in a loop. `CompareExchange(ref x, novo, staro)` writes `novo` only if `x` is still equal to `staro`, and returns the value that it found [3].
**Why:** With the lock statement, the check and the payment are in the same critical section. With CompareExchange, we read the old state and write "old minus one" only if the state is still the old state. If another task paid out meanwhile, CompareExchange writes nothing and we try again.
**Conclusion:** The lock statement is simpler and also works when several variables change together. CompareExchange needs no lock object but needs a retry loop.
**Example:** Lock statement:
```csharp
lock (brava)
{
    if (stanje >= 1)
    { stanje--; isplata++; }
}
```
CompareExchange:
```csharp
while (true)
{
    long staro = Interlocked.Read(ref stanje);
    if (staro < 1) break;   // nema novca
    long zatekao = Interlocked.CompareExchange(
        ref stanje, staro - 1, staro);
    if (zatekao == staro)
    {
        Interlocked.Increment(ref isplata);
        break;
    }
}   // inače netko je bio brži: ponovi
```
Output: lock statement around both steps: stanje 0, isplata 10.000. CompareExchange: stanje 0, isplata 10.000.
**Link:** Demo: DataRaceDemo, Primjer6_ProvjeriPaPromijeni, last two rows. The same retry pattern fixes the stack (see "Fix without lock: CompareExchange"). Exercise: Zadatak2_Racun, steps 2 and 3.

## Trap: List<T> from many tasks   [S17]
**What:** 12 tasks each call `lista.Add(i)` 100.000 times on the same list. The expected count is 1.200.000.
**Why:** `Add` reads the element count, writes the element and increases the count. These are three steps, like `brojac++`. The documentation says: reading at the same time is safe, but for writing you must do the synchronization yourself [6]. The same holds for Dictionary, Queue, Stack and the other collections in System.Collections.Generic.
**Conclusion:** Ordinary collections are not safe for writing from many tasks. Three fixes: the lock statement around `Add`; each task its own list, merged after the wait (isolation); or concurrent collections (week 7).
**Example:** Output (expected 1.200.000):

| Variant | Count |
| --- | --- |
| Shared list (1) | 159.341 |
| Shared list (2) | 145.054 |
| Shared list (3) | 140.817 |
| lock statement around Add | 1.200.000 |
| Each task its own list | 1.200.000 |

**Pitfall:** In one run on the same day, the first attempt did not lose elements. It threw `ArgumentException`. In the notes: about 150 thousand elements remain, and sometimes `Add` throws an exception.
**Link:** Demo: DataRaceDemo, Primjer7_Lista. Week 7 (concurrent collections). The next section shows the same loss in a linked structure.

## How an element is lost: stack as a linked list   [S18]
**What:** `Push(v)` on a stack as a linked list has two steps: (1) `novi.Sljedeci = vrh;` (2) `vrh = novi;`.
**Why:** Both tasks read the same top (`vrh`). The second write to `vrh` overwrites the first.
**Conclusion:** Node 4 points to 2, but nothing points to node 4. It is lost.
**Example:** The stack before: vrh -> 2 -> 1. Task A pushes 4. Task B pushes 5.

| Step | Task A: Push(4) | Task B: Push(5) |
| --- | --- | --- |
| 1 | 4.Sljedeci = vrh (2) | |
| 2 | | 5.Sljedeci = vrh (2) |
| 3 | vrh = 4 | |
| 4 | | vrh = 5 |

After step 4: vrh -> 5 -> 2 -> 1. Node 4 points to 2, and nothing points to 4.
**Pitfall:** The order (1) then (2) matters even without tasks. The source says that the reverse order would lose the rest of the list. (Inferred reason: with one task, `vrh = novi;` first and `novi.Sljedeci = vrh;` second make the new node point to itself, so the old list is lost.) `Stack<T>` in .NET stores elements in an array, not in a list [7], but it is also not safe for writing from many tasks.
**Link:** Visualization racun_stog.html, section 2. Exercise: Zadatak3_Stog. Fix in the next section.

## Fix without lock: CompareExchange   [S19]
**What:** Do step (2) with `CompareExchange`: `vrh` becomes the new node only if `vrh` is still the value read in step (1).
**Why:** If `vrh` changed, another task was faster. The loop then retries.
**Conclusion:** `ConcurrentStack<T>` works this way: Push with CompareExchange, without a lock object [7]. About 85 percent of the nodes are lost without protection. With CompareExchange none is lost.
**Example:**
```csharp
var novi = new Cvor(v);
do
{
    novi.Sljedeci = vrh;
} while (Interlocked.CompareExchange(
         ref vrh, novi, novi.Sljedeci)
         != novi.Sljedeci);
```
Output (12 tasks, 100.000 each, expected 1.200.000):

| Variant | Nodes | Lost |
| --- | --- | --- |
| No protection (1) | 188.117 | 1.011.883 |
| No protection (2) | 166.067 | 1.033.933 |
| No protection (3) | 184.621 | 1.015.379 |
| CompareExchange | 1.200.000 | 0 |

**Link:** Demo: DataRaceDemo, Primjer8_Stog. Exercise: Zadatak3_Stog, step 3. Source [7]: Stack.cs and ConcurrentStack.cs in dotnet/runtime. Same pattern as the account fix.

## New trap: deadlock   [S20]
**What:** A deadlock (zastoj) is a state in which tasks wait for each other forever. Task 1 locks A and asks for B. Task 2 locks B and asks for A. The drawing (checked in the .pptx shapes): task 1 is top left, lock object A top right, lock object B bottom left, task 2 bottom right. The arrow from A to task 1 has the label "drži" (holds). The arrow from task 1 down to B has the label "čeka" (waits). The arrow from B to task 2 has the label "drži". The arrow from task 2 up to A has the label "čeka". The four arrows make a cycle: task 1 holds A and waits for B, and task 2 holds B and waits for A. All four arrows have arrowheads in the .pptx XML. Four necessary conditions (Coffman et al., source [8]):
1. mutual exclusion (lock statement);
2. hold and wait: a task holds one lock object and waits for another;
3. a lock object cannot be taken away (no preemption);
4. circular wait: 1 waits for 2, 2 waits for 1.

**Why:** A deadlock occurs only when all four conditions hold. It is enough to break one. The easiest to break is circular wait.
**Conclusion:** Always take the lock objects in the same order (A, then B). Then circular wait cannot occur. In the demo, both tasks then pass in 201 ms.
**Example:** Output (task 1: A then B, task 2: B then A):
```
  task 1: drži A, 2 s čeka B: zastoj
  task 2: drži B, 2 s čeka A: zastoj
  (2106 ms)
```
The program waits 2 s only so that it can finish. With an ordinary lock statement it would stop forever. With the same order: "task 2: drži A, dobio i B (201 ms)".
**Link:** Demo: DataRaceDemo, Primjer9_Zastoj (about 2,5 s). Week 7: deadlock, semaphores, Monitor, starvation (izgladnjivanje), concurrent collections. Rule 5 in "Rules for the lock statement".
**Pitfall:** The lock statement itself creates this trap.

## What to choose   [S21]
**What:**

| Situation | Fix | Example |
| --- | --- | --- |
| The work is divided and the results are merged at the end | isolation: local variables, Parallel.For with local state | sum, pi, counter |
| The datum is only read | nothing: immutability | input array |
| One variable, one operation | Interlocked | counter, flag |
| Several steps or several variables together | lock statement (or CompareExchange in a loop) | check then change; account |
| Collection | lock statement, local collections, concurrent collections (week 7) | list of results |

**Conclusion:** Order of thinking: first try not to share a mutable datum. Synchronize only what you must, and as rarely as possible.
**Link:** Fixes are defined in "Four fixes".

## Summary: traps and fixes   [S22]
**What:**

| Trap | How you see it | Fix |
| --- | --- | --- |
| Data race (`brojac++`) | result too small and different each time | isolation, Interlocked, lock statement |
| Each task its own lock object | lock statement exists, result wrong | one shared lock object |
| Synchronization in each iteration | correct, but slower than sequential | local, then once per task |
| Check, then change | state below zero although each step is atomic | lock statement around both steps or CompareExchange |
| Collection from many tasks | elements vanish or an exception | lock statement, local collections |
| Lock objects in different order | program stops (deadlock) | always the same order |

**Link:** Each row has a section above.

## References   [S23]
**What:** The lecture cites these sources.
- [1] Microsoft Learn: The lock statement (C# reference).
- [2] Microsoft Learn: Managed threading best practices.
- [3] Microsoft Learn: Interlocked Class (System.Threading).
- [4] J. Regehr: Race Condition vs. Data Race. Embedded in Academia, 13. 3. 2011. blog.regehr.org/archives/490
- [5] Microsoft Learn: How to: Write a Parallel.For loop with thread-local variables.
- [6] Microsoft Learn: List<T> Class, section Thread Safety.
- [7] .NET runtime source code: Stack.cs and ConcurrentStack.cs (github.com/dotnet/runtime, src/libraries).
- [8] E. G. Coffman, M. J. Elphick, A. Shoshani: System Deadlocks. ACM Computing Surveys 3(2), 1971, 67-78.
- [9] T. Dadić: Data race condition, lecture, 2022/23.

The notes say the sources are the Microsoft documentation, the Regehr text, the .NET source code, the Coffman paper and the lecture of dr. sc. Tonći Dadić. Next week: computer models, RAM and PRAM.

## Exercise cross-reference
Exercise folder: `/Users/drazenbaric/Projects/parallel-programming/exercises/03-data-race/` (README.md, Vjezba3_upute.html, Vjezba3.sln, intro deck "Data race, lock i Interlocked 2026-27.pptx", Vizualizacije/index.html, brojac.html, racun_stog.html). The three C# projects contain the solutions as comments and code. Measure times in Release (`dotnet run -c Release`).

| Task and file | Concept | Key insight of the solution | What goes wrong if you ignore it |
| --- | --- | --- | --- |
| Zadatak 1: `Zadatak1_Brojac/Zadatak1_Brojac.cs`. 12 tasks, 1.000.000 increments each. | Data race, lock, wrong lock, Interlocked, local count then Add | Five variants run in turn. No protection: less than 12.000.000, different each run, because `brojac++` is three steps. Lock statement with one shared lock object created before the tasks: 12.000.000, about 155 ms against about 5 ms sequential. Lock object per task: for example 3.195.136, because nobody waits. Interlocked.Increment: 12.000.000, about 246 ms. Local count plus one `Interlocked.Add`: 12.000.000, about 6 ms, the fastest. | Lost increments. A lock object created inside the task does not protect. Synchronizing at each step is slower than sequential. |
| Zadatak 2: `Zadatak2_Racun/Zadatak2_Racun.cs`. Account 10.000, 12 tasks, each tries 2 * 10.000 times. | Race condition without data race. Lock around both steps. CompareExchange loop. | Check and change must be one whole. First variant mostly gives a state below zero (for example -2) and isplata over 10.000. Sometimes you do not see it: run several times. The lock statement and CompareExchange give stanje 0, isplata 10.000. | The account goes below zero although each step is atomic. Protecting each step is not enough. |
| Zadatak 3: `Zadatak3_Stog/Zadatak3_Stog.cs`. 12 tasks push 100.000 nodes each. | Lost node in a linked list. Lock. CompareExchange Push. | `PushBezZastite` loses nodes (for example 188.117 nodes, about 1.000.000 lost). `PushLock` and `PushCas` give 1.200.000 and 0 lost. CompareExchange succeeds only if `vrh` is still the value from step (1). | Two tasks read the same top. The second write overwrites the first, and nodes vanish. |

Visualizations (Vizualizacije/):
- `brojac.html`: two tasks, each runs `brojac++` twice (expected 4). Four modes: no protection, one shared lock object, each task its own lock object, Interlocked.Increment. Part 1 is step by step. Part 2 counts all possible orders. The count is a number of orders, not a probability. Slides 4 and 7-12.
- `racun_stog.html`: account with 1 EUR and two tasks; stack with Push(4) and Push(5). Modes: no protection, lock statement, CompareExchange. Slides 5, 15-16, 18-19.
- Questions in the visualizations (for self-test): Why does one lock object remove the loss, although load, increase and write stay separate? Why is a lock object per task no better than no protection? What is the minimum and maximum counter value with three increments per task? Why is "local, then one Add" the fastest? Why is Interlocked on each step not enough for the account? Can a CompareExchange loop repeat forever, and what must hold for that? Why does the order (1) then (2) in Push matter with one task? When do you choose the lock statement and when CompareExchange?
- `index.html` maps slides to views. Slides 13-14 (cost), 17 (List<T>) and 20 (deadlock) have no view on purpose. It says that exercise 4 is not yet made.

## Open points
- Slides 7 and 20: I read the shape positions and labels in the .pptx. The arrow directions on slide 20 come from the arrowheads in the .pptx XML. The three entry lines on slide 7 have no arrowheads.
- Slide 12 says Interlocked is about two times faster than lock. The starter code of Zadatak1_Brojac says that on that computer Interlocked (about 246 ms) is not faster than lock (about 155 ms). The measurements differ from the lecture (456 ms, 203 ms). The source does not explain the difference. It says: measure on your own computer.
- Slide 13: the notes say the unprotected version (12 ms) is slower than sequential because all cores write to one location. The source gives no direct measurement of the cause.
- Slide 9 uses "oko 440 ms" (three runs: 427, 443, 449). Slide 13 uses 456 ms ("najbolje od 3 mjerenja"). Both are in the source. The source does not say why 456 is above all three runs.
- Slide 18: the source states that the reverse order loses the rest of the list. The reason (the new node points to itself) is inferred here.
- The intro deck in the exercise folder (`Data race, lock i Interlocked 2026-27.pptx`) repeats the 23 lecture slides and notes (checked by a judge). It adds nothing new.
- Slide 8: the source says the lock statement is a short form of `Monitor.Enter` and `Monitor.Exit`. It also recommends the Lock type. For the Lock type the compiler does not use `Monitor` (inferred, from general .NET knowledge, not from the source).
- Slide 13: the slide says that synchronization once per task is faster than sequential. The chart shows `Parallel.For` with local state at 8 ms, slower than sequential at 5 ms. Only the 1 ms row is faster. The source does not explain this.
- Slide 3: the slide says the error would be "rare" on one core. The notes say the result would "almost always be correct". Both are in the source.
- brojac.html also cites slides 13-14 and 4, 7-14 as sources. Its question about "local, then one Add" uses slides 13-14.

## Coverage ledger
| Slide | Topic section | Items |
| --- | --- | --- |
| 1 | Context from earlier lectures | 4 (title, authors, academic year, notes: shared data question) |
| 2 | Context from earlier lectures | 6 (pi 0,30; isolation; Bernstein; three parts; question; notes) |
| 3 | Trap: data race on a counter | 9 (code, 3 results, 78-85 %, compiler silent, one core, 12.000.000, link to pi, demo) |
| 4 | Why a data race happens | 6 (three steps, table, stale value, scheduler, nondeterminism, demo) |
| 5 | Data race and race condition | 7 (two definitions, two examples, one without other, Bernstein, notes) |
| 6 | Four fixes | 6 (four fixes, cost note, order of preference) |
| 7 | Critical section | 7 (two definitions, drawing with three entry lines, one exit arrow and its label, waiting cost, short section) |
| 8 | The lock statement in C# | 8 (code, Monitor code, finally, await, Lock type, net8.0, notes, inferred Lock-type note) |
| 9 | Fix with the lock statement | 6 (code, 3 results, slow, 5 ms, reason, demo) |
| 10 | Rules for the lock statement | 5 (five rules with reasons) |
| 11 | Trap: each task its own lock object | 6 (code, 2 results, reason, fix, same error other form, demo) |
| 12 | Interlocked: atomic operations | 8 (code, 5 methods, 3 results, 2x faster, limit, demo) |
| 13 | Cost of synchronization | 10 (7 chart values, 40-140x, Lock slower, once-per-task claim with 8 ms caveat, setup, demo) |
| 14 | Fix: synchronize less often | 6 (two code blocks, 3 lambdas, 1 ms and 8 ms, isolation, Foster, demo) |
| 15 | Trap: check, then change | 6 (setup, code, 3 results, reason, race condition, other patterns) |
| 16 | Fix: check and change together | 6 (lock code, CAS code, CAS definition, results, reasoning, demo) |
| 17 | Trap: List<T> from many tasks | 8 (setup, reason, docs, other collections, 3 fixes, 5 results, ArgumentException, demo) |
| 18 | How an element is lost | 6 (Push steps, table, lost node, order, Stack<T> array, notes) |
| 19 | Fix without lock: CompareExchange | 5 (code, reason, ConcurrentStack, 4 results, 85 %) |
| 20 | New trap: deadlock | 8 (drawing cycle, four conditions, fix by order, outputs, 201 ms, 2106 ms, week 7, demo) |
| 21 | What to choose | 6 (five rows, order of thinking) |
| 22 | Summary: traps and fixes | 6 (six rows) |
| 23 | References | 10 (nine references, closing note) |
