'use strict';

(function (angular) {

    /*!
* angular-ui-uploader
* https://github.com/angular-ui/ui-uploader
* Version: 1.4.0 - 2017-02-14T02:35:18.253Z
* License: MIT
* 修改点
* 支持files已有file的初始化
*/
    

    angular.module('uploader', []).directive('e2FileUploader', function ($parse,$timeout) {
        return {
            restrict: 'EA',
            replace: true,
            transclude: {"Title":"?uploaderTitle","Result":"?uploaderResult"},
            scope: {
                files: '=ngModel',
                fileSizeLimit: '@',
                url: '@',
                multiple: '@',
                readOnly: '@',
                type: '@',
                showBadge:'@',
                uploadFinished: '&?',
                uploadFailed: '&',
                fileRemoved:'&'
            },
            controller: function ($scope, $attrs) {
                //$parse($attrs.name).assign($scope.$parent, this);
                var uiUploader = function () {

                    var self = this;
                    for (var i = 0; i < $scope.files.length; i++) {
                        $scope.files[i].active = true;
                    }
                   
                    self.options = {};
                    self.activeUploads = 0;
                    self.uploadedFiles = 0;

                    function addFiles(file) {
                        $scope.files.push(file);
                    }

                    function getFiles() {
                        return $scope.files;
                    }

                    function startUpload(options) {
                        self.options = options;

                        //headers are not shared by requests
                        var headers = options.headers || {};

                        for (var i = 0; i < $scope.files.length; i++) {
                            if (self.activeUploads == self.options.concurrency) {
                                break;
                            }
                            if ($scope.files[i].active || $scope.files[i].error != null)
                                continue;
                            ajaxUpload($scope.files[i], self.options.url, self.options.data, self.options.paramName, headers);
                        }
                    }

                    function removeFile(file) {
                        $scope.files.splice($scope.files.indexOf(file), 1);
                    }

                    function removeAll() {
                        $scope.files.splice(0, $scope.files.length);
                    }

                    return {
                        addFiles: addFiles,
                        getFiles: getFiles,
                        files: $scope.files,
                        startUpload: startUpload,
                        removeFile: removeFile,
                        removeAll: removeAll
                    };

                    function getHumanSize(bytes) {
                        var sizes = ['n/a', 'bytes', 'KiB', 'MiB', 'GiB', 'TB', 'PB', 'EiB', 'ZiB', 'YiB'];
                        var i = (bytes === 0) ? 0 : +Math.floor(Math.log(bytes) / Math.log(1024));
                        return (bytes / Math.pow(1024, i)).toFixed(i ? 1 : 0) + ' ' + sizes[isNaN(bytes) ? 0 : i + 1];
                    }

                    function ajaxUpload(file, url, data, key, headers) {
                        var xhr, formData, prop;
                        data = data || {};
                        key = key || 'file';

                        self.activeUploads += 1;
                        file.active = true;
                        xhr = new window.XMLHttpRequest();

                        // To account for sites that may require CORS
                        if (data.withCredentials === true) {
                            xhr.withCredentials = true;
                        }

                        formData = new window.FormData();
                        xhr.open('POST', url);

                        if (headers) {
                            for (var headerKey in headers) {
                                if (headers.hasOwnProperty(headerKey)) {
                                    xhr.setRequestHeader(headerKey, headers[headerKey]);
                                }
                            }
                        }

                        // Triggered when upload starts:
                        xhr.upload.onloadstart = function () {
                        };

                        // Triggered many times during upload:
                        xhr.upload.onprogress = function (event) {
                            if (!event.lengthComputable) {
                                return;
                            }
                            // Update file size because it might be bigger than reported by
                            // the fileSize:
                            //$log.info("progres..");
                            //console.info(event.loaded);
                            file.loaded = event.loaded;
                            file.humanSize = getHumanSize(event.loaded);
                            if (angular.isFunction(self.options.onProgress)) {
                                self.options.onProgress(file);
                            }
                        };

                        // Triggered when the upload is successful (the server may not have responded yet).
                        xhr.upload.onload = function () {

                            if (angular.isFunction(self.options.onUploadSuccess)) {
                                self.options.onUploadSuccess(file);
                            }
                        };

                        // Triggered when upload fails:
                        xhr.upload.onerror = function (e) {
                            if (angular.isFunction(self.options.onError)) {
                                self.options.onError(e);
                            }
                        };

                        // Triggered when the upload has completed AND the server has responded. Equivalent to
                        // listening for the readystatechange event when xhr.readyState === XMLHttpRequest.DONE.
                        xhr.onload = function () {

                            self.activeUploads -= 1;
                            self.uploadedFiles += 1;

                            startUpload(self.options);

                            if (angular.isFunction(self.options.onCompleted)) {
                                self.options.onCompleted(file, xhr.responseText, xhr.status);
                            }

                            if (self.activeUploads === 0) {
                                self.uploadedFiles = 0;
                                if (angular.isFunction(self.options.onCompletedAll)) {
                                    self.options.onCompletedAll($scope.files);
                                }
                            }
                        };

                        // Append additional data if provided:
                        if (data) {
                            for (prop in data) {
                                if (data.hasOwnProperty(prop)) {
                                    formData.append(prop, data[prop]);
                                }
                            }
                        }

                        // Append file data:
                        formData.append(key, file, file.name);

                        // Initiate upload:
                        xhr.send(formData);

                        return xhr;
                    }

                }

                //$parse($attrs.name).assign($scope.$parent, this);

                this.clearAll = function () {
                    $scope.clear();
                };

                var self = this;
                self.Var ={
                    controls : {},
                    Uploader: new uiUploader()
                }

                this.upload = function () {
                    self.Var.Uploader.startUpload({
                        url: $scope.url,
                        concurrency: 1,
                        onProgress: function (file) {
                            $scope.$apply();
                        },
                        onCompleted: function (file, response, status) {
                            if (parseInt(status) == 200) {
                                var result = JSON.parse(response);
                                if (result.error) {
                                    alert(result.error);
                                    file.active = false;
                                    $scope.uploadFailed(file, response, status);
                                }
                                else {
                                    file.active = true;
                                    $scope.uploadFinished({ file: file, response: result, status: status });
                                }
                            }
                            else {
                                file.active = false;
                                alert("上传失败，状态码=" + status);
                            }
                            $scope.$apply();
                        }
                    });
                };
                
                // 单个文件的上传大小限制，默认4MB
                this.fileSizeLimit = ($scope.fileSizeLimit || 30) * 1024 * 1024;

                // 服务器处理程序的地址
                $scope.url = $scope.url || '../Upload/UploadAttachment?SaveTo=';
                                
                $scope.multiple = $scope.multiple || "true";
                $scope.showBadge = $scope.showBadge || "true";
                $scope.readOnly = $scope.readOnly || "false";

                if (!$scope.uploadFinished) {
                    $scope.uploadFinished = function (res) {
                        res.file.url = res.response.url;
                        res.file.OriginalFileName = res.response.OriginalFileName;
                        res.file.SavedFileName = res.response.SavedFileName;
                        res.file.Temporary = res.response.Temporary;
                    };
                }

                $scope.uploadFailed = $scope.uploadFailed || function (file, response, status) { };

                $scope.fileRemoved = $scope.fileRemoved || function (file) { };

                $scope.remove = function (file) {
                    self.Var.Uploader.removeFile(file);
                    $scope.fileRemoved(file);
                };

                $scope.clear = function () {
                    self.Var.Uploader.removeAll();
                };

                                
                $scope.showList = function () {
                    self.Var.controls.layer.style.display = "block";
                }

                $scope.closeList = function () {
                    self.Var.controls.layer.style.display = "none";
                }
            },
            link: function (scope, element, atts, controller) {
                var getExtension = function (file) {
                    var n = file.lastIndexOf(".");
                    if (n >= 0)
                        return file.substr(n);
                    else
                        return "";
                }
                controller.Var.controls.file = element.children()[0].getElementsByClassName('file-uploader-file')[0];
                controller.Var.controls.layer = element.children()[1].getElementsByClassName('file-uploader-result')[0];

                controller.Var.controls.file.multiple = (scope.multiple=="true");
                
                if (scope.type) {
                    controller.Var.controls.file.accept =scope.type;
                }
                controller.Var.controls.file.addEventListener('change', function (e) {
                    var files = e.target.files;
                    var bValid = false;
                    for (var i = 0; i < files.length; i++) {
                        if (scope.type) {
                            var sExt = getExtension(files[i].name);
                            // 文件类型限制
                            if (sExt=="" || scope.type.indexOf(sExt) < 0) {
                                alert("该文件类型未被允许上传");
                                continue;
                            }
                        }

                        // 文件大小限制
                        if (files[i].size > controller.fileSizeLimit) {
                            alert("文件大小超过限制" + controller.fileSizeLimit/1024/1024 + "MB"); 
                            continue;
                        }

                        controller.Var.Uploader.addFiles(files[i]);
                        bValid = true;
                    }
                    if (!bValid) return;
                    scope.files = controller.Var.Uploader.getFiles();
                    controller.upload();
                    e.srcElement.value = ""
                    scope.$apply();
                });
            },
            templateUrl: '../Shared/Directive/FileUploader/FileUploader.html'
        };
    });
})(angular);