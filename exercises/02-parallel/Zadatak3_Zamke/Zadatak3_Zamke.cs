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
            zadaci.Add(Task.Run(() => i * 10));

        Console.WriteLine("Zamka A   Očekivano: 0 10 20 30");
        Console.WriteLine("          Dobiveno:  " + string.Join(" ", zadaci.Select(z => z.Result)));
    }

    // Zamka B
    static void ZamkaB()
    {
        const int koraka = 10_000_000;
        long brojac = 0;

        var sat = Stopwatch.StartNew();
        var a = Task.Run(() => { for (int i = 0; i < koraka; i++) brojac++; });
        var b = Task.Run(() => { for (int i = 0; i < koraka; i++) brojac++; });
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
        Parallel.For(0, n, i => { zbroj += a[i]; });
        Console.WriteLine($"          Petlja 2, zbroj += a[i]:   zbroj = {zbroj}, očekivano {n}");
    }

    // Zamka D: 20 objekata s jedinstvenim Id-om (0 … 19) i slučajnim brojem, ispisanih redom po Id-u
    static void ZamkaD()
    {
        Console.WriteLine("Zamka D   Očekivano: " + string.Join(" ", Enumerable.Range(0, 20)));
        Console.Write("          Dobiveno:  ");

        var zadaci = new List<Task>();
        for (int i = 0; i < 20; i++)
        {
            var rnd = new Random();
            zadaci.Add(Task.Run(() =>
            {
                var e = new Entitet(i, rnd.Next(100));
                Console.Write(e.Id + " ");
            }));
        }
        Task.WaitAll(zadaci.ToArray());
        Console.WriteLine();
    }
}
