using System.Collections.Generic;
using System.ComponentModel;

namespace MemoDummy
{
    public class ArraysGeneratorScript : AbstractMemoScript
    {
        public override string Name => "Arrays Generator";
        public override string Description => "Creates some arrays, size, type and number can be configured";

        [Category("Config")]
        public long Size { get; set; } = 1000;

        [Category("Config")]
        public int Count { get; set; } = 100;

        [Category("Config")]
        public int NullPeriod { get; set; } = 4;

        public override void Run()
        {
            List<object> objects = [];

            for (int i = 0; i < Count; i++)
            {
                if (stopRequested)
                {
                    break;
                }
                int[] intArray = new int[Size];
                string[] stringArray = new string[Size];

                objects.Add(intArray);
                objects.Add(stringArray);

                for (int j = 0; j < Size; j++)
                {
                    intArray[j] = i * j;
                    if (j % NullPeriod != 0)
                    {
                        stringArray[j] = $"#{i}_{j}";
                    }
                }
            }
        }
    }
}
