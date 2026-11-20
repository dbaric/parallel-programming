# Prefix Sum Algorithm (source: 06-algoritam-sume-prefiksa.md, 34 slides)

## Map
- This file covers the prefix sum (suma prefiksa), also named scan, and the Blelloch algorithm that computes it in parallel.
- Main idea: a problem that looks sequential can have a good parallel algorithm. The algorithm uses associativity (asocijativnost) and a tree.
- The lecture shows seven traps (zamke). Each trap is first an error, then a fix.
- Topic order: the problem (T1) -> a naive parallel loop fails (T2) -> a tree idea (T3) -> step-by-step algorithm and its two flaws (T4-T7) -> Blelloch up-sweep and down-sweep (T8-T10) -> traps on operation and operand order (T11-T12) -> task size, blocks and the Brent rule (T13-T16) -> padding, uses, barriers (T17-T19) -> summary (T20).
- Lecture author: Marin Aglić Čuvić, after the lectures of Tonći Dadić. Academic year 2026/2027 (slide 1).
- Notes of slide 1: the prefix sum is one of the most useful building blocks of parallel algorithms. The lecture shows seven traps on it, each first as an error, then as a fix.
- Agenda (slide 2): Problem = prefix sum, each result needs the previous one. Traps = loop dependency, data race inside a step, too much work, the operation, too small tasks. Fixes = the Blelloch tree, blocks and the Brent rule.
- Terms: "stride" = the spacing of indices (korak i in the index table). "Step" = one parallel round. "Pair" = par. "Block" = blok. "Barrier" = ograda. "Level" = razina. "Neutral element" = neutralni element. "Step" = korak. "Work" = posao (total number of operations).
- Notation: the symbol ⊕ means any associative operation. The letter I means the neutral element. The letter n is the array length. The letter p is the number of processors.

## 1. The prefix sum problem: scan, prescan, reduction   [S1-S4]
**What:** The input is an associative operation ⊕ and an array a₀, a₁, …, aₙ₋₁. The output (scan, inclusive) is a₀, a₀ ⊕ a₁, …, a₀ ⊕ a₁ ⊕ … ⊕ aₙ₋₁ [Blelloch 1993 and Dadić, refs 1 and 2].
- Three related operations. The example input is `3 1 7 0 4 1 6 3` with the operation +.

| operation | returns | result for the example |
| --- | --- | --- |
| scan (uključivi, inclusive) | a₀, a₀ ⊕ a₁, …, a₀ ⊕ … ⊕ aₙ₋₁ | 3 4 11 11 15 16 22 25 |
| prescan (isključivi, exclusive) | I, a₀, …, a₀ ⊕ … ⊕ aₙ₋₂ | 0 3 4 11 11 15 16 22 |
| reduction (reduciranje) | a₀ ⊕ … ⊕ aₙ₋₁ (one number) | 25 |

- I is the neutral element: 0 for +, 1 for ·, the smallest possible number for max [1].
- The slide highlights 15 = 3 + 1 + 7 + 0 + 4 (the fifth output).
- The operation does not have to be addition. It can be multiplication, max, or string concatenation. It can be any associative operation.
- The name "scan" comes from the language APL [2].
- Sequential code and cost: n − 1 additions, O(n). For n = 2^22 the count is 4.194.303 additions (Example 1 in the demo).
```csharp
long s = 0;
for (int i = 0; i < n; i++)
    { s += a[i]; b[i] = s; }
```
**Why:** Look at the loop. The value b[i] needs s, and s is b[i − 1]. Each step waits for the previous step. The problem looks sequential [2].
- The prescan is the primary output because the Blelloch algorithm writes the sum of everything left of position i into position i, without aᵢ. The Blelloch algorithm (slides 12-20) therefore produces a prescan and not a scan [2].
- Prescan -> scan: shift the prescan one place to the left and put the total at the end [2].
**Conclusion:** Know the three operations and the neutral element. Prescan is the natural output of the tree algorithm. Scan is a shifted prescan.
**Example:** The table above. Blelloch lists the prefix sum as a building block: radix sort, deleting marked elements, recursions, comparison of strings [2] (see topic 18).
**Pitfall:** None on these slides.
**Link:** Lecture 5 (PRAM, aPRAM): PRAM says how much parallelism a problem has. aPRAM says how much that parallelism costs (barrier, "ograda B"). Today's problem is the example of a problem that looks sequential but has a good parallel algorithm.
**Demo (notes):** Run `SumaPrefiksaDemo`, `Primjer1_Slijedno`.

