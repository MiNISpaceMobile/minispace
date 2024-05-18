using Domain.DataModel;
using Domain.Services;
using Domain.Services.Implementations;
using Infrastructure.PictureHandlers;
using Infrastructure.Storages;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Domain.Services;

[TestClass]
public class PictureServiceTests
{
#pragma warning disable CS8618 // Unassigned non-nullables
    private Stream tooBigFile;
    private Stream okFile;

    private Student st0;
    private Student st1;
    private Event ev0;
    private Post po0;

    private DictionaryStorage dictionaryStorage;
    private FakePictureHandler pictureHandler;
    private DictionaryUnitOfWork uow;
    private PictureService sut;
#pragma warning restore CS8618 // Unassigned non-nullables

    [TestInitialize]
    public void PreTest()
    {
        tooBigFile = new MemoryStream([0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        okFile = new MemoryStream([0, 0, 0]);

        var now = DateTime.Now;

        st0 = new Student("st0", "st0", "st0") { IsOrganizer = true };
        st1 = new Student("st0", "st0", "st0");
        ev0 = new Event(st0, "ev0", "ev0", EventCategory.Uncategorized, now, now, now, "ev0", null, null);
        po0 = new Post(st0, ev0, "po0");

        uow = new DictionaryUnitOfWork([st0, st1, ev0, po0]);
        dictionaryStorage = new DictionaryStorage();
        pictureHandler = new FakePictureHandler();

        dictionaryStorage.Storage[$"user-{st1.Guid}"] = new Dictionary<string, Stream>();
        dictionaryStorage.Storage[$"user-{st1.Guid}"]["profile"] = okFile;
        st1.HasProfilePicture = true;
        dictionaryStorage.Storage[$"event-{ev0.Guid}"] = new Dictionary<string, Stream>();
        dictionaryStorage.Storage[$"event-{ev0.Guid}"]["0"] = okFile;
        dictionaryStorage.Storage[$"event-{ev0.Guid}"]["1"] = okFile;
        ev0.PictureCount = 2;
        dictionaryStorage.Storage[$"post-{po0.Guid}"] = new Dictionary<string, Stream>();
        dictionaryStorage.Storage[$"post-{po0.Guid}"]["0"] = okFile;
        dictionaryStorage.Storage[$"post-{po0.Guid}"]["1"] = okFile;
        po0.PictureCount = 2;

        sut = new PictureService(uow, dictionaryStorage, pictureHandler) { MaxFileSize = 5 };
    }

    #region UploadUserProfilePicture
    [TestMethod]
    public void UploadUserProfilePicture_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.UploadUserProfilePicture(okFile);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UploadUserProfilePicture_FileTooBig_ThrowsFileTooBig()
    {
        var act = () => sut.AsUser(st0.Guid).UploadUserProfilePicture(tooBigFile);

        Assert.ThrowsException<FileTooBigException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UploadUserProfilePicture_Correct_UploadsPicture()
    {
        sut.AsUser(st0.Guid).UploadUserProfilePicture(okFile);

        Assert.AreSame(okFile, dictionaryStorage.Storage[$"user-{st0.Guid}"]["profile"]);
        Assert.IsTrue(st0.HasProfilePicture);
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion UploadUserProfilePicture

    #region DeleteUserProfilePicture
    [TestMethod]
    public void DeleteUserProfilePicture_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.DeleteUserProfilePicture();

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteUserProfilePicture_Correct_DeletesPicture()
    {
        sut.AsUser(st1.Guid).DeleteUserProfilePicture();

        Assert.IsFalse(dictionaryStorage.Storage[$"user-{st1.Guid}"].ContainsKey("profile"));
        Assert.IsFalse(st0.HasProfilePicture);
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion DeleteUserProfilePicture

    #region UploadEventPicture
    [TestMethod]
    public void UploadEventPicture_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(st1.Guid).UploadEventPicture(ev0.Guid, okFile);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UploadEventPicture_FileTooBig_ThrowsFileTooBig()
    {
        var act = () => sut.AsUser(st0.Guid).UploadEventPicture(ev0.Guid, tooBigFile);

        Assert.ThrowsException<FileTooBigException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UploadEventPicture_InvalidGuid_ThrowsInvalidGuid()
    {
        var act = () => sut.AsUser(st0.Guid).UploadEventPicture(Guid.NewGuid(), okFile);

        Assert.ThrowsException<InvalidGuidException<Event>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UploadEventPicture_LimitExceeded_ThrowsFileLimit()
    {
        int startCount = ev0.PictureCount;
        for (int i = startCount; i < sut.MaxPicturesPerEvent; i++)
            sut.AsUser(st0.Guid).UploadEventPicture(ev0.Guid, okFile);

        var act = () => sut.AsUser(st0.Guid).UploadEventPicture(ev0.Guid, okFile);

        Assert.ThrowsException<FileLimitExeption>(act);
        Assert.AreEqual(sut.MaxPicturesPerEvent - startCount, uow.CommitCount);
    }

    [TestMethod]
    public void UploadEventPicture_Correct_UploadsPicture()
    {
        int nextIndex = ev0.PictureCount;

        sut.AsUser(st0.Guid).UploadEventPicture(ev0.Guid, okFile);

        Assert.IsTrue(dictionaryStorage.Storage[$"event-{ev0.Guid}"].ContainsKey($"{nextIndex}"));
        Assert.AreEqual(nextIndex + 1, ev0.PictureCount);
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion UploadEventPicture

    #region DeleteEventPicture
    [TestMethod]
    public void DeleteEventPicture_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(st1.Guid).DeleteEventPicture(ev0.Guid, 0);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteEventPicture_InvalidGuid_ThrowsInvalidGuid()
    {
        var act = () => sut.AsUser(st0.Guid).DeleteEventPicture(Guid.NewGuid(), 0);

        Assert.ThrowsException<InvalidGuidException<Event>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteEventPicture_InvalidIndex_ThrowsFileIndex()
    {
        var act = () => sut.AsUser(st0.Guid).DeleteEventPicture(ev0.Guid, ev0.PictureCount);

        Assert.ThrowsException<FileIndexException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeleteEventPicture_Correct_DeletesPicture()
    {
        int lastIndex = ev0.PictureCount - 1;

        sut.AsUser(st0.Guid).DeleteEventPicture(ev0.Guid, 0);

        Assert.IsFalse(dictionaryStorage.Storage[$"event-{ev0.Guid}"].ContainsKey($"{lastIndex}"));
        for (int i = 0; i < lastIndex; i++)
            Assert.IsTrue(dictionaryStorage.Storage[$"event-{ev0.Guid}"].ContainsKey($"{i}"));
        Assert.AreEqual(lastIndex, ev0.PictureCount);
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion DeleteEventPicture

    #region UploadPostPicture
    [TestMethod]
    public void UploadPostPicture_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(st1.Guid).UploadPostPicture(po0.Guid, okFile);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UploadPostPicture_FileTooBig_ThrowsFileTooBig()
    {
        var act = () => sut.AsUser(st0.Guid).UploadPostPicture(po0.Guid, tooBigFile);

        Assert.ThrowsException<FileTooBigException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UploadPostPicture_InvalidGuid_ThrowsInvalidGuid()
    {
        var act = () => sut.AsUser(st0.Guid).UploadPostPicture(Guid.NewGuid(), okFile);

        Assert.ThrowsException<InvalidGuidException<Post>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void UploadPostPicture_LimitExceeded_ThrowsFileLimit()
    {
        int startCount = po0.PictureCount;
        for (int i = startCount; i < sut.MaxPicturesPerPost; i++)
            sut.AsUser(st0.Guid).UploadPostPicture(po0.Guid, okFile);

        var act = () => sut.AsUser(st0.Guid).UploadPostPicture(po0.Guid, okFile);

        Assert.ThrowsException<FileLimitExeption>(act);
        Assert.AreEqual(sut.MaxPicturesPerPost - startCount, uow.CommitCount);
    }

    [TestMethod]
    public void UploadPostPicture_Correct_UploadsPicture()
    {
        int nextIndex = po0.PictureCount;

        sut.AsUser(st0.Guid).UploadPostPicture(po0.Guid, okFile);

        Assert.IsTrue(dictionaryStorage.Storage[$"post-{po0.Guid}"].ContainsKey($"{nextIndex}"));
        Assert.AreEqual(nextIndex + 1, po0.PictureCount);
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion UploadPostPicture

    #region DeletePostPicture
    [TestMethod]
    public void DeletePostPicture_Unauthorized_ThrowsUserUnauthorized()
    {
        var act = () => sut.AsUser(st1.Guid).DeletePostPicture(po0.Guid, 0);

        Assert.ThrowsException<UserUnauthorizedException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeletePostPicture_InvalidGuid_ThrowsInvalidGuid()
    {
        var act = () => sut.AsUser(st0.Guid).DeletePostPicture(Guid.NewGuid(), 0);

        Assert.ThrowsException<InvalidGuidException<Post>>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeletePostPicture_InvalidIndex_ThrowsFileIndex()
    {
        var act = () => sut.AsUser(st0.Guid).DeletePostPicture(po0.Guid, po0.PictureCount);

        Assert.ThrowsException<FileIndexException>(act);
        Assert.AreEqual(0, uow.CommitCount);
    }

    [TestMethod]
    public void DeletePostPicture_Correct_DeletesPicture()
    {
        int lastIndex = po0.PictureCount - 1;

        sut.AsUser(st0.Guid).DeletePostPicture(po0.Guid, 0);

        Assert.IsFalse(dictionaryStorage.Storage[$"post-{po0.Guid}"].ContainsKey($"{lastIndex}"));
        for (int i = 0; i < lastIndex; i++)
            Assert.IsTrue(dictionaryStorage.Storage[$"post-{po0.Guid}"].ContainsKey($"{i}"));
        Assert.AreEqual(lastIndex, po0.PictureCount);
        Assert.AreEqual(1, uow.CommitCount);
    }
    #endregion DeletePostPicture
}
