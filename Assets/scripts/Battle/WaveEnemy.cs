using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveEnemy : MonoBehaviour
{
    //����� �����
    public GameObject enemyPrefab; //������ ���������
    public int spawnEnemyCount = 1; // ������� ������
    public List<Transform> spawner; // ��������
    public float timeToRespawn; //��� �� �������� ����
    public float exctraTimeSpawn = 10f; //��� ���������� ������ ����
    private int enemyCount;
    //������
    public Text timerText;


    void Start()
    {
        SpawnEnemyWave(spawnEnemyCount);
        timerText.text = "��� �� ���� ����: " + exctraTimeSpawn.ToString();
    }

    void Update()
    {
        //������� ��'���� � ����� �����
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = obj.Length;
        //�������� ��� �� ���������� ������
        exctraTimeSpawn -= Time.deltaTime;
        timerText.text = "��� �� ���� ����: " + Mathf.Round(exctraTimeSpawn).ToString();
        //���� ������ ����, ��� ��� ���������
        if (enemyCount == 0 || exctraTimeSpawn < 0)
        {
            SpawnEnemyWave(spawnEnemyCount);
            //��������� ��� ��� ���������� ������
            exctraTimeSpawn = 10f;
        }

    }
    //������� ������
    private Vector3 GeneralSpawnPosition(int indexSpawner)
    {

        float spawnPosX = Random.Range(-10, 10);
        float spawnPosZ = Random.Range(-10, 10);

        Vector3 randomPos = new Vector3(spawner[indexSpawner].transform.position.x + spawnPosX,
            0, spawner[indexSpawner].transform.position.z + spawnPosZ);

        return randomPos;

    }
    //����� ������
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