## 2. Trap 1: Parallel.For with a loop dependency   [S5]
**What:** Goal: split the n iterations of the sequential loop over the cores and get the same result, but faster. The first attempt that comes to mind:
```csharp
b[0] = a[0];
Parallel.For(1, n, i =>
    { b[i] = b[i - 1] + a[i]; });
```
**Why:** Iteration i reads b[i − 1]. Another core computes this value, and it may not have finished. This is a loop-carried dependency (ovisnost iteracija, engl. loop-carried dependency). The result differs from run to run.
**Example (output, n = 2^22 = 4.194.304, five attempts):**
```
Pokušaj 1: krivih mjesta 4.162.572 od 4.194.304
Pokušaj 2: krivih mjesta 4.145.264 od 4.194.304
Pokušaj 3: krivih mjesta 4.180.476 od 4.194.304
Pokušaj 4: krivih mjesta 4.155.396 od 4.194.304
Pokušaj 5: krivih mjesta 4.142.092 od 4.194.304
```
Almost all positions are wrong, and the count is different each time.
**Conclusion (fix):** Do not parallelize this loop. Change the algorithm and use associativity. A lock inside the loop is not the fix (notes: "the fix is not a lock in the loop, but another algorithm").
**Link:** Topic 3 (the tree uses associativity). Lecture 3 and 4 (data race): the cause here is a dependency, not only a race (inferred; the slide says only "ovisnost iteracija").
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer2_ParallelForOvisnost`.

## 3. The idea: associativity and the tree   [S6]
**What:** Associativity: (a ⊕ b) ⊕ c = a ⊕ (b ⊕ c). The parentheses can move. Add in pairs, like a tournament (turnir).
- For 8 elements the depth is log₂ n = 3. At each level all pairs work at the same time. The root is the reduction (25).
- The tree for `3 1 7 0 4 1 6 3`:
  - leaves (no label on the slide): 3 1 7 0 4 1 6 3
  - "razina 0" (level 0): 4 7 5 9 (pairs 3+1, 7+0, 4+1, 6+3)
  - "razina 1" (level 1): 11 14
  - "razina 2" (level 2): 25, the root
- Numbering: the slide labels the sums, not the leaves. Level r is the level of additions with pairs 2^r apart. Topic 8 and slide 12 use the same numbering. The depth is 3 levels of additions.
**Why:** A sequential sum adds left to right. A tree adds winners of pairs, so the number of levels is only log₂ n and the pairs on one level are independent.
**Conclusion:** The tree gives the total in log₂ n steps. The open question is how to get all prefixes and not only the last one.
**Link:** Topic 4 and topic 8 answer the question in two different ways.

## 4. First attempt: step by step (Hillis and Steele)   [S7-S8]
**What:** In step r every element adds the element that is shift = 2^r places to its left [3]. The shifts are 1, then 2, then 4. The algorithm comes from the paper of Hillis and Steele (notes).
| step | a₀ | a₁ | a₂ | a₃ | a₄ | a₅ | a₆ | a₇ |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| input | 3 | 1 | 7 | 0 | 4 | 1 | 6 | 3 |
| r = 0: shift 1 | 3 | 4 | 8 | 7 | 4 | 5 | 7 | 9 |
| r = 1: shift 2 | 3 | 4 | 11 | 11 | 12 | 12 | 11 | 14 |
| r = 2: shift 4 | 3 | 4 | 11 | 11 | 15 | 16 | 22 | 25 |
```csharp
for (int pomak = 1; pomak < n; pomak *= 2)
    Parallel.For(pomak, n, i =>
        { b[i] = b[i] + b[i - pomak]; });
```
**Why (shift 2^r):** Follow a₅.
- r = 0: 4 + 1 = 5 (two elements).
- r = 1: a₂ … a₅ = 12 (four elements).
- r = 2: a₀ … a₅ = 16 (all six elements).
Each step doubles the number of elements in the sum. After log₂ n steps the element holds the complete prefix.
- The network view (slide 8): a line goes from the element that is read to the element that is computed. A vertical line means the element reads itself. A diagonal line means the element reads the element 2^r places left.
- Path to a₅ (orange): a₅ = a₅ + a₄, then + a₃ (a₃ then holds the sum a₂…a₃), then + a₁ (a₁ then holds the sum a₀…a₁). Three steps collect all six elements.
- All lines inside one step run at the same time. This matters for trap 2 (topic 6).
**Conclusion:** The method needs log₂ n steps. The slide asks: "Is this code correct?" The answer is in topics 6 and 7: it is correct on a PRAM, wrong in C# with one array, and correct but costly with two arrays.
**Demo (notes):** `Vizualizacije/suma_prefiksa.html`, section 3 (number of operations). Slide 8: section 3.

## 5. Thinking through the PRAM   [S9]
**What:** From lecture 5: a PRAM has p processors, shared memory and a shared clock (lock step).
- Use one processor per element: Pᵢ computes b[i].
- Each step has three phases for all processors: read, compute, write. Nobody writes until all have read.
- On the PRAM, ask: how many steps? How many operations in total? Who reads or writes the same cell (EREW, CREW)? Then write C#. C# has no lock step, so the programmer must provide it.
**Example:** Step r = 0 (shift 1), first three processors:
| phase | P₁ | P₂ | P₃ |
| --- | --- | --- | --- |
| read | b[0], b[1] | b[1], b[2] | b[2], b[3] |
| compute | sum | sum | sum |
| write | b[1] | b[2] | b[3] |
- P₁ and P₂ both read b[1] in the same step. This is a simultaneous read, so the model is CREW (concurrent read, exclusive write).
- On the PRAM, P₂ gets the old value of b[1], because the write happens after the read phase.
**Conclusion:** Design on the PRAM first. Then check what the lock-step guarantee gives you in C#.
**Link:** Lecture 5 (PRAM, EREW, CREW, aPRAM). Topic 6 shows what happens in C#.

## 6. Trap 2: one array inside a step   [S10]
**What:** The code of slide 7 reads and writes the same array b. It is correct on a PRAM, because all processors read before any processor writes (slide 9). It is wrong in C#, where the phases do not exist.
**Why:** P₁ writes b[1], and in the same step P₂ reads b[1] to compute b[2]. P₂ can get the old or the new value, and the result depends on who is faster. This is a data race (utrka podataka) inside the step.
**Example (trap, one array):**
```csharp
for (int p = 1; p < n; p *= 2)
    Parallel.For(p, n, i =>
        { b[i] = b[i] + b[i - p]; });
```
Output:
```
Pokušaj 1: krivih mjesta 4.194.302
Pokušaj 2: krivih mjesta 4.194.302
Pokušaj 3: krivih mjesta 4.194.302
```
(Of 2^22 = 4.194.304 positions, all except two are wrong in every attempt. The notes say "all positions except two".)
**Example (fix, two arrays):**
```csharp
// st: staro polje, nv: novo polje
for (int p = 1; p < n; p *= 2) {
  Parallel.For(0, n, i => nv[i] =
    i >= p ? st[i] + st[i - p] : st[i]);
  (st, nv) = (nv, st); }
