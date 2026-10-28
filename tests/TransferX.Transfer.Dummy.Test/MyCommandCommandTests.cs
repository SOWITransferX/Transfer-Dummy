// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TransferX.Provider.Abstractions;
using TransferX.Transfer.Abstractions;
using TransferX.Transfer.Abstractions.Metadata;
using TransferX.Transfer.Abstractions.Progress;
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
    private Mock<ITransferProgress> _progressMock = null!;
    private MyCommandCommand _command = null!;

    /// <summary>
    /// Initialisiert Mocks und Command vor jedem Test.
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        _sourceProviderMock = new Mock<IProvider>();
        _targetProviderMock = new Mock<IProvider>();
        _progressMock = new Mock<ITransferProgress>();
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
        var result = await _command.ExecuteAsync(CreateContext());

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
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
            _command.ExecuteAsync(CreateContext(), cts.Token));
    }

    /// <summary>
    /// Prüft, dass das Plugin <c>TransferX.Core</c> weder als Assembly noch als Paket referenziert.<br/>
    /// Seit Abstractions 3.0.0-preview.1 ist nur <c>TransferX.Transfer.Abstractions</c> zulässig.
    /// </summary>
    [TestMethod]
    public void References_DoNotInclude_TransferXCore()
    {
        var referencedNames = typeof(MyCommandCommand).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToList();

        CollectionAssert.Contains(referencedNames, "TransferX.Transfer.Abstractions");
        CollectionAssert.DoesNotContain(referencedNames, "TransferX.Core");

        var projectFile = FindPluginProjectFile();
        var projectText = File.ReadAllText(projectFile);

        StringAssert.Contains(projectText, "Include=\"TransferX.Transfer.Abstractions\"");
        StringAssert.Contains(projectText, "3.0.0-preview.1");
        Assert.IsFalse(
            projectText.Contains("TransferX.Core", StringComparison.Ordinal),
            "Die Projektdatei darf TransferX.Core nicht enthalten.");
    }

    private TransferContext CreateContext() =>
        new()
        {
            TransferId = Guid.NewGuid(),
            SourcePath = "/source",
            TargetPath = "/target",
            Source = _sourceProviderMock.Object,
            Target = _targetProviderMock.Object,
            Progress = _progressMock.Object
        };

    /// <summary>
    /// Sucht die Plugin-Projektdatei ausgehend vom Testausgabeverzeichnis.
    /// </summary>
    private static string FindPluginProjectFile()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(
                directory.FullName,
                "src",
                "TransferX.Transfer.Dummy",
                "TransferX.Transfer.Dummy.csproj");

            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        Assert.Fail("TransferX.Transfer.Dummy.csproj wurde nicht gefunden.");
        return string.Empty;
    }
}
