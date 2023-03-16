/* Kendo UI Localization Project for v2012.3.1114 
* Copyright 2012 Telerik AD. All rights reserved.
* 
* Simplified Chinese (zh-CN) Language Pack
*
* Project home  : https://github.com/loudenvier/kendo-global
* Kendo UI home : http://kendoui.com
* Author        : IKKI Phoenix  
*                 
*
* This project is released to the public domain, although one must abide to the 
* licensing terms set forth by Telerik to use Kendo UI, as shown bellow.
*
* Telerik's original licensing terms:
* -----------------------------------
* Kendo UI Web commercial licenses may be obtained at
* https://www.kendoui.com/purchase/license-agreement/kendo-ui-web-commercial.aspx
* If you do not own a commercial license, this file shall be governed by the
* GNU General Public License (GPL) version 3.
* For GPL requirements, please review: http://www.gnu.org/copyleft/gpl.html
*/

kendo.culture("zh-CN"); // Add by IKKI
kendo.ui.Locale = "Simplified Chinese (zh-CN)";
//Grid message by Ziff
kendo.ui.Grid.prototype.options.messages =
    $.extend(kendo.ui.Grid.prototype.options.messages, {
        commands: {
            edit: "编辑",
            destroy: "删除",
            cancel: "取消",
            update: "保存",
            create: "新增",
            canceledit: "取消",
            save: "保存",
            excel: "导出Excel",
            pdf: "导出pdf"
        },
        editable: {
            cancelDelete: "取消",
            confirmDelete: "删除",
            confirmation: "你确定删除这条记录吗?"
        }
    })
//End
//Tree List message by Ziff
kendo.ui.TreeList.prototype.options.messages =
    $.extend(true, kendo.ui.TreeList.prototype.options.messages, {
        "noRows": "无相关数据",
        "loading": "载入中...",
        "requestFailed": "请求失败！",
        "retry": "重试",
        "commands": {
            "edit": "编辑",
            "update": "更新",
            "canceledit": "取消",
            "create": "新增",
            "createchild": "新增子项",
            "destroy": "删除",
            "excel": "导出 Excel",
            "pdf": "导出 PDF"
        }
    });
//End
//Spreadsheet message by Ziff
kendo.spreadsheet.messages.colorPicker =
   $.extend(true, kendo.spreadsheet.messages.colorPicker, {
       "reset": "重置颜色",
       "customColor": "其它颜色...",
       "apply": "应用",
       "cancel": "取消"
   });

kendo.spreadsheet.messages.borderPalette =
   $.extend(true, kendo.spreadsheet.messages.borderPalette, {
       //"nameSpace": "Message"
       "allBorders": "全边框",
       "insideBorders": "内部边框",
       "insideHorizontalBorders": "内部横向边框",
       "insideVerticalBorders": "内部纵向边框",
       "outsideBorders": "外部边框",
       "leftBorder": "左边框",
       "topBorder": "上边框",
       "rightBorder": "右边框",
       "bottomBorder": "下边框",
       "noBorders": "无边框"
   });
