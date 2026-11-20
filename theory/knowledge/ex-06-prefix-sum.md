# Exercise 6: prefix sum algorithm (source: exercises/06-prefix-sum, intro deck 14 slides, README.md, Vjezba6_upute.html, 6 C# solutions)

## Map
- This exercise makes the student build the Blelloch prefix sum (algoritam sume prefiksa) from lecture [06-algoritam-sume-prefiksa.md](06-algoritam-sume-prefiksa.md).
- The path has four steps: tree indexes (Zadatak 0), reduction (Zadatak 1), prescan and scan (Zadatak 2), measurement (Zadatak 3).
- Two applications follow: GCD (NZD, Zadatak 4) and binary numbers (Zadatak 5, optional).
- The main idea: one tree algorithm works for any associative operation, but only if the code handles length, order and neutral element correctly. It is fast only if each task has enough work.
- Each task starts with a starter project that has empty methods. The starter `Main` prints a wrong result (usually 0) and "ne" (no). The student must change every "ne" to "da" (yes).

## Setup and workflow   [S1-S3, S14]
**What:**
- Install the .NET 10 SDK from dotnet.microsoft.com/en-us/download. Check with `dotnet --list-sdks`. The output must show version 10.x.
- Supported environments: Visual Studio 2026 (.NET workload), JetBrains Rider, Visual Studio Code with the C# Dev Kit extension.
- Visual Studio and Rider: open `Vjezba6.sln`. Each task is a separate project. Use Set as Startup Project, then F5 (or Ctrl+F5).
- Rider: File > Open, select `Vjezba6.sln`, select the task project in the run configuration list.
- VS Code: install the suggested extension C# Dev Kit (or the extension C#). File > Open Folder, select the exercise folder, run configuration "Vježba 6: zadatak N (Debug)". Use the **Release** configuration for timing.
- Command line: `cd Zadatak1_Reduciranje`, then `dotnet run`. For timing use `dotnet run -c Release`. From the exercise folder this also works: `dotnet run --project Zadatak1_Reduciranje`.
- Each task is one file: `ZadatakN_*/ZadatakN_*.cs`. It holds the whole task including the starter code.
- Only Visual Studio 2022? In the `.csproj` file of each task change `<TargetFramework>` from `net10.0` to `net8.0`. In VS Code also change the paths in `.vscode/launch.json`. The tasks work the same.
- Goals stated in the deck: derive the tree indexes, write reduction, prescan and scan for any associative operation, then apply them to GCD and binary numbers, and measure when the parallel prefix sum is worth its cost.
- Preparation: read lecture 6 first (scan, prescan, reduction, down-sweep, blocks, Brent rule).
- Submission: the exercise is recorded by attendance (at least 70 % of the exercise sessions). Two homework tasks are graded. The source says they will be described later.

**Why:**
- Zadatak 3 and Zadatak 4 measure time. The source says: run the program in the Release configuration, not in Debug.
- (inferred) The Debug build is not optimized, so Debug times do not show the real speed.
- Every `Main` sets `Console.OutputEncoding = UTF8` and `CultureInfo.CurrentCulture = hr-HR`. (inferred) UTF-8 prints the symbols `⊕` and `→` and the quote signs. The Croatian culture prints a decimal comma (`0,01`) and a dot as thousands separator (`−22.532.375`, `4.194.304`).
- The same environment as in earlier exercises is used (speaker note).
- The empty starter methods are the student's work. `Main` already calls them and compares with the sequential result (speaker note).

**Conclusion:** Always measure in Release. Read each `Main` output as the test: "da" means correct, "ne" means the student's method differs from the sequential one.

**Pitfall:** Example outputs in `Vjezba6_upute.html` come from a Release run on a 6-core, 12-logical-core processor (macOS). The numbers and times change from run to run.

**Link:** [ex-02-parallel.md](ex-02-parallel.md) uses the same environment and `Parallel.For`.

## Recap: scan, prescan, reduction   [S4]
**What:** For input `3 1 7 0 4 1 6 3` and addition:
```
ulaz:         3 1 7 0 4 1 6 3
scan:         3 4 11 11 15 16 22 25
prescan:      0 3 4 11 11 15 16 22
reduciranje:  25
```
- Scan (scan): element i is the sum up to i, including i. Prescan (prescan): element i is the sum of everything before i. Element 0 is the neutral element. Reduction (reduciranje): only the last number.
- Neutral element (neutralni element) I: `x ⊕ I = I ⊕ x = x`.

| Operation | Neutral element |
|---|---|
| addition | 0 |
| multiplication | 1 |
| max | the smallest possible number |
| string concatenation (spajanje nizova) | `""` |

- The tree works in the same array, one level after the other. At level r the pair is at distance 2^r.
- The operation must be associative: `(x ⊕ y) ⊕ z = x ⊕ (y ⊕ z)`.

**Why:** The tree groups the operands in a different way than the sequential loop. Associativity guarantees the same result for any grouping. (inferred from the lecture; see [06-algoritam-sume-prefiksa.md](06-algoritam-sume-prefiksa.md), section "Trap 4: associativity".)

**Conclusion:** Before the tree, check two properties of the operation: associative (required) and commutative (not required, but the operand order must be kept).

**Link:** Definitions are in lecture section 1. Zadatak 1 (subtraction) breaks associativity. Zadatak 2 (strings) tests order.

## The four traps of the day   [S5]
**What:** The deck lists four traps, written as the student will see them.

| Trap (deck wording) | Detail | Tasks |
|---|---|---|
| Drugi rezultat (a different result) | The tree gives a result different from the sequential one, but only for some operations. | 1 |
| Radi za 16, ne za 20 (works for 16, not for 20) | For some lengths the result is wrong and no exception occurs. | 0, 2, 4 |
| Paralelno je sporije (parallel is slower) | Even with 12 cores it is slower than sequential. | 3, 4 |
| Radi za +, ne za vašu operaciju (works for +, not for your operation) | The sum is correct, but strings or prefixes are not. | 2, 5 |

