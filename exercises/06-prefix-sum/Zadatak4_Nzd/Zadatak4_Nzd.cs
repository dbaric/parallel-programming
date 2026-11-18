// Zadatak 4: NZD svakog retka i cijele matrice
// NzdRetka(redak): najveći zajednički djelitelj retka, reduciranjem stablom (paralelno unutar razine).
// NzdRedaka(m): NZD svakog retka; retci paralelno, za svaki redak NzdRetka.
// NzdMatrice(m): NZD svih elemenata matrice, pomoću NzdRedaka.
// NzdRedakaSlijednoURetku(m): NZD svakog retka; retci paralelno, unutar retka slijedno.
// Duljina retka nije potencija broja 2. Main uspoređuje sa slijednim i mjeri vrijeme za tri oblika matrice.
// Mjeriti u Release konfiguraciji: dotnet run -c Release

using System.Diagnostics;
using System.Globalization;

static class Zadatak4_Nzd
{
    // Zadano.
    static long Nzd(long a, long b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }
        return Math.Abs(a);
    }

    static long NzdRetka(long[] redak)
    {
        var a = (long[])redak.Clone();
        for (int d = 1; d < a.Length; d *= 2)
        {
            Parallel.For(0, (a.Length + 2 * d - 1) / (2 * d), k =>
            {
                if (k * 2 * d + d < a.Length)
                {
                    a[k * 2 * d] = Nzd(a[k * 2 * d], a[k * 2 * d + d]);
                }
            });
        }
        return a[0];
    }

    static long[] NzdRedaka(long[][] m)
    {
        var rezultat = new long[m.Length];
        Parallel.For(0, m.Length, i => rezultat[i] = NzdRetka(m[i]));
        return rezultat;
    }

    static long NzdMatrice(long[][] m) => NzdRetka(NzdRedaka(m));

    static long[] NzdRedakaSlijednoURetku(long[][] m)
    {
        var rezultat = new long[m.Length];
        Parallel.For(0, m.Length, i => rezultat[i] = m[i].Aggregate(Nzd));
        return rezultat;
    }

    // Zadano: slijedno, za provjeru.
    static long[] NzdRedakaSlijedno(long[][] m)
    {
        var rezultat = new long[m.Length];
        for (int i = 0; i < m.Length; i++)
        {
            long g = m[i][0];
            for (int j = 1; j < m[i].Length; j++)
            {
                g = Nzd(g, m[i][j]);
            }
            rezultat[i] = g;
        }
        return rezultat;
    }

    // Zadano: element retka i je g · k_i · slučajni broj od 1 do 999, k_i od 2 do 12.
    static long[][] Matrica(int retci, int stupci, long g, int sjeme)
    {
        var slucajni = new Random(sjeme);
        var m = new long[retci][];
        for (int i = 0; i < retci; i++)
        {
            long k = slucajni.Next(2, 13);
            m[i] = new long[stupci];
            for (int j = 0; j < stupci; j++)
            {
                m[i][j] = g * k * slucajni.Next(1, 1000);
            }
        }
        return m;
    }

    static double Najkrace(Func<bool> f, out bool tocno)
    {
        tocno = f();
        double najbolje = double.MaxValue;
        for (int k = 0; k < 3; k++)
        {
            var sat = Stopwatch.StartNew();
            f();
            najbolje = Math.Min(najbolje, sat.Elapsed.TotalMilliseconds);
        }
        return najbolje;
    }

    static string Da(bool b) => b ? "da" : "ne";

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("hr-HR");

        long[][] mala =
        {
            new long[] { 12, 18, 30, 42, 54, 6 },
            new long[] { 35, 14, 21, 49, 7, 28 },
            new long[] { 8, 20, 44, 12, 16, 4 },
        };
        var ocekivano = NzdRedakaSlijedno(mala);
        var dobiveno = NzdRedaka(mala);
        Console.WriteLine("Test 1: 3 × 6");
        Console.WriteLine($"  NzdRedaka: {string.Join(" ", dobiveno)}, slijedno: {string.Join(" ", ocekivano)}, isto: {Da(dobiveno.SequenceEqual(ocekivano))}");   // 6 7 4, 6 7 4, da
        long gm = NzdMatrice(mala);
        long gs = ocekivano.Aggregate(Nzd);
        Console.WriteLine($"  NzdMatrice: {gm}, slijedno: {gs}, isto: {Da(gm == gs)}");   // 1, 1, da

        var velika = Matrica(1000, 1000, 3, 2026);
        ocekivano = NzdRedakaSlijedno(velika);
        Console.WriteLine("Test 2: 1000 × 1000, svi elementi djeljivi s 3");
        Console.WriteLine($"  NzdRedaka isto: {Da(NzdRedaka(velika).SequenceEqual(ocekivano))}, NzdRedakaSlijednoURetku isto: {Da(NzdRedakaSlijednoURetku(velika).SequenceEqual(ocekivano))}");
        gm = NzdMatrice(velika);
        gs = ocekivano.Aggregate(Nzd);
        Console.WriteLine($"  NzdMatrice: {gm}, slijedno: {gs}, isto: {Da(gm == gs)}");   // 3, 3, da

        Console.WriteLine();
        Console.WriteLine($"Mjerenje: ms, najkraće od 3, {Environment.ProcessorCount} logičkih jezgri (kriv rezultat: »krivo«)");
        Console.WriteLine($"{"matrica",-16}{"slijedno",12}{"NzdRedaka",14}{"SlijednoURetku",18}");
        foreach (var (retci, stupci) in new[] { (1000, 1000), (12, 1_000_000), (100_000, 10) })
        {
            var m = Matrica(retci, stupci, 1, 2026);
            var tocan = NzdRedakaSlijedno(m);
            double t0 = Najkrace(() => NzdRedakaSlijedno(m).SequenceEqual(tocan), out _);
            double t1 = Najkrace(() => NzdRedaka(m).SequenceEqual(tocan), out bool ok1);
            double t2 = Najkrace(() => NzdRedakaSlijednoURetku(m).SequenceEqual(tocan), out bool ok2);
            string Vrijeme(double t, bool ok) => ok ? t.ToString("F1") : "krivo";
            Console.WriteLine($"{$"{retci:N0} × {stupci:N0}",-16}{t0,12:F1}{Vrijeme(t1, ok1),14}{Vrijeme(t2, ok2),18}");   // 1.000 × 1.000: 3,4 8,1 0,7; 12 × 1.000.000: 42,6 33,2 7,8; 100.000 × 10: 4,8 117,2 2,1
        }
    }
}
