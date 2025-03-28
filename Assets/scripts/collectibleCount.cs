using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectibleCount : MonoBehaviour
{
    TMPro.TMP_Text text;
    int count;


    private void Start() => UpdateCount();

    private void Awake()
    {
       text = GetComponent<TMPro.TMP_Text>();
    }

    private void OnEnable() => collectible.OnCollected += OnCollectibleCollected;
    private void OnDisable() => collectible.OnCollected -= OnCollectibleCollected;

    public void OnCollectibleCollected()
    {
        count++;
        UpdateCount();
    }

    public void UpdateCount()
    {
        text.text = $"{count} / {collectible.total}";
    }
}
