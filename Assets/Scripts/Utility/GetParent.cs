using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetParent<T>
{
    public static T TryGetParent(Checkpoints checkpoint)
    {       
        return checkpoint.GetComponentInChildren<T>();
    }

    public static T[] TryGetParents(Checkpoints checkpoint)
    {
        return checkpoint.GetComponentsInChildren<T>();
    }
}
