using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public WebQueries WebQueries;

    void Start()
    {
        Instance = this;
        WebQueries = GetComponent<WebQueries>();
    }
}
