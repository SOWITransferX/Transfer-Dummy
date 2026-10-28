# TransferX.Transfer.Dummy

`TransferX.Transfer.Dummy` ist ein Referenz- und Test-Transfer-Plugin für das Framework TransferX.
Das Projekt dient als Vorlage für Entwickler, die eigene Transfer-Plugins für TransferX erstellen möchten.

Der Dummy-Transfer implementiert das Interface `ITransferCommand` mit dem Command-Namen **MyCommand**, führt jedoch **keine echte Dateiübertragung** durch und liefert ein leeres Erfolgsergebnis zurück.

Entwickler können dieses Repository:

* direkt als Ausgangsbasis verwenden
* forken und erweitern
* herunterladen und daraus einen eigenen Transfer-Command entwickeln

Das Ziel ist es, die Entwicklung neuer TransferX-Transfer-Plugins zu vereinfachen und eine funktionierende Referenzimplementierung bereitzustellen.

## Was ist TransferX?

TransferX ist ein modulares Framework für Datei- und Datentransfers zwischen unterschiedlichen Systemen und Datenquellen.

TransferX ermöglicht:

* Daten zwischen Providern zu kopieren oder zu synchronisieren
* unterschiedliche Systeme miteinander zu verbinden
* eigene Provider-Plugins zu entwickeln
* Transfer-Strategien wie Copy, Move oder Sync flexibel zu erweitern

Weitere Informationen zur Architektur und zum Gesamtsystem befinden sich in der Hauptdokumentation des Projekts.

## Unterstützte Operationen

Der Dummy-Command `MyCommand` stellt ein **Scaffold-Plugin** bereit:

- Registrierung als Transfer-Command über `[TransferMetadata]`
- Ausführung von `ExecuteAsync(TransferContext, CancellationToken)`
- Rückgabe eines leeren `TransferResult` mit `Success = true` (0 Dateien, 0 Bytes)
- Abbruch über `CancellationToken` (`OperationCanceledException`)

**Nicht implementiert** (bewusste Stubs für spätere Erweiterung):

- Dateien auflisten, herunterladen und hochladen (`ListFiles` → `DownloadFile` → `UploadFile`)
- Pfad-Mapping zwischen Quelle und Ziel (`PathMapper`)
- Änderungserkennung für Sync-Transfers (`ChangeDetector`)

Für eine vollständige Copy-Implementierung siehe `TransferXImplementTransferPlugin.md` Abschnitt 7 (MyCopyCommand):
`ListFiles` → `Download` → `Upload` mit Fortschritt über `context.Progress`.

## Abhängigkeiten

Einzige direkte Paketabhängigkeit:

| Paket | Version | Quelle |
| --- | --- | --- |
| `TransferX.Transfer.Abstractions` | `3.0.0-preview.1` | nuget.org |

Transitiv folgen `TransferX.Provider.Abstractions` und `TransferX.Domain` in derselben Version.
Das Plugin referenziert `TransferX.Core` nicht. Den Fortschritt liefert der Host über `context.Progress` (`ITransferProgress`).

## Ausführungskontext

`ExecuteAsync` erhält einen `TransferContext`. Der Host löst die Provider auf und setzt den Fortschritt.

| Eigenschaft  | Beschreibung                                           |
| ------------ | ------------------------------------------------------ |
| `TransferId` | Eindeutige ID des Transfers                            |
| `SourcePath` | Pfad am Quell-Provider (z. B. `/source`)               |
| `TargetPath` | Pfad am Ziel-Provider (z. B. `/target`)                |
| `Source`     | Bereits initialisierter Quell-Provider                 |
| `Target`     | Bereits initialisierter Ziel-Provider                  |
| `Progress`   | Fortschritt des Hosts (`ITransferProgress`, nie null)  |

Das Plugin orchestriert die Übertragung zwischen `Source` und `Target`. `CommandName` bleibt `MyCommand`.

## Projektstruktur

- `src/TransferX.Transfer.Dummy` – Transfer-Command-Implementierung (`MyCommandCommand`)
- `src/TransferX.Transfer.Dummy/Helpers` – optionale Hilfsklassen (`PathMapper`, `ChangeDetector`)
- `tests/TransferX.Transfer.Dummy.Test` – MSTest-Tests

## Build & Test

```bash
dotnet build TransferX.Transfer.Dummy.sln
dotnet test TransferX.Transfer.Dummy.sln
```

## Verwendung als Vorlage für eigene Transfer-Plugins

Der Dummy-Transfer zeigt die grundlegende Struktur eines TransferX-Transfer-Commands und kann als Ausgangspunkt für eigene Implementierungen verwendet werden.

Typische Vorgehensweise:

1. Repository forken oder herunterladen
2. Projekt, Namespace und `CommandName` umbenennen
3. Eigene Transfer-Logik in `ExecuteAsync` implementieren
4. Optional: `PathMapper` und `ChangeDetector` ausarbeiten
5. Transfer-Command in TransferX registrieren
6. Tests erweitern und anpassen

Mögliche Transfer-Strategien für eigene Plugins:

* Copy (Dateien kopieren)
* Move (Dateien verschieben)
* Sync (bidirektionale oder unidirektionale Synchronisation)
* Mirror (Spiegelung mit Löschung überzähliger Dateien)
* Filter-basierte Transfers (z. B. nur bestimmte Dateitypen)
* Inkrementelle Transfers (nur geänderte Dateien)

## Hinweise

* Der Dummy-Command führt **keine** Provider-Aufrufe aus; Quell- und Ziel-Provider werden in Tests per Mock bereitgestellt
* `ExecuteAsync` mit abgebrochenem `CancellationToken` wirft `OperationCanceledException`
* Unerwartete Exceptions werden in ein `TransferResult` mit `Success = false` und `ErrorMessage` umgewandelt
* `PathMapper.MapToTarget` wirft derzeit `NotImplementedException`
* Das Plugin ist primär für Entwicklung, Referenzimplementierungen und automatisierte Tests gedacht
