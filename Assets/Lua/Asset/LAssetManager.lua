--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion

LAssetManager = LManagerBase:New()

LAssetManager.__index = LAssetManager

local this = LAssetManager

function LAssetManager:New()
    local self = {}
    setmetatable(self,LAssetManager)
    return self
end

function LAssetManager:GetInstance()
    return this
end

function LAssetManager:SendMessage(msg)
    if msg:GetManager() == LuaManagerID.LAssetManager  then
        self:ProcessEvent(msg)
		print("执行1")
    else
        LMsgCenter.GetInstacne().SendMessage(msg)
		print("执行2")
    end
end