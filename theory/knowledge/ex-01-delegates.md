# Exercise 1: Delegates and Lambda Expressions (source: ex-01-delegates-deck.md, 18 slides; also README.md, Vjezba1_upute.html, Vizualizacije/delegati.html, Zadatak1..5 C# files)

## Map
- This file covers the intro deck of exercise 1, the instruction page, the interactive page, and the five tasks.
- Main idea: a delegate (delegat) is a type whose value is a reference to a method. A method can then be a variable and an argument of another method.
- Parallel programming in C# depends on functions that receive other functions (`Task.Run`, `Parallel.For`). For this reason exercise 1 starts with delegates.
- The topics connect in this order: delegate, multicast, `Action`/`Func`/`Predicate`, lambda, closure, higher-order function and LINQ, generic class.
- Closure (zatvaranje nad varijablom) is the link to exercise 2: a lambda that runs in another task can read a variable that has changed. See `ex-02-parallel.md`.
- The exercise accompanies lecture 1 (`01-uvod-u-paralelno-programiranje.md`). The notes of slide 1 say it follows that lecture.

## Course context and setup   [S1-S4]
**What (S1):** Title: "Paralelno programiranje, Vježba 1: delegati i lambda izrazi". Author: Marin Aglić Čuvić, based on lectures by Tonći Dadić. Academic year 2026/2027. Notes: exercise 1, week 1. Student instructions are in `Vjezba1_upute.html`.

**What (S2, setup in three steps):**
1. Install the .NET 10 SDK from dotnet.microsoft.com/en-us/download. The command `dotnet --list-sdks` must print version 10.x.
2. Select an environment: Visual Studio 2026 (.NET workload), JetBrains Rider, or Visual Studio Code with the C# Dev Kit extension.
3. Open and run a project.
- Visual Studio and Rider: open `Vjezba1.sln`. VS Code: File > Open Folder, the project folder.
- Run with F5 or `dotnet run`.
- Measure time in the Release configuration, not in Debug.
- Only Visual Studio 2022? S2 says: in the `.csproj` file change `net10.0` to `net8.0` (also in `.vscode/launch.json` for VS Code). The tasks work the same. The README and the instruction page say `Directory.Build.props` instead (see Open points).
- The notes: each project has a `README.md` with the steps and a `.vscode` folder with run configurations (Debug and Release). Check that the classroom has the .NET 10 SDK. If not, use `net8.0`.

**What (instructions page and README):**
- Each task is a separate project in the solution `Vjezba1.sln`. A task runs alone, without a menu. A task and its starting statements are in one file.
- The README gives these ways to run a task:
  - Docker (recommended in this repository), from the repository root: `docker compose run --rm dotnet dotnet run --project exercises/01-delegates/Zadatak1_Delegati`.
  - HTML pages: `docker compose up web`, then http://localhost:8080/01-delegates/Vjezba1_upute.html and http://localhost:8080/01-delegates/Vizualizacije/delegati.html.
  - Visual Studio: open `Vjezba1.sln`, right-click the task project, Set as Startup Project, then F5 or Ctrl+F5.
  - Rider: File > Open, select `Vjezba1.sln`, select the task project in the run configuration list (top right).
  - VS Code: Open Folder, install C# Dev Kit (or C#), F5, configuration "Vježba 1: zadatak N (Debug)".
  - Command line: `dotnet run` in the task folder, or `dotnet run --project Zadatak1_Delegati` from the exercise folder.
- `Directory.Build.props` holds the settings for all task projects: `OutputType` Exe, `TargetFramework` `net10.0`, `ImplicitUsings` enable, `Nullable` enable. The `.csproj` files only contain a comment that says the settings are in `../Directory.Build.props`.
- Because `ImplicitUsings` is enabled, the `.cs` files have no `using` lines. The `System`, `System.Linq` and `System.Collections.Generic` namespaces are available (inferred from the standard .NET SDK behavior). For this reason `List<T>`, `Where`, `Select` and `Count` work without `using`.
- The projects target .NET 10. With Visual Studio 2022 only, change `net10.0` to `net8.0` in `Directory.Build.props` (shared file for all tasks) and in the paths of `.vscode/launch.json`.
- The instruction page says: after the exercise the student can pass a function as an argument. This is a precondition for `Task.Run` and `Parallel.For` in the next weeks.
- The instruction page says for task 5: also read the section about the generic class `Stog<T>` in `delegati.html` (folder "Vizualizacije").
- Every `.cs` file starts with `Console.OutputEncoding = System.Text.Encoding.UTF8;`. Cause: the output has Croatian letters (inferred).

**What (S3, passing the course):**

