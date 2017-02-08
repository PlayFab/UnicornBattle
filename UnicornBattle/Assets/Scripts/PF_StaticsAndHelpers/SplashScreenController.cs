using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour
{
    public enum SplashScreenMode { UseDarkLogo, UseLightLogo };
    public SplashScreenMode activeMode = SplashScreenMode.UseLightLogo;
    public string sceneToLoadNext = string.Empty;
    public Image darkImg;
    public Image lightImg;
    public Image hideFire;
    public float waitTime = 3.0f;
    public float fadeInTime = .5f;
    public float fadeOutTime = .5f;
    public Text countDown;
    private float _startTime = 0;

    // Use this for initialization
    IEnumerator Start()
    {
        hideFire.CrossFadeAlpha(1, .01f * .5f, true);
        if (activeMode == SplashScreenMode.UseDarkLogo)
        {
            Camera.main.backgroundColor = Color.white;
            darkImg.CrossFadeAlpha(0, .01f, true);
            darkImg.enabled = true;
            lightImg.enabled = false;

            hideFire.color = Color.white;
        }
        else
        {
            Camera.main.backgroundColor = Color.black;
            lightImg.CrossFadeAlpha(0, .01f, true);
            darkImg.enabled = false;
            lightImg.enabled = true;
            hideFire.color = Color.black;
        }

        yield return new WaitForSeconds(.333f);
        StartCoroutine(Fade());
    }

    // Update is called once per frame
    void Update()
    {
        countDown.text = string.Format("{0:0.00}", (_startTime + waitTime) - Time.time);
    }

    public IEnumerator Fade()
    {
        _startTime = Time.time;

        if (activeMode == SplashScreenMode.UseDarkLogo)
        {
            darkImg.CrossFadeAlpha(1, fadeInTime, true);

            yield return new WaitForSeconds(fadeInTime * 2);
            hideFire.CrossFadeAlpha(0, fadeInTime * .5f, true);

            while (_startTime + waitTime > Time.time)
            {
                yield return new WaitForEndOfFrame();
            }

            darkImg.CrossFadeAlpha(0, fadeOutTime, true);
            yield return new WaitForSeconds(fadeOutTime);
        }
        else
        {
            lightImg.CrossFadeAlpha(1, fadeInTime, true);

            yield return new WaitForSeconds(fadeInTime * 2);
            hideFire.CrossFadeAlpha(0, fadeInTime * .5f, true);

            while (_startTime + waitTime > Time.time)
                yield return new WaitForEndOfFrame();

            lightImg.CrossFadeAlpha(0, fadeOutTime, true);
            yield return new WaitForSeconds(fadeOutTime + .5f);
        }

        if (string.IsNullOrEmpty(sceneToLoadNext))
            Debug.LogError("Tried to load the next scene, but no scene was found. Enter your \"sceneToLoadNext'\" via the inspector.");
        else
            SceneManager.LoadSceneAsync(sceneToLoadNext);
    }
}
