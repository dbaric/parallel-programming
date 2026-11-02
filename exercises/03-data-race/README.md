# Vježba 3

Data race, lock i Interlocked. Početni projekti za studente. Zadaci su opisani u `Vjezba3_upute.html`, a gradivo je u prezentaciji i mapi `Vizualizacije/` (`index.html`).

## Struktura

Svaki zadatak je zaseban projekt u rješenju `Vjezba3.sln`, pa se pokreće samostalno, bez izbornika.

| Zadatak | Mapa projekta | Datoteka |
|---|---|---|
| 1 | `Zadatak1_Brojac/` | `Zadatak1_Brojac/Zadatak1_Brojac.cs` |
| 2 | `Zadatak2_Racun/` | `Zadatak2_Racun/Zadatak2_Racun.cs` |
| 3 | `Zadatak3_Stog/` | `Zadatak3_Stog/Zadatak3_Stog.cs` |

## Otvaranje i pokretanje

- **Docker (preporučeno u ovom repozitoriju):** iz korijena repozitorija
  - zadatak: `docker compose run --rm dotnet dotnet run -c Release --project exercises/03-data-race/Zadatak1_Brojac`
  - HTML stranice: `docker compose up web`, zatim http://localhost:8080/03-data-race/Vjezba3_upute.html i http://localhost:8080/03-data-race/Vizualizacije/index.html
- **Visual Studio / Rider:** otvorite `Vjezba3.sln`, odaberite projekt zadatka i pokrenite ga.
- **Visual Studio Code:** File > Open Folder i odaberite ovu mapu, konfiguracija »Vježba 3: zadatak N«. Za mjerenje vremena odaberite **Release**.
- **Naredbeni redak:** u mapi zadatka `dotnet run -c Release`.

Projekti ciljaju .NET 10 (vidi `Directory.Build.props`).
