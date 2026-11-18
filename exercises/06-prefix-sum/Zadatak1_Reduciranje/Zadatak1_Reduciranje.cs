// Zadatak 1: reduciranje stablom
// Reduciraj računa a[0] ⊕ a[1] ⊕ … ⊕ a[n − 1] stablom (up-sweep), paralelno unutar razine, na mjestu, za n koji je potencija broja 2.
// Main za svaki test ispisuje rezultat Reduciraj, slijedni rezultat i jesu li isti.
// Mjeriti u Release konfiguraciji: dotnet run -c Release

using System.Diagnostics;
using System.Globalization;

static class Zadatak1_Reduciranje
{
    // Zadano: slijedno, slijeva nadesno.
    static long Slijedno(long[] a, Func<long, long, long> op)
    {
        long s = a[0];
        for (int i = 1; i < a.Length; i++)
        {
            s = op(s, a[i]);
        }
        return s;
    }

    static long Reduciraj(long[] a, Func<long, long, long> op)
    {
        for (int d = 2; d <= a.Length; d *= 2)
        {
            Parallel.For(1, a.Length / d + 1, k => a[k * d - 1] = op(a[k * d - 1 - d / 2], a[k * d - 1]));
        }
        return a[^1];
    }

    static void Test(string naziv, long[] ulaz, Func<long, long, long> op, bool ispisiNiz = false)
    {
        long slijedno = Slijedno(ulaz, op);
        var a = (long[])ulaz.Clone();
        long stablo = Reduciraj(a, op);
        Console.WriteLine(naziv);
        if (ispisiNiz)
        {
            Console.WriteLine($"  niz nakon Reduciraj: {string.Join(" ", a)}");   // 3 4 7 11 4 5 6 25
        }
        Console.WriteLine($"  Reduciraj: {stablo:N0}, slijedno: {slijedno:N0}, isto: {(stablo == slijedno ? "da" : "ne")}");   // test 1: 25, 25, da; test 2: −22.532.375, da; test 3: 1.000.000, da; test 4: −5, −19, ne
    }

    static double Najkrace(Func<long> f)
    {
        f();
        double najbolje = double.MaxValue;
        for (int k = 0; k < 3; k++)
        {
            var sat = Stopwatch.StartNew();
            f();
            najbolje = Math.Min(najbolje, sat.Elapsed.TotalMilliseconds);
        }
        return najbolje;
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("hr-HR");

        long[] mali = { 3, 1, 7, 0, 4, 1, 6, 3 };
        Test("Test 1: 3 1 7 0 4 1 6 3, zbrajanje", mali, (x, y) => x + y, ispisiNiz: true);

        int n = 1 << 22;
        var slucajni = new Random(2026);
        var veliki = new long[n];
        for (int i = 0; i < n; i++)
        {
            veliki[i] = slucajni.Next(-1_000_000, 1_000_001);
        }
        Test("Test 2: n = 2^22, zbrajanje", veliki, (x, y) => x + y);
        double tSlijedno = Najkrace(() => Slijedno(veliki, (x, y) => x + y));
        double tStablo = Najkrace(() => Reduciraj((long[])veliki.Clone(), (x, y) => x + y));
        Console.WriteLine($"  vrijeme: slijedno {tSlijedno:F1} ms, Reduciraj {tStablo:F1} ms (s kopiranjem niza), najkraće od 3");   // slijedno 7,2 ms, Reduciraj 9,3 ms

        Test("Test 3: n = 2^22, najveći element", veliki, Math.Max);
        Test("Test 4: 3 1 7 0 4 1 6 3, oduzimanje", mali, (x, y) => x - y);
    }
}
