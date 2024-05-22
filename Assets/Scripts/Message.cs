using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    private static Message instance;
    public static Message Instance { get { return instance; } }

    public GameObject warningMessageObject;
    public TextMeshProUGUI warningContent;
    Coroutine currentCoroutine;

    private void Awake()
    {
        if (instance!=null&&instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    public void WarningMessage(string text)
    {
        warningContent.text = text;
        if (currentCoroutine!=null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(Duration(warningMessageObject));
        }
        else
        {
            currentCoroutine = StartCoroutine(Duration(warningMessageObject));
        }
    }

    IEnumerator Duration(GameObject messageObejct)
    {
        messageObejct.SetActive(true);
        yield return new WaitForSeconds(4);
        messageObejct.SetActive(false);
        currentCoroutine = null;
    }
}
