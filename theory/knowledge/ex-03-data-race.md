# Exercise 3: data race, lock and Interlocked (source: exercises/03-data-race/, README.md, Vjezba3_upute.html, 2 visualization pages, 3 C# projects; the .pptx is lecture 04 and is not covered here)

## Map
- The index page asks: what happens when data must be shared? How to protect it (critical section, lock, atomic operations)? What does protection cost? Which new traps does it bring? The pages show the orders of steps of two tasks. The student chooses the next task, and the program counts what all possible orders give.
- The counter page continues section 6 of week 2 ("Nastavak odjeljka 6 tjedna 2").
- This file covers exercise 3 (Vježba 3): three tasks (counter, account, stack) and two interactive visualizations.
- Main idea: two tasks that read, change and write shared data in separate steps can lose an update. A step that is safe alone does not make a group of steps safe.
- Fixes: one shared lock (brava), `Interlocked` for one operation, `Interlocked.CompareExchange` in a loop for "check, then change". The best fix is to not share the data.
- Theory is in [04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md). The visualizations cite slide numbers of lecture 04. This file gives the slide numbers as the source states them.

## Setup and how to run   [README.md, Vjezba3_upute.html]
**What:**
- `Vjezba3.sln` holds three separate projects. Each task runs alone, without a menu.

| Task | Project folder | File |
|---|---|---|
| 1 | `Zadatak1_Brojac/` | `Zadatak1_Brojac/Zadatak1_Brojac.cs` |
| 2 | `Zadatak2_Racun/` | `Zadatak2_Racun/Zadatak2_Racun.cs` |
| 3 | `Zadatak3_Stog/` | `Zadatak3_Stog/Zadatak3_Stog.cs` |

- Docker (recommended in this repository), from the repository root: `docker compose run --rm dotnet dotnet run -c Release --project exercises/03-data-race/Zadatak1_Brojac`
- HTML pages: `docker compose up web`, then http://localhost:8080/03-data-race/Vjezba3_upute.html and http://localhost:8080/03-data-race/Vizualizacije/index.html
- Visual Studio or Rider: open `Vjezba3.sln`, select the task project, run it.
- Visual Studio Code: File > Open Folder, select this folder, use the configuration "Vježba 3: zadatak N". For timing, select **Release**.
- Command line: in the task folder, `dotnet run -c Release`.
- The projects target .NET 10 (see `Directory.Build.props`). The comment in that file says: if you have only Visual Studio 2022, change `net10.0` to `net8.0`. The file also enables `ImplicitUsings` and `Nullable`.
- The instructions say: before the exercise, read the lecture presentation and the visualizations (counter, account and stack).
- Learning goal (instruction page): after the exercise the student can recognize a data race and a race condition, and fix them with a lock, `Interlocked` and `CompareExchange`.

**Why (Release):** The task measures time. A Debug build adds work that changes the times. (inferred) The source only says "measure time in Release".
**Conclusion:** Always run timing in Release.

## Data race and the three steps of `brojac++`   [visualization brojac.html, section 1]
**What:** `brojac++` is three steps: read the counter into a register of the task, increase the register, write the register into the counter. The counter is shared. Each task has its own register (slide 4 of lecture 04).
**Why:** Two tasks can both read the same number before any task writes. Then both write the same result and one increment is lost. This is a data race.
**Conclusion:** A lost increment means the result is smaller than expected and different on each run.
**Example (visualization):**
- Two tasks, A and B. Each task does `brojac++` twice. Expected result: 4. The student chooses which task runs the next step. The real scheduler chooses by itself.
- Button "Do kraja (slučajno)" finishes the run with random choices. Button "Ispočetka" resets.
- Goal in the mode "bez zaštite" (no protection): build an order where both tasks read the same number before anyone writes.
- Code lines shown by the page for each mode (the loop header is `for (int i = 0; i < 2; i++)   // povećanje N od 2`):
```
// bez zaštite
reg = brojac;
reg = reg + 1;
brojac = reg;
// jedna zajednička brava
lock (brava)       // zajednička
    reg = brojac;
    reg = reg + 1;
    brojac = reg;
// kraj lock
// svaki task svoja brava: same lines, but the first line is
lock (mojaBrava)   // svaki task svoju
// Interlocked.Increment
Interlocked.Increment(ref brojac);
```
- Button behaviour: the page shows the next step of each task as a highlighted line. The display "brava drži" shows "nitko" (nobody), A, B, "svaki svoju" (each its own), or "–" in the modes "bez zaštite" and "Interlocked.Increment". The page also shows "koraka" (the number of log lines) and, for each task, "registar: N" or "gotov" (finished). The result line shows "točno" (correct) or "krivo" (wrong) with "očekivano 4: izgubljeno N" (expected 4: lost N).
- Instruction "Kad task čeka na bravu, pogledajte što se dogodi ako kliknete njegov korak": in mode "jedna zajednička brava" a task that waits is marked "← čeka bravu". A click on its step changes no counter, register or lock state. The log still gets the line, and the "koraka" (steps) number, which is the log length, still grows. The log line is "X: čeka, bravu drži Y". (racun_stog.html does the same in lock mode.) A waiting task cannot take a step. Therefore the counting section leaves out such orders.
**Pitfall:** The step list is a model. The real CPU may differ in detail. (inferred) The source says only that the scheduler chooses the order in a real program.
**Link:** Task 1 below. Week 2 section 6 animation (the source mentions it for three increments per task; the file is outside this folder).

