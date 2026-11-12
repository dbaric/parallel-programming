// Zadatak 1: ime svake sekunde, do prekida
// Task ispisuje ime svake sekunde dok glavna nit ne zatraži prekid (Enter).
// Program mjeri koliko prođe od Cancel() do kraja taska.

using System.Diagnostics;

static class Zadatak1_ImeSvakeSekunde
{
    const string Ime = "Ana";

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var izvor = new CancellationTokenSource();
        CancellationToken token = izvor.Token;

        Console.WriteLine("Za prekid pritisnite Enter.");
        Task t = Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine(Ime);
                Thread.Sleep(1000);
            }
        });

        Console.ReadLine();

        var sat = Stopwatch.StartNew();
        izvor.Cancel();
        t.Wait();
        Console.WriteLine($"Task završio {sat.ElapsedMilliseconds} ms nakon Cancel().");
    }
}
