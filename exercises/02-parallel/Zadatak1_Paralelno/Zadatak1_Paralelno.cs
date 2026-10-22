// Zadatak 1: Paralelno<T, U>
// Dva različita posla: f zbraja brojeve, a g broji proste brojeve. Slijedni poziv je zadan.
// VAŽNO: vrijeme mjerite u Release konfiguraciji (Build > Configuration: Release, Ctrl+F5).

using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak1_Paralelno.Pokreni();

static class Zadatak1_Paralelno
{
    // Zbroj brojeva iz [od, do]
    static long Zbroji(long od, long @do)
    {
        long s = 0;
        for (long i = od; i <= @do; i++) s += i;
        return s;
    }

    // Broj prostih brojeva iz [2, n]
    static int BrojProstih(int n)
    {
        int broj = 0;
        for (int x = 2; x <= n; x++)
        {
            bool prost = true;
            for (int d = 2; (long)d * d <= x; d++)
                if (x % d == 0) { prost = false; break; }
            if (prost) broj++;
        }
        return broj;
    }

    public static void Pokreni()
    {
        Func<long> f = () => Zbroji(1, 3_000_000_000L);
        Func<int> g = () => BrojProstih(3_000_000);

        var sat = Stopwatch.StartNew();
        long rf = f();
        int rg = g();
        sat.Stop();
        Console.WriteLine($"Slijedno:   f = {rf}, g = {rg}  ({sat.ElapsedMilliseconds} ms)");
    }
}
