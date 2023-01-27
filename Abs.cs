using ModCommon.Util;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

internal class Abs : MonoBehaviour {
    private int HP = 2000;
    private GameObject _spikeMaster;
    private GameObject _spikeTemplate;
    private GameObject _beamsweeper;
    private GameObject _beamsweeper2;
    private GameObject _knight;
    private HealthManager _hm;
    private PlayMakerFSM _attackChoices;
    private PlayMakerFSM _attackCommands;
    private PlayMakerFSM _control;
    private PlayMakerFSM _phaseControl;
    private PlayMakerFSM _spikeMasterControl;
    private PlayMakerFSM _beamsweepercontrol;
    private PlayMakerFSM _beamsweeper2control;
    private PlayMakerFSM _spellControl;
    private PlayMakerFSM _teleport;
    private GameObject[] _spikeset1;
    private GameObject[] _spikeset2;
    private GameObject[] _spikeset3;
    private GameObject ascendBeam = null;
    private int CWRepeats = 0;
    private bool disableBeamSet = false;
    private bool swordRainSpikesSet = false;
    private bool arena2Set = false;
    private bool onePlatSet = false;
    private bool noPlatsSet = false;
    private bool finalDanceOrbsConfigured = false;
    private bool finalDanceExtraAttackConfigured = false;

    private void Awake() {
        Log("Added AbsRad MonoBehaviour");
        _hm = base.gameObject.GetComponent<HealthManager>();
        _attackChoices = base.gameObject.LocateMyFSM("Attack Choices");
        _attackCommands = base.gameObject.LocateMyFSM("Attack Commands");
        _control = base.gameObject.LocateMyFSM("Control");
        _phaseControl = base.gameObject.LocateMyFSM("Phase Control");
        _spikeMaster = GameObject.Find("Spike Control");
        _spikeMasterControl = _spikeMaster.LocateMyFSM("Control");
        _spikeTemplate = GameObject.Find("Radiant Spike");
        _beamsweeper = GameObject.Find("Beam Sweeper");
        _beamsweeper2 = Object.Instantiate(_beamsweeper);
        _beamsweeper2.AddComponent<BeamSweeperClone>();
        _beamsweepercontrol = _beamsweeper.LocateMyFSM("Control");
        _beamsweeper2control = _beamsweeper2.LocateMyFSM("Control");
        _knight = GameObject.Find("Knight");
        _spellControl = _knight.LocateMyFSM("Spell Control");
        _teleport = base.gameObject.LocateMyFSM("Teleport");
        _spikeset1 = new GameObject[5];
        _spikeset2 = new GameObject[9];
        _spikeset3 = new GameObject[9];
    }

