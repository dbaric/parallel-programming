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

    // Dijeli [1, N] na k dijelova; svaki zbraja jedan Task
    static long ZbrojTask(int k) =>
        Enumerable.Range(0, k).Select(j => Task.Run(() => Zbroji(j * (N / k) + 1, (j + 1) * (N / k)))).ToArray().Sum(z => z.Result);   // ToArray pokreće sve taskove prije čekanja; 500000000500000000

    // Isto s Parallel.For; svaka iteracija piše svoj element polja
    static long ZbrojFor(int k)
    {
        var d = new long[k];
        Parallel.For(0, k, j => d[j] = Zbroji(j * (N / k) + 1, (j + 1) * (N / k)));   // d[j] = zbroj j-tog dijela
        return d.Sum();   // 500000000500000000
    }

    // Najkraće vrijeme (ms) od tri izvođenja
    static double Najkraće(Action a) => Enumerable.Range(0, 3).Min(_ => { var sat = Stopwatch.StartNew(); a(); return sat.Elapsed.TotalMilliseconds; });   // npr. 70

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
        Console.WriteLine($"1 task:     {r1}  ({sat.ElapsedMilliseconds} ms)");   // 1 task:     500000000500000000  (≈ slijedno)

        Console.WriteLine($"Jezgri (logičkih): {Environment.ProcessorCount}");   // Jezgri (logičkih): broj logičkih jezgri računala
        double ts = Najkraće(() => Zbroji(1, N));   // najkraće od 3 mjerenja slijedno
        foreach (var (naziv, zbrojK) in new (string, Func<int, long>)[] { ("Task.Run", ZbrojTask), ("Parallel.For", ZbrojFor) })
        {
            Console.WriteLine($"\n{naziv}\nk     vrijeme     S     S/k   zbroj");   // zaglavlje tablice
            foreach (int k in new[] { 2, 4, 8, 16 })
            {
                long z = 0;
                double t = Najkraće(() => z = zbrojK(k));   // najkraće od 3 mjerenja za k
                Console.WriteLine($"{k,-4} {t,6:F0} ms {ts / t,5:F2} {ts / t / k,6:F2}   {(z == zbroj ? "ispravan" : "POGREŠKA")}");   // npr. 4 70 ms 3,66 0,91 ispravan; S < k zbog režije, memorije i dijeljenih jezgri
            }
        }
    }
}
