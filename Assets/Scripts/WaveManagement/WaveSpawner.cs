using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public int waveGold = 100;            // cat "gold" are wave-ul
    public int costPerCube = 5;           // cat costa un cub
    public int goldIncreasePerWave = 0;  // cu cat creste bugetul la fiecare wave nou
    
    public Transform spawnPoint;           
    //public float spawnRadius = 3f;        // Testare Wave ca cerc
    public GameObject enemyPrefab;        

    private int currentWave = 0;

    void Start()
    {
        SpawnWave();
    }

    // Click dreapta pe componenta in Inspector -> "Spawn Wave" ca sa testezi in Play Mode
    [ContextMenu("Spawn Wave")]
    public void SpawnWave()
    {
        GameObject waveParent = new GameObject("Wave" + currentWave);
        currentWave++;
        int gold = waveGold + (currentWave - 1) * goldIncreasePerWave; //gold pe wave-ul curent
        int cubeCount = gold / costPerCube;//Nr unituri alocate
        //TO DO: extindere pe mai multe clase de unituri cu Preturi diferite.

        // Setare punct implicita
        //Vector3 center = spawnPoint != null ? spawnPoint.position : transform.position;
        
        Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
        
        Debug.Log("Wave " + currentWave + ": " + gold + " gold -> " + cubeCount + " cuburi");

        //Cube Spawning

        float DistGrupuri = 4f; //Distanta intre formatiuni 
        float DistUnitati = 1.5f;
        
        Vector2 offset = new Vector2(0.0f, 0.0f);
        for (int i = 0; i < cubeCount; i++)  
        {
            int group = i / 4;          // nr grupului (cate 4)
            int inGroup = i % 4;        // pozitia pe grid(al catelea este)
            
            Transform groupParent;
            if (inGroup == 0)
            {
                GameObject g = new GameObject("Group" + group);
                g.transform.SetParent(waveParent.transform);
                groupParent = g.transform;
            }
            else
            {
                groupParent = waveParent.transform.GetChild(group);   // grupul deja creat
            }

            float x = group * DistGrupuri + (inGroup % 2) * DistUnitati;   
            float z = (inGroup / 2) * 1.5f;//0 fata 1 spate(relativ momentan)
            Vector3 position = center + new Vector3(x, 0.5f, z);

            SpawnUnit(position, groupParent);
        }
    }
    //Instantierea Uniturilor
    void SpawnUnit(Vector3 position, Transform parent)
    {
        GameObject unit;

        if (enemyPrefab != null)
        {
            unit = Instantiate(enemyPrefab, position, Quaternion.identity);
        }
        else
        {
            unit = GameObject.CreatePrimitive(PrimitiveType.Cube);
            unit.transform.position = position;
        }
        unit.transform.SetParent(parent);
        unit.name = "Enemy";
    }
}