    private void Start() {
        Log("Changing fight variables...");
        // intro
        _control.GetAction<SendEventByName>("First Tele", 3).sendEvent = "HugeShake";

        // hp values
        _hm.hp += HP;
        _phaseControl.FsmVariables.GetFsmInt("P2 Spike Waves").Value += 1750;
        _phaseControl.FsmVariables.GetFsmInt("P3 A1 Rage").Value += 1700;
        _phaseControl.FsmVariables.GetFsmInt("P4 Stun1").Value += 1300;
        _phaseControl.FsmVariables.GetFsmInt("P5 Acend").Value += 1000;
        _control.GetAction<SetHP>("Scream", 7).hp = 2000;

        // orb barrage
        _attackCommands.GetAction<Wait>("Orb Antic", 0).time = 0.1f;
        _attackCommands.GetAction<SetIntValue>("Orb Antic", 1).intValue = 12;
        _attackCommands.GetAction<RandomInt>("Orb Antic", 2).min = 10;
        _attackCommands.GetAction<RandomInt>("Orb Antic", 2).max = 14;
        _attackCommands.GetAction<Wait>("Orb Summon", 2).time = 0.1f;
        _attackCommands.GetAction<Wait>("Orb Pause", 0).time = 0.1f;
        _attackChoices.GetAction<Wait>("Orb Recover", 0).time = 0.5f;

        // nail fan
        _attackCommands.GetAction<SetIntValue>("Nail Fan", 4).intValue.Value = 18;
        _attackCommands.GetAction<Wait>("Nail Fan", 2).time.Value = 0.01f;
        _attackCommands.GetAction<SetIntValue>("CW Restart", 0).intValue.Value = 18;
        _attackCommands.GetAction<SetIntValue>("CCW Restart", 0).intValue.Value = 18;
        _attackCommands.GetAction<FloatAdd>("CW Restart", 2).add.Value = -10f;
        _attackCommands.GetAction<FloatAdd>("CCW Restart", 2).add.Value = 10f;
        _attackCommands.RemoveAction("CW Restart", 1);
        _attackCommands.RemoveAction("CCW Restart", 1);
        _attackCommands.RemoveAction("CW Repeat", 0);
        _attackCommands.RemoveAction("CCW Repeat", 0);
        _attackCommands.GetAction<FloatAdd>("CW Spawn", 2).add.Value = -20f;
        _attackCommands.GetAction<FloatAdd>("CCW Spawn", 2).add.Value = 20f;

        // beam sweep
        _attackChoices.GetAction<Wait>("Beam Sweep L", 0).time = 0.5f;
		_attackChoices.GetAction<Wait>("Beam Sweep R", 0).time = 0.5f;
		_attackChoices.ChangeTransition("A1 Choice", "BEAM SWEEP R", "Beam Sweep L");
		_attackChoices.ChangeTransition("A2 Choice", "BEAM SWEEP R", "Beam Sweep L 2");
		_attackChoices.GetAction<SendEventByName>("Beam Sweep L 2", 1).sendEvent = "BEAM SWEEP L";
		_attackChoices.GetAction<SendEventByName>("Beam Sweep R 2", 1).sendEvent = "BEAM SWEEP R";
        _attackChoices.GetAction<Wait>("Beam Sweep L 2", 0).time = 3.5f;
		_attackChoices.GetAction<Wait>("Beam Sweep R 2", 0).time = 3.5f;

        // beam burst
        _attackCommands.GetAction<SendEventByName>("EB 1", 9).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 1", 10).time = 0.5f;
        _attackCommands.GetAction<SendEventByName>("EB 2", 9).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 2", 10).time = 0.5f;
        _attackCommands.GetAction<SendEventByName>("EB 3", 9).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 3", 10).time = 0.5f;
        _attackCommands.GetAction<SendEventByName>("EB 4", 4).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 4", 5).time = 0.5f;
        _attackCommands.GetAction<SendEventByName>("EB 5", 5).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 5", 6).time = 0.5f;
        _attackCommands.GetAction<SendEventByName>("EB 6", 5).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 6", 6).time = 0.5f;
        _attackCommands.GetAction<SendEventByName>("EB 7", 8).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 7", 9).time = 0.5f;
        _attackCommands.GetAction<SendEventByName>("EB 8", 8).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 8", 9).time = 0.5f;
        _attackCommands.GetAction<SendEventByName>("EB 9", 8).delay = 0.3f;
        _attackCommands.GetAction<Wait>("EB 9", 9).time = 0.5f;
        _attackCommands.GetAction<Wait>("Eb Extra Wait", 0).time = 0.05f;

        // ascension beam
        _attackCommands.GetAction<SendEventByName>("Aim", 10).delay = 1f;
        _attackCommands.GetAction<Wait>("Aim", 11).time = 0.4f;
        _attackCommands.GetAction<SendEventByName>("Aim", 8).delay = 0.4f;
        _attackCommands.GetAction<SendEventByName>("Aim", 9).delay = 0.4f;

        // sword rain attack
        _attackChoices.GetAction<SendEventByName>("Nail Top Sweep", 1).delay = 0.35f;
        _attackChoices.GetAction<SendEventByName>("Nail Top Sweep", 2).delay = 0.7f;
        _attackChoices.GetAction<SendEventByName>("Nail Top Sweep", 3).delay = 1.05f;
        _attackChoices.GetAction<Wait>("Nail Top Sweep", 4).time = 3f;
        _attackChoices.InsertAction("Nail Top Sweep", new SendEventByName{
            eventTarget = _attackChoices.GetAction<SendEventByName>("Nail Top Sweep", 2).eventTarget,
            sendEvent = "COMB TOP",
            delay = 1.4f,
            everyFrame = false
        }, 4);
        _attackChoices.InsertAction("Nail Top Sweep", new SendEventByName{
            eventTarget = _attackChoices.GetAction<SendEventByName>("Nail Top Sweep", 2).eventTarget,
            sendEvent = "COMB TOP",
            delay = 1.75f,
            everyFrame = false
        }, 4);

        // sword rain phase
        _control.GetAction<Wait>("Rage Comb", 0).time = 0.35f;
        
        // nail sweep
        _attackChoices.GetAction<SendEventByName>("Nail L Sweep", 1).delay = 0.25f;
        _attackChoices.GetAction<SendEventByName>("Nail L Sweep", 1).delay = 1.85f;
        _attackChoices.GetAction<SendEventByName>("Nail L Sweep", 2).delay = 3.45f;
        _attackChoices.GetAction<Wait>("Nail L Sweep", 3).time = 4.5f;
        _attackChoices.GetAction<SendEventByName>("Nail R Sweep", 1).delay = 0.25f;
        _attackChoices.GetAction<SendEventByName>("Nail R Sweep", 1).delay = 1.85f;
        _attackChoices.GetAction<SendEventByName>("Nail R Sweep", 2).delay = 3.45f;
        _attackChoices.GetAction<Wait>("Nail R Sweep", 3).time = 4.5f;
        AddNailWall("Nail L Sweep", "COMB R", 1.3f, 1);
        AddNailWall("Nail R Sweep", "COMB L", 1.3f, 1);
        AddNailWall("Nail L Sweep", "COMB R", 2.9f, 1);
        AddNailWall("Nail R Sweep", "COMB L", 2.9f, 1);
        AddNailWall("Nail L Sweep 2", "COMB R2", 1f, 1);
        AddNailWall("Nail R Sweep 2", "COMB L2", 1f, 1);

        // add shake to teleport
        _teleport.GetAction<SendEventByName>("Arrive", 5).eventTarget = _control.GetAction<SendEventByName>("Stun1 Out", 9).eventTarget;
        _teleport.GetAction<SendEventByName>("Arrive", 5).sendEvent = "SmallShake";

        // first phase spikes
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Left", 0).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Left", 1).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Left", 2).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Left", 3).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Left", 4).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Right", 0).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Right", 1).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Right", 2).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Right", 3).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Spikes Right", 4).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave L", 2).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave L", 3).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave L", 4).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave L", 5).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave L", 6).sendEvent = "UP";
        _spikeMasterControl.GetAction<WaitRandom>("Wave L", 7).timeMin = 0.1f;
        _spikeMasterControl.GetAction<WaitRandom>("Wave L", 7).timeMax = 0.1f;
        _spikeMasterControl.GetAction<SendEventByName>("Wave R", 2).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave R", 3).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave R", 4).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave R", 5).sendEvent = "UP";
        _spikeMasterControl.GetAction<SendEventByName>("Wave R", 6).sendEvent = "UP";
        _spikeMasterControl.GetAction<WaitRandom>("Wave R", 7).timeMin = 0.1f;
        _spikeMasterControl.GetAction<WaitRandom>("Wave R", 7).timeMax = 0.1f;
        _spikeMasterControl.SetState("Spike Waves");
        foreach (Transform spikeGroup in _spikeMaster.transform) {
            foreach (Transform spike in spikeGroup.gameObject.transform) {
                GameObject gameObject = spike.gameObject;
                gameObject.GetComponent<DamageHero>().damageDealt = 2;
            }
		}

        // first phase teleport range
        _control.FsmVariables.FindFsmFloat("A1 X Min").Value -= 5f;
        _control.FsmVariables.FindFsmFloat("A1 X Max").Value += 5f;
        _control.GetAction<RandomFloat>("Set Dest", 4).min.Value -= 2f;
        _control.GetAction<RandomFloat>("Set Dest", 4).max.Value += 2f;

        // prevent sword rain phase transition from removing full spike floor
        _phaseControl.RemoveAction("Set Phase 3", 0);

        // reduce stun animation time
        _control.GetAction<Wait>("Stun1 Start", 9).time = 0.5f;
        _control.GetAction<Wait>("Stun1 Roar", 3).time = 1f;
        _control.GetAction<Wait>("Plat Setup", 6).time = 1f;

        // platform phase spikes
        for (int i = 0; i < _spikeset1.Length; i++) {
            Vector2 position5 = new Vector2(66.5f + (float)i / 2f, 39.1f);
            _spikeset1[i] = Object.Instantiate(_spikeTemplate);
            _spikeset1[i].SetActive(value: true);
            _spikeset1[i].transform.SetPosition2D(position5);
            _spikeset1[i].GetComponent<DamageHero>().damageDealt = 2;
            _spikeset1[i].LocateMyFSM("Control").SendEvent("DOWN");
        }
        for (int i = 0; i < _spikeset2.Length; i++) {
            Vector2 position2 = new Vector2(57.7f + (float)i / 2f, 45.9f);
            _spikeset2[i] = Object.Instantiate(_spikeTemplate);
            _spikeset2[i].SetActive(value: true);
            _spikeset2[i].transform.SetPosition2D(position2);
            _spikeset2[i].GetComponent<DamageHero>().damageDealt = 2;
            _spikeset2[i].LocateMyFSM("Control").SendEvent("DOWN");
        }
        for (int i = 0; i < _spikeset3.Length; i++) {
            Vector2 position3 = new Vector2(49.6f + (float)i / 2f, 37.6f);
            _spikeset3[i] = Object.Instantiate(_spikeTemplate);
            _spikeset3[i].SetActive(value: true);
            _spikeset3[i].transform.SetPosition2D(position3);
            _spikeset3[i].GetComponent<DamageHero>().damageDealt = 2;
            _spikeset3[i].LocateMyFSM("Control").SendEvent("DOWN");
        }

        // left platform in platform phase
        FsmEventTarget platsEventTarget = new FsmEventTarget();
        platsEventTarget.target = FsmEventTarget.EventTarget.GameObject;
        platsEventTarget.excludeSelf = false;
        platsEventTarget.gameObject = new FsmOwnerDefault();
        platsEventTarget.gameObject.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
        platsEventTarget.gameObject.GameObject.Value = GameObject.Find("P2 SetA/Radiant Plat Wide (2)");
        platsEventTarget.sendToChildren = true;
        _control.GetAction<SendEventByName>("Climb Plats1", 3).eventTarget = platsEventTarget;

        // respawn platform in platform phase
        GameObject bottomPlat = GameObject.Find("Hazard Plat/Radiant Plat Wide (4)");
        FsmEventTarget bottomPlatEventTarget = new FsmEventTarget();
        bottomPlatEventTarget.target = FsmEventTarget.EventTarget.GameObject;
        bottomPlatEventTarget.excludeSelf = false;
        bottomPlatEventTarget.gameObject = new FsmOwnerDefault();
        bottomPlatEventTarget.gameObject.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
        bottomPlatEventTarget.gameObject.GameObject.Value = bottomPlat;
        bottomPlatEventTarget.sendToChildren = true;
        _control.AddAction("Climb Plats1", new SendEventByName{
            eventTarget = bottomPlatEventTarget,
            sendEvent = "SLOW VANISH",
            delay = 1.25f,
            everyFrame = false
        });
        bottomPlat.LocateMyFSM("radiant_plat").GetAction<Wait>("Vanish Antic", 1).time = 3.5f;

        // final phase
        _attackCommands.RemoveAction("Set Final Orbs", 0);
        GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").GetAction<Wait>("Vanish Antic", 1).time = 3.5f;
        // teleport y positions moved down by 1
        _control.GetAction<SetVector3Value>("Tele 11", 1).vector3Value = new Vector3(62.94f, 157.65f, 0.006f);
        _control.GetAction<SetVector3Value>("Tele 12", 1).vector3Value = new Vector3(53.88f, 157.65f, 0.006f);
        _control.GetAction<SetVector3Value>("Tele 13", 1).vector3Value = new Vector3(72.4f, 157.65f, 0.006f);

        Log("fin.");
    }

    private void Update() {
        // Create four waves during nail fan attack
        if (_attackCommands.FsmVariables.GetFsmBool("Repeated").Value) {
            switch (CWRepeats) {
                case 0:
                    CWRepeats = 1;
                    _attackCommands.FsmVariables.GetFsmBool("Repeated").Value = false;
                    break;
                case 1:
                    CWRepeats = 2;
                    _attackCommands.FsmVariables.GetFsmBool("Repeated").Value = false;
                    break;
                case 2:
                    CWRepeats = 3;
                    break;
            }
        } else if (CWRepeats == 3) {
            CWRepeats = 0;
        }

        // create second beam during beam sweep attack
        if (_beamsweepercontrol.ActiveStateName == _beamsweeper2control.ActiveStateName)
        {
            string activeStateName = _beamsweepercontrol.ActiveStateName;
            string text = activeStateName;
            if (text != null)
            {
                if (!(text == "Beam Sweep L"))
                {
                    if (text == "Beam Sweep R")
                    {
                        _beamsweeper2control.SendEvent("BEAM SWEEP L");
                    }
                }
                else
                {
                    _beamsweeper2control.SendEvent("BEAM SWEEP R");
                }
            }
        }

        // disable single beam sweep attack during platform phase
        if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P3 A1 Rage").Value + 30 && !disableBeamSet) {
            disableBeamSet = true;
            _attackChoices.ChangeTransition("A1 Choice", "BEAM SWEEP L", "Orb Wait");
			_attackChoices.ChangeTransition("A1 Choice", "BEAM SWEEP R", "Eye Beam Wait");
        }

        // set damage of spikes to 1 during sword rain phase
        if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P3 A1 Rage").Value && !swordRainSpikesSet) {
            swordRainSpikesSet = true;
            foreach (Transform spikeGroup in _spikeMaster.transform) {
                foreach (Transform spike in spikeGroup.gameObject.transform) {
                    spike.gameObject.GetComponent<DamageHero>().damageDealt = 1;
                }
            }
        }

        // plat phase beam attack
        if (_attackChoices.FsmVariables.GetFsmInt("Arena").Value == 2 && !arena2Set) {
            arena2Set = true;
            _beamsweepercontrol.GetAction<SetPosition>("Beam Sweep L", 3).x = 89f;
            _beamsweepercontrol.GetAction<iTweenMoveBy>("Beam Sweep L", 5).vector = new Vector3(-75f, 0f, 0f);
            _beamsweepercontrol.GetAction<iTweenMoveBy>("Beam Sweep L", 5).time = 3f;
            _beamsweepercontrol.GetAction<SetPosition>("Beam Sweep R", 4).x = 32.6f;
            _beamsweepercontrol.GetAction<iTweenMoveBy>("Beam Sweep R", 6).vector = new Vector3(75f, 0f, 0f);
            _beamsweepercontrol.GetAction<iTweenMoveBy>("Beam Sweep R", 6).time = 3f;
            _beamsweeper2control.GetAction<SetPosition>("Beam Sweep L", 2).x = 89f;
            _beamsweeper2control.GetAction<iTweenMoveBy>("Beam Sweep L", 4).vector = new Vector3(-75f, 0f, 0f);
            _beamsweeper2control.GetAction<iTweenMoveBy>("Beam Sweep L", 4).time = 3f;
            _beamsweeper2control.GetAction<SetPosition>("Beam Sweep R", 3).x = 32.6f;
            _beamsweeper2control.GetAction<iTweenMoveBy>("Beam Sweep R", 5).vector = new Vector3(75f, 0f, 0f);
            _beamsweeper2control.GetAction<iTweenMoveBy>("Beam Sweep R", 5).time = 3f;
        }

        // reappearance of platform phase spikes after being downed
        if (arena2Set && _spikeset1[0].LocateMyFSM("Control").ActiveStateName == "Downed") {
            foreach (GameObject go in _spikeset1) {
                go.LocateMyFSM("Control").SendEvent("UP");
            }
            foreach (GameObject go2 in _spikeset2) {
                go2.LocateMyFSM("Control").SendEvent("UP");
            }
            foreach (GameObject go3 in _spikeset3) {
                go3.LocateMyFSM("Control").SendEvent("UP");
            }
        }

        // respawn platform in platform phase
        if (arena2Set && GameObject.Find("Hazard Plat/Radiant Plat Wide (4)").LocateMyFSM("radiant_plat").ActiveStateName == "Appear 2") {
            GameObject.Find("Hazard Plat/Radiant Plat Wide (4)").LocateMyFSM("radiant_plat").SendEvent("SLOW VANISH");
        }

        if (!(base.gameObject.transform.position.y >= 150f)) {
            if (ascendBeam == null) {
                ascendBeam = GameObject.Find("Ascend Beam");
            }
        }

        // final phase beam attack
        if(_knight.transform.GetPositionY() > 152f && !finalDanceExtraAttackConfigured) {
            finalDanceExtraAttackConfigured = true;
            ascendBeam.LocateMyFSM("Control").SendEvent("END");

            // set ascend beam attack back to normal abs rad values
            _attackCommands.GetAction<SendEventByName>("Aim", 10).delay = 0.65f;
            _attackCommands.GetAction<Wait>("Aim", 11).time = 0.95f;
            _attackCommands.GetAction<SendEventByName>("Aim", 8).delay = 0.5f;
            _attackCommands.GetAction<SendEventByName>("Aim", 9).delay = 0.5f;

            // add beam attack actions to the teleport sequence
            _control.AddAction("Final Idle", _attackCommands.GetAction<ActivateGameObject>("AB Start", 0));
            for (int i = 0; i <= 10; i++) {
                _control.AddAction("Final Idle", _attackCommands.GetAction("Aim", i));
            }
            _control.InsertAction("A2 Tele Choice 2", new ActivateGameObject{
                gameObject = _attackCommands.GetAction<ActivateGameObject>("AB Start", 0).gameObject,
                activate = false,
                recursive = false,
                resetOnExit = false,
                everyFrame = false,
            }, 0);
        }

        // configure final phase orbs
        if(_hm.hp <= _phaseControl.FsmVariables.GetFsmInt("P5 Acend").Value && !finalDanceOrbsConfigured) {
            finalDanceOrbsConfigured = true;
            _attackCommands.GetAction<Wait>("Orb Antic", 0).time.Value = 0.01f;
            _attackCommands.GetAction<Wait>("FinalOrb Pause", 0).time.Value = 0.75f;
            _attackChoices.GetAction<Wait>("Orb Recover", 0).time.Value = 0.01f;
        }

        // final phase first plat removal
        if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P5 Acend").Value - 500 && !onePlatSet) {
            onePlatSet = true;
            GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").SendEvent("SLOW VANISH");
        }

        // final phase second plat removal
        if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P5 Acend").Value - 500 - 500 && !noPlatsSet) {
            noPlatsSet = true;
            GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").SendEvent("SLOW VANISH");
        }

        // final phase respawn platform
        if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P5 Acend").Value - 500 - 500 && GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").ActiveStateName == "Appear 2") {
            GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").SendEvent("SLOW VANISH");
        }
    }

    private void AddNailWall(string stateName, string eventName, float delay, int index) {
        FsmutilExt.InsertAction(_attackChoices, stateName, (FsmStateAction)new SendEventByName
        {
            eventTarget = _attackChoices.GetAction<SendEventByName>("Nail L Sweep", 0).eventTarget,
            sendEvent = eventName,
            delay = delay,
            everyFrame = false
        }, index);
    }

    private static void Log(object obj) {
        Modding.Logger.Log("[Any Radiance] - " + obj);
    }
}