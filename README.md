# PP

Paralelno programiranje (Parallel Programming)

Prirodoslovno-matematički fakultet, Sveučilište u Splitu

- `exercises/` vježbe
- `theory/` teorija
- `exams/` ispiti

## Setup (Docker + .NET 10 SDK)

All exercises use .NET 10 and run in Docker, so the only prerequisite is [Docker](https://www.docker.com/products/docker-desktop/) (with Compose).

- `Dockerfile` – image based on `mcr.microsoft.com/dotnet/sdk:10.0`
- `docker-compose.yml` – two services:
  - `dotnet` – SDK shell; the repo root is mounted at `/workspace`, NuGet cache is kept in a named volume
  - `web` – nginx serving `exercises/` as static HTML on port 8080

### Run the .NET SDK

From the repo root:

```sh
docker compose run --rm dotnet                  # interactive shell in /workspace
docker compose run --rm dotnet dotnet --version # one-off command
```

Run a single task:

```sh
docker compose run --rm dotnet dotnet run --project exercises/01-delegates/Zadatak1_Delegati
```

Build a whole exercise solution:

```sh
docker compose run --rm dotnet dotnet build exercises/01-delegates/Vjezba1.sln
```

### View the HTML pages

Instructions and visualizations are static HTML:

```sh
docker compose up web      # add -d to run in the background
docker compose down        # stop
```

- http://localhost:8080/01-delegates/Vjezba1_upute.html
- http://localhost:8080/01-delegates/Vizualizacije/delegati.html

### Notes

- Each exercise is a folder `exercises/NN-slug/`; its own `README.md` (if any) describes the tasks.
- Console programs print to the terminal, they have no web output.
- Some links in `01-delegates/Vizualizacije/` point to pages of later weeks that are not in the repo yet.
