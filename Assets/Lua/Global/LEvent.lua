--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion

LUIEvent = 
{
    LOpenLight = LuaManagerID.LUIManager +1,
    LCloseLight = LuaManagerID.LUIManager +2,
    LSendUIPos = LuaManagerID.LUIManager +3,
    MaxValue = LuaManagerID.LUIManager + 4--lua里枚举不会自增
}

LAssetEvent = 
{
    HunkRes = LuaManagerID.LAssetManager + 1,
    HunkResBackMsg = LuaManagerID.LAssetManager + 2,
    ReleaseSingleABFile = LuaManagerID.LAssetManager + 3,
    ReleaseABFiles = LuaManagerID.LAssetManager + 4,
    ReleaseAllABFiles = LuaManagerID.LAssetManager + 5,
    ReleaseBundle = LuaManagerID.LAssetManager + 6,
    ReleaseAllBundle = LuaManagerID.LAssetManager + 7,
    ReleaseAllBundleAndABFiles = LuaManagerID.LAssetManager + 8,
    MaxValue = LuaManagerID.LAssetManager + 9,
}

LNetEvent = 
{
	Connect = LuaManagerID.LNetWorkManager + 1,
	SendMsg = LuaManagerID.LNetWorkManager + 2,
	MaxValue = LuaManagerID.LNetWorkManager + 3
}