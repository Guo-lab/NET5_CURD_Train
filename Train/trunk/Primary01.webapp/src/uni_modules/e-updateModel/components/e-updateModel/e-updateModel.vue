<template>
  <view>
    <myMask
      ref="updataMask"
      top="0"
      :isflexcenter="true"
      :hindex="true"
      noclickhide="true"
      @onHideHander="onHideUpdateMask()"
      @onShowHander="onShowUpdateMask()"
    >
      <div
        class="model_box container_flex column center"
        :class="{ active: showActive }"
        :style="{ height: aheight + 'upx', width: awidth + 'upx' }"
      >
        <!-- <slot></slot> -->
        <div class="active_box">
          <div class="update_box_text1">版本更新</div>
          <div class="update_box_text2 text_info">发现新版本，请更新~</div>
          <p class="update_box_text3">版本号:{{ updateText }}-{{version}}</p>
          <div class="button-box container_flex row">
            <button
              v-if="!hideUpdateCancel"
              class="quit_btn"
              plain="true"
              hover-class="plain-hover"
              type="primary"
              @click.stop="cancelQuit()"
            >
              取消
            </button>
            <button
              class="noborder confirm_btn"
              hover-class="primary-hover"
              type="primary"
              @click.stop="confirmUpdate()"
            >
              更新
            </button>
          </div>
        </div>
      </div>
    </myMask>
  </view>
</template>
<script>
import myMask from "./mask.vue";
import JSGlobal from "projectbase/JSGlobal";
export default {
  // 自定义弹框宽高
  props: ["aheight", "awidth"],
  data() {
    return {
      showActive: true, // 弹框动画
      hideUpdateCancel: 0, // 是否显示取消按钮
      updateText: "P2104", // 更新tips
      updateUrl: "", // 整包安装包
      wgtUrl: "", // wgt更新包
	  version:""
    }; 
  },
  components: {
    myMask,
  },
  methods: {
	  	   // 按需返回整包安装包，与wgt更新包，有wgt更新包时优先更新wgt包。此处替换成自己的接口即可，字段名要对应。
    updateVersion() {
	 var update = this.$refs.updataMask;
	  var website=JSGlobal.pbConfig.UrlContextPrefix+JSGlobal.pbConfig.UrlMappingPrefix;
	 var that =this;
	  plus.runtime.getProperty(plus.runtime.appid, function(wgtInfo) {
	   //例子:获取版本号
	    console.log(JSGlobal.pbConfig);
	   var localversion =wgtInfo.version;
	   uni.request({
	   		  url: website+'Home/Main/GetVersion?_ForViewModelOnly=true&_ForSmallScreen=true',
	   		  success: (res) => {
				  var version = res.data.data.replace(".apk","").replace(".wgt","");
				  console.log(res.data.data,localversion);
				   if( wgtInfo.version !=version){ 
					that.version = res.data.data;
				    var url=JSGlobal.pbConfig.UrlContextPrefix+"/Mobile/Download/DownloadAttachment?psSaveTo=APK&psOriginalFileName=";						
					if(res.data.toString().includes(".apk")){
						that.updateUrl=url+"My.apk&psSavedFileName="+res.data.data;
					}else{
						
						that.wgtUrl=url+"My.wgt&psSavedFileName="+res.data.data;
					}					   
					   console.log(that.wgtUrl);
					update.showMask();			  
				   }else{
						   uni.showModal({
							 title: "提示",
							 showCancel: false,
							 content: "当前已是最新版本",
							 success: function (res) {
							  // uni.redirectTo({ url: "ShowLogin" });
							 }
						   });
				   }
	   		  }
	   });	  
	  });
    },
    onHideUpdateMask() {
      this.showActive = false;
    },
    onShowUpdateMask() {
      let _this = this;
      setTimeout(() => {
        _this.showActive = true;
      }, 200);
    },
    confirmUpdate() {
      let _this = this;
      // 优先更新wgt包
      if (this.wgtUrl) {
        this.hotUpdateFun();
      } else if (this.updateUrl) {
        plus.runtime.openURL(this.updateUrl);
        if (plus.os.name.toLowerCase() === "android") {
          plus.runtime.quit();
        } else {
          const threadClass = plus.ios.importClass("NSThread");
          const mainThread = plus.ios.invoke(threadClass, "mainThread");
          plus.ios.invoke(mainThread, "exit");
        }
      } else {
        plus.nativeUI.toast("无安装包可用，请联系管理员");
      }
    },
    cancelQuit() {
      this.$refs.updataMask.hideMask();
    },
    hotUpdateFun() {
      let _this = this;
      var watiting = plus.nativeUI.showWaiting("开始下载：0%");
      // 创建下载任务
      const downloadTask = uni.downloadFile({
        url: _this.wgtUrl,
        success: (res) => {
          if (res.statusCode === 200) {
            watiting.setTitle("安装中...");
            // console.log('>>>>>tempFilePath', res.tempFilePath)
            plus.runtime.install(
              res.tempFilePath,
              {
                force: true,
              },
              function (succ) {
                // console.log('install success...');
                plus.nativeUI.closeWaiting();
                plus.nativeUI.alert("更新完成！", function () {
                  // 热更新完成自动重启
                  plus.runtime.restart();
                });
              },
              function (e) {
                plus.nativeUI.closeWaiting();
                plus.nativeUI.alert("更新失败,点击确认手动更新", function () {
                  plus.runtime.openURL(_this.updateUrl);
                  plus.runtime.quit();
                });
                console.error("install fail...", e);
              }
            );
          }
        },
      });
      downloadTask.onProgressUpdate((res) => {
        console.log("下载进度" + res.progress);
        // console.log('已经下载的数据长度' + res.totalBytesWritten);
        // console.log('预期需要下载的数据总长度' + res.totalBytesExpectedToWrite);
        watiting.setTitle("已下载：" + res.progress + "%");
      });
    },
  },
  onload() {},
};
</script>
<style scoped lang="scss">
$primarycolor: rgb(21, 125, 251);
$primaryHoverColor: lighten($primarycolor, 10%);
$whiteColor: white;

