using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectibleCount : MonoBehaviour
{
    TMPro.TMP_Text text;
    int count;


    //when the script is called check how many collectibles there are so the count matches
    private void Start() => UpdateCount();


    //get the text component
    private void Awake()
    {
       text = GetComponent<TMPro.TMP_Text>();
    }
    
    private void OnEnable() => collectible.onCollected += OnCollectibleCollected;
    private void OnDisable() => collectible.onCollected -= OnCollectibleCollected;


    //if the collectble is collected up the count by 1 and recheck the total collectibles
    public void OnCollectibleCollected()
    {
        count++;
        UpdateCount();
    }


    //write the amount that the player collected and how many that are left
    public void UpdateCount()
    {
        text.text = $"{count} / {collectible.total}";
    }
}