kendo.spreadsheet.messages.dialogs =
    $.extend(true, kendo.spreadsheet.messages.dialogs, {
        //"nameSpace": "Message"
        "apply": "应用",
        "save": "保存",
        "cancel": "取消",
        "remove": "删除",
        "okText": "确定",
        "retry": "重试",
        "revert": "撤销",
        "formatCellsDialog": {
            "title": "格式",
            "categories": {
                "number": "数字",
                "currency": "货币",
                "date": "日期"
            }
        },
        "fontFamilyDialog": {
            "title": "字体"
        },
        "fontSizeDialog": {
            "title": "字体大小"
        },
        "bordersDialog": {
            "title": "边框"
        },
        "alignmentDialog": {
            "title": "对齐",
            "buttons": {
                "justtifyLeft": "左对齐",
                "justifyCenter": "居中",
                "justifyRight": "右对齐",
                "justifyFull": "两端对齐",
                "alignTop": "顶端对齐",
                "alignMiddle": "垂直居中",
                "alignBottom": "底端对齐"
            }
        },
        "confirmationDialog": {
            "text": "你确定删除此工作表?",
            "title": "工作表删除"
        },
        "duplicateSheetNameDialog": {
            "errorMessage": "工作表名重复"
        },
        "mergeDialog": {
            "title": "合并单元格",
            "buttons": {
                "mergeCells": "全部合并",
                "mergeHorizontally": "横向合并",
                "mergeVertically": "纵向合并",
                "unmerge": "取消合并"
            }
        },
        "freezeDialog": {
            "title": "冻结窗格",
            "buttons": {
                "freezePanes": "冻结窗格",
                "freezeRows": "冻结行",
                "freezeColumns": "冻结列",
                "unfreeze": "取消冻结"
            }
        },
        "validationDialog": {
            "title": "数据验证",
            "hintMessage": "",
            "hintTitle": "",
            "criteria": {
                "any": "任意值",
                "list": "序列",
                "number": "数字",
                "text": "文本",
                "date": "日期",
                "custom": "自定义公式"
            },
            "comparers": {
                "greaterThan": "大于",
                "lessThan": "小于",
                "between": "介于",
                "notBetween": "未介于",
                "equalTo": "等于",
                "notEqualTo": "不等于",
                "greaterThanOrEqualTo": "大于等于",
                "lessThanOrEqualTo": "小于等于"
            },
            "comparerMessages": {
                "greaterThan": "大于{0}",
                "lessThan": "小于{0}",
                "between": "介于{0}和{1}",
                "notBetween": "未介于{0}和{1}",
                "equalTo": "等于{0}",
                "notEqualTo": "不等于{0}",
                "greaterThanOrEqualTo": "大于等于{0}",
                "lessThanOrEqualTo": "小于等于{0}",
                "custom": "符合公式: {0}"
            },
            "labels": {
                "criteria": "条件",
                "ignoreBlank": "忽略空白",
                "comparer": "比较",
                "showCalendarButton": "显示日期选择按钮",
                "showListButton": "显示下拉箭头",
                "min": "最小",
                "max": "最大",
                "value": "值",
                "start": "开始",
                "end": "结束",
                "onInvalidData": "对于非法数据",
                "rejectInput": "拒绝输入",
                "showWarning": "显示警告",
                "showHint": "显示提示",
                "hintTitle": "提示标题",
                "hintMessage": "提示消息"
            },
            "placeholders": {
                "typeTitle": "输入标题",
                "typeMessage": "输入消息"
            }
        },
        "exportAsDialog": {
            "title": "另存为...",
            "labels": {
                "center": "居中",
                "exportArea": "导出",
                "fileName": "文件名",
                "fit": "适合页",
                "guidelines": "装订线",
                "horizontally": "横向",
                "margins": "边距",
                "orientation": "纸张方向",
                "paperSize": "纸张大小",
                "print": "打印",
                "saveAsType": "文件类型",
                "scale": "尺寸",
                "verticaly": "纵向"
            }
        },
        "incompatibleRangesDialog": {
            "errorMessage": "不兼容的区域"
        },
        "linkDialog": {
            "labels": {
                "removeLink": "删除链接",
                "text": "文字",
                "url": "地址"

            },
            "title": "超链接"
        },
        "noFillDirectionDialog": {
            "errorMessage": "无法确定填充方向"
        },
        "overflowDialog": {
            "errorMessage": "无法粘贴，因为复制的区域和要粘贴的区域大小和形状不同"
        },
        "rangeDisabledDialog": {
            "errorMessage": "目的区域包含禁用的单元格"
        },
        "modifyMergedDialog": {
            "errorMessage": "不能修改合并的单元格的一部分"
        },
        "useKeyboardDialog": {
            "title": "复制和粘贴",
            "errorMessage": "这些操作无法通过菜单完成，请用键盘快捷键代替:",
            "labels": {
                "forCopy": "复制",
                "forCut": "剪切",
                "forPaste": "粘贴"
            }
        },
        "unsupportedSelectionDialog": {
            "errorMessage": "操作无法在多重选择上完成."
        }
    });

