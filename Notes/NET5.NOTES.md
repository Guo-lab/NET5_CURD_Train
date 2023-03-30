# NET5.Core Building Process

> [Asp.Net Core](https://blog.csdn.net/stone0823/article/details/118796624?spm=1001.2101.3001.6650.3&utm_medium=distribute.pc_relevant.none-task-blog-2%7Edefault%7EESLANDING%7Edefault-3-118796624-blog-118890054.pc_relevant_landingrelevant&depth_1-utm_source=distribute.pc_relevant.none-task-blog-2%7Edefault%7EESLANDING%7Edefault-3-118796624-blog-118890054.pc_relevant_landingrelevant&utm_relevant_index=4)

1. Install SQLServer, Visual Studio, (SVN) and SSMS
2. SVN Init, 运行项目的签出以及BaseLibs的签出, 对于基本库和运行项目软连接的建立, 建立分支可在待fork目录右键fork到新分支（文件夹建立软连接的时候，目标文件夹不可存在，但是其父目录需完整）
3. ESC5.Web\NHibernate.config中配置了数据库连接串, 因此SSMS可以进行配置
4. SSMS配置: (1) 先建立SQL Server 用户(PUBLIC + SYSADMIN)，通过服务器SQL Server验证 (2) 后进行数据库还原，bak备份导入
5. 数据库字符串设置

   ```
   <property name="connection.connection_string">Data Source=.;Database=Train_Gsq;User ID=admin;Password=123456</property>
   <property name="default_schema">Train_Gsq.dbo</property>
   ```

   [DEFAULT_SCHEMA](https://learn.microsoft.com/en-us/sql/t-sql/statements/alter-user-transact-sql?view=sql-server-ver16)
6. 利用IIS Express运行sln工程

(cshtml 中的 Input 需要在 VM 中到单独定义)

<br> <br><br>

# 为清单增加数据库 field 增加查询条件

1. 增添 CURD 功能，涉及到（以增添筛选功能为例）Controller, Edit.ctrl.js 与 Edit.cshtml 在 MVC 的框架下调整
2. 增加数据库的表的 field， 创建数据库，创建表在SSMS 或 SQL Server 中操作即可

   [Add Columns to a Table (Database Engine) - SQL Server | Microsoft Learn](https://learn.microsoft.com/en-us/sql/relational-databases/tables/add-columns-to-a-table-database-engine?view=sql-server-ver16)

   ```sql
   SELECT * FROM dbo.TR_User

   ALTER TABLE dbo.TR_User
   ADD Mood VARCHAR(5) NULL;
   ```

   [SQL Server数据类型 - SQL Server教程 (yiibai.com)](https://www.yiibai.com/sqlserver/sqlserver_datatypes.html)[Create tables (Database Engine) - SQL Server | Microsoft Learn](https://learn.microsoft.com/en-us/sql/relational-databases/tables/create-tables-database-engine?view=sql-server-ver16)

   ```sql
   CREATE TABLE Train_Gsq.dbo.TR_Customer(
       Id           INT            NOT NULL     PRIMARY KEY,
   	Email        VARCHAR (50)   NOT NULL,
   	Name_        VARCHAR (10)   NOT NULL,
   	Gender       TINYINT        NOT NULL,
   	RegisterDate SMALLDATETIME      NULL,
   	Spending     DECIMAL(10, 3) NOT NULL,
   	Vip          TINYINT        NOT NULL,
   	Active       BIT            NOT NULL,
   );


   INSERT INTO Train_Gsq.dbo.TR_Customer VALUES('88', '123@qq.com', 'Gsq', '0', '', '100.000', '3', '1');

   UPDATE Train_Gsq.dbo.TR_Customer SET RegisterDate = NULL WHERE Name_ = 'Gsq';

   SELECT * FROM Train_Gsq.dbo.TR_Customer;
   ```

   建表之后，处理 CURD 功能
3. [第 3 部分，将视图添加到 ASP.NET Core MVC 应用 | Microsoft Learn](https://learn.microsoft.com/zh-cn/aspnet/core/tutorials/first-mvc-app/adding-view?view=aspnetcore-3.1&tabs=visual-studio)

> 规范编程 CoC?  配置 => 规范
> 三个类库 -> MVC

依靠规范，在创建文件的时候新建普通文件即可，通过规范“编译链接”，会在 Web（cshtml，js）bin 集中 ddl

<br>

- Bugs: "sqlserver: 不能将值 NULL 插入列 'id'" + "阻止保存要求或者重新创建表"
  Solution:

  - 数据库表邮右键设计，修改标识规范
  - [不允许保存更改的错误消息 - SQL Server | Microsoft Learn](https://learn.microsoft.com/zh-cn/troubleshoot/sql/ssms/error-when-you-save-table)
- Bugs: Error: kendo grid创建失败，请检查属性/参数设置是否正确、列设置的列数与tablehead中的列数是否一致 + Cannot read properties of undefined (reading 'replace')
  Solution: 命名需一致 （+ 有参数 undefined）
  `<!-- 'Vip, | Display: \'Customer_RankEnum\'', Or 'Vip, | Dict: \'Customer_RankEnum\'',-->`

  ``````html
  <table kendo-grid="c.grdUser" k-data-source="DataSource" options="GridOptions" pb-kendo-grid="c.vm" 
  pb-kendo-grid-cols="['Email',
      'Name_,T,70px|{style: \'text-align: center;background-color:lightgreen\'}',
      'Gender,,80px', 'RegisterDate', 'Spending', 'Vip', 'Active', ',OT', ]" 
  class="myTable">

  </table>
      ...
  <script Id="Name_-template" type="text/x-kendo-template">
      <a class="a-link" @Url.State("forward:Edit(Id:#=Id#)")>#=Name_#</a>
  </script>

  <script Id="Operation-template" type="text/x-kendo-template">
      <kendo-button type="button" icon="'k-icon k-i-delete'" ng-click="c.Delete_click(#=Id#,'#=Name_#')">
          <span translate="Delete"></span>
      </kendo-button>
  </script>
  ``````

  对于 kendo 命名 DisplayExtension.cs
  对于 Enum  命名 ListBuilder.cs
  对于标识栏 命名 zh-cn, App_Dict + Scripts/lang/dict.js
  改了一通，不知道怎么回事就过了...

> Debug: [Debug.WriteLine 方法 (System.Diagnostics) | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/api/system.diagnostics.debug.writeline?view=net-8.0)

<br>
<br>
<br>

# 新任务：加外键√，增添搜索条件√，

1. 加外键 [SQL FOREIGN KEY 约束 | 菜鸟教程 (runoob.com)](https://www.runoob.com/sql/sql-foreignkey.html)
2. 在外键的情况下，进行联动，通过 DORef 的 list 和 map 可以在第二个表中选取第一个表中的对象，BD需要提前获取RefList，否则下拉的时候无法找到 List

   ```cs
   public ActionResult Edit(int Id) {
       var m = new CustomerEditVM()
       {
           UserList = UserBD.GetRefList()
       };
       m.Input = CustomerBD.GetDto<CustomerEditVM.EditInput>(Id);
       return ForView(m);
   }
   ```
3. 添加清单视图

   > razor [ASP.NET Core 的 Razor 语法参考 | Microsoft Learn](https://learn.microsoft.com/zh-cn/aspnet/core/mvc/views/razor?view=aspnetcore-6.0) 和 cshtml 是服务器解析
   > js 是客户端解析
   >

   添加 VM， 对应上新加的 cshtml
   cshtml 的 @using (Html.KendoForm("c.frmSearch", "SearchByApproved")) 需要与 Controller 中负责处理查询按键一致

   ContentState 对应上 Controller 的初始化函数
   其中的对应关系在于 服务器 razor 与 js 客户端
4. 确认方式 AjaxSubmit内置确认

   ```js
       c.AjaxSubmit('c.frmEdit', null, { confirm: '' }, function (r) {
           if (r.IsNoop) {
               pb.AjaxNavBack();
           }
       });
   ```

   确认方式 pbui.Confirm

   ```js
       c.Delete_click = function (id, name) {
           pbui.Confirm('ConfirmDelete', name).then(confirmed => {
               if (confirmed) {
                   c.AjaxSubmit('c.frmSearch', 'Id=' + id, {url: 'Delete'}, function (r) {
                       c.grdCustomer.Bind(r.data);
                   });
               }
           });
       };

   ```

# 多方式验证√，多行编辑√，DTO查询√，增添提示确认方式√

5. 验证： `..Train\branches\Train_Gsq\ESC5.Web\Scripts\ProjectBase\Validators.js`

   - 日期：[DateTime](https://learn.microsoft.com/zh-cn/dotnet/api/system.datetime.dayofweek?view=net-6.0) 为了更灵活会操作[Custom Date Formats and the MVC Model Binder - greatrexpectations](https://greatrexpectations.com/2013/01/10/custom-date-formats-and-the-mvc-model-binder) and [Custom DateTime Model Binding with .NET and Angular | Phrase](https://phrase.com/blog/posts/custom-datetime-model-binding-net-angular/)

     ```javascript
        ValDate: function (modelValue, v) {
            console.log(c.vm.Input.RegisterDate);
            var value = modelValue || v;
            console.log(typeof value, value);
            return c.vm.Input.RegisterDate;
        },
     ```

     经受验证之后，可空字段不一定变非空字段，是因为我一开始的操作使得其字段变成了 NOT NULL，但后来又恢复了，原因不详
   - 设定一个字符型字段可空，要求验证当整型字段大于 100 时，此字符型字段必填，否则可以不填

     ```sql
     ALTER TABLE Train_Gsq.dbo.TR_Customer
     ADD OtherGender VARCHAR(10) NULL;
     ```

     1. `[ValidateWhen]` + `: X ValidatableObject, √ IValidateWhen` [Compiler Error CS0534 | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs0534?f1url=%3FappId%3Droslyn%26k%3Dk(CS0534))
     2. `X public override string? Validate()` + `√ public string ShouldValidateGroups()`
     3. 字段自定义验证 还需要在 js 的 pb 双向设置
   - 如果两个字符型字段长度之和大于整数字段值，并且布尔型字段值为true，并且日期在10天前，则验证也不通过

     - 整体验证 / 字段验证 VmInner 提示可以按一定顺序验证?
     - `ValidatableObject` and `public override string? Validate()`
       <br>
   - 多行编辑验证
   - `DTO` 构建查询

     [ASP.NET CORE 第七篇 DTOs 对象映射使用，项目部署Windows+Linux完整版 - 简书 (jianshu.com)](https://www.jianshu.com/p/575e6fd5fdd1)

     > AutoMapper是一个.NET的对象映射工具。主要作用是进行领域对象与模型（DTO）之间的转换、数据库查询结果映射至实体对象
     >

     [(DTO) | Microsoft Learn](https://learn.microsoft.com/zh-cn/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5)

     <br>
   - 如何不修改表单，临时添加任何验证方式

     <br>

# 遇到的问题

6. Debug: 在多个 viewModel 二次刷新会崩溃，原因可能是
   （1）c.grdCustomer 可能会产生网页间的关联，（2）template 的命名一定要检查规范

   `<!--c.grdCustomer 可能会产生网页间的关联-->`
7. Debug:

   > ProjectBase.Utils.NetArchException:“无法从DTO对象的属性名推断出对应的Select表达式：Name”
   >

   and Mapping VM

   对于 View Model 的 DTO 查询的理解，在 View Model 中设置的字段，与查询 VM 中的字段名一致，如果不一致，需要SelectorMap 映射出新的名称

   ```cs
       public class CustomerSectionVM
       {
           [SelectorCascade]
           public S1 Section_1 { get; set; }
           [SelectorCascade]
           public S2 Section_2 { get; set; }

           public decimal Spending { get; set; }
           public class S1
           {
               public string Email { get; set; } 
               public string Contact { get; set; }
               public string Name_ { get; set; }

               [SelectorCascade]
               public S2 Section_1_2 { get; set; }

               public static IDictionary<string, Expression<Func<Customer, object?>>> 
                   SelectorMap = new Dictionary<string, Expression<Func<Customer, object?>>> {
                   { nameof(Contact), o => o.Email }
               };
           }

           public class S2
           {
               public DateTime? RegisterDate { get; set; }
               public decimal Spending { get; set; }
           }
   }
   ```
   至于 S2 的 Spending 相当于嵌套S2中的属性，会映射回到上一层次
   此时，在 Controller 中可以通过省略高效式完成查询 `var m_1 = CustomerBD.GetDto<CustomerSectionVM>(o => o.Name_ == "hhhaa");`  这样的 DTO 查询可能需要 Selector Ignore 选择展示列
   也可以选择原始的 new 新列对象 这里，不赋值的列是 null

   ```cs
    var m_1_2 = CustomerBD.GetDto(
        o => o.Email == "1@outlook.com",                 // row select
        o => new CustomerSectionVM                       // col select
        {
            Section_1 = new CustomerSectionVM.S1
            {
                Contact = o.Email, // 相当于 Selector Map
                Name_ = o.Name_,
                Email = o.Email,
                Section_1_2 = new CustomerSectionVM.S2
                {
                    RegisterDate = o.RegisterDate,
                    Spending = o.Spending
                }
            },
        }
    );
   ```
8. [How to pass multiple models to one view in Asp.net Core (quizdeveloper.com)](https://quizdeveloper.com/tips/how-to-pass-multiple-models-to-one-view-in-aspdotnet-core-aid1215)
9. Debug:

   多行验证，保存返回提示·字段 required· 在调试 F12 网络上看到Save包证明客户端验证通过，问题在于服务端，发现

   网络 > 负载里面，因为 `NgCheckBoxFor` 的使用出现冗余列，暂时无法解决


10. Debug: 小心 GetDto 查询结果不唯一
11. Debug
   > NHibernate.HibernateException:“The dialect was unable to perform paging of a statement that requires distinct results, and is ordered by a column that is not included in the result set of the query.”
   >
   问题在于 VM 中的 Selector Map 有问题
 