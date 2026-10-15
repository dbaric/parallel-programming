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
        Console.WriteLine($"zbroj(3, 4) = {zbroj(3, 4)}");   // zbroj(3, 4) = 7

        Action<string> pozdrav = ime => Console.WriteLine($"Bok, {ime}!");   // Bok, svijete!
        pozdrav("svijete");

        Predicate<int> pozitivan = x => x > 0;
        Console.WriteLine($"pozitivan(-5) = {pozitivan(-5)}");   // pozitivan(-5) = False

        Func<double, double, double> hipotenuza = (a, b) => Math.Sqrt(a * a + b * b);
        Console.WriteLine($"Hipotenuza(3, 4) = {hipotenuza(3, 4)}");   // Hipotenuza(3, 4) = 5

        int[] niz = { 5, -2, 9, 0, -7, 4 };
        int[] parni = Array.FindAll(niz, x => x % 2 == 0);
        Console.WriteLine("Parni: " + string.Join(", ", parni));   // Parni: -2, 0, 4

        Action<int[]> ispisi = a => Console.WriteLine(string.Join(" ", a));   // 5 -2 9 0 -7 4
        ispisi(niz);
        Console.WriteLine("Pozitivni (LINQ Where): " + string.Join(", ", niz.Where(x => x > 0)));   // Pozitivni (LINQ Where): 5, 9, 4
        Console.WriteLine("Kvadrati (LINQ Select): " + string.Join(", ", niz.Select(x => x * x)));   // Kvadrati (LINQ Select): 25, 4, 81, 0, 49, 16
    }
}
