<script lang="js">
import Vue from "vue";
import { app } from "@/core/AppService";

export default Vue.extend({
  mpType: "app",
  onLaunch() {
    app.Application_Start();
  },
  onShow() {

  },
  onHide() {
    // console.log("App Hide");
  },
  onError(err) {
    app.Application_Error(err);
  },
});
</script>

<style>
/*每个页面公共css */
.uni-modal .uni-modal__hd {
  padding: 0rpx !important;
}
uni-page {
  display: block;
  width: 100%;
  height: 100%;
  background-color: #f5f5f5;
}
</style>