$.extend(true, kendo.spreadsheet.messages.filterMenu, {
    //"nameSpace": "Message"
    "addToCurrent": "添加到当前选择",
    "sortAscending": "升序",
    "sortDescending": "降序",
    "filterByValue": "按值查找",
    "filterByCondition": "按条件查找",
    "apply": "应用",
    "search": "查找",
    "clear": "清空",
    "blanks": "(空白)",
    "operatorNone": "无",
    "and": "并且",
    "or": "或者",
    "operators": {
        "string": {
            "contains": "包含",
            "doesnotcontain": "不包含",
            "startswith": "开头是",
            "endswith": "结尾是",
            "doesnotmatch": "不匹配",
            "matches": "匹配"
        },
        "date": {
            "eq": "等于",
            "neq": "不等于",
            "lt": "早于",
            "gt": "晚于"
        },
        "number": {
            "eq": "等于",
            "neq": "不等于",
            "gte": "大于等于",
            "gt": "大于",
            "lte": "小于等于",
            "lt": "小于"
        }
    }
});

$.extend(true, kendo.spreadsheet.messages.toolbar, {
    //"nameSpace": "Message"
    "addColumnLeft": "在左侧插入列",
    "addColumnRight": "在右侧插入列",
    "addRowAbove": "在上方插入行",
    "addRowBelow": "在下方插入行",
    "alignment": "对齐",
    "alignmentButtons": {
        "justtifyLeft": "左对齐",
        "justifyCenter": "居中",
        "justifyRight": "右对齐",
        "justifyFull": "两端对齐",
        "alignTop": "顶端对齐",
        "alignMiddle": "垂直居中",
        "alignBottom": "底端对齐"
    },
    "backgroundColor": "背景色",
    "bold": "加粗",
    "borders": "边框",
    "copy": "复制",
    "cut": "剪切",
    "deleteColumn": "删除列",
    "deleteRow": "删除行",
    "exportAs": "导出",
    "filter": "筛选",
    "fontFamily": "字体",
    "fontSize": "字体大小",
    "format": "自定义格式...",
    "formatTypes": {
        "automatic": "自动",
        "number": "数值",
        "percent": "百分比",
        "financial": "会计专用",
        "currency": "货币",
        "date": "日期",
        "time": "时间",
        "dateTime": "日期时间",
        "duration": "时间段",
        "text": "文本",
        "moreFormats": "更多格式..."
    },
    "formatDecreaseDecimal": "减少小数位",
    "formatIncreaseDecimal": "增加小数位",
    "freeze": "冻结窗格",
    "freezeButtons": {
        "freezePanes": "冻结窗格",
        "freezeRows": "冻结行",
        "freezeColumns": "冻结列",
        "unfreeze": "取消冻结"
    },
    "hyperlink": "链接",
    "italic": "斜体",
    "merge": "合并单元格",
    "mergeButtons": {
        "mergeCells": "全部合并",
        "mergeHorizontally": "合并列",
        "mergeVertically": "合并行",
        "unmerge": "取消合并"
    },
    "open": "打开...",
    "paste": "粘贴",
    "quickAccess": {
        "redo": "重做",
        "undo": "撤销"
    },
    "sort": "排序",
    "sortButtons": {
        "sortRangeAsc": "升序",
        "sortRangeDesc": "降序"
    },
    "textColor": "文本颜色",
    "textWrap": "折行",
    "toggleGridlines": "网格线",
    "underline": "下划线",
    "validation": "数据有效性"
});