## Counting all possible orders   [visualization brojac.html, section 2]
**What:** The page walks through every possible order of the steps of two tasks (a task that waits for a lock cannot take a step) and counts how many orders end with each counter value.
**Why:** (inferred) It shows which fixes are correct for every order, not only for one run.
**Step model per mode (from the page code):** no protection = 3 steps per increment (read, increase, write). Shared lock and own lock = 5 steps (lock, read, increase, write, unlock). `Interlocked.Increment` = 1 step. Each task does 2 increments.
**Why the counts are so:** (inferred from the step counts) No protection: 6 steps per task, C(12,6) = 924 orders. Interlocked: 2 steps per task, C(4,2) = 6 orders, and each step is indivisible. Own lock: 10 steps per task, C(20,10) = 184,756 orders, and no task ever waits, so the lock removes no order. Shared lock: only 6 orders remain, because a task cannot step while the other holds the lock.
**Conclusion:** The lock removes orders, and the own lock removes none. The count is a number of possible orders, not a probability. The scheduler does not choose all orders equally often. Therefore the ratio does not tell how many increments disappear in a real program.
**Example (results I computed by running the page logic with 2 tasks, 2 increments each; the page shows them as a histogram, the source text does not list the numbers):**

| Mode | Orders and final counter value |
|---|---|
| bez zaštite (no protection) | 924 orders: counter 2 in 702, counter 3 in 216, counter 4 in 6 |
| jedna zajednička brava (one shared lock) | 6 orders: all give 4 |
| svaki task svoja brava (own lock per task) | 184,756 orders: counter 2 in 96,550, counter 3 in 78,100, counter 4 in 10,106 |
| Interlocked.Increment | 6 orders: all give 4 |

- Task given by the page: run all four modes. In which modes is every order correct? What is the smallest value the counter can have without protection?
- Answer from the numbers (inferred from the computed table): modes "one shared lock" and "Interlocked.Increment". The smallest value is 2 with two tasks and two increments each. (The page leaves this as a question.)
**Pitfall:** Do not read the histogram as a chance of failure.
**Link:** Questions for thinking, below.

## Lock with one shared lock object   [Task 1, slides 7-9]
**What:** `lock (brava) { brojac++; }`. While one task holds the lock, the other task waits at the entrance. The three steps stay separate, but no second task can run its steps inside the lock.
**Why:** (inferred) The lock lets one task at a time run the critical section (kritični odsječak). The steps read, increase, write then run as one unit for every task that uses the same lock object. The page asks this as thinking question 1 and gives no answer; the theory is in [04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md).
**Conclusion:** One lock object, created before the tasks, shared by all tasks. Result: 12,000,000 every time.
**Pitfall:** The lock object must be the same object for all tasks (see next section).
**Link:** Zadatak 1 step 2.

