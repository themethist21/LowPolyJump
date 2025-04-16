using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCheat : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetString("playerModel", "DOG");
            SceneManager.LoadScene(PlayerPrefs.GetInt("level") + 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerPrefs.SetInt("level", 2);
            PlayerPrefs.SetString("playerModel", "DOG");
            SceneManager.LoadScene(PlayerPrefs.GetInt("level") + 1);
        }
    }
}
