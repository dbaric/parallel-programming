// Zadatak 1: delegati
// Delegat je tip koji opisuje potpis metode. Varijabla tog tipa može pokazivati na bilo koju
// metodu s istim potpisom, pa se metoda može predati drugoj metodi kao argument.

Console.OutputEncoding = System.Text.Encoding.UTF8;
Zadatak1_Delegati.Pokreni();

delegate int Calc(int x);

static class Zadatak1_Delegati
{
    const int n = 50;

    static int Oduzmi(int x) => x - n;
    static int Pomnozi(int x) => x * n;
    static int Podijeli(int x) => x / n;

    public static void Pokreni()
    {
        Calc racun = Oduzmi;
        Console.WriteLine($"Oduzmi(100)  = {racun(100)}");   // Oduzmi(100)  = 50

        racun = Pomnozi;
        Console.WriteLine($"Pomnozi(100) = {racun(100)}");   // Pomnozi(100) = 5000

        racun = Podijeli;
        Console.WriteLine($"Podijeli(100) = {racun(100)}");   // Podijeli(100) = 2

        Calc oboje = Oduzmi;
        oboje += Pomnozi;
        // Poziv vraća vrijednost zadnje metode u listi poziva
        Console.WriteLine($"oboje(100) = {oboje(100)}");   // oboje(100) = 5000
        Console.WriteLine($"Broj metoda u listi poziva: {oboje.GetInvocationList().Length}");   // Broj metoda u listi poziva: 2
    }
}
