using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCheckpoint : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public TextMeshProUGUI checkpointMessage;
    public float checkpointMessageTime;
    private float messageTimePassed;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        messageTimePassed = 0;
    }

    void Update()
    {
        if (checkpointMessage.gameObject.activeSelf)
        {
            messageTimePassed += Time.deltaTime;
            if (messageTimePassed > checkpointMessageTime)
            {
                checkpointMessage.gameObject.SetActive(false);
                messageTimePassed = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Checkpoint")
        {
            playerMovement.SetCheckpoint(transform, col.gameObject.GetComponent<Checkpoint>().id);
            Destroy(col.gameObject);
            checkpointMessage.gameObject.SetActive(true);
        }
    }
}