$.extend(true, kendo.spreadsheet.messages.view, {
    "nameBox": "名称框",
    "errors": {
        "cannotModifyDisabled": "无法修改禁用的单元格",
        "cantSortMixedCells": "包含混合形状的区域无法排序",
        "cantSortMultipleSelection": "多重选择无法排序",
        "cantSortNullRef": "空白选择无法排序",
        "filterRangeContainingMerges": "包含合并单元格的区域无法筛选",
        "insertColumnWhenRowIsSelected": "所有列都已选中无法再插入列",
        "insertRowWhenColumnIsSelected": "所有行都已选中无法再插入行",
        "openUnsupported": "无法识别的格式，请选择.xlsx文件",
        "shiftingNonblankCells": "数据可能丢失，无法插入单元格。 请选择其它插入位置或从工作表的底部删除行。",
        "sortRangeContainingMerges": "包含合并单元格的区域无法排序",
        "validationError": "您输入的值违反此单元格设定的规则"

    },
    "tabs": {
        "home": "开始",
        "insert": "插入",
        "data": "数据"
    }
});
//end
kendo.ui.ColumnMenu.prototype.options.messages =
	$.extend(kendo.ui.ColumnMenu.prototype.options.messages, {

	    /* COLUMN MENU MESSAGES 
         ****************************************************************************/
	    sortAscending: "升序排列",
	    sortDescending: "降序排列",
	    filter: "筛选",
	    columns: "字段列"
	    /***************************************************************************/
	});

kendo.ui.Groupable.prototype.options.messages =
	$.extend(kendo.ui.Groupable.prototype.options.messages, {

	    /* GRID GROUP PANEL MESSAGES 
         ****************************************************************************/
	    empty: "将字段列名称拖拽到此处可进行该列的分组显示"
	    /***************************************************************************/
	});

kendo.ui.FilterMenu.prototype.options.messages =
	$.extend(kendo.ui.FilterMenu.prototype.options.messages, {

	    /* FILTER MENU MESSAGES 
         ***************************************************************************/
	    info: "筛选条件：",	// sets the text on top of the filter menu
	    filter: "筛选",		// sets the text for the "Filter" button
	    clear: "清空",		// sets the text for the "Clear" button
	    // when filtering boolean numbers
	    isTrue: "是",		// sets the text for "isTrue" radio button
	    isFalse: "否",		// sets the text for "isFalse" radio button
	    //changes the text of the "And" and "Or" of the filter menu
	    and: "并且",
	    or: "或者",
	    selectValue: "-= 请选择 =-"
	    /***************************************************************************/
	});

kendo.ui.FilterMenu.prototype.options.operators =
	$.extend(kendo.ui.FilterMenu.prototype.options.operators, {

	    /* FILTER MENU OPERATORS (for each supported data type) 
         ****************************************************************************/
	    string: {
	        eq: "等于",
	        neq: "不等于",
	        contains: "包含",
	        doesnotcontain: "不包含",
	        startswith: "开始于",
	        endswith: "结束于"
	    },
	    number: {
	        eq: "等于",
	        neq: "不等于",
	        gt: "大于",
	        gte: "大于等于",
	        lt: "小于",
	        lte: "小于等于"
	    },
	    date: {
	        eq: "等于",
	        neq: "不等于",
	        gt: "晚于",
	        gte: "晚于等于",
	        lt: "早于",
	        lte: "早于等于"
	    },
	    enums: {
	        eq: "等于",
	        neq: "不等于"
	    }
	    /***************************************************************************/
	});

kendo.ui.Pager.prototype.options.messages =
	$.extend(kendo.ui.Pager.prototype.options.messages, {

	    /* PAGER MESSAGES 
         ****************************************************************************/
	    display: "{0} - {1} 条　共 {2} 条数据",
	    empty: "无数据",
	    page: "转到第",
	    of: "页　共 {0} 页",
	    itemsPerPage: "条每页",
	    first: "首页",
	    previous: "上一页",
	    next: "下一页",
	    last: "尾页",
	    refresh: "刷新"
	    /***************************************************************************/
	});

