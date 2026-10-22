// Zadatak 2: zbroj na k taskova
// Task.Run prima delegat (lambdu) i izvodi ga na niti iz fonda niti.
// VAŽNO: vrijeme mjerite u Release konfiguraciji (Build > Configuration: Release, Ctrl+F5).

using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak2_Task.Pokreni();

static class Zadatak2_Task
{
    const long N = 1_000_000_000L;

    // Zbroj brojeva iz [od, do]
    static long Zbroji(long od, long @do)
    {
        long s = 0;
        for (long i = od; i <= @do; i++) s += i;
        return s;
    }

    public static void Pokreni()
    {
        // Slijedno
        var sat = Stopwatch.StartNew();
        long zbroj = Zbroji(1, N);
        sat.Stop();
        Console.WriteLine($"Slijedno:   {zbroj}  ({sat.ElapsedMilliseconds} ms)");

        // Jedan Task
        sat.Restart();
        Task<long> jedan = Task.Run(() => Zbroji(1, N));
        long r1 = jedan.Result;
        sat.Stop();
        Console.WriteLine($"1 task:     {r1}  ({sat.ElapsedMilliseconds} ms)");
    }
}
