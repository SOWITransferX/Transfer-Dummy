// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
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
/// ListFiles → Download → Upload mit <c>ProgressAggregator</c> aus TransferX.Core.
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
    public Task<TransferResult> ExecuteAsync(
        TransferConfigItem config,
        IProvider sourceProvider,
        IProvider targetProvider,
        IProgress<ProgressReport>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var fileResults = new List<FileTransferResult>();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Bewusster Stub: keine List/Download/Upload-Logik.
            // Später z. B.:
            // var aggregator = new ProgressAggregator(config.TransferId, progress);
            // var listResponse = (ListFilesResponse)await sourceProvider.ExecuteAsync(
            //     new ListFilesRequest { Path = config.SourcePath, Recursive = true },
            //     progress: null,
            //     cancellationToken);

            return Task.FromResult(new TransferResult
            {
                Success = true,
                TotalFiles = 0,
                SuccessfulFiles = 0,
                FailedFiles = 0,
                TotalBytesTransferred = 0,
                Duration = DateTime.UtcNow - startTime,
                FileResults = fileResults.AsReadOnly()
            });
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Task.FromResult(new TransferResult
            {
                Success = false,
                TotalFiles = 0,
                SuccessfulFiles = 0,
                FailedFiles = 0,
                TotalBytesTransferred = 0,
                Duration = DateTime.UtcNow - startTime,
                FileResults = fileResults.AsReadOnly(),
                ErrorMessage = ex.Message
            });
        }
    }
}
