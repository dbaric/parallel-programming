// Zadatak 4: generički delegati
// Ne moramo deklarirati vlastiti delegat. Ugrađeni su:
//   Action<T...>      metoda koja ne vraća vrijednost
//   Func<T..., R>     metoda koja vraća vrijednost tipa R (zadnji tip)
//   Predicate<T>      metoda koja vraća bool

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak4_GenerickiDelegati.Pokreni();

static class Zadatak4_GenerickiDelegati
{
    public static void Pokreni()
    {
        Func<int, int, int> zbroj = (a, b) => a + b;
        Console.WriteLine($"zbroj(3, 4) = {zbroj(3, 4)}");

        Action<string> pozdrav = ime => Console.WriteLine($"Bok, {ime}!");
        pozdrav("svijete");

        Predicate<int> pozitivan = x => x > 0;
        Console.WriteLine($"pozitivan(-5) = {pozitivan(-5)}");

        int[] niz = { 5, -2, 9, 0, -7, 4 };
        Console.WriteLine("Pozitivni (LINQ Where): " + string.Join(", ", niz.Where(x => x > 0)));
        Console.WriteLine("Kvadrati (LINQ Select): " + string.Join(", ", niz.Select(x => x * x)));
    }
}
