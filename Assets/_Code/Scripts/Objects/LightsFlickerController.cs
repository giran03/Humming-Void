using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsFlickerController : MonoBehaviour
{
    Light bulb;
    [Header("Configs")]
    [SerializeField] bool enableFlicker;
    [SerializeField] bool onOffBehaviour;
    [SerializeField] float minWaitTime;
    [SerializeField] float maxWaitTime;
    [SerializeField] float minLightIntensity;
    Material lightMat;
    float defaultIntensity;
    bool changeIntensity;
    bool turnOffBulb;

    private void Start()
    {
        bulb = GetComponent<Light>();
        lightMat = GetComponent<Renderer>().material;

        defaultIntensity = bulb.intensity;

        if (!enableFlicker) return;

        if (!onOffBehaviour)
            StartCoroutine(LightsFlickerIntensity());
        else
            StartCoroutine(LightsFlickerOnOff());
    }

    private void Update()
    {
        if (!turnOffBulb)
        {
            bulb.enabled = true;
            lightMat.SetColor("_EmissionColor", Color.white * 3f);
        }
        else
        {
            bulb.enabled = false;
            lightMat.SetColor("_EmissionColor", Color.white * 0f);
        }

        if (onOffBehaviour) return;

        if (!changeIntensity)
        {
            bulb.intensity = defaultIntensity;
            // lightMat.SetColor("_EmissiveColor", new Vector4(255, 255, 255, 3f));
            lightMat.SetColor("_EmissionColor", Color.white * 3f);
        }
        else
        {
            bulb.intensity = minLightIntensity;
            // lightMat.SetColor("_EmissiveColor", new Vector4(255, 255, 255, .5f));
            lightMat.SetColor("_EmissionColor", Color.white * 1f);
        }
    }

    IEnumerator LightsFlickerIntensity()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            changeIntensity = !changeIntensity;
        }
    }

    IEnumerator LightsFlickerOnOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            turnOffBulb = !turnOffBulb;
        }
    }
}
