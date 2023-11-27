using System.Collections.Generic;

using MemoScope.Core;
using MemoScope.Modules.RootPath;

using NUnit.Framework;

namespace UnitTestProject
{
    [TestFixture()]
    public class RootPathAnalysisTests
    {
        MockClrDump clrDump;
        List<ulong> currentPath;
        List<ulong> shortestPath;

        [SetUp]
        public void SetUp()
        {
            clrDump = new MockClrDump();
            currentPath = [];
            shortestPath = null;
        }

        [Test()]
        public void RootPathAnalysisTest_1()
        {
            clrDump.Add(1, 2, 3);
            ulong[] shortestPathl = [1, 2];
            Run(1, shortestPathl);
        }

        [Test()]
        public void RootPathAnalysisTest_2()
        {
            /* 
                1 
                |_ 2 
                   |_ 4
                      |_ 5
                |_ 3
            */

            clrDump.Add(1, 2, 3);
            clrDump.Add(2, 4);
            clrDump.Add(4, 5);

            ulong[] shortestPathl = [1, 3];
            Run(1, shortestPathl);
        }

        [Test()]
        public void RootPathAnalysisTest_3()
        {
            /* 
                1 
                |_ 2 
                   |_ 4
                      |_ 5
                |_ 3
                    |_ 6
                |_ 7
                    |_ 8
            */

            clrDump.Add(1, 2, 3, 7);
            clrDump.Add(2, 4);
            clrDump.Add(4, 5);
            clrDump.Add(3, 6);
            clrDump.Add(4, 5);
            clrDump.Add(7, 8);

            ulong[] shortestPathl = [1, 7, 8];
            Run(1, shortestPathl);
        }

        [Test()]
        public void RootPathAnalysisTest_4()
        {
            /* 
                1 
                |_ 7
                    |_ 8
                |_ 2 
                   |_ 4
                      |_ 5
                |_ 3
                    |_ 6
            */

            clrDump.Add(1, 2, 3, 7);
            clrDump.Add(7, 8);
            clrDump.Add(2, 4);
            clrDump.Add(4, 5);
            clrDump.Add(3, 6);
            clrDump.Add(4, 5);

            ulong[] shortestPathl = [1, 7, 8];
            Run(1, shortestPathl);
        }

        [Test()]
        public void RootPathAnalysisTest_5()
        {
            /* 
                1 
            */

            ulong[] shortestPathl = null;
            Run(1, shortestPathl, false);
        }

        [Test()]
        public void RootPathAnalysisTest_6()
        {
            /* 
                1 
                |_ 2
                   |_ 1
                   |_ 2
                   |_ 3
            */
            clrDump.Add(1, 2);
            clrDump.Add(2, 1);
            clrDump.Add(2, 2);
            clrDump.Add(2, 3);

            ulong[] shortestPathl = [1, 2, 3];
            Run(1, shortestPathl);
        }

        [Test()]
        public void RootPathAnalysisTest_7()
        {
            /* 
                1 
                |_ 2
                   |_ 3
                      |_ 4
                |_ 3
                   |_ 4
            */
            clrDump.Add(1, 2, 3);
            clrDump.Add(2, 3);
            clrDump.Add(3, 4);

            ulong[] shortestPathl = [1, 3, 4];
            Run(1, shortestPathl);
        }

        private void Run(ulong address, ulong[] expectedPath, bool result = true)
        {
            currentPath.Add(address);
            var res = RootPathAnalysis.FindShortestPath(currentPath, ref shortestPath, clrDump);
            Assert.That(res, Is.EqualTo(result));
            Assert.That(shortestPath, Is.EqualTo(expectedPath));
        }
    }

    public class MockClrDump : IClrDump
    {
        public void Add(ulong address, params ulong[] referers)
        {
            this.referers[address] = referers;
        }
        readonly Dictionary<ulong, ulong[]> referers = [];
        public IEnumerable<ulong> EnumerateReferers(ulong address)
        {
            if (referers.TryGetValue(address, out ulong[] refs))
            {
                return refs;
            }
            return new ulong[0];
        }

        public bool HasReferers(ulong address)
        {
            return referers.ContainsKey(address);
        }
    }
}
