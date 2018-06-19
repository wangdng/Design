using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这里之所以从30000开始,而不是从0开始,是因为lua中对应的消息是从0开始,所以这里从30000开始
public enum MsgManager
{
    //发给lua的消息
    LGameManager = 0,

    LUIManager = FramTool.MsgSpan,

    LNPCManager = FramTool.MsgSpan * 2,

    LAssetManager = FramTool.MsgSpan * 3,

    LParticleManager = FramTool.MsgSpan * 4,

    LNetWorkManager = FramTool.MsgSpan * 5,

    LCharactorManager = FramTool.MsgSpan * 6,

    //C#中的消息,中间预留一部分可扩展

    GameManager = FramTool.MsgSpan * 10,//消息从30000-32999都是GameManager

    UIManager = FramTool.MsgSpan * 11,//消息从33000-35999都是UIManager

    NPCManager = FramTool.MsgSpan * 12,//消息从36000-38999都是NPCManager

    AssetManager = FramTool.MsgSpan * 13,//消息从39000-41999都是AssetManager

    ParticleManager = FramTool.MsgSpan * 14,//消息从42000-44999都是ParticleManager

    NetWorkManager = FramTool.MsgSpan * 15,//消息从45000-47999都是NetWorkManager

    CharactorManager = FramTool.MsgSpan * 16,//消息从48000-50999都是CharactorManager
}

public class FramTool  
{
    //使用无符号short足够用了,因为ushort的取值范围是0-65535,而我们总共的消息才17999
    public const ushort MsgSpan = 3000;
}
