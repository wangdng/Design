--主入口函数。从这里开始lua逻辑
--将所有要使用的lua脚本在初始化的时候一次性加载进来,这样在单一的某个脚本里就不需要require,能够防止重复require
--重复require并不会报错,但是会重置数据,我在项目中出现一个脚本中的一个全局表就是赋值不成功找了很久都找不到原因就是因为这个
--require就是Dofile,这里一次性加载了,在C#里也不需要用Dofile了
require "Global/Define"
require "Global/LEvent"
require "Global/EventNode"
require "Global/LMsgCenter"
require "Global/LManagerBase"
require "Global/LMsgBase"
require "UI/LUIBase"
require "UI/LUIMsg"
require "UI/LUIManager"
require "UI/LUILoad"
require "Asset/LAssetManager"
require "Asset/LAssetBase"
require "Asset/LAssetMsg"
require "NetWork/LNetWorkMnanger"
require "NetWork/LNetBase"

function Main()					
	print("logic start")	 		
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
end