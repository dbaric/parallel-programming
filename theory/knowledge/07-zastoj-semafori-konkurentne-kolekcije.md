# Lecture 7: Deadlock, semaphores and concurrent collections (source: 07-zastoj-semafori-konkurentne-kolekcije, 27 slides)

## Map
- This lecture covers deadlock (zastoj), semaphores (semafori), waiting for a condition (Monitor.Wait and Pulse) and concurrent collections (konkurentne kolekcije).
- The main idea: each tool in this lecture has a trap, and each trap has a fix. The lecture shows the error first and the repair second.
- Deadlock topics (S3-S11) connect to lecture 4 (lock order). Starvation (S12-S13) is the third problem after deadlock and livelock.
- Semaphore topics (S14-S16) give the "at most K" tool. The same semaphore is the repair on S9.
- Wait/Pulse (S17-S19) and BlockingCollection (S20-S22) solve the producer and consumer problem. S23-S25 summarise tools, traps and the first exam.
- Source numbers [1]-[11] refer to the references on S27.

## Recap: the deadlock pattern   [S1-S3]
**What:** S1: "Paralelno programiranje", lecture 7. The author is Marin Aglić Čuvić, based on the lectures of Tonći Dadić. The academic year is 2026/2027.
- The lecture is the seventh and the last before the first exam.
- S2 recalls earlier work: lecture 4 (lock, Interlocked, deadlock in short, slide 20), lecture 5 (barrier, ograda) and lecture 6 (prefix sum).
- S2 lists the plan: deadlock in detail (conditions, philosophers, fixes, livelock, starvation).
- The plan continues with synchronisation (semaphore, waiting for a condition with Monitor.Wait and Pulse) and collections (concurrent collections and BlockingCollection).
- At the end the lecture repeats the traps of weeks 1-7.
**Why (notes S1-S2):** Almost every topic today is a trap. First the lecture shows the error, then the repair. The lecture also shows what happens when the student prevents deadlock in a wrong way (livelock, S10).
**Example:** (S3, lecture 4 slide 20)
```csharp
lock (A) { … lock (B) { … } }   // task 1
lock (B) { … lock (A) { … } }   // task 2
```
Task 1 holds A and waits for B. Task 2 holds B and waits for A. The diagram on S3 uses a solid line for "holds" and a dashed line for "waits". Its caption says: task 1 holds A and waits for B; task 2 holds B and waits for A: deadlock. Fix: the same locks in the same order. The diagram has only text labels (brava A, brava B), checked in the .pptx.
**Why:** Each task holds what the other task needs, and nobody interrupts them. The program throws no exception and uses no processor time. It only stands still (notes S3).
**Conclusion:** Fix: take the same locks in the same order. The lecture explains why this works.
**Link:** Lecture 4 (lock, Interlocked, deadlock in short). Demos named in the notes: DataRaceDemo (week 4) and Primjer9_Zastoj.

## Four necessary conditions of deadlock (Coffman)   [S4]
**What:** Deadlock occurs only when all four conditions hold at the same time [2] (Coffman, Elphick, Shoshani, 1971).

| condition | meaning | in C# | on the S3 example |
| --- | --- | --- | --- |
| mutual exclusion (međusobno isključivanje) | only one task holds the resource | lock, Monitor | task 2 cannot enter lock (A) while task 1 holds A |
| hold and wait (drži i čeka) | a task holds one resource while it waits for another | lock inside lock | task 1 holds A (outer lock) while it waits for B (inner lock) |
| no preemption (nema oduzimanja) | nobody can take the resource away | a lock is never taken away | A is released only when task 1 leaves the block; the lock waits without a time limit |
| circular wait (kružno čekanje) | a chain where each task waits for the next one | A waits for B, B waits for A | task 1 waits for B from task 2, task 2 waits for A from task 1 |

**Why:** In the S3 code the first three conditions always hold. The circle forms only if task 1 takes A and task 2 takes B before either task takes its second lock. For this reason the deadlock does not appear in every run (notes S4).
**Conclusion:** It is enough to remove one condition. In the slide classification, each fix in this lecture removes exactly one condition. The inferred note in the S11 section of this file adds an extra: "return and try again" also removes hold and wait.
**Link:** S7 (removal of each condition), S11 (table of fixes).

