// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Transfer.Abstractions;
using TransferX.Transfer.Abstractions.Metadata;
using TransferX.Transfer.Abstractions.Models;

namespace TransferX.Transfer.Dummy;

/// <summary>
/// Dummy-Transfer-Command für TransferX mit dem Command-Namen <c>MyCommand</c>.<br/>
/// Dient als Scaffold-Plugin ohne echte Dateiübertragung.
/// </summary>
/// <remarks>
/// Für eine vollständige Copy-Implementierung siehe
/// <c>TransferXImplementTransferPlugin.md</c> Abschnitt 7 (MyCopyCommand):<br/>
/// ListFiles → Download → Upload mit Fortschritt über <c>context.Progress</c>.<br/>
/// Ab Abstractions 3.0.0 übergibt der Host Pfade, Provider und Fortschritt<br/>
/// im <see cref="TransferContext"/>; ein Core-Paket braucht das Plugin dafür nicht.
/// </remarks>
[TransferMetadata]
public sealed class MyCommandCommand : ITransferCommand
{
    /// <inheritdoc/>
    public string CommandName => "MyCommand";

    /// <inheritdoc/>
    public string Description =>
        "Dummy-Transfer-Plugin ohne Dateiübertragung; liefert ein leeres Erfolgsergebnis.";

    /// <inheritdoc/>
    public string Version => "1.0.0";

    /// <inheritdoc/>
    /// <remarks>
    /// Signatur ab Abstractions 3.0.0.<br/>
    /// <paramref name="context"/> bündelt Pfade, Provider und <c>context.Progress</c>.<br/>
    /// <see cref="TransferResult.Success"/> und die Zähler berechnet das Modell;<br/>
    /// <c>Duration</c> gibt es am Ergebnis nicht mehr.
    /// </remarks>
    public Task<TransferResult> ExecuteAsync(
        TransferContext context,
        CancellationToken cancellationToken = default)
    {
        var fileResults = new List<FileTransferResult>();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Bewusster Stub: keine List/Download/Upload-Logik.
            // Später z. B. Fortschritt über context.Progress (vom Host gesetzt, nie null):
            // var listResponse = (ListFilesResponse)await context.Source.ExecuteAsync(
            //     new ListFilesRequest { Path = context.SourcePath, Recursive = true },
            //     progress: null,
            //     cancellationToken);
            // context.Progress.Initialize(listResponse.Items.Count, listResponse.TotalSizeBytes);

            // context wird im Stub nicht ausgewertet; der Host liefert ihn für die spätere Logik.
            _ = context;

            return Task.FromResult(new TransferResult
            {
                TotalFiles = 0,
                FileResults = fileResults.AsReadOnly()
            });
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Success wird false, sobald ErrorMessage gesetzt ist.
            return Task.FromResult(new TransferResult
            {
                TotalFiles = 0,
                FileResults = fileResults.AsReadOnly(),
                ErrorMessage = ex.Message
            });
        }
    }
}
