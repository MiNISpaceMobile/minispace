namespace Domain.Abstractions
{
    public interface IPictureHandler
    {
        public string Format { get; }
        public string Extension { get; }

        Stream ConvertIfNeeded(Stream picture);
    }
}
