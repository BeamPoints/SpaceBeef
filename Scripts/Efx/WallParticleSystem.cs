using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallParticleSystem : MonoBehaviour
{
    [SerializeField] private GameObject stageOne;
    [SerializeField] private GameObject stageTwo;
    [SerializeField] private GameObject stageThree;
    [SerializeField] private List<int> percentageStages;
    [SerializeField] private Health health;

    private ParticleSystem stageThreeParticle;
    private float onePercent;

    private void Awake()
    {
        stageThreeParticle = stageThree.GetComponent<ParticleSystem>();
        stageOne.SetActive(false);
        stageTwo.SetActive(false);
        stageThree.SetActive(false);
    }

    private void Start()
    {
        onePercent = (float)health.MaxHealth / 100.0f;
    }

    private void Update()
    {
        if (health == null || health.CurrentHealth <= 0)
        {
            if (stageThree != null)
                stageThree.SetActive(false);

            if (stageThreeParticle != null)
                stageThreeParticle.Stop(true);

            if (stageOne != null)
                Destroy(stageOne);

            if (stageTwo != null)
                Destroy(stageTwo);
        }

        if (health.CurrentHealth < onePercent * percentageStages[0] && health.CurrentHealth >= onePercent * percentageStages[1])
        {
            if(!stageOne.activeSelf)
                SetWallDamageStage(1);
        }
        else if (health.CurrentHealth < onePercent * percentageStages[1] && health.CurrentHealth >= onePercent * percentageStages[2])
        {
            if (!stageTwo.activeSelf)
                SetWallDamageStage(2);
        }
        else if (health.CurrentHealth < onePercent * percentageStages[2] && health.CurrentHealth > 0)
        {
            if (!stageThree.activeSelf)
                SetWallDamageStage(3);
        }
    }

    private void SetWallDamageStage(int _stage)
    {
        stageOne.SetActive(false);
        stageTwo.SetActive(false);
        stageThree.SetActive(false);

        switch (_stage)
        {
            case 1: stageOne.SetActive(true); stageOne.GetComponent<ParticleSystem>().Play(); break;
            case 2: stageTwo.SetActive(true); stageTwo.GetComponent<ParticleSystem>().Play(); break;
            case 3: stageThree.SetActive(true); stageThree.GetComponent<ParticleSystem>().Play(); break;
            default: break;
        }
    }
}
