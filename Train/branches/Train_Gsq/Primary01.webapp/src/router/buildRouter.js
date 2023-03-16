/** 此脚本从各模块目录下读取pages配置数据，进行合并。*/

let pagesJson = {
    pages: []
};
let p = require('../App_ProjectHierarchy.js');
p.Modules.forEach((item) => {
	const moduleRouter = require(`../pages/${item}/${item}-routing.js`);
	moduleRouter.forEach((subItem) => {
        pagesJson.pages.push(subItem);
	})
});
//不可使用export default
module.exports=pagesJson;