using System;
using Mahjong;
using Mahjong.Test;

class FocusedProgram
{
    static void Main(string[] args)
    {
        Console.WriteLine("====Focused Python Equivalent Tests");        
        // Python等価テストのみ実行
        PythonEquivalentTests.RunAllTests();
    }
}
