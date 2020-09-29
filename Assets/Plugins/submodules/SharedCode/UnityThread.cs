﻿ #define ENABLE_UPDATE_FUNCTION_CALLBACK
 #define ENABLE_LATEUPDATE_FUNCTION_CALLBACK
 #define ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  namespace Plugins.submodules.SharedCode
 {
     /// <summary>
     /// Позволяет вызывать unity api не из главного потока.
     /// </summary>
     public class UnityThread : MonoBehaviour
     {
         //our (singleton) instance
         private static UnityThread instance = null;


         ////////////////////////////////////////////////UPDATE IMPL////////////////////////////////////////////////////////
         //Holds actions received from another Thread. Will be coped to actionCopiedQueueUpdateFunc then executed from there
         private static readonly List<System.Action> actionQueuesUpdateFunc = new List<Action>();

         //holds Actions copied from actionQueuesUpdateFunc to be executed
         readonly List<System.Action> actionCopiedQueueUpdateFunc = new List<System.Action>();

         // Used to know if whe have new Action function to execute. This prevents the use of the lock keyword every frame
         private static volatile bool noActionQueueToExecuteUpdateFunc = true;


         ////////////////////////////////////////////////LATEUPDATE IMPL////////////////////////////////////////////////////////
         //Holds actions received from another Thread. Will be coped to actionCopiedQueueLateUpdateFunc then executed from there
         private static readonly List<System.Action> ActionQueuesLateUpdateFunc = new List<Action>();

         //holds Actions copied from actionQueuesLateUpdateFunc to be executed
         readonly List<System.Action> actionCopiedQueueLateUpdateFunc = new List<System.Action>();

         // Used to know if whe have new Action function to execute. This prevents the use of the lock keyword every frame
         private static volatile bool noActionQueueToExecuteLateUpdateFunc = true;



         ////////////////////////////////////////////////FIXEDUPDATE IMPL////////////////////////////////////////////////////////
         //Holds actions received from another Thread. Will be coped to actionCopiedQueueFixedUpdateFunc then executed from there
         private static readonly List<System.Action> ActionQueuesFixedUpdateFunc = new List<Action>();

         //holds Actions copied from actionQueuesFixedUpdateFunc to be executed
         readonly List<System.Action> actionCopiedQueueFixedUpdateFunc = new List<System.Action>();

         // Used to know if whe have new Action function to execute. This prevents the use of the lock keyword every frame
         private volatile static bool noActionQueueToExecuteFixedUpdateFunc = true;


         //Used to initialize UnityThread. Call once before any function here
         public static void InitUnityThread(bool visible = false)
         {
             if (instance != null)
             {
                 return;
             }

             if (Application.isPlaying)
             {
                 // add an invisible game object to the scene
                 GameObject obj = new GameObject("MainThreadExecuter");
                 if (!visible)
                 {
                     obj.hideFlags = HideFlags.HideAndDontSave;
                 }

                 DontDestroyOnLoad(obj);
                 instance = obj.AddComponent<UnityThread>();
             }
         }

         public void Awake()
         {
             DontDestroyOnLoad(gameObject);
         }

         //////////////////////////////////////////////COROUTINE IMPL//////////////////////////////////////////////////////
#if (ENABLE_UPDATE_FUNCTION_CALLBACK)
         public static void ExecuteCoroutine(IEnumerator action)
         {
             if (instance != null)
             {
                 Execute(() => instance.StartCoroutine(action));
             }
         }

         ////////////////////////////////////////////UPDATE IMPL////////////////////////////////////////////////////
         public static void Execute(System.Action action)
         {
             if (action == null)
             {
                 throw new ArgumentNullException("action");
             }

             lock (actionQueuesUpdateFunc)
             {
                 actionQueuesUpdateFunc.Add(action);
                 noActionQueueToExecuteUpdateFunc = false;
             }
         }

         public void Update()
         {
             if (noActionQueueToExecuteUpdateFunc)
             {
                 return;
             }

             //Clear the old actions from the actionCopiedQueueUpdateFunc queue
             actionCopiedQueueUpdateFunc.Clear();
             lock (actionQueuesUpdateFunc)
             {
                 //Copy actionQueuesUpdateFunc to the actionCopiedQueueUpdateFunc variable
                 actionCopiedQueueUpdateFunc.AddRange(actionQueuesUpdateFunc);
                 //Now clear the actionQueuesUpdateFunc since we've done copying it
                 actionQueuesUpdateFunc.Clear();
                 noActionQueueToExecuteUpdateFunc = true;
             }

             // Loop and execute the functions from the actionCopiedQueueUpdateFunc
             for (int i = 0; i < actionCopiedQueueUpdateFunc.Count; i++)
             {
                 actionCopiedQueueUpdateFunc[i].Invoke();
             }
         }
#endif

         ////////////////////////////////////////////LATEUPDATE IMPL////////////////////////////////////////////////////
#if (ENABLE_LATEUPDATE_FUNCTION_CALLBACK)
         public static void ExecuteInLateUpdate(System.Action action)
         {
             if (action == null)
             {
                 throw new ArgumentNullException("action");
             }

             lock (ActionQueuesLateUpdateFunc)
             {
                 ActionQueuesLateUpdateFunc.Add(action);
                 noActionQueueToExecuteLateUpdateFunc = false;
             }
         }


         public void LateUpdate()
         {
             if (noActionQueueToExecuteLateUpdateFunc)
             {
                 return;
             }

             //Clear the old actions from the actionCopiedQueueLateUpdateFunc queue
             actionCopiedQueueLateUpdateFunc.Clear();
             lock (ActionQueuesLateUpdateFunc)
             {
                 //Copy actionQueuesLateUpdateFunc to the actionCopiedQueueLateUpdateFunc variable
                 actionCopiedQueueLateUpdateFunc.AddRange(ActionQueuesLateUpdateFunc);
                 //Now clear the actionQueuesLateUpdateFunc since we've done copying it
                 ActionQueuesLateUpdateFunc.Clear();
                 noActionQueueToExecuteLateUpdateFunc = true;
             }

             // Loop and execute the functions from the actionCopiedQueueLateUpdateFunc
             for (int i = 0; i < actionCopiedQueueLateUpdateFunc.Count; i++)
             {
                 actionCopiedQueueLateUpdateFunc[i].Invoke();
             }
         }
#endif

         ////////////////////////////////////////////FIXEDUPDATE IMPL//////////////////////////////////////////////////
#if (ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK)
         public static void ExecuteInFixedUpdate(System.Action action)
         {
             if (action == null)
             {
                 throw new ArgumentNullException("action");
             }

             lock (ActionQueuesFixedUpdateFunc)
             {
                 ActionQueuesFixedUpdateFunc.Add(action);
                 noActionQueueToExecuteFixedUpdateFunc = false;
             }
         }

         public void FixedUpdate()
         {
             if (noActionQueueToExecuteFixedUpdateFunc)
             {
                 return;
             }

             //Clear the old actions from the actionCopiedQueueFixedUpdateFunc queue
             actionCopiedQueueFixedUpdateFunc.Clear();
             lock (ActionQueuesFixedUpdateFunc)
             {
                 //Copy actionQueuesFixedUpdateFunc to the actionCopiedQueueFixedUpdateFunc variable
                 actionCopiedQueueFixedUpdateFunc.AddRange(ActionQueuesFixedUpdateFunc);
                 //Now clear the actionQueuesFixedUpdateFunc since we've done copying it
                 ActionQueuesFixedUpdateFunc.Clear();
                 noActionQueueToExecuteFixedUpdateFunc = true;
             }

             // Loop and execute the functions from the actionCopiedQueueFixedUpdateFunc
             for (int i = 0; i < actionCopiedQueueFixedUpdateFunc.Count; i++)
             {
                 actionCopiedQueueFixedUpdateFunc[i].Invoke();
             }
         }
#endif

         public void OnDisable()
         {
             if (instance == this)
             {
                 instance = null;
             }
         }
     }
 }