**Why:** Speaker note: the traps are written as the student sees them. There are four kinds: a result different from the sequential one, code that works for some lengths and not for others without an exception, parallel code that is slower, and code that works for addition but not for another operation.

**Conclusion:** For each trap do three steps in this order. First find the test that fails. Then find the cause. Then fix the code.

**Link:** The traps of the lecture are in [06-algoritam-sume-prefiksa.md](06-algoritam-sume-prefiksa.md), section 20. The note of slide 1 says: "Na predavanju smo vidjeli Blellochov algoritam i šest zamki oko njega" (six traps). The lecture file counts seven (see Open points).

## Visualizations and demo instructions   [S6, S9, S12, S14; Vjezba6_upute.html]
**What:** Two interactive pages belong to lecture 6 (folder `Vizualizacije`, week 6). The folder is not in this repository (open point).
- `blelloch.html`: phase 1, phase 2 and the whole algorithm on a tree and in an array. Use it with Zadatak 0, 1, 2 and 5.
- `suma_prefiksa.html`: limited number of processors. Section 2 has a finished example (check for the paper task). Section 3 shows the number of operations (use with Zadatak 3).

**Demo cues from the speaker notes:**
- Zadatak 0: show `blelloch.html`, section 1, after step 3.
- Zadatak 3: show `suma_prefiksa.html`, section 3 (number of operations), after the task.
- Paper task: check the result in `suma_prefiksa.html`, section 2.

**Link:** Lecture sections 8, 9 (Blelloch phases) and 14, 15 (blocks, Brent rule).

## Exercise tasks

### Zadatak 0: tree indexes   [S6]  (`Zadatak0_Indeksi/Zadatak0_Indeksi.cs`)
**Task statement:**
1. Without a computer, continue the sequences to 31: `1 3 5 7 …`, `3 7 11 15 …`, `7 15 23 31 …`, `15 31 …`. Count them.
2. Write `IspisiNizove(n)`: print these sequences (only numbers smaller than n), each in its own row.
3. Write `IspisiParove(n)`: for each reduction level print all pairs `a[i] := a[j] ⊕ a[i]` of that level in one row. For n = 8 compare with the index table of lecture slide 14.
4. Write `ReducirajSlijedno(a)`: the same pairs, level by level, but sequential, in place. Return `a[n − 1]`.
5. Run. For n = 16 the result is correct. Explain the result for n = 20.
- Given: `Main` prints sequences for n = 32, pairs for n = 8 and 16, and sums 1, 2, …, n for n = 16 and n = 20. Speaker note: the first minutes are without a computer.

**Concept tested:** The index rule of the up-sweep: at level with distance d the pairs are `a[k·d − 1] := a[k·d − 1 − d/2] ⊕ a[k·d − 1]`. Also the limit of the tree: it needs n = 2^k (trap "works for 16, not for 20").

**Solution idea:**
```csharp
for (int d = 2; d <= n; d *= 2)
{
    Console.WriteLine(string.Join(" ", Enumerable.Range(1, n / d).Select(k => k * d - 1)));   // IspisiNizove
}
```
```csharp
Console.WriteLine(string.Join("  ", Enumerable.Range(1, n / d).Select(k => $"a[{k * d - 1}] := a[{k * d - 1 - d / 2}] ⊕ a[{k * d - 1}]")));   // IspisiParove, in the same loop over d
```
```csharp
for (int d = 2; d <= a.Length; d *= 2)
{
    for (int i = d - 1; i < a.Length; i += d)
    {
        a[i] += a[i - d / 2];
    }
}
return a[^1];
```
- Outputs in the solution comments. For n = 32: 5 sequences (`1 3 5 … 31; 3 7 … 31; 7 15 23 31; 15 31; 31`). For n = 8: 3 rows (`a[1] := a[0] ⊕ a[1] … a[7] := a[3] ⊕ a[7]`).
- n = 16: `a[15] = 136`, sum 136, "da". n = 20: `a[19] = 74`, sum 210, "ne".
- The starter prints `a[15] = 0`, sum 136, "ne" and `a[19] = 0`, sum 210, "ne".

**Reasoning:**
- The number of sequences is log2(n) = 5 for n = 32, because d doubles from 2 to 32. The sequence for distance d starts at d − 1 and grows by d. (inferred)
- Each sequence is one tree level. Each element of the sequence is the index that receives a sum at that level.
- For n = 20 the loop stops at d = 16. Only index 15 receives a sum of 16 elements.
- Index 19 never receives the sum of the whole array. (inferred) With 0-based indexes, `a[19]` gets `a[18]` at d = 2 and then `a[17]` at d = 4.
- The value is a[16..19] = 17 + 18 + 19 + 20 = 74. (inferred; the code comment gives only 74 and "n nije potencija broja 2")
- The code throws no exception, so the error is silent.

**Pitfall:** The result for n = 20 is wrong and nothing signals it. The tree needs n = 2^k, or padding with the neutral element (see Zadatak 2).

**Link:** Lecture [06-algoritam-sume-prefiksa.md](06-algoritam-sume-prefiksa.md), sections 8 (index table, slide 14) and 17 (padding and trap 7).