## Dining philosophers   [S5]
**What:** Five philosophers and five forks sit at a round table. Philosopher Fi eats only with the fork on the left (vi) and the fork on the right (vi+1). While a philosopher eats, he holds both forks. Then he returns them and thinks. In C#: philosopher = task, fork = lock. Reference [3]. The notes name Dijkstra as the author of the classic problem.
**Why:** It is the simplest example of circular wait.
**Example:** Question to the student before S6: what happens if all philosophers take the left fork at the same time? The diagram shows F0-F4, forks v0-v4 and the table.
**Link:** Demo in notes: Vizualizacije/filozofi.html, section 1.

## Trap: everybody takes the left fork   [S6]
**What:** Code:
```csharp
lock (vilica[i])                 // lijeva
    lock (vilica[(i + 1) % 5])   // desna
        Jedi();
```
**Why:** All philosophers hold the left fork (orange in the picture) and wait for the right fork, which the neighbour holds. This is circular wait and all four conditions hold.
**Example:** Five attempts. Main waits 3 s and then gives up.

| attempt | after | meals per philosopher (of 100,000) |
| --- | --- | --- |
| 1 | 3031 ms | 357 245 71 9 0 |
| 2 | 3004 ms | 9 10 6 8 0 |
| 3 | 3002 ms | 9 6 6 7 0 |
| 4 | 3002 ms | 8 8 12 10 0 |
| 5 | 3004 ms | 5 9 5 6 0 |

Deadlock occurred in 5 of 5 attempts. Some philosophers eat a few meals before the circle closes, and one philosopher eats none.
**Pitfall:** In a real program Main would not give up. It would wait forever (notes S6).
**Link:** Demos: ZastojDemo, Primjer1_Filozofi; filozofi.html section 1 ("svi uzmu lijevu").

## Prevention: remove one condition   [S7]
**What:**

| condition | how to remove it [2] | in C# |
| --- | --- | --- |
| mutual exclusion | not possible for all resources | do not share mutable data (isolation) |
| hold and wait | request all resources at once | one lock for both |
| no preemption | if you do not get the second resource, return what you hold | Monitor.TryEnter, then Exit |
| circular wait | all request in the same order | first the fork with the lower number |

**Why:** A fork is really held by one philosopher, so mutual exclusion stays. You can only avoid sharing, as in lecture 4 (notes S7).
**Conclusion:** Two of the next three fixes work. The third one adds a new trap (livelock).
**Link:** S8, S9, S10.

## Fix 1: fork order   [S8]
**What:**
```csharp
int prva = Math.Min(lijeva, desna);
int druga = Math.Max(lijeva, desna);
lock (vilica[prva]) lock (vilica[druga]) Jedi();
```
**Why:** Forks have numbers and each philosopher takes the lower number first. For four philosophers nothing changes. Philosopher 4 now asks v0 first and then v4, not v4 then v0. All philosophers ask in the same order (Coffman condition: circular wait), so the circle cannot close [2].
**Conclusion:** It is the same rule as in lecture 4: always take locks in the same order.
**Example:** Attempt 1: finished in 11798 ms, meals 100000 × 5. Attempt 2: finished in 11957 ms, meals 100000 × 5. All 100,000 meals complete.
**Link:** Demo: ZastojDemo, Primjer2_Redoslijed; filozofi.html section 1 ("redoslijed").

## Fix 2: at most four at the table (waiter)   [S9]
**What:**
```csharp
var konobar = new SemaphoreSlim(4);
konobar.Wait();
try { lock (lijeva) lock (desna) Jedi(); }
finally { konobar.Release(); }
```
The semaphore (S14) lets at most 4 philosophers sit at the table. The fifth philosopher waits until a philosopher leaves the table. Reference [11].
**Why:** Suppose all four seated philosophers take the left fork. The last one also has a free right fork, because the fifth philosopher is not at the table. The last philosopher eats and releases his forks. The circle does not close.
**Example:** Attempt 1: finished in 12294 ms. Attempt 2: finished in 12404 ms. The time is similar to the order fix.
**Link:** Demo: ZastojDemo, Primjer3_Konobar; filozofi.html section 1 ("konobar"). Semaphore: S14-S16.

