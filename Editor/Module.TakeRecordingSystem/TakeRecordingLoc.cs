using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MWU.FilmLib
{


    public static class TakeRecordingLoc
    {
        public const string WINDOW_TITLE = "Take System";
        public const string WINDOW_MAINDESCRIPTION = "Multi-take recording system.";

        // timeline management
        public const string TIMELINE_CREATENEWMASTER = "Create";
        public const string TIMELINE_CREATENEWBEAT = "Add";
        public const string TIMELINE_REFRESHTIMELINES = "Refresh";

        // tooltips
        public const string TOOLTIP_CREATENEWMASTER = "Create a new Master Timeline in the current scene";
        public const string TOOLTIP_CREATENEWBEAT = "Create a new sub-timeline beat in the current scene";
        public const string TOOLTIP_REFRESHTIMELINES = "Refresh timeline list";
    }
}