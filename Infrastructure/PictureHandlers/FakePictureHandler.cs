using Domain.Abstractions;

namespace Infrastructure.PictureHandlers
{
    public class FakePictureHandler : IPictureHandler
    {
        public string Format => "Test";
        public string Extension => "";

        public bool ConvertCalled { get; private set; } = false;

        public Stream ConvertIfNeeded(Stream picture)
        {
            ConvertCalled = true;
            return picture;
        }
    }
}
