using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // Singleton para acceder desde cualquier lugar

    [Header("Audio Sources")]
    public AudioSource sfxSource; // Para efectos de sonido

    public AudioSource sfxjumpSource; // Para efectos de sonido

    public AudioSource sfxcoinSource; // Para sonidos en bucle
    public AudioSource loopSource; // Para sonidos en bucle
    public AudioSource musicSource; // Para música de fondo

    [Header("Clips de sonido")]
    public AudioClip[] soundClips; // Lista de sonidos

    private void Awake()
    {
        // Configurar Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistir entre escenas
            // Suscribirse al evento de cambio de escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Reproducir efecto de sonido
    public void PlaySFX(string clipName, float volume = 1.0f)
    {
        AudioClip clip = GetClipByName(clipName);
        if (clipName == "jump"){
            sfxSource.Stop();
        }
        
        if (clip != null && sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume); // Asegura que esté entre 0 y 1
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Clip '{clipName}' no encontrado o AudioSource no asignado.");
        }
    }

    public void PlaySFXJump(float volume = 1.0f){
        if (sfxjumpSource.isPlaying){
            sfxjumpSource.Stop();
        }
        sfxjumpSource.volume = Mathf.Clamp01(volume);
        sfxjumpSource.loop = false;
        AudioClip clip = GetClipByName("jump");
        sfxjumpSource.PlayOneShot(clip);
    }

    public void PauseAllSounds(){
        sfxSource.Pause();
        sfxjumpSource.Pause();
        sfxcoinSource.Pause();
        loopSource.Pause();
        musicSource.Pause();
    }

    public void StopAllSounds(){
        sfxSource.Stop();
        sfxjumpSource.Stop();
        sfxcoinSource.Stop();
        loopSource.Stop();
        musicSource.Stop();
    }

    public void ResumeAllSounds(){
        sfxSource.UnPause();
        sfxjumpSource.UnPause();
        sfxcoinSource.UnPause();
        loopSource.UnPause();
        musicSource.UnPause();
    }


    public void PlaySFXCoin(float volume = 1.0f){
        sfxcoinSource.volume = Mathf.Clamp01(volume);
        sfxcoinSource.loop = false;
        AudioClip clip = GetClipByName("coin");
        sfxcoinSource.PlayOneShot(clip);
    }

        // Reproducir sonidos en bucle
    public void PlayLoopSound(string clipName, float volume = 1.0f)
    {
        
        if (loopSource != null){
            loopSource.volume = Mathf.Clamp01(volume); // Asegura que esté entre 0 y 1
            AudioClip clip = GetClipByName(clipName);
            if (loopSource.clip != clip)
            {
                loopSource.clip = clip;
                loopSource.loop = true;
                loopSource.Play();
            }else if (!loopSource.isPlaying)
            {
                loopSource.Play(); // Reanuda si está detenido
            }
        }

    }

    public void StopLoopSound()
    {
        loopSource.Stop();
    }

    public void RePlaySounds(){
        loopSource.Play();
    }

    // Reproducir música
    public void PlayMusic(string clipName, float volume = 1.0f, float delayInSeconds = 0.0f)
    {
        AudioClip clip = GetClipByName(clipName);
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.volume = Mathf.Clamp01(volume); // Asegura que esté entre 0 y 1
            musicSource.loop = true;
            musicSource.PlayDelayed(delayInSeconds); // Retraso opcional
        }
        else
        {
            Debug.LogWarning($"Clip '{clipName}' no encontrado o AudioSource no asignado.");
        }
    }

    // Detener música
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Buscar clip por nombre
    private AudioClip GetClipByName(string clipName)
    {
        foreach (AudioClip clip in soundClips)
        {
            if (clip.name == clipName)
                return clip;
        }
        Debug.LogWarning($"Clip '{clipName}' no encontrado.");


        return null;
    }

    public void AssignButtonClickSounds()
    {
        // Encuentra todos los botones ahora que todos los objetos están activos
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button button in buttons)
        {            
            GameObject parent = button.gameObject;

            button.onClick.AddListener(() => PlaySFX("click"));       
        }

    }

    public void AssignMusicScene(int sceneIndex){
        float delayInSeconds = 0f;

        switch(sceneIndex){
            case 0: //MainMenu
                PlayMusic("ambient", 0.8f);
                break;

            case 1: //CharacterSelection
                break;
            
            case 2: //Musica lvl 1
                delayInSeconds = 2.1f; // Retraso de x segundos
                PlayMusic("lvl1music", 0.4f, delayInSeconds);
                break;
            
            case 3: //Musica lvl 2
                delayInSeconds = 2f; // Retraso de x segundos
                PlayMusic("lvl2music", 0.4f, delayInSeconds);
                break;
            
            default:
                break;
        }
    }


    private void OnDestroy() //Se ejecuta cuando el objeto es destruido (por si acaso)
    {
        // Desuscribirse del evento para evitar errores
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignButtonClickSounds(); // Asigna sonidos a los botones de la nueva escena

        AssignMusicScene(scene.buildIndex); // Asigna música de fondo según la escena
    }

}
