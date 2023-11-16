using System;
using System.Drawing;

namespace MemoDummy
{
    public enum EBool
    {
        _True_,
        _False_,
        _FileNotFound_
    }

    struct StructData
    {
        public EBool myFlags;
    }

    public interface IMyInterface
    {
        int Id { get; }
        double MyDoubleValue { get; }
    }

    public class MyInterfaceImplV1 : IMyInterface
    {
        private static int n = 0;
        protected int backfield_id;
        public int Id
        {
            get
            {
                return backfield_id;
            }
        }
        private static int CurrentN()
        {
            return n++;
        }

        public double MyDoubleValue { get; set; }
        public string Name { get; set; }
        public MyInterfaceImplV1()
        {
            backfield_id = CurrentN();
            MyDoubleValue = backfield_id + 0.001;
            Name = $"#{0:Id}";
        }
    }

    public class MyInterfaceImplV2 : MyInterfaceImplV1
    {
        public DateTime TimeStamp { get; set; }
        public MyInterfaceImplV2()
        {
            backfield_id *= -1;
            MyDoubleValue = backfield_id - 0.001;
            TimeStamp = DateTime.Now;
        }
    }

    class InternalData
    {
        public string Desc;
        public bool IsNeg;
        public double X { get; set; }
        public double Y { get; set; }
    }
    public abstract class AnAbstractType
    {
        public abstract double AbstractDoubleProperty { get; }
        public double MyDoubleProperty { get; set; }
    }

    public class AnAbstractTypeImpl : AnAbstractType
    {
        private static int n = 0;
        public new double MyDoubleProperty { get; }
        public override double AbstractDoubleProperty { get; }
        private static void IncrementN()
        {
            n++;
        }
        public AnAbstractTypeImpl()
        {
            IncrementN();
            AbstractDoubleProperty = -n - 0.0005;
            base.MyDoubleProperty = n + 0.0005;
            this.MyDoubleProperty = n + 0.0006;
        }
    }

    internal class ComplexObject
    {
        static private int n = 0;

        internal StructData StructData => structData;

        StructData structData;
        public string[] SomeStrings { get; set; }
        private static int CurrentN()
        {
            return n++;
        }
        public ComplexObject()
        {
            int id = CurrentN();
            structData = new StructData()
            {
                myFlags = (id % 3) switch
                {
                    0 => EBool._False_,
                    1 => EBool._True_,
                    _ => EBool._FileNotFound_,
                }
            };
            SomeStrings = new string[id % 32];
            for (int i = 0; i < id % 32; i++)
            {
                int nl = (id + i);
                SomeStrings[i] = nl.ToString("X");
            }
        }
    }
}