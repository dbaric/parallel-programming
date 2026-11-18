# Vježba 6

Početni projekti za studente. Zadaci su opisani u `Vjezba6_upute.html` (zadatak 5 je neobavezan; zadatak »na papiru« je za vježbu kod kuće, bez projekta).

## Struktura

Svaki zadatak je zaseban projekt u rješenju `Vjezba6.sln`, pa se pokreće samostalno, bez izbornika. Cijeli zadatak, zajedno s početnim naredbama, nalazi se u jednoj datoteci.

| Zadatak | Mapa projekta | Datoteka |
|---|---|---|
| 0 | `Zadatak0_Indeksi/` | `Zadatak0_Indeksi/Zadatak0_Indeksi.cs` |
| 1 | `Zadatak1_Reduciranje/` | `Zadatak1_Reduciranje/Zadatak1_Reduciranje.cs` |
| 2 | `Zadatak2_Prescan/` | `Zadatak2_Prescan/Zadatak2_Prescan.cs` |
| 3 | `Zadatak3_Granularnost/` | `Zadatak3_Granularnost/Zadatak3_Granularnost.cs` |
| 4 | `Zadatak4_Nzd/` | `Zadatak4_Nzd/Zadatak4_Nzd.cs` |
| 5 | `Zadatak5_Binarni/` | `Zadatak5_Binarni/Zadatak5_Binarni.cs` |

## Otvaranje i pokretanje

- **Visual Studio:** otvorite `Vjezba6.sln`. Desni klik na projekt zadatka > *Set as Startup Project*, zatim F5 (ili Ctrl+F5).
- **JetBrains Rider:** File > Open i odaberite `Vjezba6.sln`. U popisu konfiguracija pokretanja (gore desno) odaberite projekt zadatka i pokrenite ga.
- **Visual Studio Code:** File > Open Folder i odaberite ovu mapu. Instalirajte predloženo proširenje C# Dev Kit (ili C#). Pokretanje: F5, konfiguracija »Vježba 6: zadatak N (Debug)«. Za mjerenje vremena odaberite konfiguraciju **Release**.
- **Naredbeni redak:** svaki zadatak pokreće se iz svoje mape, samo s `dotnet run`, npr. `cd Zadatak1_Reduciranje` pa `dotnet run`. Za mjerenja: `dotnet run -c Release`. Iz ove mape radi i `dotnet run --project Zadatak1_Reduciranje`.

Projekti ciljaju .NET 10. Ako imate samo Visual Studio 2022, u datoteci `.csproj` svakog zadatka (`<TargetFramework>`) promijenite `net10.0` u `net8.0`, a u `.vscode/launch.json` isto u putanjama programa.