| Part | Share | Rule |
|---|---|---|
| Attendance (prisutnost) | 70 % | At lectures and at exercises, for each separately. Without 70 % attendance there is no signature. |
| Homework (domaće zadaće) | 20 % | Two homework tasks. They are independent work. Tasks and deadlines come later. |
| Exams (ispiti) | 80 % | Two exams. |
- The exercises have no grade. The system records only attendance.
- The notes: this replaces the old way (4 homework tasks, an oral exam through Teams). The split inside 20 % and 80 % (for example 10 + 10 and 40 + 40) is not confirmed. For this reason it is not on the slide.
- The instruction page repeats it: attendance is at least 70 % of the exercise sessions. Two homework tasks are graded.

**What (S4, plan of exercises, changes possible):**

| No. | Topic | Week |
|---|---|---|
| 1 | Delegates, Action, Func, lambda expressions, closure | 1 |
| 2 | Tasks, speedup, first traps | 2 |
| 3 | Parallel.For, speedup factor, data race | 3-4 |
| 4 | Cancelling tasks, prefix sum | 5-6 |
| 5 | Message passing: sending, patterns, deadlock | 9-11 |
| 6 | Producer-consumer and message queue | 12 |
| 7 | GPU and a distributed example | 13-14 |
- The notes: the plan agrees with the weekly plan of the introduction lecture. Week 7 is a recap. Weeks 8 and 15 are exams. From week 3 the plan is approximate. The message passing tool is not yet chosen.
**Link:** `01-uvod-u-paralelno-programiranje.md` has the weekly plan of the lecture.

## What C# knowledge the course expects   [S5]
**What:**

| Known from before | New in this course |
|---|---|
| Object-oriented programming | Delegates, Action, Func |
| Classes, generic types, collections | Lambda expressions and closure |
| LINQ queries | Task, Parallel.For, thread pool |
**Why:** Parallel programming in C# depends on functions that receive other functions.
**Conclusion:** The exercise starts with delegates because the main APIs, such as `Task.Run` and `Parallel.For`, take a delegate (inferred from S5 and S11).
**Pitfall:** The notes of slide 12 say generic classes and methods were not explained in the old presentation. Task 5 needs them. The deck lists them under "known" but explains them on slide 12 (see the Open points).

## Delegate: a type for a method   [S6, task 1, HTML section 1]
**What:** A delegate (delegat) is a type. Its value is a reference to a method. The signature of the delegate decides which methods fit: the same parameter list and the same return type.
**Why:** Types such as `int`, `string`, `bool` hold data (inferred). A delegate type holds a method. Then the program can store the method in a variable and give it to another method.
**Conclusion:** A variable of a delegate type can point to any method with the same signature. Because of this, a method can be an argument.
**Example (S6):**
```csharp
delegate int Calc(int x);

static int Pomnozi(int x)
    => x * 50;
Calc racun = Pomnozi;
int r = racun(100);   // 5000
```
- The keyword is `delegate`. The type name is `Calc`.
- The notes link this slide to task 1 and to `Vizualizacije/delegati.html`, section 1.
**Link:** Task 1 below. `Task.Run(posao)` follows the same idea: the library knows how to run the job, the caller says which job (see HTML section 2).

## Multicast delegate (invocation list)   [S7, task 1c, HTML section 1]
**What:** A delegate variable keeps a list of methods (lista poziva, invocation list). The operator `+=` adds a method to the end of the list. A call runs all methods in the list, in order, with the same argument. The call returns the value of the last method.
**Why:** The delegate has one return value. The values of the earlier methods are discarded without a message.
**Conclusion:** A multicast delegate returns only the last value. The call returns the value of the last method: 5000 in the example.
**Example (S7):**
```csharp
Calc oboje = Oduzmi;
oboje += Pomnozi;

int r = oboje(100);   // ?
```
- Check: `oboje.GetInvocationList().Length` is 2.
- The notes: this is task 1c (instruction). The notes say that the single returned value is a frequent source of wrong expectations.
**Rules from the HTML page (section 1):**
- Button `+=` adds a method at the end of the list. A method can be in the list more than once.
- `-=` removes the last occurrence of the method. If the method is not in the list, nothing changes.
- `racun = null` or removing all methods leaves an empty list. An empty delegate is `null`.
- The page shows the last C# statement above the list (for example `racun += Pomnozi;`) with a comment: "uklonjena zadnja pojava", "te metode nema u listi: ništa se ne mijenja", or "lista je prazna: racun je null".
- The page shows a log with the result of each method (for example `Oduzmi(100) = 50`), the label `GetInvocationList().Length = k`, and a field "Vrijednost koju vraća poziv delegata". The log shows all results. The field shows only the last result. This is the demo conclusion.
- Demo steps: add `Oduzmi` a second time, then press "−= Oduzmi": the last occurrence goes. Then press "−= Pomnozi": the method goes from the middle of the list. Also press "−=" for a method that is not in the list: nothing changes.
- An empty list shows "prazna (null)" and the text that the call would throw `NullReferenceException` (hence `racun?.Invoke(x)`). The input must be an integer from −2147483648 to 2147483647. Other input gives an error message.
- Input x = 100000000: `Pomnozi` overflows the range of `int`. The page computes as 32-bit `int`: overflow without exception, integer division that drops the decimals.
- The page starts with the list `Calc racun = Oduzmi;`.
**Pitfall (HTML "Problem"):**
- All methods run, but the delegate returns only the result of the last one. The others are lost without a message.
- A call of an empty delegate (`null`) throws `NullReferenceException`.
- For tasks: if one job you gave fails, or its result is lost, you often do not notice it at once.
**Solution (HTML "Rješenje"):** When all results are needed, walk the list by hand with `GetInvocationList()`. Protect an empty delegate with `racun?.Invoke(x)`.
**Link:** The question "What happens if a multicast delegate has a method that throws an exception?" is in the thinking questions below.