```
Output: `krivih mjesta: 0`.
**Conclusion:** Two arrays restore the PRAM phases: all read the old array and write the new array. The end of Parallel.For is a barrier between steps (lecture 5).
**Link:** Lecture 3 and 4 (data race). Lecture 5 (barrier, aPRAM). Topic 19 (barrier between levels).
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer3_KorakPoKorak`.

## 7. Trap 3: correct, but too much work   [S11]
**What:** Compare the sequential loop and the step-by-step method with two arrays (n = 2^22).
| | steps | additions, n = 2^22 | time |
| --- | --- | --- | --- |
| sequential | n − 1 | 4.194.303 | 56,5 ms |
| step by step, two arrays | log₂ n = 22 | 88.080.385 (21 times more) | 405,3 ms |
**Why (where 88.080.385 comes from):**
- Step r adds only positions i ≥ 2^r, because earlier positions have nothing at that distance to their left. This is n − 2^r additions.
- For n = 8 (slide 7): 7 + 6 + 4 = 17 additions. The sequential method needs 7.
- General formula: (n − 1) + (n − 2) + (n − 4) + … = n log₂ n − (n − 1).
- For n = 2^22: 22 · 4.194.304 − 4.194.303 = 88.080.385. The program counted the same number.
- The count is O(n log₂ n). The sequential count is O(n). The method is not work-efficient (»work-efficient«) [3].
**Conclusion:** A small number of steps is not enough. Total work counts too. With 12 logical cores and 21 times more work, it is hard to be faster than the sequential loop. The goal is O(log n) steps and O(n) operations in total.
**Link:** Topic 8 (Blelloch reaches this goal). Topic 15 (Brent rule gives about 7,3 million for this method).
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer3_KorakPoKorak` (second part). `Vizualizacije/suma_prefiksa.html`, section 3.

## 8. Blelloch phase 1: reduction (up-sweep)   [S12-S15]
**What:** The tournament of topic 3, but in the same array. The sum of a pair is written to the position of the right child.
- On level r the pairs are 2^r apart. The left child stays in the array, because phase 2 needs it.
- For n = 8: 3 levels, and 4 + 2 + 1 = 7 additions.
- The level name is r and not d. Reason (notes): d is the cost of a global access from lecture 5.
- Pseudocode:
```
za r := 0 .. log₂ n − 1
    paralelno (za i := 0 .. n − 1, korak 2^(r+1))   // korak = stride 2^(r+1)
        a[i + 2^(r+1) − 1] := a[i + 2^r − 1] ⊕ a[i + 2^(r+1) − 1]