### Zadatak 1: reduction with the tree   [S7]  (`Zadatak1_Reduciranje/Zadatak1_Reduciranje.cs`)
**Task statement:**
1. Write `Reduciraj(long[] a, Func<long, long, long> op)`: tree, parallel inside the level, in place, n = 2^k. It returns `a[n − 1]`.
2. Compare the array after reduction (test 1) with the table of the lecture.
3. Test 2: compare the time with the sequential time. Why is the tree not faster?
4. Test 4 (subtraction) differs from the sequential result. Why?
- Given: `Slijedno(a, op)` (left to right) and `Main` with four tests. The big array has 2^22 values from `Random(2026).Next(-1_000_000, 1_000_001)`. Test 3 gives 1.000.000, the upper end of this range. The four tests are: `3 1 7 0 4 1 6 3` with addition (also prints the array); 2^22 random numbers with addition (and time); the same numbers with max (`Math.Max`); `3 1 7 0 4 1 6 3` with subtraction.

**Concept tested:** The reduction (reduciranje, also called up-sweep) with a parallel loop per level. Also associativity: the tree is correct only for associative operations.

**Solution idea:**
```csharp
for (int d = 2; d <= a.Length; d *= 2)
{
    Parallel.For(1, a.Length / d + 1, k => a[k * d - 1] = op(a[k * d - 1 - d / 2], a[k * d - 1]));
}
return a[^1];
```
- The loop over levels is sequential. Only the pairs inside one level run in parallel. `Parallel.For` returns when all pairs of the level are done, so it works as a barrier between levels. (inferred)
- Outputs: test 1 array `3 4 7 11 4 5 6 25`, result 25 = sequential 25. Test 2: −22.532.375 for both. Test 3: 1.000.000 for both. Test 4: tree −5, sequential −19, "ne".
- Times in the solution comment: sequential 7,2 ms, `Reduciraj` 9,3 ms (with array copy, shortest of 3). The starter output shows 55,2 ms and 61,2 ms (with the starter's empty `Reduciraj`). `Najkrace` does one warm-up call, then keeps the shortest of 3 runs.
- The starter prints `Reduciraj: 0`, array unchanged `3 1 7 0 4 1 6 3`, "ne" in all four tests.

**Reasoning:**
- The array after reduction holds partial sums. In `3 4 7 11 4 5 6 25` positions 1, 3 and 7 hold the sums of the first 2, 4 and 8 elements (4, 11, 25). This is the table from the lecture. (inferred)
- Subtraction is not associative. Sequential: `((((((3−1)−7)−0)−4)−1)−6)−3 = −19`. The tree groups the pairs differently and gives −5. (the grouping calculation is inferred)
- The source does not state why the tree is not faster in test 2. The reasons below are (inferred); see lecture sections 7, 13 and 15.
- The tree does n − 1 operations. The sequential loop does the same number.
- The tree starts a `Parallel.For` for each of the 22 levels. The operation `+` is cheap, so this overhead is large against the work.
- The timed `Reduciraj` also copies the 32 MB array with `Clone`. The source says "s kopiranjem niza".
- A sequential loop over 4 million elements reads the memory in order. Memory speed can limit it, not the processor.

**Pitfall:** The tree gives a different result for a non-associative operation like subtraction. For addition and max it is correct.

**Link:** Lecture sections 8 (reduction), 11 (trap 4: associativity), 13 (trap 6: tasks too small). Next: Zadatak 2 adds the down-sweep.

### Zadatak 2: prescan and scan   [S8]  (`Zadatak2_Prescan/Zadatak2_Prescan.cs`)
**Task statement:**
1. Write `Prescan<T>(T[] a, Func<T,T,T> op, T neutralni)`: reduction, then down-sweep, parallel inside the level. `a` becomes the prescan. It returns the total result `a0 ⊕ … ⊕ an−1`. The length need not be 2^k. The operation `op` is associative, not always commutative.
2. Write `Scan(a, op, neutralni)` with `Prescan`. It returns a new array. `a` stays unchanged.
3. All five tests must print "da". If one does not, find out why that one fails.
- Given: `ScanSlijedno` and `Main` with five tests; the methods are generic (T). Tests: 1) `3 1 7 0 4 1 6 3`, addition. 2) n = 2^22, addition. 3) n = 1000, addition. 4) n = 1000, values −100 to −1, max. 5) `a b c d e f`, string concatenation.
- Random data: seed 2026; tests 2 and 3 use values `Next(0, 100)`. Test 4 uses `Next(-100, 0)`.
- What "da" checks in the test harness: `prescan[0]` equals the neutral element, `prescan[i]` equals the sequential scan element `i − 1`, the total equals the last sequential scan element, and `Scan` equals the sequential scan.
- Speaker note: watch two sentences in the task: the length need not be a power of two, and the operation need not be commutative.

**Concept tested:** Padding to a power of 2 with the neutral element, the order of operands in the down-sweep, the neutral element of each operation.

**Solution idea:**
```csharp
int m = 1;
while (m < a.Length) { m *= 2; }
var b = new T[m];
Array.Fill(b, neutralni);
a.CopyTo(b, 0);
for (int d = 2; d <= m; d *= 2)
{
    Parallel.For(1, m / d + 1, k => b[k * d - 1] = op(b[k * d - 1 - d / 2], b[k * d - 1]));
}
T ukupno = b[m - 1];
b[m - 1] = neutralni;
for (int d = m; d >= 2; d /= 2)
{
    Parallel.For(1, m / d + 1, k =>
    {
        T lijevo = b[k * d - 1 - d / 2];
        b[k * d - 1 - d / 2] = b[k * d - 1];
        b[k * d - 1] = op(b[k * d - 1], lijevo);
    });
}
Array.Copy(b, a, a.Length);
return ukupno;
```
```csharp
var b = (T[])a.Clone();
T ukupno = Prescan(b, op, neutralni);
return b.Skip(1).Append(ukupno).ToArray();
```
- Outputs: test 1 prescan `[0] [3] [4] [11] [11] [15] [16] [22]`, scan `[3] [4] [11] [11] [15] [16] [22] [25]`. Test 5 prescan `[] [a] [ab] [abc] [abcd] [abcde]`, scan `[a] [ab] [abc] [abcd] [abcde] [abcdef]`. All five tests print "da, da".
- The starter prints prescan unchanged, scan all `[0]` (or `[]` for strings), "ne".

