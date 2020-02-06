using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

 namespace DItan
 {
     [CreateAssetMenu(menuName = "DItan/Scriptable Injector")]
     public class Injector : ScriptableObject
     {
         [SerializeField] private List<ScriptableObject> _commonObjectsToInject;
         private List<Method> _methods;

         private class Method
         {
             private delegate void MethodDelegate();

             private readonly MethodInfo _methodInfo;
             private readonly Object _obj;
             private readonly MethodDelegate _delegate;
             public Priority Priority { get; }

             public Method(MethodInfo methodInfo, Object obj, Priority priority)
             {
                 _methodInfo = methodInfo;
                 _obj = obj;
                 Priority = priority;
                 _delegate = (MethodDelegate) Delegate.CreateDelegate(typeof(MethodDelegate), _obj, _methodInfo);
             }

             public void Invoke()
             {
                 _delegate();
             }
         }
    
         private void OnEnable()
         {
             //OnEnable is called BEFORE OnSceneLoaded.
             SceneManager.sceneLoaded += Inject;
#if UNITY_EDITOR
        AssemblyReloadEvents.afterAssemblyReload += ReloadSceneAfterAssemblyReload;
#endif
         }
    
#if UNITY_EDITOR
    private void ReloadSceneAfterAssemblyReload()
    {
        if (Application.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.ExitPlaymode(); 
            Inject(); //just to avoid error messages 
            throw new UnityException("DItan does not support hot reload."); 
        }
    }
#endif
    
         private void Inject(Scene scene, LoadSceneMode mode)
         {
             Inject();
         }

         public GameObject InstantiateNew(GameObject gameObject, Transform parent)
         {
             List<ScriptableObject> fullList = GetFullListOfInjectable();
             var go = Instantiate(gameObject,  parent);
             InstantiateInternal(go, fullList);
             return go;
         }

         public GameObject InstantiateNew(GameObject gameObject, Vector3 position, Quaternion rotation)
         {
             List<ScriptableObject> fullList = GetFullListOfInjectable();
             var go = Instantiate(gameObject, position, rotation);
             InstantiateInternal(go, fullList);
             return go;
         }

         private void InstantiateInternal(GameObject go, List<ScriptableObject> fullList)
         {
             go.SetActive(false);

             var c = go.GetComponents<Component>();
             var cc = go.GetComponentsInChildren<Component>();

             foreach (var component in c)
             {
                 Inject(component, fullList);
             }

             foreach (var component in cc)
             {
                 Inject(component, fullList);
             }

             go.SetActive(true);
         }

         public void Inject()
         {
             _methods = new List<Method>();
             var objectInstance = Resources.FindObjectsOfTypeAll<Object>();

             if (objectInstance.Any())
             {
                 var fullList = GetFullListOfInjectable();

                 foreach (var obj in objectInstance)
                 {
                     Inject(obj, fullList);
                 }
             }

             var scriptableObjects = Resources.FindObjectsOfTypeAll<ScriptableObject>();
             foreach (var scriptableObject in scriptableObjects)
             {
                 foreach (var method in scriptableObject.GetType()
                     .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                 {
                     foreach (var customAttribute in method.GetCustomAttributes(false))
                     {
                         if (customAttribute.GetType() == typeof(CallAfterInjection))
                         {
                             _methods.Add(new Method(method, (Object) scriptableObject,
                                 ((CallAfterInjection) customAttribute).Priority));
                         }
                     }
                 }
             }
        
             _methods
                 .OrderBy((m) => m.Priority)
                 .ToList()
                 .ForEach(m => m.Invoke());
         }

         private List<ScriptableObject> GetFullListOfInjectable()
         {
             List<ScriptableObject> fullList;
             fullList = new List<ScriptableObject>(_commonObjectsToInject);
        

             fullList.Add(this); //so it can inject itself
             return fullList;
         }

         private void Inject(Object obj, List<ScriptableObject> injecteds)
         {
             obj
                 .GetType()
                 .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic )
                 .ToList()
                 .ForEach(field =>
                 {
                     field.GetCustomAttributes(false)
                         .Where(attr => attr.GetType() == typeof(Inject))
                         .Select(attr => attr as Inject)
                         .ToList()
                         .ForEach((injectScriptable) =>
                         {
                             if (injecteds.Any())
                             {
                                 try
                                 {
                                     var inject = injecteds.Find((m) =>
                                     {
                                         if (injectScriptable.ExplicitType != null)
                                         {
                                             return m.GetType() == injectScriptable.ExplicitType;
                                         }

                                         return m.GetType() == field.FieldType
                                                || m.GetType().BaseType == field.FieldType;
                                     });

                                     if (inject == null)
                                     {
                                         throw new UnityException("Could not find " + field.FieldType +
                                                                  " in the Scriptable objects list");
                                     }

                                     field.SetValue(obj, inject);
                                 }
                                 catch (Exception ex)
                                 {
                                     Debug.Log(ex);
                                 }
                             }
                             else
                             {
                                 throw new UnityException("Nothing to inject");
                             }
                         });
                 });            
         }
     }
 }