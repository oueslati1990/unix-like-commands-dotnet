namespace UnixCommands.Shared;

public static class ConsoleHelper
{
    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Error.WriteLine(message);
        Console.ResetColor();
    }

    public static int ExitWithError(string message, int exitCode = 1)
    {
        WriteError(message);
        return exitCode;
    }
}