**Reasoning:**
- Padding: copy `a` into a work array `b` of length m (next power of 2) filled with the neutral element. The tree is then valid. The neutral element does not change any sum. At the end only the first `a.Length` elements are copied back. (n = 1000 gives m = 1024.)
- The total result is saved in `ukupno` before the last element is replaced by the neutral element. The down-sweep needs that neutral element at the root.
- Down-sweep step: the left child receives the old value of the parent. The right child receives `parent ⊕ left sum`. The parent value is on the left of the operator, because the parent holds the sum of all earlier elements. This keeps the order of operands. For strings, `x + y` is not commutative. A reversed order would give `ba` instead of `ab`. (inferred; see lecture section 12, trap 5)
- Scan from prescan: the scan is the prescan shifted left by one place, with the total result added at the end (`Skip(1)` then `Append(ukupno)`). `Clone` keeps `a` unchanged.
- Neutral element in test 4: all numbers are negative, so the neutral element of max must be `long.MinValue`. A neutral element 0 would be larger than all values and would give 0 in place of the real maximum. (inferred)
- Test 5: the neutral element is `""`.

**Pitfall:**
- If the student omits the padding, the code works for 2^k but not for n = 1000 (trap "works for 16, not for 20").
- Works for `+` but not for string concatenation: wrong operand order in the down-sweep.
- Wrong neutral element for max (for example 0) breaks test 4.

**Link:** Lecture sections 9 (down-sweep), 12 (trap 5), 17 (padding). Zadatak 3, 4 and 5 reuse this code.

### Zadatak 3: three implementations and measurement   [S9]  (`Zadatak3_Granularnost/Zadatak3_Granularnost.cs`)
**Task statement:**
1. `PrescanPoRazini`: copy `Prescan` from Zadatak 2 (n is always 2^k here).
2. Write `PrescanTaskPoParu`: the same algorithm, but one `Task.Run` for each pair on each level (like a PRAM with n/2 processors).
3. Write `PrescanBlokovi(a, p, op)`: p blocks as in the lecture (slides 25-26). n need not be divisible by p.
4. Step 1 of the output: which implementation is the slowest and why? Which is the only one faster than sequential?
5. Step 2: why is p = 1 slower than sequential? Why is p = 4.194.304 also not good?
6. Step 3 (`SkupoZbroji`): what changed? Explain with the Brent rule and granularity (granularnost).
7. A suggestion from the exercise of 2020/21: each task works with 4 elements instead of 2. Which of your methods already does this, and with how many elements?
- Measured sizes: n = 2^10, 2^14, 2^18, 2^20, 2^22, 2^24 (step 1). Step 2 uses n = 2^24 and p = 1, 2, 4, 8, `jezgri`, 2·`jezgri`, 1000, 100.000, 2^22. Data are `Next(0, 100)`, seed 2026.
- Given: `PrescanSlijedno`, `SkupoZbroji` (ten to fifty µs, depends on the computer and load; `Main` prints the measured value), and `Main` that checks and measures. A wrong result prints as "krivo" (wrong). All methods compute the prescan in place with neutral element 0 and return the total.

**Concept tested:** Granularity (granularnost) of tasks, Brent rule (Brentovo pravilo), block algorithm with p processors, cost of task creation.

**Solution idea:**
```csharp
static long PrescanPoRazini(long[] a, Func<long, long, long> op) =>
    PrescanStablo(a, op, (n, f) => Parallel.For(0, n, f));

static long PrescanTaskPoParu(long[] a, Func<long, long, long> op) =>
    PrescanStablo(a, op, (n, f) => Task.WaitAll(Enumerable.Range(0, n).Select(k => Task.Run(() => f(k))).ToArray()));
```
- `PrescanSlijedno` is one pass in place. For each element it saves the old value `t = a[i]`, writes the running sum `s` to `a[i]`, then updates the sum with `s = op(s, t)`. It returns `s`, the total.
- `PrescanStablo` is one tree algorithm (up-sweep and down-sweep, as in Zadatak 2) that takes the loop as a parameter `Action<int, Action<int>> petlja`. The index is `(k + 1) * d - 1` because k starts at 0. The `Parallel.For` version and the `Task.Run` version differ only in this loop.
- Block algorithm:
```csharp
int Pocetak(int b) => (int)((long)b * a.Length / p);
var zbrojevi = new long[p];
Parallel.For(0, p, b =>
{
    long s = 0;
    for (int i = Pocetak(b); i < Pocetak(b + 1); i++) { s = op(s, a[i]); }
    zbrojevi[b] = s;
});
long ukupno = PrescanSlijedno(zbrojevi, op);
Parallel.For(0, p, b =>
{
    long s = zbrojevi[b];
    for (int i = Pocetak(b); i < Pocetak(b + 1); i++)
    {
        (a[i], s) = (s, op(s, a[i]));
    }
});
return ukupno;
```
- Three phases: (1) each block computes its sum, in parallel. (2) A prescan of the p block sums gives the start value of each block. (3) Each block does a sequential prescan with its start value, in parallel.
- `SkupoZbroji` runs `for (int k = 0; k < 7000; k++) z += Math.Sqrt(k);` and returns `x + y + (z < 0 ? 1 : 0)`. The 7000 square roots give the cost of 10 to 50 µs. The result is always `x + y`, because `z` is never negative. (inferred) The term with `z` makes the loop result used, so the compiler cannot remove the loop.
- The line "jedan SkupoZbroji" times 1000 calls and prints the total in ms. Total ms for 1000 calls equals the µs of one call.
- `Izmjeri`: runs the method 4 times on a clone, returns NaN (printed as "krivo") if the result is wrong, and keeps the shortest time of runs 1-3 (run 0 is a warm-up). `PrescanTaskPoParu` is skipped for k > 18 ("preskočeno").
- Speaker note: write down a conclusion for each step ("Za svaki korak zapišite zaključak"). The `Main` output has three steps, and each step has its own questions.
- Step 1 output (ms, p = 12): 2^10: slijedno 0,00, po razini 0,06, task po paru 0,59, blokovi 0,01. 2^18: 0,34 / 0,98 / 301,53 / 0,35. 2^24: 40,90 / 47,06 / preskočeno / 11,25.
- Step 2 (n = 2^24): p = 1: 65,8 ms, S = 0,59. p = 11 (comment text; see open points): 11,3 ms, S = 3,44. p = 1.000: 9,2 ms, S = 4,23. p = 4.194.304: 27,9 ms, S = 1,40. S is the speed-up (sequential time / parallel time).
- Step 3 (n = 2^14, `SkupoZbroji`): slijedno 60,66, po razini 20,06, task po paru 23,24, blokovi 22,22. S: po razini 3,02, task po paru 2,61, blokovi 2,73.
- Starter output: "krivo" in all parallel columns, sequential shows 0,01 / 0,13 / 2,13 ms for 2^10, 2^14, 2^18.

