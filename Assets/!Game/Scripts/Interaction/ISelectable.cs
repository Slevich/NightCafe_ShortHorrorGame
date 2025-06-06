using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    void Select ();
    void Deselect ();
    public bool LockToCast { get; set; }
}
