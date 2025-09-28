namespace EsrInterpreter;

internal class Program
{
    /* The width and height of the Framebuffer
       provided to the language.
     */
    private static readonly int FramebufferWidth = 15;
    private static readonly int FramebufferHeight = 15;

    private static readonly EsriHelpers EsriHelpers = new();

    /// <summary>
    /// Prints the contents of the framebuffer to the console using colors.
    /// </summary>
    /// <param name="frameBuffer">2D framebuffer array of encoded char/color values.</param>
    /// <exception cref="ArgumentException">Thrown if a color value is invalid.</exception>
    private static void DisplayFrameBuffer(int[,] frameBuffer)
    {
        Console.Clear();
        for (var row = 0; row < frameBuffer.GetLength(0); row++)
        {
            for (var col = 0; col < frameBuffer.GetLength(1); col++)
            {
                var val = frameBuffer[row, col];

                var character = val % 127;
                var colorVal = val / 127;

                var convertedChar = (char)character;

                if (colorVal < 0 || colorVal > 15)
                {
                    Console.WriteLine("ERROR: Invalid Color Value on framebuffer!");
                    throw new ArgumentException();
                }

                Console.ForegroundColor = (ConsoleColor)colorVal;

                Console.Write(convertedChar);
            }

            Console.Write("\n");
        }
    }
    
    /// <summary>
    /// Converts a 1D framebuffer index into 2D row/column coordinates.
    /// </summary>
    /// <param name="input">The 1D index.</param>
    /// <param name="row">The calculated row output.</param>
    /// <param name="col">The calculated column output.</param>
    private static void ConvertToFramebufferAddress(int input, out int row, out int col)
    {
        row = input / FramebufferWidth;
        col = input % FramebufferHeight;
    }
    
    /// <summary>
    /// Sets a value in 1D or 2D memory.
    /// </summary>
    /// <param name="memory">Target memory array (1D or 2D).</param>
    /// <param name="index">Index to set.</param>
    /// <param name="value">Value to assign.</param>
    private static void SetMemoryVal(Array memory, int index, int value)
    {
        if (memory.Rank == 1)
        {
            memory.SetValue(value, index);
        }
        else if (memory.Rank == 2)
        {
            ConvertToFramebufferAddress(index, out var row, out var col);
            memory.SetValue(value, row, col);
        }
    }
    
    /// <summary>
    /// Gets a value from 1D or 2D memory.
    /// </summary>
    /// <param name="memory">Memory array (1D or 2D).</param>
    /// <param name="index">Index to retrieve.</param>
    /// <returns>Value at the given index.</returns>
    /// <exception cref="ArgumentNullException">Thrown if memory is null.</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if index is negative or out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown if memory is not 1D or 2D.</exception>
    private static int GetMemoryVal(Array memory, int index)
    {
        if (memory == null) throw new ArgumentNullException(nameof(memory));
        if (index < 0) throw new IndexOutOfRangeException("Index must be non-negative.");

        if (memory.Rank == 1)
        {
            if (index >= memory.Length)
                throw new IndexOutOfRangeException("Index was outside the bounds of the 1D array.");
            return Convert.ToInt32(memory.GetValue(index));
        }

        if (memory.Rank == 2)
        {
            var rows = memory.GetLength(0);
            var cols = memory.GetLength(1);
            if (rows == 0 || cols == 0)
                throw new ArgumentException("2D array has zero rows or columns.", nameof(memory));

            var total = (long)rows * cols;
            if (index >= total)
                throw new IndexOutOfRangeException($"Index {index} exceeds total elements {total} of the 2D array.");

            var row = index / cols;
            var col = index % cols;

            return Convert.ToInt32(memory.GetValue(row, col));
        }

        throw new ArgumentException("Only 1D or 2D arrays are supported.", nameof(memory));
    }

    /// <summary>
    /// Gets the total size of the memory array in cells.
    /// </summary>
    /// <param name="memory">Memory array (1D or 2D).</param>
    /// <returns>Total number of elements.</returns>
    private static int GetMemorySize(Array memory)
    {
        if (memory.Rank == 1) return memory.Length;

        return memory.GetLength(0) * memory.GetLength(1);
    }

    /// <summary>
    /// Changes the value at a given memory address by an amount.
    /// </summary>
    /// <param name="memory">Memory array (1D or 2D).</param>
    /// <param name="address">Address to modify.</param>
    /// <param name="amount">Amount to add or subtract.</param>
    /// <exception cref="ArgumentException">Thrown if address is out of range.</exception>
    private static void ChangeBy(Array memory, int address, int amount)
    {
        if (address < 0 || address >= memory.Length)
        {
            Console.WriteLine("ERROR: Value is out of bounds!");
            throw new ArgumentException();
        }

        SetMemoryVal(memory, address, GetMemoryVal(memory, address) + amount);
    }