**Reasoning:**
- `PrescanTaskPoParu` is the slowest. It creates one task per pair on each level.
- A task costs much more than one `+`. At 2^18 it takes 301,53 ms against 0,34 ms sequential.
- The cause is from the lecture; see lecture section 13.
- Only the block method is faster than sequential for the cheap `+` (11,25 against 40,90 ms at 2^24).
- It has only p tasks. Each task works on one large block with a sequential loop.
- The block method does about 2n operations, the tree does about 2(n − 1) operations in 2·log2(n) steps. (inferred; see lecture section 10)
- p = 1: the block algorithm does the work twice (sum pass and prefix pass) with no parallel gain, so S = 0,59 (below 1). (inferred)
- (inferred) With n = 2^24 and p = 4.194.304, each block has exactly 4 elements. This is the "4 elements per task" case of the 2020/21 suggestion.
- There are millions of tasks with little work each. The task overhead is large against the work, so S falls to 1,40.
- A moderate p gives the best result (p = 1.000 gives S = 4,23).
- The data show S = 3,44 at p = 11 and S = 4,23 at p = 1.000. So a p larger than the number of cores can still help a little. (inferred) Each block is then smaller and the work is shared more evenly. See lecture section 15.
- Step 3: `SkupoZbroji` takes 10 to 50 µs. One operation now costs much more than the task overhead.
- All parallel versions are faster than sequential (S from 2,61 to 3,02). The task-per-pair version is also faster than sequential.
- (inferred) This follows the Brent rule: a parallel version is faster when the work of one task is larger than the cost of the task.
- (inferred) A larger cost of one operation raises the speed-up of all three parallel versions. In step 3 the per-level version has the highest S (3,02) and the block version has S = 2,73.
- The 2020/21 suggestion ("4 elements per task instead of 2"): the block method already does this. (inferred; the source gives no answer)
- Each task handles n/p elements. For n = 2^24 and p = 12 this is about 1,4 million elements per task. For p = 4.194.304 it is exactly 4.

**Pitfall:**
- Too many tasks (task per pair, or p near n) is slower than sequential for a cheap operation.
- Too few tasks (p = 1) is also slower than sequential.
- Timing in Debug gives wrong conclusions (source: use Release). (inferred) A run without a warm-up also includes the start cost of the first call.
- `Pocetak(b)` uses `(long)b * a.Length / p`. The cast to `long` prevents overflow, and the formula handles n not divisible by p. (inferred)

