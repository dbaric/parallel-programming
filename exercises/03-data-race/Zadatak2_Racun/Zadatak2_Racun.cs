// Zadatak 2: provjeri, pa promijeni
// Račun ima 10 000 €; 12 taskova isplaćuje po 1 € dok ima novca. Svaki korak je atomaran (Interlocked),
// a stanje ipak ode ispod nule. Popravite.

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak2_Racun.Pokreni();

static class Zadatak2_Racun
{
    const int K = 12;
    const long Pocetno = 10_000;

    static long stanje, isplata;

    static void Ispisi(string naziv) =>
        Console.WriteLine($"{naziv,-30} stanje {stanje,4}, isplata {isplata:N0}");

    static void Izvedi(Action isplati)
    {
        stanje = Pocetno;
        isplata = 0;
        Task.WaitAll(Enumerable.Range(0, K).Select(_ => Task.Run(() =>
        {
            for (int i = 0; i < 2 * Pocetno; i++) isplati();   // svaki task pokuša više puta nego ima novca
        })).ToArray());
    }

    public static void Pokreni()
    {
        Izvedi(() =>
        {
            if (Interlocked.Read(ref stanje) >= 1)   // provjeri
            {
                Interlocked.Decrement(ref stanje);   // promijeni
                Interlocked.Increment(ref isplata);
            }
        });
        Ispisi("Provjeri, pa promijeni");   // najčešće stanje ispod nule (npr. −2), isplata > 10.000 (ponekad se ne vidi, pokrenite više puta): race condition, svaki korak je zaštićen, a cjelina nije

        var brava = new object();
        Izvedi(() =>
        {
            lock (brava)   // provjera i promjena zajedno
            {
                if (stanje >= 1) { stanje--; isplata++; }
            }
        });
        Ispisi("lock oko oba koraka");   // stanje 0, isplata 10.000

        Izvedi(() =>
        {
            while (true)
            {
                long staro = Interlocked.Read(ref stanje);
                if (staro < 1) break;   // nema novca
                if (Interlocked.CompareExchange(ref stanje, staro - 1, staro) == staro)   // upiše samo ako se stanje nije promijenilo
                {
                    Interlocked.Increment(ref isplata);
                    break;
                }
            }   // inače je netko bio brži: ponovi
        });
        Ispisi("CompareExchange");   // stanje 0, isplata 10.000
    }
}
