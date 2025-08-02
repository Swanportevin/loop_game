using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject zombiePrefab;       // Reference to the zombie prefab
    public int maxZombies = 10;           // Max zombies to spawn
    public float spawnAreaSize = 1000f;    // Size of the spawn area
    public GameObject PlayerOrigin;       // Player reference for offset
    public GameObject ParentObject;       // Parent object for spawned zombies
    public int zombieCount = 0;
    private int instanceCount = 0;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI GameOverText;
    public int PlayerHealth = 10;          // Player health
    public int Score = 0;
    public Image[] Hearts;

    void Start()
    {
        StartCoroutine(SpawnZombiesLoop());
    }

    void Update()
    {
        ScoreText.text = "Zombie Kill: " + Score.ToString();
        if (PlayerHealth <= 0)
        {
            GameOver();
        }
    }

    private IEnumerator SpawnZombiesLoop()
    {
        while (zombieCount < maxZombies)
        {
            Vector3 spawnPosition = GetRandomPositionInArea();
            GameObject obj = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity, ParentObject.transform);
            obj.name = "ZombieNumber" + instanceCount.ToString();
            zombieCount++;
            instanceCount++;
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f)); // Wait before spawning the next one
        }
    }

    Vector3 GetRandomPositionInArea()
    {
        float halfSize = spawnAreaSize / 2f;
        float x = UnityEngine.Random.Range(-halfSize, halfSize);
        float z = UnityEngine.Random.Range(-halfSize, halfSize);
        float y = 0f; // Slightly above ground

        return PlayerOrigin.transform.position + new Vector3(x, y, z);
    }

    private void GameOver()
    {
        GameOverText.text = "Game Over! Final Score: " + Score.ToString();
        GameOverText.gameObject.SetActive(true);
        Time.timeScale = 0; // Stop the game
    }

    public void UpdateHearts()
    {
        PlayerHealth -= 1;
        Debug.Log("Updating Hearts. Current Player Health: " + PlayerHealth);
        string heartName = "EmptyHeart" + (10 - PlayerHealth).ToString();
        foreach (var heart in Hearts)
        {
            if (heart != null && heart.gameObject.name == heartName)
            {
                heart.gameObject.SetActive(true);
                break;
            }
        }
    }
}