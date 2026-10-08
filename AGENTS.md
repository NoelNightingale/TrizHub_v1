# TRiZHub — Agent Orientation

Timesheet, project and performance-management system. ASP.NET MVC/WebAPI on **.NET Framework 4.8**
(old-style csproj + `packages.config`), Entity Framework 6 code-first against SQL Server, and an
**AngularJS 1.x** single-page app served from one Razor view.

## Solution layout

| Path | What it is |
| --- | --- |
| `UI/TRiZHub` | Web app: WebAPI controllers, models, Razor shell, and the AngularJS SPA under `Portals/app` |
| `BL/TRiZHub.BL` | Domain layer: EF entities, `DataContext`, providers, migrations, scheduled jobs |
| `Lib/TCR.Lib`, `Lib/TCR.Lib.BL` | Shared infrastructure: `DbEntity`, `ProviderBase`, email, SQL, logging, validators |
| `Tools/*` | Console utilities: `ClientImport`, `DbPopulate`, `UnitTestRun` |
| `BL/TRiZHub.BL.Test`, `UI/TRiZHub.Tests`, `UI/TRiZHub/AuctionRoom.Tests` | MSTest projects |
| `UI/TRIZHub_UI` | **Untracked React/Vite spike. Not the active codebase — do not modify unless asked.** |

## Environment facts

- `msbuild`, `nuget` and `gh` are **not on PATH**. Visual Studio 2022 Community is installed:
  - `C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe`
  - `C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe`
- `dotnet` exists but **cannot build these projects** — they are old-style, non-SDK csproj.
- `packages/` is gitignored and restored locally. A fresh clone needs a NuGet restore via Visual Studio.
- `UI/TRiZHub/Web.config` has an **active connection string to `TriZHub_Prod`** using integrated
  security on the local default instance. Never run code that touches EF without confirming the
  target database first.

## Working agreement

- The user builds and runs; do not assume a change compiles. State clearly what was not verified.
- Current work is on branch `epic/Tap4PhaseA`. Do not commit unless explicitly asked.
- Git history is squashed to 3 commits, so past commits are not a guide to conventions — these files are.

## Adding a feature: the vertical slice

A maintenance feature touches these layers in order. Steps 2, 6 and 8 are the ones people forget.

1. **Entity** — `BL/TRiZHub.BL/Entities/<Area>Data/`, then add a `DbSet` to `Context/DataContext.cs`.
2. **Migration** — explicit EF6 migration in `BL/TRiZHub.BL/Migrations`. Automatic migrations are off.
3. **Provider** — `BL/TRiZHub.BL/Provider/<Area>Data/` as an `I<X>Provider`/`<X>Provider` pair plus an `<X>Exception`.
4. **API models** — `UI/TRiZHub/Models/<X>Models/`, conventionally `<X>DropdownModel`, `<X>EditModel`, `<X>GridModel`.
5. **Controller** — `UI/TRiZHub/Controllers/<X>Controller.cs` deriving from `TCRControllerBase`.
6. **Angular service** — `Portals/app/Services/<X>Service/~<X>Service.ts` + `~Models.ts`, then register the
   folder in `App_Start/BundleConfig.cs` and the `.ts` files in `TRiZHub.csproj`.
7. **Angular controllers** — `Portals/app/states/mainState/.../{grid,detail}/controllers/~<X>Controller.ts`.
8. **State registration** — add the states to the single `.config` block in `Portals/app/AngularApp.js`.

Detailed conventions live in `.cursor/rules/` and load automatically when you open matching files.
`ActivityController.cs` with `ActivityProvider.cs` is the cleanest reference slice.
