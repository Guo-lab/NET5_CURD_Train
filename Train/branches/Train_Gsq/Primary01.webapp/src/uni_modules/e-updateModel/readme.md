# e-updateModel

- 可在`Android`app内实现应用内更新，自动重启，无需重新安装
- 更新逻辑分为两种：wgt包更新、整包更新。
- 更新优先级：wgt包 > 整包 （优先更新wgt包）
- wgt更新方式：应用内更新安装，更新完成自动重启
- 整包更新：点击跳转到浏览器打开接口返回的整包地址，进行下载安装
## 示例

![](https://ryk-test.oss-cn-shanghai.aliyuncs.com/qiuzong/update.gif)

## 快速上手

1. 例子
```html
<template>
	<view>
		<e-updateModel ref="myUpdataModel"></e-updateModel>
	</view>
</template>
```
2. 使用`$refs`调用更新接口
```js
	#ifdef APP-PLUS  
	 this.$refs.myUpdataModel.updateVersion()
	#endif
```
