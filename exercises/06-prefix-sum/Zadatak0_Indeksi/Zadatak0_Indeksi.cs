// Zadatak 0: indeksi stabla
// IspisiNizove(n): nizovi 1 3 5 7 …; 3 7 11 15 …; 7 15 23 31 …; … (samo brojevi manji od n), svaki niz u svom retku.
// IspisiParove(n): za svaku razinu reduciranja svi parovi a[i] := a[j] ⊕ a[i] te razine, u jednom retku.
// ReducirajSlijedno(a): zbrajanje stablom (istim parovima), razinu po razinu, ali slijedno; na mjestu; vraća a[n − 1].
// Main ispisuje nizove za n = 32, parove za n = 8 i 16, zatim zbraja 1, 2, …, n za n = 16 i n = 20.

using System.Globalization;

static class Zadatak0_Indeksi
{
    // Napišite.
    static void IspisiNizove(int n)
    {
    }

    // Napišite.
    static void IspisiParove(int n)
    {
    }

    // Napišite: zbrajanje stablom, slijedno, na mjestu (mijenja a). Vraća a[n − 1].
    static long ReducirajSlijedno(long[] a)
    {
        return 0;
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("hr-HR");

        Console.WriteLine("Nizovi za n = 32:");
        IspisiNizove(32);
        Console.WriteLine("Parovi za n = 8:");
        IspisiParove(8);
        Console.WriteLine("Parovi za n = 16:");
        IspisiParove(16);

        foreach (int n in new[] { 16, 20 })
        {
            var a = new long[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = i + 1;
            }
            long stablo = ReducirajSlijedno(a);
            long zbroj = (long)n * (n + 1) / 2;
            Console.WriteLine($"n = {n}: a[{n - 1}] = {stablo}, zbroj 1 … {n} = {zbroj}, isto: {(stablo == zbroj ? "da" : "ne")}");
        }
    }
}
