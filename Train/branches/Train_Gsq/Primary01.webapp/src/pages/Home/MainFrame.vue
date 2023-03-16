<template>
  <view>
    <navigator :url="Url.Link('TR/User/List?Id=10')"> 点我进入User清单页面</navigator>
    <navigator url="/pages/TR/User/List"> 前进到List页 </navigator>
    {{Greeting}}
    <button type="button" @click="btn_click()">click</button>
  </view>
</template>

<script lang="ts" src='./MainFrame'></script>

<style>
.content {
  background-color: white;
}
.title {
  width: 100%;
  margin: 0 auto;
  vertical-align: middle;
  padding: 20rpx;
}
.title-head {
  height: 50rpx;
  width: 100rpx;
  margin: 30rpx auto 20rpx 20rpx;
  background-color: darkorange;
  text-align: center;
  color: white;
}
.title-text {
  text-align: left;
  font-weight: 800;
  margin-left: 20rpx;
}
.pendingCount {
  font-size: 60rpx !important;
  font-weight: 800;
  text-align: center;
  margin-top: 20rpx;
}
.pendingTitel {
  text-align: center;
  margin-top: 10rpx;
}
.modules {
  text-align: center;
  color: rgb(102, 102, 102);
  height: 100%;
}
.title-image {
  width: 100%;
}
.text {
  width: 100%;
}
</style>
