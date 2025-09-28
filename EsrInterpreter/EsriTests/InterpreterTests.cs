namespace EsriTests;

public class InterpreterTests
{
    [Fact]
    public void TestBasicPointerFunctions()
    {
        EsrInterpreter.Interpreter.ExecuteProgram("+", 1, out int[] dump1, out var _);
        Assert.True(dump1[0] == 1, $"The Add instruction is not behaving correctly.");
        
        EsrInterpreter.Interpreter.ExecuteProgram("-", 1, out int[] dump2, out var _);
        Assert.True(dump2[0] == -1, "The Subtract instruction is not behaving correctly.");
        
        EsrInterpreter.Interpreter.ExecuteProgram(">+", 2, out int[] dump3, out var _);
        Assert.True(dump3[1] == 1, "The Shift instruction is not behaving correctly.");
        
        EsrInterpreter.Interpreter.ExecuteProgram(">-", 2, out int[] dump4, out var _);
        Assert.True(dump4[1] == -1, "The Shift instruction is not behaving correctly.");
    }

    [Fact]
    public void TestLoops()
    {
        EsrInterpreter.Interpreter.ExecuteProgram("++[-]-",1, out int[] dump1, out var _);
        Assert.True(dump1[0] == -1, "Loops are not behaving correctly");
        
        EsrInterpreter.Interpreter.ExecuteProgram("+>+>+>+<<<<[>]+", 5, out int[] dump2, out var _);
        Assert.True(dump2[dump2.Length-1] == 1, "Loop gliding is not behaving correctly.");
    }

    [Fact]
    public void TestNestedLoops()
    {
        EsrInterpreter.Interpreter.ExecuteProgram("+>++++<[>[-]<-]", 2, out int[] dump1, out var _);
        Assert.True(dump1[0] == 0 && dump1[1] == 0, "Nested loops are not behaving correctly.");
    }

    [Fact]
    public void TestAddressReset()
    {
        EsrInterpreter.Interpreter.ExecuteProgram(">+++++,+", 2, out int[] dump1, out var _);
        Assert.True(dump1[0] == 1, "Resetting");
    }

    [Fact]
    public void TestGraphicsBufferWrite()
    {
        EsrInterpreter.Interpreter.ExecuteProgram("+$[-]-$", 1, out int[] dump1, out int[,] fbDump);
        Assert.True(dump1[0] == 1 && fbDump[0,0] == -1, $"Memory type switching is not behaving correctly.");
    }

    [Fact]
    public void TestMemoryCopy()
    {
        EsrInterpreter.Interpreter.ExecuteProgram("+$|$", 1, out int[] dump1, out int[,] fbDump);
        Assert.True(dump1[0] == fbDump[0,0] && fbDump[0,0] == 1);
    }
}