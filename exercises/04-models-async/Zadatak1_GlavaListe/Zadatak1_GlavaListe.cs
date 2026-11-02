// Zadatak 1: glava vezane liste (PRAM, ograda)
// next[i] je sljedbenik čvora i (-1 ako ga nema); glava je jedini čvor koji nema prethodnika.
// PRAM algoritam: 1) M[i] := 0   2) M[next[i]] := 1   3) ako je M[i] = 0, onda je i glava.
// Zadana je neispravna izvedba bez ograde. Napišite ispravne izvedbe s ogradom između koraka.

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak1_GlavaListe.Pokreni();

static class Zadatak1_GlavaListe
{
    const int N = 1000;

    // Nasumična lista od n čvorova; vraća next i pravu glavu
    static (int[] next, int glava) NapraviListu(int n)
    {
        var redoslijed = Enumerable.Range(0, n).OrderBy(_ => Random.Shared.Next()).ToArray();
        var next = new int[n];
        for (int k = 0; k < n; k++) next[redoslijed[k]] = k + 1 < n ? redoslijed[k + 1] : -1;
        return (next, redoslijed[0]);
    }

    // Krivo: sva tri koraka u jednoj petlji, bez ograde
    static int BezOgrade(int[] next)
    {
        int n = next.Length;
        var M = new int[n];
        var glave = new System.Collections.Concurrent.ConcurrentBag<int>();
        Parallel.For(0, n, i =>
        {
            M[i] = 0;
            if (next[i] >= 0) M[next[i]] = 1;
            if (M[i] == 0) glave.Add(i);
        });
        return glave.Count;
    }

    // TODO a): tri uzastopna Parallel.For (kraj petlje čeka sve iteracije); vrati broj pronađenih glava
    // TODO b): p taskova i Barrier; svaki task obrađuje čvorove i = id, id + p, ...; ograda između koraka

    public static void Pokreni()
    {
        var (next, glava) = NapraviListu(N);
        Console.WriteLine($"Lista od {N} čvorova, prava glava: {glava}");
        Console.WriteLine($"Parallel.For, bez ograde:  {BezOgrade(next)} glava");
    }
}
