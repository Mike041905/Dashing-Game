using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mike
{
    public static class MikeColorTransitionExtention
    {
        static List<MikeColorTransition> activeTransitions = new List<MikeColorTransition>();
        public static void StartColorTransion(this Graphic graphic, Color newColor, float transitionSpeed, UnityAction onFinish = null)
        {
            if (graphic == null) { return; }

            for (int i = 0; i < activeTransitions.Count; i++)
            {
                if (activeTransitions[i].graphic == graphic) { activeTransitions[i].Stop(); activeTransitions.RemoveAt(i); }
            }

            activeTransitions.Add(new MikeColorTransition(graphic, newColor, transitionSpeed, onFinish));
        }

        public static void RemoveActiveTransition(MikeColorTransition transition) => activeTransitions.Remove(transition);
    }

    public class MikeColorTransition
    {
        public Graphic graphic;
        Coroutine transition;

        public MikeColorTransition(Graphic graphic, Color newColor, float transitionSpeed, UnityAction onFinish = null)
        {
            this.graphic = graphic;
            transition = graphic.StartCoroutine(ColorTransition(newColor, transitionSpeed, onFinish));
        }

        IEnumerator ColorTransition(Color newColor, float transitionSpeed, UnityAction onFinish)
        {
            if (graphic == null) { yield break; }

            Color startColor = graphic.color;
            float t = 0;

            while (graphic.color != newColor)
            {
                if (graphic == null) { yield break; }

                t += 1 / transitionSpeed * Time.deltaTime;
                graphic.color = Color.Lerp(startColor, newColor, t);
                yield return null;
            }

            MikeColorTransitionExtention.RemoveActiveTransition(this);

            onFinish?.Invoke();
        }

        public void Stop()
        {
            if(transition == null) { return; }
            graphic.StopCoroutine(transition);
        }
    }
}