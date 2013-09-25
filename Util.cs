using System;
using UnityEngine;

public sealed class Util
    {
        public static void DebugPrint(object message){
#if DEBUG
            MonoBehaviour.print("[Kerbopolis]: "+message.ToString());
#endif
        }
    }
