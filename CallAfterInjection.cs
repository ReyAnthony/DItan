using System;
using JetBrains.Annotations;

namespace DItan
{
    public enum Priority
    {
        MAX = Int32.MinValue, 
        MID = 0, 
        MIN = Int32.MaxValue
    }

    /**
 * Can only be used on ScriptableObjects for performance purpose
 * (And it's useless on monobehaviors also..)
 *
 * After all dependencies are injected, the method will be called, according to its Priority.
 *
 */
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Method)]
    public class CallAfterInjection : Attribute
    {
        public Priority Priority { get; private set;  }

        public CallAfterInjection(Priority priority = Priority.MIN)
        {
            Priority = priority;
        }
    }
}