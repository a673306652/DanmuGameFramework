using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TabGroupAttribute : MetaAttribute, IGroupAttribute
    {
        public string Name { get; private set; }

        public TabGroupAttribute(string name = "Default")
        {
            Name = name;
        }
    }
}