using UnityEngine;

public class ExitGameOnEscape : MonoBehaviour
{
    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Exit the game if in a build
            Application.Quit();

            // If in the editor, log a message (since Application.Quit doesn't work in the editor)
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}

