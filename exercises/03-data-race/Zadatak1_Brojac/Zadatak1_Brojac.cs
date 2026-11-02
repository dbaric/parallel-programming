// Zadatak 1: data race na brojaču
// 12 taskova povećava isti brojač po 1 000 000 puta (očekivano 12 000 000). Zadano je rješenje bez zaštite.
// VAŽNO: vrijeme mjerite u Release konfiguraciji.

using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak1_Brojac.Pokreni();

static class Zadatak1_Brojac
{
    const int K = 12;
    const int Povecanja = 1_000_000;

    // Pokrene K taskova, svaki izvodi posao; vraća brojač i vrijeme (ms)
    static (long, long) Izvedi(Action<int> posao)
    {
        brojac = 0;
        var sat = Stopwatch.StartNew();
        Task.WaitAll(Enumerable.Range(0, K).Select(j => Task.Run(() => posao(j))).ToArray());
        return (brojac, sat.ElapsedMilliseconds);
    }

    static long brojac;

    static void Ispisi(string naziv, (long b, long ms) r)
    {
        Console.WriteLine($"{naziv,-28} {r.b,12:N0}  ({r.ms} ms)");
    }

    public static void Pokreni()
    {
        Ispisi("Bez zaštite", Izvedi(_ => { for (int i = 0; i < Povecanja; i++) brojac++; }));   // manje od 12.000.000 i svaki put drukčije: brojac++ su tri koraka (učitaj, povećaj, upiši)

        var brava = new object();   // jedna, stvorena prije taskova
        Ispisi("lock, zajednička brava", Izvedi(_ => { for (int i = 0; i < Povecanja; i++) lock (brava) { brojac++; } }));   // 12.000.000, točno ali sporo: ≈ 155 ms naspram ≈ 5 ms slijedno

        Ispisi("lock, svaki task svoja", Izvedi(_ =>
        {
            var svoja = new object();   // svoja brava: nitko nikoga ne čeka
            for (int i = 0; i < Povecanja; i++) lock (svoja) { brojac++; }
        }));   // npr. 3.195.136, manje od 12.000.000: lock postoji, ali ne štiti

        Ispisi("Interlocked.Increment", Izvedi(_ => { for (int i = 0; i < Povecanja; i++) Interlocked.Increment(ref brojac); }));   // 12.000.000, ≈ 246 ms (na ovom računalu nije brže od locka: mjerite na svom)

        Ispisi("lokalno + Interlocked.Add", Izvedi(_ =>
        {
            long lokalno = 0;
            for (int i = 0; i < Povecanja; i++) lokalno++;   // bez dijeljenja
            Interlocked.Add(ref brojac, lokalno);   // jedna sinkronizacija po tasku
        }));   // 12.000.000, najbrže (≈ 6 ms)
    }
}
