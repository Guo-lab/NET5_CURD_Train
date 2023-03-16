import Vue from 'vue';
import scan from "./uni_modules/p-scan_1.0.2/components/p-scan/scan.vue";
import App from './App.vue';
import { pbConfig } from './projectbase/ProjectBaseConfig';


Vue.config.productionTip = false;
Vue.component('scan', scan);
pbConfig.UrlContextPrefix = 'http://localhost:27710/Primary01';
//pbConfig.UrlContextPrefix = 'http://192.168.102.28:27710/Primary01';
pbConfig.UrlMappingPrefix = '';
pbConfig.TokenChannel = 'Cookie';
new App().$mount();

