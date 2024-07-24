using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchDrawer : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform Control;

    private void Start()
    {
        
    }

    public Vector3 Evaluate(float t)
    {
        Vector3 ac = Vector3.Lerp(A.position, Control.position, t);
        Vector3 cb = Vector3.Lerp(Control.position, B.position, t);
        return Vector3.Lerp(ac, cb, t);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(3, 3, 3), 0.5f);

        if(A == null || B == null || Control == null)
        {
            return;
        }

        for(int i = 0; i < 20; i++)
        {
            Gizmos.DrawSphere(Evaluate(i / 20f), 0.25f);
        }
    }
}
