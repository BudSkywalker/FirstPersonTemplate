using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextWriter : MonoBehaviour
{
    private enum WriteMode
    {
        Overflow,
        Scroll,
        Delete,
        Stop
    }

    [SerializeField]
    private float timePerLetter = 0.1f;
    [Multiline]
    [SerializeField]
    private string message;
    [SerializeField]
    private WriteMode writeMode;
    private TMP_Text textbox;

    private void Start()
    {
        textbox = GetComponent<TMP_Text>();
        StartTypingMessage();
    }

    public void StartTypingMessage()
    {
        StartCoroutine(AnimateMessage());
    }

    private IEnumerator AnimateMessage()
    {
        textbox.text = "";

        foreach (char c in message)
        {
            textbox.text += c;
            if (textbox.preferredHeight > textbox.GetComponent<RectTransform>().rect.height)
                switch (writeMode)
                {
                    case WriteMode.Overflow:
                        break;
                    case WriteMode.Scroll:
                        while (textbox.preferredHeight > textbox.GetComponent<RectTransform>().rect.height)
                        {
                            textbox.text = textbox.text[1..];
                        }
                        break;
                    case WriteMode.Delete:
                        textbox.text = "";
                        break;
                    case WriteMode.Stop:
                        yield break;
                    default:
                        Debug.LogError("Unknown Write Mode: " + writeMode);
                        break;
                }

            yield return new WaitForSeconds(timePerLetter);
        }
    }
}