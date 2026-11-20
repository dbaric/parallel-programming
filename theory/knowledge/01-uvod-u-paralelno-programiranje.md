# Introduction to Parallel Programming (Uvod u paralelno programiranje) (source: 01-uvod-u-paralelno-programiranje.md, 48 slides)

## Map
- This file covers lecture 1. Part A is the course rules (S1-S13). Part B (S14-S48) covers parallel execution, the reasons to use it, the hardware, the design steps, the Amdahl law, and the problems of parallel programs.
- The main idea: a single core no longer becomes faster by itself. A programmer must divide the work so that many cores run it at the same time.
- The division has a cost. The cost appears as waiting, as extra work, as a sequential part, and as errors (data race, deadlock).
- The topics link in this order: why (Moore, frequency wall) -> where (levels, architectures) -> how (design steps, library) -> limit (Amdahl) -> risk (what goes wrong).
- Exercise 1 (delegates) and exercise 2 (tasks) test the first tools. The cross-reference section lists them.

## Course organization (Organizacija kolegija)   [S1-S13]
**What:**
- Course: "Paralelno programiranje". Lecturer on the slides: Marin Aglić Čuvić. The lectures follow the lectures of dr. sc. Tonći Dadić. Year: 2026/2027.
- Credits and hours: 5 ECTS, 30 hours of lectures, 30 hours of exercises. This is two hours per week for each kind.
- Prerequisite: Objektno programiranje (object-oriented programming). The exercises use C#. The student must know classes, methods and basic collections.
- Course owners (Nositelji): prof. dr. sc. Marko Rosić and dr. sc. Tonći Dadić. Associate (Suradnik): Marin Aglić Čuvić, mag. educ. inf.
- Goal: understand how a program runs in parallel, and gain the knowledge and skills to build programs that use parallel algorithms.
- Learning outcomes (S4):
  1. Explain the models of parallel program execution.
  2. Explain process, thread, data race (utrka za podatke), critical section (kritični odsječak), synchronization and deadlock (potpuni zastoj).
  3. Apply the Amdahl law to estimate the speedup (ubrzanje) of a program.
  4. Build simple parallel algorithms without help.
  5. Understand more advanced parallel algorithms and apply them to given problems.
  6. Implement and evaluate parallel programs.
- The S4 slide text says "nadmetanje za podatke". The note says the course uses "data race (utrka za podatke)" for the same thing. This file uses "data race" only.
- Outcomes 1-3 are understanding. Outcomes 4-6 are skills. The exams test both.
- Two parts of the course (S5). Each part ends with an exam.

| Part 1: parallel programs in C# (shared memory) | Part 2: message passing model |
|---|---|
| Delegates and lambda expressions | Message passing model (model razmjene poruka) |
| Tasks, Parallel.For, Amdahl law | Actors (aktori), MPI, producer-consumer |
| Data race: lock and Interlocked | Communication patterns |
| Computer models (RAM, PRAM) | Deadlock, order, overload |
| Task cancellation | Queue in a process and broker (Pub/Sub) |
| Prefix sum algorithm | GPU and large language models |
| Deadlock, semaphores, collections | Distributed systems |
| Exam 1 | Exam 2 |

- Attendance (S6): at least 70 % of lectures and at least 70 % of exercises. The course counts them separately. Attendance is the condition for the signature (potpis).
- 70 % of 14 sessions is at least 10 sessions. A student cannot replace missed exercises with lectures. The card reader records the student at the room entrance with an "X-ica".
- Grading (S7): two homework assignments (domaće zadaće), 10 % each, total 20 %. Two exams, 40 % each, total 80 %.
- The homework gives the student a chance to write a program alone, without hurry, and to measure it. The exams test understanding and problem solving.
- Exam rules (S8):
  - Exam 1 covers part 1. Exam 2 covers part 2. The pass mark is 50 % of the points on each exam.
  - A student who fails one part takes only that part, in a practical or a written exam. The pass mark is again 50 % for the part.
  - A passed exam or part stays valid until the end of the academic year.
  - A student who enrols again in the next academic year takes the whole exam.
- Literature (S9): D. Jakobović, "Predavanja iz kolegija Paralelno programiranje", FER, Zagreb (2015); the lecture and exercise materials; interactive HTML views for the lectures. The HTML views work without internet. The student opens them after the lecture and tries them.
- Tools (S9): C# and .NET 10 (LTS). The student chooses one of Visual Studio 2026, JetBrains Rider, or Visual Studio Code (dotnet CLI). The tool for message passing in part 2 is chosen later.
- Setup (S13):
  1. Install the .NET 10 SDK from dotnet.microsoft.com/en-us/download. Check with `dotnet --list-sdks`. The output must show version 10.x.
  2. Choose the environment: Visual Studio 2026 (the .NET workload), JetBrains Rider, or Visual Studio Code with the C# Dev Kit extension.
  3. Open and run a task. Visual Studio and Rider: open `Vjezba1.sln`. Each task is a separate project. Choose the project (Set as Startup Project) and press F5.
  4. VS Code: Open Folder, configuration "zadatak N". Terminal: `dotnet run` in the task folder.
- Each task is a separate project in the same solution, so the student runs it alone. The README of the exercise describes the steps (S13 note). The student chooses the environment.
- Only Visual Studio 2022? Change `net10.0` to `net8.0` in `Directory.Build.props` (and in `.vscode/launch.json` for VS Code). The tasks work the same.
- Weekly plan (S10, lecture and exercise):

| Week | Lecture | Exercise |
|---|---|---|
| 1 | Introduction: parallel execution and its problems | Delegates and lambda expressions |
| 2 | Flynn taxonomy, granularity, Monte Carlo π | Tasks, speedup, first pitfalls |
| 3 | Amdahl law and programming models | Parallel.For and speedup factor |
| 4 | Data race: lock and Interlocked | Data race and its fixes |
| 5 | Computer models: RAM, PRAM, aPRAM | Task cancellation |
| 6 | Prefix sum algorithm | Prefix sum algorithm |
| 7 | Deadlock, semaphores, concurrent collections; revision | Recap |
| 8 | Exam 1 | Exam 1 |
| 9 | Message passing: actors, MPI, producer-consumer | Sending and receiving messages |
| 10 | Communication patterns: point-to-point and collective | Communication patterns |
| 11 | Message passing problems: deadlock, order, overload | Deadlock and message order |
| 12 | Producer-consumer: queue in a process and broker (Pub/Sub); actors | Producer-consumer and message queue |
| 13 | GPU and large language models | GPU programming overview |
| 14 | From one machine to distributed systems; revision | Distributed example (demo) |
| 15 | Exam 2 | Exam 2 |

- The structure matters, not each row. Weeks 1-7 are part 1. Week 8 is exam 1. Weeks 9-14 are message passing. Week 15 is exam 2. The plan can change a little during the semester.
- Exercises by week (S11):

| Week | Exercise | What the students do |
|---|---|---|
| 1 | Delegates and lambda expressions | Delegates, Action and Func, closure, generic Stog<T> |
| 2 | Tasks, speedup, first pitfalls | Paralelno(f, g), sum on k tasks, speedup; four pitfalls |
| 3 | Parallel.For and speedup factor | Measure speedup and efficiency for different N |
| 4 | Data race and its fixes | Lost updates and ways to protect shared data |
| 5 | Task cancellation | Stop a long job on user request |
| 6 | Prefix sum algorithm | Parallel prefix sum in C# |
| 7 | Recap | Repeat exercises 1-6 before exam 1 |
| 8 | Exam 1 | (empty) |
| 9 | Sending and receiving messages | First program with messages (tool still to choose) |
| 10 | Communication patterns | Point-to-point and collective communication |
| 11 | Deadlock and message order | A program that stalls and its fix |
| 12 | Producer-consumer and message queue | Bounded queue, many producers and consumers, end of work |
| 13 | GPU programming overview | First GPU program or a demonstration |
| 14 | Distributed example (demo) | Producer-consumer through a broker |
| 15 | Exam 2 | (empty) |

