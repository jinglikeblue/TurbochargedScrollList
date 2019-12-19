# 文件简介(Class Intro)

- Basics
    - BaseScrollList.cs  
    列表类基类  
    the base class of all scrolllist
    - EGridConstraint.cs  
    网格列表的排列枚举，和GridLayoutGroup使用方式一致
    - GridPos.cs  
    网格列表使用的格子参数
    - IScrollList.cs  
    列表的接口
    - ScrollListItemModel.cs  
    列表项的数据模型
- LayoutSettings
    - GridLayoutSettings.cs  
    网格列表布局参数
    - HorizontalLayoutSettings.cs  
    水平列表布局参数
    - VerticalLayoutSettings.cs  
    垂直列表布局参数
- UnityComponents
    - ScrollListAdapter.cs  
    滚动列表适配器，用来实现列表类的更新
    - ScrollListItem.cs  
    滚动列表渲染列表项组件
    - TurbochargedGridScrollList.cs  
    挂载到ScrollView的网格列表组件
    - TurbochargedHorizontalScrollList.cs  
    挂载到ScrollView的水平列表组件
    - TurbochargedVerticalScrollList.cs  
    挂载到ScrollView的垂直列表组件
- GridScrollList.cs  
网格列表具体实现类
- HorizontalScrollList.cs  
水平列表具体实现类
- VerticalScrollList.cs  
垂直列表具体实现类

# 使用方法(How To Use)

## 实例化 Turbocharged Scroll List

开发中，我们有两种方式来吧Scroll View包装成高效的Turbocharged Scroll List。在三个不同列表的Demo中，我演示了3种不同的方式来生成Turbocharged Scroll，下面我们就来一一描述

>We have 2 way can improve Scroll View to Turbocharged Scroll List。And there i show 3 different way to create Turbocharged Scroll.

在垂直列表中,我们的列表项是来自于Assets中一个Prefab的引用，然后我在VerticalScrollListDemo中实例化了一个布局设置类，简单的设置了列表的「列表项间距」以及列表上方的空出间隔。接着我实例化了一个垂直列表类「Jing.TurbochargedScrollList.VerticalScrollList」，并传入了Scroll View对象，列表项，以及布局类。

>In VerticalScrollListDemo, i take the list item prefab from Assets

在水平列表中，我们的列表项是来自于Hierarchy中的一个GameObject，它被当做列表项时，HorizontalScrollList自动将其Active设置为了false。后续步骤则和VerticalScrollListDemo中提到的一样。

>In HorizontalScrollListDemo, i take the list item prefab from Hierarchy

在网格列表中，我们用了另一种方式来实例化Turbocharged Scroll List， 首先我们在Scroll View上添加了一个组件TurbochargedGridScrollList，并且直接配置了引用的列表项Prefab以及布局参数。代码中我们只需要拿到这个Component调用起GetList()方法，则可以获取到列表对象

>In GridScrollListDemo, i add TurbochargedGridScrollList Component on the Scroll View, and call the GetList() function to get list

## 常用事件

onRenderItem:该事件在每一个列表项被渲染时触发，三个参数分别是列表项GameObject上的组件，列表项对应的数据，以及一个bool值标记了该列表项是否是全新的，如果是false，则表示它是一个刚刚从active为false变为true的列表项，其对应的数据并没有改变，我们可以通过该bool值来优化我们的代码逻辑。

>onRenderItem: When the item show in list, this event called, and the isRefresh mark the item is have the different data(if false, we can jump the logic code)

onRebuildContent： 每当列表总宽度或者高度改变时触发

>onRebuildContent: Called when the list content size change

onRefresh: 每当列表滚动时触发

>onRefresh: Called when the list scroll