// Zadatak 2: async/await je konkurentnost, a ne paralelizam
// Dohvati glumi čekanje na mrežu. Racunaj samo računa.
// VAŽNO: vrijeme mjerite u Release konfiguraciji.

using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
await Zadatak2_Async.Pokreni();

static class Zadatak2_Async
{
    static async Task<int> Dohvati(int id)
    {
        await Task.Delay(1000);   // npr. mreža
        return id;
    }

    static async Task<long> Racunaj(long n)
    {
        long s = 0;
        for (long i = 0; i < n; i++) s += i % 7;
        return s;
    }

    const long N = 600_000_000;

    public static async Task Pokreni()
    {
        var sat = Stopwatch.StartNew();
        for (int i = 0; i < 3; i++) await Dohvati(i);
        sat.Stop();
        Console.WriteLine($"jedno po jedno, 3 × Delay:  {sat.ElapsedMilliseconds} ms");

        // TODO a): isto s Task.WhenAll nad 3 poziva Dohvati, pa nad 100 poziva
        // TODO b): Task.WhenAll(Racunaj(N), Racunaj(N), Racunaj(N)) bez Task.Run, pa s Task.Run; usporedite s tri poziva jedan po jedan
    }
}
