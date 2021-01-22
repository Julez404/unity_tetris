using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelController : MonoBehaviour
{
    public GameObject pixelArray;

    public void SetPixel(int x, int y, Color color)
    {
        int id = GetIdByCoordinates(x, y);
        SetColorById(id, color);
    }

    private int GetIdByCoordinates(int x, int y)
    {
        return (10 * y) + x;
    }

    private void SetColorById(int id, Color color)
    {
        GameObject gameObject = pixelArray.transform.GetChild(id).gameObject;
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }
}
