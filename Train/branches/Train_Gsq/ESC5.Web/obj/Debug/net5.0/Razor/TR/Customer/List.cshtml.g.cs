#pragma checksum "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "95f02499a64d85f523cfef3bff6bd588f8e3e868"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.TR_Customer_List), @"mvc.1.0.view", @"/TR/Customer/List.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"95f02499a64d85f523cfef3bff6bd588f8e3e868", @"/TR/Customer/List.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"525638ae3b924b2c388495b2920a3ec2336831e5", @"/_ViewImports.cshtml")]
    public class TR_Customer_List : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<CustomerListVM>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 4 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml"
 using (Html.KendoForm("c.frmSearch", "Search"))
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <!--KendoTextBoxFor： HtmlHelper Class 客户端输入控件 m绑定VM属性-->\r\n    <div class=\"row\">\r\n        <div class=\"clearfix\">\r\n            <div class=\"e2-col-lg-3 e2-col-md-3 e2-col-sm-3 e2-col-xs-3\">\r\n                ");
#nullable restore
#line 10 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml"
           Write(Html.NgLabelFor(m => m.Input.Name_));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 11 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml"
           Write(Html.KendoTextBoxFor(m => m.Input.Name_));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n\r\n            <div class=\"e2-col-lg-3 e2-col-md-3 e2-col-sm-3 e2-col-xs-3\">\r\n                ");
#nullable restore
#line 15 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml"
           Write(Html.NgLabelFor(m => m.Input.Gender));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                ");
#nullable restore
#line 16 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml"
           Write(Html.KendoNumericBoxFor(m => m.Input.Gender));

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
");
            WriteLiteral(@"    <div style=""margin-top:5px;"">
        <!-- kendo-grid 控件 pb-kendo-grid为kendogrid绑定数据。该指令值为绑定的数据，通常是c.vm.该指令将绑定数据的ResultList属性和Input.ListInput属性关联到kendogrid的行数据、分页数据、排序数据上 同时支持页面跳转回来时保持这些表格状态 -->
        <!-- pb-kendo-grid-cols 列设置-->
        <!-- OT 表示使用模板且模板名称为固定的 Operation-template -->
        <table kendo-grid=""c.grdUser"" k-data-source=""DataSource"" options=""GridOptions"" pb-kendo-grid=""c.vm""
               pb-kendo-grid-cols=""[
                   'Email',
                   'Name_,T,70px|{style: \'text-align: center;background-color:green\'}',
                   'Gender,,80px',
                   'RegisterDate',
                   'Spending',
                   'Vip',
                   'Active',
                   ',OT',
               ]""  
               class=""myTable"">
            <!--Table 内本地化字段名显示文本时 key 值规范为表名加下划线加字段名 -->
            <thead>
                <tr>
                    <th class=""tableth""><span translate=""Customer_Email""></span></th>
                    <th class");
            WriteLiteral(@"=""tableth""><span translate=""Customer_Name_""></span></th>
                    <th class=""tableth""><span translate=""Customer_Gender""></span></th>
                    <th class=""tableth""><span translate=""Customer_RegisterDate""></span></th>
                    <th class=""tableth""><span translate=""Customer_Spending""></span></th>
                    <th class=""tableth""><span translate=""Customer_Vip""></span></th>
                    <th class=""tableth""><span translate=""Customer_Active""></span></th>
                    <th class=""tableth""></th>
                </tr>
            </thead>
        </table>
        <!--kendo grid 列设置中使用的模板写在 table 后面-->
        <!--kendo grid 的工具栏模板名固定为 Toolbar-template-->
        <!--列使用的模板 命名为列名加 -template-->
        <!--表格操作列模板 固定命名为 Operation-template-->
        <!--对于 at Url.State nav:action-->
        <script Id=""Toolbar-template"" type=""text/x-kendo-template"">
            <kendo-button ");
#nullable restore
#line 64 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml"
                     Write(Url.State("forward:Add"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@" icon=""'k-icon k-i-add'"" type=""button"">
                <span translate=""Add""></span>
            </kendo-button>
        </script>

        <!--""helper方法帮助生成链接的ngState的属性，点击即切换ngState。多个参数keyvalue对用逗号分隔""-->
        <script Id=""Name_-template"" type=""text/x-kendo-template"">
            <a class=""a-link"" ");
#nullable restore
#line 71 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml"
                         Write(Url.State("forward:Edit(Id:#=Id#)"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@">#=Name_#</a>
        </script>

        <script Id=""Operation-template"" type=""text/x-kendo-template"">
            <kendo-button type=""button"" icon=""'k-icon k-i-delete'"" ng-click=""c.Delete_click(#=Id#,'#=Name_#')"">
                <span translate=""Delete""></span>
            </kendo-button>
        </script>
    </div>
");
#nullable restore
#line 80 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\List.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<CustomerListVM> Html { get; private set; }
    }
}
#pragma warning restore 1591
