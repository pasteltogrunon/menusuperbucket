using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    [SerializeField] HealthManager playerHealth;
    [SerializeField] GameObject lifeImage;
    [SerializeField] Vector2 startPos;
    [SerializeField] Vector2 displacement;
    [SerializeField] Vector2 randomDisplacement;

    List<GameObject> lifeObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        playerHealth.onHealthChanged += updateHealth;
    }

    // Update is called once per frame
    void Update()
    {
        updateMaxHealth();
    }

    public void updateMaxHealth()
    {
        int lifes = playerHealth.MaxHealth / 5;
        for(int i = lifeObjects.Count; i <lifes; i++)
        {
            lifeObjects.Add(Instantiate(lifeImage, startPos + i * displacement + Random.Range(-0.5f, 0.5f) * randomDisplacement, Quaternion.identity, transform));
        }
    }

    public void updateHealth()
    {
        int lifes = playerHealth.Health / 5;
        for (int i = 0; i < lifeObjects.Count; i++)
        {
            lifeObjects[i].transform.GetChild(0).gameObject.SetActive(i < lifes);
        }
    }
}
