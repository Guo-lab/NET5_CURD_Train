#pragma checksum "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "293abe4797107fd9b6d527034b82caf86beb3b0e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.TR_Task_Info_MultiEdit), @"mvc.1.0.view", @"/TR/Task_Info/MultiEdit.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\_ViewImports.cshtml"
using Microsoft.AspNetCore.Html;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\_ViewImports.cshtml"
using ProjectBase.Web.Mvc;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\_ViewImports.cshtml"
using ProjectBase.Web.Mvc.Angular;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\_ViewImports.cshtml"
using KendoUIHelper;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\_ViewImports.cshtml"
using ESC5.Common.ViewModel.TR;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"293abe4797107fd9b6d527034b82caf86beb3b0e", @"/TR/Task_Info/MultiEdit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"525638ae3b924b2c388495b2920a3ec2336831e5", @"/_ViewImports.cshtml")]
    public class TR_Task_Info_MultiEdit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<TaskMultiEditVM>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div class=\"row\" ng-repeat=\"item in c.vm.Input.Rows\">\r\n\r\n");
#nullable restore
#line 5 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
     using (Html.KendoFormArray("c.frmEdit", "Save",new { DummyPrefix = "DummyRow", IndexedPrefix= "Input.Rows[{{$index}}]" }))
    {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <div style=""white-space: nowrap;"">
            <div class=""e2-col-lg-3"" style=""text-align:left;"">
                <kendo-button type=""button"" icon=""'k-icon k-i-save'"" ng-click=""c.Save_click($index)"">
                    <span translate=""Save""></span>
                </kendo-button>
            </div>
            <div class=""e2-col-lg-3"" style=""text-align:left;"">
                <kendo-button type=""button"" icon=""'k-icon k-i-back'"" ng-click=""c.Back_click()"">
                    <span translate=""Back""></span>
                </kendo-button>
            </div>
        </div>
");
#nullable restore
#line 19 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
   Write(Html.NgHiddenFor(m => m.DummyRow.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("        <table class=\"table table-bordered table-striped\">\r\n            <tbody>\r\n                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 24 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgLabelFor(m => m.DummyRow.Code, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 27 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.KendoTextBoxFor(m => m.DummyRow.Code));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 32 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgLabelFor(m => m.DummyRow.Name, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 35 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgTextBoxFor(m => m.DummyRow.Name, null));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 40 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgLabelFor(m => m.DummyRow.User, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 43 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.KendoDropDownListFor(m => m.DummyRow.User).DataSource("c.vm.UserList"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 48 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgLabelFor(m => m.DummyRow.MaxItemCount, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 51 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.KendoNumericBoxFor(m => m.DummyRow.MaxItemCount));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 56 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgLabelFor(m => m.DummyRow.CreateDate, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 59 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.KendoDatePickerFor(m => m.DummyRow.CreateDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 64 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgLabelFor(m => m.DummyRow.Score, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 67 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.KendoNumericBoxFor(m => m.DummyRow.Score));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 72 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgLabelFor(m => m.DummyRow.Status, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 75 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.KendoDropDownListFor(m => m.DummyRow.Status).Enum("Task_StatusEnum"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 80 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgLabelFor(m => m.DummyRow.Active, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 83 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
                   Write(Html.NgCheckBoxFor(m => m.DummyRow.Active));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n\r\n        </table>\r\n");
#nullable restore
#line 89 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_Info\MultiEdit.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\r\n<kendo-button type=\"button\" icon=\"\'k-icon k-i-save\'\" ng-click=\"c.MultiSave_click()\">\r\n    <span translate=\"多行保存\"></span>\r\n</kendo-button>\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<TaskMultiEditVM> Html { get; private set; }
    }
}
#pragma warning restore 1591