```
- Tree with array positions: level 0 holds 4 (a[1]), 7 (a[3]), 5 (a[5]), 9 (a[7]). Level 1 holds 11 (a[3]) and 14 (a[7]). Level 2 holds 25 (a[7]).
**Example (array in memory after each level):**
| step | a₀ | a₁ | a₂ | a₃ | a₄ | a₅ | a₆ | a₇ |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| input | 3 | 1 | 7 | 0 | 4 | 1 | 6 | 3 |
| reduction r = 0 | 3 | 4 | 7 | 7 | 4 | 5 | 6 | 9 |
| reduction r = 1 | 3 | 4 | 7 | 11 | 4 | 5 | 6 | 14 |
| reduction r = 2 | 3 | 4 | 7 | 11 | 4 | 5 | 6 | 25 |
- Inside one level each pair writes its own position, so there is no data race. At the end a₇ = 25.
- The slide highlights the cells that change on each level: r = 0: a₁, a₃, a₅, a₇. r = 1: a₃, a₇. r = 2: a₇. Each row is one level of the tree of topic 3.
**Example (indices per level):**
| r | stride i | operations (each on its own core) | count |
| --- | --- | --- | --- |
| 0 | 2 | a₁ := a₀ ⊕ a₁, a₃ := a₂ ⊕ a₃, a₅ := a₄ ⊕ a₅, a₇ := a₆ ⊕ a₇ | 4 |
| 1 | 4 | a₃ := a₁ ⊕ a₃, a₇ := a₅ ⊕ a₇ | 2 |
| 2 | 8 | a₇ := a₃ ⊕ a₇ | 1 |
- Operations: 4 + 2 + 1 = n − 1 = 7. The number of working cores falls: n/2, n/4, …, 1.
- The left operand is always the element that is left in the array. Trap 5 needs this (topic 12).
```csharp
// C#: jedan Parallel.For po razini
Parallel.For(0, n / (2 * p), k =>
{
  int l = k*2*p + p - 1, d = l + p;
  a[d] = op(a[l], a[d]);
});
```
**What the array holds after phase 1 (slide 15):**
| position | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| input | 3 | 1 | 7 | 0 | 4 | 1 | 6 | 3 |
| after phase 1 | 3 | 4 | 7 | 11 | 4 | 5 | 6 | 25 |
| cell holds | a₀ | a₀…a₁ | a₂ | a₀…a₃ | a₄ | a₄…a₅ | a₆ | a₀…a₇ |
| wanted prescan | 0 | 3 | 4 | 11 | 11 | 15 | 16 | 22 |
- The array now has the total (25) and the sums of parts of the array: each left subtree [2]. Even positions are untouched, the same as in the input.
- The prescan is not ready: position i needs the sum of everything left of i.
- Example: a₆ needs 16 = 11 (a₀…a₃) + 5 (a₄…a₅). Both parts already exist.
**Conclusion:** Phase 1 costs n − 1 operations in log₂ n steps. It leaves the building parts for the prescan.
**Link:** Phase 2 (topic 9) puts the parts together.
**Demo (notes):** `Vizualizacije/blelloch.html`, section 1 (tree and array, step by step; slide 13: the array under the tree).

## 9. Blelloch phase 2: down-sweep (spuštanje)   [S16-S19]
**What:** The root gets I. The left child copies the value of the parent. The right child = parent ⊕ the number that was in the left child [2]. The sweep goes from the root to the leaves.
- Save the total (25) before the root gets the neutral element.
- Notation on slide 16: v is the parent value, L is the left child, D is the right child. L ← v. D ← v ⊕ (old L).
**Example (array table):**
| step | a₀ | a₁ | a₂ | a₃ | a₄ | a₅ | a₆ | a₇ |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| reduction r = 2 | 3 | 4 | 7 | 11 | 4 | 5 | 6 | 25 |
| root ← 0 | 3 | 4 | 7 | 11 | 4 | 5 | 6 | 0 |
| down-sweep 1 | 3 | 4 | 7 | 0 | 4 | 5 | 6 | 11 |
| down-sweep 2 | 3 | 0 | 7 | 4 | 4 | 11 | 6 | 16 |
| down-sweep 3 | 0 | 3 | 4 | 11 | 11 | 15 | 16 | 22 |
Result: the prescan `0 3 4 11 11 15 16 22`.
**Example (tree, slides 17-18):**
- Start: the tree from the end of phase 1 (slide 12). Only the root changes: 25 -> 0. Array in memory: `3 4 7 11 4 5 6 0`. Every node holds the sum of its subtree from phase 1, for example 11 = a₀ + … + a₃.
- Why 0: nothing is left of the whole array. The total 25 is saved first.
- Reading the picture: in a node, the value that the down-sweep writes (the sum of everything left of the node). Under the left child: the copied value and the number that was in it ("bilo"). Under the right child: parent + that number (same colour).
- Level 2: left child copies 0 (it had 11). Right child = 0 + 11 = 11.
- Level 1: left of the left half: copies 0 (had 4), right = 0 + 4 = 4. Left of the right half: copies 11 (had 5), right = 11 + 5 = 16.
- Level 0: a₀ = 0 (had 3), a₁ = 0 + 3 = 3. a₂ = 4 (had 7), a₃ = 4 + 7 = 11. a₄ = 11 (had 4), a₅ = 11 + 4 = 15. a₆ = 16 (had 6), a₇ = 16 + 6 = 22.
- Orange path: a₄…a₇: 0 + 11 = 11. a₆…a₇: 11 + 5 = 16. Leaf a₇: 16 + 6 = 22.
- The leaves are the prescan, the same as the table of slide 16.
**Why it works (slide 19):** Claim [2]: after the down-sweep every node holds the sum of all leaves that precede it.
- Let the parent hold A (everything left of the parent). Let B be the sum of the left subtree.
- Everything before the left child is the same as before the parent: A. The left child copies A.
- Everything before the right child is A and then B: A ⊕ B. B is what the reduction left in the left child.
- The root holds I, because nothing is left of it. Induction from the root gives the claim (inferred; the slide does not name induction).
**Conclusion:** The reduction writes the sum to the right child on purpose: the left value must survive until the down-sweep [2].
**Link:** Topic 12 (the order A ⊕ B is the order of the operands). Exercise task 2.
**Demo (notes):** `Vizualizacije/blelloch.html`, section 2 (slide 17: first step; slide 18: the "bilo" number under the left child).

## 10. Steps and work: three methods compared   [S20]
**What:**
| | steps | operations ⊕ | n = 2^22, counted |
| --- | --- | --- | --- |
| sequential | n − 1 | n − 1 | 4.194.303 |
| step by step | log₂ n | O(n log n) | 88.080.385 |
| Blelloch | 2 log₂ n | 2(n − 1) and n − 1 zamjena ("swaps", not defined on the slide, see Open points) | 8.388.606 |
- Blelloch needs 2 log₂ n steps with n/2 processors [2].
- It does twice the operations of the sequential method, but the count is O(n): work-efficient [3].
- A barrier is needed between the levels: 2 · 22 = 44 barriers for n = 2^22. There is one barrier after each level.
- Experiment: the result equals the sequential result, 0 wrong positions (Example 4).
**Conclusion:** Blelloch gives the best balance: few steps and linear work.
**Link:** Topic 15 uses t = 44 and q = 8.388.606 in the Brent rule.
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer4_Blelloch`.

