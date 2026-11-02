// Zadatak 3: blokiranje usred async koda
// Sto istih čekanja od 1 s. Zadano: ispravno čekanje s await Task.Delay.

using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
await Zadatak3_Blokiranje.Pokreni();

static class Zadatak3_Blokiranje
{
    const int K = 100;

    public static async Task Pokreni()
    {
        Console.WriteLine($"Jezgri (logičkih): {Environment.ProcessorCount}");

        var sat = Stopwatch.StartNew();
        await Task.WhenAll(Enumerable.Range(0, K).Select(_ => Task.Delay(1000)));
        sat.Stop();
        Console.WriteLine($"Task.WhenAll, {K} × Delay:  {sat.ElapsedMilliseconds} ms");

        // TODO a): isto čekanje sa Thread.Sleep(1000) unutar Task.Run; zašto traje dulje?
        // TODO b): isto čekanje uz prekid: CancellationTokenSource i Task.Delay(1000, token); prekinite nakon 300 ms i ispišite stanje taskova
    }
}
