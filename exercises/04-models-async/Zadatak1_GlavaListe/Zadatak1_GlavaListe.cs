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

    // Tri uzastopna Parallel.For: kraj svake petlje je ograda
    static int TriPetlje(int[] next)
    {
        int n = next.Length;
        var M = new int[n];
        Parallel.For(0, n, i => M[i] = 0);   // korak 1 za sve
        Parallel.For(0, n, i => { if (next[i] >= 0) M[next[i]] = 1; });   // korak 2 za sve; svaki čvor ima najviše jednog prethodnika, pa nema sudara
        return M.Count(m => m == 0);   // korak 3: ostane 0 samo ćelija glave; 1
    }

    // p taskova, čvorovi i = id, id + p, ...; Barrier između koraka
    static int SOgradom(int[] next, int p)
    {
        int n = next.Length;
        var M = new int[n];
        int glave = 0;
        using var ograda = new Barrier(p);
        Task.WaitAll(Enumerable.Range(0, p).Select(id => Task.Run(() =>
        {
            for (int i = id; i < n; i += p) M[i] = 0;
            ograda.SignalAndWait();   // nitko ne piše jedinice prije nego svi upišu nule
            for (int i = id; i < n; i += p) if (next[i] >= 0) M[next[i]] = 1;
            ograda.SignalAndWait();   // nitko ne čita M prije nego svi upišu jedinice
            for (int i = id; i < n; i += p) if (M[i] == 0) Interlocked.Increment(ref glave);
        })).ToArray());
        return glave;   // 1
    }

    public static void Pokreni()
    {
        var (next, glava) = NapraviListu(N);
        Console.WriteLine($"Lista od {N} čvorova, prava glava: {glava}");
        Console.WriteLine($"Parallel.For, bez ograde:  {BezOgrade(next)} glava");   // 1000 glava (krivo: čvor prepiše jedinicu prethodnika ili pročita prerano)
        Console.WriteLine($"Tri Parallel.For:          {TriPetlje(next)} glava");   // 1 glava
        Console.WriteLine($"Barrier, 12 taskova:       {SOgradom(next, 12)} glava");   // 1 glava
    }
}
