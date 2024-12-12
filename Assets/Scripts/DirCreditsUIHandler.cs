using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DirCreditsUIHandler : MonoBehaviour
{
    public void GoToMainMenu() {
        SceneManager.LoadScene(0);
    }
}
