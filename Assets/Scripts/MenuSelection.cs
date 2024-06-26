using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class MenuSelection : MonoBehaviour
{
    [SerializeField] GameObject underScoreLeft;
    [SerializeField] GameObject underScoreRight;
    [SerializeField] bool yes;

    private void Start()
    {
        yes = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            yes = !yes;
        }

        underScoreLeft.SetActive(yes);
        underScoreRight.SetActive(!yes);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(yes == false)
            {
                Application.Quit();
            }
        }
    }

}
