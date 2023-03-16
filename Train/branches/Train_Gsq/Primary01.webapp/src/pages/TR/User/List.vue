<template>
    <view class="content" v-if="ResolveReady">
        <button type="button" @click="btn_resolve()">手动Resolve</button>
        <UserEditEmbeded ref="myEmbeded" :prop1="1" :pvm='t'></UserEditEmbeded>
<!--        <z-paging use-page-scroll ref="paging" :auto="false" v-model="vm.ResultList" @query="PagingBind($event,'Search')"
             :default-page-size="vm.Input.ListInput.Pager.PageSize">
 -->              <uni-table>
                <uni-tr v-for="(item, index) in vm.ResultList" :key="index">
                  <uni-td>
                    <navigator :url="Url.Link('TR/User/Edit',{Id:item.Id})"> {{ item.Name }}</navigator>
                  </uni-td>
                </uni-tr>
                <uni-tr></uni-tr>
              </uni-table>
<!--        </z-paging> -->
    </view>
</template>
<!-- <UserEditEmbeded ref="myEmbeded" :prop1="1" :pvm='t'></UserEditEmbeded> -->
<!-- <z-paging use-page-scroll ref="paging" :auto="false" v-model="vm.ResultList" @query="PagingBind($event,'Search')"
     :default-page-size="vm.Input.ListInput.Pager.PageSize">
      <uni-table>
        <uni-tr v-for="(item, index) in vm.ResultList" :key="index">
          <uni-td>
            <navigator :url="Url.Link('TR/User/Edit',{Id:item.Id})"> {{ item.Name }}</navigator>
          </uni-td>
        </uni-tr>
        <uni-tr></uni-tr>
      </uni-table>
</z-paging> -->
<script lang="ts" src="./List"></script>

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