- Pattern of the exercises: in the first two weeks the students build tools (delegates, lambda expressions, tasks, speedup measurement). From week 3 each exercise first causes a problem (for example a data race or a deadlock) and then fixes it. In part 2 the students write programs with messages. The last weeks are an overview and a demonstration.
- Link between lecture and exercise (S12):

| Week | Lecture | Exercise | Link | Level |
|---|---|---|---|---|
| 1 | Introduction | Delegates and lambda expressions | Task and Parallel.For take a lambda | direct |
| 2 | Flynn, granularity, Monte Carlo π | Tasks, speedup, first pitfalls | Granularity = task size | partial |
| 3 | Amdahl law | Parallel.For and speedup factor | Measured speedup against the limit | direct |
| 4 | Data race: lock and Interlocked | Data race and fixes | Same problem, fix in code | direct |
| 5 | Computer models: RAM, PRAM | Task cancellation | No direct link | none |
| 6 | Prefix sum algorithm | Prefix sum algorithm | PRAM algorithm made in C# | direct |
| 7 | Deadlock, semaphores, collections | Recap | Collections are practised in week 12 | partial |
| 9 | Message passing: overview | Sending and receiving messages | First program with messages | direct |
| 10 | Communication patterns | Communication patterns | Same patterns in code | direct |
| 11 | Message passing problems | Deadlock and message order | Cause a deadlock and fix it | direct |
| 12 | Producer-consumer, broker, actors | Producer-consumer and message queue | Queue in a process; broker in the lecture | partial |
| 13 | GPU and large language models | GPU programming overview | Depends on room equipment | partial |
| 14 | Distributed systems | Distributed example (demo) | Demonstration, not a task | partial |

- Direct link means: what the lecture explains, the student writes in code in the exercise. Example: granularity from lecture 2 is the task size in exercise 2. The Amdahl limit from lecture 3 is the value that the student compares with the measured speedup. The legend has three levels, shown by cell colour on the slide: direct link (izravna veza), partial (djelomična), no link (nema veze). Direct: weeks 1, 3, 4, 6, 9, 10, 11. Partial: weeks 2, 7, 12, 13, 14. No link: week 5. Week 5 has no link: the lecture is about computer models and the exercise is about task cancellation.

**Why:**
- The organization part is short. It still sets what the course expects from the student during the semester (inferred from the speaker note of S2).
- The course has two parts. The memory model changes between them (inferred as the reason for the split). In part 1 all threads share memory. In part 2 processes have no shared memory and send messages. The same problems, deadlock and order, return in a different form.
- Attendance is counted separately for lectures and for exercises. A student cannot replace missed exercises with lecture attendance.

**Conclusion:** Part 1 = shared memory on one computer in C#. Part 2 = message passing. The grade is 20 % homework and 80 % exams. The signature needs 70 % attendance for lectures and 70 % for exercises, separately.

**Pitfall:**
- Measure time only in the Release configuration, not in Debug. Debug switches off optimizations, so the measurement has no meaning (S13 note).
- A student cannot make up missed exercises with lecture attendance (S6 note).

**Link:** The lecture ends by linking to exercise 1 (S1 note, S46). Exercise 1 and 2 (see the cross-reference section). Lecture 2 file: `02-flynn-granularnost-monte-carlo-pi.md`.

## Definition and goal of parallel programming   [S14-S15]
**What:**
- Parallel programming (paralelno programiranje, engl. parallel programming): many computations or processes execute at the same time. The word "process" here is probably used in the general sense, not as an operating system process (inferred).
- A large problem is divided into smaller problems. The smaller problems can execute at the same time.
- The goal is a speedup of the computation: get the result in less time.
- Lecture 1 answers three questions: what is parallel execution, why every programmer needs it today, and what can go wrong.

**Why:** The idea is simple: divide a large problem into parts and solve the parts at the same time.

**Conclusion:** The goal is less time. The division has a cost, and not every job can be divided. The whole course is about this limit.

**Example:** Worker analogy from the teacher. One worker packs 1000 boxes in 1000 units of time. Four workers, each with its own pile, finish in about 250 units. The key word is "about" (oko): dividing the work has a cost.

**Pitfall:** A job that cannot be divided gives no gain from more workers (S15 note, S29 note).

**Link:** Amdahl law (topic "Amdahl law"). Dependencies between parts (topic "Sequential program, parallel program, parallel problem").

## Why now: Moore law and the frequency wall   [S16, S18, S19]
**What:**
- Moore law (Mooreov zakon): the number of transistors on a chip doubles about every two years.
- Physical limit (fizikalna granica): a higher clock frequency means higher power use and more heat. From about the middle of the 2000s the frequency grows very slowly.
- More cores (više jezgri): the extra transistors go into new cores on the same chip. The program must run in parallel to use them.
- Message of the slide: a faster program no longer comes by itself from a faster processor.

**Why:**
- For decades the designers used the extra transistors to raise the frequency. The same program ran faster each year without a change. Around the middle of the 2000s this stopped because of power and heat. The programmer must now write the program so that it uses the cores.
- This is why every programmer needs parallel programming now, not only the programmers of supercomputers.

**Conclusion:** The performance of one core grows slowly since about 2003. Extra performance comes from more cores. The program must use them.

**Example (S18, calculated curve, logarithmic scale, single-core performance relative to 1978 = 1.0, inferred from the first chart value):**
- Source: J. Hennessy, D. Patterson, "Computer Architecture: A Quantitative Approach", 6th ed. (2019). The curve is a reconstruction from average yearly rates.
- Yearly growth by period:

| Period | Growth per year |
|---|---|
| 1978-1986 | about 22 % |
| 1986-2003 | about 52 % |
| 2003-2011 | about 23 % |
| 2011-2015 | up to 12 % |
| from 2015 | about 3.5 % |

- On a logarithmic scale a constant percentage growth is a straight line. A growth of 52 % per year doubles the performance about every 20 months.
- Around 2003 the curve bends: power and heat do not allow a higher frequency. The extra performance since then comes from more cores.
- The dashed line shows where performance would be if the 52 % growth had continued. The difference between the dashed line and reality is today supplied by parallelism.
- Series values (year: actual single core; the dashed series starts in 2003 with the same value):
```
1978: 1.0     1979: 1.22    1980: 1.49    1981: 1.82    1982: 2.22
1983: 2.7     1984: 3.3     1985: 4.02    1986: 4.91    1987: 7.46
1988: 11.34   1989: 17.23   1990: 26.2    1991: 39.82   1992: 60.53
1993: 92.0    1994: 139.84  1995: 212.55  1996: 323.08  1997: 491.09
1998: 746.45  1999: 1134.61 2000: 1724.6  2001: 2621.39 2002: 3984.52
Year: actual single core / dashed (52 % growth continued)
2003: 6056.47 / 6056.47     2004: 7449.46 / 9205.84
2005: 9162.83 / 13992.87    2006: 11270.29 / 21269.16
2007: 13862.45 / 32329.13   2008: 17050.82 / 49140.27
2009: 20972.51 / 74693.22   2010: 25796.18 / 113533.69
2011: 31729.3 / 172571.21   2012: 35536.82 / 262308.24
2013: 39801.24 / 398708.52  2014: 44577.39 / 606036.96
2015: 49926.67 / 921176.17  2016: 51674.11 / 1400187.78
2017: 53482.7 / 2128285.43  2018: 55354.59 / 3234993.85
```

**Example (S19, measured data):**
- Each point is one processor and its result on the SPECint test with one thread, from 1988 to 2021. Karl Rupp collected the data and published it freely: K. Rupp, "Microprocessor Trend Data" (2022), CC BY 4.0 [16].
- Growth of the best processors per year:

| Period | Growth per year |
|---|---|
| 1988-2003 | about 40 % |
| 2003-2011 | about 17 % |
| 2011-2015 | about 9 % |
| 2015-2021 | about 8 % |

