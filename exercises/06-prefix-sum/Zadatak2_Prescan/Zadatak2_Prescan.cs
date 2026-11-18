// Zadatak 2: prescan i scan
// Prescan(a, op, neutralni) mijenja a u prescan: a[i] postaje a[0] ⊕ … ⊕ a[i − 1], a a[0] neutralni. Vraća a[0] ⊕ … ⊕ a[n − 1].
// Scan(a, op, neutralni) vraća novi niz: element i je a[0] ⊕ … ⊕ a[i]; a ostaje nepromijenjen.
// n ne mora biti potencija broja 2. Operacija ⊕ je asocijativna, ali ne nužno komutativna.
// Main za svaki test uspoređuje sa slijednim i ispisuje »isto kao slijedno: da/ne«.

using System.Globalization;

static class Zadatak2_Prescan
{
    static T Prescan<T>(T[] a, Func<T, T, T> op, T neutralni)
    {
        int m = 1;
        while (m < a.Length)
        {
            m *= 2;
        }
        var b = new T[m];
        Array.Fill(b, neutralni);
        a.CopyTo(b, 0);
        for (int d = 2; d <= m; d *= 2)
        {
            Parallel.For(1, m / d + 1, k => b[k * d - 1] = op(b[k * d - 1 - d / 2], b[k * d - 1]));
        }
        T ukupno = b[m - 1];
        b[m - 1] = neutralni;
        for (int d = m; d >= 2; d /= 2)
        {
            Parallel.For(1, m / d + 1, k =>
            {
                T lijevo = b[k * d - 1 - d / 2];
                b[k * d - 1 - d / 2] = b[k * d - 1];
                b[k * d - 1] = op(b[k * d - 1], lijevo);
            });
        }
        Array.Copy(b, a, a.Length);
        return ukupno;
    }

    static T[] Scan<T>(T[] a, Func<T, T, T> op, T neutralni)
    {
        var b = (T[])a.Clone();
        T ukupno = Prescan(b, op, neutralni);
        return b.Skip(1).Append(ukupno).ToArray();
    }

    // Zadano: slijedni scan.
    static T[] ScanSlijedno<T>(T[] a, Func<T, T, T> op)
    {
        var b = new T[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            b[i] = i == 0 ? a[0] : op(b[i - 1], a[i]);
        }
        return b;
    }

    static void Test<T>(string naziv, T[] ulaz, Func<T, T, T> op, T neutralni, bool ispisi = false)
    {
        T[] ocekivano = ScanSlijedno(ulaz, op);
        var kopija = (T[])ulaz.Clone();
        T[] scan = Scan(ulaz, op, neutralni);
        T ukupno = Prescan(kopija, op, neutralni);
        bool prescanIsti = ulaz.Length > 0 && EqualityComparer<T>.Default.Equals(kopija[0], neutralni);
        for (int i = 1; i < ulaz.Length && prescanIsti; i++)
        {
            prescanIsti = EqualityComparer<T>.Default.Equals(kopija[i], ocekivano[i - 1]);
        }
        bool ukupnoIsto = EqualityComparer<T>.Default.Equals(ukupno, ocekivano[^1]);
        Console.WriteLine(naziv);
        if (ispisi)
        {
            Console.WriteLine($"  Prescan: {string.Join(" ", kopija.Select(x => $"[{x}]"))}");   // test 1: [0] [3] [4] [11] [11] [15] [16] [22]; test 5: [] [a] [ab] [abc] [abcd] [abcde]
            Console.WriteLine($"  Scan:    {string.Join(" ", scan.Select(x => $"[{x}]"))}");   // test 1: [3] [4] [11] [11] [15] [16] [22] [25]; test 5: [a] [ab] [abc] [abcd] [abcde] [abcdef]
        }
        Console.WriteLine($"  Prescan isto kao slijedno: {(prescanIsti && ukupnoIsto ? "da" : "ne")}, Scan isto kao slijedno: {(scan.SequenceEqual(ocekivano) ? "da" : "ne")}");   // da, da u svih pet testova
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("hr-HR");

        Test("Test 1: 3 1 7 0 4 1 6 3, zbrajanje", new long[] { 3, 1, 7, 0, 4, 1, 6, 3 }, (x, y) => x + y, 0L, ispisi: true);

        var slucajni = new Random(2026);
        var veliki = new long[1 << 22];
        for (int i = 0; i < veliki.Length; i++)
        {
            veliki[i] = slucajni.Next(0, 100);
        }
        Test("Test 2: n = 2^22, zbrajanje", veliki, (x, y) => x + y, 0L);

        var tisucu = new long[1000];
        for (int i = 0; i < tisucu.Length; i++)
        {
            tisucu[i] = slucajni.Next(0, 100);
        }
        Test("Test 3: n = 1000, zbrajanje", tisucu, (x, y) => x + y, 0L);

        var negativni = new long[1000];
        for (int i = 0; i < negativni.Length; i++)
        {
            negativni[i] = slucajni.Next(-100, 0);
        }
        Test("Test 4: n = 1000, brojevi od −100 do −1, najveći dosad (max)", negativni, Math.Max, long.MinValue);

        Test("Test 5: a b c d e f, spajanje nizova znakova", new[] { "a", "b", "c", "d", "e", "f" }, (x, y) => x + y, "", ispisi: true);
    }
}
