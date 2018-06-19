--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion

LManagerBase = {eventTree = {}}

LManagerBase.__index = LManagerBase

function LManagerBase:New()
    local self = {}

    setmetatable(self,LManagerBase)

    return self

end
--查找是否已经注册了该消息
function LManagerBase:FindEvent(dict,id)
    for k,v in pairs(dict) do
        if k == id then
            return true
        end
    end
	
	return false
end

--通过一个消息注册一个或者多个脚本(EventNode是一个链表结构,多个脚本就存放在这个链表中),一个消息和多个脚本对应起来
function LManagerBase:RegisterMsg(id,EventNode)
--这种情况是该消息还没有注册,那么直接将脚本EventNode添加到EventTrees中
    if not self:FindEvent(self.eventTree,id) then
        self.eventTree[id] = EventNode
    else--这种情况是该消息已经存在,则将该脚本EventNode添加到和这个消息对应的EventNode的后面的节点(next)上

        tmpNode =  self.eventTree[id]
        --循环找到最后一个节点
        while tmpNode.Next ~= nil do
            tmpNode = tmpNode.Next
        end

        tmpNode.Next = EventNode

    end
end
--注册一个脚本对应多个消息,将一个脚本和多个消息对应起来
function LManagerBase:RegisterMsgs(script,msgs)
    for k,v in pairs(msgs) do
        tmpEvent = EventNode:New(script)

        self:RegisterMsg(v,tmpEvent)
    end
end

--删除消息ID为msgId对应的所有脚本中的script脚本节点
function LManagerBase:UnRegisterMsg(msgId,script)
    if  self:FindEvent(self.eventTree,msgId) then
    tmpEvent = self.eventTree[msgId]
    --要移除的脚步正好在链表的头部
        if tmpEvent.Value ==  script then
        --如果头部后面还有节点
            if tmpEvent.Next ~= nil then
                self.eventTree[msgId] = tmpEvent.Next--将头部节点向后移动一位,原来的头部节点就被移除了(也就是我们要移除的脚本)
                tmpEvent.Next = nil--将原先头部脚本的Next也赋值为nil
            else--如果头部后面没有节点,直接将头部删除
                tmpEvent.Value = nil
            end
        else--要移除的节点不是头部,在中间
            while tmpEvent.Next ~= nil and tmpEvent.Next.Value ~= script do--首先找到要移除的节点
                tmpEvent = tmpEvent.Next
            end

            if tmpEvent.Next.Next ~= nil then--要移除的节点的下一个节点不为空
                deleteNode =  tmpEvent.Next--得到要移除的节点
                tmpEvent.Next = deleteNode.Next--然后将要移除节点的上一个节点的Next(本来指向要移除的节点)指向为要移除节点的下个节点
                deleteNode.Next = nil--将要移除节点的Next也赋值为空
            else
                tmpEvent.Next = nil--要移除的节点的下一个节点为空说明要移除的节点就是最后一个节点,直接删除
            end
         
        end
    end
end

--删除传入的消息列表中所有对应script脚本的节点
function LManagerBase:UnRegisterMsgs(script,...)

    if ... == nil then
        return
    end

    for  k,v in pairs(...) do
        self:UnRegisterMsg(v,script)
    end
end

function LManagerBase:Destroy()
    keys = {}
    keyCount = 0
    for k,v in pairs(self.eventTree) do
        keys[keyCount] = k
        keyCount = keyCount +1
    end

    for i = 1, keyCount do
        self.eventTree[keys[i]] = nil
    end
end

function LManagerBase:ProcessEvent(msg)
    if self:FindEvent(self.eventTree,msg.MsgId) then
        local tmpEvent = self.eventTree[msg.MsgId]

        while tmpEvent ~= nil do
            tmpEvent.Value:ProcessEvent(msg)
			tmpEvent = tmpEvent.Next
        end
    else
        print("eventTree is not contain msg:"..msg.MsgId)
    end
end