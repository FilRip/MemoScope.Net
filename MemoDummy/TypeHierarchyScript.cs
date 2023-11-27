using System.Collections.Generic;

namespace MemoDummy
{
    public class TypeHierarchyScript : AbstractMemoScript
    {
        public override string Name => "Type Hierarchy";
        public override string Description => "Creates some type to study hierarchy";

#pragma warning disable S4487, IDE0052 // Unread "private" fields should be removed
        private List<object> objects;
#pragma warning restore S4487, IDE0052 // Unread "private" fields should be removed

        public override void Run()
        {
            objects =
            [
                new FirstTestClass(),
                new SecondTestClass()
            ];
        }
    }

    public abstract class MyAbstractClass
    {
        public string Name { get; }
        public abstract void Init();
    }

    public class FirstTestClass : MyAbstractClass, IMySecondInterface
    {
        public int Id { get; private set; }
        public override void Init()
        {
        }
    }
    public class SecondTestClass : FirstTestClass, IAnotherInterface
    {
        public string Description { get; protected set; }
        public void DoSomething()
        {
            // Ignore content method
        }
    }
    public interface IMyFirstInterface
    {

    }
    public interface IMySecondInterface : IMyFirstInterface
    {

    }
    public interface IAnotherInterface
    {

    }
}
