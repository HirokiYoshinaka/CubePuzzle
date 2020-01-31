using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGMの再生、シーンの終了などを制御します。
/// </summary>
public class SceneController : MonoBehaviour
{
    AudioSource audioSource = null;
    AudioClip BGM = null;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        BGM = Resources.Load("Sounds/spring-mountain1") as AudioClip;
        audioSource.clip = BGM;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // ESCキーが押下されたときアプリケーションを終了します。
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
        }
    }
}
