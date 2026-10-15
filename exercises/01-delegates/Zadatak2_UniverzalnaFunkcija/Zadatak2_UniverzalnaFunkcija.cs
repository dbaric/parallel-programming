// Zadatak 2: univerzalna funkcija
// Jedna funkcija radi za sve uvjete jer se uvjet predaje kao delegat.

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak2_UniverzalnaFunkcija.Pokreni();

delegate bool Uvjet(int x);

static class Zadatak2_UniverzalnaFunkcija
{
    static bool JeParan(int x) => x % 2 == 0;
    static bool JeVeciOdDeset(int x) => x > 10;
    static bool JeDjeljivSTri(int x) => x % 3 == 0;

    static int Ispisi(int[] niz, Uvjet uvjet)
    {
        int broj = 0;
        foreach (int x in niz)
            if (uvjet(x))
            {
                Console.Write(x + " ");   // 8 12 20 4 za prvi poziv
                broj++;
            }
        Console.WriteLine();   // novi redak
        return broj;
    }

    public static void Pokreni()
    {
        int[] niz = { 3, 8, 12, 15, 20, 7, 4 };

        Console.Write("Parni: ");   // Parni:
        Ispisi(niz, JeParan);   // 8 12 20 4

        Console.Write("Veći od 10: ");   // Veći od 10:
        Ispisi(niz, JeVeciOdDeset);   // 12 15 20

        Console.Write("Djeljivi s 3: ");   // Djeljivi s 3:
        int ispisano = Ispisi(niz, JeDjeljivSTri);   // 3 12 15
        Console.WriteLine($"Ispisano elemenata: {ispisano}");   // Ispisano elemenata: 3
    }
}
