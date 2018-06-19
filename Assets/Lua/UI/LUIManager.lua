--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion

LUIManager = LManagerBase:New()

LUIManager.__index = LUIManager

local this = LUIManager

sonMembers = {}

function LUIManager:New()
    local self = {}
    setmetatable(self,LUIManager)
    return self
end

function LUIManager:GetInstance()
    return this
end

function LUIManager:SendMessage(msg)
    if msg:GetManager() == LuaManagerID.LUIManager  then
        self:ProcessEvent(msg)
    else
        LMsgCenter.GetInstacne().SendMessage(msg)
    end
end

function LUIManager:FindGameObject(go)
	for k,v in pairs(sonMembers) do
		if k == go.name then
			return 1
		end
	end
	
	return 0
end

function LUIManager.RegisterGameObject(go)
        if this:FindGameObject(go) == 0 then
            sonMembers[go.name] = go
        end
end

function LUIManager:GetGameObject(name)
		for k,v in pairs(sonMembers) do
             return sonMembers[name]
        end
end
