#pragma checksum "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "8783a479260afcc93b9a45af968d5934990320c8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.TR_Customer_Edit), @"mvc.1.0.view", @"/TR/Customer/Edit.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8783a479260afcc93b9a45af968d5934990320c8", @"/TR/Customer/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"525638ae3b924b2c388495b2920a3ec2336831e5", @"/_ViewImports.cshtml")]
    public class TR_Customer_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<CustomerEditVM>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
 using (Html.KendoForm("c.frmEdit", "Save"))
{

#line default
#line hidden
#nullable disable
            WriteLiteral(@"    <div style=""white-space: nowrap;"">
        <div class=""e2-col-lg-3"" style=""text-align:left;"">
            <kendo-button type=""button"" icon=""'k-icon k-i-save'"" ng-click=""c.Save_click()"">
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
#line 17 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
Write(Html.NgHiddenFor(m => m.Input.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral(" <!--不需要输入单要提交的数据使用隐藏控件-->\r\n    <table class=\"table table-bordered table-striped\">\r\n        <tbody>\r\n            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 22 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.NgLabelFor(m => m.Input.Email, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 25 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.KendoTextBoxFor(m => m.Input.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 30 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.NgLabelFor(m => m.Input.Name_, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 33 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.KendoTextBoxFor(m => m.Input.Name_));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 38 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.NgLabelFor(m => m.Input.Gender, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 41 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.KendoNumericBoxFor(m => m.Input.Gender));

#line default
#line hidden
#nullable disable
            WriteLiteral(" <!--数字型数据使用数字输入控件-->\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 46 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.NgLabelFor(m => m.Input.RegisterDate, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 49 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.KendoDatePickerFor(m => m.Input.RegisterDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 54 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.NgLabelFor(m => m.Input.Spending, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 57 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.KendoNumericBoxFor(m => m.Input.Spending));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 62 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.NgLabelFor(m => m.Input.Vip, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 65 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.KendoNumericBoxFor(m => m.Input.Vip));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 70 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.NgLabelFor(m => m.Input.Active, "e2-label"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 73 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
               Write(Html.NgCheckBoxFor(m => m.Input.Active));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n\r\n        </tbody>\r\n\r\n    </table>\r\n");
#nullable restore
#line 80 "C:\Project\Train\branches\Train_Gsq\ESC5.Web\TR\Customer\Edit.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<CustomerEditVM> Html { get; private set; }
    }
}
#pragma warning restore 1591