## Livelock: "return and try again"   [S10]
**What:** Livelock (živo blokiranje): tasks run and use the processor, but nobody makes progress [4].
```csharp
Monitor.Enter(lijeva);
if (Monitor.TryEnter(desna)) { Jedi(); … }
else Monitor.Exit(lijeva);   // pokušaj ponovno
```
**Why:** No deadlock occurs, because nobody holds a fork while he waits (the classification of this fix is in S11). If all work in the same rhythm, this cycle repeats:
1. All take the left fork.
2. Nobody gets the right fork.
3. All return the left fork.
4. The cycle starts again.

Nobody waits and nobody progresses. The experiment forced the same rhythm with a barrier (ograda). Without the barrier, the result depends on the rhythm (notes S10).
**Example:** Same code, 5 philosophers, 2 s:

| case | meals | attempts |
| --- | --- | --- |
| in the same rhythm | 0 | 2.9-3.8 million |
| no barrier, fork held longer | 6,848-10,937 | about 930,000 |
| no barrier, fork held shortly | 4.2-4.8 million | 14.0-15.3 million |

Progress can also be slow (middle row) or normal.
**Conclusion:** "Return and try again" is not a fix. The fix is the order (S8).
**Link:** Demo: ZastojDemo, Primjer4_ZivoBlokiranje; filozofi.html section 1 ("vrati pa pokušaj", "u istom ritmu").

## Fixes and Coffman conditions   [S11]
**What:**

| fix | removes condition | reason |
| --- | --- | --- |
| order (S8) | circular wait | All ask for forks in the same order. A circle needs a philosopher who holds a higher fork and waits for a lower fork. Such a philosopher does not exist [2]. |
| waiter (S9) | circular wait | Fi waits only for the fork that neighbour Fi+1 holds. A circle around the table needs all five philosophers. At most four sit at the table [11]. |
| "return and try again" (S10) | no preemption | Who does not get the right fork returns the left fork himself [2]. No deadlock occurs. Progress is not guaranteed: livelock. |

**Note (inferred):** The slides class "return and try again" as removal of "no preemption". Monitor.TryEnter does not wait, so the philosopher also does not wait while he holds a fork. This is also the removal of "hold and wait" (S7 lists TryEnter for "no preemption"). Nobody takes the fork away: the philosopher returns it voluntarily.
**Why:** In the order fix and the waiter fix the other three conditions still hold: one philosopher holds a fork, and he holds one fork while he waits for the second (notes S11). Deadlock needs all four [2].
**Example:** Check in filozofi.html, "Sva stanja" (all states): default 82 states and 1 deadlock; order 70 states and 0 deadlocks; waiter 491 states and 0 deadlocks. Both fixes give 0 deadlocks.
**Link:** S4 (conditions), S7 (prevention).

## Starvation   [S12]
**What:** Deadlock: nobody progresses. Starvation (izgladnjivanje): others progress and one task never gets the resource [3], [4].
- Deadlock-freedom (sloboda od zastoja): if somebody wants to enter, somebody will enter [3].
- Starvation-freedom (sloboda od izgladnjivanja): everybody who wants to enter enters at some time [3].
**Why:** Starvation-freedom implies deadlock-freedom; the reverse does not hold [3]. The usual cause is a "greedy" task that always takes the same resource while the others wait [4].
**Conclusion:** Starvation-freedom is the stronger guarantee.
**Link:** Herlihy and Shavit [3]; livelock S10.

## Is lock fair? Measured   [S13]
**What:** Microsoft gives no order guarantee for lock, so the lecture measured it. 12 tasks, 1 s, the same lock.

| case | fewest entries | most | ratio |
| --- | --- | --- | --- |
| no work outside the lock, 1st run | 23,007 | 26,002 | 1.1 |
| no work outside the lock, 2nd run | 22,979 | 26,587 | 1.2 |
| with work outside the lock | 7,306 | 7,535 | 1.0 |