## Trap: each task has its own lock   [Task 1, slide 11]
**What:** Each task makes its own `new object()` and locks it.
**Why:** The lock exists, but it protects nothing, because no task ever needs the object of another task. Nobody waits, so the result is as wrong as without protection. In the visualization the step text says "uzima svoju bravu (nitko drugi je ne treba)": takes its own lock (nobody else needs it).
**Conclusion:** A lock protects only against tasks that use the same lock object.
**Example:** In the lecture run (12 tasks, 1,000,000 each): own lock per task gave 1,981,047. In the C# comment: for example 3,195,136, less than 12,000,000.
**Link:** Zadatak 1 step 2 ("show the trap").

## Interlocked and local counter   [Task 1, slides 12-14]
**What:** `Interlocked.Increment(ref brojac)` makes the increment one indivisible step (slide 12). The fastest way is to not share the counter: count in a local variable, then add once with `Interlocked.Add` (slide 14).
**Why:** Synchronization costs time. A local counter needs one synchronization per task, not one per increment.
**Conclusion:** Prefer isolation: not sharing a changeable value is better than protecting it (index.html: "najbolje ne dijeliti promjenjiv podatak"). index.html also lists as a goal to know when `Interlocked` is enough and when a lock or `CompareExchange` is needed (check, then change). The rule "use `Interlocked` when the whole operation is one atomic call" is (inferred) from the account example.
**Example (lecture measurement, 12 tasks, 1,000,000 each, source: DataRaceDemo output of 2026-10-25, i7-9750H, .NET 10, Release):**

| Method | Result | Time |
|---|---|---|
| no protection | 1.8 to 2.6 million instead of 12 million | not given |
| lock | 12,000,000 | 456 ms |
| Interlocked | 12,000,000 | 203 ms |
| sequential in one task | 12,000,000 | 5 ms |
| local + one `Interlocked.Add` | 12,000,000 | 1 ms |

- Comments in the solution code, measured on the exercise author's computer: lock about 155 ms against about 5 ms sequential; `Interlocked.Increment` about 246 ms ("on this computer it is not faster than the lock: measure on yours"); local + `Interlocked.Add` about 6 ms. See Open points.
**Pitfall:** `Interlocked.Increment` is not always faster than a lock. Measure on your own computer.
**Link:** Lecture 04 slides 13-14 for the measurement; [04-data-race-lock-interlocked.md](04-data-race-lock-interlocked.md).

## Race condition without data race ("check, then change")   [visualization racun_stog.html, section 1; slides 5, 15-16]
**What:** Every single step is protected (atomic with `Interlocked`), so there is no data race. The group of two steps (check the balance, then change it) is not protected. Between check and change another task takes the last euro. This is a race condition. (The source keeps the English terms "race condition" and "data race".)
**Why:** Atomicity of each step does not give atomicity of the sequence. The correct result needs check and change as one unit.
**Conclusion:** Data race and race condition are different. A program can have a race condition with no data race. Fix: lock around both steps, or `CompareExchange`, which writes the new state only if the old state is still the one the task read.
**Example (visualization):**
- The account holds 1 euro. Two tasks each want to pay out 1 euro, only if money exists.
- Mode "provjeri, pa promijeni": read the balance; if at least 1, decrease it and increase the payout counter.
- Mode "lock oko oba koraka": check and change inside the same lock (slide 16).
- Mode "CompareExchange": read the old balance; if it is at least 1, write old - 1 only if the balance still equals old; if not, try again (slide 16).
- Goal: in the first mode, let both tasks check before anyone decreases. Then use the same order in the other two modes. When `CompareExchange` fails, watch what the task does.
- Code lines shown in the page for the two other modes:
```
// provjeri, pa promijeni
if (Interlocked.Read(ref stanje) >= 1)
    Interlocked.Decrement(ref stanje);
    Interlocked.Increment(ref isplata);
// lock oko oba koraka
lock (brava) {
    if (stanje >= 1)
    { stanje--; isplata++; }
}
```
- Step-log texts: "Read(stanje) = N → ima novca" or "nema novca, kraj" (money exists / no money, end); "uzima bravu" (takes the lock); "otpušta bravu" (releases the lock); "staro = N → nema novca, kraj" in the CompareExchange mode.
- Code lines shown in the page for the CompareExchange mode:
```
staro = Interlocked.Read(ref stanje);   // ako < 1: kraj
zatekao = CompareExchange(ref stanje, staro - 1, staro);
if (zatekao == staro) Increment(ref isplata);   // inače ponovi
```
- `zatekao` means "found": the value that `CompareExchange` found in the balance. The step log says "CompareExchange zatekao X = staro" on success and "zatekao X ≠ staro Y → ponovi" on failure.
- Result line when wrong: "isplaćeno 2 €, a bio je samo 1 €: stanje −1" (paid 2 euro, but only 1 euro existed).
- Orders counted (I computed them; 2 tasks, balance 1):

