// Zadatak 3: četiri zamke
// Svi programi se prevedu i pokrenu bez greške, ali ne daju ono što očekujete.
// Cilj: uočiti problem, objasniti ga i popraviti.

using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak3_Zamke.Pokreni();

class Entitet
{
    public int Id { get; }
    public int Br { get; }

    public Entitet(int id, int br)
    {
        Id = id;
        Br = br;
    }
}

static class Zadatak3_Zamke
{
    public static void Pokreni()
    {
        ZamkaA();
        ZamkaB();
        ZamkaC();
        ZamkaD();
    }

    // Zamka A
    static void ZamkaA()
    {
        var zadaci = new List<Task<int>>();
        for (int i = 0; i < 4; i++)
        {
            int j = i;   // vlastita kopija po iteraciji; lambda je prije hvatala zajedničko i (koje je na kraju 4)
            zadaci.Add(Task.Run(() => j * 10));   // 0 10 20 30
        }

        Console.WriteLine("Zamka A   Očekivano: 0 10 20 30");
        Console.WriteLine("          Dobiveno:  " + string.Join(" ", zadaci.Select(z => z.Result)));
    }

    // Zamka B
    static void ZamkaB()
    {
        const int koraka = 10_000_000;
        long brojac = 0;

        var sat = Stopwatch.StartNew();
        var a = Task.Run(() => { for (int i = 0; i < koraka; i++) Interlocked.Increment(ref brojac); });   // atomarno povećanje, nema izgubljenih ažuriranja
        var b = Task.Run(() => { for (int i = 0; i < koraka; i++) Interlocked.Increment(ref brojac); });   // brojac = 20000000 (≈ 80 ms naspram ≈ 23 ms nezaštićeno, koje gubi ≈ polovicu ažuriranja)
        Task.WaitAll(a, b);
        sat.Stop();

        Console.WriteLine($"Zamka B   Očekivano: {2L * koraka}");
        Console.WriteLine($"          Dobiveno:  {brojac}  ({sat.Elapsed.TotalMilliseconds:F1} ms)");
    }

    // Zamka C
    static void ZamkaC()
    {
        const int n = 1_000_000;
        var a = new int[n];
        Array.Fill(a, 1);
        var b = new int[n];

        Parallel.For(0, n, i => { b[i] = a[i] * 2; });
        Console.WriteLine($"Zamka C   Petlja 1, b[i] = a[i] * 2: zbroj = {b.Sum(x => (long)x)}, očekivano {2L * n}");

        long zbroj = 0;
        Parallel.For(0, n, i => Interlocked.Add(ref zbroj, a[i]));   // atomarno zbrajanje; zbroj = 1000000 (petlja 1 je uvijek točna: svaka iteracija piše svoj b[i])
        Console.WriteLine($"          Petlja 2, zbroj += a[i]:   zbroj = {zbroj}, očekivano {n}");
    }

    // Zamka D: 20 objekata s jedinstvenim Id-om (0 … 19) i slučajnim brojem, ispisanih redom po Id-u
    static void ZamkaD()
    {
        Console.WriteLine("Zamka D   Očekivano: " + string.Join(" ", Enumerable.Range(0, 20)));
        Console.Write("          Dobiveno:  ");

        var zadaci = new List<Task<Entitet>>();
        for (int i = 0; i < 20; i++)
        {
            int j = i;   // greška 1: zatvaranje nad i, svaki Entitet je dobio isti Id
            zadaci.Add(Task.Run(() => new Entitet(j, Random.Shared.Next(100))));   // greška 2: new Random() u istoj milisekundi daje iste nizove; Random.Shared je siguran za niti
        }
        Task.WaitAll(zadaci.ToArray());
        Console.WriteLine(string.Join(" ", zadaci.Select(z => z.Result).OrderBy(e => e.Id).Select(e => e.Id)));   // 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 (redom po Id-u, ne po završetku taska)
    }
}
