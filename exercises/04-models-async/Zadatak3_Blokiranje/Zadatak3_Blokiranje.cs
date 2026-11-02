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
        Console.WriteLine($"Task.WhenAll, {K} × Delay:  {sat.ElapsedMilliseconds} ms");   // ≈ 1004 ms

        sat.Restart();
        await Task.WhenAll(Enumerable.Range(0, K).Select(_ => Task.Run(() => Thread.Sleep(1000))));   // svako čekanje drži svoju nit
        Console.WriteLine($"Task.WhenAll, {K} × Sleep:  {sat.ElapsedMilliseconds} ms");   // npr. 7528 ms: fond niti dodaje niti postupno, a ne sve odjednom

        using var izvor = new CancellationTokenSource();
        var cekanja = Enumerable.Range(0, K).Select(_ => Task.Delay(1000, izvor.Token)).ToArray();
        sat.Restart();
        izvor.CancelAfter(300);   // zatraži prekid nakon 300 ms
        try { await Task.WhenAll(cekanja); }
        catch (OperationCanceledException) { }   // WhenAll baca iznimku prvog prekinutog taska
        Console.WriteLine($"Prekid nakon 300 ms: {sat.ElapsedMilliseconds} ms, prekinuto {cekanja.Count(t => t.IsCanceled)} od {K}");   // npr. 324 ms, prekinuto 100 od 100 (Task.Delay prekida i dok spava; Thread.Sleep ne bi)
    }
}
