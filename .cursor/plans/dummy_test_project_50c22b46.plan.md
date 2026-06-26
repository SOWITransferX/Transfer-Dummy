---
name: Dummy Test Project
overview: Neues MSTest-Projekt `TransferX.Transfer.Dummy.Test` unter `tests/` anlegen, an das Plugin `TransferX.Transfer.Dummy` anbinden, mit ersten Unit-Tests für Metadaten und Stub-`ExecuteAsync`, und in die bestehende Solution einbinden.
todos:
  - id: create-test-csproj
    content: tests/TransferX.Transfer.Dummy.Test/TransferX.Transfer.Dummy.Test.csproj mit MSTest, Moq, ProjectReference
    status: completed
  - id: create-test-class
    content: "MyCommandCommandTests.cs: Metadaten-, Attribut- und Stub-ExecuteAsync-Tests"
    status: completed
  - id: update-solution
    content: "TransferX.Transfer.Dummy.sln: Testprojekt hinzufügen, Plugin-GUIDs korrigieren"
    status: completed
  - id: run-dotnet-test
    content: dotnet test Release ausführen und Erfolg verifizieren
    status: completed
isProject: false
---

# Testprojekt TransferX.Transfer.Dummy.Test

## Ziel

Unter [`Source/Plugins/Transfers/Dummy/tests/`](C:/Data/Repositories/TransferX/Source/Plugins/Transfers/Dummy/tests/) ein neues Testprojekt **`TransferX.Transfer.Dummy.Test`** erstellen – analog zum Copy-Plugin, aber mit dem von Ihnen gewünschten Projektnamen (Singular `.Test`, nicht `.Tests`).

## Referenz im Repo

Vorbild: [`TransferX.Transfer.Copy.Tests`](C:/Data/Repositories/TransferX/Source/Plugins/Transfers/Copy/tests/TransferX.Transfer.Copy.Tests/)

| Aspekt | Copy | Dummy (neu) |
|--------|------|-------------|
| Pfad | `Copy/tests/TransferX.Transfer.Copy.Tests/` | `Dummy/tests/TransferX.Transfer.Dummy.Test/` |
| Framework | net8.0, MSTest, Moq | gleich |
| Plugin-Referenz | `ProjectReference` auf Plugin | `ProjectReference` auf [`TransferX.Transfer.Dummy.csproj`](C:/Data/Repositories/TransferX/Source/Plugins/Transfers/Dummy/TransferX.Transfer.Dummy/TransferX.Transfer.Dummy.csproj) |

```mermaid
flowchart LR
    TestProj["TransferX.Transfer.Dummy.Test"]
    Plugin["TransferX.Transfer.Dummy"]
    NuGet["NuGet: Abstractions + Core"]
    TestProj -->|ProjectReference| Plugin
    Plugin --> NuGet
```

## Geplante Dateistruktur

```tex
Dummy/
├── tests/
│   └── TransferX.Transfer.Dummy.Test/
│       ├── TransferX.Transfer.Dummy.Test.csproj
│       └── MyCommandCommandTests.cs
├── TransferX.Transfer.Dummy/
│   └── ...
└── TransferX.Transfer.Dummy.sln   ← Testprojekt ergänzen + GUIDs korrigieren
```

## 1. `.csproj` anlegen

[`tests/TransferX.Transfer.Dummy.Test/TransferX.Transfer.Dummy.Test.csproj`](C:/Data/Repositories/TransferX/Source/Plugins/Transfers/Dummy/tests/TransferX.Transfer.Dummy.Test/TransferX.Transfer.Dummy.Test.csproj):

- `TargetFramework`: net8.0
- `IsPackable`: false
- Pakete (wie Copy): `Microsoft.NET.Test.Sdk` 17.10.0, `MSTest.TestAdapter` / `MSTest.TestFramework` 3.4.3, `Moq` 4.20.70
- `ProjectReference`: `..\..\TransferX.Transfer.Dummy\TransferX.Transfer.Dummy.csproj`
- `RootNamespace` / `AssemblyName`: `TransferX.Transfer.Dummy.Test`

Keine direkte Referenz auf `TransferX.Core` im Testprojekt nötig – kommt transitiv über das Plugin; für Stub-Tests reichen `Moq` + Abstractions-Typen.

## 2. Erste Testklasse

[`MyCommandCommandTests.cs`](C:/Data/Repositories/TransferX/Source/Plugins/Transfers/Dummy/tests/TransferX.Transfer.Dummy.Test/MyCommandCommandTests.cs) mit Datei-Header (SOWI / Franz Schönbächler) und deutschen XML-Kommentaren.

Sinnvolle Tests für den aktuellen **Stub** in [`MyCommandCommand.cs`](C:/Data/Repositories/TransferX/Source/Plugins/Transfers/Dummy/TransferX.Transfer.Dummy/MyCommandCommand.cs):

| Test | Prüfung |
|------|---------|
| `CommandName_ShouldBe_MyCommand` | `CommandName == "MyCommand"` |
| `Version_ShouldBe_1_0_0` | `Version == "1.0.0"` |
| `Description_ShouldNotBeEmpty` | Description gesetzt |
| `ExecuteAsync_EmptyStub_ReturnsSuccessWithZeroFiles` | `Success`, `TotalFiles == 0`, leere `FileResults` |
| `ExecuteAsync_Cancelled_ThrowsOperationCanceledException` | `CancellationToken` bereits abgebrochen |
| `Type_HasTransferMetadataAttribute` | `[TransferMetadata]` auf `MyCommandCommand` |

Optional (ohne Core-Referenz im Testprojekt): kein `TransferLoader`-Integrationstest – das bleibt in [`TransferLoaderTests`](C:/Data/Repositories/TransferX/Source/TransferX/tests/TransferX.Core.Tests/Loaders/TransferLoaderTests.cs).

`ExecuteAsync`-Tests mit `Mock<IProvider>` (Setup nicht nötig, da Stub keine Provider aufruft).

## 3. Solution aktualisieren

[`TransferX.Transfer.Dummy.sln`](C:/Data/Repositories/TransferX/Source/Plugins/Transfers/Dummy/TransferX.Transfer.Dummy.sln) enthält aktuell **inkonsistente GUIDs** (Projekt-GUID `{31a254f1-...}` vs. Konfiguration `{7dd4cfad-...}`). Beim Hinzufügen des Testprojekts:

- Plugin-Projekt-Konfiguration auf die echte GUID `{31a254f1-88f3-4e13-b573-30a3aa63a8f2}` korrigieren
- Testprojekt mit neuer GUID eintragen (`Debug`/`Release`, Build.0)

Alternativ: `dotnet sln add` verwenden (erzeugt korrekte Einträge automatisch).

## 4. Verifikation

```powershell
dotnet test "C:\Data\Repositories\TransferX\Source\Plugins\Transfers\Dummy\tests\TransferX.Transfer.Dummy.Test\TransferX.Transfer.Dummy.Test.csproj" -c Release
```

Voraussetzung: lokaler NuGet-Feed [`Packages/`](C:/Data/Repositories/TransferX/Packages) mit `TransferX.Transfer.Abstractions.2.0.0` und `TransferX.Core.2.0.0` (bereits aus Stub-Implementierung vorhanden).

## Bewusst ausserhalb des Scopes

- Erweiterte Copy-/Provider-Mock-Tests (erst relevant nach echter Transfer-Logik)
- Anpassung von `New-TransferXPluginDummy.ps1` (kann später ergänzt werden)
- Änderungen an zentraler TransferX-Solution
