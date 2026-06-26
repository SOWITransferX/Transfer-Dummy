// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

namespace TransferX.Transfer.Dummy.Helpers;

/// <summary>
/// Hilfklasse für Pfad-Mapping zwischen Quell- und Ziel-Provider (optional).
/// </summary>
public static class PathMapper
{
    /// <summary>
    /// Berechnet den Zielpfad für eine Quelldatei unter Beibehaltung der relativen Ordnerstruktur.
    /// </summary>
    /// <param name="sourceRoot">Quellbasis-Pfad aus der Transfer-Konfiguration.</param>
    /// <param name="targetRoot">Zielbasis-Pfad aus der Transfer-Konfiguration.</param>
    /// <param name="sourceFilePath">Vollständiger Quellpfad der Datei.</param>
    /// <returns>Zielpfad für die Datei beim Ziel-Provider.</returns>
    /// <exception cref="NotImplementedException">Noch nicht implementiert (Stub).</exception>
    public static string MapToTarget(string sourceRoot, string targetRoot, string sourceFilePath)
    {
        throw new NotImplementedException("Pfad-Mapping ist im Dummy-Plugin noch nicht implementiert.");
    }
}