- The rates are the best result near the first and near the last year of the period (± 1 year).
- The measured data confirm the bend around 2003. The difference to the dashed line (the old pace) is today more than 100 times.
- The data stop in 2021. The SPEC benchmark was replaced by a new version with mostly new tests, so newer results are not comparable with old results. SPEC CPU2026 has 16 new tests of 19 integer tests [17].
- Same message: one core no longer grows fast enough.
- Chart data (read with python-pptx). The scatter series "Izmjereni procesori" has 74 points. The year runs from 1988.65 to 2021.3. The SPECint value runs from 50.0 (minimum) to 105300.0 (maximum). The first points are 83.5, 50.0, 55.7, 90.6. The last points are 99000.0, 97200.0, 105300.0. The file does not list the middle points.
- The dashed series "Da se nastavio rast od 52 % godišnje" has 18 points, one per year from 2004 to 2021:
```
2004: 15415.1      2005: 23430.9      2006: 35615.0      2007: 54134.8
2008: 82284.9      2009: 125073.1     2010: 190111.1     2011: 288968.8
2012: 439232.6     2013: 667633.5     2014: 1014803.0    2015: 1542500.5
2016: 2344600.8    2017: 3563793.3    2018: 5416965.7    2019: 8233787.9
2020: 12515357.7   2021: 19023343.6
```

**Pitfall (inferred):** Do not compare the S18 numbers with the S19 numbers directly. S18 is a curve from average rates (1986-2003: 52 %). S19 is measured data (1988-2003: 40 %). The sources and the periods differ.

**Link:** Levels of parallelism (next topic). Pentium 4 (end of frequency scaling, 2004) is reference [7]. The slides S16-S19 do not cite [7], so this link is inferred. Parallel computers (topic "Parallel computers: hardware view").

## Levels of parallelism   [S17, S20, S21]
**What:** Parallelism exists on several levels in a computer.

| Level | Meaning | Who does it |
|---|---|---|
| Bit level | Doubling the word size. From 1970 to 1986: 4, 8, 16, 32 bit registers. One instruction processes more bits at once. | Hardware |
| Instruction level (strojne instrukcije) | All modern processors have a pipeline (protočna obrada). Many instructions are at the same time in different execution phases. | Hardware |
| Data parallelism (paralelizam podataka, engl. Data Parallelism) | The same operation on different parts of the data. | Programmer |
| Task parallelism (paralelizam zadatka, engl. Task Parallelism) | Different jobs at the same time. | Programmer |

**Why:** The first two levels are done by the hardware without the programmer. The programmer must write data parallelism and task parallelism. This course is about these two.

**Conclusion:** Hardware gives bit and instruction parallelism free. The programmer must create data and task parallelism.

**Example (pipeline, S20):**
- Pipeline: the execution of one instruction is divided into 5 phases. The next instruction enters the first phase as soon as the previous one moves to the second phase.
- Analogies: a factory line, or laundry. While the first wash dries, the second is already in the washing machine.
- The 5 phases: IF = instruction fetch (dohvat instrukcije), ID = decode (dekodiranje), EX = execute (izvršavanje), MEM = memory access (pristup memoriji), WB = write back of the result (upis rezultata).
- Clock cycle (ciklus sata): one tick of the processor clock. In each cycle each phase does one step. At 3 GHz there are 3 billion cycles per second. One cycle lasts about one third of a nanosecond.
- Table of 4 instructions (rows) over 8 cycles (columns). Instruction n starts in cycle n.

| | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 |
|---|---|---|---|---|---|---|---|---|
| Instr. 1 | IF | ID | EX | MEM | WB | | | |
| Instr. 2 | | IF | ID | EX | MEM | WB | | |
| Instr. 3 | | | IF | ID | EX | MEM | WB | |
| Instr. 4 | | | | IF | ID | EX | MEM | WB |

- The table layout is inferred from the slide text and the note (the picture grid is not in the text export). In cycle 4 four instructions work at the same time, each in a different phase.
- Sequential: 4 × 5 = 20 cycles. Pipelined: 5 + 3 = 8 cycles.
- The program is written sequentially. The processor still runs it partly in parallel.

**Example (data and task parallelism, S21):**

| | Data parallelism | Task parallelism |
|---|---|---|
| Idea | Same operation, different data | Different operations at the same time |
| What is divided | The data (for example an array of one million numbers) | The job (for example load, process, save) |
| What each core does | The same, on its own part | Something different |
| Example | Sum of an array: each core adds its own part | One task fetches data, another draws the interface |
| In C# | Parallel.For, Parallel.ForEach, PLINQ | Task.Run, Task.WhenAll |
| Limit | The parts must be independent | Tasks often wait for each other |

- How to choose: many equal elements means data parallelism. A few different jobs mean task parallelism. In practice both are combined.
- Data parallelism scales well: more data, more cores. Task parallelism is limited by the number of different jobs and by the waiting between them.
- Demo: `Vizualizacije/paralelizam.html`, section 2 (pipeline, S20) and section 3 (data and task parallelism, S21).

**Link:** Exercise 2 uses both forms: the sum of an array on k tasks (data parallelism) and `Paralelno(f, g)` (task parallelism). See the cross-reference section. The pipeline idea returns in the signal example (topic "Pipeline example: signal processing").

## Parallelism versus concurrency   [S22]
**What:**
- Parallelism (paralelizam): execution at the same time on different cores (engl. CPU/GPU core).
- Concurrency (konkurentnost): execution in turns on the same (shared) core.

**Why:**
- Parallelism means two tasks run at the same moment on two different cores.
- Concurrency means the operating system quickly switches the tasks on one core. It looks as if they run at the same time.
- Concurrency does not speed up the computation. It lets a program stay responsive while it waits for something, for example the network or the disk.
- The operating system decides when the threads switch, not the program.

**Conclusion:** Parallelism = different cores at the same time. Concurrency = one core in turns. Data races occur in both cases.

**Example (picture on S22, read from the image):** A table with two rows. Row "Parallelism": CPU Core 1 runs Task1 in one continuous line. CPU Core 2 runs Task2 in one continuous line. Both lines run at the same time. Row "Concurrency": one CPU Core. The line of Task1 and the line of Task2 are broken into pieces that alternate. The two drawn lines nearly touch where one piece ends and the other begins.

**Pitfall:** A data race occurs in both cases. The switch can happen in the middle of an operation, because the operating system chooses the moment.

**Link:** Thread scheduling (topic "Process and thread"). Data race (topic "What can go wrong"). Demo: `Vizualizacije/paralelizam.html`, section 1.

## Process and thread   [S23]
**What:**
- Process (proces): in this file the word means the operating system process, except where a note says otherwise. It is a program that runs. It has its own memory (address space), a unique number PID, and at least one thread [13].
- Thread (nit): the part of a process that the operating system schedules on a core. Threads of the same process share its memory. Each thread has its own stack (stog) [13].
- Shared memory of the process: variables and objects. Communication is easy, but here the data race occurs.

**Why:**
- Threads share memory, so they exchange data easily. For the same reason they can change the same variable at the same time. This is the key fact for the whole of part 1.
- Who decides which thread runs when? The scheduler (raspoređivač) of the operating system. It gives each thread a time slice on a core. It switches the core to another thread in three cases:
  - The time slice ends.
  - The thread waits for the disk or the network.
  - A thread with a higher priority is ready.
- The processor gives only the mechanism: a clock that interrupts the thread, and the saving of the registers at the switch.
- The program does not control this. A thread can stop in the middle of a statement such as `zbroj += x`.
- In .NET there is one more level. The thread pool (fond niti) decides which task goes to which thread. The operating system decides when that thread gets a core.

**Conclusion:** Shared memory makes communication easy and causes data races. The program does not control the thread switch.

