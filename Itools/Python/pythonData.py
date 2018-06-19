#coding=utf-8

import pythonMoudle

#python 中的元组,用括号表示,用逗号隔开
tup =("123","456")

print("cur data:" + tup[1])

#python 中的元组不能使用这种方式来更新数据
#tup[1] = "789"

#元组不能直接修改其元素的数据,只能取其中的数据来使用,元组是只读的
tmp = tup[1] + "789"

print("tmp data:" + tmp)

#删除元组
#del tup

#因为删除了,所以报找不到的错误
#print(tup[0])

#元组取指定数据,取从开始到结束的数据
print(tup[0:2])

#也可以用负数来取
print(tup[-2:-1])

print("================================================================")
#列表,列表是用中括号表示,可以增删改查的
list = ["ggg","kkk","ppp"]

print(list[2])

list[2] = "yyyyy"

print(list[2])

#列表的删除操作
del list[2]

print(list)

#列表的增加操作,增加操作可以增加不同的数据类型
list.append(890)

print(list)
#结果为["ggg","kkk",890]

#列表的插入操作
list.insert(1,"iii")
print(list)
#结果为["ggg","iii","kkk",890]

print("================================================================")

#字典,字典是用大括号来表示,并且初始化赋值的时候使用的是冒号
dict = {"name":"leon","age":20,"sex":"man"}
#访问
print(dict["name"])

#修改,并且可以修改为不同的数据类型
dict["name"] = 123
print(dict["name"])

#删除
del dict["name"]
print(dict)
#输出结果为{"age":20,"sex":"man"}

#清空,注意上边的list是没有clear方法的
dict.clear()
print(dict)
#输出结果为{}
print("================================================================")

#调用其他模块方法
pythonMoudle.test()
#输出结果为moudle is Used
















