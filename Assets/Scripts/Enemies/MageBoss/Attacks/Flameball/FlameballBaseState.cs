using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlameballBaseState
{
    public abstract void EnterState(Flameball manager);
    public abstract void UpdateState(Flameball manager);
    public abstract void OnTriggerStay2D(Flameball manager, Collider2D collision);

}
