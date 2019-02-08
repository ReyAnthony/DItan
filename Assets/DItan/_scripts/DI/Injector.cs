using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "DItan/Scriptable Injector")]
public class Injector : ScriptableObject
{
    [SerializeField] private List<ScriptableObject> _commonObjectsToInject;
    [SerializeField] private SystemSpecficsInjector _specficsInjector;
    
    public void Inject()
    {
        List<ScriptableObject> fullList;
        if (_specficsInjector != null)
        {
             fullList = 
                 new List<ScriptableObject>(_commonObjectsToInject.Concat(_specficsInjector.ObjectsToInject));
        }
        else
        {
            fullList = _commonObjectsToInject;
        }
        
        var objectInstance = Resources.FindObjectsOfTypeAll<Object>();
        if (objectInstance.Any())
        {
            foreach (var obj in objectInstance)
            {
                foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    foreach (var customAttribute in field.GetCustomAttributes())
                    {
                        if (customAttribute.GetType() == typeof(Inject))
                        {
                            var injectScriptable = (Inject) customAttribute;
                            if (fullList.Any())
                            {
                                try
                                {
                                    var inject = fullList.Find((m) =>
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
                                        throw new UnityException("Could not find " + field.FieldType + " in the Scriptable objects list");
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
                                throw new UnityException("Could not find " + field.FieldType + " in the Scriptable objects list");
                            }
                        }
                    }
            
                }
            }
        }
    }
}