## 11. Trap 4: associativity   [S21]
**What:** The tree changes the parentheses. The operation must therefore be associative.
**Example (subtraction, `3 1 7 0 4 1 6 3`):**
- Sequential: ((3 − 1) − 7) − … = −19.
- Tree: ((3 − 1) − (7 − 0)) − ((4 − 1) − (6 − 3)) = −5.
- Subtraction is not associative, so the tree gives another result.
**Example (float, 2^22 numbers from [0, 1)):**
- sequential float: 2.097.121,0
- tree float: 2.097.140,8
- sequential double: 2.097.140,73
**Why:** Floating-point addition on a computer is not exactly associative. The result depends on the order of the additions. Here the tree is closer to the double result.
**Conclusion:** Use only associative operations. A parallel float result that differs from the sequential result is not necessarily an error. Comparing the two with "==" is an error.
**Pitfall:** Do not compare floating-point results with equality.
**Link:** Exercise task 1, test 4 (subtraction gives −5 and −19).
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer5_Asocijativnost`.

## 12. Trap 5: operand order   [S22]
**What:** The condition of the algorithm is only associativity [2]. Commutativity (komutativnost) is not needed. The algorithm never swaps the operands. It always computes (the left element in the array) ⊕ (the right element).
**Example (string concatenation, not commutative, input `a b c d`, prescan):**
| down-sweep, right child | prescan |
| --- | --- |
| D ← v ⊕ L (algorithm) | "", "a", "ab", "abc" |
| D ← L ⊕ v (swapped) | "", "a", "ab", "cab" |
**Why the second string (position 3, last down-sweep step):** The needed value is a ⊕ b ⊕ c, in this order. The parent brings v = a ⊕ b = "ab". The left child holds L = c.
- Algorithm: v ⊕ L = (a ⊕ b) ⊕ c = "abc".
- Swapped: L ⊕ v = c ⊕ (a ⊕ b) = "cab". The parts are the same, the order is different.
- The swapped code is correct only when the swap does not matter (3 + 4 = 4 + 3), for example for + and max. The experiment confirmed it: the swapped row is correct for + and max, and wrong for strings.
**Conclusion:** Commutativity is a condition for the faulty code, not for the algorithm. Always compute left ⊕ right.
**Pitfall:** This error was in the course library LIB (method `DownSweep`), because only addition was tested with it. The LIB that the students get is fixed. Test a library with an operation that is not commutative.
**Link:** Topic 8 (the left operand is always the left element). Exercise task 2, test 5 (concatenation).
**Demo (notes):** `Vizualizacije/blelloch.html`, section 3 (string concatenation, swapped operands).

## 13. Trap 6: tasks that are too small   [S23-S24]
**What:** The same Blelloch algorithm in three forms. Times in ms, the shortest of 3.
| n | sequential | Parallel.For per level | Task.Run per pair |
| --- | --- | --- | --- |
| 2^10 | 0,00 | 0,46 | 3,11 |
| 2^14 | 0,02 | 1,10 | 32,65 |
| 2^16 | 0,08 | 1,73 | 167,01 |
| 2^18 | 0,51 | 4,91 | 918,05 |
| 2^20 | 1,78 | 10,64 | — |
| 2^22 | 6,41 | 43,49 | — |
"—" means skipped (too long).
**Why:**
- Task per pair: one addition per task. The overhead (tara, cost of creating and awaiting a task) is much larger than the work. For 2^18: 918 ms against 0,51 ms. The notes say: almost a second for about 200 thousand elements, against half a millisecond.
- Per level: better, but still slower than the sequential loop. The upper levels have little work, and every level waits for all tasks.
**Conclusion (fix):** Give each task more work. Two ways: a more expensive operation (topic 13b) and p blocks instead of n/2 pairs (topic 14).
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer6_TaskPoParu`.

### 13b. Expensive operation: the tree pays off   [S24]
**What:** n = 2^14. The operation ⊕ does addition and also some extra computation. Times in ms. In brackets: speedup S = sequential time / parallel time. 12 logical cores.
| ⊕ takes | sequential | task per pair | per level | blocks |
| --- | --- | --- | --- | --- |
| 0,01 µs | 0,1 | 26,6 (0,00) | 0,9 (0,11) | 0,1 (0,83) |
| 0,46 µs | 7,6 | 31,0 (0,24) | 3,3 (2,27) | 2,9 (2,62) |
| 4,66 µs | 76,3 | 37,6 (2,03) | 29,7 (2,57) | 30,7 (2,49) |
| 46,77 µs | 765,0 | 277,4 (2,76) | 269,5 (2,84) | 273,9 (2,79) |
**Why:** When ⊕ takes long enough, the task overhead is small compared with the work. Then even the task per pair speeds up.
- Notes: at 0,01 µs all parallel methods lose. At 0,46 µs the tree already gains about 2 times. At about 40 µs even task per pair gains almost 3 times.
**Conclusion:** Granularity (granularnost) depends not only on the number of elements, but also on the cost of one operation.
**Link:** `02-flynn-granularnost-monte-carlo-pi.md` (granularity). Speedup S: lecture 3. Exercise task 3, step 3.
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer8_SkupaOperacija`.

## 14. Fewer processors: blocks   [S25-S26]
**What:** A real computer has 12 cores, not n/2 processors. Divide the array into p blocks. Three steps:
1. Each processor adds its own block of n/p elements (in parallel).
2. Prescan the p block sums. p is small, so this step can be sequential. The results are the offsets (pomaci).
3. Each processor makes a prescan of its own block, starting from its own offset (in parallel).
**Example (n = 12, p = 4, 3 elements per block; the example is from Blelloch, notes):**
- Array: `4 7 1 | 0 5 2 | 6 4 8 | 1 9 5`.
- Step 1: block sums P0: 12, P1: 7, P2: 18, P3: 15.
- Step 2: prescan of the sums `12 7 18 15` gives `0 12 19 37`. These are the offsets.
- Step 3: example P1 starts from 12 and gets 12, 12, 17.
- Final prescan: `0 4 11 12 12 17 19 25 29 37 38 47`.
**Time [2]:** T = 2(⌈n/p⌉ + ⌈lg p⌉) = O(n/p + lg p).
- n = 12, p = 4: 2(3 + 2) = 10 steps; sequential 11.
- n = 2^22, p = 12: 2(349.526 + 4) = 699.060; sequential 4.194.303.
**Example (n = 20, p = 8) [slide 26]:**
- Point of the example: n is not divisible by p. Each processor computes from its own number i where its block starts and how long it is, without agreement with the others. Block sizes differ by at most 1.
- Array: `4 7 1 0 5 2 6 4 8 1 9 7 3 0 1 5 2 7 4 3`. n / p = 2, remainder 4, so the first four processors get 3 elements each [1].
| | P0 | P1 | P2 | P3 | P4 | P5 | P6 | P7 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| block | 4 7 1 | 0 5 2 | 6 4 8 | 1 9 7 | 3 0 | 1 5 | 2 7 | 4 3 |
| block sum | 12 | 7 | 18 | 17 | 3 | 6 | 9 | 7 |
```
elem := n / p;  poc := (n / p) · i
ako n % p ≠ 0:
    ako i < n % p: elem := elem + 1
    poc := poc + min(i, n % p)
