<template>
    <div v-if="isShow"
         catchtouchmove="ture"
         @touchmove.stop.prevent="moveHandle"
         class="mask"
         :class="{'active': showBgk, 'container_flex': isflexcenter, 'center': isflexcenter, 'hz_index': hindex }"
         :style="{top: top + 'upx'}"
         id="mask"
         @click.stop="hideMask($event)">
        <slot></slot>
    </div>
</template>
<script>
export default {
    /**
 * mask遮罩
 * @description mask遮罩蒙层。
 * @property {number} top 距离顶部距离，适用于顶部有筛选菜单时使用
 * @property {Boolean} noclickhide 点击蒙层是否关闭mask
 * @property {Boolean} isflexcenter flex布局，center
 * @property {Boolean} hindex 层级
 */
    props: ['top', 'noclickhide', 'isflexcenter', 'hindex'],
    data() {
        return {
            isShow: false,
            showBgk: false,
        }
    },
    onLoad() {
        this.isShow = false

    },
    methods: {
        moveHandle() {
            return
        },
        hideMask(e) {
            if (!e) {
                this.toHideMask()
            } else if (e.target.id === 'mask') {
                if (this.noclickhide) return // msak 点击屏蔽
                this.toHideMask()
            }
        },
        showMask() {
            this.showBGK()
            this.$emit('onShowHander')
        },
        toHideMask() {
            this.hideBGK()
            this.$emit('onHideHander')
        },
        showBGK() {
            this.isShow = true
            let _this = this
            setTimeout(() => {
                _this.showBgk = true
            }, 100)
        },
        hideBGK() {
            let _this = this
            setTimeout(() => {
                this.showBgk = false
                setTimeout(() => {
                    _this.isShow = false
                }, 200)
            }, 100)
        }
    }
}
</script>
<style scoped lang="scss">
.mask {
    width: 100%;
    height: 100%;
    position: fixed;
    left: 0;
    z-index: 999;
    opacity: 0;
    transition: all 0.2s;
}
.active {
    opacity: 1;
    background-color: rgba(0, 0, 0, 0.5);
}
.hz_index {
    z-index: 1111;
}
.container_flex {
    display: flex;
    display: -webkit-flex;
}
</style>