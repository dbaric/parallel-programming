// Zadatak 3: stog kao povezana lista
// 12 taskova stavlja po 100 000 čvorova na stog (očekivano 1 200 000). Zadan je stog bez zaštite.

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak3_Stog.Pokreni();

class Cvor(int vrijednost)
{
    public int Vrijednost = vrijednost;
    public Cvor? Sljedeci;
}

static class Zadatak3_Stog
{
    const int K = 12;
    const int PoTasku = 100_000;

    static Cvor? vrh;

    // Bez zaštite: dva taska mogu pročitati isti vrh, pa jedan čvor nestane
    static void PushBezZastite(int v)
    {
        var novi = new Cvor(v);
        novi.Sljedeci = vrh;   // (1)
        vrh = novi;            // (2)
    }

    static readonly object brava = new();

    // lock oko oba koraka
    static void PushLock(int v)
    {
        var novi = new Cvor(v);
        lock (brava)
        {
            novi.Sljedeci = vrh;
            vrh = novi;
        }
    }

    // Bez locka: korak (2) uspije samo ako je vrh još onaj iz koraka (1), inače ponovi
    static void PushCas(int v)
    {
        var novi = new Cvor(v);
        do
        {
            novi.Sljedeci = vrh;
        } while (Interlocked.CompareExchange(ref vrh, novi, novi.Sljedeci) != novi.Sljedeci);
    }

    static int Prebroji()
    {
        int n = 0;
        for (var c = vrh; c != null; c = c.Sljedeci) n++;
        return n;
    }

    static void Izvedi(string naziv, Action<int> push)
    {
        vrh = null;
        Task.WaitAll(Enumerable.Range(0, K).Select(_ => Task.Run(() =>
        {
            for (int i = 0; i < PoTasku; i++) push(i);
        })).ToArray());
        int n = Prebroji();
        Console.WriteLine($"{naziv,-18} {n,9:N0} čvorova, izgubljeno {K * PoTasku - n:N0}");
    }

    public static void Pokreni()
    {
        Izvedi("Bez zaštite", PushBezZastite);   // npr. 188.117 čvorova, izgubljeno ≈ 1.000.000
        Izvedi("lock", PushLock);   // 1.200.000 čvorova, izgubljeno 0
        Izvedi("CompareExchange", PushCas);   // 1.200.000 čvorova, izgubljeno 0
    }
}
