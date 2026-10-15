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

    public static void Pokreni()
    {
        Calc racun = Oduzmi;
        Console.WriteLine($"Oduzmi(100)  = {racun(100)}");

        racun = Pomnozi;
        Console.WriteLine($"Pomnozi(100) = {racun(100)}");
    }
}
