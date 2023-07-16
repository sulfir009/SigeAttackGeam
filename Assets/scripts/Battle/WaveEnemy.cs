using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveEnemy : MonoBehaviour
{
    //����� �����
    public GameObject[] enemyPrefab = new GameObject[10]; //������ ���������
    public int spawnEnemyCount = 1; // ������� ������
    public List<Transform> spawner; // ��������
    public int typeEnemys = 0;
    
    public float exctraTimeSpawn = 10f; //��� ���������� ������ ����
    private int enemyCount;
    //������
    public Text timerText;

    private int waveNumber = 1;
    public MobStrengthController mobStrengthController_Skeleton;

    //test
    private int firstTypeCount;
    private bool spawned = true;

    void Start()
    {
        SpawnEnemyWave(spawnEnemyCount);
        timerText.text = "��� �� ���� ����: " + exctraTimeSpawn.ToString();
        firstTypeCount = spawnEnemyCount -1;
        secondTypeCount = 1;
        mobStrengthController_Skeleton = enemyPrefab[0].GetComponent<MobStrengthController>();
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
        if ((enemyCount == 0 || exctraTimeSpawn < 0) && spawned)
        {
            spawned = false;
            //��������� ���� ������
            waveNumber += 1;
            mobStrengthController_Skeleton.SetStrength(waveNumber * 5);

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
            1, spawner[indexSpawner].transform.position.z + spawnPosZ);

        return randomPos;

    }
    //����� ������
    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        if (waveNumber % 5 == 0) { typeEnemys++; firstTypeCount = spawnEnemyCount - 2; }
        if (typeEnemys == enemyPrefab.Length) typeEnemys = enemyPrefab.Length;
        for (int j = 0; j < spawner.Count; j++)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                if (typeEnemys == 0)
                {
                    Instantiate(enemyPrefab[0], GeneralSpawnPosition(j), Quaternion.identity);
                }
                else if (typeEnemys < enemyPrefab.Length)
                {
                    if (i > firstTypeCount)
                    {
                        Instantiate(enemyPrefab[typeEnemys], GeneralSpawnPosition(j), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(enemyPrefab[typeEnemys - 1], GeneralSpawnPosition(j), Quaternion.identity);
                    }
                }
                else {
                    Instantiate(enemyPrefab[typeEnemys - 1], GeneralSpawnPosition(j), Quaternion.identity);
                }
            }
        }

        firstTypeCount--;

        spawned = true;
    }
}


/*
 void fill(int arr[], int size) {
  for (int i = 0; i < size; i++) {
    arr[i] = i + 1;
  }
}

void main() {
  const int enemyCount = 5; //������� ����� ������
  int enemyArray[enemyCount];
  int maxArr[enemyCount] = { 10,8,8,8,10 };
  int maxEnemyCount = 10; //����������� ������� ������
  int nawEnemyCount; //������� ������ �����
  int waveNumber = 0; //����� ����

  int j = 0;

  fill(enemyArray, enemyCount);

  while (true) {
    waveNumber++;
    nawEnemyCount = 0;
    cout << "\n# " << waveNumber<<" : ";

    for (int k = 0; k <= j; k++) { //��� ������
      for (int i = 0; i < maxArr[k];i++) { //������� ������
        cout << enemyArray[k];
        nawEnemyCount++;
        if (nawEnemyCount >= maxEnemyCount) break; // ���� ����� �� ���� ���� �� ��������
      }
    }

    if (waveNumber % 5 == 0 && j<enemyCount - 1) j++;
      for (int i = 0; i < j; i++)
        maxArr[i]--;


    system("pause");
  }
}
 */