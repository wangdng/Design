--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion

LAssetMsg = LMsgBase:New(LuaManagerID.LAssetManager)

LAssetMsg.__index = LAssetMsg

function LAssetMsg:New(msgId, sceneName,  bundleName,  ABName,  isSingle)
    local self = {}
    setmetatable(self,LAssetMsg)
    self.sceneName = sceneName
    self.bundleName = bundleName
    self.ABName = ABName
    self.isSingle = isSingle
    self.MsgId = msgId

    return self
end



