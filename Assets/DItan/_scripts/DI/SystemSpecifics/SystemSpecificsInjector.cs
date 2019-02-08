using System.Collections.Generic;
using UnityEngine;

public abstract class SystemSpecficsInjector : ScriptableObject
{
    public abstract List<ScriptableObject> ObjectsToInject { get; }
}