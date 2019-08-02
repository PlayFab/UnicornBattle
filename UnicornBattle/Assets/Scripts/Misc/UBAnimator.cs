using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UBAnimator 
{
    #region UI Animation
    /// <summary>
    /// Wait the specified time and callback to method.
    /// </summary>
    /// <param name="time">Time.</param>
    /// <param name="callback">Callback.</param>
    public static IEnumerator Wait(float time, UnityAction callback) {
        yield return new WaitForSeconds(time);
        callback();
    }

    /// <summary>
    /// Intros the pane.
    /// </summary>
    /// <param name="obj">Object - object to animate in</param>
    /// <param name="duration">Duration -   how long is the transition</param>
    /// <param name="callback">Callback - method to call after the animation is complete </param>
    public static void IntroPane(GameObject obj, float duration, UnityAction callback = null) {
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg != null) {
            cg.blocksRaycasts = true;
            TweenCGAlpha.Tween(obj, duration, 1, callback);
        } else {
            // will add a cg automatically
            TweenCGAlpha.Tween(obj, duration, 1, callback);
        }
    }

    /// <summary>
    /// Outros the pane.
    /// </summary>
    /// <param name="obj">Object - object to animate out</param>
    /// <param name="duration">Duration -   how long is the transition</param>
    /// <param name="callback">Callback - method to call after the animation is complete </param>
    public static void OutroPane(GameObject obj, float duration, UnityAction callback = null) {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg != null) {
            cg.blocksRaycasts = false;
            TweenCGAlpha.Tween(obj, duration, 0, callback);
        } else {
            // will add a cg automatically
            TweenCGAlpha.Tween(obj, duration, 0, callback);
        }
    }
    #endregion

}