**Example:**
- Windows Task Manager (Upravitelj zadataka): process Zadatak2_Task (PID 39256). Open Details, right-click the header, Select columns, Threads [14]. The columns are Name, PID, Status, Threads. The row shows Zadatak2_Task.exe with status Running. On the slide the PID and the thread count are "?" and the teacher reads them live.
- macOS Activity Monitor [15]: tab CPU; columns View > Columns > Threads. Row: Zadatak2_Task, 25 threads, PID 39256. Bottom of the window: Processes 872, Threads 4490.
- An empty .NET program already has 8 threads (for garbage collection and compilation). Task 2 of exercise 2 has 25 threads while it computes on many tasks. Tasks get their threads from the thread pool.
- Diagram: shared process memory (variables, objects) above; thread 1 (main), thread 2, thread 3, each with its own stack.
- Demo (note of S23): open Task Manager (Details, column Threads) or Activity Monitor. Then run the solution of exercise 2, task 2 live: `Tjedan 02 - Taskovi i zamke/Rjesenja (nastavnik)/Zadatak2_Task`, `dotnet run -c Release`, about 4 s. Show how the thread count grows. On Windows read the PID and the thread count, because the slide shows "?".

**Link:** Data race (topic "Computer with shared memory"). Thread pool and tasks: exercise 2, task 2 (`Zadatak2_Task`). Reference [10] (the managed thread pool) is not cited on S23. The link to it is inferred.

## Computer architectures   [S24-S26]
**What:** Types of systems on which parallel programs run (S24).

| Term | Meaning |
|---|---|
| CPU | Multi-core or multi-processor computer with shared memory. The teacher says this is the student's own laptop. |
| GPGPU | General computing on graphics processing units. A graphics card has thousands of simple cores. |
| Distributed system (raspodijeljeni, distribuirani sustav) | Loosely coupled system, connected by a local network, without shared hardware |
| Cluster (klaster) | Identical computers connected by a commercial TCP/IP network |
| Massively parallel processor (masivni paralelni procesor) | A very fast specialized network |
| Grid | System of computers connected by the internet |

- The key split for the course: do the cores share memory or not?

**Shared memory computer (S25):**
- One processor (one chip) has many cores. A core executes instructions. All cores see the same memory.
- Layout in the figure: many cores, then a shared cache memory, then the main memory (RAM).
- One core can read what another core wrote. Communication is fast: it is only reading and writing a variable. It is also dangerous: writing to the same variable at the same time gives a data race (utrka za podatke).
- Example: if two cores increment the same variable at the same time, one increment can be lost.
- Real chip example (the slide cites no reference here; [8] IBM Blue Gene/Q in the list is the likely source, inferred): IBM Blue Gene/Q has 18 cores and 32 MB of shared L2 cache memory. 16 cores compute, 1 core runs the operating system, and 1 core is a spare.
- The data race is the first big problem of the course.

**Computers connected by a network (S26):**
- Many computers. Each has its own memory and does not see the memory of the others. Data go as messages over the network.
- This is slower than shared memory. The number of computers can grow. Clusters, grids and supercomputers work on this principle.
- There is no shared variable, so a data race cannot occur. Other problems occur: deadlock (everyone waits for a message from another), lost messages, and messages in the wrong order.
- The course teaches this model in part 2.

**Why:** Memory sharing decides the speed of communication and the kind of problem. Shared memory: fast, risk of data race. Own memory: slow, scalable, risk of deadlock, lost messages and wrong order.

**Conclusion:** Shared memory = part 1 (data race). Network with messages = part 2 (deadlock, order, lost messages).

**Link:** Part 1 and part 2 of the course (topic "Course organization"). Deadlock: `07-zastoj-semafori-konkurentne-kolekcije.md`.

## Sequential program, parallel program, parallel problem   [S27-S29]
**What:**
- Sequential program (S27): the problem is decomposed into a discrete sequence of instructions. The instructions run one after the other, on one core. At any moment one instruction runs.
- Run time = the sum of the durations of all instructions. Example: 6 instructions of 1 unit each need 6 units.
- Parallel program (S28): the problem is split into discrete parts that can be solved at the same time. Each part is a sequence of instructions. The instructions of each part run at the same time on different cores. A general control and coordination mechanism is applied.
- Figure: a problem split into part 1 to part 4, each on its own core (core 1 to core 4), each with instructions 1-4.
- Parallel problem (S29): a computational problem that
  - is split into discrete parts of work that can be solved at the same time;
  - runs many program instructions at any moment;
  - is solved in less time with more computing resources than with one resource.
- Computing resources: one computer with many processors (cores), or any number of such computers connected by a network. These are the two architectures from S25 and S26.

**Why:**
- If the parts are equal, four cores finish about four times faster.
- Coordination is needed. If part 2 needs the result of part 1, it must wait. Waiting costs time.
- Two pitfalls appear already: bad division (one part is larger than the others) and dependencies between the parts.
- Not every problem is parallel. If each step needs the result of the previous step, extra cores do not help.

**Conclusion:** A sequential run time is a sum. A parallel run time depends on the equal size of the parts, the waiting and the coordination. The longest part sets the end time (inferred).

**Pitfall:** Dependencies and unequal parts reduce the gain. (S28 note)

**Link:** Amdahl law and design steps (below).

## Who uses parallel programming   [S30, S34]
**What:** Parallel programming is used where there is much data or there are many independent computations. The last line of the slide: "common to all: a large data set or many independent computations".

| Industry and commerce | Science and engineering |
|---|---|
| Artificial intelligence: model training spread on thousands of GPU cores | Weather and climate: the atmosphere divided into cells, each on its own core |
| Web search engines: a query is searched on many servers at the same time | Genetics: comparison of millions of parts of DNA sequences |
| Databases: a query runs on parts of a table at the same time | Particle physics: processing of millions of collisions from a detector |
| Medical imaging: reconstruction of a CT image by layers | Chemistry and drugs: simulation of molecule interaction |
| Finance: Monte Carlo simulation of millions of scenarios | Mechanical engineering: air flow around cars and aircraft |

