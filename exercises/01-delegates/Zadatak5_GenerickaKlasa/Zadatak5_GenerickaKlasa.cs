// Zadatak 5: generička klasa
// Stog<T> radi s bilo kojim tipom. Dopunite ga metodama koje primaju delegate.

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak5_GenerickaKlasa.Pokreni();

class Stog<T>
{
    readonly List<T> elementi = new();

    public int Velicina => elementi.Count;

    public void Stavi(T x) => elementi.Add(x);

    public void ZaSvaki(Action<T> akcija)
    {
        for (int i = elementi.Count - 1; i >= 0; i--) akcija(elementi[i]);
    }

    public int Broji(Predicate<T> uvjet) => elementi.Count(x => uvjet(x));

    public T Skini()
    {
        if (elementi.Count == 0) throw new InvalidOperationException("Stog je prazan.");
        T x = elementi[^1];
        elementi.RemoveAt(elementi.Count - 1);
        return x;
    }
}

static class Zadatak5_GenerickaKlasa
{
    public static void Pokreni()
    {
        var brojevi = new Stog<int>();
        for (int i = 1; i <= 5; i++) brojevi.Stavi(i * 10);
        Console.WriteLine($"Skinuto: {brojevi.Skini()}, veličina: {brojevi.Velicina}");   // Skinuto: 50, veličina: 4

        var rijeci = new Stog<string>();
        rijeci.Stavi("paralelno");
        rijeci.Stavi("programiranje");
        rijeci.ZaSvaki(Console.WriteLine);   // programiranje / paralelno

        Console.WriteLine($"Brojeva većih od 20: {brojevi.Broji(x => x > 20)}");   // Brojeva većih od 20: 2
    }
}
