namespace EsrInterpreter;

public class EsriHelpers
{
    /// <summary>
    /// Parses an ASCII character and a ConsoleColor into a single int
    /// value.
    /// </summary>
    /// <param name="asciiChar">The ASCII character to use.</param>
    /// <param name="color">The ConsoleColor the character should be displayed in.</param>
    /// <returns>An integer representing the character and color.</returns>
    /// <exception cref="ArgumentException">Is thrown if the character is not an ASCII character.</exception>
    public int GetValueFromCharAndColor(char asciiChar, ConsoleColor color)
    {
        var asciiCode = (int)asciiChar;


        if (asciiCode >= 0 && asciiCode <= 127)
        {
            var index = (int)color;

            var fullCode = index * 127 + asciiCode;
            return fullCode;
        }

        throw new ArgumentException($"ERROR: Invalid ASCII Value: {asciiCode}");
    }

    public string GetValueString(int value, char positiveChar, char negativeChar)
    {
        var result = "";

        if (value > 0)
            result = new string(positiveChar, value);
        else if (value < 0) result = new string(negativeChar, value);

        return result;
    }

    public string SetToValue(int value)
    {
        return "[-]" + AddValue(value);
    }

    public string MoveToValue(int value, ref int memoryLocation)
    {
        return "," + MoveBy(value, ref memoryLocation);
    }

    public string AddValue(int value)
    {
        return GetValueString(value, '+', '-');
    }

    public string MoveBy(int value, ref int memoryLocation)
    {
        memoryLocation += value;
        return GetValueString(value, '>', '<');
    }

    public string Print(string value)
    {
        var result = "";
        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];
            var generatedNumber = GetValueFromCharAndColor(current, ConsoleColor.White);
            result += $"# '{current}' \n";
            result += SetToValue(generatedNumber) + ">" + "\n";
        }

        return result;
    }
}