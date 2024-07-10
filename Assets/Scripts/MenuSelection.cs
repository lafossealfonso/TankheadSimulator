using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

}
