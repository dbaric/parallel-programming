// Zadatak 5 (neobavezan): binarni brojevi u dekadske
// Binarni broj je niz bitova duljine N (N je potencija broja 2); bitovi[0] je bit najveće težine.
// UDekadski(bitovi): vrijednost broja, paralelno (reduciranjem ili Prescanom iz zadatka 2).
// SviUDekadske(brojevi): vrijednosti svih brojeva, paralelno; rezultat istim redoslijedom kao ulaz.
// Prefiksi(bitovi): element k je vrijednost prvih k + 1 bitova kao binarnog broja; pomoću Scan iz zadatka 2.
// Main uspoređuje s Convert.ToUInt64(s, 2).

using System.Globalization;
using Dio = (ulong v, int l);

static class Zadatak5_Binarni
{
    static Dio Spoji(Dio x, Dio y) => ((x.v << y.l) | y.v, x.l + y.l);

    static Dio[] Dijelovi(int[] bitovi) => bitovi.Select(b => ((ulong)b, 1)).ToArray();

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

    static ulong UDekadski(int[] bitovi) => Prescan<Dio>(Dijelovi(bitovi), Spoji, (0UL, 0)).v;

    static ulong[] SviUDekadske(int[][] brojevi)
    {
        var rezultat = new ulong[brojevi.Length];
        Parallel.For(0, brojevi.Length, i => rezultat[i] = UDekadski(brojevi[i]));
        return rezultat;
    }

    static ulong[] Prefiksi(int[] bitovi) => Scan<Dio>(Dijelovi(bitovi), Spoji, (0UL, 0)).Select(x => x.v).ToArray();

    static int[] Bitovi(string s) => s.Select(c => c - '0').ToArray();

    static string Da(bool b) => b ? "da" : "ne";

    // Zadano: osam slučajnih brojeva od N bitova; prvi je N jedinica.
    static string[] Slucajni(int n, Random slucajni)
    {
        var s = new string[8];
        s[0] = new string('1', n);
        for (int k = 1; k < s.Length; k++)
        {
            s[k] = string.Concat(Enumerable.Range(0, n).Select(_ => slucajni.Next(2) == 0 ? '0' : '1'));
        }
        return s;
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("hr-HR");

        string[] osam = { "10100100", "10100101", "00000001", "10000000", "11010010", "01101100", "11111111", "00110101" };
        Console.WriteLine("Test 1: N = 8");
        foreach (string s in osam)
        {
            ulong v = UDekadski(Bitovi(s));
            ulong c = Convert.ToUInt64(s, 2);
            Console.WriteLine($"  {s} → {v}, Convert: {c}, isto: {Da(v == c)}");   // 10100100 → 164, 10100101 → 165, 00000001 → 1, 10000000 → 128, … sve da
        }

        var slucajni = new Random(2026);
        var svi = new List<string>(osam);
        int test = 2;
        foreach (int n in new[] { 16, 32, 64 })
        {
            string[] brojevi = Slucajni(n, slucajni);
            svi.AddRange(brojevi);
            bool isto = brojevi.All(s => UDekadski(Bitovi(s)) == Convert.ToUInt64(s, 2));
            Console.WriteLine($"Test {test++}: N = {n}, 8 brojeva (prvi: {n} {(n % 10 is >= 2 and <= 4 && n % 100 / 10 != 1 ? "jedinice" : "jedinica")}), isto: {Da(isto)}");
        }

        var rezultat = SviUDekadske(svi.Select(Bitovi).ToArray());
        bool sviIsti = rezultat.SequenceEqual(svi.Select(s => Convert.ToUInt64(s, 2)));
        Console.WriteLine($"Test 5: SviUDekadske, {svi.Count} broja (N = 8, 16, 32, 64), isto i istim redoslijedom: {Da(sviIsti)}");

        Console.WriteLine("Test 6: Prefiksi");
        foreach (string s in new[] { "10100100", "10100101", "11010010" })
        {
            ulong[] p = Prefiksi(Bitovi(s));
            var ocekivano = Enumerable.Range(1, s.Length).Select(k => Convert.ToUInt64(s[..k], 2));
            Console.WriteLine($"  {s}: {string.Join(" ", p)}, isto: {Da(p.SequenceEqual(ocekivano))}");   // 10100100: 1 2 5 10 20 41 82 164, da
        }
        bool prefiksiIsti = svi.Skip(8).All(s => Prefiksi(Bitovi(s)).SequenceEqual(Enumerable.Range(1, s.Length).Select(k => Convert.ToUInt64(s[..k], 2))));
        Console.WriteLine($"  N = 16, 32, 64 (24 broja): isto: {Da(prefiksiIsti)}");
    }
}
