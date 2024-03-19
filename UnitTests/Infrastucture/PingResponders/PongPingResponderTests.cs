using Infrastructure.PingResponders;

namespace UnitTests.Infrastucture.PingResponders;

[TestClass]
public class PongPingResponderTests
{
    [TestMethod]
    public void Response_Always_Pong()
    {
        // Arrange
        var pingResponder = new PongPingResponder();

        // Act
        var response = pingResponder.Response();

        // Assert
        Assert.AreEqual(response, "Pong");
    }
}