| Mode | Orders: result |
|---|---|
| provjeri, pa promijeni | 16 orders: 4 give "balance 0, payout 1"; 12 give "balance -1, payout 2" |
| lock oko oba koraka | 2 orders: both give "balance 0, payout 1" |
| CompareExchange | 16 orders: all give "balance 0, payout 1" |

- Lecture run: 10,000 euro, 12 tasks, balance -4 and -5.
- Solution statement from the page: with lock or `CompareExchange` both give balance 0 and payout 10,000.
**Pitfall:** "Every step is atomic" does not mean "the program is correct".
**Link:** Task 2.

## Lost node in a linked-list stack   [visualization racun_stog.html, section 2; slides 18-19]
**What:** Stack as a linked list. `Push` has two steps: (1) the new node points to the current top, (2) the top becomes the new node. With `CompareExchange`, step (2) succeeds only if the top is still the one read in step (1). If not, the task repeats step (1).
**Why:** Both tasks read the same top, so the second write of the top overwrites the first. The node of the first task points to the old top, but nothing points to that node. The node is lost.
**Conclusion:** `CompareExchange` sets the top only if it is unchanged, otherwise retry. `ConcurrentStack<T>` works in the same way (slide 19).
**Example (visualization):**
- Start: top -> 2 -> 1. Task A pushes 4. Task B pushes 5.
- Goal without protection: both tasks do step (1) before step (2). Which node disappears? Then run the same order with `CompareExchange`.
- Orders counted (I computed them): without protection 6 orders, 2 lose nothing, 2 lose node 4, 2 lose node 5. With `CompareExchange` 6 orders, none lose a node.
- Code lines shown by the page:
```
// bez zaštite
novi.Sljedeci = vrh;   // (1)
vrh = novi;            // (2)
// CompareExchange
novi.Sljedeci = vrh;   // (1)
CompareExchange(ref vrh, novi, novi.Sljedeci);   // (2), inače ponovi (1)
```
- Buttons: "korak A (Push 4)", "korak B (Push 5)", "Prebroji sve redoslijede za ovaj način" (count all orders for this mode). Step-log texts: "N.Sljedeci = vrh (M)", "vrh = N", and "CompareExchange: vrh je X, a ne Y → ponovi" (top is X, not Y, so repeat).
- Result lines: wrong: "čvor N pokazuje na stog, ali na njega ne pokazuje nitko" (node N points to the stack, but nothing points to it). Correct: "na stogu su oba nova čvora" (both new nodes are on the stack). Histogram pills: "N od M redoslijeda daje krivi rezultat" (N of M orders give a wrong result) and "svaki redoslijed daje točan rezultat" (every order gives a correct result).
- The page draws the lost node with its pointer, for example "izgubljen: 4 → 2". When `CompareExchange` retried, the result line adds "jedan task je pokušao N puta" (one task tried N times). This makes the retry visible.
- Lecture run (12 tasks, 100,000 each): more than 1 million nodes lost out of 1.2 million without protection. With `CompareExchange`: 1,200,000 nodes, 0 lost.
**Pitfall:** Thinking question from the page: the order (1) then (2) in `Push` matters even with one task only (answer not in the source).
**Link:** Task 3.

## Questions for thinking (from both pages)   [brojac.html, racun_stog.html]
Brojac page:
1. With one lock there are no lost increments, although read, increase, write are still separate steps. Why?
2. In mode "own lock per task" a lock exists. Why is the result not better than without protection?
3. What are the smallest and largest values of the counter with three increments per task (as in section 6 of week 2)?
4. `Interlocked` is about two times faster than a lock. Sequential counting is about 40 times faster than `Interlocked` (slide 13). Why is "local, then one Add" the fastest?

