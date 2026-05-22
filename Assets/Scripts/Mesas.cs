using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Mesas : MonoBehaviour
{
    [SerializeField] private Transform tableParent;
    [SerializeField] private List<GameObject> tablePrefabs;
    [SerializeField] private float velocidad = 4f;
    [SerializeField] private int numTables = 5;
    [SerializeField] private bool canTablesMove = true;
        
    private readonly List<GameObject> _tables = new List<GameObject>();
    private const float TableWidth = 4f;
    private const float DistanceResposThreshold = -5f;

    private void Start()
    {
        for (int i = 0; i < numTables; i++)
        {
            Vector3 positionToSpawn = Vector3.zero;
            if (i > 0 && _tables[i-1] != null)
            {
                positionToSpawn = _tables[i-1].transform.position;
                positionToSpawn.z += TableWidth;
            }
            _tables.Add(InstantiateTable(tableParent, positionToSpawn));
        }
    }

    void Update()
    {
        if (canTablesMove)
        {
            for (int i = 0; i < _tables.Count; i++)
            {
                _tables[i].transform.Translate(Vector3.back * (velocidad * Time.deltaTime));
            }
        }
        
        //Checkeamos posiciones para reposicionar
        for (int i = 0; i < _tables.Count; i++)
        {
            if (_tables[i].transform.position.z < DistanceResposThreshold)
            {
                TableReposition(_tables[i]);
            }
        }
    }

    private GameObject InstantiateTable(Transform parent, Vector3 position)
    {
        Quaternion rotation = parent.transform.rotation;
        int randomIndex= Random.Range(0, tablePrefabs.Count);
        GameObject newTable = Instantiate(tablePrefabs[randomIndex], position, rotation, tableParent);
        
        return  newTable;
    }

    private void TableReposition(GameObject table)
    {
        _tables.Remove(table);
        Destroy(table.gameObject);
        
        Vector3 lastTablePosition = _tables.Last().transform.position;
        lastTablePosition += new Vector3(0,0,TableWidth);
        
        GameObject newTable = InstantiateTable(tableParent, lastTablePosition);
        
        _tables.Add(newTable);
    }
}
