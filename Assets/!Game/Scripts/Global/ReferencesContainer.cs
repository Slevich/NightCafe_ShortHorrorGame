using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferencesContainer : MonoBehaviour
{
    private static ReferencesContainer _instance;
    public static ReferencesContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ReferencesContainer>();
            }

            return _instance;
        }
    }


}