Account and stack page:
5. In section 1 there is no data race and the result is wrong. Why is `Interlocked` on every step not enough?
6. When `CompareExchange` fails, the task reads again. Can a task repeat forever? What must be true for that?
7. Why does the order of steps (1) then (2) in `Push` matter, also when only one task runs?
8. When do you choose a lock and when `CompareExchange` (slide 21)?

Answers that the page text supports (inferred from the Problem and Solution boxes):
- Question 1: a held lock makes the other task wait, so the three steps of one task cannot mix with the steps of another task.
- Question 2: each task locks its own object, so nobody waits and the steps mix as without protection (box "Problem").
- Question 4: the local counter needs one synchronization per task, not one per increment (slide 14).
- Question 5: the check and the change are two steps. Another task can run between them. Atomicity of each step does not make the pair atomic (box "Problem").

The source gives no answers to questions 3, 6, 7 and 8. Do not invent them.

## What the student should know after week 4   [index.html]
- Explain why an increment is lost when many tasks write one value (data race) and the difference between data race and race condition.
- Protect a critical section with a lock, with one shared lock object.
- Know when `Interlocked` is enough and when a lock or `CompareExchange` is needed (check, then change).
- Know that synchronization costs time and that it is best to not share a changeable value (isolation, local first, then once).
- Slides without a visualization, on purpose: cost of synchronization (13-14) is measured and drawn on the slide; `List<T>` (17) loses elements as the counter does; deadlock (20) is covered in week 7 (see [07-zastoj-semafori-konkurentne-kolekcije.md](07-zastoj-semafori-konkurentne-kolekcije.md)). Exercise 4 does not exist yet.

Slide-to-visualization map (index.html):

| Slides | Topic | Visualization |
|---|---|---|
| 3-4 | Data race on counter; `brojac++` is three steps | brojac.html, mode "bez zaštite"; week 2 section 6 |
| 7-9 | Critical section, lock, fix with lock | brojac.html, mode "jedna zajednička brava" |
| 11 | Trap: own lock per task | brojac.html, mode "svaki task svoja brava" |
| 12 | Interlocked | brojac.html, mode "Interlocked.Increment" |
| 15-16 | Check, then change; lock around both steps and `CompareExchange` | racun_stog.html, section 1 |
| 18-19 | Stack as list; `CompareExchange` | racun_stog.html, section 2 |

References on the pages: Microsoft Learn (lock statement, Interlocked class, `Interlocked.CompareExchange`); .NET source code, `ConcurrentStack`; measurements DataRaceDemo, examples 6 and 8.

## Exercise tasks

### Zadatak 1: counter (`Zadatak1_Brojac.cs`)
**Task statement:** Given: 12 tasks increase the same counter 1,000,000 times each, without protection (expected 12,000,000). Steps:
1. Run the given code several times. How many increments are lost and why is the result different each time? (brojac.html)
2. Fix with a lock with one shared lock object, then show the trap "each task its own lock".
3. Fix with `Interlocked.Increment`.
4. Fix with a local counter in the task and one `Interlocked.Add`. Compare the times of all ways.

The file header says: measure time in Release.

**Concept tested:** data race, critical section, lock identity, atomic operation, cost of synchronization, isolation.

**Solution code idea:** A helper runs `K = 12` tasks, each one calls `posao(j)`. It resets `brojac`, measures time with `Stopwatch` and returns the pair (counter, ms). `Ispisi` prints name, counter with `N0` format and time.
```csharp
const int K = 12;
const int Povecanja = 1_000_000;
static (long, long) Izvedi(Action<int> posao)
{
    brojac = 0;
    var sat = Stopwatch.StartNew();
    Task.WaitAll(Enumerable.Range(0, K).Select(j => Task.Run(() => posao(j))).ToArray());
    return (brojac, sat.ElapsedMilliseconds);
}
```
The five variants:
```csharp
Ispisi("Bez zaštite", Izvedi(_ => { for (int i = 0; i < Povecanja; i++) brojac++; }));
var brava = new object();   // jedna, stvorena prije taskova
Ispisi("lock, zajednička brava", Izvedi(_ => { for (int i = 0; i < Povecanja; i++) lock (brava) { brojac++; } }));
Ispisi("lock, svaki task svoja", Izvedi(_ =>
{
    var svoja = new object();   // svoja brava: nitko nikoga ne čeka
    for (int i = 0; i < Povecanja; i++) lock (svoja) { brojac++; }
}));
Ispisi("Interlocked.Increment", Izvedi(_ => { for (int i = 0; i < Povecanja; i++) Interlocked.Increment(ref brojac); }));
Ispisi("lokalno + Interlocked.Add", Izvedi(_ =>
{
    long lokalno = 0;
    for (int i = 0; i < Povecanja; i++) lokalno++;   // bez dijeljenja
    Interlocked.Add(ref brojac, lokalno);   // jedna sinkronizacija po tasku
}));
```
Results in the comments:

