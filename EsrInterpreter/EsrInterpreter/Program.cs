namespace EsrInterpreter;

internal class Program
{
    private static void Main(string[] args)
    {
        string? fileName;

        if (args.Length < 1)
        {
            Console.WriteLine("USAGE: esri [filename.bf]");
            Console.Write("Filename: ");
            fileName = Console.ReadLine();

            if (fileName == null) throw new ArgumentException("Filename must be provided!");
        }
        else
        {
            fileName = args[0];
        }

        string fileContent;

        try
        {
            fileContent = File.ReadAllText(fileName);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: Could not read file: {fileName} with exception: {e.Message}");
            return;
        }
        
        Interpreter.ExecuteProgram(fileContent, 512);

        Console.WriteLine("Finished Executing!");
        Console.ReadKey();
    }
}
