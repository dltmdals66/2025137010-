using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Fruit : MonoBehaviour
{
    [HideInInspector] public int fruitType;
    [HideInInspector] public FruitGame gameManager;
    bool _merged = false;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (_merged) return;

        Fruit otherF = other.gameObject.GetComponent<Fruit>();
        if (otherF != null
            && otherF.fruitType == fruitType
            && !otherF._merged)
        {
            _merged = otherF._merged = true;

            Vector3 mergePos = (transform.position + otherF.transform.position) * 0.5f;
            Destroy(gameObject);
            Destroy(otherF.gameObject);

            gameManager.SpawnMergedFruit(fruitType + 1, mergePos);
        }
    }
}
