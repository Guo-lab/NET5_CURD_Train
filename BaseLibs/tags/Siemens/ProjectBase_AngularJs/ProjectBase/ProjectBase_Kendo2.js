(function (String, angular) {
    'use strict';

    var pbm = angular.module('projectbase');

    var kendoFn2 = ['pb', '$parse', '$window', function (pb, $parse, $window) {
        var GridClientDataSource = function (result, pageSize, schema, sort) {
            var ds = {
                data: result
            }
            if (pageSize != undefined) { ds.pageSize = pageSize; }

            if (schema) {
                ds.schema = schema;
            }
            if (sort) {
                ds.sort = sort;
            }
            return new kendo.data.DataSource(ds);
        }

        var GridServerDataSource = function (scope, kendoGridName, boundData, form,  beforeBinding) {
            if (!boundData) throw new Error('参数boundData不可为空');
            if (!boundData.ResultList) {
                boundData.ResultList = {};
            }
            if (beforeBinding) {
                beforeBinding(boundData.ResultList);
            }
            if (!boundData.Input) {
                boundData.Input={ ListInput: { Pager: { PageSize: 0, PageNum: 1 }, OrderExpression: null }};
            }
            var listInput0 = boundData.Input;
            var listInputPrefix = 'Input';
            var pagerPrefix = 'Input.Pager';
            if (boundData.Input && boundData.Input.ListInput) {
                listInput0 = boundData.Input.ListInput;
                listInputPrefix = 'Input.ListInput';
                pagerPrefix = listInputPrefix + '.Pager';
            }
            var pageSize = listInput0.Pager.PageSize;
            var pageNum = listInput0.Pager.PageNum;

            function initSort(sort) {
                if (typeof (sort) == 'string') {
                    var pairs = sort.split(',');
                    sort = [];
                    angular.forEach(pairs, function (v, i) {
                        var parts = v.split(' ');
                        parts[1] = parts[1] || 'asc';
                        sort[i] = { field: parts[0], dir: parts[1] }
                    });
                }
                return sort;
            }
            var sort = initSort(listInput0.OrderExpression);
            var ds = {
                transport: {
                    read: function (options) {
                        var kendoGrid = $parse(kendoGridName)(scope);
                        kendoGrid.pbBoundData = boundData;
                        var listInput = boundData.Input;
                        if (boundData.Input && boundData.Input.ListInput) {
                            listInput = boundData.Input.ListInput;
                        }
                        if (!options.data.sort && listInput.OrderExpression) {
                            options.data.sort = initSort(listInput.OrderExpression);
                        }
                        if (kendoGrid.pbAdjustPageNum == true) {//因调整页号引起read
                            kendoGrid.pbAdjustPageNum = false;
                            options.success(kendoGrid.pbBoundData);
                            return;
                        }
                        if (kendoGrid.pbUseBoundData) {//列表数据从绑定的数据中取
                            kendoGrid.pbUseBoundData = false;
                            options.success(kendoGrid.pbBoundData);
                            return;
                        }
                        //分页获取
                        listInput.Pager.PageSize = options.data.pageSize;
                        listInput.Pager.PageNum = options.data.page;
                        var additionalData = pagerPrefix + ".PageSize=" + options.data.pageSize + "&" + pagerPrefix + ".PageNum=" + options.data.page;
                        if (options.data.sort) {
                            var expr = options.data.sort.map(function (x) {
                                return x.field + " " + x.dir;
                            }).join(",");
                            additionalData = additionalData + "&" + listInputPrefix + ".OrderExpression=" + expr;
                            listInput.OrderExpression = expr;
                        }
                        return pb.AjaxSubmit(form, null, { data: additionalData }, function (r) {
                            boundData.ResultList = r.data.ResultList;
                            listInput = r.data.Input.ListInput;
                            if (options.data.page != listInput.Pager.PageNum) {
                                kendoGrid.pbAdjustPageNum = true;
                                kendoGrid.dataSource.page(listInput.Pager.PageNum);
                            }
                            if (beforeBinding) {
                                beforeBinding(r.data.ResultList);//?r.Input
                            }
                            options.success(r.data);
                        });
                    }
                },
                serverFiltering: true,
                serverSorting: true,
                serverPaging: true,
                pageSize: pageSize,
                page: pageNum,
                schema: {
                    data: "ResultList",
                    total: pagerPrefix + ".ItemCount",
                    model: {
                        id: "Id"
                    }
                },
                sort: sort
            }
            //if (group) {
            //    ds.group = group;
            //}
            return new kendo.data.DataSource(ds);
        }
        /**
         * @param {any[]} colsettings 列设置,数组，元素为字符串或对象。
         *                              如果是字符串，可以包含逗号分隔的三部分，第一部分是字段名，第二部分是模板，第三部分是宽度，
         *                                   如'Name,T,80px',其中第二部分可取值为：（空表示不用模板）
         *                                      1.T表示使用模板且模板名为Name-template.
         *                                      2 OT表示模板名为Operation-template
         *                                      3.|Display表示对值进行转换
         *                                      4.其它字符串值表示模板名。
         *                              如果是对象则同kendo的列设置对象，如{ field: "", width: "200px", attributes: { style: "text-align: center" }, template: kendo.template($('#Operation-template').html()) }
         *                              如果是kendo的列设置对象，其中template可简写为't',则表示使用约定模板名
         * @param {any} opn 配置对象,类型为{
         *                               toolbarTpl:string|false='Toolbar-template',
         *                               heightMinus:窗口高度-85后再减一个数
         *                               heightFunc:(windowHeight)=>number 用于计算高度的函数，参数为窗口高度
         *                               其它同原kendo的配置属性
         *                               autoBind: bool=true,
        *                               pageable: {
        *                                   pageSizes: number[]=[5, 10, 20],
        *                                   buttonCount: number=5,
        *                                   numeric: bool=true
        *                               },
        *                               scrollable: bool=true,
        *                               sortable: any,
         * 
         */
        var BuildGridOptions = function (colsettings, opn) {
            var CreateEditor = function (template) {
                var Editor = function (container, options) {
                    $($("#" + template).html()).appendTo(container);
                }
                return Editor;
            }

            var defaultOption = {
                toolbarTpl: 'Toolbar-template',
                pageable: {
                    pageSizes: [5, 10, 20],
                    buttonCount: 5,
                    numeric: true
                },
                defaultPageSize: 20,
                scrollable: true,
                resizable: true
                //sortable: {
                //    showIndexes: true,
                //    mode: "multiple"
                //}
            }
            if (opn == undefined) {
                opn = {};
            }
            opn = angular.extend({}, defaultOption, opn);
            
            if (opn.heightMinus) {
                opn.height = $(window).height() - 135 - opn.heightMinus;
            } else if (opn.heightFunc) {
                opn.height = opn.heightFunc($(window).height());
            } else {
                opn.height = $(window).height() - 135;
            }

            var columns = [];
            for (var i = 0; i < colsettings.length; i++) {
                var setting = colsettings[i];
                if (typeof (setting) == 'string') {//'Name,T,80px|{},ET'
                    setting = setting.split(',');
                    if (setting.length < 4) { setting.length = 4; }
                    columns[i] = { field: setting[0] };
                    if (setting[1]) {
                        columns[i].template = setting[1];
                    }
                    if (setting[2]) {//80px|left|{style: 'text-align: right'...}
                        var attrs = setting[2].split('|');
                        for (var k = 0; k <= attrs.length - 1; k++) {
                            var attr = attrs[k].toLowerCase();
                            if (attr == 'left' || attr == 'right' || attr == 'center') {
                                columns[i].attributes = columns[i].attributes || {};
                                columns[i].attributes.style = 'text-align: ' + attr;
                            } else if (attr.startsWith('{')) {
                                columns[i].attributes = $parse(attr)();
                            } else {
                                columns[i].width = attr;
                            }
                        }
                    }
                    if (setting[3]) {
                        columns[i].editor = setting[3];
                    }
                } else {
                    columns[i] = setting;
                }
                if (columns[i].template) {
                    if (columns[i].template.toLowerCase() == 't') {
                        columns[i].template = kendo.template($('#' + columns[i].field + '-template').html());
                    } else if (columns[i].template.toLowerCase() == 'ot') {
                        columns[i].template = kendo.template($('#Operation-template').html());
                    } else if (columns[i].template.startsWith('|')) {
                        //columns[i].template中可以有单引号但不能有双引号
                        columns[i].template = '<span ng-bind="dataItem.' + columns[i].field + columns[i].template + '"></span>';
                    } else {
                        columns[i].template = kendo.template($('#' + columns[i].template).html());
                    }
                }
                if (columns[i].editor) {
                    if (columns[i].editor.toLowerCase() == 'et') {
                        columns[i].editor = CreateEditor(columns[i].field + "-Editor-template");
                    }
                }
                if (columns[i].command) {
                    angular.forEach(columns[i].command, function (item) {
                        if (item.text) {
                            if (!item.name) { item.name = item.text;}
                            item.text = "<span translate='" + item.text + "'></span";
                        }
                        if (item.iconClass) {
                            item.iconClass = "k-icon k-i-" + item.iconClass;
                        }
                        if (item.click) {
                            var tmpClick = item.click;
                            item.click = function (e) {
                                e.preventDefault();
                                e.dataItem = this.dataItem($(e.currentTarget).closest("tr"));
                                tmpClick(e);
                            }
                        }
                    })
                    if (columns[i].command.find(function (x) { return x == "edit" || x.name=="edit" }) == undefined) {
                        columns[i].command.unshift("edit");
                    }
                }
            }
            opn.columns = columns;
            if (opn.toolbarTpl) {
                opn.toolbar = kendo.template($('#' + opn.toolbarTpl).html());
            }
            if (opn.editable) {
                opn.cancel = function (e) {
                    if (e.model.Id > 0) {
                        e.sender.dataSource.cancelChanges();
                        e.sender.refresh();
                    }
                }

                var cmdColumn = opn.columns.find(function (x) { return x.command });
                if (opn.dataBound) {
                    var tmpDataBound = opn.dataBound;
                    opn.dataBound = function (e) {
                        var grid = e.sender;
                        var gridData = grid.dataSource.view();
                        for (var i = 0; i < gridData.length; i++) {
                            var currentUid = gridData[i].uid;
                            var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                            var evt = {
                                sender: grid, data: gridData[i], container: $(currentRow), buttons: {}
                            };
                            angular.forEach(cmdColumn.command, function (col) {
                                if (col == "edit" || col.name=="edit") {
                                    evt.buttons.Edit = evt.container.find(".k-grid-edit");
                                } else if (col.name) {
                                    evt.buttons[col.name] = evt.container.find(".k-grid-" + col.name);
                                }
                            })
                            tmpDataBound(evt);
                        }
                    };
                } else {
                    opn.dataBound = function (e) {
                        var grid = e.sender;
                        var gridData = grid.dataSource.view();
                        for (var i = 0; i < gridData.length; i++) {
                            var currentUid = gridData[i].uid;
                            var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                            angular.forEach(cmdColumn.command, function (col) {
                                if (angular.isFunction(col.visible)) {
                                    var button = $(currentRow).find(".k-grid-" + col.name);
                                    if (!col.visible.call(pb.GetMyController(button),gridData[i])) {
                                        button.hide();
                                    }
                                }
                            })
                        }
                    };
                }
                if (opn.save) {
                    var tmpSave = opn.save;
                    opn.save = function (e) {
                        e.preventDefault();
                        tmpSave(e).then(function (result) {
                            if (result == undefined || result.isRcResult)
                                e.sender.saveChanges();
                        }, function (reason) {

                        });
                    }
                }
            }

            return opn;
        }
        return {
            GridClientDataSource: GridClientDataSource,
            GridServerDataSource: GridServerDataSource,
            BuildGridOptions: BuildGridOptions,
        };
    }];

    pbm.provider('kendoui2', function () {
        var me = this;
        me.$get = kendoFn2;
    });

    pbm.directive("pbKendoClientGrid", ['$parse', 'kendoui2', function ($parse, kendoui2) {
        var directiveDefinitionObject = {
            priority: 0,
            restrict: "A",
            require: ["kendoGrid"],
            compile: function compile(ele, attrs, transclude) {
                return {
                    pre: function preLink(scope, ele, attrs, ctrls) {
                        var opn = scope.$eval(attrs['pbKendoGridOptions']);
                        if (opn == undefined) {
                            opn = {};
                        }
                        if (opn.pageable == undefined) {
                            opn.pageable = {
                                pageSizes: [5, 10, 20],
                                buttonCount: 5,
                                numeric: true
                            };
                        }
                        var defaultPageSize = (opn.pageable && opn.defaultPageSize) ? opn.defaultPageSize : 20;
                        var schema = scope.$eval(attrs['pbKendoGridSchema']);
                        if (schema) {
                            var convertedSchema = {};
                            for (var field in schema) {
                                if (angular.isObject(schema[field])) {
                                    convertedSchema[field] = schema[field];
                                } else {
                                    convertedSchema[field] = { type: schema[field] };
                                }
                            }
                            schema = {
                                fields: convertedSchema
                            };
                            
                        }
                        if (opn.editable) {
                            schema.fields["Id"] = { type: "number", defaultValue: 0 };
                            angular.extend(schema, { id: "Id" });
                        }
                        if (schema) {
                            schema = { model: schema };
                        }
                                               
                        var defaultSort = opn.defaultSort ? opn.defaultSort : undefined;
                        var boundData = $parse(attrs['pbKendoClientGrid'])(scope);
                        var dataSource = attrs['kDataSource'].split(".");
                        if (dataSource.length == 1) {
                            scope[dataSource] = kendoui2.GridClientDataSource(boundData, defaultPageSize, schema, defaultSort);
                        }
                        else {
                            scope[dataSource[0]][dataSource[1]] = kendoui2.GridClientDataSource(boundData, defaultPageSize, schema, defaultSort);
                        }

                        var cols = scope.$eval(attrs['pbKendoGridCols']);
                        cols = cols || opn;
                        if (!angular.isArray(cols)) throw new Error('kendogrid列设置错误，请提供数组数据');
                        if (opn && !angular.isObject(opn)) {
                            throw new Error('pbKendoGridOptions设置错误，请提供对象数据');
                        }
                        //Client grid 通常应该是true，即页面加载时VM已经通过Action返回并自动绑定
                        if (opn.autoBind == undefined) { opn.autoBind = true; }
                        scope[attrs['options']] = kendoui2.BuildGridOptions(cols, opn);
                    },
                    post: function postLink(scope, ele, attrs, ctrls) {
                        scope.$on('kendoWidgetCreated', function (event, widget) {
                            var kendoGrid = $parse(attrs['kendoGrid'])(scope);
                            if (widget != kendoGrid) return;
                            kendoGrid.filter = function (filter) {
                                kendoGrid.dataSource.filter(filter);
                            }
                        });
                        scope.$on('kendoRendered', function () {
                            var kendoGrid = $parse(attrs['kendoGrid'])(scope);
                            if (!kendoGrid) {
                                throw new Error('kendo grid创建失败，请检查属性/参数设置是否正确、列设置的列数与tablehead中的列数是否一致');
                            }
                        });
                    }
                };
            }
        };
        return directiveDefinitionObject;
    }]);

    pbm.directive("pbKendoGrid", ['$parse', 'kendoui2', function ($parse, kendoui2) {
        var directiveDefinitionObject = {
            priority: 0,
            restrict: "A",
            require: ["kendoGrid"],
            compile: function compile(ele, attrs, transclude) {
                return {
                    pre: function preLink(scope, ele, attrs, ctrls) {
                        var boundData = $parse(attrs['pbKendoGrid'])(scope);
                        var formName = ele.controller('form').$name;
                        var beforeBinding = $parse(attrs["beforeBinding"])(scope);
                        var dataSource = attrs['kDataSource'].split(".");
                        if (dataSource.length == 1) {
                            scope[attrs['kDataSource']] = kendoui2.GridServerDataSource(scope, attrs['kendoGrid'], boundData, formName,beforeBinding);
                        } else {
                            scope[dataSource[0]][dataSource[1]] = kendoui2.GridServerDataSource(scope, attrs['kendoGrid'], boundData, formName,beforeBinding);
                        }
                        var cols = scope.$eval(attrs['pbKendoGridCols']);
                        var opn = scope.$eval(attrs['pbKendoGridOptions']);
                        opn = opn || {};
                        cols = cols || opn;
                        if (!angular.isArray(cols)) throw new Error('kendogrid列设置错误，请提供数组数据');
                        if (opn && !angular.isObject(opn)) {
                            throw new Error('pbKendoGridOptions设置错误，请提供对象数据');
                        }
                        //Server grid必须是false，绑定数据的grid没有autoBind的概念，因为有数据肯定要bind。而且如果autoBind=true，将导致grid对象在read后才加到scope中
                        opn.autoBind = false;
                        scope[attrs['options']] = kendoui2.BuildGridOptions(cols, opn);
                    },
                    post: function postLink(scope, ele, attrs, ctrls) {
                        var boundData = $parse(attrs['pbKendoGrid'])(scope);
                        var opn = scope.$eval(attrs['pbKendoGridOptions']);
                        var notAutoBind = opn && opn.autoBind==false;
                        scope.$on('kendoWidgetCreated', function (event, widget) {
                            var kendoGrid = $parse(attrs['kendoGrid'])(scope);
                            if (widget != kendoGrid) return;
                            kendoGrid.pbUseBoundData = true;
                            kendoGrid.AjaxRead = function () {
                                kendoGrid.pbUseBoundData = false;
                                kendoGrid.dataSource.read();
                            }
                            kendoGrid.Bind = function (bindData) {
                                if (bindData.ResultList) {
                                    boundData.ResultList = bindData.ResultList;
                                } else {
                                    throw new Error('数据必须有ResultList属性');
                                }
                                if (bindData.Input && bindData.Input.ListInput) {
                                    boundData.Input = boundData.Input || {};//初始绑定的vm=boundData可能是{}
                                    boundData.Input.ListInput = bindData.Input.ListInput;
                                }
                                kendoGrid.pbUseBoundData = true;
                                kendoGrid.dataSource.read();
                            }
                            if (!notAutoBind && boundData.ResultList && boundData.ResultList.length > 0) {
                                kendoGrid.dataSource.read();
                            }
                        });
                        scope.$on('kendoRendered', function () {
                            var kendoGrid = $parse(attrs['kendoGrid'])(scope);
                            if (!kendoGrid) {
                                throw new Error('kendo grid创建失败，请检查属性/参数设置是否正确、列设置的列数与tablehead中的列数是否一致');
                            }
                        });
                    }
                };
            }
        };
        return directiveDefinitionObject;
    }]);
}(String, angular));                         //end pack