using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    public GameObject pnPause;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab)){
            pnPause.SetActive(true);
        }

    }
    public void exit(){
        Application.Quit();
    }
    public void loadScene(string scene){
        SceneManager.LoadScene(scene);
    }
    public void resume(){
        pnPause.SetActive(false);
    }

}
