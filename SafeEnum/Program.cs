using System;
using System.Diagnostics;
using Ardalis.SmartEnum;

namespace SafeEnum
{
    class Program
    {
       public static TestSmartEnum a = TestSmartEnum.One;
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            var count = int.Parse(args[0]);
            sw.Start();
            TestEnum(count);
            sw.Stop();
            System.Console.WriteLine(sw.ElapsedTicks);

            sw.Reset();
            sw.Start();
            TestSEnum(count);
            sw.Stop();
            System.Console.WriteLine(sw.ElapsedTicks);
        }

        static void TestEnum(int count)
        {
            var a = ETest.A;
            while (count > 0)
            {
                Method(a);
                count--;
            }
        }

        static void TestSEnum(int count)
        {
            while (count > 0)
            {
                MethodSmart(a);
                count--;
            }
        }

        static void Method(ETest t)
        {
            var b = t;
        }
        static void MethodSmart(TestSmartEnum t)
        {
            var b = t;
        }
    }

    public enum ETest
    {
        A, B, C, D, E, F, G
    }

    public class TestSmartEnum : SmartEnum<TestSmartEnum, int>
    {
        public static TestSmartEnum One = new TestSmartEnum(nameof(One), 1);
        public static TestSmartEnum Two = new TestSmartEnum(nameof(Two), 2);
        public static TestSmartEnum Three = new TestSmartEnum(nameof(Three), 3);

        protected TestSmartEnum(string name, int value) : base(name, value)
        {
        }
    }
}
