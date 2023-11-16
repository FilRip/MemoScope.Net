using System.Collections.Generic;

namespace MemoDummy
{
    public class RefererScript : AbstractMemoScript
    {
        public override string Name => "Referers";
        public override string Description => "Creates some objects references patterns to be analyzed with Referers Module";

        private List<object> objectsList;
        private object[] objectsArray;
        private HashSet<object> objectsSet;


        public int NbObjects { get; set; } = 1000;

        public override void Run()
        {
            objectsList = new List<object>();
            objectsArray = new object[NbObjects];
            objectsSet = new HashSet<object>();

            for (int i = 0; i < NbObjects; i++)
            {
                var o = new ComplexObject();
#pragma warning disable S2583 // False positive, Conditionally executed code should be reachable
                switch (i % 3)
                {
                    case 0:
                        objectsList.Add(o);
                        break;
                    case 1:
                        objectsArray[i / 3] = o;
                        break;
                    case 2:
                        objectsSet.Add(o);
                        break;
                }
#pragma warning restore S2583 // Conditionally executed code should be reachable
            }
        }
    }
}
