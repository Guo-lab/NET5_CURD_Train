/** 此处以编程方式动态返回pages配置数据，用于覆盖pages.json . 这是uni-app提供的钩子*/

let pagesJsonMerged = require('./router/buildRouter.js');
module.exports = (pagesJson, loader) => {//此处pagesJson是pages.json加载后的对象
    pagesJson.pages = pagesJsonMerged.pages;
    return pagesJson;
}