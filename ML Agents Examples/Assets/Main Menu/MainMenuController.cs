using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string[] sceneNames;

    [SerializeField] private Transform content;
    [SerializeField] private GameObject buttonPrefab;

    private void Start()
    {
        foreach (string scene in sceneNames)
        {
            GameObject button = Instantiate(buttonPrefab, content);
            button.GetComponentInChildren<TMP_Text>().text = scene;
            Button buttonComponent = button.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => SceneManager.LoadScene(scene));
        }

        Cursor.visible = true;
    }
}
