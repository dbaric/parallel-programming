// Zadatak 4: Periodicno2<U>
// Periodicno2 poziva f svakih ms milisekundi, pamti svaku povratnu vrijednost i nakon prekida vraća listu.
// Test 1: f vraća vrijeme od početka u ms, prekid nakon 3,5 s (CancelAfter).
// Test 2 koristi zadanu metodu PeriodicnoUZajednickuListu: piše u listu koju glavna nit za to vrijeme čita.

using System.Diagnostics;

static class Zadatak4_Periodicno2
{
    static Task<List<U>> Periodicno2<U>(Func<U> f, int ms, CancellationToken token)
    {
        return Task.Run(() =>
        {
            var lista = new List<U>();   // lokalna lista, nitko drugi je ne vidi dok task ne završi
            var sat = Stopwatch.StartNew();
            for (int k = 1; !token.IsCancellationRequested; k++)
            {
                lista.Add(f());
                token.WaitHandle.WaitOne((int)Math.Max(0, k * ms - sat.ElapsedMilliseconds));
            }
            return lista;
        });
    }

    // Zadano: dodaje f() u zajedničku listu svakih ms milisekundi, dok se ne zatraži prekid.
    static Task PeriodicnoUZajednickuListu<U>(Func<U> f, int ms, List<U> lista, CancellationToken token)
    {
        return Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                lock (lista) lista.Add(f());
                Thread.Sleep(ms);
            }
        });
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Test 1: Periodicno2, prekid nakon 3,5 s");
        var sat = Stopwatch.StartNew();
        var izvor = new CancellationTokenSource();
        izvor.CancelAfter(3500);
        List<long> vremena = Periodicno2(() => sat.ElapsedMilliseconds, 1000, izvor.Token).Result;
        Console.WriteLine($"Elemenata: {vremena.Count}: {string.Join(", ", vremena)}");   // Elemenata: 4: 2, 1005, 2007, 3005

        Console.WriteLine("Test 2: zajednička lista, glavna nit je čita dok se puni");
        var lista = new List<long>();
        var izvor2 = new CancellationTokenSource();
        Task t = PeriodicnoUZajednickuListu(() => sat.ElapsedMilliseconds, 0, lista, izvor2.Token);
        var dok = Stopwatch.StartNew();
        long citanja = 0;
        while (dok.ElapsedMilliseconds < 2000)
        {
            long zbroj = 0;
            lock (lista) foreach (long x in lista) zbroj += x;
            citanja++;
        }
        izvor2.Cancel();
        t.Wait();
        Console.WriteLine($"Pročitano {citanja} puta, elemenata na kraju: {lista.Count}");   // npr. Pročitano 471719 puta, elemenata na kraju: 13523; bez lock-a: InvalidOperationException (lista se promijenila tijekom foreach)
    }
}