**Link:** Lecture [06-algoritam-sume-prefiksa.md](06-algoritam-sume-prefiksa.md), sections 13 (trap 6), 14 (blocks), 15 (Brent rule), 16 (blocks in C# measured). Granularity: [02-flynn-granularnost-monte-carlo-pi.md](02-flynn-granularnost-monte-carlo-pi.md).

### Zadatak 4: GCD of rows and of the whole matrix   [S10]  (`Zadatak4_Nzd/Zadatak4_Nzd.cs`)
**Task statement:**
1. Write `NzdRetka(redak)`: GCD (najveći zajednički djelitelj, NZD) of a row, with the tree reduction of Zadatak 1 (you may copy it), parallel inside the level.
2. Write `NzdRedaka(m)`: rows in parallel, for each row `NzdRetka`. Write `NzdMatrice(m)` with `NzdRedaka`.
3. Write `NzdRedakaSlijednoURetku(m)`: rows in parallel, a sequential loop inside the row.
4. Both tests must print "da". Compare the times for three matrix shapes. Which version is faster and why?
- Given: `Nzd(a, b)`, a matrix generator (seed 2026), the sequential `NzdRedakaSlijedno`, two tests (3 × 6; 1000 × 1000 with all elements divisible by 3) and a measurement for 1000 × 1000, 12 × 1.000.000 and 100.000 × 10. The row length is not 2^k. Speaker note: the task is from exam terms of some years ago.
- The generator: element of row i is `g · k_i · random(1..999)`, with `k_i` from 2 to 12. Seed 2026. Test 2 uses g = 3. The timing matrices use g = 1.
- Test 1 rows: `12 18 30 42 54 6`, `35 14 21 49 7 28`, `8 20 44 12 16 4`. Row GCD values are 6, 7, 4. The GCD of the matrix is 1.
- `Najkrace(f, out tocno)` makes one run to get the correctness flag. Then it keeps the shortest time of 3 runs.

**Concept tested:** The tree reduction for a length that is not 2^k (without padding). Choice of the level of parallelism: nested parallelism against one parallel level.

**Solution idea:**
```csharp
var a = (long[])redak.Clone();
for (int d = 1; d < a.Length; d *= 2)
{
    Parallel.For(0, (a.Length + 2 * d - 1) / (2 * d), k =>
    {
        if (k * 2 * d + d < a.Length)
        {
            a[k * 2 * d] = Nzd(a[k * 2 * d], a[k * 2 * d + d]);
        }
    });
}
return a[0];
```
```csharp
Parallel.For(0, m.Length, i => rezultat[i] = NzdRetka(m[i]));              // NzdRedaka
static long NzdMatrice(long[][] m) => NzdRetka(NzdRedaka(m));               // NzdMatrice
Parallel.For(0, m.Length, i => rezultat[i] = m[i].Aggregate(Nzd));          // NzdRedakaSlijednoURetku
```
- `Nzd(a, b)` is the remainder loop `while (b != 0) { (a, b) = (b, a % b); }`. It ends with `return Math.Abs(a)`.
- Here the tree has a different index form: the pair is `a[k·2d]` and `a[k·2d + d]`, and the result goes to `a[k·2d]`. The final result is `a[0]`. The guard `if (k * 2 * d + d < a.Length)` skips pairs whose right element does not exist.
- Outputs: test 1: `NzdRedaka` 6 7 4, sequential 6 7 4, "da". `NzdMatrice` 1, sequential 1, "da". Test 2: "da" for both methods. `NzdMatrice` 3, sequential 3, "da".
- Timing (ms, 12 logical cores): 1.000 × 1.000: slijedno 3,4, NzdRedaka 8,1, SlijednoURetku 0,7. 12 × 1.000.000: 42,6 / 33,2 / 7,8. 100.000 × 10: 4,8 / 117,2 / 2,1.
- Starter output: `NzdRedaka: 0 0 0`, slijedno `6 7 4`, "ne". `NzdMatrice: 0`, slijedno 1. Test 2 `NzdMatrice: 0`, slijedno 3. Starter timing: 20,5 / 248,4 / 29,3 ms for sequential, "krivo" for the two parallel versions.

**Reasoning:**
- GCD is associative and commutative, and `gcd(0, x) = x`. So the tree works, and a missing partner can simply be skipped. No neutral element is needed because the guard avoids padding. (inferred)
- The whole matrix: the GCD of all elements is the GCD of the row GCDs. So `NzdMatrice` applies `NzdRetka` to the vector `NzdRedaka(m)`.
- `NzdRedakaSlijednoURetku` is the fastest in all three shapes. The source asks "why" only. The reasons below are (inferred).
- `Nzd` is cheap, so the work per element is small.
- The rows give enough parallelism: 1000, 12 or 100.000 rows for 12 cores.
- A sequential inner loop has no `Parallel.For` overhead per level. For 100.000 × 10 the tree version starts about 4 `Parallel.For` loops for each of the 100.000 rows. This costs 117,2 ms.
- For 12 × 1.000.000 the overhead is small (12 rows, about 20 levels). So overhead alone does not explain 33,2 ms against 7,8 ms.
- (inferred) The cost of `Nzd` depends on the values. In a sequential loop the running GCD becomes small after a few elements, so each `Nzd` call needs few steps. In the tree, the low levels join two large random values, so they need more steps. A simulation gives about 2,0 steps per element for the sequential loop and about 3,6 for the tree. (inferred; the source does not give these numbers)
- The tree version also copies each row with `Clone` and reads the row with a stride.
- `Nzd` uses `Math.Abs(a)` at the end, so the sign of the numbers does not change the result.

**Pitfall:** Nested parallelism (parallel rows, and a parallel tree inside each row) adds a large overhead when the rows are short. Parallelize at one level with enough work per task. Also, a tree for n not 2^k must have the guard, or the code reads outside the array.

**Link:** Zadatak 1 (tree reduction). Lecture section 13 (small tasks) and section 15 (Brent rule). [02-flynn-granularnost-monte-carlo-pi.md](02-flynn-granularnost-monte-carlo-pi.md) (granularity).

### Zadatak 5 (optional): binary numbers   [S11]  (`Zadatak5_Binarni/Zadatak5_Binarni.cs`)
**Task statement:**
1. Write `UDekadski(int[] bitovi)`: the value as `ulong`, in parallel (reduction or `Prescan` of Zadatak 2; you may copy it).
2. Write `SviUDekadske(int[][] brojevi)`: values of all numbers in parallel, in the same order as the input.
3. Write `Prefiksi(int[] bitovi)` with `Scan` of Zadatak 2 (you may copy it): element k is the value of the first k + 1 bits as a binary number. For `10100100`: `1 2 5 10 20 41 82 164`.
4. Which operation did you give to `Scan`? Is it associative? Is it commutative?
- Given: N bits (N = 8, 16, 32, 64; N is a power of 2), `bitovi[0]` has the highest weight. `Main` compares with `Convert.ToUInt64(s, 2)`. Exam example: `10100100` → 164, `10100101` → 165. Speaker note: the task is from an old exam, the tests go up to 64 bits.
- Test 1 numbers: `10100100`, `10100101`, `00000001`, `10000000`, `11010010`, `01101100`, `11111111`, `00110101`. Test 6 first uses `10100100`, `10100101` and `11010010`. The helper `Bitovi(string)` converts a bit string to `int[]`. Random numbers: seed 2026.
- Six tests: 1) N = 8 (8 numbers). 2, 3, 4) N = 16, 32, 64: 8 random numbers each, the first one is all ones. 5) `SviUDekadske` for 32 numbers (N = 8, 16, 32, 64). 6) `Prefiksi`, three numbers of 8 bits, then 24 numbers of N = 16, 32, 64.