## Action, Func and Predicate   [S8, task 4, HTML section 4]
**What:** The language has built-in generic delegates. A custom delegate type is not necessary.

| Type | Returns | Meaning of the type arguments |
|---|---|---|
| `Action` | nothing | no parameters |
| `Action<int>` | nothing | one `int` parameter |
| `Func<...>` | a value | the last type is the return type |
| `Func<int, int, bool>` | `bool` | two `int` parameters, returns `bool` |
| `Predicate<int>` | `bool` | one `int` parameter |
- The notes add two more examples: `Action` without parameters, and `Func<int>` without parameters that returns `int`.
- HTML table:

| Type | Returns | Example from the exercise |
|---|---|---|
| `Action<...>` | nothing | `Action<string> pozdrav = ime => Console.WriteLine(ime);` |
| `Func<..., R>` | R | `Func<int, int, int> zbroj = (a, b) => a + b;` |
| `Predicate<T>` | bool | `Predicate<int> pozitivan = x => x > 0;` |
- HTML section 4: select the number of parameters (0-3) and the return type. The page shows the matching type. 1 parameter and return type `bool` gives `Predicate<int>`. 0 parameters and no return value gives plain `Action`.
- The page builds the line with the variable names `akcija`, `uvjet`, `funkcija`, the parameter names a, b, c, and the parameter types `int`, `string`, `double` in this order. 0 parameters with a return type gives `Func<R>`. Two or three parameters with return type `bool` give `Func<..., bool>`.
- The page shows this note: `Predicate<T>` is the same as `Func<T, bool>`, only shorter.
**Why:** The choice depends on two facts: the number of parameters and the return type.
**Conclusion:** Use `Action` for no result, `Func` for a result (the last type argument), `Predicate` for a `bool` test of one value.
**Link:** Task 4 below.

## Lambda expression: a function without a name   [S9, task 3, HTML section 3]
**What:** A lambda expression (lambda izraz) is a function without a name. The form is `(parametri) => izraz`.
**Why:** You write it at the place where you use it. It replaces a method that you would otherwise have to name. The compiler most often infers the parameter types.
**Conclusion:** For more than one statement, use `{ }` and `return`.
**Example (S9):**
```csharp
(parametri) => izraz
() => 4
(a, b) => a + b
x => x % 2 == 0
s => Console.WriteLine(s)

() =>
{
    int a = 5;
    return a + 2;
}
```
- The notes: this slide joins four old slides (anonymous functions, the form of the lambda, examples, the multi-statement body). It is task 3. HTML section 3 is the interactive part.
**Link:** Task 3 below.

