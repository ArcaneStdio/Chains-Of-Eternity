using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] AudioResource bgm;
    [SerializeField] AudioResource levelUp;
    [SerializeField] AudioResource Hit;

    private void Awake()
    {
        // Enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void playAudio(AudioResource resource)
    {
        audioSource.resource = resource;
        audioSource.Play();
    }

    public void PlayBGM()
    {
        playAudio(bgm);
    }

    public void PlayLevelUp()
    {
        playAudio(levelUp);
    }

    public void PlayHit()
    {
        playAudio(Hit);
    }

}
