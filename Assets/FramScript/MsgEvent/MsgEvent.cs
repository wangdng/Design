using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent:ushort
{
    Register = MsgManager.GameManager + 1,
    CloseLight,
    MaxValue
}

public enum UIEvent:ushort
{
    OpenLight = MsgManager.UIManager + 1,
    CloseLight,
    SendUIPos,
    BloodLose,
    MaxValue
}

public enum CharactorAnimEvent: ushort
{
    init = MsgManager.CharactorManager + 1,
    Idle,
    Run,
    Attack,
    BigAttack,
    SmallAttack,
    SpecialAttack,
    Die,
    MaxValue
}

public enum CharactorDataEvent : ushort
{
    init = CharactorAnimEvent.MaxValue + 1,

    joyStick,

    joyStickBegin,

    joyStickEnd,

    ReduceBlood,

    MonsterPlayerDead,

    MaxValue
}

public enum AssetEvent : ushort
{
    HunkRes = MsgManager.AssetManager + 1,
    HunkResBackMsg,
    ReleaseSingleABFile,
    ReleaseABFiles,
    ReleaseAllABFiles,
    ReleaseBundle,
    ReleaseAllBundle,
    ReleaseAllBundleAndABFiles,
    MaxValue
}

public enum ParticleEvent : ushort
{
   PlayPartice = MsgManager.ParticleManager + 1,
   StopPartice,
    MaxValue
}

public enum NetWorkEvent : ushort
{
    TcpConnect = MsgManager.NetWorkManager + 1,
    TcpSendMsg,
    UdpConnect,
    UdpSendMsg,
    MaxValue
}

public enum NPCAnimEvent:ushort
{
    NPCIdle = MsgManager.NPCManager +1,
    NPCRun,
    NPCAttack,
    NPCdie,

    MaxValue
}


public enum NPCLogicEvent : ushort
{
    CalcMove = NPCAnimEvent.MaxValue + 1,
    PlayAttack,
    PlayAttackEnd,
    MaxValue
}