**Concept tested:** Build an associative but non-commutative operation for a new problem. The prefix sum is not only for sums.

**Solution idea:**
```csharp
using Dio = (ulong v, int l);

static Dio Spoji(Dio x, Dio y) => ((x.v << y.l) | y.v, x.l + y.l);
static Dio[] Dijelovi(int[] bitovi) => bitovi.Select(b => ((ulong)b, 1)).ToArray();

static ulong UDekadski(int[] bitovi) => Prescan<Dio>(Dijelovi(bitovi), Spoji, (0UL, 0)).v;

static ulong[] SviUDekadske(int[][] brojevi)
{
    var rezultat = new ulong[brojevi.Length];
    Parallel.For(0, brojevi.Length, i => rezultat[i] = UDekadski(brojevi[i]));
    return rezultat;
}

static ulong[] Prefiksi(int[] bitovi) => Scan<Dio>(Dijelovi(bitovi), Spoji, (0UL, 0)).Select(x => x.v).ToArray();
```
- Each bit becomes a part `(value, length)` = `(b, 1)`. `Spoji` joins two parts: it shifts the left value by the length of the right part, then ORs the right value. The length is the sum of the lengths. The neutral element is `(0UL, 0)`.
- `Prescan` and `Scan` are copied from Zadatak 2 into this project.
- `UDekadski` returns the total result of `Prescan`, which is the joined value of all bits (`.v`).
- Outputs: `10100100 → 164`, `10100101 → 165`, `00000001 → 1`, `10000000 → 128`, all "da". `Prefiksi` for `10100100`: `1 2 5 10 20 41 82 164`, "da".
- The starter prints `10100100 → 0, Convert: 164, isto: ne` and `Prefiksi` all zeros.

**Reasoning:**
- The value of a bit string is the concatenation of the bit parts. Concatenation of the left part `x` and the right part `y` has value `x.v · 2^(y.l) + y.v`. The length is needed because the shift depends on how many bits the right part has. A plain number would lose this information. (inferred)
- `Spoji` is associative: joining `(x, y), z` and `x, (y, z)` gives the same bit string. It is not commutative: `Spoji(x, y) ≠ Spoji(y, x)`. This answers question 4. (inferred from the operation definition)
- `Prefiksi` is a scan (prefixes include the element itself): the prefix of length k + 1 is the joined value of the first k + 1 bits.
- `SviUDekadske` parallelizes over the numbers. Each `UDekadski` already contains a `Parallel.For`.

**Pitfall:**
- The operation is not commutative, so a wrong operand order in the down-sweep (see Zadatak 2) gives wrong prefixes.
- Test 1 with `10100100` and `10100101` is a weak test (deck question). The source does not say why. Possible reasons (inferred): N = 8 is short, and the two numbers differ only in the last bit. Values below 256 fit any integer type, so errors with overflow or with 64-bit shifts stay hidden. The all-ones numbers of N = 16, 32, 64 in tests 2-4 test the larger lengths and the full 64-bit range.

**Link:** Zadatak 2 (`Prescan`, `Scan`, order of operands). Lecture section 12 (trap 5: operand order) and section 18 (where the prefix sum is used).

### Paper task for home: n = 20, p = 8   [S12; Vjezba6_upute.html]
**Task statement:** Do a prescan with `+` on `4 7 1 0 5 2 6 4 8 1 9 7 3 0 1 5 2 7 4 3` (n = 20) with p = 8 processors. No computer.
1. Mark which processor gets which elements (n is not divisible by p).
2. Write each step in a table: sums of blocks, reduction and down-sweep on the block sums, prescan inside the blocks.
3. Check the result with sequential addition, then in `suma_prefiksa.html`, section 2 (finished example).
- Speaker note: this kind of task also comes in the exam.

**Concept tested:** The block algorithm of the lecture (slides 25-26) with n not divisible by p.

**Worked answer (inferred; the source gives no solution).** It follows the split of lecture slide 26: the first n % p = 4 processors get 3 elements.
- Blocks: P0 `4 7 1`, P1 `0 5 2`, P2 `6 4 8`, P3 `1 9 7`, P4 `3 0`, P5 `1 5`, P6 `2 7`, P7 `4 3`.
- Block sums: 12, 7, 18, 17, 3, 6, 9, 7. Total: 79 (the sum of the sums, and the sequential sum).
- Reduction on the 8 sums (8 is a power of 2, so no padding is needed):

| Step | Array of 8 sums |
|---|---|
| start | 12 7 18 17 3 6 9 7 |
| d = 2 | 12 19 18 35 3 9 9 16 |
| d = 4 | 12 19 18 54 3 9 9 25 |
| d = 8 | 12 19 18 54 3 9 9 79 |

- Down-sweep: set the last element to 0 (total 79 is saved).

| Step | Array of 8 sums |
|---|---|
| start | 12 19 18 54 3 9 9 0 |
| d = 8 | 12 19 18 0 3 9 9 54 |
| d = 4 | 12 0 18 19 3 54 9 63 |
| d = 2 | 0 12 19 37 54 57 63 72 |

- The offsets of the blocks are 0, 12, 19, 37, 54, 57, 63, 72. They equal the prescan of the block sums.
- Prescan inside the blocks:

| Block | Offset | Result |
|---|---|---|
| P0 | 0 | 0 4 11 |
| P1 | 12 | 12 12 17 |
| P2 | 19 | 19 25 29 |
| P3 | 37 | 37 38 47 |
| P4 | 54 | 54 57 |
| P5 | 57 | 57 58 |
| P6 | 63 | 63 65 |
| P7 | 72 | 72 76 |

