// Zadatak 5 (neobavezan): binarni brojevi u dekadske
// Binarni broj je niz bitova duljine N (N je potencija broja 2); bitovi[0] je bit najveće težine.
// UDekadski(bitovi): vrijednost broja, paralelno (reduciranjem ili Prescanom iz zadatka 2).
// SviUDekadske(brojevi): vrijednosti svih brojeva, paralelno; rezultat istim redoslijedom kao ulaz.
// Prefiksi(bitovi): element k je vrijednost prvih k + 1 bitova kao binarnog broja; pomoću Scan iz zadatka 2.
// Main uspoređuje s Convert.ToUInt64(s, 2).

using System.Globalization;

static class Zadatak5_Binarni
{
    // Napišite.
    static ulong UDekadski(int[] bitovi)
    {
        return 0;
    }

    // Napišite.
    static ulong[] SviUDekadske(int[][] brojevi)
    {
        return new ulong[brojevi.Length];
    }

    // Napišite.
    static ulong[] Prefiksi(int[] bitovi)
    {
        return new ulong[bitovi.Length];
    }

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
            Console.WriteLine($"  {s} → {v}, Convert: {c}, isto: {Da(v == c)}");
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
            Console.WriteLine($"  {s}: {string.Join(" ", p)}, isto: {Da(p.SequenceEqual(ocekivano))}");
        }
        bool prefiksiIsti = svi.Skip(8).All(s => Prefiksi(Bitovi(s)).SequenceEqual(Enumerable.Range(1, s.Length).Select(k => Convert.ToUInt64(s[..k], 2))));
        Console.WriteLine($"  N = 16, 32, 64 (24 broja): isto: {Da(prefiksiIsti)}");
    }
}