| Variant | Result in comment |
|---|---|
| Bez zaštite | less than 12,000,000 and different every run |
| lock, shared lock | 12,000,000, correct but slow: about 155 ms against about 5 ms sequential |
| lock, own lock per task | for example 3,195,136, less than 12,000,000: the lock exists, but it does not protect |
| Interlocked.Increment | 12,000,000, about 246 ms (not faster than lock on this computer) |
| local + Interlocked.Add | 12,000,000, fastest, about 6 ms |

**Reasoning:**
- `brojac++` is three steps (read, increase, write). Two tasks read the same value, so an increment is lost.
- The shared lock makes the three steps one critical section. The lock object is created before the tasks, so all tasks use the same object.
- The own lock is created inside the task, so each task has a different object. No task waits for another.
- `Interlocked.Increment` is one atomic step. (inferred) It still synchronizes on every increment, 12,000,000 times in total.
- The local counter is not shared. Only 12 synchronizations remain (one per task).
- `brojac` is `long`, so `Interlocked` works on it with `ref`. (inferred)

**Pitfall:**
- Creating the lock object inside the task body (the trap).
- Timing in Debug.
- Expecting `Interlocked` to be faster than `lock` without measuring.
- Running the unprotected variant once: it can give a different value each run, so run it several times.

### Zadatak 2: account (`Zadatak2_Racun.cs`)
**Task statement:** Given: a payout from an account, each step atomic, and the balance still goes below zero. Account holds 10,000 euro; 12 tasks pay out 1 euro each while money exists. Steps:
1. Explain why this is a race condition and not a data race. (racun_stog.html)
2. Fix with a lock around both steps.
3. Fix with `Interlocked.CompareExchange` in a loop.

**Concept tested:** race condition without data race; atomic group of operations; compare-and-swap retry loop.

**Solution code idea:** Setup: `K = 12`, `Pocetno = 10_000`, fields `stanje` and `isplata`. `Izvedi` resets both and each task calls `isplati()` `2 * Pocetno` times, "each task tries more times than there is money".
```csharp
Izvedi(() =>
{
    if (Interlocked.Read(ref stanje) >= 1)   // provjeri
    {
        Interlocked.Decrement(ref stanje);   // promijeni
        Interlocked.Increment(ref isplata);
    }
});
```
Comment: most often balance below zero (for example -2), payout above 10,000 (sometimes not visible, run several times): race condition, every step is protected, the whole is not.
```csharp
lock (brava)   // provjera i promjena zajedno
{
    if (stanje >= 1) { stanje--; isplata++; }
}
```
Result: balance 0, payout 10,000.
```csharp
while (true)
{
    long staro = Interlocked.Read(ref stanje);
    if (staro < 1) break;   // nema novca
    if (Interlocked.CompareExchange(ref stanje, staro - 1, staro) == staro)   // upiše samo ako se stanje nije promijenilo
    {
        Interlocked.Increment(ref isplata);
        break;
    }
}   // inače je netko bio brži: ponovi
```
Result: balance 0, payout 10,000.

**Support code (Zadatak 2 only):**
```csharp
static void Ispisi(string naziv) =>
    Console.WriteLine($"{naziv,-30} stanje {stanje,4}, isplata {isplata:N0}");

static void Izvedi(Action isplati)
{
    stanje = Pocetno;
    isplata = 0;
    Task.WaitAll(Enumerable.Range(0, K).Select(_ => Task.Run(() =>
    {
        for (int i = 0; i < 2 * Pocetno; i++) isplati();   // svaki task pokuša više puta nego ima novca
    })).ToArray());
}
```
After the header comments, each of the three files runs `Console.OutputEncoding = System.Text.Encoding.UTF8;` and then calls `ZadatakN_Name.Pokreni();`. Only Zadatak1_Brojac.cs has `using System.Diagnostics;` before these lines, for `Stopwatch`.