## Closure: a lambda captures a variable   [S10, task 3, HTML section 3]
**What:** A lambda does not copy the value of an outer variable. It keeps the variable itself.
**Why:** Because the lambda keeps the variable, a change of the variable after the lambda is created changes the result of the next call.
**Conclusion:** A lambda reads the current value of the variable at the time of the call, not the value at the time of creation.
**Example (S10):**
```csharp
int faktor = 3;
Func<int, int> puta = x => x * faktor;

Console.WriteLine(puta(5));   // 15
faktor = 10;
Console.WriteLine(puta(5));   // 50
```
- HTML section 3 uses the lambda `x => x * faktor` and the type `Calc3` (the same name as in task 3). The log prints "puta(5) = ... (lambda čita trenutni faktor = k)". Move the slider (faktor 1-10), press "Pozovi puta(5)", change the slider, press again. The same call gives a different result. The button "Usporedi s kopijom vrijednosti" shows that a copy of the value (3) would give 5 * 3 = 15. The closure gives 5 * the current factor.
**Pitfall:**
- The `for` loop has one variable for all iterations. A lambda in the loop body that goes to another task sees the changed variable. This is trap A of exercise 2.
- The notes: `foreach` captures a new variable for each iteration (from C# 5). `for` does not.
- Rule on the slide: when a lambda goes to another task, store the value in a local variable inside the loop body (`int j = i;`) and use that variable.
- HTML "Problem": if another party (a loop, another thread) changes the variable, the lambda sees the change. With tasks the result depends on when the task starts. You do not control this.
- HTML "Solution": if you need the value at the time of creation, store it in a local variable and use it in the lambda.
**Link:** `ex-02-parallel.md` (trap A). The HTML page links to `Tjedan 02 - Taskovi i zamke/Vizualizacije/zamke.html`, section 1. The notes of S10 cite the same page. Data races in general: `04-data-race-lock-interlocked.md`.

## Higher-order functions and LINQ   [S11, HTML section 2]
**What:** A higher-order function (funkcija višeg reda) receives a function as a parameter or returns a function. `Task.Run` and `Parallel.For` are such functions. LINQ is a library for queries on collections. Most of its methods receive a delegate.
**Example (S11):**
```csharp
int[] niz = { 5, -2, 9, 0, -7, 4 };

niz.Where(x => x > 0)
niz.Select(x => x * x)
niz.Aggregate((a, b) => a + b)
```
- Names: Where = Filter, Select = Map, Aggregate = Reduce.
- The notes link to HTML section 2 (the condition as a delegate, as in `Where`).
**HTML section 2 (universal function):** One function `Ispisi` processes the array. The delegate `Uvjet` decides what it prints. Green elements meet the condition. The others are skipped. The function does not change. Only the behavior that you give it changes. The same pattern is in `Task.Run(posao)`. The library knows how, you say what.
- The page has conditions: `x % 2 == 0`, `x > 10`, `x % 3 == 0`, `x < 8`. The default array is `3, 8, 12, 15, 20, 7, 4`. The array input is comma separated. Each cell shows the label "ispis" (condition true) or "preskok" (skipped). It shows "Ispisano elemenata: k od m".
**Link:** Task 2 and task 4 below (the LINQ lines `Where` and `Select` are in the task 4 code).

## Generic class: Stog<T>   [S12, task 5, HTML section 5]
**What:** `T` is a marker for a type that you choose when you use the class. The same code works for `int`, `string` or any other type. A stack (stog) works by the rule last in, first out: `Stavi` adds on top, `Skini` takes from the top. Methods of a generic class can receive delegates, for example `Action<T>` or `Predicate<T>`.
**Example (S12):**
```csharp
class Stog<T>
{
    List<T> elementi = new();
    public void Stavi(T x) { ... }
    public T Skini() { ... }
}

var brojevi = new Stog<int>();
brojevi.Stavi(10);
var rijeci = new Stog<string>();
rijeci.Stavi("paralelno");
static void Primijeni<T>(T[] niz, Action<T> akcija)
```
- The last line is a generic method that receives a delegate. The student does something similar in task 5, but inside `Stog<T>`.
- The notes: generic classes and methods were not explained in the old presentation. They are needed for task 5.
**HTML section 5:** `ZaSvaki(Action<T>)` goes from the top to the bottom and calls the action on each element. `Broji(Predicate<T>)` counts the elements that meet the condition. The task of the method (what to do with an element) is a delegate, so one class serves many purposes. The page starts with the stack 30, 12, 45 (top 45). Buttons: Stavi (default value 30), Skini, Broji, ZaSvaki. The top element has the label "vrh". The log shows `Stavi(v)`, `Skini() = v`, `Broji(x => ...) = k` and `ZaSvaki: ...` (top to bottom). Conditions: `x > 20`, `x % 2 == 0`, `x < 10`.
**Pitfall (HTML "Problem"):** `Skini()` on an empty stack must not return a wrong value without a message. The log shows `InvalidOperationException: Stog je prazan.` The exercise throws `InvalidOperationException`. Test it: pop until the stack is empty.

## Exercise tasks
All five tasks are console programs. Each starts with `Console.OutputEncoding = System.Text.Encoding.UTF8;` and calls `Pokreni()` of a static class. The instruction page `Vjezba1_upute.html` repeats the task statements and the sample outputs of S13-S17 word for word. The code blocks below are abridged: they show key lines, and some `Console.WriteLine` lines and comments are left out or shortened. The notes of S13-S17 say: the output is an example, not a measurement. The notes say the solution is in the folder "Rjesenja" (teacher). In this repository the solutions are in the `Zadatak*` files below.

### Task 1: delegates (S13)   `exercises/01-delegates/Zadatak1_Delegati/Zadatak1_Delegati.cs`
**Statement:** Given: delegate `Calc(int x)` and the methods `Oduzmi` and `Pomnozi` (constant n = 50).
1. Add the method `Podijeli` that returns `x / n`. Call it through the delegate.
2. Make one delegate that runs `Oduzmi` and `Pomnozi` at a call. Call it with 100.
3. Print `oboje.GetInvocationList().Length`. Which value does the call return, and why?

**Expected output:**
```
Oduzmi(100)  = 50
Pomnozi(100) = 5000
Podijeli(100) = 2
oboje(100) = 5000
Broj metoda u listi poziva: 2
```
**Concept:** Delegate type, assignment of a method group to a variable, multicast with `+=`, return value of the last method.
**Solution idea (key lines):**
```csharp
delegate int Calc(int x);
const int n = 50;
static int Oduzmi(int x) => x - n;
static int Pomnozi(int x) => x * n;
static int Podijeli(int x) => x / n;

Calc racun = Oduzmi;
racun = Pomnozi;
racun = Podijeli;

Calc oboje = Oduzmi;
oboje += Pomnozi;
// Poziv vraća vrijednost zadnje metode u listi poziva
Console.WriteLine($"oboje(100) = {oboje(100)}");   // oboje(100) = 5000
Console.WriteLine($"Broj metoda u listi poziva: {oboje.GetInvocationList().Length}");   // 2
```
**Reasoning:** One variable `racun` points to one method at a time. An assignment replaces the target. The operator `+=` does not replace. It appends. The list then has two methods. A call runs `Oduzmi(100)` = 50, then `Pomnozi(100)` = 5000. The delegate has one return value, so it returns 5000, the value of the last method. The value 50 is lost.
**Pitfall:**
- The student may expect 50, or both values (inferred). The call returns only 5000.
- `racun = Pomnozi;` (with `=`) replaces the list. Only `+=` extends it.
- `Podijeli` uses integer division: 100 / 50 = 2. For other inputs the decimals are dropped (HTML section 1).
- Order matters: the call returns the value of the method that was added last (inferred from the rule).
**Link:** Section "Multicast delegate" above.

### Task 2: universal function (S14)   `exercises/01-delegates/Zadatak2_UniverzalnaFunkcija/Zadatak2_UniverzalnaFunkcija.cs`
**Statement:** Given: delegate `Uvjet`, function `Ispisi(niz, uvjet)`, conditions `JeParan` and `JeVeciOdDeset`.
1. Write the condition `JeDjeljivSTri`. Call `Ispisi` with it.
2. Change `Ispisi` so that it returns the number of printed elements. Print that number.

**Expected output:**
```
Parni: 8 12 20 4
Veći od 10: 12 15 20
Djeljivi s 3: 3 12 15
Ispisano elemenata: 3
```
**Concept:** A delegate as a parameter. One function, many behaviors.
**Solution idea (key lines):**
```csharp
delegate bool Uvjet(int x);

static bool JeParan(int x) => x % 2 == 0;
static bool JeVeciOdDeset(int x) => x > 10;
static bool JeDjeljivSTri(int x) => x % 3 == 0;

static int Ispisi(int[] niz, Uvjet uvjet)
{
    int broj = 0;
    foreach (int x in niz)
        if (uvjet(x))
        {
            Console.Write(x + " ");   // 8 12 20 4 za prvi poziv
            broj++;
        }
    Console.WriteLine();   // novi redak
    return broj;
}

int[] niz = { 3, 8, 12, 15, 20, 7, 4 };
Console.Write("Parni: ");
Ispisi(niz, JeParan);
Console.Write("Veći od 10: ");
Ispisi(niz, JeVeciOdDeset);
Console.Write("Djeljivi s 3: ");
int ispisano = Ispisi(niz, JeDjeljivSTri);
Console.WriteLine($"Ispisano elemenata: {ispisano}");   // 3
```
**Reasoning:** `Ispisi` knows how to walk the array. The delegate `uvjet` says which element to print. The counter `broj` increases only when `uvjet(x)` is true. The return type changes from `void` to `int`. The first two calls ignore the returned value. The third call stores it. Elements 3, 12, 15 are divisible by 3, so the count is 3.
**Pitfall:**
- The student adds a new condition method and does not copy `Ispisi` for each condition (inferred).
- `Console.WriteLine();` after the loop ends the output line. The label `Console.Write("Parni: ")` is written before the call, so the order of the text depends on the order of the statements (inferred).
- The method signature must match `bool (int)`. A method with other parameters does not fit `Uvjet`.
**Link:** Section "Higher-order functions and LINQ". HTML section 2.

### Task 3: lambda expressions (S15)   `exercises/01-delegates/Zadatak3_Lambda/Zadatak3_Lambda.cs`
**Statement:** Given: the lambdas `kvadrat` and `zbroji`, and an example with a closure (variable `faktor`).
1. Run the program. Why does the first call `puta(5)` give 15 and the second 50?
2. Write a lambda for the absolute value without `Math.Abs`. Print `abs(-8)`.
3. Write a lambda without parameters that prints the current time. Call it (you need `Action`, see task 4).

**Expected output:**
```
kvadrat(7) = 49
zbroj 1..10 = 55
puta(5), faktor = 3: 15
puta(5), faktor = 10: 50
abs(-8) = 8
Trenutno vrijeme: 17:56:03
```
(The time is an example. The code comment shows 16:26:51.)
**Concept:** Lambda forms (expression body and statement body), closure, `Action` without parameters.
**Solution idea (key lines):**
```csharp
delegate int Calc3(int x);

Calc3 kvadrat = x => x * x;
Calc3 zbroji = x =>
{
    int s = 0;
    for (int i = 1; i <= x; i++) s += i;
    return s;
};

int faktor = 3;
Calc3 puta = x => x * faktor;
Console.WriteLine($"puta(5), faktor = 3: {puta(5)}");   // 15
faktor = 10;
Console.WriteLine($"puta(5), faktor = 10: {puta(5)}");   // 50, zašto je rezultat drukčiji?
// Lambda pamti varijablu faktor a ne njezinu vrijednost

Calc3 abs = x => x < 0 ? -x : x;
Action vrijeme = () => Console.WriteLine($"Trenutno vrijeme: {DateTime.Now:HH:mm:ss}");
vrijeme();
```
**Reasoning:**
- `zbroji(10)` = 1 + 2 + ... + 10 = 55. The body has several statements, so it uses `{ }` and `return`.
- The lambda `puta` keeps the variable `faktor`, not the value 3. After `faktor = 10`, the second call computes 5 * 10 = 50.
- `abs` uses the conditional operator: `x < 0 ? -x : x`. For -8 the result is 8.
- `vrijeme` has no parameter and no result, so its type is `Action`.
**Pitfall:** The student may think the lambda copied 3. It did not. The comment in the code says the same: the lambda keeps the variable. This causes wrong results with tasks (see closure section and `ex-02-parallel.md`).
**Note on names:** The file declares `Calc3`, not `Calc`. The reason is not in the source (see Open points).

### Task 4: generic delegates (S16)   `exercises/01-delegates/Zadatak4_GenerickiDelegati/Zadatak4_GenerickiDelegati.cs`
**Statement:** Given: examples with `Func`, `Action`, `Predicate`, and the array `{ 5, -2, 9, 0, -7, 4 }`.
1. Write a lambda of type `Func<double, double, double>` that computes the hypotenuse (hipotenuza) of a right triangle with legs a and b (`Math.Sqrt`). Call it with 3 and 4.
2. Use `Array.FindAll` and `Predicate<int>` to find the even numbers of the array. Print them.
3. Write a lambda of type `Action<int[]>` (receives an array of integers, returns nothing) that prints all elements separated by a space in one line. Store it in a variable. Call it with the array.

**Expected output:**
```
Hipotenuza(3, 4) = 5
Parni: -2, 0, 4
5 -2 9 0 -7 4
```
**Concept:** `Func`, `Action`, `Predicate` with type arguments. A lambda passed to a library method (`Array.FindAll`).
**Solution idea (key lines):**
```csharp
Func<int, int, int> zbroj = (a, b) => a + b;                 // zbroj(3, 4) = 7
Action<string> pozdrav = ime => Console.WriteLine($"Bok, {ime}!");   // Bok, svijete!
Predicate<int> pozitivan = x => x > 0;                       // pozitivan(-5) = False

Func<double, double, double> hipotenuza = (a, b) => Math.Sqrt(a * a + b * b);
// Hipotenuza(3, 4) = 5

int[] niz = { 5, -2, 9, 0, -7, 4 };
int[] parni = Array.FindAll(niz, x => x % 2 == 0);
Console.WriteLine("Parni: " + string.Join(", ", parni));   // Parni: -2, 0, 4

Action<int[]> ispisi = a => Console.WriteLine(string.Join(" ", a));   // 5 -2 9 0 -7 4
ispisi(niz);
Console.WriteLine("Pozitivni (LINQ Where): " + string.Join(", ", niz.Where(x => x > 0)));   // 5, 9, 4
Console.WriteLine("Kvadrati (LINQ Select): " + string.Join(", ", niz.Select(x => x * x)));   // 25, 4, 81, 0, 49, 16
```
- (inferred) The first three lines and the two LINQ lines are probably the given examples. The student writes the hypotenuse, `FindAll` and `ispisi`. The statement says only: given are examples with `Func`, `Action` and `Predicate`.
- The code header comment of the file says: `Action<T...>` is a method that returns no value. `Func<T..., R>` is a method that returns a value of type R (the last type). `Predicate<T>` is a method that returns `bool`.
**Reasoning:**
- `Func<double, double, double>`: two `double` parameters, the last type is the return type. The Pythagorean formula is `Math.Sqrt(a * a + b * b)`. For 3 and 4 the result is 5.
- `Array.FindAll(niz, ...)` receives a `Predicate<int>`. The lambda `x => x % 2 == 0` fits it. The even numbers are -2, 0, 4. The value 0 is even and -2 is even, because `% 2` gives 0 for them. (In C# the remainder of a negative even number is 0.)
- `Action<int[]>` has one parameter (the array) and no result. `string.Join(" ", a)` makes one line.
**Pitfall:**
- Choosing the wrong delegate type: a method that returns a value is not an `Action`. The last type of `Func` is the result, not a parameter.
- The test for even numbers must also accept 0 and negative numbers (expected output `-2, 0, 4`).
- Without `Console.WriteLine` around `string.Join`, nothing prints (inferred).
**Link:** Section "Action, Func and Predicate". Section "Higher-order functions and LINQ".

### Task 5: generic class (S17)   `exercises/01-delegates/Zadatak5_GenerickaKlasa/Zadatak5_GenerickaKlasa.cs`
**Statement:** Given: class `Stog<T>` with the methods `Stavi`, `Skini` and the property `Velicina`.
1. Add the method `ZaSvaki(Action<T>)`. It calls the action for each element, from the top to the bottom.
2. Add the method `Broji(Predicate<T>)`. It returns the number of elements that meet the condition.
3. On a stack of words, print the elements with `ZaSvaki`. On a stack of numbers (after `Skini`), count the elements larger than 20.

**Expected output:**
```
Skinuto: 50, veličina: 4
programiranje
paralelno
Brojeva većih od 20: 2
```
**Concept:** Generic class, a delegate as a parameter of a method of the class, method group as a delegate (`Console.WriteLine`).
**Solution idea (key lines):**
```csharp
class Stog<T>
{
    readonly List<T> elementi = new();

    public int Velicina => elementi.Count;

    public void Stavi(T x) => elementi.Add(x);

    public void ZaSvaki(Action<T> akcija)
    {
        for (int i = elementi.Count - 1; i >= 0; i--) akcija(elementi[i]);
    }

    public int Broji(Predicate<T> uvjet) => elementi.Count(x => uvjet(x));

    public T Skini()
    {
        if (elementi.Count == 0) throw new InvalidOperationException("Stog je prazan.");
        T x = elementi[^1];
        elementi.RemoveAt(elementi.Count - 1);
        return x;
    }
}

var brojevi = new Stog<int>();
for (int i = 1; i <= 5; i++) brojevi.Stavi(i * 10);
Console.WriteLine($"Skinuto: {brojevi.Skini()}, veličina: {brojevi.Velicina}");   // Skinuto: 50, veličina: 4

var rijeci = new Stog<string>();
rijeci.Stavi("paralelno");
rijeci.Stavi("programiranje");
rijeci.ZaSvaki(Console.WriteLine);   // programiranje / paralelno

Console.WriteLine($"Brojeva većih od 20: {brojevi.Broji(x => x > 20)}");   // 2
```
**Reasoning:**
- The list stores the elements. The top of the stack is the end of the list. For this reason `ZaSvaki` goes from index `Count - 1` down to 0 (top to bottom).
- `Skini` takes `elementi[^1]` (the last element), then removes it. It throws `InvalidOperationException` when the list is empty.
- `Broji` wraps the `Predicate<T>` in a lambda for `Count`: `elementi.Count(x => uvjet(x))`.
- The numbers are 10, 20, 30, 40, 50. `Skini` returns 50 and the size is 4. The remaining numbers larger than 20 are 30 and 40, so the count is 2.
- `rijeci.ZaSvaki(Console.WriteLine)` passes a method directly as the `Action<string>`. It prints "programiranje" first because it is on top.
**Pitfall:**
- If `ZaSvaki` goes from index 0 up, the order is wrong: bottom to top (inferred from the statement "od vrha prema dnu").
- The count must be made after `Skini`. Before `Skini` the answer would be 3 (30, 40, 50) (inferred from the numbers).
- `Skini` on an empty stack must throw an exception. It must not return a default value without a message (HTML section 5).
**Link:** Section "Generic class: Stog<T>".

## Thinking questions (HTML page)
The page lists three questions. The source gives no answers. All three answers are (inferred) from the sections above:
1. Why does `Task.Run` receive a delegate and not a finished result? (inferred) The library decides how and when to run the job. A result would mean the job already ran in the caller.
2. What happens if a multicast delegate has a method that throws an exception? The source gives no answer. (inferred) The exception leaves the call, so the methods later in the list do not run. Verify this before you rely on it.
3. What is the difference between passing a value and passing a reference to a variable? (inferred) A value is a copy at the time of the call. A reference to a variable (closure) shows the current value at every use. See the closure section.

## References
- [1] Microsoft Learn: Delegates, learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/
- [2] Microsoft Learn: Lambda expressions, learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions
- [3] Microsoft Learn: foreach and variable capture, learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements
- The HTML page lists the reference [1] only.

## Open points
- S5 lists "generic types" as known, but S12 and its notes say generic classes were not explained before. The two statements differ.
- The notes of S13-S17 refer to a folder "Rjesenja (nastavnik)". It is not in `exercises/01-delegates/`. The solutions are in the `Zadatak*` files.
- `Zadatak3_Lambda.cs` declares `Calc3`, while task 1 declares `Calc`. The source gives no reason for the different name.
- The time in the S15 sample (17:56:03) differs from the code comment (16:26:51). Both are examples.
- The deck shows `Aggregate` on S11, but no task code uses `Aggregate`.
- The HTML thinking questions have no answers in the source. The answers above are inferred.
- The pptx check found no picture shapes on any slide. The text export is complete.
- `Vizualizacije/zajednicko.js` only builds the navigation bar (links to `index.html`, `paralelizam.html`, `delegati.html`, `amdahl.html` and the page of week 2) and defines the helper `$` for `getElementById`. It teaches nothing. The pages `index.html`, `paralelizam.html` and `amdahl.html` are not in this exercise folder.
- S2 says to change `net10.0` in the `.csproj` file. The README and the instruction page say to change it in `Directory.Build.props`. The `.csproj` files contain no `TargetFramework`, so `Directory.Build.props` is correct (inferred from the files).
- The slide text export shows the layout of S2, S3, S4 and S13-S17 as flat text. The column order in the tables above follows the text order.

## Coverage ledger

| Slide | Topic section where it is covered | Items |
|---|---|---|
| 1 | Course context and setup | 4 (title, author, year, notes link to lecture and instructions) |
| 2 | Course context and setup | 9 (3 steps, 3 environments, open/run, Release rule, net8.0 rule, notes about README, .vscode, classroom SDK) |
| 3 | Course context and setup | 6 (70 %, 20 %, 80 %, signature rule, notes on old method, unconfirmed split) |
| 4 | Course context and setup | 8 (7 exercises with weeks, notes on weeks 7, 8, 15 and the messaging tool) |
| 5 | What C# knowledge the course expects | 3 (known list, new list, reason sentence) |
| 6 | Delegate: a type for a method | 6 (definition, signature rule, variable use, code, keyword note, link note) |
| 7 | Multicast delegate | 6 (`+=`, order, last value 5000, `GetInvocationList`, notes on task 1c and wrong expectations, link) |
| 8 | Action, Func and Predicate | 7 (5 type rows, notes with extra examples, link) |
| 9 | Lambda expression | 7 (form, 5 example lambdas, multi-statement, four rules, notes about merged slides) |
| 10 | Closure | 8 (code, copy rule, reason, loop trap, local-variable rule, foreach note, links) |
| 11 | Higher-order functions and LINQ | 6 (definition, `Task.Run`/`Parallel.For`, LINQ, three calls, mapping Filter/Map/Reduce, notes link) |
| 12 | Generic class: Stog<T> | 8 (class code, `T`, same code for all types, LIFO, delegates in methods, usage code, generic method, notes) |
| 13 | Task 1 | 6 (given, 3 steps, output, notes) |
| 14 | Task 2 | 5 (given, 2 steps, output, notes) |
| 15 | Task 3 | 6 (given, 3 steps, output, notes) |
| 16 | Task 4 | 6 (given, 3 steps, output, notes) |
| 17 | Task 5 | 6 (given, 3 steps, output, notes) |
| 18 | References | 3 (three links) |
| delegati.html sections 1-5 | Multicast (section 1), Higher-order functions (section 2), Closure (section 3), Action/Func/Predicate (section 4), Generic class (section 5), Thinking questions | 5 demos with page behavior, 3 "Problem" boxes, 2 "Solution" boxes, 3 questions |
| zajednicko.js | Open points | 1 (navigation and helper only) |
| Directory.Build.props and .csproj files | Course context and setup | 5 (four settings, empty csproj) |
| Instructions page and README | Course context and setup, Exercise tasks | 5 run ways, net8.0 rule, 5 task statements and outputs, submission rule |
| Task files 1-5 | Exercise tasks | key lines of each file (abridged), all comments with outputs, 5 solution ideas |
