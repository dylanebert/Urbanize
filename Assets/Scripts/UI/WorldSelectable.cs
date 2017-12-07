using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldSelectable {

    void Select();
    void Deselect();
    void Hover();
    void Dehover();
}
