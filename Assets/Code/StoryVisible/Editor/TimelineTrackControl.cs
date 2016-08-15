using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace xxstoryEditor
{

    public class TimelineTrackControl : SidebarControl
    {
        public class TrackStyles
        {
            public GUIStyle TrackAreaStyle;

            public GUIStyle backgroundSelected;

            public GUIStyle backgroundContentSelected;

            public GUIStyle TrackSidebarBG1;

            public GUIStyle TrackSidebarBG2;

            public GUIStyle TrackItemStyle;

            public GUIStyle AudioTrackItemStyle;

            public GUIStyle ShotTrackItemStyle;

            public GUIStyle GlobalTrackItemStyle;

            public GUIStyle ActorTrackItemStyle;

            public GUIStyle CurveTrackItemStyle;

            public GUIStyle TrackItemSelectedStyle;

            public GUIStyle AudioTrackItemSelectedStyle;

            public GUIStyle ShotTrackItemSelectedStyle;

            public GUIStyle GlobalTrackItemSelectedStyle;

            public GUIStyle ActorTrackItemSelectedStyle;

            public GUIStyle CurveTrackItemSelectedStyle;

            public GUIStyle keyframeStyle;

            public GUIStyle keyframeContextStyle;

            public GUIStyle curveStyle;

            public GUIStyle tangentStyle;

            public GUIStyle curveCanvasStyle;

            public GUIStyle compressStyle;

            public GUIStyle expandStyle;

            public GUIStyle editCurveItemStyle;

            public GUIStyle EventItemStyle;

            public GUIStyle EventItemBottomStyle;

            public TrackStyles(GUISkin skin)
            {
                this.TrackAreaStyle = skin.FindStyle("Track Area");
                this.TrackItemStyle = skin.FindStyle("Track Item");
                this.TrackItemSelectedStyle = skin.FindStyle("TrackItemSelected");
                this.ShotTrackItemStyle = skin.FindStyle("ShotTrackItem");
                this.ShotTrackItemSelectedStyle = skin.FindStyle("ShotTrackItemSelected");
                this.AudioTrackItemStyle = skin.FindStyle("AudioTrackItem");
                this.AudioTrackItemSelectedStyle = skin.FindStyle("AudioTrackItemSelected");
                this.GlobalTrackItemStyle = skin.FindStyle("GlobalTrackItem");
                this.GlobalTrackItemSelectedStyle = skin.FindStyle("GlobalTrackItemSelected");
                this.ActorTrackItemStyle = skin.FindStyle("ActorTrackItem");
                this.ActorTrackItemSelectedStyle = skin.FindStyle("ActorTrackItemSelected");
                this.CurveTrackItemStyle = skin.FindStyle("CurveTrackItem");
                this.CurveTrackItemSelectedStyle = skin.FindStyle("CurveTrackItemSelected");
                this.keyframeStyle = skin.FindStyle("Keyframe");
                this.curveStyle = skin.FindStyle("Curve");
                this.tangentStyle = skin.FindStyle("TangentHandle");
                this.curveCanvasStyle = skin.FindStyle("CurveCanvas");
                this.compressStyle = skin.FindStyle("CompressVertical");
                this.expandStyle = skin.FindStyle("ExpandVertical");
                this.editCurveItemStyle = skin.FindStyle("EditCurveItem");
                this.EventItemStyle = skin.FindStyle("EventItem");
                this.EventItemBottomStyle = skin.FindStyle("EventItemBottom");
                this.keyframeContextStyle = skin.FindStyle("KeyframeContext");
                this.TrackSidebarBG1 = skin.FindStyle("TrackSidebarBG");
                this.TrackSidebarBG2 = skin.FindStyle("TrackSidebarBGAlt");
                this.backgroundSelected = skin.FindStyle("TrackGroupFocused");
                this.backgroundContentSelected = skin.FindStyle("TrackGroupContentFocused");
            }
        }

        public static TimelineTrackControl.TrackStyles styles;

        internal static void InitStyles(GUISkin skin)
        {
            if (TimelineTrackControl.styles == null)
            {
                TimelineTrackControl.styles = new TimelineTrackControl.TrackStyles(skin);
            }
        }
    }
}
