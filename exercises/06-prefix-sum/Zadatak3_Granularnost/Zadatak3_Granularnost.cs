// Zadatak 3: tri izvedbe istog algoritma i mjerenje
// Sve tri metode računaju prescan na mjestu (operacija op, neutralni 0) i vraćaju ukupni zbroj.
// PrescanPoRazini: vaš Prescan iz zadatka 2 (Parallel.For po razini); ovdje je n uvijek potencija broja 2.
// PrescanTaskPoParu: isti algoritam, ali jedan Task.Run za svaki par na svakoj razini (kao PRAM s n/2 procesora).
// PrescanBlokovi: p blokova; n ne mora biti djeljiv s p.
// Main provjerava rezultat (slijedno) i mjeri vrijeme (najkraće od 3). Mjeriti u Release konfiguraciji.

using System.Diagnostics;
using System.Globalization;

static class Zadatak3_Granularnost
{
    // Zadano: zbrajanje koje uz to nešto računa (desetak do pedesetak µs, ovisi o računalu i opterećenju; Main ispisuje izmjereno). Rezultat je uvijek x + y.
    static long SkupoZbroji(long x, long y)
    {
        double z = 0;
        for (int k = 0; k < 7000; k++)
        {
            z += Math.Sqrt(k);
        }
        return x + y + (z < 0 ? 1 : 0);
    }

    // Zadano.
    static long PrescanSlijedno(long[] a, Func<long, long, long> op)
    {
        long s = 0;
        for (int i = 0; i < a.Length; i++)
        {
            long t = a[i];
            a[i] = s;
            s = op(s, t);
        }
        return s;
    }

    // Napišite (iz zadatka 2).
    static long PrescanPoRazini(long[] a, Func<long, long, long> op)
    {
        return 0;
    }

    // Napišite.
    static long PrescanTaskPoParu(long[] a, Func<long, long, long> op)
    {
        return 0;
    }

    // Napišite.
    static long PrescanBlokovi(long[] a, int p, Func<long, long, long> op)
    {
        return 0;
    }

    // Vraća najkraće vrijeme od tri mjerenja u ms, ili NaN ako je rezultat kriv.
    static double Izmjeri(Func<long[], long> prescan, long[] ulaz, long[] ocekivano, long ocekivanoUkupno)
    {
        double najbolje = double.MaxValue;
        for (int k = 0; k < 4; k++)
        {
            var a = (long[])ulaz.Clone();
            var sat = Stopwatch.StartNew();
            long ukupno = prescan(a);
            sat.Stop();
            if (ukupno != ocekivanoUkupno || !a.SequenceEqual(ocekivano))
            {
                return double.NaN;
            }
            if (k > 0)
            {
                najbolje = Math.Min(najbolje, sat.Elapsed.TotalMilliseconds);
            }
        }
        return najbolje;
    }

    static string Vrijeme(double t) => double.IsNaN(t) ? "krivo" : $"{t:F2}";

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("hr-HR");
        Func<long, long, long> zbroji = (x, y) => x + y;
        int jezgri = Environment.ProcessorCount;
        var slucajni = new Random(2026);

        Console.WriteLine($"Korak 1: zbrajanje, p = {jezgri} (logičkih jezgri); ms, najkraće od 3");
        Console.WriteLine($"{"n",-6} {"slijedno",10} {"po razini",12} {"task po paru",14} {"blokovi",10}");
        foreach (int k in new[] { 10, 14, 18, 20, 22, 24 })
        {
            int n = 1 << k;
            var ulaz = new long[n];
            for (int i = 0; i < n; i++)
            {
                ulaz[i] = slucajni.Next(0, 100);
            }
            var ocekivano = (long[])ulaz.Clone();
            long ukupno = PrescanSlijedno(ocekivano, zbroji);
            double tS = Izmjeri(a => PrescanSlijedno(a, zbroji), ulaz, ocekivano, ukupno);
            double tR = Izmjeri(a => PrescanPoRazini(a, zbroji), ulaz, ocekivano, ukupno);
            string tT = k <= 18 ? Vrijeme(Izmjeri(a => PrescanTaskPoParu(a, zbroji), ulaz, ocekivano, ukupno)) : "preskočeno";
            double tB = Izmjeri(a => PrescanBlokovi(a, jezgri, zbroji), ulaz, ocekivano, ukupno);
            Console.WriteLine($"2^{k,-4} {Vrijeme(tS),10} {Vrijeme(tR),12} {tT,14} {Vrijeme(tB),10}");
        }

        Console.WriteLine();
        Console.WriteLine("Korak 2: blokovi, n = 2^24, različit p");
        {
            int n = 1 << 24;
            var ulaz = new long[n];
            for (int i = 0; i < n; i++)
            {
                ulaz[i] = slucajni.Next(0, 100);
            }
            var ocekivano = (long[])ulaz.Clone();
            long ukupno = PrescanSlijedno(ocekivano, zbroji);
            double tS = Izmjeri(a => PrescanSlijedno(a, zbroji), ulaz, ocekivano, ukupno);
            Console.WriteLine($"  slijedno:        {tS,7:F1} ms");
            foreach (int p in new[] { 1, 2, 4, 8, jezgri, 2 * jezgri, 1000, 100_000, 1 << 22 })
            {
                double t = Izmjeri(a => PrescanBlokovi(a, p, zbroji), ulaz, ocekivano, ukupno);
                Console.WriteLine(double.IsNaN(t) ? $"  p = {p,9:N0}: krivo" : $"  p = {p,9:N0}: {t,7:F1} ms   S = {tS / t:F2}");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Korak 3: SkupoZbroji, n = 2^14, p = {jezgri}; ms, najkraće od 3");
        {
            int n = 1 << 14;
            var ulaz = new long[n];
            for (int i = 0; i < n; i++)
            {
                ulaz[i] = slucajni.Next(0, 100);
            }
            var ocekivano = (long[])ulaz.Clone();
            long ukupno = PrescanSlijedno(ocekivano, zbroji);
            var sat = Stopwatch.StartNew();
            long q = 0;
            for (int i = 0; i < 1000; i++)
            {
                q = SkupoZbroji(q, 1);
            }
            Console.WriteLine($"  jedan SkupoZbroji: {sat.Elapsed.TotalMilliseconds:F1} µs");
            double tS = Izmjeri(a => PrescanSlijedno(a, SkupoZbroji), ulaz, ocekivano, ukupno);
            double tR = Izmjeri(a => PrescanPoRazini(a, SkupoZbroji), ulaz, ocekivano, ukupno);
            double tT = Izmjeri(a => PrescanTaskPoParu(a, SkupoZbroji), ulaz, ocekivano, ukupno);
            double tB = Izmjeri(a => PrescanBlokovi(a, jezgri, SkupoZbroji), ulaz, ocekivano, ukupno);
            Console.WriteLine($"  slijedno {Vrijeme(tS)}, po razini {Vrijeme(tR)}, task po paru {Vrijeme(tT)}, blokovi {Vrijeme(tB)}");
            if (!double.IsNaN(tR) && !double.IsNaN(tT) && !double.IsNaN(tB))
            {
                Console.WriteLine($"  S: po razini {tS / tR:F2}, task po paru {tS / tT:F2}, blokovi {tS / tB:F2}");
            }
        }
    }
}
