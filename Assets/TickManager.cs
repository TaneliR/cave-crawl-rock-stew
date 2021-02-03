using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    public class OnTickEventArgs : EventArgs {
        public int tick;
    }

    public static event EventHandler<OnTickEventArgs> OnTick;
    private int tick;

    void Awake() {
        tick = 0;
    }
    public void Act(int duration) {
        tick += duration;
        for(int i = 0; i < duration; i++) {
            if (OnTick != null) OnTick(this, new OnTickEventArgs{ tick = tick });
        }
    }
}
