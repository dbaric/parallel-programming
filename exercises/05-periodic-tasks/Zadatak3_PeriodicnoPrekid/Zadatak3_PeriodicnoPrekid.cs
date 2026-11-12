// Zadatak 3: Periodicno s prekidom
// Periodicno poziva akciju svakih ms milisekundi dok glavna nit ne zatraži prekid tokenom.
// Vraća Task, da glavna nit može čekati kraj i pročitati njegovo stanje (Status).
// Test 1: prekid nakon 3,5 s. Test 2: prekid zatražen prije poziva metode.

using System.Diagnostics;

static class Zadatak3_PeriodicnoPrekid
{
    static Task Periodicno(Action akcija, int ms, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var sat = Stopwatch.StartNew();
        int poziv = 0;
        Action akcija = () =>
        {
            poziv++;
            Console.WriteLine($"poziv {poziv}: {sat.ElapsedMilliseconds,5} ms");
        };

        Console.WriteLine("Test 1: prekid nakon 3,5 s");
        var izvor = new CancellationTokenSource();
        Task t = Periodicno(akcija, 1000, izvor.Token);
        Thread.Sleep(3500);
        var doKraja = Stopwatch.StartNew();
        izvor.Cancel();
        t.Wait();
        Console.WriteLine($"Task završio {doKraja.ElapsedMilliseconds} ms nakon Cancel().");
        Console.WriteLine($"Status: {t.Status}");

        Console.WriteLine("Test 2: prekid zatražen prije poziva");
        var izvor2 = new CancellationTokenSource();
        izvor2.Cancel();
        Task t2 = Periodicno(akcija, 1000, izvor2.Token);
        t2.Wait();
        Console.WriteLine($"Status: {t2.Status}");
    }
}
