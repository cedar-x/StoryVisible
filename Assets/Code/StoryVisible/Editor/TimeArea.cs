﻿using System;
using UnityEditor;
using UnityEngine;
namespace xxstoryEditor
{
    public class TimeArea : ZoomableArea
    {
        private class TimeAreaStyle
        {
            public GUIStyle labelTickMarks = "CurveEditorLabelTickMarks";

            public GUIStyle TimelineTick = "AnimationTimelineTick";
        }

        private TickHandler horizontalTicks;

        private DirectorControlSettings m_Settings;

        private static TimeArea.TimeAreaStyle styles;

        internal TickHandler hTicks
        {
            get
            {
                return this.horizontalTicks;
            }
            set
            {
                this.horizontalTicks = value;
            }
        }

        internal DirectorControlSettings settings
        {
            get
            {
                return this.m_Settings;
            }
            set
            {
                if (value != null)
                {
                    this.m_Settings = value;
                    this.ApplySettings();
                }
            }
        }

        public TimeArea()
        {
            this.m_Settings = new DirectorControlSettings();
            float[] tickModulos = new float[]
		{
			0.0005f,
			0.001f,
			0.005f,
			0.01f,
			0.05f,
			0.1f,
			0.5f,
			1f,
			5f,
			10f,
			50f,
			100f,
			500f,
			1000f,
			5000f,
			10000f
		};
            this.hTicks = new TickHandler();
            this.hTicks.SetTickModulos(tickModulos);
        }

        private void ApplySettings()
        {
            base.hRangeLocked = this.settings.hRangeLocked;
            base.hRangeMin = this.settings.HorizontalRangeMin;
            base.hRangeMax = this.settings.hRangeMax;
            base.scaleWithWindow = this.settings.scaleWithWindow;
            base.hSlider = this.settings.hSlider;
        }

        public float GetMajorTickDistance(float frameRate)
        {
            float result = 0f;
            for (int i = 0; i < this.hTicks.tickLevels; i++)
            {
                if (this.hTicks.GetStrengthOfLevel(i) > 0.5f)
                {
                    return this.hTicks.GetPeriodOfLevel(i);
                }
            }
            return result;
        }

        public void DrawMajorTicks(Rect position, float frameRate)
        {
            Color color = Handles.color;
            GUI.BeginGroup(position);
            if ((int)Event.current.type != 7)
            {
                GUI.EndGroup();
                return;
            }
            TimeArea.InitStyles();
            this.SetTickMarkerRanges();
            this.hTicks.SetTickStrengths(3f, 80f, true);
            Color textColor = TimeArea.styles.TimelineTick.normal.textColor;
            textColor.a = 0.3f;
            Handles.color = (textColor);
            for (int i = 0; i < this.hTicks.tickLevels; i++)
            {
                float strengthOfLevel = this.hTicks.GetStrengthOfLevel(i);
                if (strengthOfLevel > 0.5f)
                {
                    float[] ticksAtLevel = this.hTicks.GetTicksAtLevel(i, true);
                    for (int j = 0; j < ticksAtLevel.Length; j++)
                    {
                        if (ticksAtLevel[j] >= 0f)
                        {
                            int num = Mathf.RoundToInt(ticksAtLevel[j] * frameRate);
                            float num2 = this.FrameToPixel((float)num, frameRate, position);
                            Handles.DrawLine(new Vector3(num2, 0f, 0f), new Vector3(num2, position.height, 0f));
                            if (strengthOfLevel > 0.8f)
                            {
                                Handles.DrawLine(new Vector3(num2 + 1f, 0f, 0f), new Vector3(num2 + 1f, position.height, 0f));
                            }
                        }
                    }
                }
            }
            GUI.EndGroup();
            Handles.color = (color);
        }

        public string FormatFrame(int frame, float frameRate)
        {
            int num = (int)frameRate;
            int length = num.ToString().Length;
            int num2 = frame / num;
            float num3 = (float)frame % frameRate;
            return string.Format("{0}:{1}", num2.ToString(), num3.ToString().PadLeft(length, '0'));
        }

        public float FrameToPixel(float i, float frameRate, Rect rect)
        {
            Rect shownArea = base.shownArea;
            return (i - shownArea.xMin * frameRate) * rect.width / (shownArea.width * frameRate);
        }

        private static void InitStyles()
        {
            if (TimeArea.styles == null)
            {
                TimeArea.styles = new TimeArea.TimeAreaStyle();
            }
        }

        public void SetTickMarkerRanges()
        {
            Rect shownArea = base.shownArea;
            this.hTicks.SetRanges(shownArea.xMin, shownArea.xMax, base.drawRect.xMin, base.drawRect.xMax);
        }

        public void TimeRuler(Rect position, float frameRate)
        {
            Color color = Handles.color;
            GUI.BeginGroup(position);
            // 		if ((int)Event.current.type != 7)
            // 		{
            // 			GUI.EndGroup();
            // 			return;
            // 		}
            TimeArea.InitStyles();
            this.SetTickMarkerRanges();
            this.hTicks.SetTickStrengths(3f, 80f, true);
            Color textColor = TimeArea.styles.TimelineTick.normal.textColor;
            textColor.a = 0.3f;
            Handles.color = (textColor);
            for (int i = 0; i < this.hTicks.tickLevels; i++)
            {
                float strengthOfLevel = this.hTicks.GetStrengthOfLevel(i);
                if (strengthOfLevel > 0.2f)
                {
                    float[] ticksAtLevel = this.hTicks.GetTicksAtLevel(i, true);
                    for (int j = 0; j < ticksAtLevel.Length; j++)
                    {
                        if (ticksAtLevel[j] >= base.hRangeMin && ticksAtLevel[j] <= base.hRangeMax)
                        {
                            int num = Mathf.RoundToInt(ticksAtLevel[j] * frameRate);
                            float num2 = position.height * Mathf.Min(1f, strengthOfLevel) * 0.7f;
                            float num3 = this.FrameToPixel((float)num, frameRate, position);
                            Handles.DrawLine(new Vector3(num3, position.height - num2 + 0.5f, 0f), new Vector3(num3, position.height - 0.5f, 0f));
                            if (strengthOfLevel > 0.5f)
                            {
                                Handles.DrawLine(new Vector3(num3 + 1f, position.height - num2 + 0.5f, 0f), new Vector3(num3 + 1f, position.height - 0.5f, 0f));
                            }
                        }
                    }
                }
            }
            GL.End();
            int levelWithMinSeparation = this.hTicks.GetLevelWithMinSeparation(40f);
            float[] ticksAtLevel2 = this.hTicks.GetTicksAtLevel(levelWithMinSeparation, false);
            for (int k = 0; k < ticksAtLevel2.Length; k++)
            {
                if (ticksAtLevel2[k] >= base.hRangeMin && ticksAtLevel2[k] <= base.hRangeMax)
                {
                    int num4 = Mathf.RoundToInt(ticksAtLevel2[k] * frameRate);
                    float arg_21E_0 = Mathf.Floor(this.FrameToPixel((float)num4, frameRate, base.rect));
                    string text = this.FormatFrame(num4, frameRate);
                    GUI.Label(new Rect(arg_21E_0 + 3f, -3f, 40f, 20f), text, TimeArea.styles.TimelineTick);
                }
            }
            GUI.EndGroup();
            Handles.color = (color);
        }
    }
}
