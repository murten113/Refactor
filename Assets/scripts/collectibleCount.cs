using UnityEngine;

public class CollectibleCount : MonoBehaviour
{
    private TMPro.TMP_Text text;
    private int count;

    private void Awake()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    private void Start() => UpdateCount();

    private void OnEnable() => Collectible.OnCollected += OnCollectibleCollected;
    private void OnDisable() => Collectible.OnCollected -= OnCollectibleCollected;

    private void OnCollectibleCollected()
    {
        count++;
        UpdateCount();
    }

    private void UpdateCount()
    {
        text.text = $"{count} / {Collectible.Total}";
    }
}