    /// <summary>
    /// Executes a single Brainf*** / ES instruction.
    /// </summary>
    /// <param name="instruction">Instruction character.</param>
    /// <param name="memory">1D RAM memory array.</param>
    /// <param name="frameBuffer">2D framebuffer array.</param>
    /// <param name="currentAddress">Reference to current memory address.</param>
    /// <param name="editingFrameBuffer">Reference flag: true if writing to framebuffer.</param>
    private static void ExecuteInstruction(char instruction, int[] memory,
        int[,] frameBuffer, ref int currentAddress, ref bool editingFrameBuffer)
    {
        Array currentMemory = memory;

        if (editingFrameBuffer) currentMemory = frameBuffer;

        switch (instruction)
        {
            case '+':
                ChangeBy(currentMemory, currentAddress, 1);
                break;
            case '-':
                ChangeBy(currentMemory, currentAddress, -1);
                break;
            case '>':
                currentAddress++;

                // Loops around to 0
                if (currentAddress >= GetMemorySize(currentMemory)) currentAddress = 0;
                break;
            case '<':
                currentAddress--;

                // Loops around to the max number
                if (currentAddress < 0) currentAddress = GetMemorySize(currentMemory) - 1;
                break;
            case '$':
                editingFrameBuffer = !editingFrameBuffer;
                break;
            case ';':
                DisplayFrameBuffer(frameBuffer);
                break;
            case '?':
                SetMemoryVal(currentMemory, currentAddress, currentAddress);
                break;
            case '|':
                Array otherMemory = memory;

                if (otherMemory == currentMemory) otherMemory = frameBuffer;

                SetMemoryVal(currentMemory, currentAddress, GetMemoryVal(otherMemory, currentAddress));

                break;
            case '.':
                var characterValue = Console.ReadKey().KeyChar;

                if (characterValue > 127) break;

                var calculatedValue = EsriHelpers.GetValueFromCharAndColor(characterValue, ConsoleColor.White);
                SetMemoryVal(currentMemory, currentAddress, calculatedValue);
                break;
            case ',':
                currentAddress = 0;
                break;
        }
    }

    /// <summary>
    /// Finds the matching bracket for a loop.
    /// </summary>
    /// <param name="code">ES program code.</param>
    /// <param name="pos">Position of the bracket to match.</param>
    /// <returns>Index of the matching bracket.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if pos is outside the code.</exception>
    /// <exception cref="ArgumentException">Thrown if pos is not at a bracket.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no match is found.</exception>
    private static int FindMatchingBracket(string code, int pos)
    {
        if (pos < 0 || pos >= code.Length) throw new ArgumentOutOfRangeException(nameof(pos));
        var c = code[pos];
        if (c != '[' && c != ']') throw new ArgumentException("ERROR: Position must be at a bracket!");

        var depth = 1;
        if (c == '[')
        {
            for (var i = pos + 1; i < code.Length; i++)
                if (code[i] == '[')
                {
                    depth++;
                }
                else if (code[i] == ']')
                {
                    depth--;
                    if (depth == 0) return i;
                }
        }
        else
        {
            for (var i = pos - 1; i >= 0; i--)
                if (code[i] == ']')
                {
                    depth++;
                }
                else if (code[i] == '[')
                {
                    depth--;
                    if (depth == 0) return i;
                }
        }

        throw new InvalidOperationException("No matching bracket found.");
    }
    
    /// <summary>
    /// Initializes the framebuffer with blank spaces (white color).
    /// </summary>
    /// <param name="buf">Output initialized framebuffer.</param>
    private static void InitializeFrameBuffer(out int[,] buf)
    {
        var frameBuffer = new int[FramebufferWidth, FramebufferHeight];
        for (var i = 0; i < frameBuffer.GetLength(0); i++)
        for (var j = 0; j < frameBuffer.GetLength(1); j++)
            frameBuffer[i, j] = EsriHelpers.GetValueFromCharAndColor(' ', ConsoleColor.White);

        buf = frameBuffer;
    }

    /// <summary>
    /// Executes a program with memory and framebuffer.
    /// </summary>
    /// <param name="bfDecoded">Source code string.</param>
    /// <param name="memorySize">Size of RAM memory (in cells).</param>
    private static void ExecuteBf(string bfDecoded, int memorySize)
    {
        var memorySizeBytes = memorySize * 4;
        var memory = new int[memorySize];
        var currentAddress = 0;

        var editingFrameBuffer = false;

        InitializeFrameBuffer(out var frameBuffer);

        Console.WriteLine($"Initializing Brainfuck interpreter with {memorySizeBytes} Bytes of RAM.");

        // IMPORTANT: In this code 'pc' acts similarly to a
        // program counter. This means that when, for example
        // a loop is finished, the program counter is changed.
        int pc = 0;
        
        do
        {
            var instruction = bfDecoded[pc];

            Array selectedMemory = memory;

            if (editingFrameBuffer) selectedMemory = frameBuffer;

            var memoryVal = GetMemoryVal(selectedMemory, currentAddress);

            if (instruction != '(' && instruction != ')'
                                   && instruction != '[' && instruction != ']')
            {
                //Console.Write(instruction);
                ExecuteInstruction(instruction, memory, frameBuffer, ref currentAddress, ref editingFrameBuffer);
            }
            else
            {
                if (instruction == '[')
                {
                    if (memoryVal == 0) pc = FindMatchingBracket(bfDecoded, pc);
                }
                else if (instruction == ']' && memoryVal != 0)
                {
                    pc = FindMatchingBracket(bfDecoded, pc);
                }
            }

            pc++;
        } while (pc < bfDecoded.Length);

        Console.WriteLine(string.Join(", ", memory));
    }

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

        ExecuteBf(fileContent, 512);

        Console.WriteLine("Finished Executing!");
        Console.ReadKey();
    }
}
