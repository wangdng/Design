--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion

LAssetBase = {msgIds = {}}

LAssetBase.__index = LAssetBase

function LAssetBase:New()
    local self = {}
    setmetatable(self,LAssetBase)
    return self
end

function LAssetBase:RegisteSelf(script,msgs)
    LAssetManager:GetInstance():RegisterMsgs(script,msgs)
end

function LAssetBase:UnRegisteSelf(script,msgs)
    LAssetManager:GetInstance():UnRegisterMsgs(script,msgs)
end

function LAssetBase:SendMessage(msg)
    LAssetManager:GetInstance():SendMessage(msg)
end
