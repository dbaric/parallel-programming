// Zadatak 2: univerzalna funkcija
// Jedna funkcija radi za sve uvjete jer se uvjet predaje kao delegat.

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak2_UniverzalnaFunkcija.Pokreni();

delegate bool Uvjet(int x);

static class Zadatak2_UniverzalnaFunkcija
{
    static bool JeParan(int x) => x % 2 == 0;
    static bool JeVeciOdDeset(int x) => x > 10;

    static void Ispisi(int[] niz, Uvjet uvjet)
    {
        foreach (int x in niz)
            if (uvjet(x))
                Console.Write(x + " ");
        Console.WriteLine();
    }

    public static void Pokreni()
    {
        int[] niz = { 3, 8, 12, 15, 20, 7, 4 };

        Console.Write("Parni: ");
        Ispisi(niz, JeParan);

        Console.Write("Veći od 10: ");
        Ispisi(niz, JeVeciOdDeset);
    }
}
