# Vježba 1

Početni projekti za studente. Zadaci su opisani u `Vjezba1_upute.html`.

## Struktura

Svaki zadatak je zaseban projekt u rješenju `Vjezba1.sln`, pa se pokreće samostalno, bez izbornika. Cijeli zadatak, zajedno s početnim naredbama, nalazi se u jednoj datoteci.

| Zadatak | Mapa projekta | Datoteka |
|---|---|---|
| 1 | `Zadatak1_Delegati/` | `Zadatak1_Delegati/Zadatak1_Delegati.cs` |
| 2 | `Zadatak2_UniverzalnaFunkcija/` | `Zadatak2_UniverzalnaFunkcija/Zadatak2_UniverzalnaFunkcija.cs` |
| 3 | `Zadatak3_Lambda/` | `Zadatak3_Lambda/Zadatak3_Lambda.cs` |
| 4 | `Zadatak4_GenerickiDelegati/` | `Zadatak4_GenerickiDelegati/Zadatak4_GenerickiDelegati.cs` |
| 5 | `Zadatak5_GenerickaKlasa/` | `Zadatak5_GenerickaKlasa/Zadatak5_GenerickaKlasa.cs` |

## Otvaranje i pokretanje

- **Docker (preporučeno u ovom repozitoriju):** iz korijena repozitorija
  - zadatak: `docker compose run --rm dotnet dotnet run --project exercises/01-delegates/Zadatak1_Delegati`
  - HTML stranice: `docker compose up web`, zatim http://localhost:8080/01-delegates/Vjezba1_upute.html i http://localhost:8080/01-delegates/Vizualizacije/delegati.html

- **Visual Studio:** otvorite `Vjezba1.sln`. Desni klik na projekt zadatka > *Set as Startup Project*, zatim F5 (ili Ctrl+F5).
- **JetBrains Rider:** File > Open i odaberite `Vjezba1.sln`. U popisu konfiguracija pokretanja (gore desno) odaberite projekt zadatka i pokrenite ga.
- **Visual Studio Code:** File > Open Folder i odaberite ovu mapu. Instalirajte predloženo proširenje C# Dev Kit (ili C#). Pokretanje: F5, konfiguracija »Vježba 1: zadatak N (Debug)«.
- **Naredbeni redak:** u mapi zadatka `dotnet run`, ili iz ove mape `dotnet run --project Zadatak1_Delegati`.

Projekti ciljaju .NET 10. Ako imate samo Visual Studio 2022, u `Directory.Build.props` (zajednička datoteka za sve zadatke) promijenite `net10.0` u `net8.0`, a u `.vscode/launch.json` isto u putanjama programa.
