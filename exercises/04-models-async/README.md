# Vježba 4

Modeli računala (RAM, PRAM, aPRAM) i async/await. Početni projekti za studente. Zadaci su opisani u `Vjezba4_upute.html`, a gradivo je u prezentaciji i mapi `Vizualizacije/` (`index.html`).

## Struktura

Svaki zadatak je zaseban projekt u rješenju `Vjezba4.sln`, pa se pokreće samostalno, bez izbornika.

| Zadatak | Mapa projekta | Datoteka |
|---|---|---|
| 1 | `Zadatak1_GlavaListe/` | `Zadatak1_GlavaListe/Zadatak1_GlavaListe.cs` |
| 2 | `Zadatak2_Async/` | `Zadatak2_Async/Zadatak2_Async.cs` |
| 3 | `Zadatak3_Blokiranje/` | `Zadatak3_Blokiranje/Zadatak3_Blokiranje.cs` |

## Otvaranje i pokretanje

- **Docker (preporučeno u ovom repozitoriju):** iz korijena repozitorija
  - zadatak: `docker compose run --rm dotnet dotnet run -c Release --project exercises/04-models-async/Zadatak1_GlavaListe`
  - HTML stranice: `docker compose up web`, zatim http://localhost:8080/04-models-async/Vjezba4_upute.html i http://localhost:8080/04-models-async/Vizualizacije/index.html
- **Visual Studio / Rider:** otvorite `Vjezba4.sln`, odaberite projekt zadatka i pokrenite ga.
- **Visual Studio Code:** File > Open Folder i odaberite ovu mapu, konfiguracija »Vježba 4: zadatak N«. Za mjerenje vremena odaberite **Release**.
- **Naredbeni redak:** u mapi zadatka `dotnet run -c Release`.

Projekti ciljaju .NET 10 (vidi `Directory.Build.props`).
