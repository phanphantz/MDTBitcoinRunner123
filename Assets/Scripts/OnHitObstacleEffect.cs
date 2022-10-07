using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class OnHitObstacleEffect : MonoBehaviour 
{
    public SpriteRenderer spriteRenderer;

    public void Spawn()
    {
        var  originalPos = transform.position;
        var targetPosY = originalPos.y + 5;

        spriteRenderer.color = new Color(1,1,1,0f);

        transform.DOMoveY(targetPosY, 0.5f)
        .SetEase(Ease.OutBack)
        .OnComplete(PlayFadeOutAnimation);

        spriteRenderer.DOFade(1f, 0.25f)
        .SetEase(Ease.InSine);
    }

    void PlayFadeOutAnimation()
    {
        transform.DOScale(Vector3.zero , 1f).SetEase(Ease.OutExpo);
    }

}
