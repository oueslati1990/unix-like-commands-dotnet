using UnixCommands.Shared;

return await Cat.RunAsync(args);

public static class Cat
{
    public static async Task<int> RunAsync(string[] args)
    {
        bool showLineNumbers = false;
        var files = new List<string>();

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "-h" or "--help":
                    ShowHelp();
                    return 0;
                case "--version":
                    Console.WriteLine("dotnet-cat 1.0.0");
                    return 0;
                case "-n" or "--number":
                    showLineNumbers = true;
                    break;
                default:
                    if (arg.StartsWith("-") && arg != "-")
                    {
                        ConsoleHelper.WriteError($"dotnet-cat: unknown option: {arg}");
                        return 1;
                    }
                    files.Add(arg);
                    break;
            }
        }

        try
        {
            int lineNumber = 0;

            if (files.Count == 0)
            {
                lineNumber = await ProcessStreamAsync(Console.OpenStandardInput(), showLineNumbers, lineNumber);
            }
            else
            {
                foreach (var file in files)
                {
                    if (file == "-")
                    {
                        lineNumber = await ProcessStreamAsync(Console.OpenStandardInput(), showLineNumbers, lineNumber);
                    }
                    else if (!File.Exists(file))
                    {
                        ConsoleHelper.WriteError($"dotnet-cat: {file}: No such file or directory");
                        return 1;
                    }
                    else
                    {
                        await using var stream = File.OpenRead(file);
                        lineNumber = await ProcessStreamAsync(stream, showLineNumbers, lineNumber);
                    }
                }
            }

            return 0;
        }
        catch (UnauthorizedAccessException ex)
        {
            return ConsoleHelper.ExitWithError($"dotnet-cat: {ex.Message}");
        }
        catch (IOException ex)
        {
            return ConsoleHelper.ExitWithError($"dotnet-cat: {ex.Message}");
        }
    }

    private static async Task<int> ProcessStreamAsync(Stream stream, bool showLineNumbers, int lineNumber)
    {
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync() is { } line)
        {
            lineNumber++;

            if (showLineNumbers)
            {
                Console.WriteLine($"{lineNumber,6}\t{line}");
            }
            else
            {
                Console.WriteLine(line);
            }
        }

        return lineNumber;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("""
            Usage: dotnet-cat [OPTION]... [FILE]...
            Concatenate FILE(s) to standard output.

            With no FILE, or when FILE is -, read standard input.

              -n, --number    number all output lines
              -h, --help      display this help and exit
                  --version   output version information and exit

            Examples:
              dotnet-cat file.txt         Output file.txt content
              dotnet-cat file1 file2      Concatenate file1 and file2
              dotnet-cat -n file.txt      Output with line numbers
              dotnet-cat                  Copy standard input to standard output
            """);
    }
}