**Reasoning:**
- No two tasks write a variable without protection, so there is no data race. The error is in the logic: between the check and the change, another task pays the last euro. Balance goes below zero.
- Lock: check and change are one critical section. (inferred) Inside the lock, plain `stanje--` is safe because every payout access to `stanje` goes through the same lock.
- `CompareExchange(ref stanje, staro - 1, staro)` returns the value it found. If it equals `staro`, nobody changed the balance and the write succeeded. If not, another task was faster and the loop repeats with a fresh read.
- Only the task whose write succeeded increases `isplata`. (inferred) This keeps `isplata` equal to the money paid out.

**Pitfall:**
- Protecting each step and believing the whole is safe.
- In the `CompareExchange` loop, forgetting to read again after failure, or increasing `isplata` when the write failed.
- The race does not always show: run several times.
- Do not mix lock and bare `Interlocked` on the same variable in one variant. (inferred; the three variants are run one after another, never at the same time)

### Zadatak 3: stack (`Zadatak3_Stog.cs`)
**Task statement:** Given: a stack as a linked list, `Push` without protection. 12 tasks push 100,000 nodes each (expected 1,200,000). Steps:
1. Run the given code. How many nodes are lost?
2. Write `Push` with a lock.
3. Write `Push` with `CompareExchange`, without a lock.

**Concept tested:** compound update of shared reference (read top, link, write top); lock and lock-free (`CompareExchange`) retry loop.

**Solution code idea:**
```csharp
class Cvor(int vrijednost)
{
    public int Vrijednost = vrijednost;
    public Cvor? Sljedeci;
}
static Cvor? vrh;

static void PushBezZastite(int v)
{
    var novi = new Cvor(v);
    novi.Sljedeci = vrh;   // (1)
    vrh = novi;            // (2)
}

static readonly object brava = new();
static void PushLock(int v)
{
    var novi = new Cvor(v);
    lock (brava)
    {
        novi.Sljedeci = vrh;
        vrh = novi;
    }
}

static void PushCas(int v)
{
    var novi = new Cvor(v);
    do
    {
        novi.Sljedeci = vrh;
    } while (Interlocked.CompareExchange(ref vrh, novi, novi.Sljedeci) != novi.Sljedeci);
}
```
Support code:
```csharp
static int Prebroji()
{
    int n = 0;
    for (var c = vrh; c != null; c = c.Sljedeci) n++;
    return n;
}

static void Izvedi(string naziv, Action<int> push)
{
    vrh = null;
    Task.WaitAll(Enumerable.Range(0, K).Select(_ => Task.Run(() =>
    {
        for (int i = 0; i < PoTasku; i++) push(i);
    })).ToArray());
    int n = Prebroji();
    Console.WriteLine($"{naziv,-18} {n,9:N0} čvorova, izgubljeno {K * PoTasku - n:N0}");
}
```
The file starts with the same `Console.OutputEncoding` line and the call `Zadatak3_Stog.Pokreni();`. `Pokreni` calls `Izvedi("Bez zaštite", PushBezZastite)`, `Izvedi("lock", PushLock)` and `Izvedi("CompareExchange", PushCas)`.

| Variant | Result in comment |
|---|---|
| Bez zaštite | for example 188,117 nodes, lost about 1,000,000 |
| lock | 1,200,000 nodes, lost 0 |
| CompareExchange | 1,200,000 nodes, lost 0 |

**Reasoning:**
- Without protection two tasks read the same top, so one node disappears (comment in code).
- The new node is made before the lock, so the lock holds only the two steps that touch shared data. (inferred)
- In the lock variant, steps (1) and (2) are one unit.
- In the `CompareExchange` variant, step (2) succeeds only if `vrh` is still the value that step (1) read. `CompareExchange` returns the value it found. If that differs from `novi.Sljedeci`, the loop runs again and step (1) reads the new top.
- `novi.Sljedeci` belongs to a node that only this task knows, so it does not change between the read and the compare. (inferred)

