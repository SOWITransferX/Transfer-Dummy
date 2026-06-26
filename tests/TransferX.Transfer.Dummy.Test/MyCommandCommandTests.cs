// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Transfer.Abstractions.Metadata;
using TransferX.Transfer.Abstractions.Models;
using TransferX.Transfer.Dummy;

namespace TransferX.Transfer.Dummy.Test;

/// <summary>
/// Unit-Tests für <see cref="MyCommandCommand"/>.<br/>
/// Prüft Metadaten, TransferMetadata-Marker und Stub-Verhalten von <see cref="MyCommandCommand.ExecuteAsync"/>.
/// </summary>
[TestClass]
public sealed class MyCommandCommandTests
{
    private Mock<IProvider> _sourceProviderMock = null!;
    private Mock<IProvider> _targetProviderMock = null!;
    private MyCommandCommand _command = null!;

    /// <summary>
    /// Initialisiert Mocks und Command vor jedem Test.
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        _sourceProviderMock = new Mock<IProvider>();
        _targetProviderMock = new Mock<IProvider>();
        _command = new MyCommandCommand();
    }

    /// <summary>
    /// Prüft, dass der Command-Name <c>MyCommand</c> ist.
    /// </summary>
    [TestMethod]
    public void CommandName_ShouldBe_MyCommand()
    {
        Assert.AreEqual("MyCommand", _command.CommandName);
    }

    /// <summary>
    /// Prüft, dass die Version <c>1.0.0</c> ist.
    /// </summary>
    [TestMethod]
    public void Version_ShouldBe_1_0_0()
    {
        Assert.AreEqual("1.0.0", _command.Version);
    }

    /// <summary>
    /// Prüft, dass eine Beschreibung gesetzt ist.
    /// </summary>
    [TestMethod]
    public void Description_ShouldNotBeEmpty()
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(_command.Description));
    }

    /// <summary>
    /// Prüft, dass <see cref="MyCommandCommand"/> das <see cref="TransferMetadataAttribute"/> trägt.
    /// </summary>
    [TestMethod]
    public void Type_HasTransferMetadataAttribute()
    {
        var attribute = Attribute.GetCustomAttribute(
            typeof(MyCommandCommand),
            typeof(TransferMetadataAttribute));

        Assert.IsNotNull(attribute);
        Assert.IsInstanceOfType<TransferMetadataAttribute>(attribute);
    }

    /// <summary>
    /// Prüft, dass der Stub ein leeres Erfolgsergebnis ohne Dateien zurückgibt.
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_EmptyStub_ReturnsSuccessWithZeroFiles()
    {
        var config = CreateConfig();

        var result = await _command.ExecuteAsync(
            config,
            _sourceProviderMock.Object,
            _targetProviderMock.Object);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.TotalFiles);
        Assert.AreEqual(0, result.SuccessfulFiles);
        Assert.AreEqual(0, result.FailedFiles);
        Assert.AreEqual(0, result.TotalBytesTransferred);
        Assert.AreEqual(0, result.FileResults.Count);
        Assert.IsNull(result.ErrorMessage);
    }

    /// <summary>
    /// Prüft, dass ein abgebrochenes Token <see cref="OperationCanceledException"/> auslöst.
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_Cancelled_ThrowsOperationCanceledException()
    {
        var config = CreateConfig();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
            _command.ExecuteAsync(
                config,
                _sourceProviderMock.Object,
                _targetProviderMock.Object,
                progress: null,
                cancellationToken: cts.Token));
    }

    private static TransferConfigItem CreateConfig() =>
        new()
        {
            TransferId = Guid.NewGuid(),
            SourceProviderId = Guid.NewGuid(),
            SourcePath = "/source",
            TargetProviderId = Guid.NewGuid(),
            TargetPath = "/target",
            CommandName = "MyCommand"
        };
}
