﻿using UnityEngine;

 namespace DItan
 {
     [ExecuteInEditMode]
     public class SceneInjector : MonoBehaviour
     {
         //Just so it loads it while in the editor and call the onEnableFunction..
         [SerializeField] private Injector _injector;
     }
 }