**Why:** The fewest and the most entries differ by about ten percent (notes). Nobody stayed without an entry.
**Conclusion:** The entries are unequal, but nobody stayed without an entry. Starvation is possible and rare in practice [3]. For a semaphore, Microsoft states explicitly: no guaranteed order (neither FIFO nor LIFO) [5].
**Link:** Demo: ZastojDemo, Primjer5_Postenost.

## Semaphore: at most K at the same time   [S14]
**What:** A semaphore (semafor) generalises the lock: at most K tasks at the same time, not one [3].
```csharp
var s = new SemaphoreSlim(3); // 3 dozvole
s.Wait();    // uzmi dozvolu ili čekaj
… posao …
s.Release(); // vrati dozvolu
```
Wait takes a permit, or waits if none is free. Release returns the permit.
**Example:** Analogy from the archive slides [1]: a shop that lets in a limited number of customers. Experiment: 20 tasks of 50 ms.

| 20 tasks of 50 ms | most at the same time | time |
| --- | --- | --- |
| no semaphore | 12 | 109 ms |
| SemaphoreSlim(3) | 3 | 361 ms |

**Why:** Without the semaphore 12 tasks run, because the thread pool had started 12 threads at that moment. With the semaphore at most 3 run, so the run takes longer.
**Link:** Demo: ZastojDemo, Primjer6_Semafor. Use in S9.

## Trap: Release is skipped   [S15]
**What:**
```csharp
s.Wait();
Posao();     // iznimka u svakom 4. tasku
s.Release(); // tada se ne izvede
```
Rule [6]: call Release as many times as you entered (Wait).
**Why:** An exception in Posao skips Release. The permit is lost forever. After three exceptions all three permits are lost. Then the other tasks wait forever.
**Example:** Release after the work: 7 of 15 finished, 10 of 20 still wait, 0 free permits. Fix:
```csharp
s.Wait();
try { Posao(); } finally { s.Release(); }
```
Result: 15 of 15 finished, 0 of 20 still wait, 3 free permits.
**Conclusion:** Put Release in finally. A lock does this itself; with a semaphore you must write it.
**Link:** Demo: ZastojDemo, Primjer6_Semafor. Same pattern in S9.

## A semaphore is not a lock   [S16]
**What:**

| | lock (Monitor) | SemaphoreSlim |
| --- | --- | --- |
| how many at the same time | 1 | K (as set) |
| who releases | only the thread that holds it | anybody [5] |
| too many releases | exception | the number of permits can grow above K [5] |

**Why:** The notes ask: why not use a semaphore with one permit instead of a lock? The answer: they are different. A lock is released only by the thread that holds it. A semaphore is released by anybody. A wrong extra Release raises the permit count above K, and more tasks enter than the limit allows.
**Conclusion:** Use a semaphore for "at most K". Use a lock to protect data.
**Link:** Lecture 5, slide 28: the thread that took a Monitor lock must release it.

## Waiting for a condition: Wait and PulseAll   [S17]
**What:**
```csharp
// potrošač
lock (brava)
{
    while (red.Count == 0) Monitor.Wait(brava);
    x = red.Dequeue();
}
// producent
lock (brava)
{
    red.Enqueue(x); Monitor.PulseAll(brava);
}
```
- Wait releases the lock and waits. After PulseAll it takes the lock again and only then returns [7].
- Call Wait and Pulse only while you hold the lock. Otherwise an exception occurs [7], [8].
- Pulse wakes one waiting thread. PulseAll wakes all waiting threads [7].
**Why:** Without Wait, the consumer would check the empty queue in a loop and use one core (notes S17).
**Link:** Demo: filozofi.html section 2. Next traps: S18, S19. Ready-made alternative: BlockingCollection (S21).

## Trap: if instead of while   [S18]
**What:**
```csharp
if (red.Count == 0) Monitor.Wait(brava);
x = red.Dequeue();  // red je možda prazan
```
Rule [3]: after wake-up the condition may not hold. Always use while: check the condition again and, if it does not hold, call Wait again.
**Why:** PulseAll wakes all consumers. The first one takes the element. The others wake with an empty queue and call Dequeue.
**Example:** 1 producer, 4 consumers, 200,000 items.

