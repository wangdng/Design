using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventManager
{
    public Dictionary<string, AnimEventBase> animEvents;

    public List<string> EventNames;

    public AnimEventManager()
    {
        EventNames= new List<string>();
        animEvents = new Dictionary<string, AnimEventBase>();
    }

    public void RegisterEvents(Dictionary<string, AnimEventBase> tmpEvents)
    {
        if (animEvents.Count > 0)
            return;
        foreach (KeyValuePair<string, AnimEventBase> tmpkvp in tmpEvents)
        {
            animEvents.Add(tmpkvp.Key, tmpkvp.Value);
            EventNames.Add(tmpkvp.Key);
        }
    }

    public AnimEventBase GetAnimEvent(string eventName)
    {
        if (animEvents.ContainsKey(eventName))
            return animEvents[eventName];

        return null;
    }

    void RemoveEvents(List<string> CompleteEnevts)
    {
        for (int i = 0; i < CompleteEnevts.Count; i++)
        {
            if (EventNames.Contains(CompleteEnevts[i]))
            {
                EventNames.Remove(CompleteEnevts[i]);
                animEvents.Remove(CompleteEnevts[i]);
            }
        }
    }

    public void OnUpdate()
    {
        List<string> CompleteEnevts = new List<string>();
        if (animEvents.Count > 0)
        {
            for (int i = 0; i < EventNames.Count; i++)
            {
                AnimEventBase curEvent = animEvents[EventNames[i]];

                if (curEvent.Execute(Time.deltaTime))
                {
                    CompleteEnevts.Add(EventNames[i]);
                }
            }
        }

        RemoveEvents(CompleteEnevts);
    }

    public void Dispose()
    {
        animEvents.Clear();
        EventNames.Clear();
    }
}