kendo.ui.Validator.prototype.options.messages =
	$.extend(kendo.ui.Validator.prototype.options.messages, {

	    /* VALIDATOR MESSAGES 
         ****************************************************************************/
	    required: "{0} 是必填项！",
	    pattern: "{0} 的格式不正确！",
	    min: "{0} 必须大于或等于 {1} ！",
	    max: "{0} 必须小于或等于 {1} ！",
	    step: "{0} 不是正确的步进值！",
	    email: "{0} 不是正确的电子邮件！",
	    url: "{0} 不是正确的网址！",
	    date: "{0} 不是正确的日期！"
	    /***************************************************************************/
	});

// The upload part add by IKKI
kendo.ui.Upload.prototype.options.localization =
	$.extend(kendo.ui.Upload.prototype.options.localization, {

	    /* UPLOAD LOCALIZATION
         ****************************************************************************/
	    select: "选择文件",
	    dropFilesHere: "将文件拖拽到此处上传",
	    cancel: "取消",
	    remove: "移除",
	    uploadSelectedFiles: "上传文件",
	    statusUploading: "上传中……",
	    statusUploaded: "上传成功！",
	    statusFailed: "上传失败！",
	    retry: "重试"
	    /***************************************************************************/
	});

kendo.ui.ImageBrowser.prototype.options.messages =
	$.extend(kendo.ui.ImageBrowser.prototype.options.messages, {

	    /* IMAGE BROWSER MESSAGES 
         ****************************************************************************/
	    uploadFile: "上传文件",
	    orderBy: "排序方式",
	    orderByName: "按名称排序",
	    orderBySize: "按大小排序",
	    directoryNotFound: "目录未找到",
	    emptyFolder: "空文件夹",
	    deleteFile: '你确定要删除【{0}】这个文件吗？',
	    invalidFileType: "你上传的文件格式 {0} 是无效的，支持的文件类型为：{1}",
	    overwriteFile: "一个名为【{0}】的文件已经存在，是否覆盖？",
	    dropFilesHere: "将文件拖拽到此处上传"
	    /***************************************************************************/
	});

kendo.ui.Editor.prototype.options.messages =
	$.extend(kendo.ui.Editor.prototype.options.messages, {

	    /* EDITOR MESSAGES 
         ****************************************************************************/
	    bold: "粗体",
	    italic: "斜体",
	    underline: "下划线",
	    strikethrough: "删除线",
	    superscript: "上标",
	    subscript: "下标",
	    justifyCenter: "居中对齐",
	    justifyLeft: "左对齐",
	    justifyRight: "右对齐",
	    justifyFull: "两端对齐",
	    insertUnorderedList: "插入无序列表",
	    insertOrderedList: "插入有序列表",
	    indent: "增加缩进",
	    outdent: "减少缩进",
	    createLink: "插入链接",
	    unlink: "删除链接",
	    insertImage: "插入图片",
	    insertHtml: "插入HTML",
	    fontName: "请选择字体",
	    fontNameInherit: "（默认字体）",
	    fontSize: "请选择字号",
	    fontSizeInherit: "（默认字号）",
	    formatBlock: "格式",
	    foreColor: "文字颜色",
	    backColor: "文字背景色",
	    style: "样式",
	    emptyFolder: "空文件夹",
	    uploadFile: "上传文件",
	    orderBy: "排序方式：",
	    orderBySize: "按大小排序",
	    orderByName: "按名称排序",
	    invalidFileType: "你上传的文件格式 {0} 是无效的，支持的文件类型为：{1}",
	    deleteFile: '你确定要删除【{0}】这个文件吗？',
	    overwriteFile: '一个名为【{0}】的文件已经存在，是否覆盖？',
	    directoryNotFound: "目录未找到",
	    imageWebAddress: "图片链接地址",
	    imageAltText: "图片占位符",
	    linkWebAddress: "链接地址",
	    linkText: "链接文字",
	    linkToolTip: "文字提示",
	    linkOpenInNewWindow: "是否在新窗口中打开",
	    dialogInsert: "插入",
	    dialogButtonSeparator: "或",
	    dialogCancel: "取消"
	    /***************************************************************************/
	});
