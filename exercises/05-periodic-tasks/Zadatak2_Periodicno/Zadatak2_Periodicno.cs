// Zadatak 2: Periodicno(akcija, ms)
// Periodicno u novom tasku poziva akciju svakih ms milisekundi, beskonačno.
// Test: akcija ispisuje redni broj poziva i vrijeme od početka, a traje 300 ms. Glavna nit čeka 9,5 s pa završi
// (taskovi iz fonda niti završe zajedno s programom).

using System.Diagnostics;

static class Zadatak2_Periodicno
{
    static void Periodicno(Action akcija, int ms)
    {
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var sat = Stopwatch.StartNew();
        int poziv = 0;
        Action akcija = () =>
        {
            poziv++;
            Console.WriteLine($"poziv {poziv,2}: {sat.ElapsedMilliseconds,6} ms");
            Thread.Sleep(300);   // posao akcije
        };

        Periodicno(akcija, 1000);
        Thread.Sleep(9_500);
        Console.WriteLine($"Kraj nakon {sat.ElapsedMilliseconds} ms, poziva: {poziv}");
    }
}
