using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    [SerializeField] private int object_ID = -1;

    public int GetObjectID() {
        return object_ID;
    }
}
