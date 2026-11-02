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
        Ispisi("Bez zaštite", Izvedi(_ => { for (int i = 0; i < Povecanja; i++) brojac++; }));

        // TODO a): popravak lockom (jedna zajednička brava)
        // TODO b): zamka: svaki task svoja brava. Koliko je brojač sada?
        // TODO c): Interlocked.Increment
        // TODO d): lokalni brojač u tasku, pa jedan Interlocked.Add na kraju
    }
}
