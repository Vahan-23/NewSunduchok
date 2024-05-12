using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Drawing;
using Unity.VisualScripting;

public class TextCloud : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> _circles;
    [SerializeField] private List<Vector3> _circlesScale;
    
    [SerializeField] private TextMeshProUGUI _necesaryCardText;

    private void Start()
    {
        for(int i = 0; i < _circles.Count; i++)
        {
            _circlesScale.Add(_circles[i].transform.localScale);
            _circles[i].transform.localScale = Vector3.zero;
        }

        foreach (var circle in _circles)
        {
            circle.transform.DOMoveY(circle.transform.position.y + 0.3f, UnityEngine.Random.Range(0.8f, 1.5f)).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void anim1(CardsNumber cardsNumber, UnityEngine.Color color)
    {
        var sequence = DOTween.Sequence();
        foreach (var circle in _circles)
        {
            sequence.Append(circle.DOColor(color, 0.7f));

        }
    }
    public void anim2(CardsNumber cardsNumber, bool appearing)
    {

        var sequence = DOTween.Sequence();
        for (int i = 0; i< _circles.Count; i++)
        {
           // _circles[i].transform.localScale = Vector3.zero;
            Vector3 targetScale = appearing ? _circlesScale[i] : Vector3.zero;
            sequence.Append(_circles[i].transform.DOScale(targetScale, 0.2f));
        }
        _necesaryCardText.text = cardsNumber.ToString();
    }
}
