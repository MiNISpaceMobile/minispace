using Infrastructure.PingResponders;
using Infrastructure.UnitOfWorks;

namespace UnitTests.Infrastucture.PingResponders;

[TestClass]
public class PongPingResponderTests
{
    [TestMethod]
    public void Response_Always_Pong()
    {
        // Arrange
        var pingResponder = new PongPingResponder(new DictionaryUnitOfWork([]));

        // Act
        var response = pingResponder.Response();

        // Assert
        Assert.AreEqual(response, "Pong");
    }
}