| variant | Dequeue on an empty queue |
| --- | --- |
| if (…) Wait | 26 times |
| while (…) Wait | 0 |

**Link:** Demo: ZastojDemo, Primjer7_WaitPulse; filozofi.html section 2 (if versus while).

## Trap: a pulse is not remembered   [S19]
**What:**
```csharp
lock (o) Monitor.Pulse(o);      // nitko ne čeka
lock (o) Monitor.Wait(o, 1000); // istekne
```
**Why:** If the producer calls Pulse before the consumer calls Wait, nobody waits and the pulse is lost. A consumer that does not check the condition before Wait waits forever. The message exists, but the consumer does not see it. This is a lost wake-up (izgubljeno buđenje, lost wakeup) [3].
**Example:** In the experiment, Pulse before Wait: the Wait expired after 1 s (the pulse was not remembered).
**Conclusion:** Fix: the same while as on S18. Check the condition first, then call Wait.
**Link:** Demo: ZastojDemo, Primjer7_WaitPulse.

## Concurrent collections   [S20]
**What:**

| collection [9] | what it is |
| --- | --- |
| ConcurrentQueue<T> | queue (FIFO) |
| ConcurrentStack<T> | stack (LIFO) |
| ConcurrentBag<T> | unordered collection |
| ConcurrentDictionary<K, V> | dictionary |
| BlockingCollection<T> | waits when empty or full: producer and consumer |

**Why:** These collections synchronise themselves, so you do not write the lock code. Ordinary collections (List, Queue, Dictionary) have no synchronisation: your code must provide it [9].
**Link:** Lecture 4, slide 17 (List loses elements like a counter; from the exercise 3 visualisation index.html).

## BlockingCollection and capacity   [S21]
**What:**
```csharp
var bc = new BlockingCollection<int>(
        boundedCapacity: 100);
// producent
for (…) bc.Add(x);   // čeka dok je puno
bc.CompleteAdding(); // više nema
// potrošači
foreach (var x in bc.GetConsumingEnumerable())
    Obradi(x);
```
- Take waits while the collection is empty. Add waits while it is full [10].
- foreach ends when the collection is empty and CompleteAdding was called [10].
**Why:** Capacity stops a fast producer from running too far ahead of the consumer [10]. Without capacity, a fast producer can fill the memory (notes).
**Example:** Capacity 100, fast producer and slow consumer: at most 100 elements in the collection.
**Link:** Demo: ZastojDemo, Primjer8_BlockingCollection. Manual version: S17.

## Trap: check, then take   [S22]
**What:**
```csharp
while (!bc.IsCompleted)   // još nije gotovo …
    x = bc.Take();        // … a drugi uzme zadnji
```
**Why:** Between the check and Take, another consumer takes the last element. Take then throws InvalidOperationException (Take on a completed collection) [10]. This is the "check, then change" pattern from lecture 4: the check and the action are not one operation.
**Example:** 4 consumers, 20 runs:

| variant | exceptions from Take |
| --- | --- |
| while (!IsCompleted) Take() | 54 times |
| foreach GetConsumingEnumerable | 0 |

**Conclusion:** Use GetConsumingEnumerable. It waits and takes as one operation.
**Link:** Lecture 4 (check, then change); exercise 3 task 2. Demo: Primjer8_BlockingCollection.

## What to choose   [S23]
**What:**

| you need | tool |
| --- | --- |
| one counter | Interlocked |
| several fields together | lock |
| at most K at the same time | SemaphoreSlim |
| wait for a condition | while + Monitor.Wait, or BlockingCollection |
| a shared queue | ConcurrentQueue, BlockingCollection |
| two locks | always in the same order |
| best | do not share: local state, merge at the end |

## Review: traps of weeks 1-7   [S24]
**What:** Each trap has a recognisable symptom and a fix.

