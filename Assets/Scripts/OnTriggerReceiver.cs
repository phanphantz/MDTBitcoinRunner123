using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OnTriggerReceiver : MonoBehaviour
{
    public Action onTriggerEnter;
    public void NotifyOnTriggerEnter()
    {
        Debug.Log("Notify On Trigger Enter : " + name);
        onTriggerEnter?.Invoke();
    }

}
