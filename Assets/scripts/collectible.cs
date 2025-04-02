using System;
using UnityEngine;

public class collectible : MonoBehaviour
{
    public static event Action onCollected;
    public static int total;

    //when the script starts the total count gets increased by one so when you have 10 collectbles with this script the total = 10
    private void Awake() => total++;


  
    private void Update()
    {
        RotateItem();
    }

    //make the collectible rotate to show the player its interacable
    private void RotateItem()
    {
        transform.localRotation = Quaternion.Euler(90f, Time.time * 100f, 0);

    }


    //when the player moves into its hit box invoke the onCollected func and destroy the game object so the player cant pick this collectible up multiple times
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            onCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}

