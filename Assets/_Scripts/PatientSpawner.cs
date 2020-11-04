using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientSpawner : MonoBehaviour
{
    public GameObject patient;
    public int patientCount;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnPatient", 3);
    }

    void SpawnPatient()
    {
        Instantiate(patient, this.transform.position, Quaternion.identity);
        Invoke("SpawnPatient", Random.Range(5, 10));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
