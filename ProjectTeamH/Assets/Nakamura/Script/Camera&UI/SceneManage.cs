﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    [SerializeField]
    private string SceneName;
    [SerializeField]
    private string SecretName;
    [SerializeField]
    int SecretGoalCount;

    [SerializeField]
    GameObject ClearUI;

    AudioSource audioSource;

    [SerializeField]
    AudioClip ClearSE;

    // Start is called before the first frame update
    void Start()
    {
        ClearUI = GameObject.Find("ClearUI");
        ClearUI.SetActive(false);

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Mathf.Approximately(Time.timeScale, 0f)) { return; }

        if (ClearUI.activeSelf)
        {

        }

        if (Goal.GetGoal())
        {
            ClearUI.SetActive(true);
           

            Time.timeScale = 0f;

            if (Input.GetButtonDown("A"))
            {
                audioSource.PlayOneShot(ClearSE);
                //隠し条件設定なし時通常ゴール
                if (SecretGoalCount == 0)
                {
                    SceneManager.LoadScene(SceneName);
                }

                //かくｓ条件設定あり時通常ゴール
                if (PlayerController.GetNeutralizer() != SecretGoalCount && SecretGoalCount != 0)
                {
                    SceneManager.LoadScene(SceneName);
                }

                //隠し条件設定あり時隠しゴール
                if (PlayerController.GetNeutralizer() == SecretGoalCount && SecretGoalCount != 0)
                {
                    SceneManager.LoadScene(SecretName);
                }
            }     
        }


        

        //アプリケーションの終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
