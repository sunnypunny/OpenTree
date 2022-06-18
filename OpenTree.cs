
using System.Collections.Generic;
using UnityEngine;
using KSP.Localization;

namespace OpenTree {
    
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class OpenTreeAddon : MonoBehaviour {
        public void Start() {
            if (CareerOrSciMode()) {
                GameEvents.OnTechnologyResearched.Add(OnTechResearched);
                if (IsTechLocked("uncrewedTech") && IsTechLocked("crewedTech")) Popup(); }
        }
        public void OnDisable() { if (CareerOrSciMode()) GameEvents.OnTechnologyResearched.Remove(OnTechResearched); }
        private bool CareerOrSciMode() => HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX;
        private bool IsTechLocked(string techID) => ResearchAndDevelopment.Instance.GetTechState(techID) == null;
        private void UnlockTech(string techID) => ResearchAndDevelopment.Instance.UnlockProtoTechNode(new ProtoTechNode { techID = techID, scienceCost = 1 });
        private void OnTechResearched(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> action) {
            if (action.target == RDTech.OperationResult.Successful) {
                Dictionary<string, string> autoUnlocks = new Dictionary<string, string>() {
                    ["roverTechII"] = "fieldScience",
                    ["structuralII"] = "generalConstruction",
                    ["probeControlI"] = "flightControl",
                    ["probeControlII"] = "advFlightControl",
                    ["probeControlIII"] = "unmannedTech",
                    ["probeControlIV"] = "advUnmanned" };
                if (autoUnlocks.ContainsKey(action.host.techID))
                    UnlockTech(autoUnlocks[action.host.techID]); }
        }
        private void Popup() => PopupDialog.SpawnPopupDialog(
            new MultiOptionDialog("OpenTreePopup", "", Localizer.Format("#oTreeLoc_start"), HighLogic.UISkin, new Rect(0.5f, 0.5f, 250, 1),
                new DialogGUIBase[2] {
                    new DialogGUIButton(Localizer.Format("#oTreeLoc_uncrewedTech"), delegate { UnlockTech("uncrewedTech"); }, true),
                    new DialogGUIButton(Localizer.Format("#oTreeLoc_crewedTech"), delegate { UnlockTech("crewedTech"); }, true)
                }), false, HighLogic.UISkin);
    }

}
