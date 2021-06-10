using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawnManager : MonoBehaviour
{
    public GameManager gameManagerScript;
    public GameObject[] powerupPrefabs;
    public Transform[] spawnPositions;
    public List<Vector3> occupiedSpawnPositions;
    public float spawnXRange;
    public float spawnZRange;
    public float spawnWaitTime;
    public float spawnTime;
    public int powerupCountLimit;
    public int powerupCount;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        powerupCount = GameObject.FindGameObjectsWithTag("Powerup").Length;

        for (int i = powerupCount; i < powerupCountLimit; i++)
        {
            SpawnNewPowerup();
        }
        //SpawnNewPowerup();
    }

    // Update is called once per frame
    void Update()
    {
        //if(gameManagerScript.isGameActive)
        //{
        //powerupCount = GameObject.FindGameObjectsWithTag("Powerup").Length;

        //StartCoroutine(SpawnNewRandomPowerupRoutine());
        //}

    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnXRange, spawnXRange);
        float spawnPosZ = Random.Range(-spawnZRange, spawnZRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    private int PickSpawnPosition()
    {
        int pickedSpawnPosition = Random.Range(0, spawnPositions.Length);

        while (occupiedSpawnPositions.Contains(spawnPositions[pickedSpawnPosition].transform.position))
        {
            pickedSpawnPosition = Random.Range(0, spawnPositions.Length);
        }

        occupiedSpawnPositions.Add(spawnPositions[pickedSpawnPosition].transform.position);

        return pickedSpawnPosition;
    }

    private void SpawnNewPowerup()
    {
        powerupCount = GameObject.FindGameObjectsWithTag("Powerup").Length;
        int randomPowerup = Random.Range(0, powerupPrefabs.Length);


        if (powerupCount < powerupCountLimit)
        {
            Instantiate(powerupPrefabs[randomPowerup], spawnPositions[PickSpawnPosition()].transform.position, powerupPrefabs[randomPowerup].transform.rotation);
            powerupCount++;
        }
    }

    public IEnumerator SpawnNewRandomPowerupRoutine()
    {
        yield return new WaitForSeconds(spawnWaitTime);
        SpawnNewPowerup();
    }
}