.container_flex.center {
  align-items: center;
  justify-content: center;
  -webkit-align-items: center;
  -webkit-justify-content: center;
}
.container_flex {
  display: flex;
  display: -webkit-flex;
}
// 布局end
button[type="primary"] {
  background-color: $primarycolor;
  border-color: $primarycolor;
  color: $whiteColor;
}
button[type="primary"][plain] {
  background-color: transparent;
  color: $primarycolor;
}
button.noborder::after {
  border: 0 !important;
  outline: 0 !important;
}
.primary-hover {
  background-color: $primaryHoverColor !important;
  border-color: $primaryHoverColor !important;
}

.model_box {
  position: absolute;
  width: 75%;
  height: 368upx;
  // max-height: 500upx;
  opacity: 0.5;
  transform: scale(0.5);
  border-radius: 15upx;
  transition: all 0.3s;
  background-color: #fff;
}
.active {
  transform: scale(1);
  opacity: 1;
}
// activeModel
.active_box {
  width: 100%;
  height: 100%;
  text-align: center;
  padding: 20upx;
  box-sizing: border-box;
  word-wrap: break-word;
  word-break: normal;
}
.update_box_text1 {
  font-size: 38upx;
  color: rgb(71, 79, 102);
  font-weight: 600;
  text-align: center;
}
.update_box_text2 {
  font-size: 32upx;
  color: rgb(122, 130, 153);
  margin-top: 20upx;
}
.update_box_text3 {
  color: rgb(122, 130, 153);
  margin-top: 20upx;
}
.button-box {
  position: absolute;
  justify-content: space-around;
  -webkit-justify-content: space-around;
  bottom: 20upx;
  width: 100%;
  height: 100upx;
  align-items: center;
  -webkit-align-items: center;
  left: 0;
}
.quit_btn,
.confirm_btn {
  flex: 1;
  height: 84upx;
  border-radius: 40upx;
  font-size: 34upx;
  line-height: 84upx;
  margin: 0 30upx;
}
.quit_btn {
  border: 1px solid $primarycolor;
  color: $primarycolor !important;
}
.confirm_btn {
  color: #fff;
}
// activeModel end
</style>
