using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinPerson : MonoBehaviour
{
    public SpriteRenderer
        head,
        body,
        under,
        leftHand,
        rightHand,
        leftLeg,
        rightLeg;

    public void SetColor(Color color)
    {
        SetColor(head, color);
        SetColor(body, color);
        SetColor(under, color);
        SetColor(leftHand, color);
        SetColor(rightHand, color);
        SetColor(leftLeg, color);
        SetColor(rightLeg, color);
    }

    private void SetColor(SpriteRenderer sr, Color color)
    {
        sr.color = color;
    }
}