| week | trap | fix |
| --- | --- | --- |
| 1-2 | closure in a loop | local copy of the counter |
| 2 | tasks too small, remainder N / k | bigger chunks, the last takes the remainder |
| 3 | sequential part, bad partition | Amdahl, many blocks |
| 4 | data race, check then change, own lock | lock, Interlocked, isolation |
| 5 | foreign write without a barrier, Sleep in async | barrier, await Task.Delay |
| 6 | iteration dependency, reversed operands | Blelloch, blocks |
| 7 | deadlock, livelock, if-Wait, Release | order, semaphore, while, finally |

**Why (symptoms from notes):** closure: wrong indices. Tasks too small: slower than sequential. Data race: wrong sum, different each run. Iteration dependency: wrong prefix sum. Deadlock: the program stands still.
**Example:** In exercise 7 the student finds five such errors in ready code (S26).

## First exam and exercise 7   [S25-S26]
**What:** The first exam is in week 8. It covers weeks 1-7, including deadlock, semaphore, wait on condition and concurrent collections. For practice: the collection (zbirka) of 13 solved TPL tasks and exercises 1-7.
Exercise 7 contains:
- Philosophers: deadlock, two fixes, measurement.
- A car park with a semaphore: at most 3 cars, also when a car breaks down.
- A message queue with Monitor.Wait and PulseAll: repair the given queue.
- Five errors from weeks 1-6: find, fix, explain.
Visualisation: Vizualizacije/filozofi.html shows philosophers step by step (deadlock, order, waiter, "return and try again") and a queue with if or while.
**Link:** (inferred) The car park with "when a car breaks down" tests S15 (Release in finally).

## References   [S27]
**What:** The notes say: deadlock conditions come from Coffman, philosophers and starvation definitions from Herlihy and Shavit, livelock from the Oracle tutorials, class behaviour from Microsoft documentation.
- [1] T. Dadić: Data race condition, lecture, 2022/23.
- [2] E. G. Coffman, M. J. Elphick, A. Shoshani: System Deadlocks. ACM Computing Surveys 3(2), 1971, 67-78.
- [3] M. Herlihy, N. Shavit: The Art of Multiprocessor Programming. Morgan Kaufmann, 2008.
- [4] Oracle: The Java Tutorials, Starvation and Livelock. docs.oracle.com/javase/tutorial/essential/concurrency/starvelive.html
- [5] Microsoft Learn: SemaphoreSlim Class. learn.microsoft.com/dotnet/api/system.threading.semaphoreslim
- [6] Microsoft Learn: Semaphore and SemaphoreSlim. learn.microsoft.com/dotnet/standard/threading/semaphore-and-semaphoreslim
- [7] Microsoft Learn: Monitor.Wait, additional remarks. learn.microsoft.com/dotnet/fundamentals/runtime-libraries/system-threading-monitor-wait
- [8] Microsoft Learn: Monitor.Pulse. learn.microsoft.com/dotnet/api/system.threading.monitor.pulse
- [9] Microsoft Learn: Thread-safe collections. learn.microsoft.com/dotnet/standard/collections/thread-safe
- [10] Microsoft Learn: BlockingCollection overview. learn.microsoft.com/dotnet/standard/collections/thread-safe/blockingcollection-overview
- [11] ThreadMentor (Michigan Tech): The Dining Philosophers Problem with Four Chairs. pages.mtu.edu/~shene/NSF-3/e-Book/SEMA/TM-example-philos-4chairs.html

## Exercise cross-reference
Exercise 7 is not in the repository yet. No project for philosophers, semaphore or queue exists under `exercises/`. These tasks relate to this lecture.

- `exercises/05-periodic-tasks/Zadatak5_OgradaZastoj/Zadatak5_OgradaZastoj.cs` (optional task).
  - Concept: waiting without a time limit. Eight tasks use a Barrier. One random task sleeps 10 s before the second barrier.
  - Key insight: the default program waits about 10 s (sample output: "Glava: 654 (10016 ms)"). The barrier waits for the slowest task. The fix: a CancellationTokenSource(2000), `ograda.SignalAndWait(token)`, and `Task.WaitAll` in try/catch. The sample output is "Rezultat nije dobiven (2010 ms)".
  - If ignored: the program waits as long as the slowest task. This is not a deadlock by Coffman conditions, because the slow task is not in a circle (inferred). It shows the same symptom: tasks wait and do not progress. The "no preemption" idea links to it: a time limit or cancellation lets a waiter give up (inferred). The upute say: the program must be correct with any delay of any processor (aPRAM, lecture 5).
