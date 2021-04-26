using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public bool displayed;
    public GameObject tuto;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (displayed)
            {
                tuto.SetActive(false);
                displayed = false;
            }
            else
            {
                tuto.SetActive(true);
                displayed = true;
            }
        }
    }
}
