// Zadatak 5 (neobavezno): ograda i task koji zastane
// Glava vezane liste s predavanja 5 (primjer 3): p taskova, svaki za svoje čvorove, ograda (Barrier) između koraka.
// Jedan nasumično odabran task prije druge ograde zastane 10 s (Thread.Sleep), kao procesor koji kasni.

using System.Diagnostics;

static class Zadatak5_OgradaZastoj
{
    const int N = 1000;

    // Nasumična lista od N čvorova: redoslijed[k] je k-ti čvor liste, next[i] je sljedbenik od i (−1 za zadnji).
    static int[] NapraviListu(out int glava)
    {
        var rnd = new Random(1);
        int[] redoslijed = Enumerable.Range(0, N).OrderBy(_ => rnd.Next()).ToArray();
        int[] next = new int[N];
        for (int k = 0; k < N - 1; k++) next[redoslijed[k]] = redoslijed[k + 1];
        next[redoslijed[N - 1]] = -1;
        glava = redoslijed[0];
        return next;
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        int[] next = NapraviListu(out int pravaGlava);
        int p = 8;
        int sporiTask = new Random().Next(p);
        Console.WriteLine($"Prava glava: {pravaGlava}; {p} taskova, task {sporiTask} zastane 10 s");   // Prava glava: 654; 8 taskova, task 6 zastane 10 s

        var sat = Stopwatch.StartNew();
        var izvor = new CancellationTokenSource(2000);   // taskovi odustaju 2 s nakon početka
        var token = izvor.Token;
        int[] M = new int[N];
        int glava = -1;
        using var ograda = new Barrier(p);
        var taskovi = new Task[p];
        for (int id = 0; id < p; id++)
        {
            int moj = id;
            taskovi[id] = Task.Factory.StartNew(() =>
            {
                for (int i = moj; i < N; i += p) M[i] = 0;
                ograda.SignalAndWait();
                for (int i = moj; i < N; i += p) if (next[i] >= 0) M[next[i]] = 1;
                if (moj == sporiTask) token.WaitHandle.WaitOne(10_000);
                ograda.SignalAndWait(token);
                for (int i = moj; i < N; i += p) if (M[i] == 0) glava = i;
            }, TaskCreationOptions.LongRunning);
        }
        try
        {
            Task.WaitAll(taskovi);
            Console.WriteLine($"Glava: {glava} ({sat.ElapsedMilliseconds} ms)");
        }
        catch (AggregateException)
        {
            Console.WriteLine($"Rezultat nije dobiven ({sat.ElapsedMilliseconds} ms)");   // Rezultat nije dobiven (2010 ms)
        }
    }
}