```
- P3: elem = 2 + 1 = 3 (because 3 < 4). poc = 2 · 3 + min(3, 4) = 9, so the block is a₉…a₁₁ = 1 9 7.
- P5: elem = 2 (because 5 ≥ 4). poc = 2 · 5 + min(5, 4) = 14, so the block is a₁₄…a₁₅ = 1 5.
- The rest (reduction and down-sweep over the block sums, prescan inside blocks) is the paper task of exercise 6 (the slides call it "zadatak 0", see Open points). A task like this comes on the exam (notes).
**Conclusion:** With blocks, each task has much work and the tree runs only over p sums. Each processor computes its own start and length from i.
**Link:** Topic 15 (the Brent bound is about 699.091 and the blocks give 699.060, almost equal). The paper task and `Zadatak3` (`PrescanBlokovi`).
**Demo (notes):** `Vizualizacije/suma_prefiksa.html`, section 2 (slide 26: finished example 1.9, after the exercise).

## 15. Brent rule (Brentovo pravilo)   [S27]
**What:** Lemma [4]: if a computation can be done in t steps with q operations with enough processors, then it takes at most t + (q − t) / p steps with p processors.
- Symbols: n = number of array elements. p = processors that we have. t = steps with enough processors. q = all operations (work).
**Why now:** Blelloch was designed for a PRAM with n/2 processors. The computer has 12 cores. The Brent rule gives the maximum time of any PRAM algorithm on p processors, without a new version of the algorithm.
**Example (Blelloch):**
| | value | source |
| --- | --- | --- |
| n | 2^22 = 4.194.304 | number of elements in the lecture example |
| p | 12 | logical cores of this computer |
| t | 2 · log₂ n = 2 · 22 = 44 | levels of reduction and down-sweep (slide 20) |
| q | 2(n − 1) = 8.388.606 | operations ⊕ in both phases (slide 20) |
| t + (q − t) / p | 44 + 8.388.562 / 12 ≈ 699.091 | Brent bound |
| sequential | n − 1 = 4.194.303 | for comparison |
- Blocks (topic 14): 2(⌈n/p⌉ + ⌈lg p⌉) = 699.060. This is almost equal to the bound.
- Step by step (q = 88.080.385): about 7,3 million. This is worse than sequential.
**Pitfall:** The rule does not know the task overhead (trap 6, topic 13).
**Conclusion:** Use the Brent rule to estimate time on p cores from t and q. It confirms that total work q matters, not only steps t.
**Link:** Lecture 5 (PRAM, work and steps). Exercise task 3, step 3 ("explain with the Brent rule and granularity").

## 16. Blocks in C#: measured   [S28]
**What:** n = 2^24, sequential 31,7 ms. p blocks, one Parallel.For over the blocks (shortest of 3). S = speedup = sequential time / time with p blocks (as in lecture 3).
| p | 1 | 2 | 4 | 8 | 12 | 16 | 48 | 1.000 | 100.000 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| S | 0,42 | 0,79 | 1,25 | 1,63 | 1,51 | 1,60 | 1,40 | 1,66 | 1,62 |
**Why:**
- p = 1 is more than twice slower than sequential, because the array is read twice (phase 1 for the sum, phase 3 for the prescan).
- The best S is about 1,7 with 12 logical cores.
- The prefix sum has little work per element: one addition per one read and one write. The memory access dominates (inferred from the notes; the notes state the low work per element).
- With a more expensive operation S grows (slide 24, topic 13b).
- Small n: for n = 2^10 even blocks are not faster, because the overhead of Parallel.For is larger than the whole sequential work (trap 6).
**Conclusion:** A prefix sum of cheap additions gives only a small speedup. Blocks are the best form, but the gain is limited.
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer7_Blokovi`.
**Link:** Exercise task 3, step 2 (the solution comments show another machine with higher S, see Open points).

## 17. When n is not a power of 2: padding and trap 7   [S29]
**What:** The tree needs n = 2^k [2]. Pad the array with the neutral element up to the next power of 2 [3]. The result is the first n positions. Example: n = 1000, pad to 1024.
- Blocks do not need a power of 2: block i has n/p or n/p + 1 elements [1].
**Pitfall (trap 7, padding with zero):** max over 1000 numbers from [−100, −1] with padding 0 gives max = 0. This number is not in the array. Padding with the smallest number gives max = −1.
**Conclusion (fix):** Pad with the neutral element of the operation: 0 for +, 1 for ·, the smallest number for max.
**Link:** Topic 1 (neutral element). Exercise task 2, test 4 (max with `long.MinValue`).
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer5_Asocijativnost`, part d).

## 18. Where the prefix sum is used   [S30]
| use [2] | operation |
| --- | --- |
| deleting marked elements (array compaction) | + |
| radix sort (split) | + |
| line of sight (linija pogleda) | max |
| comparison of strings | "first different" (»prvi različit«) |
| recursions, for example xᵢ = aᵢ · xᵢ₋₁ + bᵢ | on pairs (a, b) |
- On a GPU: radix sort and compaction [3].
- The students do the string comparison in exercise 6. The other uses are in the homework (notes).
**Conclusion:** The prefix sum is a building block. Different uses only change the operation ⊕.
**Link:** Topic 1. Slides 30 and 33 say the string comparison is done in exercise 6. No such project exists in the exercise folder (see Open points). `Zadatak5` (binary numbers) is another non-commutative operation (inferred).

## 19. Link to the models: the barrier   [S31]
**What:** Trap: Parallel.For over the levels. Fix: Parallel.For per level.
```csharp
// trap
Parallel.For(0, razina, r =>
{
    int p = 1 << r;
    for (int i = 0; i < n; i += 2 * p)
        a[i + 2*p - 1] += a[i + p - 1];
});
```
```csharp
// fix
for (int p = 1; p < n; p *= 2)
    Parallel.For(0, n / (2 * p), k =>
    {
        int l = k * 2 * p + p - 1;
        a[l + p] += a[l];
    });
