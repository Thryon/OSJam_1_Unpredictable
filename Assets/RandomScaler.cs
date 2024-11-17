using UnityEngine;

public class RandomScaler : MonoBehaviour
{
    [SerializeField] Vector2 randomScaleMinMax = new Vector2(0.5f, 1f);
    
    void OnEnable()
    {
        transform.localScale = Vector3.one * Random.Range(randomScaleMinMax.x, randomScaleMinMax.y);
    }
}
