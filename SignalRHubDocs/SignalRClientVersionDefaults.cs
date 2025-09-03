namespace SignalRHubDocs;

public static class SignalRClientVersionDefaults
{
    public static void ConfigureForCurrentFramework(SignalRDocumentationOptions options)
    {
#if NET9_0_OR_GREATER
        options.SignalRClientVersion = "8.0.0";
        options.MessagePackClientVersion = "8.0.0";
#elif NET8_0_OR_GREATER
        options.SignalRClientVersion = "8.0.0";
        options.MessagePackClientVersion = "8.0.0";
#elif NET7_0_OR_GREATER
        options.SignalRClientVersion = "7.0.0";
        options.MessagePackClientVersion = "7.0.0";
#elif NET6_0_OR_GREATER
        options.SignalRClientVersion = "6.0.0";
        options.MessagePackClientVersion = "6.0.0";
#elif NET5_0_OR_GREATER
        options.SignalRClientVersion = "5.0.0";
        options.MessagePackClientVersion = "5.0.0";
#endif
    }
}