```
**Example (output):** correct sum 207.616.572.
```
preko razina, pokušaj 1:     204.141
preko razina, pokušaj 2:   6.034.845
preko razina, pokušaj 5:   8.537.133
po razini:               207.616.572
```
**Why:** In the trap the levels run at the same time. Level r + 1 reads a sum that level r has not yet written. The total is wrong and different every time.
- EREW PRAM: the algorithm works because nobody shares a cell on one level [2].
- aPRAM: a barrier is needed after each level. In C# the end of Parallel.For is the barrier.
**Conclusion:** Parallelize inside a level, never across levels.
**Link:** Lecture 5 (list head, glava liste: the same trap). Topic 6.
**Demo (notes):** `SumaPrefiksaDemo`, `Primjer9_BezOgrade`.

## 20. Summary of the seven traps, exercise plan, references   [S32-S34]
| Trap | How it shows | Fix | Topic |
| --- | --- | --- | --- |
| Loop dependency | Parallel.For: almost all positions wrong | other algorithm (tree) | 2 |
| One array inside a step | step by step: all positions wrong | two arrays or Blelloch | 6 |
| Too much work | 21 times more additions than sequential | reduction + down-sweep: 2(n − 1) | 7 |
| Operation not associative | subtraction: −5 instead of −19 | only associative operations | 11 |
| Reversed operands | "cab" instead of "abc" | left ⊕ right, always | 12 |
| Tasks too small | task per pair: 918 ms against 0,51 ms | blocks (Brent), fewer tasks | 13 |
| Padding with zero | max = 0 over negative numbers | pad with the neutral element | 17 |
- Notes: "Seven traps, seven fixes." The operation must be associative and the operand order must stay.
**Exercise 6 plan (slide 33):**
- Paper task (the slides say "zadatak 0"): prescan n = 20 with p = 8.
- Reduction with a tree (and what happens with subtraction).
- Prescan and scan for any operation and n that is not a power of 2.
- Three forms and measurement: task per pair, Parallel.For per level, blocks.
- Which string is first in the alphabet.
- Visualization `Vizualizacije/suma_prefiksa.html`: reduction and down-sweep step by step, blocks, number of operations, operand order. It helps to check what your code does, step by step.
**References (slide 34):**
1. T. Dadić: Algoritam sume prefiksa, lecture, 2025/26.
2. G. E. Blelloch: Prefix Sums and Their Applications. In: J. H. Reif (ed.), Synthesis of Parallel Algorithms, Morgan Kaufmann, 1993, pp. 35-60.
3. M. Harris, S. Sengupta, J. D. Owens: Parallel Prefix Sum (Scan) with CUDA. GPU Gems 3, ch. 39, NVIDIA.
4. R. P. Brent: The Parallel Evaluation of General Arithmetic Expressions. Journal of the ACM 21(2), 1974, 201-206.
5. Zadaci iz kolegija Paralelno programiranje v1.3, PMF, 2017 (the 20-element task is from this collection; slide 34 notes).
- Notes on sources: main sources are the Blelloch chapter, the GPU Gems 3 chapter and the Brent paper.

## Exercise cross-reference
Folder: `/Users/drazenbaric/Projects/parallel-programming/exercises/06-prefix-sum/`. Details, outputs and solution code are in `ex-06-prefix-sum.md`. Do not copy them here. Target framework: .NET 10. Run: `dotnet run -c Release` in the task folder. This section gives only the concept link.

| task | concept | key insight | what goes wrong if ignored |
| --- | --- | --- | --- |
| `Zadatak0_Indeksi/Zadatak0_Indeksi.cs` | Tree indices (topic 8). | Level with stride d uses positions k·d − 1. For n = 32 there are 5 rows. Same pairs, run sequentially. | For n = 16 the sum is 136 (correct). For n = 20 the result is 74, not 210, because the tree needs n = 2^k (topic 17). |
| `Zadatak1_Reduciranje/Zadatak1_Reduciranje.cs` | Reduction, Parallel.For per level, associativity (topics 8, 11). | One Parallel.For per level. Array after reduction: `3 4 7 11 4 5 6 25`. Test 2 (n = 2^22): tree not faster than sequential (comment: 7,2 ms and 9,3 ms). | Test 4 (subtraction): −5 against −19 (topic 11). Reason for the slow tree: little work per element (inferred from topics 13 and 16). |
| `Zadatak2_Prescan/Zadatak2_Prescan.cs` | Full prescan, padding, operand order, scan from prescan (topics 9, 12, 17, 1). | Pad with the neutral element. Save the total, then set the root to the neutral element. Down-sweep: `b[right] = op(b[right], lijevo)`. Scan = prescan shifted, plus the total. Expected outputs (comments): test 1 prescan `[0] [3] [4] [11] [11] [15] [16] [22]`, scan `[3] [4] [11] [11] [15] [16] [22] [25]`. Test 5 prescan `[] [a] [ab] [abc] [abcd] [abcde]`, scan `[a] [ab] [abc] [abcd] [abcde] [abcdef]`. All five tests print "da". | Swapped operands break test 5 (the concatenation); the wrong order is the trap of topic 12 (the "cab" result is the 4-letter slide example, not test 5). Padding with 0 breaks test 4 (max, neutral `long.MinValue`). A length that is not 2^k breaks test 3 (n = 1000). |
| `Zadatak3_Granularnost/Zadatak3_Granularnost.cs` | Three forms, Brent rule, granularity (topics 13-16). | One tree code with a pluggable loop gives per level and task per pair. Blocks: sums in parallel, sequential prescan of the p sums, prescan of each block from its offset. The measurement ignores the first run (warm-up) and prints "krivo" for a wrong result. Step 1 comments: 2^10: 0,00 0,06 0,59 0,01 (sequential, per level, task per pair, blocks); 2^18: 0,34 0,98 301,53 0,35; 2^24: 40,90 47,06 skipped 11,25. Step 2: p = 1: 65,8 ms S 0,59; p = 11: 11,3 ms S 3,44; p = 1.000: 9,2 ms S 4,23; p = 4.194.304: 27,9 ms S 1,40. `SkupoZbroji` does 7000 square roots (10 to 50 µs, the program prints the value). Step 3: sequential 60,66, per level 20,06, task per pair 23,24, blocks 22,22; S 3,02, 2,61, 2,73. | Task overhead is larger than the work (trap 6). p = 1 reads the array twice. A very large p makes tiny blocks and the overhead returns (inferred). The 2020/21 "4 elements per task" question: blocks already give n/p elements per task (inferred). |
| `Zadatak4_Nzd/Zadatak4_Nzd.cs` | Reduction with gcd (NZD) and granularity. | Tree reduction per row, rows in parallel; compare with a sequential loop inside each row. Expected: test 1 `6 7 4` and matrix gcd 1; test 2 gcd 3. Times (ms; sequential, NzdRedaka, SlijednoURetku): 1.000×1.000: 3,4 8,1 0,7; 12×1.000.000: 42,6 33,2 7,8; 100.000×10: 4,8 117,2 2,1. The sequential loop in a row is fastest. | A tree inside each row gives many small tasks (inferred: nested parallelism). gcd is associative and commutative (inferred), so the operand order does not matter here. |
| `Zadatak5_Binarni/Zadatak5_Binarni.cs` (optional) | Non-commutative associative operation, scan (topics 12, 1). | Each bit is a piece `(v, l)`. `Spoji(x, y) = ((x.v << y.l) \| y.v, x.l + y.l)`, neutral `(0, 0)`. `Prefiksi` of `10100100` gives `1 2 5 10 20 41 82 164`. Six tests: N = 8 (examples 164, 165, 1, 128, …), then N = 16, 32, 64 with the first number all ones, then `SviUDekadske`, then `Prefiksi`. The exam example: `10100100` -> 164, `10100101` -> 165 (upute). | The operation is join of bit strings: associative, not commutative (inferred). Swapped operands give a wrong value (inferred). |
| Paper task (home, no project) | Blocks with n not divisible by p (topic 14). | Blocks from the formula: the first 4 processors get 3 elements. Block sums `12 7 18 17 3 6 9 7`. Offsets (own recompute: prescan of the sums) `0 12 19 37 54 57 63 72`. Check: total 79 = 72 + 7. Check with the sequential sum and `suma_prefiksa.html`, section 2. | Wrong block sizes break the offsets. The task comes on the exam. |

## Open points
- Slides 6, 12, 17 and 18 hold tree pictures. A judge checked the node values, index labels and "bilo" labels against the pptx shapes. They match.
- Slide 7 asks "Je li ovaj kod točan?". The slide does not give the answer in one place. Topics 6 and 7 answer it (inferred: correct on a PRAM, wrong in C#, correct with two arrays).
- Slide 16 line "Zbroj 25 spremljen je prije nego što korijen dobije 0" has no code on the slide. Exercise task 2 shows the code (`ukupno = b[m-1]`).
- The speedup values differ between slide 28 (best S about 1,7, n = 2^24) and the solution comments of `Zadatak3` (S up to 4,23). The exercise text says only that numbers change from run to run, on a 6-core, 12-logical-core macOS machine. The reason for the difference is unknown.
- The block formula on slide 26 (first n % p processors get one more element) differs from `PrescanBlokovi` in the solution (`start = b·n/p`). Both give block sizes that differ by at most 1.
- The speaker notes of slide 28 say p = 1 reads the array twice. The slide does not state why S stays near 1,7 beyond "little work per element". I marked the memory explanation as inferred.
- Slide 21 does not state the tree order used for the float sum. The numbers are taken as given.
- The exercise intro deck `Vjezba6 - prezentacija 2026-27.pptx` was not read. Another stream can cover it.
- The slides call the paper task "zadatak 0" (slides 26, 33). The exercise folder uses `Zadatak0_Indeksi` for tree indices. This file says "paper task" for the first meaning.
- Slides 30 and 33 say that string comparison is done in exercise 6. The exercise folder has no string-comparison project.
- Slide 20 says "n − 1 zamjena" and does not define it. (Inferred: the pairs "left copies, right sums" of the down-sweep.)

## Coverage ledger
| slide | topic section where it is covered | items |
| --- | --- | --- |
| 1 | Map (author, year, building block, seven traps) | 4 |
| 2 | Map (agenda), topic 1 (Link), topic 18 | 6 |
| 3 | Topic 1 | 8 |
| 4 | Topic 1 | 6 |
| 5 | Topic 2 | 6 |
| 6 | Topic 3 (level labels checked in the pptx) | 6 |
| 7 | Topic 4 | 7 |
| 8 | Topic 4 | 5 |
| 9 | Topic 5 | 7 |
| 10 | Topic 6 | 8 |
| 11 | Topic 7 | 8 |
| 12 | Topic 8 | 7 |
| 13 | Topic 8 | 6 |
| 14 | Topic 8 | 6 |
| 15 | Topic 8 | 5 |
| 16 | Topic 9 | 5 |
| 17 | Topic 9 | 4 |
| 18 | Topic 9 | 7 |
| 19 | Topic 9 | 5 |
| 20 | Topic 10 | 6 |
| 21 | Topic 11 | 6 |
| 22 | Topic 12 | 8 |
| 23 | Topic 13 | 6 |
| 24 | Topic 13b | 6 |
| 25 | Topic 14 | 10 |
| 26 | Topic 14 | 8 |
| 27 | Topic 15 | 8 |
| 28 | Topic 16 | 7 |
| 29 | Topic 17 | 5 |
| 30 | Topic 18 | 6 |
| 31 | Topic 19 | 6 |
| 32 | Topic 20 | 7 |
| 33 | Topic 20 | 5 |
| 34 | Topic 20 (references) | 5 |
