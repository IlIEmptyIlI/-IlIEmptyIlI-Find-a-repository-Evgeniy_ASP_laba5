namespace Laba5
{
    public static class FileLoggerextensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath)

        {

            builder.AddProvider(new Provider(filePath));

            return builder;

        }

    }
}
