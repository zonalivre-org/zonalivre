using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CleanNinja : MonoBehaviour
{
    [Header("Rules")]
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private float coolDownBetweenObjects;

    [Header("Objects Properties")]
    public float maxSpeed;
    public float minSpeed;
    public float minXRange;
    public float maxXRange;
    public float yPosition;

    [Header("Variables")]
    private bool canGenerate = false;
    private float actualTime;
    private float nextTime;

    [Header("Components")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] private Transform zPosition;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;


    void Update()
    {
        if (canGenerate)
        {
            if (actualTime < nextTime)
            {
                actualTime = Time.time;
            }
            else 
            {
                SpawnObject();
                nextTime = Time.time + coolDownBetweenObjects;
            }
        }
    }

    public void StartCleanNinja()
    {
        startPanel.SetActive(false);
        canGenerate = true;
    }

    private void SpawnObject()
    {
        int randomIndex = Random.Range(0, objects.Count);
        GameObject obj = Instantiate(objects[randomIndex]);

        obj.transform.SetParent(this.transform, false);
        obj.GetComponent<JumpObjects>().cleanNinja = this;
        obj.GetComponent<JumpObjects>().zPos = zPosition.position.z;
    }
}
