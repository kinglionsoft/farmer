using System;

namespace DebugDemo
{
    class Program
    {
        static void Main(string[] args)
        {            
            throw new Exception("crash now"); // crash
        }
    }
}
