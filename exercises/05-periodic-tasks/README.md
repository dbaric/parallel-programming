# Vježba 5

Početni projekti za studente. Zadaci su opisani u `Vjezba5_upute.html`.

## Struktura

Svaki zadatak je zaseban projekt u rješenju `Vjezba5.sln`, pa se pokreće samostalno, bez izbornika. Cijeli zadatak, zajedno s početnim naredbama, nalazi se u jednoj datoteci.

| Zadatak | Mapa projekta | Datoteka |
|---|---|---|
| 1 | `Zadatak1_ImeSvakeSekunde/` | `Zadatak1_ImeSvakeSekunde/Zadatak1_ImeSvakeSekunde.cs` |
| 2 | `Zadatak2_Periodicno/` | `Zadatak2_Periodicno/Zadatak2_Periodicno.cs` |
| 3 | `Zadatak3_PeriodicnoPrekid/` | `Zadatak3_PeriodicnoPrekid/Zadatak3_PeriodicnoPrekid.cs` |
| 4 | `Zadatak4_Periodicno2/` | `Zadatak4_Periodicno2/Zadatak4_Periodicno2.cs` |
| 5 (neobavezno) | `Zadatak5_OgradaZastoj/` | `Zadatak5_OgradaZastoj/Zadatak5_OgradaZastoj.cs` |

## Otvaranje i pokretanje

- **Docker (preporučeno u ovom repozitoriju):** iz korijena repozitorija
  - zadatak: `docker compose run --rm dotnet dotnet run -c Release --project exercises/05-periodic-tasks/Zadatak2_Periodicno`
  - HTML stranica: `docker compose up web`, zatim http://localhost:8080/05-periodic-tasks/Vjezba5_upute.html
- **Visual Studio:** otvorite `Vjezba5.sln`. Desni klik na projekt zadatka > *Set as Startup Project*, zatim F5 (ili Ctrl+F5).
- **JetBrains Rider:** File > Open i odaberite `Vjezba5.sln`. U popisu konfiguracija pokretanja (gore desno) odaberite projekt zadatka i pokrenite ga.
- **Visual Studio Code:** File > Open Folder i odaberite ovu mapu. Instalirajte predloženo proširenje C# Dev Kit (ili C#). Pokretanje: F5, konfiguracija »Vježba 5: zadatak N (Debug)«. Za mjerenje vremena odaberite konfiguraciju **Release**.
- **Naredbeni redak:** svaki zadatak pokreće se iz svoje mape, samo s `dotnet run`, npr. `cd Zadatak1_ImeSvakeSekunde` pa `dotnet run`. Za mjerenja: `dotnet run -c Release`. Iz ove mape radi i `dotnet run --project Zadatak1_ImeSvakeSekunde`.

Zadatak 1 čeka da pritisnete Enter, pa ga pokrenite u terminalu (u VS Codeu konfiguracije koriste integrirani terminal).

Projekti ciljaju .NET 10. Ako imate samo Visual Studio 2022, u datoteci `.csproj` svakog zadatka (`<TargetFramework>`) promijenite `net10.0` u `net8.0`, a u `.vscode/launch.json` isto u putanjama programa.
