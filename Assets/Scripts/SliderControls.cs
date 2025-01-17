using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class SliderControls : MonoBehaviour
{
    public Slider progressBar; // Reference to the Slider
    //public Button startButton; // Reference to the Button
    public Image sliderFillImage; // Reference to the Slider's fill area
    public float decreaseDuration = 10f; 

    private Coroutine sliderCoroutine;

   
    internal void StartSliderBehavior()
    {
        // If a slider coroutine is already running, stop it
        if (sliderCoroutine != null)
        {
            StopCoroutine(sliderCoroutine);
        }
        // Start the slider behavior coroutine
        sliderCoroutine = StartCoroutine(SliderBehavior());
    }

    private IEnumerator SliderBehavior()
    {
        // Step 1: Fill the slider instantly and set it to green
        progressBar.value = 1f;
        sliderFillImage.color = Color.green;

        // Step 2: Start decreasing the slider
        float elapsedTime = 0f;
        float initialValue = progressBar.value;

        while (elapsedTime < decreaseDuration)
        {
            elapsedTime += Time.deltaTime;
            progressBar.value = Mathf.Lerp(initialValue, 0f, elapsedTime / decreaseDuration);

            // Step 3: Change color to red when below 25%
            if (progressBar.value <= 0.25f)
            {
                sliderFillImage.color = Color.red;
            }

            yield return null; // Wait for the next frame
        }

        UI_Manager.Instance.isSuperXpEnable = false;
        // Step 4: Slider is empty, hide it
        gameObject.SetActive(false);
    }
}


