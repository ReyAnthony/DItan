using UnityEngine;
public class SceneInjector : MonoBehaviour
{
    [SerializeField] private Injector _injector;

    private void Awake()
    {
        _injector.Inject();
    }
}
