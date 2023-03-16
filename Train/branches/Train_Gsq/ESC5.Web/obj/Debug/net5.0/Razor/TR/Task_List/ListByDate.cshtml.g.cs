#pragma checksum "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_List\ListByDate.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "adb441d45af91dad55e264b64f18555a6155e0d8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.TR_Task_List_ListByDate), @"mvc.1.0.view", @"/TR/Task_List/ListByDate.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"adb441d45af91dad55e264b64f18555a6155e0d8", @"/TR/Task_List/ListByDate.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"525638ae3b924b2c388495b2920a3ec2336831e5", @"/_ViewImports.cshtml")]
    public class TR_Task_List_ListByDate : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Task_ListListByDateVM>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_List\ListByDate.cshtml"
 using (Html.KendoForm("c.frmSearch", "SearchByDate"))
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"row\">\r\n            <div class=\"clearfix\">\r\n                <div class=\"e2-col-lg-3 e2-col-md-3 e2-col-sm-3 e2-col-xs-3\">\r\n                    ");
#nullable restore
#line 7 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_List\ListByDate.cshtml"
               Write(Html.NgLabelFor(m => m.Input.CreateDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    ");
#nullable restore
#line 8 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_List\ListByDate.cshtml"
               Write(Html.KendoDatePickerFor(m => m.Input.CreateDate));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                </div>
                <div class=""e2-col-lg-2 e2-col-md-8 e2-col-sm-8 e2-col-xs-8"">
                    <kendo-button type=""button"" ng-click=""c.Search_click()"" icon=""'k-icon k-i-search'"">
                        <span translate=""Search""></span>
                    </kendo-button>
                </div>
            </div>
        </div>
        <div style=""margin-top:5px;"">
            <table kendo-grid=""c.grdTask"" k-data-source=""DataSource"" options=""GridOptions"" pb-kendo-grid=""c.vm""
                   pb-kendo-grid-cols=""[
            'Code',
            'Name,T,80px',
            'User,|Display',
            'MaxItemCount',
            'CreateDate,|Display',
            'Score',
            'Status,|Display:\'Task_StatusEnum\'',
            'Active,|Display'
        ]"" pb-kendo-grid-options=""{toolbarTpl:'MyToolbar-tpl'}"" class=""myTable"">
                <thead>
                    <tr>
                        <th class=""tableth""><span translate=""Task_Code""></span></th");
            WriteLiteral(@">
                        <th class=""tableth""><span translate=""Task_Name""></span></th>
                        <th class=""tableth""><span translate=""Task_User""></span></th>
                        <th class=""tableth""><span translate=""Task_MaxItemCount""></span></th>
                        <th class=""tableth""><span translate=""Task_CreateDate""></span></th>
                        <th class=""tableth""><span translate=""Task_Score""></span></th>
                        <th class=""tableth""><span translate=""Task_Status""></span></th>
                        <th class=""tableth""><span translate=""Task_Active""></span></th>
                    </tr>
                </thead>
            </table>
            <script Id=""MyToolbar-tpl"" type=""text/x-kendo-template"">
                    <kendo-button ");
#nullable restore
#line 43 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_List\ListByDate.cshtml"
                             Write(Url.State("forward:Add"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@" icon=""'k-icon k-i-add'"" type=""button"">
                        <span translate=""Add""></span>
                    </kendo-button>
            </script>
            <script Id=""Name-template"" type=""text/x-kendo-template"">
                    <a class=""a-link"" ");
#nullable restore
#line 48 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_List\ListByDate.cshtml"
                                 Write(Url.State("forward:Edit(id:#=Id#)"));

#line default
#line hidden
#nullable disable
            WriteLiteral(">#=Name#</a>\r\n            </script>\r\n        </div>\r\n");
#nullable restore
#line 51 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Task_List\ListByDate.cshtml"
    }

#line default
#line hidden
#nullable disable
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Task_ListListByDateVM> Html { get; private set; }
    }
}
#pragma warning restore 1591
