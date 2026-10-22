# Vježba 2

Početni projekti za studente. Zadaci su opisani u `Vjezba2_upute.html`.

## Struktura

Svaki zadatak je zaseban projekt u rješenju `Vjezba2.sln`, pa se pokreće samostalno, bez izbornika. Cijeli zadatak, zajedno s početnim naredbama, nalazi se u jednoj datoteci.

| Zadatak | Mapa projekta | Datoteka |
|---|---|---|
| 1 | `Zadatak1_Paralelno/` | `Zadatak1_Paralelno/Zadatak1_Paralelno.cs` |
| 2 | `Zadatak2_Task/` | `Zadatak2_Task/Zadatak2_Task.cs` |
| 3 | `Zadatak3_Zamke/` | `Zadatak3_Zamke/Zadatak3_Zamke.cs` |

## Otvaranje i pokretanje

- **Visual Studio:** otvorite `Vjezba2.sln`. Desni klik na projekt zadatka > *Set as Startup Project*, zatim F5 (ili Ctrl+F5).
- **JetBrains Rider:** File > Open i odaberite `Vjezba2.sln`. U popisu konfiguracija pokretanja (gore desno) odaberite projekt zadatka i pokrenite ga.
- **Visual Studio Code:** File > Open Folder i odaberite ovu mapu. Instalirajte predloženo proširenje C# Dev Kit (ili C#). Pokretanje: F5, konfiguracija »Vježba 2: zadatak N (Debug)«. Za mjerenje vremena (zadaci 1 i 2) odaberite konfiguraciju **Release**.
- **Naredbeni redak:** u mapi zadatka `dotnet run`, ili iz ove mape `dotnet run --project Zadatak1_Paralelno`. Za mjerenja: `dotnet run -c Release`.

Projekti ciljaju .NET 10. Ako imate samo Visual Studio 2022, u `Directory.Build.props` (zajednička datoteka za sve zadatke) promijenite `net10.0` u `net8.0`, a u `.vscode/launch.json` isto u putanjama programa.
