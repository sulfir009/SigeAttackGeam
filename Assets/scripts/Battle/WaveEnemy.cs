using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveEnemy : MonoBehaviour
{
    //спавн мобов
    public GameObject enemyPrefab; //модель персонажа
    public int spawnEnemyCount = 1; // кількість ворогів
    public List<Transform> spawner; // спавнери
    public float timeToRespawn; //час до респавна юнітів
    public float exctraTimeSpawn = 10f; //час досрочного спавна юнітів
    private int enemyCount;
    //таймер
    public Text timerText;


    void Start()
    {
        SpawnEnemyWave(spawnEnemyCount);
        timerText.text = "Час до нової хвилі: " + exctraTimeSpawn.ToString();
    }

    void Update()
    {
        //кількість об'єктів з тегом Ворог
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = obj.Length;
        //рахується час до досрочного спавна
        exctraTimeSpawn -= Time.deltaTime;
        timerText.text = "Час до нової хвилі: " + Mathf.Round(exctraTimeSpawn).ToString();
        //якщо ворогів немає, або час закінчився
        if (enemyCount == 0 || exctraTimeSpawn < 0)
        {
            SpawnEnemyWave(spawnEnemyCount);
            //повертаємо час для досрочного спавну
            exctraTimeSpawn = 10f;
        }

    }
    //позиція спавну
    private Vector3 GeneralSpawnPosition(int indexSpawner)
    {

        float spawnPosX = Random.Range(-10, 10);
        float spawnPosZ = Random.Range(-10, 10);

        Vector3 randomPos = new Vector3(spawner[indexSpawner].transform.position.x + spawnPosX,
            0, spawner[indexSpawner].transform.position.z + spawnPosZ);

        return randomPos;

    }
    //спавн ворогів
    private void SpawnEnemyWave(int enemiesToSpawn)
    {

        for (int j = 0; j < spawner.Count; j++)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                Instantiate(enemyPrefab, GeneralSpawnPosition(i), Quaternion.identity);
            }
        }
    }
}