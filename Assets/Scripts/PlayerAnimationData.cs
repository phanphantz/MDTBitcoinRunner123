using UnityEngine;

public class PlayerAnimationData
{
     public Transform obj;
     public Vector3 targetValue;
     public float time;
     public bool isShouldDeactivate;

     public PlayerAnimationData(Transform obj, Vector3 scale, float time, bool deactivate)
     {
        this.obj = obj;
        this.targetValue = scale;
        this.time = time;
        this.isShouldDeactivate = deactivate;
     }
     
}
