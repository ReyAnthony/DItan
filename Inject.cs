using System;
using JetBrains.Annotations;

namespace DItan
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field)]
    public class Inject : Attribute
    {
        [CanBeNull]
        public Type ExplicitType { get; }

        public Inject()
        {
        
        }
    
        public Inject(Type explicitType)
        {
            ExplicitType = explicitType;
        }
    }
}