
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DItan/Default System Specfic Injector")]
public class DefaultSystemInjector : SystemSpecficsInjector
{
    [SerializeField] private List<ScriptableObject> _forPc;
    
    //TODO, here we will check if the player uses a gamepad etc..
    //If he is on a game console or whatnot
    public override List<ScriptableObject> ObjectsToInject => _forPc;
}
