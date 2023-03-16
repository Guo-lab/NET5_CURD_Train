(function (String, angular) {
    'use strict';

    var pbm = angular.module('projectbase');

    //<------------------------kendo service--------------------------------------------------
    var kendoFn = ['pb', '$state', '$translate', '$timeout', '$parse', function (pb, $state, $translate, $timeout, $parse) {
        var AddRemovedItem = function (array, obj, prop) {
            var CheckExist = function (array, obj, prop) {
                for (var i = 0; i < array.length; i++) {
                    if (array[i][prop] == obj[prop])
                        return true;
                }
                return false;
            }

            prop = prop == undefined ? "Id" : prop;
            if (obj[prop] && !CheckExist(array, obj, prop)) {
                var newList = angular.copy(array);
                newList.push(obj);
                return newList;
            } else {
                return array;
            }
        }
        var KendoDataSource = function (array, obj, prop) {
            if (obj == undefined)
                return new kendo.data.ObservableArray(array);
            else {
                return new kendo.data.ObservableArray(AddRemovedItem(array, obj, prop));
            }
        }

        var GetResultFromProperty = function (result, property) {
            var tmp = property.split(".")
            var ret = result;
            for (var i = 0; i < tmp.length; i++)
                ret = ret[tmp[i]];
            return ret;
        }

        var SetTreeviewChecked = function (source, array, clear) {
            var ids = array.split(',');
            angular.forEach(source, function (item, index) {
                if (ids.indexOf(item.Id.toString()) != -1) {
                    item.checked = true;
                } else {
                    if (clear) {
                        item.checked = false;
                    }
                }
                SetTreeviewChecked(item.items, array);
            })
        }

        var KendoAutoCompleteServer = function (url, param, resultProperty, dynamicParamFunc) {
            return new kendo.data.DataSource({
                serverFiltering: true,
                transport: {
                    read: function (options) {//options.data.filter.filters[0].value is the typed string
                        var parameters = param.replace("{0}", options.data.filter.filters[0].value)
                        if (dynamicParamFunc) {
                            parameters = parameters + "&" + dynamicParamFunc();
                        }
                        return pb.CallAction(url, parameters, function (result) {
                            if (resultProperty == undefined)
                                options.success(result.data);
                            else
                                options.success(GetResultFromProperty(result, resultProperty));
                        });
                    }
                }
            });
        }
        var KendoComboBoxServer = function (url, param, resultProperty, dynamicParamFunc) {
            return new kendo.data.DataSource({
                serverFiltering: true,
                transport: {
                    read: function (options) {
                        //since we are using server filter, we should prevent it from reading whole result if user click the right arrow
                        if (options.data.filter.filters.length == 0) {
                            options.success([]);
                            return;
                        }
                        var parameters = param.replace("{0}", options.data.filter.filters[0].value)
                        if (dynamicParamFunc) {
                            parameters = parameters + "&" + dynamicParamFunc();
                        }

                        return pb.CallAction(url, parameters, function (result) {
                            if (resultProperty == undefined)
                                options.success(result.data);
                            else
                                options.success(GetResultFromProperty(result, resultProperty));
                        });
                    }
                }
            });
        }
        var KendoTreeDataSource = function (tree, currentId) {

            var SetExpanded = function (list) {
                angular.forEach(list, function (item, index) {
                    item.expanded = true;
                    if (item.items.length > 0) {
                        SetExpanded(item.items);
                    }
                })
            }
            SetExpanded(tree);


            SetTreeviewChecked(tree, currentId);
            return new kendo.data.HierarchicalDataSource({ data: tree });
        };

        var KendoAutoCompleteClientEvent = function ($scope, widgetId, vmObj, valueField, datasourceValueField) {
            var autocompleteWidget;
            var selected = false;
            $scope.$on("kendoWidgetCreated", function (event, widget) {
                if (widget === $scope[widgetId]) {
                    autocompleteWidget = widget;
                    autocompleteWidget.bind("close", function (e) {
                        if (!selected) {
                            e.sender.value("");
                            vmObj[valueField] = null;
                        }
                        selected = false;
                    });
                    autocompleteWidget.bind("change", function (e) {
                        if (!selected) {
                            e.sender.value("");
                            vmObj[valueField] = null;
                        }
                    });
                    autocompleteWidget.bind("open", function (e) {
                        selected = false;
                    });
                    autocompleteWidget.bind("select", function (e) {
                        selected = true;
                        if (e.dataItem != null) {
                            vmObj[valueField] = e.dataItem[datasourceValueField];
                        }
                    });
                }
            });
        }
        var GetCheckedItem = function (listviewId, treeviewId) {
            var GetCheckedNodes = function (nodes) {
                var node, childCheckedNodes;
                var checkedNodes = [];

                for (var i = 0; i < nodes.length; i++) {
                    node = nodes[i];
                    if (node.checked) {
                        checkedNodes.push(node.Id);
                    }
                    if (node.hasChildren) {
                        childCheckedNodes = GetCheckedNodes(node.children.view());
                        if (childCheckedNodes.length > 0) {
                            checkedNodes = checkedNodes.concat(childCheckedNodes);
                        }
                    }
                }
                return checkedNodes;
            }
            var source = $("#" + listviewId).data("kendo-list-view").dataSource.view();
            var checkedId = [];
            angular.forEach(source, function (item, index) {
                checkedId = checkedId.concat(GetCheckedNodes($("#" + treeviewId + item.uid).data("kendo-tree-view").dataSource.view()));
            });
            return checkedId;
        }

        var KendoGridServerDataSource = function (form, sort, pageSize, group, beforeBinding) {
            var ds = {
                transport: {
                    read: function (options) {
                        var additionalData = "Input.Pager.PageSize=" + options.data.pageSize + "&Input.Pager.PageNum=" + options.data.page;
                        if (options.data.sort) {
                            additionalData = additionalData + "&Input.OrderExpression=" +
                                options.data.sort.map(function (x) {
                                    return x.field + " " + x.dir;
                                }).join(",");
                        }
                        return pb.AjaxSubmit(form, null, { "ajax-data": additionalData }, function (result) {
                            if (beforeBinding) {
                                beforeBinding(result.data.ViewModel ? result.data.ViewModel.ResultList : result.data.ResultList);
                            }
                            options.success(result.data.ViewModel ? result.data.ViewModel : result.data);
                        });
                    }
                },
                serverFiltering: true,
                serverSorting: true,
                serverPaging: true,
                pageSize: pageSize,
                schema: {
                    data: "ResultList",
                    total: "Input.Pager.ItemCount",
                    model: {
                        id: "Id"
                    }
                },
                sort: sort
            }
            if (group) {
                ds.group = group;
            }
            return new kendo.data.DataSource(ds);
        }
        
        var KendoGridDataSource = function (result, pageSize, schema, sort, group) {
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
            if (group) {
                ds.group = group;
            }
            return new kendo.data.DataSource(ds);
        }

        var KendoSpreadsheetServerDataSource = function ($scope, mySheet, form, title, columns) {
            var sheet = {
                name: title,
                columns: new Array(),
                rows: [{ height: 40, cells: new Array() }]
            }
            var colName = [];
            for (var i = 0; i < columns.length; i++) {
                if (columns[i].width) {
                    sheet.columns.push({ width: columns[i].width });
                } else {
                    sheet.columns.push({ width: 200 });
                }
                if (columns[i].cells) {
                    columns[i].cells.bold = "true";
                    columns[i].cells.background = "#9c27b0";
                    columns[i].cells.textAlign = "center";
                    columns[i].cells.color = "white";
                    columns[i].cells.enable = false;
                    sheet.rows[0].cells.push(columns[i].cells);
                } else {
                    var cells = {
                        bold: "true",
                        background: "#9c27b0",
                        textAlign: "center",
                        color: "white",
                        enable: false
                    }
                    sheet.rows[0].cells.push(cells);
                }
                if (columns[i].format) {
                    sheet.rows[0].cells[i].format = columns[i].format
                }
                colName.push(columns[i].name);
            }

            var ds = {
                requestEnd: function (e) {
                    if (e.type === 'read') {
                        setTimeout(function () {
                            var activeSheet = $scope.mySheet.activeSheet();
                            if (columns && columns.length > 0) {
                                var headerRange = activeSheet.range('A1:' + createCellPos(columns.length - 1) + '1');
                                headerRange.values([colName]);
                                for (var i = 0; i < columns.length; i++) {
                                    if (columns[i].validation) {
                                        activeSheet.range("$" + createCellPos(i) + ":$" + createCellPos(i)).validation(columns[i].validation);
                                    }
                                }
                            }
                        }, 0);
                    }
                },
                transport: {
                    read: function (options) {
                        return pb.AjaxSubmit(form, null, null, function (result) {
                            options.success(ConvertToSpreadsheetDataSource(result.data.ViewModel, columns));
                        });
                    }
                }
            };
            sheet.dataSource = new kendo.data.DataSource(ds);
            return sheet;
        }

        var KendoSpreadsheetDataSource = function ($scope, result, title, columns, schema) {
            var sheet = {
                name: title,
                columns: new Array(),
                rows: [{ height: 0 }, { height: 40, cells: new Array() }]
            }
            var colName = [];
            for (var i = 0; i < columns.length; i++) {
                colName.push(columns[i].name);
                if (columns[i].width) {
                    sheet.columns.push({ width: columns[i].width });
                } else {
                    sheet.columns.push({ width: 200 });
                }
                if (columns[i].cells) {
                    columns[i].cells.bold = "true";
                    columns[i].cells.background = "#9c27b0";
                    columns[i].cells.textAlign = "center";
                    columns[i].cells.color = "white";
                    columns[i].cells.enable = false;
                    sheet.rows[1].cells.push(columns[i].cells);
                } else {
                    var cells = {
                        bold: "true",
                        background: "#9c27b0",
                        textAlign: "center",
                        color: "white",
                        enable: false
                    }
                    sheet.rows[1].cells.push(cells);
                }
            }

            var ds = {
                requestEnd: function (e) {
                    if (e.type === 'read') {
                        setTimeout(function () {
                            var activeSheet = $scope.mySheet.activeSheet();
                            if (columns && columns.length > 0) {
                                for (var i = 0; i < columns.length; i++) {
                                    if (columns[i].validation) {
                                        activeSheet.range("$" + createCellPos(i) + ":$" + createCellPos(i)).validation(columns[i].validation);
                                    }
                                    if (columns[i].format) {
                                        activeSheet.range("$" + createCellPos(i) + ":$" + createCellPos(i)).format(columns[i].format);
                                    }
                                }
                            }
                        }, 0);
                    }
                },
                transport: {
                    read: function (options) {
                        return options.success(ConvertToSpreadsheetDataSource(result, columns));
                    },
                    submit: function (e) {
                        console.log(e);
                    }
                },
            };
            sheet.dataSource = new kendo.data.DataSource(ds);
            return sheet;
        }
        var ConvertToSpreadsheetDataSource = function (data, columns) {
            var title = {};
            var ret = new Array();
            ret.push(title);
            angular.forEach(data, function (x) {
                var obj = {};
                angular.forEach(columns, function (y) {
                    obj[y.field] = x[y.field];
                    title[y.field] = y.name;
                })
                ret.push(obj);
            })
            return ret;
        }
        var createCellPos = function (n) {
            var ordA = 'A'.charCodeAt(0);
            var ordZ = 'Z'.charCodeAt(0);
            var len = ordZ - ordA + 1;
            var s = "";
            while (n >= 0) {
                s = String.fromCharCode(n % len + ordA) + s;
                n = Math.floor(n / len) - 1;
            }
            return s;
        }


        var KendoStatefulGrid = function ($scope, gridId, fnSaveFilter, fnRestoreFilter, initialShowGrid) {
            var grid;
            var stateName = $state.current.name;
            var savedState = {};
            if (initialShowGrid == undefined) { initialShowGrid = true }
            var restoring = false;
            $scope.$on("kendoWidgetCreated", function (event, widget) {
                if (widget === $scope[gridId]) {
                    grid = widget;
                    grid.bind("dataBound", function (e) {
                        if (restoring) return;
                        SaveGridState()
                    });
                    RestoreGridState();
                }
            });

            if (sessionStorage[stateName]) {
                savedState = angular.fromJson(sessionStorage[stateName]);
                if (fnRestoreFilter)
                    fnRestoreFilter(savedState.filter);
            }

            var SaveGridState = function () {
                var state = {
                    gridState: {
                        page: grid.dataSource.page(),
                        pageSize: grid.dataSource.pageSize(),
                        filter: grid.dataSource.filter(),
                        sort: grid.dataSource.sort()
                    },
                    filter: {}
                }
                if (fnSaveFilter) {
                    state.filter = fnSaveFilter()
                }
                angular.extend(savedState, state);
                sessionStorage[stateName] = angular.toJson(savedState);
            }
            var RestoreGridState = function () {
                if (sessionStorage[stateName] == undefined && initialShowGrid == false) {
                    return;
                }
                restoring = true; //unnecessay to save state again because it is just restored from sessionStorage
                if (savedState.gridState) {
                    $timeout(function () {
                        grid.dataSource.query({ page: savedState.gridState.page, pageSize: savedState.gridState.pageSize, filter: savedState.gridState.filter, sort: savedState.gridState.sort });
                    })
                } else {
                    grid.dataSource.read();
                }
                restoring = false;
            }

            return {
                SaveAdditionalValue: function (data) {
                    if (!savedState.additional) {
                        savedState.additional = {};
                    }
                    angular.extend(savedState.additional, data);
                    sessionStorage[stateName] = angular.toJson(savedState);
                },
                GetAdditionalValue: function () {
                    if (savedState) {
                        return savedState.additional;
                    } else {
                        return null;
                    }
                }
            }
        }

        var KendoInlineEditor = function () {
            var widget;
            return {
                DropDownList: function (container, options) { return new DropDownList(container, options); },
                DatePicker: function (container, options) { return new DatePicker(container, options); },
                NumericBox: function (container, options) { return new NumericBox(container, options); },
                TextBox: function (container, options) { return new TextBox(container, options); },
                AutoComplete: function (container, options) { return new AutoComplete(container, options); }
            }
        }

        var KendoEditableGrid = function (options, $scope, formName, fnBeforeEdit, fnSave, fndataBound) {
            if (fnBeforeEdit) {
                options.beforeEdit = fnBeforeEdit;
            } else {
                options.beforeEdit = function (e) {
                    if (!e.model.Id) {
                        e.model.Id = 0;
                    }
                }
            }
            if (fnSave) {
                options.save = function (e) {
                    e.preventDefault();
                    if (pb.KendoValidator($scope, formName).validate()) {
                        fnSave(e).then(function (result) {
                            if (result == undefined || result.isRcResult)
                                e.sender.saveChanges();
                        }, function (reason) {
                            //console.log(reason);
                        });
                    }
                }
            }

            if (fndataBound) {
                options.dataBound = function (e) {
                    var grid = e.sender;
                    var gridData = grid.dataSource.view();
                    for (var i = 0; i < gridData.length; i++) {
                        var currentUid = gridData[i].uid;
                        var currentRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                        fndataBound({ data: gridData[i], container: $(currentRow) });
                    }
                };
                options.cancel = function (e) {
                    if (e.model.Id > 0) {
                        e.sender.dataSource.cancelChanges();
                        e.sender.refresh();
                    }
                }
            }
            return options;
        }

        var KendoSelectable = function (controller, fnItemFilter, initialSelection) {
            controller.selection = {
                AllSelected: false
            };
            var selectedList;
            if (initialSelection) {
                selectedList = initialSelection;
            } else {
                selectedList = [];
            };
            var isItemSelectable = function (item) {
                if (fnItemFilter) {
                    return fnItemFilter(item);
                } else {
                    return true;
                }
            }
            var addRemoveItem = function (item) {
                if (!isItemSelectable(item)) { return; }
                var index = selectedList.indexOf(item.Id);
                if (index == -1 && item.Selected) {
                    selectedList.push(item.Id);
                } else if (index != -1 && !item.Selected) {
                    selectedList.splice(index, 1);
                }
            }
            var calcAllSelected = function (dataSource) {
                var list = (dataSource == undefined ? controller.DataSource.view() : dataSource);
                var allSelected = true;
                var hasSelection = false;
                for (var i = 0; i < list.length; i++) {
                    if (isItemSelectable(list[i])) {
                        if (list[i].Selected) {
                            hasSelection = true;
                        }
                        else {
                            allSelected = false;
                            break;
                        }
                    }
                }
                controller.selection.AllSelected = allSelected && hasSelection;
            }
            // Select/Unselect all
            controller.ToggleSelectAll = function () {
                var list = controller.DataSource.view();
                angular.forEach(list, function (item) {
                    if (isItemSelectable(item)) {
                        item.Selected = controller.selection.AllSelected;
                        addRemoveItem(item);
                    }
                })
            };
            // Clear all selected. 
            controller.ClearAllSelected = function () {
                selectedList = [];
                var list = controller.DataSource.data();
                angular.forEach(list, function (item) {
                    if (isItemSelectable(item)) {
                        item.Selected = false;
                    }
                })
                controller.selection.AllSelected = false;
            };
            // Select/Unselect single item
            controller.ToggleSelected = function (item) {
                addRemoveItem(item);
                calcAllSelected();
            };
            //BeforeBinding
            controller.BeforeBinding = function (list) {
                angular.forEach(list, function (item) {
                    if (isItemSelectable(item)) {
                        item.Selected = (selectedList.indexOf(item.Id) != -1);
                    }
                })
                calcAllSelected(list);
            };
            controller.GetSelection = function () {
                return selectedList;
            }
        }
        function ControlBase(container, directiveName) {
            this.widget;
            var widgetId = "";
            this.TranslateKey = function (key) {
                this.widget.attr("translatekey", key)
                return this;
            }
            this.Required = function () {
                this.widget.attr("required", "required")
                return this;
            }

            this.Name = function (name) {
                this.widget.attr("name", name);
                return this;
            }
            this.Id = function (Id) {
                widgetId = Id;
                return this;
            }
            this.NgModel = function (ngModel, primitive) {
                this.widget.attr("k-ng-model", ngModel);
                if (primitive)
                    this.widget.attr("k-value-primitive", "true");
                return this;
            }
            this.HtmlAttributes = function (attributes) {
                for (var att in attributes) {
                    this.widget.attr(att, attributes[att]);
                }
                return this;
            }
            this.ReadOnly = function (readOnly) {
                this.widget.attr("k-ng-readonly", readOnly);
                return this;
            }
            this.Events = function (events) {
                for (var e in events) {
                    this.widget.attr("k-on-" + e, events[e] + "(kendoEvent)");
                }
                return this;
            }
            this.Show = function () {
                if (directiveName != "")
                    this.widget.attr(directiveName, widgetId);
                this.widget.appendTo(container);
            }
        }

        function ListBase(container, options, directiveName) {
            ControlBase.call(this, container, directiveName);
            this.DataSource = function (dataSource) {
                this.widget.attr("k-data-source", dataSource);
                return this;
            }
            this.DataValueField = function (dataValueField) {
                this.widget.attr("k-data-value-field", "'" + dataValueField + "'");
                return this;
            }
            this.DataTextField = function (dataTextField) {
                this.widget.attr("k-data-text-field", "'" + dataTextField + "'");
                return this;
            }
            this.Enum = function (enumName) {
                this.DataSource("DictObj." + enumName).DataTextField("Text").DataValueField("Id");
                return this;
            }

        }
        extend(ListBase, ControlBase);

        function DropDownList(container, options) {
            ListBase.call(this, container, options, "kendo-drop-down-list");
            this.widget = $("<select></select>");
        }
        extend(DropDownList, ListBase);

        function AutoComplete(container, options) {
            ListBase.call(this, container, options, "kendo-auto-complete");
            this.widget = $("<input k-enforce-min-length='true'/>");
            this.MinLength = function (minLength) {
                this.widget.attr("k-min-length", minLength);
                return this;
            }
        }
        extend(AutoComplete, ListBase);

        function DatePicker(container, options) {
            ControlBase.call(this, container, "kendo-date-picker");
            this.widget = $("<input />");
            this.Format = function (format) {
                this.widget.attr("k-format", "'" + format + "'");
                return this;
            }
            this.Min = function (min) {
                this.widget.attr("k-min", "'" + min + "'");
                return this;
            }
            this.Max = function (max) {
                this.widget.attr("k-max", "'" + max + "'");
                return this;
            }
        }
        extend(DatePicker, ControlBase);

        function TextBox(container, options) {
            ControlBase.call(this, container, "")
            this.widget = $("<input type=text class='k-textbox' />");
            this.MaxLength = function (maxLength) {
                this.widget.attr("maxlength", maxLength);
                return this;
            }
            this.Email = function () {
                this.widget.attr("type", "email");
                return this;
            }
            this.Pattern = function (pattern, valMsg) {
                this.widget.attr("pattern", pattern);
                var that = this;
                if (valMsg) {
                    $translate(valMsg).then(function (message) {
                        that.widget.attr("pb-valmsg", "{\"pattern\":\"" + message + "\"}");
                    })
                }
                return this;
            }
        }
        extend(TextBox, ControlBase);
        function NumericBox(container, options) {
            ControlBase.call(this, container, "kendo-numeric-text-box");
            this.widget = $("<input />");
            this.Min = function (min) {
                this.widget.attr("k-min", min);
                return this;
            }
            this.Max = function (max) {
                this.widget.attr("k-max", max);
                return this;
            }
            this.Format = function (format) {
                this.widget.attr("k-format", "'" + format + "'");
                return this;
            }
            this.Scale = function (scale) {
                this.widget.attr("k-decimals", scale);
                return this;
            }
            this.Step = function (step) {
                this.widget.attr("k-step", step);
                return this;
            }
        }
        extend(NumericBox, ControlBase);

        return {
            KendoDataSource: KendoDataSource,
            KendoTreeDataSource: KendoTreeDataSource,
            GetCheckedItem: GetCheckedItem,
            SetTreeviewChecked: SetTreeviewChecked,
            AddRemovedItem: AddRemovedItem,
            KendoAutoCompleteServer: KendoAutoCompleteServer,
            KendoComboBoxServer: KendoComboBoxServer,
            KendoGridDataSource: KendoGridDataSource,
            KendoGridServerDataSource: KendoGridServerDataSource,
            KendoStatefulGrid: KendoStatefulGrid,
            KendoInlineEditor: KendoInlineEditor,
            KendoEditableGrid: KendoEditableGrid,
            KendoAutoCompleteClientEvent: KendoAutoCompleteClientEvent,
            KendoSelectable: KendoSelectable,
            KendoSpreadsheetDataSource: KendoSpreadsheetDataSource,
            KendoSpreadsheetServerDataSource: KendoSpreadsheetServerDataSource
        };
    }];

    pbm.provider('kendoui', function () {
        var me = this;
        me.$get = kendoFn;
    });

}(String, angular));                         //end pack