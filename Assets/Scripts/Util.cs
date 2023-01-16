using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util 
{
    public static void SetLayerRecursively(GameObject _obj, int _newLayerIndex)
    {
        if (_obj == null)
            return;

        _obj.layer = _newLayerIndex;

        foreach (Transform _child in _obj.transform)
        {
            if (_child == null)
                continue;

            SetLayerRecursively(_child.gameObject, _newLayerIndex);
            
        }
    }
}
