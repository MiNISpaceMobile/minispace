using Domain.Abstractions;
using SixLabors.ImageSharp;

namespace Infrastructure.PictureHandlers
{
    public class WebpPictureHandler : IPictureHandler
    {
        public string Format => "WebP";
        public string Extension => ".webp";

        public Stream ConvertIfNeeded(Stream source)
        {
            Stream result;
            try
            {
                var format = Image.DetectFormat(source);
                if (string.Equals(format.Name, Format, StringComparison.InvariantCultureIgnoreCase))
                    result = source;
                else // need to convert
                {
                    result = new MemoryStream((int)source.Length);
                    Image.Load(source).SaveAsWebp(result);
                    source.Dispose();
                }
            }
            catch
            {
                throw new Exception("Invalid source format");
            }
            return result;
        }
    }
}
