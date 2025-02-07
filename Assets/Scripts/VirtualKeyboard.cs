using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VirtualKeyboard : MonoBehaviour
{
    public TMP_InputField inputField;

    void Update()
    {

    }
    public void OnKeyPress(string key)
    {
        inputField.text += key; // Append the pressed key to the input field
    }

    // Method to delete the last character
    public void OnBackspace()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1); // Remove last character
        }
    }
}
