using HutongGames.PlayMaker.Actions;
using ModCommon.Util;
using UnityEngine;

namespace AnyRadiance {
    internal class BeamSweeperClone : MonoBehaviour {
        private PlayMakerFSM _control;

        private void Awake() {
            AnyRadiance.instance.Log("Added BeamSweeperClone MonoBehaviour");
            _control = gameObject.LocateMyFSM("Control");
        }

        private void Start() {
            _control.GetAction<GetOwner>("Init", 0).storeGameObject = gameObject;
            // Refactoring this part actually introduces bugs so it has to be left as-is for now
            FsmutilExt.ChangeTransition(_control, "Idle", "BEAM SWEEP L", "Beam Sweep R"); // Cross the wires
            FsmutilExt.ChangeTransition(_control, "Idle", "BEAM SWEEP R", "Beam Sweep L");
            FsmutilExt.ChangeTransition(_control, "Idle", "BEAM SWEEP L 2", "Beam Sweep R 2");
            FsmutilExt.ChangeTransition(_control, "Idle", "BEAM SWEEP R 2", "Beam Sweep L 2");

            FsmutilExt.RemoveAction(_control, "Beam Sweep L", 0); // Ignore forced direction switches, to prevent accidental overlap
            FsmutilExt.RemoveAction(_control, "Beam Sweep R", 0); // bob fred is the GOAT
            FsmutilExt.RemoveAction(_control, "Beam Sweep L 2", 0);
            FsmutilExt.RemoveAction(_control, "Beam Sweep R 2", 2);
        }
    }
}
