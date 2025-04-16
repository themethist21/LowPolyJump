using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ClearPlayerPrefsOnBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0; // Orden de ejecución (puedes dejarlo en 0)

    public void OnPreprocessBuild(BuildReport report)
    {
        // Limpia los valores de PlayerPrefs antes de construir el juego
        PlayerPrefs.SetFloat("level1Best", 0f); // Restaura el valor de level1Best a 0
        PlayerPrefs.SetFloat("level2Best", 0f); // Restaura el valor de level2Best a 0
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs limpiadas antes de la construcción.");
    }
}
