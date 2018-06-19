MsgSpan = 3000

--lua和C#中所有消息
LuaManagerID = 
{
--Lua中的消息
	LGameManager = 0,

    LUIManager = MsgSpan,

    LNPCManager = MsgSpan * 2,

    LAssetManager = MsgSpan * 3,

    LParticleManager = MsgSpan * 4,

    LNetWorkManager = MsgSpan * 5,

    LCharactorManager = MsgSpan * 6,
	
--C#中的消息,中间预留一部分可扩展
	GameManager = MsgSpan * 10,

    UIManager = MsgSpan * 11,

    NPCManager = MsgSpan * 12,

    AssetManager = MsgSpan * 13,

    ParticleManager = MsgSpan * 14,

    NetWorkManager = MsgSpan * 15,

    CharactorManager = MsgSpan * 16,
}