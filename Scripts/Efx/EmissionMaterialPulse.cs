using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionMaterialPulse : MonoBehaviour
{
    [SerializeField] private Renderer emittingRenderer;
    [SerializeField] private int materialIdx;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float pulsateSpeed;
    [SerializeField] private float pulsateMaxDistance;
    [SerializeField] private bool randomSpeed;

    private Material emitMaterial;
    private Color baseColor;
    private Color finalColor;
    private float changeSpeed;
    private float emission;

    private void Awake()
    {
        emitMaterial = emittingRenderer.materials[materialIdx];
        baseColor = emitMaterial.GetColor("_EmissionColor");
        changeSpeed = 15.0f;

        if (randomSpeed)
            GetRandomSpeed();
    }

    private void Update()
    {
        emission = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * pulsateSpeed, pulsateMaxDistance));
        finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        emitMaterial.SetColor("_EmissionColor", finalColor);

        changeSpeed -= Time.deltaTime;

        if(changeSpeed <= 0.0f)
        {
            changeSpeed = 15.0f;
            GetRandomSpeed();
        }
    }

    private void GetRandomSpeed()
    {
        pulsateSpeed = Random.Range(0.5f, 4.0f);
    }
}
