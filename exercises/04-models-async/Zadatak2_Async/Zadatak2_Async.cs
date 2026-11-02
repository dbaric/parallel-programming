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
        Console.WriteLine($"jedno po jedno, 3 × Delay:  {sat.ElapsedMilliseconds} ms");   // ≈ 3003 ms

        foreach (int k in new[] { 3, 100 })
        {
            sat.Restart();
            await Task.WhenAll(Enumerable.Range(0, k).Select(Dohvati));   // sva čekanja se preklapaju, nit se za to vrijeme vraća u fond
            Console.WriteLine($"Task.WhenAll, {k} × Delay:  {sat.ElapsedMilliseconds} ms");   // ≈ 1001 ms za 3, ≈ 1004 ms za 100: čekanje ne treba jezgre
        }

        sat.Restart();
        for (int i = 0; i < 3; i++) await Racunaj(N);
        Console.WriteLine($"Racunaj, jedan po jedan:     {sat.ElapsedMilliseconds} ms");   // npr. 1030 ms

        sat.Restart();
        await Task.WhenAll(Racunaj(N), Racunaj(N), Racunaj(N));   // bez await u metodi, svaki poziv se izvede do kraja u ovoj niti
        Console.WriteLine($"WhenAll, bez Task.Run:       {sat.ElapsedMilliseconds} ms");   // npr. 1017 ms ≈ jedan po jedan

        sat.Restart();
        await Task.WhenAll(Task.Run(() => Racunaj(N)), Task.Run(() => Racunaj(N)), Task.Run(() => Racunaj(N)));   // tri izračuna na tri niti iz fonda
        Console.WriteLine($"WhenAll, s Task.Run:         {sat.ElapsedMilliseconds} ms");   // npr. 348 ms ≈ trećina; async služi za čekanje, a posao koji računa ide na Task.Run
    }
}
