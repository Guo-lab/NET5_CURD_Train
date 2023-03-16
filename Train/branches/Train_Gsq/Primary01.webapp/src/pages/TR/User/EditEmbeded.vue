<template>
  <view class="content" v-if="ResolveReady">
    <uni-forms ref="vmForm" :value="fv" >
        <uni-forms-item label="姓名" name="Name">
          <uni-easyinput
            type="text"
            v-model="fv.Name"
            placeholder="请输入姓名"
          />
        </uni-forms-item>
        <uni-forms-item name="Age" required label="年龄">
          <input
            type="text"
            v-model="fv.Age"
            class="uni-input-border"
            placeholder="请输入年龄"
          />
        </uni-forms-item>
    </uni-forms>
  </view>
</template>

<script lang="ts" src='./EditEmbeded'>
</script>

<style>
.content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.card {
  box-shadow: 5px 5px 5px rgba(153, 204, 255, 0.4);
  border: 2px solid rgba(153, 204, 255, 0.4) !important;
  margin: 2%;
  height: 100%;
  border-radius: 0.5rem;
  width: 100%;
}
.align-center {
  width: 100%;
  height: 2rem;
  line-height: 2rem;
  text-align: center;
}
</style>
