# DItan
Quick and dirty Dependency Injection for Unity / DI totalement à la noix 

DItan is used to Inject scriptable object at the start of a scene. 

# Usage 

First, create an empty GameObject in the scene with the component "SceneInjector" attached. 
Then create a ScriptableInjector asset with Right click > Create/DItan/ScriptableInjector. 

```
[CreateAssetMenu(menuName = "game/assets/Player stats")]
public class PlayerStats : ScriptableObject
{
    public int Score => 10;
}
```

```
public class ScoreBoard : MonoBehaviour
{
    [Inject] private PlayerStats _stats;
    [SerializeField] private Text _scoreText;

    private void Start()
    {
        _scoreText.text = _stats.Score.ToString();
    }
}
```

```
 private void InternalSpawnIntoScene(Vector3 position)
{
        _refInScene = _injector.InstantiateNew(_modelPrefab, position, Quaternion.identity);
        _selectable = _refInScene.GetComponent<Selectable>();
        _navMeshAgent = _refInScene.GetComponent<NavMeshAgent>();
        ....
}
```

