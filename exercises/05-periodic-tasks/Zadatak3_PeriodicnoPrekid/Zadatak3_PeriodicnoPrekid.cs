// Zadatak 3: Periodicno s prekidom
// Periodicno poziva akciju svakih ms milisekundi dok glavna nit ne zatraži prekid tokenom.
// Vraća Task, da glavna nit može čekati kraj i pročitati njegovo stanje (Status).
// Test 1: prekid nakon 3,5 s. Test 2: prekid zatražen prije poziva metode.

using System.Diagnostics;

static class Zadatak3_PeriodicnoPrekid
{
    static Task Periodicno(Action akcija, int ms, CancellationToken token)
    {
        return Task.Run(() =>
        {
            var sat = Stopwatch.StartNew();
            for (int k = 1; !token.IsCancellationRequested; k++)
            {
                akcija();
                token.WaitHandle.WaitOne((int)Math.Max(0, k * ms - sat.ElapsedMilliseconds));
            }
            token.ThrowIfCancellationRequested();   // Status = Canceled
        }, token);   // token i ovdje: prekid prije početka, task se ne pokrene
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var sat = Stopwatch.StartNew();
        int poziv = 0;
        Action akcija = () =>
        {
            poziv++;
            Console.WriteLine($"poziv {poziv}: {sat.ElapsedMilliseconds,5} ms");   // poziv 1: 3 ms, poziv 2: 1008 ms, poziv 3: 2005 ms, poziv 4: 3005 ms
        };

        Console.WriteLine("Test 1: prekid nakon 3,5 s");
        var izvor = new CancellationTokenSource();
        Task t = Periodicno(akcija, 1000, izvor.Token);
        Thread.Sleep(3500);
        var doKraja = Stopwatch.StartNew();
        izvor.Cancel();
        try { t.Wait(); } catch (AggregateException) { }   // Wait() na prekinutom tasku baca iznimku
        Console.WriteLine($"Task završio {doKraja.ElapsedMilliseconds} ms nakon Cancel().");   // 40 ms
        Console.WriteLine($"Status: {t.Status}");   // Canceled

        Console.WriteLine("Test 2: prekid zatražen prije poziva");
        var izvor2 = new CancellationTokenSource();
        izvor2.Cancel();
        Task t2 = Periodicno(akcija, 1000, izvor2.Token);
        try { t2.Wait(); } catch (AggregateException) { }
        Console.WriteLine($"Status: {t2.Status}");   // Canceled; akcija se nije pozvala jer se task s već prekinutim tokenom ne pokrene
    }
}