**Example (S34):**
- The slide says: parallel programming is now used intensively all over the world, in a wide range of applications (source: https://www.top500.org/).
- Picture, read from the image: a bar chart "Top500 HPC Application Areas". The vertical axis goes from 0 to 160. The highest bars are "Other" (about 150) and "Research" (about 75). Finance and Logistic Services are at about 37. Service is at about 33. Other areas, for example Defense, Geophysics, Telecomm, WWW, Information Service and Aerospace, have lower bars. The exact bar values are approximate readings.
- The note names finance, logistics, defense, telecommunications and geophysics. The message is variety: parallel programming is not only for scientists. It is also for companies that process large data each day.

**Conclusion:** Use parallel programming when the problem has a large data set or many independent computations.

**Link:** Monte Carlo simulation: the student writes one in lecture 2 (`02-flynn-granularnost-monte-carlo-pi.md`). Reference [2] is TOP500, June 2025.

## The real world is parallel   [S31-S33]
**What:**
- In the natural world many complex, connected events happen at the same time, but inside a time order.
- Compared with sequential programming, parallel programming is much more suitable to model, simulate and understand complex real-world phenomena.

**Why:** There is a second reason beyond speed. The real world is parallel by nature. Examples are city traffic, weather and the movement of planets. All parts of the system change at the same time and affect each other. A sequential program must put them in an artificial sequence. A parallel program follows the structure of the problem more naturally.

**Example (pictures):**
- S32 picture with caption "Real world phenomena can be simulated with parallel computing". Six photos: Galaxy Formation, Planetary Movments (spelling as in the picture), Climate Change, Rush Hour Traffic, Plate Tectonics, Weather. In each case there are many parts that act at the same time: stars, vehicles, plates, air masses. Each part is computed locally. The parts exchange data with their neighbours from time to time.
- S33 picture with caption "Parallel computing is key to simulating a range of complex physical phenomena". Examples of simulations: flow through a jet engine, flow and mixing of fluids, the brain, oceans around Antarctica, the universe, the interior of the Earth. In each simulation the space is divided into many small cells. The cells are computed in parallel. Without parallel programming these simulations would take too long to be useful.

**Link:** Cell grid and exchange of border cells: topic "Climate modeling".

## Pipeline example: signal processing   [S35]
**What:** An example of task parallelism, exactly a pipeline.
- An audio signal is divided into segments 1, 2, 3, ... The segments pass through four filters. Each filter is a separate stage. (The slide calls each filter a "process". Reading it in the general sense is inferred.) A filter processes one segment in one step.
- A segment passes the filters in order. A filter takes the next segment at once.
- Read the table like the instruction table. The row is a filter and the column is a step (1-9). The number in the cell is the segment. Segment 1 is in filter 1 at step 1 and in filter 2 at step 2. At that time filter 1 already processes segment 2.
- From step 4 all four filters work at the same time (in step 4 all four are busy).
- One sequential worker that does all four filters in turn, 6 segments: 6 × 4 = 24 steps. (The slide says "one process". Same inferred reading.) Pipelined: 4 + 5 = 9 steps.
- Same idea as the instruction pipeline.

**Pitfall:** The whole line runs at the speed of the slowest filter. (This assumes a filter needs one step per segment, as the note says.)

**Link:** Instruction pipeline (topic "Levels of parallelism"). Idle-time reasoning: the pipeline needs time to fill (4 + 5 = 9, not 6) (inferred).

## Climate modeling   [S36-S37]
**What:**
- A climate model has components: atmosphere (atmosfera), land (kopno), hydrology (hidrologija) and ocean.
- Each component is a separate task on its own cores. This is task parallelism (S36).
- Arrows are the data exchange during the computation. Two arrows are shown: atmosphere to ocean: wind speed. Ocean to atmosphere: sea surface temperature.
- Inside one component (S37): the model divides the Earth into a grid of cells. Each cell has layers of atmosphere and ocean. In each cell the model computes temperature, wind, humidity and currents, step by step in time.
- The cells are divided among the cores. In the example four cores each get a quarter of the grid. This is data parallelism.
- A cell on the border needs values of a neighbour cell. That cell belongs to another core. So in each step the neighbour cores exchange the border cells.
- Figure: one square = one cell. Darker squares = border cells that neighbour cores exchange (arrows). Four regions: core 1, core 2, core 3, core 4.

**Why:** The ocean cannot compute further until it receives the wind. Tasks wait for each other. Communication and synchronization cost time. This shows the price of parallelism.

**Conclusion:** A climate model combines both forms. Between components: task parallelism. Inside each component: data parallelism.

**Link:** Levels of parallelism (data and task). Communication and synchronization cost: topic "What can go wrong". Message passing and border exchange: part 2 of the course (inferred).

## Large language model: why it needs many cores   [S38]
**What:** Llama 3 is the largest model of the company Meta. The training of the model Llama 3 405B needs about 3.8 · 10²⁵ operations [9]. How long does it take?
- Formula: time = operations / speed.
- Three cases:

| # | Hardware | Speed | Time |
|---|---|---|---|
| 1 | One processor core | about 10⁹ operations per second (simplified) | about 1.2 billion years (1,204,147,336.9 years in the chart) |
| 2 | One H100 GPU (16,896 cores) [18] | in practice about 4 · 10¹⁴ operations per second [9] | about 3,000 years (3010.4 in the chart) |
| 3 | 16,384 GPUs together (about 2.8 · 10⁸ cores), work divided on all GPUs | (computed) | about 67 days (0.1837 years in the chart) |

- Calculation: 3.8 · 10²⁵ / 10⁹ ≈ 3.8 · 10¹⁶ s. 3.8 · 10²⁵ / (4 · 10¹⁴) ≈ 9.5 · 10¹⁰ s. 9.5 · 10¹⁰ s / 16,384 ≈ 5.8 · 10⁶ s. GPU speed: [9], table 4.
- The chart has a logarithmic scale of years. Each step shortens the time by many orders of magnitude.

**Why:** The model exists only because the work is divided among hundreds of millions of cores.

**Conclusion:** Even here the speedup is not ideal. The GPUs run at only about 40 % of peak performance, because the cores communicate and wait for each other [9].

**Link:** Amdahl law. Demo: `Vizualizacije/amdahl.html`, section "Primjer: učenje velikog jezičnog modela". The GPU and LLM lecture is in week 13.

## Designing a parallel program   [S39-S41]
**What:**
- Start: a sequential program that the programmer wants to speed up. Before the code, answer three questions (S39).

| # | Question | Action |
|---|---|---|
| 1 | What can run at the same time? | Find parts of the work that do not depend on each other. This step is called decomposition (dekompozicija) in the literature. |
| 2 | How to divide? | Divide the data or the tasks into parts of about equal size. |
| 3 | Is it correct, and does it pay off? | Check that the results are correct. Measure whether the program is really faster. |

- Next question: who does steps 1 and 2, the programmer or a tool?

**Who decides what to parallelize (S40):** Steps 1 and 2 have three possible doers:
- a compiler (alat koji prevodi kod);
- a programmer with directives (oznake u kodu);
- a programmer with a library.

| | Compiler, automatic | Programmer + directives (for example OpenMP) | Programmer + library (C#: this course) |
|---|---|---|---|
| Finds the parallelism | Compiler, by loop analysis | Programmer | Programmer |
| Writes the parallel code | Compiler | Programmer marks the loop, the compiler creates threads | Programmer chooses a construct: Task, Parallel.For, PLINQ |
| Guarantees correctness | Compiler, but only if it proves independence | Programmer | Programmer |
| Typical problem | Often gives up or makes slower code | A wrongly marked loop gives wrong results | Data race, tasks that are too small |

- In C# no tool knows by itself what is safe to parallelize. The programmer chooses the construct. The library only schedules the work on threads.

**Check of iteration independence (S41):**
The slide has two code blocks. Heading "Ispravno" (Correct):
```csharp
Parallel.For(0, n, i =>
{
    b[i] = a[i] * 2;
});
```
Heading "Pogrešno" (Wrong):
```csharp
long zbroj = 0;
Parallel.For(0, n, i =>
{
    zbroj += a[i];
});
```
- Correct: each iteration writes its own element. The order does not matter, so the parallelization is safe.
- Wrong: all iterations change the same variable. This is a data race. The result is wrong and changes at each run. The compiler does not report it.
- Cause, from the note: two threads read the same old value, both add their number, and both write. One addition is lost.
- Question before each parallelization: may the iterations run at the same time and in any order?
- Protection mechanisms come in the lecture on data race (week 4). Until then the fix is isolation: each task adds into its own local variable, and at the end the local sums are added.
- Demo: `Tjedan 02 - Taskovi i zamke/Vizualizacije/zamke.html`, section 3.

**Why:**
- Step 2 needs parts of about equal size. If not, one core works and the others wait (S39 note).
- A compiler parallelizes a loop only if it proves that the iterations are independent. This rarely succeeds, so it often gives up (S40 note).
- With directives such as OpenMP, a wrong mark gives a wrong result (S40 note).
- The library does not check correctness. The responsibility is always on the programmer. For this reason the course teaches how to recognize the problems.

**Conclusion:** Ask three questions: what is parallel, how to divide, is it correct and faster. Measure, do not assume. The library schedules, the programmer is responsible.

**Pitfall:**
- `zbroj += a[i]` inside `Parallel.For` is a data race. The program compiles and runs. The result changes at each run.

**Link:** Exercise 2, task 3, pitfall C (`Zadatak3_Zamke`) is exactly this example. Lecture week 4: lock and Interlocked.

## Parallel computers: hardware view   [S42]
**What:** From the hardware view nearly every stand-alone computer today is parallel. It has:
- many functional units (L1 cache, L2 cache, branching, prefetch, decoding, floating point, graphics processing (GPU), integer, etc.);
- many execution units (cores);
- many hardware threads.

**Why:** Inside one core many functional units work at the same time. A chip has many cores. A core often has many hardware threads (note).

**Conclusion:** Parallel hardware is on every desk. The question is whether the program uses it.

**Link:** Levels of parallelism. Architectures.

## Amdahl law: the limit of speedup   [S43-S44]
**What:**
- The speedup of a parallel program depends on two values. The first is the number of cores N. The second is the part P of the program that can be parallelized.
- Formula: U(N) = 1 / ((1 − P) + P / N).
  - P = the part of the time that can be parallelized.
  - 1 − P = the sequential part. It always takes the same time.
  - N = number of cores. U = speedup (ubrzanje).
- Limit when N grows: U → 1 / (1 − P).
- Limits of speedup (S43): 2× (P = 50 %), 4× (75 %), 10× (90 %), 20× (95 %), 100× (99 %).

**Why:**
- The formula is 1 divided by the sum of the sequential part, 1 − P, and the parallel part divided by N cores, P/N.
- When N grows, P/N goes to zero. The speedup goes to 1 / (1 − P).
- The curves flatten because of the part that cannot be parallelized. It always takes the same time. This is the Amdahl law (G. Amdahl, 1967) [6].

**Conclusion:** More cores do not help without a smaller sequential part. Before adding cores, reduce the sequential part.

**Example:**
- P = 90 %, N = 10: sequential part 0.1, parallel part 0.9 / 10 = 0.09, total 0.19. U = 1 / (0.1 + 0.09) ≈ 5.3, not 10.
- Question for students: speedup for P = 90 % and N = 100? Answer: about 9.2 (1 / (0.1 + 0.009)).
- Even 1024 cores give only 2× if half of the program is sequential.
- Table S43: speedup by number of cores N and the part P.

| N | P = 50 % | P = 75 % | P = 90 % | P = 95 % | P = 99 % |
|---|---|---|---|---|---|
| 1 | 1.0 | 1.0 | 1.0 | 1.0 | 1.0 |
| 2 | 1.33 | 1.6 | 1.82 | 1.9 | 1.98 |
| 4 | 1.6 | 2.29 | 3.08 | 3.48 | 3.88 |
| 8 | 1.78 | 2.91 | 4.71 | 5.93 | 7.48 |
| 16 | 1.88 | 3.37 | 6.4 | 9.14 | 13.91 |
| 32 | 1.94 | 3.66 | 7.8 | 12.55 | 24.43 |
| 64 | 1.97 | 3.82 | 8.77 | 15.42 | 39.26 |
| 128 | 1.98 | 3.91 | 9.34 | 17.41 | 56.39 |
| 256 | 1.99 | 3.95 | 9.66 | 18.62 | 72.11 |
| 512 | 2.0 | 3.98 | 9.83 | 19.28 | 83.8 |
| 1024 | 2.0 | 3.99 | 9.91 | 19.64 | 91.18 |
| 2048 | 2.0 | 3.99 | 9.96 | 19.82 | 95.39 |
| 4096 | 2.0 | 4.0 | 9.98 | 19.91 | 97.64 |

- Table S44: speedup on 1024 cores by part P.

| P | 50 % | 75 % | 90 % | 95 % | 99 % | 99.9 % |
|---|---|---|---|---|---|---|
| U(1024) | 2.0 | 4.0 | 9.9 | 19.6 | 91.2 | 506.2 |

- Demo: `Vizualizacije/amdahl.html`.

**Pitfall:** The sequential part is one of the "traps" (zamke) of the course. It limits the speedup however many cores the student adds.

**Link:** Week 3 lecture works with it in detail. The week-3 exercise "Parallel.For and speedup factor" compares the measured speedup with this limit (S12). This exercise is not in the repo (see Open points). Exercise 2, task 2 only asks why S is not equal to k. See the cross-reference section.

## What can go wrong   [S45]
**What:** The frame of the whole course. Two rows of problems.

| Correctness problems | Meaning |
|---|---|
| Data race (utrka za podatke) | Two threads change the same variable and one update is lost. |
| Deadlock (potpuni zastoj) | Each thread holds one resource and waits for the resource of another. Nobody makes progress. |
| Nondeterminism (nedeterminizam) | The order of execution is not guaranteed. The same program can give different results. |

| Efficiency problems | Meaning |
|---|---|
| Too small granularity (premala granularnost) | If the job is small, the creation of tasks costs more than it saves. |
| Bad division of work (loša podjela posla) | One thread gets the most work and all others wait for it. |
| Sequential part (sekvencijalni dio) | A part that cannot be divided limits the speedup (Amdahl law). |

**Why:** Libraries give tools. They do not protect against these errors.

**Conclusion:** The course teaches how to recognize these problems and solve them. For each problem the lecture first shows a program that does not work, then the fix.

**Link:** Exercise 2, task 3 (`Zadatak3_Zamke`). Demo: `Tjedan 02 - Taskovi i zamke/Vizualizacije/zamke.html`. Granularity: lecture 2. Data race: week 4. Deadlock: week 7 and week 11.

## Task takes a function as an argument   [S46]
**What:**
```csharp
Task t = Task.Run(() =>
    Console.WriteLine("Paralelno!"));

Parallel.For(0, 1000, i =>
{
    // tijelo petlje
});
```
- `() => ...` is a lambda expression (lambda izraz): a function without a name that the program gives to the library.
- `Task.Run` and `Parallel.For` receive a delegate. So we define the body of the task, and the library decides where and when it runs. The library runs it on some thread.

**Why:** Without delegates and lambda expressions the student cannot write one task. For this reason exercise 1 is about delegates and lambda expressions.

**Conclusion:** A task body is a function passed as an argument.

**Pitfall:** Closure (the first pitfall, shown in exercise 1): a lambda captures the variable, not its value. (Note of S46.)

**Link:** Exercise 1 (all tasks) and exercise 2 pitfalls A and D. Demo: `Vizualizacije/delegati.html`. Reference [12] (`foreach` and variable capture) is not cited on S46. The link to it is inferred.

## Summary and references   [S47-S48]
**What (S47):**
- Parallel execution: the problem is divided into parts solved at the same time, so the result comes sooner.
- Why now: the processor frequency grows very slowly, so computers become faster by more cores. The program must use these cores.
- Parallelism and concurrency: execution at the same time on different cores against execution in turns on the same core.
- What can go wrong: data race, deadlock, nondeterminism and bad division of work. The course teaches how to recognize and solve them.
- Next: exercise 1 (delegates and lambda expressions) and the architectures of parallel computers. Next week lecture: Flynn taxonomy, granularity and the first parallel program, Monte Carlo calculation of π.

**References (S48, the numbers appear in square brackets on the slides):**

| # | Source |
|---|---|
| [1] | LLNL: Introduction to Parallel Computing (hpc.llnl.gov/documentation/tutorials/introduction-parallel-computing-tutorial) |
| [2] | TOP500, June 2025 (top500.org/lists/top500/2025/06/highs/) |
| [3] | Moore's law (en.wikipedia.org/wiki/Moore%27s_law) |
| [4] | J. Hennessy, D. Patterson: Computer Architecture: A Quantitative Approach, 6th ed., 2019 |
| [5] | Classic RISC pipeline (en.wikipedia.org/wiki/Classic_RISC_pipeline) |
| [6] | Amdahl law (G. Amdahl, 1967) (en.wikipedia.org/wiki/Amdahl%27s_law) |
| [7] | Pentium 4 (end of frequency scaling, 2004) (en.wikipedia.org/wiki/Pentium_4) |
| [8] | IBM Blue Gene/Q (en.wikipedia.org/wiki/Blue_Gene) |
| [9] | The Llama 3 Herd of Models, 2024 (arxiv.org/abs/2407.21783) |
| [10] | Microsoft Learn: The managed thread pool (learn.microsoft.com/dotnet/standard/threading/the-managed-thread-pool) |
| [11] | Microsoft Learn: Data parallelism (TPL) (learn.microsoft.com/dotnet/standard/parallel-programming/data-parallelism-task-parallel-library) |
| [12] | Microsoft Learn: foreach and variable capture (learn.microsoft.com/dotnet/csharp/language-reference/statements/iteration-statements) |
| [13] | Microsoft Learn: About Processes and Threads (learn.microsoft.com/windows/win32/procthread/about-processes-and-threads) |
| [14] | Microsoft Learn: Troubleshoot processes by using Task Manager (learn.microsoft.com/troubleshoot/windows-server/support-tools/support-tools-task-manager) |
| [15] | Apple: Activity Monitor, View information about processes (support.apple.com/guide/activity-monitor/actmntr1001/mac) |
| [16] | K. Rupp: Microprocessor Trend Data, 2022 (CC BY 4.0) (github.com/karlrupp/microprocessor-trend-data) |
| [17] | R. Li et al.: SPEC CPU2026: Characterization, Representativeness, and Cross-Suite Comparison, 2026 (arxiv.org/abs/2605.03713) |
| [18] | NVIDIA: H100 Tensor Core GPU Architecture (whitepaper, v1.02), 2022 |

- The note of S48 says the references are the sources of the numbers and definitions. The closing words are "Thanks for your attention. Questions?".

## Exercise cross-reference
Source files: `exercises/01-delegates/` and `exercises/02-parallel/` (starter projects with solution comments in the code, instruction pages `Vjezba1_upute.html` and `Vjezba2_upute.html`). Exercise 2 has its own knowledge file: `ex-02-parallel.md`.

**Exercise 1: delegates and lambda expressions** (`exercises/01-delegates/`). Concept: Task.Run and Parallel.For take a function. The exercise builds this skill.

| Task | Concept | Key insight of the solution | If ignored (inferred) |
|---|---|---|---|
| 1 `Zadatak1_Delegati/Zadatak1_Delegati.cs` | A delegate is a type for a method signature (`delegate int Calc(int x)`). A variable of this type can point to any method with the same signature. | `Podijeli` returns `x / n`. A multicast delegate `oboje = Oduzmi; oboje += Pomnozi;` calls both. The call `oboje(100)` returns the value of the last method in the invocation list: 5000. `GetInvocationList().Length` is 2. Output: 50, 5000, 2, 5000, 2. | The student expects the result of the first method or the sum of both. The student cannot pass a method as an argument. |
| 2 `Zadatak2_UniverzalnaFunkcija/Zadatak2_UniverzalnaFunkcija.cs` | One function works for all conditions because the condition is passed as a delegate (`delegate bool Uvjet(int x)`). | `Ispisi(niz, uvjet)` counts and returns printed elements. For `{3, 8, 12, 15, 20, 7, 4}`: even 8 12 20 4; larger than 10: 12 15 20; divisible by 3: 3 12 15, count 3. | The student writes one function per condition. The "body decided by the caller" idea of Task.Run is not understood. |
| 3 `Zadatak3_Lambda/Zadatak3_Lambda.cs` | A lambda is a method without a name, written where it is used: `(parameters) => expression`. Closure. | `puta = x => x * faktor` gives 15 for `faktor = 3` and 50 after `faktor = 10`. The lambda remembers the variable `faktor`, not its value. Also: `kvadrat(7) = 49`, `zbroj 1..10 = 55`, `abs(-8) = 8`, `Action vrijeme` prints the time. | The student is surprised by changed results. This is the cause of pitfall A and D in exercise 2 (loop variable captured by tasks). |
| 4 `Zadatak4_GenerickiDelegati/Zadatak4_GenerickiDelegati.cs` | Built-in delegates: `Action<T...>` (no return value), `Func<T..., R>` (last type is the result), `Predicate<T>` (returns bool). | `Func<double,double,double> hipotenuza = (a, b) => Math.Sqrt(a*a + b*b)` gives 5 for (3, 4). `Array.FindAll(niz, x => x % 2 == 0)` gives -2, 0, 4. `Action<int[]>` prints `5 -2 9 0 -7 4`. LINQ Where and Select also appear: 5, 9, 4 and 25, 4, 81, 0, 49, 16. | The student declares custom delegates for everything. `Func<T>` is the type that `Task.Run` and `Paralelno<T,U>` use in exercise 2. |
| 5 `Zadatak5_GenerickaKlasa/Zadatak5_GenerickaKlasa.cs` | A generic class `Stog<T>` with methods that take delegates. | `ZaSvaki(Action<T>)` goes from the top down (index from `Count - 1` to 0). `Broji(Predicate<T>)` counts. `Skini` throws `InvalidOperationException("Stog je prazan.")` on an empty stack. Output: `Skinuto: 50, veličina: 4`; `programiranje`, `paralelno`; `Brojeva većih od 20: 2`. | Behaviour is fixed inside the class, so the class is not reusable. |

Instruction details for exercise 1 (from `Vjezba1_upute.html`):
- Target framework: .NET 10. Run a task with `dotnet run` in the task folder.
- Task 1: the given constant is `n = 50`. `Oduzmi` returns `x - n` and `Pomnozi` returns `x * n`. The student adds `Podijeli` and calls it through the delegate. The student makes one delegate that runs both `Oduzmi` and `Pomnozi` and calls it with 100. The student prints `oboje.GetInvocationList().Length` and answers: which value does the call return and why?
- Task 3: the student runs the program and answers why the first `puta(5)` gives 15 and the second gives 50. The student writes `abs` without `Math.Abs` and prints `abs(-8)`. The student writes a lambda with no parameters that prints the current time. It needs `Action` (see task 4). Sample output line: `Trenutno vrijeme: 17:56:03`.
- Task 4 (the first output lines come from the comments in `Zadatak4_GenerickiDelegati.cs`, not from `Vjezba1_upute.html`): first lines of the output are `zbroj(3, 4) = 7`, `Bok, svijete!` (from `Action<string> pozdrav`) and `pozitivan(-5) = False` (from `Predicate<int> pozitivan`). The student writes the `hipotenuza` lambda, the `Array.FindAll` call and the `Action<int[]>` lambda.

**Exercise 2: tasks, speedup, first pitfalls** (`exercises/02-parallel/`). Concept: data parallelism, task parallelism, speedup, and the first correctness problems from S45.

| Task | Concept | Key insight of the solution | If ignored (inferred) |
|---|---|---|---|
| 1 `Zadatak1_Paralelno/Zadatak1_Paralelno.cs` | Task parallelism with two different jobs f (sum 1..3·10⁹, returns long) and g (count of primes up to 3·10⁶, returns int). | `Paralelno<T,U>(Func<T> f, Func<U> g)` starts `Task.Run(f)` and `Task.Run(g)`. Then it returns `(tf.Result, tg.Result)`. `Result` blocks until the task ends. Results: f = 4500000001500000000, g = 216816. Example times: sequential 1056 ms, parallel 833 ms. The time is about max(tf, tg). The speedup is at most 2, limited by the longer job and the number of cores. Time must be measured in Release. | The student runs the jobs one after the other, or measures in Debug. The method name must not be `Parallel` because it is a class name in System.Threading.Tasks. |
| 2 `Zadatak2_Task/Zadatak2_Task.cs` | Data parallelism: sum 1..10⁹ on k = 2, 4, 8, 16 tasks, with Task.Run and with Parallel.For. Speedup S = T(sequential) / T(k tasks) and efficiency S/k. | Result must always be 500000000500000000. `ToArray()` starts all tasks before the wait. Each `Parallel.For` iteration writes its own element `d[j]`. Use the shortest of 3 runs. Example (6 cores, 12 logical): S = 1.95, 3.66, 5.57, 5.22 for k = 2, 4, 8, 16 with Task.Run. S is less than k because of overhead, memory and shared cores. | Without the shortest of three runs the numbers are noisy. Without the check of the sum a wrong parallel program looks fast. Expecting S = k ignores overhead (inferred). |
| 3 `Zadatak3_Zamke/Zadatak3_Zamke.cs` | The four pitfalls: A closure over the loop variable, B lost updates, C iteration independence, D two errors. | A: copy `int j = i;` per iteration (before the fix: 40 40 40 40; expected 0 10 20 30). B: `Interlocked.Increment(ref brojac)` gives 20000000 in about 80 ms against about 23 ms unprotected, which loses about half of the updates (sample unprotected value 10238038). C: loop 1 `b[i] = a[i] * 2` is always correct because each iteration writes its own element. Loop 2 `zbroj += a[i]` is a data race (sample 148596 instead of 1000000). Fix with `Interlocked.Add(ref zbroj, a[i])`. D: error 1, closure over `i` (all Id = 20); error 2, `new Random()` in the same millisecond gives equal sequences. Use `Random.Shared`. Print sorted by Id. | The program compiles and runs without an error but the result is wrong and changes at each run. The compiler does not warn. |

Instruction details for exercise 2 (from `Vjezba2_upute.html`):
- Measure time in the Release configuration for tasks 1 and 2: choose Release in Visual Studio or Rider, the "(Release)" launch configuration in VS Code, or run `dotnet run -c Release`. Otherwise the results are not reliable.
- Task 1 question: how large can the speedup be for two jobs, and what does it depend on? The answer: at most 2, and it depends on the longer job and the number of cores. Example output (6 cores, 12 logical): sequential 1687 ms, parallel 831 ms.
- Task 2 example output (6 cores, 12 logical, `Environment.ProcessorCount` = 12): sequential 256 ms, 1 task 259 ms (about equal to sequential). Parallel.For rows (k, time, S, S/k): 2, 127 ms, 2.02, 1.01; 4, 64 ms, 4.00, 1.00; 8, 46 ms, 5.57, 0.70; 16, 51 ms, 5.02, 0.31. Task.Run rows: 2, 131 ms, 1.95, 0.98; 4, 70 ms, 3.66, 0.91; 8, 46 ms, 5.57, 0.70; 16, 49 ms, 5.22, 0.33. The exact values can differ. The shape and the sum must match.
- The student answers: why is S not equal to k?
- Exercise 2 text says: the goal is not to learn all about synchronization today, but to see that correctness is not guaranteed. The protection mechanisms come in week 4.
- The instruction page for exercise 2 points to `amdahl.html` (folder Vizualizacije of week 1) and `zamke.html` (week 2).

## Open points
- The week-3 exercise (Parallel.For and speedup factor, S11 and S12) is not in the repo. The folder `exercises/03-data-race` is the data race exercise, which S11 puts in week 4. The numbering of the folders differs from the week numbers.
- S12: the link levels come from the cell fill colours in the .pptx (verified with python-pptx). The text export does not show them.
- S20 and S35: the grids of the pipeline tables are drawn as shapes. The tables here are rebuilt from the text and the notes (inferred). S35 step 9 column: the last segment 6 leaves filter 4 at step 9.
- S34: bar values in the Top500 picture are approximate readings of the image.
- S32 and S33 pictures have no alternative text in the deck. The caption texts are read from the images.
- S18/S19: the curve in S18 is a reconstruction, so values differ from the measured data in S19 (52 % against 40 %). The source does not explain the difference beyond that.
- S23: the PID and thread count are "?" on the Windows part of the slide by design. The teacher fills them live.
- Visualization files (`paralelizam.html`, `amdahl.html`, `zamke.html`) are named in the notes. Only `exercises/01-delegates/Vizualizacije/delegati.html` was found in the repo. This file did not read those pages.
- The solution notes of exercises are comments inside the C# starter files. The folder `Rjesenja (nastavnik)` named in S23 is not in the repo.

## Coverage ledger
| Slide | Topic section where it is covered | Items |
|---|---|---|
| 1 | Course organization | 4 (title, lecturer, basis of lectures, year; note: three questions, link to exercise 1) |
| 2 | Course organization | agenda only (section title; note: goal, outcomes, duties, grading, plan) |
| 3 | Course organization | 8 (ECTS, hours, prerequisite, owners, associate, goal, C# reason, two hours per week) |
| 4 | Course organization | 8 (6 outcomes, data race term, skills against understanding) |
| 5 | Course organization | 6 (two parts, topic table, shared memory against messages, same problems return) |
| 6 | Course organization | 5 (70 % twice, separate counting, 14 sessions, 10 sessions, X-ica) |
| 7 | Course organization | 5 (20 %, 80 %, 10 % and 40 %, purpose of homework and exams) |
| 8 | Course organization | 6 (exam 1, exam 2, 50 %, partial retake, validity, re-enrolment) |
| 9 | Course organization | 6 (literature, materials, HTML views, C# and .NET 10, IDEs, message tool later) |
| 10 | Course organization | 4 (weekly table, structure of weeks, exams in weeks 8 and 15, plan can change) |
| 11 | Course organization | 4 (exercises table, building tools, problem then fix pattern, last weeks) |
| 12 | Course organization | 6 (link table with level per week from fill colours, direct link meaning, granularity example, Amdahl example, week 5 no link) |
| 13 | Course organization | 7 (3 steps, SDK check, IDEs, VS 2022 note, Release for timing, Debug reason, README) |
| 14 | Definition and goal | 2 (transition, three questions) |
| 15 | Definition and goal | 4 (definition, division, goal, worker analogy) |
| 16 | Why now | 6 (Moore, physical limit, more cores, message, history of frequency, programmer writes) |
| 17 | Levels of parallelism | 5 (bit level with 4-32 bit, instruction level, data, task, hardware against programmer) |
| 18 | Why now | 8 (chart series, five growth rates, 52 % doubles in 20 months, bend in 2003, dashed line, source, log scale, reconstruction) |
| 19 | Why now | 9 (chart data summary, dashed series, four rates, source Rupp, SPECint, data end 2021 and reason, 100 times, CPU2026 note, same message) |
| 20 | Levels of parallelism | 8 (5 phases, cycle, 3 GHz, 20 against 8 cycles, cycle 4, analogy, sequential program, demo) |
| 21 | Levels of parallelism | 8 (comparison table rows, how to choose, scaling, limits, exercise 2 link, demo) |
| 22 | Parallelism versus concurrency | 7 (two definitions, picture, responsiveness, OS decides, data race in both, demo) |
| 23 | Process and thread | 11 (Running status, process, thread, PID, stack, shared memory, scheduler, three switch reasons, mechanism, thread pool, 8 and 25 threads, demo) |
| 24 | Computer architectures | 8 (laptop anchor, six architecture terms, key split) |
| 25 | Shared memory computer | 6 (core, shared memory, fast and dangerous, data race, Blue Gene/Q numbers, increment lost example) |
| 26 | Network computers | 7 (own memory, messages, slower, scalable, cluster and grid, no data race, other problems) |
| 27 | Sequential program | 5 (four properties, run time as sum, 6 instruction example) |
| 28 | Sequential program, parallel program | 6 (four properties, figure, equal parts four times faster, coordination, two pitfalls) |
| 29 | Sequential program, parallel program | 5 (definition three points, resources, dependency note) |
| 30 | Who uses | 11 (10 uses, common line, plus note on Monte Carlo) |
| 31 | The real world is parallel | 3 (statement, comparison, reason with examples) |
| 32 | The real world is parallel | 2 (picture with six phenomena, local computation) |
| 33 | The real world is parallel | 2 (picture with simulations, cells in parallel) |
| 34 | Who uses | 3 (statement with source, bar chart reading, variety message) |
| 35 | Pipeline example | 7 (4 filters, segments, table reading, step 4, 24 against 9, same idea, slowest filter) |
| 36 | Climate modeling | 6 (components, task parallelism, arrows, two exchanges, waiting cost) |
| 37 | Climate modeling | 6 (grid of cells, computed values, data parallelism, border exchange, both forms, figure legend) |
| 38 | Large language model | 10 (Llama 3 is the largest model of Meta, three cases with numbers, formula, calculation, log chart, 40 % peak, source, demo) |
| 39 | Designing a parallel program | 5 (three questions, decomposition, equal parts, measure, next question) |
| 40 | Designing a parallel program | 6 (three ways, table of four rows, OpenMP, C# no tool, library only schedules, programmer responsible) |
| 41 | Designing a parallel program | 7 (correct code, wrong code, explanation, question, isolation fix, data race cause, demo) |
| 42 | Parallel computers: hardware view | 4 (functional units, cores, hardware threads, note) |
| 43 | Amdahl law | 6 (two dependencies, full table, limits, flatten, 1967, demo) |
| 44 | Amdahl law | 9 (formula, symbols, example 5.3, limit, 1024 cores 2×, question 9.2, table with 99.9 %, message, demo) |
| 45 | What can go wrong | 8 (six problems, library remark, plan of the course, demo) |
| 46 | Task takes a function | 6 (code, lambda, delegate, reason for exercise 1, closure pitfall, demo) |
| 47 | Summary and references | 5 (four points, next steps) |
| 48 | Summary and references | 18 (all references [1]-[18]) |