- `exercises/03-data-race/Zadatak2_Racun/Zadatak2_Racun.cs`.
  - Concept: "check, then change" (S22 pattern). Withdrawing 1 EUR when the balance is at least 1: each step is atomic with Interlocked, but the pair is not.
  - Key insight: a lock around both steps, or CompareExchange, gives balance 0 and payout 10,000.
  - If ignored: the same error as `while (!bc.IsCompleted) bc.Take()`.
- `exercises/03-data-race/Zadatak1_Brojac/Zadatak1_Brojac.cs`.
  - Concept: lock with a shared lock object versus one lock per task. The "own lock" trap from the S24 week-4 row.
  - If ignored: the sum is wrong, for example 3,195,136 instead of 12,000,000.
- `exercises/03-data-race/Vizualizacije/index.html` states that deadlock (lecture 4, slide 20) is treated in detail in week 7.

## Open points
- S3, S5, S6 and S8 contain diagrams (locks A and B, table with F0-F4 and v0-v4). S9 has only code boxes and text. I checked the .pptx: they are native shapes (boxes, text labels and lines) with text labels only. On S3 four lines show "holds" (A to task 1, B to task 2) and "waits" (task 1 for B, task 2 for A). The caption on the slide states the same facts, so no content is lost.
- S10 and S11: the slides class "return and try again" as removal of "no preemption". The reasoning also fits "hold and wait" (see the inferred note in S11).
- The numbers "slide 28 of lecture 5" (S16) and "slide 17 / 20 of lecture 4" (S2, S3, S20) refer to other decks. I did not verify them. The numbers "slajd 14" (S9), "slajd 8", "slajd 9" and "slajd 10" (S11), and "slajda 18" (S19) refer to this deck and are correct.
- The sources exercise 7 (philosophers, car park, message queue, five errors) and the demo project ZastojDemo (Primjer1-9) are not in the repository. Demo results come only from the slides.
- S10: the table row "in the same rhythm" is forced by a barrier. The source does not say which barrier type or how the code works.
- S9 uses `lock (lijeva) lock (desna)` with shorthand names. The source does not define lijeva and desna as variables here.
- Only S25 mentions the "zbirka" (13 solved TPL tasks). The source does not name where it is.

## Coverage ledger
| slide | topic section | items |
| --- | --- | --- |
| 1 | Recap (title, author, base lecturer, year, last lecture before exam) | 4 |
| 2 | Recap (plan, recap of lectures 4, 5, 6, wrong-prevention remark) | 6 |
| 3 | Recap: deadlock pattern (with diagram caption) | 7 |
| 4 | Four necessary conditions | 8 |
| 5 | Dining philosophers | 5 |
| 6 | Trap: everybody takes left | 6 |
| 7 | Prevention | 5 |
| 8 | Fix 1: order | 5 |
| 9 | Fix 2: waiter | 5 |
| 10 | Livelock | 6 |
| 11 | Fixes and Coffman conditions (with inferred note, 0 deadlocks for both fixes) | 9 |
| 12 | Starvation | 6 |
| 13 | Is lock fair (with ten percent note) | 6 |
| 14 | Semaphore | 7 |
| 15 | Trap: Release skipped | 6 |
| 16 | A semaphore is not a lock | 5 |
| 17 | Wait and PulseAll | 6 |
| 18 | Trap: if instead of while | 5 |
| 19 | Trap: pulse not remembered | 5 |
| 20 | Concurrent collections | 6 |
| 21 | BlockingCollection | 6 |
| 22 | Trap: check, then take | 5 |
| 23 | What to choose | 7 |
| 24 | Review of traps | 7 |
| 25 | First exam | 3 |
| 26 | Exercise 7 | 5 |
| 27 | References | 11 |
