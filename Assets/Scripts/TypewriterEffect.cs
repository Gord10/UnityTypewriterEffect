using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.VersionControl;
public class TypewriterEffect : MonoBehaviour
{
    [Tooltip("Text mesh that the message will be written at")]
    [SerializeField]
    TextMeshProUGUI m_TextMesh;

    [Tooltip("How many characters written per second")]
    [SerializeField]
    float m_CharSpeed = 2f;

    Coroutine m_TypeWriterCoroutine; //This will be used if player wants to halt the effect to see the whole message instantly

    bool m_IsEffectRunning = false;
    public bool IsRunning => m_IsEffectRunning;

    public float TimeInterval => (1f / m_CharSpeed); //Calculated each time we need to get this value, in case we change effect speed while this effect is still running

    string m_Message; //The string we will show to the player with this typewriter effect

    private void Awake()
    {
        m_TextMesh.text = ""; //Make sure the default content of text mesh won't be seen, even for a glimpse of a frame
    }

    public void StartEffect(string message)
    {
        m_Message = message;
        m_IsEffectRunning = true;

        //m_TypeWriterCoroutine = StartCoroutine(IncreaseMaxVisibleChar(message)); //This is the good one to use.
        m_TypeWriterCoroutine = StartCoroutine(AppendCharsToUiText(message)); //This is the bad one. Here only for demonstration.
    }

    //Characters will be appended to text mesh gradually (with an interval of time for each character) in this approach. NOT advised to use!
    IEnumerator AppendCharsToUiText(string message)
    {
        m_TextMesh.text = ""; //Empty the content of the text mesh, because we will gradually append the characters
        int index = 0;
        while (index < message.Length)
        {
            m_TextMesh.text += message[index]; //Append the character into text mesh
            index++;
            yield return new WaitForSeconds(TimeInterval);
        }

        print("Typewriter effect is completed");
        m_IsEffectRunning = false;
    }

    //textMesh.maxVisibleCharacters will increase gradually in this approach. Advised to use this function.
    IEnumerator IncreaseMaxVisibleChar(string message)
    {
        m_TextMesh.text = message; //Make the text mesh's content the whole message string right at the beginning. So the characters will stay at the correct positions since the beginning

        m_TextMesh.maxVisibleCharacters = 0;
        int messageCharLength = message.Length;
        while (m_TextMesh.maxVisibleCharacters < messageCharLength)
        {
            m_TextMesh.maxVisibleCharacters++;
            yield return new WaitForSeconds(TimeInterval);
        }

        m_IsEffectRunning = false;
        print("Typewriter effect is completed");
    }

    //Let player see the whole message in instant
    public void Halt()
    {
        if (m_TypeWriterCoroutine != null)
        {
            StopCoroutine(m_TypeWriterCoroutine);
        }

        m_TextMesh.text = m_Message;
        m_TextMesh.maxVisibleCharacters = int.MaxValue;
        print("Typewriter effect halted by player");
        m_IsEffectRunning = false;
    }

    private void Update()
    {
        //We assume player needs to press left mouse click to halt the effect, if effect is still running
        if (IsRunning)
        {
            bool isPlayerHaltingTypewriter = Input.GetMouseButtonDown(0);
            if (isPlayerHaltingTypewriter)
            {
                Halt();
            }
        }
    }
}
