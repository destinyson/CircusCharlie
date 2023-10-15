using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite pressSprite;

    public bool low;

    public void press()
    {
        GetComponent<SpriteRenderer>().sprite = pressSprite;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.08f);
        GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, 0.48f);
        Invoke("cover", 0.05f);
    }

    private void cover()
    {
        GetComponent<SpriteRenderer>().sprite = normalSprite;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f);
        GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, 0.64f);
    }
}
