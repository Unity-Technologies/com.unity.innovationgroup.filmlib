using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MWU.FilmLib
{


    public static class TakeRecordingLoc
    {
        public const string WINDOW_TITLE = "Take System";
        public const string WINDOW_MAINDESCRIPTION = "Multi-take recording system. Create a new Timeline or select one from the list to configure.";

        // timeline management
        public const string TIMELINE_CURRENTSELECTION = "Current active Timeline:";
        public const string TIMELINE_CREATENEWMASTER = "Create";
        public const string TIMELINE_CREATENEWBEAT = "Add";
        public const string TIMELINE_REFRESHTIMELINES = "Refresh";

        // tooltips
        public const string TOOLTIP_CREATENEWMASTER = "Create a new Master Timeline in the current scene";
        public const string TOOLTIP_CREATENEWBEAT = "Create a new sub-timeline beat in the current scene";
        public const string TOOLTIP_REFRESHTIMELINES = "Refresh timeline list";

        // track tooltips
        public const string TOOLTIP_GROUPTRACK = "Group Track";
        public const string TOOLTIP_ANIMATIONTRACK = "Animation Track";
        public const string TOOLTIP_AUDIOTRACK = "Audio Track";
        public const string TOOLTIP_CONTROlTRACK = "Control Track";
        public const string TOOLTIP_CINEMACHINETRACK = "Cinemachine Track";
        public const string TOOLTIP_ACTIVATIONTRACK = "Activation Track";
        public const string TOOLTIP_UNKNOWNTRACK = "Custom / Unknown Track";

    }
}