- Final prescan: `0 4 11 12 12 17 19 25 29 37 38 47 54 57 57 58 63 65 72 76`. It equals the sequential sums shifted by one place (last sequential sum 79).
- Variant with the rule `Pocetak(b) = b · n / p` of Zadatak 3: block starts 0, 2, 5, 7, 10, 12, 15, 17. Block sums 11, 6, 8, 13, 16, 4, 7, 14. Offsets 0, 11, 17, 25, 38, 54, 58, 65. The final prescan is the same.

**Pitfall:** An exam answer must show all steps in a table. The result alone is not enough (inferred from "tablično napišite svaki korak").

### Closing questions   [S13]
The speaker note: answer them from your own code and measurement. The source gives no answers. Short answers below are inferred from the code.
1. Why does the reduction write the sum to the right and not to the left? The source gives no answer. In the solutions the sum goes to the right element (`a[k*d-1]`), and the down-sweep then reads it there. See lecture section 8.
2. How many barriers does your `Prescan` have for n = 2^20? For n = 1000? Each level ends with one barrier (the end of `Parallel.For`). The reduction has log2(m) levels and the down-sweep has log2(m) levels. n = 2^20 gives 20 + 20 = 40. n = 1000 gives m = 1024, so 10 + 10 = 20. (inferred)
3. Would `Prescan` work with one `Parallel.For` over all levels? The levels depend on each other. A level needs the finished result of the previous level. So one loop over all levels is not correct. (inferred)
4. Zadatak 3: how many tasks does `PrescanTaskPoParu` create for n = 2^18? Level d has n/d pairs. The reduction creates n − 1 tasks and the down-sweep creates n − 1 tasks. Total 2(n − 1) = 524.286. (inferred)
5. Zadatak 5: why is the exam example (`10100100` and `10100101`) a weak test? See the pitfall of Zadatak 5.

## References   [S14]
- [1] G. E. Blelloch: Prefix Sums and Their Applications. In: J. H. Reif (ed.), Synthesis of Parallel Algorithms, Morgan Kaufmann, 1993, pp. 35-60.
- [2] Zadaci iz kolegija Paralelno programiranje v1.3, PMF, 2017 (task 1.9).
- [3] Paralelno programiranje 2020/21, course materials: exercises 5 and 6, Vjezbe_profesor 8-10, exam terms.
- Speaker note: Blelloch's chapter, the PMF task collection and the course materials of 2020/21.
- Authors on the title slide: Marin Aglić Čuvić, after the lectures of Tonći Dadić. Academic year 2026/2027.

## Open points
- The note of slide 1 says the lecture showed "šest zamki" (six traps). The lecture knowledge file counts seven traps. The source does not explain the difference. (inferred) The count may differ because the lecture slides number the traps differently or because the note is old.
- The folder `Vizualizacije` (`blelloch.html`, `suma_prefiksa.html`) is not in `exercises/06-prefix-sum`. The deck cues for them could not be checked.
- The source gives no reason for "the tree is not faster" (Zadatak 1), "SlijednoURetku is faster" (Zadatak 4), "weak test" (Zadatak 5) and the closing questions. All answers here are marked (inferred).
- `Vjezba6_upute.html` shows "2^22" as "222" in the plain-text export (superscript lost). The C# code confirms `1 << 22`.
- The starter timings in `Vjezba6_upute.html` (for example 55,2 ms) differ from the solution comments (7,2 ms). They come from different runs and machines or empty methods.
- Zadatak 5: the source does not discuss a shift by 64. (inferred) In C#, `ulong << int` uses only the low 6 bits of the count, so `x.v << 64` would equal `x.v`, not 0. This does not happen because, in `Prescan`, the right operand `y` has `l` of at most N/2 = 32 or `l` = 0 (neutral element).
- Zadatak 3: the solution comment shows p = 11, but `jezgri` is 12 in the printed header (and p = `jezgri` in the code). The comment probably comes from a run on another machine. The source does not say.
- The lecture slide 26 splits n = 20 for p = 8 as 3,3,3,3,2,2,2,2 (first n % p processors get one more). `PrescanBlokovi` uses `b·n/p`, which gives 2,3,2,3,2,3,2,3. Both give correct prescans. The paper task answer follows slide 26.
- Slides 4 and 12 in the .pptx hold only text shapes (checked with python-pptx). Slide 12 shows the 20 input numbers as 20 boxes. No picture is lost.

## Coverage ledger
| Slide / source | Topic section where it is covered | Items |
|---|---|---|
| S1 title and note | Map, Setup and workflow, References, Open points (six traps against seven) | 5 |
| S2 environment setup | Setup and workflow | 10 |
| S3 what we will do | Setup and workflow (goals) | 4 |
| S4 recap (text shapes only, checked in .pptx) | Recap: scan, prescan, reduction | 9 |
| S5 traps of the day | The four traps of the day | 6 |
| S6 Zadatak 0 | Zadatak 0, Visualizations (demo cue) | 9 |
| S7 Zadatak 1 | Zadatak 1 | 10 |
| S8 Zadatak 2 | Zadatak 2 | 10 |
| S9 Zadatak 3 | Zadatak 3 (incl. `PrescanSlijedno` idea, conclusion per step), Visualizations (demo cue) | 16 |
| S10 Zadatak 4 | Zadatak 4 (incl. `Nzd` loop) | 12 |
| S11 Zadatak 5 | Zadatak 5 | 10 |
| S12 paper task (20 input boxes, text only, checked in .pptx) | Paper task for home (worked answer, 2 variants) | 10 |
| S13 closing questions | Closing questions | 5 |
| S14 references | References | 3 |
| README.md | Setup and workflow (commands, table of files) | 6 |
| Vjezba6_upute.html | Setup, all Zadatak sections, Paper task, Visualizations, Open points | 12 |
| Zadatak0..5 `.cs` files | Zadatak 0 to 5 solution ideas and outputs | 6 files |
