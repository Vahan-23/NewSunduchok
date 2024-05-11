using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Drawing;
using Unity.VisualScripting;

public class TextCloud : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> circles;
    [SerializeField] private List<Vector3> circlesScale;

    private void Start()
    {
        for(int i = 0; i < circles.Count; i++)
        {
            circlesScale.Add(circles[i].transform.localScale);
            circles[i].transform.localScale = Vector3.zero;
        }

        foreach (var circle in circles)
        {
            circle.transform.DOMoveY(circle.transform.position.y + 0.3f, UnityEngine.Random.Range(0.8f, 1.5f)).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void anim1(int cardsNumber, UnityEngine.Color color)
    {
        var sequence = DOTween.Sequence();
        foreach (var circle in circles)
        {
            sequence.Append(circle.DOColor(color, 0.7f));

        }
    }
    public void anim2(int cardsNumber, bool appearing)
    {

        var sequence = DOTween.Sequence();
        for (int i = 0; i< circles.Count; i++)
        {
           // circles[i].transform.localScale = Vector3.zero;
            Vector3 targetScale = appearing ? circlesScale[i] : Vector3.zero;
            sequence.Append(circles[i].transform.DOScale(targetScale, 0.2f));

        }
    }
}