**Pitfall:**
- Reading `vrh` twice, once for the link and once for the compare. Another task can change `vrh` between the two reads. The code avoids this: it compares with `novi.Sljedeci`, which holds the value read in step (1). (inferred)
- (inferred) Counting nodes while tasks still run. The code calls `Task.WaitAll` before `Prebroji`.
- (inferred) Without protection the number of lost nodes can differ between runs. The source gives only one example run, "npr. 188.117" nodes.

## Open points
- Timing differs between sources. The page text (lecture, i7-9750H) says: lock 456 ms, Interlocked 203 ms (Interlocked about two times faster), sequential 5 ms, local + `Add` 1 ms. The code comments (exercise author's computer) say: lock about 155 ms, Interlocked about 246 ms (not faster), local about 6 ms. The source does not explain the difference. Both are kept.
- Lecture 04 mentions "slide 21" (lock or `CompareExchange`) and "slide 5" (race condition). This file does not hold those slides. See the lecture file.
- The histogram numbers in the "all possible orders" sections were computed by running the page logic. The page text does not list them. The page shows them as bars.
- The page links to files outside this folder (week 2 `tjedan2.html`, weeks 3 and 5 indexes). They are not in the repository folder. Not read.
- Zadatak 2 uses `Interlocked.Read` and `Interlocked.Decrement` on a `long`. The source gives no reason. This file gives none.
- The source gives no answers to the thinking questions. No answer is invented.
- `stil.css` (two copies) is only styling. The `.vscode`, `.editorconfig`, `.gitignore`, `.csproj` and `.sln` files are tooling boilerplate and give no knowledge. `Directory.Build.props` was read: Setup uses its target framework and comment.

## Coverage ledger
| Source item | Section where covered | Items |
|---|---|---|
| README.md | Setup and how to run | 9 |
| Vjezba3_upute.html: Priprema | Setup and how to run | 3 |
| Vjezba3_upute.html: Zadatak 1 (4 steps) | Exercise tasks, Zadatak 1 | 5 |
| Vjezba3_upute.html: Zadatak 2 (3 steps) | Exercise tasks, Zadatak 2 | 4 |
| Vjezba3_upute.html: Zadatak 3 (3 steps) | Exercise tasks, Zadatak 3 | 4 |
| Zadatak1_Brojac.cs | Exercise tasks, Zadatak 1: helper, 5 variants, 5 result comments (items: Izvedi, Ispisi, 5 code variants, 5 comments, K and Povecanja) | 13 |
| Zadatak2_Racun.cs | Exercise tasks, Zadatak 2: Ispisi, Izvedi, 3 variants, 3 result comments, entry lines | 10 |
| Zadatak3_Stog.cs | Exercise tasks, Zadatak 3: Cvor, 3 Push variants, Prebroji, Izvedi, 3 result comments, entry lines | 12 |
| Vizualizacije/index.html | Map; What the student should know; slide map | 14 |
| Vizualizacije/brojac.html section 1 | Data race and the three steps (buttons, display, result lines, wait-click behaviour); lock sections | 11 |
| Vizualizacije/brojac.html section 2 | Counting all possible orders (step model, counts, reason for counts) | 9 |
| Vizualizacije/brojac.html problem, solution, questions | Trap; Interlocked and local counter; Questions | 12 |
| Vizualizacije/racun_stog.html section 1 | Race condition without data race (code lines, `zatekao`, result line) | 12 |
| Vizualizacije/racun_stog.html section 2 | Lost node in a linked-list stack (lost-node drawing, retry count) | 10 |
| Vizualizacije/racun_stog.html questions, sources | Questions; Map references | 5 |
| Vizualizacije/zajednicko.js | navigation and number format helper only, no knowledge | 0 (not applicable) |
| Vizualizacije/stil.css, stil.css | styling only | 0 (not applicable) |
| Directory.Build.props | Setup (net10.0, net8.0 fallback, ImplicitUsings, Nullable) | 3 |
| .vscode/launch.json, tasks.json, .editorconfig, .gitignore, .csproj, .sln | not extracted: tooling boilerplate, outside the scope rule; README already gives the run configuration "Vježba 3: zadatak N" and Release advice | 0 |
| .pptx | lecture 04, covered by another file | 0 (excluded by instruction) |
